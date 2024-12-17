namespace PKHeX.WinForms
{
    partial class SAV_SecretBase3
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
            LB_Bases = new System.Windows.Forms.ListBox();
            L_Trainers = new System.Windows.Forms.Label();
            T_TrainerGender = new Controls.GenderToggle();
            TB_Name = new System.Windows.Forms.TextBox();
            CHK_Battled = new System.Windows.Forms.CheckBox();
            CHK_Registered = new System.Windows.Forms.CheckBox();
            TB_Entered = new System.Windows.Forms.TextBox();
            L_Entered = new System.Windows.Forms.Label();
            TB_Class = new System.Windows.Forms.TextBox();
            L_Class = new System.Windows.Forms.Label();
            TB_TID = new System.Windows.Forms.TextBox();
            TB_SID = new System.Windows.Forms.TextBox();
            L_TID = new System.Windows.Forms.Label();
            L_SID = new System.Windows.Forms.Label();
            NUD_TeamMember = new System.Windows.Forms.NumericUpDown();
            CB_Species = new System.Windows.Forms.ComboBox();
            L_Member = new System.Windows.Forms.Label();
            CB_Form = new System.Windows.Forms.ComboBox();
            L_Item = new System.Windows.Forms.Label();
            CB_Item = new System.Windows.Forms.ComboBox();
            L_PID = new System.Windows.Forms.Label();
            TB_PID = new System.Windows.Forms.TextBox();
            NUD_Level = new System.Windows.Forms.NumericUpDown();
            NUD_EVs = new System.Windows.Forms.NumericUpDown();
            L_Level = new System.Windows.Forms.Label();
            L_EVs = new System.Windows.Forms.Label();
            CB_Move1 = new System.Windows.Forms.ComboBox();
            CB_Move2 = new System.Windows.Forms.ComboBox();
            CB_Move3 = new System.Windows.Forms.ComboBox();
            CB_Move4 = new System.Windows.Forms.ComboBox();
            L_Move1 = new System.Windows.Forms.Label();
            L_Move2 = new System.Windows.Forms.Label();
            L_Move3 = new System.Windows.Forms.Label();
            L_Move4 = new System.Windows.Forms.Label();
            B_UpdateTrainer = new System.Windows.Forms.Button();
            B_UpdatePKM = new System.Windows.Forms.Button();
            B_Save = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)NUD_TeamMember).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Level).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_EVs).BeginInit();
            SuspendLayout();
            // 
            // LB_Bases
            // 
            LB_Bases.FormattingEnabled = true;
            LB_Bases.ItemHeight = 15;
            LB_Bases.Location = new System.Drawing.Point(12, 26);
            LB_Bases.Name = "LB_Bases";
            LB_Bases.Size = new System.Drawing.Size(125, 379);
            LB_Bases.TabIndex = 0;
            // 
            // L_Trainers
            // 
            L_Trainers.AutoSize = true;
            L_Trainers.Location = new System.Drawing.Point(12, 9);
            L_Trainers.Name = "L_Trainers";
            L_Trainers.Size = new System.Drawing.Size(50, 15);
            L_Trainers.TabIndex = 1;
            L_Trainers.Text = "Trainers:";
            // 
            // T_TrainerGender
            // 
            T_TrainerGender.AllowClick = true;
            T_TrainerGender.Gender = 2;
            T_TrainerGender.Location = new System.Drawing.Point(282, 31);
            T_TrainerGender.Name = "T_TrainerGender";
            T_TrainerGender.Size = new System.Drawing.Size(18, 18);
            T_TrainerGender.TabIndex = 2;
            // 
            // TB_Name
            // 
            TB_Name.Location = new System.Drawing.Point(154, 26);
            TB_Name.Name = "TB_Name";
            TB_Name.Size = new System.Drawing.Size(122, 23);
            TB_Name.TabIndex = 3;
            // 
            // CHK_Battled
            // 
            CHK_Battled.AutoSize = true;
            CHK_Battled.Location = new System.Drawing.Point(168, 65);
            CHK_Battled.Name = "CHK_Battled";
            CHK_Battled.Size = new System.Drawing.Size(97, 19);
            CHK_Battled.TabIndex = 4;
            CHK_Battled.Text = "Battled Today";
            CHK_Battled.UseVisualStyleBackColor = true;
            // 
            // CHK_Registered
            // 
            CHK_Registered.AutoSize = true;
            CHK_Registered.Location = new System.Drawing.Point(168, 90);
            CHK_Registered.Name = "CHK_Registered";
            CHK_Registered.Size = new System.Drawing.Size(81, 19);
            CHK_Registered.TabIndex = 5;
            CHK_Registered.Text = "Registered";
            CHK_Registered.UseVisualStyleBackColor = true;
            // 
            // TB_Entered
            // 
            TB_Entered.Location = new System.Drawing.Point(154, 218);
            TB_Entered.Name = "TB_Entered";
            TB_Entered.Size = new System.Drawing.Size(74, 23);
            TB_Entered.TabIndex = 7;
            // 
            // L_Entered
            // 
            L_Entered.AutoSize = true;
            L_Entered.Location = new System.Drawing.Point(154, 200);
            L_Entered.Name = "L_Entered";
            L_Entered.Size = new System.Drawing.Size(122, 15);
            L_Entered.TabIndex = 8;
            L_Entered.Text = "Number of entrances:";
            // 
            // TB_Class
            // 
            TB_Class.Location = new System.Drawing.Point(154, 283);
            TB_Class.Name = "TB_Class";
            TB_Class.ReadOnly = true;
            TB_Class.Size = new System.Drawing.Size(146, 23);
            TB_Class.TabIndex = 9;
            // 
            // L_Class
            // 
            L_Class.AutoSize = true;
            L_Class.Location = new System.Drawing.Point(154, 265);
            L_Class.Name = "L_Class";
            L_Class.Size = new System.Drawing.Size(73, 15);
            L_Class.TabIndex = 10;
            L_Class.Text = "Trainer class:";
            // 
            // TB_TID
            // 
            TB_TID.Location = new System.Drawing.Point(191, 127);
            TB_TID.Name = "TB_TID";
            TB_TID.Size = new System.Drawing.Size(74, 23);
            TB_TID.TabIndex = 11;
            // 
            // TB_SID
            // 
            TB_SID.Location = new System.Drawing.Point(191, 156);
            TB_SID.Name = "TB_SID";
            TB_SID.Size = new System.Drawing.Size(74, 23);
            TB_SID.TabIndex = 12;
            // 
            // L_TID
            // 
            L_TID.AutoSize = true;
            L_TID.Location = new System.Drawing.Point(158, 130);
            L_TID.Name = "L_TID";
            L_TID.Size = new System.Drawing.Size(27, 15);
            L_TID.TabIndex = 13;
            L_TID.Text = "TID:";
            // 
            // L_SID
            // 
            L_SID.AutoSize = true;
            L_SID.Location = new System.Drawing.Point(158, 159);
            L_SID.Name = "L_SID";
            L_SID.Size = new System.Drawing.Size(27, 15);
            L_SID.TabIndex = 14;
            L_SID.Text = "SID:";
            // 
            // NUD_TeamMember
            // 
            NUD_TeamMember.Location = new System.Drawing.Point(476, 32);
            NUD_TeamMember.Maximum = new decimal(new int[] { 6, 0, 0, 0 });
            NUD_TeamMember.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NUD_TeamMember.Name = "NUD_TeamMember";
            NUD_TeamMember.Size = new System.Drawing.Size(33, 23);
            NUD_TeamMember.TabIndex = 15;
            NUD_TeamMember.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // CB_Species
            // 
            CB_Species.FormattingEnabled = true;
            CB_Species.Location = new System.Drawing.Point(384, 74);
            CB_Species.Name = "CB_Species";
            CB_Species.Size = new System.Drawing.Size(139, 23);
            CB_Species.TabIndex = 16;
            // 
            // L_Member
            // 
            L_Member.AutoSize = true;
            L_Member.Location = new System.Drawing.Point(384, 34);
            L_Member.Name = "L_Member";
            L_Member.Size = new System.Drawing.Size(86, 15);
            L_Member.TabIndex = 17;
            L_Member.Text = "Team Member:";
            // 
            // CB_Form
            // 
            CB_Form.Enabled = false;
            CB_Form.FormattingEnabled = true;
            CB_Form.Location = new System.Drawing.Point(529, 74);
            CB_Form.Name = "CB_Form";
            CB_Form.Size = new System.Drawing.Size(38, 23);
            CB_Form.TabIndex = 18;
            CB_Form.Visible = false;
            // 
            // L_Item
            // 
            L_Item.AutoSize = true;
            L_Item.Location = new System.Drawing.Point(384, 173);
            L_Item.Name = "L_Item";
            L_Item.Size = new System.Drawing.Size(34, 15);
            L_Item.TabIndex = 19;
            L_Item.Text = "Item:";
            // 
            // CB_Item
            // 
            CB_Item.FormattingEnabled = true;
            CB_Item.Location = new System.Drawing.Point(384, 191);
            CB_Item.Name = "CB_Item";
            CB_Item.Size = new System.Drawing.Size(139, 23);
            CB_Item.TabIndex = 20;
            // 
            // L_PID
            // 
            L_PID.AutoSize = true;
            L_PID.Location = new System.Drawing.Point(384, 114);
            L_PID.Name = "L_PID";
            L_PID.Size = new System.Drawing.Size(28, 15);
            L_PID.TabIndex = 21;
            L_PID.Text = "PID:";
            // 
            // TB_PID
            // 
            TB_PID.Location = new System.Drawing.Point(384, 132);
            TB_PID.Name = "TB_PID";
            TB_PID.Size = new System.Drawing.Size(100, 23);
            TB_PID.TabIndex = 22;
            // 
            // NUD_Level
            // 
            NUD_Level.Location = new System.Drawing.Point(412, 235);
            NUD_Level.Name = "NUD_Level";
            NUD_Level.Size = new System.Drawing.Size(46, 23);
            NUD_Level.TabIndex = 23;
            // 
            // NUD_EVs
            // 
            NUD_EVs.Location = new System.Drawing.Point(510, 235);
            NUD_EVs.Maximum = new decimal(new int[] { 85, 0, 0, 0 });
            NUD_EVs.Name = "NUD_EVs";
            NUD_EVs.Size = new System.Drawing.Size(46, 23);
            NUD_EVs.TabIndex = 24;
            // 
            // L_Level
            // 
            L_Level.AutoSize = true;
            L_Level.Location = new System.Drawing.Point(384, 237);
            L_Level.Name = "L_Level";
            L_Level.Size = new System.Drawing.Size(22, 15);
            L_Level.TabIndex = 25;
            L_Level.Text = "LV:";
            // 
            // L_EVs
            // 
            L_EVs.AutoSize = true;
            L_EVs.Location = new System.Drawing.Point(476, 237);
            L_EVs.Name = "L_EVs";
            L_EVs.Size = new System.Drawing.Size(28, 15);
            L_EVs.TabIndex = 26;
            L_EVs.Text = "EVs:";
            // 
            // CB_Move1
            // 
            CB_Move1.FormattingEnabled = true;
            CB_Move1.Location = new System.Drawing.Point(439, 283);
            CB_Move1.Name = "CB_Move1";
            CB_Move1.Size = new System.Drawing.Size(139, 23);
            CB_Move1.TabIndex = 27;
            // 
            // CB_Move2
            // 
            CB_Move2.FormattingEnabled = true;
            CB_Move2.Location = new System.Drawing.Point(439, 312);
            CB_Move2.Name = "CB_Move2";
            CB_Move2.Size = new System.Drawing.Size(139, 23);
            CB_Move2.TabIndex = 28;
            // 
            // CB_Move3
            // 
            CB_Move3.FormattingEnabled = true;
            CB_Move3.Location = new System.Drawing.Point(439, 341);
            CB_Move3.Name = "CB_Move3";
            CB_Move3.Size = new System.Drawing.Size(139, 23);
            CB_Move3.TabIndex = 29;
            // 
            // CB_Move4
            // 
            CB_Move4.FormattingEnabled = true;
            CB_Move4.Location = new System.Drawing.Point(439, 370);
            CB_Move4.Name = "CB_Move4";
            CB_Move4.Size = new System.Drawing.Size(139, 23);
            CB_Move4.TabIndex = 31;
            // 
            // L_Move1
            // 
            L_Move1.AutoSize = true;
            L_Move1.Location = new System.Drawing.Point(384, 286);
            L_Move1.Name = "L_Move1";
            L_Move1.Size = new System.Drawing.Size(49, 15);
            L_Move1.TabIndex = 32;
            L_Move1.Text = "Move 1:";
            // 
            // L_Move2
            // 
            L_Move2.AutoSize = true;
            L_Move2.Location = new System.Drawing.Point(384, 315);
            L_Move2.Name = "L_Move2";
            L_Move2.Size = new System.Drawing.Size(49, 15);
            L_Move2.TabIndex = 33;
            L_Move2.Text = "Move 2:";
            // 
            // L_Move3
            // 
            L_Move3.AutoSize = true;
            L_Move3.Location = new System.Drawing.Point(384, 344);
            L_Move3.Name = "L_Move3";
            L_Move3.Size = new System.Drawing.Size(49, 15);
            L_Move3.TabIndex = 34;
            L_Move3.Text = "Move 3:";
            // 
            // L_Move4
            // 
            L_Move4.AutoSize = true;
            L_Move4.Location = new System.Drawing.Point(384, 373);
            L_Move4.Name = "L_Move4";
            L_Move4.Size = new System.Drawing.Size(49, 15);
            L_Move4.TabIndex = 35;
            L_Move4.Text = "Move 4:";
            // 
            // B_UpdateTrainer
            // 
            B_UpdateTrainer.Location = new System.Drawing.Point(154, 332);
            B_UpdateTrainer.Name = "B_UpdateTrainer";
            B_UpdateTrainer.Size = new System.Drawing.Size(118, 27);
            B_UpdateTrainer.TabIndex = 36;
            B_UpdateTrainer.Text = "Update Trainer";
            B_UpdateTrainer.UseVisualStyleBackColor = true;
            B_UpdateTrainer.Click += B_UpdateTrainer_Click;
            // 
            // B_UpdatePKM
            // 
            B_UpdatePKM.Location = new System.Drawing.Point(154, 366);
            B_UpdatePKM.Name = "B_UpdatePKM";
            B_UpdatePKM.Size = new System.Drawing.Size(118, 27);
            B_UpdatePKM.TabIndex = 37;
            B_UpdatePKM.Text = "Update PKM";
            B_UpdatePKM.UseVisualStyleBackColor = true;
            B_UpdatePKM.Click += B_UpdatePKM_Click;
            // 
            // B_Save
            // 
            B_Save.Location = new System.Drawing.Point(479, 413);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(88, 27);
            B_Save.TabIndex = 38;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Location = new System.Drawing.Point(384, 413);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(88, 27);
            B_Cancel.TabIndex = 39;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // SAV_SecretBase3
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(591, 461);
            Controls.Add(B_Cancel);
            Controls.Add(B_Save);
            Controls.Add(B_UpdatePKM);
            Controls.Add(B_UpdateTrainer);
            Controls.Add(L_Move4);
            Controls.Add(L_Move3);
            Controls.Add(L_Move2);
            Controls.Add(L_Move1);
            Controls.Add(CB_Move4);
            Controls.Add(CB_Move3);
            Controls.Add(CB_Move2);
            Controls.Add(CB_Move1);
            Controls.Add(L_EVs);
            Controls.Add(L_Level);
            Controls.Add(NUD_EVs);
            Controls.Add(NUD_Level);
            Controls.Add(TB_PID);
            Controls.Add(L_PID);
            Controls.Add(CB_Item);
            Controls.Add(L_Item);
            Controls.Add(CB_Form);
            Controls.Add(L_Member);
            Controls.Add(CB_Species);
            Controls.Add(NUD_TeamMember);
            Controls.Add(L_SID);
            Controls.Add(L_TID);
            Controls.Add(TB_SID);
            Controls.Add(TB_TID);
            Controls.Add(L_Class);
            Controls.Add(TB_Class);
            Controls.Add(L_Entered);
            Controls.Add(TB_Entered);
            Controls.Add(CHK_Registered);
            Controls.Add(CHK_Battled);
            Controls.Add(TB_Name);
            Controls.Add(T_TrainerGender);
            Controls.Add(L_Trainers);
            Controls.Add(LB_Bases);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_SecretBase3";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Secret Base Editor";
            ((System.ComponentModel.ISupportInitialize)NUD_TeamMember).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Level).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_EVs).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListBox LB_Bases;
        private System.Windows.Forms.Label L_Trainers;
        private Controls.GenderToggle T_TrainerGender;
        private System.Windows.Forms.TextBox TB_Name;
        private System.Windows.Forms.CheckBox CHK_Battled;
        private System.Windows.Forms.CheckBox CHK_Registered;
        private System.Windows.Forms.TextBox TB_Entered;
        private System.Windows.Forms.Label L_Entered;
        private System.Windows.Forms.TextBox TB_Class;
        private System.Windows.Forms.Label L_Class;
        private System.Windows.Forms.TextBox TB_TID;
        private System.Windows.Forms.TextBox TB_SID;
        private System.Windows.Forms.Label L_TID;
        private System.Windows.Forms.Label L_SID;
        private System.Windows.Forms.NumericUpDown NUD_TeamMember;
        private System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.Label L_Member;
        private System.Windows.Forms.ComboBox CB_Form;
        private System.Windows.Forms.Label L_Item;
        private System.Windows.Forms.ComboBox CB_Item;
        private System.Windows.Forms.Label L_PID;
        private System.Windows.Forms.TextBox TB_PID;
        private System.Windows.Forms.NumericUpDown NUD_Level;
        private System.Windows.Forms.NumericUpDown NUD_EVs;
        private System.Windows.Forms.Label L_Level;
        private System.Windows.Forms.Label L_EVs;
        private System.Windows.Forms.ComboBox CB_Move1;
        private System.Windows.Forms.ComboBox CB_Move2;
        private System.Windows.Forms.ComboBox CB_Move3;
        private System.Windows.Forms.ComboBox CB_Move4;
        private System.Windows.Forms.Label L_Move1;
        private System.Windows.Forms.Label L_Move2;
        private System.Windows.Forms.Label L_Move3;
        private System.Windows.Forms.Label L_Move4;
        private System.Windows.Forms.Button B_UpdateTrainer;
        private System.Windows.Forms.Button B_UpdatePKM;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
    }
}
