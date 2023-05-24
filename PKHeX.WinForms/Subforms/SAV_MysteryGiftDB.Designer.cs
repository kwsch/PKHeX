namespace PKHeX.WinForms
{
    partial class SAV_MysteryGiftDB
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
            Menu_OpenDB = new System.Windows.Forms.ToolStripMenuItem();
            Menu_Export = new System.Windows.Forms.ToolStripMenuItem();
            Menu_Import = new System.Windows.Forms.ToolStripMenuItem();
            P_Results = new System.Windows.Forms.Panel();
            MysteryPokeGrid = new Controls.PokeGrid();
            CB_HeldItem = new System.Windows.Forms.ComboBox();
            CB_Species = new System.Windows.Forms.ComboBox();
            CB_Move4 = new System.Windows.Forms.ComboBox();
            CB_Move3 = new System.Windows.Forms.ComboBox();
            CB_Move2 = new System.Windows.Forms.ComboBox();
            CB_Move1 = new System.Windows.Forms.ComboBox();
            Label_HeldItem = new System.Windows.Forms.Label();
            Label_Species = new System.Windows.Forms.Label();
            B_Search = new System.Windows.Forms.Button();
            L_Move1 = new System.Windows.Forms.Label();
            L_Move2 = new System.Windows.Forms.Label();
            L_Move3 = new System.Windows.Forms.Label();
            L_Move4 = new System.Windows.Forms.Label();
            B_Reset = new System.Windows.Forms.Button();
            L_Count = new System.Windows.Forms.Label();
            L_Viewed = new System.Windows.Forms.Label();
            FLP_Egg = new System.Windows.Forms.FlowLayoutPanel();
            CHK_IsEgg = new System.Windows.Forms.CheckBox();
            CHK_Shiny = new System.Windows.Forms.CheckBox();
            TLP_Filters = new System.Windows.Forms.TableLayoutPanel();
            FLP_Format = new System.Windows.Forms.FlowLayoutPanel();
            CB_FormatComparator = new System.Windows.Forms.ComboBox();
            CB_Format = new System.Windows.Forms.ComboBox();
            L_Format = new System.Windows.Forms.Label();
            FLP_Level = new System.Windows.Forms.FlowLayoutPanel();
            mnu = new System.Windows.Forms.ContextMenuStrip(components);
            mnuView = new System.Windows.Forms.ToolStripMenuItem();
            mnuSaveMG = new System.Windows.Forms.ToolStripMenuItem();
            mnuSavePK = new System.Windows.Forms.ToolStripMenuItem();
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
            Menu_Tools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { Menu_OpenDB, Menu_Export, Menu_Import });
            Menu_Tools.Name = "Menu_Tools";
            Menu_Tools.Size = new System.Drawing.Size(46, 20);
            Menu_Tools.Text = "Tools";
            // 
            // Menu_OpenDB
            // 
            Menu_OpenDB.Image = Properties.Resources.folder;
            Menu_OpenDB.Name = "Menu_OpenDB";
            Menu_OpenDB.Size = new System.Drawing.Size(209, 22);
            Menu_OpenDB.Text = "Open Database Folder";
            Menu_OpenDB.Click += OpenDB;
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
            // P_Results
            // 
            P_Results.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            P_Results.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            P_Results.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            P_Results.Controls.Add(MysteryPokeGrid);
            P_Results.Controls.Add(SCR_Box);
            P_Results.Location = new System.Drawing.Point(14, 37);
            P_Results.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            P_Results.Name = "P_Results";
            P_Results.Size = new System.Drawing.Size(332, 406);
            P_Results.TabIndex = 66;
            // 
            // MysteryPokeGrid
            // 
            MysteryPokeGrid.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            MysteryPokeGrid.Location = new System.Drawing.Point(2, 2);
            MysteryPokeGrid.Margin = new System.Windows.Forms.Padding(0);
            MysteryPokeGrid.Name = "MysteryPokeGrid";
            MysteryPokeGrid.Size = new System.Drawing.Size(293, 399);
            MysteryPokeGrid.TabIndex = 2;
            // 
            // CB_HeldItem
            // 
            CB_HeldItem.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_HeldItem.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_HeldItem.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_HeldItem.FormattingEnabled = true;
            CB_HeldItem.Location = new System.Drawing.Point(70, 42);
            CB_HeldItem.Margin = new System.Windows.Forms.Padding(0);
            CB_HeldItem.Name = "CB_HeldItem";
            CB_HeldItem.Size = new System.Drawing.Size(142, 23);
            CB_HeldItem.TabIndex = 69;
            // 
            // CB_Species
            // 
            CB_Species.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_Species.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Species.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Species.FormattingEnabled = true;
            CB_Species.Location = new System.Drawing.Point(70, 19);
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
            CB_Move4.Location = new System.Drawing.Point(70, 134);
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
            CB_Move3.Location = new System.Drawing.Point(70, 111);
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
            CB_Move2.Location = new System.Drawing.Point(70, 88);
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
            CB_Move1.Location = new System.Drawing.Point(70, 65);
            CB_Move1.Margin = new System.Windows.Forms.Padding(0);
            CB_Move1.Name = "CB_Move1";
            CB_Move1.Size = new System.Drawing.Size(142, 23);
            CB_Move1.TabIndex = 71;
            // 
            // Label_HeldItem
            // 
            Label_HeldItem.Anchor = System.Windows.Forms.AnchorStyles.Right;
            Label_HeldItem.AutoSize = true;
            Label_HeldItem.Location = new System.Drawing.Point(4, 46);
            Label_HeldItem.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Label_HeldItem.Name = "Label_HeldItem";
            Label_HeldItem.Size = new System.Drawing.Size(62, 15);
            Label_HeldItem.TabIndex = 93;
            Label_HeldItem.Text = "Held Item:";
            Label_HeldItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_Species
            // 
            Label_Species.Anchor = System.Windows.Forms.AnchorStyles.Right;
            Label_Species.AutoSize = true;
            Label_Species.Location = new System.Drawing.Point(17, 23);
            Label_Species.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Label_Species.Name = "Label_Species";
            Label_Species.Size = new System.Drawing.Size(49, 15);
            Label_Species.TabIndex = 90;
            Label_Species.Text = "Species:";
            Label_Species.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // B_Search
            // 
            B_Search.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Search.Location = new System.Drawing.Point(388, 408);
            B_Search.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Search.Name = "B_Search";
            B_Search.Size = new System.Drawing.Size(240, 35);
            B_Search.TabIndex = 102;
            B_Search.Text = "Search!";
            B_Search.UseVisualStyleBackColor = true;
            B_Search.Click += B_Search_Click;
            // 
            // L_Move1
            // 
            L_Move1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Move1.AutoSize = true;
            L_Move1.Location = new System.Drawing.Point(17, 69);
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
            L_Move2.Location = new System.Drawing.Point(17, 92);
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
            L_Move3.Location = new System.Drawing.Point(17, 115);
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
            L_Move4.Location = new System.Drawing.Point(17, 138);
            L_Move4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            L_Move4.Name = "L_Move4";
            L_Move4.Size = new System.Drawing.Size(49, 15);
            L_Move4.TabIndex = 108;
            L_Move4.Text = "Move 4:";
            L_Move4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            // L_Viewed
            // 
            L_Viewed.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            L_Viewed.AutoSize = true;
            L_Viewed.Location = new System.Drawing.Point(10, 445);
            L_Viewed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Viewed.Name = "L_Viewed";
            L_Viewed.Size = new System.Drawing.Size(89, 15);
            L_Viewed.TabIndex = 117;
            L_Viewed.Text = "Last Viewed: {0}";
            L_Viewed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FLP_Egg
            // 
            FLP_Egg.Anchor = System.Windows.Forms.AnchorStyles.Left;
            FLP_Egg.AutoSize = true;
            FLP_Egg.Controls.Add(CHK_IsEgg);
            FLP_Egg.Location = new System.Drawing.Point(70, 0);
            FLP_Egg.Margin = new System.Windows.Forms.Padding(0);
            FLP_Egg.Name = "FLP_Egg";
            FLP_Egg.Size = new System.Drawing.Size(46, 19);
            FLP_Egg.TabIndex = 120;
            // 
            // CHK_IsEgg
            // 
            CHK_IsEgg.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CHK_IsEgg.AutoSize = true;
            CHK_IsEgg.Checked = true;
            CHK_IsEgg.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            CHK_IsEgg.Location = new System.Drawing.Point(0, 0);
            CHK_IsEgg.Margin = new System.Windows.Forms.Padding(0);
            CHK_IsEgg.Name = "CHK_IsEgg";
            CHK_IsEgg.Size = new System.Drawing.Size(46, 19);
            CHK_IsEgg.TabIndex = 98;
            CHK_IsEgg.Text = "Egg";
            CHK_IsEgg.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            CHK_IsEgg.ThreeState = true;
            CHK_IsEgg.UseVisualStyleBackColor = true;
            // 
            // CHK_Shiny
            // 
            CHK_Shiny.Anchor = System.Windows.Forms.AnchorStyles.Right;
            CHK_Shiny.AutoSize = true;
            CHK_Shiny.Checked = true;
            CHK_Shiny.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            CHK_Shiny.Location = new System.Drawing.Point(15, 0);
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
            TLP_Filters.Controls.Add(Label_HeldItem, 0, 3);
            TLP_Filters.Controls.Add(CB_HeldItem, 1, 3);
            TLP_Filters.Controls.Add(FLP_Level, 1, 5);
            TLP_Filters.Controls.Add(L_Move1, 0, 9);
            TLP_Filters.Controls.Add(CB_Move1, 1, 9);
            TLP_Filters.Controls.Add(L_Move2, 0, 10);
            TLP_Filters.Controls.Add(CB_Move2, 1, 10);
            TLP_Filters.Controls.Add(L_Move3, 0, 11);
            TLP_Filters.Controls.Add(CB_Move3, 1, 11);
            TLP_Filters.Controls.Add(L_Move4, 0, 12);
            TLP_Filters.Controls.Add(CB_Move4, 1, 12);
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
            TLP_Filters.Size = new System.Drawing.Size(292, 358);
            TLP_Filters.TabIndex = 118;
            // 
            // FLP_Format
            // 
            FLP_Format.AutoSize = true;
            FLP_Format.Controls.Add(CB_FormatComparator);
            FLP_Format.Controls.Add(CB_Format);
            FLP_Format.Location = new System.Drawing.Point(70, 157);
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
            CB_FormatComparator.Items.AddRange(new object[] { "Any", ">=", "==", "<=" });
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
            CB_Format.DropDownWidth = 100;
            CB_Format.FormattingEnabled = true;
            CB_Format.Items.AddRange(new object[] { "Any", ".wc9", ".wc8", ".wc7", ".wc6", ".pgf", ".pcd/pgt/.wc4", ".wc3" });
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
            L_Format.Location = new System.Drawing.Point(18, 161);
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
            FLP_Level.Location = new System.Drawing.Point(70, 65);
            FLP_Level.Margin = new System.Windows.Forms.Padding(0);
            FLP_Level.Name = "FLP_Level";
            FLP_Level.Size = new System.Drawing.Size(0, 0);
            FLP_Level.TabIndex = 119;
            // 
            // mnu
            // 
            mnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuView, mnuSaveMG, mnuSavePK });
            mnu.Name = "contextMenuStrip1";
            mnu.Size = new System.Drawing.Size(127, 70);
            // 
            // mnuView
            // 
            mnuView.Image = Properties.Resources.other;
            mnuView.Name = "mnuView";
            mnuView.Size = new System.Drawing.Size(126, 22);
            mnuView.Text = "View";
            mnuView.Click += ClickView;
            // 
            // mnuSaveMG
            // 
            mnuSaveMG.Image = Properties.Resources.gift;
            mnuSaveMG.Name = "mnuSaveMG";
            mnuSaveMG.Size = new System.Drawing.Size(126, 22);
            mnuSaveMG.Text = "Save Gift";
            mnuSaveMG.Click += ClickSaveMG;
            // 
            // mnuSavePK
            // 
            mnuSavePK.Image = Properties.Resources.savePKM;
            mnuSavePK.Name = "mnuSavePK";
            mnuSavePK.Size = new System.Drawing.Size(126, 22);
            mnuSavePK.Text = "Save PKM";
            mnuSavePK.Click += ClickSavePK;
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
            TC_SearchSettings.Size = new System.Drawing.Size(308, 392);
            TC_SearchSettings.TabIndex = 120;
            // 
            // Tab_General
            // 
            Tab_General.Controls.Add(TLP_Filters);
            Tab_General.Location = new System.Drawing.Point(4, 24);
            Tab_General.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_General.Name = "Tab_General";
            Tab_General.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_General.Size = new System.Drawing.Size(300, 364);
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
            Tab_Advanced.Size = new System.Drawing.Size(300, 364);
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
            B_Add.TabIndex = 121;
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
            RTB_Instructions.Size = new System.Drawing.Size(298, 313);
            RTB_Instructions.TabIndex = 120;
            RTB_Instructions.Text = "";
            // 
            // SAV_MysteryGiftDB
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(670, 463);
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
            Name = "SAV_MysteryGiftDB";
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
        private System.Windows.Forms.Panel P_Results;
        private System.Windows.Forms.ComboBox CB_HeldItem;
        public System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.ComboBox CB_Move4;
        private System.Windows.Forms.ComboBox CB_Move3;
        private System.Windows.Forms.ComboBox CB_Move2;
        private System.Windows.Forms.ComboBox CB_Move1;
        private System.Windows.Forms.Label Label_HeldItem;
        private System.Windows.Forms.Label Label_Species;
        private System.Windows.Forms.Button B_Search;
        private System.Windows.Forms.Label L_Move1;
        private System.Windows.Forms.Label L_Move2;
        private System.Windows.Forms.Label L_Move3;
        private System.Windows.Forms.Label L_Move4;
        private System.Windows.Forms.Button B_Reset;
        private System.Windows.Forms.Label L_Count;
        private System.Windows.Forms.Label L_Viewed;
        private System.Windows.Forms.ToolStripMenuItem Menu_Export;
        private System.Windows.Forms.TableLayoutPanel TLP_Filters;
        public System.Windows.Forms.CheckBox CHK_Shiny;
        public System.Windows.Forms.CheckBox CHK_IsEgg;
        private System.Windows.Forms.FlowLayoutPanel FLP_Egg;
        private System.Windows.Forms.FlowLayoutPanel FLP_Level;
        private System.Windows.Forms.Label L_Format;
        private System.Windows.Forms.FlowLayoutPanel FLP_Format;
        private System.Windows.Forms.ComboBox CB_FormatComparator;
        private System.Windows.Forms.ComboBox CB_Format;
        private System.Windows.Forms.ToolStripMenuItem Menu_Import;
        private Controls.PokeGrid MysteryPokeGrid;
        private System.Windows.Forms.ContextMenuStrip mnu;
        private System.Windows.Forms.ToolStripMenuItem mnuView;
        private System.Windows.Forms.ToolStripMenuItem mnuSaveMG;
        private System.Windows.Forms.ToolStripMenuItem mnuSavePK;
        private System.Windows.Forms.ToolTip hover;
        private System.Windows.Forms.TabControl TC_SearchSettings;
        private System.Windows.Forms.TabPage Tab_General;
        private System.Windows.Forms.TabPage Tab_Advanced;
        private System.Windows.Forms.RichTextBox RTB_Instructions;
        private System.Windows.Forms.Button B_Add;
    }
}
