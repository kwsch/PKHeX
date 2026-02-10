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
            B_Require = new System.Windows.Forms.Button();
            CB_Property = new System.Windows.Forms.ComboBox();
            CB_Format = new System.Windows.Forms.ComboBox();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            toolTip2 = new System.Windows.Forms.ToolTip(components);
            toolTip3 = new System.Windows.Forms.ToolTip(components);
            requireMenu = new System.Windows.Forms.ContextMenuStrip(components);
            SuspendLayout();
            // 
            // L_PropValue
            // 
            L_PropValue.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            L_PropValue.AutoSize = true;
            L_PropValue.Location = new System.Drawing.Point(236, 30);
            L_PropValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_PropValue.Name = "L_PropValue";
            L_PropValue.Size = new System.Drawing.Size(89, 17);
            L_PropValue.TabIndex = 18;
            L_PropValue.Text = "PropertyValue";
            // 
            // L_PropType
            // 
            L_PropType.AutoSize = true;
            L_PropType.Location = new System.Drawing.Point(55, 28);
            L_PropType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_PropType.Name = "L_PropType";
            L_PropType.Size = new System.Drawing.Size(85, 17);
            L_PropType.TabIndex = 17;
            L_PropType.Text = "PropertyType";
            // 
            // B_Require
            // 
            B_Require.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            B_Require.Location = new System.Drawing.Point(236, 0);
            B_Require.Margin = new System.Windows.Forms.Padding(4);
            B_Require.Name = "B_Require";
            B_Require.Size = new System.Drawing.Size(40, 25);
            B_Require.TabIndex = 2;
            B_Require.Text = "Set";
            B_Require.UseVisualStyleBackColor = true;
            B_Require.Click += B_Require_Click;
            // 
            // CB_Property
            // 
            CB_Property.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            CB_Property.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            CB_Property.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Property.DropDownWidth = 200;
            CB_Property.FormattingEnabled = true;
            CB_Property.Location = new System.Drawing.Point(56, 0);
            CB_Property.Margin = new System.Windows.Forms.Padding(4);
            CB_Property.Name = "CB_Property";
            CB_Property.Size = new System.Drawing.Size(176, 25);
            CB_Property.TabIndex = 1;
            CB_Property.SelectedIndexChanged += CB_Property_SelectedIndexChanged;
            // 
            // CB_Format
            // 
            CB_Format.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Format.FormattingEnabled = true;
            CB_Format.Location = new System.Drawing.Point(0, 0);
            CB_Format.Margin = new System.Windows.Forms.Padding(4);
            CB_Format.Name = "CB_Format";
            CB_Format.Size = new System.Drawing.Size(52, 25);
            CB_Format.TabIndex = 0;
            CB_Format.SelectedIndexChanged += CB_Format_SelectedIndexChanged;
            // 
            // requireMenu
            // 
            requireMenu.Name = "requireMenu";
            requireMenu.Size = new System.Drawing.Size(181, 26);
            // 
            // EntityInstructionBuilder
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(L_PropValue);
            Controls.Add(L_PropType);
            Controls.Add(B_Require);
            Controls.Add(CB_Property);
            Controls.Add(CB_Format);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "EntityInstructionBuilder";
            Size = new System.Drawing.Size(360, 46);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label L_PropValue;
        private System.Windows.Forms.Label L_PropType;
        private System.Windows.Forms.Button B_Require;
        private System.Windows.Forms.ComboBox CB_Property;
        private System.Windows.Forms.ComboBox CB_Format;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolTip toolTip2;
        private System.Windows.Forms.ToolTip toolTip3;
        private System.Windows.Forms.ContextMenuStrip requireMenu;
    }
}
