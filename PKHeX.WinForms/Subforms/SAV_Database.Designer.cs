namespace PKHeX.WinForms
{
    partial class SAV_Database
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
            components = new System.ComponentModel.Container();
            SCR_Box = new System.Windows.Forms.VScrollBar();
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            Menu_Close = new System.Windows.Forms.ToolStripMenuItem();
            Menu_Exit = new System.Windows.Forms.ToolStripMenuItem();
            Menu_Tools = new System.Windows.Forms.ToolStripMenuItem();
            Menu_SearchSettings = new System.Windows.Forms.ToolStripMenuItem();
            Menu_SearchBoxes = new System.Windows.Forms.ToolStripMenuItem();
            Menu_SearchDatabase = new System.Windows.Forms.ToolStripMenuItem();
            Menu_SearchBackups = new System.Windows.Forms.ToolStripMenuItem();
            Menu_SearchLegal = new System.Windows.Forms.ToolStripMenuItem();
            Menu_SearchIllegal = new System.Windows.Forms.ToolStripMenuItem();
            Menu_SearchClones = new System.Windows.Forms.ToolStripMenuItem();
            Menu_OpenDB = new System.Windows.Forms.ToolStripMenuItem();
            Menu_Report = new System.Windows.Forms.ToolStripMenuItem();
            Menu_Export = new System.Windows.Forms.ToolStripMenuItem();
            Menu_Import = new System.Windows.Forms.ToolStripMenuItem();
            Menu_DeleteClones = new System.Windows.Forms.ToolStripMenuItem();
            P_Results = new System.Windows.Forms.Panel();
            DatabasePokeGrid = new PKHeX.WinForms.Controls.PokeGrid();
            UC_EntitySearch = new PKHeX.WinForms.Controls.EntitySearchControl();
            B_Search = new System.Windows.Forms.Button();
            B_Reset = new System.Windows.Forms.Button();
            L_Count = new System.Windows.Forms.Label();
            L_Viewed = new System.Windows.Forms.Label();
            mnu = new System.Windows.Forms.ContextMenuStrip(components);
            mnuView = new System.Windows.Forms.ToolStripMenuItem();
            mnuDelete = new System.Windows.Forms.ToolStripMenuItem();
            hover = new System.Windows.Forms.ToolTip(components);
            TC_SearchSettings = new System.Windows.Forms.TabControl();
            Tab_General = new System.Windows.Forms.TabPage();
            Tab_Advanced = new System.Windows.Forms.TabPage();
            B_Add = new System.Windows.Forms.Button();
            RTB_Instructions = new System.Windows.Forms.RichTextBox();
            menuStrip1.SuspendLayout();
            P_Results.SuspendLayout();
            mnu.SuspendLayout();
            TC_SearchSettings.SuspendLayout();
            Tab_General.SuspendLayout();
            Tab_Advanced.SuspendLayout();
            SuspendLayout();
            // 
            // SCR_Box
            // 
            SCR_Box.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            SCR_Box.LargeChange = 1;
            SCR_Box.Location = new System.Drawing.Point(299, 3);
            SCR_Box.Name = "SCR_Box";
            SCR_Box.Size = new System.Drawing.Size(24, 479);
            SCR_Box.TabIndex = 1;
            SCR_Box.Scroll += UpdateScroll;
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = System.Drawing.Color.Transparent;
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { Menu_Close, Menu_Tools });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            menuStrip1.Size = new System.Drawing.Size(692, 25);
            menuStrip1.TabIndex = 65;
            menuStrip1.Text = "menuStrip1";
            // 
            // Menu_Close
            // 
            Menu_Close.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { Menu_Exit });
            Menu_Close.Name = "Menu_Close";
            Menu_Close.Size = new System.Drawing.Size(39, 21);
            Menu_Close.Text = "File";
            // 
            // Menu_Exit
            // 
            Menu_Exit.Image = Properties.Resources.exit;
            Menu_Exit.Name = "Menu_Exit";
            Menu_Exit.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E;
            Menu_Exit.ShowShortcutKeys = false;
            Menu_Exit.Size = new System.Drawing.Size(100, 22);
            Menu_Exit.Text = "&Close";
            Menu_Exit.Click += Menu_Exit_Click;
            // 
            // Menu_Tools
            // 
            Menu_Tools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { Menu_SearchSettings, Menu_OpenDB, Menu_Report, Menu_Export, Menu_Import, Menu_DeleteClones });
            Menu_Tools.Name = "Menu_Tools";
            Menu_Tools.Size = new System.Drawing.Size(51, 21);
            Menu_Tools.Text = "Tools";
            // 
            // Menu_SearchSettings
            // 
            Menu_SearchSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { Menu_SearchBoxes, Menu_SearchDatabase, Menu_SearchBackups, Menu_SearchLegal, Menu_SearchIllegal, Menu_SearchClones });
            Menu_SearchSettings.Image = Properties.Resources.settings;
            Menu_SearchSettings.Name = "Menu_SearchSettings";
            Menu_SearchSettings.Size = new System.Drawing.Size(226, 22);
            Menu_SearchSettings.Text = "Search Settings";
            // 
            // Menu_SearchBoxes
            // 
            Menu_SearchBoxes.Checked = true;
            Menu_SearchBoxes.CheckOnClick = true;
            Menu_SearchBoxes.CheckState = System.Windows.Forms.CheckState.Checked;
            Menu_SearchBoxes.Name = "Menu_SearchBoxes";
            Menu_SearchBoxes.Size = new System.Drawing.Size(214, 22);
            Menu_SearchBoxes.Text = "Search Within Boxes";
            // 
            // Menu_SearchDatabase
            // 
            Menu_SearchDatabase.Checked = true;
            Menu_SearchDatabase.CheckOnClick = true;
            Menu_SearchDatabase.CheckState = System.Windows.Forms.CheckState.Checked;
            Menu_SearchDatabase.Name = "Menu_SearchDatabase";
            Menu_SearchDatabase.Size = new System.Drawing.Size(214, 22);
            Menu_SearchDatabase.Text = "Search Within Database";
            // 
            // Menu_SearchBackups
            // 
            Menu_SearchBackups.Checked = true;
            Menu_SearchBackups.CheckOnClick = true;
            Menu_SearchBackups.CheckState = System.Windows.Forms.CheckState.Checked;
            Menu_SearchBackups.Name = "Menu_SearchBackups";
            Menu_SearchBackups.Size = new System.Drawing.Size(214, 22);
            Menu_SearchBackups.Text = "Search Within Backups";
            // 
            // Menu_SearchLegal
            // 
            Menu_SearchLegal.Checked = true;
            Menu_SearchLegal.CheckOnClick = true;
            Menu_SearchLegal.CheckState = System.Windows.Forms.CheckState.Checked;
            Menu_SearchLegal.Name = "Menu_SearchLegal";
            Menu_SearchLegal.Size = new System.Drawing.Size(214, 22);
            Menu_SearchLegal.Text = "Show Legal";
            // 
            // Menu_SearchIllegal
            // 
            Menu_SearchIllegal.Checked = true;
            Menu_SearchIllegal.CheckOnClick = true;
            Menu_SearchIllegal.CheckState = System.Windows.Forms.CheckState.Checked;
            Menu_SearchIllegal.Name = "Menu_SearchIllegal";
            Menu_SearchIllegal.Size = new System.Drawing.Size(214, 22);
            Menu_SearchIllegal.Text = "Show Illegal";
            // 
            // Menu_SearchClones
            // 
            Menu_SearchClones.CheckOnClick = true;
            Menu_SearchClones.Name = "Menu_SearchClones";
            Menu_SearchClones.Size = new System.Drawing.Size(214, 22);
            Menu_SearchClones.Text = "Clones Only";
            // 
            // Menu_OpenDB
            // 
            Menu_OpenDB.Image = Properties.Resources.folder;
            Menu_OpenDB.Name = "Menu_OpenDB";
            Menu_OpenDB.Size = new System.Drawing.Size(226, 22);
            Menu_OpenDB.Text = "Open Database Folder";
            Menu_OpenDB.Click += OpenDB;
            // 
            // Menu_Report
            // 
            Menu_Report.Image = Properties.Resources.report;
            Menu_Report.Name = "Menu_Report";
            Menu_Report.Size = new System.Drawing.Size(226, 22);
            Menu_Report.Text = "Create Data Report";
            Menu_Report.Click += GenerateDBReport;
            // 
            // Menu_Export
            // 
            Menu_Export.Image = Properties.Resources.export;
            Menu_Export.Name = "Menu_Export";
            Menu_Export.Size = new System.Drawing.Size(226, 22);
            Menu_Export.Text = "Export Results to Folder";
            Menu_Export.Click += Menu_Export_Click;
            // 
            // Menu_Import
            // 
            Menu_Import.Image = Properties.Resources.savePKM;
            Menu_Import.Name = "Menu_Import";
            Menu_Import.Size = new System.Drawing.Size(226, 22);
            Menu_Import.Text = "Import Results to SaveFile";
            Menu_Import.Click += Menu_Import_Click;
            // 
            // Menu_DeleteClones
            // 
            Menu_DeleteClones.Image = Properties.Resources.nocheck;
            Menu_DeleteClones.Name = "Menu_DeleteClones";
            Menu_DeleteClones.Size = new System.Drawing.Size(226, 22);
            Menu_DeleteClones.Text = "Delete Clones";
            Menu_DeleteClones.Click += Menu_DeleteClones_Click;
            // 
            // P_Results
            // 
            P_Results.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            P_Results.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            P_Results.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            P_Results.Controls.Add(DatabasePokeGrid);
            P_Results.Controls.Add(SCR_Box);
            P_Results.Location = new System.Drawing.Point(14, 37);
            P_Results.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            P_Results.Name = "P_Results";
            P_Results.Size = new System.Drawing.Size(332, 488);
            P_Results.TabIndex = 66;
            // 
            // DatabasePokeGrid
            // 
            DatabasePokeGrid.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            DatabasePokeGrid.Location = new System.Drawing.Point(2, 2);
            DatabasePokeGrid.Margin = new System.Windows.Forms.Padding(0);
            DatabasePokeGrid.Name = "DatabasePokeGrid";
            DatabasePokeGrid.Size = new System.Drawing.Size(293, 484);
            DatabasePokeGrid.TabIndex = 2;
            // 
            // UC_EntitySearch
            // 
            UC_EntitySearch.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            UC_EntitySearch.Dock = System.Windows.Forms.DockStyle.Fill;
            UC_EntitySearch.Location = new System.Drawing.Point(4, 3);
            UC_EntitySearch.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            UC_EntitySearch.Name = "UC_EntitySearch";
            UC_EntitySearch.Size = new System.Drawing.Size(314, 449);
            UC_EntitySearch.TabIndex = 118;
            // 
            // B_Search
            // 
            B_Search.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Search.Location = new System.Drawing.Point(355, 495);
            B_Search.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Search.Name = "B_Search";
            B_Search.Size = new System.Drawing.Size(330, 32);
            B_Search.TabIndex = 102;
            B_Search.Text = "Search!";
            B_Search.UseVisualStyleBackColor = true;
            B_Search.Click += B_Search_Click;
            // 
            // B_Reset
            // 
            B_Reset.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            B_Reset.Location = new System.Drawing.Point(604, 0);
            B_Reset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Reset.Name = "B_Reset";
            B_Reset.Size = new System.Drawing.Size(88, 27);
            B_Reset.TabIndex = 111;
            B_Reset.Text = "Reset Filters";
            B_Reset.UseVisualStyleBackColor = true;
            B_Reset.Click += ResetFilters;
            // 
            // L_Count
            // 
            L_Count.Location = new System.Drawing.Point(115, 21);
            L_Count.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Count.Name = "L_Count";
            L_Count.Size = new System.Drawing.Size(97, 15);
            L_Count.TabIndex = 114;
            L_Count.Text = "Count: {0}";
            L_Count.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // L_Viewed
            // 
            L_Viewed.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            L_Viewed.AutoSize = true;
            L_Viewed.Location = new System.Drawing.Point(10, 528);
            L_Viewed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Viewed.Name = "L_Viewed";
            L_Viewed.Size = new System.Drawing.Size(99, 17);
            L_Viewed.TabIndex = 117;
            L_Viewed.Text = "Last Viewed: {0}";
            L_Viewed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            L_Viewed.MouseEnter += L_Viewed_MouseEnter;
            // 
            // mnu
            // 
            mnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuView, mnuDelete });
            mnu.Name = "mnu";
            mnu.Size = new System.Drawing.Size(114, 48);
            // 
            // mnuView
            // 
            mnuView.Image = Properties.Resources.other;
            mnuView.Name = "mnuView";
            mnuView.Size = new System.Drawing.Size(113, 22);
            mnuView.Text = "View";
            mnuView.Click += ClickView;
            // 
            // mnuDelete
            // 
            mnuDelete.Image = Properties.Resources.nocheck;
            mnuDelete.Name = "mnuDelete";
            mnuDelete.Size = new System.Drawing.Size(113, 22);
            mnuDelete.Text = "Delete";
            mnuDelete.Click += ClickDelete;
            // 
            // TC_SearchSettings
            // 
            TC_SearchSettings.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            TC_SearchSettings.Controls.Add(Tab_General);
            TC_SearchSettings.Controls.Add(Tab_Advanced);
            TC_SearchSettings.Location = new System.Drawing.Point(355, 9);
            TC_SearchSettings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TC_SearchSettings.Name = "TC_SearchSettings";
            TC_SearchSettings.Padding = new System.Drawing.Point(0, 0);
            TC_SearchSettings.SelectedIndex = 0;
            TC_SearchSettings.Size = new System.Drawing.Size(330, 485);
            TC_SearchSettings.TabIndex = 120;
            // 
            // Tab_General
            // 
            Tab_General.Controls.Add(UC_EntitySearch);
            Tab_General.Location = new System.Drawing.Point(4, 26);
            Tab_General.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_General.Name = "Tab_General";
            Tab_General.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_General.Size = new System.Drawing.Size(322, 455);
            Tab_General.TabIndex = 0;
            Tab_General.Text = "General";
            Tab_General.UseVisualStyleBackColor = true;
            // 
            // Tab_Advanced
            // 
            Tab_Advanced.Controls.Add(B_Add);
            Tab_Advanced.Controls.Add(RTB_Instructions);
            Tab_Advanced.Location = new System.Drawing.Point(4, 26);
            Tab_Advanced.Margin = new System.Windows.Forms.Padding(0);
            Tab_Advanced.Name = "Tab_Advanced";
            Tab_Advanced.Size = new System.Drawing.Size(322, 455);
            Tab_Advanced.TabIndex = 1;
            Tab_Advanced.Text = "Advanced";
            Tab_Advanced.UseVisualStyleBackColor = true;
            // 
            // B_Add
            // 
            B_Add.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            B_Add.Location = new System.Drawing.Point(252, 0);
            B_Add.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Add.Name = "B_Add";
            B_Add.Size = new System.Drawing.Size(70, 27);
            B_Add.TabIndex = 122;
            B_Add.Text = "Add";
            B_Add.UseVisualStyleBackColor = true;
            B_Add.Click += B_Add_Click;
            // 
            // RTB_Instructions
            // 
            RTB_Instructions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            RTB_Instructions.Location = new System.Drawing.Point(0, 59);
            RTB_Instructions.Margin = new System.Windows.Forms.Padding(0);
            RTB_Instructions.Name = "RTB_Instructions";
            RTB_Instructions.Size = new System.Drawing.Size(322, 396);
            RTB_Instructions.TabIndex = 120;
            RTB_Instructions.Text = "";
            // 
            // SAV_Database
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(692, 545);
            Controls.Add(B_Reset);
            Controls.Add(TC_SearchSettings);
            Controls.Add(B_Search);
            Controls.Add(L_Viewed);
            Controls.Add(L_Count);
            Controls.Add(P_Results);
            Controls.Add(menuStrip1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_Database";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Database";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            P_Results.ResumeLayout(false);
            mnu.ResumeLayout(false);
            TC_SearchSettings.ResumeLayout(false);
            Tab_General.ResumeLayout(false);
            Tab_Advanced.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.VScrollBar SCR_Box;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem Menu_Close;
        private System.Windows.Forms.ToolStripMenuItem Menu_Exit;
        private System.Windows.Forms.ToolStripMenuItem Menu_Tools;
        private System.Windows.Forms.ToolStripMenuItem Menu_OpenDB;
        private System.Windows.Forms.ToolStripMenuItem Menu_Report;
        private System.Windows.Forms.Panel P_Results;
        private System.Windows.Forms.Button B_Search;
        private System.Windows.Forms.Button B_Reset;
        private System.Windows.Forms.Label L_Count;
        private System.Windows.Forms.Label L_Viewed;
        private System.Windows.Forms.ToolStripMenuItem Menu_Export;
        private System.Windows.Forms.ToolStripMenuItem Menu_SearchSettings;
        private System.Windows.Forms.ToolStripMenuItem Menu_SearchBoxes;
        private System.Windows.Forms.ToolStripMenuItem Menu_SearchDatabase;
        private System.Windows.Forms.ToolStripMenuItem Menu_SearchLegal;
        private System.Windows.Forms.ToolStripMenuItem Menu_SearchIllegal;
        private Controls.EntitySearchControl UC_EntitySearch;
        private System.Windows.Forms.ToolStripMenuItem Menu_SearchClones;
        private System.Windows.Forms.ToolStripMenuItem Menu_DeleteClones;
        private System.Windows.Forms.ToolStripMenuItem Menu_Import;
        private System.Windows.Forms.ContextMenuStrip mnu;
        private System.Windows.Forms.ToolStripMenuItem mnuView;
        private System.Windows.Forms.ToolStripMenuItem mnuDelete;
        private System.Windows.Forms.ToolTip hover;
        private Controls.PokeGrid DatabasePokeGrid;
        private System.Windows.Forms.TabControl TC_SearchSettings;
        private System.Windows.Forms.TabPage Tab_General;
        private System.Windows.Forms.TabPage Tab_Advanced;
        private System.Windows.Forms.RichTextBox RTB_Instructions;
        private System.Windows.Forms.ToolStripMenuItem Menu_SearchBackups;
        private System.Windows.Forms.Button B_Add;
    }
}
