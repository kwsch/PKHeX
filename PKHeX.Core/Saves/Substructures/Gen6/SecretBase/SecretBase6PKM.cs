using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class SecretBase6PKM : ISanityChecksum, IFatefulEncounter
{
    public const int SIZE = 0x34;
    public readonly byte[] Data;

    public SecretBase6PKM(byte[] data) => Data = data;
    public SecretBase6PKM() => Data = new byte[SIZE];

    public uint EncryptionConstant
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(0x00));
        set => WriteUInt32LittleEndian(Data.AsSpan(0x00), value);
    }

    public ushort Sanity
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x04));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x04), value);
    }

    public ushort Checksum
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x06));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x06), value);
    }

    public ushort Species
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x08));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x08), value);
    }

    public int HeldItem
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x0A));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x0A), (ushort)value);
    }

    public int Ability { get => Data[0x0C]; set => Data[0x0C] = (byte)value; }
    public int AbilityNumber { get => Data[0x0D]; set => Data[0x0D] = (byte)value; }

    public uint PID
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(0x10));
        set => WriteUInt32LittleEndian(Data.AsSpan(0x10), value);
    }

    public Nature Nature { get => (Nature)Data[0x14]; set => Data[0x14] = (byte)value; }

    public bool FatefulEncounter { get => (Data[0x15] & 1) == 1; set => Data[0x15] = (byte)((Data[0x15] & ~0x01) | (value ? 1 : 0)); }
    public byte Gender { get => (byte)((Data[0x15] >> 1) & 0x3); set => Data[0x15] = (byte)((Data[0x15] & ~0x06) | (value << 1)); }
    public byte Form { get => (byte)(Data[0x15] >> 3); set => Data[0x15] = (byte)((Data[0x15] & 0x07) | (value << 3)); }

    public int EV_HP { get => Data[0x16]; set => Data[0x16] = (byte)value; }
    public int EV_ATK { get => Data[0x17]; set => Data[0x17] = (byte)value; }
    public int EV_DEF { get => Data[0x18]; set => Data[0x18] = (byte)value; }
    public int EV_SPE { get => Data[0x19]; set => Data[0x19] = (byte)value; }
    public int EV_SPA { get => Data[0x1A]; set => Data[0x1A] = (byte)value; }
    public int EV_SPD { get => Data[0x1B]; set => Data[0x1B] = (byte)value; }

    public ushort Move1
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x1C));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x1C), value);
    }

    public ushort Move2
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x1E));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x1E), value);
    }

    public ushort Move3
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x20));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x20), value);
    }

    public ushort Move4
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(0x22));
        set => WriteUInt16LittleEndian(Data.AsSpan(0x22), value);
    }

    public int Move1_PPUps { get => Data[0x24]; set => Data[0x24] = (byte)value; }
    public int Move2_PPUps { get => Data[0x25]; set => Data[0x25] = (byte)value; }
    public int Move3_PPUps { get => Data[0x26]; set => Data[0x26] = (byte)value; }
    public int Move4_PPUps { get => Data[0x27]; set => Data[0x27] = (byte)value; }

    // they messed up their bit struct and these ended up as individual fields? (5bits per byte)
    public int IV_HP { get => Data[0x28]; set => Data[0x28] = (byte)value; }
    public int IV_ATK { get => Data[0x29]; set => Data[0x29] = (byte)value; }
    public int IV_DEF { get => Data[0x2A]; set => Data[0x2A] = (byte)value; }
    public int IV_SPE { get => Data[0x2B]; set => Data[0x2B] = (byte)value; }
    public int IV_SPA { get => Data[0x2C]; set => Data[0x2C] = (byte)value; }
    public int IV_SPD { get => Data[0x2D] & 0x1F; set => Data[0x2D] = (byte)((Data[0x2D] & ~31) | value); }
    public bool IsEgg { get => ((Data[0x2D] >> 5) & 1) == 1; set => Data[0x2D] = (byte)((Data[0x2D] & ~32) | (value ? 32 : 0)); }
    public bool IsShiny { get => ((Data[0x2D] >> 5) & 1) == 1; set => Data[0x2D] = (byte)((Data[0x2D] & ~32) | (value ? 32 : 0)); }

    public int CurrentFriendship { get => Data[0x2E]; set => Data[0x2E] = (byte)value; }
    public byte Ball { get => Data[0x2F]; set => Data[0x2F] = value; }
    public byte CurrentLevel { get => Data[0x30]; set => Data[0x30] = value; }
    // 0x31,0x32,0x33 unused (alignment padding to u32)
}
