using System;

namespace LotusNES
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            Emulator.Initialize();
        }
    }
}
