using XNAColor = Microsoft.Xna.Framework.Color;
using NETColor = System.Drawing.Color;

namespace LotusNES
{
    public static class PaletteMap
    {
        private static XNAColor[] PaletteXNA;
        private static NETColor[] PaletteNET;

        static PaletteMap()
        {
            PaletteXNA = new XNAColor[]
            {
                new XNAColor(84, 84, 84),
                new XNAColor(0, 30, 116),
                new XNAColor(8, 16, 144),
                new XNAColor(48, 0, 136),
                new XNAColor(68, 0, 100),
                new XNAColor(92, 0, 48),
                new XNAColor(84, 4, 0),
                new XNAColor(60, 24, 0),
                new XNAColor(32, 42, 0),
                new XNAColor(8, 58, 0),
                new XNAColor(0, 64, 0),
                new XNAColor(0, 60, 0),
                new XNAColor(0, 50, 60),
                new XNAColor(0, 0, 0),
                new XNAColor(0, 0, 0),
                new XNAColor(0, 0, 0),
                new XNAColor(152, 150, 152),
                new XNAColor(8, 76, 196),
                new XNAColor(48, 50, 236),
                new XNAColor(92, 30, 228),
                new XNAColor(136, 20, 176),
                new XNAColor(160, 20, 100),
                new XNAColor(152, 34, 32),
                new XNAColor(120, 60, 0),
                new XNAColor(84, 90, 0),
                new XNAColor(40, 114, 0),
                new XNAColor(8, 124, 0),
                new XNAColor(0, 118, 40),
                new XNAColor(0, 102, 120),
                new XNAColor(0, 0, 0),
                new XNAColor(0, 0, 0),
                new XNAColor(0, 0, 0),
                new XNAColor(236, 238, 236),
                new XNAColor(76, 154, 236),
                new XNAColor(120, 124, 236),
                new XNAColor(176, 98, 236),
                new XNAColor(228, 84, 236),
                new XNAColor(236, 88, 180),
                new XNAColor(236, 106, 100),
                new XNAColor(212, 136, 32),
                new XNAColor(160, 170, 0),
                new XNAColor(116, 196, 0),
                new XNAColor(76, 208, 32),
                new XNAColor(56, 204, 108),
                new XNAColor(56, 180, 204),
                new XNAColor(60, 60, 60),
                new XNAColor(0, 0, 0),
                new XNAColor(0, 0, 0),
                new XNAColor(236, 238, 236),
                new XNAColor(168, 204, 236),
                new XNAColor(188, 188, 236),
                new XNAColor(212, 178, 236),
                new XNAColor(236, 174, 236),
                new XNAColor(236, 174, 212),
                new XNAColor(236, 180, 176),
                new XNAColor(228, 196, 144),
                new XNAColor(204, 210, 120),
                new XNAColor(180, 222, 120),
                new XNAColor(168, 226, 144),
                new XNAColor(152, 226, 180),
                new XNAColor(160, 214, 228),
                new XNAColor(160, 162, 160),
                new XNAColor(0, 0, 0),
                new XNAColor(0, 0, 0)
            };

            PaletteNET = new NETColor[PaletteXNA.Length];
            for (int i = 0; i < PaletteNET.Length; i++)
            { 
                PaletteNET[i] = NETColor.FromArgb(PaletteXNA[i].R, PaletteXNA[i].G, PaletteXNA[i].B);
            }
        }

        public static XNAColor MapXNA(byte Color)
        {
            return PaletteXNA[Color];
        }

        public static NETColor MapNET(byte Color)
        {
            return PaletteNET[Color];
        }
    }
}
