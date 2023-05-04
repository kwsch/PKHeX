namespace PKHeX.WinForms
{
    partial class SAV_Trainer7GG
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
            B_Cancel = new System.Windows.Forms.Button();
            B_Save = new System.Windows.Forms.Button();
            TB_OTName = new System.Windows.Forms.TextBox();
            L_TrainerName = new System.Windows.Forms.Label();
            MT_Money = new System.Windows.Forms.MaskedTextBox();
            L_Money = new System.Windows.Forms.Label();
            L_Saying5 = new System.Windows.Forms.Label();
            L_Saying4 = new System.Windows.Forms.Label();
            L_Saying3 = new System.Windows.Forms.Label();
            L_Saying2 = new System.Windows.Forms.Label();
            L_Saying1 = new System.Windows.Forms.Label();
            TB_Saying5 = new System.Windows.Forms.TextBox();
            TB_Saying4 = new System.Windows.Forms.TextBox();
            TB_Saying3 = new System.Windows.Forms.TextBox();
            TB_Saying2 = new System.Windows.Forms.TextBox();
            TB_Saying1 = new System.Windows.Forms.TextBox();
            L_Seconds = new System.Windows.Forms.Label();
            L_Minutes = new System.Windows.Forms.Label();
            MT_Seconds = new System.Windows.Forms.MaskedTextBox();
            MT_Minutes = new System.Windows.Forms.MaskedTextBox();
            L_Hours = new System.Windows.Forms.Label();
            MT_Hours = new System.Windows.Forms.MaskedTextBox();
            L_Language = new System.Windows.Forms.Label();
            B_MaxCash = new System.Windows.Forms.Button();
            CB_Language = new System.Windows.Forms.ComboBox();
            CB_Game = new System.Windows.Forms.ComboBox();
            CB_Gender = new System.Windows.Forms.ComboBox();
            TC_Editor = new System.Windows.Forms.TabControl();
            Tab_Overview = new System.Windows.Forms.TabPage();
            B_AllFashionItems = new System.Windows.Forms.Button();
            B_AllTrainerTitles = new System.Windows.Forms.Button();
            TB_RivalName = new System.Windows.Forms.TextBox();
            L_RivalName = new System.Windows.Forms.Label();
            trainerID1 = new Controls.TrainerID();
            GB_Adventure = new System.Windows.Forms.GroupBox();
            Tab_Complex = new System.Windows.Forms.TabPage();
            B_DeleteAll = new System.Windows.Forms.Button();
            B_DeleteGo = new System.Windows.Forms.Button();
            B_ImportGoFiles = new System.Windows.Forms.Button();
            B_ExportGoFiles = new System.Windows.Forms.Button();
            L_GoSlotSummary = new System.Windows.Forms.Label();
            B_Import = new System.Windows.Forms.Button();
            B_Export = new System.Windows.Forms.Button();
            L_GoSlot = new System.Windows.Forms.Label();
            NUD_GoIndex = new System.Windows.Forms.NumericUpDown();
            B_ExportGoSummary = new System.Windows.Forms.Button();
            TC_Editor.SuspendLayout();
            Tab_Overview.SuspendLayout();
            GB_Adventure.SuspendLayout();
            Tab_Complex.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_GoIndex).BeginInit();
            SuspendLayout();
            // 
            // B_Cancel
            // 
            B_Cancel.Location = new System.Drawing.Point(292, 385);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(88, 27);
            B_Cancel.TabIndex = 0;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_Save
            // 
            B_Save.Location = new System.Drawing.Point(386, 385);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(88, 27);
            B_Save.TabIndex = 1;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // TB_OTName
            // 
            TB_OTName.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TB_OTName.Location = new System.Drawing.Point(115, 8);
            TB_OTName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_OTName.MaxLength = 12;
            TB_OTName.Name = "TB_OTName";
            TB_OTName.Size = new System.Drawing.Size(108, 20);
            TB_OTName.TabIndex = 2;
            TB_OTName.Text = "WWWWWWWWWWWW";
            TB_OTName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            TB_OTName.MouseDown += ClickString;
            // 
            // L_TrainerName
            // 
            L_TrainerName.Location = new System.Drawing.Point(8, 10);
            L_TrainerName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_TrainerName.Name = "L_TrainerName";
            L_TrainerName.Size = new System.Drawing.Size(102, 18);
            L_TrainerName.TabIndex = 3;
            L_TrainerName.Text = "Trainer Name:";
            L_TrainerName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MT_Money
            // 
            MT_Money.Location = new System.Drawing.Point(139, 33);
            MT_Money.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MT_Money.Mask = "0000000";
            MT_Money.Name = "MT_Money";
            MT_Money.Size = new System.Drawing.Size(60, 23);
            MT_Money.TabIndex = 4;
            MT_Money.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // L_Money
            // 
            L_Money.AutoSize = true;
            L_Money.Location = new System.Drawing.Point(119, 37);
            L_Money.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Money.Name = "L_Money";
            L_Money.Size = new System.Drawing.Size(16, 15);
            L_Money.TabIndex = 5;
            L_Money.Text = "$:";
            // 
            // L_Saying5
            // 
            L_Saying5.Location = new System.Drawing.Point(0, 0);
            L_Saying5.Name = "L_Saying5";
            L_Saying5.Size = new System.Drawing.Size(100, 23);
            L_Saying5.TabIndex = 0;
            // 
            // L_Saying4
            // 
            L_Saying4.Location = new System.Drawing.Point(0, 0);
            L_Saying4.Name = "L_Saying4";
            L_Saying4.Size = new System.Drawing.Size(100, 23);
            L_Saying4.TabIndex = 0;
            // 
            // L_Saying3
            // 
            L_Saying3.Location = new System.Drawing.Point(0, 0);
            L_Saying3.Name = "L_Saying3";
            L_Saying3.Size = new System.Drawing.Size(100, 23);
            L_Saying3.TabIndex = 0;
            // 
            // L_Saying2
            // 
            L_Saying2.Location = new System.Drawing.Point(0, 0);
            L_Saying2.Name = "L_Saying2";
            L_Saying2.Size = new System.Drawing.Size(100, 23);
            L_Saying2.TabIndex = 0;
            // 
            // L_Saying1
            // 
            L_Saying1.Location = new System.Drawing.Point(0, 0);
            L_Saying1.Name = "L_Saying1";
            L_Saying1.Size = new System.Drawing.Size(100, 23);
            L_Saying1.TabIndex = 0;
            // 
            // TB_Saying5
            // 
            TB_Saying5.Location = new System.Drawing.Point(0, 0);
            TB_Saying5.Name = "TB_Saying5";
            TB_Saying5.Size = new System.Drawing.Size(100, 23);
            TB_Saying5.TabIndex = 0;
            // 
            // TB_Saying4
            // 
            TB_Saying4.Location = new System.Drawing.Point(0, 0);
            TB_Saying4.Name = "TB_Saying4";
            TB_Saying4.Size = new System.Drawing.Size(100, 23);
            TB_Saying4.TabIndex = 0;
            // 
            // TB_Saying3
            // 
            TB_Saying3.Location = new System.Drawing.Point(0, 0);
            TB_Saying3.Name = "TB_Saying3";
            TB_Saying3.Size = new System.Drawing.Size(100, 23);
            TB_Saying3.TabIndex = 0;
            // 
            // TB_Saying2
            // 
            TB_Saying2.Location = new System.Drawing.Point(0, 0);
            TB_Saying2.Name = "TB_Saying2";
            TB_Saying2.Size = new System.Drawing.Size(100, 23);
            TB_Saying2.TabIndex = 0;
            // 
            // TB_Saying1
            // 
            TB_Saying1.Location = new System.Drawing.Point(0, 0);
            TB_Saying1.Name = "TB_Saying1";
            TB_Saying1.Size = new System.Drawing.Size(100, 23);
            TB_Saying1.TabIndex = 0;
            // 
            // L_Seconds
            // 
            L_Seconds.AutoSize = true;
            L_Seconds.Location = new System.Drawing.Point(159, 20);
            L_Seconds.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Seconds.Name = "L_Seconds";
            L_Seconds.Size = new System.Drawing.Size(28, 15);
            L_Seconds.TabIndex = 30;
            L_Seconds.Text = "Sec:";
            // 
            // L_Minutes
            // 
            L_Minutes.AutoSize = true;
            L_Minutes.Location = new System.Drawing.Point(98, 20);
            L_Minutes.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Minutes.Name = "L_Minutes";
            L_Minutes.Size = new System.Drawing.Size(31, 15);
            L_Minutes.TabIndex = 29;
            L_Minutes.Text = "Min:";
            // 
            // MT_Seconds
            // 
            MT_Seconds.Location = new System.Drawing.Point(194, 16);
            MT_Seconds.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MT_Seconds.Mask = "00";
            MT_Seconds.Name = "MT_Seconds";
            MT_Seconds.Size = new System.Drawing.Size(25, 23);
            MT_Seconds.TabIndex = 28;
            MT_Seconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            MT_Seconds.TextChanged += Change255;
            // 
            // MT_Minutes
            // 
            MT_Minutes.Location = new System.Drawing.Point(130, 16);
            MT_Minutes.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MT_Minutes.Mask = "00";
            MT_Minutes.Name = "MT_Minutes";
            MT_Minutes.Size = new System.Drawing.Size(25, 23);
            MT_Minutes.TabIndex = 27;
            MT_Minutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            MT_Minutes.TextChanged += Change255;
            // 
            // L_Hours
            // 
            L_Hours.AutoSize = true;
            L_Hours.Location = new System.Drawing.Point(14, 20);
            L_Hours.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Hours.Name = "L_Hours";
            L_Hours.Size = new System.Drawing.Size(28, 15);
            L_Hours.TabIndex = 26;
            L_Hours.Text = "Hrs:";
            // 
            // MT_Hours
            // 
            MT_Hours.Location = new System.Drawing.Point(51, 16);
            MT_Hours.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MT_Hours.Mask = "00000";
            MT_Hours.Name = "MT_Hours";
            MT_Hours.Size = new System.Drawing.Size(44, 23);
            MT_Hours.TabIndex = 25;
            MT_Hours.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // L_Language
            // 
            L_Language.Location = new System.Drawing.Point(4, 95);
            L_Language.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Language.Name = "L_Language";
            L_Language.Size = new System.Drawing.Size(93, 15);
            L_Language.TabIndex = 21;
            L_Language.Text = "Language:";
            L_Language.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // B_MaxCash
            // 
            B_MaxCash.Location = new System.Drawing.Point(201, 33);
            B_MaxCash.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_MaxCash.Name = "B_MaxCash";
            B_MaxCash.Size = new System.Drawing.Size(23, 23);
            B_MaxCash.TabIndex = 16;
            B_MaxCash.Text = "+";
            B_MaxCash.UseVisualStyleBackColor = true;
            // 
            // CB_Language
            // 
            CB_Language.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Language.FormattingEnabled = true;
            CB_Language.Location = new System.Drawing.Point(115, 90);
            CB_Language.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Language.Name = "CB_Language";
            CB_Language.Size = new System.Drawing.Size(108, 23);
            CB_Language.TabIndex = 15;
            // 
            // CB_Game
            // 
            CB_Game.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Game.FormattingEnabled = true;
            CB_Game.Location = new System.Drawing.Point(115, 59);
            CB_Game.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Game.Name = "CB_Game";
            CB_Game.Size = new System.Drawing.Size(157, 23);
            CB_Game.TabIndex = 24;
            // 
            // CB_Gender
            // 
            CB_Gender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Gender.Enabled = false;
            CB_Gender.FormattingEnabled = true;
            CB_Gender.Items.AddRange(new object[] { "♂", "♀" });
            CB_Gender.Location = new System.Drawing.Point(226, 90);
            CB_Gender.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Gender.Name = "CB_Gender";
            CB_Gender.Size = new System.Drawing.Size(46, 23);
            CB_Gender.TabIndex = 22;
            // 
            // TC_Editor
            // 
            TC_Editor.Controls.Add(Tab_Overview);
            TC_Editor.Controls.Add(Tab_Complex);
            TC_Editor.Location = new System.Drawing.Point(14, 14);
            TC_Editor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TC_Editor.Name = "TC_Editor";
            TC_Editor.SelectedIndex = 0;
            TC_Editor.Size = new System.Drawing.Size(460, 365);
            TC_Editor.TabIndex = 54;
            // 
            // Tab_Overview
            // 
            Tab_Overview.Controls.Add(B_AllFashionItems);
            Tab_Overview.Controls.Add(B_AllTrainerTitles);
            Tab_Overview.Controls.Add(TB_RivalName);
            Tab_Overview.Controls.Add(L_RivalName);
            Tab_Overview.Controls.Add(trainerID1);
            Tab_Overview.Controls.Add(GB_Adventure);
            Tab_Overview.Controls.Add(TB_OTName);
            Tab_Overview.Controls.Add(CB_Gender);
            Tab_Overview.Controls.Add(CB_Game);
            Tab_Overview.Controls.Add(L_TrainerName);
            Tab_Overview.Controls.Add(MT_Money);
            Tab_Overview.Controls.Add(L_Money);
            Tab_Overview.Controls.Add(L_Language);
            Tab_Overview.Controls.Add(CB_Language);
            Tab_Overview.Controls.Add(B_MaxCash);
            Tab_Overview.Location = new System.Drawing.Point(4, 24);
            Tab_Overview.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_Overview.Name = "Tab_Overview";
            Tab_Overview.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_Overview.Size = new System.Drawing.Size(452, 337);
            Tab_Overview.TabIndex = 0;
            Tab_Overview.Text = "Overview";
            Tab_Overview.UseVisualStyleBackColor = true;
            // 
            // B_AllFashionItems
            // 
            B_AllFashionItems.Location = new System.Drawing.Point(290, 173);
            B_AllFashionItems.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_AllFashionItems.Name = "B_AllFashionItems";
            B_AllFashionItems.Size = new System.Drawing.Size(153, 73);
            B_AllFashionItems.TabIndex = 70;
            B_AllFashionItems.Text = "Unlock all Fashion Items";
            B_AllFashionItems.UseVisualStyleBackColor = true;
            B_AllFashionItems.Click += B_AllFashionItems_Click;
            // 
            // B_AllTrainerTitles
            // 
            B_AllTrainerTitles.Location = new System.Drawing.Point(290, 252);
            B_AllTrainerTitles.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_AllTrainerTitles.Name = "B_AllTrainerTitles";
            B_AllTrainerTitles.Size = new System.Drawing.Size(153, 73);
            B_AllTrainerTitles.TabIndex = 69;
            B_AllTrainerTitles.Text = "Unlock all Trainer Titles";
            B_AllTrainerTitles.UseVisualStyleBackColor = true;
            B_AllTrainerTitles.Click += B_AllTrainerTitles_Click;
            // 
            // TB_RivalName
            // 
            TB_RivalName.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TB_RivalName.Location = new System.Drawing.Point(338, 7);
            TB_RivalName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_RivalName.MaxLength = 12;
            TB_RivalName.Name = "TB_RivalName";
            TB_RivalName.Size = new System.Drawing.Size(108, 20);
            TB_RivalName.TabIndex = 67;
            TB_RivalName.Text = "WWWWWWWWWWWW";
            TB_RivalName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            TB_RivalName.MouseDown += ClickString;
            // 
            // L_RivalName
            // 
            L_RivalName.Location = new System.Drawing.Point(231, 9);
            L_RivalName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_RivalName.Name = "L_RivalName";
            L_RivalName.Size = new System.Drawing.Size(102, 18);
            L_RivalName.TabIndex = 68;
            L_RivalName.Text = "Rival Name:";
            L_RivalName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // trainerID1
            // 
            trainerID1.Location = new System.Drawing.Point(7, 30);
            trainerID1.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            trainerID1.Name = "trainerID1";
            trainerID1.Size = new System.Drawing.Size(105, 61);
            trainerID1.TabIndex = 66;
            // 
            // GB_Adventure
            // 
            GB_Adventure.Controls.Add(MT_Seconds);
            GB_Adventure.Controls.Add(MT_Hours);
            GB_Adventure.Controls.Add(L_Seconds);
            GB_Adventure.Controls.Add(L_Hours);
            GB_Adventure.Controls.Add(MT_Minutes);
            GB_Adventure.Controls.Add(L_Minutes);
            GB_Adventure.Location = new System.Drawing.Point(4, 150);
            GB_Adventure.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Adventure.Name = "GB_Adventure";
            GB_Adventure.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Adventure.Size = new System.Drawing.Size(233, 174);
            GB_Adventure.TabIndex = 56;
            GB_Adventure.TabStop = false;
            GB_Adventure.Text = "Adventure Info";
            // 
            // Tab_Complex
            // 
            Tab_Complex.AllowDrop = true;
            Tab_Complex.Controls.Add(B_DeleteAll);
            Tab_Complex.Controls.Add(B_DeleteGo);
            Tab_Complex.Controls.Add(B_ImportGoFiles);
            Tab_Complex.Controls.Add(B_ExportGoFiles);
            Tab_Complex.Controls.Add(L_GoSlotSummary);
            Tab_Complex.Controls.Add(B_Import);
            Tab_Complex.Controls.Add(B_Export);
            Tab_Complex.Controls.Add(L_GoSlot);
            Tab_Complex.Controls.Add(NUD_GoIndex);
            Tab_Complex.Controls.Add(B_ExportGoSummary);
            Tab_Complex.Location = new System.Drawing.Point(4, 24);
            Tab_Complex.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_Complex.Name = "Tab_Complex";
            Tab_Complex.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_Complex.Size = new System.Drawing.Size(452, 337);
            Tab_Complex.TabIndex = 4;
            Tab_Complex.Text = "GO Complex";
            Tab_Complex.UseVisualStyleBackColor = true;
            Tab_Complex.DragDrop += Main_DragDrop;
            Tab_Complex.DragEnter += Main_DragEnter;
            // 
            // B_DeleteAll
            // 
            B_DeleteAll.Location = new System.Drawing.Point(354, 160);
            B_DeleteAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_DeleteAll.Name = "B_DeleteAll";
            B_DeleteAll.Size = new System.Drawing.Size(88, 27);
            B_DeleteAll.TabIndex = 9;
            B_DeleteAll.Text = "Delete All";
            B_DeleteAll.UseVisualStyleBackColor = true;
            B_DeleteAll.Click += B_DeleteAll_Click;
            // 
            // B_DeleteGo
            // 
            B_DeleteGo.Location = new System.Drawing.Point(259, 160);
            B_DeleteGo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_DeleteGo.Name = "B_DeleteGo";
            B_DeleteGo.Size = new System.Drawing.Size(88, 27);
            B_DeleteGo.TabIndex = 8;
            B_DeleteGo.Text = "Delete";
            B_DeleteGo.UseVisualStyleBackColor = true;
            B_DeleteGo.Click += B_DeleteGo_Click;
            // 
            // B_ImportGoFiles
            // 
            B_ImportGoFiles.Location = new System.Drawing.Point(142, 255);
            B_ImportGoFiles.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ImportGoFiles.Name = "B_ImportGoFiles";
            B_ImportGoFiles.Size = new System.Drawing.Size(128, 73);
            B_ImportGoFiles.TabIndex = 7;
            B_ImportGoFiles.Text = "Import from Folder (start at current slot)";
            B_ImportGoFiles.UseVisualStyleBackColor = true;
            B_ImportGoFiles.Click += B_ImportGoFiles_Click;
            // 
            // B_ExportGoFiles
            // 
            B_ExportGoFiles.Location = new System.Drawing.Point(7, 255);
            B_ExportGoFiles.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ExportGoFiles.Name = "B_ExportGoFiles";
            B_ExportGoFiles.Size = new System.Drawing.Size(128, 73);
            B_ExportGoFiles.TabIndex = 6;
            B_ExportGoFiles.Text = "Export all to Folder";
            B_ExportGoFiles.UseVisualStyleBackColor = true;
            B_ExportGoFiles.Click += B_ExportGoFiles_Click;
            // 
            // L_GoSlotSummary
            // 
            L_GoSlotSummary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            L_GoSlotSummary.Location = new System.Drawing.Point(31, 67);
            L_GoSlotSummary.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_GoSlotSummary.Name = "L_GoSlotSummary";
            L_GoSlotSummary.Size = new System.Drawing.Size(220, 120);
            L_GoSlotSummary.TabIndex = 5;
            L_GoSlotSummary.Text = "Summary";
            // 
            // B_Import
            // 
            B_Import.Location = new System.Drawing.Point(259, 100);
            B_Import.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Import.Name = "B_Import";
            B_Import.Size = new System.Drawing.Size(88, 27);
            B_Import.TabIndex = 4;
            B_Import.Text = "Import";
            B_Import.UseVisualStyleBackColor = true;
            B_Import.Click += B_Import_Click;
            // 
            // B_Export
            // 
            B_Export.Location = new System.Drawing.Point(259, 67);
            B_Export.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Export.Name = "B_Export";
            B_Export.Size = new System.Drawing.Size(88, 27);
            B_Export.TabIndex = 3;
            B_Export.Text = "Export";
            B_Export.UseVisualStyleBackColor = true;
            B_Export.Click += B_Export_Click;
            // 
            // L_GoSlot
            // 
            L_GoSlot.Location = new System.Drawing.Point(6, 40);
            L_GoSlot.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_GoSlot.Name = "L_GoSlot";
            L_GoSlot.Size = new System.Drawing.Size(99, 23);
            L_GoSlot.TabIndex = 2;
            L_GoSlot.Text = "Slot:";
            L_GoSlot.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_GoIndex
            // 
            NUD_GoIndex.Location = new System.Drawing.Point(112, 40);
            NUD_GoIndex.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_GoIndex.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            NUD_GoIndex.Name = "NUD_GoIndex";
            NUD_GoIndex.Size = new System.Drawing.Size(140, 23);
            NUD_GoIndex.TabIndex = 1;
            NUD_GoIndex.ValueChanged += NUD_GoIndex_ValueChanged;
            // 
            // B_ExportGoSummary
            // 
            B_ExportGoSummary.Location = new System.Drawing.Point(278, 255);
            B_ExportGoSummary.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ExportGoSummary.Name = "B_ExportGoSummary";
            B_ExportGoSummary.Size = new System.Drawing.Size(153, 73);
            B_ExportGoSummary.TabIndex = 0;
            B_ExportGoSummary.Text = "Dump Text Summary of Go Park Entities";
            B_ExportGoSummary.UseVisualStyleBackColor = true;
            B_ExportGoSummary.Click += B_ExportGoSummary_Click;
            // 
            // SAV_Trainer7GG
            // 
            AllowDrop = true;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(483, 422);
            Controls.Add(TC_Editor);
            Controls.Add(B_Save);
            Controls.Add(B_Cancel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_Trainer7GG";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Trainer Data Editor";
            DragDrop += Main_DragDrop;
            DragEnter += Main_DragEnter;
            TC_Editor.ResumeLayout(false);
            Tab_Overview.ResumeLayout(false);
            Tab_Overview.PerformLayout();
            GB_Adventure.ResumeLayout(false);
            GB_Adventure.PerformLayout();
            Tab_Complex.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)NUD_GoIndex).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.TextBox TB_OTName;
        private System.Windows.Forms.Label L_TrainerName;
        private System.Windows.Forms.MaskedTextBox MT_Money;
        private System.Windows.Forms.Label L_Money;
        private System.Windows.Forms.Label L_Saying5;
        private System.Windows.Forms.Label L_Saying4;
        private System.Windows.Forms.Label L_Saying3;
        private System.Windows.Forms.Label L_Saying2;
        private System.Windows.Forms.Label L_Saying1;
        private System.Windows.Forms.TextBox TB_Saying5;
        private System.Windows.Forms.TextBox TB_Saying4;
        private System.Windows.Forms.TextBox TB_Saying3;
        private System.Windows.Forms.TextBox TB_Saying2;
        private System.Windows.Forms.TextBox TB_Saying1;
        private System.Windows.Forms.ComboBox CB_Language;
        private System.Windows.Forms.Button B_MaxCash;
        private System.Windows.Forms.Label L_Language;
        private System.Windows.Forms.ComboBox CB_Game;
        private System.Windows.Forms.ComboBox CB_Gender;
        private System.Windows.Forms.Label L_Seconds;
        private System.Windows.Forms.Label L_Minutes;
        private System.Windows.Forms.MaskedTextBox MT_Seconds;
        private System.Windows.Forms.MaskedTextBox MT_Minutes;
        private System.Windows.Forms.Label L_Hours;
        private System.Windows.Forms.MaskedTextBox MT_Hours;
        private System.Windows.Forms.TabControl TC_Editor;
        private System.Windows.Forms.TabPage Tab_Overview;
        private System.Windows.Forms.GroupBox GB_Adventure;
        private System.Windows.Forms.TabPage Tab_Complex;
        private Controls.TrainerID trainerID1;
        private System.Windows.Forms.TextBox TB_RivalName;
        private System.Windows.Forms.Label L_RivalName;
        private System.Windows.Forms.Button B_ExportGoSummary;
        private System.Windows.Forms.NumericUpDown NUD_GoIndex;
        private System.Windows.Forms.Label L_GoSlot;
        private System.Windows.Forms.Button B_Import;
        private System.Windows.Forms.Button B_Export;
        private System.Windows.Forms.Label L_GoSlotSummary;
        private System.Windows.Forms.Button B_ExportGoFiles;
        private System.Windows.Forms.Button B_ImportGoFiles;
        private System.Windows.Forms.Button B_DeleteAll;
        private System.Windows.Forms.Button B_DeleteGo;
        private System.Windows.Forms.Button B_AllTrainerTitles;
        private System.Windows.Forms.Button B_AllFashionItems;
    }
}
