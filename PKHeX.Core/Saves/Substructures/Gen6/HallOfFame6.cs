using System;

namespace PKHeX.Core;

public sealed class HallOfFame6(SAV6 sav, Memory<byte> raw) : SaveBlock<SAV6>(sav, raw)
{
    public const int Entries = 16; // First clear, and 15 most-recent entries.
    public const int PokeSize = HallFame6Entity.SIZE; // 0x48
    public const int PokeCount = 6;
    public const int EntrySize = (PokeCount * PokeSize) + HallFame6Index.SIZE; // 0x1B4

    public const int MaxComplete = 9999;

    public Span<byte> GetEntry(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)Entries);
        return Data.Slice(index * EntrySize, EntrySize);
    }

    public uint GetInsertIndex(out uint clear)
    {
        // Check for empty slots (where player hasn't yet registered enough Fame clears)
        for (uint i = 0; i < Entries; i++)
        {
            var entry = GetEntry((int)i);
            var vnd = new HallFame6Index(entry[^4..]);
            if (!vnd.HasData)
                return clear = i;
        }

        // No empty slots, return the last slot.
        ClearEntry(1);
        clear = new HallFame6Index(GetEntry(14)).ClearIndex + 1;
        return 15;
    }

    public void ClearEntry(int index)
    {
        int offset = index * EntrySize;
        if (index != 15)
        {
            // Shift down
            var dest = Data[offset..];
            var above = Data.Slice(offset + EntrySize, EntrySize * (Entries - 1 - index));
            above.CopyTo(dest);
        }

        // Ensure Last Entry is Cleared
        Data.Slice(EntrySize * (Entries - 1), EntrySize).Clear();
    }

    public Span<byte> GetEntity(int team, int member)
    {
        return GetEntry(team).Slice(member * PokeSize, PokeSize);
    }
}
