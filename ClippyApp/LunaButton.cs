using System.Drawing.Drawing2D;

namespace ClippyApp;

class LunaButton : Button
{
    private bool _hover;

    public LunaButton()
    {
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        BackColor = Color.Transparent;
        ForeColor = Color.Black;
        Font = LunaColors.Ui;
        Height = 24;
        Cursor = Cursors.Hand;
    }

    public void FitToContent(int minWidth = 75)
    {
        var textSize = TextRenderer.MeasureText(Text, Font);
        Width = Math.Max(minWidth, textSize.Width + 20);
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        _hover = true;
        Invalidate();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        _hover = false;
        Invalidate();
        base.OnMouseLeave(e);
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        using var path = GdiHelpers.RoundedRect(rect, 3);

        Color top = Enabled ? LunaColors.ButtonTop : LunaColors.ButtonDisabledTop;
        Color mid = Enabled ? LunaColors.ButtonStop85 : LunaColors.ButtonDisabledTop;
        Color bottom = Enabled ? LunaColors.ButtonBottom : LunaColors.ButtonDisabledBottom;
        using (var brush = GdiHelpers.MultiStopBrush(rect, new[] { top, mid, bottom }, new[] { 0f, 0.85f, 1f }))
            g.FillPath(brush, path);

        using (var pen = new Pen(Enabled ? LunaColors.ButtonBorder : Color.Gray, 1f))
            g.DrawPath(pen, path);

        if (_hover && Enabled)
        {
            var innerRect = Rectangle.Inflate(rect, -2, -2);
            using var innerPath = GdiHelpers.RoundedRect(innerRect, 2);
            using var hoverPen = new Pen(LunaColors.ButtonHoverRing, 1.5f);
            g.DrawPath(hoverPen, innerPath);
        }

        var textColor = Enabled ? ForeColor : LunaColors.ButtonDisabledText;
        TextRenderer.DrawText(g, Text, Font, rect, textColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
    }
}
