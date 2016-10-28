namespace PKHeX
{
    partial class SAV_Inventory
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SAV_Inventory));
            this.B_Cancel = new System.Windows.Forms.Button();
            this.B_Save = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.IL_Pouch = new System.Windows.Forms.ImageList(this.components);
            this.B_GiveAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(177, 328);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(70, 23);
            this.B_Cancel.TabIndex = 14;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(253, 328);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(70, 23);
            this.B_Save.TabIndex = 15;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.ImageList = this.IL_Pouch;
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(311, 308);
            this.tabControl1.TabIndex = 17;
            // 
            // IL_Pouch
            // 
            this.IL_Pouch.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("IL_Pouch.ImageStream")));
            this.IL_Pouch.TransparentColor = System.Drawing.Color.Transparent;
            this.IL_Pouch.Images.SetKeyName(0, "Bag_Items.png");
            this.IL_Pouch.Images.SetKeyName(1, "Bag_Key.png");
            this.IL_Pouch.Images.SetKeyName(2, "Bag_TMHM.png");
            this.IL_Pouch.Images.SetKeyName(3, "Bag_Medicine.png");
            this.IL_Pouch.Images.SetKeyName(4, "Bag_Berries.png");
            this.IL_Pouch.Images.SetKeyName(5, "Bag_Balls.png");
            this.IL_Pouch.Images.SetKeyName(6, "Bag_Battle.png");
            this.IL_Pouch.Images.SetKeyName(7, "Bag_Mail.png");
            this.IL_Pouch.Images.SetKeyName(8, "Bag_PCItems.png");
            this.IL_Pouch.Images.SetKeyName(9, "Bag_Free.png");
            this.IL_Pouch.Images.SetKeyName(10, "Bag_Z.png");
            // 
            // B_GiveAll
            // 
            this.B_GiveAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.B_GiveAll.Location = new System.Drawing.Point(12, 328);
            this.B_GiveAll.Name = "B_GiveAll";
            this.B_GiveAll.Size = new System.Drawing.Size(75, 23);
            this.B_GiveAll.TabIndex = 18;
            this.B_GiveAll.Text = "Give All";
            this.B_GiveAll.UseVisualStyleBackColor = true;
            this.B_GiveAll.Click += new System.EventHandler(this.B_GiveAll_Click);
            // 
            // SAV_Inventory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 361);
            this.Controls.Add(this.B_GiveAll);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_Cancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(350, 400);
            this.Name = "SAV_Inventory";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Inventory Editor";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Button B_GiveAll;
        private System.Windows.Forms.ImageList IL_Pouch;
    }
}