namespace PKHeX.WinForms
{
    partial class SAV_Encounters
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
            P_Results = new System.Windows.Forms.Panel();
            EncounterPokeGrid = new Controls.PokeGrid();
            CB_Species = new System.Windows.Forms.ComboBox();
            CB_Move4 = new System.Windows.Forms.ComboBox();
            CB_Move3 = new System.Windows.Forms.ComboBox();
            CB_Move2 = new System.Windows.Forms.ComboBox();
            CB_Move1 = new System.Windows.Forms.ComboBox();
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
            TLP_Filters = new System.Windows.Forms.TableLayoutPanel();
            FLP_Level = new System.Windows.Forms.FlowLayoutPanel();
            CB_GameOrigin = new System.Windows.Forms.ComboBox();
            L_Version = new System.Windows.Forms.Label();
            TypeFilters = new System.Windows.Forms.FlowLayoutPanel();
            CHK_IsEgg = new System.Windows.Forms.CheckBox();
            CHK_Shiny = new System.Windows.Forms.CheckBox();
            RTB_Instructions = new System.Windows.Forms.RichTextBox();
            hover = new System.Windows.Forms.ToolTip(components);
            mnu = new System.Windows.Forms.ContextMenuStrip(components);
            mnuView = new System.Windows.Forms.ToolStripMenuItem();
            TC_SearchOptions = new System.Windows.Forms.TabControl();
            Tab_General = new System.Windows.Forms.TabPage();
            Tab_Advanced = new System.Windows.Forms.TabPage();
            B_Add = new System.Windows.Forms.Button();
            menuStrip1.SuspendLayout();
            P_Results.SuspendLayout();
            TLP_Filters.SuspendLayout();
            mnu.SuspendLayout();
            TC_SearchOptions.SuspendLayout();
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
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { Menu_Close });
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
            // P_Results
            // 
            P_Results.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            P_Results.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            P_Results.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            P_Results.Controls.Add(EncounterPokeGrid);
            P_Results.Controls.Add(SCR_Box);
            P_Results.Location = new System.Drawing.Point(14, 37);
            P_Results.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            P_Results.Name = "P_Results";
            P_Results.Size = new System.Drawing.Size(332, 406);
            P_Results.TabIndex = 66;
            // 
            // EncounterPokeGrid
            // 
            EncounterPokeGrid.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            EncounterPokeGrid.Location = new System.Drawing.Point(2, 2);
            EncounterPokeGrid.Margin = new System.Windows.Forms.Padding(0);
            EncounterPokeGrid.Name = "EncounterPokeGrid";
            EncounterPokeGrid.Size = new System.Drawing.Size(293, 399);
            EncounterPokeGrid.TabIndex = 2;
            // 
            // CB_Species
            // 
            CB_Species.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_Species.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Species.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Species.FormattingEnabled = true;
            CB_Species.Location = new System.Drawing.Point(73, 19);
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
            CB_Move4.Location = new System.Drawing.Point(73, 111);
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
            CB_Move3.Location = new System.Drawing.Point(73, 88);
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
            CB_Move2.Location = new System.Drawing.Point(73, 65);
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
            CB_Move1.Location = new System.Drawing.Point(73, 42);
            CB_Move1.Margin = new System.Windows.Forms.Padding(0);
            CB_Move1.Name = "CB_Move1";
            CB_Move1.Size = new System.Drawing.Size(142, 23);
            CB_Move1.TabIndex = 71;
            // 
            // Label_Species
            // 
            Label_Species.Anchor = System.Windows.Forms.AnchorStyles.Right;
            Label_Species.AutoSize = true;
            Label_Species.Location = new System.Drawing.Point(20, 23);
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
            L_Move1.Location = new System.Drawing.Point(20, 46);
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
            L_Move2.Location = new System.Drawing.Point(20, 69);
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
            L_Move3.Location = new System.Drawing.Point(20, 92);
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
            L_Move4.Location = new System.Drawing.Point(20, 115);
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
            FLP_Egg.Location = new System.Drawing.Point(0, 249);
            FLP_Egg.Margin = new System.Windows.Forms.Padding(0);
            FLP_Egg.Name = "FLP_Egg";
            FLP_Egg.Size = new System.Drawing.Size(0, 0);
            FLP_Egg.TabIndex = 120;
            // 
            // TLP_Filters
            // 
            TLP_Filters.AutoScroll = true;
            TLP_Filters.AutoScrollMargin = new System.Drawing.Size(3, 3);
            TLP_Filters.AutoSize = true;
            TLP_Filters.ColumnCount = 2;
            TLP_Filters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Filters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Filters.Controls.Add(Label_Species, 0, 1);
            TLP_Filters.Controls.Add(CB_Species, 1, 1);
            TLP_Filters.Controls.Add(FLP_Level, 1, 5);
            TLP_Filters.Controls.Add(L_Move1, 0, 9);
            TLP_Filters.Controls.Add(CB_Move1, 1, 9);
            TLP_Filters.Controls.Add(L_Move2, 0, 10);
            TLP_Filters.Controls.Add(CB_Move2, 1, 10);
            TLP_Filters.Controls.Add(L_Move3, 0, 11);
            TLP_Filters.Controls.Add(CB_Move3, 1, 11);
            TLP_Filters.Controls.Add(L_Move4, 0, 12);
            TLP_Filters.Controls.Add(CB_Move4, 1, 12);
            TLP_Filters.Controls.Add(CB_GameOrigin, 1, 16);
            TLP_Filters.Controls.Add(L_Version, 0, 16);
            TLP_Filters.Controls.Add(TypeFilters, 1, 17);
            TLP_Filters.Controls.Add(CHK_IsEgg, 1, 0);
            TLP_Filters.Controls.Add(FLP_Egg, 0, 17);
            TLP_Filters.Controls.Add(CHK_Shiny, 0, 0);
            TLP_Filters.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Filters.Location = new System.Drawing.Point(4, 3);
            TLP_Filters.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TLP_Filters.Name = "TLP_Filters";
            TLP_Filters.RowCount = 18;
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
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            TLP_Filters.Size = new System.Drawing.Size(292, 358);
            TLP_Filters.TabIndex = 118;
            // 
            // FLP_Level
            // 
            FLP_Level.Anchor = System.Windows.Forms.AnchorStyles.Left;
            FLP_Level.AutoSize = true;
            FLP_Level.Location = new System.Drawing.Point(73, 42);
            FLP_Level.Margin = new System.Windows.Forms.Padding(0);
            FLP_Level.Name = "FLP_Level";
            FLP_Level.Size = new System.Drawing.Size(0, 0);
            FLP_Level.TabIndex = 119;
            // 
            // CB_GameOrigin
            // 
            CB_GameOrigin.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_GameOrigin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_GameOrigin.FormattingEnabled = true;
            CB_GameOrigin.Location = new System.Drawing.Point(73, 134);
            CB_GameOrigin.Margin = new System.Windows.Forms.Padding(0);
            CB_GameOrigin.Name = "CB_GameOrigin";
            CB_GameOrigin.Size = new System.Drawing.Size(142, 23);
            CB_GameOrigin.TabIndex = 121;
            // 
            // L_Version
            // 
            L_Version.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Version.AutoSize = true;
            L_Version.Location = new System.Drawing.Point(4, 138);
            L_Version.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            L_Version.Name = "L_Version";
            L_Version.Size = new System.Drawing.Size(65, 15);
            L_Version.TabIndex = 122;
            L_Version.Text = "OT Version:";
            L_Version.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TypeFilters
            // 
            TypeFilters.Dock = System.Windows.Forms.DockStyle.Fill;
            TypeFilters.Location = new System.Drawing.Point(77, 160);
            TypeFilters.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TypeFilters.Name = "TypeFilters";
            TypeFilters.Size = new System.Drawing.Size(266, 178);
            TypeFilters.TabIndex = 123;
            // 
            // CHK_IsEgg
            // 
            CHK_IsEgg.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CHK_IsEgg.AutoSize = true;
            CHK_IsEgg.Checked = true;
            CHK_IsEgg.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            CHK_IsEgg.Location = new System.Drawing.Point(73, 0);
            CHK_IsEgg.Margin = new System.Windows.Forms.Padding(0);
            CHK_IsEgg.Name = "CHK_IsEgg";
            CHK_IsEgg.Size = new System.Drawing.Size(46, 19);
            CHK_IsEgg.TabIndex = 125;
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
            CHK_Shiny.Location = new System.Drawing.Point(18, 0);
            CHK_Shiny.Margin = new System.Windows.Forms.Padding(0);
            CHK_Shiny.Name = "CHK_Shiny";
            CHK_Shiny.Size = new System.Drawing.Size(55, 19);
            CHK_Shiny.TabIndex = 126;
            CHK_Shiny.Text = "Shiny";
            CHK_Shiny.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            CHK_Shiny.ThreeState = true;
            CHK_Shiny.UseVisualStyleBackColor = true;
            // 
            // RTB_Instructions
            // 
            RTB_Instructions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            RTB_Instructions.Location = new System.Drawing.Point(0, 48);
            RTB_Instructions.Margin = new System.Windows.Forms.Padding(0);
            RTB_Instructions.Name = "RTB_Instructions";
            RTB_Instructions.Size = new System.Drawing.Size(298, 313);
            RTB_Instructions.TabIndex = 119;
            RTB_Instructions.Text = "";
            // 
            // mnu
            // 
            mnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuView });
            mnu.Name = "contextMenuStrip1";
            mnu.Size = new System.Drawing.Size(100, 26);
            // 
            // mnuView
            // 
            mnuView.Image = Properties.Resources.other;
            mnuView.Name = "mnuView";
            mnuView.Size = new System.Drawing.Size(99, 22);
            mnuView.Text = "View";
            mnuView.Click += ClickView;
            // 
            // TC_SearchOptions
            // 
            TC_SearchOptions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            TC_SearchOptions.Controls.Add(Tab_General);
            TC_SearchOptions.Controls.Add(Tab_Advanced);
            TC_SearchOptions.Location = new System.Drawing.Point(355, 9);
            TC_SearchOptions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TC_SearchOptions.Name = "TC_SearchOptions";
            TC_SearchOptions.SelectedIndex = 0;
            TC_SearchOptions.Size = new System.Drawing.Size(308, 392);
            TC_SearchOptions.TabIndex = 120;
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
            B_Add.Location = new System.Drawing.Point(230, -1);
            B_Add.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Add.Name = "B_Add";
            B_Add.Size = new System.Drawing.Size(66, 27);
            B_Add.TabIndex = 122;
            B_Add.Text = "Add";
            B_Add.UseVisualStyleBackColor = true;
            B_Add.Click += B_Add_Click;
            // 
            // SAV_Encounters
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(670, 463);
            Controls.Add(B_Reset);
            Controls.Add(TC_SearchOptions);
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
            Name = "SAV_Encounters";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Database";
            FormClosing += SAV_Encounters_FormClosing;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            P_Results.ResumeLayout(false);
            TLP_Filters.ResumeLayout(false);
            TLP_Filters.PerformLayout();
            mnu.ResumeLayout(false);
            TC_SearchOptions.ResumeLayout(false);
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
        private System.Windows.Forms.Panel P_Results;
        public System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.ComboBox CB_Move4;
        private System.Windows.Forms.ComboBox CB_Move3;
        private System.Windows.Forms.ComboBox CB_Move2;
        private System.Windows.Forms.ComboBox CB_Move1;
        private System.Windows.Forms.Label Label_Species;
        private System.Windows.Forms.Button B_Search;
        private System.Windows.Forms.Label L_Move1;
        private System.Windows.Forms.Label L_Move2;
        private System.Windows.Forms.Label L_Move3;
        private System.Windows.Forms.Label L_Move4;
        private System.Windows.Forms.Button B_Reset;
        private System.Windows.Forms.Label L_Count;
        private System.Windows.Forms.Label L_Viewed;
        private System.Windows.Forms.TableLayoutPanel TLP_Filters;
        private System.Windows.Forms.FlowLayoutPanel FLP_Egg;
        private System.Windows.Forms.FlowLayoutPanel FLP_Level;
        private System.Windows.Forms.RichTextBox RTB_Instructions;
        private System.Windows.Forms.ComboBox CB_GameOrigin;
        private System.Windows.Forms.Label L_Version;
        private Controls.PokeGrid EncounterPokeGrid;
        private System.Windows.Forms.FlowLayoutPanel TypeFilters;
        private System.Windows.Forms.ToolTip hover;
        private System.Windows.Forms.ContextMenuStrip mnu;
        private System.Windows.Forms.ToolStripMenuItem mnuView;
        private System.Windows.Forms.TabControl TC_SearchOptions;
        private System.Windows.Forms.TabPage Tab_General;
        private System.Windows.Forms.TabPage Tab_Advanced;
        public System.Windows.Forms.CheckBox CHK_IsEgg;
        public System.Windows.Forms.CheckBox CHK_Shiny;
        private System.Windows.Forms.Button B_Add;
    }
}
