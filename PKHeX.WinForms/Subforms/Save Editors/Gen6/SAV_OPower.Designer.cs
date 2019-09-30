namespace PKHeX.WinForms
{
    partial class SAV_OPower
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
            this.B_Cancel = new System.Windows.Forms.Button();
            this.B_Save = new System.Windows.Forms.Button();
            this.L_Type = new System.Windows.Forms.Label();
            this.CB_Type = new System.Windows.Forms.ComboBox();
            this.CB_Value = new System.Windows.Forms.ComboBox();
            this.B_ClearAll = new System.Windows.Forms.Button();
            this.B_GiveAll = new System.Windows.Forms.Button();
            this.B_GiveAllMAX = new System.Windows.Forms.Button();
            this.CHK_Master = new System.Windows.Forms.CheckBox();
            this.CHK_S = new System.Windows.Forms.CheckBox();
            this.CHK_MAX = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(214, 71);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(64, 23);
            this.B_Cancel.TabIndex = 34;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(284, 71);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(64, 23);
            this.B_Save.TabIndex = 35;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // L_Type
            // 
            this.L_Type.Location = new System.Drawing.Point(9, 13);
            this.L_Type.Name = "L_Type";
            this.L_Type.Size = new System.Drawing.Size(50, 20);
            this.L_Type.TabIndex = 36;
            this.L_Type.Text = "Type:";
            this.L_Type.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_Type
            // 
            this.CB_Type.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_Type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Type.FormattingEnabled = true;
            this.CB_Type.Location = new System.Drawing.Point(65, 14);
            this.CB_Type.Name = "CB_Type";
            this.CB_Type.Size = new System.Drawing.Size(103, 21);
            this.CB_Type.TabIndex = 37;
            // 
            // CB_Value
            // 
            this.CB_Value.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_Value.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Value.FormattingEnabled = true;
            this.CB_Value.Location = new System.Drawing.Point(174, 14);
            this.CB_Value.Name = "CB_Value";
            this.CB_Value.Size = new System.Drawing.Size(64, 21);
            this.CB_Value.TabIndex = 38;
            // 
            // B_ClearAll
            // 
            this.B_ClearAll.Location = new System.Drawing.Point(12, 71);
            this.B_ClearAll.Name = "B_ClearAll";
            this.B_ClearAll.Size = new System.Drawing.Size(75, 23);
            this.B_ClearAll.TabIndex = 39;
            this.B_ClearAll.Text = "Clear All";
            this.B_ClearAll.UseVisualStyleBackColor = true;
            // 
            // B_GiveAll
            // 
            this.B_GiveAll.Location = new System.Drawing.Point(12, 41);
            this.B_GiveAll.Name = "B_GiveAll";
            this.B_GiveAll.Size = new System.Drawing.Size(75, 23);
            this.B_GiveAll.TabIndex = 40;
            this.B_GiveAll.Text = "Give All";
            this.B_GiveAll.UseVisualStyleBackColor = true;
            // 
            // B_GiveAllMAX
            // 
            this.B_GiveAllMAX.Location = new System.Drawing.Point(93, 41);
            this.B_GiveAllMAX.Name = "B_GiveAllMAX";
            this.B_GiveAllMAX.Size = new System.Drawing.Size(75, 23);
            this.B_GiveAllMAX.TabIndex = 41;
            this.B_GiveAllMAX.Text = "MAX All";
            this.B_GiveAllMAX.UseVisualStyleBackColor = true;
            // 
            // CHK_Master
            // 
            this.CHK_Master.AutoSize = true;
            this.CHK_Master.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CHK_Master.Location = new System.Drawing.Point(101, 75);
            this.CHK_Master.Name = "CHK_Master";
            this.CHK_Master.Size = new System.Drawing.Size(67, 17);
            this.CHK_Master.TabIndex = 42;
            this.CHK_Master.Text = "??? Flag";
            this.CHK_Master.UseVisualStyleBackColor = true;
            // 
            // CHK_S
            // 
            this.CHK_S.AutoSize = true;
            this.CHK_S.Location = new System.Drawing.Point(244, 16);
            this.CHK_S.Name = "CHK_S";
            this.CHK_S.Size = new System.Drawing.Size(33, 17);
            this.CHK_S.TabIndex = 43;
            this.CHK_S.Text = "S";
            this.CHK_S.UseVisualStyleBackColor = true;
            // 
            // CHK_MAX
            // 
            this.CHK_MAX.AutoSize = true;
            this.CHK_MAX.Location = new System.Drawing.Point(283, 16);
            this.CHK_MAX.Name = "CHK_MAX";
            this.CHK_MAX.Size = new System.Drawing.Size(49, 17);
            this.CHK_MAX.TabIndex = 44;
            this.CHK_MAX.Text = "MAX";
            this.CHK_MAX.UseVisualStyleBackColor = true;
            // 
            // SAV_OPower
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(355, 106);
            this.Controls.Add(this.CHK_MAX);
            this.Controls.Add(this.CHK_S);
            this.Controls.Add(this.CHK_Master);
            this.Controls.Add(this.B_GiveAllMAX);
            this.Controls.Add(this.B_GiveAll);
            this.Controls.Add(this.B_ClearAll);
            this.Controls.Add(this.CB_Value);
            this.Controls.Add(this.CB_Type);
            this.Controls.Add(this.L_Type);
            this.Controls.Add(this.B_Save);
            this.Controls.Add(this.B_Cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SAV_OPower";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "O-Power Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Label L_Type;
        private System.Windows.Forms.ComboBox CB_Type;
        private System.Windows.Forms.ComboBox CB_Value;
        private System.Windows.Forms.Button B_ClearAll;
        private System.Windows.Forms.Button B_GiveAll;
        private System.Windows.Forms.Button B_GiveAllMAX;
        private System.Windows.Forms.CheckBox CHK_Master;
        private System.Windows.Forms.CheckBox CHK_S;
        private System.Windows.Forms.CheckBox CHK_MAX;
    }
}