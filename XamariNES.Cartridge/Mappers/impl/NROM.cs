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
        private readonly byte[] _chrRom = new byte[8192];

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
        public byte ReadByte(enumMemoryType memoryType, int offset)
        {
            switch (memoryType)
            {
                case enumMemoryType.CPU:
                    return _prgRom[CpuOffsetToPrgRomOffset(offset)];
                case enumMemoryType.PPU:
                    return !ReadInterceptors.TryGetValue(offset, out currentReadInterceptor) ? _chrRom[offset] : currentReadInterceptor(offset);
                default:
                    throw new ArgumentOutOfRangeException(nameof(memoryType), memoryType, null);
            }
        }

        /// <summary>
        ///     Writes one byte to the specified bank, at the specified offset
        /// </summary>
        /// <param name="memoryType"></param>
        /// <param name="offset"></param>
        /// <param name="data"></param>
        public void WriteByte(enumMemoryType memoryType, int offset, byte data)
        {
            switch(memoryType)
            {
                case enumMemoryType.CPU:
                        _prgRom[CpuOffsetToPrgRomOffset(offset)] = data;
                        return;
                case enumMemoryType.PPU:
                    if (!WriteInterceptors.TryGetValue(offset, out currentWriteInterceptor))
                    {
                        _chrRom[offset] = data;
                    }
                    else
                    {
                        currentWriteInterceptor(offset, data);
                    }
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(memoryType), memoryType, null);
            }
        }

        /// <summary>
        ///     Memory access from the CPU uses CPU mapped memory addressing.
        ///
        ///     In the NROM mapper, PGR ROM starts at 0x0000, CPU maps this to 0x8000
        /// 
        ///     Because of this, we subtract 0x8000 from the offset requested by the CPU
        ///     to get the relative offset for the actual PGR address in _pgrRom
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CpuOffsetToPrgRomOffset(int offset) => offset - 0x8000;
    }
}
