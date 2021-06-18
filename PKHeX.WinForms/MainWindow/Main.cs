using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using PKHeX.Core;
using PKHeX.Drawing;
using PKHeX.WinForms.Controls;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms
{
    public partial class Main : Form
    {
        private static readonly Version CurrentProgramVersion = Assembly.GetExecutingAssembly().GetName().Version!;

        public Main()
        {
            string[] args = Environment.GetCommandLineArgs();
            FormLoadInitialSettings(args, out bool showChangelog, out bool BAKprompt);

            InitializeComponent();
            C_SAV.SetEditEnvironment(new SaveDataEditor<PictureBox>(new FakeSaveFile(), PKME_Tabs));
            FormLoadAddEvents();
            #if DEBUG // translation updater -- all controls are added at this point -- call translate now
            if (DevUtil.IsUpdatingTranslations)
                WinFormsUtil.TranslateInterface(this, CurrentLanguage); // Translate the UI to language.
            #endif
            FormInitializeSecond();

            FormLoadCustomBackupPaths();
            FormLoadInitialFiles(args);
            FormLoadCheckForUpdates();

            if (Settings.Startup.LoadPlugins)
                FormLoadPlugins();

            if (HaX)
            {
                PKMConverter.AllowIncompatibleConversion = true;
                WinFormsUtil.Alert(MsgProgramIllegalModeActive, MsgProgramIllegalModeBehave);
            }
            else if (showChangelog)
            {
                ShowAboutDialog(1);
            }

            if (BAKprompt && !Directory.Exists(BackupPath))
                PromptBackup();

            BringToFront();
            WindowState = FormWindowState.Minimized;
            Show();
            WindowState = FormWindowState.Normal;
        }

        #region Important Variables
        public static string CurrentLanguage
        {
            get => GameInfo.CurrentLanguage;
            private set => GameInfo.CurrentLanguage = value;
        }

        private static bool _unicode;

        public static bool Unicode
        {
            get => _unicode;
            private set
            {
                _unicode = value;
                GenderSymbols = value ? GameInfo.GenderSymbolUnicode : GameInfo.GenderSymbolASCII;
            }
        }

        public static IReadOnlyList<string> GenderSymbols { get; private set; } = GameInfo.GenderSymbolUnicode;
        public static bool HaX { get; private set; }

        private readonly string[] main_langlist = Enum.GetNames(typeof(ProgramLanguage));

        private static readonly List<IPlugin> Plugins = new();
        #endregion

        #region Path Variables

        public static readonly string WorkingDirectory = Application.StartupPath;
        public static readonly string DatabasePath = Path.Combine(WorkingDirectory, "pkmdb");
        public static readonly string MGDatabasePath = Path.Combine(WorkingDirectory, "mgdb");
        public static readonly string ConfigPath = Path.Combine(WorkingDirectory, "cfg.json");
        public static readonly string BackupPath = Path.Combine(WorkingDirectory, "bak");
        public static readonly string CryPath = Path.Combine(WorkingDirectory, "sounds");
        private static readonly string TemplatePath = Path.Combine(WorkingDirectory, "template");
        private static readonly string PluginPath = Path.Combine(WorkingDirectory, "plugins");
        private const string ThreadPath = "https://projectpokemon.org/pkhex/";

        public static readonly PKHeXSettings Settings = PKHeXSettings.GetSettings(ConfigPath);

        #endregion

        #region //// MAIN MENU FUNCTIONS ////
        private static void FormLoadInitialSettings(string[] args, out bool showChangelog, out bool BAKprompt)
        {
            showChangelog = false;
            BAKprompt = false;

            HaX = args.Any(x => string.Equals(x.Trim('-'), nameof(HaX), StringComparison.CurrentCultureIgnoreCase))
                || Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule!.FileName!).EndsWith(nameof(HaX));

            FormLoadConfig(out BAKprompt, out showChangelog);
            HaX |= Settings.Startup.ForceHaXOnLaunch;

            WinFormsUtil.AddSaveFileExtensions(Settings.Backup.OtherSaveFileExtensions);
        }

        private static void FormLoadCustomBackupPaths()
        {
            SaveFinder.CustomBackupPaths.Clear();
            SaveFinder.CustomBackupPaths.AddRange(Settings.Backup.OtherBackupPaths.Where(Directory.Exists));
        }

        private void FormLoadAddEvents()
        {
            C_SAV.Menu_Redo = Menu_Redo;
            C_SAV.Menu_Undo = Menu_Undo;
            dragout.GiveFeedback += (sender, e) => e.UseDefaultCursors = false;
            GiveFeedback += (sender, e) => e.UseDefaultCursors = false;
            PKME_Tabs.EnableDragDrop(Main_DragEnter, Main_DragDrop);
            C_SAV.EnableDragDrop(Main_DragEnter, Main_DragDrop);

            // ToolTips for Drag&Drop
            dragTip.SetToolTip(dragout, "PKM QuickSave");

            // Box to Tabs D&D
            dragout.AllowDrop = true;

            // Add ContextMenus
            var mnu = new ContextMenuPKM();
            mnu.RequestEditorLegality += (o, args) => ClickLegality(mnu, args);
            mnu.RequestEditorQR += (o, args) => ClickQR(mnu, args);
            mnu.RequestEditorSaveAs += (o, args) => MainMenuSave(mnu, args);
            dragout.ContextMenuStrip = mnu.mnuL;
            C_SAV.menu.RequestEditorLegality += ShowLegality;
        }

        private void FormLoadInitialFiles(string[] args)
        {
            string pkmArg = string.Empty;
            foreach (string arg in args.Skip(1)) // skip .exe
            {
                var fi = new FileInfo(arg);
                if (!fi.Exists)
                    continue;

                if (PKX.IsPKM(fi.Length))
                    pkmArg = arg;
                else
                    OpenFromPath(arg);
            }
            if (C_SAV.SAV is FakeSaveFile) // No SAV loaded from exe args
            {
                #if !DEBUG
                try
                #endif
                {
                    var startup = Settings.Startup;
                    string path = string.Empty;
                    SaveFile? sav = null;
                    if (startup.AutoLoadSaveOnStartup == AutoLoadSetting.RecentBackup)
                    {
                        if (!SaveFinder.DetectSaveFile(out path, out sav))
                        {
                            if (!string.IsNullOrWhiteSpace(path))
                                WinFormsUtil.Error(path); // `path` contains the error message
                        }
                    }
                    else if (startup.AutoLoadSaveOnStartup == AutoLoadSetting.LastLoaded)
                    {
                        if (startup.RecentlyLoaded.Count != 0)
                        {
                            path = startup.RecentlyLoaded[0];
                            if (File.Exists(path))
                                sav = SaveUtil.GetVariantSAV(path);
                        }
                    }

                    bool savLoaded = false;
                    if (sav != null && path.Length != 0)
                    {
                        savLoaded = OpenSAV(sav, path);
                    }
                    if (!savLoaded)
                        LoadBlankSaveFile(startup.DefaultSaveVersion);
                }
                #if !DEBUG
                catch (Exception ex)
                {
                    ErrorWindow.ShowErrorDialog(MsgFileLoadFailAuto, ex, true);
                }
                #endif
            }

            if (!string.IsNullOrWhiteSpace(pkmArg) && File.Exists(pkmArg))
            {
                byte[] data = File.ReadAllBytes(pkmArg);
                var pk = PKMConverter.GetPKMfromBytes(data);
                if (pk != null)
                    OpenPKM(pk);
            }
        }

        private void LoadBlankSaveFile(GameVersion ver)
        {
            var current = C_SAV?.SAV;
            var lang = SaveUtil.GetSafeLanguage(current);
            var tr = SaveUtil.GetSafeTrainerName(current, lang);
            var sav = SaveUtil.GetBlankSAV(ver, tr, lang);
            if (sav.Version == GameVersion.Invalid) // will fail to load
                sav = SaveUtil.GetBlankSAV((GameVersion)GameInfo.VersionDataSource.Max(z => z.Value), tr, lang);
            OpenSAV(sav, string.Empty);
            C_SAV!.SAV.State.Edited = false; // Prevents form close warning from showing until changes are made
        }

        private void FormLoadCheckForUpdates()
        {
            L_UpdateAvailable.Click += (sender, e) => Process.Start(ThreadPath);
            Task.Run(() =>
            {
                Version? latestVersion;
                // User might not be connected to the internet or with a flaky connection.
                try { latestVersion = UpdateUtil.GetLatestPKHeXVersion(); }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    Debug.WriteLine($"Exception while checking for latest version: {ex}");
                    return;
                }
                if (latestVersion is not null && latestVersion > CurrentProgramVersion)
                    Invoke((MethodInvoker)(() => NotifyNewVersionAvailable(latestVersion)));
            });
        }

        private void NotifyNewVersionAvailable(Version ver)
        {
            L_UpdateAvailable.Visible = true;
            var date = $"{2000 + ver.Major:00}{ver.Minor:00}{ver.Build:00}";
            L_UpdateAvailable.Text = $"{MsgProgramUpdateAvailable} {date}";
        }

        private static void FormLoadConfig(out bool BAKprompt, out bool showChangelog)
        {
            BAKprompt = false;
            showChangelog = false;

            // Version Check
            if (Settings.Startup.Version.Length > 0 && Settings.Startup.ShowChangelogOnUpdate) // already run on system
            {
                bool parsed = Version.TryParse(Settings.Startup.Version, out var lastrev);
                showChangelog = parsed && lastrev < CurrentProgramVersion;
            }
            Settings.Startup.Version = CurrentProgramVersion.ToString(); // set current ver so this doesn't happen until the user updates next time

            // BAK Prompt
            if (!Settings.Backup.BAKPrompt)
                BAKprompt = Settings.Backup.BAKPrompt = true;
        }

        public static DrawConfig Draw { get; private set; } = new();

        private void FormInitializeSecond()
        {
            var settings = Settings;
            Draw = C_SAV.M.Hover.Draw = PKME_Tabs.Draw = settings.Draw;
            ReloadProgramSettings(settings);
            CB_MainLanguage.Items.AddRange(main_langlist);
            PB_Legal.Visible = !HaX;
            C_SAV.HaX = PKME_Tabs.HaX = HaX;

            #if DEBUG
            DevUtil.AddControl(Menu_Tools);
            #endif

            // Select Language
            CB_MainLanguage.SelectedIndex = GameLanguage.GetLanguageIndex(settings.Startup.Language);
        }

        private void FormLoadPlugins()
        {
            #if !MERGED // merged should load dlls from within too, folder is no longer required
            if (!Directory.Exists(PluginPath))
                return;
            #endif
            try
            {
                Plugins.AddRange(PluginLoader.LoadPlugins<IPlugin>(PluginPath));
            }
            catch (InvalidCastException c)
            {
                WinFormsUtil.Error(MsgPluginFailLoad, c);
                return;
            }
            foreach (var p in Plugins.OrderBy(z => z.Priority))
                p.Initialize(C_SAV, PKME_Tabs, menuStrip1, CurrentProgramVersion);
        }

        // Main Menu Strip UI Functions
        private void MainMenuOpen(object sender, EventArgs e)
        {
            if (WinFormsUtil.OpenSAVPKMDialog(C_SAV.SAV.PKMExtensions, out var path))
                OpenQuick(path!);
        }

        private void MainMenuSave(object sender, EventArgs e)
        {
            if (!PKME_Tabs.EditsComplete)
                return;
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

        private void MainMenuAbout(object sender, EventArgs e) => ShowAboutDialog(0);

        private static void ShowAboutDialog(int index = 0)
        {
            using var form = new About(index);
            form.ShowDialog();
        }

        // Sub Menu Options
        private void MainMenuBoxReport(object sender, EventArgs e)
        {
            if (this.OpenWindowExists<ReportGrid>())
                return;

            var report = new ReportGrid();
            report.Show();
            report.PopulateData(C_SAV.SAV.BoxData);
        }

        private void MainMenuDatabase(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Shift)
            {
                if (!this.OpenWindowExists<KChart>())
                    new KChart(C_SAV.SAV).Show();
                return;
            }

            if (!Directory.Exists(DatabasePath))
            {
                WinFormsUtil.Alert(MsgDatabase, string.Format(MsgDatabaseAdvice, DatabasePath));
                return;
            }

            if (!this.OpenWindowExists<SAV_Database>())
                new SAV_Database(PKME_Tabs, C_SAV).Show();
        }

        private void Menu_EncDatabase_Click(object sender, EventArgs e)
        {
            if (!this.OpenWindowExists<SAV_Encounters>())
                new SAV_Encounters(PKME_Tabs).Show();
        }

        private void MainMenuMysteryDB(object sender, EventArgs e)
        {
            if (!this.OpenWindowExists<SAV_MysteryGiftDB>())
                new SAV_MysteryGiftDB(PKME_Tabs, C_SAV).Show();
        }

        private void MainMenuSettings(object sender, EventArgs e)
        {
            var settings = Settings;
            var ver = Settings.Startup.DefaultSaveVersion; // check if it changes
            using var form = new SettingsEditor(settings);
            form.ShowDialog();

            // Reload text (if OT details hidden)
            Text = GetProgramTitle(C_SAV.SAV);
            // Update final settings
            ReloadProgramSettings(Settings);

            if (ver != Settings.Startup.DefaultSaveVersion) // changed by user
            {
                LoadBlankSaveFile(Settings.Startup.DefaultSaveVersion);
                return;
            }

            PKME_Tabs_UpdatePreviewSprite(sender, e);
            if (C_SAV.SAV.HasBox)
                C_SAV.ReloadSlots();
        }

        private void ReloadProgramSettings(PKHeXSettings settings)
        {
            Draw.LoadBrushes();
            PKME_Tabs.Unicode = Unicode = settings.Display.Unicode;
            PKME_Tabs.UpdateUnicode(GenderSymbols);
            SpriteName.AllowShinySprite = settings.Display.ShinySprites;
            SaveFile.SetUpdateDex = settings.SlotWrite.SetUpdateDex ? PKMImportSetting.Update : PKMImportSetting.Skip;
            SaveFile.SetUpdatePKM = settings.SlotWrite.SetUpdatePKM ? PKMImportSetting.Update : PKMImportSetting.Skip;
            C_SAV.ModifyPKM = PKME_Tabs.ModifyPKM = settings.SlotWrite.SetUpdatePKM;
            CommonEdits.ShowdownSetIVMarkings = settings.Import.ApplyMarkings;
            CommonEdits.ShowdownSetBehaviorNature = settings.Import.ApplyNature;
            C_SAV.FlagIllegal = settings.Display.FlagIllegal;
            C_SAV.M.Hover.GlowHover = settings.Hover.HoverSlotGlowEdges;
            SpriteBuilder.ShowEggSpriteAsItem = settings.Display.ShowEggSpriteAsHeldItem;
            ParseSettings.InitFromSettings(settings.Legality);
            PKME_Tabs.HideSecretValues = C_SAV.HideSecretDetails = settings.Privacy.HideSecretDetails;
            PKMConverter.AllowIncompatibleConversion = settings.Advanced.AllowIncompatibleConversion;
            WinFormsUtil.DetectSaveFileOnFileOpen = settings.Startup.TryDetectRecentSave;
        }

        private void MainMenuBoxLoad(object sender, EventArgs e)
        {
            string? path = null;
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
            string? path = null;
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
            using var form = new BatchEditor(PKME_Tabs.PreparePKM(), C_SAV.SAV);
            form.ShowDialog();
            C_SAV.SetPKMBoxes(); // refresh
            C_SAV.UpdateBoxViewers();
        }

        private void MainMenuFolder(object sender, EventArgs e)
        {
            if (this.OpenWindowExists<SAV_FolderList>())
                return;
            var form = new SAV_FolderList(s => OpenSAV(SaveUtil.GetVariantSAV(s.Metadata.FilePath!), s.Metadata.FilePath!));
            form.Show();
        }

        // Misc Options
        private void ClickShowdownImportPKM(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsText())
            { WinFormsUtil.Alert(MsgClipboardFailRead); return; }

            // Get Simulator Data
            var Set = new ShowdownSet(Clipboard.GetText());

            if (Set.Species < 0)
            { WinFormsUtil.Alert(MsgSimulatorFailClipboard); return; }

            if (Set.Nickname.Length > C_SAV.SAV.NickLength)
                Set.Nickname = Set.Nickname[..C_SAV.SAV.NickLength];

            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgSimulatorLoad, Set.Text))
                return;

            if (Set.InvalidLines.Count > 0)
                WinFormsUtil.Alert(MsgSimulatorInvalid, string.Join(Environment.NewLine, Set.InvalidLines));

            // Set Species & Nickname
            PKME_Tabs.LoadShowdownSet(Set);
        }

        private void ClickShowdownExportPKM(object sender, EventArgs e)
        {
            if (!PKME_Tabs.EditsComplete)
            {
                WinFormsUtil.Alert(MsgSimulatorExportBadFields);
                return;
            }

            var pk = PreparePKM();
            var text = ShowdownParsing.GetShowdownText(pk);
            bool success = WinFormsUtil.SetClipboardText(text);
            if (!success || !Clipboard.GetText().Equals(text))
                WinFormsUtil.Alert(MsgClipboardFailWrite, MsgSimulatorExportFail);
            else
                WinFormsUtil.Alert(MsgSimulatorExportSuccess, text);
        }

        private void ClickShowdownExportParty(object sender, EventArgs e) => C_SAV.ClickShowdownExportParty(sender, e);
        private void ClickShowdownExportCurrentBox(object sender, EventArgs e) => C_SAV.ClickShowdownExportCurrentBox(sender, e);

        // Main Menu Subfunctions
        private void OpenQuick(string path)
        {
            if (!CanFocus)
            {
                SystemSounds.Asterisk.Play();
                return;
            }
            OpenFromPath(path);
        }

        private void OpenFromPath(string path)
        {
            if (Plugins.Any(p => p.TryLoadFile(path)))
                return; // handled by plugin

            // detect if it is a folder (load into boxes or not)
            if (Directory.Exists(path))
            { C_SAV.LoadBoxes(out string _, path); return; }

            var fi = new FileInfo(path);
            if (!fi.Exists)
                return;

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
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e) { WinFormsUtil.Error(MsgFileInUse + path, e); return; }
#pragma warning restore CA1031 // Do not catch general exception types

            string ext = fi.Extension;
            #if DEBUG
                OpenFile(input, path, ext);
            #else
                try { OpenFile(input, path, ext); }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e) { WinFormsUtil.Error(MsgFileLoadFail + "\nPath: " + path, e); }
