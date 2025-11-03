using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using PKHeX.Core;
using PKHeX.Drawing;
using PKHeX.Drawing.Misc;
using PKHeX.Drawing.PokeSprite;
using PKHeX.WinForms.Controls;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms;

public partial class Main : Form
{
    public Main()
    {
        InitializeComponent();
        if (Settings.Display.DisableScalingDpi)
            AutoScaleMode = AutoScaleMode.Font;
        C_SAV.SetEditEnvironment(new SaveDataEditor<PictureBox>(FakeSaveFile.Default, PKME_Tabs));
        FormLoadAddEvents();
#if DEBUG // translation updater -- all controls are added at this point -- call translate now
        if (DevUtil.IsUpdatingTranslations)
        {
            WinFormsUtil.TranslateInterface(this, CurrentLanguage); // Translate the UI to language.
            return;
        }
#endif
        FormInitializeSecond();
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
    public static bool HaX => Program.HaX;
    private static List<IPlugin> Plugins { get; } = [];
    private static PluginLoadResult? PluginLoadResult { get; set; } // keep alive so that plugins may load their external dependencies if needed
    #endregion

    #region Path Variables

    public static string DatabasePath => Settings.LocalResources.GetDatabasePath();
    public static string MGDatabasePath => Settings.LocalResources.GetMGDatabasePath();
    public static string BackupPath => Settings.LocalResources.GetBackupPath();
    public static string CryPath => Settings.LocalResources.GetCryPath();
    private static string TemplatePath => Settings.LocalResources.GetTemplatePath();
    private static string TrainerPath => Settings.LocalResources.GetTrainerPath();
    private const string ThreadPath = "https://projectpokemon.org/pkhex/";

    public static PKHeXSettings Settings => Program.Settings;

    #endregion

    #region //// MAIN MENU FUNCTIONS ////

    private void FormLoadAddEvents()
    {
        C_SAV.Menu_Redo = Menu_Redo;
        C_SAV.Menu_Undo = Menu_Undo;
        dragout.GiveFeedback += (_, e) => e.UseDefaultCursors = false;
        GiveFeedback += (_, e) => e.UseDefaultCursors = false;
        PKME_Tabs.EnableDragDrop(Main_DragEnter, Main_DragDrop);
        C_SAV.EnableDragDrop(Main_DragEnter, Main_DragDrop);
        menuStrip1.AllowDrop = true;
        menuStrip1.DragEnter += Main_DragEnter;
        menuStrip1.DragDrop += Main_DragDrop;
        PB_Legal.AllowDrop = true;
        PB_Legal.DragEnter += Main_DragEnter;
        PB_Legal.DragDrop += Main_DragDrop;

        // ToolTips for Drag&Drop
        toolTip.SetToolTip(dragout, "Drag to Save");

        // Box to Tabs D&D
        dragout.AllowDrop = true;

        // Add ContextMenus
        var mnu = new ContextMenuPKM();
        mnu.RequestEditorLegality += ClickLegality;
        mnu.RequestEditorQR += ClickQR;
        mnu.RequestEditorSaveAs += MainMenuSave;
        dragout.ContextMenuStrip = mnu.mnuL;
        C_SAV.menu.RequestEditorLegality = DisplayLegalityReport;
    }

    public void LoadInitialFiles(StartupArguments args)
    {
        var sav = args.SAV!;
        var path = sav.Metadata.FilePath ?? string.Empty;
        OpenSAV(sav, path);

        var pk = args.Entity!;
        OpenPKM(pk);

        if (args.Error is { } ex)
            ErrorWindow.ShowErrorDialog(MsgFileLoadFailAuto, ex, true);
    }

    private void LoadBlankSaveFile(GameVersion version)
    {
        if (!version.IsValidSavedVersion())
            version = Latest.Version;
        var current = C_SAV?.SAV;
        var sav = BlankSaveFile.Get(version, current);
        OpenSAV(sav, string.Empty);
        C_SAV!.SAV.State.Edited = false; // Prevents form close warning from showing until changes are made
    }

    public async Task CheckForUpdates()
    {
        Version? latestVersion;
        // User might not be connected to the internet or with a flaky connection.
        try { latestVersion = UpdateUtil.GetLatestPKHeXVersion(); }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception while checking for latest version: {ex}");
            return;
        }
        if (latestVersion is null || latestVersion <= Program.CurrentVersion)
            return;

        while (!IsHandleCreated) // Wait for form to be ready
            await Task.Delay(2_000).ConfigureAwait(false);
        await InvokeAsync(() => NotifyNewVersionAvailable(latestVersion)).ConfigureAwait(false); // invoke on GUI thread
    }

