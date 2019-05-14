using System;
using System.Collections.Generic;
using System.IO;

namespace LotusNES.Core
{
    public class RewindManager
    {
        private Stack<RewindBlock> blocks;

        public int FrameOffset
        {
            get
            {
                return blocks.Peek().FramePointer;
            }
        }

        public RewindManager()
        {
            blocks = new Stack<RewindBlock>();
        }

        public void Reset()
        {
            blocks.Clear();
        }

        //call this on end of vblank, frame start
        public void PushFrame(byte[] input, byte[] frame)
        {
            if (blocks.Count == 0 || blocks.Peek().PushFrame(input, Compression.Compress(frame)))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    Emulator.WriteStateToStream(ms);
                    blocks.Push(new RewindBlock(ms.ToArray()));
                }
            }
        }

        public (byte[] input, byte[] frame, bool startReached) PopFrame()
        {
            var frame = blocks.Peek().PopFrame();
            if (frame.startReached)
            {
                if (blocks.Count == 1)
                {
                    return (frame.input, Compression.Decompress(frame.frame, 240 * 256), true);
                }
                blocks.Pop();
                using (MemoryStream ms = new MemoryStream(blocks.Peek().SaveState))
                {
                    Emulator.ReadStateFromStream(ms);
                }                
            }
            return (frame.input, Compression.Decompress(frame.frame, 240 * 256), false);
        }

        public (byte[] input, byte[] frame) GetFrameRelative(int offset)
        {
            var result = blocks.Peek().GetFrame(offset);
            result.frame = Compression.Decompress(result.frame, 240 * 256);
            return result;
        }

        public void SetFrameRelative(int offset, byte[] input, byte[] frame)
        {
            blocks.Peek().SetFrame(offset, input, Compression.Compress(frame));
        }
    }
}
