using System;
using System.IO;
using NLog;
using XamariNES.Cartridge.Flags;
using XamariNES.Cartridge.Mappers;
using XamariNES.Cartridge.Mappers.Enums;
using XamariNES.Cartridge.Mappers.impl;
using XamariNES.Common.Extensions;
using XamariNES.Common.Logging;

namespace XamariNES.Cartridge
{
    /// <summary>
    ///     Class that Represents a NES cartridge by loading an iNES format ROM
    ///
    ///     This class/project will contain the PGR/CHR memory as well as mapper functionality.
    ///     Access to this class is abstracted through the ICartridge interface, which is referenced
    ///     directly by both the CPU and PPU (as in the actual NES)
    ///
    ///     ROM Format: https://wiki.nesdev.com/w/index.php/INES
    /// </summary>
    public class NESCartridge : ICartridge
    {
        protected static readonly Logger _logger = LogManager.GetCurrentClassLogger(typeof(CustomLogger));

        private byte Flags6;
        private byte Flags7;
        private byte[] _prgRom;
        private byte _prgRomBanks;
        private byte[] _chrRom;
        private byte _chrRomBanks;
        private byte[] _prgRam;
        private bool UsesCHRRAM;
        private enumNametableMirroring _nametableMirroring;

        public IMapper MemoryMapper { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="path">Path to the desired iNES ROM to load</param>
        public NESCartridge(string path)
        {
            if (!File.Exists(path))
                throw new Exception($"Unable to Load ROM: {path}");

            if (new FileInfo(path).Length > int.MaxValue)
                throw new Exception($"Unsupported ROM - File size too large: {path}");

            LoadROM(File.ReadAllBytes(path));
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="ROM">Byte Array containing the desired iNES ROM to load</param>
        public NESCartridge(byte[] ROM) => LoadROM(ROM);

        /// <summary>
        ///     Loads the specified iNES ROM
        /// </summary>
        /// <param name="ROM">Byte Array containing the desired iNES ROM to load</param>
        /// <returns>TRUE if load was successful</returns>
        public bool LoadROM(byte[] ROM)
        {
            //Header is 16 bytes

            //PRG Rom starts right after, unless there's a 512 byte trainer (indicated by flags)
            var prgROMOffset = 16;

            //_header == "NES<EOF>"
            if (BitConverter.ToInt32(ROM, 0) != 0x1A53454E)
                throw new Exception("Invalid ROM Header");

            //Setup Memory
            _prgRomBanks = ROM[4];
            var prgROMSize = _prgRomBanks * 16384;
            _prgRom = new byte[prgROMSize];
            _logger.Info($"PRG ROM Size: {prgROMSize}");

            _chrRomBanks = ROM[5];
            var chrROMSize = _chrRomBanks * 8192; //0 denotes default 8k
            if (ROM[5] == 0)
            {
                _chrRom = new byte[8192];
                UsesCHRRAM = true;
            }
            else
            {
                _chrRom = new byte[chrROMSize];
            }
            _logger.Info($"CHR ROM Size: {chrROMSize}");

            //Set Flags6
            Flags6 = ROM[6];

            //Move PGR ROM Start if Trainer Present
            if (Flags6.IsFlagSet(Byte6Flags.TrainerPresent))
                prgROMOffset += 512;

            //Set Initial Mirroring Mode
            _nametableMirroring = Flags6.IsFlagSet(Byte6Flags.VerticalMirroring) ? enumNametableMirroring.Vertical : enumNametableMirroring.Horizontal;

            //Set Flags7
            Flags7 = ROM[7];

            var prgRAMSize = ROM[8] == 0 ? 8192 : ROM[8] * 8192; //0 denoted default 8k
            _prgRam = new byte[prgRAMSize];

            //Load PRG ROM
            Array.Copy(ROM, prgROMOffset, _prgRom, 0, prgROMSize);

            //Load CHR ROM
            Array.Copy(ROM, prgROMOffset+prgROMSize, _chrRom, 0, chrROMSize);

            //Load Proper Mapper
            var mapperNumber = Flags7 & 0xF0 | (Flags6 >> 4 & 0xF);
            switch (mapperNumber)
            {
                case 0:
                    MemoryMapper = new NROM(_prgRom, _chrRom, _nametableMirroring);
                    break;
                case 1:
                    MemoryMapper = new MMC1(_prgRomBanks, _chrRom, _prgRom, UsesCHRRAM, false, _nametableMirroring);
                    break;
                case 2:
                    MemoryMapper = new UxROM(_prgRom, _prgRomBanks, _chrRom, _nametableMirroring);
                    break;
                case 3:
                    MemoryMapper = new CNROM(_prgRom, _prgRomBanks, _chrRom, _nametableMirroring);
                    break;
                default:
                    throw new Exception($"Unsupported Mapper: {mapperNumber}");
            }

            return true;
        }
    }
}
