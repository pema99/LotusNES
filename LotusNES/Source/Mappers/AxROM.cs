using System;

namespace LotusNES
{
    [Serializable]
    public class AxROM : Mapper
    {
        private int prgBankBase;

        public AxROM()
            : base(VRAMMirroringMode.SingleScreenLower)
        {
        }

        public override byte Read(ushort address)
        {
            //For PPU
            if (address < 0x2000)
            {
                return Emulator.GamePak.ReadCharROM(address);
            }

            //Program ROM is from 0x8000 - 0xFFFF
            else if (address >= 0x8000)
            {
                return Emulator.GamePak.ReadProgramROM(prgBankBase + (address - 0x8000));
            }

            //Open bus, apparently
            return 0;
        }

        public override void Write(ushort address, byte data)
        {
            //For PPU
            if (address < 0x2000)
            {
                Emulator.GamePak.WriteCharRAM(address, data);
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
