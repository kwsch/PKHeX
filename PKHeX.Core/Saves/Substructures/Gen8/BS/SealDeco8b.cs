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
public sealed class SealBallDecoData8b : SaveBlock<SAV8BS>
{
    public const int COUNT_CAPSULE = 99; // CapsuleData[99]

    private const int SIZE = 4 + (COUNT_CAPSULE * SealCapsule8b.SIZE); // 0x4288

    public SealBallDecoData8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

    public void Clear() => Data.AsSpan(Offset, SIZE).Clear();

    public byte CapsuleCount { get => Data[Offset]; set => Data[Offset] = value; }

    public SealCapsule8b[] Capsules
    {
        get => GetCapsules();
        set => SetCapsules(value);
    }

    private SealCapsule8b[] GetCapsules()
    {
        var result = new SealCapsule8b[COUNT_CAPSULE];
        for (int i = 0; i < result.Length; i++)
            result[i] = new SealCapsule8b(Data, Offset + 4 + (i * SealCapsule8b.SIZE));
        return result;
    }

    private static void SetCapsules(IReadOnlyList<SealCapsule8b> value)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(value.Count, COUNT_CAPSULE);
        // data is already hard-referencing the original byte array. This is mostly a hack for Property Grid displays.
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class SealCapsule8b(byte[] Data, int Offset)
{
    public const int COUNT_SEAL = 20; // AffixSealData[20]
    public const int SIZE = 12 + (COUNT_SEAL * AffixSealData8b.SIZE); // 0xAC

    public override string ToString() => $"{(Species)Species}-{EncryptionConstant:X8}-{Unknown}";

    public uint Species            { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0), value); }
    public uint EncryptionConstant { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 4)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 4), value); }
    public uint Unknown            { get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 8)); set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 8), value); }

    public AffixSealData8b[] Seals
    {
        get => GetSeals();
        set => SetSeals(value);
    }

    private AffixSealData8b[] GetSeals()
    {
        var result = new AffixSealData8b[COUNT_SEAL];
        for (int i = 0; i < result.Length; i++)
            result[i] = new AffixSealData8b(Data, Offset + 0xC + (i * AffixSealData8b.SIZE));
        return result;
    }

    private static void SetSeals(IReadOnlyList<AffixSealData8b> value)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(value.Count, COUNT_SEAL);
        // data is already hard-referencing the original byte array. This is mostly a hack for Property Grid displays.
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class AffixSealData8b(byte[] Data, int Offset)
{
    public const int SIZE = 8; // u16 id, s16 x,y,z

    public override string ToString() => $"{(Seal8b)SealID}-({X},{Y},{Z})";

    public ushort SealID { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0), value); }
    public short X { get => ReadInt16LittleEndian(Data.AsSpan(Offset + 2)); set => WriteInt16LittleEndian(Data.AsSpan(Offset + 2), value); }
    public short Y { get => ReadInt16LittleEndian(Data.AsSpan(Offset + 4)); set => WriteInt16LittleEndian(Data.AsSpan(Offset + 4), value); }
    public short Z { get => ReadInt16LittleEndian(Data.AsSpan(Offset + 6)); set => WriteInt16LittleEndian(Data.AsSpan(Offset + 6), value); }
}
