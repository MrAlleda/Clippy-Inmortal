namespace ClippyApp;

abstract class XpFormBase : Form
{
    private const int TitleBarHeight = 28;
    private const int Cs_DropShadow = 0x00020000;

    protected readonly Panel Body;
    private readonly int _bodyWidth;

    protected XpFormBase(string title, Size bodySize)
    {
        _bodyWidth = bodySize.Width;

        Text = title;
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.CenterScreen;
        ShowInTaskbar = false;
        Font = LunaColors.Ui;
        Icon = ClippyAssets.LoadIcon();
        BackColor = LunaColors.WindowBorder;
        Padding = new Padding(1);
        ClientSize = new Size(bodySize.Width, bodySize.Height + TitleBarHeight);

        Body = new Panel { Dock = DockStyle.Fill, BackColor = LunaColors.Body };
        Controls.Add(Body);
        Controls.Add(new LunaTitleBar(this, title));

        Load += (s, e) => ApplyRoundedTopCorners();
        Resize += (s, e) => ApplyRoundedTopCorners();
    }

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ClassStyle |= Cs_DropShadow;
            return cp;
        }
    }

    private void ApplyRoundedTopCorners()
    {
        using var path = GdiHelpers.RoundedTopRect(Width, Height, 8);
        Region = new Region(path);
    }

    protected GroupBox AddGroup(string title, Point location, Size size)
    {
        var group = new GroupBox
        {
            Text = title,
            Location = location,
            Size = size,
            ForeColor = LunaColors.GroupTitle,
            Font = LunaColors.Ui,
        };
        Body.Controls.Add(group);
        return group;
    }

    protected Panel AddBalloon(string text, Point location, Size size)
    {
        var panel = new Panel { Location = location, Size = size, BackColor = LunaColors.BalloonFill };
        var label = new Label
        {
            Text = "📎 " + text,
            AutoSize = false,
            BackColor = Color.Transparent,
            Dock = DockStyle.Fill,
            Padding = new Padding(8),
            Font = LunaColors.Ui,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        panel.Controls.Add(label);
        panel.Paint += (s, e) => e.Graphics.DrawRectangle(new Pen(LunaColors.BalloonBorder), 0, 0, panel.Width - 1, panel.Height - 1);
        Body.Controls.Add(panel);
        return panel;
    }

    protected static LunaButton MakeButton(string text, EventHandler? onClick = null, DialogResult dialogResult = DialogResult.None)
    {
        var btn = new LunaButton { Text = text, DialogResult = dialogResult };
        btn.FitToContent();
        if (onClick != null) btn.Click += onClick;
        return btn;
    }

    protected void PlaceButtonsRight(int y, int rightMargin, params LunaButton[] buttonsRightToLeft)
    {
        int x = _bodyWidth - rightMargin;
        foreach (var btn in buttonsRightToLeft)
        {
            x -= btn.Width;
            btn.Location = new Point(x, y);
            Body.Controls.Add(btn);
            x -= 8;
        }
    }

    protected LunaButton AddCancelAndPrimaryButtons(int y, string primaryText, EventHandler? onPrimaryClick = null, DialogResult primaryResult = DialogResult.None)
    {
        var cancel = MakeButton("Cancelar", dialogResult: DialogResult.Cancel);
        var primary = MakeButton(primaryText, onPrimaryClick, primaryResult);
        PlaceButtonsRight(y, 14, cancel, primary);
        AcceptButton = primary;
        CancelButton = cancel;
        return primary;
    }

    protected void PlaceButtonsCentered(int y, params LunaButton[] buttons)
    {
        int totalWidth = buttons.Sum(b => b.Width) + 8 * (buttons.Length - 1);
        int x = (_bodyWidth - totalWidth) / 2;
        foreach (var btn in buttons)
        {
            btn.Location = new Point(x, y);
            Body.Controls.Add(btn);
            x += btn.Width + 8;
        }
    }
}
