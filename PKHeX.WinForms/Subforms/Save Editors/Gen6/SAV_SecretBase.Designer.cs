namespace PKHeX.WinForms
{
    partial class SAV_SecretBase
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
            B_Save = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            LB_Bases = new System.Windows.Forms.ListBox();
            L_Favorite = new System.Windows.Forms.Label();
            f_PKM = new System.Windows.Forms.TabPage();
            PAN_PKM = new System.Windows.Forms.Panel();
            NUD_FPKM = new System.Windows.Forms.NumericUpDown();
            TB_SPEIV = new System.Windows.Forms.MaskedTextBox();
            CB_PPu3 = new System.Windows.Forms.ComboBox();
            TB_SPDIV = new System.Windows.Forms.MaskedTextBox();
            CB_PPu4 = new System.Windows.Forms.ComboBox();
            L_Participant = new System.Windows.Forms.Label();
            CB_PPu2 = new System.Windows.Forms.ComboBox();
            TB_SPAIV = new System.Windows.Forms.MaskedTextBox();
            L_PPups = new System.Windows.Forms.Label();
            L_HP = new System.Windows.Forms.Label();
            CB_Move4 = new System.Windows.Forms.ComboBox();
            TB_DEFIV = new System.Windows.Forms.MaskedTextBox();
            CHK_Shiny = new System.Windows.Forms.CheckBox();
            L_ATK = new System.Windows.Forms.Label();
            CB_PPu1 = new System.Windows.Forms.ComboBox();
            TB_ATKIV = new System.Windows.Forms.MaskedTextBox();
            CB_Form = new System.Windows.Forms.ComboBox();
            L_DEF = new System.Windows.Forms.Label();
            CB_Move3 = new System.Windows.Forms.ComboBox();
            TB_HPIV = new System.Windows.Forms.MaskedTextBox();
            TB_Level = new System.Windows.Forms.MaskedTextBox();
            L_SpA = new System.Windows.Forms.Label();
            CB_Move2 = new System.Windows.Forms.ComboBox();
            TB_ATKEV = new System.Windows.Forms.MaskedTextBox();
            TB_Friendship = new System.Windows.Forms.MaskedTextBox();
            L_SpD = new System.Windows.Forms.Label();
            CB_Move1 = new System.Windows.Forms.ComboBox();
            TB_DEFEV = new System.Windows.Forms.MaskedTextBox();
            CB_Ball = new System.Windows.Forms.ComboBox();
            L_SPE = new System.Windows.Forms.Label();
            L_PKFriendship = new System.Windows.Forms.Label();
            TB_SPEEV = new System.Windows.Forms.MaskedTextBox();
            CB_Species = new System.Windows.Forms.ComboBox();
            L_IVs = new System.Windows.Forms.Label();
            CB_Ability = new System.Windows.Forms.ComboBox();
            TB_SPDEV = new System.Windows.Forms.MaskedTextBox();
            CB_HeldItem = new System.Windows.Forms.ComboBox();
            L_EVs = new System.Windows.Forms.Label();
            CB_Nature = new System.Windows.Forms.ComboBox();
            TB_SPAEV = new System.Windows.Forms.MaskedTextBox();
            Label_Gender = new System.Windows.Forms.Label();
            TB_EC = new System.Windows.Forms.TextBox();
            L_EncryptionConstant = new System.Windows.Forms.Label();
            TB_HPEV = new System.Windows.Forms.MaskedTextBox();
            f_MAIN = new System.Windows.Forms.TabPage();
            PG_Base = new System.Windows.Forms.PropertyGrid();
            GB_Object = new System.Windows.Forms.GroupBox();
            L_Y = new System.Windows.Forms.Label();
            L_X = new System.Windows.Forms.Label();
            NUD_FX = new System.Windows.Forms.NumericUpDown();
            NUD_FY = new System.Windows.Forms.NumericUpDown();
            L_Rotation = new System.Windows.Forms.Label();
            L_Decoration = new System.Windows.Forms.Label();
            NUD_FObjType = new System.Windows.Forms.NumericUpDown();
            L_Index = new System.Windows.Forms.Label();
            NUD_FRot = new System.Windows.Forms.NumericUpDown();
            NUD_FObject = new System.Windows.Forms.NumericUpDown();
            Tab_Base = new System.Windows.Forms.TabControl();
            B_GiveDecor = new System.Windows.Forms.Button();
            L_FlagsCaptured = new System.Windows.Forms.Label();
            B_FDelete = new System.Windows.Forms.Button();
            B_Export = new System.Windows.Forms.Button();
            B_Import = new System.Windows.Forms.Button();
            NUD_CapturedRecord = new System.Windows.Forms.NumericUpDown();
            f_PKM.SuspendLayout();
            PAN_PKM.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_FPKM).BeginInit();
            f_MAIN.SuspendLayout();
            GB_Object.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_FX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_FY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_FObjType).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_FRot).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_FObject).BeginInit();
            Tab_Base.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_CapturedRecord).BeginInit();
            SuspendLayout();
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(580, 432);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(88, 27);
            B_Save.TabIndex = 0;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(490, 432);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(83, 27);
            B_Cancel.TabIndex = 1;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // LB_Bases
            // 
            LB_Bases.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            LB_Bases.FormattingEnabled = true;
            LB_Bases.ItemHeight = 15;
            LB_Bases.Location = new System.Drawing.Point(14, 29);
            LB_Bases.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LB_Bases.Name = "LB_Bases";
            LB_Bases.Size = new System.Drawing.Size(90, 394);
            LB_Bases.TabIndex = 4;
            LB_Bases.SelectedIndexChanged += ChangeIndexBase;
            // 
            // L_Favorite
            // 
            L_Favorite.AutoSize = true;
            L_Favorite.Location = new System.Drawing.Point(14, 10);
            L_Favorite.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Favorite.Name = "L_Favorite";
            L_Favorite.Size = new System.Drawing.Size(57, 15);
            L_Favorite.TabIndex = 6;
            L_Favorite.Text = "Favorites:";
            // 
            // f_PKM
            // 
            f_PKM.Controls.Add(PAN_PKM);
            f_PKM.Location = new System.Drawing.Point(4, 24);
            f_PKM.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            f_PKM.Name = "f_PKM";
            f_PKM.Size = new System.Drawing.Size(519, 387);
            f_PKM.TabIndex = 2;
            f_PKM.Text = "Pokemon";
            f_PKM.UseVisualStyleBackColor = true;
            // 
            // PAN_PKM
            // 
            PAN_PKM.Controls.Add(NUD_FPKM);
            PAN_PKM.Controls.Add(TB_SPEIV);
            PAN_PKM.Controls.Add(CB_PPu3);
            PAN_PKM.Controls.Add(TB_SPDIV);
            PAN_PKM.Controls.Add(CB_PPu4);
            PAN_PKM.Controls.Add(L_Participant);
            PAN_PKM.Controls.Add(CB_PPu2);
            PAN_PKM.Controls.Add(TB_SPAIV);
            PAN_PKM.Controls.Add(L_PPups);
            PAN_PKM.Controls.Add(L_HP);
            PAN_PKM.Controls.Add(CB_Move4);
            PAN_PKM.Controls.Add(TB_DEFIV);
            PAN_PKM.Controls.Add(CHK_Shiny);
            PAN_PKM.Controls.Add(L_ATK);
            PAN_PKM.Controls.Add(CB_PPu1);
            PAN_PKM.Controls.Add(TB_ATKIV);
            PAN_PKM.Controls.Add(CB_Form);
            PAN_PKM.Controls.Add(L_DEF);
            PAN_PKM.Controls.Add(CB_Move3);
            PAN_PKM.Controls.Add(TB_HPIV);
            PAN_PKM.Controls.Add(TB_Level);
            PAN_PKM.Controls.Add(L_SpA);
            PAN_PKM.Controls.Add(CB_Move2);
            PAN_PKM.Controls.Add(TB_ATKEV);
            PAN_PKM.Controls.Add(TB_Friendship);
            PAN_PKM.Controls.Add(L_SpD);
            PAN_PKM.Controls.Add(CB_Move1);
            PAN_PKM.Controls.Add(TB_DEFEV);
            PAN_PKM.Controls.Add(CB_Ball);
            PAN_PKM.Controls.Add(L_SPE);
            PAN_PKM.Controls.Add(L_PKFriendship);
            PAN_PKM.Controls.Add(TB_SPEEV);
            PAN_PKM.Controls.Add(CB_Species);
            PAN_PKM.Controls.Add(L_IVs);
            PAN_PKM.Controls.Add(CB_Ability);
            PAN_PKM.Controls.Add(TB_SPDEV);
            PAN_PKM.Controls.Add(CB_HeldItem);
            PAN_PKM.Controls.Add(L_EVs);
            PAN_PKM.Controls.Add(CB_Nature);
            PAN_PKM.Controls.Add(TB_SPAEV);
            PAN_PKM.Controls.Add(Label_Gender);
            PAN_PKM.Controls.Add(TB_EC);
            PAN_PKM.Controls.Add(L_EncryptionConstant);
            PAN_PKM.Controls.Add(TB_HPEV);
            PAN_PKM.Dock = System.Windows.Forms.DockStyle.Fill;
            PAN_PKM.Location = new System.Drawing.Point(0, 0);
            PAN_PKM.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PAN_PKM.Name = "PAN_PKM";
            PAN_PKM.Size = new System.Drawing.Size(519, 387);
            PAN_PKM.TabIndex = 98;
            // 
            // NUD_FPKM
            // 
            NUD_FPKM.Location = new System.Drawing.Point(107, 10);
            NUD_FPKM.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_FPKM.Maximum = new decimal(new int[] { 2, 0, 0, 0 });
            NUD_FPKM.Name = "NUD_FPKM";
            NUD_FPKM.Size = new System.Drawing.Size(44, 23);
            NUD_FPKM.TabIndex = 66;
            NUD_FPKM.ValueChanged += ChangeIndexPKM;
            // 
            // TB_SPEIV
            // 
            TB_SPEIV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_SPEIV.Location = new System.Drawing.Point(374, 164);
            TB_SPEIV.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_SPEIV.Mask = "00";
            TB_SPEIV.Name = "TB_SPEIV";
            TB_SPEIV.Size = new System.Drawing.Size(25, 23);
            TB_SPEIV.TabIndex = 91;
            TB_SPEIV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CB_PPu3
            // 
            CB_PPu3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_PPu3.FormattingEnabled = true;
            CB_PPu3.Items.AddRange(new object[] { "0", "1", "2", "3" });
            CB_PPu3.Location = new System.Drawing.Point(161, 262);
            CB_PPu3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_PPu3.Name = "CB_PPu3";
            CB_PPu3.Size = new System.Drawing.Size(44, 23);
            CB_PPu3.TabIndex = 73;
            // 
            // TB_SPDIV
            // 
            TB_SPDIV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_SPDIV.Location = new System.Drawing.Point(374, 138);
            TB_SPDIV.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_SPDIV.Mask = "00";
            TB_SPDIV.Name = "TB_SPDIV";
            TB_SPDIV.Size = new System.Drawing.Size(25, 23);
            TB_SPDIV.TabIndex = 90;
            TB_SPDIV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CB_PPu4
            // 
            CB_PPu4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_PPu4.FormattingEnabled = true;
            CB_PPu4.Items.AddRange(new object[] { "0", "1", "2", "3" });
            CB_PPu4.Location = new System.Drawing.Point(161, 287);
            CB_PPu4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_PPu4.Name = "CB_PPu4";
            CB_PPu4.Size = new System.Drawing.Size(44, 23);
            CB_PPu4.TabIndex = 76;
            // 
            // L_Participant
            // 
            L_Participant.Location = new System.Drawing.Point(9, 8);
            L_Participant.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Participant.Name = "L_Participant";
            L_Participant.Size = new System.Drawing.Size(91, 23);
            L_Participant.TabIndex = 67;
            L_Participant.Text = "Participant:";
            L_Participant.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_PPu2
            // 
            CB_PPu2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_PPu2.FormattingEnabled = true;
            CB_PPu2.Items.AddRange(new object[] { "0", "1", "2", "3" });
            CB_PPu2.Location = new System.Drawing.Point(161, 237);
            CB_PPu2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_PPu2.Name = "CB_PPu2";
            CB_PPu2.Size = new System.Drawing.Size(44, 23);
            CB_PPu2.TabIndex = 71;
            // 
            // TB_SPAIV
            // 
            TB_SPAIV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_SPAIV.Location = new System.Drawing.Point(374, 113);
            TB_SPAIV.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_SPAIV.Mask = "00";
            TB_SPAIV.Name = "TB_SPAIV";
            TB_SPAIV.Size = new System.Drawing.Size(25, 23);
            TB_SPAIV.TabIndex = 89;
            TB_SPAIV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // L_PPups
            // 
            L_PPups.Location = new System.Drawing.Point(158, 195);
            L_PPups.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_PPups.Name = "L_PPups";
            L_PPups.Size = new System.Drawing.Size(52, 15);
            L_PPups.TabIndex = 75;
            L_PPups.Text = "PP Ups";
            L_PPups.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // L_HP
            // 
            L_HP.Location = new System.Drawing.Point(309, 37);
            L_HP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_HP.Name = "L_HP";
            L_HP.Size = new System.Drawing.Size(58, 24);
            L_HP.TabIndex = 2;
            L_HP.Text = "HP";
            L_HP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_Move4
            // 
            CB_Move4.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Move4.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Move4.FormattingEnabled = true;
            CB_Move4.Location = new System.Drawing.Point(9, 287);
            CB_Move4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Move4.Name = "CB_Move4";
            CB_Move4.Size = new System.Drawing.Size(142, 23);
            CB_Move4.TabIndex = 74;
            // 
            // TB_DEFIV
            // 
            TB_DEFIV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_DEFIV.Location = new System.Drawing.Point(374, 88);
            TB_DEFIV.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_DEFIV.Mask = "00";
            TB_DEFIV.Name = "TB_DEFIV";
            TB_DEFIV.Size = new System.Drawing.Size(25, 23);
            TB_DEFIV.TabIndex = 88;
            TB_DEFIV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CHK_Shiny
            // 
            CHK_Shiny.AutoSize = true;
            CHK_Shiny.Location = new System.Drawing.Point(162, 92);
            CHK_Shiny.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_Shiny.Name = "CHK_Shiny";
            CHK_Shiny.Size = new System.Drawing.Size(36, 19);
            CHK_Shiny.TabIndex = 77;
            CHK_Shiny.Text = "☆";
            CHK_Shiny.UseVisualStyleBackColor = true;
            // 
            // L_ATK
            // 
            L_ATK.Location = new System.Drawing.Point(309, 62);
            L_ATK.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_ATK.Name = "L_ATK";
            L_ATK.Size = new System.Drawing.Size(58, 24);
            L_ATK.TabIndex = 3;
            L_ATK.Text = "ATK";
            L_ATK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_PPu1
            // 
            CB_PPu1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_PPu1.FormattingEnabled = true;
            CB_PPu1.Items.AddRange(new object[] { "0", "1", "2", "3" });
            CB_PPu1.Location = new System.Drawing.Point(161, 211);
            CB_PPu1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_PPu1.Name = "CB_PPu1";
            CB_PPu1.Size = new System.Drawing.Size(44, 23);
            CB_PPu1.TabIndex = 69;
            // 
            // TB_ATKIV
            // 
            TB_ATKIV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_ATKIV.Location = new System.Drawing.Point(374, 62);
            TB_ATKIV.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_ATKIV.Mask = "00";
            TB_ATKIV.Name = "TB_ATKIV";
            TB_ATKIV.Size = new System.Drawing.Size(25, 23);
            TB_ATKIV.TabIndex = 87;
            TB_ATKIV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CB_Form
            // 
            CB_Form.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Form.DropDownWidth = 85;
            CB_Form.Enabled = false;
            CB_Form.FormattingEnabled = true;
            CB_Form.Location = new System.Drawing.Point(71, 88);
            CB_Form.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Form.Name = "CB_Form";
            CB_Form.Size = new System.Drawing.Size(80, 23);
            CB_Form.TabIndex = 78;
            CB_Form.SelectedIndexChanged += UpdateForm;
            // 
            // L_DEF
            // 
            L_DEF.Location = new System.Drawing.Point(309, 88);
            L_DEF.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_DEF.Name = "L_DEF";
            L_DEF.Size = new System.Drawing.Size(58, 24);
            L_DEF.TabIndex = 4;
            L_DEF.Text = "DEF";
            L_DEF.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_Move3
            // 
            CB_Move3.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Move3.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Move3.FormattingEnabled = true;
            CB_Move3.Location = new System.Drawing.Point(9, 262);
            CB_Move3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Move3.Name = "CB_Move3";
            CB_Move3.Size = new System.Drawing.Size(142, 23);
            CB_Move3.TabIndex = 72;
            // 
            // TB_HPIV
            // 
            TB_HPIV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_HPIV.Location = new System.Drawing.Point(374, 37);
            TB_HPIV.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_HPIV.Mask = "00";
            TB_HPIV.Name = "TB_HPIV";
            TB_HPIV.Size = new System.Drawing.Size(25, 23);
            TB_HPIV.TabIndex = 86;
            TB_HPIV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TB_Level
            // 
            TB_Level.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_Level.Location = new System.Drawing.Point(162, 63);
            TB_Level.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_Level.Mask = "000";
            TB_Level.Name = "TB_Level";
            TB_Level.Size = new System.Drawing.Size(25, 23);
            TB_Level.TabIndex = 79;
            TB_Level.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // L_SpA
            // 
            L_SpA.Location = new System.Drawing.Point(309, 113);
            L_SpA.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_SpA.Name = "L_SpA";
            L_SpA.Size = new System.Drawing.Size(58, 24);
            L_SpA.TabIndex = 11;
            L_SpA.Text = "SpA";
            L_SpA.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_Move2
            // 
            CB_Move2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Move2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Move2.FormattingEnabled = true;
            CB_Move2.Location = new System.Drawing.Point(9, 237);
            CB_Move2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Move2.Name = "CB_Move2";
            CB_Move2.Size = new System.Drawing.Size(142, 23);
            CB_Move2.TabIndex = 70;
            // 
            // TB_ATKEV
            // 
            TB_ATKEV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_ATKEV.Location = new System.Drawing.Point(407, 62);
            TB_ATKEV.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_ATKEV.Mask = "000";
            TB_ATKEV.Name = "TB_ATKEV";
            TB_ATKEV.Size = new System.Drawing.Size(36, 23);
            TB_ATKEV.TabIndex = 93;
            TB_ATKEV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TB_Friendship
            // 
            TB_Friendship.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_Friendship.Location = new System.Drawing.Point(418, 226);
            TB_Friendship.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_Friendship.Mask = "000";
            TB_Friendship.Name = "TB_Friendship";
            TB_Friendship.Size = new System.Drawing.Size(25, 23);
            TB_Friendship.TabIndex = 80;
            // 
            // L_SpD
            // 
            L_SpD.Location = new System.Drawing.Point(309, 138);
            L_SpD.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_SpD.Name = "L_SpD";
            L_SpD.Size = new System.Drawing.Size(58, 24);
            L_SpD.TabIndex = 12;
            L_SpD.Text = "SpD";
            L_SpD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_Move1
            // 
            CB_Move1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Move1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Move1.FormattingEnabled = true;
            CB_Move1.Location = new System.Drawing.Point(9, 211);
            CB_Move1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Move1.Name = "CB_Move1";
            CB_Move1.Size = new System.Drawing.Size(142, 23);
            CB_Move1.TabIndex = 68;
            // 
            // TB_DEFEV
            // 
            TB_DEFEV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_DEFEV.Location = new System.Drawing.Point(407, 88);
            TB_DEFEV.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_DEFEV.Mask = "000";
            TB_DEFEV.Name = "TB_DEFEV";
            TB_DEFEV.Size = new System.Drawing.Size(36, 23);
            TB_DEFEV.TabIndex = 94;
            TB_DEFEV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CB_Ball
            // 
            CB_Ball.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Ball.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Ball.FormattingEnabled = true;
            CB_Ball.Location = new System.Drawing.Point(301, 287);
            CB_Ball.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Ball.Name = "CB_Ball";
            CB_Ball.Size = new System.Drawing.Size(142, 23);
            CB_Ball.TabIndex = 81;
            // 
            // L_SPE
            // 
            L_SPE.Location = new System.Drawing.Point(309, 164);
            L_SPE.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_SPE.Name = "L_SPE";
            L_SPE.Size = new System.Drawing.Size(58, 24);
            L_SPE.TabIndex = 13;
            L_SPE.Text = "SPE";
            L_SPE.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_PKFriendship
            // 
            L_PKFriendship.Location = new System.Drawing.Point(323, 225);
            L_PKFriendship.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_PKFriendship.Name = "L_PKFriendship";
            L_PKFriendship.Size = new System.Drawing.Size(88, 24);
            L_PKFriendship.TabIndex = 82;
            L_PKFriendship.Text = "Friendship";
            L_PKFriendship.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TB_SPEEV
            // 
            TB_SPEEV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_SPEEV.Location = new System.Drawing.Point(407, 164);
            TB_SPEEV.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_SPEEV.Mask = "000";
            TB_SPEEV.Name = "TB_SPEEV";
            TB_SPEEV.Size = new System.Drawing.Size(36, 23);
            TB_SPEEV.TabIndex = 97;
            TB_SPEEV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CB_Species
            // 
            CB_Species.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Species.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Species.FormattingEnabled = true;
            CB_Species.Location = new System.Drawing.Point(9, 62);
            CB_Species.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Species.Name = "CB_Species";
            CB_Species.Size = new System.Drawing.Size(142, 23);
            CB_Species.TabIndex = 65;
            CB_Species.SelectedIndexChanged += UpdateSpecies;
            // 
            // L_IVs
            // 
            L_IVs.AutoSize = true;
            L_IVs.Location = new System.Drawing.Point(374, 18);
            L_IVs.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_IVs.Name = "L_IVs";
            L_IVs.Size = new System.Drawing.Size(22, 15);
            L_IVs.TabIndex = 16;
            L_IVs.Text = "IVs";
            // 
            // CB_Ability
            // 
            CB_Ability.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Ability.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Ability.FormattingEnabled = true;
            CB_Ability.Location = new System.Drawing.Point(9, 113);
            CB_Ability.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Ability.Name = "CB_Ability";
            CB_Ability.Size = new System.Drawing.Size(142, 23);
            CB_Ability.TabIndex = 83;
            // 
            // TB_SPDEV
            // 
            TB_SPDEV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_SPDEV.Location = new System.Drawing.Point(407, 138);
            TB_SPDEV.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_SPDEV.Mask = "000";
            TB_SPDEV.Name = "TB_SPDEV";
            TB_SPDEV.Size = new System.Drawing.Size(36, 23);
            TB_SPDEV.TabIndex = 96;
            TB_SPDEV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CB_HeldItem
            // 
            CB_HeldItem.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_HeldItem.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_HeldItem.FormattingEnabled = true;
            CB_HeldItem.Location = new System.Drawing.Point(9, 138);
            CB_HeldItem.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_HeldItem.Name = "CB_HeldItem";
            CB_HeldItem.Size = new System.Drawing.Size(142, 23);
            CB_HeldItem.TabIndex = 64;
            // 
            // L_EVs
            // 
            L_EVs.AutoSize = true;
            L_EVs.Location = new System.Drawing.Point(408, 18);
            L_EVs.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_EVs.Name = "L_EVs";
            L_EVs.Size = new System.Drawing.Size(25, 15);
            L_EVs.TabIndex = 23;
            L_EVs.Text = "EVs";
            // 
            // CB_Nature
            // 
            CB_Nature.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Nature.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Nature.FormattingEnabled = true;
            CB_Nature.Location = new System.Drawing.Point(301, 256);
            CB_Nature.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Nature.Name = "CB_Nature";
            CB_Nature.Size = new System.Drawing.Size(142, 23);
            CB_Nature.TabIndex = 63;
            // 
            // TB_SPAEV
            // 
            TB_SPAEV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_SPAEV.Location = new System.Drawing.Point(407, 113);
            TB_SPAEV.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_SPAEV.Mask = "000";
            TB_SPAEV.Name = "TB_SPAEV";
            TB_SPAEV.Size = new System.Drawing.Size(36, 23);
            TB_SPAEV.TabIndex = 95;
            TB_SPAEV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Label_Gender
            // 
            Label_Gender.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Label_Gender.Location = new System.Drawing.Point(195, 66);
            Label_Gender.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label_Gender.Name = "Label_Gender";
            Label_Gender.Size = new System.Drawing.Size(19, 15);
            Label_Gender.TabIndex = 85;
            Label_Gender.Text = "-";
            Label_Gender.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            Label_Gender.Click += Label_Gender_Click;
            // 
            // TB_EC
            // 
            TB_EC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_EC.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TB_EC.Location = new System.Drawing.Point(82, 37);
            TB_EC.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_EC.MaxLength = 8;
            TB_EC.Name = "TB_EC";
            TB_EC.Size = new System.Drawing.Size(70, 20);
            TB_EC.TabIndex = 61;
            TB_EC.Text = "12345678";
            // 
            // L_EncryptionConstant
            // 
            L_EncryptionConstant.Location = new System.Drawing.Point(9, 37);
            L_EncryptionConstant.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_EncryptionConstant.Name = "L_EncryptionConstant";
            L_EncryptionConstant.Size = new System.Drawing.Size(65, 24);
            L_EncryptionConstant.TabIndex = 62;
            L_EncryptionConstant.Text = "ENC:";
            L_EncryptionConstant.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TB_HPEV
            // 
            TB_HPEV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_HPEV.Location = new System.Drawing.Point(407, 37);
            TB_HPEV.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_HPEV.Mask = "000";
            TB_HPEV.Name = "TB_HPEV";
            TB_HPEV.Size = new System.Drawing.Size(36, 23);
            TB_HPEV.TabIndex = 92;
            TB_HPEV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // f_MAIN
            // 
            f_MAIN.Controls.Add(PG_Base);
            f_MAIN.Controls.Add(GB_Object);
            f_MAIN.Location = new System.Drawing.Point(4, 24);
            f_MAIN.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            f_MAIN.Name = "f_MAIN";
            f_MAIN.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            f_MAIN.Size = new System.Drawing.Size(519, 387);
            f_MAIN.TabIndex = 0;
            f_MAIN.Text = "Main";
            f_MAIN.UseVisualStyleBackColor = true;
            // 
            // PG_Base
            // 
            PG_Base.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            PG_Base.HelpVisible = false;
            PG_Base.Location = new System.Drawing.Point(1, 3);
            PG_Base.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PG_Base.Name = "PG_Base";
            PG_Base.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            PG_Base.Size = new System.Drawing.Size(338, 380);
            PG_Base.TabIndex = 19;
            PG_Base.ToolbarVisible = false;
            // 
            // GB_Object
            // 
            GB_Object.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            GB_Object.Controls.Add(L_Y);
            GB_Object.Controls.Add(L_X);
            GB_Object.Controls.Add(NUD_FX);
            GB_Object.Controls.Add(NUD_FY);
            GB_Object.Controls.Add(L_Rotation);
            GB_Object.Controls.Add(L_Decoration);
            GB_Object.Controls.Add(NUD_FObjType);
            GB_Object.Controls.Add(L_Index);
            GB_Object.Controls.Add(NUD_FRot);
            GB_Object.Controls.Add(NUD_FObject);
            GB_Object.Location = new System.Drawing.Point(345, 87);
            GB_Object.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Object.Name = "GB_Object";
            GB_Object.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Object.Size = new System.Drawing.Size(166, 178);
            GB_Object.TabIndex = 13;
            GB_Object.TabStop = false;
            GB_Object.Text = "Object Layout";
            // 
            // L_Y
            // 
            L_Y.Location = new System.Drawing.Point(12, 114);
            L_Y.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Y.Name = "L_Y";
            L_Y.Size = new System.Drawing.Size(88, 24);
            L_Y.TabIndex = 9;
            L_Y.Text = "Y Coordinate:";
            L_Y.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_X
            // 
            L_X.Location = new System.Drawing.Point(12, 90);
            L_X.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_X.Name = "L_X";
            L_X.Size = new System.Drawing.Size(88, 24);
            L_X.TabIndex = 8;
            L_X.Text = "X Coordinate:";
            L_X.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_FX
            // 
            NUD_FX.Location = new System.Drawing.Point(102, 92);
            NUD_FX.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_FX.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NUD_FX.Name = "NUD_FX";
            NUD_FX.Size = new System.Drawing.Size(47, 23);
            NUD_FX.TabIndex = 7;
            // 
            // NUD_FY
            // 
            NUD_FY.Location = new System.Drawing.Point(102, 117);
            NUD_FY.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_FY.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NUD_FY.Name = "NUD_FY";
            NUD_FY.Size = new System.Drawing.Size(47, 23);
            NUD_FY.TabIndex = 6;
            // 
            // L_Rotation
            // 
            L_Rotation.Location = new System.Drawing.Point(12, 144);
            L_Rotation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Rotation.Name = "L_Rotation";
            L_Rotation.Size = new System.Drawing.Size(88, 24);
            L_Rotation.TabIndex = 5;
            L_Rotation.Text = "Rotation Val:";
            L_Rotation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Decoration
            // 
            L_Decoration.Location = new System.Drawing.Point(12, 60);
            L_Decoration.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Decoration.Name = "L_Decoration";
            L_Decoration.Size = new System.Drawing.Size(88, 24);
            L_Decoration.TabIndex = 4;
            L_Decoration.Text = "Decoration:";
            L_Decoration.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_FObjType
            // 
            NUD_FObjType.Location = new System.Drawing.Point(102, 62);
            NUD_FObjType.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_FObjType.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NUD_FObjType.Minimum = new decimal(new int[] { 1, 0, 0, int.MinValue });
            NUD_FObjType.Name = "NUD_FObjType";
            NUD_FObjType.Size = new System.Drawing.Size(47, 23);
            NUD_FObjType.TabIndex = 2;
            NUD_FObjType.Value = new decimal(new int[] { 1, 0, 0, int.MinValue });
            // 
            // L_Index
            // 
            L_Index.Location = new System.Drawing.Point(12, 18);
            L_Index.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Index.Name = "L_Index";
            L_Index.Size = new System.Drawing.Size(88, 24);
            L_Index.TabIndex = 1;
            L_Index.Text = "Index:";
            L_Index.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_FRot
            // 
            NUD_FRot.Location = new System.Drawing.Point(102, 147);
            NUD_FRot.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_FRot.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_FRot.Name = "NUD_FRot";
            NUD_FRot.Size = new System.Drawing.Size(47, 23);
            NUD_FRot.TabIndex = 3;
            // 
            // NUD_FObject
            // 
            NUD_FObject.Location = new System.Drawing.Point(102, 21);
            NUD_FObject.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_FObject.Maximum = new decimal(new int[] { 27, 0, 0, 0 });
            NUD_FObject.Name = "NUD_FObject";
            NUD_FObject.Size = new System.Drawing.Size(47, 23);
            NUD_FObject.TabIndex = 0;
            NUD_FObject.ValueChanged += ChangeIndexPlacement;
            // 
            // Tab_Base
            // 
            Tab_Base.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            Tab_Base.Controls.Add(f_MAIN);
            Tab_Base.Controls.Add(f_PKM);
            Tab_Base.Location = new System.Drawing.Point(140, 10);
            Tab_Base.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_Base.Name = "Tab_Base";
            Tab_Base.SelectedIndex = 0;
            Tab_Base.Size = new System.Drawing.Size(527, 415);
            Tab_Base.TabIndex = 17;
            // 
            // B_GiveDecor
            // 
            B_GiveDecor.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_GiveDecor.Location = new System.Drawing.Point(252, 432);
            B_GiveDecor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_GiveDecor.Name = "B_GiveDecor";
            B_GiveDecor.Size = new System.Drawing.Size(131, 27);
            B_GiveDecor.TabIndex = 19;
            B_GiveDecor.Text = "Give All Decorations";
            B_GiveDecor.UseVisualStyleBackColor = true;
            B_GiveDecor.Click += B_GiveDecor_Click;
            // 
            // L_FlagsCaptured
            // 
            L_FlagsCaptured.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            L_FlagsCaptured.Location = new System.Drawing.Point(10, 437);
            L_FlagsCaptured.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_FlagsCaptured.Name = "L_FlagsCaptured";
            L_FlagsCaptured.Size = new System.Drawing.Size(117, 15);
            L_FlagsCaptured.TabIndex = 22;
            L_FlagsCaptured.Text = "Flags Captured: ";
            L_FlagsCaptured.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // B_FDelete
            // 
            B_FDelete.Location = new System.Drawing.Point(112, 29);
            B_FDelete.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_FDelete.Name = "B_FDelete";
            B_FDelete.Size = new System.Drawing.Size(21, 28);
            B_FDelete.TabIndex = 23;
            B_FDelete.Text = "X";
            B_FDelete.UseVisualStyleBackColor = true;
            B_FDelete.Click += B_FDelete_Click;
            // 
            // B_Export
            // 
            B_Export.Location = new System.Drawing.Point(493, 2);
            B_Export.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Export.Name = "B_Export";
            B_Export.Size = new System.Drawing.Size(88, 27);
            B_Export.TabIndex = 24;
            B_Export.Text = "Export";
            B_Export.UseVisualStyleBackColor = true;
            B_Export.Click += B_Export_Click;
            // 
            // B_Import
            // 
            B_Import.Location = new System.Drawing.Point(404, 2);
            B_Import.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Import.Name = "B_Import";
            B_Import.Size = new System.Drawing.Size(88, 27);
            B_Import.TabIndex = 25;
            B_Import.Text = "Import";
            B_Import.UseVisualStyleBackColor = true;
            B_Import.Click += B_Import_Click;
            // 
            // NUD_CapturedRecord
            // 
            NUD_CapturedRecord.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            NUD_CapturedRecord.Location = new System.Drawing.Point(130, 434);
            NUD_CapturedRecord.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_CapturedRecord.Maximum = new decimal(new int[] { -1, 0, 0, 0 });
            NUD_CapturedRecord.Name = "NUD_CapturedRecord";
            NUD_CapturedRecord.Size = new System.Drawing.Size(115, 23);
            NUD_CapturedRecord.TabIndex = 26;
            NUD_CapturedRecord.Value = new decimal(new int[] { -1, 0, 0, 0 });
            // 
            // SAV_SecretBase
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(686, 466);
            Controls.Add(NUD_CapturedRecord);
            Controls.Add(B_Import);
            Controls.Add(B_Export);
            Controls.Add(B_FDelete);
            Controls.Add(L_FlagsCaptured);
            Controls.Add(B_GiveDecor);
            Controls.Add(B_Cancel);
            Controls.Add(B_Save);
            Controls.Add(Tab_Base);
            Controls.Add(L_Favorite);
            Controls.Add(LB_Bases);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_SecretBase";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Secret Base Editor";
            f_PKM.ResumeLayout(false);
            PAN_PKM.ResumeLayout(false);
            PAN_PKM.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_FPKM).EndInit();
            f_MAIN.ResumeLayout(false);
            GB_Object.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)NUD_FX).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_FY).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_FObjType).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_FRot).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_FObject).EndInit();
            Tab_Base.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)NUD_CapturedRecord).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.ListBox LB_Bases;
        private System.Windows.Forms.Label L_Favorite;
        private System.Windows.Forms.TabPage f_PKM;
        private System.Windows.Forms.Label L_EVs;
        private System.Windows.Forms.Label L_IVs;
        private System.Windows.Forms.Label L_SPE;
        private System.Windows.Forms.Label L_SpD;
        private System.Windows.Forms.Label L_SpA;
        private System.Windows.Forms.Label L_DEF;
        private System.Windows.Forms.Label L_ATK;
        private System.Windows.Forms.Label L_HP;
        private System.Windows.Forms.TabPage f_MAIN;
        private System.Windows.Forms.GroupBox GB_Object;
        private System.Windows.Forms.Label L_Y;
        private System.Windows.Forms.Label L_X;
        private System.Windows.Forms.NumericUpDown NUD_FX;
        private System.Windows.Forms.NumericUpDown NUD_FY;
        private System.Windows.Forms.Label L_Rotation;
        private System.Windows.Forms.Label L_Decoration;
        private System.Windows.Forms.NumericUpDown NUD_FRot;
        private System.Windows.Forms.NumericUpDown NUD_FObjType;
        private System.Windows.Forms.Label L_Index;
        private System.Windows.Forms.NumericUpDown NUD_FObject;
        private System.Windows.Forms.TabControl Tab_Base;
        private System.Windows.Forms.Label L_EncryptionConstant;
        private System.Windows.Forms.TextBox TB_EC;
        private System.Windows.Forms.ComboBox CB_HeldItem;
        private System.Windows.Forms.ComboBox CB_Nature;
        private System.Windows.Forms.Label L_Participant;
        private System.Windows.Forms.NumericUpDown NUD_FPKM;
        public System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.CheckBox CHK_Shiny;
        private System.Windows.Forms.Label L_PPups;
        private System.Windows.Forms.ComboBox CB_PPu4;
        private System.Windows.Forms.ComboBox CB_PPu3;
        private System.Windows.Forms.ComboBox CB_PPu2;
        private System.Windows.Forms.ComboBox CB_Move4;
        private System.Windows.Forms.ComboBox CB_PPu1;
        private System.Windows.Forms.ComboBox CB_Move3;
        private System.Windows.Forms.ComboBox CB_Move2;
        public System.Windows.Forms.ComboBox CB_Move1;
        private System.Windows.Forms.ComboBox CB_Form;
        public System.Windows.Forms.MaskedTextBox TB_Friendship;
        private System.Windows.Forms.MaskedTextBox TB_Level;
        private System.Windows.Forms.ComboBox CB_Ball;
        private System.Windows.Forms.Label L_PKFriendship;
        private System.Windows.Forms.ComboBox CB_Ability;
        private System.Windows.Forms.Label Label_Gender;
        private System.Windows.Forms.Button B_GiveDecor;
        private System.Windows.Forms.MaskedTextBox TB_SPEIV;
        private System.Windows.Forms.MaskedTextBox TB_SPDIV;
        private System.Windows.Forms.MaskedTextBox TB_SPAIV;
        private System.Windows.Forms.MaskedTextBox TB_DEFIV;
        private System.Windows.Forms.MaskedTextBox TB_ATKIV;
        private System.Windows.Forms.MaskedTextBox TB_HPIV;
        private System.Windows.Forms.MaskedTextBox TB_ATKEV;
        private System.Windows.Forms.MaskedTextBox TB_DEFEV;
        private System.Windows.Forms.MaskedTextBox TB_SPEEV;
        private System.Windows.Forms.MaskedTextBox TB_SPDEV;
        private System.Windows.Forms.MaskedTextBox TB_SPAEV;
        private System.Windows.Forms.MaskedTextBox TB_HPEV;
        private System.Windows.Forms.Label L_FlagsCaptured;
        private System.Windows.Forms.Button B_FDelete;
        private System.Windows.Forms.Button B_Export;
        private System.Windows.Forms.Button B_Import;
        private System.Windows.Forms.NumericUpDown NUD_CapturedRecord;
        private System.Windows.Forms.Panel PAN_PKM;
        private System.Windows.Forms.PropertyGrid PG_Base;
    }
}
