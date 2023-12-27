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
            B_Save = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            B_Reset = new System.Windows.Forms.Button();
            GB_Passed = new System.Windows.Forms.GroupBox();
            L_ESecond = new System.Windows.Forms.Label();
            L_EMinute = new System.Windows.Forms.Label();
            L_EHour = new System.Windows.Forms.Label();
            L_EDay = new System.Windows.Forms.Label();
            NUD_ESecond = new System.Windows.Forms.NumericUpDown();
            NUD_EMinute = new System.Windows.Forms.NumericUpDown();
            NUD_EHour = new System.Windows.Forms.NumericUpDown();
            NUD_EDay = new System.Windows.Forms.NumericUpDown();
            GB_Initial = new System.Windows.Forms.GroupBox();
            L_ISecond = new System.Windows.Forms.Label();
            L_IMinute = new System.Windows.Forms.Label();
            L_IHour = new System.Windows.Forms.Label();
            L_IDay = new System.Windows.Forms.Label();
            NUD_ISecond = new System.Windows.Forms.NumericUpDown();
            NUD_IMinute = new System.Windows.Forms.NumericUpDown();
            NUD_IHour = new System.Windows.Forms.NumericUpDown();
            NUD_IDay = new System.Windows.Forms.NumericUpDown();
            B_BerryFix = new System.Windows.Forms.Button();
            GB_Passed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_ESecond).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_EMinute).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_EHour).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_EDay).BeginInit();
            GB_Initial.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_ISecond).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_IMinute).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_IHour).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_IDay).BeginInit();
            SuspendLayout();
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(287, 163);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(88, 27);
            B_Save.TabIndex = 73;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(192, 163);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(88, 27);
            B_Cancel.TabIndex = 72;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_Reset
            // 
            B_Reset.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_Reset.Location = new System.Drawing.Point(14, 163);
            B_Reset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Reset.Name = "B_Reset";
            B_Reset.Size = new System.Drawing.Size(88, 27);
            B_Reset.TabIndex = 77;
            B_Reset.Text = "Reset RTC";
            B_Reset.UseVisualStyleBackColor = true;
            B_Reset.Click += B_Reset_Click;
            // 
            // GB_Passed
            // 
            GB_Passed.Controls.Add(L_ESecond);
            GB_Passed.Controls.Add(L_EMinute);
            GB_Passed.Controls.Add(L_EHour);
            GB_Passed.Controls.Add(L_EDay);
            GB_Passed.Controls.Add(NUD_ESecond);
            GB_Passed.Controls.Add(NUD_EMinute);
            GB_Passed.Controls.Add(NUD_EHour);
            GB_Passed.Controls.Add(NUD_EDay);
            GB_Passed.Location = new System.Drawing.Point(197, 14);
            GB_Passed.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Passed.Name = "GB_Passed";
            GB_Passed.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Passed.Size = new System.Drawing.Size(176, 142);
            GB_Passed.TabIndex = 75;
            GB_Passed.TabStop = false;
            GB_Passed.Text = "Time Elapsed";
            // 
            // L_ESecond
            // 
            L_ESecond.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            L_ESecond.AutoSize = true;
            L_ESecond.Location = new System.Drawing.Point(88, 115);
            L_ESecond.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_ESecond.Name = "L_ESecond";
            L_ESecond.Size = new System.Drawing.Size(51, 15);
            L_ESecond.TabIndex = 20;
            L_ESecond.Text = "Seconds";
            L_ESecond.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // L_EMinute
            // 
            L_EMinute.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            L_EMinute.AutoSize = true;
            L_EMinute.Location = new System.Drawing.Point(88, 85);
            L_EMinute.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_EMinute.Name = "L_EMinute";
            L_EMinute.Size = new System.Drawing.Size(50, 15);
            L_EMinute.TabIndex = 19;
            L_EMinute.Text = "Minutes";
            L_EMinute.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // L_EHour
            // 
            L_EHour.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            L_EHour.AutoSize = true;
            L_EHour.Location = new System.Drawing.Point(88, 55);
            L_EHour.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_EHour.Name = "L_EHour";
            L_EHour.Size = new System.Drawing.Size(39, 15);
            L_EHour.TabIndex = 18;
            L_EHour.Text = "Hours";
            L_EHour.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // L_EDay
            // 
            L_EDay.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            L_EDay.AutoSize = true;
            L_EDay.Location = new System.Drawing.Point(88, 25);
            L_EDay.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_EDay.Name = "L_EDay";
            L_EDay.Size = new System.Drawing.Size(32, 15);
            L_EDay.TabIndex = 14;
            L_EDay.Text = "Days";
            L_EDay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // NUD_ESecond
            // 
            NUD_ESecond.Location = new System.Drawing.Point(7, 112);
            NUD_ESecond.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_ESecond.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            NUD_ESecond.Name = "NUD_ESecond";
            NUD_ESecond.Size = new System.Drawing.Size(77, 23);
            NUD_ESecond.TabIndex = 17;
            NUD_ESecond.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // NUD_EMinute
            // 
            NUD_EMinute.Location = new System.Drawing.Point(7, 82);
            NUD_EMinute.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_EMinute.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            NUD_EMinute.Name = "NUD_EMinute";
            NUD_EMinute.Size = new System.Drawing.Size(77, 23);
            NUD_EMinute.TabIndex = 16;
            NUD_EMinute.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // NUD_EHour
            // 
            NUD_EHour.Location = new System.Drawing.Point(7, 52);
            NUD_EHour.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_EHour.Maximum = new decimal(new int[] { 23, 0, 0, 0 });
            NUD_EHour.Name = "NUD_EHour";
            NUD_EHour.Size = new System.Drawing.Size(77, 23);
            NUD_EHour.TabIndex = 15;
            NUD_EHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // NUD_EDay
            // 
            NUD_EDay.Location = new System.Drawing.Point(7, 22);
            NUD_EDay.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_EDay.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NUD_EDay.Name = "NUD_EDay";
            NUD_EDay.Size = new System.Drawing.Size(77, 23);
            NUD_EDay.TabIndex = 14;
            NUD_EDay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // GB_Initial
            // 
            GB_Initial.Controls.Add(L_ISecond);
            GB_Initial.Controls.Add(L_IMinute);
            GB_Initial.Controls.Add(L_IHour);
            GB_Initial.Controls.Add(L_IDay);
            GB_Initial.Controls.Add(NUD_ISecond);
            GB_Initial.Controls.Add(NUD_IMinute);
            GB_Initial.Controls.Add(NUD_IHour);
            GB_Initial.Controls.Add(NUD_IDay);
            GB_Initial.Location = new System.Drawing.Point(14, 14);
            GB_Initial.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Initial.Name = "GB_Initial";
            GB_Initial.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Initial.Size = new System.Drawing.Size(176, 142);
            GB_Initial.TabIndex = 74;
            GB_Initial.TabStop = false;
            GB_Initial.Text = "Initial Time";
            // 
            // L_ISecond
            // 
            L_ISecond.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            L_ISecond.AutoSize = true;
            L_ISecond.Location = new System.Drawing.Point(88, 115);
            L_ISecond.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_ISecond.Name = "L_ISecond";
            L_ISecond.Size = new System.Drawing.Size(51, 15);
            L_ISecond.TabIndex = 20;
            L_ISecond.Text = "Seconds";
            L_ISecond.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // L_IMinute
            // 
            L_IMinute.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            L_IMinute.AutoSize = true;
            L_IMinute.Location = new System.Drawing.Point(88, 85);
            L_IMinute.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_IMinute.Name = "L_IMinute";
            L_IMinute.Size = new System.Drawing.Size(50, 15);
            L_IMinute.TabIndex = 19;
            L_IMinute.Text = "Minutes";
            L_IMinute.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // L_IHour
            // 
            L_IHour.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            L_IHour.AutoSize = true;
            L_IHour.Location = new System.Drawing.Point(88, 55);
            L_IHour.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_IHour.Name = "L_IHour";
            L_IHour.Size = new System.Drawing.Size(39, 15);
            L_IHour.TabIndex = 18;
            L_IHour.Text = "Hours";
            L_IHour.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // L_IDay
            // 
            L_IDay.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            L_IDay.AutoSize = true;
            L_IDay.Location = new System.Drawing.Point(88, 25);
            L_IDay.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_IDay.Name = "L_IDay";
            L_IDay.Size = new System.Drawing.Size(32, 15);
            L_IDay.TabIndex = 14;
            L_IDay.Text = "Days";
            L_IDay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // NUD_ISecond
            // 
            NUD_ISecond.Location = new System.Drawing.Point(8, 112);
            NUD_ISecond.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_ISecond.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            NUD_ISecond.Name = "NUD_ISecond";
            NUD_ISecond.Size = new System.Drawing.Size(77, 23);
            NUD_ISecond.TabIndex = 17;
            NUD_ISecond.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // NUD_IMinute
            // 
            NUD_IMinute.Location = new System.Drawing.Point(8, 82);
            NUD_IMinute.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_IMinute.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            NUD_IMinute.Name = "NUD_IMinute";
            NUD_IMinute.Size = new System.Drawing.Size(77, 23);
            NUD_IMinute.TabIndex = 16;
            NUD_IMinute.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // NUD_IHour
            // 
            NUD_IHour.Location = new System.Drawing.Point(8, 52);
            NUD_IHour.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_IHour.Maximum = new decimal(new int[] { 23, 0, 0, 0 });
            NUD_IHour.Name = "NUD_IHour";
            NUD_IHour.Size = new System.Drawing.Size(77, 23);
            NUD_IHour.TabIndex = 15;
            NUD_IHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // NUD_IDay
            // 
            NUD_IDay.Location = new System.Drawing.Point(8, 22);
            NUD_IDay.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_IDay.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NUD_IDay.Name = "NUD_IDay";
            NUD_IDay.Size = new System.Drawing.Size(77, 23);
            NUD_IDay.TabIndex = 14;
            NUD_IDay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // B_BerryFix
            // 
            B_BerryFix.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_BerryFix.Location = new System.Drawing.Point(108, 163);
            B_BerryFix.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_BerryFix.Name = "B_BerryFix";
            B_BerryFix.Size = new System.Drawing.Size(77, 27);
            B_BerryFix.TabIndex = 78;
            B_BerryFix.Text = "Berry Fix";
            B_BerryFix.UseVisualStyleBackColor = true;
            B_BerryFix.Click += B_BerryFix_Click;
            // 
            // SAV_RTC3
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(386, 203);
            Controls.Add(B_BerryFix);
            Controls.Add(B_Reset);
            Controls.Add(GB_Passed);
            Controls.Add(GB_Initial);
            Controls.Add(B_Save);
            Controls.Add(B_Cancel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(231, 167);
            Name = "SAV_RTC3";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Real Time Clock Editor";
            GB_Passed.ResumeLayout(false);
            GB_Passed.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_ESecond).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_EMinute).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_EHour).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_EDay).EndInit();
            GB_Initial.ResumeLayout(false);
            GB_Initial.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_ISecond).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_IMinute).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_IHour).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_IDay).EndInit();
            ResumeLayout(false);
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
