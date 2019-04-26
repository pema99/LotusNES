using System;

namespace Lotus6502
{
    [Serializable]
    public class CPU
    {
        //Status bitflag bits
        private const byte StatusCarryFlag = 0;
        private const byte StatusZeroFlag = 1;
        private const byte StatusInterruptDisable = 2;
        private const byte StatusDecimalModeFlag = 3;
        private const byte StatusBreakCommand = 4;
        private const byte StatusUnused = 5;
        private const byte StatusOverflowFlag = 6;
        private const byte StatusNegativeFlag = 7;

        //Interrupt vectors
        private const ushort NMIAddress = 0xFFFA;
        private const ushort RSTAddress = 0xFFFC;
        private const ushort BRKAddress = 0xFFFE;

        #region Instruction Data
        private delegate void Instruction(ushort address, int addressMode);
        private readonly Instruction[] Instructions;

        private delegate ushort addressMode();
        private readonly addressMode[] AddressModes;

        private readonly int[] InstructionAddressMode = new int[256]
        {
            5, 6, 5, 6, 10, 10, 10, 10, 5, 4, 3, 4, 0, 0, 0, 0,
            9, 8, 5, 8, 11, 11, 11, 11, 5, 2, 5, 2, 1, 1, 1, 1,
            0, 6, 5, 6, 10, 10, 10, 10, 5, 4, 3, 4, 0, 0, 0, 0,
            9, 8, 5, 8, 11, 11, 11, 11, 5, 2, 5, 2, 1, 1, 1, 1,
            5, 6, 5, 6, 10, 10, 10, 10, 5, 4, 3, 4, 0, 0, 0, 0,
            9, 8, 5, 8, 11, 11, 11, 11, 5, 2, 5, 2, 1, 1, 1, 1,
            5, 6, 5, 6, 10, 10, 10, 10, 5, 4, 3, 4, 7, 0, 0, 0,
            9, 8, 5, 8, 11, 11, 11, 11, 5, 2, 5, 2, 1, 1, 1, 1,
            4, 6, 4, 6, 10, 10, 10, 10, 5, 4, 5, 4, 0, 0, 0, 0,
            9, 8, 5, 8, 11, 11, 12, 12, 5, 2, 5, 2, 1, 1, 2, 2,
            4, 6, 4, 6, 10, 10, 10, 10, 5, 4, 5, 4, 0, 0, 0, 0,
            9, 8, 5, 8, 11, 11, 12, 12, 5, 2, 5, 2, 1, 1, 2, 2,
            4, 6, 4, 6, 10, 10, 10, 10, 5, 4, 5, 4, 0, 0, 0, 0,
            9, 8, 5, 8, 11, 11, 11, 11, 5, 2, 5, 2, 1, 1, 1, 1,
            4, 6, 4, 6, 10, 10, 10, 10, 5, 4, 5, 4, 0, 0, 0, 0,
            9, 8, 5, 8, 11, 11, 11, 11, 5, 2, 5, 2, 1, 1, 1, 1,
        };

        private readonly int[] InstructionCycles = new int[256]
        {
            7, 6, 2, 8, 3, 3, 5, 5, 3, 2, 2, 2, 4, 4, 6, 6,
            2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            6, 6, 2, 8, 3, 3, 5, 5, 4, 2, 2, 2, 4, 4, 6, 6,
            2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            6, 6, 2, 8, 3, 3, 5, 5, 3, 2, 2, 2, 3, 4, 6, 6,
            2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            6, 6, 2, 8, 3, 3, 5, 5, 4, 2, 2, 2, 5, 4, 6, 6,
            2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            2, 6, 2, 6, 3, 3, 3, 3, 2, 2, 2, 2, 4, 4, 4, 4,
            2, 6, 2, 6, 4, 4, 4, 4, 2, 5, 2, 5, 5, 5, 5, 5,
            2, 6, 2, 6, 3, 3, 3, 3, 2, 2, 2, 2, 4, 4, 4, 4,
            2, 5, 2, 5, 4, 4, 4, 4, 2, 4, 2, 4, 4, 4, 4, 4,
            2, 6, 2, 8, 3, 3, 5, 5, 2, 2, 2, 2, 4, 4, 6, 6,
            2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
            2, 6, 2, 8, 3, 3, 5, 5, 2, 2, 2, 2, 4, 4, 6, 6,
            2, 5, 2, 8, 4, 4, 6, 6, 2, 4, 2, 7, 4, 4, 7, 7,
        };

