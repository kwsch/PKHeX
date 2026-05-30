using System.Windows.Forms;

namespace PKHeX.WinForms
{
    partial class JoinAvenueAssistantSpecificEditor
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
            TLP_Main = new TableLayoutPanel();
            L_Position0 = new Label();
            NUD_Position0 = new NumericUpDown();
            L_Position1 = new Label();
            NUD_Position1 = new NumericUpDown();
            L_Position2 = new Label();
            NUD_Position2 = new NumericUpDown();
            L_UnusedPosition = new Label();
            NUD_PositionUnused = new NumericUpDown();
            L_InteractedToday = new Label();
            CHK_InteractedToday = new CheckBox();
            TLP_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Position0).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Position1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Position2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_PositionUnused).BeginInit();
            SuspendLayout();
            // 
            // TLP_Main
            // 
            TLP_Main.AutoScroll = true;
            TLP_Main.ColumnCount = 2;
            TLP_Main.ColumnStyles.Add(new ColumnStyle());
            TLP_Main.ColumnStyles.Add(new ColumnStyle());
            TLP_Main.Controls.Add(L_Position0, 0, 0);
            TLP_Main.Controls.Add(NUD_Position0, 1, 0);
            TLP_Main.Controls.Add(L_Position1, 0, 1);
            TLP_Main.Controls.Add(NUD_Position1, 1, 1);
            TLP_Main.Controls.Add(L_Position2, 0, 2);
            TLP_Main.Controls.Add(NUD_Position2, 1, 2);
            TLP_Main.Controls.Add(L_UnusedPosition, 0, 3);
            TLP_Main.Controls.Add(NUD_PositionUnused, 1, 3);
            TLP_Main.Controls.Add(L_InteractedToday, 0, 4);
            TLP_Main.Controls.Add(CHK_InteractedToday, 1, 4);
            TLP_Main.Dock = DockStyle.Fill;
            TLP_Main.Location = new System.Drawing.Point(0, 0);
            TLP_Main.Margin = new Padding(0);
            TLP_Main.Name = "TLP_Main";
            TLP_Main.RowStyles.Add(new RowStyle());
            TLP_Main.RowStyles.Add(new RowStyle());
            TLP_Main.RowStyles.Add(new RowStyle());
            TLP_Main.RowStyles.Add(new RowStyle());
            TLP_Main.RowStyles.Add(new RowStyle());
            TLP_Main.Size = new System.Drawing.Size(480, 147);
            TLP_Main.TabIndex = 0;
            // 
            // L_Position0
            // 
            L_Position0.Anchor = AnchorStyles.Right;
            L_Position0.AutoSize = true;
            L_Position0.Location = new System.Drawing.Point(47, 4);
            L_Position0.Name = "L_Position0";
            L_Position0.Size = new System.Drawing.Size(64, 17);
            L_Position0.TabIndex = 0;
            L_Position0.Text = "Position0:";
            // 
            // NUD_Position0
            // 
            NUD_Position0.Location = new System.Drawing.Point(114, 0);
            NUD_Position0.Margin = new Padding(0, 0, 0, 1);
            NUD_Position0.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_Position0.Name = "NUD_Position0";
            NUD_Position0.Size = new System.Drawing.Size(48, 25);
            NUD_Position0.TabIndex = 1;
            // 
            // L_Position1
            // 
            L_Position1.Anchor = AnchorStyles.Right;
            L_Position1.AutoSize = true;
            L_Position1.Location = new System.Drawing.Point(47, 30);
            L_Position1.Name = "L_Position1";
            L_Position1.Size = new System.Drawing.Size(64, 17);
            L_Position1.TabIndex = 2;
            L_Position1.Text = "Position1:";
            // 
            // NUD_Position1
            // 
            NUD_Position1.Location = new System.Drawing.Point(114, 26);
            NUD_Position1.Margin = new Padding(0, 0, 0, 1);
            NUD_Position1.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_Position1.Name = "NUD_Position1";
            NUD_Position1.Size = new System.Drawing.Size(48, 25);
            NUD_Position1.TabIndex = 3;
            // 
            // L_Position2
            // 
            L_Position2.Anchor = AnchorStyles.Right;
            L_Position2.AutoSize = true;
            L_Position2.Location = new System.Drawing.Point(47, 56);
            L_Position2.Name = "L_Position2";
            L_Position2.Size = new System.Drawing.Size(64, 17);
            L_Position2.TabIndex = 4;
            L_Position2.Text = "Position2:";
            // 
            // NUD_Position2
            // 
            NUD_Position2.Location = new System.Drawing.Point(114, 52);
            NUD_Position2.Margin = new Padding(0, 0, 0, 1);
            NUD_Position2.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_Position2.Name = "NUD_Position2";
            NUD_Position2.Size = new System.Drawing.Size(48, 25);
            NUD_Position2.TabIndex = 5;
            // 
            // L_UnusedPosition
            // 
            L_UnusedPosition.Anchor = AnchorStyles.Right;
            L_UnusedPosition.AutoSize = true;
            L_UnusedPosition.Location = new System.Drawing.Point(56, 82);
            L_UnusedPosition.Name = "L_UnusedPosition";
            L_UnusedPosition.Size = new System.Drawing.Size(55, 17);
            L_UnusedPosition.TabIndex = 6;
            L_UnusedPosition.Text = "Unused:";
            // 
            // NUD_PositionUnused
            // 
            NUD_PositionUnused.Location = new System.Drawing.Point(114, 78);
            NUD_PositionUnused.Margin = new Padding(0, 0, 0, 1);
            NUD_PositionUnused.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_PositionUnused.Name = "NUD_PositionUnused";
            NUD_PositionUnused.Size = new System.Drawing.Size(48, 25);
            NUD_PositionUnused.TabIndex = 7;
            // 
            // L_InteractedToday
            // 
            L_InteractedToday.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            L_InteractedToday.AutoSize = true;
            L_InteractedToday.Location = new System.Drawing.Point(3, 104);
            L_InteractedToday.Name = "L_InteractedToday";
            L_InteractedToday.Size = new System.Drawing.Size(108, 17);
            L_InteractedToday.TabIndex = 8;
            L_InteractedToday.Text = "Interacted Today:";
            // 
            // CHK_InteractedToday
            // 
            CHK_InteractedToday.AutoSize = true;
            CHK_InteractedToday.Location = new System.Drawing.Point(117, 107);
            CHK_InteractedToday.Name = "CHK_InteractedToday";
            CHK_InteractedToday.Size = new System.Drawing.Size(15, 14);
            CHK_InteractedToday.TabIndex = 9;
            // 
            // JoinAvenueAssistantSpecificEditor
            // 
            AutoScaleMode = AutoScaleMode.Inherit;
            Controls.Add(TLP_Main);
            Name = "JoinAvenueAssistantSpecificEditor";
            Size = new System.Drawing.Size(480, 147);
            TLP_Main.ResumeLayout(false);
            TLP_Main.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Position0).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Position1).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Position2).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_PositionUnused).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TLP_Main;
        private System.Windows.Forms.Label L_Position0;
        private System.Windows.Forms.NumericUpDown NUD_Position0;
        private System.Windows.Forms.Label L_Position1;
        private System.Windows.Forms.NumericUpDown NUD_Position1;
        private System.Windows.Forms.Label L_Position2;
        private System.Windows.Forms.NumericUpDown NUD_Position2;
        private System.Windows.Forms.Label L_UnusedPosition;
        private System.Windows.Forms.NumericUpDown NUD_PositionUnused;
        private System.Windows.Forms.Label L_InteractedToday;
        private System.Windows.Forms.CheckBox CHK_InteractedToday;
    }
}
