namespace PKHeX.WinForms
{
    partial class BoxExporter
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
            PG_Settings = new System.Windows.Forms.PropertyGrid();
            B_Export = new System.Windows.Forms.Button();
            FLP_NamerSelect = new System.Windows.Forms.FlowLayoutPanel();
            L_Namer = new System.Windows.Forms.Label();
            CB_Namer = new System.Windows.Forms.ComboBox();
            FLP_NamerSelect.SuspendLayout();
            SuspendLayout();
            // 
            // PG_Settings
            // 
            PG_Settings.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            PG_Settings.BackColor = System.Drawing.SystemColors.Control;
            PG_Settings.CommandsVisibleIfAvailable = false;
            PG_Settings.HelpVisible = false;
            PG_Settings.Location = new System.Drawing.Point(0, 31);
            PG_Settings.Name = "PG_Settings";
            PG_Settings.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            PG_Settings.Size = new System.Drawing.Size(304, 203);
            PG_Settings.TabIndex = 0;
            PG_Settings.ToolbarVisible = false;
            // 
            // B_Export
            // 
            B_Export.Dock = System.Windows.Forms.DockStyle.Bottom;
            B_Export.Location = new System.Drawing.Point(0, 240);
            B_Export.Name = "B_Export";
            B_Export.Size = new System.Drawing.Size(304, 41);
            B_Export.TabIndex = 1;
            B_Export.Text = "Export";
            B_Export.UseVisualStyleBackColor = true;
            B_Export.Click += B_Export_Click;
            // 
            // FLP_NamerSelect
            // 
            FLP_NamerSelect.Controls.Add(L_Namer);
            FLP_NamerSelect.Controls.Add(CB_Namer);
            FLP_NamerSelect.Dock = System.Windows.Forms.DockStyle.Top;
            FLP_NamerSelect.Location = new System.Drawing.Point(0, 0);
            FLP_NamerSelect.Name = "FLP_NamerSelect";
            FLP_NamerSelect.Size = new System.Drawing.Size(304, 27);
            FLP_NamerSelect.TabIndex = 2;
            // 
            // L_Namer
            // 
            L_Namer.AutoSize = true;
            L_Namer.Location = new System.Drawing.Point(8, 8);
            L_Namer.Margin = new System.Windows.Forms.Padding(8, 8, 0, 8);
            L_Namer.Name = "L_Namer";
            L_Namer.Size = new System.Drawing.Size(51, 17);
            L_Namer.TabIndex = 0;
            L_Namer.Text = "Namer:";
            // 
            // CB_Namer
            // 
            CB_Namer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Namer.FormattingEnabled = true;
            CB_Namer.Location = new System.Drawing.Point(59, 4);
            CB_Namer.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            CB_Namer.Name = "CB_Namer";
            CB_Namer.Size = new System.Drawing.Size(155, 25);
            CB_Namer.TabIndex = 1;
            // 
            // BoxExporter
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(304, 281);
            Controls.Add(FLP_NamerSelect);
            Controls.Add(B_Export);
            Controls.Add(PG_Settings);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "BoxExporter";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Box Export";
            FormClosing += BoxExporter_FormClosing;
            FLP_NamerSelect.ResumeLayout(false);
            FLP_NamerSelect.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.PropertyGrid PG_Settings;
        private System.Windows.Forms.Button B_Export;
        private System.Windows.Forms.FlowLayoutPanel FLP_NamerSelect;
        private System.Windows.Forms.Label L_Namer;
        private System.Windows.Forms.ComboBox CB_Namer;
    }
}
