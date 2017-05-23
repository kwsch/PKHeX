using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Properties;
using System.Configuration;
using System.Threading.Tasks;
using PKHeX.WinForms.Controls;

namespace PKHeX.WinForms
{
    public partial class Main : Form
    {
        public Main()
        {
            #region Initialize Form
            new Thread(() => new SplashScreen().ShowDialog()).Start();
            InitializeComponent();
            C_SAV.PKME_Tabs = PKME_Tabs;
            C_SAV.Menu_Redo = Menu_Redo;
            C_SAV.Menu_Undo = Menu_Undo;
            dragout.GiveFeedback += (sender, e) => { e.UseDefaultCursors = false; };
            GiveFeedback += (sender, e) => { e.UseDefaultCursors = false; };
            PKME_Tabs.EnableDragDrop(tabMain_DragEnter, tabMain_DragDrop);
            C_SAV.EnableDragDrop(tabMain_DragEnter, tabMain_DragDrop);

            // Check for Updates
            L_UpdateAvailable.Click += (sender, e) => Process.Start(ThreadPath);
            new Thread(() =>
            {
                string data = NetUtil.getStringFromURL(VersionPath);
                if (data == null)
                    return;
                try
                {
                    DateTime upd = GetDate(data);
                    DateTime cur = GetDate(Resources.ProgramVersion);

                    if (upd <= cur)
                        return;

                    string message = $"New Update Available! {upd:d}";

                    if (InvokeRequired)
                        try { Invoke((MethodInvoker)ToggleUpdateMessage); }
                        catch { ToggleUpdateMessage(); }
                    else { ToggleUpdateMessage(); }

                    DateTime GetDate(string str) => DateTime.ParseExact(str, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                    void ToggleUpdateMessage() { L_UpdateAvailable.Visible = true; L_UpdateAvailable.Text = message; }
                }
                catch { }
            }).Start();
            
            // Set up Language Selection
            foreach (var cbItem in main_langlist)
                CB_MainLanguage.Items.Add(cbItem);

            // ToolTips for Drag&Drop
            new ToolTip().SetToolTip(dragout, "PKM QuickSave");

            Menu_Modify.DropDown.Closing += (sender, e) =>
            {
                if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                    e.Cancel = true;
            };
            Menu_Options.DropDown.Closing += (sender, e) =>
            {
                if (!Menu_Unicode.Selected)
                    return;
                if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                    e.Cancel = true;
            };

            // Box to Tabs D&D
            dragout.AllowDrop = true;

            // Add ContextMenus
            var mnu = new ContextMenuPKM();
            mnu.RequestEditorLegality += clickLegality;
            mnu.RequestEditorQR += clickQR;
            mnu.RequestEditorSaveAs += mainMenuSave;
            dragout.ContextMenuStrip = mnu.mnuL;

            // Load Event Databases
            refreshMGDB();

            #endregion
            #region Localize & Populate Fields
            string[] args = Environment.GetCommandLineArgs();
            C_SAV.HaX = PKME_Tabs.HaX = HaX = args.Any(x => x.Trim('-').ToLower() == "hax");

            bool showChangelog = false;
            bool BAKprompt = false;
            int languageID = 1; // English
            try
            {
                ConfigUtil.checkConfig();
                loadConfig(out BAKprompt, out showChangelog, out languageID); 
            }
            catch (ConfigurationErrorsException e)
            {
                // Delete the settings if they exist
                var settingsFilename = (e.InnerException as ConfigurationErrorsException)?.Filename;
                if (!string.IsNullOrEmpty(settingsFilename) && File.Exists(settingsFilename))
                    deleteConfig(settingsFilename);
                else
                    WinFormsUtil.Error("Unable to load settings.", e);
            }
            CB_MainLanguage.SelectedIndex = languageID;

            PKME_Tabs.InitializeFields();
            PKME_Tabs.TemplateFields(loadTemplate(C_SAV.SAV));

            #endregion
            #region Load Initial File(s)
            string pkmArg = null;
            foreach (string arg in args.Skip(1)) // skip .exe
            {
                var fi = new FileInfo(arg);
                if (!fi.Exists)
                    continue;

                if (PKX.getIsPKM(fi.Length))
                    pkmArg = arg;
                else
                    OpenQuick(arg, force: true);
            }
            if (!C_SAV.SAV.Exportable) // No SAV loaded from exe args
            {
                string path = null;
                try
                {
                    string cgse = "";
                    string pathCache = CyberGadgetUtil.GetCacheFolder();
                    if (Directory.Exists(pathCache))
                        cgse = Path.Combine(pathCache);
                    if (!PathUtilWindows.detectSaveFile(out path, cgse))
                        WinFormsUtil.Error(path);
                }
                catch (Exception ex)
                {
                    ErrorWindow.ShowErrorDialog("An error occurred while attempting to auto-load your save file.", ex, true);
                }
                
                if (path != null && File.Exists(path))
                    OpenQuick(path, force: true);
                else
                {
                    openSAV(C_SAV.SAV, null);
                    C_SAV.SAV.Edited = false; // Prevents form close warning from showing until changes are made
                }                    
            }
            if (pkmArg != null)
                OpenQuick(pkmArg, force: true);

            formInitialized = true; // Splash Screen closes on its own.
            BringToFront();
            WindowState = FormWindowState.Minimized;
            Show();
            WindowState = FormWindowState.Normal;
            if (HaX) WinFormsUtil.Alert("Illegal mPode activated.", "Please behave.");
            
            if (showChangelog)
                new About().ShowDialog();

            if (BAKprompt && !Directory.Exists(BackupPath))
                promptBackup();

            #endregion
        }

        #region Important Variables
        public static string curlanguage
        {
            get => GameInfo.CurrentLanguage;
            set => GameInfo.CurrentLanguage = value;
        }
        public static string[] gendersymbols = { "♂", "♀", "-" };

        public static bool unicode
        {
            get => Unicode;
            private set
            {
                Unicode = value;
                gendersymbols = value ? new[] {"♂", "♀", "-"} : new[] {"M", "F", "-"};
            }
        }
        private static bool Unicode;

        public static bool HaX;
        public static volatile bool formInitialized;
        private static readonly string[] main_langlist =
            {
                "日本語", // JPN
                "English", // ENG
                "Français", // FRE
                "Italiano", // ITA
                "Deutsch", // GER
                "Español", // SPA
                "한국어", // KOR
                "中文", // CHN
                "Português", // Portuguese
            };
        #endregion

        #region Path Variables

        public static string WorkingDirectory => WinFormsUtil.IsClickonceDeployed ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PKHeX") : Application.StartupPath;
        public static string DatabasePath => Path.Combine(WorkingDirectory, "pkmdb");
        public static string MGDatabasePath => Path.Combine(WorkingDirectory, "mgdb");
        public static string BackupPath => Path.Combine(WorkingDirectory, "bak");
        private static string TemplatePath => Path.Combine(WorkingDirectory, "template");
        private const string ThreadPath = @"https://projectpokemon.org/PKHeX/";
        private const string VersionPath = @"https://raw.githubusercontent.com/kwsch/PKHeX/master/PKHeX.WinForms/Resources/text/version.txt";

        #endregion

        #region //// MAIN MENU FUNCTIONS ////
        
        private void loadConfig(out bool BAKprompt, out bool showChangelog, out int languageID)
        {
            BAKprompt = false;
            showChangelog = false;
            languageID = 1;

            var Settings = Properties.Settings.Default;
            Settings.Upgrade();

            PKME_Tabs.Unicode = unicode = Menu_Unicode.Checked = Settings.Unicode;
            PKME_Tabs.updateUnicode(gendersymbols);
            SaveFile.SetUpdateDex = Menu_ModifyDex.Checked = Settings.SetUpdateDex;
            SaveFile.SetUpdatePKM = C_SAV.ModifyPKM = PKME_Tabs.ModifyPKM = Menu_ModifyPKM.Checked = Settings.SetUpdatePKM;
            C_SAV.FlagIllegal = Menu_FlagIllegal.Checked = Settings.FlagIllegal;
            Menu_ModifyUnset.Checked = Settings.ModifyUnset;

            // Select Language
            string l = Settings.Language;
            int lang = GameInfo.Language(l);
            if (lang < 0)
                lang = GameInfo.Language();
            if (lang > -1)
                languageID = lang;

            // Version Check
            if (Settings.Version.Length > 0) // already run on system
            {
                int.TryParse(Settings.Version, out int lastrev);
                int.TryParse(Resources.ProgramVersion, out int currrev);

                showChangelog = lastrev < currrev;
            }

            // BAK Prompt
            if (!Settings.BAKPrompt)
                BAKprompt = Settings.BAKPrompt = true;

            Settings.Version = Resources.ProgramVersion;
        }
        private static void deleteConfig(string settingsFilename)
        {
            var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "PKHeX's settings are corrupt. Would you like to reset the settings?",
                "Yes to delete the settings or No to close the program.");

            if (dr == DialogResult.Yes)
            {
                File.Delete(settingsFilename);
                WinFormsUtil.Alert("The settings have been deleted", "Please restart the program.");
            }
            Process.GetCurrentProcess().Kill();
        }
        // Main Menu Strip UI Functions
        private void mainMenuOpen(object sender, EventArgs e)
        {
            if (WinFormsUtil.OpenSAVPKMDialog(C_SAV.SAV.PKMExtensions, out string path))
                OpenQuick(path);
        }
        private void mainMenuSave(object sender, EventArgs e)
        {
            if (!PKME_Tabs.verifiedPKM()) return;
            PKM pk = preparePKM();
            WinFormsUtil.SavePKMDialog(pk);
        }
        private void mainMenuExit(object sender, EventArgs e)
        {
            if (ModifierKeys != Keys.Control)
                Close(); // not triggered via hotkey

            if (DialogResult.Yes == WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Quit PKHeX?"))
                Close();
        }
        private void mainMenuAbout(object sender, EventArgs e) => new About().ShowDialog();

        // Sub Menu Options
        private void mainMenuBoxReport(object sender, EventArgs e)
        {
            if (this.FirstFormOfType<frmReport>() is frmReport z)
            { z.CenterToForm(this); z.BringToFront(); return; }
            
            frmReport ReportForm = new frmReport();
            ReportForm.Show();
            ReportForm.PopulateData(C_SAV.SAV.BoxData);
        }
        private void mainMenuDatabase(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Shift)
            {
                if (this.FirstFormOfType<KChart>() is KChart c)
                { c.CenterToForm(this); c.BringToFront(); }
                else
                    new KChart(C_SAV.SAV).Show();
                return;
            }

            if (this.FirstFormOfType<SAV_Database>() is SAV_Database z)
            { z.CenterToForm(this); z.BringToFront(); return; }

            if (Directory.Exists(DatabasePath))
                new SAV_Database(PKME_Tabs, C_SAV).Show();
            else
                WinFormsUtil.Alert("PKHeX's database was not found.",
                    $"Please dump all boxes from a save file, then ensure the '{DatabasePath}' folder exists.");
        }
        private void mainMenuMysteryDB(object sender, EventArgs e)
        {
            if (this.FirstFormOfType<SAV_MysteryGiftDB>() is SAV_MysteryGiftDB z)
            { z.CenterToForm(this); z.BringToFront(); return; }

            new SAV_MysteryGiftDB(PKME_Tabs, C_SAV).Show();
        }
        private void mainMenuUnicode(object sender, EventArgs e)
        {
            Settings.Default.Unicode = PKME_Tabs.Unicode = unicode = Menu_Unicode.Checked;
            PKME_Tabs.updateUnicode(gendersymbols);
        }
        private void mainMenuModifyDex(object sender, EventArgs e) => Settings.Default.SetUpdateDex = SaveFile.SetUpdateDex = Menu_ModifyDex.Checked;
        private void mainMenuModifyUnset(object sender, EventArgs e) => Settings.Default.ModifyUnset = Menu_ModifyUnset.Checked;
        private void mainMenuModifyPKM(object sender, EventArgs e) => Settings.Default.SetUpdatePKM = SaveFile.SetUpdatePKM = Menu_ModifyPKM.Checked;
        private void mainMenuFlagIllegal(object sender, EventArgs e) => C_SAV.FlagIllegal = Settings.Default.FlagIllegal = Menu_FlagIllegal.Checked;

