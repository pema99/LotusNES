using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace LotusNES.Frontend
{
    public class FastBitmap : IDisposable
    {
        public Bitmap Bitmap { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        public bool Disposed { get; private set; }

        private int[] colors;
        private GCHandle handle;

        public FastBitmap(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.colors = new int[width * height];
            this.handle = GCHandle.Alloc(colors, GCHandleType.Pinned);
            this.Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, handle.AddrOfPinnedObject());
        }

        public void SetPixel(int x, int y, Color colour)
        {
            colors[x + y * Width] = colour.ToArgb();
        }

        public Color GetPixel(int x, int y)
        {
            return Color.FromArgb(colors[x + y * Width]);
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                Disposed = true;
                Bitmap.Dispose();
                handle.Free();
            }
        }

        public static implicit operator Bitmap(FastBitmap fb)
        {
            return fb.Bitmap;
        }
    }
}
