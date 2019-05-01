using System;

namespace LotusNES
{
    [Serializable]
    public class PPUMemory : MemoryMap
    {
        private byte[] VRAM;
        private byte[] paletteRAM;

        public PPUMemory()
        {
            this.VRAM = new byte[2048];
            this.paletteRAM = new byte[32];
        }

        public override byte Read(ushort address)
        {
            //2k char rom, pattern tables, between 0 - 0x2000
            if (address < 0x2000)
            {
                return Emulator.Mapper.Read(address);
            }

            //Next comes 2k VRAM, mirrored until 0x3F00
            else if (address < 0x3F00)
            {             
                return VRAM[HandleVRAMMirroring(address)];
            }

            //32 bytes of palette ram, mirror until 0x4000
            else
            {
                int paletteIndex = HandlePaletteRAMMirroring(address);
                if (paletteIndex % 4 == 0) //Redirect to global background only on read
                {
                    paletteIndex = 0;
                }
                return paletteRAM[paletteIndex];
            }
        }

        public override void Write(ushort address, byte data)
        {
            //2k char rom, pattern tables, between 0 - 0x2000
            if (address < 0x2000)
            {
                Emulator.Mapper.Write(address, data);
            }

            //Next comes 2k VRAM, nametables, mirrored until 0x3F00
            else if (address < 0x3F00)
            {
                VRAM[HandleVRAMMirroring(address)] = data;
            }

            //32 bytes of palette ram, mirror until 0x4000
            else
            {
                paletteRAM[HandlePaletteRAMMirroring(address)] = data;
            }
        }

        private int HandleVRAMMirroring(ushort address)
        {
            int vramIndex = (address - 0x2000) % 0x1000;
            if (Emulator.Mapper.VRAMMirroring == VRAMMirroringMode.Vertical)
            {
                if (vramIndex >= 0x800)
                {
                    vramIndex -= 0x800;
                }
            }
            else if (Emulator.Mapper.VRAMMirroring == VRAMMirroringMode.Horizontal)
            {
                if (vramIndex > 0x800)
                {
                    vramIndex %= 0x400;
                    vramIndex += 0x400;
                }
                else
                {
                    vramIndex %= 0x400;
                }
            }
            else if (Emulator.Mapper.VRAMMirroring == VRAMMirroringMode.SingleScreenLower)
            {
                vramIndex %= 0x400;
            }
            else if (Emulator.Mapper.VRAMMirroring == VRAMMirroringMode.SingleScreenUpper)
            {
                vramIndex %= 0x400;
                vramIndex += 0x400;
            }
            return vramIndex;
        }

        private int HandlePaletteRAMMirroring(ushort address)
        {
            //https://wiki.nesdev.com/w/index.php/PPU_palettes#Memory_Map
            int paletteIndex = (address - 0x3F00) % 32;

            //Universal background color for sprites
            if (paletteIndex % 4 == 0 && paletteIndex >= 16)
            {
                paletteIndex -= 16;
            }

            return paletteIndex;
        }
    }
}
