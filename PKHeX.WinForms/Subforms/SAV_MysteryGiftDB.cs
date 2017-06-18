using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Controls;

namespace PKHeX.WinForms
{
    public partial class SAV_MysteryGiftDB : Form
    {
        private readonly PKMEditor PKME_Tabs;
        private readonly SaveFile SAV;
        public SAV_MysteryGiftDB(PKMEditor tabs, SAVEditor sav)
        {
            SAV = sav.SAV;
            PKME_Tabs = tabs;
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
                        ClickView(sender, args);
                };
            }
            
            Counter = L_Count.Text;
            Viewed = L_Viewed.Text;
            L_Viewed.Text = ""; // invis for now

            ContextMenuStrip mnu = new ContextMenuStrip();
            ToolStripMenuItem mnuView = new ToolStripMenuItem("View");
            ToolStripMenuItem mnuSaveMG = new ToolStripMenuItem("Save Gift");
            ToolStripMenuItem mnuSavePK = new ToolStripMenuItem("Save PKM");

            // Assign event handlers
            mnuView.Click += ClickView;
            mnuSaveMG.Click += ClickSaveMG;
            mnuSavePK.Click += ClickSavePK;

            // Add to main context menu
            mnu.Items.AddRange(new ToolStripItem[] { mnuView, mnuSaveMG, mnuSavePK });

            // Assign to datagridview
            foreach (PictureBox p in PKXBOXES)
                p.ContextMenuStrip = mnu;

            // Load Data
            RawDB = new List<MysteryGift>();
            RawDB.AddRange(Legal.MGDB_G4);
            RawDB.AddRange(Legal.MGDB_G5);
            RawDB.AddRange(Legal.MGDB_G6);
            RawDB.AddRange(Legal.MGDB_G7);

            RawDB = new List<MysteryGift>(RawDB.Where(mg => !mg.IsItem && mg.IsPokémon && mg.Species > 0).Distinct().OrderBy(mg => mg.Species));
            foreach (var mg in RawDB)
                mg.GiftUsed = false;
            SetResults(RawDB);

            Menu_SearchSettings.DropDown.Closing += (sender, e) =>
            {
                if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                    e.Cancel = true;
            };

