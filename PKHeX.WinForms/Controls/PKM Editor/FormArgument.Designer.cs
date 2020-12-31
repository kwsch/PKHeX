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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.NUD_FormArg = new System.Windows.Forms.NumericUpDown();
            this.CB_FormArg = new System.Windows.Forms.ComboBox();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_FormArg)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.NUD_FormArg);
            this.flowLayoutPanel1.Controls.Add(this.CB_FormArg);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(162, 22);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // NUD_FormArg
            // 
            this.NUD_FormArg.Location = new System.Drawing.Point(0, 0);
            this.NUD_FormArg.Margin = new System.Windows.Forms.Padding(0);
            this.NUD_FormArg.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.NUD_FormArg.Name = "NUD_FormArg";
            this.NUD_FormArg.Size = new System.Drawing.Size(44, 20);
            this.NUD_FormArg.TabIndex = 0;
            this.NUD_FormArg.Value = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.NUD_FormArg.Visible = false;
            this.NUD_FormArg.ValueChanged += new System.EventHandler(this.NUD_FormArg_ValueChanged);
            // 
            // CB_FormArg
            // 
            this.CB_FormArg.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_FormArg.FormattingEnabled = true;
            this.CB_FormArg.Items.AddRange(new object[] {
            ""});
            this.CB_FormArg.Location = new System.Drawing.Point(44, 0);
            this.CB_FormArg.Margin = new System.Windows.Forms.Padding(0);
            this.CB_FormArg.Name = "CB_FormArg";
            this.CB_FormArg.Size = new System.Drawing.Size(66, 21);
            this.CB_FormArg.TabIndex = 1;
            this.CB_FormArg.Visible = false;
            this.CB_FormArg.SelectedIndexChanged += new System.EventHandler(this.CB_FormArg_SelectedIndexChanged);
            // 
            // FormArgument
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "FormArgument";
            this.Size = new System.Drawing.Size(162, 22);
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NUD_FormArg)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.NumericUpDown NUD_FormArg;
        private System.Windows.Forms.ComboBox CB_FormArg;
    }
}
