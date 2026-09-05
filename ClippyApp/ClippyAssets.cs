namespace ClippyApp;

static class ClippyAssets
{
    private static string AnimPath(string poseName) =>
        Path.Combine(AppContext.BaseDirectory, "Animaciones", poseName + ".gif");

    public static Image LoadPose(string poseName) => Image.FromFile(AnimPath(poseName));

    public static Image LoadSocialIcon(string name) =>
        Image.FromFile(Path.Combine(AppContext.BaseDirectory, "Icons", name + ".png"));

    public static Icon LoadIcon(string poseName = "RestPose", int size = 32)
    {
        try
        {
            using var bmp = new Bitmap(AnimPath(poseName));
            using var square = new Bitmap(size, size);
            using (var g = Graphics.FromImage(square))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, 0, 0, size, size);
            }
            return Icon.FromHandle(square.GetHicon());
        }
        catch
        {
            return SystemIcons.Application;
        }
    }
}
