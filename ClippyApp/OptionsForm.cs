namespace ClippyApp;

class OptionsForm : XpFormBase
{
    private readonly LunaButton _tabGeneral;
    private readonly LunaButton _tabAvisos;
    private readonly Panel _tabHost;
    private readonly Panel _generalPanel;
    private readonly Panel _avisosPanel;

    private readonly CheckBox _startWithWindows;
    private readonly CheckBox _soundEnabled;
    private readonly CheckBox _annoyingMode;
    private readonly NumericUpDown _annoyingMin;
    private readonly NumericUpDown _annoyingMax;
    private readonly CheckBox _clipboardReactions;
    private readonly ComboBox _assistant;

    private readonly CheckBox _notifyDayBefore;
    private readonly CheckBox _repeatDueAlerts;
    private readonly CheckBox _pauseReminder;
    private readonly DateTimePicker _dailyTime;
    private readonly ComboBox _soundChoice;

    public AppSettings Result { get; }

    public OptionsForm(AppSettings current) : base("Opciones de Clippy", new Size(380, 292))
    {
        Result = new AppSettings
        {
            ClipboardReactionsEnabled = current.ClipboardReactionsEnabled,
            AnnoyingModeEnabled = current.AnnoyingModeEnabled,
            AnnoyingIntervalMinMinutes = current.AnnoyingIntervalMinMinutes,
            AnnoyingIntervalMaxMinutes = current.AnnoyingIntervalMaxMinutes,
            PauseReminderEnabled = current.PauseReminderEnabled,
            SoundEnabled = current.SoundEnabled,
            StartWithWindows = current.StartWithWindows,
            NotifyBirthdayDayBefore = current.NotifyBirthdayDayBefore,
            RepeatDueAlerts = current.RepeatDueAlerts,
            DailyCheckTime = current.DailyCheckTime,
            SoundChoice = current.SoundChoice,
            AssistantName = current.AssistantName,
        };

        _tabGeneral = MakeButton("General");
        _tabAvisos = MakeButton("Avisos");
        _tabGeneral.Location = new Point(14, 12);
        _tabAvisos.Location = new Point(14 + _tabGeneral.Width, 12);
        _tabGeneral.Click += (s, e) => ShowTab(general: true);
        _tabAvisos.Click += (s, e) => ShowTab(general: false);
        Body.Controls.Add(_tabGeneral);
        Body.Controls.Add(_tabAvisos);

        _tabHost = new Panel { Location = new Point(14, 34), Size = new Size(352, 184), BackColor = LunaColors.TabBorder };
        var inner = new Panel { Location = new Point(1, 1), Size = new Size(350, 182), BackColor = LunaColors.TabActiveBg };
        _tabHost.Controls.Add(inner);
        Body.Controls.Add(_tabHost);

        (_generalPanel, _startWithWindows, _soundEnabled, _annoyingMode, _annoyingMin, _annoyingMax, _clipboardReactions, _assistant) = BuildGeneralPanel();
        (_avisosPanel, _notifyDayBefore, _repeatDueAlerts, _pauseReminder, _dailyTime, _soundChoice) = BuildAvisosPanel();
        inner.Controls.Add(_generalPanel);
        inner.Controls.Add(_avisosPanel);

        var apply = MakeButton("Aplicar", (s, e) => ApplyToResult());
        var ok = MakeButton("Aceptar", (s, e) => { ApplyToResult(); DialogResult = DialogResult.OK; });
        PlaceButtonsRight(238, 14, apply, ok);
        AcceptButton = ok;

        ShowTab(general: true);
    }

    private (Panel, CheckBox, CheckBox, CheckBox, NumericUpDown, NumericUpDown, CheckBox, ComboBox) BuildGeneralPanel()
    {
        var panel = new Panel { Dock = DockStyle.Fill, BackColor = LunaColors.TabActiveBg };

        var startWithWindows = new CheckBox { Text = "Mostrar a Clippy al iniciar Windows", AutoSize = true, BackColor = Color.Transparent, Checked = Result.StartWithWindows, Location = new Point(12, 12) };
        var soundEnabled = new CheckBox { Text = "Sonidos del asistente", AutoSize = true, BackColor = Color.Transparent, Checked = Result.SoundEnabled, Location = new Point(12, 38) };
        var annoyingMode = new CheckBox { Text = "Consejos al azar cada tanto", AutoSize = true, BackColor = Color.Transparent, Checked = Result.AnnoyingModeEnabled, Location = new Point(12, 64) };

        var lblInterval = new Label { Text = "Cada", AutoSize = true, BackColor = Color.Transparent, Location = new Point(30, 90) };
        var annoyingMin = new NumericUpDown { Minimum = 1, Maximum = 180, Value = Result.AnnoyingIntervalMinMinutes, Location = new Point(64, 86), Width = 50 };
        var lblIntervalTo = new Label { Text = "a", AutoSize = true, BackColor = Color.Transparent, Location = new Point(118, 90) };
        var annoyingMax = new NumericUpDown { Minimum = 1, Maximum = 180, Value = Result.AnnoyingIntervalMaxMinutes, Location = new Point(134, 86), Width = 50 };
        var lblIntervalUnit = new Label { Text = "min", AutoSize = true, BackColor = Color.Transparent, Location = new Point(188, 90) };

        void UpdateIntervalEnabled() => annoyingMin.Enabled = annoyingMax.Enabled = annoyingMode.Checked;
        UpdateIntervalEnabled();
        annoyingMode.CheckedChanged += (s, e) => UpdateIntervalEnabled();

        var clipboardReactions = new CheckBox { Text = "Reaccionar al portapapeles (texto y capturas)", AutoSize = true, BackColor = Color.Transparent, Checked = Result.ClipboardReactionsEnabled, Location = new Point(12, 114) };

        var lblAssistant = new Label { Text = "Asistente:", AutoSize = true, BackColor = Color.Transparent, Location = new Point(12, 142) };
        var assistant = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(90, 138), Width = 150 };
        assistant.Items.AddRange(new object[] { "Clippy (clip)", "Rex (perro)", "Merlin (mago)" });
        assistant.SelectedItem = Result.AssistantName;
        if (assistant.SelectedIndex < 0) assistant.SelectedIndex = 0;

