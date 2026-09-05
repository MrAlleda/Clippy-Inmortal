namespace ClippyApp;

class ReminderEditForm : XpFormBase
{
    private static readonly string[] Categories = { "Cumpleaños", "Reunión", "Aniversario", "Otro" };

    private readonly TextBox _title;
    private readonly DateTimePicker _date;
    private readonly DateTimePicker _time;
    private readonly ComboBox _category;
    private readonly RadioButton _repeatNever;
    private readonly RadioButton _repeatYearly;
    private readonly RadioButton _repeatMonthly;
    private readonly CheckBox _notifyDayBefore;
    private readonly Label _status;

    public ReminderRecord Result { get; }

    public ReminderEditForm(ReminderRecord? existing = null)
        : base(existing == null ? "Nuevo recordatorio" : "Editar recordatorio", new Size(420, 268))
    {
        Result = existing ?? new ReminderRecord();

        var group1 = AddGroup("Datos del recordatorio", new Point(14, 12), new Size(392, 108));
        var lblTitle = new Label { Text = "Título:", AutoSize = true, Location = new Point(14, 26) };
        _title = new TextBox { Text = Result.Title, Location = new Point(100, 22), Width = 278, BorderStyle = BorderStyle.FixedSingle };

        var lblDate = new Label { Text = "Fecha:", AutoSize = true, Location = new Point(14, 54) };
        _date = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = Result.Date, Location = new Point(100, 50), Width = 120 };

        var lblTime = new Label { Text = "Hora:", AutoSize = true, Location = new Point(230, 54) };
        _time = new DateTimePicker { Format = DateTimePickerFormat.Time, ShowUpDown = true, Value = DateTime.Today.Add(Result.Time), Location = new Point(268, 50), Width = 84 };

        var lblCat = new Label { Text = "Categoría:", AutoSize = true, Location = new Point(14, 82) };
        _category = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(100, 78), Width = 140 };
        _category.Items.AddRange(Categories);
        _category.SelectedIndex = Math.Max(0, Array.IndexOf(Categories, Result.Category));

        group1.Controls.AddRange(new Control[] { lblTitle, _title, lblDate, _date, lblTime, _time, lblCat, _category });

        var group2 = AddGroup("Repetir", new Point(14, 128), new Size(392, 46));
        _repeatNever = new RadioButton { Text = "Nunca", AutoSize = true, Location = new Point(14, 20) };
        _repeatYearly = new RadioButton { Text = "Cada año", AutoSize = true, Location = new Point(100, 20) };
        _repeatMonthly = new RadioButton { Text = "Cada mes", AutoSize = true, Location = new Point(210, 20) };
        _repeatNever.Checked = Result.Repeat == RepeatMode.Never;
        _repeatYearly.Checked = Result.Repeat == RepeatMode.Yearly;
        _repeatMonthly.Checked = Result.Repeat == RepeatMode.Monthly;
        group2.Controls.AddRange(new Control[] { _repeatNever, _repeatYearly, _repeatMonthly });

        _notifyDayBefore = new CheckBox
        {
            Text = "Que Clippy avise el día anterior",
            AutoSize = true,
            Checked = Result.NotifyDayBefore,
            Location = new Point(14, 182),
        };

        _status = new Label { AutoSize = true, ForeColor = LunaColors.SuccessText, Font = LunaColors.UiBold, Location = new Point(14, 204), Visible = false };

        Body.Controls.AddRange(new Control[] { _notifyDayBefore, _status });

        AddCancelAndPrimaryButtons(228, "💾 Guardar", (s, e) => Save());
    }

    private void Save()
    {
        if (string.IsNullOrWhiteSpace(_title.Text))
        {
            _status.ForeColor = LunaColors.ErrorText;
            _status.Text = "Poné un título para el recordatorio.";
            _status.Visible = true;
            return;
        }

        Result.Title = _title.Text.Trim();
        Result.Date = _date.Value.Date;
        Result.Time = _time.Value.TimeOfDay;
        Result.Category = (string)_category.SelectedItem!;
        Result.Repeat = _repeatYearly.Checked ? RepeatMode.Yearly : _repeatMonthly.Checked ? RepeatMode.Monthly : RepeatMode.Never;
        Result.NotifyDayBefore = _notifyDayBefore.Checked;

        DialogResult = DialogResult.OK;
    }
}
