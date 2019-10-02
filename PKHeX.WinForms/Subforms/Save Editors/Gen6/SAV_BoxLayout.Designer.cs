namespace PKHeX.WinForms
{
    partial class SAV_BoxLayout
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
            this.LB_BoxSelect = new System.Windows.Forms.ListBox();
            this.TB_BoxName = new System.Windows.Forms.TextBox();
            this.L_BoxName = new System.Windows.Forms.Label();
            this.B_Save = new System.Windows.Forms.Button();
            this.B_Cancel = new System.Windows.Forms.Button();
            this.CB_BG = new System.Windows.Forms.ComboBox();
            this.PAN_BG = new System.Windows.Forms.Panel();
            this.FLP_Misc = new System.Windows.Forms.FlowLayoutPanel();
            this.FLP_Unlocked = new System.Windows.Forms.FlowLayoutPanel();
            this.L_Unlocked = new System.Windows.Forms.Label();
            this.CB_Unlocked = new System.Windows.Forms.ComboBox();
            this.FLP_Flags = new System.Windows.Forms.FlowLayoutPanel();
            this.L_Flag = new System.Windows.Forms.Label();
            this.B_Up = new System.Windows.Forms.Button();
            this.B_Down = new System.Windows.Forms.Button();
            this.FLP_Misc.SuspendLayout();
            this.FLP_Unlocked.SuspendLayout();
            this.FLP_Flags.SuspendLayout();
            this.SuspendLayout();
            // 
            // LB_BoxSelect
            // 
            this.LB_BoxSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.LB_BoxSelect.FormattingEnabled = true;
            this.LB_BoxSelect.Items.AddRange(new object[] {
            "Boxes"});
            this.LB_BoxSelect.Location = new System.Drawing.Point(12, 14);
            this.LB_BoxSelect.Name = "LB_BoxSelect";
            this.LB_BoxSelect.Size = new System.Drawing.Size(111, 290);
            this.LB_BoxSelect.TabIndex = 0;
            this.LB_BoxSelect.SelectedIndexChanged += new System.EventHandler(this.ChangeBox);
            // 
            // TB_BoxName
            // 
            this.TB_BoxName.Location = new System.Drawing.Point(158, 35);
            this.TB_BoxName.MaxLength = 15;
            this.TB_BoxName.Name = "TB_BoxName";
            this.TB_BoxName.Size = new System.Drawing.Size(136, 20);
            this.TB_BoxName.TabIndex = 1;
            this.TB_BoxName.WordWrap = false;
            this.TB_BoxName.TextChanged += new System.EventHandler(this.ChangeBoxDetails);
            // 
            // L_BoxName
            // 
            this.L_BoxName.AutoSize = true;
            this.L_BoxName.Location = new System.Drawing.Point(156, 19);
            this.L_BoxName.Name = "L_BoxName";
            this.L_BoxName.Size = new System.Drawing.Size(59, 13);
            this.L_BoxName.TabIndex = 2;
            this.L_BoxName.Text = "Box Name:";
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(328, 255);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(67, 23);
            this.B_Save.TabIndex = 9;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(328, 284);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(67, 23);
            this.B_Cancel.TabIndex = 10;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // CB_BG
            // 
            this.CB_BG.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_BG.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_BG.FormattingEnabled = true;
            this.CB_BG.Location = new System.Drawing.Point(300, 34);
            this.CB_BG.Name = "CB_BG";
            this.CB_BG.Size = new System.Drawing.Size(98, 21);
            this.CB_BG.TabIndex = 13;
            this.CB_BG.SelectedIndexChanged += new System.EventHandler(this.ChangeBoxBackground);
            // 
            // PAN_BG
            // 
            this.PAN_BG.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PAN_BG.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.PAN_BG.Location = new System.Drawing.Point(126, 61);
            this.PAN_BG.Name = "PAN_BG";
            this.PAN_BG.Size = new System.Drawing.Size(272, 160);
            this.PAN_BG.TabIndex = 14;
            // 
            // FLP_Misc
            // 
            this.FLP_Misc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_Misc.Controls.Add(this.FLP_Unlocked);
            this.FLP_Misc.Controls.Add(this.FLP_Flags);
            this.FLP_Misc.Location = new System.Drawing.Point(129, 234);
            this.FLP_Misc.Name = "FLP_Misc";
            this.FLP_Misc.Size = new System.Drawing.Size(193, 73);
            this.FLP_Misc.TabIndex = 15;
            // 
            // FLP_Unlocked
            // 
            this.FLP_Unlocked.Controls.Add(this.L_Unlocked);
            this.FLP_Unlocked.Controls.Add(this.CB_Unlocked);
            this.FLP_Unlocked.Location = new System.Drawing.Point(3, 3);
            this.FLP_Unlocked.Name = "FLP_Unlocked";
            this.FLP_Unlocked.Size = new System.Drawing.Size(185, 25);
            this.FLP_Unlocked.TabIndex = 16;
            // 
            // L_Unlocked
            // 
            this.L_Unlocked.Location = new System.Drawing.Point(3, 0);
            this.L_Unlocked.Name = "L_Unlocked";
            this.L_Unlocked.Size = new System.Drawing.Size(70, 21);
            this.L_Unlocked.TabIndex = 1;
            this.L_Unlocked.Text = "Unlocked:";
            this.L_Unlocked.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_Unlocked
            // 
            this.CB_Unlocked.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Unlocked.FormattingEnabled = true;
            this.CB_Unlocked.Location = new System.Drawing.Point(76, 0);
            this.CB_Unlocked.Margin = new System.Windows.Forms.Padding(0);
            this.CB_Unlocked.Name = "CB_Unlocked";
            this.CB_Unlocked.Size = new System.Drawing.Size(40, 21);
            this.CB_Unlocked.TabIndex = 0;
            // 
            // FLP_Flags
            // 
            this.FLP_Flags.Controls.Add(this.L_Flag);
            this.FLP_Flags.Location = new System.Drawing.Point(3, 34);
            this.FLP_Flags.Name = "FLP_Flags";
            this.FLP_Flags.Size = new System.Drawing.Size(185, 25);
            this.FLP_Flags.TabIndex = 17;
            // 
            // L_Flag
            // 
            this.L_Flag.Location = new System.Drawing.Point(3, 0);
            this.L_Flag.Name = "L_Flag";
            this.L_Flag.Size = new System.Drawing.Size(70, 21);
            this.L_Flag.TabIndex = 1;
            this.L_Flag.Text = "Flags:";
            this.L_Flag.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // B_Up
            // 
            this.B_Up.Location = new System.Drawing.Point(124, 13);
            this.B_Up.Name = "B_Up";
            this.B_Up.Size = new System.Drawing.Size(23, 23);
            this.B_Up.TabIndex = 16;
            this.B_Up.Text = "^";
            this.B_Up.UseVisualStyleBackColor = true;
            this.B_Up.Click += new System.EventHandler(this.MoveBox);
            // 
            // B_Down
            // 
            this.B_Down.Location = new System.Drawing.Point(124, 35);
            this.B_Down.Name = "B_Down";
            this.B_Down.Size = new System.Drawing.Size(23, 23);
            this.B_Down.TabIndex = 17;
            this.B_Down.Text = "v";
            this.B_Down.UseVisualStyleBackColor = true;
            this.B_Down.Click += new System.EventHandler(this.MoveBox);
            // 
            // SAV_BoxLayout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 321);
            this.Controls.Add(this.B_Down);
            this.Controls.Add(this.B_Up);
            this.Controls.Add(this.FLP_Misc);
            this.Controls.Add(this.PAN_BG);
            this.Controls.Add(this.CB_BG);
            this.Controls.Add(this.B_Cancel);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.L_BoxName);
            this.Controls.Add(this.TB_BoxName);
            this.Controls.Add(this.LB_BoxSelect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SAV_BoxLayout";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Box Layout Editor";
            this.FLP_Misc.ResumeLayout(false);
            this.FLP_Unlocked.ResumeLayout(false);
            this.FLP_Flags.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox LB_BoxSelect;
        private System.Windows.Forms.TextBox TB_BoxName;
        private System.Windows.Forms.Label L_BoxName;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.ComboBox CB_BG;
        private System.Windows.Forms.Panel PAN_BG;
        private System.Windows.Forms.FlowLayoutPanel FLP_Misc;
        private System.Windows.Forms.ComboBox CB_Unlocked;
        private System.Windows.Forms.Label L_Unlocked;
        private System.Windows.Forms.FlowLayoutPanel FLP_Unlocked;
        private System.Windows.Forms.FlowLayoutPanel FLP_Flags;
        private System.Windows.Forms.Label L_Flag;
        private System.Windows.Forms.Button B_Up;
        private System.Windows.Forms.Button B_Down;
    }
}