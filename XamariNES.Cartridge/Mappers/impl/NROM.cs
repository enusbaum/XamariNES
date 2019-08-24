using System;
using System.Runtime.CompilerServices;
using XamariNES.Cartridge.Mappers.Enums;

namespace XamariNES.Cartridge.Mappers.impl
{
    /// <summary>
    ///     NES Mapper 0 (NROM)
    ///     
    ///     Documentation: https://wiki.nesdev.com/w/index.php/NROM
    /// </summary>
    public class NROM : MapperBase, IMapper
    {
        //ROM Internal Memory
        private readonly byte[] _prgRom = new byte[0x8000];
        private readonly byte[] _chrRom = new byte[0x2000];

        public enumNametableMirroring NametableMirroring { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="prgRom"></param>
        /// <param name="chrRom"></param>
        /// <param name="mirroring"></param>
        public NROM(byte[] prgRom, byte[] chrRom, enumNametableMirroring mirroring = enumNametableMirroring.Horizontal)
        {
            NametableMirroring = mirroring;

            //Assign first 16k of ROM
            Array.Copy(prgRom, 0, _prgRom, 0, prgRom.Length);

            //If it's less than 16k, just mirror it to the second bank
            if (prgRom.Length <= 0x4000)
                Array.Copy(prgRom, 0, _prgRom, 0x4000, prgRom.Length);

            //Setup PPU ROM
            if (chrRom != null)
                Array.Copy(chrRom, 0, _chrRom, 0, chrRom.Length);
        }

        /// <summary>
        ///     Reads one byte from the specified bank, at the specified offset
        /// </summary>
        /// <param name="memoryType"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public byte ReadByte(int offset)
        {
            //CHR ROM
            if (offset < 0x2000)
                return _chrRom[offset];

            //PPU Interceptors (Read)
            if (offset <= 0x3FFF)
                return ReadInterceptors.TryGetValue(offset, out currentReadInterceptor)
                    ? currentReadInterceptor(offset)
                    : (byte) 0x0;

            //PRG ROM
            if (offset <= 0xFFFF)
                return _prgRom[offset - 0x8000];

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
            //CHR ROM
            if (offset < 0x2000)
            {
                _chrRom[offset] = data;
                return;
            }

            //PPU Interceptors
            if (offset <= 0x3FFF || offset == 0x4014)
            {
                if (!WriteInterceptors.TryGetValue(offset, out currentWriteInterceptor)) return;
                currentWriteInterceptor(offset, data);
                return;
            }

            if (offset >= 0x8000 && offset <= 0xFFFF)
            { 
                _prgRom[offset - 0x8000] = data;
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(offset), offset, "Maximum value of offset is 0xFFFF");
        }
    }
}
