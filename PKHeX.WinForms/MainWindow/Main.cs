using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using PKHeX.Core;
using PKHeX.WinForms.Controls;
using PKHeX.WinForms.Properties;

namespace PKHeX.WinForms
{
    public partial class Main : Form
    {
        public Main()
        {
            new Task(() => new SplashScreen().ShowDialog()).Start();
            new Task(RefreshMGDB).Start();
            InitializeComponent();

            FormLoadCheckForUpdates();
            FormLoadAddEvents();

            string[] args = Environment.GetCommandLineArgs();
            FormLoadInitialSettings(args, out bool showChangelog, out bool BAKprompt);
            FormLoadInitialFiles(args);

            IsInitialized = true; // Splash Screen closes on its own.
            PKME_Tabs_UpdatePreviewSprite(null, null);
            BringToFront();
            WindowState = FormWindowState.Minimized;
            Show();
            WindowState = FormWindowState.Normal;
            if (HaX)
                WinFormsUtil.Alert("Illegal mode activated.", "Please behave.");
            else if (showChangelog)
                new About().ShowDialog();

            if (BAKprompt && !Directory.Exists(BackupPath))
                PromptBackup();
        }

        #region Important Variables
        public static string CurrentLanguage
        {
            get => GameInfo.CurrentLanguage;
            private set => GameInfo.CurrentLanguage = value;
        }
        private static bool _unicode { get; set; }
        public static bool Unicode
        {
            get => _unicode;
            private set
            {
                _unicode = value;
                GenderSymbols = value ? new[] {"♂", "♀", "-"} : new[] {"M", "F", "-"};
            }
        }

        public static string[] GenderSymbols { get; private set; } = { "♂", "♀", "-" };
        public static bool HaX { get; private set; }
        public static bool IsInitialized { get; private set; }
        private readonly string[] main_langlist =
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
        private const string ThreadPath = @"https://projectpokemon.org/pkhex/";
        private const string VersionPath = @"https://raw.githubusercontent.com/kwsch/PKHeX/master/PKHeX.WinForms/Resources/text/version.txt";

        #endregion

