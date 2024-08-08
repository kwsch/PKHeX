namespace PKHeX.WinForms
{
    partial class SAV_Misc2
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
            B_Save = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            B_VirtualConsoleGSBall = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(248, 220);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(88, 27);
            B_Save.TabIndex = 73;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(154, 220);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(88, 27);
            B_Cancel.TabIndex = 72;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_VirtualConsoleGSBall
            // 
            B_VirtualConsoleGSBall.Location = new System.Drawing.Point(12, 12);
            B_VirtualConsoleGSBall.Name = "B_VirtualConsoleGSBall";
            B_VirtualConsoleGSBall.Size = new System.Drawing.Size(160, 64);
            B_VirtualConsoleGSBall.TabIndex = 74;
            B_VirtualConsoleGSBall.Text = "Enable GS Ball Event (Virtual Console)";
            B_VirtualConsoleGSBall.UseVisualStyleBackColor = true;
            B_VirtualConsoleGSBall.Click += B_VirtualConsoleGSBall_Click;
            // 
            // SAV_Misc2
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(344, 261);
            Controls.Add(B_VirtualConsoleGSBall);
            Controls.Add(B_Save);
            Controls.Add(B_Cancel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimumSize = new System.Drawing.Size(231, 167);
            Name = "SAV_Misc2";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Misc Editor";
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_VirtualConsoleGSBall;
    }
}
