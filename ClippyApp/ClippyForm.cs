namespace ClippyApp;

public class ClippyForm : Form
{
    private const int NativeWidth = 124;
    private const int NativeHeight = 93;
    private const float DefaultScale = 1.25f;
    private const int PauseReminderIntervalMs = 30 * 60 * 1000;

    private static readonly (string Label, float Scale)[] SizePresets =
    {
        ("Pequeño", 0.85f),
        ("Mediano", 1.25f),
        ("Grande", 1.7f),
        ("Muy grande", 2.2f),
    };

    private static readonly (string Label, int Minutes)[] QuickReminderPresets =
    {
        ("En 5 minutos", 5),
        ("En 10 minutos", 10),
        ("En 30 minutos", 30),
        ("En 1 hora", 60),
    };

    private readonly string _animDir;
    private readonly LayeredGifPlayer _player;
    private readonly BalloonForm _balloon;
    private readonly NotifyIcon _trayIcon;
    private readonly System.Windows.Forms.Timer _idleTimer = new();
    private readonly System.Windows.Forms.Timer _specialDateTimer = new() { Interval = 60 * 60 * 1000 };
    private readonly RecurringTimer _pauseReminder;
    private readonly RecurringTimer _annoyingMode;
    private readonly Random _rng = new();
    private readonly HashSet<string> _celebratedToday = new();

    private float _scale;
    private AppSettings _appSettings = new();
    private bool _clipboardReactionsEnabled = true;
    private string? _lastClipboardText;
    private DateTime _lastClipboardReaction = DateTime.MinValue;
    private int? _birthdayMonth;
    private int? _birthdayDay;

    private Point _dragStartMouse;
    private Point _dragStartForm;
    private bool _dragging;
    private bool _dragMoved;

