namespace PKHeX.WinForms
{
    partial class SAV_HallOfFame3
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
            LB_Entries = new System.Windows.Forms.ListBox();
            CB_Species = new System.Windows.Forms.ComboBox();
            NUD_Members = new System.Windows.Forms.NumericUpDown();
            TB_PID = new System.Windows.Forms.TextBox();
            TB_Nickname = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            NUD_Level = new System.Windows.Forms.NumericUpDown();
            B_Save = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            TB_TID = new System.Windows.Forms.MaskedTextBox();
            TB_SID = new System.Windows.Forms.MaskedTextBox();
            CHK_Shiny = new System.Windows.Forms.CheckBox();
            B_Clear = new System.Windows.Forms.Button();
            PB_Sprite = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)NUD_Members).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Level).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PB_Sprite).BeginInit();
            SuspendLayout();
            // 
            // LB_Entries
            // 
            LB_Entries.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            LB_Entries.FormattingEnabled = true;
            LB_Entries.Location = new System.Drawing.Point(12, 14);
            LB_Entries.Name = "LB_Entries";
            LB_Entries.Size = new System.Drawing.Size(120, 199);
            LB_Entries.TabIndex = 0;
            // 
            // CB_Species
            // 
            CB_Species.FormattingEnabled = true;
            CB_Species.Location = new System.Drawing.Point(169, 14);
            CB_Species.Name = "CB_Species";
            CB_Species.Size = new System.Drawing.Size(119, 23);
            CB_Species.TabIndex = 2;
            // 
            // NUD_Members
            // 
            NUD_Members.Location = new System.Drawing.Point(344, 14);
            NUD_Members.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            NUD_Members.Name = "NUD_Members";
            NUD_Members.Size = new System.Drawing.Size(35, 23);
            NUD_Members.TabIndex = 3;
            // 
            // TB_PID
            // 
            TB_PID.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            TB_PID.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            TB_PID.Location = new System.Drawing.Point(224, 101);
            TB_PID.MaxLength = 8;
            TB_PID.Name = "TB_PID";
            TB_PID.Size = new System.Drawing.Size(68, 20);
            TB_PID.TabIndex = 5;
            TB_PID.Text = "00000000";
            TB_PID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TB_Nickname
            // 
            TB_Nickname.Location = new System.Drawing.Point(224, 43);
            TB_Nickname.MaxLength = 10;
            TB_Nickname.Name = "TB_Nickname";
            TB_Nickname.Size = new System.Drawing.Size(120, 23);
            TB_Nickname.TabIndex = 6;
            TB_Nickname.Text = "WWWWWWWWWW";
            // 
            // label1
            // 
            label1.Location = new System.Drawing.Point(138, 122);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(80, 24);
            label1.TabIndex = 7;
            label1.Text = "TID:";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            label2.Location = new System.Drawing.Point(138, 146);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(80, 24);
            label2.TabIndex = 8;
            label2.Text = "SID:";
            label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            label3.Location = new System.Drawing.Point(138, 98);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(80, 24);
            label3.TabIndex = 9;
            label3.Text = "PID:";
            label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            label4.Location = new System.Drawing.Point(138, 41);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(80, 24);
            label4.TabIndex = 10;
            label4.Text = "Nickname:";
            label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            label5.Location = new System.Drawing.Point(138, 71);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(80, 24);
            label5.TabIndex = 11;
            label5.Text = "Level:";
            label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_Level
            // 
            NUD_Level.Location = new System.Drawing.Point(224, 72);
            NUD_Level.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_Level.Name = "NUD_Level";
            NUD_Level.Size = new System.Drawing.Size(40, 23);
            NUD_Level.TabIndex = 12;
            NUD_Level.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(389, 185);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(89, 27);
            B_Save.TabIndex = 13;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(294, 185);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(89, 27);
            B_Cancel.TabIndex = 14;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // TB_TID
            // 
            TB_TID.Location = new System.Drawing.Point(224, 124);
            TB_TID.Mask = "00000";
            TB_TID.Name = "TB_TID";
            TB_TID.Size = new System.Drawing.Size(40, 23);
            TB_TID.TabIndex = 15;
            TB_TID.Text = "00000";
            TB_TID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TB_SID
            // 
            TB_SID.Location = new System.Drawing.Point(224, 148);
            TB_SID.Mask = "00000";
            TB_SID.Name = "TB_SID";
            TB_SID.Size = new System.Drawing.Size(40, 23);
            TB_SID.TabIndex = 16;
            TB_SID.Text = "00000";
            TB_SID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CHK_Shiny
            // 
            CHK_Shiny.AutoSize = true;
            CHK_Shiny.Enabled = false;
            CHK_Shiny.Location = new System.Drawing.Point(298, 101);
            CHK_Shiny.Name = "CHK_Shiny";
            CHK_Shiny.Size = new System.Drawing.Size(55, 19);
            CHK_Shiny.TabIndex = 17;
            CHK_Shiny.Text = "Shiny";
            CHK_Shiny.UseVisualStyleBackColor = true;
            // 
            // B_Clear
            // 
            B_Clear.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            B_Clear.Location = new System.Drawing.Point(385, 11);
            B_Clear.Name = "B_Clear";
            B_Clear.Size = new System.Drawing.Size(89, 27);
            B_Clear.TabIndex = 18;
            B_Clear.Text = "Clear";
            B_Clear.UseVisualStyleBackColor = true;
            B_Clear.Click += B_Clear_Click;
            // 
            // PB_Sprite
            // 
            PB_Sprite.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            PB_Sprite.Location = new System.Drawing.Point(389, 53);
            PB_Sprite.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PB_Sprite.Name = "PB_Sprite";
            PB_Sprite.Size = new System.Drawing.Size(81, 67);
            PB_Sprite.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            PB_Sprite.TabIndex = 32;
            PB_Sprite.TabStop = false;
            // 
            // SAV_HallOfFame3
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(490, 224);
            Controls.Add(PB_Sprite);
            Controls.Add(B_Clear);
            Controls.Add(CHK_Shiny);
            Controls.Add(TB_SID);
            Controls.Add(TB_TID);
            Controls.Add(B_Cancel);
            Controls.Add(B_Save);
            Controls.Add(NUD_Level);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(TB_Nickname);
            Controls.Add(TB_PID);
            Controls.Add(NUD_Members);
            Controls.Add(CB_Species);
            Controls.Add(LB_Entries);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_HallOfFame3";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Hall of Fame Viewer";
            ((System.ComponentModel.ISupportInitialize)NUD_Members).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Level).EndInit();
            ((System.ComponentModel.ISupportInitialize)PB_Sprite).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListBox LB_Entries;
        private System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.NumericUpDown NUD_Members;
        private System.Windows.Forms.TextBox TB_PID;
        private System.Windows.Forms.TextBox TB_Nickname;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown NUD_Level;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.MaskedTextBox TB_TID;
        private System.Windows.Forms.MaskedTextBox TB_SID;
        private System.Windows.Forms.CheckBox CHK_Shiny;
        private System.Windows.Forms.Button B_Clear;
        private System.Windows.Forms.PictureBox PB_Sprite;
    }
}
