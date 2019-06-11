namespace PKHeX.WinForms
{
    partial class PoffinCase4Editor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.LB_Poffins = new System.Windows.Forms.ListBox();
            this.B_PoffinDel = new System.Windows.Forms.Button();
            this.B_PoffinAll = new System.Windows.Forms.Button();
            this.PG_Poffins = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // LB_Poffins
            // 
            this.LB_Poffins.FormattingEnabled = true;
            this.LB_Poffins.Location = new System.Drawing.Point(0, 2);
            this.LB_Poffins.Name = "LB_Poffins";
            this.LB_Poffins.Size = new System.Drawing.Size(120, 225);
            this.LB_Poffins.TabIndex = 12;
            this.LB_Poffins.SelectedIndexChanged += new System.EventHandler(this.LB_Poffins_SelectedIndexChanged);
            // 
            // B_PoffinDel
            // 
            this.B_PoffinDel.Location = new System.Drawing.Point(211, 201);
            this.B_PoffinDel.Name = "B_PoffinDel";
            this.B_PoffinDel.Size = new System.Drawing.Size(82, 26);
            this.B_PoffinDel.TabIndex = 11;
            this.B_PoffinDel.Text = "Delete All";
            this.B_PoffinDel.UseVisualStyleBackColor = true;
            this.B_PoffinDel.Click += new System.EventHandler(this.B_PoffinDel_Click);
            // 
            // B_PoffinAll
            // 
            this.B_PoffinAll.Location = new System.Drawing.Point(126, 201);
            this.B_PoffinAll.Name = "B_PoffinAll";
            this.B_PoffinAll.Size = new System.Drawing.Size(79, 26);
            this.B_PoffinAll.TabIndex = 10;
            this.B_PoffinAll.Text = "Give All";
            this.B_PoffinAll.UseVisualStyleBackColor = true;
            this.B_PoffinAll.Click += new System.EventHandler(this.B_PoffinAll_Click);
            // 
            // PG_Poffins
            // 
            this.PG_Poffins.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PG_Poffins.HelpVisible = false;
            this.PG_Poffins.Location = new System.Drawing.Point(126, 2);
            this.PG_Poffins.Name = "PG_Poffins";
            this.PG_Poffins.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.PG_Poffins.Size = new System.Drawing.Size(221, 195);
            this.PG_Poffins.TabIndex = 9;
            this.PG_Poffins.ToolbarVisible = false;
            this.PG_Poffins.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.PG_Poffins_PropertyValueChanged);
            // 
            // PoffinCase4Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LB_Poffins);
            this.Controls.Add(this.B_PoffinDel);
            this.Controls.Add(this.B_PoffinAll);
            this.Controls.Add(this.PG_Poffins);
            this.Name = "PoffinCase4Editor";
            this.Size = new System.Drawing.Size(347, 232);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox LB_Poffins;
        private System.Windows.Forms.Button B_PoffinDel;
        private System.Windows.Forms.Button B_PoffinAll;
        private System.Windows.Forms.PropertyGrid PG_Poffins;
    }
}