        #region //// MAIN MENU FUNCTIONS ////
        private void FormLoadInitialSettings(string[] args, out bool showChangelog, out bool BAKprompt)
        {
            showChangelog = false;
            BAKprompt = false;

            // Set up Language Selection
            foreach (var cbItem in main_langlist)
                CB_MainLanguage.Items.Add(cbItem);
            C_SAV.HaX = PKME_Tabs.HaX = HaX = args.Any(x => string.Equals(x.Trim('-'), nameof(HaX), StringComparison.CurrentCultureIgnoreCase));
            PB_Legal.Visible = !HaX;

            int languageID = 1; // English
            try
            {
                ConfigUtil.CheckConfig();
                FormLoadConfig(out BAKprompt, out showChangelog, out languageID);
            }
            catch (ConfigurationErrorsException e)
            {
                // Delete the settings if they exist
                var settingsFilename = (e.InnerException as ConfigurationErrorsException)?.Filename;
                if (!string.IsNullOrEmpty(settingsFilename) && File.Exists(settingsFilename))
                    DeleteConfig(settingsFilename);
                else
                    WinFormsUtil.Error("Unable to load settings.", e);
            }
            CB_MainLanguage.SelectedIndex = languageID;

            PKME_Tabs.InitializeFields();
            PKME_Tabs.TemplateFields(LoadTemplate(C_SAV.SAV));
        }
        private void FormLoadAddEvents()
        {
            C_SAV.PKME_Tabs = PKME_Tabs;
            C_SAV.Menu_Redo = Menu_Redo;
            C_SAV.Menu_Undo = Menu_Undo;
            dragout.GiveFeedback += (sender, e) => e.UseDefaultCursors = false;
            GiveFeedback += (sender, e) => e.UseDefaultCursors = false;
            PKME_Tabs.EnableDragDrop(Main_DragEnter, Main_DragDrop);
            C_SAV.EnableDragDrop(Main_DragEnter, Main_DragDrop);

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
            mnu.RequestEditorLegality += ClickLegality;
            mnu.RequestEditorQR += ClickQR;
            mnu.RequestEditorSaveAs += MainMenuSave;
            dragout.ContextMenuStrip = mnu.mnuL;
            C_SAV.menu.RequestEditorLegality += ShowLegality;
        }
        private void FormLoadInitialFiles(string[] args)
        {
            string pkmArg = null;
            foreach (string arg in args.Skip(1)) // skip .exe
            {
                var fi = new FileInfo(arg);
                if (!fi.Exists)
                    continue;

                if (PKX.IsPKM(fi.Length))
                    pkmArg = arg;
                else
                    OpenQuick(arg, force: true);
            }
            if (!C_SAV.SAV.Exportable) // No SAV loaded from exe args
            {
                try
                {
                    if (!DetectSaveFile(out string path) && path != null)
                        WinFormsUtil.Error(path); // `path` contains the error message

                    if (path != null && File.Exists(path))
                        OpenQuick(path, force: true);
                    else
                    {
                        OpenSAV(C_SAV.SAV, null);
                        C_SAV.SAV.Edited = false; // Prevents form close warning from showing until changes are made
                    }
                }
                catch (Exception ex)
                {
                    ErrorWindow.ShowErrorDialog("An error occurred while attempting to auto-load your save file.", ex, true);
                }
            }
            if (pkmArg != null)
                OpenQuick(pkmArg, force: true);
            else
                GetPreview(dragout);
        }
        private void FormLoadCheckForUpdates()
        {
            L_UpdateAvailable.Click += (sender, e) => Process.Start(ThreadPath);
            new Task(() =>
            {
                string data = NetUtil.GetStringFromURL(VersionPath);
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

                    DateTime GetDate(string str) => DateTime.ParseExact(str, "yyyyMMdd", CultureInfo.InvariantCulture,
                        DateTimeStyles.None);

                    void ToggleUpdateMessage()
                    {
                        L_UpdateAvailable.Visible = true;
                        L_UpdateAvailable.Text = message;
                    }
                }
                catch { }
            }).Start();
        }
        private void FormLoadConfig(out bool BAKprompt, out bool showChangelog, out int languageID)
        {
            BAKprompt = false;
            showChangelog = false;
            languageID = 1;

            var Settings = Properties.Settings.Default;
            Settings.Upgrade();

            PKME_Tabs.Unicode = Unicode = Menu_Unicode.Checked = Settings.Unicode;
            PKME_Tabs.UpdateUnicode(GenderSymbols);
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
        private static void DeleteConfig(string settingsFilename)
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
        private void MainMenuOpen(object sender, EventArgs e)
        {
            if (WinFormsUtil.OpenSAVPKMDialog(C_SAV.SAV.PKMExtensions, out string path))
                OpenQuick(path);
        }
        private void MainMenuSave(object sender, EventArgs e)
        {
            if (!PKME_Tabs.VerifiedPKM()) return;
            PKM pk = PreparePKM();
            WinFormsUtil.SavePKMDialog(pk);
        }
        private void MainMenuExit(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control) // triggered via hotkey
                if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Quit PKHeX?"))
                    return;
                 
            Close();
        }
        private void MainMenuAbout(object sender, EventArgs e) => new About().ShowDialog();

        // Sub Menu Options
        private void MainMenuBoxReport(object sender, EventArgs e)
        {
            if (this.FirstFormOfType<ReportGrid>() is ReportGrid z)
            { z.CenterToForm(this); z.BringToFront(); return; }
            
            ReportGrid report = new ReportGrid();
            report.Show();
            report.PopulateData(C_SAV.SAV.BoxData);
        }
        private void MainMenuDatabase(object sender, EventArgs e)
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
        private void MainMenuMysteryDB(object sender, EventArgs e)
        {
            if (this.FirstFormOfType<SAV_MysteryGiftDB>() is SAV_MysteryGiftDB z)
            { z.CenterToForm(this); z.BringToFront(); return; }

            new SAV_MysteryGiftDB(PKME_Tabs, C_SAV).Show();
        }
        private void MainMenuUnicode(object sender, EventArgs e)
        {
            Settings.Default.Unicode = PKME_Tabs.Unicode = Unicode = Menu_Unicode.Checked;
            PKME_Tabs.UpdateUnicode(GenderSymbols);
        }
        private void MainMenuModifyDex(object sender, EventArgs e) => Settings.Default.SetUpdateDex = SaveFile.SetUpdateDex = Menu_ModifyDex.Checked;
        private void MainMenuModifyUnset(object sender, EventArgs e) => Settings.Default.ModifyUnset = Menu_ModifyUnset.Checked;
        private void MainMenuModifyPKM(object sender, EventArgs e) => Settings.Default.SetUpdatePKM = SaveFile.SetUpdatePKM = Menu_ModifyPKM.Checked;
        private void MainMenuFlagIllegal(object sender, EventArgs e) => C_SAV.FlagIllegal = Settings.Default.FlagIllegal = Menu_FlagIllegal.Checked;

