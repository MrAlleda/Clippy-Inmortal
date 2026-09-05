namespace ClippyApp;

class ConfirmDialog : XpFormBase
{
    public ConfirmDialog(string title, string message) : base(title, new Size(340, 140))
    {
        var icon = new Label { Text = "⚠️", Font = new Font("Segoe UI", 18f), AutoSize = true, Location = new Point(16, 16) };
        var text = new Label
        {
            Text = message,
            AutoSize = false,
            Location = new Point(60, 16),
            Size = new Size(264, 70),
            Font = LunaColors.Ui,
        };
        Body.Controls.Add(icon);
        Body.Controls.Add(text);

        var no = MakeButton("No", dialogResult: DialogResult.No);
        var yes = MakeButton("Sí", dialogResult: DialogResult.Yes);
        PlaceButtonsCentered(96, yes, no);

        AcceptButton = no;
        CancelButton = no;
    }
}
