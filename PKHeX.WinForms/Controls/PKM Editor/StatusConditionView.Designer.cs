namespace PKHeX.WinForms.Controls
{
    partial class StatusConditionView
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
            PB_Status = new SelectablePictureBox();
            ((System.ComponentModel.ISupportInitialize)PB_Status).BeginInit();
            SuspendLayout();
            // 
            // PB_Status
            // 
            PB_Status.Dock = System.Windows.Forms.DockStyle.Fill;
            PB_Status.Location = new System.Drawing.Point(0, 0);
            PB_Status.Margin = new System.Windows.Forms.Padding(0);
            PB_Status.Name = "PB_Status";
            PB_Status.Size = new System.Drawing.Size(224, 96);
            PB_Status.TabIndex = 0;
            PB_Status.Click += PB_Status_Click;
            // 
            // StatusConditionView
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(PB_Status);
            Margin = new System.Windows.Forms.Padding(0);
            Name = "StatusConditionView";
            Size = new System.Drawing.Size(224, 96);
            ((System.ComponentModel.ISupportInitialize)PB_Status).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private SelectablePictureBox PB_Status;
    }
}
