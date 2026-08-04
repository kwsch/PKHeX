using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class SecretBaseEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly ISpriteRenderer _spriteRenderer;
    private readonly SecretBaseManager3? _manager;

    public SecretBaseEditorViewModel(SaveFile sav, ISpriteRenderer spriteRenderer)
    {
        _sav = sav;
        _spriteRenderer = spriteRenderer;

        if (GetLargeHoenn(sav) is { } hoenn)
        {
            _manager = hoenn.SecretBases;
            IsSupported = true;
            LoadBases();
        }
    }


    private static ISaveBlock3LargeHoenn? GetLargeHoenn(SaveFile sav) => sav switch
    {
        SAV3RS rs => rs.LargeBlock,
        SAV3E e => e.LargeBlock,
        _ => null,
    };

    public bool IsSupported { get; }

    [ObservableProperty]
    private ObservableCollection<SecretBaseViewModel> _bases = [];

    [ObservableProperty]
    private SecretBaseViewModel? _selectedBase;

    partial void OnSelectedBaseChanged(SecretBaseViewModel? value)
    {
        if (value is not null)
            LoadTeam(value);
    }

    [ObservableProperty]
    private ObservableCollection<SecretBaseTeamMemberViewModel> _teamMembers = [];

    [ObservableProperty]
    private SecretBaseTeamMemberViewModel? _selectedTeamMember;

    private void LoadBases()
    {
        Bases.Clear();
        if (_manager is null) return;

        foreach (var baseData in _manager.Bases)
        {
            Bases.Add(new SecretBaseViewModel(baseData));
        }

        if (Bases.Count > 0)
            SelectedBase = Bases[0];
    }

    private void LoadTeam(SecretBaseViewModel baseVm)
    {
        TeamMembers.Clear();
        var team = baseVm.SecretBase.Team.Team;

        for (int i = 0; i < team.Length; i++)
        {
            var pkm = team[i];
            TeamMembers.Add(new SecretBaseTeamMemberViewModel(i, pkm, _spriteRenderer, _sav));
        }

        if (TeamMembers.Count > 0)
            SelectedTeamMember = TeamMembers[0];
    }

    [RelayCommand]
    private void Refresh()
    {
        LoadBases();
    }

    [RelayCommand]
    private void Save()
    {
        if (_manager is null) return;

        var bases = Bases.Select(b => b.SecretBase).ToList();
        _manager.Bases = bases;
        _manager.Save();
    }
}

public partial class SecretBaseViewModel : ViewModelBase
{
    public SecretBase3 SecretBase { get; }

    public SecretBaseViewModel(SecretBase3 secretBase)
    {
        SecretBase = secretBase;
        _trainerName = secretBase.OriginalTrainerName;
        _tid = secretBase.TID16;
        _sid = secretBase.SID16;
        _gender = secretBase.OriginalTrainerGender;
        _timesEntered = secretBase.TimesEntered;
        _battledToday = secretBase.BattledToday;
        _registered = secretBase.RegistryStatus == 1;
    }

    [ObservableProperty]
    private string _trainerName;

    partial void OnTrainerNameChanged(string value) => SecretBase.OriginalTrainerName = value;

    [ObservableProperty]
    private ushort _tid;

    partial void OnTidChanged(ushort value) => SecretBase.TID16 = value;

    [ObservableProperty]
    private ushort _sid;

    partial void OnSidChanged(ushort value) => SecretBase.SID16 = value;

    [ObservableProperty]
    private byte _gender;

    partial void OnGenderChanged(byte value) => SecretBase.OriginalTrainerGender = value;

    public string GenderSymbol => Gender == 0 ? "♂" : "♀";

    [ObservableProperty]
    private byte _timesEntered;

    partial void OnTimesEnteredChanged(byte value) => SecretBase.TimesEntered = value;

    [ObservableProperty]
    private bool _battledToday;

    partial void OnBattledTodayChanged(bool value) => SecretBase.BattledToday = value;

    [ObservableProperty]
    private bool _registered;

    partial void OnRegisteredChanged(bool value) => SecretBase.RegistryStatus = value ? 1 : 0;

    public string DisplayName => string.IsNullOrEmpty(TrainerName) ? "(Empty)" : TrainerName;
}

public partial class SecretBaseTeamMemberViewModel : ViewModelBase
{
    private readonly SecretBase3PKM _pkm;
    private readonly ISpriteRenderer _spriteRenderer;
    private readonly SaveFile _sav;

    public SecretBaseTeamMemberViewModel(int index, SecretBase3PKM pkm, ISpriteRenderer spriteRenderer, SaveFile sav)
    {
        Index = index;
        _pkm = pkm;
        _spriteRenderer = spriteRenderer;
        _sav = sav;

        _species = pkm.Species;
        _level = pkm.Level;
        _heldItem = pkm.HeldItem;
        _move1 = pkm.Move1;
        _move2 = pkm.Move2;
        _move3 = pkm.Move3;
        _move4 = pkm.Move4;
        _evAll = pkm.EVAll;
    }

    public int Index { get; }
    public string SlotLabel => $"Slot {Index + 1}";

    [ObservableProperty]
    private ushort _species;

    partial void OnSpeciesChanged(ushort value)
    {
        _pkm.Species = value;
        OnPropertyChanged(nameof(SpeciesName));
        OnPropertyChanged(nameof(Sprite));
    }

    [ObservableProperty]
    private byte _level;

    partial void OnLevelChanged(byte value) => _pkm.Level = value;

    [ObservableProperty]
    private ushort _heldItem;

    partial void OnHeldItemChanged(ushort value) => _pkm.HeldItem = value;

    [ObservableProperty]
    private ushort _move1;

    partial void OnMove1Changed(ushort value) => _pkm.Move1 = value;

    [ObservableProperty]
    private ushort _move2;

    partial void OnMove2Changed(ushort value) => _pkm.Move2 = value;

    [ObservableProperty]
    private ushort _move3;

    partial void OnMove3Changed(ushort value) => _pkm.Move3 = value;

    [ObservableProperty]
    private ushort _move4;

    partial void OnMove4Changed(ushort value) => _pkm.Move4 = value;

    [ObservableProperty]
    private byte _evAll;

    partial void OnEvAllChanged(byte value) => _pkm.EVAll = value;

    public string SpeciesName
    {
        get
        {
            if (Species == 0) return "(Empty)";
            var names = GameInfo.Strings.Species;
            return Species < names.Count ? names[Species] : $"Species #{Species}";
        }
    }

    public byte[]? Sprite
    {
        get
        {
            if (Species == 0) return null;
            var pk = _sav.BlankPKM;
            pk.Species = Species;
            return _spriteRenderer.GetSprite(pk);
        }
    }
}