    public ClippyForm()
    {
        _animDir = Path.Combine(AppContext.BaseDirectory, "Animaciones");
        _scale = SettingsStore.LoadScale(DefaultScale);

        _appSettings = SettingsStore.LoadAppSettings();
        _appSettings.StartWithWindows = StartupManager.IsEnabled();
        _clipboardReactionsEnabled = _appSettings.ClipboardReactionsEnabled;
        Sound.Enabled = _appSettings.SoundEnabled;

        ConfigureWindowStyle();
        PositionAtBottomRight();

        _player = new LayeredGifPlayer(this, CurrentDisplaySize());
        _balloon = new BalloonForm { Owner = this };
        _trayIcon = CreateTrayIcon();

        _pauseReminder = new RecurringTimer(
            () => PauseReminderIntervalMs,
            () => ReactWithMessage("Alert", "¡Hora de una pausa! Estirate, tomá agua y descansá la vista un momento.", autoHideMs: 0));

        _annoyingMode = new RecurringTimer(RandomAnnoyingInterval, () =>
        {
            var (anim, line) = _rng.Pick(AnimationLibrary.AnnoyingTips);
            ReactWithMessage(anim, line);
        });
        _annoyingMode.SetEnabled(_appSettings.AnnoyingModeEnabled);
        _pauseReminder.SetEnabled(_appSettings.PauseReminderEnabled);

        ContextMenuStrip = BuildContextMenu();
        HookMouseEvents();

        _idleTimer.Tick += (s, e) => PlayAnimationOnly(_rng.Pick(AnimationLibrary.Idle));

        LoadBirthdaySetting();
        _specialDateTimer.Tick += (s, e) => { CheckSpecialDate(); CheckDueReminders(); };
        _specialDateTimer.Start();

        Load += OnFirstShown;
    }

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= NativeLayered.WS_EX_LAYERED;
            return cp;
        }
    }

    private void ConfigureWindowStyle()
    {
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        TopMost = true;
        StartPosition = FormStartPosition.Manual;
        ClientSize = CurrentDisplaySize();
    }

    private void PositionAtBottomRight()
    {
        var screen = Screen.PrimaryScreen!.WorkingArea;
        Location = new Point(screen.Right - ClientSize.Width - 30, screen.Bottom - ClientSize.Height - 20);
    }

    private Size CurrentDisplaySize() => new((int)(NativeWidth * _scale), (int)(NativeHeight * _scale));

    private NotifyIcon CreateTrayIcon()
    {
        var icon = new NotifyIcon
        {
            Icon = ClippyAssets.LoadIcon(),
            Visible = true,
            Text = "Clippy",
            ContextMenuStrip = BuildTrayMenu(),
        };
        icon.DoubleClick += (s, e) => ToggleVisible();
        return icon;
    }

    private void HookMouseEvents()
    {
        MouseDown += Clippy_MouseDown;
        MouseMove += Clippy_MouseMove;
        MouseUp += Clippy_MouseUp;
    }

    private void LoadBirthdaySetting()
    {
        if (SettingsStore.LoadBirthday() is { } birthday)
        {
            _birthdayMonth = birthday.Month;
            _birthdayDay = birthday.Day;
        }
    }

    private void OnFirstShown(object? sender, EventArgs e)
    {
        PlayAction("Greeting");
        DelayedAction.Run(6000, () => { CheckSpecialDate(); CheckDueReminders(); });
    }

    private string GifPath(string name) => Path.Combine(_animDir, name + ".gif");

    private void PlayAnimationOnly(string name)
    {
        _idleTimer.Stop();
        _player.Play(GifPath(name), loopForever: false, onComplete: () =>
        {
            _player.Play(GifPath("RestPose"), loopForever: true);
            ScheduleIdle();
        });
    }

    private void PlayAction(string name)
    {
        PlayAnimationOnly(name);

        string line = AnimationLibrary.RandomLine(name, _rng);
        if (!string.IsNullOrEmpty(line) && !AnimationLibrary.Silent.Contains(name))
        {
            _balloon.ShowSimple(line, AnchorPoint());
        }
    }

    private void ReactWithMessage(string animationName, string message, int autoHideMs = 7000)
    {
        if (!Visible) Show();
        Sound.PlayPop();
        PlayAnimationOnly(animationName);
        _balloon.ShowSimple(message, AnchorPoint(), autoHideMs);
    }

    private void ScheduleIdle()
    {
        _idleTimer.Stop();
        _idleTimer.Interval = 8000 + _rng.Next(14000);
        _idleTimer.Start();
    }

    private Point AnchorPoint() => new(Left + (int)(ClientSize.Width * 0.65), Top - 4);

    private void SpeakRandom()
    {
        var choices = AnimationLibrary.SpeakLines.Keys
            .Where(k => !AnimationLibrary.Silent.Contains(k))
            .ToArray();
        PlayAction(_rng.Pick(choices));
    }

    private void ApplySize(float scale)
    {
        _scale = scale;
        SettingsStore.SaveScale(scale);

        var oldCenter = new Point(Left + ClientSize.Width / 2, Top + ClientSize.Height / 2);
        var newSize = CurrentDisplaySize();
        ClientSize = newSize;
        Location = new Point(oldCenter.X - newSize.Width / 2, oldCenter.Y - newSize.Height / 2);
        ClampToScreen(newSize);

        _player.SetDisplaySize(newSize);
        _balloon.MoveTo(AnchorPoint());
    }

    private void ClampToScreen(Size size)
    {
        var wa = Screen.FromControl(this).WorkingArea;
        int x = Math.Max(wa.Left, Math.Min(Left, wa.Right - size.Width));
        int y = Math.Max(wa.Top, Math.Min(Top, wa.Bottom - size.Height));
        Location = new Point(x, y);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        ClipboardNative.AddClipboardFormatListener(Handle);
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        ClipboardNative.RemoveClipboardFormatListener(Handle);
        base.OnHandleDestroyed(e);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == ClipboardNative.WM_CLIPBOARDUPDATE) OnClipboardChanged();
        base.WndProc(ref m);
    }

    private void OnClipboardChanged()
    {
        if (!_clipboardReactionsEnabled || !Visible) return;
        if ((DateTime.Now - _lastClipboardReaction).TotalMilliseconds < 800) return;

        try
        {
            if (Clipboard.ContainsImage())
            {
                _lastClipboardReaction = DateTime.Now;
                var (shotAnim, shotMsg) = ClipboardReactions.RandomScreenshotReaction(_rng);
                ReactWithMessage(shotAnim, shotMsg);
                return;
            }

            if (!Clipboard.ContainsText()) return;
            string text = Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(text) || text == _lastClipboardText) return;
            _lastClipboardText = text;
            _lastClipboardReaction = DateTime.Now;

            var (anim, message) = ClipboardReactions.Classify(text, _rng);
            ReactWithMessage(anim, message);
        }
        catch
        {
        }
    }

    private void ScheduleReminder(int minutes, string customMessage)
    {
        DelayedAction.Run(minutes * 60 * 1000, () =>
        {
            string msg = string.IsNullOrWhiteSpace(customMessage)
                ? "¡Recordatorio! Pasó el tiempo que me pediste."
                : customMessage;
            ReactWithMessage("Alert", msg, autoHideMs: 0);
        });

        ReactWithMessage("Congratulate", $"¡Listo! Te aviso en {FormatMinutes(minutes)}.");
    }

    private static string FormatMinutes(int minutes)
    {
        if (minutes % 60 != 0) return $"{minutes} minutos";
        int hours = minutes / 60;
        return hours == 1 ? "1 hora" : $"{hours} horas";
    }

    private void OpenCustomReminderDialog()
    {
        using var dlg = new ReminderDialog();
        if (dlg.ShowDialog(this) == DialogResult.OK)
            ScheduleReminder(dlg.Minutes, dlg.Message);
    }

    private int RandomAnnoyingInterval() => (3 + _rng.Next(5)) * 60 * 1000;

    private void Surprise()
    {
        var anim = _rng.Pick(AnimationLibrary.All);
        var line = _rng.Pick(AnimationLibrary.SurpriseLines);
        ReactWithMessage(anim, line);
    }

    private void CheckSpecialDate()
    {
        var today = DateTime.Today;
        TryCelebrate(today.Month == 1 && today.Day == 1, "newyear",
            () => ReactWithMessage("GetWizardy", "¡Feliz Año Nuevo! 🎉🥳", autoHideMs: 0));
        TryCelebrate(today.Month == 12 && today.Day == 25, "christmas",
            () => ReactWithMessage("Congratulate", "¡Feliz Navidad! 🎄🎁", autoHideMs: 0));
        TryCelebrate(_birthdayMonth == today.Month && _birthdayDay == today.Day, "birthday",
            () => ReactWithMessage("Congratulate", "¡Feliz cumpleaños! 🎂🥳 ¡Que tengas un día genial!", autoHideMs: 0));
    }

    private void TryCelebrate(bool condition, string key, Action celebrate)
    {
        if (!condition) return;
        string mark = $"{DateTime.Today:yyyyMMdd}:{key}";
        if (!_celebratedToday.Add(mark)) return;
        celebrate();
    }

    private void CheckDueReminders()
    {
        if (DateTime.Now.TimeOfDay < _appSettings.DailyCheckTime) return;

        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);
        foreach (var r in RemindersStore.Load())
        {
            TryCelebrate(MatchesDate(r, today), $"reminder:{r.Id}:{today:yyyyMMdd}",
                () => ReactWithMessage("Alert", $"📅 {r.Title}", autoHideMs: 0));

            if (r.NotifyDayBefore && _appSettings.NotifyBirthdayDayBefore)
            {
                TryCelebrate(MatchesDate(r, tomorrow), $"reminder-eve:{r.Id}:{today:yyyyMMdd}",
                    () => ReactWithMessage("Alert", $"📅 Mañana: {r.Title}", autoHideMs: 0));
            }
        }
    }

    private static bool MatchesDate(ReminderRecord r, DateTime day) => r.Repeat switch
    {
        RepeatMode.Yearly => r.Date.Month == day.Month && r.Date.Day == day.Day,
        RepeatMode.Monthly => r.Date.Day == day.Day,
        _ => r.Date.Date == day.Date,
    };

    private void OpenOptionsForm()
    {
        using var form = new OptionsForm(_appSettings);
        if (form.ShowDialog(this) != DialogResult.OK) return;

        _appSettings = form.Result;
        SettingsStore.SaveAppSettings(_appSettings);
        Sound.Enabled = _appSettings.SoundEnabled;
        _clipboardReactionsEnabled = _appSettings.ClipboardReactionsEnabled;
        _annoyingMode.SetEnabled(_appSettings.AnnoyingModeEnabled);
        _pauseReminder.SetEnabled(_appSettings.PauseReminderEnabled);
        StartupManager.SetEnabled(_appSettings.StartWithWindows);

        ReactWithMessage("Congratulate", "¡Listo! Guardé tus opciones.");
    }

    private void OpenNewReminder()
    {
        using var form = new ReminderEditForm();
        if (form.ShowDialog(this) != DialogResult.OK) return;

        var all = RemindersStore.Load();
        all.Add(form.Result);
        RemindersStore.Save(all);
        ReactWithMessage("Congratulate", "📎 Recordatorio guardado correctamente.");
    }

    private void OpenSearchReminders()
    {
        using var form = new SearchRemindersForm();
        form.ShowDialog(this);
    }

    private void OpenBirthdayWizard()
    {
        using var form = new BirthdayWizardForm();
        if (form.ShowDialog(this) != DialogResult.OK || form.Result == null) return;

        var all = RemindersStore.Load();
        all.Add(form.Result);
        RemindersStore.Save(all);
        ReactWithMessage("Congratulate", "📎 ¡Listo! Voy a recordar ese cumpleaños todos los años.");
    }

    private void OpenBirthdayDialog()
    {
        using var dlg = new BirthdayDialog(_birthdayMonth, _birthdayDay);
        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        if (dlg.Cleared)
        {
            _birthdayMonth = null;
            _birthdayDay = null;
            SettingsStore.ClearBirthday();
            ReactWithMessage("Wave", "Listo, borré la fecha.");
        }
        else
        {
            _birthdayMonth = dlg.Month;
            _birthdayDay = dlg.Day;
            SettingsStore.SaveBirthday(dlg.Month, dlg.Day);
            ReactWithMessage("Congratulate", "¡Guardado! Te voy a saludar ese día.");
        }
    }

    private void Clippy_MouseDown(object? sender, MouseEventArgs e)
    {
        _dragging = true;
        _dragMoved = false;
        _dragStartMouse = Cursor.Position;
        _dragStartForm = Location;
    }

    private void Clippy_MouseMove(object? sender, MouseEventArgs e)
    {
        if (!_dragging) return;
        var cur = Cursor.Position;
        int dx = cur.X - _dragStartMouse.X;
        int dy = cur.Y - _dragStartMouse.Y;
        if (!_dragMoved && (Math.Abs(dx) > 4 || Math.Abs(dy) > 4)) _dragMoved = true;
        if (_dragMoved)
        {
            Location = new Point(_dragStartForm.X + dx, _dragStartForm.Y + dy);
            _balloon.MoveTo(AnchorPoint());
        }
    }

    private void Clippy_MouseUp(object? sender, MouseEventArgs e)
    {
        _dragging = false;
        if (!_dragMoved && e.Button == MouseButtons.Left)
        {
            SpeakRandom();
        }
    }

    private ContextMenuStrip BuildContextMenu()
    {
        var menu = new ContextMenuStrip();
        menu.Items.Add("Animar", null, (s, e) => SpeakRandom());
        menu.Items.Add(BuildChooseAnimationMenu());

        var (sizeMenu, sizeMenuItems) = BuildSizeMenu();
        menu.Items.Add(sizeMenu);
        menu.Opening += (s, e) =>
        {
            foreach (var (item, scale) in sizeMenuItems)
                item.Checked = Math.Abs(scale - _scale) < 0.001f;
        };

        menu.Items.Add(BuildReminderMenu());
        menu.Items.Add("¡Sorprendeme!", null, (s, e) => Surprise());
        menu.Items.Add("Configurar mi cumpleaños...", null, (s, e) => OpenBirthdayDialog());

        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Nuevo recordatorio...", null, (s, e) => OpenNewReminder());
        menu.Items.Add("Buscar recordatorios...", null, (s, e) => OpenSearchReminders());
        menu.Items.Add("Asistente: nuevo cumpleaños...", null, (s, e) => OpenBirthdayWizard());

        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Ocultar", null, (s, e) => HideToTray());
        menu.Items.Add("Opciones...", null, (s, e) => OpenOptionsForm());
        menu.Items.Add("Acerca de...", null, (s, e) => { using var f = new AboutForm(); f.ShowDialog(this); });
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Salir", null, (s, e) => Application.Exit());
        return menu;
    }

    private ContextMenuStrip BuildTrayMenu()
    {
        var menu = new ContextMenuStrip();
        menu.Items.Add("Mostrar/Ocultar", null, (s, e) => ToggleVisible());
        menu.Items.Add("Animar", null, (s, e) => { if (!Visible) ToggleVisible(); SpeakRandom(); });
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Salir", null, (s, e) => Application.Exit());
        return menu;
    }

    private ToolStripMenuItem BuildChooseAnimationMenu()
    {
        var chooseMenu = new ToolStripMenuItem("Elegir animación");
        foreach (var name in AnimationLibrary.All)
            chooseMenu.DropDownItems.Add(name, null, (s, e) => PlayAction(name));
        return chooseMenu;
    }

    private (ToolStripMenuItem Menu, List<(ToolStripMenuItem Item, float Scale)> Items) BuildSizeMenu()
    {
        var items = new List<(ToolStripMenuItem, float)>();
        var sizeMenu = new ToolStripMenuItem("Tamaño");
        foreach (var (label, scale) in SizePresets)
        {
            var item = new ToolStripMenuItem(label);
            item.Click += (s, e) => ApplySize(scale);
            sizeMenu.DropDownItems.Add(item);
            items.Add((item, scale));
        }
        return (sizeMenu, items);
    }

    private ToolStripMenuItem BuildReminderMenu()
    {
        var reminderMenu = new ToolStripMenuItem("Recordatorios");
        foreach (var (label, minutes) in QuickReminderPresets)
            reminderMenu.DropDownItems.Add(label, null, (s, e) => ScheduleReminder(minutes, ""));

        reminderMenu.DropDownItems.Add(new ToolStripSeparator());
        reminderMenu.DropDownItems.Add("Personalizado...", null, (s, e) => OpenCustomReminderDialog());

        return reminderMenu;
    }

    private void HideToTray()
    {
        _balloon.Hide();
        _idleTimer.Stop();
        _player.Play(GifPath("Hide"), loopForever: false, onComplete: () =>
        {
            Hide();
            _trayIcon.ShowBalloonTip(2000, "Clippy", "Sigo por acá. Hacé doble clic para volver a verme.", ToolTipIcon.Info);
        });
    }

    private void ToggleVisible()
    {
        if (Visible)
        {
            HideToTray();
        }
        else
        {
            Show();
            PlayAction("Greeting");
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _trayIcon.Visible = false;
        _trayIcon.Dispose();
        _player.Dispose();
        _pauseReminder.Dispose();
        _annoyingMode.Dispose();
        _specialDateTimer.Stop();
        _specialDateTimer.Dispose();
        base.OnFormClosing(e);
    }
}
