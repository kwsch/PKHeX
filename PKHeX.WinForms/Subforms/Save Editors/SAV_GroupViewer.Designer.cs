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
            components = new System.ComponentModel.Container();
            B_BoxRight = new System.Windows.Forms.Button();
            B_BoxLeft = new System.Windows.Forms.Button();
            CB_BoxSelect = new System.Windows.Forms.ComboBox();
            mnu = new System.Windows.Forms.ContextMenuStrip(components);
            mnuView = new System.Windows.Forms.ToolStripMenuItem();
            Box = new Controls.PokeGrid();
            mnu.SuspendLayout();
            SuspendLayout();
            // 
            // B_BoxRight
            // 
            B_BoxRight.FlatStyle = System.Windows.Forms.FlatStyle.System;
            B_BoxRight.Location = new System.Drawing.Point(240, 3);
            B_BoxRight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_BoxRight.Name = "B_BoxRight";
            B_BoxRight.Size = new System.Drawing.Size(31, 27);
            B_BoxRight.TabIndex = 68;
            B_BoxRight.Text = ">>";
            B_BoxRight.UseVisualStyleBackColor = true;
            B_BoxRight.Click += B_BoxRight_Click;
            // 
            // B_BoxLeft
            // 
            B_BoxLeft.FlatStyle = System.Windows.Forms.FlatStyle.System;
            B_BoxLeft.Location = new System.Drawing.Point(19, 3);
            B_BoxLeft.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_BoxLeft.Name = "B_BoxLeft";
            B_BoxLeft.Size = new System.Drawing.Size(31, 27);
            B_BoxLeft.TabIndex = 67;
            B_BoxLeft.Text = "<<";
            B_BoxLeft.UseVisualStyleBackColor = true;
            B_BoxLeft.Click += B_BoxLeft_Click;
            // 
            // CB_BoxSelect
            // 
            CB_BoxSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_BoxSelect.DropDownWidth = 252;
            CB_BoxSelect.FormattingEnabled = true;
            CB_BoxSelect.Location = new System.Drawing.Point(56, 5);
            CB_BoxSelect.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_BoxSelect.Name = "CB_BoxSelect";
            CB_BoxSelect.Size = new System.Drawing.Size(177, 23);
            CB_BoxSelect.TabIndex = 66;
            CB_BoxSelect.SelectedIndexChanged += CB_BoxSelect_SelectedIndexChanged;
            // 
            // mnu
            // 
            mnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuView });
            mnu.Name = "mnu";
            mnu.Size = new System.Drawing.Size(100, 26);
            // 
            // mnuView
            // 
            mnuView.Image = Properties.Resources.other;
            mnuView.Name = "mnuView";
            mnuView.Size = new System.Drawing.Size(99, 22);
            mnuView.Text = "View";
            mnuView.Click += ClickView;
            // 
            // Box
            // 
            Box.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            Box.Location = new System.Drawing.Point(0, 33);
            Box.Margin = new System.Windows.Forms.Padding(0);
            Box.Name = "Box";
            Box.Size = new System.Drawing.Size(290, 177);
            Box.TabIndex = 0;
            // 
            // SAV_GroupViewer
            // 
            AllowDrop = true;
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(292, 211);
            Controls.Add(B_BoxRight);
            Controls.Add(B_BoxLeft);
            Controls.Add(CB_BoxSelect);
            Controls.Add(Box);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_GroupViewer";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Group Viewer";
            mnu.ResumeLayout(false);
            ResumeLayout(false);
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
