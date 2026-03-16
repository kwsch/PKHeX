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
    }

    [ObservableProperty] private int _ability;
    [ObservableProperty] private int _heldItem;
    [ObservableProperty] private bool _isShiny;
    [ObservableProperty] private bool _isEgg;
    [ObservableProperty] private bool _isNicknamed;

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
            UpdateLegality();
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
        // Notify UI that all list properties changed
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
        Ability = pk.Ability;
        HeldItem = pk.HeldItem;
        IsShiny = pk.IsShiny;
        IsEgg = pk.IsEgg;
        IsNicknamed = pk.IsNicknamed;

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

        Move1 = pk.Move1;
        Move2 = pk.Move2;
        Move3 = pk.Move3;
        Move4 = pk.Move4;

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

        // Encryption Constant
        EncryptionConstantHex = pk.EncryptionConstant.ToString("X8");

        // Home Tracker
        if (pk is IHomeTrack ht)
        {
            HasHomeTracker = true;
            HomeTrackerHex = ht.Tracker.ToString("X16");
        }
        else
        {
            HasHomeTracker = false;
            HomeTrackerHex = "0000000000000000";
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

        // Refresh location lists based on version/context before setting selected items
        RefreshLocationLists();
        RefreshFormList();

        // Look up ComboItems by matching Value
        SelectedSpecies = SpeciesList.FirstOrDefault(x => x.Value == pk.Species);
        SelectedNature = NatureList.FirstOrDefault(x => x.Value == (int)pk.Nature);
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

        SelectedForm = FormList.FirstOrDefault(x => x.Value == pk.Form);

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

        Entity.Move1 = Move1;
        Entity.Move2 = Move2;
        Entity.Move3 = Move3;
        Entity.Move4 = Move4;

        Entity.OriginalTrainerName = Ot;
        Entity.TID16 = Tid;
        Entity.SID16 = Sid;

        Entity.IsEgg = IsEgg;
        Entity.IsNicknamed = IsNicknamed;

        // OT Gender
        Entity.OriginalTrainerGender = OtGender;

        // Handling Trainer
        Entity.CurrentHandler = (byte)CurrentHandler;
        Entity.HandlingTrainerName = HandlingTrainerName;
        Entity.HandlingTrainerGender = HtGender;

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

        return Entity;
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
        }
        catch
        {
            LegalityImage = null;
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
