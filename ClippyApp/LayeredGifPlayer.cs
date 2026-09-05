using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ClippyApp;

class LayeredGifPlayer : IDisposable
{
    private readonly Form _host;
    private Size _displaySize;
    private readonly System.Windows.Forms.Timer _timer = new();

    private Image? _source;
    private int _frameCount;
    private int[] _delays = Array.Empty<int>();
    private int _currentFrame;
    private bool _loopForever;
    private Action? _onComplete;

    public LayeredGifPlayer(Form host, Size displaySize)
    {
        _host = host;
        _displaySize = displaySize;
        _timer.Tick += (s, e) => Advance();
    }

    public void Play(string gifPath, bool loopForever, Action? onComplete = null)
    {
        _timer.Stop();
        _source?.Dispose();

        var bytes = File.ReadAllBytes(gifPath);
        _source = Image.FromStream(new MemoryStream(bytes));
        _frameCount = Math.Max(_source.GetFrameCount(FrameDimension.Time), 1);
        _delays = ReadDelays(_source, _frameCount);
        _loopForever = loopForever;
        _onComplete = onComplete;
        _currentFrame = 0;

        DrawFrame(0);

        if (_frameCount > 1)
        {
            _timer.Interval = Math.Max(_delays[0], 20);
            _timer.Start();
        }
    }

    public void SetDisplaySize(Size newSize)
    {
        _displaySize = newSize;
        if (_source != null) DrawFrame(_currentFrame);
    }

    private void Advance()
    {
        _currentFrame++;
        if (_currentFrame >= _frameCount)
        {
            if (_loopForever)
            {
                _currentFrame = 0;
            }
            else
            {
                _timer.Stop();
                _onComplete?.Invoke();
                return;
            }
        }
        DrawFrame(_currentFrame);
        _timer.Interval = Math.Max(_delays[_currentFrame], 20);
    }

    private void DrawFrame(int index)
    {
        if (_source == null || _host.IsDisposed) return;
        _source.SelectActiveFrame(FrameDimension.Time, index);

        var canvas = new Bitmap(_displaySize.Width, _displaySize.Height, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(canvas))
        {
            g.CompositingMode = CompositingMode.SourceCopy;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawImage(_source, new Rectangle(0, 0, _displaySize.Width, _displaySize.Height));
        }

        NativeLayered.SetBitmap(_host.Handle, canvas, _host.Location);
        canvas.Dispose();
    }

    private static int[] ReadDelays(Image img, int frameCount)
    {
        try
        {
            var item = img.GetPropertyItem(0x5100);
            if (item?.Value == null) return Enumerable.Repeat(100, frameCount).ToArray();
            var delays = new int[frameCount];
            for (int i = 0; i < frameCount; i++)
                delays[i] = BitConverter.ToInt32(item.Value, i * 4) * 10;
            return delays;
        }
        catch
        {
            return Enumerable.Repeat(100, frameCount).ToArray();
        }
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Dispose();
        _source?.Dispose();
    }
}
