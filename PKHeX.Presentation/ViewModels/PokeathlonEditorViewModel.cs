using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Editor for the Pokéathlon save block (<see cref="Pokeathlon4"/>) of <see cref="SAV4HGSS"/>,
/// reachable as <see cref="SAV4HGSS.Pokeathlon"/>. Edits write through the live save buffer the block wraps.
/// </summary>
/// <remarks>
/// The block re-creates a fresh <see cref="Pokeathlon4"/> wrapper on each property access, but every wrapper
/// (and every nested struct it hands out) slices the same underlying <see cref="SAV4HGSS"/> buffer, so writes
/// persist. Where a sub-structure exposes <c>Set*</c> methods or struct field writes over that slice we use them
/// directly; the only piece surfaced read-only is the per-course Athlete-Point sub-scores' breakdown (still
/// editable as four numeric scores), which has no impractical members.
/// </remarks>
public partial class PokeathlonEditorViewModel : ViewModelBase
{
    private readonly SAV4HGSS _sav;
    private readonly bool _loading;

    public bool IsSupported { get; }

    /// <summary>Human-readable description of the loaded game.</summary>
    public string GameInfo { get; } = "No Generation 4 HGSS save loaded.";

    /// <summary>Gate used by the host before constructing the editor.</summary>
    public static bool IsSupportedSave(SaveFile sav) => sav is SAV4HGSS;

    private Pokeathlon4 Block => _sav.Pokeathlon;

    public PokeathlonEditorViewModel(SAV4HGSS sav, ISpriteRenderer? spriteRenderer = null)
    {
        _loading = true;
        _sav = sav;
        IsSupported = true;
        GameInfo = $"{sav.Version} — Pokéathlon Dome (HGSS)";

        var block = Block;

        // ---- General ----
        _points = block.Points;
        var dailyFlags = block.FlagsDailyShop;
        for (int i = 0; i < DailyShopBits; i++)
            DailyShop.Add(new PokeathlonFlagViewModel(this, i, $"Shop Slot {i + 1}", ((dailyFlags >> i) & 1) != 0, SetDailyShopBit));

        // Data Card flags are named after the Key Items (item ids 505..531 = Data Card 01..27).
        var itemNames = Core.GameInfo.Strings.GetItemStrings(EntityContext.Gen4);
        var dataCardFlags = block.FlagsDataCard;
        for (int i = 0; i < (int)DataCard4.Count; i++)
        {
            int itemId = 505 + i;
            var name = itemId < itemNames.Length && !string.IsNullOrEmpty(itemNames[itemId]) ? itemNames[itemId] : $"Data Card {i + 1:00}";
            DataCards.Add(new PokeathlonFlagViewModel(this, i, $"[{i}] {name}", ((dataCardFlags >> i) & 1) != 0, SetDataCardBit));
        }

        // ---- Global counters ----
        var counters = block.GlobalCounters;
        _timeSpent = counters.TimeSpent;
        _sessionsJoined = counters.SessionsJoined;
        _placedFirst = counters.PlacedFirst;
        _placedLast = counters.PlacedLast;
        _bonusesEarned = counters.BonusesEarned;
        _instructions = counters.Instructions;
        _failed = counters.Failed;
        _jumped = counters.Jumped;
        _acquired = counters.Acquired;
        _tackled = counters.Tackled;
        _fellDown = counters.FellDown;
        _dashed = counters.Dashed;
        _switched = counters.Switched;
        _selfImpeded = counters.SelfImpeded;
        _connectionJoined = counters.ConnectionJoined;
        _connectionFirst = counters.ConnectionFirst;
        _connectionLast = counters.ConnectionLast;
        _totalEventLast = counters.TotalEventLast;
        _fame = counters.Fame;

        // Per-event first-place counters + best scores (10 events).
        for (int i = 0; i < (int)PokeathlonEvent4.Count; i++)
        {
            var ev = (PokeathlonEvent4)i;
            EventStats.Add(new PokeathlonEventStatViewModel(this, ev, GetEventName(ev), counters[ev], block.GetBestScore(ev)));
        }

        // ---- Medals (per-species, 5 bits each) ----
        var medals = block.Medals;
        for (ushort species = 1; species <= MaxSpeciesGen4; species++)
            Medals.Add(new PokeathlonMedalViewModel(this, species, medals.GetMedal(species), spriteRenderer));

        // ---- Courses (5 stats: Speed/Power/Skill/Stamina/Jump) ----
        for (int i = 0; i < (int)PokeathlonStat4.Count; i++)
        {
            var stat = (PokeathlonStat4)i;
            Courses.Add(new PokeathlonCourseViewModel(this, stat, GetCourseName(stat), spriteRenderer));
        }
        _selectedCourse = Courses[0];

        // ---- Self event data (10 events: attempts + 5 records) ----
        for (int i = 0; i < (int)PokeathlonEvent4.Count; i++)
        {
            var ev = (PokeathlonEvent4)i;
            SelfEvents.Add(new PokeathlonEventDataViewModel(this, ev, GetEventName(ev), spriteRenderer));
        }
        _selectedSelfEvent = SelfEvents[0];

        // ---- Connection event data (10 events: inner attempts + 5 records + 5 trainers) ----
        for (int i = 0; i < (int)PokeathlonEvent4.Count; i++)
        {
            var ev = (PokeathlonEvent4)i;
            Connections.Add(new PokeathlonConnectionViewModel(this, ev, GetEventName(ev), spriteRenderer));
        }
        _selectedConnection = Connections[0];

        _loading = false;
    }

