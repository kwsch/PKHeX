namespace PKHeX.WinForms
{
    partial class SettingsEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsEditor));
            this.FLP_Settings = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            //
            // FLP_Settings
            //
            this.FLP_Settings.AutoScroll = true;
            this.FLP_Settings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FLP_Settings.Location = new System.Drawing.Point(0, 0);
            this.FLP_Settings.Name = "FLP_Settings";
            this.FLP_Settings.Size = new System.Drawing.Size(306, 311);
            this.FLP_Settings.TabIndex = 0;
            //
            // SettingsEditor
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(306, 311);
            this.Controls.Add(this.FLP_Settings);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsEditor_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel FLP_Settings;
    }
}