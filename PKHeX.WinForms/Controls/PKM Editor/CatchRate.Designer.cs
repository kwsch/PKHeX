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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.NUD_CatchRate = new System.Windows.Forms.NumericUpDown();
            this.B_Clear = new System.Windows.Forms.Button();
            this.B_Reset = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_CatchRate)).BeginInit();
            this.SuspendLayout();
            //
            // flowLayoutPanel1
            //
            this.flowLayoutPanel1.Controls.Add(this.NUD_CatchRate);
            this.flowLayoutPanel1.Controls.Add(this.B_Clear);
            this.flowLayoutPanel1.Controls.Add(this.B_Reset);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(162, 25);
            this.flowLayoutPanel1.TabIndex = 1;
            //
            // NUD_CatchRate
            //
            this.NUD_CatchRate.Location = new System.Drawing.Point(3, 3);
            this.NUD_CatchRate.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NUD_CatchRate.Name = "NUD_CatchRate";
            this.NUD_CatchRate.Size = new System.Drawing.Size(40, 20);
            this.NUD_CatchRate.TabIndex = 0;
            this.NUD_CatchRate.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NUD_CatchRate.ValueChanged += new System.EventHandler(this.ChangeValue);
            //
            // B_Clear
            //
            this.B_Clear.Location = new System.Drawing.Point(47, 1);
            this.B_Clear.Margin = new System.Windows.Forms.Padding(1, 1, 1, 3);
            this.B_Clear.Name = "B_Clear";
            this.B_Clear.Size = new System.Drawing.Size(55, 23);
            this.B_Clear.TabIndex = 1;
            this.B_Clear.Text = "Clear";
            this.B_Clear.UseVisualStyleBackColor = true;
            this.B_Clear.Click += new System.EventHandler(this.Clear);
            //
            // B_Reset
            //
            this.B_Reset.Location = new System.Drawing.Point(104, 1);
            this.B_Reset.Margin = new System.Windows.Forms.Padding(1, 1, 1, 3);
            this.B_Reset.Name = "B_Reset";
            this.B_Reset.Size = new System.Drawing.Size(55, 23);
            this.B_Reset.TabIndex = 2;
            this.B_Reset.Text = "Reset";
            this.B_Reset.UseVisualStyleBackColor = true;
            this.B_Reset.Click += new System.EventHandler(this.Reset);
            //
            // CatchRate
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "CatchRate";
            this.Size = new System.Drawing.Size(162, 25);
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NUD_CatchRate)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.NumericUpDown NUD_CatchRate;
        private System.Windows.Forms.Button B_Clear;
        private System.Windows.Forms.Button B_Reset;
    }
}