    public const int MaxSpeciesGen4 = 493;
    public const int DailyShopBits = 12;

    internal static string GetEventName(PokeathlonEvent4 ev) => $"{(int)ev + 1} - {AddSpaces(ev.ToString())}";
    internal static string GetCourseName(PokeathlonStat4 stat) => $"{(int)stat + 1} - {stat}";

    private static string AddSpaces(string value)
    {
        // PennantCapture -> Pennant Capture
        Span<char> buffer = stackalloc char[value.Length * 2];
        int len = 0;
        for (int i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (i > 0 && char.IsUpper(c))
                buffer[len++] = ' ';
            buffer[len++] = c;
        }
        return new string(buffer[..len]);
    }

    // ---------------- General ----------------

    [ObservableProperty]
    private long _points;

    partial void OnPointsChanged(long value) => Apply(b => b.Points = (uint)Math.Clamp(value, 0, Pokeathlon4.MaxPoints));

    public uint MaxPoints => Pokeathlon4.MaxPoints;

    public ObservableCollection<PokeathlonFlagViewModel> DailyShop { get; } = [];
    public ObservableCollection<PokeathlonFlagViewModel> DataCards { get; } = [];

    private void SetDailyShopBit(int index, bool on)
    {
        if (_loading) return;
        var current = Block.FlagsDailyShop;
        if (on) current |= (ushort)(1 << index);
        else current &= (ushort)~(1 << index);
        Apply(b => b.FlagsDailyShop = current);
    }

    private void SetDataCardBit(int index, bool on)
    {
        if (_loading) return;
        var current = Block.FlagsDataCard;
        if (on) current |= 1u << index;
        else current &= ~(1u << index);
        Apply(b => b.FlagsDataCard = current);
    }

    [RelayCommand]
    private void GiveAllDataCards()
    {
        Apply(b => b.FlagsDataCard = Pokeathlon4.DataCardAllObtained);
        ReloadFlags(DataCards, Block.FlagsDataCard);
    }

    [RelayCommand]
    private void ClearAllDataCards()
    {
        Apply(b => b.FlagsDataCard = 0);
        ReloadFlags(DataCards, 0);
    }

    private void ReloadFlags(ObservableCollection<PokeathlonFlagViewModel> flags, uint bits)
    {
        for (int i = 0; i < flags.Count; i++)
            flags[i].SetCheckedQuiet(((bits >> i) & 1) != 0);
    }

    // ---------------- Global counters ----------------

    [ObservableProperty] private long _timeSpent;
    [ObservableProperty] private long _sessionsJoined;
    [ObservableProperty] private long _placedFirst;
    [ObservableProperty] private long _placedLast;
    [ObservableProperty] private long _bonusesEarned;
    [ObservableProperty] private long _instructions;
    [ObservableProperty] private long _failed;
    [ObservableProperty] private long _jumped;
    [ObservableProperty] private long _acquired;
    [ObservableProperty] private long _tackled;
    [ObservableProperty] private long _fellDown;
    [ObservableProperty] private long _dashed;
    [ObservableProperty] private long _switched;
    [ObservableProperty] private long _selfImpeded;
    [ObservableProperty] private long _connectionJoined;
    [ObservableProperty] private long _connectionFirst;
    [ObservableProperty] private long _connectionLast;
    [ObservableProperty] private long _totalEventLast;
    [ObservableProperty] private long _fame;

