using System;
using System.Runtime.InteropServices;
using System.Text;
using ManualMapInjection.Injection.Types;

namespace ManualMapInjection.Injection.Win32
{
    public class PIMAGE_DOS_HEADER : ManagedPtr<IMAGE_DOS_HEADER>
    {
        public PIMAGE_DOS_HEADER(IntPtr address) : base(address)
        {
        }

        public PIMAGE_DOS_HEADER(object value) : base(value)
        {
        }

        public static explicit operator PIMAGE_DOS_HEADER(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            return new PIMAGE_DOS_HEADER(ptr);
        }
    }

    public class PIMAGE_NT_HEADERS32 : ManagedPtr<IMAGE_NT_HEADERS32>
    {
        public PIMAGE_NT_HEADERS32(IntPtr address) : base(address)
        {
        }

        public PIMAGE_NT_HEADERS32(object value) : base(value)
        {
        }

        public static explicit operator PIMAGE_NT_HEADERS32(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            return new PIMAGE_NT_HEADERS32(ptr);
        }
    }

    public class PCHAR : ManagedPtr<char>
    {
        public PCHAR(IntPtr address) : base(address)
        {
        }

        public PCHAR(object value) : base(value)
        {
        }

        public PCHAR(string value) : base(Encoding.UTF8.GetBytes(value))
        {
        }

        public static PCHAR operator +(PCHAR c1, int c2)
        {
            return new PCHAR(c1.Address + c2 * c1.StructSize);
        }

        public static PCHAR operator ++(PCHAR a)
        {
            return a + 1;
        }

        public static explicit operator PCHAR(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            return new PCHAR(ptr);
        }

        public override string ToString()
        {
            return Marshal.PtrToStringAnsi(Address) ?? string.Empty;
        }
    }

    public class PPROCESS_BASIC_INFORMATION : ManagedPtr<PROCESS_BASIC_INFORMATION>
    {
        public PPROCESS_BASIC_INFORMATION(IntPtr address) : base(address)
        {
        }

        public PPROCESS_BASIC_INFORMATION(object value) : base(value)
        {
        }

        public static explicit operator PPROCESS_BASIC_INFORMATION(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            return new PPROCESS_BASIC_INFORMATION(ptr);
        }
    }

    public class PIMAGE_IMPORT_DESCRIPTOR : ManagedPtr<IMAGE_IMPORT_DESCRIPTOR>
    {
        public PIMAGE_IMPORT_DESCRIPTOR(IntPtr address) : base(address)
        {
        }

        public PIMAGE_IMPORT_DESCRIPTOR(object value) : base(value)
        {
        }

        public static PIMAGE_IMPORT_DESCRIPTOR operator +(PIMAGE_IMPORT_DESCRIPTOR c1, int c2)
        {
            return new PIMAGE_IMPORT_DESCRIPTOR(c1.Address + c2 * c1.StructSize);
        }

        public static PIMAGE_IMPORT_DESCRIPTOR operator ++(PIMAGE_IMPORT_DESCRIPTOR a)
        {
            return a + 1;
        }

        public static explicit operator PIMAGE_IMPORT_DESCRIPTOR(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            return new PIMAGE_IMPORT_DESCRIPTOR(ptr);
        }
    }

    public class PIMAGE_SECTION_HEADER : ManagedPtr<IMAGE_SECTION_HEADER>
    {
        public PIMAGE_SECTION_HEADER(IntPtr address) : base(address)
        {
        }

        public PIMAGE_SECTION_HEADER(object value) : base(value)
        {
        }

        public static explicit operator PIMAGE_SECTION_HEADER(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            return new PIMAGE_SECTION_HEADER(ptr);
        }
    }

    public class PIMAGE_THUNK_DATA : ManagedPtr<IMAGE_THUNK_DATA>
    {
        public PIMAGE_THUNK_DATA(IntPtr address) : base(address)
        {
        }

        public PIMAGE_THUNK_DATA(object value) : base(value)
        {
        }

        public static PIMAGE_THUNK_DATA operator +(PIMAGE_THUNK_DATA c1, int c2)
        {
            return new PIMAGE_THUNK_DATA(c1.Address + c2 * c1.StructSize);
        }

        public static PIMAGE_THUNK_DATA operator ++(PIMAGE_THUNK_DATA a)
        {
            return a + 1;
        }

        public static explicit operator PIMAGE_THUNK_DATA(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            return new PIMAGE_THUNK_DATA(ptr);
        }
    }

    public class PIMAGE_EXPORT_DIRECTORY : ManagedPtr<IMAGE_EXPORT_DIRECTORY>
    {
        public PIMAGE_EXPORT_DIRECTORY(IntPtr address) : base(address)
        {
        }

        public PIMAGE_EXPORT_DIRECTORY(object value) : base(value)
        {
        }

        public static PIMAGE_EXPORT_DIRECTORY operator +(PIMAGE_EXPORT_DIRECTORY c1, int c2)
        {
            return new PIMAGE_EXPORT_DIRECTORY(c1.Address + c2 * c1.StructSize);
        }

        public static PIMAGE_EXPORT_DIRECTORY operator ++(PIMAGE_EXPORT_DIRECTORY a)
        {
            return a + 1;
        }

        public static explicit operator PIMAGE_EXPORT_DIRECTORY(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            return new PIMAGE_EXPORT_DIRECTORY(ptr);
        }
    }

    public class PWORD : ManagedPtr<ushort>
    {
        public PWORD(IntPtr address) : base(address)
        {
        }

        public PWORD(object value) : base(value)
        {
        }

