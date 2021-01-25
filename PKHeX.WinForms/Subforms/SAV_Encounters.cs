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
using PKHeX.Drawing;
using PKHeX.WinForms.Properties;
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

            PKME_Tabs = f1;

            var grid = EncounterPokeGrid;
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
                    if (ModifierKeys == Keys.Control)
                        ClickView(sender, e);
                };
                slot.ContextMenuStrip = mnu;
            }

            Counter = L_Count.Text;
            L_Viewed.Text = string.Empty; // invis for now
            L_Viewed.MouseEnter += (sender, e) => hover.SetToolTip(L_Viewed, L_Viewed.Text);
            PopulateComboBoxes();

            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            GetTypeFilters();

            // Load Data
            L_Count.Text = "Ready...";

            CenterToParent();
        }

        private void GetTypeFilters()
        {
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
                TypeFilters.Controls.Add(chk);
                TypeFilters.SetFlowBreak(chk, true);
            }
        }

        private EncounterOrder[] GetTypes()
        {
            return TypeFilters.Controls.OfType<CheckBox>().Where(z => z.Checked).Select(z => z.Name)
                .Select(z => (EncounterOrder)Enum.Parse(typeof(EncounterOrder), z)).ToArray();
        }

        private readonly PictureBox[] PKXBOXES;
        private List<IEncounterInfo> Results = new();
        private int slotSelected = -1; // = null;
        private Image? slotColor;
        private const int RES_MAX = 66;
        private const int RES_MIN = 6;
        private readonly string Counter;

        // Important Events
        private void ClickView(object sender, EventArgs e)
        {
            var pb = WinFormsUtil.GetUnderlyingControl<PictureBox>(sender);
            int index = Array.IndexOf(PKXBOXES, pb);
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
            slotColor = SpriteUtil.Spriter.View;
            FillPKXBoxes(SCR_Box.Value);
        }

        private void PopulateComboBoxes()
        {
            // Set the Text
            CB_Species.InitializeBinding();
            CB_GameOrigin.InitializeBinding();

            var Any = new ComboItem(MsgAny, -1);

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
            ResetFilters(this, EventArgs.Empty);
        }

        private void ResetFilters(object sender, EventArgs e)
        {
            CB_Species.SelectedIndex = 0;
            CB_Move1.SelectedIndex = CB_Move2.SelectedIndex = CB_Move3.SelectedIndex = CB_Move4.SelectedIndex = 0;
            CB_GameOrigin.SelectedIndex = 0;

            RTB_Instructions.Clear();
            if (sender == this)
                return; // still starting up
            foreach (var chk in TypeFilters.Controls.OfType<CheckBox>())
                chk.Checked = true;

            System.Media.SystemSounds.Asterisk.Play();
        }

        // View Updates
        private IEnumerable<IEncounterInfo> SearchDatabase()
        {
            var settings = GetSearchSettings();
            var moves = settings.Moves.ToArray();

            // If nothing is specified, instead of just returning all possible encounters, just return nothing.
            if (settings.Species <= 0 && moves.Length == 0)
                return Array.Empty<IEncounterInfo>();
            var pk = SAV.BlankPKM;

            var species = settings.Species <= 0 ? Enumerable.Range(1, SAV.MaxSpeciesID) : new[] { settings.Species };
            var versions = settings.GetVersions(SAV);
            var results = species.SelectMany(z => GetEncounters(z, moves, pk, versions));
            if (settings.SearchEgg != null)
                results = results.Where(z => z.EggEncounter == settings.SearchEgg);

            // return filtered results
            var comparer = new ReferenceComparer<IEncounterInfo>();
            results = results.Distinct(comparer); // only distinct objects

            // when all sprites in new size are available, remove this filter
            results = SAV is SAV8SWSH
                ? results.Where(z => ((PersonalInfoSWSH)PersonalTable.SWSH.GetFormEntry(z.Species, z.Form)).IsPresentInGame)
                : results.Where(z => z.Generation <= 7);
            return results;
        }

        private sealed class ReferenceComparer<T> : IEqualityComparer<T> where T : class
        {
            public bool Equals(T? x, T? y)
            {
                if (x == null)
                    return false;
                if (y == null)
                    return false;
                return RuntimeHelpers.GetHashCode(x).Equals(RuntimeHelpers.GetHashCode(y));
            }

            public int GetHashCode(T obj)
            {
                if (obj == null) throw new ArgumentNullException(nameof(obj));
                return RuntimeHelpers.GetHashCode(obj);
            }
        }

        private static IEnumerable<IEncounterInfo> GetEncounters(int species, int[] moves, PKM pk, IReadOnlyList<GameVersion> vers)
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

        private void SetResults(List<IEncounterInfo> res)
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
            {
                var enc = Results[i + begin];
                PKXBOXES[i].Image = SpriteUtil.GetSprite(enc.Species, enc.Form, 0, 0, 0, enc.EggEncounter, false, enc.Generation);
            }
            for (int i = end; i < RES_MAX; i++)
                PKXBOXES[i].Image = null;

            for (int i = 0; i < RES_MAX; i++)
                PKXBOXES[i].BackgroundImage = SpriteUtil.Spriter.Transparent;
            if (slotSelected != -1 && slotSelected >= begin && slotSelected < begin + RES_MAX)
                PKXBOXES[slotSelected - begin].BackgroundImage = slotColor ?? SpriteUtil.Spriter.View;
        }

        private void Menu_SearchAdvanced_Click(object sender, EventArgs e)
        {
            // todo
        }

        private void Menu_Exit_Click(object sender, EventArgs e) => Close();

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (!EncounterPokeGrid.RectangleToScreen(EncounterPokeGrid.ClientRectangle).Contains(MousePosition))
                return;
            int oldval = SCR_Box.Value;
            int newval = oldval + (e.Delta < 0 ? 1 : -1);
            if (newval >= SCR_Box.Minimum && SCR_Box.Maximum >= newval)
                FillPKXBoxes(SCR_Box.Value = newval);
        }
    }
}
