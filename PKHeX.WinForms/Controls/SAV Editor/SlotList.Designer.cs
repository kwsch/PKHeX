namespace PKHeX.WinForms.Controls
{
    partial class SlotList
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
            this.FLP_Slots = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // FLP_Slots
            // 
            this.FLP_Slots.AutoScroll = true;
            this.FLP_Slots.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FLP_Slots.Location = new System.Drawing.Point(0, 0);
            this.FLP_Slots.Name = "FLP_Slots";
            this.FLP_Slots.Size = new System.Drawing.Size(86, 32);
            this.FLP_Slots.TabIndex = 0;
            // 
            // SlotList
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.FLP_Slots);
            this.Name = "SlotList";
            this.Size = new System.Drawing.Size(86, 32);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel FLP_Slots;
    }
}
