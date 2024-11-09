namespace PKHeX.WinForms
{
    partial class SAV_SecretBase3
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SAV_SecretBase3));
            LB_Bases = new System.Windows.Forms.ListBox();
            L_Trainers = new System.Windows.Forms.Label();
            T_TrainerGender = new Controls.GenderToggle();
            TB_Name = new System.Windows.Forms.TextBox();
            SuspendLayout();
            // 
            // LB_Bases
            // 
            LB_Bases.FormattingEnabled = true;
            LB_Bases.ItemHeight = 15;
            LB_Bases.Location = new System.Drawing.Point(12, 26);
            LB_Bases.Name = "LB_Bases";
            LB_Bases.Size = new System.Drawing.Size(125, 409);
            LB_Bases.TabIndex = 0;
            // 
            // L_Trainers
            // 
            L_Trainers.AutoSize = true;
            L_Trainers.Location = new System.Drawing.Point(12, 9);
            L_Trainers.Name = "L_Trainers";
            L_Trainers.Size = new System.Drawing.Size(50, 15);
            L_Trainers.TabIndex = 1;
            L_Trainers.Text = "Trainers:";
            // 
            // T_TrainerGender
            // 
            T_TrainerGender.AccessibleDescription = " (2) (2)";
            T_TrainerGender.AccessibleName = " (2)";
            T_TrainerGender.AllowClick = true;
            T_TrainerGender.BackgroundImage = (System.Drawing.Image)resources.GetObject("T_TrainerGender.BackgroundImage");
            T_TrainerGender.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            T_TrainerGender.Gender = 2;
            T_TrainerGender.Location = new System.Drawing.Point(282, 31);
            T_TrainerGender.Name = "T_TrainerGender";
            T_TrainerGender.Size = new System.Drawing.Size(18, 18);
            T_TrainerGender.TabIndex = 2;
            // 
            // TB_Name
            // 
            TB_Name.Location = new System.Drawing.Point(154, 26);
            TB_Name.Name = "TB_Name";
            TB_Name.Size = new System.Drawing.Size(122, 23);
            TB_Name.TabIndex = 3;
            // 
            // SAV_SecretBase3
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(TB_Name);
            Controls.Add(T_TrainerGender);
            Controls.Add(L_Trainers);
            Controls.Add(LB_Bases);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "SAV_SecretBase3";
            Text = "SAV_SecretBase3";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListBox LB_Bases;
        private System.Windows.Forms.Label L_Trainers;
        private Controls.GenderToggle T_TrainerGender;
        private System.Windows.Forms.TextBox TB_Name;
    }
}
