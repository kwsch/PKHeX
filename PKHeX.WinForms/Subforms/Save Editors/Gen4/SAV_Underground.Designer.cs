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
            LU_PlayersMet = new System.Windows.Forms.Label();
            U_PlayersMet = new System.Windows.Forms.NumericUpDown();
            LU_Gifts = new System.Windows.Forms.Label();
            U_Gifts = new System.Windows.Forms.NumericUpDown();
            LU_Spheres = new System.Windows.Forms.Label();
            U_Spheres = new System.Windows.Forms.NumericUpDown();
            LU_Fossils = new System.Windows.Forms.Label();
            U_Fossils = new System.Windows.Forms.NumericUpDown();
            LU_TrapsA = new System.Windows.Forms.Label();
            U_TrapsA = new System.Windows.Forms.NumericUpDown();
            LU_TrapsT = new System.Windows.Forms.Label();
            U_TrapsT = new System.Windows.Forms.NumericUpDown();
            LU_Flags = new System.Windows.Forms.Label();
            U_Flags = new System.Windows.Forms.NumericUpDown();
            GB_UScores = new System.Windows.Forms.GroupBox();
            TC_UGItems = new System.Windows.Forms.TabControl();
            TB_UGGoods = new System.Windows.Forms.TabPage();
            DGV_UGGoods = new System.Windows.Forms.DataGridView();
            Item_Goods = new System.Windows.Forms.DataGridViewComboBoxColumn();
            TB_UGSpheres = new System.Windows.Forms.TabPage();
            DGV_UGSpheres = new System.Windows.Forms.DataGridView();
            Item_Spheres = new System.Windows.Forms.DataGridViewComboBoxColumn();
            Size_Spheres = new System.Windows.Forms.DataGridViewTextBoxColumn();
            TB_UGTraps = new System.Windows.Forms.TabPage();
            DGV_UGTraps = new System.Windows.Forms.DataGridView();
            Item_Traps = new System.Windows.Forms.DataGridViewComboBoxColumn();
            TB_UGTreasures = new System.Windows.Forms.TabPage();
            DGV_UGTreasures = new System.Windows.Forms.DataGridView();
            Item_Treasures = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ((System.ComponentModel.ISupportInitialize)U_PlayersMet).BeginInit();
            ((System.ComponentModel.ISupportInitialize)U_Gifts).BeginInit();
            ((System.ComponentModel.ISupportInitialize)U_Spheres).BeginInit();
            ((System.ComponentModel.ISupportInitialize)U_Fossils).BeginInit();
            ((System.ComponentModel.ISupportInitialize)U_TrapsA).BeginInit();
            ((System.ComponentModel.ISupportInitialize)U_TrapsT).BeginInit();
            ((System.ComponentModel.ISupportInitialize)U_Flags).BeginInit();
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
            SuspendLayout();
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(411, 232);
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
            B_Cancel.Location = new System.Drawing.Point(316, 232);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(88, 27);
            B_Cancel.TabIndex = 25;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // LU_PlayersMet
            // 
            LU_PlayersMet.Location = new System.Drawing.Point(9, 23);
            LU_PlayersMet.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            LU_PlayersMet.Name = "LU_PlayersMet";
            LU_PlayersMet.Size = new System.Drawing.Size(124, 21);
            LU_PlayersMet.TabIndex = 3;
            LU_PlayersMet.Text = "Players Met";
            LU_PlayersMet.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // U_PlayersMet
            // 
            U_PlayersMet.Location = new System.Drawing.Point(138, 23);
            U_PlayersMet.Margin = new System.Windows.Forms.Padding(2);
            U_PlayersMet.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            U_PlayersMet.Name = "U_PlayersMet";
            U_PlayersMet.Size = new System.Drawing.Size(83, 23);
            U_PlayersMet.TabIndex = 1;
            // 
            // LU_Gifts
            // 
            LU_Gifts.Location = new System.Drawing.Point(9, 48);
            LU_Gifts.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            LU_Gifts.Name = "LU_Gifts";
            LU_Gifts.Size = new System.Drawing.Size(124, 21);
            LU_Gifts.TabIndex = 5;
            LU_Gifts.Text = "Gifts Given";
            LU_Gifts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // U_Gifts
            // 
            U_Gifts.Location = new System.Drawing.Point(138, 48);
            U_Gifts.Margin = new System.Windows.Forms.Padding(2);
            U_Gifts.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            U_Gifts.Name = "U_Gifts";
            U_Gifts.Size = new System.Drawing.Size(83, 23);
            U_Gifts.TabIndex = 2;
            // 
            // LU_Spheres
            // 
            LU_Spheres.Location = new System.Drawing.Point(9, 75);
            LU_Spheres.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            LU_Spheres.Name = "LU_Spheres";
            LU_Spheres.Size = new System.Drawing.Size(124, 21);
            LU_Spheres.TabIndex = 7;
            LU_Spheres.Text = "Spheres Obtained";
            LU_Spheres.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // U_Spheres
            // 
            U_Spheres.Location = new System.Drawing.Point(138, 75);
            U_Spheres.Margin = new System.Windows.Forms.Padding(2);
            U_Spheres.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            U_Spheres.Name = "U_Spheres";
            U_Spheres.Size = new System.Drawing.Size(83, 23);
            U_Spheres.TabIndex = 3;
            // 
            // LU_Fossils
            // 
            LU_Fossils.Location = new System.Drawing.Point(9, 102);
            LU_Fossils.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            LU_Fossils.Name = "LU_Fossils";
            LU_Fossils.Size = new System.Drawing.Size(124, 21);
            LU_Fossils.TabIndex = 9;
            LU_Fossils.Text = "Fossils Obtained";
            LU_Fossils.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // U_Fossils
            // 
            U_Fossils.Location = new System.Drawing.Point(138, 102);
            U_Fossils.Margin = new System.Windows.Forms.Padding(2);
            U_Fossils.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            U_Fossils.Name = "U_Fossils";
            U_Fossils.Size = new System.Drawing.Size(83, 23);
            U_Fossils.TabIndex = 4;
            // 
            // LU_TrapsA
            // 
            LU_TrapsA.Location = new System.Drawing.Point(9, 127);
            LU_TrapsA.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            LU_TrapsA.Name = "LU_TrapsA";
            LU_TrapsA.Size = new System.Drawing.Size(124, 21);
            LU_TrapsA.TabIndex = 11;
            LU_TrapsA.Text = "Traps Avoided";
            LU_TrapsA.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // U_TrapsA
            // 
            U_TrapsA.Location = new System.Drawing.Point(138, 127);
            U_TrapsA.Margin = new System.Windows.Forms.Padding(2);
            U_TrapsA.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            U_TrapsA.Name = "U_TrapsA";
            U_TrapsA.Size = new System.Drawing.Size(83, 23);
            U_TrapsA.TabIndex = 5;
            // 
            // LU_TrapsT
            // 
            LU_TrapsT.Location = new System.Drawing.Point(9, 153);
            LU_TrapsT.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            LU_TrapsT.Name = "LU_TrapsT";
            LU_TrapsT.Size = new System.Drawing.Size(124, 21);
            LU_TrapsT.TabIndex = 13;
            LU_TrapsT.Text = "Traps Triggered";
            LU_TrapsT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // U_TrapsT
            // 
            U_TrapsT.Location = new System.Drawing.Point(138, 153);
            U_TrapsT.Margin = new System.Windows.Forms.Padding(2);
            U_TrapsT.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            U_TrapsT.Name = "U_TrapsT";
            U_TrapsT.Size = new System.Drawing.Size(83, 23);
            U_TrapsT.TabIndex = 6;
            // 
            // LU_Flags
            // 
            LU_Flags.Location = new System.Drawing.Point(9, 179);
            LU_Flags.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            LU_Flags.Name = "LU_Flags";
            LU_Flags.Size = new System.Drawing.Size(124, 21);
            LU_Flags.TabIndex = 15;
            LU_Flags.Text = "Flags Captured";
            LU_Flags.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // U_Flags
            // 
            U_Flags.Location = new System.Drawing.Point(138, 179);
            U_Flags.Margin = new System.Windows.Forms.Padding(2);
            U_Flags.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            U_Flags.Name = "U_Flags";
            U_Flags.Size = new System.Drawing.Size(83, 23);
            U_Flags.TabIndex = 7;
            // 
            // GB_UScores
            // 
            GB_UScores.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            GB_UScores.Controls.Add(U_Flags);
            GB_UScores.Controls.Add(LU_Flags);
            GB_UScores.Controls.Add(U_TrapsT);
            GB_UScores.Controls.Add(LU_TrapsT);
            GB_UScores.Controls.Add(U_TrapsA);
            GB_UScores.Controls.Add(LU_TrapsA);
            GB_UScores.Controls.Add(U_Fossils);
            GB_UScores.Controls.Add(LU_Fossils);
            GB_UScores.Controls.Add(U_Spheres);
            GB_UScores.Controls.Add(LU_Spheres);
            GB_UScores.Controls.Add(U_Gifts);
            GB_UScores.Controls.Add(LU_Gifts);
            GB_UScores.Controls.Add(U_PlayersMet);
            GB_UScores.Controls.Add(LU_PlayersMet);
            GB_UScores.Location = new System.Drawing.Point(12, 13);
            GB_UScores.Margin = new System.Windows.Forms.Padding(2);
            GB_UScores.Name = "GB_UScores";
            GB_UScores.Padding = new System.Windows.Forms.Padding(2);
            GB_UScores.Size = new System.Drawing.Size(231, 209);
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
            TC_UGItems.Size = new System.Drawing.Size(248, 209);
            TC_UGItems.TabIndex = 28;
            // 
            // TB_UGGoods
            // 
            TB_UGGoods.Controls.Add(DGV_UGGoods);
            TB_UGGoods.Location = new System.Drawing.Point(4, 24);
            TB_UGGoods.Margin = new System.Windows.Forms.Padding(2);
            TB_UGGoods.Name = "TB_UGGoods";
            TB_UGGoods.Padding = new System.Windows.Forms.Padding(2);
            TB_UGGoods.Size = new System.Drawing.Size(240, 181);
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
            DGV_UGGoods.Size = new System.Drawing.Size(236, 177);
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
            TB_UGSpheres.Size = new System.Drawing.Size(240, 181);
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
            DGV_UGSpheres.Size = new System.Drawing.Size(236, 177);
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
            TB_UGTraps.Size = new System.Drawing.Size(240, 181);
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
            DGV_UGTraps.Size = new System.Drawing.Size(236, 177);
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
            TB_UGTreasures.Size = new System.Drawing.Size(240, 181);
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
            DGV_UGTreasures.Size = new System.Drawing.Size(236, 177);
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
            // SAV_Underground
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(506, 267);
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
            ((System.ComponentModel.ISupportInitialize)U_PlayersMet).EndInit();
            ((System.ComponentModel.ISupportInitialize)U_Gifts).EndInit();
            ((System.ComponentModel.ISupportInitialize)U_Spheres).EndInit();
            ((System.ComponentModel.ISupportInitialize)U_Fossils).EndInit();
            ((System.ComponentModel.ISupportInitialize)U_TrapsA).EndInit();
            ((System.ComponentModel.ISupportInitialize)U_TrapsT).EndInit();
            ((System.ComponentModel.ISupportInitialize)U_Flags).EndInit();
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
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Label LU_PlayersMet;
        private System.Windows.Forms.NumericUpDown U_PlayersMet;
        private System.Windows.Forms.Label LU_Gifts;
        private System.Windows.Forms.NumericUpDown U_Gifts;
        private System.Windows.Forms.Label LU_Spheres;
        private System.Windows.Forms.NumericUpDown U_Spheres;
        private System.Windows.Forms.Label LU_Fossils;
        private System.Windows.Forms.NumericUpDown U_Fossils;
        private System.Windows.Forms.Label LU_TrapsA;
        private System.Windows.Forms.NumericUpDown U_TrapsA;
        private System.Windows.Forms.Label LU_TrapsT;
        private System.Windows.Forms.NumericUpDown U_TrapsT;
        private System.Windows.Forms.Label LU_Flags;
        private System.Windows.Forms.NumericUpDown U_Flags;
        private System.Windows.Forms.GroupBox GB_UScores;
        private System.Windows.Forms.TabControl TC_UGItems;
        private System.Windows.Forms.TabPage TB_UGGoods;
        private System.Windows.Forms.TabPage TB_UGSpheres;
        private System.Windows.Forms.TabPage TB_UGTraps;
        private System.Windows.Forms.TabPage TB_UGTreasures;
        private System.Windows.Forms.DataGridView DGV_UGGoods;
        private System.Windows.Forms.DataGridView DGV_UGTraps;
        private System.Windows.Forms.DataGridView DGV_UGTreasures;
        private System.Windows.Forms.DataGridView DGV_UGSpheres;
        private System.Windows.Forms.DataGridViewComboBoxColumn Item_Goods;
        private System.Windows.Forms.DataGridViewComboBoxColumn Item_Traps;
        private System.Windows.Forms.DataGridViewComboBoxColumn Item_Treasures;
        private System.Windows.Forms.DataGridViewComboBoxColumn Item_Spheres;
        private System.Windows.Forms.DataGridViewTextBoxColumn Size_Spheres;
    }
}