#pragma warning restore CA1031 // Do not catch general exception types
            #endif
        }

        private void OpenFile(byte[] input, string path, string ext)
        {
            var obj = FileUtil.GetSupportedFile(input, ext, C_SAV.SAV);
            if (obj != null && LoadFile(obj, path))
                return;

            bool isSAV = WinFormsUtil.IsFileExtensionSAV(path);
            var msg = isSAV ? MsgFileUnsupported : MsgPKMUnsupported;
            WinFormsUtil.Error(msg,
                $"{MsgFileLoad}{Environment.NewLine}{path}",
                $"{string.Format(MsgFileSize, input.Length)}{Environment.NewLine}{input.Length} bytes (0x{input.Length:X4})");
        }

        private bool LoadFile(object? input, string path)
        {
            if (input == null)
                return false;

            switch (input)
            {
                case PKM pk: return OpenPKM(pk);
                case SaveFile s: return OpenSAV(s, path);
                case IPokeGroup b: return OpenGroup(b);
                case MysteryGift g: return OpenMysteryGift(g, path);
                case IEnumerable<byte[]> pkms: return OpenPCBoxBin(pkms);
                case IEncounterable enc: return OpenPKM(enc.ConvertToPKM(C_SAV.SAV));

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
            var tmp = PKMConverter.ConvertToType(pk, C_SAV.SAV.PKMType, out string c);
            Debug.WriteLine(c);
            if (tmp == null)
                return false;
            C_SAV.SAV.AdaptPKM(tmp);
            PKME_Tabs.PopulateFields(tmp);
            return true;
        }

        private bool OpenGroup(IPokeGroup b)
        {
            bool result = C_SAV.OpenGroup(b, out string c);
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
            var pk = PKMConverter.ConvertToType(temp, C_SAV.SAV.PKMType, out string c);

            if (pk == null)
            {
                WinFormsUtil.Alert(MsgPKMConvertFail, c);
                return true;
            }

            C_SAV.SAV.AdaptPKM(pk);
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
            if (MC.HasCOLO) games.Add(new ComboItem(MsgGameColosseum, (int)GameVersion.COLO));
            if (MC.HasXD) games.Add(new ComboItem(MsgGameXD, (int)GameVersion.XD));
            if (MC.HasRSBOX) games.Add(new ComboItem(MsgGameRSBOX, (int)GameVersion.RSBOX));

            var dialog = new SAV_GameSelect(games, MsgFileLoadSaveMultiple, MsgFileLoadSaveSelectGame);
            dialog.ShowDialog();
            return dialog.Result;
        }

        private static bool CheckGCMemoryCard(SAV3GCMemoryCard MC, string path)
        {
            var state = MC.GetMemoryCardState();
            switch (state)
            {
                case GCMemoryCardState.NoPkmSaveGame:
                    WinFormsUtil.Error(MsgFileGameCubeNoGames, path);
                    return false;

                case GCMemoryCardState.DuplicateCOLO:
                case GCMemoryCardState.DuplicateXD:
                case GCMemoryCardState.DuplicateRSBOX:
                    WinFormsUtil.Error(MsgFileGameCubeDuplicate, path);
                    return false;

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

                default:
                    WinFormsUtil.Error(!SaveUtil.IsSizeValid(MC.Data.Length) ? MsgFileGameCubeBad : MsgFileLoadSaveLoadFail, path);
                    return false;
            }
            return true;
        }

        private static void StoreLegalSaveGameData(SaveFile sav)
        {
            if (sav is SAV3 sav3)
                EReaderBerrySettings.LoadFrom(sav3);
        }

        private bool OpenSAV(SaveFile? sav, string path)
        {
            if (sav == null || sav.Version == GameVersion.Invalid)
            {
                WinFormsUtil.Error(MsgFileLoadSaveLoadFail, path);
                return true;
            }

            sav.Metadata.SetExtraInfo(path);
            if (!SanityCheckSAV(ref sav))
                return true;

            if (C_SAV.SAV.State.Edited && Settings.SlotWrite.ModifyUnset)
            {
                var prompt = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgProgramCloseUnsaved, MsgProgramSaveFileConfirm);
                if (prompt != DialogResult.Yes)
                    return true;
            }

            PKME_Tabs.Focus(); // flush any pending changes
            StoreLegalSaveGameData(sav);
            PKMConverter.SetPrimaryTrainer(sav);
            SpriteUtil.Initialize(sav); // refresh sprite generator
            dragout.Size = new Size(SpriteUtil.Spriter.Width, SpriteUtil.Spriter.Height);

            // clean fields
            Menu_ExportSAV.Enabled = sav.State.Exportable;

            // No changes made yet
            Menu_Undo.Enabled = false;
            Menu_Redo.Enabled = false;

            GameInfo.FilteredSources = new FilteredGameDataSource(sav, GameInfo.Sources, HaX);
            ResetSAVPKMEditors(sav);
            C_SAV.M.Reset();

            Text = GetProgramTitle(sav);
            TryBackupExportCheck(sav, path);

            Menu_ShowdownExportParty.Visible = sav.HasParty;
            Menu_ShowdownExportCurrentBox.Visible = sav.HasBox;

            Settings.Startup.LoadSaveFile(path);
            if (Settings.Sounds.PlaySoundSAVLoad)
                SystemSounds.Asterisk.Play();
            return true;
        }

        private void ResetSAVPKMEditors(SaveFile sav)
        {
            bool WindowToggleRequired = C_SAV.SAV.Generation < 3 && sav.Generation >= 3; // version combobox refresh hack
            C_SAV.SetEditEnvironment(new SaveDataEditor<PictureBox>(sav, PKME_Tabs));

            var pk = sav.LoadTemplate(TemplatePath);
            var isBlank = pk.Data.SequenceEqual(sav.BlankPKM.Data);
            if (isBlank)
                EntityTemplates.TemplateFields(pk, sav);
            bool init = PKME_Tabs.IsInitialized;
            PKME_Tabs.CurrentPKM = pk;
            if (!init)
            {
                PKME_Tabs.InitializeBinding();
                PKME_Tabs.IsInitialized = true;
                PKME_Tabs.SetPKMFormatMode(sav.Generation, pk);
                PKME_Tabs.ChangeLanguage(sav, pk); // populates fields
            }
            else
            {
                PKME_Tabs.SetPKMFormatMode(sav.Generation, pk);
                PKME_Tabs.PopulateFields(pk);
            }

            // Initialize Overall Info
            Menu_LoadBoxes.Enabled = Menu_DumpBoxes.Enabled = Menu_DumpBox.Enabled = Menu_Report.Enabled = C_SAV.SAV!.HasBox;

            // Initialize Subviews
            bool WindowTranslationRequired = false;
            WindowTranslationRequired |= PKME_Tabs.ToggleInterface(sav, pk);
            WindowTranslationRequired |= C_SAV.ToggleInterface();
            if (WindowTranslationRequired) // force update -- re-added controls may be untranslated
                WinFormsUtil.TranslateInterface(this, CurrentLanguage);

            PKME_Tabs.PopulateFields(pk);
            if (WindowToggleRequired) // Version combobox selectedvalue needs a little help, only updates once it is visible
                PKME_Tabs.FlickerInterface();
            foreach (var p in Plugins)
                p.NotifySaveLoaded();
            sav.State.Edited = false;
        }

        private static string GetProgramTitle()
        {
            #if DEBUG
            var date = File.GetLastWriteTime(Assembly.GetEntryAssembly()!.Location);
            string version = $"d-{date:yyyyMMdd}";
            #else
            var ver = CurrentProgramVersion;
            string version = $"{2000+ver.Major:00}{ver.Minor:00}{ver.Build:00}";
            #endif
            return $"PKH{(HaX ? "a" : "e")}X ({version})";
        }

        private static string GetProgramTitle(SaveFile sav)
        {
            string title = GetProgramTitle() + $" - {sav.GetType().Name}: ";
            if (sav is ISaveFileRevision rev)
                title = title.Insert(title.Length - 2, rev.SaveRevisionString);
            var ver = GameInfo.GetVersionName(sav.Version);
            if (Settings.Privacy.HideSAVDetails)
                return title + $"[{ver}]";
            if (!sav.State.Exportable) // Blank save file
                return title + $"{sav.Metadata.FileName} [{sav.OT} ({ver})]";
            return title + Path.GetFileNameWithoutExtension(Util.CleanFileName(sav.Metadata.BAKName)); // more descriptive
        }

        private static bool TryBackupExportCheck(SaveFile sav, string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !Settings.Backup.BAKEnabled) // not actual save
                return false;

            // If backup folder exists, save a backup.
            string backupName = Path.Combine(BackupPath, Util.CleanFileName(sav.Metadata.BAKName));
            if (sav.State.Exportable && Directory.Exists(BackupPath) && !File.Exists(backupName))
                File.WriteAllBytes(backupName, sav.State.BAK);

            if (!FileUtil.IsFileLocked(path))
                return true;

            WinFormsUtil.Alert(MsgFileWriteProtected + Environment.NewLine + path, MsgFileWriteProtectedAdvice);
            return false;
        }

        private static bool SanityCheckSAV(ref SaveFile sav)
        {
            ParseSettings.InitFromSaveFileData(sav); // physical GB, no longer used in logic

            if (sav.State.Exportable && sav is SAV3 s3)
            {
                if (ModifierKeys == Keys.Control || s3.IsCorruptPokedexFF())
                {
                    var g = new[] { GameVersion.R, GameVersion.S, GameVersion.E, GameVersion.FR, GameVersion.LG };
                    var games = g.Select(z => GameInfo.VersionDataSource.First(v => v.Value == (int)z));
                    var msg = string.Format(MsgFileLoadVersionDetect, $"3 ({s3.Version})");
                    using var dialog = new SAV_GameSelect(games, msg, MsgFileLoadSaveSelectVersion);
                    dialog.ShowDialog();
                    if (dialog.Result is GameVersion.Invalid)
                        return false;

                    var s = SaveUtil.GetG3SaveOverride(sav, dialog.Result);
                    var origin = s3.Metadata.FilePath;
                    if (origin is not null)
                        s.Metadata.SetExtraInfo(origin);

                    sav = s;
                    if (sav is SAV3FRLG frlg)
                    {
                        bool result = frlg.ResetPersonal(dialog.Result);
                        if (!result)
                            return false;
                    }
                }
                else if (s3 is SAV3FRLG frlg) // IndeterminateSubVersion
                {
                    string fr = GameInfo.GetVersionName(GameVersion.FR);
                    string lg = GameInfo.GetVersionName(GameVersion.LG);
                    string dual = "{1}/{2} " + MsgFileLoadVersionDetect;
                    var g = new[] { GameVersion.FR, GameVersion.LG };
                    var games = g.Select(z => GameInfo.VersionDataSource.First(v => v.Value == (int)z));
                    var msg = string.Format(dual, "3", fr, lg);
                    using var dialog = new SAV_GameSelect(games, msg, MsgFileLoadSaveSelectVersion);
                    dialog.ShowDialog();
                    bool result = frlg.ResetPersonal(dialog.Result);
                    if (!result)
                        return false;
                }
            }

            return true;
        }

        public static void SetCountrySubRegion(ComboBox CB, string type)
        {
            int index = CB.SelectedIndex;
            string cl = GameInfo.CurrentLanguage;
            CB.DataSource = Util.GetCountryRegionList(type, cl);

            if (index > 0 && index < CB.Items.Count)
                CB.SelectedIndex = index;
        }

        // Language Translation
        private void ChangeMainLanguage(object sender, EventArgs e)
        {
            var index = CB_MainLanguage.SelectedIndex;
            if ((uint)index < CB_MainLanguage.Items.Count)
                CurrentLanguage = GameLanguage.Language2Char(index);

            // Set the culture (makes it easy to pass language to other forms)
            var lang = CurrentLanguage;
            Settings.Startup.Language = lang;
            var ci = new CultureInfo(lang[..2]);
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = ci;

            Menu_Options.DropDown.Close();

            var sav = C_SAV.SAV;
            LocalizeUtil.InitializeStrings(lang, sav, HaX);
            WinFormsUtil.TranslateInterface(this, lang); // Translate the UI to language.
            LocalizedDescriptionAttribute.Localizer = WinFormsTranslator.GetDictionary(lang);

            if (sav is not FakeSaveFile)
            {
                var pk = PKME_Tabs.CurrentPKM.Clone();

                PKME_Tabs.ChangeLanguage(sav, pk);
                Text = GetProgramTitle(sav);
            }
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
                    if (url.StartsWith("http") && !url.Contains('\n')) // qr payload
                        ImportQRToTabs(url);
                    else
                        ClickShowdownImportPKM(sender, e);
                    return;
                }
            }
            ExportQRFromTabs();
        }

        private void ImportQRToTabs(string url)
        {
            var msg = QRDecode.GetQRData(url, out var input);
            if (msg != 0)
            {
                WinFormsUtil.Alert(msg.ConvertMsg());
                return;
            }

            if (input.Length == 0)
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
            if (!PKME_Tabs.EditsComplete)
                return;

            PKM pk = PreparePKM();
            if (pk.Format == 6 && !QR6Notified) // hint that the user should not be using QR6 injection
            {
                WinFormsUtil.Alert(MsgQRDeprecated, MsgQRAlternative);
                QR6Notified = true;
            }

            var qr = QREncode.GenerateQRCode(pk);

            var sprite = dragout.Image;
            var la = new LegalityAnalysis(pk, C_SAV.SAV.Personal);
            if (la.Parsed && pk.Species != 0)
            {
                var img = SpriteUtil.GetLegalIndicator(la.Valid);
                sprite = ImageUtil.LayerImage(sprite, img, sprite.Width - img.Width, 0);
            }

            string[] r = pk.GetQRLines();
            string refer = GetProgramTitle();
            using var form = new QR(qr, sprite, pk, r[0], r[1], r[2], $"{refer} ({pk.GetType().Name})");
            form.ShowDialog();
        }

        private void ClickLegality(object sender, EventArgs e)
        {
            if (!PKME_Tabs.EditsComplete)
            { SystemSounds.Hand.Play(); return; }

            var pk = PreparePKM();

            if (pk.Species == 0 || !pk.ChecksumValid)
            { SystemSounds.Hand.Play(); return; }

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
                if (dr != DialogResult.Yes)
                    return;
#if DEBUG
                var enc = SummaryPreviewer.GetTextLines(la.EncounterOriginal);
                report += Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine, enc);
