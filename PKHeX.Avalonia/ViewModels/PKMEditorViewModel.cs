using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Avalonia.Converters;
using PKHeX.Avalonia.ViewModels.Subforms;
using PKHeX.Avalonia.Views.Subforms;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite.Avalonia;
using SkiaSharp;

namespace PKHeX.Avalonia.ViewModels;

/// <summary>
/// ViewModel for the PKM editor panel. Implements Pokemon data viewing and editing.
/// </summary>
public partial class PKMEditorViewModel : ObservableObject
{
    private SaveFile? _sav;
    private bool _isPopulating;

    [ObservableProperty]
    private PKM? _entity;

    [ObservableProperty]
    private Bitmap? _spriteImage;

    [ObservableProperty]
    private Bitmap? _legalityImage;

    // Basic Info
    [ObservableProperty] private ushort _species;
    [ObservableProperty] private string _nickname = string.Empty;
    [ObservableProperty] private byte _level;
    [ObservableProperty] private byte _form;
    [ObservableProperty] private byte _gender;
    [ObservableProperty] private Nature _nature;

    /// <summary>
    /// Returns the gender symbol for the current <see cref="Gender"/> value:
    /// 0 = Male, 1 = Female, 2 = Genderless.
    /// </summary>
    public string GenderSymbol => Gender switch
    {
        0 => "\u2642",  // ♂
        1 => "\u2640",  // ♀
        _ => "\u2014",  // —
    };

    partial void OnGenderChanged(byte value)
    {
        OnPropertyChanged(nameof(GenderSymbol));
        if (!_isPopulating && Entity is not null)
        {
            Entity.Gender = value;
            UpdateSprite();
            UpdateLegality();
        }
    }

    /// <summary>
    /// Cycles the PKM gender: Male → Female → Genderless → Male.
    /// Only applies if the species supports multiple genders.
    /// </summary>
    [RelayCommand]
    private void ToggleGender()
    {
        if (Entity is null) return;
        var pi = Entity.PersonalInfo;
        // Fixed gender species cannot be toggled
        if (pi.Genderless || pi.OnlyFemale || pi.OnlyMale)
            return;
        Gender = (byte)((Gender + 1) % 2); // Toggle between 0 (M) and 1 (F)
    }

    /// <summary>Tooltip showing the numeric species ID.</summary>
    public string SpeciesTooltip => Entity is null ? "" : $"Species #{Entity.Species:000}";

    /// <summary>
    /// Tooltip showing which stats are raised/lowered by the current nature.
    /// </summary>
    public string NatureTooltip
    {
        get
        {
            var n = Nature;
            var idx = (int)n;
            if ((uint)idx >= 25) return n.ToString();
            var up = idx / 5;
            var down = idx % 5;
            if (up == down) return $"{n} (Neutral)";
            var statNames = new[] { "Atk", "Def", "Spe", "SpA", "SpD" };
            return $"{n} (+{statNames[up]} / -{statNames[down]})";
        }
    }

    // Stat Nature (Gen 8+)
    [ObservableProperty] private Nature _statNature;
    [ObservableProperty] private bool _hasStatNature;
    [ObservableProperty] private ComboItem? _selectedStatNature;

    partial void OnSelectedStatNatureChanged(ComboItem? value)
    {
        if (value is not null)
            StatNature = (Nature)value.Value;
        if (!_isPopulating && Entity is not null)
        {
            Entity.StatNature = StatNature;
            OnPropertyChanged(nameof(AtkColor));
            OnPropertyChanged(nameof(DefColor));
            OnPropertyChanged(nameof(SpAColor));
            OnPropertyChanged(nameof(SpDColor));
            OnPropertyChanged(nameof(SpeColor));
            RecalcStats();
            UpdateLegality();
        }
    }

    [ObservableProperty] private int _ability;
    [ObservableProperty] private int _heldItem;
    [ObservableProperty] private bool _isShiny;
    [ObservableProperty] private bool _isEgg;
    [ObservableProperty] private bool _isNicknamed;

    partial void OnIsNicknamedChanged(bool value) { if (!_isPopulating && Entity is not null) { Entity.IsNicknamed = value; UpdateLegality(); } }

    // HaX mode: raw ability number (bit field)
    [ObservableProperty] private int _abilityNumber;
    public bool IsHaXMode => App.HaX;

    partial void OnAbilityNumberChanged(int value)
    {
        if (!_isPopulating && Entity is not null)
        {
            Entity.AbilityNumber = value;
            UpdateLegality();
        }
    }

    /// <summary>
    /// Returns "Hatch Counter:" when the PKM is an egg, otherwise "Friendship:".
    /// </summary>
    public string FriendshipLabel => IsEgg ? "Hatch Counter:" : "Friendship:";

    // Pokerus
    [ObservableProperty] private bool _isInfected;
    [ObservableProperty] private bool _isCured;
    [ObservableProperty] private int _pkrsStrain;
    [ObservableProperty] private int _pkrsDays;

    public bool ShowPkrsDetails => IsInfected;

    partial void OnPkrsStrainChanged(int value) { if (!_isPopulating && Entity is not null) { Entity.PokerusStrain = value; UpdateLegality(); } }
    partial void OnPkrsDaysChanged(int value) { if (!_isPopulating && Entity is not null) { Entity.PokerusDays = value; UpdateLegality(); } }

    partial void OnIsCuredChanged(bool value)
    {
        ShowCuredMark = value;
        if (!_isPopulating && Entity is not null)
        {
            Entity.IsPokerusCured = value;
            UpdateLegality();
        }
    }

    partial void OnIsInfectedChanged(bool value)
    {
        OnPropertyChanged(nameof(ShowPkrsDetails));
        if (!_isPopulating && Entity is not null)
        {
            Entity.IsPokerusInfected = value;
            UpdateLegality();
        }
    }

    // Shiny display indicators
    [ObservableProperty] private bool _isShinyDisplay;
    [ObservableProperty] private bool _isSquareShiny;

    // New fields
    [ObservableProperty] private string _pidHex = "00000000";

