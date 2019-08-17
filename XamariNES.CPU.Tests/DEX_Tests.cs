using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class DEX_Tests
    {
        [TestMethod]
        public void DEX_Zero()
        {
            var mapper = new NROM(new byte[] {0xCA}, null);
            var cpu = new Core(mapper) {X = 1};

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x01, cpu.X);
            Assert.AreEqual(0x00, cpu.X);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void DEX_Negative()
        {
            var mapper = new NROM(new byte[] {0xCA}, null);
            var cpu = new Core(mapper) {X = 0};

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x00, cpu.X);
            Assert.AreEqual(0xFF, cpu.X);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Negative);
        }
    }
}