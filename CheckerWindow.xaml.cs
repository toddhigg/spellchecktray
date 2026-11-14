using System.Runtime.InteropServices;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Graphics;
using Windows.System;
using WinRT.Interop;

namespace SpellTray;

public sealed partial class CheckerWindow : Window
{
    private const uint MonitorDefaultToNearest = 2;

    private bool _allowClose;

    public CheckerWindow()
    {
        InitializeComponent();
        try
        {
            SystemBackdrop = new MicaBackdrop();
        }
        catch (Exception ex)
        {
            Log.Write(ex.ToString());
        }

        var hwnd = WindowNative.GetWindowHandle(this);
        var iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Dictionary.ico");
        if (File.Exists(iconPath))
        {
            AppWindow.SetIcon(iconPath);
        }

        AppWindow.Resize(new SizeInt32(Physical(hwnd, 560), Physical(hwnd, 240)));
        AppWindow.Closing += OnClosing;
        Input.KeyDown += OnInputKeyDown;
        AppWindow.Hide();
    }

    public void Toggle()
    {
        if (AppWindow.IsVisible)
        {
            AppWindow.Hide();
            return;
        }

        ShowCentered();
    }

    public void CloseForExit()
    {
        _allowClose = true;
        Close();
    }

    private void ShowCentered()
    {
        try
        {
            var hwnd = WindowNative.GetWindowHandle(this);
            if (GetCursorPos(out var cursor))
            {
                var monitor = MonitorFromPoint(cursor, MonitorDefaultToNearest);
                var info = new MonitorInfo { cbSize = Marshal.SizeOf<MonitorInfo>() };
                if (GetMonitorInfo(monitor, ref info))
                {
                    var size = AppWindow.Size;
                    var x = info.rcWork.left + Math.Max(0, (info.rcWork.Width - size.Width) / 2);
                    var y = info.rcWork.top + Math.Max(0, (info.rcWork.Height - size.Height) / 2);
                    AppWindow.Move(new PointInt32(x, y));
                }
            }

            Activate();
            Input.Focus(FocusState.Programmatic);
            Input.SelectionStart = Input.Text.Length;
            Input.SelectionLength = 0;
        }
        catch (Exception ex)
        {
            Log.Write(ex.ToString());
        }
    }

    private void OnInputKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Escape)
        {
            AppWindow.Hide();
            e.Handled = true;
        }
    }

    private void OnClosing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        if (_allowClose)
        {
            return;
        }

        args.Cancel = true;
        sender.Hide();
    }

    private static int Physical(nint hwnd, int dip)
    {
        var dpi = GetDpiForWindow(hwnd);
        if (dpi == 0)
        {
            dpi = 96;
        }

        return (int)Math.Round(dip * (dpi / 96.0));
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Point
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
        public int Width => right - left;
        public int Height => bottom - top;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MonitorInfo
    {
        public int cbSize;
        public Rect rcMonitor;
        public Rect rcWork;
        public uint dwFlags;
    }

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out Point lpPoint);

    [DllImport("user32.dll")]
    private static extern nint MonitorFromPoint(Point pt, uint dwFlags);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern bool GetMonitorInfo(nint hMonitor, ref MonitorInfo lpmi);

    [DllImport("user32.dll")]
    private static extern uint GetDpiForWindow(nint hwnd);
}
