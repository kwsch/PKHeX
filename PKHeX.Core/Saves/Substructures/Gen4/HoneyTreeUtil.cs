using System;
using static System.Buffers.Binary.BinaryPrimitives;
using static PKHeX.Core.HoneyTreeSlotGroup;

namespace PKHeX.Core;

/// <summary>
/// Utility class for calculating honey trees for <see cref="GameVersion.DPPt"/> and <see cref="GameVersion.BDSP"/>.
/// </summary>
public static class HoneyTreeUtil
{
    /// <summary>
    /// Number of possible honey trees that exist in the game.
    /// </summary>
    private const byte HoneyTreeCount = 21;

    /// <summary>
    /// Populates the given span with the 4 possible honey trees for the given ID.
    /// </summary>
    /// <param name="id">The 32-bit Trainer ID to calculate the trees for.</param>
    /// <param name="result">Result span that will be populated with the 4 possible trees.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static void CalculateMunchlaxTrees(uint id, Span<byte> result)
    {
        if (result.Length != sizeof(uint))
            throw new ArgumentOutOfRangeException(nameof(result));
        WriteUInt32BigEndian(result, id);
        foreach (ref var b in result)
            b %= HoneyTreeCount;

        AdjustOverlap(result);
    }

    private static void AdjustOverlap(Span<byte> result)
    {
        // If a tree is the same as any previous one, increment it and reset to 0 if it overflows the max.
        // Yields 3-4 unique trees per ID. 1935328924d => {10,6,9,10}
        // The original intent was likely to have 4 rare trees per save file; if so, the implementation was flawed.
        // (the inner loop should have restarted if it detected a duplicate, instead of continuing to the next tree)
        for (int i = 1; i < result.Length; i++)
        {
            ref var b = ref result[i];
            for (int j = 0; j < i; j++)
            {
                if (b != result[j])
                    continue;
                b++;
                if (b >= HoneyTreeCount)
                    b = 0;
            }
        }
    }

    // There aren't any RNG considerations for Munchlax/etc. encounter slot legality.
    // The RNG calls for populating the tree are disjointed from the RNG calls for generating the Entity upon encounter.
    // Group -> Slot -> Shakes

    /// <summary>
    /// Calculates the honey tree result for the given seed when the tree elapses.
    /// </summary>
    /// <param name="seed">Current RNG state seed of the game.</param>
    /// <param name="isMunchlaxTree">If the tree is a "rare" tree based on <see cref="CalculateMunchlaxTrees"/>.</param>"/>
    /// <remarks><see cref="GameVersion.DPPt"/></remarks>
    public static (HoneyTreeSlotGroup Group, int Slot, int Shakes) GetHoneyTreeResult(uint seed, bool isMunchlaxTree)
    {
        var randGroup  = LCRNG.Next16(ref seed) / 656;
        var randSlot   = LCRNG.Next16(ref seed) / 656;
        var randShakes = LCRNG.Next16(ref seed) / 656;

        var group = GetHoneyTreeGroup(randGroup, isMunchlaxTree);
        var slot = GetHoneyTreeSlotIndex(randSlot);
        var shakes = GetShakeCount(randShakes, group);

        return (group, slot, shakes);
    }

    /// <summary>
    /// Indicates which slot group rarity inhabits a honey tree.
    /// </summary>
    public static HoneyTreeSlotGroup GetHoneyTreeGroup(uint rnd, bool isMunchlaxTree) => isMunchlaxTree switch
    {
        true => rnd switch
        {
            0    => Munchlax, // 1%
            < 10 => None,     // 9% fail
            < 30 => Common,   // 20%
            _    => Rare,     // 70%
        },
        _ => rnd switch
        {
            < 10 => None,   // 10% fail
            < 30 => Rare,   // 20%
            _    => Common, // 70%
        },
    };

    /// <summary>
    /// Indicates which slot index of the group inhabits in the honey tree.
    /// </summary>
    public static int GetHoneyTreeSlotIndex(uint rnd) => rnd switch
    {
        < 05 => 5, // 5%
        < 10 => 4, // 5%
        < 20 => 3, // 10%
        < 40 => 2, // 20%
        < 60 => 1, // 20%
        _    => 0, // 20%
    };

    /// <summary>
    /// Indicates how many times the tree shakes before the encounter.
    /// </summary>
    public static int GetShakeCount(uint rnd, HoneyTreeSlotGroup group) => group switch
    {
        Common => rnd switch
        {
          < 19 => 2, // 19%
          < 79 => 1, // 60%
          < 99 => 0, // 20%
            _  => 3, // 1%
        },
        Rare => rnd switch
        {
          < 75 => 2, // 75%
          < 95 => 1, // 20%
            95 => 0, // 1%
            _  => 3, // 4%
        },
        Munchlax => rnd switch
        {
          < 5  => 2, // 5%
            5  => 1, // 1%
            6  => 0, // 1%
            _  => 3, // 93%
        },
        _ => 0,
    };
}

/// <summary>
/// Indicates which slot group rarity inhabits a honey tree.
/// </summary>
public enum HoneyTreeSlotGroup : byte
{
    /// <summary> No species inhabits the tree. </summary>
    None,
    /// <summary> Common species inhabit the tree. </summary>
    Common,
    /// <summary> Rare species inhabit the tree. </summary>
    Rare,
    /// <summary> Munchlax inhabits the tree. </summary>
    Munchlax,
}
