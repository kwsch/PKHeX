using static PKHeX.Core.SlotType3;

namespace PKHeX.Core;

/// <summary>
/// Encounter slot determination for <see cref="MethodH"/>.
/// </summary>
public static class SlotMethodH
{
    private const byte Invalid = byte.MaxValue; // all slots are [0,X], unsigned. This will always result in a non-match.

    /// <summary>
    /// Gets the <see cref="INumberedSlot.SlotNumber"/> from the raw 16bit <see cref="rand"/> seed half.
    /// </summary>
    public static byte GetSlot(SlotType3 type, uint rand) => type switch
    {
        Grass => GetRegular(rand % 100),
        Surf => GetSurf(rand % 100),
        Old_Rod => GetOldRod(rand % 100),
        Good_Rod => GetGoodRod(rand % 100),
        Super_Rod => GetSuperRod(rand % 100),
        Rock_Smash => GetSurf(rand % 100),

        SwarmFish50 => (rand % 100 < 50) ? (byte)0 : Invalid,
        SwarmGrass50 => (rand % 100 < 50) ? (byte)0 : Invalid,
        _ => Invalid,
    };

    /// <summary>
    /// Gets the range that a given slot number is allowed to roll for a specific <see cref="SlotType3"/>.
    /// </summary>
    public static (byte Min, byte Max) GetRange(SlotType3 type, byte slotNumber) => type switch
    {
        Grass => GetRangeGrass(slotNumber),
        Surf => GetRangeSurf(slotNumber),
        Old_Rod => GetRangeOldRod(slotNumber),
        Good_Rod => GetRangeGoodRod(slotNumber),
        Super_Rod => GetRangeSuperRod(slotNumber),
        Rock_Smash => GetRangeSurf(slotNumber),

        SwarmFish50 => (0, 49),
        SwarmGrass50 => (0, 49),
        _ => (Invalid, Invalid),
    };

    /// <summary>
    /// Calculates the encounter slot index based on the roll for a Gen3 Wild encounter.
    /// </summary>
    /// <param name="roll">[0,100)</param>
    public static byte GetRegular(uint roll) => roll switch
    {
        < 20 => 0, // 00,19 (20%)
        < 40 => 1, // 20,39 (20%)
        < 50 => 2, // 40,49 (10%)
        < 60 => 3, // 50,59 (10%)
        < 70 => 4, // 60,69 (10%)
        < 80 => 5, // 70,79 (10%)
        < 85 => 6, // 80,84 ( 5%)
        < 90 => 7, // 85,89 ( 5%)
        < 94 => 8, // 90,93 ( 4%)
        < 98 => 9, // 94,97 ( 4%)
        < 99 => 10,// 98,98 ( 1%)
          99 => 11,//    99 ( 1%)
           _ => Invalid,
    };

    /// <summary>
    /// Calculates the encounter slot index based on the roll for a Gen3 Surf encounter.
    /// </summary>
    /// <param name="roll">[0,100)</param>
    public static byte GetSurf(uint roll) => roll switch
    {
        < 60 => 0, // 00,59 (60%)
        < 90 => 1, // 60,89 (30%)
        < 95 => 2, // 90,94 ( 5%)
        < 99 => 3, // 95,98 ( 4%)
          99 => 4, //    99 ( 1%)
           _ => Invalid,
    };

    /// <summary>
    /// Calculates the encounter slot index based on the roll for a Gen3 Old Rod encounter.
    /// </summary>
    /// <param name="roll">[0,100)</param>
    public static byte GetOldRod(uint roll) => roll switch
    {
        < 70 => 0, // 00,69 (70%)
       <= 99 => 1, // 70,99 (30%)
           _ => Invalid,
    };

    /// <summary>
    /// Calculates the encounter slot index based on the roll for a Gen3 Good Rod encounter.
    /// </summary>
    /// <param name="roll">[0,100)</param>
    public static byte GetGoodRod(uint roll) => roll switch
    {
        < 60 => 0, // 00,59 (60%)
        < 80 => 1, // 60,79 (20%)
       <= 99 => 2, // 80,99 (20%)
           _ => Invalid,
    };

    /// <summary>
    /// Calculates the encounter slot index based on the roll for a Gen3 Super Rod encounter.
    /// </summary>
    /// <param name="roll">[0,100)</param>
    public static byte GetSuperRod(uint roll) => roll switch
    {
        < 40 => 0, // 00,39 (40%)
        < 80 => 1, // 40,69 (30%)
        < 95 => 2, // 70,94 (25%)
        < 99 => 3, // 95,98 ( 4%)
          99 => 4, //    99 ( 1%)
           _ => Invalid,
    };

    /// <summary>
    /// Gets the range a given slot number is allowed to roll in for a <see cref="Grass"/> encounter.
    /// </summary>
    public static (byte Min, byte Max) GetRangeGrass(byte slotNumber) => slotNumber switch
    {
        0 => (00, 19), // (20%)
        1 => (20, 39), // (20%)
        2 => (40, 49), // (10%)
        3 => (50, 59), // (10%)
        4 => (60, 69), // (10%)
        5 => (70, 79), // (10%)
        6 => (80, 84), // ( 5%)
        7 => (85, 89), // ( 5%)
        8 => (90, 93), // ( 4%)
        9 => (94, 97), // ( 4%)
        10=> (98, 98), // ( 1%)
        11=> (99, 99), // ( 1%)
        _ => (Invalid, Invalid),
    };

    /// <summary>
    /// Gets the range a given slot number is allowed to roll in for a <see cref="Surf"/> encounter.
    /// </summary>
    public static (byte Min, byte Max) GetRangeSurf(byte slotNumber) => slotNumber switch
    {
        0 => (00, 59), // (60%)
        1 => (60, 89), // (30%)
        2 => (90, 94), // ( 5%)
        3 => (95, 98), // ( 4%)
        4 => (99, 99), // ( 1%)
        _ => (Invalid, Invalid),
    };

    /// <summary>
    /// Gets the range a given slot number is allowed to roll in for a <see cref="Old_Rod"/> encounter.
    /// </summary>
    public static (byte Min, byte Max) GetRangeOldRod(byte slotNumber) => slotNumber switch
    {
        0 => (00, 69), // (70%)
        1 => (70, 99), // (30%)
        _ => (Invalid, Invalid),
    };

    /// <summary>
    /// Gets the range a given slot number is allowed to roll in for a <see cref="Good_Rod"/> encounter.
    /// </summary>
    public static (byte Min, byte Max) GetRangeGoodRod(byte slotNumber) => slotNumber switch
    {
        0 => (00, 59), // (60%)
        1 => (60, 79), // (20%)
        2 => (80, 99), // (20%)
        _ => (Invalid, Invalid),
    };

    /// <summary>
    /// Gets the range a given slot number is allowed to roll in for a <see cref="Super_Rod"/> encounter.
    /// </summary>
    public static (byte Min, byte Max) GetRangeSuperRod(byte slotNumber) => slotNumber switch
    {
        0 => (00, 39), // (40%)
        1 => (40, 69), // (30%)
        2 => (70, 94), // (25%)
        3 => (95, 98), // ( 4%)
        4 => (99, 99), // ( 1%)
        _ => (Invalid, Invalid),
    };
}
