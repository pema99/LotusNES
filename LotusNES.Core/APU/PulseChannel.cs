using System;

namespace LotusNES.Core
{
    [Serializable]
    public class PulseChannel
    {
        //https://wiki.nesdev.com/w/index.php/APU_Pulse
        private static readonly byte[] PulseDutyLookup =
        {
            0, 1, 0, 0, 0, 0, 0, 0,
            0, 1, 1, 0, 0, 0, 0, 0,
            0, 1, 1, 1, 1, 0, 0, 0,
            1, 0, 0, 1, 1, 1, 1, 1
        };

        public bool Enabled { get; set; }

        public Envelope Envelope { get; private set; }
        public LengthCounter LengthCounter { get; private set; }
        public Sweep Sweep { get; private set; }

        public byte DutySequence { get; set; }
        public byte DutyValue { get; set; }
      
        public ushort TimerPeriod { get; set; }
        private ushort timerValue;

        public bool SecondChannel { get; private set; }

        public PulseChannel(bool SecondChannel = false)
        {
            this.SecondChannel = SecondChannel;

            this.Envelope = new Envelope();
            this.LengthCounter = new LengthCounter();
            this.Sweep = new Sweep();
        }

        public byte Sample()
        {
            byte high = PulseDutyLookup[DutySequence * 8 + DutyValue];
            if (!Enabled || high == 0 || LengthCounter.Value <= 0 || timerValue < 8 || TimerPeriod >= 2048)
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
                DutyValue++;
                DutyValue %= 8;
            }
        }

        public void ClockSweep()
        {
            if (Sweep.Reset)
            {
                Sweep.Value = Sweep.Period;
                Sweep.Value++;
                Sweep.Reset = false;
                return;
            }

            if (Sweep.Value > 0)
            {
                Sweep.Value--;
            }
            else
            {
                Sweep.Value = Sweep.Period;
                Sweep.Value++;
                if (Sweep.Enabled)
                {
                    ushort delta = (ushort)(TimerPeriod >> Sweep.Shift);
                    if (Sweep.Negate)
                    {
                        if (SecondChannel)
                        {
                            TimerPeriod -= delta;
                        }
                        else
                        {
                            TimerPeriod -= (ushort)(delta + 1);
                        }
                    }
                    else
                    {
                        TimerPeriod += delta;
                    }
                }
            }
        }
    }
}
