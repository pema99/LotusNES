using System;

namespace Lotus6502
{
    [Serializable]
    public class LengthCounter
    {
        public bool Enabled { get; set; }
        public byte Value { get; set; }

        public void Clock()
        {
            if (Enabled && Value > 0)
            {
                Value--;
            }
        }
    }
}
