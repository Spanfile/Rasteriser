using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Rasteriser.External;
using static Rasteriser.External.Win32;
using static Rasteriser.Helper;
using System.Drawing.Imaging;

namespace Rasteriser
{
    public class Window : IDisposable
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public string Title { get; private set; }

        Bitmap bitmap;
        BitmapData data;
        int bytesPerColor;
        int byteCount;
        byte[] bytes;

        IntPtr hBitmap;
        IntPtr bitmapHdc;

        IntPtr hWnd;
        WNDCLASS wndc;

        IntPtr wndProcPtr;
        WndProc wndProcDelegate;
        GCHandle wndProcHandle;

        Stopwatch messageTimer;

        public Window(uint width, uint height, string title)
        {
            Width = (int)width;
            Height = (int)height;
            Title = title;

            messageTimer = new Stopwatch();

            Init();
        }

        void Init()
        {
            var hInstance = Process.GetCurrentProcess().Handle;

            wndc = new WNDCLASS();

            wndProcDelegate = new WndProc(WindowProc);
            wndProcPtr = Marshal.GetFunctionPointerForDelegate(wndProcDelegate);
            wndProcHandle = GCHandle.Alloc(wndProcDelegate);

            wndc.style = ClassStyles.HorizontalRedraw | ClassStyles.VerticalRedraw;
            wndc.lpfnWndProc = wndProcPtr;
            wndc.cbClsExtra = 0;
            wndc.cbWndExtra = 0;
            wndc.hInstance = hInstance;
            wndc.hIcon = LoadIcon(IntPtr.Zero, new IntPtr((int)External.SystemIcons.IDI_APPLICATION));
            wndc.hCursor = LoadCursor(IntPtr.Zero, (int)IdcStandardCursors.IDC_ARROW);
            wndc.hbrBackground = (IntPtr)(External.Color.WINDOW + 1);
            wndc.lpszMenuName = null;
            wndc.lpszClassName = Title;

            var regResult = RegisterClass(ref wndc);

            if (regResult == 0)
            {
                Console.WriteLine("RegisterClassEx failed");
                Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
                return;
            }

            var hWnd = CreateWindowEx(
                WindowStylesEx.WS_EX_OVERLAPPEDWINDOW,
                new IntPtr((int)(uint)regResult),
                Title,
                WindowStyles.WS_OVERLAPPEDWINDOW,
                Constants.CW_USEDEFAULT,
                Constants.CW_USEDEFAULT,
                Width,
                Height,
                IntPtr.Zero,
                IntPtr.Zero,
                hInstance,
                IntPtr.Zero);

            if (hWnd == IntPtr.Zero)
                Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());

            this.hWnd = hWnd;

            bitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            byteCount = data.Stride * bitmap.Height;
            bytesPerColor = data.Stride / bitmap.Width;
            bytes = new byte[byteCount];
            //Marshal.Copy(data.Scan0, bytes, 0, byteCount);

            bitmapHdc = CreateCompatibleDC(IntPtr.Zero);

            ShowWindow(hWnd, ShowWindowCommands.Normal);
            UpdateWindow(hWnd);
        }

        public void Dispose()
        {
            bitmap.UnlockBits(data);
            bitmap.Dispose();

            DeleteObject(hWnd);
            DeleteObject(hBitmap);
            DeleteDC(bitmapHdc);
        }

        public void SetPixel(int x, int y, System.Drawing.Color color)
        {
            if (x < 0 || x >= Width ||
                y < 0 || y >= Height)
                return;

            var index = bytesPerColor * (Width * y + x);
            bytes[index] = color.B;
            bytes[index + 1] = color.G;
            bytes[index + 2] = color.R;
        }

        public void SetTitle(string title) => SetWindowText(hWnd, title);

        public void Clear()
        {
            Array.Clear(bytes, 0, bytes.Length);
        }

        public void Invalidate()
        {
            InvalidateRect(hWnd, IntPtr.Zero, false);
        }

        void PrepareBitmap()
        {
            Marshal.Copy(bytes, 0, data.Scan0, byteCount);

            DeleteObject(hBitmap);
            hBitmap = bitmap.GetHbitmap(System.Drawing.Color.Black);
            SelectObject(bitmapHdc, hBitmap);
        }

        IntPtr WindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            PAINTSTRUCT ps;
            IntPtr hdc;

            switch ((WM)msg)
            {
                case WM.PAINT:
                    hdc = BeginPaint(hWnd, out ps);

                    PrepareBitmap();
                    if (!BitBlt(hdc, 0, 0, Width, Height, bitmapHdc, 0, 0, TernaryRasterOperations.SRCCOPY))
                        Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());

                    EndPaint(hWnd, ref ps);
                    return IntPtr.Zero;

                case WM.DESTROY:
                    PostQuitMessage(0);
                    return IntPtr.Zero;

                default:
                    return DefWindowProc(hWnd, (WM)msg, wParam, lParam);
            }
        }

        public bool DispatchMessages()
        {
            messageTimer.Restart();

            MSG msg;
            while (PeekMessage(out msg, IntPtr.Zero, 0, 0, (uint)PeekMessageParams.PM_REMOVE))
            {
                if (msg.message == (int)WM.QUIT)
                    return true;

                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }

            messageTimer.Stop();

            return false;
        }
    }
}
