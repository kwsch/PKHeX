namespace PKHeX
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SAV_HallOfFame));
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.RTB = new System.Windows.Forms.RichTextBox();
            this.B_Close = new System.Windows.Forms.Button();
            this.bpkx = new System.Windows.Forms.PictureBox();
            this.NUP_PartyIndex = new System.Windows.Forms.NumericUpDown();
            this.L_PartyNum = new System.Windows.Forms.Label();
            this.CB_Species = new System.Windows.Forms.ComboBox();
            this.Label_Species = new System.Windows.Forms.Label();
            this.CHK_Nicknamed = new System.Windows.Forms.CheckBox();
            this.TB_Nickname = new System.Windows.Forms.TextBox();
            this.TB_EC = new System.Windows.Forms.TextBox();
            this.Label_EncryptionConstant = new System.Windows.Forms.Label();
            this.GB_CurrentMoves = new System.Windows.Forms.GroupBox();
            this.CB_Move4 = new System.Windows.Forms.ComboBox();
            this.CB_Move3 = new System.Windows.Forms.ComboBox();
            this.CB_Move2 = new System.Windows.Forms.ComboBox();
            this.CB_Move1 = new System.Windows.Forms.ComboBox();
            this.Label_HeldItem = new System.Windows.Forms.Label();
            this.CB_HeldItem = new System.Windows.Forms.ComboBox();
            this.GB_OT = new System.Windows.Forms.GroupBox();
            this.TB_OT = new System.Windows.Forms.TextBox();
            this.TB_SID = new System.Windows.Forms.MaskedTextBox();
            this.TB_TID = new System.Windows.Forms.MaskedTextBox();
            this.Label_OT = new System.Windows.Forms.Label();
            this.Label_SID = new System.Windows.Forms.Label();
            this.Label_TID = new System.Windows.Forms.Label();
            this.L_Victory = new System.Windows.Forms.Label();
            this.TB_VN = new System.Windows.Forms.MaskedTextBox();
            this.CAL_MetDate = new System.Windows.Forms.DateTimePicker();
            this.Label_MetDate = new System.Windows.Forms.Label();
            this.B_Cancel = new System.Windows.Forms.Button();
            this.Label_Gender = new System.Windows.Forms.Label();
            this.CB_Form = new System.Windows.Forms.ComboBox();
            this.Label_Form = new System.Windows.Forms.Label();
            this.CHK_Shiny = new System.Windows.Forms.CheckBox();
            this.L_Shiny = new System.Windows.Forms.Label();
            this.TB_Level = new System.Windows.Forms.MaskedTextBox();
            this.L_Level = new System.Windows.Forms.Label();
            this.B_CopyText = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.bpkx)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_PartyIndex)).BeginInit();
            this.GB_CurrentMoves.SuspendLayout();
            this.GB_OT.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Items.AddRange(new object[] {
            "First",
            "01",
            "02",
            "03",
            "04",
            "05",
            "06",
            "07",
            "08",
            "09",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15"});
            this.listBox1.Location = new System.Drawing.Point(7, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(59, 264);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.displayEntry);
            // 
            // RTB
            // 
            this.RTB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RTB.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RTB.Location = new System.Drawing.Point(72, 16);
            this.RTB.Name = "RTB";
            this.RTB.ReadOnly = true;
            this.RTB.Size = new System.Drawing.Size(220, 287);
            this.RTB.TabIndex = 1;
            this.RTB.Text = "";
            this.RTB.WordWrap = false;
            // 
            // B_Close
            // 
            this.B_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Close.Location = new System.Drawing.Point(529, 270);
            this.B_Close.Name = "B_Close";
            this.B_Close.Size = new System.Drawing.Size(76, 23);
            this.B_Close.TabIndex = 3;
            this.B_Close.Text = "Save";
            this.B_Close.UseVisualStyleBackColor = true;
            this.B_Close.Click += new System.EventHandler(this.B_Close_Click);
            // 
            // bpkx
            // 
            this.bpkx.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bpkx.Location = new System.Drawing.Point(308, 99);
            this.bpkx.Name = "bpkx";
            this.bpkx.Size = new System.Drawing.Size(42, 32);
            this.bpkx.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.bpkx.TabIndex = 31;
            this.bpkx.TabStop = false;
            // 
            // NUP_PartyIndex
            // 
            this.NUP_PartyIndex.Location = new System.Drawing.Point(369, 47);
            this.NUP_PartyIndex.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.NUP_PartyIndex.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUP_PartyIndex.Name = "NUP_PartyIndex";
            this.NUP_PartyIndex.Size = new System.Drawing.Size(28, 20);
            this.NUP_PartyIndex.TabIndex = 32;
            this.NUP_PartyIndex.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUP_PartyIndex.ValueChanged += new System.EventHandler(this.NUP_PartyIndex_ValueChanged);
            // 
            // L_PartyNum
            // 
            this.L_PartyNum.AutoSize = true;
            this.L_PartyNum.Location = new System.Drawing.Point(300, 49);
            this.L_PartyNum.Name = "L_PartyNum";
            this.L_PartyNum.Size = new System.Drawing.Size(63, 13);
            this.L_PartyNum.TabIndex = 33;
            this.L_PartyNum.Text = "Party Index:";
            // 
            // CB_Species
            // 
            this.CB_Species.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_Species.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Species.FormattingEnabled = true;
            this.CB_Species.Location = new System.Drawing.Point(483, 46);
            this.CB_Species.Name = "CB_Species";
            this.CB_Species.Size = new System.Drawing.Size(122, 21);
            this.CB_Species.TabIndex = 35;
            this.CB_Species.SelectedValueChanged += new System.EventHandler(this.updateSpecies);
            // 
            // Label_Species
            // 
            this.Label_Species.Location = new System.Drawing.Point(428, 49);
            this.Label_Species.Name = "Label_Species";
            this.Label_Species.Size = new System.Drawing.Size(50, 13);
            this.Label_Species.TabIndex = 34;
            this.Label_Species.Text = "Species:";
            this.Label_Species.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CHK_Nicknamed
            // 
            this.CHK_Nicknamed.Location = new System.Drawing.Point(403, 73);
            this.CHK_Nicknamed.Name = "CHK_Nicknamed";
            this.CHK_Nicknamed.Size = new System.Drawing.Size(80, 17);
            this.CHK_Nicknamed.TabIndex = 36;
            this.CHK_Nicknamed.Text = "Nickname:";
            this.CHK_Nicknamed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CHK_Nicknamed.UseVisualStyleBackColor = true;
            this.CHK_Nicknamed.CheckedChanged += new System.EventHandler(this.updateNickname);
            // 
            // TB_Nickname
            // 
            this.TB_Nickname.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_Nickname.Location = new System.Drawing.Point(483, 71);
            this.TB_Nickname.MaxLength = 11;
            this.TB_Nickname.Name = "TB_Nickname";
            this.TB_Nickname.Size = new System.Drawing.Size(122, 20);
            this.TB_Nickname.TabIndex = 37;
            this.TB_Nickname.TextChanged += new System.EventHandler(this.Write_Entry);
            // 
            // TB_EC
            // 
            this.TB_EC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_EC.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_EC.Location = new System.Drawing.Point(545, 150);
            this.TB_EC.MaxLength = 8;
            this.TB_EC.Name = "TB_EC";
            this.TB_EC.Size = new System.Drawing.Size(60, 20);
            this.TB_EC.TabIndex = 63;
            this.TB_EC.Text = "12345678";
            this.TB_EC.TextChanged += new System.EventHandler(this.Write_Entry);
            // 
            // Label_EncryptionConstant
            // 
            this.Label_EncryptionConstant.Location = new System.Drawing.Point(438, 153);
            this.Label_EncryptionConstant.Name = "Label_EncryptionConstant";
            this.Label_EncryptionConstant.Size = new System.Drawing.Size(105, 13);
            this.Label_EncryptionConstant.TabIndex = 62;
            this.Label_EncryptionConstant.Text = "Encryption Constant:";
            this.Label_EncryptionConstant.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GB_CurrentMoves
            // 
            this.GB_CurrentMoves.Controls.Add(this.CB_Move4);
            this.GB_CurrentMoves.Controls.Add(this.CB_Move3);
            this.GB_CurrentMoves.Controls.Add(this.CB_Move2);
            this.GB_CurrentMoves.Controls.Add(this.CB_Move1);
            this.GB_CurrentMoves.Location = new System.Drawing.Point(299, 185);
            this.GB_CurrentMoves.Name = "GB_CurrentMoves";
            this.GB_CurrentMoves.Size = new System.Drawing.Size(139, 112);
            this.GB_CurrentMoves.TabIndex = 64;
            this.GB_CurrentMoves.TabStop = false;
            this.GB_CurrentMoves.Text = "Current Moves";
            // 
            // CB_Move4
            // 
            this.CB_Move4.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_Move4.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Move4.FormattingEnabled = true;
            this.CB_Move4.Location = new System.Drawing.Point(9, 85);
            this.CB_Move4.Name = "CB_Move4";
            this.CB_Move4.Size = new System.Drawing.Size(121, 21);
            this.CB_Move4.TabIndex = 10;
            this.CB_Move4.SelectedValueChanged += new System.EventHandler(this.Write_Entry);
            // 
            // CB_Move3
            // 
            this.CB_Move3.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_Move3.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Move3.FormattingEnabled = true;
            this.CB_Move3.Location = new System.Drawing.Point(9, 63);
            this.CB_Move3.Name = "CB_Move3";
            this.CB_Move3.Size = new System.Drawing.Size(121, 21);
            this.CB_Move3.TabIndex = 7;
            this.CB_Move3.SelectedValueChanged += new System.EventHandler(this.Write_Entry);
            // 
            // CB_Move2
            // 
            this.CB_Move2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_Move2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Move2.FormattingEnabled = true;
            this.CB_Move2.Location = new System.Drawing.Point(9, 41);
            this.CB_Move2.Name = "CB_Move2";
            this.CB_Move2.Size = new System.Drawing.Size(121, 21);
            this.CB_Move2.TabIndex = 4;
            this.CB_Move2.SelectedValueChanged += new System.EventHandler(this.Write_Entry);
            // 
            // CB_Move1
            // 
            this.CB_Move1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_Move1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Move1.FormattingEnabled = true;
            this.CB_Move1.Location = new System.Drawing.Point(9, 19);
            this.CB_Move1.Name = "CB_Move1";
            this.CB_Move1.Size = new System.Drawing.Size(121, 21);
            this.CB_Move1.TabIndex = 1;
            this.CB_Move1.SelectedValueChanged += new System.EventHandler(this.Write_Entry);
            // 
            // Label_HeldItem
            // 
            this.Label_HeldItem.Location = new System.Drawing.Point(403, 99);
            this.Label_HeldItem.Name = "Label_HeldItem";
            this.Label_HeldItem.Size = new System.Drawing.Size(77, 13);
            this.Label_HeldItem.TabIndex = 66;
            this.Label_HeldItem.Text = "Held Item:";
            this.Label_HeldItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_HeldItem
            // 
            this.CB_HeldItem.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_HeldItem.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_HeldItem.FormattingEnabled = true;
            this.CB_HeldItem.Location = new System.Drawing.Point(483, 96);
            this.CB_HeldItem.Name = "CB_HeldItem";
            this.CB_HeldItem.Size = new System.Drawing.Size(122, 21);
            this.CB_HeldItem.TabIndex = 65;
            this.CB_HeldItem.SelectedValueChanged += new System.EventHandler(this.Write_Entry);
            // 
            // GB_OT
            // 
            this.GB_OT.Controls.Add(this.TB_OT);
            this.GB_OT.Controls.Add(this.TB_SID);
            this.GB_OT.Controls.Add(this.TB_TID);
            this.GB_OT.Controls.Add(this.Label_OT);
            this.GB_OT.Controls.Add(this.Label_SID);
            this.GB_OT.Controls.Add(this.Label_TID);
            this.GB_OT.Location = new System.Drawing.Point(444, 185);
            this.GB_OT.Name = "GB_OT";
            this.GB_OT.Size = new System.Drawing.Size(161, 75);
            this.GB_OT.TabIndex = 67;
            this.GB_OT.TabStop = false;
            this.GB_OT.Text = "Trainer Information";
            // 
            // TB_OT
            // 
            this.TB_OT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_OT.Location = new System.Drawing.Point(36, 46);
            this.TB_OT.MaxLength = 11;
            this.TB_OT.Name = "TB_OT";
            this.TB_OT.Size = new System.Drawing.Size(94, 20);
            this.TB_OT.TabIndex = 3;
            this.TB_OT.Text = "PKHeX";
            this.TB_OT.TextChanged += new System.EventHandler(this.Write_Entry);
            // 
            // TB_SID
            // 
            this.TB_SID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_SID.Location = new System.Drawing.Point(117, 20);
            this.TB_SID.Mask = "00000";
            this.TB_SID.Name = "TB_SID";
            this.TB_SID.Size = new System.Drawing.Size(40, 20);
            this.TB_SID.TabIndex = 2;
            this.TB_SID.Text = "54321";
            this.TB_SID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_SID.TextChanged += new System.EventHandler(this.Write_Entry);
            // 
            // TB_TID
            // 
            this.TB_TID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_TID.Location = new System.Drawing.Point(36, 20);
            this.TB_TID.Mask = "00000";
            this.TB_TID.Name = "TB_TID";
            this.TB_TID.Size = new System.Drawing.Size(40, 20);
            this.TB_TID.TabIndex = 1;
            this.TB_TID.Text = "12345";
            this.TB_TID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_TID.TextChanged += new System.EventHandler(this.Write_Entry);
            // 
            // Label_OT
            // 
            this.Label_OT.Location = new System.Drawing.Point(9, 48);
            this.Label_OT.Name = "Label_OT";
            this.Label_OT.Size = new System.Drawing.Size(25, 13);
            this.Label_OT.TabIndex = 5;
            this.Label_OT.Text = "OT:";
            this.Label_OT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_SID
            // 
            this.Label_SID.Location = new System.Drawing.Point(80, 22);
            this.Label_SID.Name = "Label_SID";
            this.Label_SID.Size = new System.Drawing.Size(36, 13);
            this.Label_SID.TabIndex = 4;
            this.Label_SID.Text = "SID:";
            this.Label_SID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_TID
            // 
            this.Label_TID.Location = new System.Drawing.Point(6, 22);
            this.Label_TID.Name = "Label_TID";
            this.Label_TID.Size = new System.Drawing.Size(28, 13);
            this.Label_TID.TabIndex = 3;
            this.Label_TID.Text = "TID:";
            this.Label_TID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Victory
            // 
            this.L_Victory.AutoSize = true;
            this.L_Victory.Location = new System.Drawing.Point(300, 15);
            this.L_Victory.Name = "L_Victory";
            this.L_Victory.Size = new System.Drawing.Size(82, 13);
            this.L_Victory.TabIndex = 68;
            this.L_Victory.Text = "Victory Number:";
            // 
            // TB_VN
            // 
            this.TB_VN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_VN.Location = new System.Drawing.Point(388, 12);
            this.TB_VN.Mask = "000";
            this.TB_VN.Name = "TB_VN";
            this.TB_VN.Size = new System.Drawing.Size(30, 20);
            this.TB_VN.TabIndex = 6;
            this.TB_VN.Text = "000";
            this.TB_VN.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_VN.TextChanged += new System.EventHandler(this.Write_Entry);
            // 
            // CAL_MetDate
            // 
            this.CAL_MetDate.CustomFormat = "MM/dd/yyyy";
            this.CAL_MetDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.CAL_MetDate.Location = new System.Drawing.Point(496, 12);
            this.CAL_MetDate.MaxDate = new System.DateTime(2099, 12, 31, 0, 0, 0, 0);
            this.CAL_MetDate.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.CAL_MetDate.Name = "CAL_MetDate";
            this.CAL_MetDate.Size = new System.Drawing.Size(100, 20);
            this.CAL_MetDate.TabIndex = 70;
            this.CAL_MetDate.Value = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.CAL_MetDate.ValueChanged += new System.EventHandler(this.Write_Entry);
            // 
            // Label_MetDate
            // 
            this.Label_MetDate.Location = new System.Drawing.Point(441, 15);
            this.Label_MetDate.Name = "Label_MetDate";
            this.Label_MetDate.Size = new System.Drawing.Size(55, 13);
            this.Label_MetDate.TabIndex = 69;
            this.Label_MetDate.Text = "Date:";
            this.Label_MetDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(447, 270);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(76, 23);
            this.B_Cancel.TabIndex = 71;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // Label_Gender
            // 
            this.Label_Gender.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label_Gender.Location = new System.Drawing.Point(363, 138);
            this.Label_Gender.Name = "Label_Gender";
            this.Label_Gender.Size = new System.Drawing.Size(16, 13);
            this.Label_Gender.TabIndex = 72;
            this.Label_Gender.Text = "-";
            this.Label_Gender.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Label_Gender.Click += new System.EventHandler(this.updateGender);
            // 
            // CB_Form
            // 
            this.CB_Form.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Form.DropDownWidth = 85;
            this.CB_Form.Enabled = false;
            this.CB_Form.FormattingEnabled = true;
            this.CB_Form.Location = new System.Drawing.Point(483, 123);
            this.CB_Form.Name = "CB_Form";
            this.CB_Form.Size = new System.Drawing.Size(122, 21);
            this.CB_Form.TabIndex = 74;
            this.CB_Form.SelectedIndexChanged += new System.EventHandler(this.Write_Entry);
            // 
            // Label_Form
            // 
            this.Label_Form.AutoSize = true;
            this.Label_Form.Location = new System.Drawing.Point(447, 126);
            this.Label_Form.Name = "Label_Form";
            this.Label_Form.Size = new System.Drawing.Size(33, 13);
            this.Label_Form.TabIndex = 73;
            this.Label_Form.Text = "Form:";
            // 
            // CHK_Shiny
            // 
            this.CHK_Shiny.AutoSize = true;
            this.CHK_Shiny.Location = new System.Drawing.Point(347, 139);
            this.CHK_Shiny.Name = "CHK_Shiny";
            this.CHK_Shiny.Size = new System.Drawing.Size(15, 14);
            this.CHK_Shiny.TabIndex = 75;
            this.CHK_Shiny.UseVisualStyleBackColor = true;
            this.CHK_Shiny.CheckedChanged += new System.EventHandler(this.updateShiny);
            // 
            // L_Shiny
            // 
            this.L_Shiny.AutoSize = true;
            this.L_Shiny.Location = new System.Drawing.Point(305, 138);
            this.L_Shiny.Name = "L_Shiny";
            this.L_Shiny.Size = new System.Drawing.Size(36, 13);
            this.L_Shiny.TabIndex = 76;
            this.L_Shiny.Text = "Shiny:";
            // 
            // TB_Level
            // 
            this.TB_Level.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_Level.Location = new System.Drawing.Point(347, 157);
            this.TB_Level.Mask = "000";
            this.TB_Level.Name = "TB_Level";
            this.TB_Level.Size = new System.Drawing.Size(30, 20);
            this.TB_Level.TabIndex = 77;
            this.TB_Level.Text = "001";
            this.TB_Level.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_Level.TextChanged += new System.EventHandler(this.Write_Entry);
            // 
            // L_Level
            // 
            this.L_Level.AutoSize = true;
            this.L_Level.Location = new System.Drawing.Point(305, 159);
            this.L_Level.Name = "L_Level";
            this.L_Level.Size = new System.Drawing.Size(36, 13);
            this.L_Level.TabIndex = 78;
            this.L_Level.Text = "Level:";
            // 
            // B_CopyText
            // 
            this.B_CopyText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_CopyText.Location = new System.Drawing.Point(7, 280);
            this.B_CopyText.Name = "B_CopyText";
            this.B_CopyText.Size = new System.Drawing.Size(59, 23);
            this.B_CopyText.TabIndex = 79;
            this.B_CopyText.Text = "Copy txt";
            this.B_CopyText.UseVisualStyleBackColor = true;
            this.B_CopyText.Click += new System.EventHandler(this.B_CopyText_Click);
            // 
            // SAV_HallOfFame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(614, 312);
            this.Controls.Add(this.B_CopyText);
            this.Controls.Add(this.TB_Level);
            this.Controls.Add(this.L_Level);
            this.Controls.Add(this.L_Shiny);
            this.Controls.Add(this.CHK_Shiny);
            this.Controls.Add(this.CB_Form);
            this.Controls.Add(this.Label_Form);
            this.Controls.Add(this.Label_Gender);
            this.Controls.Add(this.B_Cancel);
            this.Controls.Add(this.CAL_MetDate);
            this.Controls.Add(this.Label_MetDate);
            this.Controls.Add(this.TB_VN);
            this.Controls.Add(this.L_Victory);
            this.Controls.Add(this.GB_OT);
            this.Controls.Add(this.Label_HeldItem);
            this.Controls.Add(this.CB_HeldItem);
            this.Controls.Add(this.GB_CurrentMoves);
            this.Controls.Add(this.TB_EC);
            this.Controls.Add(this.Label_EncryptionConstant);
            this.Controls.Add(this.CHK_Nicknamed);
            this.Controls.Add(this.TB_Nickname);
            this.Controls.Add(this.CB_Species);
            this.Controls.Add(this.Label_Species);
            this.Controls.Add(this.L_PartyNum);
            this.Controls.Add(this.NUP_PartyIndex);
            this.Controls.Add(this.bpkx);
            this.Controls.Add(this.B_Close);
            this.Controls.Add(this.RTB);
            this.Controls.Add(this.listBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(620, 340);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(620, 340);
            this.Name = "SAV_HallOfFame";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Hall of Fame Viewer";
            ((System.ComponentModel.ISupportInitialize)(this.bpkx)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_PartyIndex)).EndInit();
            this.GB_CurrentMoves.ResumeLayout(false);
            this.GB_OT.ResumeLayout(false);
            this.GB_OT.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
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
    }
}