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

    // Flag visibility for the entire pouch (based on the item type used by this pouch)
    public bool HasFavorite { get; }
    public bool HasNew { get; }
    public bool HasFreeSpace { get; }
    public bool HasFreeSpaceIndex { get; }
    public bool HasNewShop { get; }
    public bool HasHeld { get; }

    public InventoryPouchModel(string name, InventoryPouch pouch, SortableList<InventoryItemModel> items)
    {
        Name = name;
        Pouch = pouch;
        Items = items;

        // Determine flag visibility from the item type
        if (pouch.Items.Length > 0)
        {
            var item0 = pouch.Items[0];
            HasFavorite = item0 is IItemFavorite;
            HasNew = item0 is IItemNewFlag;
            HasFreeSpace = item0 is IItemFreeSpace;
            HasFreeSpaceIndex = item0 is IItemFreeSpaceIndex;
            HasNewShop = item0 is IItemNewShopFlag;
            HasHeld = item0 is IItemHeldFlag;
        }
    }
}
