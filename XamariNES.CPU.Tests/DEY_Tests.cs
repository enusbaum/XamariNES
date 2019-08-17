using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class DEY_Tests
    {
        [TestMethod]
        public void DEY_Zero()
        {
            var mapper = new NROM(new byte[] {0x88}, null);
            var cpu = new Core(mapper) {Y = 1};
            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x01, cpu.Y);
            Assert.AreEqual(0x00, cpu.Y);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void DEY_Negative()
        {
            var mapper = new NROM(new byte[] {0x88}, null);
            var cpu = new Core(mapper) {Y = 0};

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x00, cpu.Y);
            Assert.AreEqual(0xFF, cpu.Y);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Negative);
        }
    }
}