        var note = new Label
        {
            Text = "Por ahora solo tengo el aspecto de clip 📎",
            AutoSize = true,
            BackColor = Color.Transparent,
            ForeColor = LunaColors.SecondaryText,
            Location = new Point(12, 164),
            Visible = assistant.SelectedIndex != 0,
        };
        assistant.SelectedIndexChanged += (s, e) => note.Visible = assistant.SelectedIndex != 0;

        panel.Controls.AddRange(new Control[]
        {
            startWithWindows, soundEnabled, annoyingMode,
            lblInterval, annoyingMin, lblIntervalTo, annoyingMax, lblIntervalUnit,
            clipboardReactions, lblAssistant, assistant, note,
        });
        return (panel, startWithWindows, soundEnabled, annoyingMode, annoyingMin, annoyingMax, clipboardReactions, assistant);
    }

    private (Panel, CheckBox, CheckBox, CheckBox, DateTimePicker, ComboBox) BuildAvisosPanel()
    {
        var panel = new Panel { Dock = DockStyle.Fill, BackColor = LunaColors.TabActiveBg, Visible = false };

        var notifyDayBefore = new CheckBox { Text = "Avisar cumpleaños con 1 día de anticipación", AutoSize = true, BackColor = Color.Transparent, Checked = Result.NotifyBirthdayDayBefore, Location = new Point(12, 12) };
        var repeatDueAlerts = new CheckBox { Text = "Repetir aviso cada 15 minutos", AutoSize = true, BackColor = Color.Transparent, Checked = Result.RepeatDueAlerts, Location = new Point(12, 38) };
        var pauseReminder = new CheckBox { Text = "Recordarme cada 30 min hacer una pausa", AutoSize = true, BackColor = Color.Transparent, Checked = Result.PauseReminderEnabled, Location = new Point(12, 64) };

        var lblTime = new Label { Text = "Hora del aviso diario:", AutoSize = true, BackColor = Color.Transparent, Location = new Point(12, 92) };
        var dailyTime = new DateTimePicker { Format = DateTimePickerFormat.Time, ShowUpDown = true, Value = DateTime.Today.Add(Result.DailyCheckTime), Location = new Point(140, 88), Width = 90 };

        var lblSound = new Label { Text = "Sonido:", AutoSize = true, BackColor = Color.Transparent, Location = new Point(12, 120) };
        var soundChoice = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(140, 116), Width = 130 };
        soundChoice.Items.AddRange(new object[] { "Pop" });
        soundChoice.SelectedItem = Result.SoundChoice;
        if (soundChoice.SelectedIndex < 0) soundChoice.SelectedIndex = 0;

        panel.Controls.AddRange(new Control[] { notifyDayBefore, repeatDueAlerts, pauseReminder, lblTime, dailyTime, lblSound, soundChoice });
        return (panel, notifyDayBefore, repeatDueAlerts, pauseReminder, dailyTime, soundChoice);
    }

    private void ShowTab(bool general)
    {
        _generalPanel.Visible = general;
        _avisosPanel.Visible = !general;
    }

    private void ApplyToResult()
    {
        Result.StartWithWindows = _startWithWindows.Checked;
        Result.SoundEnabled = _soundEnabled.Checked;
        Result.AnnoyingModeEnabled = _annoyingMode.Checked;
        Result.AnnoyingIntervalMinMinutes = (int)_annoyingMin.Value;
        Result.AnnoyingIntervalMaxMinutes = (int)Math.Max(_annoyingMax.Value, _annoyingMin.Value);
        Result.ClipboardReactionsEnabled = _clipboardReactions.Checked;
        Result.AssistantName = (string)_assistant.SelectedItem!;
        Result.NotifyBirthdayDayBefore = _notifyDayBefore.Checked;
        Result.RepeatDueAlerts = _repeatDueAlerts.Checked;
        Result.PauseReminderEnabled = _pauseReminder.Checked;
        Result.DailyCheckTime = _dailyTime.Value.TimeOfDay;
        Result.SoundChoice = (string)_soundChoice.SelectedItem!;
    }
}