        private readonly int[] InstructionPageCrossCycles = new int[256]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            1, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1, 1, 1, 1, 1,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0,
        };
        #endregion

        //Memory
        public CPUMemory Memory { get; private set; }

        //Registers
        private byte a;
        private byte x;
        private byte y;
        private byte status;

        //Pointers
        private ushort pc;
        private byte sp;

        //Interrupts
        private bool nmiPending;
        private bool irqPending;

        //Emulation
        public int Cycles { get; private set; }
        public bool Error { get; private set; }
        private int idleCycles;
        private byte opcode;     

        public CPU()
        {
            this.Instructions = new Instruction[256] 
            {
                //0      1      2      3      4      5      6      7      8      9      A      B      C      D      E      F
                OpBRK, OpORA, OpERR, OpERR, OpERR, OpORA, OpASL, OpERR, OpPHP, OpORA, OpASL, OpERR, OpERR, OpORA, OpASL, OpERR, //0
                OpBPL, OpORA, OpERR, OpERR, OpERR, OpORA, OpASL, OpERR, OpCLC, OpORA, OpERR, OpERR, OpERR, OpORA, OpASL, OpERR, //1
                OpJSR, OpAND, OpERR, OpERR, OpBIT, OpAND, OpROL, OpERR, OpPLP, OpAND, OpROL, OpERR, OpBIT, OpAND, OpROL, OpERR, //2
                OpBMI, OpAND, OpERR, OpERR, OpERR, OpAND, OpROL, OpERR, OpSEC, OpAND, OpERR, OpERR, OpERR, OpAND, OpROL, OpERR, //3
                OpRTI, OpEOR, OpERR, OpERR, OpERR, OpEOR, OpLSR, OpERR, OpPHA, OpEOR, OpLSR, OpERR, OpJMP, OpEOR, OpLSR, OpERR, //4
                OpBVC, OpEOR, OpERR, OpERR, OpERR, OpEOR, OpLSR, OpERR, OpCLI, OpEOR, OpERR, OpERR, OpERR, OpEOR, OpLSR, OpERR, //5
                OpRTS, OpADC, OpERR, OpERR, OpERR, OpADC, OpROR, OpERR, OpPLA, OpADC, OpROR, OpERR, OpJMP, OpADC, OpROR, OpERR, //6
                OpBVS, OpADC, OpERR, OpERR, OpERR, OpADC, OpROR, OpERR, OpSEI, OpADC, OpERR, OpERR, OpERR, OpADC, OpROR, OpERR, //7
                OpERR, OpSTA, OpERR, OpERR, OpSTY, OpSTA, OpSTX, OpERR, OpDEY, OpERR, OpTXA, OpERR, OpSTY, OpSTA, OpSTX, OpERR, //8
                OpBCC, OpSTA, OpERR, OpERR, OpSTY, OpSTA, OpSTX, OpERR, OpTYA, OpSTA, OpTXS, OpERR, OpERR, OpSTA, OpERR, OpERR, //9
                OpLDY, OpLDA, OpLDX, OpERR, OpLDY, OpLDA, OpLDX, OpERR, OpTAY, OpLDA, OpTAX, OpERR, OpLDY, OpLDA, OpLDX, OpERR, //A
                OpBCS, OpLDA, OpERR, OpERR, OpLDY, OpLDA, OpLDX, OpERR, OpCLV, OpLDA, OpTSX, OpERR, OpLDY, OpLDA, OpLDX, OpERR, //B
                OpCPY, OpCMP, OpERR, OpERR, OpCPY, OpCMP, OpDEC, OpERR, OpINY, OpCMP, OpDEX, OpERR, OpCPY, OpCMP, OpDEC, OpERR, //C
                OpBNE, OpCMP, OpERR, OpERR, OpERR, OpCMP, OpDEC, OpERR, OpCLD, OpCMP, OpERR, OpERR, OpERR, OpCMP, OpDEC, OpERR, //D
                OpCPX, OpSBC, OpERR, OpERR, OpCPX, OpSBC, OpINC, OpERR, OpINX, OpSBC, OpNOP, OpERR, OpCPX, OpSBC, OpINC, OpERR, //E
                OpBEQ, OpSBC, OpERR, OpERR, OpERR, OpSBC, OpINC, OpERR, OpSED, OpSBC, OpERR, OpERR, OpERR, OpSBC, OpINC, OpERR  //F
            };

            this.AddressModes = new addressMode[13]
            {
                AddrAbsolute,
                AddrAbsoluteX,
                AddrAbsoluteY,
                AddrAccumulator,
                AddrImmediate,
                AddrImplied,
                AddrIndirectX,
                AddrIndirect,
                AddrIndirectY,
                AddrRelative,
                AddrZeroPage,
                AddrZeroPageX,
                AddrZeroPageY
            };
        }

