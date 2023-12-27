namespace PKHeX.WinForms
{
    sealed partial class SAV_EventFlags
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
            NUD_Flag = new System.Windows.Forms.NumericUpDown();
            MT_Stat = new System.Windows.Forms.MaskedTextBox();
            CHK_CustomFlag = new System.Windows.Forms.Label();
            CB_Stats = new System.Windows.Forms.ComboBox();
            L_Stats = new System.Windows.Forms.Label();
            B_Save = new System.Windows.Forms.Button();
            GB_Researcher = new System.Windows.Forms.GroupBox();
            L_UnSet = new System.Windows.Forms.Label();
            L_IsSet = new System.Windows.Forms.Label();
            TB_NewSAV = new System.Windows.Forms.TextBox();
            TB_OldSAV = new System.Windows.Forms.TextBox();
            TB_UnSet = new System.Windows.Forms.TextBox();
            TB_IsSet = new System.Windows.Forms.TextBox();
            B_LoadNew = new System.Windows.Forms.Button();
            B_LoadOld = new System.Windows.Forms.Button();
            L_EventFlagWarn = new System.Windows.Forms.Label();
            tabControl1 = new System.Windows.Forms.TabControl();
            GB_Flags = new System.Windows.Forms.TabPage();
            dgv = new System.Windows.Forms.DataGridView();
            GB_Constants = new System.Windows.Forms.TabPage();
            TLP_Const = new System.Windows.Forms.TableLayoutPanel();
            GB_Research = new System.Windows.Forms.TabPage();
            GB_FlagStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Flag).BeginInit();
            GB_Researcher.SuspendLayout();
            tabControl1.SuspendLayout();
            GB_Flags.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgv).BeginInit();
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
            c_CustomFlag.CheckedChanged += ChangeCustomBool;
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
            GB_FlagStatus.Controls.Add(NUD_Flag);
            GB_FlagStatus.Controls.Add(MT_Stat);
            GB_FlagStatus.Controls.Add(CHK_CustomFlag);
            GB_FlagStatus.Controls.Add(CB_Stats);
            GB_FlagStatus.Controls.Add(L_Stats);
            GB_FlagStatus.Controls.Add(c_CustomFlag);
            GB_FlagStatus.Location = new System.Drawing.Point(7, 6);
            GB_FlagStatus.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_FlagStatus.Name = "GB_FlagStatus";
            GB_FlagStatus.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_FlagStatus.Size = new System.Drawing.Size(240, 87);
            GB_FlagStatus.TabIndex = 3;
            GB_FlagStatus.TabStop = false;
            GB_FlagStatus.Text = "Check Status";
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
            NUD_Flag.KeyUp += ChangeCustomFlag;
            // 
            // MT_Stat
            // 
            MT_Stat.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            MT_Stat.Location = new System.Drawing.Point(186, 51);
            MT_Stat.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MT_Stat.Mask = "00000";
            MT_Stat.Name = "MT_Stat";
            MT_Stat.Size = new System.Drawing.Size(39, 20);
            MT_Stat.TabIndex = 34;
            MT_Stat.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            MT_Stat.TextChanged += ChangeCustomConst;
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
            GB_Researcher.Controls.Add(L_UnSet);
            GB_Researcher.Controls.Add(L_IsSet);
            GB_Researcher.Controls.Add(TB_NewSAV);
            GB_Researcher.Controls.Add(TB_OldSAV);
            GB_Researcher.Controls.Add(TB_UnSet);
            GB_Researcher.Controls.Add(TB_IsSet);
            GB_Researcher.Controls.Add(B_LoadNew);
            GB_Researcher.Controls.Add(B_LoadOld);
            GB_Researcher.Dock = System.Windows.Forms.DockStyle.Bottom;
            GB_Researcher.Location = new System.Drawing.Point(4, 188);
            GB_Researcher.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Researcher.Name = "GB_Researcher";
            GB_Researcher.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Researcher.Size = new System.Drawing.Size(486, 138);
            GB_Researcher.TabIndex = 13;
            GB_Researcher.TabStop = false;
            GB_Researcher.Text = "FlagDiff Researcher";
            // 
            // L_UnSet
            // 
            L_UnSet.Location = new System.Drawing.Point(4, 108);
            L_UnSet.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_UnSet.Name = "L_UnSet";
            L_UnSet.Size = new System.Drawing.Size(59, 24);
            L_UnSet.TabIndex = 7;
            L_UnSet.Text = "UnSet:";
            L_UnSet.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_IsSet
            // 
            L_IsSet.Location = new System.Drawing.Point(7, 84);
            L_IsSet.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_IsSet.Name = "L_IsSet";
            L_IsSet.Size = new System.Drawing.Size(56, 23);
            L_IsSet.TabIndex = 6;
            L_IsSet.Text = "IsSet:";
            L_IsSet.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TB_NewSAV
            // 
            TB_NewSAV.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TB_NewSAV.Location = new System.Drawing.Point(108, 54);
            TB_NewSAV.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_NewSAV.Name = "TB_NewSAV";
            TB_NewSAV.ReadOnly = true;
            TB_NewSAV.Size = new System.Drawing.Size(370, 23);
            TB_NewSAV.TabIndex = 5;
            TB_NewSAV.TextChanged += ChangeSAV;
            // 
            // TB_OldSAV
            // 
            TB_OldSAV.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TB_OldSAV.Location = new System.Drawing.Point(108, 24);
            TB_OldSAV.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_OldSAV.Name = "TB_OldSAV";
            TB_OldSAV.ReadOnly = true;
            TB_OldSAV.Size = new System.Drawing.Size(370, 23);
            TB_OldSAV.TabIndex = 4;
            TB_OldSAV.TextChanged += ChangeSAV;
            // 
            // TB_UnSet
            // 
            TB_UnSet.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TB_UnSet.Location = new System.Drawing.Point(65, 108);
            TB_UnSet.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_UnSet.Name = "TB_UnSet";
            TB_UnSet.ReadOnly = true;
            TB_UnSet.Size = new System.Drawing.Size(413, 23);
            TB_UnSet.TabIndex = 3;
            // 
            // TB_IsSet
            // 
            TB_IsSet.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TB_IsSet.Location = new System.Drawing.Point(65, 84);
            TB_IsSet.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_IsSet.Name = "TB_IsSet";
            TB_IsSet.ReadOnly = true;
            TB_IsSet.Size = new System.Drawing.Size(413, 23);
            TB_IsSet.TabIndex = 2;
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
            L_EventFlagWarn.Text = "Altering Event Flags may impact other story events.\r\nSave file backups are recommended.";
            L_EventFlagWarn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabControl1
            // 
            tabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tabControl1.Controls.Add(GB_Flags);
            tabControl1.Controls.Add(GB_Constants);
            tabControl1.Controls.Add(GB_Research);
            tabControl1.Location = new System.Drawing.Point(14, 14);
            tabControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(502, 357);
            tabControl1.TabIndex = 42;
            // 
            // GB_Flags
            // 
            GB_Flags.Controls.Add(dgv);
            GB_Flags.Location = new System.Drawing.Point(4, 24);
            GB_Flags.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Flags.Name = "GB_Flags";
            GB_Flags.Size = new System.Drawing.Size(494, 329);
            GB_Flags.TabIndex = 0;
            GB_Flags.Text = "Event Flags";
            GB_Flags.UseVisualStyleBackColor = true;
            // 
            // dgv
            // 
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeColumns = false;
            dgv.AllowUserToResizeRows = false;
            dgv.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            dgv.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dgv.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.ColumnHeadersVisible = false;
            dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            dgv.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            dgv.Location = new System.Drawing.Point(0, 0);
            dgv.Margin = new System.Windows.Forms.Padding(0);
            dgv.MultiSelect = false;
            dgv.Name = "dgv";
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            dgv.ShowEditingIcon = false;
            dgv.Size = new System.Drawing.Size(494, 329);
            dgv.TabIndex = 12;
            // 
            // GB_Constants
            // 
            GB_Constants.Controls.Add(TLP_Const);
            GB_Constants.Location = new System.Drawing.Point(4, 24);
            GB_Constants.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Constants.Name = "GB_Constants";
            GB_Constants.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Constants.Size = new System.Drawing.Size(494, 329);
            GB_Constants.TabIndex = 1;
            GB_Constants.Text = "Event Constants";
            GB_Constants.UseVisualStyleBackColor = true;
            // 
            // TLP_Const
            // 
            TLP_Const.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TLP_Const.AutoScroll = true;
            TLP_Const.ColumnCount = 3;
            TLP_Const.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Const.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Const.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 485F));
            TLP_Const.Location = new System.Drawing.Point(4, 3);
            TLP_Const.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TLP_Const.Name = "TLP_Const";
            TLP_Const.RowCount = 1;
            TLP_Const.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Const.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Const.Size = new System.Drawing.Size(485, 320);
            TLP_Const.TabIndex = 1;
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
            // SAV_EventFlags
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(530, 417);
            Controls.Add(tabControl1);
            Controls.Add(L_EventFlagWarn);
            Controls.Add(B_Save);
            Controls.Add(B_Cancel);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(779, 917);
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(546, 456);
            Name = "SAV_EventFlags";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Event Flag Editor";
            GB_FlagStatus.ResumeLayout(false);
            GB_FlagStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Flag).EndInit();
            GB_Researcher.ResumeLayout(false);
            GB_Researcher.PerformLayout();
            tabControl1.ResumeLayout(false);
            GB_Flags.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgv).EndInit();
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
        private System.Windows.Forms.Label L_UnSet;
        private System.Windows.Forms.Label L_IsSet;
        private System.Windows.Forms.TextBox TB_NewSAV;
        private System.Windows.Forms.TextBox TB_OldSAV;
        private System.Windows.Forms.TextBox TB_UnSet;
        private System.Windows.Forms.TextBox TB_IsSet;
        private System.Windows.Forms.Button B_LoadNew;
        private System.Windows.Forms.Button B_LoadOld;
        private System.Windows.Forms.Label L_Stats;
        private System.Windows.Forms.ComboBox CB_Stats;
        private System.Windows.Forms.MaskedTextBox MT_Stat;
        private System.Windows.Forms.Label L_EventFlagWarn;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage GB_Flags;
        private System.Windows.Forms.TabPage GB_Constants;
        private System.Windows.Forms.TabPage GB_Research;
        private System.Windows.Forms.TableLayoutPanel TLP_Const;
        private System.Windows.Forms.DataGridView dgv;
    }
}