    public uint MaxPlay => PokeathlonGlobalCounters4.MaxPlay;
    public uint MaxStat => PokeathlonGlobalCounters4.MaxStat;
    public uint MaxFame => PokeathlonGlobalCounters4.MaxFame;

    partial void OnTimeSpentChanged(long value) => Counter(c => c.TimeSpent = (uint)value);
    partial void OnSessionsJoinedChanged(long value) => Counter(c => c.SessionsJoined = (uint)value);
    partial void OnPlacedFirstChanged(long value) => Counter(c => c.PlacedFirst = (uint)value);
    partial void OnPlacedLastChanged(long value) => Counter(c => c.PlacedLast = (uint)value);
    partial void OnBonusesEarnedChanged(long value) => Counter(c => c.BonusesEarned = (uint)value);
    partial void OnInstructionsChanged(long value) => Counter(c => c.Instructions = (uint)value);
    partial void OnFailedChanged(long value) => Counter(c => c.Failed = (uint)value);
    partial void OnJumpedChanged(long value) => Counter(c => c.Jumped = (uint)value);
    partial void OnAcquiredChanged(long value) => Counter(c => c.Acquired = (uint)value);
    partial void OnTackledChanged(long value) => Counter(c => c.Tackled = (uint)value);
    partial void OnFellDownChanged(long value) => Counter(c => c.FellDown = (uint)value);
    partial void OnDashedChanged(long value) => Counter(c => c.Dashed = (uint)value);
    partial void OnSwitchedChanged(long value) => Counter(c => c.Switched = (uint)value);
    partial void OnSelfImpededChanged(long value) => Counter(c => c.SelfImpeded = (uint)value);
    partial void OnConnectionJoinedChanged(long value) => Counter(c => c.ConnectionJoined = (uint)value);
    partial void OnConnectionFirstChanged(long value) => Counter(c => c.ConnectionFirst = (uint)value);
    partial void OnConnectionLastChanged(long value) => Counter(c => c.ConnectionLast = (uint)value);
    partial void OnTotalEventLastChanged(long value) => Counter(c => c.TotalEventLast = (uint)value);
    partial void OnFameChanged(long value) => Counter(c => c.Fame = (uint)value);

    public ObservableCollection<PokeathlonEventStatViewModel> EventStats { get; } = [];

    internal void SetEventFirst(PokeathlonEvent4 ev, uint value) => Counter(c => c[ev] = value);
    internal void SetBestScore(PokeathlonEvent4 ev, ushort value) => Apply(b => b.SetBestScore(ev, value));

    private void Counter(Action<PokeathlonGlobalCounters4> write)
    {
        if (_loading) return;
        var c = Block.GlobalCounters; // struct over live slice; writes persist
        write(c);
        MarkEdited();
    }

    // ---------------- Medals ----------------

    public ObservableCollection<PokeathlonMedalViewModel> Medals { get; } = [];

    internal void SetMedal(ushort species, byte bits)
    {
        if (_loading) return;
        var medals = Block.Medals;
        medals.SetMedal(species, bits);
        MarkEdited();
    }

    [RelayCommand]
    private void GiveAllMedals()
    {
        if (_loading) return;
        Block.Medals.SetAllMedals();
        MarkEdited();
        var medals = Block.Medals;
        foreach (var m in Medals)
            m.ReloadFrom(medals.GetMedal(m.Species));
    }

    [RelayCommand]
    private void ClearAllMedals()
    {
        if (_loading) return;
        Block.Medals.Clear();
        MarkEdited();
        foreach (var m in Medals)
            m.ReloadFrom(0);
    }

    // ---------------- Courses ----------------

    public ObservableCollection<PokeathlonCourseViewModel> Courses { get; } = [];

    [ObservableProperty]
    private PokeathlonCourseViewModel? _selectedCourse;

    internal PokeathlonCourseRecord4 GetCourseRecord(PokeathlonStat4 stat) => Block.GetCourseRecord(stat);

    // ---------------- Self events ----------------

