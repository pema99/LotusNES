using System;

namespace LotusNES.Core
{
    [Serializable]
    public class GxROM : Mapper
    {
        private int prgBankBase;
        private int chrBankBase;

        public GxROM()
            : base(Emulator.GamePak.VerticalVRAMMirroring ? VRAMMirroringMode.Vertical : VRAMMirroringMode.Horizontal)
        {
        }

        public override byte Read(ushort address)
        {
            //For PPU
            if (address < 0x2000)
            {
                return Emulator.GamePak.ReadCharROM(chrBankBase + address);
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
                Emulator.GamePak.WriteCharRAM(chrBankBase + address, data);
            }

            //Bank select
            else if (address >= 0x8000)
            {
                if (Emulator.GamePak.MapperID == 11) //Color dreams bootleg games
                {
                    prgBankBase = (data & 0b00000011) * 0x8000;
                    chrBankBase = ((data & 0b11110000) >> 4) * 0x2000;
                }
                else
                {
                    prgBankBase = ((data & 0b00110000) >> 4) * 0x8000;
                    chrBankBase = (data & 0b00000011) * 0x2000;
                }
            }
        }
    }
}
