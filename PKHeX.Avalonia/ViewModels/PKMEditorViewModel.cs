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
        if (!_isPopulating)
            UpdateSprite();
    }

    // Stat Nature (Gen 8+)
    [ObservableProperty] private Nature _statNature;
    [ObservableProperty] private bool _hasStatNature;
    [ObservableProperty] private ComboItem? _selectedStatNature;

    partial void OnSelectedStatNatureChanged(ComboItem? value)
    {
        if (value is not null)
            StatNature = (Nature)value.Value;
        if (!_isPopulating)
        {
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

    // Pokerus
    [ObservableProperty] private bool _isInfected;
    [ObservableProperty] private bool _isCured;
    [ObservableProperty] private int _pkrsStrain;
    [ObservableProperty] private int _pkrsDays;

    public bool ShowPkrsDetails => IsInfected;

    partial void OnIsInfectedChanged(bool value)
    {
        OnPropertyChanged(nameof(ShowPkrsDetails));
    }

    // Shiny display indicators
    [ObservableProperty] private bool _isShinyDisplay;
    [ObservableProperty] private bool _isSquareShiny;

    // New fields
    [ObservableProperty] private string _pidHex = "00000000";
    [ObservableProperty] private uint _exp;
    [ObservableProperty] private int _friendship;
    [ObservableProperty] private int _language;

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

    partial void OnSelectedRelearnMove1Changed(ComboItem? value) { if (value is not null) RelearnMove1 = (ushort)value.Value; if (!_isPopulating) UpdateLegality(); }
    partial void OnSelectedRelearnMove2Changed(ComboItem? value) { if (value is not null) RelearnMove2 = (ushort)value.Value; if (!_isPopulating) UpdateLegality(); }
    partial void OnSelectedRelearnMove3Changed(ComboItem? value) { if (value is not null) RelearnMove3 = (ushort)value.Value; if (!_isPopulating) UpdateLegality(); }
    partial void OnSelectedRelearnMove4Changed(ComboItem? value) { if (value is not null) RelearnMove4 = (ushort)value.Value; if (!_isPopulating) UpdateLegality(); }

    // Move legality indicators
    [ObservableProperty] private bool _move1Legal = true;
    [ObservableProperty] private bool _move2Legal = true;
    [ObservableProperty] private bool _move3Legal = true;
    [ObservableProperty] private bool _move4Legal = true;

    // Hyper Training
    [ObservableProperty] private bool _htHp;
    [ObservableProperty] private bool _htAtk;
    [ObservableProperty] private bool _htDef;
    [ObservableProperty] private bool _htSpA;
    [ObservableProperty] private bool _htSpD;
    [ObservableProperty] private bool _htSpe;
    [ObservableProperty] private bool _hasHyperTraining;

    // Dynamax
    [ObservableProperty] private int _dynamaxLevel;
    [ObservableProperty] private bool _hasDynamaxLevel;

    // Gigantamax
    [ObservableProperty] private bool _canGigantamax;
    [ObservableProperty] private bool _hasGigantamax;

    // Alpha (Legends Arceus)
    [ObservableProperty] private bool _isAlpha;
    [ObservableProperty] private bool _hasAlpha;

    // Noble (Legends Arceus)
    [ObservableProperty] private bool _isNoble;
    [ObservableProperty] private bool _hasNoble;

    // Tera Type (Gen 9)
    [ObservableProperty] private int _teraTypeOriginal;
    [ObservableProperty] private int _teraTypeOverride;
    [ObservableProperty] private bool _hasTeraType;

    // EV Total display
    [ObservableProperty] private int _evTotal;
    [ObservableProperty] private bool _isEvTotalValid;

    // Hidden Power Type display
    [ObservableProperty] private string _hiddenPowerType = string.Empty;

    // Characteristic display
    [ObservableProperty] private string _characteristicText = string.Empty;

    // Base Stat Total
    [ObservableProperty] private int _baseST;

    // Met
    [ObservableProperty] private ushort _metLocation;
    [ObservableProperty] private byte _metLevel;
    [ObservableProperty] private byte _ball;
    [ObservableProperty] private ushort _eggLocation;
    [ObservableProperty] private int _version;
    [ObservableProperty] private int _metYear;
    [ObservableProperty] private int _metMonth;
    [ObservableProperty] private int _metDay;
    [ObservableProperty] private int _eggYear;
    [ObservableProperty] private int _eggMonth;
    [ObservableProperty] private int _eggDay;

    // Cosmetic — Markings
    [ObservableProperty] private bool _markCircle;
    [ObservableProperty] private bool _markTriangle;
    [ObservableProperty] private bool _markSquare;
    [ObservableProperty] private bool _markHeart;
    [ObservableProperty] private bool _markStar;
    [ObservableProperty] private bool _markDiamond;

    // Cosmetic — Contest Stats
    [ObservableProperty] private int _contestCool;
    [ObservableProperty] private int _contestBeauty;
    [ObservableProperty] private int _contestCute;
    [ObservableProperty] private int _contestSmart;
    [ObservableProperty] private int _contestTough;
    [ObservableProperty] private int _contestSheen;

    // Cosmetic — Favorite
    [ObservableProperty] private bool _isFavorite;

    // Cosmetic — visibility helpers
    [ObservableProperty] private bool _hasMarkings;
    [ObservableProperty] private bool _hasContestStats;
    [ObservableProperty] private bool _hasFavorite;

    // Origin Mark indicator
    [ObservableProperty] private string _originMarkText = string.Empty;
    [ObservableProperty] private bool _hasOriginMark;

    // Gen-specific: Shadow Pokemon (XD/Colosseum)
    [ObservableProperty] private int _shadowId;
    [ObservableProperty] private int _purification;
    [ObservableProperty] private bool _isShadow;
    [ObservableProperty] private bool _hasShadow;

    // Form Argument
    [ObservableProperty] private uint _formArgument;
    [ObservableProperty] private bool _hasFormArgument;
    [ObservableProperty] private uint _formArgumentMax;

    // Alpha Mastered Move (Gen 8a - Legends Arceus)
    [ObservableProperty] private ComboItem? _selectedAlphaMove;
    [ObservableProperty] private bool _hasAlphaMove;

    // Move Shop / Tech Record visibility
    [ObservableProperty] private bool _hasMoveShop;
    [ObservableProperty] private bool _hasTechRecords;

    // Gen-specific: Catch Rate (Gen 1)
    [ObservableProperty] private int _catchRate;
    [ObservableProperty] private bool _hasCatchRate;

    // Gen-specific: N's Sparkle (Gen 5)
    [ObservableProperty] private bool _nSparkle;
    [ObservableProperty] private bool _hasNSparkle;

    // Met — Encounter Type / Ground Tile (Gen 4-6)
    [ObservableProperty] private int _encounterType;
    [ObservableProperty] private bool _hasEncounterType;
    [ObservableProperty] private IReadOnlyList<ComboItem> _encounterTypeList = Array.Empty<ComboItem>();
    [ObservableProperty] private ComboItem? _selectedEncounterType;

    // Met — Time of Day (Gen 2)
    [ObservableProperty] private int _metTimeOfDay;
    [ObservableProperty] private bool _hasMetTimeOfDay;

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

    // Cosmetic — Spirit/Mood (Let's Go PB7)
    [ObservableProperty] private int _spirit7b;
    [ObservableProperty] private int _mood7b;
    [ObservableProperty] private bool _hasSpirit7b;

    // Cosmetic — PokeStarFame (Gen 5)
    [ObservableProperty] private int _pokeStarFame;
    [ObservableProperty] private bool _hasPokeStarFame;

    // Cosmetic — Walking Mood (Gen 4 HG/SS)
    [ObservableProperty] private int _walkingMood;
    [ObservableProperty] private bool _hasWalkingMood;

    // OT/Misc — Received Date (Let's Go PB7)
    [ObservableProperty] private int _receivedYear;
    [ObservableProperty] private int _receivedMonth;
    [ObservableProperty] private int _receivedDay;
    [ObservableProperty] private bool _hasReceivedDate;

    // OT
    [ObservableProperty] private string _ot = string.Empty;
    [ObservableProperty] private ushort _tid;
    [ObservableProperty] private ushort _sid;

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
    }

    [RelayCommand]
    private void ToggleOtGender()
    {
        OtGender = (byte)(OtGender == 0 ? 1 : 0);
    }

    // Handling Trainer
    [ObservableProperty] private int _currentHandler;
    [ObservableProperty] private string _handlingTrainerName = string.Empty;
    [ObservableProperty] private byte _htGender;

    public string HtGenderSymbol => HtGender switch
    {
        0 => "\u2642",  // ♂
        1 => "\u2640",  // ♀
        _ => "\u2014",  // —
    };

    partial void OnHtGenderChanged(byte value)
    {
        OnPropertyChanged(nameof(HtGenderSymbol));
    }

    [ObservableProperty] private bool _hasHandler;

    // Encryption Constant
    [ObservableProperty] private string _encryptionConstantHex = "00000000";

    [RelayCommand]
    private void RerollEc()
    {
        var rng = new Random();
        var ec = (uint)rng.Next();
        EncryptionConstantHex = ec.ToString("X8");
    }

    // Home Tracker
    [ObservableProperty] private string _homeTrackerHex = "0000000000000000";
    [ObservableProperty] private bool _hasHomeTracker;

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
            Entity.Data[offset] = (byte)value;
    }

    // Met — Fateful Encounter
    [ObservableProperty] private bool _fatefulEncounter;

    // Met — Obedience Level (Gen 9)
    [ObservableProperty] private byte _obedienceLevel;
    [ObservableProperty] private bool _hasObedienceLevel;

    // Met — Battle Version (Gen 8+)
    [ObservableProperty] private int _battleVersion;
    [ObservableProperty] private bool _hasBattleVersion;
    [ObservableProperty] private ComboItem? _selectedBattleVersion;

    partial void OnSelectedBattleVersionChanged(ComboItem? value)
    {
        if (value is not null)
            BattleVersion = value.Value;
    }

    partial void OnSelectedEncounterTypeChanged(ComboItem? value)
    {
        if (value is not null)
            EncounterType = value.Value;
    }

    // Awakening Values (Let's Go PB7)
    [ObservableProperty] private int _avHp, _avAtk, _avDef, _avSpa, _avSpd, _avSpe;
    [ObservableProperty] private bool _hasAwakeningValues;

    // Ganbaru Values (Legends Arceus PA8)
    [ObservableProperty] private int _gvHp, _gvAtk, _gvDef, _gvSpa, _gvSpd, _gvSpe;
    [ObservableProperty] private bool _hasGanbaruValues;

    // Region Origin (Gen 6-7, 3DS)
    [ObservableProperty] private int _country, _subRegion, _consoleRegion;
    [ObservableProperty] private bool _hasRegionData;

    // Handler Language
    [ObservableProperty] private int _htLanguage;
    [ObservableProperty] private bool _hasHtLanguage;

    // Cosmetic — Size/Scale
    [ObservableProperty] private int _heightScalar;
    [ObservableProperty] private int _weightScalar;
    [ObservableProperty] private int _scale;
    [ObservableProperty] private bool _hasSizeData;
    [ObservableProperty] private bool _hasScale;

    [ObservableProperty]
    private bool _isInitialized;

    // Legality report text (populated by context menu command)
    [ObservableProperty] private string _legalityReport = string.Empty;

    // ComboBox data sources — sourced from GameInfo.FilteredSources
    public IReadOnlyList<ComboItem> SpeciesList => GameInfo.FilteredSources.Species;
    public IReadOnlyList<ComboItem> NatureList => GameInfo.FilteredSources.Natures;
    public IReadOnlyList<ComboItem> HeldItemList => GameInfo.FilteredSources.Items;
    public IReadOnlyList<ComboItem> MoveList => GameInfo.FilteredSources.Moves;
    public IReadOnlyList<ComboItem> AbilityList => GameInfo.FilteredSources.Abilities;
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
        if (!_isPopulating)
        {
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
                try { Nickname = speciesName; }
                finally { _isPopulating = false; }
            }
            UpdateSprite();
            UpdateLegality();
        }
    }

    partial void OnSelectedNatureChanged(ComboItem? value)
    {
        if (value is not null)
            Nature = (Nature)value.Value;
        if (!_isPopulating)
        {
            OnPropertyChanged(nameof(AtkColor));
            OnPropertyChanged(nameof(DefColor));
            OnPropertyChanged(nameof(SpAColor));
            OnPropertyChanged(nameof(SpDColor));
            OnPropertyChanged(nameof(SpeColor));
            RecalcStats();
            UpdateLegality();
        }
    }

    partial void OnSelectedHeldItemChanged(ComboItem? value)
    {
        if (value is not null)
            HeldItem = value.Value;
        if (!_isPopulating)
            UpdateLegality();
    }

    partial void OnSelectedMove1Changed(ComboItem? value)
    {
        if (value is not null)
            Move1 = (ushort)value.Value;
        if (!_isPopulating)
            UpdateLegality();
    }

    partial void OnSelectedMove2Changed(ComboItem? value)
    {
        if (value is not null)
            Move2 = (ushort)value.Value;
        if (!_isPopulating)
            UpdateLegality();
    }

    partial void OnSelectedMove3Changed(ComboItem? value)
    {
        if (value is not null)
            Move3 = (ushort)value.Value;
        if (!_isPopulating)
            UpdateLegality();
    }

    partial void OnSelectedMove4Changed(ComboItem? value)
    {
        if (value is not null)
            Move4 = (ushort)value.Value;
        if (!_isPopulating)
            UpdateLegality();
    }

    partial void OnSelectedAbilityChanged(ComboItem? value)
    {
        if (value is not null)
            Ability = value.Value;
        if (!_isPopulating)
            UpdateLegality();
    }

    partial void OnSelectedLanguageChanged(ComboItem? value)
    {
        if (value is not null)
            Language = value.Value;
    }

    partial void OnSelectedBallChanged(ComboItem? value)
    {
        if (value is not null)
            Ball = (byte)value.Value;
    }

    partial void OnSelectedVersionChanged(ComboItem? value)
    {
        if (value is not null)
        {
            Version = value.Value;
            RefreshLocationLists();
        }
    }

    partial void OnSelectedMetLocationChanged(ComboItem? value)
    {
        if (value is not null)
            MetLocation = (ushort)value.Value;
    }

    partial void OnSelectedEggLocationChanged(ComboItem? value)
    {
        if (value is not null)
            EggLocation = (ushort)value.Value;
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
        OnPropertyChanged(nameof(AbilityList));
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
        }
        else
        {
            HasTeraType = false;
            TeraTypeOriginal = 0;
            TeraTypeOverride = 0;
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
        EggYear = pk.EggYear;
        EggMonth = pk.EggMonth;
        EggDay = pk.EggDay;

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
        }
        else
        {
            HasRegionData = false;
            Country = SubRegion = ConsoleRegion = 0;
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
            MarkCircle = m7.MarkingCircle != MarkingColor.None;
            MarkTriangle = m7.MarkingTriangle != MarkingColor.None;
            MarkSquare = m7.MarkingSquare != MarkingColor.None;
            MarkHeart = m7.MarkingHeart != MarkingColor.None;
            MarkStar = m7.MarkingStar != MarkingColor.None;
            MarkDiamond = m7.MarkingDiamond != MarkingColor.None;
        }
        else if (pk is IAppliedMarkings4 m4)
        {
            HasMarkings = true;
            MarkCircle = m4.MarkingCircle;
            MarkTriangle = m4.MarkingTriangle;
            MarkSquare = m4.MarkingSquare;
            MarkHeart = m4.MarkingHeart;
            MarkStar = m4.MarkingStar;
            MarkDiamond = m4.MarkingDiamond;
        }
        else if (pk is IAppliedMarkings3 m3)
        {
            HasMarkings = true;
            MarkCircle = m3.MarkingCircle;
            MarkTriangle = m3.MarkingTriangle;
            MarkSquare = m3.MarkingSquare;
            MarkHeart = m3.MarkingHeart;
            MarkStar = false;
            MarkDiamond = false;
        }
        else
        {
            HasMarkings = false;
            MarkCircle = MarkTriangle = MarkSquare = MarkHeart = MarkStar = MarkDiamond = false;
        }

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
        Entity.HeldItem = HeldItem;

        // New fields
        if (uint.TryParse(PidHex, System.Globalization.NumberStyles.HexNumber, null, out var pid))
            Entity.PID = pid;
        Entity.EXP = Exp;
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

        // Move PP Ups
        Entity.Move1_PPUps = Move1_PPUps;
        Entity.Move2_PPUps = Move2_PPUps;
        Entity.Move3_PPUps = Move3_PPUps;
        Entity.Move4_PPUps = Move4_PPUps;
        Entity.SetMaximumPPCurrent(Entity.Moves);

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

        // Pokerus
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
            m7.MarkingCircle = MarkCircle ? MarkingColor.Blue : MarkingColor.None;
            m7.MarkingTriangle = MarkTriangle ? MarkingColor.Blue : MarkingColor.None;
            m7.MarkingSquare = MarkSquare ? MarkingColor.Blue : MarkingColor.None;
            m7.MarkingHeart = MarkHeart ? MarkingColor.Blue : MarkingColor.None;
            m7.MarkingStar = MarkStar ? MarkingColor.Blue : MarkingColor.None;
            m7.MarkingDiamond = MarkDiamond ? MarkingColor.Blue : MarkingColor.None;
        }
        else if (Entity is IAppliedMarkings4 m4)
        {
            m4.MarkingCircle = MarkCircle;
            m4.MarkingTriangle = MarkTriangle;
            m4.MarkingSquare = MarkSquare;
            m4.MarkingHeart = MarkHeart;
            m4.MarkingStar = MarkStar;
            m4.MarkingDiamond = MarkDiamond;
        }
        else if (Entity is IAppliedMarkings3 m3)
        {
            m3.MarkingCircle = MarkCircle;
            m3.MarkingTriangle = MarkTriangle;
            m3.MarkingSquare = MarkSquare;
            m3.MarkingHeart = MarkHeart;
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
        Entity.EggMonth = (byte)Math.Clamp(EggMonth, 0, 12);
        Entity.EggDay = (byte)Math.Clamp(EggDay, 0, 31);

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
            scSave.ShadowID = (ushort)ShadowId;
            scSave.Purification = Purification;
        }

        // Gen-specific: Catch Rate (Gen 1)
        if (Entity is PK1 pk1Save)
            pk1Save.CatchRate = (byte)CatchRate;

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
            cd2Save.MetTimeOfDay = MetTimeOfDay;

        // Met — As Egg: if unchecked, clear egg location
        if (!MetAsEgg)
        {
            Entity.EggLocation = 0;
            Entity.EggYear = 0;
            Entity.EggMonth = 0;
            Entity.EggDay = 0;
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
            pb7Save.ReceivedMonth = (byte)Math.Clamp(ReceivedMonth, 0, 12);
            pb7Save.ReceivedDay = (byte)Math.Clamp(ReceivedDay, 0, 31);
        }

        // Cosmetic — Walking Mood (Gen 4 HG/SS)
        if (Entity is G4PKM g4MoodSave)
            g4MoodSave.WalkingMood = (sbyte)Math.Clamp(WalkingMood, -127, 127);

        // Form Argument
        if (Entity is IFormArgument faSave)
            faSave.FormArgument = FormArgument;

        // Alpha Mastered Move (Legends Arceus)
        if (Entity is PA8 pa8Save && SelectedAlphaMove is not null)
            pa8Save.AlphaMove = (ushort)SelectedAlphaMove.Value;

        return Entity;
    }

    // --- Move Shop / Tech Record editor commands ---

    [RelayCommand]
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

    [RelayCommand]
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

    // --- Ribbons / Memories placeholder commands ---

    [RelayCommand]
    private void OpenRibbons()
    {
        LegalityReport = Entity is null
            ? "No Pokémon loaded."
            : "Ribbon editor: use Tools > Ribbon Editor from the main menu.";
    }

    [RelayCommand]
    private void OpenMemories()
    {
        LegalityReport = Entity is null
            ? "No Pokémon loaded."
            : "Memory editor: use Tools > PKM Editors > Memory / Amie from the main menu.";
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
        Ev_HP = 252; Ev_ATK = 252; Ev_DEF = 252;
        Ev_SPA = 252; Ev_SPD = 252; Ev_SPE = 252;
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

    [RelayCommand]
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
        var defaultName = SpeciesName.GetSpeciesNameGeneration(Entity.Species, Entity.Language, Entity.Format);
        if (value != defaultName)
            IsNicknamed = true;
    }

    // --- Level ↔ EXP bidirectional sync ---

    partial void OnLevelChanged(byte value)
    {
        if (_isPopulating || Entity is null) return;
        Entity.Stat_Level = value;
        var growth = Entity.PersonalInfo.EXPGrowth;
        _isPopulating = true;
        try { Exp = Experience.GetEXP(value, growth); }
        finally { _isPopulating = false; }
        RecalcStats();
        UpdateLegality();
    }

    partial void OnExpChanged(uint value)
    {
        if (_isPopulating || Entity is null) return;
        Entity.EXP = value;
        var growth = Entity.PersonalInfo.EXPGrowth;
        _isPopulating = true;
        try { Level = Experience.GetLevel(value, growth); }
        finally { _isPopulating = false; }
        RecalcStats();
    }

    // --- IsEgg change triggers legality ---

    partial void OnIsEggChanged(bool value)
    {
        if (!_isPopulating)
            UpdateLegality();
    }

    partial void OnIsShinyChanged(bool value)
    {
        if (!_isPopulating)
            UpdateSprite();
    }

    // --- IV changed handlers → recalc stats + legality ---

    partial void OnIv_HPChanged(int value) { if (!_isPopulating) RecalcStats(); }
    partial void OnIv_ATKChanged(int value) { if (!_isPopulating) RecalcStats(); }
    partial void OnIv_DEFChanged(int value) { if (!_isPopulating) RecalcStats(); }
    partial void OnIv_SPAChanged(int value) { if (!_isPopulating) RecalcStats(); }
    partial void OnIv_SPDChanged(int value) { if (!_isPopulating) RecalcStats(); }
    partial void OnIv_SPEChanged(int value) { if (!_isPopulating) RecalcStats(); }

    // --- EV changed handlers → recalc stats ---

    partial void OnEv_HPChanged(int value) { if (!_isPopulating) RecalcStats(); }
    partial void OnEv_ATKChanged(int value) { if (!_isPopulating) RecalcStats(); }
    partial void OnEv_DEFChanged(int value) { if (!_isPopulating) RecalcStats(); }
    partial void OnEv_SPAChanged(int value) { if (!_isPopulating) RecalcStats(); }
    partial void OnEv_SPDChanged(int value) { if (!_isPopulating) RecalcStats(); }
    partial void OnEv_SPEChanged(int value) { if (!_isPopulating) RecalcStats(); }

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
        var nature = Nature;
        if ((byte)nature >= 25)
            return "#000000"; // default black for invalid nature
        var amps = NatureAmp.GetAmps(nature);
        return amps[statIndex] switch
        {
            1 => "#4488FF",  // blue for +10%
            -1 => "#FF4444",         // red for -10%
            _ => "#000000",          // default/neutral
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

    // --- Sprite context menu commands ---

    [RelayCommand]
    private void ShowLegalityReport()
    {
        if (Entity is null) return;
        var la = new LegalityAnalysis(Entity);
        var report = la.Report();
        LegalityReport = report;
    }

    [RelayCommand]
    private async Task ExportShowdown()
    {
        if (Entity is null) return;
        var text = ShowdownParsing.GetShowdownText(Entity);
        var clipboard = GetClipboard();
        if (clipboard is not null)
            await clipboard.SetTextAsync(text);
    }

    [RelayCommand]
    private void SavePkmFile()
    {
        if (Entity is null) return;
        PreparePKM();
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
            LegalityImage = null;
            Move1Legal = Move2Legal = Move3Legal = Move4Legal = true;
            return;
        }

        try
        {
            var la = new LegalityAnalysis(Entity);
            var valid = la.Valid;
            var color = valid ? SKColors.Green : SKColors.Red;

            using var surface = SKSurface.Create(new SKImageInfo(24, 24));
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.Transparent);
            using var paint = new SKPaint { Color = color, IsAntialias = true };
            canvas.DrawCircle(12, 12, 10, paint);

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var ms = new MemoryStream(data.ToArray());
            LegalityImage = new Bitmap(ms);

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
        }
        catch
        {
            LegalityImage = null;
            Move1Legal = Move2Legal = Move3Legal = Move4Legal = true;
        }
    }

    private void UpdateSprite()
    {
        if (Entity is null)
            return;

        var sprite = Entity.Sprite();
        SpriteImage = SKBitmapToAvaloniaBitmapConverter.ToAvaloniaBitmap(sprite);
    }
}
