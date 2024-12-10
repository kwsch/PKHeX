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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SAV_HallOfFame3));
            LB_Entries = new System.Windows.Forms.ListBox();
            TB_TID = new System.Windows.Forms.TextBox();
            CB_Species = new System.Windows.Forms.ComboBox();
            NUD_Members = new System.Windows.Forms.NumericUpDown();
            TB_SID = new System.Windows.Forms.TextBox();
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
            ((System.ComponentModel.ISupportInitialize)NUD_Members).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Level).BeginInit();
            SuspendLayout();
            // 
            // LB_Entries
            // 
            LB_Entries.FormattingEnabled = true;
            LB_Entries.Location = new System.Drawing.Point(12, 14);
            LB_Entries.Name = "LB_Entries";
            LB_Entries.Size = new System.Drawing.Size(120, 214);
            LB_Entries.TabIndex = 0;
            // 
            // TB_TID
            // 
            TB_TID.Location = new System.Drawing.Point(215, 96);
            TB_TID.Name = "TB_TID";
            TB_TID.Size = new System.Drawing.Size(100, 23);
            TB_TID.TabIndex = 1;
            // 
            // CB_Species
            // 
            CB_Species.FormattingEnabled = true;
            CB_Species.Location = new System.Drawing.Point(169, 14);
            CB_Species.Name = "CB_Species";
            CB_Species.Size = new System.Drawing.Size(184, 23);
            CB_Species.TabIndex = 2;
            // 
            // NUD_Members
            // 
            NUD_Members.Location = new System.Drawing.Point(360, 14);
            NUD_Members.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            NUD_Members.Name = "NUD_Members";
            NUD_Members.Size = new System.Drawing.Size(35, 23);
            NUD_Members.TabIndex = 3;
            // 
            // TB_SID
            // 
            TB_SID.Location = new System.Drawing.Point(215, 129);
            TB_SID.Name = "TB_SID";
            TB_SID.Size = new System.Drawing.Size(100, 23);
            TB_SID.TabIndex = 4;
            // 
            // TB_PID
            // 
            TB_PID.Location = new System.Drawing.Point(204, 164);
            TB_PID.Name = "TB_PID";
            TB_PID.Size = new System.Drawing.Size(155, 23);
            TB_PID.TabIndex = 5;
            // 
            // TB_Nickname
            // 
            TB_Nickname.Location = new System.Drawing.Point(240, 197);
            TB_Nickname.Name = "TB_Nickname";
            TB_Nickname.Size = new System.Drawing.Size(155, 23);
            TB_Nickname.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(170, 99);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(39, 15);
            label1.TabIndex = 7;
            label1.Text = "TID16:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(170, 132);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(39, 15);
            label2.TabIndex = 8;
            label2.Text = "SID16:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(170, 167);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(28, 15);
            label3.TabIndex = 9;
            label3.Text = "PID:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(170, 200);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(64, 15);
            label4.TabIndex = 10;
            label4.Text = "Nickname:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(170, 61);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(22, 15);
            label5.TabIndex = 11;
            label5.Text = "LV:";
            // 
            // NUD_Level
            // 
            NUD_Level.Location = new System.Drawing.Point(198, 59);
            NUD_Level.Name = "NUD_Level";
            NUD_Level.Size = new System.Drawing.Size(49, 23);
            NUD_Level.TabIndex = 12;
            NUD_Level.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // B_Save
            // 
            B_Save.Location = new System.Drawing.Point(306, 235);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(89, 27);
            B_Save.TabIndex = 13;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Location = new System.Drawing.Point(211, 235);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(89, 27);
            B_Cancel.TabIndex = 14;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // SAV_HallOfFame3
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(407, 274);
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
            Controls.Add(TB_SID);
            Controls.Add(NUD_Members);
            Controls.Add(CB_Species);
            Controls.Add(TB_TID);
            Controls.Add(LB_Entries);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_HallOfFame3";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Hall of Fame Viewer";
            ((System.ComponentModel.ISupportInitialize)NUD_Members).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Level).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListBox LB_Entries;
        private System.Windows.Forms.TextBox TB_TID;
        private System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.NumericUpDown NUD_Members;
        private System.Windows.Forms.TextBox TB_SID;
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
    }
}
