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
            B_Save = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            TB_SPDIV = new System.Windows.Forms.MaskedTextBox();
            TB_SPAIV = new System.Windows.Forms.MaskedTextBox();
            TB_DEFIV = new System.Windows.Forms.MaskedTextBox();
            TB_ATKIV = new System.Windows.Forms.MaskedTextBox();
            TB_HPIV = new System.Windows.Forms.MaskedTextBox();
            Label_HP = new System.Windows.Forms.Label();
            Label_ATK = new System.Windows.Forms.Label();
            Label_DEF = new System.Windows.Forms.Label();
            Label_SPA = new System.Windows.Forms.Label();
            Label_SPD = new System.Windows.Forms.Label();
            Label_SPE = new System.Windows.Forms.Label();
            TB_SPEIV = new System.Windows.Forms.MaskedTextBox();
            TB_PID = new System.Windows.Forms.TextBox();
            Label_PID = new System.Windows.Forms.Label();
            CB_Species = new System.Windows.Forms.ComboBox();
            Label_Species = new System.Windows.Forms.Label();
            CHK_Shiny = new System.Windows.Forms.CheckBox();
            CHK_Active = new System.Windows.Forms.CheckBox();
            NUD_Level = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)NUD_Level).BeginInit();
            SuspendLayout();
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(225, 125);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(88, 27);
            B_Save.TabIndex = 73;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(131, 125);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(88, 27);
            B_Cancel.TabIndex = 72;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // TB_SPDIV
            // 
            TB_SPDIV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_SPDIV.Location = new System.Drawing.Point(70, 103);
            TB_SPDIV.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            TB_SPDIV.Mask = "00";
            TB_SPDIV.Name = "TB_SPDIV";
            TB_SPDIV.Size = new System.Drawing.Size(25, 23);
            TB_SPDIV.TabIndex = 78;
            TB_SPDIV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TB_SPAIV
            // 
            TB_SPAIV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_SPAIV.Location = new System.Drawing.Point(70, 80);
            TB_SPAIV.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            TB_SPAIV.Mask = "00";
            TB_SPAIV.Name = "TB_SPAIV";
            TB_SPAIV.Size = new System.Drawing.Size(25, 23);
            TB_SPAIV.TabIndex = 77;
            TB_SPAIV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TB_DEFIV
            // 
            TB_DEFIV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_DEFIV.Location = new System.Drawing.Point(70, 57);
            TB_DEFIV.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            TB_DEFIV.Mask = "00";
            TB_DEFIV.Name = "TB_DEFIV";
            TB_DEFIV.Size = new System.Drawing.Size(25, 23);
            TB_DEFIV.TabIndex = 76;
            TB_DEFIV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TB_ATKIV
            // 
            TB_ATKIV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_ATKIV.Location = new System.Drawing.Point(70, 33);
            TB_ATKIV.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            TB_ATKIV.Mask = "00";
            TB_ATKIV.Name = "TB_ATKIV";
            TB_ATKIV.Size = new System.Drawing.Size(25, 23);
            TB_ATKIV.TabIndex = 75;
            TB_ATKIV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TB_HPIV
            // 
            TB_HPIV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_HPIV.Location = new System.Drawing.Point(70, 10);
            TB_HPIV.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            TB_HPIV.Mask = "00";
            TB_HPIV.Name = "TB_HPIV";
            TB_HPIV.Size = new System.Drawing.Size(25, 23);
            TB_HPIV.TabIndex = 74;
            TB_HPIV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Label_HP
            // 
            Label_HP.Location = new System.Drawing.Point(10, 10);
            Label_HP.Margin = new System.Windows.Forms.Padding(0);
            Label_HP.Name = "Label_HP";
            Label_HP.Size = new System.Drawing.Size(58, 24);
            Label_HP.TabIndex = 80;
            Label_HP.Text = "HP:";
            Label_HP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_ATK
            // 
            Label_ATK.Location = new System.Drawing.Point(10, 33);
            Label_ATK.Margin = new System.Windows.Forms.Padding(0);
            Label_ATK.Name = "Label_ATK";
            Label_ATK.Size = new System.Drawing.Size(58, 24);
            Label_ATK.TabIndex = 81;
            Label_ATK.Text = "Atk:";
            Label_ATK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_DEF
            // 
            Label_DEF.Location = new System.Drawing.Point(10, 57);
            Label_DEF.Margin = new System.Windows.Forms.Padding(0);
            Label_DEF.Name = "Label_DEF";
            Label_DEF.Size = new System.Drawing.Size(58, 24);
            Label_DEF.TabIndex = 82;
            Label_DEF.Text = "Def:";
            Label_DEF.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_SPA
            // 
            Label_SPA.Location = new System.Drawing.Point(10, 80);
            Label_SPA.Margin = new System.Windows.Forms.Padding(0);
            Label_SPA.Name = "Label_SPA";
            Label_SPA.Size = new System.Drawing.Size(58, 24);
            Label_SPA.TabIndex = 83;
            Label_SPA.Text = "SpA:";
            Label_SPA.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_SPD
            // 
            Label_SPD.Location = new System.Drawing.Point(10, 103);
            Label_SPD.Margin = new System.Windows.Forms.Padding(0);
            Label_SPD.Name = "Label_SPD";
            Label_SPD.Size = new System.Drawing.Size(58, 24);
            Label_SPD.TabIndex = 84;
            Label_SPD.Text = "SpD:";
            Label_SPD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_SPE
            // 
            Label_SPE.Location = new System.Drawing.Point(12, 126);
            Label_SPE.Margin = new System.Windows.Forms.Padding(0);
            Label_SPE.Name = "Label_SPE";
            Label_SPE.Size = new System.Drawing.Size(58, 24);
            Label_SPE.TabIndex = 85;
            Label_SPE.Text = "Spe:";
            Label_SPE.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TB_SPEIV
            // 
            TB_SPEIV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_SPEIV.Location = new System.Drawing.Point(70, 126);
            TB_SPEIV.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            TB_SPEIV.Mask = "00";
            TB_SPEIV.Name = "TB_SPEIV";
            TB_SPEIV.Size = new System.Drawing.Size(25, 23);
            TB_SPEIV.TabIndex = 79;
            TB_SPEIV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TB_PID
            // 
            TB_PID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TB_PID.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            TB_PID.Location = new System.Drawing.Point(161, 67);
            TB_PID.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
            TB_PID.MaxLength = 8;
            TB_PID.Name = "TB_PID";
            TB_PID.Size = new System.Drawing.Size(70, 20);
            TB_PID.TabIndex = 86;
            TB_PID.Text = "12345678";
            TB_PID.TextChanged += TB_PID_TextChanged;
            // 
            // Label_PID
            // 
            Label_PID.AutoSize = true;
            Label_PID.Location = new System.Drawing.Point(127, 69);
            Label_PID.Margin = new System.Windows.Forms.Padding(0, 6, 0, 5);
            Label_PID.Name = "Label_PID";
            Label_PID.Size = new System.Drawing.Size(28, 15);
            Label_PID.TabIndex = 87;
            Label_PID.Text = "PID:";
            Label_PID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_Species
            // 
            CB_Species.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Species.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Species.FormattingEnabled = true;
            CB_Species.Location = new System.Drawing.Point(131, 39);
            CB_Species.Margin = new System.Windows.Forms.Padding(0);
            CB_Species.Name = "CB_Species";
            CB_Species.Size = new System.Drawing.Size(146, 23);
            CB_Species.TabIndex = 88;
            // 
            // Label_Species
            // 
            Label_Species.AutoSize = true;
            Label_Species.Location = new System.Drawing.Point(127, 20);
            Label_Species.Margin = new System.Windows.Forms.Padding(0);
            Label_Species.Name = "Label_Species";
            Label_Species.Size = new System.Drawing.Size(49, 15);
            Label_Species.TabIndex = 89;
            Label_Species.Text = "Species:";
            Label_Species.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CHK_Shiny
            // 
            CHK_Shiny.AutoSize = true;
            CHK_Shiny.Enabled = false;
            CHK_Shiny.Location = new System.Drawing.Point(161, 93);
            CHK_Shiny.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_Shiny.Name = "CHK_Shiny";
            CHK_Shiny.Size = new System.Drawing.Size(60, 19);
            CHK_Shiny.TabIndex = 90;
            CHK_Shiny.Text = "Shiny?";
            CHK_Shiny.UseVisualStyleBackColor = true;
            // 
            // CHK_Active
            // 
            CHK_Active.Location = new System.Drawing.Point(234, 70);
            CHK_Active.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_Active.Name = "CHK_Active";
            CHK_Active.Size = new System.Drawing.Size(89, 35);
            CHK_Active.TabIndex = 91;
            CHK_Active.Text = "Roaming (Active)";
            CHK_Active.UseVisualStyleBackColor = true;
            // 
            // NUD_Level
            // 
            NUD_Level.Location = new System.Drawing.Point(225, 14);
            NUD_Level.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            NUD_Level.Name = "NUD_Level";
            NUD_Level.Size = new System.Drawing.Size(52, 23);
            NUD_Level.TabIndex = 92;
            // 
            // SAV_Roamer3
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(324, 165);
            Controls.Add(NUD_Level);
            Controls.Add(CHK_Active);
            Controls.Add(CHK_Shiny);
            Controls.Add(Label_Species);
            Controls.Add(CB_Species);
            Controls.Add(Label_PID);
            Controls.Add(TB_PID);
            Controls.Add(TB_SPEIV);
            Controls.Add(Label_SPE);
            Controls.Add(Label_SPD);
            Controls.Add(TB_SPDIV);
            Controls.Add(TB_SPAIV);
            Controls.Add(Label_SPA);
            Controls.Add(Label_DEF);
            Controls.Add(TB_DEFIV);
            Controls.Add(TB_ATKIV);
            Controls.Add(Label_ATK);
            Controls.Add(Label_HP);
            Controls.Add(TB_HPIV);
            Controls.Add(B_Save);
            Controls.Add(B_Cancel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(231, 167);
            Name = "SAV_Roamer3";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Roamer Editor";
            ((System.ComponentModel.ISupportInitialize)NUD_Level).EndInit();
            ResumeLayout(false);
            PerformLayout();
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
