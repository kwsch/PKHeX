namespace PKHeX.WinForms
{
    partial class SAV_Medals5
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            TC_Main = new System.Windows.Forms.TabControl();
            Tab_Medals = new System.Windows.Forms.TabPage();
            DGV_Medals = new PKHeX.WinForms.Controls.DoubleBufferedDataGridView();
            MedalIndexColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            MedalNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            MedalTypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            MedalStateColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            MedalUnreadColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            MedalDateColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            MedalSettingsPanel = new System.Windows.Forms.TableLayoutPanel();
            L_PinnedMedal = new System.Windows.Forms.Label();
            CB_PinnedMedal = new System.Windows.Forms.ComboBox();
            L_Rank = new System.Windows.Forms.Label();
            CB_Rank = new System.Windows.Forms.ComboBox();
            CHK_TutorialComplete = new System.Windows.Forms.CheckBox();
            MedalButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            B_ExportAll = new System.Windows.Forms.Button();
            B_ImportAll = new System.Windows.Forms.Button();
            B_GiveAll = new System.Windows.Forms.Button();
            Tab_Habitat = new System.Windows.Forms.TabPage();
            DGV_Habitat = new PKHeX.WinForms.Controls.DoubleBufferedDataGridView();
            HabitatIndexColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            HabitatCompleteColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            HabitatGrassColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            HabitatSurfColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            HabitatFishColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            HabitatBottomPanel = new System.Windows.Forms.TableLayoutPanel();
            FLP_HabitatActions = new System.Windows.Forms.FlowLayoutPanel();
            B_HabitatClear = new System.Windows.Forms.Button();
            B_HabitatSetComplete = new System.Windows.Forms.Button();
            CHK_HabitatTutorialViewed = new System.Windows.Forms.CheckBox();
            CHK_HabitatTutorialCompleteCapture = new System.Windows.Forms.CheckBox();
            L_Unknown90 = new System.Windows.Forms.Label();
            NUD_Unknown90 = new System.Windows.Forms.NumericUpDown();
            L_Unknown92 = new System.Windows.Forms.Label();
            NUD_Unknown92 = new System.Windows.Forms.NumericUpDown();
            L_LastEncounterType = new System.Windows.Forms.Label();
            CB_LastEncounterType = new System.Windows.Forms.ComboBox();
            ButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            B_Save = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            TC_Main.SuspendLayout();
            Tab_Medals.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_Medals).BeginInit();
            MedalSettingsPanel.SuspendLayout();
            MedalButtonPanel.SuspendLayout();
            Tab_Habitat.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_Habitat).BeginInit();
            HabitatBottomPanel.SuspendLayout();
            FLP_HabitatActions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Unknown90).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Unknown92).BeginInit();
            ButtonPanel.SuspendLayout();
            SuspendLayout();
            // 
            // TC_Main
            // 
            TC_Main.Controls.Add(Tab_Medals);
            TC_Main.Controls.Add(Tab_Habitat);
            TC_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            TC_Main.Location = new System.Drawing.Point(0, 0);
            TC_Main.Name = "TC_Main";
            TC_Main.SelectedIndex = 0;
            TC_Main.Size = new System.Drawing.Size(984, 522);
            TC_Main.TabIndex = 0;
            // 
            // Tab_Medals
            // 
            Tab_Medals.Controls.Add(DGV_Medals);
            Tab_Medals.Controls.Add(MedalSettingsPanel);
            Tab_Medals.Controls.Add(MedalButtonPanel);
            Tab_Medals.Location = new System.Drawing.Point(4, 26);
            Tab_Medals.Name = "Tab_Medals";
            Tab_Medals.Padding = new System.Windows.Forms.Padding(3);
            Tab_Medals.Size = new System.Drawing.Size(976, 492);
            Tab_Medals.TabIndex = 0;
            Tab_Medals.Text = "Medals";
            Tab_Medals.UseVisualStyleBackColor = true;
            // 
            // DGV_Medals
            // 
            DGV_Medals.AllowUserToAddRows = false;
            DGV_Medals.AllowUserToDeleteRows = false;
            DGV_Medals.AllowUserToResizeRows = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.ControlLight;
            DGV_Medals.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            DGV_Medals.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            DGV_Medals.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_Medals.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { MedalIndexColumn, MedalNameColumn, MedalTypeColumn, MedalStateColumn, MedalUnreadColumn, MedalDateColumn });
            DGV_Medals.Dock = System.Windows.Forms.DockStyle.Fill;
            DGV_Medals.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            DGV_Medals.Location = new System.Drawing.Point(3, 3);
            DGV_Medals.MultiSelect = false;
            DGV_Medals.Name = "DGV_Medals";
            DGV_Medals.RowHeadersVisible = false;
            DGV_Medals.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            DGV_Medals.Size = new System.Drawing.Size(970, 363);
            DGV_Medals.TabIndex = 0;
            DGV_Medals.CellBeginEdit += DGV_Medals_CellBeginEdit;
            DGV_Medals.CellValueChanged += DGV_Medals_CellValueChanged;
            DGV_Medals.CellParsing += DGV_Medals_CellParsing;
            DGV_Medals.CellValidating += DGV_Medals_CellValidating;
            DGV_Medals.CurrentCellDirtyStateChanged += DGV_Medals_CurrentCellDirtyStateChanged;
            DGV_Medals.DataError += DGV_Medals_DataError;
            DGV_Medals.EditingControlShowing += DGV_Medals_EditingControlShowing;
            // 
            // MedalIndexColumn
            // 
            MedalIndexColumn.FillWeight = 60F;
            MedalIndexColumn.HeaderText = "Index";
            MedalIndexColumn.Name = "MedalIndexColumn";
            MedalIndexColumn.ReadOnly = true;
            MedalIndexColumn.ValueType = typeof(int);
            // 
            // MedalNameColumn
            // 
            MedalNameColumn.FillWeight = 220F;
            MedalNameColumn.HeaderText = "Name";
            MedalNameColumn.Name = "MedalNameColumn";
            MedalNameColumn.ReadOnly = true;
            // 
            // MedalTypeColumn
            // 
            MedalTypeColumn.FillWeight = 120F;
            MedalTypeColumn.HeaderText = "Type";
            MedalTypeColumn.Name = "MedalTypeColumn";
            MedalTypeColumn.ReadOnly = true;
            // 
            // MedalStateColumn
            // 
            MedalStateColumn.FillWeight = 170F;
            MedalStateColumn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            MedalStateColumn.HeaderText = "State";
            MedalStateColumn.Name = "MedalStateColumn";
            MedalStateColumn.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            MedalStateColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            MedalStateColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // MedalUnreadColumn
            // 
            MedalUnreadColumn.FillWeight = 90F;
            MedalUnreadColumn.HeaderText = "IsUnread";
            MedalUnreadColumn.Name = "MedalUnreadColumn";
            // 
            // MedalDateColumn
            // 
            MedalDateColumn.FillWeight = 120F;
            MedalDateColumn.HeaderText = "Date";
            MedalDateColumn.Name = "MedalDateColumn";
            // 
            // MedalSettingsPanel
            // 
            MedalSettingsPanel.AutoSize = true;
            MedalSettingsPanel.ColumnCount = 4;
            MedalSettingsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            MedalSettingsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            MedalSettingsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            MedalSettingsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            MedalSettingsPanel.Controls.Add(L_PinnedMedal, 0, 0);
            MedalSettingsPanel.Controls.Add(CB_PinnedMedal, 1, 0);
            MedalSettingsPanel.Controls.Add(L_Rank, 2, 0);
            MedalSettingsPanel.Controls.Add(CB_Rank, 3, 0);
            MedalSettingsPanel.Controls.Add(CHK_TutorialComplete, 0, 1);
            MedalSettingsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            MedalSettingsPanel.Location = new System.Drawing.Point(3, 366);
            MedalSettingsPanel.Name = "MedalSettingsPanel";
            MedalSettingsPanel.Padding = new System.Windows.Forms.Padding(8);
            MedalSettingsPanel.RowCount = 2;
            MedalSettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            MedalSettingsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            MedalSettingsPanel.Size = new System.Drawing.Size(970, 74);
            MedalSettingsPanel.TabIndex = 2;
            // 
            // L_PinnedMedal
            // 
            L_PinnedMedal.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_PinnedMedal.AutoSize = true;
            L_PinnedMedal.Location = new System.Drawing.Point(11, 15);
            L_PinnedMedal.Name = "L_PinnedMedal";
            L_PinnedMedal.Size = new System.Drawing.Size(91, 17);
            L_PinnedMedal.TabIndex = 0;
            L_PinnedMedal.Text = "Pinned Medal:";
            // 
            // CB_PinnedMedal
            // 
            CB_PinnedMedal.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            CB_PinnedMedal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_PinnedMedal.FormattingEnabled = true;
            CB_PinnedMedal.Location = new System.Drawing.Point(108, 11);
            CB_PinnedMedal.Name = "CB_PinnedMedal";
            CB_PinnedMedal.Size = new System.Drawing.Size(400, 25);
            CB_PinnedMedal.TabIndex = 1;
            // 
            // L_Rank
            // 
            L_Rank.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Rank.AutoSize = true;
            L_Rank.Location = new System.Drawing.Point(514, 15);
            L_Rank.Name = "L_Rank";
            L_Rank.Size = new System.Drawing.Size(39, 17);
            L_Rank.TabIndex = 2;
            L_Rank.Text = "Rank:";
            L_Rank.Click += L_Rank_Click;
            // 
            // CB_Rank
            // 
            CB_Rank.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            CB_Rank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Rank.FormattingEnabled = true;
            CB_Rank.Location = new System.Drawing.Point(559, 11);
            CB_Rank.Name = "CB_Rank";
            CB_Rank.Size = new System.Drawing.Size(400, 25);
            CB_Rank.TabIndex = 3;
            // 
            // CHK_TutorialComplete
            // 
            CHK_TutorialComplete.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CHK_TutorialComplete.AutoSize = true;
            MedalSettingsPanel.SetColumnSpan(CHK_TutorialComplete, 2);
            CHK_TutorialComplete.Location = new System.Drawing.Point(11, 42);
            CHK_TutorialComplete.Name = "CHK_TutorialComplete";
            CHK_TutorialComplete.Size = new System.Drawing.Size(131, 21);
            CHK_TutorialComplete.TabIndex = 4;
            CHK_TutorialComplete.Text = "Tutorial Complete";
            CHK_TutorialComplete.UseVisualStyleBackColor = true;
            // 
            // MedalButtonPanel
            // 
            MedalButtonPanel.AutoSize = true;
            MedalButtonPanel.Controls.Add(B_ExportAll);
            MedalButtonPanel.Controls.Add(B_ImportAll);
            MedalButtonPanel.Controls.Add(B_GiveAll);
            MedalButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            MedalButtonPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            MedalButtonPanel.Location = new System.Drawing.Point(3, 440);
            MedalButtonPanel.Name = "MedalButtonPanel";
            MedalButtonPanel.Padding = new System.Windows.Forms.Padding(8);
            MedalButtonPanel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            MedalButtonPanel.Size = new System.Drawing.Size(970, 49);
            MedalButtonPanel.TabIndex = 1;
            MedalButtonPanel.WrapContents = false;
            // 
            // B_ExportAll
            // 
            B_ExportAll.AutoSize = true;
            B_ExportAll.Location = new System.Drawing.Point(11, 11);
            B_ExportAll.Name = "B_ExportAll";
            B_ExportAll.Size = new System.Drawing.Size(77, 27);
            B_ExportAll.TabIndex = 0;
            B_ExportAll.Text = "Export All";
            B_ExportAll.UseVisualStyleBackColor = true;
            B_ExportAll.Click += B_ExportAll_Click;
            // 
            // B_ImportAll
            // 
            B_ImportAll.AutoSize = true;
            B_ImportAll.Location = new System.Drawing.Point(94, 11);
            B_ImportAll.Name = "B_ImportAll";
            B_ImportAll.Size = new System.Drawing.Size(77, 27);
            B_ImportAll.TabIndex = 1;
            B_ImportAll.Text = "Import All";
            B_ImportAll.UseVisualStyleBackColor = true;
            B_ImportAll.Click += B_ImportAll_Click;
            // 
            // B_GiveAll
            // 
            B_GiveAll.AutoSize = true;
            B_GiveAll.Location = new System.Drawing.Point(177, 11);
            B_GiveAll.Name = "B_GiveAll";
            B_GiveAll.Size = new System.Drawing.Size(65, 27);
            B_GiveAll.TabIndex = 2;
            B_GiveAll.Text = "Give All";
            B_GiveAll.UseVisualStyleBackColor = true;
            B_GiveAll.Click += B_GiveAll_Click;
            // 
            // Tab_Habitat
            // 
            Tab_Habitat.Controls.Add(DGV_Habitat);
            Tab_Habitat.Controls.Add(HabitatBottomPanel);
            Tab_Habitat.Location = new System.Drawing.Point(4, 26);
            Tab_Habitat.Name = "Tab_Habitat";
            Tab_Habitat.Padding = new System.Windows.Forms.Padding(3);
            Tab_Habitat.Size = new System.Drawing.Size(976, 492);
            Tab_Habitat.TabIndex = 1;
            Tab_Habitat.Text = "Habitat";
            Tab_Habitat.UseVisualStyleBackColor = true;
            // 
            // DGV_Habitat
            // 
            DGV_Habitat.AllowUserToAddRows = false;
            DGV_Habitat.AllowUserToDeleteRows = false;
            DGV_Habitat.AllowUserToResizeRows = false;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.ControlLight;
            DGV_Habitat.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle6;
            DGV_Habitat.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            DGV_Habitat.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_Habitat.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { HabitatIndexColumn, HabitatCompleteColumn, HabitatGrassColumn, HabitatSurfColumn, HabitatFishColumn });
            DGV_Habitat.Dock = System.Windows.Forms.DockStyle.Fill;
            DGV_Habitat.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            DGV_Habitat.Location = new System.Drawing.Point(3, 3);
            DGV_Habitat.Name = "DGV_Habitat";
            DGV_Habitat.RowHeadersVisible = false;
            DGV_Habitat.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            DGV_Habitat.Size = new System.Drawing.Size(970, 381);
            DGV_Habitat.TabIndex = 0;
            DGV_Habitat.CellValueChanged += DGV_Habitat_CellValueChanged;
            DGV_Habitat.CurrentCellDirtyStateChanged += DGV_Habitat_CurrentCellDirtyStateChanged;
            DGV_Habitat.DataError += DGV_Habitat_DataError;
            DGV_Habitat.EditingControlShowing += DGV_Habitat_EditingControlShowing;
            // 
            // HabitatIndexColumn
            // 
            HabitatIndexColumn.FillWeight = 70F;
            HabitatIndexColumn.HeaderText = "Index";
            HabitatIndexColumn.Name = "HabitatIndexColumn";
            HabitatIndexColumn.ReadOnly = true;
            HabitatIndexColumn.ValueType = typeof(int);
            // 
            // HabitatCompleteColumn
            // 
            HabitatCompleteColumn.FillWeight = 90F;
            HabitatCompleteColumn.HeaderText = "Complete";
            HabitatCompleteColumn.Name = "HabitatCompleteColumn";
            // 
            // HabitatGrassColumn
            // 
            HabitatGrassColumn.FillWeight = 120F;
            HabitatGrassColumn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            HabitatGrassColumn.HeaderText = "Grass";
            HabitatGrassColumn.Name = "HabitatGrassColumn";
            HabitatGrassColumn.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            HabitatGrassColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            HabitatGrassColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // HabitatSurfColumn
            // 
            HabitatSurfColumn.FillWeight = 120F;
            HabitatSurfColumn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            HabitatSurfColumn.HeaderText = "Surf";
            HabitatSurfColumn.Name = "HabitatSurfColumn";
            HabitatSurfColumn.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            HabitatSurfColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            HabitatSurfColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // HabitatFishColumn
            // 
            HabitatFishColumn.FillWeight = 120F;
            HabitatFishColumn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            HabitatFishColumn.HeaderText = "Fish";
            HabitatFishColumn.Name = "HabitatFishColumn";
            HabitatFishColumn.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            HabitatFishColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            HabitatFishColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // HabitatBottomPanel
            // 
            HabitatBottomPanel.AutoSize = true;
            HabitatBottomPanel.ColumnCount = 6;
            HabitatBottomPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            HabitatBottomPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            HabitatBottomPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            HabitatBottomPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            HabitatBottomPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            HabitatBottomPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            HabitatBottomPanel.Controls.Add(FLP_HabitatActions, 5, 0);
            HabitatBottomPanel.Controls.Add(CHK_HabitatTutorialViewed, 0, 0);
            HabitatBottomPanel.Controls.Add(CHK_HabitatTutorialCompleteCapture, 1, 0);
            HabitatBottomPanel.Controls.Add(L_Unknown90, 0, 1);
            HabitatBottomPanel.Controls.Add(NUD_Unknown90, 1, 1);
            HabitatBottomPanel.Controls.Add(L_Unknown92, 2, 1);
            HabitatBottomPanel.Controls.Add(NUD_Unknown92, 3, 1);
            HabitatBottomPanel.Controls.Add(L_LastEncounterType, 0, 2);
            HabitatBottomPanel.Controls.Add(CB_LastEncounterType, 1, 2);
            HabitatBottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            HabitatBottomPanel.Location = new System.Drawing.Point(3, 384);
            HabitatBottomPanel.Name = "HabitatBottomPanel";
            HabitatBottomPanel.Padding = new System.Windows.Forms.Padding(8);
            HabitatBottomPanel.RowCount = 3;
            HabitatBottomPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            HabitatBottomPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            HabitatBottomPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            HabitatBottomPanel.Size = new System.Drawing.Size(970, 105);
            HabitatBottomPanel.TabIndex = 1;
            // 
            // FLP_HabitatActions
            // 
            FLP_HabitatActions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            FLP_HabitatActions.AutoSize = true;
            FLP_HabitatActions.Controls.Add(B_HabitatClear);
            FLP_HabitatActions.Controls.Add(B_HabitatSetComplete);
            FLP_HabitatActions.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            FLP_HabitatActions.Location = new System.Drawing.Point(810, 8);
            FLP_HabitatActions.Margin = new System.Windows.Forms.Padding(0);
            FLP_HabitatActions.Name = "FLP_HabitatActions";
            HabitatBottomPanel.SetRowSpan(FLP_HabitatActions, 3);
            FLP_HabitatActions.Size = new System.Drawing.Size(152, 27);
            FLP_HabitatActions.TabIndex = 8;
            FLP_HabitatActions.WrapContents = false;
            // 
            // B_HabitatClear
            // 
            B_HabitatClear.AutoSize = true;
            B_HabitatClear.Location = new System.Drawing.Point(104, 0);
            B_HabitatClear.Margin = new System.Windows.Forms.Padding(0);
            B_HabitatClear.Name = "B_HabitatClear";
            B_HabitatClear.Size = new System.Drawing.Size(48, 27);
            B_HabitatClear.TabIndex = 0;
            B_HabitatClear.Text = "Clear";
            B_HabitatClear.UseVisualStyleBackColor = true;
            B_HabitatClear.Click += B_HabitatClear_Click;
            // 
            // B_HabitatSetComplete
            // 
            B_HabitatSetComplete.AutoSize = true;
            B_HabitatSetComplete.Location = new System.Drawing.Point(0, 0);
            B_HabitatSetComplete.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            B_HabitatSetComplete.Name = "B_HabitatSetComplete";
            B_HabitatSetComplete.Size = new System.Drawing.Size(96, 27);
            B_HabitatSetComplete.TabIndex = 1;
            B_HabitatSetComplete.Text = "Set Complete";
            B_HabitatSetComplete.UseVisualStyleBackColor = true;
            B_HabitatSetComplete.Click += B_HabitatSetComplete_Click;
            // 
            // CHK_HabitatTutorialViewed
            // 
            CHK_HabitatTutorialViewed.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CHK_HabitatTutorialViewed.AutoSize = true;
            CHK_HabitatTutorialViewed.Location = new System.Drawing.Point(11, 11);
            CHK_HabitatTutorialViewed.Name = "CHK_HabitatTutorialViewed";
            CHK_HabitatTutorialViewed.Size = new System.Drawing.Size(117, 21);
            CHK_HabitatTutorialViewed.TabIndex = 0;
            CHK_HabitatTutorialViewed.Text = "Tutorial Viewed";
            CHK_HabitatTutorialViewed.UseVisualStyleBackColor = true;
            // 
            // CHK_HabitatTutorialCompleteCapture
            // 
            CHK_HabitatTutorialCompleteCapture.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CHK_HabitatTutorialCompleteCapture.AutoSize = true;
            HabitatBottomPanel.SetColumnSpan(CHK_HabitatTutorialCompleteCapture, 2);
            CHK_HabitatTutorialCompleteCapture.Location = new System.Drawing.Point(144, 11);
            CHK_HabitatTutorialCompleteCapture.Name = "CHK_HabitatTutorialCompleteCapture";
            CHK_HabitatTutorialCompleteCapture.Size = new System.Drawing.Size(156, 21);
            CHK_HabitatTutorialCompleteCapture.TabIndex = 1;
            CHK_HabitatTutorialCompleteCapture.Text = "Tutorial Capture Done";
            CHK_HabitatTutorialCompleteCapture.UseVisualStyleBackColor = true;
            // 
            // L_Unknown90
            // 
            L_Unknown90.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Unknown90.AutoSize = true;
            L_Unknown90.Location = new System.Drawing.Point(60, 42);
            L_Unknown90.Name = "L_Unknown90";
            L_Unknown90.Size = new System.Drawing.Size(78, 17);
            L_Unknown90.TabIndex = 2;
            L_Unknown90.Text = "Unknown90:";
            // 
            // NUD_Unknown90
            // 
            NUD_Unknown90.Location = new System.Drawing.Point(144, 38);
            NUD_Unknown90.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NUD_Unknown90.Name = "NUD_Unknown90";
            NUD_Unknown90.Size = new System.Drawing.Size(64, 25);
            NUD_Unknown90.TabIndex = 3;
            // 
            // L_Unknown92
            // 
            L_Unknown92.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Unknown92.AutoSize = true;
            L_Unknown92.Location = new System.Drawing.Point(246, 42);
            L_Unknown92.Name = "L_Unknown92";
            L_Unknown92.Size = new System.Drawing.Size(78, 17);
            L_Unknown92.TabIndex = 4;
            L_Unknown92.Text = "Unknown92:";
            // 
            // NUD_Unknown92
            // 
            NUD_Unknown92.Location = new System.Drawing.Point(330, 38);
            NUD_Unknown92.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_Unknown92.Name = "NUD_Unknown92";
            NUD_Unknown92.Size = new System.Drawing.Size(48, 25);
            NUD_Unknown92.TabIndex = 5;
            // 
            // L_LastEncounterType
            // 
            L_LastEncounterType.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_LastEncounterType.AutoSize = true;
            L_LastEncounterType.Location = new System.Drawing.Point(11, 73);
            L_LastEncounterType.Name = "L_LastEncounterType";
            L_LastEncounterType.Size = new System.Drawing.Size(127, 17);
            L_LastEncounterType.TabIndex = 6;
            L_LastEncounterType.Text = "Last Encounter Type:";
            // 
            // CB_LastEncounterType
            // 
            CB_LastEncounterType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            HabitatBottomPanel.SetColumnSpan(CB_LastEncounterType, 2);
            CB_LastEncounterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_LastEncounterType.FormattingEnabled = true;
            CB_LastEncounterType.Location = new System.Drawing.Point(144, 69);
            CB_LastEncounterType.Name = "CB_LastEncounterType";
            CB_LastEncounterType.Size = new System.Drawing.Size(180, 25);
            CB_LastEncounterType.TabIndex = 7;
            // 
            // ButtonPanel
            // 
            ButtonPanel.AutoSize = true;
            ButtonPanel.Controls.Add(B_Save);
            ButtonPanel.Controls.Add(B_Cancel);
            ButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            ButtonPanel.Location = new System.Drawing.Point(0, 522);
            ButtonPanel.Name = "ButtonPanel";
            ButtonPanel.Padding = new System.Windows.Forms.Padding(8);
            ButtonPanel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            ButtonPanel.Size = new System.Drawing.Size(984, 49);
            ButtonPanel.TabIndex = 1;
            ButtonPanel.WrapContents = false;
            // 
            // B_Save
            // 
            B_Save.AutoSize = true;
            B_Save.Location = new System.Drawing.Point(913, 11);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(52, 27);
            B_Save.TabIndex = 0;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.AutoSize = true;
            B_Cancel.Location = new System.Drawing.Point(844, 11);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(63, 27);
            B_Cancel.TabIndex = 1;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // SAV_Medals5
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(984, 571);
            Controls.Add(TC_Main);
            Controls.Add(ButtonPanel);
            Icon = Properties.Resources.Icon;
            MaximizeBox = false;
            MinimumSize = new System.Drawing.Size(760, 420);
            Name = "SAV_Medals5";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Medals";
            TC_Main.ResumeLayout(false);
            Tab_Medals.ResumeLayout(false);
            Tab_Medals.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_Medals).EndInit();
            MedalSettingsPanel.ResumeLayout(false);
            MedalSettingsPanel.PerformLayout();
            MedalButtonPanel.ResumeLayout(false);
            MedalButtonPanel.PerformLayout();
            Tab_Habitat.ResumeLayout(false);
            Tab_Habitat.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_Habitat).EndInit();
            HabitatBottomPanel.ResumeLayout(false);
            HabitatBottomPanel.PerformLayout();
            FLP_HabitatActions.ResumeLayout(false);
            FLP_HabitatActions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Unknown90).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Unknown92).EndInit();
            ButtonPanel.ResumeLayout(false);
            ButtonPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TabControl TC_Main;
        private System.Windows.Forms.TabPage Tab_Medals;
        private PKHeX.WinForms.Controls.DoubleBufferedDataGridView DGV_Medals;
        private System.Windows.Forms.FlowLayoutPanel MedalButtonPanel;
        private System.Windows.Forms.Button B_ExportAll;
        private System.Windows.Forms.Button B_ImportAll;
        private System.Windows.Forms.Button B_GiveAll;
        private System.Windows.Forms.TableLayoutPanel MedalSettingsPanel;
        private System.Windows.Forms.Label L_PinnedMedal;
        private System.Windows.Forms.ComboBox CB_PinnedMedal;
        private System.Windows.Forms.Label L_Rank;
        private System.Windows.Forms.ComboBox CB_Rank;
        private System.Windows.Forms.CheckBox CHK_TutorialComplete;
        private System.Windows.Forms.TabPage Tab_Habitat;
        private PKHeX.WinForms.Controls.DoubleBufferedDataGridView DGV_Habitat;
        private System.Windows.Forms.TableLayoutPanel HabitatBottomPanel;
        private System.Windows.Forms.FlowLayoutPanel FLP_HabitatActions;
        private System.Windows.Forms.Button B_HabitatClear;
        private System.Windows.Forms.Button B_HabitatSetComplete;
        private System.Windows.Forms.CheckBox CHK_HabitatTutorialViewed;
        private System.Windows.Forms.CheckBox CHK_HabitatTutorialCompleteCapture;
        private System.Windows.Forms.Label L_Unknown90;
        private System.Windows.Forms.NumericUpDown NUD_Unknown90;
        private System.Windows.Forms.Label L_Unknown92;
        private System.Windows.Forms.NumericUpDown NUD_Unknown92;
        private System.Windows.Forms.Label L_LastEncounterType;
        private System.Windows.Forms.ComboBox CB_LastEncounterType;
        private System.Windows.Forms.FlowLayoutPanel ButtonPanel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.DataGridViewTextBoxColumn MedalIndexColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MedalNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MedalTypeColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn MedalStateColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn MedalUnreadColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MedalDateColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn HabitatIndexColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn HabitatCompleteColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn HabitatGrassColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn HabitatSurfColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn HabitatFishColumn;
    }
}
