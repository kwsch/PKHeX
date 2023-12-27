namespace PKHeX.WinForms
{
    partial class PokeGear4Editor
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
            B_DeleteAll = new System.Windows.Forms.Button();
            B_GiveAll = new System.Windows.Forms.Button();
            PG_Rolodex = new System.Windows.Forms.PropertyGrid();
            B_GiveAllNoTrainers = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // B_DeleteAll
            // 
            B_DeleteAll.Location = new System.Drawing.Point(4, 40);
            B_DeleteAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_DeleteAll.Name = "B_DeleteAll";
            B_DeleteAll.Size = new System.Drawing.Size(96, 30);
            B_DeleteAll.TabIndex = 11;
            B_DeleteAll.Text = "Delete All";
            B_DeleteAll.UseVisualStyleBackColor = true;
            B_DeleteAll.Click += B_DeleteAll_Click;
            // 
            // B_GiveAll
            // 
            B_GiveAll.Location = new System.Drawing.Point(4, 3);
            B_GiveAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_GiveAll.Name = "B_GiveAll";
            B_GiveAll.Size = new System.Drawing.Size(96, 30);
            B_GiveAll.TabIndex = 10;
            B_GiveAll.Text = "Give All";
            B_GiveAll.UseVisualStyleBackColor = true;
            B_GiveAll.Click += B_GiveAll_Click;
            // 
            // PG_Rolodex
            // 
            PG_Rolodex.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            PG_Rolodex.HelpVisible = false;
            PG_Rolodex.Location = new System.Drawing.Point(106, 2);
            PG_Rolodex.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PG_Rolodex.Name = "PG_Rolodex";
            PG_Rolodex.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            PG_Rolodex.Size = new System.Drawing.Size(299, 262);
            PG_Rolodex.TabIndex = 9;
            PG_Rolodex.ToolbarVisible = false;
            PG_Rolodex.PropertyValueChanged += PG_Rolodex_PropertyValueChanged;
            // 
            // B_GiveAllNoTrainers
            // 
            B_GiveAllNoTrainers.Location = new System.Drawing.Point(4, 77);
            B_GiveAllNoTrainers.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_GiveAllNoTrainers.Name = "B_GiveAllNoTrainers";
            B_GiveAllNoTrainers.Size = new System.Drawing.Size(96, 61);
            B_GiveAllNoTrainers.TabIndex = 12;
            B_GiveAllNoTrainers.Text = "Give All Non-Trainers";
            B_GiveAllNoTrainers.UseVisualStyleBackColor = true;
            B_GiveAllNoTrainers.Click += B_GiveAllNoTrainers_Click;
            // 
            // PokeGear4Editor
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(B_GiveAllNoTrainers);
            Controls.Add(B_DeleteAll);
            Controls.Add(B_GiveAll);
            Controls.Add(PG_Rolodex);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "PokeGear4Editor";
            Size = new System.Drawing.Size(405, 268);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Button B_DeleteAll;
        private System.Windows.Forms.Button B_GiveAll;
        private System.Windows.Forms.PropertyGrid PG_Rolodex;
        private System.Windows.Forms.Button B_GiveAllNoTrainers;
    }
}
