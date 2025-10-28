namespace PKHeX.WinForms
{
    partial class PoffinCase4Editor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            LB_Poffins = new System.Windows.Forms.ListBox();
            B_PoffinDel = new System.Windows.Forms.Button();
            B_PoffinAll = new System.Windows.Forms.Button();
            PG_Poffins = new System.Windows.Forms.PropertyGrid();
            SuspendLayout();
            // 
            // LB_Poffins
            // 
            LB_Poffins.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            LB_Poffins.FormattingEnabled = true;
            LB_Poffins.ItemHeight = 15;
            LB_Poffins.Location = new System.Drawing.Point(0, 2);
            LB_Poffins.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LB_Poffins.Name = "LB_Poffins";
            LB_Poffins.Size = new System.Drawing.Size(139, 259);
            LB_Poffins.TabIndex = 12;
            LB_Poffins.SelectedIndexChanged += LB_Poffins_SelectedIndexChanged;
            // 
            // B_PoffinDel
            // 
            B_PoffinDel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_PoffinDel.Location = new System.Drawing.Point(246, 232);
            B_PoffinDel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_PoffinDel.Name = "B_PoffinDel";
            B_PoffinDel.Size = new System.Drawing.Size(96, 30);
            B_PoffinDel.TabIndex = 11;
            B_PoffinDel.Text = "Delete All";
            B_PoffinDel.UseVisualStyleBackColor = true;
            B_PoffinDel.Click += B_PoffinDel_Click;
            // 
            // B_PoffinAll
            // 
            B_PoffinAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_PoffinAll.Location = new System.Drawing.Point(147, 232);
            B_PoffinAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_PoffinAll.Name = "B_PoffinAll";
            B_PoffinAll.Size = new System.Drawing.Size(92, 30);
            B_PoffinAll.TabIndex = 10;
            B_PoffinAll.Text = "Give All";
            B_PoffinAll.UseVisualStyleBackColor = true;
            B_PoffinAll.Click += B_PoffinAll_Click;
            // 
            // PG_Poffins
            // 
            PG_Poffins.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            PG_Poffins.HelpVisible = false;
            PG_Poffins.Location = new System.Drawing.Point(147, 2);
            PG_Poffins.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PG_Poffins.Name = "PG_Poffins";
            PG_Poffins.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            PG_Poffins.Size = new System.Drawing.Size(258, 225);
            PG_Poffins.TabIndex = 9;
            PG_Poffins.ToolbarVisible = false;
            PG_Poffins.PropertyValueChanged += PG_Poffins_PropertyValueChanged;
            // 
            // PoffinCase4Editor
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(LB_Poffins);
            Controls.Add(B_PoffinDel);
            Controls.Add(B_PoffinAll);
            Controls.Add(PG_Poffins);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "PoffinCase4Editor";
            Size = new System.Drawing.Size(405, 268);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ListBox LB_Poffins;
        private System.Windows.Forms.Button B_PoffinDel;
        private System.Windows.Forms.Button B_PoffinAll;
        private System.Windows.Forms.PropertyGrid PG_Poffins;
    }
}
