namespace PKHeX.WinForms
{
    partial class SAV_Gear
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
            B_UnlockAll = new System.Windows.Forms.Button();
            B_Clear = new System.Windows.Forms.Button();
            DGV_Gear = new System.Windows.Forms.DataGridView();
            Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            CharacterStyle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Category = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Gear = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Obtained = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            GB_ShinyOutfits = new System.Windows.Forms.GroupBox();
            CHK_Groudon = new System.Windows.Forms.CheckBox();
            CHK_Lucario = new System.Windows.Forms.CheckBox();
            CHK_Electivire = new System.Windows.Forms.CheckBox();
            CHK_Kyogre = new System.Windows.Forms.CheckBox();
            CHK_Roserade = new System.Windows.Forms.CheckBox();
            CHK_Pachirisu = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)DGV_Gear).BeginInit();
            GB_ShinyOutfits.SuspendLayout();
            SuspendLayout();
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(435, 381);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(88, 27);
            B_Save.TabIndex = 5;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(341, 381);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(88, 27);
            B_Cancel.TabIndex = 4;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_UnlockAll
            // 
            B_UnlockAll.Location = new System.Drawing.Point(13, 12);
            B_UnlockAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_UnlockAll.Name = "B_UnlockAll";
            B_UnlockAll.Size = new System.Drawing.Size(248, 27);
            B_UnlockAll.TabIndex = 0;
            B_UnlockAll.Text = "Unlock All Gear";
            B_UnlockAll.UseVisualStyleBackColor = true;
            B_UnlockAll.Click += B_UnlockAll_Click;
            // 
            // B_Clear
            // 
            B_Clear.Location = new System.Drawing.Point(269, 12);
            B_Clear.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Clear.Name = "B_Clear";
            B_Clear.Size = new System.Drawing.Size(248, 27);
            B_Clear.TabIndex = 1;
            B_Clear.Text = "Reset Gear to Default";
            B_Clear.UseVisualStyleBackColor = true;
            B_Clear.Click += B_Clear_Click;
            // 
            // DGV_Gear
            // 
            DGV_Gear.AllowUserToAddRows = false;
            DGV_Gear.AllowUserToDeleteRows = false;
            DGV_Gear.AllowUserToResizeColumns = false;
            DGV_Gear.AllowUserToResizeRows = false;
            DGV_Gear.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            DGV_Gear.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            DGV_Gear.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            DGV_Gear.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_Gear.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Index, CharacterStyle, Category, Gear, Obtained });
            DGV_Gear.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            DGV_Gear.Location = new System.Drawing.Point(13, 45);
            DGV_Gear.MultiSelect = false;
            DGV_Gear.Name = "DGV_Gear";
            DGV_Gear.RowHeadersVisible = false;
            DGV_Gear.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            DGV_Gear.Size = new System.Drawing.Size(505, 287);
            DGV_Gear.StandardTab = true;
            DGV_Gear.TabIndex = 2;
            // 
            // Index
            // 
            Index.HeaderText = "Index";
            Index.Name = "Index";
            Index.ReadOnly = true;
            Index.Visible = false;
            // 
            // Model
            // 
            CharacterStyle.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            CharacterStyle.HeaderText = "Character Style";
            CharacterStyle.Name = "Model";
            CharacterStyle.ReadOnly = true;
            // 
            // Category
            // 
            Category.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            Category.HeaderText = "Category";
            Category.Name = "Category";
            Category.ReadOnly = true;
            // 
            // Gear
            // 
            Gear.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            Gear.FillWeight = 150F;
            Gear.HeaderText = "Gear";
            Gear.Name = "Gear";
            Gear.ReadOnly = true;
            // 
            // Obtained
            // 
            Obtained.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            Obtained.HeaderText = "Obtained";
            Obtained.Name = "Obtained";
            Obtained.Width = 63;
            // 
            // GB_ShinyOutfits
            // 
            GB_ShinyOutfits.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            GB_ShinyOutfits.Controls.Add(CHK_Pachirisu);
            GB_ShinyOutfits.Controls.Add(CHK_Roserade);
            GB_ShinyOutfits.Controls.Add(CHK_Kyogre);
            GB_ShinyOutfits.Controls.Add(CHK_Electivire);
            GB_ShinyOutfits.Controls.Add(CHK_Lucario);
            GB_ShinyOutfits.Controls.Add(CHK_Groudon);
            GB_ShinyOutfits.Location = new System.Drawing.Point(13, 338);
            GB_ShinyOutfits.Name = "GB_ShinyOutfits";
            GB_ShinyOutfits.Size = new System.Drawing.Size(303, 70);
            GB_ShinyOutfits.TabIndex = 3;
            GB_ShinyOutfits.TabStop = false;
            GB_ShinyOutfits.Text = "Shiny Outfits";
            // 
            // CHK_Groudon
            // 
            CHK_Groudon.AutoSize = true;
            CHK_Groudon.Location = new System.Drawing.Point(7, 18);
            CHK_Groudon.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_Groudon.Name = "CHK_Groudon";
            CHK_Groudon.Size = new System.Drawing.Size(73, 19);
            CHK_Groudon.TabIndex = 0;
            CHK_Groudon.Text = "Groudon";
            CHK_Groudon.UseVisualStyleBackColor = true;
            // 
            // CHK_Lucario
            // 
            CHK_Lucario.AutoSize = true;
            CHK_Lucario.Location = new System.Drawing.Point(106, 18);
            CHK_Lucario.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_Lucario.Name = "CHK_Lucario";
            CHK_Lucario.Size = new System.Drawing.Size(65, 19);
            CHK_Lucario.TabIndex = 1;
            CHK_Lucario.Text = "Lucario";
            CHK_Lucario.UseVisualStyleBackColor = true;
            // 
            // CHK_Electivire
            // 
            CHK_Electivire.AutoSize = true;
            CHK_Electivire.Location = new System.Drawing.Point(205, 18);
            CHK_Electivire.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_Electivire.Name = "CHK_Electivire";
            CHK_Electivire.Size = new System.Drawing.Size(73, 19);
            CHK_Electivire.TabIndex = 2;
            CHK_Electivire.Text = "Electivire";
            CHK_Electivire.UseVisualStyleBackColor = true;
            // 
            // CHK_Kyogre
            // 
            CHK_Kyogre.AutoSize = true;
            CHK_Kyogre.Location = new System.Drawing.Point(7, 42);
            CHK_Kyogre.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_Kyogre.Name = "CHK_Kyogre";
            CHK_Kyogre.Size = new System.Drawing.Size(62, 19);
            CHK_Kyogre.TabIndex = 3;
            CHK_Kyogre.Text = "Kyogre";
            CHK_Kyogre.UseVisualStyleBackColor = true;
            // 
            // CHK_Roserade
            // 
            CHK_Roserade.AutoSize = true;
            CHK_Roserade.Location = new System.Drawing.Point(106, 42);
            CHK_Roserade.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_Roserade.Name = "CHK_Roserade";
            CHK_Roserade.Size = new System.Drawing.Size(74, 19);
            CHK_Roserade.TabIndex = 4;
            CHK_Roserade.Text = "Roserade";
            CHK_Roserade.UseVisualStyleBackColor = true;
            // 
            // CHK_Pachirisu
            // 
            CHK_Pachirisu.AutoSize = true;
            CHK_Pachirisu.Location = new System.Drawing.Point(205, 42);
            CHK_Pachirisu.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_Pachirisu.Name = "CHK_Pachirisu";
            CHK_Pachirisu.Size = new System.Drawing.Size(74, 19);
            CHK_Pachirisu.TabIndex = 5;
            CHK_Pachirisu.Text = "Pachirisu";
            CHK_Pachirisu.UseVisualStyleBackColor = true;
            // 
            // SAV_Gear
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(530, 417);
            Controls.Add(GB_ShinyOutfits);
            Controls.Add(DGV_Gear);
            Controls.Add(B_Clear);
            Controls.Add(B_UnlockAll);
            Controls.Add(B_Cancel);
            Controls.Add(B_Save);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(2);
            MaximizeBox = false;
            Name = "SAV_Gear";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Gear Editor";
            ((System.ComponentModel.ISupportInitialize)DGV_Gear).EndInit();
            GB_ShinyOutfits.ResumeLayout(false);
            GB_ShinyOutfits.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_UnlockAll;
        private System.Windows.Forms.Button B_Clear;
        private System.Windows.Forms.DataGridView DGV_Gear;
        private System.Windows.Forms.DataGridViewTextBoxColumn Index;
        private System.Windows.Forms.DataGridViewTextBoxColumn CharacterStyle;
        private System.Windows.Forms.DataGridViewTextBoxColumn Category;
        private System.Windows.Forms.DataGridViewTextBoxColumn Gear;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Obtained;
        private System.Windows.Forms.GroupBox GB_ShinyOutfits;
        private System.Windows.Forms.CheckBox CHK_Groudon;
        private System.Windows.Forms.CheckBox CHK_Lucario;
        private System.Windows.Forms.CheckBox CHK_Electivire;
        private System.Windows.Forms.CheckBox CHK_Kyogre;
        private System.Windows.Forms.CheckBox CHK_Roserade;
        private System.Windows.Forms.CheckBox CHK_Pachirisu;
    }
}
