namespace ClippyApp;

class BirthdayDialog : XpFormBase
{
    private static readonly string[] MonthNames =
    {
        "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
        "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre",
    };

    private readonly NumericUpDown _day;
    private readonly ComboBox _month;

    public int Day => (int)_day.Value;
    public int Month => _month.SelectedIndex + 1;
    public bool Cleared { get; private set; }

    public BirthdayDialog(int? currentMonth, int? currentDay) : base("Mi cumpleaños", new Size(320, 130))
    {
        var group = AddGroup("Fecha", new Point(14, 12), new Size(292, 66));

        var lblDay = new Label { Text = "Día:", AutoSize = true, Location = new Point(16, 26) };
        _day = new NumericUpDown { Minimum = 1, Maximum = 31, Value = currentDay ?? 1, Location = new Point(52, 22), Width = 60 };

        var lblMonth = new Label { Text = "Mes:", AutoSize = true, Location = new Point(134, 26) };
        _month = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(172, 22), Width = 104 };
        _month.Items.AddRange(MonthNames);
        _month.SelectedIndex = (currentMonth ?? 1) - 1;

        group.Controls.AddRange(new Control[] { lblDay, _day, lblMonth, _month });

        var clear = MakeButton("🗑 Borrar", (s, e) => { Cleared = true; DialogResult = DialogResult.OK; });
        clear.Location = new Point(14, 88);
        Body.Controls.Add(clear);

        AddCancelAndPrimaryButtons(88, "Aceptar", primaryResult: DialogResult.OK);
    }
}