    public ObservableCollection<PokeathlonEventDataViewModel> SelfEvents { get; } = [];

    [ObservableProperty]
    private PokeathlonEventDataViewModel? _selectedSelfEvent;

    internal PokeathlonEventData4 GetEventSelf(PokeathlonEvent4 ev) => Block.GetEventSelf(ev);

    // ---------------- Connections ----------------

    public ObservableCollection<PokeathlonConnectionViewModel> Connections { get; } = [];

    [ObservableProperty]
    private PokeathlonConnectionViewModel? _selectedConnection;

    internal PokeathlonConnection4 GetEventConnection(PokeathlonEvent4 ev) => Block.GetEventConnection(ev);

    // ---------------- Plumbing ----------------

    private void Apply(Action<Pokeathlon4> write)
    {
        if (_loading) return;
        write(Block);
        MarkEdited();
    }

    internal void MarkEdited()
    {
        if (!_loading)
            _sav.State.Edited = true;
    }

    internal bool IsLoading => _loading;
}

/// <summary>A single bit-flag entry (daily shop slot or Data Card), routed back to the owner on toggle.</summary>
public partial class PokeathlonFlagViewModel : ViewModelBase
{
    private readonly int _index;
    private readonly Action<int, bool> _setter;
    private bool _suppress;

    public PokeathlonFlagViewModel(PokeathlonEditorViewModel parent, int index, string name, bool isChecked, Action<int, bool> setter)
    {
        _ = parent;
        _index = index;
        Name = name;
        _isChecked = isChecked;
        _setter = setter;
    }

    public string Name { get; }

    [ObservableProperty]
    private bool _isChecked;

    partial void OnIsCheckedChanged(bool value)
    {
        if (_suppress) return;
        _setter(_index, value);
    }

    internal void SetCheckedQuiet(bool value)
    {
        _suppress = true;
        IsChecked = value;
        _suppress = false;
    }
}

/// <summary>Per-event first-place counter plus the player's best Athlete-Point score for that event.</summary>
public partial class PokeathlonEventStatViewModel : ViewModelBase
{
    private readonly PokeathlonEditorViewModel _parent;
    private readonly PokeathlonEvent4 _event;

    public PokeathlonEventStatViewModel(PokeathlonEditorViewModel parent, PokeathlonEvent4 ev, string name, uint firstPlaces, ushort bestScore)
    {
        _parent = parent;
        _event = ev;
        Name = name;
        _firstPlaces = firstPlaces;
        _bestScore = bestScore;
    }

    public string Name { get; }
    public uint MaxStat => PokeathlonGlobalCounters4.MaxStat;

    [ObservableProperty]
    private long _firstPlaces;

    [ObservableProperty]
    private int _bestScore;

    partial void OnFirstPlacesChanged(long value) => _parent.SetEventFirst(_event, (uint)value);
    partial void OnBestScoreChanged(int value) => _parent.SetBestScore(_event, (ushort)value);
}

/// <summary>One species' 5-bit medal mask (one bit per course/stat).</summary>
public partial class PokeathlonMedalViewModel : ViewModelBase
{
    private readonly PokeathlonEditorViewModel _parent;
    private readonly ISpriteRenderer? _spriteRenderer;
    private bool _suppress;

    public PokeathlonMedalViewModel(PokeathlonEditorViewModel parent, ushort species, byte bits, ISpriteRenderer? spriteRenderer)
    {
        _parent = parent;
        _spriteRenderer = spriteRenderer;
        Species = species;
        var names = Core.GameInfo.Strings.Species;
        SpeciesName = species < names.Count ? names[species] : $"#{species}";

        _speed = (bits & 0x01) != 0;
        _power = (bits & 0x02) != 0;
        _skill = (bits & 0x04) != 0;
        _stamina = (bits & 0x08) != 0;
        _jump = (bits & 0x10) != 0;
    }

    public ushort Species { get; }
    public string SpeciesName { get; }

    public byte[]? Sprite => _spriteRenderer?.GetSprite(Species, 0, 0, 0, false, EntityContext.Gen4);

    [ObservableProperty] private bool _speed;
    [ObservableProperty] private bool _power;
    [ObservableProperty] private bool _skill;
    [ObservableProperty] private bool _stamina;
    [ObservableProperty] private bool _jump;

