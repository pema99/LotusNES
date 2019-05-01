using System;

namespace LotusNES.Core
{
    [Serializable]
    public class Envelope
    {
        public bool Enabled { get; set; }
        public byte Period { get; set; }
        public bool Loop { get; set; }

        public bool Reset { get; set; }    
        public byte Value { get; private set; }
        private byte step;

        public void Clock()
        {
            if (Reset)
            {
                Value = 15;
                step = Period;
                Reset = false;
            }
            else
            {
                if (step > 0)
                {
                    step--;
                }
                else
                {
                    step = Period;
                    if (Value > 0)
                    {
                        Value--;
                    }
                    else if (Loop)
                    {
                        Value = 15;
                    }
                }
            }
        }
    }
}
