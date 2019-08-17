using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    /// <summary>
    ///     Unit Tests for AND Opcode
    ///     http://www.obelisk.me.uk/6502/reference.html#AND
    /// </summary>
    [TestClass]
    public class AND_Tests
    {
        [TestMethod]
        public void AND_Immediate()
        {
            var mapper = new NROM(new byte[] {0x29, 0x0A}, null);
            var cpu = new Core(mapper) {A = 0x7F};

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x7F, cpu.A);
            Assert.AreEqual(0x0A, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void AND_Immediate_Zero()
        {
            var mapper = new NROM(new byte[] {0x29, 0x05}, null);
            var cpu = new Core(mapper) {A = 0x0A};

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x0A, cpu.A);
            Assert.AreEqual(0x00, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void AND_Immediate_Negative()
        {
            var mapper = new NROM(new byte[] {0x29, 0x80}, null);
            var cpu = new Core(mapper) {A = 0xFF};

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x0A, cpu.A);
            Assert.AreEqual(0x80, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Negative);
        }
    }
}