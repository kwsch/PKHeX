namespace PKHeX.WinForms
{
    partial class SAV_PokedexBDSPLumi
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
            this.B_Cancel = new System.Windows.Forms.Button();
            this.LB_Species = new System.Windows.Forms.ListBox();
            this.CHK_LangKOR = new System.Windows.Forms.CheckBox();
            this.CHK_LangSPA = new System.Windows.Forms.CheckBox();
            this.CHK_LangGER = new System.Windows.Forms.CheckBox();
            this.CHK_LangITA = new System.Windows.Forms.CheckBox();
            this.CHK_LangFRE = new System.Windows.Forms.CheckBox();
            this.CHK_LangENG = new System.Windows.Forms.CheckBox();
            this.CHK_LangJPN = new System.Windows.Forms.CheckBox();
            this.L_goto = new System.Windows.Forms.Label();
            this.CB_Species = new System.Windows.Forms.ComboBox();
            this.B_GiveAll = new System.Windows.Forms.Button();
            this.B_Save = new System.Windows.Forms.Button();
            this.B_Modify = new System.Windows.Forms.Button();
            this.GB_Language = new System.Windows.Forms.GroupBox();
            this.CHK_LangCHT = new System.Windows.Forms.CheckBox();
            this.CHK_LangCHS = new System.Windows.Forms.CheckBox();
            this.GB_Encountered = new System.Windows.Forms.GroupBox();
            this.CHK_FS = new System.Windows.Forms.CheckBox();
            this.CHK_MS = new System.Windows.Forms.CheckBox();
            this.CHK_F = new System.Windows.Forms.CheckBox();
            this.CHK_M = new System.Windows.Forms.CheckBox();
            this.modifyMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuSeenNone = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSeenAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCaughtNone = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCaughtAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuComplete = new System.Windows.Forms.ToolStripMenuItem();
            this.CLB_FormRegular = new System.Windows.Forms.CheckedListBox();
            this.L_FormsSeen = new System.Windows.Forms.Label();
            this.CLB_FormShiny = new System.Windows.Forms.CheckedListBox();
            this.L_FormDisplayed = new System.Windows.Forms.Label();
            this.modifyMenuForms = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuFormNone = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFormAllRegular = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFormAllShinies = new System.Windows.Forms.ToolStripMenuItem();
            this.CB_State = new System.Windows.Forms.ComboBox();
            this.B_ModifyForms = new System.Windows.Forms.Button();
            this.CHK_National = new System.Windows.Forms.CheckBox();
            this.GB_Language.SuspendLayout();
            this.GB_Encountered.SuspendLayout();
            this.modifyMenu.SuspendLayout();
            this.modifyMenuForms.SuspendLayout();
            this.SuspendLayout();
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(351, 297);
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
            this.LB_Species.Size = new System.Drawing.Size(130, 277);
            this.LB_Species.TabIndex = 2;
            this.LB_Species.SelectedIndexChanged += new System.EventHandler(this.ChangeLBSpecies);
            // 
            // CHK_LangKOR
            // 
            this.CHK_LangKOR.AutoSize = true;
            this.CHK_LangKOR.Location = new System.Drawing.Point(6, 98);
            this.CHK_LangKOR.Name = "CHK_LangKOR";
            this.CHK_LangKOR.Size = new System.Drawing.Size(60, 17);
            this.CHK_LangKOR.TabIndex = 19;
            this.CHK_LangKOR.Text = "Korean";
            this.CHK_LangKOR.UseVisualStyleBackColor = true;
            // 
            // CHK_LangSPA
            // 
            this.CHK_LangSPA.AutoSize = true;
            this.CHK_LangSPA.Location = new System.Drawing.Point(6, 84);
            this.CHK_LangSPA.Name = "CHK_LangSPA";
            this.CHK_LangSPA.Size = new System.Drawing.Size(64, 17);
            this.CHK_LangSPA.TabIndex = 18;
            this.CHK_LangSPA.Text = "Spanish";
            this.CHK_LangSPA.UseVisualStyleBackColor = true;
            // 
            // CHK_LangGER
            // 
            this.CHK_LangGER.AutoSize = true;
            this.CHK_LangGER.Location = new System.Drawing.Point(6, 70);
            this.CHK_LangGER.Name = "CHK_LangGER";
            this.CHK_LangGER.Size = new System.Drawing.Size(63, 17);
            this.CHK_LangGER.TabIndex = 17;
            this.CHK_LangGER.Text = "German";
            this.CHK_LangGER.UseVisualStyleBackColor = true;
            // 
            // CHK_LangITA
            // 
            this.CHK_LangITA.AutoSize = true;
            this.CHK_LangITA.Location = new System.Drawing.Point(6, 56);
            this.CHK_LangITA.Name = "CHK_LangITA";
            this.CHK_LangITA.Size = new System.Drawing.Size(54, 17);
            this.CHK_LangITA.TabIndex = 16;
            this.CHK_LangITA.Text = "Italian";
            this.CHK_LangITA.UseVisualStyleBackColor = true;
            // 
            // CHK_LangFRE
            // 
            this.CHK_LangFRE.AutoSize = true;
            this.CHK_LangFRE.Location = new System.Drawing.Point(6, 42);
            this.CHK_LangFRE.Name = "CHK_LangFRE";
            this.CHK_LangFRE.Size = new System.Drawing.Size(59, 17);
            this.CHK_LangFRE.TabIndex = 15;
            this.CHK_LangFRE.Text = "French";
            this.CHK_LangFRE.UseVisualStyleBackColor = true;
            // 
            // CHK_LangENG
            // 
            this.CHK_LangENG.AutoSize = true;
            this.CHK_LangENG.Location = new System.Drawing.Point(6, 28);
            this.CHK_LangENG.Name = "CHK_LangENG";
            this.CHK_LangENG.Size = new System.Drawing.Size(60, 17);
            this.CHK_LangENG.TabIndex = 14;
            this.CHK_LangENG.Text = "English";
            this.CHK_LangENG.UseVisualStyleBackColor = true;
            // 
            // CHK_LangJPN
            // 
            this.CHK_LangJPN.AutoSize = true;
            this.CHK_LangJPN.Location = new System.Drawing.Point(6, 14);
            this.CHK_LangJPN.Name = "CHK_LangJPN";
            this.CHK_LangJPN.Size = new System.Drawing.Size(72, 17);
            this.CHK_LangJPN.TabIndex = 13;
            this.CHK_LangJPN.Text = "Japanese";
            this.CHK_LangJPN.UseVisualStyleBackColor = true;
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
            this.CB_Species.Size = new System.Drawing.Size(92, 21);
            this.CB_Species.TabIndex = 21;
            this.CB_Species.SelectedIndexChanged += new System.EventHandler(this.ChangeCBSpecies);
            this.CB_Species.SelectedValueChanged += new System.EventHandler(this.ChangeCBSpecies);
            // 
            // B_GiveAll
            // 
            this.B_GiveAll.Location = new System.Drawing.Point(149, 11);
            this.B_GiveAll.Name = "B_GiveAll";
            this.B_GiveAll.Size = new System.Drawing.Size(60, 23);
            this.B_GiveAll.TabIndex = 23;
            this.B_GiveAll.Text = "Check All";
            this.B_GiveAll.UseVisualStyleBackColor = true;
            this.B_GiveAll.Click += new System.EventHandler(this.B_GiveAll_Click);
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(437, 297);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(80, 23);
            this.B_Save.TabIndex = 24;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // B_Modify
            // 
            this.B_Modify.Location = new System.Drawing.Point(233, 11);
            this.B_Modify.Name = "B_Modify";
            this.B_Modify.Size = new System.Drawing.Size(60, 23);
            this.B_Modify.TabIndex = 25;
            this.B_Modify.Text = "Modify...";
            this.B_Modify.UseVisualStyleBackColor = true;
            this.B_Modify.Click += new System.EventHandler(this.B_Modify_Click);
            // 
            // GB_Language
            // 
            this.GB_Language.Controls.Add(this.CHK_LangCHT);
            this.GB_Language.Controls.Add(this.CHK_LangCHS);
            this.GB_Language.Controls.Add(this.CHK_LangKOR);
            this.GB_Language.Controls.Add(this.CHK_LangSPA);
            this.GB_Language.Controls.Add(this.CHK_LangGER);
            this.GB_Language.Controls.Add(this.CHK_LangITA);
            this.GB_Language.Controls.Add(this.CHK_LangFRE);
            this.GB_Language.Controls.Add(this.CHK_LangENG);
            this.GB_Language.Controls.Add(this.CHK_LangJPN);
            this.GB_Language.Location = new System.Drawing.Point(149, 146);
            this.GB_Language.Name = "GB_Language";
            this.GB_Language.Size = new System.Drawing.Size(115, 145);
            this.GB_Language.TabIndex = 26;
            this.GB_Language.TabStop = false;
            this.GB_Language.Text = "Languages";
            // 
            // CHK_LangCHT
            // 
            this.CHK_LangCHT.AutoSize = true;
            this.CHK_LangCHT.Location = new System.Drawing.Point(6, 126);
            this.CHK_LangCHT.Name = "CHK_LangCHT";
            this.CHK_LangCHT.Size = new System.Drawing.Size(71, 17);
            this.CHK_LangCHT.TabIndex = 21;
            this.CHK_LangCHT.Text = "ChineseT";
            this.CHK_LangCHT.UseVisualStyleBackColor = true;
            // 
            // CHK_LangCHS
            // 
            this.CHK_LangCHS.AutoSize = true;
            this.CHK_LangCHS.Location = new System.Drawing.Point(6, 112);
            this.CHK_LangCHS.Name = "CHK_LangCHS";
            this.CHK_LangCHS.Size = new System.Drawing.Size(71, 17);
            this.CHK_LangCHS.TabIndex = 20;
            this.CHK_LangCHS.Text = "ChineseS";
            this.CHK_LangCHS.UseVisualStyleBackColor = true;
            // 
            // GB_Encountered
            // 
            this.GB_Encountered.Controls.Add(this.CHK_FS);
            this.GB_Encountered.Controls.Add(this.CHK_MS);
            this.GB_Encountered.Controls.Add(this.CHK_F);
            this.GB_Encountered.Controls.Add(this.CHK_M);
            this.GB_Encountered.Location = new System.Drawing.Point(149, 69);
            this.GB_Encountered.Name = "GB_Encountered";
            this.GB_Encountered.Size = new System.Drawing.Size(115, 72);
            this.GB_Encountered.TabIndex = 31;
            this.GB_Encountered.TabStop = false;
            this.GB_Encountered.Text = "Seen";
            // 
            // CHK_FS
            // 
            this.CHK_FS.AutoSize = true;
            this.CHK_FS.Location = new System.Drawing.Point(6, 55);
            this.CHK_FS.Name = "CHK_FS";
            this.CHK_FS.Size = new System.Drawing.Size(89, 17);
            this.CHK_FS.TabIndex = 7;
            this.CHK_FS.Text = "Shiny Female";
            this.CHK_FS.UseVisualStyleBackColor = true;
            // 
            // CHK_MS
            // 
            this.CHK_MS.AutoSize = true;
            this.CHK_MS.Location = new System.Drawing.Point(6, 41);
            this.CHK_MS.Name = "CHK_MS";
            this.CHK_MS.Size = new System.Drawing.Size(78, 17);
            this.CHK_MS.TabIndex = 6;
            this.CHK_MS.Text = "Shiny Male";
            this.CHK_MS.UseVisualStyleBackColor = true;
            // 
            // CHK_F
            // 
            this.CHK_F.AutoSize = true;
            this.CHK_F.Location = new System.Drawing.Point(6, 27);
            this.CHK_F.Name = "CHK_F";
            this.CHK_F.Size = new System.Drawing.Size(60, 17);
            this.CHK_F.TabIndex = 5;
            this.CHK_F.Text = "Female";
            this.CHK_F.UseVisualStyleBackColor = true;
            // 
            // CHK_M
            // 
            this.CHK_M.AutoSize = true;
            this.CHK_M.Location = new System.Drawing.Point(6, 13);
            this.CHK_M.Name = "CHK_M";
            this.CHK_M.Size = new System.Drawing.Size(49, 17);
            this.CHK_M.TabIndex = 4;
            this.CHK_M.Text = "Male";
            this.CHK_M.UseVisualStyleBackColor = true;
            // 
            // modifyMenu
            // 
            this.modifyMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSeenNone,
            this.mnuSeenAll,
            this.mnuCaughtNone,
            this.mnuCaughtAll,
            this.mnuComplete});
            this.modifyMenu.Name = "modifyMenu";
            this.modifyMenu.Size = new System.Drawing.Size(150, 114);
            // 
            // mnuSeenNone
            // 
            this.mnuSeenNone.Name = "mnuSeenNone";
            this.mnuSeenNone.Size = new System.Drawing.Size(149, 22);
            this.mnuSeenNone.Text = "Seen none";
            this.mnuSeenNone.Click += new System.EventHandler(this.ModifyAll);
            // 
            // mnuSeenAll
            // 
            this.mnuSeenAll.Name = "mnuSeenAll";
            this.mnuSeenAll.Size = new System.Drawing.Size(149, 22);
            this.mnuSeenAll.Text = "Seen all";
            this.mnuSeenAll.Click += new System.EventHandler(this.ModifyAll);
            // 
            // mnuCaughtNone
            // 
            this.mnuCaughtNone.Name = "mnuCaughtNone";
            this.mnuCaughtNone.Size = new System.Drawing.Size(149, 22);
            this.mnuCaughtNone.Text = "Caught none";
            this.mnuCaughtNone.Click += new System.EventHandler(this.ModifyAll);
            // 
            // mnuCaughtAll
            // 
            this.mnuCaughtAll.Name = "mnuCaughtAll";
            this.mnuCaughtAll.Size = new System.Drawing.Size(149, 22);
            this.mnuCaughtAll.Text = "Caught all";
            this.mnuCaughtAll.Click += new System.EventHandler(this.ModifyAll);
            // 
            // mnuComplete
            // 
            this.mnuComplete.Name = "mnuComplete";
            this.mnuComplete.Size = new System.Drawing.Size(149, 22);
            this.mnuComplete.Text = "Complete Dex";
            this.mnuComplete.Click += new System.EventHandler(this.ModifyAll);
            // 
            // CLB_FormRegular
            // 
            this.CLB_FormRegular.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.CLB_FormRegular.FormattingEnabled = true;
            this.CLB_FormRegular.Location = new System.Drawing.Point(270, 64);
            this.CLB_FormRegular.Name = "CLB_FormRegular";
            this.CLB_FormRegular.Size = new System.Drawing.Size(119, 229);
            this.CLB_FormRegular.TabIndex = 34;
            // 
            // L_FormsSeen
            // 
            this.L_FormsSeen.Location = new System.Drawing.Point(267, 43);
            this.L_FormsSeen.Name = "L_FormsSeen";
            this.L_FormsSeen.Size = new System.Drawing.Size(104, 20);
            this.L_FormsSeen.TabIndex = 35;
            this.L_FormsSeen.Text = "Forms:";
            this.L_FormsSeen.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CLB_FormShiny
            // 
            this.CLB_FormShiny.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.CLB_FormShiny.FormattingEnabled = true;
            this.CLB_FormShiny.Location = new System.Drawing.Point(395, 64);
            this.CLB_FormShiny.Name = "CLB_FormShiny";
            this.CLB_FormShiny.Size = new System.Drawing.Size(119, 229);
            this.CLB_FormShiny.TabIndex = 36;
            // 
            // L_FormDisplayed
            // 
            this.L_FormDisplayed.Location = new System.Drawing.Point(392, 43);
            this.L_FormDisplayed.Name = "L_FormDisplayed";
            this.L_FormDisplayed.Size = new System.Drawing.Size(104, 20);
            this.L_FormDisplayed.TabIndex = 37;
            this.L_FormDisplayed.Text = "Shiny Forms:";
            this.L_FormDisplayed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // modifyMenuForms
            // 
            this.modifyMenuForms.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFormNone,
            this.mnuFormAllRegular,
            this.mnuFormAllShinies});
            this.modifyMenuForms.Name = "modifyMenu";
            this.modifyMenuForms.Size = new System.Drawing.Size(154, 70);
            // 
            // mnuFormNone
            // 
            this.mnuFormNone.Name = "mnuFormNone";
            this.mnuFormNone.Size = new System.Drawing.Size(153, 22);
            this.mnuFormNone.Text = "Seen none";
            this.mnuFormNone.Click += new System.EventHandler(this.ModifyAllForms);
            // 
            // mnuFormAllRegular
            // 
            this.mnuFormAllRegular.Name = "mnuFormAllRegular";
            this.mnuFormAllRegular.Size = new System.Drawing.Size(153, 22);
            this.mnuFormAllRegular.Text = "Seen all";
            this.mnuFormAllRegular.Click += new System.EventHandler(this.ModifyAllForms);
            // 
            // mnuFormAllShinies
            // 
            this.mnuFormAllShinies.Name = "mnuFormAllShinies";
            this.mnuFormAllShinies.Size = new System.Drawing.Size(153, 22);
            this.mnuFormAllShinies.Text = "Seen all shinies";
            this.mnuFormAllShinies.Click += new System.EventHandler(this.ModifyAllForms);
            // 
            // CB_State
            // 
            this.CB_State.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.CB_State.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_State.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_State.DropDownWidth = 95;
            this.CB_State.FormattingEnabled = true;
            this.CB_State.Items.AddRange(new object[] {
            "None",
            "Heard Of",
            "Seen",
            "Captured"});
            this.CB_State.Location = new System.Drawing.Point(148, 46);
            this.CB_State.Name = "CB_State";
            this.CB_State.Size = new System.Drawing.Size(116, 21);
            this.CB_State.TabIndex = 39;
            // 
            // B_ModifyForms
            // 
            this.B_ModifyForms.Location = new System.Drawing.Point(454, 11);
            this.B_ModifyForms.Name = "B_ModifyForms";
            this.B_ModifyForms.Size = new System.Drawing.Size(60, 23);
            this.B_ModifyForms.TabIndex = 40;
            this.B_ModifyForms.Text = "Modify...";
            this.B_ModifyForms.UseVisualStyleBackColor = true;
            this.B_ModifyForms.Click += new System.EventHandler(this.B_ModifyForms_Click);
            // 
            // CHK_National
            // 
            this.CHK_National.AutoSize = true;
            this.CHK_National.Location = new System.Drawing.Point(155, 303);
            this.CHK_National.Name = "CHK_National";
            this.CHK_National.Size = new System.Drawing.Size(112, 17);
            this.CHK_National.TabIndex = 42;
            this.CHK_National.Text = "National PokéDex";
            this.CHK_National.UseVisualStyleBackColor = true;
            // 
            // SAV_PokedexBDSP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(524, 327);
            this.Controls.Add(this.CHK_National);
            this.Controls.Add(this.B_ModifyForms);
            this.Controls.Add(this.CB_State);
            this.Controls.Add(this.L_FormDisplayed);
            this.Controls.Add(this.CLB_FormShiny);
            this.Controls.Add(this.L_FormsSeen);
            this.Controls.Add(this.CLB_FormRegular);
            this.Controls.Add(this.GB_Encountered);
            this.Controls.Add(this.GB_Language);
            this.Controls.Add(this.B_Modify);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_GiveAll);
            this.Controls.Add(this.CB_Species);
            this.Controls.Add(this.L_goto);
            this.Controls.Add(this.LB_Species);
            this.Controls.Add(this.B_Cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SAV_PokedexBDSP";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Pokédex Editor";
            this.GB_Language.ResumeLayout(false);
            this.GB_Language.PerformLayout();
            this.GB_Encountered.ResumeLayout(false);
            this.GB_Encountered.PerformLayout();
            this.modifyMenu.ResumeLayout(false);
            this.modifyMenuForms.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.ListBox LB_Species;
        private System.Windows.Forms.CheckBox CHK_LangKOR;
        private System.Windows.Forms.CheckBox CHK_LangSPA;
        private System.Windows.Forms.CheckBox CHK_LangGER;
        private System.Windows.Forms.CheckBox CHK_LangITA;
        private System.Windows.Forms.CheckBox CHK_LangFRE;
        private System.Windows.Forms.CheckBox CHK_LangENG;
        private System.Windows.Forms.CheckBox CHK_LangJPN;
        private System.Windows.Forms.Label L_goto;
        private System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.Button B_GiveAll;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Modify;
        private System.Windows.Forms.GroupBox GB_Language;
        private System.Windows.Forms.GroupBox GB_Encountered;
        private System.Windows.Forms.CheckBox CHK_FS;
        private System.Windows.Forms.CheckBox CHK_MS;
        private System.Windows.Forms.CheckBox CHK_F;
        private System.Windows.Forms.CheckBox CHK_M;
        private System.Windows.Forms.ContextMenuStrip modifyMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuSeenNone;
        private System.Windows.Forms.ToolStripMenuItem mnuSeenAll;
        private System.Windows.Forms.ToolStripMenuItem mnuCaughtNone;
        private System.Windows.Forms.ToolStripMenuItem mnuCaughtAll;
        private System.Windows.Forms.ToolStripMenuItem mnuComplete;
        private System.Windows.Forms.Label L_FormsSeen;
        private System.Windows.Forms.CheckedListBox CLB_FormRegular;
        private System.Windows.Forms.CheckedListBox CLB_FormShiny;
        private System.Windows.Forms.Label L_FormDisplayed;
        private System.Windows.Forms.ContextMenuStrip modifyMenuForms;
        private System.Windows.Forms.ToolStripMenuItem mnuFormNone;
        private System.Windows.Forms.ToolStripMenuItem mnuFormAllShinies;
        private System.Windows.Forms.ToolStripMenuItem mnuFormAllRegular;
        private System.Windows.Forms.CheckBox CHK_LangCHT;
        private System.Windows.Forms.CheckBox CHK_LangCHS;
        private System.Windows.Forms.ComboBox CB_State;
        private System.Windows.Forms.Button B_ModifyForms;
        private System.Windows.Forms.CheckBox CHK_National;
    }
}
