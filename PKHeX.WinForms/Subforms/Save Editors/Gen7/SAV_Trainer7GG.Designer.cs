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
            this.B_Cancel = new System.Windows.Forms.Button();
            this.B_Save = new System.Windows.Forms.Button();
            this.TB_OTName = new System.Windows.Forms.TextBox();
            this.L_TrainerName = new System.Windows.Forms.Label();
            this.MT_Money = new System.Windows.Forms.MaskedTextBox();
            this.L_Money = new System.Windows.Forms.Label();
            this.L_Saying5 = new System.Windows.Forms.Label();
            this.L_Saying4 = new System.Windows.Forms.Label();
            this.L_Saying3 = new System.Windows.Forms.Label();
            this.L_Saying2 = new System.Windows.Forms.Label();
            this.L_Saying1 = new System.Windows.Forms.Label();
            this.TB_Saying5 = new System.Windows.Forms.TextBox();
            this.TB_Saying4 = new System.Windows.Forms.TextBox();
            this.TB_Saying3 = new System.Windows.Forms.TextBox();
            this.TB_Saying2 = new System.Windows.Forms.TextBox();
            this.TB_Saying1 = new System.Windows.Forms.TextBox();
            this.L_Seconds = new System.Windows.Forms.Label();
            this.L_Minutes = new System.Windows.Forms.Label();
            this.MT_Seconds = new System.Windows.Forms.MaskedTextBox();
            this.MT_Minutes = new System.Windows.Forms.MaskedTextBox();
            this.L_Hours = new System.Windows.Forms.Label();
            this.MT_Hours = new System.Windows.Forms.MaskedTextBox();
            this.L_Language = new System.Windows.Forms.Label();
            this.B_MaxCash = new System.Windows.Forms.Button();
            this.CB_Language = new System.Windows.Forms.ComboBox();
            this.CB_Game = new System.Windows.Forms.ComboBox();
            this.CB_Gender = new System.Windows.Forms.ComboBox();
            this.TC_Editor = new System.Windows.Forms.TabControl();
            this.Tab_Overview = new System.Windows.Forms.TabPage();
            this.TB_RivalName = new System.Windows.Forms.TextBox();
            this.L_RivalName = new System.Windows.Forms.Label();
            this.trainerID1 = new PKHeX.WinForms.Controls.TrainerID();
            this.GB_Adventure = new System.Windows.Forms.GroupBox();
            this.Tab_Complex = new System.Windows.Forms.TabPage();
            this.B_DeleteAll = new System.Windows.Forms.Button();
            this.B_DeleteGo = new System.Windows.Forms.Button();
            this.B_ImportGoFiles = new System.Windows.Forms.Button();
            this.B_ExportGoFiles = new System.Windows.Forms.Button();
            this.L_GoSlotSummary = new System.Windows.Forms.Label();
            this.B_Import = new System.Windows.Forms.Button();
            this.B_Export = new System.Windows.Forms.Button();
            this.L_GoSlot = new System.Windows.Forms.Label();
            this.NUD_GoIndex = new System.Windows.Forms.NumericUpDown();
            this.B_ExportGoSummary = new System.Windows.Forms.Button();
            this.TC_Editor.SuspendLayout();
            this.Tab_Overview.SuspendLayout();
            this.GB_Adventure.SuspendLayout();
            this.Tab_Complex.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_GoIndex)).BeginInit();
            this.SuspendLayout();
            //
            // B_Cancel
            //
            this.B_Cancel.Location = new System.Drawing.Point(250, 334);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(75, 23);
            this.B_Cancel.TabIndex = 0;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            //
            // B_Save
            //
            this.B_Save.Location = new System.Drawing.Point(331, 334);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(75, 23);
            this.B_Save.TabIndex = 1;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            //
            // TB_OTName
            //
            this.TB_OTName.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_OTName.Location = new System.Drawing.Point(99, 7);
            this.TB_OTName.MaxLength = 12;
            this.TB_OTName.Name = "TB_OTName";
            this.TB_OTName.Size = new System.Drawing.Size(93, 20);
            this.TB_OTName.TabIndex = 2;
            this.TB_OTName.Text = "WWWWWWWWWWWW";
            this.TB_OTName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_OTName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ClickString);
            //
            // L_TrainerName
            //
            this.L_TrainerName.Location = new System.Drawing.Point(7, 9);
            this.L_TrainerName.Name = "L_TrainerName";
            this.L_TrainerName.Size = new System.Drawing.Size(87, 16);
            this.L_TrainerName.TabIndex = 3;
            this.L_TrainerName.Text = "Trainer Name:";
            this.L_TrainerName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // MT_Money
            //
            this.MT_Money.Location = new System.Drawing.Point(119, 29);
            this.MT_Money.Mask = "0000000";
            this.MT_Money.Name = "MT_Money";
            this.MT_Money.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.MT_Money.Size = new System.Drawing.Size(52, 20);
            this.MT_Money.TabIndex = 4;
            this.MT_Money.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // L_Money
            //
            this.L_Money.AutoSize = true;
            this.L_Money.Location = new System.Drawing.Point(102, 32);
            this.L_Money.Name = "L_Money";
            this.L_Money.Size = new System.Drawing.Size(16, 13);
            this.L_Money.TabIndex = 5;
            this.L_Money.Text = "$:";
            //
            // L_Saying5
            //
            this.L_Saying5.Location = new System.Drawing.Point(0, 0);
            this.L_Saying5.Name = "L_Saying5";
            this.L_Saying5.Size = new System.Drawing.Size(100, 23);
            this.L_Saying5.TabIndex = 0;
            //
            // L_Saying4
            //
            this.L_Saying4.Location = new System.Drawing.Point(0, 0);
            this.L_Saying4.Name = "L_Saying4";
            this.L_Saying4.Size = new System.Drawing.Size(100, 23);
            this.L_Saying4.TabIndex = 0;
            //
            // L_Saying3
            //
            this.L_Saying3.Location = new System.Drawing.Point(0, 0);
            this.L_Saying3.Name = "L_Saying3";
            this.L_Saying3.Size = new System.Drawing.Size(100, 23);
            this.L_Saying3.TabIndex = 0;
            //
            // L_Saying2
            //
            this.L_Saying2.Location = new System.Drawing.Point(0, 0);
            this.L_Saying2.Name = "L_Saying2";
            this.L_Saying2.Size = new System.Drawing.Size(100, 23);
            this.L_Saying2.TabIndex = 0;
            //
            // L_Saying1
            //
            this.L_Saying1.Location = new System.Drawing.Point(0, 0);
            this.L_Saying1.Name = "L_Saying1";
            this.L_Saying1.Size = new System.Drawing.Size(100, 23);
            this.L_Saying1.TabIndex = 0;
            //
            // TB_Saying5
            //
            this.TB_Saying5.Location = new System.Drawing.Point(0, 0);
            this.TB_Saying5.Name = "TB_Saying5";
            this.TB_Saying5.Size = new System.Drawing.Size(100, 20);
            this.TB_Saying5.TabIndex = 0;
            //
            // TB_Saying4
            //
            this.TB_Saying4.Location = new System.Drawing.Point(0, 0);
            this.TB_Saying4.Name = "TB_Saying4";
            this.TB_Saying4.Size = new System.Drawing.Size(100, 20);
            this.TB_Saying4.TabIndex = 0;
            //
            // TB_Saying3
            //
            this.TB_Saying3.Location = new System.Drawing.Point(0, 0);
            this.TB_Saying3.Name = "TB_Saying3";
            this.TB_Saying3.Size = new System.Drawing.Size(100, 20);
            this.TB_Saying3.TabIndex = 0;
            //
            // TB_Saying2
            //
            this.TB_Saying2.Location = new System.Drawing.Point(0, 0);
            this.TB_Saying2.Name = "TB_Saying2";
            this.TB_Saying2.Size = new System.Drawing.Size(100, 20);
            this.TB_Saying2.TabIndex = 0;
            //
            // TB_Saying1
            //
            this.TB_Saying1.Location = new System.Drawing.Point(0, 0);
            this.TB_Saying1.Name = "TB_Saying1";
            this.TB_Saying1.Size = new System.Drawing.Size(100, 20);
            this.TB_Saying1.TabIndex = 0;
            //
            // L_Seconds
            //
            this.L_Seconds.AutoSize = true;
            this.L_Seconds.Location = new System.Drawing.Point(136, 17);
            this.L_Seconds.Name = "L_Seconds";
            this.L_Seconds.Size = new System.Drawing.Size(29, 13);
            this.L_Seconds.TabIndex = 30;
            this.L_Seconds.Text = "Sec:";
            //
            // L_Minutes
            //
            this.L_Minutes.AutoSize = true;
            this.L_Minutes.Location = new System.Drawing.Point(84, 17);
            this.L_Minutes.Name = "L_Minutes";
            this.L_Minutes.Size = new System.Drawing.Size(27, 13);
            this.L_Minutes.TabIndex = 29;
            this.L_Minutes.Text = "Min:";
            //
            // MT_Seconds
            //
            this.MT_Seconds.Location = new System.Drawing.Point(166, 14);
            this.MT_Seconds.Mask = "00";
            this.MT_Seconds.Name = "MT_Seconds";
            this.MT_Seconds.Size = new System.Drawing.Size(22, 20);
            this.MT_Seconds.TabIndex = 28;
            this.MT_Seconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.MT_Seconds.TextChanged += new System.EventHandler(this.Change255);
            //
            // MT_Minutes
            //
            this.MT_Minutes.Location = new System.Drawing.Point(111, 14);
            this.MT_Minutes.Mask = "00";
            this.MT_Minutes.Name = "MT_Minutes";
            this.MT_Minutes.Size = new System.Drawing.Size(22, 20);
            this.MT_Minutes.TabIndex = 27;
            this.MT_Minutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.MT_Minutes.TextChanged += new System.EventHandler(this.Change255);
            //
            // L_Hours
            //
            this.L_Hours.AutoSize = true;
            this.L_Hours.Location = new System.Drawing.Point(12, 17);
            this.L_Hours.Name = "L_Hours";
            this.L_Hours.Size = new System.Drawing.Size(26, 13);
            this.L_Hours.TabIndex = 26;
            this.L_Hours.Text = "Hrs:";
            //
            // MT_Hours
            //
            this.MT_Hours.Location = new System.Drawing.Point(44, 14);
            this.MT_Hours.Mask = "00000";
            this.MT_Hours.Name = "MT_Hours";
            this.MT_Hours.Size = new System.Drawing.Size(38, 20);
            this.MT_Hours.TabIndex = 25;
            this.MT_Hours.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // L_Language
            //
            this.L_Language.Location = new System.Drawing.Point(3, 82);
            this.L_Language.Name = "L_Language";
            this.L_Language.Size = new System.Drawing.Size(80, 13);
            this.L_Language.TabIndex = 21;
            this.L_Language.Text = "Language:";
            this.L_Language.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // B_MaxCash
            //
            this.B_MaxCash.Location = new System.Drawing.Point(172, 29);
            this.B_MaxCash.Name = "B_MaxCash";
            this.B_MaxCash.Size = new System.Drawing.Size(20, 20);
            this.B_MaxCash.TabIndex = 16;
            this.B_MaxCash.Text = "+";
            this.B_MaxCash.UseVisualStyleBackColor = true;
            //
            // CB_Language
            //
            this.CB_Language.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Language.FormattingEnabled = true;
            this.CB_Language.Location = new System.Drawing.Point(99, 78);
            this.CB_Language.Name = "CB_Language";
            this.CB_Language.Size = new System.Drawing.Size(93, 21);
            this.CB_Language.TabIndex = 15;
            //
            // CB_Game
            //
            this.CB_Game.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Game.FormattingEnabled = true;
            this.CB_Game.Location = new System.Drawing.Point(99, 51);
            this.CB_Game.Name = "CB_Game";
            this.CB_Game.Size = new System.Drawing.Size(135, 21);
            this.CB_Game.TabIndex = 24;
            //
            // CB_Gender
            //
            this.CB_Gender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Gender.Enabled = false;
            this.CB_Gender.FormattingEnabled = true;
            this.CB_Gender.Items.AddRange(new object[] {
            "♂",
            "♀"});
            this.CB_Gender.Location = new System.Drawing.Point(194, 78);
            this.CB_Gender.Name = "CB_Gender";
            this.CB_Gender.Size = new System.Drawing.Size(40, 21);
            this.CB_Gender.TabIndex = 22;
            //
            // TC_Editor
            //
            this.TC_Editor.Controls.Add(this.Tab_Overview);
            this.TC_Editor.Controls.Add(this.Tab_Complex);
            this.TC_Editor.Location = new System.Drawing.Point(12, 12);
            this.TC_Editor.Name = "TC_Editor";
            this.TC_Editor.SelectedIndex = 0;
            this.TC_Editor.Size = new System.Drawing.Size(394, 316);
            this.TC_Editor.TabIndex = 54;
            //
            // Tab_Overview
            //
            this.Tab_Overview.Controls.Add(this.TB_RivalName);
            this.Tab_Overview.Controls.Add(this.L_RivalName);
            this.Tab_Overview.Controls.Add(this.trainerID1);
            this.Tab_Overview.Controls.Add(this.GB_Adventure);
            this.Tab_Overview.Controls.Add(this.TB_OTName);
            this.Tab_Overview.Controls.Add(this.CB_Gender);
            this.Tab_Overview.Controls.Add(this.CB_Game);
            this.Tab_Overview.Controls.Add(this.L_TrainerName);
            this.Tab_Overview.Controls.Add(this.MT_Money);
            this.Tab_Overview.Controls.Add(this.L_Money);
            this.Tab_Overview.Controls.Add(this.L_Language);
            this.Tab_Overview.Controls.Add(this.CB_Language);
            this.Tab_Overview.Controls.Add(this.B_MaxCash);
            this.Tab_Overview.Location = new System.Drawing.Point(4, 22);
            this.Tab_Overview.Name = "Tab_Overview";
            this.Tab_Overview.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_Overview.Size = new System.Drawing.Size(386, 290);
            this.Tab_Overview.TabIndex = 0;
            this.Tab_Overview.Text = "Overview";
            this.Tab_Overview.UseVisualStyleBackColor = true;
            //
            // TB_RivalName
            //
            this.TB_RivalName.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_RivalName.Location = new System.Drawing.Point(290, 6);
            this.TB_RivalName.MaxLength = 12;
            this.TB_RivalName.Name = "TB_RivalName";
            this.TB_RivalName.Size = new System.Drawing.Size(93, 20);
            this.TB_RivalName.TabIndex = 67;
            this.TB_RivalName.Text = "WWWWWWWWWWWW";
            this.TB_RivalName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_RivalName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ClickString);
            //
            // L_RivalName
            //
            this.L_RivalName.Location = new System.Drawing.Point(198, 8);
            this.L_RivalName.Name = "L_RivalName";
            this.L_RivalName.Size = new System.Drawing.Size(87, 16);
            this.L_RivalName.TabIndex = 68;
            this.L_RivalName.Text = "Rival Name:";
            this.L_RivalName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // trainerID1
            //
            this.trainerID1.Location = new System.Drawing.Point(6, 26);
            this.trainerID1.Name = "trainerID1";
            this.trainerID1.Size = new System.Drawing.Size(90, 53);
            this.trainerID1.TabIndex = 66;
            //
            // GB_Adventure
            //
            this.GB_Adventure.Controls.Add(this.MT_Seconds);
            this.GB_Adventure.Controls.Add(this.MT_Hours);
            this.GB_Adventure.Controls.Add(this.L_Seconds);
            this.GB_Adventure.Controls.Add(this.L_Hours);
            this.GB_Adventure.Controls.Add(this.MT_Minutes);
            this.GB_Adventure.Controls.Add(this.L_Minutes);
            this.GB_Adventure.Location = new System.Drawing.Point(3, 130);
            this.GB_Adventure.Name = "GB_Adventure";
            this.GB_Adventure.Size = new System.Drawing.Size(200, 151);
            this.GB_Adventure.TabIndex = 56;
            this.GB_Adventure.TabStop = false;
            this.GB_Adventure.Text = "Adventure Info";
            //
            // Tab_Complex
            //
            this.Tab_Complex.AllowDrop = true;
            this.Tab_Complex.Controls.Add(this.B_DeleteAll);
            this.Tab_Complex.Controls.Add(this.B_DeleteGo);
            this.Tab_Complex.Controls.Add(this.B_ImportGoFiles);
            this.Tab_Complex.Controls.Add(this.B_ExportGoFiles);
            this.Tab_Complex.Controls.Add(this.L_GoSlotSummary);
            this.Tab_Complex.Controls.Add(this.B_Import);
            this.Tab_Complex.Controls.Add(this.B_Export);
            this.Tab_Complex.Controls.Add(this.L_GoSlot);
            this.Tab_Complex.Controls.Add(this.NUD_GoIndex);
            this.Tab_Complex.Controls.Add(this.B_ExportGoSummary);
            this.Tab_Complex.Location = new System.Drawing.Point(4, 22);
            this.Tab_Complex.Name = "Tab_Complex";
            this.Tab_Complex.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_Complex.Size = new System.Drawing.Size(386, 290);
            this.Tab_Complex.TabIndex = 4;
            this.Tab_Complex.Text = "Go Complex";
            this.Tab_Complex.UseVisualStyleBackColor = true;
            this.Tab_Complex.DragDrop += new System.Windows.Forms.DragEventHandler(this.Main_DragDrop);
            this.Tab_Complex.DragEnter += new System.Windows.Forms.DragEventHandler(this.Main_DragEnter);
            //
            // B_DeleteAll
            //
            this.B_DeleteAll.Location = new System.Drawing.Point(303, 139);
            this.B_DeleteAll.Name = "B_DeleteAll";
            this.B_DeleteAll.Size = new System.Drawing.Size(75, 23);
            this.B_DeleteAll.TabIndex = 9;
            this.B_DeleteAll.Text = "Delete All";
            this.B_DeleteAll.UseVisualStyleBackColor = true;
            this.B_DeleteAll.Click += new System.EventHandler(this.B_DeleteAll_Click);
            //
            // B_DeleteGo
            //
            this.B_DeleteGo.Location = new System.Drawing.Point(222, 139);
            this.B_DeleteGo.Name = "B_DeleteGo";
            this.B_DeleteGo.Size = new System.Drawing.Size(75, 23);
            this.B_DeleteGo.TabIndex = 8;
            this.B_DeleteGo.Text = "Delete";
            this.B_DeleteGo.UseVisualStyleBackColor = true;
            this.B_DeleteGo.Click += new System.EventHandler(this.B_DeleteGo_Click);
            //
            // B_ImportGoFiles
            //
            this.B_ImportGoFiles.Location = new System.Drawing.Point(122, 221);
            this.B_ImportGoFiles.Name = "B_ImportGoFiles";
            this.B_ImportGoFiles.Size = new System.Drawing.Size(110, 63);
            this.B_ImportGoFiles.TabIndex = 7;
            this.B_ImportGoFiles.Text = "Import from Folder (start at current slot)";
            this.B_ImportGoFiles.UseVisualStyleBackColor = true;
            this.B_ImportGoFiles.Click += new System.EventHandler(this.B_ImportGoFiles_Click);
            //
            // B_ExportGoFiles
            //
            this.B_ExportGoFiles.Location = new System.Drawing.Point(6, 221);
            this.B_ExportGoFiles.Name = "B_ExportGoFiles";
            this.B_ExportGoFiles.Size = new System.Drawing.Size(110, 63);
            this.B_ExportGoFiles.TabIndex = 6;
            this.B_ExportGoFiles.Text = "Export all to Folder";
            this.B_ExportGoFiles.UseVisualStyleBackColor = true;
            this.B_ExportGoFiles.Click += new System.EventHandler(this.B_ExportGoFiles_Click);
            //
            // L_GoSlotSummary
            //
            this.L_GoSlotSummary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.L_GoSlotSummary.Location = new System.Drawing.Point(27, 58);
            this.L_GoSlotSummary.Name = "L_GoSlotSummary";
            this.L_GoSlotSummary.Size = new System.Drawing.Size(189, 104);
            this.L_GoSlotSummary.TabIndex = 5;
            this.L_GoSlotSummary.Text = "Summary";
            //
            // B_Import
            //
            this.B_Import.Location = new System.Drawing.Point(222, 87);
            this.B_Import.Name = "B_Import";
            this.B_Import.Size = new System.Drawing.Size(75, 23);
            this.B_Import.TabIndex = 4;
            this.B_Import.Text = "Import";
            this.B_Import.UseVisualStyleBackColor = true;
            this.B_Import.Click += new System.EventHandler(this.B_Import_Click);
            //
            // B_Export
            //
            this.B_Export.Location = new System.Drawing.Point(222, 58);
            this.B_Export.Name = "B_Export";
            this.B_Export.Size = new System.Drawing.Size(75, 23);
            this.B_Export.TabIndex = 3;
            this.B_Export.Text = "Export";
            this.B_Export.UseVisualStyleBackColor = true;
            this.B_Export.Click += new System.EventHandler(this.B_Export_Click);
            //
            // L_GoSlot
            //
            this.L_GoSlot.Location = new System.Drawing.Point(5, 35);
            this.L_GoSlot.Name = "L_GoSlot";
            this.L_GoSlot.Size = new System.Drawing.Size(85, 20);
            this.L_GoSlot.TabIndex = 2;
            this.L_GoSlot.Text = "Slot:";
            this.L_GoSlot.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // NUD_GoIndex
            //
            this.NUD_GoIndex.Location = new System.Drawing.Point(96, 35);
            this.NUD_GoIndex.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.NUD_GoIndex.Name = "NUD_GoIndex";
            this.NUD_GoIndex.Size = new System.Drawing.Size(120, 20);
            this.NUD_GoIndex.TabIndex = 1;
            this.NUD_GoIndex.ValueChanged += new System.EventHandler(this.NUD_GoIndex_ValueChanged);
            //
            // B_ExportGoSummary
            //
            this.B_ExportGoSummary.Location = new System.Drawing.Point(238, 221);
            this.B_ExportGoSummary.Name = "B_ExportGoSummary";
            this.B_ExportGoSummary.Size = new System.Drawing.Size(131, 63);
            this.B_ExportGoSummary.TabIndex = 0;
            this.B_ExportGoSummary.Text = "Dump Text Summary of Go Park Entities";
            this.B_ExportGoSummary.UseVisualStyleBackColor = true;
            this.B_ExportGoSummary.Click += new System.EventHandler(this.B_ExportGoSummary_Click);
            //
            // SAV_Trainer7GG
            //
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 366);
            this.Controls.Add(this.TC_Editor);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_Cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SAV_Trainer7GG";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Trainer Data Editor";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Main_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Main_DragEnter);
            this.TC_Editor.ResumeLayout(false);
            this.Tab_Overview.ResumeLayout(false);
            this.Tab_Overview.PerformLayout();
            this.GB_Adventure.ResumeLayout(false);
            this.GB_Adventure.PerformLayout();
            this.Tab_Complex.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NUD_GoIndex)).EndInit();
            this.ResumeLayout(false);

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
    }
}