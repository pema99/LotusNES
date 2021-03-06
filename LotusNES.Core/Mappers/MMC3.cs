﻿using System;

namespace LotusNES.Core
{
    [Serializable]
    public class MMC3 : Mapper
    {
        //Bank data registers
        private byte[] bankRegisters;

        //Bank select register
        private int bankSelect;
        private bool prgROMBankMode;
        private bool chrROMBankMode;

        //PRG RAM protect register
        private bool writeProtect;
        private bool prgRAMEnable;

        //Scanline counting registers
        private byte irqLatch;
        private bool irqEnable;
        public int irqCounter;

        //Base addresses
        private int[] chrBankBase;
        private int[] prgBankBase;

        //MMC6 support
        private bool mmc6;

        public MMC3(Emulator emu, bool MMC6 = false)
            : base(emu, VRAMMirroringMode.Horizontal)
        {
            this.mmc6 = MMC6;

            this.bankRegisters = new byte[8];
            this.chrBankBase = new int[8]; //8 char banks
            this.prgBankBase = new int[4]; //4 prg banks

            //Start at lower 2 banks
            this.prgBankBase[0] = 0; 
            this.prgBankBase[1] = 0x2000;

            //Start at upper 2 banks
            this.prgBankBase[2] = emu.GamePak.ProgramROMBanks * 0x4000 - 0x4000;
            this.prgBankBase[3] = prgBankBase[2] + 0x2000;
        }

        public override byte Read(ushort address)
        {
            //0000 - 1FFF, 8k bankswitched chr rom or ram
            if (address < 0x2000)
            {
                return emu.GamePak.ReadCharROM(chrBankBase[address / 0x400] + (address % 0x400));
            }

            //6000 - 7FFF, non bankswitched prg ram
            else if (address >= 0x6000 && address < 0x8000)
            {
                if (prgRAMEnable)
                {
                    return emu.GamePak.ReadProgramRAM(address - 0x6000);
                }
            }

            //8000 - FFFF is prg rom
            else if (address >= 0x8000)
            {
                address -= 0x8000;
                return emu.GamePak.ReadProgramROM(prgBankBase[address / 0x2000] + (address % 0x2000));
            }

            return 0;
        }

        public override void Write(ushort address, byte data)
        {
            //0000 - 1FFF, 8k bankswitched chr rom or ram
            if (address < 0x2000)
            {
                emu.GamePak.WriteCharRAM(chrBankBase[address / 0x400] + (address % 0x400), data);
            }

            //6000 - 7FFF, non bankswitched prg ram
            else if (address >= 0x6000 && address < 0x8000)
            {
                if (!writeProtect)
                {
                    emu.GamePak.WriteProgramRAM(address - 0x6000, data);
                }
            }

            //8000 - FFFF is internal registers
            else if (address >= 0x8000)
            {
                SetRegister(address, data);
            }
        }

        public override void Step()
        {
            //When using 8x8 sprites, if the BG uses $0000, and the sprites use $1000, the IRQ counter should decrement on PPU cycle 260.
            //When using 8x8 sprites, if the BG uses $1000, and the sprites use $0000, the IRQ counter should decrement on PPU cycle 324.
            //TODO: When using 8x16 sprites PPU A12 must be explicitly tracked. 
            byte ppuctrl = emu.PPU.GetRegister(0x2000);
            bool bg = (ppuctrl & 0b00010000) != 0;
            bool sprite = (ppuctrl & 0b00001000) != 0;

            //Currently dont handle 8x16 sprites, default to 260
            int irqCycle = (bg && !sprite) ? 324 : 260;

            //  When PPU rendering        Count visible scanlines        At a specific cycle
            if (emu.PPU.Rendering && emu.PPU.Scanline < 240 && emu.PPU.Cycle == irqCycle)
            {
                if (irqCounter == 0) //Reload counter
                {
                    irqCounter = irqLatch;
                }
                else
                {
                    irqCounter--;
                    if (irqCounter == 0 && irqEnable)
                    {
                        emu.CPU.RequestIRQ();
                    }
                }
            }
        }

