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
            L_Thanks = new System.Windows.Forms.Label();
            TC_About = new System.Windows.Forms.TabControl();
            Tab_Shortcuts = new System.Windows.Forms.TabPage();
            RTB_Shortcuts = new System.Windows.Forms.RichTextBox();
            Tab_Changelog = new System.Windows.Forms.TabPage();
            RTB_Changelog = new System.Windows.Forms.RichTextBox();
            TC_About.SuspendLayout();
            Tab_Shortcuts.SuspendLayout();
            Tab_Changelog.SuspendLayout();
            SuspendLayout();
            // 
            // L_Thanks
            // 
            L_Thanks.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            L_Thanks.Location = new System.Drawing.Point(309, 5);
            L_Thanks.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Thanks.Name = "L_Thanks";
            L_Thanks.Size = new System.Drawing.Size(262, 15);
            L_Thanks.TabIndex = 2;
            L_Thanks.Text = "Thanks to all the researchers!";
            L_Thanks.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TC_About
            // 
            TC_About.Controls.Add(Tab_Shortcuts);
            TC_About.Controls.Add(Tab_Changelog);
            TC_About.Dock = System.Windows.Forms.DockStyle.Fill;
            TC_About.Location = new System.Drawing.Point(0, 0);
            TC_About.Margin = new System.Windows.Forms.Padding(0);
            TC_About.Name = "TC_About";
            TC_About.SelectedIndex = 0;
            TC_About.Size = new System.Drawing.Size(576, 429);
            TC_About.TabIndex = 5;
            // 
            // Tab_Shortcuts
            // 
            Tab_Shortcuts.Controls.Add(RTB_Shortcuts);
            Tab_Shortcuts.Location = new System.Drawing.Point(4, 24);
            Tab_Shortcuts.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_Shortcuts.Name = "Tab_Shortcuts";
            Tab_Shortcuts.Size = new System.Drawing.Size(568, 401);
            Tab_Shortcuts.TabIndex = 0;
            Tab_Shortcuts.Text = "Shortcuts";
            Tab_Shortcuts.UseVisualStyleBackColor = true;
            // 
            // RTB_Shortcuts
            // 
            RTB_Shortcuts.Dock = System.Windows.Forms.DockStyle.Fill;
            RTB_Shortcuts.Location = new System.Drawing.Point(0, 0);
            RTB_Shortcuts.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RTB_Shortcuts.Name = "RTB_Shortcuts";
            RTB_Shortcuts.ReadOnly = true;
            RTB_Shortcuts.Size = new System.Drawing.Size(568, 401);
            RTB_Shortcuts.TabIndex = 3;
            RTB_Shortcuts.Text = "";
            RTB_Shortcuts.WordWrap = false;
            // 
            // Tab_Changelog
            // 
            Tab_Changelog.Controls.Add(RTB_Changelog);
            Tab_Changelog.Location = new System.Drawing.Point(4, 24);
            Tab_Changelog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Tab_Changelog.Name = "Tab_Changelog";
            Tab_Changelog.Size = new System.Drawing.Size(568, 401);
            Tab_Changelog.TabIndex = 1;
            Tab_Changelog.Text = "Changelog";
            Tab_Changelog.UseVisualStyleBackColor = true;
            // 
            // RTB_Changelog
            // 
            RTB_Changelog.Dock = System.Windows.Forms.DockStyle.Fill;
            RTB_Changelog.Location = new System.Drawing.Point(0, 0);
            RTB_Changelog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RTB_Changelog.Name = "RTB_Changelog";
            RTB_Changelog.ReadOnly = true;
            RTB_Changelog.Size = new System.Drawing.Size(568, 401);
            RTB_Changelog.TabIndex = 2;
            RTB_Changelog.Text = "";
            RTB_Changelog.WordWrap = false;
            // 
            // About
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(576, 429);
            Controls.Add(L_Thanks);
            Controls.Add(TC_About);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(1059, 813);
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(592, 467);
            Name = "About";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "About";
            TC_About.ResumeLayout(false);
            Tab_Shortcuts.ResumeLayout(false);
            Tab_Changelog.ResumeLayout(false);
            ResumeLayout(false);
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
