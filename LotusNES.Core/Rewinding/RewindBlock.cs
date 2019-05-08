using System;

namespace LotusNES.Core
{
    public class RewindBlock
    {
        public const int FramesPerBlock = 60;

        public byte[] SaveState { get; private set; }
        public int FramePointer { get; private set; }

        private byte[][] inputs;
        private byte[][] frames;

        public RewindBlock(byte[] saveState)
        {
            this.SaveState = saveState;
            this.FramePointer = 0;
            this.inputs = new byte[FramesPerBlock][];
            this.frames = new byte[FramesPerBlock][];
        }

        //return true if block end reached
        public bool PushFrame(byte[] input, byte[] frame)
        {
            inputs[FramePointer] = input;
            frames[FramePointer] = frame; //TODO: Compress framebuffer

            if (++FramePointer >= FramesPerBlock)
            {
                return true;
            }

            return false;
        }

        public (byte[] input, byte[] frame, bool startReached) PopFrame()
        {
            FramePointer--;
            byte[] input = inputs[FramePointer];
            byte[] frame = frames[FramePointer];

            return (input, frame, FramePointer <= 0);
        }

        public void SetFrame(int frameNum, byte[] input, byte[] frame)
        {
            inputs[frameNum] = input;
            frames[frameNum] = frame;
        }

        public (byte[] input, byte[] frame) GetFrame(int frameNum)
        {
            return (inputs[frameNum], frames[frameNum]);
        }
    }
}
