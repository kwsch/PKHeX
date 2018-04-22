namespace PKHeX.WinForms.Controls
{
    partial class ContextMenuPKM
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
            this.mnuL = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuLLegality = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLQR = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuL.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuL
            // 
            this.mnuL.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuLLegality,
            this.mnuLQR,
            this.mnuLSave});
            this.mnuL.Name = "mnuL";
            this.mnuL.Size = new System.Drawing.Size(153, 92);
            // 
            // mnuLLegality
            // 
            this.mnuLLegality.Image = global::PKHeX.WinForms.Properties.Resources.export;
            this.mnuLLegality.Name = "mnuLLegality";
            this.mnuLLegality.Size = new System.Drawing.Size(152, 22);
            this.mnuLLegality.Text = "Legality";
            this.mnuLLegality.Click += new System.EventHandler(this.ClickShowLegality);
            // 
            // mnuLQR
            // 
            this.mnuLQR.Image = global::PKHeX.WinForms.Properties.Resources.qr;
            this.mnuLQR.Name = "mnuLQR";
            this.mnuLQR.Size = new System.Drawing.Size(152, 22);
            this.mnuLQR.Text = "QR!";
            this.mnuLQR.Click += new System.EventHandler(this.ClickShowQR);
            // 
            // mnuLSave
            // 
            this.mnuLSave.Image = global::PKHeX.WinForms.Properties.Resources.savePKM;
            this.mnuLSave.Name = "mnuLSave";
            this.mnuLSave.Size = new System.Drawing.Size(152, 22);
            this.mnuLSave.Text = "Save as...";
            this.mnuLSave.Click += new System.EventHandler(this.ClickSaveAs);
            // 
            // ContextMenuPKM
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Name = "ContextMenuPKM";
            this.mnuL.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ContextMenuStrip mnuL;
        private System.Windows.Forms.ToolStripMenuItem mnuLLegality;
        private System.Windows.Forms.ToolStripMenuItem mnuLQR;
        private System.Windows.Forms.ToolStripMenuItem mnuLSave;
    }
}
