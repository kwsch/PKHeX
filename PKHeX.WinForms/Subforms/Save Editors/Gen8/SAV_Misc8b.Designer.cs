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
            B_Cancel = new System.Windows.Forms.Button();
            B_Save = new System.Windows.Forms.Button();
            TC_Misc = new System.Windows.Forms.TabControl();
            TAB_Main = new System.Windows.Forms.TabPage();
            B_Fashion = new System.Windows.Forms.Button();
            B_Arceus = new System.Windows.Forms.Button();
            B_Zones = new System.Windows.Forms.Button();
            B_RebattleEyecatch = new System.Windows.Forms.Button();
            B_DefeatEyecatch = new System.Windows.Forms.Button();
            B_Roamer = new System.Windows.Forms.Button();
            B_DialgaPalkia = new System.Windows.Forms.Button();
            B_Darkrai = new System.Windows.Forms.Button();
            B_Shaymin = new System.Windows.Forms.Button();
            B_Spiritomb = new System.Windows.Forms.Button();
            TC_Misc.SuspendLayout();
            TAB_Main.SuspendLayout();
            SuspendLayout();
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(226, 436);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(88, 29);
            B_Cancel.TabIndex = 1;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(321, 436);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(88, 29);
            B_Save.TabIndex = 2;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // TC_Misc
            // 
            TC_Misc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TC_Misc.Controls.Add(TAB_Main);
            TC_Misc.Location = new System.Drawing.Point(10, 10);
            TC_Misc.Margin = new System.Windows.Forms.Padding(0);
            TC_Misc.Name = "TC_Misc";
            TC_Misc.SelectedIndex = 0;
            TC_Misc.Size = new System.Drawing.Size(401, 419);
            TC_Misc.TabIndex = 0;
            // 
            // TAB_Main
            // 
            TAB_Main.Controls.Add(B_Fashion);
            TAB_Main.Controls.Add(B_Arceus);
            TAB_Main.Controls.Add(B_Zones);
            TAB_Main.Controls.Add(B_RebattleEyecatch);
            TAB_Main.Controls.Add(B_DefeatEyecatch);
            TAB_Main.Controls.Add(B_Roamer);
            TAB_Main.Controls.Add(B_DialgaPalkia);
            TAB_Main.Controls.Add(B_Darkrai);
            TAB_Main.Controls.Add(B_Shaymin);
            TAB_Main.Controls.Add(B_Spiritomb);
            TAB_Main.Location = new System.Drawing.Point(4, 24);
            TAB_Main.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TAB_Main.Name = "TAB_Main";
            TAB_Main.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TAB_Main.Size = new System.Drawing.Size(393, 391);
            TAB_Main.TabIndex = 0;
            TAB_Main.Text = "Main";
            TAB_Main.UseVisualStyleBackColor = true;
            // 
            // B_Fashion
            // 
            B_Fashion.Location = new System.Drawing.Point(159, 302);
            B_Fashion.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Fashion.Name = "B_Fashion";
            B_Fashion.Size = new System.Drawing.Size(145, 67);
            B_Fashion.TabIndex = 9;
            B_Fashion.Text = "Unlock All Fashion";
            B_Fashion.UseVisualStyleBackColor = true;
            B_Fashion.Click += B_Fashion_Click;
            // 
            // B_Arceus
            // 
            B_Arceus.Location = new System.Drawing.Point(7, 228);
            B_Arceus.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Arceus.Name = "B_Arceus";
            B_Arceus.Size = new System.Drawing.Size(145, 67);
            B_Arceus.TabIndex = 8;
            B_Arceus.Text = "Unlock Arceus Event";
            B_Arceus.UseVisualStyleBackColor = true;
            B_Arceus.Click += B_Arceus_Click;
            // 
            // B_Zones
            // 
            B_Zones.Location = new System.Drawing.Point(7, 302);
            B_Zones.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Zones.Name = "B_Zones";
            B_Zones.Size = new System.Drawing.Size(145, 67);
            B_Zones.TabIndex = 7;
            B_Zones.Text = "Unlock All Zones";
            B_Zones.UseVisualStyleBackColor = true;
            B_Zones.Click += B_Zones_Click;
            // 
            // B_RebattleEyecatch
            // 
            B_RebattleEyecatch.Location = new System.Drawing.Point(159, 228);
            B_RebattleEyecatch.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_RebattleEyecatch.Name = "B_RebattleEyecatch";
            B_RebattleEyecatch.Size = new System.Drawing.Size(145, 67);
            B_RebattleEyecatch.TabIndex = 6;
            B_RebattleEyecatch.Text = "Rebattle all Eyecatch Trainers";
            B_RebattleEyecatch.UseVisualStyleBackColor = true;
            B_RebattleEyecatch.Click += B_RebattleEyecatch_Click;
            // 
            // B_DefeatEyecatch
            // 
            B_DefeatEyecatch.Location = new System.Drawing.Point(159, 155);
            B_DefeatEyecatch.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_DefeatEyecatch.Name = "B_DefeatEyecatch";
            B_DefeatEyecatch.Size = new System.Drawing.Size(145, 67);
            B_DefeatEyecatch.TabIndex = 5;
            B_DefeatEyecatch.Text = "Defeat all Eyecatch Trainers";
            B_DefeatEyecatch.UseVisualStyleBackColor = true;
            B_DefeatEyecatch.Click += B_DefeatEyecatch_Click;
            // 
            // B_Roamer
            // 
            B_Roamer.Location = new System.Drawing.Point(159, 81);
            B_Roamer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Roamer.Name = "B_Roamer";
            B_Roamer.Size = new System.Drawing.Size(145, 67);
            B_Roamer.TabIndex = 4;
            B_Roamer.Text = "Reset Roamers";
            B_Roamer.UseVisualStyleBackColor = true;
            B_Roamer.Click += B_Roamer_Click;
            // 
            // B_DialgaPalkia
            // 
            B_DialgaPalkia.Location = new System.Drawing.Point(159, 7);
            B_DialgaPalkia.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_DialgaPalkia.Name = "B_DialgaPalkia";
            B_DialgaPalkia.Size = new System.Drawing.Size(145, 67);
            B_DialgaPalkia.TabIndex = 3;
            B_DialgaPalkia.Text = "Reset Dialga/Palkia Encounter";
            B_DialgaPalkia.UseVisualStyleBackColor = true;
            B_DialgaPalkia.Click += B_DialgaPalkia_Click;
            // 
            // B_Darkrai
            // 
            B_Darkrai.Location = new System.Drawing.Point(7, 155);
            B_Darkrai.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Darkrai.Name = "B_Darkrai";
            B_Darkrai.Size = new System.Drawing.Size(145, 67);
            B_Darkrai.TabIndex = 2;
            B_Darkrai.Text = "Unlock Darkrai Event";
            B_Darkrai.UseVisualStyleBackColor = true;
            B_Darkrai.Click += B_Darkrai_Click;
            // 
            // B_Shaymin
            // 
            B_Shaymin.Location = new System.Drawing.Point(7, 81);
            B_Shaymin.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Shaymin.Name = "B_Shaymin";
            B_Shaymin.Size = new System.Drawing.Size(145, 67);
            B_Shaymin.TabIndex = 1;
            B_Shaymin.Text = "Unlock Shaymin Event";
            B_Shaymin.UseVisualStyleBackColor = true;
            B_Shaymin.Click += B_Shaymin_Click;
            // 
            // B_Spiritomb
            // 
            B_Spiritomb.Location = new System.Drawing.Point(7, 7);
            B_Spiritomb.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Spiritomb.Name = "B_Spiritomb";
            B_Spiritomb.Size = new System.Drawing.Size(145, 67);
            B_Spiritomb.TabIndex = 0;
            B_Spiritomb.Text = "Greet all Underground NPCs (Spiritomb)";
            B_Spiritomb.UseVisualStyleBackColor = true;
            B_Spiritomb.Click += B_Spiritomb_Click;
            // 
            // SAV_Misc8b
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(422, 475);
            Controls.Add(TC_Misc);
            Controls.Add(B_Save);
            Controls.Add(B_Cancel);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MinimumSize = new System.Drawing.Size(438, 468);
            Name = "SAV_Misc8b";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Misc Editor";
            TC_Misc.ResumeLayout(false);
            TAB_Main.ResumeLayout(false);
            ResumeLayout(false);
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