        private void mainMenuBoxLoad(object sender, EventArgs e)
        {
            string path = null;
            if (Directory.Exists(DatabasePath))
            {
                var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Load from PKHeX's database?");
                if (dr == DialogResult.Yes)
                    path = DatabasePath;
            }
            if (C_SAV.LoadBoxes(out string result, path))
                WinFormsUtil.Alert(result);
        }
        private void mainMenuBoxDump(object sender, EventArgs e)
        {
            // Dump all of box content to files.
            string path = null;
            DialogResult ld = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Save to PKHeX's database?");
            if (ld == DialogResult.Yes)
                path = DatabasePath;
            else if (ld != DialogResult.No)
                return;

            if (C_SAV.DumpBoxes(out string result, path))
                WinFormsUtil.Alert(result);
        }
        private void mainMenuBoxDumpSingle(object sender, EventArgs e)
        {
            if (C_SAV.DumpBox(out string result))
                WinFormsUtil.Alert(result);
        }
        private void mainMenuBatchEditor(object sender, EventArgs e)
        {
            new BatchEditor(PKME_Tabs.preparePKM(), C_SAV.SAV).ShowDialog();
            C_SAV.setPKXBoxes(); // refresh
            C_SAV.updateBoxViewers();
        }
        private void mainMenuFolder(object sender, EventArgs e) => new SAV_FolderList().ShowDialog();
        // Misc Options
        private void clickShowdownImportPKM(object sender, EventArgs e)
        {
            if (!formInitialized)
                return;
            if (!Clipboard.ContainsText())
            { WinFormsUtil.Alert("Clipboard does not contain text."); return; }

            // Get Simulator Data
            ShowdownSet Set = new ShowdownSet(Clipboard.GetText());

            if (Set.Species < 0)
            { WinFormsUtil.Alert("Set data not found in clipboard."); return; }

            if (Set.Nickname != null && Set.Nickname.Length > C_SAV.SAV.NickLength)
                Set.Nickname = Set.Nickname.Substring(0, C_SAV.SAV.NickLength);

            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Import this set?", Set.getText())) 
            { return; }

            if (Set.InvalidLines.Any())
                WinFormsUtil.Alert("Invalid lines detected:", string.Join(Environment.NewLine, Set.InvalidLines));

            // Set Species & Nickname
            PKME_Tabs.LoadShowdownSet(Set, C_SAV.SAV);
        }
        private void clickShowdownExportPKM(object sender, EventArgs e)
        {
            if (!formInitialized)
                return;
            if (!PKME_Tabs.verifiedPKM())
            { WinFormsUtil.Alert("Fix data before exporting."); return; }

            Clipboard.SetText(preparePKM().ShowdownText);
            WinFormsUtil.Alert("Exported Showdown Set to Clipboard:", Clipboard.GetText());
        }
        private void clickShowdownExportParty(object sender, EventArgs e)
        {
            if (C_SAV.SAV.PartyData.Length <= 0) return;
            try
            {
                Clipboard.SetText(
                    C_SAV.SAV.PartyData.Aggregate("", (current, pk) => current + pk.ShowdownText
                            + Environment.NewLine + Environment.NewLine).Trim());
                WinFormsUtil.Alert("Showdown Team (Party) set to Clipboard.");
            }
            catch { }
        }
        private void clickShowdownExportBattleBox(object sender, EventArgs e)
        {
            if (C_SAV.SAV.BattleBoxData.Length <= 0) return;
            try
            {
                Clipboard.SetText(
                    C_SAV.SAV.BattleBoxData.Aggregate("", (current, pk) => current + pk.ShowdownText
                            + Environment.NewLine + Environment.NewLine).Trim());
                WinFormsUtil.Alert("Showdown Team (Battle Box) set to Clipboard.");
            }
            catch { }
        }

