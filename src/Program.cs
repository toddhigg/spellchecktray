using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace SpellTray;

public static class Program
{
    private static Mutex? _singleInstance;

    [STAThread]
    public static void Main(string[] args)
    {
        if (args.Any(arg => string.Equals(arg, "--uninstall", StringComparison.OrdinalIgnoreCase)))
        {
            AppUninstall.Run();
            return;
        }

        _singleInstance = new Mutex(true, @"Local\SpellTray.SingleInstance", out var created);
        if (!created)
        {
            return;
        }

        AppUninstall.Register();

        try
        {
            Microsoft.Windows.ApplicationModel.DynamicDependency.Bootstrap.Initialize(0x00010006);
            WinRT.ComWrappersSupport.InitializeComWrappers();
            Application.Start(p =>
            {
                var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
                SynchronizationContext.SetSynchronizationContext(context);
                new App();
            });
        }
        catch (Exception ex)
        {
            Log.Write(ex.ToString());
        }
    }
}
