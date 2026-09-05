namespace ClippyApp;

class AboutForm : XpFormBase
{
    private const string TwitchUrl = "https://twitch.tv/MrAlleda";
    private const string InstagramUrl = "https://instagram.com/MrAlleda";

    public AboutForm() : base("Acerca de Clippy", new Size(340, 250))
    {
        var icon = new Label { Text = "📎", Font = new Font("Segoe UI", 30f), AutoSize = true, TextAlign = ContentAlignment.MiddleCenter, Location = new Point(140, 8) };
        var name = new Label { Text = "Clippy", Font = new Font("Tahoma", 10f, FontStyle.Bold), AutoSize = false, Location = new Point(0, 62), Width = 340, TextAlign = ContentAlignment.MiddleCenter };
        var version = new Label { Text = "Versión 1.0", ForeColor = LunaColors.SecondaryText, AutoSize = false, Width = 340, TextAlign = ContentAlignment.MiddleCenter, Location = new Point(0, 82) };
        var credit = new Label { Text = "Hecho por MrAlleda", Font = LunaColors.UiBold, AutoSize = false, Width = 340, TextAlign = ContentAlignment.MiddleCenter, Location = new Point(0, 100) };

        var tips = new ToolTip();
        var twitch = MakeSocialIcon(tips, "twitch", TwitchUrl, "Twitch: twitch.tv/MrAlleda");
        var instagram = MakeSocialIcon(tips, "instagram", InstagramUrl, "Instagram: instagram.com/MrAlleda");
        const int gap = 10;
        int rowWidth = twitch.Width + gap + instagram.Width;
        int rowX = (340 - rowWidth) / 2;
        twitch.Location = new Point(rowX, 122);
        instagram.Location = new Point(rowX + twitch.Width + gap, 122);

        var separator = new Panel { BackColor = LunaColors.GroupBorder, Location = new Point(20, 160), Size = new Size(300, 1) };
        var description = new Label
        {
            Text = "Tu asistente para cumpleaños, fechas y recordatorios.\nHecho con cariño y GDI+.",
            AutoSize = false,
            TextAlign = ContentAlignment.TopCenter,
            Location = new Point(20, 168),
            Size = new Size(300, 40),
        };

        Body.Controls.AddRange(new Control[] { icon, name, version, credit, twitch, instagram, separator, description });

        var ok = MakeButton("Aceptar", dialogResult: DialogResult.OK);
        PlaceButtonsCentered(214, ok);
        AcceptButton = ok;
        CancelButton = ok;
    }

    private static PictureBox MakeSocialIcon(ToolTip tips, string iconName, string url, string tooltip)
    {
        var box = new PictureBox
        {
            Image = ClippyAssets.LoadSocialIcon(iconName),
            SizeMode = PictureBoxSizeMode.Zoom,
            Size = new Size(28, 28),
            Cursor = Cursors.Hand,
        };
        box.Click += (s, e) => ExternalLinks.Open(url);
        tips.SetToolTip(box, tooltip);
        return box;
    }
}
