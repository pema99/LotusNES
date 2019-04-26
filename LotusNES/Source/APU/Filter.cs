using System;

namespace LotusNES
{
    public abstract class Filter
    {
        protected float rc;
        protected float dt;
        protected float alpha;

        protected float previousSampleX;
        protected float previousSampleY;

        //https://en.wikipedia.org/wiki/High-pass_filter#Discrete-time_realization
        protected Filter(float cutoffFrequency, float sampleRate)
        {
            //RC = 1 / (2pi*fc)
            this.rc = 1f / (6.28318531f * cutoffFrequency);

            //dt = 1 / s
            this.dt = 1f / sampleRate;

            //Previous sample (filter is continuous)
            this.previousSampleX = 0;
            this.previousSampleY = 0;
        }

        public abstract float Apply(float sampleX);
    }
}
