using System.Drawing.Drawing2D;

namespace ClippyApp;

class LunaTitleBar : Panel
{
    private readonly Form _owner;
    private Point _dragStart;
    private bool _dragging;

    public LunaTitleBar(Form owner, string title)
    {
        _owner = owner;
        Dock = DockStyle.Top;
        Height = 28;

        var titleLabel = new Label
        {
            Text = title,
            ForeColor = Color.White,
            Font = new Font("Tahoma", 9f, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoSize = false,
            Location = new Point(8, 0),
            Size = new Size(280, Height),
            BackColor = Color.Transparent,
        };

        var close = new Label
        {
            BackColor = Color.Transparent,
            Size = new Size(20, 20),
            Location = new Point(Width - 24, 4),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Cursor = Cursors.Hand,
        };
        close.Paint += (s, e) => DrawCloseButton(e.Graphics, close.ClientRectangle);
        close.Click += (s, e) => { _owner.DialogResult = DialogResult.Cancel; _owner.Close(); };

        Controls.Add(titleLabel);
        Controls.Add(close);

        EnableDrag(this);
        EnableDrag(titleLabel);
    }

    private static void DrawCloseButton(Graphics g, Rectangle rect)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var r = Rectangle.Inflate(rect, -1, -1);
        using var path = GdiHelpers.RoundedRect(r, 3);
        using (var brush = GdiHelpers.MultiStopBrush(r,
                   new[] { LunaColors.CloseTop, LunaColors.CloseStop40, LunaColors.CloseBottom },
                   new[] { 0f, 0.4f, 1f }))
            g.FillPath(brush, path);
        using (var pen = new Pen(Color.White, 1f))
            g.DrawPath(pen, path);

        using var xPen = new Pen(Color.White, 1.6f);
        var cross = Rectangle.Inflate(rect, -6, -6);
        g.DrawLine(xPen, cross.Left, cross.Top, cross.Right, cross.Bottom);
        g.DrawLine(xPen, cross.Left, cross.Bottom, cross.Right, cross.Top);
    }

    private void EnableDrag(Control c)
    {
        c.MouseDown += (s, e) => { _dragging = true; _dragStart = e.Location; };
        c.MouseMove += (s, e) =>
        {
            if (!_dragging) return;
            _owner.Location = new Point(_owner.Left + e.X - _dragStart.X, _owner.Top + e.Y - _dragStart.Y);
        };
        c.MouseUp += (s, e) => _dragging = false;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        using var brush = GdiHelpers.MultiStopBrush(ClientRectangle,
            new[] { LunaColors.TitleTop, LunaColors.TitleStop20, LunaColors.TitleStop70, LunaColors.TitleBottom },
            new[] { 0f, 0.2f, 0.7f, 1f });
        e.Graphics.FillRectangle(brush, ClientRectangle);
    }
}
