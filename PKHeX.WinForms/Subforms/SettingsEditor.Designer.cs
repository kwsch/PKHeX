using System.Windows.Forms;
using PKHeX.WinForms.Controls;

namespace PKHeX.WinForms
{
    partial class SettingsEditor
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
            if (disposing && (components is not null))
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
            FLP_Blank = new FlowLayoutPanel();
            L_Blank = new Label();
            CB_Blank = new ComboBox();
            B_Reset = new Button();
            LB_Tabs = new ListBox();
            splitContainer1 = new SplitContainer();
            PG_Editor = new PropertyGrid();
            FLP_Blank.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // FLP_Blank
            // 
            FLP_Blank.Controls.Add(L_Blank);
            FLP_Blank.Controls.Add(CB_Blank);
            FLP_Blank.Dock = DockStyle.Top;
            FLP_Blank.Location = new System.Drawing.Point(0, 0);
            FLP_Blank.Name = "FLP_Blank";
            FLP_Blank.Size = new System.Drawing.Size(584, 27);
            FLP_Blank.TabIndex = 1;
            // 
            // L_Blank
            // 
            L_Blank.AutoSize = true;
            L_Blank.Location = new System.Drawing.Point(3, 0);
            L_Blank.Name = "L_Blank";
            L_Blank.Padding = new Padding(0, 6, 0, 0);
            L_Blank.Size = new System.Drawing.Size(119, 23);
            L_Blank.TabIndex = 0;
            L_Blank.Text = "Blank Save Version:";
            // 
            // CB_Blank
            // 
            CB_Blank.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            CB_Blank.AutoCompleteSource = AutoCompleteSource.ListItems;
            CB_Blank.FormattingEnabled = true;
            CB_Blank.Location = new System.Drawing.Point(128, 3);
            CB_Blank.Name = "CB_Blank";
            CB_Blank.Size = new System.Drawing.Size(180, 25);
            CB_Blank.TabIndex = 1;
            // 
            // B_Reset
            // 
            B_Reset.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            B_Reset.Location = new System.Drawing.Point(506, 2);
            B_Reset.Name = "B_Reset";
            B_Reset.Size = new System.Drawing.Size(75, 23);
            B_Reset.TabIndex = 4;
            B_Reset.Text = "Reset All";
            B_Reset.UseVisualStyleBackColor = true;
            // 
            // LB_Tabs
            // 
            LB_Tabs.Dock = DockStyle.Fill;
            LB_Tabs.FormattingEnabled = true;
            LB_Tabs.Location = new System.Drawing.Point(0, 0);
            LB_Tabs.Name = "LB_Tabs";
            LB_Tabs.Size = new System.Drawing.Size(194, 354);
            LB_Tabs.TabIndex = 5;
            LB_Tabs.SelectedIndexChanged += LB_Tabs_SelectedIndexChanged;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.Location = new System.Drawing.Point(0, 27);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(LB_Tabs);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(PG_Editor);
            splitContainer1.Size = new System.Drawing.Size(584, 354);
            splitContainer1.SplitterDistance = 194;
            splitContainer1.TabIndex = 6;
            // 
            // PG_Editor
            // 
            PG_Editor.BackColor = System.Drawing.SystemColors.Control;
            PG_Editor.Dock = DockStyle.Fill;
            PG_Editor.Location = new System.Drawing.Point(0, 0);
            PG_Editor.Name = "PG_Editor";
            PG_Editor.Size = new System.Drawing.Size(386, 354);
            PG_Editor.TabIndex = 0;
            // 
            // SettingsEditor
            // 
            AutoScaleMode = AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(584, 381);
            Controls.Add(splitContainer1);
            Controls.Add(B_Reset);
            Controls.Add(FLP_Blank);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Icon = Properties.Resources.Icon;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(510, 375);
            Name = "SettingsEditor";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Settings";
            KeyDown += SettingsEditor_KeyDown;
            FLP_Blank.ResumeLayout(false);
            FLP_Blank.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel FLP_Blank;
        private System.Windows.Forms.Label L_Blank;
        private System.Windows.Forms.ComboBox CB_Blank;
        private System.Windows.Forms.Button B_Reset;
        private ListBox LB_Tabs;
        private SplitContainer splitContainer1;
        private PropertyGrid PG_Editor;
    }
}
