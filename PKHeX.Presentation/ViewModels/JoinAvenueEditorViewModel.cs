using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Application.Abstractions;
using PKHeX.Core;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Editor for the Join Avenue save block (<see cref="JoinAvenue5"/>) of <see cref="SAV5B2W2"/>,
/// reachable as <see cref="SAV5B2W2.JoinAvenue"/>. Edits write through the live save buffer that the block wraps.
/// </summary>
/// <remarks>
/// The block re-creates a fresh wrapper on each property access (and each nested entity slices the same
/// underlying buffer), so struct/field writes over those slices persist. The Settings sub-structure, the
/// visiting-player database, and the Self/Visitor/Fan/Occupant/Assistant entities are edited in place.
/// Per-entity single-file import/export uses the raw byte window each entity exposes via
/// <see cref="IJoinAvenueEntity5.Write"/> and the same-type <c>CopyFrom</c> path.
/// </remarks>
public partial class JoinAvenueEditorViewModel : ViewModelBase
{
    private readonly SAV5B2W2? _sav;
    private readonly JoinAvenue5? _block;
    private readonly ISpriteRenderer? _spriteRenderer;
    private readonly IDialogService? _dialogService;
    private readonly bool _loading;

    public bool IsSupported { get; }

    /// <summary>Human-readable description of the loaded game, or an unsupported notice.</summary>
    public string GameInfo { get; } = LocalizedStrings.Instance["JoinAvenue_NotLoaded"];

    /// <summary>Gate used by the host before constructing the editor.</summary>
    public static bool IsSupportedSave(SaveFile sav) => sav is SAV5B2W2;

    public JoinAvenueEditorViewModel(SAV5B2W2 sav) : this(sav, null, null) { }
    public JoinAvenueEditorViewModel(SAV5B2W2 sav, ISpriteRenderer? spriteRenderer) : this(sav, spriteRenderer, null) { }

    public JoinAvenueEditorViewModel(SAV5B2W2 sav, ISpriteRenderer? spriteRenderer, IDialogService? dialogService)
    {
        _loading = true;
        _sav = sav;
        _spriteRenderer = spriteRenderer;
        _dialogService = dialogService;
        _block = sav.JoinAvenue;
        IsSupported = true;
        GameInfo = LocalizedStrings.Instance.Format("JoinAvenue_GameInfo", sav.Version);

        var block = _block;
        var settings = block.Settings;

        // ---- Settings ----
        _avenueName = settings.Name;
        _playerTitle = settings.PlayerTitle;
        _experience = settings.Experience;
        _rank = settings.Rank;
        _ceilingColorIndex = (int)settings.CeilingColor;
        _flags = settings.Flags;
        _seed = settings.Seed;
        _scriptFlag = block.ScriptFlag;
        _visitingPlayerCount = settings.VisitingPlayerDatabaseCount;
        _visitingPlayerInsertIndex = settings.VistiingPlayerDatabaseInsertIndex;
        _promotionDaysElapsed = settings.PromotionDaysElapsed;
        _isPromotionActive = settings.IsPromotionActive;

        for (int i = 0; i < JoinAvenueSettings5.CountVisitingPlayersRemembered; i++)
            VisitingPlayers.Add(new JoinAvenueVisitingPlayerViewModel(this, i, settings.GetVisitingPlayerTrainerID(i)));

        // ---- Self (a Visitor entity that won't always be filled out) ----
        Self = new JoinAvenueVisitorEntryViewModel(this, "Self", () => _block!.Self, _spriteRenderer);

        // ---- Visitors / Occupants (both Visitor entities) ----
        for (int i = 0; i < JoinAvenue5.VisitorCount; i++)
        {
            int slot = i;
            Visitors.Add(new JoinAvenueVisitorEntryViewModel(this, $"Visitor {slot + 1}", () => _block!.GetVisitor(slot), _spriteRenderer));
        }
        _selectedVisitor = Visitors[0];

        for (int i = 0; i < JoinAvenue5.OccupantCount; i++)
        {
            int slot = i;
            Occupants.Add(new JoinAvenueVisitorEntryViewModel(this, $"Occupant {slot + 1}", () => _block!.GetOccupant(slot), _spriteRenderer));
        }
        _selectedOccupant = Occupants[0];

        // ---- Fans ----
        for (int i = 0; i < JoinAvenue5.FanCount; i++)
        {
            int slot = i;
            Fans.Add(new JoinAvenueFanEntryViewModel(this, $"Fan {slot + 1}", () => _block!.GetFan(slot), _spriteRenderer));
        }
        _selectedFan = Fans[0];

        // ---- Assistants ----
        for (int i = 0; i < JoinAvenue5.AssistantCount; i++)
        {
            int slot = i;
            Assistants.Add(new JoinAvenueAssistantEntryViewModel(this, $"Assistant {slot + 1}", () => _block!.GetAssistant(slot), _spriteRenderer));
        }
        _selectedAssistant = Assistants[0];

        _loading = false;
    }

