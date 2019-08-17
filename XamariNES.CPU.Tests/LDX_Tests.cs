using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class LDX_Tests
    {
        [TestMethod]
        public void LDX_Immediate_Clear()
        {
            var mapper = new NROM(new byte[] {0xA2, 0x01}, null);
            var cpu = new Core(mapper);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.X);
            Assert.AreEqual(0x01, cpu.X);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void LDX_Immediate_Negative()
        {
            var mapper = new NROM(new byte[] {0xA2, 0x80}, null);
            var cpu = new Core(mapper);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.X);
            Assert.AreEqual(0x80, cpu.X);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Negative);
        }

        [TestMethod]
        public void LDX_ZeroPage_Clear()
        {
            var mapper = new NROM(new byte[] {0xA6, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x01);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.X);
            Assert.AreEqual(0x01, cpu.X);

            //Verify Cycles
            Assert.AreEqual(3u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void LDX_ZeroPageY_Clear()
        {
            var mapper = new NROM(new byte[] {0xB6, 0x00}, null);
            var cpu = new Core(mapper) {Y = 1};
            cpu.CPUMemory.WriteByte(0x01, 0x01);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.X);
            Assert.AreEqual(0x01, cpu.X);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void LDX_Absolute_Clear()
        {
            var mapper = new NROM(new byte[] {0xAE, 0x03, 0xC0, 0x01}, null);
            var cpu = new Core(mapper);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.X);
            Assert.AreEqual(0x01, cpu.X);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }


        [TestMethod]
        public void LDX_AbsoluteY_Clear()
        {
            var mapper = new NROM(new byte[] {0xBE, 0x02, 0xC0, 0x01}, null);
            var cpu = new Core(mapper) {Y = 1};

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.X);
            Assert.AreEqual(0x01, cpu.X);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void LDX_AbsoluteY_PageBoundary_Clear()
        {
            var mapper = new NROM(new byte[] {0xBE, 0xFF, 0xC0}, null);
            var cpu = new Core(mapper) {Y = 1};
            cpu.CPUMemory.WriteByte(0xC100, 0x01);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.X);
            Assert.AreEqual(0x01, cpu.X);

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }
    }
}