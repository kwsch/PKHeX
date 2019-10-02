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
            this.B_Save = new System.Windows.Forms.Button();
            this.B_Cancel = new System.Windows.Forms.Button();
            this.B_CaughtNone = new System.Windows.Forms.Button();
            this.B_CaughtAll = new System.Windows.Forms.Button();
            this.B_SeenNone = new System.Windows.Forms.Button();
            this.B_SeenAll = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.CLB_Caught = new System.Windows.Forms.CheckedListBox();
            this.Label_Seen = new System.Windows.Forms.Label();
            this.CLB_Seen = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // B_Save
            // 
            this.B_Save.Location = new System.Drawing.Point(146, 301);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(85, 23);
            this.B_Save.TabIndex = 19;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // B_Cancel
            // 
            this.B_Cancel.Location = new System.Drawing.Point(53, 301);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(85, 23);
            this.B_Cancel.TabIndex = 18;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // B_CaughtNone
            // 
            this.B_CaughtNone.Location = new System.Drawing.Point(151, 267);
            this.B_CaughtNone.Name = "B_CaughtNone";
            this.B_CaughtNone.Size = new System.Drawing.Size(120, 23);
            this.B_CaughtNone.TabIndex = 17;
            this.B_CaughtNone.Text = "Caught None";
            this.B_CaughtNone.UseVisualStyleBackColor = true;
            this.B_CaughtNone.Click += new System.EventHandler(this.B_CaughtNone_Click);
            // 
            // B_CaughtAll
            // 
            this.B_CaughtAll.Location = new System.Drawing.Point(151, 238);
            this.B_CaughtAll.Name = "B_CaughtAll";
            this.B_CaughtAll.Size = new System.Drawing.Size(120, 23);
            this.B_CaughtAll.TabIndex = 16;
            this.B_CaughtAll.Text = "Caught All";
            this.B_CaughtAll.UseVisualStyleBackColor = true;
            this.B_CaughtAll.Click += new System.EventHandler(this.B_CaughtAll_Click);
            // 
            // B_SeenNone
            // 
            this.B_SeenNone.Location = new System.Drawing.Point(14, 267);
            this.B_SeenNone.Name = "B_SeenNone";
            this.B_SeenNone.Size = new System.Drawing.Size(120, 23);
            this.B_SeenNone.TabIndex = 15;
            this.B_SeenNone.Text = "Seen None";
            this.B_SeenNone.UseVisualStyleBackColor = true;
            this.B_SeenNone.Click += new System.EventHandler(this.B_SeenNone_Click);
            // 
            // B_SeenAll
            // 
            this.B_SeenAll.Location = new System.Drawing.Point(14, 238);
            this.B_SeenAll.Name = "B_SeenAll";
            this.B_SeenAll.Size = new System.Drawing.Size(120, 23);
            this.B_SeenAll.TabIndex = 14;
            this.B_SeenAll.Text = "Seen All";
            this.B_SeenAll.UseVisualStyleBackColor = true;
            this.B_SeenAll.Click += new System.EventHandler(this.B_SeenAll_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(151, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Caught:";
            // 
            // CLB_Caught
            // 
            this.CLB_Caught.FormattingEnabled = true;
            this.CLB_Caught.Location = new System.Drawing.Point(151, 33);
            this.CLB_Caught.Name = "CLB_Caught";
            this.CLB_Caught.Size = new System.Drawing.Size(120, 199);
            this.CLB_Caught.TabIndex = 12;
            this.CLB_Caught.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CLB_Caught_ItemCheck);
            // 
            // Label_Seen
            // 
            this.Label_Seen.AutoSize = true;
            this.Label_Seen.Location = new System.Drawing.Point(14, 13);
            this.Label_Seen.Name = "Label_Seen";
            this.Label_Seen.Size = new System.Drawing.Size(35, 13);
            this.Label_Seen.TabIndex = 11;
            this.Label_Seen.Text = "Seen:";
            // 
            // CLB_Seen
            // 
            this.CLB_Seen.FormattingEnabled = true;
            this.CLB_Seen.Location = new System.Drawing.Point(14, 33);
            this.CLB_Seen.Name = "CLB_Seen";
            this.CLB_Seen.Size = new System.Drawing.Size(120, 199);
            this.CLB_Seen.TabIndex = 10;
            this.CLB_Seen.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CLB_Seen_ItemCheck);
            // 
            // SAV_SimplePokedex
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 336);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_Cancel);
            this.Controls.Add(this.B_CaughtNone);
            this.Controls.Add(this.B_CaughtAll);
            this.Controls.Add(this.B_SeenNone);
            this.Controls.Add(this.B_SeenAll);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CLB_Caught);
            this.Controls.Add(this.Label_Seen);
            this.Controls.Add(this.CLB_Seen);
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximumSize = new System.Drawing.Size(300, 375);
            this.MinimumSize = new System.Drawing.Size(300, 375);
            this.Name = "SAV_SimplePokedex";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Pokedex Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_CaughtNone;
        private System.Windows.Forms.Button B_CaughtAll;
        private System.Windows.Forms.Button B_SeenNone;
        private System.Windows.Forms.Button B_SeenAll;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckedListBox CLB_Caught;
        private System.Windows.Forms.Label Label_Seen;
        private System.Windows.Forms.CheckedListBox CLB_Seen;
    }
}