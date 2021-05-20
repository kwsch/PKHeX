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
            this.B_Cancel = new System.Windows.Forms.Button();
            this.LB_Species = new System.Windows.Forms.ListBox();
            this.L_goto = new System.Windows.Forms.Label();
            this.CB_Species = new System.Windows.Forms.ComboBox();
            this.B_Save = new System.Windows.Forms.Button();
            this.B_Modify = new System.Windows.Forms.Button();
            this.mnuFormNone = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuForm1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFormAll = new System.Windows.Forms.ToolStripMenuItem();
            this.NUD_SpeciesCaptured = new System.Windows.Forms.NumericUpDown();
            this.NUD_SpeciesTransferred = new System.Windows.Forms.NumericUpDown();
            this.NUD_TotalTransferred = new System.Windows.Forms.NumericUpDown();
            this.NUD_TotalCaptured = new System.Windows.Forms.NumericUpDown();
            this.L_SpeciesCaptured = new System.Windows.Forms.Label();
            this.L_SpeciesTransferred = new System.Windows.Forms.Label();
            this.L_TotalTransferred = new System.Windows.Forms.Label();
            this.L_TotalCaptured = new System.Windows.Forms.Label();
            this.GB_Species = new System.Windows.Forms.GroupBox();
            this.B_SumTotal = new System.Windows.Forms.Button();
            this.GB_Total = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_SpeciesCaptured)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_SpeciesTransferred)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_TotalTransferred)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_TotalCaptured)).BeginInit();
            this.GB_Species.SuspendLayout();
            this.GB_Total.SuspendLayout();
            this.SuspendLayout();
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(231, 296);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(80, 23);
            this.B_Cancel.TabIndex = 0;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // LB_Species
            // 
            this.LB_Species.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.LB_Species.FormattingEnabled = true;
            this.LB_Species.Location = new System.Drawing.Point(12, 40);
            this.LB_Species.Name = "LB_Species";
            this.LB_Species.Size = new System.Drawing.Size(160, 277);
            this.LB_Species.TabIndex = 2;
            this.LB_Species.SelectedIndexChanged += new System.EventHandler(this.ChangeLBSpecies);
            // 
            // L_goto
            // 
            this.L_goto.AutoSize = true;
            this.L_goto.Location = new System.Drawing.Point(12, 16);
            this.L_goto.Name = "L_goto";
            this.L_goto.Size = new System.Drawing.Size(31, 13);
            this.L_goto.TabIndex = 20;
            this.L_goto.Text = "goto:";
            // 
            // CB_Species
            // 
            this.CB_Species.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.CB_Species.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Species.DropDownWidth = 95;
            this.CB_Species.FormattingEnabled = true;
            this.CB_Species.Items.AddRange(new object[] {
            "0"});
            this.CB_Species.Location = new System.Drawing.Point(50, 13);
            this.CB_Species.Name = "CB_Species";
            this.CB_Species.Size = new System.Drawing.Size(122, 21);
            this.CB_Species.TabIndex = 21;
            this.CB_Species.SelectedIndexChanged += new System.EventHandler(this.ChangeCBSpecies);
            this.CB_Species.SelectedValueChanged += new System.EventHandler(this.ChangeCBSpecies);
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(317, 296);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(80, 23);
            this.B_Save.TabIndex = 24;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // B_Modify
            // 
            this.B_Modify.Location = new System.Drawing.Point(164, 17);
            this.B_Modify.Name = "B_Modify";
            this.B_Modify.Size = new System.Drawing.Size(47, 41);
            this.B_Modify.TabIndex = 25;
            this.B_Modify.Text = "Set All";
            this.B_Modify.UseVisualStyleBackColor = true;
            this.B_Modify.Click += new System.EventHandler(this.B_Modify_Click);
            // 
            // mnuFormNone
            // 
            this.mnuFormNone.Name = "mnuFormNone";
            this.mnuFormNone.Size = new System.Drawing.Size(32, 19);
            // 
            // mnuForm1
            // 
            this.mnuForm1.Name = "mnuForm1";
            this.mnuForm1.Size = new System.Drawing.Size(32, 19);
            // 
            // mnuFormAll
            // 
            this.mnuFormAll.Name = "mnuFormAll";
            this.mnuFormAll.Size = new System.Drawing.Size(32, 19);
            // 
            // NUD_SpeciesCaptured
            // 
            this.NUD_SpeciesCaptured.Location = new System.Drawing.Point(86, 17);
            this.NUD_SpeciesCaptured.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.NUD_SpeciesCaptured.Name = "NUD_SpeciesCaptured";
            this.NUD_SpeciesCaptured.Size = new System.Drawing.Size(45, 20);
            this.NUD_SpeciesCaptured.TabIndex = 53;
            this.NUD_SpeciesCaptured.Value = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            // 
            // NUD_SpeciesTransferred
            // 
            this.NUD_SpeciesTransferred.Location = new System.Drawing.Point(86, 40);
            this.NUD_SpeciesTransferred.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.NUD_SpeciesTransferred.Name = "NUD_SpeciesTransferred";
            this.NUD_SpeciesTransferred.Size = new System.Drawing.Size(75, 20);
            this.NUD_SpeciesTransferred.TabIndex = 54;
            this.NUD_SpeciesTransferred.Value = new decimal(new int[] {
            123456789,
            0,
            0,
            0});
            // 
            // NUD_TotalTransferred
            // 
            this.NUD_TotalTransferred.Location = new System.Drawing.Point(84, 42);
            this.NUD_TotalTransferred.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.NUD_TotalTransferred.Name = "NUD_TotalTransferred";
            this.NUD_TotalTransferred.Size = new System.Drawing.Size(75, 20);
            this.NUD_TotalTransferred.TabIndex = 56;
            this.NUD_TotalTransferred.Value = new decimal(new int[] {
            123456789,
            0,
            0,
            0});
            // 
            // NUD_TotalCaptured
            // 
            this.NUD_TotalCaptured.Location = new System.Drawing.Point(84, 19);
            this.NUD_TotalCaptured.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.NUD_TotalCaptured.Name = "NUD_TotalCaptured";
            this.NUD_TotalCaptured.Size = new System.Drawing.Size(75, 20);
            this.NUD_TotalCaptured.TabIndex = 55;
            this.NUD_TotalCaptured.Value = new decimal(new int[] {
            123456789,
            0,
            0,
            0});
            // 
            // L_SpeciesCaptured
            // 
            this.L_SpeciesCaptured.Location = new System.Drawing.Point(4, 17);
            this.L_SpeciesCaptured.Name = "L_SpeciesCaptured";
            this.L_SpeciesCaptured.Size = new System.Drawing.Size(80, 20);
            this.L_SpeciesCaptured.TabIndex = 61;
            this.L_SpeciesCaptured.Text = "Captured";
            this.L_SpeciesCaptured.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.L_SpeciesCaptured.Click += new System.EventHandler(this.ClickSpeciesLabel);
            // 
            // L_SpeciesTransferred
            // 
            this.L_SpeciesTransferred.Location = new System.Drawing.Point(4, 38);
            this.L_SpeciesTransferred.Name = "L_SpeciesTransferred";
            this.L_SpeciesTransferred.Size = new System.Drawing.Size(80, 20);
            this.L_SpeciesTransferred.TabIndex = 62;
            this.L_SpeciesTransferred.Text = "Transferred";
            this.L_SpeciesTransferred.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.L_SpeciesTransferred.Click += new System.EventHandler(this.ClickSpeciesLabel);
            // 
            // L_TotalTransferred
            // 
            this.L_TotalTransferred.Location = new System.Drawing.Point(2, 40);
            this.L_TotalTransferred.Name = "L_TotalTransferred";
            this.L_TotalTransferred.Size = new System.Drawing.Size(80, 20);
            this.L_TotalTransferred.TabIndex = 64;
            this.L_TotalTransferred.Text = "Transferred";
            this.L_TotalTransferred.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.L_TotalTransferred.Click += new System.EventHandler(this.ClickTotalLabel);
            // 
            // L_TotalCaptured
            // 
            this.L_TotalCaptured.Location = new System.Drawing.Point(2, 19);
            this.L_TotalCaptured.Name = "L_TotalCaptured";
            this.L_TotalCaptured.Size = new System.Drawing.Size(80, 20);
            this.L_TotalCaptured.TabIndex = 63;
            this.L_TotalCaptured.Text = "Captured";
            this.L_TotalCaptured.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.L_TotalCaptured.Click += new System.EventHandler(this.ClickTotalLabel);
            // 
            // GB_Species
            // 
            this.GB_Species.Controls.Add(this.NUD_SpeciesCaptured);
            this.GB_Species.Controls.Add(this.NUD_SpeciesTransferred);
            this.GB_Species.Controls.Add(this.L_SpeciesTransferred);
            this.GB_Species.Controls.Add(this.B_Modify);
            this.GB_Species.Controls.Add(this.L_SpeciesCaptured);
            this.GB_Species.Location = new System.Drawing.Point(178, 40);
            this.GB_Species.Name = "GB_Species";
            this.GB_Species.Size = new System.Drawing.Size(216, 71);
            this.GB_Species.TabIndex = 65;
            this.GB_Species.TabStop = false;
            this.GB_Species.Text = "Species Info";
            // 
            // B_SumTotal
            // 
            this.B_SumTotal.Location = new System.Drawing.Point(164, 19);
            this.B_SumTotal.Name = "B_SumTotal";
            this.B_SumTotal.Size = new System.Drawing.Size(47, 43);
            this.B_SumTotal.TabIndex = 66;
            this.B_SumTotal.Text = "Σ";
            this.B_SumTotal.UseVisualStyleBackColor = true;
            this.B_SumTotal.Click += new System.EventHandler(this.B_SumTotal_Click);
            // 
            // GB_Total
            // 
            this.GB_Total.Controls.Add(this.B_SumTotal);
            this.GB_Total.Controls.Add(this.NUD_TotalTransferred);
            this.GB_Total.Controls.Add(this.L_TotalCaptured);
            this.GB_Total.Controls.Add(this.L_TotalTransferred);
            this.GB_Total.Controls.Add(this.NUD_TotalCaptured);
            this.GB_Total.Location = new System.Drawing.Point(178, 117);
            this.GB_Total.Name = "GB_Total";
            this.GB_Total.Size = new System.Drawing.Size(216, 71);
            this.GB_Total.TabIndex = 68;
            this.GB_Total.TabStop = false;
            this.GB_Total.Text = "Totals";
            // 
            // SAV_Capture7GG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 326);
            this.Controls.Add(this.GB_Total);
            this.Controls.Add(this.GB_Species);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.CB_Species);
            this.Controls.Add(this.L_goto);
            this.Controls.Add(this.LB_Species);
            this.Controls.Add(this.B_Cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SAV_Capture7GG";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Capture Record Editor";
            ((System.ComponentModel.ISupportInitialize)(this.NUD_SpeciesCaptured)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_SpeciesTransferred)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_TotalTransferred)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_TotalCaptured)).EndInit();
            this.GB_Species.ResumeLayout(false);
            this.GB_Total.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

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