        // Main Menu Subfunctions
        private void OpenQuick(string path, bool force = false)
        {
            if (!(CanFocus || force))
            {
                SystemSounds.Asterisk.Play();
                return;
            }
            // detect if it is a folder (load into boxes or not)
            if (Directory.Exists(path))
            { C_SAV.LoadBoxes(out string _, path); return; }

            string ext = Path.GetExtension(path);
            FileInfo fi = new FileInfo(path);
            if (fi.Length > 0x10009C && fi.Length != 0x380000 && ! SAV3GCMemoryCard.IsMemoryCardSize(fi.Length))
                WinFormsUtil.Error("Input file is too large." + Environment.NewLine + $"Size: {fi.Length} bytes", path);
            else if (fi.Length < 32)
                WinFormsUtil.Error("Input file is too small." + Environment.NewLine + $"Size: {fi.Length} bytes", path);
            else
            {
                byte[] input; try { input = File.ReadAllBytes(path); }
                catch (Exception e) { WinFormsUtil.Error("Unable to load file.  It could be in use by another program.\nPath: " + path, e); return; }

                #if DEBUG
                OpenFile(input, path, ext, C_SAV.SAV);
                #else
                try { OpenFile(input, path, ext, C_SAV.SAV); }
                catch (Exception e) { WinFormsUtil.Error("Unable to load file.\nPath: " + path, e); }
                #endif
            }
        }
        private void OpenFile(byte[] input, string path, string ext, SaveFile currentSaveFile)
        {
            if (TryLoadXorpadSAV(input, path))
                return;
            if (TryLoadSAV(input, path))
                return;
            if (TryLoadMemoryCard(input, path))
                return;
            if (TryLoadPKM(input, path, ext, currentSaveFile))
                return;
            if (TryLoadPCBoxBin(input))
                return;
            if (TryLoadBattleVideo(input))
                return;
            if (TryLoadMysteryGift(input, path, ext))
                return;

            WinFormsUtil.Error("Attempted to load an unsupported file type/size.",
                $"File Loaded:{Environment.NewLine}{path}",
                $"File Size:{Environment.NewLine}{input.Length} bytes (0x{input.Length:X4})");
        }
        private bool TryLoadXorpadSAV(byte[] input, string path)
        {
            if (input.Length == 0x10009C) // Resize to 1MB
            {
                Array.Copy(input, 0x9C, input, 0, 0x100000);
                Array.Resize(ref input, 0x100000);
            }
            if (input.Length != 0x100000)
                return false;
            if (openXOR(input, path)) // Check if we can load the save via xorpad
                return true;

            if (BitConverter.ToUInt64(input, 0x10) != 0) // encrypted save
            {
                WinFormsUtil.Error("PKHeX only edits decrypted save files." + Environment.NewLine + "This save file is not decrypted.", path);
                return true;
            }

            DialogResult sdr = WinFormsUtil.Prompt(MessageBoxButtons.YesNoCancel, "Press Yes to load the sav at 0x3000",
                "Press No for the one at 0x82000");
            if (sdr == DialogResult.Cancel)
                return true;
            int savshift = sdr == DialogResult.Yes ? 0 : 0x7F000;
            byte[] psdata = input.Skip(0x5400 + savshift).Take(SaveUtil.SIZE_G6ORAS).ToArray();

            if (BitConverter.ToUInt32(psdata, SaveUtil.SIZE_G6ORAS - 0x1F0) == SaveUtil.BEEF)
                Array.Resize(ref psdata, SaveUtil.SIZE_G6ORAS); // set to ORAS size
            else if (BitConverter.ToUInt32(psdata, SaveUtil.SIZE_G6XY - 0x1F0) == SaveUtil.BEEF)
                Array.Resize(ref psdata, SaveUtil.SIZE_G6XY); // set to X/Y size
            else if (BitConverter.ToUInt32(psdata, SaveUtil.SIZE_G7SM - 0x1F0) == SaveUtil.BEEF)
                Array.Resize(ref psdata, SaveUtil.SIZE_G7SM); // set to S/M size
            else
            {
                WinFormsUtil.Error("The data file is not a valid save file", path);
                return true;
            }

            openSAV(SaveUtil.getVariantSAV(psdata), path);
            return true;
        }
        private bool TryLoadSAV(byte[] input, string path)
        {
            var sav = SaveUtil.getVariantSAV(input);
            if (sav == null)
                return false;
            openSAV(sav, path);
            return true;
        }
        private bool TryLoadMemoryCard(byte[] input, string path)
        {
            if (!SAV3GCMemoryCard.IsMemoryCardSize(input))
                return false;
            SAV3GCMemoryCard MC = CheckGCMemoryCard(input, path);
            if (MC == null)
                return false;
            var sav = SaveUtil.getVariantSAV(MC);
            if (sav == null)
                return false;
            openSAV(sav, path);
            return true;
        }
        private bool TryLoadPKM(byte[] input, string path, string ext, SaveFile SAV)
        {
            var temp = PKMConverter.getPKMfromBytes(input, prefer: ext.Length > 0 ? (ext.Last() - 0x30) & 7 : C_SAV.SAV.Generation);
            if (temp == null)
                return false;

            var type = PKME_Tabs.pkm.GetType();
            string c;
            PKM pk = PKMConverter.convertToFormat(temp, type, out c);
            if (pk == null)
            {
                WinFormsUtil.Alert("Conversion failed.", c);
                return false;
            }
            if (SAV.Generation < 3 && ((pk as PK1)?.Japanese ?? ((PK2)pk).Japanese) != SAV.Japanese)
            {
                var strs = new[] { "International", "Japanese" };
                var val = SAV.Japanese ? 0 : 1;
                WinFormsUtil.Alert($"Cannot load {strs[val]} {pk.GetType().Name}s to {strs[val ^ 1]} saves.");
                return false;
            }
            
            PKME_Tabs.populateFields(pk);
            Console.WriteLine(c);
            return true;
        }
        private bool TryLoadPCBoxBin(byte[] input)
        {
            if (!(BitConverter.ToUInt16(input, 4) == 0 && BitConverter.ToUInt32(input, 8) != 0 && C_SAV.IsPCBoxBin(input.Length)))
                return false;
            if (!C_SAV.OpenPCBoxBin(input, out string c))
            {
                WinFormsUtil.Alert("Binary is not compatible with save file.", c);
                return false;
            }

            WinFormsUtil.Alert(c);
            return true;
        }
        private bool TryLoadBattleVideo(byte[] input)
        {
            if (!BattleVideo.getIsValid(input))
                return false;

            BattleVideo b = BattleVideo.getVariantBattleVideo(input);
            bool result = C_SAV.OpenBattleVideo(b, out string c);
            WinFormsUtil.Alert(c);
            Console.WriteLine(c);
            return result;
        }
        private bool TryLoadMysteryGift(byte[] input, string path, string ext)
        {
            var tg = MysteryGift.getMysteryGift(input, ext);
            if (tg == null)
                return false;
            if (!tg.IsPokémon)
            {
                WinFormsUtil.Alert("Mystery Gift is not a Pokémon.", path);
                return true;
            }

            var temp = tg.convertToPKM(C_SAV.SAV);
            PKM pk = PKMConverter.convertToFormat(temp, C_SAV.SAV.PKMType, out string c);

            if (pk == null)
            {
                WinFormsUtil.Alert("Conversion failed.", c);
                return true;
            }

            PKME_Tabs.populateFields(pk);
            Console.WriteLine(c);
            return true;
        }

