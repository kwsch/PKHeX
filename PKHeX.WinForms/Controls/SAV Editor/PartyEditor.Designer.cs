namespace PKHeX.WinForms.Controls
{
    partial class PartyEditor
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
            this.PartyPokeGrid = new PKHeX.WinForms.Controls.PokeGrid();
            this.SuspendLayout();
            // 
            // pokeGrid1
            // 
            this.PartyPokeGrid.AutoSize = true;
            this.PartyPokeGrid.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.PartyPokeGrid.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.PartyPokeGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PartyPokeGrid.Location = new System.Drawing.Point(0, 0);
            this.PartyPokeGrid.Margin = new System.Windows.Forms.Padding(0);
            this.PartyPokeGrid.Name = "PartyPokeGrid";
            this.PartyPokeGrid.Size = new System.Drawing.Size(251, 185);
            this.PartyPokeGrid.TabIndex = 66;
            // 
            // PartyEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSize = true;
            this.Controls.Add(this.PartyPokeGrid);
            this.Name = "PartyEditor";
            this.Size = new System.Drawing.Size(251, 185);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private PokeGrid PartyPokeGrid;
    }
}
