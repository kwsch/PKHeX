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
            this.components = new System.ComponentModel.Container();
            this.SCR_Box = new System.Windows.Forms.VScrollBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.Menu_Close = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Tools = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_SearchSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_SearchBoxes = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_SearchDatabase = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_SearchLegal = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_SearchIllegal = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_SearchClones = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_OpenDB = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Report = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Export = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Import = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_DeleteClones = new System.Windows.Forms.ToolStripMenuItem();
            this.P_Results = new System.Windows.Forms.Panel();
            this.DatabasePokeGrid = new PKHeX.WinForms.Controls.PokeGrid();
            this.CB_Ability = new System.Windows.Forms.ComboBox();
            this.CB_HeldItem = new System.Windows.Forms.ComboBox();
            this.CB_Nature = new System.Windows.Forms.ComboBox();
            this.CB_Species = new System.Windows.Forms.ComboBox();
            this.CB_Move4 = new System.Windows.Forms.ComboBox();
            this.CB_Move3 = new System.Windows.Forms.ComboBox();
            this.CB_Move2 = new System.Windows.Forms.ComboBox();
            this.CB_Move1 = new System.Windows.Forms.ComboBox();
            this.TB_Level = new System.Windows.Forms.MaskedTextBox();
            this.Label_CurLevel = new System.Windows.Forms.Label();
            this.Label_HeldItem = new System.Windows.Forms.Label();
            this.Label_Ability = new System.Windows.Forms.Label();
            this.Label_Nature = new System.Windows.Forms.Label();
            this.Label_Species = new System.Windows.Forms.Label();
            this.CB_EVTrain = new System.Windows.Forms.ComboBox();
            this.CB_HPType = new System.Windows.Forms.ComboBox();
            this.Label_HiddenPowerPrefix = new System.Windows.Forms.Label();
            this.CB_GameOrigin = new System.Windows.Forms.ComboBox();
            this.CB_IV = new System.Windows.Forms.ComboBox();
            this.B_Search = new System.Windows.Forms.Button();
            this.CB_Level = new System.Windows.Forms.ComboBox();
            this.L_Version = new System.Windows.Forms.Label();
            this.L_Move1 = new System.Windows.Forms.Label();
            this.L_Move2 = new System.Windows.Forms.Label();
            this.L_Move3 = new System.Windows.Forms.Label();
            this.L_Move4 = new System.Windows.Forms.Label();
            this.L_Potential = new System.Windows.Forms.Label();
            this.L_EVTraining = new System.Windows.Forms.Label();
            this.B_Reset = new System.Windows.Forms.Button();
            this.L_Count = new System.Windows.Forms.Label();
            this.L_Generation = new System.Windows.Forms.Label();
            this.CB_Generation = new System.Windows.Forms.ComboBox();
            this.L_Viewed = new System.Windows.Forms.Label();
            this.FLP_Egg = new System.Windows.Forms.FlowLayoutPanel();
            this.CHK_IsEgg = new System.Windows.Forms.CheckBox();
            this.L_ESV = new System.Windows.Forms.Label();
            this.MT_ESV = new System.Windows.Forms.MaskedTextBox();
            this.CHK_Shiny = new System.Windows.Forms.CheckBox();
            this.TLP_Filters = new System.Windows.Forms.TableLayoutPanel();
            this.FLP_Format = new System.Windows.Forms.FlowLayoutPanel();
            this.CB_FormatComparator = new System.Windows.Forms.ComboBox();
            this.CB_Format = new System.Windows.Forms.ComboBox();
            this.L_Format = new System.Windows.Forms.Label();
            this.FLP_Level = new System.Windows.Forms.FlowLayoutPanel();
            this.mnu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.hover = new System.Windows.Forms.ToolTip(this.components);
            this.TC_SearchSettings = new System.Windows.Forms.TabControl();
            this.Tab_General = new System.Windows.Forms.TabPage();
            this.Tab_Advanced = new System.Windows.Forms.TabPage();
            this.RTB_Instructions = new System.Windows.Forms.RichTextBox();
            this.menuStrip1.SuspendLayout();
            this.P_Results.SuspendLayout();
            this.FLP_Egg.SuspendLayout();
            this.TLP_Filters.SuspendLayout();
            this.FLP_Format.SuspendLayout();
            this.FLP_Level.SuspendLayout();
            this.mnu.SuspendLayout();
            this.TC_SearchSettings.SuspendLayout();
            this.Tab_General.SuspendLayout();
            this.Tab_Advanced.SuspendLayout();
            this.SuspendLayout();
            // 
            // SCR_Box
            // 
            this.SCR_Box.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SCR_Box.LargeChange = 1;
            this.SCR_Box.Location = new System.Drawing.Point(256, 3);
            this.SCR_Box.Name = "SCR_Box";
            this.SCR_Box.Size = new System.Drawing.Size(24, 344);
            this.SCR_Box.TabIndex = 1;
            this.SCR_Box.Scroll += new System.Windows.Forms.ScrollEventHandler(this.UpdateScroll);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_Close,
            this.Menu_Tools});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(574, 24);
            this.menuStrip1.TabIndex = 65;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // Menu_Close
            // 
            this.Menu_Close.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_Exit});
            this.Menu_Close.Name = "Menu_Close";
            this.Menu_Close.Size = new System.Drawing.Size(37, 20);
            this.Menu_Close.Text = "File";
            // 
            // Menu_Exit
            // 
            this.Menu_Exit.Image = global::PKHeX.WinForms.Properties.Resources.exit;
            this.Menu_Exit.Name = "Menu_Exit";
            this.Menu_Exit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.Menu_Exit.ShowShortcutKeys = false;
            this.Menu_Exit.Size = new System.Drawing.Size(96, 22);
            this.Menu_Exit.Text = "&Close";
            this.Menu_Exit.Click += new System.EventHandler(this.Menu_Exit_Click);
            // 
            // Menu_Tools
            // 
            this.Menu_Tools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_SearchSettings,
            this.Menu_OpenDB,
            this.Menu_Report,
            this.Menu_Export,
            this.Menu_Import,
            this.Menu_DeleteClones});
            this.Menu_Tools.Name = "Menu_Tools";
            this.Menu_Tools.Size = new System.Drawing.Size(47, 20);
            this.Menu_Tools.Text = "Tools";
            // 
            // Menu_SearchSettings
            // 
            this.Menu_SearchSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_SearchBoxes,
            this.Menu_SearchDatabase,
            this.Menu_SearchLegal,
            this.Menu_SearchIllegal,
            this.Menu_SearchClones});
            this.Menu_SearchSettings.Image = global::PKHeX.WinForms.Properties.Resources.settings;
            this.Menu_SearchSettings.Name = "Menu_SearchSettings";
            this.Menu_SearchSettings.Size = new System.Drawing.Size(209, 22);
            this.Menu_SearchSettings.Text = "Search Settings";
            // 
            // Menu_SearchBoxes
            // 
            this.Menu_SearchBoxes.Checked = true;
            this.Menu_SearchBoxes.CheckOnClick = true;
            this.Menu_SearchBoxes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Menu_SearchBoxes.Name = "Menu_SearchBoxes";
            this.Menu_SearchBoxes.Size = new System.Drawing.Size(198, 22);
            this.Menu_SearchBoxes.Text = "Search Within Boxes";
            // 
            // Menu_SearchDatabase
            // 
            this.Menu_SearchDatabase.Checked = true;
            this.Menu_SearchDatabase.CheckOnClick = true;
            this.Menu_SearchDatabase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Menu_SearchDatabase.Name = "Menu_SearchDatabase";
            this.Menu_SearchDatabase.Size = new System.Drawing.Size(198, 22);
            this.Menu_SearchDatabase.Text = "Search Within Database";
            // 
            // Menu_SearchLegal
            // 
            this.Menu_SearchLegal.Checked = true;
            this.Menu_SearchLegal.CheckOnClick = true;
            this.Menu_SearchLegal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Menu_SearchLegal.Name = "Menu_SearchLegal";
            this.Menu_SearchLegal.Size = new System.Drawing.Size(198, 22);
            this.Menu_SearchLegal.Text = "Show Legal";
            // 
            // Menu_SearchIllegal
            // 
            this.Menu_SearchIllegal.Checked = true;
            this.Menu_SearchIllegal.CheckOnClick = true;
            this.Menu_SearchIllegal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Menu_SearchIllegal.Name = "Menu_SearchIllegal";
            this.Menu_SearchIllegal.Size = new System.Drawing.Size(198, 22);
            this.Menu_SearchIllegal.Text = "Show Illegal";
            // 
            // Menu_SearchClones
            // 
            this.Menu_SearchClones.CheckOnClick = true;
            this.Menu_SearchClones.Name = "Menu_SearchClones";
            this.Menu_SearchClones.Size = new System.Drawing.Size(198, 22);
            this.Menu_SearchClones.Text = "Clones Only";
            // 
            // Menu_OpenDB
            // 
            this.Menu_OpenDB.Image = global::PKHeX.WinForms.Properties.Resources.folder;
            this.Menu_OpenDB.Name = "Menu_OpenDB";
            this.Menu_OpenDB.Size = new System.Drawing.Size(209, 22);
            this.Menu_OpenDB.Text = "Open Database Folder";
            this.Menu_OpenDB.Click += new System.EventHandler(this.OpenDB);
            // 
            // Menu_Report
            // 
            this.Menu_Report.Image = global::PKHeX.WinForms.Properties.Resources.report;
            this.Menu_Report.Name = "Menu_Report";
            this.Menu_Report.Size = new System.Drawing.Size(209, 22);
            this.Menu_Report.Text = "Create Data Report";
            this.Menu_Report.Click += new System.EventHandler(this.GenerateDBReport);
            // 
            // Menu_Export
            // 
            this.Menu_Export.Image = global::PKHeX.WinForms.Properties.Resources.export;
            this.Menu_Export.Name = "Menu_Export";
            this.Menu_Export.Size = new System.Drawing.Size(209, 22);
            this.Menu_Export.Text = "Export Results to Folder";
            this.Menu_Export.Click += new System.EventHandler(this.Menu_Export_Click);
            // 
            // Menu_Import
            // 
            this.Menu_Import.Image = global::PKHeX.WinForms.Properties.Resources.savePKM;
            this.Menu_Import.Name = "Menu_Import";
            this.Menu_Import.Size = new System.Drawing.Size(209, 22);
            this.Menu_Import.Text = "Import Results to SaveFile";
            this.Menu_Import.Click += new System.EventHandler(this.Menu_Import_Click);
            // 
            // Menu_DeleteClones
            // 
            this.Menu_DeleteClones.Image = global::PKHeX.WinForms.Properties.Resources.nocheck;
            this.Menu_DeleteClones.Name = "Menu_DeleteClones";
            this.Menu_DeleteClones.Size = new System.Drawing.Size(209, 22);
            this.Menu_DeleteClones.Text = "Delete Clones";
            this.Menu_DeleteClones.Click += new System.EventHandler(this.Menu_DeleteClones_Click);
            // 
            // P_Results
            // 
            this.P_Results.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.P_Results.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.P_Results.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.P_Results.Controls.Add(this.DatabasePokeGrid);
            this.P_Results.Controls.Add(this.SCR_Box);
            this.P_Results.Location = new System.Drawing.Point(12, 32);
            this.P_Results.Name = "P_Results";
            this.P_Results.Size = new System.Drawing.Size(285, 352);
            this.P_Results.TabIndex = 66;
            // 
            // DatabasePokeGrid
            // 
            this.DatabasePokeGrid.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.DatabasePokeGrid.Location = new System.Drawing.Point(2, 2);
            this.DatabasePokeGrid.Margin = new System.Windows.Forms.Padding(0);
            this.DatabasePokeGrid.Name = "DatabasePokeGrid";
            this.DatabasePokeGrid.Size = new System.Drawing.Size(251, 346);
            this.DatabasePokeGrid.TabIndex = 2;
            // 
            // CB_Ability
            // 
            this.CB_Ability.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CB_Ability.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_Ability.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Ability.FormattingEnabled = true;
            this.CB_Ability.Items.AddRange(new object[] {
            "Item"});
            this.CB_Ability.Location = new System.Drawing.Point(83, 83);
            this.CB_Ability.Margin = new System.Windows.Forms.Padding(0);
            this.CB_Ability.Name = "CB_Ability";
            this.CB_Ability.Size = new System.Drawing.Size(122, 21);
            this.CB_Ability.TabIndex = 70;
            // 
            // CB_HeldItem
            // 
            this.CB_HeldItem.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CB_HeldItem.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_HeldItem.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_HeldItem.FormattingEnabled = true;
            this.CB_HeldItem.Location = new System.Drawing.Point(83, 62);
            this.CB_HeldItem.Margin = new System.Windows.Forms.Padding(0);
            this.CB_HeldItem.Name = "CB_HeldItem";
            this.CB_HeldItem.Size = new System.Drawing.Size(122, 21);
            this.CB_HeldItem.TabIndex = 69;
            // 
            // CB_Nature
            // 
            this.CB_Nature.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CB_Nature.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_Nature.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Nature.FormattingEnabled = true;
            this.CB_Nature.Location = new System.Drawing.Point(83, 41);
            this.CB_Nature.Margin = new System.Windows.Forms.Padding(0);
            this.CB_Nature.Name = "CB_Nature";
            this.CB_Nature.Size = new System.Drawing.Size(122, 21);
            this.CB_Nature.TabIndex = 68;
            // 
            // CB_Species
            // 
            this.CB_Species.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CB_Species.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_Species.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Species.FormattingEnabled = true;
            this.CB_Species.Location = new System.Drawing.Point(83, 20);
            this.CB_Species.Margin = new System.Windows.Forms.Padding(0);
            this.CB_Species.Name = "CB_Species";
            this.CB_Species.Size = new System.Drawing.Size(122, 21);
            this.CB_Species.TabIndex = 67;
            // 
            // CB_Move4
            // 
            this.CB_Move4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CB_Move4.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_Move4.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Move4.FormattingEnabled = true;
            this.CB_Move4.Location = new System.Drawing.Point(83, 251);
            this.CB_Move4.Margin = new System.Windows.Forms.Padding(0);
            this.CB_Move4.Name = "CB_Move4";
            this.CB_Move4.Size = new System.Drawing.Size(122, 21);
            this.CB_Move4.TabIndex = 74;
            // 
            // CB_Move3
            // 
            this.CB_Move3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CB_Move3.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_Move3.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Move3.FormattingEnabled = true;
            this.CB_Move3.Location = new System.Drawing.Point(83, 230);
            this.CB_Move3.Margin = new System.Windows.Forms.Padding(0);
            this.CB_Move3.Name = "CB_Move3";
            this.CB_Move3.Size = new System.Drawing.Size(122, 21);
            this.CB_Move3.TabIndex = 73;
            // 
            // CB_Move2
            // 
            this.CB_Move2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CB_Move2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_Move2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Move2.FormattingEnabled = true;
            this.CB_Move2.Location = new System.Drawing.Point(83, 209);
            this.CB_Move2.Margin = new System.Windows.Forms.Padding(0);
            this.CB_Move2.Name = "CB_Move2";
            this.CB_Move2.Size = new System.Drawing.Size(122, 21);
            this.CB_Move2.TabIndex = 72;
            // 
            // CB_Move1
            // 
            this.CB_Move1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CB_Move1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_Move1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Move1.FormattingEnabled = true;
            this.CB_Move1.Location = new System.Drawing.Point(83, 188);
            this.CB_Move1.Margin = new System.Windows.Forms.Padding(0);
            this.CB_Move1.Name = "CB_Move1";
            this.CB_Move1.Size = new System.Drawing.Size(122, 21);
            this.CB_Move1.TabIndex = 71;
            // 
            // TB_Level
            // 
            this.TB_Level.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.TB_Level.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_Level.Location = new System.Drawing.Point(0, 0);
            this.TB_Level.Margin = new System.Windows.Forms.Padding(0);
            this.TB_Level.Mask = "000";
            this.TB_Level.Name = "TB_Level";
            this.TB_Level.Size = new System.Drawing.Size(22, 20);
            this.TB_Level.TabIndex = 89;
            this.TB_Level.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Label_CurLevel
            // 
            this.Label_CurLevel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Label_CurLevel.AutoSize = true;
            this.Label_CurLevel.Location = new System.Drawing.Point(44, 108);
            this.Label_CurLevel.Margin = new System.Windows.Forms.Padding(3);
            this.Label_CurLevel.Name = "Label_CurLevel";
            this.Label_CurLevel.Size = new System.Drawing.Size(36, 13);
            this.Label_CurLevel.TabIndex = 88;
            this.Label_CurLevel.Text = "Level:";
            this.Label_CurLevel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_HeldItem
            // 
            this.Label_HeldItem.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Label_HeldItem.AutoSize = true;
            this.Label_HeldItem.Location = new System.Drawing.Point(25, 66);
            this.Label_HeldItem.Margin = new System.Windows.Forms.Padding(3);
            this.Label_HeldItem.Name = "Label_HeldItem";
            this.Label_HeldItem.Size = new System.Drawing.Size(55, 13);
            this.Label_HeldItem.TabIndex = 93;
            this.Label_HeldItem.Text = "Held Item:";
            this.Label_HeldItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_Ability
            // 
            this.Label_Ability.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Label_Ability.AutoSize = true;
            this.Label_Ability.Location = new System.Drawing.Point(43, 87);
            this.Label_Ability.Margin = new System.Windows.Forms.Padding(3);
            this.Label_Ability.Name = "Label_Ability";
            this.Label_Ability.Size = new System.Drawing.Size(37, 13);
            this.Label_Ability.TabIndex = 92;
            this.Label_Ability.Text = "Ability:";
            this.Label_Ability.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_Nature
            // 
            this.Label_Nature.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Label_Nature.AutoSize = true;
            this.Label_Nature.Location = new System.Drawing.Point(38, 45);
            this.Label_Nature.Margin = new System.Windows.Forms.Padding(3);
            this.Label_Nature.Name = "Label_Nature";
            this.Label_Nature.Size = new System.Drawing.Size(42, 13);
            this.Label_Nature.TabIndex = 91;
            this.Label_Nature.Text = "Nature:";
            this.Label_Nature.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_Species
            // 
            this.Label_Species.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Label_Species.AutoSize = true;
            this.Label_Species.Location = new System.Drawing.Point(32, 24);
            this.Label_Species.Margin = new System.Windows.Forms.Padding(3);
            this.Label_Species.Name = "Label_Species";
            this.Label_Species.Size = new System.Drawing.Size(48, 13);
            this.Label_Species.TabIndex = 90;
            this.Label_Species.Text = "Species:";
            this.Label_Species.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_EVTrain
            // 
            this.CB_EVTrain.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CB_EVTrain.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_EVTrain.DropDownWidth = 85;
            this.CB_EVTrain.FormattingEnabled = true;
            this.CB_EVTrain.Items.AddRange(new object[] {
            "Any",
            "None (0)",
            "Some (127-0)",
            "Half (128-507)",
            "Full (508+)"});
            this.CB_EVTrain.Location = new System.Drawing.Point(83, 146);
            this.CB_EVTrain.Margin = new System.Windows.Forms.Padding(0);
            this.CB_EVTrain.Name = "CB_EVTrain";
            this.CB_EVTrain.Size = new System.Drawing.Size(94, 21);
            this.CB_EVTrain.TabIndex = 94;
            // 
            // CB_HPType
            // 
            this.CB_HPType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CB_HPType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_HPType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_HPType.DropDownWidth = 80;
            this.CB_HPType.FormattingEnabled = true;
            this.CB_HPType.Location = new System.Drawing.Point(83, 167);
            this.CB_HPType.Margin = new System.Windows.Forms.Padding(0);
            this.CB_HPType.Name = "CB_HPType";
            this.CB_HPType.Size = new System.Drawing.Size(122, 21);
            this.CB_HPType.TabIndex = 96;
            // 
            // Label_HiddenPowerPrefix
            // 
            this.Label_HiddenPowerPrefix.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Label_HiddenPowerPrefix.AutoSize = true;
            this.Label_HiddenPowerPrefix.Location = new System.Drawing.Point(3, 171);
            this.Label_HiddenPowerPrefix.Margin = new System.Windows.Forms.Padding(3);
            this.Label_HiddenPowerPrefix.Name = "Label_HiddenPowerPrefix";
            this.Label_HiddenPowerPrefix.Size = new System.Drawing.Size(77, 13);
            this.Label_HiddenPowerPrefix.TabIndex = 95;
            this.Label_HiddenPowerPrefix.Text = "Hidden Power:";
            this.Label_HiddenPowerPrefix.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_GameOrigin
            // 
            this.CB_GameOrigin.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CB_GameOrigin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_GameOrigin.FormattingEnabled = true;
            this.CB_GameOrigin.Location = new System.Drawing.Point(83, 272);
            this.CB_GameOrigin.Margin = new System.Windows.Forms.Padding(0);
            this.CB_GameOrigin.Name = "CB_GameOrigin";
            this.CB_GameOrigin.Size = new System.Drawing.Size(122, 21);
            this.CB_GameOrigin.TabIndex = 97;
            this.CB_GameOrigin.SelectedIndexChanged += new System.EventHandler(this.ChangeGame);
            // 
            // CB_IV
            // 
            this.CB_IV.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CB_IV.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_IV.DropDownWidth = 85;
            this.CB_IV.FormattingEnabled = true;
            this.CB_IV.Items.AddRange(new object[] {
            "Any",
            "<= 90",
            "91-120",
            "121-150",
            "151-179",
            "180+",
            "== 186"});
            this.CB_IV.Location = new System.Drawing.Point(83, 125);
            this.CB_IV.Margin = new System.Windows.Forms.Padding(0);
            this.CB_IV.Name = "CB_IV";
            this.CB_IV.Size = new System.Drawing.Size(94, 21);
            this.CB_IV.TabIndex = 100;
            // 
            // B_Search
            // 
            this.B_Search.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Search.Location = new System.Drawing.Point(333, 402);
            this.B_Search.Name = "B_Search";
            this.B_Search.Size = new System.Drawing.Size(206, 30);
            this.B_Search.TabIndex = 102;
            this.B_Search.Text = "Search!";
            this.B_Search.UseVisualStyleBackColor = true;
            this.B_Search.Click += new System.EventHandler(this.B_Search_Click);
            // 
            // CB_Level
            // 
            this.CB_Level.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CB_Level.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Level.DropDownWidth = 85;
            this.CB_Level.FormattingEnabled = true;
            this.CB_Level.Items.AddRange(new object[] {
            "Any",
            "==",
            ">=",
            "<="});
            this.CB_Level.Location = new System.Drawing.Point(22, 0);
            this.CB_Level.Margin = new System.Windows.Forms.Padding(0);
            this.CB_Level.Name = "CB_Level";
            this.CB_Level.Size = new System.Drawing.Size(66, 21);
            this.CB_Level.TabIndex = 103;
            this.CB_Level.SelectedIndexChanged += new System.EventHandler(this.ChangeLevel);
            // 
            // L_Version
            // 
            this.L_Version.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.L_Version.AutoSize = true;
            this.L_Version.Location = new System.Drawing.Point(17, 276);
            this.L_Version.Margin = new System.Windows.Forms.Padding(3);
            this.L_Version.Name = "L_Version";
            this.L_Version.Size = new System.Drawing.Size(63, 13);
            this.L_Version.TabIndex = 104;
            this.L_Version.Text = "OT Version:";
            this.L_Version.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Move1
            // 
            this.L_Move1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.L_Move1.AutoSize = true;
            this.L_Move1.Location = new System.Drawing.Point(34, 192);
            this.L_Move1.Margin = new System.Windows.Forms.Padding(3);
            this.L_Move1.Name = "L_Move1";
            this.L_Move1.Size = new System.Drawing.Size(46, 13);
            this.L_Move1.TabIndex = 105;
            this.L_Move1.Text = "Move 1:";
            this.L_Move1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Move2
            // 
            this.L_Move2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.L_Move2.AutoSize = true;
            this.L_Move2.Location = new System.Drawing.Point(34, 213);
            this.L_Move2.Margin = new System.Windows.Forms.Padding(3);
            this.L_Move2.Name = "L_Move2";
            this.L_Move2.Size = new System.Drawing.Size(46, 13);
            this.L_Move2.TabIndex = 106;
            this.L_Move2.Text = "Move 2:";
            this.L_Move2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Move3
            // 
            this.L_Move3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.L_Move3.AutoSize = true;
            this.L_Move3.Location = new System.Drawing.Point(34, 234);
            this.L_Move3.Margin = new System.Windows.Forms.Padding(3);
            this.L_Move3.Name = "L_Move3";
            this.L_Move3.Size = new System.Drawing.Size(46, 13);
            this.L_Move3.TabIndex = 107;
            this.L_Move3.Text = "Move 3:";
            this.L_Move3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Move4
            // 
            this.L_Move4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.L_Move4.AutoSize = true;
            this.L_Move4.Location = new System.Drawing.Point(34, 255);
            this.L_Move4.Margin = new System.Windows.Forms.Padding(3);
            this.L_Move4.Name = "L_Move4";
            this.L_Move4.Size = new System.Drawing.Size(46, 13);
            this.L_Move4.TabIndex = 108;
            this.L_Move4.Text = "Move 4:";
            this.L_Move4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Potential
            // 
            this.L_Potential.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.L_Potential.AutoSize = true;
            this.L_Potential.Location = new System.Drawing.Point(16, 129);
            this.L_Potential.Margin = new System.Windows.Forms.Padding(3);
            this.L_Potential.Name = "L_Potential";
            this.L_Potential.Size = new System.Drawing.Size(64, 13);
            this.L_Potential.TabIndex = 109;
            this.L_Potential.Text = "IV Potential:";
            this.L_Potential.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_EVTraining
            // 
            this.L_EVTraining.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.L_EVTraining.AutoSize = true;
            this.L_EVTraining.Location = new System.Drawing.Point(15, 150);
            this.L_EVTraining.Margin = new System.Windows.Forms.Padding(3);
            this.L_EVTraining.Name = "L_EVTraining";
            this.L_EVTraining.Size = new System.Drawing.Size(65, 13);
            this.L_EVTraining.TabIndex = 110;
            this.L_EVTraining.Text = "EV Training:";
            this.L_EVTraining.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // B_Reset
            // 
            this.B_Reset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Reset.Location = new System.Drawing.Point(499, 0);
            this.B_Reset.Name = "B_Reset";
            this.B_Reset.Size = new System.Drawing.Size(75, 23);
            this.B_Reset.TabIndex = 111;
            this.B_Reset.Text = "Reset Filters";
            this.B_Reset.UseVisualStyleBackColor = true;
            this.B_Reset.Click += new System.EventHandler(this.ResetFilters);
            // 
            // L_Count
            // 
            this.L_Count.Location = new System.Drawing.Point(99, 18);
            this.L_Count.Name = "L_Count";
            this.L_Count.Size = new System.Drawing.Size(83, 13);
            this.L_Count.TabIndex = 114;
            this.L_Count.Text = "Count: {0}";
            this.L_Count.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // L_Generation
            // 
            this.L_Generation.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.L_Generation.AutoSize = true;
            this.L_Generation.Location = new System.Drawing.Point(18, 297);
            this.L_Generation.Margin = new System.Windows.Forms.Padding(3);
            this.L_Generation.Name = "L_Generation";
            this.L_Generation.Size = new System.Drawing.Size(62, 13);
            this.L_Generation.TabIndex = 116;
            this.L_Generation.Text = "Generation:";
            this.L_Generation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_Generation
            // 
            this.CB_Generation.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CB_Generation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Generation.FormattingEnabled = true;
            this.CB_Generation.Items.AddRange(new object[] {
            "Any",
            "Gen 1 (RBY/GSC)",
            "Gen 2 (RBY/GSC)",
            "Gen 3 (RSE/FRLG/CXD)",
            "Gen 4 (DPPt/HGSS)",
            "Gen 5 (BW/B2W2)",
            "Gen 6 (XY/ORAS)",
            "Gen 7 (SM/USUM)"});
            this.CB_Generation.Location = new System.Drawing.Point(83, 293);
            this.CB_Generation.Margin = new System.Windows.Forms.Padding(0);
            this.CB_Generation.Name = "CB_Generation";
            this.CB_Generation.Size = new System.Drawing.Size(122, 21);
            this.CB_Generation.TabIndex = 115;
            this.CB_Generation.SelectedIndexChanged += new System.EventHandler(this.ChangeGeneration);
            // 
            // L_Viewed
            // 
            this.L_Viewed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.L_Viewed.AutoSize = true;
            this.L_Viewed.Location = new System.Drawing.Point(9, 429);
            this.L_Viewed.Name = "L_Viewed";
            this.L_Viewed.Size = new System.Drawing.Size(85, 13);
            this.L_Viewed.TabIndex = 117;
            this.L_Viewed.Text = "Last Viewed: {0}";
            this.L_Viewed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.L_Viewed.MouseEnter += new System.EventHandler(this.L_Viewed_MouseEnter);
            // 
            // FLP_Egg
            // 
            this.FLP_Egg.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.FLP_Egg.AutoSize = true;
            this.FLP_Egg.Controls.Add(this.CHK_IsEgg);
            this.FLP_Egg.Controls.Add(this.L_ESV);
            this.FLP_Egg.Controls.Add(this.MT_ESV);
            this.FLP_Egg.Location = new System.Drawing.Point(83, 0);
            this.FLP_Egg.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_Egg.Name = "FLP_Egg";
            this.FLP_Egg.Size = new System.Drawing.Size(119, 20);
            this.FLP_Egg.TabIndex = 120;
            // 
            // CHK_IsEgg
            // 
            this.CHK_IsEgg.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CHK_IsEgg.AutoSize = true;
            this.CHK_IsEgg.Checked = true;
            this.CHK_IsEgg.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.CHK_IsEgg.Location = new System.Drawing.Point(0, 1);
            this.CHK_IsEgg.Margin = new System.Windows.Forms.Padding(0);
            this.CHK_IsEgg.Name = "CHK_IsEgg";
            this.CHK_IsEgg.Size = new System.Drawing.Size(45, 17);
            this.CHK_IsEgg.TabIndex = 98;
            this.CHK_IsEgg.Text = "Egg";
            this.CHK_IsEgg.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CHK_IsEgg.ThreeState = true;
            this.CHK_IsEgg.UseVisualStyleBackColor = true;
            this.CHK_IsEgg.CheckedChanged += new System.EventHandler(this.ToggleESV);
            // 
            // L_ESV
            // 
            this.L_ESV.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.L_ESV.Location = new System.Drawing.Point(45, 1);
            this.L_ESV.Margin = new System.Windows.Forms.Padding(0);
            this.L_ESV.Name = "L_ESV";
            this.L_ESV.Size = new System.Drawing.Size(43, 17);
            this.L_ESV.TabIndex = 113;
            this.L_ESV.Text = "ESV:";
            this.L_ESV.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.L_ESV.Visible = false;
            // 
            // MT_ESV
            // 
            this.MT_ESV.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.MT_ESV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MT_ESV.Location = new System.Drawing.Point(88, 0);
            this.MT_ESV.Margin = new System.Windows.Forms.Padding(0);
            this.MT_ESV.Mask = "0000";
            this.MT_ESV.Name = "MT_ESV";
            this.MT_ESV.Size = new System.Drawing.Size(31, 20);
            this.MT_ESV.TabIndex = 112;
            this.MT_ESV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.MT_ESV.Visible = false;
            // 
            // CHK_Shiny
            // 
            this.CHK_Shiny.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.CHK_Shiny.AutoSize = true;
            this.CHK_Shiny.Checked = true;
            this.CHK_Shiny.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.CHK_Shiny.Location = new System.Drawing.Point(31, 1);
            this.CHK_Shiny.Margin = new System.Windows.Forms.Padding(0);
            this.CHK_Shiny.Name = "CHK_Shiny";
            this.CHK_Shiny.Size = new System.Drawing.Size(52, 17);
            this.CHK_Shiny.TabIndex = 99;
            this.CHK_Shiny.Text = "Shiny";
            this.CHK_Shiny.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CHK_Shiny.ThreeState = true;
            this.CHK_Shiny.UseVisualStyleBackColor = true;
            // 
            // TLP_Filters
            // 
            this.TLP_Filters.AutoScroll = true;
            this.TLP_Filters.AutoScrollMargin = new System.Drawing.Size(3, 3);
            this.TLP_Filters.AutoSize = true;
            this.TLP_Filters.ColumnCount = 2;
            this.TLP_Filters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TLP_Filters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TLP_Filters.Controls.Add(this.FLP_Format, 1, 15);
            this.TLP_Filters.Controls.Add(this.L_Format, 0, 15);
            this.TLP_Filters.Controls.Add(this.FLP_Egg, 1, 0);
            this.TLP_Filters.Controls.Add(this.CHK_Shiny, 0, 0);
            this.TLP_Filters.Controls.Add(this.Label_Species, 0, 1);
            this.TLP_Filters.Controls.Add(this.CB_Species, 1, 1);
            this.TLP_Filters.Controls.Add(this.Label_Nature, 0, 2);
            this.TLP_Filters.Controls.Add(this.CB_Nature, 1, 2);
            this.TLP_Filters.Controls.Add(this.Label_HeldItem, 0, 3);
            this.TLP_Filters.Controls.Add(this.CB_HeldItem, 1, 3);
            this.TLP_Filters.Controls.Add(this.Label_Ability, 0, 4);
            this.TLP_Filters.Controls.Add(this.CB_Ability, 1, 4);
            this.TLP_Filters.Controls.Add(this.FLP_Level, 1, 5);
            this.TLP_Filters.Controls.Add(this.Label_CurLevel, 0, 5);
            this.TLP_Filters.Controls.Add(this.L_Potential, 0, 6);
            this.TLP_Filters.Controls.Add(this.CB_IV, 1, 6);
            this.TLP_Filters.Controls.Add(this.L_EVTraining, 0, 7);
            this.TLP_Filters.Controls.Add(this.CB_EVTrain, 1, 7);
            this.TLP_Filters.Controls.Add(this.Label_HiddenPowerPrefix, 0, 8);
            this.TLP_Filters.Controls.Add(this.CB_HPType, 1, 8);
            this.TLP_Filters.Controls.Add(this.L_Move1, 0, 9);
            this.TLP_Filters.Controls.Add(this.CB_Move1, 1, 9);
            this.TLP_Filters.Controls.Add(this.L_Move2, 0, 10);
            this.TLP_Filters.Controls.Add(this.CB_Move2, 1, 10);
            this.TLP_Filters.Controls.Add(this.L_Move3, 0, 11);
            this.TLP_Filters.Controls.Add(this.CB_Move3, 1, 11);
            this.TLP_Filters.Controls.Add(this.L_Move4, 0, 12);
            this.TLP_Filters.Controls.Add(this.CB_Move4, 1, 12);
            this.TLP_Filters.Controls.Add(this.L_Version, 0, 13);
            this.TLP_Filters.Controls.Add(this.CB_GameOrigin, 1, 13);
            this.TLP_Filters.Controls.Add(this.L_Generation, 0, 14);
            this.TLP_Filters.Controls.Add(this.CB_Generation, 1, 14);
            this.TLP_Filters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLP_Filters.Location = new System.Drawing.Point(3, 3);
            this.TLP_Filters.Name = "TLP_Filters";
            this.TLP_Filters.RowCount = 17;
            this.TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Filters.Size = new System.Drawing.Size(250, 356);
            this.TLP_Filters.TabIndex = 118;
            // 
            // FLP_Format
            // 
            this.FLP_Format.AutoSize = true;
            this.FLP_Format.Controls.Add(this.CB_FormatComparator);
            this.FLP_Format.Controls.Add(this.CB_Format);
            this.FLP_Format.Location = new System.Drawing.Point(83, 314);
            this.FLP_Format.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_Format.Name = "FLP_Format";
            this.FLP_Format.Size = new System.Drawing.Size(122, 21);
            this.FLP_Format.TabIndex = 124;
            // 
            // CB_FormatComparator
            // 
            this.CB_FormatComparator.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CB_FormatComparator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_FormatComparator.FormattingEnabled = true;
            this.CB_FormatComparator.Items.AddRange(new object[] {
            "Any",
            "==",
            ">=",
            "<="});
            this.CB_FormatComparator.Location = new System.Drawing.Point(0, 0);
            this.CB_FormatComparator.Margin = new System.Windows.Forms.Padding(0);
            this.CB_FormatComparator.Name = "CB_FormatComparator";
            this.CB_FormatComparator.Size = new System.Drawing.Size(54, 21);
            this.CB_FormatComparator.TabIndex = 122;
            this.CB_FormatComparator.SelectedIndexChanged += new System.EventHandler(this.ChangeFormatFilter);
            // 
            // CB_Format
            // 
            this.CB_Format.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CB_Format.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Format.FormattingEnabled = true;
            this.CB_Format.Items.AddRange(new object[] {
            "Any",
            ".pk8",
            ".pk7",
            ".pk6",
            ".pk5",
            ".pk4",
            ".pk3",
            ".pk2",
            ".pk1"});
            this.CB_Format.Location = new System.Drawing.Point(54, 0);
            this.CB_Format.Margin = new System.Windows.Forms.Padding(0);
            this.CB_Format.Name = "CB_Format";
            this.CB_Format.Size = new System.Drawing.Size(68, 21);
            this.CB_Format.TabIndex = 121;
            this.CB_Format.Visible = false;
            // 
            // L_Format
            // 
            this.L_Format.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.L_Format.AutoSize = true;
            this.L_Format.Location = new System.Drawing.Point(38, 318);
            this.L_Format.Margin = new System.Windows.Forms.Padding(3);
            this.L_Format.Name = "L_Format";
            this.L_Format.Size = new System.Drawing.Size(42, 13);
            this.L_Format.TabIndex = 122;
            this.L_Format.Text = "Format:";
            this.L_Format.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FLP_Level
            // 
            this.FLP_Level.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.FLP_Level.AutoSize = true;
            this.FLP_Level.Controls.Add(this.TB_Level);
            this.FLP_Level.Controls.Add(this.CB_Level);
            this.FLP_Level.Location = new System.Drawing.Point(83, 104);
            this.FLP_Level.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_Level.Name = "FLP_Level";
            this.FLP_Level.Size = new System.Drawing.Size(88, 21);
            this.FLP_Level.TabIndex = 119;
            // 
            // mnu
            // 
            this.mnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuView,
            this.mnuDelete});
            this.mnu.Name = "mnu";
            this.mnu.Size = new System.Drawing.Size(108, 48);
            this.mnu.Click += new System.EventHandler(this.ClickView);
            // 
            // mnuView
            // 
            this.mnuView.Image = global::PKHeX.WinForms.Properties.Resources.other;
            this.mnuView.Name = "mnuView";
            this.mnuView.Size = new System.Drawing.Size(107, 22);
            this.mnuView.Text = "View";
            // 
            // mnuDelete
            // 
            this.mnuDelete.Image = global::PKHeX.WinForms.Properties.Resources.nocheck;
            this.mnuDelete.Name = "mnuDelete";
            this.mnuDelete.Size = new System.Drawing.Size(107, 22);
            this.mnuDelete.Text = "Delete";
            this.mnuDelete.Click += new System.EventHandler(this.ClickDelete);
            // 
            // TC_SearchSettings
            // 
            this.TC_SearchSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TC_SearchSettings.Controls.Add(this.Tab_General);
            this.TC_SearchSettings.Controls.Add(this.Tab_Advanced);
            this.TC_SearchSettings.Location = new System.Drawing.Point(304, 8);
            this.TC_SearchSettings.Name = "TC_SearchSettings";
            this.TC_SearchSettings.SelectedIndex = 0;
            this.TC_SearchSettings.Size = new System.Drawing.Size(264, 388);
            this.TC_SearchSettings.TabIndex = 120;
            // 
            // Tab_General
            // 
            this.Tab_General.Controls.Add(this.TLP_Filters);
            this.Tab_General.Location = new System.Drawing.Point(4, 22);
            this.Tab_General.Name = "Tab_General";
            this.Tab_General.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_General.Size = new System.Drawing.Size(256, 362);
            this.Tab_General.TabIndex = 0;
            this.Tab_General.Text = "General";
            this.Tab_General.UseVisualStyleBackColor = true;
            // 
            // Tab_Advanced
            // 
            this.Tab_Advanced.Controls.Add(this.RTB_Instructions);
            this.Tab_Advanced.Location = new System.Drawing.Point(4, 22);
            this.Tab_Advanced.Name = "Tab_Advanced";
            this.Tab_Advanced.Size = new System.Drawing.Size(256, 362);
            this.Tab_Advanced.TabIndex = 1;
            this.Tab_Advanced.Text = "Advanced";
            this.Tab_Advanced.UseVisualStyleBackColor = true;
            // 
            // RTB_Instructions
            // 
            this.RTB_Instructions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RTB_Instructions.Location = new System.Drawing.Point(0, 0);
            this.RTB_Instructions.Name = "RTB_Instructions";
            this.RTB_Instructions.Size = new System.Drawing.Size(256, 362);
            this.RTB_Instructions.TabIndex = 120;
            this.RTB_Instructions.Text = "";
            // 
            // SAV_Database
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 444);
            this.Controls.Add(this.B_Reset);
            this.Controls.Add(this.TC_SearchSettings);
            this.Controls.Add(this.B_Search);
            this.Controls.Add(this.L_Viewed);
            this.Controls.Add(this.L_Count);
            this.Controls.Add(this.P_Results);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SAV_Database";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Database";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.P_Results.ResumeLayout(false);
            this.FLP_Egg.ResumeLayout(false);
            this.FLP_Egg.PerformLayout();
            this.TLP_Filters.ResumeLayout(false);
            this.TLP_Filters.PerformLayout();
            this.FLP_Format.ResumeLayout(false);
            this.FLP_Level.ResumeLayout(false);
            this.FLP_Level.PerformLayout();
            this.mnu.ResumeLayout(false);
            this.TC_SearchSettings.ResumeLayout(false);
            this.Tab_General.ResumeLayout(false);
            this.Tab_General.PerformLayout();
            this.Tab_Advanced.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}