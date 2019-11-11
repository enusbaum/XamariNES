using System;
using System.IO;
using System.Threading.Tasks;
using XamariNES.Common.Bitmap;
using XamariNES.Emulator;

namespace XamariNES.UI.App.Console
{
    class Program
    {
        private static string _inputFile;
        private static string _outputPath;
        private static NESEmulator _emulator;
        private static BitmapFactory _bmpFactory = new BitmapFactory();

        private static int frameCount;
        static void Main(string[] args)
        {
            
            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i].ToUpper())
                {
                    case "-I":
                        _inputFile = args[i + 1];
                        i++;
                        break;
                    case "-O":
                        _outputPath = args[i + 1];
                        i++;
                        break;
                    case "-?":
                        System.Console.WriteLine("-I <file> -- Input ROM");
                        System.Console.WriteLine("-O <file> -- Output Directory for bitmaps to be rendered to");
                        return;
                }
            }

            _emulator = new NESEmulator(File.ReadAllBytes(_inputFile), RenderFrame, Emulator.Enums.enumEmulatorSpeed.Half);
            _emulator.Start();
            System.Console.ReadKey();
        }

        private static async Task RenderFrame(byte[] frameData)
        {
            try
            {
                File.WriteAllBytes($"{_outputPath}{frameCount}.bmp", _bmpFactory.BitmapFromByteArray(frameData));
                frameCount++;
            }
            catch(Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }
    }
}
