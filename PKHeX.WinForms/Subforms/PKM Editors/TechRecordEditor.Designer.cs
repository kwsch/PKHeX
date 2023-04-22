namespace PKHeX.WinForms
{
    partial class TechRecordEditor
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
            components = new System.ComponentModel.Container();
            B_Save = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            B_None = new System.Windows.Forms.Button();
            B_All = new System.Windows.Forms.Button();
            tipName = new System.Windows.Forms.ToolTip(components);
            CLB_Flags = new System.Windows.Forms.CheckedListBox();
            SuspendLayout();
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_Save.Location = new System.Drawing.Point(126, 398);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(105, 27);
            B_Save.TabIndex = 1;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_Cancel.Location = new System.Drawing.Point(14, 398);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(105, 27);
            B_Cancel.TabIndex = 2;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_None
            // 
            B_None.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_None.Location = new System.Drawing.Point(126, 365);
            B_None.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_None.Name = "B_None";
            B_None.Size = new System.Drawing.Size(105, 27);
            B_None.TabIndex = 5;
            B_None.Text = "Remove All";
            B_None.UseVisualStyleBackColor = true;
            B_None.Click += B_None_Click;
            // 
            // B_All
            // 
            B_All.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_All.Location = new System.Drawing.Point(14, 365);
            B_All.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_All.Name = "B_All";
            B_All.Size = new System.Drawing.Size(105, 27);
            B_All.TabIndex = 4;
            B_All.Text = "Give All";
            B_All.UseVisualStyleBackColor = true;
            B_All.Click += B_All_Click;
            // 
            // CLB_Flags
            // 
            CLB_Flags.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            CLB_Flags.CheckOnClick = true;
            CLB_Flags.FormattingEnabled = true;
            CLB_Flags.Location = new System.Drawing.Point(14, 14);
            CLB_Flags.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CLB_Flags.Name = "CLB_Flags";
            CLB_Flags.Size = new System.Drawing.Size(216, 328);
            CLB_Flags.TabIndex = 6;
            // 
            // TechRecordEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(245, 438);
            Controls.Add(CLB_Flags);
            Controls.Add(B_None);
            Controls.Add(B_All);
            Controls.Add(B_Cancel);
            Controls.Add(B_Save);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "TechRecordEditor";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "TR Relearn Editor";
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_None;
        private System.Windows.Forms.Button B_All;
        private System.Windows.Forms.ToolTip tipName;
        private System.Windows.Forms.CheckedListBox CLB_Flags;
    }
}
