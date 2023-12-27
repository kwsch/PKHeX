using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 Mystery Event data structure.
/// </summary>
public class MysteryEvent3 : Gen3MysteryData
{
    public const int SIZE = sizeof(uint) + 1000; // total 0x3EC

    public MysteryEvent3(byte[] data) : base(data) => ArgumentOutOfRangeException.ThrowIfNotEqual(data.Length, SIZE);

    public byte Magic { get => Data[4]; set => Data[4] = value; }
    public byte MapGroup { get => Data[5]; set => Data[5] = value; }
    public byte MapNumber { get => Data[6]; set => Data[6] = value; }
    public byte ObjectID { get => Data[7]; set => Data[7] = value; }
}
