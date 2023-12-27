namespace PKHeX.WinForms
{
    partial class SuperTrainingEditor
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
            B_None = new System.Windows.Forms.Button();
            B_All = new System.Windows.Forms.Button();
            PAN_Training = new System.Windows.Forms.Panel();
            SPLIT_Training = new System.Windows.Forms.SplitContainer();
            TLP_SuperTrain = new System.Windows.Forms.TableLayoutPanel();
            CHK_SecretComplete = new System.Windows.Forms.CheckBox();
            CHK_SecretUnlocked = new System.Windows.Forms.CheckBox();
            CB_Bag = new System.Windows.Forms.ComboBox();
            L_Bag = new System.Windows.Forms.Label();
            NUD_BagHits = new System.Windows.Forms.NumericUpDown();
            L_Hits = new System.Windows.Forms.Label();
            TLP_DistSuperTrain = new System.Windows.Forms.TableLayoutPanel();
            PAN_Training.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SPLIT_Training).BeginInit();
            SPLIT_Training.Panel1.SuspendLayout();
            SPLIT_Training.Panel2.SuspendLayout();
            SPLIT_Training.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_BagHits).BeginInit();
            SuspendLayout();
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(371, 287);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(105, 27);
            B_Save.TabIndex = 1;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(259, 287);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(105, 27);
            B_Cancel.TabIndex = 2;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_None
            // 
            B_None.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_None.Location = new System.Drawing.Point(126, 287);
            B_None.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_None.Name = "B_None";
            B_None.Size = new System.Drawing.Size(105, 27);
            B_None.TabIndex = 5;
            B_None.Text = "Remove All";
            B_None.UseVisualStyleBackColor = true;
            B_None.Click += B_None_Click;
            // 
            // B_All
            // 
            B_All.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_All.Location = new System.Drawing.Point(14, 287);
            B_All.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_All.Name = "B_All";
            B_All.Size = new System.Drawing.Size(105, 27);
            B_All.TabIndex = 4;
            B_All.Text = "Give All";
            B_All.UseVisualStyleBackColor = true;
            B_All.Click += B_All_Click;
            // 
            // PAN_Training
            // 
            PAN_Training.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            PAN_Training.Controls.Add(SPLIT_Training);
            PAN_Training.Location = new System.Drawing.Point(14, 14);
            PAN_Training.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PAN_Training.Name = "PAN_Training";
            PAN_Training.Size = new System.Drawing.Size(462, 267);
            PAN_Training.TabIndex = 6;
            // 
            // SPLIT_Training
            // 
            SPLIT_Training.BackColor = System.Drawing.SystemColors.Window;
            SPLIT_Training.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            SPLIT_Training.Dock = System.Windows.Forms.DockStyle.Fill;
            SPLIT_Training.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            SPLIT_Training.IsSplitterFixed = true;
            SPLIT_Training.Location = new System.Drawing.Point(0, 0);
            SPLIT_Training.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SPLIT_Training.Name = "SPLIT_Training";
            // 
            // SPLIT_Training.Panel1
            // 
            SPLIT_Training.Panel1.Controls.Add(TLP_SuperTrain);
            SPLIT_Training.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            SPLIT_Training.Panel1MinSize = 1;
            // 
            // SPLIT_Training.Panel2
            // 
            SPLIT_Training.Panel2.Controls.Add(CHK_SecretComplete);
            SPLIT_Training.Panel2.Controls.Add(CHK_SecretUnlocked);
            SPLIT_Training.Panel2.Controls.Add(CB_Bag);
            SPLIT_Training.Panel2.Controls.Add(L_Bag);
            SPLIT_Training.Panel2.Controls.Add(NUD_BagHits);
            SPLIT_Training.Panel2.Controls.Add(L_Hits);
            SPLIT_Training.Panel2.Controls.Add(TLP_DistSuperTrain);
            SPLIT_Training.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            SPLIT_Training.Panel2MinSize = 1;
            SPLIT_Training.Size = new System.Drawing.Size(462, 267);
            SPLIT_Training.SplitterDistance = 175;
            SPLIT_Training.SplitterWidth = 5;
            SPLIT_Training.TabIndex = 4;
            // 
            // TLP_SuperTrain
            // 
            TLP_SuperTrain.AutoScroll = true;
            TLP_SuperTrain.ColumnCount = 2;
            TLP_SuperTrain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_SuperTrain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_SuperTrain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            TLP_SuperTrain.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_SuperTrain.Location = new System.Drawing.Point(0, 0);
            TLP_SuperTrain.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TLP_SuperTrain.Name = "TLP_SuperTrain";
            TLP_SuperTrain.RowCount = 1;
            TLP_SuperTrain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_SuperTrain.Size = new System.Drawing.Size(173, 265);
            TLP_SuperTrain.TabIndex = 4;
            // 
            // CHK_SecretComplete
            // 
            CHK_SecretComplete.AutoSize = true;
            CHK_SecretComplete.Location = new System.Drawing.Point(4, 20);
            CHK_SecretComplete.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_SecretComplete.Name = "CHK_SecretComplete";
            CHK_SecretComplete.Size = new System.Drawing.Size(158, 19);
            CHK_SecretComplete.TabIndex = 37;
            CHK_SecretComplete.Text = "Secret Training Complete";
            CHK_SecretComplete.UseVisualStyleBackColor = true;
            // 
            // CHK_SecretUnlocked
            // 
            CHK_SecretUnlocked.AutoSize = true;
            CHK_SecretUnlocked.Location = new System.Drawing.Point(4, 2);
            CHK_SecretUnlocked.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_SecretUnlocked.Name = "CHK_SecretUnlocked";
            CHK_SecretUnlocked.Size = new System.Drawing.Size(156, 19);
            CHK_SecretUnlocked.TabIndex = 36;
            CHK_SecretUnlocked.Text = "Secret Training Unlocked";
            CHK_SecretUnlocked.UseVisualStyleBackColor = true;
            CHK_SecretUnlocked.CheckedChanged += CHK_Secret_CheckedChanged;
            // 
            // CB_Bag
            // 
            CB_Bag.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            CB_Bag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Bag.FormattingEnabled = true;
            CB_Bag.Location = new System.Drawing.Point(83, 62);
            CB_Bag.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Bag.Name = "CB_Bag";
            CB_Bag.Size = new System.Drawing.Size(160, 23);
            CB_Bag.TabIndex = 35;
            // 
            // L_Bag
            // 
            L_Bag.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            L_Bag.Location = new System.Drawing.Point(65, 44);
            L_Bag.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Bag.Name = "L_Bag";
            L_Bag.Size = new System.Drawing.Size(117, 15);
            L_Bag.TabIndex = 34;
            L_Bag.Text = "Last Used Bag:";
            L_Bag.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // NUD_BagHits
            // 
            NUD_BagHits.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            NUD_BagHits.Location = new System.Drawing.Point(185, 91);
            NUD_BagHits.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_BagHits.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_BagHits.Name = "NUD_BagHits";
            NUD_BagHits.Size = new System.Drawing.Size(58, 23);
            NUD_BagHits.TabIndex = 33;
            NUD_BagHits.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            NUD_BagHits.Value = new decimal(new int[] { 255, 0, 0, 0 });
            // 
            // L_Hits
            // 
            L_Hits.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            L_Hits.Location = new System.Drawing.Point(65, 93);
            L_Hits.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Hits.Name = "L_Hits";
            L_Hits.Size = new System.Drawing.Size(117, 15);
            L_Hits.TabIndex = 32;
            L_Hits.Text = "Hits Remaining:";
            L_Hits.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // TLP_DistSuperTrain
            // 
            TLP_DistSuperTrain.AutoScroll = true;
            TLP_DistSuperTrain.AutoSize = true;
            TLP_DistSuperTrain.ColumnCount = 1;
            TLP_DistSuperTrain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_DistSuperTrain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_DistSuperTrain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            TLP_DistSuperTrain.Dock = System.Windows.Forms.DockStyle.Bottom;
            TLP_DistSuperTrain.Location = new System.Drawing.Point(0, 265);
            TLP_DistSuperTrain.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TLP_DistSuperTrain.Name = "TLP_DistSuperTrain";
            TLP_DistSuperTrain.RowCount = 1;
            TLP_DistSuperTrain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_DistSuperTrain.Size = new System.Drawing.Size(280, 0);
            TLP_DistSuperTrain.TabIndex = 3;
            // 
            // SuperTrainingEditor
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(495, 324);
            Controls.Add(PAN_Training);
            Controls.Add(B_None);
            Controls.Add(B_All);
            Controls.Add(B_Cancel);
            Controls.Add(B_Save);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimumSize = new System.Drawing.Size(511, 363);
            Name = "SuperTrainingEditor";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Ribbon Editor";
            PAN_Training.ResumeLayout(false);
            SPLIT_Training.Panel1.ResumeLayout(false);
            SPLIT_Training.Panel2.ResumeLayout(false);
            SPLIT_Training.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)SPLIT_Training).EndInit();
            SPLIT_Training.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)NUD_BagHits).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_None;
        private System.Windows.Forms.Button B_All;
        private System.Windows.Forms.Panel PAN_Training;
        private System.Windows.Forms.TableLayoutPanel TLP_DistSuperTrain;
        private System.Windows.Forms.Label L_Hits;
        private System.Windows.Forms.NumericUpDown NUD_BagHits;
        private System.Windows.Forms.Label L_Bag;
        private System.Windows.Forms.ComboBox CB_Bag;
        private System.Windows.Forms.CheckBox CHK_SecretUnlocked;
        private System.Windows.Forms.TableLayoutPanel TLP_SuperTrain;
        private System.Windows.Forms.SplitContainer SPLIT_Training;
        private System.Windows.Forms.CheckBox CHK_SecretComplete;
    }
}
