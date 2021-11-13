using Windows.Win32.UI.WindowsAndMessaging;

namespace Bobs.Shell
{
    public enum ShowCommand
    {
        Normal = (int)SHOW_WINDOW_CMD.SW_SHOWNORMAL,
        Minimized = (int)SHOW_WINDOW_CMD.SW_SHOWMINIMIZED,
        Maximized = (int)SHOW_WINDOW_CMD.SW_SHOWMAXIMIZED,
    }
}