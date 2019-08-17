# XamariNES.CPU - MOS 6502 Processor

XamariNES.CPU is a cross-plaform MOS 6502 Emulator written in C# using .Net Standard. The CPU was the first component of the NES that I started with, as the project initially stated with me wanting to write a simple 8-bit emulated CPU. 
![](https://enusbaum-git.s3.amazonaws.com/XamariNES/xamarines.cpu_layout.png)

# Overview

The actual NES CPU itself was a Ricoh 2A03, which included a variation of the MOS 6502 processor [(More Information)](https://en.wikipedia.org/wiki/Ricoh_2A03).

This implementation of the 6502 not only supports the [official MOS 6502 opcodes](http://obelisk.me.uk/6502/reference.html), but also several of the [undocumented opcodes](https://wiki.nesdev.com/w/index.php/Programming_with_unofficial_opcodes) specific to the Ricoh 2A03 CPU mariant in the NES.

```
public int Tick()
```

Increments the CPU one **instruction**. A single instruction can take N cycles, so the return value represents the number of cycles passed. We need to know the number of cycles that pass for each CPU instruction so we can increment the PPU the correct number of cycles.

```
public void Reset()
```

Reset the CPU back to a startup state, clearing RAM and register values