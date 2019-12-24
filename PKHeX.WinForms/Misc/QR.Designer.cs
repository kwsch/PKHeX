using System.ComponentModel;
using System.Windows.Forms;

namespace PKHeX.WinForms
{
    partial class QR
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            qr?.Dispose();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.PB_QR = new System.Windows.Forms.PictureBox();
            this.NUD_Box = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.NUD_Slot = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.NUD_Copies = new System.Windows.Forms.NumericUpDown();
            this.B_Refresh = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.PB_QR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Box)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Slot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Copies)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // PB_QR
            // 
            this.PB_QR.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.PB_QR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PB_QR.Location = new System.Drawing.Point(0, 0);
            this.PB_QR.Name = "PB_QR";
            this.PB_QR.Size = new System.Drawing.Size(284, 176);
            this.PB_QR.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.PB_QR.TabIndex = 0;
            this.PB_QR.TabStop = false;
            this.PB_QR.Click += new System.EventHandler(this.PB_QR_Click);
            // 
            // NUD_Box
            // 
            this.NUD_Box.Location = new System.Drawing.Point(37, 9);
            this.NUD_Box.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.NUD_Box.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUD_Box.Name = "NUD_Box";
            this.NUD_Box.Size = new System.Drawing.Size(61, 20);
            this.NUD_Box.TabIndex = 2;
            this.NUD_Box.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Box:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(104, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Slot:";
            // 
            // NUD_Slot
            // 
            this.NUD_Slot.Location = new System.Drawing.Point(138, 9);
            this.NUD_Slot.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.NUD_Slot.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUD_Slot.Name = "NUD_Slot";
            this.NUD_Slot.Size = new System.Drawing.Size(61, 20);
            this.NUD_Slot.TabIndex = 4;
            this.NUD_Slot.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(210, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Copies:";
            // 
            // NUD_Copies
            // 
            this.NUD_Copies.Location = new System.Drawing.Point(258, 9);
            this.NUD_Copies.Maximum = new decimal(new int[] {
            960,
            0,
            0,
            0});
            this.NUD_Copies.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUD_Copies.Name = "NUD_Copies";
            this.NUD_Copies.Size = new System.Drawing.Size(52, 20);
            this.NUD_Copies.TabIndex = 6;
            this.NUD_Copies.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // B_Refresh
            // 
            this.B_Refresh.Location = new System.Drawing.Point(316, 8);
            this.B_Refresh.Name = "B_Refresh";
            this.B_Refresh.Size = new System.Drawing.Size(80, 23);
            this.B_Refresh.TabIndex = 8;
            this.B_Refresh.Text = "Refresh";
            this.B_Refresh.UseVisualStyleBackColor = true;
            this.B_Refresh.Click += new System.EventHandler(this.UpdateBoxSlotCopies);
            // 
            // splitContainer1
            // 
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.NUD_Box);
            this.splitContainer1.Panel1.Controls.Add(this.B_Refresh);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.NUD_Slot);
            this.splitContainer1.Panel1.Controls.Add(this.NUD_Copies);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1MinSize = 34;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.PB_QR);
            this.splitContainer1.Size = new System.Drawing.Size(284, 211);
            this.splitContainer1.SplitterDistance = 34;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 9;
            // 
            // QR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(284, 211);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QR";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PKHeX QR Code (Click QR to Copy Image)";
            ((System.ComponentModel.ISupportInitialize)(this.PB_QR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Box)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Slot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Copies)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private PictureBox PB_QR;
        private NumericUpDown NUD_Box;
        private Label label1;
        private Label label2;
        private NumericUpDown NUD_Slot;
        private Label label3;
        private NumericUpDown NUD_Copies;
        private Button B_Refresh;
        private SplitContainer splitContainer1;
    }
}