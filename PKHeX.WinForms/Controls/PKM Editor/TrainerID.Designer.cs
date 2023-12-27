namespace PKHeX.WinForms.Controls
{
    partial class TrainerID
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
            FLP = new System.Windows.Forms.FlowLayoutPanel();
            Label_TID = new System.Windows.Forms.Label();
            TB_TID = new System.Windows.Forms.MaskedTextBox();
            TB_TID7 = new System.Windows.Forms.MaskedTextBox();
            Label_SID = new System.Windows.Forms.Label();
            TB_SID = new System.Windows.Forms.MaskedTextBox();
            TB_SID7 = new System.Windows.Forms.MaskedTextBox();
            TSVTooltip = new System.Windows.Forms.ToolTip(components);
            FLP.SuspendLayout();
            SuspendLayout();
            // 
            // FLP
            // 
            FLP.Controls.Add(Label_TID);
            FLP.Controls.Add(TB_TID);
            FLP.Controls.Add(TB_TID7);
            FLP.Controls.Add(Label_SID);
            FLP.Controls.Add(TB_SID);
            FLP.Controls.Add(TB_SID7);
            FLP.Dock = System.Windows.Forms.DockStyle.Fill;
            FLP.Location = new System.Drawing.Point(0, 0);
            FLP.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            FLP.Name = "FLP";
            FLP.Size = new System.Drawing.Size(128, 48);
            FLP.TabIndex = 0;
            // 
            // Label_TID
            // 
            Label_TID.Location = new System.Drawing.Point(0, 0);
            Label_TID.Margin = new System.Windows.Forms.Padding(0);
            Label_TID.Name = "Label_TID";
            Label_TID.Size = new System.Drawing.Size(40, 24);
            Label_TID.TabIndex = 7;
            Label_TID.Text = "TID:";
            Label_TID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TB_TID
            // 
            TB_TID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_TID.Location = new System.Drawing.Point(40, 0);
            TB_TID.Margin = new System.Windows.Forms.Padding(0);
            TB_TID.Mask = "00000";
            TB_TID.Name = "TB_TID";
            TB_TID.Size = new System.Drawing.Size(40, 23);
            TB_TID.TabIndex = 1;
            TB_TID.Text = "12345";
            TB_TID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            TB_TID.TextChanged += Update_ID;
            TB_TID.MouseHover += UpdateTSV;
            // 
            // TB_TID7
            // 
            TB_TID7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_TID7.Location = new System.Drawing.Point(80, 0);
            TB_TID7.Margin = new System.Windows.Forms.Padding(0);
            TB_TID7.Mask = "000000";
            TB_TID7.Name = "TB_TID7";
            TB_TID7.Size = new System.Drawing.Size(48, 23);
            TB_TID7.TabIndex = 2;
            TB_TID7.Text = "123456";
            TB_TID7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            TB_TID7.TextChanged += Update_ID;
            TB_TID7.MouseHover += UpdateTSV;
            // 
            // Label_SID
            // 
            Label_SID.Location = new System.Drawing.Point(0, 24);
            Label_SID.Margin = new System.Windows.Forms.Padding(0);
            Label_SID.Name = "Label_SID";
            Label_SID.Size = new System.Drawing.Size(40, 24);
            Label_SID.TabIndex = 8;
            Label_SID.Text = "SID:";
            Label_SID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TB_SID
            // 
            TB_SID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_SID.Location = new System.Drawing.Point(40, 24);
            TB_SID.Margin = new System.Windows.Forms.Padding(0);
            TB_SID.Mask = "00000";
            TB_SID.Name = "TB_SID";
            TB_SID.Size = new System.Drawing.Size(40, 23);
            TB_SID.TabIndex = 3;
            TB_SID.Text = "12345";
            TB_SID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            TB_SID.TextChanged += Update_ID;
            TB_SID.MouseHover += UpdateTSV;
            // 
            // TB_SID7
            // 
            TB_SID7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_SID7.Location = new System.Drawing.Point(80, 24);
            TB_SID7.Margin = new System.Windows.Forms.Padding(0);
            TB_SID7.Mask = "0000";
            TB_SID7.Name = "TB_SID7";
            TB_SID7.Size = new System.Drawing.Size(32, 23);
            TB_SID7.TabIndex = 4;
            TB_SID7.Text = "1234";
            TB_SID7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            TB_SID7.TextChanged += Update_ID;
            TB_SID7.MouseHover += UpdateTSV;
            // 
            // TrainerID
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(FLP);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "TrainerID";
            Size = new System.Drawing.Size(128, 48);
            FLP.ResumeLayout(false);
            FLP.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel FLP;
        private System.Windows.Forms.MaskedTextBox TB_SID;
        private System.Windows.Forms.MaskedTextBox TB_TID;
        private System.Windows.Forms.Label Label_SID;
        private System.Windows.Forms.Label Label_TID;
        private System.Windows.Forms.MaskedTextBox TB_TID7;
        private System.Windows.Forms.MaskedTextBox TB_SID7;
        private System.Windows.Forms.ToolTip TSVTooltip;
    }
}
