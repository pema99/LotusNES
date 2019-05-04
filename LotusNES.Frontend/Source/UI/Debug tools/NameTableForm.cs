using System;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LotusNES.Core;

namespace LotusNES.Frontend
{
    public partial class NameTableForm : DoubleBufferedForm
    {
        public bool UpdateMirroring { get; set; }
        private FastBitmap nameTables;

        public NameTableForm()
        {
            InitializeComponent();

            nameTables = new FastBitmap(PictureNameTable.Width, PictureNameTable.Height);
        }

        private void TimerUpdate_Tick(object sender, EventArgs e)
        {
            if (Visible && Emulator.Running)
            {
                DrawNameTable(0, 0, 0);
                DrawNameTable(256, 0, 1);
                DrawNameTable(0, 240, 2);
                DrawNameTable(256, 240, 3);
                PictureNameTable.Invalidate();

                if (UpdateMirroring)
                {
                    UpdateMirroring = false;
                    ComboMirroring.SelectedIndex = (int)Emulator.Mapper.VRAMMirroring;
                }
            }
        }

        private void PictureNameTable_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(nameTables, 0, 0, nameTables.Width, nameTables.Height);
        }

        private void DrawNameTable(int offsetX, int offsetY, int nameTableNum)
        {
            //Loop through each tile
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 30; j++)
                {                  
                    //Fetch nametable entry                         Base nametable + offset       + index as 1D
                    int tileNum = Emulator.PPU.Memory.Read((ushort)(0x2000 + nameTableNum * 0x400 + i + 32 * j));
                    //Fetch palette num from attribute table            Base attribute + offset         + index as 1D, scaled down to 8x8
                    byte paletteNum = Emulator.PPU.Memory.Read((ushort)(0x23C0 + (nameTableNum * 0x400) + ((i / 4) + 8 * (j / 4))));
                    //Check which quadrant of palette byte, if lower quadrant shift left 4, if right quadrant shift right 2 
                    paletteNum = (byte)((paletteNum >> (2 * (j & 0b10))) >> (i & 0b10) & 0b11);

                    //Loop through each pixel
                    int backgroundPatternTable = ((Emulator.PPU.GetRegister(0x2000) & 0b00010000) >> 4) * 0x1000;
                    for (int x = 0; x < 8; x++)
                    {
                        for (int y = 0; y < 8; y++)
                        {
                            //Get bitplane bytes
                            byte colLSB = Emulator.PPU.Memory.Read((ushort)(backgroundPatternTable + tileNum * 16 + y));
                            byte colMSB = Emulator.PPU.Memory.Read((ushort)(backgroundPatternTable + tileNum * 16 + 8 + y));
                            //Combine them
                            int colNum = ((colLSB >> (7 - x)) & 0b01) | ((colMSB >> (7 - x) << 1) & 0b10);
                            //Combine with palette to get color
                            byte pixel = Emulator.PPU.Memory.Read((ushort)(0x3F00 + (paletteNum * 4) + colNum));
                            pixel &= 0b111111;

                            //Draw at offset
                            nameTables.SetPixel(offsetX + i * 8 + x, offsetY + j * 8 + y, PaletteMap.MapNET(pixel));
                        }
                    }
                }
            }
        }

        private void ComboMirroring_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Emulator.Running)
            {
                Emulator.Mapper.VRAMMirroring = (VRAMMirroringMode)ComboMirroring.SelectedIndex;
            }
        }

        private void NameTableForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
