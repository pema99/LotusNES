namespace LotusNES
{
    partial class GameGenieForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameGenieForm));
            this.ListCodes = new System.Windows.Forms.ListBox();
            this.InputCode = new System.Windows.Forms.TextBox();
            this.ButtonAdd = new System.Windows.Forms.Button();
            this.ButtonRemove = new System.Windows.Forms.Button();
            this.LabelCode = new System.Windows.Forms.Label();
            this.CheckToggle = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // ListCodes
            // 
            this.ListCodes.Enabled = false;
            this.ListCodes.FormattingEnabled = true;
            this.ListCodes.Location = new System.Drawing.Point(127, 9);
            this.ListCodes.Name = "ListCodes";
            this.ListCodes.Size = new System.Drawing.Size(130, 121);
            this.ListCodes.TabIndex = 0;
            // 
            // InputCode
            // 
            this.InputCode.Enabled = false;
            this.InputCode.Location = new System.Drawing.Point(12, 52);
            this.InputCode.MaxLength = 8;
            this.InputCode.Name = "InputCode";
            this.InputCode.Size = new System.Drawing.Size(100, 20);
            this.InputCode.TabIndex = 1;
            // 
            // ButtonAdd
            // 
            this.ButtonAdd.Enabled = false;
            this.ButtonAdd.Location = new System.Drawing.Point(12, 78);
            this.ButtonAdd.Name = "ButtonAdd";
            this.ButtonAdd.Size = new System.Drawing.Size(100, 23);
            this.ButtonAdd.TabIndex = 2;
            this.ButtonAdd.Text = "Add";
            this.ButtonAdd.UseVisualStyleBackColor = true;
            this.ButtonAdd.Click += new System.EventHandler(this.ButtonAdd_Click);
            // 
            // ButtonRemove
            // 
            this.ButtonRemove.Enabled = false;
            this.ButtonRemove.Location = new System.Drawing.Point(12, 107);
            this.ButtonRemove.Name = "ButtonRemove";
            this.ButtonRemove.Size = new System.Drawing.Size(100, 23);
            this.ButtonRemove.TabIndex = 3;
            this.ButtonRemove.Text = "Remove";
            this.ButtonRemove.UseVisualStyleBackColor = true;
            this.ButtonRemove.Click += new System.EventHandler(this.ButtonRemove_Click);
            // 
            // LabelCode
            // 
            this.LabelCode.AutoSize = true;
            this.LabelCode.Enabled = false;
            this.LabelCode.Location = new System.Drawing.Point(9, 36);
            this.LabelCode.Name = "LabelCode";
            this.LabelCode.Size = new System.Drawing.Size(93, 13);
            this.LabelCode.TabIndex = 4;
            this.LabelCode.Text = "GameGenie code:";
            // 
            // CheckToggle
            // 
            this.CheckToggle.AutoSize = true;
            this.CheckToggle.Location = new System.Drawing.Point(13, 9);
            this.CheckToggle.Name = "CheckToggle";
            this.CheckToggle.Size = new System.Drawing.Size(65, 17);
            this.CheckToggle.TabIndex = 5;
            this.CheckToggle.Text = "Enabled";
            this.CheckToggle.UseVisualStyleBackColor = true;
            this.CheckToggle.CheckedChanged += new System.EventHandler(this.CheckToggle_CheckedChanged);
            // 
            // GameGenieForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(266, 140);
            this.Controls.Add(this.CheckToggle);
            this.Controls.Add(this.LabelCode);
            this.Controls.Add(this.ButtonRemove);
            this.Controls.Add(this.ButtonAdd);
            this.Controls.Add(this.InputCode);
            this.Controls.Add(this.ListCodes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GameGenieForm";
            this.Text = "GameGenie menu";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameGenieForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox ListCodes;
        private System.Windows.Forms.TextBox InputCode;
        private System.Windows.Forms.Button ButtonAdd;
        private System.Windows.Forms.Button ButtonRemove;
        private System.Windows.Forms.Label LabelCode;
        private System.Windows.Forms.CheckBox CheckToggle;
    }
}