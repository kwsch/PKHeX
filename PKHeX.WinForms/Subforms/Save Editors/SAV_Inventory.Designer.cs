namespace PKHeX.WinForms
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SAV_Inventory));
            B_Cancel = new System.Windows.Forms.Button();
            B_Save = new System.Windows.Forms.Button();
            tabControl1 = new System.Windows.Forms.TabControl();
            B_GiveAll = new System.Windows.Forms.Button();
            B_Sort = new System.Windows.Forms.Button();
            sortMenu = new System.Windows.Forms.ContextMenuStrip(components);
            mnuSortName = new System.Windows.Forms.ToolStripMenuItem();
            mnuSortNameReverse = new System.Windows.Forms.ToolStripMenuItem();
            mnuSortCount = new System.Windows.Forms.ToolStripMenuItem();
            mnuSortCountReverse = new System.Windows.Forms.ToolStripMenuItem();
            mnuSortIndex = new System.Windows.Forms.ToolStripMenuItem();
            mnuSortIndexReverse = new System.Windows.Forms.ToolStripMenuItem();
            giveMenu = new System.Windows.Forms.ContextMenuStrip(components);
            giveAll = new System.Windows.Forms.ToolStripMenuItem();
            giveNone = new System.Windows.Forms.ToolStripMenuItem();
            giveModify = new System.Windows.Forms.ToolStripMenuItem();
            L_Count = new System.Windows.Forms.Label();
            NUD_Count = new System.Windows.Forms.NumericUpDown();
            sortMenu.SuspendLayout();
            giveMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(NUD_Count)).BeginInit();
            SuspendLayout();
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            B_Cancel.Location = new System.Drawing.Point(232, 378);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(70, 23);
            B_Cancel.TabIndex = 14;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += new System.EventHandler(B_Cancel_Click);
            // 
            // B_Save
            // 
            B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            B_Save.Location = new System.Drawing.Point(232, 354);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(70, 23);
            B_Save.TabIndex = 15;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += new System.EventHandler(B_Save_Click);
            // 
            // tabControl1
            // 
            tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            tabControl1.Location = new System.Drawing.Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(291, 336);
            tabControl1.TabIndex = 17;
            tabControl1.SelectedIndexChanged += new System.EventHandler(SwitchBag);
            // 
            // B_GiveAll
            // 
            B_GiveAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            B_GiveAll.Location = new System.Drawing.Point(12, 378);
            B_GiveAll.Name = "B_GiveAll";
            B_GiveAll.Size = new System.Drawing.Size(75, 23);
            B_GiveAll.TabIndex = 18;
            B_GiveAll.Text = "Give All";
            B_GiveAll.UseVisualStyleBackColor = true;
            B_GiveAll.Click += new System.EventHandler(B_GiveAll_Click);
            // 
            // B_Sort
            // 
            B_Sort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            B_Sort.ContextMenuStrip = sortMenu;
            B_Sort.Location = new System.Drawing.Point(12, 354);
            B_Sort.Name = "B_Sort";
            B_Sort.Size = new System.Drawing.Size(75, 23);
            B_Sort.TabIndex = 19;
            B_Sort.Text = "Sort";
            B_Sort.UseVisualStyleBackColor = true;
            B_Sort.Click += new System.EventHandler(B_Sort_Click);
            // 
            // sortMenu
            // 
            sortMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            mnuSortName,
            mnuSortNameReverse,
            mnuSortCount,
            mnuSortCountReverse,
            mnuSortIndex,
            mnuSortIndexReverse});
            sortMenu.Name = "modifyMenu";
            sortMenu.Size = new System.Drawing.Size(159, 136);
            // 
            // mnuSortName
            // 
            mnuSortName.Image = global::PKHeX.WinForms.Properties.Resources.alphaAZ;
            mnuSortName.Name = "mnuSortName";
            mnuSortName.Size = new System.Drawing.Size(158, 22);
            mnuSortName.Text = "Name";
            mnuSortName.Click += new System.EventHandler(SortByName);
            // 
            // mnuSortNameReverse
            // 
            mnuSortNameReverse.Image = global::PKHeX.WinForms.Properties.Resources.alphaZA;
            mnuSortNameReverse.Name = "mnuSortNameReverse";
            mnuSortNameReverse.Size = new System.Drawing.Size(158, 22);
            mnuSortNameReverse.Text = "Name (Reverse)";
            mnuSortNameReverse.Click += new System.EventHandler(SortByName);
            // 
            // mnuSortCount
            // 
            mnuSortCount.Image = global::PKHeX.WinForms.Properties.Resources.numlohi;
            mnuSortCount.Name = "mnuSortCount";
            mnuSortCount.Size = new System.Drawing.Size(158, 22);
            mnuSortCount.Text = "Count";
            mnuSortCount.Click += new System.EventHandler(SortByCount);
            // 
            // mnuSortCountReverse
            // 
            mnuSortCountReverse.Image = global::PKHeX.WinForms.Properties.Resources.numhilo;
            mnuSortCountReverse.Name = "mnuSortCountReverse";
            mnuSortCountReverse.Size = new System.Drawing.Size(158, 22);
            mnuSortCountReverse.Text = "Count (Reverse)";
            mnuSortCountReverse.Click += new System.EventHandler(SortByCount);
            // 
            // mnuSortIndex
            // 
            mnuSortIndex.Image = global::PKHeX.WinForms.Properties.Resources.numlohi;
            mnuSortIndex.Name = "mnuSortIndex";
            mnuSortIndex.Size = new System.Drawing.Size(158, 22);
            mnuSortIndex.Text = "Index";
            mnuSortIndex.Click += new System.EventHandler(SortByIndex);
            // 
            // mnuSortIndexReverse
            // 
            mnuSortIndexReverse.Image = global::PKHeX.WinForms.Properties.Resources.numhilo;
            mnuSortIndexReverse.Name = "mnuSortIndexReverse";
            mnuSortIndexReverse.Size = new System.Drawing.Size(158, 22);
            mnuSortIndexReverse.Text = "Index (Reverse)";
            mnuSortIndexReverse.Click += new System.EventHandler(SortByIndex);
            // 
            // giveMenu
            // 
            giveMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            giveAll,
            giveNone,
            giveModify});
            giveMenu.Name = "modifyMenu";
            giveMenu.Size = new System.Drawing.Size(113, 70);
            // 
            // giveAll
            // 
            giveAll.Image = global::PKHeX.WinForms.Properties.Resources.database;
            giveAll.Name = "giveAll";
            giveAll.Size = new System.Drawing.Size(112, 22);
            giveAll.Text = "All";
            giveAll.Click += new System.EventHandler(GiveAllItems);
            // 
            // giveNone
            // 
            giveNone.Image = global::PKHeX.WinForms.Properties.Resources.open;
            giveNone.Name = "giveNone";
            giveNone.Size = new System.Drawing.Size(112, 22);
            giveNone.Text = "None";
            giveNone.Click += new System.EventHandler(RemoveAllItems);
            // 
            // giveModify
            // 
            giveModify.Image = global::PKHeX.WinForms.Properties.Resources.settings;
            giveModify.Name = "giveModify";
            giveModify.Size = new System.Drawing.Size(112, 22);
            giveModify.Text = "Modify";
            giveModify.Click += new System.EventHandler(ModifyAllItems);
            // 
            // L_Count
            // 
            L_Count.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            L_Count.AutoSize = true;
            L_Count.Location = new System.Drawing.Point(92, 367);
            L_Count.Name = "L_Count";
            L_Count.Size = new System.Drawing.Size(38, 13);
            L_Count.TabIndex = 20;
            L_Count.Text = "Count:";
            // 
            // NUD_Count
            // 
            NUD_Count.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            NUD_Count.Location = new System.Drawing.Point(93, 381);
            NUD_Count.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            NUD_Count.Name = "NUD_Count";
            NUD_Count.Size = new System.Drawing.Size(49, 20);
            NUD_Count.TabIndex = 21;
            NUD_Count.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // SAV_Inventory
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(314, 411);
            Controls.Add(NUD_Count);
            Controls.Add(L_Count);
            Controls.Add(B_Sort);
            Controls.Add(B_GiveAll);
            Controls.Add(tabControl1);
            Controls.Add(B_Save);
            Controls.Add(B_Cancel);
            Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(330, 450);
            Name = "SAV_Inventory";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Inventory Editor";
            sortMenu.ResumeLayout(false);
            giveMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(NUD_Count)).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Button B_GiveAll;
        private System.Windows.Forms.Button B_Sort;
        private System.Windows.Forms.ContextMenuStrip sortMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuSortNameReverse;
        private System.Windows.Forms.ToolStripMenuItem mnuSortCountReverse;
        private System.Windows.Forms.ContextMenuStrip giveMenu;
        private System.Windows.Forms.ToolStripMenuItem giveNone;
        private System.Windows.Forms.ToolStripMenuItem giveAll;
        private System.Windows.Forms.ToolStripMenuItem mnuSortName;
        private System.Windows.Forms.ToolStripMenuItem mnuSortCount;
        private System.Windows.Forms.Label L_Count;
        private System.Windows.Forms.NumericUpDown NUD_Count;
        private System.Windows.Forms.ToolStripMenuItem giveModify;
        private System.Windows.Forms.ToolStripMenuItem mnuSortIndex;
        private System.Windows.Forms.ToolStripMenuItem mnuSortIndexReverse;
    }
}
