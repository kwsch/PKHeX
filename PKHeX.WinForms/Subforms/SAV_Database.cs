using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Core.Searching;
using PKHeX.Drawing.PokeSprite;
using PKHeX.WinForms.Controls;
using PKHeX.WinForms.Properties;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms;

public partial class SAV_Database : Form
{
    private readonly SaveFile SAV;
    private readonly SAVEditor BoxView;
    private readonly PKMEditor PKME_Tabs;

    public SAV_Database(PKMEditor f1, SAVEditor saveditor)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        var UC_Builder = new EntityInstructionBuilder(() => f1.PreparePKM())
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            Width = Tab_Advanced.Width,
            Dock = DockStyle.Top,
            ReadOnly = true,
        };
        Tab_Advanced.Controls.Add(UC_Builder);

        SAV = saveditor.SAV;
        BoxView = saveditor;
        PKME_Tabs = f1;

        // Preset Filters to only show PKM available for loaded save
        CB_FormatComparator.SelectedIndex = 3; // <=

        var grid = DatabasePokeGrid;
        var smallWidth = grid.Width;
        var smallHeight = grid.Height;
        grid.InitializeGrid(6, 11, SpriteUtil.Spriter);
        grid.SetBackground(Resources.box_wp_clean);
        var newWidth = grid.Width;
        var newHeight = grid.Height;
        var wdelta = newWidth - smallWidth;
        if (wdelta != 0)
            Width += wdelta;
        var hdelta = newHeight - smallHeight;
        if (hdelta != 0)
            Height += hdelta;
        PKXBOXES = grid.Entries.ToArray();

        // Enable Scrolling when hovered over
        foreach (var slot in PKXBOXES)
        {
            // Enable Click
            slot.MouseClick += (sender, e) =>
            {
                if (sender == null)
                    return;
                switch (ModifierKeys)
                {
                    case Keys.Control: ClickView(sender, e); break;
                    case Keys.Alt: ClickDelete(sender, e); break;
                    case Keys.Shift: ClickSet(sender, e); break;
                }
            };

            slot.ContextMenuStrip = mnu;
            if (Main.Settings.Hover.HoverSlotShowText)
                slot.MouseEnter += (o, args) => ShowHoverTextForSlot(slot, args);
        }

        Counter = L_Count.Text;
        Viewed = L_Viewed.Text;
        L_Viewed.Text = string.Empty; // invisible for now
        PopulateComboBoxes();

        // Load Data
        B_Search.Enabled = false;
        L_Count.Text = "Loading...";
        var task = new Task(LoadDatabase);
        task.ContinueWith(z =>
        {
            if (!z.IsFaulted)
                return;
            Invoke((MethodInvoker)(() => L_Count.Text = "Failed."));
            if (z.Exception == null)
                return;
            WinFormsUtil.Error("Loading database failed.", z.Exception.InnerException ?? new Exception(z.Exception.Message));
        });
        task.Start();

        Menu_SearchSettings.DropDown.Closing += (sender, e) =>
        {
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                e.Cancel = true;
        };
        CB_Format.Items[0] = MsgAny;
        CenterToParent();
    }

    private readonly PictureBox[] PKXBOXES;
    private readonly string DatabasePath = Main.DatabasePath;
    private List<SlotCache> Results = new();
    private List<SlotCache> RawDB = new();
    private int slotSelected = -1; // = null;
    private Image? slotColor;
    private const int RES_MAX = 66;
    private const int RES_MIN = 6;
    private readonly string Counter;
    private readonly string Viewed;
    private const int MAXFORMAT = PKX.Generation;
    private readonly SummaryPreviewer ShowSet = new();

    private void ClickView(object sender, EventArgs e)
    {
        var pb = WinFormsUtil.GetUnderlyingControl<PictureBox>(sender);
        int index = Array.IndexOf(PKXBOXES, pb);
        if (!GetShiftedIndex(ref index))
        {
            System.Media.SystemSounds.Exclamation.Play();
            return;
        }

        if (sender == mnu)
            mnu.Hide();

        slotSelected = index;
        slotColor = SpriteUtil.Spriter.View;
        FillPKXBoxes(SCR_Box.Value);
        L_Viewed.Text = string.Format(Viewed, Results[index].Identify());
        PKME_Tabs.PopulateFields(Results[index].Entity, false);
    }

    private void ClickDelete(object sender, EventArgs e)
    {
        var pb = WinFormsUtil.GetUnderlyingControl<PictureBox>(sender);
        int index = Array.IndexOf(PKXBOXES, pb);
        if (!GetShiftedIndex(ref index))
        {
            System.Media.SystemSounds.Exclamation.Play();
            return;
        }

        var entry = Results[index];
        var pk = entry.Entity;

        if (entry.Source is SlotInfoFile f)
        {
            // Data from Database: Delete file from disk
            var path = f.Path;
            if (File.Exists(path))
                File.Delete(path);
        }
        else if (entry.Source is SlotInfoBox(var box, var slot) && entry.SAV == SAV)
        {
            // Data from Box: Delete from save file
            var change = new SlotInfoBox(box, slot);
            var pkSAV = change.Read(SAV);

            if (!pkSAV.DecryptedBoxData.SequenceEqual(pk.DecryptedBoxData)) // data still exists in SAV, unmodified
            {
                WinFormsUtil.Error(MsgDBDeleteFailModified, MsgDBDeleteFailWarning);
                return;
            }
            BoxView.EditEnv.Slots.Delete(change);
        }
        else
        {
            WinFormsUtil.Error(MsgDBDeleteFailBackup, MsgDBDeleteFailWarning);
            return;
        }
        // Remove from database.
        RawDB.Remove(entry);
        Results.Remove(entry);
        // Refresh database view.
        L_Count.Text = string.Format(Counter, Results.Count);
        slotSelected = -1;
        FillPKXBoxes(SCR_Box.Value);
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void ClickSet(object sender, EventArgs e)
    {
        // Don't care what slot was clicked, just add it to the database
        if (!PKME_Tabs.EditsComplete)
            return;

        PKM pk = PKME_Tabs.PreparePKM();
        Directory.CreateDirectory(DatabasePath);

        string path = Path.Combine(DatabasePath, Util.CleanFileName(pk.FileName));

        if (File.Exists(path))
        {
            WinFormsUtil.Alert(MsgDBAddFailExistsFile);
            return;
        }

        File.WriteAllBytes(path, pk.DecryptedBoxData);

        var info = new SlotInfoFile(path);
        var entry = new SlotCache(info, pk);
        Results.Add(entry);

        // Refresh database view.
        L_Count.Text = string.Format(Counter, Results.Count);
        slotSelected = Results.Count - 1;
        slotColor = SpriteUtil.Spriter.Set;
        if ((SCR_Box.Maximum + 1) * 6 < Results.Count)
            SCR_Box.Maximum++;
        SCR_Box.Value = Math.Max(0, SCR_Box.Maximum - (PKXBOXES.Length / 6) + 1);
        FillPKXBoxes(SCR_Box.Value);
        WinFormsUtil.Alert(MsgDBAddFromTabsSuccess);
    }

    private bool GetShiftedIndex(ref int index)
    {
        if ((uint)index >= RES_MAX)
            return false;
        index += SCR_Box.Value * RES_MIN;
        return index < Results.Count;
    }

    private void PopulateComboBoxes()
    {
        // Set the Text
        CB_HeldItem.InitializeBinding();
        CB_Species.InitializeBinding();
        CB_Ability.InitializeBinding();
        CB_Nature.InitializeBinding();
        CB_GameOrigin.InitializeBinding();
        CB_HPType.InitializeBinding();

        var comboAny = new ComboItem(MsgAny, 0);

        var species = new List<ComboItem>(GameInfo.SpeciesDataSource);
        species.RemoveAt(0);
        species.Insert(0, comboAny);
        CB_Species.DataSource = species;

        var items = new List<ComboItem>(GameInfo.ItemDataSource);
        items.Insert(0, comboAny);
        CB_HeldItem.DataSource = items;

        var natures = new List<ComboItem>(GameInfo.NatureDataSource);
        natures.Insert(0, comboAny);
        CB_Nature.DataSource = natures;

        var abilities = new List<ComboItem>(GameInfo.AbilityDataSource);
        abilities.Insert(0, comboAny);
        CB_Ability.DataSource = abilities;

        var versions = new List<ComboItem>(GameInfo.VersionDataSource);
        versions.Insert(0, comboAny);
        CB_GameOrigin.DataSource = versions;

        string[] hptypes = new string[GameInfo.Strings.types.Length - 2];
        Array.Copy(GameInfo.Strings.types, 1, hptypes, 0, hptypes.Length);
        var types = Util.GetCBList(hptypes);
        types.Insert(0, comboAny);
        CB_HPType.DataSource = types;

        // Set the Move ComboBoxes too..
        var moves = new List<ComboItem>(GameInfo.MoveDataSource);
        moves.RemoveAt(0);
        moves.Insert(0, comboAny);
        {
            foreach (ComboBox cb in new[] { CB_Move1, CB_Move2, CB_Move3, CB_Move4 })
            {
                cb.InitializeBinding();
                cb.DataSource = new BindingSource(moves, null);
            }
        }

        // Trigger a Reset
        ResetFilters(this, EventArgs.Empty);
    }

    private void ResetFilters(object sender, EventArgs e)
    {
        CHK_Shiny.Checked = CHK_IsEgg.Checked = true;
        CHK_Shiny.CheckState = CHK_IsEgg.CheckState = CheckState.Indeterminate;
        MT_ESV.Text = string.Empty;
        CB_HeldItem.SelectedIndex = 0;
        CB_Species.SelectedIndex = 0;
        CB_Ability.SelectedIndex = 0;
        CB_Nature.SelectedIndex = 0;
        CB_HPType.SelectedIndex = 0;

        CB_Level.SelectedIndex = 0;
        TB_Level.Text = string.Empty;
        CB_EVTrain.SelectedIndex = 0;
        CB_IV.SelectedIndex = 0;

        CB_Move1.SelectedIndex = CB_Move2.SelectedIndex = CB_Move3.SelectedIndex = CB_Move4.SelectedIndex = 0;

        CB_GameOrigin.SelectedIndex = 0;
        CB_Generation.SelectedIndex = 0;

        MT_ESV.Visible = L_ESV.Visible = false;
        RTB_Instructions.Clear();

        if (sender != this)
            System.Media.SystemSounds.Asterisk.Play();
    }

    private void GenerateDBReport(object sender, EventArgs e)
    {
        if (WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgDBCreateReportPrompt, MsgDBCreateReportWarning) != DialogResult.Yes)
            return;

        if (this.OpenWindowExists<ReportGrid>())
            return;

        ReportGrid reportGrid = new();
        reportGrid.Show();
        reportGrid.PopulateData(Results);
    }

    private sealed class SearchFolderDetail
    {
        public string Path { get; }
        public bool IgnoreBackupFiles { get; }

        public SearchFolderDetail(string path, bool ignoreBackupFiles)
        {
            Path = path;
            IgnoreBackupFiles = ignoreBackupFiles;
        }
    }

    private void LoadDatabase()
    {
        var settings = Main.Settings;
        var otherPaths = new List<SearchFolderDetail>();
        if (settings.EntityDb.SearchExtraSaves)
            otherPaths.AddRange(settings.Backup.OtherBackupPaths.Where(Directory.Exists).Select(z => new SearchFolderDetail(z, true)));
        if (settings.EntityDb.SearchBackups)
            otherPaths.Add(new SearchFolderDetail(Main.BackupPath, false));

        RawDB = LoadPKMSaves(DatabasePath, SAV, otherPaths, settings.EntityDb.SearchExtraSavesDeep);

        // Load stats for pk who do not have any
        foreach (var entry in RawDB)
        {
            var pk = entry.Entity;
            pk.ForcePartyData();
        }

        try
        {
            while (!IsHandleCreated) { }
            BeginInvoke(new MethodInvoker(() => SetResults(RawDB)));
        }
        catch { /* Window Closed? */ }
    }

    private static List<SlotCache> LoadPKMSaves(string pkmdb, SaveFile sav, List<SearchFolderDetail> otherPaths, bool otherDeep)
    {
        var dbTemp = new ConcurrentBag<SlotCache>();
        var extensions = new HashSet<string>(PKM.Extensions.Select(z => $".{z}"));

        var files = Directory.EnumerateFiles(pkmdb, "*", SearchOption.AllDirectories);
        Parallel.ForEach(files, file => SlotInfoLoader.AddFromLocalFile(file, dbTemp, sav, extensions));

        foreach (var folder in otherPaths)
        {
            if (!SaveUtil.GetSavesFromFolder(folder.Path, otherDeep, out IEnumerable<string> paths, folder.IgnoreBackupFiles))
                continue;

            Parallel.ForEach(paths, file => TryAddPKMsFromSaveFilePath(dbTemp, file));
        }

        // Fetch from save file
        SlotInfoLoader.AddFromSaveFile(sav, dbTemp);
        var result = new List<SlotCache>(dbTemp);
        result.RemoveAll(z => !z.IsDataValid());

        if (Main.Settings.EntityDb.FilterUnavailableSpecies)
        {
            static bool IsPresentInGameSWSH(ISpeciesForm pk) => pk is PK8 || PersonalTable.SWSH.IsPresentInGame(pk.Species, pk.Form);
            static bool IsPresentInGameBDSP(ISpeciesForm pk) => pk is PB8 || PersonalTable.BDSP.IsPresentInGame(pk.Species, pk.Form);
            static bool IsPresentInGamePLA (ISpeciesForm pk) => pk is PA8 || PersonalTable.LA  .IsPresentInGame(pk.Species, pk.Form);
            if (sav is SAV8SWSH)
                result.RemoveAll(z => !IsPresentInGameSWSH(z.Entity));
            else if (sav is SAV8BS)
                result.RemoveAll(z => !IsPresentInGameBDSP(z.Entity));
            else if (sav is SAV8LA)
                result.RemoveAll(z => !IsPresentInGamePLA(z.Entity));
        }

        var sort = Main.Settings.EntityDb.InitialSortMode;
        if (sort is DatabaseSortMode.SlotIdentity)
            result.Sort();
        else if (sort is DatabaseSortMode.SpeciesForm)
            result.Sort((first, second) => first.CompareToSpeciesForm(second));

        // Finalize the Database
        return result;
    }

    private static void TryAddPKMsFromSaveFilePath(ConcurrentBag<SlotCache> dbTemp, string file)
    {
        var sav = SaveUtil.GetVariantSAV(file);
        if (sav == null)
        {
            Debug.WriteLine("Unable to load SaveFile: " + file);
            return;
        }

        SlotInfoLoader.AddFromSaveFile(sav, dbTemp);
    }

    // IO Usage
    private void OpenDB(object sender, EventArgs e)
    {
        if (Directory.Exists(DatabasePath))
            Process.Start("explorer.exe", DatabasePath);
    }

    private void Menu_Export_Click(object sender, EventArgs e)
    {
        if (Results.Count == 0)
        { WinFormsUtil.Alert(MsgDBCreateReportFail); return; }

        if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgDBExportResultsPrompt))
            return;

        using var fbd = new FolderBrowserDialog();
        if (DialogResult.OK != fbd.ShowDialog())
            return;

        string path = fbd.SelectedPath;
        Directory.CreateDirectory(path);

        foreach (var pk in Results.Select(z => z.Entity))
            File.WriteAllBytes(Path.Combine(path, Util.CleanFileName(pk.FileName)), pk.DecryptedPartyData);
    }

    private void Menu_Import_Click(object sender, EventArgs e)
    {
        if (!BoxView.GetBulkImportSettings(out var clearAll, out var overwrite, out var noSetb))
            return;

        int box = BoxView.Box.CurrentBox;
        int ctr = SAV.LoadBoxes(Results.Select(z => z.Entity), out var result, box, clearAll, overwrite, noSetb);
        if (ctr <= 0)
            return;

        BoxView.SetPKMBoxes();
        BoxView.UpdateBoxViewers();
        WinFormsUtil.Alert(result);
    }

    // View Updates
    private IEnumerable<SlotCache> SearchDatabase()
    {
        var settings = GetSearchSettings();

        IEnumerable<SlotCache> res = RawDB;

        // pre-filter based on the file path (if specified)
        if (!Menu_SearchBoxes.Checked)
            res = res.Where(z => z.SAV != SAV);
        if (!Menu_SearchDatabase.Checked)
            res = res.Where(z => !IsIndividualFilePKMDB(z));
        if (!Menu_SearchBackups.Checked)
            res = res.Where(z => !IsBackupSaveFile(z));

        // return filtered results
        return settings.Search(res);
    }

    private SearchSettings GetSearchSettings()
    {
        var settings = new SearchSettings
        {
            Format = MAXFORMAT - CB_Format.SelectedIndex + 1, // 0->(n-1) => 1->n
            SearchFormat = (SearchComparison)CB_FormatComparator.SelectedIndex,
            Generation = CB_Generation.SelectedIndex,

            Version = WinFormsUtil.GetIndex(CB_GameOrigin),
            HiddenPowerType = WinFormsUtil.GetIndex(CB_HPType),

            Species = (ushort)WinFormsUtil.GetIndex(CB_Species),
            Ability = WinFormsUtil.GetIndex(CB_Ability),
            Nature = WinFormsUtil.GetIndex(CB_Nature),
            Item = WinFormsUtil.GetIndex(CB_HeldItem),

            BatchInstructions = RTB_Instructions.Lines,

            Level = int.TryParse(TB_Level.Text, out var lvl) ? lvl : null,
            SearchLevel = (SearchComparison)CB_Level.SelectedIndex,
            EVType = CB_EVTrain.SelectedIndex,
            IVType = CB_IV.SelectedIndex,
        };

        settings.AddMove((ushort)WinFormsUtil.GetIndex(CB_Move1));
        settings.AddMove((ushort)WinFormsUtil.GetIndex(CB_Move2));
        settings.AddMove((ushort)WinFormsUtil.GetIndex(CB_Move3));
        settings.AddMove((ushort)WinFormsUtil.GetIndex(CB_Move4));

        if (CHK_Shiny.CheckState != CheckState.Indeterminate)
            settings.SearchShiny = CHK_Shiny.CheckState == CheckState.Checked;

        if (CHK_IsEgg.CheckState != CheckState.Indeterminate)
        {
            settings.SearchEgg = CHK_IsEgg.CheckState == CheckState.Checked;
            if (int.TryParse(MT_ESV.Text, out int esv))
                settings.ESV = esv;
        }

        if (Menu_SearchLegal.Checked != Menu_SearchIllegal.Checked)
            settings.SearchLegal = Menu_SearchLegal.Checked;

        if (Menu_SearchClones.Checked)
        {
            settings.SearchClones = ModifierKeys switch
            {
                Keys.Control => CloneDetectionMethod.HashPID,
                _ => CloneDetectionMethod.HashDetails,
            };
        }

        return settings;
    }

    private async void B_Search_Click(object sender, EventArgs e)
    {
        B_Search.Enabled = false;
        var search = SearchDatabase();

        bool legalSearch = Menu_SearchLegal.Checked ^ Menu_SearchIllegal.Checked;
        bool wordFilter = ParseSettings.CheckWordFilter;
        if (wordFilter && legalSearch && WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgDBSearchLegalityWordfilter) == DialogResult.No)
            ParseSettings.CheckWordFilter = false;
        var results = await Task.Run(() => search.ToList()).ConfigureAwait(true);
        ParseSettings.CheckWordFilter = wordFilter;

        if (results.Count == 0)
        {
            if (!Menu_SearchBoxes.Checked && !Menu_SearchDatabase.Checked && !Menu_SearchBackups.Checked)
                WinFormsUtil.Alert(MsgDBSearchFail, MsgDBSearchNone);
            else
                WinFormsUtil.Alert(MsgDBSearchNone);
        }
        SetResults(results); // updates Count Label as well.
        System.Media.SystemSounds.Asterisk.Play();
        B_Search.Enabled = true;
    }

    private void UpdateScroll(object sender, ScrollEventArgs e)
    {
        if (e.OldValue != e.NewValue)
            FillPKXBoxes(e.NewValue);
    }

    private void SetResults(List<SlotCache> res)
    {
        Results = res;
        ShowSet.Clear();

        SCR_Box.Maximum = (int)Math.Ceiling((decimal)Results.Count / RES_MIN);
        if (SCR_Box.Maximum > 0) SCR_Box.Maximum--;

        slotSelected = -1; // reset the slot last viewed
        SCR_Box.Value = 0;
        FillPKXBoxes(0);

        L_Count.Text = string.Format(Counter, Results.Count);
        B_Search.Enabled = true;
    }

    private void FillPKXBoxes(int start)
    {
        if (Results.Count == 0)
        {
            for (int i = 0; i < RES_MAX; i++)
            {
                PKXBOXES[i].Image = null;
                PKXBOXES[i].BackgroundImage = null;
            }
            return;
        }
        int begin = start*RES_MIN;
        int end = Math.Min(RES_MAX, Results.Count - begin);
        for (int i = 0; i < end; i++)
            PKXBOXES[i].Image = Results[i + begin].Entity.Sprite(SAV, -1, -1, true);
        for (int i = end; i < RES_MAX; i++)
            PKXBOXES[i].Image = null;

        for (int i = 0; i < RES_MAX; i++)
            PKXBOXES[i].BackgroundImage = SpriteUtil.Spriter.Transparent;
        if (slotSelected != -1 && slotSelected >= begin && slotSelected < begin + RES_MAX)
            PKXBOXES[slotSelected - begin].BackgroundImage = slotColor ?? SpriteUtil.Spriter.View;
    }

    // Misc Update Methods
    private void ToggleESV(object sender, EventArgs e) => L_ESV.Visible = MT_ESV.Visible = CHK_IsEgg.CheckState == CheckState.Checked;

    private void ChangeLevel(object sender, EventArgs e)
    {
        if (CB_Level.SelectedIndex == 0)
            TB_Level.Text = string.Empty;
    }

    private void ChangeGame(object sender, EventArgs e)
    {
        if (CB_GameOrigin.SelectedIndex != 0)
            CB_Generation.SelectedIndex = 0;
    }

    private void ChangeGeneration(object sender, EventArgs e)
    {
        if (CB_Generation.SelectedIndex != 0)
            CB_GameOrigin.SelectedIndex = 0;
    }

    private void Menu_Exit_Click(object sender, EventArgs e) => Close();

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        if (!DatabasePokeGrid.RectangleToScreen(DatabasePokeGrid.ClientRectangle).Contains(MousePosition))
            return;
        int oldval = SCR_Box.Value;
        int newval = oldval + (e.Delta < 0 ? 1 : -1);
        if (newval >= SCR_Box.Minimum && SCR_Box.Maximum >= newval)
            FillPKXBoxes(SCR_Box.Value = newval);
    }

    private void ChangeFormatFilter(object sender, EventArgs e)
    {
        if (CB_FormatComparator.SelectedIndex == 0)
        {
            CB_Format.Visible = false; // !any
            CB_Format.SelectedIndex = 0;
        }
        else
        {
            CB_Format.Visible = true;
            int index = MAXFORMAT - SAV.Generation + 1;
            CB_Format.SelectedIndex = index < CB_Format.Items.Count ? index : 0; // SAV generation (offset by 1 for "Any")
        }
    }

    private void Menu_DeleteClones_Click(object sender, EventArgs e)
    {
        var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo,
            MsgDBDeleteCloneWarning + Environment.NewLine +
            MsgDBDeleteCloneAdvice, MsgContinue);

        if (dr != DialogResult.Yes)
            return;

        var deleted = 0;
        var db = RawDB.Where(IsIndividualFilePKMDB)
            .OrderByDescending(GetRevisedTime);

        const CloneDetectionMethod method = CloneDetectionMethod.HashDetails;
        var hasher = SearchUtil.GetCloneDetectMethod(method);
        var duplicates = SearchUtil.GetExtraClones(db, z => hasher(z.Entity));
        foreach (var entry in duplicates)
        {
            var src = entry.Source;
            var path = ((SlotInfoFile)src).Path;
            if (!File.Exists(path))
                continue;

            try { File.Delete(path); ++deleted; }
            catch (Exception ex) { WinFormsUtil.Error(MsgDBDeleteCloneFail + Environment.NewLine + ex.Message + Environment.NewLine + path); }
        }

        var boxClear = new BoxManipClearDuplicate<string>(BoxManipType.DeleteClones, pk => SearchUtil.GetCloneDetectMethod(method)(pk));
        var param = new BoxManipParam(0, SAV.BoxCount - 1);
        int count = boxClear.Execute(SAV, param);
        deleted += count;

        if (deleted == 0)
        { WinFormsUtil.Alert(MsgDBDeleteCloneNone); return; }

        WinFormsUtil.Alert(string.Format(MsgFileDeleteCount, deleted), MsgWindowClose);
        BoxView.ReloadSlots();

        Close();
    }

    private static DateTime GetRevisedTime(SlotCache arg)
    {
        var src = arg.Source;
        if (src is not SlotInfoFile f)
            return DateTime.Now;
        return File.GetLastWriteTimeUtc(f.Path);
    }

    private bool IsBackupSaveFile(SlotCache pk) => pk.SAV is not FakeSaveFile && pk.SAV != SAV;
    private bool IsIndividualFilePKMDB(SlotCache pk) => pk.Source is SlotInfoFile f && f.Path.StartsWith(DatabasePath + Path.DirectorySeparatorChar, StringComparison.Ordinal);

    private void L_Viewed_MouseEnter(object sender, EventArgs e) => hover.SetToolTip(L_Viewed, L_Viewed.Text);

    private void ShowHoverTextForSlot(object sender, EventArgs e)
    {
        var pb = (PictureBox)sender;
        int index = Array.IndexOf(PKXBOXES, pb);
        if (!GetShiftedIndex(ref index))
            return;

        ShowSet.Show(pb, Results[index].Entity);
    }
}
