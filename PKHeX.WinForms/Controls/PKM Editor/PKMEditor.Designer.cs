namespace PKHeX.WinForms.Controls
{
    partial class PKMEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PKMEditor));
            this.Hidden_TC = new System.Windows.Forms.TabControl();
            this.Hidden_Main = new System.Windows.Forms.TabPage();
            this.FLP_Main = new System.Windows.Forms.FlowLayoutPanel();
            this.FLP_PID = new System.Windows.Forms.FlowLayoutPanel();
            this.FLP_PIDLeft = new System.Windows.Forms.FlowLayoutPanel();
            this.Label_PID = new System.Windows.Forms.Label();
            this.BTN_Shinytize = new System.Windows.Forms.Button();
            this.PB_ShinyStar = new System.Windows.Forms.PictureBox();
            this.PB_ShinySquare = new System.Windows.Forms.PictureBox();
            this.FLP_PIDRight = new System.Windows.Forms.FlowLayoutPanel();
            this.TB_PID = new System.Windows.Forms.TextBox();
            this.UC_Gender = new PKHeX.WinForms.Controls.GenderToggle();
            this.BTN_RerollPID = new System.Windows.Forms.Button();
            this.FLP_Species = new System.Windows.Forms.FlowLayoutPanel();
            this.Label_Species = new System.Windows.Forms.Label();
            this.CB_Species = new System.Windows.Forms.ComboBox();
            this.FLP_Nickname = new System.Windows.Forms.FlowLayoutPanel();
            this.FLP_NicknameLeft = new System.Windows.Forms.FlowLayoutPanel();
            this.CHK_Nicknamed = new System.Windows.Forms.Label();
            this.CHK_NicknamedFlag = new System.Windows.Forms.CheckBox();
            this.TB_Nickname = new System.Windows.Forms.TextBox();
            this.FLP_EXPLevel = new System.Windows.Forms.FlowLayoutPanel();
            this.Label_EXP = new System.Windows.Forms.Label();
            this.FLP_EXPLevelRight = new System.Windows.Forms.FlowLayoutPanel();
            this.TB_EXP = new System.Windows.Forms.MaskedTextBox();
            this.Label_CurLevel = new System.Windows.Forms.Label();
            this.TB_Level = new System.Windows.Forms.MaskedTextBox();
            this.FLP_Nature = new System.Windows.Forms.FlowLayoutPanel();
            this.Label_Nature = new System.Windows.Forms.Label();
            this.CB_Nature = new System.Windows.Forms.ComboBox();
            this.FLP_OriginalNature = new System.Windows.Forms.FlowLayoutPanel();
            this.L_OriginalNature = new System.Windows.Forms.Label();
            this.CB_StatNature = new System.Windows.Forms.ComboBox();
            this.FLP_Form = new System.Windows.Forms.FlowLayoutPanel();
            this.FLP_FormLeft = new System.Windows.Forms.FlowLayoutPanel();
            this.Label_Form = new System.Windows.Forms.Label();
            this.L_FormArgument = new System.Windows.Forms.Label();
            this.FLP_FormRight = new System.Windows.Forms.FlowLayoutPanel();
            this.CB_Form = new System.Windows.Forms.ComboBox();
            this.FA_Form = new PKHeX.WinForms.Controls.FormArgument();
            this.FLP_HeldItem = new System.Windows.Forms.FlowLayoutPanel();
            this.Label_HeldItem = new System.Windows.Forms.Label();
            this.CB_HeldItem = new System.Windows.Forms.ComboBox();
            this.FLP_Ability = new System.Windows.Forms.FlowLayoutPanel();
            this.Label_Ability = new System.Windows.Forms.Label();
            this.FLP_AbilityRight = new System.Windows.Forms.FlowLayoutPanel();
            this.CB_Ability = new System.Windows.Forms.ComboBox();
            this.DEV_Ability = new System.Windows.Forms.ComboBox();
            this.TB_AbilityNumber = new System.Windows.Forms.MaskedTextBox();
            this.FLP_Friendship = new System.Windows.Forms.FlowLayoutPanel();
            this.FLP_FriendshipLeft = new System.Windows.Forms.FlowLayoutPanel();
            this.Label_Friendship = new System.Windows.Forms.Label();
            this.Label_HatchCounter = new System.Windows.Forms.Label();
            this.FLP_FriendshipRight = new System.Windows.Forms.FlowLayoutPanel();
            this.TB_Friendship = new System.Windows.Forms.MaskedTextBox();
            this.FLP_Language = new System.Windows.Forms.FlowLayoutPanel();
            this.Label_Language = new System.Windows.Forms.Label();
            this.CB_Language = new System.Windows.Forms.ComboBox();
            this.FLP_EggPKRS = new System.Windows.Forms.FlowLayoutPanel();
            this.FLP_EggPKRSLeft = new System.Windows.Forms.FlowLayoutPanel();
            this.CHK_IsEgg = new System.Windows.Forms.CheckBox();
            this.FLP_EggPKRSRight = new System.Windows.Forms.FlowLayoutPanel();
            this.CHK_Infected = new System.Windows.Forms.CheckBox();
            this.CHK_Cured = new System.Windows.Forms.CheckBox();
            this.FLP_PKRS = new System.Windows.Forms.FlowLayoutPanel();
            this.Label_PKRS = new System.Windows.Forms.Label();
            this.FLP_PKRSRight = new System.Windows.Forms.FlowLayoutPanel();
            this.CB_PKRSStrain = new System.Windows.Forms.ComboBox();
            this.Label_PKRSdays = new System.Windows.Forms.Label();
            this.CB_PKRSDays = new System.Windows.Forms.ComboBox();
            this.FLP_NSparkle = new System.Windows.Forms.FlowLayoutPanel();
            this.L_NSparkle = new System.Windows.Forms.Label();
            this.CHK_NSparkle = new System.Windows.Forms.CheckBox();
            this.FLP_ShadowID = new System.Windows.Forms.FlowLayoutPanel();
            this.L_ShadowID = new System.Windows.Forms.Label();
            this.NUD_ShadowID = new System.Windows.Forms.NumericUpDown();
            this.FLP_Purification = new System.Windows.Forms.FlowLayoutPanel();
            this.L_HeartGauge = new System.Windows.Forms.Label();
            this.NUD_Purification = new System.Windows.Forms.NumericUpDown();
            this.CHK_Shadow = new System.Windows.Forms.CheckBox();
            this.FLP_CatchRate = new System.Windows.Forms.FlowLayoutPanel();
            this.L_CatchRate = new System.Windows.Forms.Label();
            this.CR_PK1 = new PKHeX.WinForms.Controls.CatchRate();
            this.Hidden_Met = new System.Windows.Forms.TabPage();
            this.CHK_AsEgg = new System.Windows.Forms.CheckBox();
            this.GB_EggConditions = new System.Windows.Forms.GroupBox();
            this.CB_EggLocation = new System.Windows.Forms.ComboBox();
            this.CAL_EggDate = new System.Windows.Forms.DateTimePicker();
            this.Label_EggDate = new System.Windows.Forms.Label();
            this.Label_EggLocation = new System.Windows.Forms.Label();
            this.FLP_Met = new System.Windows.Forms.FlowLayoutPanel();
            this.FLP_OriginGame = new System.Windows.Forms.FlowLayoutPanel();
            this.Label_OriginGame = new System.Windows.Forms.Label();
            this.CB_GameOrigin = new System.Windows.Forms.ComboBox();
            this.FLP_BattleVersion = new System.Windows.Forms.FlowLayoutPanel();
            this.L_BattleVersion = new System.Windows.Forms.Label();
            this.CB_BattleVersion = new System.Windows.Forms.ComboBox();
            this.FLP_MetLocation = new System.Windows.Forms.FlowLayoutPanel();
            this.Label_MetLocation = new System.Windows.Forms.Label();
            this.CB_MetLocation = new System.Windows.Forms.ComboBox();
            this.FLP_Ball = new System.Windows.Forms.FlowLayoutPanel();
            this.FLP_BallLeft = new System.Windows.Forms.FlowLayoutPanel();
            this.Label_Ball = new System.Windows.Forms.Label();
            this.PB_Ball = new System.Windows.Forms.PictureBox();
            this.CB_Ball = new System.Windows.Forms.ComboBox();
            this.FLP_MetDate = new System.Windows.Forms.FlowLayoutPanel();
            this.Label_MetDate = new System.Windows.Forms.Label();
            this.CAL_MetDate = new System.Windows.Forms.DateTimePicker();
            this.FLP_MetLevel = new System.Windows.Forms.FlowLayoutPanel();
            this.Label_MetLevel = new System.Windows.Forms.Label();
            this.TB_MetLevel = new System.Windows.Forms.MaskedTextBox();
            this.CHK_Fateful = new System.Windows.Forms.CheckBox();
            this.FLP_ObedienceLevel = new System.Windows.Forms.FlowLayoutPanel();
            this.L_ObedienceLevel = new System.Windows.Forms.Label();
            this.TB_ObedienceLevel = new System.Windows.Forms.MaskedTextBox();
            this.FLP_GroundTile = new System.Windows.Forms.FlowLayoutPanel();
            this.Label_GroundTile = new System.Windows.Forms.Label();
            this.CB_GroundTile = new System.Windows.Forms.ComboBox();
            this.FLP_TimeOfDay = new System.Windows.Forms.FlowLayoutPanel();
            this.L_MetTimeOfDay = new System.Windows.Forms.Label();
            this.CB_MetTimeOfDay = new System.Windows.Forms.ComboBox();
            this.Hidden_Stats = new System.Windows.Forms.TabPage();
            this.Stats = new PKHeX.WinForms.Controls.StatEditor();
            this.Hidden_Moves = new System.Windows.Forms.TabPage();
            this.L_AlphaMastered = new System.Windows.Forms.Label();
            this.CB_AlphaMastered = new System.Windows.Forms.ComboBox();
            this.FLP_MoveFlags = new System.Windows.Forms.FlowLayoutPanel();
            this.B_RelearnFlags = new System.Windows.Forms.Button();
            this.B_MoveShop = new System.Windows.Forms.Button();
            this.GB_RelearnMoves = new System.Windows.Forms.GroupBox();
            this.PB_WarnRelearn4 = new System.Windows.Forms.PictureBox();
            this.PB_WarnRelearn3 = new System.Windows.Forms.PictureBox();
            this.PB_WarnRelearn2 = new System.Windows.Forms.PictureBox();
            this.PB_WarnRelearn1 = new System.Windows.Forms.PictureBox();
            this.CB_RelearnMove4 = new System.Windows.Forms.ComboBox();
            this.CB_RelearnMove3 = new System.Windows.Forms.ComboBox();
            this.CB_RelearnMove2 = new System.Windows.Forms.ComboBox();
            this.CB_RelearnMove1 = new System.Windows.Forms.ComboBox();
            this.GB_CurrentMoves = new System.Windows.Forms.GroupBox();
            this.FLP_Moves = new System.Windows.Forms.FlowLayoutPanel();
            this.MC_Move1 = new PKHeX.WinForms.Controls.MoveChoice();
            this.MC_Move2 = new PKHeX.WinForms.Controls.MoveChoice();
            this.MC_Move3 = new PKHeX.WinForms.Controls.MoveChoice();
            this.MC_Move4 = new PKHeX.WinForms.Controls.MoveChoice();
            this.Label_CurPP = new System.Windows.Forms.Label();
            this.Label_PPups = new System.Windows.Forms.Label();
            this.Hidden_Cosmetic = new System.Windows.Forms.TabPage();
            this.Contest = new PKHeX.WinForms.Controls.ContestStat();
            this.FLP_PKMEditors = new System.Windows.Forms.FlowLayoutPanel();
            this.BTN_Ribbons = new System.Windows.Forms.Button();
            this.BTN_Medals = new System.Windows.Forms.Button();
            this.BTN_History = new System.Windows.Forms.Button();
            this.FLP_CosmeticTop = new System.Windows.Forms.FlowLayoutPanel();
            this.GB_Markings = new System.Windows.Forms.GroupBox();
            this.PB_Mark6 = new System.Windows.Forms.PictureBox();
            this.PB_Mark3 = new System.Windows.Forms.PictureBox();
            this.PB_Mark5 = new System.Windows.Forms.PictureBox();
            this.PB_MarkCured = new System.Windows.Forms.PictureBox();
            this.PB_Mark2 = new System.Windows.Forms.PictureBox();
            this.PB_MarkShiny = new System.Windows.Forms.PictureBox();
            this.PB_Mark1 = new System.Windows.Forms.PictureBox();
            this.PB_Mark4 = new System.Windows.Forms.PictureBox();
            this.FLP_BigMarkings = new System.Windows.Forms.FlowLayoutPanel();
            this.PB_Favorite = new System.Windows.Forms.PictureBox();
            this.PB_Origin = new System.Windows.Forms.PictureBox();
            this.PB_BattleVersion = new System.Windows.Forms.PictureBox();
            this.SizeCP = new PKHeX.WinForms.Controls.SizeCP();
            this.ShinyLeaf = new PKHeX.WinForms.Controls.ShinyLeaf();
            this.Hidden_OTMisc = new System.Windows.Forms.TabPage();
            this.FLP_OTMisc = new System.Windows.Forms.FlowLayoutPanel();
            this.GB_OT = new System.Windows.Forms.Label();
            this.TID_Trainer = new PKHeX.WinForms.Controls.TrainerID();
            this.FLP_OT = new System.Windows.Forms.FlowLayoutPanel();
            this.Label_OT = new System.Windows.Forms.Label();
            this.TB_OT = new System.Windows.Forms.TextBox();
            this.UC_OTGender = new PKHeX.WinForms.Controls.GenderToggle();
            this.FLP_Country = new System.Windows.Forms.FlowLayoutPanel();
            this.Label_Country = new System.Windows.Forms.Label();
            this.CB_Country = new System.Windows.Forms.ComboBox();
            this.FLP_SubRegion = new System.Windows.Forms.FlowLayoutPanel();
            this.Label_SubRegion = new System.Windows.Forms.Label();
            this.CB_SubRegion = new System.Windows.Forms.ComboBox();
            this.FLP_3DSRegion = new System.Windows.Forms.FlowLayoutPanel();
            this.Label_3DSRegion = new System.Windows.Forms.Label();
            this.CB_3DSReg = new System.Windows.Forms.ComboBox();
            this.FLP_Handler = new System.Windows.Forms.FlowLayoutPanel();
            this.L_CurrentHandler = new System.Windows.Forms.Label();
            this.CB_Handler = new System.Windows.Forms.ComboBox();
            this.GB_nOT = new System.Windows.Forms.Label();
            this.FLP_HT = new System.Windows.Forms.FlowLayoutPanel();
            this.Label_PrevOT = new System.Windows.Forms.Label();
            this.TB_HT = new System.Windows.Forms.TextBox();
            this.UC_HTGender = new PKHeX.WinForms.Controls.GenderToggle();
            this.FLP_HTLanguage = new System.Windows.Forms.FlowLayoutPanel();
            this.L_LanguageHT = new System.Windows.Forms.Label();
            this.CB_HTLanguage = new System.Windows.Forms.ComboBox();
            this.FLP_ExtraBytes = new System.Windows.Forms.FlowLayoutPanel();
            this.L_ExtraBytes = new System.Windows.Forms.Label();
            this.CB_ExtraBytes = new System.Windows.Forms.ComboBox();
            this.TB_ExtraByte = new System.Windows.Forms.MaskedTextBox();
            this.L_HomeTracker = new System.Windows.Forms.Label();
            this.TB_HomeTracker = new System.Windows.Forms.TextBox();
            this.Label_EncryptionConstant = new System.Windows.Forms.Label();
            this.TB_EC = new System.Windows.Forms.TextBox();
            this.BTN_RerollEC = new System.Windows.Forms.Button();
            this.TC_Editor = new PKHeX.WinForms.Controls.VerticalTabControlEntityEditor();
            this.Tab_Main = new System.Windows.Forms.TabPage();
            this.Tab_Met = new System.Windows.Forms.TabPage();
            this.Tab_Stats = new System.Windows.Forms.TabPage();
            this.Tab_Moves = new System.Windows.Forms.TabPage();
            this.Tab_Cosmetic = new System.Windows.Forms.TabPage();
            this.Tab_OTMisc = new System.Windows.Forms.TabPage();
            this.Hidden_TC.SuspendLayout();
            this.Hidden_Main.SuspendLayout();
            this.FLP_Main.SuspendLayout();
            this.FLP_PID.SuspendLayout();
            this.FLP_PIDLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_ShinyStar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_ShinySquare)).BeginInit();
            this.FLP_PIDRight.SuspendLayout();
            this.FLP_Species.SuspendLayout();
            this.FLP_Nickname.SuspendLayout();
            this.FLP_NicknameLeft.SuspendLayout();
            this.FLP_EXPLevel.SuspendLayout();
            this.FLP_EXPLevelRight.SuspendLayout();
            this.FLP_Nature.SuspendLayout();
            this.FLP_OriginalNature.SuspendLayout();
            this.FLP_Form.SuspendLayout();
            this.FLP_FormLeft.SuspendLayout();
            this.FLP_FormRight.SuspendLayout();
            this.FLP_HeldItem.SuspendLayout();
            this.FLP_Ability.SuspendLayout();
            this.FLP_AbilityRight.SuspendLayout();
            this.FLP_Friendship.SuspendLayout();
            this.FLP_FriendshipLeft.SuspendLayout();
            this.FLP_FriendshipRight.SuspendLayout();
            this.FLP_Language.SuspendLayout();
            this.FLP_EggPKRS.SuspendLayout();
            this.FLP_EggPKRSLeft.SuspendLayout();
            this.FLP_EggPKRSRight.SuspendLayout();
            this.FLP_PKRS.SuspendLayout();
            this.FLP_PKRSRight.SuspendLayout();
            this.FLP_NSparkle.SuspendLayout();
            this.FLP_ShadowID.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_ShadowID)).BeginInit();
            this.FLP_Purification.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Purification)).BeginInit();
            this.FLP_CatchRate.SuspendLayout();
            this.Hidden_Met.SuspendLayout();
            this.GB_EggConditions.SuspendLayout();
            this.FLP_Met.SuspendLayout();
            this.FLP_OriginGame.SuspendLayout();
            this.FLP_BattleVersion.SuspendLayout();
            this.FLP_MetLocation.SuspendLayout();
            this.FLP_Ball.SuspendLayout();
            this.FLP_BallLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Ball)).BeginInit();
            this.FLP_MetDate.SuspendLayout();
            this.FLP_MetLevel.SuspendLayout();
            this.FLP_ObedienceLevel.SuspendLayout();
            this.FLP_GroundTile.SuspendLayout();
            this.FLP_TimeOfDay.SuspendLayout();
            this.Hidden_Stats.SuspendLayout();
            this.Hidden_Moves.SuspendLayout();
            this.FLP_MoveFlags.SuspendLayout();
            this.GB_RelearnMoves.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_WarnRelearn4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_WarnRelearn3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_WarnRelearn2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_WarnRelearn1)).BeginInit();
            this.GB_CurrentMoves.SuspendLayout();
            this.FLP_Moves.SuspendLayout();
            this.Hidden_Cosmetic.SuspendLayout();
            this.FLP_PKMEditors.SuspendLayout();
            this.FLP_CosmeticTop.SuspendLayout();
            this.GB_Markings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Mark6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Mark3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Mark5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_MarkCured)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Mark2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_MarkShiny)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Mark1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Mark4)).BeginInit();
            this.FLP_BigMarkings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Favorite)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Origin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_BattleVersion)).BeginInit();
            this.Hidden_OTMisc.SuspendLayout();
            this.FLP_OTMisc.SuspendLayout();
            this.FLP_OT.SuspendLayout();
            this.FLP_Country.SuspendLayout();
            this.FLP_SubRegion.SuspendLayout();
            this.FLP_3DSRegion.SuspendLayout();
            this.FLP_Handler.SuspendLayout();
            this.FLP_HT.SuspendLayout();
            this.FLP_HTLanguage.SuspendLayout();
            this.FLP_ExtraBytes.SuspendLayout();
            this.TC_Editor.SuspendLayout();
            this.SuspendLayout();
            // 
            // Hidden_TC
            // 
            this.Hidden_TC.AllowDrop = true;
            this.Hidden_TC.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Hidden_TC.Controls.Add(this.Hidden_Main);
            this.Hidden_TC.Controls.Add(this.Hidden_Met);
            this.Hidden_TC.Controls.Add(this.Hidden_Stats);
            this.Hidden_TC.Controls.Add(this.Hidden_Moves);
            this.Hidden_TC.Controls.Add(this.Hidden_Cosmetic);
            this.Hidden_TC.Controls.Add(this.Hidden_OTMisc);
            this.Hidden_TC.ItemSize = new System.Drawing.Size(0, 1);
            this.Hidden_TC.Location = new System.Drawing.Point(96, -3);
            this.Hidden_TC.Margin = new System.Windows.Forms.Padding(0);
            this.Hidden_TC.Name = "Hidden_TC";
            this.Hidden_TC.SelectedIndex = 0;
            this.Hidden_TC.Size = new System.Drawing.Size(304, 400);
            this.Hidden_TC.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.Hidden_TC.TabIndex = 1;
            this.Hidden_TC.TabStop = false;
            // 
            // Hidden_Main
            // 
            this.Hidden_Main.AllowDrop = true;
            this.Hidden_Main.Controls.Add(this.FLP_Main);
            this.Hidden_Main.Location = new System.Drawing.Point(4, 5);
            this.Hidden_Main.Margin = new System.Windows.Forms.Padding(0);
            this.Hidden_Main.Name = "Hidden_Main";
            this.Hidden_Main.Size = new System.Drawing.Size(296, 391);
            this.Hidden_Main.TabIndex = 0;
            this.Hidden_Main.Text = "Main";
            this.Hidden_Main.UseVisualStyleBackColor = true;
            // 
            // FLP_Main
            // 
            this.FLP_Main.Controls.Add(this.FLP_PID);
            this.FLP_Main.Controls.Add(this.FLP_Species);
            this.FLP_Main.Controls.Add(this.FLP_Nickname);
            this.FLP_Main.Controls.Add(this.FLP_EXPLevel);
            this.FLP_Main.Controls.Add(this.FLP_Nature);
            this.FLP_Main.Controls.Add(this.FLP_OriginalNature);
            this.FLP_Main.Controls.Add(this.FLP_Form);
            this.FLP_Main.Controls.Add(this.FLP_HeldItem);
            this.FLP_Main.Controls.Add(this.FLP_Ability);
            this.FLP_Main.Controls.Add(this.FLP_Friendship);
            this.FLP_Main.Controls.Add(this.FLP_Language);
            this.FLP_Main.Controls.Add(this.FLP_EggPKRS);
            this.FLP_Main.Controls.Add(this.FLP_PKRS);
            this.FLP_Main.Controls.Add(this.FLP_NSparkle);
            this.FLP_Main.Controls.Add(this.FLP_ShadowID);
            this.FLP_Main.Controls.Add(this.FLP_Purification);
            this.FLP_Main.Controls.Add(this.FLP_CatchRate);
            this.FLP_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FLP_Main.Location = new System.Drawing.Point(0, 0);
            this.FLP_Main.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_Main.Name = "FLP_Main";
            this.FLP_Main.Padding = new System.Windows.Forms.Padding(0, 16, 0, 0);
            this.FLP_Main.Size = new System.Drawing.Size(296, 391);
            this.FLP_Main.TabIndex = 103;
            // 
            // FLP_PID
            // 
            this.FLP_PID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_PID.Controls.Add(this.FLP_PIDLeft);
            this.FLP_PID.Controls.Add(this.FLP_PIDRight);
            this.FLP_PID.Location = new System.Drawing.Point(0, 16);
            this.FLP_PID.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_PID.Name = "FLP_PID";
            this.FLP_PID.Size = new System.Drawing.Size(272, 24);
            this.FLP_PID.TabIndex = 0;
            // 
            // FLP_PIDLeft
            // 
            this.FLP_PIDLeft.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.FLP_PIDLeft.Controls.Add(this.Label_PID);
            this.FLP_PIDLeft.Controls.Add(this.BTN_Shinytize);
            this.FLP_PIDLeft.Controls.Add(this.PB_ShinyStar);
            this.FLP_PIDLeft.Controls.Add(this.PB_ShinySquare);
            this.FLP_PIDLeft.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.FLP_PIDLeft.Location = new System.Drawing.Point(0, 0);
            this.FLP_PIDLeft.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_PIDLeft.Name = "FLP_PIDLeft";
            this.FLP_PIDLeft.Size = new System.Drawing.Size(104, 24);
            this.FLP_PIDLeft.TabIndex = 0;
            // 
            // Label_PID
            // 
            this.Label_PID.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Label_PID.Location = new System.Drawing.Point(72, 0);
            this.Label_PID.Margin = new System.Windows.Forms.Padding(0);
            this.Label_PID.Name = "Label_PID";
            this.Label_PID.Size = new System.Drawing.Size(32, 24);
            this.Label_PID.TabIndex = 0;
            this.Label_PID.Text = "PID:";
            this.Label_PID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // BTN_Shinytize
            // 
            this.BTN_Shinytize.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.BTN_Shinytize.Location = new System.Drawing.Point(48, 0);
            this.BTN_Shinytize.Margin = new System.Windows.Forms.Padding(0);
            this.BTN_Shinytize.Name = "BTN_Shinytize";
            this.BTN_Shinytize.Size = new System.Drawing.Size(24, 24);
            this.BTN_Shinytize.TabIndex = 1;
            this.BTN_Shinytize.Text = "☆";
            this.BTN_Shinytize.UseVisualStyleBackColor = true;
            this.BTN_Shinytize.Click += new System.EventHandler(this.UpdateShinyPID);
            // 
            // PB_ShinyStar
            // 
            this.PB_ShinyStar.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.PB_ShinyStar.Image = global::PKHeX.WinForms.Properties.Resources.rare_icon;
            this.PB_ShinyStar.InitialImage = global::PKHeX.WinForms.Properties.Resources.rare_icon;
            this.PB_ShinyStar.Location = new System.Drawing.Point(24, 0);
            this.PB_ShinyStar.Margin = new System.Windows.Forms.Padding(0);
            this.PB_ShinyStar.Name = "PB_ShinyStar";
            this.PB_ShinyStar.Size = new System.Drawing.Size(24, 24);
            this.PB_ShinyStar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PB_ShinyStar.TabIndex = 62;
            this.PB_ShinyStar.TabStop = false;
            this.PB_ShinyStar.Visible = false;
            // 
            // PB_ShinySquare
            // 
            this.PB_ShinySquare.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.PB_ShinySquare.Image = global::PKHeX.WinForms.Properties.Resources.rare_icon_2;
            this.PB_ShinySquare.InitialImage = global::PKHeX.WinForms.Properties.Resources.rare_icon_2;
            this.PB_ShinySquare.Location = new System.Drawing.Point(0, 0);
            this.PB_ShinySquare.Margin = new System.Windows.Forms.Padding(0);
            this.PB_ShinySquare.Name = "PB_ShinySquare";
            this.PB_ShinySquare.Size = new System.Drawing.Size(24, 24);
            this.PB_ShinySquare.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PB_ShinySquare.TabIndex = 62;
            this.PB_ShinySquare.TabStop = false;
            this.PB_ShinySquare.Visible = false;
            // 
            // FLP_PIDRight
            // 
            this.FLP_PIDRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_PIDRight.Controls.Add(this.TB_PID);
            this.FLP_PIDRight.Controls.Add(this.UC_Gender);
            this.FLP_PIDRight.Controls.Add(this.BTN_RerollPID);
            this.FLP_PIDRight.Location = new System.Drawing.Point(104, 0);
            this.FLP_PIDRight.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_PIDRight.Name = "FLP_PIDRight";
            this.FLP_PIDRight.Size = new System.Drawing.Size(162, 24);
            this.FLP_PIDRight.TabIndex = 104;
            // 
            // TB_PID
            // 
            this.TB_PID.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.TB_PID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_PID.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TB_PID.Location = new System.Drawing.Point(0, 3);
            this.TB_PID.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.TB_PID.MaxLength = 8;
            this.TB_PID.Name = "TB_PID";
            this.TB_PID.PlaceholderText = "12345678";
            this.TB_PID.Size = new System.Drawing.Size(64, 20);
            this.TB_PID.TabIndex = 1;
            this.TB_PID.MouseHover += new System.EventHandler(this.UpdateTSV);
            this.TB_PID.Validated += new System.EventHandler(this.Update_ID);
            // 
            // UC_Gender
            // 
            this.UC_Gender.AllowClick = false;
            this.UC_Gender.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("UC_Gender.BackgroundImage")));
            this.UC_Gender.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.UC_Gender.Gender = 0;
            this.UC_Gender.Location = new System.Drawing.Point(64, 0);
            this.UC_Gender.Margin = new System.Windows.Forms.Padding(0);
            this.UC_Gender.Name = "UC_Gender";
            this.UC_Gender.Size = new System.Drawing.Size(24, 24);
            this.UC_Gender.TabIndex = 56;
            this.UC_Gender.Click += new System.EventHandler(this.ClickGender);
            // 
            // BTN_RerollPID
            // 
            this.BTN_RerollPID.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.BTN_RerollPID.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BTN_RerollPID.Location = new System.Drawing.Point(88, 0);
            this.BTN_RerollPID.Margin = new System.Windows.Forms.Padding(0);
            this.BTN_RerollPID.Name = "BTN_RerollPID";
            this.BTN_RerollPID.Size = new System.Drawing.Size(56, 24);
            this.BTN_RerollPID.TabIndex = 1;
            this.BTN_RerollPID.Text = "Reroll";
            this.BTN_RerollPID.UseVisualStyleBackColor = true;
            this.BTN_RerollPID.Click += new System.EventHandler(this.UpdateRandomPID);
            // 
            // FLP_Species
            // 
            this.FLP_Species.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_Species.Controls.Add(this.Label_Species);
            this.FLP_Species.Controls.Add(this.CB_Species);
            this.FLP_Species.Location = new System.Drawing.Point(0, 40);
            this.FLP_Species.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_Species.Name = "FLP_Species";
            this.FLP_Species.Size = new System.Drawing.Size(272, 24);
            this.FLP_Species.TabIndex = 1;
            // 
            // Label_Species
            // 
            this.Label_Species.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Label_Species.Location = new System.Drawing.Point(0, 0);
            this.Label_Species.Margin = new System.Windows.Forms.Padding(0);
            this.Label_Species.Name = "Label_Species";
            this.Label_Species.Size = new System.Drawing.Size(104, 24);
            this.Label_Species.TabIndex = 1;
            this.Label_Species.Text = "Species:";
            this.Label_Species.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Label_Species.Click += new System.EventHandler(this.UpdateNickname);
            // 
            // CB_Species
            // 
            this.CB_Species.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_Species.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Species.FormattingEnabled = true;
            this.CB_Species.Location = new System.Drawing.Point(104, 0);
            this.CB_Species.Margin = new System.Windows.Forms.Padding(0);
            this.CB_Species.Name = "CB_Species";
            this.CB_Species.Size = new System.Drawing.Size(144, 23);
            this.CB_Species.TabIndex = 3;
            this.CB_Species.SelectedIndexChanged += new System.EventHandler(this.ValidateComboBox2);
            this.CB_Species.SelectedValueChanged += new System.EventHandler(this.UpdateSpecies);
            this.CB_Species.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateComboBox);
            // 
            // FLP_Nickname
            // 
            this.FLP_Nickname.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_Nickname.Controls.Add(this.FLP_NicknameLeft);
            this.FLP_Nickname.Controls.Add(this.TB_Nickname);
            this.FLP_Nickname.Location = new System.Drawing.Point(0, 64);
            this.FLP_Nickname.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_Nickname.Name = "FLP_Nickname";
            this.FLP_Nickname.Size = new System.Drawing.Size(272, 24);
            this.FLP_Nickname.TabIndex = 2;
            // 
            // FLP_NicknameLeft
            // 
            this.FLP_NicknameLeft.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.FLP_NicknameLeft.Controls.Add(this.CHK_Nicknamed);
            this.FLP_NicknameLeft.Controls.Add(this.CHK_NicknamedFlag);
            this.FLP_NicknameLeft.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.FLP_NicknameLeft.Location = new System.Drawing.Point(0, 0);
            this.FLP_NicknameLeft.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_NicknameLeft.Name = "FLP_NicknameLeft";
            this.FLP_NicknameLeft.Size = new System.Drawing.Size(104, 24);
            this.FLP_NicknameLeft.TabIndex = 109;
            // 
            // CHK_Nicknamed
            // 
            this.CHK_Nicknamed.AutoSize = true;
            this.CHK_Nicknamed.Location = new System.Drawing.Point(40, 0);
            this.CHK_Nicknamed.Margin = new System.Windows.Forms.Padding(0);
            this.CHK_Nicknamed.MinimumSize = new System.Drawing.Size(0, 24);
            this.CHK_Nicknamed.Name = "CHK_Nicknamed";
            this.CHK_Nicknamed.Size = new System.Drawing.Size(64, 24);
            this.CHK_Nicknamed.TabIndex = 5;
            this.CHK_Nicknamed.Text = "Nickname:";
            this.CHK_Nicknamed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CHK_Nicknamed.Click += new System.EventHandler(this.CHK_Nicknamed_Click);
            // 
            // CHK_NicknamedFlag
            // 
            this.CHK_NicknamedFlag.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.CHK_NicknamedFlag.Location = new System.Drawing.Point(24, 0);
            this.CHK_NicknamedFlag.Margin = new System.Windows.Forms.Padding(0);
            this.CHK_NicknamedFlag.Name = "CHK_NicknamedFlag";
            this.CHK_NicknamedFlag.Size = new System.Drawing.Size(16, 24);
            this.CHK_NicknamedFlag.TabIndex = 4;
            this.CHK_NicknamedFlag.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CHK_NicknamedFlag.UseVisualStyleBackColor = true;
            this.CHK_NicknamedFlag.CheckedChanged += new System.EventHandler(this.UpdateNickname);
            // 
            // TB_Nickname
            // 
            this.TB_Nickname.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_Nickname.Location = new System.Drawing.Point(104, 0);
            this.TB_Nickname.Margin = new System.Windows.Forms.Padding(0);
            this.TB_Nickname.MaxLength = 12;
            this.TB_Nickname.Name = "TB_Nickname";
            this.TB_Nickname.Size = new System.Drawing.Size(144, 23);
            this.TB_Nickname.TabIndex = 5;
            this.TB_Nickname.TextChanged += new System.EventHandler(this.UpdateIsNicknamed);
            this.TB_Nickname.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UpdateNicknameClick);
            // 
            // FLP_EXPLevel
            // 
            this.FLP_EXPLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_EXPLevel.Controls.Add(this.Label_EXP);
            this.FLP_EXPLevel.Controls.Add(this.FLP_EXPLevelRight);
            this.FLP_EXPLevel.Location = new System.Drawing.Point(0, 88);
            this.FLP_EXPLevel.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_EXPLevel.Name = "FLP_EXPLevel";
            this.FLP_EXPLevel.Size = new System.Drawing.Size(272, 24);
            this.FLP_EXPLevel.TabIndex = 3;
            // 
            // Label_EXP
            // 
            this.Label_EXP.Location = new System.Drawing.Point(0, 0);
            this.Label_EXP.Margin = new System.Windows.Forms.Padding(0);
            this.Label_EXP.Name = "Label_EXP";
            this.Label_EXP.Size = new System.Drawing.Size(104, 24);
            this.Label_EXP.TabIndex = 3;
            this.Label_EXP.Text = "EXP:";
            this.Label_EXP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FLP_EXPLevelRight
            // 
            this.FLP_EXPLevelRight.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.FLP_EXPLevelRight.Controls.Add(this.TB_EXP);
            this.FLP_EXPLevelRight.Controls.Add(this.Label_CurLevel);
            this.FLP_EXPLevelRight.Controls.Add(this.TB_Level);
            this.FLP_EXPLevelRight.Location = new System.Drawing.Point(104, 0);
            this.FLP_EXPLevelRight.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_EXPLevelRight.Name = "FLP_EXPLevelRight";
            this.FLP_EXPLevelRight.Size = new System.Drawing.Size(160, 24);
            this.FLP_EXPLevelRight.TabIndex = 0;
            // 
            // TB_EXP
            // 
            this.TB_EXP.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.TB_EXP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_EXP.Location = new System.Drawing.Point(0, 0);
            this.TB_EXP.Margin = new System.Windows.Forms.Padding(0);
            this.TB_EXP.Mask = "0000000";
            this.TB_EXP.Name = "TB_EXP";
            this.TB_EXP.Size = new System.Drawing.Size(48, 23);
            this.TB_EXP.TabIndex = 7;
            this.TB_EXP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_EXP.TextChanged += new System.EventHandler(this.UpdateEXPLevel);
            // 
            // Label_CurLevel
            // 
            this.Label_CurLevel.Location = new System.Drawing.Point(48, 0);
            this.Label_CurLevel.Margin = new System.Windows.Forms.Padding(0);
            this.Label_CurLevel.Name = "Label_CurLevel";
            this.Label_CurLevel.Size = new System.Drawing.Size(72, 21);
            this.Label_CurLevel.TabIndex = 7;
            this.Label_CurLevel.Text = "Level:";
            this.Label_CurLevel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Label_CurLevel.Click += new System.EventHandler(this.ClickMetLocation);
            // 
            // TB_Level
            // 
            this.TB_Level.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_Level.Location = new System.Drawing.Point(120, 0);
            this.TB_Level.Margin = new System.Windows.Forms.Padding(0);
            this.TB_Level.Mask = "000";
            this.TB_Level.Name = "TB_Level";
            this.TB_Level.Size = new System.Drawing.Size(24, 23);
            this.TB_Level.TabIndex = 8;
            this.TB_Level.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_Level.Click += new System.EventHandler(this.ClickLevel);
            this.TB_Level.TextChanged += new System.EventHandler(this.UpdateEXPLevel);
            // 
            // FLP_Nature
            // 
            this.FLP_Nature.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_Nature.Controls.Add(this.Label_Nature);
            this.FLP_Nature.Controls.Add(this.CB_Nature);
            this.FLP_Nature.Location = new System.Drawing.Point(0, 112);
            this.FLP_Nature.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_Nature.Name = "FLP_Nature";
            this.FLP_Nature.Size = new System.Drawing.Size(272, 24);
            this.FLP_Nature.TabIndex = 4;
            // 
            // Label_Nature
            // 
            this.Label_Nature.Location = new System.Drawing.Point(0, 0);
            this.Label_Nature.Margin = new System.Windows.Forms.Padding(0);
            this.Label_Nature.Name = "Label_Nature";
            this.Label_Nature.Size = new System.Drawing.Size(104, 24);
            this.Label_Nature.TabIndex = 8;
            this.Label_Nature.Text = "Nature:";
            this.Label_Nature.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Label_Nature.Click += new System.EventHandler(this.ClickNature);
            // 
            // CB_Nature
            // 
            this.CB_Nature.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_Nature.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Nature.FormattingEnabled = true;
            this.CB_Nature.Location = new System.Drawing.Point(104, 0);
            this.CB_Nature.Margin = new System.Windows.Forms.Padding(0);
            this.CB_Nature.Name = "CB_Nature";
            this.CB_Nature.Size = new System.Drawing.Size(144, 23);
            this.CB_Nature.TabIndex = 9;
            this.CB_Nature.SelectedIndexChanged += new System.EventHandler(this.ValidateComboBox2);
            this.CB_Nature.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateComboBox);
            // 
            // FLP_OriginalNature
            // 
            this.FLP_OriginalNature.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_OriginalNature.Controls.Add(this.L_OriginalNature);
            this.FLP_OriginalNature.Controls.Add(this.CB_StatNature);
            this.FLP_OriginalNature.Location = new System.Drawing.Point(0, 136);
            this.FLP_OriginalNature.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_OriginalNature.Name = "FLP_OriginalNature";
            this.FLP_OriginalNature.Size = new System.Drawing.Size(272, 24);
            this.FLP_OriginalNature.TabIndex = 5;
            // 
            // L_OriginalNature
            // 
            this.L_OriginalNature.Location = new System.Drawing.Point(0, 0);
            this.L_OriginalNature.Margin = new System.Windows.Forms.Padding(0);
            this.L_OriginalNature.Name = "L_OriginalNature";
            this.L_OriginalNature.Size = new System.Drawing.Size(104, 24);
            this.L_OriginalNature.TabIndex = 8;
            this.L_OriginalNature.Text = "Stat Nature:";
            this.L_OriginalNature.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.L_OriginalNature.Click += new System.EventHandler(this.ClickNature);
            // 
            // CB_StatNature
            // 
            this.CB_StatNature.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_StatNature.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_StatNature.FormattingEnabled = true;
            this.CB_StatNature.Location = new System.Drawing.Point(104, 0);
            this.CB_StatNature.Margin = new System.Windows.Forms.Padding(0);
            this.CB_StatNature.Name = "CB_StatNature";
            this.CB_StatNature.Size = new System.Drawing.Size(144, 23);
            this.CB_StatNature.TabIndex = 10;
            this.CB_StatNature.SelectedIndexChanged += new System.EventHandler(this.ValidateComboBox2);
            this.CB_StatNature.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateComboBox);
            // 
            // FLP_Form
            // 
            this.FLP_Form.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_Form.Controls.Add(this.FLP_FormLeft);
            this.FLP_Form.Controls.Add(this.FLP_FormRight);
            this.FLP_Form.Location = new System.Drawing.Point(0, 160);
            this.FLP_Form.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_Form.Name = "FLP_Form";
            this.FLP_Form.Size = new System.Drawing.Size(338, 24);
            this.FLP_Form.TabIndex = 6;
            // 
            // FLP_FormLeft
            // 
            this.FLP_FormLeft.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.FLP_FormLeft.Controls.Add(this.Label_Form);
            this.FLP_FormLeft.Controls.Add(this.L_FormArgument);
            this.FLP_FormLeft.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.FLP_FormLeft.Location = new System.Drawing.Point(0, 0);
            this.FLP_FormLeft.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_FormLeft.Name = "FLP_FormLeft";
            this.FLP_FormLeft.Size = new System.Drawing.Size(104, 24);
            this.FLP_FormLeft.TabIndex = 0;
            // 
            // Label_Form
            // 
            this.Label_Form.Location = new System.Drawing.Point(0, 0);
            this.Label_Form.Margin = new System.Windows.Forms.Padding(0);
            this.Label_Form.Name = "Label_Form";
            this.Label_Form.Size = new System.Drawing.Size(104, 24);
            this.Label_Form.TabIndex = 11;
            this.Label_Form.Text = "Form:";
            this.Label_Form.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_FormArgument
            // 
            this.L_FormArgument.Location = new System.Drawing.Point(6, 24);
            this.L_FormArgument.Margin = new System.Windows.Forms.Padding(0);
            this.L_FormArgument.Name = "L_FormArgument";
            this.L_FormArgument.Size = new System.Drawing.Size(98, 21);
            this.L_FormArgument.TabIndex = 12;
            this.L_FormArgument.Text = "Form Argument:";
            this.L_FormArgument.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FLP_FormRight
            // 
            this.FLP_FormRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_FormRight.Controls.Add(this.CB_Form);
            this.FLP_FormRight.Controls.Add(this.FA_Form);
            this.FLP_FormRight.Location = new System.Drawing.Point(104, 0);
            this.FLP_FormRight.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_FormRight.Name = "FLP_FormRight";
            this.FLP_FormRight.Size = new System.Drawing.Size(224, 24);
            this.FLP_FormRight.TabIndex = 104;
            // 
            // CB_Form
            // 
            this.CB_Form.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Form.DropDownWidth = 85;
            this.CB_Form.Enabled = false;
            this.CB_Form.FormattingEnabled = true;
            this.CB_Form.Items.AddRange(new object[] {
            ""});
            this.CB_Form.Location = new System.Drawing.Point(0, 0);
            this.CB_Form.Margin = new System.Windows.Forms.Padding(0);
            this.CB_Form.Name = "CB_Form";
            this.CB_Form.Size = new System.Drawing.Size(128, 23);
            this.CB_Form.TabIndex = 12;
            this.CB_Form.SelectedIndexChanged += new System.EventHandler(this.UpdateForm);
            // 
            // FA_Form
            // 
            this.FA_Form.Location = new System.Drawing.Point(128, 0);
            this.FA_Form.Margin = new System.Windows.Forms.Padding(0);
            this.FA_Form.Name = "FA_Form";
            this.FA_Form.Size = new System.Drawing.Size(80, 24);
            this.FA_Form.TabIndex = 19;
            this.FA_Form.ValueChanged += new System.EventHandler(this.UpdateFormArgument);
            // 
            // FLP_HeldItem
            // 
            this.FLP_HeldItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_HeldItem.Controls.Add(this.Label_HeldItem);
            this.FLP_HeldItem.Controls.Add(this.CB_HeldItem);
            this.FLP_HeldItem.Location = new System.Drawing.Point(0, 184);
            this.FLP_HeldItem.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_HeldItem.Name = "FLP_HeldItem";
            this.FLP_HeldItem.Size = new System.Drawing.Size(272, 24);
            this.FLP_HeldItem.TabIndex = 7;
            // 
            // Label_HeldItem
            // 
            this.Label_HeldItem.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Label_HeldItem.Location = new System.Drawing.Point(0, 0);
            this.Label_HeldItem.Margin = new System.Windows.Forms.Padding(0);
            this.Label_HeldItem.Name = "Label_HeldItem";
            this.Label_HeldItem.Size = new System.Drawing.Size(104, 24);
            this.Label_HeldItem.TabIndex = 51;
            this.Label_HeldItem.Text = "Held Item:";
            this.Label_HeldItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_HeldItem
            // 
            this.CB_HeldItem.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_HeldItem.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_HeldItem.FormattingEnabled = true;
            this.CB_HeldItem.Location = new System.Drawing.Point(104, 0);
            this.CB_HeldItem.Margin = new System.Windows.Forms.Padding(0);
            this.CB_HeldItem.Name = "CB_HeldItem";
            this.CB_HeldItem.Size = new System.Drawing.Size(144, 23);
            this.CB_HeldItem.TabIndex = 10;
            this.CB_HeldItem.SelectedIndexChanged += new System.EventHandler(this.ValidateComboBox2);
            this.CB_HeldItem.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateComboBox);
            // 
            // FLP_Ability
            // 
            this.FLP_Ability.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_Ability.Controls.Add(this.Label_Ability);
            this.FLP_Ability.Controls.Add(this.FLP_AbilityRight);
            this.FLP_Ability.Location = new System.Drawing.Point(0, 208);
            this.FLP_Ability.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_Ability.Name = "FLP_Ability";
            this.FLP_Ability.Size = new System.Drawing.Size(272, 24);
            this.FLP_Ability.TabIndex = 8;
            // 
            // Label_Ability
            // 
            this.Label_Ability.Location = new System.Drawing.Point(0, 0);
            this.Label_Ability.Margin = new System.Windows.Forms.Padding(0);
            this.Label_Ability.Name = "Label_Ability";
            this.Label_Ability.Size = new System.Drawing.Size(104, 24);
            this.Label_Ability.TabIndex = 10;
            this.Label_Ability.Text = "Ability:";
            this.Label_Ability.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FLP_AbilityRight
            // 
            this.FLP_AbilityRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_AbilityRight.Controls.Add(this.CB_Ability);
            this.FLP_AbilityRight.Controls.Add(this.DEV_Ability);
            this.FLP_AbilityRight.Controls.Add(this.TB_AbilityNumber);
            this.FLP_AbilityRight.Location = new System.Drawing.Point(104, 0);
            this.FLP_AbilityRight.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_AbilityRight.Name = "FLP_AbilityRight";
            this.FLP_AbilityRight.Size = new System.Drawing.Size(160, 24);
            this.FLP_AbilityRight.TabIndex = 109;
            // 
            // CB_Ability
            // 
            this.CB_Ability.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_Ability.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Ability.FormattingEnabled = true;
            this.CB_Ability.Location = new System.Drawing.Point(0, 0);
            this.CB_Ability.Margin = new System.Windows.Forms.Padding(0);
            this.CB_Ability.Name = "CB_Ability";
            this.CB_Ability.Size = new System.Drawing.Size(144, 23);
            this.CB_Ability.TabIndex = 13;
            this.CB_Ability.SelectedIndexChanged += new System.EventHandler(this.ValidateComboBox2);
            this.CB_Ability.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateComboBox);
            // 
            // DEV_Ability
            // 
            this.DEV_Ability.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.DEV_Ability.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.DEV_Ability.Enabled = false;
            this.DEV_Ability.FormattingEnabled = true;
            this.DEV_Ability.Location = new System.Drawing.Point(0, 23);
            this.DEV_Ability.Margin = new System.Windows.Forms.Padding(0);
            this.DEV_Ability.Name = "DEV_Ability";
            this.DEV_Ability.Size = new System.Drawing.Size(126, 23);
            this.DEV_Ability.TabIndex = 14;
            this.DEV_Ability.Visible = false;
            // 
            // TB_AbilityNumber
            // 
            this.TB_AbilityNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_AbilityNumber.Location = new System.Drawing.Point(126, 23);
            this.TB_AbilityNumber.Margin = new System.Windows.Forms.Padding(0);
            this.TB_AbilityNumber.Mask = "0";
            this.TB_AbilityNumber.Name = "TB_AbilityNumber";
            this.TB_AbilityNumber.Size = new System.Drawing.Size(19, 23);
            this.TB_AbilityNumber.TabIndex = 14;
            this.TB_AbilityNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_AbilityNumber.Visible = false;
            // 
            // FLP_Friendship
            // 
            this.FLP_Friendship.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_Friendship.Controls.Add(this.FLP_FriendshipLeft);
            this.FLP_Friendship.Controls.Add(this.FLP_FriendshipRight);
            this.FLP_Friendship.Location = new System.Drawing.Point(0, 232);
            this.FLP_Friendship.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_Friendship.Name = "FLP_Friendship";
            this.FLP_Friendship.Size = new System.Drawing.Size(272, 24);
            this.FLP_Friendship.TabIndex = 9;
            // 
            // FLP_FriendshipLeft
            // 
            this.FLP_FriendshipLeft.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.FLP_FriendshipLeft.Controls.Add(this.Label_Friendship);
            this.FLP_FriendshipLeft.Controls.Add(this.Label_HatchCounter);
            this.FLP_FriendshipLeft.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.FLP_FriendshipLeft.Location = new System.Drawing.Point(0, 0);
            this.FLP_FriendshipLeft.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_FriendshipLeft.Name = "FLP_FriendshipLeft";
            this.FLP_FriendshipLeft.Size = new System.Drawing.Size(104, 24);
            this.FLP_FriendshipLeft.TabIndex = 0;
            // 
            // Label_Friendship
            // 
            this.Label_Friendship.Location = new System.Drawing.Point(0, 0);
            this.Label_Friendship.Margin = new System.Windows.Forms.Padding(0);
            this.Label_Friendship.Name = "Label_Friendship";
            this.Label_Friendship.Size = new System.Drawing.Size(104, 24);
            this.Label_Friendship.TabIndex = 9;
            this.Label_Friendship.Text = "Friendship:";
            this.Label_Friendship.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Label_Friendship.Click += new System.EventHandler(this.ClickFriendship);
            // 
            // Label_HatchCounter
            // 
            this.Label_HatchCounter.Location = new System.Drawing.Point(-6, 24);
            this.Label_HatchCounter.Margin = new System.Windows.Forms.Padding(0);
            this.Label_HatchCounter.Name = "Label_HatchCounter";
            this.Label_HatchCounter.Size = new System.Drawing.Size(110, 21);
            this.Label_HatchCounter.TabIndex = 61;
            this.Label_HatchCounter.Text = "Hatch Counter:";
            this.Label_HatchCounter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Label_HatchCounter.Click += new System.EventHandler(this.ClickFriendship);
            // 
            // FLP_FriendshipRight
            // 
            this.FLP_FriendshipRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_FriendshipRight.Controls.Add(this.TB_Friendship);
            this.FLP_FriendshipRight.Location = new System.Drawing.Point(104, 0);
            this.FLP_FriendshipRight.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_FriendshipRight.Name = "FLP_FriendshipRight";
            this.FLP_FriendshipRight.Size = new System.Drawing.Size(160, 24);
            this.FLP_FriendshipRight.TabIndex = 104;
            // 
            // TB_Friendship
            // 
            this.TB_Friendship.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_Friendship.Location = new System.Drawing.Point(0, 0);
            this.TB_Friendship.Margin = new System.Windows.Forms.Padding(0);
            this.TB_Friendship.Mask = "000";
            this.TB_Friendship.Name = "TB_Friendship";
            this.TB_Friendship.Size = new System.Drawing.Size(24, 23);
            this.TB_Friendship.TabIndex = 11;
            this.TB_Friendship.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_Friendship.Validated += new System.EventHandler(this.Update255_MTB);
            // 
            // FLP_Language
            // 
            this.FLP_Language.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_Language.Controls.Add(this.Label_Language);
            this.FLP_Language.Controls.Add(this.CB_Language);
            this.FLP_Language.Location = new System.Drawing.Point(0, 256);
            this.FLP_Language.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_Language.Name = "FLP_Language";
            this.FLP_Language.Size = new System.Drawing.Size(272, 24);
            this.FLP_Language.TabIndex = 10;
            // 
            // Label_Language
            // 
            this.Label_Language.Location = new System.Drawing.Point(0, 0);
            this.Label_Language.Margin = new System.Windows.Forms.Padding(0);
            this.Label_Language.Name = "Label_Language";
            this.Label_Language.Size = new System.Drawing.Size(104, 24);
            this.Label_Language.TabIndex = 12;
            this.Label_Language.Text = "Language:";
            this.Label_Language.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_Language
            // 
            this.CB_Language.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CB_Language.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Language.FormattingEnabled = true;
            this.CB_Language.Location = new System.Drawing.Point(104, 0);
            this.CB_Language.Margin = new System.Windows.Forms.Padding(0);
            this.CB_Language.Name = "CB_Language";
            this.CB_Language.Size = new System.Drawing.Size(144, 23);
            this.CB_Language.TabIndex = 15;
            this.CB_Language.SelectedIndexChanged += new System.EventHandler(this.UpdateNickname);
            // 
            // FLP_EggPKRS
            // 
            this.FLP_EggPKRS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_EggPKRS.Controls.Add(this.FLP_EggPKRSLeft);
            this.FLP_EggPKRS.Controls.Add(this.FLP_EggPKRSRight);
            this.FLP_EggPKRS.Location = new System.Drawing.Point(0, 280);
            this.FLP_EggPKRS.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_EggPKRS.Name = "FLP_EggPKRS";
            this.FLP_EggPKRS.Size = new System.Drawing.Size(272, 24);
            this.FLP_EggPKRS.TabIndex = 11;
            // 
            // FLP_EggPKRSLeft
            // 
            this.FLP_EggPKRSLeft.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.FLP_EggPKRSLeft.Controls.Add(this.CHK_IsEgg);
            this.FLP_EggPKRSLeft.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.FLP_EggPKRSLeft.Location = new System.Drawing.Point(0, 0);
            this.FLP_EggPKRSLeft.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_EggPKRSLeft.Name = "FLP_EggPKRSLeft";
            this.FLP_EggPKRSLeft.Size = new System.Drawing.Size(104, 24);
            this.FLP_EggPKRSLeft.TabIndex = 0;
            // 
            // CHK_IsEgg
            // 
            this.CHK_IsEgg.AutoSize = true;
            this.CHK_IsEgg.Location = new System.Drawing.Point(47, 3);
            this.CHK_IsEgg.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.CHK_IsEgg.Name = "CHK_IsEgg";
            this.CHK_IsEgg.Size = new System.Drawing.Size(57, 19);
            this.CHK_IsEgg.TabIndex = 16;
            this.CHK_IsEgg.Text = "Is Egg";
            this.CHK_IsEgg.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CHK_IsEgg.UseVisualStyleBackColor = true;
            this.CHK_IsEgg.CheckedChanged += new System.EventHandler(this.UpdateIsEgg);
            // 
            // FLP_EggPKRSRight
            // 
            this.FLP_EggPKRSRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_EggPKRSRight.Controls.Add(this.CHK_Infected);
            this.FLP_EggPKRSRight.Controls.Add(this.CHK_Cured);
            this.FLP_EggPKRSRight.Location = new System.Drawing.Point(104, 0);
            this.FLP_EggPKRSRight.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_EggPKRSRight.Name = "FLP_EggPKRSRight";
            this.FLP_EggPKRSRight.Size = new System.Drawing.Size(160, 24);
            this.FLP_EggPKRSRight.TabIndex = 104;
            // 
            // CHK_Infected
            // 
            this.CHK_Infected.AutoSize = true;
            this.CHK_Infected.Location = new System.Drawing.Point(0, 3);
            this.CHK_Infected.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.CHK_Infected.Name = "CHK_Infected";
            this.CHK_Infected.Size = new System.Drawing.Size(69, 19);
            this.CHK_Infected.TabIndex = 17;
            this.CHK_Infected.Text = "Infected";
            this.CHK_Infected.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CHK_Infected.UseVisualStyleBackColor = true;
            this.CHK_Infected.CheckedChanged += new System.EventHandler(this.UpdatePKRSInfected);
            // 
            // CHK_Cured
            // 
            this.CHK_Cured.AutoSize = true;
            this.CHK_Cured.Location = new System.Drawing.Point(69, 3);
            this.CHK_Cured.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.CHK_Cured.Name = "CHK_Cured";
            this.CHK_Cured.Size = new System.Drawing.Size(58, 19);
            this.CHK_Cured.TabIndex = 18;
            this.CHK_Cured.Text = "Cured";
            this.CHK_Cured.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CHK_Cured.UseVisualStyleBackColor = true;
            this.CHK_Cured.CheckedChanged += new System.EventHandler(this.UpdatePKRSCured);
            // 
            // FLP_PKRS
            // 
            this.FLP_PKRS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_PKRS.Controls.Add(this.Label_PKRS);
            this.FLP_PKRS.Controls.Add(this.FLP_PKRSRight);
            this.FLP_PKRS.Location = new System.Drawing.Point(0, 304);
            this.FLP_PKRS.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_PKRS.Name = "FLP_PKRS";
            this.FLP_PKRS.Size = new System.Drawing.Size(272, 24);
            this.FLP_PKRS.TabIndex = 12;
            // 
            // Label_PKRS
            // 
            this.Label_PKRS.Location = new System.Drawing.Point(0, 0);
            this.Label_PKRS.Margin = new System.Windows.Forms.Padding(0);
            this.Label_PKRS.Name = "Label_PKRS";
            this.Label_PKRS.Size = new System.Drawing.Size(104, 24);
            this.Label_PKRS.TabIndex = 14;
            this.Label_PKRS.Text = "PkRs:";
            this.Label_PKRS.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Label_PKRS.Visible = false;
            // 
            // FLP_PKRSRight
            // 
            this.FLP_PKRSRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_PKRSRight.Controls.Add(this.CB_PKRSStrain);
            this.FLP_PKRSRight.Controls.Add(this.Label_PKRSdays);
            this.FLP_PKRSRight.Controls.Add(this.CB_PKRSDays);
            this.FLP_PKRSRight.Location = new System.Drawing.Point(104, 0);
            this.FLP_PKRSRight.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_PKRSRight.Name = "FLP_PKRSRight";
            this.FLP_PKRSRight.Size = new System.Drawing.Size(160, 24);
            this.FLP_PKRSRight.TabIndex = 105;
            // 
            // CB_PKRSStrain
            // 
            this.CB_PKRSStrain.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_PKRSStrain.FormattingEnabled = true;
            this.CB_PKRSStrain.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15"});
            this.CB_PKRSStrain.Location = new System.Drawing.Point(0, 0);
            this.CB_PKRSStrain.Margin = new System.Windows.Forms.Padding(0);
            this.CB_PKRSStrain.Name = "CB_PKRSStrain";
            this.CB_PKRSStrain.Size = new System.Drawing.Size(40, 23);
            this.CB_PKRSStrain.TabIndex = 19;
            this.CB_PKRSStrain.Visible = false;
            this.CB_PKRSStrain.SelectedValueChanged += new System.EventHandler(this.UpdatePKRSstrain);
            // 
            // Label_PKRSdays
            // 
            this.Label_PKRSdays.Location = new System.Drawing.Point(40, 0);
            this.Label_PKRSdays.Margin = new System.Windows.Forms.Padding(0);
            this.Label_PKRSdays.Name = "Label_PKRSdays";
            this.Label_PKRSdays.Size = new System.Drawing.Size(25, 21);
            this.Label_PKRSdays.TabIndex = 15;
            this.Label_PKRSdays.Text = "d:";
            this.Label_PKRSdays.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Label_PKRSdays.Visible = false;
            // 
            // CB_PKRSDays
            // 
            this.CB_PKRSDays.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_PKRSDays.FormattingEnabled = true;
            this.CB_PKRSDays.Location = new System.Drawing.Point(65, 0);
            this.CB_PKRSDays.Margin = new System.Windows.Forms.Padding(0);
            this.CB_PKRSDays.Name = "CB_PKRSDays";
            this.CB_PKRSDays.Size = new System.Drawing.Size(32, 23);
            this.CB_PKRSDays.TabIndex = 20;
            this.CB_PKRSDays.Visible = false;
            this.CB_PKRSDays.SelectedIndexChanged += new System.EventHandler(this.UpdatePKRSdays);
            // 
            // FLP_NSparkle
            // 
            this.FLP_NSparkle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_NSparkle.Controls.Add(this.L_NSparkle);
            this.FLP_NSparkle.Controls.Add(this.CHK_NSparkle);
            this.FLP_NSparkle.Location = new System.Drawing.Point(0, 328);
            this.FLP_NSparkle.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_NSparkle.Name = "FLP_NSparkle";
            this.FLP_NSparkle.Size = new System.Drawing.Size(272, 24);
            this.FLP_NSparkle.TabIndex = 16;
            // 
            // L_NSparkle
            // 
            this.L_NSparkle.Location = new System.Drawing.Point(0, 0);
            this.L_NSparkle.Margin = new System.Windows.Forms.Padding(0);
            this.L_NSparkle.Name = "L_NSparkle";
            this.L_NSparkle.Size = new System.Drawing.Size(104, 24);
            this.L_NSparkle.TabIndex = 17;
            this.L_NSparkle.Text = "N\'s Sparkle:";
            this.L_NSparkle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CHK_NSparkle
            // 
            this.CHK_NSparkle.AutoSize = true;
            this.CHK_NSparkle.Location = new System.Drawing.Point(104, 3);
            this.CHK_NSparkle.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.CHK_NSparkle.Name = "CHK_NSparkle";
            this.CHK_NSparkle.Size = new System.Drawing.Size(59, 19);
            this.CHK_NSparkle.TabIndex = 18;
            this.CHK_NSparkle.Text = "Active";
            this.CHK_NSparkle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CHK_NSparkle.UseVisualStyleBackColor = true;
            // 
            // FLP_ShadowID
            // 
            this.FLP_ShadowID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_ShadowID.Controls.Add(this.L_ShadowID);
            this.FLP_ShadowID.Controls.Add(this.NUD_ShadowID);
            this.FLP_ShadowID.Location = new System.Drawing.Point(0, 352);
            this.FLP_ShadowID.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_ShadowID.Name = "FLP_ShadowID";
            this.FLP_ShadowID.Size = new System.Drawing.Size(272, 24);
            this.FLP_ShadowID.TabIndex = 17;
            // 
            // L_ShadowID
            // 
            this.L_ShadowID.Location = new System.Drawing.Point(0, 0);
            this.L_ShadowID.Margin = new System.Windows.Forms.Padding(0);
            this.L_ShadowID.Name = "L_ShadowID";
            this.L_ShadowID.Size = new System.Drawing.Size(104, 24);
            this.L_ShadowID.TabIndex = 9;
            this.L_ShadowID.Text = "Shadow ID:";
            this.L_ShadowID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_ShadowID
            // 
            this.NUD_ShadowID.Location = new System.Drawing.Point(104, 1);
            this.NUD_ShadowID.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.NUD_ShadowID.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
            this.NUD_ShadowID.Name = "NUD_ShadowID";
            this.NUD_ShadowID.Size = new System.Drawing.Size(48, 23);
            this.NUD_ShadowID.TabIndex = 103;
            this.NUD_ShadowID.ValueChanged += new System.EventHandler(this.UpdateShadowID);
            // 
            // FLP_Purification
            // 
            this.FLP_Purification.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_Purification.Controls.Add(this.L_HeartGauge);
            this.FLP_Purification.Controls.Add(this.NUD_Purification);
            this.FLP_Purification.Controls.Add(this.CHK_Shadow);
            this.FLP_Purification.Location = new System.Drawing.Point(0, 376);
            this.FLP_Purification.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_Purification.Name = "FLP_Purification";
            this.FLP_Purification.Size = new System.Drawing.Size(272, 24);
            this.FLP_Purification.TabIndex = 18;
            // 
            // L_HeartGauge
            // 
            this.L_HeartGauge.Location = new System.Drawing.Point(0, 0);
            this.L_HeartGauge.Margin = new System.Windows.Forms.Padding(0);
            this.L_HeartGauge.Name = "L_HeartGauge";
            this.L_HeartGauge.Size = new System.Drawing.Size(104, 24);
            this.L_HeartGauge.TabIndex = 9;
            this.L_HeartGauge.Text = "Heart Gauge:";
            this.L_HeartGauge.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_Purification
            // 
            this.NUD_Purification.Location = new System.Drawing.Point(104, 1);
            this.NUD_Purification.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.NUD_Purification.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.NUD_Purification.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.NUD_Purification.Name = "NUD_Purification";
            this.NUD_Purification.Size = new System.Drawing.Size(48, 23);
            this.NUD_Purification.TabIndex = 103;
            this.NUD_Purification.ValueChanged += new System.EventHandler(this.UpdatePurification);
            // 
            // CHK_Shadow
            // 
            this.CHK_Shadow.AutoSize = true;
            this.CHK_Shadow.Location = new System.Drawing.Point(152, 3);
            this.CHK_Shadow.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.CHK_Shadow.Name = "CHK_Shadow";
            this.CHK_Shadow.Size = new System.Drawing.Size(68, 19);
            this.CHK_Shadow.TabIndex = 16;
            this.CHK_Shadow.Text = "Shadow";
            this.CHK_Shadow.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CHK_Shadow.UseVisualStyleBackColor = true;
            this.CHK_Shadow.CheckedChanged += new System.EventHandler(this.UpdateShadowCHK);
            // 
            // FLP_CatchRate
            // 
            this.FLP_CatchRate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_CatchRate.Controls.Add(this.L_CatchRate);
            this.FLP_CatchRate.Controls.Add(this.CR_PK1);
            this.FLP_CatchRate.Location = new System.Drawing.Point(0, 400);
            this.FLP_CatchRate.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_CatchRate.Name = "FLP_CatchRate";
            this.FLP_CatchRate.Size = new System.Drawing.Size(288, 24);
            this.FLP_CatchRate.TabIndex = 20;
            // 
            // L_CatchRate
            // 
            this.L_CatchRate.Location = new System.Drawing.Point(0, 0);
            this.L_CatchRate.Margin = new System.Windows.Forms.Padding(0);
            this.L_CatchRate.Name = "L_CatchRate";
            this.L_CatchRate.Size = new System.Drawing.Size(104, 24);
            this.L_CatchRate.TabIndex = 9;
            this.L_CatchRate.Text = "Catch Rate:";
            this.L_CatchRate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CR_PK1
            // 
            this.CR_PK1.Location = new System.Drawing.Point(104, 0);
            this.CR_PK1.Margin = new System.Windows.Forms.Padding(0);
            this.CR_PK1.Name = "CR_PK1";
            this.CR_PK1.Size = new System.Drawing.Size(184, 25);
            this.CR_PK1.TabIndex = 10;
            // 
            // Hidden_Met
            // 
            this.Hidden_Met.AllowDrop = true;
            this.Hidden_Met.Controls.Add(this.CHK_AsEgg);
            this.Hidden_Met.Controls.Add(this.GB_EggConditions);
            this.Hidden_Met.Controls.Add(this.FLP_Met);
            this.Hidden_Met.Location = new System.Drawing.Point(4, 5);
            this.Hidden_Met.Margin = new System.Windows.Forms.Padding(0);
            this.Hidden_Met.Name = "Hidden_Met";
            this.Hidden_Met.Padding = new System.Windows.Forms.Padding(0, 16, 0, 0);
            this.Hidden_Met.Size = new System.Drawing.Size(296, 391);
            this.Hidden_Met.TabIndex = 1;
            this.Hidden_Met.Text = "Met";
            this.Hidden_Met.UseVisualStyleBackColor = true;
            // 
            // CHK_AsEgg
            // 
            this.CHK_AsEgg.AutoSize = true;
            this.CHK_AsEgg.Location = new System.Drawing.Point(104, 216);
            this.CHK_AsEgg.Name = "CHK_AsEgg";
            this.CHK_AsEgg.Size = new System.Drawing.Size(62, 19);
            this.CHK_AsEgg.TabIndex = 10;
            this.CHK_AsEgg.Text = "As Egg";
            this.CHK_AsEgg.UseVisualStyleBackColor = true;
            this.CHK_AsEgg.Click += new System.EventHandler(this.UpdateMetAsEgg);
            // 
            // GB_EggConditions
            // 
            this.GB_EggConditions.Controls.Add(this.CB_EggLocation);
            this.GB_EggConditions.Controls.Add(this.CAL_EggDate);
            this.GB_EggConditions.Controls.Add(this.Label_EggDate);
            this.GB_EggConditions.Controls.Add(this.Label_EggLocation);
            this.GB_EggConditions.Enabled = false;
            this.GB_EggConditions.Location = new System.Drawing.Point(32, 232);
            this.GB_EggConditions.Name = "GB_EggConditions";
            this.GB_EggConditions.Size = new System.Drawing.Size(240, 72);
            this.GB_EggConditions.TabIndex = 3;
            this.GB_EggConditions.TabStop = false;
            this.GB_EggConditions.Text = "Egg Met Conditions";
            // 
            // CB_EggLocation
            // 
            this.CB_EggLocation.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_EggLocation.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_EggLocation.FormattingEnabled = true;
            this.CB_EggLocation.Location = new System.Drawing.Point(72, 19);
            this.CB_EggLocation.Name = "CB_EggLocation";
            this.CB_EggLocation.Size = new System.Drawing.Size(160, 23);
            this.CB_EggLocation.TabIndex = 4;
            this.CB_EggLocation.SelectedIndexChanged += new System.EventHandler(this.ValidateLocation);
            this.CB_EggLocation.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateComboBox);
            // 
            // CAL_EggDate
            // 
            this.CAL_EggDate.CustomFormat = "MM/dd/yyyy";
            this.CAL_EggDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.CAL_EggDate.Location = new System.Drawing.Point(72, 40);
            this.CAL_EggDate.MaxDate = new System.DateTime(2099, 12, 31, 0, 0, 0, 0);
            this.CAL_EggDate.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.CAL_EggDate.Name = "CAL_EggDate";
            this.CAL_EggDate.Size = new System.Drawing.Size(136, 23);
            this.CAL_EggDate.TabIndex = 5;
            this.CAL_EggDate.Value = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            // 
            // Label_EggDate
            // 
            this.Label_EggDate.Location = new System.Drawing.Point(8, 44);
            this.Label_EggDate.Name = "Label_EggDate";
            this.Label_EggDate.Size = new System.Drawing.Size(64, 13);
            this.Label_EggDate.TabIndex = 8;
            this.Label_EggDate.Text = "Date:";
            this.Label_EggDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_EggLocation
            // 
            this.Label_EggLocation.Location = new System.Drawing.Point(8, 24);
            this.Label_EggLocation.Name = "Label_EggLocation";
            this.Label_EggLocation.Size = new System.Drawing.Size(64, 13);
            this.Label_EggLocation.TabIndex = 6;
            this.Label_EggLocation.Text = "Location:";
            this.Label_EggLocation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FLP_Met
            // 
            this.FLP_Met.Controls.Add(this.FLP_OriginGame);
            this.FLP_Met.Controls.Add(this.FLP_BattleVersion);
            this.FLP_Met.Controls.Add(this.FLP_MetLocation);
            this.FLP_Met.Controls.Add(this.FLP_Ball);
            this.FLP_Met.Controls.Add(this.FLP_MetDate);
            this.FLP_Met.Controls.Add(this.FLP_MetLevel);
            this.FLP_Met.Controls.Add(this.FLP_ObedienceLevel);
            this.FLP_Met.Controls.Add(this.FLP_GroundTile);
            this.FLP_Met.Controls.Add(this.FLP_TimeOfDay);
            this.FLP_Met.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FLP_Met.Location = new System.Drawing.Point(0, 16);
            this.FLP_Met.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_Met.Name = "FLP_Met";
            this.FLP_Met.Size = new System.Drawing.Size(296, 375);
            this.FLP_Met.TabIndex = 1;
            // 
            // FLP_OriginGame
            // 
            this.FLP_OriginGame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_OriginGame.Controls.Add(this.Label_OriginGame);
            this.FLP_OriginGame.Controls.Add(this.CB_GameOrigin);
            this.FLP_OriginGame.Location = new System.Drawing.Point(0, 0);
            this.FLP_OriginGame.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_OriginGame.Name = "FLP_OriginGame";
            this.FLP_OriginGame.Size = new System.Drawing.Size(280, 24);
            this.FLP_OriginGame.TabIndex = 1;
            // 
            // Label_OriginGame
            // 
            this.Label_OriginGame.Location = new System.Drawing.Point(0, 0);
            this.Label_OriginGame.Margin = new System.Windows.Forms.Padding(0);
            this.Label_OriginGame.Name = "Label_OriginGame";
            this.Label_OriginGame.Size = new System.Drawing.Size(104, 24);
            this.Label_OriginGame.TabIndex = 0;
            this.Label_OriginGame.Text = "Origin Game:";
            this.Label_OriginGame.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_GameOrigin
            // 
            this.CB_GameOrigin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_GameOrigin.FormattingEnabled = true;
            this.CB_GameOrigin.Location = new System.Drawing.Point(104, 0);
            this.CB_GameOrigin.Margin = new System.Windows.Forms.Padding(0);
            this.CB_GameOrigin.Name = "CB_GameOrigin";
            this.CB_GameOrigin.Size = new System.Drawing.Size(136, 23);
            this.CB_GameOrigin.TabIndex = 1;
            this.CB_GameOrigin.SelectedIndexChanged += new System.EventHandler(this.UpdateOriginGame);
            // 
            // FLP_BattleVersion
            // 
            this.FLP_BattleVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_BattleVersion.Controls.Add(this.L_BattleVersion);
            this.FLP_BattleVersion.Controls.Add(this.CB_BattleVersion);
            this.FLP_BattleVersion.Location = new System.Drawing.Point(0, 24);
            this.FLP_BattleVersion.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_BattleVersion.Name = "FLP_BattleVersion";
            this.FLP_BattleVersion.Size = new System.Drawing.Size(280, 24);
            this.FLP_BattleVersion.TabIndex = 2;
            // 
            // L_BattleVersion
            // 
            this.L_BattleVersion.Location = new System.Drawing.Point(0, 0);
            this.L_BattleVersion.Margin = new System.Windows.Forms.Padding(0);
            this.L_BattleVersion.Name = "L_BattleVersion";
            this.L_BattleVersion.Size = new System.Drawing.Size(104, 24);
            this.L_BattleVersion.TabIndex = 0;
            this.L_BattleVersion.Text = "Battle Version:";
            this.L_BattleVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_BattleVersion
            // 
            this.CB_BattleVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_BattleVersion.FormattingEnabled = true;
            this.CB_BattleVersion.Location = new System.Drawing.Point(104, 0);
            this.CB_BattleVersion.Margin = new System.Windows.Forms.Padding(0);
            this.CB_BattleVersion.Name = "CB_BattleVersion";
            this.CB_BattleVersion.Size = new System.Drawing.Size(136, 23);
            this.CB_BattleVersion.TabIndex = 1;
            this.CB_BattleVersion.SelectedValueChanged += new System.EventHandler(this.CB_BattleVersion_SelectedValueChanged);
            // 
            // FLP_MetLocation
            // 
            this.FLP_MetLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_MetLocation.Controls.Add(this.Label_MetLocation);
            this.FLP_MetLocation.Controls.Add(this.CB_MetLocation);
            this.FLP_MetLocation.Location = new System.Drawing.Point(0, 48);
            this.FLP_MetLocation.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_MetLocation.Name = "FLP_MetLocation";
            this.FLP_MetLocation.Size = new System.Drawing.Size(280, 24);
            this.FLP_MetLocation.TabIndex = 3;
            // 
            // Label_MetLocation
            // 
            this.Label_MetLocation.Location = new System.Drawing.Point(0, 0);
            this.Label_MetLocation.Margin = new System.Windows.Forms.Padding(0);
            this.Label_MetLocation.Name = "Label_MetLocation";
            this.Label_MetLocation.Size = new System.Drawing.Size(104, 24);
            this.Label_MetLocation.TabIndex = 1;
            this.Label_MetLocation.Text = "Met Location:";
            this.Label_MetLocation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Label_MetLocation.Click += new System.EventHandler(this.ClickMetLocation);
            // 
            // CB_MetLocation
            // 
            this.CB_MetLocation.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_MetLocation.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_MetLocation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CB_MetLocation.FormattingEnabled = true;
            this.CB_MetLocation.Location = new System.Drawing.Point(104, 0);
            this.CB_MetLocation.Margin = new System.Windows.Forms.Padding(0);
            this.CB_MetLocation.Name = "CB_MetLocation";
            this.CB_MetLocation.Size = new System.Drawing.Size(160, 23);
            this.CB_MetLocation.TabIndex = 2;
            this.CB_MetLocation.SelectedIndexChanged += new System.EventHandler(this.ValidateLocation);
            this.CB_MetLocation.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateComboBox);
            // 
            // FLP_Ball
            // 
            this.FLP_Ball.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_Ball.Controls.Add(this.FLP_BallLeft);
            this.FLP_Ball.Controls.Add(this.CB_Ball);
            this.FLP_Ball.Location = new System.Drawing.Point(0, 72);
            this.FLP_Ball.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_Ball.Name = "FLP_Ball";
            this.FLP_Ball.Size = new System.Drawing.Size(280, 24);
            this.FLP_Ball.TabIndex = 4;
            // 
            // FLP_BallLeft
            // 
            this.FLP_BallLeft.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.FLP_BallLeft.Controls.Add(this.Label_Ball);
            this.FLP_BallLeft.Controls.Add(this.PB_Ball);
            this.FLP_BallLeft.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.FLP_BallLeft.Location = new System.Drawing.Point(0, 0);
            this.FLP_BallLeft.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_BallLeft.Name = "FLP_BallLeft";
            this.FLP_BallLeft.Size = new System.Drawing.Size(104, 24);
            this.FLP_BallLeft.TabIndex = 4;
            this.FLP_BallLeft.Click += new System.EventHandler(this.ClickBall);
            // 
            // Label_Ball
            // 
            this.Label_Ball.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Label_Ball.AutoSize = true;
            this.Label_Ball.Location = new System.Drawing.Point(75, 1);
            this.Label_Ball.Margin = new System.Windows.Forms.Padding(0);
            this.Label_Ball.Name = "Label_Ball";
            this.Label_Ball.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.Label_Ball.Size = new System.Drawing.Size(29, 21);
            this.Label_Ball.TabIndex = 2;
            this.Label_Ball.Text = "Ball:";
            this.Label_Ball.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Label_Ball.Click += new System.EventHandler(this.ClickBall);
            // 
            // PB_Ball
            // 
            this.PB_Ball.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.PB_Ball.Location = new System.Drawing.Point(48, 0);
            this.PB_Ball.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.PB_Ball.Name = "PB_Ball";
            this.PB_Ball.Size = new System.Drawing.Size(24, 24);
            this.PB_Ball.TabIndex = 3;
            this.PB_Ball.TabStop = false;
            this.PB_Ball.Click += new System.EventHandler(this.ClickBall);
            // 
            // CB_Ball
            // 
            this.CB_Ball.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_Ball.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Ball.FormattingEnabled = true;
            this.CB_Ball.Location = new System.Drawing.Point(104, 0);
            this.CB_Ball.Margin = new System.Windows.Forms.Padding(0);
            this.CB_Ball.Name = "CB_Ball";
            this.CB_Ball.Size = new System.Drawing.Size(136, 23);
            this.CB_Ball.TabIndex = 3;
            this.CB_Ball.SelectedIndexChanged += new System.EventHandler(this.ValidateComboBox2);
            this.CB_Ball.SelectedValueChanged += new System.EventHandler(this.UpdateBall);
            this.CB_Ball.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateComboBox);
            // 
            // FLP_MetDate
            // 
            this.FLP_MetDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_MetDate.Controls.Add(this.Label_MetDate);
            this.FLP_MetDate.Controls.Add(this.CAL_MetDate);
            this.FLP_MetDate.Location = new System.Drawing.Point(0, 96);
            this.FLP_MetDate.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_MetDate.Name = "FLP_MetDate";
            this.FLP_MetDate.Size = new System.Drawing.Size(280, 24);
            this.FLP_MetDate.TabIndex = 5;
            // 
            // Label_MetDate
            // 
            this.Label_MetDate.Location = new System.Drawing.Point(0, 0);
            this.Label_MetDate.Margin = new System.Windows.Forms.Padding(0);
            this.Label_MetDate.Name = "Label_MetDate";
            this.Label_MetDate.Size = new System.Drawing.Size(104, 24);
            this.Label_MetDate.TabIndex = 4;
            this.Label_MetDate.Text = "Met Date:";
            this.Label_MetDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CAL_MetDate
            // 
            this.CAL_MetDate.CustomFormat = "MM/dd/yyyy";
            this.CAL_MetDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.CAL_MetDate.Location = new System.Drawing.Point(104, 0);
            this.CAL_MetDate.Margin = new System.Windows.Forms.Padding(0);
            this.CAL_MetDate.MaxDate = new System.DateTime(2099, 12, 31, 0, 0, 0, 0);
            this.CAL_MetDate.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.CAL_MetDate.Name = "CAL_MetDate";
            this.CAL_MetDate.Size = new System.Drawing.Size(136, 23);
            this.CAL_MetDate.TabIndex = 5;
            this.CAL_MetDate.Value = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            // 
            // FLP_MetLevel
            // 
            this.FLP_MetLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_MetLevel.Controls.Add(this.Label_MetLevel);
            this.FLP_MetLevel.Controls.Add(this.TB_MetLevel);
            this.FLP_MetLevel.Controls.Add(this.CHK_Fateful);
            this.FLP_MetLevel.Location = new System.Drawing.Point(0, 120);
            this.FLP_MetLevel.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_MetLevel.Name = "FLP_MetLevel";
            this.FLP_MetLevel.Size = new System.Drawing.Size(280, 24);
            this.FLP_MetLevel.TabIndex = 6;
            // 
            // Label_MetLevel
            // 
            this.Label_MetLevel.Location = new System.Drawing.Point(0, 0);
            this.Label_MetLevel.Margin = new System.Windows.Forms.Padding(0);
            this.Label_MetLevel.Name = "Label_MetLevel";
            this.Label_MetLevel.Size = new System.Drawing.Size(104, 24);
            this.Label_MetLevel.TabIndex = 3;
            this.Label_MetLevel.Text = "Met Level:";
            this.Label_MetLevel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TB_MetLevel
            // 
            this.TB_MetLevel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_MetLevel.Location = new System.Drawing.Point(104, 0);
            this.TB_MetLevel.Margin = new System.Windows.Forms.Padding(0);
            this.TB_MetLevel.Mask = "000";
            this.TB_MetLevel.Name = "TB_MetLevel";
            this.TB_MetLevel.Size = new System.Drawing.Size(22, 23);
            this.TB_MetLevel.TabIndex = 4;
            this.TB_MetLevel.Validated += new System.EventHandler(this.Update255_MTB);
            // 
            // CHK_Fateful
            // 
            this.CHK_Fateful.AutoSize = true;
            this.CHK_Fateful.Location = new System.Drawing.Point(131, 3);
            this.CHK_Fateful.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.CHK_Fateful.Name = "CHK_Fateful";
            this.CHK_Fateful.Size = new System.Drawing.Size(119, 19);
            this.CHK_Fateful.TabIndex = 6;
            this.CHK_Fateful.Text = "Fateful Encounter";
            this.CHK_Fateful.UseVisualStyleBackColor = true;
            // 
            // FLP_ObedienceLevel
            // 
            this.FLP_ObedienceLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_ObedienceLevel.Controls.Add(this.L_ObedienceLevel);
            this.FLP_ObedienceLevel.Controls.Add(this.TB_ObedienceLevel);
            this.FLP_ObedienceLevel.Location = new System.Drawing.Point(0, 144);
            this.FLP_ObedienceLevel.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_ObedienceLevel.Name = "FLP_ObedienceLevel";
            this.FLP_ObedienceLevel.Size = new System.Drawing.Size(280, 24);
            this.FLP_ObedienceLevel.TabIndex = 7;
            // 
            // L_ObedienceLevel
            // 
            this.L_ObedienceLevel.Location = new System.Drawing.Point(0, 0);
            this.L_ObedienceLevel.Margin = new System.Windows.Forms.Padding(0);
            this.L_ObedienceLevel.Name = "L_ObedienceLevel";
            this.L_ObedienceLevel.Size = new System.Drawing.Size(104, 24);
            this.L_ObedienceLevel.TabIndex = 3;
            this.L_ObedienceLevel.Text = "Obedience Level:";
            this.L_ObedienceLevel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.L_ObedienceLevel.Click += new System.EventHandler(this.L_Obedience_Click);
            // 
            // TB_ObedienceLevel
            // 
            this.TB_ObedienceLevel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_ObedienceLevel.Location = new System.Drawing.Point(104, 0);
            this.TB_ObedienceLevel.Margin = new System.Windows.Forms.Padding(0);
            this.TB_ObedienceLevel.Mask = "000";
            this.TB_ObedienceLevel.Name = "TB_ObedienceLevel";
            this.TB_ObedienceLevel.Size = new System.Drawing.Size(22, 23);
            this.TB_ObedienceLevel.TabIndex = 4;
            // 
            // FLP_GroundTile
            // 
            this.FLP_GroundTile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_GroundTile.Controls.Add(this.Label_GroundTile);
            this.FLP_GroundTile.Controls.Add(this.CB_GroundTile);
            this.FLP_GroundTile.Location = new System.Drawing.Point(0, 168);
            this.FLP_GroundTile.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_GroundTile.Name = "FLP_GroundTile";
            this.FLP_GroundTile.Size = new System.Drawing.Size(280, 24);
            this.FLP_GroundTile.TabIndex = 8;
            // 
            // Label_GroundTile
            // 
            this.Label_GroundTile.Location = new System.Drawing.Point(0, 0);
            this.Label_GroundTile.Margin = new System.Windows.Forms.Padding(0);
            this.Label_GroundTile.Name = "Label_GroundTile";
            this.Label_GroundTile.Size = new System.Drawing.Size(104, 24);
            this.Label_GroundTile.TabIndex = 5;
            this.Label_GroundTile.Text = "Encounter:";
            this.Label_GroundTile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_GroundTile
            // 
            this.CB_GroundTile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_GroundTile.DropDownWidth = 160;
            this.CB_GroundTile.FormattingEnabled = true;
            this.CB_GroundTile.Location = new System.Drawing.Point(104, 0);
            this.CB_GroundTile.Margin = new System.Windows.Forms.Padding(0);
            this.CB_GroundTile.Name = "CB_GroundTile";
            this.CB_GroundTile.Size = new System.Drawing.Size(136, 23);
            this.CB_GroundTile.TabIndex = 7;
            // 
            // FLP_TimeOfDay
            // 
            this.FLP_TimeOfDay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_TimeOfDay.Controls.Add(this.L_MetTimeOfDay);
            this.FLP_TimeOfDay.Controls.Add(this.CB_MetTimeOfDay);
            this.FLP_TimeOfDay.Location = new System.Drawing.Point(0, 192);
            this.FLP_TimeOfDay.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_TimeOfDay.Name = "FLP_TimeOfDay";
            this.FLP_TimeOfDay.Size = new System.Drawing.Size(280, 24);
            this.FLP_TimeOfDay.TabIndex = 9;
            // 
            // L_MetTimeOfDay
            // 
            this.L_MetTimeOfDay.Location = new System.Drawing.Point(0, 0);
            this.L_MetTimeOfDay.Margin = new System.Windows.Forms.Padding(0);
            this.L_MetTimeOfDay.Name = "L_MetTimeOfDay";
            this.L_MetTimeOfDay.Size = new System.Drawing.Size(104, 24);
            this.L_MetTimeOfDay.TabIndex = 10;
            this.L_MetTimeOfDay.Text = "Time of Day:";
            this.L_MetTimeOfDay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_MetTimeOfDay
            // 
            this.CB_MetTimeOfDay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_MetTimeOfDay.DropDownWidth = 150;
            this.CB_MetTimeOfDay.FormattingEnabled = true;
            this.CB_MetTimeOfDay.Items.AddRange(new object[] {
            "(None)",
            "Morning",
            "Day",
            "Night"});
            this.CB_MetTimeOfDay.Location = new System.Drawing.Point(104, 0);
            this.CB_MetTimeOfDay.Margin = new System.Windows.Forms.Padding(0);
            this.CB_MetTimeOfDay.Name = "CB_MetTimeOfDay";
            this.CB_MetTimeOfDay.Size = new System.Drawing.Size(136, 23);
            this.CB_MetTimeOfDay.TabIndex = 11;
            // 
            // Hidden_Stats
            // 
            this.Hidden_Stats.AllowDrop = true;
            this.Hidden_Stats.Controls.Add(this.Stats);
            this.Hidden_Stats.Location = new System.Drawing.Point(4, 5);
            this.Hidden_Stats.Margin = new System.Windows.Forms.Padding(0);
            this.Hidden_Stats.Name = "Hidden_Stats";
            this.Hidden_Stats.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.Hidden_Stats.Size = new System.Drawing.Size(296, 391);
            this.Hidden_Stats.TabIndex = 2;
            this.Hidden_Stats.Text = "Stats";
            this.Hidden_Stats.UseVisualStyleBackColor = true;
            // 
            // Stats
            // 
            this.Stats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Stats.EVsFishy = System.Drawing.Color.LightYellow;
            this.Stats.EVsInvalid = System.Drawing.Color.Red;
            this.Stats.EVsMaxed = System.Drawing.Color.Honeydew;
            this.Stats.HaX = false;
            this.Stats.Location = new System.Drawing.Point(0, 8);
            this.Stats.Margin = new System.Windows.Forms.Padding(0);
            this.Stats.Name = "Stats";
            this.Stats.Size = new System.Drawing.Size(296, 383);
            this.Stats.StatDecreased = System.Drawing.Color.Blue;
            this.Stats.StatHyperTrained = System.Drawing.Color.LightGreen;
            this.Stats.StatIncreased = System.Drawing.Color.Red;
            this.Stats.TabIndex = 0;
            // 
            // Hidden_Moves
            // 
            this.Hidden_Moves.AllowDrop = true;
            this.Hidden_Moves.Controls.Add(this.L_AlphaMastered);
            this.Hidden_Moves.Controls.Add(this.CB_AlphaMastered);
            this.Hidden_Moves.Controls.Add(this.FLP_MoveFlags);
            this.Hidden_Moves.Controls.Add(this.GB_RelearnMoves);
            this.Hidden_Moves.Controls.Add(this.GB_CurrentMoves);
            this.Hidden_Moves.Location = new System.Drawing.Point(4, 5);
            this.Hidden_Moves.Margin = new System.Windows.Forms.Padding(0);
            this.Hidden_Moves.Name = "Hidden_Moves";
            this.Hidden_Moves.Size = new System.Drawing.Size(296, 391);
            this.Hidden_Moves.TabIndex = 3;
            this.Hidden_Moves.Text = "Attacks";
            this.Hidden_Moves.UseVisualStyleBackColor = true;
            // 
            // L_AlphaMastered
            // 
            this.L_AlphaMastered.Location = new System.Drawing.Point(8, 328);
            this.L_AlphaMastered.Margin = new System.Windows.Forms.Padding(0);
            this.L_AlphaMastered.Name = "L_AlphaMastered";
            this.L_AlphaMastered.Size = new System.Drawing.Size(112, 22);
            this.L_AlphaMastered.TabIndex = 101;
            this.L_AlphaMastered.Text = "Alpha Mastered:";
            this.L_AlphaMastered.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_AlphaMastered
            // 
            this.CB_AlphaMastered.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_AlphaMastered.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_AlphaMastered.FormattingEnabled = true;
            this.CB_AlphaMastered.Location = new System.Drawing.Point(120, 328);
            this.CB_AlphaMastered.Name = "CB_AlphaMastered";
            this.CB_AlphaMastered.Size = new System.Drawing.Size(124, 23);
            this.CB_AlphaMastered.TabIndex = 12;
            this.CB_AlphaMastered.SelectedIndexChanged += new System.EventHandler(this.ValidateMove);
            // 
            // FLP_MoveFlags
            // 
            this.FLP_MoveFlags.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_MoveFlags.AutoSize = true;
            this.FLP_MoveFlags.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.FLP_MoveFlags.Controls.Add(this.B_RelearnFlags);
            this.FLP_MoveFlags.Controls.Add(this.B_MoveShop);
            this.FLP_MoveFlags.Location = new System.Drawing.Point(5, 301);
            this.FLP_MoveFlags.Name = "FLP_MoveFlags";
            this.FLP_MoveFlags.Size = new System.Drawing.Size(292, 25);
            this.FLP_MoveFlags.TabIndex = 100;
            this.FLP_MoveFlags.WrapContents = false;
            // 
            // B_RelearnFlags
            // 
            this.B_RelearnFlags.Location = new System.Drawing.Point(1, 1);
            this.B_RelearnFlags.Margin = new System.Windows.Forms.Padding(1);
            this.B_RelearnFlags.Name = "B_RelearnFlags";
            this.B_RelearnFlags.Size = new System.Drawing.Size(144, 23);
            this.B_RelearnFlags.TabIndex = 10;
            this.B_RelearnFlags.Text = "Relearn Flags";
            this.B_RelearnFlags.UseVisualStyleBackColor = true;
            this.B_RelearnFlags.Click += new System.EventHandler(this.B_Records_Click);
            // 
            // B_MoveShop
            // 
            this.B_MoveShop.Location = new System.Drawing.Point(147, 1);
            this.B_MoveShop.Margin = new System.Windows.Forms.Padding(1);
            this.B_MoveShop.Name = "B_MoveShop";
            this.B_MoveShop.Size = new System.Drawing.Size(144, 23);
            this.B_MoveShop.TabIndex = 11;
            this.B_MoveShop.Text = "Move Shop";
            this.B_MoveShop.UseVisualStyleBackColor = true;
            this.B_MoveShop.Click += new System.EventHandler(this.B_MoveShop_Click);
            // 
            // GB_RelearnMoves
            // 
            this.GB_RelearnMoves.Controls.Add(this.PB_WarnRelearn4);
            this.GB_RelearnMoves.Controls.Add(this.PB_WarnRelearn3);
            this.GB_RelearnMoves.Controls.Add(this.PB_WarnRelearn2);
            this.GB_RelearnMoves.Controls.Add(this.PB_WarnRelearn1);
            this.GB_RelearnMoves.Controls.Add(this.CB_RelearnMove4);
            this.GB_RelearnMoves.Controls.Add(this.CB_RelearnMove3);
            this.GB_RelearnMoves.Controls.Add(this.CB_RelearnMove2);
            this.GB_RelearnMoves.Controls.Add(this.CB_RelearnMove1);
            this.GB_RelearnMoves.Location = new System.Drawing.Point(64, 160);
            this.GB_RelearnMoves.Name = "GB_RelearnMoves";
            this.GB_RelearnMoves.Size = new System.Drawing.Size(168, 128);
            this.GB_RelearnMoves.TabIndex = 5;
            this.GB_RelearnMoves.TabStop = false;
            this.GB_RelearnMoves.Text = "Relearn Moves";
            // 
            // PB_WarnRelearn4
            // 
            this.PB_WarnRelearn4.Image = global::PKHeX.WinForms.Properties.Resources.warn;
            this.PB_WarnRelearn4.Location = new System.Drawing.Point(8, 96);
            this.PB_WarnRelearn4.Margin = new System.Windows.Forms.Padding(0);
            this.PB_WarnRelearn4.Name = "PB_WarnRelearn4";
            this.PB_WarnRelearn4.Size = new System.Drawing.Size(24, 24);
            this.PB_WarnRelearn4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PB_WarnRelearn4.TabIndex = 19;
            this.PB_WarnRelearn4.TabStop = false;
            this.PB_WarnRelearn4.Visible = false;
            // 
            // PB_WarnRelearn3
            // 
            this.PB_WarnRelearn3.Image = global::PKHeX.WinForms.Properties.Resources.warn;
            this.PB_WarnRelearn3.Location = new System.Drawing.Point(8, 72);
            this.PB_WarnRelearn3.Margin = new System.Windows.Forms.Padding(0);
            this.PB_WarnRelearn3.Name = "PB_WarnRelearn3";
            this.PB_WarnRelearn3.Size = new System.Drawing.Size(24, 24);
            this.PB_WarnRelearn3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PB_WarnRelearn3.TabIndex = 18;
            this.PB_WarnRelearn3.TabStop = false;
            this.PB_WarnRelearn3.Visible = false;
            // 
            // PB_WarnRelearn2
            // 
            this.PB_WarnRelearn2.Image = global::PKHeX.WinForms.Properties.Resources.warn;
            this.PB_WarnRelearn2.Location = new System.Drawing.Point(8, 48);
            this.PB_WarnRelearn2.Margin = new System.Windows.Forms.Padding(0);
            this.PB_WarnRelearn2.Name = "PB_WarnRelearn2";
            this.PB_WarnRelearn2.Size = new System.Drawing.Size(24, 24);
            this.PB_WarnRelearn2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PB_WarnRelearn2.TabIndex = 17;
            this.PB_WarnRelearn2.TabStop = false;
            this.PB_WarnRelearn2.Visible = false;
            // 
            // PB_WarnRelearn1
            // 
            this.PB_WarnRelearn1.Image = global::PKHeX.WinForms.Properties.Resources.warn;
            this.PB_WarnRelearn1.Location = new System.Drawing.Point(8, 24);
            this.PB_WarnRelearn1.Margin = new System.Windows.Forms.Padding(0);
            this.PB_WarnRelearn1.Name = "PB_WarnRelearn1";
            this.PB_WarnRelearn1.Size = new System.Drawing.Size(24, 24);
            this.PB_WarnRelearn1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PB_WarnRelearn1.TabIndex = 6;
            this.PB_WarnRelearn1.TabStop = false;
            this.PB_WarnRelearn1.Visible = false;
            // 
            // CB_RelearnMove4
            // 
            this.CB_RelearnMove4.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_RelearnMove4.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_RelearnMove4.FormattingEnabled = true;
            this.CB_RelearnMove4.Location = new System.Drawing.Point(32, 96);
            this.CB_RelearnMove4.Name = "CB_RelearnMove4";
            this.CB_RelearnMove4.Size = new System.Drawing.Size(124, 23);
            this.CB_RelearnMove4.TabIndex = 9;
            this.CB_RelearnMove4.SelectedIndexChanged += new System.EventHandler(this.ValidateMove);
            this.CB_RelearnMove4.Leave += new System.EventHandler(this.ValidateComboBox2);
            this.CB_RelearnMove4.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateComboBox);
            // 
            // CB_RelearnMove3
            // 
            this.CB_RelearnMove3.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_RelearnMove3.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_RelearnMove3.FormattingEnabled = true;
            this.CB_RelearnMove3.Location = new System.Drawing.Point(32, 72);
            this.CB_RelearnMove3.Name = "CB_RelearnMove3";
            this.CB_RelearnMove3.Size = new System.Drawing.Size(124, 23);
            this.CB_RelearnMove3.TabIndex = 8;
            this.CB_RelearnMove3.SelectedIndexChanged += new System.EventHandler(this.ValidateMove);
            this.CB_RelearnMove3.Leave += new System.EventHandler(this.ValidateComboBox2);
            this.CB_RelearnMove3.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateComboBox);
            // 
            // CB_RelearnMove2
            // 
            this.CB_RelearnMove2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_RelearnMove2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_RelearnMove2.FormattingEnabled = true;
            this.CB_RelearnMove2.Location = new System.Drawing.Point(32, 48);
            this.CB_RelearnMove2.Name = "CB_RelearnMove2";
            this.CB_RelearnMove2.Size = new System.Drawing.Size(124, 23);
            this.CB_RelearnMove2.TabIndex = 7;
            this.CB_RelearnMove2.SelectedIndexChanged += new System.EventHandler(this.ValidateMove);
            this.CB_RelearnMove2.Leave += new System.EventHandler(this.ValidateComboBox2);
            this.CB_RelearnMove2.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateComboBox);
            // 
            // CB_RelearnMove1
            // 
            this.CB_RelearnMove1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_RelearnMove1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_RelearnMove1.FormattingEnabled = true;
            this.CB_RelearnMove1.Location = new System.Drawing.Point(32, 24);
            this.CB_RelearnMove1.Name = "CB_RelearnMove1";
            this.CB_RelearnMove1.Size = new System.Drawing.Size(124, 23);
            this.CB_RelearnMove1.TabIndex = 6;
            this.CB_RelearnMove1.SelectedIndexChanged += new System.EventHandler(this.ValidateMove);
            this.CB_RelearnMove1.Leave += new System.EventHandler(this.ValidateComboBox2);
            this.CB_RelearnMove1.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateComboBox);
            // 
            // GB_CurrentMoves
            // 
            this.GB_CurrentMoves.Controls.Add(this.FLP_Moves);
            this.GB_CurrentMoves.Controls.Add(this.Label_CurPP);
            this.GB_CurrentMoves.Controls.Add(this.Label_PPups);
            this.GB_CurrentMoves.Location = new System.Drawing.Point(16, 16);
            this.GB_CurrentMoves.Name = "GB_CurrentMoves";
            this.GB_CurrentMoves.Size = new System.Drawing.Size(248, 136);
            this.GB_CurrentMoves.TabIndex = 0;
            this.GB_CurrentMoves.TabStop = false;
            this.GB_CurrentMoves.Text = "Current Moves";
            // 
            // FLP_Moves
            // 
            this.FLP_Moves.Controls.Add(this.MC_Move1);
            this.FLP_Moves.Controls.Add(this.MC_Move2);
            this.FLP_Moves.Controls.Add(this.MC_Move3);
            this.FLP_Moves.Controls.Add(this.MC_Move4);
            this.FLP_Moves.Location = new System.Drawing.Point(8, 32);
            this.FLP_Moves.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_Moves.Name = "FLP_Moves";
            this.FLP_Moves.Size = new System.Drawing.Size(240, 96);
            this.FLP_Moves.TabIndex = 14;
            // 
            // MC_Move1
            // 
            this.MC_Move1.Location = new System.Drawing.Point(0, 0);
            this.MC_Move1.Margin = new System.Windows.Forms.Padding(0);
            this.MC_Move1.Name = "MC_Move1";
            this.MC_Move1.PP = 0;
            this.MC_Move1.PPUps = 0;
            this.MC_Move1.SelectedMove = ((ushort)(0));
            this.MC_Move1.Size = new System.Drawing.Size(240, 24);
            this.MC_Move1.TabIndex = 1;
            // 
            // MC_Move2
            // 
            this.MC_Move2.Location = new System.Drawing.Point(0, 24);
            this.MC_Move2.Margin = new System.Windows.Forms.Padding(0);
            this.MC_Move2.Name = "MC_Move2";
            this.MC_Move2.PP = 0;
            this.MC_Move2.PPUps = 0;
            this.MC_Move2.SelectedMove = ((ushort)(0));
            this.MC_Move2.Size = new System.Drawing.Size(240, 24);
            this.MC_Move2.TabIndex = 2;
            // 
            // MC_Move3
            // 
            this.MC_Move3.Location = new System.Drawing.Point(0, 48);
            this.MC_Move3.Margin = new System.Windows.Forms.Padding(0);
            this.MC_Move3.Name = "MC_Move3";
            this.MC_Move3.PP = 0;
            this.MC_Move3.PPUps = 0;
            this.MC_Move3.SelectedMove = ((ushort)(0));
            this.MC_Move3.Size = new System.Drawing.Size(240, 24);
            this.MC_Move3.TabIndex = 3;
            // 
            // MC_Move4
            // 
            this.MC_Move4.Location = new System.Drawing.Point(0, 72);
            this.MC_Move4.Margin = new System.Windows.Forms.Padding(0);
            this.MC_Move4.Name = "MC_Move4";
            this.MC_Move4.PP = 0;
            this.MC_Move4.PPUps = 0;
            this.MC_Move4.SelectedMove = ((ushort)(0));
            this.MC_Move4.Size = new System.Drawing.Size(240, 24);
            this.MC_Move4.TabIndex = 4;
            // 
            // Label_CurPP
            // 
            this.Label_CurPP.Location = new System.Drawing.Point(160, 16);
            this.Label_CurPP.Name = "Label_CurPP";
            this.Label_CurPP.Size = new System.Drawing.Size(32, 16);
            this.Label_CurPP.TabIndex = 2;
            this.Label_CurPP.Text = "PP";
            this.Label_CurPP.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.Label_CurPP.Click += new System.EventHandler(this.ClickPP);
            // 
            // Label_PPups
            // 
            this.Label_PPups.Location = new System.Drawing.Point(192, 16);
            this.Label_PPups.Name = "Label_PPups";
            this.Label_PPups.Size = new System.Drawing.Size(48, 16);
            this.Label_PPups.TabIndex = 12;
            this.Label_PPups.Text = "PP Ups";
            this.Label_PPups.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.Label_PPups.Click += new System.EventHandler(this.ClickPPUps);
            // 
            // Hidden_Cosmetic
            // 
            this.Hidden_Cosmetic.Controls.Add(this.Contest);
            this.Hidden_Cosmetic.Controls.Add(this.FLP_PKMEditors);
            this.Hidden_Cosmetic.Controls.Add(this.FLP_CosmeticTop);
            this.Hidden_Cosmetic.Location = new System.Drawing.Point(4, 5);
            this.Hidden_Cosmetic.Name = "Hidden_Cosmetic";
            this.Hidden_Cosmetic.Size = new System.Drawing.Size(296, 391);
            this.Hidden_Cosmetic.TabIndex = 5;
            this.Hidden_Cosmetic.Text = "Cosmetic";
            this.Hidden_Cosmetic.UseVisualStyleBackColor = true;
            // 
            // Contest
            // 
            this.Contest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Contest.CNT_Beauty = ((byte)(0));
            this.Contest.CNT_Cool = ((byte)(0));
            this.Contest.CNT_Cute = ((byte)(0));
            this.Contest.CNT_Sheen = ((byte)(0));
            this.Contest.CNT_Smart = ((byte)(0));
            this.Contest.CNT_Tough = ((byte)(0));
            this.Contest.Location = new System.Drawing.Point(4, 328);
            this.Contest.Margin = new System.Windows.Forms.Padding(0);
            this.Contest.Name = "Contest";
            this.Contest.Size = new System.Drawing.Size(288, 56);
            this.Contest.TabIndex = 50;
            // 
            // FLP_PKMEditors
            // 
            this.FLP_PKMEditors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_PKMEditors.AutoSize = true;
            this.FLP_PKMEditors.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.FLP_PKMEditors.Controls.Add(this.BTN_Ribbons);
            this.FLP_PKMEditors.Controls.Add(this.BTN_Medals);
            this.FLP_PKMEditors.Controls.Add(this.BTN_History);
            this.FLP_PKMEditors.Location = new System.Drawing.Point(56, 288);
            this.FLP_PKMEditors.Name = "FLP_PKMEditors";
            this.FLP_PKMEditors.Size = new System.Drawing.Size(191, 27);
            this.FLP_PKMEditors.TabIndex = 6;
            this.FLP_PKMEditors.WrapContents = false;
            // 
            // BTN_Ribbons
            // 
            this.BTN_Ribbons.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BTN_Ribbons.AutoSize = true;
            this.BTN_Ribbons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BTN_Ribbons.Location = new System.Drawing.Point(1, 1);
            this.BTN_Ribbons.Margin = new System.Windows.Forms.Padding(1);
            this.BTN_Ribbons.Name = "BTN_Ribbons";
            this.BTN_Ribbons.Size = new System.Drawing.Size(60, 25);
            this.BTN_Ribbons.TabIndex = 28;
            this.BTN_Ribbons.Text = "Ribbons";
            this.BTN_Ribbons.UseVisualStyleBackColor = true;
            this.BTN_Ribbons.Click += new System.EventHandler(this.OpenRibbons);
            // 
            // BTN_Medals
            // 
            this.BTN_Medals.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BTN_Medals.AutoSize = true;
            this.BTN_Medals.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BTN_Medals.Location = new System.Drawing.Point(63, 1);
            this.BTN_Medals.Margin = new System.Windows.Forms.Padding(1);
            this.BTN_Medals.Name = "BTN_Medals";
            this.BTN_Medals.Size = new System.Drawing.Size(55, 25);
            this.BTN_Medals.TabIndex = 29;
            this.BTN_Medals.Text = "Medals";
            this.BTN_Medals.UseVisualStyleBackColor = true;
            this.BTN_Medals.Click += new System.EventHandler(this.OpenMedals);
            // 
            // BTN_History
            // 
            this.BTN_History.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BTN_History.AutoSize = true;
            this.BTN_History.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BTN_History.Location = new System.Drawing.Point(120, 1);
            this.BTN_History.Margin = new System.Windows.Forms.Padding(1);
            this.BTN_History.Name = "BTN_History";
            this.BTN_History.Size = new System.Drawing.Size(70, 25);
            this.BTN_History.TabIndex = 30;
            this.BTN_History.Text = "Memories";
            this.BTN_History.UseVisualStyleBackColor = true;
            this.BTN_History.Click += new System.EventHandler(this.OpenHistory);
            // 
            // FLP_CosmeticTop
            // 
            this.FLP_CosmeticTop.Controls.Add(this.GB_Markings);
            this.FLP_CosmeticTop.Controls.Add(this.FLP_BigMarkings);
            this.FLP_CosmeticTop.Controls.Add(this.SizeCP);
            this.FLP_CosmeticTop.Controls.Add(this.ShinyLeaf);
            this.FLP_CosmeticTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.FLP_CosmeticTop.Location = new System.Drawing.Point(0, 0);
            this.FLP_CosmeticTop.Name = "FLP_CosmeticTop";
            this.FLP_CosmeticTop.Padding = new System.Windows.Forms.Padding(0, 16, 0, 0);
            this.FLP_CosmeticTop.Size = new System.Drawing.Size(296, 280);
            this.FLP_CosmeticTop.TabIndex = 0;
            // 
            // GB_Markings
            // 
            this.GB_Markings.Controls.Add(this.PB_Mark6);
            this.GB_Markings.Controls.Add(this.PB_Mark3);
            this.GB_Markings.Controls.Add(this.PB_Mark5);
            this.GB_Markings.Controls.Add(this.PB_MarkCured);
            this.GB_Markings.Controls.Add(this.PB_Mark2);
            this.GB_Markings.Controls.Add(this.PB_MarkShiny);
            this.GB_Markings.Controls.Add(this.PB_Mark1);
            this.GB_Markings.Controls.Add(this.PB_Mark4);
            this.FLP_CosmeticTop.SetFlowBreak(this.GB_Markings, true);
            this.GB_Markings.Location = new System.Drawing.Point(64, 16);
            this.GB_Markings.Margin = new System.Windows.Forms.Padding(64, 0, 8, 0);
            this.GB_Markings.Name = "GB_Markings";
            this.GB_Markings.Padding = new System.Windows.Forms.Padding(0);
            this.GB_Markings.Size = new System.Drawing.Size(160, 72);
            this.GB_Markings.TabIndex = 1;
            this.GB_Markings.TabStop = false;
            this.GB_Markings.Text = "Markings";
            // 
            // PB_Mark6
            // 
            this.PB_Mark6.Image = global::PKHeX.WinForms.Properties.Resources.box_mark_06;
            this.PB_Mark6.InitialImage = global::PKHeX.WinForms.Properties.Resources.box_mark_06;
            this.PB_Mark6.Location = new System.Drawing.Point(128, 40);
            this.PB_Mark6.Margin = new System.Windows.Forms.Padding(0);
            this.PB_Mark6.Name = "PB_Mark6";
            this.PB_Mark6.Size = new System.Drawing.Size(24, 24);
            this.PB_Mark6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PB_Mark6.TabIndex = 5;
            this.PB_Mark6.TabStop = false;
            this.PB_Mark6.Click += new System.EventHandler(this.ClickMarking);
            // 
            // PB_Mark3
            // 
            this.PB_Mark3.Image = global::PKHeX.WinForms.Properties.Resources.box_mark_03;
            this.PB_Mark3.InitialImage = global::PKHeX.WinForms.Properties.Resources.box_mark_03;
            this.PB_Mark3.Location = new System.Drawing.Point(56, 40);
            this.PB_Mark3.Margin = new System.Windows.Forms.Padding(0);
            this.PB_Mark3.Name = "PB_Mark3";
            this.PB_Mark3.Size = new System.Drawing.Size(24, 24);
            this.PB_Mark3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PB_Mark3.TabIndex = 2;
            this.PB_Mark3.TabStop = false;
            this.PB_Mark3.Click += new System.EventHandler(this.ClickMarking);
            // 
            // PB_Mark5
            // 
            this.PB_Mark5.Image = global::PKHeX.WinForms.Properties.Resources.box_mark_05;
            this.PB_Mark5.InitialImage = global::PKHeX.WinForms.Properties.Resources.box_mark_05;
            this.PB_Mark5.Location = new System.Drawing.Point(104, 40);
            this.PB_Mark5.Margin = new System.Windows.Forms.Padding(0);
            this.PB_Mark5.Name = "PB_Mark5";
            this.PB_Mark5.Size = new System.Drawing.Size(24, 24);
            this.PB_Mark5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PB_Mark5.TabIndex = 4;
            this.PB_Mark5.TabStop = false;
            this.PB_Mark5.Click += new System.EventHandler(this.ClickMarking);
            // 
            // PB_MarkCured
            // 
            this.PB_MarkCured.Image = global::PKHeX.WinForms.Properties.Resources.anti_pokerus_icon;
            this.PB_MarkCured.InitialImage = global::PKHeX.WinForms.Properties.Resources.anti_pokerus_icon;
            this.PB_MarkCured.Location = new System.Drawing.Point(80, 16);
            this.PB_MarkCured.Margin = new System.Windows.Forms.Padding(0);
            this.PB_MarkCured.Name = "PB_MarkCured";
            this.PB_MarkCured.Size = new System.Drawing.Size(24, 24);
            this.PB_MarkCured.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PB_MarkCured.TabIndex = 7;
            this.PB_MarkCured.TabStop = false;
            this.PB_MarkCured.Click += new System.EventHandler(this.PB_MarkCured_Click);
            // 
            // PB_Mark2
            // 
            this.PB_Mark2.Image = global::PKHeX.WinForms.Properties.Resources.box_mark_02;
            this.PB_Mark2.InitialImage = global::PKHeX.WinForms.Properties.Resources.box_mark_02;
            this.PB_Mark2.Location = new System.Drawing.Point(32, 40);
            this.PB_Mark2.Margin = new System.Windows.Forms.Padding(0);
            this.PB_Mark2.Name = "PB_Mark2";
            this.PB_Mark2.Size = new System.Drawing.Size(24, 24);
            this.PB_Mark2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PB_Mark2.TabIndex = 1;
            this.PB_Mark2.TabStop = false;
            this.PB_Mark2.Click += new System.EventHandler(this.ClickMarking);
            // 
            // PB_MarkShiny
            // 
            this.PB_MarkShiny.Image = global::PKHeX.WinForms.Properties.Resources.rare_icon;
            this.PB_MarkShiny.InitialImage = global::PKHeX.WinForms.Properties.Resources.rare_icon;
            this.PB_MarkShiny.Location = new System.Drawing.Point(56, 16);
            this.PB_MarkShiny.Margin = new System.Windows.Forms.Padding(0);
            this.PB_MarkShiny.Name = "PB_MarkShiny";
            this.PB_MarkShiny.Size = new System.Drawing.Size(24, 24);
            this.PB_MarkShiny.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PB_MarkShiny.TabIndex = 6;
            this.PB_MarkShiny.TabStop = false;
            this.PB_MarkShiny.Click += new System.EventHandler(this.PB_MarkShiny_Click);
            // 
            // PB_Mark1
            // 
            this.PB_Mark1.Image = global::PKHeX.WinForms.Properties.Resources.box_mark_01;
            this.PB_Mark1.InitialImage = global::PKHeX.WinForms.Properties.Resources.box_mark_01;
            this.PB_Mark1.Location = new System.Drawing.Point(8, 40);
            this.PB_Mark1.Margin = new System.Windows.Forms.Padding(0);
            this.PB_Mark1.Name = "PB_Mark1";
            this.PB_Mark1.Size = new System.Drawing.Size(24, 24);
            this.PB_Mark1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PB_Mark1.TabIndex = 0;
            this.PB_Mark1.TabStop = false;
            this.PB_Mark1.Click += new System.EventHandler(this.ClickMarking);
            // 
            // PB_Mark4
            // 
            this.PB_Mark4.Image = global::PKHeX.WinForms.Properties.Resources.box_mark_04;
            this.PB_Mark4.InitialImage = global::PKHeX.WinForms.Properties.Resources.box_mark_04;
            this.PB_Mark4.Location = new System.Drawing.Point(80, 40);
            this.PB_Mark4.Margin = new System.Windows.Forms.Padding(0);
            this.PB_Mark4.Name = "PB_Mark4";
            this.PB_Mark4.Size = new System.Drawing.Size(24, 24);
            this.PB_Mark4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PB_Mark4.TabIndex = 3;
            this.PB_Mark4.TabStop = false;
            this.PB_Mark4.Click += new System.EventHandler(this.ClickMarking);
            // 
            // FLP_BigMarkings
            // 
            this.FLP_BigMarkings.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.FLP_BigMarkings.AutoSize = true;
            this.FLP_BigMarkings.Controls.Add(this.PB_Favorite);
            this.FLP_BigMarkings.Controls.Add(this.PB_Origin);
            this.FLP_BigMarkings.Controls.Add(this.PB_BattleVersion);
            this.FLP_BigMarkings.Location = new System.Drawing.Point(72, 96);
            this.FLP_BigMarkings.Margin = new System.Windows.Forms.Padding(72, 8, 0, 0);
            this.FLP_BigMarkings.Name = "FLP_BigMarkings";
            this.FLP_BigMarkings.Size = new System.Drawing.Size(144, 48);
            this.FLP_BigMarkings.TabIndex = 2;
            // 
            // PB_Favorite
            // 
            this.PB_Favorite.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PB_Favorite.Image = global::PKHeX.WinForms.Properties.Resources.icon_favo;
            this.PB_Favorite.InitialImage = global::PKHeX.WinForms.Properties.Resources.icon_favo;
            this.PB_Favorite.Location = new System.Drawing.Point(0, 0);
            this.PB_Favorite.Margin = new System.Windows.Forms.Padding(0);
            this.PB_Favorite.Name = "PB_Favorite";
            this.PB_Favorite.Size = new System.Drawing.Size(48, 48);
            this.PB_Favorite.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PB_Favorite.TabIndex = 10;
            this.PB_Favorite.TabStop = false;
            this.PB_Favorite.Click += new System.EventHandler(this.ClickFavorite);
            // 
            // PB_Origin
            // 
            this.PB_Origin.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PB_Origin.Location = new System.Drawing.Point(48, 0);
            this.PB_Origin.Margin = new System.Windows.Forms.Padding(0);
            this.PB_Origin.Name = "PB_Origin";
            this.PB_Origin.Size = new System.Drawing.Size(48, 48);
            this.PB_Origin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PB_Origin.TabIndex = 11;
            this.PB_Origin.TabStop = false;
            this.PB_Origin.Click += new System.EventHandler(this.ClickVersionMarking);
            // 
            // PB_BattleVersion
            // 
            this.PB_BattleVersion.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PB_BattleVersion.Image = global::PKHeX.WinForms.Properties.Resources.icon_btlrom;
            this.PB_BattleVersion.InitialImage = global::PKHeX.WinForms.Properties.Resources.icon_btlrom;
            this.PB_BattleVersion.Location = new System.Drawing.Point(96, 0);
            this.PB_BattleVersion.Margin = new System.Windows.Forms.Padding(0);
            this.PB_BattleVersion.Name = "PB_BattleVersion";
            this.PB_BattleVersion.Size = new System.Drawing.Size(48, 48);
            this.PB_BattleVersion.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PB_BattleVersion.TabIndex = 12;
            this.PB_BattleVersion.TabStop = false;
            this.PB_BattleVersion.Click += new System.EventHandler(this.ClickVersionMarking);
            // 
            // SizeCP
            // 
            this.SizeCP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SizeCP.Location = new System.Drawing.Point(48, 152);
            this.SizeCP.Margin = new System.Windows.Forms.Padding(48, 8, 0, 0);
            this.SizeCP.Name = "SizeCP";
            this.SizeCP.Size = new System.Drawing.Size(224, 80);
            this.SizeCP.TabIndex = 3;
            // 
            // ShinyLeaf
            // 
            this.ShinyLeaf.Location = new System.Drawing.Point(56, 240);
            this.ShinyLeaf.Margin = new System.Windows.Forms.Padding(56, 8, 0, 0);
            this.ShinyLeaf.Name = "ShinyLeaf";
            this.ShinyLeaf.Size = new System.Drawing.Size(200, 64);
            this.ShinyLeaf.TabIndex = 4;
            // 
            // Hidden_OTMisc
            // 
            this.Hidden_OTMisc.AllowDrop = true;
            this.Hidden_OTMisc.Controls.Add(this.FLP_OTMisc);
            this.Hidden_OTMisc.Location = new System.Drawing.Point(4, 5);
            this.Hidden_OTMisc.Margin = new System.Windows.Forms.Padding(0);
            this.Hidden_OTMisc.Name = "Hidden_OTMisc";
            this.Hidden_OTMisc.Size = new System.Drawing.Size(296, 391);
            this.Hidden_OTMisc.TabIndex = 4;
            this.Hidden_OTMisc.Text = "OT/Misc";
            this.Hidden_OTMisc.UseVisualStyleBackColor = true;
            // 
            // FLP_OTMisc
            // 
            this.FLP_OTMisc.Controls.Add(this.GB_OT);
            this.FLP_OTMisc.Controls.Add(this.TID_Trainer);
            this.FLP_OTMisc.Controls.Add(this.FLP_OT);
            this.FLP_OTMisc.Controls.Add(this.FLP_Country);
            this.FLP_OTMisc.Controls.Add(this.FLP_SubRegion);
            this.FLP_OTMisc.Controls.Add(this.FLP_3DSRegion);
            this.FLP_OTMisc.Controls.Add(this.FLP_Handler);
            this.FLP_OTMisc.Controls.Add(this.GB_nOT);
            this.FLP_OTMisc.Controls.Add(this.FLP_HT);
            this.FLP_OTMisc.Controls.Add(this.FLP_HTLanguage);
            this.FLP_OTMisc.Controls.Add(this.FLP_ExtraBytes);
            this.FLP_OTMisc.Controls.Add(this.L_HomeTracker);
            this.FLP_OTMisc.Controls.Add(this.TB_HomeTracker);
            this.FLP_OTMisc.Controls.Add(this.Label_EncryptionConstant);
            this.FLP_OTMisc.Controls.Add(this.TB_EC);
            this.FLP_OTMisc.Controls.Add(this.BTN_RerollEC);
            this.FLP_OTMisc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FLP_OTMisc.Location = new System.Drawing.Point(0, 0);
            this.FLP_OTMisc.Name = "FLP_OTMisc";
            this.FLP_OTMisc.Size = new System.Drawing.Size(296, 391);
            this.FLP_OTMisc.TabIndex = 1;
            // 
            // GB_OT
            // 
            this.FLP_OTMisc.SetFlowBreak(this.GB_OT, true);
            this.GB_OT.Location = new System.Drawing.Point(56, 16);
            this.GB_OT.Margin = new System.Windows.Forms.Padding(56, 16, 0, 0);
            this.GB_OT.Name = "GB_OT";
            this.GB_OT.Size = new System.Drawing.Size(216, 24);
            this.GB_OT.TabIndex = 47;
            this.GB_OT.Text = "Trainer Information";
            this.GB_OT.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TID_Trainer
            // 
            this.TID_Trainer.Location = new System.Drawing.Point(64, 40);
            this.TID_Trainer.Margin = new System.Windows.Forms.Padding(64, 0, 0, 0);
            this.TID_Trainer.Name = "TID_Trainer";
            this.TID_Trainer.Size = new System.Drawing.Size(208, 24);
            this.TID_Trainer.TabIndex = 1;
            // 
            // FLP_OT
            // 
            this.FLP_OT.Controls.Add(this.Label_OT);
            this.FLP_OT.Controls.Add(this.TB_OT);
            this.FLP_OT.Controls.Add(this.UC_OTGender);
            this.FLP_OT.Location = new System.Drawing.Point(0, 64);
            this.FLP_OT.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_OT.Name = "FLP_OT";
            this.FLP_OT.Size = new System.Drawing.Size(272, 24);
            this.FLP_OT.TabIndex = 2;
            // 
            // Label_OT
            // 
            this.Label_OT.Location = new System.Drawing.Point(0, 0);
            this.Label_OT.Margin = new System.Windows.Forms.Padding(0);
            this.Label_OT.Name = "Label_OT";
            this.Label_OT.Size = new System.Drawing.Size(104, 24);
            this.Label_OT.TabIndex = 2;
            this.Label_OT.Text = "OT:";
            this.Label_OT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Label_OT.Click += new System.EventHandler(this.ClickOT);
            // 
            // TB_OT
            // 
            this.TB_OT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_OT.Location = new System.Drawing.Point(104, 0);
            this.TB_OT.Margin = new System.Windows.Forms.Padding(0);
            this.TB_OT.MaxLength = 12;
            this.TB_OT.Name = "TB_OT";
            this.TB_OT.Size = new System.Drawing.Size(94, 23);
            this.TB_OT.TabIndex = 3;
            this.TB_OT.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UpdateNicknameClick);
            // 
            // UC_OTGender
            // 
            this.UC_OTGender.AllowClick = true;
            this.UC_OTGender.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("UC_OTGender.BackgroundImage")));
            this.UC_OTGender.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.UC_OTGender.Gender = 0;
            this.UC_OTGender.Location = new System.Drawing.Point(198, 0);
            this.UC_OTGender.Margin = new System.Windows.Forms.Padding(0);
            this.UC_OTGender.Name = "UC_OTGender";
            this.UC_OTGender.Size = new System.Drawing.Size(24, 24);
            this.UC_OTGender.TabIndex = 4;
            // 
            // FLP_Country
            // 
            this.FLP_Country.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_Country.Controls.Add(this.Label_Country);
            this.FLP_Country.Controls.Add(this.CB_Country);
            this.FLP_Country.Location = new System.Drawing.Point(0, 88);
            this.FLP_Country.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_Country.Name = "FLP_Country";
            this.FLP_Country.Size = new System.Drawing.Size(272, 24);
            this.FLP_Country.TabIndex = 3;
            // 
            // Label_Country
            // 
            this.Label_Country.Location = new System.Drawing.Point(0, 0);
            this.Label_Country.Margin = new System.Windows.Forms.Padding(0);
            this.Label_Country.Name = "Label_Country";
            this.Label_Country.Size = new System.Drawing.Size(104, 24);
            this.Label_Country.TabIndex = 16;
            this.Label_Country.Text = "Country:";
            this.Label_Country.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_Country
            // 
            this.CB_Country.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_Country.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Country.DropDownWidth = 180;
            this.CB_Country.FormattingEnabled = true;
            this.CB_Country.Location = new System.Drawing.Point(104, 0);
            this.CB_Country.Margin = new System.Windows.Forms.Padding(0);
            this.CB_Country.Name = "CB_Country";
            this.CB_Country.Size = new System.Drawing.Size(126, 23);
            this.CB_Country.TabIndex = 21;
            this.CB_Country.SelectedIndexChanged += new System.EventHandler(this.UpdateCountry);
            this.CB_Country.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateComboBox);
            // 
            // FLP_SubRegion
            // 
            this.FLP_SubRegion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_SubRegion.Controls.Add(this.Label_SubRegion);
            this.FLP_SubRegion.Controls.Add(this.CB_SubRegion);
            this.FLP_SubRegion.Location = new System.Drawing.Point(0, 112);
            this.FLP_SubRegion.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_SubRegion.Name = "FLP_SubRegion";
            this.FLP_SubRegion.Size = new System.Drawing.Size(272, 24);
            this.FLP_SubRegion.TabIndex = 4;
            // 
            // Label_SubRegion
            // 
            this.Label_SubRegion.Location = new System.Drawing.Point(0, 0);
            this.Label_SubRegion.Margin = new System.Windows.Forms.Padding(0);
            this.Label_SubRegion.Name = "Label_SubRegion";
            this.Label_SubRegion.Size = new System.Drawing.Size(104, 24);
            this.Label_SubRegion.TabIndex = 17;
            this.Label_SubRegion.Text = "Sub Region:";
            this.Label_SubRegion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_SubRegion
            // 
            this.CB_SubRegion.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_SubRegion.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_SubRegion.DropDownWidth = 180;
            this.CB_SubRegion.FormattingEnabled = true;
            this.CB_SubRegion.Location = new System.Drawing.Point(104, 0);
            this.CB_SubRegion.Margin = new System.Windows.Forms.Padding(0);
            this.CB_SubRegion.Name = "CB_SubRegion";
            this.CB_SubRegion.Size = new System.Drawing.Size(126, 23);
            this.CB_SubRegion.TabIndex = 22;
            this.CB_SubRegion.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateComboBox);
            // 
            // FLP_3DSRegion
            // 
            this.FLP_3DSRegion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_3DSRegion.Controls.Add(this.Label_3DSRegion);
            this.FLP_3DSRegion.Controls.Add(this.CB_3DSReg);
            this.FLP_3DSRegion.Location = new System.Drawing.Point(0, 136);
            this.FLP_3DSRegion.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_3DSRegion.Name = "FLP_3DSRegion";
            this.FLP_3DSRegion.Size = new System.Drawing.Size(272, 24);
            this.FLP_3DSRegion.TabIndex = 5;
            // 
            // Label_3DSRegion
            // 
            this.Label_3DSRegion.Location = new System.Drawing.Point(0, 0);
            this.Label_3DSRegion.Margin = new System.Windows.Forms.Padding(0);
            this.Label_3DSRegion.Name = "Label_3DSRegion";
            this.Label_3DSRegion.Size = new System.Drawing.Size(104, 24);
            this.Label_3DSRegion.TabIndex = 18;
            this.Label_3DSRegion.Text = "3DS Region:";
            this.Label_3DSRegion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_3DSReg
            // 
            this.CB_3DSReg.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_3DSReg.FormattingEnabled = true;
            this.CB_3DSReg.Location = new System.Drawing.Point(104, 0);
            this.CB_3DSReg.Margin = new System.Windows.Forms.Padding(0);
            this.CB_3DSReg.Name = "CB_3DSReg";
            this.CB_3DSReg.Size = new System.Drawing.Size(126, 23);
            this.CB_3DSReg.TabIndex = 23;
            // 
            // FLP_Handler
            // 
            this.FLP_Handler.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_Handler.Controls.Add(this.L_CurrentHandler);
            this.FLP_Handler.Controls.Add(this.CB_Handler);
            this.FLP_Handler.Location = new System.Drawing.Point(0, 176);
            this.FLP_Handler.Margin = new System.Windows.Forms.Padding(0, 16, 0, 8);
            this.FLP_Handler.Name = "FLP_Handler";
            this.FLP_Handler.Size = new System.Drawing.Size(272, 24);
            this.FLP_Handler.TabIndex = 6;
            // 
            // L_CurrentHandler
            // 
            this.L_CurrentHandler.Location = new System.Drawing.Point(0, 0);
            this.L_CurrentHandler.Margin = new System.Windows.Forms.Padding(0);
            this.L_CurrentHandler.Name = "L_CurrentHandler";
            this.L_CurrentHandler.Size = new System.Drawing.Size(128, 24);
            this.L_CurrentHandler.TabIndex = 18;
            this.L_CurrentHandler.Text = "Current Handler:";
            this.L_CurrentHandler.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_Handler
            // 
            this.CB_Handler.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Handler.FormattingEnabled = true;
            this.CB_Handler.Items.AddRange(new object[] {
            "OT",
            "HT"});
            this.CB_Handler.Location = new System.Drawing.Point(128, 0);
            this.CB_Handler.Margin = new System.Windows.Forms.Padding(0);
            this.CB_Handler.Name = "CB_Handler";
            this.CB_Handler.Size = new System.Drawing.Size(48, 23);
            this.CB_Handler.TabIndex = 1;
            this.CB_Handler.SelectedIndexChanged += new System.EventHandler(this.ChangeHandlerIndex);
            // 
            // GB_nOT
            // 
            this.FLP_OTMisc.SetFlowBreak(this.GB_nOT, true);
            this.GB_nOT.Location = new System.Drawing.Point(56, 208);
            this.GB_nOT.Margin = new System.Windows.Forms.Padding(56, 0, 0, 0);
            this.GB_nOT.Name = "GB_nOT";
            this.GB_nOT.Size = new System.Drawing.Size(216, 24);
            this.GB_nOT.TabIndex = 48;
            this.GB_nOT.Text = "Latest (not OT) Handler";
            this.GB_nOT.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FLP_HT
            // 
            this.FLP_HT.Controls.Add(this.Label_PrevOT);
            this.FLP_HT.Controls.Add(this.TB_HT);
            this.FLP_HT.Controls.Add(this.UC_HTGender);
            this.FLP_HT.Location = new System.Drawing.Point(0, 232);
            this.FLP_HT.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_HT.Name = "FLP_HT";
            this.FLP_HT.Size = new System.Drawing.Size(272, 24);
            this.FLP_HT.TabIndex = 10;
            // 
            // Label_PrevOT
            // 
            this.Label_PrevOT.Location = new System.Drawing.Point(0, 0);
            this.Label_PrevOT.Margin = new System.Windows.Forms.Padding(0);
            this.Label_PrevOT.Name = "Label_PrevOT";
            this.Label_PrevOT.Size = new System.Drawing.Size(104, 24);
            this.Label_PrevOT.TabIndex = 1;
            this.Label_PrevOT.Text = "OT:";
            this.Label_PrevOT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Label_PrevOT.Click += new System.EventHandler(this.ClickCT);
            // 
            // TB_HT
            // 
            this.TB_HT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_HT.Location = new System.Drawing.Point(104, 0);
            this.TB_HT.Margin = new System.Windows.Forms.Padding(0);
            this.TB_HT.MaxLength = 12;
            this.TB_HT.Name = "TB_HT";
            this.TB_HT.Size = new System.Drawing.Size(94, 23);
            this.TB_HT.TabIndex = 2;
            this.TB_HT.WordWrap = false;
            this.TB_HT.TextChanged += new System.EventHandler(this.UpdateNotOT);
            this.TB_HT.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UpdateNicknameClick);
            // 
            // UC_HTGender
            // 
            this.UC_HTGender.AllowClick = true;
            this.UC_HTGender.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("UC_HTGender.BackgroundImage")));
            this.UC_HTGender.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.UC_HTGender.Gender = 0;
            this.UC_HTGender.Location = new System.Drawing.Point(198, 0);
            this.UC_HTGender.Margin = new System.Windows.Forms.Padding(0);
            this.UC_HTGender.Name = "UC_HTGender";
            this.UC_HTGender.Size = new System.Drawing.Size(24, 24);
            this.UC_HTGender.TabIndex = 3;
            // 
            // FLP_HTLanguage
            // 
            this.FLP_HTLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_HTLanguage.Controls.Add(this.L_LanguageHT);
            this.FLP_HTLanguage.Controls.Add(this.CB_HTLanguage);
            this.FLP_HTLanguage.Location = new System.Drawing.Point(0, 256);
            this.FLP_HTLanguage.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_HTLanguage.Name = "FLP_HTLanguage";
            this.FLP_HTLanguage.Size = new System.Drawing.Size(272, 24);
            this.FLP_HTLanguage.TabIndex = 11;
            // 
            // L_LanguageHT
            // 
            this.L_LanguageHT.Location = new System.Drawing.Point(0, 0);
            this.L_LanguageHT.Margin = new System.Windows.Forms.Padding(0);
            this.L_LanguageHT.Name = "L_LanguageHT";
            this.L_LanguageHT.Size = new System.Drawing.Size(104, 24);
            this.L_LanguageHT.TabIndex = 18;
            this.L_LanguageHT.Text = "Language:";
            this.L_LanguageHT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_HTLanguage
            // 
            this.CB_HTLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_HTLanguage.FormattingEnabled = true;
            this.CB_HTLanguage.Location = new System.Drawing.Point(104, 0);
            this.CB_HTLanguage.Margin = new System.Windows.Forms.Padding(0);
            this.CB_HTLanguage.Name = "CB_HTLanguage";
            this.CB_HTLanguage.Size = new System.Drawing.Size(126, 23);
            this.CB_HTLanguage.TabIndex = 4;
            // 
            // FLP_ExtraBytes
            // 
            this.FLP_ExtraBytes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_ExtraBytes.Controls.Add(this.L_ExtraBytes);
            this.FLP_ExtraBytes.Controls.Add(this.CB_ExtraBytes);
            this.FLP_ExtraBytes.Controls.Add(this.TB_ExtraByte);
            this.FLP_ExtraBytes.Location = new System.Drawing.Point(0, 304);
            this.FLP_ExtraBytes.Margin = new System.Windows.Forms.Padding(0, 24, 0, 8);
            this.FLP_ExtraBytes.Name = "FLP_ExtraBytes";
            this.FLP_ExtraBytes.Size = new System.Drawing.Size(272, 24);
            this.FLP_ExtraBytes.TabIndex = 20;
            // 
            // L_ExtraBytes
            // 
            this.L_ExtraBytes.Location = new System.Drawing.Point(0, 0);
            this.L_ExtraBytes.Margin = new System.Windows.Forms.Padding(0);
            this.L_ExtraBytes.Name = "L_ExtraBytes";
            this.L_ExtraBytes.Size = new System.Drawing.Size(128, 24);
            this.L_ExtraBytes.TabIndex = 18;
            this.L_ExtraBytes.Text = "Extra Bytes:";
            this.L_ExtraBytes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_ExtraBytes
            // 
            this.CB_ExtraBytes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_ExtraBytes.FormattingEnabled = true;
            this.CB_ExtraBytes.Location = new System.Drawing.Point(128, 0);
            this.CB_ExtraBytes.Margin = new System.Windows.Forms.Padding(0);
            this.CB_ExtraBytes.Name = "CB_ExtraBytes";
            this.CB_ExtraBytes.Size = new System.Drawing.Size(64, 23);
            this.CB_ExtraBytes.TabIndex = 1;
            this.CB_ExtraBytes.SelectedIndexChanged += new System.EventHandler(this.UpdateExtraByteIndex);
            // 
            // TB_ExtraByte
            // 
            this.TB_ExtraByte.Location = new System.Drawing.Point(200, 0);
            this.TB_ExtraByte.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.TB_ExtraByte.Mask = "000";
            this.TB_ExtraByte.Name = "TB_ExtraByte";
            this.TB_ExtraByte.Size = new System.Drawing.Size(32, 23);
            this.TB_ExtraByte.TabIndex = 2;
            this.TB_ExtraByte.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_ExtraByte.Validated += new System.EventHandler(this.UpdateExtraByteValue);
            // 
            // L_HomeTracker
            // 
            this.L_HomeTracker.Location = new System.Drawing.Point(0, 336);
            this.L_HomeTracker.Margin = new System.Windows.Forms.Padding(0);
            this.L_HomeTracker.Name = "L_HomeTracker";
            this.L_HomeTracker.Size = new System.Drawing.Size(128, 24);
            this.L_HomeTracker.TabIndex = 30;
            this.L_HomeTracker.Text = "HOME Tracker:";
            this.L_HomeTracker.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TB_HomeTracker
            // 
            this.TB_HomeTracker.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_HomeTracker.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TB_HomeTracker.Location = new System.Drawing.Point(128, 338);
            this.TB_HomeTracker.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.TB_HomeTracker.MaxLength = 16;
            this.TB_HomeTracker.Name = "TB_HomeTracker";
            this.TB_HomeTracker.PlaceholderText = "0123456789ABCDEF";
            this.TB_HomeTracker.Size = new System.Drawing.Size(120, 20);
            this.TB_HomeTracker.TabIndex = 31;
            this.TB_HomeTracker.Validated += new System.EventHandler(this.Update_ID64);
            // 
            // Label_EncryptionConstant
            // 
            this.Label_EncryptionConstant.Location = new System.Drawing.Point(0, 360);
            this.Label_EncryptionConstant.Margin = new System.Windows.Forms.Padding(0);
            this.Label_EncryptionConstant.Name = "Label_EncryptionConstant";
            this.Label_EncryptionConstant.Size = new System.Drawing.Size(128, 24);
            this.Label_EncryptionConstant.TabIndex = 32;
            this.Label_EncryptionConstant.Text = "Encryption Constant:";
            this.Label_EncryptionConstant.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TB_EC
            // 
            this.TB_EC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_EC.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TB_EC.Location = new System.Drawing.Point(128, 362);
            this.TB_EC.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.TB_EC.MaxLength = 8;
            this.TB_EC.Name = "TB_EC";
            this.TB_EC.PlaceholderText = "12345678";
            this.TB_EC.Size = new System.Drawing.Size(64, 20);
            this.TB_EC.TabIndex = 33;
            this.TB_EC.Validated += new System.EventHandler(this.Update_ID);
            // 
            // BTN_RerollEC
            // 
            this.BTN_RerollEC.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BTN_RerollEC.Location = new System.Drawing.Point(192, 360);
            this.BTN_RerollEC.Margin = new System.Windows.Forms.Padding(0);
            this.BTN_RerollEC.Name = "BTN_RerollEC";
            this.BTN_RerollEC.Size = new System.Drawing.Size(56, 24);
            this.BTN_RerollEC.TabIndex = 34;
            this.BTN_RerollEC.Text = "Reroll";
            this.BTN_RerollEC.UseVisualStyleBackColor = true;
            this.BTN_RerollEC.Click += new System.EventHandler(this.UpdateRandomEC);
            // 
            // TC_Editor
            // 
            this.TC_Editor.Alignment = System.Windows.Forms.TabAlignment.Right;
            this.TC_Editor.Controls.Add(this.Tab_Main);
            this.TC_Editor.Controls.Add(this.Tab_Met);
            this.TC_Editor.Controls.Add(this.Tab_Stats);
            this.TC_Editor.Controls.Add(this.Tab_Moves);
            this.TC_Editor.Controls.Add(this.Tab_Cosmetic);
            this.TC_Editor.Controls.Add(this.Tab_OTMisc);
            this.TC_Editor.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.TC_Editor.ItemSize = new System.Drawing.Size(40, 96);
            this.TC_Editor.Location = new System.Drawing.Point(0, 56);
            this.TC_Editor.Margin = new System.Windows.Forms.Padding(0);
            this.TC_Editor.Multiline = true;
            this.TC_Editor.Name = "TC_Editor";
            this.TC_Editor.Padding = new System.Drawing.Point(0, 0);
            this.TC_Editor.SelectedIndex = 0;
            this.TC_Editor.Size = new System.Drawing.Size(96, 320);
            this.TC_Editor.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.TC_Editor.TabIndex = 0;
            this.TC_Editor.SelectedIndexChanged += new System.EventHandler(this.ChangeSelectedTabIndex);
            // 
            // Tab_Main
            // 
            this.Tab_Main.Location = new System.Drawing.Point(4, 4);
            this.Tab_Main.Margin = new System.Windows.Forms.Padding(0);
            this.Tab_Main.Name = "Tab_Main";
            this.Tab_Main.Size = new System.Drawing.Size(0, 312);
            this.Tab_Main.TabIndex = 0;
            this.Tab_Main.Text = "Main";
            this.Tab_Main.UseVisualStyleBackColor = true;
            // 
            // Tab_Met
            // 
            this.Tab_Met.Location = new System.Drawing.Point(4, 4);
            this.Tab_Met.Margin = new System.Windows.Forms.Padding(0);
            this.Tab_Met.Name = "Tab_Met";
            this.Tab_Met.Size = new System.Drawing.Size(0, 312);
            this.Tab_Met.TabIndex = 1;
            this.Tab_Met.Text = "Met";
            this.Tab_Met.UseVisualStyleBackColor = true;
            // 
            // Tab_Stats
            // 
            this.Tab_Stats.Location = new System.Drawing.Point(4, 4);
            this.Tab_Stats.Margin = new System.Windows.Forms.Padding(0);
            this.Tab_Stats.Name = "Tab_Stats";
            this.Tab_Stats.Size = new System.Drawing.Size(0, 312);
            this.Tab_Stats.TabIndex = 2;
            this.Tab_Stats.Text = "Stats";
            this.Tab_Stats.UseVisualStyleBackColor = true;
            // 
            // Tab_Moves
            // 
            this.Tab_Moves.Location = new System.Drawing.Point(4, 4);
            this.Tab_Moves.Margin = new System.Windows.Forms.Padding(0);
            this.Tab_Moves.Name = "Tab_Moves";
            this.Tab_Moves.Size = new System.Drawing.Size(0, 312);
            this.Tab_Moves.TabIndex = 3;
            this.Tab_Moves.Text = "Moves";
            this.Tab_Moves.UseVisualStyleBackColor = true;
            // 
            // Tab_Cosmetic
            // 
            this.Tab_Cosmetic.Location = new System.Drawing.Point(4, 4);
            this.Tab_Cosmetic.Name = "Tab_Cosmetic";
            this.Tab_Cosmetic.Size = new System.Drawing.Size(0, 312);
            this.Tab_Cosmetic.TabIndex = 5;
            this.Tab_Cosmetic.Text = "Cosmetic";
            this.Tab_Cosmetic.UseVisualStyleBackColor = true;
            // 
            // Tab_OTMisc
            // 
            this.Tab_OTMisc.Location = new System.Drawing.Point(4, 4);
            this.Tab_OTMisc.Margin = new System.Windows.Forms.Padding(0);
            this.Tab_OTMisc.Name = "Tab_OTMisc";
            this.Tab_OTMisc.Size = new System.Drawing.Size(0, 312);
            this.Tab_OTMisc.TabIndex = 4;
            this.Tab_OTMisc.Text = "OT/Misc";
            this.Tab_OTMisc.UseVisualStyleBackColor = true;
            // 
            // PKMEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.TC_Editor);
            this.Controls.Add(this.Hidden_TC);
            this.Name = "PKMEditor";
            this.Size = new System.Drawing.Size(400, 400);
            this.Hidden_TC.ResumeLayout(false);
            this.Hidden_Main.ResumeLayout(false);
            this.FLP_Main.ResumeLayout(false);
            this.FLP_PID.ResumeLayout(false);
            this.FLP_PIDLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PB_ShinyStar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_ShinySquare)).EndInit();
            this.FLP_PIDRight.ResumeLayout(false);
            this.FLP_PIDRight.PerformLayout();
            this.FLP_Species.ResumeLayout(false);
            this.FLP_Nickname.ResumeLayout(false);
            this.FLP_Nickname.PerformLayout();
            this.FLP_NicknameLeft.ResumeLayout(false);
            this.FLP_NicknameLeft.PerformLayout();
            this.FLP_EXPLevel.ResumeLayout(false);
            this.FLP_EXPLevelRight.ResumeLayout(false);
            this.FLP_EXPLevelRight.PerformLayout();
            this.FLP_Nature.ResumeLayout(false);
            this.FLP_OriginalNature.ResumeLayout(false);
            this.FLP_Form.ResumeLayout(false);
            this.FLP_FormLeft.ResumeLayout(false);
            this.FLP_FormRight.ResumeLayout(false);
            this.FLP_HeldItem.ResumeLayout(false);
            this.FLP_Ability.ResumeLayout(false);
            this.FLP_AbilityRight.ResumeLayout(false);
            this.FLP_AbilityRight.PerformLayout();
            this.FLP_Friendship.ResumeLayout(false);
            this.FLP_FriendshipLeft.ResumeLayout(false);
            this.FLP_FriendshipRight.ResumeLayout(false);
            this.FLP_FriendshipRight.PerformLayout();
            this.FLP_Language.ResumeLayout(false);
            this.FLP_EggPKRS.ResumeLayout(false);
            this.FLP_EggPKRSLeft.ResumeLayout(false);
            this.FLP_EggPKRSLeft.PerformLayout();
            this.FLP_EggPKRSRight.ResumeLayout(false);
            this.FLP_EggPKRSRight.PerformLayout();
            this.FLP_PKRS.ResumeLayout(false);
            this.FLP_PKRSRight.ResumeLayout(false);
            this.FLP_NSparkle.ResumeLayout(false);
            this.FLP_NSparkle.PerformLayout();
            this.FLP_ShadowID.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NUD_ShadowID)).EndInit();
            this.FLP_Purification.ResumeLayout(false);
            this.FLP_Purification.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Purification)).EndInit();
            this.FLP_CatchRate.ResumeLayout(false);
            this.Hidden_Met.ResumeLayout(false);
            this.Hidden_Met.PerformLayout();
            this.GB_EggConditions.ResumeLayout(false);
            this.FLP_Met.ResumeLayout(false);
            this.FLP_OriginGame.ResumeLayout(false);
            this.FLP_BattleVersion.ResumeLayout(false);
            this.FLP_MetLocation.ResumeLayout(false);
            this.FLP_Ball.ResumeLayout(false);
            this.FLP_BallLeft.ResumeLayout(false);
            this.FLP_BallLeft.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Ball)).EndInit();
            this.FLP_MetDate.ResumeLayout(false);
            this.FLP_MetLevel.ResumeLayout(false);
            this.FLP_MetLevel.PerformLayout();
            this.FLP_ObedienceLevel.ResumeLayout(false);
            this.FLP_ObedienceLevel.PerformLayout();
            this.FLP_GroundTile.ResumeLayout(false);
            this.FLP_TimeOfDay.ResumeLayout(false);
            this.Hidden_Stats.ResumeLayout(false);
            this.Hidden_Moves.ResumeLayout(false);
            this.Hidden_Moves.PerformLayout();
            this.FLP_MoveFlags.ResumeLayout(false);
            this.GB_RelearnMoves.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PB_WarnRelearn4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_WarnRelearn3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_WarnRelearn2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_WarnRelearn1)).EndInit();
            this.GB_CurrentMoves.ResumeLayout(false);
            this.FLP_Moves.ResumeLayout(false);
            this.Hidden_Cosmetic.ResumeLayout(false);
            this.Hidden_Cosmetic.PerformLayout();
            this.FLP_PKMEditors.ResumeLayout(false);
            this.FLP_PKMEditors.PerformLayout();
            this.FLP_CosmeticTop.ResumeLayout(false);
            this.FLP_CosmeticTop.PerformLayout();
            this.GB_Markings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PB_Mark6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Mark3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Mark5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_MarkCured)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Mark2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_MarkShiny)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Mark1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Mark4)).EndInit();
            this.FLP_BigMarkings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PB_Favorite)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Origin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_BattleVersion)).EndInit();
            this.Hidden_OTMisc.ResumeLayout(false);
            this.FLP_OTMisc.ResumeLayout(false);
            this.FLP_OTMisc.PerformLayout();
            this.FLP_OT.ResumeLayout(false);
            this.FLP_OT.PerformLayout();
            this.FLP_Country.ResumeLayout(false);
            this.FLP_SubRegion.ResumeLayout(false);
            this.FLP_3DSRegion.ResumeLayout(false);
            this.FLP_Handler.ResumeLayout(false);
            this.FLP_HT.ResumeLayout(false);
            this.FLP_HT.PerformLayout();
            this.FLP_HTLanguage.ResumeLayout(false);
            this.FLP_ExtraBytes.ResumeLayout(false);
            this.FLP_ExtraBytes.PerformLayout();
            this.TC_Editor.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl Hidden_TC;
        private System.Windows.Forms.TabPage Hidden_Main;
        private System.Windows.Forms.FlowLayoutPanel FLP_Main;
        private System.Windows.Forms.FlowLayoutPanel FLP_PID;
        private System.Windows.Forms.FlowLayoutPanel FLP_PIDLeft;
        private System.Windows.Forms.Label Label_PID;
        private System.Windows.Forms.Button BTN_Shinytize;
        private System.Windows.Forms.PictureBox PB_ShinyStar;
        private System.Windows.Forms.PictureBox PB_ShinySquare;
        private System.Windows.Forms.FlowLayoutPanel FLP_PIDRight;
        private System.Windows.Forms.TextBox TB_PID;
        private System.Windows.Forms.Button BTN_RerollPID;
        private System.Windows.Forms.FlowLayoutPanel FLP_Species;
        private System.Windows.Forms.Label Label_Species;
        private System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.FlowLayoutPanel FLP_Nickname;
        private System.Windows.Forms.FlowLayoutPanel FLP_NicknameLeft;
        private System.Windows.Forms.CheckBox CHK_NicknamedFlag;
        private System.Windows.Forms.TextBox TB_Nickname;
        private System.Windows.Forms.FlowLayoutPanel FLP_EXPLevel;
        private System.Windows.Forms.Label Label_EXP;
        private System.Windows.Forms.FlowLayoutPanel FLP_EXPLevelRight;
        private System.Windows.Forms.MaskedTextBox TB_EXP;
        private System.Windows.Forms.Label Label_CurLevel;
        private System.Windows.Forms.MaskedTextBox TB_Level;
        private System.Windows.Forms.FlowLayoutPanel FLP_Nature;
        private System.Windows.Forms.Label Label_Nature;
        private System.Windows.Forms.ComboBox CB_Nature;
        private System.Windows.Forms.FlowLayoutPanel FLP_HeldItem;
        private System.Windows.Forms.Label Label_HeldItem;
        private System.Windows.Forms.ComboBox CB_HeldItem;
        private System.Windows.Forms.FlowLayoutPanel FLP_Friendship;
        private System.Windows.Forms.FlowLayoutPanel FLP_FriendshipLeft;
        private System.Windows.Forms.Label Label_Friendship;
        private System.Windows.Forms.Label Label_HatchCounter;
        private System.Windows.Forms.FlowLayoutPanel FLP_FriendshipRight;
        private System.Windows.Forms.MaskedTextBox TB_Friendship;
        private System.Windows.Forms.Label Label_Form;
        private System.Windows.Forms.FlowLayoutPanel FLP_Ability;
        private System.Windows.Forms.Label Label_Ability;
        private System.Windows.Forms.FlowLayoutPanel FLP_AbilityRight;
        private System.Windows.Forms.ComboBox CB_Ability;
        private System.Windows.Forms.ComboBox DEV_Ability;
        private System.Windows.Forms.MaskedTextBox TB_AbilityNumber;
        private System.Windows.Forms.FlowLayoutPanel FLP_Language;
        private System.Windows.Forms.Label Label_Language;
        private System.Windows.Forms.ComboBox CB_Language;
        private System.Windows.Forms.FlowLayoutPanel FLP_EggPKRS;
        private System.Windows.Forms.FlowLayoutPanel FLP_EggPKRSLeft;
        private System.Windows.Forms.CheckBox CHK_IsEgg;
        private System.Windows.Forms.FlowLayoutPanel FLP_EggPKRSRight;
        private System.Windows.Forms.CheckBox CHK_Infected;
        private System.Windows.Forms.CheckBox CHK_Cured;
        private System.Windows.Forms.FlowLayoutPanel FLP_PKRS;
        private System.Windows.Forms.Label Label_PKRS;
        private System.Windows.Forms.FlowLayoutPanel FLP_PKRSRight;
        private System.Windows.Forms.ComboBox CB_PKRSStrain;
        private System.Windows.Forms.Label Label_PKRSdays;
        private System.Windows.Forms.ComboBox CB_PKRSDays;
        private System.Windows.Forms.FlowLayoutPanel FLP_Country;
        private System.Windows.Forms.Label Label_Country;
        private System.Windows.Forms.ComboBox CB_Country;
        private System.Windows.Forms.FlowLayoutPanel FLP_SubRegion;
        private System.Windows.Forms.Label Label_SubRegion;
        private System.Windows.Forms.ComboBox CB_SubRegion;
        private System.Windows.Forms.FlowLayoutPanel FLP_3DSRegion;
        private System.Windows.Forms.Label Label_3DSRegion;
        private System.Windows.Forms.ComboBox CB_3DSReg;
        private System.Windows.Forms.FlowLayoutPanel FLP_NSparkle;
        private System.Windows.Forms.Label L_NSparkle;
        private System.Windows.Forms.CheckBox CHK_NSparkle;
        private System.Windows.Forms.FlowLayoutPanel FLP_ShadowID;
        private System.Windows.Forms.Label L_ShadowID;
        private System.Windows.Forms.NumericUpDown NUD_ShadowID;
        private System.Windows.Forms.FlowLayoutPanel FLP_Purification;
        private System.Windows.Forms.Label L_HeartGauge;
        private System.Windows.Forms.NumericUpDown NUD_Purification;
        private System.Windows.Forms.CheckBox CHK_Shadow;
        private System.Windows.Forms.TabPage Hidden_Met;
        private System.Windows.Forms.CheckBox CHK_AsEgg;
        private System.Windows.Forms.GroupBox GB_EggConditions;
        private System.Windows.Forms.ComboBox CB_EggLocation;
        private System.Windows.Forms.DateTimePicker CAL_EggDate;
        private System.Windows.Forms.Label Label_EggDate;
        private System.Windows.Forms.Label Label_EggLocation;
        private System.Windows.Forms.FlowLayoutPanel FLP_Met;
        private System.Windows.Forms.FlowLayoutPanel FLP_OriginGame;
        private System.Windows.Forms.Label Label_OriginGame;
        private System.Windows.Forms.ComboBox CB_GameOrigin;
        private System.Windows.Forms.FlowLayoutPanel FLP_MetLocation;
        private System.Windows.Forms.ComboBox CB_MetLocation;
        private System.Windows.Forms.FlowLayoutPanel FLP_Ball;
        private System.Windows.Forms.FlowLayoutPanel FLP_BallLeft;
        private System.Windows.Forms.Label Label_Ball;
        private System.Windows.Forms.PictureBox PB_Ball;
        private System.Windows.Forms.ComboBox CB_Ball;
        private System.Windows.Forms.FlowLayoutPanel FLP_MetLevel;
        private System.Windows.Forms.Label Label_MetLevel;
        private System.Windows.Forms.MaskedTextBox TB_MetLevel;
        private System.Windows.Forms.FlowLayoutPanel FLP_MetDate;
        private System.Windows.Forms.Label Label_MetDate;
        private System.Windows.Forms.DateTimePicker CAL_MetDate;
        private System.Windows.Forms.CheckBox CHK_Fateful;
        private System.Windows.Forms.FlowLayoutPanel FLP_GroundTile;
        private System.Windows.Forms.Label Label_GroundTile;
        private System.Windows.Forms.ComboBox CB_GroundTile;
        private System.Windows.Forms.FlowLayoutPanel FLP_TimeOfDay;
        private System.Windows.Forms.Label L_MetTimeOfDay;
        private System.Windows.Forms.ComboBox CB_MetTimeOfDay;
        private System.Windows.Forms.TabPage Hidden_Stats;
        private System.Windows.Forms.TabPage Hidden_Moves;
        private System.Windows.Forms.GroupBox GB_RelearnMoves;
        private System.Windows.Forms.PictureBox PB_WarnRelearn4;
        private System.Windows.Forms.PictureBox PB_WarnRelearn3;
        private System.Windows.Forms.PictureBox PB_WarnRelearn2;
        private System.Windows.Forms.PictureBox PB_WarnRelearn1;
        private System.Windows.Forms.ComboBox CB_RelearnMove4;
        private System.Windows.Forms.ComboBox CB_RelearnMove3;
        private System.Windows.Forms.ComboBox CB_RelearnMove2;
        private System.Windows.Forms.ComboBox CB_RelearnMove1;
        private System.Windows.Forms.GroupBox GB_CurrentMoves;
        private System.Windows.Forms.Label Label_CurPP;
        private System.Windows.Forms.Label Label_PPups;
        private System.Windows.Forms.TabPage Hidden_OTMisc;
        private System.Windows.Forms.FlowLayoutPanel FLP_PKMEditors;
        private System.Windows.Forms.Button BTN_Ribbons;
        private System.Windows.Forms.Button BTN_Medals;
        private System.Windows.Forms.Button BTN_History;
        private System.Windows.Forms.TextBox TB_EC;
        private System.Windows.Forms.TextBox TB_HT;
        private System.Windows.Forms.Label Label_PrevOT;
        private System.Windows.Forms.Button BTN_RerollEC;
        private System.Windows.Forms.GroupBox GB_Markings;
        private System.Windows.Forms.PictureBox PB_Mark6;
        private System.Windows.Forms.PictureBox PB_Mark3;
        private System.Windows.Forms.PictureBox PB_Mark5;
        private System.Windows.Forms.PictureBox PB_MarkCured;
        private System.Windows.Forms.PictureBox PB_Mark2;
        private System.Windows.Forms.PictureBox PB_MarkShiny;
        private System.Windows.Forms.PictureBox PB_Mark1;
        private System.Windows.Forms.PictureBox PB_Mark4;
        private System.Windows.Forms.MaskedTextBox TB_ExtraByte;
        private System.Windows.Forms.ComboBox CB_ExtraBytes;
        private System.Windows.Forms.TextBox TB_OT;
        private System.Windows.Forms.Label Label_OT;
        private System.Windows.Forms.Label Label_EncryptionConstant;
        private Controls.ShinyLeaf ShinyLeaf;
        private System.Windows.Forms.FlowLayoutPanel FLP_CatchRate;
        private System.Windows.Forms.Label L_CatchRate;
        private CatchRate CR_PK1;
        private SizeCP SizeCP;
        private System.Windows.Forms.PictureBox PB_Favorite;
        private System.Windows.Forms.FlowLayoutPanel FLP_OriginalNature;
        private System.Windows.Forms.Label L_OriginalNature;
        private System.Windows.Forms.ComboBox CB_StatNature;
        private System.Windows.Forms.PictureBox PB_Origin;
        private System.Windows.Forms.ComboBox CB_HTLanguage;
        private System.Windows.Forms.Button B_RelearnFlags;
        private System.Windows.Forms.FlowLayoutPanel FLP_Form;
        private System.Windows.Forms.FlowLayoutPanel FLP_FormLeft;
        private System.Windows.Forms.FlowLayoutPanel FLP_FormRight;
        private System.Windows.Forms.ComboBox CB_Form;
        private System.Windows.Forms.TextBox TB_HomeTracker;
        private System.Windows.Forms.Label L_HomeTracker;
        private System.Windows.Forms.FlowLayoutPanel FLP_BattleVersion;
        private System.Windows.Forms.Label L_BattleVersion;
        private System.Windows.Forms.ComboBox CB_BattleVersion;
        private System.Windows.Forms.PictureBox PB_BattleVersion;
        private FormArgument FA_Form;
        private System.Windows.Forms.Button B_MoveShop;
        private System.Windows.Forms.FlowLayoutPanel FLP_MoveFlags;
        private System.Windows.Forms.Label L_AlphaMastered;
        private System.Windows.Forms.ComboBox CB_AlphaMastered;
        private GenderToggle UC_Gender;
        private GenderToggle UC_HTGender;
        private GenderToggle UC_OTGender;
        private System.Windows.Forms.FlowLayoutPanel FLP_ObedienceLevel;
        private System.Windows.Forms.Label L_ObedienceLevel;
        private System.Windows.Forms.MaskedTextBox TB_ObedienceLevel;
        private System.Windows.Forms.Label L_FormArgument;
        private System.Windows.Forms.Label Label_MetLocation;
        private System.Windows.Forms.FlowLayoutPanel FLP_Moves;
        private MoveChoice MC_Move1;
        private MoveChoice MC_Move2;
        private MoveChoice MC_Move3;
        private MoveChoice MC_Move4;
        private PKHeX.WinForms.Controls.VerticalTabControlEntityEditor TC_Editor;
        private System.Windows.Forms.TabPage Tab_Main;
        private System.Windows.Forms.TabPage Tab_Met;
        private System.Windows.Forms.TabPage Tab_Stats;
        private System.Windows.Forms.TabPage Tab_Moves;
        private System.Windows.Forms.TabPage Tab_OTMisc;
        private System.Windows.Forms.TabPage Hidden_Cosmetic;
        private System.Windows.Forms.TabPage Tab_Cosmetic;
        private System.Windows.Forms.FlowLayoutPanel FLP_CosmeticTop;
        private System.Windows.Forms.FlowLayoutPanel FLP_BigMarkings;
        private StatEditor Stats;
        private ContestStat Contest;
        private TrainerID TID_Trainer;
        private System.Windows.Forms.Label CHK_Nicknamed;
        private System.Windows.Forms.FlowLayoutPanel FLP_OTMisc;
        private System.Windows.Forms.Label GB_OT;
        private System.Windows.Forms.FlowLayoutPanel FLP_OT;
        private System.Windows.Forms.Label GB_nOT;
        private System.Windows.Forms.FlowLayoutPanel FLP_HT;
        private System.Windows.Forms.FlowLayoutPanel FLP_HTLanguage;
        private System.Windows.Forms.Label L_LanguageHT;
        private System.Windows.Forms.FlowLayoutPanel FLP_ExtraBytes;
        private System.Windows.Forms.Label L_ExtraBytes;
        private System.Windows.Forms.FlowLayoutPanel FLP_Handler;
        private System.Windows.Forms.Label L_CurrentHandler;
        private System.Windows.Forms.ComboBox CB_Handler;
    }
}
