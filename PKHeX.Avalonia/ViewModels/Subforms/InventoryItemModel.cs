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
        _index = item.Index;
        _count = item.Count;
        _itemName = item.Index < itemNames.Length ? itemNames[item.Index] : $"(Item #{item.Index:000})";
    }
}
