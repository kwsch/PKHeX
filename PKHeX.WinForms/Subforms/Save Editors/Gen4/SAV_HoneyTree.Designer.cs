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
            B_Save = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            CB_TreeList = new System.Windows.Forms.ComboBox();
            L_HoneyTree = new System.Windows.Forms.Label();
            L_Slot = new System.Windows.Forms.Label();
            L_Time = new System.Windows.Forms.Label();
            L_Shake = new System.Windows.Forms.Label();
            NUD_Time = new System.Windows.Forms.NumericUpDown();
            NUD_Shake = new System.Windows.Forms.NumericUpDown();
            L_Munchlax = new System.Windows.Forms.Label();
            L_Tree0 = new System.Windows.Forms.Label();
            B_Catchable = new System.Windows.Forms.Button();
            NUD_Group = new System.Windows.Forms.NumericUpDown();
            L_Group = new System.Windows.Forms.Label();
            NUD_Slot = new System.Windows.Forms.NumericUpDown();
            L_Species = new System.Windows.Forms.Label();
            GB_TreeInfo = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)NUD_Time).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Shake).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Group).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Slot).BeginInit();
            GB_TreeInfo.SuspendLayout();
            SuspendLayout();
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(366, 209);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(88, 27);
            B_Save.TabIndex = 73;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(272, 209);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(88, 27);
            B_Cancel.TabIndex = 72;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // CB_TreeList
            // 
            CB_TreeList.FormattingEnabled = true;
            CB_TreeList.Items.AddRange(new object[] { "Route 205, Floaroma Town side", "Route 205, Eterna City side", "Route 206", "Route 207", "Route 208", "Route 209", "Route 210, Solaceon Town side", "Route 210, Celestic Town side", "Route 211", "Route 212, Hearthome City side", "Route 212, Pastoria City side", "Route 213", "Route 214", "Route 215", "Route 218", "Route 221", "Route 222", "Valley Windworks", "Eterna Forest", "Fuego Ironworks", "Floaroma Meadow" });
            CB_TreeList.Location = new System.Drawing.Point(12, 42);
            CB_TreeList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_TreeList.Name = "CB_TreeList";
            CB_TreeList.Size = new System.Drawing.Size(213, 23);
            CB_TreeList.TabIndex = 74;
            CB_TreeList.SelectedIndexChanged += ChangeTree;
            // 
            // L_HoneyTree
            // 
            L_HoneyTree.Location = new System.Drawing.Point(8, 25);
            L_HoneyTree.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_HoneyTree.Name = "L_HoneyTree";
            L_HoneyTree.Size = new System.Drawing.Size(217, 15);
            L_HoneyTree.TabIndex = 75;
            L_HoneyTree.Text = "Honey Tree";
            L_HoneyTree.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // L_Slot
            // 
            L_Slot.AutoSize = true;
            L_Slot.Location = new System.Drawing.Point(56, 18);
            L_Slot.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Slot.Name = "L_Slot";
            L_Slot.Size = new System.Drawing.Size(27, 15);
            L_Slot.TabIndex = 76;
            L_Slot.Text = "Slot";
            // 
            // L_Time
            // 
            L_Time.AutoSize = true;
            L_Time.Location = new System.Drawing.Point(13, 77);
            L_Time.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Time.Name = "L_Time";
            L_Time.Size = new System.Drawing.Size(107, 15);
            L_Time.TabIndex = 77;
            L_Time.Text = "Time left (minutes)";
            // 
            // L_Shake
            // 
            L_Shake.AutoSize = true;
            L_Shake.Location = new System.Drawing.Point(135, 77);
            L_Shake.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Shake.Name = "L_Shake";
            L_Shake.Size = new System.Drawing.Size(38, 15);
            L_Shake.TabIndex = 79;
            L_Shake.Text = "Shake";
            // 
            // NUD_Time
            // 
            NUD_Time.Location = new System.Drawing.Point(13, 96);
            NUD_Time.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_Time.Maximum = new decimal(new int[] { 1440, 0, 0, 0 });
            NUD_Time.Name = "NUD_Time";
            NUD_Time.Size = new System.Drawing.Size(119, 23);
            NUD_Time.TabIndex = 80;
            // 
            // NUD_Shake
            // 
            NUD_Shake.Location = new System.Drawing.Point(139, 96);
            NUD_Shake.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_Shake.Maximum = new decimal(new int[] { 3, 0, 0, 0 });
            NUD_Shake.Name = "NUD_Shake";
            NUD_Shake.Size = new System.Drawing.Size(42, 23);
            NUD_Shake.TabIndex = 81;
            // 
            // L_Munchlax
            // 
            L_Munchlax.AutoSize = true;
            L_Munchlax.Location = new System.Drawing.Point(232, 25);
            L_Munchlax.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Munchlax.Name = "L_Munchlax";
            L_Munchlax.Size = new System.Drawing.Size(92, 15);
            L_Munchlax.TabIndex = 82;
            L_Munchlax.Text = "Munchlax Trees:";
            // 
            // L_Tree0
            // 
            L_Tree0.AutoSize = true;
            L_Tree0.Location = new System.Drawing.Point(232, 45);
            L_Tree0.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Tree0.Name = "L_Tree0";
            L_Tree0.Size = new System.Drawing.Size(178, 15);
            L_Tree0.TabIndex = 83;
            L_Tree0.Text = "- Route 205, Floaroma Town side";
            // 
            // B_Catchable
            // 
            B_Catchable.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            B_Catchable.Location = new System.Drawing.Point(13, 126);
            B_Catchable.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Catchable.Name = "B_Catchable";
            B_Catchable.Size = new System.Drawing.Size(117, 27);
            B_Catchable.TabIndex = 87;
            B_Catchable.Text = "Make catchable";
            B_Catchable.UseVisualStyleBackColor = true;
            B_Catchable.Click += B_Catchable_Click;
            // 
            // NUD_Group
            // 
            NUD_Group.Location = new System.Drawing.Point(10, 37);
            NUD_Group.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_Group.Maximum = new decimal(new int[] { 3, 0, 0, 0 });
            NUD_Group.Name = "NUD_Group";
            NUD_Group.Size = new System.Drawing.Size(42, 23);
            NUD_Group.TabIndex = 88;
            NUD_Group.ValueChanged += ChangeGroupSlot;
            // 
            // L_Group
            // 
            L_Group.AutoSize = true;
            L_Group.Location = new System.Drawing.Point(7, 18);
            L_Group.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Group.Name = "L_Group";
            L_Group.Size = new System.Drawing.Size(40, 15);
            L_Group.TabIndex = 89;
            L_Group.Text = "Group";
            // 
            // NUD_Slot
            // 
            NUD_Slot.Location = new System.Drawing.Point(59, 37);
            NUD_Slot.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_Slot.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            NUD_Slot.Name = "NUD_Slot";
            NUD_Slot.Size = new System.Drawing.Size(42, 23);
            NUD_Slot.TabIndex = 90;
            NUD_Slot.ValueChanged += ChangeGroupSlot;
            // 
            // L_Species
            // 
            L_Species.Location = new System.Drawing.Point(108, 18);
            L_Species.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Species.Name = "L_Species";
            L_Species.Size = new System.Drawing.Size(103, 59);
            L_Species.TabIndex = 91;
            L_Species.Text = "Species";
            L_Species.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GB_TreeInfo
            // 
            GB_TreeInfo.Controls.Add(L_Group);
            GB_TreeInfo.Controls.Add(L_Species);
            GB_TreeInfo.Controls.Add(L_Slot);
            GB_TreeInfo.Controls.Add(NUD_Slot);
            GB_TreeInfo.Controls.Add(L_Time);
            GB_TreeInfo.Controls.Add(L_Shake);
            GB_TreeInfo.Controls.Add(NUD_Group);
            GB_TreeInfo.Controls.Add(NUD_Time);
            GB_TreeInfo.Controls.Add(B_Catchable);
            GB_TreeInfo.Controls.Add(NUD_Shake);
            GB_TreeInfo.Location = new System.Drawing.Point(14, 73);
            GB_TreeInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_TreeInfo.Name = "GB_TreeInfo";
            GB_TreeInfo.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_TreeInfo.Size = new System.Drawing.Size(216, 163);
            GB_TreeInfo.TabIndex = 92;
            GB_TreeInfo.TabStop = false;
            GB_TreeInfo.Text = "Tree Info";
            // 
            // SAV_HoneyTree
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(465, 249);
            Controls.Add(GB_TreeInfo);
            Controls.Add(L_Tree0);
            Controls.Add(L_Munchlax);
            Controls.Add(L_HoneyTree);
            Controls.Add(CB_TreeList);
            Controls.Add(B_Save);
            Controls.Add(B_Cancel);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(231, 167);
            Name = "SAV_HoneyTree";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Honey Tree Editor";
            ((System.ComponentModel.ISupportInitialize)NUD_Time).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Shake).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Group).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Slot).EndInit();
            GB_TreeInfo.ResumeLayout(false);
            GB_TreeInfo.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
