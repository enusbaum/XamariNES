using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class CMP_Tests
    {
        [TestMethod]
        public void CMP_Immediate_Carry()
        {
            var mapper = new NROM(new byte[] {0xC9, 0x01}, null);
            var cpu = new Core(mapper) {A = 0x7F};

            cpu.Tick();

            //Verify Register Values
            Assert.AreEqual(0x7F, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void CMP_Immediate_Negative()
        {
            var mapper = new NROM(new byte[] {0xC9, 0x80}, null);
            var cpu = new Core(mapper) {A = 0x7E};

            cpu.Tick();

            //Verify Register Values
            Assert.AreEqual(0x7E, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(true, cpu.Status.Negative);
        }

        [TestMethod]
        public void CMP_Immediate_Carry_Zero()
        {
            var mapper = new NROM(new byte[] {0xC9, 0x7E}, null);
            var cpu = new Core(mapper) {A = 0x7E};

            cpu.Tick();

            //Verify Register Values
            Assert.AreEqual(0x7E, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void CMP_ZeroPage_Carry_Zero()
        {
            var mapper = new NROM(new byte[] {0xC5, 0x00}, null);
            var cpu = new Core(mapper) {A = 0x7E};
            cpu.CPUMemory.WriteByte(0x00, 0x7E);

            cpu.Tick();

            //Verify Register Values
            Assert.AreEqual(0x7E, cpu.A);

            //Verify Cycles
            Assert.AreEqual(3u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void CMP_ZeroPageX_Carry_Zero()
        {
            var mapper = new NROM(new byte[] {0xD5, 0x00}, null);
            var cpu = new Core(mapper) {A = 0x7E, X = 1};
            cpu.CPUMemory.WriteByte(0x01, 0x7E);

            cpu.Tick();

            //Verify Register Values
            Assert.AreEqual(0x7E, cpu.A);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void CMP_Absolute_Carry_Zero()
        {
            var mapper = new NROM(new byte[] {0xCD, 0x03, 0xC0, 0x7E}, null);
            var cpu = new Core(mapper) {A = 0x7E};

            cpu.Tick();

            //Verify Register Values
            Assert.AreEqual(0x7E, cpu.A);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void CMP_AbsoluteX_Carry_Zero()
        {
            var mapper = new NROM(new byte[] {0xDD, 0x03, 0xC0, 0x00, 0x7E}, null);
            var cpu = new Core(mapper) {A = 0x7E, X = 1};

            cpu.Tick();

            //Verify Register Values
            Assert.AreEqual(0x7E, cpu.A);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void CMP_AbsoluteX_PageBoundary_Carry_Zero()
        {
            var mapper = new NROM(new byte[] {0xDD, 0xFF, 0xC0}, null);
            var cpu = new Core(mapper) {A = 0x7E, X = 1};
            cpu.CPUMemory.WriteByte(0xC100, 0x7E);

            cpu.Tick();

            //Verify Register Values
            Assert.AreEqual(0x7E, cpu.A);

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void CMP_AbsoluteY_Carry_Zero()
        {
            var mapper = new NROM(new byte[] {0xD9, 0x03, 0xC0, 0x00, 0x7E}, null);
            var cpu = new Core(mapper) {A = 0x7E, Y = 1};

            cpu.Tick();

            //Verify Register Values
            Assert.AreEqual(0x7E, cpu.A);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void CMP_AbsoluteY_PageBoundary_Carry_Zero()
        {
            var mapper = new NROM(new byte[] {0xD9, 0xFF, 0xC0}, null);
            var cpu = new Core(mapper) {A = 0x7E, Y = 1};
            cpu.CPUMemory.WriteByte(0xC100, 0x7E);

            cpu.Tick();

            //Verify Register Values
            Assert.AreEqual(0x7E, cpu.A);

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void CMP_IndexedIndirect_Carry_Zero()
        {
            var mapper = new NROM(new byte[] {0xC1, 0x00}, null);
            var cpu = new Core(mapper) {A = 0x7E, X = 1};
            cpu.CPUMemory.WriteByte(0x00, 0x00);
            cpu.CPUMemory.WriteByte(0x01, 0x03);
            cpu.CPUMemory.WriteByte(0x02, 0x00);
            cpu.CPUMemory.WriteByte(0x03, 0x7E);

            cpu.Tick();

            //Verify Register Values
            Assert.AreEqual(0x7E, cpu.A);

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void CMP_IndirectIndexed_Carry_Zero()
        {
            var mapper = new NROM(new byte[] {0xD1, 0x00}, null);

            var cpu = new Core(mapper) {A = 0x7E, Y = 1};
            cpu.CPUMemory.WriteByte(0x00, 0x01);
            cpu.CPUMemory.WriteByte(0x01, 0x00);
            cpu.CPUMemory.WriteByte(0x02, 0x7E);

            cpu.Tick();

            //Verify Register Values
            Assert.AreEqual(0x7E, cpu.A);

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void CMP_IndirectIndexed_PageBoundary_Carry_Zero()
        {
            var mapper = new NROM(new byte[] {0xD1, 0x00}, null);
            var cpu = new Core(mapper) {A = 0x7E, Y = 1};
            cpu.CPUMemory.WriteByte(0x00, 0xFF);
            cpu.CPUMemory.WriteByte(0x01, 0x00);
            cpu.CPUMemory.WriteByte(0x100, 0x7E);

            cpu.Tick();

            //Verify Register Values
            Assert.AreEqual(0x7E, cpu.A);

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }
    }
}