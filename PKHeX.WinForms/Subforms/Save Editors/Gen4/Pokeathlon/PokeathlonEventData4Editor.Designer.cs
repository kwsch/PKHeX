namespace PKHeX.WinForms
{
    partial class PokeathlonEventData4Editor
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            TLP_Main = new System.Windows.Forms.TableLayoutPanel();
            L_Attempts = new System.Windows.Forms.Label();
            NUD_Attempts = new System.Windows.Forms.NumericUpDown();
            UC_Record0 = new PokeathlonEventRecord4Editor();
            UC_Record1 = new PokeathlonEventRecord4Editor();
            UC_Record2 = new PokeathlonEventRecord4Editor();
            UC_Record3 = new PokeathlonEventRecord4Editor();
            UC_Record4 = new PokeathlonEventRecord4Editor();
            TLP_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Attempts).BeginInit();
            SuspendLayout();
            // 
            // TLP_Main
            // 
            TLP_Main.AutoScroll = true;
            TLP_Main.AutoSize = true;
            TLP_Main.ColumnCount = 2;
            TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_Main.Controls.Add(L_Attempts, 0, 0);
            TLP_Main.Controls.Add(NUD_Attempts, 1, 0);
            TLP_Main.Controls.Add(UC_Record0, 0, 1);
            TLP_Main.Controls.Add(UC_Record1, 0, 2);
            TLP_Main.Controls.Add(UC_Record2, 0, 3);
            TLP_Main.Controls.Add(UC_Record3, 0, 4);
            TLP_Main.Controls.Add(UC_Record4, 0, 5);
            TLP_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Main.Location = new System.Drawing.Point(0, 0);
            TLP_Main.Margin = new System.Windows.Forms.Padding(0);
            TLP_Main.Name = "TLP_Main";
            TLP_Main.RowCount = 6;
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.Size = new System.Drawing.Size(258, 841);
            TLP_Main.TabIndex = 0;
            // 
            // L_Attempts
            // 
            L_Attempts.Anchor = System.Windows.Forms.AnchorStyles.Left;
            L_Attempts.AutoSize = true;
            L_Attempts.Location = new System.Drawing.Point(3, 4);
            L_Attempts.Name = "L_Attempts";
            L_Attempts.Size = new System.Drawing.Size(63, 17);
            L_Attempts.TabIndex = 0;
            L_Attempts.Text = "Attempts:";
            // 
            // NUD_Attempts
            // 
            NUD_Attempts.Location = new System.Drawing.Point(69, 0);
            NUD_Attempts.Margin = new System.Windows.Forms.Padding(0);
            NUD_Attempts.Maximum = new decimal(new int[] { 9999999, 0, 0, 0 });
            NUD_Attempts.Name = "NUD_Attempts";
            NUD_Attempts.Size = new System.Drawing.Size(120, 25);
            NUD_Attempts.TabIndex = 1;
            // 
            // UC_Record0
            // 
            UC_Record0.AutoSize = true;
            TLP_Main.SetColumnSpan(UC_Record0, 2);
            UC_Record0.Dock = System.Windows.Forms.DockStyle.Fill;
            UC_Record0.Location = new System.Drawing.Point(0, 37);
            UC_Record0.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            UC_Record0.Name = "UC_Record0";
            UC_Record0.Size = new System.Drawing.Size(258, 148);
            UC_Record0.TabIndex = 2;
            // 
            // UC_Record1
            // 
            UC_Record1.AutoSize = true;
            TLP_Main.SetColumnSpan(UC_Record1, 2);
            UC_Record1.Dock = System.Windows.Forms.DockStyle.Fill;
            UC_Record1.Location = new System.Drawing.Point(0, 197);
            UC_Record1.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            UC_Record1.Name = "UC_Record1";
            UC_Record1.Size = new System.Drawing.Size(258, 148);
            UC_Record1.TabIndex = 3;
            // 
            // UC_Record2
            // 
            UC_Record2.AutoSize = true;
            TLP_Main.SetColumnSpan(UC_Record2, 2);
            UC_Record2.Dock = System.Windows.Forms.DockStyle.Fill;
            UC_Record2.Location = new System.Drawing.Point(0, 357);
            UC_Record2.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            UC_Record2.Name = "UC_Record2";
            UC_Record2.Size = new System.Drawing.Size(258, 148);
            UC_Record2.TabIndex = 4;
            // 
            // UC_Record3
            // 
            UC_Record3.AutoSize = true;
            TLP_Main.SetColumnSpan(UC_Record3, 2);
            UC_Record3.Dock = System.Windows.Forms.DockStyle.Fill;
            UC_Record3.Location = new System.Drawing.Point(0, 517);
            UC_Record3.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            UC_Record3.Name = "UC_Record3";
            UC_Record3.Size = new System.Drawing.Size(258, 148);
            UC_Record3.TabIndex = 5;
            // 
            // UC_Record4
            // 
            UC_Record4.AutoSize = true;
            TLP_Main.SetColumnSpan(UC_Record4, 2);
            UC_Record4.Location = new System.Drawing.Point(0, 677);
            UC_Record4.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            UC_Record4.Name = "UC_Record4";
            UC_Record4.Size = new System.Drawing.Size(249, 148);
            UC_Record4.TabIndex = 6;
            // 
            // PokeathlonEventData4Editor
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            AutoSize = true;
            Controls.Add(TLP_Main);
            Margin = new System.Windows.Forms.Padding(0);
            Name = "PokeathlonEventData4Editor";
            Size = new System.Drawing.Size(258, 841);
            TLP_Main.ResumeLayout(false);
            TLP_Main.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Attempts).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TLP_Main;
        private System.Windows.Forms.Label L_Attempts;
        private System.Windows.Forms.NumericUpDown NUD_Attempts;
        private PokeathlonEventRecord4Editor UC_Record0;
        private PokeathlonEventRecord4Editor UC_Record1;
        private PokeathlonEventRecord4Editor UC_Record2;
        private PokeathlonEventRecord4Editor UC_Record3;
        private PokeathlonEventRecord4Editor UC_Record4;
    }
}
