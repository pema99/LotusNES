using System;

namespace Lotus6502.NetPlayClient
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new PeerViewport())
                game.Run();
        }
    }
}
