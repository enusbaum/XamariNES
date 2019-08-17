using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class BIT_Tests
    {
        [TestMethod]
        public void BIT_ZeroPage_ZeroClear_NegativeClear_OverflowClear()
        {
            var mapper = new NROM(new byte[] {0x24, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x01);
            cpu.Status.Zero = true;
            cpu.Status.Negative = true;
            cpu.Status.Overflow = true;
            cpu.A = 0x7F;

            cpu.Tick();

            //Verify Cycles
            Assert.AreEqual(3u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Overflow);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void BIT_ZeroPage_ZeroClear_NegativeClear_Overflow()
        {
            var mapper = new NROM(new byte[] {0x24, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x7F);
            cpu.Status.Zero = true;
            cpu.Status.Negative = true;
            cpu.Status.Overflow = true;
            cpu.A = 0x7F;

            cpu.Tick();

            //Verify Cycles
            Assert.AreEqual(3u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Overflow);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void BIT_ZeroPage_Zero_NegativeClear_OverflowClear()
        {
            var mapper = new NROM(new byte[] {0x24, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x00);
            cpu.Status.Zero = true;
            cpu.Status.Negative = true;
            cpu.Status.Overflow = true;
            cpu.A = 0x7F;

            cpu.Tick();

            //Verify Cycles
            Assert.AreEqual(3u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Overflow);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void BIT_ZeroPage_ZeroClear_Negative_OverflowClear()
        {
            var mapper = new NROM(new byte[] {0x24, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x80);

            cpu.Status.Zero = true;
            cpu.Status.Negative = true;
            cpu.Status.Overflow = true;
            cpu.A = 0x80;

            cpu.Tick();

            //Verify Cycles
            Assert.AreEqual(3u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Overflow);
            Assert.AreEqual(true, cpu.Status.Negative);
        }

        [TestMethod]
        public void BIT_Absolute_ZeroClear_NegativeClear_OverflowClear()
        {
            var mapper = new NROM(new byte[] {0x2C, 0x00, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x01);
            cpu.Status.Zero = true;
            cpu.Status.Negative = true;
            cpu.Status.Overflow = true;
            cpu.A = 0x7F;

            cpu.Tick();

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Overflow);
            Assert.AreEqual(false, cpu.Status.Negative);
        }
    }
}