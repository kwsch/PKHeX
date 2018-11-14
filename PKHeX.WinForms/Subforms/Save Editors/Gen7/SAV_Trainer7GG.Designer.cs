namespace PKHeX.WinForms
{
    partial class SAV_Trainer7GG
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SAV_Trainer7GG));
            this.B_Cancel = new System.Windows.Forms.Button();
            this.B_Save = new System.Windows.Forms.Button();
            this.TB_OTName = new System.Windows.Forms.TextBox();
            this.L_TrainerName = new System.Windows.Forms.Label();
            this.MT_Money = new System.Windows.Forms.MaskedTextBox();
            this.L_Money = new System.Windows.Forms.Label();
            this.L_Saying5 = new System.Windows.Forms.Label();
            this.L_Saying4 = new System.Windows.Forms.Label();
            this.L_Saying3 = new System.Windows.Forms.Label();
            this.L_Saying2 = new System.Windows.Forms.Label();
            this.L_Saying1 = new System.Windows.Forms.Label();
            this.TB_Saying5 = new System.Windows.Forms.TextBox();
            this.TB_Saying4 = new System.Windows.Forms.TextBox();
            this.TB_Saying3 = new System.Windows.Forms.TextBox();
            this.TB_Saying2 = new System.Windows.Forms.TextBox();
            this.TB_Saying1 = new System.Windows.Forms.TextBox();
            this.L_Seconds = new System.Windows.Forms.Label();
            this.L_Minutes = new System.Windows.Forms.Label();
            this.MT_Seconds = new System.Windows.Forms.MaskedTextBox();
            this.MT_Minutes = new System.Windows.Forms.MaskedTextBox();
            this.L_Hours = new System.Windows.Forms.Label();
            this.MT_Hours = new System.Windows.Forms.MaskedTextBox();
            this.L_Language = new System.Windows.Forms.Label();
            this.B_MaxCash = new System.Windows.Forms.Button();
            this.CB_Language = new System.Windows.Forms.ComboBox();
            this.CB_Game = new System.Windows.Forms.ComboBox();
            this.CB_Gender = new System.Windows.Forms.ComboBox();
            this.TC_Editor = new System.Windows.Forms.TabControl();
            this.Tab_Overview = new System.Windows.Forms.TabPage();
            this.TB_RivalName = new System.Windows.Forms.TextBox();
            this.L_RivalName = new System.Windows.Forms.Label();
            this.trainerID1 = new PKHeX.WinForms.Controls.TrainerID();
            this.GB_Adventure = new System.Windows.Forms.GroupBox();
            this.Tab_Misc = new System.Windows.Forms.TabPage();
            this.CHK_UnlockZMove = new System.Windows.Forms.CheckBox();
            this.CHK_UnlockMega = new System.Windows.Forms.CheckBox();
            this.TC_Editor.SuspendLayout();
            this.Tab_Overview.SuspendLayout();
            this.GB_Adventure.SuspendLayout();
            this.Tab_Misc.SuspendLayout();
            this.SuspendLayout();
            //
            // B_Cancel
            //
            this.B_Cancel.Location = new System.Drawing.Point(250, 334);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(75, 23);
            this.B_Cancel.TabIndex = 0;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            //
            // B_Save
            //
            this.B_Save.Location = new System.Drawing.Point(331, 334);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(75, 23);
            this.B_Save.TabIndex = 1;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            //
            // TB_OTName
            //
            this.TB_OTName.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_OTName.Location = new System.Drawing.Point(99, 7);
            this.TB_OTName.MaxLength = 12;
            this.TB_OTName.Name = "TB_OTName";
            this.TB_OTName.Size = new System.Drawing.Size(93, 20);
            this.TB_OTName.TabIndex = 2;
            this.TB_OTName.Text = "WWWWWWWWWWWW";
            this.TB_OTName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_OTName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ClickString);
            //
            // L_TrainerName
            //
            this.L_TrainerName.Location = new System.Drawing.Point(7, 9);
            this.L_TrainerName.Name = "L_TrainerName";
            this.L_TrainerName.Size = new System.Drawing.Size(87, 16);
            this.L_TrainerName.TabIndex = 3;
            this.L_TrainerName.Text = "Trainer Name:";
            this.L_TrainerName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // MT_Money
            //
            this.MT_Money.Location = new System.Drawing.Point(119, 29);
            this.MT_Money.Mask = "0000000";
            this.MT_Money.Name = "MT_Money";
            this.MT_Money.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.MT_Money.Size = new System.Drawing.Size(52, 20);
            this.MT_Money.TabIndex = 4;
            this.MT_Money.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // L_Money
            //
            this.L_Money.AutoSize = true;
            this.L_Money.Location = new System.Drawing.Point(102, 32);
            this.L_Money.Name = "L_Money";
            this.L_Money.Size = new System.Drawing.Size(16, 13);
            this.L_Money.TabIndex = 5;
            this.L_Money.Text = "$:";
            //
            // L_Saying5
            //
            this.L_Saying5.Location = new System.Drawing.Point(0, 0);
            this.L_Saying5.Name = "L_Saying5";
            this.L_Saying5.Size = new System.Drawing.Size(100, 23);
            this.L_Saying5.TabIndex = 0;
            //
            // L_Saying4
            //
            this.L_Saying4.Location = new System.Drawing.Point(0, 0);
            this.L_Saying4.Name = "L_Saying4";
            this.L_Saying4.Size = new System.Drawing.Size(100, 23);
            this.L_Saying4.TabIndex = 0;
            //
            // L_Saying3
            //
            this.L_Saying3.Location = new System.Drawing.Point(0, 0);
            this.L_Saying3.Name = "L_Saying3";
            this.L_Saying3.Size = new System.Drawing.Size(100, 23);
            this.L_Saying3.TabIndex = 0;
            //
            // L_Saying2
            //
            this.L_Saying2.Location = new System.Drawing.Point(0, 0);
            this.L_Saying2.Name = "L_Saying2";
            this.L_Saying2.Size = new System.Drawing.Size(100, 23);
            this.L_Saying2.TabIndex = 0;
            //
            // L_Saying1
            //
            this.L_Saying1.Location = new System.Drawing.Point(0, 0);
            this.L_Saying1.Name = "L_Saying1";
            this.L_Saying1.Size = new System.Drawing.Size(100, 23);
            this.L_Saying1.TabIndex = 0;
            //
            // TB_Saying5
            //
            this.TB_Saying5.Location = new System.Drawing.Point(0, 0);
            this.TB_Saying5.Name = "TB_Saying5";
            this.TB_Saying5.Size = new System.Drawing.Size(100, 20);
            this.TB_Saying5.TabIndex = 0;
            //
            // TB_Saying4
            //
            this.TB_Saying4.Location = new System.Drawing.Point(0, 0);
            this.TB_Saying4.Name = "TB_Saying4";
            this.TB_Saying4.Size = new System.Drawing.Size(100, 20);
            this.TB_Saying4.TabIndex = 0;
            //
            // TB_Saying3
            //
            this.TB_Saying3.Location = new System.Drawing.Point(0, 0);
            this.TB_Saying3.Name = "TB_Saying3";
            this.TB_Saying3.Size = new System.Drawing.Size(100, 20);
            this.TB_Saying3.TabIndex = 0;
            //
            // TB_Saying2
            //
            this.TB_Saying2.Location = new System.Drawing.Point(0, 0);
            this.TB_Saying2.Name = "TB_Saying2";
            this.TB_Saying2.Size = new System.Drawing.Size(100, 20);
            this.TB_Saying2.TabIndex = 0;
            //
            // TB_Saying1
            //
            this.TB_Saying1.Location = new System.Drawing.Point(0, 0);
            this.TB_Saying1.Name = "TB_Saying1";
            this.TB_Saying1.Size = new System.Drawing.Size(100, 20);
            this.TB_Saying1.TabIndex = 0;
            //
            // L_Seconds
            //
            this.L_Seconds.AutoSize = true;
            this.L_Seconds.Location = new System.Drawing.Point(136, 17);
            this.L_Seconds.Name = "L_Seconds";
            this.L_Seconds.Size = new System.Drawing.Size(29, 13);
            this.L_Seconds.TabIndex = 30;
            this.L_Seconds.Text = "Sec:";
            //
            // L_Minutes
            //
            this.L_Minutes.AutoSize = true;
            this.L_Minutes.Location = new System.Drawing.Point(84, 17);
            this.L_Minutes.Name = "L_Minutes";
            this.L_Minutes.Size = new System.Drawing.Size(27, 13);
            this.L_Minutes.TabIndex = 29;
            this.L_Minutes.Text = "Min:";
            //
            // MT_Seconds
            //
            this.MT_Seconds.Location = new System.Drawing.Point(166, 14);
            this.MT_Seconds.Mask = "00";
            this.MT_Seconds.Name = "MT_Seconds";
            this.MT_Seconds.Size = new System.Drawing.Size(22, 20);
            this.MT_Seconds.TabIndex = 28;
            this.MT_Seconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.MT_Seconds.TextChanged += new System.EventHandler(this.Change255);
            //
            // MT_Minutes
            //
            this.MT_Minutes.Location = new System.Drawing.Point(111, 14);
            this.MT_Minutes.Mask = "00";
            this.MT_Minutes.Name = "MT_Minutes";
            this.MT_Minutes.Size = new System.Drawing.Size(22, 20);
            this.MT_Minutes.TabIndex = 27;
            this.MT_Minutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.MT_Minutes.TextChanged += new System.EventHandler(this.Change255);
            //
            // L_Hours
            //
            this.L_Hours.AutoSize = true;
            this.L_Hours.Location = new System.Drawing.Point(12, 17);
            this.L_Hours.Name = "L_Hours";
            this.L_Hours.Size = new System.Drawing.Size(26, 13);
            this.L_Hours.TabIndex = 26;
            this.L_Hours.Text = "Hrs:";
            //
            // MT_Hours
            //
            this.MT_Hours.Location = new System.Drawing.Point(44, 14);
            this.MT_Hours.Mask = "00000";
            this.MT_Hours.Name = "MT_Hours";
            this.MT_Hours.Size = new System.Drawing.Size(38, 20);
            this.MT_Hours.TabIndex = 25;
            this.MT_Hours.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // L_Language
            //
            this.L_Language.Location = new System.Drawing.Point(3, 102);
            this.L_Language.Name = "L_Language";
            this.L_Language.Size = new System.Drawing.Size(80, 13);
            this.L_Language.TabIndex = 21;
            this.L_Language.Text = "Language:";
            this.L_Language.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // B_MaxCash
            //
            this.B_MaxCash.Location = new System.Drawing.Point(172, 29);
            this.B_MaxCash.Name = "B_MaxCash";
            this.B_MaxCash.Size = new System.Drawing.Size(20, 20);
            this.B_MaxCash.TabIndex = 16;
            this.B_MaxCash.Text = "+";
            this.B_MaxCash.UseVisualStyleBackColor = true;
            //
            // CB_Language
            //
            this.CB_Language.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Language.FormattingEnabled = true;
            this.CB_Language.Location = new System.Drawing.Point(99, 78);
            this.CB_Language.Name = "CB_Language";
            this.CB_Language.Size = new System.Drawing.Size(93, 21);
            this.CB_Language.TabIndex = 15;
            //
            // CB_Game
            //
            this.CB_Game.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Game.FormattingEnabled = true;
            this.CB_Game.Location = new System.Drawing.Point(99, 51);
            this.CB_Game.Name = "CB_Game";
            this.CB_Game.Size = new System.Drawing.Size(135, 21);
            this.CB_Game.TabIndex = 24;
            //
            // CB_Gender
            //
            this.CB_Gender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Gender.Enabled = false;
            this.CB_Gender.FormattingEnabled = true;
            this.CB_Gender.Items.AddRange(new object[] {
            "♂",
            "♀"});
            this.CB_Gender.Location = new System.Drawing.Point(194, 78);
            this.CB_Gender.Name = "CB_Gender";
            this.CB_Gender.Size = new System.Drawing.Size(40, 21);
            this.CB_Gender.TabIndex = 22;
            //
            // TC_Editor
            //
            this.TC_Editor.Controls.Add(this.Tab_Overview);
            this.TC_Editor.Controls.Add(this.Tab_Misc);
            this.TC_Editor.Location = new System.Drawing.Point(12, 12);
            this.TC_Editor.Name = "TC_Editor";
            this.TC_Editor.SelectedIndex = 0;
            this.TC_Editor.Size = new System.Drawing.Size(394, 316);
            this.TC_Editor.TabIndex = 54;
            //
            // Tab_Overview
            //
            this.Tab_Overview.Controls.Add(this.TB_RivalName);
            this.Tab_Overview.Controls.Add(this.L_RivalName);
            this.Tab_Overview.Controls.Add(this.trainerID1);
            this.Tab_Overview.Controls.Add(this.GB_Adventure);
            this.Tab_Overview.Controls.Add(this.TB_OTName);
            this.Tab_Overview.Controls.Add(this.CB_Gender);
            this.Tab_Overview.Controls.Add(this.CB_Game);
            this.Tab_Overview.Controls.Add(this.L_TrainerName);
            this.Tab_Overview.Controls.Add(this.MT_Money);
            this.Tab_Overview.Controls.Add(this.L_Money);
            this.Tab_Overview.Controls.Add(this.L_Language);
            this.Tab_Overview.Controls.Add(this.CB_Language);
            this.Tab_Overview.Controls.Add(this.B_MaxCash);
            this.Tab_Overview.Location = new System.Drawing.Point(4, 22);
            this.Tab_Overview.Name = "Tab_Overview";
            this.Tab_Overview.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_Overview.Size = new System.Drawing.Size(386, 290);
            this.Tab_Overview.TabIndex = 0;
            this.Tab_Overview.Text = "Overview";
            this.Tab_Overview.UseVisualStyleBackColor = true;
            //
            // TB_RivalName
            //
            this.TB_RivalName.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_RivalName.Location = new System.Drawing.Point(290, 6);
            this.TB_RivalName.MaxLength = 12;
            this.TB_RivalName.Name = "TB_RivalName";
            this.TB_RivalName.Size = new System.Drawing.Size(93, 20);
            this.TB_RivalName.TabIndex = 67;
            this.TB_RivalName.Text = "WWWWWWWWWWWW";
            this.TB_RivalName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_RivalName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ClickString);
            //
            // L_RivalName
            //
            this.L_RivalName.Location = new System.Drawing.Point(198, 8);
            this.L_RivalName.Name = "L_RivalName";
            this.L_RivalName.Size = new System.Drawing.Size(87, 16);
            this.L_RivalName.TabIndex = 68;
            this.L_RivalName.Text = "Rival Name:";
            this.L_RivalName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // trainerID1
            //
            this.trainerID1.Location = new System.Drawing.Point(6, 26);
            this.trainerID1.Name = "trainerID1";
            this.trainerID1.Size = new System.Drawing.Size(90, 64);
            this.trainerID1.TabIndex = 66;
            //
            // GB_Adventure
            //
            this.GB_Adventure.Controls.Add(this.MT_Seconds);
            this.GB_Adventure.Controls.Add(this.MT_Hours);
            this.GB_Adventure.Controls.Add(this.L_Seconds);
            this.GB_Adventure.Controls.Add(this.L_Hours);
            this.GB_Adventure.Controls.Add(this.MT_Minutes);
            this.GB_Adventure.Controls.Add(this.L_Minutes);
            this.GB_Adventure.Location = new System.Drawing.Point(3, 130);
            this.GB_Adventure.Name = "GB_Adventure";
            this.GB_Adventure.Size = new System.Drawing.Size(200, 151);
            this.GB_Adventure.TabIndex = 56;
            this.GB_Adventure.TabStop = false;
            this.GB_Adventure.Text = "Adventure Info";
            //
            // Tab_Misc
            //
            this.Tab_Misc.Controls.Add(this.CHK_UnlockZMove);
            this.Tab_Misc.Controls.Add(this.CHK_UnlockMega);
            this.Tab_Misc.Location = new System.Drawing.Point(4, 22);
            this.Tab_Misc.Name = "Tab_Misc";
            this.Tab_Misc.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_Misc.Size = new System.Drawing.Size(386, 290);
            this.Tab_Misc.TabIndex = 4;
            this.Tab_Misc.Text = "Misc";
            this.Tab_Misc.UseVisualStyleBackColor = true;
            //
            // CHK_UnlockZMove
            //
            this.CHK_UnlockZMove.AutoSize = true;
            this.CHK_UnlockZMove.Location = new System.Drawing.Point(6, 271);
            this.CHK_UnlockZMove.Name = "CHK_UnlockZMove";
            this.CHK_UnlockZMove.Size = new System.Drawing.Size(117, 17);
            this.CHK_UnlockZMove.TabIndex = 73;
            this.CHK_UnlockZMove.Text = "Unlocked Z-Moves";
            this.CHK_UnlockZMove.UseVisualStyleBackColor = true;
            //
            // CHK_UnlockMega
            //
            this.CHK_UnlockMega.AutoSize = true;
            this.CHK_UnlockMega.Location = new System.Drawing.Point(6, 254);
            this.CHK_UnlockMega.Name = "CHK_UnlockMega";
            this.CHK_UnlockMega.Size = new System.Drawing.Size(149, 17);
            this.CHK_UnlockMega.TabIndex = 72;
            this.CHK_UnlockMega.Text = "Unlocked Mega Evolution";
            this.CHK_UnlockMega.UseVisualStyleBackColor = true;
            //
            // SAV_Trainer7GG
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 366);
            this.Controls.Add(this.TC_Editor);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_Cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SAV_Trainer7GG";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Trainer Data Editor";
            this.TC_Editor.ResumeLayout(false);
            this.Tab_Overview.ResumeLayout(false);
            this.Tab_Overview.PerformLayout();
            this.GB_Adventure.ResumeLayout(false);
            this.GB_Adventure.PerformLayout();
            this.Tab_Misc.ResumeLayout(false);
            this.Tab_Misc.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.TextBox TB_OTName;
        private System.Windows.Forms.Label L_TrainerName;
        private System.Windows.Forms.MaskedTextBox MT_Money;
        private System.Windows.Forms.Label L_Money;
        private System.Windows.Forms.Label L_Saying5;
        private System.Windows.Forms.Label L_Saying4;
        private System.Windows.Forms.Label L_Saying3;
        private System.Windows.Forms.Label L_Saying2;
        private System.Windows.Forms.Label L_Saying1;
        private System.Windows.Forms.TextBox TB_Saying5;
        private System.Windows.Forms.TextBox TB_Saying4;
        private System.Windows.Forms.TextBox TB_Saying3;
        private System.Windows.Forms.TextBox TB_Saying2;
        private System.Windows.Forms.TextBox TB_Saying1;
        private System.Windows.Forms.ComboBox CB_Language;
        private System.Windows.Forms.Button B_MaxCash;
        private System.Windows.Forms.Label L_Language;
        private System.Windows.Forms.ComboBox CB_Game;
        private System.Windows.Forms.ComboBox CB_Gender;
        private System.Windows.Forms.Label L_Seconds;
        private System.Windows.Forms.Label L_Minutes;
        private System.Windows.Forms.MaskedTextBox MT_Seconds;
        private System.Windows.Forms.MaskedTextBox MT_Minutes;
        private System.Windows.Forms.Label L_Hours;
        private System.Windows.Forms.MaskedTextBox MT_Hours;
        private System.Windows.Forms.TabControl TC_Editor;
        private System.Windows.Forms.TabPage Tab_Overview;
        private System.Windows.Forms.GroupBox GB_Adventure;
        private System.Windows.Forms.TabPage Tab_Misc;
        private System.Windows.Forms.CheckBox CHK_UnlockMega;
        private System.Windows.Forms.CheckBox CHK_UnlockZMove;
        private Controls.TrainerID trainerID1;
        private System.Windows.Forms.TextBox TB_RivalName;
        private System.Windows.Forms.Label L_RivalName;
    }
}