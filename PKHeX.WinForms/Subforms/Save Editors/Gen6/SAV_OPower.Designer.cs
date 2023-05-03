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
            B_Cancel = new System.Windows.Forms.Button();
            B_Save = new System.Windows.Forms.Button();
            L_Type = new System.Windows.Forms.Label();
            CB_Type = new System.Windows.Forms.ComboBox();
            CB_Value = new System.Windows.Forms.ComboBox();
            B_ClearAll = new System.Windows.Forms.Button();
            B_GiveAll = new System.Windows.Forms.Button();
            B_GiveAllMAX = new System.Windows.Forms.Button();
            CHK_Master = new System.Windows.Forms.CheckBox();
            CHK_S = new System.Windows.Forms.CheckBox();
            CHK_MAX = new System.Windows.Forms.CheckBox();
            SuspendLayout();
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(250, 82);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(75, 27);
            B_Cancel.TabIndex = 34;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(331, 82);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(75, 27);
            B_Save.TabIndex = 35;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // L_Type
            // 
            L_Type.Location = new System.Drawing.Point(10, 15);
            L_Type.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_Type.Name = "L_Type";
            L_Type.Size = new System.Drawing.Size(58, 23);
            L_Type.TabIndex = 36;
            L_Type.Text = "Type:";
            L_Type.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CB_Type
            // 
            CB_Type.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            CB_Type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Type.FormattingEnabled = true;
            CB_Type.Location = new System.Drawing.Point(76, 16);
            CB_Type.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Type.Name = "CB_Type";
            CB_Type.Size = new System.Drawing.Size(119, 23);
            CB_Type.TabIndex = 37;
            // 
            // CB_Value
            // 
            CB_Value.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            CB_Value.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Value.FormattingEnabled = true;
            CB_Value.Location = new System.Drawing.Point(203, 16);
            CB_Value.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Value.Name = "CB_Value";
            CB_Value.Size = new System.Drawing.Size(74, 23);
            CB_Value.TabIndex = 38;
            // 
            // B_ClearAll
            // 
            B_ClearAll.Location = new System.Drawing.Point(14, 82);
            B_ClearAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ClearAll.Name = "B_ClearAll";
            B_ClearAll.Size = new System.Drawing.Size(88, 27);
            B_ClearAll.TabIndex = 39;
            B_ClearAll.Text = "Clear All";
            B_ClearAll.UseVisualStyleBackColor = true;
            // 
            // B_GiveAll
            // 
            B_GiveAll.Location = new System.Drawing.Point(14, 47);
            B_GiveAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_GiveAll.Name = "B_GiveAll";
            B_GiveAll.Size = new System.Drawing.Size(88, 27);
            B_GiveAll.TabIndex = 40;
            B_GiveAll.Text = "Give All";
            B_GiveAll.UseVisualStyleBackColor = true;
            // 
            // B_GiveAllMAX
            // 
            B_GiveAllMAX.Location = new System.Drawing.Point(108, 47);
            B_GiveAllMAX.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_GiveAllMAX.Name = "B_GiveAllMAX";
            B_GiveAllMAX.Size = new System.Drawing.Size(88, 27);
            B_GiveAllMAX.TabIndex = 41;
            B_GiveAllMAX.Text = "MAX All";
            B_GiveAllMAX.UseVisualStyleBackColor = true;
            // 
            // CHK_Master
            // 
            CHK_Master.AutoSize = true;
            CHK_Master.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            CHK_Master.Location = new System.Drawing.Point(118, 87);
            CHK_Master.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_Master.Name = "CHK_Master";
            CHK_Master.Size = new System.Drawing.Size(66, 19);
            CHK_Master.TabIndex = 42;
            CHK_Master.Text = "??? Flag";
            CHK_Master.UseVisualStyleBackColor = true;
            // 
            // CHK_S
            // 
            CHK_S.AutoSize = true;
            CHK_S.Location = new System.Drawing.Point(285, 18);
            CHK_S.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_S.Name = "CHK_S";
            CHK_S.Size = new System.Drawing.Size(32, 19);
            CHK_S.TabIndex = 43;
            CHK_S.Text = "S";
            CHK_S.UseVisualStyleBackColor = true;
            // 
            // CHK_MAX
            // 
            CHK_MAX.AutoSize = true;
            CHK_MAX.Location = new System.Drawing.Point(330, 18);
            CHK_MAX.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CHK_MAX.Name = "CHK_MAX";
            CHK_MAX.Size = new System.Drawing.Size(52, 19);
            CHK_MAX.TabIndex = 44;
            CHK_MAX.Text = "MAX";
            CHK_MAX.UseVisualStyleBackColor = true;
            // 
            // SAV_OPower
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(414, 122);
            Controls.Add(CHK_MAX);
            Controls.Add(CHK_S);
            Controls.Add(CHK_Master);
            Controls.Add(B_GiveAllMAX);
            Controls.Add(B_GiveAll);
            Controls.Add(B_ClearAll);
            Controls.Add(CB_Value);
            Controls.Add(CB_Type);
            Controls.Add(L_Type);
            Controls.Add(B_Save);
            Controls.Add(B_Cancel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_OPower";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "O-Power Editor";
            ResumeLayout(false);
            PerformLayout();
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
