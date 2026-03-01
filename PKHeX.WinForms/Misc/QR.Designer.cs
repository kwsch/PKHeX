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
            L_Box = new Label();
            L_Slot = new Label();
            NUD_Slot = new NumericUpDown();
            L_Copies = new Label();
            NUD_Copies = new NumericUpDown();
            B_Refresh = new Button();
            splitContainer1 = new SplitContainer();
            flowLayoutPanel1 = new FlowLayoutPanel();
            ((ISupportInitialize)PB_QR).BeginInit();
            ((ISupportInitialize)NUD_Box).BeginInit();
            ((ISupportInitialize)NUD_Slot).BeginInit();
            ((ISupportInitialize)NUD_Copies).BeginInit();
            ((ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // PB_QR
            // 
            PB_QR.BackgroundImageLayout = ImageLayout.None;
            PB_QR.Dock = DockStyle.Fill;
            PB_QR.Location = new System.Drawing.Point(0, 0);
            PB_QR.Margin = new Padding(0);
            PB_QR.Name = "PB_QR";
            PB_QR.Size = new System.Drawing.Size(404, 344);
            PB_QR.SizeMode = PictureBoxSizeMode.AutoSize;
            PB_QR.TabIndex = 0;
            PB_QR.TabStop = false;
            PB_QR.Click += PB_QR_Click;
            // 
            // NUD_Box
            // 
            NUD_Box.Location = new System.Drawing.Point(44, 6);
            NUD_Box.Margin = new Padding(0, 2, 8, 0);
            NUD_Box.Maximum = new decimal(new int[] { 32, 0, 0, 0 });
            NUD_Box.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NUD_Box.Name = "NUD_Box";
            NUD_Box.Size = new System.Drawing.Size(40, 25);
            NUD_Box.TabIndex = 2;
            NUD_Box.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // L_Box
            // 
            L_Box.AutoSize = true;
            L_Box.Location = new System.Drawing.Point(8, 8);
            L_Box.Margin = new Padding(4, 4, 4, 0);
            L_Box.Name = "L_Box";
            L_Box.Size = new System.Drawing.Size(32, 17);
            L_Box.TabIndex = 3;
            L_Box.Text = "Box:";
            // 
            // L_Slot
            // 
            L_Slot.AutoSize = true;
            L_Slot.Location = new System.Drawing.Point(96, 8);
            L_Slot.Margin = new Padding(4, 4, 4, 0);
            L_Slot.Name = "L_Slot";
            L_Slot.Size = new System.Drawing.Size(33, 17);
            L_Slot.TabIndex = 5;
            L_Slot.Text = "Slot:";
            // 
            // NUD_Slot
            // 
            NUD_Slot.Location = new System.Drawing.Point(133, 6);
            NUD_Slot.Margin = new Padding(0, 2, 8, 0);
            NUD_Slot.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            NUD_Slot.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NUD_Slot.Name = "NUD_Slot";
            NUD_Slot.Size = new System.Drawing.Size(40, 25);
            NUD_Slot.TabIndex = 4;
            NUD_Slot.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // L_Copies
            // 
            L_Copies.AutoSize = true;
            L_Copies.Location = new System.Drawing.Point(185, 8);
            L_Copies.Margin = new Padding(4, 4, 4, 0);
            L_Copies.Name = "L_Copies";
            L_Copies.Size = new System.Drawing.Size(51, 17);
            L_Copies.TabIndex = 7;
            L_Copies.Text = "Copies:";
            // 
            // NUD_Copies
            // 
            NUD_Copies.Location = new System.Drawing.Point(240, 6);
            NUD_Copies.Margin = new Padding(0, 2, 8, 0);
            NUD_Copies.Maximum = new decimal(new int[] { 960, 0, 0, 0 });
            NUD_Copies.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NUD_Copies.Name = "NUD_Copies";
            NUD_Copies.Size = new System.Drawing.Size(40, 25);
            NUD_Copies.TabIndex = 6;
            NUD_Copies.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // B_Refresh
            // 
            B_Refresh.Location = new System.Drawing.Point(290, 6);
            B_Refresh.Margin = new Padding(2);
            B_Refresh.Name = "B_Refresh";
            B_Refresh.Size = new System.Drawing.Size(93, 24);
            B_Refresh.TabIndex = 8;
            B_Refresh.Text = "Refresh";
            B_Refresh.UseVisualStyleBackColor = true;
            B_Refresh.Click += UpdateBoxSlotCopies;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Margin = new Padding(0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(flowLayoutPanel1);
            splitContainer1.Panel1MinSize = 34;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(PB_QR);
            splitContainer1.Size = new System.Drawing.Size(404, 381);
            splitContainer1.SplitterDistance = 36;
            splitContainer1.SplitterWidth = 1;
            splitContainer1.TabIndex = 9;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.Controls.Add(L_Box);
            flowLayoutPanel1.Controls.Add(NUD_Box);
            flowLayoutPanel1.Controls.Add(L_Slot);
            flowLayoutPanel1.Controls.Add(NUD_Slot);
            flowLayoutPanel1.Controls.Add(L_Copies);
            flowLayoutPanel1.Controls.Add(NUD_Copies);
            flowLayoutPanel1.Controls.Add(B_Refresh);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Padding = new Padding(4);
            flowLayoutPanel1.Size = new System.Drawing.Size(404, 36);
            flowLayoutPanel1.TabIndex = 9;
            // 
            // QR
            // 
            AutoScaleMode = AutoScaleMode.Inherit;
            AutoSize = true;
            ClientSize = new System.Drawing.Size(404, 381);
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
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox PB_QR;
        private NumericUpDown NUD_Box;
        private Label L_Box;
        private Label L_Slot;
        private NumericUpDown NUD_Slot;
        private Label L_Copies;
        private NumericUpDown NUD_Copies;
        private Button B_Refresh;
        private SplitContainer splitContainer1;
        private FlowLayoutPanel flowLayoutPanel1;
    }
}
