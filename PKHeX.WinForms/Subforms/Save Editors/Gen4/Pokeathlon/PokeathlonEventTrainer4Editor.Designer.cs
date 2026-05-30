namespace PKHeX.WinForms
{
    partial class PokeathlonEventTrainer4Editor
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
            L_OT = new System.Windows.Forms.Label();
            TB_OT = new System.Windows.Forms.TextBox();
            L_TID16 = new System.Windows.Forms.Label();
            TB_TID16 = new System.Windows.Forms.TextBox();
            L_SID16 = new System.Windows.Forms.Label();
            TB_SID16 = new System.Windows.Forms.TextBox();
            L_Language = new System.Windows.Forms.Label();
            CB_Language = new System.Windows.Forms.ComboBox();
            TLP_Main.SuspendLayout();
            SuspendLayout();
            // 
            // TLP_Main
            // 
            TLP_Main.AutoSize = true;
            TLP_Main.ColumnCount = 2;
            TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Main.Controls.Add(L_OT, 0, 0);
            TLP_Main.Controls.Add(TB_OT, 1, 0);
            TLP_Main.Controls.Add(L_TID16, 0, 1);
            TLP_Main.Controls.Add(TB_TID16, 1, 1);
            TLP_Main.Controls.Add(L_SID16, 0, 2);
            TLP_Main.Controls.Add(TB_SID16, 1, 2);
            TLP_Main.Controls.Add(L_Language, 0, 3);
            TLP_Main.Controls.Add(CB_Language, 1, 3);
            TLP_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Main.Location = new System.Drawing.Point(0, 0);
            TLP_Main.Margin = new System.Windows.Forms.Padding(0);
            TLP_Main.Name = "TLP_Main";
            TLP_Main.RowCount = 4;
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.Size = new System.Drawing.Size(225, 118);
            TLP_Main.TabIndex = 0;
            // 
            // L_OT
            // 
            L_OT.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_OT.AutoSize = true;
            L_OT.Location = new System.Drawing.Point(41, 5);
            L_OT.Margin = new System.Windows.Forms.Padding(0);
            L_OT.Name = "L_OT";
            L_OT.Size = new System.Drawing.Size(27, 17);
            L_OT.TabIndex = 0;
            L_OT.Text = "OT:";
            // 
            // TB_OT
            // 
            TB_OT.Anchor = System.Windows.Forms.AnchorStyles.Left;
            TB_OT.Location = new System.Drawing.Point(71, 0);
            TB_OT.Margin = new System.Windows.Forms.Padding(3, 0, 0, 3);
            TB_OT.Name = "TB_OT";
            TB_OT.Size = new System.Drawing.Size(136, 25);
            TB_OT.TabIndex = 1;
            // 
            // L_TID16
            // 
            L_TID16.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_TID16.AutoSize = true;
            L_TID16.Location = new System.Drawing.Point(38, 33);
            L_TID16.Margin = new System.Windows.Forms.Padding(0);
            L_TID16.Name = "L_TID16";
            L_TID16.Size = new System.Drawing.Size(30, 17);
            L_TID16.TabIndex = 2;
            L_TID16.Text = "TID:";
            // 
            // TB_TID16
            // 
            TB_TID16.Location = new System.Drawing.Point(71, 28);
            TB_TID16.Margin = new System.Windows.Forms.Padding(3, 0, 0, 3);
            TB_TID16.Name = "TB_TID16";
            TB_TID16.Size = new System.Drawing.Size(90, 25);
            TB_TID16.TabIndex = 3;
            // 
            // L_SID16
            // 
            L_SID16.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_SID16.AutoSize = true;
            L_SID16.Location = new System.Drawing.Point(38, 61);
            L_SID16.Margin = new System.Windows.Forms.Padding(0);
            L_SID16.Name = "L_SID16";
            L_SID16.Size = new System.Drawing.Size(30, 17);
            L_SID16.TabIndex = 4;
            L_SID16.Text = "SID:";
            // 
            // TB_SID16
            // 
            TB_SID16.Location = new System.Drawing.Point(71, 56);
            TB_SID16.Margin = new System.Windows.Forms.Padding(3, 0, 0, 3);
            TB_SID16.Name = "TB_SID16";
            TB_SID16.Size = new System.Drawing.Size(90, 25);
            TB_SID16.TabIndex = 5;
            // 
            // L_Language
            // 
            L_Language.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            L_Language.AutoSize = true;
            L_Language.Location = new System.Drawing.Point(0, 88);
            L_Language.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            L_Language.Name = "L_Language";
            L_Language.Size = new System.Drawing.Size(68, 17);
            L_Language.TabIndex = 6;
            L_Language.Text = "Language:";
            // 
            // CB_Language
            // 
            CB_Language.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Language.FormattingEnabled = true;
            CB_Language.Location = new System.Drawing.Point(71, 84);
            CB_Language.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            CB_Language.Name = "CB_Language";
            CB_Language.Size = new System.Drawing.Size(136, 25);
            CB_Language.TabIndex = 7;
            // 
            // PokeathlonEventTrainer4Editor
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            AutoSize = true;
            Controls.Add(TLP_Main);
            Name = "PokeathlonEventTrainer4Editor";
            Size = new System.Drawing.Size(225, 118);
            TLP_Main.ResumeLayout(false);
            TLP_Main.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TLP_Main;
        private System.Windows.Forms.Label L_OT;
        private System.Windows.Forms.TextBox TB_OT;
        private System.Windows.Forms.Label L_TID16;
        private System.Windows.Forms.TextBox TB_TID16;
        private System.Windows.Forms.Label L_SID16;
        private System.Windows.Forms.TextBox TB_SID16;
        private System.Windows.Forms.Label L_Language;
        private System.Windows.Forms.ComboBox CB_Language;
    }
}
