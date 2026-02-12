using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Core.Searching;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms.Controls;

public partial class EntitySearchControl : UserControl
{
    private EntityContext SaveContext { get; set; } = Latest.Context;
    
    public EntitySearchControl() => InitializeComponent();

    /// <summary>
    /// Creates a filter function based on the current search settings, including any batch instructions.
    /// </summary>
    /// <param name="batchInstructions">Optional batch instructions to include in the search settings.</param>
    public Func<PKM, bool> GetFilter(string batchInstructions = "")
        => CreateSearchSettings(batchInstructions).CreateSearchPredicate();

    /// <summary>
    /// Populates combo box bindings with game data sources.
    /// </summary>
    public void PopulateComboBoxes(FilteredGameDataSource filtered)
    {
        CB_HeldItem.InitializeBinding();
        CB_Species.InitializeBinding();
        CB_Ability.InitializeBinding();
        CB_Nature.InitializeBinding();
        CB_GameOrigin.InitializeBinding();
        CB_HPType.InitializeBinding();
        CB_Format.InitializeBinding();

        var comboAny = new ComboItem(MsgAny, -1);

        var source = filtered.Source;
        var species = new List<ComboItem>(source.SpeciesDataSource)
        {
            [0] = comboAny
        };
        CB_Species.DataSource = species;

        var items = new List<ComboItem>(filtered.Items);
        items.Insert(0, comboAny);
        CB_HeldItem.DataSource = items;

        var natures = new List<ComboItem>(source.NatureDataSource);
        natures.Insert(0, comboAny);
        CB_Nature.DataSource = natures;

        var abilities = new List<ComboItem>(source.AbilityDataSource);
        abilities.Insert(0, comboAny);
        CB_Ability.DataSource = abilities;

        var versions = new List<ComboItem>(source.VersionDataSource);
        versions.Insert(0, comboAny);
        versions.RemoveAt(versions.Count - 1);
        CB_GameOrigin.DataSource = versions;

        var hptypes = source.Strings.HiddenPowerTypes;
        var types = Util.GetCBList(hptypes);
        types.Insert(0, comboAny);
        CB_HPType.DataSource = types;

        var moves = new List<ComboItem>(filtered.Moves);
        moves.RemoveAt(0);
        moves.Insert(0, comboAny);
        foreach (ComboBox cb in new[] { CB_Move1, CB_Move2, CB_Move3, CB_Move4 })
        {
            cb.InitializeBinding();
            cb.DataSource = new BindingSource(moves, string.Empty);
        }

        var contexts = new List<ComboItem>
        {
            new(MsgAny, (int)EntityContext.None)
        };

        foreach (var context in Enum.GetValues<EntityContext>())
        {
            if (context is EntityContext.None or EntityContext.SplitInvalid or EntityContext.MaxInvalid)
                continue;
            if (!context.IsValid || !context.ToString().StartsWith("Gen", StringComparison.Ordinal))
                continue;
            contexts.Add(new ComboItem(context.ToString(), (int)context));
        }

        CB_Format.DataSource = contexts;

        ResetFilters();
    }

    public void InitializeSelections(SaveFile sav, bool showContext = true)
    {
        SaveContext = sav.Context;
        if (sav.Generation >= 8)
        {
            CB_FormatComparator.SelectedIndex = 1; // ==
            CB_Format.SelectedValue = (int)sav.Context;
        }
        else
        {
            CB_FormatComparator.SelectedIndex = 3; // <=
        }
        L_Format.Visible = CB_FormatComparator.Visible = CB_Format.Visible = showContext;
    }
    
    /// <summary>
    /// Sets the localized text for the format "Any" option.
    /// </summary>
    public void SetFormatAnyText(string text)
    {
        if (CB_Format.DataSource is List<ComboItem> { Count: > 0 } list)
        {
            list[0] = new ComboItem(text, list[0].Value);
            CB_Format.DataSource = null;
            CB_Format.DataSource = list;
            CB_Format.InitializeBinding();
            return;
        }

        if (CB_Format.Items.Count > 0)
            CB_Format.Items[0] = text;
    }

