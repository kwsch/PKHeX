using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using PKHeX.Core;
using PKHeX.Core.Searching;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Reusable entity filter model shared by the PKM Database view and the box-viewer
/// seek bar. Holds the filter inputs, their combo data sources, and converts them into
/// a <see cref="SearchSettings"/> / search predicate.
/// </summary>
public partial class EntityFilterViewModel : ViewModelBase
{
    private readonly SaveFile _sav;

    // Filtering properties (mapped to SearchSettings)
    [ObservableProperty] private int _species;
    [ObservableProperty] private int _nature;
    [ObservableProperty] private int _ability;
    [ObservableProperty] private int _item;
    [ObservableProperty] private string _nickname = string.Empty;
    [ObservableProperty] private bool? _isShiny;
    [ObservableProperty] private bool? _isLegal;
    [ObservableProperty] private bool? _isEgg;
    [ObservableProperty] private int _hiddenPowerType;
    [ObservableProperty] private int _level;
    [ObservableProperty] private int _levelComparison;
    [ObservableProperty] private int _ivType;
    [ObservableProperty] private int _evType;
    [ObservableProperty] private bool _esvEnabled;
    [ObservableProperty] private int _esv;

    /// <summary>
    /// Batch-editor instruction filter lines (e.g. <c>IVs=31</c>, <c>Move1=33</c>), one per line.
    /// Parsed by Core into a search predicate via <see cref="SearchSettings.BatchInstructions"/>.
    /// </summary>
    [ObservableProperty] private string _batchInstructions = string.Empty;

    // Data Sources for View
    [ObservableProperty] private IReadOnlyList<ComboItem> _speciesList = [];
    [ObservableProperty] private IReadOnlyList<ComboItem> _natureList = [];
    [ObservableProperty] private IReadOnlyList<ComboItem> _abilityList = [];
    [ObservableProperty] private IReadOnlyList<ComboItem> _itemList = [];
    [ObservableProperty] private IReadOnlyList<ComboItem> _hiddenPowerList = [];
    [ObservableProperty] private IReadOnlyList<ComboItem> _levelComparisonList = [];
    [ObservableProperty] private IReadOnlyList<ComboItem> _ivTypeList = [];
    [ObservableProperty] private IReadOnlyList<ComboItem> _evTypeList = [];

    public EntityFilterViewModel(SaveFile sav)
    {
        _sav = sav;

        // Wildcard values: Species=0, Nature=25 (Random), Ability=-1, Item=-1
        Species = 0;
        Nature = 25;
        Ability = -1;
        Item = -1;

        // Extended filter defaults ("Any"/disabled)
        HiddenPowerType = -1;       // -1 = any HP type
        LevelComparison = (int)SearchComparison.None; // None = don't filter by level
        Level = 0;
        IvType = 0;                 // 0 = any IV total bucket
        EvType = 0;                 // 0 = any EV total bucket
        EsvEnabled = false;
        Esv = 0;

        InitDataSources();
        WeakReferenceMessenger.Default.Register<LanguageChangedMessage>(this, (r, m) => RefreshLanguage());
    }

    private void InitDataSources()
    {
        SpeciesList = new List<ComboItem> { new("Any", 0) }.Concat(GameInfo.Sources.SpeciesDataSource).ToList();
        NatureList = new List<ComboItem> { new("Any", 25) }.Concat(GameInfo.Sources.NatureDataSource).ToList();
        AbilityList = new List<ComboItem> { new("Any", -1) }.Concat(GameInfo.Sources.AbilityDataSource).ToList();
        ItemList = new List<ComboItem> { new("Any", -1) }.Concat(GameInfo.Sources.GetItemDataSource(_sav.Version, _sav.Context, _sav.HeldItems)).ToList();

        // Hidden Power type: value is the 0-based HP type index (matches PKM.HPType); -1 = any.
        var hpTypes = new List<ComboItem> { new("Any", -1) };
        var hpNames = GameInfo.Strings.HiddenPowerTypes;
        for (int i = 0; i < hpNames.Length; i++)
            hpTypes.Add(new ComboItem(hpNames[i], i));
        HiddenPowerList = hpTypes;

        // Level comparison operands (mirrors SearchUtil.SatisfiesFilterLevel).
        LevelComparisonList = new List<ComboItem>
        {
            new("Any", (int)SearchComparison.None),
            new("= (equal)", (int)SearchComparison.Equals),
            new(">= (at least)", (int)SearchComparison.GreaterThanEquals),
            new("<= (at most)", (int)SearchComparison.LessThanEquals),
        };

        // IV total buckets (mirrors SearchUtil.SatisfiesFilterIVs); 0 = any.
        IvTypeList = new List<ComboItem>
        {
            new("Any", 0),
            new("<= 90", 1),
            new("91-120", 2),
            new("121-150", 3),
            new("151-179", 4),
            new("180+", 5),
            new("Flawless (186)", 6),
        };

        // EV total buckets (mirrors SearchUtil.SatisfiesFilterEVs); 0 = any.
        EvTypeList = new List<ComboItem>
        {
            new("Any", 0),
            new("None (0)", 1),
            new("Some (1-127)", 2),
            new("Half (128-507)", 3),
            new("Full (508+)", 4),
        };
    }

    public SearchSettings GetSearchSettings() => new()
    {
        Species = (ushort)Species,
        Nature = (Nature)Nature,
        Ability = Ability,
        Item = Item,
        Nickname = Nickname,
        SearchShiny = IsShiny,
        SearchLegal = IsLegal,
        SearchEgg = IsEgg,
        HiddenPowerType = HiddenPowerType,
        // Only filter by level when a comparison operand is selected; otherwise leave null.
        Level = (SearchComparison)LevelComparison == SearchComparison.None ? null : (byte)Level,
        SearchLevel = (SearchComparison)LevelComparison,
        IVType = IvType,
        EVType = EvType,
        // ESV is only meaningful alongside Egg=Yes (see SearchSettings.FilterResultEgg).
        ESV = EsvEnabled ? Esv : null,
        // Batch-editor instruction filter lines; Core parses and applies these in the predicate.
        BatchInstructions = BatchInstructions ?? string.Empty,
        Context = _sav.Context
    };

    /// <summary>Builds a predicate from the current filter inputs for in-place seeking.</summary>
    public Func<PKM, bool> CreateSearchPredicate() => GetSearchSettings().CreateSearchPredicate();

    public void RefreshLanguage() => InitDataSources();
}
