
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class PokemonEditorViewModel
{
    // Contest Stats (Gen 3-6)
    [ObservableProperty]
    private int _contestCool;

    [ObservableProperty]
    private int _contestBeauty;

    [ObservableProperty]
    private int _contestCute;

    [ObservableProperty]
    private int _contestSmart;

    [ObservableProperty]
    private int _contestTough;

    [ObservableProperty]
    private int _contestSheen;

    public bool HasContestStats => _pk is IContestStatsReadOnly;

    // Markings (all generations support some subset)
    [ObservableProperty]
    private bool _markingCircle;

    [ObservableProperty]
    private bool _markingTriangle;

    [ObservableProperty]
    private bool _markingSquare;

    [ObservableProperty]
    private bool _markingHeart;

    [ObservableProperty]
    private bool _markingStar;

    [ObservableProperty]
    private bool _markingDiamond;

    public bool HasMarkings => _pk is IAppliedMarkings;
    public bool HasSixMarkings => _pk is IAppliedMarkings4 or IAppliedMarkings7;

    // Memories (Gen 6+)
    [ObservableProperty]
    private int _otMemory;

    [ObservableProperty]
    private int _otMemoryIntensity;

    [ObservableProperty]
    private int _otMemoryFeeling;

    [ObservableProperty]
    private int _otMemoryVariable;

    [ObservableProperty]
    private int _htMemory;

    [ObservableProperty]
    private int _htMemoryIntensity;

    [ObservableProperty]
    private int _htMemoryFeeling;

    [ObservableProperty]
    private int _htMemoryVariable;

    public bool HasMemories => _pk is IMemoryOT;

    // Ribbons
    [ObservableProperty]
    private ObservableCollection<RibbonItemViewModel> _ribbons = [];
    
    public bool HasRibbons => Ribbons.Count > 0;
    public int RibbonCount => Ribbons.Count(r => r.IsBooleanRibbon ? r.HasRibbon : r.RibbonCount > 0);
    
    private void LoadRibbons()
    {
        var ribbonInfos = RibbonInfo.GetRibbonInfo(_pk);
        Ribbons = new ObservableCollection<RibbonItemViewModel>(
            ribbonInfos.Select(r => new RibbonItemViewModel(_pk, r))
        );
        OnPropertyChanged(nameof(HasRibbons));
        OnPropertyChanged(nameof(RibbonCount));
    }

    [RelayCommand]
    private void SetAllRibbons()
    {
        foreach (var ribbon in Ribbons)
        {
            if (ribbon.IsBooleanRibbon)
                ribbon.HasRibbon = true;
            else
                ribbon.RibbonCount = ribbon.MaxCount;
        }
        OnPropertyChanged(nameof(RibbonCount));
    }

    [RelayCommand]
    private void ClearRibbons()
    {
        foreach (var ribbon in Ribbons)
        {
            if (ribbon.IsBooleanRibbon)
                ribbon.HasRibbon = false;
            else
                ribbon.RibbonCount = 0;
        }
        OnPropertyChanged(nameof(RibbonCount));
    }

    // PID/EC
    [ObservableProperty]
    private string _pid = string.Empty;

    [ObservableProperty]
    private string _encryptionConstant = string.Empty;

    // EXP & Friendship
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Level), nameof(ExpPercent))]
    private long _exp;

    [ObservableProperty]
    private int _happiness;

    /// <summary>
    /// Progress through the current level as a fraction in [0,1], used to fill the Experience bar.
    /// At <see cref="Experience.MaxLevel"/> the bar is full.
    /// </summary>
    public double ExpPercent => Level >= Experience.MaxLevel
        ? 1.0
        : Experience.GetEXPToLevelUpPercentage((byte)Level, (uint)Exp, _pk.PersonalInfo.EXPGrowth);

    partial void OnExpChanged(long value)
    {
        if (_isLoading) return;
        var newLevel = Experience.GetLevel((uint)value, _pk.PersonalInfo.EXPGrowth);
        if (Level != newLevel)
            Level = newLevel;
    }

    /// <summary>
    /// Sets <see cref="Exp"/> from a horizontal fraction [0,1) along the Experience bar, staying within the
    /// current level (mirrors upstream's left-click/drag behavior, which never changes the level).
    /// </summary>
    /// <param name="fraction">Fractional position along the bar; clamped to [0,1).</param>
    public void SetExpFromFraction(double fraction)
    {
        if (Level >= Experience.MaxLevel)
            return;

        fraction = Math.Clamp(fraction, 0.0, 1.0);
        var growth = _pk.PersonalInfo.EXPGrowth;
        var start = Experience.GetEXP((byte)Level, growth);
        var range = Experience.GetEXPToLevelUp((byte)Level, growth);
        if (range == 0)
            return;

        // Stay strictly below the next level's threshold (range-1 is the level's high edge).
        var progress = Math.Min(range - 1, (uint)(range * fraction));
        Exp = start + progress;
    }

    /// <summary>
    /// Snaps <see cref="Exp"/> to the high edge of the current level (one EXP below the next level-up),
    /// mirroring upstream's modifier-click behavior.
    /// </summary>
    public void SetExpToLevelEdgeHigh()
    {
        if (Level >= Experience.MaxLevel)
            return;

        var growth = _pk.PersonalInfo.EXPGrowth;
        var range = Experience.GetEXPToLevelUp((byte)Level, growth);
        if (range == 0)
            return;
        Exp = Experience.GetEXP((byte)Level, growth) + range - 1;
    }

    // Pokerus
    [ObservableProperty]
    private int _pkrsStrain;

    [ObservableProperty]
    private int _pkrsDays;
    
    [ObservableProperty]
    private bool _isPokerusInfected;

    [ObservableProperty]
    private bool _isPokerusCured;

    // Equality guards skip the load-time assignments (the values are read from _pk) while still writing live edits through.
    partial void OnIsPokerusInfectedChanged(bool value)
    {
        if (value == _pk.IsPokerusInfected)
            return;
        _pk.IsPokerusInfected = value;
        if (value)
        {
            if (_pk.PokerusDays == 0)
                _pk.PokerusDays = 1;
        }
        else
        {
            _pk.PokerusDays = 0;
            IsPokerusCured = false;
        }
        PkrsStrain = _pk.PokerusStrain;
        PkrsDays = _pk.PokerusDays;
    }

    partial void OnIsPokerusCuredChanged(bool value)
    {
        if (value == _pk.IsPokerusCured)
            return;
        _pk.IsPokerusCured = value;
        IsPokerusInfected = true;
        PkrsStrain = _pk.PokerusStrain;
        PkrsDays = _pk.PokerusDays;
    }

    // OT Info
    [ObservableProperty]
    private string _originalTrainerName = string.Empty;

    [ObservableProperty]
    private long _trainerID;

    [ObservableProperty]
    private int _originalTrainerGender;

    [ObservableProperty]
    private int _originalTrainerFriendship;

    [ObservableProperty]
    private string _handlingTrainerName = string.Empty;

    [ObservableProperty]
    private int _handlingTrainerGender;

    [ObservableProperty]
    private int _handlingTrainerFriendship;

    [ObservableProperty]
    private int _currentHandler;
    
    [ObservableProperty]
    private int _abilityNumber;

    // Form Argument (species/form-dependent extra value, e.g. Alcremie topping, Furfrou trim days)
    private FormArgumentType _formArgumentType = FormArgumentType.None;

    [ObservableProperty]
    private int _formArgumentValue;

    [ObservableProperty]
    private int _formArgumentMax;

    [ObservableProperty]
    private IReadOnlyList<string> _formArgumentList = [];

    /// <summary>Whether the current entity/species/form uses a Form Argument at all.</summary>
    public bool ShowFormArgument => _pk is IFormArgument && _formArgumentType != FormArgumentType.None;

    /// <summary>The Form Argument is a named selection (e.g. Alcremie topping).</summary>
    public bool ShowFormArgumentNamed => ShowFormArgument && _formArgumentType == FormArgumentType.Named;

    /// <summary>The Form Argument is a raw numeric value (everything editable that isn't Named).</summary>
    public bool ShowFormArgumentRaw => ShowFormArgument && _formArgumentType != FormArgumentType.Named;

    /// <summary>Re-evaluates the Form Argument type/range for the current species/form and reloads its value.</summary>
    private void UpdateFormArgument()
    {
        if (_pk is not IFormArgument fa)
        {
            _formArgumentType = FormArgumentType.None;
            FormArgumentList = [];
            FormArgumentMax = 0;
            NotifyFormArgumentVisibility();
            return;
        }

        var species = (ushort)Species;
        var form = (byte)Form;
        _formArgumentType = FormArgumentUtil.GetType(species, form, _pk.Context);
        FormArgumentList = _formArgumentType == FormArgumentType.Named
            ? FormConverter.GetFormArgumentStrings(species)
            : [];
        FormArgumentMax = (int)FormArgumentUtil.GetFormArgumentMaxEdge(species, form, _pk.Context);

        // Triple types (Furfrou/Hoopa) pack remain/elapsed/max into FormArgument; surface the "remain" value.
        var current = FormArgumentUtil.IsFormArgumentTypeDateTriple(species, form)
            ? fa.FormArgumentRemain
            : fa.FormArgument;

        var wasLoading = _isLoading;
        _isLoading = true;
        FormArgumentValue = (int)Math.Min(current, (uint)FormArgumentMax);
        _isLoading = wasLoading;

        NotifyFormArgumentVisibility();
    }

    private void NotifyFormArgumentVisibility()
    {
        OnPropertyChanged(nameof(ShowFormArgument));
        OnPropertyChanged(nameof(ShowFormArgumentNamed));
        OnPropertyChanged(nameof(ShowFormArgumentRaw));
    }

    partial void OnFormArgumentValueChanged(int value)
    {
        if (_isLoading) return;
        if (_pk is not IFormArgument)
            return;
        _pk.ChangeFormArgument((uint)value);
        Validate();
    }

    /// <summary>Trainer Shiny Value derived from the current TID/SID (format-aware via Core).</summary>
    public int Tsv => (int)_pk.TSV;

    partial void OnOriginalTrainerGenderChanged(int value) { if (!_isLoading) Validate(); }

    partial void OnTrainerIDChanged(long value)
    {
        if (_isLoading) return;
        _pk.DisplayTID = (uint)value;
        OnPropertyChanged(nameof(Tsv));
        Validate();
    }

    partial void OnSidChanged(int value)
    {
        if (_isLoading) return;
        _pk.DisplaySID = (uint)value;
        OnPropertyChanged(nameof(Tsv));
        Validate();
    }
}