        #region Processor Functions
        public void Reset()
        {
            Memory = new CPUMemory();

            a = 0;
            x = 0;
            y = 0;

            sp = 253; //https://superuser.com/a/606770 https://wiki.nesdev.com/w/index.php/CPU_power_up_state
            pc = Memory.Read16(RSTAddress);

            status = 0x24;

            nmiPending = false;
            irqPending = false;

            Cycles = 0;
            idleCycles = 0;
            Error = false;
        }

        public int Step()
        {
            //Idle cycles force the cpu to idle, ppu can control this
            if (idleCycles > 0)
            {
                idleCycles--;
                return 1;
            }

            //Handle interrupts
            if (nmiPending)
            {
                DoNMI();
            }
            if (irqPending)
            {
                DoIRQ();
            }

            //Get cycles before instruction
            int startCycles = Cycles;

            //Fetch opcode, increment PC
            opcode = Memory.Read(pc);
            pc++;

            //Decode and run opcode
            Instructions[opcode](AddressModes[InstructionAddressMode[opcode]](), InstructionAddressMode[opcode]);
            
            //Add cycles
            Cycles += InstructionCycles[opcode];

            //Return cycles the instruction took
            return Cycles - startCycles;         
        }

        public void Stall(int cycles)
        {
            idleCycles += cycles;
        }

        public void RequestIRQ()
        {
            irqPending = true;
        }

        public void RequestNMI()
        {
            nmiPending = true;
        }

        private void DoIRQ()
        {
            //IRQ can be ignored if interrupts are disabled
            if (!GetStatus(StatusInterruptDisable))
            {
                PushStack16(pc);

                PushStack(status);

                SetStatus(StatusInterruptDisable, true);

                pc = Memory.Read16(BRKAddress);
            }

            irqPending = false;
        }

        private void DoNMI()
        {
            PushStack16(pc);
            PushStack(status);

            SetStatus(StatusInterruptDisable, true);

            pc = Memory.Read16(NMIAddress);

            nmiPending = false;
            irqPending = false; //https://wiki.nesdev.com/w/index.php/CPU_interrupts - Both NMI and IRQ -> IRQ forgotten
        }

