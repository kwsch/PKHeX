using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class HallOfFame3EditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;

    public HallOfFame3EditorViewModel(SaveFile sav)
    {
        _sav = sav;

        if (sav is SAV3 sav3)
        {
            IsSupported = true;
            LoadData(sav3);
        }
    }

    public bool IsSupported { get; }
    public string GameInfo => $"{_sav.Version} - Hall of Fame (50 entries max)";

    [ObservableProperty]
    private ObservableCollection<HallOfFame3EntryViewModel> _entries = [];

    private void LoadData(SAV3 sav3)
    {
        Entries.Clear();
        var hofEntries = HallFame3Entry.GetEntries(sav3);
        var speciesNames = Core.GameInfo.Strings.Species;

        for (int i = 0; i < hofEntries.Length; i++)
        {
            var entry = hofEntries[i];
            var team = entry.Team;

            // Check if entry has valid data (first member has species)
            if (team[0].Species == 0)
                continue;

            var entryVm = new HallOfFame3EntryViewModel(i + 1);
            foreach (var member in team)
            {
                if (member.Species == 0) continue;
                var name = member.Species < speciesNames.Count ? speciesNames[member.Species] : $"#{member.Species}";
                entryVm.Members.Add(new HallOfFame3MemberViewModel(name, member.Nickname, member.Level, member.IsShiny));
            }
            Entries.Add(entryVm);
        }
    }

    [RelayCommand]
    private void Refresh()
    {
        if (_sav is SAV3 sav3)
            LoadData(sav3);
    }
}

public partial class HallOfFame3EntryViewModel : ViewModelBase
{
    public HallOfFame3EntryViewModel(int clearNumber)
    {
        ClearNumber = clearNumber;
    }

    public int ClearNumber { get; }
    public string Title => $"Clear #{ClearNumber}";

    [ObservableProperty]
    private ObservableCollection<HallOfFame3MemberViewModel> _members = [];
}

public class HallOfFame3MemberViewModel
{
    public HallOfFame3MemberViewModel(string species, string nickname, int level, bool isShiny)
    {
        Species = species;
        Nickname = nickname;
        Level = level;
        IsShiny = isShiny;
    }

    public string Species { get; }
    public string Nickname { get; }
    public int Level { get; }
    public bool IsShiny { get; }

    public string Display => IsShiny ? $"★ {Nickname} ({Species}) Lv.{Level}" : $"{Nickname} ({Species}) Lv.{Level}";
}
