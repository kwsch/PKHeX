namespace PKHeX.WinForms
{
    partial class SplashScreen
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
            this.L_Status = new System.Windows.Forms.Label();
            this.L_Site = new System.Windows.Forms.Label();
            this.PB_Icon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Icon)).BeginInit();
            this.SuspendLayout();
            // 
            // L_Status
            // 
            this.L_Status.AutoSize = true;
            this.L_Status.Location = new System.Drawing.Point(40, 3);
            this.L_Status.Name = "L_Status";
            this.L_Status.Size = new System.Drawing.Size(105, 13);
            this.L_Status.TabIndex = 0;
            this.L_Status.Text = "Starting up PKHeX...";
            // 
            // L_Site
            // 
            this.L_Site.AutoSize = true;
            this.L_Site.Location = new System.Drawing.Point(40, 21);
            this.L_Site.Name = "L_Site";
            this.L_Site.Size = new System.Drawing.Size(103, 13);
            this.L_Site.TabIndex = 1;
            this.L_Site.Text = "ProjectPokemon.org";
            // 
            // PB_Icon
            // 
            this.PB_Icon.BackgroundImage = global::PKHeX.WinForms.Properties.Resources.Icon.ToBitmap();
            this.PB_Icon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.PB_Icon.Location = new System.Drawing.Point(2, 3);
            this.PB_Icon.Name = "PB_Icon";
            this.PB_Icon.Size = new System.Drawing.Size(32, 32);
            this.PB_Icon.TabIndex = 2;
            this.PB_Icon.TabStop = false;
            // 
            // SplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(150, 38);
            this.Controls.Add(this.PB_Icon);
            this.Controls.Add(this.L_Site);
            this.Controls.Add(this.L_Status);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.Name = "SplashScreen";
            this.Opacity = 0.5D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.PB_Icon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label L_Site;
        private System.Windows.Forms.PictureBox PB_Icon;
        public System.Windows.Forms.Label L_Status;
    }
}