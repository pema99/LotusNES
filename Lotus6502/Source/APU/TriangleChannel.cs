using System;

namespace Lotus6502
{
    [Serializable]
    public class TriangleChannel
    {
        private static readonly byte[] TriangleWaveFormLookup =
        {
            15, 14, 13, 12, 11, 10, 9, 8,
            7, 6, 5, 4, 3, 2, 1, 0,
            0, 1, 2, 3, 4, 5, 6, 7,
            8, 9, 10, 11, 12, 13, 14, 15
        };

        public bool Enabled { get; set; }

        public LengthCounter LengthCounter { get; private set; }
        public LinearCounter LinearCounter { get; private set; }

        private byte dutyValue;

        public ushort TimerPeriod { get; set; }
        public ushort TimerValue { get; set; }  

        public TriangleChannel()
        {
            this.LengthCounter = new LengthCounter();
            this.LinearCounter = new LinearCounter();
        }

        public byte Sample()
        {
            if (!Enabled || LengthCounter.Value <= 0 || LinearCounter.Value <= 0)
            {
                return 0;
            }
            else
            {
                return TriangleWaveFormLookup[dutyValue];
            }
        }

        public void ClockTimer()
        {
            if (TimerValue > 0)
            {
                TimerValue--;
            }
            else
            {
                TimerValue = TimerPeriod;
                dutyValue++;
                dutyValue %= 32;
            }
        }
    }
}
