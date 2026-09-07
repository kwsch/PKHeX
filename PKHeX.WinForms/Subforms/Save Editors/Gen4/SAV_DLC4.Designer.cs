namespace PKHeX.WinForms;

partial class SAV_DLC4
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        TL_Main = new System.Windows.Forms.TableLayoutPanel();
        TL_Content = new System.Windows.Forms.TableLayoutPanel();
        TL_Slots = new System.Windows.Forms.TableLayoutPanel();
        L_Slots = new System.Windows.Forms.Label();
        LB_BattleVideos = new System.Windows.Forms.ListBox();
        TL_Detail = new System.Windows.Forms.TableLayoutPanel();
        L_VideoTitle = new System.Windows.Forms.Label();
        L_VideoStatus = new System.Windows.Forms.Label();
        P_Teams = new System.Windows.Forms.Panel();
        TL_Teams = new System.Windows.Forms.TableLayoutPanel();
        TL_Export = new System.Windows.Forms.TableLayoutPanel();
        CHK_ExportDecrypted = new System.Windows.Forms.CheckBox();
        B_Import = new System.Windows.Forms.Button();
        B_Export = new System.Windows.Forms.Button();
        TL_Actions = new System.Windows.Forms.TableLayoutPanel();
        B_Cancel = new System.Windows.Forms.Button();
        B_Save = new System.Windows.Forms.Button();
        TL_Main.SuspendLayout();
        TL_Content.SuspendLayout();
        TL_Slots.SuspendLayout();
        TL_Detail.SuspendLayout();
        P_Teams.SuspendLayout();
        TL_Export.SuspendLayout();
        TL_Actions.SuspendLayout();
        SuspendLayout();
        // 
        // TL_Main
        // 
        TL_Main.ColumnCount = 1;
        TL_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        TL_Main.Controls.Add(TL_Content, 0, 0);
        TL_Main.Controls.Add(TL_Actions, 0, 1);
        TL_Main.Dock = System.Windows.Forms.DockStyle.Fill;
        TL_Main.Location = new System.Drawing.Point(0, 0);
        TL_Main.Margin = new System.Windows.Forms.Padding(0);
        TL_Main.Name = "TL_Main";
        TL_Main.RowCount = 2;
        TL_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        TL_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
        TL_Main.Size = new System.Drawing.Size(744, 545);
        TL_Main.TabIndex = 0;
        // 
        // TL_Content
        // 
        TL_Content.ColumnCount = 2;
        TL_Content.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 190F));
        TL_Content.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        TL_Content.Controls.Add(TL_Slots, 0, 0);
        TL_Content.Controls.Add(TL_Detail, 1, 0);
        TL_Content.Dock = System.Windows.Forms.DockStyle.Fill;
        TL_Content.Location = new System.Drawing.Point(0, 0);
        TL_Content.Margin = new System.Windows.Forms.Padding(0);
        TL_Content.Name = "TL_Content";
        TL_Content.RowCount = 1;
        TL_Content.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        TL_Content.Size = new System.Drawing.Size(744, 513);
        TL_Content.TabIndex = 0;
        // 
        // TL_Slots
        // 
        TL_Slots.ColumnCount = 1;
        TL_Slots.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        TL_Slots.Controls.Add(L_Slots, 0, 0);
        TL_Slots.Controls.Add(LB_BattleVideos, 0, 1);
        TL_Slots.Dock = System.Windows.Forms.DockStyle.Fill;
        TL_Slots.Location = new System.Drawing.Point(0, 0);
        TL_Slots.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
        TL_Slots.Name = "TL_Slots";
        TL_Slots.RowCount = 2;
        TL_Slots.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
        TL_Slots.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        TL_Slots.Size = new System.Drawing.Size(182, 513);
        TL_Slots.TabIndex = 0;
        // 
        // L_Slots
        // 
        L_Slots.AutoSize = true;
        L_Slots.Dock = System.Windows.Forms.DockStyle.Fill;
        L_Slots.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
        L_Slots.Location = new System.Drawing.Point(3, 0);
        L_Slots.Name = "L_Slots";
        L_Slots.Size = new System.Drawing.Size(176, 32);
        L_Slots.TabIndex = 0;
        L_Slots.Text = "Battle Videos";
        L_Slots.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // LB_BattleVideos
        // 
        LB_BattleVideos.Dock = System.Windows.Forms.DockStyle.Fill;
        LB_BattleVideos.FormattingEnabled = true;
        LB_BattleVideos.IntegralHeight = false;
        LB_BattleVideos.Location = new System.Drawing.Point(3, 35);
        LB_BattleVideos.Name = "LB_BattleVideos";
        LB_BattleVideos.Size = new System.Drawing.Size(176, 475);
        LB_BattleVideos.TabIndex = 1;
        LB_BattleVideos.SelectedIndexChanged += LB_BattleVideos_SelectedIndexChanged;
        // 
        // TL_Detail
        // 
        TL_Detail.ColumnCount = 1;
        TL_Detail.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        TL_Detail.Controls.Add(L_VideoTitle, 0, 0);
        TL_Detail.Controls.Add(L_VideoStatus, 0, 1);
        TL_Detail.Controls.Add(P_Teams, 0, 2);
        TL_Detail.Controls.Add(TL_Export, 0, 3);
        TL_Detail.Dock = System.Windows.Forms.DockStyle.Fill;
        TL_Detail.Location = new System.Drawing.Point(190, 0);
        TL_Detail.Margin = new System.Windows.Forms.Padding(0);
        TL_Detail.Name = "TL_Detail";
        TL_Detail.RowCount = 4;
        TL_Detail.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
        TL_Detail.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
        TL_Detail.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        TL_Detail.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
        TL_Detail.Size = new System.Drawing.Size(554, 513);
        TL_Detail.TabIndex = 1;
        // 
        // L_VideoTitle
        // 
        L_VideoTitle.AutoSize = true;
        L_VideoTitle.Dock = System.Windows.Forms.DockStyle.Fill;
        L_VideoTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
        L_VideoTitle.Location = new System.Drawing.Point(0, 0);
        L_VideoTitle.Margin = new System.Windows.Forms.Padding(0);
        L_VideoTitle.Name = "L_VideoTitle";
        L_VideoTitle.Size = new System.Drawing.Size(554, 32);
        L_VideoTitle.TabIndex = 0;
        L_VideoTitle.Text = "Battle Video Title";
        L_VideoTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // L_VideoStatus
        // 
        L_VideoStatus.AutoEllipsis = true;
        L_VideoStatus.Dock = System.Windows.Forms.DockStyle.Fill;
        L_VideoStatus.Location = new System.Drawing.Point(0, 32);
        L_VideoStatus.Margin = new System.Windows.Forms.Padding(0);
        L_VideoStatus.Name = "L_VideoStatus";
        L_VideoStatus.Size = new System.Drawing.Size(554, 32);
        L_VideoStatus.TabIndex = 1;
        L_VideoStatus.Text = "Select a Battle Video.";
        L_VideoStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // P_Teams
        // 
        P_Teams.AutoScroll = true;
        P_Teams.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        P_Teams.Controls.Add(TL_Teams);
        P_Teams.Dock = System.Windows.Forms.DockStyle.Fill;
        P_Teams.Location = new System.Drawing.Point(0, 64);
        P_Teams.Margin = new System.Windows.Forms.Padding(0);
        P_Teams.Name = "P_Teams";
        P_Teams.Size = new System.Drawing.Size(554, 417);
        P_Teams.TabIndex = 2;
        // 
        // TL_Teams
        // 
        TL_Teams.AutoSize = true;
        TL_Teams.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        TL_Teams.ColumnCount = 2;
        TL_Teams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
        TL_Teams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
        TL_Teams.Dock = System.Windows.Forms.DockStyle.Top;
        TL_Teams.Location = new System.Drawing.Point(0, 0);
        TL_Teams.Margin = new System.Windows.Forms.Padding(0);
        TL_Teams.Name = "TL_Teams";
        TL_Teams.RowCount = 1;
        TL_Teams.RowStyles.Add(new System.Windows.Forms.RowStyle());
        TL_Teams.Size = new System.Drawing.Size(552, 0);
        TL_Teams.TabIndex = 0;
        // 
        // TL_Export
        // 
        TL_Export.ColumnCount = 3;
        TL_Export.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        TL_Export.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
        TL_Export.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
        TL_Export.Controls.Add(CHK_ExportDecrypted, 0, 0);
        TL_Export.Controls.Add(B_Import, 1, 0);
        TL_Export.Controls.Add(B_Export, 2, 0);
        TL_Export.Dock = System.Windows.Forms.DockStyle.Fill;
        TL_Export.Location = new System.Drawing.Point(0, 481);
        TL_Export.Margin = new System.Windows.Forms.Padding(0);
        TL_Export.Name = "TL_Export";
        TL_Export.RowCount = 1;
        TL_Export.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        TL_Export.Size = new System.Drawing.Size(554, 32);
        TL_Export.TabIndex = 3;
        // 
        // CHK_ExportDecrypted
        // 
        CHK_ExportDecrypted.Anchor = System.Windows.Forms.AnchorStyles.Left;
        CHK_ExportDecrypted.AutoSize = true;
        CHK_ExportDecrypted.Location = new System.Drawing.Point(3, 5);
        CHK_ExportDecrypted.Name = "CHK_ExportDecrypted";
        CHK_ExportDecrypted.Size = new System.Drawing.Size(164, 21);
        CHK_ExportDecrypted.TabIndex = 0;
        CHK_ExportDecrypted.Text = "Force decrypted export";
        CHK_ExportDecrypted.UseVisualStyleBackColor = true;
        // 
        // B_Import
        // 
        B_Import.Dock = System.Windows.Forms.DockStyle.Fill;
        B_Import.Location = new System.Drawing.Point(354, 0);
        B_Import.Margin = new System.Windows.Forms.Padding(0);
        B_Import.Name = "B_Import";
        B_Import.Size = new System.Drawing.Size(100, 32);
        B_Import.TabIndex = 1;
        B_Import.Text = "Import";
        B_Import.UseVisualStyleBackColor = true;
        B_Import.Click += B_Import_Click;
        // 
        // B_Export
        // 
        B_Export.Dock = System.Windows.Forms.DockStyle.Fill;
        B_Export.Location = new System.Drawing.Point(454, 0);
        B_Export.Margin = new System.Windows.Forms.Padding(0);
        B_Export.Name = "B_Export";
        B_Export.Size = new System.Drawing.Size(100, 32);
        B_Export.TabIndex = 2;
        B_Export.Text = "Export";
        B_Export.UseVisualStyleBackColor = true;
        B_Export.Click += B_Export_Click;
        // 
        // TL_Actions
        // 
        TL_Actions.ColumnCount = 3;
        TL_Actions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        TL_Actions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
        TL_Actions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
        TL_Actions.Controls.Add(B_Cancel, 1, 0);
        TL_Actions.Controls.Add(B_Save, 2, 0);
        TL_Actions.Dock = System.Windows.Forms.DockStyle.Fill;
        TL_Actions.Location = new System.Drawing.Point(0, 513);
        TL_Actions.Margin = new System.Windows.Forms.Padding(0);
        TL_Actions.Name = "TL_Actions";
        TL_Actions.RowCount = 1;
        TL_Actions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        TL_Actions.Size = new System.Drawing.Size(744, 32);
        TL_Actions.TabIndex = 1;
        // 
        // B_Cancel
        // 
        B_Cancel.Dock = System.Windows.Forms.DockStyle.Fill;
        B_Cancel.Location = new System.Drawing.Point(544, 0);
        B_Cancel.Margin = new System.Windows.Forms.Padding(0);
        B_Cancel.Name = "B_Cancel";
        B_Cancel.Size = new System.Drawing.Size(100, 32);
        B_Cancel.TabIndex = 0;
        B_Cancel.Text = "Cancel";
        B_Cancel.UseVisualStyleBackColor = true;
        B_Cancel.Click += B_Cancel_Click;
        // 
        // B_Save
        // 
        B_Save.Dock = System.Windows.Forms.DockStyle.Fill;
        B_Save.Location = new System.Drawing.Point(644, 0);
        B_Save.Margin = new System.Windows.Forms.Padding(0);
        B_Save.Name = "B_Save";
        B_Save.Size = new System.Drawing.Size(100, 32);
        B_Save.TabIndex = 1;
        B_Save.Text = "Save";
        B_Save.UseVisualStyleBackColor = true;
        B_Save.Click += B_Save_Click;
        // 
        // SAV_DLC4
        // 
        AcceptButton = B_Save;
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(744, 545);
        Controls.Add(TL_Main);
        Icon = Properties.Resources.Icon;
        MaximizeBox = false;
        MinimumSize = new System.Drawing.Size(760, 584);
        Name = "SAV_DLC4";
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        Text = "Generation 4 Battle Videos";
        TL_Main.ResumeLayout(false);
        TL_Content.ResumeLayout(false);
        TL_Slots.ResumeLayout(false);
        TL_Slots.PerformLayout();
        TL_Detail.ResumeLayout(false);
        TL_Detail.PerformLayout();
        P_Teams.ResumeLayout(false);
        P_Teams.PerformLayout();
        TL_Export.ResumeLayout(false);
        TL_Export.PerformLayout();
        TL_Actions.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel TL_Main;
    private System.Windows.Forms.TableLayoutPanel TL_Content;
    private System.Windows.Forms.TableLayoutPanel TL_Slots;
    private System.Windows.Forms.Label L_Slots;
    private System.Windows.Forms.ListBox LB_BattleVideos;
    private System.Windows.Forms.TableLayoutPanel TL_Detail;
    private System.Windows.Forms.Label L_VideoTitle;
    private System.Windows.Forms.Label L_VideoStatus;
    private System.Windows.Forms.Panel P_Teams;
    private System.Windows.Forms.TableLayoutPanel TL_Teams;
    private System.Windows.Forms.TableLayoutPanel TL_Export;
    private System.Windows.Forms.CheckBox CHK_ExportDecrypted;
    private System.Windows.Forms.Button B_Import;
    private System.Windows.Forms.Button B_Export;
    private System.Windows.Forms.TableLayoutPanel TL_Actions;
    private System.Windows.Forms.Button B_Cancel;
    private System.Windows.Forms.Button B_Save;
}
