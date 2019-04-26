using System;

namespace Lotus6502
{
    [Serializable]
    public class APU
    {
        //Constants
        public const int SampleRate = 60000; //Sample rate for apu slightly higher than audio api sample rate to prevent popping
        public const int BufferSize = 1024;
        public const int APUClockRate = 240;
        private const int CPUClockRate = 1789773;

        //Lookup tables     
        private static readonly byte[] LengthCounterLookup =
        {
            10, 254, 20, 2, 40, 4, 80, 6,
            160, 8, 60, 10, 14, 12, 26, 14,
            12, 16, 24, 18, 48, 20, 96, 22,
            192, 24, 72, 26, 16, 28, 32, 30
        };

        private static readonly byte[] PulseWaveformLookup =
        {
            0b01000000, 0b01100000, 0b01111000, 0b10011111
        };

        private static readonly ushort[] NoisePeriodLookup =
        {
            4, 8, 16, 32, 64, 96, 128, 160,
            202, 254, 380, 508, 762, 1016, 2034, 4068,
        };

        private static readonly uint[] DMCPeriodLookup =
        {
            428, 380, 340, 320, 286, 254, 226, 214,
            190, 160, 142, 128, 106,  84,  72,  54
        };

        private static float[] TndTable = new float[203];
        private static float[] PulseTable = new float[31];
        static APU()
        {
            //https://wiki.nesdev.com/w/index.php/APU_Mixer#Emulation
            for (int i = 0; i < 31; i++)
            {
                PulseTable[i] = 95.52f / (8128f / i + 100f);
            }
            for (int i = 0; i < 203; i++)
            {
                TndTable[i] = 163.67f / (24329f / i + 100f);
            }
        }

        private PulseChannel[] pulse;
        private TriangleChannel triangle;
        private NoiseChannel noise;
        private DMCChannel dmc;

        //Frame Counter register
        private bool frameCounterMode;
        private bool frameCounterBlockIRQ;

        //State
        private int cycles;
        private int frameStep;

        //Filters
        private Filter[] filterChain;
        public bool EnableFilters { get; set; }

        //Output
        private float[] audioBuffer = new float[BufferSize * 5];
        private int audioBufferIndex;

        public void Reset()
        {
            this.pulse = new PulseChannel[] { new PulseChannel(), new PulseChannel(true) };
            this.triangle = new TriangleChannel();
            this.noise = new NoiseChannel();
            this.dmc = new DMCChannel();

            SetRegister(0x4015, 0);
            this.frameCounterBlockIRQ = true;

            //https://wiki.nesdev.com/w/index.php/APU_Mixer
            this.filterChain = new Filter[] { new HighPassFilter(90, 44100), new HighPassFilter(440, 44100), new LowPassFilter(14000, 44100) };

            this.audioBufferIndex = 0;
        }

        //Step one cpu cycle
        public void Step()
        {
            cycles++;
            ClockTimers();

            //240 hz cycle
            if (cycles % (CPUClockRate / APUClockRate) == 0)
            {
                // mode 0:    mode 1:       function
                // ---------  -----------  -----------------------------
                //  - - - f    - - - - -    IRQ (if bit 6 is clear)
                //  - l - l    l - l - -    Length counter and sweep
                //  e e e e    e e e e -    Envelope and linear counter
                if (frameCounterMode)
                {
                    int seq = frameStep % 5;
                    if (seq != 4)
                    {
                        ClockEnvelopes();
                    }
                    if (seq == 0 || seq == 2)
                    {
                        ClockLengthCounters();
                        ClockSweeps();
                    }
                }
                else
                {
                    int seq = frameStep % 4;
                    ClockEnvelopes();
                    if (seq == 1 || seq == 3)
                    {
                        ClockLengthCounters();
                        ClockSweeps();
                    }
                    if (seq == 3 && !frameCounterBlockIRQ)
                    {
                        Emulator.CPU.RequestIRQ();
                    }
                }
                frameStep++;
            }

            if (cycles % (CPUClockRate / SampleRate) == 0)
            {
                Sample();
            }
        }

