namespace PKHeX.WinForms.Controls
{
    partial class EntitySearchControl
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            CB_Ability = new System.Windows.Forms.ComboBox();
            CB_HeldItem = new System.Windows.Forms.ComboBox();
            CB_Nature = new System.Windows.Forms.ComboBox();
            CB_Species = new System.Windows.Forms.ComboBox();
            TB_Nickname = new System.Windows.Forms.TextBox();
            CB_Move4 = new System.Windows.Forms.ComboBox();
            CB_Move3 = new System.Windows.Forms.ComboBox();
            CB_Move2 = new System.Windows.Forms.ComboBox();
            CB_Move1 = new System.Windows.Forms.ComboBox();
            TB_Level = new System.Windows.Forms.MaskedTextBox();
            Label_CurLevel = new System.Windows.Forms.Label();
            Label_HeldItem = new System.Windows.Forms.Label();
            Label_Ability = new System.Windows.Forms.Label();
            Label_Nature = new System.Windows.Forms.Label();
            Label_Species = new System.Windows.Forms.Label();
            Label_Nickname = new System.Windows.Forms.Label();
            CB_EVTrain = new System.Windows.Forms.ComboBox();
            CB_HPType = new System.Windows.Forms.ComboBox();
            Label_HiddenPowerPrefix = new System.Windows.Forms.Label();
            CB_GameOrigin = new System.Windows.Forms.ComboBox();
            CB_IV = new System.Windows.Forms.ComboBox();
            CB_Level = new System.Windows.Forms.ComboBox();
            L_Version = new System.Windows.Forms.Label();
            L_Move1 = new System.Windows.Forms.Label();
            L_Move2 = new System.Windows.Forms.Label();
            L_Move3 = new System.Windows.Forms.Label();
            L_Move4 = new System.Windows.Forms.Label();
            L_Potential = new System.Windows.Forms.Label();
            L_EVTraining = new System.Windows.Forms.Label();
            L_Generation = new System.Windows.Forms.Label();
            CB_Generation = new System.Windows.Forms.ComboBox();
            FLP_Egg = new System.Windows.Forms.FlowLayoutPanel();
            CHK_IsEgg = new System.Windows.Forms.CheckBox();
            L_ESV = new System.Windows.Forms.Label();
            MT_ESV = new System.Windows.Forms.MaskedTextBox();
            CHK_Shiny = new System.Windows.Forms.CheckBox();
            TLP_Filters = new System.Windows.Forms.TableLayoutPanel();
            FLP_Format = new System.Windows.Forms.FlowLayoutPanel();
            CB_FormatComparator = new System.Windows.Forms.ComboBox();
            CB_Format = new System.Windows.Forms.ComboBox();
            L_Format = new System.Windows.Forms.Label();
            FLP_Level = new System.Windows.Forms.FlowLayoutPanel();
            FLP_Egg.SuspendLayout();
            TLP_Filters.SuspendLayout();
            FLP_Format.SuspendLayout();
            FLP_Level.SuspendLayout();
            SuspendLayout();
            // 
            // CB_Ability
            // 
            CB_Ability.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_Ability.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Ability.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Ability.FormattingEnabled = true;
            CB_Ability.Items.AddRange(new object[] { "Item" });
            CB_Ability.Location = new System.Drawing.Point(101, 130);
            CB_Ability.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            CB_Ability.Name = "CB_Ability";
            CB_Ability.Size = new System.Drawing.Size(142, 25);
            CB_Ability.TabIndex = 4;
            // 
            // CB_HeldItem
            // 
            CB_HeldItem.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_HeldItem.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_HeldItem.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_HeldItem.FormattingEnabled = true;
            CB_HeldItem.Location = new System.Drawing.Point(101, 104);
            CB_HeldItem.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            CB_HeldItem.Name = "CB_HeldItem";
            CB_HeldItem.Size = new System.Drawing.Size(142, 25);
            CB_HeldItem.TabIndex = 3;
            // 
            // CB_Nature
            // 
            CB_Nature.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_Nature.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Nature.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Nature.FormattingEnabled = true;
            CB_Nature.Location = new System.Drawing.Point(101, 78);
            CB_Nature.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            CB_Nature.Name = "CB_Nature";
            CB_Nature.Size = new System.Drawing.Size(142, 25);
            CB_Nature.TabIndex = 2;
            // 
            // CB_Species
            // 
            CB_Species.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_Species.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Species.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Species.FormattingEnabled = true;
            CB_Species.Location = new System.Drawing.Point(101, 26);
            CB_Species.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            CB_Species.Name = "CB_Species";
            CB_Species.Size = new System.Drawing.Size(142, 25);
            CB_Species.TabIndex = 0;
            // 
            // TB_Nickname
            // 
            TB_Nickname.Anchor = System.Windows.Forms.AnchorStyles.Left;
            TB_Nickname.Location = new System.Drawing.Point(101, 52);
            TB_Nickname.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            TB_Nickname.Name = "TB_Nickname";
            TB_Nickname.Size = new System.Drawing.Size(142, 25);
            TB_Nickname.TabIndex = 1;
            // 
            // CB_Move4
            // 
            CB_Move4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_Move4.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Move4.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Move4.FormattingEnabled = true;
            CB_Move4.Location = new System.Drawing.Point(101, 338);
            CB_Move4.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            CB_Move4.Name = "CB_Move4";
            CB_Move4.Size = new System.Drawing.Size(142, 25);
            CB_Move4.TabIndex = 13;
            // 
            // CB_Move3
            // 
            CB_Move3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_Move3.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Move3.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Move3.FormattingEnabled = true;
            CB_Move3.Location = new System.Drawing.Point(101, 312);
            CB_Move3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            CB_Move3.Name = "CB_Move3";
            CB_Move3.Size = new System.Drawing.Size(142, 25);
            CB_Move3.TabIndex = 12;
            // 
            // CB_Move2
            // 
            CB_Move2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_Move2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Move2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Move2.FormattingEnabled = true;
            CB_Move2.Location = new System.Drawing.Point(101, 286);
            CB_Move2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            CB_Move2.Name = "CB_Move2";
            CB_Move2.Size = new System.Drawing.Size(142, 25);
            CB_Move2.TabIndex = 11;
            // 
            // CB_Move1
            // 
            CB_Move1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_Move1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Move1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Move1.FormattingEnabled = true;
            CB_Move1.Location = new System.Drawing.Point(101, 260);
            CB_Move1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            CB_Move1.Name = "CB_Move1";
            CB_Move1.Size = new System.Drawing.Size(142, 25);
            CB_Move1.TabIndex = 10;
            // 
            // TB_Level
            // 
            TB_Level.Anchor = System.Windows.Forms.AnchorStyles.Left;
            TB_Level.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_Level.Location = new System.Drawing.Point(0, 0);
            TB_Level.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            TB_Level.Mask = "000";
            TB_Level.Name = "TB_Level";
            TB_Level.Size = new System.Drawing.Size(25, 25);
            TB_Level.TabIndex = 5;
            TB_Level.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Label_CurLevel
            // 
            Label_CurLevel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            Label_CurLevel.AutoSize = true;
            Label_CurLevel.Location = new System.Drawing.Point(57, 160);
            Label_CurLevel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Label_CurLevel.Name = "Label_CurLevel";
            Label_CurLevel.Size = new System.Drawing.Size(40, 17);
            Label_CurLevel.TabIndex = 95;
            Label_CurLevel.Text = "Level:";
            Label_CurLevel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_HeldItem
            // 
            Label_HeldItem.Anchor = System.Windows.Forms.AnchorStyles.Right;
            Label_HeldItem.AutoSize = true;
            Label_HeldItem.Location = new System.Drawing.Point(30, 108);
            Label_HeldItem.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Label_HeldItem.Name = "Label_HeldItem";
            Label_HeldItem.Size = new System.Drawing.Size(67, 17);
            Label_HeldItem.TabIndex = 93;
            Label_HeldItem.Text = "Held Item:";
            Label_HeldItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_Ability
            // 
            Label_Ability.Anchor = System.Windows.Forms.AnchorStyles.Right;
            Label_Ability.AutoSize = true;
            Label_Ability.Location = new System.Drawing.Point(51, 134);
            Label_Ability.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Label_Ability.Name = "Label_Ability";
            Label_Ability.Size = new System.Drawing.Size(46, 17);
            Label_Ability.TabIndex = 94;
            Label_Ability.Text = "Ability:";
            Label_Ability.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_Nature
            // 
            Label_Nature.Anchor = System.Windows.Forms.AnchorStyles.Right;
            Label_Nature.AutoSize = true;
            Label_Nature.Location = new System.Drawing.Point(46, 82);
            Label_Nature.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Label_Nature.Name = "Label_Nature";
            Label_Nature.Size = new System.Drawing.Size(51, 17);
            Label_Nature.TabIndex = 92;
            Label_Nature.Text = "Nature:";
            Label_Nature.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_Species
            // 
            Label_Species.Anchor = System.Windows.Forms.AnchorStyles.Right;
            Label_Species.AutoSize = true;
            Label_Species.Location = new System.Drawing.Point(42, 30);
            Label_Species.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Label_Species.Name = "Label_Species";
            Label_Species.Size = new System.Drawing.Size(55, 17);
            Label_Species.TabIndex = 90;
            Label_Species.Text = "Species:";
            Label_Species.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_Nickname
            // 
            Label_Nickname.Anchor = System.Windows.Forms.AnchorStyles.Right;
            Label_Nickname.AutoSize = true;
            Label_Nickname.Location = new System.Drawing.Point(29, 56);
            Label_Nickname.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Label_Nickname.Name = "Label_Nickname";
            Label_Nickname.Size = new System.Drawing.Size(68, 17);
            Label_Nickname.TabIndex = 91;
            Label_Nickname.Text = "Nickname:";
            Label_Nickname.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_EVTrain
            // 
            CB_EVTrain.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_EVTrain.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_EVTrain.DropDownWidth = 85;
            CB_EVTrain.FormattingEnabled = true;
            CB_EVTrain.Items.AddRange(new object[] { "Any", "None (0)", "Some (127-1)", "Half (128-507)", "Full (508+)" });
            CB_EVTrain.Location = new System.Drawing.Point(101, 208);
            CB_EVTrain.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            CB_EVTrain.Name = "CB_EVTrain";
            CB_EVTrain.Size = new System.Drawing.Size(109, 25);
            CB_EVTrain.TabIndex = 8;
            // 
            // CB_HPType
            // 
            CB_HPType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_HPType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_HPType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_HPType.DropDownWidth = 80;
            CB_HPType.FormattingEnabled = true;
            CB_HPType.Location = new System.Drawing.Point(101, 234);
            CB_HPType.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            CB_HPType.Name = "CB_HPType";
            CB_HPType.Size = new System.Drawing.Size(142, 25);
            CB_HPType.TabIndex = 9;
            // 
            // Label_HiddenPowerPrefix
            // 
            Label_HiddenPowerPrefix.Anchor = System.Windows.Forms.AnchorStyles.Right;
            Label_HiddenPowerPrefix.AutoSize = true;
            Label_HiddenPowerPrefix.Location = new System.Drawing.Point(4, 238);
            Label_HiddenPowerPrefix.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Label_HiddenPowerPrefix.Name = "Label_HiddenPowerPrefix";
            Label_HiddenPowerPrefix.Size = new System.Drawing.Size(93, 17);
            Label_HiddenPowerPrefix.TabIndex = 98;
            Label_HiddenPowerPrefix.Text = "Hidden Power:";
            Label_HiddenPowerPrefix.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_GameOrigin
            // 
            CB_GameOrigin.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_GameOrigin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_GameOrigin.FormattingEnabled = true;
            CB_GameOrigin.Location = new System.Drawing.Point(101, 364);
            CB_GameOrigin.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            CB_GameOrigin.Name = "CB_GameOrigin";
            CB_GameOrigin.Size = new System.Drawing.Size(142, 25);
            CB_GameOrigin.TabIndex = 14;
            CB_GameOrigin.SelectedIndexChanged += ChangeGame;
            // 
            // CB_IV
            // 
            CB_IV.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_IV.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_IV.DropDownWidth = 85;
            CB_IV.FormattingEnabled = true;
            CB_IV.Items.AddRange(new object[] { "Any", "<= 90", "91-120", "121-150", "151-179", "180+", "== 186" });
            CB_IV.Location = new System.Drawing.Point(101, 182);
            CB_IV.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            CB_IV.Name = "CB_IV";
            CB_IV.Size = new System.Drawing.Size(109, 25);
            CB_IV.TabIndex = 7;
            // 
            // CB_Level
            // 
            CB_Level.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_Level.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Level.DropDownWidth = 85;
            CB_Level.FormattingEnabled = true;
            CB_Level.Items.AddRange(new object[] { "Any", "==", ">=", "<=" });
            CB_Level.Location = new System.Drawing.Point(25, 0);
            CB_Level.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            CB_Level.Name = "CB_Level";
            CB_Level.Size = new System.Drawing.Size(76, 25);
            CB_Level.TabIndex = 6;
            CB_Level.SelectedIndexChanged += ChangeLevel;
            // 
            // L_Version
            // 
            L_Version.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Version.AutoSize = true;
            L_Version.Location = new System.Drawing.Point(23, 368);
            L_Version.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            L_Version.Name = "L_Version";
            L_Version.Size = new System.Drawing.Size(74, 17);
            L_Version.TabIndex = 103;
            L_Version.Text = "OT Version:";
            L_Version.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Move1
            // 
            L_Move1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Move1.AutoSize = true;
            L_Move1.Location = new System.Drawing.Point(42, 264);
            L_Move1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            L_Move1.Name = "L_Move1";
            L_Move1.Size = new System.Drawing.Size(55, 17);
            L_Move1.TabIndex = 99;
            L_Move1.Text = "Move 1:";
            L_Move1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Move2
            // 
            L_Move2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Move2.AutoSize = true;
            L_Move2.Location = new System.Drawing.Point(42, 290);
            L_Move2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            L_Move2.Name = "L_Move2";
            L_Move2.Size = new System.Drawing.Size(55, 17);
            L_Move2.TabIndex = 100;
            L_Move2.Text = "Move 2:";
            L_Move2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Move3
            // 
            L_Move3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Move3.AutoSize = true;
            L_Move3.Location = new System.Drawing.Point(42, 316);
            L_Move3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            L_Move3.Name = "L_Move3";
            L_Move3.Size = new System.Drawing.Size(55, 17);
            L_Move3.TabIndex = 101;
            L_Move3.Text = "Move 3:";
            L_Move3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Move4
            // 
            L_Move4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Move4.AutoSize = true;
            L_Move4.Location = new System.Drawing.Point(42, 342);
            L_Move4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            L_Move4.Name = "L_Move4";
            L_Move4.Size = new System.Drawing.Size(55, 17);
            L_Move4.TabIndex = 102;
            L_Move4.Text = "Move 4:";
            L_Move4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Potential
            // 
            L_Potential.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Potential.AutoSize = true;
            L_Potential.Location = new System.Drawing.Point(21, 186);
            L_Potential.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            L_Potential.Name = "L_Potential";
            L_Potential.Size = new System.Drawing.Size(76, 17);
            L_Potential.TabIndex = 96;
            L_Potential.Text = "IV Potential:";
            L_Potential.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_EVTraining
            // 
            L_EVTraining.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_EVTraining.AutoSize = true;
            L_EVTraining.Location = new System.Drawing.Point(21, 212);
            L_EVTraining.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            L_EVTraining.Name = "L_EVTraining";
            L_EVTraining.Size = new System.Drawing.Size(76, 17);
            L_EVTraining.TabIndex = 97;
            L_EVTraining.Text = "EV Training:";
            L_EVTraining.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Generation
            // 
            L_Generation.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Generation.AutoSize = true;
            L_Generation.Location = new System.Drawing.Point(22, 394);
            L_Generation.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            L_Generation.Name = "L_Generation";
            L_Generation.Size = new System.Drawing.Size(75, 17);
            L_Generation.TabIndex = 114;
            L_Generation.Text = "Generation:";
            L_Generation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_Generation
            // 
            CB_Generation.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_Generation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Generation.FormattingEnabled = true;
            CB_Generation.Items.AddRange(new object[] { "Any", "Gen 1 (RBY/GSC)", "Gen 2 (RBY/GSC)", "Gen 3 (RSE/FRLG/CXD)", "Gen 4 (DPPt/HGSS)", "Gen 5 (BW/B2W2)", "Gen 6 (XY/ORAS)", "Gen 7 (SM/USUM/LGPE)", "Gen 8 (SWSH/BDSP/LA)", "Gen 9 (SV)" });
            CB_Generation.Location = new System.Drawing.Point(101, 390);
            CB_Generation.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            CB_Generation.Name = "CB_Generation";
            CB_Generation.Size = new System.Drawing.Size(142, 25);
            CB_Generation.TabIndex = 15;
            CB_Generation.SelectedIndexChanged += ChangeGeneration;
            // 
            // FLP_Egg
            // 
            FLP_Egg.Anchor = System.Windows.Forms.AnchorStyles.Left;
            FLP_Egg.AutoSize = true;
            FLP_Egg.Controls.Add(CHK_IsEgg);
            FLP_Egg.Controls.Add(L_ESV);
            FLP_Egg.Controls.Add(MT_ESV);
            FLP_Egg.Location = new System.Drawing.Point(101, 0);
            FLP_Egg.Margin = new System.Windows.Forms.Padding(0);
            FLP_Egg.Name = "FLP_Egg";
            FLP_Egg.Size = new System.Drawing.Size(136, 26);
            FLP_Egg.TabIndex = 120;
            // 
            // CHK_IsEgg
            // 
            CHK_IsEgg.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CHK_IsEgg.AutoSize = true;
            CHK_IsEgg.Checked = true;
            CHK_IsEgg.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            CHK_IsEgg.Location = new System.Drawing.Point(0, 4);
            CHK_IsEgg.Margin = new System.Windows.Forms.Padding(0, 4, 0, 1);
            CHK_IsEgg.Name = "CHK_IsEgg";
            CHK_IsEgg.Size = new System.Drawing.Size(50, 21);
            CHK_IsEgg.TabIndex = 51;
            CHK_IsEgg.Text = "Egg";
            CHK_IsEgg.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            CHK_IsEgg.ThreeState = true;
            CHK_IsEgg.UseVisualStyleBackColor = true;
            CHK_IsEgg.CheckStateChanged += ToggleESV;
            // 
            // L_ESV
            // 
            L_ESV.Anchor = System.Windows.Forms.AnchorStyles.Left;
            L_ESV.Location = new System.Drawing.Point(50, 3);
            L_ESV.Margin = new System.Windows.Forms.Padding(0);
            L_ESV.Name = "L_ESV";
            L_ESV.Size = new System.Drawing.Size(50, 20);
            L_ESV.TabIndex = 53;
            L_ESV.Text = "ESV:";
            L_ESV.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            L_ESV.Visible = false;
            // 
            // MT_ESV
            // 
            MT_ESV.Anchor = System.Windows.Forms.AnchorStyles.Left;
            MT_ESV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            MT_ESV.Location = new System.Drawing.Point(100, 0);
            MT_ESV.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            MT_ESV.Mask = "0000";
            MT_ESV.Name = "MT_ESV";
            MT_ESV.Size = new System.Drawing.Size(36, 25);
            MT_ESV.TabIndex = 52;
            MT_ESV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            MT_ESV.Visible = false;
            // 
            // CHK_Shiny
            // 
            CHK_Shiny.Anchor = System.Windows.Forms.AnchorStyles.Right;
            CHK_Shiny.AutoSize = true;
            CHK_Shiny.Checked = true;
            CHK_Shiny.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            CHK_Shiny.Location = new System.Drawing.Point(44, 4);
            CHK_Shiny.Margin = new System.Windows.Forms.Padding(0, 4, 0, 1);
            CHK_Shiny.Name = "CHK_Shiny";
            CHK_Shiny.Size = new System.Drawing.Size(57, 21);
            CHK_Shiny.TabIndex = 50;
            CHK_Shiny.Text = "Shiny";
            CHK_Shiny.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            CHK_Shiny.ThreeState = true;
            CHK_Shiny.UseVisualStyleBackColor = true;
            // 
            // TLP_Filters
            // 
            TLP_Filters.AutoScroll = true;
            TLP_Filters.AutoScrollMargin = new System.Drawing.Size(3, 3);
            TLP_Filters.AutoSize = true;
            TLP_Filters.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            TLP_Filters.ColumnCount = 2;
            TLP_Filters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Filters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Filters.Controls.Add(FLP_Format, 1, 16);
            TLP_Filters.Controls.Add(L_Format, 0, 16);
            TLP_Filters.Controls.Add(FLP_Egg, 1, 0);
            TLP_Filters.Controls.Add(CHK_Shiny, 0, 0);
            TLP_Filters.Controls.Add(Label_Species, 0, 1);
            TLP_Filters.Controls.Add(CB_Species, 1, 1);
            TLP_Filters.Controls.Add(Label_Nickname, 0, 2);
            TLP_Filters.Controls.Add(TB_Nickname, 1, 2);
            TLP_Filters.Controls.Add(Label_Nature, 0, 3);
            TLP_Filters.Controls.Add(CB_Nature, 1, 3);
            TLP_Filters.Controls.Add(Label_HeldItem, 0, 4);
            TLP_Filters.Controls.Add(CB_HeldItem, 1, 4);
            TLP_Filters.Controls.Add(Label_Ability, 0, 5);
            TLP_Filters.Controls.Add(CB_Ability, 1, 5);
            TLP_Filters.Controls.Add(FLP_Level, 1, 6);
            TLP_Filters.Controls.Add(Label_CurLevel, 0, 6);
            TLP_Filters.Controls.Add(L_Potential, 0, 7);
            TLP_Filters.Controls.Add(CB_IV, 1, 7);
            TLP_Filters.Controls.Add(L_EVTraining, 0, 8);
            TLP_Filters.Controls.Add(CB_EVTrain, 1, 8);
            TLP_Filters.Controls.Add(Label_HiddenPowerPrefix, 0, 9);
            TLP_Filters.Controls.Add(CB_HPType, 1, 9);
            TLP_Filters.Controls.Add(L_Move1, 0, 10);
            TLP_Filters.Controls.Add(CB_Move1, 1, 10);
            TLP_Filters.Controls.Add(L_Move2, 0, 11);
            TLP_Filters.Controls.Add(CB_Move2, 1, 11);
            TLP_Filters.Controls.Add(L_Move3, 0, 12);
            TLP_Filters.Controls.Add(CB_Move3, 1, 12);
            TLP_Filters.Controls.Add(L_Move4, 0, 13);
            TLP_Filters.Controls.Add(CB_Move4, 1, 13);
            TLP_Filters.Controls.Add(L_Version, 0, 14);
            TLP_Filters.Controls.Add(CB_GameOrigin, 1, 14);
            TLP_Filters.Controls.Add(L_Generation, 0, 15);
            TLP_Filters.Controls.Add(CB_Generation, 1, 15);
            TLP_Filters.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Filters.Location = new System.Drawing.Point(0, 0);
            TLP_Filters.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TLP_Filters.Name = "TLP_Filters";
            TLP_Filters.RowCount = 18;
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Filters.Size = new System.Drawing.Size(243, 442);
            TLP_Filters.TabIndex = 118;
            // 
            // FLP_Format
            // 
            FLP_Format.AutoSize = true;
            FLP_Format.Controls.Add(CB_FormatComparator);
            FLP_Format.Controls.Add(CB_Format);
            FLP_Format.Location = new System.Drawing.Point(101, 416);
            FLP_Format.Margin = new System.Windows.Forms.Padding(0);
            FLP_Format.Name = "FLP_Format";
            FLP_Format.Size = new System.Drawing.Size(141, 26);
            FLP_Format.TabIndex = 124;
            // 
            // CB_FormatComparator
            // 
            CB_FormatComparator.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_FormatComparator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_FormatComparator.FormattingEnabled = true;
            CB_FormatComparator.Items.AddRange(new object[] { "Any", "==", ">=", "<=" });
            CB_FormatComparator.Location = new System.Drawing.Point(0, 0);
            CB_FormatComparator.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            CB_FormatComparator.Name = "CB_FormatComparator";
            CB_FormatComparator.Size = new System.Drawing.Size(62, 25);
            CB_FormatComparator.TabIndex = 16;
            CB_FormatComparator.SelectedIndexChanged += ChangeFormatFilter;
            // 
            // CB_Format
            // 
            CB_Format.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_Format.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Format.FormattingEnabled = true;
            CB_Format.Items.AddRange(new object[] { "Any", ".pk9", ".pk8", ".pk7", ".pk6", ".pk5", ".pk4", ".pk3", ".pk2", ".pk1" });
            CB_Format.Location = new System.Drawing.Point(62, 0);
            CB_Format.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            CB_Format.Name = "CB_Format";
            CB_Format.Size = new System.Drawing.Size(79, 25);
            CB_Format.TabIndex = 17;
            CB_Format.Visible = false;
            // 
            // L_Format
            // 
            L_Format.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Format.AutoSize = true;
            L_Format.Location = new System.Drawing.Point(45, 420);
            L_Format.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            L_Format.Name = "L_Format";
            L_Format.Size = new System.Drawing.Size(52, 17);
            L_Format.TabIndex = 125;
            L_Format.Text = "Format:";
            L_Format.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FLP_Level
            // 
            FLP_Level.Anchor = System.Windows.Forms.AnchorStyles.Left;
            FLP_Level.AutoSize = true;
            FLP_Level.Controls.Add(TB_Level);
            FLP_Level.Controls.Add(CB_Level);
            FLP_Level.Location = new System.Drawing.Point(101, 156);
            FLP_Level.Margin = new System.Windows.Forms.Padding(0);
            FLP_Level.Name = "FLP_Level";
            FLP_Level.Size = new System.Drawing.Size(101, 26);
            FLP_Level.TabIndex = 5;
            // 
            // EntitySearchControl
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            Controls.Add(TLP_Filters);
            Name = "EntitySearchControl";
            Size = new System.Drawing.Size(243, 442);
            FLP_Egg.ResumeLayout(false);
            FLP_Egg.PerformLayout();
            TLP_Filters.ResumeLayout(false);
            TLP_Filters.PerformLayout();
            FLP_Format.ResumeLayout(false);
            FLP_Level.ResumeLayout(false);
            FLP_Level.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ComboBox CB_Ability;
        private System.Windows.Forms.ComboBox CB_HeldItem;
        private System.Windows.Forms.ComboBox CB_Nature;
        private System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.TextBox TB_Nickname;
        private System.Windows.Forms.ComboBox CB_Move4;
        private System.Windows.Forms.ComboBox CB_Move3;
        private System.Windows.Forms.ComboBox CB_Move2;
        private System.Windows.Forms.ComboBox CB_Move1;
        private System.Windows.Forms.MaskedTextBox TB_Level;
        private System.Windows.Forms.Label Label_CurLevel;
        private System.Windows.Forms.Label Label_HeldItem;
        private System.Windows.Forms.Label Label_Ability;
        private System.Windows.Forms.Label Label_Nature;
        private System.Windows.Forms.Label Label_Species;
        private System.Windows.Forms.Label Label_Nickname;
        private System.Windows.Forms.ComboBox CB_EVTrain;
        private System.Windows.Forms.ComboBox CB_HPType;
        private System.Windows.Forms.Label Label_HiddenPowerPrefix;
        private System.Windows.Forms.ComboBox CB_GameOrigin;
        private System.Windows.Forms.ComboBox CB_IV;
        private System.Windows.Forms.ComboBox CB_Level;
        private System.Windows.Forms.Label L_Version;
        private System.Windows.Forms.Label L_Move1;
        private System.Windows.Forms.Label L_Move2;
        private System.Windows.Forms.Label L_Move3;
        private System.Windows.Forms.Label L_Move4;
        private System.Windows.Forms.Label L_Potential;
        private System.Windows.Forms.Label L_EVTraining;
        private System.Windows.Forms.Label L_Generation;
        private System.Windows.Forms.ComboBox CB_Generation;
        private System.Windows.Forms.FlowLayoutPanel FLP_Egg;
        private System.Windows.Forms.CheckBox CHK_IsEgg;
        private System.Windows.Forms.Label L_ESV;
        private System.Windows.Forms.MaskedTextBox MT_ESV;
        private System.Windows.Forms.CheckBox CHK_Shiny;
        private System.Windows.Forms.TableLayoutPanel TLP_Filters;
        private System.Windows.Forms.FlowLayoutPanel FLP_Format;
        private System.Windows.Forms.ComboBox CB_FormatComparator;
        private System.Windows.Forms.ComboBox CB_Format;
        private System.Windows.Forms.Label L_Format;
        private System.Windows.Forms.FlowLayoutPanel FLP_Level;
    }
}
