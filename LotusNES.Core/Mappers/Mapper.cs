using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace LotusNES.Core
{
    [Serializable]
    public abstract class Mapper : Component
    {
        public VRAMMirroringMode VRAMMirroring { get; set; }

        protected Mapper(Emulator emu, VRAMMirroringMode vramMirroring)
            : base(emu)
        {
            this.VRAMMirroring = vramMirroring;
        }

        //R/W to cartridge
        public abstract byte Read(ushort address);
        public abstract void Write(ushort address, byte data);

        //Some mappers need these
        public virtual void Step() { }

        //Cart database
        private static Dictionary<string, int> CartDB = new Dictionary<string, int>();
        static Mapper()
        {
            if (File.Exists("CartDB.xml"))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("CartDB.xml");

                var games = xmlDoc.GetElementsByTagName("game");
                foreach (XmlNode game in games)
                {
                    foreach (XmlNode cartridge in game.ChildNodes)
                    {
                        if (cartridge.Name == "cartridge")
                        {
                            if (!CartDB.ContainsKey(cartridge.Attributes["sha1"].Value))
                            {
                                CartDB.Add(cartridge.Attributes["sha1"].Value, int.Parse(cartridge.ChildNodes[0].Attributes["mapper"].Value));
                            }
                        }
                    }
                }
            }
        }

        //Mapper list
        public static Mapper Create(Emulator emu)
        {
            //Check special cases using checksum
            switch (emu.GamePak.Checksum)
            {
                //StarTropics 1 and 2
                case "74C53FE9AC779F146C59AC01E701C9BF912B3C7B":
                case "88F0DFA7F034B95308BC04E2A0A38FFB2320268B":
                case "FCB1EF7398B842EBD28C3227852D7A132CE7B887":
                    return new MMC3(emu, true);

                default:
                    break;
            }

            //First see if checksum is in database
            int mapperID = emu.GamePak.MapperID;
            if (CartDB.ContainsKey(emu.GamePak.Checksum))
            {
                mapperID = CartDB[emu.GamePak.Checksum];
            }

            //Else default to mapper ID from file
            switch (mapperID)
            {
                case 0:
                    return new NROM(emu);

                case 1:
                    return new MMC1(emu);

                case 2:
                case 71:
                    return new UxROM(emu);

                case 3:
                    return new CNROM(emu);

                case 4:
                case 206:
                    return new MMC3(emu);

                case 7:
                    return new AxROM(emu);

                case 66:
                case 11:
                    return new GxROM(emu);

                default:
                    throw new Exception("Unimplemented mapper");
            }
        }
    }
}
