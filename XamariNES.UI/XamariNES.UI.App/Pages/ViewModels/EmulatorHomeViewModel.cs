using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using XamariNES.Controller.Enums;
using XamariNES.Emulator;
using XamariNES.UI.App.Renderer;

namespace XamariNES.UI.App.Pages.ViewModels
{
    public class EmulatorHomeViewModel : ViewModelBase
    {
        //Commands
        public Command PaintEmulatorCommand { get; }
        public Command PaintControllerCommand { get; }
        public Command PaintConsoleCommand { get; }
        public Command TouchConsoleCommand { get; }
        public Command TouchControllerCommand { get; }

        //Resource Readers
        private readonly Resources.Reader _resourceReader;

        //Resource Names
        private const string ResourceConsoleImage = "XamariNES.UI.App.Resources.images.nesconsole.png";
        private const string ResourceControllerImage = "XamariNES.UI.App.Resources.images.nescontroller.png";
        private const string ResourceRom = "XamariNES.UI.App.Resources.roms.nestest.nes";

        //Emulator Specific Variables
        private readonly NESEmulator _emu;
        private byte[] _frame = new byte[256*240];
        private readonly SKBitmapRenderer _renderer;
        private bool _powerOn;
        private float _screenHeight;
        private bool _horizontalPaddingSet;
        private float _horizontalPadding;
        private long _framesRendered;

        //Console Button Areas
        private SKRect _powerOnButtonRect;
        private SKRect _loadCartridgeButtonRect;

        //Controller Button Areas
        private SKRect _controllerUpRect;
        private SKRect _controllerDownRect;
        private SKRect _controllerRightRect;
        private SKRect _controllerLeftRect;
        private SKRect _controllerStartRect;
        private SKRect _controllerSelectRect;
        private SKRect _controllerARect;
        private SKRect _controllerBRect;


        private readonly Dictionary<long, enumButtons> _controllerEventTracker = new Dictionary<long, enumButtons>();

        //Tasks
        private Task _renderStaticTask;
        private Task _measureFpsTask;

        public EmulatorHomeViewModel()
        {
            //Setup Paint and Touch Commands for SKView elements
            PaintEmulatorCommand = new Command<SKPaintSurfaceEventArgs>(PaintEmulator);
            PaintControllerCommand = new Command<SKPaintSurfaceEventArgs>(PaintController);
            PaintConsoleCommand = new Command<SKPaintSurfaceEventArgs>(PaintConsole);
            TouchConsoleCommand = new Command<SKTouchEventArgs>(TouchConsole);
            TouchControllerCommand = new Command<SKTouchEventArgs>(TouchController);

            //Setup Renderer
            _resourceReader = new Resources.Reader();
            _renderer = new SKBitmapRenderer();

            //Setup Emulator
            _emu = new NESEmulator(_resourceReader.GetResource(ResourceRom), GetFrameFromEmulator);

            //Setup Static Generator
            _renderStaticTask = Task.Factory.StartNew(RenderStatic);
        }

        /// <summary>
        ///     Task Delegate to render static to the emulator Canvas
        /// </summary>
        public void RenderStatic()
        {
            while(!_powerOn)
            {
                _frame = _renderer.GenerateNoise();
                MessagingCenter.Send(this, "InvalidateEmulatorSurface");
                Thread.Sleep(33); //~29.97fps -- NTSC
            }
        }

        /// <summary>
        ///     Logs our current FPS to the output console
        /// </summary>
        public void MeasureFPS()
        {
            long previousFps = 0;
            while(_powerOn)
            {
                Thread.Sleep(1000);
                Console.WriteLine($"{_framesRendered - previousFps}fps");
                previousFps = _framesRendered;
            }
        }

        /// <summary>
        ///     Delegate to receive frame that's ready from the emulator and
        ///     trigger a draw event.
        ///
        ///     TODO: Because this isn't thread safe, this might lead to some
        ///     screen tearing. Probably need to refactor this.
        /// </summary>
        /// <param name="frame"></param>
        private void GetFrameFromEmulator(byte[] frame)
        {
            _frame = frame;
            MessagingCenter.Send(this, "InvalidateEmulatorSurface");
        }

