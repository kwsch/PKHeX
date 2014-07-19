namespace PKHeX
{
    partial class CodeGenerator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CodeGenerator));
            this.RTB_Code = new System.Windows.Forms.RichTextBox();
            this.TB_Write = new System.Windows.Forms.TextBox();
            this.L_Write = new System.Windows.Forms.Label();
            this.L_Source = new System.Windows.Forms.Label();
            this.CB_Source = new System.Windows.Forms.ComboBox();
            this.B_CnE = new System.Windows.Forms.Button();
            this.L_Box = new System.Windows.Forms.Label();
            this.L_Slot = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.L_Info = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // RTB_Code
            // 
            this.RTB_Code.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RTB_Code.Location = new System.Drawing.Point(12, 71);
            this.RTB_Code.Name = "RTB_Code";
            this.RTB_Code.ReadOnly = true;
            this.RTB_Code.Size = new System.Drawing.Size(210, 175);
            this.RTB_Code.TabIndex = 0;
            this.RTB_Code.Text = "01234567 01234567 01234567 ";
            // 
            // TB_Write
            // 
            this.TB_Write.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_Write.Location = new System.Drawing.Point(12, 24);
            this.TB_Write.Name = "TB_Write";
            this.TB_Write.Size = new System.Drawing.Size(65, 20);
            this.TB_Write.TabIndex = 1;
            this.TB_Write.Text = "01234567";
            // 
            // L_Write
            // 
            this.L_Write.AutoSize = true;
            this.L_Write.Location = new System.Drawing.Point(9, 8);
            this.L_Write.Name = "L_Write";
            this.L_Write.Size = new System.Drawing.Size(66, 13);
            this.L_Write.TabIndex = 2;
            this.L_Write.Text = "Write Offset:";
            // 
            // L_Source
            // 
            this.L_Source.AutoSize = true;
            this.L_Source.Location = new System.Drawing.Point(87, 8);
            this.L_Source.Name = "L_Source";
            this.L_Source.Size = new System.Drawing.Size(70, 13);
            this.L_Source.TabIndex = 5;
            this.L_Source.Text = "Data Source:";
            // 
            // CB_Source
            // 
            this.CB_Source.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Source.FormattingEnabled = true;
            this.CB_Source.Items.AddRange(new object[] {
            "Loaded EKX (Tabs)",
            "Box EKX ",
            "Wondercard"});
            this.CB_Source.Location = new System.Drawing.Point(90, 23);
            this.CB_Source.Name = "CB_Source";
            this.CB_Source.Size = new System.Drawing.Size(132, 21);
            this.CB_Source.TabIndex = 6;
            // 
            // B_CnE
            // 
            this.B_CnE.Location = new System.Drawing.Point(37, 252);
            this.B_CnE.Name = "B_CnE";
            this.B_CnE.Size = new System.Drawing.Size(157, 23);
            this.B_CnE.TabIndex = 9;
            this.B_CnE.Text = "Create && Export Code File";
            this.B_CnE.UseVisualStyleBackColor = true;
            // 
            // L_Box
            // 
            this.L_Box.AutoSize = true;
            this.L_Box.Location = new System.Drawing.Point(87, 48);
            this.L_Box.Name = "L_Box";
            this.L_Box.Size = new System.Drawing.Size(28, 13);
            this.L_Box.TabIndex = 10;
            this.L_Box.Text = "Box:";
            // 
            // L_Slot
            // 
            this.L_Slot.AutoSize = true;
            this.L_Slot.Location = new System.Drawing.Point(156, 48);
            this.L_Slot.Name = "L_Slot";
            this.L_Slot.Size = new System.Drawing.Size(28, 13);
            this.L_Slot.TabIndex = 11;
            this.L_Slot.Text = "Slot:";
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "Loaded EKX (Tabs)",
            "Box EKX ",
            "Wondercard"});
            this.comboBox2.Location = new System.Drawing.Point(187, 44);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(35, 21);
            this.comboBox2.TabIndex = 12;
            // 
            // comboBox3
            // 
            this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Items.AddRange(new object[] {
            "Loaded EKX (Tabs)",
            "Box EKX ",
            "Wondercard"});
            this.comboBox3.Location = new System.Drawing.Point(115, 44);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(35, 21);
            this.comboBox3.TabIndex = 13;
            // 
            // L_Info
            // 
            this.L_Info.AutoSize = true;
            this.L_Info.Location = new System.Drawing.Point(12, 48);
            this.L_Info.Name = "L_Info";
            this.L_Info.Size = new System.Drawing.Size(31, 13);
            this.L_Info.TabIndex = 14;
            this.L_Info.Text = "(Info)";
            // 
            // CodeGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(230, 282);
            this.Controls.Add(this.L_Info);
            this.Controls.Add(this.comboBox3);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.L_Slot);
            this.Controls.Add(this.L_Box);
            this.Controls.Add(this.B_CnE);
            this.Controls.Add(this.CB_Source);
            this.Controls.Add(this.L_Source);
            this.Controls.Add(this.L_Write);
            this.Controls.Add(this.TB_Write);
            this.Controls.Add(this.RTB_Code);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CodeGenerator";
            this.Text = "CodeGenerator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox RTB_Code;
        private System.Windows.Forms.TextBox TB_Write;
        private System.Windows.Forms.Label L_Write;
        private System.Windows.Forms.Label L_Source;
        private System.Windows.Forms.ComboBox CB_Source;
        private System.Windows.Forms.Button B_CnE;
        private System.Windows.Forms.Label L_Box;
        private System.Windows.Forms.Label L_Slot;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.Label L_Info;
    }
}