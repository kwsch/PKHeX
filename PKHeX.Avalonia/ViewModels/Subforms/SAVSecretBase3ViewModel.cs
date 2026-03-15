using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Gen 3 Secret Base editor.
/// Allows viewing/editing secret base trainers and their team members.
/// </summary>
public partial class SAVSecretBase3ViewModel : SaveEditorViewModelBase
{
    private readonly SAV3 _sav;
    private readonly SecretBaseManager3 _manager;

    public ObservableCollection<string> BaseNames { get; } = [];
    public List<ComboItem> SpeciesList { get; }
    public List<ComboItem> ItemsList { get; }
    public List<ComboItem> MovesList { get; }

    [ObservableProperty]
    private int _selectedBaseIndex = -1;

    // Trainer fields
    [ObservableProperty]
    private string _trainerName = string.Empty;

    [ObservableProperty]
    private string _trainerTid = "0";

    [ObservableProperty]
    private string _trainerSid = "0";

    [ObservableProperty]
    private byte _timesEntered;

    [ObservableProperty]
    private string _trainerClass = string.Empty;

    [ObservableProperty]
    private bool _battledToday;

    [ObservableProperty]
    private bool _registered;

    [ObservableProperty]
    private int _trainerGender;

    // Team member fields
    [ObservableProperty]
    private int _teamMemberIndex = 1;

    [ObservableProperty]
    private int _selectedSpecies;

    [ObservableProperty]
    private string _pid = "00000000";

    [ObservableProperty]
    private int _selectedItem;

    [ObservableProperty]
    private int _move1;

    [ObservableProperty]
    private int _move2;

    [ObservableProperty]
    private int _move3;

    [ObservableProperty]
    private int _move4;

    [ObservableProperty]
    private byte _level;

    [ObservableProperty]
    private byte _evAll;

    [ObservableProperty]
    private bool _hasBases;

    public string[] GenderSymbols { get; } = ["Male", "Female"];

    public SAVSecretBase3ViewModel(SAV3 sav) : base(sav)
    {
        _sav = sav;
        var large = (ISaveBlock3LargeHoenn)sav.LargeBlock;
        _manager = large.SecretBases;

        var filtered = GameInfo.FilteredSources;
        SpeciesList = filtered.Species.ToList();
        ItemsList = filtered.Items.ToList();
        MovesList = filtered.Moves.ToList();

        foreach (var b in _manager.Bases)
            BaseNames.Add(b.OriginalTrainerName);

        HasBases = _manager.Count > 0;
        if (HasBases)
            SelectedBaseIndex = 0;
    }

    partial void OnSelectedBaseIndexChanged(int value)
    {
        if (value < 0 || value >= _manager.Bases.Count)
            return;
        LoadTrainer(_manager.Bases[value]);
    }

    partial void OnTeamMemberIndexChanged(int value)
    {
        if (SelectedBaseIndex < 0 || SelectedBaseIndex >= _manager.Bases.Count)
            return;
        var secret = _manager.Bases[SelectedBaseIndex];
        var idx = Math.Clamp(value - 1, 0, 5);
        LoadPkm(secret.Team.Team[idx]);
    }

    private void LoadTrainer(SecretBase3 trainer)
    {
        TrainerName = trainer.OriginalTrainerName;
        TrainerGender = trainer.OriginalTrainerGender;
        TrainerTid = trainer.TID16.ToString();
        TrainerSid = trainer.SID16.ToString();
        TimesEntered = trainer.TimesEntered;
        TrainerClass = trainer.OriginalTrainerClassName;
        BattledToday = trainer.BattledToday;
        Registered = trainer.RegistryStatus == 1;
        TeamMemberIndex = 1;
        LoadPkm(trainer.Team.Team[0]);
    }

    private void LoadPkm(SecretBase3PKM pk)
    {
        SelectedSpecies = pk.Species;
        Pid = pk.PID.ToString("X8");
        SelectedItem = pk.HeldItem;
        Move1 = pk.Move1;
        Move2 = pk.Move2;
        Move3 = pk.Move3;
        Move4 = pk.Move4;
        Level = pk.Level;
        EvAll = pk.EVAll;
    }

    [RelayCommand]
    private void UpdateTrainer()
    {
        if (SelectedBaseIndex < 0 || SelectedBaseIndex >= _manager.Bases.Count)
            return;
        var secret = _manager.Bases[SelectedBaseIndex];
        secret.OriginalTrainerName = TrainerName;
        secret.OriginalTrainerGender = (byte)TrainerGender;
        secret.TID16 = ushort.TryParse(TrainerTid, out var tid) ? tid : (ushort)0;
        secret.SID16 = ushort.TryParse(TrainerSid, out var sid) ? sid : (ushort)0;
        secret.TimesEntered = TimesEntered;
        secret.BattledToday = BattledToday;
        secret.RegistryStatus = Registered ? 1 : 0;

        BaseNames[SelectedBaseIndex] = TrainerName;
    }

    [RelayCommand]
    private void UpdatePkm()
    {
        if (SelectedBaseIndex < 0 || SelectedBaseIndex >= _manager.Bases.Count)
            return;
        var secret = _manager.Bases[SelectedBaseIndex];
        var pkmteam = secret.Team;
        var idx = Math.Clamp(TeamMemberIndex - 1, 0, 5);
        var pkm = pkmteam.Team[idx];

        pkm.Species = (ushort)SelectedSpecies;
        pkm.PID = Convert.ToUInt32(Pid, 16);
        pkm.HeldItem = (ushort)SelectedItem;
        pkm.Move1 = (ushort)Move1;
        pkm.Move2 = (ushort)Move2;
        pkm.Move3 = (ushort)Move3;
        pkm.Move4 = (ushort)Move4;
        pkm.Level = Level;
        pkm.EVAll = EvAll;
        secret.Team = pkmteam;
    }

    [RelayCommand]
    private void Save()
    {
        _manager.Save();
        Modified = true;
    }
}
