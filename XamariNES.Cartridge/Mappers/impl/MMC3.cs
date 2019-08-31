using System;
using System.Runtime.CompilerServices;
using XamariNES.Cartridge.Mappers.Enums;

namespace XamariNES.Cartridge.Mappers.impl
{
    /// <summary>
    ///     iNES Mapper 4 (MMC3)
    ///
    ///     More Info: https://wiki.nesdev.com/w/index.php/MMC3
    /// </summary>
    public class MMC3 : MapperBase, IMapper, IA12Connection
    {
        private readonly byte[] _chrRom;
        private readonly byte[] _prgRom;
        private readonly byte[] _prgRam = new byte[0x2000];

        private int _currentChrMode;
        private int _currentPrgMode;

        private readonly int[] _chrBankOffsets = new int[8];
        private readonly int[] _prgBankOffsets = new int[4];

        //Registers
        private int _registerBankSelect;
        private int _registerPrgRamProtect;
        private int _registerPrgRamEnabled;
        private int _registerIRQLatch;
        private int _registerIRQCounter;
        private bool _registerIsIRQEnabled;
        private readonly int[] _registerBanks = new int[8];

        /// <summary>
        ///     Track if it's an even/odd offset
        /// </summary>
        private bool _isEven;

        private readonly int _prgRomBanks;

        public enumNametableMirroring NametableMirroring { get; set; }

        public bool isIRQ { get; set; }

        public MMC3(byte[] prgRom, int prgRomBanks, byte[] chrRom, enumNametableMirroring nametableMirroring)
        {
            _prgRom = prgRom;
            _chrRom = chrRom;
            _prgRomBanks = prgRomBanks;
            NametableMirroring = nametableMirroring;

            //Set Initial Values for PRG Bank Offsets
            _prgBankOffsets[0] = 0;
            _prgBankOffsets[1] = 0x2000;
            _prgBankOffsets[2] = (_prgRomBanks * 2 - 2) * 0x2000;
            _prgBankOffsets[3] = _prgBankOffsets[2] + 0x2000;
        }

        public byte ReadByte(int offset)
        {
            //CHR ROM Banks
            if (offset <= 0x1FFF)
            {
                var selectedBank = offset / 0x400;
                var selectedBankOffset = _chrBankOffsets[selectedBank] + offset % 0x400;
                return _chrRom[selectedBankOffset];
            }

            //PPU Registers
            if (offset <= 0x3FFF)
                return ReadInterceptors.TryGetValue(offset, out currentReadInterceptor) ? currentReadInterceptor(offset) : (byte)0x0;

            //PRG RAM
            if (offset >= 0x6000 && offset <= 0x7FFF)
                return _registerPrgRamEnabled == 1 ? _prgRam[offset - 0x6000] : (byte)0;

            //PRG ROM Banks
            if (offset <= 0xFFFF) // 8 KB PRG ROM banks
            {
                var selectedBank = (offset - 0x8000) / 0x2000;
                var selectedBankOffset = _prgBankOffsets[selectedBank] + (offset % 0x2000);
                return _prgRom[selectedBankOffset];
            }

            throw new ArgumentOutOfRangeException(nameof(offset), offset, "Maximum value of offset is 0xFFFF");
        }

