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
            this.B_Cancel = new System.Windows.Forms.Button();
            this.B_Save = new System.Windows.Forms.Button();
            this.PG_Raid = new System.Windows.Forms.PropertyGrid();
            this.CB_Raid = new System.Windows.Forms.ComboBox();
            this.TB_SeedTomorrow = new System.Windows.Forms.TextBox();
            this.TB_SeedToday = new System.Windows.Forms.TextBox();
            this.L_SeedCurrent = new System.Windows.Forms.Label();
            this.L_SeedTomorrow = new System.Windows.Forms.Label();
            this.B_Star6 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(280, 336);
            this.B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(96, 24);
            this.B_Cancel.TabIndex = 0;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(280, 304);
            this.B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(96, 24);
            this.B_Save.TabIndex = 1;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // PG_Raid
            // 
            this.PG_Raid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PG_Raid.Location = new System.Drawing.Point(8, 40);
            this.PG_Raid.Margin = new System.Windows.Forms.Padding(0);
            this.PG_Raid.Name = "PG_Raid";
            this.PG_Raid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.PG_Raid.Size = new System.Drawing.Size(368, 256);
            this.PG_Raid.TabIndex = 2;
            this.PG_Raid.ToolbarVisible = false;
            // 
            // CB_Raid
            // 
            this.CB_Raid.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Raid.FormattingEnabled = true;
            this.CB_Raid.Location = new System.Drawing.Point(8, 8);
            this.CB_Raid.Margin = new System.Windows.Forms.Padding(0);
            this.CB_Raid.Name = "CB_Raid";
            this.CB_Raid.Size = new System.Drawing.Size(120, 23);
            this.CB_Raid.TabIndex = 3;
            this.CB_Raid.SelectedIndexChanged += new System.EventHandler(this.CB_Raid_SelectedIndexChanged);
            // 
            // TB_SeedTomorrow
            // 
            this.TB_SeedTomorrow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TB_SeedTomorrow.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TB_SeedTomorrow.Location = new System.Drawing.Point(120, 336);
            this.TB_SeedTomorrow.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.TB_SeedTomorrow.MaxLength = 16;
            this.TB_SeedTomorrow.Name = "TB_SeedTomorrow";
            this.TB_SeedTomorrow.Size = new System.Drawing.Size(124, 20);
            this.TB_SeedTomorrow.TabIndex = 17;
            this.TB_SeedTomorrow.Text = "0000000000000000";
            // 
            // TB_SeedToday
            // 
            this.TB_SeedToday.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TB_SeedToday.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TB_SeedToday.Location = new System.Drawing.Point(120, 304);
            this.TB_SeedToday.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.TB_SeedToday.MaxLength = 16;
            this.TB_SeedToday.Name = "TB_SeedToday";
            this.TB_SeedToday.Size = new System.Drawing.Size(124, 20);
            this.TB_SeedToday.TabIndex = 16;
            this.TB_SeedToday.Text = "0000000000000000";
            // 
            // L_SeedCurrent
            // 
            this.L_SeedCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.L_SeedCurrent.Location = new System.Drawing.Point(8, 304);
            this.L_SeedCurrent.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.L_SeedCurrent.Name = "L_SeedCurrent";
            this.L_SeedCurrent.Size = new System.Drawing.Size(97, 23);
            this.L_SeedCurrent.TabIndex = 18;
            this.L_SeedCurrent.Text = "Current Seed:";
            this.L_SeedCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_SeedTomorrow
            // 
            this.L_SeedTomorrow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.L_SeedTomorrow.Location = new System.Drawing.Point(8, 336);
            this.L_SeedTomorrow.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.L_SeedTomorrow.Name = "L_SeedTomorrow";
            this.L_SeedTomorrow.Size = new System.Drawing.Size(97, 23);
            this.L_SeedTomorrow.TabIndex = 19;
            this.L_SeedTomorrow.Text = "Tomorrow:";
            this.L_SeedTomorrow.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // B_Star6
            // 
            this.B_Star6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Star6.Location = new System.Drawing.Point(152, 8);
            this.B_Star6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.B_Star6.Name = "B_Star6";
            this.B_Star6.Size = new System.Drawing.Size(224, 24);
            this.B_Star6.TabIndex = 20;
            this.B_Star6.Text = "Copy to Other Raids";
            this.B_Star6.UseVisualStyleBackColor = true;
            this.B_Star6.Click += new System.EventHandler(this.B_CopyToOthers_Click);
            // 
            // SAV_Raid9
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 370);
            this.Controls.Add(this.B_Star6);
            this.Controls.Add(this.L_SeedTomorrow);
            this.Controls.Add(this.L_SeedCurrent);
            this.Controls.Add(this.TB_SeedTomorrow);
            this.Controls.Add(this.TB_SeedToday);
            this.Controls.Add(this.CB_Raid);
            this.Controls.Add(this.PG_Raid);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_Cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(382, 409);
            this.Name = "SAV_Raid9";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Raid Parameter Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.Button B_Star6;
    }
}
