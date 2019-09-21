using System;

namespace LotusNES.Core
{
    //An emulator component which requires access to other components
    [Serializable]
    public abstract class Component
    {
        [NonSerialized]
        protected Emulator emu;

        public Component(Emulator emu)
        {
            this.emu = emu;
        }

        public void RefreshEmulatorReference(Emulator emu)
        {
            this.emu = emu;
        }
    }
}
