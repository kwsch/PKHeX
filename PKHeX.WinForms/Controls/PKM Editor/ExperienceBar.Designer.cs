namespace PKHeX.WinForms.Controls
{
    partial class ExperienceBar
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
            components = new System.ComponentModel.Container();
            PAN_ExpPercent = new System.Windows.Forms.Panel();
            TT_Exp = new System.Windows.Forms.ToolTip(components);
            SuspendLayout();
            // 
            // PAN_ExpPercent
            // 
            PAN_ExpPercent.BackColor = System.Drawing.Color.DeepSkyBlue;
            PAN_ExpPercent.Cursor = System.Windows.Forms.Cursors.SizeWE;
            PAN_ExpPercent.Dock = System.Windows.Forms.DockStyle.Left;
            PAN_ExpPercent.Location = new System.Drawing.Point(0, 0);
            PAN_ExpPercent.Margin = new System.Windows.Forms.Padding(0);
            PAN_ExpPercent.Name = "PAN_ExpPercent";
            PAN_ExpPercent.Size = new System.Drawing.Size(40, 12);
            PAN_ExpPercent.TabIndex = 0;
            PAN_ExpPercent.MouseDown += HandleMouseDown;
            PAN_ExpPercent.MouseEnter += HandleMouseEnter;
            PAN_ExpPercent.MouseLeave += HandleMouseLeave;
            PAN_ExpPercent.MouseMove += HandleMouseMove;
            PAN_ExpPercent.MouseUp += HandleMouseUp;
            //
            // TT_Exp
            //
            TT_Exp.AutoPopDelay = 30000;
            TT_Exp.InitialDelay = 0;
            TT_Exp.ReshowDelay = 0;
            TT_Exp.ShowAlways = true;
            TT_Exp.UseAnimation = false;
            // 
            // ExperienceBar
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlLight;
            Controls.Add(PAN_ExpPercent);
            Cursor = System.Windows.Forms.Cursors.SizeWE;
            Margin = new System.Windows.Forms.Padding(0);
            Name = "ExperienceBar";
            Size = new System.Drawing.Size(100, 12);
            MouseCaptureChanged += HandleMouseCaptureChanged;
            MouseDown += HandleMouseDown;
            MouseEnter += HandleMouseEnter;
            MouseLeave += HandleMouseLeave;
            MouseMove += HandleMouseMove;
            MouseUp += HandleMouseUp;
            MouseWheel += OnScroll;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel PAN_ExpPercent;
        private System.Windows.Forms.ToolTip TT_Exp;
    }
}
