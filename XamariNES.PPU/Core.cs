using System;
using System.Runtime.CompilerServices;
using XamariNES.Cartridge.Mappers;
using XamariNES.Common.Extensions;
using XamariNES.PPU.Flags;

namespace XamariNES.PPU
{
    /// <summary>
    ///     NES PPU Core
    ///
    ///     Let me come out immediately and say the NES PPU is incredibly convoluted
    ///     due to the limitations of the hardware at the time. Hats off to the
    ///     original designers for actually making this work.
    ///
    ///     That being said, because of the original design the code itself it a
    ///     a little convoluted as a result. I've done my best to make it readable
    ///     and reuse simple routines from other emulators to make it as easy to
    ///     follow as I could.
    ///
    ///     For more information on the NES Picture Processing Unit:
    ///     https://wiki.nesdev.com/w/index.php/PPU
    /// </summary>
    public class Core
    {
        public delegate byte[] DMAWriteDelegate(byte[] oam, int oamAddress, int offset);

        //Internal Registers
        private byte _X; // Fine X scroll (3 bits)
        private byte _writeOrderToggle;
        private byte _frameOrderToggle;

        //PPU Registers
        //More Info: https://wiki.nesdev.com/w/index.php/PPU_registers
        private byte _registerPPUCTRL;
        private byte _registerPPUMASK;
        private byte _registerPPUSTATUS;
        private byte _registerOAMADDR;
        private int _registerPPUADDR;
        private int _registerPPUSCROLL;
        private byte _registerPPUDATABuffer;

        /// <summary>
        ///     Represents the four PPU Shift Registers as one value
        ///
        ///     2*16-bit shift registers+2*8-bit shift registers
        ///
        ///     More Info: https://wiki.nesdev.com/w/index.php/PPU_rendering#Preface
        /// </summary>
        private ulong _tileShiftRegister;

        //Latches
        //More Info: https://wiki.nesdev.com/w/index.php/PPU_rendering#Cycles_1-256
        private byte _nameTableByte;
        private byte _attributeTableByte;
        private byte _tileDataLow;
        private byte _tileDataHigh;

        // OAM / Sprite rendering
        private byte[] _oamData;
        private readonly byte[] _sprites;
        private readonly int[] _spriteIndices;
        private int _spriteIndex;
        private int _countedSprites; //Sprites Counted During Evaluation

        //Constants
        //https://wiki.nesdev.com/w/index.php/PPU_rendering
        private const int MaxCycles = 340;
        private const int MaxScanline = 261;
        private const int MaxWidth = 256;
        private const int MaxHeight = 240;

        /// <summary>
        ///     Signals that there is a frame in the buffer ready for rendering
        /// </summary>
        public bool FrameReady;

        /// <summary>
        ///     Frame buffer holding 240x256 rendered frame
        /// </summary>
        public readonly byte[] FrameBuffer = new byte[MaxHeight * MaxWidth];

        //Internal Counters
        private int _currentCycle;
        private int _currentScanline;
        private int _scanLineState;
        private int _cycleState;
        public long Cycles;

        /// <summary>
        ///     Signals if an NMI has occured
        /// </summary>
        public bool NMI;

        /// <summary>
        ///     PPU Memory Space
        /// </summary>
        public Memory PPUMemory;

