namespace PKHeX.WinForms
{
    partial class RibbonEditor
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
            this.B_Save = new System.Windows.Forms.Button();
            this.B_Cancel = new System.Windows.Forms.Button();
            this.B_None = new System.Windows.Forms.Button();
            this.B_All = new System.Windows.Forms.Button();
            this.PAN_Container = new System.Windows.Forms.Panel();
            this.SPLIT_Ribbons = new System.Windows.Forms.SplitContainer();
            this.FLP_Ribbons = new System.Windows.Forms.FlowLayoutPanel();
            this.TLP_Ribbons = new System.Windows.Forms.TableLayoutPanel();
            this.tipName = new System.Windows.Forms.ToolTip(this.components);
            this.CB_Affixed = new System.Windows.Forms.ComboBox();
            this.PAN_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SPLIT_Ribbons)).BeginInit();
            this.SPLIT_Ribbons.Panel1.SuspendLayout();
            this.SPLIT_Ribbons.Panel2.SuspendLayout();
            this.SPLIT_Ribbons.SuspendLayout();
            this.SuspendLayout();
            // 
            // B_Save
            // 
            this.B_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Save.Location = new System.Drawing.Point(418, 249);
            this.B_Save.Name = "B_Save";
            this.B_Save.Size = new System.Drawing.Size(90, 23);
            this.B_Save.TabIndex = 1;
            this.B_Save.Text = "Save";
            this.B_Save.UseVisualStyleBackColor = true;
            this.B_Save.Click += new System.EventHandler(this.B_Save_Click);
            // 
            // B_Cancel
            // 
            this.B_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_Cancel.Location = new System.Drawing.Point(322, 249);
            this.B_Cancel.Name = "B_Cancel";
            this.B_Cancel.Size = new System.Drawing.Size(90, 23);
            this.B_Cancel.TabIndex = 2;
            this.B_Cancel.Text = "Cancel";
            this.B_Cancel.UseVisualStyleBackColor = true;
            this.B_Cancel.Click += new System.EventHandler(this.B_Cancel_Click);
            // 
            // B_None
            // 
            this.B_None.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.B_None.Location = new System.Drawing.Point(108, 249);
            this.B_None.Name = "B_None";
            this.B_None.Size = new System.Drawing.Size(90, 23);
            this.B_None.TabIndex = 5;
            this.B_None.Text = "Remove All";
            this.B_None.UseVisualStyleBackColor = true;
            this.B_None.Click += new System.EventHandler(this.B_None_Click);
            // 
            // B_All
            // 
            this.B_All.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.B_All.Location = new System.Drawing.Point(12, 249);
            this.B_All.Name = "B_All";
            this.B_All.Size = new System.Drawing.Size(90, 23);
            this.B_All.TabIndex = 4;
            this.B_All.Text = "Give All";
            this.B_All.UseVisualStyleBackColor = true;
            this.B_All.Click += new System.EventHandler(this.B_All_Click);
            // 
            // PAN_Container
            // 
            this.PAN_Container.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PAN_Container.BackColor = System.Drawing.SystemColors.Window;
            this.PAN_Container.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PAN_Container.Controls.Add(this.SPLIT_Ribbons);
            this.PAN_Container.Location = new System.Drawing.Point(12, 12);
            this.PAN_Container.Name = "PAN_Container";
            this.PAN_Container.Size = new System.Drawing.Size(496, 231);
            this.PAN_Container.TabIndex = 6;
            // 
            // SPLIT_Ribbons
            // 
            this.SPLIT_Ribbons.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SPLIT_Ribbons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SPLIT_Ribbons.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.SPLIT_Ribbons.IsSplitterFixed = true;
            this.SPLIT_Ribbons.Location = new System.Drawing.Point(0, 0);
            this.SPLIT_Ribbons.Name = "SPLIT_Ribbons";
            // 
            // SPLIT_Ribbons.Panel1
            // 
            this.SPLIT_Ribbons.Panel1.Controls.Add(this.FLP_Ribbons);
            this.SPLIT_Ribbons.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SPLIT_Ribbons.Panel1MinSize = 1;
            // 
            // SPLIT_Ribbons.Panel2
            // 
            this.SPLIT_Ribbons.Panel2.Controls.Add(this.TLP_Ribbons);
            this.SPLIT_Ribbons.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SPLIT_Ribbons.Panel2MinSize = 1;
            this.SPLIT_Ribbons.Size = new System.Drawing.Size(494, 229);
            this.SPLIT_Ribbons.SplitterDistance = 270;
            this.SPLIT_Ribbons.SplitterWidth = 1;
            this.SPLIT_Ribbons.TabIndex = 7;
            // 
            // FLP_Ribbons
            // 
            this.FLP_Ribbons.AutoScroll = true;
            this.FLP_Ribbons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FLP_Ribbons.Location = new System.Drawing.Point(0, 0);
            this.FLP_Ribbons.Name = "FLP_Ribbons";
            this.FLP_Ribbons.Size = new System.Drawing.Size(268, 227);
            this.FLP_Ribbons.TabIndex = 5;
            // 
            // TLP_Ribbons
            // 
            this.TLP_Ribbons.AutoScroll = true;
            this.TLP_Ribbons.ColumnCount = 2;
            this.TLP_Ribbons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TLP_Ribbons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TLP_Ribbons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TLP_Ribbons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLP_Ribbons.Location = new System.Drawing.Point(0, 0);
            this.TLP_Ribbons.Name = "TLP_Ribbons";
            this.TLP_Ribbons.RowCount = 1;
            this.TLP_Ribbons.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_Ribbons.Size = new System.Drawing.Size(221, 227);
            this.TLP_Ribbons.TabIndex = 3;
            // 
            // CB_Affixed
            // 
            this.CB_Affixed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_Affixed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Affixed.DropDownWidth = 200;
            this.CB_Affixed.FormattingEnabled = true;
            this.CB_Affixed.Location = new System.Drawing.Point(204, 250);
            this.CB_Affixed.Name = "CB_Affixed";
            this.CB_Affixed.Size = new System.Drawing.Size(112, 21);
            this.CB_Affixed.TabIndex = 7;
            // 
            // RibbonEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(524, 281);
            this.Controls.Add(this.CB_Affixed);
            this.Controls.Add(this.PAN_Container);
            this.Controls.Add(this.B_None);
            this.Controls.Add(this.B_All);
            this.Controls.Add(this.B_Cancel);
            this.Controls.Add(this.B_Save);
            this.Icon = global::PKHeX.WinForms.Properties.Resources.Icon;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(540, 320);
            this.Name = "RibbonEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Ribbon Editor";
            this.PAN_Container.ResumeLayout(false);
            this.SPLIT_Ribbons.Panel1.ResumeLayout(false);
            this.SPLIT_Ribbons.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SPLIT_Ribbons)).EndInit();
            this.SPLIT_Ribbons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_None;
        private System.Windows.Forms.Button B_All;
        private System.Windows.Forms.Panel PAN_Container;
        private System.Windows.Forms.TableLayoutPanel TLP_Ribbons;
        private System.Windows.Forms.FlowLayoutPanel FLP_Ribbons;
        private System.Windows.Forms.SplitContainer SPLIT_Ribbons;
        private System.Windows.Forms.ToolTip tipName;
        private System.Windows.Forms.ComboBox CB_Affixed;
    }
}