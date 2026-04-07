namespace PKHeX.WinForms
{
    partial class SAV_DonutGenerator9a
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            L_FlavorPool = new System.Windows.Forms.Label();
            CLB_Flavors = new System.Windows.Forms.CheckedListBox();
            L_Range = new System.Windows.Forms.Label();
            NUD_Start = new System.Windows.Forms.NumericUpDown();
            NUD_End = new System.Windows.Forms.NumericUpDown();
            L_RangeSeparator = new System.Windows.Forms.Label();
            B_Generate = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)NUD_Start).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_End).BeginInit();
            SuspendLayout();
            // 
            // L_FlavorPool
            // 
            L_FlavorPool.AutoSize = true;
            L_FlavorPool.Location = new System.Drawing.Point(12, 12);
            L_FlavorPool.Name = "L_FlavorPool";
            L_FlavorPool.Size = new System.Drawing.Size(73, 17);
            L_FlavorPool.TabIndex = 0;
            L_FlavorPool.Text = "Flavor Pool";
            // 
            // CLB_Flavors
            // 
            CLB_Flavors.CheckOnClick = true;
            CLB_Flavors.FormattingEnabled = true;
            CLB_Flavors.Location = new System.Drawing.Point(12, 30);
            CLB_Flavors.Name = "CLB_Flavors";
            CLB_Flavors.Size = new System.Drawing.Size(300, 184);
            CLB_Flavors.TabIndex = 1;
            // 
            // L_Range
            // 
            L_Range.AutoSize = true;
            L_Range.Location = new System.Drawing.Point(328, 108);
            L_Range.Name = "L_Range";
            L_Range.Size = new System.Drawing.Size(112, 17);
            L_Range.TabIndex = 4;
            L_Range.Text = "Range [start, end)";
            // 
            // NUD_Start
            // 
            NUD_Start.Location = new System.Drawing.Point(328, 126);
            NUD_Start.Name = "NUD_Start";
            NUD_Start.Size = new System.Drawing.Size(80, 25);
            NUD_Start.TabIndex = 5;
            // 
            // NUD_End
            // 
            NUD_End.Location = new System.Drawing.Point(444, 126);
            NUD_End.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            NUD_End.Name = "NUD_End";
            NUD_End.Size = new System.Drawing.Size(80, 25);
            NUD_End.TabIndex = 7;
            NUD_End.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // L_RangeSeparator
            // 
            L_RangeSeparator.AutoSize = true;
            L_RangeSeparator.Location = new System.Drawing.Point(420, 129);
            L_RangeSeparator.Name = "L_RangeSeparator";
            L_RangeSeparator.Size = new System.Drawing.Size(20, 17);
            L_RangeSeparator.TabIndex = 6;
            L_RangeSeparator.Text = "to";
            // 
            // B_Generate
            // 
            B_Generate.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Generate.Location = new System.Drawing.Point(349, 193);
            B_Generate.Name = "B_Generate";
            B_Generate.Size = new System.Drawing.Size(84, 27);
            B_Generate.TabIndex = 9;
            B_Generate.Text = "Generate";
            B_Generate.UseVisualStyleBackColor = true;
            B_Generate.Click += B_Generate_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(440, 193);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(84, 27);
            B_Cancel.TabIndex = 10;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // SAV_DonutGenerator9a
            // 
            AcceptButton = B_Generate;
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            CancelButton = B_Cancel;
            ClientSize = new System.Drawing.Size(536, 232);
            Controls.Add(B_Cancel);
            Controls.Add(B_Generate);
            Controls.Add(L_RangeSeparator);
            Controls.Add(NUD_End);
            Controls.Add(NUD_Start);
            Controls.Add(L_Range);
            Controls.Add(CLB_Flavors);
            Controls.Add(L_FlavorPool);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_DonutGenerator9a";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Random Donut Generator";
            ((System.ComponentModel.ISupportInitialize)NUD_Start).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_End).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label L_FlavorPool;
        private System.Windows.Forms.CheckedListBox CLB_Flavors;
        private System.Windows.Forms.Label L_Range;
        private System.Windows.Forms.NumericUpDown NUD_Start;
        private System.Windows.Forms.NumericUpDown NUD_End;
        private System.Windows.Forms.Label L_RangeSeparator;
        private System.Windows.Forms.Button B_Generate;
        private System.Windows.Forms.Button B_Cancel;
    }
}
