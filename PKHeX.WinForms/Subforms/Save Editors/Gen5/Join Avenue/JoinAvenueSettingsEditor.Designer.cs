namespace PKHeX.WinForms
{
    partial class JoinAvenueSettingsEditor
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            TLP_Main = new System.Windows.Forms.TableLayoutPanel();
            L_Name = new System.Windows.Forms.Label();
            TB_Name = new System.Windows.Forms.TextBox();
            L_Title = new System.Windows.Forms.Label();
            TB_Title = new System.Windows.Forms.TextBox();
            L_Experience = new System.Windows.Forms.Label();
            NUD_Experience = new System.Windows.Forms.NumericUpDown();
            L_Rank = new System.Windows.Forms.Label();
            NUD_Rank = new System.Windows.Forms.NumericUpDown();
            L_CeilingColor = new System.Windows.Forms.Label();
            CB_CeilingColor = new System.Windows.Forms.ComboBox();
            L_Flags = new System.Windows.Forms.Label();
            NUD_Flags = new System.Windows.Forms.NumericUpDown();
            L_Seed = new System.Windows.Forms.Label();
            NUD_Seed = new System.Windows.Forms.NumericUpDown();
            L_IsPromotionActive = new System.Windows.Forms.Label();
            CHK_IsPromotionActive = new System.Windows.Forms.CheckBox();
            L_PromotionDaysElapsed = new System.Windows.Forms.Label();
            NUD_PromotionDaysElapsed = new System.Windows.Forms.NumericUpDown();
            L_PlayerIDCount = new System.Windows.Forms.Label();
            NUD_PlayerCount = new System.Windows.Forms.NumericUpDown();
            L_PlayerIDInsert = new System.Windows.Forms.Label();
            NUD_PlayerInsert = new System.Windows.Forms.NumericUpDown();
            L_VisitingPlayerDatabase = new System.Windows.Forms.Label();
            DGV_VisitingPlayerDatabase = new System.Windows.Forms.DataGridView();
            Column_Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column_TID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column_SID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            TLP_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Experience).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Rank).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Flags).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Seed).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_PromotionDaysElapsed).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_PlayerCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_PlayerInsert).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DGV_VisitingPlayerDatabase).BeginInit();
            SuspendLayout();
            // 
            // TLP_Main
            // 
            TLP_Main.AutoScroll = true;
            TLP_Main.ColumnCount = 2;
            TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Main.Controls.Add(L_Name, 0, 0);
            TLP_Main.Controls.Add(TB_Name, 1, 0);
            TLP_Main.Controls.Add(L_Title, 0, 1);
            TLP_Main.Controls.Add(TB_Title, 1, 1);
            TLP_Main.Controls.Add(L_Experience, 0, 2);
            TLP_Main.Controls.Add(NUD_Experience, 1, 2);
            TLP_Main.Controls.Add(L_Rank, 0, 3);
            TLP_Main.Controls.Add(NUD_Rank, 1, 3);
            TLP_Main.Controls.Add(L_CeilingColor, 0, 4);
            TLP_Main.Controls.Add(CB_CeilingColor, 1, 4);
            TLP_Main.Controls.Add(L_Flags, 0, 5);
            TLP_Main.Controls.Add(NUD_Flags, 1, 5);
            TLP_Main.Controls.Add(L_Seed, 0, 6);
            TLP_Main.Controls.Add(NUD_Seed, 1, 6);
            TLP_Main.Controls.Add(L_IsPromotionActive, 0, 7);
            TLP_Main.Controls.Add(CHK_IsPromotionActive, 1, 7);
            TLP_Main.Controls.Add(L_PromotionDaysElapsed, 0, 8);
            TLP_Main.Controls.Add(NUD_PromotionDaysElapsed, 1, 8);
            TLP_Main.Controls.Add(L_PlayerIDCount, 0, 9);
            TLP_Main.Controls.Add(NUD_PlayerCount, 1, 9);
            TLP_Main.Controls.Add(L_PlayerIDInsert, 0, 10);
            TLP_Main.Controls.Add(NUD_PlayerInsert, 1, 10);
            TLP_Main.Controls.Add(L_VisitingPlayerDatabase, 0, 11);
            TLP_Main.Controls.Add(DGV_VisitingPlayerDatabase, 1, 11);
            TLP_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Main.Location = new System.Drawing.Point(0, 0);
            TLP_Main.Margin = new System.Windows.Forms.Padding(0);
            TLP_Main.Name = "TLP_Main";
            TLP_Main.RowCount = 12;
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.Size = new System.Drawing.Size(347, 548);
            TLP_Main.TabIndex = 0;
            // 
            // L_Name
            // 
            L_Name.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Name.AutoSize = true;
            L_Name.Location = new System.Drawing.Point(108, 4);
            L_Name.Margin = new System.Windows.Forms.Padding(0);
            L_Name.Name = "L_Name";
            L_Name.Size = new System.Drawing.Size(46, 17);
            L_Name.TabIndex = 0;
            L_Name.Text = "Name:";
            // 
            // TB_Name
            // 
            TB_Name.Location = new System.Drawing.Point(154, 0);
            TB_Name.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            TB_Name.MaxLength = 20;
            TB_Name.Name = "TB_Name";
            TB_Name.Size = new System.Drawing.Size(180, 25);
            TB_Name.TabIndex = 1;
            // 
            // L_Title
            // 
            L_Title.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Title.AutoSize = true;
            L_Title.Location = new System.Drawing.Point(119, 30);
            L_Title.Margin = new System.Windows.Forms.Padding(0);
            L_Title.Name = "L_Title";
            L_Title.Size = new System.Drawing.Size(35, 17);
            L_Title.TabIndex = 2;
            L_Title.Text = "Title:";
            // 
            // TB_Title
            // 
            TB_Title.Location = new System.Drawing.Point(154, 26);
            TB_Title.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            TB_Title.MaxLength = 20;
            TB_Title.Name = "TB_Title";
            TB_Title.Size = new System.Drawing.Size(180, 25);
            TB_Title.TabIndex = 3;
            // 
            // L_Experience
            // 
            L_Experience.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Experience.AutoSize = true;
            L_Experience.Location = new System.Drawing.Point(80, 56);
            L_Experience.Margin = new System.Windows.Forms.Padding(0);
            L_Experience.Name = "L_Experience";
            L_Experience.Size = new System.Drawing.Size(74, 17);
            L_Experience.TabIndex = 4;
            L_Experience.Text = "Experience:";
            // 
            // NUD_Experience
            // 
            NUD_Experience.Location = new System.Drawing.Point(154, 52);
            NUD_Experience.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            NUD_Experience.Maximum = new decimal(new int[] { -1, 0, 0, 0 });
            NUD_Experience.Name = "NUD_Experience";
            NUD_Experience.Size = new System.Drawing.Size(120, 25);
            NUD_Experience.TabIndex = 5;
            // 
            // L_Rank
            // 
            L_Rank.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Rank.AutoSize = true;
            L_Rank.Location = new System.Drawing.Point(115, 82);
            L_Rank.Margin = new System.Windows.Forms.Padding(0);
            L_Rank.Name = "L_Rank";
            L_Rank.Size = new System.Drawing.Size(39, 17);
            L_Rank.TabIndex = 6;
            L_Rank.Text = "Rank:";
            // 
            // NUD_Rank
            // 
            NUD_Rank.Location = new System.Drawing.Point(154, 78);
            NUD_Rank.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            NUD_Rank.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            NUD_Rank.Name = "NUD_Rank";
            NUD_Rank.Size = new System.Drawing.Size(64, 25);
            NUD_Rank.TabIndex = 7;
            // 
            // L_CeilingColor
            // 
            L_CeilingColor.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_CeilingColor.AutoSize = true;
            L_CeilingColor.Location = new System.Drawing.Point(68, 108);
            L_CeilingColor.Margin = new System.Windows.Forms.Padding(0);
            L_CeilingColor.Name = "L_CeilingColor";
            L_CeilingColor.Size = new System.Drawing.Size(86, 17);
            L_CeilingColor.TabIndex = 8;
            L_CeilingColor.Text = "Ceiling Color:";
            // 
            // CB_CeilingColor
            // 
            CB_CeilingColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_CeilingColor.FormattingEnabled = true;
            CB_CeilingColor.Location = new System.Drawing.Point(154, 104);
            CB_CeilingColor.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            CB_CeilingColor.Name = "CB_CeilingColor";
            CB_CeilingColor.Size = new System.Drawing.Size(180, 25);
            CB_CeilingColor.TabIndex = 9;
            // 
            // L_Flags
            // 
            L_Flags.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Flags.AutoSize = true;
            L_Flags.Location = new System.Drawing.Point(113, 134);
            L_Flags.Margin = new System.Windows.Forms.Padding(0);
            L_Flags.Name = "L_Flags";
            L_Flags.Size = new System.Drawing.Size(41, 17);
            L_Flags.TabIndex = 10;
            L_Flags.Text = "Flags:";
            // 
            // NUD_Flags
            // 
            NUD_Flags.Location = new System.Drawing.Point(154, 130);
            NUD_Flags.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            NUD_Flags.Maximum = new decimal(new int[] { -1, 0, 0, 0 });
            NUD_Flags.Name = "NUD_Flags";
            NUD_Flags.Size = new System.Drawing.Size(120, 25);
            NUD_Flags.TabIndex = 11;
            // 
            // L_Seed
            // 
            L_Seed.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Seed.AutoSize = true;
            L_Seed.Location = new System.Drawing.Point(114, 160);
            L_Seed.Margin = new System.Windows.Forms.Padding(0);
            L_Seed.Name = "L_Seed";
            L_Seed.Size = new System.Drawing.Size(40, 17);
            L_Seed.TabIndex = 12;
            L_Seed.Text = "Seed:";
            // 
            // NUD_Seed
            // 
            NUD_Seed.Location = new System.Drawing.Point(154, 156);
            NUD_Seed.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            NUD_Seed.Maximum = new decimal(new int[] { -1, 0, 0, 0 });
            NUD_Seed.Name = "NUD_Seed";
            NUD_Seed.Size = new System.Drawing.Size(120, 25);
            NUD_Seed.TabIndex = 13;
            // 
            // L_IsPromotionActive
            // 
            L_IsPromotionActive.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_IsPromotionActive.AutoSize = true;
            L_IsPromotionActive.Location = new System.Drawing.Point(31, 186);
            L_IsPromotionActive.Margin = new System.Windows.Forms.Padding(0);
            L_IsPromotionActive.Name = "L_IsPromotionActive";
            L_IsPromotionActive.Size = new System.Drawing.Size(123, 17);
            L_IsPromotionActive.TabIndex = 14;
            L_IsPromotionActive.Text = "Is Promotion Active:";
            // 
            // CHK_IsPromotionActive
            // 
            CHK_IsPromotionActive.Location = new System.Drawing.Point(154, 182);
            CHK_IsPromotionActive.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            CHK_IsPromotionActive.Name = "CHK_IsPromotionActive";
            CHK_IsPromotionActive.Size = new System.Drawing.Size(120, 25);
            CHK_IsPromotionActive.TabIndex = 15;
            // 
            // L_PromotionDaysElapsed
            // 
            L_PromotionDaysElapsed.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_PromotionDaysElapsed.AutoSize = true;
            L_PromotionDaysElapsed.Location = new System.Drawing.Point(0, 212);
            L_PromotionDaysElapsed.Margin = new System.Windows.Forms.Padding(0);
            L_PromotionDaysElapsed.Name = "L_PromotionDaysElapsed";
            L_PromotionDaysElapsed.Size = new System.Drawing.Size(154, 17);
            L_PromotionDaysElapsed.TabIndex = 16;
            L_PromotionDaysElapsed.Text = "Promotion Days Elapsed:";
            // 
            // NUD_PromotionDaysElapsed
            // 
            NUD_PromotionDaysElapsed.Location = new System.Drawing.Point(154, 208);
            NUD_PromotionDaysElapsed.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            NUD_PromotionDaysElapsed.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NUD_PromotionDaysElapsed.Name = "NUD_PromotionDaysElapsed";
            NUD_PromotionDaysElapsed.Size = new System.Drawing.Size(64, 25);
            NUD_PromotionDaysElapsed.TabIndex = 17;
            // 
            // L_PlayerIDCount
            // 
            L_PlayerIDCount.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_PlayerIDCount.AutoSize = true;
            L_PlayerIDCount.Location = new System.Drawing.Point(54, 238);
            L_PlayerIDCount.Margin = new System.Windows.Forms.Padding(0);
            L_PlayerIDCount.Name = "L_PlayerIDCount";
            L_PlayerIDCount.Size = new System.Drawing.Size(100, 17);
            L_PlayerIDCount.TabIndex = 18;
            L_PlayerIDCount.Text = "Player ID Count:";
            // 
            // NUD_PlayerCount
            // 
            NUD_PlayerCount.Location = new System.Drawing.Point(154, 234);
            NUD_PlayerCount.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            NUD_PlayerCount.Maximum = new decimal(new int[] { 32, 0, 0, 0 });
            NUD_PlayerCount.Name = "NUD_PlayerCount";
            NUD_PlayerCount.Size = new System.Drawing.Size(64, 25);
            NUD_PlayerCount.TabIndex = 19;
            // 
            // L_PlayerIDInsert
            // 
            L_PlayerIDInsert.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_PlayerIDInsert.AutoSize = true;
            L_PlayerIDInsert.Location = new System.Drawing.Point(56, 264);
            L_PlayerIDInsert.Margin = new System.Windows.Forms.Padding(0);
            L_PlayerIDInsert.Name = "L_PlayerIDInsert";
            L_PlayerIDInsert.Size = new System.Drawing.Size(98, 17);
            L_PlayerIDInsert.TabIndex = 20;
            L_PlayerIDInsert.Text = "Player ID Insert:";
            // 
            // NUD_PlayerInsert
            // 
            NUD_PlayerInsert.Location = new System.Drawing.Point(154, 260);
            NUD_PlayerInsert.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            NUD_PlayerInsert.Maximum = new decimal(new int[] { 31, 0, 0, 0 });
            NUD_PlayerInsert.Name = "NUD_PlayerInsert";
            NUD_PlayerInsert.Size = new System.Drawing.Size(64, 25);
            NUD_PlayerInsert.TabIndex = 21;
            // 
            // L_VisitingPlayerDatabase
            // 
            L_VisitingPlayerDatabase.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_VisitingPlayerDatabase.AutoSize = true;
            L_VisitingPlayerDatabase.Location = new System.Drawing.Point(83, 408);
            L_VisitingPlayerDatabase.Name = "L_VisitingPlayerDatabase";
            L_VisitingPlayerDatabase.Size = new System.Drawing.Size(68, 17);
            L_VisitingPlayerDatabase.TabIndex = 22;
            L_VisitingPlayerDatabase.Text = "Player IDs:";
            // 
            // DGV_VisitingPlayerDatabase
            // 
            DGV_VisitingPlayerDatabase.AllowUserToAddRows = false;
            DGV_VisitingPlayerDatabase.AllowUserToDeleteRows = false;
            DGV_VisitingPlayerDatabase.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ControlLight;
            DGV_VisitingPlayerDatabase.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            DGV_VisitingPlayerDatabase.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_VisitingPlayerDatabase.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Column_Index, Column_TID, Column_SID });
            DGV_VisitingPlayerDatabase.Dock = System.Windows.Forms.DockStyle.Fill;
            DGV_VisitingPlayerDatabase.Location = new System.Drawing.Point(154, 286);
            DGV_VisitingPlayerDatabase.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            DGV_VisitingPlayerDatabase.MultiSelect = false;
            DGV_VisitingPlayerDatabase.Name = "DGV_VisitingPlayerDatabase";
            DGV_VisitingPlayerDatabase.RowHeadersVisible = false;
            DGV_VisitingPlayerDatabase.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            DGV_VisitingPlayerDatabase.Size = new System.Drawing.Size(193, 261);
            DGV_VisitingPlayerDatabase.TabIndex = 23;
            // 
            // Column_Index
            // 
            Column_Index.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            Column_Index.HeaderText = "#";
            Column_Index.Name = "Column_Index";
            Column_Index.ReadOnly = true;
            Column_Index.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            Column_Index.Width = 22;
            // 
            // Column_TID
            // 
            Column_TID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            Column_TID.HeaderText = "TID";
            Column_TID.Name = "Column_TID";
            Column_TID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            Column_TID.Width = 33;
            // 
            // Column_SID
            // 
            Column_SID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            Column_SID.HeaderText = "SID";
            Column_SID.Name = "Column_SID";
            Column_SID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            Column_SID.Width = 33;
            // 
            // JoinAvenueSettingsEditor
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(TLP_Main);
            Margin = new System.Windows.Forms.Padding(0);
            Name = "JoinAvenueSettingsEditor";
            Size = new System.Drawing.Size(347, 548);
            TLP_Main.ResumeLayout(false);
            TLP_Main.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Experience).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Rank).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Flags).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Seed).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_PromotionDaysElapsed).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_PlayerCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_PlayerInsert).EndInit();
            ((System.ComponentModel.ISupportInitialize)DGV_VisitingPlayerDatabase).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TLP_Main;
        private System.Windows.Forms.Label L_Name;
        private System.Windows.Forms.TextBox TB_Name;
        private System.Windows.Forms.Label L_Title;
        private System.Windows.Forms.TextBox TB_Title;
        private System.Windows.Forms.Label L_Experience;
        private System.Windows.Forms.NumericUpDown NUD_Experience;
        private System.Windows.Forms.Label L_Rank;
        private System.Windows.Forms.NumericUpDown NUD_Rank;
        private System.Windows.Forms.Label L_CeilingColor;
        private System.Windows.Forms.ComboBox CB_CeilingColor;
        private System.Windows.Forms.Label L_Flags;
        private System.Windows.Forms.NumericUpDown NUD_Flags;
        private System.Windows.Forms.Label L_PlayerIDCount;
        private System.Windows.Forms.NumericUpDown NUD_PlayerCount;
        private System.Windows.Forms.Label L_PlayerIDInsert;
        private System.Windows.Forms.NumericUpDown NUD_PlayerInsert;
        private System.Windows.Forms.Label L_Seed;
        private System.Windows.Forms.NumericUpDown NUD_Seed;
        private System.Windows.Forms.Label L_IsPromotionActive;
        private System.Windows.Forms.CheckBox CHK_IsPromotionActive;
        private System.Windows.Forms.Label L_PromotionDaysElapsed;
        private System.Windows.Forms.NumericUpDown NUD_PromotionDaysElapsed;
        private System.Windows.Forms.Label L_VisitingPlayerDatabase;
        private System.Windows.Forms.DataGridView DGV_VisitingPlayerDatabase;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Index;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_TID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_SID;
    }
}
