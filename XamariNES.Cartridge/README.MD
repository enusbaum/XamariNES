# XamariNES.Cartridge - Nintendo Cartridge (iNES ROM)

This project contains all the code required to load and work with an iNES format ROM. More information on the iNES file format can be found [here](https://wiki.nesdev.com/w/index.php/INES).

# General Information

The NES cartridge traditionally contains two types of data: PRG ROM and CHR ROM

The PRG ROM is connected directly to the CPU, and CHR ROM is connected directly to the PPU. Some cartridges also contain CHR RAM which is another way for the CPU to copy data to the PPU, but we'll cover that elsewhere.

XamariNES uses the Mapper as a central memory controller for access to any memory outside of the individual component. Because of this, calls to memory addresses within the PPU and CPU that are outside of their own internal buffers are sent to the Mapper and handled there.

# Mappers

The NES PPU is limited to only being able to address 8KiB of tile data at once, so to work around this limitation games began using special chips called Mappers which allow the cartridge to expose different pages of memory within the same address space. This means if the CPU writes a specified value to a specific address in its memory space, the cartridge would bankswitch and load a completely different 8KiB set of data into the PPU address space.

To support this, the iNES ROM format specifies which Mapper a given ROM is to use when loaded.

Currently Supported Mappers in XamariNES:

| Mapper # | Name | Supported | Number of Games |ROM List |
| ------ | ----------- | ----------- | ----------- | ----------- |
| 0 | [NROM](https://wiki.nesdev.com/w/index.php/NROM) | YES | 119 | [List](http://bootgod.dyndns.org:7777/search.php?ines=0&group=groupid)
| 1 | [MMC1](https://wiki.nesdev.com/w/index.php/MMC1) | YES | 391 | [List](http://bootgod.dyndns.org:7777/search.php?ines=1&group=groupid)
| 2 | [UxROM](https://wiki.nesdev.com/w/index.php/UxROM) | YES | 156 | [List](http://bootgod.dyndns.org:7777/search.php?ines=2&group=groupid)
| 3 | [CNROM](https://wiki.nesdev.com/w/index.php/CNROM) | YES | 105 | [List](http://bootgod.dyndns.org:7777/search.php?ines=3&group=groupid)
| 4 | [MMC3](https://wiki.nesdev.com/w/index.php/MMC3) | NO | 303 | [List](http://bootgod.dyndns.org:7777/search.php?ines=4&group=groupid)

Full list of iNES Mappers: [Link](https://wiki.nesdev.com/w/index.php/Mapper)

# Interceptors

The XamariNES Cartridge implements an Interceptor Pattern in the addressable memory space to support the ability for writes/reads from a specific address in memory to invoke the specified delegate.

```
//Interceptor Delegates
public delegate byte ReadInterceptor(int offset);
public delegate void WriteInterceptor(int offset, byte value);
```

Delegates used to define methods which will be registered as an Interceptor

```
void RegisterReadInterceptor(MapperBase.ReadInterceptor readInterceptor, int offset);
void RegisterReadInterceptor(MapperBase.ReadInterceptor readInterceptor, int offsetStart, int offsetEnd);
void RegisterWriteInterceptor(MapperBase.WriteInterceptor writeInterceptor, int offset);
void RegisterWriteInterceptor(MapperBase.WriteInterceptor writeInterceptor, int offsetStart, int offsetEnd);
```

Methods to register Interceptors to the designated memory offset, or offset range