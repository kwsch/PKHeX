namespace PKHeX.WinForms.Controls
{
    partial class FormArgument
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
            FLP_FormArg = new System.Windows.Forms.FlowLayoutPanel();
            NUD_FormArg = new System.Windows.Forms.NumericUpDown();
            CB_FormArg = new System.Windows.Forms.ComboBox();
            FLP_FormArg.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_FormArg).BeginInit();
            SuspendLayout();
            // 
            // FLP_FormArg
            // 
            FLP_FormArg.AutoSize = true;
            FLP_FormArg.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            FLP_FormArg.Controls.Add(NUD_FormArg);
            FLP_FormArg.Controls.Add(CB_FormArg);
            FLP_FormArg.Dock = System.Windows.Forms.DockStyle.Fill;
            FLP_FormArg.Location = new System.Drawing.Point(0, 0);
            FLP_FormArg.Margin = new System.Windows.Forms.Padding(0);
            FLP_FormArg.Name = "FLP_FormArg";
            FLP_FormArg.Size = new System.Drawing.Size(119, 25);
            FLP_FormArg.TabIndex = 1;
            // 
            // NUD_FormArg
            // 
            NUD_FormArg.Location = new System.Drawing.Point(0, 0);
            NUD_FormArg.Margin = new System.Windows.Forms.Padding(0);
            NUD_FormArg.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            NUD_FormArg.Name = "NUD_FormArg";
            NUD_FormArg.Size = new System.Drawing.Size(44, 25);
            NUD_FormArg.TabIndex = 0;
            NUD_FormArg.Value = new decimal(new int[] { 9999, 0, 0, 0 });
            NUD_FormArg.Visible = false;
            NUD_FormArg.ValueChanged += NUD_FormArg_ValueChanged;
            // 
            // CB_FormArg
            // 
            CB_FormArg.Dock = System.Windows.Forms.DockStyle.Fill;
            CB_FormArg.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_FormArg.FormattingEnabled = true;
            CB_FormArg.Items.AddRange(new object[] { "" });
            CB_FormArg.Location = new System.Drawing.Point(44, 0);
            CB_FormArg.Margin = new System.Windows.Forms.Padding(0);
            CB_FormArg.Name = "CB_FormArg";
            CB_FormArg.Size = new System.Drawing.Size(75, 25);
            CB_FormArg.TabIndex = 1;
            CB_FormArg.Visible = false;
            CB_FormArg.SelectedIndexChanged += CB_FormArg_SelectedIndexChanged;
            // 
            // FormArgument
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            Controls.Add(FLP_FormArg);
            Name = "FormArgument";
            Size = new System.Drawing.Size(119, 25);
            FLP_FormArg.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)NUD_FormArg).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel FLP_FormArg;
        private System.Windows.Forms.NumericUpDown NUD_FormArg;
        private System.Windows.Forms.ComboBox CB_FormArg;
    }
}
