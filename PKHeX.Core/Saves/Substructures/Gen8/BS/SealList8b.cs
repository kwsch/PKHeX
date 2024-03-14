using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Array storing all the seal sticker counts the player has collected.
/// </summary>
/// <remarks>size: 0x960</remarks>
public sealed class SealList8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw)
{
    public const int SealSaveSize = 200;
    public const int SealMaxCount = 99;

    public IReadOnlyList<SealSticker8b> ReadItems()
    {
        var result = new SealSticker8b[SealSaveSize];
        for (int i = 0; i < result.Length; i++)
            result[i] = new SealSticker8b(Data, i);
        return result;
    }

    public void WriteItems(IReadOnlyList<SealSticker8b> items)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(items.Count, SealSaveSize);
        foreach (var item in items)
            item.Write(Data);
        SAV.State.Edited = true;
    }
}
