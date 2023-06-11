namespace PKHeX.WinForms
{
    partial class SAV_UnityTower
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
            this.B_SetAllLocations = new System.Windows.Forms.Button();
            this.B_SetAllLegalLocations = new System.Windows.Forms.Button();
            this.B_ClearLocations = new System.Windows.Forms.Button();
            this.CHK_GlobalFlag = new System.Windows.Forms.CheckBox();
            this.CHK_UnityTowerFlag = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(195, 137);
            this.B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(88, 27);
            this.B_Save.TabIndex = 26;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(101, 137);
            this.B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(88, 27);
            this.B_Cancel.TabIndex = 25;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // B_SetAllLocations
            // 
            this.B_SetAllLocations.Location = new System.Drawing.Point(13, 12);
            this.B_SetAllLocations.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.B_SetAllLocations.Name = "B_SetAllLocations";
            this.B_SetAllLocations.Size = new System.Drawing.Size(264, 27);
            this.B_SetAllLocations.TabIndex = 27;
            this.B_SetAllLocations.Text = "Set All Locations";
            this.B_SetAllLocations.UseVisualStyleBackColor = true;
            this.B_SetAllLocations.Click += new System.EventHandler(this.B_SetAllLocations_Click);
            // 
            // B_SetAllLegalLocations
            // 
            this.B_SetAllLegalLocations.Location = new System.Drawing.Point(13, 45);
            this.B_SetAllLegalLocations.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.B_SetAllLegalLocations.Name = "B_SetAllLegalLocations";
            this.B_SetAllLegalLocations.Size = new System.Drawing.Size(264, 27);
            this.B_SetAllLegalLocations.TabIndex = 28;
            this.B_SetAllLegalLocations.Text = "Set All Legal Locations";
            this.B_SetAllLegalLocations.UseVisualStyleBackColor = true;
            this.B_SetAllLegalLocations.Click += new System.EventHandler(this.B_SetAllLegalLocations_Click);
            // 
            // B_ClearLocations
            // 
            this.B_ClearLocations.Location = new System.Drawing.Point(13, 78);
            this.B_ClearLocations.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.B_ClearLocations.Name = "B_ClearLocations";
            this.B_ClearLocations.Size = new System.Drawing.Size(264, 27);
            this.B_ClearLocations.TabIndex = 29;
            this.B_ClearLocations.Text = "Clear Locations";
            this.B_ClearLocations.UseVisualStyleBackColor = true;
            this.B_ClearLocations.Click += new System.EventHandler(this.B_ClearLocations_Click);
            // 
            // CHK_GlobalFlag
            // 
            this.CHK_GlobalFlag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CHK_GlobalFlag.AutoSize = true;
            this.CHK_GlobalFlag.Location = new System.Drawing.Point(13, 111);
            this.CHK_GlobalFlag.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CHK_GlobalFlag.Name = "CHK_GlobalFlag";
            this.CHK_GlobalFlag.Size = new System.Drawing.Size(131, 19);
            this.CHK_GlobalFlag.TabIndex = 45;
            this.CHK_GlobalFlag.Text = "Whole Globe Visible";
            this.CHK_GlobalFlag.UseVisualStyleBackColor = true;
            // 
            // CHK_UnityTowerFlag
            // 
            this.CHK_UnityTowerFlag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CHK_UnityTowerFlag.AutoSize = true;
            this.CHK_UnityTowerFlag.Location = new System.Drawing.Point(144, 111);
            this.CHK_UnityTowerFlag.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CHK_UnityTowerFlag.Name = "CHK_UnityTowerFlag";
            this.CHK_UnityTowerFlag.Size = new System.Drawing.Size(141, 19);
            this.CHK_UnityTowerFlag.TabIndex = 46;
            this.CHK_UnityTowerFlag.Text = "Unity Tower Unlocked";
            this.CHK_UnityTowerFlag.UseVisualStyleBackColor = true;
            // 
            // SAV_UnityTower
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(290, 173);
            this.Controls.Add(this.CHK_UnityTowerFlag);
            this.Controls.Add(this.CHK_GlobalFlag);
            this.Controls.Add(this.B_ClearLocations);
            this.Controls.Add(this.B_SetAllLegalLocations);
            this.Controls.Add(this.B_SetAllLocations);
            this.Controls.Add(this.B_Cancel);
            this.Controls.Add(this.B_Save);
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(306, 212);
            this.Name = "SAV_UnityTower";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Unity Tower Editor";
            this.Load += new System.EventHandler(this.SAV_UnityTower_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_SetAllLocations;
        private System.Windows.Forms.Button B_SetAllLegalLocations;
        private System.Windows.Forms.Button B_ClearLocations;
        private System.Windows.Forms.CheckBox CHK_GlobalFlag;
        private System.Windows.Forms.CheckBox CHK_UnityTowerFlag;
    }
}