        public string DebugString()
        {
            string Info = "";
            Info += string.Format("Opcode:            0x{0}\n", Convert.ToString(opcode, 16).ToUpper().PadLeft(2, '0'));
            Info += string.Format("Accumulator:       {0}\n", a);
            Info += string.Format("X register:        {0}\n", x);
            Info += string.Format("Y register:        {0}\n", y);
            Info += string.Format("Program counter:   {0}\n", pc);
            Info += string.Format("Stack pointer:     {0}\n", sp);
            Info += "\n";

            Info += string.Format("Carry:             {0}\n", GetStatus(StatusCarryFlag));
            Info += string.Format("Zero flag:         {0}\n", GetStatus(StatusZeroFlag));
            Info += string.Format("Interrupt toggle:  {0}\n", GetStatus(StatusInterruptDisable));
            Info += string.Format("Decimal mode:      {0}\n", GetStatus(StatusDecimalModeFlag));
            Info += string.Format("Interrupt execute: {0}\n", GetStatus(StatusBreakCommand));
            Info += string.Format("Overflow:          {0}\n", GetStatus(StatusOverflowFlag));
            Info += string.Format("Sign:              {0}", GetStatus(StatusNegativeFlag));
            return Info;
        }
        #endregion

        #region Status Bitflag Operations
        private void SetStatus(byte index, bool value)
        {
            byte indexBinary = (byte)(1 << index);
            if (value)
            {
                status |= indexBinary;
            }
            else
            {
                status &= (byte)(~indexBinary);
            }
        }

        private bool GetStatus(byte index)
        {
            byte indexBinary = (byte)(1 << index);
            return (status & indexBinary) > 0;
        }

        private void SetZeroSign(byte result)
        {
            SetStatus(StatusZeroFlag, result == 0);
            SetStatus(StatusNegativeFlag, (result & 0b10000000) != 0);
        }
        #endregion

        #region Stack Operations
        private byte PopStack()
        {
            sp++;
            return Memory.Read((ushort)(256 | sp));
        }

        private ushort PopStack16()
        {
            byte addressLS = PopStack();
            byte addressMS = PopStack();

            return (ushort)((addressMS << 8) + addressLS);
        }

        private void PushStack(byte data)
        {
            Memory.Write((ushort)(256 | sp), data);
            sp--;
        }

        private void PushStack16(ushort data)
        {
            PushStack((byte)((data >> 8) & 0b11111111));
            PushStack((byte)(data & 0b11111111));
        }
        #endregion

        #region Utility Functions
        private bool PageCrossed(ushort from, ushort to)
        {
            return (from & 0b11111111) != (to & 0b11111111);
        }

        private void AddBranchCycles(ushort address)
        {
            Cycles++;
            if (PageCrossed(pc, address))
            {
                Cycles++;
            }
        }
        #endregion

        #region Opcodes
        private void OpADC(ushort address, int addressMode)
        {
            byte data = Memory.Read(address);
            int carry = GetStatus(StatusCarryFlag) ? 1 : 0;
            byte result = (byte)(data + a + carry);

            SetZeroSign(result);
            SetStatus(StatusOverflowFlag, (~(a ^ data) & (a ^ result) & 0b10000000) != 0) ;
            SetStatus(StatusCarryFlag, (a + data + carry) > 0b11111111);

            a = (byte)(result & 0b11111111);
        }

        private void OpAND(ushort address, int addressMode)
        {
            a &= Memory.Read(address);

            SetZeroSign(a);
        }

        private void OpASL(ushort address, int addressMode)
        {
            if (addressMode == 3)
            {
                SetStatus(StatusCarryFlag, (a & 0b10000000) != 0);

                a <<= 1;

                SetZeroSign(a);
            }
            else
            {
                byte data = Memory.Read(address);

                SetStatus(StatusCarryFlag, (data & 0b10000000) != 0);

                data <<= 1;
                Memory.Write(address, data);

                SetZeroSign(data);
            }
        }

        private void OpBCC(ushort address, int addressMode)
        {
            if (!GetStatus(StatusCarryFlag))
            {
                AddBranchCycles(address);
                pc = address;
            }
        }

        private void OpBCS(ushort address, int addressMode)
        {
            if (GetStatus(StatusCarryFlag))
            {
                AddBranchCycles(address);
                pc = address;
            }
        }

