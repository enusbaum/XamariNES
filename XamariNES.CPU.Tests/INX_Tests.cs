using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class INX_Tests
    {
        [TestMethod]
        public void INX_Clear()
        {
            var mapper = new NROM(new byte[] {0xE8}, null);
            var cpu = new Core(mapper) {X = 0x01};

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x01, cpu.X);
            Assert.AreEqual(0x02, cpu.X);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void INX_Zero()
        {
            var mapper = new NROM(new byte[] {0xE8}, null);
            var cpu = new Core(mapper) {X = 0xFF};

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xFF, cpu.X);
            Assert.AreEqual(0x00, cpu.X);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void INX_Negative()
        {
            var mapper = new NROM(new byte[] {0xE8}, null);
            var cpu = new Core(mapper) {X = 0x7F};

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x7F, cpu.X);
            Assert.AreEqual(0x80, cpu.X);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Negative);
        }
    }
}