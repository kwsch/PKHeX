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
            FLP_CP = new System.Windows.Forms.FlowLayoutPanel();
            MT_CP = new System.Windows.Forms.MaskedTextBox();
            CHK_Auto = new System.Windows.Forms.CheckBox();
            L_CP = new System.Windows.Forms.Label();
            L_Height = new System.Windows.Forms.Label();
            L_Weight = new System.Windows.Forms.Label();
            NUD_HeightScalar = new System.Windows.Forms.NumericUpDown();
            TB_HeightAbs = new System.Windows.Forms.TextBox();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            L_Scale = new System.Windows.Forms.Label();
            FLP_Weight = new System.Windows.Forms.FlowLayoutPanel();
            NUD_WeightScalar = new System.Windows.Forms.NumericUpDown();
            TB_WeightAbs = new System.Windows.Forms.TextBox();
            L_SizeW = new System.Windows.Forms.Label();
            FLP_Height = new System.Windows.Forms.FlowLayoutPanel();
            L_SizeH = new System.Windows.Forms.Label();
            FLP_Scale3 = new System.Windows.Forms.FlowLayoutPanel();
            NUD_Scale = new System.Windows.Forms.NumericUpDown();
            L_SizeS = new System.Windows.Forms.Label();
            FLP_CP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_HeightScalar).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            FLP_Weight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_WeightScalar).BeginInit();
            FLP_Height.SuspendLayout();
            FLP_Scale3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Scale).BeginInit();
            SuspendLayout();
            // 
            // FLP_CP
            // 
            FLP_CP.Controls.Add(MT_CP);
            FLP_CP.Controls.Add(CHK_Auto);
            FLP_CP.Location = new System.Drawing.Point(80, 72);
            FLP_CP.Margin = new System.Windows.Forms.Padding(0);
            FLP_CP.Name = "FLP_CP";
            FLP_CP.Size = new System.Drawing.Size(120, 24);
            FLP_CP.TabIndex = 1;
            // 
            // MT_CP
            // 
            MT_CP.Location = new System.Drawing.Point(0, 0);
            MT_CP.Margin = new System.Windows.Forms.Padding(0);
            MT_CP.Mask = "00000";
            MT_CP.Name = "MT_CP";
            MT_CP.Size = new System.Drawing.Size(40, 23);
            MT_CP.TabIndex = 5;
            MT_CP.TextChanged += MT_CP_TextChanged;
            // 
            // CHK_Auto
            // 
            CHK_Auto.Location = new System.Drawing.Point(48, 0);
            CHK_Auto.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            CHK_Auto.Name = "CHK_Auto";
            CHK_Auto.Size = new System.Drawing.Size(64, 24);
            CHK_Auto.TabIndex = 6;
            CHK_Auto.Text = "Auto";
            CHK_Auto.UseVisualStyleBackColor = true;
            CHK_Auto.CheckedChanged += UpdateFlagState;
            // 
            // L_CP
            // 
            L_CP.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            L_CP.Location = new System.Drawing.Point(0, 72);
            L_CP.Margin = new System.Windows.Forms.Padding(0);
            L_CP.Name = "L_CP";
            L_CP.Size = new System.Drawing.Size(80, 24);
            L_CP.TabIndex = 0;
            L_CP.Text = "CP:";
            L_CP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Height
            // 
            L_Height.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            L_Height.Location = new System.Drawing.Point(0, 0);
            L_Height.Margin = new System.Windows.Forms.Padding(0);
            L_Height.Name = "L_Height";
            L_Height.Size = new System.Drawing.Size(80, 24);
            L_Height.TabIndex = 1;
            L_Height.Text = "Height:";
            L_Height.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Weight
            // 
            L_Weight.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            L_Weight.Location = new System.Drawing.Point(0, 24);
            L_Weight.Margin = new System.Windows.Forms.Padding(0);
            L_Weight.Name = "L_Weight";
            L_Weight.Size = new System.Drawing.Size(80, 24);
            L_Weight.TabIndex = 2;
            L_Weight.Text = "Weight:";
            L_Weight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_HeightScalar
            // 
            NUD_HeightScalar.Location = new System.Drawing.Point(0, 0);
            NUD_HeightScalar.Margin = new System.Windows.Forms.Padding(0);
            NUD_HeightScalar.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_HeightScalar.Name = "NUD_HeightScalar";
            NUD_HeightScalar.Size = new System.Drawing.Size(40, 23);
            NUD_HeightScalar.TabIndex = 1;
            NUD_HeightScalar.Value = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_HeightScalar.ValueChanged += NUD_HeightScalar_ValueChanged;
            NUD_HeightScalar.Click += ClickScalarEntry;
            // 
            // TB_HeightAbs
            // 
            TB_HeightAbs.Location = new System.Drawing.Point(40, 0);
            TB_HeightAbs.Margin = new System.Windows.Forms.Padding(0);
            TB_HeightAbs.Name = "TB_HeightAbs";
            TB_HeightAbs.Size = new System.Drawing.Size(64, 23);
            TB_HeightAbs.TabIndex = 2;
            TB_HeightAbs.Text = "123.456789";
            TB_HeightAbs.TextChanged += TB_HeightAbs_TextChanged;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(L_Scale, 0, 2);
            tableLayoutPanel1.Controls.Add(FLP_Weight, 1, 1);
            tableLayoutPanel1.Controls.Add(L_Weight, 0, 1);
            tableLayoutPanel1.Controls.Add(FLP_Height, 1, 0);
            tableLayoutPanel1.Controls.Add(L_Height, 0, 0);
            tableLayoutPanel1.Controls.Add(FLP_CP, 1, 3);
            tableLayoutPanel1.Controls.Add(L_CP, 0, 3);
            tableLayoutPanel1.Controls.Add(FLP_Scale3, 1, 2);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.Size = new System.Drawing.Size(224, 96);
            tableLayoutPanel1.TabIndex = 2;
            // 
            // L_Scale
            // 
            L_Scale.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            L_Scale.Location = new System.Drawing.Point(0, 48);
            L_Scale.Margin = new System.Windows.Forms.Padding(0);
            L_Scale.Name = "L_Scale";
            L_Scale.Size = new System.Drawing.Size(80, 24);
            L_Scale.TabIndex = 6;
            L_Scale.Text = "Scale:";
            L_Scale.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FLP_Weight
            // 
            FLP_Weight.Controls.Add(NUD_WeightScalar);
            FLP_Weight.Controls.Add(TB_WeightAbs);
            FLP_Weight.Controls.Add(L_SizeW);
            FLP_Weight.Location = new System.Drawing.Point(80, 24);
            FLP_Weight.Margin = new System.Windows.Forms.Padding(0);
            FLP_Weight.Name = "FLP_Weight";
            FLP_Weight.Size = new System.Drawing.Size(144, 24);
            FLP_Weight.TabIndex = 4;
            // 
            // NUD_WeightScalar
            // 
            NUD_WeightScalar.Location = new System.Drawing.Point(0, 0);
            NUD_WeightScalar.Margin = new System.Windows.Forms.Padding(0);
            NUD_WeightScalar.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_WeightScalar.Name = "NUD_WeightScalar";
            NUD_WeightScalar.Size = new System.Drawing.Size(40, 23);
            NUD_WeightScalar.TabIndex = 3;
            NUD_WeightScalar.Value = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_WeightScalar.ValueChanged += NUD_WeightScalar_ValueChanged;
            NUD_WeightScalar.Click += ClickScalarEntry;
            // 
            // TB_WeightAbs
            // 
            TB_WeightAbs.Location = new System.Drawing.Point(40, 0);
            TB_WeightAbs.Margin = new System.Windows.Forms.Padding(0);
            TB_WeightAbs.Name = "TB_WeightAbs";
            TB_WeightAbs.Size = new System.Drawing.Size(64, 23);
            TB_WeightAbs.TabIndex = 4;
            TB_WeightAbs.Text = "123.456789";
            TB_WeightAbs.TextChanged += TB_WeightAbs_TextChanged;
            // 
            // L_SizeW
            // 
            L_SizeW.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            L_SizeW.Location = new System.Drawing.Point(112, 0);
            L_SizeW.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            L_SizeW.Name = "L_SizeW";
            L_SizeW.Size = new System.Drawing.Size(32, 20);
            L_SizeW.TabIndex = 9;
            L_SizeW.Text = "XS";
            L_SizeW.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FLP_Height
            // 
            FLP_Height.Controls.Add(NUD_HeightScalar);
            FLP_Height.Controls.Add(TB_HeightAbs);
            FLP_Height.Controls.Add(L_SizeH);
            FLP_Height.Location = new System.Drawing.Point(80, 0);
            FLP_Height.Margin = new System.Windows.Forms.Padding(0);
            FLP_Height.Name = "FLP_Height";
            FLP_Height.Size = new System.Drawing.Size(144, 24);
            FLP_Height.TabIndex = 3;
            // 
            // L_SizeH
            // 
            L_SizeH.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            L_SizeH.Location = new System.Drawing.Point(112, 0);
            L_SizeH.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            L_SizeH.Name = "L_SizeH";
            L_SizeH.Size = new System.Drawing.Size(32, 20);
            L_SizeH.TabIndex = 8;
            L_SizeH.Text = "XL";
            L_SizeH.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FLP_Scale3
            // 
            FLP_Scale3.Controls.Add(NUD_Scale);
            FLP_Scale3.Controls.Add(L_SizeS);
            FLP_Scale3.Location = new System.Drawing.Point(80, 48);
            FLP_Scale3.Margin = new System.Windows.Forms.Padding(0);
            FLP_Scale3.Name = "FLP_Scale3";
            FLP_Scale3.Size = new System.Drawing.Size(120, 24);
            FLP_Scale3.TabIndex = 5;
            // 
            // NUD_Scale
            // 
            NUD_Scale.Location = new System.Drawing.Point(0, 0);
            NUD_Scale.Margin = new System.Windows.Forms.Padding(0);
            NUD_Scale.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_Scale.Name = "NUD_Scale";
            NUD_Scale.Size = new System.Drawing.Size(40, 23);
            NUD_Scale.TabIndex = 3;
            NUD_Scale.Value = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_Scale.ValueChanged += NUD_Scale_ValueChanged;
            NUD_Scale.Click += ClickScalarEntry;
            // 
            // L_SizeS
            // 
            L_SizeS.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            L_SizeS.Location = new System.Drawing.Point(48, 0);
            L_SizeS.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            L_SizeS.Name = "L_SizeS";
            L_SizeS.Size = new System.Drawing.Size(40, 20);
            L_SizeS.TabIndex = 9;
            L_SizeS.Text = "XXXS";
            L_SizeS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SizeCP
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(tableLayoutPanel1);
            Name = "SizeCP";
            Size = new System.Drawing.Size(224, 96);
            FLP_CP.ResumeLayout(false);
            FLP_CP.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_HeightScalar).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            FLP_Weight.ResumeLayout(false);
            FLP_Weight.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_WeightScalar).EndInit();
            FLP_Height.ResumeLayout(false);
            FLP_Height.PerformLayout();
            FLP_Scale3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)NUD_Scale).EndInit();
            ResumeLayout(false);
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
