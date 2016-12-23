using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_Database : Form
    {
        public SAV_Database(Main f1)
        {
            m_parent = f1;
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
            RawDB = new List<PKM>();
            foreach (string file in Directory.GetFiles(DatabasePath, "*", SearchOption.AllDirectories))
            {
                FileInfo fi = new FileInfo(file);
                if (fi.Extension.Contains(".pk") && PKX.getIsPKM(fi.Length))
                    RawDB.Add(PKMConverter.getPKMfromBytes(File.ReadAllBytes(file), file));
            }
            // Fetch from save file
            foreach (var pkm in Main.SAV.BoxData.Where(pk => pk.Species != 0))
                RawDB.Add(pkm);

            // Prepare Database
            RawDB = new List<PKM>(RawDB.Where(pk => pk.ChecksumValid && pk.Species != 0 && pk.Sanity == 0));
            RawDB = new List<PKM>(RawDB.Distinct());
            setResults(RawDB);

            Menu_SearchSettings.DropDown.Closing += (sender, e) =>
            {
                if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                    e.Cancel = true;
            };
            CenterToParent();
        }
        private readonly Main m_parent;
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
        private readonly Func<PKM, string> hash = pk => pk.Species.ToString("000") + pk.PID.ToString("X8");

        // Important Events
        private void clickView(object sender, EventArgs e)
        {
            sender = ((sender as ToolStripItem)?.Owner as ContextMenuStrip)?.SourceControl ?? sender as PictureBox;
            int index = Array.IndexOf(PKXBOXES, sender);

            var dataArr = Results.Skip(SCR_Box.Value * RES_MIN).Take(RES_MAX).ToArray();
            if (index >= dataArr.Length)
                System.Media.SystemSounds.Exclamation.Play();
            else
            {
                m_parent.populateFields(dataArr[index], false);
                slotSelected = index + SCR_Box.Value * RES_MIN;
                slotColor = Properties.Resources.slotView;
                FillPKXBoxes(SCR_Box.Value);
                L_Viewed.Text = string.Format(Viewed, dataArr[index].Identifier);
            }
        }
        private void clickDelete(object sender, EventArgs e)
        {
            sender = ((sender as ToolStripItem)?.Owner as ContextMenuStrip)?.SourceControl ?? sender as PictureBox;
            int index = Array.IndexOf(PKXBOXES, sender);

            var dataArr = Results.Skip(SCR_Box.Value * RES_MIN).Take(RES_MAX).ToArray();
            if (index >= dataArr.Length)
                System.Media.SystemSounds.Exclamation.Play();
            else
            {
                var pk = dataArr[index];
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
                    int offset = Main.SAV.getBoxOffset(box) + slot*Main.SAV.SIZE_STORED;
                    PKM pkSAV = Main.SAV.getStoredSlot(offset);

                    if (pkSAV.Data.SequenceEqual(pk.Data))
                    {
                        Main.SAV.setStoredSlot(Main.SAV.BlankPKM, offset);
                        m_parent.setPKXBoxes();
                    }
                    else
                    {
                        Util.Error("Database slot data does not match save data!", "Don't move Pokémon after initializing the Database, please re-open the Database viewer.");
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
        }
        private void clickSet(object sender, EventArgs e)
        {
            // Don't care what slot was clicked, just add it to the database
            if (!m_parent.verifiedPKM())
                return;

            PKM pk = m_parent.preparePKM();
            if (!Directory.Exists(DatabasePath))
                Directory.CreateDirectory(DatabasePath);

            string path = Path.Combine(DatabasePath, Util.CleanFileName(pk.FileName));

            if (RawDB.Any(p => p.Identifier == path))
            {
                Util.Alert("File already exists in database!");
                return;
            }

            File.WriteAllBytes(path, pk.Data.Take(pk.SIZE_STORED).ToArray());
            pk.Identifier = path;

            int pre = RawDB.Count;
            RawDB.Add(pk);
            RawDB = new List<PKM>(RawDB.Distinct()); // just in case
            int post = RawDB.Count;
            if (pre == post)
            { Util.Alert("Pokémon already exists in database."); return; }
            Results.Add(pk);

            // Refresh database view.
            L_Count.Text = string.Format(Counter, Results.Count);
            slotSelected = Results.Count - 1;
            slotColor = Properties.Resources.slotSet;
            if ((SCR_Box.Maximum+1)*6 < Results.Count)
                SCR_Box.Maximum += 1;
            SCR_Box.Value = Math.Max(0, SCR_Box.Maximum - PKXBOXES.Length/6 + 1);
            FillPKXBoxes(SCR_Box.Value);
            Util.Alert("Added Pokémon from tabs to database.");
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
            
            string[] hptypes = new string[Main.GameStrings.types.Length - 2]; Array.Copy(Main.GameStrings.types, 1, hptypes, 0, hptypes.Length);
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
            if (Util.Prompt(MessageBoxButtons.YesNo, "Generate a Report on all data?", "This may take a while...")
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
            { Util.Alert("No results to export."); return; }

            if (DialogResult.Yes != Util.Prompt(MessageBoxButtons.YesNo, "Export to a folder?"))
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
            int species = Util.getIndex(CB_Species);
            int ability = Util.getIndex(CB_Ability);
            int nature = Util.getIndex(CB_Nature);
            int item = Util.getIndex(CB_HeldItem);
            if (species != -1) res = res.Where(pk => pk.Species == species);
            if (ability != -1) res = res.Where(pk => pk.Ability == ability);
            if (nature != -1) res = res.Where(pk => pk.Nature == nature);
            if (item != -1) res = res.Where(pk => pk.HeldItem == item);

            // Secondary Searchables
            int move1 = Util.getIndex(CB_Move1);
            int move2 = Util.getIndex(CB_Move2);
            int move3 = Util.getIndex(CB_Move3);
            int move4 = Util.getIndex(CB_Move4);
            if (move1 != -1) res = res.Where(pk => pk.Moves.Contains(move1));
            if (move2 != -1) res = res.Where(pk => pk.Moves.Contains(move2));
            if (move3 != -1) res = res.Where(pk => pk.Moves.Contains(move3));
            if (move4 != -1) res = res.Where(pk => pk.Moves.Contains(move4));
            int vers = Util.getIndex(CB_GameOrigin);
            if (vers != -1) res = res.Where(pk => pk.Version == vers);
            int hptype = Util.getIndex(CB_HPType);
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
                res = res.Where(pk => pk.Identifier.StartsWith(DatabasePath + Path.DirectorySeparatorChar));
            if (!Menu_SearchDatabase.Checked)
                res = res.Where(pk => !pk.Identifier.StartsWith(DatabasePath + Path.DirectorySeparatorChar));

            slotSelected = -1; // reset the slot last viewed
            
            if (Menu_SearchLegal.Checked && !Menu_SearchIllegal.Checked) // Legal Only
                res = res.Where(pk => pk.GenNumber >= 6 && new LegalityAnalysis(pk).Valid);
            if (!Menu_SearchLegal.Checked && Menu_SearchIllegal.Checked) // Illegal Only
                res = res.Where(pk => pk.GenNumber >= 6 && !new LegalityAnalysis(pk).Valid);

            if (RTB_Instructions.Lines.Any(line => line.Length > 0))
            {
                var raw =
                    RTB_Instructions.Lines
                        .Where(line => !string.IsNullOrWhiteSpace(line))
                        .Where(line => new[] { '!', '=' }.Contains(line[0]));

                var filters = (from line in raw
                        let eval = line[0] == '='
                        let split = line.Substring(1).Split('=')
                        where split.Length == 2 && !string.IsNullOrWhiteSpace(split[0])
                        select new BatchEditor.StringInstruction { PropertyName = split[0], PropertyValue = split[1], Evaluator = eval }).ToArray();

                if (filters.Any(z => string.IsNullOrWhiteSpace(z.PropertyValue)))
                { Util.Error("Empty Filter Value detected."); return; }

                BatchEditor.screenStrings(filters);
                res = res.Where(pkm => // Compare across all filters
                {
                    foreach (var cmd in filters)
                    {
                        if (!pkm.GetType().HasPropertyAll(cmd.PropertyName))
                            return false;
                        try { if (ReflectUtil.GetValueEquals(pkm, cmd.PropertyName, cmd.PropertyValue) == cmd.Evaluator) continue; }
                        catch { Console.WriteLine($"Unable to compare {cmd.PropertyName} to {cmd.PropertyValue}."); }
                        return false;
                    }
                    return true;
                });
            }

            if (Menu_SearchClones.Checked)
            {
                var r = res.ToArray();
                var hashes = r.Select(hash).ToArray();
                res = r.Where((t, i) => hashes.Count(x => x == hashes[i]) > 1).OrderBy(hash);
            }

            var results = res.ToArray();
            if (results.Length == 0)
            {
                if (!Menu_SearchBoxes.Checked && !Menu_SearchDatabase.Checked)
                    Util.Alert("No data source to search!", "No results found!");
                else
                    Util.Alert("No results found!");
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
            PKM[] data = Results.Skip(start * RES_MIN).Take(RES_MAX).ToArray();
            for (int i = 0; i < data.Length; i++)
                PKXBOXES[i].Image = data[i].Sprite;
            for (int i = data.Length; i < RES_MAX; i++)
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
                int index = MAXFORMAT - Main.SAV.Generation + 1;
                CB_Format.SelectedIndex = index < CB_Format.Items.Count ? index : 0; // SAV generation (offset by 1 for "Any")
            }
        }

        private void Menu_DeleteClones_Click(object sender, EventArgs e)
        {
            var dr = Util.Prompt(MessageBoxButtons.YesNo,
                "Deleting clones from database is not reversible." + Environment.NewLine +
                "If a PKM is deemed a clone, only the newest file (date modified) will be kept.", "Continue?");

            if (dr != DialogResult.Yes)
                return;

            var hashes = new List<string>();
            var deleted = 0;
            var db = RawDB.Where(pk => pk.Identifier.StartsWith(DatabasePath + Path.DirectorySeparatorChar))
                .OrderByDescending(file => new FileInfo(file.Identifier).LastWriteTime);
            foreach (var pk in db)
            {
                var h = hash(pk);
                if (!hashes.Contains(h))
                { hashes.Add(h); continue; }

                try { File.Delete(pk.Identifier); ++deleted; }
                catch { Util.Error("Unable to delete clone:" + Environment.NewLine + pk.Identifier); }
            }

            if (deleted == 0)
            { Util.Alert("No clones detected or deleted."); return; }

            Util.Alert($"{deleted} files deleted.", "The form will now close.");
            Close();
        }
    }
}
