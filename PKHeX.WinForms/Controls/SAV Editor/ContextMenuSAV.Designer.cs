namespace PKHeX.WinForms.Controls
{
    partial class ContextMenuSAV
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
            this.mnuVSD = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSet = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLegality = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuVSD.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuVSD
            // 
            this.mnuVSD.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuView,
            this.mnuSet,
            this.mnuDelete,
            this.mnuLegality});
            this.mnuVSD.Name = "mnuVSD";
            this.mnuVSD.Size = new System.Drawing.Size(153, 114);
            this.mnuVSD.Opening += new System.ComponentModel.CancelEventHandler(this.MenuOpening);
            // 
            // mnuView
            // 
            this.mnuView.Image = global::PKHeX.WinForms.Properties.Resources.other;
            this.mnuView.Name = "mnuView";
            this.mnuView.Size = new System.Drawing.Size(152, 22);
            this.mnuView.Text = "View";
            this.mnuView.Click += new System.EventHandler(this.ClickView);
            // 
            // mnuSet
            // 
            this.mnuSet.Image = global::PKHeX.WinForms.Properties.Resources.exit;
            this.mnuSet.Name = "mnuSet";
            this.mnuSet.Size = new System.Drawing.Size(152, 22);
            this.mnuSet.Text = "Set";
            this.mnuSet.Click += new System.EventHandler(this.ClickSet);
            // 
            // mnuDelete
            // 
            this.mnuDelete.Image = global::PKHeX.WinForms.Properties.Resources.nocheck;
            this.mnuDelete.Name = "mnuDelete";
            this.mnuDelete.Size = new System.Drawing.Size(152, 22);
            this.mnuDelete.Text = "Delete";
            this.mnuDelete.Click += new System.EventHandler(this.ClickDelete);
            // 
            // mnuLegality
            // 
            this.mnuLegality.Image = global::PKHeX.WinForms.Properties.Resources.export;
            this.mnuLegality.Name = "mnuLegality";
            this.mnuLegality.Size = new System.Drawing.Size(152, 22);
            this.mnuLegality.Text = "Legality";
            this.mnuLegality.Click += new System.EventHandler(this.ClickShowLegality);
            // 
            // ContextMenuSAV
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSize = true;
            this.Name = "ContextMenuSAV";
            this.mnuVSD.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ContextMenuStrip mnuVSD;
        private System.Windows.Forms.ToolStripMenuItem mnuView;
        private System.Windows.Forms.ToolStripMenuItem mnuSet;
        private System.Windows.Forms.ToolStripMenuItem mnuDelete;
        private System.Windows.Forms.ToolStripMenuItem mnuLegality;
    }
}
