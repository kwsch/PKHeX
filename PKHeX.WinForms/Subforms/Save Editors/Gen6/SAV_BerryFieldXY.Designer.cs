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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.L_Unfinished = new System.Windows.Forms.Label();
            this.L_Field = new System.Windows.Forms.Label();
            this.B_Cancel = new System.Windows.Forms.Button();
            this.B_Save = new System.Windows.Forms.Button();
            this.TB_Berry = new System.Windows.Forms.TextBox();
            this.L_Berry = new System.Windows.Forms.Label();
            this.L_u1 = new System.Windows.Forms.Label();
            this.L_u2 = new System.Windows.Forms.Label();
            this.L_u3 = new System.Windows.Forms.Label();
            this.TB_u1 = new System.Windows.Forms.TextBox();
            this.TB_u2 = new System.Windows.Forms.TextBox();
            this.TB_u3 = new System.Windows.Forms.TextBox();
            this.TB_u7 = new System.Windows.Forms.TextBox();
            this.TB_u6 = new System.Windows.Forms.TextBox();
            this.TB_u5 = new System.Windows.Forms.TextBox();
            this.L_u7 = new System.Windows.Forms.Label();
            this.L_u6 = new System.Windows.Forms.Label();
            this.L_u5 = new System.Windows.Forms.Label();
            this.L_u4 = new System.Windows.Forms.Label();
            this.TB_u4 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Items.AddRange(new object[] {
            "01",
            "02",
            "03",
            "04",
            "05",
            "06",
            "07",
            "08",
            "09",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
            "32",
            "33",
            "34",
            "35",
            "36"});
            this.listBox1.Location = new System.Drawing.Point(12, 25);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(38, 264);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.Changefield);
            // 
            // L_Unfinished
            // 
            this.L_Unfinished.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.L_Unfinished.AutoSize = true;
            this.L_Unfinished.Location = new System.Drawing.Point(67, 258);
            this.L_Unfinished.Name = "L_Unfinished";
            this.L_Unfinished.Size = new System.Drawing.Size(173, 13);
            this.L_Unfinished.TabIndex = 1;
            this.L_Unfinished.Text = "Unfinished - Needs More Research";
            // 
            // L_Field
            // 
            this.L_Field.AutoSize = true;
            this.L_Field.Location = new System.Drawing.Point(12, 9);
            this.L_Field.Name = "L_Field";
            this.L_Field.Size = new System.Drawing.Size(29, 13);
            this.L_Field.TabIndex = 2;
            this.L_Field.Text = "Field";
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.B_Cancel.Location = new System.Drawing.Point(81, 274);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(75, 23);
            this.B_Cancel.TabIndex = 3;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.B_Save.Enabled = false;
            this.B_Save.Location = new System.Drawing.Point(168, 274);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(75, 23);
            this.B_Save.TabIndex = 4;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            // 
            // TB_Berry
            // 
            this.TB_Berry.Location = new System.Drawing.Point(98, 12);
            this.TB_Berry.Name = "TB_Berry";
            this.TB_Berry.ReadOnly = true;
            this.TB_Berry.Size = new System.Drawing.Size(100, 20);
            this.TB_Berry.TabIndex = 5;
            // 
            // L_Berry
            // 
            this.L_Berry.AutoSize = true;
            this.L_Berry.Location = new System.Drawing.Point(58, 15);
            this.L_Berry.Name = "L_Berry";
            this.L_Berry.Size = new System.Drawing.Size(34, 13);
            this.L_Berry.TabIndex = 6;
            this.L_Berry.Text = "Berry:";
            // 
            // L_u1
            // 
            this.L_u1.AutoSize = true;
            this.L_u1.Location = new System.Drawing.Point(58, 41);
            this.L_u1.Name = "L_u1";
            this.L_u1.Size = new System.Drawing.Size(13, 13);
            this.L_u1.TabIndex = 7;
            this.L_u1.Text = "1";
            // 
            // L_u2
            // 
            this.L_u2.AutoSize = true;
            this.L_u2.Location = new System.Drawing.Point(58, 67);
            this.L_u2.Name = "L_u2";
            this.L_u2.Size = new System.Drawing.Size(13, 13);
            this.L_u2.TabIndex = 8;
            this.L_u2.Text = "2";
            // 
            // L_u3
            // 
            this.L_u3.AutoSize = true;
            this.L_u3.Location = new System.Drawing.Point(58, 93);
            this.L_u3.Name = "L_u3";
            this.L_u3.Size = new System.Drawing.Size(13, 13);
            this.L_u3.TabIndex = 9;
            this.L_u3.Text = "3";
            // 
            // TB_u1
            // 
            this.TB_u1.Location = new System.Drawing.Point(99, 38);
            this.TB_u1.Name = "TB_u1";
            this.TB_u1.ReadOnly = true;
            this.TB_u1.Size = new System.Drawing.Size(100, 20);
            this.TB_u1.TabIndex = 12;
            // 
            // TB_u2
            // 
            this.TB_u2.Location = new System.Drawing.Point(98, 64);
            this.TB_u2.Name = "TB_u2";
            this.TB_u2.ReadOnly = true;
            this.TB_u2.Size = new System.Drawing.Size(100, 20);
            this.TB_u2.TabIndex = 13;
            // 
            // TB_u3
            // 
            this.TB_u3.Location = new System.Drawing.Point(98, 90);
            this.TB_u3.Name = "TB_u3";
            this.TB_u3.ReadOnly = true;
            this.TB_u3.Size = new System.Drawing.Size(100, 20);
            this.TB_u3.TabIndex = 14;
            // 
            // TB_u7
            // 
            this.TB_u7.Location = new System.Drawing.Point(98, 194);
            this.TB_u7.Name = "TB_u7";
            this.TB_u7.ReadOnly = true;
            this.TB_u7.Size = new System.Drawing.Size(100, 20);
            this.TB_u7.TabIndex = 22;
            // 
            // TB_u6
            // 
            this.TB_u6.Location = new System.Drawing.Point(98, 168);
            this.TB_u6.Name = "TB_u6";
            this.TB_u6.ReadOnly = true;
            this.TB_u6.Size = new System.Drawing.Size(100, 20);
            this.TB_u6.TabIndex = 21;
            // 
            // TB_u5
            // 
            this.TB_u5.Location = new System.Drawing.Point(99, 142);
            this.TB_u5.Name = "TB_u5";
            this.TB_u5.ReadOnly = true;
            this.TB_u5.Size = new System.Drawing.Size(100, 20);
            this.TB_u5.TabIndex = 20;
            // 
            // L_u7
            // 
            this.L_u7.AutoSize = true;
            this.L_u7.Location = new System.Drawing.Point(58, 197);
            this.L_u7.Name = "L_u7";
            this.L_u7.Size = new System.Drawing.Size(13, 13);
            this.L_u7.TabIndex = 19;
            this.L_u7.Text = "7";
            // 
            // L_u6
            // 
            this.L_u6.AutoSize = true;
            this.L_u6.Location = new System.Drawing.Point(58, 171);
            this.L_u6.Name = "L_u6";
            this.L_u6.Size = new System.Drawing.Size(13, 13);
            this.L_u6.TabIndex = 18;
            this.L_u6.Text = "6";
            // 
            // L_u5
            // 
            this.L_u5.AutoSize = true;
            this.L_u5.Location = new System.Drawing.Point(58, 145);
            this.L_u5.Name = "L_u5";
            this.L_u5.Size = new System.Drawing.Size(13, 13);
            this.L_u5.TabIndex = 17;
            this.L_u5.Text = "5";
            // 
            // L_u4
            // 
            this.L_u4.AutoSize = true;
            this.L_u4.Location = new System.Drawing.Point(58, 119);
            this.L_u4.Name = "L_u4";
            this.L_u4.Size = new System.Drawing.Size(13, 13);
            this.L_u4.TabIndex = 16;
            this.L_u4.Text = "4";
            // 
            // TB_u4
            // 
            this.TB_u4.Location = new System.Drawing.Point(98, 116);
            this.TB_u4.Name = "TB_u4";
            this.TB_u4.ReadOnly = true;
            this.TB_u4.Size = new System.Drawing.Size(100, 20);
            this.TB_u4.TabIndex = 15;
            // 
            // SAV_BerryField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(256, 305);
            this.Controls.Add(this.TB_u7);
            this.Controls.Add(this.TB_u6);
            this.Controls.Add(this.TB_u5);
            this.Controls.Add(this.L_u7);
            this.Controls.Add(this.L_u6);
            this.Controls.Add(this.L_u5);
            this.Controls.Add(this.L_u4);
            this.Controls.Add(this.TB_u4);
            this.Controls.Add(this.TB_u3);
            this.Controls.Add(this.TB_u2);
            this.Controls.Add(this.TB_u1);
            this.Controls.Add(this.L_u3);
            this.Controls.Add(this.L_u2);
            this.Controls.Add(this.L_u1);
            this.Controls.Add(this.L_Berry);
            this.Controls.Add(this.TB_Berry);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_Cancel);
            this.Controls.Add(this.L_Field);
            this.Controls.Add(this.L_Unfinished);
            this.Controls.Add(this.listBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SAV_BerryField";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Berry Field Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

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