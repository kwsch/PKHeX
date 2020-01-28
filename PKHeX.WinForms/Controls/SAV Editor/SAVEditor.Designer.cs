namespace PKHeX.WinForms.Controls
{
    partial class SAVEditor
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
            SortMenu?.Dispose();
            menu?.Dispose();
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabBoxMulti = new System.Windows.Forms.TabControl();
            this.Tab_Box = new System.Windows.Forms.TabPage();
            this.Box = new PKHeX.WinForms.Controls.BoxEditor();
            this.Tab_PartyBattle = new System.Windows.Forms.TabPage();
            this.SL_Party = new PKHeX.WinForms.Controls.PartyEditor();
            this.Tab_Other = new System.Windows.Forms.TabPage();
            this.SL_Extra = new PKHeX.WinForms.Controls.SlotList();
            this.GB_Daycare = new System.Windows.Forms.GroupBox();
            this.L_XP2 = new System.Windows.Forms.Label();
            this.L_XP1 = new System.Windows.Forms.Label();
            this.TB_Daycare2XP = new System.Windows.Forms.TextBox();
            this.TB_Daycare1XP = new System.Windows.Forms.TextBox();
            this.L_DC2 = new System.Windows.Forms.Label();
            this.L_DC1 = new System.Windows.Forms.Label();
            this.L_DaycareSeed = new System.Windows.Forms.Label();
            this.TB_RNGSeed = new System.Windows.Forms.TextBox();
            this.dcpkx2 = new System.Windows.Forms.PictureBox();
            this.dcpkx1 = new System.Windows.Forms.PictureBox();
            this.DayCare_HasEgg = new System.Windows.Forms.CheckBox();
            this.L_ReadOnlyOther = new System.Windows.Forms.Label();
            this.Tab_SAV = new System.Windows.Forms.TabPage();
            this.CB_SaveSlot = new System.Windows.Forms.ComboBox();
            this.GB_SAVtools = new System.Windows.Forms.GroupBox();
            this.FLP_SAVtools = new System.Windows.Forms.FlowLayoutPanel();
            this.B_OpenTrainerInfo = new System.Windows.Forms.Button();
            this.B_OpenItemPouch = new System.Windows.Forms.Button();
            this.B_OpenBoxLayout = new System.Windows.Forms.Button();
            this.B_OpenWondercards = new System.Windows.Forms.Button();
            this.B_OpenOPowers = new System.Windows.Forms.Button();
            this.B_OpenEventFlags = new System.Windows.Forms.Button();
            this.B_OpenPokedex = new System.Windows.Forms.Button();
            this.B_OpenLinkInfo = new System.Windows.Forms.Button();
            this.B_OpenBerryField = new System.Windows.Forms.Button();
            this.B_OpenPokeblocks = new System.Windows.Forms.Button();
            this.B_OpenSecretBase = new System.Windows.Forms.Button();
            this.B_OpenPokepuffs = new System.Windows.Forms.Button();
            this.B_OpenSuperTraining = new System.Windows.Forms.Button();
            this.B_OpenHallofFame = new System.Windows.Forms.Button();
            this.B_OUTPasserby = new System.Windows.Forms.Button();
            this.B_CGearSkin = new System.Windows.Forms.Button();
            this.B_OpenPokeBeans = new System.Windows.Forms.Button();
            this.B_CellsStickers = new System.Windows.Forms.Button();
            this.B_OpenMiscEditor = new System.Windows.Forms.Button();
            this.B_OpenHoneyTreeEditor = new System.Windows.Forms.Button();
            this.B_OpenFriendSafari = new System.Windows.Forms.Button();
            this.B_OpenRTCEditor = new System.Windows.Forms.Button();
            this.B_OpenUGSEditor = new System.Windows.Forms.Button();
            this.B_Roamer = new System.Windows.Forms.Button();
            this.B_FestivalPlaza = new System.Windows.Forms.Button();
            this.B_MailBox = new System.Windows.Forms.Button();
            this.B_OpenApricorn = new System.Windows.Forms.Button();
            this.B_Raids = new System.Windows.Forms.Button();
            this.B_Blocks = new System.Windows.Forms.Button();
            this.L_SaveSlot = new System.Windows.Forms.Label();
            this.L_Secure2 = new System.Windows.Forms.Label();
            this.TB_Secure2 = new System.Windows.Forms.TextBox();
            this.L_Secure1 = new System.Windows.Forms.Label();
            this.TB_Secure1 = new System.Windows.Forms.TextBox();
            this.B_JPEG = new System.Windows.Forms.Button();
            this.L_GameSync = new System.Windows.Forms.Label();
            this.TB_GameSync = new System.Windows.Forms.TextBox();
            this.B_SaveBoxBin = new System.Windows.Forms.Button();
            this.B_VerifyCHK = new System.Windows.Forms.Button();
            this.tabBoxMulti.SuspendLayout();
            this.Tab_Box.SuspendLayout();
            this.Tab_PartyBattle.SuspendLayout();
            this.Tab_Other.SuspendLayout();
            this.GB_Daycare.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dcpkx2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dcpkx1)).BeginInit();
            this.Tab_SAV.SuspendLayout();
            this.GB_SAVtools.SuspendLayout();
            this.FLP_SAVtools.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabBoxMulti
            // 
            this.tabBoxMulti.AllowDrop = true;
            this.tabBoxMulti.Controls.Add(this.Tab_Box);
            this.tabBoxMulti.Controls.Add(this.Tab_PartyBattle);
            this.tabBoxMulti.Controls.Add(this.Tab_Other);
            this.tabBoxMulti.Controls.Add(this.Tab_SAV);
            this.tabBoxMulti.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabBoxMulti.Location = new System.Drawing.Point(0, 0);
            this.tabBoxMulti.Name = "tabBoxMulti";
            this.tabBoxMulti.SelectedIndex = 0;
            this.tabBoxMulti.Size = new System.Drawing.Size(449, 363);
            this.tabBoxMulti.TabIndex = 101;
            this.tabBoxMulti.DragOver += new System.Windows.Forms.DragEventHandler(this.MultiDragOver);
            this.tabBoxMulti.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ClickBoxSort);
            this.tabBoxMulti.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ClickBoxDouble);
            // 
            // Tab_Box
            // 
            this.Tab_Box.AllowDrop = true;
            this.Tab_Box.Controls.Add(this.Box);
            this.Tab_Box.Location = new System.Drawing.Point(4, 22);
            this.Tab_Box.Name = "Tab_Box";
            this.Tab_Box.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_Box.Size = new System.Drawing.Size(441, 337);
            this.Tab_Box.TabIndex = 0;
            this.Tab_Box.Text = "Box";
            this.Tab_Box.UseVisualStyleBackColor = true;
            // 
            // Box
            // 
            this.Box.AllowDrop = true;
            this.Box.AutoSize = true;
            this.Box.CanSetCurrentBox = true;
            this.Box.ControlsEnabled = true;
            this.Box.ControlsVisible = true;
            this.Box.CurrentBox = -1;
            this.Box.Editor = null;
            this.Box.FlagIllegal = false;
            this.Box.Location = new System.Drawing.Point(99, 7);
            this.Box.M = null;
            this.Box.Name = "Box";
            this.Box.Size = new System.Drawing.Size(251, 185);
            this.Box.TabIndex = 1;
            // 
            // Tab_PartyBattle
            // 
            this.Tab_PartyBattle.AllowDrop = true;
            this.Tab_PartyBattle.Controls.Add(this.SL_Party);
            this.Tab_PartyBattle.Location = new System.Drawing.Point(4, 22);
            this.Tab_PartyBattle.Name = "Tab_PartyBattle";
            this.Tab_PartyBattle.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_PartyBattle.Size = new System.Drawing.Size(441, 337);
            this.Tab_PartyBattle.TabIndex = 1;
            this.Tab_PartyBattle.Text = "Party";
            this.Tab_PartyBattle.UseVisualStyleBackColor = true;
            // 
            // SL_Party
            // 
            this.SL_Party.AutoSize = true;
            this.SL_Party.FlagIllegal = false;
            this.SL_Party.Location = new System.Drawing.Point(6, 6);
            this.SL_Party.M = null;
            this.SL_Party.Name = "SL_Party";
            this.SL_Party.Size = new System.Drawing.Size(251, 185);
            this.SL_Party.TabIndex = 0;
            // 
            // Tab_Other
            // 
            this.Tab_Other.Controls.Add(this.SL_Extra);
            this.Tab_Other.Controls.Add(this.GB_Daycare);
            this.Tab_Other.Controls.Add(this.L_ReadOnlyOther);
            this.Tab_Other.Location = new System.Drawing.Point(4, 22);
            this.Tab_Other.Name = "Tab_Other";
            this.Tab_Other.Size = new System.Drawing.Size(441, 337);
            this.Tab_Other.TabIndex = 2;
            this.Tab_Other.Text = "Other";
            this.Tab_Other.UseVisualStyleBackColor = true;
            // 
            // SL_Extra
            // 
            this.SL_Extra.Dock = System.Windows.Forms.DockStyle.Right;
            this.SL_Extra.FlagIllegal = false;
            this.SL_Extra.Location = new System.Drawing.Point(366, 0);
            this.SL_Extra.Name = "SL_Extra";
            this.SL_Extra.SAV = null;
            this.SL_Extra.Size = new System.Drawing.Size(75, 337);
            this.SL_Extra.TabIndex = 30;
            this.SL_Extra.ViewIndex = -1;
            // 
            // GB_Daycare
            // 
            this.GB_Daycare.Controls.Add(this.L_XP2);
            this.GB_Daycare.Controls.Add(this.L_XP1);
            this.GB_Daycare.Controls.Add(this.TB_Daycare2XP);
            this.GB_Daycare.Controls.Add(this.TB_Daycare1XP);
            this.GB_Daycare.Controls.Add(this.L_DC2);
            this.GB_Daycare.Controls.Add(this.L_DC1);
            this.GB_Daycare.Controls.Add(this.L_DaycareSeed);
            this.GB_Daycare.Controls.Add(this.TB_RNGSeed);
            this.GB_Daycare.Controls.Add(this.dcpkx2);
            this.GB_Daycare.Controls.Add(this.dcpkx1);
            this.GB_Daycare.Controls.Add(this.DayCare_HasEgg);
            this.GB_Daycare.Location = new System.Drawing.Point(16, 4);
            this.GB_Daycare.Name = "GB_Daycare";
            this.GB_Daycare.Size = new System.Drawing.Size(205, 170);
            this.GB_Daycare.TabIndex = 28;
            this.GB_Daycare.TabStop = false;
            this.GB_Daycare.Text = "Daycare";
            // 
            // L_XP2
            // 
            this.L_XP2.AutoSize = true;
            this.L_XP2.Location = new System.Drawing.Point(74, 88);
            this.L_XP2.Name = "L_XP2";
            this.L_XP2.Size = new System.Drawing.Size(30, 13);
            this.L_XP2.TabIndex = 17;
            this.L_XP2.Text = "+XP:";
            // 
            // L_XP1
            // 
            this.L_XP1.AutoSize = true;
            this.L_XP1.Location = new System.Drawing.Point(74, 35);
            this.L_XP1.Name = "L_XP1";
            this.L_XP1.Size = new System.Drawing.Size(30, 13);
            this.L_XP1.TabIndex = 16;
            this.L_XP1.Text = "+XP:";
            // 
            // TB_Daycare2XP
            // 
            this.TB_Daycare2XP.Location = new System.Drawing.Point(108, 85);
            this.TB_Daycare2XP.Name = "TB_Daycare2XP";
            this.TB_Daycare2XP.ReadOnly = true;
            this.TB_Daycare2XP.Size = new System.Drawing.Size(73, 20);
            this.TB_Daycare2XP.TabIndex = 15;
            // 
            // TB_Daycare1XP
            // 
            this.TB_Daycare1XP.Location = new System.Drawing.Point(108, 32);
            this.TB_Daycare1XP.Name = "TB_Daycare1XP";
            this.TB_Daycare1XP.ReadOnly = true;
            this.TB_Daycare1XP.Size = new System.Drawing.Size(73, 20);
            this.TB_Daycare1XP.TabIndex = 14;
            // 
            // L_DC2
            // 
            this.L_DC2.AutoSize = true;
            this.L_DC2.Location = new System.Drawing.Point(74, 71);
            this.L_DC2.Name = "L_DC2";
            this.L_DC2.Size = new System.Drawing.Size(19, 13);
            this.L_DC2.TabIndex = 13;
            this.L_DC2.Text = "2: ";
            // 
            // L_DC1
            // 
            this.L_DC1.AutoSize = true;
            this.L_DC1.Location = new System.Drawing.Point(74, 18);
            this.L_DC1.Name = "L_DC1";
            this.L_DC1.Size = new System.Drawing.Size(19, 13);
            this.L_DC1.TabIndex = 12;
            this.L_DC1.Text = "1: ";
            // 
            // L_DaycareSeed
            // 
            this.L_DaycareSeed.AutoSize = true;
            this.L_DaycareSeed.Location = new System.Drawing.Point(23, 143);
            this.L_DaycareSeed.Name = "L_DaycareSeed";
            this.L_DaycareSeed.Size = new System.Drawing.Size(35, 13);
            this.L_DaycareSeed.TabIndex = 9;
            this.L_DaycareSeed.Text = "Seed:";
            // 
            // TB_RNGSeed
            // 
            this.TB_RNGSeed.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_RNGSeed.Location = new System.Drawing.Point(61, 140);
            this.TB_RNGSeed.MaxLength = 16;
            this.TB_RNGSeed.Name = "TB_RNGSeed";
            this.TB_RNGSeed.Size = new System.Drawing.Size(120, 20);
            this.TB_RNGSeed.TabIndex = 8;
            this.TB_RNGSeed.Text = "0123456789ABCDEF";
            this.TB_RNGSeed.Validated += new System.EventHandler(this.UpdateStringSeed);
            // 
            // dcpkx2
            // 
            this.dcpkx2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dcpkx2.Location = new System.Drawing.Point(26, 71);
            this.dcpkx2.Name = "dcpkx2";
            this.dcpkx2.Size = new System.Drawing.Size(42, 32);
            this.dcpkx2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.dcpkx2.TabIndex = 11;
            this.dcpkx2.TabStop = false;
            // 
            // dcpkx1
            // 
            this.dcpkx1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dcpkx1.Location = new System.Drawing.Point(26, 18);
            this.dcpkx1.Name = "dcpkx1";
            this.dcpkx1.Size = new System.Drawing.Size(42, 32);
            this.dcpkx1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.dcpkx1.TabIndex = 10;
            this.dcpkx1.TabStop = false;
            // 
            // DayCare_HasEgg
            // 
            this.DayCare_HasEgg.AutoSize = true;
            this.DayCare_HasEgg.Enabled = false;
            this.DayCare_HasEgg.Location = new System.Drawing.Point(61, 123);
            this.DayCare_HasEgg.Name = "DayCare_HasEgg";
            this.DayCare_HasEgg.Size = new System.Drawing.Size(91, 17);
            this.DayCare_HasEgg.TabIndex = 7;
            this.DayCare_HasEgg.Text = "Egg Available";
            this.DayCare_HasEgg.UseVisualStyleBackColor = true;
            // 
            // L_ReadOnlyOther
            // 
            this.L_ReadOnlyOther.ForeColor = System.Drawing.Color.Red;
            this.L_ReadOnlyOther.Location = new System.Drawing.Point(32, 179);
            this.L_ReadOnlyOther.Name = "L_ReadOnlyOther";
            this.L_ReadOnlyOther.Size = new System.Drawing.Size(170, 13);
            this.L_ReadOnlyOther.TabIndex = 29;
            this.L_ReadOnlyOther.Text = "This tab is read only.";
            this.L_ReadOnlyOther.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Tab_SAV
            // 
            this.Tab_SAV.Controls.Add(this.CB_SaveSlot);
            this.Tab_SAV.Controls.Add(this.GB_SAVtools);
            this.Tab_SAV.Controls.Add(this.L_SaveSlot);
            this.Tab_SAV.Controls.Add(this.L_Secure2);
            this.Tab_SAV.Controls.Add(this.TB_Secure2);
            this.Tab_SAV.Controls.Add(this.L_Secure1);
            this.Tab_SAV.Controls.Add(this.TB_Secure1);
            this.Tab_SAV.Controls.Add(this.B_JPEG);
            this.Tab_SAV.Controls.Add(this.L_GameSync);
            this.Tab_SAV.Controls.Add(this.TB_GameSync);
            this.Tab_SAV.Controls.Add(this.B_SaveBoxBin);
            this.Tab_SAV.Controls.Add(this.B_VerifyCHK);
            this.Tab_SAV.Location = new System.Drawing.Point(4, 22);
            this.Tab_SAV.Name = "Tab_SAV";
            this.Tab_SAV.Size = new System.Drawing.Size(441, 337);
            this.Tab_SAV.TabIndex = 3;
            this.Tab_SAV.Text = "SAV";
            this.Tab_SAV.UseVisualStyleBackColor = true;
            // 
            // CB_SaveSlot
            // 
            this.CB_SaveSlot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_SaveSlot.FormattingEnabled = true;
            this.CB_SaveSlot.Location = new System.Drawing.Point(150, 148);
            this.CB_SaveSlot.Name = "CB_SaveSlot";
            this.CB_SaveSlot.Size = new System.Drawing.Size(121, 21);
            this.CB_SaveSlot.TabIndex = 20;
            this.CB_SaveSlot.SelectedIndexChanged += new System.EventHandler(this.UpdateSaveSlot);
            // 
            // GB_SAVtools
            // 
            this.GB_SAVtools.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GB_SAVtools.Controls.Add(this.FLP_SAVtools);
            this.GB_SAVtools.Location = new System.Drawing.Point(0, 175);
            this.GB_SAVtools.Name = "GB_SAVtools";
            this.GB_SAVtools.Size = new System.Drawing.Size(441, 162);
            this.GB_SAVtools.TabIndex = 102;
            this.GB_SAVtools.TabStop = false;
            // 
            // FLP_SAVtools
            // 
            this.FLP_SAVtools.AutoScroll = true;
            this.FLP_SAVtools.Controls.Add(this.B_OpenTrainerInfo);
            this.FLP_SAVtools.Controls.Add(this.B_OpenItemPouch);
            this.FLP_SAVtools.Controls.Add(this.B_OpenBoxLayout);
            this.FLP_SAVtools.Controls.Add(this.B_OpenWondercards);
            this.FLP_SAVtools.Controls.Add(this.B_OpenOPowers);
            this.FLP_SAVtools.Controls.Add(this.B_OpenEventFlags);
            this.FLP_SAVtools.Controls.Add(this.B_OpenPokedex);
            this.FLP_SAVtools.Controls.Add(this.B_OpenLinkInfo);
            this.FLP_SAVtools.Controls.Add(this.B_OpenBerryField);
            this.FLP_SAVtools.Controls.Add(this.B_OpenPokeblocks);
            this.FLP_SAVtools.Controls.Add(this.B_OpenSecretBase);
            this.FLP_SAVtools.Controls.Add(this.B_OpenPokepuffs);
            this.FLP_SAVtools.Controls.Add(this.B_OpenSuperTraining);
            this.FLP_SAVtools.Controls.Add(this.B_OpenHallofFame);
            this.FLP_SAVtools.Controls.Add(this.B_OUTPasserby);
            this.FLP_SAVtools.Controls.Add(this.B_CGearSkin);
            this.FLP_SAVtools.Controls.Add(this.B_OpenPokeBeans);
            this.FLP_SAVtools.Controls.Add(this.B_CellsStickers);
            this.FLP_SAVtools.Controls.Add(this.B_OpenMiscEditor);
            this.FLP_SAVtools.Controls.Add(this.B_OpenHoneyTreeEditor);
            this.FLP_SAVtools.Controls.Add(this.B_OpenFriendSafari);
            this.FLP_SAVtools.Controls.Add(this.B_OpenRTCEditor);
            this.FLP_SAVtools.Controls.Add(this.B_OpenUGSEditor);
            this.FLP_SAVtools.Controls.Add(this.B_Roamer);
            this.FLP_SAVtools.Controls.Add(this.B_FestivalPlaza);
            this.FLP_SAVtools.Controls.Add(this.B_MailBox);
            this.FLP_SAVtools.Controls.Add(this.B_OpenApricorn);
            this.FLP_SAVtools.Controls.Add(this.B_Raids);
            this.FLP_SAVtools.Controls.Add(this.B_Blocks);
            this.FLP_SAVtools.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FLP_SAVtools.Location = new System.Drawing.Point(3, 16);
            this.FLP_SAVtools.Name = "FLP_SAVtools";
            this.FLP_SAVtools.Size = new System.Drawing.Size(435, 143);
            this.FLP_SAVtools.TabIndex = 101;
            // 
            // B_OpenTrainerInfo
            // 
            this.B_OpenTrainerInfo.Location = new System.Drawing.Point(3, 3);
            this.B_OpenTrainerInfo.Name = "B_OpenTrainerInfo";
            this.B_OpenTrainerInfo.Size = new System.Drawing.Size(87, 23);
            this.B_OpenTrainerInfo.TabIndex = 1;
            this.B_OpenTrainerInfo.Text = "Trainer Info";
            this.B_OpenTrainerInfo.UseVisualStyleBackColor = true;
            this.B_OpenTrainerInfo.Click += new System.EventHandler(this.B_OpenTrainerInfo_Click);
            // 
            // B_OpenItemPouch
            // 
            this.B_OpenItemPouch.Location = new System.Drawing.Point(96, 3);
            this.B_OpenItemPouch.Name = "B_OpenItemPouch";
            this.B_OpenItemPouch.Size = new System.Drawing.Size(87, 23);
            this.B_OpenItemPouch.TabIndex = 1;
            this.B_OpenItemPouch.Text = "Items";
            this.B_OpenItemPouch.UseVisualStyleBackColor = true;
            this.B_OpenItemPouch.Click += new System.EventHandler(this.B_OpenItemPouch_Click);
            // 
            // B_OpenBoxLayout
            // 
            this.B_OpenBoxLayout.Location = new System.Drawing.Point(189, 3);
            this.B_OpenBoxLayout.Name = "B_OpenBoxLayout";
            this.B_OpenBoxLayout.Size = new System.Drawing.Size(87, 23);
            this.B_OpenBoxLayout.TabIndex = 1;
            this.B_OpenBoxLayout.Text = "Box Layout";
            this.B_OpenBoxLayout.UseVisualStyleBackColor = true;
            this.B_OpenBoxLayout.Click += new System.EventHandler(this.B_OpenBoxLayout_Click);
            // 
            // B_OpenWondercards
            // 
            this.B_OpenWondercards.Location = new System.Drawing.Point(282, 3);
            this.B_OpenWondercards.Name = "B_OpenWondercards";
            this.B_OpenWondercards.Size = new System.Drawing.Size(87, 23);
            this.B_OpenWondercards.TabIndex = 1;
            this.B_OpenWondercards.Text = "Wondercard";
            this.B_OpenWondercards.UseVisualStyleBackColor = true;
            this.B_OpenWondercards.Click += new System.EventHandler(this.B_OpenWondercards_Click);
            // 
            // B_OpenOPowers
            // 
            this.B_OpenOPowers.Location = new System.Drawing.Point(3, 32);
            this.B_OpenOPowers.Name = "B_OpenOPowers";
            this.B_OpenOPowers.Size = new System.Drawing.Size(87, 23);
            this.B_OpenOPowers.TabIndex = 1;
            this.B_OpenOPowers.Text = "O-Powers";
            this.B_OpenOPowers.UseVisualStyleBackColor = true;
            this.B_OpenOPowers.Click += new System.EventHandler(this.B_OpenOPowers_Click);
            // 
            // B_OpenEventFlags
            // 
            this.B_OpenEventFlags.Location = new System.Drawing.Point(96, 32);
            this.B_OpenEventFlags.Name = "B_OpenEventFlags";
            this.B_OpenEventFlags.Size = new System.Drawing.Size(87, 23);
            this.B_OpenEventFlags.TabIndex = 1;
            this.B_OpenEventFlags.Text = "Event Flags";
            this.B_OpenEventFlags.UseVisualStyleBackColor = true;
            this.B_OpenEventFlags.Click += new System.EventHandler(this.B_OpenEventFlags_Click);
            // 
            // B_OpenPokedex
            // 
            this.B_OpenPokedex.Location = new System.Drawing.Point(189, 32);
            this.B_OpenPokedex.Name = "B_OpenPokedex";
            this.B_OpenPokedex.Size = new System.Drawing.Size(87, 23);
            this.B_OpenPokedex.TabIndex = 1;
            this.B_OpenPokedex.Text = "Pokédex";
            this.B_OpenPokedex.UseVisualStyleBackColor = true;
            this.B_OpenPokedex.Click += new System.EventHandler(this.B_OpenPokedex_Click);
            // 
            // B_OpenLinkInfo
            // 
            this.B_OpenLinkInfo.Location = new System.Drawing.Point(282, 32);
            this.B_OpenLinkInfo.Name = "B_OpenLinkInfo";
            this.B_OpenLinkInfo.Size = new System.Drawing.Size(87, 23);
            this.B_OpenLinkInfo.TabIndex = 1;
            this.B_OpenLinkInfo.Text = "Link Data";
            this.B_OpenLinkInfo.UseVisualStyleBackColor = true;
            this.B_OpenLinkInfo.Click += new System.EventHandler(this.B_LinkInfo_Click);
            // 
            // B_OpenBerryField
            // 
            this.B_OpenBerryField.Location = new System.Drawing.Point(3, 61);
            this.B_OpenBerryField.Name = "B_OpenBerryField";
            this.B_OpenBerryField.Size = new System.Drawing.Size(87, 23);
            this.B_OpenBerryField.TabIndex = 1;
            this.B_OpenBerryField.Text = "Berry Field";
            this.B_OpenBerryField.UseVisualStyleBackColor = true;
            this.B_OpenBerryField.Click += new System.EventHandler(this.B_OpenBerryField_Click);
            // 
            // B_OpenPokeblocks
            // 
            this.B_OpenPokeblocks.Location = new System.Drawing.Point(96, 61);
            this.B_OpenPokeblocks.Name = "B_OpenPokeblocks";
            this.B_OpenPokeblocks.Size = new System.Drawing.Size(87, 23);
            this.B_OpenPokeblocks.TabIndex = 1;
            this.B_OpenPokeblocks.Text = "Pokéblocks";
            this.B_OpenPokeblocks.UseVisualStyleBackColor = true;
            this.B_OpenPokeblocks.Visible = false;
            this.B_OpenPokeblocks.Click += new System.EventHandler(this.B_OpenPokeblocks_Click);
            // 
            // B_OpenSecretBase
            // 
            this.B_OpenSecretBase.Location = new System.Drawing.Point(189, 61);
            this.B_OpenSecretBase.Name = "B_OpenSecretBase";
            this.B_OpenSecretBase.Size = new System.Drawing.Size(87, 23);
            this.B_OpenSecretBase.TabIndex = 1;
            this.B_OpenSecretBase.Text = "Secret Base";
            this.B_OpenSecretBase.UseVisualStyleBackColor = true;
            this.B_OpenSecretBase.Visible = false;
            this.B_OpenSecretBase.Click += new System.EventHandler(this.B_OpenSecretBase_Click);
            // 
            // B_OpenPokepuffs
            // 
            this.B_OpenPokepuffs.Location = new System.Drawing.Point(282, 61);
            this.B_OpenPokepuffs.Name = "B_OpenPokepuffs";
            this.B_OpenPokepuffs.Size = new System.Drawing.Size(87, 23);
            this.B_OpenPokepuffs.TabIndex = 1;
            this.B_OpenPokepuffs.Text = "‎Poké Puffs";
            this.B_OpenPokepuffs.UseVisualStyleBackColor = true;
            this.B_OpenPokepuffs.Click += new System.EventHandler(this.B_OpenPokepuffs_Click);
            // 
            // B_OpenSuperTraining
            // 
            this.B_OpenSuperTraining.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.B_OpenSuperTraining.Location = new System.Drawing.Point(3, 90);
            this.B_OpenSuperTraining.Name = "B_OpenSuperTraining";
            this.B_OpenSuperTraining.Size = new System.Drawing.Size(87, 23);
            this.B_OpenSuperTraining.TabIndex = 1;
            this.B_OpenSuperTraining.Text = "Super Train";
            this.B_OpenSuperTraining.UseVisualStyleBackColor = true;
            this.B_OpenSuperTraining.Click += new System.EventHandler(this.B_OpenSuperTraining_Click);
            // 
            // B_OpenHallofFame
            // 
            this.B_OpenHallofFame.Location = new System.Drawing.Point(96, 90);
            this.B_OpenHallofFame.Name = "B_OpenHallofFame";
            this.B_OpenHallofFame.Size = new System.Drawing.Size(87, 23);
            this.B_OpenHallofFame.TabIndex = 1;
            this.B_OpenHallofFame.Text = "Hall of Fame";
            this.B_OpenHallofFame.UseVisualStyleBackColor = true;
            this.B_OpenHallofFame.Click += new System.EventHandler(this.B_OUTHallofFame_Click);
            // 
            // B_OUTPasserby
            // 
            this.B_OUTPasserby.Location = new System.Drawing.Point(189, 90);
            this.B_OUTPasserby.Name = "B_OUTPasserby";
            this.B_OUTPasserby.Size = new System.Drawing.Size(87, 23);
            this.B_OUTPasserby.TabIndex = 1;
            this.B_OUTPasserby.Text = "Passerby";
            this.B_OUTPasserby.UseVisualStyleBackColor = true;
            this.B_OUTPasserby.Click += new System.EventHandler(this.B_OUTPasserby_Click);
            // 
            // B_CGearSkin
            // 
            this.B_CGearSkin.Location = new System.Drawing.Point(282, 90);
            this.B_CGearSkin.Name = "B_CGearSkin";
            this.B_CGearSkin.Size = new System.Drawing.Size(87, 23);
            this.B_CGearSkin.TabIndex = 1;
            this.B_CGearSkin.Text = "C-Gear Skin";
            this.B_CGearSkin.UseVisualStyleBackColor = true;
            this.B_CGearSkin.Click += new System.EventHandler(this.B_CGearSkin_Click);
            // 
            // B_OpenPokeBeans
            // 
            this.B_OpenPokeBeans.Location = new System.Drawing.Point(3, 119);
            this.B_OpenPokeBeans.Name = "B_OpenPokeBeans";
            this.B_OpenPokeBeans.Size = new System.Drawing.Size(87, 23);
            this.B_OpenPokeBeans.TabIndex = 1;
            this.B_OpenPokeBeans.Text = "‎Poké Beans";
            this.B_OpenPokeBeans.UseVisualStyleBackColor = true;
            this.B_OpenPokeBeans.Click += new System.EventHandler(this.B_OpenPokeBeans_Click);
            // 
            // B_CellsStickers
            // 
            this.B_CellsStickers.Location = new System.Drawing.Point(96, 119);
            this.B_CellsStickers.Name = "B_CellsStickers";
            this.B_CellsStickers.Size = new System.Drawing.Size(87, 23);
            this.B_CellsStickers.TabIndex = 1;
            this.B_CellsStickers.Text = "Cells/Stickers";
            this.B_CellsStickers.UseVisualStyleBackColor = true;
            this.B_CellsStickers.Click += new System.EventHandler(this.B_CellsStickers_Click);
            // 
            // B_OpenMiscEditor
            // 
            this.B_OpenMiscEditor.Location = new System.Drawing.Point(189, 119);
            this.B_OpenMiscEditor.Name = "B_OpenMiscEditor";
            this.B_OpenMiscEditor.Size = new System.Drawing.Size(87, 23);
            this.B_OpenMiscEditor.TabIndex = 1;
            this.B_OpenMiscEditor.Text = "Misc Edits";
            this.B_OpenMiscEditor.UseVisualStyleBackColor = true;
            this.B_OpenMiscEditor.Click += new System.EventHandler(this.B_OpenMiscEditor_Click);
            // 
            // B_OpenHoneyTreeEditor
            // 
            this.B_OpenHoneyTreeEditor.Location = new System.Drawing.Point(282, 119);
            this.B_OpenHoneyTreeEditor.Name = "B_OpenHoneyTreeEditor";
            this.B_OpenHoneyTreeEditor.Size = new System.Drawing.Size(87, 23);
            this.B_OpenHoneyTreeEditor.TabIndex = 1;
            this.B_OpenHoneyTreeEditor.Text = "Honey Tree";
            this.B_OpenHoneyTreeEditor.UseVisualStyleBackColor = true;
            this.B_OpenHoneyTreeEditor.Click += new System.EventHandler(this.B_OpenHoneyTreeEditor_Click);
            // 
            // B_OpenFriendSafari
            // 
            this.B_OpenFriendSafari.Location = new System.Drawing.Point(3, 148);
            this.B_OpenFriendSafari.Name = "B_OpenFriendSafari";
            this.B_OpenFriendSafari.Size = new System.Drawing.Size(87, 23);
            this.B_OpenFriendSafari.TabIndex = 1;
            this.B_OpenFriendSafari.Text = "Friend Safari";
            this.B_OpenFriendSafari.UseVisualStyleBackColor = true;
            this.B_OpenFriendSafari.Click += new System.EventHandler(this.B_OpenFriendSafari_Click);
            // 
            // B_OpenRTCEditor
            // 
            this.B_OpenRTCEditor.Location = new System.Drawing.Point(96, 148);
            this.B_OpenRTCEditor.Name = "B_OpenRTCEditor";
            this.B_OpenRTCEditor.Size = new System.Drawing.Size(87, 23);
            this.B_OpenRTCEditor.TabIndex = 1;
            this.B_OpenRTCEditor.Text = "Clock (RTC)";
            this.B_OpenRTCEditor.UseVisualStyleBackColor = true;
            this.B_OpenRTCEditor.Click += new System.EventHandler(this.B_OpenRTCEditor_Click);
            // 
            // B_OpenUGSEditor
            // 
            this.B_OpenUGSEditor.Location = new System.Drawing.Point(189, 148);
            this.B_OpenUGSEditor.Name = "B_OpenUGSEditor";
            this.B_OpenUGSEditor.Size = new System.Drawing.Size(87, 23);
            this.B_OpenUGSEditor.TabIndex = 1;
            this.B_OpenUGSEditor.Text = "Underground";
            this.B_OpenUGSEditor.UseVisualStyleBackColor = true;
            this.B_OpenUGSEditor.Click += new System.EventHandler(this.B_OpenUGSEditor_Click);
            // 
            // B_Roamer
            // 
            this.B_Roamer.Location = new System.Drawing.Point(282, 148);
            this.B_Roamer.Name = "B_Roamer";
            this.B_Roamer.Size = new System.Drawing.Size(87, 23);
            this.B_Roamer.TabIndex = 1;
            this.B_Roamer.Text = "Roamer";
            this.B_Roamer.UseVisualStyleBackColor = true;
            this.B_Roamer.Click += new System.EventHandler(this.B_Roamer_Click);
            // 
            // B_FestivalPlaza
            // 
            this.B_FestivalPlaza.Location = new System.Drawing.Point(3, 177);
            this.B_FestivalPlaza.Name = "B_FestivalPlaza";
            this.B_FestivalPlaza.Size = new System.Drawing.Size(87, 23);
            this.B_FestivalPlaza.TabIndex = 1;
            this.B_FestivalPlaza.Text = "Festival Plaza";
            this.B_FestivalPlaza.UseVisualStyleBackColor = true;
            this.B_FestivalPlaza.Click += new System.EventHandler(this.B_FestivalPlaza_Click);
            // 
            // B_MailBox
            // 
            this.B_MailBox.Location = new System.Drawing.Point(96, 177);
            this.B_MailBox.Name = "B_MailBox";
            this.B_MailBox.Size = new System.Drawing.Size(87, 23);
            this.B_MailBox.TabIndex = 1;
            this.B_MailBox.Text = "Mail Box";
            this.B_MailBox.UseVisualStyleBackColor = true;
            this.B_MailBox.Click += new System.EventHandler(this.B_MailBox_Click);
            // 
            // B_OpenApricorn
            // 
            this.B_OpenApricorn.Location = new System.Drawing.Point(189, 177);
            this.B_OpenApricorn.Name = "B_OpenApricorn";
            this.B_OpenApricorn.Size = new System.Drawing.Size(87, 23);
            this.B_OpenApricorn.TabIndex = 1;
            this.B_OpenApricorn.Text = "Apricorns";
            this.B_OpenApricorn.UseVisualStyleBackColor = true;
            this.B_OpenApricorn.Click += new System.EventHandler(this.B_OpenApricorn_Click);
            // 
            // B_Raids
            // 
            this.B_Raids.Location = new System.Drawing.Point(282, 177);
            this.B_Raids.Name = "B_Raids";
            this.B_Raids.Size = new System.Drawing.Size(87, 23);
            this.B_Raids.TabIndex = 1;
            this.B_Raids.Text = "Raids";
            this.B_Raids.UseVisualStyleBackColor = true;
            this.B_Raids.Click += new System.EventHandler(this.B_OpenRaids_Click);
            // 
            // B_Blocks
            // 
            this.B_Blocks.Location = new System.Drawing.Point(3, 206);
            this.B_Blocks.Name = "B_Blocks";
            this.B_Blocks.Size = new System.Drawing.Size(87, 23);
            this.B_Blocks.TabIndex = 1;
            this.B_Blocks.Text = "Block Data";
            this.B_Blocks.UseVisualStyleBackColor = true;
            this.B_Blocks.Click += new System.EventHandler(this.B_Blocks_Click);
            // 
            // L_SaveSlot
            // 
            this.L_SaveSlot.AutoSize = true;
            this.L_SaveSlot.Location = new System.Drawing.Point(92, 151);
            this.L_SaveSlot.Name = "L_SaveSlot";
            this.L_SaveSlot.Size = new System.Drawing.Size(56, 13);
            this.L_SaveSlot.TabIndex = 19;
            this.L_SaveSlot.Text = "Save Slot:";
            // 
            // L_Secure2
            // 
            this.L_Secure2.Location = new System.Drawing.Point(33, 113);
            this.L_Secure2.Name = "L_Secure2";
            this.L_Secure2.Size = new System.Drawing.Size(115, 20);
            this.L_Secure2.TabIndex = 18;
            this.L_Secure2.Text = "Secure Value 2:";
            this.L_Secure2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TB_Secure2
            // 
            this.TB_Secure2.Enabled = false;
            this.TB_Secure2.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_Secure2.Location = new System.Drawing.Point(151, 113);
            this.TB_Secure2.MaxLength = 16;
            this.TB_Secure2.Name = "TB_Secure2";
            this.TB_Secure2.Size = new System.Drawing.Size(120, 20);
            this.TB_Secure2.TabIndex = 17;
            this.TB_Secure2.Text = "0000000000000000";
            this.TB_Secure2.Validated += new System.EventHandler(this.UpdateStringSeed);
            // 
            // L_Secure1
            // 
            this.L_Secure1.Location = new System.Drawing.Point(33, 91);
            this.L_Secure1.Name = "L_Secure1";
            this.L_Secure1.Size = new System.Drawing.Size(115, 20);
            this.L_Secure1.TabIndex = 16;
            this.L_Secure1.Text = "Secure Value 1:";
            this.L_Secure1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TB_Secure1
            // 
            this.TB_Secure1.Enabled = false;
            this.TB_Secure1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_Secure1.Location = new System.Drawing.Point(151, 91);
            this.TB_Secure1.MaxLength = 16;
            this.TB_Secure1.Name = "TB_Secure1";
            this.TB_Secure1.Size = new System.Drawing.Size(120, 20);
            this.TB_Secure1.TabIndex = 15;
            this.TB_Secure1.Text = "0000000000000000";
            this.TB_Secure1.Validated += new System.EventHandler(this.UpdateStringSeed);
            // 
            // B_JPEG
            // 
            this.B_JPEG.Location = new System.Drawing.Point(198, 20);
            this.B_JPEG.Name = "B_JPEG";
            this.B_JPEG.Size = new System.Drawing.Size(75, 45);
            this.B_JPEG.TabIndex = 12;
            this.B_JPEG.Text = "Save PGL .JPEG";
            this.B_JPEG.UseVisualStyleBackColor = true;
            this.B_JPEG.Click += new System.EventHandler(this.B_JPEG_Click);
            // 
            // L_GameSync
            // 
            this.L_GameSync.Location = new System.Drawing.Point(33, 69);
            this.L_GameSync.Name = "L_GameSync";
            this.L_GameSync.Size = new System.Drawing.Size(115, 20);
            this.L_GameSync.TabIndex = 11;
            this.L_GameSync.Text = "Game Sync ID:";
            this.L_GameSync.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TB_GameSync
            // 
            this.TB_GameSync.Enabled = false;
            this.TB_GameSync.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_GameSync.Location = new System.Drawing.Point(151, 69);
            this.TB_GameSync.MaxLength = 16;
            this.TB_GameSync.Name = "TB_GameSync";
            this.TB_GameSync.Size = new System.Drawing.Size(120, 20);
            this.TB_GameSync.TabIndex = 10;
            this.TB_GameSync.Text = "0000000000000000";
            this.TB_GameSync.Validated += new System.EventHandler(this.UpdateStringSeed);
            // 
            // B_SaveBoxBin
            // 
            this.B_SaveBoxBin.Location = new System.Drawing.Point(116, 20);
            this.B_SaveBoxBin.Name = "B_SaveBoxBin";
            this.B_SaveBoxBin.Size = new System.Drawing.Size(75, 45);
            this.B_SaveBoxBin.TabIndex = 8;
            this.B_SaveBoxBin.Text = "Save Box Data++";
            this.B_SaveBoxBin.UseVisualStyleBackColor = true;
            this.B_SaveBoxBin.Click += new System.EventHandler(this.B_SaveBoxBin_Click);
            // 
            // B_VerifyCHK
            // 
            this.B_VerifyCHK.Enabled = false;
            this.B_VerifyCHK.Location = new System.Drawing.Point(32, 20);
            this.B_VerifyCHK.Name = "B_VerifyCHK";
            this.B_VerifyCHK.Size = new System.Drawing.Size(75, 45);
            this.B_VerifyCHK.TabIndex = 2;
            this.B_VerifyCHK.Text = "Verify Checksums";
            this.B_VerifyCHK.UseVisualStyleBackColor = true;
            this.B_VerifyCHK.Click += new System.EventHandler(this.ClickVerifyCHK);
            // 
            // SAVEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.tabBoxMulti);
            this.Name = "SAVEditor";
            this.Size = new System.Drawing.Size(449, 363);
            this.tabBoxMulti.ResumeLayout(false);
            this.Tab_Box.ResumeLayout(false);
            this.Tab_Box.PerformLayout();
            this.Tab_PartyBattle.ResumeLayout(false);
            this.Tab_PartyBattle.PerformLayout();
            this.Tab_Other.ResumeLayout(false);
            this.GB_Daycare.ResumeLayout(false);
            this.GB_Daycare.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dcpkx2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dcpkx1)).EndInit();
            this.Tab_SAV.ResumeLayout(false);
            this.Tab_SAV.PerformLayout();
            this.GB_SAVtools.ResumeLayout(false);
            this.FLP_SAVtools.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabBoxMulti;
        private System.Windows.Forms.TabPage Tab_Box;
        private System.Windows.Forms.TabPage Tab_PartyBattle;
        private System.Windows.Forms.TabPage Tab_Other;
        private System.Windows.Forms.GroupBox GB_Daycare;
        private System.Windows.Forms.Label L_XP2;
        private System.Windows.Forms.Label L_XP1;
        private System.Windows.Forms.TextBox TB_Daycare2XP;
        private System.Windows.Forms.TextBox TB_Daycare1XP;
        private System.Windows.Forms.Label L_DC2;
        private System.Windows.Forms.Label L_DC1;
        private System.Windows.Forms.Label L_DaycareSeed;
        private System.Windows.Forms.TextBox TB_RNGSeed;
        private System.Windows.Forms.PictureBox dcpkx2;
        private System.Windows.Forms.PictureBox dcpkx1;
        private System.Windows.Forms.CheckBox DayCare_HasEgg;
        private System.Windows.Forms.Label L_ReadOnlyOther;
        private System.Windows.Forms.TabPage Tab_SAV;
        private System.Windows.Forms.ComboBox CB_SaveSlot;
        private System.Windows.Forms.Label L_SaveSlot;
        private System.Windows.Forms.Label L_Secure2;
        private System.Windows.Forms.TextBox TB_Secure2;
        private System.Windows.Forms.Label L_Secure1;
        private System.Windows.Forms.TextBox TB_Secure1;
        private System.Windows.Forms.Button B_JPEG;
        private System.Windows.Forms.Label L_GameSync;
        private System.Windows.Forms.TextBox TB_GameSync;
        private System.Windows.Forms.Button B_SaveBoxBin;
        private System.Windows.Forms.Button B_VerifyCHK;
        private System.Windows.Forms.GroupBox GB_SAVtools;
        private System.Windows.Forms.FlowLayoutPanel FLP_SAVtools;
        private System.Windows.Forms.Button B_OpenTrainerInfo;
        private System.Windows.Forms.Button B_OpenItemPouch;
        private System.Windows.Forms.Button B_OpenBoxLayout;
        private System.Windows.Forms.Button B_OpenWondercards;
        private System.Windows.Forms.Button B_OpenOPowers;
        private System.Windows.Forms.Button B_OpenEventFlags;
        private System.Windows.Forms.Button B_OpenPokedex;
        private System.Windows.Forms.Button B_OpenLinkInfo;
        private System.Windows.Forms.Button B_OpenBerryField;
        private System.Windows.Forms.Button B_OpenPokeblocks;
        private System.Windows.Forms.Button B_OpenSecretBase;
        private System.Windows.Forms.Button B_OpenPokepuffs;
        private System.Windows.Forms.Button B_OpenSuperTraining;
        private System.Windows.Forms.Button B_OpenHallofFame;
        private System.Windows.Forms.Button B_OUTPasserby;
        private System.Windows.Forms.Button B_CGearSkin;
        private System.Windows.Forms.Button B_OpenPokeBeans;
        private System.Windows.Forms.Button B_CellsStickers;
        private System.Windows.Forms.Button B_OpenMiscEditor;
        private System.Windows.Forms.Button B_OpenHoneyTreeEditor;
        private System.Windows.Forms.Button B_OpenFriendSafari;
        private System.Windows.Forms.Button B_OpenRTCEditor;
        public BoxEditor Box;
        private System.Windows.Forms.Button B_OpenUGSEditor;
        private System.Windows.Forms.Button B_Roamer;
        private System.Windows.Forms.Button B_FestivalPlaza;
        private System.Windows.Forms.Button B_MailBox;
        private System.Windows.Forms.Button B_OpenApricorn;
        private SlotList SL_Extra;
        private PartyEditor SL_Party;
        private System.Windows.Forms.Button B_Raids;
        private System.Windows.Forms.Button B_Blocks;
    }
}
