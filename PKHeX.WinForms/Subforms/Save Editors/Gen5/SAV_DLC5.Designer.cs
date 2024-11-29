using PKHeX.WinForms.Controls;

namespace PKHeX.WinForms
{
    partial class SAV_DLC5
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
            PB_CGearBackground = new System.Windows.Forms.PictureBox();
            B_Cancel = new System.Windows.Forms.Button();
            B_Save = new System.Windows.Forms.Button();
            B_ImportPNG = new System.Windows.Forms.Button();
            B_ExportPNG = new System.Windows.Forms.Button();
            B_ExportCGB = new System.Windows.Forms.Button();
            B_ImportCGB = new System.Windows.Forms.Button();
            TC_Tabs = new VerticalTabControl();
            Tab_CGear = new System.Windows.Forms.TabPage();
            Tab_PokeDex = new System.Windows.Forms.TabPage();
            B_PokeDexBackgroundLoad = new System.Windows.Forms.Button();
            B_PokeDexBackgroundSave = new System.Windows.Forms.Button();
            PB_PokeDexBackground = new System.Windows.Forms.PictureBox();
            PB_PokeDexForeground = new System.Windows.Forms.PictureBox();
            B_PokeDexSkinSave = new System.Windows.Forms.Button();
            B_PokeDexForegroundLoad = new System.Windows.Forms.Button();
            B_PokeDexSkinLoad = new System.Windows.Forms.Button();
            B_PokeDexForegroundSave = new System.Windows.Forms.Button();
            Tab_Musical = new System.Windows.Forms.TabPage();
            B_MusicalExport = new System.Windows.Forms.Button();
            B_MusicalImport = new System.Windows.Forms.Button();
            Tab_BattleVideo = new System.Windows.Forms.TabPage();
            B_BattleVideoExportDecrypted = new System.Windows.Forms.Button();
            B_BattleVideoExport = new System.Windows.Forms.Button();
            B_BattleVideoImport = new System.Windows.Forms.Button();
            LB_BattleVideo = new System.Windows.Forms.ListBox();
            Tab_Pokestar = new System.Windows.Forms.TabPage();
            B_PokestarExport = new System.Windows.Forms.Button();
            B_PokestarImport = new System.Windows.Forms.Button();
            LB_Pokestar = new System.Windows.Forms.ListBox();
            Tab_PWT = new System.Windows.Forms.TabPage();
            B_PWTExport = new System.Windows.Forms.Button();
            B_PWTImport = new System.Windows.Forms.Button();
            LB_PWT = new System.Windows.Forms.ListBox();
            Tab_MemoryLink = new System.Windows.Forms.TabPage();
            B_Memory2Export = new System.Windows.Forms.Button();
            B_Memory2Import = new System.Windows.Forms.Button();
            B_Memory1Export = new System.Windows.Forms.Button();
            B_Memory1Import = new System.Windows.Forms.Button();
            Tab_BattleTest = new System.Windows.Forms.TabPage();
            B_BattleTestExport = new System.Windows.Forms.Button();
            B_BattleTestImport = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)PB_CGearBackground).BeginInit();
            TC_Tabs.SuspendLayout();
            Tab_CGear.SuspendLayout();
            Tab_PokeDex.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PB_PokeDexBackground).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PB_PokeDexForeground).BeginInit();
            Tab_Musical.SuspendLayout();
            Tab_BattleVideo.SuspendLayout();
            Tab_Pokestar.SuspendLayout();
            Tab_PWT.SuspendLayout();
            Tab_MemoryLink.SuspendLayout();
            Tab_BattleTest.SuspendLayout();
            SuspendLayout();
            // 
            // PB_CGearBackground
            // 
            PB_CGearBackground.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            PB_CGearBackground.Location = new System.Drawing.Point(7, 6);
            PB_CGearBackground.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PB_CGearBackground.Name = "PB_CGearBackground";
            PB_CGearBackground.Size = new System.Drawing.Size(258, 194);
            PB_CGearBackground.TabIndex = 0;
            PB_CGearBackground.TabStop = false;
            // 
            // B_Cancel
            // 
            B_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Cancel.Location = new System.Drawing.Point(407, 346);
            B_Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Cancel.Name = "B_Cancel";
            B_Cancel.Size = new System.Drawing.Size(120, 27);
            B_Cancel.TabIndex = 1;
            B_Cancel.Text = "Cancel";
            B_Cancel.UseVisualStyleBackColor = true;
            B_Cancel.Click += B_Cancel_Click;
            // 
            // B_Save
            // 
            B_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            B_Save.Location = new System.Drawing.Point(535, 346);
            B_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_Save.Name = "B_Save";
            B_Save.Size = new System.Drawing.Size(120, 27);
            B_Save.TabIndex = 2;
            B_Save.Text = "Save";
            B_Save.UseVisualStyleBackColor = true;
            B_Save.Click += B_Save_Click;
            // 
            // B_ImportPNG
            // 
            B_ImportPNG.Location = new System.Drawing.Point(273, 6);
            B_ImportPNG.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ImportPNG.Name = "B_ImportPNG";
            B_ImportPNG.Size = new System.Drawing.Size(120, 32);
            B_ImportPNG.TabIndex = 3;
            B_ImportPNG.Text = "Load Image";
            B_ImportPNG.UseVisualStyleBackColor = true;
            B_ImportPNG.Click += B_ImportPNGCGear_Click;
            // 
            // B_ExportPNG
            // 
            B_ExportPNG.Location = new System.Drawing.Point(273, 44);
            B_ExportPNG.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ExportPNG.Name = "B_ExportPNG";
            B_ExportPNG.Size = new System.Drawing.Size(120, 32);
            B_ExportPNG.TabIndex = 4;
            B_ExportPNG.Text = "Save Image";
            B_ExportPNG.UseVisualStyleBackColor = true;
            B_ExportPNG.Click += B_ExportPNGCGear_Click;
            // 
            // B_ExportCGB
            // 
            B_ExportCGB.Location = new System.Drawing.Point(273, 168);
            B_ExportCGB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ExportCGB.Name = "B_ExportCGB";
            B_ExportCGB.Size = new System.Drawing.Size(120, 32);
            B_ExportCGB.TabIndex = 6;
            B_ExportCGB.Text = "Export";
            B_ExportCGB.UseVisualStyleBackColor = true;
            B_ExportCGB.Click += B_ExportCGB_Click;
            // 
            // B_ImportCGB
            // 
            B_ImportCGB.Location = new System.Drawing.Point(273, 130);
            B_ImportCGB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_ImportCGB.Name = "B_ImportCGB";
            B_ImportCGB.Size = new System.Drawing.Size(120, 32);
            B_ImportCGB.TabIndex = 5;
            B_ImportCGB.Text = "Import";
            B_ImportCGB.UseVisualStyleBackColor = true;
            B_ImportCGB.Click += B_ImportCGB_Click;
            // 
            // TC_Tabs
            // 
            TC_Tabs.Alignment = System.Windows.Forms.TabAlignment.Left;
            TC_Tabs.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TC_Tabs.Controls.Add(Tab_CGear);
            TC_Tabs.Controls.Add(Tab_PokeDex);
            TC_Tabs.Controls.Add(Tab_BattleTest);
            TC_Tabs.Controls.Add(Tab_Musical);
            TC_Tabs.Controls.Add(Tab_BattleVideo);
            TC_Tabs.Controls.Add(Tab_Pokestar);
            TC_Tabs.Controls.Add(Tab_PWT);
            TC_Tabs.Controls.Add(Tab_MemoryLink);
            TC_Tabs.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            TC_Tabs.ItemSize = new System.Drawing.Size(40, 128);
            TC_Tabs.Location = new System.Drawing.Point(0, 0);
            TC_Tabs.Margin = new System.Windows.Forms.Padding(0);
            TC_Tabs.Multiline = true;
            TC_Tabs.Name = "TC_Tabs";
            TC_Tabs.Padding = new System.Drawing.Point(0, 0);
            TC_Tabs.SelectedIndex = 0;
            TC_Tabs.Size = new System.Drawing.Size(663, 339);
            TC_Tabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            TC_Tabs.TabIndex = 7;
            // 
            // Tab_CGear
            // 
            Tab_CGear.Controls.Add(PB_CGearBackground);
            Tab_CGear.Controls.Add(B_ExportCGB);
            Tab_CGear.Controls.Add(B_ImportPNG);
            Tab_CGear.Controls.Add(B_ImportCGB);
            Tab_CGear.Controls.Add(B_ExportPNG);
            Tab_CGear.Location = new System.Drawing.Point(132, 4);
            Tab_CGear.Name = "Tab_CGear";
            Tab_CGear.Padding = new System.Windows.Forms.Padding(3);
            Tab_CGear.Size = new System.Drawing.Size(527, 331);
            Tab_CGear.TabIndex = 0;
            Tab_CGear.Text = "C-Gear Skin";
            Tab_CGear.UseVisualStyleBackColor = true;
            // 
            // Tab_PokeDex
            // 
            Tab_PokeDex.Controls.Add(B_PokeDexBackgroundLoad);
            Tab_PokeDex.Controls.Add(B_PokeDexBackgroundSave);
            Tab_PokeDex.Controls.Add(PB_PokeDexBackground);
            Tab_PokeDex.Controls.Add(PB_PokeDexForeground);
            Tab_PokeDex.Controls.Add(B_PokeDexSkinSave);
            Tab_PokeDex.Controls.Add(B_PokeDexForegroundLoad);
            Tab_PokeDex.Controls.Add(B_PokeDexSkinLoad);
            Tab_PokeDex.Controls.Add(B_PokeDexForegroundSave);
            Tab_PokeDex.Location = new System.Drawing.Point(132, 4);
            Tab_PokeDex.Name = "Tab_PokeDex";
            Tab_PokeDex.Padding = new System.Windows.Forms.Padding(3);
            Tab_PokeDex.Size = new System.Drawing.Size(527, 331);
            Tab_PokeDex.TabIndex = 1;
            Tab_PokeDex.Text = "PokéDex Skin";
            Tab_PokeDex.UseVisualStyleBackColor = true;
            // 
            // B_PokeDexBackgroundLoad
            // 
            B_PokeDexBackgroundLoad.Location = new System.Drawing.Point(403, 200);
            B_PokeDexBackgroundLoad.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_PokeDexBackgroundLoad.Name = "B_PokeDexBackgroundLoad";
            B_PokeDexBackgroundLoad.Size = new System.Drawing.Size(120, 32);
            B_PokeDexBackgroundLoad.TabIndex = 13;
            B_PokeDexBackgroundLoad.Text = "Load Image";
            B_PokeDexBackgroundLoad.UseVisualStyleBackColor = true;
            // 
            // B_PokeDexBackgroundSave
            // 
            B_PokeDexBackgroundSave.Location = new System.Drawing.Point(403, 238);
            B_PokeDexBackgroundSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_PokeDexBackgroundSave.Name = "B_PokeDexBackgroundSave";
            B_PokeDexBackgroundSave.Size = new System.Drawing.Size(120, 32);
            B_PokeDexBackgroundSave.TabIndex = 14;
            B_PokeDexBackgroundSave.Text = "Save Image";
            B_PokeDexBackgroundSave.UseVisualStyleBackColor = true;
            // 
            // PB_PokeDexBackground
            // 
            PB_PokeDexBackground.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            PB_PokeDexBackground.Location = new System.Drawing.Point(265, 0);
            PB_PokeDexBackground.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PB_PokeDexBackground.Name = "PB_PokeDexBackground";
            PB_PokeDexBackground.Size = new System.Drawing.Size(258, 194);
            PB_PokeDexBackground.TabIndex = 12;
            PB_PokeDexBackground.TabStop = false;
            // 
            // PB_PokeDexForeground
            // 
            PB_PokeDexForeground.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            PB_PokeDexForeground.Location = new System.Drawing.Point(0, 0);
            PB_PokeDexForeground.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            PB_PokeDexForeground.Name = "PB_PokeDexForeground";
            PB_PokeDexForeground.Size = new System.Drawing.Size(258, 194);
            PB_PokeDexForeground.TabIndex = 7;
            PB_PokeDexForeground.TabStop = false;
            // 
            // B_PokeDexSkinSave
            // 
            B_PokeDexSkinSave.Location = new System.Drawing.Point(265, 293);
            B_PokeDexSkinSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_PokeDexSkinSave.Name = "B_PokeDexSkinSave";
            B_PokeDexSkinSave.Size = new System.Drawing.Size(120, 32);
            B_PokeDexSkinSave.TabIndex = 11;
            B_PokeDexSkinSave.Text = "Export";
            B_PokeDexSkinSave.UseVisualStyleBackColor = true;
            B_PokeDexSkinSave.Click += B_PokeDexSkinSave_Click;
            // 
            // B_PokeDexForegroundLoad
            // 
            B_PokeDexForegroundLoad.Location = new System.Drawing.Point(7, 200);
            B_PokeDexForegroundLoad.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_PokeDexForegroundLoad.Name = "B_PokeDexForegroundLoad";
            B_PokeDexForegroundLoad.Size = new System.Drawing.Size(120, 32);
            B_PokeDexForegroundLoad.TabIndex = 8;
            B_PokeDexForegroundLoad.Text = "Load Image";
            B_PokeDexForegroundLoad.UseVisualStyleBackColor = true;
            // 
            // B_PokeDexSkinLoad
            // 
            B_PokeDexSkinLoad.Location = new System.Drawing.Point(138, 293);
            B_PokeDexSkinLoad.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_PokeDexSkinLoad.Name = "B_PokeDexSkinLoad";
            B_PokeDexSkinLoad.Size = new System.Drawing.Size(120, 32);
            B_PokeDexSkinLoad.TabIndex = 10;
            B_PokeDexSkinLoad.Text = "Import";
            B_PokeDexSkinLoad.UseVisualStyleBackColor = true;
            B_PokeDexSkinLoad.Click += B_PokeDexSkinLoad_Click;
            // 
            // B_PokeDexForegroundSave
            // 
            B_PokeDexForegroundSave.Location = new System.Drawing.Point(7, 238);
            B_PokeDexForegroundSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            B_PokeDexForegroundSave.Name = "B_PokeDexForegroundSave";
            B_PokeDexForegroundSave.Size = new System.Drawing.Size(120, 32);
            B_PokeDexForegroundSave.TabIndex = 9;
            B_PokeDexForegroundSave.Text = "Save Image";
            B_PokeDexForegroundSave.UseVisualStyleBackColor = true;
            // 
            // Tab_Musical
            // 
            Tab_Musical.Controls.Add(B_MusicalExport);
            Tab_Musical.Controls.Add(B_MusicalImport);
            Tab_Musical.Location = new System.Drawing.Point(132, 4);
            Tab_Musical.Name = "Tab_Musical";
            Tab_Musical.Size = new System.Drawing.Size(527, 331);
            Tab_Musical.TabIndex = 2;
            Tab_Musical.Text = "Musical";
            Tab_Musical.UseVisualStyleBackColor = true;
            // 
            // B_MusicalExport
            // 
            B_MusicalExport.Location = new System.Drawing.Point(368, 3);
            B_MusicalExport.Name = "B_MusicalExport";
            B_MusicalExport.Size = new System.Drawing.Size(120, 48);
            B_MusicalExport.TabIndex = 11;
            B_MusicalExport.Text = "Export";
            B_MusicalExport.UseVisualStyleBackColor = true;
            B_MusicalExport.Click += B_MusicalExport_Click;
            // 
            // B_MusicalImport
            // 
            B_MusicalImport.Location = new System.Drawing.Point(243, 3);
            B_MusicalImport.Name = "B_MusicalImport";
            B_MusicalImport.Size = new System.Drawing.Size(120, 48);
            B_MusicalImport.TabIndex = 10;
            B_MusicalImport.Text = "Import";
            B_MusicalImport.UseVisualStyleBackColor = true;
            B_MusicalImport.Click += B_MusicalImport_Click;
            // 
            // Tab_BattleVideo
            // 
            Tab_BattleVideo.Controls.Add(B_BattleVideoExportDecrypted);
            Tab_BattleVideo.Controls.Add(B_BattleVideoExport);
            Tab_BattleVideo.Controls.Add(B_BattleVideoImport);
            Tab_BattleVideo.Controls.Add(LB_BattleVideo);
            Tab_BattleVideo.Location = new System.Drawing.Point(132, 4);
            Tab_BattleVideo.Name = "Tab_BattleVideo";
            Tab_BattleVideo.Size = new System.Drawing.Size(527, 331);
            Tab_BattleVideo.TabIndex = 3;
            Tab_BattleVideo.Text = "Battle Videos";
            Tab_BattleVideo.UseVisualStyleBackColor = true;
            // 
            // B_BattleVideoExportDecrypted
            // 
            B_BattleVideoExportDecrypted.Location = new System.Drawing.Point(368, 111);
            B_BattleVideoExportDecrypted.Name = "B_BattleVideoExportDecrypted";
            B_BattleVideoExportDecrypted.Size = new System.Drawing.Size(120, 48);
            B_BattleVideoExportDecrypted.TabIndex = 10;
            B_BattleVideoExportDecrypted.Text = "Export Decrypted";
            B_BattleVideoExportDecrypted.UseVisualStyleBackColor = true;
            B_BattleVideoExportDecrypted.Click += B_BattleVideoExportDecrypted_Click;
            // 
            // B_BattleVideoExport
            // 
            B_BattleVideoExport.Location = new System.Drawing.Point(368, 3);
            B_BattleVideoExport.Name = "B_BattleVideoExport";
            B_BattleVideoExport.Size = new System.Drawing.Size(120, 48);
            B_BattleVideoExport.TabIndex = 8;
            B_BattleVideoExport.Text = "Export";
            B_BattleVideoExport.UseVisualStyleBackColor = true;
            B_BattleVideoExport.Click += B_BattleVideoExport_Click;
            // 
            // B_BattleVideoImport
            // 
            B_BattleVideoImport.Location = new System.Drawing.Point(243, 3);
            B_BattleVideoImport.Name = "B_BattleVideoImport";
            B_BattleVideoImport.Size = new System.Drawing.Size(120, 48);
            B_BattleVideoImport.TabIndex = 7;
            B_BattleVideoImport.Text = "Import";
            B_BattleVideoImport.UseVisualStyleBackColor = true;
            B_BattleVideoImport.Click += B_BattleVideoImport_Click;
            // 
            // LB_BattleVideo
            // 
            LB_BattleVideo.FormattingEnabled = true;
            LB_BattleVideo.ItemHeight = 15;
            LB_BattleVideo.Location = new System.Drawing.Point(3, 3);
            LB_BattleVideo.Name = "LB_BattleVideo";
            LB_BattleVideo.Size = new System.Drawing.Size(234, 64);
            LB_BattleVideo.TabIndex = 6;
            LB_BattleVideo.SelectedIndexChanged += LB_BattleVideo_SelectedIndexChanged;
            // 
            // Tab_Pokestar
            // 
            Tab_Pokestar.Controls.Add(B_PokestarExport);
            Tab_Pokestar.Controls.Add(B_PokestarImport);
            Tab_Pokestar.Controls.Add(LB_Pokestar);
            Tab_Pokestar.Location = new System.Drawing.Point(132, 4);
            Tab_Pokestar.Name = "Tab_Pokestar";
            Tab_Pokestar.Size = new System.Drawing.Size(527, 331);
            Tab_Pokestar.TabIndex = 5;
            Tab_Pokestar.Text = "Pokéstar Studios";
            Tab_Pokestar.UseVisualStyleBackColor = true;
            // 
            // B_PokestarExport
            // 
            B_PokestarExport.Location = new System.Drawing.Point(368, 3);
            B_PokestarExport.Name = "B_PokestarExport";
            B_PokestarExport.Size = new System.Drawing.Size(120, 48);
            B_PokestarExport.TabIndex = 5;
            B_PokestarExport.Text = "Export";
            B_PokestarExport.UseVisualStyleBackColor = true;
            B_PokestarExport.Click += B_PokestarExport_Click;
            // 
            // B_PokestarImport
            // 
            B_PokestarImport.Location = new System.Drawing.Point(243, 3);
            B_PokestarImport.Name = "B_PokestarImport";
            B_PokestarImport.Size = new System.Drawing.Size(120, 48);
            B_PokestarImport.TabIndex = 4;
            B_PokestarImport.Text = "Import";
            B_PokestarImport.UseVisualStyleBackColor = true;
            B_PokestarImport.Click += B_PokestarImport_Click;
            // 
            // LB_Pokestar
            // 
            LB_Pokestar.FormattingEnabled = true;
            LB_Pokestar.ItemHeight = 15;
            LB_Pokestar.Location = new System.Drawing.Point(3, 3);
            LB_Pokestar.Name = "LB_Pokestar";
            LB_Pokestar.Size = new System.Drawing.Size(234, 124);
            LB_Pokestar.TabIndex = 3;
            LB_Pokestar.SelectedIndexChanged += LB_Pokestar_SelectedIndexChanged;
            // 
            // Tab_PWT
            // 
            Tab_PWT.Controls.Add(B_PWTExport);
            Tab_PWT.Controls.Add(B_PWTImport);
            Tab_PWT.Controls.Add(LB_PWT);
            Tab_PWT.Location = new System.Drawing.Point(132, 4);
            Tab_PWT.Name = "Tab_PWT";
            Tab_PWT.Size = new System.Drawing.Size(527, 331);
            Tab_PWT.TabIndex = 4;
            Tab_PWT.Text = "PWT";
            Tab_PWT.UseVisualStyleBackColor = true;
            // 
            // B_PWTExport
            // 
            B_PWTExport.Location = new System.Drawing.Point(368, 3);
            B_PWTExport.Name = "B_PWTExport";
            B_PWTExport.Size = new System.Drawing.Size(120, 48);
            B_PWTExport.TabIndex = 2;
            B_PWTExport.Text = "Export";
            B_PWTExport.UseVisualStyleBackColor = true;
            B_PWTExport.Click += B_PWTExport_Click;
            // 
            // B_PWTImport
            // 
            B_PWTImport.Location = new System.Drawing.Point(243, 3);
            B_PWTImport.Name = "B_PWTImport";
            B_PWTImport.Size = new System.Drawing.Size(120, 48);
            B_PWTImport.TabIndex = 1;
            B_PWTImport.Text = "Import";
            B_PWTImport.UseVisualStyleBackColor = true;
            B_PWTImport.Click += B_PWTImport_Click;
            // 
            // LB_PWT
            // 
            LB_PWT.FormattingEnabled = true;
            LB_PWT.ItemHeight = 15;
            LB_PWT.Location = new System.Drawing.Point(3, 3);
            LB_PWT.Name = "LB_PWT";
            LB_PWT.Size = new System.Drawing.Size(234, 49);
            LB_PWT.TabIndex = 0;
            LB_PWT.SelectedIndexChanged += LB_PWTIndex_SelectedIndexChanged;
            // 
            // Tab_MemoryLink
            // 
            Tab_MemoryLink.Controls.Add(B_Memory2Export);
            Tab_MemoryLink.Controls.Add(B_Memory2Import);
            Tab_MemoryLink.Controls.Add(B_Memory1Export);
            Tab_MemoryLink.Controls.Add(B_Memory1Import);
            Tab_MemoryLink.Location = new System.Drawing.Point(132, 4);
            Tab_MemoryLink.Name = "Tab_MemoryLink";
            Tab_MemoryLink.Padding = new System.Windows.Forms.Padding(3);
            Tab_MemoryLink.Size = new System.Drawing.Size(527, 331);
            Tab_MemoryLink.TabIndex = 6;
            Tab_MemoryLink.Text = "Memory Link";
            Tab_MemoryLink.UseVisualStyleBackColor = true;
            // 
            // B_Memory2Export
            // 
            B_Memory2Export.Location = new System.Drawing.Point(368, 57);
            B_Memory2Export.Name = "B_Memory2Export";
            B_Memory2Export.Size = new System.Drawing.Size(120, 48);
            B_Memory2Export.TabIndex = 6;
            B_Memory2Export.Text = "Export Slot 2";
            B_Memory2Export.UseVisualStyleBackColor = true;
            B_Memory2Export.Click += B_Memory2Export_Click;
            // 
            // B_Memory2Import
            // 
            B_Memory2Import.Location = new System.Drawing.Point(243, 57);
            B_Memory2Import.Name = "B_Memory2Import";
            B_Memory2Import.Size = new System.Drawing.Size(120, 48);
            B_Memory2Import.TabIndex = 5;
            B_Memory2Import.Text = "Import Slot 2";
            B_Memory2Import.UseVisualStyleBackColor = true;
            B_Memory2Import.Click += B_Memory2Import_Click;
            // 
            // B_Memory1Export
            // 
            B_Memory1Export.Location = new System.Drawing.Point(368, 3);
            B_Memory1Export.Name = "B_Memory1Export";
            B_Memory1Export.Size = new System.Drawing.Size(120, 48);
            B_Memory1Export.TabIndex = 4;
            B_Memory1Export.Text = "Export Slot 1";
            B_Memory1Export.UseVisualStyleBackColor = true;
            B_Memory1Export.Click += B_Memory1Export_Click;
            // 
            // B_Memory1Import
            // 
            B_Memory1Import.Location = new System.Drawing.Point(243, 3);
            B_Memory1Import.Name = "B_Memory1Import";
            B_Memory1Import.Size = new System.Drawing.Size(120, 48);
            B_Memory1Import.TabIndex = 3;
            B_Memory1Import.Text = "Import Slot 1";
            B_Memory1Import.UseVisualStyleBackColor = true;
            B_Memory1Import.Click += B_Memory1Import_Click;
            // 
            // Tab_BattleTest
            // 
            Tab_BattleTest.Controls.Add(B_BattleTestExport);
            Tab_BattleTest.Controls.Add(B_BattleTestImport);
            Tab_BattleTest.Location = new System.Drawing.Point(132, 4);
            Tab_BattleTest.Name = "Tab_BattleTest";
            Tab_BattleTest.Padding = new System.Windows.Forms.Padding(3);
            Tab_BattleTest.Size = new System.Drawing.Size(527, 331);
            Tab_BattleTest.TabIndex = 7;
            Tab_BattleTest.Text = "Battle Test";
            Tab_BattleTest.UseVisualStyleBackColor = true;
            // 
            // B_BattleTestExport
            // 
            B_BattleTestExport.Location = new System.Drawing.Point(368, 3);
            B_BattleTestExport.Name = "B_BattleTestExport";
            B_BattleTestExport.Size = new System.Drawing.Size(120, 48);
            B_BattleTestExport.TabIndex = 13;
            B_BattleTestExport.Text = "Export";
            B_BattleTestExport.UseVisualStyleBackColor = true;
            B_BattleTestExport.Click += B_BattleTestExport_Click;
            // 
            // B_BattleTestImport
            // 
            B_BattleTestImport.Location = new System.Drawing.Point(243, 3);
            B_BattleTestImport.Name = "B_BattleTestImport";
            B_BattleTestImport.Size = new System.Drawing.Size(120, 48);
            B_BattleTestImport.TabIndex = 12;
            B_BattleTestImport.Text = "Import";
            B_BattleTestImport.UseVisualStyleBackColor = true;
            B_BattleTestImport.Click += B_BattleTestImport_Click;
            // 
            // SAV_DLC5
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(664, 381);
            Controls.Add(TC_Tabs);
            Controls.Add(B_Save);
            Controls.Add(B_Cancel);
            Icon = Properties.Resources.Icon;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(1080, 720);
            MinimumSize = new System.Drawing.Size(680, 420);
            Name = "SAV_DLC5";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Generation 5 DLC I/O";
            ((System.ComponentModel.ISupportInitialize)PB_CGearBackground).EndInit();
            TC_Tabs.ResumeLayout(false);
            Tab_CGear.ResumeLayout(false);
            Tab_PokeDex.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PB_PokeDexBackground).EndInit();
            ((System.ComponentModel.ISupportInitialize)PB_PokeDexForeground).EndInit();
            Tab_Musical.ResumeLayout(false);
            Tab_BattleVideo.ResumeLayout(false);
            Tab_Pokestar.ResumeLayout(false);
            Tab_PWT.ResumeLayout(false);
            Tab_MemoryLink.ResumeLayout(false);
            Tab_BattleTest.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.PictureBox PB_CGearBackground;
        private System.Windows.Forms.Button B_Cancel;
        private System.Windows.Forms.Button B_Save;
        private System.Windows.Forms.Button B_ImportPNG;
        private System.Windows.Forms.Button B_ExportPNG;
        private System.Windows.Forms.Button B_ExportCGB;
        private System.Windows.Forms.Button B_ImportCGB;
        private VerticalTabControl TC_Tabs;
        private System.Windows.Forms.TabPage Tab_CGear;
        private System.Windows.Forms.TabPage Tab_PokeDex;
        private System.Windows.Forms.TabPage Tab_Musical;
        private System.Windows.Forms.TabPage Tab_BattleVideo;
        private System.Windows.Forms.TabPage Tab_PWT;
        private System.Windows.Forms.TabPage Tab_Pokestar;
        private System.Windows.Forms.TabPage Tab_MemoryLink;
        private System.Windows.Forms.Button B_PWTExport;
        private System.Windows.Forms.Button B_PWTImport;
        private System.Windows.Forms.ListBox LB_PWT;
        private System.Windows.Forms.Button B_PokestarExport;
        private System.Windows.Forms.Button B_PokestarImport;
        private System.Windows.Forms.ListBox LB_Pokestar;
        private System.Windows.Forms.Button B_BattleVideoExport;
        private System.Windows.Forms.Button B_BattleVideoImport;
        private System.Windows.Forms.ListBox LB_BattleVideo;
        private System.Windows.Forms.Button B_MusicalExport;
        private System.Windows.Forms.Button B_MusicalImport;
        private System.Windows.Forms.Button B_Memory2Export;
        private System.Windows.Forms.Button B_Memory2Import;
        private System.Windows.Forms.Button B_Memory1Export;
        private System.Windows.Forms.Button B_Memory1Import;
        private System.Windows.Forms.Button B_PokeDexBackgroundLoad;
        private System.Windows.Forms.Button B_PokeDexBackgroundSave;
        private System.Windows.Forms.PictureBox PB_PokeDexBackground;
        private System.Windows.Forms.PictureBox PB_PokeDexForeground;
        private System.Windows.Forms.Button B_PokeDexSkinSave;
        private System.Windows.Forms.Button B_PokeDexForegroundLoad;
        private System.Windows.Forms.Button B_PokeDexSkinLoad;
        private System.Windows.Forms.Button B_PokeDexForegroundSave;
        private System.Windows.Forms.Button B_BattleVideoExportDecrypted;
        private System.Windows.Forms.TabPage Tab_BattleTest;
        private System.Windows.Forms.Button B_BattleTestExport;
        private System.Windows.Forms.Button B_BattleTestImport;
    }
}
