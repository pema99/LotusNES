using System;
using System.Collections.Generic;
using System.IO;

namespace LotusNES.Core
{
    public class RewindManager : Component
    {
        private Stack<RewindBlock> blocks;

        public int FrameOffset
        {
            get
            {
                return blocks.Peek().FramePointer;
            }
        }

        public RewindManager(Emulator emu)
            : base(emu)
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
                    emu.WriteStateToStream(ms);
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
                    emu.ReadStateFromStream(ms);
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
