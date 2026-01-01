namespace PKHeX.WinForms
{
    partial class SAV_Wondercard
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
            B_Save = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            B_Output = new System.Windows.Forms.Button();
            B_Import = new System.Windows.Forms.Button();
            LB_Received = new System.Windows.Forms.ListBox();
            mnuDel = new System.Windows.Forms.ContextMenuStrip(components);
            flagDel = new System.Windows.Forms.ToolStripMenuItem();
            L_Received = new System.Windows.Forms.Label();
            RTB = new System.Windows.Forms.RichTextBox();
            L_Details = new System.Windows.Forms.Label();
            L_QR = new System.Windows.Forms.Label();
            PB_Preview = new System.Windows.Forms.PictureBox();
            mnuVSD = new System.Windows.Forms.ContextMenuStrip(components);
            mnuView = new System.Windows.Forms.ToolStripMenuItem();
            mnuSet = new System.Windows.Forms.ToolStripMenuItem();
            mnuDelete = new System.Windows.Forms.ToolStripMenuItem();
            FLP_Gifts = new System.Windows.Forms.FlowLayoutPanel();
            B_UnusedAll = new System.Windows.Forms.Button();
            B_UsedAll = new System.Windows.Forms.Button();
            mnuDel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PB_Preview).BeginInit();
            mnuVSD.SuspendLayout();
            SuspendLayout();
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(622, 549);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(88, 27);
            B_Save.TabIndex = 9;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(539, 549);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(83, 27);
            B_Cancel.TabIndex = 8;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_Output
            // 
            B_Output.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            B_Output.Location = new System.Drawing.Point(622, 14);
            B_Output.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Output.Name = "B_Output";
            B_Output.Size = new System.Drawing.Size(88, 27);
            B_Output.TabIndex = 2;
            B_Output.Text = "Export";
            B_Output.UseVisualStyleBackColor = true;
            B_Output.Click += B_Output_Click;
            // 
            // B_Import
            // 
            B_Import.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            B_Import.Location = new System.Drawing.Point(622, 39);
            B_Import.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Import.Name = "B_Import";
            B_Import.Size = new System.Drawing.Size(88, 27);
            B_Import.TabIndex = 3;
            B_Import.Text = "Import";
            B_Import.UseVisualStyleBackColor = true;
            B_Import.Click += B_Import_Click;
            // 
            // LB_Received
            // 
            LB_Received.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            LB_Received.ContextMenuStrip = mnuDel;
            LB_Received.FormattingEnabled = true;
            LB_Received.ItemHeight = 15;
            LB_Received.Location = new System.Drawing.Point(14, 36);
            LB_Received.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LB_Received.Name = "LB_Received";
            LB_Received.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            LB_Received.Size = new System.Drawing.Size(90, 544);
            LB_Received.Sorted = true;
            LB_Received.TabIndex = 1;
            LB_Received.KeyDown += LB_Received_KeyDown;
            // 
            // mnuDel
            // 
            mnuDel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { flagDel });
            mnuDel.Name = "mnuVSD";
            mnuDel.Size = new System.Drawing.Size(108, 26);
            // 
            // flagDel
            // 
            flagDel.Name = "flagDel";
            flagDel.Size = new System.Drawing.Size(107, 22);
            flagDel.Text = "Delete";
            flagDel.Click += ClearReceivedFlag;
            // 
            // L_Received
            // 
            L_Received.AutoSize = true;
            L_Received.Location = new System.Drawing.Point(14, 15);
            L_Received.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Received.Name = "L_Received";
            L_Received.Size = new System.Drawing.Size(78, 15);
            L_Received.TabIndex = 0;
            L_Received.Text = "Received List:";
            // 
            // RTB
            // 
            RTB.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            RTB.Location = new System.Drawing.Point(130, 68);
            RTB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RTB.Name = "RTB";
            RTB.ReadOnly = true;
            RTB.Size = new System.Drawing.Size(579, 115);
            RTB.TabIndex = 4;
            RTB.Text = "";
            // 
            // L_Details
            // 
            L_Details.AutoSize = true;
            L_Details.Location = new System.Drawing.Point(126, 50);
            L_Details.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Details.Name = "L_Details";
            L_Details.Size = new System.Drawing.Size(45, 15);
            L_Details.TabIndex = 2;
            L_Details.Text = "Details:";
            // 
            // L_QR
            // 
            L_QR.AutoSize = true;
            L_QR.Location = new System.Drawing.Point(302, 24);
            L_QR.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_QR.Name = "L_QR";
            L_QR.Size = new System.Drawing.Size(26, 15);
            L_QR.TabIndex = 62;
            L_QR.Text = "QR!";
            L_QR.Click += ClickQR;
            // 
            // PB_Preview
            // 
            PB_Preview.BackColor = System.Drawing.Color.Transparent;
            PB_Preview.Location = new System.Drawing.Point(344, 2);
            PB_Preview.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PB_Preview.Name = "PB_Preview";
            PB_Preview.Size = new System.Drawing.Size(79, 65);
            PB_Preview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            PB_Preview.TabIndex = 63;
            PB_Preview.TabStop = false;
            // 
            // mnuVSD
            // 
            mnuVSD.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuView, mnuSet, mnuDelete });
            mnuVSD.Name = "mnuVSD";
            mnuVSD.Size = new System.Drawing.Size(108, 70);
            // 
            // mnuView
            // 
            mnuView.Image = Properties.Resources.other;
            mnuView.Name = "mnuView";
            mnuView.Size = new System.Drawing.Size(107, 22);
            mnuView.Text = "View";
            mnuView.Click += ClickView;
            // 
            // mnuSet
            // 
            mnuSet.Image = Properties.Resources.exit;
            mnuSet.Name = "mnuSet";
            mnuSet.Size = new System.Drawing.Size(107, 22);
            mnuSet.Text = "Set";
            mnuSet.Click += ClickSet;
            // 
            // mnuDelete
            // 
            mnuDelete.Image = Properties.Resources.nocheck;
            mnuDelete.Name = "mnuDelete";
            mnuDelete.Size = new System.Drawing.Size(107, 22);
            mnuDelete.Text = "Delete";
            mnuDelete.Click += ClickDelete;
            // 
            // FLP_Gifts
            // 
            FLP_Gifts.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            FLP_Gifts.AutoScroll = true;
            FLP_Gifts.Location = new System.Drawing.Point(130, 189);
            FLP_Gifts.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            FLP_Gifts.Name = "FLP_Gifts";
            FLP_Gifts.Size = new System.Drawing.Size(580, 353);
            FLP_Gifts.TabIndex = 5;
            // 
            // B_UnusedAll
            // 
            B_UnusedAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_UnusedAll.Location = new System.Drawing.Point(295, 549);
            B_UnusedAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_UnusedAll.Name = "B_UnusedAll";
            B_UnusedAll.Size = new System.Drawing.Size(83, 27);
            B_UnusedAll.TabIndex = 6;
            B_UnusedAll.Text = "All Unused";
            B_UnusedAll.UseVisualStyleBackColor = true;
            B_UnusedAll.Click += B_ModifyAll_Click;
            // 
            // B_UsedAll
            // 
            B_UsedAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_UsedAll.Location = new System.Drawing.Point(378, 549);
            B_UsedAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_UsedAll.Name = "B_UsedAll";
            B_UsedAll.Size = new System.Drawing.Size(88, 27);
            B_UsedAll.TabIndex = 7;
            B_UsedAll.Text = "All Used";
            B_UsedAll.UseVisualStyleBackColor = true;
            B_UsedAll.Click += B_ModifyAll_Click;
            // 
            // SAV_Wondercard
            // 
            AllowDrop = true;
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(740, 590);
            Controls.Add(B_UnusedAll);
            Controls.Add(B_UsedAll);
            Controls.Add(FLP_Gifts);
            Controls.Add(PB_Preview);
            Controls.Add(L_QR);
            Controls.Add(L_Details);
            Controls.Add(RTB);
            Controls.Add(L_Received);
            Controls.Add(LB_Received);
            Controls.Add(B_Import);
            Controls.Add(B_Output);
            Controls.Add(B_Cancel);
            Controls.Add(B_Save);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(814, 686);
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(756, 629);
            Name = "SAV_Wondercard";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Wonder Card I/O";
            mnuDel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PB_Preview).EndInit();
            mnuVSD.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Output;
        private System.Windows.Forms.Button B_Import;
        private System.Windows.Forms.ListBox LB_Received;
        private System.Windows.Forms.Label L_Received;
        private System.Windows.Forms.RichTextBox RTB;
        private System.Windows.Forms.Label L_Details;
        private System.Windows.Forms.Label L_QR;
        private System.Windows.Forms.PictureBox PB_Preview;
        private System.Windows.Forms.ContextMenuStrip mnuVSD;
        private System.Windows.Forms.ToolStripMenuItem mnuView;
        private System.Windows.Forms.ToolStripMenuItem mnuSet;
        private System.Windows.Forms.ToolStripMenuItem mnuDelete;
        private System.Windows.Forms.ContextMenuStrip mnuDel;
        private System.Windows.Forms.ToolStripMenuItem flagDel;
        private System.Windows.Forms.FlowLayoutPanel FLP_Gifts;
        private System.Windows.Forms.Button B_UnusedAll;
        private System.Windows.Forms.Button B_UsedAll;
    }
}
