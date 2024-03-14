namespace PKHeX.WinForms
{
    partial class SAV_EventReset1
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
            FLP_List = new System.Windows.Forms.FlowLayoutPanel();
            SuspendLayout();
            // 
            // FLP_List
            // 
            FLP_List.AutoScroll = true;
            FLP_List.Dock = System.Windows.Forms.DockStyle.Fill;
            FLP_List.Location = new System.Drawing.Point(0, 0);
            FLP_List.Name = "FLP_List";
            FLP_List.Size = new System.Drawing.Size(284, 261);
            FLP_List.TabIndex = 0;
            // 
            // SAV_EventReset1
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(284, 261);
            Controls.Add(FLP_List);
            Icon = Properties.Resources.Icon;
            MaximizeBox = false;
            Name = "SAV_EventReset1";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Event Resetter";
            FormClosing += SAV_EventReset1_FormClosing;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel FLP_List;
    }
}
