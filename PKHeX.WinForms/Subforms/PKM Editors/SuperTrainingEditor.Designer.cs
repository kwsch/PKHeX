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
            this.B_Save = new System.Windows.Forms.Button();
            this.B_Cancel = new System.Windows.Forms.Button();
            this.B_None = new System.Windows.Forms.Button();
            this.B_All = new System.Windows.Forms.Button();
            this.PAN_Training = new System.Windows.Forms.Panel();
            this.SPLIT_Training = new System.Windows.Forms.SplitContainer();
            this.TLP_SuperTrain = new System.Windows.Forms.TableLayoutPanel();
            this.CHK_SecretComplete = new System.Windows.Forms.CheckBox();
            this.CHK_SecretUnlocked = new System.Windows.Forms.CheckBox();
            this.CB_Bag = new System.Windows.Forms.ComboBox();
            this.L_Bag = new System.Windows.Forms.Label();
            this.NUD_BagHits = new System.Windows.Forms.NumericUpDown();
            this.L_Hits = new System.Windows.Forms.Label();
            this.TLP_DistSuperTrain = new System.Windows.Forms.TableLayoutPanel();
            this.PAN_Training.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SPLIT_Training)).BeginInit();
            this.SPLIT_Training.Panel1.SuspendLayout();
            this.SPLIT_Training.Panel2.SuspendLayout();
            this.SPLIT_Training.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_BagHits)).BeginInit();
            this.SuspendLayout();
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(318, 249);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(90, 23);
            this.B_Save.TabIndex = 1;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(222, 249);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(90, 23);
            this.B_Cancel.TabIndex = 2;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // B_None
            // 
            this.B_None.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.B_None.Location = new System.Drawing.Point(108, 249);
            this.B_None.Name = "B_None";
            this.B_None.Size = new System.Drawing.Size(90, 23);
            this.B_None.TabIndex = 5;
            this.B_None.Text = "Remove All";
            this.B_None.UseVisualStyleBackColor = true;
            this.B_None.Click += new System.EventHandler(this.B_None_Click);
            // 
            // B_All
            // 
            this.B_All.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.B_All.Location = new System.Drawing.Point(12, 249);
            this.B_All.Name = "B_All";
            this.B_All.Size = new System.Drawing.Size(90, 23);
            this.B_All.TabIndex = 4;
            this.B_All.Text = "Give All";
            this.B_All.UseVisualStyleBackColor = true;
            this.B_All.Click += new System.EventHandler(this.B_All_Click);
            // 
            // PAN_Training
            // 
            this.PAN_Training.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PAN_Training.Controls.Add(this.SPLIT_Training);
            this.PAN_Training.Location = new System.Drawing.Point(12, 12);
            this.PAN_Training.Name = "PAN_Training";
            this.PAN_Training.Size = new System.Drawing.Size(396, 231);
            this.PAN_Training.TabIndex = 6;
            // 
            // SPLIT_Training
            // 
            this.SPLIT_Training.BackColor = System.Drawing.SystemColors.Window;
            this.SPLIT_Training.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SPLIT_Training.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SPLIT_Training.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.SPLIT_Training.IsSplitterFixed = true;
            this.SPLIT_Training.Location = new System.Drawing.Point(0, 0);
            this.SPLIT_Training.Name = "SPLIT_Training";
            // 
            // SPLIT_Training.Panel1
            // 
            this.SPLIT_Training.Panel1.Controls.Add(this.TLP_SuperTrain);
            this.SPLIT_Training.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SPLIT_Training.Panel1MinSize = 1;
            // 
            // SPLIT_Training.Panel2
            // 
            this.SPLIT_Training.Panel2.Controls.Add(this.CHK_SecretComplete);
            this.SPLIT_Training.Panel2.Controls.Add(this.CHK_SecretUnlocked);
            this.SPLIT_Training.Panel2.Controls.Add(this.CB_Bag);
            this.SPLIT_Training.Panel2.Controls.Add(this.L_Bag);
            this.SPLIT_Training.Panel2.Controls.Add(this.NUD_BagHits);
            this.SPLIT_Training.Panel2.Controls.Add(this.L_Hits);
            this.SPLIT_Training.Panel2.Controls.Add(this.TLP_DistSuperTrain);
            this.SPLIT_Training.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SPLIT_Training.Panel2MinSize = 1;
            this.SPLIT_Training.Size = new System.Drawing.Size(396, 231);
            this.SPLIT_Training.SplitterDistance = 175;
            this.SPLIT_Training.TabIndex = 4;
            // 
            // TLP_SuperTrain
            // 
            this.TLP_SuperTrain.AutoScroll = true;
            this.TLP_SuperTrain.ColumnCount = 2;
            this.TLP_SuperTrain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TLP_SuperTrain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TLP_SuperTrain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TLP_SuperTrain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLP_SuperTrain.Location = new System.Drawing.Point(0, 0);
            this.TLP_SuperTrain.Name = "TLP_SuperTrain";
            this.TLP_SuperTrain.RowCount = 1;
            this.TLP_SuperTrain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_SuperTrain.Size = new System.Drawing.Size(173, 229);
            this.TLP_SuperTrain.TabIndex = 4;
            // 
            // CHK_SecretComplete
            // 
            this.CHK_SecretComplete.AutoSize = true;
            this.CHK_SecretComplete.Location = new System.Drawing.Point(3, 17);
            this.CHK_SecretComplete.Name = "CHK_SecretComplete";
            this.CHK_SecretComplete.Size = new System.Drawing.Size(145, 17);
            this.CHK_SecretComplete.TabIndex = 37;
            this.CHK_SecretComplete.Text = "Secret Training Complete";
            this.CHK_SecretComplete.UseVisualStyleBackColor = true;
            // 
            // CHK_SecretUnlocked
            // 
            this.CHK_SecretUnlocked.AutoSize = true;
            this.CHK_SecretUnlocked.Location = new System.Drawing.Point(3, 2);
            this.CHK_SecretUnlocked.Name = "CHK_SecretUnlocked";
            this.CHK_SecretUnlocked.Size = new System.Drawing.Size(147, 17);
            this.CHK_SecretUnlocked.TabIndex = 36;
            this.CHK_SecretUnlocked.Text = "Secret Training Unlocked";
            this.CHK_SecretUnlocked.UseVisualStyleBackColor = true;
            this.CHK_SecretUnlocked.CheckedChanged += new System.EventHandler(this.CHK_Secret_CheckedChanged);
            // 
            // CB_Bag
            // 
            this.CB_Bag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_Bag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Bag.FormattingEnabled = true;
            this.CB_Bag.Location = new System.Drawing.Point(46, 54);
            this.CB_Bag.Name = "CB_Bag";
            this.CB_Bag.Size = new System.Drawing.Size(138, 21);
            this.CB_Bag.TabIndex = 35;
            // 
            // L_Bag
            // 
            this.L_Bag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_Bag.Location = new System.Drawing.Point(31, 38);
            this.L_Bag.Name = "L_Bag";
            this.L_Bag.Size = new System.Drawing.Size(100, 13);
            this.L_Bag.TabIndex = 34;
            this.L_Bag.Text = "Last Used Bag:";
            this.L_Bag.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // NUD_BagHits
            // 
            this.NUD_BagHits.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NUD_BagHits.Location = new System.Drawing.Point(134, 79);
            this.NUD_BagHits.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NUD_BagHits.Name = "NUD_BagHits";
            this.NUD_BagHits.Size = new System.Drawing.Size(50, 20);
            this.NUD_BagHits.TabIndex = 33;
            this.NUD_BagHits.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NUD_BagHits.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // L_Hits
            // 
            this.L_Hits.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_Hits.Location = new System.Drawing.Point(31, 81);
            this.L_Hits.Name = "L_Hits";
            this.L_Hits.Size = new System.Drawing.Size(100, 13);
            this.L_Hits.TabIndex = 32;
            this.L_Hits.Text = "Hits Remaining:";
            this.L_Hits.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // TLP_DistSuperTrain
            // 
            this.TLP_DistSuperTrain.AutoScroll = true;
            this.TLP_DistSuperTrain.AutoSize = true;
            this.TLP_DistSuperTrain.ColumnCount = 1;
            this.TLP_DistSuperTrain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TLP_DistSuperTrain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TLP_DistSuperTrain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TLP_DistSuperTrain.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.TLP_DistSuperTrain.Location = new System.Drawing.Point(0, 229);
            this.TLP_DistSuperTrain.Name = "TLP_DistSuperTrain";
            this.TLP_DistSuperTrain.RowCount = 1;
            this.TLP_DistSuperTrain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_DistSuperTrain.Size = new System.Drawing.Size(215, 0);
            this.TLP_DistSuperTrain.TabIndex = 3;
            // 
            // SuperTrainingEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 281);
            this.Controls.Add(this.PAN_Training);
            this.Controls.Add(this.B_None);
            this.Controls.Add(this.B_All);
            this.Controls.Add(this.B_Cancel);
            this.Controls.Add(this.B_Save);
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(440, 320);
            this.Name = "SuperTrainingEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Ribbon Editor";
            this.PAN_Training.ResumeLayout(false);
            this.SPLIT_Training.Panel1.ResumeLayout(false);
            this.SPLIT_Training.Panel2.ResumeLayout(false);
            this.SPLIT_Training.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SPLIT_Training)).EndInit();
            this.SPLIT_Training.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NUD_BagHits)).EndInit();
            this.ResumeLayout(false);

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