using System;

namespace LotusNES
{
    [Serializable]
    public class NROM : Mapper
    {
        public NROM()
            : base(Emulator.GamePak.VerticalVRAMMirroring ? VRAMMirroringMode.Vertical : VRAMMirroringMode.Horizontal)
        {
        }

        public override byte Read(ushort address)
        {
            //For PPU
            if (address < 0x2000)
            {
                return Emulator.GamePak.ReadCharROM(address);
            }

            //Program RAM
            else if (address >= 0x6000 && address < 0x8000)
            {
                return Emulator.GamePak.ReadProgramRAM(address - 0x6000);
            }

            //Program ROM is from 0x8000 - 0xFFFF
            else if (address >= 0x8000)
            {
                if (Emulator.GamePak.ProgramROMBanks == 1)
                {
                    //Mirror ROM
                    return Emulator.GamePak.ReadProgramROM((address - 0x8000) % 16384);
                }
                else
                {
                    return Emulator.GamePak.ReadProgramROM(address - 0x8000);
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
                Emulator.GamePak.WriteCharRAM(address, data);
            }

            //Program RAM
            else if (address >= 0x6000 && address < 0x8000)
            {
                Emulator.GamePak.WriteProgramRAM(address - 0x6000, data);
            }
        }
    }
}