    private void NotifyNewVersionAvailable(Version version)
    {
        var date = $"{2000 + version.Major:00}{version.Minor:00}{version.Build:00}";
        var lbl = L_UpdateAvailable;
        lbl.Text = $"{MsgProgramUpdateAvailable} {date}";
        lbl.Click += (_, _) => Process.Start(new ProcessStartInfo(ThreadPath) { UseShellExecute = true });
        lbl.Visible = lbl.TabStop = lbl.Enabled = true;
    }

    public static DrawConfig Draw { get; private set; } = new();

    private void FormInitializeSecond()
    {
        var settings = Settings;
        Draw = C_SAV.M.Hover.Draw = PKME_Tabs.Draw = settings.Draw;
        ReloadProgramSettings(settings, true);
        CB_MainLanguage.Items.AddRange(Enum.GetNames<ProgramLanguage>());
        PB_Legal.Visible = !HaX;
        C_SAV.HaX = PKME_Tabs.HaX = HaX;

#if DEBUG
        DevUtil.AddDeveloperControls(Menu_Tools, Plugins);
#endif

        // Select Language
        CB_MainLanguage.SelectedIndex = GameLanguage.GetLanguageIndex(settings.Startup.Language);
    }

    public void AttachPlugins()
    {
        var folder = Settings.LocalResources.GetPluginPath();
        if (Plugins.Count != 0)
            return; // already loaded
        if (!Directory.Exists(folder))
            return;

        try
        {
            PluginLoadResult = PluginLoader.LoadPlugins(folder, Plugins, Settings.Startup.PluginLoadMerged);
        }
        catch (InvalidCastException c)
        {
            WinFormsUtil.Error(MsgPluginFailLoad, c);
            return;
        }

        var list = Plugins.OrderBy(z => z.Priority);
        foreach (var p in list)
        {
            try
            {
                p.Initialize(C_SAV, PKME_Tabs, menuStrip1, Program.CurrentVersion);
            }
            catch (Exception ex)
            {
                WinFormsUtil.Error(MsgPluginFailLoad, ex);
                Plugins.Remove(p);
            }
        }
    }

    // Main Menu Strip UI Functions
    private void MainMenuOpen(object sender, EventArgs e)
    {
        if (WinFormsUtil.OpenSAVPKMDialog(C_SAV.SAV.PKMExtensions, out var path))
            OpenQuick(path);
    }

