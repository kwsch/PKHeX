namespace PKHeX.WinForms.Controls
{
    partial class EntityInstructionBuilder
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
            this.components = new System.ComponentModel.Container();
            this.L_PropValue = new System.Windows.Forms.Label();
            this.L_PropType = new System.Windows.Forms.Label();
            this.CB_Require = new System.Windows.Forms.ComboBox();
            this.CB_Property = new System.Windows.Forms.ComboBox();
            this.CB_Format = new System.Windows.Forms.ComboBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip3 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // L_PropValue
            // 
            this.L_PropValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_PropValue.AutoSize = true;
            this.L_PropValue.Location = new System.Drawing.Point(171, 24);
            this.L_PropValue.Name = "L_PropValue";
            this.L_PropValue.Size = new System.Drawing.Size(73, 13);
            this.L_PropValue.TabIndex = 18;
            this.L_PropValue.Text = "PropertyValue";
            // 
            // L_PropType
            // 
            this.L_PropType.AutoSize = true;
            this.L_PropType.Location = new System.Drawing.Point(47, 24);
            this.L_PropType.Name = "L_PropType";
            this.L_PropType.Size = new System.Drawing.Size(70, 13);
            this.L_PropType.TabIndex = 17;
            this.L_PropType.Text = "PropertyType";
            // 
            // CB_Require
            // 
            this.CB_Require.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_Require.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Require.FormattingEnabled = true;
            this.CB_Require.Items.AddRange(new object[] {
            "Set Equal To",
            "Require Equals",
            "Require Not Equals"});
            this.CB_Require.Location = new System.Drawing.Point(174, 0);
            this.CB_Require.Name = "CB_Require";
            this.CB_Require.Size = new System.Drawing.Size(116, 21);
            this.CB_Require.TabIndex = 16;
            // 
            // CB_Property
            // 
            this.CB_Property.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_Property.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_Property.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Property.DropDownWidth = 200;
            this.CB_Property.FormattingEnabled = true;
            this.CB_Property.Location = new System.Drawing.Point(50, 0);
            this.CB_Property.Name = "CB_Property";
            this.CB_Property.Size = new System.Drawing.Size(118, 21);
            this.CB_Property.TabIndex = 15;
            this.CB_Property.SelectedIndexChanged += new System.EventHandler(this.CB_Property_SelectedIndexChanged);
            // 
            // CB_Format
            // 
            this.CB_Format.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Format.FormattingEnabled = true;
            this.CB_Format.Location = new System.Drawing.Point(0, 0);
            this.CB_Format.Name = "CB_Format";
            this.CB_Format.Size = new System.Drawing.Size(44, 21);
            this.CB_Format.TabIndex = 14;
            this.CB_Format.SelectedIndexChanged += new System.EventHandler(this.CB_Format_SelectedIndexChanged);
            // 
            // EntityInstructionBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.L_PropValue);
            this.Controls.Add(this.L_PropType);
            this.Controls.Add(this.CB_Require);
            this.Controls.Add(this.CB_Property);
            this.Controls.Add(this.CB_Format);
            this.Name = "EntityInstructionBuilder";
            this.Size = new System.Drawing.Size(290, 40);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label L_PropValue;
        private System.Windows.Forms.Label L_PropType;
        private System.Windows.Forms.ComboBox CB_Require;
        private System.Windows.Forms.ComboBox CB_Property;
        private System.Windows.Forms.ComboBox CB_Format;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolTip toolTip2;
        private System.Windows.Forms.ToolTip toolTip3;
    }
}
