using System;
using System.Windows.Forms;
using LotusNES.Core;

namespace LotusNES.Frontend
{
    public static class Program
    {
        //UI
        public static MainForm Form;
        public static MainViewport Viewport;

        [STAThread]
        static void Main(string[] args)
        {
            //Init emulator
            Emulator emu = new Emulator();
            emu.ErrorHandler = (e) => {
                MessageBox.Show(
                    string.Format("Emulation error occurred: {0}", e.Message),
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            };

            //Parse arguments
            bool headless = false;
            if (args.Length > 0)
            {
                int i = 0;
                while (i < args.Length)
                {
                    switch (args[i])
                    {
                        case "-f":
                            i++;
                            emu.LoadROM(args[i]);
                            break;

                        case "-h":
                            headless = true;
                            break;
                    }
                    i++;
                }
            }

            //Init UI
            if (!headless)
            {
                Application.EnableVisualStyles();
                Form = new MainForm(emu);
                Viewport = new MainViewport(emu);
                Form.Show();
                Viewport.Run();
            }
        }
    }
}
