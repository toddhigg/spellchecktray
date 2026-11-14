using H.NotifyIcon;
using H.NotifyIcon.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace SpellTray;

public partial class App : Application
{
    private TaskbarIcon? _tray;
    private CheckerWindow? _checker;
    private HotkeyHook? _hotkey;
    private ToggleMenuFlyoutItem? _startupItem;

    public App()
    {
        UnhandledException += (_, e) =>
        {
            Log.Write(e.Exception.ToString());
            e.Handled = true;
        };
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            StartupSettings.EnsureFirstRun();
            _checker = new CheckerWindow();

            var iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Dictionary.ico");
            _tray = new TaskbarIcon
            {
                ToolTipText = "Spell check",
                NoLeftClickDelay = true,
                ContextMenuMode = ContextMenuMode.SecondWindow,
                Icon = new System.Drawing.Icon(iconPath),
            };

            var open = new XamlUICommand();
            open.ExecuteRequested += (_, _) => _checker.Toggle();
            _tray.LeftClickCommand = open;

            _startupItem = new ToggleMenuFlyoutItem
            {
                Text = "Launch at startup",
                IsChecked = StartupSettings.IsEnabled(),
            };
            _startupItem.Click += (_, _) => StartupSettings.SetEnabled(_startupItem.IsChecked);

            var exit = new MenuFlyoutItem { Text = "Exit" };
            exit.Click += (_, _) => Quit();

            var menu = new MenuFlyout();
            menu.Items.Add(_startupItem);
            menu.Items.Add(new MenuFlyoutSeparator());
            menu.Items.Add(exit);
            _tray.ContextFlyout = menu;
            _tray.ForceCreate();

            _hotkey = new HotkeyHook();
            _hotkey.Pressed += () => _checker.DispatcherQueue.TryEnqueue(_checker.Toggle);
            if (!_hotkey.TryRegister())
            {
                _tray.TrayIcon.ShowNotification(
                    "Spell check",
                    "Ctrl+Space is already in use. Left-click the dictionary icon.",
                    NotificationIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            Log.Write(ex.ToString());
        }
    }

    private void Quit()
    {
        _hotkey?.Dispose();
        _tray?.Dispose();
        _checker?.CloseForExit();
        Environment.Exit(0);
    }
}
