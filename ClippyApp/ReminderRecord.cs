namespace ClippyApp;

enum RepeatMode { Never, Yearly, Monthly }

class ReminderRecord
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Title { get; set; } = "";
    public DateTime Date { get; set; } = DateTime.Today;
    public TimeSpan Time { get; set; } = new(9, 0, 0);
    public string Category { get; set; } = "Otro";
    public RepeatMode Repeat { get; set; } = RepeatMode.Yearly;
    public bool NotifyDayBefore { get; set; }
}
