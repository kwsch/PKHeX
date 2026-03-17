using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// View model for a single inventory item row displayed in the DataGrid.
/// Wraps the underlying <see cref="InventoryItem"/> with observable properties.
/// </summary>
public partial class InventoryItemModel : ObservableObject
{
    private readonly string[] _itemNames;

    [ObservableProperty]
    private string _itemName;

    [ObservableProperty]
    private int _count;

    [ObservableProperty]
    private int _index;

    [ObservableProperty]
    private bool _isFavorite;

    [ObservableProperty]
    private bool _isNew;

    [ObservableProperty]
    private bool _isFreeSpace;

    [ObservableProperty]
    private uint _freeSpaceIndex;

    [ObservableProperty]
    private bool _isNewShop;

    [ObservableProperty]
    private bool _isHeld;

    /// <summary>
    /// Reference to the underlying inventory item from the save data.
    /// </summary>
    public InventoryItem BackingItem { get; }

    /// <summary>
    /// List of valid item names for the combo box selection in this pouch.
    /// </summary>
    public IReadOnlyList<string> ValidItems { get; }

    // Flag visibility — set once based on the backing item type
    public bool HasFavorite { get; }
    public bool HasNew { get; }
    public bool HasFreeSpace { get; }
    public bool HasFreeSpaceIndex { get; }
    public bool HasNewShop { get; }
    public bool HasHeld { get; }

    public InventoryItemModel(InventoryItem item, string[] itemNames, IReadOnlyList<string> validItems)
    {
        BackingItem = item;
        ValidItems = validItems;
        _itemNames = itemNames;
        _index = item.Index;
        _count = item.Count;
        _itemName = item.Index < itemNames.Length ? itemNames[item.Index] : $"(Item #{item.Index:000})";

        // Read flag values from the backing item
        if (item is IItemFavorite fav)
        {
            HasFavorite = true;
            _isFavorite = fav.IsFavorite;
        }
        if (item is IItemNewFlag nf)
        {
            HasNew = true;
            _isNew = nf.IsNew;
        }
        if (item is IItemFreeSpace fs)
        {
            HasFreeSpace = true;
            _isFreeSpace = fs.IsFreeSpace;
        }
        if (item is IItemFreeSpaceIndex fsi)
        {
            HasFreeSpaceIndex = true;
            _freeSpaceIndex = fsi.FreeSpaceIndex;
        }
        if (item is IItemNewShopFlag ns)
        {
            HasNewShop = true;
            _isNewShop = ns.IsNewShop;
        }
        if (item is IItemHeldFlag hf)
        {
            HasHeld = true;
            _isHeld = hf.IsHeld;
        }
    }

    partial void OnItemNameChanged(string value)
    {
        // Update Index when the user changes the item name (e.g. via text edit).
        // This keeps Index in sync so WriteBackAllPouches can use it directly.
        var id = Array.IndexOf(_itemNames, value);
        if (id >= 0)
            Index = id;
    }
}
