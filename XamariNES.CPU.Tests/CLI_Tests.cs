using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class CLI_Tests
    {
        [TestMethod]
        public void CLI_Interrupt()
        {
            var mapper = new NROM(new byte[] {0x58}, null);
            var cpu = new Core(mapper);
            cpu.Status.InterruptDisable = true;

            cpu.Tick();

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.InterruptDisable);
        }
    }
}