using System;
using System.Linq;

namespace PKHeX.Core
{
    public static class EvolutionRestrictions
    {
        public static bool IsMoveRequiredToEvolve(PKM pkm, int gen) => IsMoveRequiredToEvolve(pkm.Species, pkm.Format, gen);

        public static bool IsMoveRequiredToEvolve(int species, int format, int gen)
        {
            if (!SpeciesEvolutionWithMove.Contains(species))
                return false;
            if (format <= 3)
                return false;
            if (BabyEvolutionWithMove.Contains(species))
                return gen > 3;
            return true;
        }

        internal static readonly int[] BabyEvolutionWithMove =
        {
            122, // Mr. Mime (Mime Jr with Mimic)
            185, // Sudowoodo (Bonsly with Mimic)
        };

        // List of species that evolve from a previous species having a move while leveling up
        internal static readonly int[] SpeciesEvolutionWithMove =
        {
            122, // Mr. Mime (Mime Jr with Mimic)
            185, // Sudowoodo (Bonsly with Mimic)
            424, // Ambipom (Aipom with Double Hit)
            463, // Lickilicky (Lickitung with Rollout)
            465, // Tangrowth (Tangela with Ancient Power)
            469, // Yanmega (Yamma with Ancient Power)
            473, // Mamoswine (Piloswine with Ancient Power)
            700, // Sylveon (Eevee with Fairy Move)
            763, // Tsareena (Steenee with Stomp)
            (int)Species.Grapploct // (Clobbopus with Taunt)
        };

        internal static readonly int[] FairyMoves =
        {
            186, // Sweet Kiss
            204, // Charm
            236, // Moonlight
            574, // Disarming Voice
            577, // Draining Kiss
            578, // Crafty Shield
            579, // Flower Shield
            581, // Misty Terrain
            583, // Play Rough
            584, // Fairy Wind
            585, // Moonblast
            587, // Fairy Lock
            597, // Aromatic Mist
            601, // Geomancy
            605, // Dazzling Gleam
            608, // Baby-Doll Eyes
            617, // Light of Ruin
            656, // Twinkle Tackle
            657, // Twinkle Tackle
            666, // Floral Healing
            698, // Guardian of Alola
            705, // Fleur Cannon
            717, // Nature's Madness
            726, // Let's Snuggle Forever
            740, // Sparkly Swirl
            767, // Max Starfall
            777, // Decorate
            789, // Spirit Break
            790, // Strange Steam
            802, // Misty Explosion
        };

        // Moves that trigger the evolution by move
        private static readonly int[][] MoveEvolutionWithMove =
        {
            new [] { 102 }, // Mr. Mime (Mime Jr with Mimic)
            new [] { 102 }, // Sudowoodo (Bonsly with Mimic)
            new [] { 458 }, // Ambipom (Aipom with Double Hit)
            new [] { 205 }, // Lickilicky (Lickitung with Rollout)
            new [] { 246 }, // Tangrowth (Tangela with Ancient Power)
            new [] { 246 }, // Yanmega (Yamma with Ancient Power)
            new [] { 246 }, // Mamoswine (Piloswine with Ancient Power)
            FairyMoves, // Sylveon (Eevee with Fairy Move)
            new [] { 023 }, // Tsareena (Steenee with Stomp)
            new [] { 269 }, // Grapploct (Clobbopus with Taunt)
        };

        // Min level for any species for every generation to learn the move for evolution by move
        // 0 means it cant be learned in that generation
        private static readonly int[][] MinLevelEvolutionWithMove =
        {
            // Mr. Mime (Mime Jr with Mimic)
            new [] { 0, 0, 0, 0, 18, 15, 15, 2, 2 },
            // Sudowoodo (Bonsly with Mimic)
            new [] { 0, 0, 0, 0, 17, 17, 15, 2, 2 },
            // Ambipom (Aipom with Double Hit)
            new [] { 0, 0, 0, 0, 32, 32, 32, 2, 2 },
            // Lickilicky (Lickitung with Rollout)
            new [] { 0, 0, 2, 0, 2, 33, 33, 2, 2 },
            // Tangrowth (Tangela with Ancient Power)
            new [] { 0, 0, 0, 0, 2, 36, 38, 2, 2 },
            // Yanmega (Yanma with Ancient Power)
            new [] { 0, 0, 0, 0, 2, 33, 33, 2, 2 },
            // Mamoswine (Piloswine with Ancient Power)
            new [] { 0, 0, 0, 0, 2, 2, 2, 2, 2 },
            // Sylveon (Eevee with Fairy Move)
            new [] { 0, 0, 0, 0, 0, 29, 9, 2, 2 },
            // Tsareena (Steenee with Stomp)
            new [] { 0, 0, 0, 0, 0, 0, 0, 2, 28 },
            // Grapploct (Clobbopus with Taunt)
            new [] { 0, 0, 0, 0, 0, 0, 0, 0, 35 },
        };

