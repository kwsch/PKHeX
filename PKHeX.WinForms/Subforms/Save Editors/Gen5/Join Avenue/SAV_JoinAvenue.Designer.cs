namespace PKHeX.WinForms
{
    partial class SAV_JoinAvenue
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            TC_JoinAvenue = new System.Windows.Forms.TabControl();
            Tab_Settings = new System.Windows.Forms.TabPage();
            UC_Settings = new JoinAvenueSettingsEditor();
            TLP_SettingsTop = new System.Windows.Forms.TableLayoutPanel();
            CHK_ScriptFlag = new System.Windows.Forms.CheckBox();
            L_VisitorCount = new System.Windows.Forms.Label();
            NUD_VisitorCount = new System.Windows.Forms.NumericUpDown();
            L_FanCount = new System.Windows.Forms.Label();
            NUD_FanCount = new System.Windows.Forms.NumericUpDown();
            Tab_Visitors = new System.Windows.Forms.TabPage();
            P_Visitors = new System.Windows.Forms.Panel();
            Tab_Fans = new System.Windows.Forms.TabPage();
            P_Fans = new System.Windows.Forms.Panel();
            Tab_Occupants = new System.Windows.Forms.TabPage();
            P_Occupants = new System.Windows.Forms.Panel();
            Tab_Assistants = new System.Windows.Forms.TabPage();
            P_Assistants = new System.Windows.Forms.Panel();
            Tab_Self = new System.Windows.Forms.TabPage();
            TC_Self = new System.Windows.Forms.TabControl();
            Tab_SelfGeneral = new System.Windows.Forms.TabPage();
            UC_SelfGeneral = new JoinAvenueEntityGeneralEditor();
            Tab_SelfSpecific = new System.Windows.Forms.TabPage();
            UC_SelfSpecific = new JoinAvenueVisitorSpecificEditor();
            B_Cancel = new System.Windows.Forms.Button();
            B_Save = new System.Windows.Forms.Button();
            TC_JoinAvenue.SuspendLayout();
            Tab_Settings.SuspendLayout();
            TLP_SettingsTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_VisitorCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_FanCount).BeginInit();
            Tab_Visitors.SuspendLayout();
            Tab_Fans.SuspendLayout();
            Tab_Occupants.SuspendLayout();
            Tab_Assistants.SuspendLayout();
            Tab_Self.SuspendLayout();
            TC_Self.SuspendLayout();
            Tab_SelfGeneral.SuspendLayout();
            Tab_SelfSpecific.SuspendLayout();
            SuspendLayout();
            // 
            // TC_JoinAvenue
            // 
            TC_JoinAvenue.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TC_JoinAvenue.Controls.Add(Tab_Settings);
            TC_JoinAvenue.Controls.Add(Tab_Visitors);
            TC_JoinAvenue.Controls.Add(Tab_Fans);
            TC_JoinAvenue.Controls.Add(Tab_Occupants);
            TC_JoinAvenue.Controls.Add(Tab_Assistants);
            TC_JoinAvenue.Controls.Add(Tab_Self);
            TC_JoinAvenue.Location = new System.Drawing.Point(0, 0);
            TC_JoinAvenue.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TC_JoinAvenue.Name = "TC_JoinAvenue";
            TC_JoinAvenue.SelectedIndex = 0;
            TC_JoinAvenue.Size = new System.Drawing.Size(964, 687);
            TC_JoinAvenue.TabIndex = 0;
            // 
            // Tab_Settings
            // 
            Tab_Settings.Controls.Add(UC_Settings);
            Tab_Settings.Controls.Add(TLP_SettingsTop);
            Tab_Settings.Location = new System.Drawing.Point(4, 26);
            Tab_Settings.Name = "Tab_Settings";
            Tab_Settings.Padding = new System.Windows.Forms.Padding(3);
            Tab_Settings.Size = new System.Drawing.Size(956, 657);
            Tab_Settings.TabIndex = 0;
            Tab_Settings.Text = "Settings";
            Tab_Settings.UseVisualStyleBackColor = true;
            // 
            // UC_Settings
            // 
            UC_Settings.Dock = System.Windows.Forms.DockStyle.Fill;
            UC_Settings.Location = new System.Drawing.Point(3, 96);
            UC_Settings.Margin = new System.Windows.Forms.Padding(0);
            UC_Settings.Name = "UC_Settings";
            UC_Settings.Size = new System.Drawing.Size(950, 558);
            UC_Settings.TabIndex = 1;
            // 
            // TLP_SettingsTop
            // 
            TLP_SettingsTop.AutoSize = true;
            TLP_SettingsTop.ColumnCount = 2;
            TLP_SettingsTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_SettingsTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_SettingsTop.Controls.Add(CHK_ScriptFlag, 1, 0);
            TLP_SettingsTop.Controls.Add(L_VisitorCount, 0, 1);
            TLP_SettingsTop.Controls.Add(NUD_VisitorCount, 1, 1);
            TLP_SettingsTop.Controls.Add(L_FanCount, 0, 2);
            TLP_SettingsTop.Controls.Add(NUD_FanCount, 1, 2);
            TLP_SettingsTop.Dock = System.Windows.Forms.DockStyle.Top;
            TLP_SettingsTop.Location = new System.Drawing.Point(3, 3);
            TLP_SettingsTop.Name = "TLP_SettingsTop";
            TLP_SettingsTop.Padding = new System.Windows.Forms.Padding(8);
            TLP_SettingsTop.RowCount = 3;
            TLP_SettingsTop.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_SettingsTop.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_SettingsTop.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_SettingsTop.Size = new System.Drawing.Size(950, 93);
            TLP_SettingsTop.TabIndex = 0;
            // 
            // CHK_ScriptFlag
            // 
            CHK_ScriptFlag.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CHK_ScriptFlag.AutoSize = true;
            CHK_ScriptFlag.Location = new System.Drawing.Point(97, 11);
            CHK_ScriptFlag.Name = "CHK_ScriptFlag";
            CHK_ScriptFlag.Size = new System.Drawing.Size(88, 21);
            CHK_ScriptFlag.TabIndex = 1;
            CHK_ScriptFlag.Text = "Script Flag";
            CHK_ScriptFlag.UseVisualStyleBackColor = true;
            // 
            // L_VisitorCount
            // 
            L_VisitorCount.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_VisitorCount.AutoSize = true;
            L_VisitorCount.Location = new System.Drawing.Point(8, 39);
            L_VisitorCount.Margin = new System.Windows.Forms.Padding(0);
            L_VisitorCount.Name = "L_VisitorCount";
            L_VisitorCount.Size = new System.Drawing.Size(86, 17);
            L_VisitorCount.TabIndex = 2;
            L_VisitorCount.Text = "Visitor Count:";
            // 
            // NUD_VisitorCount
            // 
            NUD_VisitorCount.Location = new System.Drawing.Point(94, 35);
            NUD_VisitorCount.Margin = new System.Windows.Forms.Padding(0);
            NUD_VisitorCount.Maximum = new decimal(new int[] { -1, 0, 0, 0 });
            NUD_VisitorCount.Name = "NUD_VisitorCount";
            NUD_VisitorCount.Size = new System.Drawing.Size(120, 25);
            NUD_VisitorCount.TabIndex = 3;
            // 
            // L_FanCount
            // 
            L_FanCount.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_FanCount.AutoSize = true;
            L_FanCount.Location = new System.Drawing.Point(25, 64);
            L_FanCount.Margin = new System.Windows.Forms.Padding(0);
            L_FanCount.Name = "L_FanCount";
            L_FanCount.Size = new System.Drawing.Size(69, 17);
            L_FanCount.TabIndex = 4;
            L_FanCount.Text = "Fan Count:";
            // 
            // NUD_FanCount
            // 
            NUD_FanCount.Location = new System.Drawing.Point(94, 60);
            NUD_FanCount.Margin = new System.Windows.Forms.Padding(0);
            NUD_FanCount.Maximum = new decimal(new int[] { -1, 0, 0, 0 });
            NUD_FanCount.Name = "NUD_FanCount";
            NUD_FanCount.Size = new System.Drawing.Size(120, 25);
            NUD_FanCount.TabIndex = 5;
            // 
            // Tab_Visitors
            // 
            Tab_Visitors.Controls.Add(P_Visitors);
            Tab_Visitors.Location = new System.Drawing.Point(4, 26);
            Tab_Visitors.Name = "Tab_Visitors";
            Tab_Visitors.Padding = new System.Windows.Forms.Padding(3);
            Tab_Visitors.Size = new System.Drawing.Size(956, 657);
            Tab_Visitors.TabIndex = 1;
            Tab_Visitors.Text = "Visitors";
            Tab_Visitors.UseVisualStyleBackColor = true;
            // 
            // P_Visitors
            // 
            P_Visitors.Dock = System.Windows.Forms.DockStyle.Fill;
            P_Visitors.Location = new System.Drawing.Point(3, 3);
            P_Visitors.Name = "P_Visitors";
            P_Visitors.Size = new System.Drawing.Size(950, 651);
            P_Visitors.TabIndex = 0;
            // 
            // Tab_Fans
            // 
            Tab_Fans.Controls.Add(P_Fans);
            Tab_Fans.Location = new System.Drawing.Point(4, 26);
            Tab_Fans.Name = "Tab_Fans";
            Tab_Fans.Padding = new System.Windows.Forms.Padding(3);
            Tab_Fans.Size = new System.Drawing.Size(956, 657);
            Tab_Fans.TabIndex = 2;
            Tab_Fans.Text = "Fans";
            Tab_Fans.UseVisualStyleBackColor = true;
            // 
            // P_Fans
            // 
            P_Fans.Dock = System.Windows.Forms.DockStyle.Fill;
            P_Fans.Location = new System.Drawing.Point(3, 3);
            P_Fans.Name = "P_Fans";
            P_Fans.Size = new System.Drawing.Size(950, 651);
            P_Fans.TabIndex = 0;
            // 
            // Tab_Occupants
            // 
            Tab_Occupants.Controls.Add(P_Occupants);
            Tab_Occupants.Location = new System.Drawing.Point(4, 26);
            Tab_Occupants.Name = "Tab_Occupants";
            Tab_Occupants.Padding = new System.Windows.Forms.Padding(3);
            Tab_Occupants.Size = new System.Drawing.Size(956, 657);
            Tab_Occupants.TabIndex = 3;
            Tab_Occupants.Text = "Occupants";
            Tab_Occupants.UseVisualStyleBackColor = true;
            // 
            // P_Occupants
            // 
            P_Occupants.Dock = System.Windows.Forms.DockStyle.Fill;
            P_Occupants.Location = new System.Drawing.Point(3, 3);
            P_Occupants.Name = "P_Occupants";
            P_Occupants.Size = new System.Drawing.Size(950, 651);
            P_Occupants.TabIndex = 0;
            // 
            // Tab_Assistants
            // 
            Tab_Assistants.Controls.Add(P_Assistants);
            Tab_Assistants.Location = new System.Drawing.Point(4, 26);
            Tab_Assistants.Name = "Tab_Assistants";
            Tab_Assistants.Padding = new System.Windows.Forms.Padding(3);
            Tab_Assistants.Size = new System.Drawing.Size(956, 657);
            Tab_Assistants.TabIndex = 4;
            Tab_Assistants.Text = "Assistants";
            Tab_Assistants.UseVisualStyleBackColor = true;
            // 
            // P_Assistants
            // 
            P_Assistants.Dock = System.Windows.Forms.DockStyle.Fill;
            P_Assistants.Location = new System.Drawing.Point(3, 3);
            P_Assistants.Name = "P_Assistants";
            P_Assistants.Size = new System.Drawing.Size(950, 651);
            P_Assistants.TabIndex = 0;
            // 
            // Tab_Self
            // 
            Tab_Self.Controls.Add(TC_Self);
            Tab_Self.Location = new System.Drawing.Point(4, 26);
            Tab_Self.Name = "Tab_Self";
            Tab_Self.Padding = new System.Windows.Forms.Padding(3);
            Tab_Self.Size = new System.Drawing.Size(956, 657);
            Tab_Self.TabIndex = 5;
            Tab_Self.Text = "Self";
            Tab_Self.UseVisualStyleBackColor = true;
            // 
            // TC_Self
            // 
            TC_Self.Controls.Add(Tab_SelfGeneral);
            TC_Self.Controls.Add(Tab_SelfSpecific);
            TC_Self.Dock = System.Windows.Forms.DockStyle.Fill;
            TC_Self.Location = new System.Drawing.Point(3, 3);
            TC_Self.Name = "TC_Self";
            TC_Self.SelectedIndex = 0;
            TC_Self.Size = new System.Drawing.Size(950, 651);
            TC_Self.TabIndex = 0;
            // 
            // Tab_SelfGeneral
            // 
            Tab_SelfGeneral.Controls.Add(UC_SelfGeneral);
            Tab_SelfGeneral.Location = new System.Drawing.Point(4, 26);
            Tab_SelfGeneral.Name = "Tab_SelfGeneral";
            Tab_SelfGeneral.Size = new System.Drawing.Size(942, 621);
            Tab_SelfGeneral.TabIndex = 0;
            Tab_SelfGeneral.Text = "General";
            Tab_SelfGeneral.UseVisualStyleBackColor = true;
            // 
            // UC_SelfGeneral
            // 
            UC_SelfGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
            UC_SelfGeneral.Location = new System.Drawing.Point(0, 0);
            UC_SelfGeneral.Margin = new System.Windows.Forms.Padding(0);
            UC_SelfGeneral.Name = "UC_SelfGeneral";
            UC_SelfGeneral.Size = new System.Drawing.Size(942, 621);
            UC_SelfGeneral.TabIndex = 0;
            // 
            // Tab_SelfSpecific
            // 
            Tab_SelfSpecific.Controls.Add(UC_SelfSpecific);
            Tab_SelfSpecific.Location = new System.Drawing.Point(4, 26);
            Tab_SelfSpecific.Name = "Tab_SelfSpecific";
            Tab_SelfSpecific.Size = new System.Drawing.Size(942, 621);
            Tab_SelfSpecific.TabIndex = 1;
            Tab_SelfSpecific.Text = "Specific";
            Tab_SelfSpecific.UseVisualStyleBackColor = true;
            // 
            // UC_SelfSpecific
            // 
            UC_SelfSpecific.Dock = System.Windows.Forms.DockStyle.Fill;
            UC_SelfSpecific.Location = new System.Drawing.Point(0, 0);
            UC_SelfSpecific.Margin = new System.Windows.Forms.Padding(0);
            UC_SelfSpecific.Name = "UC_SelfSpecific";
            UC_SelfSpecific.Size = new System.Drawing.Size(942, 621);
            UC_SelfSpecific.TabIndex = 0;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(712, 690);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(120, 27);
            B_Cancel.TabIndex = 1;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(838, 690);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(120, 27);
            B_Save.TabIndex = 2;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // SAV_JoinAvenue
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(964, 725);
            Controls.Add(B_Save);
            Controls.Add(B_Cancel);
            Controls.Add(TC_JoinAvenue);
            Icon = Properties.Resources.Icon;
            MaximizeBox = false;
            MinimumSize = new System.Drawing.Size(980, 764);
            Name = "SAV_JoinAvenue";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Join Avenue";
            TC_JoinAvenue.ResumeLayout(false);
            Tab_Settings.ResumeLayout(false);
            Tab_Settings.PerformLayout();
            TLP_SettingsTop.ResumeLayout(false);
            TLP_SettingsTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_VisitorCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_FanCount).EndInit();
            Tab_Visitors.ResumeLayout(false);
            Tab_Fans.ResumeLayout(false);
            Tab_Occupants.ResumeLayout(false);
            Tab_Assistants.ResumeLayout(false);
            Tab_Self.ResumeLayout(false);
            TC_Self.ResumeLayout(false);
            Tab_SelfGeneral.ResumeLayout(false);
            Tab_SelfSpecific.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl TC_JoinAvenue;
        private System.Windows.Forms.TabPage Tab_Settings;
        private System.Windows.Forms.TabPage Tab_Visitors;
        private System.Windows.Forms.TabPage Tab_Fans;
        private System.Windows.Forms.TabPage Tab_Occupants;
        private System.Windows.Forms.TabPage Tab_Assistants;
        private System.Windows.Forms.TabPage Tab_Self;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.TableLayoutPanel TLP_SettingsTop;
        private System.Windows.Forms.CheckBox CHK_ScriptFlag;
        private System.Windows.Forms.Label L_VisitorCount;
        private System.Windows.Forms.NumericUpDown NUD_VisitorCount;
        private System.Windows.Forms.Label L_FanCount;
        private System.Windows.Forms.NumericUpDown NUD_FanCount;
        private JoinAvenueSettingsEditor UC_Settings;
        private System.Windows.Forms.Panel P_Visitors;
        private System.Windows.Forms.Panel P_Fans;
        private System.Windows.Forms.Panel P_Occupants;
        private System.Windows.Forms.Panel P_Assistants;
        private System.Windows.Forms.TabControl TC_Self;
        private System.Windows.Forms.TabPage Tab_SelfGeneral;
        private System.Windows.Forms.TabPage Tab_SelfSpecific;
        private JoinAvenueEntityGeneralEditor UC_SelfGeneral;
        private JoinAvenueVisitorSpecificEditor UC_SelfSpecific;
    }
}
