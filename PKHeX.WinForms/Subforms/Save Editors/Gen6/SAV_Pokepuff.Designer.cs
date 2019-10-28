namespace PKHeX.WinForms
{
    partial class SAV_Pokepuff
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
            this.dgv = new System.Windows.Forms.DataGridView();
            this.B_Save = new System.Windows.Forms.Button();
            this.B_Cancel = new System.Windows.Forms.Button();
            this.B_All = new System.Windows.Forms.Button();
            this.B_Sort = new System.Windows.Forms.Button();
            this.B_None = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AllowUserToResizeColumns = false;
            this.dgv.AllowUserToResizeRows = false;
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.dgv.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgv.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgv.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.ColumnHeadersVisible = false;
            this.dgv.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgv.Location = new System.Drawing.Point(12, 43);
            this.dgv.MultiSelect = false;
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersVisible = false;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgv.ShowEditingIcon = false;
            this.dgv.Size = new System.Drawing.Size(200, 186);
            this.dgv.TabIndex = 11;
            this.dgv.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DropClick);
            // 
            // B_Save
            // 
            this.B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.B_Save.Location = new System.Drawing.Point(162, 235);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(50, 23);
            this.B_Save.TabIndex = 12;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.B_Cancel.Location = new System.Drawing.Point(106, 235);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(50, 23);
            this.B_Cancel.TabIndex = 13;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // B_All
            // 
            this.B_All.Location = new System.Drawing.Point(12, 14);
            this.B_All.Name = "B_All";
            this.B_All.Size = new System.Drawing.Size(60, 23);
            this.B_All.TabIndex = 14;
            this.B_All.Text = "All";
            this.B_All.UseVisualStyleBackColor = true;
            this.B_All.Click += new System.EventHandler(this.B_All_Click);
            // 
            // B_Sort
            // 
            this.B_Sort.Location = new System.Drawing.Point(152, 14);
            this.B_Sort.Name = "B_Sort";
            this.B_Sort.Size = new System.Drawing.Size(60, 23);
            this.B_Sort.TabIndex = 15;
            this.B_Sort.Text = "Sort";
            this.B_Sort.UseVisualStyleBackColor = true;
            this.B_Sort.Click += new System.EventHandler(this.B_Sort_Click);
            // 
            // B_None
            // 
            this.B_None.Location = new System.Drawing.Point(82, 14);
            this.B_None.Name = "B_None";
            this.B_None.Size = new System.Drawing.Size(60, 23);
            this.B_None.TabIndex = 16;
            this.B_None.Text = "None";
            this.B_None.UseVisualStyleBackColor = true;
            this.B_None.Click += new System.EventHandler(this.B_None_Click);
            // 
            // SAV_Pokepuff
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 272);
            this.Controls.Add(this.B_None);
            this.Controls.Add(this.B_Sort);
            this.Controls.Add(this.B_All);
            this.Controls.Add(this.B_Cancel);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.dgv);
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(240, 750);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(240, 300);
            this.Name = "SAV_Pokepuff";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "‎Poké Puffs Editor";
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_All;
        private System.Windows.Forms.Button B_Sort;
        private System.Windows.Forms.Button B_None;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolTip toolTip2;
    }
}