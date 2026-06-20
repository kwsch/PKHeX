namespace PKHeX.WinForms.Controls
{
    partial class ExperienceBar
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
            PAN_ExpPercent = new System.Windows.Forms.Panel();
            SuspendLayout();
            // 
            // PAN_ExpPercent
            // 
            PAN_ExpPercent.BackColor = System.Drawing.Color.DeepSkyBlue;
            PAN_ExpPercent.Dock = System.Windows.Forms.DockStyle.Left;
            PAN_ExpPercent.Location = new System.Drawing.Point(0, 0);
            PAN_ExpPercent.Margin = new System.Windows.Forms.Padding(0);
            PAN_ExpPercent.Name = "PAN_ExpPercent";
            PAN_ExpPercent.Size = new System.Drawing.Size(40, 12);
            PAN_ExpPercent.TabIndex = 0;
            PAN_ExpPercent.MouseClick += HandleClick;
            // 
            // ExperienceBar
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlLight;
            Controls.Add(PAN_ExpPercent);
            Margin = new System.Windows.Forms.Padding(0);
            Name = "ExperienceBar";
            Size = new System.Drawing.Size(100, 12);
            MouseClick += HandleClick;
            MouseWheel += OnScroll;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel PAN_ExpPercent;
    }
}