        /// <summary>
        ///     PPU Constructor
        /// </summary>
        /// <param name="memoryMapper">Cartridge Memory Mapper - Used to access CHR memory</param>
        /// <param name="dmaWriteDelegate">DMA Write Delegate - Invoked to copy data from CPU memory to the OAM buffer</param>
        public Core(IMapper memoryMapper, DMAWriteDelegate dmaWriteDelegate)
        {
            //Declare the things
            _oamData = new byte[256];
            _sprites = new byte[32];
            _spriteIndices = new int[8];

            PPUMemory = new Memory(memoryMapper);

            //Set Everything to a startup state
            Reset();

            //These memory addresses are mapped on the CPU addressing range, so
            //the CPU memory controller will handle the mirroring index

            //PPUCTRL ($2000, WRITE)
            memoryMapper.RegisterWriteInterceptor(delegate(int offset, byte value)
            {
                UpdatePPUSTATUSRegister(value);

                //Update Register
                _registerPPUCTRL = value;

                //Cache the new value here
                _registerPPUSCROLL = (_registerPPUSCROLL & 0xF3FF) | ((value & 0x03) << 10);
            }, 0x2000);

            //PPUMASK ($2001, WRITE)
            memoryMapper.RegisterWriteInterceptor(delegate(int offset, byte value)
            {
                UpdatePPUSTATUSRegister(value);

                _registerPPUMASK = value;
            }, 0x2001);

            //PPUSTATUS ($2002, READ)
            memoryMapper.RegisterReadInterceptor(delegate
            {
                var output = _registerPPUSTATUS;
                _registerPPUSTATUS = _registerPPUSTATUS.RemoveFlag(PPUStatusFlags.VerticalBlankStarted);
                _writeOrderToggle = 0;
                return output;

            }, 0x2002);

            //OAMADDR ($2003, WRITE)
            memoryMapper.RegisterWriteInterceptor(delegate(int offset, byte value)
            {
                UpdatePPUSTATUSRegister(value);

                _registerOAMADDR = value;
            }, 0x2003);

            //OAMDATA ($2004, READ)
            memoryMapper.RegisterReadInterceptor(offset => _oamData[_registerOAMADDR], 0x2004);

            //OAMDATA ($2004, WRITE)
            memoryMapper.RegisterWriteInterceptor(delegate(int offset, byte value)
            {
                UpdatePPUSTATUSRegister(value);

                //When data is written to the Data register, we buffer it our our array here
                _oamData[_registerOAMADDR] = value;
                _registerOAMADDR++;

            }, 0x2004);

            //PPUSCROLL ($2005, WRITE)
            memoryMapper.RegisterWriteInterceptor(delegate(int offset, byte value)
            {
                UpdatePPUSTATUSRegister(value);

                //Update Local Values
                if (_writeOrderToggle == 0)
                {
                    _registerPPUSCROLL = (_registerPPUSCROLL & 0xFFE0) | (value >> 3);
                    _X = (byte) (value & 0x07);
                    _writeOrderToggle = 1;
                }
                else
                {
                    _registerPPUSCROLL &= 0xC1F;
                    _registerPPUSCROLL |= (value & 0x07) << 12; // CBA
                    _registerPPUSCROLL |= (value & 0xF8) << 2; // HG FED
                    _writeOrderToggle = 0;
                }
            }, 0x2005);

            //PPUADDR ($2006, WRITE)
            memoryMapper.RegisterWriteInterceptor(delegate(int offset, byte value)
            {
                UpdatePPUSTATUSRegister(value);

                _registerPPUADDR = value;
                if (_writeOrderToggle == 0)
                {
                    _registerPPUSCROLL = (_registerPPUSCROLL & 0x00FF) | (value << 8);
                    _writeOrderToggle = 1;
                }
                else
                {
                    _registerPPUSCROLL = (_registerPPUSCROLL & 0xFF00) | value;
                    _registerPPUADDR = _registerPPUSCROLL;
                    _writeOrderToggle = 0;
                }
            }, 0x2006);

            //PPUDATA ($2007, READ)
            memoryMapper.RegisterReadInterceptor(delegate
            {
                var data = PPUMemory.ReadByte(_registerPPUADDR);

                // Buffered read emulation
                // https://wiki.nesdev.com/w/index.php/PPU_registers#The_PPUDATA_read_buffer_.28post-fetch.29
                if (_registerPPUADDR < 0x3F00)
                {
                    var bufferedData = _registerPPUDATABuffer;
                    _registerPPUDATABuffer = data;
                    data = bufferedData;
                }
                else
                {
                    _registerPPUDATABuffer = PPUMemory.ReadByte(_registerPPUADDR - 0x1000);
                }

                //Increment PPU VRAM Address depending on VRAMAddressIncrement Flag
                _registerPPUADDR +=
                    _registerPPUCTRL.IsFlagSet(PPUCtrlFlags.VRAMAddressIncrement)
                        ? 32
                        : 1;

                return data;

            }, 0x2007);

            //PPUDATA ($2007, WRITE)
            memoryMapper.RegisterWriteInterceptor(delegate(int offset, byte value)
            {
                UpdatePPUSTATUSRegister(value);

                PPUMemory.WriteByte(_registerPPUADDR, value);

                //Increment PPU VRAM Address depending on VRAMAddressIncrement Flag
                _registerPPUADDR +=
                    _registerPPUCTRL.IsFlagSet(PPUCtrlFlags.VRAMAddressIncrement)
                        ? 32
                        : 1;

            }, 0x2007);

            //OAMDMA ($4014, WRITE)
            memoryMapper.RegisterWriteInterceptor(
                delegate(int offset, byte value)
                {
                    UpdatePPUSTATUSRegister(value);
                    _oamData = dmaWriteDelegate(_oamData, _registerOAMADDR, value << 8);
                }, 0x4014);
        }

