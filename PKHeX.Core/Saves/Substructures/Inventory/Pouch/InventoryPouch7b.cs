using System;
using System.Diagnostics.CodeAnalysis;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Inventory Pouch used by <see cref="GameVersion.GG"/>
/// </summary>
public sealed class InventoryPouch7b(InventoryType type, IItemStorage info, int maxCount, int offset, [ConstantExpected] int size)
    : InventoryPouch(type, info, maxCount, offset, size)
{
    public bool SetNew { get; set; }
    private int[] OriginalItems = [];

    public override InventoryItem7b GetEmpty(int itemID = 0, int count = 0) => new() { Index = itemID, Count = count };

    public override void GetPouch(ReadOnlySpan<byte> data)
    {
        var span = data[Offset..];
        var items = new InventoryItem7b[PouchDataSize];
        for (int i = 0; i < items.Length; i++)
        {
            var item = span.Slice(i * 4, 4);
            uint val = ReadUInt32LittleEndian(item);
            items[i] = InventoryItem7b.GetValue(val);
        }
        Items = items;
        OriginalItems = Array.ConvertAll(items, z => z.Index);
    }

    public override void SetPouch(Span<byte> data)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(Items.Length, PouchDataSize);

        var span = data[Offset..];
        var items = (InventoryItem7b[])Items;
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

    public static int GetSuggestedCount(InventoryType t, int item, int requestVal)
    {
        switch (t)
        {
            // mixed regular battle items & mega stones
            case InventoryType.BattleItems when item > 100:
            // mixed regular items & key items
            case InventoryType.Items when ItemStorage7GG.Pouch_Regular_GG_Key.Contains((ushort)item):
                return Math.Min(1, requestVal);

            default:
                return requestVal;
        }
    }
}
