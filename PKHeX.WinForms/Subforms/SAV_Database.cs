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
using PKHeX.Drawing;
using PKHeX.WinForms.Controls;
using PKHeX.WinForms.Properties;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms
{
    public partial class SAV_Database : Form
    {
        private readonly SaveFile SAV;
        private readonly SAVEditor BoxView;
        private readonly PKMEditor PKME_Tabs;

        public SAV_Database(PKMEditor f1, SAVEditor saveditor)
        {
            InitializeComponent();

            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

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
        private List<PKM> Results = new();
        private List<PKM> RawDB = new();
        private int slotSelected = -1; // = null;
        private Image? slotColor;
        private const int RES_MAX = 66;
        private const int RES_MIN = 6;
        private readonly string Counter;
        private readonly string Viewed;
        private const int MAXFORMAT = PKX.Generation;
        private readonly SummaryPreviewer ShowSet = new();

        // Important Events
        private void ClickView(object sender, EventArgs e)
        {
            var pb = WinFormsUtil.GetUnderlyingControl<PictureBox>(sender);
            int index = Array.IndexOf(PKXBOXES, pb);
            if (!GetShiftedIndex(ref index))
            {
                System.Media.SystemSounds.Exclamation.Play();
                return;
            }

            PKME_Tabs.PopulateFields(Results[index], false);
            slotSelected = index;
            slotColor = SpriteUtil.Spriter.View;
            FillPKXBoxes(SCR_Box.Value);
            L_Viewed.Text = string.Format(Viewed, Results[index].Identifier);
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

            var pk = Results[index];
            var path = pk.Identifier;

#if LOADALL
            if (path.StartsWith(EXTERNAL_SAV))
            {
                WinFormsUtil.Alert(MsgDBDeleteFailBackup);
                return;
            }
#endif
            if (path?.Contains(Path.DirectorySeparatorChar) == true)
            {
                // Data from Database: Delete file from disk
                if (File.Exists(path))
                    File.Delete(path);
            }
            else
            {
                // Data from Box: Delete from save file
                int box = pk.Box-1;
                int slot = pk.Slot-1;
                var change = new SlotInfoBox(box, slot);
                var pkSAV = change.Read(SAV);

                if (!pkSAV.DecryptedBoxData.SequenceEqual(pk.DecryptedBoxData)) // data still exists in SAV, unmodified
                {
                    WinFormsUtil.Error(MsgDBDeleteFailModified, MsgDBDeleteFailWarning);
                    return;
                }
                BoxView.EditEnv.Slots.Delete(change);
            }
            // Remove from database.
            RawDB.Remove(pk);
            Results.Remove(pk);
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

            if (RawDB.Any(p => p.Identifier == path))
            {
                WinFormsUtil.Alert(MsgDBAddFailExistsFile);
                return;
            }

            File.WriteAllBytes(path, pk.DecryptedBoxData);
            pk.Identifier = path;

            int pre = RawDB.Count;
            RawDB.Add(pk);
            RawDB = new List<PKM>(RawDB);
            int post = RawDB.Count;
            if (pre == post)
            { WinFormsUtil.Alert(MsgDBAddFailExistsPKM); return; }
            Results.Add(pk);

            // Refresh database view.
            L_Count.Text = string.Format(Counter, Results.Count);
            slotSelected = Results.Count - 1;
            slotColor = SpriteUtil.Spriter.Set;
            if ((SCR_Box.Maximum+1)*6 < Results.Count)
                SCR_Box.Maximum++;
            SCR_Box.Value = Math.Max(0, SCR_Box.Maximum - (PKXBOXES.Length/6) + 1);
            FillPKXBoxes(SCR_Box.Value);
            WinFormsUtil.Alert(MsgDBAddFromTabsSuccess);
        }

        private bool GetShiftedIndex(ref int index)
        {
            if (index >= RES_MAX)
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

            var Any = new ComboItem(MsgAny, -1);

            var DS_Species = new List<ComboItem>(GameInfo.SpeciesDataSource);
            DS_Species.RemoveAt(0); DS_Species.Insert(0, Any); CB_Species.DataSource = DS_Species;

            var DS_Item = new List<ComboItem>(GameInfo.ItemDataSource);
            DS_Item.Insert(0, Any); CB_HeldItem.DataSource = DS_Item;

            var DS_Nature = new List<ComboItem>(GameInfo.NatureDataSource);
            DS_Nature.Insert(0, Any); CB_Nature.DataSource = DS_Nature;

            var DS_Ability = new List<ComboItem>(GameInfo.AbilityDataSource);
            DS_Ability.Insert(0, Any); CB_Ability.DataSource = DS_Ability;

            var DS_Version = new List<ComboItem>(GameInfo.VersionDataSource);
            DS_Version.Insert(0, Any); CB_GameOrigin.DataSource = DS_Version;

            string[] hptypes = new string[GameInfo.Strings.types.Length - 2]; Array.Copy(GameInfo.Strings.types, 1, hptypes, 0, hptypes.Length);
            var DS_Type = Util.GetCBList(hptypes);
            DS_Type.Insert(0, Any); CB_HPType.DataSource = DS_Type;

            // Set the Move ComboBoxes too..
            var DS_Move = new List<ComboItem>(GameInfo.MoveDataSource);
            DS_Move.RemoveAt(0); DS_Move.Insert(0, Any);
            {
                foreach (ComboBox cb in new[] { CB_Move1, CB_Move2, CB_Move3, CB_Move4 })
                {
                    cb.InitializeBinding();
                    cb.DataSource = new BindingSource(DS_Move, null);
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
            reportGrid.PopulateData(Results.ToArray());
        }

        private void LoadDatabase()
        {
            var settings = Main.Settings;
            var otherPaths = new List<string>();
            if (settings.EntityDb.SearchBackups)
                otherPaths.Add(Main.BackupPath);

            if (settings.EntityDb.SearchExtraSaves)
                otherPaths.AddRange(settings.Backup.OtherBackupPaths.Where(Directory.Exists));

            RawDB = LoadPKMSaves(DatabasePath, SAV, otherPaths, settings.EntityDb.SearchExtraSavesDeep);

            // Load stats for pkm who do not have any
            foreach (var pk in RawDB.Where(z => z.Stat_Level == 0))
            {
                pk.Stat_Level = pk.CurrentLevel;
                pk.SetStats(pk.GetStats(pk.PersonalInfo));
            }

            try
            {
                while (!IsHandleCreated) { }
                BeginInvoke(new MethodInvoker(() => SetResults(RawDB)));
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch { /* Window Closed? */ }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private static List<PKM> LoadPKMSaves(string pkmdb, SaveFile SAV, IEnumerable<string> otherPaths, bool otherDeep)
        {
            var dbTemp = new ConcurrentBag<PKM>();
            var extensions = new HashSet<string>(PKM.Extensions.Select(z => $".{z}"));

            var files = Directory.EnumerateFiles(pkmdb, "*", SearchOption.AllDirectories);
            Parallel.ForEach(files, file => TryAddPKMsFromFolder(dbTemp, file, SAV, extensions));

            foreach (var folder in otherPaths)
            {
                if (!SaveUtil.GetSavesFromFolder(folder, otherDeep, out IEnumerable<string> result))
                    continue;

                var prefix = Path.GetDirectoryName(folder) + Path.DirectorySeparatorChar;
                Parallel.ForEach(result, file => TryAddPKMsFromSaveFilePath(dbTemp, file, prefix));
            }

            // Fetch from save file
            var savpkm = SAV.BoxData.Where(pk => pk.Species != 0);

            var bakpkm = dbTemp.Where(pk => pk.Species != 0).OrderBy(pk => pk.Identifier);
            var db = bakpkm.Concat(savpkm).Where(pk => pk.ChecksumValid && pk.Sanity == 0);

            if (Main.Settings.EntityDb.FilterUnavailableSpecies)
            {
                db = SAV is SAV8SWSH
                    ? db.Where(z => z is PK8 || ((PersonalInfoSWSH) PersonalTable.SWSH.GetFormEntry(z.Species, z.Form)).IsPresentInGame)
                    : db.Where(z => z is not PK8);
            }

            // Finalize the Database
            return new List<PKM>(db);
        }

        private static void TryAddPKMsFromFolder(ConcurrentBag<PKM> dbTemp, string file, ITrainerInfo dest, ICollection<string> validExtensions)
        {
            var fi = new FileInfo(file);
            if (!validExtensions.Contains(fi.Extension) || !PKX.IsPKM(fi.Length))
                return;

            var data = File.ReadAllBytes(file);
            var prefer = PKX.GetPKMFormatFromExtension(fi.Extension, dest.Generation);
            var pk = PKMConverter.GetPKMfromBytes(data, prefer);
            if (pk?.Species is not > 0)
                return;
            pk.Identifier = file;
            dbTemp.Add(pk);
        }

        private static void TryAddPKMsFromSaveFilePath(ConcurrentBag<PKM> dbTemp, string file, string externalFilePrefix)
        {
            var sav = SaveUtil.GetVariantSAV(file);
            if (sav == null)
            {
                Console.WriteLine("Unable to load SaveFile: " + file);
                return;
            }

            var path = externalFilePrefix + Path.GetFileName(file);
            if (sav.HasBox)
            {
                foreach (var pk in sav.BoxData)
                {
                    if (pk.Species == 0)
                        continue;

                    pk.Identifier = Path.Combine(path, pk.Identifier ?? string.Empty);
                    dbTemp.Add(pk);
                }
            }

            if (sav.HasParty)
            {
                foreach (var pk in sav.PartyData)
                {
                    if (pk.Species == 0)
                        continue;

                    pk.Identifier = Path.Combine(path, pk.Identifier ?? string.Empty);
                    dbTemp.Add(pk);
                }
            }

            var extra = sav.GetExtraSlots(true);
            foreach (var x in extra)
            {
                var pk = x.Read(sav);
                if (pk.Species == 0)
                    continue;

                pk.Identifier = Path.Combine(path, pk.Identifier ?? x.Type.ToString());
                dbTemp.Add(pk);
            }
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

            foreach (PKM pkm in Results)
                File.WriteAllBytes(Path.Combine(path, Util.CleanFileName(pkm.FileName)), pkm.DecryptedPartyData);
        }

        private void Menu_Import_Click(object sender, EventArgs e)
        {
            if (!BoxView.GetBulkImportSettings(out var clearAll, out var overwrite, out var noSetb))
                return;

            int box = BoxView.Box.CurrentBox;
            int ctr = SAV.LoadBoxes(Results, out var result, box, clearAll, overwrite, noSetb);
            if (ctr <= 0)
                return;

            BoxView.SetPKMBoxes();
            BoxView.UpdateBoxViewers();
            WinFormsUtil.Alert(result);
        }

        // View Updates
        private IEnumerable<PKM> SearchDatabase()
        {
            var settings = GetSearchSettings();

            IEnumerable<PKM> res = RawDB;

            // pre-filter based on the file path (if specified)
            if (!Menu_SearchBoxes.Checked)
                res = res.Where(pk => pk.Identifier?.StartsWith(DatabasePath + Path.DirectorySeparatorChar, StringComparison.Ordinal) == true);
            if (!Menu_SearchDatabase.Checked)
            {
                res = res.Where(pk => pk.Identifier?.StartsWith(DatabasePath + Path.DirectorySeparatorChar, StringComparison.Ordinal) == false);
#if LOADALL
                res = res.Where(pk => pk.Identifier?.StartsWith(EXTERNAL_SAV, StringComparison.Ordinal) == false);
#endif
            }

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

                Species = WinFormsUtil.GetIndex(CB_Species),
                Ability = WinFormsUtil.GetIndex(CB_Ability),
                Nature = WinFormsUtil.GetIndex(CB_Nature),
                Item = WinFormsUtil.GetIndex(CB_HeldItem),

                BatchInstructions = RTB_Instructions.Lines,

                Level = int.TryParse(TB_Level.Text, out var lvl) ? lvl : null,
                SearchLevel = (SearchComparison)CB_Level.SelectedIndex,
                EVType = CB_EVTrain.SelectedIndex,
                IVType = CB_IV.SelectedIndex,
            };

            settings.AddMove(WinFormsUtil.GetIndex(CB_Move1));
            settings.AddMove(WinFormsUtil.GetIndex(CB_Move2));
            settings.AddMove(WinFormsUtil.GetIndex(CB_Move3));
            settings.AddMove(WinFormsUtil.GetIndex(CB_Move4));

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
                    _ => CloneDetectionMethod.HashDetails
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
                if (!Menu_SearchBoxes.Checked && !Menu_SearchDatabase.Checked)
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

        private void SetResults(List<PKM> res)
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
                PKXBOXES[i].Image = Results[i + begin].Sprite(SAV, -1, -1, true);
            for (int i = end; i < RES_MAX; i++)
                PKXBOXES[i].Image = null;

            for (int i = 0; i < RES_MAX; i++)
                PKXBOXES[i].BackgroundImage = SpriteUtil.Spriter.Transparent;
            if (slotSelected != -1 && slotSelected >= begin && slotSelected < begin + RES_MAX)
                PKXBOXES[slotSelected - begin].BackgroundImage = slotColor ?? SpriteUtil.Spriter.View;
        }

        // Misc Update Methods
        private void ToggleESV(object sender, EventArgs e)
        {
            L_ESV.Visible = MT_ESV.Visible = CHK_IsEgg.CheckState == CheckState.Checked;
        }

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
            var db = RawDB.Where(pk => pk.Identifier?.StartsWith(DatabasePath + Path.DirectorySeparatorChar, StringComparison.Ordinal) == true)
                .OrderByDescending(file => File.GetLastWriteTimeUtc(file.Identifier!));

            var clones = SearchUtil.GetExtraClones(db);
            foreach (var pk in clones)
            {
                var path = pk.Identifier;
                if (path == null || !File.Exists(path))
                    continue;

                try { File.Delete(path); ++deleted; }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception ex) { WinFormsUtil.Error(MsgDBDeleteCloneFail + Environment.NewLine + ex.Message + Environment.NewLine + pk.Identifier); }
#pragma warning restore CA1031 // Do not catch general exception types
            }

            if (deleted == 0)
            { WinFormsUtil.Alert(MsgDBDeleteCloneNone); return; }

            WinFormsUtil.Alert(string.Format(MsgFileDeleteCount, deleted), MsgWindowClose);
            Close();
        }

        private void L_Viewed_MouseEnter(object sender, EventArgs e) => hover.SetToolTip(L_Viewed, L_Viewed.Text);

        private void ShowHoverTextForSlot(object sender, EventArgs e)
        {
            var pb = (PictureBox)sender;
            int index = Array.IndexOf(PKXBOXES, pb);
            if (!GetShiftedIndex(ref index))
                return;

            ShowSet.Show(pb, Results[index]);
        }
    }
}
