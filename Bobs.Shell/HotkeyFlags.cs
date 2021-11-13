namespace Bobs.Shell
{
    [Flags]
    public enum HotkeyFlags : ushort
    {
        None = 0,
        Shift = 0x01,
        Control = 0x02,
        Alt = 0x04,
    }
}