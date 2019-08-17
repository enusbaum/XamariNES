using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class INC_Tests
    {
        [TestMethod]
        public void INC_ZeroPage_Zero()
        {
            var mapper = new NROM(new byte[] {0xE6, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0xFF);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0xFF, cpu.CPUMemory.ReadByte(0x00));
            Assert.AreEqual(0x00, cpu.CPUMemory.ReadByte(0x00));

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void INC_ZeroPage_Negative()
        {
            var mapper = new NROM(new byte[] {0xE6, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x7F);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x7F, cpu.CPUMemory.ReadByte(0x00));
            Assert.AreEqual(0x80, cpu.CPUMemory.ReadByte(0x00));

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(true, cpu.Status.Negative);
        }

        [TestMethod]
        public void INC_ZeroPageX_Zero()
        {
            var mapper = new NROM(new byte[] {0xF6, 0x00}, null);
            var cpu = new Core(mapper) {X = 1};
            cpu.CPUMemory.WriteByte(0x01, 0xFF);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0xFF, cpu.CPUMemory.ReadByte(0x01));
            Assert.AreEqual(0x00, cpu.CPUMemory.ReadByte(0x01));

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void INC_Absolute_Zero()
        {
            var mapper = new NROM(new byte[] {0xEE, 0x03, 0xC0, 0xFF}, null);
            var cpu = new Core(mapper);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0xFF, cpu.CPUMemory.ReadByte(0xC003));
            Assert.AreEqual(0x00, cpu.CPUMemory.ReadByte(0xC003));

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void INC_AbsoluteX_Zero()
        {
            var mapper = new NROM(new byte[] {0xFE, 0x03, 0xC0, 0x00, 0xFF}, null);
            var cpu = new Core(mapper) {X = 1};

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0xFF, cpu.CPUMemory.ReadByte(0xC004));
            Assert.AreEqual(0x00, cpu.CPUMemory.ReadByte(0xC004));

            //Verify Cycles
            Assert.AreEqual(7u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
        }
    }
}