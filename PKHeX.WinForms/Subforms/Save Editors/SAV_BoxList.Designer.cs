namespace PKHeX.WinForms
{
    sealed partial class SAV_BoxList
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
            FLP_Boxes = new System.Windows.Forms.FlowLayoutPanel();
            SuspendLayout();
            // 
            // FLP_Boxes
            // 
            FLP_Boxes.AutoScroll = true;
            FLP_Boxes.Dock = System.Windows.Forms.DockStyle.Fill;
            FLP_Boxes.Location = new System.Drawing.Point(0, 0);
            FLP_Boxes.Name = "FLP_Boxes";
            FLP_Boxes.Size = new System.Drawing.Size(250, 185);
            FLP_Boxes.TabIndex = 0;
            // 
            // SAV_BoxList
            // 
            AllowDrop = true;
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(250, 185);
            Controls.Add(FLP_Boxes);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_BoxList";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Storage Viewer";
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel FLP_Boxes;
    }
}
