using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Gen 1 Hall of Fame viewer/editor.
/// Shows team entries and allows editing of species, level, and nickname.
/// </summary>
public partial class SAVHallOfFame1ViewModel : SaveEditorViewModelBase
{
    private readonly SAV1 _sav;
    private readonly HallOfFameReader1 _fame;
    private int _currentTeam = -1;
    private int _currentSlot = -1;
    private bool _loading;

    [ObservableProperty]
    private ObservableCollection<string> _teamEntries = [];

    [ObservableProperty]
    private int _selectedTeamIndex = -1;

    [ObservableProperty]
    private int _partyIndex = 1;

    [ObservableProperty]
    private byte _hallOfFameClears;

    [ObservableProperty]
    private string _teamSummary = string.Empty;

    [ObservableProperty]
    private int _selectedSpecies;

    [ObservableProperty]
    private byte _level;

    [ObservableProperty]
    private string _nickname = string.Empty;

    [ObservableProperty]
    private bool _isNicknamed;

    [ObservableProperty]
    private bool _canDelete;

    [ObservableProperty]
    private bool _canClearSlot;

    public List<ComboItem> SpeciesList { get; }
    public int MaxNicknameLength { get; }

    public SAVHallOfFame1ViewModel(SAV1 sav) : base(sav)
    {
        _sav = sav;
        _fame = sav.HallOfFame;
        MaxNicknameLength = sav.Japanese ? 5 : 10;

        SpeciesList = GameInfo.FilteredSources.Species.ToList();

        for (int i = 0; i < HallOfFameReader1.TeamCount; i++)
            TeamEntries.Add(GetTeamIndication(i));

        _hallOfFameClears = sav.HallOfFameCount;
        SelectedTeamIndex = 0;
    }

    partial void OnSelectedTeamIndexChanged(int value)
    {
        if (_loading || value < 0)
            return;
        SaveEntity();
        LoadEntity(value, PartyIndex - 1);
        _currentTeam = value;
        _currentSlot = PartyIndex - 1;
        UpdateTeamPreview(value);
        CanDelete = value > 0;
        CanClearSlot = (PartyIndex - 1) > 0;
    }

    partial void OnPartyIndexChanged(int value)
    {
        if (_loading || SelectedTeamIndex < 0)
            return;
        SaveEntity();
        var slot = value - 1;
        LoadEntity(SelectedTeamIndex, slot);
        _currentTeam = SelectedTeamIndex;
        _currentSlot = slot;
        UpdateTeamPreview(SelectedTeamIndex);
        CanClearSlot = slot > 0;
    }

    private string GetTeamIndication(int team)
    {
        var count = _fame.GetTeamMemberCount(team);
        var state = count switch
        {
            0 => "x",
            6 => "ok",
            _ => $"{count}/6",
        };
        return $"{team + 1:00} ({state})";
    }

    private void RefreshAllTeamEntries()
    {
        for (int i = 0; i < HallOfFameReader1.TeamCount; i++)
            TeamEntries[i] = GetTeamIndication(i);
    }

    private void UpdateTeamPreview(int team)
    {
        if (team < 0)
            return;
        TeamEntries[team] = GetTeamIndication(team);
        TeamSummary = _fame.GetTeamSummary(team, GameInfo.Strings.specieslist);
    }

    private void LoadEntity(int team, int slot)
    {
        if (team < 0 || slot < 0)
            return;
        _loading = true;
        var pk = _fame.GetEntity(team, slot);
        SelectedSpecies = (int)pk.Species;
        Level = pk.Level;
        Nickname = pk.Nickname;
        IsNicknamed = CheckIsNicknamed(pk.Species, pk.Nickname);
        _loading = false;
    }

    private void SaveEntity()
    {
        if (_currentTeam < 0 || _currentSlot < 0)
            return;
        var pk = _fame.GetEntity(_currentTeam, _currentSlot);

        var species = (ushort)SelectedSpecies;
        if (species is 0 or > 151)
        {
            pk.Clear();
            return;
        }

        pk.Species = species;
        pk.Level = Level;
        if (pk.Nickname != Nickname)
            pk.Nickname = Nickname;
    }

    private bool CheckIsNicknamed(ushort species, string nickname)
    {
        var expect = SpeciesName.GetSpeciesNameGeneration(species, _sav.Language, 1);
        return nickname != expect;
    }

    [RelayCommand]
    private void Delete()
    {
        if (_currentTeam <= 0)
            return;
        _loading = true;
        _fame.Delete(_currentTeam);
        RefreshAllTeamEntries();
        LoadEntity(_currentTeam, _currentSlot);
        UpdateTeamPreview(_currentTeam);
        _loading = false;
    }

    [RelayCommand]
    private void ClearSlot()
    {
        if (SelectedTeamIndex < 0)
            return;
        var entity = _fame.GetEntity(SelectedTeamIndex, PartyIndex - 1);
        entity.Clear();
        LoadEntity(SelectedTeamIndex, PartyIndex - 1);
        UpdateTeamPreview(SelectedTeamIndex);
    }

    [RelayCommand]
    private void SetParty()
    {
        _loading = true;
        var count = _fame.RegisterParty(_sav, _sav.HallOfFameCount);
        RefreshAllTeamEntries();
        HallOfFameClears = _sav.HallOfFameCount = count;
        _currentTeam = -1;
        _loading = false;

        var index = count - 1;
        SelectedTeamIndex = index;
    }

    [RelayCommand]
    private void ClearAll()
    {
        _fame.Clear();
        HallOfFameClears = 0;
        _sav.HallOfFameCount = 0;
        Modified = true;
    }

    [RelayCommand]
    private void Save()
    {
        SaveEntity();
        _sav.HallOfFameCount = HallOfFameClears;
        Modified = true;
    }
}
