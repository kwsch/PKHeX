namespace PKHeX.WinForms
{
    partial class SAV_RaidSevenStar9
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
            this.SuspendLayout();
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(227, 216);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(75, 23);
            this.B_Cancel.TabIndex = 0;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(227, 190);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(75, 23);
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
            this.PG_Raid.Location = new System.Drawing.Point(12, 32);
            this.PG_Raid.Name = "PG_Raid";
            this.PG_Raid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.PG_Raid.Size = new System.Drawing.Size(290, 154);
            this.PG_Raid.TabIndex = 2;
            this.PG_Raid.ToolbarVisible = false;
            // 
            // CB_Raid
            // 
            this.CB_Raid.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Raid.FormattingEnabled = true;
            this.CB_Raid.Location = new System.Drawing.Point(12, 5);
            this.CB_Raid.Name = "CB_Raid";
            this.CB_Raid.Size = new System.Drawing.Size(103, 21);
            this.CB_Raid.TabIndex = 3;
            this.CB_Raid.SelectedIndexChanged += new System.EventHandler(this.CB_Raid_SelectedIndexChanged);
            // 
            // SAV_RaidSevenStar9
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 251);
            this.Controls.Add(this.CB_Raid);
            this.Controls.Add(this.PG_Raid);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_Cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(330, 290);
            this.Name = "SAV_RaidSevenStar9";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "7 Star Raid Parameter Editor";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.PropertyGrid PG_Raid;
        private System.Windows.Forms.ComboBox CB_Raid;
    }
}
