using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// 2ED4
/// </summary>
public sealed class UndergroundItemList8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw)
{
    public const int ItemSaveSize = 999;
    public const int ItemMaxCount = 999;
    public const int StatueMaxCount = 99;
    public const int SIZE = ItemSaveSize * UndergroundItem8b.SIZE;

    public IReadOnlyList<UndergroundItem8b> ReadItems()
    {
        var result = new UndergroundItem8b[ItemSaveSize];
        for (int i = 0; i < result.Length; i++)
            result[i] = new UndergroundItem8b(Data, i);
        return result;
    }

    public void WriteItems(IReadOnlyList<UndergroundItem8b> items)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(items.Count, ItemSaveSize);
        foreach (var item in items)
            item.Write(Data);
        SAV.State.Edited = true;
    }
}
