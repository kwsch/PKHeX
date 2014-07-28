namespace PKHeX
{
    partial class SAV_Pokedex
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SAV_Pokedex));
            this.B_Cancel = new System.Windows.Forms.Button();
            this.L_Beta = new System.Windows.Forms.Label();
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
            this.SuspendLayout();
            // 
            // B_Cancel
            // 
            this.B_Cancel.Location = new System.Drawing.Point(262, 189);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(80, 23);
            this.B_Cancel.TabIndex = 0;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // L_Beta
            // 
            this.L_Beta.AutoSize = true;
            this.L_Beta.ForeColor = System.Drawing.Color.Red;
            this.L_Beta.Location = new System.Drawing.Point(156, 209);
            this.L_Beta.Name = "L_Beta";
            this.L_Beta.Size = new System.Drawing.Size(84, 13);
            this.L_Beta.TabIndex = 1;
            this.L_Beta.Text = "Still researching.";
            // 
            // LB_Species
            // 
            this.LB_Species.FormattingEnabled = true;
            this.LB_Species.Location = new System.Drawing.Point(12, 40);
            this.LB_Species.Name = "LB_Species";
            this.LB_Species.Size = new System.Drawing.Size(130, 173);
            this.LB_Species.TabIndex = 2;
            this.LB_Species.SelectedIndexChanged += new System.EventHandler(this.changeLBSpecies);
            // 
            // CHK_P1
            // 
            this.CHK_P1.AutoSize = true;
            this.CHK_P1.Enabled = false;
            this.CHK_P1.Location = new System.Drawing.Point(159, 40);
            this.CHK_P1.Name = "CHK_P1";
            this.CHK_P1.Size = new System.Drawing.Size(73, 17);
            this.CHK_P1.TabIndex = 3;
            this.CHK_P1.Text = "Partition 1";
            this.CHK_P1.UseVisualStyleBackColor = true;
            // 
            // CHK_P2
            // 
            this.CHK_P2.AutoSize = true;
            this.CHK_P2.Enabled = false;
            this.CHK_P2.Location = new System.Drawing.Point(159, 57);
            this.CHK_P2.Name = "CHK_P2";
            this.CHK_P2.Size = new System.Drawing.Size(73, 17);
            this.CHK_P2.TabIndex = 4;
            this.CHK_P2.Text = "Partition 2";
            this.CHK_P2.UseVisualStyleBackColor = true;
            // 
            // CHK_P3
            // 
            this.CHK_P3.AutoSize = true;
            this.CHK_P3.Enabled = false;
            this.CHK_P3.Location = new System.Drawing.Point(159, 74);
            this.CHK_P3.Name = "CHK_P3";
            this.CHK_P3.Size = new System.Drawing.Size(73, 17);
            this.CHK_P3.TabIndex = 5;
            this.CHK_P3.Text = "Partition 3";
            this.CHK_P3.UseVisualStyleBackColor = true;
            // 
            // CHK_P4
            // 
            this.CHK_P4.AutoSize = true;
            this.CHK_P4.Enabled = false;
            this.CHK_P4.Location = new System.Drawing.Point(159, 91);
            this.CHK_P4.Name = "CHK_P4";
            this.CHK_P4.Size = new System.Drawing.Size(73, 17);
            this.CHK_P4.TabIndex = 6;
            this.CHK_P4.Text = "Partition 4";
            this.CHK_P4.UseVisualStyleBackColor = true;
            // 
            // CHK_P5
            // 
            this.CHK_P5.AutoSize = true;
            this.CHK_P5.Enabled = false;
            this.CHK_P5.Location = new System.Drawing.Point(159, 108);
            this.CHK_P5.Name = "CHK_P5";
            this.CHK_P5.Size = new System.Drawing.Size(73, 17);
            this.CHK_P5.TabIndex = 7;
            this.CHK_P5.Text = "Partition 5";
            this.CHK_P5.UseVisualStyleBackColor = true;
            // 
            // CHK_P6
            // 
            this.CHK_P6.AutoSize = true;
            this.CHK_P6.Enabled = false;
            this.CHK_P6.Location = new System.Drawing.Point(159, 125);
            this.CHK_P6.Name = "CHK_P6";
            this.CHK_P6.Size = new System.Drawing.Size(73, 17);
            this.CHK_P6.TabIndex = 8;
            this.CHK_P6.Text = "Partition 6";
            this.CHK_P6.UseVisualStyleBackColor = true;
            // 
            // CHK_P7
            // 
            this.CHK_P7.AutoSize = true;
            this.CHK_P7.Enabled = false;
            this.CHK_P7.Location = new System.Drawing.Point(159, 142);
            this.CHK_P7.Name = "CHK_P7";
            this.CHK_P7.Size = new System.Drawing.Size(73, 17);
            this.CHK_P7.TabIndex = 9;
            this.CHK_P7.Text = "Partition 7";
            this.CHK_P7.UseVisualStyleBackColor = true;
            // 
            // CHK_P8
            // 
            this.CHK_P8.AutoSize = true;
            this.CHK_P8.Enabled = false;
            this.CHK_P8.Location = new System.Drawing.Point(159, 159);
            this.CHK_P8.Name = "CHK_P8";
            this.CHK_P8.Size = new System.Drawing.Size(73, 17);
            this.CHK_P8.TabIndex = 10;
            this.CHK_P8.Text = "Partition 8";
            this.CHK_P8.UseVisualStyleBackColor = true;
            // 
            // CHK_P9
            // 
            this.CHK_P9.AutoSize = true;
            this.CHK_P9.Enabled = false;
            this.CHK_P9.Location = new System.Drawing.Point(159, 176);
            this.CHK_P9.Name = "CHK_P9";
            this.CHK_P9.Size = new System.Drawing.Size(73, 17);
            this.CHK_P9.TabIndex = 11;
            this.CHK_P9.Text = "Partition 9";
            this.CHK_P9.UseVisualStyleBackColor = true;
            // 
            // CHK_P10
            // 
            this.CHK_P10.AutoSize = true;
            this.CHK_P10.Enabled = false;
            this.CHK_P10.Location = new System.Drawing.Point(159, 193);
            this.CHK_P10.Name = "CHK_P10";
            this.CHK_P10.Size = new System.Drawing.Size(74, 17);
            this.CHK_P10.TabIndex = 12;
            this.CHK_P10.Text = "Partition A";
            this.CHK_P10.UseVisualStyleBackColor = true;
            // 
            // CHK_L7
            // 
            this.CHK_L7.AutoSize = true;
            this.CHK_L7.Enabled = false;
            this.CHK_L7.Location = new System.Drawing.Point(262, 142);
            this.CHK_L7.Name = "CHK_L7";
            this.CHK_L7.Size = new System.Drawing.Size(83, 17);
            this.CHK_L7.TabIndex = 19;
            this.CHK_L7.Text = "Language 7";
            this.CHK_L7.UseVisualStyleBackColor = true;
            // 
            // CHK_L6
            // 
            this.CHK_L6.AutoSize = true;
            this.CHK_L6.Enabled = false;
            this.CHK_L6.Location = new System.Drawing.Point(262, 125);
            this.CHK_L6.Name = "CHK_L6";
            this.CHK_L6.Size = new System.Drawing.Size(83, 17);
            this.CHK_L6.TabIndex = 18;
            this.CHK_L6.Text = "Language 6";
            this.CHK_L6.UseVisualStyleBackColor = true;
            // 
            // CHK_L5
            // 
            this.CHK_L5.AutoSize = true;
            this.CHK_L5.Enabled = false;
            this.CHK_L5.Location = new System.Drawing.Point(262, 108);
            this.CHK_L5.Name = "CHK_L5";
            this.CHK_L5.Size = new System.Drawing.Size(83, 17);
            this.CHK_L5.TabIndex = 17;
            this.CHK_L5.Text = "Language 5";
            this.CHK_L5.UseVisualStyleBackColor = true;
            // 
            // CHK_L4
            // 
            this.CHK_L4.AutoSize = true;
            this.CHK_L4.Enabled = false;
            this.CHK_L4.Location = new System.Drawing.Point(262, 91);
            this.CHK_L4.Name = "CHK_L4";
            this.CHK_L4.Size = new System.Drawing.Size(83, 17);
            this.CHK_L4.TabIndex = 16;
            this.CHK_L4.Text = "Language 4";
            this.CHK_L4.UseVisualStyleBackColor = true;
            // 
            // CHK_L3
            // 
            this.CHK_L3.AutoSize = true;
            this.CHK_L3.Enabled = false;
            this.CHK_L3.Location = new System.Drawing.Point(262, 74);
            this.CHK_L3.Name = "CHK_L3";
            this.CHK_L3.Size = new System.Drawing.Size(83, 17);
            this.CHK_L3.TabIndex = 15;
            this.CHK_L3.Text = "Language 3";
            this.CHK_L3.UseVisualStyleBackColor = true;
            // 
            // CHK_L2
            // 
            this.CHK_L2.AutoSize = true;
            this.CHK_L2.Enabled = false;
            this.CHK_L2.Location = new System.Drawing.Point(262, 57);
            this.CHK_L2.Name = "CHK_L2";
            this.CHK_L2.Size = new System.Drawing.Size(83, 17);
            this.CHK_L2.TabIndex = 14;
            this.CHK_L2.Text = "Language 2";
            this.CHK_L2.UseVisualStyleBackColor = true;
            // 
            // CHK_L1
            // 
            this.CHK_L1.AutoSize = true;
            this.CHK_L1.Enabled = false;
            this.CHK_L1.Location = new System.Drawing.Point(262, 40);
            this.CHK_L1.Name = "CHK_L1";
            this.CHK_L1.Size = new System.Drawing.Size(83, 17);
            this.CHK_L1.TabIndex = 13;
            this.CHK_L1.Text = "Language 1";
            this.CHK_L1.UseVisualStyleBackColor = true;
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
            this.B_GiveAll.Enabled = false;
            this.B_GiveAll.Location = new System.Drawing.Point(157, 11);
            this.B_GiveAll.Name = "B_GiveAll";
            this.B_GiveAll.Size = new System.Drawing.Size(80, 23);
            this.B_GiveAll.TabIndex = 23;
            this.B_GiveAll.Text = "Give All";
            this.B_GiveAll.UseVisualStyleBackColor = true;
            // 
            // B_Save
            // 
            this.B_Save.Enabled = false;
            this.B_Save.Location = new System.Drawing.Point(262, 165);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(80, 23);
            this.B_Save.TabIndex = 24;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            // 
            // B_FillDex
            // 
            this.B_FillDex.Enabled = false;
            this.B_FillDex.Location = new System.Drawing.Point(262, 11);
            this.B_FillDex.Name = "B_FillDex";
            this.B_FillDex.Size = new System.Drawing.Size(80, 23);
            this.B_FillDex.TabIndex = 25;
            this.B_FillDex.Text = "Fill Dex";
            this.B_FillDex.UseVisualStyleBackColor = true;
            // 
            // SAV_Pokedex
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 222);
            this.Controls.Add(this.L_Beta);
            this.Controls.Add(this.B_FillDex);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_GiveAll);
            this.Controls.Add(this.CB_Species);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CHK_L7);
            this.Controls.Add(this.CHK_L6);
            this.Controls.Add(this.CHK_L5);
            this.Controls.Add(this.CHK_L4);
            this.Controls.Add(this.CHK_L3);
            this.Controls.Add(this.CHK_L2);
            this.Controls.Add(this.CHK_L1);
            this.Controls.Add(this.CHK_P10);
            this.Controls.Add(this.CHK_P9);
            this.Controls.Add(this.CHK_P8);
            this.Controls.Add(this.CHK_P7);
            this.Controls.Add(this.CHK_P6);
            this.Controls.Add(this.CHK_P5);
            this.Controls.Add(this.CHK_P4);
            this.Controls.Add(this.CHK_P3);
            this.Controls.Add(this.CHK_P2);
            this.Controls.Add(this.CHK_P1);
            this.Controls.Add(this.LB_Species);
            this.Controls.Add(this.B_Cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SAV_Pokedex";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Pokédex Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Label L_Beta;
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
    }
}