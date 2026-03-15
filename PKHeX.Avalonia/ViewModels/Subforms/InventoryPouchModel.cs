using PKHeX.Avalonia.Controls;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// View model representing a single inventory pouch tab (e.g., Items, Balls, Medicine).
/// </summary>
public sealed class InventoryPouchModel
{
    /// <summary>
    /// Display name for the tab header.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The underlying pouch this model represents.
    /// </summary>
    public InventoryPouch Pouch { get; }

    /// <summary>
    /// The observable collection of item rows for the DataGrid.
    /// </summary>
    public SortableList<InventoryItemModel> Items { get; }

    public InventoryPouchModel(string name, InventoryPouch pouch, SortableList<InventoryItemModel> items)
    {
        Name = name;
        Pouch = pouch;
        Items = items;
    }
}
