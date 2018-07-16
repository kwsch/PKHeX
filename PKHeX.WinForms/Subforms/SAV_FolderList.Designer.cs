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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SAV_FolderList));
            this.FLP_Buttons = new System.Windows.Forms.FlowLayoutPanel();
            this.TC_Tabs = new System.Windows.Forms.TabControl();
            this.Tab_Recent = new System.Windows.Forms.TabPage();
            this.dgDataRecent = new System.Windows.Forms.DataGridView();
            this.Tab_Backup = new System.Windows.Forms.TabPage();
            this.dgDataBackup = new System.Windows.Forms.DataGridView();
            this.Tab_Folders = new System.Windows.Forms.TabPage();
            this.TC_Tabs.SuspendLayout();
            this.Tab_Recent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgDataRecent)).BeginInit();
            this.Tab_Backup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgDataBackup)).BeginInit();
            this.Tab_Folders.SuspendLayout();
            this.SuspendLayout();
            //
            // FLP_Buttons
            //
            this.FLP_Buttons.AutoScroll = true;
            this.FLP_Buttons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FLP_Buttons.Location = new System.Drawing.Point(0, 0);
            this.FLP_Buttons.Name = "FLP_Buttons";
            this.FLP_Buttons.Padding = new System.Windows.Forms.Padding(3);
            this.FLP_Buttons.Size = new System.Drawing.Size(601, 335);
            this.FLP_Buttons.TabIndex = 0;
            //
            // TC_Tabs
            //
            this.TC_Tabs.Controls.Add(this.Tab_Recent);
            this.TC_Tabs.Controls.Add(this.Tab_Backup);
            this.TC_Tabs.Controls.Add(this.Tab_Folders);
            this.TC_Tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TC_Tabs.Location = new System.Drawing.Point(0, 0);
            this.TC_Tabs.Name = "TC_Tabs";
            this.TC_Tabs.SelectedIndex = 0;
            this.TC_Tabs.Size = new System.Drawing.Size(609, 361);
            this.TC_Tabs.TabIndex = 1;
            //
            // Tab_Recent
            //
            this.Tab_Recent.Controls.Add(this.dgDataRecent);
            this.Tab_Recent.Location = new System.Drawing.Point(4, 22);
            this.Tab_Recent.Name = "Tab_Recent";
            this.Tab_Recent.Size = new System.Drawing.Size(601, 335);
            this.Tab_Recent.TabIndex = 1;
            this.Tab_Recent.Text = "Recent";
            this.Tab_Recent.UseVisualStyleBackColor = true;
            //
            // dgDataRecent
            //
            this.dgDataRecent.AllowUserToAddRows = false;
            this.dgDataRecent.AllowUserToDeleteRows = false;
            this.dgDataRecent.AllowUserToOrderColumns = true;
            this.dgDataRecent.AllowUserToResizeColumns = false;
            this.dgDataRecent.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgDataRecent.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgDataRecent.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.dgDataRecent.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgDataRecent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgDataRecent.Location = new System.Drawing.Point(0, 0);
            this.dgDataRecent.Margin = new System.Windows.Forms.Padding(0);
            this.dgDataRecent.Name = "dgDataRecent";
            this.dgDataRecent.RowHeadersVisible = false;
            this.dgDataRecent.Size = new System.Drawing.Size(601, 335);
            this.dgDataRecent.TabIndex = 2;
            this.dgDataRecent.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridCellMouseDown);
            //
            // Tab_Backup
            //
            this.Tab_Backup.Controls.Add(this.dgDataBackup);
            this.Tab_Backup.Location = new System.Drawing.Point(4, 22);
            this.Tab_Backup.Name = "Tab_Backup";
            this.Tab_Backup.Size = new System.Drawing.Size(601, 335);
            this.Tab_Backup.TabIndex = 2;
            this.Tab_Backup.Text = "Backups";
            this.Tab_Backup.UseVisualStyleBackColor = true;
            //
            // dgDataBackup
            //
            this.dgDataBackup.AllowUserToAddRows = false;
            this.dgDataBackup.AllowUserToDeleteRows = false;
            this.dgDataBackup.AllowUserToOrderColumns = true;
            this.dgDataBackup.AllowUserToResizeColumns = false;
            this.dgDataBackup.AllowUserToResizeRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgDataBackup.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgDataBackup.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.dgDataBackup.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgDataBackup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgDataBackup.Location = new System.Drawing.Point(0, 0);
            this.dgDataBackup.Name = "dgDataBackup";
            this.dgDataBackup.RowHeadersVisible = false;
            this.dgDataBackup.Size = new System.Drawing.Size(601, 335);
            this.dgDataBackup.TabIndex = 1;
            this.dgDataBackup.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridCellMouseDown);
            //
            // Tab_Folders
            //
            this.Tab_Folders.Controls.Add(this.FLP_Buttons);
            this.Tab_Folders.Location = new System.Drawing.Point(4, 22);
            this.Tab_Folders.Name = "Tab_Folders";
            this.Tab_Folders.Size = new System.Drawing.Size(601, 335);
            this.Tab_Folders.TabIndex = 0;
            this.Tab_Folders.Text = "Folders";
            this.Tab_Folders.UseVisualStyleBackColor = true;
            //
            // SAV_FolderList
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 361);
            this.Controls.Add(this.TC_Tabs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SAV_FolderList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Folder List";
            this.TC_Tabs.ResumeLayout(false);
            this.Tab_Recent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgDataRecent)).EndInit();
            this.Tab_Backup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgDataBackup)).EndInit();
            this.Tab_Folders.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel FLP_Buttons;
        private System.Windows.Forms.TabControl TC_Tabs;
        private System.Windows.Forms.TabPage Tab_Folders;
        private System.Windows.Forms.TabPage Tab_Recent;
        private System.Windows.Forms.TabPage Tab_Backup;
        private System.Windows.Forms.DataGridView dgDataBackup;
        private System.Windows.Forms.DataGridView dgDataRecent;
    }
}