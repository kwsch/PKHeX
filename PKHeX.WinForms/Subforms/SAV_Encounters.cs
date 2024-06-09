using PKHeX.Core;
using PKHeX.Core.Searching;
using PKHeX.WinForms.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PKHeX.Drawing.PokeSprite;
using PKHeX.WinForms.Properties;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms;

public partial class SAV_Encounters : Form
{
    private readonly PKMEditor PKME_Tabs;
    private SaveFile SAV => PKME_Tabs.RequestSaveFile;
    private readonly SummaryPreviewer ShowSet = new();
    private readonly TrainerDatabase Trainers;
    private readonly CancellationTokenSource TokenSource = new();
    private readonly EntityInstructionBuilder UC_Builder;

    private const int GridWidth = 6;
    private const int GridHeight = 11;

    public SAV_Encounters(PKMEditor f1, TrainerDatabase db)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        UC_Builder = new EntityInstructionBuilder(() => f1.PreparePKM())
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            Width = Tab_Advanced.Width,
            Dock = DockStyle.Top,
            ReadOnly = true,
        };
        Tab_Advanced.Controls.Add(UC_Builder);
        UC_Builder.SendToBack();

        PKME_Tabs = f1;
        Trainers = db;

        var grid = EncounterPokeGrid;
        var smallWidth = grid.Width;
        var smallHeight = grid.Height;
        grid.InitializeGrid(GridWidth, GridHeight, SpriteUtil.Spriter);
        grid.SetBackground(Resources.box_wp_clean);
        var newWidth = grid.Width;
        var newHeight = grid.Height;
        var wdelta = newWidth - smallWidth;
        if (wdelta != 0)
            Width += wdelta;
        var hdelta = newHeight - smallHeight;
        if (hdelta != 0)
            Height += hdelta;

        PKXBOXES = [..grid.Entries];

        // Enable Scrolling when hovered over
        foreach (var slot in PKXBOXES)
        {
            // Enable Click
            slot.MouseClick += (_, e) =>
            {
                if (ModifierKeys == Keys.Control)
                    ClickView(slot, e);
            };
            slot.Enter += (_, _) =>
            {
                var index = Array.IndexOf(PKXBOXES, slot);
                if (index < 0)
                    return;
                index += (SCR_Box.Value * RES_MIN);
                if (index >= Results.Count)
                    return;

                var enc = Results[index];
                slot.AccessibleDescription = string.Join(Environment.NewLine, enc.GetTextLines());
            };
            slot.ContextMenuStrip = mnu;
            if (Main.Settings.Hover.HoverSlotShowText)
                slot.MouseEnter += (_, _) => ShowHoverTextForSlot(slot);
        }

        Counter = L_Count.Text;
        L_Viewed.Text = string.Empty; // invisible for now
        L_Viewed.MouseEnter += (_, _) => hover.SetToolTip(L_Viewed, L_Viewed.Text);
        PopulateComboBoxes();

        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        GetTypeFilters();

        // Load Data
        L_Count.Text = "Ready...";

        CenterToParent();
    }

    private void GetTypeFilters()
    {
        var types = Enum.GetValues<EncounterTypeGroup>();
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

    private EncounterTypeGroup[] GetTypes()
    {
        return TypeFilters.Controls.OfType<CheckBox>().Where(z => z.Checked).Select(z => z.Name)
            .Select(z => (EncounterTypeGroup)Enum.Parse(typeof(EncounterTypeGroup), z)).ToArray();
    }

    private readonly PictureBox[] PKXBOXES;
    private List<IEncounterInfo> Results = [];
    private int slotSelected = -1; // = null;
    private Image? slotColor;
    private const int RES_MIN = GridWidth * 1;
    private const int RES_MAX = GridWidth * GridHeight;
    private readonly string Counter;

    private bool GetShiftedIndex(ref int index)
    {
        if (index >= RES_MAX)
            return false;
        index += SCR_Box.Value * RES_MIN;
        return index < Results.Count;
    }

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
        var criteria = GetCriteria(enc, Main.Settings.EncounterDb);
        var trainer = Trainers.GetTrainer(enc.Version, enc.Generation <= 2 ? (LanguageID)SAV.Language : null) ?? SAV;
        var pk = enc.ConvertToPKM(trainer, criteria);
        pk.RefreshChecksum();
        PKME_Tabs.PopulateFields(pk, false);
        slotSelected = index;
        slotColor = SpriteUtil.Spriter.View;
        FillPKXBoxes(SCR_Box.Value);
    }

    private EncounterCriteria GetCriteria(ISpeciesForm enc, EncounterDatabaseSettings settings)
    {
        if (!settings.UseTabsAsCriteria)
            return EncounterCriteria.Unrestricted;

        var editor = PKME_Tabs.Data;
        var tree = EvolutionTree.GetEvolutionTree(editor.Context);
        bool isInChain = tree.IsSpeciesDerivedFrom(editor.Species, editor.Form, enc.Species, enc.Form);

        if (!settings.UseTabsAsCriteriaAnySpecies)
        {
            if (!isInChain)
                return EncounterCriteria.Unrestricted;
        }

        var set = new ShowdownSet(editor);
        var criteria = EncounterCriteria.GetCriteria(set, editor.PersonalInfo);
        if (!isInChain)
            criteria = criteria with { Gender = default }; // Genderless tabs and a gendered enc -> let's play safe.
        return criteria;
    }

    private void PopulateComboBoxes()
    {
        // Set the Text
        CB_Species.InitializeBinding();
        CB_GameOrigin.InitializeBinding();

        var Any = new ComboItem(MsgAny, 0);

        var DS_Species = new List<ComboItem>(GameInfo.SpeciesDataSource);
        DS_Species.RemoveAt(0); DS_Species.Insert(0, Any); CB_Species.DataSource = DS_Species;

        // Set the Move ComboBoxes too.
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
        DS_Version.Insert(0, Any);
        DS_Version.RemoveAt(DS_Version.Count - 1);
        CB_GameOrigin.DataSource = DS_Version;

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
    private IEnumerable<IEncounterInfo> SearchDatabase(CancellationToken token)
    {
        var settings = GetSearchSettings();

        // If nothing is specified, instead of just returning all possible encounters, just return nothing.
        if (settings is { Species: 0, Moves.Count: 0 } && Main.Settings.EncounterDb.ReturnNoneIfEmptySearch)
            return [];
        var pk = SAV.BlankPKM;

        var moves = settings.Moves.ToArray();
        var versions = settings.GetVersions(SAV);
        var species = settings.Species == 0 ? GetFullRange(SAV.MaxSpeciesID) : [settings.Species];
        var results = GetAllSpeciesFormEncounters(species, SAV.Personal, versions, moves, pk, token);
        if (settings.SearchEgg != null)
            results = results.Where(z => z.IsEgg == settings.SearchEgg);
        if (settings.SearchShiny != null)
            results = results.Where(z => z.IsShiny == settings.SearchShiny);

        // return filtered results
        var comparer = new ReferenceComparer<IEncounterInfo>();
        results = results.Distinct(comparer); // only distinct objects

        static Func<IEncounterInfo, bool> IsPresent<TTable>(TTable pt) where TTable : IPersonalTable => z =>
        {
            if (pt.IsPresentInGame(z.Species, z.Form))
                return true;
            return z is IEncounterFormRandom { IsRandomUnspecificForm: true } && pt.IsSpeciesInGame(z.Species);
        };
        if (Main.Settings.EncounterDb.FilterUnavailableSpecies)
        {
            results = SAV switch
            {
                SAV9SV s9 => results.Where(IsPresent(s9.Personal)),
                SAV8SWSH s8 => results.Where(IsPresent(s8.Personal)),
                SAV8BS b8 => results.Where(IsPresent(b8.Personal)),
                SAV8LA a8 => results.Where(IsPresent(a8.Personal)),
                _ => results.Where(z => z.Generation <= 7),
            };
        }

        if (token.IsCancellationRequested)
            return results;

        ReadOnlySpan<char> batchText = RTB_Instructions.Text;
        if (batchText.Length != 0 && !StringInstructionSet.HasEmptyLine(batchText))
        {
            var filters = StringInstruction.GetFilters(batchText);
            BatchEditing.ScreenStrings(filters);
            results = results.Where(enc => BatchEditing.IsFilterMatch(filters, enc)); // Compare across all filters
        }

        return results;
    }

    private static IEnumerable<ushort> GetFullRange(int max)
    {
        for (ushort i = 1; i <= max; i++)
            yield return i;
    }

    private IEnumerable<IEncounterInfo> GetAllSpeciesFormEncounters(IEnumerable<ushort> species, IPersonalTable pt, IReadOnlyList<GameVersion> versions, ReadOnlyMemory<ushort> moves, PKM pk, CancellationToken token)
    {
        foreach (var s in species)
        {
            if (token.IsCancellationRequested)
                break;

            var pi = pt.GetFormEntry(s, 0);
            var fc = pi.FormCount;
            if (fc == 0 && !Main.Settings.EncounterDb.FilterUnavailableSpecies) // not present in game
            {
                // try again using past-gen table
                pi = PersonalTable.USUM.GetFormEntry(s, 0);
                fc = pi.FormCount;
            }
            for (byte f = 0; f < fc; f++)
            {
                if (FormInfo.IsBattleOnlyForm(s, f, pk.Format))
                    continue;
                var encs = GetEncounters(s, f, moves, pk, versions);
                foreach (var enc in encs)
                    yield return enc;
            }
        }
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

        public int GetHashCode(T obj) => RuntimeHelpers.GetHashCode(obj);
    }

    private IEnumerable<IEncounterInfo> GetEncounters(ushort species, byte form, ReadOnlyMemory<ushort> moves, PKM pk, IReadOnlyList<GameVersion> vers)
    {
        pk.Species = species;
        pk.Form = form;
        pk.SetGender(pk.GetSaneGender());
        EncounterMovesetGenerator.OptimizeCriteria(pk, SAV);
        return EncounterMovesetGenerator.GenerateEncounters(pk, moves, vers);
    }

    private SearchSettings GetSearchSettings()
    {
        var settings = new SearchSettings
        {
            Format = SAV.Generation, // 0->(n-1) => 1->n
            Generation = SAV.Generation,

            Species = GetU16(CB_Species),

            BatchInstructions = RTB_Instructions.Text,
            Version = (GameVersion)WinFormsUtil.GetIndex(CB_GameOrigin),
        };

        static ushort GetU16(ListControl cb)
        {
            var val = WinFormsUtil.GetIndex(cb);
            if (val <= 0)
                return 0;
            return (ushort)val;
        }

        settings.AddMove(GetU16(CB_Move1));
        settings.AddMove(GetU16(CB_Move2));
        settings.AddMove(GetU16(CB_Move3));
        settings.AddMove(GetU16(CB_Move4));

        if (CHK_IsEgg.CheckState != CheckState.Indeterminate)
            settings.SearchEgg = CHK_IsEgg.CheckState == CheckState.Checked;

        if (CHK_Shiny.CheckState != CheckState.Indeterminate)
            settings.SearchShiny = CHK_Shiny.CheckState == CheckState.Checked;

        return settings;
    }

    private async void B_Search_Click(object sender, EventArgs e)
    {
        B_Search.Enabled = false;
        EncounterMovesetGenerator.PriorityList = GetTypes();

        var token = TokenSource.Token;
        var search = SearchDatabase(token);
        if (token.IsCancellationRequested)
        {
            EncounterMovesetGenerator.ResetFilters();
            return;
        }

        var results = await Task.Run(() => search.ToList(), token).ConfigureAwait(true);
        if (token.IsCancellationRequested)
        {
            EncounterMovesetGenerator.ResetFilters();
            return;
        }

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
        var boxes = PKXBOXES;
        if (Results.Count == 0)
        {
            for (int i = 0; i < RES_MAX; i++)
            {
                boxes[i].Image = null;
                boxes[i].BackgroundImage = null;
            }
            return;
        }

        // Load new sprites
        int begin = start * RES_MIN;
        int end = Math.Min(RES_MAX, Results.Count - begin);
        for (int i = 0; i < end; i++)
        {
            var pb = boxes[i];
            var enc = Results[i + begin];
            pb.Image = enc.Sprite();
        }

        // Clear empty slots
        for (int i = end; i < RES_MAX; i++)
            boxes[i].Image = null;

        // Reset backgrounds for all
        for (int i = 0; i < RES_MAX; i++)
            boxes[i].BackgroundImage = SpriteUtil.Spriter.Transparent;

        // Reload last viewed index's background if still within view
        if (slotSelected != -1 && slotSelected >= begin && slotSelected < begin + RES_MAX)
            boxes[slotSelected - begin].BackgroundImage = slotColor ?? SpriteUtil.Spriter.View;
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

    private void ShowHoverTextForSlot(PictureBox pb)
    {
        int index = Array.IndexOf(PKXBOXES, pb);
        if (!GetShiftedIndex(ref index))
            return;

        ShowSet.Show(pb, Results[index]);
    }

    private void SAV_Encounters_FormClosing(object sender, FormClosingEventArgs e) => TokenSource.Cancel();

    private void B_Add_Click(object sender, EventArgs e)
    {
        var s = UC_Builder.Create();
        if (s.Length == 0)
        { WinFormsUtil.Alert(MsgBEPropertyInvalid); return; }

        // If we already have text, add a new line (except if the last line is blank).
        var tb = RTB_Instructions;
        var batchText = tb.Text;
        if (batchText.Length != 0 && !batchText.EndsWith('\n'))
            tb.AppendText(Environment.NewLine);
        tb.AppendText(s);
    }
}
