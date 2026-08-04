using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Editor for the Gen 5 Global Link / Pokémon Dream World (PGL) save block (<see cref="GlobalLink5"/>),
/// reachable as <see cref="SAV5.GlobalLink"/>. Edits write through the live save buffer that the block wraps.
/// </summary>
public partial class GlobalLink5EditorViewModel : ViewModelBase
{
    private readonly SAV5? _sav;
    private readonly GlobalLink5? _block;
    private readonly ISpriteRenderer? _spriteRenderer;
    private readonly bool _loading;

    public bool IsSupported { get; }

    /// <summary>Human-readable description of the loaded game, or an unsupported notice.</summary>
    public string GameInfo { get; } = "No Generation 5 save loaded.";

    public GlobalLink5EditorViewModel(SAV5 sav, ISpriteRenderer? spriteRenderer = null)
    {
        _loading = true;
        _sav = sav;
        _spriteRenderer = spriteRenderer;
        _block = sav.GlobalLink;
        IsSupported = true;
        GameInfo = $"{sav.Version} — Global Link (Dream World)";

        // Counters / status
        _uploadCount = _block.UploadCount;
        _uploadStatus = _block.UploadStatus;

        // Flags
        _isSlotPresent = _block.IsSlotPresent;
        _isRegistered = _block.IsRegistered;
        _isAccountFullAccess = _block.IsAccountFullAccess;

        // Selected furniture / sync
        _selectedFurnitureIndex = _block.SelectedFurnitureIndex;
        _isFurnitureSynchronized = _block.IsFurnitureSynchronized;

        // Downloaded-content trackers
        _musical = _block.Musical;
        _cGearSkin = _block.CGearSkin;
        _dexSkin = _block.DexSkin;

        // Upload date (DateQuad5 has setters, but the block exposes it as a read-only computed
        // property over a fresh slice, so we surface it read-only here).
        var date = _block.UploadDate;
        UploadDate = date.IsEmpty ? "Never" : date.ToDateOnly().ToString("yyyy-MM-dd");

        // Build the item-name list for the combo. Global Link items are arbitrary held items,
        // so every id 0..MaxItemID is offered (mirrors InventoryEditor's name-list fallback).
        var itemStrings = Core.GameInfo.Strings.GetItemStrings(sav.Context, sav.Version);
        int max = sav.MaxItemID;
        var itemList = new List<ComboItem>(max + 1);
        for (int i = 0; i <= max && i < itemStrings.Length; i++)
        {
            var name = string.IsNullOrEmpty(itemStrings[i]) ? $"(Item #{i:000})" : itemStrings[i];
            itemList.Add(new ComboItem(name, i));
        }
        ItemList = itemList;

        for (int i = 0; i < GlobalLink5.CountItems; i++)
            Items.Add(new GlobalLinkItemViewModel(this, i, _block.GetItem(i), _block.GetItemQuantity(i), itemList, _spriteRenderer));

        for (int i = 0; i < GlobalLink5.CountFurniture; i++)
        {
            var furniture = _block.GetFurniture(i);
            Furniture.Add(new GlobalLinkFurnitureViewModel(this, i, furniture.Value, furniture.Name));
        }

        _loading = false;
    }

    // ---- Counters / status ----

    [ObservableProperty]
    private int _uploadCount;

    [ObservableProperty]
    private byte _uploadStatus;

    public string UploadDate { get; } = "Never";

    // ---- Flags ----

    [ObservableProperty]
    private bool _isSlotPresent;

    [ObservableProperty]
    private bool _isRegistered;

    [ObservableProperty]
    private bool _isAccountFullAccess;

    // ---- Selected furniture / sync ----

    [ObservableProperty]
    private byte _selectedFurnitureIndex;

    [ObservableProperty]
    private bool _isFurnitureSynchronized;

    // ---- Downloaded-content trackers ----

    [ObservableProperty]
    private byte _musical;

    [ObservableProperty]
    private byte _cGearSkin;

    [ObservableProperty]
    private byte _dexSkin;

    // ---- Item grid ----

