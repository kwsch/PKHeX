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
            this.components = new System.ComponentModel.Container();
            this.B_Cancel = new System.Windows.Forms.Button();
            this.LB_Species = new System.Windows.Forms.ListBox();
            this.CHK_Caught = new System.Windows.Forms.CheckBox();
            this.CHK_L6 = new System.Windows.Forms.CheckBox();
            this.CHK_L5 = new System.Windows.Forms.CheckBox();
            this.CHK_L4 = new System.Windows.Forms.CheckBox();
            this.CHK_L3 = new System.Windows.Forms.CheckBox();
            this.CHK_L2 = new System.Windows.Forms.CheckBox();
            this.CHK_L1 = new System.Windows.Forms.CheckBox();
            this.L_goto = new System.Windows.Forms.Label();
            this.CB_Species = new System.Windows.Forms.ComboBox();
            this.B_GiveAll = new System.Windows.Forms.Button();
            this.B_Save = new System.Windows.Forms.Button();
            this.B_Modify = new System.Windows.Forms.Button();
            this.GB_Language = new System.Windows.Forms.GroupBox();
            this.modifyMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuSeenNone = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSeenAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCaughtNone = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCaughtAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuComplete = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuUpgraded = new System.Windows.Forms.ToolStripMenuItem();
            this.CB_DexUpgraded = new System.Windows.Forms.ToolStripComboBox();
            this.CHK_Seen = new System.Windows.Forms.CheckBox();
            this.LB_Gender = new System.Windows.Forms.ListBox();
            this.B_GUp = new System.Windows.Forms.Button();
            this.B_GDown = new System.Windows.Forms.Button();
            this.B_GRight = new System.Windows.Forms.Button();
            this.B_GLeft = new System.Windows.Forms.Button();
            this.LB_NGender = new System.Windows.Forms.ListBox();
            this.LB_NForm = new System.Windows.Forms.ListBox();
            this.B_FRight = new System.Windows.Forms.Button();
            this.B_FLeft = new System.Windows.Forms.Button();
            this.B_FDown = new System.Windows.Forms.Button();
            this.B_FUp = new System.Windows.Forms.Button();
            this.LB_Form = new System.Windows.Forms.ListBox();
            this.L_Seen = new System.Windows.Forms.Label();
            this.L_NotSeen = new System.Windows.Forms.Label();
            this.GB_Language.SuspendLayout();
            this.modifyMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(233, 247);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(80, 23);
            this.B_Cancel.TabIndex = 0;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // LB_Species
            // 
            this.LB_Species.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.LB_Species.FormattingEnabled = true;
            this.LB_Species.Location = new System.Drawing.Point(12, 40);
            this.LB_Species.Name = "LB_Species";
            this.LB_Species.Size = new System.Drawing.Size(130, 225);
            this.LB_Species.TabIndex = 2;
            this.LB_Species.SelectedIndexChanged += new System.EventHandler(this.ChangeLBSpecies);
            // 
            // CHK_Caught
            // 
            this.CHK_Caught.AutoSize = true;
            this.CHK_Caught.Location = new System.Drawing.Point(338, 15);
            this.CHK_Caught.Name = "CHK_Caught";
            this.CHK_Caught.Size = new System.Drawing.Size(60, 17);
            this.CHK_Caught.TabIndex = 3;
            this.CHK_Caught.Text = "Caught";
            this.CHK_Caught.UseVisualStyleBackColor = true;
            // 
            // CHK_L6
            // 
            this.CHK_L6.AutoSize = true;
            this.CHK_L6.Location = new System.Drawing.Point(171, 33);
            this.CHK_L6.Name = "CHK_L6";
            this.CHK_L6.Size = new System.Drawing.Size(64, 17);
            this.CHK_L6.TabIndex = 18;
            this.CHK_L6.Text = "Spanish";
            this.CHK_L6.UseVisualStyleBackColor = true;
            // 
            // CHK_L5
            // 
            this.CHK_L5.AutoSize = true;
            this.CHK_L5.Location = new System.Drawing.Point(93, 33);
            this.CHK_L5.Name = "CHK_L5";
            this.CHK_L5.Size = new System.Drawing.Size(63, 17);
            this.CHK_L5.TabIndex = 17;
            this.CHK_L5.Text = "German";
            this.CHK_L5.UseVisualStyleBackColor = true;
            // 
            // CHK_L4
            // 
            this.CHK_L4.AutoSize = true;
            this.CHK_L4.Location = new System.Drawing.Point(18, 33);
            this.CHK_L4.Name = "CHK_L4";
            this.CHK_L4.Size = new System.Drawing.Size(54, 17);
            this.CHK_L4.TabIndex = 16;
            this.CHK_L4.Text = "Italian";
            this.CHK_L4.UseVisualStyleBackColor = true;
            // 
            // CHK_L3
            // 
            this.CHK_L3.AutoSize = true;
            this.CHK_L3.Location = new System.Drawing.Point(171, 16);
            this.CHK_L3.Name = "CHK_L3";
            this.CHK_L3.Size = new System.Drawing.Size(59, 17);
            this.CHK_L3.TabIndex = 15;
            this.CHK_L3.Text = "French";
            this.CHK_L3.UseVisualStyleBackColor = true;
            // 
            // CHK_L2
            // 
            this.CHK_L2.AutoSize = true;
            this.CHK_L2.Location = new System.Drawing.Point(93, 16);
            this.CHK_L2.Name = "CHK_L2";
            this.CHK_L2.Size = new System.Drawing.Size(60, 17);
            this.CHK_L2.TabIndex = 14;
            this.CHK_L2.Text = "English";
            this.CHK_L2.UseVisualStyleBackColor = true;
            // 
            // CHK_L1
            // 
            this.CHK_L1.AutoSize = true;
            this.CHK_L1.Location = new System.Drawing.Point(18, 16);
            this.CHK_L1.Name = "CHK_L1";
            this.CHK_L1.Size = new System.Drawing.Size(72, 17);
            this.CHK_L1.TabIndex = 13;
            this.CHK_L1.Text = "Japanese";
            this.CHK_L1.UseVisualStyleBackColor = true;
            // 
            // L_goto
            // 
            this.L_goto.AutoSize = true;
            this.L_goto.Location = new System.Drawing.Point(12, 16);
            this.L_goto.Name = "L_goto";
            this.L_goto.Size = new System.Drawing.Size(31, 13);
            this.L_goto.TabIndex = 20;
            this.L_goto.Text = "goto:";
            // 
            // CB_Species
            // 
            this.CB_Species.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.CB_Species.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Species.DropDownWidth = 95;
            this.CB_Species.FormattingEnabled = true;
            this.CB_Species.Items.AddRange(new object[] {
            "0"});
            this.CB_Species.Location = new System.Drawing.Point(50, 13);
            this.CB_Species.Name = "CB_Species";
            this.CB_Species.Size = new System.Drawing.Size(92, 21);
            this.CB_Species.TabIndex = 21;
            this.CB_Species.SelectedValueChanged += new System.EventHandler(this.ChangeCBSpecies);
            // 
            // B_GiveAll
            // 
            this.B_GiveAll.Location = new System.Drawing.Point(149, 11);
            this.B_GiveAll.Name = "B_GiveAll";
            this.B_GiveAll.Size = new System.Drawing.Size(60, 23);
            this.B_GiveAll.TabIndex = 23;
            this.B_GiveAll.Text = "Check All";
            this.B_GiveAll.UseVisualStyleBackColor = true;
            this.B_GiveAll.Click += new System.EventHandler(this.B_GiveAll_Click);
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(319, 247);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(80, 23);
            this.B_Save.TabIndex = 24;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // B_Modify
            // 
            this.B_Modify.Location = new System.Drawing.Point(215, 11);
            this.B_Modify.Name = "B_Modify";
            this.B_Modify.Size = new System.Drawing.Size(60, 23);
            this.B_Modify.TabIndex = 25;
            this.B_Modify.Text = "Modify...";
            this.B_Modify.UseVisualStyleBackColor = true;
            this.B_Modify.Click += new System.EventHandler(this.B_Modify_Click);
            // 
            // GB_Language
            // 
            this.GB_Language.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.GB_Language.Controls.Add(this.CHK_L6);
            this.GB_Language.Controls.Add(this.CHK_L5);
            this.GB_Language.Controls.Add(this.CHK_L4);
            this.GB_Language.Controls.Add(this.CHK_L3);
            this.GB_Language.Controls.Add(this.CHK_L2);
            this.GB_Language.Controls.Add(this.CHK_L1);
            this.GB_Language.Location = new System.Drawing.Point(148, 187);
            this.GB_Language.Name = "GB_Language";
            this.GB_Language.Size = new System.Drawing.Size(250, 55);
            this.GB_Language.TabIndex = 26;
            this.GB_Language.TabStop = false;
            this.GB_Language.Text = "Languages";
            // 
            // modifyMenu
            // 
            this.modifyMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSeenNone,
            this.mnuSeenAll,
            this.mnuCaughtNone,
            this.mnuCaughtAll,
            this.mnuComplete,
            this.mnuUpgraded});
            this.modifyMenu.Name = "modifyMenu";
            this.modifyMenu.Size = new System.Drawing.Size(149, 136);
            // 
            // mnuSeenNone
            // 
            this.mnuSeenNone.Name = "mnuSeenNone";
            this.mnuSeenNone.Size = new System.Drawing.Size(148, 22);
            this.mnuSeenNone.Text = "Seen none";
            this.mnuSeenNone.Click += new System.EventHandler(this.ModifyAll);
            // 
            // mnuSeenAll
            // 
            this.mnuSeenAll.Name = "mnuSeenAll";
            this.mnuSeenAll.Size = new System.Drawing.Size(148, 22);
            this.mnuSeenAll.Text = "Seen all";
            this.mnuSeenAll.Click += new System.EventHandler(this.ModifyAll);
            // 
            // mnuCaughtNone
            // 
            this.mnuCaughtNone.Name = "mnuCaughtNone";
            this.mnuCaughtNone.Size = new System.Drawing.Size(148, 22);
            this.mnuCaughtNone.Text = "Caught none";
            this.mnuCaughtNone.Click += new System.EventHandler(this.ModifyAll);
            // 
            // mnuCaughtAll
            // 
            this.mnuCaughtAll.Name = "mnuCaughtAll";
            this.mnuCaughtAll.Size = new System.Drawing.Size(148, 22);
            this.mnuCaughtAll.Text = "Caught all";
            this.mnuCaughtAll.Click += new System.EventHandler(this.ModifyAll);
            // 
            // mnuComplete
            // 
            this.mnuComplete.Name = "mnuComplete";
            this.mnuComplete.Size = new System.Drawing.Size(148, 22);
            this.mnuComplete.Text = "Complete Dex";
            this.mnuComplete.Click += new System.EventHandler(this.ModifyAll);
            // 
            // mnuUpgraded
            // 
            this.mnuUpgraded.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CB_DexUpgraded});
            this.mnuUpgraded.Name = "mnuUpgraded";
            this.mnuUpgraded.Size = new System.Drawing.Size(148, 22);
            this.mnuUpgraded.Text = "Dex Upgrade";
            // 
            // CB_DexUpgraded
            // 
            this.CB_DexUpgraded.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_DexUpgraded.Name = "CB_DexUpgraded";
            this.CB_DexUpgraded.Size = new System.Drawing.Size(112, 23);
            // 
            // CHK_Seen
            // 
            this.CHK_Seen.AutoSize = true;
            this.CHK_Seen.Location = new System.Drawing.Point(281, 15);
            this.CHK_Seen.Name = "CHK_Seen";
            this.CHK_Seen.Size = new System.Drawing.Size(51, 17);
            this.CHK_Seen.TabIndex = 44;
            this.CHK_Seen.Text = "Seen";
            this.CHK_Seen.UseVisualStyleBackColor = true;
            this.CHK_Seen.CheckedChanged += new System.EventHandler(this.CHK_Seen_CheckedChanged);
            // 
            // LB_Gender
            // 
            this.LB_Gender.FormattingEnabled = true;
            this.LB_Gender.Location = new System.Drawing.Point(166, 53);
            this.LB_Gender.Name = "LB_Gender";
            this.LB_Gender.Size = new System.Drawing.Size(100, 43);
            this.LB_Gender.TabIndex = 45;
            // 
            // B_GUp
            // 
            this.B_GUp.Location = new System.Drawing.Point(146, 53);
            this.B_GUp.Name = "B_GUp";
            this.B_GUp.Size = new System.Drawing.Size(20, 20);
            this.B_GUp.TabIndex = 46;
            this.B_GUp.Text = "↑";
            this.B_GUp.UseVisualStyleBackColor = true;
            this.B_GUp.Click += new System.EventHandler(this.MoveGender);
            // 
            // B_GDown
            // 
            this.B_GDown.Location = new System.Drawing.Point(146, 76);
            this.B_GDown.Name = "B_GDown";
            this.B_GDown.Size = new System.Drawing.Size(20, 20);
            this.B_GDown.TabIndex = 47;
            this.B_GDown.Text = "↓";
            this.B_GDown.UseVisualStyleBackColor = true;
            this.B_GDown.Click += new System.EventHandler(this.MoveGender);
            // 
            // B_GRight
            // 
            this.B_GRight.Location = new System.Drawing.Point(272, 76);
            this.B_GRight.Name = "B_GRight";
            this.B_GRight.Size = new System.Drawing.Size(20, 20);
            this.B_GRight.TabIndex = 49;
            this.B_GRight.Text = ">";
            this.B_GRight.UseVisualStyleBackColor = true;
            this.B_GRight.Click += new System.EventHandler(this.ToggleSeen);
            // 
            // B_GLeft
            // 
            this.B_GLeft.Location = new System.Drawing.Point(272, 53);
            this.B_GLeft.Name = "B_GLeft";
            this.B_GLeft.Size = new System.Drawing.Size(20, 20);
            this.B_GLeft.TabIndex = 48;
            this.B_GLeft.Text = "<";
            this.B_GLeft.UseVisualStyleBackColor = true;
            this.B_GLeft.Click += new System.EventHandler(this.ToggleSeen);
            // 
            // LB_NGender
            // 
            this.LB_NGender.FormattingEnabled = true;
            this.LB_NGender.Location = new System.Drawing.Point(298, 53);
            this.LB_NGender.Name = "LB_NGender";
            this.LB_NGender.Size = new System.Drawing.Size(100, 43);
            this.LB_NGender.TabIndex = 50;
            // 
            // LB_NForm
            // 
            this.LB_NForm.FormattingEnabled = true;
            this.LB_NForm.Location = new System.Drawing.Point(298, 103);
            this.LB_NForm.Name = "LB_NForm";
            this.LB_NForm.Size = new System.Drawing.Size(100, 82);
            this.LB_NForm.TabIndex = 56;
            // 
            // B_FRight
            // 
            this.B_FRight.Location = new System.Drawing.Point(272, 126);
            this.B_FRight.Name = "B_FRight";
            this.B_FRight.Size = new System.Drawing.Size(20, 20);
            this.B_FRight.TabIndex = 55;
            this.B_FRight.Text = ">";
            this.B_FRight.UseVisualStyleBackColor = true;
            this.B_FRight.Click += new System.EventHandler(this.ToggleForm);
            // 
            // B_FLeft
            // 
            this.B_FLeft.Location = new System.Drawing.Point(272, 103);
            this.B_FLeft.Name = "B_FLeft";
            this.B_FLeft.Size = new System.Drawing.Size(20, 20);
            this.B_FLeft.TabIndex = 54;
            this.B_FLeft.Text = "<";
            this.B_FLeft.UseVisualStyleBackColor = true;
            this.B_FLeft.Click += new System.EventHandler(this.ToggleForm);
            // 
            // B_FDown
            // 
            this.B_FDown.Location = new System.Drawing.Point(146, 126);
            this.B_FDown.Name = "B_FDown";
            this.B_FDown.Size = new System.Drawing.Size(20, 20);
            this.B_FDown.TabIndex = 53;
            this.B_FDown.Text = "↓";
            this.B_FDown.UseVisualStyleBackColor = true;
            this.B_FDown.Click += new System.EventHandler(this.MoveForm);
            // 
            // B_FUp
            // 
            this.B_FUp.Location = new System.Drawing.Point(146, 103);
            this.B_FUp.Name = "B_FUp";
            this.B_FUp.Size = new System.Drawing.Size(20, 20);
            this.B_FUp.TabIndex = 52;
            this.B_FUp.Text = "↑";
            this.B_FUp.UseVisualStyleBackColor = true;
            this.B_FUp.Click += new System.EventHandler(this.MoveForm);
            // 
            // LB_Form
            // 
            this.LB_Form.FormattingEnabled = true;
            this.LB_Form.Location = new System.Drawing.Point(166, 103);
            this.LB_Form.Name = "LB_Form";
            this.LB_Form.Size = new System.Drawing.Size(100, 82);
            this.LB_Form.TabIndex = 51;
            // 
            // L_Seen
            // 
            this.L_Seen.AutoSize = true;
            this.L_Seen.Location = new System.Drawing.Point(163, 37);
            this.L_Seen.Name = "L_Seen";
            this.L_Seen.Size = new System.Drawing.Size(32, 13);
            this.L_Seen.TabIndex = 57;
            this.L_Seen.Text = "Seen";
            // 
            // L_NotSeen
            // 
            this.L_NotSeen.AutoSize = true;
            this.L_NotSeen.Location = new System.Drawing.Point(295, 37);
            this.L_NotSeen.Name = "L_NotSeen";
            this.L_NotSeen.Size = new System.Drawing.Size(52, 13);
            this.L_NotSeen.TabIndex = 58;
            this.L_NotSeen.Text = "Not Seen";
            // 
            // SAV_Pokedex4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 277);
            this.Controls.Add(this.L_NotSeen);
            this.Controls.Add(this.L_Seen);
            this.Controls.Add(this.LB_NForm);
            this.Controls.Add(this.B_FRight);
            this.Controls.Add(this.B_FLeft);
            this.Controls.Add(this.B_FDown);
            this.Controls.Add(this.B_FUp);
            this.Controls.Add(this.LB_Form);
            this.Controls.Add(this.LB_NGender);
            this.Controls.Add(this.B_GRight);
            this.Controls.Add(this.B_GLeft);
            this.Controls.Add(this.B_GDown);
            this.Controls.Add(this.B_GUp);
            this.Controls.Add(this.LB_Gender);
            this.Controls.Add(this.CHK_Seen);
            this.Controls.Add(this.CHK_Caught);
            this.Controls.Add(this.GB_Language);
            this.Controls.Add(this.B_Modify);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_GiveAll);
            this.Controls.Add(this.CB_Species);
            this.Controls.Add(this.L_goto);
            this.Controls.Add(this.LB_Species);
            this.Controls.Add(this.B_Cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SAV_Pokedex4";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Pokédex Editor";
            this.GB_Language.ResumeLayout(false);
            this.GB_Language.PerformLayout();
            this.modifyMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

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
