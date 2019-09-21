using System.Windows.Forms;
using LotusNES.Core;

namespace LotusNES.Frontend
{
    public partial class GameGenieForm : DoubleBufferedForm
    {
        public GameGenieForm(Emulator emu)
            : base(emu)
        {
            InitializeComponent();
        }

        private void GameGenieForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void CheckToggle_CheckedChanged(object sender, System.EventArgs e)
        {
            foreach (Control control in Controls)
            {
                if (control != CheckToggle)
                {
                    control.Enabled = CheckToggle.Checked;
                }
            }
            emu.GameGenie.Enabled = CheckToggle.Checked;
        }

        private void ButtonAdd_Click(object sender, System.EventArgs e)
        {
            if (emu.GameGenie.AddCode(InputCode.Text))
            {
                ListCodes.Items.Add(InputCode.Text);
            }
        }

        private void ButtonRemove_Click(object sender, System.EventArgs e)
        {
            if (emu.GameGenie.RemoveCode((string)ListCodes.SelectedItem))
            {
                ListCodes.Items.Remove((string)ListCodes.SelectedItem);
            }
        }
    }
}
