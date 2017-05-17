namespace PKHeX.WinForms
{
    partial class SAV_FolderList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SAV_FolderList));
            this.FLP_Buttons = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // FLP_Buttons
            // 
            this.FLP_Buttons.AutoScroll = true;
            this.FLP_Buttons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FLP_Buttons.Location = new System.Drawing.Point(0, 0);
            this.FLP_Buttons.Name = "FLP_Buttons";
            this.FLP_Buttons.Padding = new System.Windows.Forms.Padding(3);
            this.FLP_Buttons.Size = new System.Drawing.Size(304, 161);
            this.FLP_Buttons.TabIndex = 0;
            // 
            // SAV_FolderList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 161);
            this.Controls.Add(this.FLP_Buttons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SAV_FolderList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Folder List";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel FLP_Buttons;
    }
}