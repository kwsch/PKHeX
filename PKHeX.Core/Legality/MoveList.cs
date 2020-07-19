using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.Legal;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for obtaining a list of moves.
    /// </summary>
    internal static class MoveList
    {
        internal static IEnumerable<int> GetValidRelearn(PKM pkm, int species, int form, bool inheritlvlmoves, GameVersion version = GameVersion.Any)
        {
            if (pkm.GenNumber < 6)
                return Array.Empty<int>();

            var r = new List<int>();
            r.AddRange(MoveEgg.GetRelearnLVLMoves(pkm, species, 1, form, version));

            if (pkm.Format == 6 && pkm.Species != (int)Species.Meowstic)
                form = 0;

            r.AddRange(MoveEgg.GetEggMoves(pkm, species, form, version));
            if (inheritlvlmoves)
                r.AddRange(MoveEgg.GetRelearnLVLMoves(pkm, species, 100, form, version));
            return r.Distinct();
        }

        internal static int[] GetShedinjaEvolveMoves(PKM pkm, int generation, int lvl)
        {
            if (pkm.Species != (int)Species.Shedinja || lvl < 20)
                return Array.Empty<int>();

            // If Nincada evolves into Ninjask and learns a move after evolution from Ninjask's LevelUp data, Shedinja would appear with that move.
            // Only one move above level 20 is allowed; check the count of Ninjask moves elsewhere.
            return generation switch
            {
                3 when pkm.InhabitedGeneration(3) => LevelUpE[(int)Species.Ninjask].GetMoves(lvl, 20), // Same LevelUp data in all Gen3 games
                4 when pkm.InhabitedGeneration(4) => LevelUpPt[(int)Species.Ninjask].GetMoves(lvl, 20), // Same LevelUp data in all Gen4 games
                _ => Array.Empty<int>(),
            };
        }

        internal static int GetShedinjaMoveLevel(int species, int move, int generation)
        {
            var src = generation == 4 ? LevelUpPt : LevelUpE;
            var moves = src[species];
            return moves.GetLevelLearnMove(move);
        }

        internal static int[] GetBaseEggMoves(PKM pkm, int species, int form, GameVersion gameSource, int lvl)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            switch (gameSource)
            {
                case GameVersion.GSC:
                case GameVersion.GS:
                    // If checking back-transfer specimen (GSC->RBY), remove moves that must be deleted prior to transfer
                    static int[] getRBYCompatibleMoves(int format, int[] moves) => format == 1 ? moves.Where(m => m <= MaxMoveID_1).ToArray() : moves;
                    if (pkm.InhabitedGeneration(2))
                        return getRBYCompatibleMoves(pkm.Format, LevelUpGS[species].GetMoves(lvl));
                    break;
                case GameVersion.C:
                    if (pkm.InhabitedGeneration(2))
                        return getRBYCompatibleMoves(pkm.Format, LevelUpC[species].GetMoves(lvl));
                    break;

                case GameVersion.R:
                case GameVersion.S:
                case GameVersion.RS:
                    if (pkm.InhabitedGeneration(3))
                        return LevelUpRS[species].GetMoves(lvl);
                    break;
                case GameVersion.E:
                    if (pkm.InhabitedGeneration(3))
                        return LevelUpE[species].GetMoves(lvl);
                    break;
                case GameVersion.FR:
                case GameVersion.LG:
                case GameVersion.FRLG:
                    // The only difference in FR/LG is Deoxys, which doesn't breed.
                    if (pkm.InhabitedGeneration(3))
                        return LevelUpFR[species].GetMoves(lvl);
                    break;

                case GameVersion.D:
                case GameVersion.P:
                case GameVersion.DP:
                    if (pkm.InhabitedGeneration(4))
                        return LevelUpDP[species].GetMoves(lvl);
                    break;
                case GameVersion.Pt:
                    if (pkm.InhabitedGeneration(4))
                        return LevelUpPt[species].GetMoves(lvl);
                    break;
                case GameVersion.HG:
                case GameVersion.SS:
                case GameVersion.HGSS:
                    if (pkm.InhabitedGeneration(4))
                        return LevelUpHGSS[species].GetMoves(lvl);
                    break;

                case GameVersion.B:
                case GameVersion.W:
                case GameVersion.BW:
                    if (pkm.InhabitedGeneration(5))
                        return LevelUpBW[species].GetMoves(lvl);
                    break;

                case GameVersion.B2:
                case GameVersion.W2:
                case GameVersion.B2W2:
                    if (pkm.InhabitedGeneration(5))
                        return LevelUpB2W2[species].GetMoves(lvl);
                    break;

                case GameVersion.X:
                case GameVersion.Y:
                case GameVersion.XY:
                    if (pkm.InhabitedGeneration(6))
                        return LevelUpXY[species].GetMoves(lvl);
                    break;

                case GameVersion.AS:
                case GameVersion.OR:
                case GameVersion.ORAS:
                    if (pkm.InhabitedGeneration(6))
                        return LevelUpAO[species].GetMoves(lvl);
                    break;

                case GameVersion.SN:
                case GameVersion.MN:
                case GameVersion.SM:
                    if (species > MaxSpeciesID_7)
                        break;
                    if (pkm.InhabitedGeneration(7))
                    {
                        int index = PersonalTable.SM.GetFormeIndex(species, form);
                        return LevelUpSM[index].GetMoves(lvl);
                    }
                    break;

                case GameVersion.US:
                case GameVersion.UM:
                case GameVersion.USUM:
                    if (pkm.InhabitedGeneration(7))
                    {
                        int index = PersonalTable.USUM.GetFormeIndex(species, form);
                        return LevelUpUSUM[index].GetMoves(lvl);
                    }
                    break;

                case GameVersion.SW:
                case GameVersion.SH:
                case GameVersion.SWSH:
                    if (pkm.InhabitedGeneration(8))
                    {
                        int index = PersonalTable.SWSH.GetFormeIndex(species, form);
                        return LevelUpSWSH[index].GetMoves(lvl);
                    }
                    break;
            }
            return Array.Empty<int>();
        }

        internal static IReadOnlyList<int>[] GetValidMovesAllGens(PKM pkm, IReadOnlyList<EvoCriteria>[] evoChains, int minLvLG1 = 1, int minLvLG2 = 1, bool LVL = true, bool Tutor = true, bool Machine = true, bool MoveReminder = true, bool RemoveTransferHM = true)
        {
            var result = new IReadOnlyList<int>[evoChains.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = Array.Empty<int>();

            var min = pkm is IBattleVersion b ? b.GetMinGeneration() : 1;
            for (int i = min; i < evoChains.Length; i++)
            {
                if (evoChains[i].Count == 0)
                    continue;

                result[i] = GetValidMoves(pkm, evoChains[i], i, minLvLG1, minLvLG2, LVL, Tutor, Machine, MoveReminder, RemoveTransferHM).ToList();
            }
            return result;
        }

        internal static IEnumerable<int> GetValidMoves(PKM pkm, IReadOnlyList<EvoCriteria> evoChain, int generation, int minLvLG1 = 1, int minLvLG2 = 1, bool LVL = true, bool Tutor = true, bool Machine = true, bool MoveReminder = true, bool RemoveTransferHM = true)
        {
            GameVersion version = (GameVersion)pkm.Version;
            if (!pkm.IsUntraded)
                version = GameVersion.Any;
            return GetValidMoves(pkm, version, evoChain, generation, minLvLG1: minLvLG1, minLvLG2: minLvLG2, LVL: LVL, Relearn: false, Tutor: Tutor, Machine: Machine, MoveReminder: MoveReminder, RemoveTransferHM: RemoveTransferHM);
        }

        internal static IEnumerable<int> GetValidRelearn(PKM pkm, int species, int form, GameVersion version = GameVersion.Any)
        {
            return GetValidRelearn(pkm, species, form, GetCanInheritMoves(species), version);
        }

        internal static List<int> GetValidPostEvolutionMoves(PKM pkm, int species, IReadOnlyList<EvoCriteria>[] evoChains, GameVersion Version)
        {
            // Return moves that the pokemon could learn after evolving
            var moves = new List<int>();
            for (int i = 1; i < evoChains.Length; i++)
            {
                if (evoChains[i].Count != 0)
                    moves.AddRange(GetValidPostEvolutionMoves(pkm, species, evoChains[i], i, Version));
            }

            if (pkm.GenNumber >= 6)
                moves.AddRange(pkm.RelearnMoves.Where(m => m != 0));
            return moves.Distinct().ToList();
        }

        private static List<int> GetValidPostEvolutionMoves(PKM pkm, int species, IReadOnlyList<EvoCriteria> evoChain, int generation, GameVersion Version)
        {
            var evomoves = new List<int>();
            var index = EvolutionChain.GetEvoChainSpeciesIndex(evoChain, species);
            for (int i = 0; i <= index; i++)
            {
                var evo = evoChain[i];
                var moves = GetMoves(pkm, evo.Species, 1, 1, evo.Level, evo.Form, moveTutor: true, Version: Version, LVL: true, specialTutors: true, Machine: true, MoveReminder: true, RemoveTransferHM: false, generation: generation);
                // Moves from Species or any species after in the evolution phase
                evomoves.AddRange(moves);
            }
            return evomoves;
        }

        internal static IEnumerable<int> GetExclusivePreEvolutionMoves(PKM pkm, int Species, IReadOnlyList<EvoCriteria> evoChain, int generation, GameVersion Version)
        {
            var preevomoves = new List<int>();
            var evomoves = new List<int>();
            var index = EvolutionChain.GetEvoChainSpeciesIndex(evoChain, Species);
            for (int i = 0; i < evoChain.Count; i++)
            {
                var evo = evoChain[i];
                var moves = GetMoves(pkm, evo.Species, 1, 1, evo.Level, evo.Form, moveTutor: true, Version: Version, LVL: true, specialTutors: true, Machine: true, MoveReminder: true, RemoveTransferHM: false, generation: generation);
                var list = i >= index ? preevomoves : evomoves;
                list.AddRange(moves);
            }
            return preevomoves.Except(evomoves).Distinct();
        }

        internal static IEnumerable<int> GetValidMoves(PKM pkm, GameVersion version, IReadOnlyList<EvoCriteria> chain, int generation, int minLvLG1 = 1, int minLvLG2 = 1, bool LVL = false, bool Relearn = false, bool Tutor = false, bool Machine = false, bool MoveReminder = true, bool RemoveTransferHM = true)
        {
            var r = new List<int> { 0 };
            int species = pkm.Species;

            if (FormChangeMoves.Contains(species)) // Deoxys & Shaymin & Giratina (others don't have extra but whatever)
            {
                // These don't evolve, so don't bother iterating for all entries in the evolution chain (should always be count==1).
                int formCount;

                // In gen 3 deoxys has different forms depending on the current game, in the PersonalInfo there is no alternate form info
                if (pkm.Format == 3 && species == (int)Species.Deoxys)
                    formCount = 4;
                else
                    formCount = pkm.PersonalInfo.FormeCount;

                for (int form = 0; form < formCount; form++)
                    r.AddRange(GetMoves(pkm, species, minLvLG1, minLvLG2, chain[0].Level, form, Tutor, version, LVL, Tutor, Machine, MoveReminder, RemoveTransferHM, generation));
                if (Relearn)
                    r.AddRange(pkm.RelearnMoves);
                return r.Distinct();
            }

            // Special Type Tutors Availability
            bool moveTutor = Tutor || MoveReminder; // Usually true, except when called for move suggestions (no tutored moves)

            for (var i = 0; i < chain.Count; i++)
            {
                var evo = chain[i];
                var moves = GetEvoMoves(pkm, version, chain, generation, minLvLG1, minLvLG2, LVL, Tutor, Machine, MoveReminder, RemoveTransferHM, moveTutor, i, evo);
                r.AddRange(moves);
            }

            if (pkm.Format <= 3)
                return r.Distinct();

            if (LVL)
                MoveTutor.AddSpecialFormChangeMoves(r, pkm, generation, species);
            if (Tutor)
                MoveTutor.AddSpecialTutorMoves(r, pkm, generation, species);
            if (Relearn && generation >= 6)
                r.AddRange(pkm.RelearnMoves);
            return r.Distinct();
        }

        private static IEnumerable<int> GetEvoMoves(PKM pkm, GameVersion Version, IReadOnlyList<EvoCriteria> chain, int generation, int minLvLG1, int minLvLG2, bool LVL, bool Tutor, bool Machine, bool MoveReminder, bool RemoveTransferHM, bool moveTutor, int i, EvoCriteria evo)
        {
            int minlvlevo1 = GetEvoMoveMinLevel1(pkm, generation, minLvLG1, evo);
            int minlvlevo2 = GetEvoMoveMinLevel2(pkm, generation, minLvLG2, evo);
            var maxLevel = evo.Level;
            if (i != 0 && chain[i - 1].RequiresLvlUp) // evolution
                ++maxLevel; // allow lvlmoves from the level it evolved to the next species
            return GetMoves(pkm, evo.Species, minlvlevo1, minlvlevo2, maxLevel, evo.Form, Tutor, Version, LVL, moveTutor, Machine, MoveReminder, RemoveTransferHM, generation);
        }

        /// <summary>
        /// Returns the minimum level the move can be learned at based on the species encounter level.
        /// </summary>
        private static int GetEvoMoveMinLevel1(PKM pkm, int generation, int minLvLG1, EvoCriteria evo)
        {
            if (generation != 1)
                return 1;
            // For evolutions, return the lower of the two; current level should legally be >=
            if (evo.MinLevel > 1)
                return Math.Min(pkm.CurrentLevel, evo.MinLevel);
            return minLvLG1;
        }

        private static int GetEvoMoveMinLevel2(PKM pkm, int generation, int minLvLG2, EvoCriteria evo)
        {
            if (generation != 2 || ParseSettings.AllowGen2MoveReminder(pkm))
                return 1;
            // For evolutions, return the lower of the two; current level should legally be >=
            if (evo.MinLevel > 1)
                return Math.Min(pkm.CurrentLevel, evo.MinLevel);
            return minLvLG2;
        }

        private static IEnumerable<int> GetMoves(PKM pkm, int species, int minlvlG1, int minlvlG2, int lvl, int form, bool moveTutor, GameVersion Version, bool LVL, bool specialTutors, bool Machine, bool MoveReminder, bool RemoveTransferHM, int generation)
        {
            var r = new List<int>();
            if (LVL)
                r.AddRange(MoveLevelUp.GetMovesLevelUp(pkm, species, minlvlG1, minlvlG2, lvl, form, Version, MoveReminder, generation));
            if (Machine)
                r.AddRange(MoveTechnicalMachine.GetTMHM(pkm, species, form, generation, Version, RemoveTransferHM));
            if (moveTutor)
                r.AddRange(MoveTutor.GetTutorMoves(pkm, species, form, specialTutors, generation));
            return r.Distinct();
        }
    }
}
