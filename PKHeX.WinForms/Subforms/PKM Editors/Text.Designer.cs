namespace PKHeX.WinForms
{
    partial class TrashEditor
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
            TB_Text = new System.Windows.Forms.TextBox();
            CB_Species = new System.Windows.Forms.ComboBox();
            B_Cancel = new System.Windows.Forms.Button();
            B_Save = new System.Windows.Forms.Button();
            FLP_Characters = new System.Windows.Forms.FlowLayoutPanel();
            FLP_Hex = new System.Windows.Forms.FlowLayoutPanel();
            CB_Language = new System.Windows.Forms.ComboBox();
            B_ApplyTrash = new System.Windows.Forms.Button();
            GB_Trash = new System.Windows.Forms.GroupBox();
            B_ClearTrash = new System.Windows.Forms.Button();
            L_Generation = new System.Windows.Forms.Label();
            NUD_Generation = new System.Windows.Forms.NumericUpDown();
            L_Language = new System.Windows.Forms.Label();
            L_Species = new System.Windows.Forms.Label();
            L_String = new System.Windows.Forms.Label();
            GB_Trash.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Generation).BeginInit();
            SuspendLayout();
            // 
            // TB_Text
            // 
            TB_Text.Location = new System.Drawing.Point(114, 14);
            TB_Text.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TB_Text.Name = "TB_Text";
            TB_Text.Size = new System.Drawing.Size(158, 23);
            TB_Text.TabIndex = 35;
            // 
            // CB_Species
            // 
            CB_Species.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            CB_Species.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Species.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Species.FormattingEnabled = true;
            CB_Species.Location = new System.Drawing.Point(89, 18);
            CB_Species.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Species.Name = "CB_Species";
            CB_Species.Size = new System.Drawing.Size(100, 23);
            CB_Species.TabIndex = 36;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(388, 208);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(96, 32);
            B_Cancel.TabIndex = 37;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(490, 208);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(96, 32);
            B_Save.TabIndex = 38;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // FLP_Characters
            // 
            FLP_Characters.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            FLP_Characters.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            FLP_Characters.Location = new System.Drawing.Point(14, 145);
            FLP_Characters.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            FLP_Characters.Name = "FLP_Characters";
            FLP_Characters.Size = new System.Drawing.Size(368, 94);
            FLP_Characters.TabIndex = 39;
            // 
            // FLP_Hex
            // 
            FLP_Hex.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            FLP_Hex.AutoScroll = true;
            FLP_Hex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            FLP_Hex.Location = new System.Drawing.Point(14, 44);
            FLP_Hex.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            FLP_Hex.Name = "FLP_Hex";
            FLP_Hex.Size = new System.Drawing.Size(368, 94);
            FLP_Hex.TabIndex = 40;
            FLP_Hex.Visible = false;
            // 
            // CB_Language
            // 
            CB_Language.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            CB_Language.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Language.FormattingEnabled = true;
            CB_Language.Location = new System.Drawing.Point(89, 50);
            CB_Language.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Language.Name = "CB_Language";
            CB_Language.Size = new System.Drawing.Size(100, 23);
            CB_Language.TabIndex = 41;
            // 
            // B_ApplyTrash
            // 
            B_ApplyTrash.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            B_ApplyTrash.Location = new System.Drawing.Point(7, 153);
            B_ApplyTrash.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ApplyTrash.Name = "B_ApplyTrash";
            B_ApplyTrash.Size = new System.Drawing.Size(182, 27);
            B_ApplyTrash.TabIndex = 42;
            B_ApplyTrash.Text = "Apply Trash";
            B_ApplyTrash.UseVisualStyleBackColor = true;
            B_ApplyTrash.Click += B_ApplyTrash_Click;
            // 
            // GB_Trash
            // 
            GB_Trash.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            GB_Trash.Controls.Add(B_ClearTrash);
            GB_Trash.Controls.Add(L_Generation);
            GB_Trash.Controls.Add(NUD_Generation);
            GB_Trash.Controls.Add(L_Language);
            GB_Trash.Controls.Add(L_Species);
            GB_Trash.Controls.Add(CB_Species);
            GB_Trash.Controls.Add(B_ApplyTrash);
            GB_Trash.Controls.Add(CB_Language);
            GB_Trash.Location = new System.Drawing.Point(389, 14);
            GB_Trash.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Trash.Name = "GB_Trash";
            GB_Trash.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Trash.Size = new System.Drawing.Size(196, 188);
            GB_Trash.TabIndex = 43;
            GB_Trash.TabStop = false;
            GB_Trash.Text = "Trash Byte Layers";
            GB_Trash.Visible = false;
            // 
            // B_ClearTrash
            // 
            B_ClearTrash.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            B_ClearTrash.Location = new System.Drawing.Point(7, 119);
            B_ClearTrash.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ClearTrash.Name = "B_ClearTrash";
            B_ClearTrash.Size = new System.Drawing.Size(182, 27);
            B_ClearTrash.TabIndex = 47;
            B_ClearTrash.Text = "Clear Trash";
            B_ClearTrash.UseVisualStyleBackColor = true;
            B_ClearTrash.Click += B_ClearTrash_Click;
            // 
            // L_Generation
            // 
            L_Generation.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            L_Generation.Location = new System.Drawing.Point(7, 77);
            L_Generation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Generation.Name = "L_Generation";
            L_Generation.Size = new System.Drawing.Size(120, 27);
            L_Generation.TabIndex = 46;
            L_Generation.Text = "Generation";
            L_Generation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_Generation
            // 
            NUD_Generation.Font = new System.Drawing.Font("Courier New", 8.25F);
            NUD_Generation.Location = new System.Drawing.Point(134, 81);
            NUD_Generation.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_Generation.Name = "NUD_Generation";
            NUD_Generation.Size = new System.Drawing.Size(55, 20);
            NUD_Generation.TabIndex = 45;
            // 
            // L_Language
            // 
            L_Language.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            L_Language.Location = new System.Drawing.Point(7, 50);
            L_Language.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Language.Name = "L_Language";
            L_Language.Size = new System.Drawing.Size(75, 27);
            L_Language.TabIndex = 44;
            L_Language.Text = "Language";
            L_Language.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Species
            // 
            L_Species.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            L_Species.Location = new System.Drawing.Point(7, 18);
            L_Species.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Species.Name = "L_Species";
            L_Species.Size = new System.Drawing.Size(75, 27);
            L_Species.TabIndex = 43;
            L_Species.Text = "Species";
            L_Species.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_String
            // 
            L_String.Location = new System.Drawing.Point(10, 10);
            L_String.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_String.Name = "L_String";
            L_String.Size = new System.Drawing.Size(102, 27);
            L_String.TabIndex = 44;
            L_String.Text = "String";
            L_String.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TrashEditor
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(599, 253);
            Controls.Add(L_String);
            Controls.Add(GB_Trash);
            Controls.Add(FLP_Hex);
            Controls.Add(FLP_Characters);
            Controls.Add(B_Cancel);
            Controls.Add(B_Save);
            Controls.Add(TB_Text);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "TrashEditor";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Special Characters";
            GB_Trash.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)NUD_Generation).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.TextBox TB_Text;
        private System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.FlowLayoutPanel FLP_Characters;
        private System.Windows.Forms.FlowLayoutPanel FLP_Hex;
        private System.Windows.Forms.ComboBox CB_Language;
        private System.Windows.Forms.Button B_ApplyTrash;
        private System.Windows.Forms.GroupBox GB_Trash;
        private System.Windows.Forms.Label L_Species;
        private System.Windows.Forms.Label L_Language;
        private System.Windows.Forms.NumericUpDown NUD_Generation;
        private System.Windows.Forms.Label L_Generation;
        private System.Windows.Forms.Button B_ClearTrash;
        private System.Windows.Forms.Label L_String;
    }
}
