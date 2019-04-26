using System;
using System.Collections.Generic;
using System.Linq;

namespace LotusNES
{
    public class GameGenie
    {
        public bool Enabled { get; set; }
        private static readonly char[] HexTable = { 'A', 'P', 'Z', 'L', 'G', 'I', 'T', 'Y', 'E', 'O', 'X', 'U', 'K', 'S', 'V', 'N' };
        private Dictionary<ushort, GameGenieCode> codes;

        public GameGenie()
        {
            this.codes = new Dictionary<ushort, GameGenieCode>();
        }

        public byte Read(ushort address, byte realData)
        {
            if (Enabled && codes.ContainsKey(address))
            {
                GameGenieCode result = codes[address];
                if (result.Code.Length == 6)
                {
                    return result.Data;
                }
                else
                {
                    if (result.Compare == realData)
                    {
                        return result.Data;
                    }
                    else
                    {
                        return realData;
                    }
                }
            }
            return realData;
        }

        public bool AddCode(string code)
        {
            code = code.ToUpper();
            if (codes.Where(x => x.Value.Code == code).Count() == 0)
            {
                if (code.Length >= 6 && code.All(x => HexTable.Contains(x)))
                {
                    char[] codeChars = code.ToCharArray();
                    int n0 = Array.FindIndex(HexTable, x => x == codeChars[0]);
                    int n1 = Array.FindIndex(HexTable, x => x == codeChars[1]);
                    int n2 = Array.FindIndex(HexTable, x => x == codeChars[2]);
                    int n3 = Array.FindIndex(HexTable, x => x == codeChars[3]);
                    int n4 = Array.FindIndex(HexTable, x => x == codeChars[4]);
                    int n5 = Array.FindIndex(HexTable, x => x == codeChars[5]);

                    int address = 0x8000 +
                                ((n3 & 7) << 12)
                              | ((n5 & 7) << 8) | ((n4 & 8) << 8)
                              | ((n2 & 7) << 4) | ((n1 & 8) << 4)
                              |  (n4 & 7)       |  (n3 & 8);

                    if (code.Length == 6)
                    {
                        int data =
                                 ((n1 & 7) << 4) | ((n0 & 8) << 4)
                                | (n0 & 7)       |  (n5 & 8);

                        codes.Add((ushort)address, new GameGenieCode(code, (byte)data));

                        return true;
                    }
                    if (code.Length == 8)
                    {
                        int n6 = Array.FindIndex(HexTable, x => x == codeChars[6]);
                        int n7 = Array.FindIndex(HexTable, x => x == codeChars[7]);

                        int data =
                                 ((n1 & 7) << 4) | ((n0 & 8) << 4)
                                | (n0 & 7)       |  (n7 & 8);

                        int compare =
                                 ((n7 & 7) << 4) | ((n6 & 8) << 4)
                                | (n6 & 7)       |  (n5 & 8);

                        codes.Add((ushort)address, new GameGenieCode(code, (byte)data, (byte)compare));

                        return true;
                    }
                }
            }
            return false;
        }

        public bool RemoveCode(string code)
        {
            if (code == null) return false;

            code = code.ToUpper();
            var result = codes.FirstOrDefault(x => x.Value.Code == code);
            if (result.Value != null)
            {
                codes.Remove(result.Key);
                return true;
            }
            return false;
        }

        public string[] GetCodes()
        {
            List<string> result = new List<string>();
            foreach (var code in codes)
            {
                result.Add(code.Value.Code);
            }
            return result.ToArray();
        }
    }
}
