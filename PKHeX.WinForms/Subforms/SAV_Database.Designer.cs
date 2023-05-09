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
            DatabasePokeGrid = new Controls.PokeGrid();
            CB_Ability = new System.Windows.Forms.ComboBox();
            CB_HeldItem = new System.Windows.Forms.ComboBox();
            CB_Nature = new System.Windows.Forms.ComboBox();
            CB_Species = new System.Windows.Forms.ComboBox();
            CB_Move4 = new System.Windows.Forms.ComboBox();
            CB_Move3 = new System.Windows.Forms.ComboBox();
            CB_Move2 = new System.Windows.Forms.ComboBox();
            CB_Move1 = new System.Windows.Forms.ComboBox();
            TB_Level = new System.Windows.Forms.MaskedTextBox();
            Label_CurLevel = new System.Windows.Forms.Label();
            Label_HeldItem = new System.Windows.Forms.Label();
            Label_Ability = new System.Windows.Forms.Label();
            Label_Nature = new System.Windows.Forms.Label();
            Label_Species = new System.Windows.Forms.Label();
            CB_EVTrain = new System.Windows.Forms.ComboBox();
            CB_HPType = new System.Windows.Forms.ComboBox();
            Label_HiddenPowerPrefix = new System.Windows.Forms.Label();
            CB_GameOrigin = new System.Windows.Forms.ComboBox();
            CB_IV = new System.Windows.Forms.ComboBox();
            B_Search = new System.Windows.Forms.Button();
            CB_Level = new System.Windows.Forms.ComboBox();
            L_Version = new System.Windows.Forms.Label();
            L_Move1 = new System.Windows.Forms.Label();
            L_Move2 = new System.Windows.Forms.Label();
            L_Move3 = new System.Windows.Forms.Label();
            L_Move4 = new System.Windows.Forms.Label();
            L_Potential = new System.Windows.Forms.Label();
            L_EVTraining = new System.Windows.Forms.Label();
            B_Reset = new System.Windows.Forms.Button();
            L_Count = new System.Windows.Forms.Label();
            L_Generation = new System.Windows.Forms.Label();
            CB_Generation = new System.Windows.Forms.ComboBox();
            L_Viewed = new System.Windows.Forms.Label();
            FLP_Egg = new System.Windows.Forms.FlowLayoutPanel();
            CHK_IsEgg = new System.Windows.Forms.CheckBox();
            L_ESV = new System.Windows.Forms.Label();
            MT_ESV = new System.Windows.Forms.MaskedTextBox();
            CHK_Shiny = new System.Windows.Forms.CheckBox();
            TLP_Filters = new System.Windows.Forms.TableLayoutPanel();
            FLP_Format = new System.Windows.Forms.FlowLayoutPanel();
            CB_FormatComparator = new System.Windows.Forms.ComboBox();
            CB_Format = new System.Windows.Forms.ComboBox();
            L_Format = new System.Windows.Forms.Label();
            FLP_Level = new System.Windows.Forms.FlowLayoutPanel();
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
            FLP_Egg.SuspendLayout();
            TLP_Filters.SuspendLayout();
            FLP_Format.SuspendLayout();
            FLP_Level.SuspendLayout();
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
            SCR_Box.Size = new System.Drawing.Size(24, 397);
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
            menuStrip1.Size = new System.Drawing.Size(670, 24);
            menuStrip1.TabIndex = 65;
            menuStrip1.Text = "menuStrip1";
            // 
            // Menu_Close
            // 
            Menu_Close.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { Menu_Exit });
            Menu_Close.Name = "Menu_Close";
            Menu_Close.Size = new System.Drawing.Size(37, 20);
            Menu_Close.Text = "File";
            // 
            // Menu_Exit
            // 
            Menu_Exit.Image = Properties.Resources.exit;
            Menu_Exit.Name = "Menu_Exit";
            Menu_Exit.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E;
            Menu_Exit.ShowShortcutKeys = false;
            Menu_Exit.Size = new System.Drawing.Size(96, 22);
            Menu_Exit.Text = "&Close";
            Menu_Exit.Click += Menu_Exit_Click;
            // 
            // Menu_Tools
            // 
            Menu_Tools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { Menu_SearchSettings, Menu_OpenDB, Menu_Report, Menu_Export, Menu_Import, Menu_DeleteClones });
            Menu_Tools.Name = "Menu_Tools";
            Menu_Tools.Size = new System.Drawing.Size(46, 20);
            Menu_Tools.Text = "Tools";
            // 
            // Menu_SearchSettings
            // 
            Menu_SearchSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { Menu_SearchBoxes, Menu_SearchDatabase, Menu_SearchBackups, Menu_SearchLegal, Menu_SearchIllegal, Menu_SearchClones });
            Menu_SearchSettings.Image = Properties.Resources.settings;
            Menu_SearchSettings.Name = "Menu_SearchSettings";
            Menu_SearchSettings.Size = new System.Drawing.Size(209, 22);
            Menu_SearchSettings.Text = "Search Settings";
            // 
            // Menu_SearchBoxes
            // 
            Menu_SearchBoxes.Checked = true;
            Menu_SearchBoxes.CheckOnClick = true;
            Menu_SearchBoxes.CheckState = System.Windows.Forms.CheckState.Checked;
            Menu_SearchBoxes.Name = "Menu_SearchBoxes";
            Menu_SearchBoxes.Size = new System.Drawing.Size(198, 22);
            Menu_SearchBoxes.Text = "Search Within Boxes";
            // 
            // Menu_SearchDatabase
            // 
            Menu_SearchDatabase.Checked = true;
            Menu_SearchDatabase.CheckOnClick = true;
            Menu_SearchDatabase.CheckState = System.Windows.Forms.CheckState.Checked;
            Menu_SearchDatabase.Name = "Menu_SearchDatabase";
            Menu_SearchDatabase.Size = new System.Drawing.Size(198, 22);
            Menu_SearchDatabase.Text = "Search Within Database";
            // 
            // Menu_SearchBackups
            // 
            Menu_SearchBackups.Checked = true;
            Menu_SearchBackups.CheckOnClick = true;
            Menu_SearchBackups.CheckState = System.Windows.Forms.CheckState.Checked;
            Menu_SearchBackups.Name = "Menu_SearchBackups";
            Menu_SearchBackups.Size = new System.Drawing.Size(198, 22);
            Menu_SearchBackups.Text = "Search Within Backups";
            // 
            // Menu_SearchLegal
            // 
            Menu_SearchLegal.Checked = true;
            Menu_SearchLegal.CheckOnClick = true;
            Menu_SearchLegal.CheckState = System.Windows.Forms.CheckState.Checked;
            Menu_SearchLegal.Name = "Menu_SearchLegal";
            Menu_SearchLegal.Size = new System.Drawing.Size(198, 22);
            Menu_SearchLegal.Text = "Show Legal";
            // 
            // Menu_SearchIllegal
            // 
            Menu_SearchIllegal.Checked = true;
            Menu_SearchIllegal.CheckOnClick = true;
            Menu_SearchIllegal.CheckState = System.Windows.Forms.CheckState.Checked;
            Menu_SearchIllegal.Name = "Menu_SearchIllegal";
            Menu_SearchIllegal.Size = new System.Drawing.Size(198, 22);
            Menu_SearchIllegal.Text = "Show Illegal";
            // 
            // Menu_SearchClones
            // 
            Menu_SearchClones.CheckOnClick = true;
            Menu_SearchClones.Name = "Menu_SearchClones";
            Menu_SearchClones.Size = new System.Drawing.Size(198, 22);
            Menu_SearchClones.Text = "Clones Only";
            // 
            // Menu_OpenDB
            // 
            Menu_OpenDB.Image = Properties.Resources.folder;
            Menu_OpenDB.Name = "Menu_OpenDB";
            Menu_OpenDB.Size = new System.Drawing.Size(209, 22);
            Menu_OpenDB.Text = "Open Database Folder";
            Menu_OpenDB.Click += OpenDB;
            // 
            // Menu_Report
            // 
            Menu_Report.Image = Properties.Resources.report;
            Menu_Report.Name = "Menu_Report";
            Menu_Report.Size = new System.Drawing.Size(209, 22);
            Menu_Report.Text = "Create Data Report";
            Menu_Report.Click += GenerateDBReport;
            // 
            // Menu_Export
            // 
            Menu_Export.Image = Properties.Resources.export;
            Menu_Export.Name = "Menu_Export";
            Menu_Export.Size = new System.Drawing.Size(209, 22);
            Menu_Export.Text = "Export Results to Folder";
            Menu_Export.Click += Menu_Export_Click;
            // 
            // Menu_Import
            // 
            Menu_Import.Image = Properties.Resources.savePKM;
            Menu_Import.Name = "Menu_Import";
            Menu_Import.Size = new System.Drawing.Size(209, 22);
            Menu_Import.Text = "Import Results to SaveFile";
            Menu_Import.Click += Menu_Import_Click;
            // 
            // Menu_DeleteClones
            // 
            Menu_DeleteClones.Image = Properties.Resources.nocheck;
            Menu_DeleteClones.Name = "Menu_DeleteClones";
            Menu_DeleteClones.Size = new System.Drawing.Size(209, 22);
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
            P_Results.Size = new System.Drawing.Size(332, 406);
            P_Results.TabIndex = 66;
            // 
            // DatabasePokeGrid
            // 
            DatabasePokeGrid.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            DatabasePokeGrid.Location = new System.Drawing.Point(2, 2);
            DatabasePokeGrid.Margin = new System.Windows.Forms.Padding(0);
            DatabasePokeGrid.Name = "DatabasePokeGrid";
            DatabasePokeGrid.Size = new System.Drawing.Size(293, 399);
            DatabasePokeGrid.TabIndex = 2;
            // 
            // CB_Ability
            // 
            CB_Ability.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_Ability.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Ability.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Ability.FormattingEnabled = true;
            CB_Ability.Items.AddRange(new object[] { "Item" });
            CB_Ability.Location = new System.Drawing.Point(93, 92);
            CB_Ability.Margin = new System.Windows.Forms.Padding(0);
            CB_Ability.Name = "CB_Ability";
            CB_Ability.Size = new System.Drawing.Size(142, 23);
            CB_Ability.TabIndex = 70;
            // 
            // CB_HeldItem
            // 
            CB_HeldItem.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_HeldItem.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_HeldItem.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_HeldItem.FormattingEnabled = true;
            CB_HeldItem.Location = new System.Drawing.Point(93, 69);
            CB_HeldItem.Margin = new System.Windows.Forms.Padding(0);
            CB_HeldItem.Name = "CB_HeldItem";
            CB_HeldItem.Size = new System.Drawing.Size(142, 23);
            CB_HeldItem.TabIndex = 69;
            // 
            // CB_Nature
            // 
            CB_Nature.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_Nature.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Nature.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Nature.FormattingEnabled = true;
            CB_Nature.Location = new System.Drawing.Point(93, 46);
            CB_Nature.Margin = new System.Windows.Forms.Padding(0);
            CB_Nature.Name = "CB_Nature";
            CB_Nature.Size = new System.Drawing.Size(142, 23);
            CB_Nature.TabIndex = 68;
            // 
            // CB_Species
            // 
            CB_Species.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_Species.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Species.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Species.FormattingEnabled = true;
            CB_Species.Location = new System.Drawing.Point(93, 23);
            CB_Species.Margin = new System.Windows.Forms.Padding(0);
            CB_Species.Name = "CB_Species";
            CB_Species.Size = new System.Drawing.Size(142, 23);
            CB_Species.TabIndex = 67;
            // 
            // CB_Move4
            // 
            CB_Move4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_Move4.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Move4.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Move4.FormattingEnabled = true;
            CB_Move4.Location = new System.Drawing.Point(93, 276);
            CB_Move4.Margin = new System.Windows.Forms.Padding(0);
            CB_Move4.Name = "CB_Move4";
            CB_Move4.Size = new System.Drawing.Size(142, 23);
            CB_Move4.TabIndex = 74;
            // 
            // CB_Move3
            // 
            CB_Move3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_Move3.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Move3.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Move3.FormattingEnabled = true;
            CB_Move3.Location = new System.Drawing.Point(93, 253);
            CB_Move3.Margin = new System.Windows.Forms.Padding(0);
            CB_Move3.Name = "CB_Move3";
            CB_Move3.Size = new System.Drawing.Size(142, 23);
            CB_Move3.TabIndex = 73;
            // 
            // CB_Move2
            // 
            CB_Move2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_Move2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Move2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Move2.FormattingEnabled = true;
            CB_Move2.Location = new System.Drawing.Point(93, 230);
            CB_Move2.Margin = new System.Windows.Forms.Padding(0);
            CB_Move2.Name = "CB_Move2";
            CB_Move2.Size = new System.Drawing.Size(142, 23);
            CB_Move2.TabIndex = 72;
            // 
            // CB_Move1
            // 
            CB_Move1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_Move1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Move1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Move1.FormattingEnabled = true;
            CB_Move1.Location = new System.Drawing.Point(93, 207);
            CB_Move1.Margin = new System.Windows.Forms.Padding(0);
            CB_Move1.Name = "CB_Move1";
            CB_Move1.Size = new System.Drawing.Size(142, 23);
            CB_Move1.TabIndex = 71;
            // 
            // TB_Level
            // 
            TB_Level.Anchor = System.Windows.Forms.AnchorStyles.Left;
            TB_Level.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_Level.Location = new System.Drawing.Point(0, 0);
            TB_Level.Margin = new System.Windows.Forms.Padding(0);
            TB_Level.Mask = "000";
            TB_Level.Name = "TB_Level";
            TB_Level.Size = new System.Drawing.Size(25, 23);
            TB_Level.TabIndex = 89;
            TB_Level.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Label_CurLevel
            // 
            Label_CurLevel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            Label_CurLevel.AutoSize = true;
            Label_CurLevel.Location = new System.Drawing.Point(52, 119);
            Label_CurLevel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Label_CurLevel.Name = "Label_CurLevel";
            Label_CurLevel.Size = new System.Drawing.Size(37, 15);
            Label_CurLevel.TabIndex = 88;
            Label_CurLevel.Text = "Level:";
            Label_CurLevel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_HeldItem
            // 
            Label_HeldItem.Anchor = System.Windows.Forms.AnchorStyles.Right;
            Label_HeldItem.AutoSize = true;
            Label_HeldItem.Location = new System.Drawing.Point(27, 73);
            Label_HeldItem.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Label_HeldItem.Name = "Label_HeldItem";
            Label_HeldItem.Size = new System.Drawing.Size(62, 15);
            Label_HeldItem.TabIndex = 93;
            Label_HeldItem.Text = "Held Item:";
            Label_HeldItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_Ability
            // 
            Label_Ability.Anchor = System.Windows.Forms.AnchorStyles.Right;
            Label_Ability.AutoSize = true;
            Label_Ability.Location = new System.Drawing.Point(45, 96);
            Label_Ability.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Label_Ability.Name = "Label_Ability";
            Label_Ability.Size = new System.Drawing.Size(44, 15);
            Label_Ability.TabIndex = 92;
            Label_Ability.Text = "Ability:";
            Label_Ability.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_Nature
            // 
            Label_Nature.Anchor = System.Windows.Forms.AnchorStyles.Right;
            Label_Nature.AutoSize = true;
            Label_Nature.Location = new System.Drawing.Point(43, 50);
            Label_Nature.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Label_Nature.Name = "Label_Nature";
            Label_Nature.Size = new System.Drawing.Size(46, 15);
            Label_Nature.TabIndex = 91;
            Label_Nature.Text = "Nature:";
            Label_Nature.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_Species
            // 
            Label_Species.Anchor = System.Windows.Forms.AnchorStyles.Right;
            Label_Species.AutoSize = true;
            Label_Species.Location = new System.Drawing.Point(40, 27);
            Label_Species.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Label_Species.Name = "Label_Species";
            Label_Species.Size = new System.Drawing.Size(49, 15);
            Label_Species.TabIndex = 90;
            Label_Species.Text = "Species:";
            Label_Species.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_EVTrain
            // 
            CB_EVTrain.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_EVTrain.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_EVTrain.DropDownWidth = 85;
            CB_EVTrain.FormattingEnabled = true;
            CB_EVTrain.Items.AddRange(new object[] { "Any", "None (0)", "Some (127-1)", "Half (128-507)", "Full (508+)" });
            CB_EVTrain.Location = new System.Drawing.Point(93, 161);
            CB_EVTrain.Margin = new System.Windows.Forms.Padding(0);
            CB_EVTrain.Name = "CB_EVTrain";
            CB_EVTrain.Size = new System.Drawing.Size(109, 23);
            CB_EVTrain.TabIndex = 94;
            // 
            // CB_HPType
            // 
            CB_HPType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_HPType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_HPType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_HPType.DropDownWidth = 80;
            CB_HPType.FormattingEnabled = true;
            CB_HPType.Location = new System.Drawing.Point(93, 184);
            CB_HPType.Margin = new System.Windows.Forms.Padding(0);
            CB_HPType.Name = "CB_HPType";
            CB_HPType.Size = new System.Drawing.Size(142, 23);
            CB_HPType.TabIndex = 96;
            // 
            // Label_HiddenPowerPrefix
            // 
            Label_HiddenPowerPrefix.Anchor = System.Windows.Forms.AnchorStyles.Right;
            Label_HiddenPowerPrefix.AutoSize = true;
            Label_HiddenPowerPrefix.Location = new System.Drawing.Point(4, 188);
            Label_HiddenPowerPrefix.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Label_HiddenPowerPrefix.Name = "Label_HiddenPowerPrefix";
            Label_HiddenPowerPrefix.Size = new System.Drawing.Size(85, 15);
            Label_HiddenPowerPrefix.TabIndex = 95;
            Label_HiddenPowerPrefix.Text = "Hidden Power:";
            Label_HiddenPowerPrefix.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_GameOrigin
            // 
            CB_GameOrigin.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_GameOrigin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_GameOrigin.FormattingEnabled = true;
            CB_GameOrigin.Location = new System.Drawing.Point(93, 299);
            CB_GameOrigin.Margin = new System.Windows.Forms.Padding(0);
            CB_GameOrigin.Name = "CB_GameOrigin";
            CB_GameOrigin.Size = new System.Drawing.Size(142, 23);
            CB_GameOrigin.TabIndex = 97;
            CB_GameOrigin.SelectedIndexChanged += ChangeGame;
            // 
            // CB_IV
            // 
            CB_IV.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_IV.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_IV.DropDownWidth = 85;
            CB_IV.FormattingEnabled = true;
            CB_IV.Items.AddRange(new object[] { "Any", "<= 90", "91-120", "121-150", "151-179", "180+", "== 186" });
            CB_IV.Location = new System.Drawing.Point(93, 138);
            CB_IV.Margin = new System.Windows.Forms.Padding(0);
            CB_IV.Name = "CB_IV";
            CB_IV.Size = new System.Drawing.Size(109, 23);
            CB_IV.TabIndex = 100;
            // 
            // B_Search
            // 
            B_Search.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Search.Location = new System.Drawing.Point(388, 464);
            B_Search.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Search.Name = "B_Search";
            B_Search.Size = new System.Drawing.Size(240, 35);
            B_Search.TabIndex = 102;
            B_Search.Text = "Search!";
            B_Search.UseVisualStyleBackColor = true;
            B_Search.Click += B_Search_Click;
            // 
            // CB_Level
            // 
            CB_Level.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_Level.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Level.DropDownWidth = 85;
            CB_Level.FormattingEnabled = true;
            CB_Level.Items.AddRange(new object[] { "Any", "==", ">=", "<=" });
            CB_Level.Location = new System.Drawing.Point(25, 0);
            CB_Level.Margin = new System.Windows.Forms.Padding(0);
            CB_Level.Name = "CB_Level";
            CB_Level.Size = new System.Drawing.Size(76, 23);
            CB_Level.TabIndex = 103;
            CB_Level.SelectedIndexChanged += ChangeLevel;
            // 
            // L_Version
            // 
            L_Version.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Version.AutoSize = true;
            L_Version.Location = new System.Drawing.Point(24, 303);
            L_Version.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            L_Version.Name = "L_Version";
            L_Version.Size = new System.Drawing.Size(65, 15);
            L_Version.TabIndex = 104;
            L_Version.Text = "OT Version:";
            L_Version.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Move1
            // 
            L_Move1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Move1.AutoSize = true;
            L_Move1.Location = new System.Drawing.Point(40, 211);
            L_Move1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            L_Move1.Name = "L_Move1";
            L_Move1.Size = new System.Drawing.Size(49, 15);
            L_Move1.TabIndex = 105;
            L_Move1.Text = "Move 1:";
            L_Move1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Move2
            // 
            L_Move2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Move2.AutoSize = true;
            L_Move2.Location = new System.Drawing.Point(40, 234);
            L_Move2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            L_Move2.Name = "L_Move2";
            L_Move2.Size = new System.Drawing.Size(49, 15);
            L_Move2.TabIndex = 106;
            L_Move2.Text = "Move 2:";
            L_Move2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Move3
            // 
            L_Move3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Move3.AutoSize = true;
            L_Move3.Location = new System.Drawing.Point(40, 257);
            L_Move3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            L_Move3.Name = "L_Move3";
            L_Move3.Size = new System.Drawing.Size(49, 15);
            L_Move3.TabIndex = 107;
            L_Move3.Text = "Move 3:";
            L_Move3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Move4
            // 
            L_Move4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Move4.AutoSize = true;
            L_Move4.Location = new System.Drawing.Point(40, 280);
            L_Move4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            L_Move4.Name = "L_Move4";
            L_Move4.Size = new System.Drawing.Size(49, 15);
            L_Move4.TabIndex = 108;
            L_Move4.Text = "Move 4:";
            L_Move4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Potential
            // 
            L_Potential.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Potential.AutoSize = true;
            L_Potential.Location = new System.Drawing.Point(19, 142);
            L_Potential.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            L_Potential.Name = "L_Potential";
            L_Potential.Size = new System.Drawing.Size(70, 15);
            L_Potential.TabIndex = 109;
            L_Potential.Text = "IV Potential:";
            L_Potential.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_EVTraining
            // 
            L_EVTraining.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_EVTraining.AutoSize = true;
            L_EVTraining.Location = new System.Drawing.Point(21, 165);
            L_EVTraining.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            L_EVTraining.Name = "L_EVTraining";
            L_EVTraining.Size = new System.Drawing.Size(68, 15);
            L_EVTraining.TabIndex = 110;
            L_EVTraining.Text = "EV Training:";
            L_EVTraining.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // B_Reset
            // 
            B_Reset.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            B_Reset.Location = new System.Drawing.Point(582, 0);
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
            // L_Generation
            // 
            L_Generation.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Generation.AutoSize = true;
            L_Generation.Location = new System.Drawing.Point(21, 326);
            L_Generation.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            L_Generation.Name = "L_Generation";
            L_Generation.Size = new System.Drawing.Size(68, 15);
            L_Generation.TabIndex = 116;
            L_Generation.Text = "Generation:";
            L_Generation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_Generation
            // 
            CB_Generation.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_Generation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Generation.FormattingEnabled = true;
            CB_Generation.Items.AddRange(new object[] { "Any", "Gen 1 (RBY/GSC)", "Gen 2 (RBY/GSC)", "Gen 3 (RSE/FRLG/CXD)", "Gen 4 (DPPt/HGSS)", "Gen 5 (BW/B2W2)", "Gen 6 (XY/ORAS)", "Gen 7 (SM/USUM/LGPE)", "Gen 8 (SWSH/BDSP/LA)", "Gen 9 (SV)" });
            CB_Generation.Location = new System.Drawing.Point(93, 322);
            CB_Generation.Margin = new System.Windows.Forms.Padding(0);
            CB_Generation.Name = "CB_Generation";
            CB_Generation.Size = new System.Drawing.Size(142, 23);
            CB_Generation.TabIndex = 115;
            CB_Generation.SelectedIndexChanged += ChangeGeneration;
            // 
            // L_Viewed
            // 
            L_Viewed.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            L_Viewed.AutoSize = true;
            L_Viewed.Location = new System.Drawing.Point(10, 495);
            L_Viewed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Viewed.Name = "L_Viewed";
            L_Viewed.Size = new System.Drawing.Size(89, 15);
            L_Viewed.TabIndex = 117;
            L_Viewed.Text = "Last Viewed: {0}";
            L_Viewed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            L_Viewed.MouseEnter += L_Viewed_MouseEnter;
            // 
            // FLP_Egg
            // 
            FLP_Egg.Anchor = System.Windows.Forms.AnchorStyles.Left;
            FLP_Egg.AutoSize = true;
            FLP_Egg.Controls.Add(CHK_IsEgg);
            FLP_Egg.Controls.Add(L_ESV);
            FLP_Egg.Controls.Add(MT_ESV);
            FLP_Egg.Location = new System.Drawing.Point(93, 0);
            FLP_Egg.Margin = new System.Windows.Forms.Padding(0);
            FLP_Egg.Name = "FLP_Egg";
            FLP_Egg.Size = new System.Drawing.Size(132, 23);
            FLP_Egg.TabIndex = 120;
            // 
            // CHK_IsEgg
            // 
            CHK_IsEgg.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CHK_IsEgg.AutoSize = true;
            CHK_IsEgg.Checked = true;
            CHK_IsEgg.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            CHK_IsEgg.Location = new System.Drawing.Point(0, 2);
            CHK_IsEgg.Margin = new System.Windows.Forms.Padding(0);
            CHK_IsEgg.Name = "CHK_IsEgg";
            CHK_IsEgg.Size = new System.Drawing.Size(46, 19);
            CHK_IsEgg.TabIndex = 98;
            CHK_IsEgg.Text = "Egg";
            CHK_IsEgg.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            CHK_IsEgg.ThreeState = true;
            CHK_IsEgg.UseVisualStyleBackColor = true;
            CHK_IsEgg.CheckStateChanged += ToggleESV;
            // 
            // L_ESV
            // 
            L_ESV.Anchor = System.Windows.Forms.AnchorStyles.Left;
            L_ESV.Location = new System.Drawing.Point(46, 1);
            L_ESV.Margin = new System.Windows.Forms.Padding(0);
            L_ESV.Name = "L_ESV";
            L_ESV.Size = new System.Drawing.Size(50, 20);
            L_ESV.TabIndex = 113;
            L_ESV.Text = "ESV:";
            L_ESV.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            L_ESV.Visible = false;
            // 
            // MT_ESV
            // 
            MT_ESV.Anchor = System.Windows.Forms.AnchorStyles.Left;
            MT_ESV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            MT_ESV.Location = new System.Drawing.Point(96, 0);
            MT_ESV.Margin = new System.Windows.Forms.Padding(0);
            MT_ESV.Mask = "0000";
            MT_ESV.Name = "MT_ESV";
            MT_ESV.Size = new System.Drawing.Size(36, 23);
            MT_ESV.TabIndex = 112;
            MT_ESV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            MT_ESV.Visible = false;
            // 
            // CHK_Shiny
            // 
            CHK_Shiny.Anchor = System.Windows.Forms.AnchorStyles.Right;
            CHK_Shiny.AutoSize = true;
            CHK_Shiny.Checked = true;
            CHK_Shiny.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            CHK_Shiny.Location = new System.Drawing.Point(38, 2);
            CHK_Shiny.Margin = new System.Windows.Forms.Padding(0);
            CHK_Shiny.Name = "CHK_Shiny";
            CHK_Shiny.Size = new System.Drawing.Size(55, 19);
            CHK_Shiny.TabIndex = 99;
            CHK_Shiny.Text = "Shiny";
            CHK_Shiny.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            CHK_Shiny.ThreeState = true;
            CHK_Shiny.UseVisualStyleBackColor = true;
            // 
            // TLP_Filters
            // 
            TLP_Filters.AutoScroll = true;
            TLP_Filters.AutoScrollMargin = new System.Drawing.Size(3, 3);
            TLP_Filters.AutoSize = true;
            TLP_Filters.ColumnCount = 2;
            TLP_Filters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Filters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Filters.Controls.Add(FLP_Format, 1, 15);
            TLP_Filters.Controls.Add(L_Format, 0, 15);
            TLP_Filters.Controls.Add(FLP_Egg, 1, 0);
            TLP_Filters.Controls.Add(CHK_Shiny, 0, 0);
            TLP_Filters.Controls.Add(Label_Species, 0, 1);
            TLP_Filters.Controls.Add(CB_Species, 1, 1);
            TLP_Filters.Controls.Add(Label_Nature, 0, 2);
            TLP_Filters.Controls.Add(CB_Nature, 1, 2);
            TLP_Filters.Controls.Add(Label_HeldItem, 0, 3);
            TLP_Filters.Controls.Add(CB_HeldItem, 1, 3);
            TLP_Filters.Controls.Add(Label_Ability, 0, 4);
            TLP_Filters.Controls.Add(CB_Ability, 1, 4);
            TLP_Filters.Controls.Add(FLP_Level, 1, 5);
            TLP_Filters.Controls.Add(Label_CurLevel, 0, 5);
            TLP_Filters.Controls.Add(L_Potential, 0, 6);
            TLP_Filters.Controls.Add(CB_IV, 1, 6);
            TLP_Filters.Controls.Add(L_EVTraining, 0, 7);
            TLP_Filters.Controls.Add(CB_EVTrain, 1, 7);
            TLP_Filters.Controls.Add(Label_HiddenPowerPrefix, 0, 8);
            TLP_Filters.Controls.Add(CB_HPType, 1, 8);
            TLP_Filters.Controls.Add(L_Move1, 0, 9);
            TLP_Filters.Controls.Add(CB_Move1, 1, 9);
            TLP_Filters.Controls.Add(L_Move2, 0, 10);
            TLP_Filters.Controls.Add(CB_Move2, 1, 10);
            TLP_Filters.Controls.Add(L_Move3, 0, 11);
            TLP_Filters.Controls.Add(CB_Move3, 1, 11);
            TLP_Filters.Controls.Add(L_Move4, 0, 12);
            TLP_Filters.Controls.Add(CB_Move4, 1, 12);
            TLP_Filters.Controls.Add(L_Version, 0, 13);
            TLP_Filters.Controls.Add(CB_GameOrigin, 1, 13);
            TLP_Filters.Controls.Add(L_Generation, 0, 14);
            TLP_Filters.Controls.Add(CB_Generation, 1, 14);
            TLP_Filters.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Filters.Location = new System.Drawing.Point(4, 3);
            TLP_Filters.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TLP_Filters.Name = "TLP_Filters";
            TLP_Filters.RowCount = 17;
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.Size = new System.Drawing.Size(292, 414);
            TLP_Filters.TabIndex = 118;
            // 
            // FLP_Format
            // 
            FLP_Format.AutoSize = true;
            FLP_Format.Controls.Add(CB_FormatComparator);
            FLP_Format.Controls.Add(CB_Format);
            FLP_Format.Location = new System.Drawing.Point(93, 345);
            FLP_Format.Margin = new System.Windows.Forms.Padding(0);
            FLP_Format.Name = "FLP_Format";
            FLP_Format.Size = new System.Drawing.Size(141, 23);
            FLP_Format.TabIndex = 124;
            // 
            // CB_FormatComparator
            // 
            CB_FormatComparator.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_FormatComparator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_FormatComparator.FormattingEnabled = true;
            CB_FormatComparator.Items.AddRange(new object[] { "Any", "==", ">=", "<=" });
            CB_FormatComparator.Location = new System.Drawing.Point(0, 0);
            CB_FormatComparator.Margin = new System.Windows.Forms.Padding(0);
            CB_FormatComparator.Name = "CB_FormatComparator";
            CB_FormatComparator.Size = new System.Drawing.Size(62, 23);
            CB_FormatComparator.TabIndex = 122;
            CB_FormatComparator.SelectedIndexChanged += ChangeFormatFilter;
            // 
            // CB_Format
            // 
            CB_Format.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_Format.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Format.FormattingEnabled = true;
            CB_Format.Items.AddRange(new object[] { "Any", ".pk9", ".pk8", ".pk7", ".pk6", ".pk5", ".pk4", ".pk3", ".pk2", ".pk1" });
            CB_Format.Location = new System.Drawing.Point(62, 0);
            CB_Format.Margin = new System.Windows.Forms.Padding(0);
            CB_Format.Name = "CB_Format";
            CB_Format.Size = new System.Drawing.Size(79, 23);
            CB_Format.TabIndex = 121;
            CB_Format.Visible = false;
            // 
            // L_Format
            // 
            L_Format.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Format.AutoSize = true;
            L_Format.Location = new System.Drawing.Point(41, 349);
            L_Format.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            L_Format.Name = "L_Format";
            L_Format.Size = new System.Drawing.Size(48, 15);
            L_Format.TabIndex = 122;
            L_Format.Text = "Format:";
            L_Format.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FLP_Level
            // 
            FLP_Level.Anchor = System.Windows.Forms.AnchorStyles.Left;
            FLP_Level.AutoSize = true;
            FLP_Level.Controls.Add(TB_Level);
            FLP_Level.Controls.Add(CB_Level);
            FLP_Level.Location = new System.Drawing.Point(93, 115);
            FLP_Level.Margin = new System.Windows.Forms.Padding(0);
            FLP_Level.Name = "FLP_Level";
            FLP_Level.Size = new System.Drawing.Size(101, 23);
            FLP_Level.TabIndex = 119;
            // 
            // mnu
            // 
            mnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuView, mnuDelete });
            mnu.Name = "mnu";
            mnu.Size = new System.Drawing.Size(108, 48);
            // 
            // mnuView
            // 
            mnuView.Image = Properties.Resources.other;
            mnuView.Name = "mnuView";
            mnuView.Size = new System.Drawing.Size(107, 22);
            mnuView.Text = "View";
            mnuView.Click += ClickView;
            // 
            // mnuDelete
            // 
            mnuDelete.Image = Properties.Resources.nocheck;
            mnuDelete.Name = "mnuDelete";
            mnuDelete.Size = new System.Drawing.Size(107, 22);
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
            TC_SearchSettings.SelectedIndex = 0;
            TC_SearchSettings.Size = new System.Drawing.Size(308, 448);
            TC_SearchSettings.TabIndex = 120;
            // 
            // Tab_General
            // 
            Tab_General.Controls.Add(TLP_Filters);
            Tab_General.Location = new System.Drawing.Point(4, 24);
            Tab_General.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_General.Name = "Tab_General";
            Tab_General.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_General.Size = new System.Drawing.Size(300, 420);
            Tab_General.TabIndex = 0;
            Tab_General.Text = "General";
            Tab_General.UseVisualStyleBackColor = true;
            // 
            // Tab_Advanced
            // 
            Tab_Advanced.Controls.Add(B_Add);
            Tab_Advanced.Controls.Add(RTB_Instructions);
            Tab_Advanced.Location = new System.Drawing.Point(4, 24);
            Tab_Advanced.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_Advanced.Name = "Tab_Advanced";
            Tab_Advanced.Size = new System.Drawing.Size(300, 420);
            Tab_Advanced.TabIndex = 1;
            Tab_Advanced.Text = "Advanced";
            Tab_Advanced.UseVisualStyleBackColor = true;
            // 
            // B_Add
            // 
            B_Add.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            B_Add.Location = new System.Drawing.Point(229, -1);
            B_Add.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Add.Name = "B_Add";
            B_Add.Size = new System.Drawing.Size(66, 27);
            B_Add.TabIndex = 122;
            B_Add.Text = "Add";
            B_Add.UseVisualStyleBackColor = true;
            B_Add.Click += B_Add_Click;
            // 
            // RTB_Instructions
            // 
            RTB_Instructions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            RTB_Instructions.Location = new System.Drawing.Point(0, 48);
            RTB_Instructions.Margin = new System.Windows.Forms.Padding(0);
            RTB_Instructions.Name = "RTB_Instructions";
            RTB_Instructions.Size = new System.Drawing.Size(298, 369);
            RTB_Instructions.TabIndex = 120;
            RTB_Instructions.Text = "";
            // 
            // SAV_Database
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(670, 512);
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
            FLP_Egg.ResumeLayout(false);
            FLP_Egg.PerformLayout();
            TLP_Filters.ResumeLayout(false);
            TLP_Filters.PerformLayout();
            FLP_Format.ResumeLayout(false);
            FLP_Level.ResumeLayout(false);
            FLP_Level.PerformLayout();
            mnu.ResumeLayout(false);
            TC_SearchSettings.ResumeLayout(false);
            Tab_General.ResumeLayout(false);
            Tab_General.PerformLayout();
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
        private System.Windows.Forms.ComboBox CB_Ability;
        private System.Windows.Forms.ComboBox CB_HeldItem;
        private System.Windows.Forms.ComboBox CB_Nature;
        public System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.ComboBox CB_Move4;
        private System.Windows.Forms.ComboBox CB_Move3;
        private System.Windows.Forms.ComboBox CB_Move2;
        private System.Windows.Forms.ComboBox CB_Move1;
        private System.Windows.Forms.MaskedTextBox TB_Level;
        private System.Windows.Forms.Label Label_CurLevel;
        private System.Windows.Forms.Label Label_HeldItem;
        private System.Windows.Forms.Label Label_Ability;
        private System.Windows.Forms.Label Label_Nature;
        private System.Windows.Forms.Label Label_Species;
        private System.Windows.Forms.ComboBox CB_EVTrain;
        private System.Windows.Forms.ComboBox CB_HPType;
        private System.Windows.Forms.Label Label_HiddenPowerPrefix;
        private System.Windows.Forms.ComboBox CB_GameOrigin;
        private System.Windows.Forms.ComboBox CB_IV;
        private System.Windows.Forms.Button B_Search;
        private System.Windows.Forms.ComboBox CB_Level;
        private System.Windows.Forms.Label L_Version;
        private System.Windows.Forms.Label L_Move1;
        private System.Windows.Forms.Label L_Move2;
        private System.Windows.Forms.Label L_Move3;
        private System.Windows.Forms.Label L_Move4;
        private System.Windows.Forms.Label L_Potential;
        private System.Windows.Forms.Label L_EVTraining;
        private System.Windows.Forms.Button B_Reset;
        private System.Windows.Forms.Label L_Count;
        private System.Windows.Forms.Label L_Generation;
        private System.Windows.Forms.ComboBox CB_Generation;
        private System.Windows.Forms.Label L_Viewed;
        private System.Windows.Forms.ToolStripMenuItem Menu_Export;
        private System.Windows.Forms.ToolStripMenuItem Menu_SearchSettings;
        private System.Windows.Forms.ToolStripMenuItem Menu_SearchBoxes;
        private System.Windows.Forms.ToolStripMenuItem Menu_SearchDatabase;
        private System.Windows.Forms.ToolStripMenuItem Menu_SearchLegal;
        private System.Windows.Forms.ToolStripMenuItem Menu_SearchIllegal;
        private System.Windows.Forms.TableLayoutPanel TLP_Filters;
        public System.Windows.Forms.CheckBox CHK_Shiny;
        private System.Windows.Forms.MaskedTextBox MT_ESV;
        private System.Windows.Forms.Label L_ESV;
        public System.Windows.Forms.CheckBox CHK_IsEgg;
        private System.Windows.Forms.FlowLayoutPanel FLP_Egg;
        private System.Windows.Forms.FlowLayoutPanel FLP_Level;
        private System.Windows.Forms.Label L_Format;
        private System.Windows.Forms.FlowLayoutPanel FLP_Format;
        private System.Windows.Forms.ComboBox CB_FormatComparator;
        private System.Windows.Forms.ComboBox CB_Format;
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
