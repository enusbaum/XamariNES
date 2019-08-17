using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class BNE_Tests
    {
        [TestMethod]
        public void BNE_ZeroClear()
        {
            var mapper = new NROM(new byte[] {0xD0, 0x0A, 0x00}, null);
            var cpu = new Core(mapper);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0xC000, cpu.PC);
            Assert.AreEqual(0xC00C, cpu.PC);

            //Verify Cycles
            Assert.AreEqual(3u, cpu.Cycles);

            //Verify Flags
            //Not Modified
        }

        [TestMethod]
        public void BNE_Zero()
        {
            var mapper = new NROM(new byte[] {0xD0, 0x0A, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.Status.Zero = true;

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0xC000, cpu.PC);
            Assert.AreEqual(0xC002, cpu.PC);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            //Not Modified
        }

        [TestMethod]
        public void BNE_ZeroClear_PageBoundary()
        {
            var mapper = new NROM(new byte[] {0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0xC0F0, 0xD0);
            cpu.CPUMemory.WriteByte(0xC0F1, 0x79);
            cpu.CPUMemory.WriteByte(0xC0F2, 0x00);
            cpu.PC = 0xC0F0;

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0xC0F0, cpu.PC);
            Assert.AreEqual(0xC16B, cpu.PC); //0xF0 + 0x79 + 2 bytes for instruction

            //Verify Cycles
            Assert.AreEqual(4u, cpu.Cycles);

            //Verify Flags
            //Not Modified
        }
    }
}