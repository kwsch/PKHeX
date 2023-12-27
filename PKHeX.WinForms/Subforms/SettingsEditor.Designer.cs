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
            FLP_Blank = new System.Windows.Forms.FlowLayoutPanel();
            L_Blank = new System.Windows.Forms.Label();
            CB_Blank = new System.Windows.Forms.ComboBox();
            tabControl1 = new System.Windows.Forms.TabControl();
            B_Reset = new System.Windows.Forms.Button();
            FLP_Blank.SuspendLayout();
            SuspendLayout();
            // 
            // FLP_Blank
            // 
            FLP_Blank.Controls.Add(L_Blank);
            FLP_Blank.Controls.Add(CB_Blank);
            FLP_Blank.Dock = System.Windows.Forms.DockStyle.Top;
            FLP_Blank.Location = new System.Drawing.Point(0, 0);
            FLP_Blank.Name = "FLP_Blank";
            FLP_Blank.Size = new System.Drawing.Size(494, 27);
            FLP_Blank.TabIndex = 1;
            // 
            // L_Blank
            // 
            L_Blank.AutoSize = true;
            L_Blank.Location = new System.Drawing.Point(3, 0);
            L_Blank.Name = "L_Blank";
            L_Blank.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            L_Blank.Size = new System.Drawing.Size(107, 21);
            L_Blank.TabIndex = 0;
            L_Blank.Text = "Blank Save Version:";
            // 
            // CB_Blank
            // 
            CB_Blank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Blank.FormattingEnabled = true;
            CB_Blank.Location = new System.Drawing.Point(116, 3);
            CB_Blank.Name = "CB_Blank";
            CB_Blank.Size = new System.Drawing.Size(180, 23);
            CB_Blank.TabIndex = 1;
            // 
            // tabControl1
            // 
            tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl1.Location = new System.Drawing.Point(0, 27);
            tabControl1.Multiline = true;
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(494, 309);
            tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            tabControl1.TabIndex = 3;
            // 
            // B_Reset
            // 
            B_Reset.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            B_Reset.Location = new System.Drawing.Point(416, 2);
            B_Reset.Name = "B_Reset";
            B_Reset.Size = new System.Drawing.Size(75, 23);
            B_Reset.TabIndex = 4;
            B_Reset.Text = "Reset All";
            B_Reset.UseVisualStyleBackColor = true;
            // 
            // SettingsEditor
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(494, 336);
            Controls.Add(B_Reset);
            Controls.Add(tabControl1);
            Controls.Add(FLP_Blank);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            Icon = Properties.Resources.Icon;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(510, 375);
            Name = "SettingsEditor";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Settings";
            KeyDown += SettingsEditor_KeyDown;
            FLP_Blank.ResumeLayout(false);
            FLP_Blank.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel FLP_Blank;
        private System.Windows.Forms.Label L_Blank;
        private System.Windows.Forms.ComboBox CB_Blank;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Button B_Reset;
    }
}
