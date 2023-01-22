namespace PKHeX.WinForms.Controls
{
    partial class PokedexResearchTask8aPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.LB_Species = new System.Windows.Forms.ListBox();
            this.FLP_T1 = new System.Windows.Forms.FlowLayoutPanel();
            this.PB_Bonus = new System.Windows.Forms.PictureBox();
            this.Label_Task = new System.Windows.Forms.Label();
            this.NUP_CurrentValue = new System.Windows.Forms.NumericUpDown();
            this.FLP_T1Right = new System.Windows.Forms.FlowLayoutPanel();
            this.MTB_Threshold5 = new System.Windows.Forms.MaskedTextBox();
            this.MTB_Threshold4 = new System.Windows.Forms.MaskedTextBox();
            this.MTB_Threshold3 = new System.Windows.Forms.MaskedTextBox();
            this.MTB_Threshold2 = new System.Windows.Forms.MaskedTextBox();
            this.MTB_Threshold1 = new System.Windows.Forms.MaskedTextBox();
            this.FLP_T1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Bonus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_CurrentValue)).BeginInit();
            this.FLP_T1Right.SuspendLayout();
            this.SuspendLayout();
            // 
            // LB_Species
            // 
            this.LB_Species.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.LB_Species.FormattingEnabled = true;
            this.LB_Species.ItemHeight = 15;
            this.LB_Species.Location = new System.Drawing.Point(-309, -117);
            this.LB_Species.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.LB_Species.Name = "LB_Species";
            this.LB_Species.Size = new System.Drawing.Size(145, 274);
            this.LB_Species.TabIndex = 125;
            // 
            // FLP_T1
            // 
            this.FLP_T1.Controls.Add(this.PB_Bonus);
            this.FLP_T1.Controls.Add(this.Label_Task);
            this.FLP_T1.Controls.Add(this.NUP_CurrentValue);
            this.FLP_T1.Controls.Add(this.FLP_T1Right);
            this.FLP_T1.Location = new System.Drawing.Point(0, 0);
            this.FLP_T1.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_T1.Name = "FLP_T1";
            this.FLP_T1.Size = new System.Drawing.Size(757, 25);
            this.FLP_T1.TabIndex = 127;
            // 
            // PB_Bonus
            // 
            this.PB_Bonus.Image = global::PKHeX.WinForms.Properties.Resources.research_bonus_points;
            this.PB_Bonus.InitialImage = null;
            this.PB_Bonus.Location = new System.Drawing.Point(4, 0);
            this.PB_Bonus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PB_Bonus.Name = "PB_Bonus";
            this.PB_Bonus.Size = new System.Drawing.Size(23, 23);
            this.PB_Bonus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PB_Bonus.TabIndex = 43;
            this.PB_Bonus.TabStop = false;
            // 
            // Label_Task
            // 
            this.Label_Task.Location = new System.Drawing.Point(31, 0);
            this.Label_Task.Margin = new System.Windows.Forms.Padding(0);
            this.Label_Task.Name = "Label_Task";
            this.Label_Task.Size = new System.Drawing.Size(369, 23);
            this.Label_Task.TabIndex = 19;
            this.Label_Task.Text = "Task Description:";
            this.Label_Task.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // NUP_CurrentValue
            // 
            this.NUP_CurrentValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NUP_CurrentValue.Location = new System.Drawing.Point(404, 0);
            this.NUP_CurrentValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.NUP_CurrentValue.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.NUP_CurrentValue.Name = "NUP_CurrentValue";
            this.NUP_CurrentValue.Size = new System.Drawing.Size(84, 23);
            this.NUP_CurrentValue.TabIndex = 51;
            this.NUP_CurrentValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NUP_CurrentValue.ValueChanged += new System.EventHandler(this.NUP_CurrentValue_Changed);
            // 
            // FLP_T1Right
            // 
            this.FLP_T1Right.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_T1Right.Controls.Add(this.MTB_Threshold5);
            this.FLP_T1Right.Controls.Add(this.MTB_Threshold4);
            this.FLP_T1Right.Controls.Add(this.MTB_Threshold3);
            this.FLP_T1Right.Controls.Add(this.MTB_Threshold2);
            this.FLP_T1Right.Controls.Add(this.MTB_Threshold1);
            this.FLP_T1Right.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.FLP_T1Right.Location = new System.Drawing.Point(492, 0);
            this.FLP_T1Right.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_T1Right.Name = "FLP_T1Right";
            this.FLP_T1Right.Size = new System.Drawing.Size(264, 24);
            this.FLP_T1Right.TabIndex = 121;
            // 
            // MTB_Threshold5
            // 
            this.MTB_Threshold5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MTB_Threshold5.Enabled = false;
            this.MTB_Threshold5.Location = new System.Drawing.Point(214, 0);
            this.MTB_Threshold5.Margin = new System.Windows.Forms.Padding(2, 0, 4, 0);
            this.MTB_Threshold5.Mask = "999";
            this.MTB_Threshold5.Name = "MTB_Threshold5";
            this.MTB_Threshold5.PromptChar = ' ';
            this.MTB_Threshold5.Size = new System.Drawing.Size(46, 23);
            this.MTB_Threshold5.TabIndex = 45;
            this.MTB_Threshold5.Text = "100";
            this.MTB_Threshold5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // MTB_Threshold4
            // 
            this.MTB_Threshold4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MTB_Threshold4.Enabled = false;
            this.MTB_Threshold4.Location = new System.Drawing.Point(162, 0);
            this.MTB_Threshold4.Margin = new System.Windows.Forms.Padding(2, 0, 4, 0);
            this.MTB_Threshold4.Mask = "999";
            this.MTB_Threshold4.Name = "MTB_Threshold4";
            this.MTB_Threshold4.PromptChar = ' ';
            this.MTB_Threshold4.Size = new System.Drawing.Size(46, 23);
            this.MTB_Threshold4.TabIndex = 47;
            this.MTB_Threshold4.Text = "50";
            this.MTB_Threshold4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // MTB_Threshold3
            // 
            this.MTB_Threshold3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MTB_Threshold3.Enabled = false;
            this.MTB_Threshold3.Location = new System.Drawing.Point(110, 0);
            this.MTB_Threshold3.Margin = new System.Windows.Forms.Padding(2, 0, 4, 0);
            this.MTB_Threshold3.Mask = "999";
            this.MTB_Threshold3.Name = "MTB_Threshold3";
            this.MTB_Threshold3.PromptChar = ' ';
            this.MTB_Threshold3.Size = new System.Drawing.Size(46, 23);
            this.MTB_Threshold3.TabIndex = 48;
            this.MTB_Threshold3.Text = "10";
            this.MTB_Threshold3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // MTB_Threshold2
            // 
            this.MTB_Threshold2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MTB_Threshold2.Enabled = false;
            this.MTB_Threshold2.Location = new System.Drawing.Point(58, 0);
            this.MTB_Threshold2.Margin = new System.Windows.Forms.Padding(2, 0, 4, 0);
            this.MTB_Threshold2.Mask = "999";
            this.MTB_Threshold2.Name = "MTB_Threshold2";
            this.MTB_Threshold2.PromptChar = ' ';
            this.MTB_Threshold2.Size = new System.Drawing.Size(46, 23);
            this.MTB_Threshold2.TabIndex = 49;
            this.MTB_Threshold2.Text = "5";
            this.MTB_Threshold2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // MTB_Threshold1
            // 
            this.MTB_Threshold1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MTB_Threshold1.Enabled = false;
            this.MTB_Threshold1.Location = new System.Drawing.Point(6, 0);
            this.MTB_Threshold1.Margin = new System.Windows.Forms.Padding(2, 0, 4, 0);
            this.MTB_Threshold1.Mask = "999";
            this.MTB_Threshold1.Name = "MTB_Threshold1";
            this.MTB_Threshold1.PromptChar = ' ';
            this.MTB_Threshold1.Size = new System.Drawing.Size(46, 23);
            this.MTB_Threshold1.TabIndex = 50;
            this.MTB_Threshold1.Text = "1";
            this.MTB_Threshold1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // PokedexResearchTask8aPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.FLP_T1);
            this.Controls.Add(this.LB_Species);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "PokedexResearchTask8aPanel";
            this.Size = new System.Drawing.Size(756, 25);
            this.FLP_T1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PB_Bonus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUP_CurrentValue)).EndInit();
            this.FLP_T1Right.ResumeLayout(false);
            this.FLP_T1Right.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListBox LB_Species;
        private System.Windows.Forms.FlowLayoutPanel FLP_T1;
        private System.Windows.Forms.PictureBox PB_Bonus;
        private System.Windows.Forms.Label Label_Task;
        private System.Windows.Forms.NumericUpDown NUP_CurrentValue;
        private System.Windows.Forms.FlowLayoutPanel FLP_T1Right;
        private System.Windows.Forms.MaskedTextBox MTB_Threshold5;
        private System.Windows.Forms.MaskedTextBox MTB_Threshold4;
        private System.Windows.Forms.MaskedTextBox MTB_Threshold3;
        private System.Windows.Forms.MaskedTextBox MTB_Threshold2;
        private System.Windows.Forms.MaskedTextBox MTB_Threshold1;
    }
}
