namespace LotusNES.Frontend
{
    partial class PPUViewForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PPUViewForm));
            this.PicturePatternTableA = new System.Windows.Forms.PictureBox();
            this.PicturePatternTableB = new System.Windows.Forms.PictureBox();
            this.TimerUpdate = new System.Windows.Forms.Timer(this.components);
            this.PicturePalettes = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PicturePatternTableA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicturePatternTableB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicturePalettes)).BeginInit();
            this.SuspendLayout();
            // 
            // PicturePatternTableA
            // 
            this.PicturePatternTableA.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.PicturePatternTableA.Location = new System.Drawing.Point(12, 12);
            this.PicturePatternTableA.Name = "PicturePatternTableA";
            this.PicturePatternTableA.Size = new System.Drawing.Size(256, 256);
            this.PicturePatternTableA.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PicturePatternTableA.TabIndex = 0;
            this.PicturePatternTableA.TabStop = false;
            this.PicturePatternTableA.Paint += new System.Windows.Forms.PaintEventHandler(this.PicturePatternTableA_Paint);
            // 
            // PicturePatternTableB
            // 
            this.PicturePatternTableB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PicturePatternTableB.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.PicturePatternTableB.Location = new System.Drawing.Point(268, 12);
            this.PicturePatternTableB.Name = "PicturePatternTableB";
            this.PicturePatternTableB.Size = new System.Drawing.Size(256, 256);
            this.PicturePatternTableB.TabIndex = 1;
            this.PicturePatternTableB.TabStop = false;
            this.PicturePatternTableB.Paint += new System.Windows.Forms.PaintEventHandler(this.PicturePatternTableB_Paint);
            // 
            // TimerUpdate
            // 
            this.TimerUpdate.Enabled = true;
            this.TimerUpdate.Tick += new System.EventHandler(this.TimerUpdate_Tick);
            // 
            // PicturePalettes
            // 
            this.PicturePalettes.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.PicturePalettes.Location = new System.Drawing.Point(12, 276);
            this.PicturePalettes.Name = "PicturePalettes";
            this.PicturePalettes.Size = new System.Drawing.Size(512, 80);
            this.PicturePalettes.TabIndex = 2;
            this.PicturePalettes.TabStop = false;
            this.PicturePalettes.Click += new System.EventHandler(this.PicturePalettes_Click);
            this.PicturePalettes.Paint += new System.Windows.Forms.PaintEventHandler(this.PicturePalettes_Paint);
            // 
            // PPUViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(536, 368);
            this.Controls.Add(this.PicturePalettes);
            this.Controls.Add(this.PicturePatternTableB);
            this.Controls.Add(this.PicturePatternTableA);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "PPUViewForm";
            this.Text = "PPU View";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PPUViewForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.PicturePatternTableA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicturePatternTableB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PicturePalettes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox PicturePatternTableA;
        private System.Windows.Forms.PictureBox PicturePatternTableB;
        private System.Windows.Forms.Timer TimerUpdate;
        private System.Windows.Forms.PictureBox PicturePalettes;
    }
}