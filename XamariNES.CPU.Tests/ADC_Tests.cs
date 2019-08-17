using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    /// <summary>
    ///     Unit Tests for ADC Opcode
    ///     http://www.obelisk.me.uk/6502/reference.html#ADC
    /// </summary>
    [TestClass]
    public class ADC_Tests
    {
        [TestMethod]
        public void ADC_Immediate()
        {
            var mapper = new NROM(new byte[] {0x69, 0x01}, null);
            var memory = new Memory(mapper, null);
            var cpu = new Core(mapper);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x00, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void ADC_Immediate_Carry()
        {
            var mapper = new NROM(new byte[] {0x69, 0x02}, null);
            var memory = new Memory(mapper, null);
            var cpu = new Core(mapper) {A = 0xFF};
            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xff, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void ADC_Immediate_Carry_Zero()
        {
            var mapper = new NROM(new byte[] {0x69, 0x81}, null);
            var memory = new Memory(mapper, null);
            var cpu = new Core(mapper) {A = 0x7F};

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x7f, cpu.A);
            Assert.AreEqual(0x00, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void ADC_ZeroPage()
        {
            var mapper = new NROM(new byte[] {0x65, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x01);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x00, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(3u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void ADC_ZeroPageX()
        {
            var mapper = new NROM(new byte[] {0x75, 0x00}, null);
            var cpu = new Core(mapper) {X = 1};
            cpu.CPUMemory.WriteByte(0x01, 0x01);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x00, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void ADC_Absolute()
        {
            var mapper = new NROM(new byte[] {0x6D, 0x03, 0xC0, 0x01}, null);
            var cpu = new Core(mapper);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x00, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void ADC_AbsoluteX()
        {
            var mapper = new NROM(new byte[] {0x7D, 0x02, 0xC0, 0x01}, null);
            var cpu = new Core(mapper) {A = 0, X = 1};

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x00, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void ADC_AbsoluteX_PageBoundary()
        {
            var mapper = new NROM(new byte[] {0x7D, 0x01, 0x00}, null);
            var cpu = new Core(mapper) {A = 0, X = 0xFF};
            cpu.CPUMemory.WriteByte(0x0100, 0x01);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x00, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void ADC_AbsoluteY()
        {
            var mapper = new NROM(new byte[] {0x79, 0x02, 0xC0, 0x01}, null);
            var cpu = new Core(mapper) {A = 0, Y = 1};

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x00, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void ADC_AbsoluteY_PageBoundary()
        {
            var mapper = new NROM(new byte[] {0x79, 0x01, 0x00}, null);
            var cpu = new Core(mapper) {A = 0, Y = 0xFF};
            cpu.CPUMemory.WriteByte(0x100, 0x01);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x00, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void ADC_IndexedIndirect()
        {
            var mapper = new NROM(new byte[] {0x61, 0x00}, null);
            var cpu = new Core(mapper) {A = 0, X = 1};
            cpu.CPUMemory.WriteByte(0x00, 0x00);
            cpu.CPUMemory.WriteByte(0x01, 0x03);
            cpu.CPUMemory.WriteByte(0x02, 0x00);
            cpu.CPUMemory.WriteByte(0x03, 0x01);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x00, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void ADC_IndirectIndexed()
        {
            var mapper = new NROM(new byte[] {0x71, 0x00}, null);
            var cpu = new Core(mapper) {A = 0, Y = 1};
            cpu.CPUMemory.WriteByte(0x00, 0x01);
            cpu.CPUMemory.WriteByte(0x01, 0x00);
            cpu.CPUMemory.WriteByte(0x02, 0x01);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x00, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void ADC_IndirectIndexed_PageBoundary()
        {
            var mapper = new NROM(new byte[] {0x71, 0x00}, null);
            var cpu = new Core(mapper) {A = 0, Y = 0x01};
            cpu.CPUMemory.WriteByte(0x00, 0xFF);
            cpu.CPUMemory.WriteByte(0x01, 0x00);
            cpu.CPUMemory.WriteByte(0x100, 0x01);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x00, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }
    }
}