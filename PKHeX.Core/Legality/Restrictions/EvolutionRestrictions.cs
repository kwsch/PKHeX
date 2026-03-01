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
    /// <remarks>
    /// Using moves with a counter stored in form argument is explicitly checked via other logic. <see cref="IsFormArgEvolution"/>
    /// </remarks>
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
        (int)Naganadel => (int)DragonPulse,
        (int)Grapploct => (int)Taunt,

        // Form Argument evolutions sometimes with extra conditions; verify here because they aren't checked "completely" elsewhere (yet).
        (int)Wyrdeer => (int)PsyshieldBash,
        (int)Overqwil => (int)BarbBarrage,
        (int)Annihilape => (int)RageFist,

        (int)Farigiraf => (int)TwinBeam,
        (int)Dudunsparce => (int)HyperDrill,
        (int)Hydrapple => (int)DragonCheer,
        _ => NONE,
    };

    /// <summary>
    /// Checks if the evolution is the "rare" variant (less common).
    /// </summary>
    /// <param name="encryptionConstant">Random value used to pivot between evolution results.</param>
    public static bool IsEvolvedSpeciesFormRare(uint encryptionConstant) => encryptionConstant % 100 is 0;

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

    /// <summary>
    /// Checks if the evolution result matches what is expected for <see cref="IsEvolvedSpeciesFormRare"/>
    /// </summary>
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
        (int)DisarmingVoice, // Z-A
    ];

    /// <summary>
    /// Checks if the <see cref="pk"/> is correctly evolved, assuming it had a known move requirement evolution in its evolution chain.
    /// </summary>
    /// <returns>True if unnecessary to check or the evolution was valid.</returns>
    /// <remarks>
    /// Performs some eager checks to skip doing a full evolution tree move check.
    /// </remarks>
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

        // All move evolutions arrive at a maximally-evolved species chain.
        // Except for Mr. Rime -- just manually devolve and let the rest of the logic check.
        if (species is (ushort)MrRime)
        {
            species = (ushort)MrMime;
            if (enc.Species == species)
                return true;
        }

        // Exclude evolution paths that did not require a move w/level-up evolution
        var move = GetSpeciesEvolutionMove(species);
        if (move is NONE)
            return true; // not a move evolution
        if (!IsMoveSlotAvailable(info.Moves))
            return false;

        // Check if the move is in relearn moves (can know at any time)
        if (pk.Format >= 6 && IsMoveInRelearnSource(pk, info, move))
            return true;

        // Check the entire chain to see if it could have learnt it at any point.
        var head = LearnGroupUtil.GetCurrentGroup(pk);
        var pruned = info.EvoChainsAllGens.PruneKeepPreEvolutions(species);
        if (move is EEVEE)
            return IsValidEvolutionWithMoveAny(enc, EeveeFairyMoves, pruned, pk, head);

        return MemoryPermissions.GetCanKnowMove(enc, move, pruned, pk, head);
    }

    private static bool IsMoveInRelearnSource(PKM pk, LegalInfo info, ushort move)
    {
        if (move is not EEVEE)
            return IsMoveInRelearn(pk, info, move);
        return IsMoveInRelearn(pk, info, EeveeFairyMoves);
    }

    private static bool IsMoveInRelearn(PKM pk, LegalInfo info, ReadOnlySpan<ushort> arr)
    {
        foreach (var move in arr)
        {
            if (IsMoveInRelearn(pk, info, move))
                return true;
        }
        return false;
    }

    private static bool IsMoveInRelearn(PKM pk, LegalInfo info, ushort move)
    {
        if (pk.IsOriginalMovesetDeleted())
            return WasMoveInRelearn(pk, info, move);

        var first = pk.RelearnMove1;
        if (first is 0) // eager check
            return false;
        if (pk.RelearnMove1 == move)
            return true;
        if (pk.RelearnMove2 == move)
            return true;
        if (pk.RelearnMove3 == move)
            return true;
        if (pk.RelearnMove4 == move)
            return true;
        return false;
    }

    private static bool WasMoveInRelearn(PKM pk, LegalInfo info, ushort move)
    {
        var i = pk.GetMoveIndex(move);
        if (i == -1)
            return false;
        var method = info.Moves[i].Info.Method;
        return method is { IsEggSource: true } or { IsRelearn: true } or LearnMethod.Encounter;
    }

    private static bool IsValidEvolutionWithMoveAny(IEncounterTemplate enc, ReadOnlySpan<ushort> any, EvolutionHistory history, PKM pk, ILearnGroup head)
    {
        foreach (var move in any)
        {
            if (MemoryPermissions.GetCanKnowMove(enc, move, history, pk, head))
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
            if (!move.Info.Method.IsEggSource)
                return true;
        }
        return false;
    }
}
