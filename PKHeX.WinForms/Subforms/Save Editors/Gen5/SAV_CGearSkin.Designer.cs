namespace PKHeX.WinForms
{
    partial class SAV_CGearSkin
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
            PB_Background = new System.Windows.Forms.PictureBox();
            B_Cancel = new System.Windows.Forms.Button();
            B_Save = new System.Windows.Forms.Button();
            B_ImportPNG = new System.Windows.Forms.Button();
            B_ExportPNG = new System.Windows.Forms.Button();
            B_ExportCGB = new System.Windows.Forms.Button();
            B_ImportCGB = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)PB_Background).BeginInit();
            SuspendLayout();
            // 
            // PB_Background
            // 
            PB_Background.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            PB_Background.Location = new System.Drawing.Point(14, 14);
            PB_Background.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PB_Background.Name = "PB_Background";
            PB_Background.Size = new System.Drawing.Size(301, 224);
            PB_Background.TabIndex = 0;
            PB_Background.TabStop = false;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(322, 211);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(120, 27);
            B_Cancel.TabIndex = 1;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(322, 178);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(120, 27);
            B_Save.TabIndex = 2;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_ImportPNG
            // 
            B_ImportPNG.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            B_ImportPNG.Location = new System.Drawing.Point(322, 14);
            B_ImportPNG.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ImportPNG.Name = "B_ImportPNG";
            B_ImportPNG.Size = new System.Drawing.Size(120, 27);
            B_ImportPNG.TabIndex = 3;
            B_ImportPNG.Text = "Import .png";
            B_ImportPNG.UseVisualStyleBackColor = true;
            B_ImportPNG.Click += B_ImportPNG_Click;
            // 
            // B_ExportPNG
            // 
            B_ExportPNG.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            B_ExportPNG.Location = new System.Drawing.Point(322, 47);
            B_ExportPNG.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ExportPNG.Name = "B_ExportPNG";
            B_ExportPNG.Size = new System.Drawing.Size(120, 27);
            B_ExportPNG.TabIndex = 4;
            B_ExportPNG.Text = "Export .png";
            B_ExportPNG.UseVisualStyleBackColor = true;
            B_ExportPNG.Click += B_ExportPNG_Click;
            // 
            // B_ExportCGB
            // 
            B_ExportCGB.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            B_ExportCGB.Location = new System.Drawing.Point(322, 129);
            B_ExportCGB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ExportCGB.Name = "B_ExportCGB";
            B_ExportCGB.Size = new System.Drawing.Size(120, 27);
            B_ExportCGB.TabIndex = 6;
            B_ExportCGB.Text = "Export .cgb";
            B_ExportCGB.UseVisualStyleBackColor = true;
            B_ExportCGB.Click += B_ExportCGB_Click;
            // 
            // B_ImportCGB
            // 
            B_ImportCGB.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            B_ImportCGB.Location = new System.Drawing.Point(322, 96);
            B_ImportCGB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ImportCGB.Name = "B_ImportCGB";
            B_ImportCGB.Size = new System.Drawing.Size(120, 27);
            B_ImportCGB.TabIndex = 5;
            B_ImportCGB.Text = "Import .cgb/.psk";
            B_ImportCGB.UseVisualStyleBackColor = true;
            B_ImportCGB.Click += B_ImportCGB_Click;
            // 
            // SAV_CGearSkin
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(456, 249);
            Controls.Add(B_ExportCGB);
            Controls.Add(B_ImportCGB);
            Controls.Add(B_ExportPNG);
            Controls.Add(B_ImportPNG);
            Controls.Add(B_Save);
            Controls.Add(B_Cancel);
            Controls.Add(PB_Background);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "SAV_CGearSkin";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "C-Gear Skin";
            ((System.ComponentModel.ISupportInitialize)PB_Background).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.PictureBox PB_Background;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_ImportPNG;
        private System.Windows.Forms.Button B_ExportPNG;
        private System.Windows.Forms.Button B_ExportCGB;
        private System.Windows.Forms.Button B_ImportCGB;
    }
}
