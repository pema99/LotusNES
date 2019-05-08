using System;

namespace LotusNES.Core
{
    public enum EmulatorState
    {
        Halted,
        Running,
        Rewinding,
        CatchingUp,
        Paused
    }
}
