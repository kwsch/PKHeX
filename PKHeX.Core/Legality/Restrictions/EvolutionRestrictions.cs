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
            (int)Species.MrMime, // (Mime Jr with Mimic)
            (int)Species.Sudowoodo, // (Bonsly with Mimic)
        };

        // List of species that evolve from a previous species having a move while leveling up
        internal static readonly int[] SpeciesEvolutionWithMove =
        {
            (int)Species.MrMime, // Mr. Mime (Mime Jr with Mimic)
            (int)Species.Sudowoodo, // Sudowoodo (Bonsly with Mimic)
            (int)Species.Ambipom, // Ambipom (Aipom with Double Hit)
            (int)Species.Lickilicky, // Lickilicky (Lickitung with Rollout)
            (int)Species.Tangrowth, // Tangrowth (Tangela with Ancient Power)
            (int)Species.Yanmega, // Yanmega (Yamma with Ancient Power)
            (int)Species.Mamoswine, // Mamoswine (Piloswine with Ancient Power)
            (int)Species.Sylveon, // Sylveon (Eevee with Fairy Move)
            (int)Species.Tsareena, // Tsareena (Steenee with Stomp)
            (int)Species.Grapploct // (Clobbopus with Taunt)
        };

        internal static readonly int[] FairyMoves =
        {
            (int)Move.SweetKiss,
            (int)Move.Charm,
            (int)Move.Moonlight,
            (int)Move.DisarmingVoice,
            (int)Move.DrainingKiss,
            (int)Move.CraftyShield,
            (int)Move.FlowerShield,
            (int)Move.MistyTerrain,
            (int)Move.PlayRough,
            (int)Move.FairyWind,
            (int)Move.Moonblast,
            (int)Move.FairyLock,
            (int)Move.AromaticMist,
            (int)Move.Geomancy,
            (int)Move.DazzlingGleam,
            (int)Move.BabyDollEyes,
            (int)Move.LightofRuin,
            (int)Move.TwinkleTackleP,
            (int)Move.TwinkleTackleS,
            (int)Move.FloralHealing,
            (int)Move.GuardianofAlola,
            (int)Move.FleurCannon,
            (int)Move.NaturesMadness,
            (int)Move.LetsSnuggleForever,
            (int)Move.SparklySwirl,
            (int)Move.MaxStarfall,
            (int)Move.Decorate,
            (int)Move.SpiritBreak,
            (int)Move.StrangeSteam,
            (int)Move.MistyExplosion,
        };

        // Moves that trigger the evolution by move
        private static readonly int[][] MoveEvolutionWithMove =
        {
            new [] { (int)Move.Mimic }, // Mr. Mime (Mime Jr with Mimic)
            new [] { (int)Move.Mimic }, // Sudowoodo (Bonsly with Mimic)
            new [] { (int)Move.DoubleHit }, // Ambipom (Aipom with Double Hit)
            new [] { (int)Move.Rollout }, // Lickilicky (Lickitung with Rollout)
            new [] { (int)Move.AncientPower }, // Tangrowth (Tangela with Ancient Power)
            new [] { (int)Move.AncientPower }, // Yanmega (Yamma with Ancient Power)
            new [] { (int)Move.AncientPower }, // Mamoswine (Piloswine with Ancient Power)
            FairyMoves, // Sylveon (Eevee with Fairy Move)
            new [] { (int)Move.Stomp }, // Tsareena (Steenee with Stomp)
            new [] { (int)Move.Taunt }, // Grapploct (Clobbopus with Taunt)
        };

        // Min level for any species for every generation to learn the move for evolution by move
        // 0 means it cant be learned in that generation
        private static readonly int[][] MinLevelEvolutionWithMove =
        {
            new [] { 00, 00, 00, 00, 18, 15, 15, 02, 02 }, // Mr. Mime (Mime Jr with Mimic)
            new [] { 00, 00, 00, 00, 17, 17, 15, 02, 02 }, // Sudowoodo (Bonsly with Mimic)
            new [] { 00, 00, 00, 00, 32, 32, 32, 02, 02 }, // Ambipom (Aipom with Double Hit)
            new [] { 00, 00, 02, 00, 02, 33, 33, 02, 02 }, // Lickilicky (Lickitung with Rollout)
            new [] { 00, 00, 00, 00, 02, 36, 38, 02, 02 }, // Tangrowth (Tangela with Ancient Power)
            new [] { 00, 00, 00, 00, 02, 33, 33, 02, 02 }, // Yanmega (Yanma with Ancient Power)
            new [] { 00, 00, 00, 00, 02, 02, 02, 02, 02 }, // Mamoswine (Piloswine with Ancient Power)
            new [] { 00, 00, 00, 00, 00, 29, 09, 02, 02 }, // Sylveon (Eevee with Fairy Move)
            new [] { 00, 00, 00, 00, 00, 00, 00, 02, 28 }, // Tsareena (Steenee with Stomp)
            new [] { 00, 00, 00, 00, 00, 00, 00, 00, 35 }, // Grapploct (Clobbopus with Taunt)
        };

        // True -> the pokemon could hatch from an egg with the move for evolution as an egg move
        private static readonly bool[][] EggMoveEvolutionWithMove =
        {
            new [] { false, false, false, false,  true,  true,  true,  true,  true }, // Mr. Mime (Mime Jr with Mimic)
            new [] { false, false, false, false,  true,  true,  true,  true,  true }, // Sudowoodo (Bonsly with Mimic)
            new [] { false, false, false, false,  true,  true,  true,  true,  true }, // Ambipom (Aipom with Double Hit)
            new [] { false, false,  true, false,  true,  true,  true,  true,  true }, // Lickilicky (Lickitung with Rollout)
            new [] { false, false, false, false,  true,  true,  true,  true,  true }, // Tangrowth (Tangela with Ancient Power)
            new [] { false, false, false, false,  true,  true,  true,  true,  true }, // Yanmega (Yanma with Ancient Power)
            new [] { false, false,  true,  true,  true,  true,  true,  true,  true }, // Mamoswine (Piloswine with Ancient Power)
            new [] { false, false,  true,  true,  true,  true,  true,  true,  true }, // Sylveon (Eevee with Fairy Move)
            new [] { false, false, false, false, false, false, false, false, false }, // Tsareena (Steenee with Stomp)
            new [] { false, false, false, false, false, false, false, false,  true }, // Grapploct (Clobbopus with Taunt)
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
            var index = Array.IndexOf(SpeciesEvolutionWithMove, pkm.Species);

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
