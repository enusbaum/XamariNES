using Microsoft.VisualStudio.TestTools.UnitTesting;
using XamariNES.Cartridge.Mappers.impl;

namespace XamariNES.CPU.Tests
{
    [TestClass]
    public class ROL_Tests
    {
        [TestMethod]
        public void ROL_Accumulator_Zero()
        {
            var mapper = new NROM(new byte[] {0x2A}, null);
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
        public void ROL_Accumulator_Negative()
        {
            var mapper = new NROM(new byte[] {0x2A}, null);
            var cpu = new Core(mapper) {A = 0x7F};
            cpu.Status.Carry = false;

            cpu.Tick();

            //Verify Registers
            Assert.AreEqual(0xFE, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(true, cpu.Status.Negative);

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROL_Accumulator_SetCarry()
        {
            var mapper = new NROM(new byte[] {0x2A}, null);
            var cpu = new Core(mapper) {A = 0xFF};
            cpu.Status.Carry = false;

            cpu.Tick();

            //Verify Registers
            Assert.AreEqual(0xFE, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(true, cpu.Status.Negative);

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROL_Accumulator_ApplyCarry()
        {
            var mapper = new NROM(new byte[] {0x2A}, null);
            var cpu = new Core(mapper) {A = 0xFF};
            cpu.Status.Carry = true;

            cpu.Tick();

            //Verify Registers
            Assert.AreEqual(0xFF, cpu.A);

            //Verify Cycles
            Assert.AreEqual(2u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(true, cpu.Status.Negative);

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROL_ZeroPage_Clear()
        {
            var mapper = new NROM(new byte[] {0x26, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x3F);

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);

            //Verify Memory Values
            Assert.AreEqual(0x7E, cpu.CPUMemory.ReadByte(0x00));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROL_ZeroPage_SetCarry()
        {
            var mapper = new NROM(new byte[] {0x26, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0xBF);

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);

            //Verify Memory Values
            Assert.AreEqual(0x7E, cpu.CPUMemory.ReadByte(0x00));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROL_ZeroPage_SetNegative()
        {
            var mapper = new NROM(new byte[] {0x26, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0x7F);

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(true, cpu.Status.Negative);

            //Verify Memory Values
            Assert.AreEqual(0xFE, cpu.CPUMemory.ReadByte(0x00));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROL_ZeroPage_ApplyCarry()
        {
            var mapper = new NROM(new byte[] {0x26, 0x00}, null);
            var cpu = new Core(mapper);
            cpu.CPUMemory.WriteByte(0x00, 0xFF);
            cpu.Status.Carry = true;

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(5u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(true, cpu.Status.Negative);

            //Verify Memory Values
            Assert.AreEqual(0xFF, cpu.CPUMemory.ReadByte(0x00));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROL_ZeroPageX_Clear()
        {
            var mapper = new NROM(new byte[] {0x36, 0x00}, null);
            var cpu = new Core(mapper) {X = 1};
            cpu.CPUMemory.WriteByte(0x01, 0x3F);

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);

            //Verify Memory Values
            Assert.AreEqual(0x7E, cpu.CPUMemory.ReadByte(0x01));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROL_ZeroPageX_SetCarry()
        {
            var mapper = new NROM(new byte[] {0x36, 0x00}, null);
            var cpu = new Core(mapper) {X = 1};
            cpu.CPUMemory.WriteByte(0x01, 0xBF);

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);

            //Verify Memory Values
            Assert.AreEqual(0x7E, cpu.CPUMemory.ReadByte(0x01));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROL_ZeroPageX_SetNegative()
        {
            var mapper = new NROM(new byte[] {0x36, 0x00}, null);
            var cpu = new Core(mapper) {X = 1};
            cpu.CPUMemory.WriteByte(0x01, 0x7F);

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(true, cpu.Status.Negative);

            //Verify Memory Values
            Assert.AreEqual(0xFE, cpu.CPUMemory.ReadByte(0x01));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROL_ZeroPageX_ApplyCarry()
        {
            var mapper = new NROM(new byte[] {0x36, 0x00}, null);
            var cpu = new Core(mapper) {X = 1};
            cpu.CPUMemory.WriteByte(0x01, 0xFF);
            cpu.Status.Carry = true;

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(true, cpu.Status.Negative);

            //Verify Memory Values
            Assert.AreEqual(0xFF, cpu.CPUMemory.ReadByte(0x01));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROL_Absolute_Clear()
        {
            var mapper = new NROM(new byte[] {0x2E, 0x03, 0xC0, 0x3F}, null);
            var cpu = new Core(mapper);

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);

            //Verify Memory Values
            Assert.AreEqual(0x7E, cpu.CPUMemory.ReadByte(0xC003));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROL_Absolute_SetCarry()
        {
            var mapper = new NROM(new byte[] {0x2E, 0x03, 0xC0, 0xBF}, null);
            var cpu = new Core(mapper);

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);

            //Verify Memory Values
            Assert.AreEqual(0x7E, cpu.CPUMemory.ReadByte(0xC003));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROL_Absolute_SetNegative()
        {
            var mapper = new NROM(new byte[] {0x2E, 0x03, 0xC0, 0x7F}, null);
            var cpu = new Core(mapper);

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(true, cpu.Status.Negative);

            //Verify Memory Values
            Assert.AreEqual(0xFE, cpu.CPUMemory.ReadByte(0xC003));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROL_Absolute_ApplyCarry()
        {
            var mapper = new NROM(new byte[] {0x2E, 0x03, 0xC0, 0xFF}, null);
            var cpu = new Core(mapper);
            cpu.Status.Carry = true;

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(6u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(true, cpu.Status.Negative);

            //Verify Memory Values
            Assert.AreEqual(0xFF, cpu.CPUMemory.ReadByte(0xC003));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROL_AbsoluteX_Clear()
        {
            var mapper = new NROM(new byte[] {0x3E, 0x02, 0xC0, 0x3F}, null);
            var cpu = new Core(mapper) {X = 1};

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(7u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);

            //Verify Memory Values
            Assert.AreEqual(0x7E, cpu.CPUMemory.ReadByte(0xC003));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROL_AbsoluteX_SetCarry()
        {
            var mapper = new NROM(new byte[] {0x3E, 0x02, 0xC0, 0xBF}, null);
            var cpu = new Core(mapper) {X = 1};

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(7u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(false, cpu.Status.Negative);

            //Verify Memory Values
            Assert.AreEqual(0x7E, cpu.CPUMemory.ReadByte(0xC003));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROL_AbsoluteX_SetNegative()
        {
            var mapper = new NROM(new byte[] {0x3E, 0x02, 0xC0, 0x7F}, null);
            var cpu = new Core(mapper) {X = 1};

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(7u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(false, cpu.Status.Carry);
            Assert.AreEqual(true, cpu.Status.Negative);

            //Verify Memory Values
            Assert.AreEqual(0xFE, cpu.CPUMemory.ReadByte(0xC003));

            //Verify Stack
            //Not modified
        }

        [TestMethod]
        public void ROL_AbsoluteX_ApplyCarry()
        {
            var mapper = new NROM(new byte[] {0x3E, 0x02, 0xC0, 0xFF}, null);
            var cpu = new Core(mapper) {X = 1};
            cpu.Status.Carry = true;

            cpu.Tick();

            //Verify Registers
            //Not modified

            //Verify Cycles
            Assert.AreEqual(7u, cpu.Cycles);

            //Verify Flags
            Assert.AreEqual(true, cpu.Status.Carry);
            Assert.AreEqual(true, cpu.Status.Negative);

            //Verify Memory Values
            Assert.AreEqual(0xFF, cpu.CPUMemory.ReadByte(0xC003));

            //Verify Stack
            //Not modified
        }
    }
}