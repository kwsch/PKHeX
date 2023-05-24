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
            DGV = new PKHeX.WinForms.Controls.DoubleBufferedDataGridView();
            d_Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Sprite = new System.Windows.Forms.DataGridViewImageColumn();
            SpecName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            New = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            BST = new System.Windows.Forms.DataGridViewTextBoxColumn();
            CatchRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Type1 = new System.Windows.Forms.DataGridViewImageColumn();
            Type2 = new System.Windows.Forms.DataGridViewImageColumn();
            HP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ATK = new System.Windows.Forms.DataGridViewTextBoxColumn();
            DEF = new System.Windows.Forms.DataGridViewTextBoxColumn();
            SPA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            SPD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            SPE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Ability0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Ability1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            AbilityH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(DGV)).BeginInit();
            SuspendLayout();
            // 
            // DGV
            // 
            DGV.AllowUserToAddRows = false;
            DGV.AllowUserToDeleteRows = false;
            DGV.AllowUserToResizeColumns = false;
            DGV.AllowUserToResizeRows = false;
            DGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            d_Index,
            Sprite,
            SpecName,
            New,
            BST,
            CatchRate,
            Type1,
            Type2,
            HP,
            ATK,
            DEF,
            SPA,
            SPD,
            SPE,
            Ability0,
            Ability1,
            AbilityH});
            DGV.Dock = System.Windows.Forms.DockStyle.Fill;
            DGV.Location = new System.Drawing.Point(0, 0);
            DGV.Name = "DGV";
            DGV.Size = new System.Drawing.Size(1077, 615);
            DGV.TabIndex = 33;
            // 
            // d_Index
            // 
            d_Index.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            d_Index.HeaderText = "Dex#";
            d_Index.Name = "d_Index";
            d_Index.ReadOnly = true;
            d_Index.Width = 58;
            // 
            // Sprite
            // 
            Sprite.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            Sprite.HeaderText = "Sprite";
            Sprite.Name = "Sprite";
            Sprite.ReadOnly = true;
            Sprite.Width = 40;
            // 
            // SpecName
            // 
            SpecName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            SpecName.HeaderText = "Name";
            SpecName.Name = "SpecName";
            SpecName.ReadOnly = true;
            SpecName.Width = 60;
            // 
            // New
            // 
            New.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            New.HeaderText = "Native";
            New.Name = "New";
            New.ReadOnly = true;
            New.Width = 44;
            // 
            // BST
            // 
            BST.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            BST.HeaderText = "BST";
            BST.Name = "BST";
            BST.ReadOnly = true;
            BST.Width = 53;
            // 
            // CatchRate
            // 
            CatchRate.HeaderText = "Catch Rate";
            CatchRate.Name = "CatchRate";
            CatchRate.ReadOnly = true;
            CatchRate.Width = 85;
            // 
            // Type1
            // 
            Type1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            Type1.HeaderText = "Type1";
            Type1.Name = "Type1";
            Type1.ReadOnly = true;
            Type1.Width = 43;
            // 
            // Type2
            // 
            Type2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            Type2.HeaderText = "Type2";
            Type2.Name = "Type2";
            Type2.ReadOnly = true;
            Type2.Width = 43;
            // 
            // HP
            // 
            HP.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            HP.HeaderText = "HP";
            HP.Name = "HP";
            HP.ReadOnly = true;
            HP.Width = 47;
            // 
            // ATK
            // 
            ATK.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            ATK.HeaderText = "ATK";
            ATK.Name = "ATK";
            ATK.ReadOnly = true;
            ATK.Width = 53;
            // 
            // DEF
            // 
            DEF.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            DEF.HeaderText = "DEF";
            DEF.Name = "DEF";
            DEF.ReadOnly = true;
            DEF.Width = 53;
            // 
            // SPA
            // 
            SPA.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            SPA.HeaderText = "SPA";
            SPA.Name = "SPA";
            SPA.ReadOnly = true;
            SPA.Width = 53;
            // 
            // SPD
            // 
            SPD.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            SPD.HeaderText = "SPD";
            SPD.Name = "SPD";
            SPD.ReadOnly = true;
            SPD.Width = 54;
            // 
            // SPE
            // 
            SPE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            SPE.HeaderText = "SPE";
            SPE.Name = "SPE";
            SPE.ReadOnly = true;
            SPE.Width = 53;
            // 
            // Ability0
            // 
            Ability0.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            Ability0.HeaderText = "Ability 1";
            Ability0.Name = "Ability0";
            Ability0.ReadOnly = true;
            Ability0.Width = 68;
            // 
            // Ability1
            // 
            Ability1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            Ability1.HeaderText = "Ability 2";
            Ability1.Name = "Ability1";
            Ability1.ReadOnly = true;
            Ability1.Width = 68;
            // 
            // AbilityH
            // 
            AbilityH.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            AbilityH.HeaderText = "Hidden Ability";
            AbilityH.Name = "AbilityH";
            AbilityH.ReadOnly = true;
            AbilityH.Width = 96;
            // 
            // KChart
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(1077, 615);
            Controls.Add(DGV);
            MinimumSize = new System.Drawing.Size(1093, 654);
            Name = "KChart";
            Text = "KChart";
            ((System.ComponentModel.ISupportInitialize)(DGV)).EndInit();
            ResumeLayout(false);

        }

        #endregion
        private PKHeX.WinForms.Controls.DoubleBufferedDataGridView DGV;
        private System.Windows.Forms.DataGridViewTextBoxColumn d_Index;
        private System.Windows.Forms.DataGridViewImageColumn Sprite;
        private System.Windows.Forms.DataGridViewTextBoxColumn SpecName;
        private System.Windows.Forms.DataGridViewCheckBoxColumn New;
        private System.Windows.Forms.DataGridViewTextBoxColumn BST;
        private System.Windows.Forms.DataGridViewTextBoxColumn CatchRate;
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
