﻿using System;
using System.Windows.Forms;
using LotusNES.Core;

namespace LotusNES.Frontend
{
    public partial class MainForm : DoubleBufferedForm
    {
        private GameGenieForm gameGenieForm;
        private PPUViewForm ppuViewForm;
        private NameTableForm nameTableForm;

        private InputForm inputForm;

        public MainForm(Emulator emu)
            : base(emu)
        {
            gameGenieForm = new GameGenieForm(emu);
            ppuViewForm = new PPUViewForm(emu);
            nameTableForm = new NameTableForm(emu);

            inputForm = new InputForm(emu);

            InitializeComponent();
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

        private void MainForm_Load(object sender, EventArgs e)
        {
            ComboResolution.SelectedIndex = 0;
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            LabelStatus.Text = emu.CPU.DebugString();
        }

        private void CheckPause_CheckedChanged(object sender, EventArgs e)
        {
            CheckPause.Text = CheckPause.Checked ? "Unpause" : "Pause";
            emu.Pause = CheckPause.Checked;
        }

        private void CheckTurbo_CheckedChanged(object sender, EventArgs e)
        {
            CheckTurbo.Text = CheckTurbo.Checked ? "No fast :(" : "Maximum Overdrive";
            emu.Turbo = CheckTurbo.Checked;
            SliderSpeed.Enabled = !CheckTurbo.Checked;
        }

        private void ButtonNetPlay_Click(object sender, EventArgs e)
        {
            if (emu.NetPlayServer.Running)
            {
                emu.NetPlayServer.Stop();
                ButtonNetPlay.Text = "Start NetPlay";
            }
            else
            {
                emu.NetPlayServer.Start((int)NumberPort.Value);
                ButtonNetPlay.Text = "Stop NetPlay";
            }
        }

        private void ButtonLoadROM_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = "iNES ROM (*.nes)|*.nes|All files (*.*)|*.*";

            if (OFD.ShowDialog() == DialogResult.OK)
            {
                emu.LoadROM(OFD.FileName);
                nameTableForm.UpdateMirroring = true;
            }      
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.Viewport.ExitRequest = true;
            //e.Cancel = true;
        }

        private void ComboResolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.Viewport.SetScreenScale(ComboResolution.SelectedIndex + 1);
        }

        private void SliderSpeed_Scroll(object sender, EventArgs e)
        {
            LabelSpeed.Text = string.Format("Emulation speed {0}%", SliderSpeed.Value * 25);
            emu.Speed = SliderSpeed.Value * 0.25;
        }

        private void SliderVolume_Scroll(object sender, EventArgs e)
        {
            emu.DisableAPU = SliderVolume.Value == 0;
            Program.Viewport.SetVolume(SliderVolume.Value / 100f);
            LabelVolume.Text = string.Format("Sound volume {0}%", SliderVolume.Value);
        }

        private void ButtonSaveState_Click(object sender, EventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.Filter = "Savestate file (*.sav)|*.sav|All files (*.*)|*.*";

            if (SFD.ShowDialog() == DialogResult.OK)
            {
                emu.SaveStateToFile(SFD.FileName);
            }
        }

        private void ButtonLoadState_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = "Savestate file (*.sav)|*.sav|All files (*.*)|*.*";

            if (OFD.ShowDialog() == DialogResult.OK)
            {
                emu.LoadStateFromFile(OFD.FileName);
            }
        }

        private void CheckFilters_CheckedChanged(object sender, EventArgs e)
        {
            CheckFilters.Text = CheckFilters.Checked ? "Disable APU filter chain" : "Enable APU filter chain";
            emu.APU.EnableFilters = CheckFilters.Checked;
        }

        private void ButtonGameGenie_Click(object sender, EventArgs e)
        {
            if (gameGenieForm.Visible)
            {
                gameGenieForm.WindowState = FormWindowState.Normal;
            }
            else
            {
                gameGenieForm.Show();
            }
        }

        private void ButtonPPUView_Click(object sender, EventArgs e)
        {
            if (ppuViewForm.Visible)
            {
                ppuViewForm.WindowState = FormWindowState.Normal;
            }
            else
            {
                ppuViewForm.Show();
            }
        }

        private void ButtonNametables_Click(object sender, EventArgs e)
        {
            if (nameTableForm.Visible)
            {
                nameTableForm.WindowState = FormWindowState.Normal;
            }
            else
            {
                nameTableForm.Show();
            }
        }

        private void CheckMute_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckMute.Checked)
            {
                CheckMute.Text = "Enable APU (may lower performance)";
                emu.DisableAPU = true;
                SliderVolume.Enabled = false;
            }
            else
            {
                CheckMute.Text = "Disable APU (may improve performance)";
                emu.DisableAPU = SliderVolume.Value == 0;
                SliderVolume.Enabled = true;
            }
        }

        private void ButtonInput_Click(object sender, EventArgs e)
        {
            if (inputForm.Visible)
            {
                inputForm.WindowState = FormWindowState.Normal;
            }
            else
            {
                inputForm.Show();
            }
        }

        private void ButtonStartRewind_Click(object sender, EventArgs e)
        {
            emu.StartRewinding();
        }

        private void ButtonStopRewind_Click(object sender, EventArgs e)
        {
            emu.StopRewinding();
        }

        private void CheckTrackHistory_CheckedChanged(object sender, EventArgs e)
        {
            emu.TrackHistory = CheckTrackHistory.Checked;
            ButtonStopRewind.Enabled = ButtonStartRewind.Enabled = CheckTrackHistory.Checked;
            if (!CheckTrackHistory.Checked)
            {
                emu.StopRewinding();
            }
        }
    }
}
