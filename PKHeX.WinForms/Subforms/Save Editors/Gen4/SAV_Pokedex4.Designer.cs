namespace PKHeX.WinForms
{
    partial class SAV_Pokedex4
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
            components = new System.ComponentModel.Container();
            B_Cancel = new System.Windows.Forms.Button();
            LB_Species = new System.Windows.Forms.ListBox();
            CHK_Caught = new System.Windows.Forms.CheckBox();
            CHK_L6 = new System.Windows.Forms.CheckBox();
            CHK_L5 = new System.Windows.Forms.CheckBox();
            CHK_L4 = new System.Windows.Forms.CheckBox();
            CHK_L3 = new System.Windows.Forms.CheckBox();
            CHK_L2 = new System.Windows.Forms.CheckBox();
            CHK_L1 = new System.Windows.Forms.CheckBox();
            L_goto = new System.Windows.Forms.Label();
            CB_Species = new System.Windows.Forms.ComboBox();
            B_GiveAll = new System.Windows.Forms.Button();
            B_Save = new System.Windows.Forms.Button();
            B_Modify = new System.Windows.Forms.Button();
            GB_Language = new System.Windows.Forms.GroupBox();
            modifyMenu = new System.Windows.Forms.ContextMenuStrip(components);
            mnuSeenNone = new System.Windows.Forms.ToolStripMenuItem();
            mnuSeenAll = new System.Windows.Forms.ToolStripMenuItem();
            mnuCaughtNone = new System.Windows.Forms.ToolStripMenuItem();
            mnuCaughtAll = new System.Windows.Forms.ToolStripMenuItem();
            mnuComplete = new System.Windows.Forms.ToolStripMenuItem();
            mnuUpgraded = new System.Windows.Forms.ToolStripMenuItem();
            CB_DexUpgraded = new System.Windows.Forms.ToolStripComboBox();
            CHK_Seen = new System.Windows.Forms.CheckBox();
            LB_Gender = new System.Windows.Forms.ListBox();
            B_GUp = new System.Windows.Forms.Button();
            B_GDown = new System.Windows.Forms.Button();
            B_GRight = new System.Windows.Forms.Button();
            B_GLeft = new System.Windows.Forms.Button();
            LB_NGender = new System.Windows.Forms.ListBox();
            LB_NForm = new System.Windows.Forms.ListBox();
            B_FRight = new System.Windows.Forms.Button();
            B_FLeft = new System.Windows.Forms.Button();
            B_FDown = new System.Windows.Forms.Button();
            B_FUp = new System.Windows.Forms.Button();
            LB_Form = new System.Windows.Forms.ListBox();
            L_Seen = new System.Windows.Forms.Label();
            L_NotSeen = new System.Windows.Forms.Label();
            GB_Language.SuspendLayout();
            modifyMenu.SuspendLayout();
            SuspendLayout();
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(272, 285);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(93, 27);
            B_Cancel.TabIndex = 0;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // LB_Species
            // 
            LB_Species.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            LB_Species.FormattingEnabled = true;
            LB_Species.ItemHeight = 15;
            LB_Species.Location = new System.Drawing.Point(14, 46);
            LB_Species.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LB_Species.Name = "LB_Species";
            LB_Species.Size = new System.Drawing.Size(151, 259);
            LB_Species.TabIndex = 2;
            LB_Species.SelectedIndexChanged += ChangeLBSpecies;
            // 
            // CHK_Caught
            // 
            CHK_Caught.AutoSize = true;
            CHK_Caught.Location = new System.Drawing.Point(394, 17);
            CHK_Caught.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_Caught.Name = "CHK_Caught";
            CHK_Caught.Size = new System.Drawing.Size(65, 19);
            CHK_Caught.TabIndex = 3;
            CHK_Caught.Text = "Caught";
            CHK_Caught.UseVisualStyleBackColor = true;
            // 
            // CHK_L6
            // 
            CHK_L6.AutoSize = true;
            CHK_L6.Location = new System.Drawing.Point(200, 38);
            CHK_L6.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_L6.Name = "CHK_L6";
            CHK_L6.Size = new System.Drawing.Size(67, 19);
            CHK_L6.TabIndex = 18;
            CHK_L6.Text = "Spanish";
            CHK_L6.UseVisualStyleBackColor = true;
            // 
            // CHK_L5
            // 
            CHK_L5.AutoSize = true;
            CHK_L5.Location = new System.Drawing.Point(108, 38);
            CHK_L5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_L5.Name = "CHK_L5";
            CHK_L5.Size = new System.Drawing.Size(68, 19);
            CHK_L5.TabIndex = 17;
            CHK_L5.Text = "German";
            CHK_L5.UseVisualStyleBackColor = true;
            // 
            // CHK_L4
            // 
            CHK_L4.AutoSize = true;
            CHK_L4.Location = new System.Drawing.Point(21, 38);
            CHK_L4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_L4.Name = "CHK_L4";
            CHK_L4.Size = new System.Drawing.Size(58, 19);
            CHK_L4.TabIndex = 16;
            CHK_L4.Text = "Italian";
            CHK_L4.UseVisualStyleBackColor = true;
            // 
            // CHK_L3
            // 
            CHK_L3.AutoSize = true;
            CHK_L3.Location = new System.Drawing.Point(200, 18);
            CHK_L3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_L3.Name = "CHK_L3";
            CHK_L3.Size = new System.Drawing.Size(62, 19);
            CHK_L3.TabIndex = 15;
            CHK_L3.Text = "French";
            CHK_L3.UseVisualStyleBackColor = true;
            // 
            // CHK_L2
            // 
            CHK_L2.AutoSize = true;
            CHK_L2.Location = new System.Drawing.Point(108, 18);
            CHK_L2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_L2.Name = "CHK_L2";
            CHK_L2.Size = new System.Drawing.Size(64, 19);
            CHK_L2.TabIndex = 14;
            CHK_L2.Text = "English";
            CHK_L2.UseVisualStyleBackColor = true;
            // 
            // CHK_L1
            // 
            CHK_L1.AutoSize = true;
            CHK_L1.Location = new System.Drawing.Point(21, 18);
            CHK_L1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_L1.Name = "CHK_L1";
            CHK_L1.Size = new System.Drawing.Size(73, 19);
            CHK_L1.TabIndex = 13;
            CHK_L1.Text = "Japanese";
            CHK_L1.UseVisualStyleBackColor = true;
            // 
            // L_goto
            // 
            L_goto.AutoSize = true;
            L_goto.Location = new System.Drawing.Point(14, 18);
            L_goto.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_goto.Name = "L_goto";
            L_goto.Size = new System.Drawing.Size(35, 15);
            L_goto.TabIndex = 20;
            L_goto.Text = "goto:";
            // 
            // CB_Species
            // 
            CB_Species.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            CB_Species.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Species.DropDownWidth = 95;
            CB_Species.FormattingEnabled = true;
            CB_Species.Items.AddRange(new object[] { "0" });
            CB_Species.Location = new System.Drawing.Point(58, 15);
            CB_Species.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Species.Name = "CB_Species";
            CB_Species.Size = new System.Drawing.Size(107, 23);
            CB_Species.TabIndex = 21;
            CB_Species.SelectedValueChanged += ChangeCBSpecies;
            // 
            // B_GiveAll
            // 
            B_GiveAll.Location = new System.Drawing.Point(174, 13);
            B_GiveAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_GiveAll.Name = "B_GiveAll";
            B_GiveAll.Size = new System.Drawing.Size(70, 27);
            B_GiveAll.TabIndex = 23;
            B_GiveAll.Text = "Check All";
            B_GiveAll.UseVisualStyleBackColor = true;
            B_GiveAll.Click += B_GiveAll_Click;
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(372, 285);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(93, 27);
            B_Save.TabIndex = 24;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Modify
            // 
            B_Modify.Location = new System.Drawing.Point(251, 13);
            B_Modify.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Modify.Name = "B_Modify";
            B_Modify.Size = new System.Drawing.Size(70, 27);
            B_Modify.TabIndex = 25;
            B_Modify.Text = "Modify...";
            B_Modify.UseVisualStyleBackColor = true;
            B_Modify.Click += B_Modify_Click;
            // 
            // GB_Language
            // 
            GB_Language.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            GB_Language.Controls.Add(CHK_L6);
            GB_Language.Controls.Add(CHK_L5);
            GB_Language.Controls.Add(CHK_L4);
            GB_Language.Controls.Add(CHK_L3);
            GB_Language.Controls.Add(CHK_L2);
            GB_Language.Controls.Add(CHK_L1);
            GB_Language.Location = new System.Drawing.Point(173, 216);
            GB_Language.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Language.Name = "GB_Language";
            GB_Language.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GB_Language.Size = new System.Drawing.Size(292, 63);
            GB_Language.TabIndex = 26;
            GB_Language.TabStop = false;
            GB_Language.Text = "Languages";
            // 
            // modifyMenu
            // 
            modifyMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuSeenNone, mnuSeenAll, mnuCaughtNone, mnuCaughtAll, mnuComplete, mnuUpgraded });
            modifyMenu.Name = "modifyMenu";
            modifyMenu.Size = new System.Drawing.Size(150, 136);
            // 
            // mnuSeenNone
            // 
            mnuSeenNone.Name = "mnuSeenNone";
            mnuSeenNone.Size = new System.Drawing.Size(149, 22);
            mnuSeenNone.Text = "Seen none";
            mnuSeenNone.Click += ModifyAll;
            // 
            // mnuSeenAll
            // 
            mnuSeenAll.Name = "mnuSeenAll";
            mnuSeenAll.Size = new System.Drawing.Size(149, 22);
            mnuSeenAll.Text = "Seen all";
            mnuSeenAll.Click += ModifyAll;
            // 
            // mnuCaughtNone
            // 
            mnuCaughtNone.Name = "mnuCaughtNone";
            mnuCaughtNone.Size = new System.Drawing.Size(149, 22);
            mnuCaughtNone.Text = "Caught none";
            mnuCaughtNone.Click += ModifyAll;
            // 
            // mnuCaughtAll
            // 
            mnuCaughtAll.Name = "mnuCaughtAll";
            mnuCaughtAll.Size = new System.Drawing.Size(149, 22);
            mnuCaughtAll.Text = "Caught all";
            mnuCaughtAll.Click += ModifyAll;
            // 
            // mnuComplete
            // 
            mnuComplete.Name = "mnuComplete";
            mnuComplete.Size = new System.Drawing.Size(149, 22);
            mnuComplete.Text = "Complete Dex";
            mnuComplete.Click += ModifyAll;
            // 
            // mnuUpgraded
            // 
            mnuUpgraded.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { CB_DexUpgraded });
            mnuUpgraded.Name = "mnuUpgraded";
            mnuUpgraded.Size = new System.Drawing.Size(149, 22);
            mnuUpgraded.Text = "Dex Upgrade";
            // 
            // CB_DexUpgraded
            // 
            CB_DexUpgraded.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_DexUpgraded.Name = "CB_DexUpgraded";
            CB_DexUpgraded.Size = new System.Drawing.Size(112, 23);
            // 
            // CHK_Seen
            // 
            CHK_Seen.AutoSize = true;
            CHK_Seen.Location = new System.Drawing.Point(328, 17);
            CHK_Seen.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_Seen.Name = "CHK_Seen";
            CHK_Seen.Size = new System.Drawing.Size(51, 19);
            CHK_Seen.TabIndex = 44;
            CHK_Seen.Text = "Seen";
            CHK_Seen.UseVisualStyleBackColor = true;
            CHK_Seen.CheckedChanged += CHK_Seen_CheckedChanged;
            // 
            // LB_Gender
            // 
            LB_Gender.FormattingEnabled = true;
            LB_Gender.ItemHeight = 15;
            LB_Gender.Location = new System.Drawing.Point(194, 61);
            LB_Gender.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LB_Gender.Name = "LB_Gender";
            LB_Gender.Size = new System.Drawing.Size(116, 49);
            LB_Gender.TabIndex = 45;
            // 
            // B_GUp
            // 
            B_GUp.Location = new System.Drawing.Point(170, 61);
            B_GUp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_GUp.Name = "B_GUp";
            B_GUp.Size = new System.Drawing.Size(23, 23);
            B_GUp.TabIndex = 46;
            B_GUp.Text = "↑";
            B_GUp.UseVisualStyleBackColor = true;
            B_GUp.Click += MoveGender;
            // 
            // B_GDown
            // 
            B_GDown.Location = new System.Drawing.Point(170, 88);
            B_GDown.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_GDown.Name = "B_GDown";
            B_GDown.Size = new System.Drawing.Size(23, 23);
            B_GDown.TabIndex = 47;
            B_GDown.Text = "↓";
            B_GDown.UseVisualStyleBackColor = true;
            B_GDown.Click += MoveGender;
            // 
            // B_GRight
            // 
            B_GRight.Location = new System.Drawing.Point(317, 88);
            B_GRight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_GRight.Name = "B_GRight";
            B_GRight.Size = new System.Drawing.Size(23, 23);
            B_GRight.TabIndex = 49;
            B_GRight.Text = ">";
            B_GRight.UseVisualStyleBackColor = true;
            B_GRight.Click += ToggleSeen;
            // 
            // B_GLeft
            // 
            B_GLeft.Location = new System.Drawing.Point(317, 61);
            B_GLeft.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_GLeft.Name = "B_GLeft";
            B_GLeft.Size = new System.Drawing.Size(23, 23);
            B_GLeft.TabIndex = 48;
            B_GLeft.Text = "<";
            B_GLeft.UseVisualStyleBackColor = true;
            B_GLeft.Click += ToggleSeen;
            // 
            // LB_NGender
            // 
            LB_NGender.FormattingEnabled = true;
            LB_NGender.ItemHeight = 15;
            LB_NGender.Location = new System.Drawing.Point(348, 61);
            LB_NGender.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LB_NGender.Name = "LB_NGender";
            LB_NGender.Size = new System.Drawing.Size(116, 49);
            LB_NGender.TabIndex = 50;
            // 
            // LB_NForm
            // 
            LB_NForm.FormattingEnabled = true;
            LB_NForm.ItemHeight = 15;
            LB_NForm.Location = new System.Drawing.Point(348, 119);
            LB_NForm.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LB_NForm.Name = "LB_NForm";
            LB_NForm.Size = new System.Drawing.Size(116, 94);
            LB_NForm.TabIndex = 56;
            // 
            // B_FRight
            // 
            B_FRight.Location = new System.Drawing.Point(317, 145);
            B_FRight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_FRight.Name = "B_FRight";
            B_FRight.Size = new System.Drawing.Size(23, 23);
            B_FRight.TabIndex = 55;
            B_FRight.Text = ">";
            B_FRight.UseVisualStyleBackColor = true;
            B_FRight.Click += ToggleForm;
            // 
            // B_FLeft
            // 
            B_FLeft.Location = new System.Drawing.Point(317, 119);
            B_FLeft.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_FLeft.Name = "B_FLeft";
            B_FLeft.Size = new System.Drawing.Size(23, 23);
            B_FLeft.TabIndex = 54;
            B_FLeft.Text = "<";
            B_FLeft.UseVisualStyleBackColor = true;
            B_FLeft.Click += ToggleForm;
            // 
            // B_FDown
            // 
            B_FDown.Location = new System.Drawing.Point(170, 145);
            B_FDown.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_FDown.Name = "B_FDown";
            B_FDown.Size = new System.Drawing.Size(23, 23);
            B_FDown.TabIndex = 53;
            B_FDown.Text = "↓";
            B_FDown.UseVisualStyleBackColor = true;
            B_FDown.Click += MoveForm;
            // 
            // B_FUp
            // 
            B_FUp.Location = new System.Drawing.Point(170, 119);
            B_FUp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_FUp.Name = "B_FUp";
            B_FUp.Size = new System.Drawing.Size(23, 23);
            B_FUp.TabIndex = 52;
            B_FUp.Text = "↑";
            B_FUp.UseVisualStyleBackColor = true;
            B_FUp.Click += MoveForm;
            // 
            // LB_Form
            // 
            LB_Form.FormattingEnabled = true;
            LB_Form.ItemHeight = 15;
            LB_Form.Location = new System.Drawing.Point(194, 119);
            LB_Form.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LB_Form.Name = "LB_Form";
            LB_Form.Size = new System.Drawing.Size(116, 94);
            LB_Form.TabIndex = 51;
            // 
            // L_Seen
            // 
            L_Seen.AutoSize = true;
            L_Seen.Location = new System.Drawing.Point(190, 43);
            L_Seen.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Seen.Name = "L_Seen";
            L_Seen.Size = new System.Drawing.Size(32, 15);
            L_Seen.TabIndex = 57;
            L_Seen.Text = "Seen";
            // 
            // L_NotSeen
            // 
            L_NotSeen.AutoSize = true;
            L_NotSeen.Location = new System.Drawing.Point(344, 43);
            L_NotSeen.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_NotSeen.Name = "L_NotSeen";
            L_NotSeen.Size = new System.Drawing.Size(55, 15);
            L_NotSeen.TabIndex = 58;
            L_NotSeen.Text = "Not Seen";
            // 
            // SAV_Pokedex4
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(474, 320);
            Controls.Add(L_NotSeen);
            Controls.Add(L_Seen);
            Controls.Add(LB_NForm);
            Controls.Add(B_FRight);
            Controls.Add(B_FLeft);
            Controls.Add(B_FDown);
            Controls.Add(B_FUp);
            Controls.Add(LB_Form);
            Controls.Add(LB_NGender);
            Controls.Add(B_GRight);
            Controls.Add(B_GLeft);
            Controls.Add(B_GDown);
            Controls.Add(B_GUp);
            Controls.Add(LB_Gender);
            Controls.Add(CHK_Seen);
            Controls.Add(CHK_Caught);
            Controls.Add(GB_Language);
            Controls.Add(B_Modify);
            Controls.Add(B_Save);
            Controls.Add(B_GiveAll);
            Controls.Add(CB_Species);
            Controls.Add(L_goto);
            Controls.Add(LB_Species);
            Controls.Add(B_Cancel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_Pokedex4";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Pokédex Editor";
            GB_Language.ResumeLayout(false);
            GB_Language.PerformLayout();
            modifyMenu.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.ListBox LB_Species;
        private System.Windows.Forms.CheckBox CHK_Caught;
        private System.Windows.Forms.CheckBox CHK_L6;
        private System.Windows.Forms.CheckBox CHK_L5;
        private System.Windows.Forms.CheckBox CHK_L4;
        private System.Windows.Forms.CheckBox CHK_L3;
        private System.Windows.Forms.CheckBox CHK_L2;
        private System.Windows.Forms.CheckBox CHK_L1;
        private System.Windows.Forms.Label L_goto;
        private System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.Button B_GiveAll;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Modify;
        private System.Windows.Forms.GroupBox GB_Language;
        private System.Windows.Forms.ContextMenuStrip modifyMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuSeenNone;
        private System.Windows.Forms.ToolStripMenuItem mnuSeenAll;
        private System.Windows.Forms.ToolStripMenuItem mnuCaughtNone;
        private System.Windows.Forms.ToolStripMenuItem mnuCaughtAll;
        private System.Windows.Forms.ToolStripMenuItem mnuComplete;
        private System.Windows.Forms.ToolStripMenuItem mnuUpgraded;
        private System.Windows.Forms.CheckBox CHK_Seen;
        private System.Windows.Forms.ListBox LB_Gender;
        private System.Windows.Forms.Button B_GUp;
        private System.Windows.Forms.Button B_GDown;
        private System.Windows.Forms.Button B_GRight;
        private System.Windows.Forms.Button B_GLeft;
        private System.Windows.Forms.ListBox LB_NGender;
        private System.Windows.Forms.ListBox LB_NForm;
        private System.Windows.Forms.Button B_FRight;
        private System.Windows.Forms.Button B_FLeft;
        private System.Windows.Forms.Button B_FDown;
        private System.Windows.Forms.Button B_FUp;
        private System.Windows.Forms.ListBox LB_Form;
        private System.Windows.Forms.Label L_Seen;
        private System.Windows.Forms.Label L_NotSeen;
        private System.Windows.Forms.ToolStripComboBox CB_DexUpgraded;
    }
}
