using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class JMP_Tests
    {
        [TestMethod]
        public void JMP_Absolute()
        {
            var mapper = new NROM(new byte[] {0x4C, 0x0F, 0xC0}, null);
            var cpu = new Core(mapper);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xC000, cpu.PC);
            Assert.AreEqual(0xC00F, cpu.PC);

            //Verify Cycles
            Assert.AreEqual(3u, cpu.Cycles);
        }

        [TestMethod]
        public void JMP_Indirect()
        {
            var mapper = new NROM(new byte[] {0x6C, 0x03, 0xC0, 0x0F, 0xC0}, null);
            var cpu = new Core(mapper);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xC000, cpu.PC);
            Assert.AreEqual(0xC00F, cpu.PC);

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);
        }

        [TestMethod]
        public void JMP_Indirect_PageBoundaryBug()
        {
            var mapper = new NROM(new byte[] {0x6C, 0xFF, 0xC0}, null);
            var cpu = new Core(mapper);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0x00, cpu.PC);
            Assert.AreEqual(0x6C00, cpu.PC);

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);
        }
    }
}