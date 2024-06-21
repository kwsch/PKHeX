using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;
// ReSharper disable UnusedMember.Global

namespace PKHeX.Core;

public sealed class RaidSpawnList9 : SaveBlock<SAV9SV>
{
    public readonly int CountAll;
    public readonly int CountUsed;
    public const int RaidCountLegal_T0 = 72;
    public const int RaidCountLegal_T1 = 100;
    public const int RaidCountLegal_T2 = 80;
    public readonly bool HasSeeds;
    private readonly int OffsetRaidStart;
    private readonly Memory<byte> Memory;
    private Span<byte> Span => Memory.Span;

    public RaidSpawnList9(SAV9SV sav, SCBlock block, Memory<byte> memory, int countUsed, bool hasSeeds) : base(sav, block.Data)
    {
        Memory = memory;
        var length = memory.Length;
        HasSeeds = hasSeeds;
        OffsetRaidStart = hasSeeds ? 0x10 : 0;
        CountAll = length == 0 ? 0 : (length - OffsetRaidStart) / TeraRaidDetail.SIZE;
        CountUsed = countUsed;
    }

    public TeraRaidDetail GetRaid(int entry) => new(Memory.Slice(GetOffset(entry), TeraRaidDetail.SIZE));

    private int GetOffset(int entry) => OffsetRaidStart + (entry * TeraRaidDetail.SIZE);

    public TeraRaidDetail[] GetAllRaids()
    {
        var result = new TeraRaidDetail[CountAll];
        for (int i = 0; i < result.Length; i++)
            result[i] = GetRaid(i);
        return result;
    }

    public ulong CurrentSeed
    {
        get => HasSeeds ? ReadUInt64LittleEndian(Span) : 0;
        set { if (HasSeeds) WriteUInt64LittleEndian(Span, value); }
    }

    public ulong TomorrowSeed
    {
        get => HasSeeds ? ReadUInt64LittleEndian(Span[0x8..]) : 0;
        set { if (HasSeeds) WriteUInt64LittleEndian(Span[0x8..], value); }
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

public sealed class TeraRaidDetail(Memory<byte> Data)
{
    public const int SIZE = 0x20;

    private Span<byte> Span => Data.Span;

    private const string General = nameof(General);
    private const string Misc = nameof(Misc);

    [Category(General), Description("Indicates if this entry has an active raid crystal.")]
    public bool IsEnabled
    {
        get => ReadUInt32LittleEndian(Span) != 0;
        set => WriteUInt32LittleEndian(Span, value ? 1u : 0);
    }

    [Category(General), Description("Zone the raid crystal is located in.")]
    [RefreshProperties(RefreshProperties.All)]
    public uint AreaID
    {
        get => ReadUInt32LittleEndian(Span[0x04..]);
        set => WriteUInt32LittleEndian(Span[0x04..], value);
    }

    [Category(Misc), Description("Indicates which group the crystal belongs to.")]
    [RefreshProperties(RefreshProperties.All)]
    public uint LotteryGroup
    {
        get => ReadUInt32LittleEndian(Span[0x08..]);
        set => WriteUInt32LittleEndian(Span[0x08..], value);
    }

    [Category(General), Description("Random crystal choice from the area and lottery group.")]
    [RefreshProperties(RefreshProperties.All)]
    public uint SpawnPointID
    {
        get => ReadUInt32LittleEndian(Span[0x0C..]);
        set => WriteUInt32LittleEndian(Span[0x0C..], value);
    }

    public string ScenePointName => $"Raid_Point_{AreaID}_{LotteryGroup}_{SpawnPointID}";

    [Category(General), Description("RNG Seed (32bit) for fetching the raid data table and generating the raid."), TypeConverter(typeof(TypeConverterU32))]
    public uint Seed
    {
        get => ReadUInt32LittleEndian(Span[0x10..]);
        set => WriteUInt32LittleEndian(Span[0x10..], value);
    }

    [Category(Misc), Description("Always zero.")]
    public uint Unused
    {
        get => ReadUInt32LittleEndian(Span[0x14..]);
        set => WriteUInt32LittleEndian(Span[0x14..], value);
    }

    [Category(General), Description("Indicates the source of the raid encounter data and rewards.")]
    public TeraRaidContentType Content
    {
        get => (TeraRaidContentType)ReadUInt32LittleEndian(Span[0x18..]);
        set => WriteUInt32LittleEndian(Span[0x18..], (uint)value);
    }

    [Category(Misc), Description("Has player already collected the League Points for this raid?")]
    public bool IsClaimedLeaguePoints
    {
        get => ReadUInt32LittleEndian(Span[0x1C..]) != 0;
        set => WriteUInt32LittleEndian(Span[0x1C..], value ? 1u : 0);
    }
}

public enum TeraRaidContentType : uint
{
    Base05,
    Black6,
    Distribution,
    Might7,
}

public enum TeraRaidOrigin : uint
{
    Paldea,
    Kitakami,
    BlueberryAcademy,
}
