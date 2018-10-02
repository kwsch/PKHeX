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
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms
{
    public partial class Main : Form
    {
        public Main()
        {
            new Task(() => new SplashScreen().ShowDialog()).Start();
            new Task(() => Legal.RefreshMGDB(MGDatabasePath)).Start();
            InitializeComponent();

            FormLoadAddEvents();

            string[] args = Environment.GetCommandLineArgs();
            FormLoadInitialSettings(args, out bool showChangelog, out bool BAKprompt);
            FormLoadCustomBackupPaths();
            FormLoadInitialFiles(args);
            FormLoadCheckForUpdates();

            IsInitialized = true; // Splash Screen closes on its own.
            PKME_Tabs_UpdatePreviewSprite(null, null);
            BringToFront();
            WindowState = FormWindowState.Minimized;
            Show();
            WindowState = FormWindowState.Normal;
            if (HaX)
            {
                PKMConverter.AllowIncompatibleConversion = true;
                WinFormsUtil.Alert(MsgProgramIllegalModeActive, MsgProgramIllegalModeBehave);
            }
            else if (showChangelog)
            {
                new About().ShowDialog();
            }

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
            };

        private static readonly List<IPlugin> Plugins = new List<IPlugin>();
        #endregion

        #region Path Variables

        public static string WorkingDirectory => WinFormsUtil.IsClickonceDeployed ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PKHeX") : Application.StartupPath;
        public static string DatabasePath => Path.Combine(WorkingDirectory, "pkmdb");
        public static string MGDatabasePath => Path.Combine(WorkingDirectory, "mgdb");
        public static string BackupPath => Path.Combine(WorkingDirectory, "bak");
        public static string CryPath => Path.Combine(WorkingDirectory, "sounds");
        public static string SAVPaths => Path.Combine(WorkingDirectory, "savpaths.txt");
        private static string TemplatePath => Path.Combine(WorkingDirectory, "template");
        private static string PluginPath => Path.Combine(WorkingDirectory, "plugins");
        private const string ThreadPath = "https://projectpokemon.org/pkhex/";
        private const string VersionPath = "https://raw.githubusercontent.com/kwsch/PKHeX/master/PKHeX.WinForms/Resources/text/version.txt";

        #endregion

        #region //// MAIN MENU FUNCTIONS ////
        private void FormLoadInitialSettings(string[] args, out bool showChangelog, out bool BAKprompt)
        {
            showChangelog = false;
            BAKprompt = false;

            CB_MainLanguage.Items.AddRange(main_langlist);
            HaX = args.Any(x => string.Equals(x.Trim('-'), nameof(HaX), StringComparison.CurrentCultureIgnoreCase))
                || Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName).EndsWith(nameof(HaX));

            PKMConverter.AllowIncompatibleConversion = C_SAV.HaX = PKME_Tabs.HaX = HaX;
            PB_Legal.Visible = !HaX;

            try
            {
                ConfigUtil.CheckConfig();
                FormLoadConfig(out BAKprompt, out showChangelog);
            }
            catch (ConfigurationErrorsException e)
            {
                // Delete the settings if they exist
                var settingsFilename = (e.InnerException as ConfigurationErrorsException)?.Filename;
                if (!string.IsNullOrEmpty(settingsFilename) && File.Exists(settingsFilename))
                    DeleteConfig(settingsFilename);
                else
                    WinFormsUtil.Error(MsgSettingsLoadFail, e);
            }

            FormLoadPlugins();
            var exts = Path.Combine(WorkingDirectory, "savexts.txt");
            if (File.Exists(exts))
                WinFormsUtil.AddSaveFileExtensions(File.ReadLines(exts));

            PKME_Tabs.InitializeFields();
            PKME_Tabs.TemplateFields(LoadTemplate(C_SAV.SAV));

            #if DEBUG
            DevUtil.AddControl(Menu_Tools);
            #endif
        }

        private static void FormLoadCustomBackupPaths()
        {
            SaveDetection.CustomBackupPaths.Clear();
            try
            {
                string pathCache = CyberGadgetUtil.GetCacheFolder();
                if (Directory.Exists(pathCache))
                    SaveDetection.CustomBackupPaths.Add(Path.Combine(pathCache));
                string pathTemp = CyberGadgetUtil.GetTempFolder();
                if (Directory.Exists(pathTemp))
                    SaveDetection.CustomBackupPaths.Add(Path.Combine(pathTemp));
                if (File.Exists(SAVPaths))
                    SaveDetection.CustomBackupPaths.AddRange(File.ReadAllLines(SAVPaths).Where(Directory.Exists));
            }
            catch { }
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
                    {
                        OpenQuick(path, force: true);
                    }
                    else
                    {
                        OpenSAV(C_SAV.SAV, null);
                        C_SAV.SAV.Edited = false; // Prevents form close warning from showing until changes are made
                    }
                }
                catch (Exception ex)
                {
                    ErrorWindow.ShowErrorDialog(MsgFileLoadFailAuto, ex, true);
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
                if (int.TryParse(data, out var upd) && int.TryParse(Resources.ProgramVersion, out var cur) && upd <= cur)
                    return;

                Invoke((MethodInvoker)(() =>
                {
                    L_UpdateAvailable.Visible = true;
                    L_UpdateAvailable.Text = $"{MsgProgramUpdateAvailable} {upd:d}";
                }));
            }).Start();
        }

        private void FormLoadConfig(out bool BAKprompt, out bool showChangelog)
        {
            BAKprompt = false;
            showChangelog = false;

            var Settings = Properties.Settings.Default;
            Settings.Upgrade();

            ReloadProgramSettings(Settings);

            // Select Language
            string l = Settings.Language;
            int lang = GameInfo.Language(l);
            if (lang < 0)
                lang = GameInfo.Language();
            CB_MainLanguage.SelectedIndex = lang >= 0 ? lang : 1; // english

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

        private void FormLoadPlugins()
        {
            #if !MERGED // merged should load dlls from within too, folder is no longer required
            if (!Directory.Exists(PluginPath))
                return;
            #endif
            Plugins.AddRange(PluginLoader.LoadPlugins<IPlugin>(PluginPath));
            foreach (var p in Plugins.OrderBy(z => z.Priority))
                p.Initialize(C_SAV, PKME_Tabs, menuStrip1);
        }

        private static void DeleteConfig(string settingsFilename)
        {
            var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgSettingsResetCorrupt, MsgSettingsResetPrompt);
            if (dr == DialogResult.Yes)
            {
                File.Delete(settingsFilename);
                WinFormsUtil.Alert(MsgSettingsResetSuccess, MsgProgramRestart);
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
            {
                if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Quit PKHeX?"))
                    return;
            }

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
                {
                    new KChart(C_SAV.SAV).Show();
                }

                return;
            }

            if (this.FirstFormOfType<SAV_Database>() is SAV_Database z)
            { z.CenterToForm(this); z.BringToFront(); return; }

            if (Directory.Exists(DatabasePath))
                new SAV_Database(PKME_Tabs, C_SAV).Show();
            else
                WinFormsUtil.Alert(MsgDatabase, string.Format(MsgDatabaseAdvice, DatabasePath));
        }

        private void MainMenuMysteryDB(object sender, EventArgs e)
        {
            if (this.FirstFormOfType<SAV_MysteryGiftDB>() is SAV_MysteryGiftDB z)
            { z.CenterToForm(this); z.BringToFront(); return; }

            new SAV_MysteryGiftDB(PKME_Tabs, C_SAV).Show();
        }

        private void MainMenuSettings(object sender, EventArgs e)
        {
            new SettingsEditor(Settings.Default, nameof(Settings.Default.BAKPrompt)).ShowDialog();

            // Reload text (if OT details hidden)
            Text = GetProgramTitle(C_SAV.SAV);
            // Update final settings
            ReloadProgramSettings(Settings.Default);

            PKME_Tabs_UpdatePreviewSprite(sender, e);
            if (C_SAV.SAV.HasBox)
                C_SAV.ReloadSlots();
        }

        private void ReloadProgramSettings(Settings settings)
        {
            PKME_Tabs.Unicode = Unicode = settings.Unicode;
            PKME_Tabs.UpdateUnicode(GenderSymbols);
            PKX.AllowShinySprite = settings.ShinySprites;
            SaveFile.SetUpdateDex = settings.SetUpdateDex;
            SaveFile.SetUpdatePKM = C_SAV.ModifyPKM = PKME_Tabs.ModifyPKM = settings.SetUpdatePKM;
            CommonEdits.ShowdownSetIVMarkings = settings.ApplyMarkings;
            C_SAV.FlagIllegal = settings.FlagIllegal;
            C_SAV.M.GlowHover = settings.HoverSlotGlowEdges;
            SpriteBuilder.ShowEggSpriteAsItem = settings.ShowEggSpriteAsHeldItem;
            PKME_Tabs.HideSecretValues = C_SAV.HideSecretDetails = settings.HideSecretDetails;
        }

        private void MainMenuBoxLoad(object sender, EventArgs e)
        {
            string path = null;
            if (Directory.Exists(DatabasePath))
            {
                var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgDatabaseLoad);
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
            DialogResult ld = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgDatabaseExport);
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

        private void MainMenuFolder(object sender, EventArgs e)
        {
            var ofType = Application.OpenForms.OfType<SAV_FolderList>().FirstOrDefault();
            if (ofType != null)
            {
                ofType.CenterToForm(this);
                ofType.BringToFront();
            }
            else
            {
                new SAV_FolderList(s => OpenSAV(SaveUtil.GetVariantSAV(s.FilePath), s.FilePath)).Show();
            }
        }

        // Misc Options
        private void ClickShowdownImportPKM(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsText())
            { WinFormsUtil.Alert(MsgClipboardFailRead); return; }

            // Get Simulator Data
            ShowdownSet Set = new ShowdownSet(Clipboard.GetText());

            if (Set.Species < 0)
            { WinFormsUtil.Alert(MsgSimulatorFailClipboard); return; }

            if (Set.Nickname?.Length > C_SAV.SAV.NickLength)
                Set.Nickname = Set.Nickname.Substring(0, C_SAV.SAV.NickLength);

            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgSimulatorLoad, Set.Text))
                return;

            if (Set.InvalidLines.Count > 0)
                WinFormsUtil.Alert(MsgSimulatorInvalid, string.Join(Environment.NewLine, Set.InvalidLines));

            // Set Species & Nickname
            PKME_Tabs.LoadShowdownSet(Set);
        }

        private void ClickShowdownExportPKM(object sender, EventArgs e)
        {
            if (!PKME_Tabs.VerifiedPKM())
            {
                WinFormsUtil.Alert(MsgSimulatorExportBadFields);
                return;
            }

            var pk = PreparePKM();
            var text = ShowdownSet.GetShowdownText(pk);
            Clipboard.SetText(text);
            var clip = Clipboard.GetText();
            if (clip != text)
                WinFormsUtil.Alert(MsgClipboardFailWrite, MsgSimulatorExportFail);
            else
                WinFormsUtil.Alert(MsgSimulatorExportSuccess, text);
        }

        private void ClickShowdownExportParty(object sender, EventArgs e) => C_SAV.ClickShowdownExportParty(sender, e);
        private void ClickShowdownExportBattleBox(object sender, EventArgs e) => C_SAV.ClickShowdownExportBattleBox(sender, e);
        private void ClickShowdownExportCurrentBox(object sender, EventArgs e) => C_SAV.ClickShowdownExportCurrentBox(sender, e);

        // Main Menu Subfunctions
        private void OpenQuick(string path, bool force = false)
        {
            if (!(CanFocus || force))
            {
                SystemSounds.Asterisk.Play();
                return;
            }
            if (Plugins.Any(p => p.TryLoadFile(path)))
                return; // handled by plugin

            // detect if it is a folder (load into boxes or not)
            if (Directory.Exists(path))
            { C_SAV.LoadBoxes(out string _, path); return; }

            var fi = new FileInfo(path);
            if (!fi.Exists)
                return;

            string ext = Path.GetExtension(path);
            if (FileUtil.IsFileTooBig(fi.Length))
            {
                WinFormsUtil.Error(MsgFileSizeLarge + Environment.NewLine + string.Format(MsgFileSize, fi.Length), path);
                return;
            }
            if (FileUtil.IsFileTooSmall(fi.Length))
            {
                WinFormsUtil.Error(MsgFileSizeSmall + Environment.NewLine + string.Format(MsgFileSize, fi.Length), path);
                return;
            }
            byte[] input; try { input = File.ReadAllBytes(path); }
            catch (Exception e) { WinFormsUtil.Error(MsgFileInUse + path, e); return; }

            #if DEBUG
                OpenFile(input, path, ext);
            #else
                try { OpenFile(input, path, ext); }
                catch (Exception e) { WinFormsUtil.Error(MsgFileLoadFail + "\nPath: " + path, e); }
            #endif
        }

        private void OpenFile(byte[] input, string path, string ext)
        {
            var obj = FileUtil.GetSupportedFile(input, ext, C_SAV.SAV);
            if (obj != null && LoadFile(obj, path))
                return;

            WinFormsUtil.Error(MsgFileUnsupported,
                $"{MsgFileLoad}{Environment.NewLine}{path}",
                $"{string.Format(MsgFileSize, input.Length)}{Environment.NewLine}{input.Length} bytes (0x{input.Length:X4})");
        }

        private bool LoadFile(object input, string path)
        {
            if (input == null)
                return false;

            switch (input)
            {
                case PKM pk: return OpenPKM(pk);
                case SaveFile s: return OpenSAV(s, path);
                case BattleVideo b: return OpenBattleVideo(b);
                case MysteryGift g: return OpenMysteryGift(g, path);
                case IEnumerable<byte[]> pkms: return OpenPCBoxBin(pkms);

                case SAV3GCMemoryCard gc:
                    if (!CheckGCMemoryCard(gc, path))
                        return true;
                    var mcsav = SaveUtil.GetVariantSAV(gc);
                    return OpenSAV(mcsav, path);
            }
            return false;
        }

        private bool OpenPKM(PKM pk)
        {
            pk = PKMConverter.ConvertToType(pk, C_SAV.SAV.PKMType, out string c);
            Debug.WriteLine(c);
            PKME_Tabs.PopulateFields(pk);
            return true;
        }

        private bool OpenBattleVideo(BattleVideo b)
        {
            bool result = C_SAV.OpenBattleVideo(b, out string c);
            WinFormsUtil.Alert(c);
            Debug.WriteLine(c);
            return result;
        }

        private bool OpenMysteryGift(MysteryGift tg, string path)
        {
            if (!tg.IsPokémon)
            {
                WinFormsUtil.Alert(MsgPKMMysteryGiftFail, path);
                return true;
            }

            var temp = tg.ConvertToPKM(C_SAV.SAV);
            PKM pk = PKMConverter.ConvertToType(temp, C_SAV.SAV.PKMType, out string c);

            if (pk == null)
            {
                WinFormsUtil.Alert(MsgPKMConvertFail, c);
                return true;
            }

            PKME_Tabs.PopulateFields(pk);
            Debug.WriteLine(c);
            return true;
        }

        private bool OpenPCBoxBin(IEnumerable<byte[]> pkms)
        {
            if (!C_SAV.OpenPCBoxBin(pkms.SelectMany(z => z).ToArray(), out string c))
            {
                WinFormsUtil.Alert(MsgFileLoadIncompatible, c);
                return true;
            }

            WinFormsUtil.Alert(c);
            return true;
        }

        private static GameVersion SelectMemoryCardSaveGame(SAV3GCMemoryCard MC)
        {
            if (MC.SaveGameCount == 1)
                return MC.SelectedGameVersion;

            var games = new List<ComboItem>();
            if (MC.HasCOLO) games.Add(new ComboItem { Text = MsgGameColosseum, Value = (int)GameVersion.COLO });
            if (MC.HasXD) games.Add(new ComboItem { Text = MsgGameXD, Value = (int)GameVersion.XD });
            if (MC.HasRSBOX) games.Add(new ComboItem { Text = MsgGameRSBOX, Value = (int)GameVersion.RSBOX });

            WinFormsUtil.Alert(MsgFileLoadSaveMultiple, MsgFileLoadSaveSelectGame);
            var dialog = new SAV_GameSelect(games);
            dialog.ShowDialog();
            return dialog.Result;
        }

        private static bool CheckGCMemoryCard(SAV3GCMemoryCard MC, string path)
        {
            var state = MC.GetMemoryCardState();
            switch (state)
            {
                default: { WinFormsUtil.Error(MsgFileGameCubeBad, path); return false; }
                case GCMemoryCardState.NoPkmSaveGame: { WinFormsUtil.Error(MsgFileGameCubeNoGames, path); return false; }
                case GCMemoryCardState.DuplicateCOLO:
                case GCMemoryCardState.DuplicateXD:
                case GCMemoryCardState.DuplicateRSBOX: { WinFormsUtil.Error(MsgFileGameCubeDuplicate, path); return false; }
                case GCMemoryCardState.MultipleSaveGame:
                    {
                        GameVersion Game = SelectMemoryCardSaveGame(MC);
                        if (Game == GameVersion.Invalid) //Cancel
                            return false;
                        MC.SelectSaveGame(Game);
                        break;
                    }
                case GCMemoryCardState.SaveGameCOLO: MC.SelectSaveGame(GameVersion.COLO); break;
                case GCMemoryCardState.SaveGameXD: MC.SelectSaveGame(GameVersion.XD); break;
                case GCMemoryCardState.SaveGameRSBOX: MC.SelectSaveGame(GameVersion.RSBOX); break;
            }
            return true;
        }

        private static void StoreLegalSaveGameData(SaveFile sav)
        {
            Legal.EReaderBerryIsEnigma = sav.IsEBerryIsEnigma;
            Legal.EReaderBerryName = sav.EBerryName;
            Legal.ActiveTrainer = sav;
        }

        private static PKM LoadTemplate(SaveFile sav)
        {
            if (!Directory.Exists(TemplatePath))
                return null;

            var blank = sav.BlankPKM;
            var di = new DirectoryInfo(TemplatePath);
            string path = Path.Combine(TemplatePath, $"{di.Name}.{blank.Extension}");

            if (!File.Exists(path) || !PKX.IsPKM(new FileInfo(path).Length))
                return null;

            var pk = PKMConverter.GetPKMfromBytes(File.ReadAllBytes(path), prefer: blank.Format);
            return PKMConverter.ConvertToType(pk, sav.BlankPKM.GetType(), out path); // no sneaky plz; reuse string
        }

        private bool OpenSAV(SaveFile sav, string path)
        {
            if (sav == null || sav.Version == GameVersion.Invalid)
            { WinFormsUtil.Error(MsgFileLoadSaveLoadFail, path); return true; }

            sav.SetFileInfo(path);
            if (!SanityCheckSAV(ref sav))
                return true;
            StoreLegalSaveGameData(sav);
            PKMConverter.Trainer = sav;
            SpriteUtil.Spriter.Initialize(sav); // refresh sprite generator

            // clean fields
            C_SAV.M.Reset();
            Menu_ExportSAV.Enabled = sav.Exportable;

            // No changes made yet
            Menu_Undo.Enabled = false;
            Menu_Redo.Enabled = false;

            ResetSAVPKMEditors(sav);

            Text = GetProgramTitle(sav);
            TryBackupExportCheck(sav, path);

            Menu_ShowdownExportBattleBox.Visible = sav.HasBattleBox;
            Menu_ShowdownExportParty.Visible = sav.HasParty;
            Menu_ShowdownExportCurrentBox.Visible = sav.HasBox;

            SystemSounds.Beep.Play();
            return true;
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
            Menu_LoadBoxes.Enabled = Menu_DumpBoxes.Enabled = Menu_DumpBox.Enabled = Menu_Report.Enabled = C_SAV.SAV.HasBox;

            // Initialize Subviews
            bool WindowTranslationRequired = false;
            WindowTranslationRequired |= PKME_Tabs.ToggleInterface(sav, pk);
            WindowTranslationRequired |= C_SAV.ToggleInterface();
            if (WindowTranslationRequired) // force update -- re-added controls may be untranslated
                WinFormsUtil.TranslateInterface(this, CurrentLanguage);

            if (WindowToggleRequired) // Version combobox selectedvalue needs a little help, only updates once it is visible
                PKME_Tabs.FlickerInterface();

            PKME_Tabs.TemplateFields(LoadTemplate(sav));
            foreach (var p in Plugins)
                p.NotifySaveLoaded();
            sav.Edited = false;
        }

        private static string GetProgramTitle()
        {
#if DEBUG
            var d = File.GetLastWriteTime(System.Reflection.Assembly.GetEntryAssembly().Location);
            string date = $"d-{d:yyyyMMdd}";
#else
            string date = Resources.ProgramVersion;
#endif
            return $"PKH{(HaX ? "a" : "e")}X ({date})";
        }

        private static string GetProgramTitle(SaveFile sav)
        {
            string title = GetProgramTitle() + $" - {sav.GetType().Name}: ";
            if (Settings.Default.HideSAVDetails)
                return title + $"[{sav.Version}]";
            if (!sav.Exportable) // Blank save file
                return title + $"{sav.FileName} [{sav.OT} ({sav.Version})]";
            return title + Path.GetFileNameWithoutExtension(Util.CleanFileName(sav.BAKName)); // more descriptive
        }

        private static bool TryBackupExportCheck(SaveFile sav, string path)
        {
            if (string.IsNullOrWhiteSpace(path)) // not actual save
                return false;

            // If backup folder exists, save a backup.
            string backupName = Path.Combine(BackupPath, Util.CleanFileName(sav.BAKName));
            if (sav.Exportable && Directory.Exists(BackupPath) && !File.Exists(backupName))
                File.WriteAllBytes(backupName, sav.BAK);

            if (!IsFileLocked(path))
                return true;

            WinFormsUtil.Alert(MsgFileWriteProtected + Environment.NewLine + path, MsgFileWriteProtectedAdvice);
            return false;
        }

        private static bool IsFileLocked(string path)
        {
            try { return File.GetAttributes(path).HasFlag(FileAttributes.ReadOnly); }
            catch { return true; }
        }

        private static bool SanityCheckSAV(ref SaveFile sav)
        {
            // Finish setting up the save file.
            if (sav.Generation < 3)
            {
                bool vc = sav.FileName.EndsWith("dat");
                Legal.AllowGBCartEra = !vc; // physical cart selected
                Legal.AllowGen1Tradeback = true;
                if (Legal.AllowGBCartEra && sav.Generation == 1)
                {
                    var drTradeback = WinFormsUtil.Prompt(MessageBoxButtons.YesNoCancel,
                        MsgLegalityAllowTradebacks,
                        MsgLegalityAllowTradebacksYes + Environment.NewLine + MsgLegalityAllowTradebacksNo);
                    if (drTradeback == DialogResult.Cancel)
                        return false;
                    Legal.AllowGen1Tradeback = drTradeback == DialogResult.Yes;
                }
            }
            else
            {
                Legal.AllowGBCartEra = false;
                Legal.AllowGen1Tradeback = true;
            }

            if (sav.Generation == 3 && (sav.IndeterminateGame || ModifierKeys == Keys.Control))
            {
                WinFormsUtil.Alert(string.Format(MsgFileLoadVersionDetect, sav.Generation), MsgFileLoadVersionSelect);
                var g = new[] { GameVersion.R, GameVersion.S, GameVersion.E, GameVersion.FR, GameVersion.LG };
                var games = g.Select(z => GameInfo.VersionDataSource.First(v => v.Value == (int)z));
                var dialog = new SAV_GameSelect(games);
                dialog.ShowDialog();

                sav = SaveUtil.GetG3SaveOverride(sav, dialog.Result);
                if (sav == null)
                    return false;
                if (sav.Version == GameVersion.FRLG)
                    sav.Personal = dialog.Result == GameVersion.FR ? PersonalTable.FR : PersonalTable.LG;
            }
            else if (sav.IndeterminateSubVersion && sav.Version == GameVersion.FRLG)
            {
                string fr = GameInfo.VersionDataSource.First(r => r.Value == (int) GameVersion.FR).Text;
                string lg = GameInfo.VersionDataSource.First(l => l.Value == (int) GameVersion.LG).Text;
                string dual = "{0}/{1} " + MsgFileLoadSaveDetected;
                WinFormsUtil.Alert(string.Format(dual, fr, lg), MsgFileLoadSaveSelectVersion);
                var g = new[] {GameVersion.FR, GameVersion.LG};
                var games = g.Select(z => GameInfo.VersionDataSource.First(v => v.Value == (int) z));
                var dialog = new SAV_GameSelect(games);
                dialog.ShowDialog();

                var pt = SaveUtil.GetG3Personal(dialog.Result);
                if (pt == null)
                    return false;
                sav.Personal = pt;
            }

            return true;
        }

        public static void SetCountrySubRegion(ComboBox CB, string type)
        {
            int index = CB.SelectedIndex;
            // fix for Korean / Chinese being swapped
            string cl = GameInfo.CurrentLanguage;
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
            Task.Run(() => {
                    var lang = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Substring(0, 2);
                    Util.SetLocalization(typeof(LegalityCheckStrings), lang);
                    Util.SetLocalization(typeof(MessageStrings), lang);
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
            byte[] input = QR.GetQRData(url);
            if (input == null)
                return;

            var sav = C_SAV.SAV;
            if (FileUtil.TryGetPKM(input, out var pk, sav.Generation.ToString(), sav))
            {
                OpenPKM(pk);
                return;
            }
            if (FileUtil.TryGetMysteryGift(input, out var mg, url))
            {
                OpenMysteryGift(mg, url);
                return;
            }

            WinFormsUtil.Alert(MsgQRDecodeFail, string.Format(MsgQRDecodeSize, input.Length));
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
                    qr = QR.GenerateQRCode7((PK7)pkx);
                    break;
                default:
                    if (pkx.Format == 6 && !QR6Notified) // hint that the user should not be using QR6 injection
                    {
                        WinFormsUtil.Alert(MsgQRDeprecated, MsgQRAlternative);
                        QR6Notified = true;
                    }
                    qr = QR.GetQRImage(pkx.EncryptedBoxData, QR.GetQRServer(pkx.Format));
                    break;
            }

            if (qr == null)
                return;

            var sprite = dragout.Image;
            var la = new LegalityAnalysis(pkx, C_SAV.SAV.Personal);
            if (la.Parsed && pkx.Species != 0)
            {
                var img = la.Valid ? Resources.valid : Resources.warn;
                sprite = ImageUtil.LayerImage(sprite, img, 24, 0);
            }

            string[] r = pkx.QRText;
            string refer = GetProgramTitle();
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
            var la = new LegalityAnalysis(pk, C_SAV.SAV.Personal);
            if (pk.Slot < 0)
                PKME_Tabs.UpdateLegality(la);
            bool verbose = ModifierKeys == Keys.Control;
            var report = la.Report(verbose);
            if (verbose)
            {
                var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, report, MsgClipboardLegalityExport);
                if (dr == DialogResult.Yes)
                    Clipboard.SetText(report);
            }
            else
            {
                WinFormsUtil.Alert(report);
            }
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

            pb.Image = pk.Sprite(C_SAV.SAV, -1, -1, flagIllegal: false);
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
            string fn = pkx.FileNameWithoutExtension;
            string filename = fn + (encrypt ? $".ek{pkx.Format}" : $".{pkx.Extension}");
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

        private void Dragout_DragOver(object sender, DragEventArgs e) => e.Effect = DragDropEffects.Move;

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
                var prompt = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgProgramCloseUnsaved, MsgProgramCloseConfirm);
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
            if (Menu_ExportSAV.Enabled)
                C_SAV.ExportSaveFile();
        }

        private void ClickSaveFileName(object sender, EventArgs e)
        {
            if (!DetectSaveFile(out string path))
                return;
            if (WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgFileLoadSaveDetectReload, path) == DialogResult.Yes)
                OpenQuick(path); // load save
        }

        private static bool DetectSaveFile(out string path)
        {
            string msg = null;
            var sav = SaveDetection.DetectSaveFile(Environment.GetLogicalDrives(), ref msg);
            if (sav == null && !string.IsNullOrWhiteSpace(msg))
                WinFormsUtil.Error(msg);

            path = sav?.FilePath;
            return path != null && File.Exists(path);
        }

        private static void PromptBackup()
        {
            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, string.Format(MsgBackupCreateLocation, BackupPath), MsgBackupCreateQuestion))
                return;

            try
            {
                Directory.CreateDirectory(BackupPath);
                WinFormsUtil.Alert(MsgBackupSuccess, string.Format(MsgBackupDelete, BackupPath));
            }
            catch (Exception ex) { WinFormsUtil.Error($"{MsgBackupUnable} @ {BackupPath}", ex); }
        }

        private void ClickUndo(object sender, EventArgs e) => C_SAV.ClickUndo();
        private void ClickRedo(object sender, EventArgs e) => C_SAV.ClickRedo();
        #endregion
    }
}