        public static PWORD operator +(PWORD c1, int c2)
        {
            return new PWORD(c1.Address + c2 * c1.StructSize);
        }

        public static PWORD operator ++(PWORD a)
        {
            return a + 1;
        }

        public static explicit operator PWORD(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            return new PWORD(ptr);
        }
    }

    public class PDWORD : ManagedPtr<uint>
    {
        public PDWORD(IntPtr address) : base(address)
        {
        }

        public PDWORD(object value) : base(value)
        {
        }

        public static PDWORD operator +(PDWORD c1, int c2)
        {
            return new PDWORD(c1.Address + c2 * c1.StructSize);
        }

        public static PDWORD operator ++(PDWORD a)
        {
            return a + 1;
        }

        public static explicit operator PDWORD(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            return new PDWORD(ptr);
        }
    }
    public class PIMAGE_IMPORT_BY_NAME : ManagedPtr<IMAGE_IMPORT_BY_NAME>
    {
        public PIMAGE_IMPORT_BY_NAME(IntPtr address) : base(address)
        {
        }

        public PIMAGE_IMPORT_BY_NAME(object value) : base(value)
        {
        }

        public static PIMAGE_IMPORT_BY_NAME operator +(PIMAGE_IMPORT_BY_NAME c1, int c2)
        {
            return new PIMAGE_IMPORT_BY_NAME(c1.Address + c2 * c1.StructSize);
        }

        public static PIMAGE_IMPORT_BY_NAME operator ++(PIMAGE_IMPORT_BY_NAME a)
        {
            return a + 1;
        }

        public static explicit operator PIMAGE_IMPORT_BY_NAME(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            return new PIMAGE_IMPORT_BY_NAME(ptr);
        }
    }

    public class PBYTE : ManagedPtr<byte>
    {
        public PBYTE(IntPtr address) : base(address)
        {
        }

        public PBYTE(object value) : base(value)
        {
        }

        public static PBYTE operator +(PBYTE c1, int c2)
        {
            return new PBYTE(c1.Address + c2 * c1.StructSize);
        }

        public static PBYTE operator ++(PBYTE a)
        {
            return a + 1;
        }

        public static explicit operator PBYTE(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            return new PBYTE(ptr);
        }
    }

    public class PIMAGE_BASE_RELOCATION : ManagedPtr<IMAGE_BASE_RELOCATION>
    {
        public PIMAGE_BASE_RELOCATION(IntPtr address) : base(address)
        {
        }

        public PIMAGE_BASE_RELOCATION(object value) : base(value)
        {
        }

        public static PIMAGE_BASE_RELOCATION operator +(PIMAGE_BASE_RELOCATION c1, int c2)
        {
            return new PIMAGE_BASE_RELOCATION(c1.Address + c2 * c1.StructSize);
        }

        public static PIMAGE_BASE_RELOCATION operator ++(PIMAGE_BASE_RELOCATION a)
        {
            return a + 1;
        }

        public static explicit operator PIMAGE_BASE_RELOCATION(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            return new PIMAGE_BASE_RELOCATION(ptr);
        }
    }

    public class PSHORT : ManagedPtr<short>
    {
        public PSHORT(IntPtr address) : base(address)
        {
        }

        public PSHORT(object value) : base(value)
        {
        }

        public static PSHORT operator +(PSHORT c1, int c2)
        {
            return new PSHORT(c1.Address + c2 * c1.StructSize);
        }

        public static PSHORT operator ++(PSHORT a)
        {
            return a + 1;
        }

        public static explicit operator PSHORT(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            return new PSHORT(ptr);
        }
    }

    public class PIMAGE_TLS_DIRECTORY32 : ManagedPtr<IMAGE_TLS_DIRECTORY32>
    {
        public PIMAGE_TLS_DIRECTORY32(IntPtr address) : base(address)
        {
        }

        public PIMAGE_TLS_DIRECTORY32(object value) : base(value)
        {
        }

        public static PIMAGE_TLS_DIRECTORY32 operator +(PIMAGE_TLS_DIRECTORY32 c1, int c2)
        {
            return new PIMAGE_TLS_DIRECTORY32(c1.Address + c2 * c1.StructSize);
        }

        public static PIMAGE_TLS_DIRECTORY32 operator ++(PIMAGE_TLS_DIRECTORY32 a)
        {
            return a + 1;
        }

        public static explicit operator PIMAGE_TLS_DIRECTORY32(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            return new PIMAGE_TLS_DIRECTORY32(ptr);
        }
    }

    public class PIMAGE_LOAD_CONFIG_DIRECTORY32 : ManagedPtr<IMAGE_LOAD_CONFIG_DIRECTORY32>
    {
        public PIMAGE_LOAD_CONFIG_DIRECTORY32(IntPtr address) : base(address)
        {
        }

        public PIMAGE_LOAD_CONFIG_DIRECTORY32(object value) : base(value)
        {
        }

        public static PIMAGE_LOAD_CONFIG_DIRECTORY32 operator +(PIMAGE_LOAD_CONFIG_DIRECTORY32 c1, int c2)
        {
            return new PIMAGE_LOAD_CONFIG_DIRECTORY32(c1.Address + c2 * c1.StructSize);
        }

        public static PIMAGE_LOAD_CONFIG_DIRECTORY32 operator ++(PIMAGE_LOAD_CONFIG_DIRECTORY32 a)
        {
            return a + 1;
        }

        public static explicit operator PIMAGE_LOAD_CONFIG_DIRECTORY32(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            return new PIMAGE_LOAD_CONFIG_DIRECTORY32(ptr);
        }
    }
}