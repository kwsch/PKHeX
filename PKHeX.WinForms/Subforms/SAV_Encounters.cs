using PKHeX.Core;
using PKHeX.Core.Searching;
using PKHeX.WinForms.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms
{
    public partial class SAV_Encounters : Form
    {
        private readonly PKMEditor PKME_Tabs;
        private SaveFile SAV => PKME_Tabs.RequestSaveFile;

        public SAV_Encounters(PKMEditor f1)
        {
            InitializeComponent();

            ToolStripMenuItem mnuView = new ToolStripMenuItem {Name = "mnuView", Text = "View"};

            ContextMenuStrip mnu = new ContextMenuStrip();
            mnu.Items.AddRange(new ToolStripItem[] { mnuView });

            PKME_Tabs = f1;

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
                };
            }

            Counter = L_Count.Text;
            L_Viewed.Text = string.Empty; // invis for now
            var hover = new ToolTip();
            L_Viewed.MouseEnter += (sender, e) => hover.SetToolTip(L_Viewed, L_Viewed.Text);
            PopulateComboBoxes();

            // Assign event handlers
            mnuView.Click += ClickView;

            // Add to main context menu

            // Assign to datagridview
            foreach (PictureBox p in PKXBOXES)
                p.ContextMenuStrip = mnu;

            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

            TLP_Filters.Controls.Add(TypeFilters = GetTypeFilters(), 2, TLP_Filters.RowCount - 1);

            // Load Data
            L_Count.Text = "Ready...";

            Menu_SearchSettings.DropDown.Closing += (sender, e) =>
            {
                if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                    e.Cancel = true;
            };
            CenterToParent();
        }

        private static FlowLayoutPanel GetTypeFilters()
        {
            var flp = new FlowLayoutPanel { Dock = DockStyle.Fill };
            var types = (EncounterOrder[])Enum.GetValues(typeof(EncounterOrder));
            var checks = types.Select(z => new CheckBox
            {
                Name = z.ToString(),
                Text = z.ToString(),
                AutoSize = true,
                Checked = true,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
            }).ToArray();
            foreach (var chk in checks)
            {
                flp.Controls.Add(chk);
                flp.SetFlowBreak(chk, true);
            }
            flp.AutoSize = true;
            return flp;
        }

        private EncounterOrder[] GetTypes()
        {
            return TypeFilters.Controls.OfType<CheckBox>().Where(z => z.Checked).Select(z => z.Name)
                .Select(z => (EncounterOrder)Enum.Parse(typeof(EncounterOrder), z)).ToArray();
        }

        private readonly PictureBox[] PKXBOXES;
        private List<IEncounterable> Results;
        private int slotSelected = -1; // = null;
        private Image slotColor;
        private const int RES_MAX = 66;
        private const int RES_MIN = 6;
        private readonly string Counter;
        private readonly FlowLayoutPanel TypeFilters;

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

            var enc = Results[index];
            var pk = enc.ConvertToPKM(SAV);
            pk.RefreshChecksum();
            PKME_Tabs.PopulateFields(pk, false);
            slotSelected = index;
            slotColor = Properties.Resources.slotView;
            FillPKXBoxes(SCR_Box.Value);
        }

        private void PopulateComboBoxes()
        {
            // Set the Text
            CB_Species.InitializeBinding();
            CB_GameOrigin.InitializeBinding();

            var Any = new ComboItem {Text = MsgAny, Value = -1};

            var DS_Species = new List<ComboItem>(GameInfo.SpeciesDataSource);
            DS_Species.RemoveAt(0); DS_Species.Insert(0, Any); CB_Species.DataSource = DS_Species;

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

            var DS_Version = new List<ComboItem>(GameInfo.VersionDataSource);
            DS_Version.Insert(0, Any); CB_GameOrigin.DataSource = DS_Version;

            // Trigger a Reset
            ResetFilters(null, EventArgs.Empty);
        }

        private void ResetFilters(object sender, EventArgs e)
        {
            CB_Species.SelectedIndex = 0;
            CB_Move1.SelectedIndex = CB_Move2.SelectedIndex = CB_Move3.SelectedIndex = CB_Move4.SelectedIndex = 0;
            CB_GameOrigin.SelectedIndex = 0;

            RTB_Instructions.Clear();
            if (sender == null)
                return; // still starting up
            foreach (var chk in TypeFilters.Controls.OfType<CheckBox>())
                chk.Checked = true;

            System.Media.SystemSounds.Asterisk.Play();
        }

        // View Updates
        private IEnumerable<IEncounterable> SearchDatabase()
        {
            var settings = GetSearchSettings();

            var moves = settings.Moves.ToArray();
            var pk = SAV.BlankPKM;

            var species = settings.Species <= 0 ? Enumerable.Range(1, SAV.MaxSpeciesID) : new[] { settings.Species };
            var versions = settings.GetVersions(SAV);
            var results = species.SelectMany(z => GetEncounters(z, moves, pk, versions));
            if (settings.SearchEgg != null)
                results = results.Where(z => z.EggEncounter == settings.SearchEgg);

            // return filtered results
            var comparer = new ReferenceComparer<IEncounterable>();
            return results.Distinct(comparer); // only distinct objects
        }

        private class ReferenceComparer<T> : IEqualityComparer<T>
        {
            public bool Equals(T x, T y) => RuntimeHelpers.GetHashCode(x).Equals(RuntimeHelpers.GetHashCode(y));
            public int GetHashCode(T obj) => RuntimeHelpers.GetHashCode(obj);
        }

        private static IEnumerable<IEncounterable> GetEncounters(int species, int[] moves, PKM pk, IReadOnlyList<GameVersion> vers)
        {
            pk.Species = species;
            return EncounterMovesetGenerator.GenerateEncounters(pk, moves, vers);
        }

        private SearchSettings GetSearchSettings()
        {
            var settings = new SearchSettings
            {
                Format = SAV.Generation, // 0->(n-1) => 1->n
                Generation = SAV.Generation,

                Species = WinFormsUtil.GetIndex(CB_Species),

                BatchInstructions = RTB_Instructions.Lines,
                Version = WinFormsUtil.GetIndex(CB_GameOrigin),
            };

            settings.AddMove(WinFormsUtil.GetIndex(CB_Move1));
            settings.AddMove(WinFormsUtil.GetIndex(CB_Move2));
            settings.AddMove(WinFormsUtil.GetIndex(CB_Move3));
            settings.AddMove(WinFormsUtil.GetIndex(CB_Move4));

            if (CHK_IsEgg.CheckState != CheckState.Indeterminate)
                settings.SearchEgg = CHK_IsEgg.CheckState == CheckState.Checked;

            return settings;
        }

        private async void B_Search_Click(object sender, EventArgs e)
        {
            B_Search.Enabled = false;
            EncounterMovesetGenerator.PriorityList = GetTypes();
            var search = SearchDatabase();

            var results = await Task.Run(() => search.ToList()).ConfigureAwait(true);

            if (results.Count == 0)
                WinFormsUtil.Alert(MsgDBSearchNone);

            SetResults(results); // updates Count Label as well.
            System.Media.SystemSounds.Asterisk.Play();
            B_Search.Enabled = true;
            EncounterMovesetGenerator.ResetFilters();
        }

        private void UpdateScroll(object sender, ScrollEventArgs e)
        {
            if (e.OldValue != e.NewValue)
                FillPKXBoxes(e.NewValue);
        }

        private void SetResults(List<IEncounterable> res)
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
            {
                var enc = Results[i + begin];
                var form = GetForm(enc);
                PKXBOXES[i].Image = SpriteUtil.GetSprite(enc.Species, form, 0, 0, enc.EggEncounter, false, enc is IGeneration g ? g.Generation : -1);
            }
            for (int i = end; i < RES_MAX; i++)
                PKXBOXES[i].Image = null;

            for (int i = 0; i < RES_MAX; i++)
                PKXBOXES[i].BackgroundImage = Properties.Resources.slotTrans;
            if (slotSelected != -1 && slotSelected >= begin && slotSelected < begin + RES_MAX)
                PKXBOXES[slotSelected - begin].BackgroundImage = slotColor ?? Properties.Resources.slotView;
        }

        private static int GetForm(IEncounterable enc)
        {
            switch (enc)
            {
                case EncounterSlot s: return s.Form;
                case EncounterStatic s: return s.Form;
                case MysteryGift m: return m.Form;
                case EncounterTrade t: return t.Form;
                default:
                    return 0;
            }
        }

        private void Menu_SearchAdvanced_Click(object sender, EventArgs e)
        {
            // todo
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
    }
}
