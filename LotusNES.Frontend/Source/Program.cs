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
        static void Main()
        {
            //Init emulator
            Emulator.Initialize();
            Emulator.ErrorHandler = (e) => {
                MessageBox.Show(
                    string.Format("Emulation error occurred: {0}", e.Message),
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            };

            //Init UI
            Application.EnableVisualStyles();
            Form = new MainForm();
            Viewport = new MainViewport();
            Form.Show();
            Viewport.Run();
        }
    }
}