    partial void OnPidHexChanged(string value)
    {
        if (!_isPopulating && Entity is not null && uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out var pid))
        {
            Entity.PID = pid;
            IsShiny = Entity.IsShiny;
            IsShinyDisplay = Entity.IsShiny;
            IsSquareShiny = Entity.IsShiny && Entity.ShinyXor == 0;
            ShowShinyMark = Entity.IsShiny;
            // Refresh characteristic (depends on PID % 6 for tiebreaker)
            var charIdx = Entity.Characteristic;
            CharacteristicText = charIdx >= 0 && charIdx < GameInfo.Strings.characteristics.Length
                ? GameInfo.Strings.characteristics[charIdx] : string.Empty;
            UpdateSprite();
            UpdateLegality();
        }
    }
    [ObservableProperty] private uint _exp;
    [ObservableProperty] private int _friendship;
    [ObservableProperty] private int _language;

    partial void OnFriendshipChanged(int value) { if (!_isPopulating && Entity is not null) { if (IsEgg) Entity.OriginalTrainerFriendship = (byte)Math.Clamp(value, 0, 255); else Entity.CurrentFriendship = (byte)Math.Clamp(value, 0, 255); UpdateLegality(); } }

    // Stats
    [ObservableProperty] private int _hp;
    [ObservableProperty] private int _atk;
    [ObservableProperty] private int _def;
    [ObservableProperty] private int _spA;
    [ObservableProperty] private int _spD;
    [ObservableProperty] private int _spe;

    // Base Stats (read-only display)
    [ObservableProperty] private int _base_HP;
    [ObservableProperty] private int _base_ATK;
    [ObservableProperty] private int _base_DEF;
    [ObservableProperty] private int _base_SPA;
    [ObservableProperty] private int _base_SPD;
    [ObservableProperty] private int _base_SPE;

    // IVs
    [ObservableProperty] private int _iv_HP;
    [ObservableProperty] private int _iv_ATK;
    [ObservableProperty] private int _iv_DEF;
    [ObservableProperty] private int _iv_SPA;
    [ObservableProperty] private int _iv_SPD;
    [ObservableProperty] private int _iv_SPE;

    // EVs
    [ObservableProperty] private int _ev_HP;
    [ObservableProperty] private int _ev_ATK;
    [ObservableProperty] private int _ev_DEF;
    [ObservableProperty] private int _ev_SPA;
    [ObservableProperty] private int _ev_SPD;
    [ObservableProperty] private int _ev_SPE;

    // Moves
    [ObservableProperty] private ushort _move1;
    [ObservableProperty] private ushort _move2;
    [ObservableProperty] private ushort _move3;
    [ObservableProperty] private ushort _move4;

    // Move PP and PP Ups
    [ObservableProperty] private int _move1_PP;
    [ObservableProperty] private int _move2_PP;
    [ObservableProperty] private int _move3_PP;
    [ObservableProperty] private int _move4_PP;
    [ObservableProperty] private int _move1_PPUps;
    [ObservableProperty] private int _move2_PPUps;
    [ObservableProperty] private int _move3_PPUps;
    [ObservableProperty] private int _move4_PPUps;

    // Relearn Moves
    [ObservableProperty] private ushort _relearnMove1;
    [ObservableProperty] private ushort _relearnMove2;
    [ObservableProperty] private ushort _relearnMove3;
    [ObservableProperty] private ushort _relearnMove4;
    [ObservableProperty] private ComboItem? _selectedRelearnMove1;
    [ObservableProperty] private ComboItem? _selectedRelearnMove2;
    [ObservableProperty] private ComboItem? _selectedRelearnMove3;
    [ObservableProperty] private ComboItem? _selectedRelearnMove4;
    [ObservableProperty] private bool _hasRelearnMoves;

    partial void OnSelectedRelearnMove1Changed(ComboItem? value) { if (value is not null) RelearnMove1 = (ushort)value.Value; if (!_isPopulating && Entity is not null) { Entity.RelearnMove1 = RelearnMove1; UpdateLegality(); } }
    partial void OnSelectedRelearnMove2Changed(ComboItem? value) { if (value is not null) RelearnMove2 = (ushort)value.Value; if (!_isPopulating && Entity is not null) { Entity.RelearnMove2 = RelearnMove2; UpdateLegality(); } }
    partial void OnSelectedRelearnMove3Changed(ComboItem? value) { if (value is not null) RelearnMove3 = (ushort)value.Value; if (!_isPopulating && Entity is not null) { Entity.RelearnMove3 = RelearnMove3; UpdateLegality(); } }
    partial void OnSelectedRelearnMove4Changed(ComboItem? value) { if (value is not null) RelearnMove4 = (ushort)value.Value; if (!_isPopulating && Entity is not null) { Entity.RelearnMove4 = RelearnMove4; UpdateLegality(); } }

    // Move legality indicators
    [ObservableProperty] private bool _move1Legal = true;
    [ObservableProperty] private bool _move2Legal = true;
    [ObservableProperty] private bool _move3Legal = true;
    [ObservableProperty] private bool _move4Legal = true;

    // Relearn move legality indicators
    [ObservableProperty] private bool _relearn1Legal = true;
    [ObservableProperty] private bool _relearn2Legal = true;
    [ObservableProperty] private bool _relearn3Legal = true;
    [ObservableProperty] private bool _relearn4Legal = true;

    // Status Condition display
    [ObservableProperty] private string _statusConditionText = string.Empty;

    // Hyper Training
    [ObservableProperty] private bool _htHp;
    [ObservableProperty] private bool _htAtk;
    [ObservableProperty] private bool _htDef;
    [ObservableProperty] private bool _htSpA;
    [ObservableProperty] private bool _htSpD;
    [ObservableProperty] private bool _htSpe;
    [ObservableProperty] private bool _hasHyperTraining;

    partial void OnHtHpChanged(bool value)  { if (!_isPopulating && Entity is IHyperTrain ht) { ht.HT_HP  = value; RecalcStats(); UpdateLegality(); } }
    partial void OnHtAtkChanged(bool value) { if (!_isPopulating && Entity is IHyperTrain ht) { ht.HT_ATK = value; RecalcStats(); UpdateLegality(); } }
    partial void OnHtDefChanged(bool value) { if (!_isPopulating && Entity is IHyperTrain ht) { ht.HT_DEF = value; RecalcStats(); UpdateLegality(); } }
    partial void OnHtSpAChanged(bool value) { if (!_isPopulating && Entity is IHyperTrain ht) { ht.HT_SPA = value; RecalcStats(); UpdateLegality(); } }
    partial void OnHtSpDChanged(bool value) { if (!_isPopulating && Entity is IHyperTrain ht) { ht.HT_SPD = value; RecalcStats(); UpdateLegality(); } }
    partial void OnHtSpeChanged(bool value) { if (!_isPopulating && Entity is IHyperTrain ht) { ht.HT_SPE = value; RecalcStats(); UpdateLegality(); } }

    // Dynamax
    [ObservableProperty] private int _dynamaxLevel;
    [ObservableProperty] private bool _hasDynamaxLevel;

    partial void OnDynamaxLevelChanged(int value) { if (!_isPopulating && Entity is IDynamaxLevel dl) { dl.DynamaxLevel = (byte)Math.Clamp(value, 0, 10); UpdateLegality(); } }

    // Gigantamax
    [ObservableProperty] private bool _canGigantamax;
    [ObservableProperty] private bool _hasGigantamax;

    partial void OnCanGigantamaxChanged(bool value) { if (!_isPopulating && Entity is IGigantamax gm) { gm.CanGigantamax = value; UpdateLegality(); } }

    // Alpha (Legends Arceus)
    [ObservableProperty] private bool _isAlpha;
    [ObservableProperty] private bool _hasAlpha;

    partial void OnIsAlphaChanged(bool value) { if (!_isPopulating && Entity is IAlpha a) { a.IsAlpha = value; UpdateLegality(); } }

    // Noble (Legends Arceus)
    [ObservableProperty] private bool _isNoble;
    [ObservableProperty] private bool _hasNoble;

    partial void OnIsNobleChanged(bool value) { if (!_isPopulating && Entity is INoble n) { n.IsNoble = value; UpdateLegality(); } }

    // Tera Type (Gen 9)
    [ObservableProperty] private int _teraTypeOriginal;
    [ObservableProperty] private int _teraTypeOverride;
    [ObservableProperty] private bool _hasTeraType;
    [ObservableProperty] private IReadOnlyList<ComboItem> _teraTypeList = Array.Empty<ComboItem>();
    [ObservableProperty] private ComboItem? _selectedTeraOriginal;
    [ObservableProperty] private ComboItem? _selectedTeraOverride;

    partial void OnSelectedTeraOriginalChanged(ComboItem? value)
    {
        if (value is not null)
            TeraTypeOriginal = value.Value;
        if (!_isPopulating && Entity is not null && Entity is ITeraType tt)
        {
            tt.TeraTypeOriginal = (MoveType)TeraTypeOriginal;
            UpdateLegality();
        }
    }

    partial void OnSelectedTeraOverrideChanged(ComboItem? value)
    {
        if (value is not null)
            TeraTypeOverride = value.Value;
        if (!_isPopulating && Entity is not null && Entity is ITeraType tt)
        {
            tt.TeraTypeOverride = (MoveType)TeraTypeOverride;
            UpdateLegality();
        }
    }

    // EV Total display
    [ObservableProperty] private int _evTotal;
    [ObservableProperty] private bool _isEvTotalValid;

    // Hidden Power Type display
    [ObservableProperty] private string _hiddenPowerType = string.Empty;

    // Characteristic display
    [ObservableProperty] private string _characteristicText = string.Empty;

    // Base Stat Total
    [ObservableProperty] private int _baseST;

    // Ball sprite
    [ObservableProperty] private Bitmap? _ballSprite;

    // Met
    [ObservableProperty] private ushort _metLocation;
    [ObservableProperty] private byte _metLevel;
    [ObservableProperty] private byte _ball;
    [ObservableProperty] private ushort _eggLocation;
    [ObservableProperty] private int _version;

    partial void OnMetLevelChanged(byte value) { if (!_isPopulating && Entity is not null) { Entity.MetLevel = value; UpdateLegality(); } }
    [ObservableProperty] private int _metYear;
    [ObservableProperty] private int _metMonth;
    [ObservableProperty] private int _metDay;
    [ObservableProperty] private int _eggYear;
    [ObservableProperty] private int _eggMonth;
    [ObservableProperty] private int _eggDay;

    // Met/Egg Y/M/D handlers — only fire for direct NumericUpDown edits (not from CalendarDatePicker setter)
    partial void OnMetYearChanged(int value) { if (!_isPopulating && Entity is not null) { Entity.MetYear = (byte)Math.Clamp(value, 0, 255); OnPropertyChanged(nameof(MetDate)); UpdateLegality(); } }
    partial void OnMetMonthChanged(int value) { if (!_isPopulating && Entity is not null) { Entity.MetMonth = (byte)Math.Clamp(value, 0, 12); OnPropertyChanged(nameof(MetDate)); UpdateLegality(); } }
    partial void OnMetDayChanged(int value) { if (!_isPopulating && Entity is not null) { Entity.MetDay = (byte)Math.Clamp(value, 0, 31); OnPropertyChanged(nameof(MetDate)); UpdateLegality(); } }
    partial void OnEggYearChanged(int value) { if (!_isPopulating && Entity is not null) { Entity.EggYear = (byte)Math.Clamp(value, 0, 255); OnPropertyChanged(nameof(EggDate)); }  UpdateLegality(); }
    partial void OnEggMonthChanged(int value) { if (!_isPopulating && Entity is not null) { Entity.EggMonth = (byte)Math.Clamp(value, 0, 12); OnPropertyChanged(nameof(EggDate)); }  UpdateLegality(); }
    partial void OnEggDayChanged(int value) { if (!_isPopulating && Entity is not null) { Entity.EggDay = (byte)Math.Clamp(value, 0, 31); OnPropertyChanged(nameof(EggDate)); }  UpdateLegality(); }

    /// <summary>Calendar-friendly Met Date property. Syncs with MetYear/MetMonth/MetDay.</summary>
    public DateTimeOffset? MetDate
    {
        get
        {
            if (MetYear <= 0 && MetMonth == 0 && MetDay == 0) return null;
            try
            {
                var yr = Math.Max(MetYear, 0);
                var m = Math.Clamp(MetMonth, 1, 12);
                var maxDay = DateTime.DaysInMonth(2000 + yr, m);
                var d = Math.Clamp(MetDay, 1, maxDay);
                return new DateTimeOffset(2000 + yr, m, d, 0, 0, 0, TimeSpan.Zero);
            }
            catch { return null; }
        }
        set
        {
            _isPopulating = true;
            try
            {
                if (value is { } d)
                {
                    int year = d.Year - 2000;
                    if (year is < 0 or > 255) return;
                    MetYear = year;
                    MetMonth = d.Month;
                    MetDay = d.Day;
                }
                else
                {
                    MetYear = 0; MetMonth = 0; MetDay = 0;
                }
            }
            finally { _isPopulating = false; }
            if (Entity is not null)
            {
                Entity.MetYear = (byte)Math.Clamp(MetYear, 0, 255);
                Entity.MetMonth = (byte)Math.Clamp(MetMonth, 0, 12);
                Entity.MetDay = (byte)Math.Clamp(MetDay, 0, 31);
                UpdateLegality();
            }
            OnPropertyChanged();
        }
    }

    /// <summary>Calendar-friendly Egg Date property. Syncs with EggYear/EggMonth/EggDay.</summary>
    public DateTimeOffset? EggDate
    {
        get
        {
            if (EggYear <= 0 && EggMonth == 0 && EggDay == 0) return null;
            try
            {
                var yr = Math.Max(EggYear, 0);
                var m = Math.Clamp(EggMonth, 1, 12);
                var maxDay = DateTime.DaysInMonth(2000 + yr, m);
                var d = Math.Clamp(EggDay, 1, maxDay);
                return new DateTimeOffset(2000 + yr, m, d, 0, 0, 0, TimeSpan.Zero);
            }
            catch { return null; }
        }
        set
        {
            _isPopulating = true;
            try
            {
                if (value is { } d)
                {
                    int year = d.Year - 2000;
                    if (year is < 0 or > 255) return;
                    EggYear = year;
                    EggMonth = d.Month;
                    EggDay = d.Day;
                }
                else
                {
                    EggYear = 0; EggMonth = 0; EggDay = 0;
                }
            }
            finally { _isPopulating = false; }
            if (Entity is not null)
            {
                Entity.EggYear = (byte)Math.Clamp(EggYear, 0, 255);
                Entity.EggMonth = (byte)Math.Clamp(EggMonth, 0, 12);
                Entity.EggDay = (byte)Math.Clamp(EggDay, 0, 31);
            }
            OnPropertyChanged();
        }
    }

    // Cosmetic — Markings (int: 0=None, 1=Blue, 2=Pink for Gen7+; 0/1 for older gens)
    [ObservableProperty] private int _markCircle;
    [ObservableProperty] private int _markTriangle;
    [ObservableProperty] private int _markSquare;
    [ObservableProperty] private int _markHeart;
    [ObservableProperty] private int _markStar;
    [ObservableProperty] private int _markDiamond;
    [ObservableProperty] private bool _hasGen7Markings;

    // Color strings for marking display
    public string MarkCircleColor => GetMarkingColorString(MarkCircle);
    public string MarkTriangleColor => GetMarkingColorString(MarkTriangle);
    public string MarkSquareColor => GetMarkingColorString(MarkSquare);
    public string MarkHeartColor => GetMarkingColorString(MarkHeart);
    public string MarkStarColor => GetMarkingColorString(MarkStar);
    public string MarkDiamondColor => GetMarkingColorString(MarkDiamond);

    // Tooltip strings for marking buttons (show shape + current state)
    public string MarkCircleTip => GetMarkingTip("Circle", MarkCircle);
    public string MarkTriangleTip => GetMarkingTip("Triangle", MarkTriangle);
    public string MarkSquareTip => GetMarkingTip("Square", MarkSquare);
    public string MarkHeartTip => GetMarkingTip("Heart", MarkHeart);
    public string MarkStarTip => GetMarkingTip("Star", MarkStar);
    public string MarkDiamondTip => GetMarkingTip("Diamond", MarkDiamond);

    private static string GetMarkingColorString(int state) => state switch
    {
        1 => "#4488FF", // Blue
        2 => "#FF66AA", // Pink
        _ => "#888888", // None/Gray
    };

    private static string GetMarkingStateName(int state) => state switch
    {
        1 => "Blue",
        2 => "Pink",
        _ => "None",
    };

    private static string GetMarkingTip(string shape, int state) =>
        $"{shape}: {GetMarkingStateName(state)} (click to cycle)";

    private void CycleMarking(ref int field, string fieldName, string colorName, string tipName)
    {
        int max = HasGen7Markings ? 3 : 2; // 3 states for Gen7+, 2 for older
        field = (field + 1) % max;
        OnPropertyChanged(fieldName);
        OnPropertyChanged(colorName);
        OnPropertyChanged(tipName);
        WriteMarkingsToEntity();
    }

    private void WriteMarkingsToEntity()
    {
        if (Entity is null) return;
        if (Entity is IAppliedMarkings7 m7)
        {
            m7.MarkingCircle = (MarkingColor)MarkCircle; m7.MarkingTriangle = (MarkingColor)MarkTriangle;
            m7.MarkingSquare = (MarkingColor)MarkSquare; m7.MarkingHeart = (MarkingColor)MarkHeart;
            m7.MarkingStar = (MarkingColor)MarkStar; m7.MarkingDiamond = (MarkingColor)MarkDiamond;
        }
        else if (Entity is IAppliedMarkings4 m4)
        {
            m4.MarkingCircle = MarkCircle != 0; m4.MarkingTriangle = MarkTriangle != 0;
            m4.MarkingSquare = MarkSquare != 0; m4.MarkingHeart = MarkHeart != 0;
            m4.MarkingStar = MarkStar != 0; m4.MarkingDiamond = MarkDiamond != 0;
        }
        else if (Entity is IAppliedMarkings3 m3)
        {
            m3.MarkingCircle = MarkCircle != 0; m3.MarkingTriangle = MarkTriangle != 0;
            m3.MarkingSquare = MarkSquare != 0; m3.MarkingHeart = MarkHeart != 0;
        }
    }

    [RelayCommand] private void CycleMarkCircle() => CycleMarking(ref _markCircle, nameof(MarkCircle), nameof(MarkCircleColor), nameof(MarkCircleTip));
    [RelayCommand] private void CycleMarkTriangle() => CycleMarking(ref _markTriangle, nameof(MarkTriangle), nameof(MarkTriangleColor), nameof(MarkTriangleTip));
    [RelayCommand] private void CycleMarkSquare() => CycleMarking(ref _markSquare, nameof(MarkSquare), nameof(MarkSquareColor), nameof(MarkSquareTip));
    [RelayCommand] private void CycleMarkHeart() => CycleMarking(ref _markHeart, nameof(MarkHeart), nameof(MarkHeartColor), nameof(MarkHeartTip));
    [RelayCommand] private void CycleMarkStar() => CycleMarking(ref _markStar, nameof(MarkStar), nameof(MarkStarColor), nameof(MarkStarTip));
    [RelayCommand] private void CycleMarkDiamond() => CycleMarking(ref _markDiamond, nameof(MarkDiamond), nameof(MarkDiamondColor), nameof(MarkDiamondTip));

    // Cosmetic — Contest Stats
    [ObservableProperty] private int _contestCool;
    [ObservableProperty] private int _contestBeauty;
    [ObservableProperty] private int _contestCute;
    [ObservableProperty] private int _contestSmart;
    [ObservableProperty] private int _contestTough;
    [ObservableProperty] private int _contestSheen;

    partial void OnContestCoolChanged(int value) { if (!_isPopulating && Entity is IContestStats cs) cs.ContestCool = (byte)Math.Clamp(value, 0, 255); }
    partial void OnContestBeautyChanged(int value) { if (!_isPopulating && Entity is IContestStats cs) cs.ContestBeauty = (byte)Math.Clamp(value, 0, 255); }
    partial void OnContestCuteChanged(int value) { if (!_isPopulating && Entity is IContestStats cs) cs.ContestCute = (byte)Math.Clamp(value, 0, 255); }
    partial void OnContestSmartChanged(int value) { if (!_isPopulating && Entity is IContestStats cs) cs.ContestSmart = (byte)Math.Clamp(value, 0, 255); }
    partial void OnContestToughChanged(int value) { if (!_isPopulating && Entity is IContestStats cs) cs.ContestTough = (byte)Math.Clamp(value, 0, 255); }
    partial void OnContestSheenChanged(int value) { if (!_isPopulating && Entity is IContestStats cs) cs.ContestSheen = (byte)Math.Clamp(value, 0, 255); }

    // Cosmetic — Favorite
    [ObservableProperty] private bool _isFavorite;

    partial void OnIsFavoriteChanged(bool value) { if (!_isPopulating && Entity is IFavorite fav) fav.IsFavorite = value; }

    // Cosmetic — visibility helpers
    [ObservableProperty] private bool _hasMarkings;
    [ObservableProperty] private bool _hasContestStats;
    [ObservableProperty] private bool _hasFavorite;

    // Origin Mark indicator
    [ObservableProperty] private string _originMarkText = string.Empty;
    [ObservableProperty] private bool _hasOriginMark;

    // Shiny/Cured mark indicators (Cosmetic tab)
    [ObservableProperty] private bool _showShinyMark;
    [ObservableProperty] private bool _showCuredMark;

    // Affixed ribbon display
    [ObservableProperty] private string _affixedRibbonText = string.Empty;
    [ObservableProperty] private bool _hasAffixedRibbon;

    // Battle Version mark
    [ObservableProperty] private string _battleVersionMarkText = string.Empty;
    [ObservableProperty] private bool _showBattleVersionMark;

    // Gen-specific: Shadow Pokemon (XD/Colosseum)
    [ObservableProperty] private int _shadowId;
    [ObservableProperty] private int _purification;
    [ObservableProperty] private bool _isShadow;
    [ObservableProperty] private bool _hasShadow;

    partial void OnShadowIdChanged(int value) { if (!_isPopulating && Entity is IShadowCapture sc) { sc.ShadowID = (ushort)Math.Clamp(value, 0, 65535); UpdateLegality(); } }
    partial void OnPurificationChanged(int value) { if (!_isPopulating && Entity is IShadowCapture sc) { sc.Purification = value; UpdateLegality(); } }

    // Form Argument
    [ObservableProperty] private uint _formArgument;
    [ObservableProperty] private bool _hasFormArgument;

    partial void OnFormArgumentChanged(uint value) { if (!_isPopulating && Entity is IFormArgument fa) { fa.FormArgument = value; UpdateLegality(); } }
    [ObservableProperty] private uint _formArgumentMax;

    // Alpha Mastered Move (Gen 8a - Legends Arceus)
    [ObservableProperty] private ComboItem? _selectedAlphaMove;
    [ObservableProperty] private bool _hasAlphaMove;

    partial void OnSelectedAlphaMoveChanged(ComboItem? value) { if (!_isPopulating && Entity is PA8 pa8 && value is not null) { pa8.AlphaMove = (ushort)value.Value; UpdateLegality(); } }

    // Move Shop / Tech Record / Plus Record visibility
    [ObservableProperty] private bool _hasMoveShop;
    [ObservableProperty] private bool _hasTechRecords;
    [ObservableProperty] private bool _hasPlusRecord;

    // Super Training (Medals) visibility — Gen 6
    [ObservableProperty] private bool _hasSuperTraining;

    // Gen-specific: Catch Rate (Gen 1)
    [ObservableProperty] private int _catchRate;
    [ObservableProperty] private bool _hasCatchRate;

    partial void OnCatchRateChanged(int value) { if (!_isPopulating && Entity is PK1 pk1) { pk1.CatchRate = (byte)Math.Clamp(value, 0, 255); UpdateLegality(); } }

    // Gen-specific: N's Sparkle (Gen 5)
    [ObservableProperty] private bool _nSparkle;
    [ObservableProperty] private bool _hasNSparkle;

    partial void OnNSparkleChanged(bool value) { if (!_isPopulating && Entity is PK5 pk5) { pk5.NSparkle = value; UpdateLegality(); } }

    // Met — Encounter Type / Ground Tile (Gen 4-6)
    [ObservableProperty] private int _encounterType;
    [ObservableProperty] private bool _hasEncounterType;
    [ObservableProperty] private IReadOnlyList<ComboItem> _encounterTypeList = Array.Empty<ComboItem>();
    [ObservableProperty] private ComboItem? _selectedEncounterType;

    // Met — Time of Day (Gen 2)
    [ObservableProperty] private int _metTimeOfDay;
    [ObservableProperty] private bool _hasMetTimeOfDay;

    partial void OnMetTimeOfDayChanged(int value) { if (!_isPopulating && Entity is ICaughtData2 cd2) { cd2.MetTimeOfDay = Math.Clamp(value, 0, 3); UpdateLegality(); } }

    // Met — As Egg toggle
    [ObservableProperty] private bool _metAsEgg;

    // Cosmetic — Shiny Leaf (Gen 4)
    [ObservableProperty] private bool _leaf1;
    [ObservableProperty] private bool _leaf2;
    [ObservableProperty] private bool _leaf3;
    [ObservableProperty] private bool _leaf4;
    [ObservableProperty] private bool _leaf5;
    [ObservableProperty] private bool _leafCrown;
    [ObservableProperty] private bool _hasShinyLeaf;

    private void WriteShinyLeafToEntity()
    {
        if (!_isPopulating && Entity is G4PKM g4)
        {
            int leaf = 0;
            if (Leaf1) leaf |= 1; if (Leaf2) leaf |= 2; if (Leaf3) leaf |= 4;
            if (Leaf4) leaf |= 8; if (Leaf5) leaf |= 16; if (LeafCrown) leaf |= 32;
            g4.ShinyLeaf = leaf;
        }
    }
    partial void OnLeaf1Changed(bool value) => WriteShinyLeafToEntity();
    partial void OnLeaf2Changed(bool value) => WriteShinyLeafToEntity();
    partial void OnLeaf3Changed(bool value) => WriteShinyLeafToEntity();
    partial void OnLeaf4Changed(bool value) => WriteShinyLeafToEntity();
    partial void OnLeaf5Changed(bool value) => WriteShinyLeafToEntity();
    partial void OnLeafCrownChanged(bool value) => WriteShinyLeafToEntity();

    // Cosmetic — Spirit/Mood (Let's Go PB7)
    [ObservableProperty] private int _spirit7b;
    [ObservableProperty] private int _mood7b;
    [ObservableProperty] private bool _hasSpirit7b;

    partial void OnSpirit7bChanged(int value) { if (!_isPopulating && Entity is PB7 pb7) pb7.Spirit = (byte)Math.Clamp(value, 0, 255); }
    partial void OnMood7bChanged(int value) { if (!_isPopulating && Entity is PB7 pb7) pb7.Mood = (byte)Math.Clamp(value, 0, 255); }

    // Cosmetic — PokeStarFame (Gen 5)
    [ObservableProperty] private int _pokeStarFame;
    [ObservableProperty] private bool _hasPokeStarFame;

    partial void OnPokeStarFameChanged(int value) { if (!_isPopulating && Entity is PK5 pk5) pk5.PokeStarFame = (byte)Math.Clamp(value, 0, 255); }

    // Cosmetic — Walking Mood (Gen 4 HG/SS)
    [ObservableProperty] private int _walkingMood;
    [ObservableProperty] private bool _hasWalkingMood;

    partial void OnWalkingMoodChanged(int value) { if (!_isPopulating && Entity is G4PKM g4) g4.WalkingMood = (sbyte)Math.Clamp(value, -127, 127); }

    // OT/Misc — Received Date (Let's Go PB7)
    [ObservableProperty] private int _receivedYear;
    [ObservableProperty] private int _receivedMonth;
    [ObservableProperty] private int _receivedDay;
    [ObservableProperty] private bool _hasReceivedDate;

    partial void OnReceivedYearChanged(int value) { if (!_isPopulating && Entity is PB7 pb7) pb7.ReceivedYear = (byte)Math.Clamp(value, 0, 255); }
    partial void OnReceivedMonthChanged(int value) { if (!_isPopulating && Entity is PB7 pb7) pb7.ReceivedMonth = (byte)Math.Clamp(value, 1, 12); }
    partial void OnReceivedDayChanged(int value) { if (!_isPopulating && Entity is PB7 pb7) pb7.ReceivedDay = (byte)Math.Clamp(value, 1, 31); }

    // OT
    [ObservableProperty] private string _ot = string.Empty;
    [ObservableProperty] private ushort _tid;
    [ObservableProperty] private ushort _sid;

    partial void OnOtChanged(string value) { if (!_isPopulating && Entity is not null) { Entity.OriginalTrainerName = value; UpdateLegality(); } }
    partial void OnTidChanged(ushort value) { if (!_isPopulating && Entity is not null) { Entity.TID16 = value; UpdateLegality(); } }
    partial void OnSidChanged(ushort value) { if (!_isPopulating && Entity is not null) { Entity.SID16 = value; UpdateLegality(); } }

    // OT Gender
    [ObservableProperty] private byte _otGender;

    public string OtGenderSymbol => OtGender switch
    {
        0 => "\u2642",  // ♂
        1 => "\u2640",  // ♀
        _ => "\u2014",  // —
    };

    partial void OnOtGenderChanged(byte value)
    {
        OnPropertyChanged(nameof(OtGenderSymbol));
        if (!_isPopulating && Entity is not null)
            Entity.OriginalTrainerGender = value;
        UpdateLegality();
    }

    [RelayCommand]
    private void ToggleOtGender()
    {
        OtGender = (byte)(OtGender == 0 ? 1 : 0);
    }

    // Handling Trainer
    [ObservableProperty] private int _currentHandler;
    [ObservableProperty] private string _handlingTrainerName = string.Empty;

    partial void OnCurrentHandlerChanged(int value) { if (!_isPopulating && Entity is not null) { Entity.CurrentHandler = (byte)value; UpdateLegality(); } }
    [ObservableProperty] private byte _htGender;

    partial void OnHandlingTrainerNameChanged(string value)
    {
        HasHandler = !string.IsNullOrEmpty(value);
        if (!_isPopulating && Entity is not null)
            Entity.HandlingTrainerName = value;
        UpdateLegality();
    }

    public string HtGenderSymbol => HtGender switch
    {
        0 => "\u2642",  // ♂
        1 => "\u2640",  // ♀
        _ => "\u2014",  // —
    };

    partial void OnHtGenderChanged(byte value)
    {
        OnPropertyChanged(nameof(HtGenderSymbol));
        if (!_isPopulating && Entity is not null)
            Entity.HandlingTrainerGender = value;
        UpdateLegality();
    }

    [RelayCommand]
    private void ToggleHtGender()
    {
        if (Entity is null) return;
        HtGender = (byte)((HtGender + 1) % 3);
        OnPropertyChanged(nameof(HtGenderSymbol));
    }

    [ObservableProperty] private bool _hasHandler;

    // Encryption Constant
    [ObservableProperty] private string _encryptionConstantHex = "00000000";

    partial void OnEncryptionConstantHexChanged(string value)
    {
        if (!_isPopulating && Entity is not null && uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out var ec))
            Entity.EncryptionConstant = ec;
        UpdateLegality();
    }

    [RelayCommand]
    private void RerollEc()
    {
        var ec = PKHeX.Core.Util.Rand32();
        EncryptionConstantHex = ec.ToString("X8");
    }

    [RelayCommand]
    private void Shinytize()
    {
        if (Entity is null) return;
        if (Entity.IsShiny)
        {
            Entity.SetPIDGender(Entity.Gender);
        }
        else
        {
            Entity.SetShiny(); // Random type
        }
        RefreshAfterShinyChange();
    }

    [RelayCommand]
    private void ShinytizeStar()
    {
        if (Entity is null) return;
        Entity.SetShinySID(Shiny.AlwaysStar);
        RefreshAfterShinyChange();
    }

    [RelayCommand]
    private void ShinytizeSquare()
    {
        if (Entity is null) return;
        Entity.SetShinySID(Shiny.AlwaysSquare);
        RefreshAfterShinyChange();
    }

    private void RefreshAfterShinyChange()
    {
        _isPopulating = true;
        try
        {
            PidHex = Entity!.PID.ToString("X8");
            EncryptionConstantHex = Entity.EncryptionConstant.ToString("X8");
            IsShiny = Entity.IsShiny;
            IsShinyDisplay = Entity.IsShiny;
            IsSquareShiny = Entity.IsShiny && Entity.ShinyXor == 0;
            ShowShinyMark = Entity.IsShiny;
        }
        finally { _isPopulating = false; }
        UpdateSprite();
        UpdateLegality();
    }

    [RelayCommand]
    private void RerollPid()
    {
        if (Entity is null) return;
        Entity.PID = PKHeX.Core.Util.Rand32();
        _isPopulating = true;
        try
        {
            PidHex = Entity.PID.ToString("X8");
            EncryptionConstantHex = Entity.EncryptionConstant.ToString("X8");
            IsShiny = Entity.IsShiny;
            IsShinyDisplay = Entity.IsShiny;
            IsSquareShiny = Entity.IsShiny && Entity.ShinyXor == 0;
            ShowShinyMark = Entity.IsShiny;
        }
        finally { _isPopulating = false; }
        UpdateSprite();
        UpdateLegality();
    }

    // Home Tracker
    [ObservableProperty] private string _homeTrackerHex = "0000000000000000";
    [ObservableProperty] private bool _hasHomeTracker;

    partial void OnHomeTrackerHexChanged(string value)
    {
        if (!_isPopulating && Entity is IHomeTrack ht && ulong.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out var tracker))
            ht.Tracker = tracker;
        UpdateLegality();
    }

    // Extra Bytes
    [ObservableProperty] private IReadOnlyList<string> _extraByteOffsets = Array.Empty<string>();
    [ObservableProperty] private string? _selectedExtraByteOffset;
    [ObservableProperty] private int _extraByteValue;
    [ObservableProperty] private bool _hasExtraBytes;

    partial void OnSelectedExtraByteOffsetChanged(string? value)
    {
        if (value is null || Entity is null) return;
        if (ushort.TryParse(value.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber, null, out var offset) && offset < Entity.Data.Length)
        {
            _isPopulating = true;
            try { ExtraByteValue = Entity.Data[offset]; }
            finally { _isPopulating = false; }
        }
    }

    partial void OnExtraByteValueChanged(int value)
    {
        if (_isPopulating || SelectedExtraByteOffset is null || Entity is null) return;
        if (ushort.TryParse(SelectedExtraByteOffset.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber, null, out var offset) && offset < Entity.Data.Length)
            Entity.Data[offset] = (byte)Math.Clamp(value, 0, 255);
        UpdateLegality();
    }

    // Met — Fateful Encounter
    [ObservableProperty] private bool _fatefulEncounter;

    partial void OnFatefulEncounterChanged(bool value) { if (!_isPopulating && Entity is not null) { Entity.FatefulEncounter = value; UpdateLegality(); } }

    // Met — Obedience Level (Gen 9)
    [ObservableProperty] private byte _obedienceLevel;
    [ObservableProperty] private bool _hasObedienceLevel;

    partial void OnObedienceLevelChanged(byte value) { if (!_isPopulating && Entity is IObedienceLevel ol) { ol.ObedienceLevel = value; UpdateLegality(); } }

    // Met — Battle Version (Gen 8+)
    [ObservableProperty] private int _battleVersion;
    [ObservableProperty] private bool _hasBattleVersion;
    [ObservableProperty] private ComboItem? _selectedBattleVersion;

    partial void OnSelectedBattleVersionChanged(ComboItem? value)
    {
        if (value is not null)
            BattleVersion = value.Value;
        if (!_isPopulating && Entity is not null)
        {
            if (Entity is IBattleVersion bv)
                bv.BattleVersion = (GameVersion)BattleVersion;
            UpdateLegality();
        }
    }

    partial void OnSelectedEncounterTypeChanged(ComboItem? value)
    {
        if (value is not null)
            EncounterType = value.Value;
        if (!_isPopulating && Entity is not null)
        {
            if (Entity is IGroundTile gt)
                gt.GroundTile = (GroundTileType)EncounterType;
            UpdateLegality();
        }
    }

    // Awakening Values (Let's Go PB7)
    [ObservableProperty] private int _avHp, _avAtk, _avDef, _avSpa, _avSpd, _avSpe;
    [ObservableProperty] private bool _hasAwakeningValues;

    partial void OnAvHpChanged(int value)  { if (!_isPopulating && Entity is IAwakened aw) { aw.AV_HP  = (byte)Math.Clamp(value, 0, 200); RecalcStats(); }  UpdateLegality(); }
    partial void OnAvAtkChanged(int value) { if (!_isPopulating && Entity is IAwakened aw) { aw.AV_ATK = (byte)Math.Clamp(value, 0, 200); RecalcStats(); }  UpdateLegality(); }
    partial void OnAvDefChanged(int value) { if (!_isPopulating && Entity is IAwakened aw) { aw.AV_DEF = (byte)Math.Clamp(value, 0, 200); RecalcStats(); }  UpdateLegality(); }
    partial void OnAvSpaChanged(int value) { if (!_isPopulating && Entity is IAwakened aw) { aw.AV_SPA = (byte)Math.Clamp(value, 0, 200); RecalcStats(); }  UpdateLegality(); }
    partial void OnAvSpdChanged(int value) { if (!_isPopulating && Entity is IAwakened aw) { aw.AV_SPD = (byte)Math.Clamp(value, 0, 200); RecalcStats(); }  UpdateLegality(); }
    partial void OnAvSpeChanged(int value) { if (!_isPopulating && Entity is IAwakened aw) { aw.AV_SPE = (byte)Math.Clamp(value, 0, 200); RecalcStats(); }  UpdateLegality(); }

    // Ganbaru Values (Legends Arceus PA8)
    [ObservableProperty] private int _gvHp, _gvAtk, _gvDef, _gvSpa, _gvSpd, _gvSpe;
    [ObservableProperty] private bool _hasGanbaruValues;

    partial void OnGvHpChanged(int value)  { if (!_isPopulating && Entity is IGanbaru gv) { gv.GV_HP  = (byte)Math.Clamp(value, 0, 10); RecalcStats(); }  UpdateLegality(); }
    partial void OnGvAtkChanged(int value) { if (!_isPopulating && Entity is IGanbaru gv) { gv.GV_ATK = (byte)Math.Clamp(value, 0, 10); RecalcStats(); }  UpdateLegality(); }
    partial void OnGvDefChanged(int value) { if (!_isPopulating && Entity is IGanbaru gv) { gv.GV_DEF = (byte)Math.Clamp(value, 0, 10); RecalcStats(); }  UpdateLegality(); }
    partial void OnGvSpaChanged(int value) { if (!_isPopulating && Entity is IGanbaru gv) { gv.GV_SPA = (byte)Math.Clamp(value, 0, 10); RecalcStats(); }  UpdateLegality(); }
    partial void OnGvSpdChanged(int value) { if (!_isPopulating && Entity is IGanbaru gv) { gv.GV_SPD = (byte)Math.Clamp(value, 0, 10); RecalcStats(); }  UpdateLegality(); }
    partial void OnGvSpeChanged(int value) { if (!_isPopulating && Entity is IGanbaru gv) { gv.GV_SPE = (byte)Math.Clamp(value, 0, 10); RecalcStats(); }  UpdateLegality(); }

    // Region Origin (Gen 6-7, 3DS)
    [ObservableProperty] private int _country, _subRegion, _consoleRegion;
    [ObservableProperty] private string _countryName = string.Empty;
    [ObservableProperty] private string _subRegionName = string.Empty;
    [ObservableProperty] private string _consoleRegionName = string.Empty;

    partial void OnCountryChanged(int value)
    {
        if (!_isPopulating)
        {
            UpdateRegionNames();
            if (Entity is IRegionOrigin ro)
                ro.Country = (byte)Math.Clamp(value, 0, 255);
            UpdateLegality();
        }
    }

    partial void OnSubRegionChanged(int value)
    {
        if (!_isPopulating)
        {
            UpdateRegionNames();
            if (Entity is IRegionOrigin ro)
                ro.Region = (byte)Math.Clamp(value, 0, 255);
            UpdateLegality();
        }
    }

    partial void OnConsoleRegionChanged(int value)
    {
        if (!_isPopulating)
        {
            UpdateRegionNames();
            if (Entity is IRegionOrigin ro)
                ro.ConsoleRegion = (byte)Math.Clamp(value, 0, 255);
            UpdateLegality();
        }
    }

    private void UpdateRegionNames()
    {
        try
        {
            var lang = GameInfo.CurrentLanguage;
            CountryName = GeoLocation.GetCountryName(lang, (byte)Country);
            SubRegionName = GeoLocation.GetRegionName(lang, (byte)Country, (byte)SubRegion);
            var consoleRegionNames = new[] { "Japan", "Americas", "Europe", "China", "Korea", "Taiwan" };
            ConsoleRegionName = ConsoleRegion >= 0 && ConsoleRegion < consoleRegionNames.Length ? consoleRegionNames[ConsoleRegion] : $"{ConsoleRegion}";
        }
        catch
        {
            CountryName = string.Empty;
            SubRegionName = string.Empty;
            ConsoleRegionName = string.Empty;
        }
    }
    [ObservableProperty] private bool _hasRegionData;

    // Handler Language
    [ObservableProperty] private int _htLanguage;
    [ObservableProperty] private bool _hasHtLanguage;

    partial void OnHtLanguageChanged(int value) { if (!_isPopulating && Entity is IHandlerLanguage hl) hl.HandlingTrainerLanguage = (byte)Math.Clamp(value, 0, 255);  UpdateLegality(); }

    // Cosmetic — Size/Scale
    [ObservableProperty] private int _heightScalar;
    [ObservableProperty] private int _weightScalar;
    [ObservableProperty] private int _scale;
    [ObservableProperty] private bool _hasSizeData;
    [ObservableProperty] private bool _hasScale;

    partial void OnHeightScalarChanged(int value) { if (!_isPopulating && Entity is IScaledSize ss) ss.HeightScalar = (byte)Math.Clamp(value, 0, 255); }
    partial void OnWeightScalarChanged(int value) { if (!_isPopulating && Entity is IScaledSize ss) ss.WeightScalar = (byte)Math.Clamp(value, 0, 255); }
    partial void OnScaleChanged(int value) { if (!_isPopulating && Entity is IScaledSize3 ss3) ss3.Scale = (byte)Math.Clamp(value, 0, 255); }

    [ObservableProperty]
    private bool _isInitialized;

    // Legality report text (populated by context menu command)
    [ObservableProperty] private string _legalityReport = string.Empty;

    // ComboBox data sources — sourced from GameInfo.FilteredSources
    public IReadOnlyList<ComboItem> SpeciesList => GameInfo.FilteredSources.Species;
    public IReadOnlyList<ComboItem> NatureList => GameInfo.FilteredSources.Natures;
    public IReadOnlyList<ComboItem> HeldItemList => GameInfo.FilteredSources.Items;
    public IReadOnlyList<ComboItem> MoveList => GameInfo.FilteredSources.Moves;
    [ObservableProperty] private IReadOnlyList<ComboItem> _abilityList = Array.Empty<ComboItem>();
    public IReadOnlyList<ComboItem> LanguageList => GameInfo.FilteredSources.Languages;
    public IReadOnlyList<ComboItem> BallList => GameInfo.FilteredSources.Balls;
    public IReadOnlyList<ComboItem> VersionList => GameInfo.FilteredSources.Games;

    // Location lists are version/context-dependent and refreshed on populate
    [ObservableProperty] private IReadOnlyList<ComboItem> _metLocationList = Array.Empty<ComboItem>();
    [ObservableProperty] private IReadOnlyList<ComboItem> _eggLocationList = Array.Empty<ComboItem>();

    // Form list — species-dependent, refreshed when species changes
    [ObservableProperty] private IReadOnlyList<ComboItem> _formList = Array.Empty<ComboItem>();

    // Selected ComboItem bindings
    [ObservableProperty] private ComboItem? _selectedSpecies;
    [ObservableProperty] private ComboItem? _selectedNature;
    [ObservableProperty] private ComboItem? _selectedHeldItem;
    [ObservableProperty] private ComboItem? _selectedMove1;
    [ObservableProperty] private ComboItem? _selectedMove2;
    [ObservableProperty] private ComboItem? _selectedMove3;
    [ObservableProperty] private ComboItem? _selectedMove4;
    [ObservableProperty] private ComboItem? _selectedAbility;
    [ObservableProperty] private ComboItem? _selectedLanguage;
    [ObservableProperty] private ComboItem? _selectedBall;
    [ObservableProperty] private ComboItem? _selectedVersion;
    [ObservableProperty] private ComboItem? _selectedMetLocation;
    [ObservableProperty] private ComboItem? _selectedEggLocation;
    [ObservableProperty] private ComboItem? _selectedForm;

    partial void OnSelectedFormChanged(ComboItem? value)
    {
        if (value is not null)
            Form = (byte)value.Value;
        if (!_isPopulating && Entity is not null)
        {
            Entity.Form = Form;

            // Recalc EXP for potential growth rate change (some forms have different growth)
            if (Entity.PersonalInfo is { } pi)
            {
                var growth = pi.EXPGrowth;
                Entity.EXP = Experience.GetEXP(Level, growth);
                _isPopulating = true;
                try { Exp = Entity.EXP; }
                finally { _isPopulating = false; }

                // Refresh base stats display
                Base_HP = pi.HP;
                Base_ATK = pi.ATK;
                Base_DEF = pi.DEF;
                Base_SPA = pi.SPA;
                Base_SPD = pi.SPD;
                Base_SPE = pi.SPE;
                BaseST = pi.HP + pi.ATK + pi.DEF + pi.SPA + pi.SPD + pi.SPE;
            }

            // Special Unown handling: Gen 3 PID must match form
            if (Entity.Species == (int)PKHeX.Core.Species.Unown && Entity is G3PKM)
                Entity.SetPIDUnown3(Entity.Form);

            // Sanitize gender
            Entity.Gender = Entity.GetSaneGender();
            _isPopulating = true;
            try { Gender = Entity.Gender; }
            finally { _isPopulating = false; }

            // Refresh form arguments
            if (Entity is IFormArgument fa)
            {
                HasFormArgument = true;
                FormArgument = fa.FormArgument;
                FormArgumentMax = FormArgumentUtil.GetFormArgumentMax(Entity.Species, Entity.Form, Entity.Context);
                if (FormArgumentMax == 0)
                    FormArgumentMax = 255;
            }

            // Refresh ability list (some forms have different abilities)
            RefreshAbilityList();

            // Recalculate stats
            RecalcStats();

            UpdateSprite();
            UpdateLegality();
        }
    }

    partial void OnSelectedSpeciesChanged(ComboItem? value)
    {
        if (value is not null)
            Species = (ushort)value.Value;
        if (!_isPopulating)
        {
            // Write species to Entity so downstream lookups use the new value
            if (Entity is not null)
                Entity.Species = Species;

            // Refresh form list for the new species
            RefreshFormList();
            if (FormList.Count > 0)
                SelectedForm = FormList[0];

            // Auto-update nickname when species changes and the pokemon is not nicknamed
            if (!IsNicknamed && Entity is not null)
            {
                var speciesName = SpeciesName.GetSpeciesNameGeneration(Entity.Species, Entity.Language, Entity.Format);
                _isPopulating = true;
                try { Nickname = speciesName; Entity.Nickname = speciesName; }
                finally { _isPopulating = false; }
            }

            // Recalculate EXP for new growth rate
            if (Entity?.PersonalInfo is { } pi)
            {
                var growth = pi.EXPGrowth;
                var minExp = Experience.GetEXP(Level, growth);
                if (Entity.EXP != minExp)
                {
                    Entity.EXP = minExp;
                    _isPopulating = true;
                    try { Exp = minExp; }
                    finally { _isPopulating = false; }
                }
            }

            // Sanitize gender for new species
            if (Entity is not null)
            {
                Entity.Gender = Entity.GetSaneGender();
                _isPopulating = true;
                try { Gender = Entity.Gender; }
                finally { _isPopulating = false; }
            }

            // Refresh ability list for new species
            RefreshAbilityList();

            OnPropertyChanged(nameof(SpeciesTooltip));
            UpdateSprite();
            UpdateLegality();
        }
    }

    partial void OnSelectedNatureChanged(ComboItem? value)
    {
        if (value is not null)
            Nature = (Nature)value.Value;
        if (!_isPopulating && Entity is not null)
        {
            Entity.Nature = Nature;
            OnPropertyChanged(nameof(AtkColor));
            OnPropertyChanged(nameof(DefColor));
            OnPropertyChanged(nameof(SpAColor));
            OnPropertyChanged(nameof(SpDColor));
            OnPropertyChanged(nameof(SpeColor));
            OnPropertyChanged(nameof(NatureTooltip));
            RecalcStats();
            UpdateLegality();
        }
    }

    partial void OnSelectedHeldItemChanged(ComboItem? value)
    {
        if (value is not null)
            HeldItem = value.Value;
        if (!_isPopulating && Entity is not null)
        {
            Entity.HeldItem = HeldItem;
            UpdateLegality();
        }
    }

    partial void OnSelectedMove1Changed(ComboItem? value)
    {
        if (value is not null)
            Move1 = (ushort)value.Value;
        if (!_isPopulating && Entity is not null)
        {
            Entity.Move1 = Move1;
            Entity.Move1_PP = Entity.GetMovePP(Move1, Move1_PPUps);
            Move1_PP = Entity.Move1_PP;
            UpdateLegality();
        }
    }

    partial void OnSelectedMove2Changed(ComboItem? value)
    {
        if (value is not null)
            Move2 = (ushort)value.Value;
        if (!_isPopulating && Entity is not null)
        {
            Entity.Move2 = Move2;
            Entity.Move2_PP = Entity.GetMovePP(Move2, Move2_PPUps);
            Move2_PP = Entity.Move2_PP;
            UpdateLegality();
        }
    }

    partial void OnSelectedMove3Changed(ComboItem? value)
    {
        if (value is not null)
            Move3 = (ushort)value.Value;
        if (!_isPopulating && Entity is not null)
        {
            Entity.Move3 = Move3;
            Entity.Move3_PP = Entity.GetMovePP(Move3, Move3_PPUps);
            Move3_PP = Entity.Move3_PP;
            UpdateLegality();
        }
    }

    partial void OnSelectedMove4Changed(ComboItem? value)
    {
        if (value is not null)
            Move4 = (ushort)value.Value;
        if (!_isPopulating && Entity is not null)
        {
            Entity.Move4 = Move4;
            Entity.Move4_PP = Entity.GetMovePP(Move4, Move4_PPUps);
            Move4_PP = Entity.Move4_PP;
            UpdateLegality();
        }
    }

    partial void OnMove1_PPUpsChanged(int value)
    {
        if (!_isPopulating && Entity is not null)
        {
            Entity.Move1_PPUps = value;
            var pp = Entity.GetMovePP(Move1, value);
            Move1_PP = pp;
            Entity.Move1_PP = pp;
            UpdateLegality();
        }
    }

    partial void OnMove2_PPUpsChanged(int value)
    {
        if (!_isPopulating && Entity is not null)
        {
            Entity.Move2_PPUps = value;
            var pp = Entity.GetMovePP(Move2, value);
            Move2_PP = pp;
            Entity.Move2_PP = pp;
            UpdateLegality();
        }
    }

    partial void OnMove3_PPUpsChanged(int value)
    {
        if (!_isPopulating && Entity is not null)
        {
            Entity.Move3_PPUps = value;
            var pp = Entity.GetMovePP(Move3, value);
            Move3_PP = pp;
            Entity.Move3_PP = pp;
            UpdateLegality();
        }
    }

    partial void OnMove4_PPUpsChanged(int value)
    {
        if (!_isPopulating && Entity is not null)
        {
            Entity.Move4_PPUps = value;
            var pp = Entity.GetMovePP(Move4, value);
            Move4_PP = pp;
            Entity.Move4_PP = pp;
            UpdateLegality();
        }
    }

    partial void OnSelectedAbilityChanged(ComboItem? value)
    {
        if (value is not null)
            Ability = value.Value;
        if (!_isPopulating && Entity is not null)
        {
            Entity.Ability = Ability;
            UpdateLegality();
        }
    }

    partial void OnSelectedLanguageChanged(ComboItem? value)
    {
        if (value is not null)
            Language = value.Value;
        if (!_isPopulating && Entity is not null)
        {
            Entity.Language = Language;
            UpdateLegality();
        }
    }

    partial void OnSelectedBallChanged(ComboItem? value)
    {
        if (value is not null)
            Ball = (byte)value.Value;
        if (!_isPopulating && Entity is not null)
        {
            Entity.Ball = Ball;
            UpdateBallSprite();
            UpdateLegality();
        }
    }

    private void UpdateBallSprite()
    {
        try
        {
            var skBitmap = SpriteUtil.GetBallSprite(Ball);
            var old = BallSprite;
            BallSprite = SKBitmapToAvaloniaBitmapConverter.ToAvaloniaBitmapAndDispose(skBitmap);
            old?.Dispose();
        }
        catch
        {
            var old = BallSprite;
            BallSprite = null;
            old?.Dispose();
        }
    }

    partial void OnSelectedVersionChanged(ComboItem? value)
    {
        if (value is not null)
            Version = value.Value;
        if (!_isPopulating && Entity is not null)
        {
            Entity.Version = (GameVersion)Version;
            RefreshLocationLists();
            UpdateLegality();
        }
    }

    partial void OnSelectedMetLocationChanged(ComboItem? value)
    {
        if (value is not null)
            MetLocation = (ushort)value.Value;
        if (!_isPopulating && Entity is not null)
        {
            Entity.MetLocation = MetLocation;
            UpdateLegality();
        }
    }

    partial void OnSelectedEggLocationChanged(ComboItem? value)
    {
        if (value is not null)
            EggLocation = (ushort)value.Value;
        if (!_isPopulating && Entity is not null)
        {
            Entity.EggLocation = EggLocation;
            UpdateLegality();
        }
    }

    private void RefreshLocationLists()
    {
        if (Entity is null)
            return;
        var ver = (GameVersion)Version;
        var ctx = Entity.Context;
        MetLocationList = GameInfo.GetLocationList(ver, ctx, egg: false);
        EggLocationList = GameInfo.GetLocationList(ver, ctx, egg: true);
    }

    private void RefreshFormList()
    {
        if (Entity is null)
            return;
        var strings = GameInfo.Strings;
        var formNames = FormConverter.GetFormList(Species, strings.Types, strings.forms, Entity.Context);
        var items = new ComboItem[formNames.Length];
        for (int i = 0; i < formNames.Length; i++)
        {
            var name = string.IsNullOrEmpty(formNames[i]) ? $"Form {i}" : formNames[i];
            items[i] = new ComboItem(name, i);
        }
        FormList = items;
    }

    private void RefreshAbilityList()
    {
        if (Entity is null)
        {
            AbilityList = GameInfo.FilteredSources.Abilities;
            return;
        }
        var pi = Entity.PersonalInfo;
        AbilityList = GameInfo.FilteredSources.GetAbilityList(pi);
        var current = AbilityList.FirstOrDefault(a => a.Value == Entity.Ability);
        SelectedAbility = current ?? AbilityList.FirstOrDefault();
    }

    public void Initialize(SaveFile sav)
    {
        _sav = sav;
        // Update FilteredSources with the real save file's limits
        GameInfo.FilteredSources = new FilteredGameDataSource(sav, GameInfo.Sources);
        IsInitialized = true;
        NotifyListsChanged();
    }

    /// <summary>
    /// Raises property-changed for every combo-box list property so the UI rebinds
    /// after a language change or save-file load.
    /// </summary>
    public void NotifyListsChanged()
    {
        OnPropertyChanged(nameof(SpeciesList));
        OnPropertyChanged(nameof(NatureList));
        OnPropertyChanged(nameof(HeldItemList));
        OnPropertyChanged(nameof(MoveList));
        RefreshAbilityList();
        OnPropertyChanged(nameof(LanguageList));
        OnPropertyChanged(nameof(BallList));
        OnPropertyChanged(nameof(VersionList));
    }

    public void PopulateFields(PKM pk)
    {
        _isPopulating = true;
        try
        {
        Entity = pk;

        Species = pk.Species;
        Nickname = pk.Nickname;
        Level = (byte)pk.CurrentLevel;
        Form = pk.Form;
        Gender = pk.Gender;
        Nature = pk.Nature;

        // Stat Nature (Gen 8+ have separate stat nature)
        HasStatNature = pk.Format >= 8;
        StatNature = pk.StatNature;

        Ability = pk.Ability;
        AbilityNumber = pk.AbilityNumber;
        HeldItem = pk.HeldItem;
        IsShiny = pk.IsShiny;
        IsShinyDisplay = pk.IsShiny;
        IsSquareShiny = pk.IsShiny && pk.ShinyXor == 0;
        IsEgg = pk.IsEgg;
        IsNicknamed = pk.IsNicknamed;

        // Pokerus
        IsInfected = pk.IsPokerusInfected;
        IsCured = pk.IsPokerusCured;
        PkrsStrain = pk.PokerusStrain;
        PkrsDays = pk.PokerusDays;

        // New fields
        PidHex = pk.PID.ToString("X8");
        Exp = pk.EXP;
        Friendship = pk.CurrentFriendship;
        Language = pk.Language;

        Hp = pk.Stat_HPCurrent;
        Atk = pk.Stat_ATK;
        Def = pk.Stat_DEF;
        SpA = pk.Stat_SPA;
        SpD = pk.Stat_SPD;
        Spe = pk.Stat_SPE;

        // Base stats
        var pi = pk.PersonalInfo;
        Base_HP = pi.HP;
        Base_ATK = pi.ATK;
        Base_DEF = pi.DEF;
        Base_SPA = pi.SPA;
        Base_SPD = pi.SPD;
        Base_SPE = pi.SPE;

        Iv_HP = pk.IV_HP;
        Iv_ATK = pk.IV_ATK;
        Iv_DEF = pk.IV_DEF;
        Iv_SPA = pk.IV_SPA;
        Iv_SPD = pk.IV_SPD;
        Iv_SPE = pk.IV_SPE;

        Ev_HP = pk.EV_HP;
        Ev_ATK = pk.EV_ATK;
        Ev_DEF = pk.EV_DEF;
        Ev_SPA = pk.EV_SPA;
        Ev_SPD = pk.EV_SPD;
        Ev_SPE = pk.EV_SPE;

        // Awakening Values (Let's Go)
        if (pk is IAwakened aw)
        {
            HasAwakeningValues = true;
            AvHp = aw.AV_HP; AvAtk = aw.AV_ATK; AvDef = aw.AV_DEF;
            AvSpa = aw.AV_SPA; AvSpd = aw.AV_SPD; AvSpe = aw.AV_SPE;
        }
        else
        {
            HasAwakeningValues = false;
            AvHp = AvAtk = AvDef = AvSpa = AvSpd = AvSpe = 0;
        }

        // Ganbaru Values (Legends Arceus)
        if (pk is IGanbaru gv)
        {
            HasGanbaruValues = true;
            GvHp = gv.GV_HP; GvAtk = gv.GV_ATK; GvDef = gv.GV_DEF;
            GvSpa = gv.GV_SPA; GvSpd = gv.GV_SPD; GvSpe = gv.GV_SPE;
        }
        else
        {
            HasGanbaruValues = false;
            GvHp = GvAtk = GvDef = GvSpa = GvSpd = GvSpe = 0;
        }

        Move1 = pk.Move1;
        Move2 = pk.Move2;
        Move3 = pk.Move3;
        Move4 = pk.Move4;

        // Move PP and PP Ups
        Move1_PP = pk.Move1_PP;
        Move2_PP = pk.Move2_PP;
        Move3_PP = pk.Move3_PP;
        Move4_PP = pk.Move4_PP;
        Move1_PPUps = pk.Move1_PPUps;
        Move2_PPUps = pk.Move2_PPUps;
        Move3_PPUps = pk.Move3_PPUps;
        Move4_PPUps = pk.Move4_PPUps;

        // Relearn Moves
        var hasRelearn = pk.Format >= 6;
        HasRelearnMoves = hasRelearn;
        if (hasRelearn)
        {
            RelearnMove1 = pk.RelearnMove1;
            RelearnMove2 = pk.RelearnMove2;
            RelearnMove3 = pk.RelearnMove3;
            RelearnMove4 = pk.RelearnMove4;
        }
        else
        {
            RelearnMove1 = RelearnMove2 = RelearnMove3 = RelearnMove4 = 0;
        }

        // Hyper Training
        if (pk is IHyperTrain ht)
        {
            HasHyperTraining = true;
            HtHp = ht.HT_HP;
            HtAtk = ht.HT_ATK;
            HtDef = ht.HT_DEF;
            HtSpA = ht.HT_SPA;
            HtSpD = ht.HT_SPD;
            HtSpe = ht.HT_SPE;
        }
        else
        {
            HasHyperTraining = false;
            HtHp = HtAtk = HtDef = HtSpA = HtSpD = HtSpe = false;
        }

        // Dynamax Level
        if (pk is IDynamaxLevel dl)
        {
            HasDynamaxLevel = true;
            DynamaxLevel = dl.DynamaxLevel;
        }
        else
        {
            HasDynamaxLevel = false;
            DynamaxLevel = 0;
        }

        // Gigantamax
        if (pk is IGigantamax gm)
        {
            HasGigantamax = true;
            CanGigantamax = gm.CanGigantamax;
        }
        else
        {
            HasGigantamax = false;
            CanGigantamax = false;
        }

        // Alpha (Legends Arceus)
        if (pk is IAlpha alpha)
        {
            HasAlpha = true;
            IsAlpha = alpha.IsAlpha;
        }
        else
        {
            HasAlpha = false;
            IsAlpha = false;
        }

        // Noble (Legends Arceus)
        if (pk is INoble noble)
        {
            HasNoble = true;
            IsNoble = noble.IsNoble;
        }
        else
        {
            HasNoble = false;
            IsNoble = false;
        }

        // Tera Type (Gen 9)
        if (pk is ITeraType tt)
        {
            HasTeraType = true;
            TeraTypeOriginal = (int)tt.TeraTypeOriginal;
            TeraTypeOverride = (int)tt.TeraTypeOverride;

            // Build tera type list from type names
            var teraTypes = GameInfo.Strings.Types;
            var teraList = new List<ComboItem>();
            for (int i = 0; i < teraTypes.Count; i++)
                teraList.Add(new ComboItem(teraTypes[i], i));
            TeraTypeList = teraList;
            SelectedTeraOriginal = teraList.FirstOrDefault(x => x.Value == TeraTypeOriginal);
            SelectedTeraOverride = teraList.FirstOrDefault(x => x.Value == TeraTypeOverride);
        }
        else
        {
            HasTeraType = false;
            TeraTypeOriginal = 0;
            TeraTypeOverride = 0;
            TeraTypeList = Array.Empty<ComboItem>();
            SelectedTeraOriginal = null;
            SelectedTeraOverride = null;
        }

        // EV Total
        EvTotal = pk.EV_HP + pk.EV_ATK + pk.EV_DEF + pk.EV_SPA + pk.EV_SPD + pk.EV_SPE;
        IsEvTotalValid = EvTotal <= 510;

        // Base Stat Total
        BaseST = pi.HP + pi.ATK + pi.DEF + pi.SPA + pi.SPD + pi.SPE;

        // Hidden Power Type
        Span<int> ivs = stackalloc int[6];
        pk.GetIVs(ivs);
        var hpType = HiddenPower.GetType(ivs, pk.Context);
        var typeNames = GameInfo.Strings.Types;
        HiddenPowerType = hpType + 1 < typeNames.Count ? typeNames[hpType + 1] : $"Type {hpType}";

        // Characteristic
        var charIdx = pk.Characteristic;
        if (charIdx >= 0)
        {
            var charStrings = GameInfo.Strings.characteristics;
            CharacteristicText = charIdx < charStrings.Length ? charStrings[charIdx] : string.Empty;
        }
        else
        {
            CharacteristicText = string.Empty;
        }

        // Met
        MetLocation = pk.MetLocation;
        MetLevel = pk.MetLevel;
        Ball = pk.Ball;
        EggLocation = pk.EggLocation;
        Version = (int)pk.Version;
        MetYear = pk.MetYear;
        MetMonth = pk.MetMonth;
        MetDay = pk.MetDay;
        OnPropertyChanged(nameof(MetDate));
        EggYear = pk.EggYear;
        EggMonth = pk.EggMonth;
        EggDay = pk.EggDay;
        OnPropertyChanged(nameof(EggDate));

        Ot = pk.OriginalTrainerName;
        Tid = pk.TID16;
        Sid = pk.SID16;

        // OT Gender
        OtGender = pk.OriginalTrainerGender;

        // Handling Trainer
        CurrentHandler = pk.CurrentHandler;
        HandlingTrainerName = pk.HandlingTrainerName;
        HtGender = pk.HandlingTrainerGender;
        HasHandler = !string.IsNullOrEmpty(pk.HandlingTrainerName);

        // Handler Language
        if (pk is IHandlerLanguage hl)
        {
            HasHtLanguage = true;
            HtLanguage = hl.HandlingTrainerLanguage;
        }
        else
        {
            HasHtLanguage = false;
            HtLanguage = 0;
        }

        // Region Origin (Gen 6-7)
        if (pk is IRegionOrigin ro)
        {
            HasRegionData = true;
            Country = ro.Country;
            SubRegion = ro.Region;
            ConsoleRegion = ro.ConsoleRegion;
            UpdateRegionNames();
        }
        else
        {
            HasRegionData = false;
            Country = SubRegion = ConsoleRegion = 0;
            CountryName = SubRegionName = ConsoleRegionName = string.Empty;
        }

        // Encryption Constant
        EncryptionConstantHex = pk.EncryptionConstant.ToString("X8");

        // Home Tracker
        if (pk is IHomeTrack homeTrack)
        {
            HasHomeTracker = true;
            HomeTrackerHex = homeTrack.Tracker.ToString("X16");
        }
        else
        {
            HasHomeTracker = false;
            HomeTrackerHex = "0000000000000000";
        }

        // Extra Bytes
        var extra = pk.ExtraBytes;
        if (extra.Length > 0)
        {
            HasExtraBytes = true;
            ExtraByteOffsets = extra.ToArray().Select(b => $"0x{b:X2}").ToArray();
            SelectedExtraByteOffset = ExtraByteOffsets.FirstOrDefault();
        }
        else
        {
            HasExtraBytes = false;
            ExtraByteOffsets = Array.Empty<string>();
            SelectedExtraByteOffset = null;
        }

        // Met — Fateful Encounter
        FatefulEncounter = pk.FatefulEncounter;

        // Met — Obedience Level
        if (pk is IObedienceLevel ol)
        {
            HasObedienceLevel = true;
            ObedienceLevel = ol.ObedienceLevel;
        }
        else
        {
            HasObedienceLevel = false;
            ObedienceLevel = 0;
        }

        // Met — Battle Version
        if (pk is IBattleVersion bv)
        {
            HasBattleVersion = true;
            BattleVersion = (int)bv.BattleVersion;
        }
        else
        {
            HasBattleVersion = false;
            BattleVersion = 0;
        }

        // Cosmetic — Markings
        if (pk is IAppliedMarkings7 m7)
        {
            HasMarkings = true;
            HasGen7Markings = true;
            MarkCircle = (int)m7.MarkingCircle;
            MarkTriangle = (int)m7.MarkingTriangle;
            MarkSquare = (int)m7.MarkingSquare;
            MarkHeart = (int)m7.MarkingHeart;
            MarkStar = (int)m7.MarkingStar;
            MarkDiamond = (int)m7.MarkingDiamond;
        }
        else if (pk is IAppliedMarkings4 m4)
        {
            HasMarkings = true;
            HasGen7Markings = false;
            MarkCircle = m4.MarkingCircle ? 1 : 0;
            MarkTriangle = m4.MarkingTriangle ? 1 : 0;
            MarkSquare = m4.MarkingSquare ? 1 : 0;
            MarkHeart = m4.MarkingHeart ? 1 : 0;
            MarkStar = m4.MarkingStar ? 1 : 0;
            MarkDiamond = m4.MarkingDiamond ? 1 : 0;
        }
        else if (pk is IAppliedMarkings3 m3)
        {
            HasMarkings = true;
            HasGen7Markings = false;
            MarkCircle = m3.MarkingCircle ? 1 : 0;
            MarkTriangle = m3.MarkingTriangle ? 1 : 0;
            MarkSquare = m3.MarkingSquare ? 1 : 0;
            MarkHeart = m3.MarkingHeart ? 1 : 0;
            MarkStar = 0;
            MarkDiamond = 0;
        }
        else
        {
            HasMarkings = false;
            HasGen7Markings = false;
            MarkCircle = MarkTriangle = MarkSquare = MarkHeart = MarkStar = MarkDiamond = 0;
        }
        // Notify marking color and tooltip properties
        OnPropertyChanged(nameof(MarkCircleColor));
        OnPropertyChanged(nameof(MarkTriangleColor));
        OnPropertyChanged(nameof(MarkSquareColor));
        OnPropertyChanged(nameof(MarkHeartColor));
        OnPropertyChanged(nameof(MarkStarColor));
        OnPropertyChanged(nameof(MarkDiamondColor));
        OnPropertyChanged(nameof(MarkCircleTip));
        OnPropertyChanged(nameof(MarkTriangleTip));
        OnPropertyChanged(nameof(MarkSquareTip));
        OnPropertyChanged(nameof(MarkHeartTip));
        OnPropertyChanged(nameof(MarkStarTip));
        OnPropertyChanged(nameof(MarkDiamondTip));

        // Cosmetic — Contest Stats
        if (pk is IContestStatsReadOnly cs)
        {
            HasContestStats = true;
            ContestCool = cs.ContestCool;
            ContestBeauty = cs.ContestBeauty;
            ContestCute = cs.ContestCute;
            ContestSmart = cs.ContestSmart;
            ContestTough = cs.ContestTough;
            ContestSheen = cs.ContestSheen;
        }
        else
        {
            HasContestStats = false;
            ContestCool = ContestBeauty = ContestCute = ContestSmart = ContestTough = ContestSheen = 0;
        }

        // Cosmetic — Favorite
        if (pk is IFavorite fav)
        {
            HasFavorite = true;
            IsFavorite = fav.IsFavorite;
        }
        else
        {
            HasFavorite = false;
            IsFavorite = false;
        }

        // Cosmetic — Size/Scale
        if (pk is IScaledSize ss)
        {
            HasSizeData = true;
            HeightScalar = ss.HeightScalar;
            WeightScalar = ss.WeightScalar;
            if (pk is IScaledSize3 ss3)
            {
                HasScale = true;
                Scale = ss3.Scale;
            }
            else
            {
                HasScale = false;
                Scale = 0;
            }
        }
        else
        {
            HasSizeData = false;
            HasScale = false;
            HeightScalar = 0;
            WeightScalar = 0;
            Scale = 0;
        }

        // Cosmetic — Shiny/Cured marks
        ShowShinyMark = pk.IsShiny;
        ShowCuredMark = pk.IsPokerusCured;

        // Cosmetic — Affixed Ribbon
        if (pk is IRibbonSetAffixed aff)
        {
            var idx = aff.AffixedRibbon;
            HasAffixedRibbon = idx >= 0;
            AffixedRibbonText = idx >= 0 ? ((RibbonIndex)idx).ToString() : string.Empty;
        }
        else
        {
            HasAffixedRibbon = false;
            AffixedRibbonText = string.Empty;
        }

        // Cosmetic — Battle Version mark
        if (pk is IBattleVersion bvMark)
        {
            var ver = (int)bvMark.BattleVersion;
            ShowBattleVersionMark = ver != 0;
            BattleVersionMarkText = ver != 0 ? $"Battle: v{ver}" : string.Empty;
        }
        else
        {
            ShowBattleVersionMark = false;
            BattleVersionMarkText = string.Empty;
        }

        // Gen-specific: Shadow Pokemon (XD/Colosseum)
        if (pk is IShadowCapture sc)
        {
            HasShadow = true;
            ShadowId = sc.ShadowID;
            Purification = sc.Purification;
            IsShadow = sc.IsShadow;
        }
        else
        {
            HasShadow = false;
            ShadowId = 0;
            Purification = 0;
            IsShadow = false;
        }

        // Gen-specific: Catch Rate (Gen 1)
        if (pk is PK1 pk1)
        {
            HasCatchRate = true;
            CatchRate = pk1.CatchRate;
        }
        else
        {
            HasCatchRate = false;
            CatchRate = 0;
        }

        // Gen-specific: N's Sparkle (Gen 5)
        if (pk is PK5 pk5)
        {
            HasNSparkle = true;
            NSparkle = pk5.NSparkle;
        }
        else
        {
            HasNSparkle = false;
            NSparkle = false;
        }

        // Met — Encounter Type / Ground Tile (Gen 4-6)
        if (pk is IGroundTile gt)
        {
            HasEncounterType = true;
            EncounterType = (int)gt.GroundTile;
            EncounterTypeList = GameInfo.FilteredSources.G4GroundTiles;
        }
        else
        {
            HasEncounterType = false;
            EncounterType = 0;
            EncounterTypeList = Array.Empty<ComboItem>();
        }

        // Met — Time of Day (Gen 2)
        if (pk is ICaughtData2 cd2)
        {
            HasMetTimeOfDay = true;
            MetTimeOfDay = cd2.MetTimeOfDay;
        }
        else
        {
            HasMetTimeOfDay = false;
            MetTimeOfDay = 0;
        }

        // Met — As Egg
        MetAsEgg = pk.EggLocation > 0;

        // Status Condition
        var statusType = pk.GetStatusType();
        StatusConditionText = statusType switch
        {
            StatusType.Paralysis => "Paralysis",
            StatusType.Sleep => "Sleep",
            StatusType.Freeze => "Frozen",
            StatusType.Burn => "Burn",
            StatusType.Poison => "Poison",
            _ => string.Empty,
        };

        // Cosmetic — Shiny Leaf (Gen 4)
        if (pk is G4PKM g4)
        {
            HasShinyLeaf = true;
            var leaf = g4.ShinyLeaf;
            Leaf1 = (leaf & 1) != 0;
            Leaf2 = (leaf & 2) != 0;
            Leaf3 = (leaf & 4) != 0;
            Leaf4 = (leaf & 8) != 0;
            Leaf5 = (leaf & 16) != 0;
            LeafCrown = (leaf & 32) != 0;
        }
        else
        {
            HasShinyLeaf = false;
            Leaf1 = Leaf2 = Leaf3 = Leaf4 = Leaf5 = LeafCrown = false;
        }

        // Cosmetic — Spirit/Mood (Let's Go PB7)
        if (pk is PB7 pb7)
        {
            HasSpirit7b = true;
            Spirit7b = pb7.Spirit;
            Mood7b = pb7.Mood;
        }
        else
        {
            HasSpirit7b = false;
            Spirit7b = 0;
            Mood7b = 0;
        }

        // Cosmetic — PokeStarFame (Gen 5)
        if (pk is PK5 pk5Star)
        {
            HasPokeStarFame = true;
            PokeStarFame = pk5Star.PokeStarFame;
        }
        else
        {
            HasPokeStarFame = false;
            PokeStarFame = 0;
        }

        // Cosmetic — Walking Mood (Gen 4 HG/SS)
        if (pk is G4PKM g4Mood)
        {
            HasWalkingMood = true;
            WalkingMood = g4Mood.WalkingMood;
        }
        else
        {
            HasWalkingMood = false;
            WalkingMood = 0;
        }

        // OT/Misc — Received Date (Let's Go PB7)
        if (pk is PB7 pb7Recv)
        {
            HasReceivedDate = true;
            ReceivedYear = pb7Recv.ReceivedYear;
            ReceivedMonth = pb7Recv.ReceivedMonth;
            ReceivedDay = pb7Recv.ReceivedDay;
        }
        else
        {
            HasReceivedDate = false;
            ReceivedYear = 0;
            ReceivedMonth = 0;
            ReceivedDay = 0;
        }

        // Form Argument
        if (pk is IFormArgument fa)
        {
            HasFormArgument = true;
            FormArgument = fa.FormArgument;
            FormArgumentMax = FormArgumentUtil.GetFormArgumentMax(pk.Species, pk.Form, pk.Context);
            if (FormArgumentMax == 0)
                FormArgumentMax = 255;
        }
        else
        {
            HasFormArgument = false;
            FormArgument = 0;
            FormArgumentMax = 255;
        }

        // Alpha Mastered Move (Legends Arceus)
        if (pk is PA8 pa8)
        {
            HasAlphaMove = true;
            var alphaMoveId = pa8.AlphaMove;
            SelectedAlphaMove = MoveList.FirstOrDefault(m => m.Value == alphaMoveId);
        }
        else
        {
            HasAlphaMove = false;
            SelectedAlphaMove = null;
        }

        // Move Shop (Legends Arceus)
        HasMoveShop = pk is IMoveShop8Mastery;

        // Tech Records (Gen 8+)
        HasTechRecords = pk is ITechRecord;

        // Plus Records (Gen 9a - Z:A)
        HasPlusRecord = pk is IPlusRecord;

        // Super Training / Medals (Gen 6)
        HasSuperTraining = pk is ISuperTrainRegimen;

        // Origin Mark indicator
        var gen = pk.Generation;
        HasOriginMark = gen >= 3;
        OriginMarkText = gen switch
        {
            3 => "Gen 3",
            4 => "Gen 4",
            5 => "Gen 5",
            6 => "Gen 6 \u2726",
            7 => "Gen 7 \u2605",
            8 => "Gen 8 \u25C6",
            9 => "Gen 9 \u25C7",
            _ => gen > 0 ? $"Gen {gen}" : string.Empty,
        };

        // Refresh location lists based on version/context before setting selected items
        RefreshLocationLists();
        RefreshFormList();
        RefreshAbilityList();

        // Look up ComboItems by matching Value
        SelectedSpecies = SpeciesList.FirstOrDefault(x => x.Value == pk.Species);
        SelectedNature = NatureList.FirstOrDefault(x => x.Value == (int)pk.Nature);
        if (HasStatNature)
            SelectedStatNature = NatureList.FirstOrDefault(x => x.Value == (int)pk.StatNature);
        SelectedHeldItem = HeldItemList.FirstOrDefault(x => x.Value == pk.HeldItem);
        SelectedMove1 = MoveList.FirstOrDefault(x => x.Value == pk.Move1);
        SelectedMove2 = MoveList.FirstOrDefault(x => x.Value == pk.Move2);
        SelectedMove3 = MoveList.FirstOrDefault(x => x.Value == pk.Move3);
        SelectedMove4 = MoveList.FirstOrDefault(x => x.Value == pk.Move4);
        SelectedAbility = AbilityList.FirstOrDefault(x => x.Value == pk.Ability);
        SelectedLanguage = LanguageList.FirstOrDefault(x => x.Value == pk.Language);
        SelectedBall = BallList.FirstOrDefault(x => x.Value == pk.Ball);
        UpdateBallSprite();
        SelectedVersion = VersionList.FirstOrDefault(x => x.Value == (int)pk.Version);
        SelectedMetLocation = MetLocationList.FirstOrDefault(x => x.Value == pk.MetLocation);
        SelectedEggLocation = EggLocationList.FirstOrDefault(x => x.Value == pk.EggLocation);

        if (pk is IBattleVersion)
            SelectedBattleVersion = VersionList.FirstOrDefault(x => x.Value == BattleVersion);

        if (pk is IGroundTile)
            SelectedEncounterType = EncounterTypeList.FirstOrDefault(x => x.Value == EncounterType);

        SelectedForm = FormList.FirstOrDefault(x => x.Value == pk.Form);

        // Relearn move ComboItem selections
        if (hasRelearn)
        {
            SelectedRelearnMove1 = MoveList.FirstOrDefault(x => x.Value == pk.RelearnMove1);
            SelectedRelearnMove2 = MoveList.FirstOrDefault(x => x.Value == pk.RelearnMove2);
            SelectedRelearnMove3 = MoveList.FirstOrDefault(x => x.Value == pk.RelearnMove3);
            SelectedRelearnMove4 = MoveList.FirstOrDefault(x => x.Value == pk.RelearnMove4);
        }
        else
        {
            SelectedRelearnMove1 = SelectedRelearnMove2 = SelectedRelearnMove3 = SelectedRelearnMove4 = null;
        }

        UpdateSprite();
        }
        finally
        {
            _isPopulating = false;
        }
        UpdateLegality();
    }

    /// <summary>
    /// Writes the current ViewModel property values back to the underlying <see cref="Entity"/>
    /// and returns the prepared PKM. Used by "Set" operations to write editor data into a slot.
    /// </summary>
    public PKM? PreparePKM()
    {
        if (Entity is null)
            return null;

        Entity.Species = Species;
        Entity.Nickname = Nickname;
        Entity.Form = Form;
        Entity.Gender = Gender;
        Entity.Nature = Nature;
        if (HasStatNature)
            Entity.StatNature = StatNature;
        Entity.Ability = Ability;
        if (IsHaXMode)
            Entity.AbilityNumber = AbilityNumber;
        Entity.HeldItem = HeldItem;

        // New fields
        if (uint.TryParse(PidHex, System.Globalization.NumberStyles.HexNumber, null, out var pid))
            Entity.PID = pid;
        Entity.EXP = Exp;
        Entity.Stat_Level = Level;
        if (IsEgg)
            Entity.OriginalTrainerFriendship = (byte)Math.Clamp(Friendship, 0, 255);
        else
            Entity.CurrentFriendship = (byte)Math.Clamp(Friendship, 0, 255);
        Entity.Language = Language;

        Entity.IV_HP = Iv_HP;
        Entity.IV_ATK = Iv_ATK;
        Entity.IV_DEF = Iv_DEF;
        Entity.IV_SPA = Iv_SPA;
        Entity.IV_SPD = Iv_SPD;
        Entity.IV_SPE = Iv_SPE;

        Entity.EV_HP = Ev_HP;
        Entity.EV_ATK = Ev_ATK;
        Entity.EV_DEF = Ev_DEF;
        Entity.EV_SPA = Ev_SPA;
        Entity.EV_SPD = Ev_SPD;
        Entity.EV_SPE = Ev_SPE;

        // Awakening Values (Let's Go)
        if (Entity is IAwakened awSave)
        {
            awSave.AV_HP = (byte)Math.Clamp(AvHp, 0, 200);
            awSave.AV_ATK = (byte)Math.Clamp(AvAtk, 0, 200);
            awSave.AV_DEF = (byte)Math.Clamp(AvDef, 0, 200);
            awSave.AV_SPA = (byte)Math.Clamp(AvSpa, 0, 200);
            awSave.AV_SPD = (byte)Math.Clamp(AvSpd, 0, 200);
            awSave.AV_SPE = (byte)Math.Clamp(AvSpe, 0, 200);
        }

        // Ganbaru Values (Legends Arceus)
        if (Entity is IGanbaru gvSave)
        {
            gvSave.GV_HP = (byte)Math.Clamp(GvHp, 0, 10);
            gvSave.GV_ATK = (byte)Math.Clamp(GvAtk, 0, 10);
            gvSave.GV_DEF = (byte)Math.Clamp(GvDef, 0, 10);
            gvSave.GV_SPA = (byte)Math.Clamp(GvSpa, 0, 10);
            gvSave.GV_SPD = (byte)Math.Clamp(GvSpd, 0, 10);
            gvSave.GV_SPE = (byte)Math.Clamp(GvSpe, 0, 10);
        }

        Entity.Move1 = Move1;
        Entity.Move2 = Move2;
        Entity.Move3 = Move3;
        Entity.Move4 = Move4;

        // Move PP and PP Ups
        Entity.Move1_PP = Move1_PP;
        Entity.Move2_PP = Move2_PP;
        Entity.Move3_PP = Move3_PP;
        Entity.Move4_PP = Move4_PP;
        Entity.Move1_PPUps = Move1_PPUps;
        Entity.Move2_PPUps = Move2_PPUps;
        Entity.Move3_PPUps = Move3_PPUps;
        Entity.Move4_PPUps = Move4_PPUps;

        // Relearn Moves
        if (HasRelearnMoves)
        {
            Entity.RelearnMove1 = RelearnMove1;
            Entity.RelearnMove2 = RelearnMove2;
            Entity.RelearnMove3 = RelearnMove3;
            Entity.RelearnMove4 = RelearnMove4;
        }

        // Hyper Training
        if (Entity is IHyperTrain htSaveHt)
        {
            htSaveHt.HT_HP = HtHp;
            htSaveHt.HT_ATK = HtAtk;
            htSaveHt.HT_DEF = HtDef;
            htSaveHt.HT_SPA = HtSpA;
            htSaveHt.HT_SPD = HtSpD;
            htSaveHt.HT_SPE = HtSpe;
        }

        // Dynamax Level
        if (Entity is IDynamaxLevel dlSave)
            dlSave.DynamaxLevel = (byte)Math.Clamp(DynamaxLevel, 0, 10);

        // Gigantamax
        if (Entity is IGigantamax gmSave)
            gmSave.CanGigantamax = CanGigantamax;

        // Alpha
        if (Entity is IAlpha alphaSave)
            alphaSave.IsAlpha = IsAlpha;

        // Noble
        if (Entity is INoble nobleSave)
            nobleSave.IsNoble = IsNoble;

        // Tera Type
        if (Entity is ITeraType ttSave)
        {
            ttSave.TeraTypeOriginal = (MoveType)TeraTypeOriginal;
            ttSave.TeraTypeOverride = (MoveType)TeraTypeOverride;
        }

        Entity.OriginalTrainerName = Ot;
        Entity.TID16 = Tid;
        Entity.SID16 = Sid;

        Entity.IsEgg = IsEgg;
        Entity.IsNicknamed = IsNicknamed;

        // Shiny
        if (IsShiny && !Entity.IsShiny)
            Entity.SetShiny();
        else if (!IsShiny && Entity.IsShiny)
            Entity.SetPIDGender(Entity.Gender);

        // Pokerus
        Entity.IsPokerusInfected = IsInfected;
        Entity.IsPokerusCured = IsCured;
        Entity.PokerusStrain = PkrsStrain;
        Entity.PokerusDays = PkrsDays;

        // OT Gender
        Entity.OriginalTrainerGender = OtGender;

        // Handling Trainer
        Entity.CurrentHandler = (byte)CurrentHandler;
        Entity.HandlingTrainerName = HandlingTrainerName;
        Entity.HandlingTrainerGender = HtGender;

        // Handler Language
        if (Entity is IHandlerLanguage hlSave)
            hlSave.HandlingTrainerLanguage = (byte)Math.Clamp(HtLanguage, 0, 255);

        // Region Origin (Gen 6-7)
        if (Entity is IRegionOrigin roSave)
        {
            roSave.Country = (byte)Math.Clamp(Country, 0, 255);
            roSave.Region = (byte)Math.Clamp(SubRegion, 0, 255);
            roSave.ConsoleRegion = (byte)Math.Clamp(ConsoleRegion, 0, 255);
        }

        // Encryption Constant
        if (uint.TryParse(EncryptionConstantHex, System.Globalization.NumberStyles.HexNumber, null, out var ec))
            Entity.EncryptionConstant = ec;

        // Home Tracker
        if (Entity is IHomeTrack htSave && ulong.TryParse(HomeTrackerHex, System.Globalization.NumberStyles.HexNumber, null, out var tracker))
            htSave.Tracker = tracker;

        // Cosmetic — Markings
        if (Entity is IAppliedMarkings7 m7)
        {
            m7.MarkingCircle = (MarkingColor)MarkCircle;
            m7.MarkingTriangle = (MarkingColor)MarkTriangle;
            m7.MarkingSquare = (MarkingColor)MarkSquare;
            m7.MarkingHeart = (MarkingColor)MarkHeart;
            m7.MarkingStar = (MarkingColor)MarkStar;
            m7.MarkingDiamond = (MarkingColor)MarkDiamond;
        }
        else if (Entity is IAppliedMarkings4 m4)
        {
            m4.MarkingCircle = MarkCircle != 0;
            m4.MarkingTriangle = MarkTriangle != 0;
            m4.MarkingSquare = MarkSquare != 0;
            m4.MarkingHeart = MarkHeart != 0;
            m4.MarkingStar = MarkStar != 0;
            m4.MarkingDiamond = MarkDiamond != 0;
        }
        else if (Entity is IAppliedMarkings3 m3)
        {
            m3.MarkingCircle = MarkCircle != 0;
            m3.MarkingTriangle = MarkTriangle != 0;
            m3.MarkingSquare = MarkSquare != 0;
            m3.MarkingHeart = MarkHeart != 0;
        }

        // Cosmetic — Contest Stats
        if (Entity is IContestStats csSave)
        {
            csSave.ContestCool = (byte)Math.Clamp(ContestCool, 0, 255);
            csSave.ContestBeauty = (byte)Math.Clamp(ContestBeauty, 0, 255);
            csSave.ContestCute = (byte)Math.Clamp(ContestCute, 0, 255);
            csSave.ContestSmart = (byte)Math.Clamp(ContestSmart, 0, 255);
            csSave.ContestTough = (byte)Math.Clamp(ContestTough, 0, 255);
            csSave.ContestSheen = (byte)Math.Clamp(ContestSheen, 0, 255);
        }

        // Cosmetic — Favorite
        if (Entity is IFavorite favSave)
            favSave.IsFavorite = IsFavorite;

        // Met
        Entity.MetLocation = MetLocation;
        Entity.MetLevel = MetLevel;
        Entity.Ball = Ball;
        Entity.EggLocation = EggLocation;
        Entity.Version = (GameVersion)Version;
        Entity.MetYear = (byte)Math.Clamp(MetYear, 0, 255);
        Entity.MetMonth = (byte)Math.Clamp(MetMonth, 1, 12);
        Entity.MetDay = (byte)Math.Clamp(MetDay, 1, 31);
        Entity.EggYear = (byte)Math.Clamp(EggYear, 0, 255);
        Entity.EggMonth = (byte)Math.Clamp(EggMonth, 1, 12);
        Entity.EggDay = (byte)Math.Clamp(EggDay, 1, 31);

        // Met — Fateful Encounter
        Entity.FatefulEncounter = FatefulEncounter;

        // Met — Obedience Level
        if (Entity is IObedienceLevel olSave)
            olSave.ObedienceLevel = ObedienceLevel;

        // Met — Battle Version
        if (Entity is IBattleVersion bvSave)
            bvSave.BattleVersion = (GameVersion)BattleVersion;

        // Cosmetic — Size/Scale
        if (Entity is IScaledSize ssSave)
        {
            ssSave.HeightScalar = (byte)Math.Clamp(HeightScalar, 0, 255);
            ssSave.WeightScalar = (byte)Math.Clamp(WeightScalar, 0, 255);
            if (Entity is IScaledSize3 ss3Save)
                ss3Save.Scale = (byte)Math.Clamp(Scale, 0, 255);
        }

        // Gen-specific: Shadow Pokemon (XD/Colosseum)
        if (Entity is IShadowCapture scSave)
        {
            scSave.ShadowID = (ushort)Math.Clamp(ShadowId, 0, 65535);
            scSave.Purification = Purification;
            // IsShadow is derived from ShadowID/Purification, no separate write needed
        }

        // Gen-specific: Catch Rate (Gen 1)
        if (Entity is PK1 pk1Save)
            pk1Save.CatchRate = (byte)Math.Clamp(CatchRate, 0, 255);

        // Gen-specific: N's Sparkle (Gen 5)
        if (Entity is PK5 pk5Save)
        {
            pk5Save.NSparkle = NSparkle;
            pk5Save.PokeStarFame = (byte)Math.Clamp(PokeStarFame, 0, 255);
        }

        // Met — Encounter Type / Ground Tile (Gen 4-6)
        if (Entity is IGroundTile gtSave)
            gtSave.GroundTile = (GroundTileType)EncounterType;

        // Met — Time of Day (Gen 2)
        if (Entity is ICaughtData2 cd2Save)
            cd2Save.MetTimeOfDay = Math.Clamp(MetTimeOfDay, 0, 3);

        // Met — As Egg: if unchecked, clear egg location
        if (!MetAsEgg)
        {
            Entity.EggLocation = 0;
            Entity.EggYear = 0;
            Entity.EggMonth = 1;
            Entity.EggDay = 1;
        }

        // Cosmetic — Shiny Leaf (Gen 4)
        if (Entity is G4PKM g4Save)
        {
            int leaf = 0;
            if (Leaf1) leaf |= 1;
            if (Leaf2) leaf |= 2;
            if (Leaf3) leaf |= 4;
            if (Leaf4) leaf |= 8;
            if (Leaf5) leaf |= 16;
            if (LeafCrown) leaf |= 32;
            g4Save.ShinyLeaf = leaf;
        }

        // Cosmetic — Spirit/Mood (Let's Go PB7)
        if (Entity is PB7 pb7Save)
        {
            pb7Save.Spirit = (byte)Math.Clamp(Spirit7b, 0, 255);
            pb7Save.Mood = (byte)Math.Clamp(Mood7b, 0, 255);
            pb7Save.ReceivedYear = (byte)Math.Clamp(ReceivedYear, 0, 255);
            pb7Save.ReceivedMonth = (byte)Math.Clamp(ReceivedMonth, 1, 12);
            pb7Save.ReceivedDay = (byte)Math.Clamp(ReceivedDay, 1, 31);
        }

        // Cosmetic — Walking Mood (Gen 4 HG/SS)
        if (Entity is G4PKM g4MoodSave)
            g4MoodSave.WalkingMood = (sbyte)Math.Clamp(WalkingMood, -127, 127);

        // Form Argument
        if (Entity is IFormArgument faSave)
            faSave.FormArgument = FormArgument;

        // Alpha Mastered Move (Legends Arceus)
        if (Entity is PA8 pa8Save && SelectedAlphaMove is not null)
            pa8Save.AlphaMove = (ushort)Math.Clamp(SelectedAlphaMove.Value, 0, 65535);

        // Post-save cleanup (matching WinForms SaveFields)
        Entity.FixMoves();
        switch (Entity)
        {
            case G6PKM g6: g6.FixRelearn(); break;
            case G8PKM g8: g8.FixRelearn(); break;
            case PK9 pk9: pk9.FixRelearn(); break;
            case PA8 pa8r: pa8r.FixRelearn(); break;
            case PA9 pa9r: pa9r.FixRelearn(); break;
        }
        Entity.ResetPartyStats();
        Entity.RefreshChecksum();

        return Entity;
    }

    // --- Move Shop / Tech Record editor commands ---

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task OpenMoveShop()
    {
        if (Entity is not IMoveShop8Mastery master || Entity is not IMoveShop8 shop) return;
        try
        {
            PreparePKM();
            var vm = new MoveShopEditorViewModel(shop, master, Entity);
            var view = new MoveShopEditorView { DataContext = vm };
            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow != null)
                await view.ShowDialog(mainWindow);
        }
        catch (Exception ex) { LegalityReport = $"Move Shop error: {ex.Message}"; }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task OpenTechRecords()
    {
        if (Entity is not ITechRecord tr) return;
        try
        {
            PreparePKM();
            var vm = new TechRecordEditorViewModel(tr, Entity);
            var view = new TechRecordEditorView { DataContext = vm };
            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow != null)
                await view.ShowDialog(mainWindow);
        }
        catch (Exception ex) { LegalityReport = $"Tech Record error: {ex.Message}"; }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task OpenPlusRecord()
    {
        if (Entity is not IPlusRecord plus) return;
        if (Entity.PersonalInfo is not IPermitPlus permit) return;
        try
        {
            PreparePKM();
            var vm = new PlusRecordEditorViewModel(plus, permit, Entity);
            var view = new PlusRecordEditorView { DataContext = vm };
            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow != null)
                await view.ShowDialog(mainWindow);
        }
        catch (Exception ex) { LegalityReport = $"Plus Record error: {ex.Message}"; }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task OpenMedals()
    {
        if (Entity is not ISuperTrainRegimen st) return;
        try
        {
            PreparePKM();
            var vm = new SuperTrainingEditorViewModel(st);
            var view = new SuperTrainingEditorView { DataContext = vm };
            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow != null)
                await view.ShowDialog(mainWindow);
        }
        catch (Exception ex) { LegalityReport = $"Super Training error: {ex.Message}"; }
    }

    // --- Ribbons / Memories commands ---

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task OpenRibbons()
    {
        if (Entity is null) return;
        try
        {
            PreparePKM();
            var vm = new PKHeX.Avalonia.ViewModels.Subforms.RibbonEditorViewModel(Entity);
            var view = new PKHeX.Avalonia.Views.Subforms.RibbonEditorView { DataContext = vm };
            var mainWindow = (global::Avalonia.Application.Current?.ApplicationLifetime as global::Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow is not null)
                await view.ShowDialog(mainWindow);
        }
        catch (Exception ex) { LegalityReport = $"Ribbon editor error: {ex.Message}"; }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task OpenMemories()
    {
        if (Entity is null) return;
        try
        {
            PreparePKM();
            var vm = new PKHeX.Avalonia.ViewModels.Subforms.MemoryAmieViewModel(Entity);
            var view = new PKHeX.Avalonia.Views.Subforms.MemoryAmieView { DataContext = vm };
            var mainWindow = (global::Avalonia.Application.Current?.ApplicationLifetime as global::Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow is not null)
                await view.ShowDialog(mainWindow);
        }
        catch (Exception ex) { LegalityReport = $"Memory editor error: {ex.Message}"; }
    }

    // --- IV/EV quick-set commands ---

    [RelayCommand]
    private void MaxIvs()
    {
        Iv_HP = 31; Iv_ATK = 31; Iv_DEF = 31;
        Iv_SPA = 31; Iv_SPD = 31; Iv_SPE = 31;
    }

    [RelayCommand]
    private void ClearIvs()
    {
        Iv_HP = 0; Iv_ATK = 0; Iv_DEF = 0;
        Iv_SPA = 0; Iv_SPD = 0; Iv_SPE = 0;
    }

    [RelayCommand]
    private void MaxEvs()
    {
        const int max = 510;
        const int perStat = 252;
        int remaining = max;
        Ev_HP = Math.Min(perStat, remaining); remaining -= Ev_HP;
        Ev_ATK = Math.Min(perStat, remaining); remaining -= Ev_ATK;
        Ev_DEF = Math.Min(perStat, remaining); remaining -= Ev_DEF;
        Ev_SPA = Math.Min(perStat, remaining); remaining -= Ev_SPA;
        Ev_SPD = Math.Min(perStat, remaining); remaining -= Ev_SPD;
        Ev_SPE = Math.Min(perStat, remaining);
    }

    [RelayCommand]
    private void ClearEvs()
    {
        Ev_HP = 0; Ev_ATK = 0; Ev_DEF = 0;
        Ev_SPA = 0; Ev_SPD = 0; Ev_SPE = 0;
    }

    [RelayCommand]
    private void RandomizeIvs()
    {
        var rng = new Random();
        Iv_HP = rng.Next(32);
        Iv_ATK = rng.Next(32);
        Iv_DEF = rng.Next(32);
        Iv_SPA = rng.Next(32);
        Iv_SPD = rng.Next(32);
        Iv_SPE = rng.Next(32);
    }

    [RelayCommand]
    private void RandomizeEvs()
    {
        var rng = new Random();
        int remaining = 510;
        var stats = new int[6];
        for (int i = 0; i < 6 && remaining > 0; i++)
        {
            int max = Math.Min(252, remaining);
            stats[i] = rng.Next(max + 1);
            remaining -= stats[i];
        }
        // Shuffle so it's not biased toward HP
        for (int i = stats.Length - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (stats[i], stats[j]) = (stats[j], stats[i]);
        }
        Ev_HP = stats[0]; Ev_ATK = stats[1]; Ev_DEF = stats[2];
        Ev_SPA = stats[3]; Ev_SPD = stats[4]; Ev_SPE = stats[5];
    }

    // --- Full legality report (verbose, copies to clipboard) ---

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task ShowFullLegalityReport()
    {
        if (Entity is null) return;
        PreparePKM();
        var la = new LegalityAnalysis(Entity);
        var report = la.Report(true);
        LegalityReport = report;
        var clipboard = GetClipboard();
        if (clipboard is not null)
            await clipboard.SetTextAsync(report);
    }

    // --- Nickname changed → auto-detect IsNicknamed ---

    partial void OnNicknameChanged(string value)
    {
        if (_isPopulating || Entity is null) return;
        Entity.Nickname = value;
        var defaultName = SpeciesName.GetSpeciesNameGeneration(Entity.Species, Entity.Language, Entity.Format);
        if (value != defaultName)
            IsNicknamed = true;
        UpdateLegality();
    }

    // --- Level ↔ EXP bidirectional sync ---

    partial void OnLevelChanged(byte value)
    {
        if (_isPopulating || Entity is null) return;
        Entity.Stat_Level = value;
        var pi = Entity.PersonalInfo;
        if (pi is null) return;
        var growth = pi.EXPGrowth;
        _isPopulating = true;
        try { Exp = Experience.GetEXP(value, growth); }
        finally { _isPopulating = false; }
        Entity.EXP = Exp; // Sync EXP to Entity so ResetPartyStats uses correct level
        RecalcStats();
        UpdateLegality();
    }

    partial void OnExpChanged(uint value)
    {
        if (_isPopulating || Entity is null) return;
        Entity.EXP = value;
        var pi = Entity.PersonalInfo;
        if (pi is null) return;
        var growth = pi.EXPGrowth;
        _isPopulating = true;
        try { Level = Experience.GetLevel(value, growth); }
        finally { _isPopulating = false; }
        RecalcStats();
        UpdateLegality();
    }

    // --- IsEgg change triggers legality + sprite + label ---

    partial void OnIsEggChanged(bool value)
    {
        if (_isPopulating || Entity is null) return;
        Entity.IsEgg = value;

        if (value) // Becoming an egg
        {
            // Set friendship to hatch cycles (clamped to byte range)
            var pokemon = Entity.PersonalInfo;
            if (pokemon is null) return;
            Friendship = Math.Clamp((int)pokemon.HatchCycles, 0, 255);
            Entity.CurrentFriendship = (byte)Friendship;

            // Set met as egg
            MetAsEgg = true;

            // Reset met date to 2000-01-01 (egg has no met date yet)
            _isPopulating = true;
            try
            {
                MetYear = 0; MetMonth = 1; MetDay = 1;
                Entity.MetYear = 0; Entity.MetMonth = 1; Entity.MetDay = 1;
            }
            finally { _isPopulating = false; }

            // Set met location based on trade status (Gen 4+)
            if (Entity.Format >= 4 && _sav is not null)
            {
                bool isTraded = _sav.OT != Entity.OriginalTrainerName || _sav.TID16 != Entity.TID16 || _sav.SID16 != Entity.SID16;
                var loc = isTraded
                    ? Locations.TradedEggLocation(_sav.Generation, _sav.Version)
                    : LocationEdits.GetNoneLocation(Entity);
                _isPopulating = true;
                try
                {
                    MetLocation = loc;
                    Entity.MetLocation = loc;
                    SelectedMetLocation = MetLocationList.FirstOrDefault(x => x.Value == loc);
                }
                finally { _isPopulating = false; }
            }

            // Gen 9 eggs have Version = 0 until hatched
            if (Entity is PK9)
            {
                _isPopulating = true;
                try
                {
                    Entity.Version = 0;
                    Version = 0;
                    SelectedVersion = VersionList.FirstOrDefault(x => x.Value == 0);
                }
                finally { _isPopulating = false; }
            }

            // Update nickname to egg name
            var eggName = SpeciesName.GetEggName(Entity.Language, Entity.Format);
            _isPopulating = true;
            try
            {
                Nickname = eggName;
                Entity.Nickname = eggName;
                IsNicknamed = EggStateLegality.IsNicknameFlagSet(Entity);
                Entity.IsNicknamed = IsNicknamed;
            }
            finally { _isPopulating = false; }

            // Clear memories for Gen 6+
            if (Entity.Format >= 6)
                Entity.ClearMemories();
        }
        else // No longer an egg
        {
            // Reset nickname from egg name to species name
            var speciesName = SpeciesName.GetSpeciesNameGeneration(Entity.Species, Entity.Language, Entity.Format);
            _isPopulating = true;
            try
            {
                Nickname = speciesName;
                Entity.Nickname = speciesName;
                IsNicknamed = false;
                Entity.IsNicknamed = false;
            }
            finally { _isPopulating = false; }

            // Set base friendship (clamped to byte range)
            var basePi = Entity.PersonalInfo;
            if (basePi is null) return;
            Friendship = Math.Clamp((int)basePi.BaseFriendship, 0, 255);
            Entity.CurrentFriendship = (byte)Friendship;

            // Sync met date from egg date, or set to today
            if (Entity.EggLocation != 0)
            {
                _isPopulating = true;
                try
                {
                    MetYear = EggYear; MetMonth = EggMonth; MetDay = EggDay;
                    Entity.MetYear = (byte)Math.Clamp(EggYear, 0, 255);
                    Entity.MetMonth = (byte)Math.Clamp(EggMonth, 1, 12);
                    Entity.MetDay = (byte)Math.Clamp(EggDay, 1, 31);
                }
                finally { _isPopulating = false; }
            }
            else
            {
                // No egg location: set met date to today
                var now = DateTime.Now;
                _isPopulating = true;
                try
                {
                    MetYear = now.Year - 2000; MetMonth = now.Month; MetDay = now.Day;
                }
                finally { _isPopulating = false; }
            }

            // Reset egg dates
            _isPopulating = true;
            try
            {
                EggYear = 0; EggMonth = 1; EggDay = 1;
                Entity.EggYear = 0; Entity.EggMonth = 1; Entity.EggDay = 1;
            }
            finally { _isPopulating = false; }

            // If no egg location, set reasonable defaults for no-longer-egg
            if (EggLocation == 0)
            {
                _isPopulating = true;
                try
                {
                    MetAsEgg = false;
                }
                finally { _isPopulating = false; }
            }
            else
            {
                // Has egg location — set met date/location to match hatching
                _isPopulating = true;
                try
                {
                    var hatchLoc = EncounterSuggestion.GetSuggestedEggMetLocation(Entity);
                    MetLocation = hatchLoc;
                    Entity.MetLocation = hatchLoc;
                    SelectedMetLocation = MetLocationList.FirstOrDefault(x => x.Value == hatchLoc);
                }
                finally { _isPopulating = false; }
            }
        }

        OnPropertyChanged(nameof(FriendshipLabel));
        UpdateSprite();
        UpdateLegality();
    }

    partial void OnIsShinyChanged(bool value)
    {
        if (!_isPopulating && Entity is not null)
        {
            if (value && !Entity.IsShiny) Entity.SetShiny();
            else if (!value && Entity.IsShiny) Entity.SetPIDGender(Entity.Gender);

            // Sync PID + EC hex after PID was modified by SetShiny/SetPIDGender
            _isPopulating = true;
            try
            {
                PidHex = Entity.PID.ToString("X8");
                EncryptionConstantHex = Entity.EncryptionConstant.ToString("X8");
            }
            finally { _isPopulating = false; }

            IsShinyDisplay = value;
            IsSquareShiny = value && Entity.ShinyXor == 0;
            ShowShinyMark = value;
            UpdateSprite();
            UpdateLegality();
        }
    }

    // --- IV changed handlers → recalc stats + legality ---

    partial void OnIv_HPChanged(int value) { if (!_isPopulating) { RecalcStats(); UpdateLegality(); } }
    partial void OnIv_ATKChanged(int value) { if (!_isPopulating) { RecalcStats(); UpdateLegality(); } }
    partial void OnIv_DEFChanged(int value) { if (!_isPopulating) { RecalcStats(); UpdateLegality(); } }
    partial void OnIv_SPAChanged(int value) { if (!_isPopulating) { RecalcStats(); UpdateLegality(); } }
    partial void OnIv_SPDChanged(int value) { if (!_isPopulating) { RecalcStats(); UpdateLegality(); } }
    partial void OnIv_SPEChanged(int value) { if (!_isPopulating) { RecalcStats(); UpdateLegality(); } }

    // --- EV changed handlers → recalc stats + legality ---

    partial void OnEv_HPChanged(int value) { if (!_isPopulating) { RecalcStats(); UpdateLegality(); } }
    partial void OnEv_ATKChanged(int value) { if (!_isPopulating) { RecalcStats(); UpdateLegality(); } }
    partial void OnEv_DEFChanged(int value) { if (!_isPopulating) { RecalcStats(); UpdateLegality(); } }
    partial void OnEv_SPAChanged(int value) { if (!_isPopulating) { RecalcStats(); UpdateLegality(); } }
    partial void OnEv_SPDChanged(int value) { if (!_isPopulating) { RecalcStats(); UpdateLegality(); } }
    partial void OnEv_SPEChanged(int value) { if (!_isPopulating) { RecalcStats(); UpdateLegality(); } }

    // --- Stat auto-recalculation ---

    private void RecalcStats()
    {
        if (_isPopulating || Entity is null) return;

        // Write current values to Entity
        Entity.IV_HP = Iv_HP;
        Entity.IV_ATK = Iv_ATK;
        Entity.IV_DEF = Iv_DEF;
        Entity.IV_SPA = Iv_SPA;
        Entity.IV_SPD = Iv_SPD;
        Entity.IV_SPE = Iv_SPE;

        Entity.EV_HP = Ev_HP;
        Entity.EV_ATK = Ev_ATK;
        Entity.EV_DEF = Ev_DEF;
        Entity.EV_SPA = Ev_SPA;
        Entity.EV_SPD = Ev_SPD;
        Entity.EV_SPE = Ev_SPE;

        Entity.Nature = Nature;
        if (HasStatNature)
            Entity.StatNature = StatNature;
        Entity.Stat_Level = Level;

        Entity.ResetPartyStats();

        // Read back calculated stats
        Hp = Entity.Stat_HPCurrent;
        Atk = Entity.Stat_ATK;
        Def = Entity.Stat_DEF;
        SpA = Entity.Stat_SPA;
        SpD = Entity.Stat_SPD;
        Spe = Entity.Stat_SPE;

        // Refresh computed display fields
        EvTotal = Ev_HP + Ev_ATK + Ev_DEF + Ev_SPA + Ev_SPD + Ev_SPE;
        IsEvTotalValid = EvTotal <= 510;

        Span<int> ivs = stackalloc int[6];
        Entity.GetIVs(ivs);
        var hpType = HiddenPower.GetType(ivs, Entity.Context);
        var typeNames = GameInfo.Strings.Types;
        HiddenPowerType = hpType + 1 < typeNames.Count ? typeNames[hpType + 1] : $"Type {hpType}";

        var charIdx = Entity.Characteristic;
        if (charIdx >= 0)
        {
            var charStrings = GameInfo.Strings.characteristics;
            CharacteristicText = charIdx < charStrings.Length ? charStrings[charIdx] : string.Empty;
        }
        else
        {
            CharacteristicText = string.Empty;
        }
    }

    // --- Nature stat color indicators ---

    /// <summary>ATK stat color based on nature amplification.</summary>
    public string AtkColor => GetNatureColor(0); // ATK is index 0 in the amp table
    /// <summary>DEF stat color based on nature amplification.</summary>
    public string DefColor => GetNatureColor(1);
    /// <summary>SpA stat color based on nature amplification.</summary>
    public string SpAColor => GetNatureColor(2);
    /// <summary>SpD stat color based on nature amplification.</summary>
    public string SpDColor => GetNatureColor(3);
    /// <summary>SPE stat color based on nature amplification.</summary>
    public string SpeColor => GetNatureColor(4);

    private string GetNatureColor(int statIndex)
    {
        // For Gen 8+, StatNature controls the actual stat amplification
        var nature = HasStatNature ? StatNature : Nature;
        if ((byte)nature >= 25)
            return "#000000"; // default black for invalid nature
        var amps = NatureAmp.GetAmps(nature);
        return amps[statIndex] switch
        {
            1 => "#FF0000",  // red for +10% (boosted)
            -1 => "#0000FF", // blue for -10% (hindered)
            _ => "#000000",  // default/neutral
        };
    }

    // --- Nickname warning ---

    [RelayCommand]
    private void ShowNicknameWarning()
    {
        if (Entity is null) return;
        var nickname = Nickname ?? string.Empty;
        var species = Entity.Species;
        var lang = Entity.Language;
        var defaultName = SpeciesName.GetSpeciesNameGeneration(species, lang, Entity.Format);

        // Check for non-ASCII characters that might not render in older games
        bool hasSpecialChars = false;
        foreach (var c in nickname)
        {
            if (c > 0x7E || c < 0x20)
            {
                hasSpecialChars = true;
                break;
            }
        }

        if (string.IsNullOrEmpty(nickname))
        {
            LegalityReport = "Nickname is empty.";
        }
        else if (nickname == defaultName)
        {
            LegalityReport = $"Nickname matches default species name: '{defaultName}'.";
        }
        else if (hasSpecialChars)
        {
            LegalityReport = $"Nickname '{nickname}' contains special characters that may not render correctly in-game. Default name: '{defaultName}'.";
        }
        else
        {
            LegalityReport = $"Nickname '{nickname}' appears to use standard characters. Default name: '{defaultName}'.";
        }
    }

    // --- OT Name warning ---

    [RelayCommand]
    private void ShowOtNameWarning()
    {
        if (Entity is null) return;
        var otName = Ot ?? string.Empty;

        bool hasSpecialChars = false;
        foreach (var c in otName)
        {
            if (c > 0x7E || c < 0x20)
            {
                hasSpecialChars = true;
                break;
            }
        }

        if (string.IsNullOrEmpty(otName))
        {
            LegalityReport = "OT Name is empty.";
        }
        else if (hasSpecialChars)
        {
            LegalityReport = $"OT Name '{otName}' contains special characters that may not render correctly in-game.";
        }
        else
        {
            LegalityReport = $"OT Name '{otName}' appears to use standard characters.";
        }
    }

    // --- Sprite context menu commands ---

    [RelayCommand]
    private void ShowLegalityReport()
    {
        if (Entity is null) return;
        var la = new LegalityAnalysis(Entity);
        var report = la.Report();
        LegalityReport = report;
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task ExportShowdown()
    {
        if (Entity is null) return;
        var text = ShowdownParsing.GetShowdownText(Entity);
        var clipboard = GetClipboard();
        if (clipboard is not null)
            await clipboard.SetTextAsync(text);
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task SavePkmFile()
    {
        if (Entity is null) return;
        PreparePKM();

        var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (mainWindow is null) return;

        var topLevel = TopLevel.GetTopLevel(mainWindow);
        if (topLevel is null) return;

        var ext = Entity.Extension;
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save PKM File",
            SuggestedFileName = $"{Entity.Species:000} - {Entity.Nickname}.{ext}",
            DefaultExtension = ext,
        });
        if (file is null) return;

        await using var stream = await file.OpenWriteAsync();
        var data = Entity.DecryptedBoxData;
        await stream.WriteAsync(data);
        LegalityReport = $"Saved {file.Name}";
    }

    // --- Click-shortcut commands (WinForms parity) ---

    [RelayCommand]
    private void HealPP()
    {
        if (Entity is null) return;
        Entity.HealPP();
        _isPopulating = true;
        try
        {
            Move1_PP = Entity.Move1_PP;
            Move2_PP = Entity.Move2_PP;
            Move3_PP = Entity.Move3_PP;
            Move4_PP = Entity.Move4_PP;
        }
        finally { _isPopulating = false; }
    }

    [RelayCommand]
    private void MaxFriendship()
    {
        Friendship = IsEgg ? 1 : 255;
        if (Entity is not null)
            Entity.CurrentFriendship = (byte)Friendship;
    }

    [RelayCommand]
    private void SetLevel100()
    {
        Level = 100;
        // OnLevelChanged handles EXP sync, stat recalc, and legality update
    }

    [RelayCommand]
    private void SuggestMoves()
    {
        if (Entity is null) return;
        PreparePKM();
        var moveBuf = new ushort[4];
        Entity.GetMoveSet(moveBuf);
        _isPopulating = true;
        try
        {
            { var m = MoveList.FirstOrDefault(x => x.Value == moveBuf[0]); if (m is not null) SelectedMove1 = m; Move1 = moveBuf[0]; }
            { var m = MoveList.FirstOrDefault(x => x.Value == moveBuf[1]); if (m is not null) SelectedMove2 = m; Move2 = moveBuf[1]; }
            { var m = MoveList.FirstOrDefault(x => x.Value == moveBuf[2]); if (m is not null) SelectedMove3 = m; Move3 = moveBuf[2]; }
            { var m = MoveList.FirstOrDefault(x => x.Value == moveBuf[3]); if (m is not null) SelectedMove4 = m; Move4 = moveBuf[3]; }
        }
        finally { _isPopulating = false; }

        // Write the moves to entity and reset PP
        Entity.Move1 = Move1; Entity.Move2 = Move2; Entity.Move3 = Move3; Entity.Move4 = Move4;
        Entity.HealPP();
        Move1_PP = Entity.Move1_PP; Move2_PP = Entity.Move2_PP;
        Move3_PP = Entity.Move3_PP; Move4_PP = Entity.Move4_PP;
        UpdateLegality();
    }

    [RelayCommand]
    private void SuggestRelearnMoves()
    {
        if (Entity is null || !HasRelearnMoves) return;
        PreparePKM();
        var la = new LegalityAnalysis(Entity);
        var moveBuf = new ushort[4];
        la.GetSuggestedRelearnMoves(moveBuf);

        _isPopulating = true;
        try
        {
            { var m = MoveList.FirstOrDefault(x => x.Value == moveBuf[0]); if (m is not null) SelectedRelearnMove1 = m; RelearnMove1 = moveBuf[0]; }
            { var m = MoveList.FirstOrDefault(x => x.Value == moveBuf[1]); if (m is not null) SelectedRelearnMove2 = m; RelearnMove2 = moveBuf[1]; }
            { var m = MoveList.FirstOrDefault(x => x.Value == moveBuf[2]); if (m is not null) SelectedRelearnMove3 = m; RelearnMove3 = moveBuf[2]; }
            { var m = MoveList.FirstOrDefault(x => x.Value == moveBuf[3]); if (m is not null) SelectedRelearnMove4 = m; RelearnMove4 = moveBuf[3]; }
        }
        finally { _isPopulating = false; }

        Entity.SetRelearnMoves(moveBuf);
        UpdateLegality();
    }

    [RelayCommand]
    private void SuggestMetLocation()
    {
        if (Entity is null) return;
        PreparePKM();
        var la = new LegalityAnalysis(Entity);
        var encounter = la.EncounterMatch;
        var loc = encounter.Location;
        var match = MetLocationList?.FirstOrDefault(x => x.Value == loc);
        if (match is not null)
        {
            _isPopulating = true;
            try
            {
                SelectedMetLocation = match;
                MetLocation = (ushort)match.Value;
            }
            finally { _isPopulating = false; }
            Entity.MetLocation = MetLocation;
            UpdateLegality();
        }
    }

    private static IClipboard? GetClipboard()
    {
        var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var mainWindow = lifetime?.MainWindow;
        if (mainWindow is null)
            return null;
        return TopLevel.GetTopLevel(mainWindow)?.Clipboard;
    }

    // --- Legality auto-check ---

    private void UpdateLegality()
    {
        if (Entity is null)
        {
            var old = LegalityImage;
            LegalityImage = null;
            old?.Dispose();
            Move1Legal = Move2Legal = Move3Legal = Move4Legal = true;
            Relearn1Legal = Relearn2Legal = Relearn3Legal = Relearn4Legal = true;
            return;
        }

        try
        {
            var la = new LegalityAnalysis(Entity);
            var valid = la.Valid;
            var color = valid ? SKColors.Green : SKColors.Red;

            using var surface = SKSurface.Create(new SKImageInfo(24, 24));
            if (surface is null)
            {
                var old2 = LegalityImage;
                LegalityImage = null;
                old2?.Dispose();
                return;
            }
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.Transparent);
            using var paint = new SKPaint { Color = color, IsAntialias = true };
            canvas.DrawCircle(12, 12, 10, paint);

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var ms = new MemoryStream(data.ToArray());
            var oldLegality = LegalityImage;
            LegalityImage = new Bitmap(ms);
            oldLegality?.Dispose();

            // Move legality
            var moves = la.Info.Moves;
            if (moves is { Length: >= 4 })
            {
                Move1Legal = moves[0].Valid;
                Move2Legal = moves[1].Valid;
                Move3Legal = moves[2].Valid;
                Move4Legal = moves[3].Valid;
            }
            else
            {
                Move1Legal = Move2Legal = Move3Legal = Move4Legal = true;
            }

            // Relearn legality
            var relearn = la.Info.Relearn;
            if (relearn is { Length: >= 4 })
            {
                Relearn1Legal = relearn[0].Valid;
                Relearn2Legal = relearn[1].Valid;
                Relearn3Legal = relearn[2].Valid;
                Relearn4Legal = relearn[3].Valid;
            }
            else
            {
                Relearn1Legal = Relearn2Legal = Relearn3Legal = Relearn4Legal = true;
            }
        }
        catch
        {
            var old = LegalityImage;
            LegalityImage = null;
            old?.Dispose();
            Move1Legal = Move2Legal = Move3Legal = Move4Legal = true;
            Relearn1Legal = Relearn2Legal = Relearn3Legal = Relearn4Legal = true;
        }
    }

    private void UpdateSprite()
    {
        if (Entity is null)
            return;

        var sprite = Entity.Sprite();
        var old = SpriteImage;
        SpriteImage = SKBitmapToAvaloniaBitmapConverter.ToAvaloniaBitmap(sprite);
        old?.Dispose();
        sprite.Dispose();
    }
}
