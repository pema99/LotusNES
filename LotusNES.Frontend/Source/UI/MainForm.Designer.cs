namespace LotusNES.Frontend
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.LabelStatus = new System.Windows.Forms.Label();
            this.CheckPause = new System.Windows.Forms.CheckBox();
            this.TimerUpdate = new System.Windows.Forms.Timer(this.components);
            this.CheckTurbo = new System.Windows.Forms.CheckBox();
            this.ButtonNetPlay = new System.Windows.Forms.Button();
            this.NumberPort = new System.Windows.Forms.NumericUpDown();
            this.LabelPort = new System.Windows.Forms.Label();
            this.TabsMenu = new System.Windows.Forms.TabControl();
            this.TabFile = new System.Windows.Forms.TabPage();
            this.ButtonLoadState = new System.Windows.Forms.Button();
            this.ButtonSaveState = new System.Windows.Forms.Button();
            this.ButtonLoadROM = new System.Windows.Forms.Button();
            this.TabMachine = new System.Windows.Forms.TabPage();
            this.CheckFilters = new System.Windows.Forms.CheckBox();
            this.LabelSpeed = new System.Windows.Forms.Label();
            this.SliderSpeed = new System.Windows.Forms.TrackBar();
            this.TabSettings = new System.Windows.Forms.TabPage();
            this.ButtonGameGenie = new System.Windows.Forms.Button();
            this.LabelVolume = new System.Windows.Forms.Label();
            this.SliderVolume = new System.Windows.Forms.TrackBar();
            this.LabelResolution = new System.Windows.Forms.Label();
            this.ComboResolution = new System.Windows.Forms.ComboBox();
            this.TabNetPlay = new System.Windows.Forms.TabPage();
            this.TabDebug = new System.Windows.Forms.TabPage();
            this.ButtonNametables = new System.Windows.Forms.Button();
            this.ButtonPPUView = new System.Windows.Forms.Button();
            this.CheckMute = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.NumberPort)).BeginInit();
            this.TabsMenu.SuspendLayout();
            this.TabFile.SuspendLayout();
            this.TabMachine.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SliderSpeed)).BeginInit();
            this.TabSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SliderVolume)).BeginInit();
            this.TabNetPlay.SuspendLayout();
            this.TabDebug.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabelStatus
            // 
            this.LabelStatus.AutoSize = true;
            this.LabelStatus.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelStatus.Location = new System.Drawing.Point(3, 3);
            this.LabelStatus.Name = "LabelStatus";
            this.LabelStatus.Size = new System.Drawing.Size(109, 182);
            this.LabelStatus.TabIndex = 0;
            this.LabelStatus.Text = "Opcode\r\nAccumulator\r\nX register\r\nY register\r\nProgram counter\r\nStack pointer\r\n\r\nCa" +
    "rry flag\r\nZero flag\r\nInterrupt toggle\r\nDecimal mode\r\nInterrupt execute\r\nOverflow" +
    "\r\nSign";
            // 
            // CheckPause
            // 
            this.CheckPause.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckPause.AutoSize = true;
            this.CheckPause.Location = new System.Drawing.Point(3, 32);
            this.CheckPause.Name = "CheckPause";
            this.CheckPause.Size = new System.Drawing.Size(47, 23);
            this.CheckPause.TabIndex = 4;
            this.CheckPause.Text = "Pause";
            this.CheckPause.UseVisualStyleBackColor = true;
            this.CheckPause.CheckedChanged += new System.EventHandler(this.CheckPause_CheckedChanged);
            // 
            // TimerUpdate
            // 
            this.TimerUpdate.Enabled = true;
            this.TimerUpdate.Tick += new System.EventHandler(this.UpdateTimer_Tick);
            // 
            // CheckTurbo
            // 
            this.CheckTurbo.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckTurbo.AutoSize = true;
            this.CheckTurbo.Location = new System.Drawing.Point(3, 3);
            this.CheckTurbo.Name = "CheckTurbo";
            this.CheckTurbo.Size = new System.Drawing.Size(110, 23);
            this.CheckTurbo.TabIndex = 6;
            this.CheckTurbo.Text = "Maximum Overdrive";
            this.CheckTurbo.UseVisualStyleBackColor = true;
            this.CheckTurbo.CheckedChanged += new System.EventHandler(this.CheckTurbo_CheckedChanged);
            // 
            // ButtonNetPlay
            // 
            this.ButtonNetPlay.Location = new System.Drawing.Point(6, 29);
            this.ButtonNetPlay.Name = "ButtonNetPlay";
            this.ButtonNetPlay.Size = new System.Drawing.Size(115, 23);
            this.ButtonNetPlay.TabIndex = 8;
            this.ButtonNetPlay.Text = "Start NetPlay";
            this.ButtonNetPlay.UseVisualStyleBackColor = true;
            this.ButtonNetPlay.Click += new System.EventHandler(this.ButtonNetPlay_Click);
            // 
            // NumberPort
            // 
            this.NumberPort.Location = new System.Drawing.Point(38, 3);
            this.NumberPort.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.NumberPort.Name = "NumberPort";
            this.NumberPort.Size = new System.Drawing.Size(83, 20);
            this.NumberPort.TabIndex = 9;
            this.NumberPort.Value = new decimal(new int[] {
            7777,
            0,
            0,
            0});
            // 
            // LabelPort
            // 
            this.LabelPort.AutoSize = true;
            this.LabelPort.Location = new System.Drawing.Point(3, 5);
            this.LabelPort.Name = "LabelPort";
            this.LabelPort.Size = new System.Drawing.Size(29, 13);
            this.LabelPort.TabIndex = 10;
            this.LabelPort.Text = "Port:";
            // 
            // TabsMenu
            // 
            this.TabsMenu.Controls.Add(this.TabFile);
            this.TabsMenu.Controls.Add(this.TabMachine);
            this.TabsMenu.Controls.Add(this.TabSettings);
            this.TabsMenu.Controls.Add(this.TabNetPlay);
            this.TabsMenu.Controls.Add(this.TabDebug);
            this.TabsMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabsMenu.Location = new System.Drawing.Point(0, 0);
            this.TabsMenu.Name = "TabsMenu";
            this.TabsMenu.SelectedIndex = 0;
            this.TabsMenu.Size = new System.Drawing.Size(262, 216);
            this.TabsMenu.TabIndex = 12;
            // 
            // TabFile
            // 
            this.TabFile.BackColor = System.Drawing.Color.WhiteSmoke;
            this.TabFile.Controls.Add(this.ButtonLoadState);
            this.TabFile.Controls.Add(this.ButtonSaveState);
            this.TabFile.Controls.Add(this.ButtonLoadROM);
            this.TabFile.Location = new System.Drawing.Point(4, 22);
            this.TabFile.Name = "TabFile";
            this.TabFile.Padding = new System.Windows.Forms.Padding(3);
            this.TabFile.Size = new System.Drawing.Size(254, 190);
            this.TabFile.TabIndex = 0;
            this.TabFile.Text = "File";
            // 
            // ButtonLoadState
            // 
            this.ButtonLoadState.Location = new System.Drawing.Point(3, 32);
            this.ButtonLoadState.Name = "ButtonLoadState";
            this.ButtonLoadState.Size = new System.Drawing.Size(121, 23);
            this.ButtonLoadState.TabIndex = 13;
            this.ButtonLoadState.Text = "Load state";
            this.ButtonLoadState.UseVisualStyleBackColor = true;
            this.ButtonLoadState.Click += new System.EventHandler(this.ButtonLoadState_Click);
            // 
            // ButtonSaveState
            // 
            this.ButtonSaveState.Location = new System.Drawing.Point(130, 32);
            this.ButtonSaveState.Name = "ButtonSaveState";
            this.ButtonSaveState.Size = new System.Drawing.Size(121, 23);
            this.ButtonSaveState.TabIndex = 12;
            this.ButtonSaveState.Text = "Save state";
            this.ButtonSaveState.UseVisualStyleBackColor = true;
            this.ButtonSaveState.Click += new System.EventHandler(this.ButtonSaveState_Click);
            // 
            // ButtonLoadROM
            // 
            this.ButtonLoadROM.Location = new System.Drawing.Point(3, 3);
            this.ButtonLoadROM.Name = "ButtonLoadROM";
            this.ButtonLoadROM.Size = new System.Drawing.Size(121, 23);
            this.ButtonLoadROM.TabIndex = 11;
            this.ButtonLoadROM.Text = "Load ROM";
            this.ButtonLoadROM.UseVisualStyleBackColor = true;
            this.ButtonLoadROM.Click += new System.EventHandler(this.ButtonLoadROM_Click);
            // 
            // TabMachine
            // 
            this.TabMachine.BackColor = System.Drawing.Color.WhiteSmoke;
            this.TabMachine.Controls.Add(this.CheckFilters);
            this.TabMachine.Controls.Add(this.LabelSpeed);
            this.TabMachine.Controls.Add(this.SliderSpeed);
            this.TabMachine.Controls.Add(this.CheckTurbo);
            this.TabMachine.Controls.Add(this.CheckPause);
            this.TabMachine.Location = new System.Drawing.Point(4, 22);
            this.TabMachine.Name = "TabMachine";
            this.TabMachine.Size = new System.Drawing.Size(254, 190);
            this.TabMachine.TabIndex = 2;
            this.TabMachine.Text = "Machine";
            // 
            // CheckFilters
            // 
            this.CheckFilters.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckFilters.AutoSize = true;
            this.CheckFilters.Location = new System.Drawing.Point(3, 61);
            this.CheckFilters.Name = "CheckFilters";
            this.CheckFilters.Size = new System.Drawing.Size(126, 23);
            this.CheckFilters.TabIndex = 10;
            this.CheckFilters.Text = "Enable APU filter chain";
            this.CheckFilters.UseVisualStyleBackColor = true;
            this.CheckFilters.CheckedChanged += new System.EventHandler(this.CheckFilters_CheckedChanged);
            // 
            // LabelSpeed
            // 
            this.LabelSpeed.AutoSize = true;
            this.LabelSpeed.Location = new System.Drawing.Point(8, 129);
            this.LabelSpeed.Name = "LabelSpeed";
            this.LabelSpeed.Size = new System.Drawing.Size(117, 13);
            this.LabelSpeed.TabIndex = 9;
            this.LabelSpeed.Text = "Emulation speed: 100%";
            // 
            // SliderSpeed
            // 
            this.SliderSpeed.LargeChange = 1;
            this.SliderSpeed.Location = new System.Drawing.Point(0, 145);
            this.SliderSpeed.Maximum = 20;
            this.SliderSpeed.Minimum = 1;
            this.SliderSpeed.Name = "SliderSpeed";
            this.SliderSpeed.Size = new System.Drawing.Size(254, 45);
            this.SliderSpeed.TabIndex = 8;
            this.SliderSpeed.Value = 4;
            this.SliderSpeed.Scroll += new System.EventHandler(this.SliderSpeed_Scroll);
            // 
            // TabSettings
            // 
            this.TabSettings.BackColor = System.Drawing.Color.WhiteSmoke;
            this.TabSettings.Controls.Add(this.CheckMute);
            this.TabSettings.Controls.Add(this.ButtonGameGenie);
            this.TabSettings.Controls.Add(this.LabelVolume);
            this.TabSettings.Controls.Add(this.SliderVolume);
            this.TabSettings.Controls.Add(this.LabelResolution);
            this.TabSettings.Controls.Add(this.ComboResolution);
            this.TabSettings.Location = new System.Drawing.Point(4, 22);
            this.TabSettings.Name = "TabSettings";
            this.TabSettings.Size = new System.Drawing.Size(254, 190);
            this.TabSettings.TabIndex = 4;
            this.TabSettings.Text = "Settings";
            // 
            // ButtonGameGenie
            // 
            this.ButtonGameGenie.Location = new System.Drawing.Point(6, 30);
            this.ButtonGameGenie.Name = "ButtonGameGenie";
            this.ButtonGameGenie.Size = new System.Drawing.Size(110, 23);
            this.ButtonGameGenie.TabIndex = 16;
            this.ButtonGameGenie.Text = "GameGenie menu";
            this.ButtonGameGenie.UseVisualStyleBackColor = true;
            this.ButtonGameGenie.Click += new System.EventHandler(this.ButtonGameGenie_Click);
            // 
            // LabelVolume
            // 
            this.LabelVolume.AutoSize = true;
            this.LabelVolume.Location = new System.Drawing.Point(3, 129);
            this.LabelVolume.Name = "LabelVolume";
            this.LabelVolume.Size = new System.Drawing.Size(107, 13);
            this.LabelVolume.TabIndex = 15;
            this.LabelVolume.Text = "Sound volume: 100%";
            // 
            // SliderVolume
            // 
            this.SliderVolume.Location = new System.Drawing.Point(0, 145);
            this.SliderVolume.Maximum = 100;
            this.SliderVolume.Name = "SliderVolume";
            this.SliderVolume.Size = new System.Drawing.Size(254, 45);
            this.SliderVolume.TabIndex = 14;
            this.SliderVolume.Value = 100;
            this.SliderVolume.Scroll += new System.EventHandler(this.SliderVolume_Scroll);
            // 
            // LabelResolution
            // 
            this.LabelResolution.AutoSize = true;
            this.LabelResolution.Location = new System.Drawing.Point(3, 6);
            this.LabelResolution.Name = "LabelResolution";
            this.LabelResolution.Size = new System.Drawing.Size(60, 13);
            this.LabelResolution.TabIndex = 13;
            this.LabelResolution.Text = "Resolution:";
            // 
            // ComboResolution
            // 
            this.ComboResolution.FormattingEnabled = true;
            this.ComboResolution.Items.AddRange(new object[] {
            "1x",
            "2x",
            "3x",
            "4x",
            "5x",
            "6x",
            "7x",
            "8x",
            "9x",
            "10x"});
            this.ComboResolution.Location = new System.Drawing.Point(69, 3);
            this.ComboResolution.Name = "ComboResolution";
            this.ComboResolution.Size = new System.Drawing.Size(47, 21);
            this.ComboResolution.TabIndex = 12;
            this.ComboResolution.SelectedIndexChanged += new System.EventHandler(this.ComboResolution_SelectedIndexChanged);
            // 
            // TabNetPlay
            // 
            this.TabNetPlay.BackColor = System.Drawing.Color.WhiteSmoke;
            this.TabNetPlay.Controls.Add(this.LabelPort);
            this.TabNetPlay.Controls.Add(this.NumberPort);
            this.TabNetPlay.Controls.Add(this.ButtonNetPlay);
            this.TabNetPlay.Cursor = System.Windows.Forms.Cursors.Default;
            this.TabNetPlay.ForeColor = System.Drawing.SystemColors.ControlText;
            this.TabNetPlay.Location = new System.Drawing.Point(4, 22);
            this.TabNetPlay.Name = "TabNetPlay";
            this.TabNetPlay.Size = new System.Drawing.Size(254, 190);
            this.TabNetPlay.TabIndex = 3;
            this.TabNetPlay.Text = "NetPlay";
            // 
            // TabDebug
            // 
            this.TabDebug.BackColor = System.Drawing.Color.WhiteSmoke;
            this.TabDebug.Controls.Add(this.ButtonNametables);
            this.TabDebug.Controls.Add(this.ButtonPPUView);
            this.TabDebug.Controls.Add(this.LabelStatus);
            this.TabDebug.Location = new System.Drawing.Point(4, 22);
            this.TabDebug.Name = "TabDebug";
            this.TabDebug.Padding = new System.Windows.Forms.Padding(3);
            this.TabDebug.Size = new System.Drawing.Size(254, 190);
            this.TabDebug.TabIndex = 1;
            this.TabDebug.Text = "Debug";
            // 
            // ButtonNametables
            // 
            this.ButtonNametables.Location = new System.Drawing.Point(173, 35);
            this.ButtonNametables.Name = "ButtonNametables";
            this.ButtonNametables.Size = new System.Drawing.Size(75, 23);
            this.ButtonNametables.TabIndex = 2;
            this.ButtonNametables.Text = "Nametables";
            this.ButtonNametables.UseVisualStyleBackColor = true;
            this.ButtonNametables.Click += new System.EventHandler(this.ButtonNametables_Click);
            // 
            // ButtonPPUView
            // 
            this.ButtonPPUView.Location = new System.Drawing.Point(173, 6);
            this.ButtonPPUView.Name = "ButtonPPUView";
            this.ButtonPPUView.Size = new System.Drawing.Size(75, 23);
            this.ButtonPPUView.TabIndex = 1;
            this.ButtonPPUView.Text = "PPU View";
            this.ButtonPPUView.UseVisualStyleBackColor = true;
            this.ButtonPPUView.Click += new System.EventHandler(this.ButtonPPUView_Click);
            // 
            // CheckMute
            // 
            this.CheckMute.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckMute.AutoSize = true;
            this.CheckMute.Location = new System.Drawing.Point(6, 100);
            this.CheckMute.Name = "CheckMute";
            this.CheckMute.Size = new System.Drawing.Size(207, 23);
            this.CheckMute.TabIndex = 17;
            this.CheckMute.Text = "Disable APU (may improve performance)";
            this.CheckMute.UseVisualStyleBackColor = true;
            this.CheckMute.CheckedChanged += new System.EventHandler(this.CheckMute_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(262, 216);
            this.Controls.Add(this.TabsMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "LotusNES";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.NumberPort)).EndInit();
            this.TabsMenu.ResumeLayout(false);
            this.TabFile.ResumeLayout(false);
            this.TabMachine.ResumeLayout(false);
            this.TabMachine.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SliderSpeed)).EndInit();
            this.TabSettings.ResumeLayout(false);
            this.TabSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SliderVolume)).EndInit();
            this.TabNetPlay.ResumeLayout(false);
            this.TabNetPlay.PerformLayout();
            this.TabDebug.ResumeLayout(false);
            this.TabDebug.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label LabelStatus;
        private System.Windows.Forms.Timer TimerUpdate;
        public System.Windows.Forms.CheckBox CheckPause;
        public System.Windows.Forms.CheckBox CheckTurbo;
        private System.Windows.Forms.Button ButtonNetPlay;
        private System.Windows.Forms.NumericUpDown NumberPort;
        private System.Windows.Forms.Label LabelPort;
        private System.Windows.Forms.TabControl TabsMenu;
        private System.Windows.Forms.TabPage TabFile;
        private System.Windows.Forms.TabPage TabDebug;
        private System.Windows.Forms.ComboBox ComboResolution;
        private System.Windows.Forms.Label LabelResolution;
        private System.Windows.Forms.TabPage TabMachine;
        private System.Windows.Forms.Button ButtonLoadROM;
        private System.Windows.Forms.TabPage TabSettings;
        private System.Windows.Forms.TabPage TabNetPlay;
        private System.Windows.Forms.TrackBar SliderSpeed;
        private System.Windows.Forms.Label LabelSpeed;
        private System.Windows.Forms.Label LabelVolume;
        private System.Windows.Forms.TrackBar SliderVolume;
        private System.Windows.Forms.Button ButtonSaveState;
        private System.Windows.Forms.Button ButtonLoadState;
        private System.Windows.Forms.CheckBox CheckFilters;
        private System.Windows.Forms.Button ButtonGameGenie;
        private System.Windows.Forms.Button ButtonPPUView;
        private System.Windows.Forms.Button ButtonNametables;
        private System.Windows.Forms.CheckBox CheckMute;
    }
}

