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

    /// <summary>
    /// Reference to the underlying inventory item from the save data.
    /// </summary>
    public InventoryItem BackingItem { get; }

    /// <summary>
    /// List of valid item names for the combo box selection in this pouch.
    /// </summary>
    public IReadOnlyList<string> ValidItems { get; }

    public InventoryItemModel(InventoryItem item, string[] itemNames, IReadOnlyList<string> validItems)
    {
        BackingItem = item;
        ValidItems = validItems;
        _itemNames = itemNames;
        _index = item.Index;
        _count = item.Count;
        _itemName = item.Index < itemNames.Length ? itemNames[item.Index] : $"(Item #{item.Index:000})";
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
