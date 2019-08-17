using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class INY_Tests
    {
        [TestMethod]
        public void INY_Clear()
        {
            var mapper = new NROM(new byte[] {0xC8}, null);
            var cpu = new Core(mapper) {Y = 0x01};

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x01, cpu.Y);
            Assert.AreEqual(0x02, cpu.Y);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void INY_Zero()
        {
            var mapper = new NROM(new byte[] {0xC8}, null);
            var cpu = new Core(mapper) {Y = 0xFF};

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xFF, cpu.Y);
            Assert.AreEqual(0x00, cpu.Y);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void INY_Negative()
        {
            var mapper = new NROM(new byte[] {0xC8}, null);
            var cpu = new Core(mapper) {Y = 0x7F};

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x7F, cpu.Y);
            Assert.AreEqual(0x80, cpu.Y);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Negative);
        }
    }
}