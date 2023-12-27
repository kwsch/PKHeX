namespace PKHeX.WinForms
{
    partial class SAV_FolderList
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            FLP_Buttons = new System.Windows.Forms.FlowLayoutPanel();
            TC_Tabs = new System.Windows.Forms.TabControl();
            Tab_Recent = new System.Windows.Forms.TabPage();
            dgDataRecent = new PKHeX.WinForms.Controls.DoubleBufferedDataGridView();
            Tab_Backup = new System.Windows.Forms.TabPage();
            dgDataBackup = new PKHeX.WinForms.Controls.DoubleBufferedDataGridView();
            Tab_Folders = new System.Windows.Forms.TabPage();
            CB_FilterColumn = new System.Windows.Forms.ComboBox();
            TB_FilterTextContains = new System.Windows.Forms.TextBox();
            TC_Tabs.SuspendLayout();
            Tab_Recent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(dgDataRecent)).BeginInit();
            Tab_Backup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(dgDataBackup)).BeginInit();
            Tab_Folders.SuspendLayout();
            SuspendLayout();
            //
            // FLP_Buttons
            //
            FLP_Buttons.AutoScroll = true;
            FLP_Buttons.Dock = System.Windows.Forms.DockStyle.Fill;
            FLP_Buttons.Location = new System.Drawing.Point(0, 0);
            FLP_Buttons.Name = "FLP_Buttons";
            FLP_Buttons.Padding = new System.Windows.Forms.Padding(3);
            FLP_Buttons.Size = new System.Drawing.Size(601, 335);
            FLP_Buttons.TabIndex = 0;
            //
            // TC_Tabs
            //
            TC_Tabs.Controls.Add(Tab_Recent);
            TC_Tabs.Controls.Add(Tab_Backup);
            TC_Tabs.Controls.Add(Tab_Folders);
            TC_Tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            TC_Tabs.Location = new System.Drawing.Point(0, 0);
            TC_Tabs.Name = "TC_Tabs";
            TC_Tabs.SelectedIndex = 0;
            TC_Tabs.Size = new System.Drawing.Size(609, 361);
            TC_Tabs.TabIndex = 1;
            //
            // Tab_Recent
            //
            Tab_Recent.Controls.Add(dgDataRecent);
            Tab_Recent.Location = new System.Drawing.Point(4, 22);
            Tab_Recent.Name = "Tab_Recent";
            Tab_Recent.Size = new System.Drawing.Size(601, 335);
            Tab_Recent.TabIndex = 1;
            Tab_Recent.Text = "Recent";
            Tab_Recent.UseVisualStyleBackColor = true;
            //
            // dgDataRecent
            //
            dgDataRecent.AllowUserToAddRows = false;
            dgDataRecent.AllowUserToDeleteRows = false;
            dgDataRecent.AllowUserToOrderColumns = true;
            dgDataRecent.AllowUserToResizeColumns = false;
            dgDataRecent.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dgDataRecent.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgDataRecent.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            dgDataRecent.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgDataRecent.Dock = System.Windows.Forms.DockStyle.Fill;
            dgDataRecent.Location = new System.Drawing.Point(0, 0);
            dgDataRecent.Margin = new System.Windows.Forms.Padding(0);
            dgDataRecent.Name = "dgDataRecent";
            dgDataRecent.RowHeadersVisible = false;
            dgDataRecent.Size = new System.Drawing.Size(601, 335);
            dgDataRecent.TabIndex = 2;
            dgDataRecent.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(DataGridCellMouseDown);
            //
            // Tab_Backup
            //
            Tab_Backup.Controls.Add(dgDataBackup);
            Tab_Backup.Location = new System.Drawing.Point(4, 22);
            Tab_Backup.Name = "Tab_Backup";
            Tab_Backup.Size = new System.Drawing.Size(601, 335);
            Tab_Backup.TabIndex = 2;
            Tab_Backup.Text = "Backups";
            Tab_Backup.UseVisualStyleBackColor = true;
            //
            // dgDataBackup
            //
            dgDataBackup.AllowUserToAddRows = false;
            dgDataBackup.AllowUserToDeleteRows = false;
            dgDataBackup.AllowUserToOrderColumns = true;
            dgDataBackup.AllowUserToResizeColumns = false;
            dgDataBackup.AllowUserToResizeRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dgDataBackup.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            dgDataBackup.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            dgDataBackup.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgDataBackup.Dock = System.Windows.Forms.DockStyle.Fill;
            dgDataBackup.Location = new System.Drawing.Point(0, 0);
            dgDataBackup.Name = "dgDataBackup";
            dgDataBackup.RowHeadersVisible = false;
            dgDataBackup.Size = new System.Drawing.Size(601, 335);
            dgDataBackup.TabIndex = 1;
            dgDataBackup.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(DataGridCellMouseDown);
            //
            // Tab_Folders
            //
            Tab_Folders.Controls.Add(FLP_Buttons);
            Tab_Folders.Location = new System.Drawing.Point(4, 22);
            Tab_Folders.Name = "Tab_Folders";
            Tab_Folders.Size = new System.Drawing.Size(601, 335);
            Tab_Folders.TabIndex = 0;
            Tab_Folders.Text = "Folders";
            Tab_Folders.UseVisualStyleBackColor = true;
            //
            // CB_FilterColumn
            //
            CB_FilterColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_FilterColumn.Location = new System.Drawing.Point(327, 0);
            CB_FilterColumn.Name = "CB_FilterColumn";
            CB_FilterColumn.Size = new System.Drawing.Size(121, 21);
            CB_FilterColumn.TabIndex = 0;
            CB_FilterColumn.SelectedIndexChanged += new System.EventHandler(ChangeFilterIndex);
            //
            // textBox1
            //
            TB_FilterTextContains.Location = new System.Drawing.Point(449, 0);
            TB_FilterTextContains.Name = "TB_FilterTextContains";
            TB_FilterTextContains.Size = new System.Drawing.Size(160, 20);
            TB_FilterTextContains.TabIndex = 2;
            TB_FilterTextContains.TextChanged += new System.EventHandler(ChangeFilterText);
            //
            // SAV_FolderList
            //
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(609, 361);
            Controls.Add(TB_FilterTextContains);
            Controls.Add(CB_FilterColumn);
            Controls.Add(TC_Tabs);
            Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_FolderList";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Folder List";
            TC_Tabs.ResumeLayout(false);
            Tab_Recent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(dgDataRecent)).EndInit();
            Tab_Backup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(dgDataBackup)).EndInit();
            Tab_Folders.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel FLP_Buttons;
        private System.Windows.Forms.TabControl TC_Tabs;
        private System.Windows.Forms.TabPage Tab_Folders;
        private System.Windows.Forms.TabPage Tab_Recent;
        private System.Windows.Forms.TabPage Tab_Backup;
        private PKHeX.WinForms.Controls.DoubleBufferedDataGridView dgDataBackup;
        private PKHeX.WinForms.Controls.DoubleBufferedDataGridView dgDataRecent;
        private System.Windows.Forms.ComboBox CB_FilterColumn;
        private System.Windows.Forms.TextBox TB_FilterTextContains;
    }
}