    // ---------------- Settings ----------------

    [ObservableProperty] private string _avenueName = string.Empty;
    [ObservableProperty] private string _playerTitle = string.Empty;
    [ObservableProperty] private long _experience;
    [ObservableProperty] private int _rank;
    [ObservableProperty] private int _ceilingColorIndex;
    [ObservableProperty] private long _flags;
    [ObservableProperty] private long _seed;
    [ObservableProperty] private bool _scriptFlag;
    [ObservableProperty] private int _visitingPlayerCount;
    [ObservableProperty] private int _visitingPlayerInsertIndex;
    [ObservableProperty] private int _promotionDaysElapsed;
    [ObservableProperty] private bool _isPromotionActive;

    public int MaxRank => JoinAvenueSettings5.MaxAvenueRank;
    public int MaxVisitingPlayers => JoinAvenueSettings5.CountVisitingPlayersRemembered;
    public int MaxPromotionDays => JoinAvenueSettings5.PromotionDaysMax;

    public IReadOnlyList<ComboItem> CeilingColorList { get; } = BuildEnumList<JoinAvenueCeilingColor5>();

    partial void OnAvenueNameChanged(string value) => Settings(s => s.Name = value ?? string.Empty);
    partial void OnPlayerTitleChanged(string value) => Settings(s => s.PlayerTitle = value ?? string.Empty);
    partial void OnExperienceChanged(long value) => Settings(s => s.Experience = (uint)value);
    partial void OnRankChanged(int value) => Settings(s => s.Rank = (ushort)Math.Clamp(value, 0, JoinAvenueSettings5.MaxAvenueRank));
    partial void OnCeilingColorIndexChanged(int value) => Settings(s => s.CeilingColor = (JoinAvenueCeilingColor5)value);
    partial void OnFlagsChanged(long value) => Settings(s => s.Flags = (uint)value);
    partial void OnSeedChanged(long value) => Settings(s => s.Seed = (uint)value);
    partial void OnVisitingPlayerCountChanged(int value) => Settings(s => s.VisitingPlayerDatabaseCount = (ushort)Math.Max(0, value));
    partial void OnVisitingPlayerInsertIndexChanged(int value) => Settings(s => s.VistiingPlayerDatabaseInsertIndex = (ushort)Math.Max(0, value));
    partial void OnPromotionDaysElapsedChanged(int value) => Settings(s => s.PromotionDaysElapsed = (ushort)Math.Max(0, value));
    partial void OnIsPromotionActiveChanged(bool value) => Settings(s => s.IsPromotionActive = value);

    partial void OnScriptFlagChanged(bool value)
    {
        if (_loading || _block is null) return;
        _block.ScriptFlag = value;
        MarkEdited();
    }

    private void Settings(Action<JoinAvenueSettings5> write)
    {
        if (_loading || _block is null) return;
        write(_block.Settings);
        MarkEdited();
    }

    // ---------------- Visiting player database ----------------

    public ObservableCollection<JoinAvenueVisitingPlayerViewModel> VisitingPlayers { get; } = [];

    internal void SetVisitingPlayer(int index, uint value)
    {
        if (_loading || _block is null) return;
        _block.Settings.SetVisitingPlayerTrainerID(index, value);
        MarkEdited();
    }

