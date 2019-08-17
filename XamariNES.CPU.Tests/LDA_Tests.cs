using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class LDA_Tests
    {
        [TestMethod]
        public void LDA_Immediate_Clear()
        {
            var mapper = new NROM(new byte[] {0xA9, 0x01}, null);
            var cpu = new Core(mapper);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void LDA_Immediate_Negative()
        {
            var mapper = new NROM(new byte[] {0xA9, 0x80}, null);
            var cpu = new Core(mapper);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.A);
            Assert.AreEqual(0x80, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Negative);
        }

        [TestMethod]
        public void LDA_ZeroPage_Clear()
        {
            var mapper = new NROM(new byte[] {0xA5, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x01);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(3u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void LDA_ZeroPageX_Clear()
        {
            var mapper = new NROM(new byte[] {0xB5, 0x00}, null);
            var cpu = new Core(mapper) {X = 1};
            cpu.CPUMemory.WriteByte(0x01, 0x01);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void LDA_Absolute_Clear()
        {
            var mapper = new NROM(new byte[] {0xAD, 0x03, 0xC0, 0x01}, null);
            var cpu = new Core(mapper);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void LDA_AbsoluteX_Clear()
        {
            var mapper = new NROM(new byte[] {0xBD, 0x02, 0xC0, 0x01}, null);
            var cpu = new Core(mapper) {X = 1};

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void LDA_AbsoluteX_PageBoundary_Clear()
        {
            var mapper = new NROM(new byte[] {0xBD, 0xFF, 0xC0}, null);
            var cpu = new Core(mapper) {X = 1};
            cpu.CPUMemory.WriteByte(0xC100, 0x01);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void LDA_AbsoluteY_Clear()
        {
            var mapper = new NROM(new byte[] {0xB9, 0x02, 0xC0, 0x01}, null);
            var cpu = new Core(mapper) {Y = 1};

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void LDA_AbsoluteY_PageBoundary_Clear()
        {
            var mapper = new NROM(new byte[] {0xB9, 0xFF, 0xC0}, null);
            var cpu = new Core(mapper) {Y = 1};
            cpu.CPUMemory.WriteByte(0xC100, 0x01);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void LDA_IndexedIndirect_Clear()
        {
            var mapper = new NROM(new byte[] {0xA1, 0x01, 0xC0, 0x00}, null);
            var cpu = new Core(mapper) {X = 1};
            cpu.CPUMemory.WriteByte(0x00, 0x01);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void LDA_IndirectIndexed_Clear()
        {
            var mapper = new NROM(new byte[] {0xB1, 0x00}, null);
            var cpu = new Core(mapper) {Y = 1};
            cpu.CPUMemory.WriteByte(0x00, 0x01);
            cpu.CPUMemory.WriteByte(0x01, 0x00);
            cpu.CPUMemory.WriteByte(0x02, 0x01);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void LDA_IndirectIndexed_PageBoundary_Clear()
        {
            var mapper = new NROM(new byte[] {0xB1, 0x00}, null);
            var cpu = new Core(mapper) {Y = 1};
            cpu.CPUMemory.WriteByte(0x00, 0xFF);
            cpu.CPUMemory.WriteByte(0x01, 0x00);
            cpu.CPUMemory.WriteByte(0x100, 0x01);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void LDA_IndirectIndexed_BigY_PageBoundary_Clear()
        {
            var mapper = new NROM(new byte[] { 0xB1, 0x00 }, null);
            var cpu = new Core(mapper) { Y = 3 };
            cpu.CPUMemory.WriteByte(0x00, 0xFE);
            cpu.CPUMemory.WriteByte(0x01, 0x00);
            cpu.CPUMemory.WriteByte(0x101, 0x01);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }
    }
}