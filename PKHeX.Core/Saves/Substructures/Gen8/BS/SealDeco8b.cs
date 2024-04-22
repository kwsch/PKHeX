using System;
using System.Collections.Generic;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Stores customized ball seal configurations.
/// </summary>
/// <remarks>size 0x4288</remarks>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class SealBallDecoData8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw)
{
    public const int COUNT_CAPSULE = 99; // CapsuleData[99]

    public const int SIZE = 4 + (COUNT_CAPSULE * SealCapsule8b.SIZE); // 0x4288

    public void Clear() => Data.Clear();

    public byte CapsuleCount { get => Data[0]; set => Data[0] = value; }

    public SealCapsule8b[] Capsules
    {
        get => GetCapsules();
        set => SetCapsules(value);
    }

    private SealCapsule8b[] GetCapsules()
    {
        var result = new SealCapsule8b[COUNT_CAPSULE];
        for (int i = 0; i < result.Length; i++)
            result[i] = new SealCapsule8b(Raw.Slice(4 + (i * SealCapsule8b.SIZE), SealCapsule8b.SIZE));
        return result;
    }

    private static void SetCapsules(IReadOnlyList<SealCapsule8b> value)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(value.Count, COUNT_CAPSULE);
        // data is already hard-referencing the original byte array. This is mostly a hack for Property Grid displays.
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class SealCapsule8b(Memory<byte> raw)
{
    public const int COUNT_SEAL = 20; // AffixSealData[20]
    public const int SIZE = 12 + (COUNT_SEAL * AffixSealData8b.SIZE); // 0xAC

    private Span<byte> Data => raw.Span;

    public override string ToString() => $"{(Species)Species}-{EncryptionConstant:X8}-{Unknown}";

    public uint Species            { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }
    public uint EncryptionConstant { get => ReadUInt32LittleEndian(Data[4..]); set => WriteUInt32LittleEndian(Data[4..], value); }
    public uint Unknown            { get => ReadUInt32LittleEndian(Data[8..]); set => WriteUInt32LittleEndian(Data[8..], value); }

    public AffixSealData8b[] Seals
    {
        get => GetSeals();
        set => SetSeals(value);
    }

    private AffixSealData8b[] GetSeals()
    {
        var result = new AffixSealData8b[COUNT_SEAL];
        for (int i = 0; i < result.Length; i++)
            result[i] = new AffixSealData8b(raw.Slice(0xC + (i * AffixSealData8b.SIZE), AffixSealData8b.SIZE));
        return result;
    }

    private static void SetSeals(IReadOnlyList<AffixSealData8b> value)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(value.Count, COUNT_SEAL);
        // data is already hard-referencing the original byte array. This is mostly a hack for Property Grid displays.
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class AffixSealData8b(Memory<byte> raw)
{
    public const int SIZE = 8; // u16 id, s16 x,y,z

    private Span<byte> Data => raw.Span;

    public override string ToString() => $"{(Seal8b)SealID}-({X},{Y},{Z})";

    public ushort SealID { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }
    public short X { get => ReadInt16LittleEndian(Data[2..]); set => WriteInt16LittleEndian(Data[2..], value); }
    public short Y { get => ReadInt16LittleEndian(Data[4..]); set => WriteInt16LittleEndian(Data[4..], value); }
    public short Z { get => ReadInt16LittleEndian(Data[6..]); set => WriteInt16LittleEndian(Data[6..], value); }
}
