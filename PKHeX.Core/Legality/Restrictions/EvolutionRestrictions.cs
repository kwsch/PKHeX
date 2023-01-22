using System;
using System.Collections.Generic;
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
    private static readonly Dictionary<int, MoveEvolution> SpeciesEvolutionWithMove = new()
    {
        {(int)Eevee,      new(00, 0)}, // FairyMoves
        {(int)MimeJr,     new(01, (int)Mimic)},
        {(int)Bonsly,     new(02, (int)Mimic)},
        {(int)Aipom,      new(03, (int)Mimic)},
        {(int)Lickitung,  new(04, (int)Rollout)},
        {(int)Tangela,    new(05, (int)AncientPower)},
        {(int)Yanma,      new(06, (int)AncientPower)},
        {(int)Piloswine,  new(07, (int)AncientPower)},
        {(int)Steenee,    new(08, (int)Stomp)},
        {(int)Clobbopus,  new(09, (int)Taunt)},
        {(int)Stantler,   new(10, (int)PsyshieldBash)},
        {(int)Qwilfish,   new(11, (int)BarbBarrage)},
        {(int)Primeape,   new(12, (int)RageFist)},
        {(int)Girafarig,  new(13, (int)TwinBeam)},
        {(int)Dunsparce,  new(14, (int)HyperDrill)},
    };

    private readonly record struct MoveEvolution(int ReferenceIndex, ushort Move);

    private static readonly ushort[] FairyMoves =
    {
        (int)SweetKiss,
        (int)Charm,
        (int)Moonlight,
        (int)DisarmingVoice,
        (int)DrainingKiss,
        (int)CraftyShield,
        (int)FlowerShield,
        (int)MistyTerrain,
        (int)PlayRough,
        (int)FairyWind,
        (int)Moonblast,
        (int)FairyLock,
        (int)AromaticMist,
        (int)Geomancy,
        (int)DazzlingGleam,
        (int)BabyDollEyes,
        (int)LightofRuin,
        (int)TwinkleTackleP,
        (int)TwinkleTackleS,
        (int)FloralHealing,
        (int)GuardianofAlola,
        (int)FleurCannon,
        (int)NaturesMadness,
        (int)LetsSnuggleForever,
        (int)SparklySwirl,
        (int)MaxStarfall,
        (int)Decorate,
        (int)SpiritBreak,
        (int)StrangeSteam,
        (int)MistyExplosion,
        (int)SpringtideStorm,
    };

    /// <summary>
    /// Minimum current level for a given species to have learned the evolve-move and be successfully evolved.
    /// </summary>
    /// <remarks>Having a value of 0 means the move can't be learned.</remarks>
    private static ReadOnlySpan<byte> MinLevelEvolutionWithMove => new byte[]
    {
        00, 00, 00, 00, 00, 29, 09, 02, 02, 02, // Sylveon (Eevee with Fairy Move)
        00, 00, 00, 00, 18, 15, 15, 02, 32, 00, // Mr. Mime (Mime Jr with Mimic)
        00, 00, 00, 00, 17, 17, 15, 02, 16, 16, // Sudowoodo (Bonsly with Mimic)
        00, 00, 00, 00, 32, 32, 32, 02, 32, 00, // Ambipom (Aipom with Double Hit)
        00, 00, 02, 00, 02, 33, 33, 02, 06, 00, // Lickilicky (Lickitung with Rollout)
        00, 00, 00, 00, 02, 36, 38, 02, 24, 00, // Tangrowth (Tangela with Ancient Power)
        00, 00, 00, 00, 02, 33, 33, 02, 33, 00, // Yanmega (Yanma with Ancient Power)
        00, 00, 00, 00, 02, 02, 02, 02, 02, 00, // Mamoswine (Piloswine with Ancient Power)
        00, 00, 00, 00, 00, 00, 00, 02, 28, 28, // Tsareena (Steenee with Stomp)
        00, 00, 00, 00, 00, 00, 00, 00, 35, 00, // Grapploct (Clobbopus with Taunt)
        00, 00, 00, 00, 00, 00, 00, 00, 00, 00, // Wyrdeer (Stantler with AGILE Psyshield Bash)
        00, 00, 00, 00, 00, 00, 00, 00, 00, 00, // Overqwil (Qwilfish-1 with STRONG Barb Barrage)
        00, 00, 00, 00, 00, 00, 00, 00, 00, 35, // Annihilape (Primeape with Rage Fist)
        00, 00, 00, 00, 00, 00, 00, 00, 00, 32, // Farigiraf (Girafarig with Twin Beam)
        00, 00, 00, 00, 00, 00, 00, 00, 00, 32, // Dudunsparce (Dunsparce with Hyper Drill)
    };

    private const int MinLevelEvoWidth = 10;

    private static ReadOnlySpan<byte> MinLevelEvolutionWithMove_8LA => new byte[]
    {
        00, // Sylveon (Eevee with Fairy Move)
        25, // Mr. Mime (Mime Jr with Mimic)
        29, // Sudowoodo (Bonsly with Mimic)
        25, // Ambipom (Aipom with Double Hit)
        34, // Lickilicky (Lickitung with Rollout)
        34, // Tangrowth (Tangela with Ancient Power)
        34, // Yanmega (Yanma with Ancient Power)
        34, // Mamoswine (Piloswine with Ancient Power)
        99, // Tsareena (Steenee with Stomp)
        99, // Grapploct (Clobbopus with Taunt)
        31, // Wyrdeer (Stantler with AGILE Psyshield Bash) 
        25, // Overqwil (Qwilfish-1 with STRONG Barb Barrage)
    };

    private static ReadOnlySpan<byte> CanEggHatchWithEvolveMove => new byte[]
    {
        0, 0, 1, 1, 1, 1, 1, 1, 1, 1, // Sylveon (Eevee with Fairy Move)
        0, 0, 0, 0, 1, 1, 1, 1, 1, 0, // Mr. Mime (Mime Jr with Mimic)
        0, 0, 0, 0, 1, 1, 1, 1, 1, 1, // Sudowoodo (Bonsly with Mimic)
        0, 0, 0, 0, 1, 1, 1, 1, 1, 0, // Ambipom (Aipom with Double Hit)
        0, 0, 1, 0, 1, 1, 1, 1, 1, 0, // Lickilicky (Lickitung with Rollout)
        0, 0, 0, 0, 1, 1, 1, 1, 1, 0, // Tangrowth (Tangela with Ancient Power)
        0, 0, 0, 0, 1, 1, 1, 1, 1, 0, // Yanmega (Yanma with Ancient Power)
        0, 0, 1, 1, 1, 1, 1, 1, 1, 0, // Mamoswine (Piloswine with Ancient Power)
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // Tsareena (Steenee with Stomp)
        0, 0, 0, 0, 0, 0, 0, 0, 1, 0, // Grapploct (Clobbopus with Taunt)
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // Wyrdeer (Stantler with AGILE Psyshield Bash)
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // Overqwil (Qwilfish-1 with STRONG Barb Barrage)
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // Annihilape (Primeape with Rage Fist)
        0, 0, 0, 0, 0, 0, 0, 0, 0, 1, // Farigiraf (Girafarig with Twin Beam)
        0, 0, 0, 0, 0, 0, 0, 0, 0, 1, // Dudunsparce (Dunsparce with Hyper Drill)
    };

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
        ushort species = pk.Species;
        if (info.EncounterMatch.Species == species)
            return true;

        // Exclude evolution paths that did not require a move w/level-up evolution
        var enc = info.EncounterOriginal;
        if (!SpeciesEvolutionWithMove.TryGetValue(enc.Species, out var entry))
            return true;

        var move = entry.Move;
        if (move == 0)
        {
            // Other evolutions are fine.
            if (pk.Species != (int)Sylveon)
                return true;
        }

        // Check if the move was already known when it was originally encountered.
        var gen = info.Generation;
        var index = entry.ReferenceIndex;
        if (enc is EncounterEgg)
        {
            var slice = CanEggHatchWithEvolveMove.Slice(index * MinLevelEvoWidth, MinLevelEvoWidth);
            if (slice[gen] != 0) // true
            {
                var result = move == 0 ? IsMoveInherited(pk, info, FairyMoves) : IsMoveInherited(pk, info, move);
                if (result)
                    return true;
            }
        }
        else if (enc is IMoveset s)
        {
            var moves = s.Moves;
            var result = move == 0 ? moves.ContainsAny(FairyMoves) : moves.Contains(move);
            if (result)
                return true;
        }

        // Current level must be at least the minimum post-evolution level.
        var lvl = GetMinLevelKnowRequiredMove(pk, gen, index, info.EvoChainsAllGens);
        return pk.CurrentLevel >= lvl;
    }

    private static int GetMinLevelKnowRequiredMove(PKM pk, int gen, int index, EvolutionHistory evos)
    {
        var lvl = GetLevelLearnMove(pk, gen, index);

        // If has original met location the minimum evolution level is one level after met level
        // Gen 3 pokemon in gen 4 games: minimum level is one level after transfer to generation 4
        // VC pokemon: minimum level is one level after transfer to generation 7
        // Sylveon: always one level after met level, for gen 4 and 5 Eevee in gen 6 games minimum for evolution is one level after transfer to generation 5
        if (pk.HasOriginalMetLocation || (pk.Format == 4 && gen == 3) || pk.VC || pk.Species == (int)Sylveon)
            lvl = Math.Max(pk.Met_Level + 1, lvl);

        if (evos.HasVisitedPLA) // No Level Up required, and different levels than mainline SW/SH.
        {
            var la = MinLevelEvolutionWithMove_8LA[index];
            if (la <= lvl)
                return la;
        }

        return lvl;
    }

    private static int GetLevelLearnMove(PKM pk, int gen, int index)
    {
        // Get the minimum level in any generation when the pokemon could learn the evolve move
        var levels = MinLevelEvolutionWithMove.Slice(index * MinLevelEvoWidth, MinLevelEvoWidth);
        var lvl = 101;
        var end = pk.Format;
        for (int g = gen; g <= end; g++)
        {
            var l = levels[g];
            if (l == 0)
                continue;
            if (l == 2)
                return 2; // true minimum
            if (l < lvl)
                lvl = l; // best minimum
        }
        return lvl;
    }

    private static bool IsMoveInherited(PKM pk, LegalInfo info, ushort move)
    {
        // In 3DS games, the inherited move must be in the relearn moves.
        if (info.Generation >= 6 && !pk.IsOriginalMovesetDeleted())
            return pk.HasRelearnMove(move);

        // In Pre-3DS games, the move is inherited if it has the move and it can be hatched with the move.
        if (pk.HasMove(move))
            return true;

        return DidLearnAndForget(info);
    }

    private static bool IsMoveInherited(PKM pk, LegalInfo info, ReadOnlySpan<ushort> moves)
    {
        // In 3DS games, the inherited move must be in the relearn moves.
        if (info.Generation >= 6)
        {
            Span<ushort> relearn = stackalloc ushort[4];
            pk.GetRelearnMoves(relearn);
            return relearn.IndexOfAny(moves) != -1;
        }

        // In Pre-3DS games, the move is inherited if it has the move and it can be hatched with the move.
        Span<ushort> pkMoves = stackalloc ushort[4];
        pk.GetMoves(pkMoves);
        var index = pkMoves.IndexOfAny(moves);
        if (index != -1)
            return true;

        return DidLearnAndForget(info);
    }

    private static bool DidLearnAndForget(LegalInfo info)
    {
        // If the pokemon does not currently have the move, it could have been an egg move that was forgotten.
        // This requires the pokemon to not have 4 other moves identified as egg moves or inherited level up moves.
        // If any move is not an egg source, then a slot could have been forgotten.
        foreach (var move in info.Moves)
        {
            if (!move.Info.Method.IsEggSource())
                return false;
        }
        return true;
    }
}
