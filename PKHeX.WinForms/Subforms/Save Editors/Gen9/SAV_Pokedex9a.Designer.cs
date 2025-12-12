namespace PKHeX.WinForms
{
    partial class SAV_Pokedex9a
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
            L_DisplayForm = new System.Windows.Forms.Label();
            L_DisplayGender = new System.Windows.Forms.Label();
            CB_Gender = new System.Windows.Forms.ComboBox();
            CHK_DisplayShiny = new System.Windows.Forms.CheckBox();
            GB_Language = new System.Windows.Forms.GroupBox();
            CHK_LangCHT = new System.Windows.Forms.CheckBox();
            CHK_LangCHS = new System.Windows.Forms.CheckBox();
            CHK_LangKOR = new System.Windows.Forms.CheckBox();
            CHK_LangLATAM = new System.Windows.Forms.CheckBox();
            CHK_LangSPA = new System.Windows.Forms.CheckBox();
            CHK_LangGER = new System.Windows.Forms.CheckBox();
            CHK_LangITA = new System.Windows.Forms.CheckBox();
            CHK_LangFRE = new System.Windows.Forms.CheckBox();
            CHK_LangENG = new System.Windows.Forms.CheckBox();
            CHK_LangJPN = new System.Windows.Forms.CheckBox();
            CLB_FormSeen = new System.Windows.Forms.CheckedListBox();
            GB_Seen = new System.Windows.Forms.GroupBox();
            CHK_SeenMega0 = new System.Windows.Forms.CheckBox();
            CHK_SeenMega1 = new System.Windows.Forms.CheckBox();
            CHK_SeenMega2 = new System.Windows.Forms.CheckBox();
            CHK_SeenAlpha = new System.Windows.Forms.CheckBox();
            CHK_SeenGenderless = new System.Windows.Forms.CheckBox();
            CHK_SeenFemale = new System.Windows.Forms.CheckBox();
            CHK_SeenMale = new System.Windows.Forms.CheckBox();
            CHK_IsNew = new System.Windows.Forms.CheckBox();
            CLB_FormCaught = new System.Windows.Forms.CheckedListBox();
            CLB_FormShiny = new System.Windows.Forms.CheckedListBox();
            L_Seen = new System.Windows.Forms.Label();
            L_Caught = new System.Windows.Forms.Label();
            L_SeenShiny = new System.Windows.Forms.Label();
            modifyMenu.SuspendLayout();
            GB_Displayed.SuspendLayout();
            GB_Language.SuspendLayout();
            GB_Seen.SuspendLayout();
            SuspendLayout();
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(690, 348);
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
            LB_Species.Location = new System.Drawing.Point(14, 49);
            LB_Species.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LB_Species.Name = "LB_Species";
            LB_Species.Size = new System.Drawing.Size(186, 327);
            LB_Species.TabIndex = 2;
            LB_Species.SelectedIndexChanged += ChangeLBSpecies;
            // 
            // L_goto
            // 
            L_goto.AutoSize = true;
            L_goto.Location = new System.Drawing.Point(14, 18);
            L_goto.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_goto.Name = "L_goto";
            L_goto.Size = new System.Drawing.Size(39, 17);
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
            CB_Species.Size = new System.Drawing.Size(142, 25);
            CB_Species.TabIndex = 21;
            CB_Species.SelectedIndexChanged += ChangeCBSpecies;
            CB_Species.SelectedValueChanged += ChangeCBSpecies;
            // 
            // B_GiveAll
            // 
            B_GiveAll.Location = new System.Drawing.Point(208, 13);
            B_GiveAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_GiveAll.Name = "B_GiveAll";
            B_GiveAll.Size = new System.Drawing.Size(87, 27);
            B_GiveAll.TabIndex = 23;
            B_GiveAll.Text = "Check All";
            B_GiveAll.UseVisualStyleBackColor = true;
            B_GiveAll.Click += B_GiveAll_Click;
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(690, 319);
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
            B_Modify.Location = new System.Drawing.Point(651, 12);
            B_Modify.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Modify.Name = "B_Modify";
            B_Modify.Size = new System.Drawing.Size(132, 27);
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
            modifyMenu.Size = new System.Drawing.Size(159, 114);
            // 
            // mnuSeenNone
            // 
            mnuSeenNone.Name = "mnuSeenNone";
            mnuSeenNone.Size = new System.Drawing.Size(158, 22);
            mnuSeenNone.Text = "Seen none";
            mnuSeenNone.Click += SeenNone;
            // 
            // mnuSeenAll
            // 
            mnuSeenAll.Name = "mnuSeenAll";
            mnuSeenAll.Size = new System.Drawing.Size(158, 22);
            mnuSeenAll.Text = "Seen all";
            mnuSeenAll.Click += SeenAll;
            // 
            // mnuCaughtNone
            // 
            mnuCaughtNone.Name = "mnuCaughtNone";
            mnuCaughtNone.Size = new System.Drawing.Size(158, 22);
            mnuCaughtNone.Text = "Caught none";
            mnuCaughtNone.Click += CaughtNone;
            // 
            // mnuCaughtAll
            // 
            mnuCaughtAll.Name = "mnuCaughtAll";
            mnuCaughtAll.Size = new System.Drawing.Size(158, 22);
            mnuCaughtAll.Text = "Caught all";
            mnuCaughtAll.Click += CaughtAll;
            // 
            // mnuComplete
            // 
            mnuComplete.Name = "mnuComplete";
            mnuComplete.Size = new System.Drawing.Size(158, 22);
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
            CB_DisplayForm.Location = new System.Drawing.Point(83, 47);
            CB_DisplayForm.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_DisplayForm.Name = "CB_DisplayForm";
            CB_DisplayForm.Size = new System.Drawing.Size(106, 25);
            CB_DisplayForm.TabIndex = 45;
            // 
            // GB_Displayed
            // 
            GB_Displayed.Controls.Add(L_DisplayForm);
            GB_Displayed.Controls.Add(L_DisplayGender);
            GB_Displayed.Controls.Add(CB_Gender);
            GB_Displayed.Controls.Add(CHK_DisplayShiny);
            GB_Displayed.Controls.Add(CB_DisplayForm);
            GB_Displayed.Location = new System.Drawing.Point(446, 45);
            GB_Displayed.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Displayed.Name = "GB_Displayed";
            GB_Displayed.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Displayed.Size = new System.Drawing.Size(194, 106);
            GB_Displayed.TabIndex = 44;
            GB_Displayed.TabStop = false;
            GB_Displayed.Text = "Displayed";
            // 
            // L_DisplayForm
            // 
            L_DisplayForm.Location = new System.Drawing.Point(9, 48);
            L_DisplayForm.Name = "L_DisplayForm";
            L_DisplayForm.Size = new System.Drawing.Size(72, 20);
            L_DisplayForm.TabIndex = 54;
            L_DisplayForm.Text = "Form:";
            L_DisplayForm.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_DisplayGender
            // 
            L_DisplayGender.Location = new System.Drawing.Point(9, 25);
            L_DisplayGender.Name = "L_DisplayGender";
            L_DisplayGender.Size = new System.Drawing.Size(72, 20);
            L_DisplayGender.TabIndex = 53;
            L_DisplayGender.Text = "Gender:";
            L_DisplayGender.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_Gender
            // 
            CB_Gender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Gender.FormattingEnabled = true;
            CB_Gender.Items.AddRange(new object[] { "-", "♂", "♀", "♂/♀" });
            CB_Gender.Location = new System.Drawing.Point(83, 22);
            CB_Gender.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Gender.Name = "CB_Gender";
            CB_Gender.Size = new System.Drawing.Size(64, 25);
            CB_Gender.TabIndex = 24;
            // 
            // CHK_DisplayShiny
            // 
            CHK_DisplayShiny.AutoSize = true;
            CHK_DisplayShiny.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            CHK_DisplayShiny.Location = new System.Drawing.Point(78, 76);
            CHK_DisplayShiny.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_DisplayShiny.Name = "CHK_DisplayShiny";
            CHK_DisplayShiny.Size = new System.Drawing.Size(60, 21);
            CHK_DisplayShiny.TabIndex = 9;
            CHK_DisplayShiny.Text = "Shiny:";
            CHK_DisplayShiny.UseVisualStyleBackColor = true;
            // 
            // GB_Language
            // 
            GB_Language.Controls.Add(CHK_LangCHT);
            GB_Language.Controls.Add(CHK_LangCHS);
            GB_Language.Controls.Add(CHK_LangKOR);
            GB_Language.Controls.Add(CHK_LangLATAM);
            GB_Language.Controls.Add(CHK_LangSPA);
            GB_Language.Controls.Add(CHK_LangGER);
            GB_Language.Controls.Add(CHK_LangITA);
            GB_Language.Controls.Add(CHK_LangFRE);
            GB_Language.Controls.Add(CHK_LangENG);
            GB_Language.Controls.Add(CHK_LangJPN);
            GB_Language.Location = new System.Drawing.Point(651, 64);
            GB_Language.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Language.Name = "GB_Language";
            GB_Language.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Language.Size = new System.Drawing.Size(134, 228);
            GB_Language.TabIndex = 47;
            GB_Language.TabStop = false;
            GB_Language.Text = "Languages";
            // 
            // CHK_LangCHT
            // 
            CHK_LangCHT.AutoSize = true;
            CHK_LangCHT.Location = new System.Drawing.Point(9, 201);
            CHK_LangCHT.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_LangCHT.Name = "CHK_LangCHT";
            CHK_LangCHT.Size = new System.Drawing.Size(79, 21);
            CHK_LangCHT.TabIndex = 21;
            CHK_LangCHT.Text = "ChineseT";
            CHK_LangCHT.UseVisualStyleBackColor = true;
            // 
            // CHK_LangCHS
            // 
            CHK_LangCHS.AutoSize = true;
            CHK_LangCHS.Location = new System.Drawing.Point(9, 181);
            CHK_LangCHS.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_LangCHS.Name = "CHK_LangCHS";
            CHK_LangCHS.Size = new System.Drawing.Size(79, 21);
            CHK_LangCHS.TabIndex = 20;
            CHK_LangCHS.Text = "ChineseS";
            CHK_LangCHS.UseVisualStyleBackColor = true;
            // 
            // CHK_LangKOR
            // 
            CHK_LangKOR.AutoSize = true;
            CHK_LangKOR.Location = new System.Drawing.Point(9, 161);
            CHK_LangKOR.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_LangKOR.Name = "CHK_LangKOR";
            CHK_LangKOR.Size = new System.Drawing.Size(69, 21);
            CHK_LangKOR.TabIndex = 19;
            CHK_LangKOR.Text = "Korean";
            CHK_LangKOR.UseVisualStyleBackColor = true;
            // 
            // CHK_LangLATAM
            // 
            CHK_LangLATAM.AutoSize = true;
            CHK_LangLATAM.Location = new System.Drawing.Point(9, 141);
            CHK_LangLATAM.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_LangLATAM.Name = "CHK_LangLATAM";
            CHK_LangLATAM.Size = new System.Drawing.Size(98, 21);
            CHK_LangLATAM.TabIndex = 22;
            CHK_LangLATAM.Text = "Spanish (LA)";
            CHK_LangLATAM.UseVisualStyleBackColor = true;
            // 
            // CHK_LangSPA
            // 
            CHK_LangSPA.AutoSize = true;
            CHK_LangSPA.Location = new System.Drawing.Point(9, 121);
            CHK_LangSPA.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_LangSPA.Name = "CHK_LangSPA";
            CHK_LangSPA.Size = new System.Drawing.Size(100, 21);
            CHK_LangSPA.TabIndex = 18;
            CHK_LangSPA.Text = "Spanish (EU)";
            CHK_LangSPA.UseVisualStyleBackColor = true;
            // 
            // CHK_LangGER
            // 
            CHK_LangGER.AutoSize = true;
            CHK_LangGER.Location = new System.Drawing.Point(9, 101);
            CHK_LangGER.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_LangGER.Name = "CHK_LangGER";
            CHK_LangGER.Size = new System.Drawing.Size(73, 21);
            CHK_LangGER.TabIndex = 17;
            CHK_LangGER.Text = "German";
            CHK_LangGER.UseVisualStyleBackColor = true;
            // 
            // CHK_LangITA
            // 
            CHK_LangITA.AutoSize = true;
            CHK_LangITA.Location = new System.Drawing.Point(9, 81);
            CHK_LangITA.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_LangITA.Name = "CHK_LangITA";
            CHK_LangITA.Size = new System.Drawing.Size(61, 21);
            CHK_LangITA.TabIndex = 16;
            CHK_LangITA.Text = "Italian";
            CHK_LangITA.UseVisualStyleBackColor = true;
            // 
            // CHK_LangFRE
            // 
            CHK_LangFRE.AutoSize = true;
            CHK_LangFRE.Location = new System.Drawing.Point(9, 60);
            CHK_LangFRE.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_LangFRE.Name = "CHK_LangFRE";
            CHK_LangFRE.Size = new System.Drawing.Size(65, 21);
            CHK_LangFRE.TabIndex = 15;
            CHK_LangFRE.Text = "French";
            CHK_LangFRE.UseVisualStyleBackColor = true;
            // 
            // CHK_LangENG
            // 
            CHK_LangENG.AutoSize = true;
            CHK_LangENG.Location = new System.Drawing.Point(9, 40);
            CHK_LangENG.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_LangENG.Name = "CHK_LangENG";
            CHK_LangENG.Size = new System.Drawing.Size(68, 21);
            CHK_LangENG.TabIndex = 14;
            CHK_LangENG.Text = "English";
            CHK_LangENG.UseVisualStyleBackColor = true;
            // 
            // CHK_LangJPN
            // 
            CHK_LangJPN.AutoSize = true;
            CHK_LangJPN.Location = new System.Drawing.Point(9, 20);
            CHK_LangJPN.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_LangJPN.Name = "CHK_LangJPN";
            CHK_LangJPN.Size = new System.Drawing.Size(81, 21);
            CHK_LangJPN.TabIndex = 13;
            CHK_LangJPN.Text = "Japanese";
            CHK_LangJPN.UseVisualStyleBackColor = true;
            // 
            // CLB_FormSeen
            // 
            CLB_FormSeen.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            CLB_FormSeen.FormattingEnabled = true;
            CLB_FormSeen.Location = new System.Drawing.Point(210, 172);
            CLB_FormSeen.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CLB_FormSeen.Name = "CLB_FormSeen";
            CLB_FormSeen.Size = new System.Drawing.Size(138, 204);
            CLB_FormSeen.TabIndex = 48;
            // 
            // GB_Seen
            // 
            GB_Seen.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            GB_Seen.Controls.Add(CHK_SeenMega0);
            GB_Seen.Controls.Add(CHK_SeenMega1);
            GB_Seen.Controls.Add(CHK_SeenMega2);
            GB_Seen.Controls.Add(CHK_SeenAlpha);
            GB_Seen.Controls.Add(CHK_SeenGenderless);
            GB_Seen.Controls.Add(CHK_SeenFemale);
            GB_Seen.Controls.Add(CHK_SeenMale);
            GB_Seen.Location = new System.Drawing.Point(210, 45);
            GB_Seen.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Seen.Name = "GB_Seen";
            GB_Seen.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Seen.Size = new System.Drawing.Size(230, 106);
            GB_Seen.TabIndex = 49;
            GB_Seen.TabStop = false;
            GB_Seen.Text = "Seen";
            // 
            // CHK_SeenMega0
            // 
            CHK_SeenMega0.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            CHK_SeenMega0.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            CHK_SeenMega0.Location = new System.Drawing.Point(104, 40);
            CHK_SeenMega0.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_SeenMega0.Name = "CHK_SeenMega0";
            CHK_SeenMega0.Size = new System.Drawing.Size(118, 21);
            CHK_SeenMega0.TabIndex = 55;
            CHK_SeenMega0.Text = "Mega0";
            CHK_SeenMega0.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            CHK_SeenMega0.UseVisualStyleBackColor = true;
            // 
            // CHK_SeenMega1
            // 
            CHK_SeenMega1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            CHK_SeenMega1.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            CHK_SeenMega1.Location = new System.Drawing.Point(104, 60);
            CHK_SeenMega1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_SeenMega1.Name = "CHK_SeenMega1";
            CHK_SeenMega1.Size = new System.Drawing.Size(118, 21);
            CHK_SeenMega1.TabIndex = 58;
            CHK_SeenMega1.Text = "Mega1";
            CHK_SeenMega1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            CHK_SeenMega1.UseVisualStyleBackColor = true;
            // 
            // CHK_SeenMega2
            // 
            CHK_SeenMega2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            CHK_SeenMega2.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            CHK_SeenMega2.Location = new System.Drawing.Point(104, 80);
            CHK_SeenMega2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_SeenMega2.Name = "CHK_SeenMega2";
            CHK_SeenMega2.Size = new System.Drawing.Size(118, 21);
            CHK_SeenMega2.TabIndex = 59;
            CHK_SeenMega2.Text = "Mega2";
            CHK_SeenMega2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            CHK_SeenMega2.UseVisualStyleBackColor = true;
            // 
            // CHK_SeenAlpha
            // 
            CHK_SeenAlpha.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            CHK_SeenAlpha.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            CHK_SeenAlpha.Location = new System.Drawing.Point(104, 20);
            CHK_SeenAlpha.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_SeenAlpha.Name = "CHK_SeenAlpha";
            CHK_SeenAlpha.Size = new System.Drawing.Size(118, 21);
            CHK_SeenAlpha.TabIndex = 56;
            CHK_SeenAlpha.Text = "Alpha";
            CHK_SeenAlpha.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            CHK_SeenAlpha.UseVisualStyleBackColor = true;
            // 
            // CHK_SeenGenderless
            // 
            CHK_SeenGenderless.AutoSize = true;
            CHK_SeenGenderless.Location = new System.Drawing.Point(9, 60);
            CHK_SeenGenderless.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_SeenGenderless.Name = "CHK_SeenGenderless";
            CHK_SeenGenderless.Size = new System.Drawing.Size(92, 21);
            CHK_SeenGenderless.TabIndex = 12;
            CHK_SeenGenderless.Text = "Genderless";
            CHK_SeenGenderless.UseVisualStyleBackColor = true;
            // 
            // CHK_SeenFemale
            // 
            CHK_SeenFemale.AutoSize = true;
            CHK_SeenFemale.Location = new System.Drawing.Point(9, 40);
            CHK_SeenFemale.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_SeenFemale.Name = "CHK_SeenFemale";
            CHK_SeenFemale.Size = new System.Drawing.Size(68, 21);
            CHK_SeenFemale.TabIndex = 9;
            CHK_SeenFemale.Text = "Female";
            CHK_SeenFemale.UseVisualStyleBackColor = true;
            // 
            // CHK_SeenMale
            // 
            CHK_SeenMale.AutoSize = true;
            CHK_SeenMale.Location = new System.Drawing.Point(9, 20);
            CHK_SeenMale.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_SeenMale.Name = "CHK_SeenMale";
            CHK_SeenMale.Size = new System.Drawing.Size(56, 21);
            CHK_SeenMale.TabIndex = 11;
            CHK_SeenMale.Text = "Male";
            CHK_SeenMale.UseVisualStyleBackColor = true;
            // 
            // CHK_IsNew
            // 
            CHK_IsNew.AutoSize = true;
            CHK_IsNew.Location = new System.Drawing.Point(314, 18);
            CHK_IsNew.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_IsNew.Name = "CHK_IsNew";
            CHK_IsNew.Size = new System.Drawing.Size(53, 21);
            CHK_IsNew.TabIndex = 46;
            CHK_IsNew.Text = "New";
            CHK_IsNew.UseVisualStyleBackColor = true;
            // 
            // CLB_FormCaught
            // 
            CLB_FormCaught.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            CLB_FormCaught.FormattingEnabled = true;
            CLB_FormCaught.Location = new System.Drawing.Point(356, 172);
            CLB_FormCaught.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CLB_FormCaught.Name = "CLB_FormCaught";
            CLB_FormCaught.Size = new System.Drawing.Size(138, 204);
            CLB_FormCaught.TabIndex = 50;
            // 
            // CLB_FormShiny
            // 
            CLB_FormShiny.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            CLB_FormShiny.FormattingEnabled = true;
            CLB_FormShiny.Location = new System.Drawing.Point(502, 172);
            CLB_FormShiny.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CLB_FormShiny.Name = "CLB_FormShiny";
            CLB_FormShiny.Size = new System.Drawing.Size(138, 204);
            CLB_FormShiny.TabIndex = 51;
            // 
            // L_Seen
            // 
            L_Seen.AutoSize = true;
            L_Seen.Location = new System.Drawing.Point(210, 152);
            L_Seen.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Seen.Name = "L_Seen";
            L_Seen.Size = new System.Drawing.Size(39, 17);
            L_Seen.TabIndex = 52;
            L_Seen.Text = "Seen:";
            // 
            // L_Caught
            // 
            L_Caught.AutoSize = true;
            L_Caught.Location = new System.Drawing.Point(356, 152);
            L_Caught.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Caught.Name = "L_Caught";
            L_Caught.Size = new System.Drawing.Size(52, 17);
            L_Caught.TabIndex = 53;
            L_Caught.Text = "Caught:";
            // 
            // L_SeenShiny
            // 
            L_SeenShiny.AutoSize = true;
            L_SeenShiny.Location = new System.Drawing.Point(502, 152);
            L_SeenShiny.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_SeenShiny.Name = "L_SeenShiny";
            L_SeenShiny.Size = new System.Drawing.Size(81, 17);
            L_SeenShiny.TabIndex = 54;
            L_SeenShiny.Text = "Seen (Shiny):";
            // 
            // SAV_Pokedex9a
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(798, 388);
            Controls.Add(L_SeenShiny);
            Controls.Add(L_Caught);
            Controls.Add(L_Seen);
            Controls.Add(CLB_FormShiny);
            Controls.Add(CLB_FormCaught);
            Controls.Add(CHK_IsNew);
            Controls.Add(GB_Seen);
            Controls.Add(CLB_FormSeen);
            Controls.Add(GB_Language);
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
            Name = "SAV_Pokedex9a";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Pokédex Editor";
            modifyMenu.ResumeLayout(false);
            GB_Displayed.ResumeLayout(false);
            GB_Displayed.PerformLayout();
            GB_Language.ResumeLayout(false);
            GB_Language.PerformLayout();
            GB_Seen.ResumeLayout(false);
            GB_Seen.PerformLayout();
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
        private System.Windows.Forms.ComboBox CB_Gender;
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
        private System.Windows.Forms.GroupBox GB_Seen;
        private System.Windows.Forms.CheckBox CHK_SeenGenderless;
        private System.Windows.Forms.CheckBox CHK_SeenFemale;
        private System.Windows.Forms.CheckBox CHK_SeenMale;
        private System.Windows.Forms.CheckBox CHK_IsNew;
        private System.Windows.Forms.Label L_DisplayForm;
        private System.Windows.Forms.Label L_DisplayGender;
        private System.Windows.Forms.CheckedListBox CLB_FormCaught;
        private System.Windows.Forms.CheckedListBox CLB_FormShiny;
        private System.Windows.Forms.CheckBox CHK_SeenAlpha;
        private System.Windows.Forms.CheckBox CHK_SeenMega0;
        private System.Windows.Forms.CheckBox CHK_SeenMega1;
        private System.Windows.Forms.CheckBox CHK_SeenMega2;
        private System.Windows.Forms.CheckBox CHK_LangLATAM;
        private System.Windows.Forms.Label L_Seen;
        private System.Windows.Forms.Label L_Caught;
        private System.Windows.Forms.Label L_SeenShiny;
    }
}