        private bool openXOR(byte[] input, string path)
        {
            // try to get a save file via xorpad in same folder
            var folder = new DirectoryInfo(path).Parent.FullName;
            string[] pads = Directory.GetFiles(folder);
            var s = SaveUtil.getSAVfromXORpads(input, pads);

            if (s == null) // failed to find xorpad in path folder
            {
                // try again
                pads = Directory.GetFiles(WorkingDirectory);
                s = SaveUtil.getSAVfromXORpads(input, pads);
            }

            if (s == null)
                return false; // failed

            openSAV(s, s.FileName);
            return true;
        }
        private static GameVersion SelectMemoryCardSaveGame(SAV3GCMemoryCard MC)
        {
            if (MC.SaveGameCount == 1)
                return MC.SelectedGameVersion;

            var games = new List<ComboItem>();
            if (MC.HasCOLO) games.Add(new ComboItem { Text = "Colosseum", Value = (int)GameVersion.COLO });
            if (MC.HasXD) games.Add(new ComboItem { Text = "XD", Value = (int)GameVersion.XD });
            if (MC.HasRSBOX) games.Add(new ComboItem { Text = "RS Box", Value = (int)GameVersion.RSBOX });

            WinFormsUtil.Alert("Multiple games detected", "Select a game to edit.");
            var dialog = new SAV_GameSelect(games.ToArray());
            dialog.ShowDialog();
            return dialog.Result;
        }
        private static SAV3GCMemoryCard CheckGCMemoryCard(byte[] Data, string path)
        {
            SAV3GCMemoryCard MC = new SAV3GCMemoryCard();
            GCMemoryCardState MCState = MC.LoadMemoryCardFile(Data);
            switch (MCState)
            {
                default: { WinFormsUtil.Error("Invalid or corrupted GC Memory Card. Aborting.", path); return null; }
                case GCMemoryCardState.NoPkmSaveGame: { WinFormsUtil.Error("GC Memory Card without any Pokémon save file. Aborting.", path); return null; }
                case GCMemoryCardState.DuplicateCOLO:
                case GCMemoryCardState.DuplicateXD:
                case GCMemoryCardState.DuplicateRSBOX: { WinFormsUtil.Error("GC Memory Card with duplicated game save files. Aborting.", path); return null; }
                case GCMemoryCardState.MultipleSaveGame:
                    {
                        GameVersion Game = SelectMemoryCardSaveGame(MC);
                        if (Game == GameVersion.Invalid) //Cancel
                            return null;
                        MC.SelectSaveGame(Game);
                        break;
                    }
                case GCMemoryCardState.SaveGameCOLO:    MC.SelectSaveGame(GameVersion.COLO); break;
                case GCMemoryCardState.SaveGameXD:      MC.SelectSaveGame(GameVersion.XD); break;
                case GCMemoryCardState.SaveGameRSBOX:   MC.SelectSaveGame(GameVersion.RSBOX); break;
            }
            return MC;
        }

