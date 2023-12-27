namespace PKHeX.WinForms
{
    partial class SAV_Capture7GG
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
            B_Cancel = new System.Windows.Forms.Button();
            LB_Species = new System.Windows.Forms.ListBox();
            L_goto = new System.Windows.Forms.Label();
            CB_Species = new System.Windows.Forms.ComboBox();
            B_Save = new System.Windows.Forms.Button();
            B_Modify = new System.Windows.Forms.Button();
            mnuFormNone = new System.Windows.Forms.ToolStripMenuItem();
            mnuForm1 = new System.Windows.Forms.ToolStripMenuItem();
            mnuFormAll = new System.Windows.Forms.ToolStripMenuItem();
            NUD_SpeciesCaptured = new System.Windows.Forms.NumericUpDown();
            NUD_SpeciesTransferred = new System.Windows.Forms.NumericUpDown();
            NUD_TotalTransferred = new System.Windows.Forms.NumericUpDown();
            NUD_TotalCaptured = new System.Windows.Forms.NumericUpDown();
            L_SpeciesCaptured = new System.Windows.Forms.Label();
            L_SpeciesTransferred = new System.Windows.Forms.Label();
            L_TotalTransferred = new System.Windows.Forms.Label();
            L_TotalCaptured = new System.Windows.Forms.Label();
            GB_Species = new System.Windows.Forms.GroupBox();
            B_SumTotal = new System.Windows.Forms.Button();
            GB_Total = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)NUD_SpeciesCaptured).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_SpeciesTransferred).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_TotalTransferred).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_TotalCaptured).BeginInit();
            GB_Species.SuspendLayout();
            GB_Total.SuspendLayout();
            SuspendLayout();
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(270, 342);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(93, 27);
            B_Cancel.TabIndex = 0;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // LB_Species
            // 
            LB_Species.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            LB_Species.FormattingEnabled = true;
            LB_Species.ItemHeight = 15;
            LB_Species.Location = new System.Drawing.Point(14, 46);
            LB_Species.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LB_Species.Name = "LB_Species";
            LB_Species.Size = new System.Drawing.Size(186, 319);
            LB_Species.TabIndex = 2;
            LB_Species.SelectedIndexChanged += ChangeLBSpecies;
            // 
            // L_goto
            // 
            L_goto.AutoSize = true;
            L_goto.Location = new System.Drawing.Point(14, 18);
            L_goto.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_goto.Name = "L_goto";
            L_goto.Size = new System.Drawing.Size(35, 15);
            L_goto.TabIndex = 20;
            L_goto.Text = "goto:";
            // 
            // CB_Species
            // 
            CB_Species.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            CB_Species.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Species.DropDownWidth = 95;
            CB_Species.FormattingEnabled = true;
            CB_Species.Items.AddRange(new object[] { "0" });
            CB_Species.Location = new System.Drawing.Point(58, 15);
            CB_Species.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Species.Name = "CB_Species";
            CB_Species.Size = new System.Drawing.Size(142, 23);
            CB_Species.TabIndex = 21;
            CB_Species.SelectedIndexChanged += ChangeCBSpecies;
            CB_Species.SelectedValueChanged += ChangeCBSpecies;
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(370, 342);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(93, 27);
            B_Save.TabIndex = 24;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Modify
            // 
            B_Modify.Location = new System.Drawing.Point(191, 20);
            B_Modify.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Modify.Name = "B_Modify";
            B_Modify.Size = new System.Drawing.Size(55, 47);
            B_Modify.TabIndex = 25;
            B_Modify.Text = "Set All";
            B_Modify.UseVisualStyleBackColor = true;
            B_Modify.Click += B_Modify_Click;
            // 
            // mnuFormNone
            // 
            mnuFormNone.Name = "mnuFormNone";
            mnuFormNone.Size = new System.Drawing.Size(32, 19);
            // 
            // mnuForm1
            // 
            mnuForm1.Name = "mnuForm1";
            mnuForm1.Size = new System.Drawing.Size(32, 19);
            // 
            // mnuFormAll
            // 
            mnuFormAll.Name = "mnuFormAll";
            mnuFormAll.Size = new System.Drawing.Size(32, 19);
            // 
            // NUD_SpeciesCaptured
            // 
            NUD_SpeciesCaptured.Location = new System.Drawing.Point(100, 20);
            NUD_SpeciesCaptured.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_SpeciesCaptured.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            NUD_SpeciesCaptured.Name = "NUD_SpeciesCaptured";
            NUD_SpeciesCaptured.Size = new System.Drawing.Size(52, 23);
            NUD_SpeciesCaptured.TabIndex = 53;
            NUD_SpeciesCaptured.Value = new decimal(new int[] { 9999, 0, 0, 0 });
            // 
            // NUD_SpeciesTransferred
            // 
            NUD_SpeciesTransferred.Location = new System.Drawing.Point(100, 46);
            NUD_SpeciesTransferred.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_SpeciesTransferred.Maximum = new decimal(new int[] { 999999999, 0, 0, 0 });
            NUD_SpeciesTransferred.Name = "NUD_SpeciesTransferred";
            NUD_SpeciesTransferred.Size = new System.Drawing.Size(88, 23);
            NUD_SpeciesTransferred.TabIndex = 54;
            NUD_SpeciesTransferred.Value = new decimal(new int[] { 123456789, 0, 0, 0 });
            // 
            // NUD_TotalTransferred
            // 
            NUD_TotalTransferred.Location = new System.Drawing.Point(98, 48);
            NUD_TotalTransferred.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_TotalTransferred.Maximum = new decimal(new int[] { 999999999, 0, 0, 0 });
            NUD_TotalTransferred.Name = "NUD_TotalTransferred";
            NUD_TotalTransferred.Size = new System.Drawing.Size(88, 23);
            NUD_TotalTransferred.TabIndex = 56;
            NUD_TotalTransferred.Value = new decimal(new int[] { 123456789, 0, 0, 0 });
            // 
            // NUD_TotalCaptured
            // 
            NUD_TotalCaptured.Location = new System.Drawing.Point(98, 22);
            NUD_TotalCaptured.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_TotalCaptured.Maximum = new decimal(new int[] { 999999999, 0, 0, 0 });
            NUD_TotalCaptured.Name = "NUD_TotalCaptured";
            NUD_TotalCaptured.Size = new System.Drawing.Size(88, 23);
            NUD_TotalCaptured.TabIndex = 55;
            NUD_TotalCaptured.Value = new decimal(new int[] { 123456789, 0, 0, 0 });
            // 
            // L_SpeciesCaptured
            // 
            L_SpeciesCaptured.Location = new System.Drawing.Point(5, 20);
            L_SpeciesCaptured.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_SpeciesCaptured.Name = "L_SpeciesCaptured";
            L_SpeciesCaptured.Size = new System.Drawing.Size(93, 23);
            L_SpeciesCaptured.TabIndex = 61;
            L_SpeciesCaptured.Text = "Captured";
            L_SpeciesCaptured.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            L_SpeciesCaptured.Click += ClickSpeciesLabel;
            // 
            // L_SpeciesTransferred
            // 
            L_SpeciesTransferred.Location = new System.Drawing.Point(5, 44);
            L_SpeciesTransferred.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_SpeciesTransferred.Name = "L_SpeciesTransferred";
            L_SpeciesTransferred.Size = new System.Drawing.Size(93, 23);
            L_SpeciesTransferred.TabIndex = 62;
            L_SpeciesTransferred.Text = "Transferred";
            L_SpeciesTransferred.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            L_SpeciesTransferred.Click += ClickSpeciesLabel;
            // 
            // L_TotalTransferred
            // 
            L_TotalTransferred.Location = new System.Drawing.Point(2, 46);
            L_TotalTransferred.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_TotalTransferred.Name = "L_TotalTransferred";
            L_TotalTransferred.Size = new System.Drawing.Size(93, 23);
            L_TotalTransferred.TabIndex = 64;
            L_TotalTransferred.Text = "Transferred";
            L_TotalTransferred.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            L_TotalTransferred.Click += ClickTotalLabel;
            // 
            // L_TotalCaptured
            // 
            L_TotalCaptured.Location = new System.Drawing.Point(2, 22);
            L_TotalCaptured.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_TotalCaptured.Name = "L_TotalCaptured";
            L_TotalCaptured.Size = new System.Drawing.Size(93, 23);
            L_TotalCaptured.TabIndex = 63;
            L_TotalCaptured.Text = "Captured";
            L_TotalCaptured.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            L_TotalCaptured.Click += ClickTotalLabel;
            // 
            // GB_Species
            // 
            GB_Species.Controls.Add(NUD_SpeciesCaptured);
            GB_Species.Controls.Add(NUD_SpeciesTransferred);
            GB_Species.Controls.Add(L_SpeciesTransferred);
            GB_Species.Controls.Add(B_Modify);
            GB_Species.Controls.Add(L_SpeciesCaptured);
            GB_Species.Location = new System.Drawing.Point(208, 46);
            GB_Species.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Species.Name = "GB_Species";
            GB_Species.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Species.Size = new System.Drawing.Size(252, 82);
            GB_Species.TabIndex = 65;
            GB_Species.TabStop = false;
            GB_Species.Text = "Species Info";
            // 
            // B_SumTotal
            // 
            B_SumTotal.Location = new System.Drawing.Point(191, 22);
            B_SumTotal.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_SumTotal.Name = "B_SumTotal";
            B_SumTotal.Size = new System.Drawing.Size(55, 50);
            B_SumTotal.TabIndex = 66;
            B_SumTotal.Text = "Σ";
            B_SumTotal.UseVisualStyleBackColor = true;
            B_SumTotal.Click += B_SumTotal_Click;
            // 
            // GB_Total
            // 
            GB_Total.Controls.Add(B_SumTotal);
            GB_Total.Controls.Add(NUD_TotalTransferred);
            GB_Total.Controls.Add(L_TotalCaptured);
            GB_Total.Controls.Add(L_TotalTransferred);
            GB_Total.Controls.Add(NUD_TotalCaptured);
            GB_Total.Location = new System.Drawing.Point(208, 135);
            GB_Total.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Total.Name = "GB_Total";
            GB_Total.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Total.Size = new System.Drawing.Size(252, 82);
            GB_Total.TabIndex = 68;
            GB_Total.TabStop = false;
            GB_Total.Text = "Totals";
            // 
            // SAV_Capture7GG
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(471, 376);
            Controls.Add(GB_Total);
            Controls.Add(GB_Species);
            Controls.Add(B_Save);
            Controls.Add(CB_Species);
            Controls.Add(L_goto);
            Controls.Add(LB_Species);
            Controls.Add(B_Cancel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_Capture7GG";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Capture Record Editor";
            ((System.ComponentModel.ISupportInitialize)NUD_SpeciesCaptured).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_SpeciesTransferred).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_TotalTransferred).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_TotalCaptured).EndInit();
            GB_Species.ResumeLayout(false);
            GB_Total.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.ListBox LB_Species;
        private System.Windows.Forms.Label L_goto;
        private System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Modify;
        private System.Windows.Forms.ToolStripMenuItem mnuFormNone;
        private System.Windows.Forms.ToolStripMenuItem mnuForm1;
        private System.Windows.Forms.ToolStripMenuItem mnuFormAll;
        private System.Windows.Forms.NumericUpDown NUD_SpeciesCaptured;
        private System.Windows.Forms.NumericUpDown NUD_SpeciesTransferred;
        private System.Windows.Forms.NumericUpDown NUD_TotalTransferred;
        private System.Windows.Forms.NumericUpDown NUD_TotalCaptured;
        private System.Windows.Forms.Label L_SpeciesCaptured;
        private System.Windows.Forms.Label L_SpeciesTransferred;
        private System.Windows.Forms.Label L_TotalTransferred;
        private System.Windows.Forms.Label L_TotalCaptured;
        private System.Windows.Forms.GroupBox GB_Species;
        private System.Windows.Forms.Button B_SumTotal;
        private System.Windows.Forms.GroupBox GB_Total;
    }
}
