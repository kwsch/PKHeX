namespace PKHeX.WinForms
{
    partial class BatchEditor
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
            RB_Boxes = new System.Windows.Forms.RadioButton();
            RB_Path = new System.Windows.Forms.RadioButton();
            FLP_RB = new System.Windows.Forms.FlowLayoutPanel();
            RB_Party = new System.Windows.Forms.RadioButton();
            TB_Folder = new System.Windows.Forms.TextBox();
            RTB_Instructions = new System.Windows.Forms.RichTextBox();
            B_Go = new System.Windows.Forms.Button();
            PB_Show = new System.Windows.Forms.ProgressBar();
            B_Add = new System.Windows.Forms.Button();
            b = new System.ComponentModel.BackgroundWorker();
            FLP_RB.SuspendLayout();
            SuspendLayout();
            // 
            // RB_Boxes
            // 
            RB_Boxes.Anchor = System.Windows.Forms.AnchorStyles.Left;
            RB_Boxes.Appearance = System.Windows.Forms.Appearance.Button;
            RB_Boxes.AutoSize = true;
            RB_Boxes.Checked = true;
            RB_Boxes.Location = new System.Drawing.Point(0, 1);
            RB_Boxes.Margin = new System.Windows.Forms.Padding(0);
            RB_Boxes.Name = "RB_Boxes";
            RB_Boxes.Size = new System.Drawing.Size(48, 25);
            RB_Boxes.TabIndex = 0;
            RB_Boxes.TabStop = true;
            RB_Boxes.Text = "Boxes";
            RB_Boxes.UseVisualStyleBackColor = true;
            RB_Boxes.Click += B_SAV_Click;
            // 
            // RB_Path
            // 
            RB_Path.Anchor = System.Windows.Forms.AnchorStyles.Left;
            RB_Path.Appearance = System.Windows.Forms.Appearance.Button;
            RB_Path.AutoSize = true;
            RB_Path.Location = new System.Drawing.Point(92, 1);
            RB_Path.Margin = new System.Windows.Forms.Padding(0);
            RB_Path.Name = "RB_Path";
            RB_Path.Size = new System.Drawing.Size(59, 25);
            RB_Path.TabIndex = 1;
            RB_Path.Text = "Folder...";
            RB_Path.UseVisualStyleBackColor = true;
            RB_Path.Click += B_Open_Click;
            // 
            // FLP_RB
            // 
            FLP_RB.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            FLP_RB.Controls.Add(RB_Boxes);
            FLP_RB.Controls.Add(RB_Party);
            FLP_RB.Controls.Add(RB_Path);
            FLP_RB.Controls.Add(TB_Folder);
            FLP_RB.Location = new System.Drawing.Point(14, 12);
            FLP_RB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            FLP_RB.Name = "FLP_RB";
            FLP_RB.Size = new System.Drawing.Size(432, 28);
            FLP_RB.TabIndex = 2;
            // 
            // RB_Party
            // 
            RB_Party.Anchor = System.Windows.Forms.AnchorStyles.Left;
            RB_Party.Appearance = System.Windows.Forms.Appearance.Button;
            RB_Party.AutoSize = true;
            RB_Party.Location = new System.Drawing.Point(48, 1);
            RB_Party.Margin = new System.Windows.Forms.Padding(0);
            RB_Party.Name = "RB_Party";
            RB_Party.Size = new System.Drawing.Size(44, 25);
            RB_Party.TabIndex = 5;
            RB_Party.Text = "Party";
            RB_Party.UseVisualStyleBackColor = true;
            // 
            // TB_Folder
            // 
            TB_Folder.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TB_Folder.Location = new System.Drawing.Point(153, 2);
            TB_Folder.Margin = new System.Windows.Forms.Padding(2);
            TB_Folder.Name = "TB_Folder";
            TB_Folder.ReadOnly = true;
            TB_Folder.Size = new System.Drawing.Size(251, 23);
            TB_Folder.TabIndex = 4;
            TB_Folder.Visible = false;
            // 
            // RTB_Instructions
            // 
            RTB_Instructions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            RTB_Instructions.Location = new System.Drawing.Point(14, 97);
            RTB_Instructions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RTB_Instructions.Name = "RTB_Instructions";
            RTB_Instructions.Size = new System.Drawing.Size(431, 162);
            RTB_Instructions.TabIndex = 5;
            RTB_Instructions.Text = "";
            // 
            // B_Go
            // 
            B_Go.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Go.Location = new System.Drawing.Point(379, 265);
            B_Go.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Go.Name = "B_Go";
            B_Go.Size = new System.Drawing.Size(66, 27);
            B_Go.TabIndex = 6;
            B_Go.Text = "Run";
            B_Go.UseVisualStyleBackColor = true;
            B_Go.Click += B_Go_Click;
            // 
            // PB_Show
            // 
            PB_Show.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            PB_Show.Location = new System.Drawing.Point(14, 267);
            PB_Show.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PB_Show.Name = "PB_Show";
            PB_Show.Size = new System.Drawing.Size(358, 24);
            PB_Show.TabIndex = 7;
            // 
            // B_Add
            // 
            B_Add.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            B_Add.Location = new System.Drawing.Point(378, 45);
            B_Add.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Add.Name = "B_Add";
            B_Add.Size = new System.Drawing.Size(66, 27);
            B_Add.TabIndex = 11;
            B_Add.Text = "Add";
            B_Add.UseVisualStyleBackColor = true;
            B_Add.Click += B_Add_Click;
            // 
            // b
            // 
            b.WorkerReportsProgress = true;
            // 
            // BatchEditor
            // 
            AllowDrop = true;
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(460, 301);
            Controls.Add(B_Add);
            Controls.Add(PB_Show);
            Controls.Add(B_Go);
            Controls.Add(RTB_Instructions);
            Controls.Add(FLP_RB);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimumSize = new System.Drawing.Size(476, 340);
            Name = "BatchEditor";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Batch Editor";
            FLP_RB.ResumeLayout(false);
            FLP_RB.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.RadioButton RB_Boxes;
        private System.Windows.Forms.RadioButton RB_Path;
        private System.Windows.Forms.FlowLayoutPanel FLP_RB;
        private System.Windows.Forms.TextBox TB_Folder;
        private System.Windows.Forms.RichTextBox RTB_Instructions;
        private System.Windows.Forms.Button B_Go;
        private System.Windows.Forms.ProgressBar PB_Show;
        private System.Windows.Forms.Button B_Add;
        private System.Windows.Forms.RadioButton RB_Party;
        private System.ComponentModel.BackgroundWorker b;
    }
}