        private static void StoreLegalSaveGameData(SaveFile sav)
        {
            Legal.SavegameJapanese = sav.Japanese;
            Legal.EReaderBerryIsEnigma = sav.eBerryIsEnigma;
            Legal.EReaderBerryName = sav.eBerryName;
            Legal.Savegame_Gender = sav.Gender;
            Legal.Savegame_TID = sav.TID;
            Legal.Savegame_SID = sav.SID;
            Legal.Savegame_OT = sav.OT;
            Legal.Savegame_Version = sav.Version;
        }
        private static PKM loadTemplate(SaveFile sav)
        {
            if (!Directory.Exists(TemplatePath))
                return null;

            var blank = sav.BlankPKM;
            string path = Path.Combine(TemplatePath, new DirectoryInfo(TemplatePath).Name + "." + blank.Extension);

            if (!File.Exists(path) || !PKX.getIsPKM(new FileInfo(path).Length))
                return null;

            var pk = PKMConverter.getPKMfromBytes(File.ReadAllBytes(path), prefer: blank.Format);
            return PKMConverter.convertToFormat(pk, sav.BlankPKM.GetType(), out path); // no sneaky plz; reuse string
        }
        private static void refreshMGDB()
        {
            Legal.RefreshMGDB(MGDatabasePath);
        }

        private void openSAV(SaveFile sav, string path)
        {
            if (sav == null || sav.Version == GameVersion.Invalid)
            { WinFormsUtil.Error("Invalid save file loaded. Aborting.", path); return; }

            if (!sanityCheckSAV(ref sav, path))
                return;
            StoreLegalSaveGameData(sav);

            // clean fields
            SaveFile SAV = C_SAV.SAV = sav;
            C_SAV.M.Reset();

            string title = $"PKH{(HaX ? "a" : "e")}X ({Resources.ProgramVersion}) - " + $"{SAV.GetType().Name}: ";
            if (path != null) // Actual save file
            {
                SAV.FilePath = Path.GetDirectoryName(path);
                SAV.FileName = Path.GetExtension(path) == ".bak"
                    ? Path.GetFileName(path).Split(new[] { " [" }, StringSplitOptions.None)[0]
                    : Path.GetFileName(path);
                Text = title + $"{Path.GetFileNameWithoutExtension(Util.CleanFileName(SAV.BAKName))}"; // more descriptive

                // If backup folder exists, save a backup.
                string backupName = Path.Combine(BackupPath, Util.CleanFileName(SAV.BAKName));
                if (SAV.Exportable && Directory.Exists(BackupPath) && !File.Exists(backupName))
                    File.WriteAllBytes(backupName, SAV.BAK);
            }
            else // Blank save file
            {
                SAV.FilePath = null;
                SAV.FileName = "Blank Save File";
                Text = title + $"{SAV.FileName} [{SAV.OT} ({SAV.Version})]";
            }
            Menu_ExportSAV.Enabled = SAV.Exportable;

            // No changes made yet
            Menu_Undo.Enabled = false;
            Menu_Redo.Enabled = false;

            bool WindowToggleRequired = C_SAV.SAV.Generation < 3 && sav.Generation >= 3; // version combobox refresh hack
            bool WindowTranslationRequired = false;
            PKM pk = preparePKM();
            PKME_Tabs.pkm = SAV.BlankPKM;
            PKME_Tabs.setPKMFormatMode(SAV.Generation);
            PKME_Tabs.populateFields(PKME_Tabs.pkm);
            C_SAV.SAV = sav;
            
            // Initialize Subviews
            PKME_Tabs.ToggleInterface();
            bool init = PKME_Tabs.fieldsInitialized;
            PKME_Tabs.fieldsInitialized = PKME_Tabs.fieldsLoaded = false;
            WindowTranslationRequired |= PKME_Tabs.FinalizeInterface(init, SAV, pk);
            WindowTranslationRequired |= C_SAV.ToggleInterface();
            C_SAV.FinalizeInterface();

            // Finalize Overall Info
            Menu_LoadBoxes.Enabled = Menu_DumpBoxes.Enabled = Menu_Report.Enabled = Menu_Modify.Enabled = C_SAV.SAV.HasBox;
            if (WindowTranslationRequired) // force update -- re-added controls may be untranslated
            {
                // Keep window title
                title = Text;
                WinFormsUtil.TranslateInterface(this, curlanguage);
                Text = title;
            }
            if (WindowToggleRequired) // Version combobox selectedvalue needs a little help, only updates once it is visible
            {
                PKME_Tabs.FlickerInterface();
            }

            if (!string.IsNullOrWhiteSpace(path)) // Actual Save
            {
                // Check location write protection
                bool locked = true;
                try { locked = File.GetAttributes(path).HasFlag(FileAttributes.ReadOnly); }
                catch { }

                if (locked)
                    WinFormsUtil.Alert("File's location is write protected:\n" + path,
                        "If the path is a removable disk (SD card), please ensure the write protection switch is not set.");
            }

            PKME_Tabs.TemplateFields(loadTemplate(SAV));
            SAV.Edited = false;

            PKMConverter.updateConfig(SAV.SubRegion, SAV.Country, SAV.ConsoleRegion, SAV.OT, SAV.Gender, SAV.Language);
            SystemSounds.Beep.Play();
        }

