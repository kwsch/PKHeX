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
            L_Status = new System.Windows.Forms.Label();
            L_Site = new System.Windows.Forms.Label();
            PB_Icon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)PB_Icon).BeginInit();
            SuspendLayout();
            // 
            // L_Status
            // 
            L_Status.AutoSize = true;
            L_Status.Location = new System.Drawing.Point(40, 8);
            L_Status.Name = "L_Status";
            L_Status.Size = new System.Drawing.Size(113, 15);
            L_Status.TabIndex = 0;
            L_Status.Text = "Starting up PKHeX...";
            // 
            // L_Site
            // 
            L_Site.AutoSize = true;
            L_Site.Location = new System.Drawing.Point(40, 24);
            L_Site.Name = "L_Site";
            L_Site.Size = new System.Drawing.Size(116, 15);
            L_Site.TabIndex = 1;
            L_Site.Text = "ProjectPokemon.org";
            // 
            // PB_Icon
            // 
            PB_Icon.BackgroundImage = global::PKHeX.WinForms.Properties.Resources.Icon.ToBitmap();
            PB_Icon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            PB_Icon.Location = new System.Drawing.Point(8, 8);
            PB_Icon.Name = "PB_Icon";
            PB_Icon.Size = new System.Drawing.Size(32, 32);
            PB_Icon.TabIndex = 2;
            PB_Icon.TabStop = false;
            // 
            // SplashScreen
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(160, 48);
            Controls.Add(PB_Icon);
            Controls.Add(L_Site);
            Controls.Add(L_Status);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Icon = Properties.Resources.Icon;
            Name = "SplashScreen";
            Opacity = 0.5D;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            FormClosing += SplashScreen_FormClosing;
            ((System.ComponentModel.ISupportInitialize)PB_Icon).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label L_Site;
        private System.Windows.Forms.PictureBox PB_Icon;
        private System.Windows.Forms.Label L_Status;
    }
}
