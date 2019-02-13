namespace PKHeX.WinForms
{
    partial class SettingsEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsEditor));
            this.FLP_Settings = new System.Windows.Forms.FlowLayoutPanel();
            this.FLP_Blank = new System.Windows.Forms.FlowLayoutPanel();
            this.L_Blank = new System.Windows.Forms.Label();
            this.CB_Blank = new System.Windows.Forms.ComboBox();
            this.FLP_Settings.SuspendLayout();
            this.FLP_Blank.SuspendLayout();
            this.SuspendLayout();
            // 
            // FLP_Settings
            // 
            this.FLP_Settings.AutoScroll = true;
            this.FLP_Settings.Controls.Add(this.FLP_Blank);
            this.FLP_Settings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FLP_Settings.Location = new System.Drawing.Point(0, 0);
            this.FLP_Settings.Name = "FLP_Settings";
            this.FLP_Settings.Size = new System.Drawing.Size(334, 431);
            this.FLP_Settings.TabIndex = 0;
            // 
            // FLP_Blank
            // 
            this.FLP_Blank.Controls.Add(this.L_Blank);
            this.FLP_Blank.Controls.Add(this.CB_Blank);
            this.FLP_Blank.Dock = System.Windows.Forms.DockStyle.Top;
            this.FLP_Blank.Location = new System.Drawing.Point(3, 3);
            this.FLP_Blank.Name = "FLP_Blank";
            this.FLP_Blank.Size = new System.Drawing.Size(291, 27);
            this.FLP_Blank.TabIndex = 0;
            // 
            // L_Blank
            // 
            this.L_Blank.AutoSize = true;
            this.L_Blank.Location = new System.Drawing.Point(3, 0);
            this.L_Blank.Name = "L_Blank";
            this.L_Blank.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.L_Blank.Size = new System.Drawing.Size(103, 19);
            this.L_Blank.TabIndex = 0;
            this.L_Blank.Text = "Blank Save Version:";
            // 
            // CB_Blank
            // 
            this.CB_Blank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Blank.FormattingEnabled = true;
            this.CB_Blank.Location = new System.Drawing.Point(112, 3);
            this.CB_Blank.Name = "CB_Blank";
            this.CB_Blank.Size = new System.Drawing.Size(137, 21);
            this.CB_Blank.TabIndex = 1;
            // 
            // SettingsEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(334, 431);
            this.Controls.Add(this.FLP_Settings);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsEditor_FormClosing);
            this.FLP_Settings.ResumeLayout(false);
            this.FLP_Blank.ResumeLayout(false);
            this.FLP_Blank.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel FLP_Settings;
        private System.Windows.Forms.FlowLayoutPanel FLP_Blank;
        private System.Windows.Forms.Label L_Blank;
        private System.Windows.Forms.ComboBox CB_Blank;
    }
}