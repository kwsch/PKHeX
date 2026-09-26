namespace PKHeX.WinForms
{
    partial class SAV_GiftRibbons
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            ColRibbon = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ColDescriptionIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            B_Reset = new System.Windows.Forms.Button();
            B_Legal = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            B_Save = new System.Windows.Forms.Button();
            dgv = new PKHeX.WinForms.Controls.DoubleBufferedDataGridView();
            TLP_Grid = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)dgv).BeginInit();
            TLP_Grid.SuspendLayout();
            SuspendLayout();
            // 
            // ColRibbon
            // 
            ColRibbon.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            ColRibbon.DefaultCellStyle = dataGridViewCellStyle1;
            ColRibbon.HeaderText = "Ribbon";
            ColRibbon.Name = "ColRibbon";
            ColRibbon.ReadOnly = true;
            ColRibbon.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColDescriptionIndex
            // 
            ColDescriptionIndex.HeaderText = "Desc";
            ColDescriptionIndex.Name = "ColDescriptionIndex";
            ColDescriptionIndex.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            ColDescriptionIndex.Width = 45;
            // 
            // B_Reset
            // 
            B_Reset.Dock = System.Windows.Forms.DockStyle.Fill;
            B_Reset.Location = new System.Drawing.Point(134, 3);
            B_Reset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Reset.Name = "B_Reset";
            B_Reset.Size = new System.Drawing.Size(123, 26);
            B_Reset.TabIndex = 21;
            B_Reset.Text = "Reset";
            B_Reset.UseVisualStyleBackColor = true;
            B_Reset.Click += B_Reset_Click;
            // 
            // B_Legal
            // 
            B_Legal.Dock = System.Windows.Forms.DockStyle.Fill;
            B_Legal.Location = new System.Drawing.Point(4, 3);
            B_Legal.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Legal.Name = "B_Legal";
            B_Legal.Size = new System.Drawing.Size(122, 26);
            B_Legal.TabIndex = 20;
            B_Legal.Text = "All Legal";
            B_Legal.UseVisualStyleBackColor = true;
            B_Legal.Click += B_Legal_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Dock = System.Windows.Forms.DockStyle.Fill;
            B_Cancel.Location = new System.Drawing.Point(4, 285);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(122, 26);
            B_Cancel.TabIndex = 19;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_Save
            // 
            B_Save.Dock = System.Windows.Forms.DockStyle.Fill;
            B_Save.Location = new System.Drawing.Point(134, 285);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(123, 26);
            B_Save.TabIndex = 18;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // dgv
            // 
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeColumns = false;
            dgv.AllowUserToResizeRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.ControlLight;
            dgv.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            dgv.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            dgv.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dgv.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { ColRibbon, ColDescriptionIndex });
            TLP_Grid.SetColumnSpan(dgv, 2);
            dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            dgv.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            dgv.Location = new System.Drawing.Point(4, 35);
            dgv.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dgv.MultiSelect = false;
            dgv.Name = "dgv";
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            dgv.ShowEditingIcon = false;
            dgv.Size = new System.Drawing.Size(253, 244);
            dgv.TabIndex = 17;
            // 
            // TLP_Grid
            // 
            TLP_Grid.ColumnCount = 2;
            TLP_Grid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            TLP_Grid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            TLP_Grid.Controls.Add(B_Legal, 0, 0);
            TLP_Grid.Controls.Add(B_Cancel, 0, 2);
            TLP_Grid.Controls.Add(B_Reset, 1, 0);
            TLP_Grid.Controls.Add(B_Save, 1, 2);
            TLP_Grid.Controls.Add(dgv, 0, 1);
            TLP_Grid.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Grid.Location = new System.Drawing.Point(0, 0);
            TLP_Grid.Name = "TLP_Grid";
            TLP_Grid.RowCount = 3;
            TLP_Grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            TLP_Grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_Grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            TLP_Grid.Size = new System.Drawing.Size(261, 314);
            TLP_Grid.TabIndex = 22;
            // 
            // SAV_GiftRibbons
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(261, 314);
            Controls.Add(TLP_Grid);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(2);
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(277, 353);
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(277, 353);
            Name = "SAV_GiftRibbons";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Gift Ribbon Editor";
            ((System.ComponentModel.ISupportInitialize)dgv).EndInit();
            TLP_Grid.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button B_Reset;
        private System.Windows.Forms.Button B_Legal;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private Controls.DoubleBufferedDataGridView dgv;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColRibbon;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColDescriptionIndex;
        private System.Windows.Forms.TableLayoutPanel TLP_Grid;
    }
}
