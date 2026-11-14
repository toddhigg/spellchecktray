using System.Runtime.InteropServices;

namespace SpellTray;

sealed class HotkeyHook : IDisposable
{
    private const int WmHotkey = 0x0312;
    private const uint ModControl = 0x0002;
    private const uint ModNoRepeat = 0x4000;
    private const uint VkSpace = 0x20;
    private const int HotkeyId = 1;
    private static readonly nint HwndMessage = new(-3);

    private readonly WndProc _proc;
    private nint _hwnd;
    private bool _registered;

    public event Action? Pressed;

    public HotkeyHook()
    {
        _proc = WindowProc;
    }

    public bool TryRegister()
    {
        var instance = GetModuleHandle(null);
        var cls = new WndClass
        {
            lpfnWndProc = Marshal.GetFunctionPointerForDelegate(_proc),
            hInstance = instance,
            lpszClassName = "SpellTrayHotkey",
        };
        RegisterClass(ref cls);
        _hwnd = CreateWindowEx(0, cls.lpszClassName, "", 0, 0, 0, 0, 0, HwndMessage, 0, instance, 0);
        if (_hwnd == 0)
        {
            return false;
        }

        _registered = RegisterHotKey(_hwnd, HotkeyId, ModControl | ModNoRepeat, VkSpace);
        return _registered;
    }

    public void Dispose()
    {
        if (_registered && _hwnd != 0)
        {
            UnregisterHotKey(_hwnd, HotkeyId);
            _registered = false;
        }

        if (_hwnd != 0)
        {
            DestroyWindow(_hwnd);
            _hwnd = 0;
        }
    }

    private nint WindowProc(nint hwnd, uint msg, nint wParam, nint lParam)
    {
        if (msg == WmHotkey && wParam == HotkeyId)
        {
            Pressed?.Invoke();
            return 0;
        }

        return DefWindowProc(hwnd, msg, wParam, lParam);
    }

    private delegate nint WndProc(nint hwnd, uint msg, nint wParam, nint lParam);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct WndClass
    {
        public uint style;
        public nint lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public nint hInstance;
        public nint hIcon;
        public nint hCursor;
        public nint hbrBackground;
        public string? lpszMenuName;
        public string lpszClassName;
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern ushort RegisterClass(ref WndClass lpWndClass);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern nint CreateWindowEx(
        int dwExStyle,
        string lpClassName,
        string lpWindowName,
        int dwStyle,
        int x,
        int y,
        int nWidth,
        int nHeight,
        nint hWndParent,
        nint hMenu,
        nint hInstance,
        nint lpParam);

    [DllImport("user32.dll")]
    private static extern nint DefWindowProc(nint hwnd, uint msg, nint wParam, nint lParam);

    [DllImport("user32.dll")]
    private static extern bool DestroyWindow(nint hwnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(nint hwnd, int id, uint modifiers, uint key);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(nint hwnd, int id);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern nint GetModuleHandle(string? lpModuleName);
}
