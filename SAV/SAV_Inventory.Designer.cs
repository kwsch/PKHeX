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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SAV_Inventory));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.B_Cancel = new System.Windows.Forms.Button();
            this.B_Save = new System.Windows.Forms.Button();
            this.GB_Pouch = new System.Windows.Forms.GroupBox();
            this.B_DisplayBerries = new System.Windows.Forms.Button();
            this.B_DisplayMedicine = new System.Windows.Forms.Button();
            this.B_DisplayTMHM = new System.Windows.Forms.Button();
            this.B_DisplayKeyItems = new System.Windows.Forms.Button();
            this.B_DisplayItems = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.GB_Pouch.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ColumnHeadersVisible = false;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.ShowEditingIcon = false;
            this.dataGridView1.Size = new System.Drawing.Size(200, 190);
            this.dataGridView1.TabIndex = 12;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dropclick);
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.B_Cancel.Location = new System.Drawing.Point(221, 179);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(48, 23);
            this.B_Cancel.TabIndex = 14;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // B_Save
            // 
            this.B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.B_Save.Location = new System.Drawing.Point(275, 179);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(48, 23);
            this.B_Save.TabIndex = 15;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // GB_Pouch
            // 
            this.GB_Pouch.Controls.Add(this.B_DisplayBerries);
            this.GB_Pouch.Controls.Add(this.B_DisplayMedicine);
            this.GB_Pouch.Controls.Add(this.B_DisplayTMHM);
            this.GB_Pouch.Controls.Add(this.B_DisplayKeyItems);
            this.GB_Pouch.Controls.Add(this.B_DisplayItems);
            this.GB_Pouch.Location = new System.Drawing.Point(221, 6);
            this.GB_Pouch.Name = "GB_Pouch";
            this.GB_Pouch.Size = new System.Drawing.Size(102, 167);
            this.GB_Pouch.TabIndex = 16;
            this.GB_Pouch.TabStop = false;
            this.GB_Pouch.Text = "Pouches";
            // 
            // B_DisplayBerries
            // 
            this.B_DisplayBerries.Location = new System.Drawing.Point(14, 135);
            this.B_DisplayBerries.Name = "B_DisplayBerries";
            this.B_DisplayBerries.Size = new System.Drawing.Size(75, 23);
            this.B_DisplayBerries.TabIndex = 4;
            this.B_DisplayBerries.Text = "Berry";
            this.B_DisplayBerries.UseMnemonic = false;
            this.B_DisplayBerries.UseVisualStyleBackColor = true;
            this.B_DisplayBerries.Click += new System.EventHandler(this.B_DisplayBerries_Click);
            // 
            // B_DisplayMedicine
            // 
            this.B_DisplayMedicine.Location = new System.Drawing.Point(14, 106);
            this.B_DisplayMedicine.Name = "B_DisplayMedicine";
            this.B_DisplayMedicine.Size = new System.Drawing.Size(75, 23);
            this.B_DisplayMedicine.TabIndex = 3;
            this.B_DisplayMedicine.Text = "Medicine";
            this.B_DisplayMedicine.UseMnemonic = false;
            this.B_DisplayMedicine.UseVisualStyleBackColor = true;
            this.B_DisplayMedicine.Click += new System.EventHandler(this.B_DisplayMedicine_Click);
            // 
            // B_DisplayTMHM
            // 
            this.B_DisplayTMHM.Location = new System.Drawing.Point(14, 77);
            this.B_DisplayTMHM.Name = "B_DisplayTMHM";
            this.B_DisplayTMHM.Size = new System.Drawing.Size(75, 23);
            this.B_DisplayTMHM.TabIndex = 2;
            this.B_DisplayTMHM.Text = "TM/HM";
            this.B_DisplayTMHM.UseMnemonic = false;
            this.B_DisplayTMHM.UseVisualStyleBackColor = true;
            this.B_DisplayTMHM.Click += new System.EventHandler(this.B_DisplayTMHM_Click);
            // 
            // B_DisplayKeyItems
            // 
            this.B_DisplayKeyItems.Location = new System.Drawing.Point(14, 48);
            this.B_DisplayKeyItems.Name = "B_DisplayKeyItems";
            this.B_DisplayKeyItems.Size = new System.Drawing.Size(75, 23);
            this.B_DisplayKeyItems.TabIndex = 1;
            this.B_DisplayKeyItems.Text = "Key Items";
            this.B_DisplayKeyItems.UseMnemonic = false;
            this.B_DisplayKeyItems.UseVisualStyleBackColor = true;
            this.B_DisplayKeyItems.Click += new System.EventHandler(this.B_DisplayKeyItems_Click);
            // 
            // B_DisplayItems
            // 
            this.B_DisplayItems.Location = new System.Drawing.Point(14, 19);
            this.B_DisplayItems.Name = "B_DisplayItems";
            this.B_DisplayItems.Size = new System.Drawing.Size(75, 23);
            this.B_DisplayItems.TabIndex = 0;
            this.B_DisplayItems.Text = "Items";
            this.B_DisplayItems.UseMnemonic = false;
            this.B_DisplayItems.UseVisualStyleBackColor = true;
            this.B_DisplayItems.Click += new System.EventHandler(this.B_DisplayItems_Click);
            // 
            // SAV_Inventory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 212);
            this.Controls.Add(this.GB_Pouch);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_Cancel);
            this.Controls.Add(this.dataGridView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(350, 750);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(350, 250);
            this.Name = "SAV_Inventory";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Inventory Editor";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.GB_Pouch.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.GroupBox GB_Pouch;
        private System.Windows.Forms.Button B_DisplayBerries;
        private System.Windows.Forms.Button B_DisplayMedicine;
        private System.Windows.Forms.Button B_DisplayTMHM;
        private System.Windows.Forms.Button B_DisplayKeyItems;
        private System.Windows.Forms.Button B_DisplayItems;
    }
}