namespace PKHeX.WinForms
{
    partial class TechRecordEditor
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
            B_Save = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            B_None = new System.Windows.Forms.Button();
            B_All = new System.Windows.Forms.Button();
            dgv = new System.Windows.Forms.DataGridView();
            HasFlag = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Type = new System.Windows.Forms.DataGridViewImageColumn();
            TypeInt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            MoveName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dgv).BeginInit();
            SuspendLayout();
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_Save.Location = new System.Drawing.Point(126, 401);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(105, 27);
            B_Save.TabIndex = 1;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_Cancel.Location = new System.Drawing.Point(14, 401);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(105, 27);
            B_Cancel.TabIndex = 2;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_None
            // 
            B_None.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_None.Location = new System.Drawing.Point(126, 368);
            B_None.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_None.Name = "B_None";
            B_None.Size = new System.Drawing.Size(105, 27);
            B_None.TabIndex = 5;
            B_None.Text = "Remove All";
            B_None.UseVisualStyleBackColor = true;
            B_None.Click += B_None_Click;
            // 
            // B_All
            // 
            B_All.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_All.Location = new System.Drawing.Point(14, 368);
            B_All.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_All.Name = "B_All";
            B_All.Size = new System.Drawing.Size(105, 27);
            B_All.TabIndex = 4;
            B_All.Text = "Give All";
            B_All.UseVisualStyleBackColor = true;
            B_All.Click += B_All_Click;
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
            dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { HasFlag, Index, Type, TypeInt, MoveName });
            dgv.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            dgv.Location = new System.Drawing.Point(3, 1);
            dgv.MultiSelect = false;
            dgv.Name = "dgv";
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgv.ShowEditingIcon = false;
            dgv.Size = new System.Drawing.Size(243, 362);
            dgv.TabIndex = 6;
            dgv.CellClick += ClickCell;
            dgv.ColumnHeaderMouseClick += SortColumn;
            dgv.KeyDown += PressKeyCell;
            // 
            // HasFlag
            // 
            HasFlag.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            HasFlag.Frozen = true;
            HasFlag.HeaderText = "";
            HasFlag.Name = "HasFlag";
            HasFlag.ReadOnly = true;
            HasFlag.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            HasFlag.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            HasFlag.Width = 19;
            // 
            // Index
            // 
            Index.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            Index.HeaderText = "";
            Index.Name = "Index";
            Index.ReadOnly = true;
            Index.Width = 19;
            // 
            // Type
            // 
            Type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            Type.HeaderText = "";
            Type.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            Type.Name = "Type";
            Type.ReadOnly = true;
            Type.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            Type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            Type.Width = 5;
            // 
            // TypeInt
            // 
            TypeInt.HeaderText = "";
            TypeInt.Name = "TypeInt";
            TypeInt.ReadOnly = true;
            TypeInt.Visible = false;
            // 
            // MoveName
            // 
            MoveName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            MoveName.HeaderText = "";
            MoveName.Name = "MoveName";
            MoveName.ReadOnly = true;
            // 
            // TechRecordEditor
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(248, 441);
            Controls.Add(dgv);
            Controls.Add(B_None);
            Controls.Add(B_All);
            Controls.Add(B_Cancel);
            Controls.Add(B_Save);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(264, 480);
            Name = "TechRecordEditor";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "TR Relearn Editor";
            ((System.ComponentModel.ISupportInitialize)dgv).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_None;
        private System.Windows.Forms.Button B_All;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.DataGridViewCheckBoxColumn HasFlag;
        private System.Windows.Forms.DataGridViewTextBoxColumn Index;
        private System.Windows.Forms.DataGridViewImageColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn TypeInt;
        private System.Windows.Forms.DataGridViewTextBoxColumn MoveName;
    }
}
