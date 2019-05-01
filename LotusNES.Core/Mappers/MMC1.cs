using System;

namespace LotusNES.Core
{
    [Serializable]
    public class MMC1 : Mapper
    {
        //Shared shift reg
        private byte shiftRegister;
        private int numWrites;
        
        //Registers
        private byte controlRegister;
        private byte chrBank0Register;
        private byte chrBank1Register;
        private byte prgBankRegister;

        //Cached values
        private byte prgROMBankMode;
        private bool chrROMBankMode;

        //Base addresses
        private int chrBank0Base;
        private int chrBank1Base;
        private int prgBank0Base;
        private int prgBank1Base;

        public MMC1()
            : base(VRAMMirroringMode.Horizontal)
        {
            this.shiftRegister = 0x0C;
            this.prgBank1Base = (Emulator.GamePak.ProgramROMBanks - 1) * 0x4000;
            this.chrROMBankMode = false;
            this.prgROMBankMode = 3;
        }

        public override byte Read(ushort address)
        {
            //0000 - 1FFF is CHR rom banks, 2
            if (address < 0x2000)
            {
                //0000 - 0FFF is chr bank 0
                if (address < 0x1000)
                {
                    return Emulator.GamePak.ReadCharROM(chrBank0Base + address);
                }
                //1000 - 1FFF is 1
                else
                {
                    return Emulator.GamePak.ReadCharROM(chrBank1Base + (address % 0x1000));
                }
            }

            //Program RAM, not bank switched
            if (address >= 0x6000 && address < 0x8000)
            {
                return Emulator.GamePak.ReadProgramRAM(address - 0x6000);
            }

            //8000 - FFFE is PRG ROM banks, 2
            else if (address >= 0x8000)
            {
                address -= 0x8000;

                //8000 - BFFF is prg bank 0
                if (address < 0x4000)
                {
                    return Emulator.GamePak.ReadProgramROM(prgBank0Base + address);
                }
                //C000 - FFFE is 1
                else
                {
                    return Emulator.GamePak.ReadProgramROM(prgBank1Base + (address % 0x4000));
                }
            }

            //Open bus, apparently
            else
            {
                return 0;
            }
        }

        public override void Write(ushort address, byte data)
        {
            //For PPU
            if (address < 0x2000)
            {
                if (Emulator.GamePak.UsesCharRAM)
                {
                    //0000 - 0FFF is chr bank 0
                    if (address < 0x1000)
                    {
                        Emulator.GamePak.WriteCharRAM(chrBank0Base + address, data);
                    }
                    //1000 - 1FFF is 1
                    else
                    {
                        Emulator.GamePak.WriteCharRAM(chrBank1Base + (address % 0x1000), data);
                    }
                }
            }

            //Program RAM
            else if (address >= 0x6000 && address < 0x8000)
            {
                Emulator.GamePak.WriteProgramRAM(address - 0x6000, data);
            }

            //Shift register for writing to internal registers
            else if (address >= 0x8000)
            {
                if ((data & 0b10000000) != 0)
                {
                    shiftRegister = 0;
                    numWrites = 0;
                }
                else
                {
                    shiftRegister |= (byte)((data & 1) << numWrites);
                    numWrites++;

                    if (numWrites == 5)
                    {
                        SetRegister(address);

                        shiftRegister = 0;
                        numWrites = 0;
                    }
                }
            }
        }

        private void SetRegister(ushort address)
        {
            //Control, 8000 - 9FFF
            if (address < 0xA000)
            {
                controlRegister = shiftRegister;
                switch (controlRegister & 0b11)
                {
                    case 0:
                        VRAMMirroring = VRAMMirroringMode.SingleScreenLower;
                        break;

                    case 1:
                        VRAMMirroring = VRAMMirroringMode.SingleScreenUpper;
                        break;

                    case 2:
                        VRAMMirroring = VRAMMirroringMode.Vertical;
                        break;

                    case 3:
                        VRAMMirroring = VRAMMirroringMode.Horizontal;
                        break;
                }
                prgROMBankMode = (byte)((controlRegister & 0b01100) >> 2);
                chrROMBankMode = (controlRegister & 0b10000) != 0;
            }

            //CHR 0, A000 - BFFF
            else if (address < 0xC000)
            {
                chrBank0Register = shiftRegister;
            }

            //CHR 1, C000 - DFFF
            else if (address < 0xE000)
            {
                chrBank1Register = shiftRegister;
            }

            //PRG, E000 - FFFF
            else
            {
                prgBankRegister = shiftRegister;
            }

            CalculateBankOffsets();
        }

        //https://wiki.nesdev.com/w/index.php/MMC1#Registers
        private void CalculateBankOffsets()
        {
            //Seperate 4k banks
            if (chrROMBankMode)
            {
                chrBank0Base = chrBank0Register * 0x1000;
                chrBank1Base = chrBank1Register * 0x1000;
            }
            //One 8k bank, lsb ignored
            else
            {
                chrBank0Base = ((chrBank0Register & 0b11110) >> 1) * 0x1000;
                chrBank1Base = chrBank0Base + 0x1000;
            }

            switch (prgROMBankMode)
            {
                case 0:
                case 1: //Switch 32k bank at 8000, ignore lsb
                    prgBank0Base = ((prgBankRegister & 0b1110) >> 1) * 0x4000;
                    prgBank1Base = prgBank0Base + 0x4000;
                    break;

                case 2: //Switch 16k upper bank, fix lower at 8000
                    prgBank0Base = 0;
                    prgBank1Base = (prgBankRegister & 0b1111) * 0x4000;
                    break;
                    
                case 3: //Switch 16k lower bank, fix upper at C000
                    prgBank0Base = (prgBankRegister & 0b1111) * 0x4000;
                    prgBank1Base = (Emulator.GamePak.ProgramROMBanks - 1) * 0x4000; //Top bank
                    break;
            }
        }
    }
}
