using PKHeX.WinForms.Controls;

namespace PKHeX.WinForms
{
    partial class SAV_Pokeathlon4
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            TC_Editor = new System.Windows.Forms.TabControl();
            Tab_General = new System.Windows.Forms.TabPage();
            TLP_General = new System.Windows.Forms.TableLayoutPanel();
            L_Points = new System.Windows.Forms.Label();
            NUD_Points = new System.Windows.Forms.NumericUpDown();
            L_DailyShopFlags = new System.Windows.Forms.Label();
            FLP_DailyShopFlags = new System.Windows.Forms.FlowLayoutPanel();
            CHK_DailyShop0 = new System.Windows.Forms.CheckBox();
            CHK_DailyShop1 = new System.Windows.Forms.CheckBox();
            CHK_DailyShop2 = new System.Windows.Forms.CheckBox();
            CHK_DailyShop3 = new System.Windows.Forms.CheckBox();
            CHK_DailyShop4 = new System.Windows.Forms.CheckBox();
            CHK_DailyShop5 = new System.Windows.Forms.CheckBox();
            CHK_DailyShop6 = new System.Windows.Forms.CheckBox();
            CHK_DailyShop7 = new System.Windows.Forms.CheckBox();
            CHK_DailyShop8 = new System.Windows.Forms.CheckBox();
            CHK_DailyShop9 = new System.Windows.Forms.CheckBox();
            CHK_DailyShop10 = new System.Windows.Forms.CheckBox();
            CHK_DailyShop11 = new System.Windows.Forms.CheckBox();
            L_DataCards = new System.Windows.Forms.Label();
            CLB_DataCards = new System.Windows.Forms.CheckedListBox();
            Tab_Medals = new System.Windows.Forms.TabPage();
            FLP_Medals = new System.Windows.Forms.FlowLayoutPanel();
            B_MedalsGiveAll = new System.Windows.Forms.Button();
            B_MedalsClearAll = new System.Windows.Forms.Button();
            DGV_Medals = new DoubleBufferedDataGridView();
            Tab_Counters = new System.Windows.Forms.TabPage();
            TLP_Counters = new System.Windows.Forms.TableLayoutPanel();
            Tab_Best = new System.Windows.Forms.TabPage();
            TLP_Best = new System.Windows.Forms.TableLayoutPanel();
            Tab_Courses = new System.Windows.Forms.TabPage();
            TLP_Courses = new System.Windows.Forms.TableLayoutPanel();
            L_CourseIndex = new System.Windows.Forms.Label();
            CB_CourseIndex = new System.Windows.Forms.ComboBox();
            TLP_CourseScore = new System.Windows.Forms.TableLayoutPanel();
            L_CourseScore0 = new System.Windows.Forms.Label();
            NUD_CourseScore0 = new System.Windows.Forms.NumericUpDown();
            L_CourseScore1 = new System.Windows.Forms.Label();
            NUD_CourseScore1 = new System.Windows.Forms.NumericUpDown();
            L_CourseScore2 = new System.Windows.Forms.Label();
            NUD_CourseScore2 = new System.Windows.Forms.NumericUpDown();
            L_CourseScoreMax = new System.Windows.Forms.Label();
            NUD_CourseScoreMax = new System.Windows.Forms.NumericUpDown();
            L_CourseParticipant0 = new System.Windows.Forms.Label();
            UC_CourseParticipant0 = new PokeathlonParticipant4Editor();
            L_CourseParticipant1 = new System.Windows.Forms.Label();
            UC_CourseParticipant1 = new PokeathlonParticipant4Editor();
            L_CourseParticipant2 = new System.Windows.Forms.Label();
            UC_CourseParticipant2 = new PokeathlonParticipant4Editor();
            Tab_SelfEvent = new System.Windows.Forms.TabPage();
            TLP_SelfEvent = new System.Windows.Forms.TableLayoutPanel();
            L_SelfEventIndex = new System.Windows.Forms.Label();
            CB_SelfEventIndex = new System.Windows.Forms.ComboBox();
            UC_SelfEventData = new PokeathlonEventData4Editor();
            Tab_Connection = new System.Windows.Forms.TabPage();
            TLP_Connection = new System.Windows.Forms.TableLayoutPanel();
            L_ConnectionIndex = new System.Windows.Forms.Label();
            CB_ConnectionIndex = new System.Windows.Forms.ComboBox();
            UC_Connection = new PokeathlonConnection4Editor();
            FLP_Buttons = new System.Windows.Forms.FlowLayoutPanel();
            B_Cancel = new System.Windows.Forms.Button();
            B_Save = new System.Windows.Forms.Button();
            TLP_Medals = new System.Windows.Forms.TableLayoutPanel();
            TC_Editor.SuspendLayout();
            Tab_General.SuspendLayout();
            TLP_General.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Points).BeginInit();
            FLP_DailyShopFlags.SuspendLayout();
            Tab_Medals.SuspendLayout();
            FLP_Medals.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_Medals).BeginInit();
            Tab_Counters.SuspendLayout();
            Tab_Best.SuspendLayout();
            Tab_Courses.SuspendLayout();
            TLP_Courses.SuspendLayout();
            TLP_CourseScore.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_CourseScore0).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_CourseScore1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_CourseScore2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_CourseScoreMax).BeginInit();
            Tab_SelfEvent.SuspendLayout();
            TLP_SelfEvent.SuspendLayout();
            Tab_Connection.SuspendLayout();
            TLP_Connection.SuspendLayout();
            FLP_Buttons.SuspendLayout();
            TLP_Medals.SuspendLayout();
            SuspendLayout();
            // 
            // TC_Editor
            // 
            TC_Editor.Controls.Add(Tab_General);
            TC_Editor.Controls.Add(Tab_Medals);
            TC_Editor.Controls.Add(Tab_Counters);
            TC_Editor.Controls.Add(Tab_Best);
            TC_Editor.Controls.Add(Tab_Courses);
            TC_Editor.Controls.Add(Tab_SelfEvent);
            TC_Editor.Controls.Add(Tab_Connection);
            TC_Editor.Dock = System.Windows.Forms.DockStyle.Fill;
            TC_Editor.Location = new System.Drawing.Point(0, 0);
            TC_Editor.Margin = new System.Windows.Forms.Padding(0);
            TC_Editor.Name = "TC_Editor";
            TC_Editor.SelectedIndex = 0;
            TC_Editor.Size = new System.Drawing.Size(612, 924);
            TC_Editor.TabIndex = 0;
            // 
            // Tab_General
            // 
            Tab_General.Controls.Add(TLP_General);
            Tab_General.Location = new System.Drawing.Point(4, 26);
            Tab_General.Name = "Tab_General";
            Tab_General.Padding = new System.Windows.Forms.Padding(3);
            Tab_General.Size = new System.Drawing.Size(604, 895);
            Tab_General.TabIndex = 0;
            Tab_General.Text = "General";
            Tab_General.UseVisualStyleBackColor = true;
            // 
            // TLP_General
            // 
            TLP_General.ColumnCount = 2;
            TLP_General.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_General.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_General.Controls.Add(L_Points, 0, 0);
            TLP_General.Controls.Add(NUD_Points, 1, 0);
            TLP_General.Controls.Add(L_DailyShopFlags, 0, 1);
            TLP_General.Controls.Add(FLP_DailyShopFlags, 1, 1);
            TLP_General.Controls.Add(L_DataCards, 0, 2);
            TLP_General.Controls.Add(CLB_DataCards, 1, 2);
            TLP_General.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_General.Location = new System.Drawing.Point(3, 3);
            TLP_General.Name = "TLP_General";
            TLP_General.Padding = new System.Windows.Forms.Padding(8);
            TLP_General.RowCount = 3;
            TLP_General.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_General.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_General.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_General.Size = new System.Drawing.Size(598, 889);
            TLP_General.TabIndex = 0;
            // 
            // L_Points
            // 
            L_Points.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Points.AutoSize = true;
            L_Points.Location = new System.Drawing.Point(38, 12);
            L_Points.Margin = new System.Windows.Forms.Padding(0);
            L_Points.Name = "L_Points";
            L_Points.Size = new System.Drawing.Size(46, 17);
            L_Points.TabIndex = 0;
            L_Points.Text = "Points:";
            // 
            // NUD_Points
            // 
            NUD_Points.Location = new System.Drawing.Point(84, 8);
            NUD_Points.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            NUD_Points.Maximum = new decimal(new int[] { 99999, 0, 0, 0 });
            NUD_Points.Name = "NUD_Points";
            NUD_Points.Size = new System.Drawing.Size(120, 25);
            NUD_Points.TabIndex = 1;
            // 
            // L_DailyShopFlags
            // 
            L_DailyShopFlags.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_DailyShopFlags.AutoSize = true;
            L_DailyShopFlags.Location = new System.Drawing.Point(11, 35);
            L_DailyShopFlags.Margin = new System.Windows.Forms.Padding(0);
            L_DailyShopFlags.Name = "L_DailyShopFlags";
            L_DailyShopFlags.Size = new System.Drawing.Size(73, 17);
            L_DailyShopFlags.TabIndex = 2;
            L_DailyShopFlags.Text = "Daily Shop:";
            // 
            // FLP_DailyShopFlags
            // 
            FLP_DailyShopFlags.AutoSize = true;
            FLP_DailyShopFlags.Controls.Add(CHK_DailyShop0);
            FLP_DailyShopFlags.Controls.Add(CHK_DailyShop1);
            FLP_DailyShopFlags.Controls.Add(CHK_DailyShop2);
            FLP_DailyShopFlags.Controls.Add(CHK_DailyShop3);
            FLP_DailyShopFlags.Controls.Add(CHK_DailyShop4);
            FLP_DailyShopFlags.Controls.Add(CHK_DailyShop5);
            FLP_DailyShopFlags.Controls.Add(CHK_DailyShop6);
            FLP_DailyShopFlags.Controls.Add(CHK_DailyShop7);
            FLP_DailyShopFlags.Controls.Add(CHK_DailyShop8);
            FLP_DailyShopFlags.Controls.Add(CHK_DailyShop9);
            FLP_DailyShopFlags.Controls.Add(CHK_DailyShop10);
            FLP_DailyShopFlags.Controls.Add(CHK_DailyShop11);
            FLP_DailyShopFlags.Dock = System.Windows.Forms.DockStyle.Fill;
            FLP_DailyShopFlags.Location = new System.Drawing.Point(84, 34);
            FLP_DailyShopFlags.Margin = new System.Windows.Forms.Padding(0);
            FLP_DailyShopFlags.Name = "FLP_DailyShopFlags";
            FLP_DailyShopFlags.Size = new System.Drawing.Size(506, 20);
            FLP_DailyShopFlags.TabIndex = 3;
            // 
            // CHK_DailyShop0
            // 
            CHK_DailyShop0.AutoSize = true;
            CHK_DailyShop0.Location = new System.Drawing.Point(3, 3);
            CHK_DailyShop0.Name = "CHK_DailyShop0";
            CHK_DailyShop0.Size = new System.Drawing.Size(15, 14);
            CHK_DailyShop0.TabIndex = 0;
            CHK_DailyShop0.UseVisualStyleBackColor = true;
            // 
            // CHK_DailyShop1
            // 
            CHK_DailyShop1.AutoSize = true;
            CHK_DailyShop1.Location = new System.Drawing.Point(24, 3);
            CHK_DailyShop1.Name = "CHK_DailyShop1";
            CHK_DailyShop1.Size = new System.Drawing.Size(15, 14);
            CHK_DailyShop1.TabIndex = 1;
            CHK_DailyShop1.UseVisualStyleBackColor = true;
            // 
            // CHK_DailyShop2
            // 
            CHK_DailyShop2.AutoSize = true;
            CHK_DailyShop2.Location = new System.Drawing.Point(45, 3);
            CHK_DailyShop2.Name = "CHK_DailyShop2";
            CHK_DailyShop2.Size = new System.Drawing.Size(15, 14);
            CHK_DailyShop2.TabIndex = 2;
            CHK_DailyShop2.UseVisualStyleBackColor = true;
            // 
            // CHK_DailyShop3
            // 
            CHK_DailyShop3.AutoSize = true;
            CHK_DailyShop3.Location = new System.Drawing.Point(66, 3);
            CHK_DailyShop3.Name = "CHK_DailyShop3";
            CHK_DailyShop3.Size = new System.Drawing.Size(15, 14);
            CHK_DailyShop3.TabIndex = 3;
            CHK_DailyShop3.UseVisualStyleBackColor = true;
            // 
            // CHK_DailyShop4
            // 
            CHK_DailyShop4.AutoSize = true;
            CHK_DailyShop4.Location = new System.Drawing.Point(87, 3);
            CHK_DailyShop4.Name = "CHK_DailyShop4";
            CHK_DailyShop4.Size = new System.Drawing.Size(15, 14);
            CHK_DailyShop4.TabIndex = 4;
            CHK_DailyShop4.UseVisualStyleBackColor = true;
            // 
            // CHK_DailyShop5
            // 
            CHK_DailyShop5.AutoSize = true;
            CHK_DailyShop5.Location = new System.Drawing.Point(108, 3);
            CHK_DailyShop5.Name = "CHK_DailyShop5";
            CHK_DailyShop5.Size = new System.Drawing.Size(15, 14);
            CHK_DailyShop5.TabIndex = 5;
            CHK_DailyShop5.UseVisualStyleBackColor = true;
            // 
            // CHK_DailyShop6
            // 
            CHK_DailyShop6.AutoSize = true;
            CHK_DailyShop6.Location = new System.Drawing.Point(129, 3);
            CHK_DailyShop6.Name = "CHK_DailyShop6";
            CHK_DailyShop6.Size = new System.Drawing.Size(15, 14);
            CHK_DailyShop6.TabIndex = 6;
            CHK_DailyShop6.UseVisualStyleBackColor = true;
            // 
            // CHK_DailyShop7
            // 
            CHK_DailyShop7.AutoSize = true;
            CHK_DailyShop7.Location = new System.Drawing.Point(150, 3);
            CHK_DailyShop7.Name = "CHK_DailyShop7";
            CHK_DailyShop7.Size = new System.Drawing.Size(15, 14);
            CHK_DailyShop7.TabIndex = 7;
            CHK_DailyShop7.UseVisualStyleBackColor = true;
            // 
            // CHK_DailyShop8
            // 
            CHK_DailyShop8.AutoSize = true;
            CHK_DailyShop8.Location = new System.Drawing.Point(171, 3);
            CHK_DailyShop8.Name = "CHK_DailyShop8";
            CHK_DailyShop8.Size = new System.Drawing.Size(15, 14);
            CHK_DailyShop8.TabIndex = 8;
            CHK_DailyShop8.UseVisualStyleBackColor = true;
            // 
            // CHK_DailyShop9
            // 
            CHK_DailyShop9.AutoSize = true;
            CHK_DailyShop9.Location = new System.Drawing.Point(192, 3);
            CHK_DailyShop9.Name = "CHK_DailyShop9";
            CHK_DailyShop9.Size = new System.Drawing.Size(15, 14);
            CHK_DailyShop9.TabIndex = 9;
            CHK_DailyShop9.UseVisualStyleBackColor = true;
            // 
            // CHK_DailyShop10
            // 
            CHK_DailyShop10.AutoSize = true;
            CHK_DailyShop10.Location = new System.Drawing.Point(213, 3);
            CHK_DailyShop10.Name = "CHK_DailyShop10";
            CHK_DailyShop10.Size = new System.Drawing.Size(15, 14);
            CHK_DailyShop10.TabIndex = 10;
            CHK_DailyShop10.UseVisualStyleBackColor = true;
            // 
            // CHK_DailyShop11
            // 
            CHK_DailyShop11.AutoSize = true;
            CHK_DailyShop11.Location = new System.Drawing.Point(234, 3);
            CHK_DailyShop11.Name = "CHK_DailyShop11";
            CHK_DailyShop11.Size = new System.Drawing.Size(15, 14);
            CHK_DailyShop11.TabIndex = 11;
            CHK_DailyShop11.UseVisualStyleBackColor = true;
            // 
            // L_DataCards
            // 
            L_DataCards.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            L_DataCards.AutoSize = true;
            L_DataCards.Location = new System.Drawing.Point(8, 56);
            L_DataCards.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            L_DataCards.Name = "L_DataCards";
            L_DataCards.Size = new System.Drawing.Size(76, 17);
            L_DataCards.TabIndex = 4;
            L_DataCards.Text = "Data Cards:";
            // 
            // CLB_DataCards
            // 
            CLB_DataCards.CheckOnClick = true;
            CLB_DataCards.Dock = System.Windows.Forms.DockStyle.Fill;
            CLB_DataCards.FormattingEnabled = true;
            CLB_DataCards.IntegralHeight = false;
            CLB_DataCards.Location = new System.Drawing.Point(87, 54);
            CLB_DataCards.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            CLB_DataCards.Name = "CLB_DataCards";
            CLB_DataCards.Size = new System.Drawing.Size(500, 827);
            CLB_DataCards.TabIndex = 5;
            // 
            // Tab_Medals
            // 
            Tab_Medals.Controls.Add(TLP_Medals);
            Tab_Medals.Location = new System.Drawing.Point(4, 26);
            Tab_Medals.Name = "Tab_Medals";
            Tab_Medals.Padding = new System.Windows.Forms.Padding(3);
            Tab_Medals.Size = new System.Drawing.Size(604, 895);
            Tab_Medals.TabIndex = 1;
            Tab_Medals.Text = "Medals";
            Tab_Medals.UseVisualStyleBackColor = true;
            // 
            // FLP_Medals
            // 
            FLP_Medals.AutoSize = true;
            FLP_Medals.Controls.Add(B_MedalsGiveAll);
            FLP_Medals.Controls.Add(B_MedalsClearAll);
            FLP_Medals.Dock = System.Windows.Forms.DockStyle.Fill;
            FLP_Medals.Location = new System.Drawing.Point(0, 841);
            FLP_Medals.Margin = new System.Windows.Forms.Padding(0);
            FLP_Medals.Name = "FLP_Medals";
            FLP_Medals.Padding = new System.Windows.Forms.Padding(8);
            FLP_Medals.Size = new System.Drawing.Size(598, 48);
            FLP_Medals.TabIndex = 1;
            FLP_Medals.WrapContents = false;
            // 
            // B_MedalsGiveAll
            // 
            B_MedalsGiveAll.AutoSize = true;
            B_MedalsGiveAll.Location = new System.Drawing.Point(11, 11);
            B_MedalsGiveAll.Name = "B_MedalsGiveAll";
            B_MedalsGiveAll.Size = new System.Drawing.Size(61, 27);
            B_MedalsGiveAll.TabIndex = 0;
            B_MedalsGiveAll.Text = "Give All";
            B_MedalsGiveAll.UseVisualStyleBackColor = true;
            B_MedalsGiveAll.Click += B_MedalsGiveAll_Click;
            // 
            // B_MedalsClearAll
            // 
            B_MedalsClearAll.AutoSize = true;
            B_MedalsClearAll.Location = new System.Drawing.Point(78, 11);
            B_MedalsClearAll.Name = "B_MedalsClearAll";
            B_MedalsClearAll.Size = new System.Drawing.Size(66, 27);
            B_MedalsClearAll.TabIndex = 1;
            B_MedalsClearAll.Text = "Clear All";
            B_MedalsClearAll.UseVisualStyleBackColor = true;
            B_MedalsClearAll.Click += B_MedalsClearAll_Click;
            // 
            // DGV_Medals
            // 
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.ControlLight;
            DGV_Medals.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            DGV_Medals.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_Medals.Dock = System.Windows.Forms.DockStyle.Fill;
            DGV_Medals.Location = new System.Drawing.Point(0, 0);
            DGV_Medals.Margin = new System.Windows.Forms.Padding(0);
            DGV_Medals.Name = "DGV_Medals";
            DGV_Medals.RowHeadersVisible = false;
            DGV_Medals.Size = new System.Drawing.Size(598, 841);
            DGV_Medals.TabIndex = 0;
            // 
            // Tab_Counters
            // 
            Tab_Counters.Controls.Add(TLP_Counters);
            Tab_Counters.Location = new System.Drawing.Point(4, 26);
            Tab_Counters.Name = "Tab_Counters";
            Tab_Counters.Padding = new System.Windows.Forms.Padding(3);
            Tab_Counters.Size = new System.Drawing.Size(604, 894);
            Tab_Counters.TabIndex = 2;
            Tab_Counters.Text = "Counters";
            Tab_Counters.UseVisualStyleBackColor = true;
            // 
            // TLP_Counters
            // 
            TLP_Counters.AutoScroll = true;
            TLP_Counters.ColumnCount = 2;
            TLP_Counters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Counters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Counters.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Counters.Location = new System.Drawing.Point(3, 3);
            TLP_Counters.Name = "TLP_Counters";
            TLP_Counters.Padding = new System.Windows.Forms.Padding(8);
            TLP_Counters.RowCount = 1;
            TLP_Counters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Counters.Size = new System.Drawing.Size(598, 888);
            TLP_Counters.TabIndex = 0;
            // 
            // Tab_Best
            // 
            Tab_Best.Controls.Add(TLP_Best);
            Tab_Best.Location = new System.Drawing.Point(4, 26);
            Tab_Best.Name = "Tab_Best";
            Tab_Best.Padding = new System.Windows.Forms.Padding(3);
            Tab_Best.Size = new System.Drawing.Size(604, 895);
            Tab_Best.TabIndex = 3;
            Tab_Best.Text = "Best";
            Tab_Best.UseVisualStyleBackColor = true;
            // 
            // TLP_Best
            // 
            TLP_Best.AutoScroll = true;
            TLP_Best.ColumnCount = 2;
            TLP_Best.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Best.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_Best.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Best.Location = new System.Drawing.Point(3, 3);
            TLP_Best.Name = "TLP_Best";
            TLP_Best.Padding = new System.Windows.Forms.Padding(8);
            TLP_Best.RowCount = 1;
            TLP_Best.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Best.Size = new System.Drawing.Size(598, 889);
            TLP_Best.TabIndex = 0;
            // 
            // Tab_Courses
            // 
            Tab_Courses.Controls.Add(TLP_Courses);
            Tab_Courses.Location = new System.Drawing.Point(4, 26);
            Tab_Courses.Name = "Tab_Courses";
            Tab_Courses.Padding = new System.Windows.Forms.Padding(3);
            Tab_Courses.Size = new System.Drawing.Size(604, 895);
            Tab_Courses.TabIndex = 4;
            Tab_Courses.Text = "Courses";
            Tab_Courses.UseVisualStyleBackColor = true;
            // 
            // TLP_Courses
            // 
            TLP_Courses.AutoScroll = true;
            TLP_Courses.AutoSize = true;
            TLP_Courses.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            TLP_Courses.ColumnCount = 2;
            TLP_Courses.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            TLP_Courses.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_Courses.Controls.Add(L_CourseIndex, 0, 0);
            TLP_Courses.Controls.Add(CB_CourseIndex, 1, 0);
            TLP_Courses.Controls.Add(TLP_CourseScore, 1, 1);
            TLP_Courses.Controls.Add(L_CourseParticipant0, 0, 2);
            TLP_Courses.Controls.Add(UC_CourseParticipant0, 0, 3);
            TLP_Courses.Controls.Add(L_CourseParticipant1, 0, 4);
            TLP_Courses.Controls.Add(UC_CourseParticipant1, 0, 5);
            TLP_Courses.Controls.Add(L_CourseParticipant2, 0, 6);
            TLP_Courses.Controls.Add(UC_CourseParticipant2, 0, 7);
            TLP_Courses.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Courses.Location = new System.Drawing.Point(3, 3);
            TLP_Courses.Name = "TLP_Courses";
            TLP_Courses.Padding = new System.Windows.Forms.Padding(8);
            TLP_Courses.RowCount = 8;
            TLP_Courses.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Courses.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Courses.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Courses.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Courses.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Courses.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Courses.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Courses.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Courses.Size = new System.Drawing.Size(598, 889);
            TLP_Courses.TabIndex = 0;
            // 
            // L_CourseIndex
            // 
            L_CourseIndex.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_CourseIndex.AutoSize = true;
            L_CourseIndex.Location = new System.Drawing.Point(83, 12);
            L_CourseIndex.Name = "L_CourseIndex";
            L_CourseIndex.Size = new System.Drawing.Size(42, 17);
            L_CourseIndex.TabIndex = 0;
            L_CourseIndex.Text = "Index:";
            // 
            // CB_CourseIndex
            // 
            CB_CourseIndex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_CourseIndex.FormattingEnabled = true;
            CB_CourseIndex.Location = new System.Drawing.Point(128, 8);
            CB_CourseIndex.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            CB_CourseIndex.Name = "CB_CourseIndex";
            CB_CourseIndex.Size = new System.Drawing.Size(121, 25);
            CB_CourseIndex.TabIndex = 1;
            CB_CourseIndex.SelectedIndexChanged += CB_CourseIndex_SelectedIndexChanged;
            // 
            // TLP_CourseScore
            // 
            TLP_CourseScore.AutoSize = true;
            TLP_CourseScore.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            TLP_CourseScore.ColumnCount = 4;
            TLP_CourseScore.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_CourseScore.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_CourseScore.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_CourseScore.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_CourseScore.Controls.Add(L_CourseScore0, 0, 0);
            TLP_CourseScore.Controls.Add(NUD_CourseScore0, 1, 0);
            TLP_CourseScore.Controls.Add(L_CourseScore1, 2, 0);
            TLP_CourseScore.Controls.Add(NUD_CourseScore1, 3, 0);
            TLP_CourseScore.Controls.Add(L_CourseScore2, 0, 1);
            TLP_CourseScore.Controls.Add(NUD_CourseScore2, 1, 1);
            TLP_CourseScore.Controls.Add(L_CourseScoreMax, 2, 1);
            TLP_CourseScore.Controls.Add(NUD_CourseScoreMax, 3, 1);
            TLP_CourseScore.Location = new System.Drawing.Point(128, 46);
            TLP_CourseScore.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            TLP_CourseScore.Name = "TLP_CourseScore";
            TLP_CourseScore.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_CourseScore.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_CourseScore.Size = new System.Drawing.Size(320, 52);
            TLP_CourseScore.TabIndex = 2;
            // 
            // L_CourseScore0
            // 
            L_CourseScore0.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_CourseScore0.AutoSize = true;
            L_CourseScore0.Location = new System.Drawing.Point(0, 4);
            L_CourseScore0.Margin = new System.Windows.Forms.Padding(0);
            L_CourseScore0.Name = "L_CourseScore0";
            L_CourseScore0.Size = new System.Drawing.Size(51, 17);
            L_CourseScore0.TabIndex = 0;
            L_CourseScore0.Text = "Score0:";
            // 
            // NUD_CourseScore0
            // 
            NUD_CourseScore0.Location = new System.Drawing.Point(51, 0);
            NUD_CourseScore0.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            NUD_CourseScore0.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NUD_CourseScore0.Name = "NUD_CourseScore0";
            NUD_CourseScore0.Size = new System.Drawing.Size(100, 25);
            NUD_CourseScore0.TabIndex = 1;
            // 
            // L_CourseScore1
            // 
            L_CourseScore1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_CourseScore1.AutoSize = true;
            L_CourseScore1.Location = new System.Drawing.Point(169, 4);
            L_CourseScore1.Margin = new System.Windows.Forms.Padding(0);
            L_CourseScore1.Name = "L_CourseScore1";
            L_CourseScore1.Size = new System.Drawing.Size(51, 17);
            L_CourseScore1.TabIndex = 2;
            L_CourseScore1.Text = "Score1:";
            // 
            // NUD_CourseScore1
            // 
            NUD_CourseScore1.Location = new System.Drawing.Point(220, 0);
            NUD_CourseScore1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            NUD_CourseScore1.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NUD_CourseScore1.Name = "NUD_CourseScore1";
            NUD_CourseScore1.Size = new System.Drawing.Size(100, 25);
            NUD_CourseScore1.TabIndex = 3;
            // 
            // L_CourseScore2
            // 
            L_CourseScore2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_CourseScore2.AutoSize = true;
            L_CourseScore2.Location = new System.Drawing.Point(0, 30);
            L_CourseScore2.Margin = new System.Windows.Forms.Padding(0);
            L_CourseScore2.Name = "L_CourseScore2";
            L_CourseScore2.Size = new System.Drawing.Size(51, 17);
            L_CourseScore2.TabIndex = 4;
            L_CourseScore2.Text = "Score2:";
            // 
            // NUD_CourseScore2
            // 
            NUD_CourseScore2.Location = new System.Drawing.Point(51, 26);
            NUD_CourseScore2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            NUD_CourseScore2.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NUD_CourseScore2.Name = "NUD_CourseScore2";
            NUD_CourseScore2.Size = new System.Drawing.Size(100, 25);
            NUD_CourseScore2.TabIndex = 5;
            // 
            // L_CourseScoreMax
            // 
            L_CourseScoreMax.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_CourseScoreMax.AutoSize = true;
            L_CourseScoreMax.Location = new System.Drawing.Point(151, 30);
            L_CourseScoreMax.Margin = new System.Windows.Forms.Padding(0);
            L_CourseScoreMax.Name = "L_CourseScoreMax";
            L_CourseScoreMax.Size = new System.Drawing.Size(69, 17);
            L_CourseScoreMax.TabIndex = 6;
            L_CourseScoreMax.Text = "ScoreMax:";
            // 
            // NUD_CourseScoreMax
            // 
            NUD_CourseScoreMax.Location = new System.Drawing.Point(220, 26);
            NUD_CourseScoreMax.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            NUD_CourseScoreMax.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NUD_CourseScoreMax.Name = "NUD_CourseScoreMax";
            NUD_CourseScoreMax.Size = new System.Drawing.Size(100, 25);
            NUD_CourseScoreMax.TabIndex = 7;
            // 
            // L_CourseParticipant0
            // 
            L_CourseParticipant0.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            L_CourseParticipant0.AutoSize = true;
            TLP_Courses.SetColumnSpan(L_CourseParticipant0, 2);
            L_CourseParticipant0.Location = new System.Drawing.Point(11, 98);
            L_CourseParticipant0.Name = "L_CourseParticipant0";
            L_CourseParticipant0.Size = new System.Drawing.Size(83, 17);
            L_CourseParticipant0.TabIndex = 3;
            L_CourseParticipant0.Text = "Participant 1:";
            // 
            // UC_CourseParticipant0
            // 
            UC_CourseParticipant0.AutoSize = true;
            TLP_Courses.SetColumnSpan(UC_CourseParticipant0, 2);
            UC_CourseParticipant0.Dock = System.Windows.Forms.DockStyle.Fill;
            UC_CourseParticipant0.Location = new System.Drawing.Point(8, 115);
            UC_CourseParticipant0.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            UC_CourseParticipant0.Name = "UC_CourseParticipant0";
            UC_CourseParticipant0.Size = new System.Drawing.Size(582, 65);
            UC_CourseParticipant0.TabIndex = 4;
            // 
            // L_CourseParticipant1
            // 
            L_CourseParticipant1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            L_CourseParticipant1.AutoSize = true;
            TLP_Courses.SetColumnSpan(L_CourseParticipant1, 2);
            L_CourseParticipant1.Location = new System.Drawing.Point(11, 186);
            L_CourseParticipant1.Name = "L_CourseParticipant1";
            L_CourseParticipant1.Size = new System.Drawing.Size(83, 17);
            L_CourseParticipant1.TabIndex = 5;
            L_CourseParticipant1.Text = "Participant 2:";
            // 
            // UC_CourseParticipant1
            // 
            UC_CourseParticipant1.AutoSize = true;
            TLP_Courses.SetColumnSpan(UC_CourseParticipant1, 2);
            UC_CourseParticipant1.Dock = System.Windows.Forms.DockStyle.Fill;
            UC_CourseParticipant1.Location = new System.Drawing.Point(8, 203);
            UC_CourseParticipant1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            UC_CourseParticipant1.Name = "UC_CourseParticipant1";
            UC_CourseParticipant1.Size = new System.Drawing.Size(582, 65);
            UC_CourseParticipant1.TabIndex = 6;
            // 
            // L_CourseParticipant2
            // 
            L_CourseParticipant2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            L_CourseParticipant2.AutoSize = true;
            TLP_Courses.SetColumnSpan(L_CourseParticipant2, 2);
            L_CourseParticipant2.Location = new System.Drawing.Point(11, 274);
            L_CourseParticipant2.Name = "L_CourseParticipant2";
            L_CourseParticipant2.Size = new System.Drawing.Size(83, 17);
            L_CourseParticipant2.TabIndex = 7;
            L_CourseParticipant2.Text = "Participant 3:";
            // 
            // UC_CourseParticipant2
            // 
            UC_CourseParticipant2.AutoSize = true;
            TLP_Courses.SetColumnSpan(UC_CourseParticipant2, 2);
            UC_CourseParticipant2.Dock = System.Windows.Forms.DockStyle.Fill;
            UC_CourseParticipant2.Location = new System.Drawing.Point(8, 291);
            UC_CourseParticipant2.Margin = new System.Windows.Forms.Padding(0);
            UC_CourseParticipant2.Name = "UC_CourseParticipant2";
            UC_CourseParticipant2.Size = new System.Drawing.Size(582, 590);
            UC_CourseParticipant2.TabIndex = 8;
            // 
            // Tab_SelfEvent
            // 
            Tab_SelfEvent.Controls.Add(TLP_SelfEvent);
            Tab_SelfEvent.Location = new System.Drawing.Point(4, 26);
            Tab_SelfEvent.Name = "Tab_SelfEvent";
            Tab_SelfEvent.Padding = new System.Windows.Forms.Padding(3);
            Tab_SelfEvent.Size = new System.Drawing.Size(604, 895);
            Tab_SelfEvent.TabIndex = 5;
            Tab_SelfEvent.Text = "Self Event";
            Tab_SelfEvent.UseVisualStyleBackColor = true;
            // 
            // TLP_SelfEvent
            // 
            TLP_SelfEvent.AutoScroll = true;
            TLP_SelfEvent.ColumnCount = 2;
            TLP_SelfEvent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_SelfEvent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_SelfEvent.Controls.Add(L_SelfEventIndex, 0, 0);
            TLP_SelfEvent.Controls.Add(CB_SelfEventIndex, 1, 0);
            TLP_SelfEvent.Controls.Add(UC_SelfEventData, 0, 1);
            TLP_SelfEvent.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_SelfEvent.Location = new System.Drawing.Point(3, 3);
            TLP_SelfEvent.Name = "TLP_SelfEvent";
            TLP_SelfEvent.Padding = new System.Windows.Forms.Padding(8);
            TLP_SelfEvent.RowCount = 2;
            TLP_SelfEvent.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_SelfEvent.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_SelfEvent.Size = new System.Drawing.Size(598, 889);
            TLP_SelfEvent.TabIndex = 0;
            // 
            // L_SelfEventIndex
            // 
            L_SelfEventIndex.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_SelfEventIndex.AutoSize = true;
            L_SelfEventIndex.Location = new System.Drawing.Point(8, 12);
            L_SelfEventIndex.Margin = new System.Windows.Forms.Padding(0);
            L_SelfEventIndex.Name = "L_SelfEventIndex";
            L_SelfEventIndex.Size = new System.Drawing.Size(42, 17);
            L_SelfEventIndex.TabIndex = 0;
            L_SelfEventIndex.Text = "Index:";
            // 
            // CB_SelfEventIndex
            // 
            CB_SelfEventIndex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_SelfEventIndex.FormattingEnabled = true;
            CB_SelfEventIndex.Location = new System.Drawing.Point(53, 8);
            CB_SelfEventIndex.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            CB_SelfEventIndex.Name = "CB_SelfEventIndex";
            CB_SelfEventIndex.Size = new System.Drawing.Size(121, 25);
            CB_SelfEventIndex.TabIndex = 1;
            CB_SelfEventIndex.SelectedIndexChanged += CB_SelfEventIndex_SelectedIndexChanged;
            // 
            // UC_SelfEventData
            // 
            UC_SelfEventData.AutoSize = true;
            UC_SelfEventData.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            TLP_SelfEvent.SetColumnSpan(UC_SelfEventData, 2);
            UC_SelfEventData.Dock = System.Windows.Forms.DockStyle.Fill;
            UC_SelfEventData.Location = new System.Drawing.Point(8, 41);
            UC_SelfEventData.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            UC_SelfEventData.Name = "UC_SelfEventData";
            UC_SelfEventData.Size = new System.Drawing.Size(582, 840);
            UC_SelfEventData.TabIndex = 2;
            // 
            // Tab_Connection
            // 
            Tab_Connection.Controls.Add(TLP_Connection);
            Tab_Connection.Location = new System.Drawing.Point(4, 26);
            Tab_Connection.Name = "Tab_Connection";
            Tab_Connection.Padding = new System.Windows.Forms.Padding(3);
            Tab_Connection.Size = new System.Drawing.Size(604, 895);
            Tab_Connection.TabIndex = 6;
            Tab_Connection.Text = "Connection";
            Tab_Connection.UseVisualStyleBackColor = true;
            // 
            // TLP_Connection
            // 
            TLP_Connection.AutoScroll = true;
            TLP_Connection.ColumnCount = 2;
            TLP_Connection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Connection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_Connection.Controls.Add(L_ConnectionIndex, 0, 0);
            TLP_Connection.Controls.Add(CB_ConnectionIndex, 1, 0);
            TLP_Connection.Controls.Add(UC_Connection, 0, 1);
            TLP_Connection.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Connection.Location = new System.Drawing.Point(3, 3);
            TLP_Connection.Name = "TLP_Connection";
            TLP_Connection.Padding = new System.Windows.Forms.Padding(8);
            TLP_Connection.RowCount = 2;
            TLP_Connection.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Connection.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Connection.Size = new System.Drawing.Size(598, 889);
            TLP_Connection.TabIndex = 0;
            // 
            // L_ConnectionIndex
            // 
            L_ConnectionIndex.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_ConnectionIndex.AutoSize = true;
            L_ConnectionIndex.Location = new System.Drawing.Point(8, 12);
            L_ConnectionIndex.Margin = new System.Windows.Forms.Padding(0);
            L_ConnectionIndex.Name = "L_ConnectionIndex";
            L_ConnectionIndex.Size = new System.Drawing.Size(42, 17);
            L_ConnectionIndex.TabIndex = 0;
            L_ConnectionIndex.Text = "Index:";
            // 
            // CB_ConnectionIndex
            // 
            CB_ConnectionIndex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_ConnectionIndex.FormattingEnabled = true;
            CB_ConnectionIndex.Location = new System.Drawing.Point(53, 8);
            CB_ConnectionIndex.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            CB_ConnectionIndex.Name = "CB_ConnectionIndex";
            CB_ConnectionIndex.Size = new System.Drawing.Size(121, 25);
            CB_ConnectionIndex.TabIndex = 1;
            CB_ConnectionIndex.SelectedIndexChanged += CB_ConnectionIndex_SelectedIndexChanged;
            // 
            // UC_Connection
            // 
            UC_Connection.AutoSize = true;
            TLP_Connection.SetColumnSpan(UC_Connection, 2);
            UC_Connection.Dock = System.Windows.Forms.DockStyle.Fill;
            UC_Connection.Location = new System.Drawing.Point(8, 41);
            UC_Connection.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            UC_Connection.Name = "UC_Connection";
            UC_Connection.Size = new System.Drawing.Size(582, 840);
            UC_Connection.TabIndex = 2;
            // 
            // FLP_Buttons
            // 
            FLP_Buttons.AutoSize = true;
            FLP_Buttons.Controls.Add(B_Save);
            FLP_Buttons.Controls.Add(B_Cancel);
            FLP_Buttons.Dock = System.Windows.Forms.DockStyle.Bottom;
            FLP_Buttons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            FLP_Buttons.Location = new System.Drawing.Point(0, 924);
            FLP_Buttons.Name = "FLP_Buttons";
            FLP_Buttons.Padding = new System.Windows.Forms.Padding(8);
            FLP_Buttons.Size = new System.Drawing.Size(612, 49);
            FLP_Buttons.TabIndex = 1;
            // 
            // B_Cancel
            // 
            B_Cancel.AutoSize = true;
            B_Cancel.Location = new System.Drawing.Point(427, 11);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(80, 27);
            B_Cancel.TabIndex = 0;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_Save
            // 
            B_Save.AutoSize = true;
            B_Save.Location = new System.Drawing.Point(513, 11);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(80, 27);
            B_Save.TabIndex = 1;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // TLP_Medals
            // 
            TLP_Medals.ColumnCount = 1;
            TLP_Medals.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_Medals.Controls.Add(DGV_Medals, 0, 0);
            TLP_Medals.Controls.Add(FLP_Medals, 0, 1);
            TLP_Medals.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Medals.Location = new System.Drawing.Point(3, 3);
            TLP_Medals.Name = "TLP_Medals";
            TLP_Medals.RowCount = 2;
            TLP_Medals.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_Medals.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            TLP_Medals.Size = new System.Drawing.Size(598, 889);
            TLP_Medals.TabIndex = 2;
            // 
            // SAV_Pokeathlon4
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(612, 973);
            Controls.Add(TC_Editor);
            Controls.Add(FLP_Buttons);
            Icon = Properties.Resources.Icon;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_Pokeathlon4";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Pokéathlon";
            TC_Editor.ResumeLayout(false);
            Tab_General.ResumeLayout(false);
            TLP_General.ResumeLayout(false);
            TLP_General.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Points).EndInit();
            FLP_DailyShopFlags.ResumeLayout(false);
            FLP_DailyShopFlags.PerformLayout();
            Tab_Medals.ResumeLayout(false);
            FLP_Medals.ResumeLayout(false);
            FLP_Medals.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_Medals).EndInit();
            Tab_Counters.ResumeLayout(false);
            Tab_Best.ResumeLayout(false);
            Tab_Courses.ResumeLayout(false);
            Tab_Courses.PerformLayout();
            TLP_Courses.ResumeLayout(false);
            TLP_Courses.PerformLayout();
            TLP_CourseScore.ResumeLayout(false);
            TLP_CourseScore.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_CourseScore0).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_CourseScore1).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_CourseScore2).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_CourseScoreMax).EndInit();
            Tab_SelfEvent.ResumeLayout(false);
            TLP_SelfEvent.ResumeLayout(false);
            TLP_SelfEvent.PerformLayout();
            Tab_Connection.ResumeLayout(false);
            TLP_Connection.ResumeLayout(false);
            TLP_Connection.PerformLayout();
            FLP_Buttons.ResumeLayout(false);
            FLP_Buttons.PerformLayout();
            TLP_Medals.ResumeLayout(false);
            TLP_Medals.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TabControl TC_Editor;
        private System.Windows.Forms.TabPage Tab_General;
        private System.Windows.Forms.TabPage Tab_Medals;
        private System.Windows.Forms.TabPage Tab_Counters;
        private System.Windows.Forms.TabPage Tab_Best;
        private System.Windows.Forms.TabPage Tab_Courses;
        private System.Windows.Forms.TabPage Tab_SelfEvent;
        private System.Windows.Forms.TabPage Tab_Connection;
        private System.Windows.Forms.FlowLayoutPanel FLP_Buttons;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.TableLayoutPanel TLP_General;
        private System.Windows.Forms.Label L_Points;
        private System.Windows.Forms.NumericUpDown NUD_Points;
        private System.Windows.Forms.Label L_DailyShopFlags;
        private System.Windows.Forms.FlowLayoutPanel FLP_DailyShopFlags;
        private System.Windows.Forms.CheckBox CHK_DailyShop0;
        private System.Windows.Forms.CheckBox CHK_DailyShop1;
        private System.Windows.Forms.CheckBox CHK_DailyShop2;
        private System.Windows.Forms.CheckBox CHK_DailyShop3;
        private System.Windows.Forms.CheckBox CHK_DailyShop4;
        private System.Windows.Forms.CheckBox CHK_DailyShop5;
        private System.Windows.Forms.CheckBox CHK_DailyShop6;
        private System.Windows.Forms.CheckBox CHK_DailyShop7;
        private System.Windows.Forms.CheckBox CHK_DailyShop8;
        private System.Windows.Forms.CheckBox CHK_DailyShop9;
        private System.Windows.Forms.CheckBox CHK_DailyShop10;
        private System.Windows.Forms.CheckBox CHK_DailyShop11;
        private System.Windows.Forms.Label L_DataCards;
        private System.Windows.Forms.CheckedListBox CLB_DataCards;
        private System.Windows.Forms.FlowLayoutPanel FLP_Medals;
        private System.Windows.Forms.Button B_MedalsGiveAll;
        private System.Windows.Forms.Button B_MedalsClearAll;
        private DoubleBufferedDataGridView DGV_Medals;
        private System.Windows.Forms.TableLayoutPanel TLP_Counters;
        private System.Windows.Forms.TableLayoutPanel TLP_Best;
        private System.Windows.Forms.TableLayoutPanel TLP_Courses;
        private System.Windows.Forms.Label L_CourseIndex;
        private System.Windows.Forms.ComboBox CB_CourseIndex;
        private System.Windows.Forms.TableLayoutPanel TLP_CourseScore;
        private System.Windows.Forms.Label L_CourseScore0;
        private System.Windows.Forms.NumericUpDown NUD_CourseScore0;
        private System.Windows.Forms.Label L_CourseScore1;
        private System.Windows.Forms.NumericUpDown NUD_CourseScore1;
        private System.Windows.Forms.Label L_CourseScore2;
        private System.Windows.Forms.NumericUpDown NUD_CourseScore2;
        private System.Windows.Forms.Label L_CourseScoreMax;
        private System.Windows.Forms.NumericUpDown NUD_CourseScoreMax;
        private System.Windows.Forms.Label L_CourseParticipant0;
        private PokeathlonParticipant4Editor UC_CourseParticipant0;
        private System.Windows.Forms.Label L_CourseParticipant1;
        private PokeathlonParticipant4Editor UC_CourseParticipant1;
        private System.Windows.Forms.Label L_CourseParticipant2;
        private PokeathlonParticipant4Editor UC_CourseParticipant2;
        private System.Windows.Forms.TableLayoutPanel TLP_SelfEvent;
        private System.Windows.Forms.Label L_SelfEventIndex;
        private System.Windows.Forms.ComboBox CB_SelfEventIndex;
        private PokeathlonEventData4Editor UC_SelfEventData;
        private System.Windows.Forms.TableLayoutPanel TLP_Connection;
        private System.Windows.Forms.Label L_ConnectionIndex;
        private System.Windows.Forms.ComboBox CB_ConnectionIndex;
        private PokeathlonConnection4Editor UC_Connection;
        private System.Windows.Forms.TableLayoutPanel TLP_Medals;
    }
}
