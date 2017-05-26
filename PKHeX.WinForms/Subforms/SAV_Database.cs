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
using PKHeX.WinForms.Controls;

namespace PKHeX.WinForms
{
    public partial class SAV_Database : Form
    {
        private readonly SaveFile SAV;
        private readonly SAVEditor BoxView;
        private readonly PKMEditor PKME_Tabs;
        public SAV_Database(PKMEditor f1, SAVEditor saveditor)
        {
            SAV = saveditor.SAV;
            BoxView = saveditor;
            PKME_Tabs = f1;
            InitializeComponent();

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
            PAN_Box.MouseHover += (sender, args) =>
            {
                if (ActiveForm == this)
                    SCR_Box.Focus();
            };
            foreach (var slot in PKXBOXES)
            {
                slot.MouseEnter += (sender, args) =>
                {
                    if (ActiveForm == this)
                        SCR_Box.Focus();
                };
                // Enable Click
                slot.MouseClick += (sender, args) =>
                {
                    if (ModifierKeys == Keys.Control)
                        clickView(sender, args);
                    else if (ModifierKeys == Keys.Alt)
                        clickDelete(sender, args);
                    else if (ModifierKeys == Keys.Shift)
                        clickSet(sender, args);
                };
            }
            
            Counter = L_Count.Text;
            Viewed = L_Viewed.Text;
            L_Viewed.Text = ""; // invis for now
            populateComboBoxes();

            ContextMenuStrip mnu = new ContextMenuStrip();
            ToolStripMenuItem mnuView = new ToolStripMenuItem("View");
            ToolStripMenuItem mnuDelete = new ToolStripMenuItem("Delete");

            // Assign event handlers
            mnuView.Click += clickView;
            mnuDelete.Click += clickDelete;

            // Add to main context menu
            mnu.Items.AddRange(new ToolStripItem[] { mnuView, mnuDelete });

            // Assign to datagridview
            foreach (PictureBox p in PKXBOXES)
                p.ContextMenuStrip = mnu;

            // Load Data
            var dbTemp = new ConcurrentBag<PKM>();
            var files = Directory.GetFiles(DatabasePath, "*", SearchOption.AllDirectories);
            Parallel.ForEach(files, file =>
            {
                FileInfo fi = new FileInfo(file);
                if (!fi.Extension.Contains(".pk") || !PKX.getIsPKM(fi.Length)) return;
                var pk = PKMConverter.getPKMfromBytes(File.ReadAllBytes(file), file, prefer: (fi.Extension.Last() - 0x30)&7);
                if (pk != null)
                    dbTemp.Add(pk);
            });

            // Prepare Database
            RawDB = new List<PKM>(dbTemp.OrderBy(pk => pk.Identifier)
                                        .Concat(SAV.BoxData.Where(pk => pk.Species != 0)) // Fetch from save file
                                        .Where(pk => pk.ChecksumValid && pk.Species != 0 && pk.Sanity == 0)
                                        .Distinct());
            setResults(RawDB);

            Menu_SearchSettings.DropDown.Closing += (sender, e) =>
            {
                if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                    e.Cancel = true;
            };
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
        private const int MAXFORMAT = 7;
        private static string hash(PKM pk)
        {
            switch (pk.Format)
            {
                case 1: return pk.Species.ToString("000") + ((PK1)pk).DV16.ToString("X4");
                case 2: return pk.Species.ToString("000") + ((PK2)pk).DV16.ToString("X4");
                default: return pk.Species.ToString("000") + pk.PID.ToString("X8") + string.Join(" ", pk.IVs) + pk.AltForm.ToString("00");
            }
        }

        // Important Events
        private void clickView(object sender, EventArgs e)
        {
            sender = ((sender as ToolStripItem)?.Owner as ContextMenuStrip)?.SourceControl ?? sender as PictureBox;
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
            
            PKME_Tabs.populateFields(Results[index], false);
            slotSelected = index;
            slotColor = Properties.Resources.slotView;
            FillPKXBoxes(SCR_Box.Value);
            L_Viewed.Text = string.Format(Viewed, Results[index].Identifier);
        }
        private void clickDelete(object sender, EventArgs e)
        {
            sender = ((sender as ToolStripItem)?.Owner as ContextMenuStrip)?.SourceControl ?? sender as PictureBox;
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

            if (path.Contains(Path.DirectorySeparatorChar))
            {
                // Data from Database: Delete file from disk
                File.Delete(path);
            }
            else
            {
                // Data from Box: Delete from save file
                int box = pk.Box-1;
                int slot = pk.Slot-1;
                int offset = SAV.getBoxOffset(box) + slot*SAV.SIZE_STORED;
                PKM pkSAV = SAV.getStoredSlot(offset);

                if (pkSAV.Data.SequenceEqual(pk.Data))
                {
                    SAV.setStoredSlot(SAV.BlankPKM, offset);
                    BoxView.setPKXBoxes();
                }
                else
                {
                    WinFormsUtil.Error("Database slot data does not match save data!", "Don't move Pokémon after initializing the Database, please re-open the Database viewer.");
                    return;
                }
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
        private void clickSet(object sender, EventArgs e)
        {
            // Don't care what slot was clicked, just add it to the database
            if (!PKME_Tabs.verifiedPKM())
                return;

            PKM pk = PKME_Tabs.preparePKM();
            if (!Directory.Exists(DatabasePath))
                Directory.CreateDirectory(DatabasePath);

            string path = Path.Combine(DatabasePath, Util.CleanFileName(pk.FileName));

            if (RawDB.Any(p => p.Identifier == path))
            {
                WinFormsUtil.Alert("File already exists in database!");
                return;
            }

            File.WriteAllBytes(path, pk.Data.Take(pk.SIZE_STORED).ToArray());
            pk.Identifier = path;

            int pre = RawDB.Count;
            RawDB.Add(pk);
            RawDB = new List<PKM>(RawDB);
            int post = RawDB.Count;
            if (pre == post)
            { WinFormsUtil.Alert("Pokémon already exists in database."); return; }
            Results.Add(pk);

            // Refresh database view.
            L_Count.Text = string.Format(Counter, Results.Count);
            slotSelected = Results.Count - 1;
            slotColor = Properties.Resources.slotSet;
            if ((SCR_Box.Maximum+1)*6 < Results.Count)
                SCR_Box.Maximum += 1;
            SCR_Box.Value = Math.Max(0, SCR_Box.Maximum - PKXBOXES.Length/6 + 1);
            FillPKXBoxes(SCR_Box.Value);
            WinFormsUtil.Alert("Added Pokémon from tabs to database.");
        }
        private void populateComboBoxes()
        {
            // Set the Text
            CB_HeldItem.DisplayMember =
            CB_Species.DisplayMember =
            CB_Ability.DisplayMember =
            CB_Nature.DisplayMember =
            CB_GameOrigin.DisplayMember =
            CB_HPType.DisplayMember = "Text";

            // Set the Value
            CB_HeldItem.ValueMember =
            CB_Species.ValueMember =
            CB_Ability.ValueMember =
            CB_Nature.ValueMember =
            CB_GameOrigin.ValueMember =
            CB_HPType.ValueMember = "Value";

            var Any = new ComboItem {Text = "Any", Value = -1};

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
            var DS_Type = Util.getCBList(hptypes, null); 
            DS_Type.Insert(0, Any); CB_HPType.DataSource = DS_Type;

            // Set the Move ComboBoxes too..
            var DS_Move = new List<ComboItem>(GameInfo.MoveDataSource);
            DS_Move.RemoveAt(0); DS_Move.Insert(0, Any);
            {
                foreach (ComboBox cb in new[] { CB_Move1, CB_Move2, CB_Move3, CB_Move4 })
                {
                    cb.DisplayMember = "Text"; cb.ValueMember = "Value";
                    cb.DataSource = new BindingSource(DS_Move, null);
                }
            }

            // Trigger a Reset
            resetFilters(null, null);
        }
        private void resetFilters(object sender, EventArgs e)
        {
            CHK_Shiny.Checked = CHK_IsEgg.Checked = true;
            CHK_Shiny.CheckState = CHK_IsEgg.CheckState = CheckState.Indeterminate;
            MT_ESV.Text = "";
            CB_HeldItem.SelectedIndex = 0;
            CB_Species.SelectedIndex = 0;
            CB_Ability.SelectedIndex = 0;
            CB_Nature.SelectedIndex = 0;
            CB_HPType.SelectedIndex = 0;

            CB_Level.SelectedIndex = 0;
            TB_Level.Text = "";
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
        private void generateDBReport(object sender, EventArgs e)
        {
            if (WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Generate a Report on all data?", "This may take a while...")
                != DialogResult.Yes)
                return;

            frmReport ReportForm = new frmReport();
            ReportForm.Show();
            ReportForm.PopulateData(Results.ToArray());
        }

        // IO Usage
        private void openDB(object sender, EventArgs e)
        {
            if (Directory.Exists(DatabasePath))
                Process.Start("explorer.exe", DatabasePath);
        }
        private void Menu_Export_Click(object sender, EventArgs e)
        {
            if (Results == null || Results.Count == 0)
            { WinFormsUtil.Alert("No results to export."); return; }

            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Export to a folder?"))
                return;

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (DialogResult.OK != fbd.ShowDialog())
                return;

            string path = fbd.SelectedPath;
            if (!Directory.Exists(path)) // just in case...
                Directory.CreateDirectory(path);

            foreach (PKM pkm in Results)
                File.WriteAllBytes(Path.Combine(path, Util.CleanFileName(pkm.FileName)), pkm.DecryptedBoxData);
        }

        // View Updates
        private void B_Search_Click(object sender, EventArgs e)
        {
            // Populate Search Query Result
            IEnumerable<PKM> res = RawDB;

            int format = MAXFORMAT + 1 - CB_Format.SelectedIndex;
            switch (CB_FormatComparator.SelectedIndex)
            {
                case 0: /* Do nothing */                            break;
                case 1: res = res.Where(pk => pk.Format >= format); break;
                case 2: res = res.Where(pk => pk.Format == format); break;
                case 3: res = res.Where(pk => pk.Format <= format); break;
            }
            if (CB_FormatComparator.SelectedIndex != 0)
            {
                if (format <= 2) // 1-2
                    res = res.Where(pk => pk.Format <= 2);
                if (format >= 3 && format <= 6) // 3-6
                    res = res.Where(pk => pk.Format >= 3);
                if (format >= 7) // 1,3-6,7
                    res = res.Where(pk => pk.Format != 2);
            }

            switch (CB_Generation.SelectedIndex)
            {
                case 0: /* Do nothing */                break;
                case 1: res = res.Where(pk => pk.Gen7); break;
                case 2: res = res.Where(pk => pk.Gen6); break;
                case 3: res = res.Where(pk => pk.Gen5); break;
                case 4: res = res.Where(pk => pk.Gen4); break;
                case 5: res = res.Where(pk => pk.Gen3); break;
            }

            // Primary Searchables
            int species = WinFormsUtil.getIndex(CB_Species);
            int ability = WinFormsUtil.getIndex(CB_Ability);
            int nature = WinFormsUtil.getIndex(CB_Nature);
            int item = WinFormsUtil.getIndex(CB_HeldItem);
            if (species != -1) res = res.Where(pk => pk.Species == species);
            if (ability != -1) res = res.Where(pk => pk.Ability == ability);
            if (nature != -1) res = res.Where(pk => pk.Nature == nature);
            if (item != -1) res = res.Where(pk => pk.HeldItem == item);

            // Secondary Searchables
            int move1 = WinFormsUtil.getIndex(CB_Move1);
            int move2 = WinFormsUtil.getIndex(CB_Move2);
            int move3 = WinFormsUtil.getIndex(CB_Move3);
            int move4 = WinFormsUtil.getIndex(CB_Move4);
            if (move1 != -1) res = res.Where(pk => pk.Moves.Contains(move1));
            if (move2 != -1) res = res.Where(pk => pk.Moves.Contains(move2));
            if (move3 != -1) res = res.Where(pk => pk.Moves.Contains(move3));
            if (move4 != -1) res = res.Where(pk => pk.Moves.Contains(move4));
            int vers = WinFormsUtil.getIndex(CB_GameOrigin);
            if (vers != -1) res = res.Where(pk => pk.Version == vers);
            int hptype = WinFormsUtil.getIndex(CB_HPType);
            if (hptype != -1) res = res.Where(pk => pk.HPType == hptype);
            if (CHK_Shiny.CheckState == CheckState.Checked) res = res.Where(pk => pk.IsShiny);
            if (CHK_Shiny.CheckState == CheckState.Unchecked) res = res.Where(pk => !pk.IsShiny);
            if (CHK_IsEgg.CheckState == CheckState.Checked) res = res.Where(pk => pk.IsEgg);
            if (CHK_IsEgg.CheckState == CheckState.Unchecked) res = res.Where(pk => !pk.IsEgg);
            if (CHK_IsEgg.CheckState == CheckState.Checked && MT_ESV.Text != "")
                res = res.Where(pk => pk.PSV == Convert.ToInt16(MT_ESV.Text));

            // Tertiary Searchables
            if (TB_Level.Text != "") // Level
            {
                int level = Convert.ToInt16(TB_Level.Text);
                if (level <= 100)
                switch (CB_Level.SelectedIndex)
                {
                    case 0: break; // Any 
                    case 1: // <=
                        res = res.Where(pk => pk.Stat_Level <= level);
                        break;
                    case 2: // == 
                        res = res.Where(pk => pk.Stat_Level == level);
                        break;
                    case 3: // >=
                        res = res.Where(pk => pk.Stat_Level >= level);
                        break;
                }
            }
            switch (CB_IV.SelectedIndex)
            {
                case 0: break; // Do nothing
                case 1: // <= 90
                    res = res.Where(pk => pk.IVs.Sum() <= 90);
                    break;
                case 2: // 91-120
                    res = res.Where(pk => pk.IVs.Sum() > 90 && pk.IVs.Sum() <= 120);
                    break;
                case 3: // 121-150
                    res = res.Where(pk => pk.IVs.Sum() > 120 && pk.IVs.Sum() <= 150);
                    break;
                case 4: // 151-179
                    res = res.Where(pk => pk.IVs.Sum() > 150 && pk.IVs.Sum() < 180);
                    break;
                case 5: // 180+
                    res = res.Where(pk => pk.IVs.Sum() >= 180);
                    break;
                case 6: // == 186
                    res = res.Where(pk => pk.IVs.Sum() == 186);
                    break;
            }
            switch (CB_EVTrain.SelectedIndex)
            {
                case 0: break; // Do nothing
                case 1: // None (0)
                    res = res.Where(pk => pk.EVs.Sum() == 0);
                    break;
                case 2: // Some (127-0)
                    res = res.Where(pk => pk.EVs.Sum() < 128);
                    break;
                case 3: // Half (128-507)
                    res = res.Where(pk => pk.EVs.Sum() >= 128 && pk.EVs.Sum() < 508);
                    break;
                case 4: // Full (508+)
                    res = res.Where(pk => pk.EVs.Sum() >= 508);
                    break;
            }

            // Filter for Selected Source
            if (!Menu_SearchBoxes.Checked)
                res = res.Where(pk => pk.Identifier.StartsWith(DatabasePath + Path.DirectorySeparatorChar, StringComparison.Ordinal));
            if (!Menu_SearchDatabase.Checked)
                res = res.Where(pk => !pk.Identifier.StartsWith(DatabasePath + Path.DirectorySeparatorChar, StringComparison.Ordinal));

            slotSelected = -1; // reset the slot last viewed
            
            if (Menu_SearchLegal.Checked && !Menu_SearchIllegal.Checked)
                res = res.Where(pk => new LegalityAnalysis(pk).ParsedValid);
            if (!Menu_SearchLegal.Checked && Menu_SearchIllegal.Checked)
                res = res.Where(pk => new LegalityAnalysis(pk).ParsedInvalid);

            if (RTB_Instructions.Lines.Any(line => line.Length > 0))
            {
                var filters = BatchEditor.StringInstruction.getFilters(RTB_Instructions.Lines).ToArray();
                BatchEditor.screenStrings(filters);
                res = res.Where(pkm => // Compare across all filters
                {
                    foreach (var cmd in filters)
                    {
                        if (cmd.PropertyName == nameof(PKM.Identifier) + "Contains")
                            return pkm.Identifier.Contains(cmd.PropertyValue);
                        if (!pkm.GetType().HasPropertyAll(typeof(PKM), cmd.PropertyName))
                            return false;
                        try { if (ReflectUtil.GetValueEquals(pkm, cmd.PropertyName, cmd.PropertyValue) == cmd.Evaluator) continue; }
                        catch { Console.WriteLine($"Unable to compare {cmd.PropertyName} to {cmd.PropertyValue}."); }
                        return false;
                    }
                    return true;
                });
            }

            if (Menu_SearchClones.Checked)
                res = res.GroupBy(hash).Where(group => group.Count() > 1).SelectMany(z => z);

            var results = res.ToArray();
            if (results.Length == 0)
            {
                if (!Menu_SearchBoxes.Checked && !Menu_SearchDatabase.Checked)
                    WinFormsUtil.Alert("No data source to search!", "No results found!");
                else
                    WinFormsUtil.Alert("No results found!");
            }
            setResults(new List<PKM>(results)); // updates Count Label as well.
            System.Media.SystemSounds.Asterisk.Play();
        }
        private void updateScroll(object sender, ScrollEventArgs e)
        {
            if (e.OldValue != e.NewValue)
                FillPKXBoxes(e.NewValue);
        }
        private void setResults(List<PKM> res)
        {
            Results = new List<PKM>(res);

            SCR_Box.Maximum = (int)Math.Ceiling((decimal)Results.Count / RES_MIN);
            if (SCR_Box.Maximum > 0) SCR_Box.Maximum -= 1;

            SCR_Box.Value = 0;
            FillPKXBoxes(0);

            L_Count.Text = string.Format(Counter, Results.Count);
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
            int end = Math.Min(RES_MAX, Results.Count - start*RES_MIN);
            for (int i = 0; i < end; i++)
                PKXBOXES[i].Image = Results[i + begin].Sprite();
            for (int i = end; i < RES_MAX; i++)
                PKXBOXES[i].Image = null;

            for (int i = 0; i < RES_MAX; i++)
                PKXBOXES[i].BackgroundImage = Properties.Resources.slotTrans;
            if (slotSelected != -1 && slotSelected >= RES_MIN * start && slotSelected < RES_MIN * start + RES_MAX)
                PKXBOXES[slotSelected - start * RES_MIN].BackgroundImage = slotColor ?? Properties.Resources.slotView;
        }

        // Misc Update Methods
        private void toggleESV(object sender, EventArgs e)
        {
            L_ESV.Visible = MT_ESV.Visible = CHK_IsEgg.CheckState == CheckState.Checked;
        }
        private void changeLevel(object sender, EventArgs e)
        {
            if (CB_Level.SelectedIndex == 0)
                TB_Level.Text = "";
        }
        private void changeGame(object sender, EventArgs e)
        {
            if (CB_GameOrigin.SelectedIndex != 0)
                CB_Generation.SelectedIndex = 0;
        }
        private void changeGeneration(object sender, EventArgs e)
        {
            if (CB_Generation.SelectedIndex != 0)
                CB_GameOrigin.SelectedIndex = 0;
        }

        private void Menu_SearchAdvanced_Click(object sender, EventArgs e)
        {
            if (!Menu_SearchAdvanced.Checked)
            { Size = MinimumSize; RTB_Instructions.Clear(); }
            else Size = MaximumSize;
        }

        private void Menu_Exit_Click(object sender, EventArgs e)
        {
            Close();
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (!PAN_Box.RectangleToScreen(PAN_Box.ClientRectangle).Contains(MousePosition))
                return;
            int oldval = SCR_Box.Value;
            int newval = oldval + (e.Delta < 0 ? 1 : -1);
            if (newval >= SCR_Box.Minimum && SCR_Box.Maximum >= newval)
                FillPKXBoxes(SCR_Box.Value = newval);
        }

        private void changeFormatFilter(object sender, EventArgs e)
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
                "Deleting clones from database is not reversible." + Environment.NewLine +
                "If a PKM is deemed a clone, only the newest file (date modified) will be kept.", "Continue?");

            if (dr != DialogResult.Yes)
                return;

            var deleted = 0;
            var db = RawDB.Where(pk => pk.Identifier.StartsWith(DatabasePath + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                .OrderByDescending(file => new FileInfo(file.Identifier).LastWriteTime);
            var clones = db.GroupBy(hash).Where(group => group.Count() > 1).SelectMany(z => z.Skip(1));
            foreach (var pk in clones)
            {
                try { File.Delete(pk.Identifier); ++deleted; }
                catch { WinFormsUtil.Error("Unable to delete clone:" + Environment.NewLine + pk.Identifier); }
            }

            if (deleted == 0)
            { WinFormsUtil.Alert("No clones detected or deleted."); return; }

            WinFormsUtil.Alert($"{deleted} files deleted.", "The form will now close.");
            Close();
        }
    }
}
