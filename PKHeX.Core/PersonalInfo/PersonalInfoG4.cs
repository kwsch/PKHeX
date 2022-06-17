using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="PersonalInfo"/> class with values from Generation 4 games.
/// </summary>
public sealed class PersonalInfoG4 : PersonalInfoG3
{
    public new const int SIZE = 0x2C;

    public PersonalInfoG4(byte[] data) : base(data)
    {
        // Unpack TMHM & Tutors
        TMHM = GetBits(Data.AsSpan(0x1C, 0x0D));
        //TypeTutors = Array.Empty<bool>(); // not stored in personal -- default value
    }

    public override byte[] Write()
    {
        SetBits(TMHM, Data.AsSpan(0x1C));
        return Data;
    }

    // Manually added attributes
    public override int FormCount { get => Data[0x29]; set {} }
    protected internal override int FormStatsIndex { get => ReadUInt16LittleEndian(Data.AsSpan(0x2A)); set {} }
}
