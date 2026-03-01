namespace PKHeX.WinForms
{
    partial class SAV_SimplePokedex
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
            B_Save = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            B_CaughtNone = new System.Windows.Forms.Button();
            B_CaughtAll = new System.Windows.Forms.Button();
            B_SeenNone = new System.Windows.Forms.Button();
            B_SeenAll = new System.Windows.Forms.Button();
            Label_Caught = new System.Windows.Forms.Label();
            CLB_Caught = new System.Windows.Forms.CheckedListBox();
            Label_Seen = new System.Windows.Forms.Label();
            CLB_Seen = new System.Windows.Forms.CheckedListBox();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Left;
            B_Save.Location = new System.Drawing.Point(196, 365);
            B_Save.Margin = new System.Windows.Forms.Padding(4);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(120, 32);
            B_Save.TabIndex = 19;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(68, 365);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(120, 32);
            B_Cancel.TabIndex = 18;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_CaughtNone
            // 
            B_CaughtNone.Dock = System.Windows.Forms.DockStyle.Fill;
            B_CaughtNone.Location = new System.Drawing.Point(196, 325);
            B_CaughtNone.Margin = new System.Windows.Forms.Padding(4);
            B_CaughtNone.Name = "B_CaughtNone";
            B_CaughtNone.Size = new System.Drawing.Size(184, 32);
            B_CaughtNone.TabIndex = 17;
            B_CaughtNone.Text = "Caught None";
            B_CaughtNone.UseVisualStyleBackColor = true;
            B_CaughtNone.Click += B_CaughtNone_Click;
            // 
            // B_CaughtAll
            // 
            B_CaughtAll.Dock = System.Windows.Forms.DockStyle.Fill;
            B_CaughtAll.Location = new System.Drawing.Point(196, 285);
            B_CaughtAll.Margin = new System.Windows.Forms.Padding(4);
            B_CaughtAll.Name = "B_CaughtAll";
            B_CaughtAll.Size = new System.Drawing.Size(184, 32);
            B_CaughtAll.TabIndex = 16;
            B_CaughtAll.Text = "Caught All";
            B_CaughtAll.UseVisualStyleBackColor = true;
            B_CaughtAll.Click += B_CaughtAll_Click;
            // 
            // B_SeenNone
            // 
            B_SeenNone.Dock = System.Windows.Forms.DockStyle.Fill;
            B_SeenNone.Location = new System.Drawing.Point(4, 325);
            B_SeenNone.Margin = new System.Windows.Forms.Padding(4);
            B_SeenNone.Name = "B_SeenNone";
            B_SeenNone.Size = new System.Drawing.Size(184, 32);
            B_SeenNone.TabIndex = 15;
            B_SeenNone.Text = "Seen None";
            B_SeenNone.UseVisualStyleBackColor = true;
            B_SeenNone.Click += B_SeenNone_Click;
            // 
            // B_SeenAll
            // 
            B_SeenAll.Dock = System.Windows.Forms.DockStyle.Fill;
            B_SeenAll.Location = new System.Drawing.Point(4, 285);
            B_SeenAll.Margin = new System.Windows.Forms.Padding(4);
            B_SeenAll.Name = "B_SeenAll";
            B_SeenAll.Size = new System.Drawing.Size(184, 32);
            B_SeenAll.TabIndex = 14;
            B_SeenAll.Text = "Seen All";
            B_SeenAll.UseVisualStyleBackColor = true;
            B_SeenAll.Click += B_SeenAll_Click;
            // 
            // Label_Caught
            // 
            Label_Caught.AutoSize = true;
            Label_Caught.Location = new System.Drawing.Point(196, 4);
            Label_Caught.Margin = new System.Windows.Forms.Padding(4);
            Label_Caught.Name = "Label_Caught";
            Label_Caught.Size = new System.Drawing.Size(52, 17);
            Label_Caught.TabIndex = 13;
            Label_Caught.Text = "Caught:";
            // 
            // CLB_Caught
            // 
            CLB_Caught.CheckOnClick = true;
            CLB_Caught.Dock = System.Windows.Forms.DockStyle.Fill;
            CLB_Caught.FormattingEnabled = true;
            CLB_Caught.Location = new System.Drawing.Point(196, 29);
            CLB_Caught.Margin = new System.Windows.Forms.Padding(4);
            CLB_Caught.Name = "CLB_Caught";
            CLB_Caught.Size = new System.Drawing.Size(184, 248);
            CLB_Caught.TabIndex = 12;
            // 
            // Label_Seen
            // 
            Label_Seen.AutoSize = true;
            Label_Seen.Location = new System.Drawing.Point(4, 4);
            Label_Seen.Margin = new System.Windows.Forms.Padding(4);
            Label_Seen.Name = "Label_Seen";
            Label_Seen.Size = new System.Drawing.Size(39, 17);
            Label_Seen.TabIndex = 11;
            Label_Seen.Text = "Seen:";
            // 
            // CLB_Seen
            // 
            CLB_Seen.CheckOnClick = true;
            CLB_Seen.Dock = System.Windows.Forms.DockStyle.Fill;
            CLB_Seen.FormattingEnabled = true;
            CLB_Seen.Location = new System.Drawing.Point(4, 29);
            CLB_Seen.Margin = new System.Windows.Forms.Padding(4);
            CLB_Seen.Name = "CLB_Seen";
            CLB_Seen.Size = new System.Drawing.Size(184, 248);
            CLB_Seen.TabIndex = 10;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(Label_Seen, 0, 0);
            tableLayoutPanel1.Controls.Add(Label_Caught, 1, 0);
            tableLayoutPanel1.Controls.Add(B_Save, 1, 4);
            tableLayoutPanel1.Controls.Add(CLB_Seen, 0, 1);
            tableLayoutPanel1.Controls.Add(B_Cancel, 0, 4);
            tableLayoutPanel1.Controls.Add(CLB_Caught, 1, 1);
            tableLayoutPanel1.Controls.Add(B_SeenNone, 0, 3);
            tableLayoutPanel1.Controls.Add(B_CaughtNone, 1, 3);
            tableLayoutPanel1.Controls.Add(B_SeenAll, 0, 2);
            tableLayoutPanel1.Controls.Add(B_CaughtAll, 1, 2);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 5;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.Size = new System.Drawing.Size(384, 401);
            tableLayoutPanel1.TabIndex = 20;
            // 
            // SAV_SimplePokedex
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(384, 401);
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SAV_SimplePokedex";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Pokedex Editor";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_CaughtNone;
        private System.Windows.Forms.Button B_CaughtAll;
        private System.Windows.Forms.Button B_SeenNone;
        private System.Windows.Forms.Button B_SeenAll;
        private System.Windows.Forms.Label Label_Caught;
        private System.Windows.Forms.CheckedListBox CLB_Caught;
        private System.Windows.Forms.Label Label_Seen;
        private System.Windows.Forms.CheckedListBox CLB_Seen;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