        private void OpBEQ(ushort address, int addressMode)
        {
            if (GetStatus(StatusZeroFlag))
            {
                AddBranchCycles(address);
                pc = address;
            }
        }

        private void OpBIT(ushort address, int addressMode)
        {
            byte data = Memory.Read(address);
            SetStatus(StatusZeroFlag, (a & data) == 0);
            SetStatus(StatusNegativeFlag, (data & 0b10000000) != 0);
            SetStatus(StatusOverflowFlag, (data & 0b01000000) != 0);
        }

        private void OpBMI(ushort address, int addressMode)
        {
            if (GetStatus(StatusNegativeFlag))
            {
                pc = address;
            }
        }

        private void OpBNE(ushort address, int addressMode)
        {
            if (!GetStatus(StatusZeroFlag))
            {
                AddBranchCycles(address);
                pc = address;
            }
        }

        private void OpBPL(ushort address, int addressMode)
        {
            if (!GetStatus(StatusNegativeFlag))
            {
                AddBranchCycles(address);
                pc = address;
            }
        }

        private void OpBRK(ushort address, int addressMode)
        {
            PushStack16(pc);
            PushStack(status);

            SetStatus(StatusBreakCommand, true);

            pc = Memory.Read16(BRKAddress);
        }

        private void OpBVC(ushort address, int addressMode)
        {
            if (!GetStatus(StatusOverflowFlag))
            {
                AddBranchCycles(address);
                pc = address;
            }
        }

        private void OpBVS(ushort address, int addressMode)
        {
            if (GetStatus(StatusOverflowFlag))
            {
                AddBranchCycles(address);
                pc = address;
            }
        }

        private void OpCLC(ushort address, int addressMode)
        {
            SetStatus(StatusCarryFlag, false);
        }

        private void OpCLD(ushort address, int addressMode)
        {
            SetStatus(StatusDecimalModeFlag, false);
        }

        private void OpCLI(ushort address, int addressMode)
        {
            SetStatus(StatusInterruptDisable, false);
        }

        private void OpCLV(ushort address, int addressMode)
        {
            SetStatus(StatusOverflowFlag, false);
        }

        private void OpCMP(ushort address, int addressMode)
        {
            int value = Memory.Read(address);
            SetStatus(StatusCarryFlag, a >= value);
            SetZeroSign((byte)(a - value));
        }

        private void OpCPX(ushort address, int addressMode)
        {
            int value = Memory.Read(address);
            SetStatus(StatusCarryFlag, x >= value);
            SetZeroSign((byte)(x - value));
        }

        private void OpCPY(ushort address, int addressMode)
        {
            int value = Memory.Read(address);
            SetStatus(StatusCarryFlag, y >= value);
            SetZeroSign((byte)(y - value));
        }

        private void OpDEC(ushort address, int addressMode)
        {
            byte value = Memory.Read(address);
            value--;
            SetZeroSign(value);
            Memory.Write(address, value);
        }

        private void OpDEX(ushort address, int addressMode)
        {
            x--;
            SetZeroSign(x);
        }

        private void OpDEY(ushort address, int addressMode)
        {
            y--;
            SetZeroSign(y);
        }

        private void OpEOR(ushort address, int addressMode)
        {
            a ^= Memory.Read(address);
            SetZeroSign(a);
        }

        private void OpINC(ushort address, int addressMode)
        {
            byte value = Memory.Read(address);
            value++;
            SetZeroSign(value);
            Memory.Write(address, value);
        }

        private void OpINX(ushort address, int addressMode)
        {
            x++;
            SetZeroSign(x);
        }

        private void OpINY(ushort address, int addressMode)
        {
            y++;
            SetZeroSign(y);
        }

        private void OpJMP(ushort address, int addressMode)
        {
            pc = address;
        }

        private void OpJSR(ushort address, int addressMode)
        {
            PushStack16((ushort)(pc - 1));
            pc = address;
        }

