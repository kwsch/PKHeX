namespace PKHeX.WinForms.Controls
{
    partial class GenderToggle
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // GenderToggle
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            Name = "GenderToggle";
            Size = new System.Drawing.Size(18, 18);
            Click += new System.EventHandler(GenderToggle_Click);
            KeyDown += new System.Windows.Forms.KeyEventHandler(GenderToggle_KeyDown);
            ResumeLayout(false);

        }

        #endregion
    }
}
