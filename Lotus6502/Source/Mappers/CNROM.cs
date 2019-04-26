using System;

namespace Lotus6502
{
    [Serializable]
    public class CNROM : Mapper
    {
        private int chrBankBase;

        public CNROM()
            : base(Emulator.GamePak.VerticalVRAMMirroring ? VRAMMirroringMode.Vertical : VRAMMirroringMode.Horizontal)
        {
        }

        public override byte Read(ushort address)
        {
            //0000 - 1FFF bankswitched CHR rom
            if (address < 0x2000)
            {
                return Emulator.GamePak.ReadCharROM(chrBankBase + address);
            }

            //8000 - FFFF prg rom (16 or 32k)
            if (address >= 0x8000)
            {
                address -= 0x8000;
                return Emulator.GamePak.ReadProgramROM(address);
            }

            //Open bus, apparently
            else
            {
                return 0;
            }
        }

        public override void Write(ushort address, byte data)
        {
            //0000 - 1FFF bankswitched CHR rom/ram??
            if (address < 0x2000)
            {
                if (Emulator.GamePak.UsesCharRAM)
                {
                    Emulator.GamePak.WriteCharRAM(chrBankBase + address, data);
                }
            }

            //8000 - FFFF bank select
            if (address >= 0x8000)
            {
                chrBankBase = (data & 0b11) * 0x2000;
            }
        }
    }
}
