using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace LotusNES.Core
{
    public class GamePak
    {
        //Used for checksumming
        private static SHA1CryptoServiceProvider SHA1 = new SHA1CryptoServiceProvider();

        public int MapperID { get; private set; }
        public bool VerticalVRAMMirroring { get; private set; }
        public bool Battery { get; private set; }
        public bool Trainer { get; private set; }
        public string Checksum { get; private set; }
        public bool UsesCharRAM { get; private set; }

        private byte[] programRAM;

        private byte[] programROM;
        public int ProgramROMBanks { get;  private set; }
        public int ProgramROMLength { get { return programROM.Length; } }

        private byte[] charROM;
        public int CharROMBanks { get; private set; }
        public int CharROMLength { get { return charROM.Length; } }

        public GamePak(string path)
        {
            this.programRAM = new byte[8192];

            BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read));

            //Parse header https://wiki.nesdev.com/w/index.php/INES
            if (reader.ReadUInt32() != 0x1A53454E)
            {
                throw new Exception("Bad iNES header");
            }
            this.ProgramROMBanks = reader.ReadByte();
            this.CharROMBanks = reader.ReadByte();
            if (this.CharROMBanks == 0)
            {
                this.CharROMBanks = 2;
                this.UsesCharRAM = true;
            }
            byte flags6 = reader.ReadByte();
            this.VerticalVRAMMirroring = (flags6 & 1) > 0;
            this.Battery = (flags6 & 2) > 0;
            this.Trainer = (flags6 & 4) > 0;

            byte flags7 = reader.ReadByte();

            //Check for invalid dump tags, http://forums.nesdev.com/viewtopic.php?p=61850&sid=882a4f6a0d8ce036105ce944148584ee#p61850
            reader.BaseStream.Seek(15, SeekOrigin.Begin);
            if (reader.ReadByte() != 0)
            {
                flags7 = 0;
            }

            this.MapperID = flags7 & 0b11110000 | (flags6 >> 4 & 0b00001111);

            //Parse program ROM
            int offset = Trainer ? 528 : 16;
            int size = ProgramROMBanks * 16384;
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            this.programROM = new byte[size];
            reader.Read(programROM, 0, size);
            
            //Parse char ROM
            if (this.UsesCharRAM)
            {
                this.charROM = new byte[8192];
            }
            else
            {
                this.charROM = new byte[this.CharROMBanks * 8192];
                reader.Read(this.charROM, 0, this.CharROMBanks * 8192);
            }

            //Dont use CHR RAM in Checksum calculation
            if (this.UsesCharRAM || this.CharROMBanks == 0)
            {
                this.Checksum = BitConverter.ToString(SHA1.ComputeHash(this.programROM)).Replace("-", "");
            }
            else
            {
                this.Checksum = BitConverter.ToString(SHA1.ComputeHash(this.programROM.Concat(this.charROM).ToArray())).Replace("-", "");
            }
        }

        public byte ReadProgramROM(int address)
        {
            return programROM[address];
        }

        public byte ReadProgramRAM(int address)
        {
            return programRAM[address];
        }

        public void WriteProgramRAM(int address, byte data)
        {
            programRAM[address] = data;
        }

        public byte ReadCharROM(int address)
        {
            return charROM[address];
        }

        public void WriteCharRAM(int address, byte data)
        {
            if (!UsesCharRAM)
            {
                throw new Exception("Attempt to write to ROM");
            }
            charROM[address] = data;
        }

        //For savestates
        public byte[] GetCharRAM()
        {
            if (UsesCharRAM)
            {
                return charROM;
            }
            throw new Exception("Attempted to get char rom as ram");
        }

        public void LoadCharRAM(byte[] RAM)
        {
            if (UsesCharRAM)
            {
                charROM = RAM;
            }
            else
            {
                throw new Exception("Attempted to load char rom as ram");
            }
        }
    }
}
