using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;
// ReSharper disable UnusedMember.Global

namespace PKHeX.Core;

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class FixedSpawnList9 : SaveBlock<SAV9SV>
{
    public readonly int CountAll;

    public FixedSpawnList9(SAV9SV sav, SCBlock block) : base(sav, block.Data)
    {
        CountAll = block.Data.Length / FixedSpawnDetail.SIZE;
    }

    public FixedSpawnDetail GetSpawn(int entry) => new(Data, entry * FixedSpawnDetail.SIZE);

    public FixedSpawnDetail[] Spawns => GetAllSpawns();

    public FixedSpawnDetail[] GetAllSpawns()
    {
        var result = new FixedSpawnDetail[CountAll];
        for (int i = 0; i < result.Length; i++)
            result[i] = GetSpawn(i);
        return result;
    }

    public PK9[] Entities
    {
        get => GetAllEntities();
        set
        {
            if (value.Length != CountAll)
                throw new ArgumentException(nameof(value));
            for (int i = 0; i < value.Length; i++)
                GetSpawn(i).Entity = value[i];
        }
    }

    public PK9[] GetAllEntities()
    {
        var result = new PK9[CountAll];
        for (int i = 0; i < result.Length; i++)
            result[i] = GetSpawn(i).Entity;
        return result;
    }
}

public sealed class FixedSpawnDetail
{
    public const int SIZE = 0x170;

    private readonly byte[] Data;
    private readonly int Offset;

    public FixedSpawnDetail(byte[] data, int ofs)
    {
        Data = data;
        Offset = ofs;
    }

    private const string General = nameof(General);
    private const string Misc = nameof(Misc);

    [Category(Misc), Description("Unknown Hash.")]
    public ulong Hash
    {
        get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x00));
        set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x00), value);
    }

    [Category(General), Description("Indicates if this entry is available on the overworld.")]
    public bool IsEnabled
    {
        get => Data[Offset + 0x08] == 1;
        set => Data[Offset + 0x08] = value ? (byte)1 : (byte)0;
    }

    [Category(General), Description("Encrypted Entity data.")]
    public PK9 Entity
    {
        get => new(Data.Slice(Offset + 0x09, PokeCrypto.SIZE_9PARTY));
        set => value.EncryptedPartyData.CopyTo(Data, Offset + 0x09);
    }

    // 7 bytes of padding, for alignment of next field.
    [Category(Misc), Description("time_t (64 bit) of when the encounter was last regenerated. Usually just a date (no time component).")]
    public ulong Seconds
    {
        get => ReadUInt64LittleEndian(Data.AsSpan(Offset + 0x168));
        set => WriteUInt64LittleEndian(Data.AsSpan(Offset + 0x168), value);
    }

    public override string ToString()
    {
        var pk = Entity;
        var timestamp = new Epoch1970Value(Data.AsMemory(Offset + 0x168)).DisplayValue;
        var state = IsEnabled ? "X" : " ";
        return $"{timestamp} {state} {Hash:X16} {pk.Species:000} - {pk.Nickname} ({pk.CurrentLevel})";
    }
}
