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
            this.components = new System.ComponentModel.Container();
            this.FLP = new System.Windows.Forms.FlowLayoutPanel();
            this.Label_TID = new System.Windows.Forms.Label();
            this.TB_TID = new System.Windows.Forms.MaskedTextBox();
            this.TB_TID7 = new System.Windows.Forms.MaskedTextBox();
            this.Label_SID = new System.Windows.Forms.Label();
            this.TB_SID = new System.Windows.Forms.MaskedTextBox();
            this.TB_SID7 = new System.Windows.Forms.MaskedTextBox();
            this.TSVTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.FLP.SuspendLayout();
            this.SuspendLayout();
            // 
            // FLP
            // 
            this.FLP.Controls.Add(this.Label_TID);
            this.FLP.Controls.Add(this.TB_TID);
            this.FLP.Controls.Add(this.TB_TID7);
            this.FLP.Controls.Add(this.Label_SID);
            this.FLP.Controls.Add(this.TB_SID);
            this.FLP.Controls.Add(this.TB_SID7);
            this.FLP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FLP.Location = new System.Drawing.Point(0, 0);
            this.FLP.Name = "FLP";
            this.FLP.Size = new System.Drawing.Size(125, 54);
            this.FLP.TabIndex = 0;
            // 
            // Label_TID
            // 
            this.Label_TID.AutoSize = true;
            this.Label_TID.Location = new System.Drawing.Point(3, 6);
            this.Label_TID.Margin = new System.Windows.Forms.Padding(3, 6, 2, 0);
            this.Label_TID.Name = "Label_TID";
            this.Label_TID.Size = new System.Drawing.Size(28, 13);
            this.Label_TID.TabIndex = 7;
            this.Label_TID.Text = "TID:";
            this.Label_TID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TB_TID
            // 
            this.TB_TID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_TID.Location = new System.Drawing.Point(33, 3);
            this.TB_TID.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.TB_TID.Mask = "00000";
            this.TB_TID.Name = "TB_TID";
            this.TB_TID.Size = new System.Drawing.Size(40, 20);
            this.TB_TID.TabIndex = 5;
            this.TB_TID.Text = "12345";
            this.TB_TID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_TID.TextChanged += new System.EventHandler(this.Update_ID);
            this.TB_TID.MouseHover += new System.EventHandler(this.UpdateTSV);
            // 
            // TB_TID7
            // 
            this.TB_TID7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_TID7.Location = new System.Drawing.Point(76, 3);
            this.TB_TID7.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.TB_TID7.Mask = "000000";
            this.TB_TID7.Name = "TB_TID7";
            this.TB_TID7.Size = new System.Drawing.Size(42, 20);
            this.TB_TID7.TabIndex = 9;
            this.TB_TID7.Text = "123456";
            this.TB_TID7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_TID7.TextChanged += new System.EventHandler(this.Update_ID);
            this.TB_TID7.MouseHover += new System.EventHandler(this.UpdateTSV);
            // 
            // Label_SID
            // 
            this.Label_SID.AutoSize = true;
            this.Label_SID.Location = new System.Drawing.Point(3, 29);
            this.Label_SID.Margin = new System.Windows.Forms.Padding(3, 6, 2, 0);
            this.Label_SID.Name = "Label_SID";
            this.Label_SID.Size = new System.Drawing.Size(28, 13);
            this.Label_SID.TabIndex = 8;
            this.Label_SID.Text = "SID:";
            this.Label_SID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TB_SID
            // 
            this.TB_SID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_SID.Location = new System.Drawing.Point(33, 26);
            this.TB_SID.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.TB_SID.Mask = "00000";
            this.TB_SID.Name = "TB_SID";
            this.TB_SID.Size = new System.Drawing.Size(40, 20);
            this.TB_SID.TabIndex = 6;
            this.TB_SID.Text = "12345";
            this.TB_SID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_SID.TextChanged += new System.EventHandler(this.Update_ID);
            this.TB_SID.MouseHover += new System.EventHandler(this.UpdateTSV);
            // 
            // TB_SID7
            // 
            this.TB_SID7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_SID7.Location = new System.Drawing.Point(76, 26);
            this.TB_SID7.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.TB_SID7.Mask = "0000";
            this.TB_SID7.Name = "TB_SID7";
            this.TB_SID7.Size = new System.Drawing.Size(30, 20);
            this.TB_SID7.TabIndex = 10;
            this.TB_SID7.Text = "1234";
            this.TB_SID7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_SID7.TextChanged += new System.EventHandler(this.Update_ID);
            this.TB_SID7.MouseHover += new System.EventHandler(this.UpdateTSV);
            // 
            // TrainerID
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.FLP);
            this.Name = "TrainerID";
            this.Size = new System.Drawing.Size(125, 54);
            this.FLP.ResumeLayout(false);
            this.FLP.PerformLayout();
            this.ResumeLayout(false);

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
