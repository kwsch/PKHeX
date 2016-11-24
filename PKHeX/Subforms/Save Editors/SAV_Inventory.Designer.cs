namespace PKHeX
{
    partial class SAV_Inventory
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SAV_Inventory));
            this.B_Cancel = new System.Windows.Forms.Button();
            this.B_Save = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.IL_Pouch = new System.Windows.Forms.ImageList(this.components);
            this.B_GiveAll = new System.Windows.Forms.Button();
            this.B_Sort = new System.Windows.Forms.Button();
            this.sortMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuSortNameReverse = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSortCountReverse = new System.Windows.Forms.ToolStripMenuItem();
            this.giveMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.giveNone = new System.Windows.Forms.ToolStripMenuItem();
            this.giveAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSortName = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSortCount = new System.Windows.Forms.ToolStripMenuItem();
            this.sortMenu.SuspendLayout();
            this.giveMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(177, 328);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(70, 23);
            this.B_Cancel.TabIndex = 14;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(253, 328);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(70, 23);
            this.B_Save.TabIndex = 15;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.ImageList = this.IL_Pouch;
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(311, 308);
            this.tabControl1.TabIndex = 17;
            // 
            // IL_Pouch
            // 
            this.IL_Pouch.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("IL_Pouch.ImageStream")));
            this.IL_Pouch.TransparentColor = System.Drawing.Color.Transparent;
            this.IL_Pouch.Images.SetKeyName(0, "Bag_Items.png");
            this.IL_Pouch.Images.SetKeyName(1, "Bag_Key.png");
            this.IL_Pouch.Images.SetKeyName(2, "Bag_TMHM.png");
            this.IL_Pouch.Images.SetKeyName(3, "Bag_Medicine.png");
            this.IL_Pouch.Images.SetKeyName(4, "Bag_Berries.png");
            this.IL_Pouch.Images.SetKeyName(5, "Bag_Balls.png");
            this.IL_Pouch.Images.SetKeyName(6, "Bag_Battle.png");
            this.IL_Pouch.Images.SetKeyName(7, "Bag_Mail.png");
            this.IL_Pouch.Images.SetKeyName(8, "Bag_PCItems.png");
            this.IL_Pouch.Images.SetKeyName(9, "Bag_Free.png");
            this.IL_Pouch.Images.SetKeyName(10, "Bag_Z.png");
            // 
            // B_GiveAll
            // 
            this.B_GiveAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.B_GiveAll.Location = new System.Drawing.Point(12, 328);
            this.B_GiveAll.Name = "B_GiveAll";
            this.B_GiveAll.Size = new System.Drawing.Size(75, 23);
            this.B_GiveAll.TabIndex = 18;
            this.B_GiveAll.Text = "Give All";
            this.B_GiveAll.UseVisualStyleBackColor = true;
            this.B_GiveAll.Click += new System.EventHandler(this.B_GiveAll_Click);
            // 
            // B_Sort
            // 
            this.B_Sort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.B_Sort.ContextMenuStrip = this.sortMenu;
            this.B_Sort.Location = new System.Drawing.Point(93, 328);
            this.B_Sort.Name = "B_Sort";
            this.B_Sort.Size = new System.Drawing.Size(75, 23);
            this.B_Sort.TabIndex = 19;
            this.B_Sort.Text = "Sort";
            this.B_Sort.UseVisualStyleBackColor = true;
            this.B_Sort.Click += new System.EventHandler(this.B_Sort_Click);
            // 
            // sortMenu
            // 
            this.sortMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSortName,
            this.mnuSortNameReverse,
            this.mnuSortCount,
            this.mnuSortCountReverse});
            this.sortMenu.Name = "modifyMenu";
            this.sortMenu.Size = new System.Drawing.Size(159, 114);
            // 
            // mnuSortNameReverse
            // 
            this.mnuSortNameReverse.Name = "mnuSortNameReverse";
            this.mnuSortNameReverse.Size = new System.Drawing.Size(158, 22);
            this.mnuSortNameReverse.Text = "Name (Reverse)";
            this.mnuSortNameReverse.Click += new System.EventHandler(this.sortByName);
            // 
            // mnuSortCountReverse
            // 
            this.mnuSortCountReverse.Name = "mnuSortCountReverse";
            this.mnuSortCountReverse.Size = new System.Drawing.Size(158, 22);
            this.mnuSortCountReverse.Text = "Count (Reverse)";
            this.mnuSortCountReverse.Click += new System.EventHandler(this.sortByCount);
            // 
            // giveMenu
            // 
            this.giveMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.giveAll,
            this.giveNone});
            this.giveMenu.Name = "modifyMenu";
            this.giveMenu.Size = new System.Drawing.Size(104, 48);
            // 
            // giveNone
            // 
            this.giveNone.Name = "giveNone";
            this.giveNone.Size = new System.Drawing.Size(103, 22);
            this.giveNone.Text = "None";
            this.giveNone.Click += new System.EventHandler(this.removeAllItems);
            // 
            // giveAll
            // 
            this.giveAll.Name = "giveAll";
            this.giveAll.Size = new System.Drawing.Size(152, 22);
            this.giveAll.Text = "All";
            this.giveAll.Click += new System.EventHandler(this.giveAllItems);
            // 
            // mnuSortName
            // 
            this.mnuSortName.Name = "mnuSortName";
            this.mnuSortName.Size = new System.Drawing.Size(158, 22);
            this.mnuSortName.Text = "Name";
            this.mnuSortName.Click += new System.EventHandler(this.sortByName);
            // 
            // mnuSortCount
            // 
            this.mnuSortCount.Name = "mnuSortCount";
            this.mnuSortCount.Size = new System.Drawing.Size(158, 22);
            this.mnuSortCount.Text = "Count";
            this.mnuSortCount.Click += new System.EventHandler(this.sortByCount);
            // 
            // SAV_Inventory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 361);
            this.Controls.Add(this.B_Sort);
            this.Controls.Add(this.B_GiveAll);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_Cancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(350, 400);
            this.Name = "SAV_Inventory";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Inventory Editor";
            this.sortMenu.ResumeLayout(false);
            this.giveMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Button B_GiveAll;
        private System.Windows.Forms.ImageList IL_Pouch;
        private System.Windows.Forms.Button B_Sort;
        private System.Windows.Forms.ContextMenuStrip sortMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuSortNameReverse;
        private System.Windows.Forms.ToolStripMenuItem mnuSortCountReverse;
        private System.Windows.Forms.ContextMenuStrip giveMenu;
        private System.Windows.Forms.ToolStripMenuItem giveNone;
        private System.Windows.Forms.ToolStripMenuItem giveAll;
        private System.Windows.Forms.ToolStripMenuItem mnuSortName;
        private System.Windows.Forms.ToolStripMenuItem mnuSortCount;
    }
}