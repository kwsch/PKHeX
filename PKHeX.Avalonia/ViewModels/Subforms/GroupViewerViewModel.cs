using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single slot entry in a group.
/// </summary>
public class GroupSlotModel
{
    public int Index { get; }
    public ushort Species { get; }
    public string SpeciesName { get; }
    public int Level { get; }
    public bool IsEmpty { get; }

    public GroupSlotModel(int index, PKM pk)
    {
        Index = index;
        Species = pk.Species;
        IsEmpty = pk.Species == 0;
        var names = GameInfo.Strings.specieslist;
        SpeciesName = Species < names.Length ? names[Species] : $"Species {Species}";
        Level = pk.CurrentLevel;
    }

    public string DisplayText => IsEmpty ? $"Slot {Index + 1}: (empty)" : $"Slot {Index + 1}: {SpeciesName} Lv.{Level}";
}

/// <summary>
/// ViewModel for the Group Viewer subform.
/// Shows battle team groups and their Pokemon slots.
/// </summary>
public partial class GroupViewerViewModel : SaveEditorViewModelBase
{
    private readonly IReadOnlyList<SlotGroup> _groups;

    [ObservableProperty]
    private int _selectedGroupIndex;

    public ObservableCollection<string> GroupNames { get; } = [];
    public ObservableCollection<GroupSlotModel> CurrentSlots { get; } = [];

    public string WindowTitle { get; }

    public GroupViewerViewModel(SaveFile sav, IReadOnlyList<SlotGroup> groups) : base(sav)
    {
        _groups = groups;
        WindowTitle = $"Group Viewer ({sav.Version})";

        foreach (var g in groups)
            GroupNames.Add(g.GroupName);

        // Select first group with content
        int initial = GetFirstTeamWithContent(groups);
        _selectedGroupIndex = initial;
        LoadGroup(initial);
    }

    partial void OnSelectedGroupIndexChanged(int value)
    {
        LoadGroup(value);
    }

    private void LoadGroup(int index)
    {
        CurrentSlots.Clear();
        if (index < 0 || index >= _groups.Count)
            return;

        var group = _groups[index];
        for (int i = 0; i < group.Slots.Length; i++)
            CurrentSlots.Add(new GroupSlotModel(i, group.Slots[i]));
    }

    [RelayCommand]
    private void PreviousGroup()
    {
        if (SelectedGroupIndex > 0)
            SelectedGroupIndex--;
    }

    [RelayCommand]
    private void NextGroup()
    {
        if (SelectedGroupIndex < _groups.Count - 1)
            SelectedGroupIndex++;
    }

    private static int GetFirstTeamWithContent(IReadOnlyList<SlotGroup> groups)
    {
        for (int i = 0; i < groups.Count; i++)
        {
            if (groups[i].Slots.Any(z => z.Species != 0))
                return i;
        }
        return 0;
    }
}
