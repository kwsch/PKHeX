using System;

namespace PKHeX.Core;

/// <summary>
/// Stores a bitflag medal completion state for all species.
/// </summary>
/// <remarks>
/// <see cref="PokeathlonStat4"/> for bitflag indexes.
/// </remarks>
public struct PokeathlonMedalManager4(Memory<byte> Raw)
{
    public const int SIZE = 493; // 1-indexed species [Bulbasaur..Arceus]
    public const byte MaxMedalBits = 0b11111; // 5 courses, 5 bits per species

    public Span<byte> Data => Raw.Span;

    /// <summary>
    /// Retrieves the medal bits for the given species, where each bit represents whether a medal for a particular course has been obtained or not.
    /// </summary>
    public byte GetMedal(ushort species)
    {
        species--;
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(species, (ushort)Legal.MaxSpeciesID_4);
        return Data[species];
    }

    /// <summary>
    /// Updates the medal bits for the given species.
    /// </summary>
    public void SetMedal(ushort species, byte medalBits)
    {
        species--;
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(species, (ushort)Legal.MaxSpeciesID_4);
        Data[species] = medalBits;
    }

    /// <summary>
    /// Awards the provided bit(s) to the species.
    /// </summary>
    public void AwardMedal(ushort species, byte medalBit)
    {
        species--;
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(species, (ushort)Legal.MaxSpeciesID_4);
        Data[species] |= medalBit;
    }

    /// <summary>
    /// Awards all medals to all species (complete).
    /// </summary>
    /// <param name="medalBits">Medal value to set to every species entry.</param>
    public void SetAllMedals(byte medalBits = MaxMedalBits) => Data[..SIZE].Fill(medalBits);

    /// <summary>
    /// Removes all medals from all species (resets progress).
    /// </summary>
    public void Clear() => SetAllMedals(0);

    public uint GetTotalCount()
    {
        uint result = 0;
        foreach (var bits in Data[..SIZE])
            result += (uint)System.Numerics.BitOperations.PopCount((uint)bits & MaxMedalBits);
        return result;
    }
}
