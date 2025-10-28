using System.Runtime.CompilerServices;
using static PKHeX.Core.SlotType4;

namespace PKHeX.Core;

/// <summary>
/// Encounter slot determination for <see cref="MethodK"/>.
/// </summary>
public static class SlotMethodK
{
    private const byte Invalid = byte.MaxValue; // all slots are [0,X], unsigned. This will always result in a non-match.

    /// <summary>
    /// Gets the <see cref="INumberedSlot.SlotNumber"/> from the raw 16bit <see cref="rand"/> seed half.
    /// </summary>
    public static byte GetSlot(SlotType4 type, uint rand) => type switch
    {
        Grass                            => GetRegular(rand % 100),
        Surf                             => GetSurf(rand % 100),
        Old_Rod or Good_Rod or Super_Rod => GetSuperRod(rand % 100),
        Rock_Smash                       => GetRockSmash(rand % 100),
        Headbutt or HeadbuttSpecial      => GetHeadbutt(rand % 100),
        BugContest                       => GetBugCatchingContest(rand % 100),
        // Honey Tree shouldn't enter here.
        Safari_Grass or Safari_Surf or
        Safari_Old_Rod or Safari_Good_Rod or Safari_Super_Rod => GetSafari(rand % 10),
        _ => Invalid,
    };

    /// <summary>
    /// Gets the range that a given slot number is allowed to roll for a specific <see cref="SlotType4"/>.
    /// </summary>
    public static (byte Min, byte Max) GetRange(SlotType4 type, byte slotNumber) => type switch
    {
        Grass                            => GetRangeGrass(slotNumber),
        Surf                             => GetRangeSurf(slotNumber),
        Old_Rod or Good_Rod or Super_Rod => GetRangeSuperRod(slotNumber),
        Rock_Smash                       => GetRangeRockSmash(slotNumber),
        Headbutt or HeadbuttSpecial      => GetRangeHeadbutt(slotNumber),
        BugContest                       => GetRangeBugCatchingContest(slotNumber),
        Safari_Grass or Safari_Surf or
        Safari_Old_Rod or Safari_Good_Rod or Safari_Super_Rod => GetRangeSafari(slotNumber),
        _ => (Invalid, Invalid),
    };

    /// <summary>
    /// Calculates the encounter slot index based on the roll for a Gen4 Wild encounter.
    /// </summary>
    /// <param name="roll">[0,100)</param>
    /// <remarks>Same as <see cref="SlotMethodH.GetRegular"/></remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetRegular(uint roll) => SlotMethodH.GetRegular(roll);

    /// <summary>
    /// Calculates the encounter slot index based on the roll for a Gen4 Surf encounter.
    /// </summary>
    /// <param name="roll">[0,100)</param>
    /// <remarks>Same as <see cref="SlotMethodH.GetSurf"/></remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetSurf(uint roll) => SlotMethodH.GetSurf(roll);

    /// <summary>
    /// Calculates the encounter slot index based on the roll for a Gen4 Rock Smash encounter.
    /// </summary>
    /// <param name="roll">[0,100)</param>
    public static byte GetRockSmash(uint roll) => roll >= 80 ? (byte)1 : (byte)0;

    /// <summary>
    /// Calculates the encounter slot index based on the roll for a HG/SS Super Rod encounter.
    /// </summary>
    /// <param name="roll">[0,100)</param>
    public static byte GetSuperRod(uint roll) => roll switch
    {
        < 40 => 0, // 00,39 (40%)
        < 70 => 1, // 40,69 (30%)
        < 85 => 2, // 70,84 (15%)
        < 95 => 3, // 85,94 (10%)
        <100 => 4, //    95 ( 5%)
           _ => Invalid,
    };

    /// <summary>
    /// Calculates the encounter slot index based on the roll for a Safari Zone encounter.
    /// </summary>
    /// <param name="roll">[0,9] raw roll</param>
    /// <returns></returns>
    public static byte GetSafari(uint roll) => (byte)roll;

