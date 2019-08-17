using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class RTS_Tests
    {
        [TestMethod]
        public void RTS_Clear()
        {
            var mapper = new NROM(new byte[] {0x20, 0x05, 0xC0, 0xE8, 0x00, 0x60}, null);
            var cpu = new Core(mapper);

            //Test Program
            //JSR to 0x0005
            //RTS -- Return
            //INX -- Increment X

            cpu.Tick(3);

            //Verify Registers
            Assert.AreEqual(1, cpu.X);

            //Verify Cycles
            Assert.AreEqual(14u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Overflow);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
            Assert.AreEqual(false, cpu.Status.Zero);

            //Verify Stack
            Assert.AreEqual(0xFD, cpu.SP);

            //Verify Counters
            Assert.AreEqual(0xC004, cpu.PC);
        }
    }
}