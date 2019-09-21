using System;

namespace LotusNES.Core
{
    [Serializable]
    public class CNROM : Mapper
    {
        private int chrBankBase;

        public CNROM(Emulator emu)
            : base(emu, emu.GamePak.VerticalVRAMMirroring ? VRAMMirroringMode.Vertical : VRAMMirroringMode.Horizontal)
        {
        }

        public override byte Read(ushort address)
        {
            //0000 - 1FFF bankswitched CHR rom
            if (address < 0x2000)
            {
                return emu.GamePak.ReadCharROM(chrBankBase + address);
            }

            //8000 - FFFF prg rom (16 or 32k)
            if (address >= 0x8000)
            {
                address -= 0x8000;
                return emu.GamePak.ReadProgramROM(address);
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
                if (emu.GamePak.UsesCharRAM)
                {
                    emu.GamePak.WriteCharRAM(chrBankBase + address, data);
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
