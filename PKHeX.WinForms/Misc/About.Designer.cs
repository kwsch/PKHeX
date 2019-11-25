namespace PKHeX.WinForms
{
    partial class About
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
            this.L_Thanks = new System.Windows.Forms.Label();
            this.TC_About = new System.Windows.Forms.TabControl();
            this.Tab_Shortcuts = new System.Windows.Forms.TabPage();
            this.RTB_Shortcuts = new System.Windows.Forms.RichTextBox();
            this.Tab_Changelog = new System.Windows.Forms.TabPage();
            this.RTB_Changelog = new System.Windows.Forms.RichTextBox();
            this.TC_About.SuspendLayout();
            this.Tab_Shortcuts.SuspendLayout();
            this.Tab_Changelog.SuspendLayout();
            this.SuspendLayout();
            // 
            // L_Thanks
            // 
            this.L_Thanks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_Thanks.AutoSize = true;
            this.L_Thanks.Location = new System.Drawing.Point(265, 4);
            this.L_Thanks.Name = "L_Thanks";
            this.L_Thanks.Size = new System.Drawing.Size(147, 13);
            this.L_Thanks.TabIndex = 2;
            this.L_Thanks.Text = "Thanks to all the researchers!";
            // 
            // TC_About
            // 
            this.TC_About.Controls.Add(this.Tab_Shortcuts);
            this.TC_About.Controls.Add(this.Tab_Changelog);
            this.TC_About.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TC_About.Location = new System.Drawing.Point(0, 0);
            this.TC_About.Margin = new System.Windows.Forms.Padding(0);
            this.TC_About.Name = "TC_About";
            this.TC_About.SelectedIndex = 0;
            this.TC_About.Size = new System.Drawing.Size(494, 372);
            this.TC_About.TabIndex = 5;
            // 
            // Tab_Shortcuts
            // 
            this.Tab_Shortcuts.Controls.Add(this.RTB_Shortcuts);
            this.Tab_Shortcuts.Location = new System.Drawing.Point(4, 22);
            this.Tab_Shortcuts.Name = "Tab_Shortcuts";
            this.Tab_Shortcuts.Size = new System.Drawing.Size(486, 346);
            this.Tab_Shortcuts.TabIndex = 0;
            this.Tab_Shortcuts.Text = "Shortcuts";
            this.Tab_Shortcuts.UseVisualStyleBackColor = true;
            // 
            // RTB_Shortcuts
            // 
            this.RTB_Shortcuts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RTB_Shortcuts.Location = new System.Drawing.Point(0, 0);
            this.RTB_Shortcuts.Name = "RTB_Shortcuts";
            this.RTB_Shortcuts.ReadOnly = true;
            this.RTB_Shortcuts.Size = new System.Drawing.Size(486, 346);
            this.RTB_Shortcuts.TabIndex = 3;
            this.RTB_Shortcuts.Text = "";
            this.RTB_Shortcuts.WordWrap = false;
            // 
            // Tab_Changelog
            // 
            this.Tab_Changelog.Controls.Add(this.RTB_Changelog);
            this.Tab_Changelog.Location = new System.Drawing.Point(4, 22);
            this.Tab_Changelog.Name = "Tab_Changelog";
            this.Tab_Changelog.Size = new System.Drawing.Size(486, 346);
            this.Tab_Changelog.TabIndex = 1;
            this.Tab_Changelog.Text = "Changelog";
            this.Tab_Changelog.UseVisualStyleBackColor = true;
            // 
            // RTB_Changelog
            // 
            this.RTB_Changelog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RTB_Changelog.Location = new System.Drawing.Point(0, 0);
            this.RTB_Changelog.Name = "RTB_Changelog";
            this.RTB_Changelog.ReadOnly = true;
            this.RTB_Changelog.Size = new System.Drawing.Size(486, 346);
            this.RTB_Changelog.TabIndex = 2;
            this.RTB_Changelog.Text = "";
            this.RTB_Changelog.WordWrap = false;
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 372);
            this.Controls.Add(this.L_Thanks);
            this.Controls.Add(this.TC_About);
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(910, 710);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(510, 410);
            this.Name = "About";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.TC_About.ResumeLayout(false);
            this.Tab_Shortcuts.ResumeLayout(false);
            this.Tab_Changelog.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label L_Thanks;
        private System.Windows.Forms.TabControl TC_About;
        private System.Windows.Forms.TabPage Tab_Shortcuts;
        private System.Windows.Forms.RichTextBox RTB_Shortcuts;
        private System.Windows.Forms.TabPage Tab_Changelog;
        private System.Windows.Forms.RichTextBox RTB_Changelog;
    }
}