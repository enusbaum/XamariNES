using System;

namespace XamariNES.Common.Bitmap
{
    /// <summary>
    ///     Creates Valid BMP Files from a raw array of Bitmap Pixel Data
    /// </summary>
    public class BitmapFactory
    {
        /// <summary>
        ///     Array Combination of BMP Header and DIP Header
        /// </summary>
        private readonly byte[] bmpHeader = new byte[0x36];

        /// <summary>
        ///     Bitmap Color Table for NES Color Palette
        ///     
        ///     64 Colors * 4 bytes per entry in the Color Table
        /// </summary>
        private readonly byte[] bmpColorTable = new byte[0x40 * 4]; 

        /// <summary>
        ///     Array containing 8bpp Pixel Data for Bitmap
        /// </summary>
        private readonly byte[] bmpData = new byte[0xF000];

        /// <summary>
        ///     Final Bitmap File used to return output
        /// </summary>
        private readonly byte[] bitmapFile; 

        public BitmapFactory()
        {
            //BMP Header
            Array.Copy(new byte[] { 0x42, 0x4D }, 0, bmpHeader, 0x0, 2); //ID Field, "BM" == Windows 3.1x, 95, NT
            Array.Copy(new byte[] { 0x36, 0xF0, 0x00, 0x00 }, 0, bmpHeader, 0x2, 4); //Size of BMP Header+Data
            Array.Copy(new byte[] { 0x00, 0x00 }, 0, bmpHeader, 0x6, 2); //Application Specific
            Array.Copy(new byte[] { 0x00, 0x00 }, 0, bmpHeader, 0x8, 2); //Application Specific
            Array.Copy(BitConverter.GetBytes(bmpHeader.Length + bmpColorTable.Length), 0, bmpHeader, 0xA, 4); //Offset where Pixel Data Starts

            //DIB Header
            Array.Copy(new byte[] { 0x28, 0x00, 0x00, 0x00 }, 0, bmpHeader, 0xE, 4); //Number of bytes in DIB header (from this point)
            Array.Copy(new byte[] { 0x00, 0x01, 0x00, 0x00 }, 0, bmpHeader, 0x12, 4); //Width == 256
            Array.Copy(new byte[] { 0xF0, 0x00, 0x00, 0x00 }, 0, bmpHeader, 0x16, 4); //Height == 240
            Array.Copy(new byte[] { 0x01, 0x00 }, 0, bmpHeader, 0x1A, 2); //Color Planes Being Used
            Array.Copy(new byte[] { 0x08, 0x00 }, 0, bmpHeader, 0x1C, 2); //8 bits per pixel
            Array.Copy(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0, bmpHeader, 0x1E, 4); //no compression
            Array.Copy(new byte[] { 0x00, 0xF0, 0x00, 0x00 }, 0, bmpHeader, 0x22, 4); //size of bmp data
            Array.Copy(new byte[] { 0x13, 0x0B, 0x00, 0x00 }, 0, bmpHeader, 0x26, 4); //horizontal print resolution
            Array.Copy(new byte[] { 0x13, 0x0B, 0x00, 0x00 }, 0, bmpHeader, 0x2A, 4); //vertical print resolution
            Array.Copy(new byte[] { 0x40, 0x00, 0x00, 0x00 }, 0, bmpHeader, 0x2E, 4); //colors in palette
            Array.Copy(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0, bmpHeader, 0x32, 4); //all colors are important

            //Build Color Tables based on NES Color Palette
            Array.Copy(new byte[] { 84, 84, 84, 0 }, 0, bmpColorTable, 0, 4);
            Array.Copy(new byte[] { 116, 30, 0, 0 }, 0, bmpColorTable, 4, 4);
            Array.Copy(new byte[] { 144, 16, 8, 0 }, 0, bmpColorTable, 8, 4);
            Array.Copy(new byte[] { 136, 0, 48, 0 }, 0, bmpColorTable, 12, 4);
            Array.Copy(new byte[] { 100, 0, 68, 0 }, 0, bmpColorTable, 16, 4);
            Array.Copy(new byte[] { 48, 0, 92, 0 }, 0, bmpColorTable, 20, 4);
            Array.Copy(new byte[] { 0, 4, 84, 0 }, 0, bmpColorTable, 24, 4);
            Array.Copy(new byte[] { 0, 24, 60, 0 }, 0, bmpColorTable, 28, 4);
            Array.Copy(new byte[] { 0, 42, 32, 0 }, 0, bmpColorTable, 32, 4);
            Array.Copy(new byte[] { 0, 58, 8, 0 }, 0, bmpColorTable, 36, 4);
            Array.Copy(new byte[] { 0, 64, 0, 0 }, 0, bmpColorTable, 40, 4);
            Array.Copy(new byte[] { 0, 60, 0, 0 }, 0, bmpColorTable, 44, 4);
            Array.Copy(new byte[] { 60, 50, 0, 0 }, 0, bmpColorTable, 48, 4);
            Array.Copy(new byte[] { 0, 0, 0, 0 }, 0, bmpColorTable, 52, 4);
            Array.Copy(new byte[] { 0, 0, 0, 0 }, 0, bmpColorTable, 56, 4);
            Array.Copy(new byte[] { 0, 0, 0, 0 }, 0, bmpColorTable, 60, 4);
            Array.Copy(new byte[] { 152, 150, 152, 0 }, 0, bmpColorTable, 64, 4);
            Array.Copy(new byte[] { 196, 76, 8, 0 }, 0, bmpColorTable, 68, 4);
            Array.Copy(new byte[] { 236, 50, 48, 0 }, 0, bmpColorTable, 72, 4);
            Array.Copy(new byte[] { 228, 30, 92, 0 }, 0, bmpColorTable, 76, 4);
            Array.Copy(new byte[] { 176, 20, 136, 0 }, 0, bmpColorTable, 80, 4);
            Array.Copy(new byte[] { 100, 20, 160, 0 }, 0, bmpColorTable, 84, 4);
            Array.Copy(new byte[] { 32, 34, 152, 0 }, 0, bmpColorTable, 88, 4);
            Array.Copy(new byte[] { 0, 60, 120, 0 }, 0, bmpColorTable, 92, 4);
            Array.Copy(new byte[] { 0, 90, 84, 0 }, 0, bmpColorTable, 96, 4);
            Array.Copy(new byte[] { 0, 114, 40, 0 }, 0, bmpColorTable, 100, 4);
            Array.Copy(new byte[] { 0, 124, 8, 0 }, 0, bmpColorTable, 104, 4);
            Array.Copy(new byte[] { 40, 118, 0, 0 }, 0, bmpColorTable, 108, 4);
            Array.Copy(new byte[] { 120, 102, 0, 0 }, 0, bmpColorTable, 112, 4);
            Array.Copy(new byte[] { 0, 0, 0, 0 }, 0, bmpColorTable, 116, 4);
            Array.Copy(new byte[] { 0, 0, 0, 0 }, 0, bmpColorTable, 120, 4);
            Array.Copy(new byte[] { 0, 0, 0, 0 }, 0, bmpColorTable, 124, 4);
            Array.Copy(new byte[] { 236, 238, 236, 0 }, 0, bmpColorTable, 128, 4);
            Array.Copy(new byte[] { 236, 154, 76, 0 }, 0, bmpColorTable, 132, 4);
            Array.Copy(new byte[] { 236, 124, 120, 0 }, 0, bmpColorTable, 136, 4);
            Array.Copy(new byte[] { 236, 98, 176, 0 }, 0, bmpColorTable, 140, 4);
            Array.Copy(new byte[] { 236, 84, 228, 0 }, 0, bmpColorTable, 144, 4);
            Array.Copy(new byte[] { 180, 88, 236, 0 }, 0, bmpColorTable, 148, 4);
            Array.Copy(new byte[] { 100, 106, 236, 0 }, 0, bmpColorTable, 152, 4);
            Array.Copy(new byte[] { 32, 136, 212, 0 }, 0, bmpColorTable, 156, 4);
            Array.Copy(new byte[] { 0, 170, 160, 0 }, 0, bmpColorTable, 160, 4);
            Array.Copy(new byte[] { 0, 196, 116, 0 }, 0, bmpColorTable, 164, 4);
            Array.Copy(new byte[] { 32, 208, 76, 0 }, 0, bmpColorTable, 168, 4);
            Array.Copy(new byte[] { 108, 204, 56, 0 }, 0, bmpColorTable, 172, 4);
            Array.Copy(new byte[] { 204, 180, 56, 0 }, 0, bmpColorTable, 176, 4);
            Array.Copy(new byte[] { 60, 60, 60, 0 }, 0, bmpColorTable, 180, 4);
            Array.Copy(new byte[] { 0, 0, 0, 0 }, 0, bmpColorTable, 184, 4);
            Array.Copy(new byte[] { 0, 0, 0, 0 }, 0, bmpColorTable, 188, 4);
            Array.Copy(new byte[] { 236, 238, 236, 0 }, 0, bmpColorTable, 192, 4);
            Array.Copy(new byte[] { 236, 204, 168, 0 }, 0, bmpColorTable, 196, 4);
            Array.Copy(new byte[] { 236, 188, 188, 0 }, 0, bmpColorTable, 200, 4);
            Array.Copy(new byte[] { 236, 178, 212, 0 }, 0, bmpColorTable, 204, 4);
            Array.Copy(new byte[] { 236, 174, 236, 0 }, 0, bmpColorTable, 208, 4);
            Array.Copy(new byte[] { 212, 174, 236, 0 }, 0, bmpColorTable, 212, 4);
            Array.Copy(new byte[] { 176, 180, 236, 0 }, 0, bmpColorTable, 216, 4);
            Array.Copy(new byte[] { 144, 196, 228, 0 }, 0, bmpColorTable, 220, 4);
            Array.Copy(new byte[] { 120, 210, 204, 0 }, 0, bmpColorTable, 224, 4);
            Array.Copy(new byte[] { 120, 222, 180, 0 }, 0, bmpColorTable, 228, 4);
            Array.Copy(new byte[] { 144, 226, 168, 0 }, 0, bmpColorTable, 232, 4);
            Array.Copy(new byte[] { 180, 226, 152, 0 }, 0, bmpColorTable, 236, 4);
            Array.Copy(new byte[] { 228, 214, 160, 0 }, 0, bmpColorTable, 240, 4);
            Array.Copy(new byte[] { 160, 162, 160, 0 }, 0, bmpColorTable, 244, 4);
            Array.Copy(new byte[] { 0, 0, 0, 0 }, 0, bmpColorTable, 248, 4);
            Array.Copy(new byte[] { 0, 0, 0, 0 }, 0, bmpColorTable, 252, 4);

            //Setup the output file buffer
            bitmapFile = new byte[bmpHeader.Length + bmpColorTable.Length + bmpData.Length];
            Array.Copy(bmpHeader, 0, bitmapFile, 0, bmpHeader.Length);
            Array.Copy(bmpColorTable, 0, bitmapFile, 0x36, bmpColorTable.Length);
        }

        /// <summary>
        ///     Generates a valid Bitmap file from the specified pixels
        /// </summary>
        /// <param name="bitmapPixels"></param>
        /// <returns></returns>
        public byte[] BitmapFromByteArray(byte[] bitmapPixels)
        {
            /* The NES writes the first byte of the BMP data represents the first pixel of the image,
             * in the BMP file format, the first byte represents the LAST pixel. Because of this, we need
             * to reverse the order.
             */
            //Flip Vertical Orientation
            Array.Reverse(bitmapPixels);
            
            //Flip Each Line Horizontally
            for(var i = 0; i < bitmapPixels.Length; i+= 256)
            {
                Array.Reverse(bitmapPixels, i, 256);
            }

            Array.Copy(bitmapPixels, 0, bitmapFile, bmpHeader.Length + bmpColorTable.Length, bitmapPixels.Length);

            return bitmapFile;
        }
    }
}
