namespace ClippyApp;

class RecurringTimer : IDisposable
{
    private readonly System.Windows.Forms.Timer _timer = new();
    private readonly Func<int> _nextIntervalMs;

    public RecurringTimer(Func<int> nextIntervalMs, Action onTick)
    {
        _nextIntervalMs = nextIntervalMs;
        _timer.Tick += (s, e) =>
        {
            onTick();
            _timer.Interval = _nextIntervalMs();
        };
    }

    public void SetEnabled(bool enabled)
    {
        if (enabled)
        {
            _timer.Interval = _nextIntervalMs();
            _timer.Start();
        }
        else
        {
            _timer.Stop();
        }
    }

    public void Dispose() => _timer.Dispose();
}
