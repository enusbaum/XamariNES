using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using XamariNES.Cartridge.Mappers;
using XamariNES.Common.Extensions;
using XamariNES.Controller;
using XamariNES.CPU.Enums;

namespace XamariNES.CPU
{
    /// <summary>
    ///     Simulated MOS 6502 8-bit CPU Core
    ///     
    ///     In-Depth documentation: http://www.obelisk.me.uk/6502/reference.html
    /// </summary>
    public class Core
    {
        /// <summary>
        ///     X Index Register
        /// </summary>
        public byte X { get; set;}

        /// <summary>
        ///     Y Index Register
        /// </summary>
        public byte Y { get; set; }

        /// <summary>
        ///     Accumulator
        /// </summary>
        public byte A { get; set; }

        /// <summary>
        ///     Program Counter
        /// </summary>
        public int PC { get; set; }

        /// <summary>
        ///     Stack Pointer
        /// </summary>
        public byte SP { get; set; }

        /// <summary>
        ///     CPU Status Flags
        /// </summary>
        public readonly CPUStatus Status;

        /// <summary>
        ///     CPU Memory Space
        /// </summary>
        public Memory CPUMemory;

        /// <summary>
        ///     Total Cycles the core has executed since starting
        /// </summary>
        public long Cycles;

        /// <summary>
        ///     Stores information about CPU opcodes and their metadata
        /// </summary>
        private readonly Dictionary<int, CPUInstruction> _cpuInstructions;

        /// <summary>
        ///     Current Instruction being executed
        /// </summary>
        public CPUInstruction Instruction;

        /// <summary>
        ///     Used to signal the CPU that an NMI has occured
        /// </summary>
        public bool NMI;

        /// <summary>
        ///     Stack Base Offset
        /// </summary>
        public const ushort STACK_BASE = 0x100;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="memoryMapper">Cartridge Memory Mapper</param>
        /// <param name="controller">NES Controller (Player 1)</param>
        public Core(IMapper memoryMapper, IController controller = null)
        {
            Status = new CPUStatus();
            Instruction = new CPUInstruction();
            CPUMemory = new Memory(memoryMapper, controller);

            //Setup the Instructions
            _cpuInstructions = DeclareInstructions();

            //Reset Internal Counters and Values
            Reset();
        }

