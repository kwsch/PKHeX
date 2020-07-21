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
            this.CB_Key = new System.Windows.Forms.ComboBox();
            this.L_Key = new System.Windows.Forms.Label();
            this.TC_Tabs = new System.Windows.Forms.TabControl();
            this.Tab_Blocks = new System.Windows.Forms.TabPage();
            this.PG_BlockView = new System.Windows.Forms.PropertyGrid();
            this.TC_Tabs.SuspendLayout();
            this.Tab_Blocks.SuspendLayout();
            this.SuspendLayout();
            // 
            // CB_Key
            // 
            this.CB_Key.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.CB_Key.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Key.DropDownWidth = 270;
            this.CB_Key.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CB_Key.FormattingEnabled = true;
            this.CB_Key.Location = new System.Drawing.Point(83, 9);
            this.CB_Key.Name = "CB_Key";
            this.CB_Key.Size = new System.Drawing.Size(183, 22);
            this.CB_Key.TabIndex = 0;
            this.CB_Key.SelectedIndexChanged += new System.EventHandler(this.CB_Key_SelectedIndexChanged);
            // 
            // L_Key
            // 
            this.L_Key.AutoSize = true;
            this.L_Key.Location = new System.Drawing.Point(6, 12);
            this.L_Key.Name = "L_Key";
            this.L_Key.Size = new System.Drawing.Size(37, 13);
            this.L_Key.TabIndex = 1;
            this.L_Key.Text = "Block:";
            // 
            // TC_Tabs
            // 
            this.TC_Tabs.Controls.Add(this.Tab_Blocks);
            this.TC_Tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TC_Tabs.Location = new System.Drawing.Point(0, 0);
            this.TC_Tabs.Name = "TC_Tabs";
            this.TC_Tabs.SelectedIndex = 0;
            this.TC_Tabs.Size = new System.Drawing.Size(334, 341);
            this.TC_Tabs.TabIndex = 12;
            // 
            // Tab_Blocks
            // 
            this.Tab_Blocks.Controls.Add(this.PG_BlockView);
            this.Tab_Blocks.Controls.Add(this.L_Key);
            this.Tab_Blocks.Controls.Add(this.CB_Key);
            this.Tab_Blocks.Location = new System.Drawing.Point(4, 22);
            this.Tab_Blocks.Name = "Tab_Blocks";
            this.Tab_Blocks.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_Blocks.Size = new System.Drawing.Size(326, 315);
            this.Tab_Blocks.TabIndex = 0;
            this.Tab_Blocks.Text = "Blocks";
            this.Tab_Blocks.UseVisualStyleBackColor = true;
            // 
            // PG_BlockView
            // 
            this.PG_BlockView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PG_BlockView.Location = new System.Drawing.Point(3, 37);
            this.PG_BlockView.Name = "PG_BlockView";
            this.PG_BlockView.Size = new System.Drawing.Size(320, 275);
            this.PG_BlockView.TabIndex = 14;
            this.PG_BlockView.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.PG_BlockView_PropertyValueChanged);
            // 
            // SAV_Accessor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 341);
            this.Controls.Add(this.TC_Tabs);
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SAV_Accessor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SaveBlock Editor";
            this.TC_Tabs.ResumeLayout(false);
            this.Tab_Blocks.ResumeLayout(false);
            this.Tab_Blocks.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ComboBox CB_Key;
        private System.Windows.Forms.Label L_Key;
        private System.Windows.Forms.TabControl TC_Tabs;
        private System.Windows.Forms.TabPage Tab_Blocks;
        private System.Windows.Forms.PropertyGrid PG_BlockView;
    }
}