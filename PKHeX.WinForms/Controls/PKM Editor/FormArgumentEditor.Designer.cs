namespace PKHeX.WinForms.Controls
{
    partial class FormArgumentEditor
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
            components = new System.ComponentModel.Container();
            TLP_FormArg = new System.Windows.Forms.TableLayoutPanel();
            CB_FormArg = new System.Windows.Forms.ComboBox();
            NUD_FormArg = new System.Windows.Forms.NumericUpDown();
            FLP_FormArg = new System.Windows.Forms.FlowLayoutPanel();
            NUD_FormArgMax = new System.Windows.Forms.NumericUpDown();
            NUD_FormArgElapsed = new System.Windows.Forms.NumericUpDown();
            NUD_FormArgRemain = new System.Windows.Forms.NumericUpDown();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            TLP_FormArg.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_FormArg).BeginInit();
            FLP_FormArg.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_FormArgMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_FormArgElapsed).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_FormArgRemain).BeginInit();
            SuspendLayout();
            // 
            // TLP_FormArg
            // 
            TLP_FormArg.AutoSize = true;
            TLP_FormArg.ColumnCount = 1;
            TLP_FormArg.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_FormArg.Controls.Add(CB_FormArg, 0, 0);
            TLP_FormArg.Controls.Add(NUD_FormArg, 0, 1);
            TLP_FormArg.Controls.Add(FLP_FormArg, 0, 2);
            TLP_FormArg.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_FormArg.Location = new System.Drawing.Point(0, 0);
            TLP_FormArg.Name = "TLP_FormArg";
            TLP_FormArg.RowCount = 3;
            TLP_FormArg.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_FormArg.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_FormArg.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_FormArg.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            TLP_FormArg.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            TLP_FormArg.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            TLP_FormArg.Size = new System.Drawing.Size(144, 75);
            TLP_FormArg.TabIndex = 3;
            // 
            // CB_FormArg
            // 
            CB_FormArg.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_FormArg.FormattingEnabled = true;
            CB_FormArg.Items.AddRange(new object[] { "" });
            CB_FormArg.Location = new System.Drawing.Point(0, 0);
            CB_FormArg.Margin = new System.Windows.Forms.Padding(0);
            CB_FormArg.Name = "CB_FormArg";
            CB_FormArg.Size = new System.Drawing.Size(144, 25);
            CB_FormArg.TabIndex = 3;
            CB_FormArg.Visible = false;
            CB_FormArg.SelectedIndexChanged += SelectionChanged;
            // 
            // NUD_FormArg
            // 
            NUD_FormArg.Location = new System.Drawing.Point(0, 25);
            NUD_FormArg.Margin = new System.Windows.Forms.Padding(0);
            NUD_FormArg.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            NUD_FormArg.Name = "NUD_FormArg";
            NUD_FormArg.Size = new System.Drawing.Size(48, 25);
            NUD_FormArg.TabIndex = 0;
            NUD_FormArg.Value = new decimal(new int[] { 9999, 0, 0, 0 });
            NUD_FormArg.Visible = false;
            NUD_FormArg.ValueChanged += SelectionChanged;
            // 
            // FLP_FormArg
            // 
            FLP_FormArg.AutoSize = true;
            FLP_FormArg.Controls.Add(NUD_FormArgMax);
            FLP_FormArg.Controls.Add(NUD_FormArgElapsed);
            FLP_FormArg.Controls.Add(NUD_FormArgRemain);
            FLP_FormArg.Location = new System.Drawing.Point(0, 50);
            FLP_FormArg.Margin = new System.Windows.Forms.Padding(0);
            FLP_FormArg.Name = "FLP_FormArg";
            FLP_FormArg.Size = new System.Drawing.Size(123, 25);
            FLP_FormArg.TabIndex = 11;
            // 
            // NUD_FormArgMax
            // 
            NUD_FormArgMax.Location = new System.Drawing.Point(0, 0);
            NUD_FormArgMax.Margin = new System.Windows.Forms.Padding(0);
            NUD_FormArgMax.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_FormArgMax.Name = "NUD_FormArgMax";
            NUD_FormArgMax.Size = new System.Drawing.Size(41, 25);
            NUD_FormArgMax.TabIndex = 9;
            NUD_FormArgMax.Value = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_FormArgMax.Visible = false;
            // 
            // NUD_FormArgElapsed
            // 
            NUD_FormArgElapsed.Location = new System.Drawing.Point(41, 0);
            NUD_FormArgElapsed.Margin = new System.Windows.Forms.Padding(0);
            NUD_FormArgElapsed.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_FormArgElapsed.Name = "NUD_FormArgElapsed";
            NUD_FormArgElapsed.Size = new System.Drawing.Size(41, 25);
            NUD_FormArgElapsed.TabIndex = 9;
            NUD_FormArgElapsed.Value = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_FormArgElapsed.Visible = false;
            NUD_FormArgElapsed.ValueChanged += SelectionChanged;
            // 
            // NUD_FormArgRemain
            // 
            NUD_FormArgRemain.Location = new System.Drawing.Point(82, 0);
            NUD_FormArgRemain.Margin = new System.Windows.Forms.Padding(0);
            NUD_FormArgRemain.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_FormArgRemain.Name = "NUD_FormArgRemain";
            NUD_FormArgRemain.Size = new System.Drawing.Size(41, 25);
            NUD_FormArgRemain.TabIndex = 10;
            NUD_FormArgRemain.Value = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_FormArgRemain.Visible = false;
            NUD_FormArgRemain.ValueChanged += SelectionChanged;
            // 
            // FormArgumentEditor
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            Controls.Add(TLP_FormArg);
            Name = "FormArgumentEditor";
            Size = new System.Drawing.Size(144, 75);
            TLP_FormArg.ResumeLayout(false);
            TLP_FormArg.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_FormArg).EndInit();
            FLP_FormArg.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)NUD_FormArgMax).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_FormArgElapsed).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_FormArgRemain).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TLP_FormArg;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.NumericUpDown NUD_FormArg;
        private System.Windows.Forms.ComboBox CB_FormArg;
        private System.Windows.Forms.FlowLayoutPanel FLP_FormArg;
        private System.Windows.Forms.NumericUpDown NUD_FormArgMax;
        private System.Windows.Forms.NumericUpDown NUD_FormArgElapsed;
        private System.Windows.Forms.NumericUpDown NUD_FormArgRemain;
    }
}