        /// <summary>
        ///     Sets up the instruction dictionary with MOS 6502 instructions
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, CPUInstruction> DeclareInstructions()
        {
            var output = new Dictionary<int, CPUInstruction>();

            #region OpCode Definition
            /********************
             *      ADC
             *******************/
            output.Add(0x69, new CPUInstruction()
            {
                Opcode = EnumOpcode.ADC,
                AddressingMode = EnumAddressingMode.Immediate,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = ADC
            });
            output.Add(0x65, new CPUInstruction()
            {
                Opcode = EnumOpcode.ADC,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 3,
                OpCodeExecution = ADC
            });
            output.Add(0x75, new CPUInstruction()
            {
                Opcode = EnumOpcode.ADC,
                AddressingMode = EnumAddressingMode.ZeroPageX,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 4,
                OpCodeExecution = ADC
            });
            output.Add(0x6D, new CPUInstruction()
            {
                Opcode = EnumOpcode.ADC,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = ADC
            });
            output.Add(0x7D, new CPUInstruction()
            {
                Opcode = EnumOpcode.ADC,
                AddressingMode = EnumAddressingMode.AbsoluteX,
                PageBoundaryCheck = true,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = ADC
            });
            output.Add(0x79, new CPUInstruction()
            {
                Opcode = EnumOpcode.ADC,
                AddressingMode = EnumAddressingMode.AbsoluteY,
                PageBoundaryCheck = true,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = ADC
            });
            output.Add(0x61, new CPUInstruction()
            {
                Opcode = EnumOpcode.ADC,
                AddressingMode = EnumAddressingMode.IndexedIndirect,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 6,
                OpCodeExecution = ADC
            });
            output.Add(0x71, new CPUInstruction()
            {
                Opcode = EnumOpcode.ADC,
                AddressingMode = EnumAddressingMode.IndirectIndexed,
                PageBoundaryCheck = true,
                Length = 2,
                Cycles = 5,
                OpCodeExecution = ADC
            });

            /********************
             *      AND
             *******************/
            output.Add(0x29, new CPUInstruction()
            {
                Opcode = EnumOpcode.AND,
                AddressingMode = EnumAddressingMode.Immediate,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = AND
            });
            output.Add(0x25, new CPUInstruction()
            {
                Opcode = EnumOpcode.AND,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 3,
                OpCodeExecution = AND
            });
            output.Add(0x35, new CPUInstruction()
            {
                Opcode = EnumOpcode.AND,
                AddressingMode = EnumAddressingMode.ZeroPageX,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 4,
                OpCodeExecution = AND
            });
            output.Add(0x2D, new CPUInstruction()
            {
                Opcode = EnumOpcode.AND,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = AND
            });
            output.Add(0x3D, new CPUInstruction()
            {
                Opcode = EnumOpcode.AND,
                AddressingMode = EnumAddressingMode.AbsoluteX,
                PageBoundaryCheck = true,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = AND
            });
            output.Add(0x39, new CPUInstruction()
            {
                Opcode = EnumOpcode.AND,
                AddressingMode = EnumAddressingMode.AbsoluteY,
                PageBoundaryCheck = true,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = AND
            });
            output.Add(0x21, new CPUInstruction()
            {
                Opcode = EnumOpcode.AND,
                AddressingMode = EnumAddressingMode.IndexedIndirect,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 6,
                OpCodeExecution = AND
            });
            output.Add(0x31, new CPUInstruction()
            {
                Opcode = EnumOpcode.AND,
                AddressingMode = EnumAddressingMode.IndirectIndexed,
                PageBoundaryCheck = true,
                Length = 2,
                Cycles = 5,
                OpCodeExecution = AND
            });

            /********************
             *       ASL
             *******************/
            output.Add(0x0A, new CPUInstruction()
            {
                Opcode = EnumOpcode.ASL,
                AddressingMode = EnumAddressingMode.Accumulator,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = ASL
            });
            output.Add(0x06, new CPUInstruction()
            {
                Opcode = EnumOpcode.ASL,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 5,
                OpCodeExecution = ASL
            });
            output.Add(0x16, new CPUInstruction()
            {
                Opcode = EnumOpcode.ASL,
                AddressingMode = EnumAddressingMode.ZeroPageX,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 6,
                OpCodeExecution = ASL
            });
            output.Add(0x0E, new CPUInstruction()
            {
                Opcode = EnumOpcode.ASL,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 6,
                OpCodeExecution = ASL
            });
            output.Add(0x1E, new CPUInstruction()
            {
                Opcode = EnumOpcode.ASL,
                AddressingMode = EnumAddressingMode.AbsoluteX,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 7,
                OpCodeExecution = ASL
            });

            /********************
             *       BCC
             *******************/
            output.Add(0x90, new CPUInstruction()
            {
                Opcode = EnumOpcode.BCC,
                AddressingMode = EnumAddressingMode.Relative,
                PageBoundaryCheck = true,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = BCC
            });

            //BCS
            output.Add(0xB0, new CPUInstruction()
            {
                Opcode = EnumOpcode.BCS,
                AddressingMode = EnumAddressingMode.Relative,
                PageBoundaryCheck = true,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = BCS
            });

            //BEQ
            output.Add(0xF0, new CPUInstruction()
            {
                Opcode = EnumOpcode.BEQ,
                AddressingMode = EnumAddressingMode.Relative,
                PageBoundaryCheck = true,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = BEQ
            });

            //BIT
            output.Add(0x24, new CPUInstruction()
            {
                Opcode = EnumOpcode.BIT,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 3,
                OpCodeExecution = BIT
            });
            output.Add(0x2C, new CPUInstruction()
            {
                Opcode = EnumOpcode.BIT,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = BIT
            });

            /********************
             *       BMI
             *******************/
            output.Add(0x30, new CPUInstruction()
            {
                Opcode = EnumOpcode.BMI,
                AddressingMode = EnumAddressingMode.Relative,
                PageBoundaryCheck = true,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = BMI
            });

            /********************
             *       BNE
             *******************/
            output.Add(0xD0, new CPUInstruction()
            {
                Opcode = EnumOpcode.BNE,
                AddressingMode = EnumAddressingMode.Relative,
                PageBoundaryCheck = true,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = BNE
            });

            /********************
             *       BPL
             *******************/
            output.Add(0x10, new CPUInstruction()
            {
                Opcode = EnumOpcode.BPL,
                AddressingMode = EnumAddressingMode.Relative,
                PageBoundaryCheck = true,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = BPL
            });

            /********************
             *       BRK
             *******************/
            output.Add(0x00, new CPUInstruction()
            {
                Opcode = EnumOpcode.BRK,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 7,
                OpCodeExecution = BRK
            });

            /********************
             *       BVC
             *******************/
            output.Add(0x50, new CPUInstruction()
            {
                Opcode = EnumOpcode.BVC,
                AddressingMode = EnumAddressingMode.Relative,
                PageBoundaryCheck = true,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = BVC
            });

            /********************
             *       BVS
             *******************/
            output.Add(0x70, new CPUInstruction()
            {
                Opcode = EnumOpcode.BVS,
                AddressingMode = EnumAddressingMode.Relative,
                PageBoundaryCheck = true,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = BVS
            });

            /********************
             *       CLC
             *******************/
            output.Add(0x18, new CPUInstruction()
            {
                Opcode = EnumOpcode.CLC,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = CLC
            });

            /********************
             *       CLD
             *******************/
            output.Add(0xD8, new CPUInstruction()
            {
                Opcode = EnumOpcode.CLD,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = CLD
            });

            /********************
             *       CLI
             *******************/
            output.Add(0x58, new CPUInstruction()
            {
                Opcode = EnumOpcode.CLI,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = CLI
            });

            /********************
             *       CLV
             *******************/
            output.Add(0xB8, new CPUInstruction()
            {
                Opcode = EnumOpcode.CLV,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = CLV
            });

            /********************
             *       CMP
             *******************/
            output.Add(0xC9, new CPUInstruction()
            {
                Opcode = EnumOpcode.CMP,
                AddressingMode = EnumAddressingMode.Immediate,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = CMP
            });
            output.Add(0xC5, new CPUInstruction()
            {
                Opcode = EnumOpcode.CMP,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 3,
                OpCodeExecution = CMP
            });
            output.Add(0xD5, new CPUInstruction()
            {
                Opcode = EnumOpcode.CMP,
                AddressingMode = EnumAddressingMode.ZeroPageX,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 4,
                OpCodeExecution = CMP
            });
            output.Add(0xCD, new CPUInstruction()
            {
                Opcode = EnumOpcode.CMP,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = CMP
            });
            output.Add(0xDD, new CPUInstruction()
            {
                Opcode = EnumOpcode.CMP,
                AddressingMode = EnumAddressingMode.AbsoluteX,
                PageBoundaryCheck = true,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = CMP
            });
            output.Add(0xD9, new CPUInstruction()
            {
                Opcode = EnumOpcode.CMP,
                AddressingMode = EnumAddressingMode.AbsoluteY,
                PageBoundaryCheck = true,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = CMP
            });
            output.Add(0xC1, new CPUInstruction()
            {
                Opcode = EnumOpcode.CMP,
                AddressingMode = EnumAddressingMode.IndexedIndirect,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 6,
                OpCodeExecution = CMP
            });
            output.Add(0xD1, new CPUInstruction()
            {
                Opcode = EnumOpcode.CMP,
                AddressingMode = EnumAddressingMode.IndirectIndexed,
                PageBoundaryCheck = true,
                Length = 2,
                Cycles = 5,
                OpCodeExecution = CMP
            });

            /********************
             *       CPX
             *******************/
            output.Add(0xE0, new CPUInstruction()
            {
                Opcode = EnumOpcode.CPX,
                AddressingMode = EnumAddressingMode.Immediate,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = CPX
            });
            output.Add(0xE4, new CPUInstruction()
            {
                Opcode = EnumOpcode.CPX,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 3,
                OpCodeExecution = CPX
            });
            output.Add(0xEC, new CPUInstruction()
            {
                Opcode = EnumOpcode.CPX,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = CPX
            });

            /********************
             *       CPY
             *******************/
            output.Add(0xC0, new CPUInstruction()
            {
                Opcode = EnumOpcode.CPY,
                AddressingMode = EnumAddressingMode.Immediate,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = CPY
            });
            output.Add(0xC4, new CPUInstruction()
            {
                Opcode = EnumOpcode.CPY,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 3,
                OpCodeExecution = CPY
            });
            output.Add(0xCC, new CPUInstruction()
            {
                Opcode = EnumOpcode.CPY,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = CPY
            });

            /********************
             *       DCP
             *  (Undocumented)
             *******************/
            output.Add(0xC3, new CPUInstruction()
            {
                Opcode = EnumOpcode.DCP,
                AddressingMode = EnumAddressingMode.IndexedIndirect,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 8,
                OpCodeExecution = DCP
            });
            output.Add(0xC7, new CPUInstruction()
            {
                Opcode = EnumOpcode.DCP,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 5,
                OpCodeExecution = DCP
            });
            output.Add(0xCF, new CPUInstruction()
            {
                Opcode = EnumOpcode.DCP,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 8,
                OpCodeExecution = DCP
            });
            output.Add(0xD3, new CPUInstruction()
            {
                Opcode = EnumOpcode.DCP,
                AddressingMode = EnumAddressingMode.IndirectIndexed,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 8,
                OpCodeExecution = DCP
            });
            output.Add(0xD7, new CPUInstruction()
            {
                Opcode = EnumOpcode.DCP,
                AddressingMode = EnumAddressingMode.ZeroPageX,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 6,
                OpCodeExecution = DCP
            });
            output.Add(0xDB, new CPUInstruction()
            {
                Opcode = EnumOpcode.DCP,
                AddressingMode = EnumAddressingMode.AbsoluteY,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 7,
                OpCodeExecution = DCP
            });
            output.Add(0xDF, new CPUInstruction()
            {
                Opcode = EnumOpcode.DCP,
                AddressingMode = EnumAddressingMode.AbsoluteX,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 7,
                OpCodeExecution = DCP
            });

            /********************
             *       DEC
             *******************/
            output.Add(0xC6, new CPUInstruction()
            {
                Opcode = EnumOpcode.DEC,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 5,
                OpCodeExecution = DEC
            });
            output.Add(0xD6, new CPUInstruction()
            {
                Opcode = EnumOpcode.DEC,
                AddressingMode = EnumAddressingMode.ZeroPageX,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 6,
                OpCodeExecution = DEC
            });
            output.Add(0xCE, new CPUInstruction()
            {
                Opcode = EnumOpcode.DEC,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 6,
                OpCodeExecution = DEC
            });
            output.Add(0xDE, new CPUInstruction()
            {
                Opcode = EnumOpcode.DEC,
                AddressingMode = EnumAddressingMode.AbsoluteX,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 7,
                OpCodeExecution = DEC
            });

            /********************
             *       DEX
             *******************/
            output.Add(0xCA, new CPUInstruction()
            {
                Opcode = EnumOpcode.DEX,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = DEX
            });

            /********************
             *       DEY
             *******************/
            output.Add(0x88, new CPUInstruction()
            {
                Opcode = EnumOpcode.DEY,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = DEY
            });

            /********************
             *       EOR
             *******************/
            output.Add(0x49, new CPUInstruction()
            {
                Opcode = EnumOpcode.EOR,
                AddressingMode = EnumAddressingMode.Immediate,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = EOR
            });
            output.Add(0x45, new CPUInstruction()
            {
                Opcode = EnumOpcode.EOR,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 3,
                OpCodeExecution = EOR
            });
            output.Add(0x55, new CPUInstruction()
            {
                Opcode = EnumOpcode.EOR,
                AddressingMode = EnumAddressingMode.ZeroPageX,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 4,
                OpCodeExecution = EOR
            });
            output.Add(0x4D, new CPUInstruction()
            {
                Opcode = EnumOpcode.EOR,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = EOR
            });
            output.Add(0x5D, new CPUInstruction()
            {
                Opcode = EnumOpcode.EOR,
                AddressingMode = EnumAddressingMode.AbsoluteX,
                PageBoundaryCheck = true,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = EOR
            });
            output.Add(0x59, new CPUInstruction()
            {
                Opcode = EnumOpcode.EOR,
                AddressingMode = EnumAddressingMode.AbsoluteY,
                PageBoundaryCheck = true,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = EOR
            });
            output.Add(0x41, new CPUInstruction()
            {
                Opcode = EnumOpcode.EOR,
                AddressingMode = EnumAddressingMode.IndexedIndirect,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 6,
                OpCodeExecution = EOR
            });
            output.Add(0x51, new CPUInstruction()
            {
                Opcode = EnumOpcode.EOR,
                AddressingMode = EnumAddressingMode.IndirectIndexed,
                PageBoundaryCheck = true,
                Length = 2,
                Cycles = 5,
                OpCodeExecution = EOR
            });

            /********************
             *       INC
             *******************/
            output.Add(0xE6, new CPUInstruction()
            {
                Opcode = EnumOpcode.INC,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 5,
                OpCodeExecution = INC
            });
            output.Add(0xF6, new CPUInstruction()
            {
                Opcode = EnumOpcode.INC,
                AddressingMode = EnumAddressingMode.ZeroPageX,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 6,
                OpCodeExecution = INC
            });
            output.Add(0xEE, new CPUInstruction()
            {
                Opcode = EnumOpcode.INC,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 6,
                OpCodeExecution = INC
            });
            output.Add(0xFE, new CPUInstruction()
            {
                Opcode = EnumOpcode.INC,
                AddressingMode = EnumAddressingMode.AbsoluteX,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 7,
                OpCodeExecution = INC
            });

            /********************
             *       INX
             *******************/
            output.Add(0xE8, new CPUInstruction()
            {
                Opcode = EnumOpcode.INX,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = INX
            });

            /********************
             *       INY
             *******************/
            output.Add(0xC8, new CPUInstruction()
            {
                Opcode = EnumOpcode.INY,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = INY
            });

            /********************
             *       ISB
             *  (Undocumented)
             *******************/
            output.Add(0xE3, new CPUInstruction()
            {
                Opcode = EnumOpcode.ISB,
                AddressingMode = EnumAddressingMode.IndexedIndirect,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 8,
                OpCodeExecution = ISB
            });
            output.Add(0xE7, new CPUInstruction()
            {
                Opcode = EnumOpcode.ISB,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 5,
                OpCodeExecution = ISB
            });
            output.Add(0xEF, new CPUInstruction()
            {
                Opcode = EnumOpcode.ISB,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 6,
                OpCodeExecution = ISB
            });
            output.Add(0xF3, new CPUInstruction()
            {
                Opcode = EnumOpcode.ISB,
                AddressingMode = EnumAddressingMode.IndirectIndexed,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 8,
                OpCodeExecution = ISB
            });
            output.Add(0xF7, new CPUInstruction()
            {
                Opcode = EnumOpcode.ISB,
                AddressingMode = EnumAddressingMode.ZeroPageX,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 6,
                OpCodeExecution = ISB
            });
            output.Add(0xFB, new CPUInstruction()
            {
                Opcode = EnumOpcode.ISB,
                AddressingMode = EnumAddressingMode.AbsoluteY,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 7,
                OpCodeExecution = ISB
            });
            output.Add(0xFF, new CPUInstruction()
            {
                Opcode = EnumOpcode.ISB,
                AddressingMode = EnumAddressingMode.AbsoluteX,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 7,
                OpCodeExecution = ISB
            });

            /********************
             *       JMP
             *******************/
            output.Add(0x4C, new CPUInstruction()
            {
                Opcode = EnumOpcode.JMP,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 0,
                Cycles = 3,
                OpCodeExecution = JMP
            });
            output.Add(0x6C, new CPUInstruction()
            {
                Opcode = EnumOpcode.JMP,
                AddressingMode = EnumAddressingMode.Indirect,
                PageBoundaryCheck = false,
                Length = 0,
                Cycles = 5,
                OpCodeExecution = JMP
            });

            /********************
             *       JSR
             *******************/
            output.Add(0x20, new CPUInstruction()
            {
                Opcode = EnumOpcode.JSR,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 0,
                Cycles = 6,
                OpCodeExecution = JSR
            });

            /********************
             *       LAX
             *  (Undocumented)
             *******************/
            output.Add(0xA3, new CPUInstruction()
            {
                Opcode = EnumOpcode.LAX,
                AddressingMode = EnumAddressingMode.IndexedIndirect,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 6,
                OpCodeExecution = LAX
            });
            output.Add(0xA7, new CPUInstruction()
            {
                Opcode = EnumOpcode.LAX,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 3,
                OpCodeExecution = LAX
            });
            output.Add(0xAF, new CPUInstruction()
            {
                Opcode = EnumOpcode.LAX,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = LAX
            });
            output.Add(0xB3, new CPUInstruction()
            {
                Opcode = EnumOpcode.LAX,
                AddressingMode = EnumAddressingMode.IndirectIndexed,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 5,
                OpCodeExecution = LAX
            });
            output.Add(0xB7, new CPUInstruction()
            {
                Opcode = EnumOpcode.LAX,
                AddressingMode = EnumAddressingMode.ZeroPageY,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 5,
                OpCodeExecution = LAX
            });
            output.Add(0xBF, new CPUInstruction()
            {
                Opcode = EnumOpcode.LAX,
                AddressingMode = EnumAddressingMode.AbsoluteY,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = LAX
            });

            /********************
             *       LDA
             *******************/
            output.Add(0xA9, new CPUInstruction()
            {
                Opcode = EnumOpcode.LDA,
                AddressingMode = EnumAddressingMode.Immediate,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = LDA
            });
            output.Add(0xA5, new CPUInstruction()
            {
                Opcode = EnumOpcode.LDA,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 3,
                OpCodeExecution = LDA
            });
            output.Add(0xB5, new CPUInstruction()
            {
                Opcode = EnumOpcode.LDA,
                AddressingMode = EnumAddressingMode.ZeroPageX,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 4,
                OpCodeExecution = LDA
            });
            output.Add(0xAD, new CPUInstruction()
            {
                Opcode = EnumOpcode.LDA,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = LDA
            });
            output.Add(0xBD, new CPUInstruction()
            {
                Opcode = EnumOpcode.LDA,
                AddressingMode = EnumAddressingMode.AbsoluteX,
                PageBoundaryCheck = true,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = LDA
            });
            output.Add(0xB9, new CPUInstruction()
            {
                Opcode = EnumOpcode.LDA,
                AddressingMode = EnumAddressingMode.AbsoluteY,
                PageBoundaryCheck = true,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = LDA
            });
            output.Add(0xA1, new CPUInstruction()
            {
                Opcode = EnumOpcode.LDA,
                AddressingMode = EnumAddressingMode.IndexedIndirect,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 6,
                OpCodeExecution = LDA
            });
            output.Add(0xB1, new CPUInstruction()
            {
                Opcode = EnumOpcode.LDA,
                AddressingMode = EnumAddressingMode.IndirectIndexed,
                PageBoundaryCheck = true,
                Length = 2,
                Cycles = 5,
                OpCodeExecution = LDA
            });

            /********************
             *       LDX
             *******************/
            output.Add(0xA2, new CPUInstruction()
            {
                Opcode = EnumOpcode.LDX,
                AddressingMode = EnumAddressingMode.Immediate,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = LDX
            });
            output.Add(0xA6, new CPUInstruction()
            {
                Opcode = EnumOpcode.LDX,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 3,
                OpCodeExecution = LDX
            });
            output.Add(0xB6, new CPUInstruction()
            {
                Opcode = EnumOpcode.LDX,
                AddressingMode = EnumAddressingMode.ZeroPageY,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 4,
                OpCodeExecution = LDX
            });
            output.Add(0xAE, new CPUInstruction()
            {
                Opcode = EnumOpcode.LDX,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = LDX
            });
            output.Add(0xBE, new CPUInstruction()
            {
                Opcode = EnumOpcode.LDX,
                AddressingMode = EnumAddressingMode.AbsoluteY,
                PageBoundaryCheck = true,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = LDX
            });

            /********************
             *       LDY
             *******************/
            output.Add(0xA0, new CPUInstruction()
            {
                Opcode = EnumOpcode.LDY,
                AddressingMode = EnumAddressingMode.Immediate,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = LDY
            });
            output.Add(0xA4, new CPUInstruction()
            {
                Opcode = EnumOpcode.LDY,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 3,
                OpCodeExecution = LDY
            });
            output.Add(0xB4, new CPUInstruction()
            {
                Opcode = EnumOpcode.LDY,
                AddressingMode = EnumAddressingMode.ZeroPageX,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 4,
                OpCodeExecution = LDY
            });
            output.Add(0xAC, new CPUInstruction()
            {
                Opcode = EnumOpcode.LDY,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = LDY
            });
            output.Add(0xBC, new CPUInstruction()
            {
                Opcode = EnumOpcode.LDY,
                AddressingMode = EnumAddressingMode.AbsoluteX,
                PageBoundaryCheck = true,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = LDY
            });

            /********************
             *       LSR
             *******************/
            output.Add(0x4A, new CPUInstruction()
            {
                Opcode = EnumOpcode.LSR,
                AddressingMode = EnumAddressingMode.Accumulator,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = LSR
            });
            output.Add(0x46, new CPUInstruction()
            {
                Opcode = EnumOpcode.LSR,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 5,
                OpCodeExecution = LSR
            });
            output.Add(0x56, new CPUInstruction()
            {
                Opcode = EnumOpcode.LSR,
                AddressingMode = EnumAddressingMode.ZeroPageX,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 6,
                OpCodeExecution = LSR
            });
            output.Add(0x4E, new CPUInstruction()
            {
                Opcode = EnumOpcode.LSR,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 6,
                OpCodeExecution = LSR
            });
            output.Add(0x5E, new CPUInstruction()
            {
                Opcode = EnumOpcode.LSR,
                AddressingMode = EnumAddressingMode.AbsoluteX,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 7,
                OpCodeExecution = LSR
            });

            /********************
             *       NOP
             *******************/
            output.Add(0xEA, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = () => { return; }
            });

