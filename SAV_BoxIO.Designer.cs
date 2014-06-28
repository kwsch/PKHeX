namespace PKHeX
{
    partial class SAV_BoxIO
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SAV_BoxIO));
            this.B_ImportBox = new System.Windows.Forms.Button();
            this.CB_Box = new System.Windows.Forms.ComboBox();
            this.B_ExportBox = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // B_ImportBox
            // 
            this.B_ImportBox.Location = new System.Drawing.Point(109, 17);
            this.B_ImportBox.Name = "B_ImportBox";
            this.B_ImportBox.Size = new System.Drawing.Size(73, 23);
            this.B_ImportBox.TabIndex = 1;
            this.B_ImportBox.Text = "Import Box";
            this.B_ImportBox.UseVisualStyleBackColor = true;
            this.B_ImportBox.Click += new System.EventHandler(this.B_ImportBox_Click);
            // 
            // CB_Box
            // 
            this.CB_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Box.FormattingEnabled = true;
            this.CB_Box.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
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
            "31"});
            this.CB_Box.Location = new System.Drawing.Point(6, 33);
            this.CB_Box.Name = "CB_Box";
            this.CB_Box.Size = new System.Drawing.Size(97, 21);
            this.CB_Box.TabIndex = 4;
            // 
            // B_ExportBox
            // 
            this.B_ExportBox.Location = new System.Drawing.Point(109, 46);
            this.B_ExportBox.Name = "B_ExportBox";
            this.B_ExportBox.Size = new System.Drawing.Size(73, 23);
            this.B_ExportBox.TabIndex = 7;
            this.B_ExportBox.Text = "Export Box";
            this.B_ExportBox.UseVisualStyleBackColor = true;
            this.B_ExportBox.Click += new System.EventHandler(this.B_ExportBox_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CB_Box);
            this.groupBox1.Controls.Add(this.B_ExportBox);
            this.groupBox1.Controls.Add(this.B_ImportBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(190, 80);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // SAV_BoxIO
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(214, 92);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SAV_BoxIO";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Box Import/Export";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button B_ImportBox;
        private System.Windows.Forms.ComboBox CB_Box;
        private System.Windows.Forms.Button B_ExportBox;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}