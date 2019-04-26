using Microsoft.Xna.Framework;
using System;

namespace Lotus6502
{
    public static class PaletteMap
    {
        private static Color[] Palette;

        static PaletteMap()
        {
            Palette = new Color[]
            {
                new Color(84, 84, 84),
                new Color(0, 30, 116),
                new Color(8, 16, 144),
                new Color(48, 0, 136),
                new Color(68, 0, 100),
                new Color(92, 0, 48),
                new Color(84, 4, 0),
                new Color(60, 24, 0),
                new Color(32, 42, 0),
                new Color(8, 58, 0),
                new Color(0, 64, 0),
                new Color(0, 60, 0),
                new Color(0, 50, 60),
                new Color(0, 0, 0),
                new Color(0, 0, 0),
                new Color(0, 0, 0),
                new Color(152, 150, 152),
                new Color(8, 76, 196),
                new Color(48, 50, 236),
                new Color(92, 30, 228),
                new Color(136, 20, 176),
                new Color(160, 20, 100),
                new Color(152, 34, 32),
                new Color(120, 60, 0),
                new Color(84, 90, 0),
                new Color(40, 114, 0),
                new Color(8, 124, 0),
                new Color(0, 118, 40),
                new Color(0, 102, 120),
                new Color(0, 0, 0),
                new Color(0, 0, 0),
                new Color(0, 0, 0),
                new Color(236, 238, 236),
                new Color(76, 154, 236),
                new Color(120, 124, 236),
                new Color(176, 98, 236),
                new Color(228, 84, 236),
                new Color(236, 88, 180),
                new Color(236, 106, 100),
                new Color(212, 136, 32),
                new Color(160, 170, 0),
                new Color(116, 196, 0),
                new Color(76, 208, 32),
                new Color(56, 204, 108),
                new Color(56, 180, 204),
                new Color(60, 60, 60),
                new Color(0, 0, 0),
                new Color(0, 0, 0),
                new Color(236, 238, 236),
                new Color(168, 204, 236),
                new Color(188, 188, 236),
                new Color(212, 178, 236),
                new Color(236, 174, 236),
                new Color(236, 174, 212),
                new Color(236, 180, 176),
                new Color(228, 196, 144),
                new Color(204, 210, 120),
                new Color(180, 222, 120),
                new Color(168, 226, 144),
                new Color(152, 226, 180),
                new Color(160, 214, 228),
                new Color(160, 162, 160),
                new Color(0, 0, 0),
                new Color(0, 0, 0)
            };
        }

        public static Color Map(byte Color)
        {
            return Palette[Color];
        }
    }
}
