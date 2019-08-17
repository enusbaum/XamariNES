namespace XamariNES.Cartridge.Flags
{
    /// <summary>
    ///     iNES ROM Byte 6 Flags
    ///
    ///     More Info: https://wiki.nesdev.com/w/index.php/INES#Flags_6
    /// </summary>
    internal static class Byte6Flags
    {
        public const int VerticalMirroring = 1;
        public const int BatteryBackedPRGRAM = 1 << 1;
        public const int TrainerPresent = 1 << 2;
        public const int FourScreenVRAM = 1 << 3;
    }
}