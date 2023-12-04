using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;
// ReSharper disable UnusedMember.Global

namespace PKHeX.Core;

public sealed class RaidSevenStar9(SAV9SV sav, SCBlock block) : SaveBlock<SAV9SV>(sav, block.Data)
{
    public readonly int CountAll = block.Data.Length / SevenStarRaidDetail.SIZE;

    public SevenStarRaidDetail GetRaid(int entry) => new(Data, 0x00 + (entry * SevenStarRaidDetail.SIZE));

    public SevenStarRaidDetail[] GetAllRaids()
    {
        var result = new SevenStarRaidDetail[CountAll];
        for (int i = 0; i < result.Length; i++)
            result[i] = GetRaid(i);
        return result;
    }
}

public sealed class SevenStarRaidDetail(byte[] Data, int Offset)
{
    public const int SIZE = 0x08;

    private const string General = nameof(General);

    [Category(General), Description("Identifier used for this 7 Star Raid. Matches the date this raid was first distributed.")]
    public uint Identifier
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x00));
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x00), value);
    }

    [Category(General), Description("Indicates if this Tera Raid Boss has been captured by the player.")]
    public bool Captured
    {
        get => Data[Offset + 0x04] == 1;
        set => Data[Offset + 0x04] = (byte)(value ? 1 : 0);
    }

    [Category(General), Description("Indicates if this Tera Raid Boss has been defeated at least once by the player.")]
    public bool Defeated
    {
        get => Data[Offset + 0x05] == 1;
        set => Data[Offset + 0x05] = (byte)(value ? 1 : 0);
    }

    // 0x06 - 0x07 padding
}
