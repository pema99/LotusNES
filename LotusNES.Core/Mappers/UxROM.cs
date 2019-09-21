using System;

namespace LotusNES.Core
{
    [Serializable]
    public class UxROM : Mapper
    {
        private int prgBankBase;

        public UxROM(Emulator emu)
            : base(emu, emu.GamePak.VerticalVRAMMirroring ? VRAMMirroringMode.Vertical : VRAMMirroringMode.Horizontal)
        {
        }

        public override byte Read(ushort address)
        {
            //0000 - 1FFF non bankswitched char rom
            if (address < 0x2000)
            {
                return emu.GamePak.ReadCharROM(address);
            }

            //8000 - BFFF switchable 16 prg rom
            if (address >= 0x8000 && address < 0xC000)
            {
                address -= 0x8000;
                return emu.GamePak.ReadProgramROM(prgBankBase + address);
            }
            //C000 - FFFF 16kb non switchable prg rom, fixed to last bank
            else if (address >= 0xC000)
            {
                address -= 0xC000;
                return emu.GamePak.ReadProgramROM((emu.GamePak.ProgramROMBanks - 1) * 0x4000 + address);
            }

            //Open bus, apparently
            else
            {
                return 0;
            }
        }

        public override void Write(ushort address, byte data)
        {
            //0000 - 1FFF non bankswitched char ram
            if (address < 0x2000)
            {
                emu.GamePak.WriteCharRAM(address, data);
            }

            //8000 - FFFF bank select
            if (address >= 0x8000)
            {
                int mask = emu.GamePak.MapperID == 71 ? 0b1111 : 0b111;
                prgBankBase = (data & mask) * 0x4000 % emu.GamePak.ProgramROMLength;
            }
        }
    }
}
