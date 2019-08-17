using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class LDY_Tests
    {
        [TestMethod]
        public void LDY_Immediate_Clear()
        {
            var mapper = new NROM(new byte[] {0xA0, 0x01}, null);
            var cpu = new Core(mapper);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.Y);
            Assert.AreEqual(0x01, cpu.Y);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void LDY_Immediate_Negative()
        {
            var mapper = new NROM(new byte[] {0xA0, 0x80}, null);
            var cpu = new Core(mapper);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.Y);
            Assert.AreEqual(0x80, cpu.Y);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Negative);
        }

        [TestMethod]
        public void LDY_ZeroPage_Clear()
        {
            var mapper = new NROM(new byte[] {0xA4, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x01);
            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.Y);
            Assert.AreEqual(0x01, cpu.Y);

            //Verify Cycles
            Assert.AreEqual(3u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void LDY_ZeroPageX_Clear()
        {
            var mapper = new NROM(new byte[] {0xB4, 0x00}, null);
            var cpu = new Core(mapper) {X = 1};
            cpu.CPUMemory.WriteByte(0x01, 0x01);
            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.Y);
            Assert.AreEqual(0x01, cpu.Y);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void LDY_Absolute_Clear()
        {
            var mapper = new NROM(new byte[] {0xAC, 0x03, 0xC0, 0x01}, null);
            var cpu = new Core(mapper);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.Y);
            Assert.AreEqual(0x01, cpu.Y);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }


        [TestMethod]
        public void LDY_AbsoluteX_Clear()
        {
            var mapper = new NROM(new byte[] {0xBC, 0x02, 0xC0, 0x01}, null);
            var cpu = new Core(mapper) {X = 1};

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.Y);
            Assert.AreEqual(0x01, cpu.Y);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void LDY_AbsoluteX_PageBoundary_Clear()
        {
            var mapper = new NROM(new byte[] {0xBC, 0xFF, 0xC0}, null);
            var cpu = new Core(mapper) {X = 1};
            cpu.CPUMemory.WriteByte(0xC100, 0x01);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.Y);
            Assert.AreEqual(0x01, cpu.Y);

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }
    }
}