        /// <summary>
        ///     Resets the PPU and internal values to a startup state
        /// </summary>
        public void Reset()
        {
            //Clear the Buffers
            Array.Clear(FrameBuffer, 0, FrameBuffer.Length);
            Array.Clear(_oamData, 0, _oamData.Length);
            Array.Clear(_sprites, 0, _sprites.Length);
            Array.Clear(_spriteIndices, 0, _spriteIndices.Length);

            //Clear PPU Memory
            PPUMemory.Reset();

            //Reset the Registers
            _registerPPUCTRL = 0;
            _registerPPUMASK = 0;
            _registerPPUSTATUS = 0;
            _registerOAMADDR = 0;
            _registerPPUADDR = 0;
            _registerPPUSCROLL = 0;
            _registerPPUDATABuffer = 0;

            //Set Startup Status
            _registerPPUSTATUS |= PPUStatusFlags.VerticalBlankStarted;
            _currentCycle = 340;
            _currentScanline = 240;
        }

        /// <summary>
        ///     Ticks the PPU for one cycle
        /// </summary>
        public void Tick()
        {
            UpdateInternalCounters();

            //---------------------------------
            // Set our Current Scan Line Status
            // Only Lines 0 to 239 are visible
            //---------------------------------
            if (_currentScanline < 240)
            {
                if (_currentScanline == -1)
                {
                    _scanLineState |= ScanLineStateFlags.PreRender;
                }
                else
                {
                    _scanLineState = ScanLineStateFlags.Visible;
                }
            }
            else if (_currentScanline > 240)
            {
                _scanLineState = ScanLineStateFlags.VBlank;
                if (_currentScanline == 261)
                    _scanLineState |= ScanLineStateFlags.PreRender;

            }
            else if (_currentScanline == 240)
                _scanLineState = ScanLineStateFlags.PostRender;

            //---------------------------------
            // Set our current Cycle State
            //---------------------------------
            _cycleState = CycleStateFlags.Default;
            if (_currentCycle > 0 && _currentCycle <= 256)
            {
                _cycleState |= CycleStateFlags.Visible;

                //Reset NMIOccurred on Prerender of Scanline 1
                if (_currentCycle == 1 && _scanLineState.IsFlagSet(ScanLineStateFlags.PreRender))
                {
                    _registerPPUSTATUS = _registerPPUSTATUS.RemoveFlag(PPUStatusFlags.VerticalBlankStarted);
                    _registerPPUSTATUS = _registerPPUSTATUS.RemoveFlag(PPUStatusFlags.SpriteOverflow);
                    _registerPPUSTATUS = _registerPPUSTATUS.RemoveFlag(PPUStatusFlags.SpriteZeroHit);
                }
            }
            else if (_currentCycle >= 321 && _currentCycle <= 336)
                _cycleState |= CycleStateFlags.Prefetch;

            if (_cycleState.IsFlagSet(CycleStateFlags.Visible) || _cycleState.IsFlagSet(CycleStateFlags.Prefetch))
                _cycleState |= CycleStateFlags.Fetch;

            //------------------------------------
            // If we're not rendering anything this cycle, we're done for now
            //------------------------------------
            if (!_registerPPUMASK.IsFlagSet(PPUMaskFlags.ShowSprites) &&
                !_registerPPUMASK.IsFlagSet(PPUMaskFlags.ShowBackground)) return;

            //We evaluate Sprites on cycle # 257
            if (_currentCycle == 257)
            {
                if (_scanLineState.IsFlagSet(ScanLineStateFlags.Visible))
                    EvalSprites();
                else
                    _countedSprites = 0;
            }

            //Time to render a Pixel?
            if (_scanLineState.IsFlagSet(ScanLineStateFlags.Visible) && _cycleState.IsFlagSet(CycleStateFlags.Visible))
                RenderPixel();

            // Read rendering data into internal latches and update _tileShiftReg
            // with those latches every 8 cycles
            // https://wiki.nesdev.com/w/images/d/d1/Ntsc_timing.png
            // https://wiki.nesdev.com/w/index.php/PPU_rendering#Visible_scanlines_.280-239.29
            if (_cycleState.IsFlagSet(CycleStateFlags.Fetch) &&
                (_scanLineState.IsFlagSet(ScanLineStateFlags.Visible) ||
                 _scanLineState.IsFlagSet(ScanLineStateFlags.PreRender)))
            {
                _tileShiftRegister >>= 4;
                switch (_currentCycle % 8)
                {
                    //Reload shift registers
                    case 0:
                        StoreTileData();
                        IncrementX();
                        if (_currentCycle == 256) IncrementY();
                        break;
                    //Nametable byte
                    case 1:
                        _nameTableByte = PPUMemory.ReadByte(0x2000 | (_registerPPUADDR & 0x0FFF));
                        break;
                    //Attribute table byte
                    case 3:
                        _attributeTableByte =
                            PPUMemory.ReadByte(0x23C0 | (_registerPPUADDR & 0x0C00) |
                                               ((_registerPPUADDR >> 4) & 0x38) | ((_registerPPUADDR >> 2) & 0x07));
                        break;
                    //Pattern table tile low
                    case 5:
                        var patternTableTileLowBase =
                            _registerPPUCTRL.IsFlagSet(PPUCtrlFlags.BackgroundPatternTableAddress)
                                ? 0x1000
                                : 0x0000;
                        var patternTableTileLowAddress = patternTableTileLowBase + _nameTableByte * 16 + FineY();
                        _tileDataLow = PPUMemory.ReadByte(patternTableTileLowAddress);
                        break;
                    //Pattern table tile high(+8 bytes from pattern table tile low)
                    case 7:
                        var patternTableTileHighBase =
                            _registerPPUCTRL.IsFlagSet(PPUCtrlFlags.BackgroundPatternTableAddress)
                                ? 0x1000
                                : 0x0000;
                        var patternTableTileHighAddress = patternTableTileHighBase + _nameTableByte * 16 + FineY() + 8;
                        _tileDataHigh = PPUMemory.ReadByte(patternTableTileHighAddress);
                        break;
                }
            }

            // OAMADDR is set to 0 during each of ticks 257-320 (the sprite tile loading interval) of the pre-render and visible scanlines 
            if (_currentCycle > 257 && _currentCycle <= 320 &&
                (_scanLineState.IsFlagSet(ScanLineStateFlags.PreRender) ||
                 _scanLineState.IsFlagSet(ScanLineStateFlags.Visible))) _registerOAMADDR = 0;

            // Copy horizontal position data from t to v on _cycle 257 of each scanline if rendering enabled
            if (_currentCycle == 257 && (_scanLineState.IsFlagSet(ScanLineStateFlags.Visible) ||
                                         _scanLineState.IsFlagSet(ScanLineStateFlags.PreRender)))
                _registerPPUADDR = (_registerPPUADDR & 0x7BE0) | (_registerPPUSCROLL & 0x041F);

            // Copy vertical position data from t to v repeatedly from cycle 280 to 304 (if rendering is enabled)
            if (_currentCycle >= 280 && _currentCycle <= 304 && _currentScanline == 261)
                _registerPPUADDR = (_registerPPUADDR & 0x041F) | (_registerPPUSCROLL & 0x7BE0);
        }

