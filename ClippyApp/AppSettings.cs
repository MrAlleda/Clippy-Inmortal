namespace ClippyApp;

class AppSettings
{
    public bool ClipboardReactionsEnabled = true;
    public bool AnnoyingModeEnabled;
    public int AnnoyingIntervalMinMinutes = 3;
    public int AnnoyingIntervalMaxMinutes = 7;
    public bool PauseReminderEnabled;
    public bool SoundEnabled = true;
    public bool StartWithWindows;
    public bool NotifyBirthdayDayBefore = true;
    public bool RepeatDueAlerts;
    public TimeSpan DailyCheckTime = new(9, 0, 0);
    public string SoundChoice = "Pop";
    public string AssistantName = "Clippy (clip)";
}
