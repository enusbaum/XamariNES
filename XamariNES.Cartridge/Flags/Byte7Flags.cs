namespace XamariNES.Cartridge.Flags
{
    /// <summary>
    ///     iNES ROM Byte 7 Flags
    ///
    ///     More Info: https://wiki.nesdev.com/w/index.php/INES#Flags_7
    /// </summary>
    internal static class Byte7Flags
    {
        public const int VSUnisystem = 1;
        public const int PlayChoice10 = 1 << 1;
    }
}