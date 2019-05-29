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
        private int[] chrBankBase;
        private int[] prgBankBase;

        public MMC1()
            : base(Emulator.GamePak.VerticalVRAMMirroring ? VRAMMirroringMode.Vertical : VRAMMirroringMode.Horizontal)
        {
            this.shiftRegister = 0x10;

            this.prgBankBase = new int[2];
            this.chrBankBase = new int[2];

            this.prgBankBase[1] = (Emulator.GamePak.ProgramROMBanks - 1) * 0x4000;

            this.chrROMBankMode = false;
            this.prgROMBankMode = 3;
        }

        public override byte Read(ushort address)
        {
            //0000 - 1FFF is CHR rom banks, 2
            if (address < 0x2000)
            {
                return Emulator.GamePak.ReadCharROM((chrBankBase[address / 0x1000] + address % 0x1000) % Emulator.GamePak.CharROMLength);
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
                return Emulator.GamePak.ReadProgramROM(prgBankBase[address / 0x4000] + address % 0x4000);
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
                Emulator.GamePak.WriteCharRAM((chrBankBase[address / 0x1000] + address % 0x1000) % Emulator.GamePak.CharROMLength, data);
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
                chrBankBase[0] = chrBank0Register * 0x1000;
                chrBankBase[1] = chrBank1Register * 0x1000;
            }
            //One 8k bank, lsb ignored
            else
            {
                chrBankBase[0] = (chrBank0Register & 0b11111110) * 0x1000;
                chrBankBase[1] = chrBankBase[0] + 0x1000;
            }

            switch (prgROMBankMode)
            {
                case 0:
                case 1: //Switch 32k bank at 8000, ignore lsb
                    prgBankBase[0] = ((prgBankRegister & 0b1110) >> 1) * 0x4000;
                    prgBankBase[1] = prgBankBase[0] + 0x4000;
                    break;

                case 2: //Switch 16k upper bank, fix lower at 8000
                    prgBankBase[0] = 0;
                    prgBankBase[1] = (prgBankRegister & 0b1111) * 0x4000;
                    break;
                    
                case 3: //Switch 16k lower bank, fix upper at C000
                    prgBankBase[0] = (prgBankRegister & 0b1111) * 0x4000;
                    prgBankBase[1] = (Math.Min(16, Emulator.GamePak.ProgramROMBanks) - 1) * 0x4000; //Top bank
                    break;
            }
        }
    }
}
