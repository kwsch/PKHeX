namespace PKHeX.WinForms.Controls
{
    partial class MoveChoice
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
            PB_Triangle = new System.Windows.Forms.PictureBox();
            TB_PP = new System.Windows.Forms.MaskedTextBox();
            CB_PPUps = new System.Windows.Forms.ComboBox();
            CB_Move = new System.Windows.Forms.ComboBox();
            FLP_Move = new System.Windows.Forms.FlowLayoutPanel();
            PB_Type = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)PB_Triangle).BeginInit();
            FLP_Move.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PB_Type).BeginInit();
            SuspendLayout();
            // 
            // PB_Triangle
            // 
            PB_Triangle.Image = Properties.Resources.warn;
            PB_Triangle.Location = new System.Drawing.Point(0, 0);
            PB_Triangle.Name = "PB_Triangle";
            PB_Triangle.Size = new System.Drawing.Size(24, 24);
            PB_Triangle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            PB_Triangle.TabIndex = 15;
            PB_Triangle.TabStop = false;
            PB_Triangle.Visible = false;
            // 
            // TB_PP
            // 
            TB_PP.Location = new System.Drawing.Point(160, 0);
            TB_PP.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            TB_PP.Mask = "000";
            TB_PP.Name = "TB_PP";
            TB_PP.PromptChar = ' ';
            TB_PP.Size = new System.Drawing.Size(24, 23);
            TB_PP.TabIndex = 1;
            // 
            // CB_PPUps
            // 
            CB_PPUps.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_PPUps.FormattingEnabled = true;
            CB_PPUps.Items.AddRange(new object[] { "0", "1", "2", "3" });
            CB_PPUps.Location = new System.Drawing.Point(186, 0);
            CB_PPUps.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            CB_PPUps.Name = "CB_PPUps";
            CB_PPUps.Size = new System.Drawing.Size(32, 23);
            CB_PPUps.TabIndex = 2;
            // 
            // CB_Move
            // 
            CB_Move.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Move.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Move.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            CB_Move.DropDownHeight = 400;
            CB_Move.FormattingEnabled = true;
            CB_Move.IntegralHeight = false;
            CB_Move.Location = new System.Drawing.Point(32, 0);
            CB_Move.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            CB_Move.Name = "CB_Move";
            CB_Move.Size = new System.Drawing.Size(120, 24);
            CB_Move.TabIndex = 0;
            CB_Move.SelectedIndexChanged += CB_Move_SelectedIndexChanged;
            // 
            // FLP_Move
            // 
            FLP_Move.Controls.Add(PB_Type);
            FLP_Move.Controls.Add(CB_Move);
            FLP_Move.Controls.Add(TB_PP);
            FLP_Move.Controls.Add(CB_PPUps);
            FLP_Move.Location = new System.Drawing.Point(24, 0);
            FLP_Move.Name = "FLP_Move";
            FLP_Move.Size = new System.Drawing.Size(224, 24);
            FLP_Move.TabIndex = 18;
            // 
            // PB_Type
            // 
            PB_Type.Location = new System.Drawing.Point(0, 0);
            PB_Type.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            PB_Type.Name = "PB_Type";
            PB_Type.Size = new System.Drawing.Size(24, 24);
            PB_Type.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            PB_Type.TabIndex = 4;
            PB_Type.TabStop = false;
            // 
            // MoveChoice
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(PB_Triangle);
            Controls.Add(FLP_Move);
            Name = "MoveChoice";
            Size = new System.Drawing.Size(248, 24);
            ((System.ComponentModel.ISupportInitialize)PB_Triangle).EndInit();
            FLP_Move.ResumeLayout(false);
            FLP_Move.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)PB_Type).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.PictureBox PB_Triangle;
        private System.Windows.Forms.MaskedTextBox TB_PP;
        internal System.Windows.Forms.ComboBox CB_PPUps;
        private System.Windows.Forms.FlowLayoutPanel FLP_Move;
        internal System.Windows.Forms.ComboBox CB_Move;
        private System.Windows.Forms.PictureBox PB_Type;
    }
}
