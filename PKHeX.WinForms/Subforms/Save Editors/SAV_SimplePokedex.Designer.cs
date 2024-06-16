namespace PKHeX.WinForms
{
    partial class SAV_SimplePokedex
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
            B_Save = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            B_CaughtNone = new System.Windows.Forms.Button();
            B_CaughtAll = new System.Windows.Forms.Button();
            B_SeenNone = new System.Windows.Forms.Button();
            B_SeenAll = new System.Windows.Forms.Button();
            Label_Caught = new System.Windows.Forms.Label();
            CLB_Caught = new System.Windows.Forms.CheckedListBox();
            Label_Seen = new System.Windows.Forms.Label();
            CLB_Seen = new System.Windows.Forms.CheckedListBox();
            SuspendLayout();
            // 
            // B_Save
            // 
            B_Save.Location = new System.Drawing.Point(170, 347);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(99, 27);
            B_Save.TabIndex = 19;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Location = new System.Drawing.Point(62, 347);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(99, 27);
            B_Cancel.TabIndex = 18;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_CaughtNone
            // 
            B_CaughtNone.Location = new System.Drawing.Point(176, 308);
            B_CaughtNone.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_CaughtNone.Name = "B_CaughtNone";
            B_CaughtNone.Size = new System.Drawing.Size(140, 27);
            B_CaughtNone.TabIndex = 17;
            B_CaughtNone.Text = "Caught None";
            B_CaughtNone.UseVisualStyleBackColor = true;
            B_CaughtNone.Click += B_CaughtNone_Click;
            // 
            // B_CaughtAll
            // 
            B_CaughtAll.Location = new System.Drawing.Point(176, 275);
            B_CaughtAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_CaughtAll.Name = "B_CaughtAll";
            B_CaughtAll.Size = new System.Drawing.Size(140, 27);
            B_CaughtAll.TabIndex = 16;
            B_CaughtAll.Text = "Caught All";
            B_CaughtAll.UseVisualStyleBackColor = true;
            B_CaughtAll.Click += B_CaughtAll_Click;
            // 
            // B_SeenNone
            // 
            B_SeenNone.Location = new System.Drawing.Point(16, 308);
            B_SeenNone.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_SeenNone.Name = "B_SeenNone";
            B_SeenNone.Size = new System.Drawing.Size(140, 27);
            B_SeenNone.TabIndex = 15;
            B_SeenNone.Text = "Seen None";
            B_SeenNone.UseVisualStyleBackColor = true;
            B_SeenNone.Click += B_SeenNone_Click;
            // 
            // B_SeenAll
            // 
            B_SeenAll.Location = new System.Drawing.Point(16, 275);
            B_SeenAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_SeenAll.Name = "B_SeenAll";
            B_SeenAll.Size = new System.Drawing.Size(140, 27);
            B_SeenAll.TabIndex = 14;
            B_SeenAll.Text = "Seen All";
            B_SeenAll.UseVisualStyleBackColor = true;
            B_SeenAll.Click += B_SeenAll_Click;
            // 
            // Label_Caught
            // 
            Label_Caught.AutoSize = true;
            Label_Caught.Location = new System.Drawing.Point(176, 15);
            Label_Caught.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label_Caught.Name = "Label_Caught";
            Label_Caught.Size = new System.Drawing.Size(49, 15);
            Label_Caught.TabIndex = 13;
            Label_Caught.Text = "Caught:";
            // 
            // CLB_Caught
            // 
            CLB_Caught.FormattingEnabled = true;
            CLB_Caught.Location = new System.Drawing.Point(176, 38);
            CLB_Caught.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CLB_Caught.Name = "CLB_Caught";
            CLB_Caught.Size = new System.Drawing.Size(139, 220);
            CLB_Caught.TabIndex = 12;
            // 
            // Label_Seen
            // 
            Label_Seen.AutoSize = true;
            Label_Seen.Location = new System.Drawing.Point(16, 15);
            Label_Seen.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            Label_Seen.Name = "Label_Seen";
            Label_Seen.Size = new System.Drawing.Size(35, 15);
            Label_Seen.TabIndex = 11;
            Label_Seen.Text = "Seen:";
            // 
            // CLB_Seen
            // 
            CLB_Seen.FormattingEnabled = true;
            CLB_Seen.Location = new System.Drawing.Point(16, 38);
            CLB_Seen.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CLB_Seen.Name = "CLB_Seen";
            CLB_Seen.Size = new System.Drawing.Size(139, 220);
            CLB_Seen.TabIndex = 10;
            // 
            // SAV_SimplePokedex
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(331, 388);
            Controls.Add(B_Save);
            Controls.Add(B_Cancel);
            Controls.Add(B_CaughtNone);
            Controls.Add(B_CaughtAll);
            Controls.Add(B_SeenNone);
            Controls.Add(B_SeenAll);
            Controls.Add(Label_Caught);
            Controls.Add(CLB_Caught);
            Controls.Add(Label_Seen);
            Controls.Add(CLB_Seen);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(347, 427);
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(347, 427);
            Name = "SAV_SimplePokedex";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Pokedex Editor";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_CaughtNone;
        private System.Windows.Forms.Button B_CaughtAll;
        private System.Windows.Forms.Button B_SeenNone;
        private System.Windows.Forms.Button B_SeenAll;
        private System.Windows.Forms.Label Label_Caught;
        private System.Windows.Forms.CheckedListBox CLB_Caught;
        private System.Windows.Forms.Label Label_Seen;
        private System.Windows.Forms.CheckedListBox CLB_Seen;
    }
}
