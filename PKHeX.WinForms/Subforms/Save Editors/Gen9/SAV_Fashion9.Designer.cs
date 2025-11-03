namespace PKHeX.WinForms
{
    sealed partial class SAV_Fashion9
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
            TC_Features = new System.Windows.Forms.TabControl();
            B_SetAllOwned = new System.Windows.Forms.Button();
            SuspendLayout();
            //
            // B_Cancel
            //
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(346, 397);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(88, 27);
            B_Cancel.TabIndex = 2;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            //
            // B_Save
            //
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(442, 397);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(88, 27);
            B_Save.TabIndex = 9;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            //
            // TC_Features
            //
            TC_Features.AllowDrop = true;
            TC_Features.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TC_Features.Location = new System.Drawing.Point(9, 9);
            TC_Features.Margin = new System.Windows.Forms.Padding(0);
            TC_Features.Name = "TC_Features";
            TC_Features.Padding = new System.Drawing.Point(0, 0);
            TC_Features.SelectedIndex = 0;
            TC_Features.Size = new System.Drawing.Size(525, 377);
            TC_Features.TabIndex = 42;
            //
            // B_SetAllOwned
            //
            B_SetAllOwned.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_SetAllOwned.Location = new System.Drawing.Point(13, 398);
            B_SetAllOwned.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_SetAllOwned.Name = "B_SetAllOwned";
            B_SetAllOwned.Size = new System.Drawing.Size(183, 27);
            B_SetAllOwned.TabIndex = 43;
            B_SetAllOwned.Text = "Set All Owned";
            B_SetAllOwned.UseVisualStyleBackColor = true;
            B_SetAllOwned.Click += B_SetAllOwned_Click;
            //
            // SAV_Fashion9
            //
            AllowDrop = true;
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(543, 437);
            Controls.Add(B_SetAllOwned);
            Controls.Add(TC_Features);
            Controls.Add(B_Save);
            Controls.Add(B_Cancel);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(779, 917);
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(546, 456);
            Name = "SAV_Fashion9";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Fashion Editor";
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.TabControl TC_Features;
        private System.Windows.Forms.Button B_SetAllOwned;
    }
}
