namespace PKHeX.WinForms
{
    partial class SAV_GameSelect
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
            this.CB_Game = new System.Windows.Forms.ComboBox();
            this.B_OK = new System.Windows.Forms.Button();
            this.L_Game = new System.Windows.Forms.Label();
            this.B_Cancel = new System.Windows.Forms.Button();
            this.L_Prompt = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // CB_Game
            // 
            this.CB_Game.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_Game.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Game.FormattingEnabled = true;
            this.CB_Game.Location = new System.Drawing.Point(69, 78);
            this.CB_Game.Name = "CB_Game";
            this.CB_Game.Size = new System.Drawing.Size(121, 21);
            this.CB_Game.TabIndex = 0;
            // 
            // B_OK
            // 
            this.B_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_OK.Location = new System.Drawing.Point(262, 77);
            this.B_OK.Name = "B_OK";
            this.B_OK.Size = new System.Drawing.Size(60, 23);
            this.B_OK.TabIndex = 11;
            this.B_OK.Text = "OK";
            this.B_OK.UseVisualStyleBackColor = true;
            this.B_OK.Click += new System.EventHandler(this.B_OK_Click);
            // 
            // L_Game
            // 
            this.L_Game.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.L_Game.Location = new System.Drawing.Point(12, 78);
            this.L_Game.Name = "L_Game";
            this.L_Game.Size = new System.Drawing.Size(51, 21);
            this.L_Game.TabIndex = 12;
            this.L_Game.Text = "Game:";
            this.L_Game.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(196, 77);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(60, 23);
            this.B_Cancel.TabIndex = 10;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // L_Prompt
            // 
            this.L_Prompt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.L_Prompt.Location = new System.Drawing.Point(15, 9);
            this.L_Prompt.Name = "L_Prompt";
            this.L_Prompt.Size = new System.Drawing.Size(307, 65);
            this.L_Prompt.TabIndex = 13;
            this.L_Prompt.Text = "Prompt Text is here...";
            // 
            // SAV_GameSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 112);
            this.Controls.Add(this.L_Prompt);
            this.Controls.Add(this.L_Game);
            this.Controls.Add(this.B_OK);
            this.Controls.Add(this.B_Cancel);
            this.Controls.Add(this.CB_Game);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SAV_GameSelect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Game Selection";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SAV_GameSelect_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox CB_Game;
        private System.Windows.Forms.Button B_OK;
        private System.Windows.Forms.Label L_Game;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Label L_Prompt;
    }
}