    private void MainMenuSave(object? sender, EventArgs e)
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
            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgConfirmQuitProgram))
                return;
        }

        Close();
    }

    private void MainMenuAbout(object sender, EventArgs e) => ShowAboutDialog(AboutPage.Shortcuts);

    public void ShowAboutDialog(AboutPage index)
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
        var list = new List<SlotCache>();
        SlotInfoLoader.AddFromSaveFile(C_SAV.SAV, list);

        var settings = Settings.Report;
        var extra = CollectionsMarshal.AsSpan(settings.ExtraProperties);
        var hide = CollectionsMarshal.AsSpan(settings.HiddenProperties);
        report.PopulateData(list, extra, hide);
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
        if (this.OpenWindowExists<SAV_Encounters>())
            return;

        var db = new TrainerDatabase();
        var sav = C_SAV.SAV;
        Task.Run(() =>
        {
            var dir = TrainerPath;
            if (!Directory.Exists(dir))
                return;
            var files = Directory.EnumerateFiles(TrainerPath, "*.*", SearchOption.AllDirectories);
            var pk = BoxUtil.GetPKMsFromPaths(files, sav.Context);
            foreach (var f in pk)
                db.RegisterCopy(f);
        });
        new SAV_Encounters(PKME_Tabs, db).Show();
    }

    private void MainMenuMysteryDB(object sender, EventArgs e)
    {
        if (!this.OpenWindowExists<SAV_MysteryGiftDB>())
            new SAV_MysteryGiftDB(PKME_Tabs, C_SAV).Show();
    }

    private static void ClosePopups()
    {
        var forms = Application.OpenForms.OfType<Form>().Where(IsPopupFormType).ToArray();
        foreach (var f in forms)
        {
            if (f.InvokeRequired)
                continue; // from another thread, not our scope.
            f.Close();
        }
    }

    private static bool IsPopupFormType(Form z) => z is not (Main or SplashScreen or SAV_FolderList or PokePreview);

    private void MainMenuSettings(object sender, EventArgs e)
    {
        var settings = Settings;
        using var form = new SettingsEditor(settings);
        form.ShowDialog();

        // Reload text (if OT details hidden)
        Text = GetProgramTitle(C_SAV.SAV);
        // Update final settings
        ReloadProgramSettings(Settings);

        if (form.BlankChanged) // changed by user
        {
            LoadBlankSaveFile(Settings.Startup.DefaultSaveVersion);
            return;
        }

        PKME_Tabs_UpdatePreviewSprite(sender, e);
        if (C_SAV.SAV.HasBox)
            C_SAV.ReloadSlots();
    }

    private void ReloadProgramSettings(PKHeXSettings settings, bool skipCore = false)
    {
        if (!skipCore)
            StartupUtil.ReloadSettings(settings);

        Draw.LoadBrushes();
        PKME_Tabs.Unicode = Unicode = settings.Display.Unicode;
        PKME_Tabs.UpdateUnicode(GenderSymbols);
        SpriteName.AllowShinySprite = settings.Sprite.ShinySprites;
        SpriteBuilderUtil.SpriterPreference = settings.Sprite.SpritePreference;

        C_SAV.ModifyPKM = PKME_Tabs.ModifyPKM = settings.SlotWrite.SetUpdatePKM;
        C_SAV.FlagIllegal = settings.Display.FlagIllegal;
        C_SAV.M.Hover.GlowHover = settings.Hover.HoverSlotGlowEdges;
        PKME_Tabs.HideSecretValues = C_SAV.HideSecretDetails = settings.Privacy.HideSecretDetails;
        WinFormsUtil.DetectSaveFileOnFileOpen = settings.Startup.TryDetectRecentSave;
        SelectablePictureBox.FocusBorderDeflate = GenderToggle.FocusBorderDeflate = settings.Display.FocusBorderDeflate;

        if (HaX)
        {
            EntityConverter.AllowIncompatibleConversion = EntityCompatibilitySetting.AllowIncompatibleAll;
        }
        SpriteBuilder.LoadSettings(settings.Sprite);
        WinFormsUtil.AddSaveFileExtensions(settings.Backup.OtherSaveFileExtensions);
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

    /// <summary>
    /// Dumps all Entity content stored in the SaveFile's boxes to disk.
    /// </summary>
    private void MainMenuBoxDump(object sender, EventArgs e)
    {
        DialogResult ld = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgDatabaseExport);
        if (ld == DialogResult.Yes)
        {
            BoxExport.Export(C_SAV.SAV, DatabasePath, BoxExportSettings.Default);
            return;
        }
        if (ld != DialogResult.No)
            return;

        using var dumper = new BoxExporter(C_SAV.SAV, BoxExporter.ExportOverride.All);
        dumper.ShowDialog();
    }

    private void MainMenuBoxDumpSingle(object sender, EventArgs e)
    {
        C_SAV.SAV.CurrentBox = C_SAV.CurrentBox; // double check
        using var dumper = new BoxExporter(C_SAV.SAV, BoxExporter.ExportOverride.Current);
        dumper.ShowDialog();
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
        var form = new SAV_FolderList(s => OpenSAV(s.Clone(), s.Metadata.FilePath!));
        form.Show();
    }

    // Misc Options
    private void ClickShowdownImportPKM(object? sender, EventArgs e)
    {
        if (!Clipboard.ContainsText())
        { WinFormsUtil.Alert(MsgClipboardFailRead); return; }

        // Get Simulator Data
        var text = Clipboard.GetText();
        var sets = BattleTemplateTeams.TryGetSets(text);
        var set = sets.FirstOrDefault() ?? new(string.Empty); // take only first set

        if (set.Species == 0)
        { WinFormsUtil.Alert(MsgSimulatorFailClipboard); return; }

        var programLanguage = Language.GetLanguageValue(Settings.Startup.Language);
        var settings = Settings.BattleTemplate.Export.GetSettings(programLanguage, set.Context);
        var reformatted = set.GetText(settings);
        if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgSimulatorLoad, reformatted))
            return;

        var invalid = set.InvalidLines;
        if (invalid.Count != 0)
        {
            var localization = BattleTemplateParseErrorLocalization.Get(CurrentLanguage);
            var sb = new System.Text.StringBuilder();
            foreach (var line in invalid)
            {
                var error = line.Humanize(localization);
                sb.AppendLine(error);
            }
            WinFormsUtil.Alert(MsgSimulatorInvalid, sb.ToString());
        }
        PKME_Tabs.LoadShowdownSet(set);
    }

    private void ClickShowdownExportPKM(object sender, EventArgs e)
    {
        if (!PKME_Tabs.EditsComplete)
        {
            WinFormsUtil.Alert(MsgSimulatorExportBadFields);
            return;
        }

        var pk = PreparePKM();
        var programLanguage = Language.GetLanguageValue(Settings.Startup.Language);
        var settings = Settings.BattleTemplate.Export.GetSettings(programLanguage, pk.Context);
        var text = ShowdownParsing.GetShowdownText(pk, settings);
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
        catch (Exception e) { WinFormsUtil.Error(MsgFileInUse + path, e); return; }

        string ext = fi.Extension;
#if DEBUG
        OpenFile(input, path, ext);
#else
        try { OpenFile(input, path, ext); }
        catch (Exception e) { WinFormsUtil.Error(MsgFileLoadFail + "\nPath: " + path, e); }
#endif
    }

    internal void OpenFile(Memory<byte> input, string path, string ext)
    {
        var obj = FileUtil.GetSupportedFile(input, ext, C_SAV.SAV);
        if (obj is not null && LoadFile(obj, path))
            return;

        WinFormsUtil.Error(GetHintInvalidFile(input.Span, path),
            $"{MsgFileLoad}{Environment.NewLine}{path}",
            $"{string.Format(MsgFileSize, input.Length)}{Environment.NewLine}{input.Length} bytes (0x{input.Length:X4})");
    }

    private static string GetHintInvalidFile(ReadOnlySpan<byte> input, string path)
    {
        bool isSAV = WinFormsUtil.IsFileExtensionSAV(path);
        if (!isSAV)
            return MsgPKMUnsupported;

        // Include a hint for the user to check if the file is all 00 or all FF
        bool allZero = !input.ContainsAnyExcept<byte>(0x00);
        if (allZero)
            return MsgFileLoadAllZero;
        bool allFF = !input.ContainsAnyExcept<byte>(0xFF);
        if (allFF)
            return MsgFileLoadAllFFFF;

        return MsgFileUnsupported;
    }

    private bool LoadFile(object? input, string path)
    {
        if (input is null)
            return false;

        switch (input)
        {
            case PKM pk: return OpenPKM(pk);
            case SaveFile s: return OpenSAV(s, path);
            case IPokeGroup b: return OpenGroup(b);
            case MysteryGift g: return OpenMysteryGift(g, path);
            case ConcatenatedEntitySet pkms: return OpenPCBoxBin(pkms);
            case IEncounterConvertible enc: return OpenPKM(enc.ConvertToPKM(C_SAV.SAV));

            case SAV3GCMemoryCard gc:
                if (!CheckGCMemoryCard(gc, path))
                    return true;
                if (!SaveUtil.TryGetSaveFile(gc, out var mcsav))
                    return false;
                mcsav.Metadata.SetExtraInfo(path);
                return OpenSAV(mcsav, path);
        }
        return false;
    }

    private bool OpenPKM(PKM pk)
    {
        var sav = C_SAV.SAV;
        var destType = sav.PKMType;
        var tmp = EntityConverter.ConvertToType(pk, destType, out var c);
        Debug.WriteLine(c.GetDisplayString(pk, destType));
        if (tmp is null)
            return false;

        var unconverted = ReferenceEquals(pk, tmp);
        if (unconverted && sav is { State.Exportable: true })
            sav.AdaptToSaveFile(tmp);
        PKME_Tabs.PopulateFields(tmp);
        return true;
    }

    private bool OpenGroup(IPokeGroup b)
    {
        bool result = C_SAV.OpenGroup(b, out var msg);
        if (!string.IsNullOrWhiteSpace(msg))
            WinFormsUtil.Alert(msg);
        Debug.WriteLine(msg);
        return result;
    }

    private bool OpenMysteryGift(MysteryGift tg, string path)
    {
        if (!tg.IsEntity)
        {
            WinFormsUtil.Alert(MsgPKMMysteryGiftFail, path);
            return true;
        }

        var temp = tg.ConvertToPKM(C_SAV.SAV);
        var destType = C_SAV.SAV.PKMType;
        var pk = EntityConverter.ConvertToType(temp, destType, out var c);

        if (pk is null)
        {
            WinFormsUtil.Alert(c.GetDisplayString(temp, destType));
            return true;
        }

        C_SAV.SAV.AdaptToSaveFile(pk);
        PKME_Tabs.PopulateFields(pk);
        Debug.WriteLine(c);
        return true;
    }

    private bool OpenPCBoxBin(ConcatenatedEntitySet pkms)
    {
        if (C_SAV.IsBoxDragActive)
            return true;
        Cursor = Cursors.Default;
        if (!C_SAV.OpenPCBoxBin(pkms.Data.Span, out var msg))
        {
            WinFormsUtil.Alert(MsgFileLoadIncompatible, msg);
            return true;
        }

        WinFormsUtil.Alert(msg);
        return true;
    }

    private static SaveFileType SelectMemoryCardSaveGame(SAV3GCMemoryCard memCard)
    {
        if (memCard.SaveGameCount == 1)
            return memCard.SelectedGameVersion;

        var games = GetMemoryCardGameSelectionList(memCard);
        var dialog = new SAV_GameSelect(games, MsgFileLoadSaveMultiple, MsgFileLoadSaveSelectGame);
        dialog.ShowDialog();
        return (SaveFileType)dialog.Result;
    }

    private static List<ComboItem> GetMemoryCardGameSelectionList(SAV3GCMemoryCard memCard)
    {
        var games = new List<ComboItem>();
        if (memCard.HasCOLO) games.Add(new ComboItem(MsgGameColosseum, (int)SaveFileType.Colosseum));
        if (memCard.HasXD) games.Add(new ComboItem(MsgGameXD, (int)SaveFileType.XD));
        if (memCard.HasRSBOX) games.Add(new ComboItem(MsgGameRSBOX, (int)SaveFileType.RSBox));
        return games;
    }

    private static bool CheckGCMemoryCard(SAV3GCMemoryCard memCard, string path)
    {
        var state = memCard.GetMemoryCardState();
        switch (state)
        {
            case MemoryCardSaveStatus.NoPkmSaveGame:
                WinFormsUtil.Error(MsgFileGameCubeNoGames, path);
                return false;

            case MemoryCardSaveStatus.DuplicateCOLO:
            case MemoryCardSaveStatus.DuplicateXD:
            case MemoryCardSaveStatus.DuplicateRSBOX:
                WinFormsUtil.Error(MsgFileGameCubeDuplicate, path);
                return false;

            case MemoryCardSaveStatus.MultipleSaveGame:
                var game = SelectMemoryCardSaveGame(memCard);
                if (game == 0) // Cancel
                    return false;
                memCard.SelectSaveGame(game);
                break;

            case MemoryCardSaveStatus.SaveGameCOLO: memCard.SelectSaveGame(SaveFileType.Colosseum); break;
            case MemoryCardSaveStatus.SaveGameXD: memCard.SelectSaveGame(SaveFileType.XD); break;
            case MemoryCardSaveStatus.SaveGameRSBOX: memCard.SelectSaveGame(SaveFileType.RSBox); break;

            default:
                WinFormsUtil.Error(!SAV3GCMemoryCard.IsMemoryCardSize(memCard.Data.Length) ? MsgFileGameCubeBad : GetHintInvalidFile(memCard.Data, path), path);
                return false;
        }
        return true;
    }

    private static void StoreLegalSaveGameData(SaveFile sav)
    {
        if (sav is SAV3 sav3)
            EReaderBerrySettings.LoadFrom(sav3);
    }

    private bool OpenSAV(SaveFile sav, string path)
    {
        if (ModifierKeys == Keys.Alt)
        {
            SaveTypeInfo other = default;
            if (SaveUtil.TryOverride(sav, other, out var replace))
                sav = replace;
        }
        if (!sav.IsVersionValid())
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

        ClosePopups();

        PKME_Tabs.Focus(); // flush any pending changes
        StoreLegalSaveGameData(sav);
        ParseSettings.InitFromSaveFileData(sav); // physical GB, no longer used in logic
        RecentTrainerCache.SetRecentTrainer(sav);
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
        CheckLoadPath(path);

        Menu_ShowdownExportParty.Visible = sav.HasParty;
        Menu_ShowdownExportCurrentBox.Visible = sav.HasBox;

        Settings.Startup.LoadSaveFile(path);
        if (Settings.Sounds.PlaySoundSAVLoad)
            SystemSounds.Asterisk.Play();
        return true;
    }

    private void ResetSAVPKMEditors(SaveFile sav)
    {
        C_SAV.SetEditEnvironment(new SaveDataEditor<PictureBox>(sav, PKME_Tabs));

        var pk = sav.LoadTemplate(TemplatePath);
        PKME_Tabs.CurrentPKM = pk;

        bool init = PKME_Tabs.IsInitialized;
        if (!init)
        {
            PKME_Tabs.InitializeBinding();
            PKME_Tabs.SetPKMFormatMode(pk);
            PKME_Tabs.ChangeLanguage(sav);
        }
        else
        {
            PKME_Tabs.SetPKMFormatMode(pk);
        }
        PKME_Tabs.PopulateFields(pk);

        // Initialize Overall Info
        Menu_LoadBoxes.Enabled = Menu_DumpBoxes.Enabled = Menu_DumpBox.Enabled = Menu_Report.Enabled = C_SAV.SAV.HasBox;

        // Initialize Subviews
        bool WindowTranslationRequired = false;
        WindowTranslationRequired |= PKME_Tabs.ToggleInterface(sav, pk);
        WindowTranslationRequired |= C_SAV.ToggleInterface();
        if (WindowTranslationRequired) // force update -- re-added controls may be untranslated
            WinFormsUtil.TranslateInterface(this, CurrentLanguage);

        PKME_Tabs.PopulateFields(pk);

        sav.State.Edited = false;
        foreach (var p in Plugins)
            p.NotifySaveLoaded();
    }

    private static string GetProgramTitle()
    {
#if DEBUG
        // Get the file path that started this exe.
        var path = Environment.ProcessPath;
        var date = path is null ? DateTime.Now : File.GetLastWriteTime(path);
        string version = $"d-{date:yyyyMMdd}";
#else
        var v = Program.CurrentVersion;
        string version = $"{2000+v.Major:00}{v.Minor:00}{v.Build:00}";
#endif
        return $"PKH{(HaX ? "a" : "e")}X ({version})";
    }

    private static string GetProgramTitle(SaveFile sav)
    {
        string title = GetProgramTitle() + $" - {sav.GetType().Name}: ";
        if (sav is ISaveFileRevision rev)
            title = title.Insert(title.Length - 2, rev.SaveRevisionString);
        var version = GameInfo.GetVersionName(sav.Version);
        if (Settings.Privacy.HideSAVDetails)
            return title + $"[{version}]";
        if (!sav.State.Exportable) // Blank save file
            return title + $"{sav.Metadata.FileName} [{sav.OT} ({version})]";
        return title + Path.GetFileNameWithoutExtension(PathUtil.CleanFileName(sav.Metadata.BAKName)); // more descriptive
    }

    private static bool TryBackupExportCheck(SaveFile sav, string path)
    {
        // If backup folder exists, save a backup.
        if (string.IsNullOrWhiteSpace(path))
            return false; // not actual save
        if (!Settings.Backup.BAKEnabled)
            return false;
        if (!sav.State.Exportable)
            return false; // not actual save
        var dir = BackupPath;
        if (!Directory.Exists(dir))
            return false;

        var meta = sav.Metadata;
        var backupName = meta.GetBackupFileName(dir);
        if (File.Exists(backupName))
            return false; // Already backed up.

        // Ensure the file we are copying exists.
        var src = meta.FilePath;
        if (src is null || !File.Exists(src))
            return false;

        try
        {
            // Don't need to force overwrite, but on the off-chance it was written externally, we force ours.
            File.Copy(src, backupName, true);
            return true;
        }
        catch (Exception ex)
        {
            WinFormsUtil.Error(MsgBackupUnable, ex);
            return false;
        }
    }

    private static bool CheckLoadPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false; // not actual save
        if (!FileUtil.IsFileLocked(path))
            return true;

        WinFormsUtil.Alert(MsgFileWriteProtected + Environment.NewLine + path, MsgFileWriteProtectedAdvice);
        return false;
    }

    private static bool SanityCheckSAV(ref SaveFile sav)
    {
        if (sav.Generation <= 3)
            SaveLanguage.TryRevise(sav);

        if (sav.State.Exportable && sav is SAV3 s3)
        {
            if (ModifierKeys == Keys.Control || s3.IsCorruptPokedexFF())
            {
                var games = GetGameList([GameVersion.R, GameVersion.S, GameVersion.E, GameVersion.FR, GameVersion.LG]);
                var msg = string.Format(MsgFileLoadVersionDetect, $"3 ({s3.Version})");
                using var dialog = new SAV_GameSelect(games, msg, MsgFileLoadSaveSelectVersion);
                dialog.ShowDialog();
                if (dialog.Result is 0)
                    return false;

                var game = (GameVersion)dialog.Result;
                var s = s3.ForceLoad(game);
                if (s is SAV3FRLG frlg)
                {
                    // Try to give the correct Deoxys form stats (different in R/S, E, FR and LG)
                    bool result = frlg.ResetPersonal(game);
                    if (!result)
                        return false;
                }
                var origin = sav.Metadata.FilePath;
                if (origin is not null)
                    s.Metadata.SetExtraInfo(origin);
                sav = s;
            }
            else if (s3 is SAV3FRLG frlg && !frlg.Version.IsValidSavedVersion()) // IndeterminateSubVersion
            {
                string fr = GameInfo.GetVersionName(GameVersion.FR);
                string lg = GameInfo.GetVersionName(GameVersion.LG);
                string dual = "{1}/{2} " + MsgFileLoadVersionDetect;
                var games = GetGameList([GameVersion.FR, GameVersion.LG]);
                var msg = string.Format(dual, "3", fr, lg);
                using var dialog = new SAV_GameSelect(games, msg, MsgFileLoadSaveSelectVersion);
                dialog.ShowDialog();
                var game = (GameVersion)dialog.Result;
                bool result = frlg.ResetPersonal(game);
                if (!result)
                    return false;
            }
        }

        return true;

        static ComboItem[] GetGameList(ReadOnlySpan<GameVersion> g)
        {
            var result = new ComboItem[g.Length];
            for (int i = 0; i < g.Length; i++)
            {
                int id = (int)g[i];
                result[i] = GameInfo.Sources.VersionDataSource.First(v => v.Value == id);
            }
            return result;
        }
    }

    public static void SetCountrySubRegion(ComboBox cb, string type)
    {
        // Try to retain previous selection index. If triggered by language change, the list will be reloaded.
        int index = cb.SelectedIndex;
        string cl = GameInfo.CurrentLanguage;
        cb.DataSource = Util.GetCountryRegionList(type, cl);

        if (index > 0 && index < cb.Items.Count)
            cb.SelectedIndex = index;
    }

    // Language Translation
    private void ChangeMainLanguage(object sender, EventArgs e)
    {
        var index = CB_MainLanguage.SelectedIndex;
        if ((uint)index < CB_MainLanguage.Items.Count)
            CurrentLanguage = GameLanguage.LanguageCode(index);

        var lang = CurrentLanguage;
        Settings.Startup.Language = lang;
        WinFormsUtil.SetCultureLanguage(lang);

        Menu_Options.DropDown.Close();

        var sav = C_SAV.SAV;
        LocalizeUtil.InitializeStrings(lang, sav, HaX);
        WinFormsUtil.TranslateInterface(this, lang); // Translate the UI to language.
        LocalizedDescriptionAttribute.Localizer = WinFormsTranslator.GetDictionary(lang);

        SizeCP.ResetSizeLocalizations(lang);
        PKME_Tabs.SizeCP.TryResetStats();

        if (sav is not FakeSaveFile)
        {
            var pk = PKME_Tabs.CurrentPKM.Clone();

            PKME_Tabs.ChangeLanguage(sav);
            PKME_Tabs.PopulateFields(pk); // put data back in form
            Text = GetProgramTitle(sav);
        }

        foreach (var plugin in Plugins)
            plugin.NotifyDisplayLanguageChanged(lang);
    }
    #endregion

    #region //// PKX WINDOW FUNCTIONS ////
    private bool QR6Notified;

    private void ClickQR(object? sender, EventArgs e)
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

        if (dragout.Image is not Bitmap sprite)
            return;
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

    private void ClickLegality(object? sender, EventArgs e)
    {
        if (!PKME_Tabs.EditsComplete)
        { SystemSounds.Hand.Play(); return; }

        var pk = PreparePKM();

        if (pk.Species == 0 || !pk.ChecksumValid)
        { SystemSounds.Hand.Play(); return; }

        var la = new LegalityAnalysis(pk, C_SAV.SAV.Personal);
        PKME_Tabs.UpdateLegality(la);
        DisplayLegalityReport(la);
    }

    private static void DisplayLegalityReport(LegalityAnalysis la)
    {
        bool verbose = ModifierKeys == Keys.Control ^ Settings.Display.ExportLegalityAlwaysVerbose;
        var report = la.Report(CurrentLanguage, verbose);
        if (verbose)
        {
            if (Settings.Display.ExportLegalityNeverClipboard)
            {
                WinFormsUtil.Alert(report);
                return;
            }
            var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, report, MsgClipboardLegalityExport);
            if (dr != DialogResult.Yes)
                return;
            var enc = la.EncounterOriginal.GetTextLines(Settings.Display.ExportLegalityVerboseProperties);
            report += Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine, enc);
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

        var menu = dragout.ContextMenuStrip;
        if (menu is not null)
            menu.Enabled = pk.Species != 0 || HaX; // Species

        pb.Image = pk.Sprite(C_SAV.SAV);
        if (pb.BackColor == SlotUtil.BadDataColor)
            pb.BackColor = SlotUtil.GoodDataColor;
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
        bool isValid = (sender as bool?) != false;
        PB_Legal.Image = SpriteUtil.GetLegalIndicator(isValid);
        toolTip.SetToolTip(PB_Legal, isValid ? MsgLegalityHoverValid : MsgLegalityHoverInvalid);
    }

    private void PKME_Tabs_RequestShowdownExport(object sender, EventArgs e) => ClickShowdownExportPKM(sender, e);
    private void PKME_Tabs_RequestShowdownImport(object sender, EventArgs e) => ClickShowdownImportPKM(sender, e);
    private SaveFile PKME_Tabs_SaveFileRequested(object sender, EventArgs e) => C_SAV.SAV;
    private PKM PreparePKM(bool click = true) => PKME_Tabs.PreparePKM(click);

    // Drag & Drop Events
    private static void Main_DragEnter(object? sender, DragEventArgs? e)
    {
        if (e is null)
            return;
        if (e.AllowedEffect == (DragDropEffects.Copy | DragDropEffects.Link)) // external file
            e.Effect = DragDropEffects.Copy;
        else if (e.Data is not null) // within
            e.Effect = DragDropEffects.Copy;
    }

    private void Main_DragDrop(object? sender, DragEventArgs? e)
    {
        if (e?.Data?.GetData(DataFormats.FileDrop) is not string[] { Length: not 0 } files)
            return;
        OpenQuick(files[0]);
        e.Effect = DragDropEffects.Copy;
    }

    // ReSharper disable once AsyncVoidMethod
    private async void Dragout_MouseDown(object sender, MouseEventArgs e)
    {
        try
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

            // Gather data
            var pk = PreparePKM();
            var encrypt = ModifierKeys == Keys.Control;
            var data = encrypt ? pk.EncryptedPartyData : pk.DecryptedPartyData;

            // Create Temp File to Drag
            var newfile = FileUtil.GetPKMTempFileName(pk, encrypt);
            try
            {
                await File.WriteAllBytesAsync(newfile, data).ConfigureAwait(true);

                var pb = (PictureBox)sender;
                if (pb.Image is Bitmap img)
                    C_SAV.M.Drag.Info.Cursor = Cursor = new Cursor(img.GetHicon());

                DoDragDrop(new DataObject(DataFormats.FileDrop, new[] { newfile }), DragDropEffects.Copy);
            }
            // Tons of things can happen with drag & drop; don't try to handle things, just indicate failure.
            catch (Exception x)
            { WinFormsUtil.Error("Drag && Drop Error", x); }
            finally
            {
                C_SAV.M.Drag.ResetCursor(this);
                await DeleteAsync(newfile, 20_000).ConfigureAwait(false);
            }
        }
        catch
        {
            // Ignore.
        }
    }

    private static async Task DeleteAsync(string path, int delay)
    {
        await Task.Delay(delay).ConfigureAwait(true);
        if (!File.Exists(path))
            return;

        try { File.Delete(path); }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
    }

    private void Dragout_DragOver(object sender, DragEventArgs e) => e.Effect = DragDropEffects.Copy;

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

    private void DragoutDrop(object? sender, DragEventArgs? e)
    {
        if (e?.Data?.GetData(DataFormats.FileDrop) is not string[] { Length: not 0 } files)
            return;
        OpenQuick(files[0]);
        e.Effect = DragDropEffects.Copy;

        Cursor = DefaultCursor;
    }

    private async void Main_FormClosing(object sender, FormClosingEventArgs e)
    {
        try
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

            await PKHeXSettings.SaveSettings(Program.PathConfig, Settings).ConfigureAwait(false);
        }
        catch
        {
            // Ignore; program is shutting down.
        }
    }

    #endregion

    #region //// SAVE FILE FUNCTIONS ////

    private void ClickExportSAV(object sender, EventArgs e)
    {
        if (!Menu_ExportSAV.Enabled)
            return; // hot-keys can't cheat the system!

        C_SAV.ExportSaveFile();
        Text = GetProgramTitle(C_SAV.SAV);
    }

    private void ClickSaveFileName(object sender, EventArgs e)
    {
        try
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            if (!SaveFinder.TryDetectSaveFile(cts.Token, out var sav))
                return;

            var path = sav.Metadata.FilePath!;
            var time = new FileInfo(path).CreationTime;
            var timeStamp = time.ToString(CultureInfo.CurrentCulture);
            if (WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgFileLoadSaveDetectReload, path, timeStamp) == DialogResult.Yes)
                LoadFile(sav, path); // load save
        }
        catch (Exception ex)
        {
            WinFormsUtil.Error(ex.Message); // `path` contains the error message
        }
    }

    public void PromptBackup(string folder)
    {
        if (Directory.Exists(folder))
            return;
        if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, string.Format(MsgBackupCreateLocation, folder), MsgBackupCreateQuestion))
            return;

        try
        {
            Directory.CreateDirectory(folder);
            WinFormsUtil.Alert(MsgBackupSuccess, string.Format(MsgBackupDelete, folder));
        }
        catch (Exception ex)
        // Maybe they put their exe in a folder that we can't create files/folders to.
        { WinFormsUtil.Error($"{MsgBackupUnable} @ {folder}", ex); }
    }

    private void ClickUndo(object sender, EventArgs e) => C_SAV.ClickUndo();
    private void ClickRedo(object sender, EventArgs e) => C_SAV.ClickRedo();
    #endregion

    public void WarnBehavior()
    {
        WinFormsUtil.Alert(MsgProgramIllegalModeActive, MsgProgramIllegalModeBehave);
    }
}
