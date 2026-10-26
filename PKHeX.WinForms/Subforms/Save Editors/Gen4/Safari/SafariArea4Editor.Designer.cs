using System;

namespace PKHeX.WinForms
{
    partial class SafariArea4Editor
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            TLP_Main = new System.Windows.Forms.TableLayoutPanel();
            L_Map = new System.Windows.Forms.Label();
            CB_Map = new System.Windows.Forms.ComboBox();
            L_Count = new System.Windows.Forms.Label();
            FLP_Actions = new System.Windows.Forms.FlowLayoutPanel();
            B_Import = new System.Windows.Forms.Button();
            B_Export = new System.Windows.Forms.Button();
            B_ClearAll = new System.Windows.Forms.Button();
            DGV_Objects = new System.Windows.Forms.DataGridView();
            DGV_Type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            DGV_X = new System.Windows.Forms.DataGridViewTextBoxColumn();
            DGV_Y = new System.Windows.Forms.DataGridViewTextBoxColumn();
            DGV_Z = new System.Windows.Forms.DataGridViewTextBoxColumn();
            FLP_Count = new System.Windows.Forms.FlowLayoutPanel();
            NUD_Count = new System.Windows.Forms.NumericUpDown();
            CHK_Auto = new System.Windows.Forms.CheckBox();
            TLP_Main.SuspendLayout();
            FLP_Actions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_Objects).BeginInit();
            FLP_Count.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Count).BeginInit();
            SuspendLayout();
            // 
            // TLP_Main
            // 
            TLP_Main.ColumnCount = 2;
            TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_Main.Controls.Add(L_Map, 0, 0);
            TLP_Main.Controls.Add(CB_Map, 1, 0);
            TLP_Main.Controls.Add(L_Count, 0, 1);
            TLP_Main.Controls.Add(FLP_Actions, 0, 2);
            TLP_Main.Controls.Add(DGV_Objects, 0, 3);
            TLP_Main.Controls.Add(FLP_Count, 1, 1);
            TLP_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Main.Location = new System.Drawing.Point(0, 0);
            TLP_Main.Name = "TLP_Main";
            TLP_Main.RowCount = 4;
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_Main.Size = new System.Drawing.Size(420, 300);
            TLP_Main.TabIndex = 0;
            // 
            // L_Map
            // 
            L_Map.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Map.AutoSize = true;
            L_Map.Location = new System.Drawing.Point(3, 7);
            L_Map.Name = "L_Map";
            L_Map.Size = new System.Drawing.Size(38, 17);
            L_Map.TabIndex = 0;
            L_Map.Text = "Map:";
            // 
            // CB_Map
            // 
            CB_Map.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Map.Location = new System.Drawing.Point(47, 3);
            CB_Map.Name = "CB_Map";
            CB_Map.Size = new System.Drawing.Size(180, 25);
            CB_Map.TabIndex = 1;
            // 
            // L_Count
            // 
            L_Count.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Count.AutoSize = true;
            L_Count.Location = new System.Drawing.Point(22, 38);
            L_Count.Name = "L_Count";
            L_Count.Size = new System.Drawing.Size(19, 17);
            L_Count.TabIndex = 2;
            L_Count.Text = "#:";
            // 
            // FLP_Actions
            // 
            FLP_Actions.AutoSize = true;
            TLP_Main.SetColumnSpan(FLP_Actions, 2);
            FLP_Actions.Controls.Add(B_Import);
            FLP_Actions.Controls.Add(B_Export);
            FLP_Actions.Controls.Add(B_ClearAll);
            FLP_Actions.Dock = System.Windows.Forms.DockStyle.Fill;
            FLP_Actions.Location = new System.Drawing.Point(3, 65);
            FLP_Actions.Name = "FLP_Actions";
            FLP_Actions.Size = new System.Drawing.Size(414, 33);
            FLP_Actions.TabIndex = 4;
            // 
            // B_Import
            // 
            B_Import.AutoSize = true;
            B_Import.Location = new System.Drawing.Point(3, 3);
            B_Import.Name = "B_Import";
            B_Import.Size = new System.Drawing.Size(75, 27);
            B_Import.TabIndex = 0;
            B_Import.Text = "Import";
            B_Import.UseVisualStyleBackColor = true;
            B_Import.Click += B_Import_Click;
            // 
            // B_Export
            // 
            B_Export.AutoSize = true;
            B_Export.Location = new System.Drawing.Point(84, 3);
            B_Export.Name = "B_Export";
            B_Export.Size = new System.Drawing.Size(75, 27);
            B_Export.TabIndex = 1;
            B_Export.Text = "Export";
            B_Export.UseVisualStyleBackColor = true;
            B_Export.Click += B_Export_Click;
            // 
            // B_ClearAll
            // 
            B_ClearAll.AutoSize = true;
            B_ClearAll.Location = new System.Drawing.Point(165, 3);
            B_ClearAll.Name = "B_ClearAll";
            B_ClearAll.Size = new System.Drawing.Size(75, 27);
            B_ClearAll.TabIndex = 2;
            B_ClearAll.Text = "Clear All";
            B_ClearAll.UseVisualStyleBackColor = true;
            B_ClearAll.Click += B_ClearAll_Click;
            // 
            // DGV_Objects
            // 
            DGV_Objects.AllowUserToAddRows = false;
            DGV_Objects.AllowUserToDeleteRows = false;
            DGV_Objects.AllowUserToResizeRows = false;
            DGV_Objects.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            DGV_Objects.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_Objects.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { DGV_Type, DGV_X, DGV_Y, DGV_Z });
            TLP_Main.SetColumnSpan(DGV_Objects, 2);
            DGV_Objects.Dock = System.Windows.Forms.DockStyle.Fill;
            DGV_Objects.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            DGV_Objects.Location = new System.Drawing.Point(3, 104);
            DGV_Objects.Name = "DGV_Objects";
            DGV_Objects.RowHeadersVisible = false;
            DGV_Objects.Size = new System.Drawing.Size(414, 193);
            DGV_Objects.TabIndex = 5;
            // 
            // DGV_Type
            // 
            DGV_Type.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            DGV_Type.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            DGV_Type.HeaderText = "Type";
            DGV_Type.MaxDropDownItems = 10;
            DGV_Type.MinimumWidth = 120;
            DGV_Type.Name = "DGV_Type";
            DGV_Type.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            DGV_Type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            DGV_Type.Width = 120;
            // 
            // DGV_X
            // 
            DGV_X.HeaderText = "X";
            DGV_X.MaxInputLength = 3;
            DGV_X.Name = "DGV_X";
            DGV_X.Width = 41;
            // 
            // DGV_Y
            // 
            DGV_Y.HeaderText = "Y";
            DGV_Y.MaxInputLength = 3;
            DGV_Y.Name = "DGV_Y";
            DGV_Y.Width = 40;
            // 
            // DGV_Z
            // 
            DGV_Z.HeaderText = "Z";
            DGV_Z.MaxInputLength = 3;
            DGV_Z.Name = "DGV_Z";
            DGV_Z.Width = 40;
            // 
            // FLP_Count
            // 
            FLP_Count.AutoSize = true;
            FLP_Count.Controls.Add(NUD_Count);
            FLP_Count.Controls.Add(CHK_Auto);
            FLP_Count.Dock = System.Windows.Forms.DockStyle.Fill;
            FLP_Count.Location = new System.Drawing.Point(44, 31);
            FLP_Count.Margin = new System.Windows.Forms.Padding(0);
            FLP_Count.Name = "FLP_Count";
            FLP_Count.Size = new System.Drawing.Size(376, 31);
            FLP_Count.TabIndex = 6;
            // 
            // NUD_Count
            // 
            NUD_Count.Location = new System.Drawing.Point(3, 3);
            NUD_Count.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            NUD_Count.Name = "NUD_Count";
            NUD_Count.Size = new System.Drawing.Size(50, 25);
            NUD_Count.TabIndex = 4;
            NUD_Count.ValueChanged += NUD_Count_ValueChanged;
            // 
            // CHK_Auto
            // 
            CHK_Auto.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CHK_Auto.AutoSize = true;
            CHK_Auto.Checked = true;
            CHK_Auto.Location = new System.Drawing.Point(59, 5);
            CHK_Auto.Name = "CHK_Auto";
            CHK_Auto.Size = new System.Drawing.Size(54, 21);
            CHK_Auto.TabIndex = 5;
            CHK_Auto.Text = "Auto";
            CHK_Auto.UseVisualStyleBackColor = true;
            // 
            // SafariArea4Editor
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(TLP_Main);
            Name = "SafariArea4Editor";
            Size = new System.Drawing.Size(420, 300);
            TLP_Main.ResumeLayout(false);
            TLP_Main.PerformLayout();
            FLP_Actions.ResumeLayout(false);
            FLP_Actions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_Objects).EndInit();
            FLP_Count.ResumeLayout(false);
            FLP_Count.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Count).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TLP_Main;
        private System.Windows.Forms.Label L_Map;
        private System.Windows.Forms.ComboBox CB_Map;
        private System.Windows.Forms.Label L_Count;
        private System.Windows.Forms.FlowLayoutPanel FLP_Actions;
        private System.Windows.Forms.Button B_Import;
        private System.Windows.Forms.Button B_Export;
        private System.Windows.Forms.DataGridView DGV_Objects;
        private System.Windows.Forms.FlowLayoutPanel FLP_Count;
        private System.Windows.Forms.NumericUpDown NUD_Count;
        private System.Windows.Forms.CheckBox CHK_Auto;
        private System.Windows.Forms.Button B_ClearAll;
        private System.Windows.Forms.DataGridViewComboBoxColumn DGV_Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn DGV_X;
        private System.Windows.Forms.DataGridViewTextBoxColumn DGV_Y;
        private System.Windows.Forms.DataGridViewTextBoxColumn DGV_Z;
    }
}
