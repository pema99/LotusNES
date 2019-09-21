using System;

namespace LotusNES.Core
{
    [Serializable]
    public class NROM : Mapper
    {
        public NROM(Emulator emu)
            : base(emu, emu.GamePak.VerticalVRAMMirroring ? VRAMMirroringMode.Vertical : VRAMMirroringMode.Horizontal)
        {
        }

        public override byte Read(ushort address)
        {
            //For PPU
            if (address < 0x2000)
            {
                return emu.GamePak.ReadCharROM(address);
            }

            //Program RAM
            else if (address >= 0x6000 && address < 0x8000)
            {
                return emu.GamePak.ReadProgramRAM(address - 0x6000);
            }

            //Program ROM is from 0x8000 - 0xFFFF
            else if (address >= 0x8000)
            {
                if (emu.GamePak.ProgramROMBanks == 1)
                {
                    //Mirror ROM
                    return emu.GamePak.ReadProgramROM((address - 0x8000) % 16384);
                }
                else
                {
                    return emu.GamePak.ReadProgramROM(address - 0x8000);
                }
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

            //Program RAM
            else if (address >= 0x6000 && address < 0x8000)
            {
                emu.GamePak.WriteProgramRAM(address - 0x6000, data);
            }
        }
    }
}
