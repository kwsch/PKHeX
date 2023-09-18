namespace PKHeX.WinForms
{
    partial class SAV_PokedexSVKitakami
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
            CB_PaldeaForm = new System.Windows.Forms.ComboBox();
            GB_Paldea = new System.Windows.Forms.GroupBox();
            CB_PaldeaGender = new System.Windows.Forms.ComboBox();
            CHK_PaldeaShiny = new System.Windows.Forms.CheckBox();
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
            GB_SeenFlags = new System.Windows.Forms.GroupBox();
            CHK_SeenGenderless = new System.Windows.Forms.CheckBox();
            CHK_SeenFemale = new System.Windows.Forms.CheckBox();
            CHK_SeenMale = new System.Windows.Forms.CheckBox();
            GB_Kitakami = new System.Windows.Forms.GroupBox();
            CB_KitakamiGender = new System.Windows.Forms.ComboBox();
            CB_KitakamiForm = new System.Windows.Forms.ComboBox();
            CHK_KitakamiShiny = new System.Windows.Forms.CheckBox();
            GB_Blueberry = new System.Windows.Forms.GroupBox();
            CB_BlueberryGender = new System.Windows.Forms.ComboBox();
            CB_BlueberryForm = new System.Windows.Forms.ComboBox();
            CHK_BlueberryShiny = new System.Windows.Forms.CheckBox();
            CLB_FormObtained = new System.Windows.Forms.CheckedListBox();
            CLB_FormHeard = new System.Windows.Forms.CheckedListBox();
            CLB_FormViewed = new System.Windows.Forms.CheckedListBox();
            L_Seen = new System.Windows.Forms.Label();
            L_Obtained = new System.Windows.Forms.Label();
            L_HeardOf = new System.Windows.Forms.Label();
            L_Viewed = new System.Windows.Forms.Label();
            modifyMenu.SuspendLayout();
            GB_Paldea.SuspendLayout();
            GB_Language.SuspendLayout();
            GB_SeenFlags.SuspendLayout();
            GB_Kitakami.SuspendLayout();
            GB_Blueberry.SuspendLayout();
            SuspendLayout();
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(712, 361);
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
            LB_Species.Location = new System.Drawing.Point(16, 54);
            LB_Species.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LB_Species.Name = "LB_Species";
            LB_Species.Size = new System.Drawing.Size(160, 334);
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
            CB_Species.Size = new System.Drawing.Size(118, 23);
            CB_Species.TabIndex = 21;
            CB_Species.SelectedIndexChanged += ChangeCBSpecies;
            CB_Species.SelectedValueChanged += ChangeCBSpecies;
            // 
            // B_GiveAll
            // 
            B_GiveAll.Location = new System.Drawing.Point(558, 6);
            B_GiveAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_GiveAll.Name = "B_GiveAll";
            B_GiveAll.Size = new System.Drawing.Size(104, 27);
            B_GiveAll.TabIndex = 23;
            B_GiveAll.Text = "Check All";
            B_GiveAll.UseVisualStyleBackColor = true;
            B_GiveAll.Click += B_GiveAll_Click;
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(712, 328);
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
            B_Modify.Location = new System.Drawing.Point(701, 6);
            B_Modify.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Modify.Name = "B_Modify";
            B_Modify.Size = new System.Drawing.Size(104, 27);
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
            // CB_PaldeaForm
            // 
            CB_PaldeaForm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_PaldeaForm.FormattingEnabled = true;
            CB_PaldeaForm.Items.AddRange(new object[] { "♂", "♀", "-" });
            CB_PaldeaForm.Location = new System.Drawing.Point(9, 44);
            CB_PaldeaForm.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_PaldeaForm.Name = "CB_PaldeaForm";
            CB_PaldeaForm.Size = new System.Drawing.Size(120, 23);
            CB_PaldeaForm.TabIndex = 45;
            // 
            // GB_Paldea
            // 
            GB_Paldea.Controls.Add(CB_PaldeaGender);
            GB_Paldea.Controls.Add(CB_PaldeaForm);
            GB_Paldea.Controls.Add(CHK_PaldeaShiny);
            GB_Paldea.Location = new System.Drawing.Point(184, 319);
            GB_Paldea.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Paldea.Name = "GB_Paldea";
            GB_Paldea.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Paldea.Size = new System.Drawing.Size(138, 72);
            GB_Paldea.TabIndex = 44;
            GB_Paldea.TabStop = false;
            GB_Paldea.Text = "Display: Paldea";
            // 
            // CB_PaldeaGender
            // 
            CB_PaldeaGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_PaldeaGender.FormattingEnabled = true;
            CB_PaldeaGender.Items.AddRange(new object[] { "♂", "♀", "-" });
            CB_PaldeaGender.Location = new System.Drawing.Point(8, 16);
            CB_PaldeaGender.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_PaldeaGender.Name = "CB_PaldeaGender";
            CB_PaldeaGender.Size = new System.Drawing.Size(46, 23);
            CB_PaldeaGender.TabIndex = 24;
            // 
            // CHK_PaldeaShiny
            // 
            CHK_PaldeaShiny.AutoSize = true;
            CHK_PaldeaShiny.Location = new System.Drawing.Point(64, 18);
            CHK_PaldeaShiny.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_PaldeaShiny.Name = "CHK_PaldeaShiny";
            CHK_PaldeaShiny.Size = new System.Drawing.Size(55, 19);
            CHK_PaldeaShiny.TabIndex = 9;
            CHK_PaldeaShiny.Text = "Shiny";
            CHK_PaldeaShiny.UseVisualStyleBackColor = true;
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
            GB_Language.Location = new System.Drawing.Point(670, 148);
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
            CLB_FormSeen.Location = new System.Drawing.Point(184, 57);
            CLB_FormSeen.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CLB_FormSeen.Name = "CLB_FormSeen";
            CLB_FormSeen.Size = new System.Drawing.Size(112, 256);
            CLB_FormSeen.TabIndex = 48;
            // 
            // CHK_SeenShiny
            // 
            CHK_SeenShiny.AutoSize = true;
            CHK_SeenShiny.Location = new System.Drawing.Point(6, 69);
            CHK_SeenShiny.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_SeenShiny.Name = "CHK_SeenShiny";
            CHK_SeenShiny.Size = new System.Drawing.Size(55, 19);
            CHK_SeenShiny.TabIndex = 25;
            CHK_SeenShiny.Text = "Shiny";
            CHK_SeenShiny.UseVisualStyleBackColor = true;
            // 
            // GB_SeenFlags
            // 
            GB_SeenFlags.Controls.Add(CHK_SeenGenderless);
            GB_SeenFlags.Controls.Add(CHK_SeenShiny);
            GB_SeenFlags.Controls.Add(CHK_SeenFemale);
            GB_SeenFlags.Controls.Add(CHK_SeenMale);
            GB_SeenFlags.Location = new System.Drawing.Point(670, 54);
            GB_SeenFlags.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_SeenFlags.Name = "GB_SeenFlags";
            GB_SeenFlags.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_SeenFlags.Size = new System.Drawing.Size(134, 92);
            GB_SeenFlags.TabIndex = 49;
            GB_SeenFlags.TabStop = false;
            GB_SeenFlags.Text = "Seen";
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
            // GB_Kitakami
            // 
            GB_Kitakami.Controls.Add(CB_KitakamiGender);
            GB_Kitakami.Controls.Add(CB_KitakamiForm);
            GB_Kitakami.Controls.Add(CHK_KitakamiShiny);
            GB_Kitakami.Location = new System.Drawing.Point(328, 319);
            GB_Kitakami.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Kitakami.Name = "GB_Kitakami";
            GB_Kitakami.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Kitakami.Size = new System.Drawing.Size(138, 72);
            GB_Kitakami.TabIndex = 46;
            GB_Kitakami.TabStop = false;
            GB_Kitakami.Text = "Display: Kitakami";
            // 
            // CB_KitakamiGender
            // 
            CB_KitakamiGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_KitakamiGender.FormattingEnabled = true;
            CB_KitakamiGender.Items.AddRange(new object[] { "♂", "♀", "-" });
            CB_KitakamiGender.Location = new System.Drawing.Point(8, 16);
            CB_KitakamiGender.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_KitakamiGender.Name = "CB_KitakamiGender";
            CB_KitakamiGender.Size = new System.Drawing.Size(46, 23);
            CB_KitakamiGender.TabIndex = 24;
            // 
            // CB_KitakamiForm
            // 
            CB_KitakamiForm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_KitakamiForm.FormattingEnabled = true;
            CB_KitakamiForm.Items.AddRange(new object[] { "♂", "♀", "-" });
            CB_KitakamiForm.Location = new System.Drawing.Point(9, 44);
            CB_KitakamiForm.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_KitakamiForm.Name = "CB_KitakamiForm";
            CB_KitakamiForm.Size = new System.Drawing.Size(120, 23);
            CB_KitakamiForm.TabIndex = 45;
            // 
            // CHK_KitakamiShiny
            // 
            CHK_KitakamiShiny.AutoSize = true;
            CHK_KitakamiShiny.Location = new System.Drawing.Point(64, 18);
            CHK_KitakamiShiny.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_KitakamiShiny.Name = "CHK_KitakamiShiny";
            CHK_KitakamiShiny.Size = new System.Drawing.Size(55, 19);
            CHK_KitakamiShiny.TabIndex = 9;
            CHK_KitakamiShiny.Text = "Shiny";
            CHK_KitakamiShiny.UseVisualStyleBackColor = true;
            // 
            // GB_Blueberry
            // 
            GB_Blueberry.Controls.Add(CB_BlueberryGender);
            GB_Blueberry.Controls.Add(CB_BlueberryForm);
            GB_Blueberry.Controls.Add(CHK_BlueberryShiny);
            GB_Blueberry.Location = new System.Drawing.Point(472, 319);
            GB_Blueberry.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Blueberry.Name = "GB_Blueberry";
            GB_Blueberry.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Blueberry.Size = new System.Drawing.Size(138, 72);
            GB_Blueberry.TabIndex = 47;
            GB_Blueberry.TabStop = false;
            GB_Blueberry.Text = "Display: Blueberry";
            // 
            // CB_BlueberryGender
            // 
            CB_BlueberryGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_BlueberryGender.FormattingEnabled = true;
            CB_BlueberryGender.Items.AddRange(new object[] { "♂", "♀", "-" });
            CB_BlueberryGender.Location = new System.Drawing.Point(8, 16);
            CB_BlueberryGender.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_BlueberryGender.Name = "CB_BlueberryGender";
            CB_BlueberryGender.Size = new System.Drawing.Size(46, 23);
            CB_BlueberryGender.TabIndex = 24;
            // 
            // CB_BlueberryForm
            // 
            CB_BlueberryForm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_BlueberryForm.FormattingEnabled = true;
            CB_BlueberryForm.Items.AddRange(new object[] { "♂", "♀", "-" });
            CB_BlueberryForm.Location = new System.Drawing.Point(9, 44);
            CB_BlueberryForm.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_BlueberryForm.Name = "CB_BlueberryForm";
            CB_BlueberryForm.Size = new System.Drawing.Size(120, 23);
            CB_BlueberryForm.TabIndex = 45;
            // 
            // CHK_BlueberryShiny
            // 
            CHK_BlueberryShiny.AutoSize = true;
            CHK_BlueberryShiny.Location = new System.Drawing.Point(64, 18);
            CHK_BlueberryShiny.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_BlueberryShiny.Name = "CHK_BlueberryShiny";
            CHK_BlueberryShiny.Size = new System.Drawing.Size(55, 19);
            CHK_BlueberryShiny.TabIndex = 9;
            CHK_BlueberryShiny.Text = "Shiny";
            CHK_BlueberryShiny.UseVisualStyleBackColor = true;
            // 
            // CLB_FormObtained
            // 
            CLB_FormObtained.FormattingEnabled = true;
            CLB_FormObtained.Location = new System.Drawing.Point(306, 57);
            CLB_FormObtained.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CLB_FormObtained.Name = "CLB_FormObtained";
            CLB_FormObtained.Size = new System.Drawing.Size(112, 256);
            CLB_FormObtained.TabIndex = 50;
            // 
            // CLB_FormHeard
            // 
            CLB_FormHeard.FormattingEnabled = true;
            CLB_FormHeard.Location = new System.Drawing.Point(428, 57);
            CLB_FormHeard.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CLB_FormHeard.Name = "CLB_FormHeard";
            CLB_FormHeard.Size = new System.Drawing.Size(112, 256);
            CLB_FormHeard.TabIndex = 51;
            // 
            // CLB_FormViewed
            // 
            CLB_FormViewed.FormattingEnabled = true;
            CLB_FormViewed.Location = new System.Drawing.Point(550, 57);
            CLB_FormViewed.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CLB_FormViewed.Name = "CLB_FormViewed";
            CLB_FormViewed.Size = new System.Drawing.Size(112, 256);
            CLB_FormViewed.TabIndex = 52;
            // 
            // L_Seen
            // 
            L_Seen.AutoSize = true;
            L_Seen.Location = new System.Drawing.Point(184, 41);
            L_Seen.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Seen.Name = "L_Seen";
            L_Seen.Size = new System.Drawing.Size(35, 15);
            L_Seen.TabIndex = 53;
            L_Seen.Text = "Seen:";
            // 
            // L_Obtained
            // 
            L_Obtained.AutoSize = true;
            L_Obtained.Location = new System.Drawing.Point(306, 39);
            L_Obtained.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Obtained.Name = "L_Obtained";
            L_Obtained.Size = new System.Drawing.Size(59, 15);
            L_Obtained.TabIndex = 54;
            L_Obtained.Text = "Obtained:";
            // 
            // L_HeardOf
            // 
            L_HeardOf.AutoSize = true;
            L_HeardOf.Location = new System.Drawing.Point(428, 39);
            L_HeardOf.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_HeardOf.Name = "L_HeardOf";
            L_HeardOf.Size = new System.Drawing.Size(58, 15);
            L_HeardOf.TabIndex = 55;
            L_HeardOf.Text = "Heard Of:";
            // 
            // L_Viewed
            // 
            L_Viewed.AutoSize = true;
            L_Viewed.Location = new System.Drawing.Point(550, 39);
            L_Viewed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Viewed.Name = "L_Viewed";
            L_Viewed.Size = new System.Drawing.Size(48, 15);
            L_Viewed.TabIndex = 56;
            L_Viewed.Text = "Viewed:";
            // 
            // SAV_PokedexSVKitakami
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(820, 399);
            Controls.Add(L_Viewed);
            Controls.Add(L_HeardOf);
            Controls.Add(L_Obtained);
            Controls.Add(L_Seen);
            Controls.Add(CLB_FormViewed);
            Controls.Add(CLB_FormHeard);
            Controls.Add(CLB_FormObtained);
            Controls.Add(GB_Blueberry);
            Controls.Add(GB_Kitakami);
            Controls.Add(GB_SeenFlags);
            Controls.Add(CLB_FormSeen);
            Controls.Add(GB_Language);
            Controls.Add(GB_Paldea);
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
            Name = "SAV_PokedexSVKitakami";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Pokédex Editor";
            modifyMenu.ResumeLayout(false);
            GB_Paldea.ResumeLayout(false);
            GB_Paldea.PerformLayout();
            GB_Language.ResumeLayout(false);
            GB_Language.PerformLayout();
            GB_SeenFlags.ResumeLayout(false);
            GB_SeenFlags.PerformLayout();
            GB_Kitakami.ResumeLayout(false);
            GB_Kitakami.PerformLayout();
            GB_Blueberry.ResumeLayout(false);
            GB_Blueberry.PerformLayout();
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
        private System.Windows.Forms.ComboBox CB_PaldeaForm;
        private System.Windows.Forms.GroupBox GB_Paldea;
        private System.Windows.Forms.CheckBox CHK_PaldeaShiny;
        private System.Windows.Forms.ComboBox CB_PaldeaGender;
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
        private System.Windows.Forms.GroupBox GB_SeenFlags;
        private System.Windows.Forms.CheckBox CHK_SeenGenderless;
        private System.Windows.Forms.CheckBox CHK_SeenFemale;
        private System.Windows.Forms.CheckBox CHK_SeenMale;
        private System.Windows.Forms.GroupBox GB_Kitakami;
        private System.Windows.Forms.ComboBox CB_KitakamiGender;
        private System.Windows.Forms.ComboBox CB_KitakamiForm;
        private System.Windows.Forms.CheckBox CHK_KitakamiShiny;
        private System.Windows.Forms.GroupBox GB_Blueberry;
        private System.Windows.Forms.ComboBox CB_BlueberryGender;
        private System.Windows.Forms.ComboBox CB_BlueberryForm;
        private System.Windows.Forms.CheckBox CHK_BlueberryShiny;
        private System.Windows.Forms.CheckedListBox CLB_FormObtained;
        private System.Windows.Forms.CheckedListBox CLB_FormHeard;
        private System.Windows.Forms.CheckedListBox CLB_FormViewed;
        private System.Windows.Forms.Label L_Seen;
        private System.Windows.Forms.Label L_Obtained;
        private System.Windows.Forms.Label L_HeardOf;
        private System.Windows.Forms.Label L_Viewed;
    }
}
