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
            FLP_Slots = new System.Windows.Forms.TableLayoutPanel();
            SuspendLayout();
            // 
            // FLP_Slots
            // 
            FLP_Slots.AutoScroll = true;
            FLP_Slots.Dock = System.Windows.Forms.DockStyle.Fill;
            FLP_Slots.Location = new System.Drawing.Point(0, 0);
            FLP_Slots.Name = "FLP_Slots";
            FLP_Slots.Size = new System.Drawing.Size(86, 32);
            FLP_Slots.TabIndex = 0;
            // 
            // SlotList
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(FLP_Slots);
            Name = "SlotList";
            Size = new System.Drawing.Size(86, 32);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel FLP_Slots;
    }
}
