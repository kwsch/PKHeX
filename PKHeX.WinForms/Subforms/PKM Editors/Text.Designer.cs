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
            this.TB_Text = new System.Windows.Forms.TextBox();
            this.CB_Species = new System.Windows.Forms.ComboBox();
            this.B_Cancel = new System.Windows.Forms.Button();
            this.B_Save = new System.Windows.Forms.Button();
            this.FLP_Characters = new System.Windows.Forms.FlowLayoutPanel();
            this.FLP_Hex = new System.Windows.Forms.FlowLayoutPanel();
            this.CB_Language = new System.Windows.Forms.ComboBox();
            this.B_ApplyTrash = new System.Windows.Forms.Button();
            this.GB_Trash = new System.Windows.Forms.GroupBox();
            this.B_ClearTrash = new System.Windows.Forms.Button();
            this.L_Generation = new System.Windows.Forms.Label();
            this.NUD_Generation = new System.Windows.Forms.NumericUpDown();
            this.L_Language = new System.Windows.Forms.Label();
            this.L_Species = new System.Windows.Forms.Label();
            this.L_String = new System.Windows.Forms.Label();
            this.GB_Trash.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Generation)).BeginInit();
            this.SuspendLayout();
            // 
            // TB_Text
            // 
            this.TB_Text.Location = new System.Drawing.Point(92, 12);
            this.TB_Text.Name = "TB_Text";
            this.TB_Text.Size = new System.Drawing.Size(136, 20);
            this.TB_Text.TabIndex = 35;
            // 
            // CB_Species
            // 
            this.CB_Species.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_Species.FormattingEnabled = true;
            this.CB_Species.Location = new System.Drawing.Point(76, 16);
            this.CB_Species.Name = "CB_Species";
            this.CB_Species.Size = new System.Drawing.Size(86, 21);
            this.CB_Species.TabIndex = 36;
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(333, 184);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(76, 23);
            this.B_Cancel.TabIndex = 37;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(415, 184);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(76, 23);
            this.B_Save.TabIndex = 38;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // FLP_Characters
            // 
            this.FLP_Characters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_Characters.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FLP_Characters.Location = new System.Drawing.Point(12, 126);
            this.FLP_Characters.Name = "FLP_Characters";
            this.FLP_Characters.Size = new System.Drawing.Size(305, 82);
            this.FLP_Characters.TabIndex = 39;
            // 
            // FLP_Hex
            // 
            this.FLP_Hex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_Hex.AutoScroll = true;
            this.FLP_Hex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FLP_Hex.Location = new System.Drawing.Point(12, 38);
            this.FLP_Hex.Name = "FLP_Hex";
            this.FLP_Hex.Size = new System.Drawing.Size(305, 82);
            this.FLP_Hex.TabIndex = 40;
            this.FLP_Hex.Visible = false;
            // 
            // CB_Language
            // 
            this.CB_Language.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_Language.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Language.FormattingEnabled = true;
            this.CB_Language.Location = new System.Drawing.Point(76, 43);
            this.CB_Language.Name = "CB_Language";
            this.CB_Language.Size = new System.Drawing.Size(86, 21);
            this.CB_Language.TabIndex = 41;
            // 
            // B_ApplyTrash
            // 
            this.B_ApplyTrash.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.B_ApplyTrash.Location = new System.Drawing.Point(6, 129);
            this.B_ApplyTrash.Name = "B_ApplyTrash";
            this.B_ApplyTrash.Size = new System.Drawing.Size(156, 23);
            this.B_ApplyTrash.TabIndex = 42;
            this.B_ApplyTrash.Text = "Apply Trash";
            this.B_ApplyTrash.UseVisualStyleBackColor = true;
            this.B_ApplyTrash.Click += new System.EventHandler(this.B_ApplyTrash_Click);
            // 
            // GB_Trash
            // 
            this.GB_Trash.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GB_Trash.Controls.Add(this.B_ClearTrash);
            this.GB_Trash.Controls.Add(this.L_Generation);
            this.GB_Trash.Controls.Add(this.NUD_Generation);
            this.GB_Trash.Controls.Add(this.L_Language);
            this.GB_Trash.Controls.Add(this.L_Species);
            this.GB_Trash.Controls.Add(this.CB_Species);
            this.GB_Trash.Controls.Add(this.B_ApplyTrash);
            this.GB_Trash.Controls.Add(this.CB_Language);
            this.GB_Trash.Location = new System.Drawing.Point(323, 12);
            this.GB_Trash.Name = "GB_Trash";
            this.GB_Trash.Size = new System.Drawing.Size(168, 158);
            this.GB_Trash.TabIndex = 43;
            this.GB_Trash.TabStop = false;
            this.GB_Trash.Text = "Trash Byte Layers";
            this.GB_Trash.Visible = false;
            // 
            // B_ClearTrash
            // 
            this.B_ClearTrash.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.B_ClearTrash.Location = new System.Drawing.Point(6, 100);
            this.B_ClearTrash.Name = "B_ClearTrash";
            this.B_ClearTrash.Size = new System.Drawing.Size(156, 23);
            this.B_ClearTrash.TabIndex = 47;
            this.B_ClearTrash.Text = "Clear Trash";
            this.B_ClearTrash.UseVisualStyleBackColor = true;
            this.B_ClearTrash.Click += new System.EventHandler(this.B_ClearTrash_Click);
            // 
            // L_Generation
            // 
            this.L_Generation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.L_Generation.Location = new System.Drawing.Point(6, 67);
            this.L_Generation.Name = "L_Generation";
            this.L_Generation.Size = new System.Drawing.Size(103, 23);
            this.L_Generation.TabIndex = 46;
            this.L_Generation.Text = "Generation";
            this.L_Generation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NUD_Generation
            // 
            this.NUD_Generation.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NUD_Generation.Location = new System.Drawing.Point(115, 70);
            this.NUD_Generation.Name = "NUD_Generation";
            this.NUD_Generation.Size = new System.Drawing.Size(47, 20);
            this.NUD_Generation.TabIndex = 45;
            // 
            // L_Language
            // 
            this.L_Language.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.L_Language.Location = new System.Drawing.Point(6, 43);
            this.L_Language.Name = "L_Language";
            this.L_Language.Size = new System.Drawing.Size(64, 23);
            this.L_Language.TabIndex = 44;
            this.L_Language.Text = "Language";
            this.L_Language.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_Species
            // 
            this.L_Species.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.L_Species.Location = new System.Drawing.Point(6, 16);
            this.L_Species.Name = "L_Species";
            this.L_Species.Size = new System.Drawing.Size(64, 23);
            this.L_Species.TabIndex = 43;
            this.L_Species.Text = "Species";
            this.L_Species.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // L_String
            // 
            this.L_String.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.L_String.Location = new System.Drawing.Point(9, 9);
            this.L_String.Name = "L_String";
            this.L_String.Size = new System.Drawing.Size(77, 23);
            this.L_String.TabIndex = 44;
            this.L_String.Text = "String";
            this.L_String.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TrashEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(503, 219);
            this.Controls.Add(this.L_String);
            this.Controls.Add(this.GB_Trash);
            this.Controls.Add(this.FLP_Hex);
            this.Controls.Add(this.FLP_Characters);
            this.Controls.Add(this.B_Cancel);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.TB_Text);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TrashEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Special Characters";
            this.GB_Trash.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Generation)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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