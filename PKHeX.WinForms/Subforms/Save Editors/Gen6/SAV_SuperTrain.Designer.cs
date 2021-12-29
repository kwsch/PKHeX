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
            this.CB_Species2 = new System.Windows.Forms.ComboBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.TB_Time1 = new System.Windows.Forms.TextBox();
            this.CB_Species1 = new System.Windows.Forms.ComboBox();
            this.MTB_Form1 = new System.Windows.Forms.MaskedTextBox();
            this.L_Species = new System.Windows.Forms.Label();
            this.L_Time0 = new System.Windows.Forms.Label();
            this.L_Records = new System.Windows.Forms.Label();
            this.L_Bags = new System.Windows.Forms.Label();
            this.TB_Time2 = new System.Windows.Forms.TextBox();
            this.MTB_Gender1 = new System.Windows.Forms.MaskedTextBox();
            this.MTB_Gender2 = new System.Windows.Forms.MaskedTextBox();
            this.MTB_Form2 = new System.Windows.Forms.MaskedTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(258, 281);
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
            this.B_Save.Location = new System.Drawing.Point(324, 281);
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
            this.dataGridView1.Size = new System.Drawing.Size(240, 139);
            this.dataGridView1.TabIndex = 116;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DropClick);
            // 
            // CB_Species2
            // 
            this.CB_Species2.FormattingEnabled = true;
            this.CB_Species2.Location = new System.Drawing.Point(283, 41);
            this.CB_Species2.Name = "CB_Species2";
            this.CB_Species2.Size = new System.Drawing.Size(101, 21);
            this.CB_Species2.TabIndex = 120;
            this.CB_Species2.SelectedIndexChanged += new System.EventHandler(this.ChangeRecordSpecies2);
            // 
            // listBox1
            // 
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
            // TB_Time1
            // 
            this.TB_Time1.Location = new System.Drawing.Point(174, 118);
            this.TB_Time1.Name = "TB_Time1";
            this.TB_Time1.Size = new System.Drawing.Size(100, 20);
            this.TB_Time1.TabIndex = 125;
            this.TB_Time1.TextChanged += new System.EventHandler(this.ChangeRecordTime1);
            // 
            // CB_Species1
            // 
            this.CB_Species1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_Species1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Species1.FormattingEnabled = true;
            this.CB_Species1.Location = new System.Drawing.Point(174, 41);
            this.CB_Species1.Name = "CB_Species1";
            this.CB_Species1.Size = new System.Drawing.Size(100, 21);
            this.CB_Species1.TabIndex = 126;
            this.CB_Species1.SelectedIndexChanged += new System.EventHandler(this.ChangeRecordSpecies1);
            // 
            // MTB_Form1
            // 
            this.MTB_Form1.Location = new System.Drawing.Point(237, 68);
            this.MTB_Form1.Name = "MTB_Form1";
            this.MTB_Form1.Size = new System.Drawing.Size(37, 20);
            this.MTB_Form1.TabIndex = 127;
            this.MTB_Form1.TextChanged += new System.EventHandler(this.ChangeRecordMisc1);
            // 
            // L_Species
            // 
            this.L_Species.AutoSize = true;
            this.L_Species.Location = new System.Drawing.Point(176, 25);
            this.L_Species.Name = "L_Species";
            this.L_Species.Size = new System.Drawing.Size(48, 13);
            this.L_Species.TabIndex = 129;
            this.L_Species.Text = "Species:";
            // 
            // L_Time0
            // 
            this.L_Time0.AutoSize = true;
            this.L_Time0.Location = new System.Drawing.Point(176, 102);
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
            // TB_Time2
            // 
            this.TB_Time2.Location = new System.Drawing.Point(284, 118);
            this.TB_Time2.Name = "TB_Time2";
            this.TB_Time2.Size = new System.Drawing.Size(100, 20);
            this.TB_Time2.TabIndex = 136;
            // 
            // MTB_Gender1
            // 
            this.MTB_Gender1.Location = new System.Drawing.Point(196, 68);
            this.MTB_Gender1.Name = "MTB_Gender1";
            this.MTB_Gender1.Size = new System.Drawing.Size(35, 20);
            this.MTB_Gender1.TabIndex = 137;
            this.MTB_Gender1.TextChanged += new System.EventHandler(this.ChangeRecordMisc1);
            // 
            // MTB_Gender2
            // 
            this.MTB_Gender2.Location = new System.Drawing.Point(306, 68);
            this.MTB_Gender2.Name = "MTB_Gender2";
            this.MTB_Gender2.Size = new System.Drawing.Size(35, 20);
            this.MTB_Gender2.TabIndex = 139;
            this.MTB_Gender2.TextChanged += new System.EventHandler(this.ChangeRecordMisc2);
            // 
            // MTB_Form2
            // 
            this.MTB_Form2.Location = new System.Drawing.Point(347, 68);
            this.MTB_Form2.Name = "MTB_Form2";
            this.MTB_Form2.Size = new System.Drawing.Size(37, 20);
            this.MTB_Form2.TabIndex = 138;
            this.MTB_Form2.TextChanged += new System.EventHandler(this.ChangeRecordMisc2);
            // 
            // SAV_SuperTrain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 312);
            this.Controls.Add(this.MTB_Gender2);
            this.Controls.Add(this.MTB_Form2);
            this.Controls.Add(this.MTB_Gender1);
            this.Controls.Add(this.TB_Time2);
            this.Controls.Add(this.L_Bags);
            this.Controls.Add(this.L_Records);
            this.Controls.Add(this.L_Time0);
            this.Controls.Add(this.L_Species);
            this.Controls.Add(this.MTB_Form1);
            this.Controls.Add(this.CB_Species1);
            this.Controls.Add(this.TB_Time1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.CB_Species2);
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
        private System.Windows.Forms.ComboBox CB_Species2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox TB_Time1;
        private System.Windows.Forms.ComboBox CB_Species1;
        private System.Windows.Forms.MaskedTextBox MTB_Form1;
        private System.Windows.Forms.Label L_Species;
        private System.Windows.Forms.Label L_Time0;
        private System.Windows.Forms.Label L_Records;
        private System.Windows.Forms.Label L_Bags;
        private System.Windows.Forms.TextBox TB_Time2;
        private System.Windows.Forms.MaskedTextBox MTB_Gender1;
        private System.Windows.Forms.MaskedTextBox MTB_Gender2;
        private System.Windows.Forms.MaskedTextBox MTB_Form2;
    }
}