            /********************
             *       NOP
             * (1-Byte, Undocumented)
             *******************/
            output.Add(0x1A, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = () => { return; }
            });
            output.Add(0x3A, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = () => { return; }
            });
            output.Add(0x5A, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = () => { return; }
            });
            output.Add(0x7A, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = () => { return; }
            });
            output.Add(0xDA, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = () => { return; }
            });
            output.Add(0xFA, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = () => { return; }
            });

            /********************
             *       NOP
             * (2-Byte, Undocumented)
             *******************/
            output.Add(0x04, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = () => { return; }
            });
            output.Add(0x44, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = () => { return; }
            });
            output.Add(0x64, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = () => { return; }
            });
            output.Add(0x14, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = () => { return; }
            });
            output.Add(0x34, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = () => { return; }
            });
            output.Add(0x54, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = () => { return; }
            });
            output.Add(0x74, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = () => { return; }
            });
            output.Add(0xD4, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = () => { return; }
            });
            output.Add(0xF4, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = () => { return; }
            });
            output.Add(0x80, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = () => { return; }
            });
            output.Add(0x82, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = () => { return; }
            });
            output.Add(0x89, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = () => { return; }
            });
            output.Add(0xC2, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = () => { return; }
            });
            output.Add(0xE2, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = () => { return; }
            });

            /********************
             *       NOP
             * (3-Byte, Undocumented)
             *******************/
            output.Add(0x0C, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = () => { return; }
            });
            output.Add(0x1C, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = () => { return; }
            });
            output.Add(0x3C, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = () => { return; }
            });
            output.Add(0x5C, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = () => { return; }
            });
            output.Add(0x7C, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = () => { return; }
            });
            output.Add(0xDC, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = () => { return; }
            });
            output.Add(0xFC, new CPUInstruction()
            {
                Opcode = EnumOpcode.NOP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = () => { return; }
            });

            /********************
             *       ORA
             *******************/
            output.Add(0x09, new CPUInstruction()
            {
                Opcode = EnumOpcode.ORA,
                AddressingMode = EnumAddressingMode.Immediate,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = ORA
            });
            output.Add(0x05, new CPUInstruction()
            {
                Opcode = EnumOpcode.ORA,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 3,
                OpCodeExecution = ORA
            });
            output.Add(0x15, new CPUInstruction()
            {
                Opcode = EnumOpcode.ORA,
                AddressingMode = EnumAddressingMode.ZeroPageX,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 4,
                OpCodeExecution = ORA
            });
            output.Add(0x0D, new CPUInstruction()
            {
                Opcode = EnumOpcode.ORA,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = ORA
            });
            output.Add(0x1D, new CPUInstruction()
            {
                Opcode = EnumOpcode.ORA,
                AddressingMode = EnumAddressingMode.AbsoluteX,
                PageBoundaryCheck = true,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = ORA
            });
            output.Add(0x19, new CPUInstruction()
            {
                Opcode = EnumOpcode.ORA,
                AddressingMode = EnumAddressingMode.AbsoluteY,
                PageBoundaryCheck = true,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = ORA
            });
            output.Add(0x01, new CPUInstruction()
            {
                Opcode = EnumOpcode.ORA,
                AddressingMode = EnumAddressingMode.IndexedIndirect,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 6,
                OpCodeExecution = ORA
            });
            output.Add(0x11, new CPUInstruction()
            {
                Opcode = EnumOpcode.ORA,
                AddressingMode = EnumAddressingMode.IndirectIndexed,
                PageBoundaryCheck = true,
                Length = 2,
                Cycles = 5,
                OpCodeExecution = ORA
            });

            /********************
             *       PHA
             *******************/
            output.Add(0x48, new CPUInstruction()
            {
                Opcode = EnumOpcode.PHA,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 3,
                OpCodeExecution = PHA
            });

            /********************
             *       PHP
             *******************/
            output.Add(0x08, new CPUInstruction()
            {
                Opcode = EnumOpcode.PHP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 3,
                OpCodeExecution = PHP
            });

            /********************
             *       PLA
             *******************/
            output.Add(0x68, new CPUInstruction()
            {
                Opcode = EnumOpcode.PLA,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 4,
                OpCodeExecution = PLA
            });

            /********************
             *       PLP
             *******************/
            output.Add(0x28, new CPUInstruction()
            {
                Opcode = EnumOpcode.PLP,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 4,
                OpCodeExecution = PLP
            });

            /********************
             *       RLA
             *  (Undocumented)
             *******************/
            output.Add(0x23, new CPUInstruction()
            {
                Opcode = EnumOpcode.RLA,
                AddressingMode = EnumAddressingMode.IndexedIndirect,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 8,
                OpCodeExecution = RLA
            });
            output.Add(0x27, new CPUInstruction()
            {
                Opcode = EnumOpcode.RLA,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 5,
                OpCodeExecution = RLA
            });
            output.Add(0x2F, new CPUInstruction()
            {
                Opcode = EnumOpcode.RLA,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 6,
                OpCodeExecution = RLA
            });
            output.Add(0x33, new CPUInstruction()
            {
                Opcode = EnumOpcode.RLA,
                AddressingMode = EnumAddressingMode.IndirectIndexed,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 8,
                OpCodeExecution = RLA
            });
            output.Add(0x37, new CPUInstruction()
            {
                Opcode = EnumOpcode.RLA,
                AddressingMode = EnumAddressingMode.ZeroPageX,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 6,
                OpCodeExecution = RLA
            });
            output.Add(0x3B, new CPUInstruction()
            {
                Opcode = EnumOpcode.RLA,
                AddressingMode = EnumAddressingMode.AbsoluteY,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 7,
                OpCodeExecution = RLA
            });
            output.Add(0x3F, new CPUInstruction()
            {
                Opcode = EnumOpcode.RLA,
                AddressingMode = EnumAddressingMode.AbsoluteX,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 7,
                OpCodeExecution = RLA
            });

            /********************
             *       ROL
             *******************/
            output.Add(0x2A, new CPUInstruction()
            {
                Opcode = EnumOpcode.ROL,
                AddressingMode = EnumAddressingMode.Accumulator,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = ROL
            });
            output.Add(0x26, new CPUInstruction()
            {
                Opcode = EnumOpcode.ROL,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 5,
                OpCodeExecution = ROL
            });
            output.Add(0x36, new CPUInstruction()
            {
                Opcode = EnumOpcode.ROL,
                AddressingMode = EnumAddressingMode.ZeroPageX,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 6,
                OpCodeExecution = ROL
            });
            output.Add(0x2E, new CPUInstruction()
            {
                Opcode = EnumOpcode.ROL,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 6,
                OpCodeExecution = ROL
            });
            output.Add(0x3E, new CPUInstruction()
            {
                Opcode = EnumOpcode.ROL,
                AddressingMode = EnumAddressingMode.AbsoluteX,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 7,
                OpCodeExecution = ROL
            });

            /********************
             *       ROR
             *******************/
            output.Add(0x6A, new CPUInstruction()
            {
                Opcode = EnumOpcode.ROR,
                AddressingMode = EnumAddressingMode.Accumulator,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = ROR
            });
            output.Add(0x66, new CPUInstruction()
            {
                Opcode = EnumOpcode.ROR,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 5,
                OpCodeExecution = ROR
            });
            output.Add(0x76, new CPUInstruction()
            {
                Opcode = EnumOpcode.ROR,
                AddressingMode = EnumAddressingMode.ZeroPageX,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 6,
                OpCodeExecution = ROR
            });
            output.Add(0x6E, new CPUInstruction()
            {
                Opcode = EnumOpcode.ROR,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 6,
                OpCodeExecution = ROR
            });
            output.Add(0x7E, new CPUInstruction()
            {
                Opcode = EnumOpcode.ROR,
                AddressingMode = EnumAddressingMode.AbsoluteX,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 7,
                OpCodeExecution = ROR
            });

            /********************
             *       RRA
             *  (Undocumented)
             *******************/
            output.Add(0x63, new CPUInstruction()
            {
                Opcode = EnumOpcode.RRA,
                AddressingMode = EnumAddressingMode.IndexedIndirect,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 8,
                OpCodeExecution = RRA
            });
            output.Add(0x67, new CPUInstruction()
            {
                Opcode = EnumOpcode.RRA,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 5,
                OpCodeExecution = RRA
            });
            output.Add(0x6F, new CPUInstruction()
            {
                Opcode = EnumOpcode.RRA,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 6,
                OpCodeExecution = RRA
            });
            output.Add(0x73, new CPUInstruction()
            {
                Opcode = EnumOpcode.RRA,
                AddressingMode = EnumAddressingMode.IndirectIndexed,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 8,
                OpCodeExecution = RRA
            });
            output.Add(0x77, new CPUInstruction()
            {
                Opcode = EnumOpcode.RRA,
                AddressingMode = EnumAddressingMode.ZeroPageX,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 6,
                OpCodeExecution = RRA
            });
            output.Add(0x7B, new CPUInstruction()
            {
                Opcode = EnumOpcode.RRA,
                AddressingMode = EnumAddressingMode.AbsoluteY,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 7,
                OpCodeExecution = RRA
            });
            output.Add(0x7F, new CPUInstruction()
            {
                Opcode = EnumOpcode.RRA,
                AddressingMode = EnumAddressingMode.AbsoluteX,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 7,
                OpCodeExecution = RRA
            });

            /********************
             *       RTI
             *******************/
            output.Add(0x40, new CPUInstruction()
            {
                Opcode = EnumOpcode.RTI,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                //Since RTI sets the new program counter, we won't increment it
                Length = 0,
                Cycles = 6,
                OpCodeExecution = RTI
            });

            /********************
             *       RTS
             *******************/
            output.Add(0x60, new CPUInstruction()
            {
                Opcode = EnumOpcode.RTS,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                //Since RTS sets the new program counter, we won't increment it
                Length = 0,
                Cycles = 6,
                OpCodeExecution = RTS
            });

            /********************
             *       SAX
             *  (Undocumented)
             *******************/
            output.Add(0x83, new CPUInstruction()
            {
                Opcode = EnumOpcode.SAX,
                AddressingMode = EnumAddressingMode.IndexedIndirect,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 6,
                OpCodeExecution = SAX
            });
            output.Add(0x87, new CPUInstruction()
            {
                Opcode = EnumOpcode.SAX,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 3,
                OpCodeExecution = SAX
            });
            output.Add(0x8F, new CPUInstruction()
            {
                Opcode = EnumOpcode.SAX,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = SAX
            });
            output.Add(0x97, new CPUInstruction()
            {
                Opcode = EnumOpcode.SAX,
                AddressingMode = EnumAddressingMode.ZeroPageY,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 6,
                OpCodeExecution = SAX
            });

            /********************
             *       SBC
             *******************/
            output.Add(0xE9, new CPUInstruction()
            {
                Opcode = EnumOpcode.SBC,
                AddressingMode = EnumAddressingMode.Immediate,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = SBC
            });
            output.Add(0xEB, new CPUInstruction()
            {
                Opcode = EnumOpcode.SBC,
                AddressingMode = EnumAddressingMode.Immediate,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 2,
                OpCodeExecution = SBC
            });
            output.Add(0xE5, new CPUInstruction()
            {
                Opcode = EnumOpcode.SBC,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 3,
                OpCodeExecution = SBC
            });
            output.Add(0xF5, new CPUInstruction()
            {
                Opcode = EnumOpcode.SBC,
                AddressingMode = EnumAddressingMode.ZeroPageX,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 4,
                OpCodeExecution = SBC
            });
            output.Add(0xED, new CPUInstruction()
            {
                Opcode = EnumOpcode.SBC,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = SBC
            });
            output.Add(0xFD, new CPUInstruction()
            {
                Opcode = EnumOpcode.SBC,
                AddressingMode = EnumAddressingMode.AbsoluteX,
                PageBoundaryCheck = true,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = SBC
            });
            output.Add(0xF9, new CPUInstruction()
            {
                Opcode = EnumOpcode.SBC,
                AddressingMode = EnumAddressingMode.AbsoluteY,
                PageBoundaryCheck = true,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = SBC
            });
            output.Add(0xE1, new CPUInstruction()
            {
                Opcode = EnumOpcode.SBC,
                AddressingMode = EnumAddressingMode.IndexedIndirect,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 6,
                OpCodeExecution = SBC
            });
            output.Add(0xF1, new CPUInstruction()
            {
                Opcode = EnumOpcode.SBC,
                AddressingMode = EnumAddressingMode.IndirectIndexed,
                PageBoundaryCheck = true,
                Length = 2,
                Cycles = 5,
                OpCodeExecution = SBC
            });

            /********************
             *       SEC
             *******************/
            output.Add(0x38, new CPUInstruction()
            {
                Opcode = EnumOpcode.SEC,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = SEC
            });

            /********************
             *       SED
             *******************/
            output.Add(0xF8, new CPUInstruction()
            {
                Opcode = EnumOpcode.SED,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = SED
            });

            /********************
             *       SEI
             *******************/
            output.Add(0x78, new CPUInstruction()
            {
                Opcode = EnumOpcode.SEI,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = SEI
            });

            /********************
             *       SLO
             *  (Undocumented)
             *******************/
            output.Add(0x03, new CPUInstruction()
            {
                Opcode = EnumOpcode.SLO,
                AddressingMode = EnumAddressingMode.IndexedIndirect,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 8,
                OpCodeExecution = SLO
            });
            output.Add(0x07, new CPUInstruction()
            {
                Opcode = EnumOpcode.SLO,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 5,
                OpCodeExecution = SLO
            });
            output.Add(0x0F, new CPUInstruction()
            {
                Opcode = EnumOpcode.SLO,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 6,
                OpCodeExecution = SLO
            });
            output.Add(0x13, new CPUInstruction()
            {
                Opcode = EnumOpcode.SLO,
                AddressingMode = EnumAddressingMode.IndirectIndexed,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 8,
                OpCodeExecution = SLO
            });
            output.Add(0x17, new CPUInstruction()
            {
                Opcode = EnumOpcode.SLO,
                AddressingMode = EnumAddressingMode.ZeroPageX,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 6,
                OpCodeExecution = SLO
            });
            output.Add(0x1B, new CPUInstruction()
            {
                Opcode = EnumOpcode.SLO,
                AddressingMode = EnumAddressingMode.AbsoluteY,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 7,
                OpCodeExecution = SLO
            });
            output.Add(0x1F, new CPUInstruction()
            {
                Opcode = EnumOpcode.SLO,
                AddressingMode = EnumAddressingMode.AbsoluteX,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 7,
                OpCodeExecution = SLO
            });

            /********************
             *       SRE
             *  (Undocumented)
             *******************/
            output.Add(0x43, new CPUInstruction()
            {
                Opcode = EnumOpcode.SRE,
                AddressingMode = EnumAddressingMode.IndexedIndirect,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 8,
                OpCodeExecution = SRE
            });
            output.Add(0x47, new CPUInstruction()
            {
                Opcode = EnumOpcode.SRE,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 5,
                OpCodeExecution = SRE
            });
            output.Add(0x4F, new CPUInstruction()
            {
                Opcode = EnumOpcode.SRE,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 6,
                OpCodeExecution = SRE
            });
            output.Add(0x53, new CPUInstruction()
            {
                Opcode = EnumOpcode.SRE,
                AddressingMode = EnumAddressingMode.IndirectIndexed,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 8,
                OpCodeExecution = SRE
            });
            output.Add(0x57, new CPUInstruction()
            {
                Opcode = EnumOpcode.SRE,
                AddressingMode = EnumAddressingMode.ZeroPageX,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 6,
                OpCodeExecution = SRE
            });
            output.Add(0x5B, new CPUInstruction()
            {
                Opcode = EnumOpcode.SRE,
                AddressingMode = EnumAddressingMode.AbsoluteY,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 7,
                OpCodeExecution = SRE
            });
            output.Add(0x5F, new CPUInstruction()
            {
                Opcode = EnumOpcode.SRE,
                AddressingMode = EnumAddressingMode.AbsoluteX,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 7,
                OpCodeExecution = SRE
            });

            /********************
             *       STA
             *******************/
            output.Add(0x85, new CPUInstruction()
            {
                Opcode = EnumOpcode.STA,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 3,
                OpCodeExecution = STA
            });
            output.Add(0x95, new CPUInstruction()
            {
                Opcode = EnumOpcode.STA,
                AddressingMode = EnumAddressingMode.ZeroPageX,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 4,
                OpCodeExecution = STA
            });
            output.Add(0x8D, new CPUInstruction()
            {
                Opcode = EnumOpcode.STA,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = STA
            });
            output.Add(0x9D, new CPUInstruction()
            {
                Opcode = EnumOpcode.STA,
                AddressingMode = EnumAddressingMode.AbsoluteX,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 5,
                OpCodeExecution = STA
            });
            output.Add(0x99, new CPUInstruction()
            {
                Opcode = EnumOpcode.STA,
                AddressingMode = EnumAddressingMode.AbsoluteY,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 5,
                OpCodeExecution = STA
            });
            output.Add(0x81, new CPUInstruction()
            {
                Opcode = EnumOpcode.STA,
                AddressingMode = EnumAddressingMode.IndexedIndirect,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 6,
                OpCodeExecution = STA
            });
            output.Add(0x91, new CPUInstruction()
            {
                Opcode = EnumOpcode.STA,
                AddressingMode = EnumAddressingMode.IndirectIndexed,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 6,
                OpCodeExecution = STA
            });

            /********************
             *       STX
             *******************/
            output.Add(0x86, new CPUInstruction()
            {
                Opcode = EnumOpcode.STX,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 3,
                OpCodeExecution = STX
            });
            output.Add(0x96, new CPUInstruction()
            {
                Opcode = EnumOpcode.STX,
                AddressingMode = EnumAddressingMode.ZeroPageY,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 4,
                OpCodeExecution = STX
            });
            output.Add(0x8E, new CPUInstruction()
            {
                Opcode = EnumOpcode.STX,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = STX
            });

            /********************
             *       STY
             *******************/
            output.Add(0x84, new CPUInstruction()
            {
                Opcode = EnumOpcode.STY,
                AddressingMode = EnumAddressingMode.ZeroPage,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 3,
                OpCodeExecution = STY
            });
            output.Add(0x94, new CPUInstruction()
            {
                Opcode = EnumOpcode.STY,
                AddressingMode = EnumAddressingMode.ZeroPageX,
                PageBoundaryCheck = false,
                Length = 2,
                Cycles = 4,
                OpCodeExecution = STY
            });
            output.Add(0x8C, new CPUInstruction()
            {
                Opcode = EnumOpcode.STY,
                AddressingMode = EnumAddressingMode.Absolute,
                PageBoundaryCheck = false,
                Length = 3,
                Cycles = 4,
                OpCodeExecution = STY
            });

            /********************
             *       TAX
             *******************/
            output.Add(0xAA, new CPUInstruction()
            {
                Opcode = EnumOpcode.TAX,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = TAX
            });

            /********************
             *       TAY
             *******************/
            output.Add(0xA8, new CPUInstruction()
            {
                Opcode = EnumOpcode.TAY,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = TAY
            });

            /********************
             *       TSX
             *******************/
            output.Add(0xBA, new CPUInstruction()
            {
                Opcode = EnumOpcode.TSX,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = TSX
            });

            /********************
             *       TXA
             *******************/
            output.Add(0x8A, new CPUInstruction()
            {
                Opcode = EnumOpcode.TXA,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = TXA
            });

            /********************
             *       TXS
             *******************/
            output.Add(0x9A, new CPUInstruction()
            {
                Opcode = EnumOpcode.TXS,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = TXS
            });

            /********************
             *       TYA
             *******************/
            output.Add(0x98, new CPUInstruction()
            {
                Opcode = EnumOpcode.TYA,
                AddressingMode = EnumAddressingMode.Implicit,
                PageBoundaryCheck = false,
                Length = 1,
                Cycles = 2,
                OpCodeExecution = TYA
            });
            #endregion

            return output;
        }

        //Resets the CPU to a startup state
        //Requires the ROM to be loaded into memory first
        public void Reset()
        {
            //Set Startup States for Registers
            SP = 0xFD;
            PC = 0xC000; //This is for unit testing
            A = 0;
            X = 0;
            Y = 0;
            Status.FromByte(0x24);
            Cycles = 0;

            //Zero out memory
            for (int i = 0; i < 0x2000; i++)
            {
                CPUMemory.WriteByte(i, 0x0);
            }

            //We check if the loaded ROM has starting address
            var newProgramCounter = BitConverter.ToUInt16(new[] { CPUMemory.ReadByte(0xFFFC), CPUMemory.ReadByte(0xFFFD) }, 0);
            if (newProgramCounter != 0)
                PC = newProgramCounter;
        }


        /// <summary>
        ///     Ticks the CPU for the specified number of instructions
        /// </summary>
        /// <param name="count"></param>
        public int Tick(int count)
        {
            var totalTicks = 0;
            for (var i = 0; i < count; i++)
                totalTicks += Tick();

            return totalTicks;
        }

        /// <summary>
        ///     Ticks the CPU one instruction
        /// </summary>
        public int Tick()
        {
            //Check for NMI Interrupt
            if (NMI)
            {
                Push((ushort)PC);
                Push(Status.ToByte());
                PC = BitConverter.ToUInt16(new[] { CPUMemory.ReadByte(0xFFFA), CPUMemory.ReadByte(0xFFFB) }, 0);
                Status.InterruptDisable = true;
                NMI = false;
            }

            //Decode
            Instruction = _cpuInstructions[CPUMemory.ReadByte(PC)];

            //Execute
            Instruction.OpCodeExecution.Invoke();

            //Increment the things
            PC += Instruction.Length;
            Cycles += Instruction.Cycles;

            return Instruction.Cycles;
        }

        /// <summary>
        ///     ADC - Add with Carry
        /// 
        ///     A,Z,C,N = A+M+C
        /// 
        ///     This instruction adds the contents of a memory location to the accumulator together with the carry bit.
        ///     If overflow occurs the carry bit is set, this enables multiple byte addition to be performed.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ADC()
        {
            var value = Instruction.AddressingMode == EnumAddressingMode.Immediate ? GetOperandByte() : CPUMemory.ReadByte(ResolveAddress());

            unchecked
            {
                var newA = (sbyte)A + (sbyte)value + (Status.Carry ? 1 : 0);
                Status.Zero = (byte)newA == 0;
                Status.Negative = ((byte)newA).IsNegative();
                Status.Carry = A + value + (Status.Carry ? 1 : 0) > byte.MaxValue;
                Status.Overflow = newA > 127 || newA < -128;
                A = (byte)newA;
            }
        }

        /// <summary>
        ///     AND - Logical AND
        /// 
        ///     A,Z,N = A &amp; M
        /// 
        ///     A logical AND is performed, bit by bit, on the accumulator contents using the contents of a byte of memory.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AND()
        {
            A &= Instruction.AddressingMode == EnumAddressingMode.Immediate ? GetOperandByte() : CPUMemory.ReadByte(ResolveAddress());
            Status.Negative = A.IsNegative();
            Status.Zero = A == 0;
        }

        /// <summary>
        ///     ASL - Arithmetic Shift Left
        ///
        ///     A,Z,C,N = M*2 or M, Z, C, N = M * 2
        /// 
        ///     This operation shifts all the bits of the accumulator or memory contents one bit left.Bit 0 is set to 0 and bit 7 is placed in the
        ///     carry flag. The effect of this operation is to multiply the memory contents by 2 (ignoring 2's complement considerations), setting
        ///     the carry if the result will not fit in 8 bits.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ASL()
        {
            var address = ResolveAddress();
            var value = Instruction.AddressingMode == EnumAddressingMode.Accumulator ? A : CPUMemory.ReadByte(address);

            Status.Carry = value >> 7 == 1;

            //Shift Left by 1
            value <<= 1;

            if (Instruction.AddressingMode != EnumAddressingMode.Accumulator)
            {
                CPUMemory.WriteByte(address, value);
            }
            else
            {
                A = value;
            }

            Status.Zero = value == 0;
            Status.Negative = value.IsNegative();
        }

        /// <summary>
        ///     BCC - Branch if Carry Clear
        /// 
        ///     If the carry flag is clear then add the relative displacement to the program counter to cause a branch to a new location.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BCC()
        {
            if (Status.Carry)
                return;

            //+1 cycle for success
            Cycles++;

            PC = ResolveAddress();
        }

        /// <summary>
        ///     BCS - Branch if Carry Set
        ///     
        ///     If the carry flag is set then add the relative displacement to the program counter to cause a branch to a new location.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BCS()
        {
            if (!Status.Carry)
                return;

            //+1 cycle for success
            Cycles++;

            PC = ResolveAddress();
        }

        /// <summary>
        ///     BEQ - Branch if Equal
        /// 
        ///     If the zero flag is set then add the relative displacement to the program counter to cause a branch to a new location.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BEQ()
        {
            if (!Status.Zero)
                return;

            //+1 cycle for success
            Cycles++;

            PC = ResolveAddress();
        }

        /// <summary>
        ///     BIT - Bit Test
        /// 
        ///     A &amp; M, N = M7, V = M6
        ///     
        ///     This instructions is used to test if one or more bits are set in a target memory location.The mask pattern in A is ANDed with the 
        ///     value in memory to set or clear the zero flag, but the result is not kept. Bits 7 and 6 of the value from memory are copied 
        ///     into the N and V flags.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BIT()
        {
            var value = CPUMemory.ReadByte(ResolveAddress());

            Status.Negative = value.IsNegative();
            Status.Overflow = (value & (1 << 6)) != 0;
            Status.Zero = (A & value) == 0;
        }

        /// <summary>
        ///     BMI - Branch if Minus
        /// 
        ///     If the negative flag is set then add the relative displacement to the program counter to cause a branch to a new location.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BMI()
        {
            if (!Status.Negative)
                return;

            //+1 cycle for success
            Cycles++;

            PC = ResolveAddress();
        }

        /// <summary>
        ///     BNE - Branch if Not Equal
        ///     
        ///     If the zero flag is clear then add the relative displacement to the program counter to cause a branch to a new location.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BNE()
        {
            if (Status.Zero)
                return;

            //+1 cycle for success
            Cycles++;

            PC = ResolveAddress(); 
        }

        /// <summary>
        ///     BPL - Branch if Positive
        /// 
        ///     If the negative flag is clear then add the relative displacement to the program counter to cause a branch to a new location.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BPL()
        {
            if (Status.Negative)
                return;

            //+1 cycle for success
            Cycles++;

            PC = ResolveAddress();
        }

        /// <summary>
        ///     BRK - Force Interrupt
        /// 
        ///     The BRK instruction forces the generation of an interrupt request. The program counter and processor status are pushed 
        ///     on the stack then the IRQ interrupt vector at $FFFE/F is loaded into the PC and the break flag in the status set to one.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BRK()
        {
            Push((ushort)PC);
            Push((byte) (Status.ToByte() | 0b00010000));
            Status.InterruptDisable = true;
            PC = GetWord(0xFFFE);
        }

        /// <summary>
        ///     BVC - Branch if Overflow Clear
        /// 
        ///     If the overflow flag is clear then add the relative displacement to the program counter to cause a branch to a new 
        ///     location.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BVC()
        {
            if (Status.Overflow)
                return;

            //+1 cycle for success
            Cycles++;

            PC = ResolveAddress();
        }

        /// <summary>
        ///     BVS - Branch if Overflow Set
        ///     
        ///     If the overflow flag is set then add the relative displacement to the program counter to cause a branch to a new 
        ///     location.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BVS()
        {
            if (!Status.Overflow)
                return;

            //+1 cycle for success
            Cycles++;

            PC = ResolveAddress();
        }

        /// <summary>
        ///     CLC - Clear Carry Flag
        /// 
        ///     C = 0
        /// 
        ///     Set the carry flag to zero.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CLC() => Status.Carry = false;

        /// <summary>
        ///     CLD - Clear Decimal Flag
        /// 
        ///     D = 0
        /// 
        ///     Set the decimal flag to zero.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CLD() => Status.DecimalMode = false;

        /// <summary>
        ///     CLI - Clear Interrupt Flag
        /// 
        ///     I = 0
        /// 
        ///     Set the Interrupt flag to zero.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CLI() => Status.InterruptDisable = false;

        /// <summary>
        ///     CLV - Clear Overflow Flag
        /// 
        ///     V = 0
        /// 
        ///     Set the Overflow flag to zero.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CLV() => Status.Overflow = false;

        /// <summary>
        ///     CMP - Compare
        /// 
        ///     Z,C,N = A-M
        /// 
        ///     This instruction compares the contents of the accumulator with another memory held value and sets the zero and 
        ///     carry flags as appropriate.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CMP()
        {
            var value = Instruction.AddressingMode == EnumAddressingMode.Immediate ? GetOperandByte() : CPUMemory.ReadByte(ResolveAddress());

            Status.Carry = (A >= value);
            Status.Zero = (A == value);
            unchecked
            {
                Status.Negative = ((byte)(A - value)).IsNegative();
            }   
        }

        /// <summary>
        ///     CPX - Compare X Register
        /// 
        ///     Z,C,N = X-M
        /// 
        ///     This instruction compares the contents of the X register with another memory held value and sets the zero and carry flags as appropriate.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CPX()
        {
            var value = Instruction.AddressingMode == EnumAddressingMode.Immediate ? GetOperandByte() : CPUMemory.ReadByte(ResolveAddress());

            Status.Carry = X >= value;
            Status.Zero = X == value;
            unchecked
            {
                Status.Negative = ((byte) (X - value)).IsNegative();
            }
        }

        /// <summary>
        ///     CPY - Compare Y Register
        /// 
        ///     Z,C,N = Y-M
        /// 
        ///     This instruction compares the contents of the Y register with another memory held value and sets the zero and carry flags as appropriate.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CPY()
        {
            var value = Instruction.AddressingMode == EnumAddressingMode.Immediate ? GetOperandByte() : CPUMemory.ReadByte(ResolveAddress());

            Status.Carry = Y >= value;
            Status.Zero = Y == value;
            unchecked
            {
                Status.Negative = ((byte)(Y - value)).IsNegative();
            }
        }

        /// <summary>
        ///     DCP - Undocumented OpCode
        ///
        ///     The read-modify-write instructions (INC, DEC, ASL, LSR, ROL, ROR) have few valid addressing modes, but these instructions have three more: (d,X),
        ///     (d),Y, and a,Y. In some cases, it could be worth it to use these and ignore the side effect on the accumulator.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DCP()
        {
            DEC();
            CMP();
        }

        /// <summary>
        ///     DEC - Decrement Memory
        /// 
        ///     M,Z,N = M-1
        /// 
        ///     Subtracts one from the value held at a specified memory location setting the zero and negative flags as appropriate.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DEC()
        {
            var address = ResolveAddress();
            var value = Instruction.AddressingMode == EnumAddressingMode.Immediate ? GetOperandByte() : CPUMemory.ReadByte(address);

            unchecked
            {
                value--;
            }

            Status.Zero = value == 0;
            Status.Negative = value.IsNegative();
            CPUMemory.WriteByte(address, value);
        }

        /// <summary>
        ///     DEX - Decrement X Register
        /// 
        ///     X,Z,N = X-1
        /// 
        ///     Subtracts one from the X register setting the zero and negative flags as appropriate.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DEX()
        {
            unchecked
            {
                X--;
            }

            Status.Zero = X == 0;
            Status.Negative = X.IsNegative();
        }

        /// <summary>
        ///     DEY - Decrement Y Register
        /// 
        ///     Y,Z,N = Y-1
        /// 
        ///     Subtracts one from the Y register setting the zero and negative flags as appropriate.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DEY()
        {
            unchecked
            {
                Y--;
            }

            Status.Zero = Y == 0;
            Status.Negative = Y.IsNegative();
        }

        /// <summary>
        ///     EOR - Exclusive OR
        /// 
        ///     A,Z,N = A^M
        /// 
        ///     An exclusive OR is performed, bit by bit, on the accumulator contents using the contents of a byte of memory.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EOR()
        {
            A ^= Instruction.AddressingMode == EnumAddressingMode.Immediate ? GetOperandByte() : CPUMemory.ReadByte(ResolveAddress());
            Status.Zero = A == 0;
            Status.Negative = A.IsNegative();
        }

        /// <summary>
        ///     INC - Increment Memory
        /// 
        ///     M,Z,N = M+1
        /// 
        ///     Adds one to the value held at a specified memory location setting the zero and negative flags as appropriate.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void INC()
        {
            var address = ResolveAddress();
            var value = Instruction.AddressingMode == EnumAddressingMode.Immediate ? GetOperandByte() : CPUMemory.ReadByte(address);

            unchecked
            {
                value++;
            }

            Status.Zero = value == 0;
            Status.Negative = value.IsNegative();
            CPUMemory.WriteByte(address, value);
        }

        /// <summary>
        ///     INX - Increment X Register
        ///     
        ///     X,Z,N = X+1
        /// 
        ///     Adds one to the X register setting the zero and negative flags as appropriate.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void INX()
        {
            unchecked
            {
                X++;
            }

            Status.Zero = X == 0;
            Status.Negative = X.IsNegative();
        }

        /// <summary>
        ///     INY - Increment Y Register
        /// 
        ///      Y,Z,N = Y+1
        /// 
        ///     Adds one to the Y register setting the zero and negative flags as appropriate.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void INY()
        {
            unchecked
            {
                Y++;
            }

            Status.Zero = Y == 0;
            Status.Negative = Y.IsNegative();
        }

        /// <summary>
        ///     ISB - Undocumented Opcode
        ///
        ///     Equivalent to INC value then SBC value, except supporting more addressing modes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ISB()
        {
            INC();
            SBC();
        }

        /// <summary>
        ///     JMP - Jump
        /// 
        ///     Sets the program counter to the address specified by the operand.
        /// 
        ///     NOTE: An original 6502 has does not correctly fetch the target address if the indirect vector falls on a page 
        ///     boundary (e.g. $xxFF where xx is any value from $00 to $FF). In this case fetches the LSB from $xxFF as expected 
        ///     but takes the MSB from $xx00. This is fixed in some later chips like the 65SC02 so for compatibility always ensure 
        ///     the indirect vector is not at the end of the page.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void JMP()
        {
            //Properly set address if it falls on a page boundary
            if (Instruction.AddressingMode == EnumAddressingMode.Indirect && (GetOperandWord() & 0xFF) == 0xFF)
            {
                PC = GetWord(GetOperandWord(), true);
            }
            else
            {
                PC = ResolveAddress();
            }
        }

        /// <summary>
        ///     JSR - Jump to Subroutine
        /// 
        ///     The JSR instruction pushes the address(minus one) of the return point on to the stack and then sets the program counter 
        ///     to the target memory address.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void JSR()
        {
            Push((ushort) (PC+2));
            PC = ResolveAddress();
        }

        /// <summary>
        ///     LAX - Undocumented Opcode
        ///
        ///     Shortcut for LDA value then TAX. Saves a byte and two cycles and allows use of the X register
        ///     with the (d),Y addressing mode. Notice that the immediate is missing; the opcode that would
        ///     have been LAX is affected by line noise on the data bus. MOS 6502: even the bugs have bugs.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LAX()
        {
            LDA();
            TAX();
        }

        /// <summary>
        ///     LDA - Load Accumulator
        /// 
        ///     A,Z,N = M
        /// 
        ///     Loads a byte of memory into the accumulator setting the zero and negative flags as appropriate.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LDA()
        {
            A = Instruction.AddressingMode == EnumAddressingMode.Immediate ? GetOperandByte() : CPUMemory.ReadByte(ResolveAddress());

            Status.Zero = A == 0;
            Status.Negative = A.IsNegative();
        }

        /// <summary>
        ///     LDX - Load X Register
        /// 
        ///     X,Z,N = M
        /// 
        ///     Loads a byte of memory into the X register setting the zero and negative flags as appropriate.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LDX()
        {
            X = Instruction.AddressingMode == EnumAddressingMode.Immediate ? GetOperandByte() : CPUMemory.ReadByte(ResolveAddress());

            Status.Zero = X == 0;
            Status.Negative = X.IsNegative();
        }

        /// <summary>
        ///     LDY - Load Y Register
        /// 
        ///     Y,Z,N = M
        /// 
        ///     Loads a byte of memory into the Y register setting the zero and negative flags as appropriate.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LDY()
        {
            Y = Instruction.AddressingMode == EnumAddressingMode.Immediate ? GetOperandByte() : CPUMemory.ReadByte(ResolveAddress());

            Status.Zero = (Y == 0);
            Status.Negative = Y.IsNegative();
        }

        /// <summary>
        ///     LSR - Logical Shift Right
        /// 
        ///     A,C,Z,N = A/2 or M, C, Z, N = M / 2
        /// 
        ///     Each of the bits in A or M is shift one place to the right.
        ///     The bit that was in bit 0 is shifted into the carry flag.
        ///     Bit 7 is set to zero.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LSR()
        {
            if (Instruction.AddressingMode == EnumAddressingMode.Accumulator)
            {
                Status.Carry = (A & 0x01) > 0;
                A >>= 1;
                Status.Zero = A == 0;
                Status.Negative = A.IsNegative();
                return;
            }

            var address = ResolveAddress();
            var value = CPUMemory.ReadByte(address);

            Status.Carry = (value & 0x01) == 1;
            value >>= 1;
            Status.Zero = value == 0;
            Status.Negative = value.IsNegative();

            CPUMemory.WriteByte(address, value);
        }

        /// <summary>
        ///     ORA - Logical Inclusive OR
        ///     
        ///     A,Z,N = A|M
        ///     An inclusive OR is performed, bit by bit, on the accumulator contents using the contents of a byte of memory.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ORA()
        {
            A |= Instruction.AddressingMode == EnumAddressingMode.Immediate ? GetOperandByte() : CPUMemory.ReadByte(ResolveAddress());
            Status.Negative = A.IsNegative();
            Status.Zero = A == 0;
        }

        /// <summary>
        ///     PHA - Push Accumulator
        /// 
        ///     Pushes a copy of the accumulator on to the stack.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PHA() => Push(A);

        /// <summary>
        ///     PHP - Push Processor Status
        /// 
        ///     Pushes a copy of the status flags on to the stack.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PHP() => Push((byte) (Status.ToByte() | 0b00010000));

        /// <summary>
        ///     PLA - Pull Accumulator
        /// 
        ///     Pulls an 8 bit value from the stack and into the accumulator.
        ///     The zero and negative flags are set as appropriate.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PLA()
        {
            A = PopByte();
            Status.Zero = A == 0;
            Status.Negative = A.IsNegative();
        }

        /// <summary>
        ///     PLP - Pull Processor Status
        ///     
        ///     Pulls an 8 bit value from the stack and into the processor flags.
        ///     The flags will take on new states as determined by the value pulled.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PLP() => Status.FromByte((byte) (PopByte() & ~0b00010000));

        /// <summary>
        ///     RLA - Undocumented Opcode
        ///
        ///     Equivalent to ROL value then AND value, except supporting more addressing modes.
        ///     LDA #$FF followed by RLA is an efficient way to rotate a variable while also loading it in A.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RLA()
        {
            ROL();
            AND();
        }

        /// <summary>
        ///     ROL - Rotate Left
        /// 
        ///     Move each of the bits in either A or M one place to the left. Bit 0 is filled with the current value of the carry flag
        ///     whilst the old bit 7 becomes the new carry flag value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ROL()
        {
            var value = Instruction.AddressingMode == EnumAddressingMode.Accumulator
                ? A
                : CPUMemory.ReadByte(ResolveAddress());

            //Shift Bits to new value
            var newValue = (byte)(value << 1);

            //New Bit 0 filled with current value of Carry Flag
            if (Status.Carry)
                newValue |= 0x01;

            //Old Bit 7 becomes new carry flag
            Status.Carry = value >> 7 == 1;
            
            //Set Negative Flag on new Bit 7 value
            Status.Negative = newValue.IsNegative();

            //Set Zero Flag on new value
            Status.Zero = newValue == 0;

            //Save New Value
            if (Instruction.AddressingMode == EnumAddressingMode.Accumulator)
            {
                A = newValue;
            }
            else
            {
                CPUMemory.WriteByte(ResolveAddress(), newValue);
            }
        }

        /// <summary>
        ///     ROR - Rotate Right
        /// 
        ///     Move each of the bits in either A or M one place to the right. Bit 7 is filled with the current value
        ///     of the carry flag whilst the old bit 0 becomes the new carry flag value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ROR()
        {
            var value = Instruction.AddressingMode == EnumAddressingMode.Accumulator
                ? A
                : CPUMemory.ReadByte(ResolveAddress());

            //Shift Bits to new value
            var newValue = (byte)(value >> 1);

            //New Bit 0 filled with current value of Carry Flag
            if (Status.Carry)
                newValue |= 0x80;

            //Old Bit 0 becomes new carry flag
            Status.Carry = (byte)(value << 7) == 0x80;

            //Set Negative Flag on new Bit 7 value
            Status.Negative = newValue.IsNegative();

            //Set Zero Flag on new value
            Status.Zero = newValue == 0;

            //Save New Value
            if (Instruction.AddressingMode == EnumAddressingMode.Accumulator)
            {
                A = newValue;
            }
            else
            {
                CPUMemory.WriteByte(ResolveAddress(), newValue);
            }
        }

        /// <summary>
        ///     RRA - Undocumented Opcode
        ///
        ///     Equivalent to ROR value then ADC value, except supporting more addressing modes. Essentially
        ///     this computes A + value / 2, where value is 9-bit and the division is rounded up.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RRA()
        {
            ROR();
            ADC();
        }

        /// <summary>
        ///     RTI - Return from Interrupt
        ///
        ///     The RTI instruction is used at the end of an interrupt processing routine.It pulls the processor flags
        ///     from the stack followed by the program counter.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RTI()
        {
            Status.FromByte(PopByte());
            PC = PopWord();
        }

        /// <summary>
        ///     RTS - Return from Subroutine
        ///
        ///     The RTS instruction is used at the end of a subroutine to return to the calling routine. It pulls the
        ///     program counter (minus one) from the stack.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RTS() => PC = PopWord() + 1;

        /// <summary>
        ///     SAX - Undocumented Opcode
        ///
        ///     Stores the bitwise AND of A and X. As with STA and STX, no flags are affected.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SAX() =>
            CPUMemory.WriteByte(ResolveAddress(), (byte) (A & X));

        /// <summary>
        ///     SBC - Subtract with Carry
        ///
        ///     A,Z,C,N = A-M-(1-C)
        ///
        ///     This instruction subtracts the contents of a memory location to the accumulator together with the not
        ///     of the carry bit. If overflow occurs the carry bit is clear, this enables multiple byte subtraction to be performed.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SBC()
        {
            var value = Instruction.AddressingMode == EnumAddressingMode.Immediate ? GetOperandByte() : CPUMemory.ReadByte(ResolveAddress());

            unchecked
            {
                var newA = (sbyte)A - (sbyte)value - (1- (Status.Carry ? 1 : 0));

                Status.Zero = (byte)newA == 0;
                Status.Negative = ((byte)newA).IsNegative();
                Status.Carry = A - value - (1 - (Status.Carry ? 1 : 0)) >=  byte.MinValue && A - value - (1 - (Status.Carry ? 1 : 0)) <= byte.MaxValue ;
                Status.Overflow = newA > 127 || newA < -128;
                A = (byte)newA;
            }
        }

        /// <summary>
        ///     SEC - Set Carry Flag
        ///
        ///     Set the carry flag to one.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SEC() => Status.Carry = true;

        /// <summary>
        ///     SED - Set Decimal Flag
        ///
        ///     Set the decimal mode flag to one.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SED() => Status.DecimalMode = true;

        /// <summary>
        ///     SEI - Set Interrupt Disable
        ///
        ///     Set the interrupt disable flag to one.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SEI() => Status.InterruptDisable = true;

        /// <summary>
        ///     LSO - Undocumented Opcode
        /// 
        ///     Equivalent to ASL value then ORA value, except supporting more addressing modes.
        ///     LDA #0 followed by SLO is an efficient way to shift a variable while also loading it in A.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SLO()
        {
            ASL();
            ORA();
        }

        /// <summary>
        ///     SRE - Undocumented Opcode
        ///
        ///     Equivalent to LSR value then EOR value, except supporting more addressing modes. LDA #0 followed
        ///     by SRE is an efficient way to shift a variable while also loading it in A.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SRE()
        {
            LSR();
            EOR();
        }

        /// <summary>
        ///     STA - Store Accumulator
        ///
        ///     M = A
        /// 
        ///     Stores the contents of the accumulator into memory.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void STA() => CPUMemory.WriteByte(ResolveAddress(), A);


        /// <summary>
        ///     STX - Store Accumulator
        ///
        ///     M = X
        /// 
        ///     Stores the contents of the X register into memory.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void STX() => CPUMemory.WriteByte(ResolveAddress(), X);


        /// <summary>
        ///     STY - Store Accumulator
        ///
        ///     M = Y
        /// 
        ///     Stores the contents of the Y register into memory.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void STY() => CPUMemory.WriteByte(ResolveAddress(), Y);


        /// <summary>
        ///     TAX - Transfer Accumulator to X
        ///
        ///     X = A
        /// 
        ///     Copies the current contents of the accumulator into the X register and sets the zero
        ///     and negative flags as appropriate.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TAX()
        {
            X = A;
            Status.Zero = X == 0;
            Status.Negative = X.IsNegative();
        }

        /// <summary>
        ///     TAY - Transfer Accumulator to Y
        ///
        ///     Y = A
        /// 
        ///     Copies the current contents of the accumulator into the Y register and sets the zero
        ///     and negative flags as appropriate.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TAY()
        {
            Y = A;
            Status.Zero = Y == 0;
            Status.Negative = Y.IsNegative();
        }

        /// <summary>
        ///     TSX - Stack Pointer to X
        ///
        ///     X = SP
        /// 
        ///     Copies the current contents of the Stack Pointer into the X register and sets the zero
        ///     and negative flags as appropriate.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TSX()
        {
            X = SP;
            Status.Zero = X == 0;
            Status.Negative = X.IsNegative();
        }

        /// <summary>
        ///     TXA - Transfer X Register to Accumulator
        ///
        ///     A = X
        /// 
        ///     Copies the current contents of the X register into the accumulator and sets the zero
        ///     and negative flags as appropriate.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TXA()
        {
            A = X;
            Status.Zero = A == 0;
            Status.Negative = A.IsNegative();
        }

        /// <summary>
        ///     TXS - Transfer X to Stack Pointer
        ///
        ///     SP = X
        /// 
        ///     Copies the current contents of the X register into the stack register.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TXS() => SP = X;

        /// <summary>
        ///     TYA - Transfer Y Register to Accumulator
        ///
        ///     A = Y
        /// 
        ///     Copies the current contents of the Y register into the accumulator and sets the zero
        ///     and negative flags as appropriate.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TYA()
        {
            A = Y;
            Status.Zero = A == 0;
            Status.Negative = A.IsNegative();
        }

        /// <summary>
        ///     Pushes a byte value to the stack location and decrements the stack pointer
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Push(byte value)
        {
            CPUMemory.WriteByte( STACK_BASE + SP, value);
            SP--;
        }

        /// <summary>
        ///  Pushes a word value to the stack location and decrements the stack pointer
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Push(ushort value)
        {
            CPUMemory.WriteByte(STACK_BASE + SP, (byte)(value >> 8));
            SP--;
            CPUMemory.WriteByte(STACK_BASE + SP, (byte)(value & 0xFF));
            SP--;
        }

        /// <summary>
        ///     Pops a byte value from the stack location and increments the stack pointer
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte PopByte()
        {
            SP++;
            return CPUMemory.ReadByte(STACK_BASE + SP);
        }

        /// <summary>
        ///     Pops a word value from the stack location and increments the stack pointer
        ///
        ///     Overload for PopByte, called twice and return value is cast into a ushort
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ushort PopWord() => BitConverter.ToUInt16(new[] {PopByte(), PopByte()}, 0);

        /// <summary>
        ///     Helper method to get the byte operand for an opcode
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte GetOperandByte()
        {
            Instruction.Operand = CPUMemory.ReadByte(PC + 1);
            return (byte) Instruction.Operand;
        }

        /// <summary>
        ///     Helper method to get the sbyte operand for an opcode
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private sbyte GetOperandSByte()
        {
            Instruction.Operand = (ushort)GetSbyte(PC + 1);
            return (sbyte) Instruction.Operand;
        }

        /// <summary>
        ///     Helper method to get the word operand for an opcode
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ushort GetOperandWord()
        {
            Instruction.Operand = GetWord((ushort)(PC + 1));
            return (ushort)Instruction.Operand;
        } 

        /// <summary>
        ///     Returns the address based on the specified Addressing Mode and the operand
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ResolveAddress()
        {
            switch (Instruction.AddressingMode)
            {
                case EnumAddressingMode.Absolute:
                    return GetOperandWord();
                case EnumAddressingMode.Immediate:
                    return PC + 1;
                case EnumAddressingMode.Accumulator:
                    return A;
                case EnumAddressingMode.ZeroPage:
                    return GetOperandByte();
                case EnumAddressingMode.ZeroPageX:
                    byte zeroPageXAddress;
                    unchecked
                    {
                        zeroPageXAddress = GetOperandByte();
                        zeroPageXAddress += X;
                    }
                    return zeroPageXAddress;
                case EnumAddressingMode.ZeroPageY:
                    byte zeroPageYAddress;
                    unchecked
                    {
                        zeroPageYAddress = GetOperandByte();
                        zeroPageYAddress += Y;
                    }
                    return zeroPageYAddress;
                case EnumAddressingMode.Relative:
                    if (Instruction.PageBoundaryCheck)
                        CheckBoundary(GetOperandSByte() + PC, PC);
                    return  GetOperandSByte() + PC;
                case EnumAddressingMode.AbsoluteX:
                    if (Instruction.PageBoundaryCheck)
                        CheckBoundary(GetOperandWord() + X, GetOperandWord());
                    return  GetOperandWord() + X;
                case EnumAddressingMode.AbsoluteY:
                    if (Instruction.PageBoundaryCheck)
                        CheckBoundary(GetOperandWord() + Y, GetOperandWord());

                    var targetAbsoluteYOffset = GetOperandWord();
                    //Check special case where 0xFFFF wraps back to 0x0000
                    if (targetAbsoluteYOffset == 0xFFFF)
                    {
                        return Y - 1;
                    }

                    return targetAbsoluteYOffset + Y;
                case EnumAddressingMode.Indirect:
                    return GetWord(GetOperandWord());
                case EnumAddressingMode.IndexedIndirect:
                    byte address;
                    unchecked
                    {
                        address = GetOperandByte();
                        address += X;
                    }
                    return GetWord(address, true);
                case EnumAddressingMode.IndirectIndexed:
                    if (Instruction.PageBoundaryCheck)
                        CheckBoundary( GetWord(GetOperandByte(), true) + Y, GetWord(GetOperandByte(), true));

                    var targetOffset = GetWord(GetOperandByte(), true);
                    //Check special case where 0xFFFF wraps back to 0x0000
                    if (targetOffset == 0xFFFF)
                    {
                        return Y-1;
                    }

                    return targetOffset + Y;
                    
                default:
                    throw new Exception("Unknown Addressing Mode");
            }
        }

        /// <summary>
        ///     Checks two values to see if the most significant bit is different denoting a Page Boundary will be crossed
        /// </summary>
        /// <param name="address1"></param>
        /// <param name="address2"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckBoundary(int address1, int address2)
        {
            if (address1 >> 8 != address2 >> 8)
                Cycles++;
        }

        /// <summary>
        ///     Helper Method that returns an unsigned word
        /// </summary>
        /// <param name="address"></param>
        /// <param name="pageWrap"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ushort GetWord(int address, bool pageWrap = false)
        {
            if (pageWrap && (address | 0xFF) == address)
            {
                return BitConverter.ToUInt16(
                    new[] {CPUMemory.ReadByte(address), CPUMemory.ReadByte(address & ~0xFF)},
                    0);
            }

            return BitConverter.ToUInt16(new[] { CPUMemory.ReadByte(address), CPUMemory.ReadByte(address +1) }, 0);
        }

        /// <summary>
        ///     Helper Method that returns the byte from memory as an sbyte
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private sbyte GetSbyte(int address)
        {
            unchecked
            {
                return (sbyte)CPUMemory.ReadByte(address);
            }
        }
    }
}