        private void MainMenuBoxLoad(object sender, EventArgs e)
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
        private void MainMenuBoxDump(object sender, EventArgs e)
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
        private void MainMenuBoxDumpSingle(object sender, EventArgs e)
        {
            if (C_SAV.DumpBox(out string result))
                WinFormsUtil.Alert(result);
        }
        private void MainMenuBatchEditor(object sender, EventArgs e)
        {
            new BatchEditor(PKME_Tabs.PreparePKM(), C_SAV.SAV).ShowDialog();
            C_SAV.SetPKMBoxes(); // refresh
            C_SAV.UpdateBoxViewers();
        }
        private void MainMenuFolder(object sender, EventArgs e) => new SAV_FolderList().ShowDialog();
        // Misc Options
        private void ClickShowdownImportPKM(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsText())
            { WinFormsUtil.Alert("Clipboard does not contain text."); return; }

            // Get Simulator Data
            ShowdownSet Set = new ShowdownSet(Clipboard.GetText());

            if (Set.Species < 0)
            { WinFormsUtil.Alert("Set data not found in clipboard."); return; }

            if (Set.Nickname?.Length > C_SAV.SAV.NickLength)
                Set.Nickname = Set.Nickname.Substring(0, C_SAV.SAV.NickLength);

            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Import this set?", Set.Text))
                return;

            if (Set.InvalidLines.Any())
                WinFormsUtil.Alert("Invalid lines detected:", string.Join(Environment.NewLine, Set.InvalidLines));

