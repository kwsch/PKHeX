namespace PKHeX.WinForms
{
    partial class ZipArchiveEntrySelect
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
            labelInstruction = new System.Windows.Forms.Label();
            buttonCancel = new System.Windows.Forms.Button();
            buttonSelect = new System.Windows.Forms.Button();
            listViewArchiveEntries = new System.Windows.Forms.ListView();
            columnHeaderFilename = new System.Windows.Forms.ColumnHeader();
            columnHeaderSize = new System.Windows.Forms.ColumnHeader();
            labelSelectedEntry = new System.Windows.Forms.Label();
            textBoxSelectedEntry = new System.Windows.Forms.TextBox();
            SuspendLayout();
            // 
            // labelInstruction
            // 
            labelInstruction.AutoEllipsis = true;
            labelInstruction.AutoSize = true;
            labelInstruction.Location = new System.Drawing.Point(12, 9);
            labelInstruction.Name = "labelInstruction";
            labelInstruction.Size = new System.Drawing.Size(126, 15);
            labelInstruction.TabIndex = 0;
            labelInstruction.Text = "<select entry prompt>";
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonCancel.Location = new System.Drawing.Point(397, 276);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(75, 23);
            buttonCancel.TabIndex = 1;
            buttonCancel.Text = "&Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += ClickButtonCancel;
            // 
            // buttonSelect
            // 
            buttonSelect.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonSelect.Enabled = false;
            buttonSelect.Location = new System.Drawing.Point(316, 276);
            buttonSelect.Name = "buttonSelect";
            buttonSelect.Size = new System.Drawing.Size(75, 23);
            buttonSelect.TabIndex = 1;
            buttonSelect.Text = "&Select";
            buttonSelect.UseVisualStyleBackColor = true;
            buttonSelect.Click += ClickButtonSelect;
            // 
            // listViewArchiveEntries
            // 
            listViewArchiveEntries.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            listViewArchiveEntries.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeaderFilename, columnHeaderSize });
            listViewArchiveEntries.Location = new System.Drawing.Point(12, 27);
            listViewArchiveEntries.MultiSelect = false;
            listViewArchiveEntries.Name = "listViewArchiveEntries";
            listViewArchiveEntries.Size = new System.Drawing.Size(460, 243);
            listViewArchiveEntries.TabIndex = 2;
            listViewArchiveEntries.UseCompatibleStateImageBehavior = false;
            listViewArchiveEntries.View = System.Windows.Forms.View.Details;
            listViewArchiveEntries.ItemSelectionChanged += ItemSelectionChangedListViewArchiveEntries;
            listViewArchiveEntries.MouseDoubleClick += MouseDoubleClickListViewArchiveEntries;
            // 
            // columnHeaderFilename
            // 
            columnHeaderFilename.Text = "File name";
            columnHeaderFilename.Width = 360;
            // 
            // columnHeaderSize
            // 
            columnHeaderSize.Text = "Size";
            // 
            // labelSelectedEntry
            // 
            labelSelectedEntry.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            labelSelectedEntry.AutoSize = true;
            labelSelectedEntry.Location = new System.Drawing.Point(12, 280);
            labelSelectedEntry.Name = "labelSelectedEntry";
            labelSelectedEntry.Size = new System.Drawing.Size(84, 15);
            labelSelectedEntry.TabIndex = 3;
            labelSelectedEntry.Text = "Selected entry:";
            // 
            // textBoxSelectedEntry
            // 
            textBoxSelectedEntry.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxSelectedEntry.Location = new System.Drawing.Point(102, 276);
            textBoxSelectedEntry.Name = "textBoxSelectedEntry";
            textBoxSelectedEntry.ReadOnly = true;
            textBoxSelectedEntry.Size = new System.Drawing.Size(208, 23);
            textBoxSelectedEntry.TabIndex = 4;
            // 
            // ZipArchiveEntrySelect
            // 
            AcceptButton = buttonSelect;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new System.Drawing.Size(484, 311);
            Controls.Add(textBoxSelectedEntry);
            Controls.Add(labelSelectedEntry);
            Controls.Add(listViewArchiveEntries);
            Controls.Add(buttonSelect);
            Controls.Add(buttonCancel);
            Controls.Add(labelInstruction);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(500, 350);
            Name = "ZipArchiveEntrySelect";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Opening Zip Archive...";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label labelInstruction;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSelect;
        private System.Windows.Forms.ListView listViewArchiveEntries;
        private System.Windows.Forms.ColumnHeader columnHeaderFilename;
        private System.Windows.Forms.ColumnHeader columnHeaderSize;
        private System.Windows.Forms.Label labelSelectedEntry;
        private System.Windows.Forms.TextBox textBoxSelectedEntry;
    }
}
