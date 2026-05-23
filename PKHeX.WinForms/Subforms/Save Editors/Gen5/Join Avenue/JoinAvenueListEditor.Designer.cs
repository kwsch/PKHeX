namespace PKHeX.WinForms;

partial class JoinAvenueListEditor
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
        if (disposing)
            components?.Dispose();
        base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        P_List = new System.Windows.Forms.Panel();
        LB_Entries = new System.Windows.Forms.ListBox();
        FLP_ListActions = new System.Windows.Forms.FlowLayoutPanel();
        B_Import = new System.Windows.Forms.Button();
        B_Export = new System.Windows.Forms.Button();
        TC_Editor = new System.Windows.Forms.TabControl();
        Tab_General = new System.Windows.Forms.TabPage();
        Tab_Specific = new System.Windows.Forms.TabPage();
        P_List.SuspendLayout();
        FLP_ListActions.SuspendLayout();
        SuspendLayout();
        // 
        // P_List
        // 
        P_List.Controls.Add(LB_Entries);
        P_List.Controls.Add(FLP_ListActions);
        P_List.Dock = System.Windows.Forms.DockStyle.Left;
        P_List.Location = new System.Drawing.Point(0, 0);
        P_List.Name = "P_List";
        P_List.Size = new System.Drawing.Size(180, 360);
        P_List.TabIndex = 0;
        // 
        // LB_Entries
        // 
        LB_Entries.Dock = System.Windows.Forms.DockStyle.Fill;
        LB_Entries.FormattingEnabled = true;
        LB_Entries.IntegralHeight = false;
        LB_Entries.Location = new System.Drawing.Point(0, 0);
        LB_Entries.Name = "LB_Entries";
        LB_Entries.Size = new System.Drawing.Size(180, 302);
        LB_Entries.TabIndex = 0;
        // 
        // FLP_ListActions
        // 
        FLP_ListActions.Controls.Add(B_Import);
        FLP_ListActions.Controls.Add(B_Export);
        FLP_ListActions.Dock = System.Windows.Forms.DockStyle.Bottom;
        FLP_ListActions.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
        FLP_ListActions.Location = new System.Drawing.Point(0, 302);
        FLP_ListActions.Margin = new System.Windows.Forms.Padding(0);
        FLP_ListActions.Name = "FLP_ListActions";
        FLP_ListActions.Padding = new System.Windows.Forms.Padding(4);
        FLP_ListActions.Size = new System.Drawing.Size(180, 58);
        FLP_ListActions.TabIndex = 1;
        // 
        // B_Import
        // 
        B_Import.AutoSize = true;
        B_Import.Location = new System.Drawing.Point(7, 7);
        B_Import.Name = "B_Import";
        B_Import.Size = new System.Drawing.Size(59, 27);
        B_Import.TabIndex = 0;
        B_Import.Text = "Import";
        B_Import.UseVisualStyleBackColor = true;
        // 
        // B_Export
        // 
        B_Export.AutoSize = true;
        B_Export.Location = new System.Drawing.Point(72, 7);
        B_Export.Name = "B_Export";
        B_Export.Size = new System.Drawing.Size(58, 27);
        B_Export.TabIndex = 1;
        B_Export.Text = "Export";
        B_Export.UseVisualStyleBackColor = true;
        // 
        // TC_Editor
        // 
        TC_Editor.Controls.Add(Tab_General);
        TC_Editor.Controls.Add(Tab_Specific);
        TC_Editor.Dock = System.Windows.Forms.DockStyle.Fill;
        TC_Editor.Location = new System.Drawing.Point(180, 0);
        TC_Editor.Name = "TC_Editor";
        TC_Editor.SelectedIndex = 0;
        TC_Editor.Size = new System.Drawing.Size(420, 360);
        TC_Editor.TabIndex = 1;
        // 
        // Tab_General
        // 
        Tab_General.Location = new System.Drawing.Point(4, 26);
        Tab_General.Name = "Tab_General";
        Tab_General.Padding = new System.Windows.Forms.Padding(0);
        Tab_General.Size = new System.Drawing.Size(412, 330);
        Tab_General.TabIndex = 0;
        Tab_General.Text = "General";
        Tab_General.UseVisualStyleBackColor = true;
        // 
        // Tab_Specific
        // 
        Tab_Specific.Location = new System.Drawing.Point(4, 26);
        Tab_Specific.Name = "Tab_Specific";
        Tab_Specific.Padding = new System.Windows.Forms.Padding(0);
        Tab_Specific.Size = new System.Drawing.Size(412, 330);
        Tab_Specific.TabIndex = 1;
        Tab_Specific.Text = "Specific";
        Tab_Specific.UseVisualStyleBackColor = true;
        // 
        // JoinAvenueListEditor
        // 
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
        Controls.Add(TC_Editor);
        Controls.Add(P_List);
        Name = "JoinAvenueListEditor";
        Size = new System.Drawing.Size(600, 360);
        P_List.ResumeLayout(false);
        FLP_ListActions.ResumeLayout(false);
        FLP_ListActions.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    protected System.Windows.Forms.Panel P_List;
    protected System.Windows.Forms.ListBox LB_Entries;
    protected System.Windows.Forms.FlowLayoutPanel FLP_ListActions;
    protected System.Windows.Forms.Button B_Import;
    protected System.Windows.Forms.Button B_Export;
    protected System.Windows.Forms.TabControl TC_Editor;
    protected System.Windows.Forms.TabPage Tab_General;
    protected System.Windows.Forms.TabPage Tab_Specific;
}
