using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class ORA_Tests
    {
        [TestMethod]
        public void ORA_Immediate_Clear()
        {
            var mapper = new NROM(new byte[] {0x09, 0x55}, null);
            var cpu = new Core(mapper) {A = 0x2A};

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x2A, cpu.A);
            Assert.AreEqual(0x7F, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void ORA_Immediate_Zero()
        {
            var mapper = new NROM(new byte[] {0x09, 0x00}, null);
            var cpu = new Core(mapper) {A = 0x00};

            cpu.Tick();

            //Verify Register Values
            Assert.AreEqual(0x00, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void ORA_Immediate_Negative()
        {
            var mapper = new NROM(new byte[] {0x09, 0x55}, null);
            var cpu = new Core(mapper) {A = 0xAA};

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xAA, cpu.A);
            Assert.AreEqual(0xFF, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Negative);
        }

        [TestMethod]
        public void ORA_ZeroPage_Negative()
        {
            var mapper = new NROM(new byte[] {0x05, 0x00}, null);
            var cpu = new Core(mapper) {A = 0xAA};
            cpu.CPUMemory.WriteByte(0x00, 0x55);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xAA, cpu.A);
            Assert.AreEqual(0xFF, cpu.A);

            //Verify Cycles
            Assert.AreEqual(3u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Negative);
        }

        [TestMethod]
        public void ORA_ZeroPageX_Negative()
        {
            var mapper = new NROM(new byte[] {0x15, 0x00}, null);
            var cpu = new Core(mapper) {A = 0xAA, X = 1};
            cpu.CPUMemory.WriteByte(0x01, 0x55);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xAA, cpu.A);
            Assert.AreEqual(0xFF, cpu.A);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Negative);
        }

        [TestMethod]
        public void ORA_Absolute_Negative()
        {
            var mapper = new NROM(new byte[] {0x0D, 0x03, 0xC0, 0x55}, null);
            var cpu = new Core(mapper) {A = 0xAA};

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xAA, cpu.A);
            Assert.AreEqual(0xFF, cpu.A);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Negative);
        }

        [TestMethod]
        public void ORA_AbsoluteX_Negative()
        {
            var mapper = new NROM(new byte[] {0x1D, 0x02, 0xC0, 0x55}, null);
            var cpu = new Core(mapper) {A = 0xAA, X = 1};

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xAA, cpu.A);
            Assert.AreEqual(0xFF, cpu.A);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Negative);
        }

        [TestMethod]
        public void ORA_AbsoluteX_PageBoundary_Negative()
        {
            var mapper = new NROM(new byte[] {0x1D, 0x01, 0xC0}, null);
            var cpu = new Core(mapper) {A = 0xAA, X = 0xFF};
            cpu.CPUMemory.WriteByte(0xC100, 0x55);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xAA, cpu.A);
            Assert.AreEqual(0xFF, cpu.A);

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Negative);
        }

        [TestMethod]
        public void ORA_AbsoluteY_Negative()
        {
            var mapper = new NROM(new byte[] {0x19, 0x02, 0xC0, 0x55}, null);
            var cpu = new Core(mapper) {A = 0xAA, Y = 1};

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xAA, cpu.A);
            Assert.AreEqual(0xFF, cpu.A);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Negative);
        }

        [TestMethod]
        public void ORA_AbsoluteY_PageBoundary_Negative()
        {
            var mapper = new NROM(new byte[] {0x19, 0x01, 0xC0}, null);
            var cpu = new Core(mapper) {A = 0xAA, Y = 0xFF};
            cpu.CPUMemory.WriteByte(0xC100, 0x55);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xAA, cpu.A);
            Assert.AreEqual(0xFF, cpu.A);

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Negative);
        }

        [TestMethod]
        public void ORA_IndexedIndirect_Negative()
        {
            var mapper = new NROM(new byte[] {0x01, 0x00}, null);
            var cpu = new Core(mapper) {A = 0xAA, X = 1};
            cpu.CPUMemory.WriteByte(0x01, 0x03);
            cpu.CPUMemory.WriteByte(0x02, 0x00);
            cpu.CPUMemory.WriteByte(0x03, 0x55);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xAA, cpu.A);
            Assert.AreEqual(0xFF, cpu.A);

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Negative);
        }

        [TestMethod]
        public void ORA_IndirectIndexed_Negative()
        {
            var mapper = new NROM(new byte[] {0x11, 0x00}, null);
            var cpu = new Core(mapper) {A = 0xAA, Y = 1};
            cpu.CPUMemory.WriteByte(0x00, 0x01);
            cpu.CPUMemory.WriteByte(0x01, 0x00);
            cpu.CPUMemory.WriteByte(0x02, 0x55);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xAA, cpu.A);
            Assert.AreEqual(0xFF, cpu.A);

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Negative);
        }

        [TestMethod]
        public void ORA_IndirectIndexed_PageBoundary_Negative()
        {
            var mapper = new NROM(new byte[] {0x51, 0x00}, null);
            var cpu = new Core(mapper) {A = 0xAA, Y = 1};
            cpu.CPUMemory.WriteByte(0x00, 0xFF);
            cpu.CPUMemory.WriteByte(0x100, 0x55);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xAA, cpu.A);
            Assert.AreEqual(0xFF, cpu.A);

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Negative);
        }
    }
}