    [RelayCommand]
    private void ClearVisitingPlayers()
    {
        if (_loading || _block is null) return;
        _block.Settings.ResetPlayerVisitList();
        MarkEdited();

        _suppressReload = true;
        foreach (var p in VisitingPlayers)
            p.ReloadFrom(JoinAvenueSettings5.VisitingPlayerDefaultNone);
        VisitingPlayerCount = 0;
        VisitingPlayerInsertIndex = 0;
        _suppressReload = false;
    }

    private bool _suppressReload;
    internal bool SuppressReload => _suppressReload;

    // ---------------- Self ----------------

    public JoinAvenueVisitorEntryViewModel Self { get; }

    // ---------------- Visitors / Occupants / Fans / Assistants ----------------

    public ObservableCollection<JoinAvenueVisitorEntryViewModel> Visitors { get; } = [];
    public ObservableCollection<JoinAvenueVisitorEntryViewModel> Occupants { get; } = [];
    public ObservableCollection<JoinAvenueFanEntryViewModel> Fans { get; } = [];
    public ObservableCollection<JoinAvenueAssistantEntryViewModel> Assistants { get; } = [];

    [ObservableProperty] private JoinAvenueVisitorEntryViewModel? _selectedVisitor;
    [ObservableProperty] private JoinAvenueVisitorEntryViewModel? _selectedOccupant;
    [ObservableProperty] private JoinAvenueFanEntryViewModel? _selectedFan;
    [ObservableProperty] private JoinAvenueAssistantEntryViewModel? _selectedAssistant;

    public uint CountVisitor => _block?.CountVisitor ?? 0;
    public uint CountFan => _block?.CountFan ?? 0;

    // ---------------- Import / export plumbing ----------------

    internal IDialogService? DialogService => _dialogService;

    internal async Task<byte[]?> PromptImportAsync(string title, string extension, int expectedLength)
    {
        if (_dialogService is null) return null;
        var path = await _dialogService.OpenFileAsync(title, [extension]);
        if (string.IsNullOrEmpty(path)) return null;
        try
        {
            var data = await File.ReadAllBytesAsync(path);
            if (data.Length != expectedLength)
            {
                await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["JoinAvenue_ImportErrorTitle"],
                    LocalizedStrings.Instance.Format("JoinAvenue_SizeMismatch", expectedLength, data.Length));
                return null;
            }
            return data;
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["JoinAvenue_ImportErrorTitle"], ex.Message);
            return null;
        }
    }

    internal async Task ExportBytesAsync(string title, string extension, ReadOnlyMemory<byte> data)
    {
        if (_dialogService is null) return;
        var defaultName = $"JoinAvenue.{extension}";
        var path = await _dialogService.SaveFileAsync(title, defaultName, [extension]);
        if (string.IsNullOrEmpty(path)) return;
        try
        {
            await File.WriteAllBytesAsync(path, data.ToArray());
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["JoinAvenue_ExportErrorTitle"], ex.Message);
        }
    }

    // ---------------- Plumbing ----------------

    internal SAV5B2W2? Save => _sav;
    internal bool IsLoading => _loading;

    internal void MarkEdited()
    {
        if (!_loading && _sav is not null)
            _sav.State.Edited = true;
    }

    internal static IReadOnlyList<ComboItem> BuildEnumList<T>() where T : struct, Enum
    {
        var names = Enum.GetNames<T>();
        var values = Enum.GetValues<T>();
        var list = new List<ComboItem>(names.Length);
        for (int i = 0; i < names.Length; i++)
            list.Add(new ComboItem(names[i], Convert.ToInt32(values[i])));
        return list;
    }
}

/// <summary>One remembered visiting-player trainer ID (0xFFFFFFFF = empty), routed back to the owner on edit.</summary>
public partial class JoinAvenueVisitingPlayerViewModel : ViewModelBase
{
    private readonly JoinAvenueEditorViewModel _parent;
    private readonly int _index;
    private bool _suppress;

    public JoinAvenueVisitingPlayerViewModel(JoinAvenueEditorViewModel parent, int index, uint value)
    {
        _parent = parent;
        _index = index;
        _value = (long)value;
    }

    public int Slot => _index + 1;

    [ObservableProperty]
    private long _value;

