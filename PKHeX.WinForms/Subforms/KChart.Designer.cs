namespace PKHeX.WinForms
{
    partial class KChart
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
            this.DGV = new System.Windows.Forms.DataGridView();
            this.d_Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sprite = new System.Windows.Forms.DataGridViewImageColumn();
            this.SpecName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.New = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.BST = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.Type2 = new System.Windows.Forms.DataGridViewImageColumn();
            this.HP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ATK = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DEF = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SPA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SPD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SPE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Ability0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Ability1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AbilityH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).BeginInit();
            this.SuspendLayout();
            // 
            // DGV
            // 
            this.DGV.AllowUserToAddRows = false;
            this.DGV.AllowUserToDeleteRows = false;
            this.DGV.AllowUserToResizeColumns = false;
            this.DGV.AllowUserToResizeRows = false;
            this.DGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.d_Index,
            this.Sprite,
            this.SpecName,
            this.New,
            this.BST,
            this.Type1,
            this.Type2,
            this.HP,
            this.ATK,
            this.DEF,
            this.SPA,
            this.SPD,
            this.SPE,
            this.Ability0,
            this.Ability1,
            this.AbilityH});
            this.DGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DGV.Location = new System.Drawing.Point(0, 0);
            this.DGV.Name = "DGV";
            this.DGV.Size = new System.Drawing.Size(1077, 615);
            this.DGV.TabIndex = 33;
            // 
            // d_Index
            // 
            this.d_Index.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.d_Index.HeaderText = "Dex#";
            this.d_Index.Name = "d_Index";
            this.d_Index.ReadOnly = true;
            this.d_Index.Width = 58;
            // 
            // Sprite
            // 
            this.Sprite.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Sprite.HeaderText = "Sprite";
            this.Sprite.Name = "Sprite";
            this.Sprite.ReadOnly = true;
            this.Sprite.Width = 40;
            // 
            // SpecName
            // 
            this.SpecName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.SpecName.HeaderText = "Name";
            this.SpecName.Name = "SpecName";
            this.SpecName.ReadOnly = true;
            this.SpecName.Width = 60;
            // 
            // New
            // 
            this.New.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.New.HeaderText = "Native";
            this.New.Name = "New";
            this.New.ReadOnly = true;
            this.New.Width = 44;
            // 
            // BST
            // 
            this.BST.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.BST.HeaderText = "BST";
            this.BST.Name = "BST";
            this.BST.ReadOnly = true;
            this.BST.Width = 53;
            // 
            // Type1
            // 
            this.Type1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Type1.HeaderText = "Type1";
            this.Type1.Name = "Type1";
            this.Type1.ReadOnly = true;
            this.Type1.Width = 43;
            // 
            // Type2
            // 
            this.Type2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Type2.HeaderText = "Type2";
            this.Type2.Name = "Type2";
            this.Type2.ReadOnly = true;
            this.Type2.Width = 43;
            // 
            // HP
            // 
            this.HP.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.HP.HeaderText = "HP";
            this.HP.Name = "HP";
            this.HP.ReadOnly = true;
            this.HP.Width = 47;
            // 
            // ATK
            // 
            this.ATK.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ATK.HeaderText = "ATK";
            this.ATK.Name = "ATK";
            this.ATK.ReadOnly = true;
            this.ATK.Width = 53;
            // 
            // DEF
            // 
            this.DEF.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.DEF.HeaderText = "DEF";
            this.DEF.Name = "DEF";
            this.DEF.ReadOnly = true;
            this.DEF.Width = 53;
            // 
            // SPA
            // 
            this.SPA.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.SPA.HeaderText = "SPA";
            this.SPA.Name = "SPA";
            this.SPA.ReadOnly = true;
            this.SPA.Width = 53;
            // 
            // SPD
            // 
            this.SPD.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.SPD.HeaderText = "SPD";
            this.SPD.Name = "SPD";
            this.SPD.ReadOnly = true;
            this.SPD.Width = 54;
            // 
            // SPE
            // 
            this.SPE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.SPE.HeaderText = "SPE";
            this.SPE.Name = "SPE";
            this.SPE.ReadOnly = true;
            this.SPE.Width = 53;
            // 
            // Ability0
            // 
            this.Ability0.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Ability0.HeaderText = "Ability 1";
            this.Ability0.Name = "Ability0";
            this.Ability0.ReadOnly = true;
            this.Ability0.Width = 68;
            // 
            // Ability1
            // 
            this.Ability1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Ability1.HeaderText = "Ability 2";
            this.Ability1.Name = "Ability1";
            this.Ability1.ReadOnly = true;
            this.Ability1.Width = 68;
            // 
            // AbilityH
            // 
            this.AbilityH.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.AbilityH.HeaderText = "Hidden Ability";
            this.AbilityH.Name = "AbilityH";
            this.AbilityH.ReadOnly = true;
            this.AbilityH.Width = 96;
            // 
            // KChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1077, 615);
            this.Controls.Add(this.DGV);
            this.MinimumSize = new System.Drawing.Size(1093, 654);
            this.Name = "KChart";
            this.Text = "KChart";
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView DGV;
        private System.Windows.Forms.DataGridViewTextBoxColumn d_Index;
        private System.Windows.Forms.DataGridViewImageColumn Sprite;
        private System.Windows.Forms.DataGridViewTextBoxColumn SpecName;
        private System.Windows.Forms.DataGridViewCheckBoxColumn New;
        private System.Windows.Forms.DataGridViewTextBoxColumn BST;
        private System.Windows.Forms.DataGridViewImageColumn Type1;
        private System.Windows.Forms.DataGridViewImageColumn Type2;
        private System.Windows.Forms.DataGridViewTextBoxColumn HP;
        private System.Windows.Forms.DataGridViewTextBoxColumn ATK;
        private System.Windows.Forms.DataGridViewTextBoxColumn DEF;
        private System.Windows.Forms.DataGridViewTextBoxColumn SPA;
        private System.Windows.Forms.DataGridViewTextBoxColumn SPD;
        private System.Windows.Forms.DataGridViewTextBoxColumn SPE;
        private System.Windows.Forms.DataGridViewTextBoxColumn Ability0;
        private System.Windows.Forms.DataGridViewTextBoxColumn Ability1;
        private System.Windows.Forms.DataGridViewTextBoxColumn AbilityH;
    }
}