namespace PKHeX.WinForms
{
    partial class Pokeblock3CaseEditor
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
            LB_Pokeblocks = new System.Windows.Forms.ListBox();
            B_PokeblockDel = new System.Windows.Forms.Button();
            B_PokeblockAll = new System.Windows.Forms.Button();
            PG_Pokeblocks = new System.Windows.Forms.PropertyGrid();
            SuspendLayout();
            // 
            // LB_Pokeblocks
            // 
            LB_Pokeblocks.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            LB_Pokeblocks.FormattingEnabled = true;
            LB_Pokeblocks.ItemHeight = 15;
            LB_Pokeblocks.Location = new System.Drawing.Point(0, 2);
            LB_Pokeblocks.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LB_Pokeblocks.Name = "LB_Pokeblocks";
            LB_Pokeblocks.Size = new System.Drawing.Size(139, 244);
            LB_Pokeblocks.TabIndex = 12;
            LB_Pokeblocks.SelectedIndexChanged += LB_Pokeblocks_SelectedIndexChanged;
            // 
            // B_PokeblockDel
            // 
            B_PokeblockDel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_PokeblockDel.Location = new System.Drawing.Point(246, 216);
            B_PokeblockDel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_PokeblockDel.Name = "B_PokeblockDel";
            B_PokeblockDel.Size = new System.Drawing.Size(96, 30);
            B_PokeblockDel.TabIndex = 11;
            B_PokeblockDel.Text = "Delete All";
            B_PokeblockDel.UseVisualStyleBackColor = true;
            B_PokeblockDel.Click += B_PokeblockDel_Click;
            // 
            // B_PokeblockAll
            // 
            B_PokeblockAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_PokeblockAll.Location = new System.Drawing.Point(147, 216);
            B_PokeblockAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_PokeblockAll.Name = "B_PokeblockAll";
            B_PokeblockAll.Size = new System.Drawing.Size(92, 30);
            B_PokeblockAll.TabIndex = 10;
            B_PokeblockAll.Text = "Give All";
            B_PokeblockAll.UseVisualStyleBackColor = true;
            B_PokeblockAll.Click += B_PokeblockAll_Click;
            // 
            // PG_Pokeblocks
            // 
            PG_Pokeblocks.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            PG_Pokeblocks.HelpVisible = false;
            PG_Pokeblocks.Location = new System.Drawing.Point(147, 2);
            PG_Pokeblocks.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PG_Pokeblocks.Name = "PG_Pokeblocks";
            PG_Pokeblocks.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            PG_Pokeblocks.Size = new System.Drawing.Size(201, 207);
            PG_Pokeblocks.TabIndex = 9;
            PG_Pokeblocks.ToolbarVisible = false;
            PG_Pokeblocks.PropertyValueChanged += PG_Pokeblocks_PropertyValueChanged;
            // 
            // PokeblockCase3Editor
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(LB_Pokeblocks);
            Controls.Add(B_PokeblockDel);
            Controls.Add(B_PokeblockAll);
            Controls.Add(PG_Pokeblocks);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "PokeblockCase3Editor";
            Size = new System.Drawing.Size(348, 248);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ListBox LB_Pokeblocks;
        private System.Windows.Forms.Button B_PokeblockDel;
        private System.Windows.Forms.Button B_PokeblockAll;
        private System.Windows.Forms.PropertyGrid PG_Pokeblocks;
    }
}
