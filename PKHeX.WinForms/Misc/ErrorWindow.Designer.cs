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
            this.T_ExceptionDetails = new System.Windows.Forms.TextBox();
            this.L_Message = new System.Windows.Forms.Label();
            this.L_ProvideInfo = new System.Windows.Forms.Label();
            this.B_CopyToClipboard = new System.Windows.Forms.Button();
            this.B_Abort = new System.Windows.Forms.Button();
            this.B_Continue = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // T_ExceptionDetails
            // 
            this.T_ExceptionDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.T_ExceptionDetails.Location = new System.Drawing.Point(12, 52);
            this.T_ExceptionDetails.Multiline = true;
            this.T_ExceptionDetails.Name = "T_ExceptionDetails";
            this.T_ExceptionDetails.ReadOnly = true;
            this.T_ExceptionDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.T_ExceptionDetails.Size = new System.Drawing.Size(475, 143);
            this.T_ExceptionDetails.TabIndex = 0;
            // 
            // L_Message
            // 
            this.L_Message.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.L_Message.Location = new System.Drawing.Point(9, 9);
            this.L_Message.Name = "L_Message";
            this.L_Message.Size = new System.Drawing.Size(478, 27);
            this.L_Message.TabIndex = 1;
            this.L_Message.Text = "An unknown error has occurred.";
            // 
            // L_ProvideInfo
            // 
            this.L_ProvideInfo.AutoSize = true;
            this.L_ProvideInfo.Location = new System.Drawing.Point(9, 36);
            this.L_ProvideInfo.Name = "L_ProvideInfo";
            this.L_ProvideInfo.Size = new System.Drawing.Size(269, 13);
            this.L_ProvideInfo.TabIndex = 2;
            this.L_ProvideInfo.Text = "Please provide this information when reporting this error:";
            // 
            // B_CopyToClipboard
            // 
            this.B_CopyToClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.B_CopyToClipboard.Location = new System.Drawing.Point(12, 201);
            this.B_CopyToClipboard.Name = "B_CopyToClipboard";
            this.B_CopyToClipboard.Size = new System.Drawing.Size(164, 23);
            this.B_CopyToClipboard.TabIndex = 3;
            this.B_CopyToClipboard.Text = "Copy to Clipboard";
            this.B_CopyToClipboard.UseVisualStyleBackColor = true;
            this.B_CopyToClipboard.Click += new System.EventHandler(this.ClickCopyException);
            // 
            // B_Abort
            // 
            this.B_Abort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Abort.Location = new System.Drawing.Point(412, 201);
            this.B_Abort.Name = "B_Abort";
            this.B_Abort.Size = new System.Drawing.Size(75, 23);
            this.B_Abort.TabIndex = 4;
            this.B_Abort.Text = "Abort";
            this.B_Abort.UseVisualStyleBackColor = true;
            this.B_Abort.Click += new System.EventHandler(this.ClickAbort);
            // 
            // B_Continue
            // 
            this.B_Continue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Continue.Location = new System.Drawing.Point(331, 201);
            this.B_Continue.Name = "B_Continue";
            this.B_Continue.Size = new System.Drawing.Size(75, 23);
            this.B_Continue.TabIndex = 5;
            this.B_Continue.Text = "Continue";
            this.B_Continue.UseVisualStyleBackColor = true;
            this.B_Continue.Click += new System.EventHandler(this.ClickContinue);
            // 
            // ErrorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(499, 236);
            this.Controls.Add(this.B_Continue);
            this.Controls.Add(this.B_Abort);
            this.Controls.Add(this.B_CopyToClipboard);
            this.Controls.Add(this.L_ProvideInfo);
            this.Controls.Add(this.L_Message);
            this.Controls.Add(this.T_ExceptionDetails);
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(515, 275);
            this.Name = "ErrorWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Error";
            this.ResumeLayout(false);
            this.PerformLayout();

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