    partial void OnSpeedChanged(bool value) => Push();
    partial void OnPowerChanged(bool value) => Push();
    partial void OnSkillChanged(bool value) => Push();
    partial void OnStaminaChanged(bool value) => Push();
    partial void OnJumpChanged(bool value) => Push();

    private byte CurrentBits =>
        (byte)((Speed ? 0x01 : 0) | (Power ? 0x02 : 0) | (Skill ? 0x04 : 0) | (Stamina ? 0x08 : 0) | (Jump ? 0x10 : 0));

    private void Push()
    {
        if (_suppress) return;
        _parent.SetMedal(Species, CurrentBits);
    }

    internal void ReloadFrom(byte bits)
    {
        _suppress = true;
        Speed = (bits & 0x01) != 0;
        Power = (bits & 0x02) != 0;
        Skill = (bits & 0x04) != 0;
        Stamina = (bits & 0x08) != 0;
        Jump = (bits & 0x10) != 0;
        _suppress = false;
    }
}

/// <summary>
/// A reusable species + form sub-VM wrapping a <see cref="SpeciesForm10"/> value.
/// Exposes a Species combo, a Form list, and a live sprite preview via the renderer.
/// The caller supplies a write-back delegate invoked whenever species/form changes.
/// </summary>
public partial class PokeathlonSpeciesFormViewModel : ViewModelBase
{
    private readonly ISpriteRenderer? _spriteRenderer;
    private readonly Action<SpeciesForm10>? _onChanged;
    private bool _suppress;

    public static IReadOnlyList<ComboItem> SpeciesSource { get; } = BuildSpeciesSource();

    public PokeathlonSpeciesFormViewModel(SpeciesForm10 value, ISpriteRenderer? spriteRenderer, Action<SpeciesForm10>? onChanged)
    {
        _spriteRenderer = spriteRenderer;
        _onChanged = onChanged;
        _species = value.Species;
        _formList = BuildFormList(value.Species);
        _form = Math.Min(value.Form, (byte)Math.Max(0, _formList.Count - 1));
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Sprite))]
    private int _species;

    [ObservableProperty]
    private IReadOnlyList<ComboItem> _formList;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Sprite))]
    private int _form;

    public byte[]? Sprite => _spriteRenderer?.GetSprite((ushort)Species, (byte)Math.Max(0, Form), 0, 0, false, EntityContext.Gen4);

    public SpeciesForm10 Value => new() { Species = (ushort)Species, Form = (byte)Math.Max(0, Form) };

    partial void OnSpeciesChanged(int value)
    {
        // Rebuild the form list for the new species and clamp the selection.
        _suppress = true;
        FormList = BuildFormList((ushort)value);
        if (Form >= FormList.Count)
            Form = 0;
        _suppress = false;
        Push();
    }

    partial void OnFormChanged(int value) => Push();

    private void Push()
    {
        if (_suppress) return;
        _onChanged?.Invoke(Value);
    }

    /// <summary>Replace the wrapped value without triggering write-back (used when switching the selected index).</summary>
    internal void Load(SpeciesForm10 value)
    {
        _suppress = true;
        Species = value.Species;
        FormList = BuildFormList(value.Species);
        Form = Math.Min(value.Form, Math.Max(0, FormList.Count - 1));
        _suppress = false;
        OnPropertyChanged(nameof(Sprite));
    }

    private static IReadOnlyList<ComboItem> BuildSpeciesSource()
    {
        var names = Core.GameInfo.Strings.Species;
        var list = new List<ComboItem>(PokeathlonEditorViewModel.MaxSpeciesGen4 + 1);
        for (int i = 0; i <= PokeathlonEditorViewModel.MaxSpeciesGen4 && i < names.Count; i++)
            list.Add(new ComboItem(string.IsNullOrEmpty(names[i]) ? $"#{i}" : names[i], i));
        return list;
    }

    private static IReadOnlyList<ComboItem> BuildFormList(ushort species)
    {
        if (species == 0)
            return [new ComboItem("0", 0)];
        var forms = FormConverter.GetFormList(species, Core.GameInfo.Strings.Types, Core.GameInfo.Strings.forms, Core.GameInfo.GenderSymbolASCII, EntityContext.Gen4);
        var list = new List<ComboItem>(Math.Max(1, forms.Length));
        for (int i = 0; i < forms.Length; i++)
            list.Add(new ComboItem(string.IsNullOrEmpty(forms[i]) ? i.ToString() : forms[i], i));
        if (list.Count == 0)
            list.Add(new ComboItem("0", 0));
        return list;
    }
}

