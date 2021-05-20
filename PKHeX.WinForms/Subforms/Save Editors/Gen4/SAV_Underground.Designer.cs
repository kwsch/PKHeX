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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.B_Save = new System.Windows.Forms.Button();
            this.B_Cancel = new System.Windows.Forms.Button();
            this.LU_PlayersMet = new System.Windows.Forms.Label();
            this.U_PlayersMet = new System.Windows.Forms.NumericUpDown();
            this.LU_Gifts = new System.Windows.Forms.Label();
            this.U_Gifts = new System.Windows.Forms.NumericUpDown();
            this.LU_Spheres = new System.Windows.Forms.Label();
            this.U_Spheres = new System.Windows.Forms.NumericUpDown();
            this.LU_Fossils = new System.Windows.Forms.Label();
            this.U_Fossils = new System.Windows.Forms.NumericUpDown();
            this.LU_TrapsA = new System.Windows.Forms.Label();
            this.U_TrapsA = new System.Windows.Forms.NumericUpDown();
            this.LU_TrapsT = new System.Windows.Forms.Label();
            this.U_TrapsT = new System.Windows.Forms.NumericUpDown();
            this.LU_Flags = new System.Windows.Forms.Label();
            this.U_Flags = new System.Windows.Forms.NumericUpDown();
            this.GB_UScores = new System.Windows.Forms.GroupBox();
            this.TC_UGItems = new System.Windows.Forms.TabControl();
            this.TB_UGGoods = new System.Windows.Forms.TabPage();
            this.DGV_UGGoods = new System.Windows.Forms.DataGridView();
            this.Item_Goods = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.TB_UGSpheres = new System.Windows.Forms.TabPage();
            this.DGV_UGSpheres = new System.Windows.Forms.DataGridView();
            this.Item_Spheres = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Size_Spheres = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TB_UGTraps = new System.Windows.Forms.TabPage();
            this.DGV_UGTraps = new System.Windows.Forms.DataGridView();
            this.Item_Traps = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.TB_UGTreasures = new System.Windows.Forms.TabPage();
            this.DGV_UGTreasures = new System.Windows.Forms.DataGridView();
            this.Item_Treasures = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.U_PlayersMet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.U_Gifts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.U_Spheres)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.U_Fossils)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.U_TrapsA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.U_TrapsT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.U_Flags)).BeginInit();
            this.GB_UScores.SuspendLayout();
            this.TC_UGItems.SuspendLayout();
            this.TB_UGGoods.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_UGGoods)).BeginInit();
            this.TB_UGSpheres.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_UGSpheres)).BeginInit();
            this.TB_UGTraps.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_UGTraps)).BeginInit();
            this.TB_UGTreasures.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_UGTreasures)).BeginInit();
            this.SuspendLayout();
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(352, 201);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(75, 23);
            this.B_Save.TabIndex = 26;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(271, 201);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(75, 23);
            this.B_Cancel.TabIndex = 25;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // LU_PlayersMet
            // 
            this.LU_PlayersMet.Location = new System.Drawing.Point(8, 20);
            this.LU_PlayersMet.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LU_PlayersMet.Name = "LU_PlayersMet";
            this.LU_PlayersMet.Size = new System.Drawing.Size(106, 18);
            this.LU_PlayersMet.TabIndex = 3;
            this.LU_PlayersMet.Text = "Players Met";
            this.LU_PlayersMet.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // U_PlayersMet
            // 
            this.U_PlayersMet.Location = new System.Drawing.Point(118, 20);
            this.U_PlayersMet.Margin = new System.Windows.Forms.Padding(2);
            this.U_PlayersMet.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.U_PlayersMet.Name = "U_PlayersMet";
            this.U_PlayersMet.Size = new System.Drawing.Size(71, 20);
            this.U_PlayersMet.TabIndex = 1;
            // 
            // LU_Gifts
            // 
            this.LU_Gifts.Location = new System.Drawing.Point(8, 42);
            this.LU_Gifts.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LU_Gifts.Name = "LU_Gifts";
            this.LU_Gifts.Size = new System.Drawing.Size(106, 18);
            this.LU_Gifts.TabIndex = 5;
            this.LU_Gifts.Text = "Gifts Given";
            this.LU_Gifts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // U_Gifts
            // 
            this.U_Gifts.Location = new System.Drawing.Point(118, 42);
            this.U_Gifts.Margin = new System.Windows.Forms.Padding(2);
            this.U_Gifts.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.U_Gifts.Name = "U_Gifts";
            this.U_Gifts.Size = new System.Drawing.Size(71, 20);
            this.U_Gifts.TabIndex = 2;
            // 
            // LU_Spheres
            // 
            this.LU_Spheres.Location = new System.Drawing.Point(8, 65);
            this.LU_Spheres.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LU_Spheres.Name = "LU_Spheres";
            this.LU_Spheres.Size = new System.Drawing.Size(106, 18);
            this.LU_Spheres.TabIndex = 7;
            this.LU_Spheres.Text = "Spheres Obtained";
            this.LU_Spheres.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // U_Spheres
            // 
            this.U_Spheres.Location = new System.Drawing.Point(118, 65);
            this.U_Spheres.Margin = new System.Windows.Forms.Padding(2);
            this.U_Spheres.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.U_Spheres.Name = "U_Spheres";
            this.U_Spheres.Size = new System.Drawing.Size(71, 20);
            this.U_Spheres.TabIndex = 3;
            // 
            // LU_Fossils
            // 
            this.LU_Fossils.Location = new System.Drawing.Point(8, 88);
            this.LU_Fossils.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LU_Fossils.Name = "LU_Fossils";
            this.LU_Fossils.Size = new System.Drawing.Size(106, 18);
            this.LU_Fossils.TabIndex = 9;
            this.LU_Fossils.Text = "Fossils Obtained";
            this.LU_Fossils.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // U_Fossils
            // 
            this.U_Fossils.Location = new System.Drawing.Point(118, 88);
            this.U_Fossils.Margin = new System.Windows.Forms.Padding(2);
            this.U_Fossils.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.U_Fossils.Name = "U_Fossils";
            this.U_Fossils.Size = new System.Drawing.Size(71, 20);
            this.U_Fossils.TabIndex = 4;
            // 
            // LU_TrapsA
            // 
            this.LU_TrapsA.Location = new System.Drawing.Point(8, 110);
            this.LU_TrapsA.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LU_TrapsA.Name = "LU_TrapsA";
            this.LU_TrapsA.Size = new System.Drawing.Size(106, 18);
            this.LU_TrapsA.TabIndex = 11;
            this.LU_TrapsA.Text = "Traps Avoided";
            this.LU_TrapsA.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // U_TrapsA
            // 
            this.U_TrapsA.Location = new System.Drawing.Point(118, 110);
            this.U_TrapsA.Margin = new System.Windows.Forms.Padding(2);
            this.U_TrapsA.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.U_TrapsA.Name = "U_TrapsA";
            this.U_TrapsA.Size = new System.Drawing.Size(71, 20);
            this.U_TrapsA.TabIndex = 5;
            // 
            // LU_TrapsT
            // 
            this.LU_TrapsT.Location = new System.Drawing.Point(8, 133);
            this.LU_TrapsT.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LU_TrapsT.Name = "LU_TrapsT";
            this.LU_TrapsT.Size = new System.Drawing.Size(106, 18);
            this.LU_TrapsT.TabIndex = 13;
            this.LU_TrapsT.Text = "Traps Triggered";
            this.LU_TrapsT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // U_TrapsT
            // 
            this.U_TrapsT.Location = new System.Drawing.Point(118, 133);
            this.U_TrapsT.Margin = new System.Windows.Forms.Padding(2);
            this.U_TrapsT.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.U_TrapsT.Name = "U_TrapsT";
            this.U_TrapsT.Size = new System.Drawing.Size(71, 20);
            this.U_TrapsT.TabIndex = 6;
            // 
            // LU_Flags
            // 
            this.LU_Flags.Location = new System.Drawing.Point(8, 155);
            this.LU_Flags.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LU_Flags.Name = "LU_Flags";
            this.LU_Flags.Size = new System.Drawing.Size(106, 18);
            this.LU_Flags.TabIndex = 15;
            this.LU_Flags.Text = "Flags Captured";
            this.LU_Flags.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // U_Flags
            // 
            this.U_Flags.Location = new System.Drawing.Point(118, 155);
            this.U_Flags.Margin = new System.Windows.Forms.Padding(2);
            this.U_Flags.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.U_Flags.Name = "U_Flags";
            this.U_Flags.Size = new System.Drawing.Size(71, 20);
            this.U_Flags.TabIndex = 7;
            // 
            // GB_UScores
            // 
            this.GB_UScores.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.GB_UScores.Controls.Add(this.U_Flags);
            this.GB_UScores.Controls.Add(this.LU_Flags);
            this.GB_UScores.Controls.Add(this.U_TrapsT);
            this.GB_UScores.Controls.Add(this.LU_TrapsT);
            this.GB_UScores.Controls.Add(this.U_TrapsA);
            this.GB_UScores.Controls.Add(this.LU_TrapsA);
            this.GB_UScores.Controls.Add(this.U_Fossils);
            this.GB_UScores.Controls.Add(this.LU_Fossils);
            this.GB_UScores.Controls.Add(this.U_Spheres);
            this.GB_UScores.Controls.Add(this.LU_Spheres);
            this.GB_UScores.Controls.Add(this.U_Gifts);
            this.GB_UScores.Controls.Add(this.LU_Gifts);
            this.GB_UScores.Controls.Add(this.U_PlayersMet);
            this.GB_UScores.Controls.Add(this.LU_PlayersMet);
            this.GB_UScores.Location = new System.Drawing.Point(10, 11);
            this.GB_UScores.Margin = new System.Windows.Forms.Padding(2);
            this.GB_UScores.Name = "GB_UScores";
            this.GB_UScores.Padding = new System.Windows.Forms.Padding(2);
            this.GB_UScores.Size = new System.Drawing.Size(198, 181);
            this.GB_UScores.TabIndex = 27;
            this.GB_UScores.TabStop = false;
            this.GB_UScores.Text = "Scores";
            // 
            // TC_UGItems
            // 
            this.TC_UGItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TC_UGItems.Controls.Add(this.TB_UGGoods);
            this.TC_UGItems.Controls.Add(this.TB_UGSpheres);
            this.TC_UGItems.Controls.Add(this.TB_UGTraps);
            this.TC_UGItems.Controls.Add(this.TB_UGTreasures);
            this.TC_UGItems.Location = new System.Drawing.Point(215, 11);
            this.TC_UGItems.Margin = new System.Windows.Forms.Padding(2);
            this.TC_UGItems.Name = "TC_UGItems";
            this.TC_UGItems.SelectedIndex = 0;
            this.TC_UGItems.Size = new System.Drawing.Size(213, 181);
            this.TC_UGItems.TabIndex = 28;
            // 
            // TB_UGGoods
            // 
            this.TB_UGGoods.Controls.Add(this.DGV_UGGoods);
            this.TB_UGGoods.Location = new System.Drawing.Point(4, 22);
            this.TB_UGGoods.Margin = new System.Windows.Forms.Padding(2);
            this.TB_UGGoods.Name = "TB_UGGoods";
            this.TB_UGGoods.Padding = new System.Windows.Forms.Padding(2);
            this.TB_UGGoods.Size = new System.Drawing.Size(205, 155);
            this.TB_UGGoods.TabIndex = 0;
            this.TB_UGGoods.Text = "Goods";
            this.TB_UGGoods.UseVisualStyleBackColor = true;
            // 
            // DGV_UGGoods
            // 
            this.DGV_UGGoods.AllowUserToAddRows = false;
            this.DGV_UGGoods.AllowUserToDeleteRows = false;
            this.DGV_UGGoods.AllowUserToResizeColumns = false;
            this.DGV_UGGoods.AllowUserToResizeRows = false;
            this.DGV_UGGoods.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.DGV_UGGoods.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.DGV_UGGoods.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_UGGoods.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Item_Goods});
            this.DGV_UGGoods.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DGV_UGGoods.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.DGV_UGGoods.Location = new System.Drawing.Point(2, 2);
            this.DGV_UGGoods.Margin = new System.Windows.Forms.Padding(2);
            this.DGV_UGGoods.MultiSelect = false;
            this.DGV_UGGoods.Name = "DGV_UGGoods";
            this.DGV_UGGoods.RowHeadersVisible = false;
            this.DGV_UGGoods.RowHeadersWidth = 51;
            this.DGV_UGGoods.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.DGV_UGGoods.RowTemplate.Height = 24;
            this.DGV_UGGoods.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.DGV_UGGoods.ShowEditingIcon = false;
            this.DGV_UGGoods.Size = new System.Drawing.Size(201, 151);
            this.DGV_UGGoods.TabIndex = 0;
            // 
            // Item_Goods
            // 
            this.Item_Goods.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Item_Goods.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.Item_Goods.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Item_Goods.HeaderText = "Item";
            this.Item_Goods.MinimumWidth = 6;
            this.Item_Goods.Name = "Item_Goods";
            this.Item_Goods.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Item_Goods.Width = 175;
            // 
            // TB_UGSpheres
            // 
            this.TB_UGSpheres.Controls.Add(this.DGV_UGSpheres);
            this.TB_UGSpheres.Location = new System.Drawing.Point(4, 22);
            this.TB_UGSpheres.Margin = new System.Windows.Forms.Padding(2);
            this.TB_UGSpheres.Name = "TB_UGSpheres";
            this.TB_UGSpheres.Padding = new System.Windows.Forms.Padding(2);
            this.TB_UGSpheres.Size = new System.Drawing.Size(205, 155);
            this.TB_UGSpheres.TabIndex = 1;
            this.TB_UGSpheres.Text = "Spheres";
            this.TB_UGSpheres.UseVisualStyleBackColor = true;
            // 
            // DGV_UGSpheres
            // 
            this.DGV_UGSpheres.AllowUserToAddRows = false;
            this.DGV_UGSpheres.AllowUserToDeleteRows = false;
            this.DGV_UGSpheres.AllowUserToResizeColumns = false;
            this.DGV_UGSpheres.AllowUserToResizeRows = false;
            this.DGV_UGSpheres.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.DGV_UGSpheres.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.DGV_UGSpheres.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_UGSpheres.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Item_Spheres,
            this.Size_Spheres});
            this.DGV_UGSpheres.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DGV_UGSpheres.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.DGV_UGSpheres.Location = new System.Drawing.Point(2, 2);
            this.DGV_UGSpheres.Margin = new System.Windows.Forms.Padding(2);
            this.DGV_UGSpheres.MultiSelect = false;
            this.DGV_UGSpheres.Name = "DGV_UGSpheres";
            this.DGV_UGSpheres.RowHeadersVisible = false;
            this.DGV_UGSpheres.RowHeadersWidth = 51;
            this.DGV_UGSpheres.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.DGV_UGSpheres.RowTemplate.Height = 24;
            this.DGV_UGSpheres.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.DGV_UGSpheres.ShowEditingIcon = false;
            this.DGV_UGSpheres.Size = new System.Drawing.Size(201, 151);
            this.DGV_UGSpheres.TabIndex = 1;
            // 
            // Item_Spheres
            // 
            this.Item_Spheres.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Item_Spheres.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.Item_Spheres.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Item_Spheres.HeaderText = "Sphere";
            this.Item_Spheres.MinimumWidth = 6;
            this.Item_Spheres.Name = "Item_Spheres";
            this.Item_Spheres.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Item_Spheres.Width = 150;
            // 
            // Size_Spheres
            // 
            this.Size_Spheres.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Size_Spheres.DefaultCellStyle = dataGridViewCellStyle5;
            this.Size_Spheres.HeaderText = "Size";
            this.Size_Spheres.MaxInputLength = 2;
            this.Size_Spheres.MinimumWidth = 6;
            this.Size_Spheres.Name = "Size_Spheres";
            this.Size_Spheres.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Size_Spheres.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Size_Spheres.Width = 75;
            // 
            // TB_UGTraps
            // 
            this.TB_UGTraps.Controls.Add(this.DGV_UGTraps);
            this.TB_UGTraps.Location = new System.Drawing.Point(4, 22);
            this.TB_UGTraps.Margin = new System.Windows.Forms.Padding(2);
            this.TB_UGTraps.Name = "TB_UGTraps";
            this.TB_UGTraps.Padding = new System.Windows.Forms.Padding(2);
            this.TB_UGTraps.Size = new System.Drawing.Size(205, 155);
            this.TB_UGTraps.TabIndex = 2;
            this.TB_UGTraps.Text = "Traps";
            this.TB_UGTraps.UseVisualStyleBackColor = true;
            // 
            // DGV_UGTraps
            // 
            this.DGV_UGTraps.AllowUserToAddRows = false;
            this.DGV_UGTraps.AllowUserToDeleteRows = false;
            this.DGV_UGTraps.AllowUserToResizeColumns = false;
            this.DGV_UGTraps.AllowUserToResizeRows = false;
            this.DGV_UGTraps.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.DGV_UGTraps.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.DGV_UGTraps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_UGTraps.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Item_Traps});
            this.DGV_UGTraps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DGV_UGTraps.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.DGV_UGTraps.Location = new System.Drawing.Point(2, 2);
            this.DGV_UGTraps.Margin = new System.Windows.Forms.Padding(2);
            this.DGV_UGTraps.MultiSelect = false;
            this.DGV_UGTraps.Name = "DGV_UGTraps";
            this.DGV_UGTraps.RowHeadersVisible = false;
            this.DGV_UGTraps.RowHeadersWidth = 51;
            this.DGV_UGTraps.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.DGV_UGTraps.RowTemplate.Height = 24;
            this.DGV_UGTraps.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.DGV_UGTraps.ShowEditingIcon = false;
            this.DGV_UGTraps.Size = new System.Drawing.Size(201, 151);
            this.DGV_UGTraps.TabIndex = 1;
            // 
            // Item_Traps
            // 
            this.Item_Traps.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Item_Traps.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.Item_Traps.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Item_Traps.HeaderText = "Item";
            this.Item_Traps.MinimumWidth = 6;
            this.Item_Traps.Name = "Item_Traps";
            this.Item_Traps.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Item_Traps.Width = 175;
            // 
            // TB_UGTreasures
            // 
            this.TB_UGTreasures.Controls.Add(this.DGV_UGTreasures);
            this.TB_UGTreasures.Location = new System.Drawing.Point(4, 22);
            this.TB_UGTreasures.Margin = new System.Windows.Forms.Padding(2);
            this.TB_UGTreasures.Name = "TB_UGTreasures";
            this.TB_UGTreasures.Padding = new System.Windows.Forms.Padding(2);
            this.TB_UGTreasures.Size = new System.Drawing.Size(205, 155);
            this.TB_UGTreasures.TabIndex = 3;
            this.TB_UGTreasures.Text = "Treasures";
            this.TB_UGTreasures.UseVisualStyleBackColor = true;
            // 
            // DGV_UGTreasures
            // 
            this.DGV_UGTreasures.AllowUserToAddRows = false;
            this.DGV_UGTreasures.AllowUserToDeleteRows = false;
            this.DGV_UGTreasures.AllowUserToResizeColumns = false;
            this.DGV_UGTreasures.AllowUserToResizeRows = false;
            this.DGV_UGTreasures.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.DGV_UGTreasures.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.DGV_UGTreasures.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_UGTreasures.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Item_Treasures});
            this.DGV_UGTreasures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DGV_UGTreasures.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.DGV_UGTreasures.Location = new System.Drawing.Point(2, 2);
            this.DGV_UGTreasures.Margin = new System.Windows.Forms.Padding(2);
            this.DGV_UGTreasures.MultiSelect = false;
            this.DGV_UGTreasures.Name = "DGV_UGTreasures";
            this.DGV_UGTreasures.RowHeadersVisible = false;
            this.DGV_UGTreasures.RowHeadersWidth = 51;
            this.DGV_UGTreasures.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.DGV_UGTreasures.RowTemplate.Height = 24;
            this.DGV_UGTreasures.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.DGV_UGTreasures.ShowEditingIcon = false;
            this.DGV_UGTreasures.Size = new System.Drawing.Size(201, 151);
            this.DGV_UGTreasures.TabIndex = 1;
            // 
            // Item_Treasures
            // 
            this.Item_Treasures.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Item_Treasures.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.Item_Treasures.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Item_Treasures.HeaderText = "Item";
            this.Item_Treasures.MinimumWidth = 6;
            this.Item_Treasures.Name = "Item_Treasures";
            this.Item_Treasures.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Item_Treasures.Width = 175;
            // 
            // SAV_Underground
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 231);
            this.Controls.Add(this.TC_UGItems);
            this.Controls.Add(this.GB_UScores);
            this.Controls.Add(this.B_Cancel);
            this.Controls.Add(this.B_Save);
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(450, 270);
            this.Name = "SAV_Underground";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Underground Editor";
            ((System.ComponentModel.ISupportInitialize)(this.U_PlayersMet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.U_Gifts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.U_Spheres)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.U_Fossils)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.U_TrapsA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.U_TrapsT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.U_Flags)).EndInit();
            this.GB_UScores.ResumeLayout(false);
            this.TC_UGItems.ResumeLayout(false);
            this.TB_UGGoods.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DGV_UGGoods)).EndInit();
            this.TB_UGSpheres.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DGV_UGSpheres)).EndInit();
            this.TB_UGTraps.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DGV_UGTraps)).EndInit();
            this.TB_UGTreasures.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DGV_UGTreasures)).EndInit();
            this.ResumeLayout(false);

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