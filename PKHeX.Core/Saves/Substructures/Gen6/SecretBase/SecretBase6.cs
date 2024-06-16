using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Secret base format for <see cref="GameVersion.ORAS"/>
/// </summary>
public class SecretBase6(Memory<byte> raw)
{
    public const int SIZE = 0x310;
    public const int COUNT_GOODS = 28;
    public const int MinLocationID = -1;
    public const int MaxLocationID = 85;

    protected Span<byte> Data => raw.Span;

    // structure: (first at 23D24 in sav)
    // [000-001] u8 IsNew
    // [002-003] s16 Location
    // [004-153] DecorationPosition[28] (150, 12 bytes each)

    // [154... ] ???

    // [21A-233] Trainer Name
    // [234-255] Flavor1
    // [256-277] Flavor2
    // [278-299] Saying1
    // [29A-2BB] Saying2
    // [2BC-2E9] Saying3
    // [2EA-2FF] Saying4
    // [300-303] Secret Base Rank
    // [304-307] u32 TotalFlagsFromFriends
    // [308-30B] u32 TotalFlagsFromOther
    // [30C] byte CollectedFlagsToday
    // [30D] byte CollectedFlagsYesterday
    // 2 bytes alignment for u32

    public bool IsNew
    {
        get => Data[0] == 1;
        set => WriteUInt16LittleEndian(Data, (ushort)(value ? 1 : 0));
    }

    private int RawLocation
    {
        get => ReadInt16LittleEndian(Data[2..]);
        set => WriteInt16LittleEndian(Data[2..], (short)value);
    }

    public int BaseLocation
    {
        get => RawLocation;
        set => RawLocation = value switch
        {
            1 or 2 => 0,
            > MaxLocationID => MaxLocationID,
            < MinLocationID => -1,
            _ => value,
        };
    }

    public SecretBase6GoodPlacement GetPlacement(int index) => new(raw.Slice(GetPlacementOffset(index), SecretBase6GoodPlacement.SIZE));

    private static int GetPlacementOffset(int index)
    {
        if ((uint) index >= COUNT_GOODS)
            throw new ArgumentOutOfRangeException(nameof(index));
        return 4 + (index * SecretBase6GoodPlacement.SIZE);
    }

    public byte BoppoyamaScore
    {
        get => Data[0x174];
        set => Data[0x174] = value;
    }

    private const int NameLengthBytes = 0x1A;
    private const int MessageLengthBytes = 0x22;
    private const int NameLength = (0x1A / 2) - 1; // + terminator
    private const int MessageLength = (0x22 / 2) - 1; // + terminator

    private static int Language => 0;

    public string TrainerName
    {
        get => StringConverter6.GetString(Data.Slice(0x21A, NameLengthBytes));
        set => StringConverter6.SetString(Data.Slice(0x21A, NameLengthBytes), value, NameLength, Language, StringConverterOption.ClearZero);
    }

    private Span<byte> GetMessageSpan(int index) => Data.Slice(0x234 + (MessageLengthBytes * index), MessageLengthBytes);
    private string GetMessage(int index) => StringConverter6.GetString(GetMessageSpan(index));
    private void SetMessage(int index, ReadOnlySpan<char> value) => StringConverter6.SetString(GetMessageSpan(index), value, MessageLength, Language, StringConverterOption.ClearZero);

    public string TeamName { get => GetMessage(0); set => SetMessage(0, value); }
    public string TeamSlogan { get => GetMessage(1); set => SetMessage(1, value); }
    public string SayHappy { get => GetMessage(2); set => SetMessage(2, value); }
    public string SayEncourage { get => GetMessage(3); set => SetMessage(3, value); }
    public string SayBlackboard { get => GetMessage(4); set => SetMessage(4, value); }
    public string SayConfettiBall { get => GetMessage(5); set => SetMessage(5, value); }

    public SecretBase6Rank Rank
    {
        get => (SecretBase6Rank) ReadInt32LittleEndian(Data[0x300..]);
        set => WriteInt32LittleEndian(Data[0x300..], (int)value);
    }

