namespace PKHeX.WinForms
{
    partial class EntitySearchSetup
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
            TLP_Main = new System.Windows.Forms.TableLayoutPanel();
            TC_SearchSettings = new System.Windows.Forms.TabControl();
            Tab_General = new System.Windows.Forms.TabPage();
            UC_EntitySearch = new PKHeX.WinForms.Controls.EntitySearchControl();
            Tab_Advanced = new System.Windows.Forms.TabPage();
            B_Add = new System.Windows.Forms.Button();
            RTB_Instructions = new System.Windows.Forms.RichTextBox();
            TLP_SearchNavigate = new System.Windows.Forms.TableLayoutPanel();
            B_Next = new System.Windows.Forms.Button();
            B_Previous = new System.Windows.Forms.Button();
            B_Search = new System.Windows.Forms.Button();
            B_Reset = new System.Windows.Forms.Button();
            TLP_Main.SuspendLayout();
            TC_SearchSettings.SuspendLayout();
            Tab_General.SuspendLayout();
            Tab_Advanced.SuspendLayout();
            TLP_SearchNavigate.SuspendLayout();
            SuspendLayout();
            // 
            // TLP_Main
            // 
            TLP_Main.ColumnCount = 1;
            TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_Main.Controls.Add(TC_SearchSettings, 0, 0);
            TLP_Main.Controls.Add(TLP_SearchNavigate, 0, 1);
            TLP_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Main.Location = new System.Drawing.Point(0, 0);
            TLP_Main.Margin = new System.Windows.Forms.Padding(0);
            TLP_Main.Name = "TLP_Main";
            TLP_Main.RowCount = 2;
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.Size = new System.Drawing.Size(320, 502);
            TLP_Main.TabIndex = 0;
            // 
            // TC_SearchSettings
            // 
            TC_SearchSettings.Controls.Add(Tab_General);
            TC_SearchSettings.Controls.Add(Tab_Advanced);
            TC_SearchSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            TC_SearchSettings.Location = new System.Drawing.Point(0, 0);
            TC_SearchSettings.Margin = new System.Windows.Forms.Padding(0);
            TC_SearchSettings.Name = "TC_SearchSettings";
            TC_SearchSettings.Padding = new System.Drawing.Point(0, 0);
            TC_SearchSettings.SelectedIndex = 0;
            TC_SearchSettings.Size = new System.Drawing.Size(320, 474);
            TC_SearchSettings.TabIndex = 2;
            // 
            // Tab_General
            // 
            Tab_General.Controls.Add(UC_EntitySearch);
            Tab_General.Location = new System.Drawing.Point(4, 26);
            Tab_General.Margin = new System.Windows.Forms.Padding(0);
            Tab_General.Name = "Tab_General";
            Tab_General.Size = new System.Drawing.Size(312, 444);
            Tab_General.TabIndex = 0;
            Tab_General.Text = "General";
            Tab_General.UseVisualStyleBackColor = true;
            // 
            // UC_EntitySearch
            // 
            UC_EntitySearch.AutoSize = true;
            UC_EntitySearch.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            UC_EntitySearch.Dock = System.Windows.Forms.DockStyle.Fill;
            UC_EntitySearch.Location = new System.Drawing.Point(0, 0);
            UC_EntitySearch.Margin = new System.Windows.Forms.Padding(0);
            UC_EntitySearch.Name = "UC_EntitySearch";
            UC_EntitySearch.Size = new System.Drawing.Size(312, 444);
            UC_EntitySearch.TabIndex = 0;
            // 
            // Tab_Advanced
            // 
            Tab_Advanced.Controls.Add(B_Add);
            Tab_Advanced.Controls.Add(RTB_Instructions);
            Tab_Advanced.Location = new System.Drawing.Point(4, 26);
            Tab_Advanced.Margin = new System.Windows.Forms.Padding(0);
            Tab_Advanced.Name = "Tab_Advanced";
            Tab_Advanced.Size = new System.Drawing.Size(192, 70);
            Tab_Advanced.TabIndex = 1;
            Tab_Advanced.Text = "Advanced";
            Tab_Advanced.UseVisualStyleBackColor = true;
            // 
            // B_Add
            // 
            B_Add.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            B_Add.Location = new System.Drawing.Point(128, 0);
            B_Add.Margin = new System.Windows.Forms.Padding(0);
            B_Add.Name = "B_Add";
            B_Add.Size = new System.Drawing.Size(66, 27);
            B_Add.TabIndex = 1;
            B_Add.Text = "Add";
            B_Add.UseVisualStyleBackColor = true;
            B_Add.Click += B_Add_Click;
            // 
            // RTB_Instructions
            // 
            RTB_Instructions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            RTB_Instructions.Location = new System.Drawing.Point(0, 55);
            RTB_Instructions.Margin = new System.Windows.Forms.Padding(0);
            RTB_Instructions.Name = "RTB_Instructions";
            RTB_Instructions.Size = new System.Drawing.Size(192, 15);
            RTB_Instructions.TabIndex = 0;
            RTB_Instructions.Text = "";
            // 
            // TLP_SearchNavigate
            // 
            TLP_SearchNavigate.ColumnCount = 3;
            TLP_SearchNavigate.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_SearchNavigate.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_SearchNavigate.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_SearchNavigate.Controls.Add(B_Next, 2, 0);
            TLP_SearchNavigate.Controls.Add(B_Previous, 0, 0);
            TLP_SearchNavigate.Controls.Add(B_Search, 1, 0);
            TLP_SearchNavigate.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_SearchNavigate.Location = new System.Drawing.Point(0, 474);
            TLP_SearchNavigate.Margin = new System.Windows.Forms.Padding(0);
            TLP_SearchNavigate.Name = "TLP_SearchNavigate";
            TLP_SearchNavigate.RowCount = 1;
            TLP_SearchNavigate.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_SearchNavigate.Size = new System.Drawing.Size(320, 28);
            TLP_SearchNavigate.TabIndex = 3;
            // 
            // B_Next
            // 
            B_Next.Dock = System.Windows.Forms.DockStyle.Fill;
            B_Next.Location = new System.Drawing.Point(232, 0);
            B_Next.Margin = new System.Windows.Forms.Padding(0);
            B_Next.Name = "B_Next";
            B_Next.Size = new System.Drawing.Size(88, 30);
            B_Next.TabIndex = 6;
            B_Next.Text = "Next";
            B_Next.UseVisualStyleBackColor = true;
            B_Next.Visible = false;
            B_Next.Click += B_Next_Click;
            // 
            // B_Previous
            // 
            B_Previous.Dock = System.Windows.Forms.DockStyle.Fill;
            B_Previous.Location = new System.Drawing.Point(0, 0);
            B_Previous.Margin = new System.Windows.Forms.Padding(0);
            B_Previous.Name = "B_Previous";
            B_Previous.Size = new System.Drawing.Size(88, 30);
            B_Previous.TabIndex = 5;
            B_Previous.Text = "Previous";
            B_Previous.UseVisualStyleBackColor = true;
            B_Previous.Visible = false;
            B_Previous.Click += B_Previous_Click;
            // 
            // B_Search
            // 
            B_Search.AutoSize = true;
            B_Search.Dock = System.Windows.Forms.DockStyle.Fill;
            B_Search.Location = new System.Drawing.Point(88, 0);
            B_Search.Margin = new System.Windows.Forms.Padding(0);
            B_Search.Name = "B_Search";
            B_Search.Size = new System.Drawing.Size(144, 30);
            B_Search.TabIndex = 4;
            B_Search.Text = "Search!";
            B_Search.UseVisualStyleBackColor = true;
            B_Search.Click += B_Search_Click;
            // 
            // B_Reset
            // 
            B_Reset.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            B_Reset.Location = new System.Drawing.Point(214, 0);
            B_Reset.Margin = new System.Windows.Forms.Padding(0);
            B_Reset.Name = "B_Reset";
            B_Reset.Size = new System.Drawing.Size(104, 27);
            B_Reset.TabIndex = 0;
            B_Reset.Text = "Reset Filters";
            B_Reset.UseVisualStyleBackColor = true;
            B_Reset.Click += B_Reset_Click;
            // 
            // EntitySearchSetup
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(320, 502);
            Controls.Add(B_Reset);
            Controls.Add(TLP_Main);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "EntitySearchSetup";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Search";
            KeyDown += OnKeyDown;
            TLP_Main.ResumeLayout(false);
            TC_SearchSettings.ResumeLayout(false);
            Tab_General.ResumeLayout(false);
            Tab_General.PerformLayout();
            Tab_Advanced.ResumeLayout(false);
            TLP_SearchNavigate.ResumeLayout(false);
            TLP_SearchNavigate.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TLP_Main;
        private System.Windows.Forms.Button B_Reset;
        private System.Windows.Forms.TabControl TC_SearchSettings;
        private System.Windows.Forms.TabPage Tab_General;
        private Controls.EntitySearchControl UC_EntitySearch;
        private System.Windows.Forms.TabPage Tab_Advanced;
        private System.Windows.Forms.Button B_Add;
        private System.Windows.Forms.RichTextBox RTB_Instructions;
        private System.Windows.Forms.TableLayoutPanel TLP_SearchNavigate;
        private System.Windows.Forms.Button B_Next;
        private System.Windows.Forms.Button B_Previous;
        private System.Windows.Forms.Button B_Search;
    }
}
