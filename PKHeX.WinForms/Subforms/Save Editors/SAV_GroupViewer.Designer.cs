namespace PKHeX.WinForms
{
    sealed partial class SAV_GroupViewer
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
            this.components = new System.ComponentModel.Container();
            this.B_BoxRight = new System.Windows.Forms.Button();
            this.B_BoxLeft = new System.Windows.Forms.Button();
            this.CB_BoxSelect = new System.Windows.Forms.ComboBox();
            this.mnu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
            this.Box = new PKHeX.WinForms.Controls.PokeGrid();
            this.mnu.SuspendLayout();
            this.SuspendLayout();
            // 
            // B_BoxRight
            // 
            this.B_BoxRight.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.B_BoxRight.Location = new System.Drawing.Point(206, 3);
            this.B_BoxRight.Name = "B_BoxRight";
            this.B_BoxRight.Size = new System.Drawing.Size(27, 23);
            this.B_BoxRight.TabIndex = 68;
            this.B_BoxRight.Text = ">>";
            this.B_BoxRight.UseVisualStyleBackColor = true;
            this.B_BoxRight.Click += new System.EventHandler(this.B_BoxRight_Click);
            // 
            // B_BoxLeft
            // 
            this.B_BoxLeft.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.B_BoxLeft.Location = new System.Drawing.Point(16, 3);
            this.B_BoxLeft.Name = "B_BoxLeft";
            this.B_BoxLeft.Size = new System.Drawing.Size(27, 23);
            this.B_BoxLeft.TabIndex = 67;
            this.B_BoxLeft.Text = "<<";
            this.B_BoxLeft.UseVisualStyleBackColor = true;
            this.B_BoxLeft.Click += new System.EventHandler(this.B_BoxLeft_Click);
            // 
            // CB_BoxSelect
            // 
            this.CB_BoxSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_BoxSelect.DropDownWidth = 252;
            this.CB_BoxSelect.FormattingEnabled = true;
            this.CB_BoxSelect.Location = new System.Drawing.Point(48, 4);
            this.CB_BoxSelect.Name = "CB_BoxSelect";
            this.CB_BoxSelect.Size = new System.Drawing.Size(152, 21);
            this.CB_BoxSelect.TabIndex = 66;
            this.CB_BoxSelect.SelectedIndexChanged += new System.EventHandler(this.CB_BoxSelect_SelectedIndexChanged);
            // 
            // mnu
            // 
            this.mnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuView});
            this.mnu.Name = "mnu";
            this.mnu.Size = new System.Drawing.Size(100, 26);
            // 
            // mnuView
            // 
            this.mnuView.Image = global::PKHeX.WinForms.Properties.Resources.other;
            this.mnuView.Name = "mnuView";
            this.mnuView.Size = new System.Drawing.Size(99, 22);
            this.mnuView.Text = "View";
            this.mnuView.Click += new System.EventHandler(this.ClickView);
            // 
            // Box
            // 
            this.Box.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Box.Location = new System.Drawing.Point(0, 29);
            this.Box.Margin = new System.Windows.Forms.Padding(0);
            this.Box.Name = "Box";
            this.Box.Size = new System.Drawing.Size(249, 153);
            this.Box.TabIndex = 0;
            // 
            // SAV_GroupViewer
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(250, 183);
            this.Controls.Add(this.B_BoxRight);
            this.Controls.Add(this.B_BoxLeft);
            this.Controls.Add(this.CB_BoxSelect);
            this.Controls.Add(this.Box);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SAV_GroupViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Group Viewer";
            this.mnu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.PokeGrid Box;
        public System.Windows.Forms.Button B_BoxRight;
        public System.Windows.Forms.Button B_BoxLeft;
        public System.Windows.Forms.ComboBox CB_BoxSelect;
        private System.Windows.Forms.ContextMenuStrip mnu;
        private System.Windows.Forms.ToolStripMenuItem mnuView;
    }
}