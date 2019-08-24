using XamariNES.Cartridge.Mappers.Enums;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.Cartridge.Mappers
{
    /// <summary>
    ///     Public Interface for XamariNES.Cartridge Mappers
    /// </summary>
    public interface IMapper
    {
        byte ReadByte(int offset);

        void WriteByte(int offset, byte data);

        void RegisterReadInterceptor(MapperBase.ReadInterceptor readInterceptor, int offset);

        void RegisterReadInterceptor(MapperBase.ReadInterceptor readInterceptor, int offsetStart, int offsetEnd);

        void RegisterWriteInterceptor(MapperBase.WriteInterceptor writeInterceptor, int offset);

        void RegisterWriteInterceptor(MapperBase.WriteInterceptor writeInterceptor, int offsetStart, int offsetEnd);

        enumNametableMirroring NametableMirroring { get; set; }
    }
}
