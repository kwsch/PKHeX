using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_PokedexLA : Form
{
    private readonly SAV8LA Origin;
    private readonly SAV8LA SAV;
    private readonly PokedexSave8a Dex;
    private readonly CheckBox[] CHK_SeenWild;
    private readonly CheckBox[] CHK_Obtained;
    private readonly CheckBox[] CHK_CaughtWild;

    private readonly Controls.PokedexResearchTask8aPanel[] TaskControls;

    private readonly ushort[] SpeciesToDex;
    private readonly ushort[] DexToSpecies;

    private int lastIndex = -1;
    private int lastForm = -1;
    private bool Editing;
    private readonly bool CanSave;

    private readonly List<ComboItem> DisplayedForms;

    private readonly string[] TaskDescriptions = Util.GetStringList("tasks8a", Main.CurrentLanguage);
    private readonly string[] SpeciesQuests = Util.GetStringList("species_tasks8a", Main.CurrentLanguage);
    private readonly string[] TimeTaskDescriptions = Util.GetStringList("time_tasks8a", Main.CurrentLanguage);

    public SAV_PokedexLA(SAV8LA sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV8LA)(Origin = sav).Clone();
        Dex = SAV.Blocks.PokedexSave;
        var speciesNames = GameInfo.Strings.Species;
        CHK_SeenWild = [CHK_S0, CHK_S1, CHK_S2, CHK_S3, CHK_S4, CHK_S5, CHK_S6, CHK_S7];
        CHK_Obtained = [CHK_O0, CHK_O1, CHK_O2, CHK_O3, CHK_O4, CHK_O5, CHK_O6, CHK_O7];
        CHK_CaughtWild = [CHK_C0, CHK_C1, CHK_C2, CHK_C3, CHK_C4, CHK_C5, CHK_C6, CHK_C7];

        TaskControls =
        [
            PRT_1,
            PRT_2,
            PRT_3,
            PRT_4,
            PRT_5,
            PRT_6,
            PRT_7,
            PRT_8,
            PRT_9,
            PRT_10,
        ];

        foreach (var tc in TaskControls)
        {
            tc.Visible = false;
            tc.SetStrings(TaskDescriptions, SpeciesQuests, TimeTaskDescriptions);
        }

        SpeciesToDex = new ushort[SAV.Personal.MaxSpeciesID + 1];

        var maxDex = 0;
        for (ushort s = 1; s <= SAV.Personal.MaxSpeciesID; s++)
        {
            var hisuiDex = PokedexSave8a.GetDexIndex(PokedexType8a.Hisui, s);
            if (hisuiDex == 0)
                continue;

            SpeciesToDex[s] = hisuiDex;
            if (hisuiDex > maxDex)
                maxDex = hisuiDex;
        }

        DexToSpecies = new ushort[maxDex + 1];
        for (ushort s = 1; s <= SAV.Personal.MaxSpeciesID; s++)
        {
            if (SpeciesToDex[s] != 0)
                DexToSpecies[SpeciesToDex[s]] = s;
        }

        Editing = true;
        // Clear Listbox and ComboBox
        LB_Species.Items.Clear();
        CB_Species.Items.Clear();

        // Fill List
        CB_Species.InitializeBinding();
        var species = GameInfo.FilteredSources.Species.Where(z => PokedexSave8a.GetDexIndex(PokedexType8a.Hisui, (ushort)z.Value) != 0).ToArray();
        CB_Species.DataSource = new BindingSource(species, null);

        CB_DisplayForm.InitializeBinding();
        DisplayedForms = [new(GameInfo.Strings.types[0], 0)];
        CB_DisplayForm.DataSource = new BindingSource(DisplayedForms, null);

        for (var d = 1; d < DexToSpecies.Length; d++)
            LB_Species.Items.Add($"{d:000} - {speciesNames[DexToSpecies[d]]}");

        Editing = false;
        LB_Species.SelectedIndex = 0;
        CB_Species.KeyDown += WinFormsUtil.RemoveDropCB;
        CanSave = true;
    }

    private void ChangeCBSpecies(object sender, EventArgs e)
    {
        if (Editing)
            return;

        var species = WinFormsUtil.GetIndex(CB_Species);
        var index = SpeciesToDex[species] - 1;
        if (LB_Species.SelectedIndex != index)
            LB_Species.SelectedIndex = index; // trigger event
    }

    private void ChangeLBSpecies(object sender, EventArgs e)
    {
        if (Editing || LB_Species.SelectedIndex < 0)
            return;

        SetEntry(lastIndex, lastForm);
        Editing = true;
        SuspendLayout();

        lastIndex = LB_Species.SelectedIndex;
        FillLBForms(lastIndex);
        FillResearchTasks(lastIndex);
        GetEntry(lastIndex, lastForm);
        ResumeLayout();
        Editing = false;
    }

    private void ChangeLBForms(object sender, EventArgs e)
    {
        if (Editing)
            return;

        SetEntry(lastIndex, lastForm);
        lastForm = LB_Forms.SelectedIndex;

        Editing = true;
        SuspendLayout();
        GetEntry(lastIndex, lastForm);
        ResumeLayout();
        Editing = false;
    }

    private bool FillLBForms(int index)
    {
        LB_Forms.DataSource = null;
        LB_Forms.Items.Clear();

        DisplayedForms.Clear();
        DisplayedForms.Add(new ComboItem(GameInfo.Strings.types[0], 0));
        CB_DisplayForm.DataSource = new BindingSource(DisplayedForms, null);

        lastForm = 0;

        ushort species = DexToSpecies[index + 1];
        bool hasForms = FormInfo.HasFormSelection(SAV.Personal[species], species, 8);
        LB_Forms.Enabled = CB_DisplayForm.Enabled = hasForms;
        if (!hasForms)
            return false;

        var ds = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, Main.GenderSymbols, SAV.Context);
        if (ds.Length == 1 && string.IsNullOrEmpty(ds[0]))
        {
            // empty
            LB_Forms.Enabled = CB_DisplayForm.Enabled = false;
            return false;
        }

        // Sanitize forms to only show entries with form storage
        var formCount = SAV.Personal[species].FormCount;
        var sanitized = new List<string>();
        DisplayedForms.Clear();
        for (byte form = 0; form < formCount; form++)
        {
            if (!Dex.HasFormStorage(species, form) || Dex.IsBlacklisted(species, form))
                continue;

            sanitized.Add(ds[form]);
            DisplayedForms.Add(new ComboItem(ds[form], form));
        }

        CB_DisplayForm.DataSource = new BindingSource(DisplayedForms, null);
        LB_Forms.DataSource = sanitized;
        LB_Forms.SelectedIndex = 0;

        return true;
    }

    private void FillResearchTasks(int index)
    {
        var species = DexToSpecies[index + 1];
        var tasks = PokedexConstants8a.ResearchTasks[index];

        for (var i = 0; i < tasks.Length; i++)
        {
            var tc = TaskControls[i];
            tc.Visible = false;
            Dex.GetResearchTaskLevel(species, i, out var repLevel, out _, out _);
            tc.SetTask(species, tasks[i], repLevel);
            tc.Visible = true;
        }

        for (var i = tasks.Length; i < TaskControls.Length; i++)
            TaskControls[i].Visible = false;
    }

    private void GetEntry(int index, int formIndex)
    {
        var species = DexToSpecies[index + 1];
        var form = (byte)DisplayedForms[formIndex].Value;

        // Flags
        var seenWild = Dex.GetPokeSeenInWildFlags(species, form);
        var obtain = Dex.GetPokeObtainFlags(species, form);
        var caughtWild = Dex.GetPokeCaughtInWildFlags(species, form);
        CHK_Solitude.Checked = Dex.GetSolitudeComplete(species);
        for (var i = 0; i < CHK_SeenWild.Length; ++i)
        {
            CHK_SeenWild[i].Checked = (seenWild & (1 << i)) != 0;
            CHK_Obtained[i].Checked = (obtain & (1 << i)) != 0;
            CHK_CaughtWild[i].Checked = (caughtWild & (1 << i)) != 0;
        }

        // Display
        if (CB_DisplayForm.Enabled)
        {
            var selectedForm = Dex.GetSelectedForm(species);
            CB_DisplayForm.SelectedIndex = 0;
            var items = CB_DisplayForm.Items;
            for (var i = 0; i < items.Count; ++i)
            {
                var item = items[i];
                if (item is not ComboItem cb)
                    throw new Exception("Invalid item type");
                if (cb.Value != selectedForm)
                    continue;

                CB_DisplayForm.SelectedIndex = i;
                break;
            }
        }

        CHK_A.Checked = Dex.GetSelectedAlpha(species);
        CHK_S.Checked = Dex.GetSelectedShiny(species);

        CHK_G.Enabled = PokedexSave8a.HasMultipleGenders(species);
        CHK_G.Checked = Dex.GetSelectedGender1(species);

        // Research
        var reportedRate = Dex.GetPokeResearchRate(species);
        var unreportedRate = reportedRate;
        for (var i = 0; i < PokedexConstants8a.ResearchTasks[index].Length; i++)
        {
            var unreportedLevels = Dex.GetResearchTaskLevel(species, i, out _, out var taskValue, out _);
            TaskControls[i].CurrentValue = taskValue;
            unreportedRate += unreportedLevels * TaskControls[i].PointsPerLevel;
        }

        MTB_UpdateIndex.Text = Dex.GetUpdateIndex(species).ToString();
        MTB_ResearchLevelReported.Text = reportedRate.ToString();
        MTB_ResearchLevelUnreported.Text = unreportedRate.ToString();

        CHK_Seen.Checked = Dex.HasPokeEverBeenUpdated(species);
        CHK_Complete.Checked = Dex.IsComplete(species);
        CHK_Perfect.Checked = Dex.IsPerfect(species);

        // Statistics
        Dex.GetSizeStatistics(species, form, out var hasMax, out var minHeight, out var maxHeight, out var minWeight, out var maxWeight);
        CHK_MinAndMax.Checked = hasMax;
        TB_MinHeight.Text = minHeight.ToString(CultureInfo.InvariantCulture);
        TB_MaxHeight.Text = maxHeight.ToString(CultureInfo.InvariantCulture);
        TB_MinWeight.Text = minWeight.ToString(CultureInfo.InvariantCulture);
        TB_MaxWeight.Text = maxWeight.ToString(CultureInfo.InvariantCulture);

        var pt = SAV.Personal;
        var pi = pt.GetFormEntry(species, form);
        var minTheoryHeight = PA8.GetHeightAbsolute(pi, 0x00).ToString(CultureInfo.InvariantCulture);
        var maxTheoryHeight = PA8.GetHeightAbsolute(pi, 0xFF).ToString(CultureInfo.InvariantCulture);
        var minTheoryWeight = PA8.GetWeightAbsolute(pi, 0x00, 0x00).ToString(CultureInfo.InvariantCulture);
        var maxTheoryWeight = PA8.GetWeightAbsolute(pi, 0xFF, 0xFF).ToString(CultureInfo.InvariantCulture);

        L_TheoryHeight.Text = $"Min: {minTheoryHeight}, Max: {maxTheoryHeight}";
        L_TheoryWeight.Text = $"Min: {minTheoryWeight}, Max: {maxTheoryWeight}";
    }

    private bool IsEntryEmpty(int index, int formIndex)
    {
        var species = DexToSpecies[index + 1];
        byte form = (byte)DisplayedForms[formIndex].Value;

        // Any seen/obtain flags
        for (var i = 0; i < CHK_SeenWild.Length; i++)
        {
            if (CHK_SeenWild[i].Checked)
                return false;
            if (CHK_Obtained[i].Checked)
                return false;
            if (CHK_CaughtWild[i].Checked)
                return false;
        }

        // Any display flags
        if ((CHK_G.Enabled && CHK_G.Checked) || CHK_S.Checked || CHK_A.Checked)
            return false;

        // Any research
        for (var i = 0; i < PokedexConstants8a.ResearchTasks[index].Length; i++)
        {
            Dex.GetResearchTaskLevel(species, i, out var reportedLevels, out _, out _);
            if (reportedLevels > 1)
                return false;
            if (TaskControls[i].CurrentValue != 0)
                return false;
        }

        if (CHK_Complete.Checked || CHK_Perfect.Checked)
            return false;

        // Any statistics
        Dex.GetSizeStatistics(species, form, out _, out var oldMinHeight, out var oldMaxHeight, out var oldMinWeight, out var oldMaxWeight);

        if (!float.TryParse(TB_MinHeight.Text, out var minHeight))
            minHeight = oldMinHeight;

        if (!float.TryParse(TB_MaxHeight.Text, out var maxHeight))
            maxHeight = oldMaxHeight;

        if (!float.TryParse(TB_MinWeight.Text, out var minWeight))
            minWeight = oldMinWeight;

        if (!float.TryParse(TB_MaxWeight.Text, out var maxWeight))
            maxWeight = oldMaxWeight;

        if (CHK_MinAndMax.Checked)
            return false;

        return minHeight == 0 && maxHeight == 0 && minWeight == 0 && maxWeight == 0;
    }

    private void SetEntry(int index, int formIndex)
    {
        if (!CanSave || Editing || index < 0 || formIndex < 0)
            return;

        var empty = IsEntryEmpty(index, formIndex);

        if (!CHK_Seen.Checked && empty)
            return;

        var species = DexToSpecies[index + 1];
        var form = (byte)DisplayedForms[formIndex].Value;

        if (!empty)
            Dex.SetPokeHasBeenUpdated(species);

        // Flags
        var seenWild = 0;
        var obtain = 0;
        var caughtWild = 0;
        for (var i = 0; i < CHK_SeenWild.Length; i++)
        {
            seenWild |= CHK_SeenWild[i].Checked ? (1 << i) : 0;
            obtain |= CHK_Obtained[i].Checked ? (1 << i) : 0;
            caughtWild |= CHK_CaughtWild[i].Checked ? (1 << i) : 0;
        }

        Dex.SetPokeSeenInWildFlags(species, form, (byte)seenWild);
        Dex.SetPokeObtainFlags(species, form, (byte)obtain);
        Dex.SetPokeCaughtInWildFlags(species, form, (byte)caughtWild);
        Dex.SetSolitudeComplete(species, CHK_Solitude.Checked);

        // Display
        var dispForm = form;
        if (CB_DisplayForm.Enabled)
            dispForm = (byte)WinFormsUtil.GetIndex(CB_DisplayForm);

        Dex.SetSelectedGenderForm(species, dispForm, CHK_G.Checked, CHK_S.Checked, CHK_A.Checked);

        // Set research
        for (var i = 0; i < PokedexConstants8a.ResearchTasks[index].Length; i++)
        {
            if (TaskControls[i].CanSetCurrentValue)
                Dex.SetResearchTaskProgressByForce(species, TaskControls[i].Task, TaskControls[i].CurrentValue);
        }

        // Statistics
        Dex.GetSizeStatistics(species, form, out _, out var oldMinHeight, out var oldMaxHeight, out var oldMinWeight, out var oldMaxWeight);

        if (!float.TryParse(TB_MinHeight.Text, out var minHeight))
            minHeight = oldMinHeight;

        if (!float.TryParse(TB_MaxHeight.Text, out var maxHeight))
            maxHeight = oldMaxHeight;

        if (!float.TryParse(TB_MinWeight.Text, out var minWeight))
            minWeight = oldMinWeight;

        if (!float.TryParse(TB_MaxWeight.Text, out var maxWeight))
            maxWeight = oldMaxWeight;

        Dex.SetSizeStatistics(species, form, CHK_MinAndMax.Checked, minHeight, maxHeight, minWeight, maxWeight);
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        SetEntry(lastIndex, lastForm);
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void CHK_ObtainFlag_Changed(object sender, EventArgs e)
    {
        if (Editing)
            return;

        var overrideObtainFlags = 0;
        for (var i = 0; i < CHK_Obtained.Length; i++)
        {
            if (CHK_Obtained[i].Checked)
                overrideObtainFlags |= (1 << i);
        }

        var tasks = PokedexConstants8a.ResearchTasks[lastIndex];
        var species = DexToSpecies[lastIndex + 1];
        var form = DisplayedForms[lastForm].Value;

        SuspendLayout();
        for (var i = 0; i < tasks.Length; i++)
        {
            if (tasks[i].Task != PokedexResearchTaskType8a.ObtainForms)
                continue;

            var formCount = Dex.GetObtainedFormCounts(species, form | (overrideObtainFlags << 16));
            var tc = TaskControls[i];
            if (tc.CurrentValue != formCount)
                tc.CurrentValue = formCount;
        }
        ResumeLayout();
    }

    private void B_Report_Click(object sender, EventArgs e)
    {
        // Set the entry
        SetEntry(lastIndex, lastForm);

        Editing = true;
        SuspendLayout();

        // Perform a report on the specific species
        var species = DexToSpecies[lastIndex + 1];
        Dex.UpdateSpecificReportPoke(species, out _);

        // Refresh all tasks
        FillResearchTasks(lastIndex);

        // Refresh all values
        GetEntry(lastIndex, lastForm);
        ResumeLayout();
        Editing = false;
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_AdvancedResearch_Click(object sender, EventArgs e)
    {
        // Set the entry
        SetEntry(lastIndex, lastForm);

        // Show detailed editor form
        using var form = new SAV_PokedexResearchEditorLA(SAV, DexToSpecies[lastIndex + 1], lastIndex, TaskDescriptions, TimeTaskDescriptions);
        form.ShowDialog();

        Editing = true;
        SuspendLayout();

        // Refresh all tasks
        FillResearchTasks(lastIndex);

        // Refresh all values
        GetEntry(lastIndex, lastForm);
        ResumeLayout();
        Editing = false;
    }
}
