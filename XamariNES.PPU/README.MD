# XamariNES.PPU - The Nintendo Picture Processing unit

What. A. Nightmare.

I first started this project in early 2018 and my goal was to just successfully emulate MOS 6502 processor within the Nintendo Entertainment System. As an occasional nights & weekend project that took only a month or so.

After completing the CPU and feeling proud of myself, I thought to myself:

>Hey, since I've already got a working CPU, how hard could it be to get the PPU working as well?

... a year later ...

So here it is. My implementation of the NES PPU using .Net Standard and C#. If there is any performance to be gained around optimizing for mobile devices that don't have enough horsepower, it's going to be in the here PPU.

# General Information

The Nintendo PPU renders a 256x240 8-bit image at 60fps, writing 3 pixels per CPU cycle to the Frame Buffer.

More Info: [PPU Rendering](https://wiki.nesdev.com/w/index.php/PPU_rendering)

```
public void Tick()
```
Advances the PPU one cycle, updating internal counters, memory, and rendering a pixel to the Frame Buffer (if a visible pixel/cycle)

```
public bool FrameReady
```
This value is set to TRUE when there is a fully rendered frame ready to be drawn in the frame buffer. 

```
public byte[] FrameBuffer
```
Byte array that contains a frame to be rendered. The data is an 8-bit bitmap format where each byte corresponds to the NES color palette.

More Info: [PPU Palettes](https://wiki.nesdev.com/w/index.php/PPU_palettes)

# PPU Registers

In the constructor, we use the Cartridge Mapper Interceptor Pattern to register interceptors at the memory offsets for the PPU Registers ($2000->$2007, $4014)

This allows the CPU to read data from and write data to the PPU registers.

More Info: [PPU Registers](https://wiki.nesdev.com/w/index.php/PPU_registers)

# Thanks
Success doesn't happen in a vacuum and I owe thanks to those who came before me:
- [Emulator.NES](https://github.com/Xyene/Emulator.NES) - Emulator by Tudor Brindus
- [NEScafé](https://github.com/GunshipPenguin/nescafe) - Emulator by Rhys Rustad-Elliott - I literally would not have been able to untangle how the PPU works without this. Borrowed many of the same routines and logic around memory access and frame rendering