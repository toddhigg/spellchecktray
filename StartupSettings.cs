using Microsoft.Win32;

namespace SpellTray;

static class StartupSettings
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string AppKeyPath = @"Software\SpellTray";
    private const string RunValueName = "SpellTray";
    private const string ConfiguredName = "StartupConfigured";

    public static bool IsEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath);
        return key?.GetValue(RunValueName) is string value && value.Length > 0;
    }

    public static void EnsureFirstRun()
    {
        using var app = Registry.CurrentUser.CreateSubKey(AppKeyPath);
        if (app.GetValue(ConfiguredName) == null)
        {
            SetEnabled(true);
            return;
        }

        if (IsEnabled())
        {
            SetEnabled(true);
        }
    }

    public static void SetEnabled(bool enabled)
    {
        using var run = Registry.CurrentUser.CreateSubKey(RunKeyPath);
        using var app = Registry.CurrentUser.CreateSubKey(AppKeyPath);
        app.SetValue(ConfiguredName, 1, RegistryValueKind.DWord);
        if (enabled)
        {
            var path = Environment.ProcessPath;
            if (string.IsNullOrEmpty(path))
            {
                path = Environment.GetCommandLineArgs()[0];
            }

            run.SetValue(RunValueName, $"\"{path}\"");
            return;
        }

        run.DeleteValue(RunValueName, throwOnMissingValue: false);
    }
}
