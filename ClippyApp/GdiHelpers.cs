using System.Drawing.Drawing2D;

namespace ClippyApp;

static class GdiHelpers
{
    public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
        int d = radius * 2;
        var path = new GraphicsPath();
        path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
        path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
        path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    public static GraphicsPath RoundedTopRect(int width, int height, int radius)
    {
        int d = radius * 2;
        var path = new GraphicsPath();
        path.AddArc(0, 0, d, d, 180, 90);
        path.AddArc(width - d, 0, d, d, 270, 90);
        path.AddLine(width, radius, width, height);
        path.AddLine(width, height, 0, height);
        path.CloseFigure();
        return path;
    }

    public static LinearGradientBrush MultiStopBrush(RectangleF rect, Color[] colors, float[] positions)
    {
        var brush = new LinearGradientBrush(rect, colors[0], colors[^1], LinearGradientMode.Vertical)
        {
            InterpolationColors = new ColorBlend(colors.Length) { Colors = colors, Positions = positions },
        };
        return brush;
    }
}
