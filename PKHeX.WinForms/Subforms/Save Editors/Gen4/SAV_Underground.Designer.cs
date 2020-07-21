namespace PKHeX.WinForms
{
    partial class SAV_Underground
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
            this.LU_PlayersMet = new System.Windows.Forms.Label();
            this.U_PlayersMet = new System.Windows.Forms.NumericUpDown();
            this.LU_Gifts = new System.Windows.Forms.Label();
            this.U_Gifts = new System.Windows.Forms.NumericUpDown();
            this.LU_Spheres = new System.Windows.Forms.Label();
            this.U_Spheres = new System.Windows.Forms.NumericUpDown();
            this.LU_Fossils = new System.Windows.Forms.Label();
            this.U_Fossils = new System.Windows.Forms.NumericUpDown();
            this.LU_TrapsA = new System.Windows.Forms.Label();
            this.U_TrapsA = new System.Windows.Forms.NumericUpDown();
            this.LU_TrapsT = new System.Windows.Forms.Label();
            this.U_TrapsT = new System.Windows.Forms.NumericUpDown();
            this.LU_Flags = new System.Windows.Forms.Label();
            this.U_Flags = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.U_PlayersMet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.U_Gifts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.U_Spheres)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.U_Fossils)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.U_TrapsA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.U_TrapsT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.U_Flags)).BeginInit();
            this.SuspendLayout();
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(269, 223);
            this.B_Save.Margin = new System.Windows.Forms.Padding(4);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(100, 28);
            this.B_Save.TabIndex = 26;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(161, 223);
            this.B_Cancel.Margin = new System.Windows.Forms.Padding(4);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(100, 28);
            this.B_Cancel.TabIndex = 25;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // LU_PlayersMet
            // 
            this.LU_PlayersMet.Location = new System.Drawing.Point(12, 17);
            this.LU_PlayersMet.Name = "LU_PlayersMet";
            this.LU_PlayersMet.Size = new System.Drawing.Size(161, 22);
            this.LU_PlayersMet.TabIndex = 3;
            this.LU_PlayersMet.Text = "Players Met";
            this.LU_PlayersMet.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // U_PlayersMet
            // 
            this.U_PlayersMet.Location = new System.Drawing.Point(179, 17);
            this.U_PlayersMet.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.U_PlayersMet.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.U_PlayersMet.Name = "U_PlayersMet";
            this.U_PlayersMet.Size = new System.Drawing.Size(95, 22);
            this.U_PlayersMet.TabIndex = 1;
            // 
            // LU_Gifts
            // 
            this.LU_Gifts.Location = new System.Drawing.Point(12, 45);
            this.LU_Gifts.Name = "LU_Gifts";
            this.LU_Gifts.Size = new System.Drawing.Size(161, 22);
            this.LU_Gifts.TabIndex = 5;
            this.LU_Gifts.Text = "Gifts Given";
            this.LU_Gifts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // U_Gifts
            // 
            this.U_Gifts.Location = new System.Drawing.Point(179, 45);
            this.U_Gifts.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.U_Gifts.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.U_Gifts.Name = "U_Gifts";
            this.U_Gifts.Size = new System.Drawing.Size(95, 22);
            this.U_Gifts.TabIndex = 2;
            // 
            // LU_Spheres
            // 
            this.LU_Spheres.Location = new System.Drawing.Point(12, 73);
            this.LU_Spheres.Name = "LU_Spheres";
            this.LU_Spheres.Size = new System.Drawing.Size(161, 22);
            this.LU_Spheres.TabIndex = 7;
            this.LU_Spheres.Text = "Spheres Obtained";
            this.LU_Spheres.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // U_Spheres
            // 
            this.U_Spheres.Location = new System.Drawing.Point(179, 73);
            this.U_Spheres.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.U_Spheres.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.U_Spheres.Name = "U_Spheres";
            this.U_Spheres.Size = new System.Drawing.Size(95, 22);
            this.U_Spheres.TabIndex = 3;
            // 
            // LU_Fossils
            // 
            this.LU_Fossils.Location = new System.Drawing.Point(12, 101);
            this.LU_Fossils.Name = "LU_Fossils";
            this.LU_Fossils.Size = new System.Drawing.Size(161, 22);
            this.LU_Fossils.TabIndex = 9;
            this.LU_Fossils.Text = "Fossils Obtained";
            this.LU_Fossils.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // U_Fossils
            // 
            this.U_Fossils.Location = new System.Drawing.Point(179, 101);
            this.U_Fossils.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.U_Fossils.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.U_Fossils.Name = "U_Fossils";
            this.U_Fossils.Size = new System.Drawing.Size(95, 22);
            this.U_Fossils.TabIndex = 4;
            // 
            // LU_TrapsA
            // 
            this.LU_TrapsA.Location = new System.Drawing.Point(12, 129);
            this.LU_TrapsA.Name = "LU_TrapsA";
            this.LU_TrapsA.Size = new System.Drawing.Size(161, 22);
            this.LU_TrapsA.TabIndex = 11;
            this.LU_TrapsA.Text = "Traps Avoided";
            this.LU_TrapsA.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // U_TrapsA
            // 
            this.U_TrapsA.Location = new System.Drawing.Point(179, 129);
            this.U_TrapsA.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.U_TrapsA.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.U_TrapsA.Name = "U_TrapsA";
            this.U_TrapsA.Size = new System.Drawing.Size(95, 22);
            this.U_TrapsA.TabIndex = 5;
            // 
            // LU_TrapsT
            // 
            this.LU_TrapsT.Location = new System.Drawing.Point(12, 157);
            this.LU_TrapsT.Name = "LU_TrapsT";
            this.LU_TrapsT.Size = new System.Drawing.Size(161, 22);
            this.LU_TrapsT.TabIndex = 13;
            this.LU_TrapsT.Text = "Traps Triggered";
            this.LU_TrapsT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // U_TrapsT
            // 
            this.U_TrapsT.Location = new System.Drawing.Point(179, 157);
            this.U_TrapsT.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.U_TrapsT.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.U_TrapsT.Name = "U_TrapsT";
            this.U_TrapsT.Size = new System.Drawing.Size(95, 22);
            this.U_TrapsT.TabIndex = 6;
            // 
            // LU_Flags
            // 
            this.LU_Flags.Location = new System.Drawing.Point(12, 184);
            this.LU_Flags.Name = "LU_Flags";
            this.LU_Flags.Size = new System.Drawing.Size(161, 22);
            this.LU_Flags.TabIndex = 15;
            this.LU_Flags.Text = "Flags Captured";
            this.LU_Flags.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // U_Flags
            // 
            this.U_Flags.Location = new System.Drawing.Point(179, 184);
            this.U_Flags.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.U_Flags.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.U_Flags.Name = "U_Flags";
            this.U_Flags.Size = new System.Drawing.Size(95, 22);
            this.U_Flags.TabIndex = 7;
            // 
            // SAV_Underground
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 261);
            this.Controls.Add(this.U_Flags);
            this.Controls.Add(this.LU_Flags);
            this.Controls.Add(this.U_TrapsT);
            this.Controls.Add(this.LU_TrapsT);
            this.Controls.Add(this.U_TrapsA);
            this.Controls.Add(this.LU_TrapsA);
            this.Controls.Add(this.U_Fossils);
            this.Controls.Add(this.LU_Fossils);
            this.Controls.Add(this.U_Spheres);
            this.Controls.Add(this.LU_Spheres);
            this.Controls.Add(this.U_Gifts);
            this.Controls.Add(this.LU_Gifts);
            this.Controls.Add(this.U_PlayersMet);
            this.Controls.Add(this.LU_PlayersMet);
            this.Controls.Add(this.B_Cancel);
            this.Controls.Add(this.B_Save);
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "SAV_Underground";
            this.Text = "Underground Scores Editor";
            ((System.ComponentModel.ISupportInitialize)(this.U_PlayersMet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.U_Gifts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.U_Spheres)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.U_Fossils)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.U_TrapsA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.U_TrapsT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.U_Flags)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Label LU_PlayersMet;
        private System.Windows.Forms.NumericUpDown U_PlayersMet;
        private System.Windows.Forms.Label LU_Gifts;
        private System.Windows.Forms.NumericUpDown U_Gifts;
        private System.Windows.Forms.Label LU_Spheres;
        private System.Windows.Forms.NumericUpDown U_Spheres;
        private System.Windows.Forms.Label LU_Fossils;
        private System.Windows.Forms.NumericUpDown U_Fossils;
        private System.Windows.Forms.Label LU_TrapsA;
        private System.Windows.Forms.NumericUpDown U_TrapsA;
        private System.Windows.Forms.Label LU_TrapsT;
        private System.Windows.Forms.NumericUpDown U_TrapsT;
        private System.Windows.Forms.Label LU_Flags;
        private System.Windows.Forms.NumericUpDown U_Flags;
    }
}