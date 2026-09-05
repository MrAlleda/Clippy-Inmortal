using System.Drawing.Drawing2D;

namespace ClippyApp;

class BirthdayWizardForm : XpFormBase
{
    private const int SidePanelWidth = 110;
    private const int ContentWidth = 330;
    private const int StepAreaHeight = 150;

    private readonly Label[] _stepLabels = new Label[3];
    private readonly Panel[] _steps = new Panel[3];
    private int _currentStep = 1;

    private readonly TextBox _name;
    private readonly ComboBox _relation;
    private readonly DateTimePicker _date;
    private readonly CheckBox _everyYear;
    private readonly Panel _summaryBalloonHost;
    private readonly Label _summaryLabel;

    private readonly LunaButton _back;
    private readonly LunaButton _next;

    public ReminderRecord? Result { get; private set; }

    public BirthdayWizardForm() : base("Asistente para nuevo cumpleaños", new Size(SidePanelWidth + ContentWidth, StepAreaHeight + 50))
    {
        var sidePanel = new Panel { Location = Point.Empty, Size = new Size(SidePanelWidth, StepAreaHeight + 40) };
        sidePanel.Paint += (s, e) =>
        {
            using var brush = new LinearGradientBrush(sidePanel.ClientRectangle, LunaColors.WizardPanelTop, LunaColors.WizardPanelBottom, LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(brush, sidePanel.ClientRectangle);
        };
        string[] titles = { "1. Persona", "2. Fecha", "3. Confirmar" };
        for (int i = 0; i < 3; i++)
        {
            _stepLabels[i] = new Label
            {
                Text = titles[i],
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = LunaColors.Ui,
                AutoSize = true,
                Location = new Point(10, 14 + i * 22),
            };
            sidePanel.Controls.Add(_stepLabels[i]);
        }
        Body.Controls.Add(sidePanel);

        var contentArea = new Point(SidePanelWidth + 16, 16);

        _steps[0] = new Panel { Location = contentArea, Size = new Size(ContentWidth - 32, StepAreaHeight) };
        var lblQ1 = new Label { Text = "¿De quién es el cumpleaños?", Font = LunaColors.UiBold, AutoSize = true, Location = new Point(0, 0) };
        var lblName = new Label { Text = "Nombre:", AutoSize = true, Location = new Point(0, 28) };
        _name = new TextBox { Location = new Point(80, 24), Width = 220, BorderStyle = BorderStyle.FixedSingle };
        var lblRelation = new Label { Text = "Relación:", AutoSize = true, Location = new Point(0, 56) };
        _relation = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(80, 52), Width = 140 };
        _relation.Items.AddRange(new object[] { "Familia", "Amigo/a", "Trabajo" });
        _relation.SelectedIndex = 0;
        _steps[0].Controls.AddRange(new Control[] { lblQ1, lblName, _name, lblRelation, _relation });

        _steps[1] = new Panel { Location = contentArea, Size = new Size(ContentWidth - 32, StepAreaHeight), Visible = false };
        var lblQ2 = new Label { Text = "¿Cuándo es?", Font = LunaColors.UiBold, AutoSize = true, Location = new Point(0, 0) };
        var lblDate = new Label { Text = "Fecha:", AutoSize = true, Location = new Point(0, 28) };
        _date = new DateTimePicker { Format = DateTimePickerFormat.Short, Location = new Point(80, 24), Width = 130 };
        _everyYear = new CheckBox { Text = "Recordar todos los años", AutoSize = true, Checked = true, Location = new Point(0, 56) };
        _steps[1].Controls.AddRange(new Control[] { lblQ2, lblDate, _date, _everyYear });

        _steps[2] = new Panel { Location = contentArea, Size = new Size(ContentWidth - 32, StepAreaHeight), Visible = false };
        var lblQ3 = new Label { Text = "¡Listo!", Font = LunaColors.UiBold, AutoSize = true, Location = new Point(0, 0) };
        _summaryBalloonHost = new Panel { Location = new Point(0, 26), Size = new Size(ContentWidth - 32, 60), BackColor = LunaColors.BalloonFill };
        _summaryLabel = new Label { AutoSize = false, BackColor = Color.Transparent, Dock = DockStyle.Fill, Padding = new Padding(8), Font = LunaColors.Ui };
        _summaryBalloonHost.Controls.Add(_summaryLabel);
        _summaryBalloonHost.Paint += (s, e) => e.Graphics.DrawRectangle(new Pen(LunaColors.BalloonBorder), 0, 0, _summaryBalloonHost.Width - 1, _summaryBalloonHost.Height - 1);
        _steps[2].Controls.AddRange(new Control[] { lblQ3, _summaryBalloonHost });

        Body.Controls.Add(_steps[0]);
        Body.Controls.Add(_steps[1]);
        Body.Controls.Add(_steps[2]);

        _back = MakeButton("◀ Atrás", (s, e) => GoTo(_currentStep - 1));
        _next = MakeButton("Siguiente ▶", (s, e) => OnNextClicked());
        PlaceButtonsRight(StepAreaHeight + 20, 16, _next, _back);

        GoTo(1);
    }

    private void OnNextClicked()
    {
        if (_currentStep < 3)
        {
            GoTo(_currentStep + 1);
        }
        else
        {
            Result = new ReminderRecord
            {
                Title = $"Cumpleaños de {_name.Text.Trim()}",
                Category = "Cumpleaños",
                Date = _date.Value.Date,
                Repeat = _everyYear.Checked ? RepeatMode.Yearly : RepeatMode.Never,
                NotifyDayBefore = true,
            };
            DialogResult = DialogResult.OK;
        }
    }

    private void GoTo(int step)
    {
        step = Math.Clamp(step, 1, 3);
        _currentStep = step;

        for (int i = 0; i < 3; i++)
        {
            _steps[i].Visible = i == step - 1;
            _stepLabels[i].Font = i == step - 1 ? LunaColors.UiBold : LunaColors.Ui;
        }

        if (step == 3)
        {
            var relation = (string)_relation.SelectedItem!;
            _summaryLabel.Text = $"📎 Voy a recordarte el cumpleaños de {_name.Text.Trim()} ({relation}) cada {_date.Value:d MMMM}. Hacé clic en Finalizar para guardar.";
        }

        _back.Enabled = step > 1;
        _next.Text = step == 3 ? "Finalizar" : "Siguiente ▶";
        _next.FitToContent();
    }
}
