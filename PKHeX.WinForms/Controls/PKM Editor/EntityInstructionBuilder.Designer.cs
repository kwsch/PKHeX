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
            components = new System.ComponentModel.Container();
            L_PropValue = new System.Windows.Forms.Label();
            L_PropType = new System.Windows.Forms.Label();
            CB_Require = new System.Windows.Forms.ComboBox();
            CB_Property = new System.Windows.Forms.ComboBox();
            CB_Format = new System.Windows.Forms.ComboBox();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            toolTip2 = new System.Windows.Forms.ToolTip(components);
            toolTip3 = new System.Windows.Forms.ToolTip(components);
            SuspendLayout();
            // 
            // L_PropValue
            // 
            L_PropValue.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            L_PropValue.AutoSize = true;
            L_PropValue.Location = new System.Drawing.Point(200, 28);
            L_PropValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_PropValue.Name = "L_PropValue";
            L_PropValue.Size = new System.Drawing.Size(80, 15);
            L_PropValue.TabIndex = 18;
            L_PropValue.Text = "PropertyValue";
            // 
            // L_PropType
            // 
            L_PropType.AutoSize = true;
            L_PropType.Location = new System.Drawing.Point(55, 28);
            L_PropType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_PropType.Name = "L_PropType";
            L_PropType.Size = new System.Drawing.Size(76, 15);
            L_PropType.TabIndex = 17;
            L_PropType.Text = "PropertyType";
            // 
            // CB_Require
            // 
            CB_Require.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            CB_Require.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Require.FormattingEnabled = true;
            CB_Require.Items.AddRange(new object[] { "Set", "==", "!=", ">", ">=", "<", "<=" });
            CB_Require.Location = new System.Drawing.Point(203, 0);
            CB_Require.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Require.Name = "CB_Require";
            CB_Require.Size = new System.Drawing.Size(135, 23);
            CB_Require.TabIndex = 16;
            // 
            // CB_Property
            // 
            CB_Property.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            CB_Property.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Property.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Property.DropDownWidth = 200;
            CB_Property.FormattingEnabled = true;
            CB_Property.Location = new System.Drawing.Point(58, 0);
            CB_Property.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Property.Name = "CB_Property";
            CB_Property.Size = new System.Drawing.Size(137, 23);
            CB_Property.TabIndex = 15;
            CB_Property.SelectedIndexChanged += CB_Property_SelectedIndexChanged;
            // 
            // CB_Format
            // 
            CB_Format.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Format.FormattingEnabled = true;
            CB_Format.Location = new System.Drawing.Point(0, 0);
            CB_Format.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Format.Name = "CB_Format";
            CB_Format.Size = new System.Drawing.Size(51, 23);
            CB_Format.TabIndex = 14;
            CB_Format.SelectedIndexChanged += CB_Format_SelectedIndexChanged;
            // 
            // EntityInstructionBuilder
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(L_PropValue);
            Controls.Add(L_PropType);
            Controls.Add(CB_Require);
            Controls.Add(CB_Property);
            Controls.Add(CB_Format);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "EntityInstructionBuilder";
            Size = new System.Drawing.Size(338, 46);
            ResumeLayout(false);
            PerformLayout();
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
