namespace PKHeX.WinForms
{
    partial class BatchEditor
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
            this.components = new System.ComponentModel.Container();
            this.RB_Boxes = new System.Windows.Forms.RadioButton();
            this.RB_Path = new System.Windows.Forms.RadioButton();
            this.FLP_RB = new System.Windows.Forms.FlowLayoutPanel();
            this.RB_Party = new System.Windows.Forms.RadioButton();
            this.TB_Folder = new System.Windows.Forms.TextBox();
            this.RTB_Instructions = new System.Windows.Forms.RichTextBox();
            this.B_Go = new System.Windows.Forms.Button();
            this.PB_Show = new System.Windows.Forms.ProgressBar();
            this.CB_Format = new System.Windows.Forms.ComboBox();
            this.CB_Property = new System.Windows.Forms.ComboBox();
            this.CB_Require = new System.Windows.Forms.ComboBox();
            this.B_Add = new System.Windows.Forms.Button();
            this.L_PropType = new System.Windows.Forms.Label();
            this.L_PropValue = new System.Windows.Forms.Label();
            this.b = new System.ComponentModel.BackgroundWorker();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip3 = new System.Windows.Forms.ToolTip(this.components);
            this.FLP_RB.SuspendLayout();
            this.SuspendLayout();
            // 
            // RB_Boxes
            // 
            this.RB_Boxes.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.RB_Boxes.Appearance = System.Windows.Forms.Appearance.Button;
            this.RB_Boxes.AutoSize = true;
            this.RB_Boxes.Checked = true;
            this.RB_Boxes.Location = new System.Drawing.Point(0, 0);
            this.RB_Boxes.Margin = new System.Windows.Forms.Padding(0);
            this.RB_Boxes.Name = "RB_Boxes";
            this.RB_Boxes.Size = new System.Drawing.Size(46, 23);
            this.RB_Boxes.TabIndex = 0;
            this.RB_Boxes.TabStop = true;
            this.RB_Boxes.Text = "Boxes";
            this.RB_Boxes.UseVisualStyleBackColor = true;
            this.RB_Boxes.Click += new System.EventHandler(this.B_SAV_Click);
            // 
            // RB_Path
            // 
            this.RB_Path.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.RB_Path.Appearance = System.Windows.Forms.Appearance.Button;
            this.RB_Path.AutoSize = true;
            this.RB_Path.Location = new System.Drawing.Point(87, 0);
            this.RB_Path.Margin = new System.Windows.Forms.Padding(0);
            this.RB_Path.Name = "RB_Path";
            this.RB_Path.Size = new System.Drawing.Size(55, 23);
            this.RB_Path.TabIndex = 1;
            this.RB_Path.Text = "Folder...";
            this.RB_Path.UseVisualStyleBackColor = true;
            this.RB_Path.Click += new System.EventHandler(this.B_Open_Click);
            // 
            // FLP_RB
            // 
            this.FLP_RB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_RB.Controls.Add(this.RB_Boxes);
            this.FLP_RB.Controls.Add(this.RB_Party);
            this.FLP_RB.Controls.Add(this.RB_Path);
            this.FLP_RB.Controls.Add(this.TB_Folder);
            this.FLP_RB.Location = new System.Drawing.Point(12, 10);
            this.FLP_RB.Name = "FLP_RB";
            this.FLP_RB.Size = new System.Drawing.Size(370, 24);
            this.FLP_RB.TabIndex = 2;
            // 
            // RB_Party
            // 
            this.RB_Party.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.RB_Party.Appearance = System.Windows.Forms.Appearance.Button;
            this.RB_Party.AutoSize = true;
            this.RB_Party.Location = new System.Drawing.Point(46, 0);
            this.RB_Party.Margin = new System.Windows.Forms.Padding(0);
            this.RB_Party.Name = "RB_Party";
            this.RB_Party.Size = new System.Drawing.Size(41, 23);
            this.RB_Party.TabIndex = 5;
            this.RB_Party.Text = "Party";
            this.RB_Party.UseVisualStyleBackColor = true;
            // 
            // TB_Folder
            // 
            this.TB_Folder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_Folder.Location = new System.Drawing.Point(144, 2);
            this.TB_Folder.Margin = new System.Windows.Forms.Padding(2);
            this.TB_Folder.Name = "TB_Folder";
            this.TB_Folder.ReadOnly = true;
            this.TB_Folder.Size = new System.Drawing.Size(216, 20);
            this.TB_Folder.TabIndex = 4;
            this.TB_Folder.Visible = false;
            // 
            // RTB_Instructions
            // 
            this.RTB_Instructions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RTB_Instructions.Location = new System.Drawing.Point(12, 84);
            this.RTB_Instructions.Name = "RTB_Instructions";
            this.RTB_Instructions.Size = new System.Drawing.Size(370, 141);
            this.RTB_Instructions.TabIndex = 5;
            this.RTB_Instructions.Text = "";
            // 
            // B_Go
            // 
            this.B_Go.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Go.Location = new System.Drawing.Point(325, 230);
            this.B_Go.Name = "B_Go";
            this.B_Go.Size = new System.Drawing.Size(57, 23);
            this.B_Go.TabIndex = 6;
            this.B_Go.Text = "Run";
            this.B_Go.UseVisualStyleBackColor = true;
            this.B_Go.Click += new System.EventHandler(this.B_Go_Click);
            // 
            // PB_Show
            // 
            this.PB_Show.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PB_Show.Location = new System.Drawing.Point(12, 231);
            this.PB_Show.Name = "PB_Show";
            this.PB_Show.Size = new System.Drawing.Size(307, 21);
            this.PB_Show.TabIndex = 7;
            // 
            // CB_Format
            // 
            this.CB_Format.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Format.FormattingEnabled = true;
            this.CB_Format.Location = new System.Drawing.Point(11, 40);
            this.CB_Format.Name = "CB_Format";
            this.CB_Format.Size = new System.Drawing.Size(44, 21);
            this.CB_Format.TabIndex = 8;
            this.CB_Format.SelectedIndexChanged += new System.EventHandler(this.CB_Format_SelectedIndexChanged);
            // 
            // CB_Property
            // 
            this.CB_Property.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_Property.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CB_Property.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CB_Property.DropDownWidth = 200;
            this.CB_Property.FormattingEnabled = true;
            this.CB_Property.Location = new System.Drawing.Point(61, 40);
            this.CB_Property.Name = "CB_Property";
            this.CB_Property.Size = new System.Drawing.Size(140, 21);
            this.CB_Property.TabIndex = 9;
            this.CB_Property.SelectedIndexChanged += new System.EventHandler(this.CB_Property_SelectedIndexChanged);
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
            this.CB_Require.Location = new System.Drawing.Point(207, 40);
            this.CB_Require.Name = "CB_Require";
            this.CB_Require.Size = new System.Drawing.Size(111, 21);
            this.CB_Require.TabIndex = 10;
            // 
            // B_Add
            // 
            this.B_Add.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Add.Location = new System.Drawing.Point(324, 39);
            this.B_Add.Name = "B_Add";
            this.B_Add.Size = new System.Drawing.Size(57, 23);
            this.B_Add.TabIndex = 11;
            this.B_Add.Text = "Add";
            this.B_Add.UseVisualStyleBackColor = true;
            this.B_Add.Click += new System.EventHandler(this.B_Add_Click);
            // 
            // L_PropType
            // 
            this.L_PropType.AutoSize = true;
            this.L_PropType.Location = new System.Drawing.Point(58, 64);
            this.L_PropType.Name = "L_PropType";
            this.L_PropType.Size = new System.Drawing.Size(70, 13);
            this.L_PropType.TabIndex = 12;
            this.L_PropType.Text = "PropertyType";
            // 
            // L_PropValue
            // 
            this.L_PropValue.AutoSize = true;
            this.L_PropValue.Location = new System.Drawing.Point(204, 64);
            this.L_PropValue.Name = "L_PropValue";
            this.L_PropValue.Size = new System.Drawing.Size(73, 13);
            this.L_PropValue.TabIndex = 13;
            this.L_PropValue.Text = "PropertyValue";
            // 
            // b
            // 
            this.b.WorkerReportsProgress = true;
            // 
            // BatchEditor
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 261);
            this.Controls.Add(this.L_PropValue);
            this.Controls.Add(this.L_PropType);
            this.Controls.Add(this.B_Add);
            this.Controls.Add(this.CB_Require);
            this.Controls.Add(this.CB_Property);
            this.Controls.Add(this.CB_Format);
            this.Controls.Add(this.PB_Show);
            this.Controls.Add(this.B_Go);
            this.Controls.Add(this.RTB_Instructions);
            this.Controls.Add(this.FLP_RB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(410, 300);
            this.Name = "BatchEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Batch Editor";
            this.FLP_RB.ResumeLayout(false);
            this.FLP_RB.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton RB_Boxes;
        private System.Windows.Forms.RadioButton RB_Path;
        private System.Windows.Forms.FlowLayoutPanel FLP_RB;
        private System.Windows.Forms.TextBox TB_Folder;
        private System.Windows.Forms.RichTextBox RTB_Instructions;
        private System.Windows.Forms.Button B_Go;
        private System.Windows.Forms.ProgressBar PB_Show;
        private System.Windows.Forms.ComboBox CB_Format;
        private System.Windows.Forms.ComboBox CB_Property;
        private System.Windows.Forms.ComboBox CB_Require;
        private System.Windows.Forms.Button B_Add;
        private System.Windows.Forms.Label L_PropType;
        private System.Windows.Forms.Label L_PropValue;
        private System.Windows.Forms.RadioButton RB_Party;
        private System.ComponentModel.BackgroundWorker b;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolTip toolTip2;
        private System.Windows.Forms.ToolTip toolTip3;
    }
}