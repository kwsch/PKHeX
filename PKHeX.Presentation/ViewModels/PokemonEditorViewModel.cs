
using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

public partial class PokemonEditorViewModel : ViewModelBase
{
    private PKM _pk;
    private readonly SaveFile _sav;
    private readonly ISpriteRenderer _spriteRenderer;
    private readonly IDialogService _dialogService;
    private readonly IWindowService _windowService;
    private bool _isLoading; // Flag to prevent modifying _pk during load

    // Data sources (mostly filtered by SaveFile context)
    [ObservableProperty] private IReadOnlyList<ComboItem> _speciesList;
    [ObservableProperty] private IReadOnlyList<ComboItem> _moveList;
    [ObservableProperty] private IReadOnlyList<ComboItem> _natureList;
    [ObservableProperty] private IReadOnlyList<ComboItem> _ballList;
    [ObservableProperty] private IReadOnlyList<ComboItem> _itemList;
    [ObservableProperty] private IReadOnlyList<ComboItem> _originGameList;
    [ObservableProperty] private IReadOnlyList<ComboItem> _relearnMoveDataSource;
    [ObservableProperty] private IReadOnlyList<ComboItem> _languageList;
    
    public IReadOnlyList<ComboItem> GenderList { get; } = [
        new ComboItem("Male", 0),
        new ComboItem("Female", 1),
        new ComboItem("Genderless", 2)
    ];

    // Dynamic lists
    [ObservableProperty]
    private ObservableCollection<ComboItem> _abilityList = [];

    [ObservableProperty]
    private ObservableCollection<ComboItem> _formList = [];

    [ObservableProperty]
    private byte[]? _sprite;

    [ObservableProperty]
    private string _title = LocalizedStrings.Instance["PokemonEditor_DefaultTitle"];

