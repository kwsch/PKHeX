using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Gen 3 Roaming Pokemon editor.
/// Edits species, PID, IVs, level, HP, and active status of roamer.
/// </summary>
public partial class SAVRoamer3ViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly Roamer3 _roamer;
    private readonly SAV3 _sav;

    public List<ComboItem> SpeciesList { get; }

    [ObservableProperty]
    private int _selectedSpecies;

    [ObservableProperty]
    private string _pid = "00000000";

    [ObservableProperty]
    private bool _isShiny;

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

    [ObservableProperty]
    private bool _active;

    [ObservableProperty]
    private byte _level;

    [ObservableProperty]
    private ushort _hpCurrent;

    public SAVRoamer3ViewModel(SAV3 sav) : base(sav)
    {
        _origin = sav;
        _sav = (SAV3)sav.Clone();
        _roamer = new Roamer3(_sav.LargeBlock);

        SpeciesList = GameInfo.FilteredSources.Species.ToList();
        LoadData();
    }

    private void LoadData()
    {
        Pid = _roamer.PID.ToString("X8");
        IsShiny = Roamer3.IsShiny(_roamer.PID, _sav);
        SelectedSpecies = _roamer.Species;
        IvHp = _roamer.IV_HP;
        IvAtk = _roamer.IV_ATK;
        IvDef = _roamer.IV_DEF;
        IvSpe = _roamer.IV_SPE;
        IvSpa = _roamer.IV_SPA;
        IvSpd = _roamer.IV_SPD;
        Active = _roamer.Active;
        Level = _roamer.CurrentLevel;
        HpCurrent = _roamer.HP_Current;
    }

    partial void OnPidChanged(string value)
    {
        if (!uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out var pid))
            return;
        IsShiny = Roamer3.IsShiny(pid, _sav);
    }

    [RelayCommand]
    private void Save()
    {
        if (!uint.TryParse(Pid, System.Globalization.NumberStyles.HexNumber, null, out var parsedPid))
            return;
        _roamer.PID = parsedPid;
        _roamer.Species = (ushort)SelectedSpecies;
        _roamer.SetIVs([IvHp, IvAtk, IvDef, IvSpe, IvSpa, IvSpd]);
        _roamer.Active = Active;
        _roamer.CurrentLevel = Level;
        _roamer.HP_Current = HpCurrent;
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}
