namespace PKHeX.Core;

/// <summary>
/// Converts a slot random roll [0, 100) to a slot number.
/// </summary>
public static class SlotNumber
{
    private const int Invalid = -1;

    /// <summary>
    /// Calculates the encounter slot index based on the roll for a Gen3 Old Rod encounter.
    /// </summary>
    /// <param name="roll">[0,100)</param>
    public static int GetHOldRod(uint roll) => roll switch
    {
        < 70 => 0, // 00,69 (70%)
        <=99 => 1, // 70,99 (30%)
        _ => Invalid,
    };

    /// <summary>
    /// Calculates the encounter slot index based on the roll for a Gen3 Good Rod encounter.
    /// </summary>
    /// <param name="roll">[0,100)</param>
    public static int GetHGoodRod(uint roll) => roll switch
    {
        < 60 => 0, // 00,59 (60%)
        < 80 => 1, // 60,79 (20%)
        <=99 => 2, // 80,99 (20%)
        _ => Invalid,
    };

    /// <summary>
    /// Calculates the encounter slot index based on the roll for a Gen3 Super Rod encounter.
    /// </summary>
    /// <param name="roll">[0,100)</param>
    public static int GetHSuperRod(uint roll) => roll switch
    {
        < 40 => 0, // 00,39 (40%)
        < 80 => 1, // 40,69 (40%)
        < 95 => 2, // 70,94 (15%)
        < 99 => 3, // 95,98 ( 4%)
          99 => 4, //    99 ( 1%)
        _ => Invalid,
    };

    /// <summary>
    /// Calculates the encounter slot index based on the roll for a Gen3 Surf encounter.
    /// </summary>
    /// <param name="roll">[0,100)</param>
    public static int GetHSurf(uint roll) => roll switch
    {
        < 60 => 0, // 00,59 (60%)
        < 90 => 1, // 60,89 (30%)
        < 95 => 2, // 90,94 ( 5%)
        < 99 => 3, // 95,98 ( 4%)
          99 => 4, //    99 ( 1%)
        _ => Invalid,
    };

    /// <summary>
    /// Calculates the encounter slot index based on the roll for a Gen3 Wild encounter.
    /// </summary>
    /// <param name="roll">[0,100)</param>
    public static int GetHRegular(uint roll) => roll switch
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
    /// Calculates the encounter slot index based on the roll for a D/P/Pt Super Rod encounter.
    /// </summary>
    /// <param name="roll">[0,100)</param>
    public static int GetJSuperRod(uint roll) => roll switch
    {
        < 40 => 0, // 00,39 (40%)
        < 80 => 1, // 40,79 (40%)
        < 95 => 2, // 80,94 (15%)
        < 99 => 3, // 95,98 ( 4%)
          99 => 4, //    99 ( 1%)
        _ => Invalid,
    };

    /// <summary>
    /// Calculates the encounter slot index based on the roll for a HG/SS Super Rod encounter.
    /// </summary>
    /// <param name="roll">[0,100)</param>
    public static int GetKSuperRod(uint roll) => roll switch
    {
        < 40 => 0, // 00,39 (40%)
        < 70 => 1, // 40,69 (30%)
        < 85 => 2, // 70,84 (15%)
        < 95 => 3, // 85,94 (10%)
          99 => 4, //    95 ( 5%)
        _ => Invalid,
    };

    /// <summary>
    /// Calculates the encounter slot index based on the roll for a Bug Catching Contest encounter.
    /// </summary>
    /// <param name="roll">[0,100)</param>
    /// <remarks>
    /// Slot indexes are reversed for the Bug Catching Contest.
    /// </remarks>
    public static int GetKBCC(uint roll) => roll switch
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
        < 100=> 0, // 80,99 (20%)
        _ => Invalid,
    };

    /// <summary>
    /// Calculates the encounter slot index based on the roll for a HG/SS Headbutt encounter.
    /// </summary>
    /// <param name="roll">[0,100)</param>
    public static int GetKHeadbutt(uint roll) => roll switch
    {
        < 50 => 0, // 00,49 (50%)
        < 65 => 1, // 50,64 (15%)
        < 80 => 2, // 65,79 (15%)
        < 90 => 3, // 80,89 (10%)
        < 95 => 4, // 90,94 ( 5%)
        <=99 => 5, // 95,99 ( 5%)
        _ => Invalid,
    };
}
