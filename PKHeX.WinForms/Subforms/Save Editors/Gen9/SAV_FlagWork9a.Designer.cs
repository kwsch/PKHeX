namespace PKHeX.WinForms
{
    sealed partial class SAV_FlagWork9a
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
            B_Cancel = new System.Windows.Forms.Button();
            B_Save = new System.Windows.Forms.Button();
            L_EventFlagWarn = new System.Windows.Forms.Label();
            GB_Research = new System.Windows.Forms.TabPage();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            B_LoadOld = new System.Windows.Forms.Button();
            TB_NewSAV = new System.Windows.Forms.TextBox();
            B_LoadNew = new System.Windows.Forms.Button();
            TB_OldSAV = new System.Windows.Forms.TextBox();
            RTB_Diff = new System.Windows.Forms.RichTextBox();
            TC_Features = new System.Windows.Forms.TabControl();
            GB_Research.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            TC_Features.SuspendLayout();
            SuspendLayout();
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(507, 401);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(88, 27);
            B_Cancel.TabIndex = 2;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(603, 401);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(88, 27);
            B_Save.TabIndex = 9;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // L_EventFlagWarn
            // 
            L_EventFlagWarn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            L_EventFlagWarn.ForeColor = System.Drawing.Color.Red;
            L_EventFlagWarn.Location = new System.Drawing.Point(13, 396);
            L_EventFlagWarn.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_EventFlagWarn.Name = "L_EventFlagWarn";
            L_EventFlagWarn.Size = new System.Drawing.Size(306, 36);
            L_EventFlagWarn.TabIndex = 41;
            L_EventFlagWarn.Text = "Altering Event Flags may impact other story events. Save file backups are recommended.";
            L_EventFlagWarn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GB_Research
            // 
            GB_Research.Controls.Add(splitContainer1);
            GB_Research.Location = new System.Drawing.Point(4, 26);
            GB_Research.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Research.Name = "GB_Research";
            GB_Research.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Research.Size = new System.Drawing.Size(678, 351);
            GB_Research.TabIndex = 2;
            GB_Research.Text = "Research";
            GB_Research.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(4, 3);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(B_LoadOld);
            splitContainer1.Panel1.Controls.Add(TB_NewSAV);
            splitContainer1.Panel1.Controls.Add(B_LoadNew);
            splitContainer1.Panel1.Controls.Add(TB_OldSAV);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(RTB_Diff);
            splitContainer1.Size = new System.Drawing.Size(670, 345);
            splitContainer1.SplitterDistance = 60;
            splitContainer1.TabIndex = 7;
            // 
            // B_LoadOld
            // 
            B_LoadOld.Location = new System.Drawing.Point(4, 3);
            B_LoadOld.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_LoadOld.Name = "B_LoadOld";
            B_LoadOld.Size = new System.Drawing.Size(88, 27);
            B_LoadOld.TabIndex = 0;
            B_LoadOld.Text = "Load Old";
            B_LoadOld.UseVisualStyleBackColor = true;
            B_LoadOld.Click += OpenSAV;
            // 
            // TB_NewSAV
            // 
            TB_NewSAV.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TB_NewSAV.Location = new System.Drawing.Point(98, 35);
            TB_NewSAV.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_NewSAV.Name = "TB_NewSAV";
            TB_NewSAV.ReadOnly = true;
            TB_NewSAV.Size = new System.Drawing.Size(567, 25);
            TB_NewSAV.TabIndex = 5;
            // 
            // B_LoadNew
            // 
            B_LoadNew.Location = new System.Drawing.Point(4, 33);
            B_LoadNew.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_LoadNew.Name = "B_LoadNew";
            B_LoadNew.Size = new System.Drawing.Size(88, 27);
            B_LoadNew.TabIndex = 1;
            B_LoadNew.Text = "Load New";
            B_LoadNew.UseVisualStyleBackColor = true;
            B_LoadNew.Click += OpenSAV;
            // 
            // TB_OldSAV
            // 
            TB_OldSAV.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TB_OldSAV.Location = new System.Drawing.Point(98, 5);
            TB_OldSAV.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_OldSAV.Name = "TB_OldSAV";
            TB_OldSAV.ReadOnly = true;
            TB_OldSAV.Size = new System.Drawing.Size(567, 25);
            TB_OldSAV.TabIndex = 4;
            // 
            // RTB_Diff
            // 
            RTB_Diff.Dock = System.Windows.Forms.DockStyle.Fill;
            RTB_Diff.Location = new System.Drawing.Point(0, 0);
            RTB_Diff.Margin = new System.Windows.Forms.Padding(0);
            RTB_Diff.Name = "RTB_Diff";
            RTB_Diff.ReadOnly = true;
            RTB_Diff.Size = new System.Drawing.Size(670, 281);
            RTB_Diff.TabIndex = 6;
            RTB_Diff.Text = "";
            // 
            // TC_Features
            // 
            TC_Features.AllowDrop = true;
            TC_Features.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TC_Features.Controls.Add(GB_Research);
            TC_Features.Location = new System.Drawing.Point(9, 9);
            TC_Features.Margin = new System.Windows.Forms.Padding(0);
            TC_Features.Name = "TC_Features";
            TC_Features.Padding = new System.Drawing.Point(0, 0);
            TC_Features.SelectedIndex = 0;
            TC_Features.Size = new System.Drawing.Size(686, 381);
            TC_Features.TabIndex = 42;
            // 
            // SAV_FlagWork9a
            // 
            AllowDrop = true;
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(704, 441);
            Controls.Add(TC_Features);
            Controls.Add(L_EventFlagWarn);
            Controls.Add(B_Save);
            Controls.Add(B_Cancel);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(640, 480);
            Name = "SAV_FlagWork9a";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Event Flag Editor";
            GB_Research.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            TC_Features.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Label L_EventFlagWarn;
        private System.Windows.Forms.TabPage GB_Research;
        private System.Windows.Forms.RichTextBox RTB_Diff;
        private System.Windows.Forms.TextBox TB_NewSAV;
        private System.Windows.Forms.TextBox TB_OldSAV;
        private System.Windows.Forms.Button B_LoadNew;
        private System.Windows.Forms.Button B_LoadOld;
        private System.Windows.Forms.TabControl TC_Features;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}
