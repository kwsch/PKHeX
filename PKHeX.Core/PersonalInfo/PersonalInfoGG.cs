using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="PersonalInfo"/> class with values from the <see cref="GameVersion.GG"/> games.
/// </summary>
public class PersonalInfoGG : PersonalInfoSM
{
    public PersonalInfoGG(byte[] data) : base(data)
    {
        TMHM = GetBits(Data.AsSpan(0x28, 8)); // only 60 TMs used
        TypeTutors = GetBits(Data.AsSpan(0x38, 1)); // at most 8 flags used
    }

    public int GoSpecies { get => ReadUInt16LittleEndian(Data.AsSpan(0x48)); set => WriteUInt16LittleEndian(Data.AsSpan(0x48), (ushort)value); }
}
