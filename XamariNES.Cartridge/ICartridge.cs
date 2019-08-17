using XamariNES.Cartridge.Mappers;

namespace XamariNES.Cartridge
{
    /// <summary>
    ///     Public Interface for XamariNES Cartridge
    /// </summary>
    public interface ICartridge
    {
        bool LoadROM(byte[] ROM);
        IMapper MemoryMapper { get; set; }
    }
}
