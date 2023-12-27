namespace PKHeX.WinForms
{
    partial class SAV_Raid9
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
            PG_Raid = new System.Windows.Forms.PropertyGrid();
            CB_Raid = new System.Windows.Forms.ComboBox();
            TB_SeedTomorrow = new System.Windows.Forms.TextBox();
            TB_SeedToday = new System.Windows.Forms.TextBox();
            L_SeedCurrent = new System.Windows.Forms.Label();
            L_SeedTomorrow = new System.Windows.Forms.Label();
            B_CopyToOthers = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(280, 336);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(96, 24);
            B_Cancel.TabIndex = 0;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(280, 304);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(96, 24);
            B_Save.TabIndex = 1;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // PG_Raid
            // 
            PG_Raid.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            PG_Raid.Location = new System.Drawing.Point(8, 40);
            PG_Raid.Margin = new System.Windows.Forms.Padding(0);
            PG_Raid.Name = "PG_Raid";
            PG_Raid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            PG_Raid.Size = new System.Drawing.Size(368, 256);
            PG_Raid.TabIndex = 2;
            PG_Raid.ToolbarVisible = false;
            // 
            // CB_Raid
            // 
            CB_Raid.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Raid.FormattingEnabled = true;
            CB_Raid.Location = new System.Drawing.Point(8, 8);
            CB_Raid.Margin = new System.Windows.Forms.Padding(0);
            CB_Raid.Name = "CB_Raid";
            CB_Raid.Size = new System.Drawing.Size(120, 23);
            CB_Raid.TabIndex = 3;
            CB_Raid.SelectedIndexChanged += CB_Raid_SelectedIndexChanged;
            // 
            // TB_SeedTomorrow
            // 
            TB_SeedTomorrow.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            TB_SeedTomorrow.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TB_SeedTomorrow.Location = new System.Drawing.Point(120, 336);
            TB_SeedTomorrow.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_SeedTomorrow.MaxLength = 16;
            TB_SeedTomorrow.Name = "TB_SeedTomorrow";
            TB_SeedTomorrow.Size = new System.Drawing.Size(124, 20);
            TB_SeedTomorrow.TabIndex = 17;
            TB_SeedTomorrow.Text = "0000000000000000";
            // 
            // TB_SeedToday
            // 
            TB_SeedToday.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            TB_SeedToday.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TB_SeedToday.Location = new System.Drawing.Point(120, 304);
            TB_SeedToday.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_SeedToday.MaxLength = 16;
            TB_SeedToday.Name = "TB_SeedToday";
            TB_SeedToday.Size = new System.Drawing.Size(124, 20);
            TB_SeedToday.TabIndex = 16;
            TB_SeedToday.Text = "0000000000000000";
            // 
            // L_SeedCurrent
            // 
            L_SeedCurrent.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            L_SeedCurrent.Location = new System.Drawing.Point(8, 304);
            L_SeedCurrent.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_SeedCurrent.Name = "L_SeedCurrent";
            L_SeedCurrent.Size = new System.Drawing.Size(97, 23);
            L_SeedCurrent.TabIndex = 18;
            L_SeedCurrent.Text = "Current Seed:";
            L_SeedCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_SeedTomorrow
            // 
            L_SeedTomorrow.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            L_SeedTomorrow.Location = new System.Drawing.Point(8, 336);
            L_SeedTomorrow.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_SeedTomorrow.Name = "L_SeedTomorrow";
            L_SeedTomorrow.Size = new System.Drawing.Size(97, 23);
            L_SeedTomorrow.TabIndex = 19;
            L_SeedTomorrow.Text = "Tomorrow:";
            L_SeedTomorrow.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // B_CopyToOthers
            // 
            B_CopyToOthers.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            B_CopyToOthers.Location = new System.Drawing.Point(152, 8);
            B_CopyToOthers.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_CopyToOthers.Name = "B_CopyToOthers";
            B_CopyToOthers.Size = new System.Drawing.Size(224, 24);
            B_CopyToOthers.TabIndex = 20;
            B_CopyToOthers.Text = "Copy to Other Raids";
            B_CopyToOthers.UseVisualStyleBackColor = true;
            B_CopyToOthers.Click += B_CopyToOthers_Click;
            // 
            // SAV_Raid9
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(384, 370);
            Controls.Add(B_CopyToOthers);
            Controls.Add(L_SeedTomorrow);
            Controls.Add(L_SeedCurrent);
            Controls.Add(TB_SeedTomorrow);
            Controls.Add(TB_SeedToday);
            Controls.Add(CB_Raid);
            Controls.Add(PG_Raid);
            Controls.Add(B_Save);
            Controls.Add(B_Cancel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(382, 409);
            Name = "SAV_Raid9";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Raid Parameter Editor";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.PropertyGrid PG_Raid;
        private System.Windows.Forms.ComboBox CB_Raid;
        private System.Windows.Forms.TextBox TB_SeedTomorrow;
        private System.Windows.Forms.TextBox TB_SeedToday;
        private System.Windows.Forms.Label L_SeedCurrent;
        private System.Windows.Forms.Label L_SeedTomorrow;
        private System.Windows.Forms.Button B_CopyToOthers;
    }
}
