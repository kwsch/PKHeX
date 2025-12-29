namespace PKHeX.WinForms
{
    partial class SAV_Donut9a
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
            B_Cancel = new System.Windows.Forms.Button();
            LB_Donut = new System.Windows.Forms.ListBox();
            B_Save = new System.Windows.Forms.Button();
            modifyMenu = new System.Windows.Forms.ContextMenuStrip(components);
            mnuRandomizeMax = new System.Windows.Forms.ToolStripMenuItem();
            mnuCloneCurrent = new System.Windows.Forms.ToolStripMenuItem();
            mnuShinyAssortment = new System.Windows.Forms.ToolStripMenuItem();
            B_ModifyAll = new System.Windows.Forms.Button();
            B_Reset = new System.Windows.Forms.Button();
            donutEditor = new DonutEditor9a();
            B_Import = new System.Windows.Forms.Button();
            B_Export = new System.Windows.Forms.Button();
            DonutFlavorProfile = new DonutFlavorProfile9a();
            modifyMenu.SuspendLayout();
            SuspendLayout();
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(786, 442);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(93, 27);
            B_Cancel.TabIndex = 0;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // LB_Donut
            // 
            LB_Donut.AllowDrop = true;
            LB_Donut.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            LB_Donut.FormattingEnabled = true;
            LB_Donut.Location = new System.Drawing.Point(14, 15);
            LB_Donut.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LB_Donut.Name = "LB_Donut";
            LB_Donut.Size = new System.Drawing.Size(186, 412);
            LB_Donut.TabIndex = 2;
            LB_Donut.SelectedIndexChanged += ChangeIndex;
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(786, 413);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(93, 27);
            B_Save.TabIndex = 24;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // modifyMenu
            // 
            modifyMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            modifyMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuRandomizeMax, mnuCloneCurrent, mnuShinyAssortment });
            modifyMenu.Name = "modifyMenu";
            modifyMenu.Size = new System.Drawing.Size(204, 70);
            // 
            // mnuRandomizeMax
            // 
            mnuRandomizeMax.Name = "mnuRandomizeMax";
            mnuRandomizeMax.Size = new System.Drawing.Size(203, 22);
            mnuRandomizeMax.Text = "Randomize Max Level";
            mnuRandomizeMax.Click += RandomizeAll;
            // 
            // mnuCloneCurrent
            // 
            mnuCloneCurrent.Name = "mnuCloneCurrent";
            mnuCloneCurrent.Size = new System.Drawing.Size(203, 22);
            mnuCloneCurrent.Text = "Clone Current to All";
            mnuCloneCurrent.Click += CloneCurrent;
            // 
            // mnuShinyAssortment
            // 
            mnuShinyAssortment.Name = "mnuShinyAssortment";
            mnuShinyAssortment.Size = new System.Drawing.Size(203, 22);
            mnuShinyAssortment.Text = "Shiny Assortment";
            mnuShinyAssortment.Click += ShinyAssortment;
            // 
            // B_ModifyAll
            // 
            B_ModifyAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_ModifyAll.Location = new System.Drawing.Point(208, 443);
            B_ModifyAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ModifyAll.Name = "B_ModifyAll";
            B_ModifyAll.Size = new System.Drawing.Size(128, 27);
            B_ModifyAll.TabIndex = 25;
            B_ModifyAll.Text = "Modify All";
            B_ModifyAll.UseVisualStyleBackColor = true;
            B_ModifyAll.Click += B_Modify_Click;
            // 
            // B_Reset
            // 
            B_Reset.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_Reset.Location = new System.Drawing.Point(344, 443);
            B_Reset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Reset.Name = "B_Reset";
            B_Reset.Size = new System.Drawing.Size(128, 27);
            B_Reset.TabIndex = 26;
            B_Reset.Text = "Reset";
            B_Reset.UseVisualStyleBackColor = true;
            B_Reset.Click += B_Reset_Click;
            // 
            // donutEditor
            // 
            donutEditor.AllowDrop = true;
            donutEditor.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            donutEditor.AutoSize = true;
            donutEditor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            donutEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            donutEditor.Location = new System.Drawing.Point(207, 15);
            donutEditor.Name = "donutEditor";
            donutEditor.Size = new System.Drawing.Size(647, 240);
            donutEditor.TabIndex = 27;
            // 
            // B_Import
            // 
            B_Import.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_Import.Location = new System.Drawing.Point(208, 398);
            B_Import.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Import.Name = "B_Import";
            B_Import.Size = new System.Drawing.Size(128, 27);
            B_Import.TabIndex = 28;
            B_Import.Text = "Import";
            B_Import.UseVisualStyleBackColor = true;
            B_Import.Click += B_ImportClick;
            // 
            // B_Export
            // 
            B_Export.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_Export.Location = new System.Drawing.Point(344, 398);
            B_Export.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Export.Name = "B_Export";
            B_Export.Size = new System.Drawing.Size(128, 27);
            B_Export.TabIndex = 29;
            B_Export.Text = "Export";
            B_Export.UseVisualStyleBackColor = true;
            B_Export.Click += B_Export_Click;
            // 
            // DonutFlavorProfile
            // 
            DonutFlavorProfile.BackColor = System.Drawing.Color.Transparent;
            DonutFlavorProfile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            DonutFlavorProfile.Location = new System.Drawing.Point(502, 243);
            DonutFlavorProfile.Name = "DonutFlavorProfile";
            DonutFlavorProfile.Size = new System.Drawing.Size(277, 237);
            DonutFlavorProfile.TabIndex = 30;
            // 
            // SAV_Donut9a
            // 
            AllowDrop = true;
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(894, 482);
            Controls.Add(B_Export);
            Controls.Add(B_Import);
            Controls.Add(donutEditor);
            Controls.Add(B_Reset);
            Controls.Add(B_ModifyAll);
            Controls.Add(B_Save);
            Controls.Add(LB_Donut);
            Controls.Add(B_Cancel);
            Controls.Add(DonutFlavorProfile);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_Donut9a";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Donut Editor";
            modifyMenu.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.ListBox LB_Donut;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.ContextMenuStrip modifyMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuRandomizeMax;
        private System.Windows.Forms.ToolStripMenuItem mnuCloneCurrent;
        private System.Windows.Forms.ToolStripMenuItem mnuShinyAssortment;
        private System.Windows.Forms.Button B_ModifyAll;
        private System.Windows.Forms.Button B_Reset;
        private DonutEditor9a donutEditor;
        private System.Windows.Forms.Button B_Import;
        private System.Windows.Forms.Button B_Export;
        private DonutFlavorProfile9a DonutFlavorProfile;
    }
}
