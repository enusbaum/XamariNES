using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using XamariNES.Cartridge;
using XamariNES.Controller;
using XamariNES.Emulator.Enums;

namespace XamariNES.Emulator
{
    public class NESEmulator
    {

        //Frame Rendering Components
        public delegate void ProcessFrameDelegate(byte[] outputFrame);
        private readonly ProcessFrameDelegate _processFrame;

        //NES System Components
        ////private readonly CPU.Core _cpu;
        ////private readonly PPU.Core _ppu;
        private CPU.Core _cpu;
        private PPU.Core _ppu;
        private readonly NESCartridge _cartridge;
        public readonly IController Controller1;
        private readonly enumEmulatorSpeed _enumEmulatorSpeed;
        private Task _emulatorTask;
        private bool _powerOn;
        private byte[] _romData;

        //Public Statistics
        public long TotalCPUCycles => _cpu.Cycles;
        public long TotalPPUCycles => _ppu.Cycles;

        //Internal Statistics
        private int _cpuIdleCycles;

        public NESEmulator(byte[] rom, ProcessFrameDelegate processFrameDelegate, enumEmulatorSpeed emulatorSpeed = enumEmulatorSpeed.Normal)
        {
            _romData = rom;

            //Setup Emulator Components
            Controller1 = new NESController();
            _cartridge = new NESCartridge(rom);
            _ppu = new PPU.Core(_cartridge.MemoryMapper, DMATransfer);
            _cpu = new CPU.Core(_cartridge.MemoryMapper, Controller1);
            _enumEmulatorSpeed = emulatorSpeed;
            _processFrame = processFrameDelegate;
        }

        /// <summary>
        ///     Load new ROM into memory
        /// </summary>
        /// <param name="romData"></param>
        public void LoadRom(byte[] romData)
        {
            _romData = romData;
        }

        /// <summary>
        ///     News up and Starts the Emulator Task
        /// </summary>
        public void Start()
        {
            _cartridge.LoadROM(_romData);
            _ppu = new PPU.Core(_cartridge.MemoryMapper, DMATransfer);
            _cpu = new CPU.Core(_cartridge.MemoryMapper, Controller1);

            _cpu.Reset();
            _ppu.Reset();
            _powerOn = true;
            _emulatorTask = new TaskFactory().StartNew(Run, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        ///     Signals the Emulator Task to stop
        /// </summary>
        public void Stop() => _powerOn = false;

        /// <summary>
        ///     Delegate used to transfer information between CPU memory (typically CPU RAM) and the PPU OAM buffer
        ///
        ///     https://wiki.nesdev.com/w/index.php/PPU_registers#OAMDMA
        /// </summary>
        /// <param name="oam">OAM Memory -- always 256 bytes</param>
        /// <param name="oamOffset"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private byte[] DMATransfer(byte[] oam, int oamOffset, int offset)
        {
            for(var i = 0; i < 256; i++)
            {
                oam[(oamOffset + i) % 256] = _cpu.CPUMemory.ReadByte(offset + i);
            }

            /*
             *DMA Transfer consumes 513 cycles of the CPU, so we mark those cycles
             *as idle cycles, which basically means we'll keep ticking the PPU 3 times
             *for each idle CPU cycle, and the CPU will tick once the idle cycles are
             *passed
             *
             *If DMA occurs on an odd CPU cycle, it takes an extra cycle
             *Not even joking: https://wiki.nesdev.com/w/index.php/PPU_registers#OAMDMA
             */
            _cpuIdleCycles = 513;
            if (_cpu.Cycles % 2 == 1) _cpuIdleCycles++; 

            return oam;
        }

        /// <summary>
        ///     Method used to Run the Emulator Task
        ///
        ///     Task will run until the _powerOn value is set to false
        /// </summary>
        private void Run()
        {
            //Frame Timing Stopwatch
            var sw = Stopwatch.StartNew();

            //CPU startup state is always at 4 cycles
            _cpu.Cycles = 4;

            int cpuTicks;
            while (_powerOn)
            {
                //If we're not idling (DMA), tick the CPU
                if (_cpuIdleCycles == 0)
                {
                    cpuTicks = _cpu.Tick();
                }
                else
                {
                    //Otherwise, mark it as an idle cycle and carry on
                    _cpuIdleCycles--;
                    _cpu.Instruction.Cycles = 1;
                    _cpu.Cycles++;
                    cpuTicks = 1;
                }

                //Count how many cycles that instruction took and
                //execute that number of instruction * 3 for the PPU
                //We do ceiling since it's ok for the PPU to overshoot at this point
                for (var i = 0; i < cpuTicks * 3; i++)
                {
                    _ppu.Tick();
                }

                //If the PPU has signaled NMI, reset its status and signal the CPU
                if (_ppu.NMI)
                {
                    _ppu.NMI = false;
                    _cpu.NMI = true;
                }

                //Check to see if there's a frame in the PPU Frame Buffer
                //If there is, let's render it
                if (_ppu.FrameReady)
                {
                    _processFrame(_ppu.FrameBuffer);
                    _ppu.FrameReady = false;

                    //Throttle our frame rate here to the desired rate (if required)
                    switch (_enumEmulatorSpeed)
                    {
                        case enumEmulatorSpeed.Turbo:
                            continue;
                        case enumEmulatorSpeed.Normal when sw.ElapsedMilliseconds < 17:
                            Thread.Sleep((int)(17 - sw.ElapsedMilliseconds));
                            break;
                        case enumEmulatorSpeed.Half when sw.ElapsedMilliseconds < 32:
                            Thread.Sleep((int)(32 - sw.ElapsedMilliseconds));
                            break;
                    }
                    sw.Restart();
                }
            }
        }
    }
}