    public IReadOnlyList<ComboItem> ItemList { get; } = [];

    [ObservableProperty]
    private ObservableCollection<GlobalLinkItemViewModel> _items = [];

    // ---- Furniture grid ----

    [ObservableProperty]
    private ObservableCollection<GlobalLinkFurnitureViewModel> _furniture = [];

    // ---- Write-through hooks ----

    partial void OnUploadCountChanged(int value) => Apply(b => b.UploadCount = value);
    partial void OnUploadStatusChanged(byte value) => Apply(b => b.UploadStatus = value);
    partial void OnIsSlotPresentChanged(bool value) => Apply(b => b.IsSlotPresent = value);
    partial void OnIsRegisteredChanged(bool value) => Apply(b => b.IsRegistered = value);
    partial void OnIsAccountFullAccessChanged(bool value) => Apply(b => b.IsAccountFullAccess = value);
    partial void OnSelectedFurnitureIndexChanged(byte value) => Apply(b => b.SelectedFurnitureIndex = value);
    partial void OnIsFurnitureSynchronizedChanged(bool value) => Apply(b => b.IsFurnitureSynchronized = value);
    partial void OnMusicalChanged(byte value) => Apply(b => b.Musical = value);
    partial void OnCGearSkinChanged(byte value) => Apply(b => b.CGearSkin = value);
    partial void OnDexSkinChanged(byte value) => Apply(b => b.DexSkin = value);

    private void Apply(System.Action<GlobalLink5> write)
    {
        if (_loading || _block is null)
            return;
        write(_block);
        MarkEdited();
    }

    internal void SetItem(int index, ushort itemId, byte quantity)
    {
        if (_loading || _block is null)
            return;
        _block.SetItem(index, itemId);
        _block.SetItemQuantity(index, quantity);
        MarkEdited();
    }

    internal void SetFurniture(int index, ushort value)
    {
        if (_loading || _block is null)
            return;
        var furniture = _block.GetFurniture(index);
        furniture.Value = value;
        MarkEdited();
    }

    private void MarkEdited()
    {
        if (_sav is not null)
            _sav.State.Edited = true;
    }
}

/// <summary>One of the 20 stored Global Link item slots: an item id (combo) plus a 0..255 quantity.</summary>
public partial class GlobalLinkItemViewModel : ViewModelBase
{
    private readonly GlobalLink5EditorViewModel _parent;
    private readonly int _index;
    private readonly ISpriteRenderer? _spriteRenderer;

    public GlobalLinkItemViewModel(GlobalLink5EditorViewModel parent, int index, ushort itemId, byte quantity, IReadOnlyList<ComboItem> itemList, ISpriteRenderer? spriteRenderer)
    {
        _parent = parent;
        _index = index;
        _itemId = itemId;
        _quantity = quantity;
        _spriteRenderer = spriteRenderer;
        ItemList = itemList;
    }

    public int Slot => _index + 1;
    public IReadOnlyList<ComboItem> ItemList { get; }

    public byte[]? Sprite => _spriteRenderer?.GetItemSprite(ItemId);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Sprite))]
    private int _itemId;

    [ObservableProperty]
    private int _quantity;

    partial void OnItemIdChanged(int value) => _parent.SetItem(_index, (ushort)value, (byte)Quantity);
    partial void OnQuantityChanged(int value) => _parent.SetItem(_index, (ushort)ItemId, (byte)value);
}

/// <summary>One of the 5 stored Dream World furniture slots (<see cref="DreamFurniture5"/>).</summary>
public partial class GlobalLinkFurnitureViewModel : ViewModelBase
{
    private readonly GlobalLink5EditorViewModel _parent;
    private readonly int _index;

    public GlobalLinkFurnitureViewModel(GlobalLink5EditorViewModel parent, int index, ushort value, string name)
    {
        _parent = parent;
        _index = index;
        _value = value;
        Name = name;
    }

    public int Slot => _index + 1;
    public string Name { get; }

    [ObservableProperty]
    private int _value;

    partial void OnValueChanged(int value) => _parent.SetFurniture(_index, (ushort)value);
}
