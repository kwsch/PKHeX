using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class MedalList5(SAV5B2W2 SAV, Memory<byte> raw) : SaveBlock<SAV5B2W2>(SAV, raw)
{
    // amount of medals needed to reach a specific rank
    public const int RankRookie = 50;
    public const int RankElite = 100;
    public const int RankMaster = 150;
    public const int RankLegend = 200;
    private const int MAX_MEDALS = 255; // Top Medalist

    public static Medal5[] GetMedals(Memory<byte> raw)
    {
        var count = Math.Min(MAX_MEDALS, raw.Length / Medal5.SIZE);
        var result = new Medal5[count];
        for (int i = 0; i < result.Length; i++)
            result[i] = GetMedal(raw, i);
        return result;
    }

    public static Medal5 GetMedal(Memory<byte> raw, int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, MAX_MEDALS);
        return new Medal5(raw.Slice(index * Medal5.SIZE, Medal5.SIZE));
    }

    public Medal5 this[int index] => GetMedal(Raw, index);

    public void ObtainAll(DateOnly date, bool unread = true, bool skipAlreadyObtained = true)
    {
        for (int i = 0; i < MAX_MEDALS; i++)
        {
            var medal = this[i];
            if (skipAlreadyObtained && medal.IsObtained)
                continue;
            medal.Obtain(date, unread);
        }
    }

    public void GiveAll(DateOnly date, bool unread = true)
    {
        ObtainAll(date, unread);
        Rank = CalculateRank(MAX_MEDALS);
    }

    public static MedalType5 GetMedalType(int index) => (uint)index switch
    {
        < 007 => MedalType5.Special,
        < 105 => MedalType5.Adventure,
        < 161 => MedalType5.Battle,
        < 236 => MedalType5.Entertainment,
        < MAX_MEDALS => MedalType5.Challenge,
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };

    public const int LengthAllMedals = MAX_MEDALS * Medal5.SIZE;
    public Span<byte> AllMedals => Data[..LengthAllMedals];

    public const byte PinnedMedalNone = MAX_MEDALS;

    public byte PinnedMedal
    {
        get => Data[0x3FC];
        set => Data[0x3FC] = value;
    }

    public MedalRank5 Rank
    {
        get => (MedalRank5)Data[0x3FD];
        set => Data[0x3FD] = (byte)value;
    }

    public bool IsTutorialComplete
    {
        get => Data[0x3FE] != 0;
        set => Data[0x3FE] = (byte)(value ? 1 : 0);
    }
    // 3FF unused

    public HabitatList5 HabitatList => new(Raw.Slice(0x400, HabitatList5.SIZE));
    // 2 bytes alignment, total length 0x498

    public static MedalRank5 CalculateRank(int count) => count switch
    {
        < RankRookie => MedalRank5.None,
        < RankElite => MedalRank5.Rookie,
        < RankMaster => MedalRank5.Elite,
        < RankLegend => MedalRank5.Master,
        _ => MedalRank5.Legend,
    };

    public MedalRank5 CalculateRank()
    {
        var count = GetCountObtained();
        return CalculateRank(count);
    }

    public int GetCountObtained()
    {
        int count = 0;
        for (int i = 0; i < MAX_MEDALS; i++)
        {
            if (this[i].IsObtained)
                count++;
        }
        return count;
    }
}

public sealed class HabitatList5(Memory<byte> raw)
{
    public const int SIZE = 0x96; // starts with some unused data

    public const int HabitatCount = 90;

    private Span<byte> Data => raw.Span;

    public Span<byte> Unused => Data[..0x36];

