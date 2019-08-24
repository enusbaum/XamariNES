﻿using System;
using NLog.Filters;
using XamariNES.Cartridge.Mappers.Enums;

namespace XamariNES.Cartridge.Mappers.impl
{
    /// <summary>
    ///     NES Mapper 3 (CNROM)
    ///
    ///     More Info: https://wiki.nesdev.com/w/index.php/CNROM
    /// </summary>
    public class CNROM : MapperBase, IMapper
    {
        /// <summary>
        ///     PRG ROM
        ///
        ///     32KB Capacity
        /// </summary>
        private readonly byte[] _prgRom;

        /// <summary>
        ///     CHR ROM
        ///
        ///     32KB Capacity, 8K Switchable Window
        /// </summary>
        private readonly byte[] _chrRom;

        /// <summary>
        ///     Offset of our Switched Bank in CHR ROM
        /// </summary>
        private int _chrRomOffset;

        /// <summary>
        ///     PRG ROM Mirroring Enabled
        /// </summary>
        private readonly bool _prgRomMirroring;

        public enumNametableMirroring NametableMirroring { get; set; }

        public CNROM(byte[] prgRom, int prgRomBanks, byte[] chrRom, enumNametableMirroring nametableMirroring)
        {
            _prgRom = prgRom;
            _chrRom = chrRom;
            NametableMirroring = nametableMirroring;

            //Only one 16KB Bank means we need to mirror
            _prgRomMirroring = prgRomBanks == 1;
        }

        /// <summary>
        ///     Reads one byte from the specified bank, at the specified offset
        /// </summary>
        /// <param name="memoryType"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public byte ReadByte(enumMemoryType memoryType, int offset)
        {
            // CHR ROM
            if (offset < 0x2000)
                return _chrRom[_chrRomOffset + offset];

            //PPU Registers
            if (offset <= 0x3FFF)
                return ReadInterceptors.TryGetValue(offset, out currentReadInterceptor) ? currentReadInterceptor(offset) : (byte)0x0;

            //Some UxROM boards from Nintendo have bus conflicts, we avoid these here
            if (offset >= 0x6000 && offset < 0x8000)
                return 0x0;

            //Fixed PRG ROM
            if (offset <= 0xFFFF)
            {
                //16KB PRG, Need to mirror
                if (_prgRomMirroring && offset >= 0xC000)
                    return _prgRom[offset - 0xC000];

                return _prgRom[offset - 0x8000];
            }

            throw new ArgumentOutOfRangeException(nameof(offset), offset, "Maximum value of offset is 0xFFFF");
        }

        /// <summary>
        ///     Writes one byte to the specified bank, at the specified offset
        ///
        ///     CHR Bank Switching is handled at $8000->$FFFF
        /// </summary>
        /// <param name="memoryType"></param>
        /// <param name="offset"></param>
        /// <param name="data"></param>
        public void WriteByte(enumMemoryType memoryType, int offset, byte data)
        {
            //CHR ROM+RAM Writes
            if (offset < 0x2000)
            {
                _chrRom[_chrRomOffset + offset] = data;
                return;
            }

            //PPU Registers
            if (offset <= 0x3FFF || offset == 0x4014)
            {
                if (WriteInterceptors.TryGetValue(offset, out currentWriteInterceptor))
                    currentWriteInterceptor(offset, data);

                return;
            }

            //CHR Bank Select
            if (offset >= 0x8000 && offset <= 0xFFFF)
            {
                _chrRomOffset = (data & 0x03) * 0x2000;
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(offset), offset, "Maximum value of offset is 0xFFFF");
        }

        
    }
}