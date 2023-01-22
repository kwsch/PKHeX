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
            this.PB_Triangle = new System.Windows.Forms.PictureBox();
            this.TB_PP = new System.Windows.Forms.MaskedTextBox();
            this.CB_PPUps = new System.Windows.Forms.ComboBox();
            this.CB_Move = new System.Windows.Forms.ComboBox();
            this.FLP_Move = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Triangle)).BeginInit();
            this.FLP_Move.SuspendLayout();
            this.SuspendLayout();
            // 
            // PB_Triangle
            // 
            this.PB_Triangle.Image = global::PKHeX.WinForms.Properties.Resources.warn;
            this.PB_Triangle.Location = new System.Drawing.Point(0, 0);
            this.PB_Triangle.Name = "PB_Triangle";
            this.PB_Triangle.Size = new System.Drawing.Size(24, 24);
            this.PB_Triangle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PB_Triangle.TabIndex = 15;
            this.PB_Triangle.TabStop = false;
            this.PB_Triangle.Visible = false;
            // 
            // TB_PP
            // 
            this.TB_PP.Location = new System.Drawing.Point(128, 0);
            this.TB_PP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.TB_PP.Mask = "000";
            this.TB_PP.Name = "TB_PP";
            this.TB_PP.PromptChar = ' ';
            this.TB_PP.Size = new System.Drawing.Size(32, 23);
            this.TB_PP.TabIndex = 1;
            // 
            // CB_PPUps
            // 
            this.CB_PPUps.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_PPUps.FormattingEnabled = true;
            this.CB_PPUps.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3"});
            this.CB_PPUps.Location = new System.Drawing.Point(164, 0);
            this.CB_PPUps.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.CB_PPUps.Name = "CB_PPUps";
            this.CB_PPUps.Size = new System.Drawing.Size(40, 23);
            this.CB_PPUps.TabIndex = 2;
            // 
            // CB_Move
            // 
            this.CB_Move.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_Move.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Move.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.CB_Move.DropDownHeight = 400;
            this.CB_Move.FormattingEnabled = true;
            this.CB_Move.IntegralHeight = false;
            this.CB_Move.Location = new System.Drawing.Point(0, 0);
            this.CB_Move.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.CB_Move.Name = "CB_Move";
            this.CB_Move.Size = new System.Drawing.Size(120, 24);
            this.CB_Move.TabIndex = 0;
            // 
            // FLP_Move
            // 
            this.FLP_Move.Controls.Add(this.CB_Move);
            this.FLP_Move.Controls.Add(this.TB_PP);
            this.FLP_Move.Controls.Add(this.CB_PPUps);
            this.FLP_Move.Location = new System.Drawing.Point(24, 0);
            this.FLP_Move.Name = "FLP_Move";
            this.FLP_Move.Size = new System.Drawing.Size(216, 24);
            this.FLP_Move.TabIndex = 18;
            // 
            // MoveChoice
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.PB_Triangle);
            this.Controls.Add(this.FLP_Move);
            this.Name = "MoveChoice";
            this.Size = new System.Drawing.Size(240, 24);
            ((System.ComponentModel.ISupportInitialize)(this.PB_Triangle)).EndInit();
            this.FLP_Move.ResumeLayout(false);
            this.FLP_Move.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox PB_Triangle;
        private System.Windows.Forms.MaskedTextBox TB_PP;
        internal System.Windows.Forms.ComboBox CB_PPUps;
        private System.Windows.Forms.FlowLayoutPanel FLP_Move;
        internal System.Windows.Forms.ComboBox CB_Move;
    }
}
