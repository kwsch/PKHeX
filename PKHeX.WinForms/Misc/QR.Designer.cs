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
            qr.Dispose();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            PB_QR = new PictureBox();
            NUD_Box = new NumericUpDown();
            label1 = new Label();
            label2 = new Label();
            NUD_Slot = new NumericUpDown();
            label3 = new Label();
            NUD_Copies = new NumericUpDown();
            B_Refresh = new Button();
            splitContainer1 = new SplitContainer();
            ((ISupportInitialize)PB_QR).BeginInit();
            ((ISupportInitialize)NUD_Box).BeginInit();
            ((ISupportInitialize)NUD_Slot).BeginInit();
            ((ISupportInitialize)NUD_Copies).BeginInit();
            ((ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // PB_QR
            // 
            PB_QR.BackgroundImageLayout = ImageLayout.None;
            PB_QR.Dock = DockStyle.Fill;
            PB_QR.Location = new System.Drawing.Point(0, 0);
            PB_QR.Margin = new Padding(4, 3, 4, 3);
            PB_QR.Name = "PB_QR";
            PB_QR.Size = new System.Drawing.Size(331, 203);
            PB_QR.SizeMode = PictureBoxSizeMode.AutoSize;
            PB_QR.TabIndex = 0;
            PB_QR.TabStop = false;
            PB_QR.Click += PB_QR_Click;
            // 
            // NUD_Box
            // 
            NUD_Box.Location = new System.Drawing.Point(43, 10);
            NUD_Box.Margin = new Padding(4, 3, 4, 3);
            NUD_Box.Maximum = new decimal(new int[] { 32, 0, 0, 0 });
            NUD_Box.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NUD_Box.Name = "NUD_Box";
            NUD_Box.Size = new System.Drawing.Size(71, 23);
            NUD_Box.TabIndex = 2;
            NUD_Box.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(4, 13);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(30, 15);
            label1.TabIndex = 3;
            label1.Text = "Box:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(121, 13);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(30, 15);
            label2.TabIndex = 5;
            label2.Text = "Slot:";
            // 
            // NUD_Slot
            // 
            NUD_Slot.Location = new System.Drawing.Point(161, 10);
            NUD_Slot.Margin = new Padding(4, 3, 4, 3);
            NUD_Slot.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            NUD_Slot.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NUD_Slot.Name = "NUD_Slot";
            NUD_Slot.Size = new System.Drawing.Size(71, 23);
            NUD_Slot.TabIndex = 4;
            NUD_Slot.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(245, 13);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(46, 15);
            label3.TabIndex = 7;
            label3.Text = "Copies:";
            // 
            // NUD_Copies
            // 
            NUD_Copies.Location = new System.Drawing.Point(301, 10);
            NUD_Copies.Margin = new Padding(4, 3, 4, 3);
            NUD_Copies.Maximum = new decimal(new int[] { 960, 0, 0, 0 });
            NUD_Copies.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NUD_Copies.Name = "NUD_Copies";
            NUD_Copies.Size = new System.Drawing.Size(61, 23);
            NUD_Copies.TabIndex = 6;
            NUD_Copies.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // B_Refresh
            // 
            B_Refresh.Location = new System.Drawing.Point(369, 9);
            B_Refresh.Margin = new Padding(4, 3, 4, 3);
            B_Refresh.Name = "B_Refresh";
            B_Refresh.Size = new System.Drawing.Size(93, 27);
            B_Refresh.TabIndex = 8;
            B_Refresh.Text = "Refresh";
            B_Refresh.UseVisualStyleBackColor = true;
            B_Refresh.Click += UpdateBoxSlotCopies;
            // 
            // splitContainer1
            // 
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Margin = new Padding(4, 3, 4, 3);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(NUD_Box);
            splitContainer1.Panel1.Controls.Add(B_Refresh);
            splitContainer1.Panel1.Controls.Add(label1);
            splitContainer1.Panel1.Controls.Add(label3);
            splitContainer1.Panel1.Controls.Add(NUD_Slot);
            splitContainer1.Panel1.Controls.Add(NUD_Copies);
            splitContainer1.Panel1.Controls.Add(label2);
            splitContainer1.Panel1MinSize = 34;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(PB_QR);
            splitContainer1.Size = new System.Drawing.Size(331, 243);
            splitContainer1.SplitterDistance = 39;
            splitContainer1.SplitterWidth = 1;
            splitContainer1.TabIndex = 9;
            // 
            // QR
            // 
            AutoScaleMode = AutoScaleMode.Inherit;
            AutoSize = true;
            ClientSize = new System.Drawing.Size(331, 243);
            Controls.Add(splitContainer1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "QR";
            StartPosition = FormStartPosition.CenterParent;
            Text = "PKHeX QR Code (Click QR to Copy Image)";
            ((ISupportInitialize)PB_QR).EndInit();
            ((ISupportInitialize)NUD_Box).EndInit();
            ((ISupportInitialize)NUD_Slot).EndInit();
            ((ISupportInitialize)NUD_Copies).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
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
