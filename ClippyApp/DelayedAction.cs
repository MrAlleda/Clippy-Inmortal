namespace ClippyApp;

static class DelayedAction
{
    public static void Run(int delayMs, Action action)
    {
        var timer = new System.Windows.Forms.Timer { Interval = Math.Max(delayMs, 1) };
        timer.Tick += (s, e) =>
        {
            timer.Stop();
            timer.Dispose();
            action();
        };
        timer.Start();
    }
}
