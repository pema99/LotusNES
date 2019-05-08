using System;

namespace LotusNES.Core
{
    [Serializable]
    public class HighPassFilter : Filter
    {
        public HighPassFilter(float cutoffFrequency, float sampleRate)
            : base(cutoffFrequency, sampleRate)
        {
            this.alpha = rc / (rc + dt);
        }

        public override float Apply(float sampleX)
        {
            //https://en.wikipedia.org/wiki/High-pass_filter#Algorithmic_implementation
            //for i from 1 to n
            //    y[i] := α * y[i - 1] + α * (x[i] - x[i - 1])
            float sampleY = alpha * previousSampleY + alpha * (sampleX - previousSampleX);
            previousSampleX = sampleX;
            previousSampleY = sampleY;
            return sampleY;
        }
    }
}
