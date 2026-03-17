using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Avalonia.Converters;
using PKHeX.Core;
using PKHeX.Drawing.Misc.Avalonia;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single mystery gift slot.
/// </summary>
public partial class GiftSlotModel : ObservableObject
{
    public int Index { get; }
    public DataMysteryGift Gift { get; private set; }

    [ObservableProperty]
    private Bitmap? _sprite;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private bool _isEmpty;

    public GiftSlotModel(int index, DataMysteryGift gift)
    {
        Index = index;
        Gift = gift;
        Refresh();
    }

    public void Refresh()
    {
        IsEmpty = Gift.IsEmpty;
        Description = IsEmpty ? $"Slot {Index + 1}: (empty)" : $"Slot {Index + 1}: {Gift.CardHeader}";
        var skBitmap = Gift.Sprite();
        var old = Sprite;
        Sprite = SKBitmapToAvaloniaBitmapConverter.ToAvaloniaBitmapAndDispose(skBitmap);
        old?.Dispose();
    }

    public void SetGift(DataMysteryGift gift)
    {
        Gift = gift;
        Refresh();
    }
}

/// <summary>
/// ViewModel for the Wondercard (Mystery Gift) editor subform.
/// </summary>
public partial class WondercardViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SaveFile _clone;
    private readonly IMysteryGiftStorage _cards;

    [ObservableProperty]
    private GiftSlotModel? _selectedSlot;

    [ObservableProperty]
    private string _detailText = string.Empty;

    /// <summary>All gift slot models.</summary>
    public ObservableCollection<GiftSlotModel> Slots { get; } = [];

    public WondercardViewModel(SaveFile sav) : base(sav)
    {
        _origin = sav;
        _clone = sav.Clone();
        _cards = GetMysteryGiftProvider(_clone);
        LoadSlots();

        if (Slots.Count > 0)
            SelectedSlot = Slots[0];
    }

    private void LoadSlots()
    {
        var count = _cards.GiftCountMax;
        for (int i = 0; i < count; i++)
        {
            var gift = _cards.GetMysteryGift(i);
            Slots.Add(new GiftSlotModel(i, gift));
        }
    }

    partial void OnSelectedSlotChanged(GiftSlotModel? value)
    {
        if (value is null)
        {
            DetailText = string.Empty;
            return;
        }

        if (value.Gift.IsEmpty)
        {
            DetailText = "(Empty slot)";
            return;
        }

        var lines = value.Gift.GetDescription();
        DetailText = string.Join("\n", lines);
    }

    /// <summary>
    /// Deletes the currently selected gift slot.
    /// </summary>
    [RelayCommand]
    private void DeleteSlot()
    {
        if (SelectedSlot is null)
            return;

        SelectedSlot.Gift.Clear();
        SelectedSlot.Refresh();
        OnSelectedSlotChanged(SelectedSlot);
    }

    /// <summary>
    /// Saves all gift slots back to the save file.
    /// </summary>
    [RelayCommand]
    private void Save()
    {
        for (int i = 0; i < Slots.Count && i < _cards.GiftCountMax; i++)
            _cards.SetMysteryGift(i, Slots[i].Gift);

        _origin.CopyChangesFrom(_clone);
        Modified = true;
    }

    private static IMysteryGiftStorage GetMysteryGiftProvider(SaveFile sav)
    {
        if (sav is IMysteryGiftStorageProvider provider)
            return provider.MysteryGiftStorage;
        throw new System.ArgumentException("Save file does not support Mystery Gifts.", nameof(sav));
    }
}