    /// <summary>
    /// Resets filters to their default state.
    /// </summary>
    public void ResetFilters()
    {
        CHK_Shiny.Checked = CHK_IsEgg.Checked = true;
        CHK_Shiny.CheckState = CHK_IsEgg.CheckState = CheckState.Indeterminate;
        MT_ESV.Text = string.Empty;
        CB_HeldItem.SelectedIndex = 0;
        CB_Species.SelectedIndex = 0;
        CB_Ability.SelectedIndex = 0;
        CB_Nature.SelectedIndex = 0;
        CB_HPType.SelectedIndex = 0;
        TB_Nickname.Text = string.Empty;

        CB_Level.SelectedIndex = 0;
        TB_Level.Text = string.Empty;
        CB_EVTrain.SelectedIndex = 0;
        CB_IV.SelectedIndex = 0;

        CB_Move1.SelectedIndex = CB_Move2.SelectedIndex = CB_Move3.SelectedIndex = CB_Move4.SelectedIndex = 0;

        CB_GameOrigin.SelectedIndex = 0;
        CB_Generation.SelectedIndex = 0;

        MT_ESV.Visible = L_ESV.Visible = false;
    }

    /// <summary>
    /// Resets combo box selections and text selection state.
    /// </summary>
    public void ResetComboBoxSelections()
    {
        foreach (var cb in TLP_Filters.Controls.OfType<ComboBox>())
            cb.SelectedIndex = cb.SelectionLength = 0;
    }

    /// <summary>
    /// Creates search settings based on the current filter selection.
    /// </summary>
    public SearchSettings CreateSearchSettings(string batchInstructions)
    {
        var settings = new SearchSettings
        {
            Context = (EntityContext)WinFormsUtil.GetIndex(CB_Format),
            SearchContext = (SearchComparison)CB_FormatComparator.SelectedIndex,
            Generation = (byte)CB_Generation.SelectedIndex,

            Version = (GameVersion)WinFormsUtil.GetIndex(CB_GameOrigin),
            HiddenPowerType = WinFormsUtil.GetIndex(CB_HPType),

            Species = GetU16(CB_Species),
            Ability = WinFormsUtil.GetIndex(CB_Ability),
            Nature = (Nature)WinFormsUtil.GetIndex(CB_Nature),
            Item = WinFormsUtil.GetIndex(CB_HeldItem),
            Nickname = TB_Nickname.Text.Trim(),

            BatchInstructions = batchInstructions,

            Level = byte.TryParse(TB_Level.Text, out var lvl) ? lvl : null,
            SearchLevel = (SearchComparison)CB_Level.SelectedIndex,
            EVType = CB_EVTrain.SelectedIndex,
            IVType = CB_IV.SelectedIndex,
        };

        settings.AddMove(GetU16(CB_Move1));
        settings.AddMove(GetU16(CB_Move2));
        settings.AddMove(GetU16(CB_Move3));
        settings.AddMove(GetU16(CB_Move4));

        if (CHK_Shiny.CheckState != CheckState.Indeterminate)
            settings.SearchShiny = CHK_Shiny.CheckState == CheckState.Checked;

        if (CHK_IsEgg.CheckState != CheckState.Indeterminate)
        {
            settings.SearchEgg = CHK_IsEgg.CheckState == CheckState.Checked;
            if (int.TryParse(MT_ESV.Text, out int esv))
                settings.ESV = esv;
        }

        return settings;

        static ushort GetU16(ListControl cb)
        {
            var val = WinFormsUtil.GetIndex(cb);
            if (val <= 0)
                return 0;
            return (ushort)val;
        }
    }

    private void ToggleESV(object? sender, EventArgs e) => L_ESV.Visible = MT_ESV.Visible = CHK_IsEgg.CheckState == CheckState.Checked;

    private void ChangeLevel(object? sender, EventArgs e)
    {
        if (CB_Level.SelectedIndex == 0)
            TB_Level.Text = string.Empty;
    }

    private void ChangeGame(object? sender, EventArgs e)
    {
        if (CB_GameOrigin.SelectedIndex != 0)
            CB_Generation.SelectedIndex = 0;
    }

    private void ChangeGeneration(object? sender, EventArgs e)
    {
        if (CB_Generation.SelectedIndex != 0)
            CB_GameOrigin.SelectedIndex = 0;
    }

    private void ChangeFormatFilter(object? sender, EventArgs e)
    {
        if (CB_FormatComparator.SelectedIndex == 0)
        {
            CB_Format.Visible = false;
            CB_Format.SelectedIndex = 0;
        }
        else
        {
            CB_Format.Visible = true;
            CB_Format.SelectedValue = (int)SaveContext;
        }
    }
}
