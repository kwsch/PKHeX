using PKHeX.WinForms.Controls;

namespace PKHeX.WinForms
{
    partial class PokeathlonParticipant4Editor
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            TLP_Main = new System.Windows.Forms.TableLayoutPanel();
            UC_SpeciesForm = new PokeathlonSpeciesForm4Editor();
            FLP_Meta = new System.Windows.Forms.FlowLayoutPanel();
            GT_Gender = new GenderToggle();
            CHK_IsShiny = new System.Windows.Forms.CheckBox();
            L_PID = new System.Windows.Forms.Label();
            TB_PID = new System.Windows.Forms.TextBox();
            L_TID16 = new System.Windows.Forms.Label();
            TB_TID16 = new System.Windows.Forms.TextBox();
            L_SID16 = new System.Windows.Forms.Label();
            TB_SID16 = new System.Windows.Forms.TextBox();
            TLP_Main.SuspendLayout();
            FLP_Meta.SuspendLayout();
            SuspendLayout();
            // 
            // TLP_Main
            // 
            TLP_Main.AutoSize = true;
            TLP_Main.ColumnCount = 1;
            TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_Main.Controls.Add(UC_SpeciesForm, 0, 0);
            TLP_Main.Controls.Add(FLP_Meta, 0, 1);
            TLP_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Main.Location = new System.Drawing.Point(0, 0);
            TLP_Main.Margin = new System.Windows.Forms.Padding(0);
            TLP_Main.Name = "TLP_Main";
            TLP_Main.RowCount = 2;
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.Size = new System.Drawing.Size(572, 76);
            TLP_Main.TabIndex = 0;
            // 
            // UC_SpeciesForm
            // 
            UC_SpeciesForm.DisplayGender = 0;
            UC_SpeciesForm.DisplayShiny = false;
            UC_SpeciesForm.Dock = System.Windows.Forms.DockStyle.Fill;
            UC_SpeciesForm.Location = new System.Drawing.Point(0, 0);
            UC_SpeciesForm.Margin = new System.Windows.Forms.Padding(0);
            UC_SpeciesForm.Name = "UC_SpeciesForm";
            UC_SpeciesForm.Size = new System.Drawing.Size(572, 40);
            UC_SpeciesForm.TabIndex = 0;
            // 
            // FLP_Meta
            // 
            FLP_Meta.AutoSize = true;
            FLP_Meta.Controls.Add(GT_Gender);
            FLP_Meta.Controls.Add(CHK_IsShiny);
            FLP_Meta.Controls.Add(L_PID);
            FLP_Meta.Controls.Add(TB_PID);
            FLP_Meta.Controls.Add(L_TID16);
            FLP_Meta.Controls.Add(TB_TID16);
            FLP_Meta.Controls.Add(L_SID16);
            FLP_Meta.Controls.Add(TB_SID16);
            FLP_Meta.Dock = System.Windows.Forms.DockStyle.Fill;
            FLP_Meta.Location = new System.Drawing.Point(0, 40);
            FLP_Meta.Margin = new System.Windows.Forms.Padding(0);
            FLP_Meta.Name = "FLP_Meta";
            FLP_Meta.Size = new System.Drawing.Size(572, 36);
            FLP_Meta.TabIndex = 1;
            FLP_Meta.WrapContents = false;
            // 
            // GT_Gender
            // 
            GT_Gender.AllowClick = true;
            GT_Gender.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            GT_Gender.Gender = 255;
            GT_Gender.Location = new System.Drawing.Point(0, 4);
            GT_Gender.Margin = new System.Windows.Forms.Padding(0, 4, 6, 0);
            GT_Gender.Name = "GT_Gender";
            GT_Gender.Size = new System.Drawing.Size(18, 18);
            GT_Gender.TabIndex = 0;
            // 
            // CHK_IsShiny
            // 
            CHK_IsShiny.AutoSize = true;
            CHK_IsShiny.Location = new System.Drawing.Point(24, 2);
            CHK_IsShiny.Margin = new System.Windows.Forms.Padding(0, 2, 12, 0);
            CHK_IsShiny.Name = "CHK_IsShiny";
            CHK_IsShiny.Size = new System.Drawing.Size(57, 21);
            CHK_IsShiny.TabIndex = 1;
            CHK_IsShiny.Text = "Shiny";
            CHK_IsShiny.UseVisualStyleBackColor = true;
            // 
            // L_PID
            // 
            L_PID.Anchor = System.Windows.Forms.AnchorStyles.Left;
            L_PID.AutoSize = true;
            L_PID.Location = new System.Drawing.Point(96, 4);
            L_PID.Name = "L_PID";
            L_PID.Size = new System.Drawing.Size(26, 17);
            L_PID.TabIndex = 2;
            L_PID.Text = "PID:";
            // 
            // TB_PID
            // 
            TB_PID.Location = new System.Drawing.Point(128, 0);
            TB_PID.Margin = new System.Windows.Forms.Padding(3, 0, 12, 0);
            TB_PID.MaxLength = 8;
            TB_PID.Name = "TB_PID";
            TB_PID.Size = new System.Drawing.Size(86, 25);
            TB_PID.TabIndex = 3;
            // 
            // L_TID16
            // 
            L_TID16.Anchor = System.Windows.Forms.AnchorStyles.Left;
            L_TID16.AutoSize = true;
            L_TID16.Location = new System.Drawing.Point(229, 4);
            L_TID16.Name = "L_TID16";
            L_TID16.Size = new System.Drawing.Size(30, 17);
            L_TID16.TabIndex = 4;
            L_TID16.Text = "TID:";
            // 
            // TB_TID16
            // 
            TB_TID16.Location = new System.Drawing.Point(265, 0);
            TB_TID16.Margin = new System.Windows.Forms.Padding(3, 0, 12, 0);
            TB_TID16.Name = "TB_TID16";
            TB_TID16.Size = new System.Drawing.Size(68, 25);
            TB_TID16.TabIndex = 5;
            // 
            // L_SID16
            // 
            L_SID16.Anchor = System.Windows.Forms.AnchorStyles.Left;
            L_SID16.AutoSize = true;
            L_SID16.Location = new System.Drawing.Point(348, 4);
            L_SID16.Name = "L_SID16";
            L_SID16.Size = new System.Drawing.Size(30, 17);
            L_SID16.TabIndex = 6;
            L_SID16.Text = "SID:";
            // 
            // TB_SID16
            // 
            TB_SID16.Location = new System.Drawing.Point(384, 0);
            TB_SID16.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            TB_SID16.Name = "TB_SID16";
            TB_SID16.Size = new System.Drawing.Size(68, 25);
            TB_SID16.TabIndex = 7;
            // 
            // PokeathlonParticipant4Editor
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            AutoSize = true;
            Controls.Add(TLP_Main);
            Name = "PokeathlonParticipant4Editor";
            Size = new System.Drawing.Size(572, 76);
            TLP_Main.ResumeLayout(false);
            TLP_Main.PerformLayout();
            FLP_Meta.ResumeLayout(false);
            FLP_Meta.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TLP_Main;
        private PokeathlonSpeciesForm4Editor UC_SpeciesForm;
        private GenderToggle GT_Gender;
        private System.Windows.Forms.CheckBox CHK_IsShiny;
        private System.Windows.Forms.FlowLayoutPanel FLP_Meta;
        private System.Windows.Forms.Label L_PID;
        private System.Windows.Forms.TextBox TB_PID;
        private System.Windows.Forms.Label L_TID16;
        private System.Windows.Forms.TextBox TB_TID16;
        private System.Windows.Forms.Label L_SID16;
        private System.Windows.Forms.TextBox TB_SID16;
    }
}