        // True -> the pokemon could hatch from an egg with the move for evolution as an egg move
        private static readonly bool[][] EggMoveEvolutionWithMove =
        {
            // Mr. Mime (Mime Jr with Mimic)
            new [] { false, false, false, false, true, true, true, true, true },
            // Sudowoodo (Bonsly with Mimic)
            new [] { false, false, false, false, true, true, true, true, true },
            // Ambipom (Aipom with Double Hit)
            new [] { false, false, false, false, true, true, true, true, true },
            // Lickilicky (Lickitung with Rollout)
            new [] { false, false, true, false, true, true, true, true, true },
            // Tangrowth (Tangela with Ancient Power)
            new [] { false, false, false, false, true, true, true, true, true },
            // Yanmega (Yanma with Ancient Power)
            new [] { false, false, false, false, true, true, true, true, true },
            // Mamoswine (Piloswine with Ancient Power)
            new [] { false, false, true, true, true, true, true, true, true },
            // Sylveon (Eevee with Fairy Move)
            new [] { false, false, true, true, true, true, true, true, true },
            // Tsareena (Steenee with Stomp)
            new [] { false, false, false, false, false, false, false, false, false },
            // Grapploct (Clobbopus with Taunt)
            new [] { false, false, false, false, false, false, false, false, true },
        };

        public static bool IsEvolutionValidWithMove(PKM pkm, LegalInfo info)
        {
            // Exclude species that do not evolve leveling with a move
            // Exclude gen 1-3 formats
            // Exclude Mr. Mime and Snorlax for gen 1-3 games
            var gen = info.Generation;
            if (!IsMoveRequiredToEvolve(pkm, gen))
                return true;

            var lvl = GetLevelLearnMove(pkm, gen, info.EncounterMatch, info.Moves);

            // If has original met location the minimum evolution level is one level after met level
            // Gen 3 pokemon in gen 4 games: minimum level is one level after transfer to generation 4
            // VC pokemon: minimum level is one level after transfer to generation 7
            // Sylveon: always one level after met level, for gen 4 and 5 eevees in gen 6 games minimum for evolution is one level after transfer to generation 5
            if (pkm.HasOriginalMetLocation || (pkm.Format == 4 && gen == 3) || pkm.VC || pkm.Species == (int)Species.Sylveon)
                lvl = Math.Max(pkm.Met_Level + 1, lvl);

            // Current level must be at least one the minimum learn level
            // the level-up event that triggers the learning of the move also triggers evolution with no further level-up required
            return pkm.CurrentLevel >= lvl;
        }

        public static int GetLevelLearnMove(PKM pkm, int gen, IEncounterable enc, CheckMoveResult[] res)
        {
            var index = Array.FindIndex(SpeciesEvolutionWithMove, p => p == pkm.Species);

            // Get the minimum level in any generation when the pokemon could learn the evolve move
            var levels = MinLevelEvolutionWithMove[index];
            var lvl = 101;
            for (int g = gen; g <= pkm.Format; g++)
            {
                if (pkm.InhabitedGeneration(g) && levels[g] > 0)
                    lvl = Math.Min(lvl, levels[g]);
            }

            // Check also if the current encounter include the evolve move as an special move
            // That means the pokemon have the move from the encounter level
            var moves = MoveEvolutionWithMove[index];
            if (enc is IMoveset s && s.Moves.Any(m => moves.Contains(m)))
                lvl = Math.Min(lvl, enc.LevelMin);

            // If the encounter is a player hatched egg, check if the move could be an egg move or inherited level up move
            var allowegg = EggMoveEvolutionWithMove[index][gen];
            if (enc is EncounterEgg && allowegg)
            {
                if (IsMoveInherited(pkm, gen, res, moves))
                    lvl = Math.Min(lvl, gen <= 3 ? 6 : 2);
            }

            return lvl;
        }

        private static bool IsMoveInherited(PKM pkm, int gen, CheckMoveResult[] res, int[] moves)
        {
            // In 3DS games, the inherited move must be in the relearn moves.
            if (gen >= 6)
                return pkm.RelearnMoves.Any(moves.Contains);

            // In Pre-3DS games, the move is inherited if it has the move and it can be hatched with the move.
            if (pkm.Moves.Any(moves.Contains))
                return true;

            // If the pokemon does not have the move, it still could be an egg move that was forgotten.
            // This requires the pokemon to not have 4 other moves identified as egg moves or inherited level up moves.
            return 4 > res.Count(m => m.Source == MoveSource.EggMove || m.Source == MoveSource.InheritLevelUp);
        }
    }
}
