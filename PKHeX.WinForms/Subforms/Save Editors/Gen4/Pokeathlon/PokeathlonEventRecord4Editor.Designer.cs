namespace PKHeX.WinForms
{
    partial class PokeathlonEventRecord4Editor
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
            L_Record = new System.Windows.Forms.Label();
            NUD_Record = new System.Windows.Forms.NumericUpDown();
            UC_Entry0 = new PokeathlonSpeciesForm4Editor();
            UC_Entry1 = new PokeathlonSpeciesForm4Editor();
            UC_Entry2 = new PokeathlonSpeciesForm4Editor();
            TLP_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Record).BeginInit();
            SuspendLayout();
            // 
            // TLP_Main
            // 
            TLP_Main.AutoSize = true;
            TLP_Main.ColumnCount = 2;
            TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Main.Controls.Add(L_Record, 0, 0);
            TLP_Main.Controls.Add(NUD_Record, 1, 0);
            TLP_Main.Controls.Add(UC_Entry0, 0, 1);
            TLP_Main.Controls.Add(UC_Entry1, 0, 2);
            TLP_Main.Controls.Add(UC_Entry2, 0, 3);
            TLP_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Main.Location = new System.Drawing.Point(0, 0);
            TLP_Main.Margin = new System.Windows.Forms.Padding(0);
            TLP_Main.Name = "TLP_Main";
            TLP_Main.RowCount = 4;
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.Size = new System.Drawing.Size(512, 145);
            TLP_Main.TabIndex = 0;
            // 
            // L_Record
            // 
            L_Record.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Record.AutoSize = true;
            L_Record.Location = new System.Drawing.Point(3, 4);
            L_Record.Name = "L_Record";
            L_Record.Size = new System.Drawing.Size(48, 17);
            L_Record.TabIndex = 0;
            L_Record.Text = "Record:";
            // 
            // NUD_Record
            // 
            NUD_Record.Location = new System.Drawing.Point(57, 0);
            NUD_Record.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            NUD_Record.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NUD_Record.Name = "NUD_Record";
            NUD_Record.Size = new System.Drawing.Size(120, 25);
            NUD_Record.TabIndex = 1;
            // 
            // UC_Entry0
            // 
            TLP_Main.SetColumnSpan(UC_Entry0, 2);
            UC_Entry0.Anchor = System.Windows.Forms.AnchorStyles.Left;
            UC_Entry0.Location = new System.Drawing.Point(0, 28);
            UC_Entry0.Margin = new System.Windows.Forms.Padding(0);
            UC_Entry0.Name = "UC_Entry0";
            UC_Entry0.Size = new System.Drawing.Size(249, 40);
            UC_Entry0.TabIndex = 2;
            // 
            // UC_Entry1
            // 
            TLP_Main.SetColumnSpan(UC_Entry1, 2);
            UC_Entry1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            UC_Entry1.Location = new System.Drawing.Point(0, 68);
            UC_Entry1.Margin = new System.Windows.Forms.Padding(0);
            UC_Entry1.Name = "UC_Entry1";
            UC_Entry1.Size = new System.Drawing.Size(249, 40);
            UC_Entry1.TabIndex = 3;
            // 
            // UC_Entry2
            // 
            TLP_Main.SetColumnSpan(UC_Entry2, 2);
            UC_Entry2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            UC_Entry2.Location = new System.Drawing.Point(0, 108);
            UC_Entry2.Margin = new System.Windows.Forms.Padding(0);
            UC_Entry2.Name = "UC_Entry2";
            UC_Entry2.Size = new System.Drawing.Size(249, 40);
            UC_Entry2.TabIndex = 4;
            // 
            // PokeathlonEventRecord4Editor
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(TLP_Main);
            Name = "PokeathlonEventRecord4Editor";
            Size = new System.Drawing.Size(249, 145);
            TLP_Main.ResumeLayout(false);
            TLP_Main.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Record).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TLP_Main;
        private System.Windows.Forms.Label L_Record;
        private System.Windows.Forms.NumericUpDown NUD_Record;
        private PokeathlonSpeciesForm4Editor UC_Entry0;
        private PokeathlonSpeciesForm4Editor UC_Entry1;
        private PokeathlonSpeciesForm4Editor UC_Entry2;
    }
}
