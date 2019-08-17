namespace XamariNES.CPU.Enums
{
    /// <summary>
    ///     Enumerator where we list the available Opcodes in XamariNES
    /// </summary>
    public enum EnumOpcode
    { 
        //Official 6502 Opcodes
        ADC,
        AND,
        ASL,
        BCC,
        BCS,
        BEQ,
        BIT,
        BMI,
        BNE,
        BPL,
        BRK,
        BVC,
        BVS,
        CLC,
        CLD,
        CLI,
        CLV,
        CMP,
        CPX,
        CPY,
        DEC,
        DEX,
        DEY,
        EOR,
        INC,
        INX,
        INY,
        JMP,
        JSR,
        LDA,
        LDX,
        LDY,
        LSR,
        NOP,
        ORA,
        PHA,
        PHP,
        PLA,
        PLP,
        ROL,
        ROR,
        RTI,
        RTS,
        SBC,
        SEC,
        SED,
        SEI,
        STA,
        STX,
        STY,
        TAX,
        TAY,
        TSX,
        TXA,
        TXS,
        TYA,

        //Undocumented Opcodes
        DCP,
        ISB,
        LAX,
        RLA,
        RRA,
        SAX,
        SLO,
        SRE,

        //State
        NONE
    }
}
