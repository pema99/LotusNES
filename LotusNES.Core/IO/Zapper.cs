using System;

namespace LotusNES.Core
{
    public class Zapper
    {
        public bool Trigger { get; set; }
        public bool Light { get; set; }

        public void SetState(bool trigger, bool light)
        {
            Trigger = trigger;
            Light = light;
        }

        public byte ReadControllerRegister()
        {
            byte result = 0;
            result |= (byte)(Light ?   0b00001000 : 0);
            result |= (byte)(Trigger ? 0b00010000 : 0);
            return result;
        }
    }
}
