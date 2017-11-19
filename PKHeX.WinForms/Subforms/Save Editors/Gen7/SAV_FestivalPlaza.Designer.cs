namespace PKHeX.WinForms
{
    partial class SAV_FestivalPlaza
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SAV_FestivalPlaza));
            this.NUD_FC_Current = new System.Windows.Forms.NumericUpDown();
            this.L_FC_Current = new System.Windows.Forms.Label();
            this.GB_FC = new System.Windows.Forms.GroupBox();
            this.L_FC_CollectedV = new System.Windows.Forms.Label();
            this.L_FC_CollectedL = new System.Windows.Forms.Label();
            this.NUD_FC_Used = new System.Windows.Forms.NumericUpDown();
            this.L_FC_Used = new System.Windows.Forms.Label();
            this.B_Save = new System.Windows.Forms.Button();
            this.B_Cancel = new System.Windows.Forms.Button();
            this.GB_Phrase = new System.Windows.Forms.GroupBox();
            this.CLB_Phrases = new System.Windows.Forms.CheckedListBox();
            this.B_AllPhrases = new System.Windows.Forms.Button();
            this.TB_OTName = new System.Windows.Forms.TextBox();
            this.GB_Facility = new System.Windows.Forms.GroupBox();
            this.L_FacilityColorV = new System.Windows.Forms.Label();
            this.L_VisitorMisc = new System.Windows.Forms.Label();
            this.TB_FacilityID = new System.Windows.Forms.TextBox();
            this.CB_FacilityID = new System.Windows.Forms.ComboBox();
            this.GB_FacilityMessage = new System.Windows.Forms.GroupBox();
            this.NUD_FacilityMessage = new System.Windows.Forms.NumericUpDown();
            this.CB_FacilityMessage = new System.Windows.Forms.ComboBox();
            this.Label_OTGender = new System.Windows.Forms.Label();
            this.CHK_FacilityIntroduced = new System.Windows.Forms.CheckBox();
            this.L_FacilityNPC = new System.Windows.Forms.Label();
            this.NUD_FacilityColor = new System.Windows.Forms.NumericUpDown();
            this.L_FacilityColor = new System.Windows.Forms.Label();
            this.L_FacilityType = new System.Windows.Forms.Label();
            this.L_FacilityIndex = new System.Windows.Forms.Label();
            this.CB_FacilityNPC = new System.Windows.Forms.ComboBox();
            this.CB_FacilityType = new System.Windows.Forms.ComboBox();
            this.NUD_FacilityIndex = new System.Windows.Forms.NumericUpDown();
            this.CAL_FestaStartDate = new System.Windows.Forms.DateTimePicker();
            this.CAL_FestaStartTime = new System.Windows.Forms.DateTimePicker();
            this.GB_FestaStartTime = new System.Windows.Forms.GroupBox();
            this.GB_Reward = new System.Windows.Forms.GroupBox();
            this.B_AllReadyReward = new System.Windows.Forms.Button();
            this.CLB_Reward = new System.Windows.Forms.CheckedListBox();
            this.B_AllReceiveReward = new System.Windows.Forms.Button();
            this.NUD_Rank = new System.Windows.Forms.NumericUpDown();
            this.L_Rank = new System.Windows.Forms.Label();
            this.GB_MyMessage = new System.Windows.Forms.GroupBox();
            this.NUD_MyMessage = new System.Windows.Forms.NumericUpDown();
            this.CB_MyMessage = new System.Windows.Forms.ComboBox();
            this.L_Note = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_FC_Current)).BeginInit();
            this.GB_FC.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_FC_Used)).BeginInit();
            this.GB_Phrase.SuspendLayout();
            this.GB_Facility.SuspendLayout();
            this.GB_FacilityMessage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_FacilityMessage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_FacilityColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_FacilityIndex)).BeginInit();
            this.GB_FestaStartTime.SuspendLayout();
            this.GB_Reward.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Rank)).BeginInit();
            this.GB_MyMessage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_MyMessage)).BeginInit();
            this.SuspendLayout();
            // 
            // NUD_FC_Current
            // 
            this.NUD_FC_Current.Location = new System.Drawing.Point(82, 18);
            this.NUD_FC_Current.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.NUD_FC_Current.Name = "NUD_FC_Current";
            this.NUD_FC_Current.Size = new System.Drawing.Size(66, 19);
            this.NUD_FC_Current.TabIndex = 1;
            this.NUD_FC_Current.Value = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.NUD_FC_Current.ValueChanged += new System.EventHandler(this.NUD_FC_ValueChanged);
            // 
            // L_FC_Current
            // 
            this.L_FC_Current.Location = new System.Drawing.Point(6, 17);
            this.L_FC_Current.Name = "L_FC_Current";
            this.L_FC_Current.Size = new System.Drawing.Size(70, 18);
            this.L_FC_Current.TabIndex = 2;
            this.L_FC_Current.Text = "Current:";
            this.L_FC_Current.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GB_FC
            // 
            this.GB_FC.Controls.Add(this.L_FC_CollectedV);
            this.GB_FC.Controls.Add(this.L_FC_CollectedL);
            this.GB_FC.Controls.Add(this.NUD_FC_Used);
            this.GB_FC.Controls.Add(this.L_FC_Used);
            this.GB_FC.Controls.Add(this.NUD_FC_Current);
            this.GB_FC.Controls.Add(this.L_FC_Current);
            this.GB_FC.Location = new System.Drawing.Point(12, 12);
            this.GB_FC.Name = "GB_FC";
            this.GB_FC.Size = new System.Drawing.Size(157, 96);
            this.GB_FC.TabIndex = 2;
            this.GB_FC.TabStop = false;
            this.GB_FC.Text = "Festa Coins";
            // 
            // L_FC_CollectedV
            // 
            this.L_FC_CollectedV.Location = new System.Drawing.Point(82, 67);
            this.L_FC_CollectedV.Name = "L_FC_CollectedV";
            this.L_FC_CollectedV.Size = new System.Drawing.Size(62, 18);
            this.L_FC_CollectedV.TabIndex = 6;
            this.L_FC_CollectedV.Text = "9999999";
            this.L_FC_CollectedV.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // L_FC_CollectedL
            // 
            this.L_FC_CollectedL.Location = new System.Drawing.Point(6, 67);
            this.L_FC_CollectedL.Name = "L_FC_CollectedL";
            this.L_FC_CollectedL.Size = new System.Drawing.Size(70, 18);
            this.L_FC_CollectedL.TabIndex = 5;
            this.L_FC_CollectedL.Text = "Collected:";
            this.L_FC_CollectedL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_FC_Used
            // 
            this.NUD_FC_Used.Location = new System.Drawing.Point(82, 43);
            this.NUD_FC_Used.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.NUD_FC_Used.Name = "NUD_FC_Used";
            this.NUD_FC_Used.Size = new System.Drawing.Size(66, 19);
            this.NUD_FC_Used.TabIndex = 3;
            this.NUD_FC_Used.Value = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.NUD_FC_Used.ValueChanged += new System.EventHandler(this.NUD_FC_ValueChanged);
            // 
            // L_FC_Used
            // 
            this.L_FC_Used.Location = new System.Drawing.Point(6, 42);
            this.L_FC_Used.Name = "L_FC_Used";
            this.L_FC_Used.Size = new System.Drawing.Size(70, 18);
            this.L_FC_Used.TabIndex = 4;
            this.L_FC_Used.Text = "Used:";
            this.L_FC_Used.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(398, 446);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(75, 21);
            this.B_Save.TabIndex = 1;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(317, 446);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(75, 21);
            this.B_Cancel.TabIndex = 0;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // GB_Phrase
            // 
            this.GB_Phrase.Controls.Add(this.CLB_Phrases);
            this.GB_Phrase.Controls.Add(this.B_AllPhrases);
            this.GB_Phrase.Location = new System.Drawing.Point(12, 117);
            this.GB_Phrase.Name = "GB_Phrase";
            this.GB_Phrase.Size = new System.Drawing.Size(246, 146);
            this.GB_Phrase.TabIndex = 3;
            this.GB_Phrase.TabStop = false;
            this.GB_Phrase.Text = "Common Phrases+";
            // 
            // CLB_Phrases
            // 
            this.CLB_Phrases.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CLB_Phrases.CheckOnClick = true;
            this.CLB_Phrases.FormattingEnabled = true;
            this.CLB_Phrases.IntegralHeight = false;
            this.CLB_Phrases.Location = new System.Drawing.Point(9, 53);
            this.CLB_Phrases.Name = "CLB_Phrases";
            this.CLB_Phrases.Size = new System.Drawing.Size(228, 84);
            this.CLB_Phrases.TabIndex = 1;
            // 
            // B_AllPhrases
            // 
            this.B_AllPhrases.Location = new System.Drawing.Point(9, 21);
            this.B_AllPhrases.Name = "B_AllPhrases";
            this.B_AllPhrases.Size = new System.Drawing.Size(75, 23);
            this.B_AllPhrases.TabIndex = 0;
            this.B_AllPhrases.Text = "Check All";
            this.B_AllPhrases.UseVisualStyleBackColor = true;
            this.B_AllPhrases.Click += new System.EventHandler(this.B_AllPhrases_Click);
            // 
            // TB_OTName
            // 
            this.TB_OTName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_OTName.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_OTName.Location = new System.Drawing.Point(327, 21);
            this.TB_OTName.MaxLength = 12;
            this.TB_OTName.Name = "TB_OTName";
            this.TB_OTName.Size = new System.Drawing.Size(93, 20);
            this.TB_OTName.TabIndex = 4;
            this.TB_OTName.Text = "WWWWWWWWWWWW";
            this.TB_OTName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_OTName.TextChanged += new System.EventHandler(this.TB_OTName_TextChanged);
            this.TB_OTName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TB_OTName_MouseDown);
            // 
            // GB_Facility
            // 
            this.GB_Facility.Controls.Add(this.L_FacilityColorV);
            this.GB_Facility.Controls.Add(this.L_VisitorMisc);
            this.GB_Facility.Controls.Add(this.TB_FacilityID);
            this.GB_Facility.Controls.Add(this.CB_FacilityID);
            this.GB_Facility.Controls.Add(this.GB_FacilityMessage);
            this.GB_Facility.Controls.Add(this.Label_OTGender);
            this.GB_Facility.Controls.Add(this.CHK_FacilityIntroduced);
            this.GB_Facility.Controls.Add(this.L_FacilityNPC);
            this.GB_Facility.Controls.Add(this.NUD_FacilityColor);
            this.GB_Facility.Controls.Add(this.L_FacilityColor);
            this.GB_Facility.Controls.Add(this.L_FacilityType);
            this.GB_Facility.Controls.Add(this.L_FacilityIndex);
            this.GB_Facility.Controls.Add(this.CB_FacilityNPC);
            this.GB_Facility.Controls.Add(this.CB_FacilityType);
            this.GB_Facility.Controls.Add(this.NUD_FacilityIndex);
            this.GB_Facility.Controls.Add(this.TB_OTName);
            this.GB_Facility.Location = new System.Drawing.Point(12, 269);
            this.GB_Facility.Name = "GB_Facility";
            this.GB_Facility.Size = new System.Drawing.Size(455, 165);
            this.GB_Facility.TabIndex = 5;
            this.GB_Facility.TabStop = false;
            this.GB_Facility.Text = "Facilities";
            // 
            // L_FacilityColorV
            // 
            this.L_FacilityColorV.Location = new System.Drawing.Point(122, 77);
            this.L_FacilityColorV.Name = "L_FacilityColorV";
            this.L_FacilityColorV.Size = new System.Drawing.Size(100, 18);
            this.L_FacilityColorV.TabIndex = 60;
            this.L_FacilityColorV.Text = "colorvalue";
            this.L_FacilityColorV.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // L_VisitorMisc
            // 
            this.L_VisitorMisc.Location = new System.Drawing.Point(282, 108);
            this.L_VisitorMisc.Name = "L_VisitorMisc";
            this.L_VisitorMisc.Size = new System.Drawing.Size(82, 18);
            this.L_VisitorMisc.TabIndex = 59;
            this.L_VisitorMisc.Text = "visitor misc:";
            this.L_VisitorMisc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TB_FacilityID
            // 
            this.TB_FacilityID.Location = new System.Drawing.Point(169, 137);
            this.TB_FacilityID.MaxLength = 32;
            this.TB_FacilityID.Name = "TB_FacilityID";
            this.TB_FacilityID.Size = new System.Drawing.Size(277, 19);
            this.TB_FacilityID.TabIndex = 0;
            this.TB_FacilityID.Text = "CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC";
            this.TB_FacilityID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.TB_FacilityID.TextChanged += new System.EventHandler(this.TB_FacilityID_TextChanged);
            // 
            // CB_FacilityID
            // 
            this.CB_FacilityID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_FacilityID.DropDownWidth = 76;
            this.CB_FacilityID.FormattingEnabled = true;
            this.CB_FacilityID.Location = new System.Drawing.Point(370, 108);
            this.CB_FacilityID.Name = "CB_FacilityID";
            this.CB_FacilityID.Size = new System.Drawing.Size(76, 20);
            this.CB_FacilityID.TabIndex = 1;
            this.CB_FacilityID.SelectedIndexChanged += new System.EventHandler(this.CB_FacilityID_SelectedIndexChanged);
            // 
            // GB_FacilityMessage
            // 
            this.GB_FacilityMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GB_FacilityMessage.Controls.Add(this.NUD_FacilityMessage);
            this.GB_FacilityMessage.Controls.Add(this.CB_FacilityMessage);
            this.GB_FacilityMessage.Location = new System.Drawing.Point(292, 50);
            this.GB_FacilityMessage.Name = "GB_FacilityMessage";
            this.GB_FacilityMessage.Size = new System.Drawing.Size(154, 49);
            this.GB_FacilityMessage.TabIndex = 58;
            this.GB_FacilityMessage.TabStop = false;
            this.GB_FacilityMessage.Text = "visitor message";
            // 
            // NUD_FacilityMessage
            // 
            this.NUD_FacilityMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NUD_FacilityMessage.Location = new System.Drawing.Point(98, 21);
            this.NUD_FacilityMessage.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.NUD_FacilityMessage.Name = "NUD_FacilityMessage";
            this.NUD_FacilityMessage.Size = new System.Drawing.Size(47, 19);
            this.NUD_FacilityMessage.TabIndex = 1;
            this.NUD_FacilityMessage.Value = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.NUD_FacilityMessage.ValueChanged += new System.EventHandler(this.NUD_FacilityMessage_ValueChanged);
            // 
            // CB_FacilityMessage
            // 
            this.CB_FacilityMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_FacilityMessage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_FacilityMessage.DropDownWidth = 102;
            this.CB_FacilityMessage.FormattingEnabled = true;
            this.CB_FacilityMessage.Location = new System.Drawing.Point(9, 21);
            this.CB_FacilityMessage.Name = "CB_FacilityMessage";
            this.CB_FacilityMessage.Size = new System.Drawing.Size(83, 20);
            this.CB_FacilityMessage.TabIndex = 0;
            this.CB_FacilityMessage.SelectedIndexChanged += new System.EventHandler(this.CB_FacilityMessage_SelectedIndexChanged);
            // 
            // Label_OTGender
            // 
            this.Label_OTGender.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Label_OTGender.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label_OTGender.Location = new System.Drawing.Point(426, 21);
            this.Label_OTGender.Name = "Label_OTGender";
            this.Label_OTGender.Size = new System.Drawing.Size(20, 20);
            this.Label_OTGender.TabIndex = 57;
            this.Label_OTGender.Text = "G";
            this.Label_OTGender.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Label_OTGender.Click += new System.EventHandler(this.Label_OTGender_Click);
            // 
            // CHK_FacilityIntroduced
            // 
            this.CHK_FacilityIntroduced.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CHK_FacilityIntroduced.Location = new System.Drawing.Point(193, 18);
            this.CHK_FacilityIntroduced.Name = "CHK_FacilityIntroduced";
            this.CHK_FacilityIntroduced.Size = new System.Drawing.Size(128, 24);
            this.CHK_FacilityIntroduced.TabIndex = 9;
            this.CHK_FacilityIntroduced.Text = "visitor introduced";
            this.CHK_FacilityIntroduced.UseVisualStyleBackColor = true;
            this.CHK_FacilityIntroduced.CheckedChanged += new System.EventHandler(this.CHK_FacilityIntroduced_CheckedChanged);
            // 
            // L_FacilityNPC
            // 
            this.L_FacilityNPC.Location = new System.Drawing.Point(27, 103);
            this.L_FacilityNPC.Name = "L_FacilityNPC";
            this.L_FacilityNPC.Size = new System.Drawing.Size(45, 18);
            this.L_FacilityNPC.TabIndex = 8;
            this.L_FacilityNPC.Text = "NPC:";
            this.L_FacilityNPC.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_FacilityColor
            // 
            this.NUD_FacilityColor.Location = new System.Drawing.Point(78, 78);
            this.NUD_FacilityColor.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.NUD_FacilityColor.Name = "NUD_FacilityColor";
            this.NUD_FacilityColor.Size = new System.Drawing.Size(35, 19);
            this.NUD_FacilityColor.TabIndex = 7;
            this.NUD_FacilityColor.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.NUD_FacilityColor.ValueChanged += new System.EventHandler(this.NUD_FacilityColor_ValueChanged);
            // 
            // L_FacilityColor
            // 
            this.L_FacilityColor.Location = new System.Drawing.Point(25, 77);
            this.L_FacilityColor.Name = "L_FacilityColor";
            this.L_FacilityColor.Size = new System.Drawing.Size(47, 18);
            this.L_FacilityColor.TabIndex = 6;
            this.L_FacilityColor.Text = "color:";
            this.L_FacilityColor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_FacilityType
            // 
            this.L_FacilityType.Location = new System.Drawing.Point(28, 52);
            this.L_FacilityType.Name = "L_FacilityType";
            this.L_FacilityType.Size = new System.Drawing.Size(44, 18);
            this.L_FacilityType.TabIndex = 5;
            this.L_FacilityType.Text = "type:";
            this.L_FacilityType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_FacilityIndex
            // 
            this.L_FacilityIndex.Location = new System.Drawing.Point(9, 20);
            this.L_FacilityIndex.Name = "L_FacilityIndex";
            this.L_FacilityIndex.Size = new System.Drawing.Size(49, 18);
            this.L_FacilityIndex.TabIndex = 3;
            this.L_FacilityIndex.Text = "index:";
            this.L_FacilityIndex.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_FacilityNPC
            // 
            this.CB_FacilityNPC.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_FacilityNPC.DropDownWidth = 120;
            this.CB_FacilityNPC.FormattingEnabled = true;
            this.CB_FacilityNPC.Location = new System.Drawing.Point(78, 103);
            this.CB_FacilityNPC.Name = "CB_FacilityNPC";
            this.CB_FacilityNPC.Size = new System.Drawing.Size(123, 20);
            this.CB_FacilityNPC.TabIndex = 2;
            // 
            // CB_FacilityType
            // 
            this.CB_FacilityType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_FacilityType.DropDownWidth = 180;
            this.CB_FacilityType.FormattingEnabled = true;
            this.CB_FacilityType.Location = new System.Drawing.Point(78, 52);
            this.CB_FacilityType.Name = "CB_FacilityType";
            this.CB_FacilityType.Size = new System.Drawing.Size(124, 20);
            this.CB_FacilityType.TabIndex = 1;
            this.CB_FacilityType.SelectedIndexChanged += new System.EventHandler(this.CB_FacilityType_SelectedIndexChanged);
            // 
            // NUD_FacilityIndex
            // 
            this.NUD_FacilityIndex.Location = new System.Drawing.Point(64, 21);
            this.NUD_FacilityIndex.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.NUD_FacilityIndex.Name = "NUD_FacilityIndex";
            this.NUD_FacilityIndex.Size = new System.Drawing.Size(35, 19);
            this.NUD_FacilityIndex.TabIndex = 0;
            this.NUD_FacilityIndex.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.NUD_FacilityIndex.ValueChanged += new System.EventHandler(this.NUD_FacilityIndex_ValueChanged);
            // 
            // CAL_FestaStartDate
            // 
            this.CAL_FestaStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.CAL_FestaStartDate.Location = new System.Drawing.Point(9, 21);
            this.CAL_FestaStartDate.MaxDate = new System.DateTime(2050, 12, 31, 0, 0, 0, 0);
            this.CAL_FestaStartDate.MinDate = new System.DateTime(1932, 1, 1, 0, 0, 0, 0);
            this.CAL_FestaStartDate.Name = "CAL_FestaStartDate";
            this.CAL_FestaStartDate.Size = new System.Drawing.Size(99, 19);
            this.CAL_FestaStartDate.TabIndex = 36;
            this.CAL_FestaStartDate.Value = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            // 
            // CAL_FestaStartTime
            // 
            this.CAL_FestaStartTime.CustomFormat = "HH:mm:ss";
            this.CAL_FestaStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.CAL_FestaStartTime.Location = new System.Drawing.Point(9, 46);
            this.CAL_FestaStartTime.MaxDate = new System.DateTime(2050, 12, 31, 0, 0, 0, 0);
            this.CAL_FestaStartTime.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.CAL_FestaStartTime.Name = "CAL_FestaStartTime";
            this.CAL_FestaStartTime.ShowUpDown = true;
            this.CAL_FestaStartTime.Size = new System.Drawing.Size(73, 19);
            this.CAL_FestaStartTime.TabIndex = 37;
            this.CAL_FestaStartTime.Value = new System.DateTime(2001, 1, 1, 0, 0, 0, 0);
            // 
            // GB_FestaStartTime
            // 
            this.GB_FestaStartTime.Controls.Add(this.CAL_FestaStartDate);
            this.GB_FestaStartTime.Controls.Add(this.CAL_FestaStartTime);
            this.GB_FestaStartTime.Location = new System.Drawing.Point(178, 12);
            this.GB_FestaStartTime.Name = "GB_FestaStartTime";
            this.GB_FestaStartTime.Size = new System.Drawing.Size(117, 74);
            this.GB_FestaStartTime.TabIndex = 38;
            this.GB_FestaStartTime.TabStop = false;
            this.GB_FestaStartTime.Text = "Latest Start Time";
            // 
            // GB_Reward
            // 
            this.GB_Reward.Controls.Add(this.B_AllReadyReward);
            this.GB_Reward.Controls.Add(this.CLB_Reward);
            this.GB_Reward.Controls.Add(this.B_AllReceiveReward);
            this.GB_Reward.Location = new System.Drawing.Point(267, 95);
            this.GB_Reward.Name = "GB_Reward";
            this.GB_Reward.Size = new System.Drawing.Size(200, 168);
            this.GB_Reward.TabIndex = 39;
            this.GB_Reward.TabStop = false;
            this.GB_Reward.Text = "RankUP rewards";
            // 
            // B_AllReadyReward
            // 
            this.B_AllReadyReward.Location = new System.Drawing.Point(9, 50);
            this.B_AllReadyReward.Name = "B_AllReadyReward";
            this.B_AllReadyReward.Size = new System.Drawing.Size(131, 23);
            this.B_AllReadyReward.TabIndex = 2;
            this.B_AllReadyReward.Text = "All ready to receive";
            this.B_AllReadyReward.UseVisualStyleBackColor = true;
            this.B_AllReadyReward.Click += new System.EventHandler(this.B_AllReadyReward_Click);
            // 
            // CLB_Reward
            // 
            this.CLB_Reward.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CLB_Reward.CheckOnClick = true;
            this.CLB_Reward.FormattingEnabled = true;
            this.CLB_Reward.IntegralHeight = false;
            this.CLB_Reward.Location = new System.Drawing.Point(9, 82);
            this.CLB_Reward.Name = "CLB_Reward";
            this.CLB_Reward.Size = new System.Drawing.Size(182, 77);
            this.CLB_Reward.TabIndex = 1;
            this.CLB_Reward.ThreeDCheckBoxes = true;
            // 
            // B_AllReceiveReward
            // 
            this.B_AllReceiveReward.Location = new System.Drawing.Point(9, 21);
            this.B_AllReceiveReward.Name = "B_AllReceiveReward";
            this.B_AllReceiveReward.Size = new System.Drawing.Size(131, 23);
            this.B_AllReceiveReward.TabIndex = 0;
            this.B_AllReceiveReward.Text = "All received";
            this.B_AllReceiveReward.UseVisualStyleBackColor = true;
            this.B_AllReceiveReward.Click += new System.EventHandler(this.B_AllReceiveReward_Click);
            // 
            // NUD_Rank
            // 
            this.NUD_Rank.Location = new System.Drawing.Point(353, 13);
            this.NUD_Rank.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.NUD_Rank.Name = "NUD_Rank";
            this.NUD_Rank.Size = new System.Drawing.Size(47, 19);
            this.NUD_Rank.TabIndex = 40;
            this.NUD_Rank.Value = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.NUD_Rank.ValueChanged += new System.EventHandler(this.NUD_Rank_ValueChanged);
            // 
            // L_Rank
            // 
            this.L_Rank.Location = new System.Drawing.Point(301, 12);
            this.L_Rank.Name = "L_Rank";
            this.L_Rank.Size = new System.Drawing.Size(46, 18);
            this.L_Rank.TabIndex = 41;
            this.L_Rank.Text = "Rank";
            this.L_Rank.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GB_MyMessage
            // 
            this.GB_MyMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GB_MyMessage.Controls.Add(this.NUD_MyMessage);
            this.GB_MyMessage.Controls.Add(this.CB_MyMessage);
            this.GB_MyMessage.Location = new System.Drawing.Point(304, 37);
            this.GB_MyMessage.Name = "GB_MyMessage";
            this.GB_MyMessage.Size = new System.Drawing.Size(154, 49);
            this.GB_MyMessage.TabIndex = 59;
            this.GB_MyMessage.TabStop = false;
            this.GB_MyMessage.Text = "my message";
            // 
            // NUD_MyMessage
            // 
            this.NUD_MyMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NUD_MyMessage.Location = new System.Drawing.Point(98, 21);
            this.NUD_MyMessage.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.NUD_MyMessage.Name = "NUD_MyMessage";
            this.NUD_MyMessage.Size = new System.Drawing.Size(47, 19);
            this.NUD_MyMessage.TabIndex = 1;
            this.NUD_MyMessage.Value = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.NUD_MyMessage.ValueChanged += new System.EventHandler(this.NUD_MyMessage_ValueChanged);
            // 
            // CB_MyMessage
            // 
            this.CB_MyMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_MyMessage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_MyMessage.DropDownWidth = 102;
            this.CB_MyMessage.FormattingEnabled = true;
            this.CB_MyMessage.Location = new System.Drawing.Point(9, 21);
            this.CB_MyMessage.Name = "CB_MyMessage";
            this.CB_MyMessage.Size = new System.Drawing.Size(83, 20);
            this.CB_MyMessage.TabIndex = 0;
            this.CB_MyMessage.SelectedIndexChanged += new System.EventHandler(this.CB_MyMessage_SelectedIndexChanged);
            // 
            // L_Note
            // 
            this.L_Note.Location = new System.Drawing.Point(12, 437);
            this.L_Note.Name = "L_Note";
            this.L_Note.Size = new System.Drawing.Size(299, 33);
            this.L_Note.TabIndex = 60;
            this.L_Note.Text = "note: If facility is NOT introduced by visitor,\r\n this form delete all of visitor" +
    "s data of the facility.";
            this.L_Note.Visible = false;
            // 
            // SAV_FestivalPlaza
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(485, 479);
            this.Controls.Add(this.L_Note);
            this.Controls.Add(this.GB_MyMessage);
            this.Controls.Add(this.L_Rank);
            this.Controls.Add(this.NUD_Rank);
            this.Controls.Add(this.GB_Reward);
            this.Controls.Add(this.GB_FestaStartTime);
            this.Controls.Add(this.GB_Facility);
            this.Controls.Add(this.GB_Phrase);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_Cancel);
            this.Controls.Add(this.GB_FC);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SAV_FestivalPlaza";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Festival Plaza Editor";
            ((System.ComponentModel.ISupportInitialize)(this.NUD_FC_Current)).EndInit();
            this.GB_FC.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NUD_FC_Used)).EndInit();
            this.GB_Phrase.ResumeLayout(false);
            this.GB_Facility.ResumeLayout(false);
            this.GB_Facility.PerformLayout();
            this.GB_FacilityMessage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NUD_FacilityMessage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_FacilityColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_FacilityIndex)).EndInit();
            this.GB_FestaStartTime.ResumeLayout(false);
            this.GB_Reward.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Rank)).EndInit();
            this.GB_MyMessage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NUD_MyMessage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown NUD_FC_Current;
        private System.Windows.Forms.Label L_FC_Current;
        private System.Windows.Forms.GroupBox GB_FC;
        private System.Windows.Forms.NumericUpDown NUD_FC_Used;
        private System.Windows.Forms.Label L_FC_Used;
        private System.Windows.Forms.Label L_FC_CollectedV;
        private System.Windows.Forms.Label L_FC_CollectedL;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.GroupBox GB_Phrase;
        private System.Windows.Forms.Button B_AllPhrases;
        private System.Windows.Forms.CheckedListBox CLB_Phrases;
        private System.Windows.Forms.TextBox TB_OTName;
        private System.Windows.Forms.GroupBox GB_Facility;
        private System.Windows.Forms.DateTimePicker CAL_FestaStartDate;
        private System.Windows.Forms.DateTimePicker CAL_FestaStartTime;
        private System.Windows.Forms.GroupBox GB_FestaStartTime;
        private System.Windows.Forms.GroupBox GB_Reward;
        private System.Windows.Forms.CheckedListBox CLB_Reward;
        private System.Windows.Forms.Button B_AllReceiveReward;
        private System.Windows.Forms.Button B_AllReadyReward;
        private System.Windows.Forms.NumericUpDown NUD_FacilityIndex;
        private System.Windows.Forms.CheckBox CHK_FacilityIntroduced;
        private System.Windows.Forms.Label L_FacilityNPC;
        private System.Windows.Forms.NumericUpDown NUD_FacilityColor;
        private System.Windows.Forms.Label L_FacilityColor;
        private System.Windows.Forms.Label L_FacilityType;
        private System.Windows.Forms.Label L_FacilityIndex;
        private System.Windows.Forms.ComboBox CB_FacilityNPC;
        private System.Windows.Forms.ComboBox CB_FacilityType;
        private System.Windows.Forms.Label Label_OTGender;
        private System.Windows.Forms.GroupBox GB_FacilityMessage;
        private System.Windows.Forms.ComboBox CB_FacilityMessage;
        private System.Windows.Forms.NumericUpDown NUD_FacilityMessage;
        private System.Windows.Forms.ComboBox CB_FacilityID;
        private System.Windows.Forms.TextBox TB_FacilityID;
        private System.Windows.Forms.NumericUpDown NUD_Rank;
        private System.Windows.Forms.Label L_Rank;
        private System.Windows.Forms.Label L_VisitorMisc;
        private System.Windows.Forms.GroupBox GB_MyMessage;
        private System.Windows.Forms.NumericUpDown NUD_MyMessage;
        private System.Windows.Forms.ComboBox CB_MyMessage;
        private System.Windows.Forms.Label L_FacilityColorV;
        private System.Windows.Forms.Label L_Note;
    }
}