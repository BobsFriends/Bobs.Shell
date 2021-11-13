using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Bobs.Shell;

namespace Windows.Win32.UI.Shell
{
    [SupportedOSPlatform("windows5.1.2600")]
    internal static class IShellLinkWExtensions
    {
        public unsafe static string? GetPath(this IShellLinkW link, bool expandPaths)
        {
            //WIN32_FIND_DATAW findData = new();
            Span<char> buff = stackalloc char[(int)(Constants.MAX_PATH + 1)];
            fixed (char* pBuff = buff)
            {
                PWSTR str = new(pBuff);
                link.GetPath(str, buff.Length, null, expandPaths ? 0 : (uint)SLGP_FLAGS.SLGP_RAWPATH);
                return str.Length == 0 ? null : new(pBuff, 0, str.Length);
            }
        }

        public unsafe static string? GetWorkingDirectory(this IShellLinkW link)
        {
            Span<char> buff = stackalloc char[(int)(Constants.MAX_PATH + 1)];
            fixed (char* pBuff = buff)
            {
                PWSTR str = new(pBuff);
                link.GetWorkingDirectory(str, buff.Length);
                return str.Length == 0 ? null : new(pBuff, 0, str.Length);
            }
        }
        public unsafe static string? GetArguments(this IShellLinkW link)
        {
            Span<char> buff = stackalloc char[(int)(Constants.INFOTIPSIZE + 1)];
            fixed (char* pBuff = buff)
            {
                PWSTR str = new(pBuff);
                link.GetArguments(str, buff.Length);
                return str.Length == 0 ? null : new(pBuff, 0, str.Length);
            }
        }
        public unsafe static string? GetDescription(this IShellLinkW link)
        {
            Span<char> buff = stackalloc char[(int)(Constants.INFOTIPSIZE + 1)];
            fixed (char* pBuff = buff)
            {
                try
                {
                    PWSTR str = new(pBuff);
                    link.GetDescription(str, buff.Length);
                    return str.Length == 0 ? null : new(pBuff, 0, str.Length);
                }
                catch (COMException)
                {
                    return null;
                }
            }
        }
        public unsafe static (string, int)? GetIconLocation(this IShellLinkW link)
        {
            Span<char> buff = stackalloc char[(int)(Constants.MAX_PATH + 1)];
            fixed (char* pBuff = buff)
            {
                PWSTR str = new(pBuff);
                link.GetIconLocation(str, buff.Length, out int iconIndex);
                return str.Length == 0 ? null : (new(pBuff, 0, str.Length), iconIndex);
            }
        }
        public unsafe static (VIRTUAL_KEY, HotkeyFlags)? GetHotkey(this IShellLinkW link)
        {
            link.GetHotkey(out ushort hotkey);
            return (hotkey == 0) ? null : ((VIRTUAL_KEY)(hotkey & 0x00FF), (HotkeyFlags)((hotkey & 0xFF00) >> 8));
        }
        public unsafe static SHOW_WINDOW_CMD? GetShowCmd(this IShellLinkW link)
        {
            link.GetShowCmd(out int cmd);
            return (cmd != 0) ? (SHOW_WINDOW_CMD)cmd : null;
        }
    }
}
