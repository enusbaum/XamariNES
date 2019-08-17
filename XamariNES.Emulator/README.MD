# XamariNES.Emulator - The XamariNES Emulator

The best way to think of this project is to view it as the PCB of the Nintendo itself.

Hardware components such as the CPU, PPU, Controller, and Cartridge are all wired to/through the Emulator. The Emulator handles the timings both for CPU->PPU ratio, as well as frame timing on how fast XamariNES renders frames (Half, Normal or **TURBO**).

# Overview

Unless prototyping with one specific component of XamariNES, most projects will only require a reference to this project.

```
public delegate void ProcessFrameDelegate(byte[] outputFrame);
```

Delegate passed into the constructor which is used to process frames as they become available.

```
public void Start()
```
Starts the Emulator Task

```
public void Stop()
```
Signals the Emulator Task to Stop