namespace PKHeX.WinForms
{
    partial class SAV_Raid8
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
            PG_Den = new System.Windows.Forms.PropertyGrid();
            CB_Den = new System.Windows.Forms.ComboBox();
            SuspendLayout();
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(170, 333);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(88, 27);
            B_Cancel.TabIndex = 0;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(265, 333);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(88, 27);
            B_Save.TabIndex = 1;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // PG_Den
            // 
            PG_Den.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            PG_Den.Location = new System.Drawing.Point(14, 37);
            PG_Den.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PG_Den.Name = "PG_Den";
            PG_Den.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            PG_Den.Size = new System.Drawing.Size(338, 290);
            PG_Den.TabIndex = 2;
            PG_Den.ToolbarVisible = false;
            // 
            // CB_Den
            // 
            CB_Den.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Den.FormattingEnabled = true;
            CB_Den.Location = new System.Drawing.Point(14, 6);
            CB_Den.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Den.Name = "CB_Den";
            CB_Den.Size = new System.Drawing.Size(119, 23);
            CB_Den.TabIndex = 3;
            CB_Den.SelectedIndexChanged += CB_Den_SelectedIndexChanged;
            // 
            // SAV_Raid8
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(366, 370);
            Controls.Add(CB_Den);
            Controls.Add(PG_Den);
            Controls.Add(B_Save);
            Controls.Add(B_Cancel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_Raid8";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Raid Parameter Editor";
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.PropertyGrid PG_Den;
        private System.Windows.Forms.ComboBox CB_Den;
    }
}
