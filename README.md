# XamariNES - A Cross-Platform NES Emulator using .Net!
![](http://forthebadge.com/images/badges/made-with-c-sharp.svg)
![](http://forthebadge.com/images/badges/60-percent-of-the-time-works-every-time.svg)

![](https://enusbaum-git.s3.amazonaws.com/XamariNES/xamarines_android.png)
![](https://enusbaum-git.s3.amazonaws.com/XamariNES/xamarines_ios.png)
![](https://enusbaum-git.s3.amazonaws.com/XamariNES/xamarines_uwp.png)

XamariNES is a cross-platform Nintendo Emulator using .Net Standard written in C#. This project started initially as a nighits/weekend project of mine to better understand the MOS 6502 processor in the original Nintendo Entertainment System. The CPU itself didn't take long working on it a couple hours here and there. I decided once the CPU was completed, how hard could it be just to take it to next step and do the PPU? Here we are a year later and I finally think I have the PPU in a semi-working state.

I've been a huge fan of the Microsoft Xamarin platform for many years now and wanted to utilize it as my UI layer for the emulator. This allowed me to write a fairly simple Xamarin.Forms project and quickly target multiple platforms (iOS, Android, macOS, and Windows) with only a couple platform specific lines of code. If you haven't used Xamarin or Xamarin.Forms, please use this solution to give it a whirl!

My goals for this project were simple:
- Write my fist emulated 8-bit CPU using C#
- Learn the inner workings of the MOS 6502 CPU powering the original Nintendo Entertainment System
- Structure the code to match the NES schematic as closely as possible
- Write the code in a way that made it readable and easier to understand for someone wanting to learn as I was
- Use .Net Standard & Microsoft Xamarin.Forms Framework to make the emulator cross-platform 

# Contribute!
There's still a TON of things missing from this emulator and areas of improvement which I just haven't had the time to get to yet.
- Performance Improvements
- Additional Cartridge Mappers
- Audio Support (for the brave)

# Solution Layout
Projects have a README.MD which expands on the internal functionality and layout of that project. I tried to make the solution layout similar to that of the PCB within an actual Nintendo. This is why XamariNES uses delegates as the interconnect between the PPU and the CPU (representing the PCB traces), which differs from other emulators that might pass the CPU & PPU obejcts into one another for reference. 

![](https://enusbaum-git.s3.amazonaws.com/XamariNES/xamrines_layout.png)

A brief summary of each project is as follows:
- **[XamariNES.Emulator](./XamariNES.Emulator/)** - The System
  - All components are integrated into this project, same as the PCB
  - External facing actons (Controller Input, ROM Loading) go through the Emulator
  - Handles clock synchronization/frame limiting of the CPU & PPU
  - Implements DMA Delegate to handle transfer of data between CPU & PPU
- **[XamariNES.CPU](./XamariNES.CPU/)** - MOS 6502 CPU
  - CPU Core
  - CPU Memory & Registers
  - Access to Cartridge Memory Mapper
- **[XamariNES.PPU](./XamariNES.PPU/)** - NES Picture Processing Unit
  - PPU Core
  - PPU Memory, Registers, and Latches
  - Access to Cartridge Memory Mapper
- **[XamariNES.Cartridge](./XamariNES.Cartridge/)** - NES Cartridge
  - Handles loading the specified ROM and setting up PRG/CHR banks
  - Exposes Memory Mapper used by CPU & PPU to access PRG/CHR memory
  - Interceptor Pattern to allow for hooks/delegates to be mapped to memory offsets
- **[XamariNES.Controller](./XamariNES.Controller/)** - NES Controller
  - Maintains and exposes controller button states
- **[XamariNES.UI.*platform*](./XamariNES.UI/)** - Xamarin.Forms projects for cross-platform UI
  - Xamarin.Forms project containing the UI logic for the emulator
  - Projects specific to their respective platforms (iOS, Android, Windows, macOS)

There are also a couple Common/Support/Testing projects which contain miscellaneous functionality.

# Thanks!
I wanted to put down some thank you's here for folks/projects/websites that were invaluable for helping me get this project into a functional state:

- [Nesdev wiki](http://wiki.nesdev.com/w/index.php/Nesdev_Wiki) - Wealth of detailed, accurate information on all aspects of the Nintendo Entertainment System
- [6502.org](http://www.6502.org/) - Great amount of documentation, errata and tools specific to the MOS 6502 processor. Their interactive simulator specifically was invaluable for debugging the CPU
- [Obelisk.me.uk](http://www.obelisk.me.uk/6502/reference.html) - Concise 6502 documentation, included a lot of it on my opcode documentation
- [NEScafé](https://github.com/GunshipPenguin/nescafe) Emulator by Rhys Rustad-Elliott - I literally would not have been able to untangle how the PPU works without this. Borrowed many of the same routines and logic around memory access and frame rendering

# License & Copyright

XamariNES is Copyright (c) 2019 Eric Nusbaum and is distributed under the MIT License. 