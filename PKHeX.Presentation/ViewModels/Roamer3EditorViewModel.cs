using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Roamer editor for Gen 3 saves.
/// Edits the roaming legendary (Latios/Latias in RSE, Beasts in FRLG).
/// </summary>
public partial class Roamer3EditorViewModel : ViewModelBase
{
    private readonly SAV3 _sav;
    private readonly Roamer3 _roamer;

    [ObservableProperty]
    private ObservableCollection<ComboItem> _speciesList = [];

    [ObservableProperty]
    private int _selectedSpeciesIndex;

    [ObservableProperty]
    private string _pidHex = string.Empty;

    [ObservableProperty]
    private bool _isShiny;

    [ObservableProperty]
    private bool _isActive;

    [ObservableProperty]
    private byte _currentLevel;

    [ObservableProperty]
    private ushort _currentHp;

    // IVs
    [ObservableProperty]
    private int _ivHp;

    [ObservableProperty]
    private int _ivAtk;

    [ObservableProperty]
    private int _ivDef;

    [ObservableProperty]
    private int _ivSpe;

    [ObservableProperty]
    private int _ivSpa;

    [ObservableProperty]
    private int _ivSpd;

    public bool IsGlitched => _roamer.IsGlitched;
    public string GlitchWarning => IsGlitched
        ? "Note: RS/FRLG have a bug where only 1 byte of IV data is loaded when encountering the roamer."
        : string.Empty;

    public Roamer3EditorViewModel(SAV3 sav)
    {
        _sav = sav;
        _roamer = new Roamer3(sav.LargeBlock);

        LoadSpeciesList();
        LoadData();
    }

    private void LoadSpeciesList()
    {
        SpeciesList.Clear();
        foreach (var species in GameInfo.FilteredSources.Species)
            SpeciesList.Add(species);
    }

    private void LoadData()
    {
        PidHex = _roamer.PID.ToString("X8");
        IsShiny = Roamer3.IsShiny(_roamer.PID, _sav);

        // Find species index
        var species = _roamer.Species;
        for (int i = 0; i < SpeciesList.Count; i++)
        {
            if (SpeciesList[i].Value == species)
            {
                SelectedSpeciesIndex = i;
                break;
            }
        }

        IsActive = _roamer.IsActive;
        CurrentLevel = _roamer.CurrentLevel;
        CurrentHp = _roamer.HP_Current;

        IvHp = _roamer.IV_HP;
        IvAtk = _roamer.IV_ATK;
        IvDef = _roamer.IV_DEF;
        IvSpe = _roamer.IV_SPE;
        IvSpa = _roamer.IV_SPA;
        IvSpd = _roamer.IV_SPD;
    }

    partial void OnPidHexChanged(string value)
    {
        if (uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out var pid))
        {
            IsShiny = Roamer3.IsShiny(pid, _sav);
        }
    }

    [RelayCommand]
    private void MaxIvs()
    {
        IvHp = 31;
        IvAtk = 31;
        IvDef = 31;
        IvSpe = 31;
        IvSpa = 31;
        IvSpd = 31;
    }

    [RelayCommand]
    private void RandomPid()
    {
        var rng = new System.Random();
        var pid = (uint)rng.Next() ^ ((uint)rng.Next() << 16);
        PidHex = pid.ToString("X8");
    }

    [RelayCommand]
    private void Save()
    {
        if (uint.TryParse(PidHex, System.Globalization.NumberStyles.HexNumber, null, out var pid))
            _roamer.PID = pid;

        if (SelectedSpeciesIndex >= 0 && SelectedSpeciesIndex < SpeciesList.Count)
            _roamer.Species = (ushort)SpeciesList[SelectedSpeciesIndex].Value;

        _roamer.IsActive = IsActive;
        _roamer.CurrentLevel = CurrentLevel;
        _roamer.HP_Current = CurrentHp;

        _roamer.IV_HP = IvHp;
        _roamer.IV_ATK = IvAtk;
        _roamer.IV_DEF = IvDef;
        _roamer.IV_SPE = IvSpe;
        _roamer.IV_SPA = IvSpa;
        _roamer.IV_SPD = IvSpd;
    }
}
