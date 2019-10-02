namespace PKHeX.WinForms.Controls
{
    partial class ShinyLeaf
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.CHK_1 = new System.Windows.Forms.CheckBox();
            this.CHK_2 = new System.Windows.Forms.CheckBox();
            this.CHK_3 = new System.Windows.Forms.CheckBox();
            this.CHK_4 = new System.Windows.Forms.CheckBox();
            this.CHK_5 = new System.Windows.Forms.CheckBox();
            this.CHK_C = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.CHK_1);
            this.flowLayoutPanel1.Controls.Add(this.CHK_2);
            this.flowLayoutPanel1.Controls.Add(this.CHK_3);
            this.flowLayoutPanel1.Controls.Add(this.CHK_4);
            this.flowLayoutPanel1.Controls.Add(this.CHK_5);
            this.flowLayoutPanel1.Controls.Add(this.CHK_C);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(140, 56);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // CHK_1
            // 
            this.CHK_1.Image = global::PKHeX.WinForms.Properties.Resources.leaf;
            this.CHK_1.Location = new System.Drawing.Point(0, 0);
            this.CHK_1.Margin = new System.Windows.Forms.Padding(0);
            this.CHK_1.Name = "CHK_1";
            this.CHK_1.Size = new System.Drawing.Size(40, 30);
            this.CHK_1.TabIndex = 0;
            this.CHK_1.UseVisualStyleBackColor = true;
            this.CHK_1.CheckedChanged += new System.EventHandler(this.UpdateFlagState);
            // 
            // CHK_2
            // 
            this.CHK_2.Image = global::PKHeX.WinForms.Properties.Resources.leaf;
            this.CHK_2.Location = new System.Drawing.Point(40, 0);
            this.CHK_2.Margin = new System.Windows.Forms.Padding(0);
            this.CHK_2.Name = "CHK_2";
            this.CHK_2.Size = new System.Drawing.Size(40, 30);
            this.CHK_2.TabIndex = 1;
            this.CHK_2.UseVisualStyleBackColor = true;
            this.CHK_2.CheckedChanged += new System.EventHandler(this.UpdateFlagState);
            // 
            // CHK_3
            // 
            this.CHK_3.Image = global::PKHeX.WinForms.Properties.Resources.leaf;
            this.CHK_3.Location = new System.Drawing.Point(80, 0);
            this.CHK_3.Margin = new System.Windows.Forms.Padding(0);
            this.CHK_3.Name = "CHK_3";
            this.CHK_3.Size = new System.Drawing.Size(40, 30);
            this.CHK_3.TabIndex = 2;
            this.CHK_3.UseVisualStyleBackColor = true;
            this.CHK_3.CheckedChanged += new System.EventHandler(this.UpdateFlagState);
            // 
            // CHK_4
            // 
            this.CHK_4.Image = global::PKHeX.WinForms.Properties.Resources.leaf;
            this.CHK_4.Location = new System.Drawing.Point(0, 30);
            this.CHK_4.Margin = new System.Windows.Forms.Padding(0);
            this.CHK_4.Name = "CHK_4";
            this.CHK_4.Size = new System.Drawing.Size(40, 30);
            this.CHK_4.TabIndex = 3;
            this.CHK_4.UseVisualStyleBackColor = true;
            this.CHK_4.CheckedChanged += new System.EventHandler(this.UpdateFlagState);
            // 
            // CHK_5
            // 
            this.CHK_5.Image = global::PKHeX.WinForms.Properties.Resources.leaf;
            this.CHK_5.Location = new System.Drawing.Point(40, 30);
            this.CHK_5.Margin = new System.Windows.Forms.Padding(0);
            this.CHK_5.Name = "CHK_5";
            this.CHK_5.Size = new System.Drawing.Size(40, 30);
            this.CHK_5.TabIndex = 4;
            this.CHK_5.UseVisualStyleBackColor = true;
            this.CHK_5.CheckedChanged += new System.EventHandler(this.UpdateFlagState);
            // 
            // CHK_C
            // 
            this.CHK_C.Image = global::PKHeX.WinForms.Properties.Resources.crown;
            this.CHK_C.Location = new System.Drawing.Point(80, 30);
            this.CHK_C.Margin = new System.Windows.Forms.Padding(0);
            this.CHK_C.Name = "CHK_C";
            this.CHK_C.Size = new System.Drawing.Size(60, 28);
            this.CHK_C.TabIndex = 5;
            this.CHK_C.UseVisualStyleBackColor = true;
            this.CHK_C.CheckedChanged += new System.EventHandler(this.UpdateFlagState);
            // 
            // ShinyLeaf
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "ShinyLeaf";
            this.Size = new System.Drawing.Size(140, 56);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox CHK_1;
        private System.Windows.Forms.CheckBox CHK_2;
        private System.Windows.Forms.CheckBox CHK_3;
        private System.Windows.Forms.CheckBox CHK_4;
        private System.Windows.Forms.CheckBox CHK_5;
        private System.Windows.Forms.CheckBox CHK_C;
    }
}
