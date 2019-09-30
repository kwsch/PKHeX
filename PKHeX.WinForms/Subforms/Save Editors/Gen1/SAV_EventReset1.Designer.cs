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
            this.FLP_List = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // FLP_List
            // 
            this.FLP_List.AutoScroll = true;
            this.FLP_List.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FLP_List.Location = new System.Drawing.Point(0, 0);
            this.FLP_List.Name = "FLP_List";
            this.FLP_List.Size = new System.Drawing.Size(284, 261);
            this.FLP_List.TabIndex = 0;
            // 
            // SAV_EventReset1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.FLP_List);
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.Name = "SAV_EventReset1";
            this.Text = "Event Resetter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SAV_EventReset1_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel FLP_List;
    }
}