        /// <summary>
        ///     Evaluates Sprites being rendered and retrieves data
        /// </summary>
        private void EvalSprites()
        {
            Array.Clear(_sprites, 0, _sprites.Length);
            Array.Clear(_spriteIndices, 0, _spriteIndices.Length);

            // 8x8 or 8x16 sprites
            var h = _registerPPUCTRL.IsFlagSet(PPUCtrlFlags.SpriteSize) ? 15 : 7;

            _countedSprites = 0;
            int yPos = _currentScanline;

            // Sprite evaluation starts at the current Data address and goes to the end of Data (256 bytes)
            for (int i = _registerOAMADDR; i < 256; i += 4)
            {
                var spriteYTop = _oamData[i];
                var offset = yPos - spriteYTop;

                // If this sprite is on the next scanline, copy it to the _sprites array for rendering
                if (offset <= h && offset >= 0)
                {
                    if (_countedSprites == 8)
                    {
                        _registerPPUSTATUS |= PPUStatusFlags.SpriteOverflow;
                        break;
                    }

                    Array.Copy(_oamData, i, _sprites, _countedSprites * 4, 4);
                    _spriteIndices[_countedSprites] = (i - _registerOAMADDR) / 4;
                    _countedSprites++;
                }
            }
        }

        /// <summary>
        ///     Updates current Cycle counter and fires events based off of
        ///     the current cycle/scanline or register values
        /// </summary>
        private void UpdateInternalCounters()
        {
            Cycles++;

            // Trigger an NMI at the start of _scanline 241 if VBLANK NMI's are enabled
            if (_currentScanline == 241 && _currentCycle == 1)
            {
                _registerPPUSTATUS |= PPUStatusFlags.VerticalBlankStarted;
                NMI = _registerPPUCTRL.IsFlagSet(PPUCtrlFlags.NMIEnabled);
            }

            // Skip last cycle of prerender scanline on odd frames
            if (_registerPPUMASK.IsFlagSet(PPUMaskFlags.ShowSprites) ||
                _registerPPUMASK.IsFlagSet(PPUMaskFlags.ShowBackground))
            {
                if (_currentScanline == MaxScanline && _currentCycle == 339 && _frameOrderToggle == 1)
                {
                    _frameOrderToggle ^= 1;
                    _currentScanline = 0;
                    _currentCycle = -1;
                    FrameReady = true;
                    return;
                }
            }

            _currentCycle++;

            // Reset cycle (and scanline if scanline == 260)
            // Also set to next frame if at end of last _scanline
            if (_currentCycle > MaxCycles)
            {
                if (_currentScanline == MaxScanline) // Last scanline, reset to upper left corner
                {
                    _frameOrderToggle ^= 1;
                    _currentScanline = 0;
                    _currentCycle = -1;
                    FrameReady = true;
                }
                else // Not on last scanline
                {
                    _currentCycle = -1;
                    _currentScanline++;
                }
            }
        }

