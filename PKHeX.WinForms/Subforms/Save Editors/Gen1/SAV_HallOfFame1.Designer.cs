namespace PKHeX.WinForms
{
    partial class SAV_HallOfFame1
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
            LB_DataEntry = new System.Windows.Forms.ListBox();
            RTB_Team = new System.Windows.Forms.RichTextBox();
            B_Close = new System.Windows.Forms.Button();
            PB_Sprite = new System.Windows.Forms.PictureBox();
            NUP_PartyIndex = new System.Windows.Forms.NumericUpDown();
            L_PartyNum = new System.Windows.Forms.Label();
            CB_Species = new System.Windows.Forms.ComboBox();
            Label_Species = new System.Windows.Forms.Label();
            CHK_Nicknamed = new System.Windows.Forms.CheckBox();
            TB_Nickname = new System.Windows.Forms.TextBox();
            B_Cancel = new System.Windows.Forms.Button();
            L_Level = new System.Windows.Forms.Label();
            B_Delete = new System.Windows.Forms.Button();
            GB_Entry = new System.Windows.Forms.GroupBox();
            B_ClearSlot = new System.Windows.Forms.Button();
            NUD_Level = new System.Windows.Forms.NumericUpDown();
            B_SetParty = new System.Windows.Forms.Button();
            NUD_Clears = new System.Windows.Forms.NumericUpDown();
            L_Clears = new System.Windows.Forms.Label();
            B_ClearAll = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)PB_Sprite).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUP_PartyIndex).BeginInit();
            GB_Entry.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Level).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Clears).BeginInit();
            SuspendLayout();
            // 
            // LB_DataEntry
            // 
            LB_DataEntry.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            LB_DataEntry.FormattingEnabled = true;
            LB_DataEntry.Location = new System.Drawing.Point(8, 14);
            LB_DataEntry.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LB_DataEntry.Name = "LB_DataEntry";
            LB_DataEntry.Size = new System.Drawing.Size(120, 319);
            LB_DataEntry.TabIndex = 0;
            LB_DataEntry.SelectedIndexChanged += DisplayEntry;
            // 
            // RTB_Team
            // 
            RTB_Team.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            RTB_Team.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            RTB_Team.Location = new System.Drawing.Point(136, 14);
            RTB_Team.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RTB_Team.Name = "RTB_Team";
            RTB_Team.ReadOnly = true;
            RTB_Team.Size = new System.Drawing.Size(204, 183);
            RTB_Team.TabIndex = 1;
            RTB_Team.Text = "";
            RTB_Team.WordWrap = false;
            // 
            // B_Close
            // 
            B_Close.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Close.Location = new System.Drawing.Point(536, 302);
            B_Close.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Close.Name = "B_Close";
            B_Close.Size = new System.Drawing.Size(88, 32);
            B_Close.TabIndex = 3;
            B_Close.Text = "Save";
            B_Close.UseVisualStyleBackColor = true;
            B_Close.Click += B_Close_Click;
            // 
            // PB_Sprite
            // 
            PB_Sprite.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            PB_Sprite.Location = new System.Drawing.Point(183, 108);
            PB_Sprite.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PB_Sprite.Name = "PB_Sprite";
            PB_Sprite.Size = new System.Drawing.Size(81, 67);
            PB_Sprite.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            PB_Sprite.TabIndex = 31;
            PB_Sprite.TabStop = false;
            // 
            // NUP_PartyIndex
            // 
            NUP_PartyIndex.Location = new System.Drawing.Point(120, 25);
            NUP_PartyIndex.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUP_PartyIndex.Maximum = new decimal(new int[] { 6, 0, 0, 0 });
            NUP_PartyIndex.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NUP_PartyIndex.Name = "NUP_PartyIndex";
            NUP_PartyIndex.Size = new System.Drawing.Size(35, 23);
            NUP_PartyIndex.TabIndex = 32;
            NUP_PartyIndex.Value = new decimal(new int[] { 1, 0, 0, 0 });
            NUP_PartyIndex.ValueChanged += NUP_PartyIndex_ValueChanged;
            // 
            // L_PartyNum
            // 
            L_PartyNum.Location = new System.Drawing.Point(12, 24);
            L_PartyNum.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_PartyNum.Name = "L_PartyNum";
            L_PartyNum.Size = new System.Drawing.Size(100, 20);
            L_PartyNum.TabIndex = 33;
            L_PartyNum.Text = "Party Index:";
            L_PartyNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_Species
            // 
            CB_Species.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Species.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Species.FormattingEnabled = true;
            CB_Species.Location = new System.Drawing.Point(120, 53);
            CB_Species.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Species.Name = "CB_Species";
            CB_Species.Size = new System.Drawing.Size(144, 23);
            CB_Species.TabIndex = 35;
            CB_Species.SelectedIndexChanged += CB_Species_SelectedIndexChanged;
            // 
            // Label_Species
            // 
            Label_Species.Location = new System.Drawing.Point(51, 56);
            Label_Species.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label_Species.Name = "Label_Species";
            Label_Species.Size = new System.Drawing.Size(61, 15);
            Label_Species.TabIndex = 34;
            Label_Species.Text = "Species:";
            Label_Species.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CHK_Nicknamed
            // 
            CHK_Nicknamed.Location = new System.Drawing.Point(16, 82);
            CHK_Nicknamed.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_Nicknamed.Name = "CHK_Nicknamed";
            CHK_Nicknamed.Size = new System.Drawing.Size(96, 20);
            CHK_Nicknamed.TabIndex = 36;
            CHK_Nicknamed.Text = "Nickname:";
            CHK_Nicknamed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            CHK_Nicknamed.UseVisualStyleBackColor = true;
            CHK_Nicknamed.CheckedChanged += UpdateNickname;
            // 
            // TB_Nickname
            // 
            TB_Nickname.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_Nickname.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            TB_Nickname.Location = new System.Drawing.Point(120, 82);
            TB_Nickname.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_Nickname.MaxLength = 12;
            TB_Nickname.Name = "TB_Nickname";
            TB_Nickname.Size = new System.Drawing.Size(144, 20);
            TB_Nickname.TabIndex = 37;
            TB_Nickname.Text = "WWWWWWWWWW";
            TB_Nickname.TextChanged += TB_Nickname_TextChanged;
            TB_Nickname.MouseDown += ClickNickname;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(440, 302);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(88, 32);
            B_Cancel.TabIndex = 71;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // L_Level
            // 
            L_Level.AutoSize = true;
            L_Level.Location = new System.Drawing.Point(179, 31);
            L_Level.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Level.Name = "L_Level";
            L_Level.Size = new System.Drawing.Size(37, 15);
            L_Level.TabIndex = 78;
            L_Level.Text = "Level:";
            // 
            // B_Delete
            // 
            B_Delete.Location = new System.Drawing.Point(136, 203);
            B_Delete.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Delete.Name = "B_Delete";
            B_Delete.Size = new System.Drawing.Size(136, 32);
            B_Delete.TabIndex = 80;
            B_Delete.Text = "Clear Team";
            B_Delete.UseVisualStyleBackColor = true;
            B_Delete.Click += B_Delete_Click;
            // 
            // GB_Entry
            // 
            GB_Entry.Controls.Add(B_ClearSlot);
            GB_Entry.Controls.Add(NUD_Level);
            GB_Entry.Controls.Add(PB_Sprite);
            GB_Entry.Controls.Add(NUP_PartyIndex);
            GB_Entry.Controls.Add(L_PartyNum);
            GB_Entry.Controls.Add(L_Level);
            GB_Entry.Controls.Add(Label_Species);
            GB_Entry.Controls.Add(CB_Species);
            GB_Entry.Controls.Add(TB_Nickname);
            GB_Entry.Controls.Add(CHK_Nicknamed);
            GB_Entry.Location = new System.Drawing.Point(349, 12);
            GB_Entry.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Entry.Name = "GB_Entry";
            GB_Entry.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Entry.Size = new System.Drawing.Size(280, 185);
            GB_Entry.TabIndex = 81;
            GB_Entry.TabStop = false;
            GB_Entry.Text = "Entry";
            // 
            // B_ClearSlot
            // 
            B_ClearSlot.Location = new System.Drawing.Point(8, 143);
            B_ClearSlot.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ClearSlot.Name = "B_ClearSlot";
            B_ClearSlot.Size = new System.Drawing.Size(136, 32);
            B_ClearSlot.TabIndex = 83;
            B_ClearSlot.Text = "Clear Slot";
            B_ClearSlot.UseVisualStyleBackColor = true;
            B_ClearSlot.Click += B_ClearSlot_Click;
            // 
            // NUD_Level
            // 
            NUD_Level.Location = new System.Drawing.Point(223, 25);
            NUD_Level.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_Level.Name = "NUD_Level";
            NUD_Level.Size = new System.Drawing.Size(41, 23);
            NUD_Level.TabIndex = 79;
            NUD_Level.ValueChanged += NUD_Level_ValueChanged;
            // 
            // B_SetParty
            // 
            B_SetParty.Location = new System.Drawing.Point(136, 263);
            B_SetParty.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_SetParty.Name = "B_SetParty";
            B_SetParty.Size = new System.Drawing.Size(136, 32);
            B_SetParty.TabIndex = 82;
            B_SetParty.Text = "Add Current Party";
            B_SetParty.UseVisualStyleBackColor = true;
            B_SetParty.Click += B_SetParty_Click;
            // 
            // NUD_Clears
            // 
            NUD_Clears.Location = new System.Drawing.Point(349, 309);
            NUD_Clears.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_Clears.Name = "NUD_Clears";
            NUD_Clears.Size = new System.Drawing.Size(45, 23);
            NUD_Clears.TabIndex = 83;
            // 
            // L_Clears
            // 
            L_Clears.Location = new System.Drawing.Point(278, 306);
            L_Clears.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Clears.Name = "L_Clears";
            L_Clears.Size = new System.Drawing.Size(64, 24);
            L_Clears.TabIndex = 84;
            L_Clears.Text = "Teams:";
            L_Clears.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // B_ClearAll
            // 
            B_ClearAll.Location = new System.Drawing.Point(136, 301);
            B_ClearAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ClearAll.Name = "B_ClearAll";
            B_ClearAll.Size = new System.Drawing.Size(136, 32);
            B_ClearAll.TabIndex = 86;
            B_ClearAll.Text = "Clear All";
            B_ClearAll.UseVisualStyleBackColor = true;
            B_ClearAll.Click += B_ClearAll_Click;
            // 
            // SAV_HallOfFame1
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(637, 342);
            Controls.Add(B_ClearAll);
            Controls.Add(L_Clears);
            Controls.Add(NUD_Clears);
            Controls.Add(B_SetParty);
            Controls.Add(B_Delete);
            Controls.Add(B_Cancel);
            Controls.Add(B_Close);
            Controls.Add(RTB_Team);
            Controls.Add(LB_DataEntry);
            Controls.Add(GB_Entry);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_HallOfFame1";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Hall of Fame Viewer";
            ((System.ComponentModel.ISupportInitialize)PB_Sprite).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUP_PartyIndex).EndInit();
            GB_Entry.ResumeLayout(false);
            GB_Entry.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Level).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Clears).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ListBox LB_DataEntry;
        private System.Windows.Forms.RichTextBox RTB_Team;
        private System.Windows.Forms.Button B_Close;
        private System.Windows.Forms.PictureBox PB_Sprite;
        private System.Windows.Forms.NumericUpDown NUP_PartyIndex;
        private System.Windows.Forms.Label L_PartyNum;
        public System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.Label Label_Species;
        private System.Windows.Forms.CheckBox CHK_Nicknamed;
        public System.Windows.Forms.TextBox TB_Nickname;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Label L_Level;
        private System.Windows.Forms.Button B_Delete;
        private System.Windows.Forms.GroupBox GB_Entry;
        private System.Windows.Forms.NumericUpDown NUD_Level;
        private System.Windows.Forms.Button B_SetParty;
        private System.Windows.Forms.Button B_ClearSlot;
        private System.Windows.Forms.NumericUpDown NUD_Clears;
        private System.Windows.Forms.Label L_Clears;
        private System.Windows.Forms.Button B_ClearAll;
    }
}
