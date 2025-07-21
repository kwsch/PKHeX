using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class WonderCard3Extra : Gen3MysteryData
{
    /// <summary>
    /// 0x28: Total Size of this object
    /// </summary>
    public const int SIZE = sizeof(uint) + 36;

    public WonderCard3Extra(Memory<byte> raw) : base(raw) => ArgumentOutOfRangeException.ThrowIfNotEqual(raw.Length, SIZE);

    public ushort Wins   { get => ReadUInt16LittleEndian(Data[0x4..]); set => WriteUInt16LittleEndian(Data[0x4..], value); }
    public ushort Losses { get => ReadUInt16LittleEndian(Data[0x6..]); set => WriteUInt16LittleEndian(Data[0x6..], value); }
    public ushort Trades { get => ReadUInt16LittleEndian(Data[0x8..]); set => WriteUInt16LittleEndian(Data[0x8..], value); }
    public ushort Unk    { get => ReadUInt16LittleEndian(Data[0xA..]); set => WriteUInt16LittleEndian(Data[0xA..], value); }

    // u16 distributedMons[2][7]
}
