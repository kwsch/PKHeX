namespace PKHeX.WinForms
{
    sealed partial class SAV_FlagWork8b
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
            this.B_Cancel = new System.Windows.Forms.Button();
            this.GB_FlagStatus = new System.Windows.Forms.GroupBox();
            this.CHK_CustomFlag = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.NUD_System = new System.Windows.Forms.NumericUpDown();
            this.CHK_CustomSystem = new System.Windows.Forms.CheckBox();
            this.B_ApplyFlag = new System.Windows.Forms.Button();
            this.B_ApplyWork = new System.Windows.Forms.Button();
            this.NUD_Work = new System.Windows.Forms.NumericUpDown();
            this.NUD_Flag = new System.Windows.Forms.NumericUpDown();
            this.CB_CustomWork = new System.Windows.Forms.ComboBox();
            this.L_CustomWork = new System.Windows.Forms.Label();
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
            this.TLP_Flags = new System.Windows.Forms.TableLayoutPanel();
            this.GB_System = new System.Windows.Forms.TabPage();
            this.TLP_System = new System.Windows.Forms.TableLayoutPanel();
            this.GB_Work = new System.Windows.Forms.TabPage();
            this.TLP_Work = new System.Windows.Forms.TableLayoutPanel();
            this.GB_Research = new System.Windows.Forms.TabPage();
            this.GB_FlagStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_System)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Work)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Flag)).BeginInit();
            this.GB_Researcher.SuspendLayout();
            this.TC_Features.SuspendLayout();
            this.GB_Flags.SuspendLayout();
            this.GB_System.SuspendLayout();
            this.GB_Work.SuspendLayout();
            this.GB_Research.SuspendLayout();
            this.SuspendLayout();
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(369, 330);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(75, 23);
            this.B_Cancel.TabIndex = 2;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // GB_FlagStatus
            // 
            this.GB_FlagStatus.Controls.Add(this.CHK_CustomFlag);
            this.GB_FlagStatus.Controls.Add(this.button1);
            this.GB_FlagStatus.Controls.Add(this.NUD_System);
            this.GB_FlagStatus.Controls.Add(this.CHK_CustomSystem);
            this.GB_FlagStatus.Controls.Add(this.B_ApplyFlag);
            this.GB_FlagStatus.Controls.Add(this.B_ApplyWork);
            this.GB_FlagStatus.Controls.Add(this.NUD_Work);
            this.GB_FlagStatus.Controls.Add(this.NUD_Flag);
            this.GB_FlagStatus.Controls.Add(this.CB_CustomWork);
            this.GB_FlagStatus.Controls.Add(this.L_CustomWork);
            this.GB_FlagStatus.Location = new System.Drawing.Point(3, 3);
            this.GB_FlagStatus.Name = "GB_FlagStatus";
            this.GB_FlagStatus.Size = new System.Drawing.Size(325, 103);
            this.GB_FlagStatus.TabIndex = 3;
            this.GB_FlagStatus.TabStop = false;
            this.GB_FlagStatus.Text = "Check Status";
            // 
            // CHK_CustomFlag
            // 
            this.CHK_CustomFlag.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CHK_CustomFlag.Location = new System.Drawing.Point(12, 16);
            this.CHK_CustomFlag.Name = "CHK_CustomFlag";
            this.CHK_CustomFlag.Size = new System.Drawing.Size(141, 23);
            this.CHK_CustomFlag.TabIndex = 45;
            this.CHK_CustomFlag.Text = "Event Flag:";
            this.CHK_CustomFlag.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CHK_CustomFlag.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(249, 38);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(68, 23);
            this.button1.TabIndex = 44;
            this.button1.Text = "Apply";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.B_ApplySystemFlag_Click);
            // 
            // NUD_System
            // 
            this.NUD_System.Location = new System.Drawing.Point(159, 40);
            this.NUD_System.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.NUD_System.Name = "NUD_System";
            this.NUD_System.Size = new System.Drawing.Size(45, 20);
            this.NUD_System.TabIndex = 43;
            this.NUD_System.ValueChanged += new System.EventHandler(this.ChangeCustomSystem);
            // 
            // CHK_CustomSystem
            // 
            this.CHK_CustomSystem.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CHK_CustomSystem.Location = new System.Drawing.Point(12, 39);
            this.CHK_CustomSystem.Name = "CHK_CustomSystem";
            this.CHK_CustomSystem.Size = new System.Drawing.Size(141, 23);
            this.CHK_CustomSystem.TabIndex = 41;
            this.CHK_CustomSystem.Text = "System Flag:";
            this.CHK_CustomSystem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CHK_CustomSystem.UseVisualStyleBackColor = true;
            // 
            // B_ApplyFlag
            // 
            this.B_ApplyFlag.Location = new System.Drawing.Point(249, 15);
            this.B_ApplyFlag.Name = "B_ApplyFlag";
            this.B_ApplyFlag.Size = new System.Drawing.Size(68, 23);
            this.B_ApplyFlag.TabIndex = 40;
            this.B_ApplyFlag.Text = "Apply";
            this.B_ApplyFlag.UseVisualStyleBackColor = true;
            this.B_ApplyFlag.Click += new System.EventHandler(this.B_ApplyFlag_Click);
            // 
            // B_ApplyWork
            // 
            this.B_ApplyWork.Location = new System.Drawing.Point(249, 70);
            this.B_ApplyWork.Name = "B_ApplyWork";
            this.B_ApplyWork.Size = new System.Drawing.Size(68, 23);
            this.B_ApplyWork.TabIndex = 39;
            this.B_ApplyWork.Text = "Apply";
            this.B_ApplyWork.UseVisualStyleBackColor = true;
            this.B_ApplyWork.Click += new System.EventHandler(this.B_ApplyWork_Click);
            // 
            // NUD_Work
            // 
            this.NUD_Work.Location = new System.Drawing.Point(159, 70);
            this.NUD_Work.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.NUD_Work.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.NUD_Work.Name = "NUD_Work";
            this.NUD_Work.Size = new System.Drawing.Size(84, 20);
            this.NUD_Work.TabIndex = 38;
            // 
            // NUD_Flag
            // 
            this.NUD_Flag.Location = new System.Drawing.Point(159, 17);
            this.NUD_Flag.Maximum = new decimal(new int[] {
            3999,
            0,
            0,
            0});
            this.NUD_Flag.Name = "NUD_Flag";
            this.NUD_Flag.Size = new System.Drawing.Size(45, 20);
            this.NUD_Flag.TabIndex = 9;
            this.NUD_Flag.ValueChanged += new System.EventHandler(this.ChangeCustomFlag);
            // 
            // CB_CustomWork
            // 
            this.CB_CustomWork.DropDownHeight = 156;
            this.CB_CustomWork.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_CustomWork.DropDownWidth = 180;
            this.CB_CustomWork.FormattingEnabled = true;
            this.CB_CustomWork.IntegralHeight = false;
            this.CB_CustomWork.Location = new System.Drawing.Point(87, 69);
            this.CB_CustomWork.Name = "CB_CustomWork";
            this.CB_CustomWork.Size = new System.Drawing.Size(66, 21);
            this.CB_CustomWork.TabIndex = 36;
            this.CB_CustomWork.SelectedIndexChanged += new System.EventHandler(this.ChangeConstantIndex);
            // 
            // L_CustomWork
            // 
            this.L_CustomWork.Location = new System.Drawing.Point(9, 70);
            this.L_CustomWork.Name = "L_CustomWork";
            this.L_CustomWork.Size = new System.Drawing.Size(72, 20);
            this.L_CustomWork.TabIndex = 37;
            this.L_CustomWork.Text = "Constant:";
            this.L_CustomWork.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(452, 330);
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
            this.GB_Researcher.Location = new System.Drawing.Point(3, 110);
            this.GB_Researcher.Name = "GB_Researcher";
            this.GB_Researcher.Size = new System.Drawing.Size(416, 170);
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
            this.RTB_Diff.Size = new System.Drawing.Size(413, 97);
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
            this.TC_Features.Controls.Add(this.GB_System);
            this.TC_Features.Controls.Add(this.GB_Work);
            this.TC_Features.Controls.Add(this.GB_Research);
            this.TC_Features.Location = new System.Drawing.Point(12, 12);
            this.TC_Features.Name = "TC_Features";
            this.TC_Features.SelectedIndex = 0;
            this.TC_Features.Size = new System.Drawing.Size(510, 309);
            this.TC_Features.TabIndex = 42;
            // 
            // GB_Flags
            // 
            this.GB_Flags.Controls.Add(this.TLP_Flags);
            this.GB_Flags.Location = new System.Drawing.Point(4, 22);
            this.GB_Flags.Name = "GB_Flags";
            this.GB_Flags.Padding = new System.Windows.Forms.Padding(3);
            this.GB_Flags.Size = new System.Drawing.Size(502, 283);
            this.GB_Flags.TabIndex = 0;
            this.GB_Flags.Text = "Event Flags";
            this.GB_Flags.UseVisualStyleBackColor = true;
            // 
            // TLP_Flags
            // 
            this.TLP_Flags.AutoScroll = true;
            this.TLP_Flags.ColumnCount = 2;
            this.TLP_Flags.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TLP_Flags.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TLP_Flags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLP_Flags.Location = new System.Drawing.Point(3, 3);
            this.TLP_Flags.Name = "TLP_Flags";
            this.TLP_Flags.RowCount = 2;
            this.TLP_Flags.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Flags.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Flags.Size = new System.Drawing.Size(496, 277);
            this.TLP_Flags.TabIndex = 1;
            // 
            // GB_System
            // 
            this.GB_System.Controls.Add(this.TLP_System);
            this.GB_System.Location = new System.Drawing.Point(4, 22);
            this.GB_System.Name = "GB_System";
            this.GB_System.Padding = new System.Windows.Forms.Padding(3);
            this.GB_System.Size = new System.Drawing.Size(422, 283);
            this.GB_System.TabIndex = 3;
            this.GB_System.Text = "System Flags";
            this.GB_System.UseVisualStyleBackColor = true;
            // 
            // TLP_System
            // 
            this.TLP_System.AutoScroll = true;
            this.TLP_System.ColumnCount = 2;
            this.TLP_System.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TLP_System.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TLP_System.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLP_System.Location = new System.Drawing.Point(3, 3);
            this.TLP_System.Name = "TLP_System";
            this.TLP_System.RowCount = 2;
            this.TLP_System.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_System.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_System.Size = new System.Drawing.Size(416, 277);
            this.TLP_System.TabIndex = 1;
            // 
            // GB_Work
            // 
            this.GB_Work.Controls.Add(this.TLP_Work);
            this.GB_Work.Location = new System.Drawing.Point(4, 22);
            this.GB_Work.Name = "GB_Work";
            this.GB_Work.Padding = new System.Windows.Forms.Padding(3);
            this.GB_Work.Size = new System.Drawing.Size(422, 283);
            this.GB_Work.TabIndex = 1;
            this.GB_Work.Text = "Work Values";
            this.GB_Work.UseVisualStyleBackColor = true;
            // 
            // TLP_Work
            // 
            this.TLP_Work.AutoScroll = true;
            this.TLP_Work.ColumnCount = 3;
            this.TLP_Work.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TLP_Work.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TLP_Work.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 416F));
            this.TLP_Work.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLP_Work.Location = new System.Drawing.Point(3, 3);
            this.TLP_Work.Name = "TLP_Work";
            this.TLP_Work.RowCount = 1;
            this.TLP_Work.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Work.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Work.Size = new System.Drawing.Size(416, 277);
            this.TLP_Work.TabIndex = 2;
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
            // SAV_FlagWork8b
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 361);
            this.Controls.Add(this.TC_Features);
            this.Controls.Add(this.L_EventFlagWarn);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_Cancel);
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(670, 800);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(470, 400);
            this.Name = "SAV_FlagWork8b";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Event Flag Editor";
            this.GB_FlagStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NUD_System)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Work)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Flag)).EndInit();
            this.GB_Researcher.ResumeLayout(false);
            this.GB_Researcher.PerformLayout();
            this.TC_Features.ResumeLayout(false);
            this.GB_Flags.ResumeLayout(false);
            this.GB_System.ResumeLayout(false);
            this.GB_Work.ResumeLayout(false);
            this.GB_Research.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.GroupBox GB_FlagStatus;
        private System.Windows.Forms.NumericUpDown NUD_Flag;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.GroupBox GB_Researcher;
        private System.Windows.Forms.TextBox TB_NewSAV;
        private System.Windows.Forms.TextBox TB_OldSAV;
        private System.Windows.Forms.Button B_LoadNew;
        private System.Windows.Forms.Button B_LoadOld;
        private System.Windows.Forms.Label L_CustomWork;
        private System.Windows.Forms.ComboBox CB_CustomWork;
        private System.Windows.Forms.Label L_EventFlagWarn;
        private System.Windows.Forms.TabControl TC_Features;
        private System.Windows.Forms.TabPage GB_Flags;
        private System.Windows.Forms.TabPage GB_Work;
        private System.Windows.Forms.TabPage GB_Research;
        private System.Windows.Forms.NumericUpDown NUD_Work;
        private System.Windows.Forms.Button B_ApplyFlag;
        private System.Windows.Forms.Button B_ApplyWork;
        private System.Windows.Forms.RichTextBox RTB_Diff;
        private System.Windows.Forms.TabPage GB_System;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.NumericUpDown NUD_System;
        private System.Windows.Forms.CheckBox CHK_CustomSystem;
        private System.Windows.Forms.CheckBox CHK_CustomFlag;
        private System.Windows.Forms.TableLayoutPanel TLP_Flags;
        private System.Windows.Forms.TableLayoutPanel TLP_System;
        private System.Windows.Forms.TableLayoutPanel TLP_Work;
    }
}
