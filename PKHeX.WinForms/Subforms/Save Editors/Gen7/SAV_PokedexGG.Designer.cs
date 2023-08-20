namespace PKHeX.WinForms
{
    partial class SAV_PokedexGG
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
            CHK_P1 = new System.Windows.Forms.CheckBox();
            CHK_P2 = new System.Windows.Forms.CheckBox();
            CHK_P3 = new System.Windows.Forms.CheckBox();
            CHK_P4 = new System.Windows.Forms.CheckBox();
            CHK_P5 = new System.Windows.Forms.CheckBox();
            CHK_P6 = new System.Windows.Forms.CheckBox();
            CHK_P7 = new System.Windows.Forms.CheckBox();
            CHK_P8 = new System.Windows.Forms.CheckBox();
            CHK_P9 = new System.Windows.Forms.CheckBox();
            CHK_L7 = new System.Windows.Forms.CheckBox();
            CHK_L6 = new System.Windows.Forms.CheckBox();
            CHK_L5 = new System.Windows.Forms.CheckBox();
            CHK_L4 = new System.Windows.Forms.CheckBox();
            CHK_L3 = new System.Windows.Forms.CheckBox();
            CHK_L2 = new System.Windows.Forms.CheckBox();
            CHK_L1 = new System.Windows.Forms.CheckBox();
            L_goto = new System.Windows.Forms.Label();
            CB_Species = new System.Windows.Forms.ComboBox();
            B_GiveAll = new System.Windows.Forms.Button();
            B_Save = new System.Windows.Forms.Button();
            B_Modify = new System.Windows.Forms.Button();
            GB_Language = new System.Windows.Forms.GroupBox();
            CHK_L9 = new System.Windows.Forms.CheckBox();
            CHK_L8 = new System.Windows.Forms.CheckBox();
            GB_Encountered = new System.Windows.Forms.GroupBox();
            GB_Owned = new System.Windows.Forms.GroupBox();
            GB_Displayed = new System.Windows.Forms.GroupBox();
            modifyMenu = new System.Windows.Forms.ContextMenuStrip(components);
            mnuSeenNone = new System.Windows.Forms.ToolStripMenuItem();
            mnuSeenAll = new System.Windows.Forms.ToolStripMenuItem();
            mnuCaughtNone = new System.Windows.Forms.ToolStripMenuItem();
            mnuCaughtAll = new System.Windows.Forms.ToolStripMenuItem();
            mnuComplete = new System.Windows.Forms.ToolStripMenuItem();
            mnuFormNone = new System.Windows.Forms.ToolStripMenuItem();
            mnuForm1 = new System.Windows.Forms.ToolStripMenuItem();
            mnuFormAll = new System.Windows.Forms.ToolStripMenuItem();
            LB_Forms = new System.Windows.Forms.ListBox();
            GB_SizeRecords = new System.Windows.Forms.GroupBox();
            CHK_RMaxWeight = new System.Windows.Forms.CheckBox();
            CHK_RMinWeight = new System.Windows.Forms.CheckBox();
            CHK_RMaxHeight = new System.Windows.Forms.CheckBox();
            CHK_RMinHeight = new System.Windows.Forms.CheckBox();
            L_RWeight = new System.Windows.Forms.Label();
            L_RHeight = new System.Windows.Forms.Label();
            L_RWeightMax = new System.Windows.Forms.Label();
            L_RHeightMax = new System.Windows.Forms.Label();
            L_RWeightMin = new System.Windows.Forms.Label();
            L_RHeightMin = new System.Windows.Forms.Label();
            NUD_RWeightMax = new System.Windows.Forms.NumericUpDown();
            NUD_RWeightMin = new System.Windows.Forms.NumericUpDown();
            NUD_RHeightMaxWeight = new System.Windows.Forms.NumericUpDown();
            NUD_RHeightMinWeight = new System.Windows.Forms.NumericUpDown();
            NUD_RWeightMaxHeight = new System.Windows.Forms.NumericUpDown();
            NUD_RWeightMinHeight = new System.Windows.Forms.NumericUpDown();
            NUD_RHeightMax = new System.Windows.Forms.NumericUpDown();
            NUD_RHeightMin = new System.Windows.Forms.NumericUpDown();
            B_Counts = new System.Windows.Forms.Button();
            CHK_MinH = new System.Windows.Forms.CheckBox();
            CHK_MaxH = new System.Windows.Forms.CheckBox();
            CHK_MinW = new System.Windows.Forms.CheckBox();
            CHK_MaxW = new System.Windows.Forms.CheckBox();
            GB_Language.SuspendLayout();
            GB_Encountered.SuspendLayout();
            GB_Owned.SuspendLayout();
            GB_Displayed.SuspendLayout();
            modifyMenu.SuspendLayout();
            GB_SizeRecords.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_RWeightMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_RWeightMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_RHeightMaxWeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_RHeightMinWeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_RWeightMaxHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_RWeightMinHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_RHeightMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_RHeightMin).BeginInit();
            SuspendLayout();
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(285, 453);
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
            LB_Species.Size = new System.Drawing.Size(186, 364);
            LB_Species.TabIndex = 2;
            LB_Species.SelectedIndexChanged += ChangeLBSpecies;
            // 
            // CHK_P1
            // 
            CHK_P1.AutoSize = true;
            CHK_P1.Location = new System.Drawing.Point(21, 17);
            CHK_P1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_P1.Name = "CHK_P1";
            CHK_P1.Size = new System.Drawing.Size(64, 19);
            CHK_P1.TabIndex = 3;
            CHK_P1.Text = "Owned";
            CHK_P1.UseVisualStyleBackColor = true;
            // 
            // CHK_P2
            // 
            CHK_P2.AutoSize = true;
            CHK_P2.Location = new System.Drawing.Point(7, 29);
            CHK_P2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_P2.Name = "CHK_P2";
            CHK_P2.Size = new System.Drawing.Size(52, 19);
            CHK_P2.TabIndex = 4;
            CHK_P2.Text = "Male";
            CHK_P2.UseVisualStyleBackColor = true;
            CHK_P2.Click += ChangeEncountered;
            // 
            // CHK_P3
            // 
            CHK_P3.AutoSize = true;
            CHK_P3.Location = new System.Drawing.Point(7, 48);
            CHK_P3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_P3.Name = "CHK_P3";
            CHK_P3.Size = new System.Drawing.Size(64, 19);
            CHK_P3.TabIndex = 5;
            CHK_P3.Text = "Female";
            CHK_P3.UseVisualStyleBackColor = true;
            CHK_P3.Click += ChangeEncountered;
            // 
            // CHK_P4
            // 
            CHK_P4.AutoSize = true;
            CHK_P4.Location = new System.Drawing.Point(7, 69);
            CHK_P4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_P4.Name = "CHK_P4";
            CHK_P4.Size = new System.Drawing.Size(84, 19);
            CHK_P4.TabIndex = 6;
            CHK_P4.Text = "Shiny Male";
            CHK_P4.UseVisualStyleBackColor = true;
            CHK_P4.Click += ChangeEncountered;
            // 
            // CHK_P5
            // 
            CHK_P5.AutoSize = true;
            CHK_P5.Location = new System.Drawing.Point(7, 89);
            CHK_P5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_P5.Name = "CHK_P5";
            CHK_P5.Size = new System.Drawing.Size(96, 19);
            CHK_P5.TabIndex = 7;
            CHK_P5.Text = "Shiny Female";
            CHK_P5.UseVisualStyleBackColor = true;
            CHK_P5.Click += ChangeEncountered;
            // 
            // CHK_P6
            // 
            CHK_P6.AutoSize = true;
            CHK_P6.Location = new System.Drawing.Point(7, 29);
            CHK_P6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_P6.Name = "CHK_P6";
            CHK_P6.Size = new System.Drawing.Size(52, 19);
            CHK_P6.TabIndex = 8;
            CHK_P6.Text = "Male";
            CHK_P6.UseVisualStyleBackColor = true;
            CHK_P6.Click += ChangeDisplayed;
            // 
            // CHK_P7
            // 
            CHK_P7.AutoSize = true;
            CHK_P7.Location = new System.Drawing.Point(7, 48);
            CHK_P7.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_P7.Name = "CHK_P7";
            CHK_P7.Size = new System.Drawing.Size(64, 19);
            CHK_P7.TabIndex = 9;
            CHK_P7.Text = "Female";
            CHK_P7.UseVisualStyleBackColor = true;
            CHK_P7.Click += ChangeDisplayed;
            // 
            // CHK_P8
            // 
            CHK_P8.AutoSize = true;
            CHK_P8.Location = new System.Drawing.Point(7, 69);
            CHK_P8.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_P8.Name = "CHK_P8";
            CHK_P8.Size = new System.Drawing.Size(84, 19);
            CHK_P8.TabIndex = 10;
            CHK_P8.Text = "Shiny Male";
            CHK_P8.UseVisualStyleBackColor = true;
            CHK_P8.Click += ChangeDisplayed;
            // 
            // CHK_P9
            // 
            CHK_P9.AutoSize = true;
            CHK_P9.Location = new System.Drawing.Point(7, 89);
            CHK_P9.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_P9.Name = "CHK_P9";
            CHK_P9.Size = new System.Drawing.Size(96, 19);
            CHK_P9.TabIndex = 11;
            CHK_P9.Text = "Shiny Female";
            CHK_P9.UseVisualStyleBackColor = true;
            CHK_P9.Click += ChangeDisplayed;
            // 
            // CHK_L7
            // 
            CHK_L7.AutoSize = true;
            CHK_L7.Location = new System.Drawing.Point(21, 135);
            CHK_L7.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_L7.Name = "CHK_L7";
            CHK_L7.Size = new System.Drawing.Size(63, 19);
            CHK_L7.TabIndex = 19;
            CHK_L7.Text = "Korean";
            CHK_L7.UseVisualStyleBackColor = true;
            // 
            // CHK_L6
            // 
            CHK_L6.AutoSize = true;
            CHK_L6.Location = new System.Drawing.Point(21, 117);
            CHK_L6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_L6.Name = "CHK_L6";
            CHK_L6.Size = new System.Drawing.Size(67, 19);
            CHK_L6.TabIndex = 18;
            CHK_L6.Text = "Spanish";
            CHK_L6.UseVisualStyleBackColor = true;
            // 
            // CHK_L5
            // 
            CHK_L5.AutoSize = true;
            CHK_L5.Location = new System.Drawing.Point(21, 96);
            CHK_L5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_L5.Name = "CHK_L5";
            CHK_L5.Size = new System.Drawing.Size(68, 19);
            CHK_L5.TabIndex = 17;
            CHK_L5.Text = "German";
            CHK_L5.UseVisualStyleBackColor = true;
            // 
            // CHK_L4
            // 
            CHK_L4.AutoSize = true;
            CHK_L4.Location = new System.Drawing.Point(21, 76);
            CHK_L4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_L4.Name = "CHK_L4";
            CHK_L4.Size = new System.Drawing.Size(58, 19);
            CHK_L4.TabIndex = 16;
            CHK_L4.Text = "Italian";
            CHK_L4.UseVisualStyleBackColor = true;
            // 
            // CHK_L3
            // 
            CHK_L3.AutoSize = true;
            CHK_L3.Location = new System.Drawing.Point(21, 57);
            CHK_L3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_L3.Name = "CHK_L3";
            CHK_L3.Size = new System.Drawing.Size(62, 19);
            CHK_L3.TabIndex = 15;
            CHK_L3.Text = "French";
            CHK_L3.UseVisualStyleBackColor = true;
            // 
            // CHK_L2
            // 
            CHK_L2.AutoSize = true;
            CHK_L2.Location = new System.Drawing.Point(21, 38);
            CHK_L2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_L2.Name = "CHK_L2";
            CHK_L2.Size = new System.Drawing.Size(64, 19);
            CHK_L2.TabIndex = 14;
            CHK_L2.Text = "English";
            CHK_L2.UseVisualStyleBackColor = true;
            // 
            // CHK_L1
            // 
            CHK_L1.AutoSize = true;
            CHK_L1.Location = new System.Drawing.Point(21, 17);
            CHK_L1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_L1.Name = "CHK_L1";
            CHK_L1.Size = new System.Drawing.Size(73, 19);
            CHK_L1.TabIndex = 13;
            CHK_L1.Text = "Japanese";
            CHK_L1.UseVisualStyleBackColor = true;
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
            B_Save.Location = new System.Drawing.Point(385, 453);
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
            B_Modify.Location = new System.Drawing.Point(405, 13);
            B_Modify.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Modify.Name = "B_Modify";
            B_Modify.Size = new System.Drawing.Size(70, 27);
            B_Modify.TabIndex = 25;
            B_Modify.Text = "Modify...";
            B_Modify.UseVisualStyleBackColor = true;
            B_Modify.Click += B_Modify_Click;
            // 
            // GB_Language
            // 
            GB_Language.Controls.Add(CHK_L9);
            GB_Language.Controls.Add(CHK_L8);
            GB_Language.Controls.Add(CHK_L7);
            GB_Language.Controls.Add(CHK_L6);
            GB_Language.Controls.Add(CHK_L5);
            GB_Language.Controls.Add(CHK_L4);
            GB_Language.Controls.Add(CHK_L3);
            GB_Language.Controls.Add(CHK_L2);
            GB_Language.Controls.Add(CHK_L1);
            GB_Language.Location = new System.Drawing.Point(350, 95);
            GB_Language.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Language.Name = "GB_Language";
            GB_Language.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Language.Size = new System.Drawing.Size(126, 198);
            GB_Language.TabIndex = 26;
            GB_Language.TabStop = false;
            GB_Language.Text = "Languages";
            // 
            // CHK_L9
            // 
            CHK_L9.AutoSize = true;
            CHK_L9.Location = new System.Drawing.Point(21, 175);
            CHK_L9.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_L9.Name = "CHK_L9";
            CHK_L9.Size = new System.Drawing.Size(74, 19);
            CHK_L9.TabIndex = 21;
            CHK_L9.Text = "Chinese2";
            CHK_L9.UseVisualStyleBackColor = true;
            // 
            // CHK_L8
            // 
            CHK_L8.AutoSize = true;
            CHK_L8.Location = new System.Drawing.Point(21, 155);
            CHK_L8.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_L8.Name = "CHK_L8";
            CHK_L8.Size = new System.Drawing.Size(68, 19);
            CHK_L8.TabIndex = 20;
            CHK_L8.Text = "Chinese";
            CHK_L8.UseVisualStyleBackColor = true;
            // 
            // GB_Encountered
            // 
            GB_Encountered.Controls.Add(CHK_P5);
            GB_Encountered.Controls.Add(CHK_P4);
            GB_Encountered.Controls.Add(CHK_P3);
            GB_Encountered.Controls.Add(CHK_P2);
            GB_Encountered.Location = new System.Drawing.Point(208, 46);
            GB_Encountered.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Encountered.Name = "GB_Encountered";
            GB_Encountered.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Encountered.Size = new System.Drawing.Size(135, 120);
            GB_Encountered.TabIndex = 27;
            GB_Encountered.TabStop = false;
            GB_Encountered.Text = "Seen";
            // 
            // GB_Owned
            // 
            GB_Owned.Controls.Add(CHK_P1);
            GB_Owned.Location = new System.Drawing.Point(350, 46);
            GB_Owned.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Owned.Name = "GB_Owned";
            GB_Owned.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Owned.Size = new System.Drawing.Size(126, 42);
            GB_Owned.TabIndex = 28;
            GB_Owned.TabStop = false;
            GB_Owned.Text = "Owned";
            // 
            // GB_Displayed
            // 
            GB_Displayed.Controls.Add(CHK_P9);
            GB_Displayed.Controls.Add(CHK_P8);
            GB_Displayed.Controls.Add(CHK_P7);
            GB_Displayed.Controls.Add(CHK_P6);
            GB_Displayed.Location = new System.Drawing.Point(208, 174);
            GB_Displayed.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Displayed.Name = "GB_Displayed";
            GB_Displayed.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Displayed.Size = new System.Drawing.Size(135, 120);
            GB_Displayed.TabIndex = 31;
            GB_Displayed.TabStop = false;
            GB_Displayed.Text = "Displayed";
            // 
            // modifyMenu
            // 
            modifyMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuSeenNone, mnuSeenAll, mnuCaughtNone, mnuCaughtAll, mnuComplete });
            modifyMenu.Name = "modifyMenu";
            modifyMenu.Size = new System.Drawing.Size(150, 114);
            // 
            // mnuSeenNone
            // 
            mnuSeenNone.Name = "mnuSeenNone";
            mnuSeenNone.Size = new System.Drawing.Size(149, 22);
            mnuSeenNone.Text = "Seen none";
            mnuSeenNone.Click += ModifyAll;
            // 
            // mnuSeenAll
            // 
            mnuSeenAll.Name = "mnuSeenAll";
            mnuSeenAll.Size = new System.Drawing.Size(149, 22);
            mnuSeenAll.Text = "Seen all";
            mnuSeenAll.Click += ModifyAll;
            // 
            // mnuCaughtNone
            // 
            mnuCaughtNone.Name = "mnuCaughtNone";
            mnuCaughtNone.Size = new System.Drawing.Size(149, 22);
            mnuCaughtNone.Text = "Caught none";
            mnuCaughtNone.Click += ModifyAll;
            // 
            // mnuCaughtAll
            // 
            mnuCaughtAll.Name = "mnuCaughtAll";
            mnuCaughtAll.Size = new System.Drawing.Size(149, 22);
            mnuCaughtAll.Text = "Caught all";
            mnuCaughtAll.Click += ModifyAll;
            // 
            // mnuComplete
            // 
            mnuComplete.Name = "mnuComplete";
            mnuComplete.Size = new System.Drawing.Size(149, 22);
            mnuComplete.Text = "Complete Dex";
            mnuComplete.Click += ModifyAll;
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
            // LB_Forms
            // 
            LB_Forms.FormattingEnabled = true;
            LB_Forms.ItemHeight = 15;
            LB_Forms.Location = new System.Drawing.Point(14, 418);
            LB_Forms.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LB_Forms.Name = "LB_Forms";
            LB_Forms.Size = new System.Drawing.Size(186, 64);
            LB_Forms.TabIndex = 33;
            LB_Forms.SelectedIndexChanged += ChangeLBForms;
            // 
            // GB_SizeRecords
            // 
            GB_SizeRecords.Controls.Add(CHK_MaxW);
            GB_SizeRecords.Controls.Add(CHK_MinW);
            GB_SizeRecords.Controls.Add(CHK_MaxH);
            GB_SizeRecords.Controls.Add(CHK_MinH);
            GB_SizeRecords.Controls.Add(CHK_RMaxWeight);
            GB_SizeRecords.Controls.Add(CHK_RMinWeight);
            GB_SizeRecords.Controls.Add(CHK_RMaxHeight);
            GB_SizeRecords.Controls.Add(CHK_RMinHeight);
            GB_SizeRecords.Controls.Add(L_RWeight);
            GB_SizeRecords.Controls.Add(L_RHeight);
            GB_SizeRecords.Controls.Add(L_RWeightMax);
            GB_SizeRecords.Controls.Add(L_RHeightMax);
            GB_SizeRecords.Controls.Add(L_RWeightMin);
            GB_SizeRecords.Controls.Add(L_RHeightMin);
            GB_SizeRecords.Controls.Add(NUD_RWeightMax);
            GB_SizeRecords.Controls.Add(NUD_RWeightMin);
            GB_SizeRecords.Controls.Add(NUD_RHeightMaxWeight);
            GB_SizeRecords.Controls.Add(NUD_RHeightMinWeight);
            GB_SizeRecords.Controls.Add(NUD_RWeightMaxHeight);
            GB_SizeRecords.Controls.Add(NUD_RWeightMinHeight);
            GB_SizeRecords.Controls.Add(NUD_RHeightMax);
            GB_SizeRecords.Controls.Add(NUD_RHeightMin);
            GB_SizeRecords.Location = new System.Drawing.Point(208, 301);
            GB_SizeRecords.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_SizeRecords.Name = "GB_SizeRecords";
            GB_SizeRecords.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_SizeRecords.Size = new System.Drawing.Size(268, 145);
            GB_SizeRecords.TabIndex = 52;
            GB_SizeRecords.TabStop = false;
            GB_SizeRecords.Text = "Records";
            // 
            // CHK_RMaxWeight
            // 
            CHK_RMaxWeight.AutoSize = true;
            CHK_RMaxWeight.Location = new System.Drawing.Point(203, 114);
            CHK_RMaxWeight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_RMaxWeight.Name = "CHK_RMaxWeight";
            CHK_RMaxWeight.Size = new System.Drawing.Size(52, 19);
            CHK_RMaxWeight.TabIndex = 69;
            CHK_RMaxWeight.Text = "Used";
            CHK_RMaxWeight.UseVisualStyleBackColor = true;
            CHK_RMaxWeight.CheckedChanged += CHK_RUsed_CheckedChanged;
            // 
            // CHK_RMinWeight
            // 
            CHK_RMinWeight.AutoSize = true;
            CHK_RMinWeight.Location = new System.Drawing.Point(203, 91);
            CHK_RMinWeight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_RMinWeight.Name = "CHK_RMinWeight";
            CHK_RMinWeight.Size = new System.Drawing.Size(52, 19);
            CHK_RMinWeight.TabIndex = 68;
            CHK_RMinWeight.Text = "Used";
            CHK_RMinWeight.UseVisualStyleBackColor = true;
            CHK_RMinWeight.CheckedChanged += CHK_RUsed_CheckedChanged;
            // 
            // CHK_RMaxHeight
            // 
            CHK_RMaxHeight.AutoSize = true;
            CHK_RMaxHeight.Location = new System.Drawing.Point(203, 68);
            CHK_RMaxHeight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_RMaxHeight.Name = "CHK_RMaxHeight";
            CHK_RMaxHeight.Size = new System.Drawing.Size(52, 19);
            CHK_RMaxHeight.TabIndex = 67;
            CHK_RMaxHeight.Text = "Used";
            CHK_RMaxHeight.UseVisualStyleBackColor = true;
            CHK_RMaxHeight.CheckedChanged += CHK_RUsed_CheckedChanged;
            // 
            // CHK_RMinHeight
            // 
            CHK_RMinHeight.AutoSize = true;
            CHK_RMinHeight.Location = new System.Drawing.Point(203, 47);
            CHK_RMinHeight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_RMinHeight.Name = "CHK_RMinHeight";
            CHK_RMinHeight.Size = new System.Drawing.Size(52, 19);
            CHK_RMinHeight.TabIndex = 66;
            CHK_RMinHeight.Text = "Used";
            CHK_RMinHeight.UseVisualStyleBackColor = true;
            CHK_RMinHeight.CheckedChanged += CHK_RUsed_CheckedChanged;
            // 
            // L_RWeight
            // 
            L_RWeight.Location = new System.Drawing.Point(122, 17);
            L_RWeight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_RWeight.Name = "L_RWeight";
            L_RWeight.Size = new System.Drawing.Size(85, 23);
            L_RWeight.TabIndex = 65;
            L_RWeight.Text = "Weight";
            L_RWeight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // L_RHeight
            // 
            L_RHeight.Location = new System.Drawing.Point(23, 17);
            L_RHeight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_RHeight.Name = "L_RHeight";
            L_RHeight.Size = new System.Drawing.Size(86, 23);
            L_RHeight.TabIndex = 64;
            L_RHeight.Text = "Height";
            L_RHeight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // L_RWeightMax
            // 
            L_RWeightMax.Location = new System.Drawing.Point(107, 111);
            L_RWeightMax.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_RWeightMax.Name = "L_RWeightMax";
            L_RWeightMax.Size = new System.Drawing.Size(35, 23);
            L_RWeightMax.TabIndex = 63;
            L_RWeightMax.Text = "Max";
            L_RWeightMax.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_RHeightMax
            // 
            L_RHeightMax.Location = new System.Drawing.Point(8, 65);
            L_RHeightMax.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_RHeightMax.Name = "L_RHeightMax";
            L_RHeightMax.Size = new System.Drawing.Size(35, 23);
            L_RHeightMax.TabIndex = 62;
            L_RHeightMax.Text = "Max";
            L_RHeightMax.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_RWeightMin
            // 
            L_RWeightMin.Location = new System.Drawing.Point(107, 88);
            L_RWeightMin.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_RWeightMin.Name = "L_RWeightMin";
            L_RWeightMin.Size = new System.Drawing.Size(35, 23);
            L_RWeightMin.TabIndex = 61;
            L_RWeightMin.Text = "Min";
            L_RWeightMin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_RHeightMin
            // 
            L_RHeightMin.Location = new System.Drawing.Point(8, 42);
            L_RHeightMin.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_RHeightMin.Name = "L_RHeightMin";
            L_RHeightMin.Size = new System.Drawing.Size(35, 23);
            L_RHeightMin.TabIndex = 60;
            L_RHeightMin.Text = "Min";
            L_RHeightMin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_RWeightMax
            // 
            NUD_RWeightMax.Location = new System.Drawing.Point(145, 113);
            NUD_RWeightMax.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_RWeightMax.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_RWeightMax.Name = "NUD_RWeightMax";
            NUD_RWeightMax.Size = new System.Drawing.Size(47, 23);
            NUD_RWeightMax.TabIndex = 59;
            NUD_RWeightMax.Value = new decimal(new int[] { 255, 0, 0, 0 });
            // 
            // NUD_RWeightMin
            // 
            NUD_RWeightMin.Location = new System.Drawing.Point(145, 90);
            NUD_RWeightMin.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_RWeightMin.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_RWeightMin.Name = "NUD_RWeightMin";
            NUD_RWeightMin.Size = new System.Drawing.Size(47, 23);
            NUD_RWeightMin.TabIndex = 58;
            NUD_RWeightMin.Value = new decimal(new int[] { 255, 0, 0, 0 });
            // 
            // NUD_RHeightMaxWeight
            // 
            NUD_RHeightMaxWeight.Location = new System.Drawing.Point(145, 67);
            NUD_RHeightMaxWeight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_RHeightMaxWeight.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_RHeightMaxWeight.Name = "NUD_RHeightMaxWeight";
            NUD_RHeightMaxWeight.Size = new System.Drawing.Size(47, 23);
            NUD_RHeightMaxWeight.TabIndex = 57;
            NUD_RHeightMaxWeight.Value = new decimal(new int[] { 255, 0, 0, 0 });
            // 
            // NUD_RHeightMinWeight
            // 
            NUD_RHeightMinWeight.Location = new System.Drawing.Point(145, 44);
            NUD_RHeightMinWeight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_RHeightMinWeight.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_RHeightMinWeight.Name = "NUD_RHeightMinWeight";
            NUD_RHeightMinWeight.Size = new System.Drawing.Size(47, 23);
            NUD_RHeightMinWeight.TabIndex = 56;
            NUD_RHeightMinWeight.Value = new decimal(new int[] { 255, 0, 0, 0 });
            // 
            // NUD_RWeightMaxHeight
            // 
            NUD_RWeightMaxHeight.Location = new System.Drawing.Point(46, 113);
            NUD_RWeightMaxHeight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_RWeightMaxHeight.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_RWeightMaxHeight.Name = "NUD_RWeightMaxHeight";
            NUD_RWeightMaxHeight.Size = new System.Drawing.Size(47, 23);
            NUD_RWeightMaxHeight.TabIndex = 55;
            NUD_RWeightMaxHeight.Value = new decimal(new int[] { 255, 0, 0, 0 });
            // 
            // NUD_RWeightMinHeight
            // 
            NUD_RWeightMinHeight.Location = new System.Drawing.Point(46, 90);
            NUD_RWeightMinHeight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_RWeightMinHeight.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_RWeightMinHeight.Name = "NUD_RWeightMinHeight";
            NUD_RWeightMinHeight.Size = new System.Drawing.Size(47, 23);
            NUD_RWeightMinHeight.TabIndex = 54;
            NUD_RWeightMinHeight.Value = new decimal(new int[] { 255, 0, 0, 0 });
            // 
            // NUD_RHeightMax
            // 
            NUD_RHeightMax.Location = new System.Drawing.Point(46, 67);
            NUD_RHeightMax.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_RHeightMax.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_RHeightMax.Name = "NUD_RHeightMax";
            NUD_RHeightMax.Size = new System.Drawing.Size(47, 23);
            NUD_RHeightMax.TabIndex = 53;
            NUD_RHeightMax.Value = new decimal(new int[] { 255, 0, 0, 0 });
            // 
            // NUD_RHeightMin
            // 
            NUD_RHeightMin.Location = new System.Drawing.Point(46, 44);
            NUD_RHeightMin.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_RHeightMin.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_RHeightMin.Name = "NUD_RHeightMin";
            NUD_RHeightMin.Size = new System.Drawing.Size(47, 23);
            NUD_RHeightMin.TabIndex = 52;
            NUD_RHeightMin.Value = new decimal(new int[] { 255, 0, 0, 0 });
            // 
            // B_Counts
            // 
            B_Counts.Location = new System.Drawing.Point(285, 13);
            B_Counts.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Counts.Name = "B_Counts";
            B_Counts.Size = new System.Drawing.Size(113, 27);
            B_Counts.TabIndex = 53;
            B_Counts.Text = "Counts";
            B_Counts.UseVisualStyleBackColor = true;
            B_Counts.Click += B_Counts_Click;
            // 
            // CHK_MinH
            // 
            CHK_MinH.AutoSize = true;
            CHK_MinH.Location = new System.Drawing.Point(98, 47);
            CHK_MinH.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_MinH.Name = "CHK_MinH";
            CHK_MinH.Size = new System.Drawing.Size(15, 14);
            CHK_MinH.TabIndex = 70;
            CHK_MinH.UseVisualStyleBackColor = true;
            // 
            // CHK_MaxH
            // 
            CHK_MaxH.AutoSize = true;
            CHK_MaxH.Location = new System.Drawing.Point(98, 70);
            CHK_MaxH.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_MaxH.Name = "CHK_MaxH";
            CHK_MaxH.Size = new System.Drawing.Size(15, 14);
            CHK_MaxH.TabIndex = 71;
            CHK_MaxH.UseVisualStyleBackColor = true;
            // 
            // CHK_MinW
            // 
            CHK_MinW.AutoSize = true;
            CHK_MinW.Location = new System.Drawing.Point(98, 93);
            CHK_MinW.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_MinW.Name = "CHK_MinW";
            CHK_MinW.Size = new System.Drawing.Size(15, 14);
            CHK_MinW.TabIndex = 72;
            CHK_MinW.UseVisualStyleBackColor = true;
            // 
            // CHK_MaxW
            // 
            CHK_MaxW.AutoSize = true;
            CHK_MaxW.Location = new System.Drawing.Point(98, 116);
            CHK_MaxW.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_MaxW.Name = "CHK_MaxW";
            CHK_MaxW.Size = new System.Drawing.Size(15, 14);
            CHK_MaxW.TabIndex = 73;
            CHK_MaxW.UseVisualStyleBackColor = true;
            // 
            // SAV_PokedexGG
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(486, 488);
            Controls.Add(B_Counts);
            Controls.Add(GB_SizeRecords);
            Controls.Add(LB_Forms);
            Controls.Add(GB_Displayed);
            Controls.Add(GB_Owned);
            Controls.Add(GB_Encountered);
            Controls.Add(GB_Language);
            Controls.Add(B_Modify);
            Controls.Add(B_Save);
            Controls.Add(B_GiveAll);
            Controls.Add(CB_Species);
            Controls.Add(L_goto);
            Controls.Add(LB_Species);
            Controls.Add(B_Cancel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_PokedexGG";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Pokédex Editor";
            GB_Language.ResumeLayout(false);
            GB_Language.PerformLayout();
            GB_Encountered.ResumeLayout(false);
            GB_Encountered.PerformLayout();
            GB_Owned.ResumeLayout(false);
            GB_Owned.PerformLayout();
            GB_Displayed.ResumeLayout(false);
            GB_Displayed.PerformLayout();
            modifyMenu.ResumeLayout(false);
            GB_SizeRecords.ResumeLayout(false);
            GB_SizeRecords.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_RWeightMax).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_RWeightMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_RHeightMaxWeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_RHeightMinWeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_RWeightMaxHeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_RWeightMinHeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_RHeightMax).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_RHeightMin).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.ListBox LB_Species;
        private System.Windows.Forms.CheckBox CHK_P1;
        private System.Windows.Forms.CheckBox CHK_P2;
        private System.Windows.Forms.CheckBox CHK_P3;
        private System.Windows.Forms.CheckBox CHK_P4;
        private System.Windows.Forms.CheckBox CHK_P5;
        private System.Windows.Forms.CheckBox CHK_P6;
        private System.Windows.Forms.CheckBox CHK_P7;
        private System.Windows.Forms.CheckBox CHK_P8;
        private System.Windows.Forms.CheckBox CHK_P9;
        private System.Windows.Forms.CheckBox CHK_L7;
        private System.Windows.Forms.CheckBox CHK_L6;
        private System.Windows.Forms.CheckBox CHK_L5;
        private System.Windows.Forms.CheckBox CHK_L4;
        private System.Windows.Forms.CheckBox CHK_L3;
        private System.Windows.Forms.CheckBox CHK_L2;
        private System.Windows.Forms.CheckBox CHK_L1;
        private System.Windows.Forms.Label L_goto;
        private System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.Button B_GiveAll;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Modify;
        private System.Windows.Forms.GroupBox GB_Language;
        private System.Windows.Forms.GroupBox GB_Encountered;
        private System.Windows.Forms.GroupBox GB_Owned;
        private System.Windows.Forms.GroupBox GB_Displayed;
        private System.Windows.Forms.ContextMenuStrip modifyMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuSeenNone;
        private System.Windows.Forms.ToolStripMenuItem mnuSeenAll;
        private System.Windows.Forms.ToolStripMenuItem mnuCaughtNone;
        private System.Windows.Forms.ToolStripMenuItem mnuCaughtAll;
        private System.Windows.Forms.ToolStripMenuItem mnuComplete;
        private System.Windows.Forms.ToolStripMenuItem mnuFormNone;
        private System.Windows.Forms.ToolStripMenuItem mnuForm1;
        private System.Windows.Forms.ToolStripMenuItem mnuFormAll;
        private System.Windows.Forms.CheckBox CHK_L8;
        private System.Windows.Forms.CheckBox CHK_L9;
        private System.Windows.Forms.ListBox LB_Forms;
        private System.Windows.Forms.GroupBox GB_SizeRecords;
        private System.Windows.Forms.CheckBox CHK_RMaxWeight;
        private System.Windows.Forms.CheckBox CHK_RMinWeight;
        private System.Windows.Forms.CheckBox CHK_RMaxHeight;
        private System.Windows.Forms.CheckBox CHK_RMinHeight;
        private System.Windows.Forms.Label L_RWeight;
        private System.Windows.Forms.Label L_RHeight;
        private System.Windows.Forms.Label L_RWeightMax;
        private System.Windows.Forms.Label L_RHeightMax;
        private System.Windows.Forms.Label L_RWeightMin;
        private System.Windows.Forms.Label L_RHeightMin;
        private System.Windows.Forms.NumericUpDown NUD_RWeightMax;
        private System.Windows.Forms.NumericUpDown NUD_RWeightMin;
        private System.Windows.Forms.NumericUpDown NUD_RHeightMaxWeight;
        private System.Windows.Forms.NumericUpDown NUD_RHeightMinWeight;
        private System.Windows.Forms.NumericUpDown NUD_RWeightMaxHeight;
        private System.Windows.Forms.NumericUpDown NUD_RWeightMinHeight;
        private System.Windows.Forms.NumericUpDown NUD_RHeightMax;
        private System.Windows.Forms.NumericUpDown NUD_RHeightMin;
        private System.Windows.Forms.Button B_Counts;
        private System.Windows.Forms.CheckBox CHK_MaxW;
        private System.Windows.Forms.CheckBox CHK_MinW;
        private System.Windows.Forms.CheckBox CHK_MaxH;
        private System.Windows.Forms.CheckBox CHK_MinH;
    }
}
