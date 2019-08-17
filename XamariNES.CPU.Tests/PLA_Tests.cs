using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class PLA_Tests
    {
        [TestMethod]
        public void PLA_Implied()
        {
            //Test Program
            // PHA -- Pushes value of 1 to stack
            // LDA 0x00 -- Loads 0x00 into the accumulator
            // PLA -- Pops the value of 1 from stack to accumulator
            var mapper = new NROM(new byte[] { 0x48, 0xA9, 0x00, 0x68 }, null);
            var cpu = new Core(mapper) { A = 0x01 };
            
            var test = new byte[] {  };

            cpu.Tick(3);

            //Verify Registers
            //Not Modified

            //Verify Cycles
            Assert.AreEqual(9u, cpu.Cycles);

            //Verify Flags
            //Not Modified

            //Verify Stack
            Assert.AreEqual(0x01, cpu.A);
        }
    }
}
