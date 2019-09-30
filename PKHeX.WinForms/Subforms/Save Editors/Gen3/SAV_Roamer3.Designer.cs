namespace PKHeX.WinForms
{
    partial class SAV_Roamer3
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
            this.B_Save = new System.Windows.Forms.Button();
            this.B_Cancel = new System.Windows.Forms.Button();
            this.TB_SPDIV = new System.Windows.Forms.MaskedTextBox();
            this.TB_SPAIV = new System.Windows.Forms.MaskedTextBox();
            this.TB_DEFIV = new System.Windows.Forms.MaskedTextBox();
            this.TB_ATKIV = new System.Windows.Forms.MaskedTextBox();
            this.TB_HPIV = new System.Windows.Forms.MaskedTextBox();
            this.Label_HP = new System.Windows.Forms.Label();
            this.Label_ATK = new System.Windows.Forms.Label();
            this.Label_DEF = new System.Windows.Forms.Label();
            this.Label_SPA = new System.Windows.Forms.Label();
            this.Label_SPD = new System.Windows.Forms.Label();
            this.Label_SPE = new System.Windows.Forms.Label();
            this.TB_SPEIV = new System.Windows.Forms.MaskedTextBox();
            this.TB_PID = new System.Windows.Forms.TextBox();
            this.Label_PID = new System.Windows.Forms.Label();
            this.CB_Species = new System.Windows.Forms.ComboBox();
            this.Label_Species = new System.Windows.Forms.Label();
            this.CHK_Shiny = new System.Windows.Forms.CheckBox();
            this.CHK_Active = new System.Windows.Forms.CheckBox();
            this.NUD_Level = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Level)).BeginInit();
            this.SuspendLayout();
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(193, 108);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(75, 23);
            this.B_Save.TabIndex = 73;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(112, 108);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(75, 23);
            this.B_Cancel.TabIndex = 72;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // TB_SPDIV
            // 
            this.TB_SPDIV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_SPDIV.Location = new System.Drawing.Point(60, 89);
            this.TB_SPDIV.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.TB_SPDIV.Mask = "00";
            this.TB_SPDIV.Name = "TB_SPDIV";
            this.TB_SPDIV.Size = new System.Drawing.Size(22, 20);
            this.TB_SPDIV.TabIndex = 78;
            this.TB_SPDIV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TB_SPAIV
            // 
            this.TB_SPAIV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_SPAIV.Location = new System.Drawing.Point(60, 69);
            this.TB_SPAIV.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.TB_SPAIV.Mask = "00";
            this.TB_SPAIV.Name = "TB_SPAIV";
            this.TB_SPAIV.Size = new System.Drawing.Size(22, 20);
            this.TB_SPAIV.TabIndex = 77;
            this.TB_SPAIV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TB_DEFIV
            // 
            this.TB_DEFIV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_DEFIV.Location = new System.Drawing.Point(60, 49);
            this.TB_DEFIV.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.TB_DEFIV.Mask = "00";
            this.TB_DEFIV.Name = "TB_DEFIV";
            this.TB_DEFIV.Size = new System.Drawing.Size(22, 20);
            this.TB_DEFIV.TabIndex = 76;
            this.TB_DEFIV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TB_ATKIV
            // 
            this.TB_ATKIV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_ATKIV.Location = new System.Drawing.Point(60, 29);
            this.TB_ATKIV.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.TB_ATKIV.Mask = "00";
            this.TB_ATKIV.Name = "TB_ATKIV";
            this.TB_ATKIV.Size = new System.Drawing.Size(22, 20);
            this.TB_ATKIV.TabIndex = 75;
            this.TB_ATKIV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TB_HPIV
            // 
            this.TB_HPIV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_HPIV.Location = new System.Drawing.Point(60, 9);
            this.TB_HPIV.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.TB_HPIV.Mask = "00";
            this.TB_HPIV.Name = "TB_HPIV";
            this.TB_HPIV.Size = new System.Drawing.Size(22, 20);
            this.TB_HPIV.TabIndex = 74;
            this.TB_HPIV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Label_HP
            // 
            this.Label_HP.Location = new System.Drawing.Point(9, 9);
            this.Label_HP.Margin = new System.Windows.Forms.Padding(0);
            this.Label_HP.Name = "Label_HP";
            this.Label_HP.Size = new System.Drawing.Size(50, 21);
            this.Label_HP.TabIndex = 80;
            this.Label_HP.Text = "HP:";
            this.Label_HP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_ATK
            // 
            this.Label_ATK.Location = new System.Drawing.Point(9, 29);
            this.Label_ATK.Margin = new System.Windows.Forms.Padding(0);
            this.Label_ATK.Name = "Label_ATK";
            this.Label_ATK.Size = new System.Drawing.Size(50, 21);
            this.Label_ATK.TabIndex = 81;
            this.Label_ATK.Text = "Atk:";
            this.Label_ATK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_DEF
            // 
            this.Label_DEF.Location = new System.Drawing.Point(9, 49);
            this.Label_DEF.Margin = new System.Windows.Forms.Padding(0);
            this.Label_DEF.Name = "Label_DEF";
            this.Label_DEF.Size = new System.Drawing.Size(50, 21);
            this.Label_DEF.TabIndex = 82;
            this.Label_DEF.Text = "Def:";
            this.Label_DEF.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_SPA
            // 
            this.Label_SPA.Location = new System.Drawing.Point(9, 69);
            this.Label_SPA.Margin = new System.Windows.Forms.Padding(0);
            this.Label_SPA.Name = "Label_SPA";
            this.Label_SPA.Size = new System.Drawing.Size(50, 21);
            this.Label_SPA.TabIndex = 83;
            this.Label_SPA.Text = "SpA:";
            this.Label_SPA.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_SPD
            // 
            this.Label_SPD.Location = new System.Drawing.Point(9, 89);
            this.Label_SPD.Margin = new System.Windows.Forms.Padding(0);
            this.Label_SPD.Name = "Label_SPD";
            this.Label_SPD.Size = new System.Drawing.Size(50, 21);
            this.Label_SPD.TabIndex = 84;
            this.Label_SPD.Text = "SpD:";
            this.Label_SPD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_SPE
            // 
            this.Label_SPE.Location = new System.Drawing.Point(10, 109);
            this.Label_SPE.Margin = new System.Windows.Forms.Padding(0);
            this.Label_SPE.Name = "Label_SPE";
            this.Label_SPE.Size = new System.Drawing.Size(50, 21);
            this.Label_SPE.TabIndex = 85;
            this.Label_SPE.Text = "Spe:";
            this.Label_SPE.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TB_SPEIV
            // 
            this.TB_SPEIV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_SPEIV.Location = new System.Drawing.Point(60, 109);
            this.TB_SPEIV.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.TB_SPEIV.Mask = "00";
            this.TB_SPEIV.Name = "TB_SPEIV";
            this.TB_SPEIV.Size = new System.Drawing.Size(22, 20);
            this.TB_SPEIV.TabIndex = 79;
            this.TB_SPEIV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TB_PID
            // 
            this.TB_PID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TB_PID.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_PID.Location = new System.Drawing.Point(138, 58);
            this.TB_PID.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.TB_PID.MaxLength = 8;
            this.TB_PID.Name = "TB_PID";
            this.TB_PID.Size = new System.Drawing.Size(60, 20);
            this.TB_PID.TabIndex = 86;
            this.TB_PID.Text = "12345678";
            this.TB_PID.TextChanged += new System.EventHandler(this.TB_PID_TextChanged);
            // 
            // Label_PID
            // 
            this.Label_PID.AutoSize = true;
            this.Label_PID.Location = new System.Drawing.Point(109, 60);
            this.Label_PID.Margin = new System.Windows.Forms.Padding(0, 5, 0, 4);
            this.Label_PID.Name = "Label_PID";
            this.Label_PID.Size = new System.Drawing.Size(28, 13);
            this.Label_PID.TabIndex = 87;
            this.Label_PID.Text = "PID:";
            this.Label_PID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_Species
            // 
            this.CB_Species.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_Species.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Species.FormattingEnabled = true;
            this.CB_Species.Location = new System.Drawing.Point(112, 34);
            this.CB_Species.Margin = new System.Windows.Forms.Padding(0);
            this.CB_Species.Name = "CB_Species";
            this.CB_Species.Size = new System.Drawing.Size(126, 21);
            this.CB_Species.TabIndex = 88;
            // 
            // Label_Species
            // 
            this.Label_Species.AutoSize = true;
            this.Label_Species.Location = new System.Drawing.Point(109, 17);
            this.Label_Species.Margin = new System.Windows.Forms.Padding(0);
            this.Label_Species.Name = "Label_Species";
            this.Label_Species.Size = new System.Drawing.Size(48, 13);
            this.Label_Species.TabIndex = 89;
            this.Label_Species.Text = "Species:";
            this.Label_Species.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CHK_Shiny
            // 
            this.CHK_Shiny.AutoSize = true;
            this.CHK_Shiny.Enabled = false;
            this.CHK_Shiny.Location = new System.Drawing.Point(138, 81);
            this.CHK_Shiny.Name = "CHK_Shiny";
            this.CHK_Shiny.Size = new System.Drawing.Size(58, 17);
            this.CHK_Shiny.TabIndex = 90;
            this.CHK_Shiny.Text = "Shiny?";
            this.CHK_Shiny.UseVisualStyleBackColor = true;
            // 
            // CHK_Active
            // 
            this.CHK_Active.Location = new System.Drawing.Point(201, 61);
            this.CHK_Active.Name = "CHK_Active";
            this.CHK_Active.Size = new System.Drawing.Size(76, 30);
            this.CHK_Active.TabIndex = 91;
            this.CHK_Active.Text = "Roaming (Active)";
            this.CHK_Active.UseVisualStyleBackColor = true;
            // 
            // NUD_Level
            // 
            this.NUD_Level.Location = new System.Drawing.Point(193, 12);
            this.NUD_Level.Name = "NUD_Level";
            this.NUD_Level.Size = new System.Drawing.Size(45, 20);
            this.NUD_Level.TabIndex = 92;
            // 
            // SAV_Roamer3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(278, 143);
            this.Controls.Add(this.NUD_Level);
            this.Controls.Add(this.CHK_Active);
            this.Controls.Add(this.CHK_Shiny);
            this.Controls.Add(this.Label_Species);
            this.Controls.Add(this.CB_Species);
            this.Controls.Add(this.Label_PID);
            this.Controls.Add(this.TB_PID);
            this.Controls.Add(this.TB_SPEIV);
            this.Controls.Add(this.Label_SPE);
            this.Controls.Add(this.Label_SPD);
            this.Controls.Add(this.TB_SPDIV);
            this.Controls.Add(this.TB_SPAIV);
            this.Controls.Add(this.Label_SPA);
            this.Controls.Add(this.Label_DEF);
            this.Controls.Add(this.TB_DEFIV);
            this.Controls.Add(this.TB_ATKIV);
            this.Controls.Add(this.Label_ATK);
            this.Controls.Add(this.Label_HP);
            this.Controls.Add(this.TB_HPIV);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_Cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(200, 150);
            this.Name = "SAV_Roamer3";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Roamer Editor";
            ((System.ComponentModel.ISupportInitialize)(this.NUD_Level)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.MaskedTextBox TB_SPDIV;
        private System.Windows.Forms.MaskedTextBox TB_SPAIV;
        private System.Windows.Forms.MaskedTextBox TB_DEFIV;
        private System.Windows.Forms.MaskedTextBox TB_ATKIV;
        private System.Windows.Forms.MaskedTextBox TB_HPIV;
        private System.Windows.Forms.Label Label_HP;
        private System.Windows.Forms.Label Label_ATK;
        private System.Windows.Forms.Label Label_DEF;
        private System.Windows.Forms.Label Label_SPA;
        private System.Windows.Forms.Label Label_SPD;
        private System.Windows.Forms.Label Label_SPE;
        private System.Windows.Forms.MaskedTextBox TB_SPEIV;
        private System.Windows.Forms.TextBox TB_PID;
        private System.Windows.Forms.Label Label_PID;
        public System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.Label Label_Species;
        private System.Windows.Forms.CheckBox CHK_Shiny;
        private System.Windows.Forms.CheckBox CHK_Active;
        private System.Windows.Forms.NumericUpDown NUD_Level;
    }
}
