using System;

namespace PKHeX.Core;

/// <summary>
/// Exposes permissions about Item Storage (Pouch) constraints.
/// </summary>
public interface IItemStorage
{
    /// <summary>
    /// Indicates if the item is actually obtainable for the given <see cref="type"/>.
    /// </summary>
    /// <remarks>
    /// This is used to check if the item is legal to obtain, not if the item can exist in the pouch.
    /// </remarks>
    /// <param name="type">Type of inventory to check.</param>
    /// <param name="itemIndex">Item ID to check.</param>
    /// <param name="itemCount">Quantity value for the item.</param>
    /// <returns>True if the item is possible to obtain.</returns>
    bool IsLegal(InventoryType type, int itemIndex, int itemCount);

    /// <summary>
    /// Gets all possible item IDs that can exist in the given pouch <see cref="type"/>.
    /// </summary>
    /// <param name="type">Type of inventory to check.</param>
    /// <returns>List of all possible item IDs.</returns>
    ReadOnlySpan<ushort> GetItems(InventoryType type);
}
