using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class PHA_Tests
    {
        [TestMethod]
        public void PHA_Implied()
        {
            var mapper = new NROM(new byte[] {0x48}, null);
            var cpu = new Core(mapper) {A = 0x01};

            cpu.Tick();

            //Verify Registers
            //Not Modified

            //Verify Cycles
            Assert.AreEqual(3u, cpu.Cycles);

            //Verify Flags
            //Not Modified

            //Verify Stack
            Assert.AreEqual(0x01, cpu.CPUMemory.ReadByte(Core.STACK_BASE + cpu.SP + 1));
        }
    }
}