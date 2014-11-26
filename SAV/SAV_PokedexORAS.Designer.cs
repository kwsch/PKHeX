namespace PKHeX
{
    partial class SAV_PokedexORAS
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SAV_PokedexORAS));
            this.B_Cancel = new System.Windows.Forms.Button();
            this.LB_Species = new System.Windows.Forms.ListBox();
            this.CHK_P1 = new System.Windows.Forms.CheckBox();
            this.CHK_P2 = new System.Windows.Forms.CheckBox();
            this.CHK_P3 = new System.Windows.Forms.CheckBox();
            this.CHK_P4 = new System.Windows.Forms.CheckBox();
            this.CHK_P5 = new System.Windows.Forms.CheckBox();
            this.CHK_P6 = new System.Windows.Forms.CheckBox();
            this.CHK_P7 = new System.Windows.Forms.CheckBox();
            this.CHK_P8 = new System.Windows.Forms.CheckBox();
            this.CHK_P9 = new System.Windows.Forms.CheckBox();
            this.CHK_P10 = new System.Windows.Forms.CheckBox();
            this.CHK_L7 = new System.Windows.Forms.CheckBox();
            this.CHK_L6 = new System.Windows.Forms.CheckBox();
            this.CHK_L5 = new System.Windows.Forms.CheckBox();
            this.CHK_L4 = new System.Windows.Forms.CheckBox();
            this.CHK_L3 = new System.Windows.Forms.CheckBox();
            this.CHK_L2 = new System.Windows.Forms.CheckBox();
            this.CHK_L1 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.CB_Species = new System.Windows.Forms.ComboBox();
            this.B_GiveAll = new System.Windows.Forms.Button();
            this.B_Save = new System.Windows.Forms.Button();
            this.B_FillDex = new System.Windows.Forms.Button();
            this.GB_Language = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.CHK_F1 = new System.Windows.Forms.CheckBox();
            this.TB_Spinda = new System.Windows.Forms.TextBox();
            this.L_Spinda = new System.Windows.Forms.Label();
            this.maskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.GB_Language.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // B_Cancel
            // 
            this.B_Cancel.Location = new System.Drawing.Point(297, 242);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(80, 23);
            this.B_Cancel.TabIndex = 0;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // LB_Species
            // 
            this.LB_Species.FormattingEnabled = true;
            this.LB_Species.Location = new System.Drawing.Point(12, 40);
            this.LB_Species.Name = "LB_Species";
            this.LB_Species.Size = new System.Drawing.Size(130, 225);
            this.LB_Species.TabIndex = 2;
            this.LB_Species.SelectedIndexChanged += new System.EventHandler(this.changeLBSpecies);
            // 
            // CHK_P1
            // 
            this.CHK_P1.AutoSize = true;
            this.CHK_P1.Location = new System.Drawing.Point(12, 93);
            this.CHK_P1.Name = "CHK_P1";
            this.CHK_P1.Size = new System.Drawing.Size(96, 17);
            this.CHK_P1.TabIndex = 3;
            this.CHK_P1.Text = "Native (ORAS)";
            this.CHK_P1.UseVisualStyleBackColor = true;
            this.CHK_P1.Click += new System.EventHandler(this.changePartitionBool);
            // 
            // CHK_P2
            // 
            this.CHK_P2.AutoSize = true;
            this.CHK_P2.Location = new System.Drawing.Point(12, 18);
            this.CHK_P2.Name = "CHK_P2";
            this.CHK_P2.Size = new System.Drawing.Size(49, 17);
            this.CHK_P2.TabIndex = 4;
            this.CHK_P2.Text = "Male";
            this.CHK_P2.UseVisualStyleBackColor = true;
            this.CHK_P2.Click += new System.EventHandler(this.changePartitionBool);
            // 
            // CHK_P3
            // 
            this.CHK_P3.AutoSize = true;
            this.CHK_P3.Location = new System.Drawing.Point(12, 33);
            this.CHK_P3.Name = "CHK_P3";
            this.CHK_P3.Size = new System.Drawing.Size(60, 17);
            this.CHK_P3.TabIndex = 5;
            this.CHK_P3.Text = "Female";
            this.CHK_P3.UseVisualStyleBackColor = true;
            this.CHK_P3.Click += new System.EventHandler(this.changePartitionBool);
            // 
            // CHK_P4
            // 
            this.CHK_P4.AutoSize = true;
            this.CHK_P4.Location = new System.Drawing.Point(12, 48);
            this.CHK_P4.Name = "CHK_P4";
            this.CHK_P4.Size = new System.Drawing.Size(78, 17);
            this.CHK_P4.TabIndex = 6;
            this.CHK_P4.Text = "Shiny Male";
            this.CHK_P4.UseVisualStyleBackColor = true;
            this.CHK_P4.Click += new System.EventHandler(this.changePartitionBool);
            // 
            // CHK_P5
            // 
            this.CHK_P5.AutoSize = true;
            this.CHK_P5.Location = new System.Drawing.Point(12, 63);
            this.CHK_P5.Name = "CHK_P5";
            this.CHK_P5.Size = new System.Drawing.Size(89, 17);
            this.CHK_P5.TabIndex = 7;
            this.CHK_P5.Text = "Shiny Female";
            this.CHK_P5.UseVisualStyleBackColor = true;
            this.CHK_P5.Click += new System.EventHandler(this.changePartitionBool);
            // 
            // CHK_P6
            // 
            this.CHK_P6.AutoSize = true;
            this.CHK_P6.Location = new System.Drawing.Point(12, 20);
            this.CHK_P6.Name = "CHK_P6";
            this.CHK_P6.Size = new System.Drawing.Size(49, 17);
            this.CHK_P6.TabIndex = 8;
            this.CHK_P6.Text = "Male";
            this.CHK_P6.UseVisualStyleBackColor = true;
            this.CHK_P6.Click += new System.EventHandler(this.changePartitionBool);
            // 
            // CHK_P7
            // 
            this.CHK_P7.AutoSize = true;
            this.CHK_P7.Location = new System.Drawing.Point(12, 35);
            this.CHK_P7.Name = "CHK_P7";
            this.CHK_P7.Size = new System.Drawing.Size(60, 17);
            this.CHK_P7.TabIndex = 9;
            this.CHK_P7.Text = "Female";
            this.CHK_P7.UseVisualStyleBackColor = true;
            this.CHK_P7.Click += new System.EventHandler(this.changePartitionBool);
            // 
            // CHK_P8
            // 
            this.CHK_P8.AutoSize = true;
            this.CHK_P8.Location = new System.Drawing.Point(12, 50);
            this.CHK_P8.Name = "CHK_P8";
            this.CHK_P8.Size = new System.Drawing.Size(78, 17);
            this.CHK_P8.TabIndex = 10;
            this.CHK_P8.Text = "Shiny Male";
            this.CHK_P8.UseVisualStyleBackColor = true;
            this.CHK_P8.Click += new System.EventHandler(this.changePartitionBool);
            // 
            // CHK_P9
            // 
            this.CHK_P9.AutoSize = true;
            this.CHK_P9.Location = new System.Drawing.Point(12, 65);
            this.CHK_P9.Name = "CHK_P9";
            this.CHK_P9.Size = new System.Drawing.Size(89, 17);
            this.CHK_P9.TabIndex = 11;
            this.CHK_P9.Text = "Shiny Female";
            this.CHK_P9.UseVisualStyleBackColor = true;
            this.CHK_P9.Click += new System.EventHandler(this.changePartitionBool);
            // 
            // CHK_P10
            // 
            this.CHK_P10.AutoSize = true;
            this.CHK_P10.Location = new System.Drawing.Point(12, 78);
            this.CHK_P10.Name = "CHK_P10";
            this.CHK_P10.Size = new System.Drawing.Size(72, 17);
            this.CHK_P10.TabIndex = 12;
            this.CHK_P10.Text = "Via Trade";
            this.CHK_P10.UseVisualStyleBackColor = true;
            this.CHK_P10.Click += new System.EventHandler(this.changePartitionBool);
            // 
            // CHK_L7
            // 
            this.CHK_L7.AutoSize = true;
            this.CHK_L7.Location = new System.Drawing.Point(18, 125);
            this.CHK_L7.Name = "CHK_L7";
            this.CHK_L7.Size = new System.Drawing.Size(60, 17);
            this.CHK_L7.TabIndex = 19;
            this.CHK_L7.Text = "Korean";
            this.CHK_L7.UseVisualStyleBackColor = true;
            this.CHK_L7.Click += new System.EventHandler(this.changeLanguageBool);
            // 
            // CHK_L6
            // 
            this.CHK_L6.AutoSize = true;
            this.CHK_L6.Location = new System.Drawing.Point(18, 108);
            this.CHK_L6.Name = "CHK_L6";
            this.CHK_L6.Size = new System.Drawing.Size(64, 17);
            this.CHK_L6.TabIndex = 18;
            this.CHK_L6.Text = "Spanish";
            this.CHK_L6.UseVisualStyleBackColor = true;
            this.CHK_L6.Click += new System.EventHandler(this.changeLanguageBool);
            // 
            // CHK_L5
            // 
            this.CHK_L5.AutoSize = true;
            this.CHK_L5.Location = new System.Drawing.Point(18, 91);
            this.CHK_L5.Name = "CHK_L5";
            this.CHK_L5.Size = new System.Drawing.Size(63, 17);
            this.CHK_L5.TabIndex = 17;
            this.CHK_L5.Text = "German";
            this.CHK_L5.UseVisualStyleBackColor = true;
            this.CHK_L5.Click += new System.EventHandler(this.changeLanguageBool);
            // 
            // CHK_L4
            // 
            this.CHK_L4.AutoSize = true;
            this.CHK_L4.Location = new System.Drawing.Point(18, 74);
            this.CHK_L4.Name = "CHK_L4";
            this.CHK_L4.Size = new System.Drawing.Size(54, 17);
            this.CHK_L4.TabIndex = 16;
            this.CHK_L4.Text = "Italian";
            this.CHK_L4.UseVisualStyleBackColor = true;
            this.CHK_L4.Click += new System.EventHandler(this.changeLanguageBool);
            // 
            // CHK_L3
            // 
            this.CHK_L3.AutoSize = true;
            this.CHK_L3.Location = new System.Drawing.Point(18, 57);
            this.CHK_L3.Name = "CHK_L3";
            this.CHK_L3.Size = new System.Drawing.Size(59, 17);
            this.CHK_L3.TabIndex = 15;
            this.CHK_L3.Text = "French";
            this.CHK_L3.UseVisualStyleBackColor = true;
            this.CHK_L3.Click += new System.EventHandler(this.changeLanguageBool);
            // 
            // CHK_L2
            // 
            this.CHK_L2.AutoSize = true;
            this.CHK_L2.Location = new System.Drawing.Point(18, 40);
            this.CHK_L2.Name = "CHK_L2";
            this.CHK_L2.Size = new System.Drawing.Size(60, 17);
            this.CHK_L2.TabIndex = 14;
            this.CHK_L2.Text = "English";
            this.CHK_L2.UseVisualStyleBackColor = true;
            this.CHK_L2.Click += new System.EventHandler(this.changeLanguageBool);
            // 
            // CHK_L1
            // 
            this.CHK_L1.AutoSize = true;
            this.CHK_L1.Location = new System.Drawing.Point(18, 23);
            this.CHK_L1.Name = "CHK_L1";
            this.CHK_L1.Size = new System.Drawing.Size(72, 17);
            this.CHK_L1.TabIndex = 13;
            this.CHK_L1.Text = "Japanese";
            this.CHK_L1.UseVisualStyleBackColor = true;
            this.CHK_L1.Click += new System.EventHandler(this.changeLanguageBool);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "goto:";
            // 
            // CB_Species
            // 
            this.CB_Species.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.CB_Species.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Species.DropDownWidth = 95;
            this.CB_Species.FormattingEnabled = true;
            this.CB_Species.Items.AddRange(new object[] {
            "0"});
            this.CB_Species.Location = new System.Drawing.Point(50, 13);
            this.CB_Species.Name = "CB_Species";
            this.CB_Species.Size = new System.Drawing.Size(92, 21);
            this.CB_Species.TabIndex = 21;
            this.CB_Species.SelectedIndexChanged += new System.EventHandler(this.changeCBSpecies);
            this.CB_Species.SelectedValueChanged += new System.EventHandler(this.changeCBSpecies);
            this.CB_Species.KeyDown += new System.Windows.Forms.KeyEventHandler(this.removedropCB);
            // 
            // B_GiveAll
            // 
            this.B_GiveAll.Location = new System.Drawing.Point(251, 11);
            this.B_GiveAll.Name = "B_GiveAll";
            this.B_GiveAll.Size = new System.Drawing.Size(60, 23);
            this.B_GiveAll.TabIndex = 23;
            this.B_GiveAll.Text = "Give All";
            this.B_GiveAll.UseVisualStyleBackColor = true;
            this.B_GiveAll.Click += new System.EventHandler(this.B_GiveAll_Click);
            // 
            // B_Save
            // 
            this.B_Save.Location = new System.Drawing.Point(297, 218);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(80, 23);
            this.B_Save.TabIndex = 24;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // B_FillDex
            // 
            this.B_FillDex.Location = new System.Drawing.Point(317, 11);
            this.B_FillDex.Name = "B_FillDex";
            this.B_FillDex.Size = new System.Drawing.Size(60, 23);
            this.B_FillDex.TabIndex = 25;
            this.B_FillDex.Text = "Fill Dex";
            this.B_FillDex.UseVisualStyleBackColor = true;
            this.B_FillDex.Click += new System.EventHandler(this.B_FillDex_Click);
            // 
            // GB_Language
            // 
            this.GB_Language.Controls.Add(this.CHK_L7);
            this.GB_Language.Controls.Add(this.CHK_L6);
            this.GB_Language.Controls.Add(this.CHK_L5);
            this.GB_Language.Controls.Add(this.CHK_L4);
            this.GB_Language.Controls.Add(this.CHK_L3);
            this.GB_Language.Controls.Add(this.CHK_L2);
            this.GB_Language.Controls.Add(this.CHK_L1);
            this.GB_Language.Location = new System.Drawing.Point(269, 40);
            this.GB_Language.Name = "GB_Language";
            this.GB_Language.Size = new System.Drawing.Size(108, 153);
            this.GB_Language.TabIndex = 26;
            this.GB_Language.TabStop = false;
            this.GB_Language.Text = "Languages";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CHK_P9);
            this.groupBox1.Controls.Add(this.CHK_P8);
            this.groupBox1.Controls.Add(this.CHK_P7);
            this.groupBox1.Controls.Add(this.CHK_P6);
            this.groupBox1.Location = new System.Drawing.Point(148, 176);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(115, 89);
            this.groupBox1.TabIndex = 27;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Encountered";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.CHK_F1);
            this.groupBox2.Controls.Add(this.CHK_P1);
            this.groupBox2.Controls.Add(this.CHK_P10);
            this.groupBox2.Controls.Add(this.CHK_P5);
            this.groupBox2.Controls.Add(this.CHK_P4);
            this.groupBox2.Controls.Add(this.CHK_P3);
            this.groupBox2.Controls.Add(this.CHK_P2);
            this.groupBox2.Location = new System.Drawing.Point(148, 40);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(115, 130);
            this.groupBox2.TabIndex = 28;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Owned";
            // 
            // CHK_F1
            // 
            this.CHK_F1.AutoSize = true;
            this.CHK_F1.Location = new System.Drawing.Point(12, 108);
            this.CHK_F1.Name = "CHK_F1";
            this.CHK_F1.Size = new System.Drawing.Size(61, 17);
            this.CHK_F1.TabIndex = 13;
            this.CHK_F1.Text = "Foreign";
            this.CHK_F1.UseVisualStyleBackColor = true;
            this.CHK_F1.Click += new System.EventHandler(this.changePartitionBool);
            // 
            // TB_Spinda
            // 
            this.TB_Spinda.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_Spinda.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_Spinda.Location = new System.Drawing.Point(317, 196);
            this.TB_Spinda.Name = "TB_Spinda";
            this.TB_Spinda.Size = new System.Drawing.Size(60, 20);
            this.TB_Spinda.TabIndex = 29;
            this.TB_Spinda.Text = "12345678";
            // 
            // L_Spinda
            // 
            this.L_Spinda.AutoSize = true;
            this.L_Spinda.Location = new System.Drawing.Point(266, 198);
            this.L_Spinda.Name = "L_Spinda";
            this.L_Spinda.Size = new System.Drawing.Size(43, 13);
            this.L_Spinda.TabIndex = 30;
            this.L_Spinda.Text = "Spinda:";
            // 
            // maskedTextBox1
            // 
            this.maskedTextBox1.Location = new System.Drawing.Point(207, 13);
            this.maskedTextBox1.Mask = "00000";
            this.maskedTextBox1.Name = "maskedTextBox1";
            this.maskedTextBox1.Size = new System.Drawing.Size(38, 20);
            this.maskedTextBox1.TabIndex = 31;
            this.maskedTextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.maskedTextBox1.Visible = false;
            this.maskedTextBox1.TextChanged += new System.EventHandler(this.changeEncounteredCount);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(158, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 32;
            this.label2.Text = "Battled:";
            this.label2.Visible = false;
            // 
            // SAV_PokedexORAS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 272);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.maskedTextBox1);
            this.Controls.Add(this.L_Spinda);
            this.Controls.Add(this.TB_Spinda);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.GB_Language);
            this.Controls.Add(this.B_FillDex);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_GiveAll);
            this.Controls.Add(this.CB_Species);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LB_Species);
            this.Controls.Add(this.B_Cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SAV_PokedexORAS";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Pokédex Editor";
            this.GB_Language.ResumeLayout(false);
            this.GB_Language.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.ListBox LB_Species;
        private System.Windows.Forms.CheckBox CHK_P1;
        private System.Windows.Forms.CheckBox CHK_P2;
        private System.Windows.Forms.CheckBox CHK_P3;
        private System.Windows.Forms.CheckBox CHK_P4;
        private System.Windows.Forms.CheckBox CHK_P5;
        private System.Windows.Forms.CheckBox CHK_P6;
        private System.Windows.Forms.CheckBox CHK_P7;
        private System.Windows.Forms.CheckBox CHK_P8;
        private System.Windows.Forms.CheckBox CHK_P9;
        private System.Windows.Forms.CheckBox CHK_P10;
        private System.Windows.Forms.CheckBox CHK_L7;
        private System.Windows.Forms.CheckBox CHK_L6;
        private System.Windows.Forms.CheckBox CHK_L5;
        private System.Windows.Forms.CheckBox CHK_L4;
        private System.Windows.Forms.CheckBox CHK_L3;
        private System.Windows.Forms.CheckBox CHK_L2;
        private System.Windows.Forms.CheckBox CHK_L1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.Button B_GiveAll;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_FillDex;
        private System.Windows.Forms.GroupBox GB_Language;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox CHK_F1;
        private System.Windows.Forms.TextBox TB_Spinda;
        private System.Windows.Forms.Label L_Spinda;
        private System.Windows.Forms.MaskedTextBox maskedTextBox1;
        private System.Windows.Forms.Label label2;
    }
}