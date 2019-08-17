namespace XamariNES.CPU.Enums
{
    /// <summary>
    ///     Enumerator where we list the available addressing modes in XamariNES
    /// </summary>
    public enum EnumAddressingMode
    {
        Implicit,
        Accumulator,
        Immediate,
        ZeroPage,
        ZeroPageX,
        ZeroPageY,
        Relative,
        Absolute,
        AbsoluteX,
        AbsoluteY,
        Indirect,
        IndexedIndirect,
        IndirectIndexed,
        NONE
    }
}