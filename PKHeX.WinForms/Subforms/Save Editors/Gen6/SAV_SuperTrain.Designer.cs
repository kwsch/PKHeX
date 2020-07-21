namespace PKHeX.WinForms
{
    partial class SAV_SuperTrain
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.TB_Time1 = new System.Windows.Forms.TextBox();
            this.TB_Time2 = new System.Windows.Forms.TextBox();
            this.CB_S2 = new System.Windows.Forms.ComboBox();
            this.L_Time1 = new System.Windows.Forms.Label();
            this.L_Time2 = new System.Windows.Forms.Label();
            this.L_Species2 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.TB_Time = new System.Windows.Forms.TextBox();
            this.CB_Species = new System.Windows.Forms.ComboBox();
            this.TB_Unk = new System.Windows.Forms.MaskedTextBox();
            this.L_Unk = new System.Windows.Forms.Label();
            this.L_Species = new System.Windows.Forms.Label();
            this.L_Time0 = new System.Windows.Forms.Label();
            this.L_Records = new System.Windows.Forms.Label();
            this.L_Bags = new System.Windows.Forms.Label();
            this.L_UNKNOWN = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(176, 281);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(60, 23);
            this.B_Cancel.TabIndex = 0;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(242, 281);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(60, 23);
            this.B_Save.TabIndex = 1;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ColumnHeadersVisible = false;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView1.Location = new System.Drawing.Point(12, 165);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.ShowEditingIcon = false;
            this.dataGridView1.Size = new System.Drawing.Size(158, 139);
            this.dataGridView1.TabIndex = 116;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DropClick);
            // 
            // TB_Time1
            // 
            this.TB_Time1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_Time1.Location = new System.Drawing.Point(178, 181);
            this.TB_Time1.Name = "TB_Time1";
            this.TB_Time1.Size = new System.Drawing.Size(100, 20);
            this.TB_Time1.TabIndex = 118;
            // 
            // TB_Time2
            // 
            this.TB_Time2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_Time2.Location = new System.Drawing.Point(178, 220);
            this.TB_Time2.Name = "TB_Time2";
            this.TB_Time2.Size = new System.Drawing.Size(101, 20);
            this.TB_Time2.TabIndex = 119;
            // 
            // CB_S2
            // 
            this.CB_S2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_S2.FormattingEnabled = true;
            this.CB_S2.Location = new System.Drawing.Point(178, 259);
            this.CB_S2.Name = "CB_S2";
            this.CB_S2.Size = new System.Drawing.Size(101, 21);
            this.CB_S2.TabIndex = 120;
            // 
            // L_Time1
            // 
            this.L_Time1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_Time1.AutoSize = true;
            this.L_Time1.Location = new System.Drawing.Point(176, 165);
            this.L_Time1.Name = "L_Time1";
            this.L_Time1.Size = new System.Drawing.Size(36, 13);
            this.L_Time1.TabIndex = 121;
            this.L_Time1.Text = "Time1";
            // 
            // L_Time2
            // 
            this.L_Time2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_Time2.AutoSize = true;
            this.L_Time2.Location = new System.Drawing.Point(176, 204);
            this.L_Time2.Name = "L_Time2";
            this.L_Time2.Size = new System.Drawing.Size(36, 13);
            this.L_Time2.TabIndex = 122;
            this.L_Time2.Text = "Time2";
            // 
            // L_Species2
            // 
            this.L_Species2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_Species2.AutoSize = true;
            this.L_Species2.Location = new System.Drawing.Point(175, 243);
            this.L_Species2.Name = "L_Species2";
            this.L_Species2.Size = new System.Drawing.Size(45, 13);
            this.L_Species2.TabIndex = 123;
            this.L_Species2.Text = "Species";
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Items.AddRange(new object[] {
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
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30"});
            this.listBox1.Location = new System.Drawing.Point(12, 25);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(158, 121);
            this.listBox1.TabIndex = 124;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.ChangeListRecordSelection);
            // 
            // TB_Time
            // 
            this.TB_Time.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_Time.Location = new System.Drawing.Point(176, 115);
            this.TB_Time.Name = "TB_Time";
            this.TB_Time.Size = new System.Drawing.Size(100, 20);
            this.TB_Time.TabIndex = 125;
            this.TB_Time.TextChanged += new System.EventHandler(this.ChangeRecordTime);
            // 
            // CB_Species
            // 
            this.CB_Species.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_Species.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_Species.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Species.FormattingEnabled = true;
            this.CB_Species.Location = new System.Drawing.Point(176, 38);
            this.CB_Species.Name = "CB_Species";
            this.CB_Species.Size = new System.Drawing.Size(100, 21);
            this.CB_Species.TabIndex = 126;
            this.CB_Species.SelectedIndexChanged += new System.EventHandler(this.ChangeRecordSpecies);
            // 
            // TB_Unk
            // 
            this.TB_Unk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_Unk.Location = new System.Drawing.Point(219, 65);
            this.TB_Unk.Name = "TB_Unk";
            this.TB_Unk.Size = new System.Drawing.Size(57, 20);
            this.TB_Unk.TabIndex = 127;
            this.TB_Unk.TextChanged += new System.EventHandler(this.ChangeRecordVal);
            // 
            // L_Unk
            // 
            this.L_Unk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_Unk.AutoSize = true;
            this.L_Unk.Location = new System.Drawing.Point(178, 68);
            this.L_Unk.Name = "L_Unk";
            this.L_Unk.Size = new System.Drawing.Size(39, 13);
            this.L_Unk.TabIndex = 128;
            this.L_Unk.Text = "L_Unk";
            // 
            // L_Species
            // 
            this.L_Species.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_Species.AutoSize = true;
            this.L_Species.Location = new System.Drawing.Point(178, 22);
            this.L_Species.Name = "L_Species";
            this.L_Species.Size = new System.Drawing.Size(48, 13);
            this.L_Species.TabIndex = 129;
            this.L_Species.Text = "Species:";
            // 
            // L_Time0
            // 
            this.L_Time0.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_Time0.AutoSize = true;
            this.L_Time0.Location = new System.Drawing.Point(178, 99);
            this.L_Time0.Name = "L_Time0";
            this.L_Time0.Size = new System.Drawing.Size(33, 13);
            this.L_Time0.TabIndex = 130;
            this.L_Time0.Text = "Time:";
            // 
            // L_Records
            // 
            this.L_Records.AutoSize = true;
            this.L_Records.Location = new System.Drawing.Point(9, 9);
            this.L_Records.Name = "L_Records";
            this.L_Records.Size = new System.Drawing.Size(47, 13);
            this.L_Records.TabIndex = 131;
            this.L_Records.Text = "Records";
            // 
            // L_Bags
            // 
            this.L_Bags.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.L_Bags.AutoSize = true;
            this.L_Bags.Location = new System.Drawing.Point(12, 149);
            this.L_Bags.Name = "L_Bags";
            this.L_Bags.Size = new System.Drawing.Size(72, 13);
            this.L_Bags.TabIndex = 132;
            this.L_Bags.Text = "Training Bags";
            // 
            // L_UNKNOWN
            // 
            this.L_UNKNOWN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_UNKNOWN.AutoSize = true;
            this.L_UNKNOWN.Location = new System.Drawing.Point(227, 165);
            this.L_UNKNOWN.Name = "L_UNKNOWN";
            this.L_UNKNOWN.Size = new System.Drawing.Size(65, 13);
            this.L_UNKNOWN.TabIndex = 133;
            this.L_UNKNOWN.Text = "UNKNOWN";
            // 
            // SAV_SuperTrain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 312);
            this.Controls.Add(this.L_UNKNOWN);
            this.Controls.Add(this.L_Bags);
            this.Controls.Add(this.L_Records);
            this.Controls.Add(this.L_Time0);
            this.Controls.Add(this.L_Species);
            this.Controls.Add(this.L_Unk);
            this.Controls.Add(this.TB_Unk);
            this.Controls.Add(this.CB_Species);
            this.Controls.Add(this.TB_Time);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.L_Species2);
            this.Controls.Add(this.L_Time2);
            this.Controls.Add(this.L_Time1);
            this.Controls.Add(this.CB_S2);
            this.Controls.Add(this.TB_Time2);
            this.Controls.Add(this.TB_Time1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_Cancel);
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(600, 630);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(330, 350);
            this.Name = "SAV_SuperTrain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Super Training Records";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox TB_Time1;
        private System.Windows.Forms.TextBox TB_Time2;
        private System.Windows.Forms.ComboBox CB_S2;
        private System.Windows.Forms.Label L_Time1;
        private System.Windows.Forms.Label L_Time2;
        private System.Windows.Forms.Label L_Species2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox TB_Time;
        private System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.MaskedTextBox TB_Unk;
        private System.Windows.Forms.Label L_Unk;
        private System.Windows.Forms.Label L_Species;
        private System.Windows.Forms.Label L_Time0;
        private System.Windows.Forms.Label L_Records;
        private System.Windows.Forms.Label L_Bags;
        private System.Windows.Forms.Label L_UNKNOWN;
    }
}