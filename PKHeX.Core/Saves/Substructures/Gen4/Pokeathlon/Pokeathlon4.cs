using System;
using static System.Buffers.Binary.BinaryPrimitives;


namespace PKHeX.Core;

/// <summary>
/// Manages the Pokeathlon Data for <see cref="SAV4HGSS"/>
/// </summary>
public sealed class Pokeathlon4(Memory<byte> Raw) // 0xD9D4 within SAV4HGSS
{
    public const int SIZE = 0xB80;

    public Span<byte> Data => Raw.Span;

    // 5 courses to store record data
    public PokeathlonCourseRecord4 GetCourseRecord(PokeathlonStat4 index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)PokeathlonStat4.Count);
        return new(Raw.Slice((int)index * PokeathlonCourseRecord4.SIZE, PokeathlonCourseRecord4.SIZE));
    }

    // 0xDC, 0xDAB0 within SAV
    public PokeathlonMedalManager4 Medals => new(Raw.Slice(0xDC, PokeathlonMedalManager4.SIZE));
    // 3 bytes alignment

    // 0x2CC, 0xDCA0 within SAV
    public PokeathlonEventData4 GetEventSelf(PokeathlonEvent4 index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)PokeathlonEvent4.Count);
        return new(Raw.Slice(0x2CC + ((int)index * PokeathlonEventData4.SIZE), PokeathlonEventData4.SIZE));
    }

    // 0x484, 0xDE58 within SAV
    public PokeathlonConnection4 GetEventConnection(PokeathlonEvent4 index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)PokeathlonEvent4.Count);
        return new(Raw.Slice(0x484 + ((int)index * PokeathlonConnection4.SIZE), PokeathlonConnection4.SIZE));
    }

    // 0xAEC, 0xE4C0 within SAV

    /// <summary>
    /// Player's highest score for each of the ten individual events (after conversion to Athlete Points)
    /// </summary>
    public ushort GetBestScore(PokeathlonEvent4 index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)PokeathlonEvent4.Count);
        return ReadUInt16LittleEndian(Data[(0xAEC + ((int)index * 2))..]);
    }

    public void SetBestScore(PokeathlonEvent4 index, ushort score)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)PokeathlonEvent4.Count);
        WriteUInt16LittleEndian(Data[(0xAEC + ((int)index * 2))..], score);
    }

    // 0xB00, 0xE4D4 within SAV global counters
    public PokeathlonGlobalCounters4 GlobalCounters => new(Raw.Slice(0xB00, PokeathlonGlobalCounters4.SIZE));

    // remainder @ 0xE548
    public const uint MaxPoints = 99_999;

    /// <summary>
    /// Current points count accumulated, used in buying items from shops.
    /// </summary>
    public uint Points { get => ReadUInt32LittleEndian(Data[0xB74..]); set => WriteUInt32LittleEndian(Data[0xB74..], Math.Min(MaxPoints, value)); }

    /// <summary>
    /// Obtained Data Card indexes [0,26], where each bit represents whether a Data Card has been obtained or not.
    /// They can be purchased in exchange for points in the Pokéathlon Dome (sold at the central reception desk).
    /// </summary>
    public uint FlagsDataCard { get => ReadUInt32LittleEndian(Data[0xB78..]); set => WriteUInt32LittleEndian(Data[0xB78..], Math.Min(DataCardAllObtained, value)); }

    // items exist as Key Items, not really advisable to have a one-shot method unlock, end-user implementation beware.
    public const uint DataCardAllObtained = 0x07FFFFFFu; // 27 bits, all obtained

    /// <summary>
    /// Once-daily shop purchase flags for the Athlete Shop.
    /// Since shops only have 12 items daily, only 12 bits are used.
    /// </summary>
    public ushort FlagsDailyShop { get => ReadUInt16LittleEndian(Data[0xB7C..]); set => WriteUInt16LittleEndian(Data[0xB7C..], Math.Min(FlagsShopAllObtained, value)); }
    public const ushort FlagsShopAllObtained = 0x0FFF; // 12 bits, all obtained

    // last 2 bytes unused, total size 0xB80

    /// <summary>
    /// The global Pokéathlon score is calculated as the sum of:
    /// - the player's best final score in each of the five courses,
    /// - the player's highest score for each of the ten individual events (after conversion to Athlete Points),
    /// - the total number of medals displayed in the box in the Trust room (so each Medalist species will add five to this total).
    /// </summary>
    public uint CalculateGlobalScore()
    {
        uint result = 0;
        for (PokeathlonStat4 i = 0; i < PokeathlonStat4.Count; i++)
            result += GetCourseRecord(i).ScoreMax;
        result += Medals.GetTotalCount();
        for (PokeathlonEvent4 i = 0; i < PokeathlonEvent4.Count; i++)
            result += GetBestScore(i);
        return result;
    }

    public static ReadOnlySpan<ushort> FriendshipTrophyThresholds => [3000, 3100, 3200, 3300, 3400, 3600, 3800, 4000, 4200, 4500];

    public static int CalculateFriendshipTrophyCount(uint globalScore)
    {
        int result = 0;
        foreach (var threshold in FriendshipTrophyThresholds)
        {
            if (globalScore >= threshold)
                result++;
            else
                break;
        }
        return result; // 10 max
    }
}
