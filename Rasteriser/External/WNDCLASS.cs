using System;
using System.Runtime.InteropServices;

namespace Rasteriser.External
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WNDCLASS
    {
        public ClassStyles style;
        public IntPtr lpfnWndProc;
        [MarshalAs(UnmanagedType.I4)]
        public int cbClsExtra;
        [MarshalAs(UnmanagedType.I4)]
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpszMenuName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpszClassName;
    }
}