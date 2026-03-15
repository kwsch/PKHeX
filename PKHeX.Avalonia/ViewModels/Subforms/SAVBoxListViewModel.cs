using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single box in the box list.
/// </summary>
public partial class BoxListEntry : ObservableObject
{
    public int BoxIndex { get; }
    public string BoxName { get; }
    public int SlotCount { get; }
    public int OccupiedCount { get; }

    public BoxListEntry(int boxIndex, string boxName, int slotCount, int occupiedCount)
    {
        BoxIndex = boxIndex;
        BoxName = boxName;
        SlotCount = slotCount;
        OccupiedCount = occupiedCount;
    }
}

/// <summary>
/// ViewModel for the SAV_BoxList subform.
/// Shows all boxes in a list for quick selection and navigation.
/// </summary>
public partial class SAVBoxListViewModel : ObservableObject
{
    private readonly SaveFile _sav;

    [ObservableProperty]
    private BoxListEntry? _selectedBox;

    /// <summary>All box entries.</summary>
    public ObservableCollection<BoxListEntry> Boxes { get; } = [];

    /// <summary>Callback invoked when a box is selected for navigation.</summary>
    public Action<int>? BoxSelected { get; set; }

    public SAVBoxListViewModel(SaveFile sav)
    {
        _sav = sav;
        Refresh();
    }

    /// <summary>
    /// Refreshes the box list from the save file.
    /// </summary>
    [RelayCommand]
    private void Refresh()
    {
        Boxes.Clear();
        for (int i = 0; i < _sav.BoxCount; i++)
        {
            var name = _sav is IBoxDetailNameRead n ? n.GetBoxName(i) : $"Box {i + 1}";
            int occupied = 0;
            for (int s = 0; s < _sav.BoxSlotCount; s++)
            {
                var pk = _sav.GetBoxSlotAtIndex(i, s);
                if (pk.Species != 0)
                    occupied++;
            }
            Boxes.Add(new BoxListEntry(i, name, _sav.BoxSlotCount, occupied));
        }
    }

    /// <summary>
    /// Navigates to the selected box.
    /// </summary>
    [RelayCommand]
    private void NavigateToBox()
    {
        if (SelectedBox is not null)
            BoxSelected?.Invoke(SelectedBox.BoxIndex);
    }
}
