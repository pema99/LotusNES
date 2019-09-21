using System;
using System.Windows.Forms;
using LotusNES.Core;

namespace LotusNES.Frontend
{
    public partial class InputKeyForm : DoubleBufferedForm
    {
        private int player;
        private int key;
        private Microsoft.Xna.Framework.Input.Keys lastKey;

        public InputKeyForm(Emulator emu)
            : base(emu)
        {
            InitializeComponent();
        }

        public void RequestKey(int player, int key)
        {
            WindowState = FormWindowState.Normal;
            Show();
            LabelKey.Text = "Key: " + Program.Viewport.Controls[player, key].ToString();
            this.player = player;
            this.key = key;
        }

        private void InputKeyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void InputKeyForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (Enum.TryParse(e.KeyCode.ToString(), out lastKey))
            {
                LabelKey.Text = "Key: " + lastKey.ToString();
            }
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            Program.Viewport.Controls[player, key] = lastKey;
            Hide();
        }
    }
}
