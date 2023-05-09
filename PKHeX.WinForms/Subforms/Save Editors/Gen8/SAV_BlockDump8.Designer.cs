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
            CB_Key = new System.Windows.Forms.ComboBox();
            L_Key = new System.Windows.Forms.Label();
            L_Detail_L = new System.Windows.Forms.Label();
            L_Detail_R = new System.Windows.Forms.Label();
            B_ExportAll = new System.Windows.Forms.Button();
            B_ExportAllSingle = new System.Windows.Forms.Button();
            B_ImportCurrent = new System.Windows.Forms.Button();
            B_ExportCurrent = new System.Windows.Forms.Button();
            B_ImportFolder = new System.Windows.Forms.Button();
            CHK_Type = new System.Windows.Forms.CheckBox();
            CHK_DataOnly = new System.Windows.Forms.CheckBox();
            CHK_FakeHeader = new System.Windows.Forms.CheckBox();
            CHK_Key = new System.Windows.Forms.CheckBox();
            TC_Tabs = new System.Windows.Forms.TabControl();
            Tab_Dump = new System.Windows.Forms.TabPage();
            PG_BlockView = new System.Windows.Forms.PropertyGrid();
            RTB_Hex = new System.Windows.Forms.RichTextBox();
            CB_TypeToggle = new System.Windows.Forms.ComboBox();
            Tab_Compare = new System.Windows.Forms.TabPage();
            richTextBox1 = new System.Windows.Forms.RichTextBox();
            GB_Researcher = new System.Windows.Forms.GroupBox();
            TB_NewSAV = new System.Windows.Forms.TextBox();
            TB_OldSAV = new System.Windows.Forms.TextBox();
            B_LoadNew = new System.Windows.Forms.Button();
            B_LoadOld = new System.Windows.Forms.Button();
            L_BlockName = new System.Windows.Forms.Label();
            TC_Tabs.SuspendLayout();
            Tab_Dump.SuspendLayout();
            Tab_Compare.SuspendLayout();
            GB_Researcher.SuspendLayout();
            SuspendLayout();
            // 
            // CB_Key
            // 
            CB_Key.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            CB_Key.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Key.DropDownWidth = 270;
            CB_Key.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CB_Key.FormattingEnabled = true;
            CB_Key.Location = new System.Drawing.Point(97, 10);
            CB_Key.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Key.Name = "CB_Key";
            CB_Key.Size = new System.Drawing.Size(213, 22);
            CB_Key.TabIndex = 0;
            CB_Key.SelectedIndexChanged += CB_Key_SelectedIndexChanged;
            CB_Key.KeyDown += CB_Key_KeyDown;
            // 
            // L_Key
            // 
            L_Key.AutoSize = true;
            L_Key.Location = new System.Drawing.Point(7, 14);
            L_Key.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Key.Name = "L_Key";
            L_Key.Size = new System.Drawing.Size(61, 15);
            L_Key.TabIndex = 1;
            L_Key.Text = "Block Key:";
            // 
            // L_Detail_L
            // 
            L_Detail_L.AutoSize = true;
            L_Detail_L.Location = new System.Drawing.Point(7, 70);
            L_Detail_L.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Detail_L.Name = "L_Detail_L";
            L_Detail_L.Size = new System.Drawing.Size(72, 15);
            L_Detail_L.TabIndex = 2;
            L_Detail_L.Text = "Block Detail:";
            // 
            // L_Detail_R
            // 
            L_Detail_R.AutoSize = true;
            L_Detail_R.Location = new System.Drawing.Point(93, 70);
            L_Detail_R.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Detail_R.Name = "L_Detail_R";
            L_Detail_R.Size = new System.Drawing.Size(74, 15);
            L_Detail_R.TabIndex = 3;
            L_Detail_R.Text = "Block Details";
            // 
            // B_ExportAll
            // 
            B_ExportAll.Location = new System.Drawing.Point(7, 162);
            B_ExportAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ExportAll.Name = "B_ExportAll";
            B_ExportAll.Size = new System.Drawing.Size(106, 58);
            B_ExportAll.TabIndex = 4;
            B_ExportAll.Text = "Export Blocks To Folder";
            B_ExportAll.UseVisualStyleBackColor = true;
            B_ExportAll.Click += B_ExportAll_Click;
            // 
            // B_ExportAllSingle
            // 
            B_ExportAllSingle.Location = new System.Drawing.Point(7, 291);
            B_ExportAllSingle.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ExportAllSingle.Name = "B_ExportAllSingle";
            B_ExportAllSingle.Size = new System.Drawing.Size(106, 58);
            B_ExportAllSingle.TabIndex = 5;
            B_ExportAllSingle.Text = "Export All (Single File)";
            B_ExportAllSingle.UseVisualStyleBackColor = true;
            B_ExportAllSingle.Click += B_ExportAllSingle_Click;
            // 
            // B_ImportCurrent
            // 
            B_ImportCurrent.Location = new System.Drawing.Point(120, 226);
            B_ImportCurrent.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ImportCurrent.Name = "B_ImportCurrent";
            B_ImportCurrent.Size = new System.Drawing.Size(106, 58);
            B_ImportCurrent.TabIndex = 7;
            B_ImportCurrent.Text = "Import Current Block";
            B_ImportCurrent.UseVisualStyleBackColor = true;
            B_ImportCurrent.Click += B_ImportCurrent_Click;
            // 
            // B_ExportCurrent
            // 
            B_ExportCurrent.Location = new System.Drawing.Point(7, 226);
            B_ExportCurrent.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ExportCurrent.Name = "B_ExportCurrent";
            B_ExportCurrent.Size = new System.Drawing.Size(106, 58);
            B_ExportCurrent.TabIndex = 6;
            B_ExportCurrent.Text = "Export Current Block";
            B_ExportCurrent.UseVisualStyleBackColor = true;
            B_ExportCurrent.Click += B_ExportCurrent_Click;
            // 
            // B_ImportFolder
            // 
            B_ImportFolder.Location = new System.Drawing.Point(120, 162);
            B_ImportFolder.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ImportFolder.Name = "B_ImportFolder";
            B_ImportFolder.Size = new System.Drawing.Size(106, 58);
            B_ImportFolder.TabIndex = 8;
            B_ImportFolder.Text = "Import Blocks From Folder";
            B_ImportFolder.UseVisualStyleBackColor = true;
            B_ImportFolder.Click += B_ImportFolder_Click;
            // 
            // CHK_Type
            // 
            CHK_Type.AutoSize = true;
            CHK_Type.Location = new System.Drawing.Point(120, 318);
            CHK_Type.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_Type.Name = "CHK_Type";
            CHK_Type.Size = new System.Drawing.Size(116, 19);
            CHK_Type.TabIndex = 9;
            CHK_Type.Text = "Include Type Info";
            CHK_Type.UseVisualStyleBackColor = true;
            // 
            // CHK_DataOnly
            // 
            CHK_DataOnly.AutoSize = true;
            CHK_DataOnly.Checked = true;
            CHK_DataOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            CHK_DataOnly.Location = new System.Drawing.Point(120, 286);
            CHK_DataOnly.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_DataOnly.Name = "CHK_DataOnly";
            CHK_DataOnly.Size = new System.Drawing.Size(115, 19);
            CHK_DataOnly.TabIndex = 9;
            CHK_DataOnly.Text = "Data Blocks Only";
            CHK_DataOnly.UseVisualStyleBackColor = true;
            // 
            // CHK_FakeHeader
            // 
            CHK_FakeHeader.AutoSize = true;
            CHK_FakeHeader.Checked = true;
            CHK_FakeHeader.CheckState = System.Windows.Forms.CheckState.Checked;
            CHK_FakeHeader.Location = new System.Drawing.Point(120, 335);
            CHK_FakeHeader.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_FakeHeader.Name = "CHK_FakeHeader";
            CHK_FakeHeader.Size = new System.Drawing.Size(151, 19);
            CHK_FakeHeader.TabIndex = 10;
            CHK_FakeHeader.Text = "Mark Block Start (ASCII)";
            CHK_FakeHeader.UseVisualStyleBackColor = true;
            // 
            // CHK_Key
            // 
            CHK_Key.AutoSize = true;
            CHK_Key.Location = new System.Drawing.Point(120, 302);
            CHK_Key.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_Key.Name = "CHK_Key";
            CHK_Key.Size = new System.Drawing.Size(116, 19);
            CHK_Key.TabIndex = 11;
            CHK_Key.Text = "Include 32Bit Key";
            CHK_Key.UseVisualStyleBackColor = true;
            // 
            // TC_Tabs
            // 
            TC_Tabs.Controls.Add(Tab_Dump);
            TC_Tabs.Controls.Add(Tab_Compare);
            TC_Tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            TC_Tabs.Location = new System.Drawing.Point(0, 0);
            TC_Tabs.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TC_Tabs.Name = "TC_Tabs";
            TC_Tabs.SelectedIndex = 0;
            TC_Tabs.Size = new System.Drawing.Size(553, 393);
            TC_Tabs.TabIndex = 12;
            // 
            // Tab_Dump
            // 
            Tab_Dump.Controls.Add(PG_BlockView);
            Tab_Dump.Controls.Add(RTB_Hex);
            Tab_Dump.Controls.Add(CB_TypeToggle);
            Tab_Dump.Controls.Add(L_Key);
            Tab_Dump.Controls.Add(CHK_FakeHeader);
            Tab_Dump.Controls.Add(CB_Key);
            Tab_Dump.Controls.Add(CHK_Type);
            Tab_Dump.Controls.Add(L_Detail_L);
            Tab_Dump.Controls.Add(CHK_Key);
            Tab_Dump.Controls.Add(L_Detail_R);
            Tab_Dump.Controls.Add(CHK_DataOnly);
            Tab_Dump.Controls.Add(B_ExportAll);
            Tab_Dump.Controls.Add(B_ImportFolder);
            Tab_Dump.Controls.Add(B_ExportAllSingle);
            Tab_Dump.Controls.Add(B_ImportCurrent);
            Tab_Dump.Controls.Add(B_ExportCurrent);
            Tab_Dump.Location = new System.Drawing.Point(4, 24);
            Tab_Dump.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_Dump.Name = "Tab_Dump";
            Tab_Dump.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_Dump.Size = new System.Drawing.Size(545, 365);
            Tab_Dump.TabIndex = 0;
            Tab_Dump.Text = "Dump";
            Tab_Dump.UseVisualStyleBackColor = true;
            // 
            // PG_BlockView
            // 
            PG_BlockView.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            PG_BlockView.Location = new System.Drawing.Point(316, 10);
            PG_BlockView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PG_BlockView.Name = "PG_BlockView";
            PG_BlockView.Size = new System.Drawing.Size(133, 150);
            PG_BlockView.TabIndex = 14;
            PG_BlockView.PropertyValueChanged += PG_BlockView_PropertyValueChanged;
            // 
            // RTB_Hex
            // 
            RTB_Hex.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            RTB_Hex.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RTB_Hex.Location = new System.Drawing.Point(316, 10);
            RTB_Hex.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RTB_Hex.Name = "RTB_Hex";
            RTB_Hex.ReadOnly = true;
            RTB_Hex.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            RTB_Hex.Size = new System.Drawing.Size(223, 349);
            RTB_Hex.TabIndex = 13;
            RTB_Hex.Text = "00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F";
            // 
            // CB_TypeToggle
            // 
            CB_TypeToggle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_TypeToggle.FormattingEnabled = true;
            CB_TypeToggle.Location = new System.Drawing.Point(97, 43);
            CB_TypeToggle.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_TypeToggle.Name = "CB_TypeToggle";
            CB_TypeToggle.Size = new System.Drawing.Size(112, 23);
            CB_TypeToggle.TabIndex = 12;
            CB_TypeToggle.Visible = false;
            // 
            // Tab_Compare
            // 
            Tab_Compare.Controls.Add(richTextBox1);
            Tab_Compare.Controls.Add(GB_Researcher);
            Tab_Compare.Location = new System.Drawing.Point(4, 24);
            Tab_Compare.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_Compare.Name = "Tab_Compare";
            Tab_Compare.Size = new System.Drawing.Size(545, 365);
            Tab_Compare.TabIndex = 1;
            Tab_Compare.Text = "Compare";
            Tab_Compare.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            richTextBox1.Location = new System.Drawing.Point(0, 88);
            richTextBox1.Margin = new System.Windows.Forms.Padding(0);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.Size = new System.Drawing.Size(545, 277);
            richTextBox1.TabIndex = 15;
            richTextBox1.Text = "";
            // 
            // GB_Researcher
            // 
            GB_Researcher.Controls.Add(TB_NewSAV);
            GB_Researcher.Controls.Add(TB_OldSAV);
            GB_Researcher.Controls.Add(B_LoadNew);
            GB_Researcher.Controls.Add(B_LoadOld);
            GB_Researcher.Dock = System.Windows.Forms.DockStyle.Top;
            GB_Researcher.Location = new System.Drawing.Point(0, 0);
            GB_Researcher.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Researcher.Name = "GB_Researcher";
            GB_Researcher.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Researcher.Size = new System.Drawing.Size(545, 88);
            GB_Researcher.TabIndex = 14;
            GB_Researcher.TabStop = false;
            GB_Researcher.Text = "Load Two Save Files";
            // 
            // TB_NewSAV
            // 
            TB_NewSAV.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TB_NewSAV.Location = new System.Drawing.Point(108, 54);
            TB_NewSAV.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_NewSAV.Name = "TB_NewSAV";
            TB_NewSAV.ReadOnly = true;
            TB_NewSAV.Size = new System.Drawing.Size(429, 23);
            TB_NewSAV.TabIndex = 5;
            // 
            // TB_OldSAV
            // 
            TB_OldSAV.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TB_OldSAV.Location = new System.Drawing.Point(108, 24);
            TB_OldSAV.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_OldSAV.Name = "TB_OldSAV";
            TB_OldSAV.ReadOnly = true;
            TB_OldSAV.Size = new System.Drawing.Size(429, 23);
            TB_OldSAV.TabIndex = 4;
            // 
            // B_LoadNew
            // 
            B_LoadNew.Location = new System.Drawing.Point(14, 52);
            B_LoadNew.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_LoadNew.Name = "B_LoadNew";
            B_LoadNew.Size = new System.Drawing.Size(88, 27);
            B_LoadNew.TabIndex = 1;
            B_LoadNew.Text = "Load New";
            B_LoadNew.UseVisualStyleBackColor = true;
            B_LoadNew.Click += B_LoadNew_Click;
            // 
            // B_LoadOld
            // 
            B_LoadOld.Location = new System.Drawing.Point(14, 22);
            B_LoadOld.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_LoadOld.Name = "B_LoadOld";
            B_LoadOld.Size = new System.Drawing.Size(88, 27);
            B_LoadOld.TabIndex = 0;
            B_LoadOld.Text = "Load Old";
            B_LoadOld.UseVisualStyleBackColor = true;
            B_LoadOld.Click += B_LoadOld_Click;
            // 
            // L_BlockName
            // 
            L_BlockName.AutoSize = true;
            L_BlockName.ForeColor = System.Drawing.Color.Red;
            L_BlockName.Location = new System.Drawing.Point(316, 5);
            L_BlockName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_BlockName.Name = "L_BlockName";
            L_BlockName.Size = new System.Drawing.Size(114, 15);
            L_BlockName.TabIndex = 14;
            L_BlockName.Text = "HIDDEN:BlockName";
            L_BlockName.Visible = false;
            // 
            // SAV_BlockDump8
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(553, 393);
            Controls.Add(L_BlockName);
            Controls.Add(TC_Tabs);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(569, 432);
            Name = "SAV_BlockDump8";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Savedata Block Dump";
            TC_Tabs.ResumeLayout(false);
            Tab_Dump.ResumeLayout(false);
            Tab_Dump.PerformLayout();
            Tab_Compare.ResumeLayout(false);
            GB_Researcher.ResumeLayout(false);
            GB_Researcher.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
