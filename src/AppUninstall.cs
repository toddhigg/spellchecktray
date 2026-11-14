using System.Diagnostics;
using Microsoft.Win32;

namespace SpellTray;

static class AppUninstall
{
    private const string UninstallKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Uninstall\SpellTray";

    public static void Register()
    {
        var exe = ExePath();
        var dir = Path.GetDirectoryName(exe);
        if (string.IsNullOrEmpty(dir) || File.Exists(Path.Combine(dir, "unins000.exe")))
        {
            return;
        }

        using var key = Registry.CurrentUser.CreateSubKey(UninstallKeyPath);
        key.SetValue("DisplayName", "Spell Check");
        key.SetValue("Publisher", "Spell Check");
        key.SetValue("DisplayIcon", exe);
        key.SetValue("UninstallString", $"\"{exe}\" --uninstall");
        key.SetValue("NoModify", 1, RegistryValueKind.DWord);
        key.SetValue("NoRepair", 1, RegistryValueKind.DWord);
    }

    public static void Run()
    {
        foreach (var process in Process.GetProcessesByName("SpellTray"))
        {
            if (process.Id == Environment.ProcessId)
            {
                continue;
            }

            try
            {
                process.Kill(entireProcessTree: true);
            }
            catch
            {
            }
        }

        using var run = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", writable: true);
        run?.DeleteValue("SpellTray", throwOnMissingValue: false);
        Registry.CurrentUser.DeleteSubKeyTree(@"Software\SpellTray", throwOnMissingSubKey: false);
        Registry.CurrentUser.DeleteSubKeyTree(UninstallKeyPath, throwOnMissingSubKey: false);

        var dir = Path.GetDirectoryName(ExePath());
        if (string.IsNullOrEmpty(dir))
        {
            return;
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c timeout /t 2 /nobreak >nul & rmdir /s /q \"{dir}\"",
            CreateNoWindow = true,
            UseShellExecute = false,
        });
    }

    private static string ExePath()
    {
        return Environment.ProcessPath ?? Environment.GetCommandLineArgs()[0];
    }
}
