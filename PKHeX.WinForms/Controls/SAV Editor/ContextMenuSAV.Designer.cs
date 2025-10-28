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
            components = new System.ComponentModel.Container();
            mnuVSD = new System.Windows.Forms.ContextMenuStrip(components);
            mnuView = new System.Windows.Forms.ToolStripMenuItem();
            mnuSet = new System.Windows.Forms.ToolStripMenuItem();
            mnuDelete = new System.Windows.Forms.ToolStripMenuItem();
            mnuLegality = new System.Windows.Forms.ToolStripMenuItem();
            mnuVSD.SuspendLayout();
            SuspendLayout();
            // 
            // mnuVSD
            // 
            mnuVSD.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            mnuView,
            mnuSet,
            mnuDelete,
            mnuLegality});
            mnuVSD.Name = "mnuVSD";
            mnuVSD.Size = new System.Drawing.Size(153, 114);
            mnuVSD.Opening += new System.ComponentModel.CancelEventHandler(MenuOpening);
            // 
            // mnuView
            // 
            mnuView.Image = global::PKHeX.WinForms.Properties.Resources.other;
            mnuView.Name = "mnuView";
            mnuView.Size = new System.Drawing.Size(152, 22);
            mnuView.Text = "View";
            mnuView.Click += new System.EventHandler(ClickView);
            // 
            // mnuSet
            // 
            mnuSet.Image = global::PKHeX.WinForms.Properties.Resources.exit;
            mnuSet.Name = "mnuSet";
            mnuSet.Size = new System.Drawing.Size(152, 22);
            mnuSet.Text = "Set";
            mnuSet.Click += new System.EventHandler(ClickSet);
            // 
            // mnuDelete
            // 
            mnuDelete.Image = global::PKHeX.WinForms.Properties.Resources.nocheck;
            mnuDelete.Name = "mnuDelete";
            mnuDelete.Size = new System.Drawing.Size(152, 22);
            mnuDelete.Text = "Delete";
            mnuDelete.Click += new System.EventHandler(ClickDelete);
            // 
            // mnuLegality
            // 
            mnuLegality.Image = global::PKHeX.WinForms.Properties.Resources.export;
            mnuLegality.Name = "mnuLegality";
            mnuLegality.Size = new System.Drawing.Size(152, 22);
            mnuLegality.Text = "Legality";
            mnuLegality.Click += new System.EventHandler(ClickShowLegality);
            // 
            // ContextMenuSAV
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            AutoSize = true;
            Name = "ContextMenuSAV";
            mnuVSD.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ContextMenuStrip mnuVSD;
        private System.Windows.Forms.ToolStripMenuItem mnuView;
        private System.Windows.Forms.ToolStripMenuItem mnuSet;
        private System.Windows.Forms.ToolStripMenuItem mnuDelete;
        private System.Windows.Forms.ToolStripMenuItem mnuLegality;
    }
}
