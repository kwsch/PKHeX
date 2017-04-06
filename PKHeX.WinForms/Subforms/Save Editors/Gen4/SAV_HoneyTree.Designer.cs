namespace PKHeX.WinForms
{
    partial class SAV_HoneyTree
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SAV_HoneyTree));
            this.B_Save = new System.Windows.Forms.Button();
            this.B_Cancel = new System.Windows.Forms.Button();
            this.CB_TreeList = new System.Windows.Forms.ComboBox();
            this.L_HoneyTree = new System.Windows.Forms.Label();
            this.L_Pokemon = new System.Windows.Forms.Label();
            this.L_Time = new System.Windows.Forms.Label();
            this.CB_Species = new System.Windows.Forms.ComboBox();
            this.L_Shake = new System.Windows.Forms.Label();
            this.NUD_Time = new System.Windows.Forms.NumericUpDown();
            this.NUD_Shake = new System.Windows.Forms.NumericUpDown();
            this.L_Munchlax = new System.Windows.Forms.Label();
            this.L_Tree0 = new System.Windows.Forms.Label();
            this.L_Tree1 = new System.Windows.Forms.Label();
            this.L_Tree2 = new System.Windows.Forms.Label();
            this.L_Tree3 = new System.Windows.Forms.Label();
            this.B_Catchable = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Time)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Shake)).BeginInit();
            this.SuspendLayout();
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(402, 98);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(75, 23);
            this.B_Save.TabIndex = 73;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(321, 98);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(75, 23);
            this.B_Cancel.TabIndex = 72;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // CB_TreeList
            // 
            this.CB_TreeList.FormattingEnabled = true;
            this.CB_TreeList.Items.AddRange(new object[] {
            "Route 205, Floaroma Town side",
            "Route 205, Eterna City side",
            "Route 206",
            "Route 207",
            "Route 208",
            "Route 209",
            "Route 210, Solaceon Town side",
            "Route 210, Celestic Town side",
            "Route 211",
            "Route 212, Hearthome City side",
            "Route 212, Pastoria City side",
            "Route 213",
            "Route 214",
            "Route 215",
            "Route 218",
            "Route 221",
            "Route 222",
            "Valley Windworks",
            "Eterna Forest",
            "Fuego Ironworks",
            "Floaroma Meadow"});
            this.CB_TreeList.Location = new System.Drawing.Point(10, 36);
            this.CB_TreeList.Name = "CB_TreeList";
            this.CB_TreeList.Size = new System.Drawing.Size(183, 21);
            this.CB_TreeList.TabIndex = 74;
            this.CB_TreeList.SelectedIndexChanged += new System.EventHandler(this.CB_TreeList_SelectedIndexChanged);
            // 
            // L_HoneyTree
            // 
            this.L_HoneyTree.AutoSize = true;
            this.L_HoneyTree.Location = new System.Drawing.Point(74, 20);
            this.L_HoneyTree.Name = "L_HoneyTree";
            this.L_HoneyTree.Size = new System.Drawing.Size(63, 13);
            this.L_HoneyTree.TabIndex = 75;
            this.L_HoneyTree.Text = "Honey Tree";
            // 
            // L_Pokemon
            // 
            this.L_Pokemon.AutoSize = true;
            this.L_Pokemon.Location = new System.Drawing.Point(234, 20);
            this.L_Pokemon.Name = "L_Pokemon";
            this.L_Pokemon.Size = new System.Drawing.Size(52, 13);
            this.L_Pokemon.TabIndex = 76;
            this.L_Pokemon.Text = "Pokémon";
            // 
            // L_Time
            // 
            this.L_Time.AutoSize = true;
            this.L_Time.Location = new System.Drawing.Point(336, 20);
            this.L_Time.Name = "L_Time";
            this.L_Time.Size = new System.Drawing.Size(92, 13);
            this.L_Time.TabIndex = 77;
            this.L_Time.Text = "Time left (minutes)";
            // 
            // CB_Species
            // 
            this.CB_Species.FormattingEnabled = true;
            this.CB_Species.Items.AddRange(new object[] {
            "None",
            "Aipom",
            "Burmy",
            "Cherubi",
            "Combee",
            "Heracross",
            "Munchlax",
            "Silcoon/Cascoon",
            "Wurmple"});
            this.CB_Species.Location = new System.Drawing.Point(199, 36);
            this.CB_Species.Name = "CB_Species";
            this.CB_Species.Size = new System.Drawing.Size(123, 21);
            this.CB_Species.TabIndex = 78;
            this.CB_Species.SelectedIndexChanged += new System.EventHandler(this.CB_Species_SelectedIndexChanged);
            // 
            // L_Shake
            // 
            this.L_Shake.AutoSize = true;
            this.L_Shake.Location = new System.Drawing.Point(436, 21);
            this.L_Shake.Name = "L_Shake";
            this.L_Shake.Size = new System.Drawing.Size(38, 13);
            this.L_Shake.TabIndex = 79;
            this.L_Shake.Text = "Shake";
            // 
            // NUD_Time
            // 
            this.NUD_Time.Location = new System.Drawing.Point(328, 37);
            this.NUD_Time.Maximum = new decimal(new int[] {
            1440,
            0,
            0,
            0});
            this.NUD_Time.Name = "NUD_Time";
            this.NUD_Time.Size = new System.Drawing.Size(102, 20);
            this.NUD_Time.TabIndex = 80;
            this.NUD_Time.ValueChanged += new System.EventHandler(this.NUD_Time_ValueChanged);
            // 
            // NUD_Shake
            // 
            this.NUD_Shake.Location = new System.Drawing.Point(436, 37);
            this.NUD_Shake.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.NUD_Shake.Name = "NUD_Shake";
            this.NUD_Shake.Size = new System.Drawing.Size(36, 20);
            this.NUD_Shake.TabIndex = 81;
            this.NUD_Shake.ValueChanged += new System.EventHandler(this.NUD_Shake_ValueChanged);
            // 
            // L_Munchlax
            // 
            this.L_Munchlax.AutoSize = true;
            this.L_Munchlax.Location = new System.Drawing.Point(7, 60);
            this.L_Munchlax.Name = "L_Munchlax";
            this.L_Munchlax.Size = new System.Drawing.Size(82, 13);
            this.L_Munchlax.TabIndex = 82;
            this.L_Munchlax.Text = "Munchlax trees:";
            // 
            // L_Tree0
            // 
            this.L_Tree0.AutoSize = true;
            this.L_Tree0.Location = new System.Drawing.Point(12, 73);
            this.L_Tree0.Name = "L_Tree0";
            this.L_Tree0.Size = new System.Drawing.Size(164, 13);
            this.L_Tree0.TabIndex = 83;
            this.L_Tree0.Text = "- Route 205, Floaroma Town side";
            // 
            // L_Tree1
            // 
            this.L_Tree1.AutoSize = true;
            this.L_Tree1.Location = new System.Drawing.Point(12, 86);
            this.L_Tree1.Name = "L_Tree1";
            this.L_Tree1.Size = new System.Drawing.Size(164, 13);
            this.L_Tree1.TabIndex = 84;
            this.L_Tree1.Text = "- Route 205, Floaroma Town side";
            // 
            // L_Tree2
            // 
            this.L_Tree2.AutoSize = true;
            this.L_Tree2.Location = new System.Drawing.Point(12, 99);
            this.L_Tree2.Name = "L_Tree2";
            this.L_Tree2.Size = new System.Drawing.Size(164, 13);
            this.L_Tree2.TabIndex = 85;
            this.L_Tree2.Text = "- Route 205, Floaroma Town side";
            // 
            // L_Tree3
            // 
            this.L_Tree3.AutoSize = true;
            this.L_Tree3.Location = new System.Drawing.Point(12, 112);
            this.L_Tree3.Name = "L_Tree3";
            this.L_Tree3.Size = new System.Drawing.Size(164, 13);
            this.L_Tree3.TabIndex = 86;
            this.L_Tree3.Text = "- Route 205, Floaroma Town side";
            // 
            // B_Catchable
            // 
            this.B_Catchable.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.B_Catchable.Location = new System.Drawing.Point(328, 63);
            this.B_Catchable.Name = "B_Catchable";
            this.B_Catchable.Size = new System.Drawing.Size(100, 20);
            this.B_Catchable.TabIndex = 87;
            this.B_Catchable.Text = "Make catchable";
            this.B_Catchable.UseVisualStyleBackColor = true;
            this.B_Catchable.Click += new System.EventHandler(this.B_Catchable_Click);
            // 
            // SAV_HoneyTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 133);
            this.Controls.Add(this.B_Catchable);
            this.Controls.Add(this.L_Tree3);
            this.Controls.Add(this.L_Tree2);
            this.Controls.Add(this.L_Tree1);
            this.Controls.Add(this.L_Tree0);
            this.Controls.Add(this.L_Munchlax);
            this.Controls.Add(this.NUD_Shake);
            this.Controls.Add(this.NUD_Time);
            this.Controls.Add(this.L_Shake);
            this.Controls.Add(this.CB_Species);
            this.Controls.Add(this.L_Time);
            this.Controls.Add(this.L_Pokemon);
            this.Controls.Add(this.L_HoneyTree);
            this.Controls.Add(this.CB_TreeList);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_Cancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(200, 150);
            this.Name = "SAV_HoneyTree";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Honey Tree Editor";
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Time)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Shake)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.ComboBox CB_TreeList;
        private System.Windows.Forms.Label L_HoneyTree;
        private System.Windows.Forms.Label L_Pokemon;
        private System.Windows.Forms.Label L_Time;
        private System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.Label L_Shake;
        private System.Windows.Forms.NumericUpDown NUD_Time;
        private System.Windows.Forms.NumericUpDown NUD_Shake;
        private System.Windows.Forms.Label L_Munchlax;
        private System.Windows.Forms.Label L_Tree0;
        private System.Windows.Forms.Label L_Tree1;
        private System.Windows.Forms.Label L_Tree2;
        private System.Windows.Forms.Label L_Tree3;
        private System.Windows.Forms.Button B_Catchable;
    }
}