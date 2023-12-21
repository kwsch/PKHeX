namespace PKHeX.WinForms
{
    partial class SAV_Chatter
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
            B_ImportPCM = new System.Windows.Forms.Button();
            B_ExportPCM = new System.Windows.Forms.Button();
            MT_Confusion = new System.Windows.Forms.MaskedTextBox();
            L_Confusion = new System.Windows.Forms.Label();
            CHK_Initialized = new System.Windows.Forms.CheckBox();
            B_ExportWAV = new System.Windows.Forms.Button();
            B_PlayRecording = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(29, 135);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(120, 27);
            B_Save.TabIndex = 26;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(157, 135);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(120, 27);
            B_Cancel.TabIndex = 25;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_ImportPCM
            // 
            B_ImportPCM.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            B_ImportPCM.Location = new System.Drawing.Point(157, 12);
            B_ImportPCM.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ImportPCM.Name = "B_ImportPCM";
            B_ImportPCM.Size = new System.Drawing.Size(120, 27);
            B_ImportPCM.TabIndex = 27;
            B_ImportPCM.Text = "Import .pcm";
            B_ImportPCM.UseVisualStyleBackColor = true;
            B_ImportPCM.Click += B_ImportPCM_Click;
            // 
            // B_ExportPCM
            // 
            B_ExportPCM.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            B_ExportPCM.Location = new System.Drawing.Point(157, 45);
            B_ExportPCM.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ExportPCM.Name = "B_ExportPCM";
            B_ExportPCM.Size = new System.Drawing.Size(120, 27);
            B_ExportPCM.TabIndex = 28;
            B_ExportPCM.Text = "Export .pcm";
            B_ExportPCM.UseVisualStyleBackColor = true;
            B_ExportPCM.Click += B_ExportPCM_Click;
            // 
            // MT_Confusion
            // 
            MT_Confusion.Enabled = false;
            MT_Confusion.Location = new System.Drawing.Point(104, 79);
            MT_Confusion.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MT_Confusion.Mask = "000";
            MT_Confusion.Name = "MT_Confusion";
            MT_Confusion.Size = new System.Drawing.Size(29, 23);
            MT_Confusion.TabIndex = 65;
            MT_Confusion.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // L_Confusion
            // 
            L_Confusion.Location = new System.Drawing.Point(13, 79);
            L_Confusion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Confusion.Name = "L_Confusion";
            L_Confusion.Size = new System.Drawing.Size(83, 23);
            L_Confusion.TabIndex = 64;
            L_Confusion.Text = "Confusion %:";
            L_Confusion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CHK_Initialized
            // 
            CHK_Initialized.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            CHK_Initialized.AutoSize = true;
            CHK_Initialized.Location = new System.Drawing.Point(20, 50);
            CHK_Initialized.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_Initialized.Name = "CHK_Initialized";
            CHK_Initialized.Size = new System.Drawing.Size(76, 19);
            CHK_Initialized.TabIndex = 66;
            CHK_Initialized.Text = "Initialized";
            CHK_Initialized.UseVisualStyleBackColor = true;
            CHK_Initialized.CheckedChanged += CHK_Initialized_CheckedChanged;
            // 
            // B_ExportWAV
            // 
            B_ExportWAV.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            B_ExportWAV.Location = new System.Drawing.Point(157, 79);
            B_ExportWAV.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ExportWAV.Name = "B_ExportWAV";
            B_ExportWAV.Size = new System.Drawing.Size(120, 27);
            B_ExportWAV.TabIndex = 67;
            B_ExportWAV.Text = "Export .wav";
            B_ExportWAV.UseVisualStyleBackColor = true;
            B_ExportWAV.Click += B_ExportWAV_Click;
            // 
            // B_PlayRecording
            // 
            B_PlayRecording.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            B_PlayRecording.Location = new System.Drawing.Point(13, 12);
            B_PlayRecording.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_PlayRecording.Name = "B_PlayRecording";
            B_PlayRecording.Size = new System.Drawing.Size(120, 27);
            B_PlayRecording.TabIndex = 68;
            B_PlayRecording.Text = "Play Recording";
            B_PlayRecording.UseVisualStyleBackColor = true;
            B_PlayRecording.Click += B_PlayRecording_Click;
            // 
            // SAV_Chatter
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(290, 173);
            Controls.Add(B_PlayRecording);
            Controls.Add(B_ExportWAV);
            Controls.Add(CHK_Initialized);
            Controls.Add(MT_Confusion);
            Controls.Add(L_Confusion);
            Controls.Add(B_ExportPCM);
            Controls.Add(B_ImportPCM);
            Controls.Add(B_Cancel);
            Controls.Add(B_Save);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(2);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(306, 212);
            Name = "SAV_Chatter";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Chatter Editor";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_ImportPCM;
        private System.Windows.Forms.Button B_ExportPCM;
        private System.Windows.Forms.MaskedTextBox MT_Confusion;
        private System.Windows.Forms.Label L_Confusion;
        private System.Windows.Forms.CheckBox CHK_Initialized;
        private System.Windows.Forms.Button B_ExportWAV;
        private System.Windows.Forms.Button B_PlayRecording;
    }
}
