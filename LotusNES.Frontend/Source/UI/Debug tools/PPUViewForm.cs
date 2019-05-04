using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LotusNES.Core;

namespace LotusNES.Frontend
{
    public partial class PPUViewForm : DoubleBufferedForm
    {
        private FastBitmap patternTableA;
        private FastBitmap patternTableB;
        private int selectedPalette;

        public PPUViewForm()
        {       
            InitializeComponent();

            patternTableA = new FastBitmap(128, 128);
            patternTableB = new FastBitmap(128, 128);
        }

        private void PPUViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void TimerUpdate_Tick(object sender, EventArgs e)
        {
            if (Visible && Emulator.Running)
            {
                DrawPatternTable(patternTableA, 0);
                DrawPatternTable(patternTableB, 1);
                PicturePatternTableA.Invalidate();
                PicturePatternTableB.Invalidate();
                PicturePalettes.Invalidate();
            }
        }

        private void DrawPatternTable(FastBitmap bmp, int patternTableNum)
        {
            //Loop through each tile
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    //Loop through each pixel
                    int tileNum = i + 16 * j;
                    for (int x = 0; x < 8; x++)
                    {
                        for (int y = 0; y < 8; y++)
                        {
                            byte colLSB = Emulator.PPU.Memory.Read((ushort)(0x1000 * patternTableNum + tileNum * 16 + y));
                            byte colMSB = Emulator.PPU.Memory.Read((ushort)(0x1000 * patternTableNum + tileNum * 16 + 8 + y));
                            int colNum = ((colLSB >> (7 - x)) & 0b01) | ((colMSB >> (7 - x) << 1) & 0b10);
                            byte pixel = Emulator.PPU.Memory.Read((ushort)(0x3F00 + (selectedPalette * 4) + colNum));
                            pixel &= 0b111111;
                            
                            bmp.SetPixel(i * 8 + x, j * 8 + y, PaletteMap.MapNET(pixel));
                        }
                    }
                }
            }
        }

        private void PicturePatternTableA_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(patternTableA, 0, 0, PicturePatternTableA.Width, PicturePatternTableA.Height);
        }

        private void PicturePatternTableB_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(patternTableB, 0, 0, PicturePatternTableB.Width, PicturePatternTableB.Height);
        }

        private void PicturePalettes_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            selectedPalette = me.X / (PicturePalettes.Width / 4);
            if (me.Y > PicturePalettes.Height / 2)
            {
                selectedPalette += 4;
            }
        }

        private void PicturePalettes_Paint(object sender, PaintEventArgs e)
        {
            if (Emulator.Running)
            {
                int tileW = PicturePalettes.Width / 16;
                int tileH = PicturePalettes.Height / 2;
                for (int i = 0; i < 16; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        using (Pen p = new Pen(Color.White))
                        {
                            byte paletteCol = Emulator.PPU.Memory.Read((ushort)(0x3F00 + i + j * 16));
                            paletteCol &= 0b111111;
                            p.Color = PaletteMap.MapNET(paletteCol);
                            e.Graphics.FillRectangle(p.Brush, i * tileW, j * tileH, tileW, tileH);
                        }
                    }
                }
            }
        }
    }
}
