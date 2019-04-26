using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Lotus6502
{
    [Serializable]
    public abstract class Mapper
    {
        public VRAMMirroringMode VRAMMirroring { get; protected set; }

        protected Mapper(VRAMMirroringMode vramMirroring)
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
        public static Mapper Create(GamePak gamePak)
        {
            //Check special cases using checksum
            switch (gamePak.Checksum)
            {
                //StarTropics 1 and 2
                case "74C53FE9AC779F146C59AC01E701C9BF912B3C7B":
                case "88F0DFA7F034B95308BC04E2A0A38FFB2320268B":
                case "FCB1EF7398B842EBD28C3227852D7A132CE7B887":
                    return new MMC3(true);

                default:
                    break;
            }

            //First see if checksum is in database
            int mapperID = gamePak.MapperID;
            if (CartDB.ContainsKey(gamePak.Checksum))
            {
                mapperID = CartDB[gamePak.Checksum];
            }

            //Else default to mapper ID from file
            switch (mapperID)
            {
                case 0:
                    return new NROM();

                case 1:
                    return new MMC1();

                case 2:
                case 71:
                    return new UxROM();

                case 3:
                    return new CNROM();

                case 4:
                    return new MMC3();

                case 7:
                    return new AxROM();

                case 66:
                case 11:
                    return new GxROM();

                default:
                    throw new Exception("Unimplemented mapper");
            }
        }
    }
}
