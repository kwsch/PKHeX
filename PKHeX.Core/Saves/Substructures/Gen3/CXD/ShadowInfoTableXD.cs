using System;
using System.Linq;

namespace PKHeX.Core;

public sealed class ShadowInfoTableXD
{
    private readonly ShadowInfoEntryXD[] Entries;
    private readonly int MaxLength;
    private readonly int SIZE_ENTRY;
    private const int MaxCount = 128;

    public ShadowInfoTableXD(ReadOnlySpan<byte> data, bool jp)
    {
        SIZE_ENTRY = GetEntrySize(jp);
        MaxLength = data.Length;
        int eCount = data.Length/SIZE_ENTRY;
        Entries = new ShadowInfoEntryXD[eCount];
        for (int i = 0; i < eCount; i++)
            Entries[i] = GetEntry(data, i, jp);
    }

    private static int GetEntrySize(bool jp) => jp ? ShadowInfoEntry3J.SIZE_ENTRY : ShadowInfoEntry3U.SIZE_ENTRY;

    public ShadowInfoTableXD(bool jp) : this(new byte[GetEntrySize(jp) * MaxCount], jp) { }

    private ShadowInfoEntryXD GetEntry(ReadOnlySpan<byte> data, int index, bool jp)
    {
        var slice = data.Slice(index * SIZE_ENTRY, SIZE_ENTRY).ToArray();
        return jp ? new ShadowInfoEntry3J(slice) : new ShadowInfoEntry3U(slice);
    }

    public byte[] Write() => Entries.SelectMany(entry => entry.Data).Take(MaxLength).ToArray();

    public ShadowInfoEntryXD this[int index] { get => Entries[index]; set => Entries[index] = value; }
    public int Count => Entries.Length;
}