    public HabitatStatus5 GetHabitat(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, HabitatCount);
        return new(raw.Slice(0x36 + (index * HabitatStatus5.SIZE), HabitatStatus5.SIZE));
    }

    public ushort Unknown90
    {
        get => ReadUInt16LittleEndian(Data[0x90..]);
        set => WriteUInt16LittleEndian(Data[0x90..], value);
    }

    public byte Unknown92 { get => Data[0x92]; set => Data[0x92] = value; }

    public HabitatEncounterType5 LastEncounterType
    {
        get => (HabitatEncounterType5)Data[0x93];
        set => Data[0x93] = (byte)value;
    }
    public bool IsTutorialViewed { get => Data[0x94] != 0; set => Data[0x94] = (byte)(value ? 1 : 0); }
    public bool IsTutorialCompleteCapture { get => Data[0x95] != 0; set => Data[0x95] = (byte)(value ? 1 : 0); }

    public void CompleteAll()
    {
        for (int i = 0; i < HabitatCount; i++)
            GetHabitat(i).SetComplete();
    }
}

public struct HabitatStatus5(Memory<byte> data)
{
    // a fun single-byte struct.
    // not sure if it's worthwhile figuring out how to make this less heavy...
    // feels great storing (byte*,int) to represent a single byte value, maybe as a ref byte, but for now this is fine.
    public const int SIZE = 1;
    private readonly Span<byte> Data => data.Span;

    public byte Value
    {
        readonly get => Data[0];
        set => Data[0] = value;
    }

    public HabitatCompletion5 Grass
    {
        readonly get => (HabitatCompletion5)(Data[0] & 0b0000_0011);
        set => Data[0] = (byte)((Data[0] & ~0b0000_0011) | ((byte)value & 0b11));
    }

    public HabitatCompletion5 Surf
    {
        readonly get => (HabitatCompletion5)((Data[0] >> 2) & 0b11);
        set => Data[0] = (byte)((Data[0] & ~0b0000_1100) | (((byte)value & 0b11) << 2));
    }

    public HabitatCompletion5 Fish
    {
        readonly get => (HabitatCompletion5)((Data[0] >> 4) & 0b11);
        set => Data[0] = (byte)((Data[0] & ~0b0011_0000) | (((byte)value & 0b11) << 4));
    }

    public bool IsComplete
    {
        readonly get => FlagUtil.GetFlag(Data, 0, 6);
        set => FlagUtil.SetFlag(Data, 0, 6, value);
    }

    public HabitatCompletion5 GetStatus(HabitatEncounterType5 type) => type switch
    {
        HabitatEncounterType5.Grass => Grass,
        HabitatEncounterType5.Surf => Surf,
        HabitatEncounterType5.Fish => Fish,
        _ => throw new ArgumentOutOfRangeException(nameof(type)),
    };

    public void SetStatus(HabitatEncounterType5 type, HabitatCompletion5 value)
    {
        switch (type)
        {
            case HabitatEncounterType5.Grass:
                Grass = value;
                break;
            case HabitatEncounterType5.Surf:
                Surf = value;
                break;
            case HabitatEncounterType5.Fish:
                Fish = value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type));
        }
    }

    public void SetComplete() => Value = 0b_1_11_11_11; // sets all 3 habitats to complete and the IsComplete flag to true
    public void Clear() => Value = 0;
}

public enum MedalType5
{
    Special,
    Adventure,
    Battle,
    Entertainment,
    Challenge,
}

public enum MedalRank5 : byte
{
    None = 0,
    Rookie = 1,
    Elite = 2,
    Master = 3,
    Legend = 4,
    // nothing above 200
}

public enum HabitatCompletion5 : byte
{
    None = 0,
    Seen = 1,
    Caught = 2,
    Complete = 3,
}

public enum HabitatEncounterType5 : byte
{
    /// <summary>
    /// <see cref="SlotType5.Grass"/>
    /// </summary>
    Grass = 0,

    /// <summary>
    /// <see cref="SlotType5.Surf"/>
    /// </summary>
    Surf = 1,

    /// <summary>
    /// <see cref="SlotType5.Super_Rod"/>
    /// </summary>
    Fish = 2,
}
