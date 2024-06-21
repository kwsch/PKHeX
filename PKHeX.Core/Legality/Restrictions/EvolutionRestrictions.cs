using System;
using static PKHeX.Core.Move;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Restriction logic for evolutions that are a little more complex than <see cref="EvolutionMethod"/> can simply check.
/// </summary>
/// <remarks>
/// Currently only checks "is able to know a move required to level up".
/// </remarks>
internal static class EvolutionRestrictions
{
    /// <summary>
    /// List of species that evolve from a previous species having a move while leveling up
    /// </summary>
    private static ushort GetSpeciesEvolutionMove(ushort species) => species switch
    {
        (int)Sylveon => EEVEE,
        (int)MrMime => (int)Mimic,
        (int)Sudowoodo => (int)Mimic,
        (int)Ambipom => (int)DoubleHit,
        (int)Lickilicky => (int)Rollout,
        (int)Tangrowth => (int)AncientPower,
        (int)Yanmega => (int)AncientPower,
        (int)Mamoswine => (int)AncientPower,
        (int)Tsareena => (int)Stomp,
        (int)Grapploct => (int)Taunt,
        (int)Wyrdeer => (int)PsyshieldBash,
        (int)Overqwil => (int)BarbBarrage,
        (int)Annihilape => (int)RageFist,
        (int)Farigiraf => (int)TwinBeam,
        (int)Dudunsparce => (int)HyperDrill,
        (int)Hydrapple => (int)DragonCheer,
        _ => NONE,
    };

    /// <summary>
    /// Gets the species-form that it will evolve into.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static (ushort Species, byte Form) GetEvolvedSpeciesFormEC100(ushort species, bool rare) => species switch
    {
        (ushort)Tandemaus => ((ushort)Maushold, (byte)(rare ? 0 : 1)),
        (ushort)Dunsparce => ((ushort)Dudunsparce, (byte)(rare ? 1 : 0)),
        _ => throw new ArgumentOutOfRangeException(nameof(species), species, "Incorrect EC%100 species."),
    };

    public static bool GetIsExpectedEvolveFormEC100(ushort species, byte form, bool rare) => species switch
    {
        (ushort)Maushold => form == (byte)(rare ? 0 : 1),
        (ushort)Dudunsparce => form == (byte)(rare ? 1 : 0),
        _ => throw new ArgumentOutOfRangeException(nameof(species), species, "Incorrect EC%100 species."),
    };

    public static bool IsFormArgEvolution(ushort species)
    {
        return species is (int)Runerigus or (int)Wyrdeer or (int)Annihilape or (int)Basculegion or (int)Kingambit or (int)Overqwil;
    }

    private const ushort NONE = 0;
    private const ushort EEVEE = ushort.MaxValue;

    private static ReadOnlySpan<ushort> EeveeFairyMoves =>
    [
        (int)Charm,
        (int)BabyDollEyes,
    ];

    /// <summary>
    /// Checks if the <see cref="pk"/> is correctly evolved, assuming it had a known move requirement evolution in its evolution chain.
    /// </summary>
    /// <returns>True if unnecessary to check or the evolution was valid.</returns>
    public static bool IsValidEvolutionWithMove(PKM pk, LegalInfo info)
    {
        // Known-move evolutions were introduced in Gen4.
        if (pk.Format < 4) // doesn't exist yet!
            return true;

        // OK if un-evolved from original encounter
        var enc = info.EncounterOriginal;
        ushort species = pk.Species;
        if (enc.Species == species)
            return true;

        // Exclude evolution paths that did not require a move w/level-up evolution
        var move = GetSpeciesEvolutionMove(species);
        if (move is NONE)
            return true; // not a move evolution
        if (move is EEVEE)
            return IsValidEvolutionWithMoveSylveon(pk, enc, info);
        if (!IsMoveSlotAvailable(info.Moves))
            return false;

        if (pk.HasMove(move))
            return true;

        // Check the entire chain to see if it could have learnt it at any point.
        var head = LearnGroupUtil.GetCurrentGroup(pk);
        return MemoryPermissions.GetCanKnowMove(enc, move, info.EvoChainsAllGens, pk, head);
    }

    private static bool IsValidEvolutionWithMoveSylveon(PKM pk, IEncounterTemplate enc, LegalInfo info)
    {
        if (!IsMoveSlotAvailable(info.Moves))
            return false;

        foreach (var move in EeveeFairyMoves)
        {
            if (pk.HasMove(move))
                return true;
        }

        var head = LearnGroupUtil.GetCurrentGroup(pk);
        foreach (var move in EeveeFairyMoves)
        {
            if (MemoryPermissions.GetCanKnowMove(enc, move, info.EvoChainsAllGens, pk, head))
                return true;
        }
        return false;
    }

    private static bool IsMoveSlotAvailable(ReadOnlySpan<MoveResult> moves)
    {
        // If the Pokémon does not currently have the move, it could have been an egg move that was forgotten.
        // This requires the Pokémon to not have 4 other moves identified as egg moves or inherited level up moves.
        // If any move is not an egg source, then a slot could have been forgotten.
        foreach (var move in moves)
        {
            if (!move.Info.Method.IsEggSource())
                return true;
        }
        return false;
    }
}
