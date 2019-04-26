using System;

namespace LotusNES
{
    [Serializable]
    public class DMCChannel
    {
        public bool Enabled { get; set; }

        public bool BlockIRQ { get; set; }
        public bool Loop { get; set; }

        public uint TimerPeriod { get; set; }
        public byte OutputValue { get; set; }
        public ushort SampleAddress { get; set; }
        public ushort SampleLength { get; set; }

        //TODO: The rest

        public byte Sample()
        {
            return 0;
        }

        public void ClockTimer()
        { 

        }
    }
}
