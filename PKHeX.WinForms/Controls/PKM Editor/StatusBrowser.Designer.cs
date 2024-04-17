namespace PKHeX.WinForms
{
    partial class StatusBrowser
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
            flp = new System.Windows.Forms.FlowLayoutPanel();
            SuspendLayout();
            // 
            // flp
            // 
            flp.AutoSize = true;
            flp.Dock = System.Windows.Forms.DockStyle.Fill;
            flp.Location = new System.Drawing.Point(0, 0);
            flp.Name = "flp";
            flp.Size = new System.Drawing.Size(127, 88);
            flp.TabIndex = 0;
            // 
            // StatusBrowser
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ClientSize = new System.Drawing.Size(127, 88);
            Controls.Add(flp);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Name = "StatusBrowser";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Status Browser";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flp;
    }
}