        private static bool sanityCheckSAV(ref SaveFile sav, string path)
        {
            if (!string.IsNullOrEmpty(path)) // If path is null, this is the default save
            {
                if (sav.RequiresMemeCrypto && !MemeCrypto.CanUseMemeCrypto())
                {
                    WinFormsUtil.Error("Your platform does not support the required cryptography components.",
                        "In order to be able to save your changes, you must either upgrade to a newer version of Windows or disable FIPS compliance mode.");
                    // Don't abort loading; user can still view save and fix checksum on another platform.
                }
            }
            // Finish setting up the save file.
            if (sav.Generation == 1)
            {
                // Ask the user if it is a VC save file or if it is from a physical cartridge.
                // Necessary for legality checking possibilities that are only obtainable on GSC (non VC) or event distributions.
                var drVC = WinFormsUtil.Prompt(MessageBoxButtons.YesNoCancel,
                    $"{sav.Version} Save File detected. Is this a Virtual Console Save File?",
                    "Yes: Virtual Console" + Environment.NewLine + "No: Physical Cartridge");
                if (drVC == DialogResult.Cancel)
                    return false;
                Legal.AllowGBCartEra = drVC == DialogResult.No; // physical cart selected
                if (Legal.AllowGBCartEra && sav.Generation == 1)
                {
                    var drTradeback = WinFormsUtil.Prompt(MessageBoxButtons.YesNoCancel,
                        $"Generation {sav.Generation} Save File detected. Allow tradebacks from Generation 2 for legality purposes?",
                        "Yes: Allow Generation 2 tradeback learnsets" + Environment.NewLine +
                        "No: Don't allow Generation 2 tradeback learnsets");
                    Legal.AllowGen1Tradeback = drTradeback == DialogResult.Yes;
                }
                else
                    Legal.AllowGen1Tradeback = false;
            }
            else
                Legal.AllowGBCartEra = Legal.AllowGen1Tradeback = sav.Generation == 2;

            if (sav.Generation == 3 && (sav.IndeterminateGame || ModifierKeys == Keys.Control))
            {
                WinFormsUtil.Alert($"Generation {sav.Generation} Save File detected.", "Select version.");
                var g = new[] {GameVersion.R, GameVersion.S, GameVersion.E, GameVersion.FR, GameVersion.LG};
                var games = g.Select(z => GameInfo.VersionDataSource.First(v => v.Value == (int) z));
                var dialog = new SAV_GameSelect(games);
                dialog.ShowDialog();

                switch (dialog.Result) // Reset save file info
                {
                    case GameVersion.R:
                    case GameVersion.S:
                        sav = new SAV3(sav.BAK, GameVersion.RS);
                        break;
                    case GameVersion.E:
                        sav = new SAV3(sav.BAK, GameVersion.E);
                        break;
                    case GameVersion.FR:
                    case GameVersion.LG:
                        sav = new SAV3(sav.BAK, GameVersion.FRLG);
                        break;
                    default: return false;
                }
                if (sav.Version == GameVersion.FRLG)
                    sav.Personal = dialog.Result == GameVersion.FR ? PersonalTable.FR : PersonalTable.LG;
            }
            else if (sav.IndeterminateSubVersion && sav.Version == GameVersion.FRLG)
            {
                string fr = GameInfo.VersionDataSource.First(r => r.Value == (int) GameVersion.FR).Text;
                string lg = GameInfo.VersionDataSource.First(l => l.Value == (int) GameVersion.LG).Text;
                const string dual = "{0}/{1} Save File Detected.";
                WinFormsUtil.Alert(string.Format(dual, fr, lg), "Select version.");
                var g = new[] {GameVersion.FR, GameVersion.LG};
                var games = g.Select(z => GameInfo.VersionDataSource.First(v => v.Value == (int) z));
                var dialog = new SAV_GameSelect(games);
                dialog.ShowDialog();

                sav.Personal = dialog.Result == GameVersion.FR ? PersonalTable.FR : PersonalTable.LG;
            }

            if (sav.IndeterminateLanguage)
            {
                // Japanese Save files are different. Get isJapanese
                var drJP = WinFormsUtil.Prompt(MessageBoxButtons.YesNoCancel,
                    $"{sav.Version} Save File detected. Select language...",
                    "Yes: International" + Environment.NewLine + "No: Japanese");
                if (drJP == DialogResult.Cancel)
                    return false;

                sav.Japanese = drJP == DialogResult.No;
            }
            return true;
        }