    partial void OnValueChanged(long value)
    {
        if (_suppress) return;
        _parent.SetVisitingPlayer(_index, (uint)value);
    }

    internal void ReloadFrom(uint value)
    {
        _suppress = true;
        Value = (long)value;
        _suppress = false;
    }
}

/// <summary>
/// A reusable species + sprite sub-VM wrapping a single 16-bit species id (Gen 5 favorites / fan species).
/// Exposes a Species combo and a live sprite preview via the renderer. The caller supplies a write-back
/// delegate invoked whenever the species changes.
/// </summary>
public partial class JoinAvenueSpeciesViewModel : ViewModelBase
{
    private readonly ISpriteRenderer? _spriteRenderer;
    private readonly Action<ushort>? _onChanged;
    private bool _suppress;

    public const int MaxSpeciesGen5 = 649; // Legal.MaxSpeciesID_5 (Genesect); the Core constant is internal.

    public static IReadOnlyList<ComboItem> SpeciesSource { get; } = BuildSpeciesSource();

    public JoinAvenueSpeciesViewModel(ushort species, ISpriteRenderer? spriteRenderer, Action<ushort>? onChanged)
    {
        _spriteRenderer = spriteRenderer;
        _onChanged = onChanged;
        _species = species;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Sprite))]
    private int _species;

    public byte[]? Sprite => _spriteRenderer?.GetSprite((ushort)Species, 0, 0, 0, false, EntityContext.Gen5);

    partial void OnSpeciesChanged(int value)
    {
        if (_suppress) return;
        _onChanged?.Invoke((ushort)value);
    }

    internal void Load(ushort species)
    {
        _suppress = true;
        Species = species;
        _suppress = false;
        OnPropertyChanged(nameof(Sprite));
    }

    private static IReadOnlyList<ComboItem> BuildSpeciesSource()
    {
        var names = Core.GameInfo.Strings.Species;
        var list = new List<ComboItem>(MaxSpeciesGen5 + 1);
        for (int i = 0; i <= MaxSpeciesGen5 && i < names.Count; i++)
            list.Add(new ComboItem(string.IsNullOrEmpty(names[i]) ? $"#{i}" : names[i], i));
        return list;
    }
}

/// <summary>
/// Base view-model for the shared <see cref="IJoinAvenueEntity5"/> surface (name/country/shout/version/
/// language/gender/TID/played-time/greeting-farewell/met-date/seed). Concrete subclasses add the
/// type-specific fields and supply the entity accessor + import/export wiring.
/// </summary>
public abstract partial class JoinAvenueEntityViewModel : ViewModelBase
{
    protected readonly JoinAvenueEditorViewModel Parent;

    protected JoinAvenueEntityViewModel(JoinAvenueEditorViewModel parent, string label)
    {
        Parent = parent;
        Label = label;
    }

    public string Label { get; }

    public IReadOnlyList<ComboItem> GenderList { get; } =
    [
        new ComboItem("Male", 0),
        new ComboItem("Female", 1),
        new ComboItem("Genderless", 2),
    ];

    public abstract IJoinAvenueEntity5 Entity { get; }

    // ---- Shared entity fields ----
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private int _country;
    [ObservableProperty] private int _subregion;
    [ObservableProperty] private string _shout = string.Empty;
    [ObservableProperty] private int _version;
    [ObservableProperty] private int _language;
    [ObservableProperty] private int _gender;
    [ObservableProperty] private int _tid16;
    [ObservableProperty] private int _playedHours;
    [ObservableProperty] private int _playedMinutes;
    [ObservableProperty] private int _spriteId;
    [ObservableProperty] private string _greeting = string.Empty;
    [ObservableProperty] private string _farewell = string.Empty;
    [ObservableProperty] private int _metYear;
    [ObservableProperty] private int _metMonth;
    [ObservableProperty] private int _metDay;
    [ObservableProperty] private bool _isInteractedToday;
    [ObservableProperty] private long _seed;

