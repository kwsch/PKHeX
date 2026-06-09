namespace PKHeX.WinForms
{
    partial class SAV_GlobalLink5
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            ButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            B_Save = new System.Windows.Forms.Button();
            B_Cancel = new System.Windows.Forms.Button();
            TC_Main = new System.Windows.Forms.TabControl();
            Tab_General = new System.Windows.Forms.TabPage();
            TLP_Main = new System.Windows.Forms.TableLayoutPanel();
            L_UploadCount = new System.Windows.Forms.Label();
            L_UploadDate = new System.Windows.Forms.Label();
            L_UploadStatus = new System.Windows.Forms.Label();
            NUD_UploadCount = new System.Windows.Forms.NumericUpDown();
            CHK_IsSlotPresent = new System.Windows.Forms.CheckBox();
            L_Musical = new System.Windows.Forms.Label();
            L_CGearSkin = new System.Windows.Forms.Label();
            L_DexSkin = new System.Windows.Forms.Label();
            NUD_DexSkin = new System.Windows.Forms.NumericUpDown();
            NUD_CGearSkin = new System.Windows.Forms.NumericUpDown();
            NUD_Musical = new System.Windows.Forms.NumericUpDown();
            CHK_IsRegistered = new System.Windows.Forms.CheckBox();
            CHK_IsFullAccess = new System.Windows.Forms.CheckBox();
            NUD_UploadStatus = new System.Windows.Forms.NumericUpDown();
            FLP_Date = new System.Windows.Forms.FlowLayoutPanel();
            CHK_DateSet = new System.Windows.Forms.CheckBox();
            CAL_UploadDate = new System.Windows.Forms.DateTimePicker();
            Tab_Items = new System.Windows.Forms.TabPage();
            DGV_Items = new PKHeX.WinForms.Controls.DoubleBufferedDataGridView();
            Tab_Furniture = new System.Windows.Forms.TabPage();
            TLP_Furniture = new System.Windows.Forms.TableLayoutPanel();
            TB_Furniture5 = new System.Windows.Forms.TextBox();
            TB_Furniture4 = new System.Windows.Forms.TextBox();
            TB_Furniture3 = new System.Windows.Forms.TextBox();
            TB_Furniture2 = new System.Windows.Forms.TextBox();
            NUD_Furniture1 = new System.Windows.Forms.NumericUpDown();
            NUD_Furniture2 = new System.Windows.Forms.NumericUpDown();
            NUD_Furniture3 = new System.Windows.Forms.NumericUpDown();
            NUD_Furniture4 = new System.Windows.Forms.NumericUpDown();
            NUD_Furniture5 = new System.Windows.Forms.NumericUpDown();
            TB_Furniture1 = new System.Windows.Forms.TextBox();
            CHK_FurnitureSynchronized = new System.Windows.Forms.CheckBox();
            NUD_FurnitureSelected = new System.Windows.Forms.NumericUpDown();
            L_FurnitureSelected = new System.Windows.Forms.Label();
            ButtonPanel.SuspendLayout();
            TC_Main.SuspendLayout();
            Tab_General.SuspendLayout();
            TLP_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_UploadCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_DexSkin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_CGearSkin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Musical).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_UploadStatus).BeginInit();
            FLP_Date.SuspendLayout();
            Tab_Items.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_Items).BeginInit();
            Tab_Furniture.SuspendLayout();
            TLP_Furniture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Furniture1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Furniture2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Furniture3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Furniture4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Furniture5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_FurnitureSelected).BeginInit();
            SuspendLayout();
            // 
            // ButtonPanel
            // 
            ButtonPanel.AutoSize = true;
            ButtonPanel.Controls.Add(B_Save);
            ButtonPanel.Controls.Add(B_Cancel);
            ButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            ButtonPanel.Location = new System.Drawing.Point(0, 332);
            ButtonPanel.Name = "ButtonPanel";
            ButtonPanel.Padding = new System.Windows.Forms.Padding(8);
            ButtonPanel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            ButtonPanel.Size = new System.Drawing.Size(424, 49);
            ButtonPanel.TabIndex = 1;
            ButtonPanel.WrapContents = false;
            // 
            // B_Save
            // 
            B_Save.AutoSize = true;
            B_Save.Location = new System.Drawing.Point(353, 11);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(52, 27);
            B_Save.TabIndex = 0;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_Cancel
            // 
            B_Cancel.AutoSize = true;
            B_Cancel.Location = new System.Drawing.Point(284, 11);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(63, 27);
            B_Cancel.TabIndex = 1;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // TC_Main
            // 
            TC_Main.Controls.Add(Tab_General);
            TC_Main.Controls.Add(Tab_Items);
            TC_Main.Controls.Add(Tab_Furniture);
            TC_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            TC_Main.Location = new System.Drawing.Point(0, 0);
            TC_Main.Name = "TC_Main";
            TC_Main.SelectedIndex = 0;
            TC_Main.Size = new System.Drawing.Size(424, 332);
            TC_Main.TabIndex = 2;
            // 
            // Tab_General
            // 
            Tab_General.Controls.Add(TLP_Main);
            Tab_General.Location = new System.Drawing.Point(4, 26);
            Tab_General.Margin = new System.Windows.Forms.Padding(0);
            Tab_General.Name = "Tab_General";
            Tab_General.Padding = new System.Windows.Forms.Padding(8);
            Tab_General.Size = new System.Drawing.Size(416, 302);
            Tab_General.TabIndex = 0;
            Tab_General.Text = "General";
            Tab_General.UseVisualStyleBackColor = true;
            // 
            // TLP_Main
            // 
            TLP_Main.ColumnCount = 2;
            TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            TLP_Main.Controls.Add(L_UploadCount, 0, 1);
            TLP_Main.Controls.Add(L_UploadDate, 0, 0);
            TLP_Main.Controls.Add(L_UploadStatus, 0, 2);
            TLP_Main.Controls.Add(NUD_UploadCount, 1, 1);
            TLP_Main.Controls.Add(CHK_IsSlotPresent, 1, 3);
            TLP_Main.Controls.Add(L_Musical, 0, 6);
            TLP_Main.Controls.Add(L_CGearSkin, 0, 7);
            TLP_Main.Controls.Add(L_DexSkin, 0, 8);
            TLP_Main.Controls.Add(NUD_DexSkin, 1, 8);
            TLP_Main.Controls.Add(NUD_CGearSkin, 1, 7);
            TLP_Main.Controls.Add(NUD_Musical, 1, 6);
            TLP_Main.Controls.Add(CHK_IsRegistered, 1, 4);
            TLP_Main.Controls.Add(CHK_IsFullAccess, 1, 5);
            TLP_Main.Controls.Add(NUD_UploadStatus, 1, 2);
            TLP_Main.Controls.Add(FLP_Date, 1, 0);
            TLP_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Main.Location = new System.Drawing.Point(8, 8);
            TLP_Main.Margin = new System.Windows.Forms.Padding(0);
            TLP_Main.Name = "TLP_Main";
            TLP_Main.RowCount = 10;
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_Main.Size = new System.Drawing.Size(400, 286);
            TLP_Main.TabIndex = 3;
            // 
            // L_UploadCount
            // 
            L_UploadCount.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_UploadCount.AutoSize = true;
            L_UploadCount.Location = new System.Drawing.Point(4, 38);
            L_UploadCount.Name = "L_UploadCount";
            L_UploadCount.Size = new System.Drawing.Size(92, 17);
            L_UploadCount.TabIndex = 3;
            L_UploadCount.Text = "Upload Count:";
            // 
            // L_UploadDate
            // 
            L_UploadDate.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_UploadDate.AutoSize = true;
            L_UploadDate.Location = new System.Drawing.Point(11, 7);
            L_UploadDate.Name = "L_UploadDate";
            L_UploadDate.Size = new System.Drawing.Size(85, 17);
            L_UploadDate.TabIndex = 1;
            L_UploadDate.Text = "Upload Date:";
            // 
            // L_UploadStatus
            // 
            L_UploadStatus.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_UploadStatus.AutoSize = true;
            L_UploadStatus.Location = new System.Drawing.Point(3, 69);
            L_UploadStatus.Name = "L_UploadStatus";
            L_UploadStatus.Size = new System.Drawing.Size(93, 17);
            L_UploadStatus.TabIndex = 5;
            L_UploadStatus.Text = "Upload Status:";
            // 
            // NUD_UploadCount
            // 
            NUD_UploadCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
            NUD_UploadCount.Location = new System.Drawing.Point(102, 34);
            NUD_UploadCount.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            NUD_UploadCount.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
            NUD_UploadCount.Name = "NUD_UploadCount";
            NUD_UploadCount.Size = new System.Drawing.Size(100, 25);
            NUD_UploadCount.TabIndex = 4;
            // 
            // CHK_IsSlotPresent
            // 
            CHK_IsSlotPresent.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CHK_IsSlotPresent.AutoSize = true;
            CHK_IsSlotPresent.Location = new System.Drawing.Point(102, 96);
            CHK_IsSlotPresent.Name = "CHK_IsSlotPresent";
            CHK_IsSlotPresent.Size = new System.Drawing.Size(155, 21);
            CHK_IsSlotPresent.TabIndex = 7;
            CHK_IsSlotPresent.Text = "Upload Slot Tucked In";
            CHK_IsSlotPresent.UseVisualStyleBackColor = true;
            // 
            // L_Musical
            // 
            L_Musical.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_Musical.AutoSize = true;
            L_Musical.Location = new System.Drawing.Point(41, 181);
            L_Musical.Name = "L_Musical";
            L_Musical.Size = new System.Drawing.Size(55, 17);
            L_Musical.TabIndex = 10;
            L_Musical.Text = "Musical:";
            // 
            // L_CGearSkin
            // 
            L_CGearSkin.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_CGearSkin.AutoSize = true;
            L_CGearSkin.Location = new System.Drawing.Point(22, 212);
            L_CGearSkin.Name = "L_CGearSkin";
            L_CGearSkin.Size = new System.Drawing.Size(74, 17);
            L_CGearSkin.TabIndex = 12;
            L_CGearSkin.Text = "CGear Skin:";
            // 
            // L_DexSkin
            // 
            L_DexSkin.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_DexSkin.AutoSize = true;
            L_DexSkin.Location = new System.Drawing.Point(9, 243);
            L_DexSkin.Name = "L_DexSkin";
            L_DexSkin.Size = new System.Drawing.Size(87, 17);
            L_DexSkin.TabIndex = 14;
            L_DexSkin.Text = "Pokédex Skin:";
            // 
            // NUD_DexSkin
            // 
            NUD_DexSkin.Anchor = System.Windows.Forms.AnchorStyles.Left;
            NUD_DexSkin.Location = new System.Drawing.Point(102, 239);
            NUD_DexSkin.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_DexSkin.Name = "NUD_DexSkin";
            NUD_DexSkin.Size = new System.Drawing.Size(48, 25);
            NUD_DexSkin.TabIndex = 15;
            // 
            // NUD_CGearSkin
            // 
            NUD_CGearSkin.Anchor = System.Windows.Forms.AnchorStyles.Left;
            NUD_CGearSkin.Location = new System.Drawing.Point(102, 208);
            NUD_CGearSkin.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_CGearSkin.Name = "NUD_CGearSkin";
            NUD_CGearSkin.Size = new System.Drawing.Size(48, 25);
            NUD_CGearSkin.TabIndex = 13;
            // 
            // NUD_Musical
            // 
            NUD_Musical.Anchor = System.Windows.Forms.AnchorStyles.Left;
            NUD_Musical.Location = new System.Drawing.Point(102, 177);
            NUD_Musical.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_Musical.Name = "NUD_Musical";
            NUD_Musical.Size = new System.Drawing.Size(48, 25);
            NUD_Musical.TabIndex = 11;
            // 
            // CHK_IsRegistered
            // 
            CHK_IsRegistered.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CHK_IsRegistered.AutoSize = true;
            CHK_IsRegistered.Location = new System.Drawing.Point(102, 123);
            CHK_IsRegistered.Name = "CHK_IsRegistered";
            CHK_IsRegistered.Size = new System.Drawing.Size(160, 21);
            CHK_IsRegistered.TabIndex = 8;
            CHK_IsRegistered.Text = "Game Card Registered";
            CHK_IsRegistered.UseVisualStyleBackColor = true;
            // 
            // CHK_IsFullAccess
            // 
            CHK_IsFullAccess.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CHK_IsFullAccess.AutoSize = true;
            CHK_IsFullAccess.Location = new System.Drawing.Point(102, 150);
            CHK_IsFullAccess.Name = "CHK_IsFullAccess";
            CHK_IsFullAccess.Size = new System.Drawing.Size(89, 21);
            CHK_IsFullAccess.TabIndex = 9;
            CHK_IsFullAccess.Text = "Full Access";
            CHK_IsFullAccess.UseVisualStyleBackColor = true;
            // 
            // NUD_UploadStatus
            // 
            NUD_UploadStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            NUD_UploadStatus.Location = new System.Drawing.Point(102, 65);
            NUD_UploadStatus.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_UploadStatus.Name = "NUD_UploadStatus";
            NUD_UploadStatus.Size = new System.Drawing.Size(48, 25);
            NUD_UploadStatus.TabIndex = 6;
            // 
            // FLP_Date
            // 
            FLP_Date.AutoSize = true;
            FLP_Date.Controls.Add(CHK_DateSet);
            FLP_Date.Controls.Add(CAL_UploadDate);
            FLP_Date.Dock = System.Windows.Forms.DockStyle.Fill;
            FLP_Date.Location = new System.Drawing.Point(99, 0);
            FLP_Date.Margin = new System.Windows.Forms.Padding(0);
            FLP_Date.Name = "FLP_Date";
            FLP_Date.Size = new System.Drawing.Size(301, 31);
            FLP_Date.TabIndex = 16;
            // 
            // CHK_DateSet
            // 
            CHK_DateSet.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CHK_DateSet.AutoSize = true;
            CHK_DateSet.Location = new System.Drawing.Point(0, 5);
            CHK_DateSet.Margin = new System.Windows.Forms.Padding(0);
            CHK_DateSet.Name = "CHK_DateSet";
            CHK_DateSet.Size = new System.Drawing.Size(45, 21);
            CHK_DateSet.TabIndex = 4;
            CHK_DateSet.Text = "Set";
            CHK_DateSet.UseVisualStyleBackColor = true;
            CHK_DateSet.CheckedChanged += CHK_DateSet_CheckedChanged;
            // 
            // CAL_UploadDate
            // 
            CAL_UploadDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            CAL_UploadDate.Location = new System.Drawing.Point(48, 3);
            CAL_UploadDate.MaxDate = new System.DateTime(2099, 12, 31, 0, 0, 0, 0);
            CAL_UploadDate.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            CAL_UploadDate.Name = "CAL_UploadDate";
            CAL_UploadDate.Size = new System.Drawing.Size(228, 25);
            CAL_UploadDate.TabIndex = 5;
            // 
            // Tab_Items
            // 
            Tab_Items.Controls.Add(DGV_Items);
            Tab_Items.Location = new System.Drawing.Point(4, 26);
            Tab_Items.Name = "Tab_Items";
            Tab_Items.Size = new System.Drawing.Size(396, 302);
            Tab_Items.TabIndex = 1;
            Tab_Items.Text = "Items";
            Tab_Items.UseVisualStyleBackColor = true;
            // 
            // DGV_Items
            // 
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.ControlLight;
            DGV_Items.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            DGV_Items.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_Items.Dock = System.Windows.Forms.DockStyle.Fill;
            DGV_Items.Location = new System.Drawing.Point(0, 0);
            DGV_Items.Name = "DGV_Items";
            DGV_Items.Size = new System.Drawing.Size(396, 302);
            DGV_Items.TabIndex = 0;
            // 
            // Tab_Furniture
            // 
            Tab_Furniture.Controls.Add(TLP_Furniture);
            Tab_Furniture.Location = new System.Drawing.Point(4, 26);
            Tab_Furniture.Name = "Tab_Furniture";
            Tab_Furniture.Padding = new System.Windows.Forms.Padding(3);
            Tab_Furniture.Size = new System.Drawing.Size(416, 302);
            Tab_Furniture.TabIndex = 2;
            Tab_Furniture.Text = "Furniture";
            Tab_Furniture.UseVisualStyleBackColor = true;
            // 
            // TLP_Furniture
            // 
            TLP_Furniture.ColumnCount = 2;
            TLP_Furniture.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            TLP_Furniture.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_Furniture.Controls.Add(TB_Furniture5, 1, 4);
            TLP_Furniture.Controls.Add(TB_Furniture4, 1, 3);
            TLP_Furniture.Controls.Add(TB_Furniture3, 1, 2);
            TLP_Furniture.Controls.Add(TB_Furniture2, 1, 1);
            TLP_Furniture.Controls.Add(NUD_Furniture1, 0, 0);
            TLP_Furniture.Controls.Add(NUD_Furniture2, 0, 1);
            TLP_Furniture.Controls.Add(NUD_Furniture3, 0, 2);
            TLP_Furniture.Controls.Add(NUD_Furniture4, 0, 3);
            TLP_Furniture.Controls.Add(NUD_Furniture5, 0, 4);
            TLP_Furniture.Controls.Add(TB_Furniture1, 1, 0);
            TLP_Furniture.Controls.Add(CHK_FurnitureSynchronized, 1, 5);
            TLP_Furniture.Controls.Add(NUD_FurnitureSelected, 1, 6);
            TLP_Furniture.Controls.Add(L_FurnitureSelected, 0, 6);
            TLP_Furniture.Dock = System.Windows.Forms.DockStyle.Fill;
            TLP_Furniture.Location = new System.Drawing.Point(3, 3);
            TLP_Furniture.Margin = new System.Windows.Forms.Padding(0);
            TLP_Furniture.Name = "TLP_Furniture";
            TLP_Furniture.Padding = new System.Windows.Forms.Padding(8);
            TLP_Furniture.RowCount = 8;
            TLP_Furniture.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Furniture.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Furniture.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Furniture.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Furniture.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Furniture.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Furniture.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TLP_Furniture.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TLP_Furniture.Size = new System.Drawing.Size(410, 296);
            TLP_Furniture.TabIndex = 0;
            // 
            // TB_Furniture5
            // 
            TB_Furniture5.Location = new System.Drawing.Point(81, 135);
            TB_Furniture5.Name = "TB_Furniture5";
            TB_Furniture5.Size = new System.Drawing.Size(160, 25);
            TB_Furniture5.TabIndex = 10;
            // 
            // TB_Furniture4
            // 
            TB_Furniture4.Location = new System.Drawing.Point(81, 104);
            TB_Furniture4.Name = "TB_Furniture4";
            TB_Furniture4.Size = new System.Drawing.Size(160, 25);
            TB_Furniture4.TabIndex = 8;
            // 
            // TB_Furniture3
            // 
            TB_Furniture3.Location = new System.Drawing.Point(81, 73);
            TB_Furniture3.Name = "TB_Furniture3";
            TB_Furniture3.Size = new System.Drawing.Size(160, 25);
            TB_Furniture3.TabIndex = 6;
            // 
            // TB_Furniture2
            // 
            TB_Furniture2.Location = new System.Drawing.Point(81, 42);
            TB_Furniture2.Name = "TB_Furniture2";
            TB_Furniture2.Size = new System.Drawing.Size(160, 25);
            TB_Furniture2.TabIndex = 4;
            // 
            // NUD_Furniture1
            // 
            NUD_Furniture1.Location = new System.Drawing.Point(11, 11);
            NUD_Furniture1.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NUD_Furniture1.Name = "NUD_Furniture1";
            NUD_Furniture1.Size = new System.Drawing.Size(64, 25);
            NUD_Furniture1.TabIndex = 1;
            // 
            // NUD_Furniture2
            // 
            NUD_Furniture2.Location = new System.Drawing.Point(11, 42);
            NUD_Furniture2.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NUD_Furniture2.Name = "NUD_Furniture2";
            NUD_Furniture2.Size = new System.Drawing.Size(64, 25);
            NUD_Furniture2.TabIndex = 3;
            // 
            // NUD_Furniture3
            // 
            NUD_Furniture3.Location = new System.Drawing.Point(11, 73);
            NUD_Furniture3.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NUD_Furniture3.Name = "NUD_Furniture3";
            NUD_Furniture3.Size = new System.Drawing.Size(64, 25);
            NUD_Furniture3.TabIndex = 5;
            // 
            // NUD_Furniture4
            // 
            NUD_Furniture4.Location = new System.Drawing.Point(11, 104);
            NUD_Furniture4.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NUD_Furniture4.Name = "NUD_Furniture4";
            NUD_Furniture4.Size = new System.Drawing.Size(64, 25);
            NUD_Furniture4.TabIndex = 7;
            // 
            // NUD_Furniture5
            // 
            NUD_Furniture5.Location = new System.Drawing.Point(11, 135);
            NUD_Furniture5.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NUD_Furniture5.Name = "NUD_Furniture5";
            NUD_Furniture5.Size = new System.Drawing.Size(64, 25);
            NUD_Furniture5.TabIndex = 9;
            // 
            // TB_Furniture1
            // 
            TB_Furniture1.Location = new System.Drawing.Point(81, 11);
            TB_Furniture1.Name = "TB_Furniture1";
            TB_Furniture1.Size = new System.Drawing.Size(160, 25);
            TB_Furniture1.TabIndex = 2;
            // 
            // CHK_FurnitureSynchronized
            // 
            CHK_FurnitureSynchronized.AutoSize = true;
            CHK_FurnitureSynchronized.Location = new System.Drawing.Point(81, 166);
            CHK_FurnitureSynchronized.Name = "CHK_FurnitureSynchronized";
            CHK_FurnitureSynchronized.Size = new System.Drawing.Size(104, 21);
            CHK_FurnitureSynchronized.TabIndex = 17;
            CHK_FurnitureSynchronized.Text = "Synchronized";
            CHK_FurnitureSynchronized.UseVisualStyleBackColor = true;
            // 
            // NUD_FurnitureSelected
            // 
            NUD_FurnitureSelected.Location = new System.Drawing.Point(81, 193);
            NUD_FurnitureSelected.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NUD_FurnitureSelected.Name = "NUD_FurnitureSelected";
            NUD_FurnitureSelected.Size = new System.Drawing.Size(48, 25);
            NUD_FurnitureSelected.TabIndex = 19;
            // 
            // L_FurnitureSelected
            // 
            L_FurnitureSelected.Anchor = System.Windows.Forms.AnchorStyles.Right;
            L_FurnitureSelected.AutoSize = true;
            L_FurnitureSelected.Location = new System.Drawing.Point(15, 197);
            L_FurnitureSelected.Name = "L_FurnitureSelected";
            L_FurnitureSelected.Size = new System.Drawing.Size(60, 17);
            L_FurnitureSelected.TabIndex = 20;
            L_FurnitureSelected.Text = "Selected:";
            // 
            // SAV_GlobalLink5
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(424, 381);
            Controls.Add(TC_Main);
            Controls.Add(ButtonPanel);
            Icon = Properties.Resources.Icon;
            MaximizeBox = false;
            MinimumSize = new System.Drawing.Size(440, 420);
            Name = "SAV_GlobalLink5";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Global Link";
            ButtonPanel.ResumeLayout(false);
            ButtonPanel.PerformLayout();
            TC_Main.ResumeLayout(false);
            Tab_General.ResumeLayout(false);
            TLP_Main.ResumeLayout(false);
            TLP_Main.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_UploadCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_DexSkin).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_CGearSkin).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Musical).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_UploadStatus).EndInit();
            FLP_Date.ResumeLayout(false);
            FLP_Date.PerformLayout();
            Tab_Items.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DGV_Items).EndInit();
            Tab_Furniture.ResumeLayout(false);
            TLP_Furniture.ResumeLayout(false);
            TLP_Furniture.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Furniture1).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Furniture2).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Furniture3).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Furniture4).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Furniture5).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_FurnitureSelected).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel ButtonPanel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.TabControl TC_Main;
        private System.Windows.Forms.TabPage Tab_General;
        private System.Windows.Forms.TableLayoutPanel TLP_Main;
        private System.Windows.Forms.Label L_UploadCount;
        private System.Windows.Forms.Label L_UploadDate;
        private System.Windows.Forms.Label L_UploadStatus;
        private System.Windows.Forms.TabPage Tab_Items;
        private System.Windows.Forms.NumericUpDown NUD_UploadCount;
        private System.Windows.Forms.CheckBox CHK_IsSlotPresent;
        private System.Windows.Forms.TabPage Tab_Furniture;
        private System.Windows.Forms.Label L_Musical;
        private System.Windows.Forms.Label L_CGearSkin;
        private System.Windows.Forms.Label L_DexSkin;
        private System.Windows.Forms.NumericUpDown NUD_DexSkin;
        private System.Windows.Forms.NumericUpDown NUD_CGearSkin;
        private System.Windows.Forms.NumericUpDown NUD_Musical;
        private System.Windows.Forms.CheckBox CHK_IsRegistered;
        private System.Windows.Forms.CheckBox CHK_IsFullAccess;
        private System.Windows.Forms.NumericUpDown NUD_UploadStatus;
        private Controls.DoubleBufferedDataGridView DGV_Items;
        private System.Windows.Forms.TableLayoutPanel TLP_Furniture;
        private System.Windows.Forms.NumericUpDown NUD_Furniture1;
        private System.Windows.Forms.NumericUpDown NUD_Furniture2;
        private System.Windows.Forms.NumericUpDown NUD_Furniture3;
        private System.Windows.Forms.NumericUpDown NUD_Furniture4;
        private System.Windows.Forms.NumericUpDown NUD_Furniture5;
        private System.Windows.Forms.TextBox TB_Furniture1;
        private System.Windows.Forms.TextBox TB_Furniture5;
        private System.Windows.Forms.TextBox TB_Furniture4;
        private System.Windows.Forms.TextBox TB_Furniture3;
        private System.Windows.Forms.TextBox TB_Furniture2;
        private System.Windows.Forms.CheckBox CHK_FurnitureSynchronized;
        private System.Windows.Forms.NumericUpDown NUD_FurnitureSelected;
        private System.Windows.Forms.Label L_FurnitureSelected;
        private System.Windows.Forms.FlowLayoutPanel FLP_Date;
        private System.Windows.Forms.CheckBox CHK_DateSet;
        private System.Windows.Forms.DateTimePicker CAL_UploadDate;
    }
}
