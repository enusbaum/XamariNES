namespace XamariNES.PPU.Flags
{
    public static class ScanLineStateFlags
    {
        public const int Visible = 1 << 0;
        public const int PostRender = 1 << 1;
        public const int PreRender = 1 << 2;
        public const int VBlank = 1 << 3;
        public const int Default = 1 << 4;
    }
}
