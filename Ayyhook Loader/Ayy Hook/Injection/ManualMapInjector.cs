using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using ManualMapInjection.Injection.Win32;

namespace ManualMapInjection.Injection
{
    internal class ManualMapInjector
    {
        #region settings

        public bool AsyncInjection { get; set; } = false;

        public uint TimeOut { get; set; } = 5000;

        #endregion

        #region fields

        private readonly Process _process;

        private IntPtr _hProcess;

        #endregion

        #region code

        private PIMAGE_DOS_HEADER GetDosHeader(IntPtr address)
        {
            var imageDosHeader = (PIMAGE_DOS_HEADER)address;

            if (!imageDosHeader.Value.isValid)
            {
                return null;
            }

            return imageDosHeader;
        }

        private PIMAGE_NT_HEADERS32 GetNtHeader(IntPtr address)
        {
            var imageDosHeader = GetDosHeader(address);

            if (imageDosHeader == null)
            {
                return null;
            }

            var imageNtHeaders = (PIMAGE_NT_HEADERS32)(address + imageDosHeader.Value.e_lfanew);

            if (!imageNtHeaders.Value.isValid)
            {
                return null;
            }

            return imageNtHeaders;
        }

        private IntPtr RemoteAllocateMemory(uint size)
        {
            return Imports.VirtualAllocEx(_hProcess,
                UIntPtr.Zero,
                new IntPtr(size),
                Imports.AllocationType.Commit | Imports.AllocationType.Reserve,
                Imports.MemoryProtection.ExecuteReadWrite);
        }

        private IntPtr AllocateMemory(uint size)
        {
            return Imports.VirtualAlloc(IntPtr.Zero, new UIntPtr(size), Imports.AllocationType.Commit | Imports.AllocationType.Reserve,
                Imports.MemoryProtection.ExecuteReadWrite);
        }

        private IntPtr RvaToPointer(uint rva, IntPtr baseAddress)
        {
            var imageNtHeaders = GetNtHeader(baseAddress);
            if (imageNtHeaders == null)
            {
                return IntPtr.Zero;
            }

            return Imports.ImageRvaToVa(imageNtHeaders.Address, baseAddress, new UIntPtr(rva), IntPtr.Zero);
        }

