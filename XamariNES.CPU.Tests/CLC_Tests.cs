using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class CLC_Tests
    {
        [TestMethod]
        public void CLC_Carry()
        {
            var mapper = new NROM(new byte[] {0x18}, null);
            var cpu = new Core(mapper);
            cpu.Status.Carry = true;

            cpu.Tick();

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Carry);
        }
    }
}