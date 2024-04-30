namespace PKHeX.WinForms
{
    partial class MemoryAmie
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
            BTN_Save = new System.Windows.Forms.Button();
            BTN_Cancel = new System.Windows.Forms.Button();
            M_OT_Friendship = new System.Windows.Forms.MaskedTextBox();
            L_OT_Friendship = new System.Windows.Forms.Label();
            L_Geo0 = new System.Windows.Forms.Label();
            L_Geo1 = new System.Windows.Forms.Label();
            L_Region = new System.Windows.Forms.Label();
            L_Country = new System.Windows.Forms.Label();
            L_Geo2 = new System.Windows.Forms.Label();
            L_Geo3 = new System.Windows.Forms.Label();
            L_Geo4 = new System.Windows.Forms.Label();
            L_OT_Quality = new System.Windows.Forms.Label();
            L_OT_TextLine = new System.Windows.Forms.Label();
            LOTV = new System.Windows.Forms.Label();
            L_OT_Feeling = new System.Windows.Forms.Label();
            GB_M_OT = new System.Windows.Forms.GroupBox();
            RTB_OT = new System.Windows.Forms.RichTextBox();
            CB_OTVar = new System.Windows.Forms.ComboBox();
            CB_OTMemory = new System.Windows.Forms.ComboBox();
            CB_OTQual = new System.Windows.Forms.ComboBox();
            CB_OTFeel = new System.Windows.Forms.ComboBox();
            L_OT_Affection = new System.Windows.Forms.Label();
            M_OT_Affection = new System.Windows.Forms.MaskedTextBox();
            GB_Residence = new System.Windows.Forms.GroupBox();
            B_ClearAll = new System.Windows.Forms.Button();
            CB_Region4 = new System.Windows.Forms.ComboBox();
            CB_Region3 = new System.Windows.Forms.ComboBox();
            CB_Region2 = new System.Windows.Forms.ComboBox();
            CB_Region1 = new System.Windows.Forms.ComboBox();
            CB_Region0 = new System.Windows.Forms.ComboBox();
            CB_Country4 = new System.Windows.Forms.ComboBox();
            CB_Country3 = new System.Windows.Forms.ComboBox();
            CB_Country2 = new System.Windows.Forms.ComboBox();
            CB_Country1 = new System.Windows.Forms.ComboBox();
            CB_Country0 = new System.Windows.Forms.ComboBox();
            L_Enjoyment = new System.Windows.Forms.Label();
            L_Fullness = new System.Windows.Forms.Label();
            M_Enjoyment = new System.Windows.Forms.MaskedTextBox();
            M_Fullness = new System.Windows.Forms.MaskedTextBox();
            tabControl1 = new System.Windows.Forms.TabControl();
            Tab_OTMemory = new System.Windows.Forms.TabPage();
            Tab_CTMemory = new System.Windows.Forms.TabPage();
            GB_M_CT = new System.Windows.Forms.GroupBox();
            RTB_CT = new System.Windows.Forms.RichTextBox();
            CB_CTVar = new System.Windows.Forms.ComboBox();
            CB_CTMemory = new System.Windows.Forms.ComboBox();
            CB_CTQual = new System.Windows.Forms.ComboBox();
            CB_CTFeel = new System.Windows.Forms.ComboBox();
            L_CT_Affection = new System.Windows.Forms.Label();
            L_CT_Friendship = new System.Windows.Forms.Label();
            M_CT_Affection = new System.Windows.Forms.MaskedTextBox();
            M_CT_Friendship = new System.Windows.Forms.MaskedTextBox();
            LCTV = new System.Windows.Forms.Label();
            L_CT_Feeling = new System.Windows.Forms.Label();
            L_CT_TextLine = new System.Windows.Forms.Label();
            L_CT_Quality = new System.Windows.Forms.Label();
            Tab_Residence = new System.Windows.Forms.TabPage();
            Tab_Other = new System.Windows.Forms.TabPage();
            MT_Sociability = new System.Windows.Forms.MaskedTextBox();
            L_Sociability = new System.Windows.Forms.Label();
            L_Handler = new System.Windows.Forms.Label();
            CB_Handler = new System.Windows.Forms.ComboBox();
            L_Arguments = new System.Windows.Forms.Label();
            GB_M_OT.SuspendLayout();
            GB_Residence.SuspendLayout();
            tabControl1.SuspendLayout();
            Tab_OTMemory.SuspendLayout();
            Tab_CTMemory.SuspendLayout();
            GB_M_CT.SuspendLayout();
            Tab_Residence.SuspendLayout();
            Tab_Other.SuspendLayout();
            SuspendLayout();
            // 
            // BTN_Save
            // 
            BTN_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BTN_Save.Location = new System.Drawing.Point(334, 279);
            BTN_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BTN_Save.Name = "BTN_Save";
            BTN_Save.Size = new System.Drawing.Size(89, 27);
            BTN_Save.TabIndex = 30;
            BTN_Save.Text = "Save";
            BTN_Save.UseVisualStyleBackColor = true;
            BTN_Save.Click += B_Save_Click;
            // 
            // BTN_Cancel
            // 
            BTN_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BTN_Cancel.Location = new System.Drawing.Point(238, 279);
            BTN_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BTN_Cancel.Name = "BTN_Cancel";
            BTN_Cancel.Size = new System.Drawing.Size(89, 27);
            BTN_Cancel.TabIndex = 29;
            BTN_Cancel.Text = "Cancel";
            BTN_Cancel.UseVisualStyleBackColor = true;
            BTN_Cancel.Click += B_Cancel_Click;
            // 
            // M_OT_Friendship
            // 
            M_OT_Friendship.Location = new System.Drawing.Point(100, 18);
            M_OT_Friendship.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            M_OT_Friendship.Mask = "000";
            M_OT_Friendship.Name = "M_OT_Friendship";
            M_OT_Friendship.Size = new System.Drawing.Size(27, 23);
            M_OT_Friendship.TabIndex = 2;
            M_OT_Friendship.TextChanged += Update255_MTB;
            // 
            // L_OT_Friendship
            // 
            L_OT_Friendship.AutoSize = true;
            L_OT_Friendship.Location = new System.Drawing.Point(7, 22);
            L_OT_Friendship.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_OT_Friendship.Name = "L_OT_Friendship";
            L_OT_Friendship.Size = new System.Drawing.Size(65, 15);
            L_OT_Friendship.TabIndex = 52;
            L_OT_Friendship.Text = "Friendship:";
            // 
            // L_Geo0
            // 
            L_Geo0.Location = new System.Drawing.Point(-1, 36);
            L_Geo0.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Geo0.Name = "L_Geo0";
            L_Geo0.Size = new System.Drawing.Size(93, 15);
            L_Geo0.TabIndex = 68;
            L_Geo0.Text = "Latest:";
            L_Geo0.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            L_Geo0.Click += ClickResetLocation;
            // 
            // L_Geo1
            // 
            L_Geo1.Location = new System.Drawing.Point(0, 66);
            L_Geo1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Geo1.Name = "L_Geo1";
            L_Geo1.Size = new System.Drawing.Size(93, 15);
            L_Geo1.TabIndex = 69;
            L_Geo1.Text = "Past 1:";
            L_Geo1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            L_Geo1.Click += ClickResetLocation;
            // 
            // L_Region
            // 
            L_Region.Location = new System.Drawing.Point(264, 14);
            L_Region.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Region.Name = "L_Region";
            L_Region.Size = new System.Drawing.Size(93, 15);
            L_Region.TabIndex = 73;
            L_Region.Text = "Region";
            L_Region.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Country
            // 
            L_Country.Location = new System.Drawing.Point(121, 14);
            L_Country.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Country.Name = "L_Country";
            L_Country.Size = new System.Drawing.Size(93, 15);
            L_Country.TabIndex = 74;
            L_Country.Text = "Country";
            L_Country.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Geo2
            // 
            L_Geo2.Location = new System.Drawing.Point(0, 96);
            L_Geo2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Geo2.Name = "L_Geo2";
            L_Geo2.Size = new System.Drawing.Size(93, 15);
            L_Geo2.TabIndex = 76;
            L_Geo2.Text = "Past 2:";
            L_Geo2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            L_Geo2.Click += ClickResetLocation;
            // 
            // L_Geo3
            // 
            L_Geo3.Location = new System.Drawing.Point(0, 126);
            L_Geo3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Geo3.Name = "L_Geo3";
            L_Geo3.Size = new System.Drawing.Size(93, 15);
            L_Geo3.TabIndex = 77;
            L_Geo3.Text = "Past 3:";
            L_Geo3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            L_Geo3.Click += ClickResetLocation;
            // 
            // L_Geo4
            // 
            L_Geo4.Location = new System.Drawing.Point(-1, 155);
            L_Geo4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Geo4.Name = "L_Geo4";
            L_Geo4.Size = new System.Drawing.Size(93, 15);
            L_Geo4.TabIndex = 78;
            L_Geo4.Text = "Past 4:";
            L_Geo4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            L_Geo4.Click += ClickResetLocation;
            // 
            // L_OT_Quality
            // 
            L_OT_Quality.AutoSize = true;
            L_OT_Quality.Location = new System.Drawing.Point(7, 105);
            L_OT_Quality.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_OT_Quality.Name = "L_OT_Quality";
            L_OT_Quality.Size = new System.Drawing.Size(55, 15);
            L_OT_Quality.TabIndex = 80;
            L_OT_Quality.Text = "Intensity:";
            // 
            // L_OT_TextLine
            // 
            L_OT_TextLine.AutoSize = true;
            L_OT_TextLine.Location = new System.Drawing.Point(7, 47);
            L_OT_TextLine.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_OT_TextLine.Name = "L_OT_TextLine";
            L_OT_TextLine.Size = new System.Drawing.Size(82, 15);
            L_OT_TextLine.TabIndex = 82;
            L_OT_TextLine.Text = "Memory Type:";
            // 
            // LOTV
            // 
            LOTV.AutoSize = true;
            LOTV.Location = new System.Drawing.Point(7, 75);
            LOTV.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LOTV.Name = "LOTV";
            LOTV.Size = new System.Drawing.Size(61, 15);
            LOTV.TabIndex = 83;
            LOTV.Text = "VARIABLE:";
            // 
            // L_OT_Feeling
            // 
            L_OT_Feeling.AutoSize = true;
            L_OT_Feeling.Location = new System.Drawing.Point(7, 132);
            L_OT_Feeling.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_OT_Feeling.Name = "L_OT_Feeling";
            L_OT_Feeling.Size = new System.Drawing.Size(48, 15);
            L_OT_Feeling.TabIndex = 86;
            L_OT_Feeling.Text = "Feeling:";
            // 
            // GB_M_OT
            // 
            GB_M_OT.Controls.Add(RTB_OT);
            GB_M_OT.Controls.Add(CB_OTVar);
            GB_M_OT.Controls.Add(CB_OTMemory);
            GB_M_OT.Controls.Add(CB_OTQual);
            GB_M_OT.Controls.Add(CB_OTFeel);
            GB_M_OT.Controls.Add(L_OT_Affection);
            GB_M_OT.Controls.Add(M_OT_Affection);
            GB_M_OT.Controls.Add(L_OT_Feeling);
            GB_M_OT.Controls.Add(LOTV);
            GB_M_OT.Controls.Add(L_OT_TextLine);
            GB_M_OT.Controls.Add(M_OT_Friendship);
            GB_M_OT.Controls.Add(L_OT_Friendship);
            GB_M_OT.Controls.Add(L_OT_Quality);
            GB_M_OT.Location = new System.Drawing.Point(8, 8);
            GB_M_OT.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_M_OT.Name = "GB_M_OT";
            GB_M_OT.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_M_OT.Size = new System.Drawing.Size(387, 217);
            GB_M_OT.TabIndex = 1;
            GB_M_OT.TabStop = false;
            GB_M_OT.Text = "Memories with Original Trainer";
            // 
            // RTB_OT
            // 
            RTB_OT.Location = new System.Drawing.Point(0, 163);
            RTB_OT.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RTB_OT.Name = "RTB_OT";
            RTB_OT.ReadOnly = true;
            RTB_OT.Size = new System.Drawing.Size(387, 54);
            RTB_OT.TabIndex = 8;
            RTB_OT.Text = "";
            // 
            // CB_OTVar
            // 
            CB_OTVar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_OTVar.FormattingEnabled = true;
            CB_OTVar.Location = new System.Drawing.Point(100, 70);
            CB_OTVar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_OTVar.Name = "CB_OTVar";
            CB_OTVar.Size = new System.Drawing.Size(198, 23);
            CB_OTVar.TabIndex = 5;
            CB_OTVar.SelectedIndexChanged += ChangeMemory;
            // 
            // CB_OTMemory
            // 
            CB_OTMemory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_OTMemory.DropDownWidth = 440;
            CB_OTMemory.FormattingEnabled = true;
            CB_OTMemory.Location = new System.Drawing.Point(100, 44);
            CB_OTMemory.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_OTMemory.Name = "CB_OTMemory";
            CB_OTMemory.Size = new System.Drawing.Size(279, 23);
            CB_OTMemory.TabIndex = 4;
            CB_OTMemory.SelectedIndexChanged += ChangeMemory;
            // 
            // CB_OTQual
            // 
            CB_OTQual.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_OTQual.FormattingEnabled = true;
            CB_OTQual.Location = new System.Drawing.Point(100, 102);
            CB_OTQual.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_OTQual.Name = "CB_OTQual";
            CB_OTQual.Size = new System.Drawing.Size(279, 23);
            CB_OTQual.TabIndex = 6;
            CB_OTQual.SelectedIndexChanged += ChangeMemory;
            // 
            // CB_OTFeel
            // 
            CB_OTFeel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_OTFeel.FormattingEnabled = true;
            CB_OTFeel.Location = new System.Drawing.Point(100, 128);
            CB_OTFeel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_OTFeel.Name = "CB_OTFeel";
            CB_OTFeel.Size = new System.Drawing.Size(198, 23);
            CB_OTFeel.TabIndex = 7;
            CB_OTFeel.SelectedIndexChanged += ChangeMemory;
            // 
            // L_OT_Affection
            // 
            L_OT_Affection.AutoSize = true;
            L_OT_Affection.Location = new System.Drawing.Point(174, 22);
            L_OT_Affection.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_OT_Affection.Name = "L_OT_Affection";
            L_OT_Affection.Size = new System.Drawing.Size(59, 15);
            L_OT_Affection.TabIndex = 88;
            L_OT_Affection.Text = "Affection:";
            // 
            // M_OT_Affection
            // 
            M_OT_Affection.Location = new System.Drawing.Point(265, 18);
            M_OT_Affection.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            M_OT_Affection.Mask = "000";
            M_OT_Affection.Name = "M_OT_Affection";
            M_OT_Affection.Size = new System.Drawing.Size(27, 23);
            M_OT_Affection.TabIndex = 3;
            M_OT_Affection.TextChanged += Update255_MTB;
            // 
            // GB_Residence
            // 
            GB_Residence.Controls.Add(B_ClearAll);
            GB_Residence.Controls.Add(CB_Region4);
            GB_Residence.Controls.Add(CB_Region3);
            GB_Residence.Controls.Add(CB_Region2);
            GB_Residence.Controls.Add(CB_Region1);
            GB_Residence.Controls.Add(CB_Region0);
            GB_Residence.Controls.Add(CB_Country4);
            GB_Residence.Controls.Add(CB_Country3);
            GB_Residence.Controls.Add(CB_Country2);
            GB_Residence.Controls.Add(CB_Country1);
            GB_Residence.Controls.Add(CB_Country0);
            GB_Residence.Controls.Add(L_Geo4);
            GB_Residence.Controls.Add(L_Geo3);
            GB_Residence.Controls.Add(L_Geo2);
            GB_Residence.Controls.Add(L_Country);
            GB_Residence.Controls.Add(L_Region);
            GB_Residence.Controls.Add(L_Geo1);
            GB_Residence.Controls.Add(L_Geo0);
            GB_Residence.Location = new System.Drawing.Point(8, 8);
            GB_Residence.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Residence.Name = "GB_Residence";
            GB_Residence.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Residence.Size = new System.Drawing.Size(387, 217);
            GB_Residence.TabIndex = 89;
            GB_Residence.TabStop = false;
            GB_Residence.Text = "Pokémon has Resided in:";
            // 
            // B_ClearAll
            // 
            B_ClearAll.Location = new System.Drawing.Point(96, 181);
            B_ClearAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ClearAll.Name = "B_ClearAll";
            B_ClearAll.Size = new System.Drawing.Size(89, 27);
            B_ClearAll.TabIndex = 79;
            B_ClearAll.Text = "Clear All";
            B_ClearAll.UseVisualStyleBackColor = true;
            B_ClearAll.Click += B_ClearAll_Click;
            // 
            // CB_Region4
            // 
            CB_Region4.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            CB_Region4.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Region4.DropDownWidth = 180;
            CB_Region4.FormattingEnabled = true;
            CB_Region4.Location = new System.Drawing.Point(238, 150);
            CB_Region4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Region4.Name = "CB_Region4";
            CB_Region4.Size = new System.Drawing.Size(118, 23);
            CB_Region4.TabIndex = 25;
            // 
            // CB_Region3
            // 
            CB_Region3.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            CB_Region3.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Region3.DropDownWidth = 180;
            CB_Region3.FormattingEnabled = true;
            CB_Region3.Location = new System.Drawing.Point(238, 121);
            CB_Region3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Region3.Name = "CB_Region3";
            CB_Region3.Size = new System.Drawing.Size(118, 23);
            CB_Region3.TabIndex = 23;
            // 
            // CB_Region2
            // 
            CB_Region2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            CB_Region2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Region2.DropDownWidth = 180;
            CB_Region2.FormattingEnabled = true;
            CB_Region2.Location = new System.Drawing.Point(238, 91);
            CB_Region2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Region2.Name = "CB_Region2";
            CB_Region2.Size = new System.Drawing.Size(118, 23);
            CB_Region2.TabIndex = 21;
            // 
            // CB_Region1
            // 
            CB_Region1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            CB_Region1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Region1.DropDownWidth = 180;
            CB_Region1.FormattingEnabled = true;
            CB_Region1.Location = new System.Drawing.Point(238, 61);
            CB_Region1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Region1.Name = "CB_Region1";
            CB_Region1.Size = new System.Drawing.Size(118, 23);
            CB_Region1.TabIndex = 19;
            // 
            // CB_Region0
            // 
            CB_Region0.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            CB_Region0.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Region0.DropDownWidth = 180;
            CB_Region0.FormattingEnabled = true;
            CB_Region0.Location = new System.Drawing.Point(238, 31);
            CB_Region0.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Region0.Name = "CB_Region0";
            CB_Region0.Size = new System.Drawing.Size(118, 23);
            CB_Region0.TabIndex = 17;
            // 
            // CB_Country4
            // 
            CB_Country4.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            CB_Country4.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Country4.DropDownWidth = 180;
            CB_Country4.FormattingEnabled = true;
            CB_Country4.Location = new System.Drawing.Point(96, 150);
            CB_Country4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Country4.Name = "CB_Country4";
            CB_Country4.Size = new System.Drawing.Size(118, 23);
            CB_Country4.TabIndex = 24;
            CB_Country4.SelectedIndexChanged += ChangeCountryIndex;
            CB_Country4.TextChanged += ChangeCountryText;
            // 
            // CB_Country3
            // 
            CB_Country3.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            CB_Country3.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Country3.DropDownWidth = 180;
            CB_Country3.FormattingEnabled = true;
            CB_Country3.Location = new System.Drawing.Point(96, 121);
            CB_Country3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Country3.Name = "CB_Country3";
            CB_Country3.Size = new System.Drawing.Size(118, 23);
            CB_Country3.TabIndex = 22;
            CB_Country3.SelectedIndexChanged += ChangeCountryIndex;
            CB_Country3.TextChanged += ChangeCountryText;
            // 
            // CB_Country2
            // 
            CB_Country2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            CB_Country2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Country2.DropDownWidth = 180;
            CB_Country2.FormattingEnabled = true;
            CB_Country2.Location = new System.Drawing.Point(96, 91);
            CB_Country2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Country2.Name = "CB_Country2";
            CB_Country2.Size = new System.Drawing.Size(118, 23);
            CB_Country2.TabIndex = 20;
            CB_Country2.SelectedIndexChanged += ChangeCountryIndex;
            CB_Country2.TextChanged += ChangeCountryText;
            // 
            // CB_Country1
            // 
            CB_Country1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            CB_Country1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Country1.DropDownWidth = 180;
            CB_Country1.FormattingEnabled = true;
            CB_Country1.Location = new System.Drawing.Point(96, 61);
            CB_Country1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Country1.Name = "CB_Country1";
            CB_Country1.Size = new System.Drawing.Size(118, 23);
            CB_Country1.TabIndex = 18;
            CB_Country1.SelectedIndexChanged += ChangeCountryIndex;
            CB_Country1.TextChanged += ChangeCountryText;
            // 
            // CB_Country0
            // 
            CB_Country0.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            CB_Country0.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Country0.DropDownWidth = 180;
            CB_Country0.FormattingEnabled = true;
            CB_Country0.Location = new System.Drawing.Point(96, 31);
            CB_Country0.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Country0.Name = "CB_Country0";
            CB_Country0.Size = new System.Drawing.Size(118, 23);
            CB_Country0.TabIndex = 16;
            CB_Country0.SelectedIndexChanged += ChangeCountryIndex;
            CB_Country0.TextChanged += ChangeCountryText;
            // 
            // L_Enjoyment
            // 
            L_Enjoyment.Location = new System.Drawing.Point(50, 95);
            L_Enjoyment.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Enjoyment.Name = "L_Enjoyment";
            L_Enjoyment.Size = new System.Drawing.Size(140, 15);
            L_Enjoyment.TabIndex = 99;
            L_Enjoyment.Text = "Enjoyment:";
            L_Enjoyment.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Fullness
            // 
            L_Fullness.Location = new System.Drawing.Point(50, 65);
            L_Fullness.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Fullness.Name = "L_Fullness";
            L_Fullness.Size = new System.Drawing.Size(140, 15);
            L_Fullness.TabIndex = 98;
            L_Fullness.Text = "Fullness:";
            L_Fullness.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // M_Enjoyment
            // 
            M_Enjoyment.Location = new System.Drawing.Point(203, 91);
            M_Enjoyment.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            M_Enjoyment.Mask = "000";
            M_Enjoyment.Name = "M_Enjoyment";
            M_Enjoyment.Size = new System.Drawing.Size(27, 23);
            M_Enjoyment.TabIndex = 28;
            M_Enjoyment.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            M_Enjoyment.TextChanged += Update255_MTB;
            // 
            // M_Fullness
            // 
            M_Fullness.Location = new System.Drawing.Point(203, 61);
            M_Fullness.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            M_Fullness.Mask = "000";
            M_Fullness.Name = "M_Fullness";
            M_Fullness.Size = new System.Drawing.Size(27, 23);
            M_Fullness.TabIndex = 27;
            M_Fullness.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            M_Fullness.TextChanged += Update255_MTB;
            // 
            // tabControl1
            // 
            tabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tabControl1.Controls.Add(Tab_OTMemory);
            tabControl1.Controls.Add(Tab_CTMemory);
            tabControl1.Controls.Add(Tab_Residence);
            tabControl1.Controls.Add(Tab_Other);
            tabControl1.Location = new System.Drawing.Point(0, 0);
            tabControl1.Margin = new System.Windows.Forms.Padding(0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(439, 272);
            tabControl1.TabIndex = 0;
            // 
            // Tab_OTMemory
            // 
            Tab_OTMemory.Controls.Add(GB_M_OT);
            Tab_OTMemory.Location = new System.Drawing.Point(4, 24);
            Tab_OTMemory.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_OTMemory.Name = "Tab_OTMemory";
            Tab_OTMemory.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_OTMemory.Size = new System.Drawing.Size(431, 244);
            Tab_OTMemory.TabIndex = 1;
            Tab_OTMemory.Text = "Memories with OT";
            Tab_OTMemory.UseVisualStyleBackColor = true;
            // 
            // Tab_CTMemory
            // 
            Tab_CTMemory.Controls.Add(GB_M_CT);
            Tab_CTMemory.Location = new System.Drawing.Point(4, 24);
            Tab_CTMemory.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_CTMemory.Name = "Tab_CTMemory";
            Tab_CTMemory.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_CTMemory.Size = new System.Drawing.Size(431, 244);
            Tab_CTMemory.TabIndex = 2;
            Tab_CTMemory.Text = "Memories with notOT";
            Tab_CTMemory.UseVisualStyleBackColor = true;
            // 
            // GB_M_CT
            // 
            GB_M_CT.Controls.Add(RTB_CT);
            GB_M_CT.Controls.Add(CB_CTVar);
            GB_M_CT.Controls.Add(CB_CTMemory);
            GB_M_CT.Controls.Add(CB_CTQual);
            GB_M_CT.Controls.Add(CB_CTFeel);
            GB_M_CT.Controls.Add(L_CT_Affection);
            GB_M_CT.Controls.Add(L_CT_Friendship);
            GB_M_CT.Controls.Add(M_CT_Affection);
            GB_M_CT.Controls.Add(M_CT_Friendship);
            GB_M_CT.Controls.Add(LCTV);
            GB_M_CT.Controls.Add(L_CT_Feeling);
            GB_M_CT.Controls.Add(L_CT_TextLine);
            GB_M_CT.Controls.Add(L_CT_Quality);
            GB_M_CT.Location = new System.Drawing.Point(8, 8);
            GB_M_CT.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_M_CT.Name = "GB_M_CT";
            GB_M_CT.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_M_CT.Size = new System.Drawing.Size(387, 217);
            GB_M_CT.TabIndex = 89;
            GB_M_CT.TabStop = false;
            GB_M_CT.Text = "Memories with Current Trainer";
            // 
            // RTB_CT
            // 
            RTB_CT.Location = new System.Drawing.Point(0, 163);
            RTB_CT.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RTB_CT.Name = "RTB_CT";
            RTB_CT.ReadOnly = true;
            RTB_CT.Size = new System.Drawing.Size(387, 54);
            RTB_CT.TabIndex = 15;
            RTB_CT.Text = "";
            // 
            // CB_CTVar
            // 
            CB_CTVar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_CTVar.FormattingEnabled = true;
            CB_CTVar.Location = new System.Drawing.Point(100, 70);
            CB_CTVar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_CTVar.Name = "CB_CTVar";
            CB_CTVar.Size = new System.Drawing.Size(198, 23);
            CB_CTVar.TabIndex = 12;
            CB_CTVar.SelectedIndexChanged += ChangeMemory;
            // 
            // CB_CTMemory
            // 
            CB_CTMemory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_CTMemory.DropDownWidth = 440;
            CB_CTMemory.FormattingEnabled = true;
            CB_CTMemory.Location = new System.Drawing.Point(100, 44);
            CB_CTMemory.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_CTMemory.Name = "CB_CTMemory";
            CB_CTMemory.Size = new System.Drawing.Size(279, 23);
            CB_CTMemory.TabIndex = 11;
            CB_CTMemory.SelectedIndexChanged += ChangeMemory;
            // 
            // CB_CTQual
            // 
            CB_CTQual.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_CTQual.FormattingEnabled = true;
            CB_CTQual.Location = new System.Drawing.Point(100, 102);
            CB_CTQual.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_CTQual.Name = "CB_CTQual";
            CB_CTQual.Size = new System.Drawing.Size(279, 23);
            CB_CTQual.TabIndex = 13;
            CB_CTQual.SelectedIndexChanged += ChangeMemory;
            // 
            // CB_CTFeel
            // 
            CB_CTFeel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_CTFeel.FormattingEnabled = true;
            CB_CTFeel.Location = new System.Drawing.Point(100, 128);
            CB_CTFeel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_CTFeel.Name = "CB_CTFeel";
            CB_CTFeel.Size = new System.Drawing.Size(198, 23);
            CB_CTFeel.TabIndex = 14;
            CB_CTFeel.SelectedIndexChanged += ChangeMemory;
            // 
            // L_CT_Affection
            // 
            L_CT_Affection.AutoSize = true;
            L_CT_Affection.Location = new System.Drawing.Point(174, 22);
            L_CT_Affection.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_CT_Affection.Name = "L_CT_Affection";
            L_CT_Affection.Size = new System.Drawing.Size(59, 15);
            L_CT_Affection.TabIndex = 91;
            L_CT_Affection.Text = "Affection:";
            // 
            // L_CT_Friendship
            // 
            L_CT_Friendship.AutoSize = true;
            L_CT_Friendship.Location = new System.Drawing.Point(7, 22);
            L_CT_Friendship.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_CT_Friendship.Name = "L_CT_Friendship";
            L_CT_Friendship.Size = new System.Drawing.Size(65, 15);
            L_CT_Friendship.TabIndex = 90;
            L_CT_Friendship.Text = "Friendship:";
            // 
            // M_CT_Affection
            // 
            M_CT_Affection.Location = new System.Drawing.Point(265, 18);
            M_CT_Affection.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            M_CT_Affection.Mask = "000";
            M_CT_Affection.Name = "M_CT_Affection";
            M_CT_Affection.Size = new System.Drawing.Size(27, 23);
            M_CT_Affection.TabIndex = 10;
            M_CT_Affection.TextChanged += Update255_MTB;
            // 
            // M_CT_Friendship
            // 
            M_CT_Friendship.Location = new System.Drawing.Point(100, 18);
            M_CT_Friendship.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            M_CT_Friendship.Mask = "000";
            M_CT_Friendship.Name = "M_CT_Friendship";
            M_CT_Friendship.Size = new System.Drawing.Size(27, 23);
            M_CT_Friendship.TabIndex = 9;
            M_CT_Friendship.TextChanged += Update255_MTB;
            // 
            // LCTV
            // 
            LCTV.AutoSize = true;
            LCTV.Location = new System.Drawing.Point(7, 75);
            LCTV.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LCTV.Name = "LCTV";
            LCTV.Size = new System.Drawing.Size(58, 15);
            LCTV.TabIndex = 58;
            LCTV.Text = "VARIABLE";
            // 
            // L_CT_Feeling
            // 
            L_CT_Feeling.AutoSize = true;
            L_CT_Feeling.Location = new System.Drawing.Point(7, 132);
            L_CT_Feeling.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_CT_Feeling.Name = "L_CT_Feeling";
            L_CT_Feeling.Size = new System.Drawing.Size(48, 15);
            L_CT_Feeling.TabIndex = 56;
            L_CT_Feeling.Text = "Feeling:";
            // 
            // L_CT_TextLine
            // 
            L_CT_TextLine.AutoSize = true;
            L_CT_TextLine.Location = new System.Drawing.Point(7, 47);
            L_CT_TextLine.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_CT_TextLine.Name = "L_CT_TextLine";
            L_CT_TextLine.Size = new System.Drawing.Size(82, 15);
            L_CT_TextLine.TabIndex = 55;
            L_CT_TextLine.Text = "Memory Type:";
            // 
            // L_CT_Quality
            // 
            L_CT_Quality.AutoSize = true;
            L_CT_Quality.Location = new System.Drawing.Point(7, 105);
            L_CT_Quality.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_CT_Quality.Name = "L_CT_Quality";
            L_CT_Quality.Size = new System.Drawing.Size(55, 15);
            L_CT_Quality.TabIndex = 54;
            L_CT_Quality.Text = "Intensity:";
            // 
            // Tab_Residence
            // 
            Tab_Residence.Controls.Add(GB_Residence);
            Tab_Residence.Location = new System.Drawing.Point(4, 24);
            Tab_Residence.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_Residence.Name = "Tab_Residence";
            Tab_Residence.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_Residence.Size = new System.Drawing.Size(431, 244);
            Tab_Residence.TabIndex = 0;
            Tab_Residence.Text = "Residence";
            Tab_Residence.UseVisualStyleBackColor = true;
            // 
            // Tab_Other
            // 
            Tab_Other.Controls.Add(MT_Sociability);
            Tab_Other.Controls.Add(L_Sociability);
            Tab_Other.Controls.Add(M_Enjoyment);
            Tab_Other.Controls.Add(L_Handler);
            Tab_Other.Controls.Add(M_Fullness);
            Tab_Other.Controls.Add(L_Fullness);
            Tab_Other.Controls.Add(CB_Handler);
            Tab_Other.Controls.Add(L_Enjoyment);
            Tab_Other.Location = new System.Drawing.Point(4, 24);
            Tab_Other.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_Other.Name = "Tab_Other";
            Tab_Other.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_Other.Size = new System.Drawing.Size(431, 244);
            Tab_Other.TabIndex = 3;
            Tab_Other.Text = "Other";
            Tab_Other.UseVisualStyleBackColor = true;
            // 
            // MT_Sociability
            // 
            MT_Sociability.Location = new System.Drawing.Point(203, 121);
            MT_Sociability.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MT_Sociability.Mask = "000";
            MT_Sociability.Name = "MT_Sociability";
            MT_Sociability.Size = new System.Drawing.Size(27, 23);
            MT_Sociability.TabIndex = 102;
            MT_Sociability.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // L_Sociability
            // 
            L_Sociability.Location = new System.Drawing.Point(50, 125);
            L_Sociability.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Sociability.Name = "L_Sociability";
            L_Sociability.Size = new System.Drawing.Size(140, 15);
            L_Sociability.TabIndex = 103;
            L_Sociability.Text = "Sociability:";
            L_Sociability.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Handler
            // 
            L_Handler.Location = new System.Drawing.Point(50, 33);
            L_Handler.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Handler.Name = "L_Handler";
            L_Handler.Size = new System.Drawing.Size(140, 15);
            L_Handler.TabIndex = 101;
            L_Handler.Text = "Current Handler:";
            L_Handler.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_Handler
            // 
            CB_Handler.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Handler.Enabled = false;
            CB_Handler.FormattingEnabled = true;
            CB_Handler.Location = new System.Drawing.Point(203, 30);
            CB_Handler.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Handler.Name = "CB_Handler";
            CB_Handler.Size = new System.Drawing.Size(136, 23);
            CB_Handler.TabIndex = 26;
            // 
            // L_Arguments
            // 
            L_Arguments.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            L_Arguments.AutoSize = true;
            L_Arguments.Location = new System.Drawing.Point(14, 291);
            L_Arguments.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Arguments.Name = "L_Arguments";
            L_Arguments.Size = new System.Drawing.Size(37, 15);
            L_Arguments.TabIndex = 102;
            L_Arguments.Text = "(args)";
            L_Arguments.Visible = false;
            // 
            // MemoryAmie
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            BackColor = System.Drawing.SystemColors.Control;
            ClientSize = new System.Drawing.Size(436, 315);
            Controls.Add(L_Arguments);
            Controls.Add(tabControl1);
            Controls.Add(BTN_Cancel);
            Controls.Add(BTN_Save);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(452, 354);
            Name = "MemoryAmie";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Memories / Amie Editor";
            GB_M_OT.ResumeLayout(false);
            GB_M_OT.PerformLayout();
            GB_Residence.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            Tab_OTMemory.ResumeLayout(false);
            Tab_CTMemory.ResumeLayout(false);
            GB_M_CT.ResumeLayout(false);
            GB_M_CT.PerformLayout();
            Tab_Residence.ResumeLayout(false);
            Tab_Other.ResumeLayout(false);
            Tab_Other.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button BTN_Save;
        private System.Windows.Forms.Button BTN_Cancel;
        private System.Windows.Forms.MaskedTextBox M_OT_Friendship;
        private System.Windows.Forms.Label L_OT_Friendship;
        private System.Windows.Forms.Label L_Geo0;
        private System.Windows.Forms.Label L_Geo1;
        private System.Windows.Forms.Label L_Region;
        private System.Windows.Forms.Label L_Country;
        private System.Windows.Forms.Label L_Geo2;
        private System.Windows.Forms.Label L_Geo3;
        private System.Windows.Forms.Label L_Geo4;
        private System.Windows.Forms.Label L_OT_Quality;
        private System.Windows.Forms.Label L_OT_TextLine;
        private System.Windows.Forms.Label LOTV;
        private System.Windows.Forms.Label L_OT_Feeling;
        private System.Windows.Forms.GroupBox GB_M_OT;
        private System.Windows.Forms.GroupBox GB_Residence;
        private System.Windows.Forms.Label L_OT_Affection;
        private System.Windows.Forms.MaskedTextBox M_OT_Affection;
        private System.Windows.Forms.Label L_Enjoyment;
        private System.Windows.Forms.Label L_Fullness;
        private System.Windows.Forms.MaskedTextBox M_Enjoyment;
        private System.Windows.Forms.MaskedTextBox M_Fullness;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage Tab_Residence;
        private System.Windows.Forms.TabPage Tab_OTMemory;
        private System.Windows.Forms.Label L_Handler;
        private System.Windows.Forms.ComboBox CB_Handler;
        private System.Windows.Forms.ComboBox CB_Country4;
        private System.Windows.Forms.ComboBox CB_Country3;
        private System.Windows.Forms.ComboBox CB_Country2;
        private System.Windows.Forms.ComboBox CB_Country1;
        private System.Windows.Forms.ComboBox CB_Country0;
        private System.Windows.Forms.TabPage Tab_CTMemory;
        private System.Windows.Forms.GroupBox GB_M_CT;
        private System.Windows.Forms.Label L_CT_Affection;
        private System.Windows.Forms.Label L_CT_Friendship;
        private System.Windows.Forms.MaskedTextBox M_CT_Affection;
        private System.Windows.Forms.MaskedTextBox M_CT_Friendship;
        private System.Windows.Forms.Label LCTV;
        private System.Windows.Forms.Label L_CT_Feeling;
        private System.Windows.Forms.Label L_CT_TextLine;
        private System.Windows.Forms.Label L_CT_Quality;
        private System.Windows.Forms.ComboBox CB_OTQual;
        private System.Windows.Forms.ComboBox CB_OTFeel;
        private System.Windows.Forms.ComboBox CB_CTQual;
        private System.Windows.Forms.ComboBox CB_CTFeel;
        private System.Windows.Forms.ComboBox CB_OTVar;
        private System.Windows.Forms.ComboBox CB_OTMemory;
        private System.Windows.Forms.ComboBox CB_CTVar;
        private System.Windows.Forms.ComboBox CB_CTMemory;
        private System.Windows.Forms.RichTextBox RTB_OT;
        private System.Windows.Forms.RichTextBox RTB_CT;
        private System.Windows.Forms.Label L_Arguments;
        private System.Windows.Forms.ComboBox CB_Region4;
        private System.Windows.Forms.ComboBox CB_Region3;
        private System.Windows.Forms.ComboBox CB_Region2;
        private System.Windows.Forms.ComboBox CB_Region1;
        private System.Windows.Forms.ComboBox CB_Region0;
        private System.Windows.Forms.Button B_ClearAll;
        private System.Windows.Forms.TabPage Tab_Other;
        private System.Windows.Forms.MaskedTextBox MT_Sociability;
        private System.Windows.Forms.Label L_Sociability;
    }
}