        private void OpLDA(ushort address, int addressMode)
        {
            a = Memory.Read(address);
            SetZeroSign(a);
        }

        private void OpLDX(ushort address, int addressMode)
        {
            x = Memory.Read(address);
            SetZeroSign(x);
        }

        private void OpLDY(ushort address, int addressMode)
        {
            y = Memory.Read(address);
            SetZeroSign(y);
        }

        private void OpLSR(ushort address, int addressMode)
        {
            if (addressMode == 3)
            {
                SetStatus(StatusCarryFlag, (a & 0b00000001) == 1);

                a >>= 1;

                SetZeroSign(a);
            }
            else
            {
                byte data = Memory.Read(address);

                SetStatus(StatusCarryFlag, (data & 0b00000001) == 1);

                data >>= 1;
                Memory.Write(address, data);

                SetZeroSign(data);
            }
        }

        private void OpORA(ushort address, int addressMode)
        {
            a |= Memory.Read(address);

            SetZeroSign(a);
        }

        private void OpPHA(ushort address, int addressMode)
        {
            PushStack(a);
        }

        private void OpPHP(ushort address, int addressMode)
        {
            PushStack((byte)(status | 16));
        }

        private void OpPLA(ushort address, int addressMode)
        {
            a = PopStack();
            SetZeroSign(a);
        }

        private void OpPLP(ushort address, int addressMode)
        {
            status = (byte)(PopStack() & ~16);
            SetStatus(StatusUnused, true);
        }

        private void OpROL(ushort address, int addressMode)
        {
            if (addressMode == 3)
            {
                bool lsb = GetStatus(StatusCarryFlag);
                SetStatus(StatusCarryFlag, (a & 0b10000000) != 0);

                a <<= 1;
                if (lsb)
                {
                    a += 1;
                }

                SetZeroSign(a);
            }
            else
            {
                byte data = Memory.Read(address);

                bool lsb = GetStatus(StatusCarryFlag);
                SetStatus(StatusCarryFlag, (data & 0b10000000) != 0);

                data <<= 1;
                if (lsb)
                {
                    data += 1;
                }
                Memory.Write(address, data);

                SetZeroSign(data);
            }
        }

        private void OpROR(ushort address, int addressMode)
        {
            if (addressMode == 3)
            {
                bool msb = GetStatus(StatusCarryFlag);
                SetStatus(StatusCarryFlag, (a & 0b00000001) != 0);

                a >>= 1;
                if (msb)
                {
                    a += 0b10000000;
                }

                SetZeroSign(a);
            }
            else
            { 
                byte data = Memory.Read(address);

                bool msb = GetStatus(StatusCarryFlag);
                SetStatus(StatusCarryFlag, (data & 0b00000001) != 0);

                data >>= 1;
                if (msb)
                {
                    data += 0b10000000;
                }
                Memory.Write(address, data);

                SetZeroSign(data);
            }
        }

        private void OpRTI(ushort address, int addressMode)
        {
            status = PopStack();
            SetStatus(StatusUnused, true);

            pc = PopStack16();
        }

        private void OpRTS(ushort address, int addressMode)
        {
            pc = (ushort)(PopStack16() + 1);
        }

        private void OpSBC(ushort address, int addressMode)
        {
            byte data = Memory.Read(address);
            int carry = (!GetStatus(StatusCarryFlag) ? 1 : 0);

            byte result = (byte)(a - data - carry);
            SetZeroSign(result);

            SetStatus(StatusCarryFlag, (a - data - carry) >= 0);

            SetStatus(StatusOverflowFlag, ((a ^ data) & (a ^ result) & 0x80) != 0);

            a = result;
        }

        private void OpSEC(ushort address, int addressMode)
        {
            SetStatus(StatusCarryFlag, true);
        }

        private void OpSED(ushort address, int addressMode)
        {
            SetStatus(StatusDecimalModeFlag, true);
        }