        private void SetRegister(ushort address, byte data)
        {
            bool evenAddress = address % 2 == 0;
            if (address < 0xA000)
            {
                if (evenAddress)
                {
                    bankSelect = data & 0b111;
                    prgROMBankMode = (data & 0b01000000) != 0;
                    chrROMBankMode = (data & 0b10000000) != 0;

                    if (mmc6)
                    {
                        prgRAMEnable = (data & 0b00100000) != 0;
                    }
                }
                else
                {
                    bankRegisters[bankSelect] = data;
                }
                CalculateBankOffsets();
            }
            else if (address < 0xC000)
            {
                if (evenAddress)
                {
                    VRAMMirroring = (data & 1) == 0 ? VRAMMirroringMode.Vertical : VRAMMirroringMode.Horizontal;
                }
                else
                {
                    if (!mmc6)
                    {
                        writeProtect = (data & 0b01000000) != 0;
                        prgRAMEnable = (data & 0b10000000) != 0;
                    }
                }
            }
            else if (address < 0xE000)
            {
                if (evenAddress)
                {
                    irqLatch = data;
                }
                else
                {
                    irqCounter = 0;
                }
            }
            else
            {
                if (evenAddress)
                {
                    irqEnable = false;
                }
                else
                {
                    irqEnable = true;
                }
            }
        }

        //https://wiki.nesdev.com/w/index.php/MMC3#Registers
        private void CalculateBankOffsets()
        {
            //Top diagram on nesdev
            if (chrROMBankMode) //is $80
            {
                chrBankBase[0] = bankRegisters[2] * 0x400;
                chrBankBase[1] = bankRegisters[3] * 0x400;
                chrBankBase[2] = bankRegisters[4] * 0x400;
                chrBankBase[3] = bankRegisters[5] * 0x400;
                chrBankBase[4] = (bankRegisters[0] & 0b11111110) * 0x400; //Even
                chrBankBase[5] = (bankRegisters[0] | 1) * 0x400;          //Odd
                chrBankBase[6] = (bankRegisters[1] & 0b11111110) * 0x400; //Even 
                chrBankBase[7] = (bankRegisters[1] | 1) * 0x400;          //Odd
            }
            else //is $00
            {
                chrBankBase[0] = (bankRegisters[0] & 0b11111110) * 0x400; //Even
                chrBankBase[1] = (bankRegisters[0] | 1) * 0x400;          //Odd
                chrBankBase[2] = (bankRegisters[1] & 0b11111110) * 0x400; //Even
                chrBankBase[3] = (bankRegisters[1] | 1) * 0x400;          //Odd
                chrBankBase[4] = bankRegisters[2] * 0x400;
                chrBankBase[5] = bankRegisters[3] * 0x400;
                chrBankBase[6] = bankRegisters[4] * 0x400;
                chrBankBase[7] = bankRegisters[5] * 0x400;
            }

            //Bottom diagram on nesdev
            if (prgROMBankMode) //is $80
            {
                prgBankBase[0] = emu.GamePak.ProgramROMBanks * 0x4000 - 0x4000;
                prgBankBase[1] = bankRegisters[7] * 0x2000;
                prgBankBase[2] = bankRegisters[6] * 0x2000;
                prgBankBase[3] = emu.GamePak.ProgramROMBanks * 0x4000 - 0x2000;
            }
            else //is $00
            {
                prgBankBase[0] = bankRegisters[6] * 0x2000;
                prgBankBase[1] = bankRegisters[7] * 0x2000;
                prgBankBase[2] = emu.GamePak.ProgramROMBanks * 0x4000 - 0x4000;
                prgBankBase[3] = emu.GamePak.ProgramROMBanks * 0x4000 - 0x2000;
            }

            for (int i = 0; i < prgBankBase.Length; i++)
            {
                prgBankBase[i] %= emu.GamePak.ProgramROMLength;
            }

            for (int i = 0; i < chrBankBase.Length; i++)
            {
                chrBankBase[i] %= emu.GamePak.CharROMLength;
            }
        }
    }
}
