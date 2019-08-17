using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class EOR_Tests
    {
        [TestMethod]
        public void EOR_Immediate_Zero()
        {
            var mapper = new NROM(new byte[] {0x49, 0xFF}, null);
            var cpu = new Core(mapper) {A = 0xFF};

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xFF, cpu.A);
            Assert.AreEqual(0x00, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void EOR_Immediate_Negative()
        {
            var mapper = new NROM(new byte[] {0x49, 0x7F}, null);
            var cpu = new Core(mapper) {A = 0xFF};

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xFF, cpu.A);
            Assert.AreEqual(0x80, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Negative);
        }

        [TestMethod]
        public void EOR_ZeroPage_Zero()
        {
            var mapper = new NROM(new byte[] {0x45, 0x00}, null);
            var cpu = new Core(mapper) {A = 0xFF};
            cpu.CPUMemory.WriteByte(0x00, 0xFF);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xFF, cpu.A);
            Assert.AreEqual(0x00, cpu.A);

            //Verify Cycles
            Assert.AreEqual(3u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void EOR_ZeroPageX_Zero()
        {
            var mapper = new NROM(new byte[] {0x55, 0x00}, null);
            var cpu = new Core(mapper) {A = 0xFF, X = 1};
            cpu.CPUMemory.WriteByte(0x01, 0xFF);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xFF, cpu.A);
            Assert.AreEqual(0x00, cpu.A);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void EOR_Absolute_Zero()
        {
            var mapper = new NROM(new byte[] {0x4D, 0x03, 0xC0, 0xFF}, null);
            var cpu = new Core(mapper) {A = 0xFF};
            cpu.CPUMemory.WriteByte(0x01, 0xFF);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xFF, cpu.A);
            Assert.AreEqual(0x00, cpu.A);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void EOR_AbsoluteX_Zero()
        {
            var mapper = new NROM(new byte[] {0x5D, 0x02, 0xC0, 0xFF}, null);
            var cpu = new Core(mapper) {A = 0xFF, X = 1};

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xFF, cpu.A);
            Assert.AreEqual(0x00, cpu.A);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void EOR_AbsoluteX_PageBoundary_Zero()
        {
            var mapper = new NROM(new byte[] {0x5D, 0x01, 0xC0}, null);
            var cpu = new Core(mapper) {A = 0xFF, X = 0xFF};
            cpu.CPUMemory.WriteByte(0xC100, 0xFF);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xFF, cpu.A);
            Assert.AreEqual(0x00, cpu.A);

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void EOR_AbsoluteY_Zero()
        {
            var mapper = new NROM(new byte[] {0x59, 0x02, 0xC0, 0xFF}, null);
            var cpu = new Core(mapper) {A = 0xFF, Y = 1};

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xFF, cpu.A);
            Assert.AreEqual(0x00, cpu.A);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void EOR_AbsoluteY_PageBoundary_Zero()
        {
            var mapper = new NROM(new byte[] {0x59, 0x01, 0xC0}, null);
            var cpu = new Core(mapper) {A = 0xFF, Y = 0xFF};
            cpu.CPUMemory.WriteByte(0xC100, 0xFF);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xFF, cpu.A);
            Assert.AreEqual(0x00, cpu.A);

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void EOR_IndexedIndirect_Zero()
        {
            var mapper = new NROM(new byte[] {0x41, 0x00}, null);
            var cpu = new Core(mapper) {A = 0xFF, X = 1};
            cpu.CPUMemory.WriteByte(0x00, 0x00);
            cpu.CPUMemory.WriteByte(0x01, 0x03);
            cpu.CPUMemory.WriteByte(0x02, 0x00);
            cpu.CPUMemory.WriteByte(0x03, 0xFF);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xFF, cpu.A);
            Assert.AreEqual(0x00, cpu.A);

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void EOR_IndirectIndexed_Zero()
        {
            var mapper = new NROM(new byte[] {0x51, 0x00}, null);
            var cpu = new Core(mapper) {A = 0xFF, Y = 1};
            cpu.CPUMemory.WriteByte(0x00, 0x01);
            cpu.CPUMemory.WriteByte(0x01, 0x00);
            cpu.CPUMemory.WriteByte(0x02, 0xFF);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xFF, cpu.A);
            Assert.AreEqual(0x00, cpu.A);

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void EOR_IndirectIndexed_PageBoundary_Zero()
        {
            var mapper = new NROM(new byte[] {0x51, 0x00}, null);
            var cpu = new Core(mapper) {A = 0xFF, Y = 1};
            cpu.CPUMemory.WriteByte(0x00, 0xFF);
            cpu.CPUMemory.WriteByte(0x01, 0x00);
            cpu.CPUMemory.WriteByte(0x100, 0xFF);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xFF, cpu.A);
            Assert.AreEqual(0x00, cpu.A);

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }
    }
}