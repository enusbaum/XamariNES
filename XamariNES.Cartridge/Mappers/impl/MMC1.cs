using System;
using System.Runtime.CompilerServices;
using XamariNES.Cartridge.Mappers.Enums;
using XamariNES.Common.Extensions;

namespace XamariNES.Cartridge.Mappers.impl
{
    /// <summary>
    ///     NES Mapper 1 (MMC1)
    ///
    ///     More Info: https://wiki.nesdev.com/w/index.php/MMC1
    /// </summary>
    public class MMC1 : MapperBase, IMapper
    {
        /// <summary>
        ///     PRG ROM
        ///
        ///     256kb Capacity
        /// </summary>
        private readonly byte[] _prgRom;

        /// <summary>
        ///     Number of PRG ROM Banks on this Cartridge
        /// </summary>
        private readonly int _prgRomBanks;

        /// <summary>
        ///     PRG RAM
        ///
        ///     32kb Capacity
        /// </summary>
        private readonly byte[] _prgRam = new byte[0x8000];

        /// <summary>
        ///     CHR ROM
        ///
        ///     128kb Capacity
        /// </summary>
        private readonly byte[] _chrRom;

        //Registers
        private int _registerShift;
        private int _registerShiftOffset;
        private int _registerControl;
        private int _chrBank0;
        private int _chrBank1;
        private int _prgBank;

        //Bank Switching Modes for CHR and PRG
        private int _currentPrgMode;
        private int _currentChrMode;

        //Current Offset of the banks in the total bank memory space
        private int _chrBank0Offset;
        private int _chrBank1Offset;
        private int _prgBank0Offset;
        private int _prgBank1Offset;

        //Toggles for RAM
        private readonly bool _useChrRam;
        private bool _usePrgRam;

        public enumNametableMirroring NametableMirroring { get; set; }

        public MMC1(int prgRomBanks, byte[] chrRom, byte[] prgRom, bool useChrRam, bool usePrgRam,
            enumNametableMirroring mirroring = enumNametableMirroring.Horizontal)
        {
            _prgRomBanks = prgRomBanks;
            _chrRom = chrRom;
            _prgRom = prgRom;
            NametableMirroring = mirroring;
            _useChrRam = useChrRam;
            _usePrgRam = usePrgRam;

            //Set Startup Values
            _registerShift = 0x0C;
            _prgBank1Offset = (_prgRomBanks - 1) * 0x4000;
        }

        /// <summary>
        ///     Reads one byte from the specified bank, at the specified offset
        /// </summary>
        /// <param name="memoryType"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public byte ReadByte(int offset)
        {
            // CHR Bank 0 == $0000-$0FFF
            // CHR Bank 1 == $1000-$1FFF
            if (offset <= 0x1FFF)
            {
                var chrBankOffset = offset / 0x1000 == 0 ? _chrBank0Offset : _chrBank1Offset;
                chrBankOffset += offset % 0x1000;
                return _chrRom[chrBankOffset];
            }

            //PPU Registers
            if (offset <= 0x3FFF)
                return ReadInterceptors.TryGetValue(offset, out currentReadInterceptor) ? currentReadInterceptor(offset) : (byte) 0x0;

            // PRG RAM Bank == $6000-$7FFF
            if (offset >= 0x6000 && offset <= 0x7FFF)
            {
                if(!_usePrgRam)
                    throw new AccessViolationException($"Attempt to read PRG RAM when disabled. Offset ${offset:X4}");

                return _prgRam[offset - 0x6000];
            }

            // PRG Bank 0 == $8000-$BFFF
            // PRG Bank 1 == $C000-$FFFF
            if (offset >= 0x8000 && offset <= 0xFFFF)
            {
                //Map address $8000 to our _prgRom start of 0x0000;
                var prgBaseOffset = offset - 0x8000;
                var prgBankOffset = prgBaseOffset / 0x4000 == 0 ? _prgBank0Offset : _prgBank1Offset;
                prgBankOffset += prgBaseOffset % 0x4000;
                return _prgRom[prgBankOffset];
            }

            throw new ArgumentOutOfRangeException(nameof(offset), offset, "Maximum value of offset is 0xFFFF");
        }

        /// <summary>
        ///     Writes one byte to the specified bank, at the specified offset
        /// </summary>
        /// <param name="memoryType"></param>
        /// <param name="offset"></param>
        /// <param name="data"></param>
        public void WriteByte(int offset, byte data)
        {
            // CHR Bank 0 == $0000-$0FFF
            // CHR Bank 1 == $1000-$1FFF
            if (offset <= 0x1FFF)
            {
                if(!_useChrRam)
                    throw new AccessViolationException($"Invalid write to CHR ROM (CHR RAM not enabled). Offset: {offset:X4}");

                var chrOffset = (offset / 0x1000) == 0 ? _chrBank0Offset : _chrBank1Offset;
                chrOffset += offset % 0x1000;
                _chrRom[chrOffset] = data;
                return;
            }

            //PPU Registers
            if (offset <= 0x3FFF || offset == 0x4014)
            {
                if (WriteInterceptors.TryGetValue(offset, out currentWriteInterceptor))
                    currentWriteInterceptor(offset, data);

                return;
            }

            // PRG RAM Bank == $6000-$7FFF
            if (offset >= 0x6000 && offset <= 0x7FFF)
            {
                if (!_usePrgRam)
                    throw new AccessViolationException($"Attempt to write PRG RAM when disabled. Offset ${offset:X4}");

                _prgRam[offset - 0x6000] = data;
                return;
            }

            //Writes to this range are handled by the Load Register
            if (offset >= 0x8000 && offset <= 0xFFFF)
            {
                WriteLoadRegister(offset, data);
                return;
            }

            //Sanity Check if we reach this point
            throw new ArgumentOutOfRangeException(nameof(offset), "Maximum value of offset is 0xFFFF");
        }

