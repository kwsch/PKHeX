using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Puff6(SAV6 SAV, Memory<byte> raw) : SaveBlock<SAV6>(SAV, raw)
{
    private const byte MaxPuffID = 26; // Supreme Winter Poké Puff
    private const int PuffSlots = 100;

    public Span<byte> GetPuffs() => Data[..PuffSlots];
    public void SetPuffs(ReadOnlySpan<byte> value) => SAV.SetData(GetPuffs(), value);

    public int PuffCount
    {
        get => ReadInt32LittleEndian(Data[PuffSlots..]);
        set => WriteInt32LittleEndian(Data[PuffSlots..], value);
    }

    public void Reset()
    {
        var puffs = GetPuffs();
        puffs.Clear();
        // Set the first few default Puffs
        puffs[0] = 1;
        puffs[1] = 2;
        puffs[2] = 3;
        puffs[3] = 4;
        puffs[4] = 5;
        PuffCount = 5;
    }

    public void MaxCheat(bool special = false)
    {
        var rnd = Util.Rand;
        var puffs = GetPuffs();
        if (special)
        {
            foreach (ref var puff in puffs)
                puff = (byte)(21 + rnd.Next(2)); // Supreme Wish or Honor
        }
        else
        {
            int i = 0;
            foreach (ref var puff in puffs)
                puff = (byte)((i++ % MaxPuffID) + 1);
            rnd.Shuffle(puffs);
        }
        PuffCount = PuffSlots;
    }

    public void Sort(bool reverse = false)
    {
        var puffs = GetPuffs();
        puffs.Sort();
        if (reverse)
            puffs.Reverse();
    }
}