        /// <summary>
        ///     Renders a Pixel to the Frame Buffer
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RenderPixel()
        {
            // Get pixel data (4 bits of tile shift register as specified by x)
            var bgPixelData = GetBackgroundPixelData();

            //Get pixel data for the Sprite (if one is present)
            var spritePixelData = GetSpritePixelData();

            //Determine if we loaded a sprite or if in fact we're rendering a background sprite
            var isBackgroundColor = (spritePixelData & 0x03) == 0;

            byte color;

            if ((bgPixelData & 0x03) == 0)
            {
                color = isBackgroundColor ? LookupBackgroundColor(bgPixelData) : LookupSpriteColor(spritePixelData);
            }
            else
            {
                if (isBackgroundColor)
                {
                    color = LookupBackgroundColor(bgPixelData);
                }
                else
                {
                    // Both pixels opaque, choose depending on sprite priority

                    // Set sprite zero hit flag
                    if (_spriteIndices[_spriteIndex] == 0)
                        _registerPPUSTATUS |= PPUStatusFlags.SpriteZeroHit;

                    // Get sprite priority
                    var priority = (_sprites[_spriteIndex * 4 + 2] >> 5) & 1;
                    color = priority == 1 ? LookupBackgroundColor(bgPixelData) : LookupSpriteColor(spritePixelData);
                }
            }

            //Write the pixel to the Frame Buffer
            FrameBuffer[_currentScanline * 256 + (_currentCycle - 1)] = color;
        }

