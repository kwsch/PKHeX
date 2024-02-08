using System.Runtime.CompilerServices;
using static PKHeX.Core.SlotType;

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
    public static int GetSlot(SlotType type, uint rand)
    {
        uint ESV = rand / 656;
        return type switch
        {
            Old_Rod or Rock_Smash or Surf => GetSurf(ESV),
            Good_Rod or Super_Rod => GetSuperRod(ESV),
            HoneyTree => 0,
            _ => GetRegular(ESV),
        };
    }

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
}
