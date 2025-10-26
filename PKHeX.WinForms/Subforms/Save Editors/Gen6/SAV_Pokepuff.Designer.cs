namespace PKHeX.WinForms
{
    partial class SAV_Pokepuff
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
            components = new System.ComponentModel.Container();
            dgv = new System.Windows.Forms.DataGridView();
            B_Save = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            B_All = new System.Windows.Forms.Button();
            B_Sort = new System.Windows.Forms.Button();
            B_None = new System.Windows.Forms.Button();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            toolTip2 = new System.Windows.Forms.ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)dgv).BeginInit();
            SuspendLayout();
            // 
            // dgv
            // 
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeColumns = false;
            dgv.AllowUserToResizeRows = false;
            dgv.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom;
            dgv.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            dgv.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dgv.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.ColumnHeadersVisible = false;
            dgv.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            dgv.Location = new System.Drawing.Point(14, 50);
            dgv.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dgv.MultiSelect = false;
            dgv.Name = "dgv";
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            dgv.ShowEditingIcon = false;
            dgv.Size = new System.Drawing.Size(233, 215);
            dgv.TabIndex = 11;
            dgv.CellClick += DropClick;
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            B_Save.Location = new System.Drawing.Point(189, 271);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(58, 27);
            B_Save.TabIndex = 12;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            B_Cancel.Location = new System.Drawing.Point(124, 271);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(58, 27);
            B_Cancel.TabIndex = 13;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_All
            // 
            B_All.Location = new System.Drawing.Point(14, 16);
            B_All.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_All.Name = "B_All";
            B_All.Size = new System.Drawing.Size(70, 27);
            B_All.TabIndex = 14;
            B_All.Text = "All";
            B_All.UseVisualStyleBackColor = true;
            B_All.Click += B_All_Click;
            // 
            // B_Sort
            // 
            B_Sort.Location = new System.Drawing.Point(177, 16);
            B_Sort.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Sort.Name = "B_Sort";
            B_Sort.Size = new System.Drawing.Size(70, 27);
            B_Sort.TabIndex = 15;
            B_Sort.Text = "Sort";
            B_Sort.UseVisualStyleBackColor = true;
            B_Sort.Click += B_Sort_Click;
            // 
            // B_None
            // 
            B_None.Location = new System.Drawing.Point(96, 16);
            B_None.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_None.Name = "B_None";
            B_None.Size = new System.Drawing.Size(70, 27);
            B_None.TabIndex = 16;
            B_None.Text = "None";
            B_None.UseVisualStyleBackColor = true;
            B_None.Click += B_None_Click;
            // 
            // SAV_Pokepuff
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(261, 314);
            Controls.Add(B_None);
            Controls.Add(B_Sort);
            Controls.Add(B_All);
            Controls.Add(B_Cancel);
            Controls.Add(B_Save);
            Controls.Add(dgv);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(277, 859);
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(277, 340);
            Name = "SAV_Pokepuff";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Poké Puffs Editor";
            ((System.ComponentModel.ISupportInitialize)dgv).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_All;
        private System.Windows.Forms.Button B_Sort;
        private System.Windows.Forms.Button B_None;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolTip toolTip2;
    }
}
