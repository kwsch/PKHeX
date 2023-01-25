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
            this.L_Scale = new System.Windows.Forms.Label();
            this.FLP_Weight = new System.Windows.Forms.FlowLayoutPanel();
            this.NUD_WeightScalar = new System.Windows.Forms.NumericUpDown();
            this.TB_WeightAbs = new System.Windows.Forms.TextBox();
            this.L_SizeW = new System.Windows.Forms.Label();
            this.FLP_Height = new System.Windows.Forms.FlowLayoutPanel();
            this.L_SizeH = new System.Windows.Forms.Label();
            this.FLP_Scale3 = new System.Windows.Forms.FlowLayoutPanel();
            this.NUD_Scale = new System.Windows.Forms.NumericUpDown();
            this.L_SizeS = new System.Windows.Forms.Label();
            this.FLP_CP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_HeightScalar)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.FLP_Weight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_WeightScalar)).BeginInit();
            this.FLP_Height.SuspendLayout();
            this.FLP_Scale3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Scale)).BeginInit();
            this.SuspendLayout();
            // 
            // FLP_CP
            // 
            this.FLP_CP.Controls.Add(this.MT_CP);
            this.FLP_CP.Controls.Add(this.CHK_Auto);
            this.FLP_CP.Location = new System.Drawing.Point(80, 72);
            this.FLP_CP.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_CP.Name = "FLP_CP";
            this.FLP_CP.Size = new System.Drawing.Size(120, 24);
            this.FLP_CP.TabIndex = 1;
            // 
            // MT_CP
            // 
            this.MT_CP.Location = new System.Drawing.Point(0, 0);
            this.MT_CP.Margin = new System.Windows.Forms.Padding(0);
            this.MT_CP.Mask = "00000";
            this.MT_CP.Name = "MT_CP";
            this.MT_CP.Size = new System.Drawing.Size(32, 23);
            this.MT_CP.TabIndex = 5;
            this.MT_CP.TextChanged += new System.EventHandler(this.MT_CP_TextChanged);
            // 
            // CHK_Auto
            // 
            this.CHK_Auto.Location = new System.Drawing.Point(40, 0);
            this.CHK_Auto.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.CHK_Auto.Name = "CHK_Auto";
            this.CHK_Auto.Size = new System.Drawing.Size(64, 24);
            this.CHK_Auto.TabIndex = 6;
            this.CHK_Auto.Text = "Auto";
            this.CHK_Auto.UseVisualStyleBackColor = true;
            this.CHK_Auto.CheckedChanged += new System.EventHandler(this.UpdateFlagState);
            // 
            // L_CP
            // 
            this.L_CP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_CP.Location = new System.Drawing.Point(0, 72);
            this.L_CP.Margin = new System.Windows.Forms.Padding(0);
            this.L_CP.Name = "L_CP";
            this.L_CP.Size = new System.Drawing.Size(80, 24);
            this.L_CP.TabIndex = 0;
            this.L_CP.Text = "CP:";
            this.L_CP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Height
            // 
            this.L_Height.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_Height.Location = new System.Drawing.Point(0, 0);
            this.L_Height.Margin = new System.Windows.Forms.Padding(0);
            this.L_Height.Name = "L_Height";
            this.L_Height.Size = new System.Drawing.Size(80, 24);
            this.L_Height.TabIndex = 1;
            this.L_Height.Text = "Height:";
            this.L_Height.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Weight
            // 
            this.L_Weight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_Weight.Location = new System.Drawing.Point(0, 24);
            this.L_Weight.Margin = new System.Windows.Forms.Padding(0);
            this.L_Weight.Name = "L_Weight";
            this.L_Weight.Size = new System.Drawing.Size(80, 24);
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
            this.NUD_HeightScalar.Size = new System.Drawing.Size(40, 23);
            this.NUD_HeightScalar.TabIndex = 1;
            this.NUD_HeightScalar.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NUD_HeightScalar.ValueChanged += new System.EventHandler(this.NUD_HeightScalar_ValueChanged);
            this.NUD_HeightScalar.Click += new System.EventHandler(this.ClickScalarEntry);
            // 
            // TB_HeightAbs
            // 
            this.TB_HeightAbs.Location = new System.Drawing.Point(40, 0);
            this.TB_HeightAbs.Margin = new System.Windows.Forms.Padding(0);
            this.TB_HeightAbs.Name = "TB_HeightAbs";
            this.TB_HeightAbs.Size = new System.Drawing.Size(64, 23);
            this.TB_HeightAbs.TabIndex = 2;
            this.TB_HeightAbs.Text = "123.456789";
            this.TB_HeightAbs.TextChanged += new System.EventHandler(this.TB_HeightAbs_TextChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.L_Scale, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.FLP_Weight, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.L_Weight, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.FLP_Height, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.L_Height, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.FLP_CP, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.L_CP, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.FLP_Scale3, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(224, 96);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // L_Scale
            // 
            this.L_Scale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_Scale.Location = new System.Drawing.Point(0, 48);
            this.L_Scale.Margin = new System.Windows.Forms.Padding(0);
            this.L_Scale.Name = "L_Scale";
            this.L_Scale.Size = new System.Drawing.Size(80, 24);
            this.L_Scale.TabIndex = 6;
            this.L_Scale.Text = "Scale:";
            this.L_Scale.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FLP_Weight
            // 
            this.FLP_Weight.Controls.Add(this.NUD_WeightScalar);
            this.FLP_Weight.Controls.Add(this.TB_WeightAbs);
            this.FLP_Weight.Controls.Add(this.L_SizeW);
            this.FLP_Weight.Location = new System.Drawing.Point(80, 24);
            this.FLP_Weight.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_Weight.Name = "FLP_Weight";
            this.FLP_Weight.Size = new System.Drawing.Size(144, 24);
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
            this.NUD_WeightScalar.Size = new System.Drawing.Size(40, 23);
            this.NUD_WeightScalar.TabIndex = 3;
            this.NUD_WeightScalar.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NUD_WeightScalar.ValueChanged += new System.EventHandler(this.NUD_WeightScalar_ValueChanged);
            this.NUD_WeightScalar.Click += new System.EventHandler(this.ClickScalarEntry);
            // 
            // TB_WeightAbs
            // 
            this.TB_WeightAbs.Location = new System.Drawing.Point(40, 0);
            this.TB_WeightAbs.Margin = new System.Windows.Forms.Padding(0);
            this.TB_WeightAbs.Name = "TB_WeightAbs";
            this.TB_WeightAbs.Size = new System.Drawing.Size(64, 23);
            this.TB_WeightAbs.TabIndex = 4;
            this.TB_WeightAbs.Text = "123.456789";
            this.TB_WeightAbs.TextChanged += new System.EventHandler(this.TB_WeightAbs_TextChanged);
            // 
            // L_SizeW
            // 
            this.L_SizeW.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_SizeW.Location = new System.Drawing.Point(112, 0);
            this.L_SizeW.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.L_SizeW.Name = "L_SizeW";
            this.L_SizeW.Size = new System.Drawing.Size(32, 20);
            this.L_SizeW.TabIndex = 9;
            this.L_SizeW.Text = "XS";
            this.L_SizeW.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FLP_Height
            // 
            this.FLP_Height.Controls.Add(this.NUD_HeightScalar);
            this.FLP_Height.Controls.Add(this.TB_HeightAbs);
            this.FLP_Height.Controls.Add(this.L_SizeH);
            this.FLP_Height.Location = new System.Drawing.Point(80, 0);
            this.FLP_Height.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_Height.Name = "FLP_Height";
            this.FLP_Height.Size = new System.Drawing.Size(144, 24);
            this.FLP_Height.TabIndex = 3;
            // 
            // L_SizeH
            // 
            this.L_SizeH.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_SizeH.Location = new System.Drawing.Point(112, 0);
            this.L_SizeH.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.L_SizeH.Name = "L_SizeH";
            this.L_SizeH.Size = new System.Drawing.Size(32, 20);
            this.L_SizeH.TabIndex = 8;
            this.L_SizeH.Text = "XL";
            this.L_SizeH.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FLP_Scale3
            // 
            this.FLP_Scale3.Controls.Add(this.NUD_Scale);
            this.FLP_Scale3.Controls.Add(this.L_SizeS);
            this.FLP_Scale3.Location = new System.Drawing.Point(80, 48);
            this.FLP_Scale3.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_Scale3.Name = "FLP_Scale3";
            this.FLP_Scale3.Size = new System.Drawing.Size(120, 24);
            this.FLP_Scale3.TabIndex = 5;
            // 
            // NUD_Scale
            // 
            this.NUD_Scale.Location = new System.Drawing.Point(0, 0);
            this.NUD_Scale.Margin = new System.Windows.Forms.Padding(0);
            this.NUD_Scale.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NUD_Scale.Name = "NUD_Scale";
            this.NUD_Scale.Size = new System.Drawing.Size(40, 23);
            this.NUD_Scale.TabIndex = 3;
            this.NUD_Scale.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NUD_Scale.ValueChanged += new System.EventHandler(this.NUD_Scale_ValueChanged);
            this.NUD_Scale.Click += new System.EventHandler(this.ClickScalarEntry);
            // 
            // L_SizeS
            // 
            this.L_SizeS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_SizeS.Location = new System.Drawing.Point(48, 0);
            this.L_SizeS.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.L_SizeS.Name = "L_SizeS";
            this.L_SizeS.Size = new System.Drawing.Size(40, 20);
            this.L_SizeS.TabIndex = 9;
            this.L_SizeS.Text = "XXXS";
            this.L_SizeS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SizeCP
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SizeCP";
            this.Size = new System.Drawing.Size(224, 96);
            this.FLP_CP.ResumeLayout(false);
            this.FLP_CP.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_HeightScalar)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.FLP_Weight.ResumeLayout(false);
            this.FLP_Weight.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_WeightScalar)).EndInit();
            this.FLP_Height.ResumeLayout(false);
            this.FLP_Height.PerformLayout();
            this.FLP_Scale3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Scale)).EndInit();
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
        private System.Windows.Forms.Label L_Scale;
        private System.Windows.Forms.FlowLayoutPanel FLP_Scale3;
        private System.Windows.Forms.NumericUpDown NUD_Scale;
        private System.Windows.Forms.Label L_SizeS;
    }
}