        /// <summary>
        ///     Touch Event handler on the Controller Canvas
        /// </summary>
        /// <param name="e"></param>
        void TouchController(SKTouchEventArgs e)
        {
            switch(e.ActionType)
            {
                case SKTouchAction.Moved:
                    break;
                case SKTouchAction.Pressed:
                    {
                        //Determine which button is being pressed
                        enumButtons buttonPressed;
                        if (_controllerUpRect.Contains(e.Location))
                            buttonPressed = enumButtons.Up;
                        else if (_controllerDownRect.Contains(e.Location))
                            buttonPressed = enumButtons.Down;
                        else if (_controllerRightRect.Contains(e.Location))
                            buttonPressed = enumButtons.Right;
                        else if (_controllerLeftRect.Contains(e.Location))
                            buttonPressed = enumButtons.Left;
                        else if (_controllerStartRect.Contains(e.Location))
                            buttonPressed = enumButtons.Start;
                        else if (_controllerSelectRect.Contains(e.Location))
                            buttonPressed = enumButtons.Select;
                        else if (_controllerARect.Contains(e.Location))
                            buttonPressed = enumButtons.A;
                        else if (_controllerBRect.Contains(e.Location))
                            buttonPressed = enumButtons.B;
                        else
                            return;

                        //Track The Event
                        _controllerEventTracker.Add(e.Id, buttonPressed);

                        //Press It
                        _emu.Controller1.ButtonPress(buttonPressed);
                    }
                    break;

                case SKTouchAction.Released:
                    {
                        //Find the button this event started pressing
                        _controllerEventTracker.TryGetValue(e.Id, out var buttonReleased);

                        //Release that button
                        _emu.Controller1.ButtonRelease(buttonReleased);

                        //Clear this event
                        _controllerEventTracker.Remove(e.Id);
                    }
                    break;
            }

            //Handle Event and bail
            e.Handled = true;
        }

        /// <summary>
        ///     Touch Event handler on the Console Canvas
        /// </summary>
        /// <param name="e"></param>
        void TouchConsole(SKTouchEventArgs e)
        {
            if (e.ActionType == SKTouchAction.Pressed)
            {
                if (_powerOnButtonRect.Contains(e.Location))
                {
                    if (!_powerOn)
                    {
                        _powerOn = true;
                        _framesRendered = 0;
                        _emu.Start();
                        _measureFpsTask = Task.Factory.StartNew(MeasureFPS);
                    }
                    else
                    {
                        _emu.Stop();
                        _powerOn = false;
                        _renderStaticTask = Task.Factory.StartNew(RenderStatic);
                        MessagingCenter.Send(this, "InvalidateEmulatorSurface");
                    }
                }
                else if (_loadCartridgeButtonRect.Contains(e.Location))
                {
                    SelectRomFile();
                }

                MessagingCenter.Send(this, "InvalidateConsoleSurface");
            }
        }

        /// <summary>
        ///     Invoked by Command when the SkiaSharp Canvas for the chart is painted
        /// </summary>
        /// <param name="e">E.</param>
        void PaintEmulator(SKPaintSurfaceEventArgs e)
        {
            //Only use clear when emulator is off, helps with performance
            if(!_powerOn)
                e.Surface.Canvas.Clear(SKColors.Black);

            //Set Horizontal Padding for Top/Bottom of Emulator Screen
            //TODO: Need to clean this up to handle situations where the screen might be very wide, which makes it way too tall
            if (!_horizontalPaddingSet)
            {
                _horizontalPaddingSet = true;
                _screenHeight = (e.Info.Rect.Right / 1.066f);
                _horizontalPadding = (e.Info.Height - _screenHeight) / 2;
            }

            //Draw the rendered frame
            e.Surface.Canvas.DrawBitmap(_renderer.Render(_frame), new SKRect(0, _horizontalPadding, e.Info.Rect.Right, _screenHeight + _horizontalPadding));
            _framesRendered++;
        }

