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
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            CHK_1 = new System.Windows.Forms.CheckBox();
            CHK_2 = new System.Windows.Forms.CheckBox();
            CHK_3 = new System.Windows.Forms.CheckBox();
            CHK_4 = new System.Windows.Forms.CheckBox();
            CHK_5 = new System.Windows.Forms.CheckBox();
            CHK_C = new System.Windows.Forms.CheckBox();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(CHK_1);
            flowLayoutPanel1.Controls.Add(CHK_2);
            flowLayoutPanel1.Controls.Add(CHK_3);
            flowLayoutPanel1.Controls.Add(CHK_4);
            flowLayoutPanel1.Controls.Add(CHK_5);
            flowLayoutPanel1.Controls.Add(CHK_C);
            flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(200, 64);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // CHK_1
            // 
            CHK_1.Image = Properties.Resources.leaf;
            CHK_1.Location = new System.Drawing.Point(0, 0);
            CHK_1.Margin = new System.Windows.Forms.Padding(0);
            CHK_1.Name = "CHK_1";
            CHK_1.Size = new System.Drawing.Size(40, 32);
            CHK_1.TabIndex = 0;
            CHK_1.UseVisualStyleBackColor = true;
            CHK_1.CheckedChanged += UpdateFlagState;
            // 
            // CHK_2
            // 
            CHK_2.Image = Properties.Resources.leaf;
            CHK_2.Location = new System.Drawing.Point(40, 0);
            CHK_2.Margin = new System.Windows.Forms.Padding(0);
            CHK_2.Name = "CHK_2";
            CHK_2.Size = new System.Drawing.Size(40, 32);
            CHK_2.TabIndex = 1;
            CHK_2.UseVisualStyleBackColor = true;
            CHK_2.CheckedChanged += UpdateFlagState;
            // 
            // CHK_3
            // 
            CHK_3.Image = Properties.Resources.leaf;
            CHK_3.Location = new System.Drawing.Point(80, 0);
            CHK_3.Margin = new System.Windows.Forms.Padding(0);
            CHK_3.Name = "CHK_3";
            CHK_3.Size = new System.Drawing.Size(40, 32);
            CHK_3.TabIndex = 2;
            CHK_3.UseVisualStyleBackColor = true;
            CHK_3.CheckedChanged += UpdateFlagState;
            // 
            // CHK_4
            // 
            CHK_4.Image = Properties.Resources.leaf;
            CHK_4.Location = new System.Drawing.Point(120, 0);
            CHK_4.Margin = new System.Windows.Forms.Padding(0);
            CHK_4.Name = "CHK_4";
            CHK_4.Size = new System.Drawing.Size(40, 32);
            CHK_4.TabIndex = 3;
            CHK_4.UseVisualStyleBackColor = true;
            CHK_4.CheckedChanged += UpdateFlagState;
            // 
            // CHK_5
            // 
            CHK_5.Image = Properties.Resources.leaf;
            CHK_5.Location = new System.Drawing.Point(160, 0);
            CHK_5.Margin = new System.Windows.Forms.Padding(0);
            CHK_5.Name = "CHK_5";
            CHK_5.Size = new System.Drawing.Size(40, 32);
            CHK_5.TabIndex = 4;
            CHK_5.UseVisualStyleBackColor = true;
            CHK_5.CheckedChanged += UpdateFlagState;
            // 
            // CHK_C
            // 
            CHK_C.Image = Properties.Resources.crown;
            CHK_C.Location = new System.Drawing.Point(0, 32);
            CHK_C.Margin = new System.Windows.Forms.Padding(0);
            CHK_C.Name = "CHK_C";
            CHK_C.Size = new System.Drawing.Size(64, 32);
            CHK_C.TabIndex = 5;
            CHK_C.UseVisualStyleBackColor = true;
            CHK_C.CheckedChanged += UpdateFlagState;
            // 
            // ShinyLeaf
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(flowLayoutPanel1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "ShinyLeaf";
            Size = new System.Drawing.Size(200, 64);
            flowLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
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
