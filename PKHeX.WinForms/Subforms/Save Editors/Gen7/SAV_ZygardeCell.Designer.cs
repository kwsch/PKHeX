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
            this.dgv = new System.Windows.Forms.DataGridView();
            this.dgv_ref = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgv_location = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgv_val = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.B_Cancel = new System.Windows.Forms.Button();
            this.B_Save = new System.Windows.Forms.Button();
            this.NUD_Cells = new System.Windows.Forms.NumericUpDown();
            this.L_Cells = new System.Windows.Forms.Label();
            this.B_GiveAll = new System.Windows.Forms.Button();
            this.L_Collected = new System.Windows.Forms.Label();
            this.NUD_Collected = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Cells)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Collected)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AllowUserToResizeColumns = false;
            this.dgv.AllowUserToResizeRows = false;
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgv_ref,
            this.dgv_location,
            this.dgv_val});
            this.dgv.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgv.Location = new System.Drawing.Point(12, 12);
            this.dgv.MultiSelect = false;
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersVisible = false;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgv.Size = new System.Drawing.Size(480, 263);
            this.dgv.TabIndex = 0;
            // 
            // dgv_ref
            // 
            this.dgv_ref.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgv_ref.HeaderText = "Ref";
            this.dgv_ref.Name = "dgv_ref";
            this.dgv_ref.ReadOnly = true;
            this.dgv_ref.Width = 49;
            // 
            // dgv_location
            // 
            this.dgv_location.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgv_location.HeaderText = "Location";
            this.dgv_location.Name = "dgv_location";
            this.dgv_location.ReadOnly = true;
            this.dgv_location.Width = 73;
            // 
            // dgv_val
            // 
            this.dgv_val.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dgv_val.HeaderText = "Value";
            this.dgv_val.Name = "dgv_val";
            this.dgv_val.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(417, 306);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(75, 23);
            this.B_Cancel.TabIndex = 1;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(417, 281);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(75, 23);
            this.B_Save.TabIndex = 2;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // NUD_Cells
            // 
            this.NUD_Cells.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.NUD_Cells.Location = new System.Drawing.Point(90, 309);
            this.NUD_Cells.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.NUD_Cells.Name = "NUD_Cells";
            this.NUD_Cells.Size = new System.Drawing.Size(66, 20);
            this.NUD_Cells.TabIndex = 3;
            // 
            // L_Cells
            // 
            this.L_Cells.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.L_Cells.Location = new System.Drawing.Point(12, 306);
            this.L_Cells.Name = "L_Cells";
            this.L_Cells.Size = new System.Drawing.Size(72, 23);
            this.L_Cells.TabIndex = 4;
            this.L_Cells.Text = "Stored:";
            this.L_Cells.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // B_GiveAll
            // 
            this.B_GiveAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.B_GiveAll.Location = new System.Drawing.Point(162, 284);
            this.B_GiveAll.Name = "B_GiveAll";
            this.B_GiveAll.Size = new System.Drawing.Size(75, 23);
            this.B_GiveAll.TabIndex = 5;
            this.B_GiveAll.Text = "Collect All";
            this.B_GiveAll.UseVisualStyleBackColor = true;
            this.B_GiveAll.Click += new System.EventHandler(this.B_GiveAll_Click);
            // 
            // L_Collected
            // 
            this.L_Collected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.L_Collected.Location = new System.Drawing.Point(12, 281);
            this.L_Collected.Name = "L_Collected";
            this.L_Collected.Size = new System.Drawing.Size(72, 23);
            this.L_Collected.TabIndex = 7;
            this.L_Collected.Text = "Collected:";
            this.L_Collected.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_Collected
            // 
            this.NUD_Collected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.NUD_Collected.Location = new System.Drawing.Point(90, 284);
            this.NUD_Collected.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.NUD_Collected.Name = "NUD_Collected";
            this.NUD_Collected.Size = new System.Drawing.Size(66, 20);
            this.NUD_Collected.TabIndex = 6;
            // 
            // SAV_ZygardeCell
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 341);
            this.Controls.Add(this.L_Collected);
            this.Controls.Add(this.NUD_Collected);
            this.Controls.Add(this.B_GiveAll);
            this.Controls.Add(this.L_Cells);
            this.Controls.Add(this.NUD_Cells);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_Cancel);
            this.Controls.Add(this.dgv);
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(520, 380);
            this.Name = "SAV_ZygardeCell";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cells/Sticker Editor";
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Cells)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Collected)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.NumericUpDown NUD_Cells;
        private System.Windows.Forms.Label L_Cells;
        private System.Windows.Forms.Button B_GiveAll;
        private System.Windows.Forms.DataGridViewComboBoxColumn dgv_val;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgv_location;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgv_ref;
        private System.Windows.Forms.Label L_Collected;
        private System.Windows.Forms.NumericUpDown NUD_Collected;
    }
}