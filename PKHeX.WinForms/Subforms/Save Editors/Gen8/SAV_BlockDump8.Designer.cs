namespace PKHeX.WinForms
{
    partial class SAV_BlockDump8
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
            this.CB_Key = new System.Windows.Forms.ComboBox();
            this.L_Key = new System.Windows.Forms.Label();
            this.L_Detail_L = new System.Windows.Forms.Label();
            this.L_Detail_R = new System.Windows.Forms.Label();
            this.B_ExportAll = new System.Windows.Forms.Button();
            this.B_ExportAllSingle = new System.Windows.Forms.Button();
            this.B_ImportCurrent = new System.Windows.Forms.Button();
            this.B_ExportCurrent = new System.Windows.Forms.Button();
            this.B_ImportFolder = new System.Windows.Forms.Button();
            this.CHK_Type = new System.Windows.Forms.CheckBox();
            this.CHK_DataOnly = new System.Windows.Forms.CheckBox();
            this.CHK_FakeHeader = new System.Windows.Forms.CheckBox();
            this.CHK_Key = new System.Windows.Forms.CheckBox();
            this.TC_Tabs = new System.Windows.Forms.TabControl();
            this.Tab_Dump = new System.Windows.Forms.TabPage();
            this.PG_BlockView = new System.Windows.Forms.PropertyGrid();
            this.RTB_Hex = new System.Windows.Forms.RichTextBox();
            this.CB_TypeToggle = new System.Windows.Forms.ComboBox();
            this.Tab_Compare = new System.Windows.Forms.TabPage();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.GB_Researcher = new System.Windows.Forms.GroupBox();
            this.TB_NewSAV = new System.Windows.Forms.TextBox();
            this.TB_OldSAV = new System.Windows.Forms.TextBox();
            this.B_LoadNew = new System.Windows.Forms.Button();
            this.B_LoadOld = new System.Windows.Forms.Button();
            this.L_BlockName = new System.Windows.Forms.Label();
            this.TC_Tabs.SuspendLayout();
            this.Tab_Dump.SuspendLayout();
            this.Tab_Compare.SuspendLayout();
            this.GB_Researcher.SuspendLayout();
            this.SuspendLayout();
            // 
            // CB_Key
            // 
            this.CB_Key.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.CB_Key.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Key.DropDownWidth = 270;
            this.CB_Key.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CB_Key.FormattingEnabled = true;
            this.CB_Key.Location = new System.Drawing.Point(83, 9);
            this.CB_Key.Name = "CB_Key";
            this.CB_Key.Size = new System.Drawing.Size(183, 22);
            this.CB_Key.TabIndex = 0;
            this.CB_Key.SelectedIndexChanged += new System.EventHandler(this.CB_Key_SelectedIndexChanged);
            // 
            // L_Key
            // 
            this.L_Key.AutoSize = true;
            this.L_Key.Location = new System.Drawing.Point(6, 12);
            this.L_Key.Name = "L_Key";
            this.L_Key.Size = new System.Drawing.Size(58, 13);
            this.L_Key.TabIndex = 1;
            this.L_Key.Text = "Block Key:";
            // 
            // L_Detail_L
            // 
            this.L_Detail_L.AutoSize = true;
            this.L_Detail_L.Location = new System.Drawing.Point(6, 61);
            this.L_Detail_L.Name = "L_Detail_L";
            this.L_Detail_L.Size = new System.Drawing.Size(67, 13);
            this.L_Detail_L.TabIndex = 2;
            this.L_Detail_L.Text = "Block Detail:";
            // 
            // L_Detail_R
            // 
            this.L_Detail_R.AutoSize = true;
            this.L_Detail_R.Location = new System.Drawing.Point(80, 61);
            this.L_Detail_R.Name = "L_Detail_R";
            this.L_Detail_R.Size = new System.Drawing.Size(69, 13);
            this.L_Detail_R.TabIndex = 3;
            this.L_Detail_R.Text = "Block Details";
            // 
            // B_ExportAll
            // 
            this.B_ExportAll.Location = new System.Drawing.Point(6, 140);
            this.B_ExportAll.Name = "B_ExportAll";
            this.B_ExportAll.Size = new System.Drawing.Size(91, 50);
            this.B_ExportAll.TabIndex = 4;
            this.B_ExportAll.Text = "Export Blocks To Folder";
            this.B_ExportAll.UseVisualStyleBackColor = true;
            this.B_ExportAll.Click += new System.EventHandler(this.B_ExportAll_Click);
            // 
            // B_ExportAllSingle
            // 
            this.B_ExportAllSingle.Location = new System.Drawing.Point(6, 252);
            this.B_ExportAllSingle.Name = "B_ExportAllSingle";
            this.B_ExportAllSingle.Size = new System.Drawing.Size(91, 50);
            this.B_ExportAllSingle.TabIndex = 5;
            this.B_ExportAllSingle.Text = "Export All (Single File)";
            this.B_ExportAllSingle.UseVisualStyleBackColor = true;
            this.B_ExportAllSingle.Click += new System.EventHandler(this.B_ExportAllSingle_Click);
            // 
            // B_ImportCurrent
            // 
            this.B_ImportCurrent.Location = new System.Drawing.Point(103, 196);
            this.B_ImportCurrent.Name = "B_ImportCurrent";
            this.B_ImportCurrent.Size = new System.Drawing.Size(91, 50);
            this.B_ImportCurrent.TabIndex = 7;
            this.B_ImportCurrent.Text = "Import Current Block";
            this.B_ImportCurrent.UseVisualStyleBackColor = true;
            this.B_ImportCurrent.Click += new System.EventHandler(this.B_ImportCurrent_Click);
            // 
            // B_ExportCurrent
            // 
            this.B_ExportCurrent.Location = new System.Drawing.Point(6, 196);
            this.B_ExportCurrent.Name = "B_ExportCurrent";
            this.B_ExportCurrent.Size = new System.Drawing.Size(91, 50);
            this.B_ExportCurrent.TabIndex = 6;
            this.B_ExportCurrent.Text = "Export Current Block";
            this.B_ExportCurrent.UseVisualStyleBackColor = true;
            this.B_ExportCurrent.Click += new System.EventHandler(this.B_ExportCurrent_Click);
            // 
            // B_ImportFolder
            // 
            this.B_ImportFolder.Location = new System.Drawing.Point(103, 140);
            this.B_ImportFolder.Name = "B_ImportFolder";
            this.B_ImportFolder.Size = new System.Drawing.Size(91, 50);
            this.B_ImportFolder.TabIndex = 8;
            this.B_ImportFolder.Text = "Import Blocks From Folder";
            this.B_ImportFolder.UseVisualStyleBackColor = true;
            this.B_ImportFolder.Click += new System.EventHandler(this.B_ImportFolder_Click);
            // 
            // CHK_Type
            // 
            this.CHK_Type.AutoSize = true;
            this.CHK_Type.Location = new System.Drawing.Point(103, 276);
            this.CHK_Type.Name = "CHK_Type";
            this.CHK_Type.Size = new System.Drawing.Size(109, 17);
            this.CHK_Type.TabIndex = 9;
            this.CHK_Type.Text = "Include Type Info";
            this.CHK_Type.UseVisualStyleBackColor = true;
            // 
            // CHK_DataOnly
            // 
            this.CHK_DataOnly.AutoSize = true;
            this.CHK_DataOnly.Checked = true;
            this.CHK_DataOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHK_DataOnly.Location = new System.Drawing.Point(103, 248);
            this.CHK_DataOnly.Name = "CHK_DataOnly";
            this.CHK_DataOnly.Size = new System.Drawing.Size(108, 17);
            this.CHK_DataOnly.TabIndex = 9;
            this.CHK_DataOnly.Text = "Data Blocks Only";
            this.CHK_DataOnly.UseVisualStyleBackColor = true;
            // 
            // CHK_FakeHeader
            // 
            this.CHK_FakeHeader.AutoSize = true;
            this.CHK_FakeHeader.Checked = true;
            this.CHK_FakeHeader.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHK_FakeHeader.Location = new System.Drawing.Point(103, 290);
            this.CHK_FakeHeader.Name = "CHK_FakeHeader";
            this.CHK_FakeHeader.Size = new System.Drawing.Size(141, 17);
            this.CHK_FakeHeader.TabIndex = 10;
            this.CHK_FakeHeader.Text = "Mark Block Start (ASCII)";
            this.CHK_FakeHeader.UseVisualStyleBackColor = true;
            // 
            // CHK_Key
            // 
            this.CHK_Key.AutoSize = true;
            this.CHK_Key.Location = new System.Drawing.Point(103, 262);
            this.CHK_Key.Name = "CHK_Key";
            this.CHK_Key.Size = new System.Drawing.Size(109, 17);
            this.CHK_Key.TabIndex = 11;
            this.CHK_Key.Text = "Include 32Bit Key";
            this.CHK_Key.UseVisualStyleBackColor = true;
            // 
            // TC_Tabs
            // 
            this.TC_Tabs.Controls.Add(this.Tab_Dump);
            this.TC_Tabs.Controls.Add(this.Tab_Compare);
            this.TC_Tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TC_Tabs.Location = new System.Drawing.Point(0, 0);
            this.TC_Tabs.Name = "TC_Tabs";
            this.TC_Tabs.SelectedIndex = 0;
            this.TC_Tabs.Size = new System.Drawing.Size(474, 341);
            this.TC_Tabs.TabIndex = 12;
            // 
            // Tab_Dump
            // 
            this.Tab_Dump.Controls.Add(this.PG_BlockView);
            this.Tab_Dump.Controls.Add(this.RTB_Hex);
            this.Tab_Dump.Controls.Add(this.CB_TypeToggle);
            this.Tab_Dump.Controls.Add(this.L_Key);
            this.Tab_Dump.Controls.Add(this.CHK_FakeHeader);
            this.Tab_Dump.Controls.Add(this.CB_Key);
            this.Tab_Dump.Controls.Add(this.CHK_Type);
            this.Tab_Dump.Controls.Add(this.L_Detail_L);
            this.Tab_Dump.Controls.Add(this.CHK_Key);
            this.Tab_Dump.Controls.Add(this.L_Detail_R);
            this.Tab_Dump.Controls.Add(this.CHK_DataOnly);
            this.Tab_Dump.Controls.Add(this.B_ExportAll);
            this.Tab_Dump.Controls.Add(this.B_ImportFolder);
            this.Tab_Dump.Controls.Add(this.B_ExportAllSingle);
            this.Tab_Dump.Controls.Add(this.B_ImportCurrent);
            this.Tab_Dump.Controls.Add(this.B_ExportCurrent);
            this.Tab_Dump.Location = new System.Drawing.Point(4, 22);
            this.Tab_Dump.Name = "Tab_Dump";
            this.Tab_Dump.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_Dump.Size = new System.Drawing.Size(466, 315);
            this.Tab_Dump.TabIndex = 0;
            this.Tab_Dump.Text = "Dump";
            this.Tab_Dump.UseVisualStyleBackColor = true;
            // 
            // PG_BlockView
            // 
            this.PG_BlockView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PG_BlockView.Location = new System.Drawing.Point(271, 9);
            this.PG_BlockView.Name = "PG_BlockView";
            this.PG_BlockView.Size = new System.Drawing.Size(114, 130);
            this.PG_BlockView.TabIndex = 14;
            this.PG_BlockView.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.PG_BlockView_PropertyValueChanged);
            // 
            // RTB_Hex
            // 
            this.RTB_Hex.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RTB_Hex.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RTB_Hex.Location = new System.Drawing.Point(271, 9);
            this.RTB_Hex.Name = "RTB_Hex";
            this.RTB_Hex.ReadOnly = true;
            this.RTB_Hex.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.RTB_Hex.Size = new System.Drawing.Size(192, 303);
            this.RTB_Hex.TabIndex = 13;
            this.RTB_Hex.Text = "00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F";
            // 
            // CB_TypeToggle
            // 
            this.CB_TypeToggle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_TypeToggle.FormattingEnabled = true;
            this.CB_TypeToggle.Location = new System.Drawing.Point(83, 37);
            this.CB_TypeToggle.Name = "CB_TypeToggle";
            this.CB_TypeToggle.Size = new System.Drawing.Size(97, 21);
            this.CB_TypeToggle.TabIndex = 12;
            this.CB_TypeToggle.Visible = false;
            // 
            // Tab_Compare
            // 
            this.Tab_Compare.Controls.Add(this.richTextBox1);
            this.Tab_Compare.Controls.Add(this.GB_Researcher);
            this.Tab_Compare.Location = new System.Drawing.Point(4, 22);
            this.Tab_Compare.Name = "Tab_Compare";
            this.Tab_Compare.Size = new System.Drawing.Size(466, 315);
            this.Tab_Compare.TabIndex = 1;
            this.Tab_Compare.Text = "Compare";
            this.Tab_Compare.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(0, 76);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(466, 239);
            this.richTextBox1.TabIndex = 15;
            this.richTextBox1.Text = "";
            // 
            // GB_Researcher
            // 
            this.GB_Researcher.Controls.Add(this.TB_NewSAV);
            this.GB_Researcher.Controls.Add(this.TB_OldSAV);
            this.GB_Researcher.Controls.Add(this.B_LoadNew);
            this.GB_Researcher.Controls.Add(this.B_LoadOld);
            this.GB_Researcher.Dock = System.Windows.Forms.DockStyle.Top;
            this.GB_Researcher.Location = new System.Drawing.Point(0, 0);
            this.GB_Researcher.Name = "GB_Researcher";
            this.GB_Researcher.Size = new System.Drawing.Size(466, 76);
            this.GB_Researcher.TabIndex = 14;
            this.GB_Researcher.TabStop = false;
            this.GB_Researcher.Text = "Load Two Save Files";
            // 
            // TB_NewSAV
            // 
            this.TB_NewSAV.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_NewSAV.Location = new System.Drawing.Point(93, 47);
            this.TB_NewSAV.Name = "TB_NewSAV";
            this.TB_NewSAV.ReadOnly = true;
            this.TB_NewSAV.Size = new System.Drawing.Size(367, 20);
            this.TB_NewSAV.TabIndex = 5;
            // 
            // TB_OldSAV
            // 
            this.TB_OldSAV.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_OldSAV.Location = new System.Drawing.Point(93, 21);
            this.TB_OldSAV.Name = "TB_OldSAV";
            this.TB_OldSAV.ReadOnly = true;
            this.TB_OldSAV.Size = new System.Drawing.Size(367, 20);
            this.TB_OldSAV.TabIndex = 4;
            // 
            // B_LoadNew
            // 
            this.B_LoadNew.Location = new System.Drawing.Point(12, 45);
            this.B_LoadNew.Name = "B_LoadNew";
            this.B_LoadNew.Size = new System.Drawing.Size(75, 23);
            this.B_LoadNew.TabIndex = 1;
            this.B_LoadNew.Text = "Load New";
            this.B_LoadNew.UseVisualStyleBackColor = true;
            this.B_LoadNew.Click += new System.EventHandler(this.B_LoadNew_Click);
            // 
            // B_LoadOld
            // 
            this.B_LoadOld.Location = new System.Drawing.Point(12, 19);
            this.B_LoadOld.Name = "B_LoadOld";
            this.B_LoadOld.Size = new System.Drawing.Size(75, 23);
            this.B_LoadOld.TabIndex = 0;
            this.B_LoadOld.Text = "Load Old";
            this.B_LoadOld.UseVisualStyleBackColor = true;
            this.B_LoadOld.Click += new System.EventHandler(this.B_LoadOld_Click);
            // 
            // L_BlockName
            // 
            this.L_BlockName.AutoSize = true;
            this.L_BlockName.ForeColor = System.Drawing.Color.Red;
            this.L_BlockName.Location = new System.Drawing.Point(271, 4);
            this.L_BlockName.Name = "L_BlockName";
            this.L_BlockName.Size = new System.Drawing.Size(107, 13);
            this.L_BlockName.TabIndex = 14;
            this.L_BlockName.Text = "HIDDEN:BlockName";
            this.L_BlockName.Visible = false;
            // 
            // SAV_BlockDump8
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 341);
            this.Controls.Add(this.L_BlockName);
            this.Controls.Add(this.TC_Tabs);
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(490, 380);
            this.Name = "SAV_BlockDump8";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Savedata Block Dump";
            this.TC_Tabs.ResumeLayout(false);
            this.Tab_Dump.ResumeLayout(false);
            this.Tab_Dump.PerformLayout();
            this.Tab_Compare.ResumeLayout(false);
            this.GB_Researcher.ResumeLayout(false);
            this.GB_Researcher.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox CB_Key;
        private System.Windows.Forms.Label L_Key;
        private System.Windows.Forms.Label L_Detail_L;
        private System.Windows.Forms.Label L_Detail_R;
        private System.Windows.Forms.Button B_ExportAll;
        private System.Windows.Forms.Button B_ExportAllSingle;
        private System.Windows.Forms.Button B_ImportCurrent;
        private System.Windows.Forms.Button B_ExportCurrent;
        private System.Windows.Forms.Button B_ImportFolder;
        private System.Windows.Forms.CheckBox CHK_Type;
        private System.Windows.Forms.CheckBox CHK_DataOnly;
        private System.Windows.Forms.CheckBox CHK_FakeHeader;
        private System.Windows.Forms.CheckBox CHK_Key;
        private System.Windows.Forms.TabControl TC_Tabs;
        private System.Windows.Forms.TabPage Tab_Dump;
        private System.Windows.Forms.TabPage Tab_Compare;
        private System.Windows.Forms.ComboBox CB_TypeToggle;
        private System.Windows.Forms.GroupBox GB_Researcher;
        private System.Windows.Forms.TextBox TB_NewSAV;
        private System.Windows.Forms.TextBox TB_OldSAV;
        private System.Windows.Forms.Button B_LoadNew;
        private System.Windows.Forms.Button B_LoadOld;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.RichTextBox RTB_Hex;
        private System.Windows.Forms.Label L_BlockName;
        private System.Windows.Forms.PropertyGrid PG_BlockView;
    }
}