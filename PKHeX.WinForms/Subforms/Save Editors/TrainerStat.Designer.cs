namespace PKHeX.WinForms.Controls
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
            components = new System.ComponentModel.Container();
            NUD_Stat = new System.Windows.Forms.NumericUpDown();
            L_Offset = new System.Windows.Forms.Label();
            L_Value = new System.Windows.Forms.Label();
            CB_Stats = new System.Windows.Forms.ComboBox();
            Tip = new System.Windows.Forms.ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)NUD_Stat).BeginInit();
            SuspendLayout();
            // 
            // NUD_Stat
            // 
            NUD_Stat.Location = new System.Drawing.Point(40, 25);
            NUD_Stat.Name = "NUD_Stat";
            NUD_Stat.Size = new System.Drawing.Size(103, 23);
            NUD_Stat.TabIndex = 35;
            NUD_Stat.ValueChanged += ChangeStatVal;
            // 
            // L_Offset
            // 
            L_Offset.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            L_Offset.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            L_Offset.Location = new System.Drawing.Point(3, 48);
            L_Offset.Name = "L_Offset";
            L_Offset.Size = new System.Drawing.Size(140, 20);
            L_Offset.TabIndex = 34;
            L_Offset.Text = "(offset)";
            L_Offset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Value
            // 
            L_Value.AutoSize = true;
            L_Value.Location = new System.Drawing.Point(2, 28);
            L_Value.Name = "L_Value";
            L_Value.Size = new System.Drawing.Size(35, 15);
            L_Value.TabIndex = 32;
            L_Value.Text = "Value";
            // 
            // CB_Stats
            // 
            CB_Stats.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            CB_Stats.DropDownHeight = 256;
            CB_Stats.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Stats.DropDownWidth = 200;
            CB_Stats.FormattingEnabled = true;
            CB_Stats.IntegralHeight = false;
            CB_Stats.Location = new System.Drawing.Point(3, 3);
            CB_Stats.Name = "CB_Stats";
            CB_Stats.Size = new System.Drawing.Size(140, 23);
            CB_Stats.TabIndex = 33;
            CB_Stats.SelectedIndexChanged += ChangeStat;
            // 
            // TrainerStat
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(NUD_Stat);
            Controls.Add(L_Offset);
            Controls.Add(L_Value);
            Controls.Add(CB_Stats);
            Name = "TrainerStat";
            Size = new System.Drawing.Size(146, 72);
            ((System.ComponentModel.ISupportInitialize)NUD_Stat).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.NumericUpDown NUD_Stat;
        private System.Windows.Forms.Label L_Offset;
        private System.Windows.Forms.Label L_Value;
        private System.Windows.Forms.ComboBox CB_Stats;
        private System.Windows.Forms.ToolTip Tip;
    }
}
