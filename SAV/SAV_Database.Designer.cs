namespace PKHeX
{
    partial class SAV_Database
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SAV_Database));
            this.B_OpenDB = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // B_OpenDB
            // 
            this.B_OpenDB.Location = new System.Drawing.Point(397, 12);
            this.B_OpenDB.Name = "B_OpenDB";
            this.B_OpenDB.Size = new System.Drawing.Size(75, 23);
            this.B_OpenDB.TabIndex = 0;
            this.B_OpenDB.Text = "OpenDB";
            this.B_OpenDB.UseVisualStyleBackColor = true;
            this.B_OpenDB.Click += new System.EventHandler(this.openDB);
            // 
            // SAV_Database
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 362);
            this.Controls.Add(this.B_OpenDB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SAV_Database";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Database";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button B_OpenDB;

    }
}