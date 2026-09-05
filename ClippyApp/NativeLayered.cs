using System.Runtime.InteropServices;

namespace ClippyApp;

static class NativeLayered
{
    [StructLayout(LayoutKind.Sequential)]
    private struct POINT { public int X; public int Y; public POINT(int x, int y) { X = x; Y = y; } }

    [StructLayout(LayoutKind.Sequential)]
    private struct SIZE { public int cx; public int cy; public SIZE(int cx, int cy) { this.cx = cx; this.cy = cy; } }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct BLENDFUNCTION
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }

    private const byte AC_SRC_OVER = 0;
    private const byte AC_SRC_ALPHA = 1;
    private const int ULW_ALPHA = 2;

    public const int WS_EX_LAYERED = 0x80000;

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    private static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    private static extern bool DeleteDC(IntPtr hdc);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    private static extern bool DeleteObject(IntPtr hObject);

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    private static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize,
        IntPtr hdcSrc, ref POINT pptSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);

    public static void SetBitmap(IntPtr handle, Bitmap bitmap, Point location, byte opacity = 255)
    {
        if (bitmap.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            throw new ArgumentException("Bitmap must be 32bpp ARGB.");

        IntPtr screenDc = GetDC(IntPtr.Zero);
        IntPtr memDc = CreateCompatibleDC(screenDc);
        IntPtr hBitmap = IntPtr.Zero;
        IntPtr oldBitmap = IntPtr.Zero;

        try
        {
            hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
            oldBitmap = SelectObject(memDc, hBitmap);

            var size = new SIZE(bitmap.Width, bitmap.Height);
            var pointSource = new POINT(0, 0);
            var topPos = new POINT(location.X, location.Y);
            var blend = new BLENDFUNCTION
            {
                BlendOp = AC_SRC_OVER,
                BlendFlags = 0,
                SourceConstantAlpha = opacity,
                AlphaFormat = AC_SRC_ALPHA,
            };

            UpdateLayeredWindow(handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend, ULW_ALPHA);
        }
        finally
        {
            ReleaseDC(IntPtr.Zero, screenDc);
            if (hBitmap != IntPtr.Zero)
            {
                SelectObject(memDc, oldBitmap);
                DeleteObject(hBitmap);
            }
            DeleteDC(memDc);
        }
    }
}
