using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    /// <summary>
    ///     Unit tests for ASL Opcode
    ///     http://www.obelisk.me.uk/6502/reference.html#ASL
    /// </summary>
    [TestClass]
    public class ASL_Tests
    {
        [TestMethod]
        public void ASL_ZeroPage_Negative()
        {
            var mapper = new NROM(new byte[] {0x06, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x7F);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x7F, cpu.CPUMemory.ReadByte(0x00));
            Assert.AreEqual(0xFE, cpu.CPUMemory.ReadByte(0x00));

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(true, cpu.Status.Negative);
        }

        [TestMethod]
        public void ASL_ZeroPage_Carry_Zero()
        {
            var mapper = new NROM(new byte[] {0x06, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x80);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x80, cpu.CPUMemory.ReadByte(0x00));
            Assert.AreEqual(0x00, cpu.CPUMemory.ReadByte(0x00));

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void ASL_ZeroPage()
        {
            var mapper = new NROM(new byte[] {0x06, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x01);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x01, cpu.CPUMemory.ReadByte(0x00));
            Assert.AreEqual(0x02, cpu.CPUMemory.ReadByte(0x00));

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void ASL_ZeroPageX()
        {
            var mapper = new NROM(new byte[] {0x16, 0x00}, null);
            var cpu = new Core(mapper) {X = 1};
            cpu.CPUMemory.WriteByte(0x01, 0x01);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x01, cpu.CPUMemory.ReadByte(0x01));
            Assert.AreEqual(0x02, cpu.CPUMemory.ReadByte(0x01));

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void ASL_Absolute()
        {
            var mapper = new NROM(new byte[] {0x0E, 0x00, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x01);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x01, cpu.CPUMemory.ReadByte(0x00));
            Assert.AreEqual(0x02, cpu.CPUMemory.ReadByte(0x00));

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void ASL_AbsoluteX()
        {
            var mapper = new NROM(new byte[] {0x1E, 0x00, 0x00}, null);
            var cpu = new Core(mapper) { X = 1};
            cpu.CPUMemory.WriteByte(0x01, 0x01);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x01, cpu.CPUMemory.ReadByte(0x01));
            Assert.AreEqual(0x02, cpu.CPUMemory.ReadByte(0x01));

            //Verify Cycles
            Assert.AreEqual(7u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }
    }
}