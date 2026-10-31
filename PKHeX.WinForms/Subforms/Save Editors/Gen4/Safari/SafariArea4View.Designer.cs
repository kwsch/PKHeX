namespace PKHeX.WinForms
{
    sealed partial class SafariArea4View
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
            TLP_View = new System.Windows.Forms.TableLayoutPanel();
            PB_Map = new System.Windows.Forms.PictureBox();
            L_Coordinates = new System.Windows.Forms.Label();
            TLP_View.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PB_Map).BeginInit();
            SuspendLayout();
            // 
            // TLP_View
            // 
            TLP_View.ColumnCount = 1;
            TLP_View.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_View.Controls.Add(PB_Map, 0, 0);
            TLP_View.Controls.Add(L_Coordinates, 0, 1);
            TLP_View.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_View.Location = new System.Drawing.Point(0, 0);
            TLP_View.Margin = new System.Windows.Forms.Padding(0);
            TLP_View.Name = "TLP_View";
            TLP_View.RowCount = 2;
            TLP_View.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_View.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_View.Size = new System.Drawing.Size(280, 300);
            TLP_View.TabIndex = 0;
            // 
            // PB_Map
            // 
            PB_Map.Dock = System.Windows.Forms.DockStyle.Fill;
            PB_Map.Location = new System.Drawing.Point(3, 3);
            PB_Map.Name = "PB_Map";
            PB_Map.Size = new System.Drawing.Size(274, 269);
            PB_Map.TabIndex = 0;
            PB_Map.TabStop = false;
            PB_Map.Paint += PB_Map_Paint;
            PB_Map.MouseClick += PB_Map_MouseClick;
            PB_Map.MouseLeave += PB_Map_MouseLeave;
            PB_Map.MouseMove += PB_Map_MouseMove;
            PB_Map.Resize += PB_Map_Resize;
            // 
            // L_Coordinates
            // 
            L_Coordinates.AutoSize = true;
            L_Coordinates.Dock = System.Windows.Forms.DockStyle.Fill;
            L_Coordinates.Location = new System.Drawing.Point(3, 275);
            L_Coordinates.Name = "L_Coordinates";
            L_Coordinates.Size = new System.Drawing.Size(274, 25);
            L_Coordinates.TabIndex = 1;
            L_Coordinates.Text = "X: -- Z: -- Y: --";
            L_Coordinates.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SafariArea4View
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            Controls.Add(TLP_View);
            MinimumSize = new System.Drawing.Size(200, 220);
            Name = "SafariArea4View";
            Size = new System.Drawing.Size(280, 300);
            TLP_View.ResumeLayout(false);
            TLP_View.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)PB_Map).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TLP_View;
        private System.Windows.Forms.PictureBox PB_Map;
        private System.Windows.Forms.Label L_Coordinates;
    }
}
