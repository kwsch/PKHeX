using System.Runtime.CompilerServices;
using static PKHeX.Core.SlotType4;

namespace PKHeX.Core;

/// <summary>
/// Encounter slot determination for <see cref="MethodJ"/>.
/// </summary>
public static class SlotMethodJ
{
    private const byte Invalid = byte.MaxValue; // all slots are [0,X], unsigned. This will always result in a non-match.

    /// <summary>
    /// Gets the <see cref="INumberedSlot.SlotNumber"/> from the raw 16bit <see cref="rand"/> seed half.
    /// </summary>
    public static int GetSlot(SlotType4 type, uint rand)
    {
        uint ESV = rand / 656;
        return type switch
        {
            Old_Rod or Surf => GetSurf(ESV),
            Good_Rod or Super_Rod => GetSuperRod(ESV),
            HoneyTree => 0,
            _ => GetRegular(ESV),
        };
    }

    /// <summary>
    /// Gets the range that a given slot number is allowed to roll for a specific <see cref="SlotType4"/>.
    /// </summary>
    public static (byte Min, byte Max) GetRange(SlotType4 type, byte slotNumber) => type switch
    {
        Old_Rod or Surf => GetRangeSurf(slotNumber),
        Good_Rod or Super_Rod => GetRangeSuperRod(slotNumber),
        HoneyTree => (0, 99), // Fake
        _ => GetRangeGrass(slotNumber),
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
    /// Calculates the encounter slot index based on the roll for a D/P/Pt Super Rod encounter.
    /// </summary>
    /// <param name="roll">[0,100)</param>
    public static int GetSuperRod(uint roll) => roll switch
    {
        < 40 => 0, // 00,39 (40%)
        < 80 => 1, // 40,79 (40%)
        < 95 => 2, // 80,94 (15%)
        < 99 => 3, // 95,98 ( 4%)
          99 => 4, //    99 ( 1%)
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
        1 => (40, 79), // (40%)
        2 => (80, 94), // (15%)
        3 => (95, 98), // ( 4%)
        4 => (99, 99), // ( 1%)
        _ => (Invalid, Invalid),
    };
}
