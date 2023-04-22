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
            components = new System.ComponentModel.Container();
            mnuL = new System.Windows.Forms.ContextMenuStrip(components);
            mnuLLegality = new System.Windows.Forms.ToolStripMenuItem();
            mnuLQR = new System.Windows.Forms.ToolStripMenuItem();
            mnuLSave = new System.Windows.Forms.ToolStripMenuItem();
            mnuL.SuspendLayout();
            SuspendLayout();
            // 
            // mnuL
            // 
            mnuL.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            mnuLLegality,
            mnuLQR,
            mnuLSave});
            mnuL.Name = "mnuL";
            mnuL.Size = new System.Drawing.Size(153, 92);
            // 
            // mnuLLegality
            // 
            mnuLLegality.Image = global::PKHeX.WinForms.Properties.Resources.export;
            mnuLLegality.Name = "mnuLLegality";
            mnuLLegality.Size = new System.Drawing.Size(152, 22);
            mnuLLegality.Text = "Legality";
            mnuLLegality.Click += new System.EventHandler(ClickShowLegality);
            // 
            // mnuLQR
            // 
            mnuLQR.Image = global::PKHeX.WinForms.Properties.Resources.qr;
            mnuLQR.Name = "mnuLQR";
            mnuLQR.Size = new System.Drawing.Size(152, 22);
            mnuLQR.Text = "QR!";
            mnuLQR.Click += new System.EventHandler(ClickShowQR);
            // 
            // mnuLSave
            // 
            mnuLSave.Image = global::PKHeX.WinForms.Properties.Resources.savePKM;
            mnuLSave.Name = "mnuLSave";
            mnuLSave.Size = new System.Drawing.Size(152, 22);
            mnuLSave.Text = "Save as...";
            mnuLSave.Click += new System.EventHandler(ClickSaveAs);
            // 
            // ContextMenuPKM
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Name = "ContextMenuPKM";
            mnuL.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ContextMenuStrip mnuL;
        private System.Windows.Forms.ToolStripMenuItem mnuLLegality;
        private System.Windows.Forms.ToolStripMenuItem mnuLQR;
        private System.Windows.Forms.ToolStripMenuItem mnuLSave;
    }
}
