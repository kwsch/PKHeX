namespace PKHeX
{
    partial class SAV_EventFlags
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SAV_EventFlags));
            this.CHK_CustomFlag = new System.Windows.Forms.CheckBox();
            this.B_Close = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.flag_0001 = new System.Windows.Forms.CheckBox();
            this.flag_0002 = new System.Windows.Forms.CheckBox();
            this.flag_0003 = new System.Windows.Forms.CheckBox();
            this.flag_0004 = new System.Windows.Forms.CheckBox();
            this.flag_0005 = new System.Windows.Forms.CheckBox();
            this.nud = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud)).BeginInit();
            this.SuspendLayout();
            // 
            // CHK_CustomFlag
            // 
            this.CHK_CustomFlag.AutoSize = true;
            this.CHK_CustomFlag.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CHK_CustomFlag.Enabled = false;
            this.CHK_CustomFlag.Location = new System.Drawing.Point(12, 44);
            this.CHK_CustomFlag.Name = "CHK_CustomFlag";
            this.CHK_CustomFlag.Size = new System.Drawing.Size(59, 17);
            this.CHK_CustomFlag.TabIndex = 1;
            this.CHK_CustomFlag.Text = "Status:";
            this.CHK_CustomFlag.UseVisualStyleBackColor = true;
            // 
            // B_Close
            // 
            this.B_Close.Location = new System.Drawing.Point(128, 180);
            this.B_Close.Name = "B_Close";
            this.B_Close.Size = new System.Drawing.Size(75, 23);
            this.B_Close.TabIndex = 2;
            this.B_Close.Text = "Close";
            this.B_Close.UseVisualStyleBackColor = true;
            this.B_Close.Click += new System.EventHandler(this.B_Close_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.nud);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.CHK_CustomFlag);
            this.groupBox1.Location = new System.Drawing.Point(14, 135);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(108, 68);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Check Flag Status";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Flag:";
            // 
            // flag_0001
            // 
            this.flag_0001.AutoSize = true;
            this.flag_0001.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.flag_0001.Enabled = false;
            this.flag_0001.Location = new System.Drawing.Point(12, 12);
            this.flag_0001.Name = "flag_0001";
            this.flag_0001.Size = new System.Drawing.Size(52, 17);
            this.flag_0001.TabIndex = 4;
            this.flag_0001.Text = "Flag1";
            this.flag_0001.UseVisualStyleBackColor = true;
            // 
            // flag_0002
            // 
            this.flag_0002.AutoSize = true;
            this.flag_0002.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.flag_0002.Enabled = false;
            this.flag_0002.Location = new System.Drawing.Point(12, 35);
            this.flag_0002.Name = "flag_0002";
            this.flag_0002.Size = new System.Drawing.Size(52, 17);
            this.flag_0002.TabIndex = 5;
            this.flag_0002.Text = "Flag2";
            this.flag_0002.UseVisualStyleBackColor = true;
            // 
            // flag_0003
            // 
            this.flag_0003.AutoSize = true;
            this.flag_0003.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.flag_0003.Enabled = false;
            this.flag_0003.Location = new System.Drawing.Point(12, 58);
            this.flag_0003.Name = "flag_0003";
            this.flag_0003.Size = new System.Drawing.Size(52, 17);
            this.flag_0003.TabIndex = 6;
            this.flag_0003.Text = "Flag3";
            this.flag_0003.UseVisualStyleBackColor = true;
            // 
            // flag_0004
            // 
            this.flag_0004.AutoSize = true;
            this.flag_0004.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.flag_0004.Enabled = false;
            this.flag_0004.Location = new System.Drawing.Point(12, 81);
            this.flag_0004.Name = "flag_0004";
            this.flag_0004.Size = new System.Drawing.Size(52, 17);
            this.flag_0004.TabIndex = 7;
            this.flag_0004.Text = "Flag4";
            this.flag_0004.UseVisualStyleBackColor = true;
            // 
            // flag_0005
            // 
            this.flag_0005.AutoSize = true;
            this.flag_0005.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.flag_0005.Enabled = false;
            this.flag_0005.Location = new System.Drawing.Point(12, 104);
            this.flag_0005.Name = "flag_0005";
            this.flag_0005.Size = new System.Drawing.Size(52, 17);
            this.flag_0005.TabIndex = 8;
            this.flag_0005.Text = "Flag5";
            this.flag_0005.UseVisualStyleBackColor = true;
            // 
            // nud
            // 
            this.nud.Location = new System.Drawing.Point(56, 19);
            this.nud.Maximum = new decimal(new int[] {
            3072,
            0,
            0,
            0});
            this.nud.Name = "nud";
            this.nud.Size = new System.Drawing.Size(45, 20);
            this.nud.TabIndex = 9;
            this.nud.Value = new decimal(new int[] {
            3072,
            0,
            0,
            0});
            this.nud.ValueChanged += new System.EventHandler(this.changeFlag);
            // 
            // SAV_EventFlags
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(214, 212);
            this.Controls.Add(this.flag_0005);
            this.Controls.Add(this.flag_0004);
            this.Controls.Add(this.flag_0003);
            this.Controls.Add(this.flag_0002);
            this.Controls.Add(this.flag_0001);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.B_Close);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SAV_EventFlags";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Event Flag Viewer";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox CHK_CustomFlag;
        private System.Windows.Forms.Button B_Close;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox flag_0001;
        private System.Windows.Forms.CheckBox flag_0002;
        private System.Windows.Forms.CheckBox flag_0003;
        private System.Windows.Forms.CheckBox flag_0004;
        private System.Windows.Forms.CheckBox flag_0005;
        private System.Windows.Forms.NumericUpDown nud;
    }
}