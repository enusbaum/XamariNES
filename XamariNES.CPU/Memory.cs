using System;
using XamariNES.Cartridge.Mappers;
using XamariNES.Cartridge.Mappers.Enums;
using XamariNES.Controller;

namespace XamariNES.CPU
{
    /// <summary>
    ///     NES CPU Memory
    ///
    ///     We use this to handle the CPU specific memory map
    ///     https://wiki.nesdev.com/w/index.php/CPU_memory_map
    /// </summary>
    public class Memory
    {
        private readonly IMapper _memoryMapper;
        private readonly IController _controller;
        private readonly byte[] _internalRam;

        public Memory(IMapper memoryMapper, IController controller)
        {
            _memoryMapper = memoryMapper;
            _controller = controller;
            _internalRam = new byte[2048];
        }

        /// <summary>
        ///     Reads a single byte from the specified offset in the memory address space
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public byte ReadByte(int offset)
        {
            //2KB internal RAM (+ mirrors)
            if (offset < 0x2000) 
                return _internalRam[offset % 0x800];

            //NES PPU Registers (Repeats every 8 bytes)
            if (offset <= 0x3FFF)
                return _memoryMapper.ReadByte(0x2000 + offset % 8);

            //NES APU & I/O Registers
            if (offset <= 0x4017)
            {
                switch (offset)
                {
                    case 0x4016:
                        return _controller.ReadController();
                    default:
                        return 0x0;
                }
            }

            //APU and I/O functionality that is normally disabled
            if (offset <= 0x401F)
                return 0x0;

            //Cartridge space: PRG ROM, PRG RAM, and mapper registers 
            if (offset >= 0x4020) 
                return _memoryMapper.ReadByte(offset);

            throw new Exception($"Invalid CPU read at address {offset:X4}");
        }

        /// <summary>
        ///     Write the specified byte to the offset in the memory address space
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="data"></param>
        public void WriteByte(int offset, byte data)
        {
            //2KB internal RAM (+ mirrors)
            if (offset < 0x2000)
            {
                _internalRam[offset % 0x800] = data;
                return;
            }

            //NES PPU Registers (repeats every 8 bytes and OAM register)
            if (offset <= 0x3FFF)
            {
                _memoryMapper.WriteByte(0x2000 + (offset % 8), data);
                return;
            }

            //OAM DMA
            if (offset == 0x4014)
            {
                _memoryMapper.WriteByte(offset, data);
                return;
            }

            //NES APU & I/O Registers
            if (offset <= 0x4017) 
            {
                switch (offset)
                {
                    case 0x4016:
                        _controller.SignalController(data);
                        break;
                }
                return;
            }

            //APU and I/O functionality that is normally disabled
            if (offset <= 0x401F)
                return;

            //Cartridge space: PRG ROM, PRG RAM, and mapper registers
            if (offset >= 0x4020)
            {
                _memoryMapper.WriteByte(offset, data);
                return;
            }
            
            throw new Exception($"Invalid CPU write to address {offset:X4}");
        }
    }
}
