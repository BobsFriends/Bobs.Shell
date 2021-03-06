using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace Bobs.Shell
{
    public class ShellLink
    {
        public ShellLink()
        {
        }

        public ShellLink(string path, bool expandPaths = true)
        {
            if (!OperatingSystem.IsWindowsVersionAtLeast(5, 1, 2600))
                throw new PlatformNotSupportedException("The ShellLink is currently only supported on Windows.");

            ReadShellLink(path, expandPaths);
        }

        public string? Target
        {
            get;
            set;
        }
        public string? WorkingDirectory
        {
            get;
            set;
        }
        public string? Arguments
        {
            get;
            set;
        }
        public string? Description
        {
            get;
            set;
        }
        public string? IconLocation
        {
            get;
            set;
        }
        public int? IconIndex
        {
            get;
            set;
        }
        public ushort? Hotkey
        {
            get;
            set;
        }
        public HotkeyFlags? HotkeyModifier
        {
            get;
            set;
        }
        public ShowCommand? ShowCommand
        {
            get;
            set;
        }

        public void Load(string path, bool expandPaths = false)
        {
            if (!OperatingSystem.IsWindowsVersionAtLeast(5, 1, 2600))
                throw new PlatformNotSupportedException("The ShellLink is currently only supported on Windows.");

            ReadShellLink(path, expandPaths);
        }

        public void Save(string path)
        {
            if (!OperatingSystem.IsWindowsVersionAtLeast(5, 1, 2600))
                throw new PlatformNotSupportedException("The ShellLink is currently only supported on Windows.");

            WriteShellLink(path);
        }

        public Icon? GetIcon(int iconSize)
        {
            if (!OperatingSystem.IsWindowsVersionAtLeast(5, 1, 2600))
                throw new PlatformNotSupportedException("The ShellLink is currently only supported on Windows.");

            string? iconLocation = Environment.ExpandEnvironmentVariables(IconLocation ?? String.Empty);
            int iconIndex = IconIndex ?? 0;
            if (string.IsNullOrEmpty(iconLocation) || !File.Exists(iconLocation))
            {
                iconIndex = 0;
                iconLocation = Environment.ExpandEnvironmentVariables(Target ?? String.Empty);
                if ((string.IsNullOrEmpty(iconLocation)) || !File.Exists(iconLocation))
                    return null;
            }

            HRESULT hr = PInvoke.SHDefExtractIcon(
                iconLocation, iconIndex, 0,
                out DestroyIconSafeHandle iconHandle,
                out DestroyIconSafeHandle smallIconHandle,
                (uint)iconSize
                );
            try
            {
                if (!hr.Succeeded)
                    hr.ThrowOnFailure();

                if (iconHandle.IsClosed || iconHandle.IsInvalid)
                    return null;

                using Icon tempIcon = Icon.FromHandle(iconHandle.DangerousGetHandle());
                if (tempIcon == null)
                    return null;

                MemoryStream memoryStream = new();
                tempIcon.Save(memoryStream);
                memoryStream.Position = 0;
                return new Icon(memoryStream);
            }
            finally
            {
                if (!iconHandle.IsClosed)
                    iconHandle.Dispose();
                if (!smallIconHandle.IsClosed)
                    smallIconHandle.Dispose();
            }
        }

        public override string ToString()
        {
            return $"\"{Target}\" {Arguments} ({IconLocation}, {IconIndex})";
        }

        [SupportedOSPlatform("windows5.1.2600")]
        private unsafe void ReadShellLink(string path, bool expandPaths)
        {
            HRESULT hr = PInvoke.CoCreateInstance(typeof(Windows.Win32.UI.Shell.ShellLink).GUID, null, CLSCTX.CLSCTX_INPROC_SERVER, out IShellLinkW link);
            Marshal.ThrowExceptionForHR(hr);
            if (link is IPersistFile file)
            {
                file.Load(path, 0);
                link.Resolve(default, (uint)(SLR_FLAGS.SLR_NO_UI | SLR_FLAGS.SLR_NOUPDATE | SLR_FLAGS.SLR_NOSEARCH | SLR_FLAGS.SLR_NOTRACK));

                Target = link.GetPath(expandPaths);
                WorkingDirectory = link.GetWorkingDirectory();
                Arguments = link.GetArguments();
                Description = link.GetDescription();

                (string Path, int Index)? icon = link.GetIconLocation();
                IconLocation = icon?.Path;
                IconIndex = icon?.Index;

                (VIRTUAL_KEY Key, HotkeyFlags Modifier)? hotkey = link.GetHotkey();
                Hotkey = (ushort?)hotkey?.Key;
                HotkeyModifier = hotkey?.Modifier;

                ShowCommand = (ShowCommand?)link.GetShowCmd();
            }
        }

        [SupportedOSPlatform("windows5.1.2600")]
        private unsafe void WriteShellLink(string path)
        {
            HRESULT hr = PInvoke.CoCreateInstance(typeof(Windows.Win32.UI.Shell.ShellLink).GUID, null, CLSCTX.CLSCTX_INPROC_SERVER, out IShellLinkW link);
            Marshal.ThrowExceptionForHR(hr);
            if (link is IPersistFile file)
            {
                if (Target != null)
                    link.SetPath(Target);
                if (WorkingDirectory != null)
                    link.SetWorkingDirectory(WorkingDirectory);
                if (Arguments != null)
                    link.SetArguments(Arguments);
                if (Description != null)
                    link.SetDescription(Description);
                if ((IconLocation != null) || (IconIndex.HasValue))
                    link.SetIconLocation(IconLocation ?? "", IconIndex ?? 0);
                if (Hotkey.HasValue)
                    link.SetHotkey((ushort)(Hotkey.Value | (ushort)((ushort)(HotkeyModifier ?? HotkeyFlags.None) << 8)));
                if (ShowCommand != null)
                    link.SetShowCmd((int)ShowCommand.Value);
                file.Save(path, true);
            }
        }
    }
}