            PopulateComboBoxes();
            CenterToParent();
        }
        private readonly PictureBox[] PKXBOXES;
        private readonly string DatabasePath = Main.MGDatabasePath;
        private List<MysteryGift> Results;
        private readonly List<MysteryGift> RawDB;
        private int slotSelected = -1; // = null;
        private Image slotColor;
        private const int RES_MAX = 66;
        private const int RES_MIN = 6;
        private readonly string Counter;
        private readonly string Viewed;
        private const int MAXFORMAT = 7;

        // Important Events
        private void ClickView(object sender, EventArgs e)
        {
            int index = GetSenderIndex(sender);
            PKME_Tabs.PopulateFields(Results[index].ConvertToPKM(SAV), false);
            slotSelected = index;
            slotColor = Properties.Resources.slotView;
            FillPKXBoxes(SCR_Box.Value);
            L_Viewed.Text = string.Format(Viewed, Results[index].FileName);
        }
        private void ClickSavePK(object sender, EventArgs e)
        {
            int index = GetSenderIndex(sender);
            var gift = Results[index];
            var pk = gift.ConvertToPKM(SAV);
            WinFormsUtil.SavePKMDialog(pk);
        }
        private void ClickSaveMG(object sender, EventArgs e)
        {
            int index = GetSenderIndex(sender);
            var gift = Results[index];
            WinFormsUtil.SaveMGDialog(gift);
        }

        private int GetSenderIndex(object sender)
        {
            sender = ((sender as ToolStripItem)?.Owner as ContextMenuStrip)?.SourceControl ?? sender as PictureBox;
            int index = Array.IndexOf(PKXBOXES, sender);
            if (index >= RES_MAX)
            {
                System.Media.SystemSounds.Exclamation.Play();
                return -1;
            }
            index += SCR_Box.Value*RES_MIN;
            if (index >= Results.Count)
            {
                System.Media.SystemSounds.Exclamation.Play();
                return -1;
            }
            return index;
        }
        private void PopulateComboBoxes()
        {
            // Set the Text
            CB_HeldItem.DisplayMember =
            CB_Species.DisplayMember = "Text";

            // Set the Value
            CB_HeldItem.ValueMember =
            CB_Species.ValueMember = "Value";

            var Any = new ComboItem {Text = "Any", Value = -1};

            var DS_Species = new List<ComboItem>(GameInfo.SpeciesDataSource);
            DS_Species.RemoveAt(0);
            var filteredSpecies = DS_Species.Where(spec => RawDB.Any(mg => mg.Species == spec.Value)).ToList();
            filteredSpecies.Insert(0, Any);
            CB_Species.DataSource = filteredSpecies;

            var DS_Item = new List<ComboItem>(GameInfo.ItemDataSource);
            DS_Item.Insert(0, Any); CB_HeldItem.DataSource = DS_Item;
            
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
            ResetFilters(null, null);
        }
        private void ResetFilters(object sender, EventArgs e)
        {
            CHK_Shiny.Checked = CHK_IsEgg.Checked = true;
            CHK_Shiny.CheckState = CHK_IsEgg.CheckState = CheckState.Indeterminate;
            CB_HeldItem.SelectedIndex = 0;
            CB_Species.SelectedIndex = 0;

            CB_Move1.SelectedIndex = CB_Move2.SelectedIndex = CB_Move3.SelectedIndex = CB_Move4.SelectedIndex = 0;
            RTB_Instructions.Clear();

            if (sender != null)
                System.Media.SystemSounds.Asterisk.Play();
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
            { WinFormsUtil.Alert("No results to export."); return; }

            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Export to a folder?"))
                return;

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (DialogResult.OK != fbd.ShowDialog())
                return;

            string path = fbd.SelectedPath;
            if (!Directory.Exists(path)) // just in case...
                Directory.CreateDirectory(path);

            foreach (var gift in Results)
                File.WriteAllBytes(Path.Combine(path, Util.CleanFileName(gift.FileName)), gift.Data);
        }

        // View Updates
        private void B_Search_Click(object sender, EventArgs e)
        {
            // Populate Search Query Result
            IEnumerable<MysteryGift> res = RawDB;

            int format = MAXFORMAT + 1 - CB_Format.SelectedIndex;

            switch (CB_FormatComparator.SelectedIndex)
            {
                case 0: /* Do nothing */                            break;
                case 1: res = res.Where(mg => mg.Format >= format); break;
                case 2: res = res.Where(mg => mg.Format == format); break;
                case 3: res = res.Where(mg => mg.Format <= format); break;
            }

            // Primary Searchables
            int species = WinFormsUtil.GetIndex(CB_Species);
            int item = WinFormsUtil.GetIndex(CB_HeldItem);
            if (species != -1) res = res.Where(pk => pk.Species == species);
            if (item != -1) res = res.Where(pk => pk.HeldItem == item);

            // Secondary Searchables
            int move1 = WinFormsUtil.GetIndex(CB_Move1);
            int move2 = WinFormsUtil.GetIndex(CB_Move2);
            int move3 = WinFormsUtil.GetIndex(CB_Move3);
            int move4 = WinFormsUtil.GetIndex(CB_Move4);
            if (move1 != -1) res = res.Where(pk => pk.Moves.Contains(move1));
            if (move2 != -1) res = res.Where(pk => pk.Moves.Contains(move2));
            if (move3 != -1) res = res.Where(pk => pk.Moves.Contains(move3));
            if (move4 != -1) res = res.Where(pk => pk.Moves.Contains(move4));
            if (CHK_Shiny.CheckState == CheckState.Checked) res = res.Where(pk => pk.IsShiny);
            if (CHK_Shiny.CheckState == CheckState.Unchecked) res = res.Where(pk => !pk.IsShiny);
            if (CHK_IsEgg.CheckState == CheckState.Checked) res = res.Where(pk => pk.IsEgg);
            if (CHK_IsEgg.CheckState == CheckState.Unchecked) res = res.Where(pk => !pk.IsEgg);
            
            slotSelected = -1; // reset the slot last viewed

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
                { WinFormsUtil.Error("Empty Filter Value detected."); return; }

                res = res.Where(gift => // Compare across all filters
                {
                    foreach (var cmd in filters)
                    {
                        if (!gift.GetType().HasPropertyAll(cmd.PropertyName))
                            return false;
                        try { if (ReflectUtil.IsValueEqual(gift, cmd.PropertyName, cmd.PropertyValue) == cmd.Evaluator) continue; }
                        catch { Console.WriteLine($"Unable to compare {cmd.PropertyName} to {cmd.PropertyValue}."); }
                        return false;
                    }
                    return true;
                });
            }

            var results = res.ToArray();
            if (results.Length == 0)
            {
                WinFormsUtil.Alert("No results found!");
            }
            SetResults(new List<MysteryGift>(results)); // updates Count Label as well.
            System.Media.SystemSounds.Asterisk.Play();
        }
        private void UpdateScroll(object sender, ScrollEventArgs e)
        {
            if (e.OldValue != e.NewValue)
                FillPKXBoxes(e.NewValue);
        }
        private void SetResults(List<MysteryGift> res)
        {
            Results = new List<MysteryGift>(res);

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
            int begin = start * RES_MIN;
            int end = Math.Min(RES_MAX, Results.Count - start * RES_MIN);
            for (int i = 0; i < end; i++)
                PKXBOXES[i].Image = Results[i + begin].Sprite(SAV);
            for (int i = end; i < RES_MAX; i++)
                PKXBOXES[i].Image = null;

            for (int i = 0; i < RES_MAX; i++)
                PKXBOXES[i].BackgroundImage = Properties.Resources.slotTrans;
            if (slotSelected != -1 && slotSelected >= RES_MIN * start && slotSelected < RES_MIN * start + RES_MAX)
                PKXBOXES[slotSelected - start * RES_MIN].BackgroundImage = slotColor ?? Properties.Resources.slotView;
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
    }
}
