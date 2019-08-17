namespace XamariNES.PPU.Flags
{
    /// <summary>
    ///     Defined Flags for the PPUCTRL Register
    ///
    ///     https://wiki.nesdev.com/w/index.php/PPU_registers#PPUCTRL
    /// </summary>
    static class PPUCtrlFlags
    {
        public const byte NMIEnabled = 1 << 7;
        public const byte IsPPUMaster = 1 << 6;
        public const byte SpriteSize = 1 << 5;
        public const byte BackgroundPatternTableAddress = 1 << 4;
        public const byte SpriteTableAddress = 1 << 3;
        public const byte VRAMAddressIncrement = 1 << 2;
    }
}
