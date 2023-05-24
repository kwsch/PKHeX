namespace PKHeX.WinForms
{
    partial class SAV_BerryFieldXY
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
            listBox1 = new System.Windows.Forms.ListBox();
            L_Unfinished = new System.Windows.Forms.Label();
            L_Field = new System.Windows.Forms.Label();
            B_Cancel = new System.Windows.Forms.Button();
            B_Save = new System.Windows.Forms.Button();
            TB_Berry = new System.Windows.Forms.TextBox();
            L_Berry = new System.Windows.Forms.Label();
            L_u1 = new System.Windows.Forms.Label();
            L_u2 = new System.Windows.Forms.Label();
            L_u3 = new System.Windows.Forms.Label();
            TB_u1 = new System.Windows.Forms.TextBox();
            TB_u2 = new System.Windows.Forms.TextBox();
            TB_u3 = new System.Windows.Forms.TextBox();
            TB_u7 = new System.Windows.Forms.TextBox();
            TB_u6 = new System.Windows.Forms.TextBox();
            TB_u5 = new System.Windows.Forms.TextBox();
            L_u7 = new System.Windows.Forms.Label();
            L_u6 = new System.Windows.Forms.Label();
            L_u5 = new System.Windows.Forms.Label();
            L_u4 = new System.Windows.Forms.Label();
            TB_u4 = new System.Windows.Forms.TextBox();
            SuspendLayout();
            // 
            // listBox1
            // 
            listBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Items.AddRange(new object[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36" });
            listBox1.Location = new System.Drawing.Point(14, 29);
            listBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            listBox1.Name = "listBox1";
            listBox1.Size = new System.Drawing.Size(44, 304);
            listBox1.TabIndex = 0;
            listBox1.SelectedIndexChanged += Changefield;
            // 
            // L_Unfinished
            // 
            L_Unfinished.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            L_Unfinished.AutoSize = true;
            L_Unfinished.Location = new System.Drawing.Point(78, 298);
            L_Unfinished.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Unfinished.Name = "L_Unfinished";
            L_Unfinished.Size = new System.Drawing.Size(189, 15);
            L_Unfinished.TabIndex = 1;
            L_Unfinished.Text = "Unfinished - Needs More Research";
            // 
            // L_Field
            // 
            L_Field.AutoSize = true;
            L_Field.Location = new System.Drawing.Point(14, 10);
            L_Field.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Field.Name = "L_Field";
            L_Field.Size = new System.Drawing.Size(32, 15);
            L_Field.TabIndex = 2;
            L_Field.Text = "Field";
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_Cancel.Location = new System.Drawing.Point(94, 316);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(88, 27);
            B_Cancel.TabIndex = 3;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_Save.Enabled = false;
            B_Save.Location = new System.Drawing.Point(196, 316);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(88, 27);
            B_Save.TabIndex = 4;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            // 
            // TB_Berry
            // 
            TB_Berry.Location = new System.Drawing.Point(114, 14);
            TB_Berry.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_Berry.Name = "TB_Berry";
            TB_Berry.ReadOnly = true;
            TB_Berry.Size = new System.Drawing.Size(116, 23);
            TB_Berry.TabIndex = 5;
            // 
            // L_Berry
            // 
            L_Berry.AutoSize = true;
            L_Berry.Location = new System.Drawing.Point(68, 17);
            L_Berry.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Berry.Name = "L_Berry";
            L_Berry.Size = new System.Drawing.Size(37, 15);
            L_Berry.TabIndex = 6;
            L_Berry.Text = "Berry:";
            // 
            // L_u1
            // 
            L_u1.AutoSize = true;
            L_u1.Location = new System.Drawing.Point(68, 47);
            L_u1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_u1.Name = "L_u1";
            L_u1.Size = new System.Drawing.Size(13, 15);
            L_u1.TabIndex = 7;
            L_u1.Text = "1";
            // 
            // L_u2
            // 
            L_u2.AutoSize = true;
            L_u2.Location = new System.Drawing.Point(68, 77);
            L_u2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_u2.Name = "L_u2";
            L_u2.Size = new System.Drawing.Size(13, 15);
            L_u2.TabIndex = 8;
            L_u2.Text = "2";
            // 
            // L_u3
            // 
            L_u3.AutoSize = true;
            L_u3.Location = new System.Drawing.Point(68, 107);
            L_u3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_u3.Name = "L_u3";
            L_u3.Size = new System.Drawing.Size(13, 15);
            L_u3.TabIndex = 9;
            L_u3.Text = "3";
            // 
            // TB_u1
            // 
            TB_u1.Location = new System.Drawing.Point(115, 44);
            TB_u1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_u1.Name = "TB_u1";
            TB_u1.ReadOnly = true;
            TB_u1.Size = new System.Drawing.Size(116, 23);
            TB_u1.TabIndex = 12;
            // 
            // TB_u2
            // 
            TB_u2.Location = new System.Drawing.Point(114, 74);
            TB_u2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_u2.Name = "TB_u2";
            TB_u2.ReadOnly = true;
            TB_u2.Size = new System.Drawing.Size(116, 23);
            TB_u2.TabIndex = 13;
            // 
            // TB_u3
            // 
            TB_u3.Location = new System.Drawing.Point(114, 104);
            TB_u3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_u3.Name = "TB_u3";
            TB_u3.ReadOnly = true;
            TB_u3.Size = new System.Drawing.Size(116, 23);
            TB_u3.TabIndex = 14;
            // 
            // TB_u7
            // 
            TB_u7.Location = new System.Drawing.Point(114, 224);
            TB_u7.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_u7.Name = "TB_u7";
            TB_u7.ReadOnly = true;
            TB_u7.Size = new System.Drawing.Size(116, 23);
            TB_u7.TabIndex = 22;
            // 
            // TB_u6
            // 
            TB_u6.Location = new System.Drawing.Point(114, 194);
            TB_u6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_u6.Name = "TB_u6";
            TB_u6.ReadOnly = true;
            TB_u6.Size = new System.Drawing.Size(116, 23);
            TB_u6.TabIndex = 21;
            // 
            // TB_u5
            // 
            TB_u5.Location = new System.Drawing.Point(115, 164);
            TB_u5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_u5.Name = "TB_u5";
            TB_u5.ReadOnly = true;
            TB_u5.Size = new System.Drawing.Size(116, 23);
            TB_u5.TabIndex = 20;
            // 
            // L_u7
            // 
            L_u7.AutoSize = true;
            L_u7.Location = new System.Drawing.Point(68, 227);
            L_u7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_u7.Name = "L_u7";
            L_u7.Size = new System.Drawing.Size(13, 15);
            L_u7.TabIndex = 19;
            L_u7.Text = "7";
            // 
            // L_u6
            // 
            L_u6.AutoSize = true;
            L_u6.Location = new System.Drawing.Point(68, 197);
            L_u6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_u6.Name = "L_u6";
            L_u6.Size = new System.Drawing.Size(13, 15);
            L_u6.TabIndex = 18;
            L_u6.Text = "6";
            // 
            // L_u5
            // 
            L_u5.AutoSize = true;
            L_u5.Location = new System.Drawing.Point(68, 167);
            L_u5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_u5.Name = "L_u5";
            L_u5.Size = new System.Drawing.Size(13, 15);
            L_u5.TabIndex = 17;
            L_u5.Text = "5";
            // 
            // L_u4
            // 
            L_u4.AutoSize = true;
            L_u4.Location = new System.Drawing.Point(68, 137);
            L_u4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_u4.Name = "L_u4";
            L_u4.Size = new System.Drawing.Size(13, 15);
            L_u4.TabIndex = 16;
            L_u4.Text = "4";
            // 
            // TB_u4
            // 
            TB_u4.Location = new System.Drawing.Point(114, 134);
            TB_u4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_u4.Name = "TB_u4";
            TB_u4.ReadOnly = true;
            TB_u4.Size = new System.Drawing.Size(116, 23);
            TB_u4.TabIndex = 15;
            // 
            // SAV_BerryFieldXY
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(299, 352);
            Controls.Add(TB_u7);
            Controls.Add(TB_u6);
            Controls.Add(TB_u5);
            Controls.Add(L_u7);
            Controls.Add(L_u6);
            Controls.Add(L_u5);
            Controls.Add(L_u4);
            Controls.Add(TB_u4);
            Controls.Add(TB_u3);
            Controls.Add(TB_u2);
            Controls.Add(TB_u1);
            Controls.Add(L_u3);
            Controls.Add(L_u2);
            Controls.Add(L_u1);
            Controls.Add(L_Berry);
            Controls.Add(TB_Berry);
            Controls.Add(B_Save);
            Controls.Add(B_Cancel);
            Controls.Add(L_Field);
            Controls.Add(L_Unfinished);
            Controls.Add(listBox1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_BerryFieldXY";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Berry Field Editor";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label L_Unfinished;
        private System.Windows.Forms.Label L_Field;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.TextBox TB_Berry;
        private System.Windows.Forms.Label L_Berry;
        private System.Windows.Forms.Label L_u1;
        private System.Windows.Forms.Label L_u2;
        private System.Windows.Forms.Label L_u3;
        private System.Windows.Forms.TextBox TB_u1;
        private System.Windows.Forms.TextBox TB_u2;
        private System.Windows.Forms.TextBox TB_u3;
        private System.Windows.Forms.TextBox TB_u7;
        private System.Windows.Forms.TextBox TB_u6;
        private System.Windows.Forms.TextBox TB_u5;
        private System.Windows.Forms.Label L_u7;
        private System.Windows.Forms.Label L_u6;
        private System.Windows.Forms.Label L_u5;
        private System.Windows.Forms.Label L_u4;
        private System.Windows.Forms.TextBox TB_u4;
    }
}