/// <summary>
/// A single Pokéathlon participant (a course/record team member): species/form/gender/shiny + EC/PID, TID16/SID16.
/// </summary>
public partial class PokeathlonParticipantViewModel : ViewModelBase
{
    private readonly PokeathlonEditorViewModel _parent;
    private readonly Func<PokeathlonParticipant4> _accessor;
    private readonly ISpriteRenderer? _spriteRenderer;
    private bool _suppress;

    public PokeathlonParticipantViewModel(PokeathlonEditorViewModel parent, int slot, Func<PokeathlonParticipant4> accessor, ISpriteRenderer? spriteRenderer)
    {
        _parent = parent;
        _accessor = accessor;
        _spriteRenderer = spriteRenderer;
        Slot = slot + 1;

        var p = accessor();
        _species = p.Species;
        _formList = BuildFormList(p.Species);
        _form = Math.Min(p.Form, (byte)Math.Max(0, _formList.Count - 1));
        _gender = p.Gender;
        _isShiny = p.IsShiny;
        _encryptionConstant = p.EncryptionConstant.ToString("X8");
        _tid16 = p.TID16;
        _sid16 = p.SID16;
    }

    public int Slot { get; }

    public IReadOnlyList<ComboItem> SpeciesSource => PokeathlonSpeciesFormViewModel.SpeciesSource;
    public IReadOnlyList<ComboItem> GenderList { get; } =
    [
        new ComboItem("Male", 0),
        new ComboItem("Female", 1),
        new ComboItem("Genderless", 2),
    ];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Sprite))]
    private int _species;

    [ObservableProperty]
    private IReadOnlyList<ComboItem> _formList;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Sprite))]
    private int _form;

    [ObservableProperty]
    private int _gender;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Sprite))]
    private bool _isShiny;

    [ObservableProperty]
    private string _encryptionConstant;

    [ObservableProperty]
    private int _tid16;

    [ObservableProperty]
    private int _sid16;

    public byte[]? Sprite => _spriteRenderer?.GetSprite((ushort)Species, (byte)Math.Max(0, Form), (byte)Gender, 0, IsShiny, EntityContext.Gen4);

    partial void OnSpeciesChanged(int value)
    {
        _suppress = true;
        FormList = BuildFormList((ushort)value);
        if (Form >= FormList.Count)
            Form = 0;
        _suppress = false;
        Write(p => { p.Species = (ushort)value; p.Form = (byte)Math.Max(0, Form); });
    }

    partial void OnFormChanged(int value) => Write(p => p.Form = (byte)Math.Max(0, value));
    partial void OnGenderChanged(int value) => Write(p => p.Gender = (byte)value);
    partial void OnIsShinyChanged(bool value) => Write(p => p.IsShiny = value);
    partial void OnTid16Changed(int value) => Write(p => p.TID16 = (ushort)value);
    partial void OnSid16Changed(int value) => Write(p => p.SID16 = (ushort)value);

    partial void OnEncryptionConstantChanged(string value)
    {
        if (_suppress) return;
        uint ec = ParseHex(value);
        Write(p => p.EncryptionConstant = ec);
    }

    private void Write(Action<PokeathlonParticipant4> apply)
    {
        if (_suppress || _parent.IsLoading) return;
        var p = _accessor();
        apply(p);
        _parent.MarkEdited();
    }

    private static uint ParseHex(string text)
    {
        text = new string(text.Where(Uri.IsHexDigit).ToArray());
        return uint.TryParse(text, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var value) ? value : 0;
    }

    private static IReadOnlyList<ComboItem> BuildFormList(ushort species)
    {
        if (species == 0)
            return [new ComboItem("0", 0)];
        var forms = FormConverter.GetFormList(species, Core.GameInfo.Strings.Types, Core.GameInfo.Strings.forms, Core.GameInfo.GenderSymbolASCII, EntityContext.Gen4);
        var list = new List<ComboItem>(Math.Max(1, forms.Length));
        for (int i = 0; i < forms.Length; i++)
            list.Add(new ComboItem(string.IsNullOrEmpty(forms[i]) ? i.ToString() : forms[i], i));
        if (list.Count == 0)
            list.Add(new ComboItem("0", 0));
        return list;
    }
}

