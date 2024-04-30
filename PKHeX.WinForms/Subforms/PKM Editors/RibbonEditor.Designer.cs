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
            components = new System.ComponentModel.Container();
            B_Save = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            B_None = new System.Windows.Forms.Button();
            B_All = new System.Windows.Forms.Button();
            PAN_Container = new System.Windows.Forms.Panel();
            SPLIT_Ribbons = new System.Windows.Forms.SplitContainer();
            FLP_Ribbons = new System.Windows.Forms.FlowLayoutPanel();
            TLP_Ribbons = new System.Windows.Forms.TableLayoutPanel();
            tipName = new System.Windows.Forms.ToolTip(components);
            CB_Affixed = new System.Windows.Forms.ComboBox();
            PAN_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SPLIT_Ribbons).BeginInit();
            SPLIT_Ribbons.Panel1.SuspendLayout();
            SPLIT_Ribbons.Panel2.SuspendLayout();
            SPLIT_Ribbons.SuspendLayout();
            SuspendLayout();
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(488, 287);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(105, 27);
            B_Save.TabIndex = 1;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(376, 287);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(105, 27);
            B_Cancel.TabIndex = 2;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_None
            // 
            B_None.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_None.Location = new System.Drawing.Point(126, 287);
            B_None.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_None.Name = "B_None";
            B_None.Size = new System.Drawing.Size(105, 27);
            B_None.TabIndex = 5;
            B_None.Text = "Remove All";
            B_None.UseVisualStyleBackColor = true;
            B_None.Click += B_None_Click;
            // 
            // B_All
            // 
            B_All.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            B_All.Location = new System.Drawing.Point(14, 287);
            B_All.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_All.Name = "B_All";
            B_All.Size = new System.Drawing.Size(105, 27);
            B_All.TabIndex = 4;
            B_All.Text = "Give All";
            B_All.UseVisualStyleBackColor = true;
            B_All.Click += B_All_Click;
            // 
            // PAN_Container
            // 
            PAN_Container.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            PAN_Container.BackColor = System.Drawing.SystemColors.Window;
            PAN_Container.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            PAN_Container.Controls.Add(SPLIT_Ribbons);
            PAN_Container.Location = new System.Drawing.Point(14, 14);
            PAN_Container.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PAN_Container.Name = "PAN_Container";
            PAN_Container.Size = new System.Drawing.Size(578, 266);
            PAN_Container.TabIndex = 6;
            // 
            // SPLIT_Ribbons
            // 
            SPLIT_Ribbons.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            SPLIT_Ribbons.Dock = System.Windows.Forms.DockStyle.Fill;
            SPLIT_Ribbons.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            SPLIT_Ribbons.IsSplitterFixed = true;
            SPLIT_Ribbons.Location = new System.Drawing.Point(0, 0);
            SPLIT_Ribbons.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SPLIT_Ribbons.Name = "SPLIT_Ribbons";
            // 
            // SPLIT_Ribbons.Panel1
            // 
            SPLIT_Ribbons.Panel1.Controls.Add(FLP_Ribbons);
            SPLIT_Ribbons.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            SPLIT_Ribbons.Panel1MinSize = 1;
            // 
            // SPLIT_Ribbons.Panel2
            // 
            SPLIT_Ribbons.Panel2.Controls.Add(TLP_Ribbons);
            SPLIT_Ribbons.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            SPLIT_Ribbons.Panel2MinSize = 1;
            SPLIT_Ribbons.Size = new System.Drawing.Size(576, 264);
            SPLIT_Ribbons.SplitterDistance = 270;
            SPLIT_Ribbons.SplitterWidth = 1;
            SPLIT_Ribbons.TabIndex = 7;
            // 
            // FLP_Ribbons
            // 
            FLP_Ribbons.AutoScroll = true;
            FLP_Ribbons.Dock = System.Windows.Forms.DockStyle.Fill;
            FLP_Ribbons.Location = new System.Drawing.Point(0, 0);
            FLP_Ribbons.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            FLP_Ribbons.Name = "FLP_Ribbons";
            FLP_Ribbons.Size = new System.Drawing.Size(268, 262);
            FLP_Ribbons.TabIndex = 5;
            // 
            // TLP_Ribbons
            // 
            TLP_Ribbons.AutoScroll = true;
            TLP_Ribbons.ColumnCount = 2;
            TLP_Ribbons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Ribbons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Ribbons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            TLP_Ribbons.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Ribbons.Location = new System.Drawing.Point(0, 0);
            TLP_Ribbons.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TLP_Ribbons.Name = "TLP_Ribbons";
            TLP_Ribbons.RowCount = 1;
            TLP_Ribbons.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Ribbons.Size = new System.Drawing.Size(303, 262);
            TLP_Ribbons.TabIndex = 3;
            // 
            // CB_Affixed
            // 
            CB_Affixed.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            CB_Affixed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_Affixed.DropDownWidth = 200;
            CB_Affixed.FormattingEnabled = true;
            CB_Affixed.Location = new System.Drawing.Point(238, 288);
            CB_Affixed.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CB_Affixed.Name = "CB_Affixed";
            CB_Affixed.Size = new System.Drawing.Size(130, 23);
            CB_Affixed.TabIndex = 7;
            // 
            // RibbonEditor
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(611, 324);
            Controls.Add(CB_Affixed);
            Controls.Add(PAN_Container);
            Controls.Add(B_None);
            Controls.Add(B_All);
            Controls.Add(B_Cancel);
            Controls.Add(B_Save);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(627, 363);
            Name = "RibbonEditor";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Ribbon Editor";
            PAN_Container.ResumeLayout(false);
            SPLIT_Ribbons.Panel1.ResumeLayout(false);
            SPLIT_Ribbons.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)SPLIT_Ribbons).EndInit();
            SPLIT_Ribbons.ResumeLayout(false);
            ResumeLayout(false);
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