    // Loads run with write-back suppressed (Suppress=true during Reload; Parent.IsLoading=true during ctor),
    // so assigning the generated properties here never round-trips back into the live entity.
    protected void LoadCommon()
    {
        var e = Entity;
        Name = e.Name;
        Country = e.Country;
        Subregion = e.Subregion;
        Shout = e.Shout;
        Version = e.Version;
        Language = e.Language;
        Gender = e.Gender;
        Tid16 = e.TID16;
        PlayedHours = e.PlayedHours;
        PlayedMinutes = e.PlayedMinutes;
        SpriteId = e.Sprite;
        Greeting = e.Greeting;
        Farewell = e.Farewell;
        MetYear = e.MetYear;
        MetMonth = e.MetMonth;
        MetDay = e.MetDay;
        IsInteractedToday = e.IsInteractedToday;
        Seed = e.Seed;
    }

    partial void OnNameChanged(string value) => Write(e => e.Name = value ?? string.Empty);
    partial void OnCountryChanged(int value) => Write(e => e.Country = (byte)value);
    partial void OnSubregionChanged(int value) => Write(e => e.Subregion = (byte)value);
    partial void OnShoutChanged(string value) => Write(e => e.Shout = value ?? string.Empty);
    partial void OnVersionChanged(int value) => Write(e => e.Version = (byte)value);
    partial void OnLanguageChanged(int value) => Write(e => e.Language = (byte)value);
    partial void OnGenderChanged(int value) => Write(e => e.Gender = (byte)value);
    partial void OnTid16Changed(int value) => Write(e => e.TID16 = (ushort)value);
    partial void OnPlayedHoursChanged(int value) => Write(e => e.PlayedHours = (ushort)value);
    partial void OnPlayedMinutesChanged(int value) => Write(e => e.PlayedMinutes = (byte)value);
    partial void OnSpriteIdChanged(int value) => Write(e => e.Sprite = (ushort)value);
    partial void OnGreetingChanged(string value) => Write(e => e.Greeting = value ?? string.Empty);
    partial void OnFarewellChanged(string value) => Write(e => e.Farewell = value ?? string.Empty);
    partial void OnMetYearChanged(int value) => Write(e => e.MetYear = (byte)value);
    partial void OnMetMonthChanged(int value) => Write(e => e.MetMonth = (byte)value);
    partial void OnMetDayChanged(int value) => Write(e => e.MetDay = (byte)value);
    partial void OnIsInteractedTodayChanged(bool value) => Write(e => e.IsInteractedToday = value);
    partial void OnSeedChanged(long value) => Write(e => e.Seed = (uint)value);

    protected bool Suppress;

    protected void Write(Action<IJoinAvenueEntity5> apply)
    {
        if (Suppress || Parent.IsLoading) return;
        apply(Entity);
        Parent.MarkEdited();
    }

    /// <summary>Re-read every surfaced field from the live entity without writing back (used after import).</summary>
    public void Reload()
    {
        Suppress = true;
        LoadCommon();
        LoadSpecific();
        OnPropertyChanged(string.Empty); // refresh all bindings
        Suppress = false;
    }

    protected abstract void LoadSpecific();
}

/// <summary>A Visitor (or Occupant, or Self) entity: the shared surface plus visitor-specific fields.</summary>
public partial class JoinAvenueVisitorEntryViewModel : JoinAvenueEntityViewModel
{
    private readonly Func<JoinAvenueVisitor5> _accessor;
    private readonly ISpriteRenderer? _spriteRenderer;

    public JoinAvenueVisitorEntryViewModel(JoinAvenueEditorViewModel parent, string label, Func<JoinAvenueVisitor5> accessor, ISpriteRenderer? spriteRenderer)
        : base(parent, label)
    {
        _accessor = accessor;
        _spriteRenderer = spriteRenderer;

        LoadCommon();
        LoadSpecificFields();

        var current = _accessor();
        Favorite = new JoinAvenueSpeciesViewModel(current.FavoriteSpecies, spriteRenderer, sp => WriteVisitor(v => v.FavoriteSpecies = sp));
    }

    public JoinAvenueVisitor5 Visitor => _accessor();
    public override IJoinAvenueEntity5 Entity => _accessor();

    public IReadOnlyList<ComboItem> ShopTypeList { get; } = JoinAvenueEditorViewModel.BuildEnumList<JoinAvenueShopType5>();

