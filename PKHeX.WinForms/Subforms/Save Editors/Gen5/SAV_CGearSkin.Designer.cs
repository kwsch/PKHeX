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
            this.PB_Background = new System.Windows.Forms.PictureBox();
            this.B_Cancel = new System.Windows.Forms.Button();
            this.B_Save = new System.Windows.Forms.Button();
            this.B_ImportPNG = new System.Windows.Forms.Button();
            this.B_ExportPNG = new System.Windows.Forms.Button();
            this.B_ExportCGB = new System.Windows.Forms.Button();
            this.B_ImportCGB = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Background)).BeginInit();
            this.SuspendLayout();
            // 
            // PB_Background
            // 
            this.PB_Background.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PB_Background.Location = new System.Drawing.Point(12, 12);
            this.PB_Background.Name = "PB_Background";
            this.PB_Background.Size = new System.Drawing.Size(258, 194);
            this.PB_Background.TabIndex = 0;
            this.PB_Background.TabStop = false;
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(276, 183);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(103, 23);
            this.B_Cancel.TabIndex = 1;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(276, 154);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(103, 23);
            this.B_Save.TabIndex = 2;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // B_ImportPNG
            // 
            this.B_ImportPNG.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.B_ImportPNG.Location = new System.Drawing.Point(276, 12);
            this.B_ImportPNG.Name = "B_ImportPNG";
            this.B_ImportPNG.Size = new System.Drawing.Size(103, 23);
            this.B_ImportPNG.TabIndex = 3;
            this.B_ImportPNG.Text = "Import .png";
            this.B_ImportPNG.UseVisualStyleBackColor = true;
            this.B_ImportPNG.Click += new System.EventHandler(this.B_ImportPNG_Click);
            // 
            // B_ExportPNG
            // 
            this.B_ExportPNG.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.B_ExportPNG.Location = new System.Drawing.Point(276, 41);
            this.B_ExportPNG.Name = "B_ExportPNG";
            this.B_ExportPNG.Size = new System.Drawing.Size(103, 23);
            this.B_ExportPNG.TabIndex = 4;
            this.B_ExportPNG.Text = "Export .png";
            this.B_ExportPNG.UseVisualStyleBackColor = true;
            this.B_ExportPNG.Click += new System.EventHandler(this.B_ExportPNG_Click);
            // 
            // B_ExportCGB
            // 
            this.B_ExportCGB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.B_ExportCGB.Location = new System.Drawing.Point(276, 112);
            this.B_ExportCGB.Name = "B_ExportCGB";
            this.B_ExportCGB.Size = new System.Drawing.Size(103, 23);
            this.B_ExportCGB.TabIndex = 6;
            this.B_ExportCGB.Text = "Export .cgb";
            this.B_ExportCGB.UseVisualStyleBackColor = true;
            this.B_ExportCGB.Click += new System.EventHandler(this.B_ExportCGB_Click);
            // 
            // B_ImportCGB
            // 
            this.B_ImportCGB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.B_ImportCGB.Location = new System.Drawing.Point(276, 83);
            this.B_ImportCGB.Name = "B_ImportCGB";
            this.B_ImportCGB.Size = new System.Drawing.Size(103, 23);
            this.B_ImportCGB.TabIndex = 5;
            this.B_ImportCGB.Text = "Import .cgb/.psk";
            this.B_ImportCGB.UseVisualStyleBackColor = true;
            this.B_ImportCGB.Click += new System.EventHandler(this.B_ImportCGB_Click);
            // 
            // SAV_CGearSkin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(391, 216);
            this.Controls.Add(this.B_ExportCGB);
            this.Controls.Add(this.B_ImportCGB);
            this.Controls.Add(this.B_ExportPNG);
            this.Controls.Add(this.B_ImportPNG);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_Cancel);
            this.Controls.Add(this.PB_Background);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.Name = "SAV_CGearSkin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "C-Gear Skin";
            ((System.ComponentModel.ISupportInitialize)(this.PB_Background)).EndInit();
            this.ResumeLayout(false);

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