        private void Sample()
        {
            //https://wiki.nesdev.com/w/index.php/APU_Mixer#Emulation
            float sample = PulseTable[pulse[0].Sample() + pulse[1].Sample()] + 
                      TndTable[3 * triangle.Sample() + 2 * noise.Sample() + dmc.Sample()];

            //Filter chain
            if (EnableFilters)
            {
                for (int i = 0; i < filterChain.Length; i++)
                {
                    sample = filterChain[i].Apply(sample);
                }
            }

            audioBuffer[audioBufferIndex] = sample;

            if (audioBufferIndex < audioBuffer.Length - 1)
            {
                audioBufferIndex++;
            }
        }

        public bool AudioBufferReady()
        {
            return audioBufferIndex >= BufferSize;
        }

        public float[] GetAudioBuffer()
        {
            float[] buffer = new float[BufferSize];
            Array.Copy(audioBuffer, buffer, BufferSize);
            Array.Copy(audioBuffer, BufferSize, audioBuffer, 0, audioBuffer.Length - BufferSize);
            audioBufferIndex -= BufferSize;
            return buffer;
        }

        private void ClockEnvelopes()
        {
            pulse[0].Envelope.Clock();
            pulse[1].Envelope.Clock();
            triangle.LinearCounter.Clock();
            noise.Envelope.Clock();
        }

        private void ClockLengthCounters()
        {
            pulse[0].LengthCounter.Clock();
            pulse[1].LengthCounter.Clock();
            triangle.LengthCounter.Clock();
            noise.LengthCounter.Clock();
        }

        private void ClockSweeps()
        {
            pulse[0].ClockSweep();
            pulse[1].ClockSweep();
        }

        private void ClockTimers()
        {
            triangle.ClockTimer();
            if (cycles % 2 == 1)
            {
                pulse[0].ClockTimer();
                pulse[1].ClockTimer();
                noise.ClockTimer();
                dmc.ClockTimer();
            }
        }