    public JoinAvenueSpeciesViewModel? Favorite { get; private set; }

    // ---- Visitor-specific fields ----
    [ObservableProperty] private int _joinAvenueLevel;
    [ObservableProperty] private int _shopTypeIndex;
    [ObservableProperty] private int _desiredShopType;
    [ObservableProperty] private int _dexSeen;
    [ObservableProperty] private int _medalRank;
    [ObservableProperty] private int _medalHint;
    [ObservableProperty] private int _medalCount;
    [ObservableProperty] private int _shopLevel;
    [ObservableProperty] private int _shopExperience;
    [ObservableProperty] private int _joinAvenueRank;
    [ObservableProperty] private int _origin;

    public int ShopMaxLevel => JoinAvenueVisitor5.ShopMaxRank;

    protected override void LoadSpecific()
    {
        LoadSpecificFields();
        Favorite?.Load(_accessor().FavoriteSpecies);
    }

    private void LoadSpecificFields()
    {
        var v = _accessor();
        JoinAvenueLevel = v.JoinAvenueLevel;
        ShopTypeIndex = v.ShopTypeTuple?.Type is { } type ? (int)type : 0;
        DesiredShopType = v.DesiredShopType;
        DexSeen = v.DexSeen;
        MedalRank = v.MedalRank;
        MedalHint = v.MedalHint;
        MedalCount = v.MedalCount;
        ShopLevel = v.ShopRank;
        ShopExperience = v.ShopExperience;
        JoinAvenueRank = v.JoinAvenueRank;
        Origin = v.Origin;
    }

    partial void OnJoinAvenueLevelChanged(int value) => WriteVisitor(v => v.JoinAvenueLevel = (byte)value);
    partial void OnShopTypeIndexChanged(int value) => WriteVisitor(v =>
    {
        var current = v.ShopTypeTuple;
        v.ShopTypeTuple = (current?.Version ?? 0, (JoinAvenueShopType5)Math.Clamp(value, 0, JoinAvenueVisitor5.ShopTypeCount - 1), current?.Rank ?? 0);
    });
    partial void OnDesiredShopTypeChanged(int value) => WriteVisitor(v => v.DesiredShopType = (ushort)value);
    partial void OnDexSeenChanged(int value) => WriteVisitor(v => v.DexSeen = (ushort)value);
    partial void OnMedalRankChanged(int value) => WriteVisitor(v => v.MedalRank = (byte)value);
    partial void OnMedalHintChanged(int value) => WriteVisitor(v => v.MedalHint = (byte)value);
    partial void OnMedalCountChanged(int value) => WriteVisitor(v => v.MedalCount = (byte)value);
    partial void OnShopLevelChanged(int value) => WriteVisitor(v => v.ShopRank = (byte)value);
    partial void OnShopExperienceChanged(int value) => WriteVisitor(v => v.ShopExperience = (ushort)value);
    partial void OnJoinAvenueRankChanged(int value) => WriteVisitor(v => v.JoinAvenueRank = (byte)value);
    partial void OnOriginChanged(int value) => WriteVisitor(v => v.Origin = (ushort)value);

    private void WriteVisitor(Action<JoinAvenueVisitor5> apply)
    {
        if (Suppress || Parent.IsLoading) return;
        apply(_accessor());
        Parent.MarkEdited();
    }

    [RelayCommand]
    private async Task ImportAsync()
    {
        var data = await Parent.PromptImportAsync($"Import {Label}", "jav5", JoinAvenueVisitor5.SIZE);
        if (data is null) return;
        var temp = new JoinAvenueVisitor5(data);
        _accessor().CopyFrom(temp);
        Parent.MarkEdited();
        Reload();
    }

    [RelayCommand]
    private async Task ExportAsync()
    {
        await Parent.ExportBytesAsync($"Export {Label}", "jav5", _accessor().Write().ToArray());
    }
}

/// <summary>A Fan entity: the shared surface plus fan-specific fields (species, bubble target, etc.).</summary>
public partial class JoinAvenueFanEntryViewModel : JoinAvenueEntityViewModel
{
    private readonly Func<JoinAvenueFan5> _accessor;
    private readonly ISpriteRenderer? _spriteRenderer;

