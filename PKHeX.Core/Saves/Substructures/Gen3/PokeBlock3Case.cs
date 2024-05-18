using System;

namespace PKHeX.Core;

public sealed class PokeBlock3Case
{
    private const int Count = 40;
    public const int SIZE = Count * PokeBlock3.SIZE;
    public readonly PokeBlock3[] Blocks;

    public PokeBlock3Case(ReadOnlySpan<byte> data)
    {
        Blocks = new PokeBlock3[Count];
        for (int i = 0; i < Blocks.Length; i++)
            Blocks[i] = PokeBlock3.GetBlock(data, (i * PokeBlock3.SIZE));
    }

    public byte[] Write()
    {
        byte[] result = new byte[SIZE];
        Write(result);
        return result;
    }

    public void Write(Span<byte> result)
    {
        for (int i = 0; i < Blocks.Length; i++)
            Blocks[i].SetBlock(result.Slice(i * PokeBlock3.SIZE, PokeBlock3.SIZE));
    }

    public void DeleteAll()
    {
        foreach (var b in Blocks)
            b.Delete();
    }

    public void MaximizeAll(bool createMissing = false)
    {
        foreach (var b in Blocks)
            b.Maximize(createMissing);
    }
}
