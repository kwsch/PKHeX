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
            this.PB_BoxSwap = new System.Windows.Forms.PictureBox();
            this.Box = new PKHeX.WinForms.Controls.BoxEditor();
            ((System.ComponentModel.ISupportInitialize)(this.PB_BoxSwap)).BeginInit();
            this.SuspendLayout();
            // 
            // PB_BoxSwap
            // 
            this.PB_BoxSwap.Image = global::PKHeX.WinForms.Properties.Resources.swapBox;
            this.PB_BoxSwap.Location = new System.Drawing.Point(0, 0);
            this.PB_BoxSwap.Name = "PB_BoxSwap";
            this.PB_BoxSwap.Size = new System.Drawing.Size(24, 24);
            this.PB_BoxSwap.TabIndex = 67;
            this.PB_BoxSwap.TabStop = false;
            this.PB_BoxSwap.Click += new System.EventHandler(this.PB_BoxSwap_Click);
            // 
            // Box
            // 
            this.Box.AllowDrop = true;
            this.Box.CanSetCurrentBox = false;
            this.Box.ControlsEnabled = true;
            this.Box.ControlsVisible = true;
            this.Box.CurrentBox = -1;
            this.Box.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Box.Editor = null;
            this.Box.FlagIllegal = false;
            this.Box.Location = new System.Drawing.Point(0, 0);
            this.Box.M = null;
            this.Box.Name = "Box";
            this.Box.Size = new System.Drawing.Size(249, 183);
            this.Box.TabIndex = 68;
            // 
            // SAV_BoxViewer
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(249, 183);
            this.Controls.Add(this.PB_BoxSwap);
            this.Controls.Add(this.Box);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SAV_BoxViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Box Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SAV_BoxViewer_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.PB_BoxSwap)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox PB_BoxSwap;
        public Controls.BoxEditor Box;
    }
}