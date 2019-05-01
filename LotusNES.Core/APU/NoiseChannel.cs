using System;

namespace LotusNES.Core
{
    [Serializable]
    public class NoiseChannel
    {
        public bool Enabled { get; set; }

        public Envelope Envelope { get; private set; }
        public LengthCounter LengthCounter { get; private set; }

        public bool Mode { get; set; }
        private ushort shiftRegister;

        public ushort TimerPeriod { get; set; }
        private ushort timerValue;      

        public NoiseChannel()
        {
            this.shiftRegister = 1;

            this.Envelope = new Envelope();
            this.LengthCounter = new LengthCounter();
        }

        public byte Sample()
        {
            if (!Enabled || LengthCounter.Value <= 0 || (shiftRegister & 1) != 0)
            {
                return 0;
            }
            else
            {
                if (Envelope.Enabled)
                {
                    return Envelope.Value;
                }
                else
                {
                    return Envelope.Period;
                }
            }
        }

        public void ClockTimer()
        {
            if (timerValue > 0)
            {
                timerValue--;
            }
            else
            {
                timerValue = TimerPeriod;

                int feedback = (shiftRegister ^ (shiftRegister >> (Mode ? 6 : 1))) & 1;

                shiftRegister >>= 1;
                shiftRegister |= (ushort)(feedback << 14);
            }
        }
    }
}
