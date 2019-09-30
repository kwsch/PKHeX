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
            this.c_CustomFlag = new System.Windows.Forms.CheckBox();
            this.B_Cancel = new System.Windows.Forms.Button();
            this.GB_FlagStatus = new System.Windows.Forms.GroupBox();
            this.NUD_Flag = new System.Windows.Forms.NumericUpDown();
            this.MT_Stat = new System.Windows.Forms.MaskedTextBox();
            this.CHK_CustomFlag = new System.Windows.Forms.Label();
            this.CB_Stats = new System.Windows.Forms.ComboBox();
            this.L_Stats = new System.Windows.Forms.Label();
            this.B_Save = new System.Windows.Forms.Button();
            this.GB_Researcher = new System.Windows.Forms.GroupBox();
            this.L_UnSet = new System.Windows.Forms.Label();
            this.L_IsSet = new System.Windows.Forms.Label();
            this.TB_NewSAV = new System.Windows.Forms.TextBox();
            this.TB_OldSAV = new System.Windows.Forms.TextBox();
            this.TB_UnSet = new System.Windows.Forms.TextBox();
            this.TB_IsSet = new System.Windows.Forms.TextBox();
            this.B_LoadNew = new System.Windows.Forms.Button();
            this.B_LoadOld = new System.Windows.Forms.Button();
            this.TLP_Flags = new System.Windows.Forms.TableLayoutPanel();
            this.L_EventFlagWarn = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.GB_Flags = new System.Windows.Forms.TabPage();
            this.GB_Constants = new System.Windows.Forms.TabPage();
            this.TLP_Const = new System.Windows.Forms.TableLayoutPanel();
            this.GB_Research = new System.Windows.Forms.TabPage();
            this.GB_FlagStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Flag)).BeginInit();
            this.GB_Researcher.SuspendLayout();
            this.tabControl1.SuspendLayout();
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
            this.c_CustomFlag.CheckedChanged += new System.EventHandler(this.ChangeCustomBool);
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
            this.GB_FlagStatus.Controls.Add(this.NUD_Flag);
            this.GB_FlagStatus.Controls.Add(this.MT_Stat);
            this.GB_FlagStatus.Controls.Add(this.CHK_CustomFlag);
            this.GB_FlagStatus.Controls.Add(this.CB_Stats);
            this.GB_FlagStatus.Controls.Add(this.L_Stats);
            this.GB_FlagStatus.Controls.Add(this.c_CustomFlag);
            this.GB_FlagStatus.Location = new System.Drawing.Point(6, 5);
            this.GB_FlagStatus.Name = "GB_FlagStatus";
            this.GB_FlagStatus.Size = new System.Drawing.Size(206, 75);
            this.GB_FlagStatus.TabIndex = 3;
            this.GB_FlagStatus.TabStop = false;
            this.GB_FlagStatus.Text = "Check Status";
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
            this.NUD_Flag.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ChangeCustomFlag);
            // 
            // MT_Stat
            // 
            this.MT_Stat.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MT_Stat.Location = new System.Drawing.Point(159, 44);
            this.MT_Stat.Mask = "00000";
            this.MT_Stat.Name = "MT_Stat";
            this.MT_Stat.Size = new System.Drawing.Size(34, 20);
            this.MT_Stat.TabIndex = 34;
            this.MT_Stat.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.MT_Stat.TextChanged += new System.EventHandler(this.ChangeCustomConst);
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
            this.GB_Researcher.Controls.Add(this.L_UnSet);
            this.GB_Researcher.Controls.Add(this.L_IsSet);
            this.GB_Researcher.Controls.Add(this.TB_NewSAV);
            this.GB_Researcher.Controls.Add(this.TB_OldSAV);
            this.GB_Researcher.Controls.Add(this.TB_UnSet);
            this.GB_Researcher.Controls.Add(this.TB_IsSet);
            this.GB_Researcher.Controls.Add(this.B_LoadNew);
            this.GB_Researcher.Controls.Add(this.B_LoadOld);
            this.GB_Researcher.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.GB_Researcher.Location = new System.Drawing.Point(3, 160);
            this.GB_Researcher.Name = "GB_Researcher";
            this.GB_Researcher.Size = new System.Drawing.Size(416, 120);
            this.GB_Researcher.TabIndex = 13;
            this.GB_Researcher.TabStop = false;
            this.GB_Researcher.Text = "FlagDiff Researcher";
            // 
            // L_UnSet
            // 
            this.L_UnSet.Location = new System.Drawing.Point(3, 94);
            this.L_UnSet.Name = "L_UnSet";
            this.L_UnSet.Size = new System.Drawing.Size(51, 21);
            this.L_UnSet.TabIndex = 7;
            this.L_UnSet.Text = "UnSet:";
            this.L_UnSet.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_IsSet
            // 
            this.L_IsSet.Location = new System.Drawing.Point(6, 73);
            this.L_IsSet.Name = "L_IsSet";
            this.L_IsSet.Size = new System.Drawing.Size(48, 20);
            this.L_IsSet.TabIndex = 6;
            this.L_IsSet.Text = "IsSet:";
            this.L_IsSet.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            this.TB_NewSAV.TextChanged += new System.EventHandler(this.ChangeSAV);
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
            this.TB_OldSAV.TextChanged += new System.EventHandler(this.ChangeSAV);
            // 
            // TB_UnSet
            // 
            this.TB_UnSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_UnSet.Location = new System.Drawing.Point(56, 94);
            this.TB_UnSet.Name = "TB_UnSet";
            this.TB_UnSet.ReadOnly = true;
            this.TB_UnSet.Size = new System.Drawing.Size(354, 20);
            this.TB_UnSet.TabIndex = 3;
            // 
            // TB_IsSet
            // 
            this.TB_IsSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_IsSet.Location = new System.Drawing.Point(56, 73);
            this.TB_IsSet.Name = "TB_IsSet";
            this.TB_IsSet.ReadOnly = true;
            this.TB_IsSet.Size = new System.Drawing.Size(354, 20);
            this.TB_IsSet.TabIndex = 2;
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
            // TLP_Flags
            // 
            this.TLP_Flags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TLP_Flags.AutoScroll = true;
            this.TLP_Flags.ColumnCount = 2;
            this.TLP_Flags.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TLP_Flags.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TLP_Flags.Location = new System.Drawing.Point(3, 3);
            this.TLP_Flags.Name = "TLP_Flags";
            this.TLP_Flags.RowCount = 2;
            this.TLP_Flags.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Flags.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Flags.Size = new System.Drawing.Size(416, 277);
            this.TLP_Flags.TabIndex = 0;
            // 
            // L_EventFlagWarn
            // 
            this.L_EventFlagWarn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.L_EventFlagWarn.ForeColor = System.Drawing.Color.Red;
            this.L_EventFlagWarn.Location = new System.Drawing.Point(9, 324);
            this.L_EventFlagWarn.Name = "L_EventFlagWarn";
            this.L_EventFlagWarn.Size = new System.Drawing.Size(262, 31);
            this.L_EventFlagWarn.TabIndex = 41;
            this.L_EventFlagWarn.Text = "Altering Event Flags may impact other story events.\r\nSave file backups are recomm" +
    "ended.";
            this.L_EventFlagWarn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.GB_Flags);
            this.tabControl1.Controls.Add(this.GB_Constants);
            this.tabControl1.Controls.Add(this.GB_Research);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(430, 309);
            this.tabControl1.TabIndex = 42;
            // 
            // GB_Flags
            // 
            this.GB_Flags.Controls.Add(this.TLP_Flags);
            this.GB_Flags.Location = new System.Drawing.Point(4, 22);
            this.GB_Flags.Name = "GB_Flags";
            this.GB_Flags.Padding = new System.Windows.Forms.Padding(3);
            this.GB_Flags.Size = new System.Drawing.Size(422, 283);
            this.GB_Flags.TabIndex = 0;
            this.GB_Flags.Text = "Event Flags";
            this.GB_Flags.UseVisualStyleBackColor = true;
            // 
            // GB_Constants
            // 
            this.GB_Constants.Controls.Add(this.TLP_Const);
            this.GB_Constants.Location = new System.Drawing.Point(4, 22);
            this.GB_Constants.Name = "GB_Constants";
            this.GB_Constants.Padding = new System.Windows.Forms.Padding(3);
            this.GB_Constants.Size = new System.Drawing.Size(422, 283);
            this.GB_Constants.TabIndex = 1;
            this.GB_Constants.Text = "Event Constants";
            this.GB_Constants.UseVisualStyleBackColor = true;
            // 
            // TLP_Const
            // 
            this.TLP_Const.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TLP_Const.AutoScroll = true;
            this.TLP_Const.ColumnCount = 3;
            this.TLP_Const.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TLP_Const.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TLP_Const.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 416F));
            this.TLP_Const.Location = new System.Drawing.Point(3, 3);
            this.TLP_Const.Name = "TLP_Const";
            this.TLP_Const.RowCount = 1;
            this.TLP_Const.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Const.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Const.Size = new System.Drawing.Size(416, 277);
            this.TLP_Const.TabIndex = 1;
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
            // SAV_EventFlags
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 361);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.L_EventFlagWarn);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_Cancel);
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(670, 800);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(470, 400);
            this.Name = "SAV_EventFlags";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Event Flag Editor";
            this.GB_FlagStatus.ResumeLayout(false);
            this.GB_FlagStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Flag)).EndInit();
            this.GB_Researcher.ResumeLayout(false);
            this.GB_Researcher.PerformLayout();
            this.tabControl1.ResumeLayout(false);
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
        private System.Windows.Forms.TableLayoutPanel TLP_Flags;
        private System.Windows.Forms.Label L_EventFlagWarn;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage GB_Flags;
        private System.Windows.Forms.TabPage GB_Constants;
        private System.Windows.Forms.TabPage GB_Research;
        private System.Windows.Forms.TableLayoutPanel TLP_Const;
    }
}