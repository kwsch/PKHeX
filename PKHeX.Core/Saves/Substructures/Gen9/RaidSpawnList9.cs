using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;
// ReSharper disable UnusedMember.Global

namespace PKHeX.Core;

public sealed class RaidSpawnList9 : SaveBlock<SAV9SV>
{
    public readonly int CountAll;
    public readonly int CountUsed;

    public RaidSpawnList9(SAV9SV sav, SCBlock block) : base(sav, block.Data)
    {
        CountAll = block.Data.Length / TeraRaidDetail.SIZE;
        CountUsed = RaidCountLegal_A0;
    }

    public const int RaidCountLegal_A0 = 72;

    public TeraRaidDetail GetRaid(int entry) => new(Data, 0x10 + (entry * TeraRaidDetail.SIZE));

    public TeraRaidDetail[] GetAllRaids()
    {
        var result = new TeraRaidDetail[CountAll];
        for (int i = 0; i < result.Length; i++)
            result[i] = GetRaid(i);
        return result;
    }

    public ulong CurrentSeed
    {
        get => ReadUInt64LittleEndian(Data.AsSpan(0x00));
        set => WriteUInt64LittleEndian(Data.AsSpan(0x00), value);
    }

    public ulong TomorrowSeed
    {
        get => ReadUInt64LittleEndian(Data.AsSpan(0x08));
        set => WriteUInt64LittleEndian(Data.AsSpan(0x08), value);
    }

    /// <summary>
    /// Copies content from the <see cref="sourceIndex"/> raid to all other raid details with valid position data.
    /// </summary>
    /// <param name="sourceIndex">Source raid detail</param>
    /// <param name="seedToo">Copy the RNG seed</param>
    public void Propagate(int sourceIndex, bool seedToo)
    {
        var current = GetRaid(sourceIndex);
        for (int i = 0; i < CountUsed; i++)
        {
            var raid = GetRaid(i);
            if (raid.AreaID == 0)
                continue;
            raid.IsEnabled = current.IsEnabled;
            raid.Content = current.Content;
            raid.IsClaimedLeaguePoints = current.IsClaimedLeaguePoints;
            if (seedToo)
                raid.Seed = current.Seed;
        }
    }
}

public sealed class TeraRaidDetail
{
    public const int SIZE = 0x20;

    private readonly byte[] Data;
    private readonly int Offset;

    public TeraRaidDetail(byte[] data, int ofs)
    {
        Data = data;
        Offset = ofs;
    }

    private const string General = nameof(General);
    private const string Misc = nameof(Misc);

    [Category(General), Description("Indicates if this entry has an active raid crystal.")]
    public bool IsEnabled
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x00)) != 0;
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x00), value ? 1u : 0);
    }

    [Category(General), Description("Zone the raid crystal is located in.")]
    public uint AreaID
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x04));
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x04), value);
    }

    [Category(Misc), Description("Indicates how the crystal is shown on the player's YMAP.")]
    public TeraRaidDisplayType DisplayType
    {
        get => (TeraRaidDisplayType)ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x08));
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x08), (uint)value);
    }

    [Category(General), Description("Zone-specific overworld spawn point for the raid crystal.")]
    public uint SpawnPointID
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x0C));
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x0C), value);
    }

    [Category(General), Description("RNG Seed (32bit) for fetching the Raid data table and generating the raid."), TypeConverter(typeof(TypeConverterU32))]
    public uint Seed
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x10));
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x10), value);
    }

    [Category(Misc), Description("Always zero.")]
    public uint Unused
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x14));
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x14), value);
    }

    [Category(General), Description("Indicates the source of the Raid encounter data and rewards.")]
    public TeraRaidContentType Content
    {
        get => (TeraRaidContentType)ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x18));
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x18), (uint)value);
    }

    [Category(Misc), Description("Has player already collected the League Points for this Raid?")]
    public bool IsClaimedLeaguePoints
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x1C)) != 0;
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x1C), value ? 1u : 0);
    }
}

public enum TeraRaidDisplayType : uint
{
    None = 0,
    Unrestricted = 1,
    RequiresRide = 2,
}

public enum TeraRaidContentType : uint
{
    Base05,
    Black6,
    Distribution,
    Might7,
}