        /// <summary>
        ///     Gets the color of the background Pixel (if one is being rendered)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte LookupBackgroundColor(byte data)
        {
            var colorNum = data & 0x3;
            var paletteNum = (data >> 2) & 0x3;

            // Special case for universal background color
            if (colorNum == 0) return PPUMemory.ReadByte(0x3F00);

            int paletteAddress;
            switch (paletteNum)
            {
                case 0:
                    paletteAddress = 0x3F01;
                    break;
                case 1:
                    paletteAddress = 0x3F05;
                    break;
                case 2:
                    paletteAddress = 0x3F09;
                    break;
                case 3:
                    paletteAddress = 0x3F0D;
                    break;
                default:
                    throw new Exception($"Invalid Background Palette Number: {paletteNum}");
            }

            paletteAddress += colorNum - 1;
            return PPUMemory.ReadByte(paletteAddress);
        }

        /// <summary>
        ///     Gets the color of the pixel in the current Sprite we are rendering
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        byte LookupSpriteColor(byte data)
        {
            var colorNum = data & 0x3;
            var paletteNum = (data >> 2) & 0x3;

            // Special case for universal background color
            if (colorNum == 0) return PPUMemory.ReadByte(0x3F00);

            int paletteAddress;
            switch (paletteNum)
            {
                case 0:
                    paletteAddress = 0x3F11;
                    break;
                case 1:
                    paletteAddress = 0x3F15;
                    break;
                case 2:
                    paletteAddress = 0x3F19;
                    break;
                case 3:
                    paletteAddress = 0x3F1D;
                    break;
                default:
                    throw new Exception($"Invalid Sprite Palette Number: {paletteNum}");
            }

            paletteAddress += colorNum - 1;
            return PPUMemory.ReadByte(paletteAddress);
        }

