using System.Windows.Forms;

namespace LotusNES
{
    public partial class GameGenieForm : Form
    {
        public GameGenieForm()
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
            Emulator.GameGenie.Enabled = CheckToggle.Checked;
        }

        private void ButtonAdd_Click(object sender, System.EventArgs e)
        {
            if (Emulator.GameGenie.AddCode(InputCode.Text))
            {
                ListCodes.Items.Add(InputCode.Text);
            }
        }

        private void ButtonRemove_Click(object sender, System.EventArgs e)
        {
            if (Emulator.GameGenie.RemoveCode((string)ListCodes.SelectedItem))
            {
                ListCodes.Items.Remove((string)ListCodes.SelectedItem);
            }
        }
    }
}
