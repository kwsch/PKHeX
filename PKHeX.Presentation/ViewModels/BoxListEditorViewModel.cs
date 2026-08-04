using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Box List viewer for managing multiple boxes at once.
/// </summary>
public partial class BoxListEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;

    public BoxListEditorViewModel(SaveFile sav)
    {
        _sav = sav;
        BoxCount = sav.BoxCount;
        LoadBoxes();
    }

    public int BoxCount { get; }

    public ObservableCollection<BoxSummary> Boxes { get; } = [];

    [ObservableProperty] private BoxSummary? _selectedBox;

    private void LoadBoxes()
    {
        Boxes.Clear();
        for (int i = 0; i < _sav.BoxCount; i++)
        {
            var name = $"Box {i + 1}";
            int count = 0;
            for (int slot = 0; slot < _sav.BoxSlotCount; slot++)
            {
                var pk = _sav.GetBoxSlotAtIndex(i, slot);
                if (pk.Species > 0)
                    count++;
            }
            Boxes.Add(new BoxSummary(i, name, count, _sav.BoxSlotCount));
        }
    }

    [RelayCommand]
    private void Refresh()
    {
        LoadBoxes();
    }

    [RelayCommand]
    private void ClearSelectedBox()
    {
        if (SelectedBox is null) return;
        
        for (int slot = 0; slot < _sav.BoxSlotCount; slot++)
        {
            _sav.SetBoxSlotAtIndex(_sav.BlankPKM, SelectedBox.Index, slot);
        }
        LoadBoxes();
    }
}

public class BoxSummary
{
    public BoxSummary(int index, string name, int occupiedSlots, int totalSlots)
    {
        Index = index;
        Name = name;
        OccupiedSlots = occupiedSlots;
        TotalSlots = totalSlots;
    }

    public int Index { get; }
    public string Name { get; }
    public int OccupiedSlots { get; }
    public int TotalSlots { get; }

    public string Display => $"Box {Index + 1}: {Name} ({OccupiedSlots}/{TotalSlots})";
}
