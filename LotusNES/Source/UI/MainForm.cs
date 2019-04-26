using System;
using System.Windows.Forms;

namespace LotusNES
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ComboResolution.SelectedIndex = 0;
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            LabelStatus.Text = Emulator.CPU.DebugString();
        }

        private void CheckPause_CheckedChanged(object sender, EventArgs e)
        {
            CheckPause.Text = CheckPause.Checked ? "Unpause" : "Pause";
            Emulator.Pause = CheckPause.Checked;
        }

        private void CheckTurbo_CheckedChanged(object sender, EventArgs e)
        {
            CheckTurbo.Text = CheckTurbo.Checked ? "No fast :(" : "Maximum Overdrive";
            Emulator.Turbo = CheckTurbo.Checked;
            SliderSpeed.Enabled = !CheckTurbo.Checked;
        }

        private void ButtonNetPlay_Click(object sender, EventArgs e)
        {
            if (Emulator.NetPlayServer.Running)
            {
                Emulator.NetPlayServer.Stop();
                ButtonNetPlay.Text = "Start NetPlay";
            }
            else
            {
                Emulator.NetPlayServer.Start((int)NumberPort.Value);
                ButtonNetPlay.Text = "Stop NetPlay";
            }
        }

        private void ButtonLoadROM_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = "iNES ROM (*.nes)|*.nes|All files (*.*)|*.*";

            if (OFD.ShowDialog() == DialogResult.OK)
            {
                Emulator.LoadROM(OFD.FileName);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Emulator.Viewport.ExitRequest = true;
            //e.Cancel = true;
        }

        private void ComboResolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            Emulator.Viewport.SetScreenScale(ComboResolution.SelectedIndex + 1);
        }

        private void SliderSpeed_Scroll(object sender, EventArgs e)
        {
            LabelSpeed.Text = string.Format("Emulation speed {0}%", SliderSpeed.Value * 25);
            Emulator.Speed = SliderSpeed.Value * 0.25;
        }

        private void SliderVolume_Scroll(object sender, EventArgs e)
        {
            LabelVolume.Text = string.Format("Sound volume {0}%", SliderVolume.Value);
            Emulator.Viewport.SetVolume(SliderVolume.Value / 100f);
        }

        private void ButtonSaveState_Click(object sender, EventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.Filter = "Savestate file (*.sav)|*.sav|All files (*.*)|*.*";

            if (SFD.ShowDialog() == DialogResult.OK)
            {
                Emulator.SaveState(SFD.FileName);
            }
        }

        private void ButtonLoadState_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = "Savestate file (*.sav)|*.sav|All files (*.*)|*.*";

            if (OFD.ShowDialog() == DialogResult.OK)
            {
                Emulator.LoadState(OFD.FileName);
            }
        }

        private void CheckFilters_CheckedChanged(object sender, EventArgs e)
        {
            CheckFilters.Text = CheckFilters.Checked ? "Disable APU filter chain" : "Enable APU filter chain";
            Emulator.APU.EnableFilters = CheckFilters.Checked;
        }
    }
}
