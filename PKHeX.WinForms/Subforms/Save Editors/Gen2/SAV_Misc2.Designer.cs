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
            TAB_Main = new System.Windows.Forms.TabPage();
            GB_FlyDest = new System.Windows.Forms.GroupBox();
            B_AllFlyDest = new System.Windows.Forms.Button();
            CLB_FlyDest = new System.Windows.Forms.CheckedListBox();
            tabControl1 = new System.Windows.Forms.TabControl();
            TAB_Main.SuspendLayout();
            GB_FlyDest.SuspendLayout();
            tabControl1.SuspendLayout();
            SuspendLayout();
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(420, 504);
            B_Save.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(125, 44);
            B_Save.TabIndex = 73;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(285, 504);
            B_Cancel.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(125, 44);
            B_Cancel.TabIndex = 72;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // TAB_Main
            // 
            TAB_Main.Controls.Add(GB_FlyDest);
            TAB_Main.Location = new System.Drawing.Point(4, 34);
            TAB_Main.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            TAB_Main.Name = "TAB_Main";
            TAB_Main.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            TAB_Main.Size = new System.Drawing.Size(514, 431);
            TAB_Main.TabIndex = 0;
            TAB_Main.Text = "Main";
            TAB_Main.UseVisualStyleBackColor = true;
            // 
            // GB_FlyDest
            // 
            GB_FlyDest.Controls.Add(B_AllFlyDest);
            GB_FlyDest.Controls.Add(CLB_FlyDest);
            GB_FlyDest.Location = new System.Drawing.Point(20, 12);
            GB_FlyDest.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            GB_FlyDest.Name = "GB_FlyDest";
            GB_FlyDest.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            GB_FlyDest.Size = new System.Drawing.Size(233, 407);
            GB_FlyDest.TabIndex = 5;
            GB_FlyDest.TabStop = false;
            GB_FlyDest.Text = "Fly Destination";
            // 
            // B_AllFlyDest
            // 
            B_AllFlyDest.Location = new System.Drawing.Point(8, 40);
            B_AllFlyDest.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            B_AllFlyDest.Name = "B_AllFlyDest";
            B_AllFlyDest.Size = new System.Drawing.Size(125, 48);
            B_AllFlyDest.TabIndex = 0;
            B_AllFlyDest.Text = "Check All";
            B_AllFlyDest.UseVisualStyleBackColor = true;
            B_AllFlyDest.Click += B_AllFlyDest_Click;
            // 
            // CLB_FlyDest
            // 
            CLB_FlyDest.CheckOnClick = true;
            CLB_FlyDest.FormattingEnabled = true;
            CLB_FlyDest.Location = new System.Drawing.Point(10, 104);
            CLB_FlyDest.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            CLB_FlyDest.Name = "CLB_FlyDest";
            CLB_FlyDest.Size = new System.Drawing.Size(211, 284);
            CLB_FlyDest.TabIndex = 1;
            // 
            // tabControl1
            // 
            tabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tabControl1.Controls.Add(TAB_Main);
            tabControl1.Location = new System.Drawing.Point(20, 23);
            tabControl1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(522, 469);
            tabControl1.TabIndex = 74;
            // 
            // SAV_Misc2
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(557, 571);
            Controls.Add(tabControl1);
            Controls.Add(B_Save);
            Controls.Add(B_Cancel);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            MaximizeBox = false;
            MinimumSize = new System.Drawing.Size(319, 237);
            Name = "SAV_Misc2";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Misc Editor";
            TAB_Main.ResumeLayout(false);
            GB_FlyDest.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.TabPage TAB_Main;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.GroupBox GB_FlyDest;
        private System.Windows.Forms.Button B_AllFlyDest;
        private System.Windows.Forms.CheckedListBox CLB_FlyDest;
    }
}
