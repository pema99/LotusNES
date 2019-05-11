using System;
using System.IO;
using System.IO.Compression;

namespace LotusNES.Core
{
    public class Compression
    {
        public static byte[] Compress(byte[] data)
        {
            using (MemoryStream dataStream = new MemoryStream())
            {
                using (GZipStream compressStream = new GZipStream(dataStream, CompressionMode.Compress))
                {
                    compressStream.Write(data, 0, data.Length);
                    compressStream.Flush();
                }

                return dataStream.ToArray();
            }
        }

        public static byte[] Decompress(byte[] data, int length)
        {
            using (var source = new MemoryStream(data))
            {
                using (var dataStream = new GZipStream(source, CompressionMode.Decompress))
                {
                    byte[] decompressedBuffer = new byte[length];
                    dataStream.Read(decompressedBuffer, 0, length);
                    return decompressedBuffer;
                }
            }
        }
    }
}
