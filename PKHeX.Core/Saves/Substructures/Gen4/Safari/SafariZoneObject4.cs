using System;

namespace PKHeX.Core;

public struct SafariZoneObject4
{
    public const int SIZE = 4;

    public SafariZoneObjectType4 Type { get; set; }
    public byte X { get; set; }
    /// <summary> Ground depth (fourth byte). </summary>
    public byte Z { get; set; }
    /// <summary> Height off the ground (third byte). </summary>
    public byte Y { get; set; }
    public bool IsEmpty => Type == 0 && X == 0 && Y == 0 && Z == 0;

    /// <summary> Initializes an empty object. </summary>
    public SafariZoneObject4() { }

    /// <summary>
    /// Reads an object from the data provided.
    /// </summary>
    public SafariZoneObject4(ReadOnlySpan<byte> data)
    {
        Type = (SafariZoneObjectType4)data[0];
        X = data[1];
        Z = data[3];
        Y = data[2];
    }

    public readonly void Write(Span<byte> data)
    {
        data[0] = (byte)Type;
        data[1] = X;
        data[3] = Z;
        data[2] = Y;
    }
}
