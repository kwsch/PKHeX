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
            B_Cancel = new System.Windows.Forms.Button();
            GB_FlagStatus = new System.Windows.Forms.GroupBox();
            NUD_WorkIndex = new System.Windows.Forms.NumericUpDown();
            CHK_CustomFlag = new System.Windows.Forms.CheckBox();
            button1 = new System.Windows.Forms.Button();
            NUD_System = new System.Windows.Forms.NumericUpDown();
            CHK_CustomSystem = new System.Windows.Forms.CheckBox();
            B_ApplyFlag = new System.Windows.Forms.Button();
            B_ApplyWork = new System.Windows.Forms.Button();
            NUD_Work = new System.Windows.Forms.NumericUpDown();
            NUD_Flag = new System.Windows.Forms.NumericUpDown();
            L_CustomWork = new System.Windows.Forms.Label();
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
            TLP_Flags = new System.Windows.Forms.TableLayoutPanel();
            GB_System = new System.Windows.Forms.TabPage();
            TLP_System = new System.Windows.Forms.TableLayoutPanel();
            GB_Work = new System.Windows.Forms.TabPage();
            TLP_Work = new System.Windows.Forms.TableLayoutPanel();
            GB_Research = new System.Windows.Forms.TabPage();
            GB_FlagStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_WorkIndex).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_System).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Work).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Flag).BeginInit();
            GB_Researcher.SuspendLayout();
            TC_Features.SuspendLayout();
            GB_Flags.SuspendLayout();
            GB_System.SuspendLayout();
            GB_Work.SuspendLayout();
            GB_Research.SuspendLayout();
            SuspendLayout();
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(430, 381);
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
            GB_FlagStatus.Controls.Add(NUD_WorkIndex);
            GB_FlagStatus.Controls.Add(CHK_CustomFlag);
            GB_FlagStatus.Controls.Add(button1);
            GB_FlagStatus.Controls.Add(NUD_System);
            GB_FlagStatus.Controls.Add(CHK_CustomSystem);
            GB_FlagStatus.Controls.Add(B_ApplyFlag);
            GB_FlagStatus.Controls.Add(B_ApplyWork);
            GB_FlagStatus.Controls.Add(NUD_Work);
            GB_FlagStatus.Controls.Add(NUD_Flag);
            GB_FlagStatus.Controls.Add(L_CustomWork);
            GB_FlagStatus.Location = new System.Drawing.Point(4, 3);
            GB_FlagStatus.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_FlagStatus.Name = "GB_FlagStatus";
            GB_FlagStatus.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_FlagStatus.Size = new System.Drawing.Size(379, 119);
            GB_FlagStatus.TabIndex = 3;
            GB_FlagStatus.TabStop = false;
            GB_FlagStatus.Text = "Check Status";
            // 
            // NUD_WorkIndex
            // 
            NUD_WorkIndex.Location = new System.Drawing.Point(102, 81);
            NUD_WorkIndex.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_WorkIndex.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            NUD_WorkIndex.Name = "NUD_WorkIndex";
            NUD_WorkIndex.Size = new System.Drawing.Size(52, 23);
            NUD_WorkIndex.TabIndex = 46;
            NUD_WorkIndex.ValueChanged += ChangeConstantIndex;
            // 
            // CHK_CustomFlag
            // 
            CHK_CustomFlag.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            CHK_CustomFlag.Location = new System.Drawing.Point(14, 18);
            CHK_CustomFlag.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_CustomFlag.Name = "CHK_CustomFlag";
            CHK_CustomFlag.Size = new System.Drawing.Size(164, 27);
            CHK_CustomFlag.TabIndex = 45;
            CHK_CustomFlag.Text = "Event Flag:";
            CHK_CustomFlag.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            CHK_CustomFlag.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(290, 44);
            button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(79, 27);
            button1.TabIndex = 44;
            button1.Text = "Apply";
            button1.UseVisualStyleBackColor = true;
            button1.Click += B_ApplySystemFlag_Click;
            // 
            // NUD_System
            // 
            NUD_System.Location = new System.Drawing.Point(186, 46);
            NUD_System.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_System.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            NUD_System.Name = "NUD_System";
            NUD_System.Size = new System.Drawing.Size(52, 23);
            NUD_System.TabIndex = 43;
            NUD_System.ValueChanged += ChangeCustomSystem;
            // 
            // CHK_CustomSystem
            // 
            CHK_CustomSystem.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            CHK_CustomSystem.Location = new System.Drawing.Point(14, 45);
            CHK_CustomSystem.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_CustomSystem.Name = "CHK_CustomSystem";
            CHK_CustomSystem.Size = new System.Drawing.Size(164, 27);
            CHK_CustomSystem.TabIndex = 41;
            CHK_CustomSystem.Text = "System Flag:";
            CHK_CustomSystem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            CHK_CustomSystem.UseVisualStyleBackColor = true;
            // 
            // B_ApplyFlag
            // 
            B_ApplyFlag.Location = new System.Drawing.Point(290, 17);
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
            B_ApplyWork.Location = new System.Drawing.Point(290, 81);
            B_ApplyWork.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ApplyWork.Name = "B_ApplyWork";
            B_ApplyWork.Size = new System.Drawing.Size(79, 27);
            B_ApplyWork.TabIndex = 39;
            B_ApplyWork.Text = "Apply";
            B_ApplyWork.UseVisualStyleBackColor = true;
            B_ApplyWork.Click += B_ApplyWork_Click;
            // 
            // NUD_Work
            // 
            NUD_Work.Location = new System.Drawing.Point(186, 81);
            NUD_Work.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_Work.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            NUD_Work.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
            NUD_Work.Name = "NUD_Work";
            NUD_Work.Size = new System.Drawing.Size(98, 23);
            NUD_Work.TabIndex = 38;
            // 
            // NUD_Flag
            // 
            NUD_Flag.Location = new System.Drawing.Point(186, 20);
            NUD_Flag.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_Flag.Maximum = new decimal(new int[] { 3999, 0, 0, 0 });
            NUD_Flag.Name = "NUD_Flag";
            NUD_Flag.Size = new System.Drawing.Size(52, 23);
            NUD_Flag.TabIndex = 9;
            NUD_Flag.ValueChanged += ChangeCustomFlag;
            // 
            // L_CustomWork
            // 
            L_CustomWork.Location = new System.Drawing.Point(10, 81);
            L_CustomWork.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_CustomWork.Name = "L_CustomWork";
            L_CustomWork.Size = new System.Drawing.Size(84, 23);
            L_CustomWork.TabIndex = 37;
            L_CustomWork.Text = "Constant:";
            L_CustomWork.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(527, 381);
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
            GB_Researcher.Location = new System.Drawing.Point(4, 127);
            GB_Researcher.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Researcher.Name = "GB_Researcher";
            GB_Researcher.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Researcher.Size = new System.Drawing.Size(485, 196);
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
            RTB_Diff.Size = new System.Drawing.Size(481, 111);
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
            TC_Features.Controls.Add(GB_System);
            TC_Features.Controls.Add(GB_Work);
            TC_Features.Controls.Add(GB_Research);
            TC_Features.Location = new System.Drawing.Point(14, 14);
            TC_Features.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TC_Features.Name = "TC_Features";
            TC_Features.SelectedIndex = 0;
            TC_Features.Size = new System.Drawing.Size(595, 357);
            TC_Features.TabIndex = 42;
            // 
            // GB_Flags
            // 
            GB_Flags.Controls.Add(TLP_Flags);
            GB_Flags.Location = new System.Drawing.Point(4, 24);
            GB_Flags.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Flags.Name = "GB_Flags";
            GB_Flags.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Flags.Size = new System.Drawing.Size(587, 329);
            GB_Flags.TabIndex = 0;
            GB_Flags.Text = "Event Flags";
            GB_Flags.UseVisualStyleBackColor = true;
            // 
            // TLP_Flags
            // 
            TLP_Flags.AutoScroll = true;
            TLP_Flags.ColumnCount = 2;
            TLP_Flags.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Flags.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Flags.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Flags.Location = new System.Drawing.Point(4, 3);
            TLP_Flags.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TLP_Flags.Name = "TLP_Flags";
            TLP_Flags.RowCount = 2;
            TLP_Flags.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Flags.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Flags.Size = new System.Drawing.Size(579, 323);
            TLP_Flags.TabIndex = 1;
            // 
            // GB_System
            // 
            GB_System.Controls.Add(TLP_System);
            GB_System.Location = new System.Drawing.Point(4, 24);
            GB_System.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_System.Name = "GB_System";
            GB_System.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_System.Size = new System.Drawing.Size(587, 329);
            GB_System.TabIndex = 3;
            GB_System.Text = "System Flags";
            GB_System.UseVisualStyleBackColor = true;
            // 
            // TLP_System
            // 
            TLP_System.AutoScroll = true;
            TLP_System.ColumnCount = 2;
            TLP_System.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_System.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_System.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_System.Location = new System.Drawing.Point(4, 3);
            TLP_System.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TLP_System.Name = "TLP_System";
            TLP_System.RowCount = 2;
            TLP_System.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_System.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_System.Size = new System.Drawing.Size(579, 323);
            TLP_System.TabIndex = 1;
            // 
            // GB_Work
            // 
            GB_Work.Controls.Add(TLP_Work);
            GB_Work.Location = new System.Drawing.Point(4, 24);
            GB_Work.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Work.Name = "GB_Work";
            GB_Work.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Work.Size = new System.Drawing.Size(587, 329);
            GB_Work.TabIndex = 1;
            GB_Work.Text = "Work Values";
            GB_Work.UseVisualStyleBackColor = true;
            // 
            // TLP_Work
            // 
            TLP_Work.AutoScroll = true;
            TLP_Work.ColumnCount = 3;
            TLP_Work.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Work.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Work.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 579F));
            TLP_Work.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Work.Location = new System.Drawing.Point(4, 3);
            TLP_Work.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TLP_Work.Name = "TLP_Work";
            TLP_Work.RowCount = 1;
            TLP_Work.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Work.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Work.Size = new System.Drawing.Size(579, 323);
            TLP_Work.TabIndex = 2;
            // 
            // GB_Research
            // 
            GB_Research.Controls.Add(GB_FlagStatus);
            GB_Research.Controls.Add(GB_Researcher);
            GB_Research.Location = new System.Drawing.Point(4, 24);
            GB_Research.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Research.Name = "GB_Research";
            GB_Research.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Research.Size = new System.Drawing.Size(587, 329);
            GB_Research.TabIndex = 2;
            GB_Research.Text = "Research";
            GB_Research.UseVisualStyleBackColor = true;
            // 
            // SAV_FlagWork8b
            // 
            AllowDrop = true;
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(623, 417);
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
            Name = "SAV_FlagWork8b";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Event Flag Editor";
            GB_FlagStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)NUD_WorkIndex).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_System).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Work).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Flag).EndInit();
            GB_Researcher.ResumeLayout(false);
            GB_Researcher.PerformLayout();
            TC_Features.ResumeLayout(false);
            GB_Flags.ResumeLayout(false);
            GB_System.ResumeLayout(false);
            GB_Work.ResumeLayout(false);
            GB_Research.ResumeLayout(false);
            ResumeLayout(false);
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
        private System.Windows.Forms.NumericUpDown NUD_WorkIndex;
    }
}
