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
            TIDFields = new PKHeX.WinForms.Controls.TrainerTID();
            Label_SID = new System.Windows.Forms.Label();
            SIDFields = new PKHeX.WinForms.Controls.TrainerSID();
            TSVTooltip = new System.Windows.Forms.ToolTip(components);
            FLP.SuspendLayout();
            SuspendLayout();
            // 
            // FLP
            // 
            FLP.AutoSize = true;
            FLP.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            FLP.Controls.Add(Label_TID);
            FLP.Controls.Add(TIDFields);
            FLP.Controls.Add(Label_SID);
            FLP.Controls.Add(SIDFields);
            FLP.Dock = System.Windows.Forms.DockStyle.Fill;
            FLP.Location = new System.Drawing.Point(0, 0);
            FLP.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            FLP.Name = "FLP";
            FLP.Size = new System.Drawing.Size(88, 50);
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
            // TIDFields
            // 
            TIDFields.Location = new System.Drawing.Point(40, 0);
            TIDFields.Margin = new System.Windows.Forms.Padding(0);
            TIDFields.Name = "TIDFields";
            TIDFields.Size = new System.Drawing.Size(48, 25);
            TIDFields.TabIndex = 1;
            // 
            // Label_SID
            // 
            Label_SID.Location = new System.Drawing.Point(0, 25);
            Label_SID.Margin = new System.Windows.Forms.Padding(0);
            Label_SID.Name = "Label_SID";
            Label_SID.Size = new System.Drawing.Size(40, 24);
            Label_SID.TabIndex = 8;
            Label_SID.Text = "SID:";
            Label_SID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SIDFields
            // 
            SIDFields.Location = new System.Drawing.Point(40, 25);
            SIDFields.Margin = new System.Windows.Forms.Padding(0);
            SIDFields.Name = "SIDFields";
            SIDFields.Size = new System.Drawing.Size(40, 25);
            SIDFields.TabIndex = 2;
            // 
            // TrainerID
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(FLP);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "TrainerID";
            Size = new System.Drawing.Size(88, 50);
            FLP.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel FLP;
        private System.Windows.Forms.Label Label_SID;
        private System.Windows.Forms.Label Label_TID;
        private TrainerTID TIDFields;
        private TrainerSID SIDFields;
        private System.Windows.Forms.ToolTip TSVTooltip;
    }
}
