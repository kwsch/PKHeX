namespace PKHeX.WinForms
{
    partial class PokeathlonSpeciesForm4Editor
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
            PB_Sprite = new System.Windows.Forms.PictureBox();
            CB_Species = new System.Windows.Forms.ComboBox();
            CB_Form = new System.Windows.Forms.ComboBox();
            TLP_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PB_Sprite).BeginInit();
            SuspendLayout();
            // 
            // TLP_Main
            // 
            TLP_Main.AutoSize = true;
            TLP_Main.ColumnCount = 3;
            TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Main.Controls.Add(PB_Sprite, 0, 0);
            TLP_Main.Controls.Add(CB_Species, 1, 0);
            TLP_Main.Controls.Add(CB_Form, 2, 0);
            TLP_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Main.Location = new System.Drawing.Point(0, 0);
            TLP_Main.Margin = new System.Windows.Forms.Padding(0);
            TLP_Main.Name = "TLP_Main";
            TLP_Main.RowCount = 1;
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_Main.Size = new System.Drawing.Size(420, 40);
            TLP_Main.TabIndex = 0;
            // 
            // PB_Sprite
            // 
            PB_Sprite.Dock = System.Windows.Forms.DockStyle.Fill;
            PB_Sprite.Location = new System.Drawing.Point(0, 0);
            PB_Sprite.Margin = new System.Windows.Forms.Padding(0);
            PB_Sprite.Name = "PB_Sprite";
            PB_Sprite.Size = new System.Drawing.Size(40, 40);
            PB_Sprite.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            PB_Sprite.TabIndex = 0;
            PB_Sprite.TabStop = false;
            // 
            // CB_Species
            // 
            CB_Species.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_Species.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Species.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Species.FormattingEnabled = true;
            CB_Species.Location = new System.Drawing.Point(43, 6);
            CB_Species.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            CB_Species.Name = "CB_Species";
            CB_Species.Size = new System.Drawing.Size(120, 25);
            CB_Species.TabIndex = 1;
            // 
            // CB_Form
            // 
            CB_Form.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CB_Form.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Form.FormattingEnabled = true;
            CB_Form.Location = new System.Drawing.Point(169, 6);
            CB_Form.Margin = new System.Windows.Forms.Padding(3, 6, 0, 6);
            CB_Form.Name = "CB_Form";
            CB_Form.Size = new System.Drawing.Size(80, 25);
            CB_Form.TabIndex = 2;
            // 
            // PokeathlonSpeciesForm4Editor
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(TLP_Main);
            Name = "PokeathlonSpeciesForm4Editor";
            Size = new System.Drawing.Size(249, 40);
            TLP_Main.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PB_Sprite).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TLP_Main;
        private System.Windows.Forms.PictureBox PB_Sprite;
        private System.Windows.Forms.ComboBox CB_Species;
        private System.Windows.Forms.ComboBox CB_Form;
    }
}