        /// <summary>
        ///     Gets the Pixel info for the current sprite at the current cycle
        ///
        ///     Returns 0x0 if there is no sprite at this cycle
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte GetSpritePixelData()
        {
            var xPos = _currentCycle - 1;
            var yPos = _currentScanline - 1;

            _spriteIndex = 0;

            //Bail if the PPUMASK flags are set to not show sprites
            if (!_registerPPUMASK.IsFlagSet(PPUMaskFlags.ShowSprites)) return 0;
            if (!_registerPPUMASK.IsFlagSet(PPUMaskFlags.ShowSpritesInLeftMost) && xPos < 8) return 0;

            // 8x8 sprites all come from the same pattern table as specified by a write to PPUCTRL
            // 8x16 sprites come from a pattern table defined in their Data data
            var currentSpritePatternTableOffset =
                _registerPPUCTRL.IsFlagSet(PPUCtrlFlags.SpriteTableAddress) ? 0x1000 : 0x0000;

            // Get sprite pattern bitfield
            for (var i = 0; i < _countedSprites * 4; i += 4)
            {
                var offset = xPos - _sprites[i + 3];

                //Check if the sprite intersects 
                if (offset <= 7 && offset >= 0)
                {
                    var yOffset = yPos - _sprites[i];

                    byte patternIndex;

                    // Set the pattern table and index according to whether or not sprites
                    // ar 8x8 or 8x16
                    if (_registerPPUCTRL.IsFlagSet(PPUCtrlFlags.SpriteSize))
                    {
                        currentSpritePatternTableOffset = (_sprites[i + 1] & 1) * 0x1000;
                        patternIndex = (byte) (_sprites[i + 1] & 0xFE);
                    }
                    else
                    {
                        patternIndex = _sprites[i + 1];
                    }

                    var patternAddress = currentSpritePatternTableOffset + (patternIndex * 16);

                    var flipHorizontal = (_sprites[i + 2] & 0x40) != 0;
                    var flipVertical = (_sprites[i + 2] & 0x80) != 0;
                    var colorNum = GetSpritePatternPixel(patternAddress, offset, yOffset, flipHorizontal, flipVertical);

                    // Handle transparent sprites
                    if (colorNum == 0)
                        continue;

                    var paletteNum = _sprites[i + 2] & 0x03;
                    _spriteIndex = i / 4;
                    return (byte) (((paletteNum << 2) | colorNum) & 0xF);
                }
            }

            //No Sprite to Render
            return 0x0;
        }

        /// <summary>
        ///     Looks up the color of the Pixel at the specific location in a sprite
        /// </summary>
        /// <param name="patternAddr">Address of the Sprite we're looking up</param>
        /// <param name="xPos">X position of the Pixel in the Sprite</param>
        /// <param name="yPos">Y position of the Pixel in the Sprite</param>
        /// <param name="flipHoriz">Is the Sprite flipped horizontally?</param>
        /// <param name="flipVert">Is the Sprite flipped vertically?</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int GetSpritePatternPixel(int patternAddr, int xPos, int yPos, bool flipHoriz = false, bool flipVert = false)
        {
            var h = _registerPPUCTRL.IsFlagSet(PPUCtrlFlags.SpriteSize) ? 15 : 7;

            // Flip x and y if needed
            xPos = flipHoriz ? 7 - xPos : xPos;
            yPos = flipVert ? h - yPos : yPos;

            // First byte in bitfield, wrapping accordingly for y > 7 (8x16 sprites)
            int yAddr;
            if (yPos <= 7) yAddr = patternAddr + yPos;
            else yAddr = patternAddr + 16 + (yPos - 8); // Go to next tile for 8x16 sprites

            // Read the 2 bytes in the bitfield for the y coordinate
            var pattern = new byte[2];
            pattern[0] = PPUMemory.ReadByte(yAddr);
            pattern[1] = PPUMemory.ReadByte(yAddr + 8);

            // Extract correct bits based on x coordinate
            var loBit = (pattern[0] >> (7 - xPos)) & 1;
            var hiBit = (pattern[1] >> (7 - xPos)) & 1;

            return ((hiBit << 1) | loBit) & 0x03;
        }

        /// <summary>
        ///     Stores the read tile data into the PPU shift registers
        /// </summary>
        private void StoreTileData()
        {
            var palette = (_attributeTableByte >> ((CoarseX() & 0x2) | ((CoarseY() & 0x2) << 1))) & 0x3;

            // Upper 32 bits to add to _tileShiftReg
            ulong data = 0;

            for (var i = 0; i < 8; i++)
            {
                // Get color number
                var loColorBit = (_tileDataLow >> (7 - i)) & 1;
                var hiColorBit = (_tileDataHigh >> (7 - i)) & 1;
                var colorNum = (hiColorBit << 1) | (loColorBit & 0x03);

                // Add palette number
                var fullPixelData = (((palette << 2) | colorNum) & 0xF);

                data |= (uint) (fullPixelData << (4 * i));
            }

            _tileShiftRegister &= 0xFFFFFFFF;
            _tileShiftRegister |= data << 32;
        }

