using System;

namespace PKHeX.Core;

public sealed class MedalList5(SAV5B2W2 SAV, Memory<byte> raw) : SaveBlock<SAV5B2W2>(SAV, raw)
{
    private const int MAX_MEDALS = 255;

    public static Medal5[] GetMedals(Memory<byte> memory)
    {
        var count = memory.Length / Medal5.SIZE;
        var result = new Medal5[count];
        for (int i = 0; i < result.Length; i++)
            result[i] = GetMedal(memory, i);
        return result;
    }

    public static Medal5 GetMedal(Memory<byte> memory, int index)
    {
        if ((uint)index >= MAX_MEDALS)
            throw new ArgumentOutOfRangeException(nameof(index), $"Index must be between 0 and {MAX_MEDALS - 1}.");
        return new Medal5(memory.Slice(index * Medal5.SIZE, Medal5.SIZE));
    }

    public Medal5 this[int index] => GetMedal(Raw, index);

    public void ObtainAll(DateOnly date, bool unread = true)
    {
        for (int i = 0; i < MAX_MEDALS; i++)
            this[i].Obtain(date, unread);
    }
}
