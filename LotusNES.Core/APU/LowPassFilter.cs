using System;

namespace LotusNES.Core
{
    [Serializable]
    public class LowPassFilter : Filter
    {
        public LowPassFilter(float cutoffFrequency, float sampleRate)
            : base(cutoffFrequency, sampleRate)
        {
            this.alpha = dt / (rc + dt);
        }

        public override float Apply(float sampleX)
        {
            //https://en.wikipedia.org/wiki/Low-pass_filter#Discrete-time_realization
            //for i from 1 to n
            //    y[i] := α * x[i] + (1-α) * y[i-1]
            float sampleY = alpha * sampleX + (1f - alpha) * previousSampleY;
            previousSampleX = sampleX;
            previousSampleY = sampleY;
            return sampleY;
        }
    }
}
