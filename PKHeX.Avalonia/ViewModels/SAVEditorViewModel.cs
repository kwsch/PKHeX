using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Avalonia.Controls;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite.Avalonia;

namespace PKHeX.Avalonia.ViewModels;

/// <summary>
/// ViewModel for the SAV editor panel. Manages box/party display.
/// </summary>
public partial class SAVEditorViewModel : ObservableObject
{
    private SaveFile? _sav;

    /// <summary>Gets the currently loaded <see cref="SaveFile"/>.</summary>
    public SaveFile? SAV => _sav;

    [ObservableProperty]
    private string _boxName = "Box 1";

    [ObservableProperty]
    private int _currentBox;

    [ObservableProperty]
    private int _boxCount;

    [ObservableProperty]
    private bool _isLoaded;

    public ObservableCollection<SlotModel> BoxSlots { get; } = [];
    public ObservableCollection<SlotModel> PartySlots { get; } = [];

    /// <summary>Manages drag-and-drop operations between slots.</summary>
    public SlotChangeManager SlotManager { get; }

    public SAVEditorViewModel()
    {
        SlotManager = new SlotChangeManager(this);

        // Initialize 30 box slots
        for (int i = 0; i < 30; i++)
            BoxSlots.Add(new SlotModel { Slot = i });

        // Initialize 6 party slots
        for (int i = 0; i < 6; i++)
            PartySlots.Add(new SlotModel { Slot = i });
    }

    public void LoadSaveFile(SaveFile sav)
    {
        _sav = sav;
        BoxCount = sav.BoxCount;
        CurrentBox = 0;
        IsLoaded = true;
        RefreshBox();
        RefreshParty();
    }

    [RelayCommand]
    private void NextBox()
    {
        if (_sav is null)
            return;
        CurrentBox = (CurrentBox + 1) % BoxCount;
        RefreshBox();
    }

    [RelayCommand]
    private void PreviousBox()
    {
        if (_sav is null)
            return;
        CurrentBox = (CurrentBox - 1 + BoxCount) % BoxCount;
        RefreshBox();
    }

    private void RefreshBox()
    {
        if (_sav is null)
            return;

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

    private void RefreshParty()
    {
        if (_sav is null)
            return;

        for (int i = 0; i < _sav.PartyCount && i < PartySlots.Count; i++)
        {
            var pk = _sav.GetPartySlotAtIndex(i);
            if (pk.Species == 0)
            {
                PartySlots[i].SetImage(SpriteUtil.Spriter.None);
                PartySlots[i].IsEmpty = true;
            }
            else
            {
                var sprite = pk.Sprite();
                PartySlots[i].SetImage(sprite);
                PartySlots[i].IsEmpty = false;
            }
        }

        // Clear remaining party slots
        for (int i = _sav?.PartyCount ?? 0; i < PartySlots.Count; i++)
        {
            PartySlots[i].SetImage(null);
            PartySlots[i].IsEmpty = true;
        }
    }

    public void ReloadSlots()
    {
        RefreshBox();
        RefreshParty();
    }

    public PKM? GetSlotPKM(SlotModel slot)
    {
        if (_sav is null)
            return null;

        var boxIndex = BoxSlots.IndexOf(slot);
        if (boxIndex >= 0)
            return _sav.GetBoxSlotAtIndex(CurrentBox, boxIndex);

        var partyIndex = PartySlots.IndexOf(slot);
        if (partyIndex >= 0)
            return _sav.GetPartySlotAtIndex(partyIndex);

        return null;
    }

    /// <summary>Callback invoked when a slot is clicked to view its PKM.</summary>
    public Action<PKM>? SlotSelected { get; set; }

    /// <summary>Callback to get the PKM currently in the editor (for Set operations).</summary>
    public Func<PKM?>? GetEditorPKM { get; set; }

    [RelayCommand]
    private void ViewSlot(SlotModel? slot)
    {
        if (slot is null || _sav is null)
            return;
        var pk = GetSlotPKM(slot);
        if (pk is null || pk.Species == 0)
            return;
        SlotSelected?.Invoke(pk);
    }

    [RelayCommand]
    private void SetSlot(SlotModel? slot)
    {
        if (slot is null || _sav is null)
            return;
        var pk = GetEditorPKM?.Invoke();
        if (pk is null)
            return;

        var boxIndex = BoxSlots.IndexOf(slot);
        if (boxIndex >= 0)
        {
            _sav.SetBoxSlotAtIndex(pk, CurrentBox, boxIndex);
            RefreshBox();
            return;
        }

        var partyIndex = PartySlots.IndexOf(slot);
        if (partyIndex >= 0)
        {
            _sav.SetPartySlotAtIndex(pk, partyIndex);
            RefreshParty();
        }
    }

    [RelayCommand]
    private void DeleteSlot(SlotModel? slot)
    {
        if (slot is null || _sav is null)
            return;

        var boxIndex = BoxSlots.IndexOf(slot);
        if (boxIndex >= 0)
        {
            _sav.SetBoxSlotAtIndex(_sav.BlankPKM, CurrentBox, boxIndex);
            RefreshBox();
            return;
        }

        var partyIndex = PartySlots.IndexOf(slot);
        if (partyIndex >= 0)
        {
            _sav.DeletePartySlot(partyIndex);
            RefreshParty();
        }
    }
}
