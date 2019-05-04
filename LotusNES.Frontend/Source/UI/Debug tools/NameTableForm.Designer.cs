namespace LotusNES.Frontend
{
    partial class NameTableForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NameTableForm));
            this.PictureNameTable = new System.Windows.Forms.PictureBox();
            this.TimerUpdate = new System.Windows.Forms.Timer(this.components);
            this.LabelMirroring = new System.Windows.Forms.Label();
            this.ComboMirroring = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.PictureNameTable)).BeginInit();
            this.SuspendLayout();
            // 
            // PictureNameTable
            // 
            this.PictureNameTable.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.PictureNameTable.Location = new System.Drawing.Point(0, 0);
            this.PictureNameTable.Name = "PictureNameTable";
            this.PictureNameTable.Size = new System.Drawing.Size(512, 480);
            this.PictureNameTable.TabIndex = 0;
            this.PictureNameTable.TabStop = false;
            this.PictureNameTable.Paint += new System.Windows.Forms.PaintEventHandler(this.PictureNameTable_Paint);
            // 
            // TimerUpdate
            // 
            this.TimerUpdate.Enabled = true;
            this.TimerUpdate.Tick += new System.EventHandler(this.TimerUpdate_Tick);
            // 
            // LabelMirroring
            // 
            this.LabelMirroring.AutoSize = true;
            this.LabelMirroring.Location = new System.Drawing.Point(9, 487);
            this.LabelMirroring.Name = "LabelMirroring";
            this.LabelMirroring.Size = new System.Drawing.Size(79, 13);
            this.LabelMirroring.TabIndex = 2;
            this.LabelMirroring.Text = "Mirroring mode:";
            // 
            // ComboMirroring
            // 
            this.ComboMirroring.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboMirroring.FormattingEnabled = true;
            this.ComboMirroring.Items.AddRange(new object[] {
            "Horizontal",
            "Vertical",
            "Single screen, upper",
            "Single screen, lower"});
            this.ComboMirroring.Location = new System.Drawing.Point(12, 503);
            this.ComboMirroring.Name = "ComboMirroring";
            this.ComboMirroring.Size = new System.Drawing.Size(121, 21);
            this.ComboMirroring.TabIndex = 3;
            this.ComboMirroring.SelectedIndexChanged += new System.EventHandler(this.ComboMirroring_SelectedIndexChanged);
            // 
            // NameTableForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(512, 529);
            this.Controls.Add(this.ComboMirroring);
            this.Controls.Add(this.LabelMirroring);
            this.Controls.Add(this.PictureNameTable);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "NameTableForm";
            this.Text = "Nametable View";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NameTableForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.PictureNameTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox PictureNameTable;
        private System.Windows.Forms.Timer TimerUpdate;
        private System.Windows.Forms.Label LabelMirroring;
        private System.Windows.Forms.ComboBox ComboMirroring;
    }
}