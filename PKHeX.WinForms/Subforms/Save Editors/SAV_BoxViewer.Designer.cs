namespace PKHeX.WinForms
{
    sealed partial class SAV_BoxViewer
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
            PB_BoxSwap = new System.Windows.Forms.PictureBox();
            Box = new PKHeX.WinForms.Controls.BoxEditor();
            ((System.ComponentModel.ISupportInitialize)(PB_BoxSwap)).BeginInit();
            SuspendLayout();
            // 
            // PB_BoxSwap
            // 
            PB_BoxSwap.Image = global::PKHeX.WinForms.Properties.Resources.swapBox;
            PB_BoxSwap.Location = new System.Drawing.Point(0, 0);
            PB_BoxSwap.Name = "PB_BoxSwap";
            PB_BoxSwap.Size = new System.Drawing.Size(24, 24);
            PB_BoxSwap.TabIndex = 67;
            PB_BoxSwap.TabStop = false;
            PB_BoxSwap.Click += new System.EventHandler(PB_BoxSwap_Click);
            // 
            // Box
            // 
            Box.AllowDrop = true;
            Box.CanSetCurrentBox = false;
            Box.ControlsEnabled = true;
            Box.ControlsVisible = true;
            Box.CurrentBox = -1;
            Box.Dock = System.Windows.Forms.DockStyle.Fill;
            Box.Editor = null;
            Box.FlagIllegal = false;
            Box.Location = new System.Drawing.Point(0, 0);
            Box.M = null;
            Box.Name = "Box";
            Box.Size = new System.Drawing.Size(249, 183);
            Box.TabIndex = 68;
            // 
            // SAV_BoxViewer
            // 
            AllowDrop = true;
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(249, 183);
            Controls.Add(PB_BoxSwap);
            Controls.Add(Box);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_BoxViewer";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Box Viewer";
            FormClosing += new System.Windows.Forms.FormClosingEventHandler(SAV_BoxViewer_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(PB_BoxSwap)).EndInit();
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox PB_BoxSwap;
        public Controls.BoxEditor Box;
    }
}