        public void SetRegister(ushort address, byte data)
        {
            int pulseIndex = address > 0x4003 ? 1 : 0;
            switch (address)
            {
                case 0x4000:
                case 0x4004:
                    pulse[pulseIndex].DutyCycle = (byte)((data & 0b11000000) >> 6);
                    pulse[pulseIndex].LengthCounter.Enabled = (data & 0b00100000) == 0;
                    pulse[pulseIndex].Envelope.Loop = (data & 0b00100000) != 0;
                    pulse[pulseIndex].Envelope.Enabled = (data & 0b00010000) == 0;
                    pulse[pulseIndex].Envelope.Period = (byte)(data & 0b00001111);
                    pulse[pulseIndex].Envelope.Reset = true;
                    break;

                case 0x4001:
                case 0x4005:
                    pulse[pulseIndex].Sweep.Enabled = (data & 0b10000000) != 0;
                    pulse[pulseIndex].Sweep.Period = (byte)((data & 0b01110000) >> 4);
                    pulse[pulseIndex].Sweep.Negate = (data & 0b00001000) != 0;
                    pulse[pulseIndex].Sweep.Shift = (byte)(data & 0b00000111);
                    pulse[pulseIndex].Sweep.Reset = true;
                    break;

                case 0x4002:
                case 0x4006:
                    pulse[pulseIndex].TimerPeriod &= 0b1111111100000000;
                    pulse[pulseIndex].TimerPeriod |= data;
                    break;

                case 0x4003:
                case 0x4007:
                    pulse[pulseIndex].TimerPeriod &= 0b000_11111111;
                    pulse[pulseIndex].TimerPeriod |= (ushort)((data & 0b111) << 8);
                    pulse[pulseIndex].LengthCounter.Value = LengthCounterLookup[(data & 0b11111000) >> 3];
                    pulse[pulseIndex].Envelope.Reset = true;
                    pulse[pulseIndex].DutyValue = 0;
                    break;

                case 0x4008:
                    triangle.LengthCounter.Enabled = (data & 0b10000000) == 0;
                    triangle.LinearCounter.Enabled = (data & 0b10000000) == 0;
                    triangle.LinearCounter.Period = (byte)(data & 0b01111111);
                    break;

                case 0x400A:
                    triangle.TimerPeriod &= 0b1111111100000000;
                    triangle.TimerPeriod |= data;
                    break;

                case 0x400B:
                    triangle.TimerPeriod &= 0b000_11111111;
                    triangle.TimerPeriod |= (ushort)((data & 0b111) << 8);
                    triangle.LengthCounter.Value = LengthCounterLookup[(data & 0b11111000) >> 3];
                    triangle.TimerValue = triangle.TimerPeriod;
                    triangle.LinearCounter.Reset = true;
                    break;

                case 0x400C:
                    noise.LengthCounter.Enabled = (data & 0b00100000) == 0;
                    noise.Envelope.Loop = (data & 0b00100000) != 0;
                    noise.Envelope.Enabled = (data & 0b00010000) == 0;
                    noise.Envelope.Period = (byte)(data & 0b00001111);
                    noise.Envelope.Reset = true;
                    break;

                case 0x400E:
                    noise.Mode = (data & 0b10000000) != 0;
                    noise.TimerPeriod = NoisePeriodLookup[data & 0b00001111];
                    break;

                case 0x400F:
                    noise.LengthCounter.Value = LengthCounterLookup[(data & 0b11111000) >> 3];
                    noise.Envelope.Reset = true;
                    break;

                case 0x4010:
                    dmc.BlockIRQ = (data & 0b10000000) == 0;
                    dmc.Loop = (data & 0b01000000) != 0;
                    dmc.TimerPeriod = DMCPeriodLookup[data & 0b00001111];
                    break;

                case 0x4011:
                    dmc.OutputValue = (byte)(data & 0b01111111);
                    break;

                case 0x4012:
                    dmc.SampleAddress = (ushort)(0xC000 + (data << 6));
                    break;

                case 0x4013:
                    dmc.SampleLength = (ushort)((data << 4) + 1);
                    break;

                case 0x4015:
                    //TODO: Writing a zero to any of the channel enable bits will silence that channel and immediately set its length counter to 0.
                    dmc.Enabled = (data & 0b00010000) != 0;
                    noise.Enabled = (data & 0b00001000) != 0;
                    if (!noise.Enabled)
                    {
                        noise.LengthCounter.Value = 0;
                    }
                    triangle.Enabled = (data & 0b00000100) != 0;
                    if (!triangle.Enabled)
                    {
                        triangle.LengthCounter.Value = 0;
                    }
                    pulse[1].Enabled = (data & 0b00000010) != 0;
                    if (!pulse[1].Enabled)
                    {
                        pulse[1].LengthCounter.Value = 0;
                    }
                    pulse[0].Enabled = (data & 0b00000001) != 0;
                    if (!pulse[0].Enabled)
                    {
                        pulse[0].LengthCounter.Value = 0;
                    }

                    dmc.BlockIRQ = false;

                    //TODO: OTher dmc stuff
                    break;

                case 0x4017:
                    frameCounterMode = (data & 0b10000000) != 0;
                    frameCounterBlockIRQ = (data & 0b01000000) != 0;
                    if (frameCounterMode)
                    {
                        ClockTimers();
                        ClockLengthCounters();
                        ClockSweeps();
                    }
                    break;

                default:
                    break;
            }
        }

        public byte GetStatusRegister()
        {
            byte result = 0;

            //Result |= (byte)(DMC STUFF > 0 ? 0b00010000 : 0);
            result |= (byte)(noise.LengthCounter.Value > 0 ? 0b00001000 : 0);
            result |= (byte)(triangle.LengthCounter.Value > 0 ? 0b00000100 : 0);
            result |= (byte)(pulse[1].LengthCounter.Value > 0 ? 0b00000010 : 0);
            result |= (byte)(pulse[0].LengthCounter.Value > 0 ? 0b00000001 : 0);

            result |= (byte)(dmc.BlockIRQ ? 0b10000000 : 0);
            result |= (byte)(frameCounterBlockIRQ ? 0b01000000 : 0);

            frameCounterBlockIRQ = true;

            return result;
        }
    }
}
