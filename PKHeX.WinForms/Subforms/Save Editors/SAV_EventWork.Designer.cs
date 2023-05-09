namespace PKHeX.WinForms
{
    sealed partial class SAV_EventWork
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
            c_CustomFlag = new System.Windows.Forms.CheckBox();
            B_Cancel = new System.Windows.Forms.Button();
            GB_FlagStatus = new System.Windows.Forms.GroupBox();
            B_ApplyFlag = new System.Windows.Forms.Button();
            B_ApplyWork = new System.Windows.Forms.Button();
            NUD_Stat = new System.Windows.Forms.NumericUpDown();
            NUD_Flag = new System.Windows.Forms.NumericUpDown();
            CHK_CustomFlag = new System.Windows.Forms.Label();
            CB_Stats = new System.Windows.Forms.ComboBox();
            L_Stats = new System.Windows.Forms.Label();
            B_Save = new System.Windows.Forms.Button();
            GB_Researcher = new System.Windows.Forms.GroupBox();
            RTB_Diff = new System.Windows.Forms.RichTextBox();
            TB_NewSAV = new System.Windows.Forms.TextBox();
            TB_OldSAV = new System.Windows.Forms.TextBox();
            B_LoadNew = new System.Windows.Forms.Button();
            B_LoadOld = new System.Windows.Forms.Button();
            L_EventFlagWarn = new System.Windows.Forms.Label();
            TC_Features = new System.Windows.Forms.TabControl();
            GB_Flags = new System.Windows.Forms.TabPage();
            TC_Flag = new System.Windows.Forms.TabControl();
            GB_Constants = new System.Windows.Forms.TabPage();
            TC_Work = new System.Windows.Forms.TabControl();
            GB_Research = new System.Windows.Forms.TabPage();
            GB_FlagStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Stat).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Flag).BeginInit();
            GB_Researcher.SuspendLayout();
            TC_Features.SuspendLayout();
            GB_Flags.SuspendLayout();
            GB_Constants.SuspendLayout();
            GB_Research.SuspendLayout();
            SuspendLayout();
            // 
            // c_CustomFlag
            // 
            c_CustomFlag.AutoSize = true;
            c_CustomFlag.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            c_CustomFlag.Location = new System.Drawing.Point(161, 23);
            c_CustomFlag.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            c_CustomFlag.Name = "c_CustomFlag";
            c_CustomFlag.Size = new System.Drawing.Size(15, 14);
            c_CustomFlag.TabIndex = 1;
            c_CustomFlag.UseVisualStyleBackColor = true;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(337, 381);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(88, 27);
            B_Cancel.TabIndex = 2;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // GB_FlagStatus
            // 
            GB_FlagStatus.Controls.Add(B_ApplyFlag);
            GB_FlagStatus.Controls.Add(B_ApplyWork);
            GB_FlagStatus.Controls.Add(NUD_Stat);
            GB_FlagStatus.Controls.Add(NUD_Flag);
            GB_FlagStatus.Controls.Add(CHK_CustomFlag);
            GB_FlagStatus.Controls.Add(CB_Stats);
            GB_FlagStatus.Controls.Add(L_Stats);
            GB_FlagStatus.Controls.Add(c_CustomFlag);
            GB_FlagStatus.Location = new System.Drawing.Point(7, 6);
            GB_FlagStatus.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_FlagStatus.Name = "GB_FlagStatus";
            GB_FlagStatus.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_FlagStatus.Size = new System.Drawing.Size(377, 87);
            GB_FlagStatus.TabIndex = 3;
            GB_FlagStatus.TabStop = false;
            GB_FlagStatus.Text = "Check Status";
            // 
            // B_ApplyFlag
            // 
            B_ApplyFlag.Location = new System.Drawing.Point(186, 16);
            B_ApplyFlag.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ApplyFlag.Name = "B_ApplyFlag";
            B_ApplyFlag.Size = new System.Drawing.Size(79, 27);
            B_ApplyFlag.TabIndex = 40;
            B_ApplyFlag.Text = "Apply";
            B_ApplyFlag.UseVisualStyleBackColor = true;
            B_ApplyFlag.Click += B_ApplyFlag_Click;
            // 
            // B_ApplyWork
            // 
            B_ApplyWork.Location = new System.Drawing.Point(290, 52);
            B_ApplyWork.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ApplyWork.Name = "B_ApplyWork";
            B_ApplyWork.Size = new System.Drawing.Size(79, 27);
            B_ApplyWork.TabIndex = 39;
            B_ApplyWork.Text = "Apply";
            B_ApplyWork.UseVisualStyleBackColor = true;
            B_ApplyWork.Click += B_ApplyWork_Click;
            // 
            // NUD_Stat
            // 
            NUD_Stat.Location = new System.Drawing.Point(186, 52);
            NUD_Stat.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_Stat.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            NUD_Stat.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
            NUD_Stat.Name = "NUD_Stat";
            NUD_Stat.Size = new System.Drawing.Size(98, 23);
            NUD_Stat.TabIndex = 38;
            // 
            // NUD_Flag
            // 
            NUD_Flag.Location = new System.Drawing.Point(102, 20);
            NUD_Flag.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_Flag.Maximum = new decimal(new int[] { 3072, 0, 0, 0 });
            NUD_Flag.Name = "NUD_Flag";
            NUD_Flag.Size = new System.Drawing.Size(52, 23);
            NUD_Flag.TabIndex = 9;
            NUD_Flag.ValueChanged += ChangeCustomFlag;
            // 
            // CHK_CustomFlag
            // 
            CHK_CustomFlag.Location = new System.Drawing.Point(10, 20);
            CHK_CustomFlag.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            CHK_CustomFlag.Name = "CHK_CustomFlag";
            CHK_CustomFlag.Size = new System.Drawing.Size(84, 23);
            CHK_CustomFlag.TabIndex = 2;
            CHK_CustomFlag.Text = "Flag:";
            CHK_CustomFlag.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_Stats
            // 
            CB_Stats.DropDownHeight = 156;
            CB_Stats.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Stats.DropDownWidth = 180;
            CB_Stats.FormattingEnabled = true;
            CB_Stats.IntegralHeight = false;
            CB_Stats.Location = new System.Drawing.Point(102, 51);
            CB_Stats.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Stats.Name = "CB_Stats";
            CB_Stats.Size = new System.Drawing.Size(76, 23);
            CB_Stats.TabIndex = 36;
            CB_Stats.SelectedIndexChanged += ChangeConstantIndex;
            // 
            // L_Stats
            // 
            L_Stats.Location = new System.Drawing.Point(10, 52);
            L_Stats.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Stats.Name = "L_Stats";
            L_Stats.Size = new System.Drawing.Size(84, 23);
            L_Stats.TabIndex = 37;
            L_Stats.Text = "Constant:";
            L_Stats.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(434, 381);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(88, 27);
            B_Save.TabIndex = 9;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // GB_Researcher
            // 
            GB_Researcher.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            GB_Researcher.Controls.Add(RTB_Diff);
            GB_Researcher.Controls.Add(TB_NewSAV);
            GB_Researcher.Controls.Add(TB_OldSAV);
            GB_Researcher.Controls.Add(B_LoadNew);
            GB_Researcher.Controls.Add(B_LoadOld);
            GB_Researcher.Location = new System.Drawing.Point(4, 99);
            GB_Researcher.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Researcher.Name = "GB_Researcher";
            GB_Researcher.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Researcher.Size = new System.Drawing.Size(485, 224);
            GB_Researcher.TabIndex = 13;
            GB_Researcher.TabStop = false;
            GB_Researcher.Text = "FlagDiff Researcher";
            // 
            // RTB_Diff
            // 
            RTB_Diff.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            RTB_Diff.Location = new System.Drawing.Point(4, 84);
            RTB_Diff.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RTB_Diff.Name = "RTB_Diff";
            RTB_Diff.ReadOnly = true;
            RTB_Diff.Size = new System.Drawing.Size(481, 139);
            RTB_Diff.TabIndex = 6;
            RTB_Diff.Text = "";
            // 
            // TB_NewSAV
            // 
            TB_NewSAV.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TB_NewSAV.Location = new System.Drawing.Point(108, 54);
            TB_NewSAV.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_NewSAV.Name = "TB_NewSAV";
            TB_NewSAV.ReadOnly = true;
            TB_NewSAV.Size = new System.Drawing.Size(369, 23);
            TB_NewSAV.TabIndex = 5;
            // 
            // TB_OldSAV
            // 
            TB_OldSAV.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TB_OldSAV.Location = new System.Drawing.Point(108, 24);
            TB_OldSAV.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_OldSAV.Name = "TB_OldSAV";
            TB_OldSAV.ReadOnly = true;
            TB_OldSAV.Size = new System.Drawing.Size(369, 23);
            TB_OldSAV.TabIndex = 4;
            // 
            // B_LoadNew
            // 
            B_LoadNew.Location = new System.Drawing.Point(14, 52);
            B_LoadNew.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_LoadNew.Name = "B_LoadNew";
            B_LoadNew.Size = new System.Drawing.Size(88, 27);
            B_LoadNew.TabIndex = 1;
            B_LoadNew.Text = "Load New";
            B_LoadNew.UseVisualStyleBackColor = true;
            B_LoadNew.Click += OpenSAV;
            // 
            // B_LoadOld
            // 
            B_LoadOld.Location = new System.Drawing.Point(14, 22);
            B_LoadOld.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_LoadOld.Name = "B_LoadOld";
            B_LoadOld.Size = new System.Drawing.Size(88, 27);
            B_LoadOld.TabIndex = 0;
            B_LoadOld.Text = "Load Old";
            B_LoadOld.UseVisualStyleBackColor = true;
            B_LoadOld.Click += OpenSAV;
            // 
            // L_EventFlagWarn
            // 
            L_EventFlagWarn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            L_EventFlagWarn.ForeColor = System.Drawing.Color.Red;
            L_EventFlagWarn.Location = new System.Drawing.Point(10, 374);
            L_EventFlagWarn.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_EventFlagWarn.Name = "L_EventFlagWarn";
            L_EventFlagWarn.Size = new System.Drawing.Size(306, 36);
            L_EventFlagWarn.TabIndex = 41;
            L_EventFlagWarn.Text = "Altering Event Flags may impact other story events. Save file backups are recommended.";
            L_EventFlagWarn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TC_Features
            // 
            TC_Features.AllowDrop = true;
            TC_Features.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TC_Features.Controls.Add(GB_Flags);
            TC_Features.Controls.Add(GB_Constants);
            TC_Features.Controls.Add(GB_Research);
            TC_Features.Location = new System.Drawing.Point(14, 14);
            TC_Features.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TC_Features.Name = "TC_Features";
            TC_Features.SelectedIndex = 0;
            TC_Features.Size = new System.Drawing.Size(502, 357);
            TC_Features.TabIndex = 42;
            // 
            // GB_Flags
            // 
            GB_Flags.Controls.Add(TC_Flag);
            GB_Flags.Location = new System.Drawing.Point(4, 24);
            GB_Flags.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Flags.Name = "GB_Flags";
            GB_Flags.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Flags.Size = new System.Drawing.Size(494, 329);
            GB_Flags.TabIndex = 0;
            GB_Flags.Text = "Event Flags";
            GB_Flags.UseVisualStyleBackColor = true;
            // 
            // TC_Flag
            // 
            TC_Flag.Dock = System.Windows.Forms.DockStyle.Fill;
            TC_Flag.Location = new System.Drawing.Point(4, 3);
            TC_Flag.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TC_Flag.Name = "TC_Flag";
            TC_Flag.SelectedIndex = 0;
            TC_Flag.Size = new System.Drawing.Size(486, 323);
            TC_Flag.TabIndex = 1;
            // 
            // GB_Constants
            // 
            GB_Constants.Controls.Add(TC_Work);
            GB_Constants.Location = new System.Drawing.Point(4, 24);
            GB_Constants.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Constants.Name = "GB_Constants";
            GB_Constants.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Constants.Size = new System.Drawing.Size(494, 329);
            GB_Constants.TabIndex = 1;
            GB_Constants.Text = "Event Constants";
            GB_Constants.UseVisualStyleBackColor = true;
            // 
            // TC_Work
            // 
            TC_Work.Dock = System.Windows.Forms.DockStyle.Fill;
            TC_Work.Location = new System.Drawing.Point(4, 3);
            TC_Work.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TC_Work.Name = "TC_Work";
            TC_Work.SelectedIndex = 0;
            TC_Work.Size = new System.Drawing.Size(486, 323);
            TC_Work.TabIndex = 0;
            // 
            // GB_Research
            // 
            GB_Research.Controls.Add(GB_FlagStatus);
            GB_Research.Controls.Add(GB_Researcher);
            GB_Research.Location = new System.Drawing.Point(4, 24);
            GB_Research.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Research.Name = "GB_Research";
            GB_Research.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Research.Size = new System.Drawing.Size(494, 329);
            GB_Research.TabIndex = 2;
            GB_Research.Text = "Research";
            GB_Research.UseVisualStyleBackColor = true;
            // 
            // SAV_EventWork
            // 
            AllowDrop = true;
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(530, 417);
            Controls.Add(TC_Features);
            Controls.Add(L_EventFlagWarn);
            Controls.Add(B_Save);
            Controls.Add(B_Cancel);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(779, 917);
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(546, 456);
            Name = "SAV_EventWork";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Event Flag Editor";
            GB_FlagStatus.ResumeLayout(false);
            GB_FlagStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Stat).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Flag).EndInit();
            GB_Researcher.ResumeLayout(false);
            GB_Researcher.PerformLayout();
            TC_Features.ResumeLayout(false);
            GB_Flags.ResumeLayout(false);
            GB_Constants.ResumeLayout(false);
            GB_Research.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.CheckBox c_CustomFlag;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.GroupBox GB_FlagStatus;
        private System.Windows.Forms.Label CHK_CustomFlag;
        private System.Windows.Forms.NumericUpDown NUD_Flag;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.GroupBox GB_Researcher;
        private System.Windows.Forms.TextBox TB_NewSAV;
        private System.Windows.Forms.TextBox TB_OldSAV;
        private System.Windows.Forms.Button B_LoadNew;
        private System.Windows.Forms.Button B_LoadOld;
        private System.Windows.Forms.Label L_Stats;
        private System.Windows.Forms.ComboBox CB_Stats;
        private System.Windows.Forms.Label L_EventFlagWarn;
        private System.Windows.Forms.TabControl TC_Features;
        private System.Windows.Forms.TabPage GB_Flags;
        private System.Windows.Forms.TabPage GB_Constants;
        private System.Windows.Forms.TabPage GB_Research;
        private System.Windows.Forms.TabControl TC_Work;
        private System.Windows.Forms.TabControl TC_Flag;
        private System.Windows.Forms.NumericUpDown NUD_Stat;
        private System.Windows.Forms.Button B_ApplyFlag;
        private System.Windows.Forms.Button B_ApplyWork;
        private System.Windows.Forms.RichTextBox RTB_Diff;
    }
}