        private bool InjectDependency(string dependency)
        {
            // standard LoadLibrary, CreateRemoteThread injection
            var procAddress = Imports.GetProcAddress(Imports.GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            if (procAddress == IntPtr.Zero)
            {
#if DEBUG
                Debug.WriteLine("[InjectDependency] GetProcAddress failed");
#endif
                return false;
            }

            var lpAddress = RemoteAllocateMemory((uint)dependency.Length);

            if (lpAddress == IntPtr.Zero)
            {
#if DEBUG
                Debug.WriteLine("[InjectDependency] RemoteAllocateMemory failed");
#endif
                return false;
            }

            var buffer = Encoding.ASCII.GetBytes(dependency);

            UIntPtr bytesWritten;
            var result = Imports.WriteProcessMemory(_hProcess, lpAddress, buffer, buffer.Length, out bytesWritten);

            if (result)
            {
                var hHandle = Imports.CreateRemoteThread(_hProcess, IntPtr.Zero, 0, procAddress, lpAddress, 0, IntPtr.Zero);

                if (Imports.WaitForSingleObject(hHandle, TimeOut) != 0)
                {
#if DEBUG
                    Debug.WriteLine("[InjectDependency] remote thread not signaled");
#endif
                    return false;
                }
            }
#if DEBUG
            else
            {
                Debug.WriteLine("[InjectDependency] WriteProcessMemory failed");
            }
#endif

            Imports.VirtualFreeEx(_hProcess, lpAddress, 0, Imports.FreeType.Release);
            return result;
        }

        private IntPtr GetRemoteModuleHandleA(string module)
        {
            var dwModuleHandle = IntPtr.Zero;
            var hHeap = Imports.GetProcessHeap();
            var dwSize = (uint)Marshal.SizeOf(typeof(PROCESS_BASIC_INFORMATION));
            var pbi = (PPROCESS_BASIC_INFORMATION)Imports.HeapAlloc(hHeap, /*HEAP_ZERO_MEMORY*/ 0x8, new UIntPtr(dwSize));

            uint dwSizeNeeded;
            var dwStatus = Imports.NtQueryInformationProcess(_hProcess, /*ProcessBasicInformation*/ 0, pbi.Address, dwSize, out dwSizeNeeded);

            if (dwStatus >= 0 && dwSize < dwSizeNeeded)
            {
                if (pbi != null)
                {
                    Imports.HeapFree(hHeap, 0, pbi.Address);
                }

                pbi = (PPROCESS_BASIC_INFORMATION)Imports.HeapAlloc(hHeap, /*HEAP_ZERO_MEMORY*/ 0x8, new UIntPtr(dwSize));

                if (pbi == null)
                {
#if DEBUG
                    Debug.WriteLine("[GetRemoteModuleHandleA] Couldn't allocate heap buffer");
#endif
                    return IntPtr.Zero; //Couldn't allocate heap buffer
                }

                dwStatus = Imports.NtQueryInformationProcess(_hProcess, /*ProcessBasicInformation*/ 0, pbi.Address, dwSizeNeeded, out dwSizeNeeded);
            }

            if (dwStatus >= 0)
            {
                if (pbi.Value.PebBaseAddress != IntPtr.Zero)
                {
                    UIntPtr dwBytesRead;
                    uint pebLdrAddress;

                    if (Imports.ReadProcessMemory(_hProcess, pbi.Value.PebBaseAddress + 12 /*peb.Ldr*/, out pebLdrAddress, out dwBytesRead))
                    {
                        var pLdrListHead = pebLdrAddress + /*InLoadOrderModuleList*/ 0x0C;
                        var pLdrCurrentNode = pebLdrAddress + /*InLoadOrderModuleList*/ 0x0C;

                        do
                        {
                            uint lstEntryAddress;
                            if (!Imports.ReadProcessMemory(_hProcess, new IntPtr(pLdrCurrentNode), out lstEntryAddress, out dwBytesRead))
                            {
                                Imports.HeapFree(hHeap, 0, pbi.Address);
                            }
                            pLdrCurrentNode = lstEntryAddress;

                            UNICODE_STRING baseDllName;
                            Imports.ReadProcessMemory(_hProcess, new IntPtr(lstEntryAddress) + /*BaseDllName*/ 0x2C, out baseDllName, out dwBytesRead);

                            var strBaseDllName = string.Empty;

                            if (baseDllName.Length > 0)
                            {
                                var buffer = new byte[baseDllName.Length];
                                Imports.ReadProcessMemory(_hProcess, baseDllName.Buffer, buffer, out dwBytesRead);
                                strBaseDllName = Encoding.Unicode.GetString(buffer);
                            }

                            uint dllBase;
                            uint sizeOfImage;

                            Imports.ReadProcessMemory(_hProcess, new IntPtr(lstEntryAddress) + /*DllBase*/ 0x18, out dllBase, out dwBytesRead);
                            Imports.ReadProcessMemory(_hProcess, new IntPtr(lstEntryAddress) + /*SizeOfImage*/ 0x20, out sizeOfImage, out dwBytesRead);

                            if (dllBase != 0 && sizeOfImage != 0)
                            {
                                if (string.Equals(strBaseDllName, module, StringComparison.OrdinalIgnoreCase))
                                {
                                    dwModuleHandle = new IntPtr(dllBase);
                                    break;
                                }
                            }

                        } while (pLdrListHead != pLdrCurrentNode);
                    }
                }
            }

            if (pbi != null)
            {
                Imports.HeapFree(hHeap, 0, pbi.Address);
            }

            return dwModuleHandle;
        }

        private IntPtr GetDependencyProcAddressA(IntPtr moduleBase, PCHAR procName)
        {
            IntPtr pFunc = IntPtr.Zero;
            IMAGE_DOS_HEADER hdrDos;
            IMAGE_NT_HEADERS32 hdrNt32;

            UIntPtr dwRead;
            Imports.ReadProcessMemory(_hProcess, moduleBase, out hdrDos, out dwRead);

            if (!hdrDos.isValid)
            {
                return IntPtr.Zero;
            }

            Imports.ReadProcessMemory(_hProcess, moduleBase + hdrDos.e_lfanew, out hdrNt32, out dwRead);

            if (!hdrNt32.isValid)
            {
                return IntPtr.Zero;
            }

            var expBase = hdrNt32.OptionalHeader.ExportTable.VirtualAddress;
            if (expBase > 0)
            {
                var expSize = hdrNt32.OptionalHeader.ExportTable.Size;
                var expData = (PIMAGE_EXPORT_DIRECTORY)AllocateMemory(expSize);
                Imports.ReadProcessMemory(_hProcess, moduleBase + (int)expBase, expData.Address, (int)expSize, out dwRead);

                var pAddressOfOrds = (PWORD)(expData.Address + (int)expData.Value.AddressOfNameOrdinals - (int)expBase);
                var pAddressOfNames = (PDWORD)(expData.Address + (int)expData.Value.AddressOfNames - (int)expBase);
                var pAddressOfFuncs = (PDWORD)(expData.Address + (int)expData.Value.AddressOfFunctions - (int)expBase);


                for (uint i = 0; i < expData.Value.NumberOfFunctions; i++)
                {
                    ushort ordIndex;
                    PCHAR pName = null;

                    if (new PDWORD(procName.Address).Value <= 0xFFFF)
                    {
                        ordIndex = unchecked((ushort)i);
                    }
                    else if (new PDWORD(procName.Address).Value > 0xFFFF && i < expData.Value.NumberOfNames)
                    {
                        pName = (PCHAR)new IntPtr(pAddressOfNames[i] + expData.Address.ToInt32() - expBase);
                        ordIndex = pAddressOfOrds[i];
                    }
                    else
                    {
                        return IntPtr.Zero;
                    }

                    if ((new PDWORD(procName.Address).Value <= 0xFFFF && new PDWORD(procName.Address).Value == ordIndex + expData.Value.Base) || (new PDWORD(procName.Address).Value > 0xFFFF && pName.ToString() == procName.ToString()))
                    {
                        pFunc = moduleBase + (int)pAddressOfFuncs[ordIndex];

                        if (pFunc.ToInt64() >= (moduleBase + (int)expBase).ToInt64() && pFunc.ToInt64() <= (moduleBase + (int)expBase + (int)expSize).ToInt64())
                        {
                            var forwardStr = new byte[255];
                            Imports.ReadProcessMemory(_hProcess, pFunc, forwardStr, out dwRead);

                            var chainExp = Helpers.ToStringAnsi(forwardStr);

                            var strDll = chainExp.Substring(0, chainExp.IndexOf(".")) + ".dll";
                            var strName = chainExp.Substring(chainExp.IndexOf(".") + 1);

                            var hChainMod = GetRemoteModuleHandleA(strDll);
                            if (hChainMod == IntPtr.Zero)
                            {
                                // todo
                                //hChainMod = LoadDependencyA(strDll.c_str());
                                InjectDependency(strDll);
                            }

                            if (strName.StartsWith("#"))
                            {
                                pFunc = GetDependencyProcAddressA(hChainMod, new PCHAR(strName) + 1);
                            }
                            else
                            {
                                pFunc = GetDependencyProcAddressA(hChainMod, new PCHAR(strName));
                            }
                        }

                        break;
                    }
                }

                Imports.VirtualFree(expData.Address, 0, Imports.FreeType.Release);
            }

            return pFunc;
        }

        private bool ProcessImportTable(IntPtr baseAddress)
        {
            var imageNtHeaders = GetNtHeader(baseAddress);

            if (imageNtHeaders == null)
            {
                return false;
            }

            if (imageNtHeaders.Value.OptionalHeader.ImportTable.Size > 0)
            {
                var imageImportDescriptor = (PIMAGE_IMPORT_DESCRIPTOR)RvaToPointer(imageNtHeaders.Value.OptionalHeader.ImportTable.VirtualAddress, baseAddress);

                if (imageImportDescriptor != null)
                {
                    for (; imageImportDescriptor.Value.Name > 0; imageImportDescriptor++)
                    {
                        var moduleName = (PCHAR)RvaToPointer(imageImportDescriptor.Value.Name, baseAddress);
                        if (moduleName == null)
                        {
                            continue;
                        }

                        if (moduleName.ToString().Contains("-ms-win-crt-"))
                        {
                            moduleName = new PCHAR("ucrtbase.dll");
                        }

                        var moduleBase = GetRemoteModuleHandleA(moduleName.ToString());
                        if (moduleBase == IntPtr.Zero)
                        {
                            // todo manual map injection for dependency
                            InjectDependency(moduleName.ToString());
                            moduleBase = GetRemoteModuleHandleA(moduleName.ToString());

                            if (moduleBase == IntPtr.Zero)
                            {
#if DEBUG
                                Debug.WriteLine("[ProcessImportTable] failed to obtain module handle");
#endif
                                // failed to obtain module handle
                                continue;
                            }
                        }

                        PIMAGE_THUNK_DATA imageThunkData;
                        PIMAGE_THUNK_DATA imageFuncData;

                        if (imageImportDescriptor.Value.OriginalFirstThunk > 0)
                        {
                            imageThunkData = (PIMAGE_THUNK_DATA)RvaToPointer(imageImportDescriptor.Value.OriginalFirstThunk, baseAddress);
                            imageFuncData = (PIMAGE_THUNK_DATA)RvaToPointer(imageImportDescriptor.Value.FirstThunk, baseAddress);
                        }
                        else
                        {
                            imageThunkData = (PIMAGE_THUNK_DATA)RvaToPointer(imageImportDescriptor.Value.FirstThunk, baseAddress);
                            imageFuncData = (PIMAGE_THUNK_DATA)RvaToPointer(imageImportDescriptor.Value.FirstThunk, baseAddress);
                        }

                        for (; imageThunkData.Value.AddressOfData > 0; imageThunkData++, imageFuncData++)
                        {
                            IntPtr functionAddress;
                            var bSnapByOrdinal = (imageThunkData.Value.Ordinal & /*IMAGE_ORDINAL_FLAG32*/ 0x80000000) != 0;

                            if (bSnapByOrdinal)
                            {
                                var ordinal = (short)(imageThunkData.Value.Ordinal & 0xffff);
                                functionAddress = GetDependencyProcAddressA(moduleBase, new PCHAR(ordinal));

                                if (functionAddress == IntPtr.Zero)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                var imageImportByName = (PIMAGE_IMPORT_BY_NAME)RvaToPointer(imageFuncData.Value.Ordinal, baseAddress);
                                var mameOfImport = (PCHAR)imageImportByName.Address + /*imageImportByName->Name*/ 2;
                                functionAddress = GetDependencyProcAddressA(moduleBase, mameOfImport);
                            }

                            //ImageFuncData->u1.Function = (size_t)FunctionAddress;
                            Marshal.WriteInt32(imageFuncData.Address, functionAddress.ToInt32());
                        }
                    }

                    return true;
                }
                else
                {
#if DEBUG
                    Debug.WriteLine("[ProcessImportTable] Size of table confirmed but pointer to data invalid");
#endif
                    // Size of table confirmed but pointer to data invalid
                    return false;
                }
            }
            else
            {
#if DEBUG
                Debug.WriteLine("[ProcessImportTable] no imports");
#endif
                // no imports
                return true;
            }
        }

        private bool ProcessDelayedImportTable(IntPtr baseAddress, IntPtr remoteAddress)
        {
            var imageNtHeaders = GetNtHeader(baseAddress);

            if (imageNtHeaders == null)
            {
                return false;
            }

            if (imageNtHeaders.Value.OptionalHeader.DelayImportDescriptor.Size > 0)
            {
                var imageDelayedImportDescriptor =
                    (PIMAGE_IMPORT_DESCRIPTOR)RvaToPointer(imageNtHeaders.Value.OptionalHeader.DelayImportDescriptor.VirtualAddress, baseAddress);

                if (imageDelayedImportDescriptor != null)
                {
                    for (; imageDelayedImportDescriptor.Value.Name > 0; imageDelayedImportDescriptor++)
                    {
                        var moduleName = (PCHAR)RvaToPointer(imageDelayedImportDescriptor.Value.Name, baseAddress);
                        if (moduleName == null)
                        {
                            continue;
                        }

                        var moduleBase = GetRemoteModuleHandleA(moduleName.ToString());
                        if (moduleBase == IntPtr.Zero)
                        {
                            // todo manual map injection for dependency
                            InjectDependency(moduleName.ToString());
                            moduleBase = GetRemoteModuleHandleA(moduleName.ToString());

                            if (moduleBase == IntPtr.Zero)
                            {
#if DEBUG
                                Debug.WriteLine("[ProcessDelayedImportTable] no imports");
#endif
                                // failed to obtain module handle
                                continue;
                            }
                        }

                        PIMAGE_THUNK_DATA imageThunkData = null;
                        PIMAGE_THUNK_DATA imageFuncData = null;

                        if (imageDelayedImportDescriptor.Value.OriginalFirstThunk > 0)
                        {
                            imageThunkData = (PIMAGE_THUNK_DATA)RvaToPointer(imageDelayedImportDescriptor.Value.OriginalFirstThunk, baseAddress);
                            imageFuncData = (PIMAGE_THUNK_DATA)RvaToPointer(imageDelayedImportDescriptor.Value.FirstThunk, baseAddress);
                        }
                        else
                        {
                            imageThunkData = (PIMAGE_THUNK_DATA)RvaToPointer(imageDelayedImportDescriptor.Value.FirstThunk, baseAddress);
                            imageFuncData = (PIMAGE_THUNK_DATA)RvaToPointer(imageDelayedImportDescriptor.Value.FirstThunk, baseAddress);
                        }

                        for (; imageThunkData.Value.AddressOfData > 0; imageThunkData++, imageFuncData++)
                        {
                            IntPtr functionAddress;
                            var bSnapByOrdinal = ((imageThunkData.Value.Ordinal & /*IMAGE_ORDINAL_FLAG32*/ 0x80000000) != 0);

                            if (bSnapByOrdinal)
                            {
                                var ordinal = (short)(imageThunkData.Value.Ordinal & 0xffff);
                                functionAddress = GetDependencyProcAddressA(moduleBase, new PCHAR(ordinal));

                                if (functionAddress == IntPtr.Zero)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                var imageImportByName = (PIMAGE_IMPORT_BY_NAME)RvaToPointer(imageFuncData.Value.Ordinal, baseAddress);
                                var mameOfImport = (PCHAR)imageImportByName.Address + /*imageImportByName->Name*/ 2;
                                functionAddress = GetDependencyProcAddressA(moduleBase, mameOfImport);
                            }

                            //ImageFuncData->u1.Function = (size_t)FunctionAddress;
                            Marshal.WriteInt32(imageFuncData.Address, functionAddress.ToInt32());
                        }
                    }

                    return true;
                }
                else
                {
#if DEBUG
                    Debug.WriteLine("[ProcessDelayedImportTable] Size of table confirmed but pointer to data invalid");
#endif
                    // Size of table confirmed but pointer to data invalid
                    return false;
                }
            }
            else
            {
                // no imports
                return true;
            }
        }

        private bool ProcessRelocation(uint imageBaseDelta, ushort data, PBYTE relocationBase)
        {
            var bReturn = true;
            PSHORT raw;
            PDWORD raw2;

            switch ((data >> 12) & 0xF)
            {
                case 1: // IMAGE_REL_BASED_HIGH
                    raw = (PSHORT)(relocationBase + (data & 0xFFF)).Address;
                    Marshal.WriteInt16(raw.Address, unchecked((short)(raw.Value + (uint)((ushort)((imageBaseDelta >> 16) & 0xffff)))));
                    break;

                case 2: // IMAGE_REL_BASED_LOW
                    raw = (PSHORT)(relocationBase + (data & 0xFFF)).Address;
                    Marshal.WriteInt16(raw.Address, unchecked((short)(raw.Value + (uint)((ushort)(imageBaseDelta & 0xffff)))));
                    break;

                case 3: // IMAGE_REL_BASED_HIGHLOW
                    raw2 = (PDWORD)(relocationBase + (data & 0xFFF)).Address;
                    Marshal.WriteInt32(raw2.Address, unchecked((int)(raw2.Value + imageBaseDelta)));
                    break;

                case 10: // IMAGE_REL_BASED_DIR64
                    raw2 = (PDWORD)(relocationBase + (data & 0xFFF)).Address;
                    Marshal.WriteInt32(raw2.Address, unchecked((int)(raw2.Value + imageBaseDelta)));
                    break;

                case 0: // IMAGE_REL_BASED_ABSOLUTE
                    break;

                case 4: // IMAGE_REL_BASED_HIGHADJ
                    break;

                default:
                    bReturn = false;
                    break;
            }

            return bReturn;
        }

        private bool ProcessRelocations(IntPtr baseAddress, IntPtr remoteAddress)
        {
            var imageNtHeaders = GetNtHeader(baseAddress);

            if (imageNtHeaders == null)
            {
                return false;
            }

            if ((imageNtHeaders.Value.FileHeader.Characteristics & /*IMAGE_FILE_RELOCS_STRIPPED*/ 0x01) > 0)
            {
                return true;
            }
            else
            {
                var imageBaseDelta = (uint)(remoteAddress.ToInt32() - imageNtHeaders.Value.OptionalHeader.ImageBase);
                var relocationSize = imageNtHeaders.Value.OptionalHeader.BaseRelocationTable.Size;

                if (relocationSize > 0)
                {
                    var relocationDirectory = (PIMAGE_BASE_RELOCATION)RvaToPointer(imageNtHeaders.Value.OptionalHeader.BaseRelocationTable.VirtualAddress, baseAddress);

                    if (relocationDirectory != null)
                    {
                        var relocationEnd = (PBYTE)relocationDirectory.Address + (int)relocationSize;

                        while (relocationDirectory.Address.ToInt64() < relocationEnd.Address.ToInt64())
                        {
                            var relocBase = (PBYTE)RvaToPointer(relocationDirectory.Value.VirtualAddress, baseAddress);
                            var numRelocs = (relocationDirectory.Value.SizeOfBlock - 8) >> 1;
                            var relocationData = (PWORD)((relocationDirectory + 1).Address);

                            for (uint i = 0; i < numRelocs; i++, relocationData++)
                            {
                                ProcessRelocation(imageBaseDelta, relocationData.Value, relocBase);
                            }

                            relocationDirectory = (PIMAGE_BASE_RELOCATION)relocationData.Address;
                        }
                    }
                    else
                    {
                        return false;
                    }

                }
            }

            return true;
        }

        private uint GetSectionProtection(DataSectionFlags characteristics)
        {
            uint result = 0;
            if (characteristics.HasFlag(DataSectionFlags.MemoryNotCached))
                result |= /*PAGE_NOCACHE*/ 0x200;

            if (characteristics.HasFlag(DataSectionFlags.MemoryExecute))
            {
                if (characteristics.HasFlag(DataSectionFlags.MemoryRead))
                {
                    if (characteristics.HasFlag(DataSectionFlags.MemoryWrite))
                        result |= /*PAGE_EXECUTE_READWRITE*/ 0x40;
                    else
                        result |= /*PAGE_EXECUTE_READ*/ 0x20;
                }
                else if (characteristics.HasFlag(DataSectionFlags.MemoryWrite))
                    result |= /*PAGE_EXECUTE_WRITECOPY*/ 0x80;
                else
                    result |= /*PAGE_EXECUTE*/ 0x10;
            }
            else if (characteristics.HasFlag(DataSectionFlags.MemoryRead))
            {
                if (characteristics.HasFlag(DataSectionFlags.MemoryWrite))
                    result |= /*PAGE_READWRITE*/ 0x04;
                else
                    result |= /*PAGE_READONLY*/ 0x02;
            }
            else if (characteristics.HasFlag(DataSectionFlags.MemoryWrite))
                result |= /*PAGE_WRITECOPY*/ 0x08;
            else
                result |= /*PAGE_NOACCESS*/ 0x01;

            return result;
        }

        private bool ProcessSection(char[] name, IntPtr baseAddress, IntPtr remoteAddress, ulong rawData, ulong virtualAddress, ulong rawSize, ulong virtualSize, uint protectFlag)
        {
            UIntPtr lpNumberOfBytesWritten;
            uint dwOldProtect;

            if (
                !Imports.WriteProcessMemory(_hProcess, new IntPtr(remoteAddress.ToInt64() + (long)virtualAddress), new IntPtr(baseAddress.ToInt64() + (long)rawData),
                    new IntPtr((long)rawSize), out lpNumberOfBytesWritten))
            {
                return false;
            }

            if (!Imports.VirtualProtectEx(_hProcess, new IntPtr(remoteAddress.ToInt64() + (long)virtualAddress), new UIntPtr(virtualSize), protectFlag, out dwOldProtect))
            {
                return false;
            }

            return true;
        }

        private bool ProcessSections(IntPtr baseAddress, IntPtr remoteAddress)
        {
            var imageNtHeaders = GetNtHeader(baseAddress);

            if (imageNtHeaders == null)
            {
                return false;
            }

            // skip PE header

            var imageSectionHeader = (PIMAGE_SECTION_HEADER)(imageNtHeaders.Address + /*OptionalHeader*/ 24 + imageNtHeaders.Value.FileHeader.SizeOfOptionalHeader);
            for (ushort i = 0; i < imageNtHeaders.Value.FileHeader.NumberOfSections; i++)
            {
                if (Helpers._stricmp(".reloc".ToCharArray(), imageSectionHeader[i].Name))
                {
                    continue;
                }

                var characteristics = imageSectionHeader[i].Characteristics;

                if (characteristics.HasFlag(DataSectionFlags.MemoryRead) || characteristics.HasFlag(DataSectionFlags.MemoryWrite) || characteristics.HasFlag(DataSectionFlags.MemoryExecute))
                {
                    var protection = GetSectionProtection(imageSectionHeader[i].Characteristics);
                    ProcessSection(imageSectionHeader[i].Name, baseAddress, remoteAddress, imageSectionHeader[i].PointerToRawData,
                        imageSectionHeader[i].VirtualAddress, imageSectionHeader[i].SizeOfRawData, imageSectionHeader[i].VirtualSize, protection);
                }
            }

            return true;
        }

        private bool ExecuteRemoteThreadBuffer(byte[] threadData, bool async)
        {
            var lpAddress = RemoteAllocateMemory((uint)threadData.Length);


            if (lpAddress == IntPtr.Zero)
            {
                return false;
            }

            UIntPtr bytesWritten;
            var result = Imports.WriteProcessMemory(_hProcess, lpAddress, threadData, threadData.Length, out bytesWritten);

            if (result)
            {
                var hHandle = Imports.CreateRemoteThread(_hProcess, IntPtr.Zero, 0, lpAddress, IntPtr.Zero, 0, IntPtr.Zero);

                if (async)
                {
                    var t = new Thread(() =>
                    {
                        Imports.WaitForSingleObject(hHandle, 5000);
                        Imports.VirtualFreeEx(_hProcess, lpAddress, 0, Imports.FreeType.Release);
                    })
                    { IsBackground = true };
                    t.Start();
                }
                else
                {
                    Imports.WaitForSingleObject(hHandle, 4000);
                    Imports.VirtualFreeEx(_hProcess, lpAddress, 0, Imports.FreeType.Release);
                }
            }

            return result;
        }

        private bool CallEntryPoint(IntPtr baseAddress, uint entrypoint, bool async)
        {
            var buffer = new List<byte>();
            buffer.Add(0x68);
            buffer.AddRange(BitConverter.GetBytes(baseAddress.ToInt32()));
            buffer.Add(0x68);
            buffer.AddRange(BitConverter.GetBytes(/*DLL_PROCESS_ATTACH*/1));
            buffer.Add(0x68);
            buffer.AddRange(BitConverter.GetBytes(0));
            buffer.Add(0xB8);
            buffer.AddRange(BitConverter.GetBytes(entrypoint));
            buffer.Add(0xFF);
            buffer.Add(0xD0);
            buffer.Add(0x33);
            buffer.Add(0xC0);
            buffer.Add(0xC2);
            buffer.Add(0x04);
            buffer.Add(0x00);

            return ExecuteRemoteThreadBuffer(buffer.ToArray(), async);
        }

        private bool ProcessTlsEntries(IntPtr baseAddress, IntPtr remoteAddress)
        {
            UIntPtr dwRead;
            var imageNtHeaders = GetNtHeader(baseAddress);

            if (imageNtHeaders == null)
            {
                return false;
            }

            if (imageNtHeaders.Value.OptionalHeader.TLSTable.Size == 0)
            {
                return true;
            }

            var tlsDirectory = (PIMAGE_TLS_DIRECTORY32)RvaToPointer(imageNtHeaders.Value.OptionalHeader.TLSTable.VirtualAddress, baseAddress);

            if (tlsDirectory == null)
            {
                return true;
            }

            if (tlsDirectory.Value.AddressOfCallBacks == 0)
            {
                return true;
            }

            var buffer = new byte[0xFF * 4];
            if (!Imports.ReadProcessMemory(_hProcess, new IntPtr(tlsDirectory.Value.AddressOfCallBacks), buffer, out dwRead))
            {
                return false;
            }

            var tLSCallbacks = new PDWORD(buffer);
            var result = true;

            for (uint i = 0; tLSCallbacks[i] > 0; i++)
            {
                result = CallEntryPoint(remoteAddress, tLSCallbacks[i], false);

                if (!result)
                {
                    break;
                }
            }

            return result;
        }

        private IntPtr LoadImageToMemory(IntPtr baseAddress)
        {
            var imageNtHeaders = GetNtHeader(baseAddress);

            if (imageNtHeaders == null)
            {
#if DEBUG
                Debug.WriteLine("[LoadImageToMemory] Invalid Image: No IMAGE_NT_HEADERS");
#endif
                // Invalid Image: No IMAGE_NT_HEADERS
                return IntPtr.Zero;
            }

            if (imageNtHeaders.Value.FileHeader.NumberOfSections == 0)
            {
#if DEBUG
                Debug.WriteLine("[LoadImageToMemory] Invalid Image: No Sections");
#endif
                // Invalid Image: No Sections
                return IntPtr.Zero;
            }

            var rvaLow = unchecked((uint)-1);
            var rvaHigh = 0u;
            var imageSectionHeader = (PIMAGE_SECTION_HEADER)(imageNtHeaders.Address + /*OptionalHeader*/
            24 + imageNtHeaders.Value.FileHeader.SizeOfOptionalHeader);

            for (uint i = 0; i < imageNtHeaders.Value.FileHeader.NumberOfSections; i++)
            {
                if (imageSectionHeader[i].VirtualSize == 0)
                {
                    continue;
                }

                if (imageSectionHeader[i].VirtualAddress < rvaLow)
                {
                    rvaLow = imageSectionHeader[i].VirtualAddress;
                }

                if (imageSectionHeader[i].VirtualAddress + imageSectionHeader[i].VirtualSize > rvaHigh)
                {
                    rvaHigh = imageSectionHeader[i].VirtualAddress + imageSectionHeader[i].VirtualSize;
                }
            }

            var imageSize = rvaHigh - rvaLow;

            if (imageNtHeaders.Value.OptionalHeader.ImageBase % 4096 != 0)
            {
#if DEBUG
                Debug.WriteLine("[LoadImageToMemory] Invalid Image: Not Page Aligned");
#endif
                // Invalid Image: Not Page Aligned
                return IntPtr.Zero;
            }

            if (imageNtHeaders.Value.OptionalHeader.DelayImportDescriptor.Size > 0)
            {
#if DEBUG
                Debug.WriteLine("[LoadImageToMemory] This method is not supported for Managed executables");
#endif
                // This method is not supported for Managed executables
                return IntPtr.Zero;
            }

            var allocatedRemoteMemory = RemoteAllocateMemory(imageSize);
            if (allocatedRemoteMemory == IntPtr.Zero)
            {
#if DEBUG
                Debug.WriteLine("[LoadImageToMemory] Failed to allocate remote memory for module");
#endif
                // Failed to allocate remote memory for module
                return IntPtr.Zero;
            }

            if (!ProcessImportTable(baseAddress))
            {
#if DEBUG
                Debug.WriteLine("[LoadImageToMemory] Failed to fix imports");
#endif
                // Failed to fix imports
                return IntPtr.Zero;
            }

            if (!ProcessDelayedImportTable(baseAddress, allocatedRemoteMemory))
            {
#if DEBUG
                Debug.WriteLine("[LoadImageToMemory] Failed to fix delayed imports");
#endif
                // Failed to fix delayed imports
                return IntPtr.Zero;
            }

            if (!ProcessRelocations(baseAddress, allocatedRemoteMemory))
            {
#if DEBUG
                Debug.WriteLine("[LoadImageToMemory] Failed to process relocations");
#endif
                // Failed to process relocations
                return IntPtr.Zero;
            }

            if (!ProcessSections(baseAddress, allocatedRemoteMemory))
            {
#if DEBUG
                Debug.WriteLine("[LoadImageToMemory] Failed to process sections");
#endif
                // Failed to process sections
                return IntPtr.Zero;
            }

            if (!ProcessTlsEntries(baseAddress, allocatedRemoteMemory))
            {
#if DEBUG
                Debug.WriteLine("[LoadImageToMemory] ProcessTlsEntries Failed");
#endif
                // ProcessTlsEntries Failed
                return IntPtr.Zero;
            }

            if (imageNtHeaders.Value.OptionalHeader.AddressOfEntryPoint > 0)
            {
                var dllEntryPoint = allocatedRemoteMemory.ToInt32() + (int)imageNtHeaders.Value.OptionalHeader.AddressOfEntryPoint;

                if (!CallEntryPoint(allocatedRemoteMemory, (uint)dllEntryPoint, AsyncInjection))
                {
#if DEBUG
                    Debug.WriteLine("[LoadImageToMemory] Failed to call entrypoint");
#endif
                    return IntPtr.Zero;
                }
            }

            return allocatedRemoteMemory;
        }

        private GCHandle PinBuffer(byte[] buffer)
        {
            return GCHandle.Alloc(buffer, GCHandleType.Pinned);
        }

        private void FreeHandle(GCHandle handle)
        {
            if (handle.IsAllocated)
            {
                handle.Free();
            }
        }

        private void OpenTarget()
        {
            _hProcess = Imports.OpenProcess(_process, Imports.ProcessAccessFlags.All);

            if (_hProcess == IntPtr.Zero)
            {
                throw new Exception($"Failed to open handle. Error {Marshal.GetLastWin32Error()}");
            }
        }

        private void CloseTarget()
        {
            if (_hProcess != IntPtr.Zero)
            {
                Imports.CloseHandle(_hProcess);
                _hProcess = IntPtr.Zero;
            }
        }

        #endregion

        #region API

        public IntPtr Inject(byte[] buffer)
        {
            var handle = new GCHandle();

            // clone buffer
            buffer = buffer.ToArray();

            var result = IntPtr.Zero;

            try
            {
                // verify target
                if (_process == null || _process.HasExited)
                {
                    return result;
                }

                //
                handle = PinBuffer(buffer);
                OpenTarget();

                // inject
                result = LoadImageToMemory(handle.AddrOfPinnedObject());
            }
#pragma warning disable CS0168
            catch (Exception e)
#pragma warning restore CS0168
            {
#if DEBUG
                Debug.WriteLine($"Unexpected error {e}");
#endif
            }
            finally
            {
                // close stuff
                FreeHandle(handle);
                CloseTarget();
            }

            return result;
        }

        public IntPtr Inject(string file)
        {
            return Inject(File.ReadAllBytes(file));
        }

        #endregion

        public ManualMapInjector(Process p)
        {
            _process = p;
        }
    }
}