        public static void setCountrySubRegion(ComboBox CB, string type)
        {
            int index = CB.SelectedIndex;
            // fix for Korean / Chinese being swapped
            string cl = GameInfo.CurrentLanguage + "";
            cl = cl == "zh" ? "ko" : cl == "ko" ? "zh" : cl;

            CB.DataSource = Util.getCBList(type, cl);

            if (index > 0 && index < CB.Items.Count)
                CB.SelectedIndex = index;
        }

        // Language Translation
        private void changeMainLanguage(object sender, EventArgs e)
        {
            if (CB_MainLanguage.SelectedIndex < 8)
                curlanguage = GameInfo.Language2Char((uint)CB_MainLanguage.SelectedIndex);

            // Set the culture (makes it easy to pass language to other forms)
            Settings.Default.Language = curlanguage;
            Thread.CurrentThread.CurrentCulture = new CultureInfo(curlanguage.Substring(0, 2));
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            PKM pk = C_SAV.SAV.getPKM((PKME_Tabs.fieldsInitialized ? preparePKM() : PKME_Tabs.pkm).Data);
            bool alreadyInit = PKME_Tabs.fieldsInitialized;
            PKME_Tabs.fieldsInitialized = false;
            Menu_Options.DropDown.Close();
            InitializeStrings();
            PKME_Tabs.InitializeLanguage(C_SAV.SAV);
            string ProgramTitle = Text;
            WinFormsUtil.TranslateInterface(this, curlanguage); // Translate the UI to language.
            Text = ProgramTitle;
            PKME_Tabs.CenterSubEditors();
            PKME_Tabs.populateFields(pk); // put data back in form
            PKME_Tabs.fieldsInitialized |= alreadyInit;            
        }
        private void InitializeStrings()
        {            
            string l = curlanguage;
            GameInfo.Strings = GameInfo.getStrings(l);

            // Update Legality Strings
            // Clipboard.SetText(string.Join(Environment.NewLine, Util.getLocalization(typeof(LegalityCheckStrings))));
            Task.Run(() => Util.setLocalization(typeof(LegalityCheckStrings), Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Substring(0, 2)));

            // Force an update to the met locations
            PKME_Tabs.origintrack = GameVersion.Unknown;

            // Update Legality Analysis strings
            LegalityAnalysis.movelist = GameInfo.Strings.movelist;
            LegalityAnalysis.specieslist = GameInfo.Strings.specieslist;

            if (PKME_Tabs.fieldsInitialized)
                PKME_Tabs.updateStringDisplay();
        }
        #endregion

        #region //// PKX WINDOW FUNCTIONS ////