    /// <summary>
    /// Calculates the encounter slot index based on the roll for a Bug Catching Contest encounter.
    /// </summary>
    /// <param name="roll">[0,100)</param>
    /// <remarks>
    /// Slot indexes are reversed for the Bug Catching Contest.
    /// </remarks>
    public static byte GetBugCatchingContest(uint roll) => roll switch
    {
        < 05 => 9, // 00,04 ( 5%)
        < 10 => 8, // 05,09 ( 5%)
        < 15 => 7, // 10,14 ( 5%)
        < 20 => 6, // 15,19 ( 5%)
        < 30 => 5, // 20,29 (10%)
        < 40 => 4, // 30,39 (10%)
        < 50 => 3, // 40,49 (10%)
        < 60 => 2, // 50,59 (10%)
        < 80 => 1, // 60,79 (20%)
        <100 => 0, // 80,99 (20%)
           _ => Invalid,
    };

    /// <summary>
    /// Calculates the encounter slot index based on the roll for a HG/SS Headbutt encounter.
    /// </summary>
    /// <param name="roll">[0,100)</param>
    public static byte GetHeadbutt(uint roll) => roll switch
    {
        < 50 => 0, // 00,49 (50%)
        < 65 => 1, // 50,64 (15%)
        < 80 => 2, // 65,79 (15%)
        < 90 => 3, // 80,89 (10%)
        < 95 => 4, // 90,94 ( 5%)
        <100 => 5, // 95,99 ( 5%)
           _ => Invalid,
    };

    /// <summary>
    /// Gets the range a given slot number is allowed to roll in for a <see cref="Grass"/> encounter.
    /// </summary>
    public static (byte Min, byte Max) GetRangeGrass(byte slotNumber) => SlotMethodH.GetRangeGrass(slotNumber);

    /// <summary>
    /// Gets the range a given slot number is allowed to roll in for a <see cref="Surf"/> encounter.
    /// </summary>
    public static (byte Min, byte Max) GetRangeSurf(byte slotNumber) => SlotMethodH.GetRangeSurf(slotNumber);

    /// <summary>
    /// Gets the range a given slot number is allowed to roll in for a <see cref="Super_Rod"/> encounter.
    /// </summary>
    public static (byte Min, byte Max) GetRangeSuperRod(byte slotNumber) => slotNumber switch
    {
        0 => (00, 39), // (40%)
        1 => (40, 69), // (30%)
        2 => (70, 84), // (15%)
        3 => (85, 94), // (10%)
        4 => (95, 99), // ( 5%)
        _ => (Invalid, Invalid),
    };

    /// <summary>
    /// Gets the range a given slot number is allowed to roll in for a <see cref="Rock_Smash"/> encounter.
    /// </summary>
    public static (byte Min, byte Max) GetRangeRockSmash(byte slotNumber) => slotNumber switch
    {
        0 => (00, 79), // (80%)
        1 => (80, 99), // (20%)
        _ => (Invalid, Invalid),
    };

    /// <summary>
    /// Gets the range a given slot number is allowed to roll in for a <see cref="Headbutt"/> encounter.
    /// </summary>
    public static (byte Min, byte Max) GetRangeHeadbutt(byte slotNumber) => slotNumber switch
    {
        0 => (00, 49), // (50%)
        1 => (50, 64), // (15%)
        2 => (65, 79), // (15%)
        3 => (80, 89), // (10%)
        4 => (90, 94), // ( 5%)
        5 => (95, 99), // ( 5%)
        _ => (Invalid, Invalid),
    };

    /// <summary>
    /// Gets the range a given slot number is allowed to roll in for a <see cref="BugContest"/> encounter.
    /// </summary>
    public static (byte Min, byte Max) GetRangeBugCatchingContest(byte slotNumber) => slotNumber switch
    {
        9 => (00, 04), // ( 5%)
        8 => (05, 09), // ( 5%)
        7 => (10, 14), // ( 5%)
        6 => (15, 19), // ( 5%)
        5 => (20, 29), // (10%)
        4 => (30, 39), // (10%)
        3 => (40, 49), // (10%)
        2 => (50, 59), // (10%)
        1 => (60, 79), // (20%)
        0 => (80, 99), // (20%)
        _ => (Invalid, Invalid),
    };

    /// <summary>
    /// Gets the range a given slot number is allowed to roll in for a <see cref="Safari_Grass"/> (and other Safari Types) encounter.
    /// </summary>
    public static (byte Min, byte Max) GetRangeSafari(byte slotNumber) => (slotNumber, slotNumber);
}
