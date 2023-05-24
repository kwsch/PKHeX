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
            LB_BoxSelect = new System.Windows.Forms.ListBox();
            TB_BoxName = new System.Windows.Forms.TextBox();
            L_BoxName = new System.Windows.Forms.Label();
            B_Save = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            CB_BG = new System.Windows.Forms.ComboBox();
            PAN_BG = new System.Windows.Forms.Panel();
            FLP_Misc = new System.Windows.Forms.FlowLayoutPanel();
            FLP_Unlocked = new System.Windows.Forms.FlowLayoutPanel();
            L_Unlocked = new System.Windows.Forms.Label();
            CB_Unlocked = new System.Windows.Forms.ComboBox();
            FLP_Flags = new System.Windows.Forms.FlowLayoutPanel();
            L_Flag = new System.Windows.Forms.Label();
            B_Up = new System.Windows.Forms.Button();
            B_Down = new System.Windows.Forms.Button();
            FLP_Misc.SuspendLayout();
            FLP_Unlocked.SuspendLayout();
            FLP_Flags.SuspendLayout();
            SuspendLayout();
            // 
            // LB_BoxSelect
            // 
            LB_BoxSelect.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            LB_BoxSelect.FormattingEnabled = true;
            LB_BoxSelect.ItemHeight = 15;
            LB_BoxSelect.Items.AddRange(new object[] { "Boxes" });
            LB_BoxSelect.Location = new System.Drawing.Point(14, 16);
            LB_BoxSelect.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LB_BoxSelect.Name = "LB_BoxSelect";
            LB_BoxSelect.Size = new System.Drawing.Size(129, 334);
            LB_BoxSelect.TabIndex = 0;
            LB_BoxSelect.SelectedIndexChanged += ChangeBox;
            // 
            // TB_BoxName
            // 
            TB_BoxName.Location = new System.Drawing.Point(184, 40);
            TB_BoxName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_BoxName.MaxLength = 15;
            TB_BoxName.Name = "TB_BoxName";
            TB_BoxName.Size = new System.Drawing.Size(158, 23);
            TB_BoxName.TabIndex = 1;
            TB_BoxName.WordWrap = false;
            TB_BoxName.TextChanged += ChangeBoxDetails;
            // 
            // L_BoxName
            // 
            L_BoxName.AutoSize = true;
            L_BoxName.Location = new System.Drawing.Point(182, 22);
            L_BoxName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_BoxName.Name = "L_BoxName";
            L_BoxName.Size = new System.Drawing.Size(65, 15);
            L_BoxName.TabIndex = 2;
            L_BoxName.Text = "Box Name:";
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(428, 294);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(78, 27);
            B_Save.TabIndex = 9;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(428, 328);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(78, 27);
            B_Cancel.TabIndex = 10;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // CB_BG
            // 
            CB_BG.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            CB_BG.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_BG.FormattingEnabled = true;
            CB_BG.Location = new System.Drawing.Point(350, 39);
            CB_BG.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_BG.Name = "CB_BG";
            CB_BG.Size = new System.Drawing.Size(159, 23);
            CB_BG.TabIndex = 13;
            CB_BG.SelectedIndexChanged += ChangeBoxBackground;
            // 
            // PAN_BG
            // 
            PAN_BG.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            PAN_BG.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            PAN_BG.Location = new System.Drawing.Point(147, 70);
            PAN_BG.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PAN_BG.Name = "PAN_BG";
            PAN_BG.Size = new System.Drawing.Size(363, 185);
            PAN_BG.TabIndex = 14;
            // 
            // FLP_Misc
            // 
            FLP_Misc.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            FLP_Misc.Controls.Add(FLP_Unlocked);
            FLP_Misc.Controls.Add(FLP_Flags);
            FLP_Misc.Location = new System.Drawing.Point(150, 270);
            FLP_Misc.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            FLP_Misc.Name = "FLP_Misc";
            FLP_Misc.Size = new System.Drawing.Size(271, 84);
            FLP_Misc.TabIndex = 15;
            // 
            // FLP_Unlocked
            // 
            FLP_Unlocked.Controls.Add(L_Unlocked);
            FLP_Unlocked.Controls.Add(CB_Unlocked);
            FLP_Unlocked.Location = new System.Drawing.Point(4, 3);
            FLP_Unlocked.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            FLP_Unlocked.Name = "FLP_Unlocked";
            FLP_Unlocked.Size = new System.Drawing.Size(251, 29);
            FLP_Unlocked.TabIndex = 16;
            // 
            // L_Unlocked
            // 
            L_Unlocked.Location = new System.Drawing.Point(4, 0);
            L_Unlocked.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Unlocked.Name = "L_Unlocked";
            L_Unlocked.Size = new System.Drawing.Size(82, 24);
            L_Unlocked.TabIndex = 1;
            L_Unlocked.Text = "Unlocked:";
            L_Unlocked.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_Unlocked
            // 
            CB_Unlocked.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Unlocked.FormattingEnabled = true;
            CB_Unlocked.Location = new System.Drawing.Point(90, 0);
            CB_Unlocked.Margin = new System.Windows.Forms.Padding(0);
            CB_Unlocked.Name = "CB_Unlocked";
            CB_Unlocked.Size = new System.Drawing.Size(46, 23);
            CB_Unlocked.TabIndex = 0;
            // 
            // FLP_Flags
            // 
            FLP_Flags.Controls.Add(L_Flag);
            FLP_Flags.Location = new System.Drawing.Point(4, 38);
            FLP_Flags.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            FLP_Flags.Name = "FLP_Flags";
            FLP_Flags.Size = new System.Drawing.Size(251, 29);
            FLP_Flags.TabIndex = 17;
            // 
            // L_Flag
            // 
            L_Flag.Location = new System.Drawing.Point(4, 0);
            L_Flag.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Flag.Name = "L_Flag";
            L_Flag.Size = new System.Drawing.Size(82, 24);
            L_Flag.TabIndex = 1;
            L_Flag.Text = "Flags:";
            L_Flag.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // B_Up
            // 
            B_Up.Location = new System.Drawing.Point(145, 15);
            B_Up.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Up.Name = "B_Up";
            B_Up.Size = new System.Drawing.Size(27, 27);
            B_Up.TabIndex = 16;
            B_Up.Text = "^";
            B_Up.UseVisualStyleBackColor = true;
            B_Up.Click += MoveBox;
            // 
            // B_Down
            // 
            B_Down.Location = new System.Drawing.Point(145, 40);
            B_Down.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Down.Name = "B_Down";
            B_Down.Size = new System.Drawing.Size(27, 27);
            B_Down.TabIndex = 17;
            B_Down.Text = "v";
            B_Down.UseVisualStyleBackColor = true;
            B_Down.Click += MoveBox;
            // 
            // SAV_BoxLayout
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(523, 370);
            Controls.Add(B_Down);
            Controls.Add(B_Up);
            Controls.Add(FLP_Misc);
            Controls.Add(PAN_BG);
            Controls.Add(CB_BG);
            Controls.Add(B_Cancel);
            Controls.Add(B_Save);
            Controls.Add(L_BoxName);
            Controls.Add(TB_BoxName);
            Controls.Add(LB_BoxSelect);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_BoxLayout";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Box Layout Editor";
            FLP_Misc.ResumeLayout(false);
            FLP_Unlocked.ResumeLayout(false);
            FLP_Flags.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
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
