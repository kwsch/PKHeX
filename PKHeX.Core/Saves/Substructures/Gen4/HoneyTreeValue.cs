using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Honey Tree in Sinnoh (Gen4)
/// </summary>
public sealed class HoneyTreeValue(byte[] Data)
{
    public const int Size = 8;

    public readonly byte[] Data = Data;

    public uint Time { get => ReadUInt32LittleEndian(Data.AsSpan(0)); set => WriteUInt32LittleEndian(Data.AsSpan(0), value); }
    public int Slot { get => Data[4]; set => Data[4] = (byte)value; }
    public int SubTable { get => Data[5]; set => Data[5] = (byte)value; } // offset by 1 with respect to Group
    public int Group { get => Data[6]; set { Data[6] = (byte)value; SubTable = Math.Max(0, Group - 1); } }
    public int Shake { get => Data[7]; set => Data[7] = (byte)value; }
}
