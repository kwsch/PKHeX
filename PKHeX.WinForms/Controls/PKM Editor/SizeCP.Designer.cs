namespace PKHeX.WinForms.Controls
{
    partial class SizeCP
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
            this.FLP_CP = new System.Windows.Forms.FlowLayoutPanel();
            this.MT_CP = new System.Windows.Forms.MaskedTextBox();
            this.CHK_Auto = new System.Windows.Forms.CheckBox();
            this.L_CP = new System.Windows.Forms.Label();
            this.L_Height = new System.Windows.Forms.Label();
            this.L_Weight = new System.Windows.Forms.Label();
            this.NUD_HeightScalar = new System.Windows.Forms.NumericUpDown();
            this.TB_HeightAbs = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.FLP_Weight = new System.Windows.Forms.FlowLayoutPanel();
            this.NUD_WeightScalar = new System.Windows.Forms.NumericUpDown();
            this.TB_WeightAbs = new System.Windows.Forms.TextBox();
            this.L_SizeW = new System.Windows.Forms.Label();
            this.FLP_Height = new System.Windows.Forms.FlowLayoutPanel();
            this.L_SizeH = new System.Windows.Forms.Label();
            this.FLP_CP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_HeightScalar)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.FLP_Weight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_WeightScalar)).BeginInit();
            this.FLP_Height.SuspendLayout();
            this.SuspendLayout();
            // 
            // FLP_CP
            // 
            this.FLP_CP.Controls.Add(this.MT_CP);
            this.FLP_CP.Controls.Add(this.CHK_Auto);
            this.FLP_CP.Location = new System.Drawing.Point(63, 48);
            this.FLP_CP.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_CP.Name = "FLP_CP";
            this.FLP_CP.Size = new System.Drawing.Size(140, 20);
            this.FLP_CP.TabIndex = 1;
            // 
            // MT_CP
            // 
            this.MT_CP.Location = new System.Drawing.Point(0, 0);
            this.MT_CP.Margin = new System.Windows.Forms.Padding(0);
            this.MT_CP.Mask = "00000";
            this.MT_CP.Name = "MT_CP";
            this.MT_CP.Size = new System.Drawing.Size(36, 20);
            this.MT_CP.TabIndex = 5;
            this.MT_CP.TextChanged += new System.EventHandler(this.MT_CP_TextChanged);
            // 
            // CHK_Auto
            // 
            this.CHK_Auto.AutoSize = true;
            this.CHK_Auto.Location = new System.Drawing.Point(39, 3);
            this.CHK_Auto.Name = "CHK_Auto";
            this.CHK_Auto.Size = new System.Drawing.Size(48, 17);
            this.CHK_Auto.TabIndex = 6;
            this.CHK_Auto.Text = "Auto";
            this.CHK_Auto.UseVisualStyleBackColor = true;
            this.CHK_Auto.CheckedChanged += new System.EventHandler(this.UpdateFlagState);
            // 
            // L_CP
            // 
            this.L_CP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_CP.Location = new System.Drawing.Point(3, 48);
            this.L_CP.Name = "L_CP";
            this.L_CP.Size = new System.Drawing.Size(57, 20);
            this.L_CP.TabIndex = 0;
            this.L_CP.Text = "CP:";
            this.L_CP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Height
            // 
            this.L_Height.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_Height.Location = new System.Drawing.Point(3, 0);
            this.L_Height.Name = "L_Height";
            this.L_Height.Size = new System.Drawing.Size(57, 13);
            this.L_Height.TabIndex = 1;
            this.L_Height.Text = "Height:";
            this.L_Height.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Weight
            // 
            this.L_Weight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_Weight.Location = new System.Drawing.Point(3, 24);
            this.L_Weight.Name = "L_Weight";
            this.L_Weight.Size = new System.Drawing.Size(57, 20);
            this.L_Weight.TabIndex = 2;
            this.L_Weight.Text = "Weight:";
            this.L_Weight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_HeightScalar
            // 
            this.NUD_HeightScalar.Location = new System.Drawing.Point(0, 0);
            this.NUD_HeightScalar.Margin = new System.Windows.Forms.Padding(0);
            this.NUD_HeightScalar.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NUD_HeightScalar.Name = "NUD_HeightScalar";
            this.NUD_HeightScalar.Size = new System.Drawing.Size(40, 20);
            this.NUD_HeightScalar.TabIndex = 1;
            this.NUD_HeightScalar.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NUD_HeightScalar.ValueChanged += new System.EventHandler(this.NUD_HeightScalar_ValueChanged);
            // 
            // TB_HeightAbs
            // 
            this.TB_HeightAbs.Location = new System.Drawing.Point(40, 0);
            this.TB_HeightAbs.Margin = new System.Windows.Forms.Padding(0);
            this.TB_HeightAbs.Name = "TB_HeightAbs";
            this.TB_HeightAbs.Size = new System.Drawing.Size(67, 20);
            this.TB_HeightAbs.TabIndex = 2;
            this.TB_HeightAbs.Text = "123.456789";
            this.TB_HeightAbs.TextChanged += new System.EventHandler(this.TB_HeightAbs_TextChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 31F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 69F));
            this.tableLayoutPanel1.Controls.Add(this.FLP_Weight, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.L_Weight, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.FLP_Height, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.L_Height, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.L_CP, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.FLP_CP, 1, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(204, 68);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // FLP_Weight
            // 
            this.FLP_Weight.Controls.Add(this.NUD_WeightScalar);
            this.FLP_Weight.Controls.Add(this.TB_WeightAbs);
            this.FLP_Weight.Controls.Add(this.L_SizeW);
            this.FLP_Weight.Location = new System.Drawing.Point(63, 24);
            this.FLP_Weight.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_Weight.Name = "FLP_Weight";
            this.FLP_Weight.Size = new System.Drawing.Size(140, 21);
            this.FLP_Weight.TabIndex = 4;
            // 
            // NUD_WeightScalar
            // 
            this.NUD_WeightScalar.Location = new System.Drawing.Point(0, 0);
            this.NUD_WeightScalar.Margin = new System.Windows.Forms.Padding(0);
            this.NUD_WeightScalar.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NUD_WeightScalar.Name = "NUD_WeightScalar";
            this.NUD_WeightScalar.Size = new System.Drawing.Size(40, 20);
            this.NUD_WeightScalar.TabIndex = 3;
            this.NUD_WeightScalar.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NUD_WeightScalar.ValueChanged += new System.EventHandler(this.NUD_WeightScalar_ValueChanged);
            // 
            // TB_WeightAbs
            // 
            this.TB_WeightAbs.Location = new System.Drawing.Point(40, 0);
            this.TB_WeightAbs.Margin = new System.Windows.Forms.Padding(0);
            this.TB_WeightAbs.Name = "TB_WeightAbs";
            this.TB_WeightAbs.Size = new System.Drawing.Size(67, 20);
            this.TB_WeightAbs.TabIndex = 4;
            this.TB_WeightAbs.Text = "123.456789";
            this.TB_WeightAbs.TextChanged += new System.EventHandler(this.TB_WeightAbs_TextChanged);
            // 
            // L_SizeW
            // 
            this.L_SizeW.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_SizeW.Location = new System.Drawing.Point(110, 0);
            this.L_SizeW.Name = "L_SizeW";
            this.L_SizeW.Size = new System.Drawing.Size(24, 20);
            this.L_SizeW.TabIndex = 9;
            this.L_SizeW.Text = "XS";
            this.L_SizeW.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FLP_Height
            // 
            this.FLP_Height.Controls.Add(this.NUD_HeightScalar);
            this.FLP_Height.Controls.Add(this.TB_HeightAbs);
            this.FLP_Height.Controls.Add(this.L_SizeH);
            this.FLP_Height.Location = new System.Drawing.Point(63, 0);
            this.FLP_Height.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_Height.Name = "FLP_Height";
            this.FLP_Height.Size = new System.Drawing.Size(140, 21);
            this.FLP_Height.TabIndex = 3;
            // 
            // L_SizeH
            // 
            this.L_SizeH.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_SizeH.Location = new System.Drawing.Point(110, 0);
            this.L_SizeH.Name = "L_SizeH";
            this.L_SizeH.Size = new System.Drawing.Size(24, 20);
            this.L_SizeH.TabIndex = 8;
            this.L_SizeH.Text = "XL";
            this.L_SizeH.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SizeCP
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SizeCP";
            this.Size = new System.Drawing.Size(204, 68);
            this.FLP_CP.ResumeLayout(false);
            this.FLP_CP.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_HeightScalar)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.FLP_Weight.ResumeLayout(false);
            this.FLP_Weight.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_WeightScalar)).EndInit();
            this.FLP_Height.ResumeLayout(false);
            this.FLP_Height.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel FLP_CP;
        private System.Windows.Forms.MaskedTextBox MT_CP;
        private System.Windows.Forms.CheckBox CHK_Auto;
        private System.Windows.Forms.Label L_CP;
        private System.Windows.Forms.Label L_Height;
        private System.Windows.Forms.Label L_Weight;
        private System.Windows.Forms.NumericUpDown NUD_HeightScalar;
        private System.Windows.Forms.TextBox TB_HeightAbs;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel FLP_Weight;
        private System.Windows.Forms.NumericUpDown NUD_WeightScalar;
        private System.Windows.Forms.TextBox TB_WeightAbs;
        private System.Windows.Forms.FlowLayoutPanel FLP_Height;
        private System.Windows.Forms.Label L_SizeW;
        private System.Windows.Forms.Label L_SizeH;
    }
}
