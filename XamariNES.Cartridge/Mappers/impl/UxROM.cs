using System;
using XamariNES.Cartridge.Mappers.Enums;

namespace XamariNES.Cartridge.Mappers.impl
{
    /// <summary>
    ///     NES Mapper 2 (UxROM)
    ///
    ///     More Info: https://wiki.nesdev.com/w/index.php/UxROM
    /// </summary>
    public class UxROM : MapperBase, IMapper
    {
        /// <summary>
        ///     PRG ROM
        ///
        ///     256K Capacity
        /// </summary>
        private readonly byte[] _prgRom;

        /// <summary>
        ///     CHR ROM
        ///
        ///     8K Capacity
        /// </summary>
        private readonly byte[] _chrRom;

        /// <summary>
        ///     Switchable Bank 0
        /// </summary>
        private int _prgBank0Offset = 0;

        /// <summary>
        ///     Bank 1 is always fixed to the last bank
        /// </summary>
        private readonly int _prgBank1Offset;

        public enumNametableMirroring NametableMirroring { get; set; }

        public UxROM(byte[] prgRom, int prgRomBanks, byte[] chrRom, enumNametableMirroring nametableMirroring)
        {
            _prgRom = prgRom;
            _chrRom = chrRom;
            NametableMirroring = nametableMirroring;
            _prgBank1Offset = (prgRomBanks - 1) * 0x4000;
        }

        /// <summary>
        ///     Reads one byte from the specified bank, at the specified offset
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public byte ReadByte(int offset)
        {
            // CHR ROM
            if (offset < 0x2000)
                return _chrRom[offset];

            //PPU Registers
            if (offset <= 0x3FFF)
                return ReadInterceptors.TryGetValue(offset, out currentReadInterceptor) ? currentReadInterceptor(offset) : (byte) 0x0;
            
            //Some UxROM boards from Nintendo have bus conflicts, we avoid these here
            if (offset >= 0x6000 && offset < 0x8000)
                return 0x0;

            //16 KB switchable PRG ROM bank
            if (offset <= 0xBFFF)
                return _prgRom[_prgBank0Offset + (offset - 0x8000)];

            //16 KB PRG ROM bank, fixed to the last bank
            if (offset <= 0xFFFF)
                return _prgRom[_prgBank1Offset + (offset - 0xC000)];

            throw new ArgumentOutOfRangeException(nameof(offset), offset, "Maximum value of offset is 0xFFFF");
        }

        /// <summary>
        ///     Writes one byte to the specified bank, at the specified offset
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="data"></param>
        public void WriteByte(int offset, byte data)
        {
            //CHR ROM+RAM Writes
            if (offset < 0x2000)
            {
                _chrRom[offset] = data;
                return;
            }

            //PPU Registers
            if (offset <= 0x3FFF || offset == 0x4014)
            {
                if (WriteInterceptors.TryGetValue(offset, out currentWriteInterceptor))
                    currentWriteInterceptor(offset, data);

                return;
            }

            //Some UxROM boards from Nintendo have bus conflicts, we avoid these here
            if (offset >= 0x6000 && offset <= 0x7FFF)
                return;

            //Bank Select
            if (offset >= 0xC000 && offset <= 0xFFFF)
            {
                _prgBank0Offset = (data & 0x0F) * 0x4000;
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(offset), offset, "Maximum value of offset is 0xFFFF");
        }
    }
}