        private void OpSEI(ushort address, int addressMode)
        {
            SetStatus(StatusInterruptDisable, true);
        }

        private void OpSTA(ushort address, int addressMode)
        {
            Memory.Write(address, a);
        }

        private void OpSTX(ushort address, int addressMode)
        {
            Memory.Write(address, x);
        }

        private void OpSTY(ushort address, int addressMode)
        {
            Memory.Write(address, y);
        }

        private void OpTAX(ushort address, int addressMode)
        {
            x = a;
            SetZeroSign(x);
        }

        private void OpTAY(ushort address, int addressMode)
        {
            y = a;
            SetZeroSign(y);
        }

        private void OpTSX(ushort address, int addressMode)
        {
            x = sp;
            SetZeroSign(x);
        }

        private void OpTXA(ushort address, int addressMode)
        {
            a = x;
            SetZeroSign(a);
        }

        private void OpTXS(ushort address, int addressMode)
        {
            sp = x;
        }

        private void OpTYA(ushort address, int addressMode)
        {
            a = y;
            SetZeroSign(a);
        }

        private void OpERR(ushort address, int addressMode)
        {
            Error = true;
        }

        private void OpNOP(ushort address, int addressMode)
        {
        }
        #endregion

        #region Adressing modes
        private ushort AddrImmediate()
        {
            return pc++;
        }

        private ushort AddrImplied()
        {
            return 0;
        }

        private ushort AddrZeroPage()
        {
            return Memory.Read(pc++);
        }

        private ushort AddrZeroPageX()
        {
            return (ushort)((Memory.Read(pc++) + x) & 255);
        }

        private ushort AddrZeroPageY()
        {
            return (ushort)((Memory.Read(pc++) + y) & 255);
        }

        private ushort AddrRelative()
        {
            byte offset = Memory.Read(pc++);
            //Convert unsigned representation to signed byte
            sbyte signedOffset = ((offset & 0b10000000) > 0 ? (sbyte)(offset | 0b1111111100000000) : (sbyte)offset);
            return (ushort)(pc + signedOffset);
        }

        private ushort AddrAbsolute()
        {
            ushort result = Memory.Read16(pc);
            pc += 2;
            return result;
        }

        private ushort AddrAbsoluteX()
        {
            ushort result = (ushort)(Memory.Read16(pc) + x);
            pc += 2;
            if (PageCrossed((ushort)(result - x), x) && InstructionPageCrossCycles[opcode] != 0)
            {
                Cycles++;
            }
            return result;
        }

        private ushort AddrAbsoluteY()
        {
            ushort result = (ushort)(Memory.Read16(pc) + y);
            pc += 2;
            if (PageCrossed((ushort)(result - y), y) && InstructionPageCrossCycles[opcode] != 0)
            {
                Cycles++;
            }
            return result;
        }

        private ushort AddrAccumulator()
        {
            return a;
        }

        private ushort AddrIndirect()
        {
            ushort indirectAddress = Memory.Read16(pc);
            pc += 2;
            return Memory.Read16(indirectAddress, true);
        }

        private ushort AddrIndirectX()
        {
            byte indirectAddressLS = (byte)((Memory.Read(pc++) + x) % 256);
            byte indirectAddressMS = (byte)((indirectAddressLS + 1) % 256); 

            return (ushort)(Memory.Read(indirectAddressLS) + (Memory.Read(indirectAddressMS) << 8));
        }

        private ushort AddrIndirectY()
        {
            byte indirectAddressLS = Memory.Read(pc++);
            byte indirectAddressMS = (byte)((indirectAddressLS + 1) % 256);

            ushort Result = (ushort)(Memory.Read(indirectAddressLS) + (Memory.Read(indirectAddressMS) << 8) + y);

            if (PageCrossed((ushort)(Result - y), Result) && InstructionPageCrossCycles[opcode] != 0)
            {
                Cycles++;
            }

            return Result;
        }
        #endregion
    }
}