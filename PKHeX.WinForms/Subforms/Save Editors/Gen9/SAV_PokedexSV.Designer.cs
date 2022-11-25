namespace PKHeX.WinForms
{
    partial class SAV_PokedexSV
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
            this.L_goto = new System.Windows.Forms.Label();
            this.CB_Species = new System.Windows.Forms.ComboBox();
            this.B_GiveAll = new System.Windows.Forms.Button();
            this.B_Save = new System.Windows.Forms.Button();
            this.B_Modify = new System.Windows.Forms.Button();
            this.modifyMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuSeenNone = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSeenAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCaughtNone = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCaughtAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuComplete = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFormNone = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuForm1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFormAll = new System.Windows.Forms.ToolStripMenuItem();
            this.CB_DisplayForm = new System.Windows.Forms.ComboBox();
            this.GB_Displayed = new System.Windows.Forms.GroupBox();
            this.CB_Gender = new System.Windows.Forms.ComboBox();
            this.CHK_G = new System.Windows.Forms.CheckBox();
            this.CHK_DisplayShiny = new System.Windows.Forms.CheckBox();
            this.CB_State = new System.Windows.Forms.ComboBox();
            this.GB_Language = new System.Windows.Forms.GroupBox();
            this.CHK_LangCHT = new System.Windows.Forms.CheckBox();
            this.CHK_LangCHS = new System.Windows.Forms.CheckBox();
            this.CHK_LangKOR = new System.Windows.Forms.CheckBox();
            this.CHK_LangSPA = new System.Windows.Forms.CheckBox();
            this.CHK_LangGER = new System.Windows.Forms.CheckBox();
            this.CHK_LangITA = new System.Windows.Forms.CheckBox();
            this.CHK_LangFRE = new System.Windows.Forms.CheckBox();
            this.CHK_LangENG = new System.Windows.Forms.CheckBox();
            this.CHK_LangJPN = new System.Windows.Forms.CheckBox();
            this.CLB_FormSeen = new System.Windows.Forms.CheckedListBox();
            this.CHK_SeenShiny = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CHK_SeenFemale = new System.Windows.Forms.CheckBox();
            this.CHK_SeenMale = new System.Windows.Forms.CheckBox();
            this.CHK_SeenGenderless = new System.Windows.Forms.CheckBox();
            this.CHK_IsNew = new System.Windows.Forms.CheckBox();
            this.L_DisplayedForm = new System.Windows.Forms.Label();
            this.modifyMenu.SuspendLayout();
            this.GB_Displayed.SuspendLayout();
            this.GB_Language.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(429, 294);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(80, 23);
            this.B_Cancel.TabIndex = 0;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // LB_Species
            // 
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
            // B_GiveAll
            // 
            this.B_GiveAll.Location = new System.Drawing.Point(178, 11);
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
            this.B_Save.Location = new System.Drawing.Point(429, 265);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(80, 23);
            this.B_Save.TabIndex = 24;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // B_Modify
            // 
            this.B_Modify.Location = new System.Drawing.Point(429, 11);
            this.B_Modify.Name = "B_Modify";
            this.B_Modify.Size = new System.Drawing.Size(80, 23);
            this.B_Modify.TabIndex = 25;
            this.B_Modify.Text = "Modify All...";
            this.B_Modify.UseVisualStyleBackColor = true;
            this.B_Modify.Click += new System.EventHandler(this.B_Modify_Click);
            // 
            // modifyMenu
            // 
            this.modifyMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
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
            this.mnuSeenNone.Size = new System.Drawing.Size(201, 22);
            this.mnuSeenNone.Text = "Seen none";
            this.mnuSeenNone.Click += new System.EventHandler(this.SeenNone);
            // 
            // mnuSeenAll
            // 
            this.mnuSeenAll.Name = "mnuSeenAll";
            this.mnuSeenAll.Size = new System.Drawing.Size(201, 22);
            this.mnuSeenAll.Text = "Seen all";
            this.mnuSeenAll.Click += new System.EventHandler(this.SeenAll);
            // 
            // mnuCaughtNone
            // 
            this.mnuCaughtNone.Name = "mnuCaughtNone";
            this.mnuCaughtNone.Size = new System.Drawing.Size(201, 22);
            this.mnuCaughtNone.Text = "Caught none";
            this.mnuCaughtNone.Click += new System.EventHandler(this.CaughtNone);
            // 
            // mnuCaughtAll
            // 
            this.mnuCaughtAll.Name = "mnuCaughtAll";
            this.mnuCaughtAll.Size = new System.Drawing.Size(201, 22);
            this.mnuCaughtAll.Text = "Caught all";
            this.mnuCaughtAll.Click += new System.EventHandler(this.CaughtAll);
            // 
            // mnuComplete
            // 
            this.mnuComplete.Name = "mnuComplete";
            this.mnuComplete.Size = new System.Drawing.Size(201, 22);
            this.mnuComplete.Text = "Complete Dex";
            this.mnuComplete.Click += new System.EventHandler(this.CompleteDex);
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
            // CB_DisplayForm
            // 
            this.CB_DisplayForm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_DisplayForm.FormattingEnabled = true;
            this.CB_DisplayForm.Items.AddRange(new object[] {
            "♂",
            "♀",
            "-"});
            this.CB_DisplayForm.Location = new System.Drawing.Point(6, 100);
            this.CB_DisplayForm.Name = "CB_DisplayForm";
            this.CB_DisplayForm.Size = new System.Drawing.Size(103, 21);
            this.CB_DisplayForm.TabIndex = 45;
            // 
            // GB_Displayed
            // 
            this.GB_Displayed.Controls.Add(this.L_DisplayedForm);
            this.GB_Displayed.Controls.Add(this.CB_Gender);
            this.GB_Displayed.Controls.Add(this.CHK_G);
            this.GB_Displayed.Controls.Add(this.CHK_DisplayShiny);
            this.GB_Displayed.Controls.Add(this.CB_DisplayForm);
            this.GB_Displayed.Location = new System.Drawing.Point(307, 40);
            this.GB_Displayed.Name = "GB_Displayed";
            this.GB_Displayed.Size = new System.Drawing.Size(115, 128);
            this.GB_Displayed.TabIndex = 44;
            this.GB_Displayed.TabStop = false;
            this.GB_Displayed.Text = "Displayed";
            // 
            // CB_Gender
            // 
            this.CB_Gender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Gender.FormattingEnabled = true;
            this.CB_Gender.Items.AddRange(new object[] {
            "♂",
            "♀",
            "-"});
            this.CB_Gender.Location = new System.Drawing.Point(5, 19);
            this.CB_Gender.Name = "CB_Gender";
            this.CB_Gender.Size = new System.Drawing.Size(40, 21);
            this.CB_Gender.TabIndex = 24;
            // 
            // CHK_G
            // 
            this.CHK_G.AutoSize = true;
            this.CHK_G.Location = new System.Drawing.Point(5, 54);
            this.CHK_G.Name = "CHK_G";
            this.CHK_G.Size = new System.Drawing.Size(104, 17);
            this.CHK_G.TabIndex = 10;
            this.CHK_G.Text = "Gender Different";
            this.CHK_G.UseVisualStyleBackColor = true;
            // 
            // CHK_DisplayShiny
            // 
            this.CHK_DisplayShiny.AutoSize = true;
            this.CHK_DisplayShiny.Location = new System.Drawing.Point(48, 21);
            this.CHK_DisplayShiny.Name = "CHK_DisplayShiny";
            this.CHK_DisplayShiny.Size = new System.Drawing.Size(52, 17);
            this.CHK_DisplayShiny.TabIndex = 9;
            this.CHK_DisplayShiny.Text = "Shiny";
            this.CHK_DisplayShiny.UseVisualStyleBackColor = true;
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
            this.CB_State.Location = new System.Drawing.Point(244, 12);
            this.CB_State.Name = "CB_State";
            this.CB_State.Size = new System.Drawing.Size(108, 21);
            this.CB_State.TabIndex = 46;
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
            this.GB_Language.Location = new System.Drawing.Point(307, 173);
            this.GB_Language.Name = "GB_Language";
            this.GB_Language.Size = new System.Drawing.Size(115, 145);
            this.GB_Language.TabIndex = 47;
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
            // CLB_FormSeen
            // 
            this.CLB_FormSeen.FormattingEnabled = true;
            this.CLB_FormSeen.Location = new System.Drawing.Point(180, 133);
            this.CLB_FormSeen.Name = "CLB_FormSeen";
            this.CLB_FormSeen.Size = new System.Drawing.Size(119, 184);
            this.CLB_FormSeen.TabIndex = 48;
            // 
            // CHK_SeenShiny
            // 
            this.CHK_SeenShiny.AutoSize = true;
            this.CHK_SeenShiny.Location = new System.Drawing.Point(5, 65);
            this.CHK_SeenShiny.Name = "CHK_SeenShiny";
            this.CHK_SeenShiny.Size = new System.Drawing.Size(52, 17);
            this.CHK_SeenShiny.TabIndex = 25;
            this.CHK_SeenShiny.Text = "Shiny";
            this.CHK_SeenShiny.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CHK_SeenGenderless);
            this.groupBox1.Controls.Add(this.CHK_SeenShiny);
            this.groupBox1.Controls.Add(this.CHK_SeenFemale);
            this.groupBox1.Controls.Add(this.CHK_SeenMale);
            this.groupBox1.Location = new System.Drawing.Point(178, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(121, 87);
            this.groupBox1.TabIndex = 49;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Seen";
            // 
            // CHK_SeenFemale
            // 
            this.CHK_SeenFemale.AutoSize = true;
            this.CHK_SeenFemale.Location = new System.Drawing.Point(5, 28);
            this.CHK_SeenFemale.Name = "CHK_SeenFemale";
            this.CHK_SeenFemale.Size = new System.Drawing.Size(60, 17);
            this.CHK_SeenFemale.TabIndex = 9;
            this.CHK_SeenFemale.Text = "Female";
            this.CHK_SeenFemale.UseVisualStyleBackColor = true;
            // 
            // CHK_SeenMale
            // 
            this.CHK_SeenMale.AutoSize = true;
            this.CHK_SeenMale.Location = new System.Drawing.Point(5, 14);
            this.CHK_SeenMale.Name = "CHK_SeenMale";
            this.CHK_SeenMale.Size = new System.Drawing.Size(49, 17);
            this.CHK_SeenMale.TabIndex = 11;
            this.CHK_SeenMale.Text = "Male";
            this.CHK_SeenMale.UseVisualStyleBackColor = true;
            // 
            // CHK_SeenGenderless
            // 
            this.CHK_SeenGenderless.AutoSize = true;
            this.CHK_SeenGenderless.Location = new System.Drawing.Point(5, 42);
            this.CHK_SeenGenderless.Name = "CHK_SeenGenderless";
            this.CHK_SeenGenderless.Size = new System.Drawing.Size(79, 17);
            this.CHK_SeenGenderless.TabIndex = 12;
            this.CHK_SeenGenderless.Text = "Genderless";
            this.CHK_SeenGenderless.UseVisualStyleBackColor = true;
            // 
            // CHK_IsNew
            // 
            this.CHK_IsNew.AutoSize = true;
            this.CHK_IsNew.Location = new System.Drawing.Point(355, 15);
            this.CHK_IsNew.Name = "CHK_IsNew";
            this.CHK_IsNew.Size = new System.Drawing.Size(48, 17);
            this.CHK_IsNew.TabIndex = 46;
            this.CHK_IsNew.Text = "New";
            this.CHK_IsNew.UseVisualStyleBackColor = true;
            // 
            // L_DisplayedForm
            // 
            this.L_DisplayedForm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_DisplayedForm.AutoSize = true;
            this.L_DisplayedForm.Location = new System.Drawing.Point(3, 84);
            this.L_DisplayedForm.Name = "L_DisplayedForm";
            this.L_DisplayedForm.Size = new System.Drawing.Size(82, 13);
            this.L_DisplayedForm.TabIndex = 50;
            this.L_DisplayedForm.Text = "Displayed Form:";
            // 
            // SAV_PokedexSV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(521, 327);
            this.Controls.Add(this.CHK_IsNew);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.CLB_FormSeen);
            this.Controls.Add(this.GB_Language);
            this.Controls.Add(this.CB_State);
            this.Controls.Add(this.GB_Displayed);
            this.Controls.Add(this.B_Modify);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_GiveAll);
            this.Controls.Add(this.CB_Species);
            this.Controls.Add(this.L_goto);
            this.Controls.Add(this.LB_Species);
            this.Controls.Add(this.B_Cancel);
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SAV_PokedexSV";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Pokédex Editor";
            this.modifyMenu.ResumeLayout(false);
            this.GB_Displayed.ResumeLayout(false);
            this.GB_Displayed.PerformLayout();
            this.GB_Language.ResumeLayout(false);
            this.GB_Language.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.ListBox LB_Species;
        private System.Windows.Forms.Label L_goto;
        private System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.Button B_GiveAll;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Modify;
        private System.Windows.Forms.ContextMenuStrip modifyMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuSeenNone;
        private System.Windows.Forms.ToolStripMenuItem mnuSeenAll;
        private System.Windows.Forms.ToolStripMenuItem mnuCaughtNone;
        private System.Windows.Forms.ToolStripMenuItem mnuCaughtAll;
        private System.Windows.Forms.ToolStripMenuItem mnuComplete;
        private System.Windows.Forms.ToolStripMenuItem mnuFormNone;
        private System.Windows.Forms.ToolStripMenuItem mnuForm1;
        private System.Windows.Forms.ToolStripMenuItem mnuFormAll;
        private System.Windows.Forms.ComboBox CB_DisplayForm;
        private System.Windows.Forms.GroupBox GB_Displayed;
        private System.Windows.Forms.CheckBox CHK_DisplayShiny;
        private System.Windows.Forms.ComboBox CB_State;
        private System.Windows.Forms.ComboBox CB_Gender;
        private System.Windows.Forms.CheckBox CHK_G;
        private System.Windows.Forms.GroupBox GB_Language;
        private System.Windows.Forms.CheckBox CHK_LangCHT;
        private System.Windows.Forms.CheckBox CHK_LangCHS;
        private System.Windows.Forms.CheckBox CHK_LangKOR;
        private System.Windows.Forms.CheckBox CHK_LangSPA;
        private System.Windows.Forms.CheckBox CHK_LangGER;
        private System.Windows.Forms.CheckBox CHK_LangITA;
        private System.Windows.Forms.CheckBox CHK_LangFRE;
        private System.Windows.Forms.CheckBox CHK_LangENG;
        private System.Windows.Forms.CheckBox CHK_LangJPN;
        private System.Windows.Forms.CheckedListBox CLB_FormSeen;
        private System.Windows.Forms.CheckBox CHK_SeenShiny;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox CHK_SeenGenderless;
        private System.Windows.Forms.CheckBox CHK_SeenFemale;
        private System.Windows.Forms.CheckBox CHK_SeenMale;
        private System.Windows.Forms.CheckBox CHK_IsNew;
        private System.Windows.Forms.Label L_DisplayedForm;
    }
}
