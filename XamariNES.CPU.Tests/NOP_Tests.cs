using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class NOP_Tests
    {
        [TestMethod]
        public void NOP_Implied()
        {
            var mapper = new NROM(new byte[] { 0xEA }, null);
            var cpu = new Core(mapper);

            cpu.Tick();

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);
        }
    }
}
