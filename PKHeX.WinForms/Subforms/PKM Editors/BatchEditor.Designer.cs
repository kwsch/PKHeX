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
            if (disposing)
            {
                _filterCountCancellation?.Cancel();
                _filterCountCancellation?.Dispose();
                if (components != null)
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
            B_Run = new System.Windows.Forms.Button();
            B_Reset = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            B_Save = new System.Windows.Forms.Button();
            B_Add = new System.Windows.Forms.Button();
            TLP_Bottom = new System.Windows.Forms.TableLayoutPanel();
            L_Count = new System.Windows.Forms.Label();
            FLP_RB.SuspendLayout();
            TLP_Bottom.SuspendLayout();
            SuspendLayout();
            // 
            // RB_Boxes
            // 
            RB_Boxes.Anchor = System.Windows.Forms.AnchorStyles.Left;
            RB_Boxes.Appearance = System.Windows.Forms.Appearance.Button;
            RB_Boxes.AutoSize = true;
            RB_Boxes.Checked = true;
            RB_Boxes.Location = new System.Drawing.Point(0, 0);
            RB_Boxes.Margin = new System.Windows.Forms.Padding(0);
            RB_Boxes.Name = "RB_Boxes";
            RB_Boxes.Size = new System.Drawing.Size(52, 27);
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
            RB_Path.Location = new System.Drawing.Point(99, 0);
            RB_Path.Margin = new System.Windows.Forms.Padding(0);
            RB_Path.Name = "RB_Path";
            RB_Path.Size = new System.Drawing.Size(64, 27);
            RB_Path.TabIndex = 2;
            RB_Path.Text = "Folder...";
            RB_Path.UseVisualStyleBackColor = true;
            RB_Path.Click += B_Open_Click;
            // 
            // FLP_RB
            // 
            TLP_Bottom.SetColumnSpan(FLP_RB, 5);
            FLP_RB.Controls.Add(RB_Boxes);
            FLP_RB.Controls.Add(RB_Party);
            FLP_RB.Controls.Add(RB_Path);
            FLP_RB.Controls.Add(TB_Folder);
            FLP_RB.Dock = System.Windows.Forms.DockStyle.Fill;
            FLP_RB.Location = new System.Drawing.Point(4, 4);
            FLP_RB.Margin = new System.Windows.Forms.Padding(4, 4, 4, 0);
            FLP_RB.Name = "FLP_RB";
            FLP_RB.Size = new System.Drawing.Size(626, 28);
            FLP_RB.TabIndex = 2;
            // 
            // RB_Party
            // 
            RB_Party.Anchor = System.Windows.Forms.AnchorStyles.Left;
            RB_Party.Appearance = System.Windows.Forms.Appearance.Button;
            RB_Party.AutoSize = true;
            RB_Party.Location = new System.Drawing.Point(52, 0);
            RB_Party.Margin = new System.Windows.Forms.Padding(0);
            RB_Party.Name = "RB_Party";
            RB_Party.Size = new System.Drawing.Size(47, 27);
            RB_Party.TabIndex = 1;
            RB_Party.Text = "Party";
            RB_Party.UseVisualStyleBackColor = true;
            RB_Party.Click += B_SAV_Click;
            // 
            // TB_Folder
            // 
            TB_Folder.Dock = System.Windows.Forms.DockStyle.Fill;
            TB_Folder.Location = new System.Drawing.Point(2, 29);
            TB_Folder.Margin = new System.Windows.Forms.Padding(2);
            TB_Folder.Name = "TB_Folder";
            TB_Folder.ReadOnly = true;
            TB_Folder.Size = new System.Drawing.Size(465, 25);
            TB_Folder.TabIndex = 3;
            TB_Folder.Visible = false;
            // 
            // RTB_Instructions
            // 
            RTB_Instructions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TLP_Bottom.SetColumnSpan(RTB_Instructions, 5);
            RTB_Instructions.Location = new System.Drawing.Point(4, 88);
            RTB_Instructions.Margin = new System.Windows.Forms.Padding(4, 0, 4, 4);
            RTB_Instructions.Name = "RTB_Instructions";
            RTB_Instructions.Size = new System.Drawing.Size(626, 219);
            RTB_Instructions.TabIndex = 5;
            RTB_Instructions.Text = "";
            RTB_Instructions.TextChanged += RTB_Instructions_TextChanged;
            // 
            // B_Run
            // 
            B_Run.Dock = System.Windows.Forms.DockStyle.Fill;
            B_Run.Location = new System.Drawing.Point(92, 315);
            B_Run.Margin = new System.Windows.Forms.Padding(4);
            B_Run.Name = "B_Run";
            B_Run.Size = new System.Drawing.Size(80, 32);
            B_Run.TabIndex = 9;
            B_Run.Text = "Run";
            B_Run.UseVisualStyleBackColor = true;
            B_Run.Click += B_Run_Click;
            // 
            // B_Reset
            // 
            B_Reset.Dock = System.Windows.Forms.DockStyle.Fill;
            B_Reset.Enabled = false;
            B_Reset.Location = new System.Drawing.Point(4, 315);
            B_Reset.Margin = new System.Windows.Forms.Padding(4);
            B_Reset.Name = "B_Reset";
            B_Reset.Size = new System.Drawing.Size(80, 32);
            B_Reset.TabIndex = 8;
            B_Reset.Text = "Reset";
            B_Reset.UseVisualStyleBackColor = true;
            B_Reset.Click += B_Reset_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.AutoSize = true;
            B_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            B_Cancel.Dock = System.Windows.Forms.DockStyle.Fill;
            B_Cancel.Location = new System.Drawing.Point(462, 315);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(80, 32);
            B_Cancel.TabIndex = 10;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_Save
            // 
            B_Save.AutoSize = true;
            B_Save.Dock = System.Windows.Forms.DockStyle.Fill;
            B_Save.Location = new System.Drawing.Point(550, 315);
            B_Save.Margin = new System.Windows.Forms.Padding(4);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(80, 32);
            B_Save.TabIndex = 11;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Add
            // 
            B_Add.Dock = System.Windows.Forms.DockStyle.Fill;
            B_Add.Location = new System.Drawing.Point(550, 36);
            B_Add.Margin = new System.Windows.Forms.Padding(4);
            B_Add.Name = "B_Add";
            B_Add.Size = new System.Drawing.Size(80, 48);
            B_Add.TabIndex = 4;
            B_Add.Text = "Add";
            B_Add.UseVisualStyleBackColor = true;
            B_Add.Click += B_Add_Click;
            // 
            // TLP_Bottom
            // 
            TLP_Bottom.ColumnCount = 5;
            TLP_Bottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 88F));
            TLP_Bottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 88F));
            TLP_Bottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_Bottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 88F));
            TLP_Bottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 88F));
            TLP_Bottom.Controls.Add(L_Count, 2, 3);
            TLP_Bottom.Controls.Add(B_Add, 4, 1);
            TLP_Bottom.Controls.Add(B_Save, 4, 3);
            TLP_Bottom.Controls.Add(FLP_RB, 0, 0);
            TLP_Bottom.Controls.Add(RTB_Instructions, 0, 2);
            TLP_Bottom.Controls.Add(B_Reset, 0, 3);
            TLP_Bottom.Controls.Add(B_Run, 1, 3);
            TLP_Bottom.Controls.Add(B_Cancel, 3, 3);
            TLP_Bottom.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Bottom.Location = new System.Drawing.Point(0, 0);
            TLP_Bottom.Margin = new System.Windows.Forms.Padding(4);
            TLP_Bottom.Name = "TLP_Bottom";
            TLP_Bottom.RowCount = 4;
            TLP_Bottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            TLP_Bottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            TLP_Bottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_Bottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            TLP_Bottom.Size = new System.Drawing.Size(634, 351);
            TLP_Bottom.TabIndex = 12;
            // 
            // L_Count
            // 
            L_Count.Anchor = System.Windows.Forms.AnchorStyles.None;
            L_Count.AutoSize = true;
            L_Count.Location = new System.Drawing.Point(261, 322);
            L_Count.Name = "L_Count";
            L_Count.Size = new System.Drawing.Size(112, 17);
            L_Count.TabIndex = 12;
            L_Count.Text = "Matching: {0} / {1}";
            // 
            // BatchEditor
            // 
            AllowDrop = true;
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            CancelButton = B_Cancel;
            ClientSize = new System.Drawing.Size(634, 351);
            Controls.Add(TLP_Bottom);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimumSize = new System.Drawing.Size(650, 390);
            Name = "BatchEditor";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Batch Editor";
            FormClosing += BatchEditor_FormClosing;
            DragDrop += TabMain_DragDrop;
            DragEnter += TabMain_DragEnter;
            FLP_RB.ResumeLayout(false);
            FLP_RB.PerformLayout();
            TLP_Bottom.ResumeLayout(false);
            TLP_Bottom.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.RadioButton RB_Boxes;
        private System.Windows.Forms.RadioButton RB_Path;
        private System.Windows.Forms.FlowLayoutPanel FLP_RB;
        private System.Windows.Forms.TextBox TB_Folder;
        private System.Windows.Forms.RichTextBox RTB_Instructions;
        private System.Windows.Forms.Button B_Run;
        private System.Windows.Forms.Button B_Reset;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Add;
        private System.Windows.Forms.RadioButton RB_Party;
        private System.Windows.Forms.TableLayoutPanel TLP_Bottom;
        private System.Windows.Forms.Label L_Count;
    }
}
