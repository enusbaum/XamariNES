using XamariNES.Common.Extensions;

namespace XamariNES.CPU
{
    /// <summary>
    ///     6502 CPU Status Register Helper Class
    ///
    ///     This class allows us to easily encode/decode/access the values within the
    ///     6502 status register. 
    /// </summary>
    public class CPUStatus
    {
        /// <summary>
        ///     Carry: 1 if last addition or shift resulted in a carry, or if last subtraction resulted in no borrow
        /// </summary>
        public bool Carry { get; set; }

        /// <summary>
        ///     Zero: 1 if last operation resulted in a 0 value
        /// </summary>
        public bool Zero { get; set; }

        /// <summary>
        ///     Interrupt: Interrupt inhibit
        ///     (0: /IRQ and /NMI get through; 1: only /NMI gets through)
        /// </summary>
        public bool InterruptDisable { get; set; }

        /// <summary>
        ///     Decimal: 1 to make ADC and SBC use binary-coded decimal arithmetic
        ///     (ignored on second-source 6502 like that in the NES)
        /// </summary>
        public bool DecimalMode { get; set; }

        /// <summary>
        ///     Unused by the 6502 in the NES
        /// </summary>
        public bool Bit4 { get; set; }

        /// <summary>
        ///     Unused by the 6502 in the NES
        /// </summary>
        public bool Bit5 { get; set; }

        /// <summary>
        ///     Overflow: 1 if last ADC or SBC resulted in signed overflow, or D6 from last BIT
        /// </summary>
        public bool Overflow { get; set; }

        /// <summary>
        ///     Negative: Set to bit 7 of the last operation
        /// </summary>
        public bool Negative { get; set; }

        /// <summary>
        ///     Default Constructor
        ///
        ///     Sets default Power-on state for the Status Register
        /// </summary>
        public CPUStatus()
        {
            //Default CPU Status State
            InterruptDisable = true;
            Bit5 = true;
            Bit4 = false;
        }

        /// <summary>
        ///     Returns value representing this instance of the CPU Status register as a byte
        /// </summary>
        /// <returns></returns>
        public byte ToByte()
        {
            byte output = 0x00;

            if (Negative)
                output = output.SetFlag(1 << 7);

            if (Overflow)
                output = output.SetFlag(1 << 6);

            if (Bit5)
                output = output.SetFlag(1 << 5);

            if (DecimalMode)
                output = output.SetFlag(1 << 3);

            if (InterruptDisable)
                output = output.SetFlag(1 << 2);

            if (Zero)
                output = output.SetFlag(1 << 1);

            if (Carry)
                output = output.SetFlag(1);

            return output;
        }

        /// <summary>
        ///     Sets the processor status flags by values in specified byte
        /// </summary>
        public void FromByte(byte value)
        {
            Carry = value.IsBitSet(0);
            Zero = value.IsBitSet(1);
            InterruptDisable = value.IsBitSet(2);
            DecimalMode = value.IsBitSet(3);
            Bit4 = value.IsBitSet(4);
            Bit5 = true;
            Overflow = value.IsBitSet(6);
            Negative = value.IsBitSet(7);
        }

        /// <summary>
        ///     Override - ToString()
        ///
        ///     Calls helper class, passing in this instance byte to decode and output
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToString(ToByte());

        /// <summary>
        ///     Overload - ToString()
        ///
        ///     Takes the given Status Register Byte and returns it as a human readable string
        /// </summary>
        /// <param name="statusByte"></param>
        /// <returns></returns>
        public string ToString(byte statusByte)
        {
            return $"{(statusByte.IsBitSet(7) ? "N" : "n")}{(statusByte.IsBitSet(6) ? "O" : "o")}{(statusByte.IsBitSet(3) ? "D" : "d")}{(statusByte.IsBitSet(2) ? "I" : "i")}{(statusByte.IsBitSet(1) ? "Z" : "z")}{(statusByte.IsBitSet(0) ? "C" : "c")}";
        }
    }
}