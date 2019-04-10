#define LOADALL
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
using PKHeX.WinForms.Controls;
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

            ToolStripMenuItem mnuView = new ToolStripMenuItem {Name = "mnuView", Text = "View"};
            ToolStripMenuItem mnuDelete = new ToolStripMenuItem {Name = "mnuDelete", Text = "Delete" };

            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            ContextMenuStrip mnu = new ContextMenuStrip();
            mnu.Items.AddRange(new ToolStripItem[] { mnuView, mnuDelete });

            SAV = saveditor.SAV;
            BoxView = saveditor;
            PKME_Tabs = f1;

            // Preset Filters to only show PKM available for loaded save
            CB_FormatComparator.SelectedIndex = 3; // <=

            PKXBOXES = new[]
            {
                bpkx1, bpkx2, bpkx3, bpkx4, bpkx5, bpkx6,
                bpkx7, bpkx8, bpkx9, bpkx10,bpkx11,bpkx12,
                bpkx13,bpkx14,bpkx15,bpkx16,bpkx17,bpkx18,
                bpkx19,bpkx20,bpkx21,bpkx22,bpkx23,bpkx24,
                bpkx25,bpkx26,bpkx27,bpkx28,bpkx29,bpkx30,

                bpkx31,bpkx32,bpkx33,bpkx34,bpkx35,bpkx36,
                bpkx37,bpkx38,bpkx39,bpkx40,bpkx41,bpkx42,
                bpkx43,bpkx44,bpkx45,bpkx46,bpkx47,bpkx48,
                bpkx49,bpkx50,bpkx51,bpkx52,bpkx53,bpkx54,
                bpkx55,bpkx56,bpkx57,bpkx58,bpkx59,bpkx60,
                bpkx61,bpkx62,bpkx63,bpkx64,bpkx65,bpkx66,
            };

            // Enable Scrolling when hovered over
            foreach (var slot in PKXBOXES)
            {
                // Enable Click
                slot.MouseClick += (sender, e) =>
                {
                    if (ModifierKeys == Keys.Control)
                        ClickView(sender, e);
                    else if (ModifierKeys == Keys.Alt)
                        ClickDelete(sender, e);
                    else if (ModifierKeys == Keys.Shift)
                        ClickSet(sender, e);
                };
            }

            Counter = L_Count.Text;
            Viewed = L_Viewed.Text;
            L_Viewed.Text = string.Empty; // invis for now
            var hover = new ToolTip();
            L_Viewed.MouseEnter += (sender, e) => hover.SetToolTip(L_Viewed, L_Viewed.Text);
            PopulateComboBoxes();

            // Assign event handlers
            mnuView.Click += ClickView;
            mnuDelete.Click += ClickDelete;

            // Add to main context menu

            // Assign to datagridview
            foreach (PictureBox p in PKXBOXES)
                p.ContextMenuStrip = mnu;

            // Load Data
            B_Search.Enabled = false;
            L_Count.Text = "Loading...";
            new Task(LoadDatabase).Start();

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
        private List<PKM> Results;
        private List<PKM> RawDB;
        private int slotSelected = -1; // = null;
        private Image slotColor;
        private const int RES_MAX = 66;
        private const int RES_MIN = 6;
        private readonly string Counter;
        private readonly string Viewed;
        private const int MAXFORMAT = PKX.Generation;
        private readonly string EXTERNAL_SAV = new DirectoryInfo(Main.BackupPath).Name + Path.DirectorySeparatorChar;

        // Important Events
        private void ClickView(object sender, EventArgs e)
        {
            sender = WinFormsUtil.GetUnderlyingControl(sender);
            int index = Array.IndexOf(PKXBOXES, sender);
            if (index >= RES_MAX)
            {
                System.Media.SystemSounds.Exclamation.Play();
                return;
            }
            index += SCR_Box.Value * RES_MIN;
            if (index >= Results.Count)
            {
                System.Media.SystemSounds.Exclamation.Play();
                return;
            }

            PKME_Tabs.PopulateFields(Results[index], false);
            slotSelected = index;
            slotColor = Properties.Resources.slotView;
            FillPKXBoxes(SCR_Box.Value);
            L_Viewed.Text = string.Format(Viewed, Results[index].Identifier);
        }

        private void ClickDelete(object sender, EventArgs e)
        {
            sender = WinFormsUtil.GetUnderlyingControl(sender);
            int index = Array.IndexOf(PKXBOXES, sender);
            if (index >= RES_MAX)
            {
                System.Media.SystemSounds.Exclamation.Play();
                return;
            }
            index += SCR_Box.Value * RES_MIN;
            if (index >= Results.Count)
            {
                System.Media.SystemSounds.Exclamation.Play();
                return;
            }

            var pk = Results[index];
            string path = pk.Identifier;

#if LOADALL
            if (path.StartsWith(EXTERNAL_SAV))
            {
                WinFormsUtil.Alert(MsgDBDeleteFailBackup);
                return;
            }
#endif
            if (path.Contains(Path.DirectorySeparatorChar))
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
                int offset = SAV.GetBoxOffset(box) + (slot *SAV.SIZE_STORED);
                PKM pkSAV = SAV.GetStoredSlot(offset);

                if (!pkSAV.DecryptedBoxData.SequenceEqual(pk.DecryptedBoxData)) // data still exists in SAV, unmodified
                {
                    WinFormsUtil.Error(MsgDBDeleteFailModified, MsgDBDeleteFailWarning);
                    return;
                }
                var change = new SlotChange {Box = box, Offset = offset, Slot = slot};
                BoxView.M.SetPKM(BoxView.SAV.BlankPKM, change, true, Properties.Resources.slotDel);
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

            File.WriteAllBytes(path, pk.Data.Take(pk.SIZE_STORED).ToArray());
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
            slotColor = Properties.Resources.slotSet;
            if ((SCR_Box.Maximum+1)*6 < Results.Count)
                SCR_Box.Maximum++;
            SCR_Box.Value = Math.Max(0, SCR_Box.Maximum - (PKXBOXES.Length/6) + 1);
            FillPKXBoxes(SCR_Box.Value);
            WinFormsUtil.Alert(MsgDBAddFromTabsSuccess);
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

            var Any = new ComboItem {Text = MsgAny, Value = -1};

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
            ResetFilters(null, EventArgs.Empty);
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

            if (sender != null)
                System.Media.SystemSounds.Asterisk.Play();
        }

        private void GenerateDBReport(object sender, EventArgs e)
        {
            if (WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgDBCreateReportPrompt, MsgDBCreateReportWarning) != DialogResult.Yes)
                return;

            ReportGrid reportGrid = new ReportGrid();
            reportGrid.Show();
            reportGrid.PopulateData(Results.ToArray());
        }

        private void LoadDatabase()
        {
            RawDB = LoadPKMSaves(DatabasePath, Main.BackupPath, EXTERNAL_SAV, SAV);

            // Load stats for pkm who do not have any
            foreach (var pk in RawDB.Where(z => z.Stat_Level == 0))
            {
                pk.Stat_Level = pk.CurrentLevel;
                pk.SetStats(pk.GetStats(pk.PersonalInfo));
            }

            try
            {
                BeginInvoke(new MethodInvoker(() => SetResults(RawDB)));
            }
            catch { /* Window Closed? */ }
        }

        private static List<PKM> LoadPKMSaves(string pkmdb, string savdb, string EXTERNAL_SAV, SaveFile SAV)
        {
            var dbTemp = new ConcurrentBag<PKM>();
            var files = Directory.EnumerateFiles(pkmdb, "*", SearchOption.AllDirectories);
            var extensions = new HashSet<string>(PKM.Extensions.Select(z => $".{z}"));
            Parallel.ForEach(files, file =>
            {
                var fi = new FileInfo(file);
                if (!extensions.Contains(fi.Extension) || !PKX.IsPKM(fi.Length)) return;
                var data = File.ReadAllBytes(file);
                var prefer = PKX.GetPKMFormatFromExtension(fi.Extension, SAV.Generation);
                var pk = PKMConverter.GetPKMfromBytes(data, prefer);
                if (!(pk?.Species > 0))
                    return;
                pk.Identifier = file;
                dbTemp.Add(pk);
            });

#if LOADALL
            if (SaveUtil.GetSavesFromFolder(savdb, false, out IEnumerable<string> result))
            {
                Parallel.ForEach(result, file =>
                {
                    var sav = SaveUtil.GetVariantSAV(file);
                    var path = EXTERNAL_SAV + new FileInfo(file).Name;
                    if (sav.HasBox)
                    {
                        foreach (var pk in sav.BoxData)
                            addPKM(pk);
                    }

                    void addPKM(PKM pk)
                    {
                        pk.Identifier = Path.Combine(path, pk.Identifier);
                        dbTemp.Add(pk);
                    }
                });
            }
#endif
            // Fetch from save file
            var savpkm = SAV.BoxData.Where(pk => pk.Species != 0);

            var bakpkm = dbTemp.Where(pk => pk.Species != 0).OrderBy(pk => pk.Identifier);
            var db = bakpkm.Concat(savpkm).Where(pk => pk.ChecksumValid && pk.Sanity == 0);

            // Prepare Database
            return new List<PKM>(db);
        }

        // IO Usage
        private void OpenDB(object sender, EventArgs e)
        {
            if (Directory.Exists(DatabasePath))
                Process.Start("explorer.exe", DatabasePath);
        }

        private void Menu_Export_Click(object sender, EventArgs e)
        {
            if (Results == null || Results.Count == 0)
            { WinFormsUtil.Alert(MsgDBCreateReportFail); return; }

            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgDBExportResultsPrompt))
                return;

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (DialogResult.OK != fbd.ShowDialog())
                return;

            string path = fbd.SelectedPath;
            Directory.CreateDirectory(path);

            foreach (PKM pkm in Results)
                File.WriteAllBytes(Path.Combine(path, Util.CleanFileName(pkm.FileName)), pkm.DecryptedBoxData);
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
                res = res.Where(pk => pk.Identifier.StartsWith(DatabasePath + Path.DirectorySeparatorChar, StringComparison.Ordinal));
            if (!Menu_SearchDatabase.Checked)
            {
                res = res.Where(pk => !pk.Identifier.StartsWith(DatabasePath + Path.DirectorySeparatorChar, StringComparison.Ordinal));
#if LOADALL
                res = res.Where(pk => !pk.Identifier.StartsWith(EXTERNAL_SAV, StringComparison.Ordinal));
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

                Level = int.TryParse(TB_Level.Text, out var lvl) ? (int?)lvl : null,
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
                switch (ModifierKeys)
                {
                    case Keys.Control:
                        settings.SearchClones = CloneDetectionMethod.HashPID;
                        break;
                    default:
                        settings.SearchClones = CloneDetectionMethod.HashDetails;
                        break;
                }
            }

            return settings;
        }

        private async void B_Search_Click(object sender, EventArgs e)
        {
            B_Search.Enabled = false;
            var search = SearchDatabase();

            bool legalSearch = Menu_SearchLegal.Checked ^ Menu_SearchIllegal.Checked;
            if (legalSearch && WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgDBSearchLegalityWordfilter) == DialogResult.No)
                ParseSettings.CheckWordFilter = false;
            var results = await Task.Run(() => search.ToList()).ConfigureAwait(true);
            ParseSettings.CheckWordFilter = true;

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
            if (Results == null)
            {
                for (int i = 0; i < RES_MAX; i++)
                    PKXBOXES[i].Image = null;
                return;
            }
            int begin = start*RES_MIN;
            int end = Math.Min(RES_MAX, Results.Count - begin);
            for (int i = 0; i < end; i++)
                PKXBOXES[i].Image = Results[i + begin].Sprite(SAV, -1, -1, true);
            for (int i = end; i < RES_MAX; i++)
                PKXBOXES[i].Image = null;

            for (int i = 0; i < RES_MAX; i++)
                PKXBOXES[i].BackgroundImage = Properties.Resources.slotTrans;
            if (slotSelected != -1 && slotSelected >= begin && slotSelected < begin + RES_MAX)
                PKXBOXES[slotSelected - begin].BackgroundImage = slotColor ?? Properties.Resources.slotView;
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

        private void Menu_SearchAdvanced_Click(object sender, EventArgs e)
        {
            if (Menu_SearchAdvanced.Checked)
            {
                Size = MaximumSize;
            }
            else
            {
                Size = MinimumSize;
                RTB_Instructions.Clear();
            }
        }

        private void Menu_Exit_Click(object sender, EventArgs e) => Close();

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (!PAN_Box.RectangleToScreen(PAN_Box.ClientRectangle).Contains(MousePosition))
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
            var db = RawDB.Where(pk => pk.Identifier.StartsWith(DatabasePath + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                .OrderByDescending(file => File.GetLastWriteTimeUtc(file.Identifier));

            var clones = SearchUtil.GetExtraClones(db);
            foreach (var pk in clones)
            {
                try { File.Delete(pk.Identifier); ++deleted; }
                catch (Exception ex) { WinFormsUtil.Error(MsgDBDeleteCloneFail + Environment.NewLine + ex.Message + Environment.NewLine + pk.Identifier); }
            }

            if (deleted == 0)
            { WinFormsUtil.Alert(MsgDBDeleteCloneNone); return; }

            WinFormsUtil.Alert(string.Format(MsgFileDeleteCount, deleted), MsgWindowClose);
            Close();
        }
    }
}
