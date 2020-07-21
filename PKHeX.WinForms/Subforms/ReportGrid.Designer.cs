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
            this.dgData = new System.Windows.Forms.DataGridView();
            this.BoxBar = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.dgData)).BeginInit();
            this.SuspendLayout();
            // 
            // dgData
            // 
            this.dgData.AllowUserToAddRows = false;
            this.dgData.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            this.dgData.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgData.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.dgData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgData.Location = new System.Drawing.Point(0, 0);
            this.dgData.Name = "dgData";
            this.dgData.RowHeadersVisible = false;
            this.dgData.Size = new System.Drawing.Size(812, 461);
            this.dgData.TabIndex = 0;
            this.dgData.Sorted += new System.EventHandler(this.Data_Sorted);
            // 
            // BoxBar
            // 
            this.BoxBar.Location = new System.Drawing.Point(13, 13);
            this.BoxBar.Name = "BoxBar";
            this.BoxBar.Size = new System.Drawing.Size(787, 23);
            this.BoxBar.TabIndex = 1;
            // 
            // frmReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(812, 461);
            this.Controls.Add(this.BoxBar);
            this.Controls.Add(this.dgData);
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.Name = "ReportGrid";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Box Data Report";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PromptSaveCSV);
            ((System.ComponentModel.ISupportInitialize)(this.dgData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgData;
        private System.Windows.Forms.ProgressBar BoxBar;
    }
}