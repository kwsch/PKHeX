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
            this.B_BoxRight = new System.Windows.Forms.Button();
            this.B_BoxLeft = new System.Windows.Forms.Button();
            this.CB_BoxSelect = new System.Windows.Forms.ComboBox();
            this.BoxPokeGrid = new PKHeX.WinForms.Controls.PokeGrid();
            this.SuspendLayout();
            // 
            // B_BoxRight
            // 
            this.B_BoxRight.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.B_BoxRight.Location = new System.Drawing.Point(193, -1);
            this.B_BoxRight.Name = "B_BoxRight";
            this.B_BoxRight.Size = new System.Drawing.Size(27, 23);
            this.B_BoxRight.TabIndex = 65;
            this.B_BoxRight.Text = ">>";
            this.B_BoxRight.UseVisualStyleBackColor = true;
            this.B_BoxRight.Click += new System.EventHandler(this.ClickBoxRight);
            // 
            // B_BoxLeft
            // 
            this.B_BoxLeft.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.B_BoxLeft.Location = new System.Drawing.Point(31, -1);
            this.B_BoxLeft.Name = "B_BoxLeft";
            this.B_BoxLeft.Size = new System.Drawing.Size(27, 23);
            this.B_BoxLeft.TabIndex = 64;
            this.B_BoxLeft.Text = "<<";
            this.B_BoxLeft.UseVisualStyleBackColor = true;
            this.B_BoxLeft.Click += new System.EventHandler(this.ClickBoxLeft);
            // 
            // CB_BoxSelect
            // 
            this.CB_BoxSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_BoxSelect.FormattingEnabled = true;
            this.CB_BoxSelect.Items.AddRange(new object[] {
            "Box 1",
            "Box 2",
            "Box 3",
            "Box 4",
            "Box 5",
            "Box 6",
            "Box 7",
            "Box 8",
            "Box 9",
            "Box 10",
            "Box 11",
            "Box 12",
            "Box 13",
            "Box 14",
            "Box 15",
            "Box 16",
            "Box 17",
            "Box 18",
            "Box 19",
            "Box 20",
            "Box 21",
            "Box 22",
            "Box 23",
            "Box 24",
            "Box 25",
            "Box 26",
            "Box 27",
            "Box 28",
            "Box 29",
            "Box 30",
            "Box 31"});
            this.CB_BoxSelect.Location = new System.Drawing.Point(62, 0);
            this.CB_BoxSelect.Name = "CB_BoxSelect";
            this.CB_BoxSelect.Size = new System.Drawing.Size(127, 21);
            this.CB_BoxSelect.TabIndex = 63;
            this.CB_BoxSelect.SelectedIndexChanged += new System.EventHandler(this.GetBox);
            // 
            // BoxPokeGrid
            // 
            this.BoxPokeGrid.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BoxPokeGrid.Location = new System.Drawing.Point(0, 25);
            this.BoxPokeGrid.Margin = new System.Windows.Forms.Padding(0);
            this.BoxPokeGrid.Name = "BoxPokeGrid";
            this.BoxPokeGrid.Size = new System.Drawing.Size(251, 160);
            this.BoxPokeGrid.TabIndex = 66;
            // 
            // BoxEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSize = true;
            this.Controls.Add(this.BoxPokeGrid);
            this.Controls.Add(this.B_BoxRight);
            this.Controls.Add(this.B_BoxLeft);
            this.Controls.Add(this.CB_BoxSelect);
            this.Name = "BoxEditor";
            this.Size = new System.Drawing.Size(251, 185);
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.Button B_BoxRight;
        public System.Windows.Forms.Button B_BoxLeft;
        public System.Windows.Forms.ComboBox CB_BoxSelect;
        public PokeGrid BoxPokeGrid;
    }
}
