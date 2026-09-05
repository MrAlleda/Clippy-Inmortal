using System.Drawing.Drawing2D;

namespace ClippyApp;

class BalloonForm : Form
{
    private static readonly Color BalloonColor = Color.FromArgb(255, 255, 225);
    private static readonly Color BorderColor = Color.FromArgb(90, 85, 45);

    private const int Radius = 14;
    private const int TailWidth = 22;
    private const int TailHeight = 20;
    private const int TailOffsetFromRight = 30;
    private const int InnerPad = 14;
    private const int ContentWidth = 230;

    private readonly Label _closeLabel;
    private readonly Label _textLabel;
    private System.Windows.Forms.Timer? _typeTimer;
    private System.Windows.Forms.Timer? _autoHideTimer;
    private string _fullText = "";
    private int _typeIndex;

    public BalloonForm()
    {
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        TopMost = true;
        BackColor = BalloonColor;
        DoubleBuffered = true;

        _closeLabel = new Label
        {
            Text = "×",
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            AutoSize = false,
            Size = new Size(18, 18),
            TextAlign = ContentAlignment.MiddleCenter,
            Cursor = Cursors.Hand,
            BackColor = Color.Transparent,
        };
        _closeLabel.Click += (s, e) => Hide();
        Controls.Add(_closeLabel);

        _textLabel = new Label
        {
            AutoSize = true,
            MaximumSize = new Size(ContentWidth, 0),
            Font = new Font("Tahoma", 9.5f),
            ForeColor = Color.Black,
            BackColor = Color.Transparent,
            Location = new Point(InnerPad, InnerPad),
        };
        Controls.Add(_textLabel);
    }

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= 0x08;
            return cp;
        }
    }

    public void MoveTo(Point anchorTopCenter)
    {
        if (Visible) LayoutContent(anchorTopCenter);
    }

    public void ShowSimple(string text, Point anchorTopCenter, int autoHideMs = 6000)
    {
        BeginTyping(text, anchorTopCenter, onDone: () =>
        {
            _autoHideTimer?.Stop();
            _autoHideTimer?.Dispose();
            if (autoHideMs <= 0) return;
            _autoHideTimer = new System.Windows.Forms.Timer { Interval = autoHideMs };
            _autoHideTimer.Tick += (s, e) => { _autoHideTimer!.Stop(); Hide(); };
            _autoHideTimer.Start();
        });
    }

    private void BeginTyping(string text, Point anchorTopCenter, Action onDone)
    {
        _typeTimer?.Stop();
        _typeTimer?.Dispose();
        _fullText = text;
        _typeIndex = 0;
        _textLabel.Text = "";
        LayoutContent(anchorTopCenter);
        Show();

        _typeTimer = new System.Windows.Forms.Timer { Interval = 16 };
        _typeTimer.Tick += (s, e) =>
        {
            _typeIndex++;
            _textLabel.Text = _fullText.Substring(0, Math.Min(_typeIndex, _fullText.Length));
            LayoutContent(anchorTopCenter);
            if (_typeIndex >= _fullText.Length)
            {
                _typeTimer!.Stop();
                onDone();
            }
        };
        _typeTimer.Start();
    }

    private void LayoutContent(Point anchorTopCenter)
    {
        _textLabel.Location = new Point(InnerPad, InnerPad);

        int width = InnerPad * 2 + ContentWidth;
        int bodyHeight = Math.Max(_textLabel.Bottom + InnerPad, 50);
        int totalHeight = bodyHeight + TailHeight;

        ClientSize = new Size(width, totalHeight);
        _closeLabel.Location = new Point(width - 22, 4);

        Region?.Dispose();
        Region = new Region(BuildBalloonPath(width, bodyHeight));

        Left = anchorTopCenter.X - width + TailOffsetFromRight + TailWidth / 2;
        Top = anchorTopCenter.Y - totalHeight;

        Invalidate();
    }

    private static GraphicsPath BuildBalloonPath(int width, int bodyHeight)
    {
        int d = Radius * 2;
        int tailCenter = width - TailOffsetFromRight;
        int tailBaseLeft = tailCenter - TailWidth / 2;
        int tailBaseRight = tailCenter + TailWidth / 2;
        int tipX = tailCenter - TailWidth / 4;
        int tipY = bodyHeight + TailHeight;

        var path = new GraphicsPath();
        path.AddArc(0, 0, d, d, 180, 90);
        path.AddArc(width - d, 0, d, d, 270, 90);
        path.AddArc(width - d, bodyHeight - d, d, d, 0, 90);
        path.AddLine(width - Radius, bodyHeight, tailBaseRight, bodyHeight);
        path.AddLine(tailBaseRight, bodyHeight, tipX, tipY);
        path.AddLine(tipX, tipY, tailBaseLeft, bodyHeight);
        path.AddLine(tailBaseLeft, bodyHeight, Radius, bodyHeight);
        path.AddArc(0, bodyHeight - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(BorderColor, 1.5f);
        int bodyHeight = Height - TailHeight;
        using var path = BuildBalloonPath(Width, bodyHeight);
        e.Graphics.DrawPath(pen, path);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _typeTimer?.Stop();
        _autoHideTimer?.Stop();
        base.OnFormClosing(e);
    }
}
