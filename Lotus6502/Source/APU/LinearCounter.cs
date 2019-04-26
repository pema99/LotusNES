using System;

namespace Lotus6502
{
    [Serializable]
    public class LinearCounter
    {
        public bool Enabled { get; set; }
        public byte Period { get; set; }
        public bool Reset { get; set; }
        public byte Value { get; private set; }

        public void Clock()
        {
            if (Reset)
            {
                Value = Period;
                if (Enabled)
                {
                    Reset = false;
                }
            }
            else
            {
                if (Value > 0)
                {
                    Value--;
                }
            }
        }
    }
}
