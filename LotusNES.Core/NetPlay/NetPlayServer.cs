using System;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;

namespace LotusNES.Core
{
    public class NetPlayServer
    {
        private TcpListener host;
        private TcpClient peer;
        private NetworkStream peerStream;

        public bool Running { get; private set; }

        public NetPlayServer()
        {
            this.Running = false;
        }

        public void Start(int port)
        {
            host = new TcpListener(port);
            host.Start();

            peer = host.AcceptTcpClient();
            peerStream = peer.GetStream();

            Running = true;
        }

        public void Stop()
        {
            peer.Close();
            host.Stop();
            Running = false;
        }

        public bool GetPeerInputAvailable()
        {
            return peerStream.DataAvailable;
        }

        public bool[] GetPeerInput()
        {
            byte buffer = (byte)peerStream.ReadByte();

            bool[] input = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                input[i] = ((buffer >> i) & 1) != 0;
            }

            return input;
        }

        public void SendPeerFrameBuffer()
        {
            //Compress framebuffer
            using (MemoryStream dataStream = new MemoryStream())
            {
                using (GZipStream compressStream = new GZipStream(dataStream, CompressionMode.Compress))
                {
                    compressStream.Write(Emulator.PPU.FrameBuffer, 0, Emulator.PPU.FrameBuffer.Length);
                    compressStream.Flush();
                }

                byte[] compressedFrameBuffer = dataStream.ToArray();

                peerStream.Write(BitConverter.GetBytes(compressedFrameBuffer.Length), 0, 4);

                peerStream.Write(compressedFrameBuffer, 0, compressedFrameBuffer.Length);
            }
        }
    }
}
