namespace PKHeX.WinForms
{
    partial class SAV_ZygardeCell
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
            dgv = new System.Windows.Forms.DataGridView();
            dgv_ref = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgv_location = new System.Windows.Forms.DataGridViewTextBoxColumn();
            dgv_val = new System.Windows.Forms.DataGridViewComboBoxColumn();
            B_Cancel = new System.Windows.Forms.Button();
            B_Save = new System.Windows.Forms.Button();
            NUD_CellsTotal = new System.Windows.Forms.NumericUpDown();
            L_Cells = new System.Windows.Forms.Label();
            B_GiveAll = new System.Windows.Forms.Button();
            L_Collected = new System.Windows.Forms.Label();
            NUD_CellsCollected = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)dgv).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_CellsTotal).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_CellsCollected).BeginInit();
            SuspendLayout();
            // 
            // dgv
            // 
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeColumns = false;
            dgv.AllowUserToResizeRows = false;
            dgv.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dgv.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { dgv_ref, dgv_location, dgv_val });
            dgv.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            dgv.Location = new System.Drawing.Point(14, 14);
            dgv.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dgv.MultiSelect = false;
            dgv.Name = "dgv";
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            dgv.Size = new System.Drawing.Size(560, 303);
            dgv.TabIndex = 0;
            // 
            // dgv_ref
            // 
            dgv_ref.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dgv_ref.HeaderText = "Ref";
            dgv_ref.Name = "dgv_ref";
            dgv_ref.ReadOnly = true;
            dgv_ref.Width = 49;
            // 
            // dgv_location
            // 
            dgv_location.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dgv_location.HeaderText = "Location";
            dgv_location.Name = "dgv_location";
            dgv_location.ReadOnly = true;
            dgv_location.Width = 78;
            // 
            // dgv_val
            // 
            dgv_val.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            dgv_val.HeaderText = "Value";
            dgv_val.Name = "dgv_val";
            dgv_val.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(486, 353);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(88, 27);
            B_Cancel.TabIndex = 1;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(486, 324);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(88, 27);
            B_Save.TabIndex = 2;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // NUD_Cells
            // 
            NUD_CellsTotal.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            NUD_CellsTotal.Location = new System.Drawing.Point(105, 357);
            NUD_CellsTotal.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_CellsTotal.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NUD_CellsTotal.Name = "NUD_CellsTotal";
            NUD_CellsTotal.Size = new System.Drawing.Size(77, 23);
            NUD_CellsTotal.TabIndex = 3;
            // 
            // L_Cells
            // 
            L_Cells.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            L_Cells.Location = new System.Drawing.Point(14, 353);
            L_Cells.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Cells.Name = "L_Cells";
            L_Cells.Size = new System.Drawing.Size(84, 27);
            L_Cells.TabIndex = 4;
            L_Cells.Text = "Stored:";
            L_Cells.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // B_GiveAll
            // 
            B_GiveAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_GiveAll.Location = new System.Drawing.Point(189, 328);
            B_GiveAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_GiveAll.Name = "B_GiveAll";
            B_GiveAll.Size = new System.Drawing.Size(88, 27);
            B_GiveAll.TabIndex = 5;
            B_GiveAll.Text = "Collect All";
            B_GiveAll.UseVisualStyleBackColor = true;
            B_GiveAll.Click += B_GiveAll_Click;
            // 
            // L_Collected
            // 
            L_Collected.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            L_Collected.Location = new System.Drawing.Point(14, 324);
            L_Collected.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Collected.Name = "L_Collected";
            L_Collected.Size = new System.Drawing.Size(84, 27);
            L_Collected.TabIndex = 7;
            L_Collected.Text = "Collected:";
            L_Collected.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_Collected
            // 
            NUD_CellsCollected.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            NUD_CellsCollected.Location = new System.Drawing.Point(105, 328);
            NUD_CellsCollected.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_CellsCollected.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NUD_CellsCollected.Name = "NUD_CellsCollected";
            NUD_CellsCollected.Size = new System.Drawing.Size(77, 23);
            NUD_CellsCollected.TabIndex = 6;
            // 
            // SAV_ZygardeCell
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(588, 393);
            Controls.Add(L_Collected);
            Controls.Add(NUD_CellsCollected);
            Controls.Add(B_GiveAll);
            Controls.Add(L_Cells);
            Controls.Add(NUD_CellsTotal);
            Controls.Add(B_Save);
            Controls.Add(B_Cancel);
            Controls.Add(dgv);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimumSize = new System.Drawing.Size(604, 432);
            Name = "SAV_ZygardeCell";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Cells/Sticker Editor";
            ((System.ComponentModel.ISupportInitialize)dgv).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_CellsTotal).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_CellsCollected).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.NumericUpDown NUD_CellsTotal;
        private System.Windows.Forms.Label L_Cells;
        private System.Windows.Forms.Button B_GiveAll;
        private System.Windows.Forms.DataGridViewComboBoxColumn dgv_val;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgv_location;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgv_ref;
        private System.Windows.Forms.Label L_Collected;
        private System.Windows.Forms.NumericUpDown NUD_CellsCollected;
    }
}
