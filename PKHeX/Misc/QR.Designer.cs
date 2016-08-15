namespace PKHeX
{
    partial class QR
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QR));
            this.PB_QR = new System.Windows.Forms.PictureBox();
            this.FontLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PB_QR)).BeginInit();
            this.SuspendLayout();
            // 
            // PB_QR
            // 
            this.PB_QR.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PB_QR.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.PB_QR.Location = new System.Drawing.Point(2, 1);
            this.PB_QR.Name = "PB_QR";
            this.PB_QR.Size = new System.Drawing.Size(365, 365);
            this.PB_QR.TabIndex = 0;
            this.PB_QR.TabStop = false;
            this.PB_QR.Click += new System.EventHandler(this.PB_QR_Click);
            // 
            // FontLabel
            // 
            this.FontLabel.AutoSize = true;
            this.FontLabel.Location = new System.Drawing.Point(348, 353);
            this.FontLabel.Name = "FontLabel";
            this.FontLabel.Size = new System.Drawing.Size(19, 13);
            this.FontLabel.TabIndex = 1;
            this.FontLabel.Text = "<3";
            this.FontLabel.Visible = false;
            // 
            // QR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 367);
            this.Controls.Add(this.FontLabel);
            this.Controls.Add(this.PB_QR);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QR";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PKHeX QR Code (Click QR to Copy Image)";
            ((System.ComponentModel.ISupportInitialize)(this.PB_QR)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox PB_QR;
        private System.Windows.Forms.Label FontLabel;
    }
}