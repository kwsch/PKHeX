namespace PKHeX.WinForms
{
    partial class SAV_Accessor<T>
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
            CB_Key = new System.Windows.Forms.ComboBox();
            L_Key = new System.Windows.Forms.Label();
            TC_Tabs = new System.Windows.Forms.TabControl();
            Tab_Blocks = new System.Windows.Forms.TabPage();
            PG_BlockView = new System.Windows.Forms.PropertyGrid();
            Raw = new System.Windows.Forms.TabPage();
            propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            TC_Tabs.SuspendLayout();
            Tab_Blocks.SuspendLayout();
            Raw.SuspendLayout();
            SuspendLayout();
            // 
            // CB_Key
            // 
            CB_Key.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            CB_Key.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            CB_Key.DropDownWidth = 270;
            CB_Key.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            CB_Key.FormattingEnabled = true;
            CB_Key.Location = new System.Drawing.Point(83, 9);
            CB_Key.Name = "CB_Key";
            CB_Key.Size = new System.Drawing.Size(183, 22);
            CB_Key.TabIndex = 0;
            CB_Key.SelectedIndexChanged += new System.EventHandler(CB_Key_SelectedIndexChanged);
            // 
            // L_Key
            // 
            L_Key.AutoSize = true;
            L_Key.Location = new System.Drawing.Point(6, 12);
            L_Key.Name = "L_Key";
            L_Key.Size = new System.Drawing.Size(37, 13);
            L_Key.TabIndex = 1;
            L_Key.Text = "Block:";
            // 
            // TC_Tabs
            // 
            TC_Tabs.Controls.Add(Tab_Blocks);
            TC_Tabs.Controls.Add(Raw);
            TC_Tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            TC_Tabs.Location = new System.Drawing.Point(0, 0);
            TC_Tabs.Name = "TC_Tabs";
            TC_Tabs.SelectedIndex = 0;
            TC_Tabs.Size = new System.Drawing.Size(334, 341);
            TC_Tabs.TabIndex = 12;
            // 
            // Tab_Blocks
            // 
            Tab_Blocks.Controls.Add(PG_BlockView);
            Tab_Blocks.Controls.Add(L_Key);
            Tab_Blocks.Controls.Add(CB_Key);
            Tab_Blocks.Location = new System.Drawing.Point(4, 22);
            Tab_Blocks.Name = "Tab_Blocks";
            Tab_Blocks.Padding = new System.Windows.Forms.Padding(3);
            Tab_Blocks.Size = new System.Drawing.Size(326, 315);
            Tab_Blocks.TabIndex = 0;
            Tab_Blocks.Text = "Blocks";
            Tab_Blocks.UseVisualStyleBackColor = true;
            // 
            // PG_BlockView
            // 
            PG_BlockView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            PG_BlockView.Location = new System.Drawing.Point(3, 37);
            PG_BlockView.Name = "PG_BlockView";
            PG_BlockView.Size = new System.Drawing.Size(320, 275);
            PG_BlockView.TabIndex = 14;
            PG_BlockView.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(PG_BlockView_PropertyValueChanged);
            // 
            // Raw
            // 
            Raw.Controls.Add(propertyGrid1);
            Raw.Location = new System.Drawing.Point(4, 22);
            Raw.Name = "Raw";
            Raw.Padding = new System.Windows.Forms.Padding(3);
            Raw.Size = new System.Drawing.Size(326, 315);
            Raw.TabIndex = 1;
            Raw.Text = "Tab_Raw";
            Raw.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            propertyGrid1.Location = new System.Drawing.Point(3, 3);
            propertyGrid1.Name = "propertyGrid1";
            propertyGrid1.Size = new System.Drawing.Size(320, 309);
            propertyGrid1.TabIndex = 15;
            // 
            // SAV_Accessor
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(334, 341);
            Controls.Add(TC_Tabs);
            Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_Accessor";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "SaveBlock Editor";
            TC_Tabs.ResumeLayout(false);
            Tab_Blocks.ResumeLayout(false);
            Tab_Blocks.PerformLayout();
            Raw.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ComboBox CB_Key;
        private System.Windows.Forms.Label L_Key;
        private System.Windows.Forms.TabControl TC_Tabs;
        private System.Windows.Forms.TabPage Tab_Blocks;
        private System.Windows.Forms.PropertyGrid PG_BlockView;
        private System.Windows.Forms.TabPage Raw;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
    }
}
