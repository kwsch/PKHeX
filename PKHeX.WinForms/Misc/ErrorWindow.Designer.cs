namespace PKHeX.WinForms
{
    partial class ErrorWindow
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
            T_ExceptionDetails = new System.Windows.Forms.TextBox();
            L_Message = new System.Windows.Forms.Label();
            L_ProvideInfo = new System.Windows.Forms.Label();
            B_CopyToClipboard = new System.Windows.Forms.Button();
            B_Abort = new System.Windows.Forms.Button();
            B_Continue = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // T_ExceptionDetails
            // 
            T_ExceptionDetails.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            T_ExceptionDetails.Location = new System.Drawing.Point(14, 60);
            T_ExceptionDetails.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            T_ExceptionDetails.Multiline = true;
            T_ExceptionDetails.Name = "T_ExceptionDetails";
            T_ExceptionDetails.ReadOnly = true;
            T_ExceptionDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            T_ExceptionDetails.Size = new System.Drawing.Size(554, 164);
            T_ExceptionDetails.TabIndex = 0;
            // 
            // L_Message
            // 
            L_Message.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            L_Message.Location = new System.Drawing.Point(10, 10);
            L_Message.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Message.Name = "L_Message";
            L_Message.Size = new System.Drawing.Size(558, 31);
            L_Message.TabIndex = 1;
            L_Message.Text = "An unknown error has occurred.";
            // 
            // L_ProvideInfo
            // 
            L_ProvideInfo.AutoSize = true;
            L_ProvideInfo.Location = new System.Drawing.Point(10, 42);
            L_ProvideInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_ProvideInfo.Name = "L_ProvideInfo";
            L_ProvideInfo.Size = new System.Drawing.Size(308, 15);
            L_ProvideInfo.TabIndex = 2;
            L_ProvideInfo.Text = "Please provide this information when reporting this error:";
            // 
            // B_CopyToClipboard
            // 
            B_CopyToClipboard.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_CopyToClipboard.Location = new System.Drawing.Point(14, 232);
            B_CopyToClipboard.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_CopyToClipboard.Name = "B_CopyToClipboard";
            B_CopyToClipboard.Size = new System.Drawing.Size(191, 27);
            B_CopyToClipboard.TabIndex = 3;
            B_CopyToClipboard.Text = "Copy to Clipboard";
            B_CopyToClipboard.UseVisualStyleBackColor = true;
            B_CopyToClipboard.Click += ClickCopyException;
            // 
            // B_Abort
            // 
            B_Abort.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Abort.Location = new System.Drawing.Point(481, 232);
            B_Abort.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Abort.Name = "B_Abort";
            B_Abort.Size = new System.Drawing.Size(88, 27);
            B_Abort.TabIndex = 4;
            B_Abort.Text = "Abort";
            B_Abort.UseVisualStyleBackColor = true;
            B_Abort.Click += ClickAbort;
            // 
            // B_Continue
            // 
            B_Continue.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Continue.Location = new System.Drawing.Point(386, 232);
            B_Continue.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Continue.Name = "B_Continue";
            B_Continue.Size = new System.Drawing.Size(88, 27);
            B_Continue.TabIndex = 5;
            B_Continue.Text = "Continue";
            B_Continue.UseVisualStyleBackColor = true;
            B_Continue.Click += ClickContinue;
            // 
            // ErrorWindow
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(582, 272);
            Controls.Add(B_Continue);
            Controls.Add(B_Abort);
            Controls.Add(B_CopyToClipboard);
            Controls.Add(L_ProvideInfo);
            Controls.Add(L_Message);
            Controls.Add(T_ExceptionDetails);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimumSize = new System.Drawing.Size(598, 311);
            Name = "ErrorWindow";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Error";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox T_ExceptionDetails;
        private System.Windows.Forms.Label L_Message;
        private System.Windows.Forms.Label L_ProvideInfo;
        private System.Windows.Forms.Button B_CopyToClipboard;
        private System.Windows.Forms.Button B_Abort;
        private System.Windows.Forms.Button B_Continue;
    }
}