#endif
                WinFormsUtil.SetClipboardText(report);
            }
            else if (Settings.Display.IgnoreLegalPopup && la.Valid)
            {
                if (Settings.Sounds.PlaySoundLegalityCheck)
                    SystemSounds.Asterisk.Play();
            }
            else
            {
                WinFormsUtil.Alert(Settings.Sounds.PlaySoundLegalityCheck, report);
            }
        }

        private void ClickClone(object sender, EventArgs e)
        {
            if (!PKME_Tabs.EditsComplete)
                return; // don't copy garbage to the box
            PKM pk = PKME_Tabs.PreparePKM();
            C_SAV.SetClonesToBox(pk);
        }

        private void GetPreview(PictureBox pb, PKM? pk = null)
        {
            pk ??= PreparePKM(false); // don't perform control loss click

            dragout.ContextMenuStrip.Enabled = pk.Species != 0 || HaX; // Species

            pb.Image = pk.Sprite(C_SAV.SAV, -1, -1, flagIllegal: false);
            if (pb.BackColor == Color.Red)
                pb.BackColor = Color.Transparent;
        }

        private void PKME_Tabs_UpdatePreviewSprite(object sender, EventArgs e) => GetPreview(dragout);

        private void PKME_Tabs_LegalityChanged(object sender, EventArgs e)
        {
            if (HaX)
            {
                PB_Legal.Visible = false;
                return;
            }

            PB_Legal.Visible = true;
            PB_Legal.Image = SpriteUtil.GetLegalIndicator(sender as bool? != false);
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
            var files = (string[]?)e.Data.GetData(DataFormats.FileDrop);
            if (files == null || files.Length == 0)
                return;
            OpenQuick(files[0]);
            e.Effect = DragDropEffects.Copy;
        }

        private void Dragout_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            if (ModifierKeys is Keys.Alt or Keys.Shift)
            {
                ClickQR(sender, e);
                return;
            }

            if (!PKME_Tabs.EditsComplete)
                return;

            // Create Temp File to Drag
            var pk = PreparePKM();
            var encrypt = ModifierKeys == Keys.Control;
            var newfile = FileUtil.GetPKMTempFileName(pk, encrypt);
            var data = encrypt ? pk.EncryptedPartyData : pk.DecryptedPartyData;
            // Make file
            try
            {
                File.WriteAllBytes(newfile, data);

                var pb = (PictureBox)sender;
                if (pb.Image != null)
                    C_SAV.M.Drag.Info.Cursor = Cursor = new Cursor(((Bitmap)pb.Image).GetHicon());
                DoDragDrop(new DataObject(DataFormats.FileDrop, new[] { newfile }), DragDropEffects.Move);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            // Tons of things can happen with drag & drop; don't try to handle things, just indicate failure.
            catch (Exception x)
#pragma warning restore CA1031 // Do not catch general exception types
            { WinFormsUtil.Error("Drag && Drop Error", x); }
            C_SAV.M.Drag.ResetCursor(this);
            File.Delete(newfile);
        }

        private void Dragout_DragOver(object sender, DragEventArgs e) => e.Effect = DragDropEffects.Move;

        private void DragoutEnter(object sender, EventArgs e)
        {
            dragout.BackgroundImage = PKME_Tabs.Entity.Species > 0 ? SpriteUtil.Spriter.Set : SpriteUtil.Spriter.Delete;
            Cursor = Cursors.Hand;
        }

        private void DragoutLeave(object sender, EventArgs e)
        {
            dragout.BackgroundImage = SpriteUtil.Spriter.Transparent;
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
            if (C_SAV.SAV.State.Edited || PKME_Tabs.PKMIsUnsaved)
            {
                var prompt = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgProgramCloseUnsaved, MsgProgramCloseConfirm);
                if (prompt != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }

            PKHeXSettings.SaveSettings(ConfigPath, Settings);
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
                return; // hot-keys can't cheat the system!

            C_SAV.ExportSaveFile();
            Text = GetProgramTitle(C_SAV.SAV);
        }

        private void ClickSaveFileName(object sender, EventArgs e)
        {
            if (!SaveFinder.DetectSaveFile(out string path, out var sav))
            {
                if (!string.IsNullOrWhiteSpace(path))
                    WinFormsUtil.Error(path); // `path` contains the error message
                return;
            }

            if (WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgFileLoadSaveDetectReload, path) == DialogResult.Yes)
                LoadFile(sav, path); // load save
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
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            // Maybe they put their exe in a folder that we can't create files/folders to.
#pragma warning restore CA1031 // Do not catch general exception types
            { WinFormsUtil.Error($"{MsgBackupUnable} @ {BackupPath}", ex); }
        }

        private void ClickUndo(object sender, EventArgs e) => C_SAV.ClickUndo();
        private void ClickRedo(object sender, EventArgs e) => C_SAV.ClickRedo();
        #endregion
    }
}
