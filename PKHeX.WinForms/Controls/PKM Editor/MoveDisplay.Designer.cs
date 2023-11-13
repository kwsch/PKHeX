namespace PKHeX.WinForms.Controls
{
    partial class MoveDisplay
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
            FLP_Move = new System.Windows.Forms.FlowLayoutPanel();
            PB_Type = new System.Windows.Forms.PictureBox();
            L_Move = new System.Windows.Forms.Label();
            FLP_Move.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PB_Type).BeginInit();
            SuspendLayout();
            // 
            // FLP_Move
            // 
            FLP_Move.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            FLP_Move.Controls.Add(PB_Type);
            FLP_Move.Controls.Add(L_Move);
            FLP_Move.Location = new System.Drawing.Point(0, 0);
            FLP_Move.Name = "FLP_Move";
            FLP_Move.Size = new System.Drawing.Size(138, 24);
            FLP_Move.TabIndex = 18;
            // 
            // PB_Type
            // 
            PB_Type.Location = new System.Drawing.Point(0, 0);
            PB_Type.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            PB_Type.Name = "PB_Type";
            PB_Type.Size = new System.Drawing.Size(24, 24);
            PB_Type.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            PB_Type.TabIndex = 4;
            PB_Type.TabStop = false;
            // 
            // L_Move
            // 
            L_Move.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            L_Move.Location = new System.Drawing.Point(26, 0);
            L_Move.Margin = new System.Windows.Forms.Padding(0);
            L_Move.Name = "L_Move";
            L_Move.Size = new System.Drawing.Size(112, 24);
            L_Move.TabIndex = 79;
            L_Move.Text = "Name";
            L_Move.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MoveDisplay
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            Controls.Add(FLP_Move);
            Name = "MoveDisplay";
            Size = new System.Drawing.Size(138, 24);
            FLP_Move.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PB_Type).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel FLP_Move;
        private System.Windows.Forms.PictureBox PB_Type;
        private System.Windows.Forms.Label L_Move;
    }
}
