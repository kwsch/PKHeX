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
            CB_Game = new System.Windows.Forms.ComboBox();
            B_OK = new System.Windows.Forms.Button();
            L_Game = new System.Windows.Forms.Label();
            B_Cancel = new System.Windows.Forms.Button();
            L_Prompt = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // CB_Game
            // 
            CB_Game.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            CB_Game.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Game.FormattingEnabled = true;
            CB_Game.Location = new System.Drawing.Point(80, 90);
            CB_Game.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Game.Name = "CB_Game";
            CB_Game.Size = new System.Drawing.Size(140, 23);
            CB_Game.TabIndex = 0;
            // 
            // B_OK
            // 
            B_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_OK.Location = new System.Drawing.Point(306, 89);
            B_OK.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_OK.Name = "B_OK";
            B_OK.Size = new System.Drawing.Size(70, 27);
            B_OK.TabIndex = 11;
            B_OK.Text = "OK";
            B_OK.UseVisualStyleBackColor = true;
            B_OK.Click += B_OK_Click;
            // 
            // L_Game
            // 
            L_Game.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            L_Game.Location = new System.Drawing.Point(14, 90);
            L_Game.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Game.Name = "L_Game";
            L_Game.Size = new System.Drawing.Size(59, 24);
            L_Game.TabIndex = 12;
            L_Game.Text = "Game:";
            L_Game.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(229, 89);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(70, 27);
            B_Cancel.TabIndex = 10;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // L_Prompt
            // 
            L_Prompt.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            L_Prompt.Location = new System.Drawing.Point(18, 10);
            L_Prompt.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Prompt.Name = "L_Prompt";
            L_Prompt.Size = new System.Drawing.Size(358, 75);
            L_Prompt.TabIndex = 13;
            L_Prompt.Text = "Prompt Text is here...";
            // 
            // SAV_GameSelect
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(390, 129);
            Controls.Add(L_Prompt);
            Controls.Add(L_Game);
            Controls.Add(B_OK);
            Controls.Add(B_Cancel);
            Controls.Add(CB_Game);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            KeyPreview = true;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_GameSelect";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Game Selection";
            KeyDown += SAV_GameSelect_KeyDown;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ComboBox CB_Game;
        private System.Windows.Forms.Button B_OK;
        private System.Windows.Forms.Label L_Game;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Label L_Prompt;
    }
}
