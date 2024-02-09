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
    public static byte GetSlot(SlotType type, uint rand)
    {
        var ESV = rand % 100;
        if ((type & SlotType.Swarm) != 0)
            return ESV < 50 ? (byte)0 : Invalid;

        return type switch
        {
            SlotType.Old_Rod => GetOldRod(ESV),
            SlotType.Good_Rod => GetGoodRod(ESV),
            SlotType.Super_Rod => GetSuperRod(ESV),
            SlotType.Rock_Smash => GetSurf(ESV),
            SlotType.Surf => GetSurf(ESV),
            _ => GetRegular(ESV),
        };
    }

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
        < 80 => 1, // 40,69 (40%)
        < 95 => 2, // 70,94 (15%)
        < 99 => 3, // 95,98 ( 4%)
          99 => 4, //    99 ( 1%)
           _ => Invalid,
    };
}