            // Set Species & Nickname
            PKME_Tabs.LoadShowdownSet(Set);
        }
        private void ClickShowdownExportPKM(object sender, EventArgs e)
        {
            if (!PKME_Tabs.VerifiedPKM())
            {
                WinFormsUtil.Alert("Fix data before exporting.");
                return;
            }

            var text = PreparePKM().ShowdownText;
            Clipboard.SetText(text);
            var clip = Clipboard.GetText();
            if (clip != text)
                WinFormsUtil.Alert("Unable to set to Clipboard.", "Try exporting again.");
            else
                WinFormsUtil.Alert("Exported Showdown Set to Clipboard:", text);
        }
        private void ClickShowdownExportParty(object sender, EventArgs e)
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
        private void ClickShowdownExportBattleBox(object sender, EventArgs e)
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
            if (!fi.Exists)
                return;
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
            if (OpenXOR(input, path)) // Check if we can load the save via xorpad
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

            OpenSAV(SaveUtil.GetVariantSAV(psdata), path);
            return true;
        }
        private bool TryLoadSAV(byte[] input, string path)
        {
            var sav = SaveUtil.GetVariantSAV(input);
            if (sav == null)
                return false;
            OpenSAV(sav, path);
            return true;
        }
        private bool TryLoadMemoryCard(byte[] input, string path)
        {
            if (!SAV3GCMemoryCard.IsMemoryCardSize(input))
                return false;
            SAV3GCMemoryCard MC = CheckGCMemoryCard(input, path);
            if (MC == null)
                return false;
            var sav = SaveUtil.GetVariantSAV(MC);
            if (sav == null)
                return false;
            OpenSAV(sav, path);
            return true;
        }
        private bool TryLoadPKM(byte[] input, string path, string ext, SaveFile SAV)
        {
            var temp = PKMConverter.GetPKMfromBytes(input, prefer: ext.Length > 0 ? (ext.Last() - 0x30) & 7 : C_SAV.SAV.Generation);
            if (temp == null)
                return false;

            var type = PKME_Tabs.CurrentPKM.GetType();
            PKM pk = PKMConverter.ConvertToType(temp, type, out string c);
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
            
            PKME_Tabs.PopulateFields(pk);
            Debug.WriteLine(c);
            return true;
        }
        private bool TryLoadPCBoxBin(byte[] input)
        {
            if (!C_SAV.IsPCBoxBin(input.Length))
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
            if (!BattleVideo.IsValid(input))
                return false;

            BattleVideo b = BattleVideo.GetVariantBattleVideo(input);
            bool result = C_SAV.OpenBattleVideo(b, out string c);
            WinFormsUtil.Alert(c);
            Debug.WriteLine(c);
            return result;
        }
        private bool TryLoadMysteryGift(byte[] input, string path, string ext)
        {
            var tg = MysteryGift.GetMysteryGift(input, ext);
            if (tg == null)
                return false;
            if (!tg.IsPokémon)
            {
                WinFormsUtil.Alert("Mystery Gift is not a Pokémon.", path);
                return true;
            }

            var temp = tg.ConvertToPKM(C_SAV.SAV);
            PKM pk = PKMConverter.ConvertToType(temp, C_SAV.SAV.PKMType, out string c);

            if (pk == null)
            {
                WinFormsUtil.Alert("Conversion failed.", c);
                return true;
            }

            PKME_Tabs.PopulateFields(pk);
            Debug.WriteLine(c);
            return true;
        }

        private bool OpenXOR(byte[] input, string path)
        {
            // try to get a save file via xorpad in same folder
            var folder = new DirectoryInfo(path).Parent.FullName;
            string[] pads = Directory.GetFiles(folder);
            var s = SaveUtil.GetSAVfromXORpads(input, pads);

            if (s == null) // failed to find xorpad in path folder
            {
                // try again
                pads = Directory.GetFiles(WorkingDirectory);
                s = SaveUtil.GetSAVfromXORpads(input, pads);
            }

            if (s == null)
                return false; // failed

            OpenSAV(s, s.FileName);
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
            Legal.EReaderBerryIsEnigma = sav.IsEBerryIsEnigma;
            Legal.EReaderBerryName = sav.EBerryName;
            Legal.Savegame_Gender = sav.Gender;
            Legal.Savegame_TID = sav.TID;
            Legal.Savegame_SID = sav.SID;
            Legal.Savegame_OT = sav.OT;
            Legal.Savegame_Version = sav.Version;
        }
        private static PKM LoadTemplate(SaveFile sav)
        {
            if (!Directory.Exists(TemplatePath))
                return null;

            var blank = sav.BlankPKM;
            string path = Path.Combine(TemplatePath, new DirectoryInfo(TemplatePath).Name + "." + blank.Extension);

            if (!File.Exists(path) || !PKX.IsPKM(new FileInfo(path).Length))
                return null;

            var pk = PKMConverter.GetPKMfromBytes(File.ReadAllBytes(path), prefer: blank.Format);
            return PKMConverter.ConvertToType(pk, sav.BlankPKM.GetType(), out path); // no sneaky plz; reuse string
        }
        private static void RefreshMGDB()
        {
            Legal.RefreshMGDB(MGDatabasePath);
        }

        private void OpenSAV(SaveFile sav, string path)
        {
            if (sav == null || sav.Version == GameVersion.Invalid)
            { WinFormsUtil.Error("Invalid save file loaded. Aborting.", path); return; }

            if (!SanityCheckSAV(ref sav))
                return;
            StoreLegalSaveGameData(sav);
            PKMUtil.Initialize(sav); // refresh sprite generator

            // clean fields
            C_SAV.M.Reset();
            Menu_ExportSAV.Enabled = sav.Exportable;

            // No changes made yet
            Menu_Undo.Enabled = false;
            Menu_Redo.Enabled = false;

            ResetSAVPKMEditors(sav);

            Text = GetProgramTitle(sav, path);
            TryBackupExportCheck(sav, path);

            PKMConverter.UpdateConfig(sav.SubRegion, sav.Country, sav.ConsoleRegion, sav.OT, sav.Gender, sav.Language);
            SystemSounds.Beep.Play();
        }
        private void ResetSAVPKMEditors(SaveFile sav)
        {
            bool WindowToggleRequired = C_SAV.SAV.Generation < 3 && sav.Generation >= 3; // version combobox refresh hack
            PKM pk = PreparePKM();
            var blank = sav.BlankPKM;
            PKME_Tabs.CurrentPKM = blank;
            PKME_Tabs.SetPKMFormatMode(sav.Generation);
            PKME_Tabs.PopulateFields(blank);
            C_SAV.SAV = sav;

            // Initialize Overall Info
            Menu_LoadBoxes.Enabled = Menu_DumpBoxes.Enabled = Menu_Report.Enabled = Menu_Modify.Enabled = C_SAV.SAV.HasBox;

            // Initialize Subviews
            bool WindowTranslationRequired = false;
            WindowTranslationRequired |= PKME_Tabs.ToggleInterface(sav, pk);
            WindowTranslationRequired |= C_SAV.ToggleInterface();
            if (WindowTranslationRequired) // force update -- re-added controls may be untranslated
                WinFormsUtil.TranslateInterface(this, CurrentLanguage);

            if (WindowToggleRequired) // Version combobox selectedvalue needs a little help, only updates once it is visible
                PKME_Tabs.FlickerInterface();

            PKME_Tabs.TemplateFields(LoadTemplate(sav));
            sav.Edited = false;
        }
        private static string GetProgramTitle(SaveFile sav, string path)
        {
#if DEBUG
            var d = File.GetLastWriteTime(System.Reflection.Assembly.GetEntryAssembly().Location);
            string date = "d-" + d.ToString("yyyyMMdd");
#else
            string date = Resources.ProgramVersion;
#endif
            string title = $"PKH{(HaX ? "a" : "e")}X ({date}) - " + $"{sav.GetType().Name}: ";
            if (string.IsNullOrWhiteSpace(path)) // Blank save file
            {
                sav.FilePath = null;
                sav.FileName = "Blank Save File";
                return title + $"{sav.FileName} [{sav.OT} ({sav.Version})]";
            }

            sav.FilePath = Path.GetDirectoryName(path);
            sav.FileName = Path.GetExtension(path) == ".bak"
                ? Path.GetFileName(path).Split(new[] {" ["}, StringSplitOptions.None)[0]
                : Path.GetFileName(path);
            return title + $"{Path.GetFileNameWithoutExtension(Util.CleanFileName(sav.BAKName))}"; // more descriptive
        }
        private static bool TryBackupExportCheck(SaveFile sav, string path)
        {
            if (string.IsNullOrWhiteSpace(path)) // not actual save
                return false;

            // If backup folder exists, save a backup.
            string backupName = Path.Combine(BackupPath, Util.CleanFileName(sav.BAKName));
            if (sav.Exportable && Directory.Exists(BackupPath) && !File.Exists(backupName))
                File.WriteAllBytes(backupName, sav.BAK);

            // Check location write protection
            bool locked = true;
            try { locked = File.GetAttributes(path).HasFlag(FileAttributes.ReadOnly); }
            catch { }

            if (!locked)
                return true;

            WinFormsUtil.Alert("File's location is write protected:\n" + path,
                "If the path is a removable disk (SD card), please ensure the write protection switch is not set.");
            return false;
        }
        private static bool SanityCheckSAV(ref SaveFile sav)
        {
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
                    if (drTradeback == DialogResult.Cancel)
                        return false;
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

                switch (dialog.Result)
                {
                    case GameVersion.FR:
                        sav.Personal = PersonalTable.FR;
                        break;
                    case GameVersion.LG:
                        sav.Personal = PersonalTable.LG;
                        break;
                    default:
                        return false;
                }
            }

            return true;
        }

        public static void SetCountrySubRegion(ComboBox CB, string type)
        {
            int index = CB.SelectedIndex;
            // fix for Korean / Chinese being swapped
            string cl = GameInfo.CurrentLanguage + "";
            cl = cl == "zh" ? "ko" : cl == "ko" ? "zh" : cl;

            CB.DataSource = Util.GetCBList(type, cl);

            if (index > 0 && index < CB.Items.Count)
                CB.SelectedIndex = index;
        }

        // Language Translation
        private void ChangeMainLanguage(object sender, EventArgs e)
        {
            if (CB_MainLanguage.SelectedIndex < 8)
                CurrentLanguage = GameInfo.Language2Char((uint)CB_MainLanguage.SelectedIndex);

            // Set the culture (makes it easy to pass language to other forms)
            Settings.Default.Language = CurrentLanguage;
            Thread.CurrentThread.CurrentCulture = new CultureInfo(CurrentLanguage.Substring(0, 2));
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            Menu_Options.DropDown.Close();

            PKM pk = C_SAV.SAV.GetPKM(PKME_Tabs.CurrentPKM.Data);
            InitializeStrings();
            PKME_Tabs.ChangeLanguage(C_SAV.SAV, pk);
            string ProgramTitle = Text;
            WinFormsUtil.TranslateInterface(this, CurrentLanguage); // Translate the UI to language.
            Text = ProgramTitle;
        }
        private static void InitializeStrings()
        {            
            string l = CurrentLanguage;
            GameInfo.Strings = GameInfo.GetStrings(l);

            // Update Legality Strings
            // Clipboard.SetText(string.Join(Environment.NewLine, Util.GetLocalization(typeof(LegalityCheckStrings))));
            Task.Run(() => {
                    var lang = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Substring(0, 2);
                    Util.SetLocalization(typeof(LegalityCheckStrings), lang);
                    RibbonStrings.ResetDictionary(GameInfo.Strings.ribbons);
                });

            // Update Legality Analysis strings
            LegalityAnalysis.MoveStrings = GameInfo.Strings.movelist;
            LegalityAnalysis.SpeciesStrings = GameInfo.Strings.specieslist;
        }
        #endregion

        #region //// PKX WINDOW FUNCTIONS ////
        private bool QR6Notified;
        private void ClickQR(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Alt)
            {
                string url = Clipboard.GetText();
                if (!string.IsNullOrWhiteSpace(url))
                {
                    if (!url.StartsWith("http") || url.Contains('\n'))
                        ClickShowdownImportPKM(sender, e);
                    else
                        ImportQRToTabs(url);
                    return;
                }
            }
            ExportQRFromTabs();
        }

        private void ImportQRToTabs(string url)
        {
            // Fetch data from QR code...
            byte[] ekx = QR.GetQRData(url);
            if (ekx == null)
                return;

            PKM pk = PKMConverter.GetPKMfromBytes(ekx, prefer: C_SAV.SAV.Generation);
            if (pk == null)
            {
                WinFormsUtil.Alert("Decoded data not a valid PKM.", $"QR Data Size: {ekx.Length}");
                return;
            }
            if (!pk.Valid || pk.Species <= 0)
            {
                WinFormsUtil.Alert("Invalid data detected.");
                return;
            }
            PKM pkz = PKMConverter.ConvertToType(pk, C_SAV.SAV.PKMType, out string c);
            if (pkz == null)
            {
                WinFormsUtil.Alert(c);
                return;
            }
            PKME_Tabs.PopulateFields(pkz);
        }
        private void ExportQRFromTabs()
        {
            if (!PKME_Tabs.VerifiedPKM())
                return;
            PKM pkx = PreparePKM();

            Image qr;
            switch (pkx.Format)
            {
                case 7:
                    qr = QR.GenerateQRCode7((PK7) pkx);
                    break;
                default:
                    if (pkx.Format == 6 && !QR6Notified) // hint that the user should not be using QR6 injection
                    {
                        WinFormsUtil.Alert("QR codes are deprecated in favor of other methods.",
                            "Consider utilizing homebrew or on-the-fly RAM editing custom firmware (PKMN-NTR).");
                        QR6Notified = true;
                    }
                    qr = QR.GetQRImage(pkx.EncryptedBoxData, QR.GetQRServer(pkx.Format));
                    break;
            }

            if (qr == null)
                return;

            var sprite = dragout.Image;
            var la = new LegalityAnalysis(pkx);
            if (la.Parsed && pkx.Species != 0)
            {
                var img = la.Valid ? Resources.valid : Resources.warn;
                sprite = ImageUtil.LayerImage(sprite, img, 24, 0, 1);
            }

            string[] r = pkx.QRText;
            string refer = $"PKHeX ({Resources.ProgramVersion})";
            new QR(qr, sprite, pkx, r[0], r[1], r[2], $"{refer} ({pkx.GetType().Name})").ShowDialog();
        }

        private void ClickLegality(object sender, EventArgs e)
        {
            if (!PKME_Tabs.VerifiedPKM())
            { SystemSounds.Asterisk.Play(); return; }

            var pk = PreparePKM();

            if (pk.Species == 0 || !pk.ChecksumValid)
            { SystemSounds.Asterisk.Play(); return; }

            ShowLegality(sender, e, pk);
        }
        private void ShowLegality(object sender, EventArgs e, PKM pk)
        {
            LegalityAnalysis la = new LegalityAnalysis(pk);
            if (pk.Slot < 0)
                PKME_Tabs.UpdateLegality(la);
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
        private void ClickClone(object sender, EventArgs e)
        {
            if (!PKME_Tabs.VerifiedPKM()) return; // don't copy garbage to the box
            PKM pk = PKME_Tabs.PreparePKM();
            C_SAV.SetClonesToBox(pk);
        }
        private void GetPreview(PictureBox pb, PKM pk = null)
        {
            if (!IsInitialized)
                return;
            pk = pk ?? PreparePKM(false); // don't perform control loss click

            if (pb == dragout) dragout.ContextMenuStrip.Enabled = pk.Species != 0 || HaX; // Species

            pb.Image = pk.Sprite(C_SAV.SAV, -1, -1, true);
            if (pb.BackColor == Color.Red)
                pb.BackColor = Color.Transparent;
        }
        private void PKME_Tabs_UpdatePreviewSprite(object sender, EventArgs e) => GetPreview(dragout);
        private void PKME_Tabs_LegalityChanged(object sender, EventArgs e)
        {
            if (sender == null || HaX)
            {
                PB_Legal.Visible = false;
                return;
            }

            PB_Legal.Visible = true;
            PB_Legal.Image = sender as bool? == false ? Resources.warn : Resources.valid;
        }
        private void PKME_Tabs_RequestShowdownExport(object sender, EventArgs e) => ClickShowdownExportPKM(sender, e);
        private void PKME_Tabs_RequestShowdownImport(object sender, EventArgs e) => ClickShowdownImportPKM(sender, e);
        private SaveFile PKME_Tabs_SaveFileRequested(object sender, EventArgs e) => C_SAV.SAV;
        // Open/Save Array Manipulation //
        private PKM PreparePKM(bool click = true) => PKME_Tabs.PreparePKM(click);

        // Drag & Drop Events
        private static void Main_DragEnter(object sender, DragEventArgs e)
        {
            if (e.AllowedEffect == (DragDropEffects.Copy | DragDropEffects.Link)) // external file
                e.Effect = DragDropEffects.Copy;
            else if (e.Data != null) // within
                e.Effect = DragDropEffects.Move;
        }
        private void Main_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files == null || files.Length == 0)
                return;
            OpenQuick(files[0]);
            e.Effect = DragDropEffects.Copy;

            Cursor = DefaultCursor;
        }
        // Decrypted Export
        private void Dragout_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && (ModifierKeys == Keys.Alt || ModifierKeys == Keys.Shift))
                ClickQR(sender, e);
            if (e.Button == MouseButtons.Right)
                return;
            if (!PKME_Tabs.VerifiedPKM())
                return;

            // Create Temp File to Drag
            PKM pkx = PreparePKM();
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
        private static void Dragout_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
        // Dragout Display
        private void DragoutEnter(object sender, EventArgs e)
        {
            dragout.BackgroundImage = WinFormsUtil.GetIndex(PKME_Tabs.CB_Species) > 0 ? Resources.slotSet : Resources.slotDel;
            Cursor = Cursors.Hand;
        }
        private void DragoutLeave(object sender, EventArgs e)
        {
            dragout.BackgroundImage = Resources.slotTrans;
            if (Cursor == Cursors.Hand)
                Cursor = Cursors.Default;
        }
        private void DragoutDrop(object sender, DragEventArgs e)
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
        private void ClickExportSAVBAK(object sender, EventArgs e)
        {
            if (C_SAV.ExportBackup() && !Directory.Exists(BackupPath))
                PromptBackup();
        }
        private void ClickExportSAV(object sender, EventArgs e)
        {
            if (!Menu_ExportSAV.Enabled)
                return;

            C_SAV.ExportSaveFile();
        }
        private void ClickSaveFileName(object sender, EventArgs e)
        {
            if (!DetectSaveFile(out string path))
                return;
            if (WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Open save file from the following location?", path) == DialogResult.Yes)
                OpenQuick(path); // load save
        }

        private static bool DetectSaveFile(out string path)
        {
            string cgse = "";
            string pathCache = CyberGadgetUtil.GetCacheFolder();
            if (Directory.Exists(pathCache))
                cgse = Path.Combine(pathCache);
            if (!PathUtilWindows.DetectSaveFile(out path, cgse))
                return false;

            return path != null && File.Exists(path);
        }

        private static void PromptBackup()
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

        private void ClickUndo(object sender, EventArgs e) => C_SAV.ClickUndo();
        private void ClickRedo(object sender, EventArgs e) => C_SAV.ClickRedo();
        #endregion
    }
}
