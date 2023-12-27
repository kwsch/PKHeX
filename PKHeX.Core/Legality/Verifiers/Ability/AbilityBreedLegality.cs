using System;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Tables used for <see cref="AbilityVerifier"/>
/// </summary>
internal static class AbilityBreedLegality
{
    private static ReadOnlySpan<byte> BanHidden5 =>
    [
        0x92, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02,
        0x10, 0x10, 0x20, 0x00, 0x01, 0x11, 0x02, 0x00, 0x49, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x10, 0x00, 0x90, 0x04,
        0x00, 0x00, 0x80, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x86, 0x80,
        0x49, 0x00, 0x40, 0x00, 0x48, 0x02, 0x00, 0x00, 0x10, 0x00, 0x12,
        0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x82, 0x24, 0x80, 0x0A, 0x00,
        0x00, 0x0C, 0x00, 0x00, 0x44, 0x44, 0x00, 0x00, 0xA0, 0x84, 0x80,
        0x40, 0x08, 0x12,
    ];

    /// <summary>
    /// Species that cannot be bred with a Hidden Ability originating in <see cref="GameVersion.Gen5"/>
    /// </summary>
    public static bool IsHiddenPossible5(ushort species)
    {
        var index = species >> 3;
        var table = BanHidden5;
        if (index >= table.Length)
            return true;
        return (BanHidden5[index] & (1 << (species & 7))) == 0;
    }

    /// <summary>
    /// Species that cannot be bred with a Hidden Ability originating in <see cref="GameVersion.Gen6"/>
    /// </summary>
    public static bool IsHiddenPossible6(ushort species, byte form) => species switch
    {
        // Same abilities (1/2/H), not available as H
        (int)Castform => false,
        (int)Carnivine => false,
        (int)Rotom => false,
        (int)Phione => false,
        (int)Archen => false,
        (int)Cryogonal => false,

        (int)Flabébé => form is not (2 or 4), // Orange or White - not available in Friend Safari or Horde
        (int)Honedge => false,
        (int)Furfrou => false,
        (int)Pumpkaboo => form is not (1 or 2), // Previous-Gen: Size & Ability inherit from mother

        _ => true,
    };

    /// <summary>
    /// Species that cannot be bred with a Hidden Ability originating in <see cref="GameVersion.Gen7"/>
    /// </summary>
    public static bool IsHiddenPossible7(ushort species, byte form) => species switch
    {
        // Same abilities (1/2/H), not available as H
        (int)Carnivine => false,
        (int)Rotom => false,
        (int)Phione => false,
        (int)Archen => false,
        (int)Cryogonal => false,
        (int)Honedge => false,
        (int)Pumpkaboo => form is not (1 or 2), // Previous-Gen: Size & Ability inherit from mother

        (int)Minior => false, // No SOS Encounter
        (int)Wimpod => false, // SOS slots have 0 call rate
        (int)Komala => false, // SOS slots have 0 call rate
        _ => true,
    };

    /// <summary>
    /// Species that cannot be bred with a Hidden Ability originating in <see cref="GameVersion.BDSP"/>
    /// </summary>
    public static bool IsHiddenPossibleHOME(ushort eggSpecies) => eggSpecies is not (int)Phione; // Everything else can!
}
