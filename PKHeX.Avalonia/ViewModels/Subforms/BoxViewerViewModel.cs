using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Avalonia.Controls;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite.Avalonia;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the secondary box viewer window.
/// Shows a box grid that can be navigated independently from the main editor.
/// </summary>
public partial class BoxViewerViewModel : ObservableObject
{
    private readonly SaveFile _sav;

    [ObservableProperty]
    private string _boxName = "Box 1";

    [ObservableProperty]
    private int _currentBox;

    [ObservableProperty]
    private int _boxCount;

    public ObservableCollection<SlotModel> BoxSlots { get; } = [];

    /// <summary>Callback invoked when a slot is clicked to view its PKM.</summary>
    public Action<PKM>? SlotSelected { get; set; }

    public BoxViewerViewModel(SaveFile sav, int initialBox = 0)
    {
        _sav = sav;
        BoxCount = sav.BoxCount;

        // Initialize 30 box slots
        for (int i = 0; i < 30; i++)
            BoxSlots.Add(new SlotModel { Slot = i });

        CurrentBox = Math.Clamp(initialBox, 0, BoxCount - 1);
        RefreshBox();
    }

    [RelayCommand]
    private void NextBox()
    {
        CurrentBox = (CurrentBox + 1) % BoxCount;
        RefreshBox();
    }

    [RelayCommand]
    private void PreviousBox()
    {
        CurrentBox = (CurrentBox - 1 + BoxCount) % BoxCount;
        RefreshBox();
    }

    public void RefreshBox()
    {
        BoxName = _sav is IBoxDetailNameRead n ? n.GetBoxName(CurrentBox) : $"Box {CurrentBox + 1}";

        int slotCount = Math.Min(30, _sav.BoxSlotCount);
        for (int i = 0; i < slotCount && i < BoxSlots.Count; i++)
        {
            var pk = _sav.GetBoxSlotAtIndex(CurrentBox, i);
            if (pk.Species == 0)
            {
                BoxSlots[i].SetImage(SpriteUtil.Spriter.None);
                BoxSlots[i].IsEmpty = true;
            }
            else
            {
                var sprite = pk.Sprite(_sav, CurrentBox, i);
                BoxSlots[i].SetImage(sprite);
                BoxSlots[i].IsEmpty = false;
            }
        }
    }

    [RelayCommand]
    private void ViewSlot(SlotModel? slot)
    {
        if (slot is null)
            return;

        var index = BoxSlots.IndexOf(slot);
        if (index < 0)
            return;

        var pk = _sav.GetBoxSlotAtIndex(CurrentBox, index);
        if (pk.Species == 0)
            return;
        SlotSelected?.Invoke(pk);
    }
}
