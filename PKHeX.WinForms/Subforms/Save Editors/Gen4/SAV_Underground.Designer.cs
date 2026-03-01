using PKHeX.WinForms.Controls;

namespace PKHeX.WinForms
{
    partial class SAV_Underground
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            B_Save = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            L_PeopleMet = new System.Windows.Forms.Label();
            NUD_PlayersMet = new System.Windows.Forms.NumericUpDown();
            L_Gifts = new System.Windows.Forms.Label();
            NUD_GiftsGiven = new System.Windows.Forms.NumericUpDown();
            L_Spheres = new System.Windows.Forms.Label();
            NUD_Spheres = new System.Windows.Forms.NumericUpDown();
            L_Fossils = new System.Windows.Forms.Label();
            NUD_Fossils = new System.Windows.Forms.NumericUpDown();
            L_TrapOthers = new System.Windows.Forms.Label();
            NUD_TrapPlayers = new System.Windows.Forms.NumericUpDown();
            L_TrapSelf = new System.Windows.Forms.Label();
            NUD_TrapSelf = new System.Windows.Forms.NumericUpDown();
            L_FlagsObtained = new System.Windows.Forms.Label();
            NUD_FlagsObtained = new System.Windows.Forms.NumericUpDown();
            GB_UScores = new System.Windows.Forms.GroupBox();
            TC_UGItems = new System.Windows.Forms.TabControl();
            TB_UGGoods = new System.Windows.Forms.TabPage();
            Item_Goods = new System.Windows.Forms.DataGridViewComboBoxColumn();
            DGV_UGGoods = new DoubleBufferedDataGridView();
            TB_UGSpheres = new System.Windows.Forms.TabPage();
            Item_Spheres = new System.Windows.Forms.DataGridViewComboBoxColumn();
            Size_Spheres = new System.Windows.Forms.DataGridViewTextBoxColumn();
            DGV_UGSpheres = new DoubleBufferedDataGridView();
            TB_UGTraps = new System.Windows.Forms.TabPage();
            Item_Traps = new System.Windows.Forms.DataGridViewComboBoxColumn();
            DGV_UGTraps = new DoubleBufferedDataGridView();
            TB_UGTreasures = new System.Windows.Forms.TabPage();
            Item_Treasures = new System.Windows.Forms.DataGridViewComboBoxColumn();
            DGV_UGTreasures = new DoubleBufferedDataGridView();
            NUD_MyFlagRecovered = new System.Windows.Forms.NumericUpDown();
            L_MyFlagRecovered = new System.Windows.Forms.Label();
            NUD_MyBaseMoved = new System.Windows.Forms.NumericUpDown();
            L_MyBaseMoved = new System.Windows.Forms.Label();
            NUD_FlagsCaptured = new System.Windows.Forms.NumericUpDown();
            L_FlagsCaptured = new System.Windows.Forms.Label();
            NUD_MyFlagTaken = new System.Windows.Forms.NumericUpDown();
            L_MyFlagTaken = new System.Windows.Forms.Label();
            NUD_GiftsReceived = new System.Windows.Forms.NumericUpDown();
            L_GiftsReceived = new System.Windows.Forms.Label();
            NUD_HelpedOthers = new System.Windows.Forms.NumericUpDown();
            L_OthersHelped = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)NUD_PlayersMet).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_GiftsGiven).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Spheres).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Fossils).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_TrapPlayers).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_TrapSelf).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_FlagsObtained).BeginInit();
            GB_UScores.SuspendLayout();
            TC_UGItems.SuspendLayout();
            TB_UGGoods.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_UGGoods).BeginInit();
            TB_UGSpheres.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_UGSpheres).BeginInit();
            TB_UGTraps.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_UGTraps).BeginInit();
            TB_UGTreasures.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_UGTreasures).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_MyFlagRecovered).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_MyBaseMoved).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_FlagsCaptured).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_MyFlagTaken).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_GiftsReceived).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_HelpedOthers).BeginInit();
            SuspendLayout();
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(411, 339);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(88, 27);
            B_Save.TabIndex = 26;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(316, 339);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(88, 27);
            B_Cancel.TabIndex = 25;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // L_PeopleMet
            // 
            L_PeopleMet.Location = new System.Drawing.Point(9, 17);
            L_PeopleMet.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            L_PeopleMet.Name = "L_PeopleMet";
            L_PeopleMet.Size = new System.Drawing.Size(124, 21);
            L_PeopleMet.TabIndex = 1;
            L_PeopleMet.Text = "People Met:";
            L_PeopleMet.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_PlayersMet
            // 
            NUD_PlayersMet.Location = new System.Drawing.Point(138, 17);
            NUD_PlayersMet.Margin = new System.Windows.Forms.Padding(2);
            NUD_PlayersMet.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            NUD_PlayersMet.Name = "NUD_PlayersMet";
            NUD_PlayersMet.Size = new System.Drawing.Size(83, 23);
            NUD_PlayersMet.TabIndex = 2;
            // 
            // L_Gifts
            // 
            L_Gifts.Location = new System.Drawing.Point(9, 42);
            L_Gifts.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            L_Gifts.Name = "L_Gifts";
            L_Gifts.Size = new System.Drawing.Size(124, 21);
            L_Gifts.TabIndex = 3;
            L_Gifts.Text = "Gifts Given:";
            L_Gifts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_GiftsGiven
            // 
            NUD_GiftsGiven.Location = new System.Drawing.Point(138, 42);
            NUD_GiftsGiven.Margin = new System.Windows.Forms.Padding(2);
            NUD_GiftsGiven.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            NUD_GiftsGiven.Name = "NUD_GiftsGiven";
            NUD_GiftsGiven.Size = new System.Drawing.Size(83, 23);
            NUD_GiftsGiven.TabIndex = 4;
            // 
            // L_Spheres
            // 
            L_Spheres.Location = new System.Drawing.Point(9, 92);
            L_Spheres.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            L_Spheres.Name = "L_Spheres";
            L_Spheres.Size = new System.Drawing.Size(124, 21);
            L_Spheres.TabIndex = 7;
            L_Spheres.Text = "Spheres Dug:";
            L_Spheres.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_Spheres
            // 
            NUD_Spheres.Location = new System.Drawing.Point(138, 92);
            NUD_Spheres.Margin = new System.Windows.Forms.Padding(2);
            NUD_Spheres.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            NUD_Spheres.Name = "NUD_Spheres";
            NUD_Spheres.Size = new System.Drawing.Size(83, 23);
            NUD_Spheres.TabIndex = 8;
            // 
            // L_Fossils
            // 
            L_Fossils.Location = new System.Drawing.Point(9, 117);
            L_Fossils.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            L_Fossils.Name = "L_Fossils";
            L_Fossils.Size = new System.Drawing.Size(124, 21);
            L_Fossils.TabIndex = 9;
            L_Fossils.Text = "Fossils Dug:";
            L_Fossils.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // U_Fossils
            // 
            NUD_Fossils.Location = new System.Drawing.Point(138, 117);
            NUD_Fossils.Margin = new System.Windows.Forms.Padding(2);
            NUD_Fossils.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            NUD_Fossils.Name = "U_Fossils";
            NUD_Fossils.Size = new System.Drawing.Size(83, 23);
            NUD_Fossils.TabIndex = 10;
            // 
            // L_TrapOthers
            // 
            L_TrapOthers.Location = new System.Drawing.Point(9, 142);
            L_TrapOthers.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            L_TrapOthers.Name = "L_TrapOthers";
            L_TrapOthers.Size = new System.Drawing.Size(124, 21);
            L_TrapOthers.TabIndex = 11;
            L_TrapOthers.Text = "Trap Hits (Players):";
            L_TrapOthers.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_TrapPlayers
            // 
            NUD_TrapPlayers.Location = new System.Drawing.Point(138, 142);
            NUD_TrapPlayers.Margin = new System.Windows.Forms.Padding(2);
            NUD_TrapPlayers.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            NUD_TrapPlayers.Name = "NUD_TrapPlayers";
            NUD_TrapPlayers.Size = new System.Drawing.Size(83, 23);
            NUD_TrapPlayers.TabIndex = 12;
            // 
            // L_TrapSelf
            // 
            L_TrapSelf.Location = new System.Drawing.Point(9, 167);
            L_TrapSelf.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            L_TrapSelf.Name = "L_TrapSelf";
            L_TrapSelf.Size = new System.Drawing.Size(124, 21);
            L_TrapSelf.TabIndex = 13;
            L_TrapSelf.Text = "Trap Hits (Self):";
            L_TrapSelf.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_TrapSelf
            // 
            NUD_TrapSelf.Location = new System.Drawing.Point(138, 167);
            NUD_TrapSelf.Margin = new System.Windows.Forms.Padding(2);
            NUD_TrapSelf.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            NUD_TrapSelf.Name = "NUD_TrapSelf";
            NUD_TrapSelf.Size = new System.Drawing.Size(83, 23);
            NUD_TrapSelf.TabIndex = 14;
            // 
            // L_FlagsObtained
            // 
            L_FlagsObtained.Location = new System.Drawing.Point(9, 67);
            L_FlagsObtained.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            L_FlagsObtained.Name = "L_FlagsObtained";
            L_FlagsObtained.Size = new System.Drawing.Size(124, 21);
            L_FlagsObtained.TabIndex = 5;
            L_FlagsObtained.Text = "Flags Obtained:";
            L_FlagsObtained.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_FlagsObtained
            // 
            NUD_FlagsObtained.Location = new System.Drawing.Point(138, 67);
            NUD_FlagsObtained.Margin = new System.Windows.Forms.Padding(2);
            NUD_FlagsObtained.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            NUD_FlagsObtained.Name = "NUD_FlagsObtained";
            NUD_FlagsObtained.Size = new System.Drawing.Size(83, 23);
            NUD_FlagsObtained.TabIndex = 6;
            // 
            // GB_UScores
            // 
            GB_UScores.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            GB_UScores.Controls.Add(NUD_HelpedOthers);
            GB_UScores.Controls.Add(L_OthersHelped);
            GB_UScores.Controls.Add(NUD_GiftsReceived);
            GB_UScores.Controls.Add(L_GiftsReceived);
            GB_UScores.Controls.Add(NUD_MyFlagTaken);
            GB_UScores.Controls.Add(L_MyFlagTaken);
            GB_UScores.Controls.Add(NUD_FlagsCaptured);
            GB_UScores.Controls.Add(L_FlagsCaptured);
            GB_UScores.Controls.Add(NUD_MyBaseMoved);
            GB_UScores.Controls.Add(L_MyBaseMoved);
            GB_UScores.Controls.Add(NUD_MyFlagRecovered);
            GB_UScores.Controls.Add(L_MyFlagRecovered);
            GB_UScores.Controls.Add(NUD_FlagsObtained);
            GB_UScores.Controls.Add(L_FlagsObtained);
            GB_UScores.Controls.Add(NUD_TrapSelf);
            GB_UScores.Controls.Add(L_TrapSelf);
            GB_UScores.Controls.Add(NUD_TrapPlayers);
            GB_UScores.Controls.Add(L_TrapOthers);
            GB_UScores.Controls.Add(NUD_Fossils);
            GB_UScores.Controls.Add(L_Fossils);
            GB_UScores.Controls.Add(NUD_Spheres);
            GB_UScores.Controls.Add(L_Spheres);
            GB_UScores.Controls.Add(NUD_GiftsGiven);
            GB_UScores.Controls.Add(L_Gifts);
            GB_UScores.Controls.Add(NUD_PlayersMet);
            GB_UScores.Controls.Add(L_PeopleMet);
            GB_UScores.Location = new System.Drawing.Point(12, 13);
            GB_UScores.Margin = new System.Windows.Forms.Padding(2);
            GB_UScores.Name = "GB_UScores";
            GB_UScores.Padding = new System.Windows.Forms.Padding(2);
            GB_UScores.Size = new System.Drawing.Size(231, 350);
            GB_UScores.TabIndex = 27;
            GB_UScores.TabStop = false;
            GB_UScores.Text = "Scores";
            // 
            // TC_UGItems
            // 
            TC_UGItems.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TC_UGItems.Controls.Add(TB_UGGoods);
            TC_UGItems.Controls.Add(TB_UGSpheres);
            TC_UGItems.Controls.Add(TB_UGTraps);
            TC_UGItems.Controls.Add(TB_UGTreasures);
            TC_UGItems.Location = new System.Drawing.Point(251, 13);
            TC_UGItems.Margin = new System.Windows.Forms.Padding(2);
            TC_UGItems.Name = "TC_UGItems";
            TC_UGItems.SelectedIndex = 0;
            TC_UGItems.Size = new System.Drawing.Size(248, 316);
            TC_UGItems.TabIndex = 28;
            // 
            // TB_UGGoods
            // 
            TB_UGGoods.Controls.Add(DGV_UGGoods);
            TB_UGGoods.Location = new System.Drawing.Point(4, 24);
            TB_UGGoods.Margin = new System.Windows.Forms.Padding(2);
            TB_UGGoods.Name = "TB_UGGoods";
            TB_UGGoods.Padding = new System.Windows.Forms.Padding(2);
            TB_UGGoods.Size = new System.Drawing.Size(240, 288);
            TB_UGGoods.TabIndex = 0;
            TB_UGGoods.Text = "Goods";
            TB_UGGoods.UseVisualStyleBackColor = true;
            // 
            // DGV_UGGoods
            // 
            DGV_UGGoods.AllowUserToAddRows = false;
            DGV_UGGoods.AllowUserToDeleteRows = false;
            DGV_UGGoods.AllowUserToResizeColumns = false;
            DGV_UGGoods.AllowUserToResizeRows = false;
            DGV_UGGoods.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            DGV_UGGoods.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            DGV_UGGoods.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_UGGoods.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Item_Goods });
            DGV_UGGoods.Dock = System.Windows.Forms.DockStyle.Fill;
            DGV_UGGoods.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            DGV_UGGoods.Location = new System.Drawing.Point(2, 2);
            DGV_UGGoods.Margin = new System.Windows.Forms.Padding(2);
            DGV_UGGoods.MultiSelect = false;
            DGV_UGGoods.Name = "DGV_UGGoods";
            DGV_UGGoods.RowHeadersVisible = false;
            DGV_UGGoods.RowHeadersWidth = 51;
            DGV_UGGoods.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            DGV_UGGoods.RowTemplate.Height = 24;
            DGV_UGGoods.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            DGV_UGGoods.ShowEditingIcon = false;
            DGV_UGGoods.Size = new System.Drawing.Size(236, 284);
            DGV_UGGoods.TabIndex = 0;
            // 
            // Item_Goods
            // 
            Item_Goods.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            Item_Goods.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            Item_Goods.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            Item_Goods.HeaderText = "Item";
            Item_Goods.MinimumWidth = 6;
            Item_Goods.Name = "Item_Goods";
            Item_Goods.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            Item_Goods.Width = 175;
            // 
            // TB_UGSpheres
            // 
            TB_UGSpheres.Controls.Add(DGV_UGSpheres);
            TB_UGSpheres.Location = new System.Drawing.Point(4, 24);
            TB_UGSpheres.Margin = new System.Windows.Forms.Padding(2);
            TB_UGSpheres.Name = "TB_UGSpheres";
            TB_UGSpheres.Padding = new System.Windows.Forms.Padding(2);
            TB_UGSpheres.Size = new System.Drawing.Size(240, 288);
            TB_UGSpheres.TabIndex = 1;
            TB_UGSpheres.Text = "Spheres";
            TB_UGSpheres.UseVisualStyleBackColor = true;
            // 
            // DGV_UGSpheres
            // 
            DGV_UGSpheres.AllowUserToAddRows = false;
            DGV_UGSpheres.AllowUserToDeleteRows = false;
            DGV_UGSpheres.AllowUserToResizeColumns = false;
            DGV_UGSpheres.AllowUserToResizeRows = false;
            DGV_UGSpheres.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            DGV_UGSpheres.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            DGV_UGSpheres.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_UGSpheres.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Item_Spheres, Size_Spheres });
            DGV_UGSpheres.Dock = System.Windows.Forms.DockStyle.Fill;
            DGV_UGSpheres.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            DGV_UGSpheres.Location = new System.Drawing.Point(2, 2);
            DGV_UGSpheres.Margin = new System.Windows.Forms.Padding(2);
            DGV_UGSpheres.MultiSelect = false;
            DGV_UGSpheres.Name = "DGV_UGSpheres";
            DGV_UGSpheres.RowHeadersVisible = false;
            DGV_UGSpheres.RowHeadersWidth = 51;
            DGV_UGSpheres.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            DGV_UGSpheres.RowTemplate.Height = 24;
            DGV_UGSpheres.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            DGV_UGSpheres.ShowEditingIcon = false;
            DGV_UGSpheres.Size = new System.Drawing.Size(236, 284);
            DGV_UGSpheres.TabIndex = 1;
            // 
            // Item_Spheres
            // 
            Item_Spheres.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            Item_Spheres.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            Item_Spheres.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            Item_Spheres.HeaderText = "Sphere";
            Item_Spheres.MinimumWidth = 6;
            Item_Spheres.Name = "Item_Spheres";
            Item_Spheres.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            Item_Spheres.Width = 150;
            // 
            // Size_Spheres
            // 
            Size_Spheres.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            Size_Spheres.DefaultCellStyle = dataGridViewCellStyle1;
            Size_Spheres.HeaderText = "Size";
            Size_Spheres.MaxInputLength = 2;
            Size_Spheres.MinimumWidth = 6;
            Size_Spheres.Name = "Size_Spheres";
            Size_Spheres.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            Size_Spheres.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            Size_Spheres.Width = 75;
            // 
            // TB_UGTraps
            // 
            TB_UGTraps.Controls.Add(DGV_UGTraps);
            TB_UGTraps.Location = new System.Drawing.Point(4, 24);
            TB_UGTraps.Margin = new System.Windows.Forms.Padding(2);
            TB_UGTraps.Name = "TB_UGTraps";
            TB_UGTraps.Padding = new System.Windows.Forms.Padding(2);
            TB_UGTraps.Size = new System.Drawing.Size(240, 288);
            TB_UGTraps.TabIndex = 2;
            TB_UGTraps.Text = "Traps";
            TB_UGTraps.UseVisualStyleBackColor = true;
            // 
            // DGV_UGTraps
            // 
            DGV_UGTraps.AllowUserToAddRows = false;
            DGV_UGTraps.AllowUserToDeleteRows = false;
            DGV_UGTraps.AllowUserToResizeColumns = false;
            DGV_UGTraps.AllowUserToResizeRows = false;
            DGV_UGTraps.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            DGV_UGTraps.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            DGV_UGTraps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_UGTraps.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Item_Traps });
            DGV_UGTraps.Dock = System.Windows.Forms.DockStyle.Fill;
            DGV_UGTraps.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            DGV_UGTraps.Location = new System.Drawing.Point(2, 2);
            DGV_UGTraps.Margin = new System.Windows.Forms.Padding(2);
            DGV_UGTraps.MultiSelect = false;
            DGV_UGTraps.Name = "DGV_UGTraps";
            DGV_UGTraps.RowHeadersVisible = false;
            DGV_UGTraps.RowHeadersWidth = 51;
            DGV_UGTraps.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            DGV_UGTraps.RowTemplate.Height = 24;
            DGV_UGTraps.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            DGV_UGTraps.ShowEditingIcon = false;
            DGV_UGTraps.Size = new System.Drawing.Size(236, 284);
            DGV_UGTraps.TabIndex = 1;
            // 
            // Item_Traps
            // 
            Item_Traps.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            Item_Traps.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            Item_Traps.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            Item_Traps.HeaderText = "Item";
            Item_Traps.MinimumWidth = 6;
            Item_Traps.Name = "Item_Traps";
            Item_Traps.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            Item_Traps.Width = 175;
            // 
            // TB_UGTreasures
            // 
            TB_UGTreasures.Controls.Add(DGV_UGTreasures);
            TB_UGTreasures.Location = new System.Drawing.Point(4, 24);
            TB_UGTreasures.Margin = new System.Windows.Forms.Padding(2);
            TB_UGTreasures.Name = "TB_UGTreasures";
            TB_UGTreasures.Padding = new System.Windows.Forms.Padding(2);
            TB_UGTreasures.Size = new System.Drawing.Size(240, 288);
            TB_UGTreasures.TabIndex = 3;
            TB_UGTreasures.Text = "Treasures";
            TB_UGTreasures.UseVisualStyleBackColor = true;
            // 
            // DGV_UGTreasures
            // 
            DGV_UGTreasures.AllowUserToAddRows = false;
            DGV_UGTreasures.AllowUserToDeleteRows = false;
            DGV_UGTreasures.AllowUserToResizeColumns = false;
            DGV_UGTreasures.AllowUserToResizeRows = false;
            DGV_UGTreasures.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            DGV_UGTreasures.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            DGV_UGTreasures.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_UGTreasures.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Item_Treasures });
            DGV_UGTreasures.Dock = System.Windows.Forms.DockStyle.Fill;
            DGV_UGTreasures.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            DGV_UGTreasures.Location = new System.Drawing.Point(2, 2);
            DGV_UGTreasures.Margin = new System.Windows.Forms.Padding(2);
            DGV_UGTreasures.MultiSelect = false;
            DGV_UGTreasures.Name = "DGV_UGTreasures";
            DGV_UGTreasures.RowHeadersVisible = false;
            DGV_UGTreasures.RowHeadersWidth = 51;
            DGV_UGTreasures.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            DGV_UGTreasures.RowTemplate.Height = 24;
            DGV_UGTreasures.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            DGV_UGTreasures.ShowEditingIcon = false;
            DGV_UGTreasures.Size = new System.Drawing.Size(236, 284);
            DGV_UGTreasures.TabIndex = 1;
            // 
            // Item_Treasures
            // 
            Item_Treasures.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            Item_Treasures.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            Item_Treasures.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            Item_Treasures.HeaderText = "Item";
            Item_Treasures.MinimumWidth = 6;
            Item_Treasures.Name = "Item_Treasures";
            Item_Treasures.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            Item_Treasures.Width = 175;
            // 
            // NUD_MyFlagRecovered
            // 
            NUD_MyFlagRecovered.Location = new System.Drawing.Point(138, 267);
            NUD_MyFlagRecovered.Margin = new System.Windows.Forms.Padding(2);
            NUD_MyFlagRecovered.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            NUD_MyFlagRecovered.Name = "NUD_MyFlagRecovered";
            NUD_MyFlagRecovered.Size = new System.Drawing.Size(83, 23);
            NUD_MyFlagRecovered.TabIndex = 22;
            // 
            // L_MyFlagRecovered
            // 
            L_MyFlagRecovered.Location = new System.Drawing.Point(9, 267);
            L_MyFlagRecovered.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            L_MyFlagRecovered.Name = "L_MyFlagRecovered";
            L_MyFlagRecovered.Size = new System.Drawing.Size(124, 21);
            L_MyFlagRecovered.TabIndex = 21;
            L_MyFlagRecovered.Text = "Recovered Flags:";
            L_MyFlagRecovered.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_MyBaseMoved
            // 
            NUD_MyBaseMoved.Location = new System.Drawing.Point(138, 292);
            NUD_MyBaseMoved.Margin = new System.Windows.Forms.Padding(2);
            NUD_MyBaseMoved.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            NUD_MyBaseMoved.Name = "NUD_MyBaseMoved";
            NUD_MyBaseMoved.Size = new System.Drawing.Size(83, 23);
            NUD_MyBaseMoved.TabIndex = 24;
            // 
            // L_MyBaseMoved
            // 
            L_MyBaseMoved.Location = new System.Drawing.Point(9, 292);
            L_MyBaseMoved.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            L_MyBaseMoved.Name = "L_MyBaseMoved";
            L_MyBaseMoved.Size = new System.Drawing.Size(124, 21);
            L_MyBaseMoved.TabIndex = 23;
            L_MyBaseMoved.Text = "Moved My Base:";
            L_MyBaseMoved.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_FlagsCaptured
            // 
            NUD_FlagsCaptured.Location = new System.Drawing.Point(138, 317);
            NUD_FlagsCaptured.Margin = new System.Windows.Forms.Padding(2);
            NUD_FlagsCaptured.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            NUD_FlagsCaptured.Name = "NUD_FlagsCaptured";
            NUD_FlagsCaptured.Size = new System.Drawing.Size(83, 23);
            NUD_FlagsCaptured.TabIndex = 26;
            // 
            // L_FlagsCaptured
            // 
            L_FlagsCaptured.Location = new System.Drawing.Point(9, 317);
            L_FlagsCaptured.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            L_FlagsCaptured.Name = "L_FlagsCaptured";
            L_FlagsCaptured.Size = new System.Drawing.Size(124, 21);
            L_FlagsCaptured.TabIndex = 25;
            L_FlagsCaptured.Text = "Captured Flags:";
            L_FlagsCaptured.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_MyFlagTaken
            // 
            NUD_MyFlagTaken.Location = new System.Drawing.Point(138, 242);
            NUD_MyFlagTaken.Margin = new System.Windows.Forms.Padding(2);
            NUD_MyFlagTaken.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            NUD_MyFlagTaken.Name = "NUD_MyFlagTaken";
            NUD_MyFlagTaken.Size = new System.Drawing.Size(83, 23);
            NUD_MyFlagTaken.TabIndex = 20;
            // 
            // L_MyFlagTaken
            // 
            L_MyFlagTaken.Location = new System.Drawing.Point(9, 242);
            L_MyFlagTaken.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            L_MyFlagTaken.Name = "L_MyFlagTaken";
            L_MyFlagTaken.Size = new System.Drawing.Size(124, 21);
            L_MyFlagTaken.TabIndex = 19;
            L_MyFlagTaken.Text = "My Flag Taken:";
            L_MyFlagTaken.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_GiftsReceived
            // 
            NUD_GiftsReceived.Location = new System.Drawing.Point(138, 217);
            NUD_GiftsReceived.Margin = new System.Windows.Forms.Padding(2);
            NUD_GiftsReceived.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            NUD_GiftsReceived.Name = "NUD_GiftsReceived";
            NUD_GiftsReceived.Size = new System.Drawing.Size(83, 23);
            NUD_GiftsReceived.TabIndex = 18;
            // 
            // L_GiftsReceived
            // 
            L_GiftsReceived.Location = new System.Drawing.Point(9, 217);
            L_GiftsReceived.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            L_GiftsReceived.Name = "L_GiftsReceived";
            L_GiftsReceived.Size = new System.Drawing.Size(124, 21);
            L_GiftsReceived.TabIndex = 17;
            L_GiftsReceived.Text = "Gifts Received:";
            L_GiftsReceived.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_HelpedOthers
            // 
            NUD_HelpedOthers.Location = new System.Drawing.Point(138, 192);
            NUD_HelpedOthers.Margin = new System.Windows.Forms.Padding(2);
            NUD_HelpedOthers.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            NUD_HelpedOthers.Name = "NUD_HelpedOthers";
            NUD_HelpedOthers.Size = new System.Drawing.Size(83, 23);
            NUD_HelpedOthers.TabIndex = 16;
            // 
            // L_OthersHelped
            // 
            L_OthersHelped.Location = new System.Drawing.Point(9, 192);
            L_OthersHelped.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            L_OthersHelped.Name = "L_OthersHelped";
            L_OthersHelped.Size = new System.Drawing.Size(124, 21);
            L_OthersHelped.TabIndex = 15;
            L_OthersHelped.Text = "Others Helped:";
            L_OthersHelped.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SAV_Underground
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(506, 374);
            Controls.Add(TC_UGItems);
            Controls.Add(GB_UScores);
            Controls.Add(B_Cancel);
            Controls.Add(B_Save);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(2);
            MaximizeBox = false;
            MinimumSize = new System.Drawing.Size(522, 306);
            Name = "SAV_Underground";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Underground Editor";
            ((System.ComponentModel.ISupportInitialize)NUD_PlayersMet).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_GiftsGiven).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Spheres).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Fossils).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_TrapPlayers).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_TrapSelf).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_FlagsObtained).EndInit();
            GB_UScores.ResumeLayout(false);
            TC_UGItems.ResumeLayout(false);
            TB_UGGoods.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DGV_UGGoods).EndInit();
            TB_UGSpheres.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DGV_UGSpheres).EndInit();
            TB_UGTraps.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DGV_UGTraps).EndInit();
            TB_UGTreasures.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DGV_UGTreasures).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_MyFlagRecovered).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_MyBaseMoved).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_FlagsCaptured).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_MyFlagTaken).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_GiftsReceived).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_HelpedOthers).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Label L_PeopleMet;
        private System.Windows.Forms.NumericUpDown NUD_PlayersMet;
        private System.Windows.Forms.Label L_Gifts;
        private System.Windows.Forms.NumericUpDown NUD_GiftsGiven;
        private System.Windows.Forms.Label L_Spheres;
        private System.Windows.Forms.NumericUpDown NUD_Spheres;
        private System.Windows.Forms.Label L_Fossils;
        private System.Windows.Forms.NumericUpDown NUD_Fossils;
        private System.Windows.Forms.Label L_TrapOthers;
        private System.Windows.Forms.NumericUpDown NUD_TrapPlayers;
        private System.Windows.Forms.Label L_TrapSelf;
        private System.Windows.Forms.NumericUpDown NUD_TrapSelf;
        private System.Windows.Forms.Label L_FlagsObtained;
        private System.Windows.Forms.NumericUpDown NUD_FlagsObtained;
        private System.Windows.Forms.GroupBox GB_UScores;
        private System.Windows.Forms.TabControl TC_UGItems;
        private System.Windows.Forms.TabPage TB_UGGoods;
        private System.Windows.Forms.TabPage TB_UGSpheres;
        private System.Windows.Forms.TabPage TB_UGTraps;
        private System.Windows.Forms.TabPage TB_UGTreasures;
        private System.Windows.Forms.DataGridViewComboBoxColumn Item_Goods;
        private DoubleBufferedDataGridView DGV_UGGoods;
        private DoubleBufferedDataGridView DGV_UGTraps;
        private DoubleBufferedDataGridView DGV_UGTreasures;
        private DoubleBufferedDataGridView DGV_UGSpheres;
        private System.Windows.Forms.DataGridViewComboBoxColumn Item_Traps;
        private System.Windows.Forms.DataGridViewComboBoxColumn Item_Treasures;
        private System.Windows.Forms.DataGridViewComboBoxColumn Item_Spheres;
        private System.Windows.Forms.DataGridViewTextBoxColumn Size_Spheres;
        private System.Windows.Forms.NumericUpDown NUD_FlagsCaptured;
        private System.Windows.Forms.Label L_FlagsCaptured;
        private System.Windows.Forms.NumericUpDown NUD_MyBaseMoved;
        private System.Windows.Forms.Label L_MyBaseMoved;
        private System.Windows.Forms.NumericUpDown NUD_MyFlagRecovered;
        private System.Windows.Forms.Label L_MyFlagRecovered;
        private System.Windows.Forms.NumericUpDown NUD_MyFlagTaken;
        private System.Windows.Forms.Label L_MyFlagTaken;
        private System.Windows.Forms.NumericUpDown NUD_HelpedOthers;
        private System.Windows.Forms.Label L_OthersHelped;
        private System.Windows.Forms.NumericUpDown NUD_GiftsReceived;
        private System.Windows.Forms.Label L_GiftsReceived;
    }
}
