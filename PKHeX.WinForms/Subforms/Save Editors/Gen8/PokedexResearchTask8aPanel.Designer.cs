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
            LB_Species = new System.Windows.Forms.ListBox();
            FLP_T1 = new System.Windows.Forms.FlowLayoutPanel();
            PB_Bonus = new System.Windows.Forms.PictureBox();
            Label_Task = new System.Windows.Forms.Label();
            NUP_CurrentValue = new System.Windows.Forms.NumericUpDown();
            FLP_T1Right = new System.Windows.Forms.FlowLayoutPanel();
            MTB_Threshold5 = new System.Windows.Forms.MaskedTextBox();
            MTB_Threshold4 = new System.Windows.Forms.MaskedTextBox();
            MTB_Threshold3 = new System.Windows.Forms.MaskedTextBox();
            MTB_Threshold2 = new System.Windows.Forms.MaskedTextBox();
            MTB_Threshold1 = new System.Windows.Forms.MaskedTextBox();
            FLP_T1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PB_Bonus).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUP_CurrentValue).BeginInit();
            FLP_T1Right.SuspendLayout();
            SuspendLayout();
            // 
            // LB_Species
            // 
            LB_Species.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            LB_Species.FormattingEnabled = true;
            LB_Species.ItemHeight = 15;
            LB_Species.Location = new System.Drawing.Point(-309, -117);
            LB_Species.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LB_Species.Name = "LB_Species";
            LB_Species.Size = new System.Drawing.Size(145, 274);
            LB_Species.TabIndex = 125;
            // 
            // FLP_T1
            // 
            FLP_T1.Controls.Add(PB_Bonus);
            FLP_T1.Controls.Add(Label_Task);
            FLP_T1.Controls.Add(NUP_CurrentValue);
            FLP_T1.Controls.Add(FLP_T1Right);
            FLP_T1.Location = new System.Drawing.Point(0, 0);
            FLP_T1.Margin = new System.Windows.Forms.Padding(0);
            FLP_T1.Name = "FLP_T1";
            FLP_T1.Size = new System.Drawing.Size(757, 25);
            FLP_T1.TabIndex = 127;
            // 
            // PB_Bonus
            // 
            PB_Bonus.Image = Properties.Resources.research_bonus_points;
            PB_Bonus.InitialImage = null;
            PB_Bonus.Location = new System.Drawing.Point(4, 0);
            PB_Bonus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            PB_Bonus.Name = "PB_Bonus";
            PB_Bonus.Size = new System.Drawing.Size(23, 23);
            PB_Bonus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            PB_Bonus.TabIndex = 43;
            PB_Bonus.TabStop = false;
            // 
            // Label_Task
            // 
            Label_Task.Location = new System.Drawing.Point(31, 0);
            Label_Task.Margin = new System.Windows.Forms.Padding(0);
            Label_Task.Name = "Label_Task";
            Label_Task.Size = new System.Drawing.Size(369, 23);
            Label_Task.TabIndex = 19;
            Label_Task.Text = "Task Description:";
            Label_Task.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // NUP_CurrentValue
            // 
            NUP_CurrentValue.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            NUP_CurrentValue.Location = new System.Drawing.Point(404, 0);
            NUP_CurrentValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            NUP_CurrentValue.Maximum = new decimal(new int[] { 60000, 0, 0, 0 });
            NUP_CurrentValue.Name = "NUP_CurrentValue";
            NUP_CurrentValue.Size = new System.Drawing.Size(84, 23);
            NUP_CurrentValue.TabIndex = 51;
            NUP_CurrentValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            NUP_CurrentValue.ValueChanged += NUP_CurrentValue_Changed;
            // 
            // FLP_T1Right
            // 
            FLP_T1Right.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            FLP_T1Right.Controls.Add(MTB_Threshold5);
            FLP_T1Right.Controls.Add(MTB_Threshold4);
            FLP_T1Right.Controls.Add(MTB_Threshold3);
            FLP_T1Right.Controls.Add(MTB_Threshold2);
            FLP_T1Right.Controls.Add(MTB_Threshold1);
            FLP_T1Right.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            FLP_T1Right.Location = new System.Drawing.Point(492, 0);
            FLP_T1Right.Margin = new System.Windows.Forms.Padding(0);
            FLP_T1Right.Name = "FLP_T1Right";
            FLP_T1Right.Size = new System.Drawing.Size(264, 24);
            FLP_T1Right.TabIndex = 121;
            // 
            // MTB_Threshold5
            // 
            MTB_Threshold5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            MTB_Threshold5.Enabled = false;
            MTB_Threshold5.Location = new System.Drawing.Point(214, 0);
            MTB_Threshold5.Margin = new System.Windows.Forms.Padding(2, 0, 4, 0);
            MTB_Threshold5.Mask = "999";
            MTB_Threshold5.Name = "MTB_Threshold5";
            MTB_Threshold5.PromptChar = ' ';
            MTB_Threshold5.Size = new System.Drawing.Size(46, 23);
            MTB_Threshold5.TabIndex = 45;
            MTB_Threshold5.Text = "100";
            MTB_Threshold5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // MTB_Threshold4
            // 
            MTB_Threshold4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            MTB_Threshold4.Enabled = false;
            MTB_Threshold4.Location = new System.Drawing.Point(162, 0);
            MTB_Threshold4.Margin = new System.Windows.Forms.Padding(2, 0, 4, 0);
            MTB_Threshold4.Mask = "999";
            MTB_Threshold4.Name = "MTB_Threshold4";
            MTB_Threshold4.PromptChar = ' ';
            MTB_Threshold4.Size = new System.Drawing.Size(46, 23);
            MTB_Threshold4.TabIndex = 47;
            MTB_Threshold4.Text = "50";
            MTB_Threshold4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // MTB_Threshold3
            // 
            MTB_Threshold3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            MTB_Threshold3.Enabled = false;
            MTB_Threshold3.Location = new System.Drawing.Point(110, 0);
            MTB_Threshold3.Margin = new System.Windows.Forms.Padding(2, 0, 4, 0);
            MTB_Threshold3.Mask = "999";
            MTB_Threshold3.Name = "MTB_Threshold3";
            MTB_Threshold3.PromptChar = ' ';
            MTB_Threshold3.Size = new System.Drawing.Size(46, 23);
            MTB_Threshold3.TabIndex = 48;
            MTB_Threshold3.Text = "10";
            MTB_Threshold3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // MTB_Threshold2
            // 
            MTB_Threshold2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            MTB_Threshold2.Enabled = false;
            MTB_Threshold2.Location = new System.Drawing.Point(58, 0);
            MTB_Threshold2.Margin = new System.Windows.Forms.Padding(2, 0, 4, 0);
            MTB_Threshold2.Mask = "999";
            MTB_Threshold2.Name = "MTB_Threshold2";
            MTB_Threshold2.PromptChar = ' ';
            MTB_Threshold2.Size = new System.Drawing.Size(46, 23);
            MTB_Threshold2.TabIndex = 49;
            MTB_Threshold2.Text = "5";
            MTB_Threshold2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // MTB_Threshold1
            // 
            MTB_Threshold1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            MTB_Threshold1.Enabled = false;
            MTB_Threshold1.Location = new System.Drawing.Point(6, 0);
            MTB_Threshold1.Margin = new System.Windows.Forms.Padding(2, 0, 4, 0);
            MTB_Threshold1.Mask = "999";
            MTB_Threshold1.Name = "MTB_Threshold1";
            MTB_Threshold1.PromptChar = ' ';
            MTB_Threshold1.Size = new System.Drawing.Size(46, 23);
            MTB_Threshold1.TabIndex = 50;
            MTB_Threshold1.Text = "1";
            MTB_Threshold1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // PokedexResearchTask8aPanel
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(FLP_T1);
            Controls.Add(LB_Species);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "PokedexResearchTask8aPanel";
            Size = new System.Drawing.Size(756, 25);
            FLP_T1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PB_Bonus).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUP_CurrentValue).EndInit();
            FLP_T1Right.ResumeLayout(false);
            FLP_T1Right.PerformLayout();
            ResumeLayout(false);
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
