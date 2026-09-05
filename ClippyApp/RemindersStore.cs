using System.Text.Json;

namespace ClippyApp;

static class RemindersStore
{
    private static readonly string FilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "ClippyApp", "reminders.json");

    public static List<ReminderRecord> Load()
    {
        try
        {
            if (File.Exists(FilePath))
                return JsonSerializer.Deserialize<List<ReminderRecord>>(File.ReadAllText(FilePath)) ?? new();
        }
        catch { }
        return new();
    }

    public static void Save(List<ReminderRecord> records)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            File.WriteAllText(FilePath, JsonSerializer.Serialize(records));
        }
        catch { }
    }
}
