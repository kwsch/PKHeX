using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Drives an in-save "search and seek" tool: builds a predicate from <see cref="Filter"/>
/// and jumps the associated box viewer to the next/previous matching slot, wrapping across
/// boxes. Hosted in a modeless tool window so it can be operated while the box grid updates
/// live. The cross-platform equivalent of upstream's <c>EntitySearchControl</c>.
/// </summary>
public partial class EntitySeekViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly IBoxNavigator _navigator;

    /// <summary>Shared entity filter inputs + combo data sources (bound by the view).</summary>
    public EntityFilterViewModel Filter { get; }

    /// <summary>Feedback text for the most recent seek (e.g. "Found in Box 2" / "No matches").</summary>
    [ObservableProperty]
    private string _seekStatus = string.Empty;

    public EntitySeekViewModel(SaveFile sav, IBoxNavigator navigator)
    {
        _sav = sav;
        _navigator = navigator;
        Filter = new EntityFilterViewModel(sav);
    }

    [RelayCommand]
    private void SeekNext() => Seek(reverse: false);

    [RelayCommand]
    private void SeekPrevious() => Seek(reverse: true);

    /// <summary>
    /// Jumps to and highlights the next/previous box slot matching the current filter,
    /// wrapping across boxes. Party slots are excluded — they live in a separate viewer.
    /// </summary>
    private void Seek(bool reverse)
    {
        var predicate = Filter.CreateSearchPredicate();
        int perBox = _navigator.SlotsPerBox;
        int totalBoxSlots = _navigator.BoxCount * perBox;
        if (totalBoxSlots == 0)
        {
            SeekStatus = "No box slots.";
            return;
        }

        int step = reverse ? -1 : 1;
        int current = (_navigator.CurrentBox * perBox) + _navigator.CurrentSlot;

        for (int i = 1; i <= totalBoxSlots; i++)
        {
            int index = (((current + (i * step)) % totalBoxSlots) + totalBoxSlots) % totalBoxSlots;
            int box = index / perBox;
            int slot = index % perBox;

            var pk = _sav.GetBoxSlotAtIndex(box, slot);
            if (pk.Species == 0)
                continue;
            if (!predicate(pk))
                continue;

            _navigator.NavigateTo(box, slot);
            SeekStatus = $"Found in Box {box + 1} (slot {slot + 1}).";
            return;
        }

        SeekStatus = "No matches.";
    }
}
