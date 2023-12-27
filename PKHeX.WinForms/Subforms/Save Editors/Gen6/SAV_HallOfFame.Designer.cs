namespace PKHeX.WinForms
{
    partial class SAV_HallOfFame
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
            LB_DataEntry = new System.Windows.Forms.ListBox();
            RTB = new System.Windows.Forms.RichTextBox();
            B_Close = new System.Windows.Forms.Button();
            bpkx = new System.Windows.Forms.PictureBox();
            NUP_PartyIndex = new System.Windows.Forms.NumericUpDown();
            L_PartyNum = new System.Windows.Forms.Label();
            CB_Species = new System.Windows.Forms.ComboBox();
            Label_Species = new System.Windows.Forms.Label();
            CHK_Nicknamed = new System.Windows.Forms.CheckBox();
            TB_Nickname = new System.Windows.Forms.TextBox();
            TB_EC = new System.Windows.Forms.TextBox();
            Label_EncryptionConstant = new System.Windows.Forms.Label();
            GB_CurrentMoves = new System.Windows.Forms.GroupBox();
            CB_Move4 = new System.Windows.Forms.ComboBox();
            CB_Move3 = new System.Windows.Forms.ComboBox();
            CB_Move2 = new System.Windows.Forms.ComboBox();
            CB_Move1 = new System.Windows.Forms.ComboBox();
            Label_HeldItem = new System.Windows.Forms.Label();
            CB_HeldItem = new System.Windows.Forms.ComboBox();
            GB_OT = new System.Windows.Forms.GroupBox();
            Label_OTGender = new System.Windows.Forms.Label();
            TB_OT = new System.Windows.Forms.TextBox();
            TB_SID = new System.Windows.Forms.MaskedTextBox();
            TB_TID = new System.Windows.Forms.MaskedTextBox();
            Label_OT = new System.Windows.Forms.Label();
            Label_SID = new System.Windows.Forms.Label();
            Label_TID = new System.Windows.Forms.Label();
            L_Victory = new System.Windows.Forms.Label();
            TB_VN = new System.Windows.Forms.MaskedTextBox();
            CAL_MetDate = new System.Windows.Forms.DateTimePicker();
            Label_MetDate = new System.Windows.Forms.Label();
            B_Cancel = new System.Windows.Forms.Button();
            Label_Gender = new System.Windows.Forms.Label();
            CB_Form = new System.Windows.Forms.ComboBox();
            Label_Form = new System.Windows.Forms.Label();
            CHK_Shiny = new System.Windows.Forms.CheckBox();
            L_Shiny = new System.Windows.Forms.Label();
            TB_Level = new System.Windows.Forms.MaskedTextBox();
            L_Level = new System.Windows.Forms.Label();
            B_CopyText = new System.Windows.Forms.Button();
            B_Delete = new System.Windows.Forms.Button();
            groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)bpkx).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUP_PartyIndex).BeginInit();
            GB_CurrentMoves.SuspendLayout();
            GB_OT.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // LB_DataEntry
            // 
            LB_DataEntry.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            LB_DataEntry.FormattingEnabled = true;
            LB_DataEntry.ItemHeight = 15;
            LB_DataEntry.Items.AddRange(new object[] { "First", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15" });
            LB_DataEntry.Location = new System.Drawing.Point(8, 14);
            LB_DataEntry.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LB_DataEntry.Name = "LB_DataEntry";
            LB_DataEntry.Size = new System.Drawing.Size(68, 274);
            LB_DataEntry.TabIndex = 0;
            LB_DataEntry.SelectedIndexChanged += DisplayEntry;
            // 
            // RTB
            // 
            RTB.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            RTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            RTB.Location = new System.Drawing.Point(84, 18);
            RTB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RTB.Name = "RTB";
            RTB.ReadOnly = true;
            RTB.Size = new System.Drawing.Size(257, 355);
            RTB.TabIndex = 1;
            RTB.Text = "";
            RTB.WordWrap = false;
            // 
            // B_Close
            // 
            B_Close.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Close.Location = new System.Drawing.Point(632, 336);
            B_Close.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Close.Name = "B_Close";
            B_Close.Size = new System.Drawing.Size(89, 27);
            B_Close.TabIndex = 3;
            B_Close.Text = "Save";
            B_Close.UseVisualStyleBackColor = true;
            B_Close.Click += B_Close_Click;
            // 
            // bpkx
            // 
            bpkx.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            bpkx.Location = new System.Drawing.Point(18, 98);
            bpkx.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            bpkx.Name = "bpkx";
            bpkx.Size = new System.Drawing.Size(81, 67);
            bpkx.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            bpkx.TabIndex = 31;
            bpkx.TabStop = false;
            // 
            // NUP_PartyIndex
            // 
            NUP_PartyIndex.Location = new System.Drawing.Point(89, 66);
            NUP_PartyIndex.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUP_PartyIndex.Maximum = new decimal(new int[] { 6, 0, 0, 0 });
            NUP_PartyIndex.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NUP_PartyIndex.Name = "NUP_PartyIndex";
            NUP_PartyIndex.Size = new System.Drawing.Size(35, 23);
            NUP_PartyIndex.TabIndex = 32;
            NUP_PartyIndex.Value = new decimal(new int[] { 1, 0, 0, 0 });
            NUP_PartyIndex.ValueChanged += NUP_PartyIndex_ValueChanged;
            // 
            // L_PartyNum
            // 
            L_PartyNum.AutoSize = true;
            L_PartyNum.Location = new System.Drawing.Point(8, 68);
            L_PartyNum.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_PartyNum.Name = "L_PartyNum";
            L_PartyNum.Size = new System.Drawing.Size(69, 15);
            L_PartyNum.TabIndex = 33;
            L_PartyNum.Text = "Party Index:";
            // 
            // CB_Species
            // 
            CB_Species.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Species.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Species.FormattingEnabled = true;
            CB_Species.Location = new System.Drawing.Point(222, 65);
            CB_Species.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Species.Name = "CB_Species";
            CB_Species.Size = new System.Drawing.Size(144, 23);
            CB_Species.TabIndex = 35;
            CB_Species.SelectedValueChanged += UpdateSpecies;
            // 
            // Label_Species
            // 
            Label_Species.Location = new System.Drawing.Point(158, 68);
            Label_Species.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label_Species.Name = "Label_Species";
            Label_Species.Size = new System.Drawing.Size(61, 15);
            Label_Species.TabIndex = 34;
            Label_Species.Text = "Species:";
            Label_Species.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CHK_Nicknamed
            // 
            CHK_Nicknamed.Location = new System.Drawing.Point(128, 96);
            CHK_Nicknamed.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_Nicknamed.Name = "CHK_Nicknamed";
            CHK_Nicknamed.Size = new System.Drawing.Size(96, 20);
            CHK_Nicknamed.TabIndex = 36;
            CHK_Nicknamed.Text = "Nickname:";
            CHK_Nicknamed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            CHK_Nicknamed.UseVisualStyleBackColor = true;
            CHK_Nicknamed.CheckedChanged += UpdateNickname;
            // 
            // TB_Nickname
            // 
            TB_Nickname.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_Nickname.Location = new System.Drawing.Point(222, 93);
            TB_Nickname.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_Nickname.MaxLength = 12;
            TB_Nickname.Name = "TB_Nickname";
            TB_Nickname.Size = new System.Drawing.Size(144, 23);
            TB_Nickname.TabIndex = 37;
            TB_Nickname.TextChanged += Write_Entry;
            TB_Nickname.MouseDown += ChangeNickname;
            // 
            // TB_EC
            // 
            TB_EC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_EC.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TB_EC.Location = new System.Drawing.Point(294, 185);
            TB_EC.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_EC.MaxLength = 8;
            TB_EC.Name = "TB_EC";
            TB_EC.Size = new System.Drawing.Size(72, 20);
            TB_EC.TabIndex = 63;
            TB_EC.Text = "12345678";
            TB_EC.TextChanged += Write_Entry;
            // 
            // Label_EncryptionConstant
            // 
            Label_EncryptionConstant.Location = new System.Drawing.Point(169, 188);
            Label_EncryptionConstant.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label_EncryptionConstant.Name = "Label_EncryptionConstant";
            Label_EncryptionConstant.Size = new System.Drawing.Size(125, 15);
            Label_EncryptionConstant.TabIndex = 62;
            Label_EncryptionConstant.Text = "Encryption Constant:";
            Label_EncryptionConstant.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GB_CurrentMoves
            // 
            GB_CurrentMoves.Controls.Add(CB_Move4);
            GB_CurrentMoves.Controls.Add(CB_Move3);
            GB_CurrentMoves.Controls.Add(CB_Move2);
            GB_CurrentMoves.Controls.Add(CB_Move1);
            GB_CurrentMoves.Location = new System.Drawing.Point(7, 225);
            GB_CurrentMoves.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_CurrentMoves.Name = "GB_CurrentMoves";
            GB_CurrentMoves.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_CurrentMoves.Size = new System.Drawing.Size(164, 129);
            GB_CurrentMoves.TabIndex = 64;
            GB_CurrentMoves.TabStop = false;
            GB_CurrentMoves.Text = "Current Moves";
            // 
            // CB_Move4
            // 
            CB_Move4.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Move4.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Move4.FormattingEnabled = true;
            CB_Move4.Location = new System.Drawing.Point(10, 98);
            CB_Move4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Move4.Name = "CB_Move4";
            CB_Move4.Size = new System.Drawing.Size(140, 23);
            CB_Move4.TabIndex = 10;
            CB_Move4.SelectedValueChanged += Write_Entry;
            // 
            // CB_Move3
            // 
            CB_Move3.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Move3.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Move3.FormattingEnabled = true;
            CB_Move3.Location = new System.Drawing.Point(10, 73);
            CB_Move3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Move3.Name = "CB_Move3";
            CB_Move3.Size = new System.Drawing.Size(140, 23);
            CB_Move3.TabIndex = 7;
            CB_Move3.SelectedValueChanged += Write_Entry;
            // 
            // CB_Move2
            // 
            CB_Move2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Move2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Move2.FormattingEnabled = true;
            CB_Move2.Location = new System.Drawing.Point(10, 47);
            CB_Move2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Move2.Name = "CB_Move2";
            CB_Move2.Size = new System.Drawing.Size(140, 23);
            CB_Move2.TabIndex = 4;
            CB_Move2.SelectedValueChanged += Write_Entry;
            // 
            // CB_Move1
            // 
            CB_Move1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Move1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Move1.FormattingEnabled = true;
            CB_Move1.Location = new System.Drawing.Point(10, 22);
            CB_Move1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Move1.Name = "CB_Move1";
            CB_Move1.Size = new System.Drawing.Size(140, 23);
            CB_Move1.TabIndex = 1;
            CB_Move1.SelectedValueChanged += Write_Entry;
            // 
            // Label_HeldItem
            // 
            Label_HeldItem.Location = new System.Drawing.Point(128, 126);
            Label_HeldItem.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label_HeldItem.Name = "Label_HeldItem";
            Label_HeldItem.Size = new System.Drawing.Size(92, 15);
            Label_HeldItem.TabIndex = 66;
            Label_HeldItem.Text = "Held Item:";
            Label_HeldItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_HeldItem
            // 
            CB_HeldItem.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_HeldItem.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_HeldItem.FormattingEnabled = true;
            CB_HeldItem.Location = new System.Drawing.Point(222, 122);
            CB_HeldItem.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_HeldItem.Name = "CB_HeldItem";
            CB_HeldItem.Size = new System.Drawing.Size(144, 23);
            CB_HeldItem.TabIndex = 65;
            CB_HeldItem.SelectedValueChanged += Write_Entry;
            // 
            // GB_OT
            // 
            GB_OT.Controls.Add(Label_OTGender);
            GB_OT.Controls.Add(TB_OT);
            GB_OT.Controls.Add(TB_SID);
            GB_OT.Controls.Add(TB_TID);
            GB_OT.Controls.Add(Label_OT);
            GB_OT.Controls.Add(Label_SID);
            GB_OT.Controls.Add(Label_TID);
            GB_OT.Location = new System.Drawing.Point(176, 225);
            GB_OT.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_OT.Name = "GB_OT";
            GB_OT.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_OT.Size = new System.Drawing.Size(190, 87);
            GB_OT.TabIndex = 67;
            GB_OT.TabStop = false;
            GB_OT.Text = "Trainer Information";
            // 
            // Label_OTGender
            // 
            Label_OTGender.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Label_OTGender.Location = new System.Drawing.Point(159, 55);
            Label_OTGender.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label_OTGender.Name = "Label_OTGender";
            Label_OTGender.Size = new System.Drawing.Size(21, 15);
            Label_OTGender.TabIndex = 79;
            Label_OTGender.Text = "-";
            Label_OTGender.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            Label_OTGender.Click += UpdateOTGender;
            // 
            // TB_OT
            // 
            TB_OT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_OT.Location = new System.Drawing.Point(42, 53);
            TB_OT.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_OT.MaxLength = 12;
            TB_OT.Name = "TB_OT";
            TB_OT.Size = new System.Drawing.Size(109, 23);
            TB_OT.TabIndex = 3;
            TB_OT.Text = "PKHeX";
            TB_OT.TextChanged += Write_Entry;
            // 
            // TB_SID
            // 
            TB_SID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_SID.Location = new System.Drawing.Point(136, 23);
            TB_SID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_SID.Mask = "00000";
            TB_SID.Name = "TB_SID";
            TB_SID.Size = new System.Drawing.Size(46, 23);
            TB_SID.TabIndex = 2;
            TB_SID.Text = "54321";
            TB_SID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            TB_SID.TextChanged += Write_Entry;
            // 
            // TB_TID
            // 
            TB_TID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_TID.Location = new System.Drawing.Point(42, 23);
            TB_TID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_TID.Mask = "00000";
            TB_TID.Name = "TB_TID";
            TB_TID.Size = new System.Drawing.Size(46, 23);
            TB_TID.TabIndex = 1;
            TB_TID.Text = "12345";
            TB_TID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            TB_TID.TextChanged += Write_Entry;
            // 
            // Label_OT
            // 
            Label_OT.Location = new System.Drawing.Point(10, 55);
            Label_OT.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label_OT.Name = "Label_OT";
            Label_OT.Size = new System.Drawing.Size(29, 15);
            Label_OT.TabIndex = 5;
            Label_OT.Text = "OT:";
            Label_OT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_SID
            // 
            Label_SID.Location = new System.Drawing.Point(93, 25);
            Label_SID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label_SID.Name = "Label_SID";
            Label_SID.Size = new System.Drawing.Size(42, 15);
            Label_SID.TabIndex = 4;
            Label_SID.Text = "SID16:";
            Label_SID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_TID
            // 
            Label_TID.Location = new System.Drawing.Point(7, 25);
            Label_TID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label_TID.Name = "Label_TID";
            Label_TID.Size = new System.Drawing.Size(33, 15);
            Label_TID.TabIndex = 3;
            Label_TID.Text = "TID16:";
            Label_TID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Victory
            // 
            L_Victory.AutoSize = true;
            L_Victory.Location = new System.Drawing.Point(8, 29);
            L_Victory.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Victory.Name = "L_Victory";
            L_Victory.Size = new System.Drawing.Size(94, 15);
            L_Victory.TabIndex = 68;
            L_Victory.Text = "Victory Number:";
            // 
            // TB_VN
            // 
            TB_VN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_VN.Location = new System.Drawing.Point(111, 25);
            TB_VN.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_VN.Mask = "000";
            TB_VN.Name = "TB_VN";
            TB_VN.Size = new System.Drawing.Size(37, 23);
            TB_VN.TabIndex = 6;
            TB_VN.Text = "000";
            TB_VN.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            TB_VN.TextChanged += Write_Entry;
            // 
            // CAL_MetDate
            // 
            CAL_MetDate.CustomFormat = "MM/dd/yyyy";
            CAL_MetDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            CAL_MetDate.Location = new System.Drawing.Point(237, 25);
            CAL_MetDate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CAL_MetDate.MaxDate = new System.DateTime(2050, 12, 31, 0, 0, 0, 0);
            CAL_MetDate.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            CAL_MetDate.Name = "CAL_MetDate";
            CAL_MetDate.Size = new System.Drawing.Size(118, 23);
            CAL_MetDate.TabIndex = 70;
            CAL_MetDate.Value = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            CAL_MetDate.ValueChanged += Write_Entry;
            // 
            // Label_MetDate
            // 
            Label_MetDate.Location = new System.Drawing.Point(173, 29);
            Label_MetDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label_MetDate.Name = "Label_MetDate";
            Label_MetDate.Size = new System.Drawing.Size(66, 15);
            Label_MetDate.TabIndex = 69;
            Label_MetDate.Text = "Date:";
            Label_MetDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(537, 336);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(89, 27);
            B_Cancel.TabIndex = 71;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // Label_Gender
            // 
            Label_Gender.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Label_Gender.Location = new System.Drawing.Point(82, 171);
            Label_Gender.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label_Gender.Name = "Label_Gender";
            Label_Gender.Size = new System.Drawing.Size(21, 15);
            Label_Gender.TabIndex = 72;
            Label_Gender.Text = "-";
            Label_Gender.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            Label_Gender.Click += UpdateGender;
            // 
            // CB_Form
            // 
            CB_Form.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Form.DropDownWidth = 85;
            CB_Form.Enabled = false;
            CB_Form.FormattingEnabled = true;
            CB_Form.Location = new System.Drawing.Point(222, 153);
            CB_Form.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Form.Name = "CB_Form";
            CB_Form.Size = new System.Drawing.Size(144, 23);
            CB_Form.TabIndex = 74;
            CB_Form.SelectedIndexChanged += Write_Entry;
            // 
            // Label_Form
            // 
            Label_Form.AutoSize = true;
            Label_Form.Location = new System.Drawing.Point(180, 157);
            Label_Form.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label_Form.Name = "Label_Form";
            Label_Form.Size = new System.Drawing.Size(38, 15);
            Label_Form.TabIndex = 73;
            Label_Form.Text = "Form:";
            // 
            // CHK_Shiny
            // 
            CHK_Shiny.AutoSize = true;
            CHK_Shiny.Location = new System.Drawing.Point(63, 172);
            CHK_Shiny.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_Shiny.Name = "CHK_Shiny";
            CHK_Shiny.Size = new System.Drawing.Size(15, 14);
            CHK_Shiny.TabIndex = 75;
            CHK_Shiny.UseVisualStyleBackColor = true;
            CHK_Shiny.CheckedChanged += UpdateShiny;
            // 
            // L_Shiny
            // 
            L_Shiny.AutoSize = true;
            L_Shiny.Location = new System.Drawing.Point(14, 171);
            L_Shiny.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Shiny.Name = "L_Shiny";
            L_Shiny.Size = new System.Drawing.Size(39, 15);
            L_Shiny.TabIndex = 76;
            L_Shiny.Text = "Shiny:";
            // 
            // TB_Level
            // 
            TB_Level.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_Level.Location = new System.Drawing.Point(63, 193);
            TB_Level.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_Level.Mask = "000";
            TB_Level.Name = "TB_Level";
            TB_Level.Size = new System.Drawing.Size(37, 23);
            TB_Level.TabIndex = 77;
            TB_Level.Text = "001";
            TB_Level.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            TB_Level.TextChanged += Write_Entry;
            // 
            // L_Level
            // 
            L_Level.AutoSize = true;
            L_Level.Location = new System.Drawing.Point(14, 195);
            L_Level.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Level.Name = "L_Level";
            L_Level.Size = new System.Drawing.Size(37, 15);
            L_Level.TabIndex = 78;
            L_Level.Text = "Level:";
            // 
            // B_CopyText
            // 
            B_CopyText.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_CopyText.Location = new System.Drawing.Point(4, 347);
            B_CopyText.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_CopyText.Name = "B_CopyText";
            B_CopyText.Size = new System.Drawing.Size(69, 27);
            B_CopyText.TabIndex = 79;
            B_CopyText.Text = "Copy txt";
            B_CopyText.UseVisualStyleBackColor = true;
            B_CopyText.Click += B_CopyText_Click;
            // 
            // B_Delete
            // 
            B_Delete.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_Delete.Location = new System.Drawing.Point(4, 320);
            B_Delete.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Delete.Name = "B_Delete";
            B_Delete.Size = new System.Drawing.Size(69, 27);
            B_Delete.TabIndex = 80;
            B_Delete.Text = "Delete";
            B_Delete.UseVisualStyleBackColor = true;
            B_Delete.Click += B_Delete_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(L_Victory);
            groupBox1.Controls.Add(bpkx);
            groupBox1.Controls.Add(NUP_PartyIndex);
            groupBox1.Controls.Add(TB_Level);
            groupBox1.Controls.Add(L_PartyNum);
            groupBox1.Controls.Add(L_Level);
            groupBox1.Controls.Add(Label_Species);
            groupBox1.Controls.Add(L_Shiny);
            groupBox1.Controls.Add(CB_Species);
            groupBox1.Controls.Add(CHK_Shiny);
            groupBox1.Controls.Add(TB_Nickname);
            groupBox1.Controls.Add(CB_Form);
            groupBox1.Controls.Add(CHK_Nicknamed);
            groupBox1.Controls.Add(Label_Form);
            groupBox1.Controls.Add(Label_EncryptionConstant);
            groupBox1.Controls.Add(Label_Gender);
            groupBox1.Controls.Add(TB_EC);
            groupBox1.Controls.Add(GB_CurrentMoves);
            groupBox1.Controls.Add(CAL_MetDate);
            groupBox1.Controls.Add(CB_HeldItem);
            groupBox1.Controls.Add(Label_MetDate);
            groupBox1.Controls.Add(Label_HeldItem);
            groupBox1.Controls.Add(TB_VN);
            groupBox1.Controls.Add(GB_OT);
            groupBox1.Location = new System.Drawing.Point(349, 14);
            groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Size = new System.Drawing.Size(377, 363);
            groupBox1.TabIndex = 81;
            groupBox1.TabStop = false;
            groupBox1.Text = "Entry";
            // 
            // SAV_HallOfFame
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(724, 376);
            Controls.Add(B_Delete);
            Controls.Add(B_CopyText);
            Controls.Add(B_Cancel);
            Controls.Add(B_Close);
            Controls.Add(RTB);
            Controls.Add(LB_DataEntry);
            Controls.Add(groupBox1);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(721, 386);
            Name = "SAV_HallOfFame";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Hall of Fame Viewer";
            ((System.ComponentModel.ISupportInitialize)bpkx).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUP_PartyIndex).EndInit();
            GB_CurrentMoves.ResumeLayout(false);
            GB_OT.ResumeLayout(false);
            GB_OT.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ListBox LB_DataEntry;
        private System.Windows.Forms.RichTextBox RTB;
        private System.Windows.Forms.Button B_Close;
        private System.Windows.Forms.PictureBox bpkx;
        private System.Windows.Forms.NumericUpDown NUP_PartyIndex;
        private System.Windows.Forms.Label L_PartyNum;
        public System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.Label Label_Species;
        private System.Windows.Forms.CheckBox CHK_Nicknamed;
        public System.Windows.Forms.TextBox TB_Nickname;
        private System.Windows.Forms.TextBox TB_EC;
        private System.Windows.Forms.Label Label_EncryptionConstant;
        private System.Windows.Forms.GroupBox GB_CurrentMoves;
        private System.Windows.Forms.ComboBox CB_Move4;
        private System.Windows.Forms.ComboBox CB_Move3;
        private System.Windows.Forms.ComboBox CB_Move2;
        public System.Windows.Forms.ComboBox CB_Move1;
        private System.Windows.Forms.Label Label_HeldItem;
        private System.Windows.Forms.ComboBox CB_HeldItem;
        public System.Windows.Forms.GroupBox GB_OT;
        public System.Windows.Forms.TextBox TB_OT;
        private System.Windows.Forms.MaskedTextBox TB_SID;
        private System.Windows.Forms.MaskedTextBox TB_TID;
        private System.Windows.Forms.Label Label_OT;
        private System.Windows.Forms.Label Label_SID;
        private System.Windows.Forms.Label Label_TID;
        private System.Windows.Forms.Label L_Victory;
        private System.Windows.Forms.MaskedTextBox TB_VN;
        private System.Windows.Forms.DateTimePicker CAL_MetDate;
        private System.Windows.Forms.Label Label_MetDate;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Label Label_Gender;
        private System.Windows.Forms.ComboBox CB_Form;
        private System.Windows.Forms.Label Label_Form;
        private System.Windows.Forms.CheckBox CHK_Shiny;
        private System.Windows.Forms.Label L_Shiny;
        private System.Windows.Forms.MaskedTextBox TB_Level;
        private System.Windows.Forms.Label L_Level;
        private System.Windows.Forms.Button B_CopyText;
        private System.Windows.Forms.Button B_Delete;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label Label_OTGender;
    }
}
