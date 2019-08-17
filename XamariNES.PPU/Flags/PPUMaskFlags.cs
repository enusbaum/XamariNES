namespace XamariNES.PPU.Flags
{
    /// <summary>
    ///     Defined Flags for the PPUMASK Register
    ///
    ///     https://wiki.nesdev.com/w/index.php/PPU_registers#PPUMASK
    /// </summary>
    static class PPUMaskFlags
    {
        public const byte Greyscale = 1;
        public const byte ShowBackgroundInLeftMost = 1 << 1;
        public const byte ShowSpritesInLeftMost = 1 << 2;
        public const byte ShowBackground = 1 << 3;
        public const byte ShowSprites = 1 << 4;
        public const byte EmphasizeRed = 1 << 5;
        public const byte EmphasizeGreen = 1 << 6;
        public const byte EmphasizeBlue = 1 << 7;
    }
}
