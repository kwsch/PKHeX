using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Puff6 : SaveBlock<SAV6>
{
    private const int MaxPuffID = 26; // Supreme Winter Poké Puff
    private const int PuffSlots = 100;

    public Puff6(SAV6 SAV, int offset) : base(SAV) => Offset = offset;

    public Span<byte> GetPuffs() => SAV.Data.AsSpan(Offset, PuffSlots);
    public void SetPuffs(ReadOnlySpan<byte> value) => SAV.SetData(value, Offset);

    public int PuffCount
    {
        get => ReadInt32LittleEndian(Data.AsSpan(Offset + PuffSlots));
        set => WriteInt32LittleEndian(Data.AsSpan(Offset + PuffSlots), value);
    }

    public void Reset()
    {
        Array.Clear(Data, Offset, PuffSlots);
        // Set the first few default Puffs
        Data[Offset + 0] = 1;
        Data[Offset + 1] = 2;
        Data[Offset + 2] = 3;
        Data[Offset + 3] = 4;
        Data[Offset + 4] = 5;
        PuffCount = 5;
    }

    public void MaxCheat(bool special = false)
    {
        var rnd = Util.Rand;
        if (special)
        {
            for (int i = 0; i < PuffSlots; i++)
                Data[Offset + i] = (byte)(21 + rnd.Next(2)); // Supreme Wish or Honor
        }
        else
        {
            for (int i = 0; i < PuffSlots; i++)
                Data[Offset + i] = (byte)((i % MaxPuffID) + 1);
            Util.Shuffle(Data.AsSpan(), Offset, Offset + PuffSlots, rnd);
        }
        PuffCount = PuffSlots;
    }

    public void Sort(bool reverse = false)
    {
        Array.Sort(Data, Offset, PuffCount);
        if (reverse)
            Array.Reverse(Data, Offset, PuffCount);
    }
}
