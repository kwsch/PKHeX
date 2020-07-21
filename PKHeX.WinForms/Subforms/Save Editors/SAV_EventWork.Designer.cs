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
            this.c_CustomFlag = new System.Windows.Forms.CheckBox();
            this.B_Cancel = new System.Windows.Forms.Button();
            this.GB_FlagStatus = new System.Windows.Forms.GroupBox();
            this.B_ApplyFlag = new System.Windows.Forms.Button();
            this.B_ApplyWork = new System.Windows.Forms.Button();
            this.NUD_Stat = new System.Windows.Forms.NumericUpDown();
            this.NUD_Flag = new System.Windows.Forms.NumericUpDown();
            this.CHK_CustomFlag = new System.Windows.Forms.Label();
            this.CB_Stats = new System.Windows.Forms.ComboBox();
            this.L_Stats = new System.Windows.Forms.Label();
            this.B_Save = new System.Windows.Forms.Button();
            this.GB_Researcher = new System.Windows.Forms.GroupBox();
            this.RTB_Diff = new System.Windows.Forms.RichTextBox();
            this.TB_NewSAV = new System.Windows.Forms.TextBox();
            this.TB_OldSAV = new System.Windows.Forms.TextBox();
            this.B_LoadNew = new System.Windows.Forms.Button();
            this.B_LoadOld = new System.Windows.Forms.Button();
            this.L_EventFlagWarn = new System.Windows.Forms.Label();
            this.TC_Features = new System.Windows.Forms.TabControl();
            this.GB_Flags = new System.Windows.Forms.TabPage();
            this.TC_Flag = new System.Windows.Forms.TabControl();
            this.GB_Constants = new System.Windows.Forms.TabPage();
            this.TC_Work = new System.Windows.Forms.TabControl();
            this.GB_Research = new System.Windows.Forms.TabPage();
            this.GB_FlagStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Stat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Flag)).BeginInit();
            this.GB_Researcher.SuspendLayout();
            this.TC_Features.SuspendLayout();
            this.GB_Flags.SuspendLayout();
            this.GB_Constants.SuspendLayout();
            this.GB_Research.SuspendLayout();
            this.SuspendLayout();
            // 
            // c_CustomFlag
            // 
            this.c_CustomFlag.AutoSize = true;
            this.c_CustomFlag.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.c_CustomFlag.Location = new System.Drawing.Point(138, 20);
            this.c_CustomFlag.Name = "c_CustomFlag";
            this.c_CustomFlag.Size = new System.Drawing.Size(15, 14);
            this.c_CustomFlag.TabIndex = 1;
            this.c_CustomFlag.UseVisualStyleBackColor = true;
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(289, 330);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(75, 23);
            this.B_Cancel.TabIndex = 2;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // GB_FlagStatus
            // 
            this.GB_FlagStatus.Controls.Add(this.B_ApplyFlag);
            this.GB_FlagStatus.Controls.Add(this.B_ApplyWork);
            this.GB_FlagStatus.Controls.Add(this.NUD_Stat);
            this.GB_FlagStatus.Controls.Add(this.NUD_Flag);
            this.GB_FlagStatus.Controls.Add(this.CHK_CustomFlag);
            this.GB_FlagStatus.Controls.Add(this.CB_Stats);
            this.GB_FlagStatus.Controls.Add(this.L_Stats);
            this.GB_FlagStatus.Controls.Add(this.c_CustomFlag);
            this.GB_FlagStatus.Location = new System.Drawing.Point(6, 5);
            this.GB_FlagStatus.Name = "GB_FlagStatus";
            this.GB_FlagStatus.Size = new System.Drawing.Size(323, 75);
            this.GB_FlagStatus.TabIndex = 3;
            this.GB_FlagStatus.TabStop = false;
            this.GB_FlagStatus.Text = "Check Status";
            // 
            // B_ApplyFlag
            // 
            this.B_ApplyFlag.Location = new System.Drawing.Point(159, 14);
            this.B_ApplyFlag.Name = "B_ApplyFlag";
            this.B_ApplyFlag.Size = new System.Drawing.Size(68, 23);
            this.B_ApplyFlag.TabIndex = 40;
            this.B_ApplyFlag.Text = "Apply";
            this.B_ApplyFlag.UseVisualStyleBackColor = true;
            this.B_ApplyFlag.Click += new System.EventHandler(this.B_ApplyFlag_Click);
            // 
            // B_ApplyWork
            // 
            this.B_ApplyWork.Location = new System.Drawing.Point(249, 45);
            this.B_ApplyWork.Name = "B_ApplyWork";
            this.B_ApplyWork.Size = new System.Drawing.Size(68, 23);
            this.B_ApplyWork.TabIndex = 39;
            this.B_ApplyWork.Text = "Apply";
            this.B_ApplyWork.UseVisualStyleBackColor = true;
            this.B_ApplyWork.Click += new System.EventHandler(this.B_ApplyWork_Click);
            // 
            // NUD_Stat
            // 
            this.NUD_Stat.Location = new System.Drawing.Point(159, 45);
            this.NUD_Stat.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.NUD_Stat.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.NUD_Stat.Name = "NUD_Stat";
            this.NUD_Stat.Size = new System.Drawing.Size(84, 20);
            this.NUD_Stat.TabIndex = 38;
            // 
            // NUD_Flag
            // 
            this.NUD_Flag.Location = new System.Drawing.Point(87, 17);
            this.NUD_Flag.Maximum = new decimal(new int[] {
            3072,
            0,
            0,
            0});
            this.NUD_Flag.Name = "NUD_Flag";
            this.NUD_Flag.Size = new System.Drawing.Size(45, 20);
            this.NUD_Flag.TabIndex = 9;
            this.NUD_Flag.ValueChanged += new System.EventHandler(this.ChangeCustomFlag);
            // 
            // CHK_CustomFlag
            // 
            this.CHK_CustomFlag.Location = new System.Drawing.Point(9, 17);
            this.CHK_CustomFlag.Name = "CHK_CustomFlag";
            this.CHK_CustomFlag.Size = new System.Drawing.Size(72, 20);
            this.CHK_CustomFlag.TabIndex = 2;
            this.CHK_CustomFlag.Text = "Flag:";
            this.CHK_CustomFlag.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_Stats
            // 
            this.CB_Stats.DropDownHeight = 156;
            this.CB_Stats.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Stats.DropDownWidth = 180;
            this.CB_Stats.FormattingEnabled = true;
            this.CB_Stats.IntegralHeight = false;
            this.CB_Stats.Location = new System.Drawing.Point(87, 44);
            this.CB_Stats.Name = "CB_Stats";
            this.CB_Stats.Size = new System.Drawing.Size(66, 21);
            this.CB_Stats.TabIndex = 36;
            this.CB_Stats.SelectedIndexChanged += new System.EventHandler(this.ChangeConstantIndex);
            // 
            // L_Stats
            // 
            this.L_Stats.Location = new System.Drawing.Point(9, 45);
            this.L_Stats.Name = "L_Stats";
            this.L_Stats.Size = new System.Drawing.Size(72, 20);
            this.L_Stats.TabIndex = 37;
            this.L_Stats.Text = "Constant:";
            this.L_Stats.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(372, 330);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(75, 23);
            this.B_Save.TabIndex = 9;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // GB_Researcher
            // 
            this.GB_Researcher.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GB_Researcher.Controls.Add(this.RTB_Diff);
            this.GB_Researcher.Controls.Add(this.TB_NewSAV);
            this.GB_Researcher.Controls.Add(this.TB_OldSAV);
            this.GB_Researcher.Controls.Add(this.B_LoadNew);
            this.GB_Researcher.Controls.Add(this.B_LoadOld);
            this.GB_Researcher.Location = new System.Drawing.Point(3, 86);
            this.GB_Researcher.Name = "GB_Researcher";
            this.GB_Researcher.Size = new System.Drawing.Size(416, 194);
            this.GB_Researcher.TabIndex = 13;
            this.GB_Researcher.TabStop = false;
            this.GB_Researcher.Text = "FlagDiff Researcher";
            // 
            // RTB_Diff
            // 
            this.RTB_Diff.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RTB_Diff.Location = new System.Drawing.Point(3, 73);
            this.RTB_Diff.Name = "RTB_Diff";
            this.RTB_Diff.ReadOnly = true;
            this.RTB_Diff.Size = new System.Drawing.Size(413, 121);
            this.RTB_Diff.TabIndex = 6;
            this.RTB_Diff.Text = "";
            // 
            // TB_NewSAV
            // 
            this.TB_NewSAV.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_NewSAV.Location = new System.Drawing.Point(93, 47);
            this.TB_NewSAV.Name = "TB_NewSAV";
            this.TB_NewSAV.ReadOnly = true;
            this.TB_NewSAV.Size = new System.Drawing.Size(317, 20);
            this.TB_NewSAV.TabIndex = 5;
            // 
            // TB_OldSAV
            // 
            this.TB_OldSAV.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_OldSAV.Location = new System.Drawing.Point(93, 21);
            this.TB_OldSAV.Name = "TB_OldSAV";
            this.TB_OldSAV.ReadOnly = true;
            this.TB_OldSAV.Size = new System.Drawing.Size(317, 20);
            this.TB_OldSAV.TabIndex = 4;
            // 
            // B_LoadNew
            // 
            this.B_LoadNew.Location = new System.Drawing.Point(12, 45);
            this.B_LoadNew.Name = "B_LoadNew";
            this.B_LoadNew.Size = new System.Drawing.Size(75, 23);
            this.B_LoadNew.TabIndex = 1;
            this.B_LoadNew.Text = "Load New";
            this.B_LoadNew.UseVisualStyleBackColor = true;
            this.B_LoadNew.Click += new System.EventHandler(this.OpenSAV);
            // 
            // B_LoadOld
            // 
            this.B_LoadOld.Location = new System.Drawing.Point(12, 19);
            this.B_LoadOld.Name = "B_LoadOld";
            this.B_LoadOld.Size = new System.Drawing.Size(75, 23);
            this.B_LoadOld.TabIndex = 0;
            this.B_LoadOld.Text = "Load Old";
            this.B_LoadOld.UseVisualStyleBackColor = true;
            this.B_LoadOld.Click += new System.EventHandler(this.OpenSAV);
            // 
            // L_EventFlagWarn
            // 
            this.L_EventFlagWarn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.L_EventFlagWarn.ForeColor = System.Drawing.Color.Red;
            this.L_EventFlagWarn.Location = new System.Drawing.Point(9, 324);
            this.L_EventFlagWarn.Name = "L_EventFlagWarn";
            this.L_EventFlagWarn.Size = new System.Drawing.Size(262, 31);
            this.L_EventFlagWarn.TabIndex = 41;
            this.L_EventFlagWarn.Text = "Altering Event Flags may impact other story events. Save file backups are recomme" +
    "nded.";
            this.L_EventFlagWarn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TC_Features
            // 
            this.TC_Features.AllowDrop = true;
            this.TC_Features.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TC_Features.Controls.Add(this.GB_Flags);
            this.TC_Features.Controls.Add(this.GB_Constants);
            this.TC_Features.Controls.Add(this.GB_Research);
            this.TC_Features.Location = new System.Drawing.Point(12, 12);
            this.TC_Features.Name = "TC_Features";
            this.TC_Features.SelectedIndex = 0;
            this.TC_Features.Size = new System.Drawing.Size(430, 309);
            this.TC_Features.TabIndex = 42;
            // 
            // GB_Flags
            // 
            this.GB_Flags.Controls.Add(this.TC_Flag);
            this.GB_Flags.Location = new System.Drawing.Point(4, 22);
            this.GB_Flags.Name = "GB_Flags";
            this.GB_Flags.Padding = new System.Windows.Forms.Padding(3);
            this.GB_Flags.Size = new System.Drawing.Size(422, 283);
            this.GB_Flags.TabIndex = 0;
            this.GB_Flags.Text = "Event Flags";
            this.GB_Flags.UseVisualStyleBackColor = true;
            // 
            // TC_Flag
            // 
            this.TC_Flag.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TC_Flag.Location = new System.Drawing.Point(3, 3);
            this.TC_Flag.Name = "TC_Flag";
            this.TC_Flag.SelectedIndex = 0;
            this.TC_Flag.Size = new System.Drawing.Size(416, 277);
            this.TC_Flag.TabIndex = 1;
            // 
            // GB_Constants
            // 
            this.GB_Constants.Controls.Add(this.TC_Work);
            this.GB_Constants.Location = new System.Drawing.Point(4, 22);
            this.GB_Constants.Name = "GB_Constants";
            this.GB_Constants.Padding = new System.Windows.Forms.Padding(3);
            this.GB_Constants.Size = new System.Drawing.Size(422, 283);
            this.GB_Constants.TabIndex = 1;
            this.GB_Constants.Text = "Event Constants";
            this.GB_Constants.UseVisualStyleBackColor = true;
            // 
            // TC_Work
            // 
            this.TC_Work.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TC_Work.Location = new System.Drawing.Point(3, 3);
            this.TC_Work.Name = "TC_Work";
            this.TC_Work.SelectedIndex = 0;
            this.TC_Work.Size = new System.Drawing.Size(416, 277);
            this.TC_Work.TabIndex = 0;
            // 
            // GB_Research
            // 
            this.GB_Research.Controls.Add(this.GB_FlagStatus);
            this.GB_Research.Controls.Add(this.GB_Researcher);
            this.GB_Research.Location = new System.Drawing.Point(4, 22);
            this.GB_Research.Name = "GB_Research";
            this.GB_Research.Padding = new System.Windows.Forms.Padding(3);
            this.GB_Research.Size = new System.Drawing.Size(422, 283);
            this.GB_Research.TabIndex = 2;
            this.GB_Research.Text = "Research";
            this.GB_Research.UseVisualStyleBackColor = true;
            // 
            // SAV_EventWork
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 361);
            this.Controls.Add(this.TC_Features);
            this.Controls.Add(this.L_EventFlagWarn);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_Cancel);
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(670, 800);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(470, 400);
            this.Name = "SAV_EventWork";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Event Flag Editor";
            this.GB_FlagStatus.ResumeLayout(false);
            this.GB_FlagStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Stat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Flag)).EndInit();
            this.GB_Researcher.ResumeLayout(false);
            this.GB_Researcher.PerformLayout();
            this.TC_Features.ResumeLayout(false);
            this.GB_Flags.ResumeLayout(false);
            this.GB_Constants.ResumeLayout(false);
            this.GB_Research.ResumeLayout(false);
            this.ResumeLayout(false);

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