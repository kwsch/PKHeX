using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class UndergroundItemList8b : SaveBlock<SAV8BS>
{
    public const int ItemSaveSize = 999;
    public const int ItemMaxCount = 999;
    public const int StatueMaxCount = 99;

    public UndergroundItemList8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

    public IReadOnlyList<UndergroundItem8b> ReadItems()
    {
        var result = new UndergroundItem8b[ItemSaveSize];
        for (int i = 0; i < result.Length; i++)
            result[i] = new UndergroundItem8b(Data, Offset, i);
        return result;
    }

    public void WriteItems(IReadOnlyList<UndergroundItem8b> items)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(items.Count, ItemSaveSize);
        foreach (var item in items)
            item.Write(Data, Offset);
        SAV.State.Edited = true;
    }
}
