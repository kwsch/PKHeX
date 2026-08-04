using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class RoamerEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly Roamer3? _roamer3;

    public RoamerEditorViewModel(SaveFile sav)
    {
        _sav = sav;

        if (sav is SAV3 sav3)
        {
            _roamer3 = new Roamer3(sav3.LargeBlock);
            IsSupported = true;
            LoadData();
        }
    }

    public bool IsSupported { get; }

    [ObservableProperty]
    private string _pidHex = string.Empty;

    partial void OnPidHexChanged(string value)
    {
        if (_roamer3 is null) return;
        if (uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out var pid))
        {
            _roamer3.PID = pid;
            OnPropertyChanged(nameof(IsShiny));
        }
    }

    public bool IsShiny => _roamer3 is not null && _sav is SAV3 s3 && Roamer3.IsShiny(_roamer3.PID, s3);

    [ObservableProperty]
    private ushort _species;

    partial void OnSpeciesChanged(ushort value)
    {
        if (_roamer3 is not null)
        {
            _roamer3.Species = value;
            OnPropertyChanged(nameof(SpeciesName));
        }
    }

    public string SpeciesName
    {
        get
        {
            if (Species == 0) return "(None)";
            var names = GameInfo.Strings.Species;
            return Species < names.Count ? names[Species] : $"#{Species}";
        }
    }

    [ObservableProperty] private int _ivHp;
    [ObservableProperty] private int _ivAtk;
    [ObservableProperty] private int _ivDef;
    [ObservableProperty] private int _ivSpe;
    [ObservableProperty] private int _ivSpA;
    [ObservableProperty] private int _ivSpD;

    [ObservableProperty]
    private bool _active;

    partial void OnActiveChanged(bool value)
    {
        if (_roamer3 is not null)
            _roamer3.IsActive = value;
    }

    [ObservableProperty]
    private byte _currentLevel;

    partial void OnCurrentLevelChanged(byte value)
    {
        if (_roamer3 is not null)
            _roamer3.CurrentLevel = value;
    }

    [ObservableProperty]
    private ushort _currentHP;

    partial void OnCurrentHPChanged(ushort value)
    {
        if (_roamer3 is not null)
            _roamer3.HP_Current = value;
    }

    private void LoadData()
    {
        if (_roamer3 is null) return;

        PidHex = _roamer3.PID.ToString("X8");
        Species = _roamer3.Species;

        IvHp = _roamer3.IV_HP;
        IvAtk = _roamer3.IV_ATK;
        IvDef = _roamer3.IV_DEF;
        IvSpe = _roamer3.IV_SPE;
        IvSpA = _roamer3.IV_SPA;
        IvSpD = _roamer3.IV_SPD;

        Active = _roamer3.IsActive;
        CurrentLevel = _roamer3.CurrentLevel;
        CurrentHP = _roamer3.HP_Current;
    }

    [RelayCommand]
    private void Save()
    {
        if (_roamer3 is null) return;

        _roamer3.SetIVs([IvHp, IvAtk, IvDef, IvSpe, IvSpA, IvSpD]);
    }

    [RelayCommand]
    private void Refresh() => LoadData();
}
