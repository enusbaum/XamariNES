using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class ROR_Tests
    {
        [TestMethod]
        public void ROR_Accumulator_Zero()
        {
            var mapper = new NROM(new byte[] {0x6A}, null);
            var cpu = new Core(mapper) {A = 0x00};

            cpu.Tick();

            //Verify Registers
            Assert.AreEqual(0x00, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
            Assert.AreEqual(true, cpu.Status.Zero);

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROR_Accumulator_Negative_ApplyCarry()
        {
            var mapper = new NROM(new byte[] {0x6A}, null);
            var cpu = new Core(mapper) {A = 0x00};
            cpu.Status.Carry = true;

            cpu.Tick();

            //Verify Registers
            Assert.AreEqual(0x80, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(true, cpu.Status.Negative);
            Assert.AreEqual(false, cpu.Status.Zero);

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROR_Accumulator_SetCarry()
        {
            var mapper = new NROM(new byte[] {0x6A}, null);
            var cpu = new Core(mapper) {A = 0x01};
            cpu.Status.Carry = false;

            cpu.Tick();

            //Verify Registers
            Assert.AreEqual(0x00, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
            Assert.AreEqual(true, cpu.Status.Zero);

            //Verify Stack
            //Not modified
        }


        [TestMethod]
        public void ROR_ZeroPage_Clear()
        {
            var mapper = new NROM(new byte[] {0x66, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x02);

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
            Assert.AreEqual(false, cpu.Status.Zero);

            //Verify Memory Values
            Assert.AreEqual(0x01, cpu.CPUMemory.ReadByte(0x00));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROR_ZeroPage_SetZero()
        {
            var mapper = new NROM(new byte[] {0x66, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x00);

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
            Assert.AreEqual(true, cpu.Status.Zero);

            //Verify Memory Values
            Assert.AreEqual(0x00, cpu.CPUMemory.ReadByte(0x00));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROR_ZeroPage_SetCarry()
        {
            var mapper = new NROM(new byte[] {0x66, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x03);

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
            Assert.AreEqual(false, cpu.Status.Zero);

            //Verify Memory Values
            Assert.AreEqual(0x01, cpu.CPUMemory.ReadByte(0x00));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROR_ZeroPage_SetNegative_ApplyCarry()
        {
            var mapper = new NROM(new byte[] {0x66, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x00);
            cpu.Status.Carry = true;

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(true, cpu.Status.Negative);
            Assert.AreEqual(false, cpu.Status.Zero);

            //Verify Memory Values
            Assert.AreEqual(0x80, cpu.CPUMemory.ReadByte(0x00));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROR_ZeroPageX_Clear()
        {
            var mapper = new NROM(new byte[] {0x76, 0x00}, null);
            var cpu = new Core(mapper) {X = 1};
            cpu.CPUMemory.WriteByte(0x01, 0x02);

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
            Assert.AreEqual(false, cpu.Status.Zero);

            //Verify Memory Values
            Assert.AreEqual(0x01, cpu.CPUMemory.ReadByte(0x01));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROR_ZeroPageX_SetCarry()
        {
            var mapper = new NROM(new byte[] {0x76, 0x00}, null);
            var cpu = new Core(mapper) {X = 1};
            cpu.CPUMemory.WriteByte(0x01, 0x03);

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
            Assert.AreEqual(false, cpu.Status.Zero);

            //Verify Memory Values
            Assert.AreEqual(0x01, cpu.CPUMemory.ReadByte(0x01));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROR_ZeroPageX_SetNegative_ApplyCarry()
        {
            var mapper = new NROM(new byte[] {0x76, 0x00}, null);
            var cpu = new Core(mapper) {X = 1};
            cpu.CPUMemory.WriteByte(0x01, 0x00);
            cpu.Status.Carry = true;

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(true, cpu.Status.Negative);
            Assert.AreEqual(false, cpu.Status.Zero);

            //Verify Memory Values
            Assert.AreEqual(0x80, cpu.CPUMemory.ReadByte(0x01));

            //Verify Stack
            //Not modified
        }


        [TestMethod]
        public void ROR_Absolute_Clear()
        {
            var mapper = new NROM(new byte[] {0x6E, 0x03, 0xC0, 0x02}, null);
            var cpu = new Core(mapper);

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
            Assert.AreEqual(false, cpu.Status.Zero);

            //Verify Memory Values
            Assert.AreEqual(0x01, cpu.CPUMemory.ReadByte(0xC003));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROR_Absolute_SetCarry()
        {
            var mapper = new NROM(new byte[] {0x6E, 0x03, 0xC0, 0x03}, null);
            var cpu = new Core(mapper);

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
            Assert.AreEqual(false, cpu.Status.Zero);

            //Verify Memory Values
            Assert.AreEqual(0x01, cpu.CPUMemory.ReadByte(0xC003));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROR_Absolute_SetNegative_ApplyCarry()
        {
            var mapper = new NROM(new byte[] {0x6E, 0x03, 0xC0, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.Status.Carry = true;

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(true, cpu.Status.Negative);
            Assert.AreEqual(false, cpu.Status.Zero);

            //Verify Memory Values
            Assert.AreEqual(0x80, cpu.CPUMemory.ReadByte(0xC003));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROR_AbsoluteX_Clear()
        {
            var mapper = new NROM(new byte[] {0x7E, 0x02, 0xC0, 0x02}, null);
            var cpu = new Core(mapper) {X = 1};

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(7u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
            Assert.AreEqual(false, cpu.Status.Zero);

            //Verify Memory Values
            Assert.AreEqual(0x01, cpu.CPUMemory.ReadByte(0xC003));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROR_AbsoluteX_SetCarry()
        {
            var mapper = new NROM(new byte[] {0x7E, 0x02, 0xC0, 0x03}, null);
            var cpu = new Core(mapper) {X = 1};

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(7u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);
            Assert.AreEqual(false, cpu.Status.Zero);

            //Verify Memory Values
            Assert.AreEqual(0x01, cpu.CPUMemory.ReadByte(0xC003));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROR_AbsoluteX_SetNegative_ApplyCarry()
        {
            var mapper = new NROM(new byte[] {0x7E, 0x02, 0xC0, 0x00}, null);
            var cpu = new Core(mapper) {X = 1};
            cpu.Status.Carry = true;

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(7u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(true, cpu.Status.Negative);
            Assert.AreEqual(false, cpu.Status.Zero);

            //Verify Memory Values
            Assert.AreEqual(0x80, cpu.CPUMemory.ReadByte(0xC003));

            //Verify Stack
            //Not modified
        }
    }
}