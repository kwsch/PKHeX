namespace PKHeX.WinForms.Controls
{
    partial class CatchRate
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
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            NUD_CatchRate = new System.Windows.Forms.NumericUpDown();
            B_Clear = new System.Windows.Forms.Button();
            B_Reset = new System.Windows.Forms.Button();
            flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_CatchRate).BeginInit();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(NUD_CatchRate);
            flowLayoutPanel1.Controls.Add(B_Clear);
            flowLayoutPanel1.Controls.Add(B_Reset);
            flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(184, 25);
            flowLayoutPanel1.TabIndex = 1;
            // 
            // NUD_CatchRate
            // 
            NUD_CatchRate.Location = new System.Drawing.Point(0, 0);
            NUD_CatchRate.Margin = new System.Windows.Forms.Padding(0);
            NUD_CatchRate.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_CatchRate.Name = "NUD_CatchRate";
            NUD_CatchRate.Size = new System.Drawing.Size(40, 23);
            NUD_CatchRate.TabIndex = 0;
            NUD_CatchRate.Value = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_CatchRate.ValueChanged += ChangeValue;
            // 
            // B_Clear
            // 
            B_Clear.Location = new System.Drawing.Point(40, 0);
            B_Clear.Margin = new System.Windows.Forms.Padding(0);
            B_Clear.Name = "B_Clear";
            B_Clear.Size = new System.Drawing.Size(64, 23);
            B_Clear.TabIndex = 1;
            B_Clear.Text = "Clear";
            B_Clear.UseVisualStyleBackColor = true;
            B_Clear.Click += Clear;
            // 
            // B_Reset
            // 
            B_Reset.Location = new System.Drawing.Point(104, 0);
            B_Reset.Margin = new System.Windows.Forms.Padding(0);
            B_Reset.Name = "B_Reset";
            B_Reset.Size = new System.Drawing.Size(64, 23);
            B_Reset.TabIndex = 2;
            B_Reset.Text = "Reset";
            B_Reset.UseVisualStyleBackColor = true;
            B_Reset.Click += Reset;
            // 
            // CatchRate
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(flowLayoutPanel1);
            Name = "CatchRate";
            Size = new System.Drawing.Size(184, 25);
            flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)NUD_CatchRate).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.NumericUpDown NUD_CatchRate;
        private System.Windows.Forms.Button B_Clear;
        private System.Windows.Forms.Button B_Reset;
    }
}
