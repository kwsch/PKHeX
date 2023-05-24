namespace PKHeX.WinForms
{
    partial class ReportGrid
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            dgData = new PKHeX.WinForms.Controls.DoubleBufferedDataGridView();
            ((System.ComponentModel.ISupportInitialize)(dgData)).BeginInit();
            SuspendLayout();
            // 
            // dgData
            // 
            dgData.AllowUserToAddRows = false;
            dgData.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dgData.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgData.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            dgData.Dock = System.Windows.Forms.DockStyle.Fill;
            dgData.Location = new System.Drawing.Point(0, 0);
            dgData.Name = "dgData";
            dgData.RowHeadersVisible = false;
            dgData.Size = new System.Drawing.Size(812, 461);
            dgData.TabIndex = 0;
            dgData.Sorted += new System.EventHandler(Data_Sorted);
            // 
            // ReportGrid
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(812, 461);
            Controls.Add(dgData);
            Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            Name = "ReportGrid";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Box Data Report";
            FormClosing += new System.Windows.Forms.FormClosingEventHandler(PromptSaveCSV);
            ((System.ComponentModel.ISupportInitialize)(dgData)).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private PKHeX.WinForms.Controls.DoubleBufferedDataGridView dgData;
    }
}
