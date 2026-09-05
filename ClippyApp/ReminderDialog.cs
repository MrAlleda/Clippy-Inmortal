namespace ClippyApp;

class ReminderDialog : XpFormBase
{
    private readonly NumericUpDown _minutes;
    private readonly TextBox _message;

    public int Minutes => (int)_minutes.Value;
    public string Message => _message.Text.Trim();

    public ReminderDialog() : base("Nuevo recordatorio", new Size(320, 178))
    {
        var group = AddGroup("¿Cuándo te aviso?", new Point(14, 12), new Size(292, 108));

        var lblMinutes = new Label { Text = "Minutos:", AutoSize = true, Location = new Point(16, 26) };
        _minutes = new NumericUpDown { Minimum = 1, Maximum = 999, Value = 10, Location = new Point(150, 22), Width = 80 };

        var lblMessage = new Label { Text = "Mensaje (opcional):", AutoSize = true, Location = new Point(16, 56) };
        _message = new TextBox { Location = new Point(16, 76), Width = 260, BorderStyle = BorderStyle.FixedSingle };

        group.Controls.AddRange(new Control[] { lblMinutes, _minutes, lblMessage, _message });

        AddCancelAndPrimaryButtons(134, "Aceptar", primaryResult: DialogResult.OK);
    }
}