        /// <summary>
        ///     The coarse X component of _registerPPUADDR needs to be incremented when the next tile is reached
        ///
        ///     Bits 0-4 are incremented, with overflow toggling bit 10. This means that bits 0-4 count from
        ///     0 to 31 across a single nametable, and bit 10 selects the current nametable horizontally.
        ///
        ///     More Info: https://wiki.nesdev.com/w/index.php/PPU_scrolling#Coarse_X_increment
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IncrementX()
        {
            if ((_registerPPUADDR & 0x001F) == 31)
            {
                _registerPPUADDR &= ~0x001F; // Coarse X = 0
                _registerPPUADDR ^= 0x0400; // Switch horizontal nametable
            }
            else
            {
                _registerPPUADDR++; // Increment Coarse X
            }
        }


        /// <summary>
        ///     If rendering is enabled, fine Y is incremented at dot 256 of each scanline, overflowing to coarse Y,
        ///     and finally adjusted to wrap among the nametables vertically.
        ///
        ///     Bits 12-14 are fine Y. Bits 5-9 are coarse Y. Bit 11 selects the vertical nametable.
        ///
        ///     More Info: https://wiki.nesdev.com/w/index.php/PPU_scrolling#Y_increment
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IncrementY()
        {
            if ((_registerPPUADDR & 0x7000) != 0x7000) // if fine Y < 7
            {
                _registerPPUADDR += 0x1000; // increment fine Y
            }
            else
            {
                _registerPPUADDR &= ~0x7000; // Fine Y = 0
                var y = (_registerPPUADDR & 0x03E0) >> 5; // y = Coarse Y
                switch (y)
                {
                    case 29:
                        y = 0; // coarse Y = 0
                        _registerPPUADDR ^= 0x0800; // switch vertical nametable
                        break;
                    case 31:
                        y = 0; // coarse Y = 0, nametable not switched
                        break;
                    default:
                        y++; // Increment coarse Y
                        break;
                }

                _registerPPUADDR = (_registerPPUADDR & ~0x03E0) | (y << 5); // Put coarse Y back into v
            }
        }

        /// <summary>
        ///     Retrieves the background pixel from the Tile Shift Register
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte GetBackgroundPixelData()
        {
            if (!_registerPPUMASK.IsFlagSet(PPUMaskFlags.ShowBackground)) return 0;
            if (!_registerPPUMASK.IsFlagSet(PPUMaskFlags.ShowBackgroundInLeftMost) && (_currentCycle - 1) < 8) return 0;
            return (byte) ((_tileShiftRegister >> (_X * 4)) & 0xF);
        }

        /*---------------------------
         * Tile & Attribute Fetching
         *---------------------------
         *
         * NN 1111 YYY XXX
         * || |||| ||| +++-- high 3 bits of coarse X (x/4)
         * || |||| +++------ high 3 bits of coarse Y (y/4)
         * || ++++---------- attribute offset (960 bytes)
         * ++--------------- nametable select
         */

        /// <summary>
        ///     Retrieve Coarse X
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int CoarseX() => _registerPPUADDR & 0x1f;

        /// <summary>
        ///     Retrieve Coarse Y
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int CoarseY() => (_registerPPUADDR >> 5) & 0x1f;

        /// <summary>
        ///     Retrieve Fine Y
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int FineY() => (_registerPPUADDR >> 12) & 0x7;

        /// <summary>
        ///     The least significant 5 bits of any write, to any registers
        ///     is saved to the least significant 5 bits of PPU status.
        ///
        ///     To do this, we take the first three bits of the existing PPUSTATUS
        ///     register value, then OR it with the 5 least significant bits of the
        ///     last value written to a register.
        ///
        ///     More Info: https://wiki.nesdev.com/w/index.php/PPU_registers#PPUSTATUS
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdatePPUSTATUSRegister(byte value) =>
            _registerPPUSTATUS = (byte) ((_registerPPUSTATUS & 0xE0) | (value & 0x1F));
    }
}