        /// <summary>
        ///     Load Register is mapped to $8000->$FFFF in the Mapper Memory Space
        ///
        ///     From there, the Load Register processes the write one bit at a time shifting left, until 5 writes
        ///     at which point we write to the internal register mapped to the given address of the final write.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="data"></param>
        private void WriteLoadRegister(int offset, byte data)
        {
            if (data.IsBitSet(7))
            {
                //Write Control with (Control OR $0C),
                //locking PRG ROM at $C000-$FFFF to the last bank.
                _registerShift = _registerControl | 0x0C;
                WriteInternalRegister(0x0);

                //Reset Shift Register
                _registerShiftOffset = 0;
                _registerShift = 0;
                return;
            }

            _registerShift |= (data & 1) << _registerShiftOffset;
            _registerShiftOffset++;

            //5th write gets written to the internal registers
            if (_registerShiftOffset == 5)
            {
                _registerShiftOffset = 0;
                WriteInternalRegister(offset);
                _registerShift = 0;
            }
        }

        /// <summary>
        ///     Determines, based on the offset of the write, which internal register
        ///     will be written to:
        ///
        ///     Internal Registers:
        ///     $8000->$9FFF == Control Register
        ///     $A000->$BFFF == CHR0 Register
        ///     $C000->$DFFF == CHR1 Register
        ///     $E000->$FFFF == PRG Register
        /// </summary>
        /// <param name="offset"></param>
        private void WriteInternalRegister(int offset)
        {
            if (offset <= 0x9FFF)
            {
                _registerControl = _registerShift;

                _currentPrgMode = (_registerShift >> 2) & 0x03;
                _currentChrMode = (_registerShift >> 4) & 0x01;
                switch (_registerControl & 0x03)
                {
                    case 0:
                        NametableMirroring = enumNametableMirroring.SingleLower;
                        break;
                    case 1:
                        NametableMirroring = enumNametableMirroring.SingleUpper;
                        break;
                    case 2:
                        NametableMirroring = enumNametableMirroring.Vertical;
                        break;
                    case 3:
                        NametableMirroring = enumNametableMirroring.Horizontal;
                        break;
                }
            }
            else if (offset <= 0xBFFF)
            {
                _chrBank0 = _registerShift;
            }
            else if (offset <= 0xDFFF)
            {
                _chrBank1 = _registerShift;
            }
            else
            {
                _prgBank = _registerShift;
                _usePrgRam = _registerShift >> 4 == 0;
            }

            //Based off this write, update the offsets of the PRG and CHR Banks
            UpdateBankOffsets();
        }

        /// <summary>
        ///     Updates the offsets for CHR0, CHR1, and PRG based off updates to
        ///     the control registers, or any of the other registers.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateBankOffsets()
        {
            switch (_currentChrMode)
            {
                case 0:
                    //8K (4K+4K contiguous)
                    _chrBank0Offset = ((_chrBank0 & 0x1E) >> 1) * 0x1000;
                    _chrBank1Offset = _chrBank0Offset + 0x1000;
                    break;
                case 1:
                    //4K Switched + 4K Switched
                    _chrBank0Offset = _chrBank0 * 0x1000;
                    _chrBank1Offset = _chrBank1 * 0x1000;
                    break;
                default:
                    throw new ArgumentException("Invalid CHR Mode Specified");
            }

            switch (_currentPrgMode)
            {
                case 0:
                case 1: //32KB (16KB+16KB contiguous) Switched
                    _prgBank0Offset = ((_prgBank & 0xE) >> 1) * 0x4000;
                    _prgBank1Offset = _prgBank0Offset + 0x4000;
                    break;
                case 2: //16KB Fixed (First) + 16KB Switched
                    //Fixed first bank at $8000
                    _prgBank0Offset = 0;
                    //Switched 16KB bank at $C000
                    _prgBank1Offset = (_prgBank & 0xF) * 0x4000;
                    break;
                case 3: //16KB Switched + 16KB Fixed (Last)
                    //Switched 16 KB bank at $8000
                    _prgBank0Offset = (_prgBank & 0xF) * 0x4000;
                    //Fixed last bank at $C000
                    _prgBank1Offset = (_prgRomBanks - 1) * 0x4000;
                    break;
                default:
                    throw new ArgumentException("Invalid PRG Mode Specified");
            }
        }
    }
}
