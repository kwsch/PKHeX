namespace PKHeX.WinForms.Controls
{
    partial class BoxEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            B_BoxRight = new System.Windows.Forms.Button();
            B_BoxLeft = new System.Windows.Forms.Button();
            CB_BoxSelect = new System.Windows.Forms.ComboBox();
            BoxPokeGrid = new PokeGrid();
            TB_SearchBox = new System.Windows.Forms.TextBox();
            L_SearchBox = new System.Windows.Forms.Label();
            B_SearchGlobal = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // B_BoxRight
            // 
            B_BoxRight.FlatStyle = System.Windows.Forms.FlatStyle.System;
            B_BoxRight.Location = new System.Drawing.Point(200, 0);
            B_BoxRight.Margin = new System.Windows.Forms.Padding(0);
            B_BoxRight.Name = "B_BoxRight";
            B_BoxRight.Size = new System.Drawing.Size(24, 24);
            B_BoxRight.TabIndex = 2;
            B_BoxRight.Text = ">>";
            B_BoxRight.UseVisualStyleBackColor = true;
            B_BoxRight.Click += ClickBoxRight;
            // 
            // B_BoxLeft
            // 
            B_BoxLeft.FlatStyle = System.Windows.Forms.FlatStyle.System;
            B_BoxLeft.Location = new System.Drawing.Point(32, 0);
            B_BoxLeft.Margin = new System.Windows.Forms.Padding(0);
            B_BoxLeft.Name = "B_BoxLeft";
            B_BoxLeft.Size = new System.Drawing.Size(24, 24);
            B_BoxLeft.TabIndex = 0;
            B_BoxLeft.Text = "<<";
            B_BoxLeft.UseVisualStyleBackColor = true;
            B_BoxLeft.Click += ClickBoxLeft;
            // 
            // CB_BoxSelect
            // 
            CB_BoxSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_BoxSelect.FormattingEnabled = true;
            CB_BoxSelect.Items.AddRange(new object[] { "Box 1", "Box 2", "Box 3", "Box 4", "Box 5", "Box 6", "Box 7", "Box 8", "Box 9", "Box 10", "Box 11", "Box 12", "Box 13", "Box 14", "Box 15", "Box 16", "Box 17", "Box 18", "Box 19", "Box 20", "Box 21", "Box 22", "Box 23", "Box 24", "Box 25", "Box 26", "Box 27", "Box 28", "Box 29", "Box 30", "Box 31" });
            CB_BoxSelect.Location = new System.Drawing.Point(64, 0);
            CB_BoxSelect.Margin = new System.Windows.Forms.Padding(0);
            CB_BoxSelect.MinimumSize = new System.Drawing.Size(128, 0);
            CB_BoxSelect.Name = "CB_BoxSelect";
            CB_BoxSelect.Size = new System.Drawing.Size(128, 25);
            CB_BoxSelect.TabIndex = 1;
            CB_BoxSelect.SelectedIndexChanged += GetBox;
            // 
            // BoxPokeGrid
            // 
            BoxPokeGrid.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            BoxPokeGrid.Location = new System.Drawing.Point(0, 52);
            BoxPokeGrid.Margin = new System.Windows.Forms.Padding(0);
            BoxPokeGrid.Name = "BoxPokeGrid";
            BoxPokeGrid.Size = new System.Drawing.Size(251, 160);
            BoxPokeGrid.TabIndex = 3;
            // 
            // TB_SearchBox
            // 
            TB_SearchBox.Location = new System.Drawing.Point(64, 26);
            TB_SearchBox.Name = "TB_SearchBox";
            TB_SearchBox.Size = new System.Drawing.Size(130, 23);
            TB_SearchBox.TabIndex = 4;
            TB_SearchBox.PlaceholderText = "Search Pokémon...";
            TB_SearchBox.TextChanged += SearchBox_TextChanged;
            // 
            // B_SearchGlobal
            // 
            B_SearchGlobal.FlatStyle = System.Windows.Forms.FlatStyle.System;
            B_SearchGlobal.Location = new System.Drawing.Point(196, 26);
            B_SearchGlobal.Margin = new System.Windows.Forms.Padding(0);
            B_SearchGlobal.Name = "B_SearchGlobal";
            B_SearchGlobal.Size = new System.Drawing.Size(28, 23);
            B_SearchGlobal.TabIndex = 6;
            B_SearchGlobal.Text = "🔍";
            B_SearchGlobal.UseVisualStyleBackColor = true;
            B_SearchGlobal.Click += SearchGlobal_Click;
            // 
            // L_SearchBox
            // 
            L_SearchBox.AutoSize = true;
            L_SearchBox.Location = new System.Drawing.Point(3, 29);
            L_SearchBox.Name = "L_SearchBox";
            L_SearchBox.Size = new System.Drawing.Size(45, 15);
            L_SearchBox.TabIndex = 5;
            L_SearchBox.Text = "Search:";
            // 
            // BoxEditor
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            AutoSize = true;
            Controls.Add(B_SearchGlobal);
            Controls.Add(L_SearchBox);
            Controls.Add(TB_SearchBox);
            Controls.Add(BoxPokeGrid);
            Controls.Add(B_BoxRight);
            Controls.Add(B_BoxLeft);
            Controls.Add(CB_BoxSelect);
            Name = "BoxEditor";
            Size = new System.Drawing.Size(251, 212);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        public System.Windows.Forms.Button B_BoxRight;
        public System.Windows.Forms.Button B_BoxLeft;
        public System.Windows.Forms.ComboBox CB_BoxSelect;
        public PokeGrid BoxPokeGrid;
        private System.Windows.Forms.TextBox TB_SearchBox;
        private System.Windows.Forms.Label L_SearchBox;
        private System.Windows.Forms.Button B_SearchGlobal;
    }
}
