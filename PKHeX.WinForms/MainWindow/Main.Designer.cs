namespace PKHeX.WinForms
{
    partial class Main
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

        public void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            Menu_File = new System.Windows.Forms.ToolStripMenuItem();
            Menu_Open = new System.Windows.Forms.ToolStripMenuItem();
            Menu_Save = new System.Windows.Forms.ToolStripMenuItem();
            Menu_ExportSAV = new System.Windows.Forms.ToolStripMenuItem();
            Menu_Exit = new System.Windows.Forms.ToolStripMenuItem();
            Menu_Tools = new System.Windows.Forms.ToolStripMenuItem();
            Menu_Showdown = new System.Windows.Forms.ToolStripMenuItem();
            Menu_ShowdownImportPKM = new System.Windows.Forms.ToolStripMenuItem();
            Menu_ShowdownExportPKM = new System.Windows.Forms.ToolStripMenuItem();
            Menu_ShowdownExportParty = new System.Windows.Forms.ToolStripMenuItem();
            Menu_ShowdownExportCurrentBox = new System.Windows.Forms.ToolStripMenuItem();
            Menu_Data = new System.Windows.Forms.ToolStripMenuItem();
            Menu_LoadBoxes = new System.Windows.Forms.ToolStripMenuItem();
            Menu_DumpBoxes = new System.Windows.Forms.ToolStripMenuItem();
            Menu_DumpBox = new System.Windows.Forms.ToolStripMenuItem();
            Menu_Report = new System.Windows.Forms.ToolStripMenuItem();
            Menu_Database = new System.Windows.Forms.ToolStripMenuItem();
            Menu_MGDatabase = new System.Windows.Forms.ToolStripMenuItem();
            Menu_EncDatabase = new System.Windows.Forms.ToolStripMenuItem();
            Menu_BatchEditor = new System.Windows.Forms.ToolStripMenuItem();
            Menu_Folder = new System.Windows.Forms.ToolStripMenuItem();
            Menu_Options = new System.Windows.Forms.ToolStripMenuItem();
            Menu_Language = new System.Windows.Forms.ToolStripMenuItem();
            CB_MainLanguage = new System.Windows.Forms.ToolStripComboBox();
            Menu_Undo = new System.Windows.Forms.ToolStripMenuItem();
            Menu_Redo = new System.Windows.Forms.ToolStripMenuItem();
            Menu_Settings = new System.Windows.Forms.ToolStripMenuItem();
            Menu_About = new System.Windows.Forms.ToolStripMenuItem();
            L_UpdateAvailable = new System.Windows.Forms.LinkLabel();
            toolTip = new System.Windows.Forms.ToolTip(components);
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            dragout = new Controls.SelectablePictureBox();
            PB_Legal = new System.Windows.Forms.PictureBox();
            PKME_Tabs = new Controls.PKMEditor();
            C_SAV = new Controls.SAVEditor();
            splitContainer2 = new System.Windows.Forms.SplitContainer();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dragout).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PB_Legal).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.AccessibleDescription = "Main Window Menustrip";
            menuStrip1.AccessibleName = "Main Window Menustrip";
            menuStrip1.BackColor = System.Drawing.Color.Transparent;
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { Menu_File, Menu_Tools, Menu_Options });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            menuStrip1.Size = new System.Drawing.Size(856, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // Menu_File
            // 
            Menu_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { Menu_Open, Menu_Save, Menu_ExportSAV, Menu_Exit });
            Menu_File.Name = "Menu_File";
            Menu_File.Size = new System.Drawing.Size(37, 20);
            Menu_File.Text = "File";
            // 
            // Menu_Open
            // 
            Menu_Open.Image = Properties.Resources.open;
            Menu_Open.Name = "Menu_Open";
            Menu_Open.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O;
            Menu_Open.ShowShortcutKeys = false;
            Menu_Open.Size = new System.Drawing.Size(133, 22);
            Menu_Open.Text = "&Open...";
            Menu_Open.Click += MainMenuOpen;
            // 
            // Menu_Save
            // 
            Menu_Save.Image = Properties.Resources.savePKM;
            Menu_Save.Name = "Menu_Save";
            Menu_Save.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S;
            Menu_Save.ShowShortcutKeys = false;
            Menu_Save.Size = new System.Drawing.Size(133, 22);
            Menu_Save.Text = "&Save PKM...";
            Menu_Save.Click += MainMenuSave;
            // 
            // Menu_ExportSAV
            // 
            Menu_ExportSAV.Image = Properties.Resources.saveSAV;
            Menu_ExportSAV.Name = "Menu_ExportSAV";
            Menu_ExportSAV.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E;
            Menu_ExportSAV.ShowShortcutKeys = false;
            Menu_ExportSAV.Size = new System.Drawing.Size(133, 22);
            Menu_ExportSAV.Text = "&Export SAV...";
            Menu_ExportSAV.Click += ClickExportSAV;
            // 
            // Menu_Exit
            // 
            Menu_Exit.Image = Properties.Resources.exit;
            Menu_Exit.Name = "Menu_Exit";
            Menu_Exit.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q;
            Menu_Exit.ShowShortcutKeys = false;
            Menu_Exit.Size = new System.Drawing.Size(133, 22);
            Menu_Exit.Text = "&Quit";
            Menu_Exit.Click += MainMenuExit;
            // 
            // Menu_Tools
            // 
            Menu_Tools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { Menu_Showdown, Menu_Data, Menu_Folder });
            Menu_Tools.Name = "Menu_Tools";
            Menu_Tools.Size = new System.Drawing.Size(46, 20);
            Menu_Tools.Text = "Tools";
            // 
            // Menu_Showdown
            // 
            Menu_Showdown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { Menu_ShowdownImportPKM, Menu_ShowdownExportPKM, Menu_ShowdownExportParty, Menu_ShowdownExportCurrentBox });
            Menu_Showdown.Image = Properties.Resources.showdown;
            Menu_Showdown.Name = "Menu_Showdown";
            Menu_Showdown.Size = new System.Drawing.Size(133, 22);
            Menu_Showdown.Text = "Showdown";
            // 
            // Menu_ShowdownImportPKM
            // 
            Menu_ShowdownImportPKM.Image = Properties.Resources.import;
            Menu_ShowdownImportPKM.Name = "Menu_ShowdownImportPKM";
            Menu_ShowdownImportPKM.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T;
            Menu_ShowdownImportPKM.ShowShortcutKeys = false;
            Menu_ShowdownImportPKM.Size = new System.Drawing.Size(243, 22);
            Menu_ShowdownImportPKM.Text = "Import Set from Clipboard";
            Menu_ShowdownImportPKM.Click += ClickShowdownImportPKM;
            // 
            // Menu_ShowdownExportPKM
            // 
            Menu_ShowdownExportPKM.Image = Properties.Resources.export;
            Menu_ShowdownExportPKM.Name = "Menu_ShowdownExportPKM";
            Menu_ShowdownExportPKM.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.T;
            Menu_ShowdownExportPKM.ShowShortcutKeys = false;
            Menu_ShowdownExportPKM.Size = new System.Drawing.Size(243, 22);
            Menu_ShowdownExportPKM.Text = "Export Set to Clipboard";
            Menu_ShowdownExportPKM.Click += ClickShowdownExportPKM;
            // 
            // Menu_ShowdownExportParty
            // 
            Menu_ShowdownExportParty.Image = Properties.Resources.export;
            Menu_ShowdownExportParty.Name = "Menu_ShowdownExportParty";
            Menu_ShowdownExportParty.Size = new System.Drawing.Size(243, 22);
            Menu_ShowdownExportParty.Text = "Export Party to Clipboard";
            Menu_ShowdownExportParty.Click += ClickShowdownExportParty;
            // 
            // Menu_ShowdownExportCurrentBox
            // 
            Menu_ShowdownExportCurrentBox.Image = Properties.Resources.export;
            Menu_ShowdownExportCurrentBox.Name = "Menu_ShowdownExportCurrentBox";
            Menu_ShowdownExportCurrentBox.Size = new System.Drawing.Size(243, 22);
            Menu_ShowdownExportCurrentBox.Text = "Export Current Box to Clipboard";
            Menu_ShowdownExportCurrentBox.Click += ClickShowdownExportCurrentBox;
            // 
            // Menu_Data
            // 
            Menu_Data.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { Menu_LoadBoxes, Menu_DumpBoxes, Menu_DumpBox, Menu_Report, Menu_Database, Menu_MGDatabase, Menu_EncDatabase, Menu_BatchEditor });
            Menu_Data.Image = Properties.Resources.data;
            Menu_Data.Name = "Menu_Data";
            Menu_Data.Size = new System.Drawing.Size(133, 22);
            Menu_Data.Text = "Data";
            // 
            // Menu_LoadBoxes
            // 
            Menu_LoadBoxes.Image = Properties.Resources.load;
            Menu_LoadBoxes.Name = "Menu_LoadBoxes";
            Menu_LoadBoxes.Size = new System.Drawing.Size(182, 22);
            Menu_LoadBoxes.Text = "Load Boxes";
            Menu_LoadBoxes.Click += MainMenuBoxLoad;
            // 
            // Menu_DumpBoxes
            // 
            Menu_DumpBoxes.Image = Properties.Resources.dump;
            Menu_DumpBoxes.Name = "Menu_DumpBoxes";
            Menu_DumpBoxes.Size = new System.Drawing.Size(182, 22);
            Menu_DumpBoxes.Text = "Dump Boxes";
            Menu_DumpBoxes.Click += MainMenuBoxDump;
            // 
            // Menu_DumpBox
            // 
            Menu_DumpBox.Image = Properties.Resources.dump;
            Menu_DumpBox.Name = "Menu_DumpBox";
            Menu_DumpBox.Size = new System.Drawing.Size(182, 22);
            Menu_DumpBox.Text = "Dump Box";
            Menu_DumpBox.Click += MainMenuBoxDumpSingle;
            // 
            // Menu_Report
            // 
            Menu_Report.Image = Properties.Resources.report;
            Menu_Report.Name = "Menu_Report";
            Menu_Report.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R;
            Menu_Report.ShowShortcutKeys = false;
            Menu_Report.Size = new System.Drawing.Size(182, 22);
            Menu_Report.Text = "Box Data &Report";
            Menu_Report.Click += MainMenuBoxReport;
            // 
            // Menu_Database
            // 
            Menu_Database.Image = Properties.Resources.database;
            Menu_Database.Name = "Menu_Database";
            Menu_Database.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D;
            Menu_Database.ShowShortcutKeys = false;
            Menu_Database.Size = new System.Drawing.Size(182, 22);
            Menu_Database.Text = "PKM &Database";
            Menu_Database.Click += MainMenuDatabase;
            // 
            // Menu_MGDatabase
            // 
            Menu_MGDatabase.Image = Properties.Resources.gift;
            Menu_MGDatabase.Name = "Menu_MGDatabase";
            Menu_MGDatabase.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G;
            Menu_MGDatabase.ShowShortcutKeys = false;
            Menu_MGDatabase.Size = new System.Drawing.Size(182, 22);
            Menu_MGDatabase.Text = "&Mystery Gift Database";
            Menu_MGDatabase.Click += MainMenuMysteryDB;
            // 
            // Menu_EncDatabase
            // 
            Menu_EncDatabase.Image = Properties.Resources.users;
            Menu_EncDatabase.Name = "Menu_EncDatabase";
            Menu_EncDatabase.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N;
            Menu_EncDatabase.ShowShortcutKeys = false;
            Menu_EncDatabase.Size = new System.Drawing.Size(182, 22);
            Menu_EncDatabase.Text = "E&ncounter Database";
            Menu_EncDatabase.Click += Menu_EncDatabase_Click;
            // 
            // Menu_BatchEditor
            // 
            Menu_BatchEditor.Image = Properties.Resources.settings;
            Menu_BatchEditor.Name = "Menu_BatchEditor";
            Menu_BatchEditor.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M;
            Menu_BatchEditor.ShowShortcutKeys = false;
            Menu_BatchEditor.Size = new System.Drawing.Size(182, 22);
            Menu_BatchEditor.Text = "Batch Editor";
            Menu_BatchEditor.Click += MainMenuBatchEditor;
            // 
            // Menu_Folder
            // 
            Menu_Folder.Image = Properties.Resources.folder;
            Menu_Folder.Name = "Menu_Folder";
            Menu_Folder.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F;
            Menu_Folder.ShowShortcutKeys = false;
            Menu_Folder.Size = new System.Drawing.Size(133, 22);
            Menu_Folder.Text = "Open Folder";
            Menu_Folder.Click += MainMenuFolder;
            // 
            // Menu_Options
            // 
            Menu_Options.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { Menu_Language, Menu_Undo, Menu_Redo, Menu_Settings, Menu_About });
            Menu_Options.Name = "Menu_Options";
            Menu_Options.Size = new System.Drawing.Size(61, 20);
            Menu_Options.Text = "Options";
            // 
            // Menu_Language
            // 
            Menu_Language.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { CB_MainLanguage });
            Menu_Language.Image = Properties.Resources.language;
            Menu_Language.Name = "Menu_Language";
            Menu_Language.Size = new System.Drawing.Size(164, 22);
            Menu_Language.Text = "Language";
            // 
            // CB_MainLanguage
            // 
            CB_MainLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            CB_MainLanguage.Name = "CB_MainLanguage";
            CB_MainLanguage.Size = new System.Drawing.Size(121, 23);
            CB_MainLanguage.SelectedIndexChanged += ChangeMainLanguage;
            // 
            // Menu_Undo
            // 
            Menu_Undo.Enabled = false;
            Menu_Undo.Image = Properties.Resources.bak;
            Menu_Undo.Name = "Menu_Undo";
            Menu_Undo.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U;
            Menu_Undo.ShowShortcutKeys = false;
            Menu_Undo.Size = new System.Drawing.Size(164, 22);
            Menu_Undo.Text = "Undo Last Change";
            Menu_Undo.Click += ClickUndo;
            // 
            // Menu_Redo
            // 
            Menu_Redo.Enabled = false;
            Menu_Redo.Image = Properties.Resources.redo;
            Menu_Redo.Name = "Menu_Redo";
            Menu_Redo.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y;
            Menu_Redo.ShowShortcutKeys = false;
            Menu_Redo.Size = new System.Drawing.Size(164, 22);
            Menu_Redo.Text = "Redo Last Change";
            Menu_Redo.Click += ClickRedo;
            // 
            // Menu_Settings
            // 
            Menu_Settings.Image = Properties.Resources.settings;
            Menu_Settings.Name = "Menu_Settings";
            Menu_Settings.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.S;
            Menu_Settings.ShowShortcutKeys = false;
            Menu_Settings.Size = new System.Drawing.Size(164, 22);
            Menu_Settings.Text = "Settings";
            Menu_Settings.Click += MainMenuSettings;
            // 
            // Menu_About
            // 
            Menu_About.Image = Properties.Resources.about;
            Menu_About.Name = "Menu_About";
            Menu_About.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P;
            Menu_About.ShowShortcutKeys = false;
            Menu_About.Size = new System.Drawing.Size(164, 22);
            Menu_About.Text = "About &PKHeX";
            Menu_About.Click += MainMenuAbout;
            // 
            // L_UpdateAvailable
            // 
            L_UpdateAvailable.AccessibleDescription = "If an update is available, link label can be clicked to open new download link.";
            L_UpdateAvailable.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            L_UpdateAvailable.Enabled = false;
            L_UpdateAvailable.Location = new System.Drawing.Point(564, 0);
            L_UpdateAvailable.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            L_UpdateAvailable.Name = "L_UpdateAvailable";
            L_UpdateAvailable.Size = new System.Drawing.Size(288, 24);
            L_UpdateAvailable.TabIndex = 102;
            L_UpdateAvailable.TabStop = true;
            L_UpdateAvailable.Text = "You are using the latest version!";
            L_UpdateAvailable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            L_UpdateAvailable.Visible = false;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Margin = new System.Windows.Forms.Padding(8);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.AccessibleDescription = "Main Window Split Pane";
            splitContainer1.Panel1.AccessibleName = "Main Window Split Pane";
            splitContainer1.Panel1.Controls.Add(dragout);
            splitContainer1.Panel1.Controls.Add(PB_Legal);
            splitContainer1.Panel1.Controls.Add(PKME_Tabs);
            splitContainer1.Panel1MinSize = 400;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(C_SAV);
            splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(0, 0, 1, 3);
            splitContainer1.Size = new System.Drawing.Size(856, 359);
            splitContainer1.SplitterDistance = 400;
            splitContainer1.SplitterWidth = 1;
            splitContainer1.TabIndex = 105;
            // 
            // dragout
            // 
            dragout.BackColor = System.Drawing.Color.Transparent;
            dragout.Location = new System.Drawing.Point(24, 0);
            dragout.Margin = new System.Windows.Forms.Padding(0);
            dragout.Name = "dragout";
            dragout.Size = new System.Drawing.Size(72, 56);
            dragout.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            dragout.TabIndex = 107;
            dragout.TabStop = false;
            dragout.DragDrop += DragoutDrop;
            dragout.DragOver += Dragout_DragOver;
            dragout.MouseDown += Dragout_MouseDown;
            dragout.MouseEnter += DragoutEnter;
            dragout.MouseLeave += DragoutLeave;
            // 
            // PB_Legal
            // 
            PB_Legal.Image = Properties.Resources.valid;
            PB_Legal.Location = new System.Drawing.Point(0, 0);
            PB_Legal.Margin = new System.Windows.Forms.Padding(0);
            PB_Legal.Name = "PB_Legal";
            PB_Legal.Size = new System.Drawing.Size(24, 24);
            PB_Legal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            PB_Legal.TabIndex = 108;
            PB_Legal.TabStop = false;
            PB_Legal.Click += ClickLegality;
            // 
            // PKME_Tabs
            // 
            PKME_Tabs.AccessibleDescription = "Pokémon Editor Pane";
            PKME_Tabs.AccessibleName = "Pokémon Editor Pane";
            PKME_Tabs.ChangingFields = false;
            PKME_Tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            PKME_Tabs.HaX = false;
            PKME_Tabs.Location = new System.Drawing.Point(0, 0);
            PKME_Tabs.Margin = new System.Windows.Forms.Padding(0);
            PKME_Tabs.Name = "PKME_Tabs";
            PKME_Tabs.Size = new System.Drawing.Size(400, 359);
            PKME_Tabs.TabIndex = 103;
            PKME_Tabs.Unicode = true;
            PKME_Tabs.LegalityChanged += PKME_Tabs_LegalityChanged;
            PKME_Tabs.UpdatePreviewSprite += PKME_Tabs_UpdatePreviewSprite;
            PKME_Tabs.RequestShowdownImport += PKME_Tabs_RequestShowdownImport;
            PKME_Tabs.RequestShowdownExport += PKME_Tabs_RequestShowdownExport;
            PKME_Tabs.SaveFileRequested += PKME_Tabs_SaveFileRequested;
            // 
            // C_SAV
            // 
            C_SAV.AccessibleDescription = "Save File Editor Pane";
            C_SAV.AccessibleName = "Save File Editor Pane";
            C_SAV.Dock = System.Windows.Forms.DockStyle.Fill;
            C_SAV.FlagIllegal = false;
            C_SAV.Location = new System.Drawing.Point(0, 0);
            C_SAV.Margin = new System.Windows.Forms.Padding(0, 0, 1, 3);
            C_SAV.Menu_Redo = null;
            C_SAV.Menu_Undo = null;
            C_SAV.Name = "C_SAV";
            C_SAV.Size = new System.Drawing.Size(454, 356);
            C_SAV.TabIndex = 104;
            C_SAV.ViewIndex = -1;
            C_SAV.RequestCloneData += ClickClone;
            C_SAV.RequestReloadSave += ClickSaveFileName;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer2.IsSplitterFixed = true;
            splitContainer2.Location = new System.Drawing.Point(0, 0);
            splitContainer2.Margin = new System.Windows.Forms.Padding(0);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(L_UpdateAvailable);
            splitContainer2.Panel1.Controls.Add(menuStrip1);
            splitContainer2.Panel1MinSize = 24;
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(splitContainer1);
            splitContainer2.Size = new System.Drawing.Size(856, 385);
            splitContainer2.SplitterDistance = 25;
            splitContainer2.SplitterWidth = 1;
            splitContainer2.TabIndex = 106;
            // 
            // Main
            // 
            AllowDrop = true;
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(856, 385);
            Controls.Add(splitContainer2);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.Icon;
            KeyPreview = true;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "Main";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "PKHeX";
            FormClosing += Main_FormClosing;
            DragDrop += Main_DragDrop;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dragout).EndInit();
            ((System.ComponentModel.ISupportInitialize)PB_Legal).EndInit();
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel1.PerformLayout();
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        public Controls.PKMEditor PKME_Tabs;
        private Controls.SAVEditor C_SAV;
        private System.Windows.Forms.LinkLabel L_UpdateAvailable;

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem Menu_File;
        private System.Windows.Forms.ToolStripMenuItem Menu_Open;
        private System.Windows.Forms.ToolStripMenuItem Menu_Save;
        private System.Windows.Forms.ToolStripMenuItem Menu_Exit;
        private System.Windows.Forms.ToolStripMenuItem Menu_Tools;
        private System.Windows.Forms.ToolStripMenuItem Menu_Options;
        private System.Windows.Forms.ToolStripMenuItem Menu_Language;
        private System.Windows.Forms.ToolStripComboBox CB_MainLanguage;
        private System.Windows.Forms.ToolStripMenuItem Menu_About;
        private System.Windows.Forms.ToolStripMenuItem Menu_ExportSAV;
        private System.Windows.Forms.ToolStripMenuItem Menu_Showdown;
        private System.Windows.Forms.ToolStripMenuItem Menu_ShowdownExportPKM;
        private System.Windows.Forms.ToolStripMenuItem Menu_ShowdownImportPKM;
        private System.Windows.Forms.ToolStripMenuItem Menu_ShowdownExportParty;
        private System.Windows.Forms.ToolStripMenuItem Menu_Folder;
        private System.Windows.Forms.ToolStripMenuItem Menu_Data;
        private System.Windows.Forms.ToolStripMenuItem Menu_LoadBoxes;
        private System.Windows.Forms.ToolStripMenuItem Menu_Report;
        private System.Windows.Forms.ToolStripMenuItem Menu_Database;
        private System.Windows.Forms.ToolStripMenuItem Menu_DumpBoxes;
        private System.Windows.Forms.ToolStripMenuItem Menu_DumpBox;
        private System.Windows.Forms.ToolStripMenuItem Menu_BatchEditor;
        private System.Windows.Forms.ToolStripMenuItem Menu_MGDatabase;
        private System.Windows.Forms.ToolStripMenuItem Menu_Undo;
        private System.Windows.Forms.ToolStripMenuItem Menu_Redo;
        private System.Windows.Forms.ToolStripMenuItem Menu_Settings;
        private System.Windows.Forms.ToolStripMenuItem Menu_ShowdownExportCurrentBox;
        private System.Windows.Forms.ToolStripMenuItem Menu_EncDatabase;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private PKHeX.WinForms.Controls.SelectablePictureBox dragout;
        private System.Windows.Forms.PictureBox PB_Legal;
    }
}

