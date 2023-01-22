namespace PKHeX.WinForms.Controls
{
    partial class ContestStat
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
            this.TB_Sheen = new System.Windows.Forms.MaskedTextBox();
            this.TB_Tough = new System.Windows.Forms.MaskedTextBox();
            this.TB_Smart = new System.Windows.Forms.MaskedTextBox();
            this.TB_Cute = new System.Windows.Forms.MaskedTextBox();
            this.TB_Beauty = new System.Windows.Forms.MaskedTextBox();
            this.TB_Cool = new System.Windows.Forms.MaskedTextBox();
            this.Label_Sheen = new System.Windows.Forms.Label();
            this.Label_Tough = new System.Windows.Forms.Label();
            this.Label_Cute = new System.Windows.Forms.Label();
            this.Label_Beauty = new System.Windows.Forms.Label();
            this.Label_Cool = new System.Windows.Forms.Label();
            this.Label_ContestStats = new System.Windows.Forms.Label();
            this.Label_Smart = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.FLP_SmartClever = new System.Windows.Forms.FlowLayoutPanel();
            this.Label_Clever = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.FLP_SmartClever.SuspendLayout();
            this.SuspendLayout();
            // 
            // TB_Sheen
            // 
            this.TB_Sheen.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TB_Sheen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_Sheen.Location = new System.Drawing.Point(248, 16);
            this.TB_Sheen.Margin = new System.Windows.Forms.Padding(0);
            this.TB_Sheen.Mask = "000";
            this.TB_Sheen.MinimumSize = new System.Drawing.Size(32, 23);
            this.TB_Sheen.Name = "TB_Sheen";
            this.TB_Sheen.Size = new System.Drawing.Size(32, 23);
            this.TB_Sheen.TabIndex = 58;
            this.TB_Sheen.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_Sheen.Click += new System.EventHandler(this.ClickTextBox);
            this.TB_Sheen.Validated += new System.EventHandler(this.Update255_MTB);
            // 
            // TB_Tough
            // 
            this.TB_Tough.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TB_Tough.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_Tough.Location = new System.Drawing.Point(200, 16);
            this.TB_Tough.Margin = new System.Windows.Forms.Padding(0);
            this.TB_Tough.Mask = "000";
            this.TB_Tough.MinimumSize = new System.Drawing.Size(32, 23);
            this.TB_Tough.Name = "TB_Tough";
            this.TB_Tough.Size = new System.Drawing.Size(32, 23);
            this.TB_Tough.TabIndex = 57;
            this.TB_Tough.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_Tough.Click += new System.EventHandler(this.ClickTextBox);
            this.TB_Tough.Validated += new System.EventHandler(this.Update255_MTB);
            // 
            // TB_Smart
            // 
            this.TB_Smart.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TB_Smart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_Smart.Location = new System.Drawing.Point(152, 16);
            this.TB_Smart.Margin = new System.Windows.Forms.Padding(0);
            this.TB_Smart.Mask = "000";
            this.TB_Smart.MinimumSize = new System.Drawing.Size(32, 23);
            this.TB_Smart.Name = "TB_Smart";
            this.TB_Smart.Size = new System.Drawing.Size(32, 23);
            this.TB_Smart.TabIndex = 56;
            this.TB_Smart.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_Smart.Click += new System.EventHandler(this.ClickTextBox);
            this.TB_Smart.Validated += new System.EventHandler(this.Update255_MTB);
            // 
            // TB_Cute
            // 
            this.TB_Cute.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TB_Cute.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_Cute.Location = new System.Drawing.Point(104, 16);
            this.TB_Cute.Margin = new System.Windows.Forms.Padding(0);
            this.TB_Cute.Mask = "000";
            this.TB_Cute.MinimumSize = new System.Drawing.Size(32, 23);
            this.TB_Cute.Name = "TB_Cute";
            this.TB_Cute.Size = new System.Drawing.Size(32, 23);
            this.TB_Cute.TabIndex = 55;
            this.TB_Cute.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_Cute.Click += new System.EventHandler(this.ClickTextBox);
            this.TB_Cute.Validated += new System.EventHandler(this.Update255_MTB);
            // 
            // TB_Beauty
            // 
            this.TB_Beauty.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TB_Beauty.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_Beauty.Location = new System.Drawing.Point(56, 16);
            this.TB_Beauty.Margin = new System.Windows.Forms.Padding(0);
            this.TB_Beauty.Mask = "000";
            this.TB_Beauty.MinimumSize = new System.Drawing.Size(32, 23);
            this.TB_Beauty.Name = "TB_Beauty";
            this.TB_Beauty.Size = new System.Drawing.Size(32, 23);
            this.TB_Beauty.TabIndex = 54;
            this.TB_Beauty.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_Beauty.Click += new System.EventHandler(this.ClickTextBox);
            this.TB_Beauty.Validated += new System.EventHandler(this.Update255_MTB);
            // 
            // TB_Cool
            // 
            this.TB_Cool.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TB_Cool.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_Cool.Location = new System.Drawing.Point(8, 16);
            this.TB_Cool.Margin = new System.Windows.Forms.Padding(0);
            this.TB_Cool.Mask = "000";
            this.TB_Cool.MinimumSize = new System.Drawing.Size(32, 23);
            this.TB_Cool.Name = "TB_Cool";
            this.TB_Cool.Size = new System.Drawing.Size(32, 23);
            this.TB_Cool.TabIndex = 53;
            this.TB_Cool.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TB_Cool.Click += new System.EventHandler(this.ClickTextBox);
            this.TB_Cool.Validated += new System.EventHandler(this.Update255_MTB);
            // 
            // Label_Sheen
            // 
            this.Label_Sheen.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Label_Sheen.Location = new System.Drawing.Point(240, 0);
            this.Label_Sheen.Margin = new System.Windows.Forms.Padding(0);
            this.Label_Sheen.Name = "Label_Sheen";
            this.Label_Sheen.Size = new System.Drawing.Size(48, 16);
            this.Label_Sheen.TabIndex = 65;
            this.Label_Sheen.Text = "Sheen";
            this.Label_Sheen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label_Tough
            // 
            this.Label_Tough.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Label_Tough.Location = new System.Drawing.Point(192, 0);
            this.Label_Tough.Margin = new System.Windows.Forms.Padding(0);
            this.Label_Tough.Name = "Label_Tough";
            this.Label_Tough.Size = new System.Drawing.Size(48, 16);
            this.Label_Tough.TabIndex = 64;
            this.Label_Tough.Text = "Tough";
            this.Label_Tough.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label_Cute
            // 
            this.Label_Cute.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Label_Cute.Location = new System.Drawing.Point(96, 0);
            this.Label_Cute.Margin = new System.Windows.Forms.Padding(0);
            this.Label_Cute.Name = "Label_Cute";
            this.Label_Cute.Size = new System.Drawing.Size(48, 16);
            this.Label_Cute.TabIndex = 62;
            this.Label_Cute.Text = "Cute";
            this.Label_Cute.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label_Beauty
            // 
            this.Label_Beauty.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Label_Beauty.Location = new System.Drawing.Point(48, 0);
            this.Label_Beauty.Margin = new System.Windows.Forms.Padding(0);
            this.Label_Beauty.Name = "Label_Beauty";
            this.Label_Beauty.Size = new System.Drawing.Size(48, 16);
            this.Label_Beauty.TabIndex = 61;
            this.Label_Beauty.Text = "Beauty";
            this.Label_Beauty.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label_Cool
            // 
            this.Label_Cool.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Label_Cool.Location = new System.Drawing.Point(0, 0);
            this.Label_Cool.Margin = new System.Windows.Forms.Padding(0);
            this.Label_Cool.Name = "Label_Cool";
            this.Label_Cool.Size = new System.Drawing.Size(48, 16);
            this.Label_Cool.TabIndex = 60;
            this.Label_Cool.Text = "Cool";
            this.Label_Cool.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label_ContestStats
            // 
            this.Label_ContestStats.Location = new System.Drawing.Point(74, 1);
            this.Label_ContestStats.Name = "Label_ContestStats";
            this.Label_ContestStats.Size = new System.Drawing.Size(140, 13);
            this.Label_ContestStats.TabIndex = 59;
            this.Label_ContestStats.Text = "Contest Stats";
            this.Label_ContestStats.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label_Smart
            // 
            this.Label_Smart.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Label_Smart.Location = new System.Drawing.Point(0, 0);
            this.Label_Smart.Margin = new System.Windows.Forms.Padding(0);
            this.Label_Smart.Name = "Label_Smart";
            this.Label_Smart.Size = new System.Drawing.Size(48, 16);
            this.Label_Smart.TabIndex = 66;
            this.Label_Smart.Text = "Smart";
            this.Label_Smart.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.tableLayoutPanel1.Controls.Add(this.TB_Cool, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.Label_Tough, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.Label_Sheen, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.TB_Sheen, 5, 1);
            this.tableLayoutPanel1.Controls.Add(this.TB_Beauty, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.TB_Tough, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.Label_Cute, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.TB_Cute, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.Label_Beauty, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.TB_Smart, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.Label_Cool, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.FLP_SmartClever, 3, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 16);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(288, 40);
            this.tableLayoutPanel1.TabIndex = 67;
            // 
            // FLP_SmartClever
            // 
            this.FLP_SmartClever.Controls.Add(this.Label_Smart);
            this.FLP_SmartClever.Controls.Add(this.Label_Clever);
            this.FLP_SmartClever.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FLP_SmartClever.Location = new System.Drawing.Point(144, 0);
            this.FLP_SmartClever.Margin = new System.Windows.Forms.Padding(0);
            this.FLP_SmartClever.Name = "FLP_SmartClever";
            this.FLP_SmartClever.Size = new System.Drawing.Size(48, 16);
            this.FLP_SmartClever.TabIndex = 66;
            // 
            // Label_Clever
            // 
            this.Label_Clever.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Label_Clever.Location = new System.Drawing.Point(0, 16);
            this.Label_Clever.Margin = new System.Windows.Forms.Padding(0);
            this.Label_Clever.Name = "Label_Clever";
            this.Label_Clever.Size = new System.Drawing.Size(48, 16);
            this.Label_Clever.TabIndex = 67;
            this.Label_Clever.Text = "Clever";
            this.Label_Clever.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ContestStat
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.Label_ContestStats);
            this.Name = "ContestStat";
            this.Size = new System.Drawing.Size(288, 56);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.FLP_SmartClever.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MaskedTextBox TB_Sheen;
        private System.Windows.Forms.MaskedTextBox TB_Tough;
        private System.Windows.Forms.MaskedTextBox TB_Smart;
        private System.Windows.Forms.MaskedTextBox TB_Cute;
        private System.Windows.Forms.MaskedTextBox TB_Beauty;
        private System.Windows.Forms.MaskedTextBox TB_Cool;
        private System.Windows.Forms.Label Label_Sheen;
        private System.Windows.Forms.Label Label_Tough;
        private System.Windows.Forms.Label Label_Cute;
        private System.Windows.Forms.Label Label_Beauty;
        private System.Windows.Forms.Label Label_Cool;
        private System.Windows.Forms.Label Label_ContestStats;
        private System.Windows.Forms.Label Label_Smart;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label Label_Clever;
        private System.Windows.Forms.FlowLayoutPanel FLP_SmartClever;
    }
}
