using System;
using System.Diagnostics.CodeAnalysis;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Inventory Pouch used by <see cref="GameVersion.SWSH"/>
/// </summary>
public sealed class InventoryPouch8(InventoryType type, IItemStorage info, int maxCount, int offset, [ConstantExpected] int size)
    : InventoryPouch(type, info, maxCount, offset, size)
{
    public bool SetNew { get; set; }
    private int[] OriginalItems = [];

    public override InventoryItem8 GetEmpty(int itemID = 0, int count = 0) => new() { Index = itemID, Count = count };

    public override void GetPouch(ReadOnlySpan<byte> data)
    {
        var span = data[Offset..];
        var items = new InventoryItem8[PouchDataSize];
        for (int i = 0; i < items.Length; i++)
        {
            var item = span.Slice(i * 4, 4);
            uint value = ReadUInt32LittleEndian(item);
            items[i] = InventoryItem8.GetValue(value);
        }
        Items = items;
        OriginalItems = Array.ConvertAll(items, z => z.Index);
    }

    public override void SetPouch(Span<byte> data)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(Items.Length, PouchDataSize);

        var span = data[Offset..];
        var items = (InventoryItem8[])Items;
        for (int i = 0; i < items.Length; i++)
        {
            var item = span.Slice(i * 4, 4);
            uint val = items[i].GetValue(SetNew, OriginalItems);
            WriteUInt32LittleEndian(item, val);
        }
    }

    /// <summary>
    /// Checks pouch contents for bad count values.
    /// </summary>
    /// <remarks>
    /// Certain pouches contain a mix of count-limited items and uncapped regular items.
    /// </remarks>
    internal void SanitizeCounts()
    {
        foreach (var item in Items)
            item.Count = GetSuggestedCount(Type, item.Index, item.Count);
    }

    public static int GetSuggestedCount(InventoryType t, int item, int requestVal) => t switch
    {
        // TMs are clamped to 1, let TRs be whatever
        InventoryType.TMHMs => item is >= 1130 and <= 1229 ? requestVal : 1,
        _ => requestVal,
    };
}
