using System;

namespace LotusNES.Core
{
    [Serializable]
    public class Sweep
    {
        public bool Enabled { get; set; }
        public byte Period { get; set; }
        public bool Negate { get; set; }
        public byte Shift { get; set; }

        public bool Reset { get; set; }
        public byte Value { get; set; }
    }
}
