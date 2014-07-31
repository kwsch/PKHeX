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
            this.B_Cancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.nud = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.flag_0001 = new System.Windows.Forms.CheckBox();
            this.flag_0002 = new System.Windows.Forms.CheckBox();
            this.flag_0003 = new System.Windows.Forms.CheckBox();
            this.flag_0004 = new System.Windows.Forms.CheckBox();
            this.flag_0005 = new System.Windows.Forms.CheckBox();
            this.B_Save = new System.Windows.Forms.Button();
            this.flag_2237 = new System.Windows.Forms.CheckBox();
            this.flag_2238 = new System.Windows.Forms.CheckBox();
            this.flag_2239 = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.L_UnSet = new System.Windows.Forms.Label();
            this.L_IsSet = new System.Windows.Forms.Label();
            this.TB_NewSAV = new System.Windows.Forms.TextBox();
            this.TB_OldSAV = new System.Windows.Forms.TextBox();
            this.TB_UnSet = new System.Windows.Forms.TextBox();
            this.TB_IsSet = new System.Windows.Forms.TextBox();
            this.B_LoadNew = new System.Windows.Forms.Button();
            this.B_LoadOld = new System.Windows.Forms.Button();
            this.flag_0963 = new System.Windows.Forms.CheckBox();
            this.flag_0115 = new System.Windows.Forms.CheckBox();
            this.GB_Rebattle = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.GB_Rebattle.SuspendLayout();
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
            this.CHK_CustomFlag.CheckedChanged += new System.EventHandler(this.changeCustomBool);
            // 
            // B_Cancel
            // 
            this.B_Cancel.Location = new System.Drawing.Point(128, 180);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(75, 23);
            this.B_Cancel.TabIndex = 2;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
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
            3071,
            0,
            0,
            0});
            this.nud.ValueChanged += new System.EventHandler(this.changeCustomFlag);
            this.nud.KeyUp += new System.Windows.Forms.KeyEventHandler(this.changeCustomFlag);
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
            this.flag_0001.Location = new System.Drawing.Point(12, 12);
            this.flag_0001.Name = "flag_0001";
            this.flag_0001.Size = new System.Drawing.Size(52, 17);
            this.flag_0001.TabIndex = 4;
            this.flag_0001.Text = "Flag1";
            this.flag_0001.UseVisualStyleBackColor = true;
            this.flag_0001.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_0002
            // 
            this.flag_0002.AutoSize = true;
            this.flag_0002.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.flag_0002.Location = new System.Drawing.Point(12, 28);
            this.flag_0002.Name = "flag_0002";
            this.flag_0002.Size = new System.Drawing.Size(52, 17);
            this.flag_0002.TabIndex = 5;
            this.flag_0002.Text = "Flag2";
            this.flag_0002.UseVisualStyleBackColor = true;
            this.flag_0002.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_0003
            // 
            this.flag_0003.AutoSize = true;
            this.flag_0003.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.flag_0003.Location = new System.Drawing.Point(12, 44);
            this.flag_0003.Name = "flag_0003";
            this.flag_0003.Size = new System.Drawing.Size(52, 17);
            this.flag_0003.TabIndex = 6;
            this.flag_0003.Text = "Flag3";
            this.flag_0003.UseVisualStyleBackColor = true;
            this.flag_0003.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_0004
            // 
            this.flag_0004.AutoSize = true;
            this.flag_0004.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.flag_0004.Location = new System.Drawing.Point(12, 60);
            this.flag_0004.Name = "flag_0004";
            this.flag_0004.Size = new System.Drawing.Size(52, 17);
            this.flag_0004.TabIndex = 7;
            this.flag_0004.Text = "Flag4";
            this.flag_0004.UseVisualStyleBackColor = true;
            this.flag_0004.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_0005
            // 
            this.flag_0005.AutoSize = true;
            this.flag_0005.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.flag_0005.Location = new System.Drawing.Point(12, 76);
            this.flag_0005.Name = "flag_0005";
            this.flag_0005.Size = new System.Drawing.Size(52, 17);
            this.flag_0005.TabIndex = 8;
            this.flag_0005.Text = "Flag5";
            this.flag_0005.UseVisualStyleBackColor = true;
            this.flag_0005.Click += new System.EventHandler(this.toggleFlag);
            // 
            // B_Save
            // 
            this.B_Save.Location = new System.Drawing.Point(207, 179);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(75, 23);
            this.B_Save.TabIndex = 9;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // flag_2237
            // 
            this.flag_2237.AutoSize = true;
            this.flag_2237.Location = new System.Drawing.Point(72, 12);
            this.flag_2237.Name = "flag_2237";
            this.flag_2237.Size = new System.Drawing.Size(50, 17);
            this.flag_2237.TabIndex = 10;
            this.flag_2237.Text = "2237";
            this.flag_2237.UseVisualStyleBackColor = true;
            this.flag_2237.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_2238
            // 
            this.flag_2238.AutoSize = true;
            this.flag_2238.Location = new System.Drawing.Point(72, 28);
            this.flag_2238.Name = "flag_2238";
            this.flag_2238.Size = new System.Drawing.Size(50, 17);
            this.flag_2238.TabIndex = 11;
            this.flag_2238.Text = "2238";
            this.flag_2238.UseVisualStyleBackColor = true;
            this.flag_2238.Click += new System.EventHandler(this.toggleFlag);
            // 
            // flag_2239
            // 
            this.flag_2239.AutoSize = true;
            this.flag_2239.Location = new System.Drawing.Point(72, 44);
            this.flag_2239.Name = "flag_2239";
            this.flag_2239.Size = new System.Drawing.Size(50, 17);
            this.flag_2239.TabIndex = 12;
            this.flag_2239.Text = "2239";
            this.flag_2239.UseVisualStyleBackColor = true;
            this.flag_2239.Click += new System.EventHandler(this.toggleFlag);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.L_UnSet);
            this.groupBox2.Controls.Add(this.L_IsSet);
            this.groupBox2.Controls.Add(this.TB_NewSAV);
            this.groupBox2.Controls.Add(this.TB_OldSAV);
            this.groupBox2.Controls.Add(this.TB_UnSet);
            this.groupBox2.Controls.Add(this.TB_IsSet);
            this.groupBox2.Controls.Add(this.B_LoadNew);
            this.groupBox2.Controls.Add(this.B_LoadOld);
            this.groupBox2.Location = new System.Drawing.Point(14, 210);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(268, 120);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "FlagDiff";
            // 
            // L_UnSet
            // 
            this.L_UnSet.AutoSize = true;
            this.L_UnSet.Location = new System.Drawing.Point(10, 97);
            this.L_UnSet.Name = "L_UnSet";
            this.L_UnSet.Size = new System.Drawing.Size(37, 13);
            this.L_UnSet.TabIndex = 7;
            this.L_UnSet.Text = "UnSet";
            // 
            // L_IsSet
            // 
            this.L_IsSet.AutoSize = true;
            this.L_IsSet.Location = new System.Drawing.Point(12, 77);
            this.L_IsSet.Name = "L_IsSet";
            this.L_IsSet.Size = new System.Drawing.Size(31, 13);
            this.L_IsSet.TabIndex = 6;
            this.L_IsSet.Text = "IsSet";
            // 
            // TB_NewSAV
            // 
            this.TB_NewSAV.Location = new System.Drawing.Point(93, 47);
            this.TB_NewSAV.Name = "TB_NewSAV";
            this.TB_NewSAV.ReadOnly = true;
            this.TB_NewSAV.Size = new System.Drawing.Size(169, 20);
            this.TB_NewSAV.TabIndex = 5;
            this.TB_NewSAV.TextChanged += new System.EventHandler(this.changeSAV);
            // 
            // TB_OldSAV
            // 
            this.TB_OldSAV.Location = new System.Drawing.Point(93, 21);
            this.TB_OldSAV.Name = "TB_OldSAV";
            this.TB_OldSAV.ReadOnly = true;
            this.TB_OldSAV.Size = new System.Drawing.Size(169, 20);
            this.TB_OldSAV.TabIndex = 4;
            this.TB_OldSAV.TextChanged += new System.EventHandler(this.changeSAV);
            // 
            // TB_UnSet
            // 
            this.TB_UnSet.Location = new System.Drawing.Point(56, 94);
            this.TB_UnSet.Name = "TB_UnSet";
            this.TB_UnSet.ReadOnly = true;
            this.TB_UnSet.Size = new System.Drawing.Size(206, 20);
            this.TB_UnSet.TabIndex = 3;
            // 
            // TB_IsSet
            // 
            this.TB_IsSet.Location = new System.Drawing.Point(56, 74);
            this.TB_IsSet.Name = "TB_IsSet";
            this.TB_IsSet.ReadOnly = true;
            this.TB_IsSet.Size = new System.Drawing.Size(206, 20);
            this.TB_IsSet.TabIndex = 2;
            // 
            // B_LoadNew
            // 
            this.B_LoadNew.Location = new System.Drawing.Point(12, 45);
            this.B_LoadNew.Name = "B_LoadNew";
            this.B_LoadNew.Size = new System.Drawing.Size(75, 23);
            this.B_LoadNew.TabIndex = 1;
            this.B_LoadNew.Text = "Load New";
            this.B_LoadNew.UseVisualStyleBackColor = true;
            this.B_LoadNew.Click += new System.EventHandler(this.openSAV);
            // 
            // B_LoadOld
            // 
            this.B_LoadOld.Location = new System.Drawing.Point(12, 19);
            this.B_LoadOld.Name = "B_LoadOld";
            this.B_LoadOld.Size = new System.Drawing.Size(75, 23);
            this.B_LoadOld.TabIndex = 0;
            this.B_LoadOld.Text = "Load Old";
            this.B_LoadOld.UseVisualStyleBackColor = true;
            this.B_LoadOld.Click += new System.EventHandler(this.openSAV);
            // 
            // flag_0963
            // 
            this.flag_0963.AutoSize = true;
            this.flag_0963.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.flag_0963.Location = new System.Drawing.Point(16, 19);
            this.flag_0963.Name = "flag_0963";
            this.flag_0963.Size = new System.Drawing.Size(113, 17);
            this.flag_0963.TabIndex = 14;
            this.flag_0963.Text = "Mewtwo Defeated";
            this.flag_0963.UseVisualStyleBackColor = true;
            // 
            // flag_0115
            // 
            this.flag_0115.AutoSize = true;
            this.flag_0115.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.flag_0115.Location = new System.Drawing.Point(16, 34);
            this.flag_0115.Name = "flag_0115";
            this.flag_0115.Size = new System.Drawing.Size(112, 17);
            this.flag_0115.TabIndex = 15;
            this.flag_0115.Text = "Mewtwo Captured";
            this.flag_0115.UseVisualStyleBackColor = true;
            // 
            // GB_Rebattle
            // 
            this.GB_Rebattle.Controls.Add(this.flag_0115);
            this.GB_Rebattle.Controls.Add(this.flag_0963);
            this.GB_Rebattle.Location = new System.Drawing.Point(129, 12);
            this.GB_Rebattle.Name = "GB_Rebattle";
            this.GB_Rebattle.Size = new System.Drawing.Size(152, 156);
            this.GB_Rebattle.TabIndex = 16;
            this.GB_Rebattle.TabStop = false;
            this.GB_Rebattle.Text = "Rebattle";
            // 
            // SAV_EventFlags
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 342);
            this.Controls.Add(this.GB_Rebattle);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.flag_2239);
            this.Controls.Add(this.flag_2238);
            this.Controls.Add(this.flag_2237);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.flag_0005);
            this.Controls.Add(this.flag_0004);
            this.Controls.Add(this.flag_0003);
            this.Controls.Add(this.flag_0002);
            this.Controls.Add(this.flag_0001);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.B_Cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SAV_EventFlags";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Event Flag Editor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.GB_Rebattle.ResumeLayout(false);
            this.GB_Rebattle.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox CHK_CustomFlag;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox flag_0001;
        private System.Windows.Forms.CheckBox flag_0002;
        private System.Windows.Forms.CheckBox flag_0003;
        private System.Windows.Forms.CheckBox flag_0004;
        private System.Windows.Forms.CheckBox flag_0005;
        private System.Windows.Forms.NumericUpDown nud;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.CheckBox flag_2237;
        private System.Windows.Forms.CheckBox flag_2238;
        private System.Windows.Forms.CheckBox flag_2239;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label L_UnSet;
        private System.Windows.Forms.Label L_IsSet;
        private System.Windows.Forms.TextBox TB_NewSAV;
        private System.Windows.Forms.TextBox TB_OldSAV;
        private System.Windows.Forms.TextBox TB_UnSet;
        private System.Windows.Forms.TextBox TB_IsSet;
        private System.Windows.Forms.Button B_LoadNew;
        private System.Windows.Forms.Button B_LoadOld;
        private System.Windows.Forms.CheckBox flag_0963;
        private System.Windows.Forms.CheckBox flag_0115;
        private System.Windows.Forms.GroupBox GB_Rebattle;
    }
}