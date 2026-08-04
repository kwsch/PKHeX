using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Hall of Fame editor for Gen 1 saves.
/// </summary>
public partial class HallOfFame1EditorViewModel : ViewModelBase
{
    private readonly SAV1 _sav;
    private readonly SAV1 _clone;
    private readonly HallOfFameReader1 _fame;

    [ObservableProperty]
    private ObservableCollection<HallOfFameTeam1ViewModel> _teams = [];

    [ObservableProperty]
    private HallOfFameTeam1ViewModel? _selectedTeam;

    [ObservableProperty]
    private int _selectedSlotIndex;

    [ObservableProperty]
    private byte _clearCount;

    [ObservableProperty]
    private ObservableCollection<ComboItem> _speciesList = [];

    // Current slot properties
    [ObservableProperty]
    private int _selectedSpeciesIndex;

    [ObservableProperty]
    private byte _level;

    [ObservableProperty]
    private string _nickname = string.Empty;

    [ObservableProperty]
    private string _teamSummary = string.Empty;

    public HallOfFame1EditorViewModel(SAV1 sav)
    {
        _sav = sav;
        _clone = (SAV1)sav.Clone();
        _fame = _clone.HallOfFame;

        ClearCount = _clone.HallOfFameCount;
        LoadSpeciesList();
        LoadTeams();
    }

    private void LoadSpeciesList()
    {
        SpeciesList.Clear();
        foreach (var species in GameInfo.FilteredSources.Species)
            SpeciesList.Add(species);
    }

    private void LoadTeams()
    {
        Teams.Clear();
        for (int i = 0; i < HallOfFameReader1.TeamCount; i++)
        {
            var count = _fame.GetTeamMemberCount(i);
            Teams.Add(new HallOfFameTeam1ViewModel(i, count));
        }

        if (Teams.Count > 0)
            SelectedTeam = Teams[0];
    }

    partial void OnSelectedTeamChanged(HallOfFameTeam1ViewModel? value)
    {
        if (value is null)
            return;

        SelectedSlotIndex = 0;
        UpdateTeamSummary();
        LoadSlot(value.TeamIndex, 0);
    }

    partial void OnSelectedSlotIndexChanged(int value)
    {
        if (SelectedTeam is null || value < 0 || value >= 6)
            return;

        LoadSlot(SelectedTeam.TeamIndex, value);
    }

    private void LoadSlot(int team, int slot)
    {
        var entity = _fame.GetEntity(team, slot);
        var species = entity.Species;
        var level = entity.Level;
        var nickname = entity.Nickname;

        var speciesIndex = -1;
        for (int i = 0; i < SpeciesList.Count; i++)
        {
            if (SpeciesList[i].Value == species)
            {
                speciesIndex = i;
                break;
            }
        }

        SelectedSpeciesIndex = speciesIndex >= 0 ? speciesIndex : 0;
        Level = level;
        Nickname = nickname;
    }

    private void SaveSlot()
    {
        if (SelectedTeam is null || SelectedSlotIndex < 0)
            return;

        var entity = _fame.GetEntity(SelectedTeam.TeamIndex, SelectedSlotIndex);

        if (SelectedSpeciesIndex >= 0 && SelectedSpeciesIndex < SpeciesList.Count)
        {
            var species = (ushort)SpeciesList[SelectedSpeciesIndex].Value;
            if (species is 0 or > 151)
            {
                entity.Clear();
            }
            else
            {
                entity.Species = species;
                entity.Level = Level;
                entity.Nickname = Nickname;
            }
        }

        UpdateTeamSummary();
        SelectedTeam.MemberCount = _fame.GetTeamMemberCount(SelectedTeam.TeamIndex);
    }

    private void UpdateTeamSummary()
    {
        if (SelectedTeam is null)
        {
            TeamSummary = string.Empty;
            return;
        }

        TeamSummary = _fame.GetTeamSummary(SelectedTeam.TeamIndex, GameInfo.Strings.specieslist);
    }

    [RelayCommand]
    private void ClearSlot()
    {
        if (SelectedTeam is null || SelectedSlotIndex < 0)
            return;

        var entity = _fame.GetEntity(SelectedTeam.TeamIndex, SelectedSlotIndex);
        entity.Clear();
        LoadSlot(SelectedTeam.TeamIndex, SelectedSlotIndex);
        UpdateTeamSummary();
        SelectedTeam.MemberCount = _fame.GetTeamMemberCount(SelectedTeam.TeamIndex);
    }

    [RelayCommand]
    private void DeleteTeam()
    {
        if (SelectedTeam is null || SelectedTeam.TeamIndex == 0)
            return;

        var index = SelectedTeam.TeamIndex;
        _fame.Delete(index);
        LoadTeams();

        if (index < Teams.Count)
            SelectedTeam = Teams[index];
        else if (Teams.Count > 0)
            SelectedTeam = Teams[^1];
    }

    [RelayCommand]
    private void SetParty()
    {
        var count = _fame.RegisterParty(_clone, _clone.HallOfFameCount);
        _clone.HallOfFameCount = count;
        ClearCount = count;
        LoadTeams();

        if (count > 0 && count <= Teams.Count)
            SelectedTeam = Teams[count - 1];
    }

    [RelayCommand]
    private void ClearAll()
    {
        _fame.Clear();
        ClearCount = 0;
        _clone.HallOfFameCount = 0;
        LoadTeams();
    }

    [RelayCommand]
    private void ApplySlotChanges()
    {
        SaveSlot();
    }

    [RelayCommand]
    private void Save()
    {
        SaveSlot();
        _clone.HallOfFameCount = ClearCount;
        _sav.CopyChangesFrom(_clone);
    }
}

public partial class HallOfFameTeam1ViewModel : ViewModelBase
{
    public int TeamIndex { get; }

    [ObservableProperty]
    private int _memberCount;

    public string DisplayText => $"{TeamIndex + 1:00} ({StateText})";

    private string StateText => MemberCount switch
    {
        0 => "✕",
        6 => "✓",
        _ => $"{MemberCount}/6",
    };

    public HallOfFameTeam1ViewModel(int teamIndex, int memberCount)
    {
        TeamIndex = teamIndex;
        _memberCount = memberCount;
    }

    partial void OnMemberCountChanged(int value)
    {
        OnPropertyChanged(nameof(DisplayText));
    }
}
