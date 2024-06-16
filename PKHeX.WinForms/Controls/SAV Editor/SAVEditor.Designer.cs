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
            Menu_Undo = null;
            Menu_Redo = null;
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabBoxMulti = new System.Windows.Forms.TabControl();
            Tab_Box = new System.Windows.Forms.TabPage();
            Box = new BoxEditor();
            Tab_PartyBattle = new System.Windows.Forms.TabPage();
            SL_Party = new PartyEditor();
            Tab_Other = new System.Windows.Forms.TabPage();
            SL_Extra = new SlotList();
            GB_Daycare = new System.Windows.Forms.GroupBox();
            L_XP2 = new System.Windows.Forms.Label();
            L_XP1 = new System.Windows.Forms.Label();
            TB_Daycare2XP = new System.Windows.Forms.TextBox();
            TB_Daycare1XP = new System.Windows.Forms.TextBox();
            L_DC2 = new System.Windows.Forms.Label();
            L_DC1 = new System.Windows.Forms.Label();
            L_DaycareSeed = new System.Windows.Forms.Label();
            TB_RNGSeed = new System.Windows.Forms.TextBox();
            dcpkx2 = new System.Windows.Forms.PictureBox();
            dcpkx1 = new System.Windows.Forms.PictureBox();
            DayCare_HasEgg = new System.Windows.Forms.CheckBox();
            L_ReadOnlyOther = new System.Windows.Forms.Label();
            Tab_SAV = new System.Windows.Forms.TabPage();
            FLP_SAVtools = new System.Windows.Forms.FlowLayoutPanel();
            B_OpenTrainerInfo = new System.Windows.Forms.Button();
            B_OpenItemPouch = new System.Windows.Forms.Button();
            B_OpenBoxLayout = new System.Windows.Forms.Button();
            B_OpenWondercards = new System.Windows.Forms.Button();
            B_OpenOPowers = new System.Windows.Forms.Button();
            B_OpenEventFlags = new System.Windows.Forms.Button();
            B_OpenPokedex = new System.Windows.Forms.Button();
            B_OpenLinkInfo = new System.Windows.Forms.Button();
            B_OpenBerryField = new System.Windows.Forms.Button();
            B_OpenPokeblocks = new System.Windows.Forms.Button();
            B_OpenSecretBase = new System.Windows.Forms.Button();
            B_OpenPokepuffs = new System.Windows.Forms.Button();
            B_OpenSuperTraining = new System.Windows.Forms.Button();
            B_OpenHallofFame = new System.Windows.Forms.Button();
            B_OUTPasserby = new System.Windows.Forms.Button();
            B_CGearSkin = new System.Windows.Forms.Button();
            B_OpenPokeBeans = new System.Windows.Forms.Button();
            B_CellsStickers = new System.Windows.Forms.Button();
            B_OpenMiscEditor = new System.Windows.Forms.Button();
            B_OpenHoneyTreeEditor = new System.Windows.Forms.Button();
            B_OpenFriendSafari = new System.Windows.Forms.Button();
            B_OpenRTCEditor = new System.Windows.Forms.Button();
            B_OpenUGSEditor = new System.Windows.Forms.Button();
            B_OpenGeonetEditor = new System.Windows.Forms.Button();
            B_OpenUnityTowerEditor = new System.Windows.Forms.Button();
            B_OpenChatterEditor = new System.Windows.Forms.Button();
            B_Roamer = new System.Windows.Forms.Button();
            B_FestivalPlaza = new System.Windows.Forms.Button();
            B_MailBox = new System.Windows.Forms.Button();
            B_OpenApricorn = new System.Windows.Forms.Button();
            B_Raids = new System.Windows.Forms.Button();
            B_RaidsDLC1 = new System.Windows.Forms.Button();
            B_RaidsDLC2 = new System.Windows.Forms.Button();
            B_Blocks = new System.Windows.Forms.Button();
            B_OtherSlots = new System.Windows.Forms.Button();
            B_OpenSealStickers = new System.Windows.Forms.Button();
            B_Poffins = new System.Windows.Forms.Button();
            B_RaidsSevenStar = new System.Windows.Forms.Button();
            FLP_SAVToolsMisc = new System.Windows.Forms.FlowLayoutPanel();
            B_SaveBoxBin = new System.Windows.Forms.Button();
            B_VerifyCHK = new System.Windows.Forms.Button();
            B_VerifySaveEntities = new System.Windows.Forms.Button();
            Menu_ExportBAK = new System.Windows.Forms.Button();
            B_JPEG = new System.Windows.Forms.Button();
            B_ConvertKorean = new System.Windows.Forms.Button();
            CB_SaveSlot = new System.Windows.Forms.ComboBox();
            L_SaveSlot = new System.Windows.Forms.Label();
            L_Secure2 = new System.Windows.Forms.Label();
            TB_Secure2 = new System.Windows.Forms.TextBox();
            L_Secure1 = new System.Windows.Forms.Label();
            TB_Secure1 = new System.Windows.Forms.TextBox();
            L_GameSync = new System.Windows.Forms.Label();
            TB_GameSync = new System.Windows.Forms.TextBox();
            tabBoxMulti.SuspendLayout();
            Tab_Box.SuspendLayout();
            Tab_PartyBattle.SuspendLayout();
            Tab_Other.SuspendLayout();
            GB_Daycare.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dcpkx2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dcpkx1).BeginInit();
            Tab_SAV.SuspendLayout();
            FLP_SAVtools.SuspendLayout();
            FLP_SAVToolsMisc.SuspendLayout();
            SuspendLayout();
            // 
            // tabBoxMulti
            // 
            tabBoxMulti.AllowDrop = true;
            tabBoxMulti.Controls.Add(Tab_Box);
            tabBoxMulti.Controls.Add(Tab_PartyBattle);
            tabBoxMulti.Controls.Add(Tab_Other);
            tabBoxMulti.Controls.Add(Tab_SAV);
            tabBoxMulti.Dock = System.Windows.Forms.DockStyle.Fill;
            tabBoxMulti.Location = new System.Drawing.Point(0, 0);
            tabBoxMulti.Name = "tabBoxMulti";
            tabBoxMulti.SelectedIndex = 0;
            tabBoxMulti.Size = new System.Drawing.Size(449, 363);
            tabBoxMulti.TabIndex = 101;
            tabBoxMulti.DragOver += MultiDragOver;
            tabBoxMulti.MouseClick += ClickBoxSort;
            tabBoxMulti.MouseDoubleClick += ClickBoxDouble;
            tabBoxMulti.MouseDown += TabMouseDown;
            tabBoxMulti.MouseMove += TabMouseMove;
            tabBoxMulti.MouseUp += TabMouseUp;
            // 
            // Tab_Box
            // 
            Tab_Box.AllowDrop = true;
            Tab_Box.Controls.Add(Box);
            Tab_Box.Location = new System.Drawing.Point(4, 24);
            Tab_Box.Name = "Tab_Box";
            Tab_Box.Padding = new System.Windows.Forms.Padding(3);
            Tab_Box.Size = new System.Drawing.Size(441, 335);
            Tab_Box.TabIndex = 0;
            Tab_Box.Text = "Box";
            Tab_Box.UseVisualStyleBackColor = true;
            // 
            // Box
            // 
            Box.AllowDrop = true;
            Box.AutoSize = true;
            Box.CanSetCurrentBox = true;
            Box.ControlsEnabled = true;
            Box.ControlsVisible = true;
            Box.CurrentBox = -1;
            Box.Editor = null;
            Box.FlagIllegal = false;
            Box.Location = new System.Drawing.Point(99, 7);
            Box.M = null;
            Box.Name = "Box";
            Box.Size = new System.Drawing.Size(251, 185);
            Box.TabIndex = 1;
            // 
            // Tab_PartyBattle
            // 
            Tab_PartyBattle.AllowDrop = true;
            Tab_PartyBattle.Controls.Add(SL_Party);
            Tab_PartyBattle.Location = new System.Drawing.Point(4, 24);
            Tab_PartyBattle.Name = "Tab_PartyBattle";
            Tab_PartyBattle.Padding = new System.Windows.Forms.Padding(3);
            Tab_PartyBattle.Size = new System.Drawing.Size(441, 335);
            Tab_PartyBattle.TabIndex = 1;
            Tab_PartyBattle.Text = "Party";
            Tab_PartyBattle.UseVisualStyleBackColor = true;
            // 
            // SL_Party
            // 
            SL_Party.AutoSize = true;
            SL_Party.FlagIllegal = false;
            SL_Party.Location = new System.Drawing.Point(8, 8);
            SL_Party.M = null;
            SL_Party.Name = "SL_Party";
            SL_Party.Size = new System.Drawing.Size(256, 184);
            SL_Party.TabIndex = 0;
            // 
            // Tab_Other
            // 
            Tab_Other.Controls.Add(SL_Extra);
            Tab_Other.Controls.Add(GB_Daycare);
            Tab_Other.Controls.Add(L_ReadOnlyOther);
            Tab_Other.Location = new System.Drawing.Point(4, 24);
            Tab_Other.Name = "Tab_Other";
            Tab_Other.Size = new System.Drawing.Size(441, 335);
            Tab_Other.TabIndex = 2;
            Tab_Other.Text = "Other";
            Tab_Other.UseVisualStyleBackColor = true;
            // 
            // SL_Extra
            // 
            SL_Extra.Dock = System.Windows.Forms.DockStyle.Right;
            SL_Extra.FlagIllegal = false;
            SL_Extra.Location = new System.Drawing.Point(337, 0);
            SL_Extra.Name = "SL_Extra";
            SL_Extra.SAV = null;
            SL_Extra.Size = new System.Drawing.Size(104, 335);
            SL_Extra.TabIndex = 30;
            SL_Extra.ViewIndex = -1;
            // 
            // GB_Daycare
            // 
            GB_Daycare.Controls.Add(L_XP2);
            GB_Daycare.Controls.Add(L_XP1);
            GB_Daycare.Controls.Add(TB_Daycare2XP);
            GB_Daycare.Controls.Add(TB_Daycare1XP);
            GB_Daycare.Controls.Add(L_DC2);
            GB_Daycare.Controls.Add(L_DC1);
            GB_Daycare.Controls.Add(L_DaycareSeed);
            GB_Daycare.Controls.Add(TB_RNGSeed);
            GB_Daycare.Controls.Add(dcpkx2);
            GB_Daycare.Controls.Add(dcpkx1);
            GB_Daycare.Controls.Add(DayCare_HasEgg);
            GB_Daycare.Location = new System.Drawing.Point(16, 8);
            GB_Daycare.Name = "GB_Daycare";
            GB_Daycare.Size = new System.Drawing.Size(200, 196);
            GB_Daycare.TabIndex = 28;
            GB_Daycare.TabStop = false;
            GB_Daycare.Text = "Daycare";
            // 
            // L_XP2
            // 
            L_XP2.Location = new System.Drawing.Point(80, 104);
            L_XP2.Name = "L_XP2";
            L_XP2.Size = new System.Drawing.Size(40, 24);
            L_XP2.TabIndex = 17;
            L_XP2.Text = "+XP:";
            L_XP2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // L_XP1
            // 
            L_XP1.Location = new System.Drawing.Point(80, 40);
            L_XP1.Name = "L_XP1";
            L_XP1.Size = new System.Drawing.Size(40, 24);
            L_XP1.TabIndex = 16;
            L_XP1.Text = "+XP:";
            L_XP1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TB_Daycare2XP
            // 
            TB_Daycare2XP.Location = new System.Drawing.Point(120, 104);
            TB_Daycare2XP.Name = "TB_Daycare2XP";
            TB_Daycare2XP.ReadOnly = true;
            TB_Daycare2XP.Size = new System.Drawing.Size(73, 23);
            TB_Daycare2XP.TabIndex = 15;
            // 
            // TB_Daycare1XP
            // 
            TB_Daycare1XP.Location = new System.Drawing.Point(120, 40);
            TB_Daycare1XP.Name = "TB_Daycare1XP";
            TB_Daycare1XP.ReadOnly = true;
            TB_Daycare1XP.Size = new System.Drawing.Size(73, 23);
            TB_Daycare1XP.TabIndex = 14;
            // 
            // L_DC2
            // 
            L_DC2.AutoSize = true;
            L_DC2.Location = new System.Drawing.Point(80, 88);
            L_DC2.Name = "L_DC2";
            L_DC2.Size = new System.Drawing.Size(19, 15);
            L_DC2.TabIndex = 13;
            L_DC2.Text = "2: ";
            // 
            // L_DC1
            // 
            L_DC1.AutoSize = true;
            L_DC1.Location = new System.Drawing.Point(80, 24);
            L_DC1.Name = "L_DC1";
            L_DC1.Size = new System.Drawing.Size(19, 15);
            L_DC1.TabIndex = 12;
            L_DC1.Text = "1: ";
            // 
            // L_DaycareSeed
            // 
            L_DaycareSeed.Location = new System.Drawing.Point(16, 168);
            L_DaycareSeed.Name = "L_DaycareSeed";
            L_DaycareSeed.Size = new System.Drawing.Size(48, 24);
            L_DaycareSeed.TabIndex = 9;
            L_DaycareSeed.Text = "Seed:";
            L_DaycareSeed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TB_RNGSeed
            // 
            TB_RNGSeed.Font = new System.Drawing.Font("Courier New", 8.25F);
            TB_RNGSeed.Location = new System.Drawing.Point(64, 168);
            TB_RNGSeed.MaxLength = 16;
            TB_RNGSeed.Name = "TB_RNGSeed";
            TB_RNGSeed.PlaceholderText = "0123456789ABCDEF";
            TB_RNGSeed.Size = new System.Drawing.Size(120, 20);
            TB_RNGSeed.TabIndex = 8;
            TB_RNGSeed.Validated += UpdateStringSeed;
            // 
            // dcpkx2
            // 
            dcpkx2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            dcpkx2.Location = new System.Drawing.Point(8, 80);
            dcpkx2.Name = "dcpkx2";
            dcpkx2.Size = new System.Drawing.Size(70, 58);
            dcpkx2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            dcpkx2.TabIndex = 11;
            dcpkx2.TabStop = false;
            // 
            // dcpkx1
            // 
            dcpkx1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            dcpkx1.Location = new System.Drawing.Point(8, 16);
            dcpkx1.Name = "dcpkx1";
            dcpkx1.Size = new System.Drawing.Size(70, 58);
            dcpkx1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            dcpkx1.TabIndex = 10;
            dcpkx1.TabStop = false;
            // 
            // DayCare_HasEgg
            // 
            DayCare_HasEgg.AutoSize = true;
            DayCare_HasEgg.Enabled = false;
            DayCare_HasEgg.Location = new System.Drawing.Point(64, 146);
            DayCare_HasEgg.Name = "DayCare_HasEgg";
            DayCare_HasEgg.Size = new System.Drawing.Size(97, 19);
            DayCare_HasEgg.TabIndex = 7;
            DayCare_HasEgg.Text = "Egg Available";
            DayCare_HasEgg.UseVisualStyleBackColor = true;
            // 
            // L_ReadOnlyOther
            // 
            L_ReadOnlyOther.ForeColor = System.Drawing.Color.Red;
            L_ReadOnlyOther.Location = new System.Drawing.Point(32, 208);
            L_ReadOnlyOther.Name = "L_ReadOnlyOther";
            L_ReadOnlyOther.Size = new System.Drawing.Size(176, 24);
            L_ReadOnlyOther.TabIndex = 29;
            L_ReadOnlyOther.Text = "This tab is read only.";
            L_ReadOnlyOther.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Tab_SAV
            // 
            Tab_SAV.Controls.Add(FLP_SAVtools);
            Tab_SAV.Controls.Add(FLP_SAVToolsMisc);
            Tab_SAV.Controls.Add(CB_SaveSlot);
            Tab_SAV.Controls.Add(L_SaveSlot);
            Tab_SAV.Controls.Add(L_Secure2);
            Tab_SAV.Controls.Add(TB_Secure2);
            Tab_SAV.Controls.Add(L_Secure1);
            Tab_SAV.Controls.Add(TB_Secure1);
            Tab_SAV.Controls.Add(L_GameSync);
            Tab_SAV.Controls.Add(TB_GameSync);
            Tab_SAV.Location = new System.Drawing.Point(4, 24);
            Tab_SAV.Name = "Tab_SAV";
            Tab_SAV.Size = new System.Drawing.Size(441, 335);
            Tab_SAV.TabIndex = 3;
            Tab_SAV.Text = "SAV";
            Tab_SAV.UseVisualStyleBackColor = true;
            // 
            // FLP_SAVtools
            // 
            FLP_SAVtools.AutoScroll = true;
            FLP_SAVtools.Controls.Add(B_OpenTrainerInfo);
            FLP_SAVtools.Controls.Add(B_OpenItemPouch);
            FLP_SAVtools.Controls.Add(B_OpenBoxLayout);
            FLP_SAVtools.Controls.Add(B_OpenWondercards);
            FLP_SAVtools.Controls.Add(B_OpenOPowers);
            FLP_SAVtools.Controls.Add(B_OpenEventFlags);
            FLP_SAVtools.Controls.Add(B_OpenPokedex);
            FLP_SAVtools.Controls.Add(B_OpenLinkInfo);
            FLP_SAVtools.Controls.Add(B_OpenBerryField);
            FLP_SAVtools.Controls.Add(B_OpenPokeblocks);
            FLP_SAVtools.Controls.Add(B_OpenSecretBase);
            FLP_SAVtools.Controls.Add(B_OpenPokepuffs);
            FLP_SAVtools.Controls.Add(B_OpenSuperTraining);
            FLP_SAVtools.Controls.Add(B_OpenHallofFame);
            FLP_SAVtools.Controls.Add(B_OUTPasserby);
            FLP_SAVtools.Controls.Add(B_CGearSkin);
            FLP_SAVtools.Controls.Add(B_OpenPokeBeans);
            FLP_SAVtools.Controls.Add(B_CellsStickers);
            FLP_SAVtools.Controls.Add(B_OpenMiscEditor);
            FLP_SAVtools.Controls.Add(B_OpenHoneyTreeEditor);
            FLP_SAVtools.Controls.Add(B_OpenFriendSafari);
            FLP_SAVtools.Controls.Add(B_OpenRTCEditor);
            FLP_SAVtools.Controls.Add(B_OpenUGSEditor);
            FLP_SAVtools.Controls.Add(B_OpenGeonetEditor);
            FLP_SAVtools.Controls.Add(B_OpenUnityTowerEditor);
            FLP_SAVtools.Controls.Add(B_OpenChatterEditor);
            FLP_SAVtools.Controls.Add(B_Roamer);
            FLP_SAVtools.Controls.Add(B_FestivalPlaza);
            FLP_SAVtools.Controls.Add(B_MailBox);
            FLP_SAVtools.Controls.Add(B_OpenApricorn);
            FLP_SAVtools.Controls.Add(B_Raids);
            FLP_SAVtools.Controls.Add(B_RaidsDLC1);
            FLP_SAVtools.Controls.Add(B_RaidsDLC2);
            FLP_SAVtools.Controls.Add(B_Blocks);
            FLP_SAVtools.Controls.Add(B_OtherSlots);
            FLP_SAVtools.Controls.Add(B_OpenSealStickers);
            FLP_SAVtools.Controls.Add(B_Poffins);
            FLP_SAVtools.Controls.Add(B_RaidsSevenStar);
            FLP_SAVtools.Dock = System.Windows.Forms.DockStyle.Bottom;
            FLP_SAVtools.Location = new System.Drawing.Point(0, 175);
            FLP_SAVtools.Margin = new System.Windows.Forms.Padding(0);
            FLP_SAVtools.Name = "FLP_SAVtools";
            FLP_SAVtools.Size = new System.Drawing.Size(441, 160);
            FLP_SAVtools.TabIndex = 101;
            // 
            // B_OpenTrainerInfo
            // 
            B_OpenTrainerInfo.Location = new System.Drawing.Point(4, 4);
            B_OpenTrainerInfo.Margin = new System.Windows.Forms.Padding(4);
            B_OpenTrainerInfo.Name = "B_OpenTrainerInfo";
            B_OpenTrainerInfo.Size = new System.Drawing.Size(96, 32);
            B_OpenTrainerInfo.TabIndex = 1;
            B_OpenTrainerInfo.Text = "Trainer Info";
            B_OpenTrainerInfo.UseVisualStyleBackColor = true;
            B_OpenTrainerInfo.Click += B_OpenTrainerInfo_Click;
            // 
            // B_OpenItemPouch
            // 
            B_OpenItemPouch.Location = new System.Drawing.Point(108, 4);
            B_OpenItemPouch.Margin = new System.Windows.Forms.Padding(4);
            B_OpenItemPouch.Name = "B_OpenItemPouch";
            B_OpenItemPouch.Size = new System.Drawing.Size(96, 32);
            B_OpenItemPouch.TabIndex = 1;
            B_OpenItemPouch.Text = "Items";
            B_OpenItemPouch.UseVisualStyleBackColor = true;
            B_OpenItemPouch.Click += B_OpenItemPouch_Click;
            // 
            // B_OpenBoxLayout
            // 
            B_OpenBoxLayout.Location = new System.Drawing.Point(212, 4);
            B_OpenBoxLayout.Margin = new System.Windows.Forms.Padding(4);
            B_OpenBoxLayout.Name = "B_OpenBoxLayout";
            B_OpenBoxLayout.Size = new System.Drawing.Size(96, 32);
            B_OpenBoxLayout.TabIndex = 1;
            B_OpenBoxLayout.Text = "Box Layout";
            B_OpenBoxLayout.UseVisualStyleBackColor = true;
            B_OpenBoxLayout.Click += B_OpenBoxLayout_Click;
            // 
            // B_OpenWondercards
            // 
            B_OpenWondercards.Location = new System.Drawing.Point(316, 4);
            B_OpenWondercards.Margin = new System.Windows.Forms.Padding(4);
            B_OpenWondercards.Name = "B_OpenWondercards";
            B_OpenWondercards.Size = new System.Drawing.Size(96, 32);
            B_OpenWondercards.TabIndex = 1;
            B_OpenWondercards.Text = "Wondercard";
            B_OpenWondercards.UseVisualStyleBackColor = true;
            B_OpenWondercards.Click += B_OpenWondercards_Click;
            // 
            // B_OpenOPowers
            // 
            B_OpenOPowers.Location = new System.Drawing.Point(4, 44);
            B_OpenOPowers.Margin = new System.Windows.Forms.Padding(4);
            B_OpenOPowers.Name = "B_OpenOPowers";
            B_OpenOPowers.Size = new System.Drawing.Size(96, 32);
            B_OpenOPowers.TabIndex = 1;
            B_OpenOPowers.Text = "O-Powers";
            B_OpenOPowers.UseVisualStyleBackColor = true;
            B_OpenOPowers.Click += B_OpenOPowers_Click;
            // 
            // B_OpenEventFlags
            // 
            B_OpenEventFlags.Location = new System.Drawing.Point(108, 44);
            B_OpenEventFlags.Margin = new System.Windows.Forms.Padding(4);
            B_OpenEventFlags.Name = "B_OpenEventFlags";
            B_OpenEventFlags.Size = new System.Drawing.Size(96, 32);
            B_OpenEventFlags.TabIndex = 1;
            B_OpenEventFlags.Text = "Event Flags";
            B_OpenEventFlags.UseVisualStyleBackColor = true;
            B_OpenEventFlags.Click += B_OpenEventFlags_Click;
            // 
            // B_OpenPokedex
            // 
            B_OpenPokedex.Location = new System.Drawing.Point(212, 44);
            B_OpenPokedex.Margin = new System.Windows.Forms.Padding(4);
            B_OpenPokedex.Name = "B_OpenPokedex";
            B_OpenPokedex.Size = new System.Drawing.Size(96, 32);
            B_OpenPokedex.TabIndex = 1;
            B_OpenPokedex.Text = "Pokédex";
            B_OpenPokedex.UseVisualStyleBackColor = true;
            B_OpenPokedex.Click += B_OpenPokedex_Click;
            // 
            // B_OpenLinkInfo
            // 
            B_OpenLinkInfo.Location = new System.Drawing.Point(316, 44);
            B_OpenLinkInfo.Margin = new System.Windows.Forms.Padding(4);
            B_OpenLinkInfo.Name = "B_OpenLinkInfo";
            B_OpenLinkInfo.Size = new System.Drawing.Size(96, 32);
            B_OpenLinkInfo.TabIndex = 1;
            B_OpenLinkInfo.Text = "Link Data";
            B_OpenLinkInfo.UseVisualStyleBackColor = true;
            B_OpenLinkInfo.Click += B_LinkInfo_Click;
            // 
            // B_OpenBerryField
            // 
            B_OpenBerryField.Location = new System.Drawing.Point(4, 84);
            B_OpenBerryField.Margin = new System.Windows.Forms.Padding(4);
            B_OpenBerryField.Name = "B_OpenBerryField";
            B_OpenBerryField.Size = new System.Drawing.Size(96, 32);
            B_OpenBerryField.TabIndex = 1;
            B_OpenBerryField.Text = "Berry Field";
            B_OpenBerryField.UseVisualStyleBackColor = true;
            B_OpenBerryField.Click += B_OpenBerryField_Click;
            // 
            // B_OpenPokeblocks
            // 
            B_OpenPokeblocks.Location = new System.Drawing.Point(108, 84);
            B_OpenPokeblocks.Margin = new System.Windows.Forms.Padding(4);
            B_OpenPokeblocks.Name = "B_OpenPokeblocks";
            B_OpenPokeblocks.Size = new System.Drawing.Size(96, 32);
            B_OpenPokeblocks.TabIndex = 1;
            B_OpenPokeblocks.Text = "Pokéblocks";
            B_OpenPokeblocks.UseVisualStyleBackColor = true;
            B_OpenPokeblocks.Visible = false;
            B_OpenPokeblocks.Click += B_OpenPokeblocks_Click;
            // 
            // B_OpenSecretBase
            // 
            B_OpenSecretBase.Location = new System.Drawing.Point(212, 84);
            B_OpenSecretBase.Margin = new System.Windows.Forms.Padding(4);
            B_OpenSecretBase.Name = "B_OpenSecretBase";
            B_OpenSecretBase.Size = new System.Drawing.Size(96, 32);
            B_OpenSecretBase.TabIndex = 1;
            B_OpenSecretBase.Text = "Secret Base";
            B_OpenSecretBase.UseVisualStyleBackColor = true;
            B_OpenSecretBase.Visible = false;
            B_OpenSecretBase.Click += B_OpenSecretBase_Click;
            // 
            // B_OpenPokepuffs
            // 
            B_OpenPokepuffs.Location = new System.Drawing.Point(316, 84);
            B_OpenPokepuffs.Margin = new System.Windows.Forms.Padding(4);
            B_OpenPokepuffs.Name = "B_OpenPokepuffs";
            B_OpenPokepuffs.Size = new System.Drawing.Size(96, 32);
            B_OpenPokepuffs.TabIndex = 1;
            B_OpenPokepuffs.Text = "‎Poké Puffs";
            B_OpenPokepuffs.UseVisualStyleBackColor = true;
            B_OpenPokepuffs.Click += B_OpenPokepuffs_Click;
            // 
            // B_OpenSuperTraining
            // 
            B_OpenSuperTraining.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            B_OpenSuperTraining.Location = new System.Drawing.Point(4, 124);
            B_OpenSuperTraining.Margin = new System.Windows.Forms.Padding(4);
            B_OpenSuperTraining.Name = "B_OpenSuperTraining";
            B_OpenSuperTraining.Size = new System.Drawing.Size(96, 32);
            B_OpenSuperTraining.TabIndex = 1;
            B_OpenSuperTraining.Text = "Super Train";
            B_OpenSuperTraining.UseVisualStyleBackColor = true;
            B_OpenSuperTraining.Click += B_OpenSuperTraining_Click;
            // 
            // B_OpenHallofFame
            // 
            B_OpenHallofFame.Location = new System.Drawing.Point(108, 124);
            B_OpenHallofFame.Margin = new System.Windows.Forms.Padding(4);
            B_OpenHallofFame.Name = "B_OpenHallofFame";
            B_OpenHallofFame.Size = new System.Drawing.Size(96, 32);
            B_OpenHallofFame.TabIndex = 1;
            B_OpenHallofFame.Text = "Hall of Fame";
            B_OpenHallofFame.UseVisualStyleBackColor = true;
            B_OpenHallofFame.Click += B_HallofFame_Click;
            // 
            // B_OUTPasserby
            // 
            B_OUTPasserby.Location = new System.Drawing.Point(212, 124);
            B_OUTPasserby.Margin = new System.Windows.Forms.Padding(4);
            B_OUTPasserby.Name = "B_OUTPasserby";
            B_OUTPasserby.Size = new System.Drawing.Size(96, 32);
            B_OUTPasserby.TabIndex = 1;
            B_OUTPasserby.Text = "Passerby";
            B_OUTPasserby.UseVisualStyleBackColor = true;
            B_OUTPasserby.Click += B_OUTPasserby_Click;
            // 
            // B_CGearSkin
            // 
            B_CGearSkin.Location = new System.Drawing.Point(316, 124);
            B_CGearSkin.Margin = new System.Windows.Forms.Padding(4);
            B_CGearSkin.Name = "B_CGearSkin";
            B_CGearSkin.Size = new System.Drawing.Size(96, 32);
            B_CGearSkin.TabIndex = 1;
            B_CGearSkin.Text = "C-Gear Skin";
            B_CGearSkin.UseVisualStyleBackColor = true;
            B_CGearSkin.Click += B_CGearSkin_Click;
            // 
            // B_OpenPokeBeans
            // 
            B_OpenPokeBeans.Location = new System.Drawing.Point(4, 164);
            B_OpenPokeBeans.Margin = new System.Windows.Forms.Padding(4);
            B_OpenPokeBeans.Name = "B_OpenPokeBeans";
            B_OpenPokeBeans.Size = new System.Drawing.Size(96, 32);
            B_OpenPokeBeans.TabIndex = 1;
            B_OpenPokeBeans.Text = "‎Poké Beans";
            B_OpenPokeBeans.UseVisualStyleBackColor = true;
            B_OpenPokeBeans.Click += B_OpenPokeBeans_Click;
            // 
            // B_CellsStickers
            // 
            B_CellsStickers.Location = new System.Drawing.Point(108, 164);
            B_CellsStickers.Margin = new System.Windows.Forms.Padding(4);
            B_CellsStickers.Name = "B_CellsStickers";
            B_CellsStickers.Size = new System.Drawing.Size(96, 32);
            B_CellsStickers.TabIndex = 1;
            B_CellsStickers.Text = "Cells/Stickers";
            B_CellsStickers.UseVisualStyleBackColor = true;
            B_CellsStickers.Click += B_CellsStickers_Click;
            // 
            // B_OpenMiscEditor
            // 
            B_OpenMiscEditor.Location = new System.Drawing.Point(212, 164);
            B_OpenMiscEditor.Margin = new System.Windows.Forms.Padding(4);
            B_OpenMiscEditor.Name = "B_OpenMiscEditor";
            B_OpenMiscEditor.Size = new System.Drawing.Size(96, 32);
            B_OpenMiscEditor.TabIndex = 1;
            B_OpenMiscEditor.Text = "Misc Edits";
            B_OpenMiscEditor.UseVisualStyleBackColor = true;
            B_OpenMiscEditor.Click += B_OpenMiscEditor_Click;
            // 
            // B_OpenHoneyTreeEditor
            // 
            B_OpenHoneyTreeEditor.Location = new System.Drawing.Point(316, 164);
            B_OpenHoneyTreeEditor.Margin = new System.Windows.Forms.Padding(4);
            B_OpenHoneyTreeEditor.Name = "B_OpenHoneyTreeEditor";
            B_OpenHoneyTreeEditor.Size = new System.Drawing.Size(96, 32);
            B_OpenHoneyTreeEditor.TabIndex = 1;
            B_OpenHoneyTreeEditor.Text = "Honey Tree";
            B_OpenHoneyTreeEditor.UseVisualStyleBackColor = true;
            B_OpenHoneyTreeEditor.Click += B_OpenHoneyTreeEditor_Click;
            // 
            // B_OpenFriendSafari
            // 
            B_OpenFriendSafari.Location = new System.Drawing.Point(4, 204);
            B_OpenFriendSafari.Margin = new System.Windows.Forms.Padding(4);
            B_OpenFriendSafari.Name = "B_OpenFriendSafari";
            B_OpenFriendSafari.Size = new System.Drawing.Size(96, 32);
            B_OpenFriendSafari.TabIndex = 1;
            B_OpenFriendSafari.Text = "Friend Safari";
            B_OpenFriendSafari.UseVisualStyleBackColor = true;
            B_OpenFriendSafari.Click += B_OpenFriendSafari_Click;
            // 
            // B_OpenRTCEditor
            // 
            B_OpenRTCEditor.Location = new System.Drawing.Point(108, 204);
            B_OpenRTCEditor.Margin = new System.Windows.Forms.Padding(4);
            B_OpenRTCEditor.Name = "B_OpenRTCEditor";
            B_OpenRTCEditor.Size = new System.Drawing.Size(96, 32);
            B_OpenRTCEditor.TabIndex = 1;
            B_OpenRTCEditor.Text = "Clock (RTC)";
            B_OpenRTCEditor.UseVisualStyleBackColor = true;
            B_OpenRTCEditor.Click += B_OpenRTCEditor_Click;
            // 
            // B_OpenUGSEditor
            // 
            B_OpenUGSEditor.Location = new System.Drawing.Point(212, 204);
            B_OpenUGSEditor.Margin = new System.Windows.Forms.Padding(4);
            B_OpenUGSEditor.Name = "B_OpenUGSEditor";
            B_OpenUGSEditor.Size = new System.Drawing.Size(96, 32);
            B_OpenUGSEditor.TabIndex = 1;
            B_OpenUGSEditor.Text = "Underground";
            B_OpenUGSEditor.UseVisualStyleBackColor = true;
            B_OpenUGSEditor.Click += B_OpenUGSEditor_Click;
            // 
            // B_OpenGeonetEditor
            // 
            B_OpenGeonetEditor.Location = new System.Drawing.Point(316, 204);
            B_OpenGeonetEditor.Margin = new System.Windows.Forms.Padding(4);
            B_OpenGeonetEditor.Name = "B_OpenGeonetEditor";
            B_OpenGeonetEditor.Size = new System.Drawing.Size(96, 32);
            B_OpenGeonetEditor.TabIndex = 1;
            B_OpenGeonetEditor.Text = "Geonet";
            B_OpenGeonetEditor.UseVisualStyleBackColor = true;
            B_OpenGeonetEditor.Click += B_OpenGeonetEditor_Click;
            // 
            // B_OpenUnityTowerEditor
            // 
            B_OpenUnityTowerEditor.Location = new System.Drawing.Point(4, 244);
            B_OpenUnityTowerEditor.Margin = new System.Windows.Forms.Padding(4);
            B_OpenUnityTowerEditor.Name = "B_OpenUnityTowerEditor";
            B_OpenUnityTowerEditor.Size = new System.Drawing.Size(96, 32);
            B_OpenUnityTowerEditor.TabIndex = 1;
            B_OpenUnityTowerEditor.Text = "Unity Tower";
            B_OpenUnityTowerEditor.UseVisualStyleBackColor = true;
            B_OpenUnityTowerEditor.Click += B_OpenUnityTowerEditor_Click;
            // 
            // B_OpenChatterEditor
            // 
            B_OpenChatterEditor.Location = new System.Drawing.Point(108, 244);
            B_OpenChatterEditor.Margin = new System.Windows.Forms.Padding(4);
            B_OpenChatterEditor.Name = "B_OpenChatterEditor";
            B_OpenChatterEditor.Size = new System.Drawing.Size(96, 32);
            B_OpenChatterEditor.TabIndex = 1;
            B_OpenChatterEditor.Text = "Chatter";
            B_OpenChatterEditor.UseVisualStyleBackColor = true;
            B_OpenChatterEditor.Click += B_OpenChatterEditor_Click;
            // 
            // B_Roamer
            // 
            B_Roamer.Location = new System.Drawing.Point(212, 244);
            B_Roamer.Margin = new System.Windows.Forms.Padding(4);
            B_Roamer.Name = "B_Roamer";
            B_Roamer.Size = new System.Drawing.Size(96, 32);
            B_Roamer.TabIndex = 1;
            B_Roamer.Text = "Roamer";
            B_Roamer.UseVisualStyleBackColor = true;
            B_Roamer.Click += B_Roamer_Click;
            // 
            // B_FestivalPlaza
            // 
            B_FestivalPlaza.Location = new System.Drawing.Point(316, 244);
            B_FestivalPlaza.Margin = new System.Windows.Forms.Padding(4);
            B_FestivalPlaza.Name = "B_FestivalPlaza";
            B_FestivalPlaza.Size = new System.Drawing.Size(96, 32);
            B_FestivalPlaza.TabIndex = 1;
            B_FestivalPlaza.Text = "Festival Plaza";
            B_FestivalPlaza.UseVisualStyleBackColor = true;
            B_FestivalPlaza.Click += B_FestivalPlaza_Click;
            // 
            // B_MailBox
            // 
            B_MailBox.Location = new System.Drawing.Point(4, 284);
            B_MailBox.Margin = new System.Windows.Forms.Padding(4);
            B_MailBox.Name = "B_MailBox";
            B_MailBox.Size = new System.Drawing.Size(96, 32);
            B_MailBox.TabIndex = 1;
            B_MailBox.Text = "Mail Box";
            B_MailBox.UseVisualStyleBackColor = true;
            B_MailBox.Click += B_MailBox_Click;
            // 
            // B_OpenApricorn
            // 
            B_OpenApricorn.Location = new System.Drawing.Point(108, 284);
            B_OpenApricorn.Margin = new System.Windows.Forms.Padding(4);
            B_OpenApricorn.Name = "B_OpenApricorn";
            B_OpenApricorn.Size = new System.Drawing.Size(96, 32);
            B_OpenApricorn.TabIndex = 1;
            B_OpenApricorn.Text = "Apricorns";
            B_OpenApricorn.UseVisualStyleBackColor = true;
            B_OpenApricorn.Click += B_OpenApricorn_Click;
            // 
            // B_Raids
            // 
            B_Raids.Location = new System.Drawing.Point(212, 284);
            B_Raids.Margin = new System.Windows.Forms.Padding(4);
            B_Raids.Name = "B_Raids";
            B_Raids.Size = new System.Drawing.Size(96, 32);
            B_Raids.TabIndex = 1;
            B_Raids.Text = "Raids";
            B_Raids.UseVisualStyleBackColor = true;
            B_Raids.Click += B_OpenRaids_Click;
            // 
            // B_RaidsDLC1
            // 
            B_RaidsDLC1.Location = new System.Drawing.Point(316, 284);
            B_RaidsDLC1.Margin = new System.Windows.Forms.Padding(4);
            B_RaidsDLC1.Name = "B_RaidsDLC1";
            B_RaidsDLC1.Size = new System.Drawing.Size(96, 32);
            B_RaidsDLC1.TabIndex = 2;
            B_RaidsDLC1.Text = "Raids (DLC 1)";
            B_RaidsDLC1.UseVisualStyleBackColor = true;
            B_RaidsDLC1.Click += B_OpenRaids_Click;
            // 
            // B_RaidsDLC2
            // 
            B_RaidsDLC2.Location = new System.Drawing.Point(4, 324);
            B_RaidsDLC2.Margin = new System.Windows.Forms.Padding(4);
            B_RaidsDLC2.Name = "B_RaidsDLC2";
            B_RaidsDLC2.Size = new System.Drawing.Size(96, 32);
            B_RaidsDLC2.TabIndex = 4;
            B_RaidsDLC2.Text = "Raids (DLC 2)";
            B_RaidsDLC2.UseVisualStyleBackColor = true;
            B_RaidsDLC2.Click += B_OpenRaids_Click;
            // 
            // B_Blocks
            // 
            B_Blocks.Location = new System.Drawing.Point(108, 324);
            B_Blocks.Margin = new System.Windows.Forms.Padding(4);
            B_Blocks.Name = "B_Blocks";
            B_Blocks.Size = new System.Drawing.Size(96, 32);
            B_Blocks.TabIndex = 1;
            B_Blocks.Text = "Block Data";
            B_Blocks.UseVisualStyleBackColor = true;
            B_Blocks.Click += B_Blocks_Click;
            // 
            // B_OtherSlots
            // 
            B_OtherSlots.Location = new System.Drawing.Point(212, 324);
            B_OtherSlots.Margin = new System.Windows.Forms.Padding(4);
            B_OtherSlots.Name = "B_OtherSlots";
            B_OtherSlots.Size = new System.Drawing.Size(96, 32);
            B_OtherSlots.TabIndex = 3;
            B_OtherSlots.Text = "Other Slots";
            B_OtherSlots.UseVisualStyleBackColor = true;
            B_OtherSlots.Click += B_OtherSlots_Click;
            // 
            // B_OpenSealStickers
            // 
            B_OpenSealStickers.Location = new System.Drawing.Point(316, 324);
            B_OpenSealStickers.Margin = new System.Windows.Forms.Padding(4);
            B_OpenSealStickers.Name = "B_OpenSealStickers";
            B_OpenSealStickers.Size = new System.Drawing.Size(96, 32);
            B_OpenSealStickers.TabIndex = 5;
            B_OpenSealStickers.Text = "Seal Stickers";
            B_OpenSealStickers.UseVisualStyleBackColor = true;
            B_OpenSealStickers.Click += B_OpenSealStickers_Click;
            // 
            // B_Poffins
            // 
            B_Poffins.Location = new System.Drawing.Point(4, 364);
            B_Poffins.Margin = new System.Windows.Forms.Padding(4);
            B_Poffins.Name = "B_Poffins";
            B_Poffins.Size = new System.Drawing.Size(96, 32);
            B_Poffins.TabIndex = 6;
            B_Poffins.Text = "Poffins";
            B_Poffins.UseVisualStyleBackColor = true;
            B_Poffins.Click += B_Poffins_Click;
            // 
            // B_RaidsSevenStar
            // 
            B_RaidsSevenStar.Location = new System.Drawing.Point(108, 364);
            B_RaidsSevenStar.Margin = new System.Windows.Forms.Padding(4);
            B_RaidsSevenStar.Name = "B_RaidsSevenStar";
            B_RaidsSevenStar.Size = new System.Drawing.Size(96, 32);
            B_RaidsSevenStar.TabIndex = 7;
            B_RaidsSevenStar.Text = "Raids (7 Star)";
            B_RaidsSevenStar.UseVisualStyleBackColor = true;
            B_RaidsSevenStar.Click += B_OpenRaids_Click;
            // 
            // FLP_SAVToolsMisc
            // 
            FLP_SAVToolsMisc.Controls.Add(B_SaveBoxBin);
            FLP_SAVToolsMisc.Controls.Add(B_VerifyCHK);
            FLP_SAVToolsMisc.Controls.Add(B_VerifySaveEntities);
            FLP_SAVToolsMisc.Controls.Add(Menu_ExportBAK);
            FLP_SAVToolsMisc.Controls.Add(B_JPEG);
            FLP_SAVToolsMisc.Controls.Add(B_ConvertKorean);
            FLP_SAVToolsMisc.Dock = System.Windows.Forms.DockStyle.Top;
            FLP_SAVToolsMisc.Location = new System.Drawing.Point(0, 0);
            FLP_SAVToolsMisc.Margin = new System.Windows.Forms.Padding(0);
            FLP_SAVToolsMisc.Name = "FLP_SAVToolsMisc";
            FLP_SAVToolsMisc.Size = new System.Drawing.Size(441, 52);
            FLP_SAVToolsMisc.TabIndex = 104;
            // 
            // B_SaveBoxBin
            // 
            B_SaveBoxBin.Location = new System.Drawing.Point(0, 0);
            B_SaveBoxBin.Margin = new System.Windows.Forms.Padding(0);
            B_SaveBoxBin.Name = "B_SaveBoxBin";
            B_SaveBoxBin.Size = new System.Drawing.Size(88, 48);
            B_SaveBoxBin.TabIndex = 1;
            B_SaveBoxBin.Text = "Save Box Data++";
            B_SaveBoxBin.UseVisualStyleBackColor = true;
            B_SaveBoxBin.Click += B_SaveBoxBin_Click;
            // 
            // B_VerifyCHK
            // 
            B_VerifyCHK.Location = new System.Drawing.Point(88, 0);
            B_VerifyCHK.Margin = new System.Windows.Forms.Padding(0);
            B_VerifyCHK.Name = "B_VerifyCHK";
            B_VerifyCHK.Size = new System.Drawing.Size(88, 48);
            B_VerifyCHK.TabIndex = 2;
            B_VerifyCHK.Text = "Verify Checksums";
            B_VerifyCHK.UseVisualStyleBackColor = true;
            B_VerifyCHK.Click += ClickVerifyCHK;
            // 
            // B_VerifySaveEntities
            // 
            B_VerifySaveEntities.Location = new System.Drawing.Point(176, 0);
            B_VerifySaveEntities.Margin = new System.Windows.Forms.Padding(0);
            B_VerifySaveEntities.Name = "B_VerifySaveEntities";
            B_VerifySaveEntities.Size = new System.Drawing.Size(88, 48);
            B_VerifySaveEntities.TabIndex = 3;
            B_VerifySaveEntities.Text = "Verify All PKMs";
            B_VerifySaveEntities.UseVisualStyleBackColor = true;
            B_VerifySaveEntities.Click += ClickVerifyStoredEntities;
            // 
            // Menu_ExportBAK
            // 
            Menu_ExportBAK.Location = new System.Drawing.Point(264, 0);
            Menu_ExportBAK.Margin = new System.Windows.Forms.Padding(0);
            Menu_ExportBAK.Name = "Menu_ExportBAK";
            Menu_ExportBAK.Size = new System.Drawing.Size(88, 48);
            Menu_ExportBAK.TabIndex = 4;
            Menu_ExportBAK.Text = "Export Backup";
            Menu_ExportBAK.UseVisualStyleBackColor = true;
            Menu_ExportBAK.Click += Menu_ExportBAK_Click;
            // 
            // B_JPEG
            // 
            B_JPEG.Location = new System.Drawing.Point(352, 0);
            B_JPEG.Margin = new System.Windows.Forms.Padding(0);
            B_JPEG.Name = "B_JPEG";
            B_JPEG.Size = new System.Drawing.Size(88, 48);
            B_JPEG.TabIndex = 5;
            B_JPEG.Text = "Save PGL .JPEG";
            B_JPEG.UseVisualStyleBackColor = true;
            B_JPEG.Click += B_JPEG_Click;
            // 
            // B_ConvertKorean
            // 
            B_ConvertKorean.Location = new System.Drawing.Point(0, 48);
            B_ConvertKorean.Margin = new System.Windows.Forms.Padding(0);
            B_ConvertKorean.Name = "B_ConvertKorean";
            B_ConvertKorean.Size = new System.Drawing.Size(88, 48);
            B_ConvertKorean.TabIndex = 6;
            B_ConvertKorean.Text = "Korean Save Conversion";
            B_ConvertKorean.UseVisualStyleBackColor = true;
            B_ConvertKorean.Click += B_ConvertKorean_Click;
            // 
            // CB_SaveSlot
            // 
            CB_SaveSlot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_SaveSlot.FormattingEnabled = true;
            CB_SaveSlot.Location = new System.Drawing.Point(152, 144);
            CB_SaveSlot.Name = "CB_SaveSlot";
            CB_SaveSlot.Size = new System.Drawing.Size(121, 23);
            CB_SaveSlot.TabIndex = 20;
            CB_SaveSlot.SelectedIndexChanged += UpdateSaveSlot;
            // 
            // L_SaveSlot
            // 
            L_SaveSlot.Location = new System.Drawing.Point(32, 144);
            L_SaveSlot.Name = "L_SaveSlot";
            L_SaveSlot.Size = new System.Drawing.Size(120, 24);
            L_SaveSlot.TabIndex = 19;
            L_SaveSlot.Text = "Save Slot:";
            L_SaveSlot.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Secure2
            // 
            L_Secure2.Location = new System.Drawing.Point(32, 112);
            L_Secure2.Name = "L_Secure2";
            L_Secure2.Size = new System.Drawing.Size(120, 24);
            L_Secure2.TabIndex = 18;
            L_Secure2.Text = "Secure Value 2:";
            L_Secure2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TB_Secure2
            // 
            TB_Secure2.Enabled = false;
            TB_Secure2.Font = new System.Drawing.Font("Courier New", 8.25F);
            TB_Secure2.Location = new System.Drawing.Point(152, 112);
            TB_Secure2.MaxLength = 16;
            TB_Secure2.MinimumSize = new System.Drawing.Size(120, 24);
            TB_Secure2.Name = "TB_Secure2";
            TB_Secure2.PlaceholderText = "0000000000000000";
            TB_Secure2.Size = new System.Drawing.Size(120, 24);
            TB_Secure2.TabIndex = 17;
            TB_Secure2.Validated += UpdateStringSeed;
            // 
            // L_Secure1
            // 
            L_Secure1.Location = new System.Drawing.Point(32, 88);
            L_Secure1.Name = "L_Secure1";
            L_Secure1.Size = new System.Drawing.Size(120, 24);
            L_Secure1.TabIndex = 16;
            L_Secure1.Text = "Secure Value 1:";
            L_Secure1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TB_Secure1
            // 
            TB_Secure1.Enabled = false;
            TB_Secure1.Font = new System.Drawing.Font("Courier New", 8.25F);
            TB_Secure1.Location = new System.Drawing.Point(152, 88);
            TB_Secure1.MaxLength = 16;
            TB_Secure1.MinimumSize = new System.Drawing.Size(120, 24);
            TB_Secure1.Name = "TB_Secure1";
            TB_Secure1.PlaceholderText = "0000000000000000";
            TB_Secure1.Size = new System.Drawing.Size(120, 24);
            TB_Secure1.TabIndex = 15;
            TB_Secure1.Validated += UpdateStringSeed;
            // 
            // L_GameSync
            // 
            L_GameSync.Location = new System.Drawing.Point(32, 64);
            L_GameSync.Name = "L_GameSync";
            L_GameSync.Size = new System.Drawing.Size(120, 24);
            L_GameSync.TabIndex = 11;
            L_GameSync.Text = "Game Sync ID:";
            L_GameSync.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TB_GameSync
            // 
            TB_GameSync.Enabled = false;
            TB_GameSync.Font = new System.Drawing.Font("Courier New", 8.25F);
            TB_GameSync.Location = new System.Drawing.Point(152, 64);
            TB_GameSync.MaxLength = 16;
            TB_GameSync.MinimumSize = new System.Drawing.Size(120, 24);
            TB_GameSync.Name = "TB_GameSync";
            TB_GameSync.PlaceholderText = "0000000000000000";
            TB_GameSync.Size = new System.Drawing.Size(120, 24);
            TB_GameSync.TabIndex = 10;
            TB_GameSync.Validated += UpdateStringSeed;
            // 
            // SAVEditor
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(tabBoxMulti);
            Name = "SAVEditor";
            Size = new System.Drawing.Size(449, 363);
            tabBoxMulti.ResumeLayout(false);
            Tab_Box.ResumeLayout(false);
            Tab_Box.PerformLayout();
            Tab_PartyBattle.ResumeLayout(false);
            Tab_PartyBattle.PerformLayout();
            Tab_Other.ResumeLayout(false);
            GB_Daycare.ResumeLayout(false);
            GB_Daycare.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dcpkx2).EndInit();
            ((System.ComponentModel.ISupportInitialize)dcpkx1).EndInit();
            Tab_SAV.ResumeLayout(false);
            Tab_SAV.PerformLayout();
            FLP_SAVtools.ResumeLayout(false);
            FLP_SAVToolsMisc.ResumeLayout(false);
            ResumeLayout(false);
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
        private System.Windows.Forms.Button B_OpenGeonetEditor;
        private System.Windows.Forms.Button B_OpenUnityTowerEditor;
        private System.Windows.Forms.Button B_OpenChatterEditor;
        private System.Windows.Forms.Button B_Roamer;
        private System.Windows.Forms.Button B_FestivalPlaza;
        private System.Windows.Forms.Button B_MailBox;
        private System.Windows.Forms.Button B_OpenApricorn;
        private SlotList SL_Extra;
        private PartyEditor SL_Party;
        private System.Windows.Forms.Button B_Raids;
        private System.Windows.Forms.Button B_Blocks;
        private System.Windows.Forms.Button B_RaidsDLC1;
        private System.Windows.Forms.Button B_RaidsDLC2;
        private System.Windows.Forms.Button B_OtherSlots;
        private System.Windows.Forms.Button Menu_ExportBAK;
        private System.Windows.Forms.FlowLayoutPanel FLP_SAVToolsMisc;
        private System.Windows.Forms.Button B_OpenSealStickers;
        private System.Windows.Forms.Button B_Poffins;
        private System.Windows.Forms.Button B_VerifySaveEntities;
        private System.Windows.Forms.Button B_RaidsSevenStar;
        private System.Windows.Forms.Button B_ConvertKorean;
    }
}
