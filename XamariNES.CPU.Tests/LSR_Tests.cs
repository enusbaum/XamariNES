using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class LSR_Tests
    {
        [TestMethod]
        public void LSR_Accumulator_Clear()
        {
            var mapper = new NROM(new byte[] {0x4A}, null);
            var cpu = new Core(mapper) {A = 0x80};

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x80, cpu.A);
            Assert.AreEqual(0x40, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
            Assert.AreEqual(false, cpu.Status.Carry);
        }

        [TestMethod]
        public void LSR_Accumulator_Carry()
        {
            var mapper = new NROM(new byte[] {0x4A}, null);
            var cpu = new Core(mapper) {A = 0x03};

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x03, cpu.A);
            Assert.AreEqual(0x01, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
            Assert.AreEqual(true, cpu.Status.Carry);
        }

        [TestMethod]
        public void LSR_Accumulator_Zero_Carry()
        {
            var mapper = new NROM(new byte[] {0x4A}, null);
            var cpu = new Core(mapper) {A = 0x01};

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x01, cpu.A);
            Assert.AreEqual(0x00, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
            Assert.AreEqual(true, cpu.Status.Carry);
        }

        [TestMethod]
        public void LSR_ZeroPage_Clear()
        {
            var mapper = new NROM(new byte[] {0x46, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x80);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x80, cpu.CPUMemory.ReadByte(0x00));
            Assert.AreEqual(0x40, cpu.CPUMemory.ReadByte(0x00));

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
            Assert.AreEqual(false, cpu.Status.Carry);
        }

        [TestMethod]
        public void LSR_ZeroPage_Carry()
        {
            var mapper = new NROM(new byte[] {0x46, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x03);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x03, cpu.CPUMemory.ReadByte(0x00));
            Assert.AreEqual(0x01, cpu.CPUMemory.ReadByte(0x00));

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
            Assert.AreEqual(true, cpu.Status.Carry);
        }

        [TestMethod]
        public void LSR_ZeroPage_Zero_Carry()
        {
            var mapper = new NROM(new byte[] {0x46, 0x00}, null);
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
            Assert.AreEqual(false, cpu.Status.Negative);
            Assert.AreEqual(true, cpu.Status.Carry);
        }

        [TestMethod]
        public void LSR_ZeroPageX_Clear()
        {
            var mapper = new NROM(new byte[] {0x56, 0x00}, null);

            var cpu = new Core(mapper) {X = 1};
            cpu.CPUMemory.WriteByte(0x01, 0x80);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x80, cpu.CPUMemory.ReadByte(0x01));
            Assert.AreEqual(0x40, cpu.CPUMemory.ReadByte(0x01));

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
            Assert.AreEqual(false, cpu.Status.Carry);
        }

        [TestMethod]
        public void LSR_Absolute_Clear()
        {
            var mapper = new NROM(new byte[] {0x4E, 0x03, 0xC0, 0x80}, null);
            var cpu = new Core(mapper);

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x80, cpu.CPUMemory.ReadByte(0xC003));
            Assert.AreEqual(0x40, cpu.CPUMemory.ReadByte(0xC003));

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
            Assert.AreEqual(false, cpu.Status.Carry);
        }

        [TestMethod]
        public void LSR_AbsoluteX_Clear()
        {
            var mapper = new NROM(new byte[] {0x5E, 0x02, 0xC0, 0x80}, null);
            var cpu = new Core(mapper) {X = 1};

            cpu.Tick();

            //Verify Memory Values
            Assert.AreNotEqual(0x80, cpu.CPUMemory.ReadByte(0xC003));
            Assert.AreEqual(0x40, cpu.CPUMemory.ReadByte(0xC003));

            //Verify Cycles
            Assert.AreEqual(7u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Zero);
            Assert.AreEqual(false, cpu.Status.Negative);
            Assert.AreEqual(false, cpu.Status.Carry);
        }
    }
}