/// <summary>One of the five course records (per <see cref="PokeathlonStat4"/>): four scores + three participants.</summary>
public partial class PokeathlonCourseViewModel : ViewModelBase
{
    private readonly PokeathlonEditorViewModel _parent;
    private readonly PokeathlonStat4 _stat;

    public PokeathlonCourseViewModel(PokeathlonEditorViewModel parent, PokeathlonStat4 stat, string name, ISpriteRenderer? spriteRenderer)
    {
        _parent = parent;
        _stat = stat;
        Name = name;

        var record = parent.GetCourseRecord(stat);
        _score0 = record.Score0;
        _score1 = record.Score1;
        _score2 = record.Score2;
        _scoreMax = record.ScoreMax;

        for (int i = 0; i < PokeathlonCourseRecord4.CountParticipant; i++)
        {
            int slot = i;
            Participants.Add(new PokeathlonParticipantViewModel(parent, slot,
                () => parent.GetCourseRecord(stat).GetParticipant(slot), spriteRenderer));
        }
    }

    public string Name { get; }

    public ObservableCollection<PokeathlonParticipantViewModel> Participants { get; } = [];

    [ObservableProperty] private int _score0;
    [ObservableProperty] private int _score1;
    [ObservableProperty] private int _score2;
    [ObservableProperty] private int _scoreMax;

    partial void OnScore0Changed(int value) => Write(r => r.Score0 = (ushort)value);
    partial void OnScore1Changed(int value) => Write(r => r.Score1 = (ushort)value);
    partial void OnScore2Changed(int value) => Write(r => r.Score2 = (ushort)value);
    partial void OnScoreMaxChanged(int value) => Write(r => r.ScoreMax = (ushort)value);

    private void Write(Action<PokeathlonCourseRecord4> apply)
    {
        if (_parent.IsLoading) return;
        var record = _parent.GetCourseRecord(_stat);
        apply(record);
        _parent.MarkEdited();
    }
}

/// <summary>One of the ten self-event records: an attempt counter plus five record rows (value + 3 species entries).</summary>
public partial class PokeathlonEventDataViewModel : ViewModelBase
{
    private readonly PokeathlonEditorViewModel _parent;
    private readonly PokeathlonEvent4 _event;

    public PokeathlonEventDataViewModel(PokeathlonEditorViewModel parent, PokeathlonEvent4 ev, string name, ISpriteRenderer? spriteRenderer)
    {
        _parent = parent;
        _event = ev;
        Name = name;

        var data = parent.GetEventSelf(ev);
        _attempts = data.Attempts;

        for (int i = 0; i < (int)PokeathlonEventData4.MaxRecord; i++)
        {
            int slot = i;
            Records.Add(new PokeathlonEventRecordViewModel(parent, slot,
                () => parent.GetEventSelf(ev).GetRecord(slot), spriteRenderer));
        }
    }

    public string Name { get; }
    public uint MaxAttempts => PokeathlonEventData4.MaxAttempts;

    public ObservableCollection<PokeathlonEventRecordViewModel> Records { get; } = [];

    [ObservableProperty] private long _attempts;

    partial void OnAttemptsChanged(long value)
    {
        if (_parent.IsLoading) return;
        var data = _parent.GetEventSelf(_event);
        data.Attempts = (uint)value;
        _parent.MarkEdited();
    }
}

/// <summary>A single event record row: a record value plus three responsible species (species+form each).</summary>
public partial class PokeathlonEventRecordViewModel : ViewModelBase
{
    private readonly PokeathlonEditorViewModel _parent;
    private readonly Func<PokeathlonEventRecord4> _accessor;

    public PokeathlonEventRecordViewModel(PokeathlonEditorViewModel parent, int slot, Func<PokeathlonEventRecord4> accessor, ISpriteRenderer? spriteRenderer)
    {
        _parent = parent;
        _accessor = accessor;
        Slot = slot + 1;

        var record = accessor();
        _record = record.Record;
        Entry0 = new PokeathlonSpeciesFormViewModel(record.Entry0, spriteRenderer, v => WriteEntry(0, v));
        Entry1 = new PokeathlonSpeciesFormViewModel(record.Entry1, spriteRenderer, v => WriteEntry(1, v));
        Entry2 = new PokeathlonSpeciesFormViewModel(record.Entry2, spriteRenderer, v => WriteEntry(2, v));
    }

