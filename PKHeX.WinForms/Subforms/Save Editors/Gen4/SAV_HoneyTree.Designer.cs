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
            this.B_Save = new System.Windows.Forms.Button();
            this.B_Cancel = new System.Windows.Forms.Button();
            this.CB_TreeList = new System.Windows.Forms.ComboBox();
            this.L_HoneyTree = new System.Windows.Forms.Label();
            this.L_Slot = new System.Windows.Forms.Label();
            this.L_Time = new System.Windows.Forms.Label();
            this.L_Shake = new System.Windows.Forms.Label();
            this.NUD_Time = new System.Windows.Forms.NumericUpDown();
            this.NUD_Shake = new System.Windows.Forms.NumericUpDown();
            this.L_Munchlax = new System.Windows.Forms.Label();
            this.L_Tree0 = new System.Windows.Forms.Label();
            this.B_Catchable = new System.Windows.Forms.Button();
            this.NUD_Group = new System.Windows.Forms.NumericUpDown();
            this.L_Group = new System.Windows.Forms.Label();
            this.NUD_Slot = new System.Windows.Forms.NumericUpDown();
            this.L_Species = new System.Windows.Forms.Label();
            this.GB_TreeInfo = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Time)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Shake)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Group)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Slot)).BeginInit();
            this.GB_TreeInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(314, 181);
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
            this.B_Cancel.Location = new System.Drawing.Point(233, 181);
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
            this.CB_TreeList.SelectedIndexChanged += new System.EventHandler(this.ChangeTree);
            // 
            // L_HoneyTree
            // 
            this.L_HoneyTree.Location = new System.Drawing.Point(7, 22);
            this.L_HoneyTree.Name = "L_HoneyTree";
            this.L_HoneyTree.Size = new System.Drawing.Size(186, 13);
            this.L_HoneyTree.TabIndex = 75;
            this.L_HoneyTree.Text = "Honey Tree";
            this.L_HoneyTree.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // L_Slot
            // 
            this.L_Slot.AutoSize = true;
            this.L_Slot.Location = new System.Drawing.Point(48, 16);
            this.L_Slot.Name = "L_Slot";
            this.L_Slot.Size = new System.Drawing.Size(25, 13);
            this.L_Slot.TabIndex = 76;
            this.L_Slot.Text = "Slot";
            // 
            // L_Time
            // 
            this.L_Time.AutoSize = true;
            this.L_Time.Location = new System.Drawing.Point(11, 67);
            this.L_Time.Name = "L_Time";
            this.L_Time.Size = new System.Drawing.Size(92, 13);
            this.L_Time.TabIndex = 77;
            this.L_Time.Text = "Time left (minutes)";
            // 
            // L_Shake
            // 
            this.L_Shake.AutoSize = true;
            this.L_Shake.Location = new System.Drawing.Point(116, 67);
            this.L_Shake.Name = "L_Shake";
            this.L_Shake.Size = new System.Drawing.Size(38, 13);
            this.L_Shake.TabIndex = 79;
            this.L_Shake.Text = "Shake";
            // 
            // NUD_Time
            // 
            this.NUD_Time.Location = new System.Drawing.Point(11, 83);
            this.NUD_Time.Maximum = new decimal(new int[] {
            1440,
            0,
            0,
            0});
            this.NUD_Time.Name = "NUD_Time";
            this.NUD_Time.Size = new System.Drawing.Size(102, 20);
            this.NUD_Time.TabIndex = 80;
            // 
            // NUD_Shake
            // 
            this.NUD_Shake.Location = new System.Drawing.Point(119, 83);
            this.NUD_Shake.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.NUD_Shake.Name = "NUD_Shake";
            this.NUD_Shake.Size = new System.Drawing.Size(36, 20);
            this.NUD_Shake.TabIndex = 81;
            // 
            // L_Munchlax
            // 
            this.L_Munchlax.AutoSize = true;
            this.L_Munchlax.Location = new System.Drawing.Point(199, 22);
            this.L_Munchlax.Name = "L_Munchlax";
            this.L_Munchlax.Size = new System.Drawing.Size(86, 13);
            this.L_Munchlax.TabIndex = 82;
            this.L_Munchlax.Text = "Munchlax Trees:";
            // 
            // L_Tree0
            // 
            this.L_Tree0.AutoSize = true;
            this.L_Tree0.Location = new System.Drawing.Point(199, 39);
            this.L_Tree0.Name = "L_Tree0";
            this.L_Tree0.Size = new System.Drawing.Size(164, 13);
            this.L_Tree0.TabIndex = 83;
            this.L_Tree0.Text = "- Route 205, Floaroma Town side";
            // 
            // B_Catchable
            // 
            this.B_Catchable.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.B_Catchable.Location = new System.Drawing.Point(11, 109);
            this.B_Catchable.Name = "B_Catchable";
            this.B_Catchable.Size = new System.Drawing.Size(100, 23);
            this.B_Catchable.TabIndex = 87;
            this.B_Catchable.Text = "Make catchable";
            this.B_Catchable.UseVisualStyleBackColor = true;
            this.B_Catchable.Click += new System.EventHandler(this.B_Catchable_Click);
            // 
            // NUD_Group
            // 
            this.NUD_Group.Location = new System.Drawing.Point(9, 32);
            this.NUD_Group.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.NUD_Group.Name = "NUD_Group";
            this.NUD_Group.Size = new System.Drawing.Size(36, 20);
            this.NUD_Group.TabIndex = 88;
            this.NUD_Group.ValueChanged += new System.EventHandler(this.ChangeGroupSlot);
            // 
            // L_Group
            // 
            this.L_Group.AutoSize = true;
            this.L_Group.Location = new System.Drawing.Point(6, 16);
            this.L_Group.Name = "L_Group";
            this.L_Group.Size = new System.Drawing.Size(36, 13);
            this.L_Group.TabIndex = 89;
            this.L_Group.Text = "Group";
            // 
            // NUD_Slot
            // 
            this.NUD_Slot.Location = new System.Drawing.Point(51, 32);
            this.NUD_Slot.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.NUD_Slot.Name = "NUD_Slot";
            this.NUD_Slot.Size = new System.Drawing.Size(36, 20);
            this.NUD_Slot.TabIndex = 90;
            this.NUD_Slot.ValueChanged += new System.EventHandler(this.ChangeGroupSlot);
            // 
            // L_Species
            // 
            this.L_Species.Location = new System.Drawing.Point(93, 16);
            this.L_Species.Name = "L_Species";
            this.L_Species.Size = new System.Drawing.Size(88, 51);
            this.L_Species.TabIndex = 91;
            this.L_Species.Text = "Species";
            this.L_Species.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GB_TreeInfo
            // 
            this.GB_TreeInfo.Controls.Add(this.L_Group);
            this.GB_TreeInfo.Controls.Add(this.L_Species);
            this.GB_TreeInfo.Controls.Add(this.L_Slot);
            this.GB_TreeInfo.Controls.Add(this.NUD_Slot);
            this.GB_TreeInfo.Controls.Add(this.L_Time);
            this.GB_TreeInfo.Controls.Add(this.L_Shake);
            this.GB_TreeInfo.Controls.Add(this.NUD_Group);
            this.GB_TreeInfo.Controls.Add(this.NUD_Time);
            this.GB_TreeInfo.Controls.Add(this.B_Catchable);
            this.GB_TreeInfo.Controls.Add(this.NUD_Shake);
            this.GB_TreeInfo.Location = new System.Drawing.Point(12, 63);
            this.GB_TreeInfo.Name = "GB_TreeInfo";
            this.GB_TreeInfo.Size = new System.Drawing.Size(185, 141);
            this.GB_TreeInfo.TabIndex = 92;
            this.GB_TreeInfo.TabStop = false;
            this.GB_TreeInfo.Text = "Tree Info";
            // 
            // SAV_HoneyTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 216);
            this.Controls.Add(this.GB_TreeInfo);
            this.Controls.Add(this.L_Tree0);
            this.Controls.Add(this.L_Munchlax);
            this.Controls.Add(this.L_HoneyTree);
            this.Controls.Add(this.CB_TreeList);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_Cancel);
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(200, 150);
            this.Name = "SAV_HoneyTree";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Honey Tree Editor";
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Time)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Shake)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Group)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Slot)).EndInit();
            this.GB_TreeInfo.ResumeLayout(false);
            this.GB_TreeInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.ComboBox CB_TreeList;
        private System.Windows.Forms.Label L_HoneyTree;
        private System.Windows.Forms.Label L_Slot;
        private System.Windows.Forms.Label L_Time;
        private System.Windows.Forms.Label L_Shake;
        private System.Windows.Forms.NumericUpDown NUD_Time;
        private System.Windows.Forms.NumericUpDown NUD_Shake;
        private System.Windows.Forms.Label L_Munchlax;
        private System.Windows.Forms.Label L_Tree0;
        private System.Windows.Forms.Button B_Catchable;
        private System.Windows.Forms.NumericUpDown NUD_Group;
        private System.Windows.Forms.Label L_Group;
        private System.Windows.Forms.NumericUpDown NUD_Slot;
        private System.Windows.Forms.Label L_Species;
        private System.Windows.Forms.GroupBox GB_TreeInfo;
    }
}