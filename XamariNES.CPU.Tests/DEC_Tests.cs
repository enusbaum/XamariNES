using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class DEC_Tests
    {
        [TestMethod]
        public void DEC_ZeroPage()
        {
            var mapper = new NROM(new byte[] {0xC6, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x02);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x02, cpu.CPUMemory.ReadByte(0x00));
            Assert.AreEqual(0x01, cpu.CPUMemory.ReadByte(0x00));

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void DEC_ZeroPage_Zero()
        {
            var mapper = new NROM(new byte[] {0xC6, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x01);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x01, cpu.CPUMemory.ReadByte(0x00));
            Assert.AreEqual(0x00, cpu.CPUMemory.ReadByte(0x00));

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void DEC_ZeroPage_Negative()
        {
            var mapper = new NROM(new byte[] {0xC6, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x00);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x00, cpu.CPUMemory.ReadByte(0x00));
            Assert.AreEqual(0xFF, cpu.CPUMemory.ReadByte(0x00));

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(true, cpu.Status.Negative);
        }

        [TestMethod]
        public void DEC_ZeroPageX()
        {
            var mapper = new NROM(new byte[] {0xD6, 0x00}, null);
            var cpu = new Core(mapper) {X = 0x01};
            cpu.CPUMemory.WriteByte(0x01, 0x02);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x02, cpu.CPUMemory.ReadByte(0x01));
            Assert.AreEqual(0x01, cpu.CPUMemory.ReadByte(0x01));

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void DEC_Absolute()
        {
            var mapper = new NROM(new byte[] {0xCE, 0x03, 0xC0, 0x02}, null);
            var cpu = new Core(mapper) {X = 0x01};

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x02, cpu.CPUMemory.ReadByte(0xC003));
            Assert.AreEqual(0x01, cpu.CPUMemory.ReadByte(0xC003));

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }

        [TestMethod]
        public void DEC_AbsoluteX()
        {
            var mapper = new NROM(new byte[] {0xDE, 0x02, 0xC0, 0x02}, null);
            var cpu = new Core(mapper) {X = 0x01};

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x02, cpu.CPUMemory.ReadByte(0xC003));
            Assert.AreEqual(0x01, cpu.CPUMemory.ReadByte(0xC003));

            //Verify Cycles
            Assert.AreEqual(7u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
        }
    }
}