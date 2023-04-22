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
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            NUD_FormArg = new System.Windows.Forms.NumericUpDown();
            CB_FormArg = new System.Windows.Forms.ComboBox();
            flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_FormArg).BeginInit();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(NUD_FormArg);
            flowLayoutPanel1.Controls.Add(CB_FormArg);
            flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(143, 22);
            flowLayoutPanel1.TabIndex = 1;
            // 
            // NUD_FormArg
            // 
            NUD_FormArg.Location = new System.Drawing.Point(0, 0);
            NUD_FormArg.Margin = new System.Windows.Forms.Padding(0);
            NUD_FormArg.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            NUD_FormArg.Name = "NUD_FormArg";
            NUD_FormArg.Size = new System.Drawing.Size(44, 23);
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
            CB_FormArg.Size = new System.Drawing.Size(75, 23);
            CB_FormArg.TabIndex = 1;
            CB_FormArg.Visible = false;
            CB_FormArg.SelectedIndexChanged += CB_FormArg_SelectedIndexChanged;
            // 
            // FormArgument
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(flowLayoutPanel1);
            Name = "FormArgument";
            Size = new System.Drawing.Size(143, 22);
            flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)NUD_FormArg).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.NumericUpDown NUD_FormArg;
        private System.Windows.Forms.ComboBox CB_FormArg;
    }
}
