namespace PKHeX
{
    partial class SAV_EventFlagsXY
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SAV_EventFlagsXY));
            this.CHK_CustomFlag = new System.Windows.Forms.CheckBox();
            this.B_Cancel = new System.Windows.Forms.Button();
            this.GB_FlagStatus = new System.Windows.Forms.GroupBox();
            this.nud = new System.Windows.Forms.NumericUpDown();
            this.L_Flag = new System.Windows.Forms.Label();
            this.flag_0001 = new System.Windows.Forms.CheckBox();
            this.flag_0002 = new System.Windows.Forms.CheckBox();
            this.flag_0003 = new System.Windows.Forms.CheckBox();
            this.flag_0004 = new System.Windows.Forms.CheckBox();
            this.flag_0005 = new System.Windows.Forms.CheckBox();
            this.B_Save = new System.Windows.Forms.Button();
            this.flag_2237 = new System.Windows.Forms.CheckBox();
            this.flag_2238 = new System.Windows.Forms.CheckBox();
            this.flag_2239 = new System.Windows.Forms.CheckBox();
            this.GB_Researcher = new System.Windows.Forms.GroupBox();
            this.L_UnSet = new System.Windows.Forms.Label();
            this.L_IsSet = new System.Windows.Forms.Label();
            this.TB_NewSAV = new System.Windows.Forms.TextBox();
            this.TB_OldSAV = new System.Windows.Forms.TextBox();
            this.TB_UnSet = new System.Windows.Forms.TextBox();
            this.TB_IsSet = new System.Windows.Forms.TextBox();
            this.B_LoadNew = new System.Windows.Forms.Button();
            this.B_LoadOld = new System.Windows.Forms.Button();
            this.flag_0963 = new System.Windows.Forms.CheckBox();
            this.flag_0115 = new System.Windows.Forms.CheckBox();
            this.GB_Rebattle = new System.Windows.Forms.GroupBox();
            this.flag_0114 = new System.Windows.Forms.CheckBox();
            this.flag_0790 = new System.Windows.Forms.CheckBox();
            this.GB_Misc = new System.Windows.Forms.GroupBox();
            this.flag_0289 = new System.Windows.Forms.CheckBox();
            this.flag_0288 = new System.Windows.Forms.CheckBox();
            this.flag_0287 = new System.Windows.Forms.CheckBox();
            this.flag_0294 = new System.Windows.Forms.CheckBox();
            this.flag_0293 = new System.Windows.Forms.CheckBox();
            this.flag_0292 = new System.Windows.Forms.CheckBox();
            this.flag_0291 = new System.Windows.Forms.CheckBox();
            this.flag_0290 = new System.Windows.Forms.CheckBox();
            this.flag_0675 = new System.Windows.Forms.CheckBox();
            this.flag_0286 = new System.Windows.Forms.CheckBox();
            this.flag_0285 = new System.Windows.Forms.CheckBox();
            this.flag_2546 = new System.Windows.Forms.CheckBox();
            this.GB_FlagStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud)).BeginInit();
            this.GB_Researcher.SuspendLayout();
            this.GB_Rebattle.SuspendLayout();
            this.GB_Misc.SuspendLayout();
            this.SuspendLayout();
            // 
            // CHK_CustomFlag
            // 
            this.CHK_CustomFlag.AutoSize = true;
            this.CHK_CustomFlag.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CHK_CustomFlag.Enabled = false;
            this.CHK_CustomFlag.Location = new System.Drawing.Point(12, 44);
            this.CHK_CustomFlag.Name = "CHK_CustomFlag";
            this.CHK_CustomFlag.Size = new System.Drawing.Size(59, 17);
            this.CHK_CustomFlag.TabIndex = 1;
            this.CHK_CustomFlag.Text = "Status:";
            this.CHK_CustomFlag.UseVisualStyleBackColor = true;
            this.CHK_CustomFlag.CheckedChanged += new System.EventHandler(this.changeCustomBool);
            // 
            // B_Cancel
            // 
            this.B_Cancel.Location = new System.Drawing.Point(287, 307);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(75, 23);
            this.B_Cancel.TabIndex = 2;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // GB_FlagStatus
            // 
            this.GB_FlagStatus.Controls.Add(this.nud);
            this.GB_FlagStatus.Controls.Add(this.L_Flag);
            this.GB_FlagStatus.Controls.Add(this.CHK_CustomFlag);
            this.GB_FlagStatus.Location = new System.Drawing.Point(14, 135);
            this.GB_FlagStatus.Name = "GB_FlagStatus";
            this.GB_FlagStatus.Size = new System.Drawing.Size(108, 68);
            this.GB_FlagStatus.TabIndex = 3;
            this.GB_FlagStatus.TabStop = false;
            this.GB_FlagStatus.Text = "Check Flag Status";
            // 
            // nud
            // 
            this.nud.Location = new System.Drawing.Point(56, 19);
            this.nud.Maximum = new decimal(new int[] {
            3072,
            0,
            0,
            0});
            this.nud.Name = "nud";
            this.nud.Size = new System.Drawing.Size(45, 20);
            this.nud.TabIndex = 9;
            this.nud.Value = new decimal(new int[] {
            3071,
            0,
            0,
            0});
            this.nud.ValueChanged += new System.EventHandler(this.changeCustomFlag);
            this.nud.KeyUp += new System.Windows.Forms.KeyEventHandler(this.changeCustomFlag);
            // 
            // L_Flag
            // 
            this.L_Flag.AutoSize = true;
            this.L_Flag.Location = new System.Drawing.Point(13, 21);
            this.L_Flag.Name = "L_Flag";
            this.L_Flag.Size = new System.Drawing.Size(30, 13);
            this.L_Flag.TabIndex = 2;
            this.L_Flag.Text = "Flag:";
            // 
            // flag_0001
            // 
            this.flag_0001.AutoSize = true;
            this.flag_0001.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.flag_0001.Location = new System.Drawing.Point(12, 12);
            this.flag_0001.Name = "flag_0001";
            this.flag_0001.Size = new System.Drawing.Size(52, 17);
            this.flag_0001.TabIndex = 4;
            this.flag_0001.Text = "Flag1";
            this.flag_0001.UseVisualStyleBackColor = true;
            this.flag_0001.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_0002
            // 
            this.flag_0002.AutoSize = true;
            this.flag_0002.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.flag_0002.Location = new System.Drawing.Point(12, 28);
            this.flag_0002.Name = "flag_0002";
            this.flag_0002.Size = new System.Drawing.Size(52, 17);
            this.flag_0002.TabIndex = 5;
            this.flag_0002.Text = "Flag2";
            this.flag_0002.UseVisualStyleBackColor = true;
            this.flag_0002.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_0003
            // 
            this.flag_0003.AutoSize = true;
            this.flag_0003.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.flag_0003.Location = new System.Drawing.Point(12, 44);
            this.flag_0003.Name = "flag_0003";
            this.flag_0003.Size = new System.Drawing.Size(52, 17);
            this.flag_0003.TabIndex = 6;
            this.flag_0003.Text = "Flag3";
            this.flag_0003.UseVisualStyleBackColor = true;
            this.flag_0003.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_0004
            // 
            this.flag_0004.AutoSize = true;
            this.flag_0004.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.flag_0004.Location = new System.Drawing.Point(12, 60);
            this.flag_0004.Name = "flag_0004";
            this.flag_0004.Size = new System.Drawing.Size(52, 17);
            this.flag_0004.TabIndex = 7;
            this.flag_0004.Text = "Flag4";
            this.flag_0004.UseVisualStyleBackColor = true;
            this.flag_0004.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_0005
            // 
            this.flag_0005.AutoSize = true;
            this.flag_0005.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.flag_0005.Location = new System.Drawing.Point(12, 76);
            this.flag_0005.Name = "flag_0005";
            this.flag_0005.Size = new System.Drawing.Size(52, 17);
            this.flag_0005.TabIndex = 8;
            this.flag_0005.Text = "Flag5";
            this.flag_0005.UseVisualStyleBackColor = true;
            this.flag_0005.Click += new System.EventHandler(this.toggleFlag);
            // 
            // B_Save
            // 
            this.B_Save.Location = new System.Drawing.Point(366, 306);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(75, 23);
            this.B_Save.TabIndex = 9;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // flag_2237
            // 
            this.flag_2237.AutoSize = true;
            this.flag_2237.Location = new System.Drawing.Point(72, 12);
            this.flag_2237.Name = "flag_2237";
            this.flag_2237.Size = new System.Drawing.Size(50, 17);
            this.flag_2237.TabIndex = 10;
            this.flag_2237.Text = "2237";
            this.flag_2237.UseVisualStyleBackColor = true;
            this.flag_2237.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_2238
            // 
            this.flag_2238.AutoSize = true;
            this.flag_2238.Location = new System.Drawing.Point(72, 28);
            this.flag_2238.Name = "flag_2238";
            this.flag_2238.Size = new System.Drawing.Size(50, 17);
            this.flag_2238.TabIndex = 11;
            this.flag_2238.Text = "2238";
            this.flag_2238.UseVisualStyleBackColor = true;
            this.flag_2238.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_2239
            // 
            this.flag_2239.AutoSize = true;
            this.flag_2239.Location = new System.Drawing.Point(72, 44);
            this.flag_2239.Name = "flag_2239";
            this.flag_2239.Size = new System.Drawing.Size(50, 17);
            this.flag_2239.TabIndex = 12;
            this.flag_2239.Text = "2239";
            this.flag_2239.UseVisualStyleBackColor = true;
            this.flag_2239.Click += new System.EventHandler(this.toggleFlag);
            // 
            // GB_Researcher
            // 
            this.GB_Researcher.Controls.Add(this.L_UnSet);
            this.GB_Researcher.Controls.Add(this.L_IsSet);
            this.GB_Researcher.Controls.Add(this.TB_NewSAV);
            this.GB_Researcher.Controls.Add(this.TB_OldSAV);
            this.GB_Researcher.Controls.Add(this.TB_UnSet);
            this.GB_Researcher.Controls.Add(this.TB_IsSet);
            this.GB_Researcher.Controls.Add(this.B_LoadNew);
            this.GB_Researcher.Controls.Add(this.B_LoadOld);
            this.GB_Researcher.Location = new System.Drawing.Point(14, 210);
            this.GB_Researcher.Name = "GB_Researcher";
            this.GB_Researcher.Size = new System.Drawing.Size(268, 120);
            this.GB_Researcher.TabIndex = 13;
            this.GB_Researcher.TabStop = false;
            this.GB_Researcher.Text = "FlagDiff Researcher";
            // 
            // L_UnSet
            // 
            this.L_UnSet.AutoSize = true;
            this.L_UnSet.Location = new System.Drawing.Point(10, 97);
            this.L_UnSet.Name = "L_UnSet";
            this.L_UnSet.Size = new System.Drawing.Size(37, 13);
            this.L_UnSet.TabIndex = 7;
            this.L_UnSet.Text = "UnSet";
            // 
            // L_IsSet
            // 
            this.L_IsSet.AutoSize = true;
            this.L_IsSet.Location = new System.Drawing.Point(12, 77);
            this.L_IsSet.Name = "L_IsSet";
            this.L_IsSet.Size = new System.Drawing.Size(31, 13);
            this.L_IsSet.TabIndex = 6;
            this.L_IsSet.Text = "IsSet";
            // 
            // TB_NewSAV
            // 
            this.TB_NewSAV.Location = new System.Drawing.Point(93, 47);
            this.TB_NewSAV.Name = "TB_NewSAV";
            this.TB_NewSAV.ReadOnly = true;
            this.TB_NewSAV.Size = new System.Drawing.Size(169, 20);
            this.TB_NewSAV.TabIndex = 5;
            this.TB_NewSAV.TextChanged += new System.EventHandler(this.changeSAV);
            // 
            // TB_OldSAV
            // 
            this.TB_OldSAV.Location = new System.Drawing.Point(93, 21);
            this.TB_OldSAV.Name = "TB_OldSAV";
            this.TB_OldSAV.ReadOnly = true;
            this.TB_OldSAV.Size = new System.Drawing.Size(169, 20);
            this.TB_OldSAV.TabIndex = 4;
            this.TB_OldSAV.TextChanged += new System.EventHandler(this.changeSAV);
            // 
            // TB_UnSet
            // 
            this.TB_UnSet.Location = new System.Drawing.Point(56, 94);
            this.TB_UnSet.Name = "TB_UnSet";
            this.TB_UnSet.ReadOnly = true;
            this.TB_UnSet.Size = new System.Drawing.Size(206, 20);
            this.TB_UnSet.TabIndex = 3;
            // 
            // TB_IsSet
            // 
            this.TB_IsSet.Location = new System.Drawing.Point(56, 74);
            this.TB_IsSet.Name = "TB_IsSet";
            this.TB_IsSet.ReadOnly = true;
            this.TB_IsSet.Size = new System.Drawing.Size(206, 20);
            this.TB_IsSet.TabIndex = 2;
            // 
            // B_LoadNew
            // 
            this.B_LoadNew.Location = new System.Drawing.Point(12, 45);
            this.B_LoadNew.Name = "B_LoadNew";
            this.B_LoadNew.Size = new System.Drawing.Size(75, 23);
            this.B_LoadNew.TabIndex = 1;
            this.B_LoadNew.Text = "Load New";
            this.B_LoadNew.UseVisualStyleBackColor = true;
            this.B_LoadNew.Click += new System.EventHandler(this.openSAV);
            // 
            // B_LoadOld
            // 
            this.B_LoadOld.Location = new System.Drawing.Point(12, 19);
            this.B_LoadOld.Name = "B_LoadOld";
            this.B_LoadOld.Size = new System.Drawing.Size(75, 23);
            this.B_LoadOld.TabIndex = 0;
            this.B_LoadOld.Text = "Load Old";
            this.B_LoadOld.UseVisualStyleBackColor = true;
            this.B_LoadOld.Click += new System.EventHandler(this.openSAV);
            // 
            // flag_0963
            // 
            this.flag_0963.AutoSize = true;
            this.flag_0963.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.flag_0963.Location = new System.Drawing.Point(16, 19);
            this.flag_0963.Name = "flag_0963";
            this.flag_0963.Size = new System.Drawing.Size(113, 17);
            this.flag_0963.TabIndex = 14;
            this.flag_0963.Text = "Mewtwo Defeated";
            this.flag_0963.UseVisualStyleBackColor = true;
            this.flag_0963.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_0115
            // 
            this.flag_0115.AutoSize = true;
            this.flag_0115.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.flag_0115.Location = new System.Drawing.Point(16, 34);
            this.flag_0115.Name = "flag_0115";
            this.flag_0115.Size = new System.Drawing.Size(112, 17);
            this.flag_0115.TabIndex = 15;
            this.flag_0115.Text = "Mewtwo Captured";
            this.flag_0115.UseVisualStyleBackColor = true;
            this.flag_0115.Click += new System.EventHandler(this.toggleFlag);
            // 
            // GB_Rebattle
            // 
            this.GB_Rebattle.Controls.Add(this.flag_0114);
            this.GB_Rebattle.Controls.Add(this.flag_0790);
            this.GB_Rebattle.Controls.Add(this.flag_0115);
            this.GB_Rebattle.Controls.Add(this.flag_0963);
            this.GB_Rebattle.Location = new System.Drawing.Point(129, 12);
            this.GB_Rebattle.Name = "GB_Rebattle";
            this.GB_Rebattle.Size = new System.Drawing.Size(152, 191);
            this.GB_Rebattle.TabIndex = 16;
            this.GB_Rebattle.TabStop = false;
            this.GB_Rebattle.Text = "Rebattle";
            // 
            // flag_0114
            // 
            this.flag_0114.AutoSize = true;
            this.flag_0114.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.flag_0114.Location = new System.Drawing.Point(16, 72);
            this.flag_0114.Name = "flag_0114";
            this.flag_0114.Size = new System.Drawing.Size(111, 17);
            this.flag_0114.TabIndex = 17;
            this.flag_0114.Text = "Zygarde Captured";
            this.flag_0114.UseVisualStyleBackColor = true;
            this.flag_0114.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_0790
            // 
            this.flag_0790.AutoSize = true;
            this.flag_0790.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.flag_0790.Location = new System.Drawing.Point(16, 57);
            this.flag_0790.Name = "flag_0790";
            this.flag_0790.Size = new System.Drawing.Size(112, 17);
            this.flag_0790.TabIndex = 16;
            this.flag_0790.Text = "Zygarde Defeated";
            this.flag_0790.UseVisualStyleBackColor = true;
            this.flag_0790.Click += new System.EventHandler(this.toggleFlag);
            // 
            // GB_Misc
            // 
            this.GB_Misc.Controls.Add(this.flag_0289);
            this.GB_Misc.Controls.Add(this.flag_0288);
            this.GB_Misc.Controls.Add(this.flag_0287);
            this.GB_Misc.Controls.Add(this.flag_0294);
            this.GB_Misc.Controls.Add(this.flag_0293);
            this.GB_Misc.Controls.Add(this.flag_0292);
            this.GB_Misc.Controls.Add(this.flag_0291);
            this.GB_Misc.Controls.Add(this.flag_0290);
            this.GB_Misc.Controls.Add(this.flag_0675);
            this.GB_Misc.Controls.Add(this.flag_0286);
            this.GB_Misc.Controls.Add(this.flag_0285);
            this.GB_Misc.Location = new System.Drawing.Point(287, 12);
            this.GB_Misc.Name = "GB_Misc";
            this.GB_Misc.Size = new System.Drawing.Size(154, 288);
            this.GB_Misc.TabIndex = 17;
            this.GB_Misc.TabStop = false;
            this.GB_Misc.Text = "Misc";
            // 
            // flag_0289
            // 
            this.flag_0289.AutoSize = true;
            this.flag_0289.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.flag_0289.Location = new System.Drawing.Point(6, 79);
            this.flag_0289.Name = "flag_0289";
            this.flag_0289.Size = new System.Drawing.Size(94, 17);
            this.flag_0289.TabIndex = 25;
            this.flag_0289.Text = "Multi Statuette";
            this.flag_0289.UseVisualStyleBackColor = true;
            this.flag_0289.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_0288
            // 
            this.flag_0288.AutoSize = true;
            this.flag_0288.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.flag_0288.Location = new System.Drawing.Point(6, 64);
            this.flag_0288.Name = "flag_0288";
            this.flag_0288.Size = new System.Drawing.Size(112, 17);
            this.flag_0288.TabIndex = 24;
            this.flag_0288.Text = "Rotation Statuette";
            this.flag_0288.UseVisualStyleBackColor = true;
            this.flag_0288.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_0287
            // 
            this.flag_0287.AutoSize = true;
            this.flag_0287.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.flag_0287.Location = new System.Drawing.Point(6, 49);
            this.flag_0287.Name = "flag_0287";
            this.flag_0287.Size = new System.Drawing.Size(103, 17);
            this.flag_0287.TabIndex = 23;
            this.flag_0287.Text = "Triples Statuette";
            this.flag_0287.UseVisualStyleBackColor = true;
            this.flag_0287.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_0294
            // 
            this.flag_0294.AutoSize = true;
            this.flag_0294.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.flag_0294.Location = new System.Drawing.Point(6, 159);
            this.flag_0294.Name = "flag_0294";
            this.flag_0294.Size = new System.Drawing.Size(128, 17);
            this.flag_0294.TabIndex = 22;
            this.flag_0294.Text = "Super Multi Unlocked";
            this.flag_0294.UseVisualStyleBackColor = true;
            this.flag_0294.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_0293
            // 
            this.flag_0293.AutoSize = true;
            this.flag_0293.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.flag_0293.Location = new System.Drawing.Point(6, 144);
            this.flag_0293.Name = "flag_0293";
            this.flag_0293.Size = new System.Drawing.Size(146, 17);
            this.flag_0293.TabIndex = 21;
            this.flag_0293.Text = "Super Rotation Unlocked";
            this.flag_0293.UseVisualStyleBackColor = true;
            this.flag_0293.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_0292
            // 
            this.flag_0292.AutoSize = true;
            this.flag_0292.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.flag_0292.Location = new System.Drawing.Point(6, 129);
            this.flag_0292.Name = "flag_0292";
            this.flag_0292.Size = new System.Drawing.Size(137, 17);
            this.flag_0292.TabIndex = 20;
            this.flag_0292.Text = "Super Triples Unlocked";
            this.flag_0292.UseVisualStyleBackColor = true;
            this.flag_0292.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_0291
            // 
            this.flag_0291.AutoSize = true;
            this.flag_0291.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.flag_0291.Location = new System.Drawing.Point(6, 114);
            this.flag_0291.Name = "flag_0291";
            this.flag_0291.Size = new System.Drawing.Size(145, 17);
            this.flag_0291.TabIndex = 19;
            this.flag_0291.Text = "Super Doubles Unlocked";
            this.flag_0291.UseVisualStyleBackColor = true;
            this.flag_0291.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_0290
            // 
            this.flag_0290.AutoSize = true;
            this.flag_0290.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.flag_0290.Location = new System.Drawing.Point(6, 99);
            this.flag_0290.Name = "flag_0290";
            this.flag_0290.Size = new System.Drawing.Size(140, 17);
            this.flag_0290.TabIndex = 18;
            this.flag_0290.Text = "Super Singles Unlocked";
            this.flag_0290.UseVisualStyleBackColor = true;
            this.flag_0290.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_0675
            // 
            this.flag_0675.AutoSize = true;
            this.flag_0675.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.flag_0675.Location = new System.Drawing.Point(15, 243);
            this.flag_0675.Name = "flag_0675";
            this.flag_0675.Size = new System.Drawing.Size(119, 17);
            this.flag_0675.TabIndex = 17;
            this.flag_0675.Text = "50: Beat Chatelaine";
            this.flag_0675.UseVisualStyleBackColor = true;
            this.flag_0675.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_0286
            // 
            this.flag_0286.AutoSize = true;
            this.flag_0286.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.flag_0286.Location = new System.Drawing.Point(6, 34);
            this.flag_0286.Name = "flag_0286";
            this.flag_0286.Size = new System.Drawing.Size(111, 17);
            this.flag_0286.TabIndex = 16;
            this.flag_0286.Text = "Doubles Statuette";
            this.flag_0286.UseVisualStyleBackColor = true;
            this.flag_0286.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_0285
            // 
            this.flag_0285.AutoSize = true;
            this.flag_0285.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.flag_0285.Location = new System.Drawing.Point(6, 19);
            this.flag_0285.Name = "flag_0285";
            this.flag_0285.Size = new System.Drawing.Size(106, 17);
            this.flag_0285.TabIndex = 15;
            this.flag_0285.Text = "Singles Statuette";
            this.flag_0285.UseVisualStyleBackColor = true;
            this.flag_0285.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_2546
            // 
            this.flag_2546.AutoSize = true;
            this.flag_2546.Location = new System.Drawing.Point(13, 112);
            this.flag_2546.Name = "flag_2546";
            this.flag_2546.Size = new System.Drawing.Size(114, 17);
            this.flag_2546.TabIndex = 18;
            this.flag_2546.Text = "Pokédex Obtained";
            this.flag_2546.UseVisualStyleBackColor = true;
            this.flag_2546.Click += new System.EventHandler(this.toggleFlag);
            // 
            // SAV_EventFlagsXY
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 342);
            this.Controls.Add(this.flag_2546);
            this.Controls.Add(this.GB_Misc);
            this.Controls.Add(this.GB_Rebattle);
            this.Controls.Add(this.GB_Researcher);
            this.Controls.Add(this.flag_2239);
            this.Controls.Add(this.flag_2238);
            this.Controls.Add(this.flag_2237);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.flag_0005);
            this.Controls.Add(this.flag_0004);
            this.Controls.Add(this.flag_0003);
            this.Controls.Add(this.flag_0002);
            this.Controls.Add(this.flag_0001);
            this.Controls.Add(this.GB_FlagStatus);
            this.Controls.Add(this.B_Cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SAV_EventFlagsXY";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Event Flag Editor";
            this.GB_FlagStatus.ResumeLayout(false);
            this.GB_FlagStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud)).EndInit();
            this.GB_Researcher.ResumeLayout(false);
            this.GB_Researcher.PerformLayout();
            this.GB_Rebattle.ResumeLayout(false);
            this.GB_Rebattle.PerformLayout();
            this.GB_Misc.ResumeLayout(false);
            this.GB_Misc.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox CHK_CustomFlag;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.GroupBox GB_FlagStatus;
        private System.Windows.Forms.Label L_Flag;
        private System.Windows.Forms.CheckBox flag_0001;
        private System.Windows.Forms.CheckBox flag_0002;
        private System.Windows.Forms.CheckBox flag_0003;
        private System.Windows.Forms.CheckBox flag_0004;
        private System.Windows.Forms.CheckBox flag_0005;
        private System.Windows.Forms.NumericUpDown nud;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.CheckBox flag_2237;
        private System.Windows.Forms.CheckBox flag_2238;
        private System.Windows.Forms.CheckBox flag_2239;
        private System.Windows.Forms.GroupBox GB_Researcher;
        private System.Windows.Forms.Label L_UnSet;
        private System.Windows.Forms.Label L_IsSet;
        private System.Windows.Forms.TextBox TB_NewSAV;
        private System.Windows.Forms.TextBox TB_OldSAV;
        private System.Windows.Forms.TextBox TB_UnSet;
        private System.Windows.Forms.TextBox TB_IsSet;
        private System.Windows.Forms.Button B_LoadNew;
        private System.Windows.Forms.Button B_LoadOld;
        private System.Windows.Forms.CheckBox flag_0963;
        private System.Windows.Forms.CheckBox flag_0115;
        private System.Windows.Forms.GroupBox GB_Rebattle;
        private System.Windows.Forms.CheckBox flag_0114;
        private System.Windows.Forms.CheckBox flag_0790;
        private System.Windows.Forms.GroupBox GB_Misc;
        private System.Windows.Forms.CheckBox flag_0289;
        private System.Windows.Forms.CheckBox flag_0288;
        private System.Windows.Forms.CheckBox flag_0287;
        private System.Windows.Forms.CheckBox flag_0294;
        private System.Windows.Forms.CheckBox flag_0293;
        private System.Windows.Forms.CheckBox flag_0292;
        private System.Windows.Forms.CheckBox flag_0291;
        private System.Windows.Forms.CheckBox flag_0290;
        private System.Windows.Forms.CheckBox flag_0675;
        private System.Windows.Forms.CheckBox flag_0286;
        private System.Windows.Forms.CheckBox flag_0285;
        private System.Windows.Forms.CheckBox flag_2546;
    }
}