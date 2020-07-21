namespace PKHeX.WinForms
{
    partial class SAV_RTC3
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
            this.B_Save = new System.Windows.Forms.Button();
            this.B_Cancel = new System.Windows.Forms.Button();
            this.B_Reset = new System.Windows.Forms.Button();
            this.GB_Passed = new System.Windows.Forms.GroupBox();
            this.L_ESecond = new System.Windows.Forms.Label();
            this.L_EMinute = new System.Windows.Forms.Label();
            this.L_EHour = new System.Windows.Forms.Label();
            this.L_EDay = new System.Windows.Forms.Label();
            this.NUD_ESecond = new System.Windows.Forms.NumericUpDown();
            this.NUD_EMinute = new System.Windows.Forms.NumericUpDown();
            this.NUD_EHour = new System.Windows.Forms.NumericUpDown();
            this.NUD_EDay = new System.Windows.Forms.NumericUpDown();
            this.GB_Initial = new System.Windows.Forms.GroupBox();
            this.L_ISecond = new System.Windows.Forms.Label();
            this.L_IMinute = new System.Windows.Forms.Label();
            this.L_IHour = new System.Windows.Forms.Label();
            this.L_IDay = new System.Windows.Forms.Label();
            this.NUD_ISecond = new System.Windows.Forms.NumericUpDown();
            this.NUD_IMinute = new System.Windows.Forms.NumericUpDown();
            this.NUD_IHour = new System.Windows.Forms.NumericUpDown();
            this.NUD_IDay = new System.Windows.Forms.NumericUpDown();
            this.B_BerryFix = new System.Windows.Forms.Button();
            this.GB_Passed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_ESecond)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_EMinute)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_EHour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_EDay)).BeginInit();
            this.GB_Initial.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_ISecond)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_IMinute)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_IHour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_IDay)).BeginInit();
            this.SuspendLayout();
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(246, 141);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(75, 23);
            this.B_Save.TabIndex = 73;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(165, 141);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(75, 23);
            this.B_Cancel.TabIndex = 72;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // B_Reset
            // 
            this.B_Reset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.B_Reset.Location = new System.Drawing.Point(12, 141);
            this.B_Reset.Name = "B_Reset";
            this.B_Reset.Size = new System.Drawing.Size(75, 23);
            this.B_Reset.TabIndex = 77;
            this.B_Reset.Text = "Reset RTC";
            this.B_Reset.UseVisualStyleBackColor = true;
            this.B_Reset.Click += new System.EventHandler(this.B_Reset_Click);
            // 
            // GB_Passed
            // 
            this.GB_Passed.Controls.Add(this.L_ESecond);
            this.GB_Passed.Controls.Add(this.L_EMinute);
            this.GB_Passed.Controls.Add(this.L_EHour);
            this.GB_Passed.Controls.Add(this.L_EDay);
            this.GB_Passed.Controls.Add(this.NUD_ESecond);
            this.GB_Passed.Controls.Add(this.NUD_EMinute);
            this.GB_Passed.Controls.Add(this.NUD_EHour);
            this.GB_Passed.Controls.Add(this.NUD_EDay);
            this.GB_Passed.Location = new System.Drawing.Point(169, 12);
            this.GB_Passed.Name = "GB_Passed";
            this.GB_Passed.Size = new System.Drawing.Size(151, 123);
            this.GB_Passed.TabIndex = 75;
            this.GB_Passed.TabStop = false;
            this.GB_Passed.Text = "Time Elapsed";
            // 
            // L_ESecond
            // 
            this.L_ESecond.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.L_ESecond.AutoSize = true;
            this.L_ESecond.Location = new System.Drawing.Point(75, 100);
            this.L_ESecond.Name = "L_ESecond";
            this.L_ESecond.Size = new System.Drawing.Size(49, 13);
            this.L_ESecond.TabIndex = 20;
            this.L_ESecond.Text = "Seconds";
            this.L_ESecond.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // L_EMinute
            // 
            this.L_EMinute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.L_EMinute.AutoSize = true;
            this.L_EMinute.Location = new System.Drawing.Point(75, 74);
            this.L_EMinute.Name = "L_EMinute";
            this.L_EMinute.Size = new System.Drawing.Size(44, 13);
            this.L_EMinute.TabIndex = 19;
            this.L_EMinute.Text = "Minutes";
            this.L_EMinute.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // L_EHour
            // 
            this.L_EHour.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.L_EHour.AutoSize = true;
            this.L_EHour.Location = new System.Drawing.Point(75, 48);
            this.L_EHour.Name = "L_EHour";
            this.L_EHour.Size = new System.Drawing.Size(35, 13);
            this.L_EHour.TabIndex = 18;
            this.L_EHour.Text = "Hours";
            this.L_EHour.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // L_EDay
            // 
            this.L_EDay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.L_EDay.AutoSize = true;
            this.L_EDay.Location = new System.Drawing.Point(75, 22);
            this.L_EDay.Name = "L_EDay";
            this.L_EDay.Size = new System.Drawing.Size(31, 13);
            this.L_EDay.TabIndex = 14;
            this.L_EDay.Text = "Days";
            this.L_EDay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // NUD_ESecond
            // 
            this.NUD_ESecond.Location = new System.Drawing.Point(6, 97);
            this.NUD_ESecond.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.NUD_ESecond.Name = "NUD_ESecond";
            this.NUD_ESecond.Size = new System.Drawing.Size(66, 20);
            this.NUD_ESecond.TabIndex = 17;
            this.NUD_ESecond.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // NUD_EMinute
            // 
            this.NUD_EMinute.Location = new System.Drawing.Point(6, 71);
            this.NUD_EMinute.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.NUD_EMinute.Name = "NUD_EMinute";
            this.NUD_EMinute.Size = new System.Drawing.Size(66, 20);
            this.NUD_EMinute.TabIndex = 16;
            this.NUD_EMinute.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // NUD_EHour
            // 
            this.NUD_EHour.Location = new System.Drawing.Point(6, 45);
            this.NUD_EHour.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.NUD_EHour.Name = "NUD_EHour";
            this.NUD_EHour.Size = new System.Drawing.Size(66, 20);
            this.NUD_EHour.TabIndex = 15;
            this.NUD_EHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // NUD_EDay
            // 
            this.NUD_EDay.Location = new System.Drawing.Point(6, 19);
            this.NUD_EDay.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.NUD_EDay.Name = "NUD_EDay";
            this.NUD_EDay.Size = new System.Drawing.Size(66, 20);
            this.NUD_EDay.TabIndex = 14;
            this.NUD_EDay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // GB_Initial
            // 
            this.GB_Initial.Controls.Add(this.L_ISecond);
            this.GB_Initial.Controls.Add(this.L_IMinute);
            this.GB_Initial.Controls.Add(this.L_IHour);
            this.GB_Initial.Controls.Add(this.L_IDay);
            this.GB_Initial.Controls.Add(this.NUD_ISecond);
            this.GB_Initial.Controls.Add(this.NUD_IMinute);
            this.GB_Initial.Controls.Add(this.NUD_IHour);
            this.GB_Initial.Controls.Add(this.NUD_IDay);
            this.GB_Initial.Location = new System.Drawing.Point(12, 12);
            this.GB_Initial.Name = "GB_Initial";
            this.GB_Initial.Size = new System.Drawing.Size(151, 123);
            this.GB_Initial.TabIndex = 74;
            this.GB_Initial.TabStop = false;
            this.GB_Initial.Text = "Initial Time";
            // 
            // L_ISecond
            // 
            this.L_ISecond.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.L_ISecond.AutoSize = true;
            this.L_ISecond.Location = new System.Drawing.Point(75, 100);
            this.L_ISecond.Name = "L_ISecond";
            this.L_ISecond.Size = new System.Drawing.Size(49, 13);
            this.L_ISecond.TabIndex = 20;
            this.L_ISecond.Text = "Seconds";
            this.L_ISecond.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // L_IMinute
            // 
            this.L_IMinute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.L_IMinute.AutoSize = true;
            this.L_IMinute.Location = new System.Drawing.Point(75, 74);
            this.L_IMinute.Name = "L_IMinute";
            this.L_IMinute.Size = new System.Drawing.Size(44, 13);
            this.L_IMinute.TabIndex = 19;
            this.L_IMinute.Text = "Minutes";
            this.L_IMinute.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // L_IHour
            // 
            this.L_IHour.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.L_IHour.AutoSize = true;
            this.L_IHour.Location = new System.Drawing.Point(75, 48);
            this.L_IHour.Name = "L_IHour";
            this.L_IHour.Size = new System.Drawing.Size(35, 13);
            this.L_IHour.TabIndex = 18;
            this.L_IHour.Text = "Hours";
            this.L_IHour.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // L_IDay
            // 
            this.L_IDay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.L_IDay.AutoSize = true;
            this.L_IDay.Location = new System.Drawing.Point(75, 22);
            this.L_IDay.Name = "L_IDay";
            this.L_IDay.Size = new System.Drawing.Size(31, 13);
            this.L_IDay.TabIndex = 14;
            this.L_IDay.Text = "Days";
            this.L_IDay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // NUD_ISecond
            // 
            this.NUD_ISecond.Location = new System.Drawing.Point(7, 97);
            this.NUD_ISecond.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.NUD_ISecond.Name = "NUD_ISecond";
            this.NUD_ISecond.Size = new System.Drawing.Size(66, 20);
            this.NUD_ISecond.TabIndex = 17;
            this.NUD_ISecond.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // NUD_IMinute
            // 
            this.NUD_IMinute.Location = new System.Drawing.Point(7, 71);
            this.NUD_IMinute.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.NUD_IMinute.Name = "NUD_IMinute";
            this.NUD_IMinute.Size = new System.Drawing.Size(66, 20);
            this.NUD_IMinute.TabIndex = 16;
            this.NUD_IMinute.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // NUD_IHour
            // 
            this.NUD_IHour.Location = new System.Drawing.Point(7, 45);
            this.NUD_IHour.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.NUD_IHour.Name = "NUD_IHour";
            this.NUD_IHour.Size = new System.Drawing.Size(66, 20);
            this.NUD_IHour.TabIndex = 15;
            this.NUD_IHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // NUD_IDay
            // 
            this.NUD_IDay.Location = new System.Drawing.Point(7, 19);
            this.NUD_IDay.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.NUD_IDay.Name = "NUD_IDay";
            this.NUD_IDay.Size = new System.Drawing.Size(66, 20);
            this.NUD_IDay.TabIndex = 14;
            this.NUD_IDay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // B_BerryFix
            // 
            this.B_BerryFix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.B_BerryFix.Location = new System.Drawing.Point(93, 141);
            this.B_BerryFix.Name = "B_BerryFix";
            this.B_BerryFix.Size = new System.Drawing.Size(66, 23);
            this.B_BerryFix.TabIndex = 78;
            this.B_BerryFix.Text = "Berry Fix";
            this.B_BerryFix.UseVisualStyleBackColor = true;
            this.B_BerryFix.Click += new System.EventHandler(this.B_BerryFix_Click);
            // 
            // SAV_RTC3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 176);
            this.Controls.Add(this.B_BerryFix);
            this.Controls.Add(this.B_Reset);
            this.Controls.Add(this.GB_Passed);
            this.Controls.Add(this.GB_Initial);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_Cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(200, 150);
            this.Name = "SAV_RTC3";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Real Time Clock Editor";
            this.GB_Passed.ResumeLayout(false);
            this.GB_Passed.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_ESecond)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_EMinute)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_EHour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_EDay)).EndInit();
            this.GB_Initial.ResumeLayout(false);
            this.GB_Initial.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_ISecond)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_IMinute)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_IHour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_IDay)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Reset;
        private System.Windows.Forms.GroupBox GB_Passed;
        private System.Windows.Forms.Label L_ESecond;
        private System.Windows.Forms.Label L_EMinute;
        private System.Windows.Forms.Label L_EHour;
        private System.Windows.Forms.Label L_EDay;
        private System.Windows.Forms.NumericUpDown NUD_ESecond;
        private System.Windows.Forms.NumericUpDown NUD_EMinute;
        private System.Windows.Forms.NumericUpDown NUD_EHour;
        private System.Windows.Forms.NumericUpDown NUD_EDay;
        private System.Windows.Forms.GroupBox GB_Initial;
        private System.Windows.Forms.Label L_ISecond;
        private System.Windows.Forms.Label L_IMinute;
        private System.Windows.Forms.NumericUpDown NUD_ISecond;
        private System.Windows.Forms.NumericUpDown NUD_IMinute;
        private System.Windows.Forms.NumericUpDown NUD_IHour;
        private System.Windows.Forms.NumericUpDown NUD_IDay;
        private System.Windows.Forms.Label L_IDay;
        private System.Windows.Forms.Label L_IHour;
        private System.Windows.Forms.Button B_BerryFix;
    }
}