    // Basic Info
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Title), nameof(Sprite), nameof(FormList), nameof(AbilityList), nameof(Ability), nameof(Stat_HP), nameof(Stat_ATK), nameof(Stat_DEF), nameof(Stat_SPA), nameof(Stat_SPD), nameof(Stat_SPE), nameof(Base_HP), nameof(Base_ATK), nameof(Base_DEF), nameof(Base_SPA), nameof(Base_SPD), nameof(Base_SPE))]
    private int _species;

    [ObservableProperty]
    private string _version = string.Empty;

    [ObservableProperty]
    private bool _isNicknamed;

    [ObservableProperty]
    private uint _id32;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Sprite), nameof(Stat_HP), nameof(Stat_ATK), nameof(Stat_DEF), nameof(Stat_SPA), nameof(Stat_SPD), nameof(Stat_SPE), nameof(Base_HP), nameof(Base_ATK), nameof(Base_DEF), nameof(Base_SPA), nameof(Base_SPD), nameof(Base_SPE))]
    private int _form;

    [ObservableProperty]
    private string _nickname = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Title), nameof(Stat_HP), nameof(Stat_ATK), nameof(Stat_DEF), nameof(Stat_SPA), nameof(Stat_SPD), nameof(Stat_SPE), nameof(Exp), nameof(ExpPercent))]
    private int _level;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Stat_HP), nameof(Stat_ATK), nameof(Stat_DEF), nameof(Stat_SPA), nameof(Stat_SPD), nameof(Stat_SPE))]
    private int _nature;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Stat_HP), nameof(Stat_ATK), nameof(Stat_DEF), nameof(Stat_SPA), nameof(Stat_SPD), nameof(Stat_SPE))]
    private int _ability;

    [ObservableProperty]
    private int _heldItem;

    [ObservableProperty]
    private int _ball;

    [ObservableProperty]
    private int _gender;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Sprite))]
    private bool _isShiny;

    [ObservableProperty]
    private bool _isEgg;

    [ObservableProperty]
    private bool _isFatefulEncounter;

    [ObservableProperty]
    private int _sid;

    [ObservableProperty]
    private int _language;

    public bool HasForms => FormList.Count > 1;
    public PKM TargetPKM => _pk;

    public PokemonEditorViewModel(PKM pk, SaveFile sav, ISpriteRenderer spriteRenderer, IDialogService dialogService, IWindowService windowService)
    {
        _pk = pk.Clone(); // Always work on a copy
        _sav = sav;
        _spriteRenderer = spriteRenderer;
        _dialogService = dialogService;
        _windowService = windowService;

        var filtered = GameInfo.FilteredSources;
        SpeciesList = filtered.Species;
        MoveList = filtered.Moves;
        NatureList = filtered.Natures;
        BallList = filtered.Balls;
        ItemList = filtered.Items;
        OriginGameList = filtered.Games;
        RelearnMoveDataSource = filtered.Relearn;
        LanguageList = GameInfo.Sources.LanguageDataSource(sav.Generation, sav.Context);
        LoadFromPKM();
    }

    public void LoadPKM(PKM pk)
    {
        ArgumentNullException.ThrowIfNull(pk);
        _pk = pk.Clone();
        LoadFromPKM();
    }

    /// <summary>Raised when a dropped OS file turns out to be a save file, so the host can open it.</summary>
    public event Action<string>? SaveFileDropRequested;

    /// <summary>
    /// Handles OS file(s) dropped directly onto the editor panel. A Pokémon entity file loads
    /// into the editor (without writing to any box/party slot); a save file is routed through the
    /// host's "open save" path. Reuses the same detection/conversion pipeline as folder import.
    /// </summary>
    public async Task HandleFileDropAsync(IReadOnlyList<string> paths)
    {
        foreach (var path in paths)
        {
            var result = new ImportEntityFileUseCase().Execute(_sav, path);
            switch (result.Kind)
            {
                case EntityFileDropKind.SaveFile:
                    SaveFileDropRequested?.Invoke(path);
                    return;
                case EntityFileDropKind.Entity:
                    LoadPKM(result.Entity!);
                    return;
            }
        }

        if (paths.Count > 0)
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["PokemonEditor_ImportFailedTitle"], LocalizedStrings.Instance["PokemonEditor_NoSupportedFileFound"]);
    }

    public void RefreshLanguage()
    {
        var filtered = GameInfo.FilteredSources;
        SpeciesList = filtered.Species;
        MoveList = filtered.Moves;
        NatureList = filtered.Natures;
        BallList = filtered.Balls;
        ItemList = filtered.Items;
        OriginGameList = filtered.Games;
        RelearnMoveDataSource = filtered.Relearn;
        LanguageList = GameInfo.Sources.LanguageDataSource(_sav.Generation, _sav.Context);

        // Notify that the PKM name etc might have changed
        LoadFromPKM();
    }

    private void LoadFromPKM()
    {
        _isLoading = true;
        try
        {
            Species = _pk.Species;
            Form = _pk.Form;

            // Populate dynamic lists before setting their selected values
            UpdateFormList(false);
            UpdateAbilityList(false);

            Ability = _pk.Ability;
            Nickname = _pk.Nickname;
            IsNicknamed = _pk.IsNicknamed;
            Level = _pk.CurrentLevel;
            Nature = (int)_pk.Nature;
            HeldItem = _pk.HeldItem;
            Ball = _pk.Ball;
            Gender = _pk.Gender;
            IsShiny = _pk.IsShiny;
            IsEgg = _pk.IsEgg;

            Id32 = _pk.ID32;
            Version = _pk.Version.ToString();

            Move1 = _pk.Move1;
            Move2 = _pk.Move2;
            Move3 = _pk.Move3;
            Move4 = _pk.Move4;

            RelearnMove1 = _pk.RelearnMove1;
            RelearnMove2 = _pk.RelearnMove2;
            RelearnMove3 = _pk.RelearnMove3;
            RelearnMove4 = _pk.RelearnMove4;

            Pp1 = _pk.Move1_PP;
            Pp2 = _pk.Move2_PP;
            Pp3 = _pk.Move3_PP;
            Pp4 = _pk.Move4_PP;
            PpUps1 = _pk.Move1_PPUps;
            PpUps2 = _pk.Move2_PPUps;
            PpUps3 = _pk.Move3_PPUps;
            PpUps4 = _pk.Move4_PPUps;

            IvHP = _pk.IV_HP;
            IvATK = _pk.IV_ATK;
            IvDEF = _pk.IV_DEF;
            IvSPA = _pk.IV_SPA;
            IvSPD = _pk.IV_SPD;
            IvSPE = _pk.IV_SPE;

            EvHP = _pk.EV_HP;
            EvATK = _pk.EV_ATK;
            EvDEF = _pk.EV_DEF;
            EvSPA = _pk.EV_SPA;
            EvSPD = _pk.EV_SPD;
            EvSPE = _pk.EV_SPE;

            // Hyper Training (Partial)
            if (_pk is IHyperTrain ht)
            {
                HyperTrainedHP = ht.HT_HP;
                HyperTrainedATK = ht.HT_ATK;
                HyperTrainedDEF = ht.HT_DEF;
                HyperTrainedSPA = ht.HT_SPA;
                HyperTrainedSPD = ht.HT_SPD;
                HyperTrainedSPE = ht.HT_SPE;
            }

            IsFatefulEncounter = _pk.FatefulEncounter;
            Happiness = _pk.CurrentFriendship;
            Sid = (int)_pk.DisplaySID;

            Pid = _pk.PID.ToString("X8");
            EncryptionConstant = _pk.EncryptionConstant.ToString("X8");
            Exp = _pk.EXP;
            Language = _pk.Language;
            PkrsStrain = _pk.PokerusStrain;
            PkrsDays = _pk.PokerusDays;

            OriginGame = (int)_pk.Version;
            UpdateMetDataLists(false);
            MetLocation = _pk.MetLocation;
            EggLocation = _pk.EggLocation;
            MetLevel = _pk.MetLevel;

            OriginalTrainerName = _pk.OriginalTrainerName;
            TrainerID = _pk.DisplayTID;
            OriginalTrainerGender = _pk.OriginalTrainerGender;

            StatHPCurrent = _pk.Stat_HPCurrent;
            StatHPMax = _pk.Stat_HPMax;
            StatusCondition = _pk.Status_Condition;
            StatAlignment = (int)_pk.StatAlignment;

            // Trust any year in a broad valid range (Gen 3+ era: 2000–2199, with pre-2000 fallback)
            MetDate = _pk.MetDate is { } md && md.Year > 1900 && md.Year < 2200
                ? md.ToDateTime(TimeOnly.MinValue)
                : null;
            EggDate = _pk.EggMetDate is { } ed && ed.Year > 1900 && ed.Year < 2200
                ? ed.ToDateTime(TimeOnly.MinValue)
                : null;
            
            _isLoading = false;

            // Force notifications for list-bound properties that were set while loading was suppressed
            OnPropertyChanged(nameof(MetLocation));
            OnPropertyChanged(nameof(EggLocation));
            OnPropertyChanged(nameof(Ability));
            OnPropertyChanged(nameof(MetDate));
            OnPropertyChanged(nameof(EggDate));
            OnPropertyChanged(nameof(ShowStatAlignment));
            OnPropertyChanged(nameof(HasHyperTraining));
            OnPropertyChanged(nameof(CanHyperTrain));
            OnPropertyChanged(nameof(Tsv));
            UpdateFormArgument();

            OriginalTrainerFriendship = _pk.OriginalTrainerFriendship;
            HandlingTrainerFriendship = _pk.HandlingTrainerFriendship;
            CurrentHandler = _pk.CurrentHandler;
            HandlingTrainerName = _pk.HandlingTrainerName;
            HandlingTrainerGender = _pk.HandlingTrainerGender;

            // Misc
            AbilityNumber = _pk.AbilityNumber;
            HpType = _pk.HPType;
            IsPokerusInfected = _pk.IsPokerusInfected;
            IsPokerusCured = _pk.IsPokerusCured;

            // Contest Stats (Partial)
            if (_pk is IContestStatsReadOnly cs)
            {
                ContestCool = cs.ContestCool;
                ContestBeauty = cs.ContestBeauty;
                ContestCute = cs.ContestCute;
                ContestSmart = cs.ContestSmart;
                ContestTough = cs.ContestTough;
                ContestSheen = cs.ContestSheen;
            }

            // Markings (Partial)
            if (_pk is IAppliedMarkings3 m3)
            {
                MarkingCircle = m3.MarkingCircle;
                MarkingTriangle = m3.MarkingTriangle;
                MarkingSquare = m3.MarkingSquare;
                MarkingHeart = m3.MarkingHeart;
            }
            if (_pk is IAppliedMarkings4 m4)
            {
                MarkingStar = m4.MarkingStar;
                MarkingDiamond = m4.MarkingDiamond;
            }
            else if (_pk is IAppliedMarkings7 m7)
            {
                // For Gen 7+, convert color to boolean (any color = marked)
                MarkingCircle = m7.MarkingCircle != MarkingColor.None;
                MarkingTriangle = m7.MarkingTriangle != MarkingColor.None;
                MarkingSquare = m7.MarkingSquare != MarkingColor.None;
                MarkingHeart = m7.MarkingHeart != MarkingColor.None;
                MarkingStar = m7.MarkingStar != MarkingColor.None;
                MarkingDiamond = m7.MarkingDiamond != MarkingColor.None;
            }

            // Memories (Partial)
            if (_pk is IMemoryOT mot)
            {
                OtMemory = mot.OriginalTrainerMemory;
                OtMemoryIntensity = mot.OriginalTrainerMemoryIntensity;
                OtMemoryFeeling = mot.OriginalTrainerMemoryFeeling;
                OtMemoryVariable = mot.OriginalTrainerMemoryVariable;
            }
            if (_pk is IMemoryHT mht)
            {
                HtMemory = mht.HandlingTrainerMemory;
                HtMemoryIntensity = mht.HandlingTrainerMemoryIntensity;
                HtMemoryFeeling = mht.HandlingTrainerMemoryFeeling;
                HtMemoryVariable = mht.HandlingTrainerMemoryVariable;
            }

        UpdateTitle();
        }
        finally
        {
            _isLoading = false;
        }

        OnPropertyChanged(nameof(IVTotal));
        OnPropertyChanged(nameof(EVTotal));
        OnPropertyChanged(nameof(ExpPercent));
        UpdateSprite();
        Validate();
        LoadRibbons();
    }

    [ObservableProperty]
    private int _statusCondition;

    partial void OnFormChanged(int value)
    {
        if (_isLoading) return;
        RecalculateStats();
        UpdateAbilityList();
        UpdateFormArgument();
        UpdateSprite();
    }

    partial void OnIsShinyChanged(bool value)
    {
        if (_isLoading) return;
        if (value) _pk.SetShiny(); else _pk.SetUnshiny();
        _isLoading = true;
        Pid = _pk.PID.ToString("X8");
        _isLoading = false;
        UpdateSprite();
        Validate();
    }

    partial void OnPidChanged(string value)
    {
        if (_isLoading) return;
        if (uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out var pid))
        {
            _pk.PID = pid;
            _isLoading = true;
            IsShiny = _pk.IsShiny;
            _isLoading = false;
            UpdateSprite();
            Validate();
        }
    }

    partial void OnEncryptionConstantChanged(string value)
    {
        if (_isLoading) return;
        if (uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out var ec))
        {
            _pk.EncryptionConstant = ec;
            Validate();
        }
    }

    partial void OnNicknameChanged(string value) { if (!_isLoading) Validate(); }
    partial void OnLevelChanged(int value) { if (!_isLoading) { Exp = Experience.GetEXP((byte)value, _pk.PersonalInfo.EXPGrowth); RecalculateStats(); Validate(); OnPropertyChanged(nameof(CanHyperTrain)); } }
    partial void OnNatureChanged(int value) { if (!_isLoading) { RecalculateStats(); Validate(); } }
    partial void OnAbilityChanged(int value) { if (!_isLoading) Validate(); }
    partial void OnHeldItemChanged(int value) { if (!_isLoading) Validate(); }
    partial void OnBallChanged(int value) { if (!_isLoading) Validate(); }
    partial void OnGenderChanged(int value) { if (!_isLoading) Validate(); }

    private void UpdateAbilityList(bool preserveSelection = true)
    {
        var currentAbility = Ability;
        var pi = _sav.Personal.GetFormEntry((ushort)Species, (byte)Form);
        AbilityList = new ObservableCollection<ComboItem>(GameInfo.FilteredSources.GetAbilityList(pi));

        if (_isLoading || !preserveSelection) return;

        Ability = AbilityList.Any(a => a.Value == currentAbility)
            ? currentAbility
            : AbilityList.Count > 0 ? AbilityList[0].Value : 0;
    }

    private void UpdateFormList(bool preserveSelection = true)
    {
        var currentForm = Form;
        var forms = FormConverter.GetFormList((ushort)Species, GameInfo.Strings.Types, GameInfo.Strings.forms, GameInfo.GenderSymbolASCII, _sav.Context);

        var newList = new ObservableCollection<ComboItem>();
        for (int i = 0; i < forms.Length; i++)
            newList.Add(new ComboItem(string.IsNullOrWhiteSpace(forms[i]) ? $"Form {i}" : forms[i], i));

        if (newList.Count == 0)
            newList.Add(new ComboItem("Normal", 0));

        FormList = newList;

        if (_isLoading || !preserveSelection) return;

        Form = FormList.Any(f => f.Value == currentForm)
            ? currentForm
            : FormList.Count > 0 ? FormList[0].Value : 0;

        OnPropertyChanged(nameof(HasForms));
    }

    /// <summary>Re-renders the editor preview sprite (e.g. after the sprite style changes).</summary>
    public void RefreshSprite() => UpdateSprite();

    private void UpdateSprite()
    {
        _pk.Species = (ushort)Species;
        _pk.Form = (byte)Form;
        if (IsShiny) _pk.SetShiny(); else _pk.SetUnshiny();
        Sprite = _spriteRenderer.GetSprite(_pk);
    }

    private void UpdateTitle()
    {
        var speciesName = StringResourceLookup.Species((ushort)Species);
        Title = Species == 0 ? LocalizedStrings.Instance["PokemonEditor_EmptySlot"] : LocalizedStrings.Instance.Format("PokemonEditor_EditingSpecies", speciesName);
    }

    [RelayCommand]
    private void ToggleShiny()
    {
        IsShiny = !IsShiny;
    }

    [RelayCommand]
    private void RerollPid()
    {
        var rnd = new Random();
        var newPid = EntityPID.GetRandomPID(rnd, (ushort)Species, (byte)Gender, _pk.Version, (Nature)Nature, (byte)Form, _pk.PID);
        _pk.PID = newPid;
        _isLoading = true;
        Pid = newPid.ToString("X8");
        IsShiny = _pk.IsShiny;
        _isLoading = false;
        UpdateSprite();
        Validate();
    }

    public PKM PreparePKM()
    {
        _pk.Species = (ushort)Species;
        _pk.Form = (byte)Form;
        _pk.Nickname = Nickname;
        _pk.Stat_Level = (byte)Level;
        _pk.StatAlignment = (Nature)StatAlignment;
        _pk.Nature = (Nature)Nature;
        _pk.Ability = Ability;
        _pk.HeldItem = HeldItem;
        _pk.Ball = (byte)Ball;
        _pk.Gender = (byte)Gender;
        _pk.IsEgg = IsEgg;

        if (IsShiny)
            _pk.SetShiny();
        else
            _pk.SetUnshiny();

        _pk.Move1 = (ushort)Move1;
        _pk.Move2 = (ushort)Move2;
        _pk.Move3 = (ushort)Move3;
        _pk.Move4 = (ushort)Move4;

        _pk.RelearnMove1 = (ushort)RelearnMove1;
        _pk.RelearnMove2 = (ushort)RelearnMove2;
        _pk.RelearnMove3 = (ushort)RelearnMove3;
        _pk.RelearnMove4 = (ushort)RelearnMove4;

        _pk.Move1_PP = Pp1;
        _pk.Move2_PP = Pp2;
        _pk.Move3_PP = Pp3;
        _pk.Move4_PP = Pp4;
        _pk.Move1_PPUps = PpUps1;
        _pk.Move2_PPUps = PpUps2;
        _pk.Move3_PPUps = PpUps3;
        _pk.Move4_PPUps = PpUps4;

        _pk.IV_HP = IvHP;
        _pk.IV_ATK = IvATK;
        _pk.IV_DEF = IvDEF;
        _pk.IV_SPA = IvSPA;
        _pk.IV_SPD = IvSPD;
        _pk.IV_SPE = IvSPE;

        _pk.EV_HP = EvHP;
        _pk.EV_ATK = EvATK;
        _pk.EV_DEF = EvDEF;
        _pk.EV_SPA = EvSPA;
        _pk.EV_SPD = EvSPD;
        _pk.EV_SPE = EvSPE;

        _pk.OriginalTrainerName = OriginalTrainerName;
        _pk.OriginalTrainerGender = (byte)OriginalTrainerGender;
        _pk.DisplayTID = (uint)TrainerID;
        _pk.DisplaySID = (uint)Sid;
        _pk.CurrentFriendship = (byte)Happiness;
        _pk.FatefulEncounter = IsFatefulEncounter;

        if (uint.TryParse(Pid, System.Globalization.NumberStyles.HexNumber, null, out var pid))
            _pk.PID = pid;
        if (uint.TryParse(EncryptionConstant, System.Globalization.NumberStyles.HexNumber, null, out var ec))
            _pk.EncryptionConstant = ec;

        _pk.EXP = (uint)Exp;
        _pk.Language = Language;
        _pk.PokerusStrain = PkrsStrain;
        _pk.PokerusDays = PkrsDays;

        _pk.Version = (GameVersion)OriginGame;
        _pk.MetLocation = (ushort)MetLocation;
        _pk.EggLocation = (ushort)EggLocation;
        _pk.MetLevel = (byte)MetLevel;

        _pk.MetDate = MetDate is { } md ? new DateOnly(md.Year, md.Month, md.Day) : null;
        _pk.EggMetDate = EggDate is { } ed ? new DateOnly(ed.Year, ed.Month, ed.Day) : null;

        if (_pk is IContestStats cs)
        {
            cs.ContestCool = (byte)ContestCool;
            cs.ContestBeauty = (byte)ContestBeauty;
            cs.ContestCute = (byte)ContestCute;
            cs.ContestSmart = (byte)ContestSmart;
            cs.ContestTough = (byte)ContestTough;
            cs.ContestSheen = (byte)ContestSheen;
        }

        if (_pk is IAppliedMarkings3 m3)
        {
            m3.MarkingCircle = MarkingCircle;
            m3.MarkingTriangle = MarkingTriangle;
            m3.MarkingSquare = MarkingSquare;
            m3.MarkingHeart = MarkingHeart;
        }
        if (_pk is IAppliedMarkings4 m4)
        {
            m4.MarkingStar = MarkingStar;
            m4.MarkingDiamond = MarkingDiamond;
        }
        else if (_pk is IAppliedMarkings7 m7)
        {
            // For Gen 7+, set Blue color if marked, None if not
            m7.MarkingCircle = MarkingCircle ? MarkingColor.Blue : MarkingColor.None;
            m7.MarkingTriangle = MarkingTriangle ? MarkingColor.Blue : MarkingColor.None;
            m7.MarkingSquare = MarkingSquare ? MarkingColor.Blue : MarkingColor.None;
            m7.MarkingHeart = MarkingHeart ? MarkingColor.Blue : MarkingColor.None;
            m7.MarkingStar = MarkingStar ? MarkingColor.Blue : MarkingColor.None;
            m7.MarkingDiamond = MarkingDiamond ? MarkingColor.Blue : MarkingColor.None;
        }

        if (_pk is IMemoryOT mot)
        {
            mot.OriginalTrainerMemory = (byte)OtMemory;
            mot.OriginalTrainerMemoryIntensity = (byte)OtMemoryIntensity;
            mot.OriginalTrainerMemoryFeeling = (byte)OtMemoryFeeling;
            mot.OriginalTrainerMemoryVariable = (ushort)OtMemoryVariable;
        }
        if (_pk is IMemoryHT mht)
        {
            mht.HandlingTrainerMemory = (byte)HtMemory;
            mht.HandlingTrainerMemoryIntensity = (byte)HtMemoryIntensity;
            mht.HandlingTrainerMemoryFeeling = (byte)HtMemoryFeeling;
            mht.HandlingTrainerMemoryVariable = (ushort)HtMemoryVariable;
        }

        if (_pk is IFormArgument && _formArgumentType != FormArgumentType.None)
            _pk.ChangeFormArgument((uint)FormArgumentValue);

        _pk.ResetPartyStats();
        
        return _pk.Clone();
    }

    [RelayCommand]
    private async Task OpenRibbonEditorAsync()
    {
        var vm = new RibbonEditorViewModel(_pk);
        var view = vm;
        await _windowService.ShowDialogAsync(view, LocalizedStrings.Instance["PokemonEditor_RibbonEditorTitle"]);
        LoadRibbons();
        OnPropertyChanged(nameof(RibbonCount));
    }

    [RelayCommand]
    private async Task OpenMemoryEditorAsync()
    {
        var vm = new MemoryEditorViewModel(_pk);
        var view = vm;
        await _windowService.ShowDialogAsync(view, LocalizedStrings.Instance["PokemonEditor_MemoryEditorTitle"]);
    }

    [RelayCommand]
    private async Task OpenTechRecordEditorAsync()
    {
        if (_pk is not ITechRecord tr) return;

        var vm = new TechRecordEditorViewModel(tr, _pk);
        var view = vm;
        await _windowService.ShowDialogAsync(view, LocalizedStrings.Instance["PokemonEditor_TechnicalRecordEditorTitle"]);
    }

    public bool CanOpenTechRecord => _pk is ITechRecord;

    partial void OnSpeciesChanged(int value)
    {
        if (_isLoading) return;
        RecalculateStats();
        UpdateFormList();
        UpdateAbilityList();
        UpdateFormArgument();
        UpdateSprite(); // sets _pk.Species so the new growth rate is used below
        UpdateTitle();
        OnPropertyChanged(nameof(ExpPercent));
        OnPropertyChanged(nameof(CanOpenTechRecord));
    }
}
