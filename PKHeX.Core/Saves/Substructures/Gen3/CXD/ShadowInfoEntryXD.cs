using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public abstract class ShadowInfoEntryXD
{
    public readonly byte[] Data;

    protected ShadowInfoEntryXD(byte[] data) => Data = data;

    public bool IsSnagged => Data[0] >> 6 != 0;
    public bool IsPurified { get => Data[0] >> 7 == 1; set { Data[0] &= 0x7F; if (value) Data[0] |= 0x80; } }

    public int IV_HP  { get => Data[0x0B]; set => Data[0x0B] = (byte)value; }
    public int IV_ATK { get => Data[0x0C]; set => Data[0x0C] = (byte)value; }
    public int IV_DEF { get => Data[0x0D]; set => Data[0x0D] = (byte)value; }
    public int IV_SPA { get => Data[0x0E]; set => Data[0x0E] = (byte)value; }
    public int IV_SPD { get => Data[0x0F]; set => Data[0x0F] = (byte)value; }
    public int IV_SPE { get => Data[0x10]; set => Data[0x10] = (byte)value; }

    // Gen3 Species ID
    public ushort RawSpecies { get => ReadUInt16BigEndian(Data.AsSpan(0x1A)); set => WriteUInt16BigEndian(Data.AsSpan(0x1A), value); }
    public ushort Species { get => SpeciesConverter.GetNational3(RawSpecies); set => RawSpecies = SpeciesConverter.GetInternal3(value); }
    public uint PID { get => ReadUInt32BigEndian(Data.AsSpan(0x1C)); set => WriteUInt32BigEndian(Data.AsSpan(0x1C), value); }
    public int Purification { get => ReadInt32BigEndian(Data.AsSpan(0x24)); set => WriteInt32BigEndian(Data.AsSpan(0x24), value); }

    public uint EXP
    {
        get => ReadUInt32BigEndian(Data.AsSpan(0x04)) >> 12;
        set => WriteUInt32BigEndian(Data.AsSpan(0x04), (ReadUInt32BigEndian(Data.AsSpan(0x04)) & 0xFFF) | (value << 12));
    }

    public bool IsEmpty => Species == 0;

    public abstract byte Index { get; set; }

    public override string ToString() => $"{(Species) Species} 0x{PID:X8} {Purification}";
}

public sealed class ShadowInfoEntry3J : ShadowInfoEntryXD
{
    internal const int SIZE_ENTRY = 60; // -12 from U

    public ShadowInfoEntry3J() : base(new byte[SIZE_ENTRY]) { }
    public ShadowInfoEntry3J(byte[] data) : base(data) { }

    public override byte Index { get => Data[0x35]; set => Data[0x35] = value; }

    public override string ToString() => $"{(Species)Species} 0x{PID:X8} {Purification}";
}

public sealed class ShadowInfoEntry3U : ShadowInfoEntryXD
{
    internal const int SIZE_ENTRY = 72; // -12 from U

    public ShadowInfoEntry3U() : base(new byte[SIZE_ENTRY]) { }
    public ShadowInfoEntry3U(byte[] data) : base(data) { }

    public override byte Index { get => Data[0x3F]; set => Data[0x3F] = value; }

    public override string ToString() => $"{(Species)Species} 0x{PID:X8} {Purification}";
}