    public int Slot { get; }

    public PokeathlonSpeciesFormViewModel Entry0 { get; }
    public PokeathlonSpeciesFormViewModel Entry1 { get; }
    public PokeathlonSpeciesFormViewModel Entry2 { get; }

    [ObservableProperty] private int _record;

    partial void OnRecordChanged(int value)
    {
        if (_parent.IsLoading) return;
        var record = _accessor();
        record.Record = (ushort)value;
        _parent.MarkEdited();
    }

    private void WriteEntry(int index, SpeciesForm10 value)
    {
        if (_parent.IsLoading) return;
        var record = _accessor();
        switch (index)
        {
            case 0: record.Entry0 = value; break;
            case 1: record.Entry1 = value; break;
            case 2: record.Entry2 = value; break;
        }
        _parent.MarkEdited();
    }
}

/// <summary>
/// One of the ten connection (wireless) event slots: an inner attempt counter + five record rows, plus five
/// opposing-trainer records (OT name / TID16 / SID16 / language) correlated to those records.
/// </summary>
public partial class PokeathlonConnectionViewModel : ViewModelBase
{
    private readonly PokeathlonEditorViewModel _parent;
    private readonly PokeathlonEvent4 _event;

    public PokeathlonConnectionViewModel(PokeathlonEditorViewModel parent, PokeathlonEvent4 ev, string name, ISpriteRenderer? spriteRenderer)
    {
        _parent = parent;
        _event = ev;
        Name = name;

        var connection = parent.GetEventConnection(ev);
        _attempts = connection.Inner.Attempts;

        for (int i = 0; i < (int)PokeathlonEventData4.MaxRecord; i++)
        {
            int slot = i;
            Records.Add(new PokeathlonEventRecordViewModel(parent, slot,
                () => parent.GetEventConnection(ev).Inner.GetRecord(slot), spriteRenderer));
            Trainers.Add(new PokeathlonEventTrainerViewModel(parent, slot,
                () => parent.GetEventConnection(ev).GetTrainer(slot)));
        }
    }

    public string Name { get; }
    public uint MaxAttempts => PokeathlonEventData4.MaxAttempts;

    public ObservableCollection<PokeathlonEventRecordViewModel> Records { get; } = [];
    public ObservableCollection<PokeathlonEventTrainerViewModel> Trainers { get; } = [];

    [ObservableProperty] private long _attempts;

    partial void OnAttemptsChanged(long value)
    {
        if (_parent.IsLoading) return;
        var data = _parent.GetEventConnection(_event).Inner;
        data.Attempts = (uint)value;
        _parent.MarkEdited();
    }
}

/// <summary>An opposing trainer correlated to a connection record: OT name, TID16/SID16, language.</summary>
public partial class PokeathlonEventTrainerViewModel : ViewModelBase
{
    private readonly PokeathlonEditorViewModel _parent;
    private readonly Func<PokeathlonEventTrainer4> _accessor;

    public PokeathlonEventTrainerViewModel(PokeathlonEditorViewModel parent, int slot, Func<PokeathlonEventTrainer4> accessor)
    {
        _parent = parent;
        _accessor = accessor;
        Slot = slot + 1;

        var trainer = accessor();
        _originalTrainerName = trainer.OriginalTrainerName;
        _tid16 = trainer.TID16;
        _sid16 = trainer.SID16;
        _language = trainer.Language;
    }

    public int Slot { get; }

    [ObservableProperty] private string _originalTrainerName;
    [ObservableProperty] private int _tid16;
    [ObservableProperty] private int _sid16;
    [ObservableProperty] private int _language;

    partial void OnOriginalTrainerNameChanged(string value) => Write(t => t.OriginalTrainerName = value ?? string.Empty);
    partial void OnTid16Changed(int value) => Write(t => t.TID16 = (ushort)value);
    partial void OnSid16Changed(int value) => Write(t => t.SID16 = (ushort)value);
    partial void OnLanguageChanged(int value) => Write(t => t.Language = (byte)value);

    private void Write(Action<PokeathlonEventTrainer4> apply)
    {
        if (_parent.IsLoading) return;
        var trainer = _accessor();
        apply(trainer);
        _parent.MarkEdited();
    }
}
