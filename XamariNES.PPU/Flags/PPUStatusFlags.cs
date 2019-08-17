namespace XamariNES.PPU.Flags
{
    /// <summary>
    ///     Defined Flags for the PPUSTATUS Register
    ///
    ///     https://wiki.nesdev.com/w/index.php/PPU_registers#PPUSTATUS
    /// </summary>
    public static class PPUStatusFlags
    {
        public const byte VerticalBlankStarted = 1 << 7;
        public const byte SpriteZeroHit = 1 << 6;
        public const byte SpriteOverflow = 1 << 5;
    }
}