namespace PKHeX.WinForms
{
    partial class SAV_Roamer6
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
            L_Species = new System.Windows.Forms.Label();
            CB_Species = new System.Windows.Forms.ComboBox();
            L_TimesEncountered = new System.Windows.Forms.Label();
            NUD_TimesEncountered = new System.Windows.Forms.NumericUpDown();
            L_RoamState = new System.Windows.Forms.Label();
            CB_RoamState = new System.Windows.Forms.ComboBox();
            B_Cancel = new System.Windows.Forms.Button();
            B_Save = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)NUD_TimesEncountered).BeginInit();
            SuspendLayout();
            // 
            // L_Species
            // 
            L_Species.AutoSize = true;
            L_Species.Location = new System.Drawing.Point(15, 10);
            L_Species.Name = "L_Species";
            L_Species.Size = new System.Drawing.Size(48, 15);
            L_Species.TabIndex = 0;
            L_Species.Text = "Roamer";
            // 
            // CB_Species
            // 
            CB_Species.FormattingEnabled = true;
            CB_Species.Location = new System.Drawing.Point(15, 30);
            CB_Species.Name = "CB_Species";
            CB_Species.Size = new System.Drawing.Size(121, 23);
            CB_Species.TabIndex = 1;
            CB_Species.SelectedIndexChanged += CB_Species_SelectedIndexChanged;
            // 
            // L_TimesEncountered
            // 
            L_TimesEncountered.AutoSize = true;
            L_TimesEncountered.Location = new System.Drawing.Point(150, 10);
            L_TimesEncountered.Name = "L_TimesEncountered";
            L_TimesEncountered.Size = new System.Drawing.Size(108, 15);
            L_TimesEncountered.TabIndex = 2;
            L_TimesEncountered.Text = "Times Encountered";
            // 
            // NUD_TimesEncountered
            // 
            NUD_TimesEncountered.Location = new System.Drawing.Point(150, 30);
            NUD_TimesEncountered.Maximum = new decimal(new int[] { 11, 0, 0, 0 });
            NUD_TimesEncountered.Name = "NUD_TimesEncountered";
            NUD_TimesEncountered.Size = new System.Drawing.Size(120, 23);
            NUD_TimesEncountered.TabIndex = 3;
            NUD_TimesEncountered.ValueChanged += NUD_TimesEncountered_ValueChanged;
            // 
            // L_RoamState
            // 
            L_RoamState.AutoSize = true;
            L_RoamState.Location = new System.Drawing.Point(280, 10);
            L_RoamState.Name = "L_RoamState";
            L_RoamState.Size = new System.Drawing.Size(67, 15);
            L_RoamState.TabIndex = 4;
            L_RoamState.Text = "Roam State";
            // 
            // CB_RoamState
            // 
            CB_RoamState.FormattingEnabled = true;
            CB_RoamState.Location = new System.Drawing.Point(280, 30);
            CB_RoamState.Name = "CB_RoamState";
            CB_RoamState.Size = new System.Drawing.Size(121, 23);
            CB_RoamState.TabIndex = 5;
            CB_RoamState.SelectedIndexChanged += CB_RoamState_SelectedIndexChanged;
            // 
            // B_Cancel
            // 
            B_Cancel.Location = new System.Drawing.Point(245, 66);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(75, 23);
            B_Cancel.TabIndex = 6;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_Save
            // 
            B_Save.Location = new System.Drawing.Point(326, 66);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(75, 23);
            B_Save.TabIndex = 7;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // SAV_Roamer6
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(414, 101);
            Controls.Add(B_Save);
            Controls.Add(B_Cancel);
            Controls.Add(CB_RoamState);
            Controls.Add(L_RoamState);
            Controls.Add(NUD_TimesEncountered);
            Controls.Add(L_TimesEncountered);
            Controls.Add(CB_Species);
            Controls.Add(L_Species);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_Roamer6";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Roamer Editor";
            ((System.ComponentModel.ISupportInitialize)NUD_TimesEncountered).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label L_Species;
        private System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.Label L_TimesEncountered;
        private System.Windows.Forms.NumericUpDown NUD_TimesEncountered;
        private System.Windows.Forms.Label L_RoamState;
        private System.Windows.Forms.ComboBox CB_RoamState;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
    }
}