        private bool QR6Notified;
        private void clickQR(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Alt)
            {
                // Fetch data from QR code...
                byte[] ekx = QR.getQRData();

                if (ekx == null) return;

                PKM pk = PKMConverter.getPKMfromBytes(ekx, prefer: C_SAV.SAV.Generation);
                if (pk == null) { WinFormsUtil.Alert("Decoded data not a valid PKM.", $"QR Data Size: {ekx.Length}"); }
                else
                {
                    if (!pk.Valid || pk.Species <= 0)
                    { WinFormsUtil.Alert("Invalid data detected."); return; }

                    PKM pkz = PKMConverter.convertToFormat(pk, C_SAV.SAV.PKMType, out string c);
                    if (pkz == null)
                    { WinFormsUtil.Alert(c); return; }

                    PKME_Tabs.populateFields(pkz);
                }
            }
            else
            {
                if (!PKME_Tabs.verifiedPKM()) return;
                PKM pkx = preparePKM();

                Image qr;
                switch (pkx.Format)
                {
                    case 7:
                        qr = QR.GenerateQRCode7((PK7)pkx);
                        break;
                    default:
                        if (pkx.Format == 6 && !QR6Notified) // hint that the user should not be using QR6 injection
                        {
                            WinFormsUtil.Alert("QR codes are deprecated in favor of other methods.",
                                "Consider utilizing homebrew or on-the-fly RAM editing custom firmware (PKMN-NTR).");
                            QR6Notified = true;
                        }
                        qr = QR.getQRImage(pkx.EncryptedBoxData, QR.getQRServer(pkx.Format));
                        break;
                }

                if (qr == null) return;

                var sprite = dragout.Image;
                var la = new LegalityAnalysis(pkx);
                if (la.Parsed && pkx.Species != 0)
                {
                    var img = la.Valid ? Resources.valid : Resources.warn;
                    sprite = ImageUtil.LayerImage(sprite, img, 24, 0, 1);
                }

                string[] r = pkx.QRText;
                string refer = $"PKHeX ({Resources.ProgramVersion})";
                new QR(qr, sprite, r[0], r[1], r[2], $"{refer} ({pkx.GetType().Name})", pkx).ShowDialog();
            }
        }
        private void clickLegality(object sender, EventArgs e)
        {
            if (!PKME_Tabs.verifiedPKM())
            { SystemSounds.Asterisk.Play(); return; }

            var pk = preparePKM();

            if (pk.Species == 0 || !pk.ChecksumValid)
            { SystemSounds.Asterisk.Play(); return; }

            LegalityAnalysis la = new LegalityAnalysis(pk);
            if (pk.Slot < 0)
                PKME_Tabs.updateLegality(la);
            bool verbose = ModifierKeys == Keys.Control;
            var report = la.Report(verbose);
            if (verbose)
            {
                var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, report, "Copy report to Clipboard?");
                if (dr == DialogResult.Yes)
                    Clipboard.SetText(report);
            }
            else
                WinFormsUtil.Alert(report);
        }
        private void clickClone(object sender, EventArgs e)
        {
            if (!PKME_Tabs.verifiedPKM()) return; // don't copy garbage to the box
            PKM pk = PKME_Tabs.preparePKM();
            C_SAV.SetClonesToBox(pk);
        }
        private void getPreview(PictureBox pb, PKM pk = null)
        {
            if (!PKME_Tabs.fieldsInitialized) return;
            pk = pk ?? preparePKM(false); // don't perform control loss click

            if (pb == dragout) dragout.ContextMenuStrip.Enabled = pk.Species != 0 || HaX; // Species

            pb.Image = pk.Sprite(C_SAV.SAV, -1, -1, true);
            if (pb.BackColor == Color.Red)
                pb.BackColor = Color.Transparent;
        }
        private void PKME_Tabs_UpdatePreviewSprite(object sender, EventArgs e) => getPreview(dragout);
        private void PKME_Tabs_LegalityChanged(object sender, EventArgs e)
        {
            if (PKME_Tabs.IsLegal == null)
            {
                PB_Legal.Visible = false;
                return;
            }

            PB_Legal.Visible = true;
            PB_Legal.Image = PKME_Tabs.IsLegal == false ? Resources.warn : Resources.valid;
        }
        private void PKME_Tabs_RequestShowdownExport(object sender, EventArgs e) => clickShowdownExportPKM(sender, e);
        private void PKME_Tabs_RequestShowdownImport(object sender, EventArgs e) => clickShowdownImportPKM(sender, e);
        private SaveFile PKME_Tabs_SaveFileRequested(object sender, EventArgs e) => C_SAV.SAV;
        // Open/Save Array Manipulation //
        public PKM preparePKM(bool click = true) => PKME_Tabs.preparePKM(click);

        // Drag & Drop Events
        private void tabMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.AllowedEffect == (DragDropEffects.Copy | DragDropEffects.Link)) // external file
                e.Effect = DragDropEffects.Copy;
            else if (e.Data != null) // within
                e.Effect = DragDropEffects.Move;
        }
        private void tabMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            OpenQuick(files[0]);
            e.Effect = DragDropEffects.Copy;

            Cursor = DefaultCursor;
        }
        // Decrypted Export
        private void dragout_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && (ModifierKeys == Keys.Alt || ModifierKeys == Keys.Shift))
                clickQR(sender, e);
            if (e.Button == MouseButtons.Right)
                return;
            if (!PKME_Tabs.verifiedPKM())
                return;

            // Create Temp File to Drag
            PKM pkx = preparePKM();
            bool encrypt = ModifierKeys == Keys.Control;
            string fn = pkx.FileName; fn = fn.Substring(0, fn.LastIndexOf('.'));
            string filename = $"{fn}{(encrypt ? ".ek" + pkx.Format : "." + pkx.Extension)}";
            byte[] dragdata = encrypt ? pkx.EncryptedBoxData : pkx.DecryptedBoxData;
            // Make file
            string newfile = Path.Combine(Path.GetTempPath(), Util.CleanFileName(filename));
            try
            {
                File.WriteAllBytes(newfile, dragdata);
                PictureBox pb = (PictureBox)sender;
                C_SAV.M.DragInfo.Source.PKM = pkx;
                C_SAV.M.DragInfo.Cursor = Cursor = new Cursor(((Bitmap)pb.Image).GetHicon());
                DoDragDrop(new DataObject(DataFormats.FileDrop, new[] { newfile }), DragDropEffects.Move);
            }
            catch (Exception x)
            { WinFormsUtil.Error("Drag & Drop Error", x); }
            C_SAV.M.SetCursor(DefaultCursor, sender);
            File.Delete(newfile);
        }
        private void dragout_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
        // Dragout Display
        private void dragoutEnter(object sender, EventArgs e)
        {
            dragout.BackgroundImage = WinFormsUtil.getIndex(PKME_Tabs.CB_Species) > 0 ? Resources.slotSet : Resources.slotDel;
            Cursor = Cursors.Hand;
        }
        private void dragoutLeave(object sender, EventArgs e)
        {
            dragout.BackgroundImage = Resources.slotTrans;
            if (Cursor == Cursors.Hand)
                Cursor = Cursors.Default;
        }
        private void dragoutDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            OpenQuick(files[0]);
            e.Effect = DragDropEffects.Copy;

            Cursor = DefaultCursor;
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (C_SAV.SAV.Edited || PKME_Tabs.PKMIsUnsaved)
            {
                var prompt = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Any unsaved changes will be lost.", "Are you sure you want to close PKHeX?");
                if (prompt != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }

            try { Settings.Default.Save(); }
            catch (Exception x) { File.WriteAllLines("config error.txt", new[] { x.ToString() }); }
        }
        #endregion

        #region //// SAVE FILE FUNCTIONS ////
        private void clickExportSAVBAK(object sender, EventArgs e)
        {
            if (C_SAV.ExportBackup() && !Directory.Exists(BackupPath))
                promptBackup();
        }
        private void clickExportSAV(object sender, EventArgs e)
        {
            if (!Menu_ExportSAV.Enabled)
                return;

            C_SAV.ExportSaveFile();
        }
        private void clickSaveFileName(object sender, EventArgs e)
        {
            string cgse = "";
            string pathCache = CyberGadgetUtil.GetCacheFolder();
            if (Directory.Exists(pathCache))
                cgse = Path.Combine(pathCache);
            if (!PathUtilWindows.detectSaveFile(out string path, cgse))
                WinFormsUtil.Error(path);
            if (path == null || !File.Exists(path)) return;
            if (WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Open save file from the following location?", path) == DialogResult.Yes)
                OpenQuick(path); // load save
        }
        private static void promptBackup()
        {
            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo,
                $"PKHeX can perform automatic backups if you create a folder with the name \"{BackupPath}\" in the same folder as PKHeX's executable.",
                "Would you like to create the backup folder now?")) return;

            try
            {
                Directory.CreateDirectory(BackupPath); WinFormsUtil.Alert("Backup folder created!",
              $"If you wish to no longer automatically back up save files, delete the \"{BackupPath}\" folder.");
            }
            catch (Exception ex) { WinFormsUtil.Error($"Unable to create backup folder @ {BackupPath}", ex); }
        }

        private void clickUndo(object sender, EventArgs e) => C_SAV.clickUndo();
        private void clickRedo(object sender, EventArgs e) => C_SAV.clickRedo();
        #endregion
    }
}
