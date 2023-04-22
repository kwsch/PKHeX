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
            PartyPokeGrid = new PKHeX.WinForms.Controls.PokeGrid();
            SuspendLayout();
            // 
            // pokeGrid1
            // 
            PartyPokeGrid.AutoSize = true;
            PartyPokeGrid.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            PartyPokeGrid.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            PartyPokeGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            PartyPokeGrid.Location = new System.Drawing.Point(0, 0);
            PartyPokeGrid.Margin = new System.Windows.Forms.Padding(0);
            PartyPokeGrid.Name = "PartyPokeGrid";
            PartyPokeGrid.Size = new System.Drawing.Size(251, 185);
            PartyPokeGrid.TabIndex = 66;
            // 
            // PartyEditor
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            AutoSize = true;
            Controls.Add(PartyPokeGrid);
            Name = "PartyEditor";
            Size = new System.Drawing.Size(251, 185);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private PokeGrid PartyPokeGrid;
    }
}
