﻿using System;

namespace LotusNES.Core
{
    [Serializable]
    public class AxROM : Mapper
    {
        private int prgBankBase;

        public AxROM(Emulator emu)
            : base(emu, VRAMMirroringMode.SingleScreenLower)
        {
        }

        public override byte Read(ushort address)
        {
            //For PPU
            if (address < 0x2000)
            {
                return emu.GamePak.ReadCharROM(address);
            }

            //Program ROM is from 0x8000 - 0xFFFF
            else if (address >= 0x8000)
            {
                return emu.GamePak.ReadProgramROM(prgBankBase + (address - 0x8000));
            }

            //Open bus, apparently
            return 0;
        }

        public override void Write(ushort address, byte data)
        {
            //For PPU
            if (address < 0x2000)
            {
                emu.GamePak.WriteCharRAM(address, data);
            }

            //Bank select
            else if (address >= 0x8000)
            {
                VRAMMirroring = (data & 0b00010000) != 0 ? VRAMMirroringMode.SingleScreenUpper : VRAMMirroringMode.SingleScreenLower;
                prgBankBase = (data & 0b00000111) * 0x8000;
            }
        }
    }
}
