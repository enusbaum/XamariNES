using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class CLD_Tests
    {
        [TestMethod]
        public void CLD_Decimal()
        {
            var mapper = new NROM(new byte[] {0xD8}, null);
            var cpu = new Core(mapper);
            cpu.Status.DecimalMode = true;

            cpu.Tick();

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.DecimalMode);
        }
    }
}