using System;
using System.Collections.Generic;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Array storing all the Random Seed group data.
/// </summary>
/// <remarks>size: 0x630</remarks>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class RandomGroup8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw)
{
    public const int COUNT_GROUP = 12;

    public RandomSeed8b[] Seeds
    {
        get => GetSeeds();
        set => SetSeeds(value);
    }

    private RandomSeed8b[] GetSeeds()
    {
        var result = new RandomSeed8b[COUNT_GROUP];
        for (int i = 0; i < result.Length; i++)
            result[i] = new RandomSeed8b(Raw.Slice(i * RandomSeed8b.SIZE, RandomSeed8b.SIZE));
        return result;
    }

    private static void SetSeeds(IReadOnlyList<RandomSeed8b> value)
    {
        if (value.Count != COUNT_GROUP)
            throw new ArgumentOutOfRangeException(nameof(value));
        // data is already hard-referencing the original byte array. This is mostly a hack for Property Grid displays.
    }
}

/// <summary>
/// Random Seed data.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class RandomSeed8b(Memory<byte> raw)
{
    private Span<byte> Data => raw.Span;

    public const int GROUP_NAME_SIZE = 16; // chars
    public const int PERSON_NAME_SIZE = 32; // chars

    private const int OFS_GROUPNAME = 0; // 0x00
    private const int OFS_PLAYERNAME = OFS_GROUPNAME + (GROUP_NAME_SIZE * 2); // 0x20
    private const int OFS_GENDER = OFS_PLAYERNAME + (PERSON_NAME_SIZE * 2); // 0x60
    private const int OFS_REGION = OFS_GENDER + 4; // 0x68
    private const int OFS_SEED = OFS_REGION + 4; // 0x6C
    private const int OFS_RAND = OFS_SEED + 8; // 0x70
    private const int OFS_TICK = OFS_RAND + 8; // 0x78
    private const int OFS_UID = OFS_TICK + 8; // 0x80
    public const int SIZE = OFS_UID + 4; // 0x84

    public string GroupName
    {
        get => StringConverter8.GetString(Data[..(GROUP_NAME_SIZE * 2)]);
        set => StringConverter8.SetString(Data[..(GROUP_NAME_SIZE * 2)], value, GROUP_NAME_SIZE, StringConverterOption.ClearZero);
    }

    public string PlayerName
    {
        get => StringConverter8.GetString(Data.Slice(OFS_PLAYERNAME, PERSON_NAME_SIZE * 2));
        set => StringConverter8.SetString(Data.Slice(OFS_PLAYERNAME, PERSON_NAME_SIZE * 2), value, PERSON_NAME_SIZE, StringConverterOption.ClearZero);
    }

    public bool Male { get => Data[OFS_GENDER] == 1; set => Data[+ OFS_GENDER] = (byte)(value ? 1 : 0); }

    public int RegionCode
    {
        get => ReadInt32LittleEndian(Data[OFS_REGION..]);
        set => WriteInt32LittleEndian(Data[OFS_REGION..], value);
    }

    public ulong Seed
    {
        get => ReadUInt64LittleEndian(Data[OFS_SEED..]);
        set => WriteUInt64LittleEndian(Data[OFS_SEED..], value);
    }

    public ulong Random
    {
        get => ReadUInt64LittleEndian(Data[OFS_RAND..]);
        set => WriteUInt64LittleEndian(Data[OFS_RAND..], value);
    }

    public long Ticks
    {
        get => ReadInt64LittleEndian(Data[OFS_TICK..]);
        set => WriteInt64LittleEndian(Data[OFS_TICK..], value);
    }

    public int UserID
    {
        get => ReadInt32LittleEndian(Data[OFS_UID..]);
        set => WriteInt32LittleEndian(Data[OFS_UID..], value);
    }

    public DateTime Timestamp { get => DateTime.FromFileTimeUtc(Ticks); set => Ticks = value.ToFileTimeUtc(); }
    public DateTime LocalTimestamp { get => Timestamp.ToLocalTime(); set => Timestamp = value.ToUniversalTime(); }
}
