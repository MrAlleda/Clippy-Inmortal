using System.Globalization;

namespace ClippyApp;

static class SettingsStore
{
    private static readonly string FilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "ClippyApp", "settings.txt");

    private static Dictionary<string, string> Load()
    {
        var dict = new Dictionary<string, string>();
        try
        {
            if (File.Exists(FilePath))
            {
                foreach (var line in File.ReadAllLines(FilePath))
                {
                    int idx = line.IndexOf('=');
                    if (idx > 0) dict[line[..idx]] = line[(idx + 1)..];
                }
            }
        }
        catch { }
        return dict;
    }

    private static void SaveAll(Dictionary<string, string> dict)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            File.WriteAllLines(FilePath, dict.Select(kv => $"{kv.Key}={kv.Value}"));
        }
        catch { }
    }

    public static float LoadScale(float fallback)
    {
        var dict = Load();
        if (dict.TryGetValue("scale", out var s) &&
            float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v) && v > 0)
            return v;
        return fallback;
    }

    public static void SaveScale(float scale)
    {
        var dict = Load();
        dict["scale"] = scale.ToString(CultureInfo.InvariantCulture);
        SaveAll(dict);
    }

    public static (int Month, int Day)? LoadBirthday()
    {
        var dict = Load();
        if (dict.TryGetValue("birthday", out var b))
        {
            var parts = b.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[0], out var m) && int.TryParse(parts[1], out var d))
                return (m, d);
        }
        return null;
    }

    public static void SaveBirthday(int month, int day)
    {
        var dict = Load();
        dict["birthday"] = $"{month}-{day}";
        SaveAll(dict);
    }

    public static void ClearBirthday()
    {
        var dict = Load();
        dict.Remove("birthday");
        SaveAll(dict);
    }

    public static AppSettings LoadAppSettings()
    {
        var dict = Load();
        return new AppSettings
        {
            ClipboardReactionsEnabled = GetBool(dict, "clipboardReactions", true),
            AnnoyingModeEnabled = GetBool(dict, "annoyingMode", false),
            AnnoyingIntervalMinMinutes = GetInt(dict, "annoyingMin", 3),
            AnnoyingIntervalMaxMinutes = GetInt(dict, "annoyingMax", 7),
            PauseReminderEnabled = GetBool(dict, "pauseReminder", false),
            SoundEnabled = GetBool(dict, "soundEnabled", true),
            StartWithWindows = GetBool(dict, "startWithWindows", false),
            NotifyBirthdayDayBefore = GetBool(dict, "notifyDayBefore", true),
            RepeatDueAlerts = GetBool(dict, "repeatDueAlerts", false),
            DailyCheckTime = TimeSpan.TryParse(dict.GetValueOrDefault("dailyCheckTime"), out var t) ? t : new TimeSpan(9, 0, 0),
            SoundChoice = dict.GetValueOrDefault("soundChoice", "Pop") ?? "Pop",
            AssistantName = dict.GetValueOrDefault("assistantName", "Clippy (clip)") ?? "Clippy (clip)",
        };
    }

    public static void SaveAppSettings(AppSettings s)
    {
        var dict = Load();
        dict["clipboardReactions"] = s.ClipboardReactionsEnabled.ToString();
        dict["annoyingMode"] = s.AnnoyingModeEnabled.ToString();
        dict["annoyingMin"] = s.AnnoyingIntervalMinMinutes.ToString();
        dict["annoyingMax"] = s.AnnoyingIntervalMaxMinutes.ToString();
        dict["pauseReminder"] = s.PauseReminderEnabled.ToString();
        dict["soundEnabled"] = s.SoundEnabled.ToString();
        dict["startWithWindows"] = s.StartWithWindows.ToString();
        dict["notifyDayBefore"] = s.NotifyBirthdayDayBefore.ToString();
        dict["repeatDueAlerts"] = s.RepeatDueAlerts.ToString();
        dict["dailyCheckTime"] = s.DailyCheckTime.ToString();
        dict["soundChoice"] = s.SoundChoice;
        dict["assistantName"] = s.AssistantName;
        SaveAll(dict);
    }

    private static bool GetBool(Dictionary<string, string> dict, string key, bool fallback) =>
        dict.TryGetValue(key, out var v) && bool.TryParse(v, out var b) ? b : fallback;

    private static int GetInt(Dictionary<string, string> dict, string key, int fallback) =>
        dict.TryGetValue(key, out var v) && int.TryParse(v, out var i) ? i : fallback;
}
