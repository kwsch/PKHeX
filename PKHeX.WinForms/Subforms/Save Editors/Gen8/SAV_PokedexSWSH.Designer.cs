namespace PKHeX.WinForms
{
    partial class SAV_PokedexSWSH
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
            CHK_Caught = new System.Windows.Forms.CheckBox();
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
            modifyMenu = new System.Windows.Forms.ContextMenuStrip(components);
            mnuSeenNone = new System.Windows.Forms.ToolStripMenuItem();
            mnuSeenAll = new System.Windows.Forms.ToolStripMenuItem();
            mnuCaughtNone = new System.Windows.Forms.ToolStripMenuItem();
            mnuCaughtAll = new System.Windows.Forms.ToolStripMenuItem();
            mnuComplete = new System.Windows.Forms.ToolStripMenuItem();
            mnuBattleCount = new System.Windows.Forms.ToolStripMenuItem();
            mnuFormNone = new System.Windows.Forms.ToolStripMenuItem();
            mnuForm1 = new System.Windows.Forms.ToolStripMenuItem();
            mnuFormAll = new System.Windows.Forms.ToolStripMenuItem();
            NUD_Battled = new System.Windows.Forms.NumericUpDown();
            L_Battled = new System.Windows.Forms.Label();
            L_DisplayedForm = new System.Windows.Forms.Label();
            GB_Displayed = new System.Windows.Forms.GroupBox();
            CB_Gender = new System.Windows.Forms.ComboBox();
            CHK_S = new System.Windows.Forms.CheckBox();
            CHK_G = new System.Windows.Forms.CheckBox();
            CHK_Gigantamaxed = new System.Windows.Forms.CheckBox();
            NUD_Form = new System.Windows.Forms.NumericUpDown();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            L_Male = new System.Windows.Forms.Label();
            L_Female = new System.Windows.Forms.Label();
            L_MaleShiny = new System.Windows.Forms.Label();
            L_FemaleShiny = new System.Windows.Forms.Label();
            CLB_3 = new System.Windows.Forms.CheckedListBox();
            CLB_4 = new System.Windows.Forms.CheckedListBox();
            CLB_1 = new System.Windows.Forms.CheckedListBox();
            CLB_2 = new System.Windows.Forms.CheckedListBox();
            CHK_Gigantamaxed1 = new System.Windows.Forms.CheckBox();
            GB_Language.SuspendLayout();
            modifyMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Battled).BeginInit();
            GB_Displayed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Form).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(749, 486);
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
            LB_Species.Size = new System.Drawing.Size(186, 454);
            LB_Species.TabIndex = 2;
            LB_Species.SelectedIndexChanged += ChangeLBSpecies;
            // 
            // CHK_Caught
            // 
            CHK_Caught.AutoSize = true;
            CHK_Caught.Location = new System.Drawing.Point(331, 17);
            CHK_Caught.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_Caught.Name = "CHK_Caught";
            CHK_Caught.Size = new System.Drawing.Size(64, 19);
            CHK_Caught.TabIndex = 3;
            CHK_Caught.Text = "Owned";
            CHK_Caught.UseVisualStyleBackColor = true;
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
            B_Save.Location = new System.Drawing.Point(749, 452);
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
            B_Modify.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            B_Modify.Location = new System.Drawing.Point(700, 10);
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
            GB_Language.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            GB_Language.Controls.Add(CHK_L9);
            GB_Language.Controls.Add(CHK_L8);
            GB_Language.Controls.Add(CHK_L7);
            GB_Language.Controls.Add(CHK_L6);
            GB_Language.Controls.Add(CHK_L5);
            GB_Language.Controls.Add(CHK_L4);
            GB_Language.Controls.Add(CHK_L3);
            GB_Language.Controls.Add(CHK_L2);
            GB_Language.Controls.Add(CHK_L1);
            GB_Language.Location = new System.Drawing.Point(700, 46);
            GB_Language.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Language.Name = "GB_Language";
            GB_Language.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Language.Size = new System.Drawing.Size(142, 198);
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
            // modifyMenu
            // 
            modifyMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            modifyMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuSeenNone, mnuSeenAll, mnuCaughtNone, mnuCaughtAll, mnuComplete, mnuBattleCount });
            modifyMenu.Name = "modifyMenu";
            modifyMenu.Size = new System.Drawing.Size(202, 136);
            // 
            // mnuSeenNone
            // 
            mnuSeenNone.Name = "mnuSeenNone";
            mnuSeenNone.Size = new System.Drawing.Size(201, 22);
            mnuSeenNone.Text = "Seen none";
            mnuSeenNone.Click += SeenNone;
            // 
            // mnuSeenAll
            // 
            mnuSeenAll.Name = "mnuSeenAll";
            mnuSeenAll.Size = new System.Drawing.Size(201, 22);
            mnuSeenAll.Text = "Seen all";
            mnuSeenAll.Click += SeenAll;
            // 
            // mnuCaughtNone
            // 
            mnuCaughtNone.Name = "mnuCaughtNone";
            mnuCaughtNone.Size = new System.Drawing.Size(201, 22);
            mnuCaughtNone.Text = "Caught none";
            mnuCaughtNone.Click += CaughtNone;
            // 
            // mnuCaughtAll
            // 
            mnuCaughtAll.Name = "mnuCaughtAll";
            mnuCaughtAll.Size = new System.Drawing.Size(201, 22);
            mnuCaughtAll.Text = "Caught all";
            mnuCaughtAll.Click += CaughtAll;
            // 
            // mnuComplete
            // 
            mnuComplete.Name = "mnuComplete";
            mnuComplete.Size = new System.Drawing.Size(201, 22);
            mnuComplete.Text = "Complete Dex";
            mnuComplete.Click += CompleteDex;
            // 
            // mnuBattleCount
            // 
            mnuBattleCount.Name = "mnuBattleCount";
            mnuBattleCount.Size = new System.Drawing.Size(201, 22);
            mnuBattleCount.Text = "Change All Battle Count";
            mnuBattleCount.Click += ChangeAllCounts;
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
            // NUD_Battled
            // 
            NUD_Battled.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            NUD_Battled.Location = new System.Drawing.Point(704, 413);
            NUD_Battled.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_Battled.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            NUD_Battled.Name = "NUD_Battled";
            NUD_Battled.Size = new System.Drawing.Size(132, 23);
            NUD_Battled.TabIndex = 28;
            // 
            // L_Battled
            // 
            L_Battled.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            L_Battled.AutoSize = true;
            L_Battled.Location = new System.Drawing.Point(700, 395);
            L_Battled.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Battled.Name = "L_Battled";
            L_Battled.Size = new System.Drawing.Size(47, 15);
            L_Battled.TabIndex = 29;
            L_Battled.Text = "Battled:";
            // 
            // L_DisplayedForm
            // 
            L_DisplayedForm.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            L_DisplayedForm.AutoSize = true;
            L_DisplayedForm.Location = new System.Drawing.Point(700, 253);
            L_DisplayedForm.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_DisplayedForm.Name = "L_DisplayedForm";
            L_DisplayedForm.Size = new System.Drawing.Size(92, 15);
            L_DisplayedForm.TabIndex = 32;
            L_DisplayedForm.Text = "Displayed Form:";
            // 
            // GB_Displayed
            // 
            GB_Displayed.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            GB_Displayed.Controls.Add(CB_Gender);
            GB_Displayed.Controls.Add(CHK_S);
            GB_Displayed.Controls.Add(CHK_G);
            GB_Displayed.Location = new System.Drawing.Point(700, 302);
            GB_Displayed.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Displayed.Name = "GB_Displayed";
            GB_Displayed.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Displayed.Size = new System.Drawing.Size(142, 88);
            GB_Displayed.TabIndex = 33;
            GB_Displayed.TabStop = false;
            GB_Displayed.Text = "Displayed";
            // 
            // CB_Gender
            // 
            CB_Gender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Gender.FormattingEnabled = true;
            CB_Gender.Items.AddRange(new object[] { "♂", "♀", "-" });
            CB_Gender.Location = new System.Drawing.Point(7, 57);
            CB_Gender.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Gender.Name = "CB_Gender";
            CB_Gender.Size = new System.Drawing.Size(46, 23);
            CB_Gender.TabIndex = 23;
            // 
            // CHK_S
            // 
            CHK_S.AutoSize = true;
            CHK_S.Location = new System.Drawing.Point(6, 31);
            CHK_S.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_S.Name = "CHK_S";
            CHK_S.Size = new System.Drawing.Size(55, 19);
            CHK_S.TabIndex = 9;
            CHK_S.Text = "Shiny";
            CHK_S.UseVisualStyleBackColor = true;
            // 
            // CHK_G
            // 
            CHK_G.AutoSize = true;
            CHK_G.Location = new System.Drawing.Point(6, 15);
            CHK_G.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_G.Name = "CHK_G";
            CHK_G.Size = new System.Drawing.Size(90, 19);
            CHK_G.TabIndex = 8;
            CHK_G.Text = "Gigantamax";
            CHK_G.UseVisualStyleBackColor = true;
            // 
            // CHK_Gigantamaxed
            // 
            CHK_Gigantamaxed.AutoSize = true;
            CHK_Gigantamaxed.Location = new System.Drawing.Point(453, 17);
            CHK_Gigantamaxed.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_Gigantamaxed.Name = "CHK_Gigantamaxed";
            CHK_Gigantamaxed.Size = new System.Drawing.Size(103, 19);
            CHK_Gigantamaxed.TabIndex = 34;
            CHK_Gigantamaxed.Text = "Gigantamaxed";
            CHK_Gigantamaxed.UseVisualStyleBackColor = true;
            // 
            // NUD_Form
            // 
            NUD_Form.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            NUD_Form.Location = new System.Drawing.Point(704, 272);
            NUD_Form.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_Form.Name = "NUD_Form";
            NUD_Form.Size = new System.Drawing.Size(132, 23);
            NUD_Form.TabIndex = 35;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            tableLayoutPanel1.Controls.Add(L_Male, 0, 0);
            tableLayoutPanel1.Controls.Add(L_Female, 1, 0);
            tableLayoutPanel1.Controls.Add(L_MaleShiny, 2, 0);
            tableLayoutPanel1.Controls.Add(L_FemaleShiny, 3, 0);
            tableLayoutPanel1.Controls.Add(CLB_3, 0, 1);
            tableLayoutPanel1.Controls.Add(CLB_4, 0, 1);
            tableLayoutPanel1.Controls.Add(CLB_1, 0, 1);
            tableLayoutPanel1.Controls.Add(CLB_2, 0, 1);
            tableLayoutPanel1.Location = new System.Drawing.Point(208, 46);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            tableLayoutPanel1.Size = new System.Drawing.Size(485, 457);
            tableLayoutPanel1.TabIndex = 36;
            // 
            // L_Male
            // 
            L_Male.AutoSize = true;
            L_Male.Dock = System.Windows.Forms.DockStyle.Fill;
            L_Male.Location = new System.Drawing.Point(4, 0);
            L_Male.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Male.Name = "L_Male";
            L_Male.Size = new System.Drawing.Size(113, 23);
            L_Male.TabIndex = 37;
            L_Male.Text = "Male";
            L_Male.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // L_Female
            // 
            L_Female.AutoSize = true;
            L_Female.Dock = System.Windows.Forms.DockStyle.Fill;
            L_Female.Location = new System.Drawing.Point(125, 0);
            L_Female.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Female.Name = "L_Female";
            L_Female.Size = new System.Drawing.Size(113, 23);
            L_Female.TabIndex = 38;
            L_Female.Text = "Female";
            L_Female.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // L_MaleShiny
            // 
            L_MaleShiny.AutoSize = true;
            L_MaleShiny.Dock = System.Windows.Forms.DockStyle.Fill;
            L_MaleShiny.Location = new System.Drawing.Point(246, 0);
            L_MaleShiny.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_MaleShiny.Name = "L_MaleShiny";
            L_MaleShiny.Size = new System.Drawing.Size(113, 23);
            L_MaleShiny.TabIndex = 39;
            L_MaleShiny.Text = "*Male*";
            L_MaleShiny.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // L_FemaleShiny
            // 
            L_FemaleShiny.AutoSize = true;
            L_FemaleShiny.Dock = System.Windows.Forms.DockStyle.Fill;
            L_FemaleShiny.Location = new System.Drawing.Point(367, 0);
            L_FemaleShiny.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_FemaleShiny.Name = "L_FemaleShiny";
            L_FemaleShiny.Size = new System.Drawing.Size(114, 23);
            L_FemaleShiny.TabIndex = 40;
            L_FemaleShiny.Text = "*Female*";
            L_FemaleShiny.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CLB_3
            // 
            CLB_3.Dock = System.Windows.Forms.DockStyle.Fill;
            CLB_3.FormattingEnabled = true;
            CLB_3.Location = new System.Drawing.Point(243, 24);
            CLB_3.Margin = new System.Windows.Forms.Padding(1);
            CLB_3.Name = "CLB_3";
            CLB_3.Size = new System.Drawing.Size(119, 432);
            CLB_3.TabIndex = 34;
            // 
            // CLB_4
            // 
            CLB_4.Dock = System.Windows.Forms.DockStyle.Fill;
            CLB_4.FormattingEnabled = true;
            CLB_4.Location = new System.Drawing.Point(364, 24);
            CLB_4.Margin = new System.Windows.Forms.Padding(1);
            CLB_4.Name = "CLB_4";
            CLB_4.Size = new System.Drawing.Size(120, 432);
            CLB_4.TabIndex = 33;
            // 
            // CLB_1
            // 
            CLB_1.Dock = System.Windows.Forms.DockStyle.Fill;
            CLB_1.FormattingEnabled = true;
            CLB_1.Location = new System.Drawing.Point(1, 24);
            CLB_1.Margin = new System.Windows.Forms.Padding(1);
            CLB_1.Name = "CLB_1";
            CLB_1.Size = new System.Drawing.Size(119, 432);
            CLB_1.TabIndex = 32;
            // 
            // CLB_2
            // 
            CLB_2.Dock = System.Windows.Forms.DockStyle.Fill;
            CLB_2.FormattingEnabled = true;
            CLB_2.Location = new System.Drawing.Point(122, 24);
            CLB_2.Margin = new System.Windows.Forms.Padding(1);
            CLB_2.Name = "CLB_2";
            CLB_2.Size = new System.Drawing.Size(119, 432);
            CLB_2.TabIndex = 30;
            // 
            // CHK_Gigantamaxed1
            // 
            CHK_Gigantamaxed1.AutoSize = true;
            CHK_Gigantamaxed1.Location = new System.Drawing.Point(573, 17);
            CHK_Gigantamaxed1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_Gigantamaxed1.Name = "CHK_Gigantamaxed1";
            CHK_Gigantamaxed1.Size = new System.Drawing.Size(112, 19);
            CHK_Gigantamaxed1.TabIndex = 37;
            CHK_Gigantamaxed1.Text = "Gigantamaxed 1";
            CHK_Gigantamaxed1.UseVisualStyleBackColor = true;
            // 
            // SAV_PokedexSWSH
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(856, 524);
            Controls.Add(CHK_Gigantamaxed1);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(NUD_Form);
            Controls.Add(CHK_Gigantamaxed);
            Controls.Add(GB_Displayed);
            Controls.Add(L_DisplayedForm);
            Controls.Add(L_Battled);
            Controls.Add(NUD_Battled);
            Controls.Add(CHK_Caught);
            Controls.Add(GB_Language);
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
            MinimumSize = new System.Drawing.Size(872, 562);
            Name = "SAV_PokedexSWSH";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Pokédex Editor";
            GB_Language.ResumeLayout(false);
            GB_Language.PerformLayout();
            modifyMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)NUD_Battled).EndInit();
            GB_Displayed.ResumeLayout(false);
            GB_Displayed.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Form).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.ListBox LB_Species;
        private System.Windows.Forms.CheckBox CHK_Caught;
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
        private System.Windows.Forms.NumericUpDown NUD_Battled;
        private System.Windows.Forms.Label L_Battled;
        private System.Windows.Forms.Label L_DisplayedForm;
        private System.Windows.Forms.GroupBox GB_Displayed;
        private System.Windows.Forms.CheckBox CHK_S;
        private System.Windows.Forms.CheckBox CHK_G;
        private System.Windows.Forms.CheckBox CHK_Gigantamaxed;
        private System.Windows.Forms.NumericUpDown NUD_Form;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label L_Male;
        private System.Windows.Forms.Label L_Female;
        private System.Windows.Forms.Label L_MaleShiny;
        private System.Windows.Forms.Label L_FemaleShiny;
        private System.Windows.Forms.CheckedListBox CLB_3;
        private System.Windows.Forms.CheckedListBox CLB_4;
        private System.Windows.Forms.CheckedListBox CLB_1;
        private System.Windows.Forms.CheckedListBox CLB_2;
        private System.Windows.Forms.ComboBox CB_Gender;
        private System.Windows.Forms.ToolStripMenuItem mnuBattleCount;
        private System.Windows.Forms.CheckBox CHK_Gigantamaxed1;
    }
}
