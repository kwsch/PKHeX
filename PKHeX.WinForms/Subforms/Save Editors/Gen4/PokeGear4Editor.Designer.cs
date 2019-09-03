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
            this.B_DeleteAll = new System.Windows.Forms.Button();
            this.B_GiveAll = new System.Windows.Forms.Button();
            this.PG_Rolodex = new System.Windows.Forms.PropertyGrid();
            this.B_GiveAllNoTrainers = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // B_DeleteAll
            // 
            this.B_DeleteAll.Location = new System.Drawing.Point(3, 35);
            this.B_DeleteAll.Name = "B_DeleteAll";
            this.B_DeleteAll.Size = new System.Drawing.Size(82, 26);
            this.B_DeleteAll.TabIndex = 11;
            this.B_DeleteAll.Text = "Delete All";
            this.B_DeleteAll.UseVisualStyleBackColor = true;
            this.B_DeleteAll.Click += new System.EventHandler(this.B_DeleteAll_Click);
            // 
            // B_GiveAll
            // 
            this.B_GiveAll.Location = new System.Drawing.Point(3, 3);
            this.B_GiveAll.Name = "B_GiveAll";
            this.B_GiveAll.Size = new System.Drawing.Size(82, 26);
            this.B_GiveAll.TabIndex = 10;
            this.B_GiveAll.Text = "Give All";
            this.B_GiveAll.UseVisualStyleBackColor = true;
            this.B_GiveAll.Click += new System.EventHandler(this.B_GiveAll_Click);
            // 
            // PG_Rolodex
            // 
            this.PG_Rolodex.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PG_Rolodex.HelpVisible = false;
            this.PG_Rolodex.Location = new System.Drawing.Point(91, 2);
            this.PG_Rolodex.Name = "PG_Rolodex";
            this.PG_Rolodex.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.PG_Rolodex.Size = new System.Drawing.Size(256, 227);
            this.PG_Rolodex.TabIndex = 9;
            this.PG_Rolodex.ToolbarVisible = false;
            this.PG_Rolodex.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.PG_Rolodex_PropertyValueChanged);
            // 
            // B_GiveAllNoTrainers
            // 
            this.B_GiveAllNoTrainers.Location = new System.Drawing.Point(3, 67);
            this.B_GiveAllNoTrainers.Name = "B_GiveAllNoTrainers";
            this.B_GiveAllNoTrainers.Size = new System.Drawing.Size(82, 53);
            this.B_GiveAllNoTrainers.TabIndex = 12;
            this.B_GiveAllNoTrainers.Text = "Give All Non-Trainers";
            this.B_GiveAllNoTrainers.UseVisualStyleBackColor = true;
            this.B_GiveAllNoTrainers.Click += new System.EventHandler(this.B_GiveAllNoTrainers_Click);
            // 
            // PokeGear4Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.B_GiveAllNoTrainers);
            this.Controls.Add(this.B_DeleteAll);
            this.Controls.Add(this.B_GiveAll);
            this.Controls.Add(this.PG_Rolodex);
            this.Name = "PokeGear4Editor";
            this.Size = new System.Drawing.Size(347, 232);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button B_DeleteAll;
        private System.Windows.Forms.Button B_GiveAll;
        private System.Windows.Forms.PropertyGrid PG_Rolodex;
        private System.Windows.Forms.Button B_GiveAllNoTrainers;
    }
}
