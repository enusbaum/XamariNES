using System;
using SkiaSharp;

namespace XamariNES.UI.App.Renderer
{
    /// <summary>
    ///     Renderer that generates SKBitmaps from input data
    /// </summary>
    public class SKBitmapRenderer
    {
        private readonly SKBitmap _bitmap;
        private readonly SKColor[] _colorPalette;
        private readonly Random _random = new Random(DateTime.Now.GetHashCode());

        /// <summary>
        ///     Default Constructor
        /// </summary>
        public SKBitmapRenderer()
        {
            //Set Palette based on NES 2C02 Palette
            //More Info: https://wiki.nesdev.com/w/index.php/PPU_palettes
            _colorPalette = new SKColor[0x40];
            _colorPalette[0x0] = new SKColor(84, 84, 84);
            _colorPalette[0x1] = new SKColor(0, 30, 116);
            _colorPalette[0x2] = new SKColor(8, 16, 144);
            _colorPalette[0x3] = new SKColor(48, 0, 136);
            _colorPalette[0x4] = new SKColor(68, 0, 100);
            _colorPalette[0x5] = new SKColor(92, 0, 48);
            _colorPalette[0x6] = new SKColor(84, 4, 0);
            _colorPalette[0x7] = new SKColor(60, 24, 0);
            _colorPalette[0x8] = new SKColor(32, 42, 0);
            _colorPalette[0x9] = new SKColor(8, 58, 0);
            _colorPalette[0xa] = new SKColor(0, 64, 0);
            _colorPalette[0xb] = new SKColor(0, 60, 0);
            _colorPalette[0xc] = new SKColor(0, 50, 60);
            _colorPalette[0xd] = new SKColor(0, 0, 0);
            _colorPalette[0xe] = new SKColor(0, 0, 0);
            _colorPalette[0xf] = new SKColor(0, 0, 0);
            _colorPalette[0x10] = new SKColor(152, 150, 152);
            _colorPalette[0x11] = new SKColor(8, 76, 196);
            _colorPalette[0x12] = new SKColor(48, 50, 236);
            _colorPalette[0x13] = new SKColor(92, 30, 228);
            _colorPalette[0x14] = new SKColor(136, 20, 176);
            _colorPalette[0x15] = new SKColor(160, 20, 100);
            _colorPalette[0x16] = new SKColor(152, 34, 32);
            _colorPalette[0x17] = new SKColor(120, 60, 0);
            _colorPalette[0x18] = new SKColor(84, 90, 0);
            _colorPalette[0x19] = new SKColor(40, 114, 0);
            _colorPalette[0x1a] = new SKColor(8, 124, 0);
            _colorPalette[0x1b] = new SKColor(0, 118, 40);
            _colorPalette[0x1c] = new SKColor(0, 102, 120);
            _colorPalette[0x1d] = new SKColor(0, 0, 0);
            _colorPalette[0x1e] = new SKColor(0, 0, 0);
            _colorPalette[0x1f] = new SKColor(0, 0, 0);
            _colorPalette[0x20] = new SKColor(236, 238, 236);
            _colorPalette[0x21] = new SKColor(76, 154, 236);
            _colorPalette[0x22] = new SKColor(120, 124, 236);
            _colorPalette[0x23] = new SKColor(176, 98, 236);
            _colorPalette[0x24] = new SKColor(228, 84, 236);
            _colorPalette[0x25] = new SKColor(236, 88, 180);
            _colorPalette[0x26] = new SKColor(236, 106, 100);
            _colorPalette[0x27] = new SKColor(212, 136, 32);
            _colorPalette[0x28] = new SKColor(160, 170, 0);
            _colorPalette[0x29] = new SKColor(116, 196, 0);
            _colorPalette[0x2a] = new SKColor(76, 208, 32);
            _colorPalette[0x2b] = new SKColor(56, 204, 108);
            _colorPalette[0x2c] = new SKColor(56, 180, 204);
            _colorPalette[0x2d] = new SKColor(60, 60, 60);
            _colorPalette[0x2e] = new SKColor(0, 0, 0);
            _colorPalette[0x2f] = new SKColor(0, 0, 0);
            _colorPalette[0x30] = new SKColor(236, 238, 236);
            _colorPalette[0x31] = new SKColor(168, 204, 236);
            _colorPalette[0x32] = new SKColor(188, 188, 236);
            _colorPalette[0x33] = new SKColor(212, 178, 236);
            _colorPalette[0x34] = new SKColor(236, 174, 236);
            _colorPalette[0x35] = new SKColor(236, 174, 212);
            _colorPalette[0x36] = new SKColor(236, 180, 176);
            _colorPalette[0x37] = new SKColor(228, 196, 144);
            _colorPalette[0x38] = new SKColor(204, 210, 120);
            _colorPalette[0x39] = new SKColor(180, 222, 120);
            _colorPalette[0x3a] = new SKColor(168, 226, 144);
            _colorPalette[0x3b] = new SKColor(152, 226, 180);
            _colorPalette[0x3c] = new SKColor(160, 214, 228);
            _colorPalette[0x3d] = new SKColor(160, 162, 160);
            _colorPalette[0x3e] = new SKColor(0, 0, 0);
            _colorPalette[0x3f] = new SKColor(0, 0, 0);

            //SKBitmap to reuse for each frame
            _bitmap = new SKBitmap(new SKImageInfo(256, 240));
        }

        /// <summary>
        ///     Takes the input 8bpp bitmap and renders it as a SKBitmap
        ///     using the pre-defined Color Palette
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public SKBitmap Render(byte[] bitmap)
        {
            for (var y = 0; y < 240; y++)
            {
                for (var x = 0; x < 256; x++)
                {
                    _bitmap.SetPixel(x,y,_colorPalette[bitmap[y*256+x]]);
                }
            }
            return _bitmap;
        }

        /// <summary>
        ///     Renders a black/white noise pattern
        /// </summary>
        /// <returns></returns>
        public byte[] GenerateNoise()
        {
            var output = new byte[256 * 240];
            for (var i = 0; i < output.Length; i++)
            {
                output[i] = _random.Next(0, 10) <= 5 ? (byte)0xd : (byte)0x30;
            }
            return output;
        }
    }
}
