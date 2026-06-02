namespace PKHeX.WinForms
{
    sealed partial class SaveHandlerTroubleshooter
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
            L_Path = new System.Windows.Forms.Label();
            TB_Path = new System.Windows.Forms.TextBox();
            B_Browse = new System.Windows.Forms.Button();
            L_Type = new System.Windows.Forms.Label();
            CB_Type = new System.Windows.Forms.ComboBox();
            L_SubVersion = new System.Windows.Forms.Label();
            CB_SubVersion = new System.Windows.Forms.ComboBox();
            L_Language = new System.Windows.Forms.Label();
            CB_Language = new System.Windows.Forms.ComboBox();
            L_Handler = new System.Windows.Forms.Label();
            CB_Handler = new System.Windows.Forms.ComboBox();
            B_Continue = new System.Windows.Forms.Button();
            TLP_Main = new System.Windows.Forms.TableLayoutPanel();
            TLP_Main.SuspendLayout();
            SuspendLayout();
            // 
            // L_Path
            // 
            L_Path.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Path.AutoSize = true;
            L_Path.Location = new System.Drawing.Point(55, 16);
            L_Path.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            L_Path.Name = "L_Path";
            L_Path.Size = new System.Drawing.Size(36, 17);
            L_Path.TabIndex = 0;
            L_Path.Text = "Path:";
            // 
            // TB_Path
            // 
            TB_Path.AccessibleDescription = "Path to the save file to load with the selected handler.";
            TB_Path.AccessibleName = "Save Path";
            TB_Path.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TB_Path.Location = new System.Drawing.Point(97, 3);
            TB_Path.Margin = new System.Windows.Forms.Padding(3, 3, 3, 16);
            TB_Path.Name = "TB_Path";
            TB_Path.ReadOnly = true;
            TB_Path.Size = new System.Drawing.Size(409, 25);
            TB_Path.TabIndex = 1;
            // 
            // B_Browse
            // 
            B_Browse.AccessibleDescription = "Selects the save file to load.";
            B_Browse.AccessibleName = "Browse Save File";
            B_Browse.AutoSize = true;
            B_Browse.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            B_Browse.Location = new System.Drawing.Point(512, 3);
            B_Browse.Name = "B_Browse";
            B_Browse.Size = new System.Drawing.Size(69, 27);
            B_Browse.TabIndex = 2;
            B_Browse.Text = "Browse...";
            B_Browse.UseVisualStyleBackColor = true;
            B_Browse.Click += B_Browse_Click;
            // 
            // L_Type
            // 
            L_Type.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Type.AutoSize = true;
            L_Type.Location = new System.Drawing.Point(3, 53);
            L_Type.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            L_Type.Name = "L_Type";
            L_Type.Size = new System.Drawing.Size(88, 17);
            L_Type.TabIndex = 0;
            L_Type.Text = "Save file type:";
            // 
            // CB_Type
            // 
            CB_Type.AccessibleDescription = "Selects the target save file type.";
            CB_Type.AccessibleName = "Save File Type";
            CB_Type.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            CB_Type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Type.FormattingEnabled = true;
            CB_Type.Location = new System.Drawing.Point(97, 47);
            CB_Type.Name = "CB_Type";
            CB_Type.Size = new System.Drawing.Size(409, 25);
            CB_Type.TabIndex = 1;
            // 
            // L_SubVersion
            // 
            L_SubVersion.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_SubVersion.AutoSize = true;
            L_SubVersion.Location = new System.Drawing.Point(12, 84);
            L_SubVersion.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            L_SubVersion.Name = "L_SubVersion";
            L_SubVersion.Size = new System.Drawing.Size(79, 17);
            L_SubVersion.TabIndex = 0;
            L_SubVersion.Text = "Sub version:";
            // 
            // CB_SubVersion
            // 
            CB_SubVersion.AccessibleDescription = "Selects the specific game version within the save file type.";
            CB_SubVersion.AccessibleName = "Save Sub Version";
            CB_SubVersion.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            CB_SubVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_SubVersion.FormattingEnabled = true;
            CB_SubVersion.Location = new System.Drawing.Point(97, 78);
            CB_SubVersion.Name = "CB_SubVersion";
            CB_SubVersion.Size = new System.Drawing.Size(409, 25);
            CB_SubVersion.TabIndex = 1;
            // 
            // L_Language
            // 
            L_Language.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Language.AutoSize = true;
            L_Language.Location = new System.Drawing.Point(23, 115);
            L_Language.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            L_Language.Name = "L_Language";
            L_Language.Size = new System.Drawing.Size(68, 17);
            L_Language.TabIndex = 0;
            L_Language.Text = "Language:";
            // 
            // CB_Language
            // 
            CB_Language.AccessibleDescription = "Selects the save language passed to save loading.";
            CB_Language.AccessibleName = "Save Language";
            CB_Language.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            CB_Language.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Language.FormattingEnabled = true;
            CB_Language.Location = new System.Drawing.Point(97, 109);
            CB_Language.Name = "CB_Language";
            CB_Language.Size = new System.Drawing.Size(409, 25);
            CB_Language.TabIndex = 1;
            // 
            // L_Handler
            // 
            L_Handler.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Handler.AutoSize = true;
            L_Handler.Location = new System.Drawing.Point(34, 146);
            L_Handler.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            L_Handler.Name = "L_Handler";
            L_Handler.Size = new System.Drawing.Size(57, 17);
            L_Handler.TabIndex = 0;
            L_Handler.Text = "Handler:";
            // 
            // CB_Handler
            // 
            CB_Handler.AccessibleDescription = "Selects the preprocessing save handler.";
            CB_Handler.AccessibleName = "Save Handler";
            CB_Handler.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            CB_Handler.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Handler.FormattingEnabled = true;
            CB_Handler.Location = new System.Drawing.Point(97, 140);
            CB_Handler.Name = "CB_Handler";
            CB_Handler.Size = new System.Drawing.Size(409, 25);
            CB_Handler.TabIndex = 1;
            // 
            // B_Continue
            // 
            B_Continue.AccessibleDescription = "Attempts to load the selected file with the chosen save handler settings.";
            B_Continue.AccessibleName = "Continue Load";
            B_Continue.AutoSize = true;
            B_Continue.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            B_Continue.Location = new System.Drawing.Point(512, 171);
            B_Continue.Name = "B_Continue";
            B_Continue.Size = new System.Drawing.Size(69, 27);
            B_Continue.TabIndex = 0;
            B_Continue.Text = "Continue";
            B_Continue.UseVisualStyleBackColor = true;
            B_Continue.Click += B_Continue_Click;
            // 
            // TLP_Main
            // 
            TLP_Main.ColumnCount = 3;
            TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Main.Controls.Add(B_Continue, 2, 5);
            TLP_Main.Controls.Add(CB_Handler, 1, 4);
            TLP_Main.Controls.Add(L_Handler, 0, 4);
            TLP_Main.Controls.Add(CB_Language, 1, 3);
            TLP_Main.Controls.Add(L_Language, 0, 3);
            TLP_Main.Controls.Add(CB_SubVersion, 1, 2);
            TLP_Main.Controls.Add(L_SubVersion, 0, 2);
            TLP_Main.Controls.Add(L_Type, 0, 1);
            TLP_Main.Controls.Add(B_Browse, 2, 0);
            TLP_Main.Controls.Add(CB_Type, 1, 1);
            TLP_Main.Controls.Add(TB_Path, 1, 0);
            TLP_Main.Controls.Add(L_Path, 0, 0);
            TLP_Main.AllowDrop = true;
            TLP_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Main.Location = new System.Drawing.Point(0, 0);
            TLP_Main.Name = "TLP_Main";
            TLP_Main.RowCount = 6;
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_Main.Size = new System.Drawing.Size(584, 201);
            TLP_Main.TabIndex = 6;
            TLP_Main.DragDrop += SaveHandlerTroubleshooter_DragDrop;
            TLP_Main.DragEnter += SaveHandlerTroubleshooter_DragEnter;
            // 
            // SaveHandlerTroubleshooter
            // 
            AllowDrop = true;
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(584, 201);
            Controls.Add(TLP_Main);
            DragDrop += SaveHandlerTroubleshooter_DragDrop;
            DragEnter += SaveHandlerTroubleshooter_DragEnter;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SaveHandlerTroubleshooter";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Save Handler Troubleshooter";
            TLP_Main.ResumeLayout(false);
            TLP_Main.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Label L_Path;
        private System.Windows.Forms.TextBox TB_Path;
        private System.Windows.Forms.Button B_Browse;
        private System.Windows.Forms.Label L_Type;
        private System.Windows.Forms.ComboBox CB_Type;
        private System.Windows.Forms.Label L_SubVersion;
        private System.Windows.Forms.ComboBox CB_SubVersion;
        private System.Windows.Forms.Label L_Language;
        private System.Windows.Forms.ComboBox CB_Language;
        private System.Windows.Forms.Label L_Handler;
        private System.Windows.Forms.ComboBox CB_Handler;
        private System.Windows.Forms.Button B_Continue;
        private System.Windows.Forms.TableLayoutPanel TLP_Main;
    }
}
