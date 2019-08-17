using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class JSR_Tests
    {
        [TestMethod]
        public void JSR_Clear()
        {
            var mapper = new NROM(new byte[] {0x20, 0x00, 0xC1}, null);
            var cpu = new Core(mapper);

            cpu.Tick();

            //Verify Register Values
            Assert.AreNotEqual(0xC003, cpu.PC);
            Assert.AreEqual(0xC100, cpu.PC);

            //Verify Stack Values
            Assert.AreEqual(0xC002,
                BitConverter.ToUInt16(
                    new[]
                    {
                        cpu.CPUMemory.ReadByte(Core.STACK_BASE + cpu.SP + 1),
                        cpu.CPUMemory.ReadByte(Core.STACK_BASE + cpu.SP + 2)
                    }, 0));
        }
    }
}