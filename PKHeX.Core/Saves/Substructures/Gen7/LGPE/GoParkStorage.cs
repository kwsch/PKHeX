using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace PKHeX.Core;

public sealed class GoParkStorage(SAV7b sav, Memory<byte> raw) : SaveBlock<SAV7b>(sav, raw), IEnumerable<GP1>
{
    public const int SlotsPerArea = 50;
    public const int Areas = 20;
    public const int Count = SlotsPerArea * Areas; // 1000

    public GP1 this[int index]
    {
        get
        {
            Debug.Assert(index < Count);
            return GP1.FromData(Data[(GP1.SIZE * index)..]);
        }
        set
        {
            Debug.Assert(index < Count);
            value.WriteTo(Data[(GP1.SIZE * index)..]);
        }
    }

    public GP1[] GetAllEntities()
    {
        var value = new GP1[Count];
        for (int i = 0; i < value.Length; i++)
            value[i] = this[i];
        return value;
    }

    public void SetAllEntities(IReadOnlyList<GP1> value)
    {
        Debug.Assert(value.Count == Count);
        for (int i = 0; i < value.Count; i++)
            this[i] = value[i];
    }

    public IEnumerable<string> DumpAll(IReadOnlyList<string> speciesNames)
    {
        var arr = GetAllEntities();
        for (int i = 0; i < arr.Length; i++)
        {
            var entry = arr[i];
            if (entry.Species > 0)
                yield return entry.Dump(speciesNames, i);
        }
    }

    public IEnumerator<GP1> GetEnumerator() => (IEnumerator<GP1>)GetAllEntities().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetAllEntities().GetEnumerator();

    public void DeleteAll()
    {
        var blank = new GP1();
        for (int i = 0; i < Count; i++)
            this[i] = blank;
    }
}
