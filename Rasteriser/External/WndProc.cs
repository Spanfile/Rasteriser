using System;
using System.Runtime.InteropServices;

namespace Rasteriser.External
{
    public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
}