    public uint TotalFlagsFromFriends
    {
        get => ReadUInt32LittleEndian(Data[0x304..]);
        set => WriteUInt32LittleEndian(Data[0x304..], value);
    }

    public uint TotalFlagsFromOther
    {
        get => ReadUInt32LittleEndian(Data[0x308..]);
        set => WriteUInt32LittleEndian(Data[0x308..], value);
    }

    public byte CollectedFlagsToday { get => Data[0x30C]; set => Data[0x30C] = value; }
    public byte CollectedFlagsYesterday { get => Data[0x30D]; set => Data[0x30D] = value; }

    // Derived Values

    public bool IsDummiedBaseLocation => !IsEmpty && BaseLocation < 3;
    public bool IsEmpty => BaseLocation <= 0;

    protected virtual void LoadOther(SecretBase6Other other) => LoadSelf(other);
    private void LoadSelf(SecretBase6 other) => other.Data[..SIZE].CopyTo(Data); // ensure input is truncated to correct size (other has extra fields)

    public void Load(SecretBase6 other)
    {
        if (other is SecretBase6Other o)
            LoadOther(o);
        else
            LoadSelf(other);
    }

    public virtual byte[] Write() => Data.ToArray();

    public static SecretBase6? Read(byte[] data)
    {
        return data.Length switch
        {
            SIZE => new SecretBase6(data),
            SecretBase6Other.SIZE => new SecretBase6Other(data),
            _ => null,
        };
    }
}

/// <summary>
/// An expanded structure of <see cref="SecretBase6"/> containing extra fields to describe another trainer's base.
/// </summary>
public sealed class SecretBase6Other(Memory<byte> raw) : SecretBase6(raw)
{
    public new const int SIZE = 0x3E0;

    // [310-31F] u128 key struct
    // [320] byte language
    // [321] byte trainer gender
    //       2 bytes unused alignment
    // [324-327] u32     ???
    // [328-32D] byte[6] ???
    //       2 bytes unused alignment

    // [330-3CB] SecretBase6PKM[3] (0x9C bytes, 0x34 each)
    // [0 @ 330]
    // [1 @ 364]
    // [2 @ 398]

    // [3CC-3D3] s64 ??? struct?
    // [3D4-3D5] s16 ???
    // [3D6] byte ???
    // [3D7] byte ???
    // [3D8-3D9] flags
    // remainder alignment

    public byte Language
    {
        get => Data[0x320];
        set => Data[0x320] = value;
    }

    public byte Gender
    {
        get => Data[0x321];
        set => Data[0x321] = value;
    }

    public const int COUNT_TEAM = 3;

    private Span<byte> GetParticipantData(int index) => Data.Slice(GetParticipantOffset(index), SecretBase6PKM.SIZE);
    public SecretBase6PKM GetParticipant(int index) => new(GetParticipantData(index).ToArray());
    public void SetParticipant(int index, SecretBase6PKM pk) => pk.Data.CopyTo(GetParticipantData(index));

    public static int GetParticipantOffset(int index)
    {
        if ((uint) index >= COUNT_TEAM)
            throw new ArgumentOutOfRangeException(nameof(index));
        return 0x330 + (index * SecretBase6PKM.SIZE);
    }

    public SecretBase6PKM[] GetTeam()
    {
        var result = new SecretBase6PKM[COUNT_TEAM];
        ReadTeam(result);
        return result;
    }

    public void ReadTeam(SecretBase6PKM[] result)
    {
        for (int i = 0; i < COUNT_TEAM; i++)
            result[i] = GetParticipant(i);
    }

    public void SetTeam(SecretBase6PKM[] arr)
    {
        if (arr.Length != COUNT_TEAM)
            throw new ArgumentOutOfRangeException(nameof(arr));

        for (int i = 0; i < arr.Length; i++)
            SetParticipant(i, arr[i]);
    }

    protected override void LoadOther(SecretBase6Other other) => other.Data.CopyTo(Data);

    public override byte[] Write() => Data.ToArray();
}