        /// <summary>
        ///     Draws the Controller Bitmap onto the Canvas for Controller
        /// </summary>
        /// <param name="e"></param>
        void PaintController(SKPaintSurfaceEventArgs e)
        {
            var controllerRect = new SKRect(0, 0, e.Info.Rect.Right, (e.Info.Rect.Right / 2.56f));
            var controllerImage = SKBitmap.Decode(_resourceReader.GetResource(ResourceControllerImage));

            //Determine the ratios difference between the original image and the resized image
            var xRatio = ((float)controllerRect.Width / controllerImage.Width);
            var yRatio = ((float)controllerRect.Height / controllerImage.Height);

            _controllerLeftRect = new SKRect(50 * xRatio, 127 * yRatio, 95 * xRatio, 180 * yRatio);
            _controllerRightRect = new SKRect(150 * xRatio, 127 * yRatio, 190 * xRatio, 180 * yRatio);
            _controllerUpRect = new SKRect(95 * xRatio, 75 * yRatio, 155 * xRatio, 120 * yRatio);
            _controllerDownRect = new SKRect(95 * xRatio, 180 * yRatio, 150 * xRatio, 225 * yRatio);
            _controllerStartRect = new SKRect(350 * xRatio, 175 * yRatio, 400 * xRatio, 205 * yRatio);
            _controllerSelectRect = new SKRect(260 * xRatio, 175 * yRatio, 310 * xRatio, 205 * yRatio);
            _controllerARect = new SKRect(570 * xRatio, 160 * yRatio, 635 * xRatio, 225 * yRatio);
            _controllerBRect = new SKRect(475 * xRatio, 160 * yRatio, 540 * xRatio, 225 * yRatio);

            e.Surface.Canvas.Clear(SKColors.Black);
            e.Surface.Canvas.DrawBitmap(controllerImage, controllerRect);
        }

        /// <summary>
        ///     Draws the Console Bitmap on the the Canvas for the Console
        /// </summary>
        /// <param name="e"></param>
        void PaintConsole(SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear(SKColors.Black);

            //Rectangle of Drawable Area
            var consoleRect = new SKRect(0, 0, e.Info.Rect.Right, (e.Info.Rect.Right / 3.02f));

            //Draw the console
            var consoleImage = SKBitmap.Decode(_resourceReader.GetResource(ResourceConsoleImage));
            e.Surface.Canvas.DrawBitmap(consoleImage, consoleRect);

            //Determine the ratios difference between the original image and the resized image
            var xRatio = ((float)consoleRect.Width / consoleImage.Width);
            var yRatio = ((float)consoleRect.Height / consoleImage.Height);

            //Define Power On Light based on original location * ratio
            if (_powerOn)
            {
                var powerOnLightRect = new SKRect(135 * xRatio, 255 * yRatio, 150 * xRatio, 270 * yRatio);
                e.Surface.Canvas.DrawRect(powerOnLightRect,  new SKPaint() {Color = SKColors.Red, Style = SKPaintStyle.StrokeAndFill});
            }

            //Set Power Button Rectangle, used to determine touch event location
            _powerOnButtonRect = new SKRect(170 * xRatio, 240 * yRatio, 270 * xRatio, 280 * yRatio);

            // Load cartridge - TODO: Fix location on screen
            _loadCartridgeButtonRect = new SKRect(170 * xRatio, 100 * yRatio, 370 * xRatio, 240 * yRatio);
        }

        /// <summary>
        ///     Select NES ROM using local file explorer
        /// </summary>
        private async Task SelectRomFile()
        {
            string[] fileTypes = null;

            //if (Device.RuntimePlatform == Device.Android)
            //{
            //    fileTypes = new string[] { "image/png", "image/jpeg" };
            //}

            //if (Device.RuntimePlatform == Device.iOS)
            //{
            //    fileTypes = new string[] { "public.image" }; // same as iOS constant UTType.Image
            //}

            if (Device.RuntimePlatform == Device.UWP)
            {
                fileTypes = new string[] { ".nes", ".zip" };
            }

            if (Device.RuntimePlatform == Device.WPF)
            {
                fileTypes = new string[] { "NES files (*.nes)|*.nes", "ZIP files (*.zip)|*.zip" };
            }

            var file = await CrossFilePicker.Current.PickFile(fileTypes);
            
            LoadNewGameCartridge(file);
        }

        /// <summary>
        ///     Load game data from selected file
        /// </summary>
        /// <param name="file"></param>
        private void LoadNewGameCartridge(FileData file)
        {
            if (file != null)
            {
                if (file.FileName.EndsWith("nes", StringComparison.OrdinalIgnoreCase))
                {
                    byte[] romData = ReadStreamData(file.GetStream());
                    _emu.LoadRom(romData);
                }
                else if (file.FileName.EndsWith("zip", StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        ///     Convert file stream to byte array
        /// </summary>
        /// <param name="fileStream">File stream</param>
        /// <returns>ROM's byte array</returns>
        public static byte[] ReadStreamData(Stream fileStream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                fileStream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
