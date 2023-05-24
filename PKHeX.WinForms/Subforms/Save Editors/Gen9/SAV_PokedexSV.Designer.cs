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
            components = new System.ComponentModel.Container();
            B_Cancel = new System.Windows.Forms.Button();
            LB_Species = new System.Windows.Forms.ListBox();
            L_goto = new System.Windows.Forms.Label();
            CB_Species = new System.Windows.Forms.ComboBox();
            B_GiveAll = new System.Windows.Forms.Button();
            B_Save = new System.Windows.Forms.Button();
            B_Modify = new System.Windows.Forms.Button();
            modifyMenu = new System.Windows.Forms.ContextMenuStrip(components);
            mnuSeenNone = new System.Windows.Forms.ToolStripMenuItem();
            mnuSeenAll = new System.Windows.Forms.ToolStripMenuItem();
            mnuCaughtNone = new System.Windows.Forms.ToolStripMenuItem();
            mnuCaughtAll = new System.Windows.Forms.ToolStripMenuItem();
            mnuComplete = new System.Windows.Forms.ToolStripMenuItem();
            mnuFormNone = new System.Windows.Forms.ToolStripMenuItem();
            mnuForm1 = new System.Windows.Forms.ToolStripMenuItem();
            mnuFormAll = new System.Windows.Forms.ToolStripMenuItem();
            CB_DisplayForm = new System.Windows.Forms.ComboBox();
            GB_Displayed = new System.Windows.Forms.GroupBox();
            L_DisplayedForm = new System.Windows.Forms.Label();
            CB_Gender = new System.Windows.Forms.ComboBox();
            CHK_G = new System.Windows.Forms.CheckBox();
            CHK_DisplayShiny = new System.Windows.Forms.CheckBox();
            CB_State = new System.Windows.Forms.ComboBox();
            GB_Language = new System.Windows.Forms.GroupBox();
            CHK_LangCHT = new System.Windows.Forms.CheckBox();
            CHK_LangCHS = new System.Windows.Forms.CheckBox();
            CHK_LangKOR = new System.Windows.Forms.CheckBox();
            CHK_LangSPA = new System.Windows.Forms.CheckBox();
            CHK_LangGER = new System.Windows.Forms.CheckBox();
            CHK_LangITA = new System.Windows.Forms.CheckBox();
            CHK_LangFRE = new System.Windows.Forms.CheckBox();
            CHK_LangENG = new System.Windows.Forms.CheckBox();
            CHK_LangJPN = new System.Windows.Forms.CheckBox();
            CLB_FormSeen = new System.Windows.Forms.CheckedListBox();
            CHK_SeenShiny = new System.Windows.Forms.CheckBox();
            groupBox1 = new System.Windows.Forms.GroupBox();
            CHK_SeenGenderless = new System.Windows.Forms.CheckBox();
            CHK_SeenFemale = new System.Windows.Forms.CheckBox();
            CHK_SeenMale = new System.Windows.Forms.CheckBox();
            CHK_IsNew = new System.Windows.Forms.CheckBox();
            modifyMenu.SuspendLayout();
            GB_Displayed.SuspendLayout();
            GB_Language.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(500, 339);
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
            // B_GiveAll
            // 
            B_GiveAll.Location = new System.Drawing.Point(208, 13);
            B_GiveAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_GiveAll.Name = "B_GiveAll";
            B_GiveAll.Size = new System.Drawing.Size(70, 27);
            B_GiveAll.TabIndex = 23;
            B_GiveAll.Text = "Check All";
            B_GiveAll.UseVisualStyleBackColor = true;
            B_GiveAll.Click += B_GiveAll_Click;
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(500, 306);
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
            B_Modify.Location = new System.Drawing.Point(500, 13);
            B_Modify.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Modify.Name = "B_Modify";
            B_Modify.Size = new System.Drawing.Size(93, 27);
            B_Modify.TabIndex = 25;
            B_Modify.Text = "Modify All...";
            B_Modify.UseVisualStyleBackColor = true;
            B_Modify.Click += B_Modify_Click;
            // 
            // modifyMenu
            // 
            modifyMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            modifyMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuSeenNone, mnuSeenAll, mnuCaughtNone, mnuCaughtAll, mnuComplete });
            modifyMenu.Name = "modifyMenu";
            modifyMenu.Size = new System.Drawing.Size(150, 114);
            // 
            // mnuSeenNone
            // 
            mnuSeenNone.Name = "mnuSeenNone";
            mnuSeenNone.Size = new System.Drawing.Size(149, 22);
            mnuSeenNone.Text = "Seen none";
            mnuSeenNone.Click += SeenNone;
            // 
            // mnuSeenAll
            // 
            mnuSeenAll.Name = "mnuSeenAll";
            mnuSeenAll.Size = new System.Drawing.Size(149, 22);
            mnuSeenAll.Text = "Seen all";
            mnuSeenAll.Click += SeenAll;
            // 
            // mnuCaughtNone
            // 
            mnuCaughtNone.Name = "mnuCaughtNone";
            mnuCaughtNone.Size = new System.Drawing.Size(149, 22);
            mnuCaughtNone.Text = "Caught none";
            mnuCaughtNone.Click += CaughtNone;
            // 
            // mnuCaughtAll
            // 
            mnuCaughtAll.Name = "mnuCaughtAll";
            mnuCaughtAll.Size = new System.Drawing.Size(149, 22);
            mnuCaughtAll.Text = "Caught all";
            mnuCaughtAll.Click += CaughtAll;
            // 
            // mnuComplete
            // 
            mnuComplete.Name = "mnuComplete";
            mnuComplete.Size = new System.Drawing.Size(149, 22);
            mnuComplete.Text = "Complete Dex";
            mnuComplete.Click += CompleteDex;
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
            // CB_DisplayForm
            // 
            CB_DisplayForm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_DisplayForm.FormattingEnabled = true;
            CB_DisplayForm.Items.AddRange(new object[] { "♂", "♀", "-" });
            CB_DisplayForm.Location = new System.Drawing.Point(7, 115);
            CB_DisplayForm.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_DisplayForm.Name = "CB_DisplayForm";
            CB_DisplayForm.Size = new System.Drawing.Size(119, 23);
            CB_DisplayForm.TabIndex = 45;
            // 
            // GB_Displayed
            // 
            GB_Displayed.Controls.Add(L_DisplayedForm);
            GB_Displayed.Controls.Add(CB_Gender);
            GB_Displayed.Controls.Add(CHK_G);
            GB_Displayed.Controls.Add(CHK_DisplayShiny);
            GB_Displayed.Controls.Add(CB_DisplayForm);
            GB_Displayed.Location = new System.Drawing.Point(358, 46);
            GB_Displayed.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Displayed.Name = "GB_Displayed";
            GB_Displayed.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Displayed.Size = new System.Drawing.Size(134, 148);
            GB_Displayed.TabIndex = 44;
            GB_Displayed.TabStop = false;
            GB_Displayed.Text = "Displayed";
            // 
            // L_DisplayedForm
            // 
            L_DisplayedForm.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            L_DisplayedForm.AutoSize = true;
            L_DisplayedForm.Location = new System.Drawing.Point(4, 97);
            L_DisplayedForm.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_DisplayedForm.Name = "L_DisplayedForm";
            L_DisplayedForm.Size = new System.Drawing.Size(92, 15);
            L_DisplayedForm.TabIndex = 50;
            L_DisplayedForm.Text = "Displayed Form:";
            // 
            // CB_Gender
            // 
            CB_Gender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Gender.FormattingEnabled = true;
            CB_Gender.Items.AddRange(new object[] { "♂", "♀", "-" });
            CB_Gender.Location = new System.Drawing.Point(6, 22);
            CB_Gender.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Gender.Name = "CB_Gender";
            CB_Gender.Size = new System.Drawing.Size(46, 23);
            CB_Gender.TabIndex = 24;
            // 
            // CHK_G
            // 
            CHK_G.AutoSize = true;
            CHK_G.Location = new System.Drawing.Point(6, 62);
            CHK_G.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_G.Name = "CHK_G";
            CHK_G.Size = new System.Drawing.Size(113, 19);
            CHK_G.TabIndex = 10;
            CHK_G.Text = "Gender Different";
            CHK_G.UseVisualStyleBackColor = true;
            // 
            // CHK_DisplayShiny
            // 
            CHK_DisplayShiny.AutoSize = true;
            CHK_DisplayShiny.Location = new System.Drawing.Point(56, 24);
            CHK_DisplayShiny.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_DisplayShiny.Name = "CHK_DisplayShiny";
            CHK_DisplayShiny.Size = new System.Drawing.Size(55, 19);
            CHK_DisplayShiny.TabIndex = 9;
            CHK_DisplayShiny.Text = "Shiny";
            CHK_DisplayShiny.UseVisualStyleBackColor = true;
            // 
            // CB_State
            // 
            CB_State.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            CB_State.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_State.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_State.DropDownWidth = 95;
            CB_State.FormattingEnabled = true;
            CB_State.Items.AddRange(new object[] { "None", "Heard Of", "Seen", "Captured" });
            CB_State.Location = new System.Drawing.Point(285, 14);
            CB_State.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_State.Name = "CB_State";
            CB_State.Size = new System.Drawing.Size(125, 23);
            CB_State.TabIndex = 46;
            // 
            // GB_Language
            // 
            GB_Language.Controls.Add(CHK_LangCHT);
            GB_Language.Controls.Add(CHK_LangCHS);
            GB_Language.Controls.Add(CHK_LangKOR);
            GB_Language.Controls.Add(CHK_LangSPA);
            GB_Language.Controls.Add(CHK_LangGER);
            GB_Language.Controls.Add(CHK_LangITA);
            GB_Language.Controls.Add(CHK_LangFRE);
            GB_Language.Controls.Add(CHK_LangENG);
            GB_Language.Controls.Add(CHK_LangJPN);
            GB_Language.Location = new System.Drawing.Point(358, 200);
            GB_Language.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Language.Name = "GB_Language";
            GB_Language.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Language.Size = new System.Drawing.Size(134, 167);
            GB_Language.TabIndex = 47;
            GB_Language.TabStop = false;
            GB_Language.Text = "Languages";
            // 
            // CHK_LangCHT
            // 
            CHK_LangCHT.AutoSize = true;
            CHK_LangCHT.Location = new System.Drawing.Point(7, 145);
            CHK_LangCHT.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_LangCHT.Name = "CHK_LangCHT";
            CHK_LangCHT.Size = new System.Drawing.Size(74, 19);
            CHK_LangCHT.TabIndex = 21;
            CHK_LangCHT.Text = "ChineseT";
            CHK_LangCHT.UseVisualStyleBackColor = true;
            // 
            // CHK_LangCHS
            // 
            CHK_LangCHS.AutoSize = true;
            CHK_LangCHS.Location = new System.Drawing.Point(7, 129);
            CHK_LangCHS.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_LangCHS.Name = "CHK_LangCHS";
            CHK_LangCHS.Size = new System.Drawing.Size(74, 19);
            CHK_LangCHS.TabIndex = 20;
            CHK_LangCHS.Text = "ChineseS";
            CHK_LangCHS.UseVisualStyleBackColor = true;
            // 
            // CHK_LangKOR
            // 
            CHK_LangKOR.AutoSize = true;
            CHK_LangKOR.Location = new System.Drawing.Point(7, 113);
            CHK_LangKOR.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_LangKOR.Name = "CHK_LangKOR";
            CHK_LangKOR.Size = new System.Drawing.Size(63, 19);
            CHK_LangKOR.TabIndex = 19;
            CHK_LangKOR.Text = "Korean";
            CHK_LangKOR.UseVisualStyleBackColor = true;
            // 
            // CHK_LangSPA
            // 
            CHK_LangSPA.AutoSize = true;
            CHK_LangSPA.Location = new System.Drawing.Point(7, 97);
            CHK_LangSPA.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_LangSPA.Name = "CHK_LangSPA";
            CHK_LangSPA.Size = new System.Drawing.Size(67, 19);
            CHK_LangSPA.TabIndex = 18;
            CHK_LangSPA.Text = "Spanish";
            CHK_LangSPA.UseVisualStyleBackColor = true;
            // 
            // CHK_LangGER
            // 
            CHK_LangGER.AutoSize = true;
            CHK_LangGER.Location = new System.Drawing.Point(7, 81);
            CHK_LangGER.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_LangGER.Name = "CHK_LangGER";
            CHK_LangGER.Size = new System.Drawing.Size(68, 19);
            CHK_LangGER.TabIndex = 17;
            CHK_LangGER.Text = "German";
            CHK_LangGER.UseVisualStyleBackColor = true;
            // 
            // CHK_LangITA
            // 
            CHK_LangITA.AutoSize = true;
            CHK_LangITA.Location = new System.Drawing.Point(7, 65);
            CHK_LangITA.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_LangITA.Name = "CHK_LangITA";
            CHK_LangITA.Size = new System.Drawing.Size(58, 19);
            CHK_LangITA.TabIndex = 16;
            CHK_LangITA.Text = "Italian";
            CHK_LangITA.UseVisualStyleBackColor = true;
            // 
            // CHK_LangFRE
            // 
            CHK_LangFRE.AutoSize = true;
            CHK_LangFRE.Location = new System.Drawing.Point(7, 48);
            CHK_LangFRE.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_LangFRE.Name = "CHK_LangFRE";
            CHK_LangFRE.Size = new System.Drawing.Size(62, 19);
            CHK_LangFRE.TabIndex = 15;
            CHK_LangFRE.Text = "French";
            CHK_LangFRE.UseVisualStyleBackColor = true;
            // 
            // CHK_LangENG
            // 
            CHK_LangENG.AutoSize = true;
            CHK_LangENG.Location = new System.Drawing.Point(7, 32);
            CHK_LangENG.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_LangENG.Name = "CHK_LangENG";
            CHK_LangENG.Size = new System.Drawing.Size(64, 19);
            CHK_LangENG.TabIndex = 14;
            CHK_LangENG.Text = "English";
            CHK_LangENG.UseVisualStyleBackColor = true;
            // 
            // CHK_LangJPN
            // 
            CHK_LangJPN.AutoSize = true;
            CHK_LangJPN.Location = new System.Drawing.Point(7, 16);
            CHK_LangJPN.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_LangJPN.Name = "CHK_LangJPN";
            CHK_LangJPN.Size = new System.Drawing.Size(73, 19);
            CHK_LangJPN.TabIndex = 13;
            CHK_LangJPN.Text = "Japanese";
            CHK_LangJPN.UseVisualStyleBackColor = true;
            // 
            // CLB_FormSeen
            // 
            CLB_FormSeen.FormattingEnabled = true;
            CLB_FormSeen.Location = new System.Drawing.Point(210, 153);
            CLB_FormSeen.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CLB_FormSeen.Name = "CLB_FormSeen";
            CLB_FormSeen.Size = new System.Drawing.Size(138, 202);
            CLB_FormSeen.TabIndex = 48;
            // 
            // CHK_SeenShiny
            // 
            CHK_SeenShiny.AutoSize = true;
            CHK_SeenShiny.Location = new System.Drawing.Point(6, 75);
            CHK_SeenShiny.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_SeenShiny.Name = "CHK_SeenShiny";
            CHK_SeenShiny.Size = new System.Drawing.Size(55, 19);
            CHK_SeenShiny.TabIndex = 25;
            CHK_SeenShiny.Text = "Shiny";
            CHK_SeenShiny.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(CHK_SeenGenderless);
            groupBox1.Controls.Add(CHK_SeenShiny);
            groupBox1.Controls.Add(CHK_SeenFemale);
            groupBox1.Controls.Add(CHK_SeenMale);
            groupBox1.Location = new System.Drawing.Point(208, 46);
            groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Size = new System.Drawing.Size(141, 100);
            groupBox1.TabIndex = 49;
            groupBox1.TabStop = false;
            groupBox1.Text = "Seen";
            // 
            // CHK_SeenGenderless
            // 
            CHK_SeenGenderless.AutoSize = true;
            CHK_SeenGenderless.Location = new System.Drawing.Point(6, 48);
            CHK_SeenGenderless.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_SeenGenderless.Name = "CHK_SeenGenderless";
            CHK_SeenGenderless.Size = new System.Drawing.Size(83, 19);
            CHK_SeenGenderless.TabIndex = 12;
            CHK_SeenGenderless.Text = "Genderless";
            CHK_SeenGenderless.UseVisualStyleBackColor = true;
            // 
            // CHK_SeenFemale
            // 
            CHK_SeenFemale.AutoSize = true;
            CHK_SeenFemale.Location = new System.Drawing.Point(6, 32);
            CHK_SeenFemale.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_SeenFemale.Name = "CHK_SeenFemale";
            CHK_SeenFemale.Size = new System.Drawing.Size(64, 19);
            CHK_SeenFemale.TabIndex = 9;
            CHK_SeenFemale.Text = "Female";
            CHK_SeenFemale.UseVisualStyleBackColor = true;
            // 
            // CHK_SeenMale
            // 
            CHK_SeenMale.AutoSize = true;
            CHK_SeenMale.Location = new System.Drawing.Point(6, 16);
            CHK_SeenMale.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_SeenMale.Name = "CHK_SeenMale";
            CHK_SeenMale.Size = new System.Drawing.Size(52, 19);
            CHK_SeenMale.TabIndex = 11;
            CHK_SeenMale.Text = "Male";
            CHK_SeenMale.UseVisualStyleBackColor = true;
            // 
            // CHK_IsNew
            // 
            CHK_IsNew.AutoSize = true;
            CHK_IsNew.Location = new System.Drawing.Point(414, 17);
            CHK_IsNew.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_IsNew.Name = "CHK_IsNew";
            CHK_IsNew.Size = new System.Drawing.Size(50, 19);
            CHK_IsNew.TabIndex = 46;
            CHK_IsNew.Text = "New";
            CHK_IsNew.UseVisualStyleBackColor = true;
            // 
            // SAV_PokedexSV
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(608, 377);
            Controls.Add(CHK_IsNew);
            Controls.Add(groupBox1);
            Controls.Add(CLB_FormSeen);
            Controls.Add(GB_Language);
            Controls.Add(CB_State);
            Controls.Add(GB_Displayed);
            Controls.Add(B_Modify);
            Controls.Add(B_Save);
            Controls.Add(B_GiveAll);
            Controls.Add(CB_Species);
            Controls.Add(L_goto);
            Controls.Add(LB_Species);
            Controls.Add(B_Cancel);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_PokedexSV";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Pokédex Editor";
            modifyMenu.ResumeLayout(false);
            GB_Displayed.ResumeLayout(false);
            GB_Displayed.PerformLayout();
            GB_Language.ResumeLayout(false);
            GB_Language.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
