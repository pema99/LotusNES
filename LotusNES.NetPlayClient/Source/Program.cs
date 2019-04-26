using System;

namespace LotusNES.NetPlayClient
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
