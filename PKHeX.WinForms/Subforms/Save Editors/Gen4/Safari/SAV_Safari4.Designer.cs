using PKHeX.WinForms.Controls;

namespace PKHeX.WinForms
{
    partial class SAV_Safari4
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            TC_Editor = new System.Windows.Forms.TabControl();
            Tab_General = new System.Windows.Forms.TabPage();
            TLP_General = new System.Windows.Forms.TableLayoutPanel();
            L_GeneralDayDelay = new System.Windows.Forms.Label();
            NUD_DayDelay = new System.Windows.Forms.NumericUpDown();
            L_GeneralFlags = new System.Windows.Forms.Label();
            L_GeneralElapsedMinutes = new System.Windows.Forms.Label();
            NUD_ElapsedMinutes = new System.Windows.Forms.NumericUpDown();
            L_GeneralObjectUnlockLevel = new System.Windows.Forms.Label();
            NUD_ObjectUnlockLevel = new System.Windows.Forms.NumericUpDown();
            NUD_Flags = new System.Windows.Forms.NumericUpDown();
            NUD_AreaSwitcherState = new System.Windows.Forms.NumericUpDown();
            L_GeneralAreaSwitcherState = new System.Windows.Forms.Label();
            Tab_Self = new System.Windows.Forms.TabPage();
            TC_Self = new System.Windows.Forms.TabControl();
            Tab_SelfLevels = new System.Windows.Forms.TabPage();
            TLP_LevelSelf = new System.Windows.Forms.TableLayoutPanel();
            DGV_SelfAges = new System.Windows.Forms.DataGridView();
            DGV_SelfArea = new System.Windows.Forms.DataGridViewTextBoxColumn();
            DGV_SelfAge = new System.Windows.Forms.DataGridViewTextBoxColumn();
            B_MaxAllSelf = new System.Windows.Forms.Button();
            Tab_SelfArea1 = new System.Windows.Forms.TabPage();
            Tab_SelfArea2 = new System.Windows.Forms.TabPage();
            Tab_SelfArea3 = new System.Windows.Forms.TabPage();
            Tab_SelfArea4 = new System.Windows.Forms.TabPage();
            Tab_SelfArea5 = new System.Windows.Forms.TabPage();
            Tab_SelfArea6 = new System.Windows.Forms.TabPage();
            Tab_Link = new System.Windows.Forms.TabPage();
            TC_Link = new System.Windows.Forms.TabControl();
            Tab_LinkAges = new System.Windows.Forms.TabPage();
            TLP_Link = new System.Windows.Forms.TableLayoutPanel();
            DGV_LinkAges = new System.Windows.Forms.DataGridView();
            DGV_LinkArea = new System.Windows.Forms.DataGridViewTextBoxColumn();
            DGV_LinkAge = new System.Windows.Forms.DataGridViewTextBoxColumn();
            B_MaxAllLink = new System.Windows.Forms.Button();
            Tab_LinkArea1 = new System.Windows.Forms.TabPage();
            Tab_LinkArea2 = new System.Windows.Forms.TabPage();
            Tab_LinkArea3 = new System.Windows.Forms.TabPage();
            Tab_LinkArea4 = new System.Windows.Forms.TabPage();
            Tab_LinkArea5 = new System.Windows.Forms.TabPage();
            Tab_LinkArea6 = new System.Windows.Forms.TabPage();
            Tab_LinkSettings = new System.Windows.Forms.TabPage();
            PG_LinkLeader = new System.Windows.Forms.PropertyGrid();
            FLP_Buttons = new System.Windows.Forms.FlowLayoutPanel();
            B_Save = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            TC_Editor.SuspendLayout();
            Tab_General.SuspendLayout();
            TLP_General.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_DayDelay).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_ElapsedMinutes).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_ObjectUnlockLevel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Flags).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_AreaSwitcherState).BeginInit();
            Tab_Self.SuspendLayout();
            TC_Self.SuspendLayout();
            Tab_SelfLevels.SuspendLayout();
            TLP_LevelSelf.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_SelfAges).BeginInit();
            Tab_Link.SuspendLayout();
            TC_Link.SuspendLayout();
            Tab_LinkAges.SuspendLayout();
            TLP_Link.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_LinkAges).BeginInit();
            Tab_LinkSettings.SuspendLayout();
            FLP_Buttons.SuspendLayout();
            SuspendLayout();
            // 
            // TC_Editor
            // 
            TC_Editor.Controls.Add(Tab_General);
            TC_Editor.Controls.Add(Tab_Self);
            TC_Editor.Controls.Add(Tab_Link);
            TC_Editor.Controls.Add(Tab_LinkSettings);
            TC_Editor.Dock = System.Windows.Forms.DockStyle.Fill;
            TC_Editor.Location = new System.Drawing.Point(0, 0);
            TC_Editor.Margin = new System.Windows.Forms.Padding(0);
            TC_Editor.Name = "TC_Editor";
            TC_Editor.SelectedIndex = 0;
            TC_Editor.Size = new System.Drawing.Size(584, 412);
            TC_Editor.TabIndex = 0;
            // 
            // Tab_General
            // 
            Tab_General.Controls.Add(TLP_General);
            Tab_General.Location = new System.Drawing.Point(4, 26);
            Tab_General.Name = "Tab_General";
            Tab_General.Size = new System.Drawing.Size(576, 382);
            Tab_General.TabIndex = 0;
            Tab_General.Text = "General";
            Tab_General.UseVisualStyleBackColor = true;
            // 
            // TLP_General
            // 
            TLP_General.ColumnCount = 2;
            TLP_General.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_General.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            TLP_General.Controls.Add(L_GeneralDayDelay, 0, 1);
            TLP_General.Controls.Add(NUD_DayDelay, 1, 1);
            TLP_General.Controls.Add(L_GeneralFlags, 0, 2);
            TLP_General.Controls.Add(L_GeneralElapsedMinutes, 0, 3);
            TLP_General.Controls.Add(NUD_ElapsedMinutes, 1, 3);
            TLP_General.Controls.Add(L_GeneralObjectUnlockLevel, 0, 4);
            TLP_General.Controls.Add(NUD_ObjectUnlockLevel, 1, 4);
            TLP_General.Controls.Add(NUD_Flags, 1, 2);
            TLP_General.Controls.Add(NUD_AreaSwitcherState, 1, 5);
            TLP_General.Controls.Add(L_GeneralAreaSwitcherState, 0, 5);
            TLP_General.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_General.Location = new System.Drawing.Point(0, 0);
            TLP_General.Name = "TLP_General";
            TLP_General.RowCount = 7;
            TLP_General.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_General.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_General.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_General.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_General.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_General.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_General.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_General.Size = new System.Drawing.Size(576, 382);
            TLP_General.TabIndex = 0;
            // 
            // L_GeneralDayDelay
            // 
            L_GeneralDayDelay.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_GeneralDayDelay.AutoSize = true;
            L_GeneralDayDelay.Location = new System.Drawing.Point(59, 7);
            L_GeneralDayDelay.Name = "L_GeneralDayDelay";
            L_GeneralDayDelay.Size = new System.Drawing.Size(66, 17);
            L_GeneralDayDelay.TabIndex = 8;
            L_GeneralDayDelay.Text = "Day Delay";
            // 
            // NUD_DayDelay
            // 
            NUD_DayDelay.Location = new System.Drawing.Point(131, 3);
            NUD_DayDelay.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_DayDelay.Name = "NUD_DayDelay";
            NUD_DayDelay.Size = new System.Drawing.Size(120, 25);
            NUD_DayDelay.TabIndex = 3;
            // 
            // L_GeneralFlags
            // 
            L_GeneralFlags.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_GeneralFlags.AutoSize = true;
            L_GeneralFlags.Location = new System.Drawing.Point(87, 38);
            L_GeneralFlags.Name = "L_GeneralFlags";
            L_GeneralFlags.Size = new System.Drawing.Size(38, 17);
            L_GeneralFlags.TabIndex = 9;
            L_GeneralFlags.Text = "Flags";
            // 
            // L_GeneralElapsedMinutes
            // 
            L_GeneralElapsedMinutes.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_GeneralElapsedMinutes.AutoSize = true;
            L_GeneralElapsedMinutes.Location = new System.Drawing.Point(21, 69);
            L_GeneralElapsedMinutes.Name = "L_GeneralElapsedMinutes";
            L_GeneralElapsedMinutes.Size = new System.Drawing.Size(104, 17);
            L_GeneralElapsedMinutes.TabIndex = 10;
            L_GeneralElapsedMinutes.Text = "Elapsed Minutes";
            // 
            // NUD_ElapsedMinutes
            // 
            NUD_ElapsedMinutes.Location = new System.Drawing.Point(131, 65);
            NUD_ElapsedMinutes.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NUD_ElapsedMinutes.Name = "NUD_ElapsedMinutes";
            NUD_ElapsedMinutes.Size = new System.Drawing.Size(120, 25);
            NUD_ElapsedMinutes.TabIndex = 5;
            // 
            // L_GeneralObjectUnlockLevel
            // 
            L_GeneralObjectUnlockLevel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_GeneralObjectUnlockLevel.AutoSize = true;
            L_GeneralObjectUnlockLevel.Location = new System.Drawing.Point(3, 100);
            L_GeneralObjectUnlockLevel.Name = "L_GeneralObjectUnlockLevel";
            L_GeneralObjectUnlockLevel.Size = new System.Drawing.Size(122, 17);
            L_GeneralObjectUnlockLevel.TabIndex = 11;
            L_GeneralObjectUnlockLevel.Text = "Object Unlock Level";
            // 
            // NUD_ObjectUnlockLevel
            // 
            NUD_ObjectUnlockLevel.Location = new System.Drawing.Point(131, 96);
            NUD_ObjectUnlockLevel.Maximum = new decimal(new int[] { 63, 0, 0, 0 });
            NUD_ObjectUnlockLevel.Name = "NUD_ObjectUnlockLevel";
            NUD_ObjectUnlockLevel.Size = new System.Drawing.Size(120, 25);
            NUD_ObjectUnlockLevel.TabIndex = 6;
            // 
            // NUD_Flags
            // 
            NUD_Flags.Location = new System.Drawing.Point(131, 34);
            NUD_Flags.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_Flags.Name = "NUD_Flags";
            NUD_Flags.Size = new System.Drawing.Size(120, 25);
            NUD_Flags.TabIndex = 4;
            // 
            // NUD_AreaSwitcherState
            // 
            NUD_AreaSwitcherState.Location = new System.Drawing.Point(131, 127);
            NUD_AreaSwitcherState.Maximum = new decimal(new int[] { 3, 0, 0, 0 });
            NUD_AreaSwitcherState.Name = "NUD_AreaSwitcherState";
            NUD_AreaSwitcherState.Size = new System.Drawing.Size(120, 25);
            NUD_AreaSwitcherState.TabIndex = 7;
            // 
            // L_GeneralAreaSwitcherState
            // 
            L_GeneralAreaSwitcherState.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_GeneralAreaSwitcherState.AutoSize = true;
            L_GeneralAreaSwitcherState.Location = new System.Drawing.Point(5, 131);
            L_GeneralAreaSwitcherState.Name = "L_GeneralAreaSwitcherState";
            L_GeneralAreaSwitcherState.Size = new System.Drawing.Size(120, 17);
            L_GeneralAreaSwitcherState.TabIndex = 12;
            L_GeneralAreaSwitcherState.Text = "Area Switcher State";
            // 
            // Tab_Self
            // 
            Tab_Self.Controls.Add(TC_Self);
            Tab_Self.Location = new System.Drawing.Point(4, 26);
            Tab_Self.Name = "Tab_Self";
            Tab_Self.Size = new System.Drawing.Size(604, 382);
            Tab_Self.TabIndex = 1;
            Tab_Self.Text = "Self";
            Tab_Self.UseVisualStyleBackColor = true;
            // 
            // TC_Self
            // 
            TC_Self.Controls.Add(Tab_SelfLevels);
            TC_Self.Controls.Add(Tab_SelfArea1);
            TC_Self.Controls.Add(Tab_SelfArea2);
            TC_Self.Controls.Add(Tab_SelfArea3);
            TC_Self.Controls.Add(Tab_SelfArea4);
            TC_Self.Controls.Add(Tab_SelfArea5);
            TC_Self.Controls.Add(Tab_SelfArea6);
            TC_Self.Dock = System.Windows.Forms.DockStyle.Fill;
            TC_Self.Location = new System.Drawing.Point(0, 0);
            TC_Self.Name = "TC_Self";
            TC_Self.SelectedIndex = 0;
            TC_Self.Size = new System.Drawing.Size(604, 382);
            TC_Self.TabIndex = 0;
            // 
            // Tab_SelfLevels
            // 
            Tab_SelfLevels.Controls.Add(TLP_LevelSelf);
            Tab_SelfLevels.Location = new System.Drawing.Point(4, 26);
            Tab_SelfLevels.Name = "Tab_SelfLevels";
            Tab_SelfLevels.Size = new System.Drawing.Size(596, 352);
            Tab_SelfLevels.TabIndex = 7;
            Tab_SelfLevels.Text = "Levels";
            Tab_SelfLevels.UseVisualStyleBackColor = true;
            // 
            // TLP_LevelSelf
            // 
            TLP_LevelSelf.ColumnCount = 1;
            TLP_LevelSelf.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_LevelSelf.Controls.Add(DGV_SelfAges, 0, 1);
            TLP_LevelSelf.Controls.Add(B_MaxAllSelf, 0, 0);
            TLP_LevelSelf.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_LevelSelf.Location = new System.Drawing.Point(0, 0);
            TLP_LevelSelf.Name = "TLP_LevelSelf";
            TLP_LevelSelf.RowCount = 2;
            TLP_LevelSelf.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_LevelSelf.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_LevelSelf.Size = new System.Drawing.Size(596, 352);
            TLP_LevelSelf.TabIndex = 1;
            // 
            // DGV_SelfAges
            // 
            DGV_SelfAges.AllowUserToAddRows = false;
            DGV_SelfAges.AllowUserToDeleteRows = false;
            DGV_SelfAges.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            DGV_SelfAges.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            DGV_SelfAges.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_SelfAges.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { DGV_SelfArea, DGV_SelfAge });
            DGV_SelfAges.Dock = System.Windows.Forms.DockStyle.Fill;
            DGV_SelfAges.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            DGV_SelfAges.Location = new System.Drawing.Point(3, 36);
            DGV_SelfAges.Name = "DGV_SelfAges";
            DGV_SelfAges.RowHeadersVisible = false;
            DGV_SelfAges.Size = new System.Drawing.Size(590, 313);
            DGV_SelfAges.TabIndex = 0;
            // 
            // DGV_SelfArea
            // 
            DGV_SelfArea.HeaderText = "Area";
            DGV_SelfArea.Name = "DGV_SelfArea";
            DGV_SelfArea.ReadOnly = true;
            DGV_SelfArea.Width = 60;
            // 
            // DGV_SelfAge
            // 
            DGV_SelfAge.HeaderText = "Age";
            DGV_SelfAge.MaxInputLength = 3;
            DGV_SelfAge.Name = "DGV_SelfAge";
            DGV_SelfAge.Width = 56;
            // 
            // B_MaxAllSelf
            // 
            B_MaxAllSelf.AutoSize = true;
            B_MaxAllSelf.Location = new System.Drawing.Point(3, 3);
            B_MaxAllSelf.Name = "B_MaxAllSelf";
            B_MaxAllSelf.Size = new System.Drawing.Size(152, 27);
            B_MaxAllSelf.TabIndex = 1;
            B_MaxAllSelf.Text = "Max All Ages";
            B_MaxAllSelf.UseVisualStyleBackColor = true;
            B_MaxAllSelf.Click += B_MaxAllSelf_Click;
            // 
            // Tab_SelfArea1
            // 
            Tab_SelfArea1.Location = new System.Drawing.Point(4, 26);
            Tab_SelfArea1.Name = "Tab_SelfArea1";
            Tab_SelfArea1.Size = new System.Drawing.Size(596, 352);
            Tab_SelfArea1.TabIndex = 0;
            Tab_SelfArea1.Text = "Area 1";
            Tab_SelfArea1.UseVisualStyleBackColor = true;
            // 
            // Tab_SelfArea2
            // 
            Tab_SelfArea2.Location = new System.Drawing.Point(4, 26);
            Tab_SelfArea2.Name = "Tab_SelfArea2";
            Tab_SelfArea2.Size = new System.Drawing.Size(596, 352);
            Tab_SelfArea2.TabIndex = 2;
            Tab_SelfArea2.Text = "Area 2";
            Tab_SelfArea2.UseVisualStyleBackColor = true;
            // 
            // Tab_SelfArea3
            // 
            Tab_SelfArea3.Location = new System.Drawing.Point(4, 26);
            Tab_SelfArea3.Name = "Tab_SelfArea3";
            Tab_SelfArea3.Size = new System.Drawing.Size(596, 352);
            Tab_SelfArea3.TabIndex = 3;
            Tab_SelfArea3.Text = "Area 3";
            Tab_SelfArea3.UseVisualStyleBackColor = true;
            // 
            // Tab_SelfArea4
            // 
            Tab_SelfArea4.Location = new System.Drawing.Point(4, 26);
            Tab_SelfArea4.Name = "Tab_SelfArea4";
            Tab_SelfArea4.Size = new System.Drawing.Size(596, 352);
            Tab_SelfArea4.TabIndex = 4;
            Tab_SelfArea4.Text = "Area 4";
            Tab_SelfArea4.UseVisualStyleBackColor = true;
            // 
            // Tab_SelfArea5
            // 
            Tab_SelfArea5.Location = new System.Drawing.Point(4, 26);
            Tab_SelfArea5.Name = "Tab_SelfArea5";
            Tab_SelfArea5.Size = new System.Drawing.Size(596, 352);
            Tab_SelfArea5.TabIndex = 5;
            Tab_SelfArea5.Text = "Area 5";
            Tab_SelfArea5.UseVisualStyleBackColor = true;
            // 
            // Tab_SelfArea6
            // 
            Tab_SelfArea6.Location = new System.Drawing.Point(4, 26);
            Tab_SelfArea6.Name = "Tab_SelfArea6";
            Tab_SelfArea6.Size = new System.Drawing.Size(596, 352);
            Tab_SelfArea6.TabIndex = 6;
            Tab_SelfArea6.Text = "Area 6";
            Tab_SelfArea6.UseVisualStyleBackColor = true;
            // 
            // Tab_Link
            // 
            Tab_Link.Controls.Add(TC_Link);
            Tab_Link.Location = new System.Drawing.Point(4, 26);
            Tab_Link.Name = "Tab_Link";
            Tab_Link.Size = new System.Drawing.Size(604, 382);
            Tab_Link.TabIndex = 2;
            Tab_Link.Text = "Link";
            Tab_Link.UseVisualStyleBackColor = true;
            // 
            // TC_Link
            // 
            TC_Link.Controls.Add(Tab_LinkAges);
            TC_Link.Controls.Add(Tab_LinkArea1);
            TC_Link.Controls.Add(Tab_LinkArea2);
            TC_Link.Controls.Add(Tab_LinkArea3);
            TC_Link.Controls.Add(Tab_LinkArea4);
            TC_Link.Controls.Add(Tab_LinkArea5);
            TC_Link.Controls.Add(Tab_LinkArea6);
            TC_Link.Dock = System.Windows.Forms.DockStyle.Fill;
            TC_Link.Location = new System.Drawing.Point(0, 0);
            TC_Link.Name = "TC_Link";
            TC_Link.SelectedIndex = 0;
            TC_Link.Size = new System.Drawing.Size(604, 382);
            TC_Link.TabIndex = 1;
            // 
            // Tab_LinkAges
            // 
            Tab_LinkAges.Controls.Add(TLP_Link);
            Tab_LinkAges.Location = new System.Drawing.Point(4, 26);
            Tab_LinkAges.Name = "Tab_LinkAges";
            Tab_LinkAges.Size = new System.Drawing.Size(596, 352);
            Tab_LinkAges.TabIndex = 7;
            Tab_LinkAges.Text = "Ages";
            Tab_LinkAges.UseVisualStyleBackColor = true;
            // 
            // TLP_Link
            // 
            TLP_Link.ColumnCount = 1;
            TLP_Link.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_Link.Controls.Add(DGV_LinkAges, 0, 1);
            TLP_Link.Controls.Add(B_MaxAllLink, 0, 0);
            TLP_Link.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Link.Location = new System.Drawing.Point(0, 0);
            TLP_Link.Name = "TLP_Link";
            TLP_Link.RowCount = 2;
            TLP_Link.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Link.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Link.Size = new System.Drawing.Size(596, 352);
            TLP_Link.TabIndex = 2;
            // 
            // DGV_LinkAges
            // 
            DGV_LinkAges.AllowUserToAddRows = false;
            DGV_LinkAges.AllowUserToDeleteRows = false;
            DGV_LinkAges.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            DGV_LinkAges.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            DGV_LinkAges.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_LinkAges.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { DGV_LinkArea, DGV_LinkAge });
            DGV_LinkAges.Dock = System.Windows.Forms.DockStyle.Fill;
            DGV_LinkAges.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            DGV_LinkAges.Location = new System.Drawing.Point(3, 36);
            DGV_LinkAges.Name = "DGV_LinkAges";
            DGV_LinkAges.RowHeadersVisible = false;
            DGV_LinkAges.Size = new System.Drawing.Size(590, 313);
            DGV_LinkAges.TabIndex = 1;
            // 
            // DGV_LinkArea
            // 
            DGV_LinkArea.HeaderText = "Area";
            DGV_LinkArea.Name = "DGV_LinkArea";
            DGV_LinkArea.ReadOnly = true;
            DGV_LinkArea.Width = 60;
            // 
            // DGV_LinkAge
            // 
            DGV_LinkAge.HeaderText = "Age";
            DGV_LinkAge.MaxInputLength = 3;
            DGV_LinkAge.Name = "DGV_LinkAge";
            DGV_LinkAge.Width = 56;
            // 
            // B_MaxAllLink
            // 
            B_MaxAllLink.Location = new System.Drawing.Point(3, 3);
            B_MaxAllLink.Name = "B_MaxAllLink";
            B_MaxAllLink.Size = new System.Drawing.Size(152, 27);
            B_MaxAllLink.TabIndex = 2;
            B_MaxAllLink.Text = "Max All Ages";
            B_MaxAllLink.UseVisualStyleBackColor = true;
            B_MaxAllLink.Click += B_MaxAllLink_Click;
            // 
            // Tab_LinkArea1
            // 
            Tab_LinkArea1.Location = new System.Drawing.Point(4, 26);
            Tab_LinkArea1.Name = "Tab_LinkArea1";
            Tab_LinkArea1.Size = new System.Drawing.Size(596, 352);
            Tab_LinkArea1.TabIndex = 0;
            Tab_LinkArea1.Text = "Area 1";
            Tab_LinkArea1.UseVisualStyleBackColor = true;
            // 
            // Tab_LinkArea2
            // 
            Tab_LinkArea2.Location = new System.Drawing.Point(4, 26);
            Tab_LinkArea2.Name = "Tab_LinkArea2";
            Tab_LinkArea2.Size = new System.Drawing.Size(596, 352);
            Tab_LinkArea2.TabIndex = 2;
            Tab_LinkArea2.Text = "Area 2";
            Tab_LinkArea2.UseVisualStyleBackColor = true;
            // 
            // Tab_LinkArea3
            // 
            Tab_LinkArea3.Location = new System.Drawing.Point(4, 26);
            Tab_LinkArea3.Name = "Tab_LinkArea3";
            Tab_LinkArea3.Size = new System.Drawing.Size(596, 352);
            Tab_LinkArea3.TabIndex = 3;
            Tab_LinkArea3.Text = "Area 3";
            Tab_LinkArea3.UseVisualStyleBackColor = true;
            // 
            // Tab_LinkArea4
            // 
            Tab_LinkArea4.Location = new System.Drawing.Point(4, 26);
            Tab_LinkArea4.Name = "Tab_LinkArea4";
            Tab_LinkArea4.Size = new System.Drawing.Size(596, 352);
            Tab_LinkArea4.TabIndex = 4;
            Tab_LinkArea4.Text = "Area 4";
            Tab_LinkArea4.UseVisualStyleBackColor = true;
            // 
            // Tab_LinkArea5
            // 
            Tab_LinkArea5.Location = new System.Drawing.Point(4, 26);
            Tab_LinkArea5.Name = "Tab_LinkArea5";
            Tab_LinkArea5.Size = new System.Drawing.Size(596, 352);
            Tab_LinkArea5.TabIndex = 5;
            Tab_LinkArea5.Text = "Area 5";
            Tab_LinkArea5.UseVisualStyleBackColor = true;
            // 
            // Tab_LinkArea6
            // 
            Tab_LinkArea6.Location = new System.Drawing.Point(4, 26);
            Tab_LinkArea6.Name = "Tab_LinkArea6";
            Tab_LinkArea6.Size = new System.Drawing.Size(596, 352);
            Tab_LinkArea6.TabIndex = 6;
            Tab_LinkArea6.Text = "Area 6";
            Tab_LinkArea6.UseVisualStyleBackColor = true;
            // 
            // Tab_LinkSettings
            // 
            Tab_LinkSettings.Controls.Add(PG_LinkLeader);
            Tab_LinkSettings.Location = new System.Drawing.Point(4, 26);
            Tab_LinkSettings.Name = "Tab_LinkSettings";
            Tab_LinkSettings.Size = new System.Drawing.Size(604, 382);
            Tab_LinkSettings.TabIndex = 3;
            Tab_LinkSettings.Text = "Link Settings";
            Tab_LinkSettings.UseVisualStyleBackColor = true;
            // 
            // PG_LinkLeader
            // 
            PG_LinkLeader.BackColor = System.Drawing.SystemColors.Control;
            PG_LinkLeader.Dock = System.Windows.Forms.DockStyle.Fill;
            PG_LinkLeader.Location = new System.Drawing.Point(0, 0);
            PG_LinkLeader.Name = "PG_LinkLeader";
            PG_LinkLeader.Size = new System.Drawing.Size(604, 382);
            PG_LinkLeader.TabIndex = 0;
            // 
            // FLP_Buttons
            // 
            FLP_Buttons.AutoSize = true;
            FLP_Buttons.Controls.Add(B_Save);
            FLP_Buttons.Controls.Add(B_Cancel);
            FLP_Buttons.Dock = System.Windows.Forms.DockStyle.Bottom;
            FLP_Buttons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            FLP_Buttons.Location = new System.Drawing.Point(0, 412);
            FLP_Buttons.Name = "FLP_Buttons";
            FLP_Buttons.Padding = new System.Windows.Forms.Padding(8);
            FLP_Buttons.Size = new System.Drawing.Size(584, 49);
            FLP_Buttons.TabIndex = 1;
            // 
            // B_Save
            // 
            B_Save.AutoSize = true;
            B_Save.Location = new System.Drawing.Point(485, 11);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(80, 27);
            B_Save.TabIndex = 1;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.AutoSize = true;
            B_Cancel.Location = new System.Drawing.Point(399, 11);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(80, 27);
            B_Cancel.TabIndex = 0;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // SAV_Safari4
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(584, 461);
            Controls.Add(TC_Editor);
            Controls.Add(FLP_Buttons);
            Icon = Properties.Resources.Icon;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(600, 500);
            Name = "SAV_Safari4";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Safari Zone Editor";
            TC_Editor.ResumeLayout(false);
            Tab_General.ResumeLayout(false);
            TLP_General.ResumeLayout(false);
            TLP_General.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_DayDelay).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_ElapsedMinutes).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_ObjectUnlockLevel).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Flags).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_AreaSwitcherState).EndInit();
            Tab_Self.ResumeLayout(false);
            TC_Self.ResumeLayout(false);
            Tab_SelfLevels.ResumeLayout(false);
            TLP_LevelSelf.ResumeLayout(false);
            TLP_LevelSelf.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_SelfAges).EndInit();
            Tab_Link.ResumeLayout(false);
            TC_Link.ResumeLayout(false);
            Tab_LinkAges.ResumeLayout(false);
            TLP_Link.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DGV_LinkAges).EndInit();
            Tab_LinkSettings.ResumeLayout(false);
            FLP_Buttons.ResumeLayout(false);
            FLP_Buttons.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TabControl TC_Editor;
        private System.Windows.Forms.TabPage Tab_General;
        private System.Windows.Forms.FlowLayoutPanel FLP_Buttons;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.TabPage Tab_Self;
        private System.Windows.Forms.TabControl TC_Self;
        private System.Windows.Forms.TabPage Tab_SelfArea1;
        private System.Windows.Forms.TabPage Tab_SelfArea2;
        private System.Windows.Forms.TabPage Tab_SelfArea3;
        private System.Windows.Forms.TabPage Tab_SelfArea4;
        private System.Windows.Forms.TabPage Tab_SelfArea5;
        private System.Windows.Forms.TabPage Tab_SelfArea6;
        private System.Windows.Forms.TabPage Tab_Link;
        private System.Windows.Forms.TabPage Tab_LinkSettings;
        private System.Windows.Forms.TabControl TC_Link;
        private System.Windows.Forms.TabPage Tab_LinkArea1;
        private System.Windows.Forms.TabPage Tab_LinkArea2;
        private System.Windows.Forms.TabPage Tab_LinkArea3;
        private System.Windows.Forms.TabPage Tab_LinkArea4;
        private System.Windows.Forms.TabPage Tab_LinkArea5;
        private System.Windows.Forms.TabPage Tab_LinkArea6;
        private System.Windows.Forms.PropertyGrid PG_LinkLeader;
        private System.Windows.Forms.TableLayoutPanel TLP_General;
        private System.Windows.Forms.Label L_GeneralDayDelay;
        private System.Windows.Forms.Label L_GeneralFlags;
        private System.Windows.Forms.Label L_GeneralElapsedMinutes;
        private System.Windows.Forms.Label L_GeneralObjectUnlockLevel;
        private System.Windows.Forms.NumericUpDown NUD_DayDelay;
        private System.Windows.Forms.NumericUpDown NUD_Flags;
        private System.Windows.Forms.NumericUpDown NUD_ElapsedMinutes;
        private System.Windows.Forms.NumericUpDown NUD_ObjectUnlockLevel;
        private System.Windows.Forms.TabPage Tab_SelfLevels;
        private System.Windows.Forms.TabPage Tab_LinkAges;
        private System.Windows.Forms.DataGridView DGV_LinkAges;
        private System.Windows.Forms.DataGridViewTextBoxColumn DGV_LinkArea;
        private System.Windows.Forms.DataGridViewTextBoxColumn DGV_LinkAge;
        private System.Windows.Forms.NumericUpDown NUD_AreaSwitcherState;
        private System.Windows.Forms.Label L_GeneralAreaSwitcherState;
        private System.Windows.Forms.TableLayoutPanel TLP_LevelSelf;
        private System.Windows.Forms.DataGridView DGV_SelfAges;
        private System.Windows.Forms.DataGridViewTextBoxColumn DGV_SelfArea;
        private System.Windows.Forms.DataGridViewTextBoxColumn DGV_SelfAge;
        private System.Windows.Forms.Button B_MaxAllSelf;
        private System.Windows.Forms.TableLayoutPanel TLP_Link;
        private System.Windows.Forms.Button B_MaxAllLink;
    }
}
