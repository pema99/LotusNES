using LotusNES.Core;
using System;
using System.Windows.Forms;

namespace LotusNES.Frontend
{
    public partial class InputForm : DoubleBufferedForm
    {
        private InputKeyForm inputKeyForm;

        public InputForm(Emulator emu)
            : base(emu)
        {
            inputKeyForm = new InputKeyForm(emu);

            InitializeComponent();
        }

        private void InputForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            inputKeyForm.Hide();
            Hide();
        }

        private void ButtonUp1_Click(object sender, EventArgs e)
        {
            inputKeyForm.RequestKey(0, 4);
        }

        private void ButtonDown1_Click(object sender, EventArgs e)
        {
            inputKeyForm.RequestKey(0, 5);
        }

        private void ButtonLeft1_Click(object sender, EventArgs e)
        {
            inputKeyForm.RequestKey(0, 6);
        }

        private void ButtonRight1_Click(object sender, EventArgs e)
        {
            inputKeyForm.RequestKey(0, 7);
        }

        private void ButtonSelect1_Click(object sender, EventArgs e)
        {
            inputKeyForm.RequestKey(0, 2);
        }

        private void ButtonStart1_Click(object sender, EventArgs e)
        {
            inputKeyForm.RequestKey(0, 3);
        }

        private void ButtonB1_Click(object sender, EventArgs e)
        {
            inputKeyForm.RequestKey(0, 1);
        }

        private void ButtonA1_Click(object sender, EventArgs e)
        {
            inputKeyForm.RequestKey(0, 0);
        }

        private void ButtonUp2_Click(object sender, EventArgs e)
        {
            inputKeyForm.RequestKey(1, 4);
        }

        private void ButtonDown2_Click(object sender, EventArgs e)
        {
            inputKeyForm.RequestKey(1, 5);
        }

        private void ButtonLeft2_Click(object sender, EventArgs e)
        {
            inputKeyForm.RequestKey(1, 6);
        }

        private void ButtonRight2_Click(object sender, EventArgs e)
        {
            inputKeyForm.RequestKey(1, 7);
        }

        private void ButtonSelect2_Click(object sender, EventArgs e)
        {
            inputKeyForm.RequestKey(1, 2);
        }

        private void ButtonStart2_Click(object sender, EventArgs e)
        {
            inputKeyForm.RequestKey(1, 3);
        }

        private void ButtonB2_Click(object sender, EventArgs e)
        {
            inputKeyForm.RequestKey(1, 1);
        }

        private void ButtonA2_Click(object sender, EventArgs e)
        {
            inputKeyForm.RequestKey(1, 0);
        }
    }
}
