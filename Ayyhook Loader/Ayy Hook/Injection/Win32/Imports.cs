using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace ManualMapInjection.Injection.Win32
{
    [SuppressUnmanagedCodeSecurity]
    internal static class Imports
    {
        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        [Flags]
        public enum FreeType
        {
            Decommit = 0x4000,
            Release = 0x8000,
        }

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, UIntPtr lpAddress, IntPtr dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAlloc(IntPtr lpAddress, UIntPtr dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle,int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("Dbghelp.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr ImageRvaToVa(IntPtr NtHeaders, IntPtr Base, UIntPtr Rva, [Optional] IntPtr LastRvaSection);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcessHeap();

        [DllImport("kernel32.dll", SetLastError = false)]
        public static extern IntPtr HeapAlloc(IntPtr hHeap, uint dwFlags, UIntPtr dwBytes);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool HeapFree(IntPtr hHeap, uint dwFlags, IntPtr lpMem);

        [DllImport("NTDLL.DLL", SetLastError = true)]
        public static extern int NtQueryInformationProcess(IntPtr hProcess, int pic, IntPtr pbi, uint cb, out uint pSize);

        //[DllImport("kernel32.dll", SetLastError = true)]
        //public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, uint dwSize, out UIntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int dwSize, out UIntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, IntPtr nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, FreeType dwFreeType);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualFree(IntPtr lpAddress, int dwSize, FreeType dwFreeType);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll")]
        public static extern void GetSystemTimeAsFileTime(out FILETIME lpSystemTimeAsFileTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        // wrappers
        public static bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, out UIntPtr lpNumberOfBytesRead)
        {
            var handle = GCHandle.Alloc(lpBuffer, GCHandleType.Pinned);
            var result = ReadProcessMemory(hProcess, lpBaseAddress, handle.AddrOfPinnedObject(), lpBuffer.Length, out lpNumberOfBytesRead);
            handle.Free();
            return result;
        }

        public static bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out uint lpBuffer, out UIntPtr lpNumberOfBytesRead)
        {
            var buffer = new byte[4];
            var result = ReadProcessMemory(hProcess, lpBaseAddress, buffer, out lpNumberOfBytesRead);
            lpBuffer = BitConverter.ToUInt32(buffer, 0);
            return result;
        }

        public static bool ReadProcessMemory<T>(IntPtr hProcess, IntPtr lpBaseAddress, out T lpBuffer, out UIntPtr lpNumberOfBytesRead) where T : struct
        {
            var buffer = new byte[Marshal.SizeOf(typeof(T))];
            var result = ReadProcessMemory(hProcess, lpBaseAddress, buffer, out lpNumberOfBytesRead);
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            lpBuffer = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            handle.Free();

            return result;
        }

        public static IntPtr OpenProcess(Process proc, ProcessAccessFlags flags)
        {
            return OpenProcess(flags, false, proc.Id);
        }
    }
}