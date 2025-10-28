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
            B_Cancel = new System.Windows.Forms.Button();
            B_Save = new System.Windows.Forms.Button();
            dataGridView1 = new System.Windows.Forms.DataGridView();
            CB_Species2 = new System.Windows.Forms.ComboBox();
            listBox1 = new System.Windows.Forms.ListBox();
            TB_Time1 = new System.Windows.Forms.TextBox();
            CB_Species1 = new System.Windows.Forms.ComboBox();
            MTB_Form1 = new System.Windows.Forms.MaskedTextBox();
            L_Species = new System.Windows.Forms.Label();
            L_Time0 = new System.Windows.Forms.Label();
            L_Records = new System.Windows.Forms.Label();
            L_Bags = new System.Windows.Forms.Label();
            TB_Time2 = new System.Windows.Forms.TextBox();
            MTB_Gender1 = new System.Windows.Forms.MaskedTextBox();
            MTB_Gender2 = new System.Windows.Forms.MaskedTextBox();
            MTB_Form2 = new System.Windows.Forms.MaskedTextBox();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(301, 324);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(70, 27);
            B_Cancel.TabIndex = 0;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(378, 324);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(70, 27);
            B_Save.TabIndex = 1;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.ColumnHeadersVisible = false;
            dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            dataGridView1.Location = new System.Drawing.Point(14, 190);
            dataGridView1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dataGridView1.MultiSelect = false;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            dataGridView1.ShowEditingIcon = false;
            dataGridView1.Size = new System.Drawing.Size(280, 160);
            dataGridView1.TabIndex = 116;
            dataGridView1.CellClick += DropClick;
            // 
            // CB_Species2
            // 
            CB_Species2.FormattingEnabled = true;
            CB_Species2.Location = new System.Drawing.Point(330, 47);
            CB_Species2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Species2.Name = "CB_Species2";
            CB_Species2.Size = new System.Drawing.Size(117, 23);
            CB_Species2.TabIndex = 120;
            CB_Species2.SelectedIndexChanged += ChangeRecordSpecies2;
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Items.AddRange(new object[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30" });
            listBox1.Location = new System.Drawing.Point(14, 29);
            listBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            listBox1.Name = "listBox1";
            listBox1.Size = new System.Drawing.Size(184, 139);
            listBox1.TabIndex = 124;
            listBox1.SelectedIndexChanged += ChangeListRecordSelection;
            // 
            // TB_Time1
            // 
            TB_Time1.Location = new System.Drawing.Point(203, 136);
            TB_Time1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_Time1.Name = "TB_Time1";
            TB_Time1.Size = new System.Drawing.Size(116, 23);
            TB_Time1.TabIndex = 125;
            TB_Time1.TextChanged += ChangeRecordTime1;
            // 
            // CB_Species1
            // 
            CB_Species1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Species1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Species1.FormattingEnabled = true;
            CB_Species1.Location = new System.Drawing.Point(203, 47);
            CB_Species1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Species1.Name = "CB_Species1";
            CB_Species1.Size = new System.Drawing.Size(116, 23);
            CB_Species1.TabIndex = 126;
            CB_Species1.SelectedIndexChanged += ChangeRecordSpecies1;
            // 
            // MTB_Form1
            // 
            MTB_Form1.Location = new System.Drawing.Point(276, 78);
            MTB_Form1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MTB_Form1.Name = "MTB_Form1";
            MTB_Form1.Size = new System.Drawing.Size(42, 23);
            MTB_Form1.TabIndex = 127;
            MTB_Form1.TextChanged += ChangeRecordMisc1;
            // 
            // L_Species
            // 
            L_Species.AutoSize = true;
            L_Species.Location = new System.Drawing.Point(205, 29);
            L_Species.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Species.Name = "L_Species";
            L_Species.Size = new System.Drawing.Size(49, 15);
            L_Species.TabIndex = 129;
            L_Species.Text = "Species:";
            // 
            // L_Time0
            // 
            L_Time0.AutoSize = true;
            L_Time0.Location = new System.Drawing.Point(205, 118);
            L_Time0.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Time0.Name = "L_Time0";
            L_Time0.Size = new System.Drawing.Size(36, 15);
            L_Time0.TabIndex = 130;
            L_Time0.Text = "Time:";
            // 
            // L_Records
            // 
            L_Records.AutoSize = true;
            L_Records.Location = new System.Drawing.Point(10, 10);
            L_Records.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Records.Name = "L_Records";
            L_Records.Size = new System.Drawing.Size(49, 15);
            L_Records.TabIndex = 131;
            L_Records.Text = "Records";
            // 
            // L_Bags
            // 
            L_Bags.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            L_Bags.AutoSize = true;
            L_Bags.Location = new System.Drawing.Point(14, 172);
            L_Bags.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Bags.Name = "L_Bags";
            L_Bags.Size = new System.Drawing.Size(77, 15);
            L_Bags.TabIndex = 132;
            L_Bags.Text = "Training Bags";
            // 
            // TB_Time2
            // 
            TB_Time2.Location = new System.Drawing.Point(331, 136);
            TB_Time2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_Time2.Name = "TB_Time2";
            TB_Time2.Size = new System.Drawing.Size(116, 23);
            TB_Time2.TabIndex = 136;
            TB_Time2.TextChanged += ChangeRecordTime2;
            // 
            // MTB_Gender1
            // 
            MTB_Gender1.Location = new System.Drawing.Point(229, 78);
            MTB_Gender1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MTB_Gender1.Name = "MTB_Gender1";
            MTB_Gender1.Size = new System.Drawing.Size(40, 23);
            MTB_Gender1.TabIndex = 137;
            MTB_Gender1.TextChanged += ChangeRecordMisc1;
            // 
            // MTB_Gender2
            // 
            MTB_Gender2.Location = new System.Drawing.Point(357, 78);
            MTB_Gender2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MTB_Gender2.Name = "MTB_Gender2";
            MTB_Gender2.Size = new System.Drawing.Size(40, 23);
            MTB_Gender2.TabIndex = 139;
            MTB_Gender2.TextChanged += ChangeRecordMisc2;
            // 
            // MTB_Form2
            // 
            MTB_Form2.Location = new System.Drawing.Point(405, 78);
            MTB_Form2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MTB_Form2.Name = "MTB_Form2";
            MTB_Form2.Size = new System.Drawing.Size(42, 23);
            MTB_Form2.TabIndex = 138;
            MTB_Form2.TextChanged += ChangeRecordMisc2;
            // 
            // SAV_SuperTrain
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(462, 360);
            Controls.Add(MTB_Gender2);
            Controls.Add(MTB_Form2);
            Controls.Add(MTB_Gender1);
            Controls.Add(TB_Time2);
            Controls.Add(L_Bags);
            Controls.Add(L_Records);
            Controls.Add(L_Time0);
            Controls.Add(L_Species);
            Controls.Add(MTB_Form1);
            Controls.Add(CB_Species1);
            Controls.Add(TB_Time1);
            Controls.Add(listBox1);
            Controls.Add(CB_Species2);
            Controls.Add(dataGridView1);
            Controls.Add(B_Save);
            Controls.Add(B_Cancel);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(697, 721);
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(382, 398);
            Name = "SAV_SuperTrain";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Super Training Records";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
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
