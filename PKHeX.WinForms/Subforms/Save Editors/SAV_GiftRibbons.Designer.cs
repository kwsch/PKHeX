namespace PKHeX.WinForms
{
    partial class SAV_GiftRibbons
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            B_Reset = new System.Windows.Forms.Button();
            B_Legal = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            B_Save = new System.Windows.Forms.Button();
            dgv = new PKHeX.WinForms.Controls.DoubleBufferedDataGridView();
            ((System.ComponentModel.ISupportInitialize)dgv).BeginInit();
            SuspendLayout();
            // 
            // B_Reset
            // 
            B_Reset.Location = new System.Drawing.Point(134, 16);
            B_Reset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Reset.Name = "B_Reset";
            B_Reset.Size = new System.Drawing.Size(113, 27);
            B_Reset.TabIndex = 21;
            B_Reset.Text = "Reset";
            B_Reset.UseVisualStyleBackColor = true;
            B_Reset.Click += B_Reset_Click;
            // 
            // B_Legal
            // 
            B_Legal.Location = new System.Drawing.Point(14, 16);
            B_Legal.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Legal.Name = "B_Legal";
            B_Legal.Size = new System.Drawing.Size(113, 27);
            B_Legal.TabIndex = 20;
            B_Legal.Text = "All Legal";
            B_Legal.UseVisualStyleBackColor = true;
            B_Legal.Click += B_Legal_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            B_Cancel.Location = new System.Drawing.Point(14, 271);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(113, 27);
            B_Cancel.TabIndex = 19;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            B_Save.Location = new System.Drawing.Point(134, 271);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(113, 27);
            B_Save.TabIndex = 18;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // dgv
            // 
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeColumns = false;
            dgv.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ControlLight;
            dgv.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgv.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom;
            dgv.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            dgv.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dgv.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            dgv.Location = new System.Drawing.Point(14, 50);
            dgv.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dgv.MultiSelect = false;
            dgv.Name = "dgv";
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            dgv.ShowEditingIcon = false;
            dgv.Size = new System.Drawing.Size(233, 215);
            dgv.TabIndex = 17;
            // 
            // SAV_GiftRibbons
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(261, 314);
            Controls.Add(B_Reset);
            Controls.Add(B_Legal);
            Controls.Add(B_Cancel);
            Controls.Add(B_Save);
            Controls.Add(dgv);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(2);
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(277, 353);
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(277, 353);
            Name = "SAV_GiftRibbons";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "GIft Ribbon Editor";
            ((System.ComponentModel.ISupportInitialize)dgv).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button B_Reset;
        private System.Windows.Forms.Button B_Legal;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private Controls.DoubleBufferedDataGridView dgv;
    }
}
