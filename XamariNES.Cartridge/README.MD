# XamariNES.Cartridge - Nintendo Cartridge (iNES ROM)

This project contains all the code required to load and work with an iNES format ROM. More information on the iNES file format can be found [here](https://wiki.nesdev.com/w/index.php/INES).


# General Information

The NES cartridge traditionally contains two types of data: PRG ROM and CHR ROM

The PRG ROM is connected directly to the CPU, and CHR ROM is connected directly to the PPU. Some cartridges also contain CHR RAM which is another way for the CPU to copy data to the PPU, but we'll cover that elsewhere.

# Mappers

The NES PPU is limited to only being able to address 8KiB of tile data at once, so to work around this limitation games began using special chips called Mappers which allow the cartridge to expose different pages of memory within the same address space. This means if the CPU writes a specified value to a specific address in its memory space, the cartridge would bankswitch and load a completely different 8KiB set of data into the PPU address space.

To support this, the iNES ROM format specifies which Mapper a given ROM is to use when loaded.

Currently Supported Mappers in XamariNES:

| Mapper | Supported | ROM List |
| ------ | ----------- |----------- |
| [NROM](https://wiki.nesdev.com/w/index.php/NROM)   | YES | [List](http://bootgod.dyndns.org:7777/search.php?unif_op=LIKE+`%25%40%25`&unif=-NROM)

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