        public void WriteByte(int offset, byte data)
        {
            //CHR ROM
            if (offset <= 0x1FFF)
            {
                var offsetBank = offset / 0x400;
                var chrOffset = _chrBankOffsets[offsetBank] + (offset % 0x400);
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

            //PRG RAM
            if (offset >= 0x6000 && offset <= 0x7999)
            {
                if (_registerPrgRamProtect == 0)
                    _prgRam[offset - 0x6000] = data;

                return;
            }

            //For internal Register writes, we evaluate if it's an even/odd address
            _isEven = offset % 2 == 0;

            //EVEN: Bank Select
            //ODD: Bank Data
            if (offset <= 0x9FFF)
            {
                if (_isEven)
                {
                    _registerBankSelect = data & 0x07;
                    _currentPrgMode = (data >> 6) & 0x01;
                    _currentChrMode = (data >> 7) & 0x01;
                }
                else
                {
                    _registerBanks[_registerBankSelect] = data;
                }
                UpdateBankOffsets();
                return;
            }

            //EVEN: Mirroring
            //ODD: PRG RAM Protect
            if (offset <= 0xBFFF)
            {
                if (_isEven)
                {
                    //Set Mirroring Mode
                    switch (data & 0x01)
                    {
                        case 0:
                            NametableMirroring = enumNametableMirroring.Vertical;
                            break;
                        case 1:
                            NametableMirroring = enumNametableMirroring.Horizontal;
                            break;
                    }
                }
                else
                {
                    _registerPrgRamEnabled = (data >> 7) & 0x01;
                    _registerPrgRamProtect = (data >> 6) & 0x01;
                }
                UpdateBankOffsets();
                return;
            }

            //EVEN: IRQ Latch
            //ODD: IRQ Reload
            if (offset <= 0xDFFF)
            {
                if (_isEven)
                {
                    _registerIRQLatch = data;
                }
                else
                {
                    _registerIRQCounter = 0;
                }
                UpdateBankOffsets();
                return;
            }

            //EVEN: IRQ Disable
            //ODD: IRQ Enable
            if (offset <= 0xFFFF)
            {
                if (_isEven)
                {
                    _registerIsIRQEnabled = false;
                }
                else
                {
                    _registerIsIRQEnabled = true;
                }
                UpdateBankOffsets();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateBankOffsets()
        {
            switch (_currentChrMode)
            {
                case 0:
                    _chrBankOffsets[0] = (_registerBanks[0] & 0xFE) * 0x400;
                    _chrBankOffsets[1] = (_registerBanks[0] | 0x01) * 0x400;
                    _chrBankOffsets[2] = (_registerBanks[1] & 0xFE) * 0x400;
                    _chrBankOffsets[3] = (_registerBanks[1] | 0x01) * 0x400;
                    _chrBankOffsets[4] = _registerBanks[2] * 0x400;
                    _chrBankOffsets[5] = _registerBanks[3] * 0x400;
                    _chrBankOffsets[6] = _registerBanks[4] * 0x400;
                    _chrBankOffsets[7] = _registerBanks[5] * 0x400;
                    break;
                case 1:
                    _chrBankOffsets[0] = _registerBanks[2] * 0x400;
                    _chrBankOffsets[1] = _registerBanks[3] * 0x400;
                    _chrBankOffsets[2] = _registerBanks[4] * 0x400;
                    _chrBankOffsets[3] = _registerBanks[5] * 0x400;
                    _chrBankOffsets[4] = (_registerBanks[0] & 0xFE) * 0x400;
                    _chrBankOffsets[5] = (_registerBanks[0] | 0x01) * 0x400;
                    _chrBankOffsets[6] = (_registerBanks[1] & 0xFE) * 0x400;
                    _chrBankOffsets[7] = (_registerBanks[1] | 0x01) * 0x400;
                    break;
            }

            var secondLastBankOffset = (_prgRomBanks * 2 - 2) * 0x2000;
            var lastBankOffset = secondLastBankOffset + 0x2000;
            switch (_currentPrgMode)
            {
                case 0:
                    _prgBankOffsets[0] = _registerBanks[6] * 0x2000;
                    _prgBankOffsets[1] = _registerBanks[7] * 0x2000;
                    _prgBankOffsets[2] = secondLastBankOffset;
                    _prgBankOffsets[3] = lastBankOffset;
                    break;
                case 1:
                    _prgBankOffsets[0] = secondLastBankOffset;
                    _prgBankOffsets[1] = _registerBanks[7] * 0x2000;
                    _prgBankOffsets[2] = _registerBanks[6] * 0x2000;
                    _prgBankOffsets[3] = lastBankOffset;
                    break;
            }
        }


        /// <summary>
        ///     IA12Connection Implementation
        ///
        ///     The MMC3 mapper is connected to the A12 pad on the PPU and increments the
        ///     IRQ counter on the rising edge.
        ///
        ///     More Information: https://wiki.nesdev.com/w/index.php/MMC3#IRQ_Specifics
        /// </summary>
        public void PulseA12()
        {
            if (_registerIRQCounter == 0)
            {
                _registerIRQCounter = _registerIRQLatch;
            }
            else
            {
                _registerIRQCounter--;
                if (_registerIRQCounter == 0 && _registerIsIRQEnabled)
                    isIRQ = true;
            }
        }
    }
}
