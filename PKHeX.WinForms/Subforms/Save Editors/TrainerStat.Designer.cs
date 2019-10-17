namespace PKHeX.WinForms.Subforms.Save_Editors
{
    partial class TrainerStat
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
            this.NUD_Stat = new System.Windows.Forms.NumericUpDown();
            this.L_Offset = new System.Windows.Forms.Label();
            this.L_Value = new System.Windows.Forms.Label();
            this.CB_Stats = new System.Windows.Forms.ComboBox();
            this.Tip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Stat)).BeginInit();
            this.SuspendLayout();
            // 
            // NUD_Stat
            // 
            this.NUD_Stat.Location = new System.Drawing.Point(40, 25);
            this.NUD_Stat.Name = "NUD_Stat";
            this.NUD_Stat.Size = new System.Drawing.Size(103, 20);
            this.NUD_Stat.TabIndex = 35;
            this.NUD_Stat.ValueChanged += new System.EventHandler(this.ChangeStatVal);
            // 
            // L_Offset
            // 
            this.L_Offset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.L_Offset.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.L_Offset.Location = new System.Drawing.Point(3, 48);
            this.L_Offset.Name = "L_Offset";
            this.L_Offset.Size = new System.Drawing.Size(140, 20);
            this.L_Offset.TabIndex = 34;
            this.L_Offset.Text = "(offset)";
            this.L_Offset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Value
            // 
            this.L_Value.AutoSize = true;
            this.L_Value.Location = new System.Drawing.Point(2, 28);
            this.L_Value.Name = "L_Value";
            this.L_Value.Size = new System.Drawing.Size(34, 13);
            this.L_Value.TabIndex = 32;
            this.L_Value.Text = "Value";
            // 
            // CB_Stats
            // 
            this.CB_Stats.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_Stats.DropDownHeight = 256;
            this.CB_Stats.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Stats.DropDownWidth = 200;
            this.CB_Stats.FormattingEnabled = true;
            this.CB_Stats.IntegralHeight = false;
            this.CB_Stats.Location = new System.Drawing.Point(3, 3);
            this.CB_Stats.Name = "CB_Stats";
            this.CB_Stats.Size = new System.Drawing.Size(140, 21);
            this.CB_Stats.TabIndex = 33;
            this.CB_Stats.SelectedIndexChanged += new System.EventHandler(this.ChangeStat);
            // 
            // TrainerStat
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.NUD_Stat);
            this.Controls.Add(this.L_Offset);
            this.Controls.Add(this.L_Value);
            this.Controls.Add(this.CB_Stats);
            this.Name = "TrainerStat";
            this.Size = new System.Drawing.Size(146, 72);
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Stat)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown NUD_Stat;
        private System.Windows.Forms.Label L_Offset;
        private System.Windows.Forms.Label L_Value;
        private System.Windows.Forms.ComboBox CB_Stats;
        private System.Windows.Forms.ToolTip Tip;
    }
}
