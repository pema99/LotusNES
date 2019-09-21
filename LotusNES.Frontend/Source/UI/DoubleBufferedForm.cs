using LotusNES.Core;
using System;
using System.Windows.Forms;

namespace LotusNES.Frontend
{
    public class DoubleBufferedForm : Form
    {
        protected Emulator emu;

        [Obsolete]
        private DoubleBufferedForm()
        {
            throw new Exception("Invalid constructor");
        }

        public DoubleBufferedForm(Emulator emu)
        {
            this.emu = emu;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
    }
}
