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
            this.FLP_Blank = new System.Windows.Forms.FlowLayoutPanel();
            this.L_Blank = new System.Windows.Forms.Label();
            this.CB_Blank = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.B_Reset = new System.Windows.Forms.Button();
            this.FLP_Blank.SuspendLayout();
            this.SuspendLayout();
            // 
            // FLP_Blank
            // 
            this.FLP_Blank.Controls.Add(this.L_Blank);
            this.FLP_Blank.Controls.Add(this.CB_Blank);
            this.FLP_Blank.Dock = System.Windows.Forms.DockStyle.Top;
            this.FLP_Blank.Location = new System.Drawing.Point(0, 0);
            this.FLP_Blank.Name = "FLP_Blank";
            this.FLP_Blank.Size = new System.Drawing.Size(334, 27);
            this.FLP_Blank.TabIndex = 1;
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
            // tabControl1
            // 
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 27);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(334, 284);
            this.tabControl1.TabIndex = 3;
            // 
            // B_Reset
            // 
            this.B_Reset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Reset.Location = new System.Drawing.Point(256, 2);
            this.B_Reset.Name = "B_Reset";
            this.B_Reset.Size = new System.Drawing.Size(75, 23);
            this.B_Reset.TabIndex = 4;
            this.B_Reset.Text = "Reset All";
            this.B_Reset.UseVisualStyleBackColor = true;
            // 
            // SettingsEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(334, 311);
            this.Controls.Add(this.B_Reset);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.FLP_Blank);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SettingsEditor_KeyDown);
            this.FLP_Blank.ResumeLayout(false);
            this.FLP_Blank.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel FLP_Blank;
        private System.Windows.Forms.Label L_Blank;
        private System.Windows.Forms.ComboBox CB_Blank;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Button B_Reset;
    }
}