namespace PKHeX
{
    partial class CodeImportPKM
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CodeImportPKM));
            this.RTB_Code = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.B_Paste = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // RTB_Code
            // 
            this.RTB_Code.Location = new System.Drawing.Point(8, 38);
            this.RTB_Code.Name = "RTB_Code";
            this.RTB_Code.ReadOnly = true;
            this.RTB_Code.Size = new System.Drawing.Size(149, 168);
            this.RTB_Code.TabIndex = 0;
            this.RTB_Code.Text = "";
            this.RTB_Code.WordWrap = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(87, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(70, 25);
            this.button1.TabIndex = 1;
            this.button1.Text = "Import";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.B_Import_Click);
            // 
            // B_Paste
            // 
            this.B_Paste.Location = new System.Drawing.Point(8, 7);
            this.B_Paste.Name = "B_Paste";
            this.B_Paste.Size = new System.Drawing.Size(70, 25);
            this.B_Paste.TabIndex = 2;
            this.B_Paste.Text = "Paste";
            this.B_Paste.UseVisualStyleBackColor = true;
            this.B_Paste.Click += new System.EventHandler(this.B_Paste_Click);
            // 
            // CodeImportPKM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(164, 212);
            this.Controls.Add(this.B_Paste);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.RTB_Code);
            this.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CodeImportPKM";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Code Import";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox RTB_Code;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button B_Paste;
    }
}