using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    /// <summary>
    ///     Unit tests for BCS Opcode
    ///     http://www.obelisk.me.uk/6502/reference.html#BCC
    /// </summary>
    [TestClass]
    public class BCS_Tests
    {
        [TestMethod]
        public void BCS_Carry()
        {
            var mapper = new NROM(new byte[] {0xB0, 0x0A, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.Status.Carry = false;

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0xC000, cpu.PC);
            Assert.AreEqual(0xC002, cpu.PC);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void BCS_CarryClear()
        {
            var mapper = new NROM(new byte[] {0xB0, 0x0A, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.Status.Carry = true;

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0xC000, cpu.PC);
            Assert.AreEqual(0xC00C, cpu.PC);

            //Verify Cycles
            Assert.AreEqual(3u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void BCS_CarryClear_PageBoundary()
        {
            var mapper = new NROM(new byte[] {0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00F0, 0xB0);
            cpu.CPUMemory.WriteByte(0x00F1, 0x79);
            cpu.CPUMemory.WriteByte(0x00F2, 0x00);
            cpu.Status.Carry = true;
            cpu.PC = 0xF0;

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.PC);
            Assert.AreEqual(0x16B, cpu.PC); //0xF0 + 0x79 + 2 bytes for instruction

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }
    }
}