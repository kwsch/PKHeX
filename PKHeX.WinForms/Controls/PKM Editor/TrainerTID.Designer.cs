namespace PKHeX.WinForms.Controls
{
    partial class TrainerTID
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
            TB_Five = new System.Windows.Forms.MaskedTextBox();
            TB_Six = new System.Windows.Forms.MaskedTextBox();
            SuspendLayout();
            // 
            // TB_Five
            // 
            TB_Five.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_Five.Location = new System.Drawing.Point(0, 0);
            TB_Five.Margin = new System.Windows.Forms.Padding(0);
            TB_Five.Mask = "00000";
            TB_Five.Name = "TB_Five";
            TB_Five.Size = new System.Drawing.Size(40, 25);
            TB_Five.TabIndex = 0;
            TB_Five.Text = "12345";
            TB_Five.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            TB_Five.TextChanged += RaiseValueChanged;
            // 
            // TB_Six
            // 
            TB_Six.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_Six.Location = new System.Drawing.Point(0, 0);
            TB_Six.Margin = new System.Windows.Forms.Padding(0);
            TB_Six.Mask = "000000";
            TB_Six.Name = "TB_Six";
            TB_Six.Size = new System.Drawing.Size(48, 25);
            TB_Six.TabIndex = 1;
            TB_Six.Text = "123456";
            TB_Six.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            TB_Six.TextChanged += RaiseValueChanged;
            // 
            // TrainerTID
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(TB_Six);
            Controls.Add(TB_Five);
            Margin = new System.Windows.Forms.Padding(0);
            Name = "TrainerTID";
            Size = new System.Drawing.Size(48, 25);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MaskedTextBox TB_Five;
        private System.Windows.Forms.MaskedTextBox TB_Six;
    }
}
