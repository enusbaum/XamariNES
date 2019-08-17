using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class BVC_Tests
    {
        [TestMethod]
        public void BVC_Overflow()
        {
            var mapper = new NROM(new byte[] {0x50, 0x0A, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.Status.Overflow = true;

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0xC000, cpu.PC);
            Assert.AreEqual(0xC002, cpu.PC);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Overflow);
        }

        [TestMethod]
        public void BVC_OverflowClear()
        {
            var mapper = new NROM(new byte[] {0x50, 0x0A, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.Status.Overflow = false;

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0xC000, cpu.PC);
            Assert.AreEqual(0xC00C, cpu.PC);

            //Verify Cycles
            Assert.AreEqual(3u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Overflow);
        }

        [TestMethod]
        public void BVC_OverflowClear_PageBoundary()
        {
            var mapper = new NROM(new byte[] {0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0xC0F0, 0x50);
            cpu.CPUMemory.WriteByte(0xC0F1, 0x79);
            cpu.CPUMemory.WriteByte(0xC0F2, 0x00);
            cpu.Status.Overflow = false;
            cpu.PC = 0xC0F0;

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0xC000, cpu.PC);
            Assert.AreEqual(0xC16B, cpu.PC); //0xF0 + 0x79 + 2 bytes for instruction

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Overflow);
        }
    }
}