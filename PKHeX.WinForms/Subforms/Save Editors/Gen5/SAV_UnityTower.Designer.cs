using PKHeX.WinForms.Controls;

namespace PKHeX.WinForms
{
    partial class SAV_UnityTower
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            B_Save = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            B_SetAllLocations = new System.Windows.Forms.Button();
            B_ClearLocations = new System.Windows.Forms.Button();
            CHK_GlobalFlag = new System.Windows.Forms.CheckBox();
            CHK_UnityTowerFlag = new System.Windows.Forms.CheckBox();
            DGV_Geonet = new DoubleBufferedDataGridView();
            Item_CountryIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Item_Country = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Item_RegionIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Item_Region = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Item_Point = new System.Windows.Forms.DataGridViewComboBoxColumn();
            DGV_UnityTower = new DoubleBufferedDataGridView();
            Item_Floor = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            Item_UnityTowerIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Item_UnityTower = new System.Windows.Forms.DataGridViewTextBoxColumn();
            FLP_CHK = new System.Windows.Forms.FlowLayoutPanel();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            B_SetAllLegalLocations = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)DGV_Geonet).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DGV_UnityTower).BeginInit();
            FLP_CHK.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // B_Save
            // 
            B_Save.Dock = System.Windows.Forms.DockStyle.Fill;
            B_Save.Location = new System.Drawing.Point(489, 352);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(91, 26);
            B_Save.TabIndex = 7;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Dock = System.Windows.Forms.DockStyle.Fill;
            B_Cancel.Location = new System.Drawing.Point(392, 352);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(89, 26);
            B_Cancel.TabIndex = 6;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_SetAllLocations
            // 
            tableLayoutPanel1.SetColumnSpan(B_SetAllLocations, 2);
            B_SetAllLocations.Dock = System.Windows.Forms.DockStyle.Fill;
            B_SetAllLocations.Location = new System.Drawing.Point(4, 3);
            B_SetAllLocations.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_SetAllLocations.Name = "B_SetAllLocations";
            B_SetAllLocations.Size = new System.Drawing.Size(186, 26);
            B_SetAllLocations.TabIndex = 0;
            B_SetAllLocations.Text = "Set All Locations";
            B_SetAllLocations.UseVisualStyleBackColor = true;
            B_SetAllLocations.Click += B_SetAllLocations_Click;
            // 
            // B_ClearLocations
            // 
            tableLayoutPanel1.SetColumnSpan(B_ClearLocations, 2);
            B_ClearLocations.Dock = System.Windows.Forms.DockStyle.Fill;
            B_ClearLocations.Location = new System.Drawing.Point(392, 3);
            B_ClearLocations.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ClearLocations.Name = "B_ClearLocations";
            B_ClearLocations.Size = new System.Drawing.Size(188, 26);
            B_ClearLocations.TabIndex = 2;
            B_ClearLocations.Text = "Clear Locations";
            B_ClearLocations.UseVisualStyleBackColor = true;
            B_ClearLocations.Click += B_ClearLocations_Click;
            // 
            // CHK_GlobalFlag
            // 
            CHK_GlobalFlag.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            CHK_GlobalFlag.AutoSize = true;
            CHK_GlobalFlag.Location = new System.Drawing.Point(165, 3);
            CHK_GlobalFlag.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_GlobalFlag.Name = "CHK_GlobalFlag";
            CHK_GlobalFlag.Size = new System.Drawing.Size(145, 21);
            CHK_GlobalFlag.TabIndex = 4;
            CHK_GlobalFlag.Text = "Whole Globe Visible";
            CHK_GlobalFlag.UseVisualStyleBackColor = true;
            // 
            // CHK_UnityTowerFlag
            // 
            CHK_UnityTowerFlag.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            CHK_UnityTowerFlag.AutoSize = true;
            CHK_UnityTowerFlag.Location = new System.Drawing.Point(4, 3);
            CHK_UnityTowerFlag.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_UnityTowerFlag.Name = "CHK_UnityTowerFlag";
            CHK_UnityTowerFlag.Size = new System.Drawing.Size(153, 21);
            CHK_UnityTowerFlag.TabIndex = 5;
            CHK_UnityTowerFlag.Text = "Unity Tower Unlocked";
            CHK_UnityTowerFlag.UseVisualStyleBackColor = true;
            // 
            // DGV_Geonet
            // 
            DGV_Geonet.AllowUserToAddRows = false;
            DGV_Geonet.AllowUserToDeleteRows = false;
            DGV_Geonet.AllowUserToResizeColumns = false;
            DGV_Geonet.AllowUserToResizeRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.ControlLight;
            DGV_Geonet.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            DGV_Geonet.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            DGV_Geonet.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            DGV_Geonet.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            DGV_Geonet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_Geonet.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Item_CountryIndex, Item_Country, Item_RegionIndex, Item_Region, Item_Point });
            tableLayoutPanel1.SetColumnSpan(DGV_Geonet, 4);
            DGV_Geonet.Dock = System.Windows.Forms.DockStyle.Fill;
            DGV_Geonet.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            DGV_Geonet.Location = new System.Drawing.Point(3, 35);
            DGV_Geonet.MultiSelect = false;
            DGV_Geonet.Name = "DGV_Geonet";
            DGV_Geonet.RowHeadersVisible = false;
            DGV_Geonet.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            DGV_Geonet.Size = new System.Drawing.Size(382, 279);
            DGV_Geonet.StandardTab = true;
            DGV_Geonet.TabIndex = 0;
            // 
            // Item_CountryIndex
            // 
            Item_CountryIndex.HeaderText = "CountryIndex";
            Item_CountryIndex.Name = "Item_CountryIndex";
            Item_CountryIndex.ReadOnly = true;
            Item_CountryIndex.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            Item_CountryIndex.Visible = false;
            // 
            // Item_Country
            // 
            Item_Country.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            Item_Country.FillWeight = 150F;
            Item_Country.HeaderText = "Country";
            Item_Country.MinimumWidth = 50;
            Item_Country.Name = "Item_Country";
            Item_Country.ReadOnly = true;
            Item_Country.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Item_RegionIndex
            // 
            Item_RegionIndex.HeaderText = "RegionIndex";
            Item_RegionIndex.Name = "Item_RegionIndex";
            Item_RegionIndex.ReadOnly = true;
            Item_RegionIndex.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            Item_RegionIndex.Visible = false;
            // 
            // Item_Region
            // 
            Item_Region.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            Item_Region.FillWeight = 150F;
            Item_Region.HeaderText = "Region";
            Item_Region.MinimumWidth = 50;
            Item_Region.Name = "Item_Region";
            Item_Region.ReadOnly = true;
            Item_Region.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Item_Point
            // 
            Item_Point.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            Item_Point.FillWeight = 50F;
            Item_Point.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            Item_Point.HeaderText = "Point";
            Item_Point.MinimumWidth = 50;
            Item_Point.Name = "Item_Point";
            Item_Point.Width = 50;
            // 
            // DGV_UnityTower
            // 
            DGV_UnityTower.AllowUserToAddRows = false;
            DGV_UnityTower.AllowUserToDeleteRows = false;
            DGV_UnityTower.AllowUserToResizeColumns = false;
            DGV_UnityTower.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ControlLight;
            DGV_UnityTower.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            DGV_UnityTower.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            DGV_UnityTower.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            DGV_UnityTower.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_UnityTower.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Item_Floor, Item_UnityTowerIndex, Item_UnityTower });
            tableLayoutPanel1.SetColumnSpan(DGV_UnityTower, 2);
            DGV_UnityTower.Dock = System.Windows.Forms.DockStyle.Fill;
            DGV_UnityTower.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            DGV_UnityTower.Location = new System.Drawing.Point(391, 35);
            DGV_UnityTower.MultiSelect = false;
            DGV_UnityTower.Name = "DGV_UnityTower";
            DGV_UnityTower.RowHeadersVisible = false;
            DGV_UnityTower.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            DGV_UnityTower.Size = new System.Drawing.Size(190, 279);
            DGV_UnityTower.StandardTab = true;
            DGV_UnityTower.TabIndex = 0;
            // 
            // Item_Floor
            // 
            Item_Floor.HeaderText = "Floor";
            Item_Floor.Name = "Item_Floor";
            Item_Floor.Width = 50;
            // 
            // Item_UnityTowerIndex
            // 
            Item_UnityTowerIndex.HeaderText = "CountryIndex";
            Item_UnityTowerIndex.Name = "Item_UnityTowerIndex";
            Item_UnityTowerIndex.ReadOnly = true;
            Item_UnityTowerIndex.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            Item_UnityTowerIndex.Visible = false;
            // 
            // Item_UnityTower
            // 
            Item_UnityTower.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            Item_UnityTower.HeaderText = "Country";
            Item_UnityTower.Name = "Item_UnityTower";
            Item_UnityTower.ReadOnly = true;
            Item_UnityTower.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // FLP_CHK
            // 
            tableLayoutPanel1.SetColumnSpan(FLP_CHK, 6);
            FLP_CHK.Controls.Add(CHK_UnityTowerFlag);
            FLP_CHK.Controls.Add(CHK_GlobalFlag);
            FLP_CHK.Dock = System.Windows.Forms.DockStyle.Fill;
            FLP_CHK.Location = new System.Drawing.Point(3, 320);
            FLP_CHK.Name = "FLP_CHK";
            FLP_CHK.Size = new System.Drawing.Size(578, 26);
            FLP_CHK.TabIndex = 8;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 6;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.666666F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.666666F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.666666F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.666666F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.666666F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.666666F));
            tableLayoutPanel1.Controls.Add(DGV_UnityTower, 4, 1);
            tableLayoutPanel1.Controls.Add(DGV_Geonet, 0, 1);
            tableLayoutPanel1.Controls.Add(FLP_CHK, 0, 2);
            tableLayoutPanel1.Controls.Add(B_Save, 5, 3);
            tableLayoutPanel1.Controls.Add(B_Cancel, 4, 3);
            tableLayoutPanel1.Controls.Add(B_ClearLocations, 4, 0);
            tableLayoutPanel1.Controls.Add(B_SetAllLocations, 0, 0);
            tableLayoutPanel1.Controls.Add(B_SetAllLegalLocations, 2, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            tableLayoutPanel1.Size = new System.Drawing.Size(584, 381);
            tableLayoutPanel1.TabIndex = 9;
            // 
            // B_SetAllLegalLocations
            // 
            tableLayoutPanel1.SetColumnSpan(B_SetAllLegalLocations, 2);
            B_SetAllLegalLocations.Dock = System.Windows.Forms.DockStyle.Fill;
            B_SetAllLegalLocations.Location = new System.Drawing.Point(198, 3);
            B_SetAllLegalLocations.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_SetAllLegalLocations.Name = "B_SetAllLegalLocations";
            B_SetAllLegalLocations.Size = new System.Drawing.Size(186, 26);
            B_SetAllLegalLocations.TabIndex = 1;
            B_SetAllLegalLocations.Text = "Set All Legal Locations";
            B_SetAllLegalLocations.UseVisualStyleBackColor = true;
            B_SetAllLegalLocations.Click += B_SetAllLegalLocations_Click;
            // 
            // SAV_UnityTower
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(584, 381);
            Controls.Add(tableLayoutPanel1);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(2);
            MaximizeBox = false;
            MinimumSize = new System.Drawing.Size(500, 400);
            Name = "SAV_UnityTower";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Unity Tower Editor";
            ((System.ComponentModel.ISupportInitialize)DGV_Geonet).EndInit();
            ((System.ComponentModel.ISupportInitialize)DGV_UnityTower).EndInit();
            FLP_CHK.ResumeLayout(false);
            FLP_CHK.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_SetAllLocations;
        private System.Windows.Forms.Button B_ClearLocations;
        private System.Windows.Forms.CheckBox CHK_GlobalFlag;
        private System.Windows.Forms.CheckBox CHK_UnityTowerFlag;
        private DoubleBufferedDataGridView DGV_Geonet;
        private System.Windows.Forms.DataGridViewComboBoxColumn Item_Point;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Item_Floor;
        private DoubleBufferedDataGridView DGV_UnityTower;
        private System.Windows.Forms.DataGridViewTextBoxColumn Item_CountryIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn Item_RegionIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn Item_UnityTowerIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn Item_Country;
        private System.Windows.Forms.DataGridViewTextBoxColumn Item_Region;
        private System.Windows.Forms.DataGridViewTextBoxColumn Item_UnityTower;
        private System.Windows.Forms.FlowLayoutPanel FLP_CHK;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button B_SetAllLegalLocations;
    }
}
