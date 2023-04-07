namespace PKHeX.WinForms
{
    partial class SAV_Misc8b
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
            this.B_Cancel = new System.Windows.Forms.Button();
            this.B_Save = new System.Windows.Forms.Button();
            this.TC_Misc = new System.Windows.Forms.TabControl();
            this.TAB_Main = new System.Windows.Forms.TabPage();
            this.B_Arceus = new System.Windows.Forms.Button();
            this.B_Zones = new System.Windows.Forms.Button();
            this.B_RebattleEyecatch = new System.Windows.Forms.Button();
            this.B_DefeatEyecatch = new System.Windows.Forms.Button();
            this.B_Roamer = new System.Windows.Forms.Button();
            this.B_DialgaPalkia = new System.Windows.Forms.Button();
            this.B_Darkrai = new System.Windows.Forms.Button();
            this.B_Shaymin = new System.Windows.Forms.Button();
            this.B_Spiritomb = new System.Windows.Forms.Button();
            this.B_Fashion = new System.Windows.Forms.Button();
            this.TC_Misc.SuspendLayout();
            this.TAB_Main.SuspendLayout();
            this.SuspendLayout();
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(194, 378);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(75, 25);
            this.B_Cancel.TabIndex = 1;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(275, 378);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(75, 25);
            this.B_Save.TabIndex = 2;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // TC_Misc
            // 
            this.TC_Misc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TC_Misc.Controls.Add(this.TAB_Main);
            this.TC_Misc.Location = new System.Drawing.Point(9, 9);
            this.TC_Misc.Margin = new System.Windows.Forms.Padding(0);
            this.TC_Misc.Name = "TC_Misc";
            this.TC_Misc.SelectedIndex = 0;
            this.TC_Misc.Size = new System.Drawing.Size(344, 363);
            this.TC_Misc.TabIndex = 0;
            // 
            // TAB_Main
            //
            this.TAB_Main.Controls.Add(this.B_Fashion);
            this.TAB_Main.Controls.Add(this.B_Arceus);
            this.TAB_Main.Controls.Add(this.B_Zones);
            this.TAB_Main.Controls.Add(this.B_RebattleEyecatch);
            this.TAB_Main.Controls.Add(this.B_DefeatEyecatch);
            this.TAB_Main.Controls.Add(this.B_Roamer);
            this.TAB_Main.Controls.Add(this.B_DialgaPalkia);
            this.TAB_Main.Controls.Add(this.B_Darkrai);
            this.TAB_Main.Controls.Add(this.B_Shaymin);
            this.TAB_Main.Controls.Add(this.B_Spiritomb);
            this.TAB_Main.Location = new System.Drawing.Point(4, 22);
            this.TAB_Main.Name = "TAB_Main";
            this.TAB_Main.Padding = new System.Windows.Forms.Padding(3);
            this.TAB_Main.Size = new System.Drawing.Size(336, 337);
            this.TAB_Main.TabIndex = 0;
            this.TAB_Main.Text = "Main";
            this.TAB_Main.UseVisualStyleBackColor = true;
            // 
            // B_Arceus
            // 
            this.B_Arceus.Location = new System.Drawing.Point(6, 198);
            this.B_Arceus.Name = "B_Arceus";
            this.B_Arceus.Size = new System.Drawing.Size(124, 58);
            this.B_Arceus.TabIndex = 8;
            this.B_Arceus.Text = "Unlock Arceus Event";
            this.B_Arceus.UseVisualStyleBackColor = true;
            this.B_Arceus.Click += new System.EventHandler(this.B_Arceus_Click);
            // 
            // B_Zones
            // 
            this.B_Zones.Location = new System.Drawing.Point(6, 262);
            this.B_Zones.Name = "B_Zones";
            this.B_Zones.Size = new System.Drawing.Size(124, 58);
            this.B_Zones.TabIndex = 7;
            this.B_Zones.Text = "Unlock All Zones";
            this.B_Zones.UseVisualStyleBackColor = true;
            this.B_Zones.Click += new System.EventHandler(this.B_Zones_Click);
            // 
            // B_RebattleEyecatch
            // 
            this.B_RebattleEyecatch.Location = new System.Drawing.Point(136, 198);
            this.B_RebattleEyecatch.Name = "B_RebattleEyecatch";
            this.B_RebattleEyecatch.Size = new System.Drawing.Size(124, 58);
            this.B_RebattleEyecatch.TabIndex = 6;
            this.B_RebattleEyecatch.Text = "Rebattle all Eyecatch Trainers";
            this.B_RebattleEyecatch.UseVisualStyleBackColor = true;
            this.B_RebattleEyecatch.Click += new System.EventHandler(this.B_RebattleEyecatch_Click);
            // 
            // B_DefeatEyecatch
            // 
            this.B_DefeatEyecatch.Location = new System.Drawing.Point(136, 134);
            this.B_DefeatEyecatch.Name = "B_DefeatEyecatch";
            this.B_DefeatEyecatch.Size = new System.Drawing.Size(124, 58);
            this.B_DefeatEyecatch.TabIndex = 5;
            this.B_DefeatEyecatch.Text = "Defeat all Eyecatch Trainers";
            this.B_DefeatEyecatch.UseVisualStyleBackColor = true;
            this.B_DefeatEyecatch.Click += new System.EventHandler(this.B_DefeatEyecatch_Click);
            // 
            // B_Roamer
            // 
            this.B_Roamer.Location = new System.Drawing.Point(136, 70);
            this.B_Roamer.Name = "B_Roamer";
            this.B_Roamer.Size = new System.Drawing.Size(124, 58);
            this.B_Roamer.TabIndex = 4;
            this.B_Roamer.Text = "Reset Roamers";
            this.B_Roamer.UseVisualStyleBackColor = true;
            this.B_Roamer.Click += new System.EventHandler(this.B_Roamer_Click);
            // 
            // B_DialgaPalkia
            // 
            this.B_DialgaPalkia.Location = new System.Drawing.Point(136, 6);
            this.B_DialgaPalkia.Name = "B_DialgaPalkia";
            this.B_DialgaPalkia.Size = new System.Drawing.Size(124, 58);
            this.B_DialgaPalkia.TabIndex = 3;
            this.B_DialgaPalkia.Text = "Reset Dialga/Palkia Encounter";
            this.B_DialgaPalkia.UseVisualStyleBackColor = true;
            this.B_DialgaPalkia.Click += new System.EventHandler(this.B_DialgaPalkia_Click);
            // 
            // B_Darkrai
            // 
            this.B_Darkrai.Location = new System.Drawing.Point(6, 134);
            this.B_Darkrai.Name = "B_Darkrai";
            this.B_Darkrai.Size = new System.Drawing.Size(124, 58);
            this.B_Darkrai.TabIndex = 2;
            this.B_Darkrai.Text = "Unlock Darkrai Event";
            this.B_Darkrai.UseVisualStyleBackColor = true;
            this.B_Darkrai.Click += new System.EventHandler(this.B_Darkrai_Click);
            // 
            // B_Shaymin
            // 
            this.B_Shaymin.Location = new System.Drawing.Point(6, 70);
            this.B_Shaymin.Name = "B_Shaymin";
            this.B_Shaymin.Size = new System.Drawing.Size(124, 58);
            this.B_Shaymin.TabIndex = 1;
            this.B_Shaymin.Text = "Unlock Shaymin Event";
            this.B_Shaymin.UseVisualStyleBackColor = true;
            this.B_Shaymin.Click += new System.EventHandler(this.B_Shaymin_Click);
            // 
            // B_Spiritomb
            // 
            this.B_Spiritomb.Location = new System.Drawing.Point(6, 6);
            this.B_Spiritomb.Name = "B_Spiritomb";
            this.B_Spiritomb.Size = new System.Drawing.Size(124, 58);
            this.B_Spiritomb.TabIndex = 0;
            this.B_Spiritomb.Text = "Greet all Underground NPCs (Spiritomb)";
            this.B_Spiritomb.UseVisualStyleBackColor = true;
            this.B_Spiritomb.Click += new System.EventHandler(this.B_Spiritomb_Click);
            // 
            // B_Fashion
            // 
            this.B_Fashion.Location = new System.Drawing.Point(136, 262);
            this.B_Fashion.Name = "B_Fashion";
            this.B_Fashion.Size = new System.Drawing.Size(124, 58);
            this.B_Fashion.TabIndex = 9;
            this.B_Fashion.Text = "Unlock All Fashion";
            this.B_Fashion.UseVisualStyleBackColor = true;
            this.B_Fashion.Click += new System.EventHandler(this.B_Fashion_Click);
            // 
            // SAV_Misc8b
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 412);
            this.Controls.Add(this.TC_Misc);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_Cancel);
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MinimumSize = new System.Drawing.Size(378, 411);
            this.Name = "SAV_Misc8b";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Misc Editor";
            this.TC_Misc.ResumeLayout(false);
            this.TAB_Main.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.TabControl TC_Misc;
        private System.Windows.Forms.TabPage TAB_Main;
        private System.Windows.Forms.Button B_Darkrai;
        private System.Windows.Forms.Button B_Shaymin;
        private System.Windows.Forms.Button B_Spiritomb;
        private System.Windows.Forms.Button B_DialgaPalkia;
        private System.Windows.Forms.Button B_Roamer;
        private System.Windows.Forms.Button B_DefeatEyecatch;
        private System.Windows.Forms.Button B_RebattleEyecatch;
        private System.Windows.Forms.Button B_Zones;
        private System.Windows.Forms.Button B_Arceus;
        private System.Windows.Forms.Button B_Fashion;
    }
}
