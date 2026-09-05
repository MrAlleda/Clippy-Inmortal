using System.Runtime.InteropServices;

namespace ClippyApp;

static class Sound
{
    [DllImport("winmm.dll", CharSet = CharSet.Auto)]
    private static extern int mciSendString(string command, System.Text.StringBuilder? returnValue, int returnLength, IntPtr callback);

    private static readonly string PopPath = Path.Combine(AppContext.BaseDirectory, "Sonidos", "Pop.mp3");

    public static bool Enabled { get; set; } = true;

    public static void PlayPop()
    {
        try
        {
            if (!Enabled || !File.Exists(PopPath)) return;

            string alias = "clippypop" + Environment.TickCount64;
            mciSendString($"open \"{PopPath}\" type mpegvideo alias {alias}", null, 0, IntPtr.Zero);
            mciSendString($"setaudio {alias} volume to 550", null, 0, IntPtr.Zero);
            mciSendString($"play {alias}", null, 0, IntPtr.Zero);

            DelayedAction.Run(3000, () => mciSendString($"close {alias}", null, 0, IntPtr.Zero));
        }
        catch
        {
        }
    }
}
