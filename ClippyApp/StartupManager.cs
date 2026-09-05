using Microsoft.Win32;

namespace ClippyApp;

static class StartupManager
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "ClippyApp";

    public static bool IsEnabled()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, false);
            return key?.GetValue(ValueName) != null;
        }
        catch
        {
            return false;
        }
    }

    public static void SetEnabled(bool enabled)
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true) ?? Registry.CurrentUser.CreateSubKey(RunKeyPath);
            if (enabled)
                key.SetValue(ValueName, $"\"{Application.ExecutablePath}\"");
            else
                key.DeleteValue(ValueName, false);
        }
        catch
        {
        }
    }
}