    public JoinAvenueFanEntryViewModel(JoinAvenueEditorViewModel parent, string label, Func<JoinAvenueFan5> accessor, ISpriteRenderer? spriteRenderer)
        : base(parent, label)
    {
        _accessor = accessor;
        _spriteRenderer = spriteRenderer;

        LoadCommon();
        LoadSpecificFields();

        var current = _accessor();
        FanSpecies = new JoinAvenueSpeciesViewModel(current.Species, spriteRenderer, sp => WriteFan(f => f.Species = sp));
    }

    public JoinAvenueFan5 Fan => _accessor();
    public override IJoinAvenueEntity5 Entity => _accessor();

    public JoinAvenueSpeciesViewModel? FanSpecies { get; private set; }

    [ObservableProperty] private int _bubbleTarget;

    protected override void LoadSpecific()
    {
        LoadSpecificFields();
        FanSpecies?.Load(_accessor().Species);
    }

    private void LoadSpecificFields()
    {
        var f = _accessor();
        BubbleTarget = f.BubbleTarget;
    }

    partial void OnBubbleTargetChanged(int value) => WriteFan(f => f.BubbleTarget = (byte)value);

    private void WriteFan(Action<JoinAvenueFan5> apply)
    {
        if (Suppress || Parent.IsLoading) return;
        apply(_accessor());
        Parent.MarkEdited();
    }

    [RelayCommand]
    private async Task ImportAsync()
    {
        var data = await Parent.PromptImportAsync($"Import {Label}", "jah5", JoinAvenueFan5.SIZE);
        if (data is null) return;
        var temp = new JoinAvenueFan5(data);
        _accessor().CopyFrom(temp);
        Parent.MarkEdited();
        Reload();
    }

    [RelayCommand]
    private async Task ExportAsync()
    {
        await Parent.ExportBytesAsync($"Export {Label}", "jah5", _accessor().Write().ToArray());
    }
}

/// <summary>An Assistant entity: the shared surface plus assistant-specific position fields.</summary>
public partial class JoinAvenueAssistantEntryViewModel : JoinAvenueEntityViewModel
{
    private readonly Func<JoinAvenueAssistant5> _accessor;

    public JoinAvenueAssistantEntryViewModel(JoinAvenueEditorViewModel parent, string label, Func<JoinAvenueAssistant5> accessor, ISpriteRenderer? spriteRenderer)
        : base(parent, label)
    {
        _ = spriteRenderer;
        _accessor = accessor;

        LoadCommon();
        LoadSpecificFields();
    }

    public JoinAvenueAssistant5 Assistant => _accessor();
    public override IJoinAvenueEntity5 Entity => _accessor();

    [ObservableProperty] private int _position0;
    [ObservableProperty] private int _position1;
    [ObservableProperty] private int _position2;

    protected override void LoadSpecific() => LoadSpecificFields();

    private void LoadSpecificFields()
    {
        var a = _accessor();
        Position0 = a.Position0;
        Position1 = a.Position1;
        Position2 = a.Position2;
    }

    partial void OnPosition0Changed(int value) => WriteAssistant(a => a.Position0 = (byte)value);
    partial void OnPosition1Changed(int value) => WriteAssistant(a => a.Position1 = (byte)value);
    partial void OnPosition2Changed(int value) => WriteAssistant(a => a.Position2 = (byte)value);

    private void WriteAssistant(Action<JoinAvenueAssistant5> apply)
    {
        if (Suppress || Parent.IsLoading) return;
        apply(_accessor());
        Parent.MarkEdited();
    }

    [RelayCommand]
    private async Task ImportAsync()
    {
        var data = await Parent.PromptImportAsync($"Import {Label}", "jaa5", JoinAvenueAssistant5.SIZE);
        if (data is null) return;
        var temp = new JoinAvenueAssistant5(data);
        _accessor().CopyFrom(temp);
        Parent.MarkEdited();
        Reload();
    }

    [RelayCommand]
    private async Task ExportAsync()
    {
        await Parent.ExportBytesAsync($"Export {Label}", "jaa5", _accessor().Write().ToArray());
    }
}
