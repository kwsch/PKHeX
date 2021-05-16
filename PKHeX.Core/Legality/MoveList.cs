using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.Legal;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for obtaining a list of moves.
    /// </summary>
    internal static class MoveList
    {
        internal static IEnumerable<int> GetValidRelearn(PKM pkm, int species, int form, bool inheritlvlmoves, GameVersion version = Any)
        {
            int generation = pkm.Generation;
            if (generation < 6)
                return Array.Empty<int>();

            var r = new List<int>();
            r.AddRange(MoveEgg.GetRelearnLVLMoves(pkm, species, form, 1, version));

            if (pkm.Format == 6 && pkm.Species != (int)Species.Meowstic)
                form = 0;

            r.AddRange(MoveEgg.GetEggMoves(pkm.PersonalInfo, species, form, version, Math.Max(2, generation)));
            if (inheritlvlmoves)
                r.AddRange(MoveEgg.GetRelearnLVLMoves(pkm, species, form, 100, version));
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
            if (gameSource == Any)
                gameSource = (GameVersion)pkm.Version;

            switch (gameSource)
            {
                case GSC or GS:
                    // If checking back-transfer specimen (GSC->RBY), remove moves that must be deleted prior to transfer
                    static int[] getRBYCompatibleMoves(int format, int[] moves) => format == 1 ? moves.Where(m => m <= MaxMoveID_1).ToArray() : moves;
                    if (pkm.InhabitedGeneration(2))
                        return getRBYCompatibleMoves(pkm.Format, LevelUpGS[species].GetMoves(lvl));
                    break;
                case C:
                    if (pkm.InhabitedGeneration(2))
                        return getRBYCompatibleMoves(pkm.Format, LevelUpC[species].GetMoves(lvl));
                    break;

                case R or S or RS:
                    if (pkm.InhabitedGeneration(3))
                        return LevelUpRS[species].GetMoves(lvl);
                    break;
                case E:
                    if (pkm.InhabitedGeneration(3))
                        return LevelUpE[species].GetMoves(lvl);
                    break;
                case FR or LG or FRLG:
                    // The only difference in FR/LG is Deoxys, which doesn't breed.
                    if (pkm.InhabitedGeneration(3))
                        return LevelUpFR[species].GetMoves(lvl);
                    break;

                case D or P or DP:
                    if (pkm.InhabitedGeneration(4))
                        return LevelUpDP[species].GetMoves(lvl);
                    break;
                case Pt:
                    if (pkm.InhabitedGeneration(4))
                        return LevelUpPt[species].GetMoves(lvl);
                    break;
                case HG or SS or HGSS:
                    if (pkm.InhabitedGeneration(4))
                        return LevelUpHGSS[species].GetMoves(lvl);
                    break;

                case B or W or BW:
                    if (pkm.InhabitedGeneration(5))
                        return LevelUpBW[species].GetMoves(lvl);
                    break;

                case B2 or W2 or B2W2:
                    if (pkm.InhabitedGeneration(5))
                        return LevelUpB2W2[species].GetMoves(lvl);
                    break;

                case X or Y or XY:
                    if (pkm.InhabitedGeneration(6))
                        return LevelUpXY[species].GetMoves(lvl);
                    break;

                case AS or OR or ORAS:
                    if (pkm.InhabitedGeneration(6))
                        return LevelUpAO[species].GetMoves(lvl);
                    break;

                case SN or MN or SM:
                    if (species > MaxSpeciesID_7)
                        break;
                    if (pkm.InhabitedGeneration(7))
                    {
                        int index = PersonalTable.SM.GetFormIndex(species, form);
                        return LevelUpSM[index].GetMoves(lvl);
                    }
                    break;

                case US or UM or USUM:
                    if (pkm.InhabitedGeneration(7))
                    {
                        int index = PersonalTable.USUM.GetFormIndex(species, form);
                        return LevelUpUSUM[index].GetMoves(lvl);
                    }
                    break;

                case SW or SH or SWSH:
                    if (pkm.InhabitedGeneration(8))
                    {
                        int index = PersonalTable.SWSH.GetFormIndex(species, form);
                        return LevelUpSWSH[index].GetMoves(lvl);
                    }
                    break;
            }
            return Array.Empty<int>();
        }

        internal static IReadOnlyList<int>[] GetValidMovesAllGens(PKM pkm, IReadOnlyList<EvoCriteria>[] evoChains, int minLvLG1 = 1, int minLvLG2 = 1, MoveSourceType types = MoveSourceType.ExternalSources, bool RemoveTransferHM = true)
        {
            var result = new IReadOnlyList<int>[evoChains.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = Array.Empty<int>();

            var min = pkm is IBattleVersion b ? Math.Max(0, b.GetMinGeneration()) : 1;
            for (int i = min; i < evoChains.Length; i++)
            {
                if (evoChains[i].Count == 0)
                    continue;

                result[i] = GetValidMoves(pkm, evoChains[i], i, minLvLG1, minLvLG2, types, RemoveTransferHM).ToList();
            }
            return result;
        }

        internal static IEnumerable<int> GetValidMoves(PKM pkm, IReadOnlyList<EvoCriteria> evoChain, int generation, int minLvLG1 = 1, int minLvLG2 = 1, MoveSourceType types = MoveSourceType.ExternalSources, bool RemoveTransferHM = true)
        {
            GameVersion version = (GameVersion)pkm.Version;
            if (!pkm.IsUntraded)
                version = Any;
            return GetValidMoves(pkm, version, evoChain, generation, minLvLG1: minLvLG1, minLvLG2: minLvLG2, types: types, RemoveTransferHM: RemoveTransferHM);
        }

        internal static IEnumerable<int> GetValidRelearn(PKM pkm, int species, int form, GameVersion version = Any)
        {
            return GetValidRelearn(pkm, species, form, Breeding.GetCanInheritMoves(species), version);
        }

        internal static IEnumerable<int> GetExclusivePreEvolutionMoves(PKM pkm, int Species, IReadOnlyList<EvoCriteria> evoChain, int generation, GameVersion Version)
        {
            var preevomoves = new List<int>();
            var evomoves = new List<int>();
            var index = EvolutionChain.GetEvoChainSpeciesIndex(evoChain, Species);
            for (int i = 0; i < evoChain.Count; i++)
            {
                var evo = evoChain[i];
                var moves = GetMoves(pkm, evo.Species, 1, 1, evo.Level, evo.Form, Version: Version, types: MoveSourceType.ExternalSources, RemoveTransferHM: false, generation: generation);
                var list = i >= index ? preevomoves : evomoves;
                list.AddRange(moves);
            }
            return preevomoves.Except(evomoves).Distinct();
        }

        internal static IEnumerable<int> GetValidMoves(PKM pkm, GameVersion version, IReadOnlyList<EvoCriteria> chain, int generation, int minLvLG1 = 1, int minLvLG2 = 1, MoveSourceType types = MoveSourceType.Reminder, bool RemoveTransferHM = true)
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
                    formCount = pkm.PersonalInfo.FormCount;

                for (int form = 0; form < formCount; form++)
                    r.AddRange(GetMoves(pkm, species, minLvLG1, minLvLG2, chain[0].Level, form, version, types, RemoveTransferHM, generation));
                if (types.HasFlagFast(MoveSourceType.RelearnMoves))
                    r.AddRange(pkm.RelearnMoves);
                return r.Distinct();
            }

            for (var i = 0; i < chain.Count; i++)
            {
                var evo = chain[i];
                var moves = GetEvoMoves(pkm, version, types, chain, generation, minLvLG1, minLvLG2, RemoveTransferHM, i, evo);
                r.AddRange(moves);
            }

            if (pkm.Format <= 3)
                return r.Distinct();

            if (types.HasFlagFast(MoveSourceType.LevelUp))
                MoveTutor.AddSpecialFormChangeMoves(r, pkm, generation, species);
            if (types.HasFlagFast(MoveSourceType.SpecialTutor))
                MoveTutor.AddSpecialTutorMoves(r, pkm, generation, species);
            if (types.HasFlagFast(MoveSourceType.RelearnMoves) && generation >= 6)
                r.AddRange(pkm.RelearnMoves);
            return r.Distinct();
        }

        private static IEnumerable<int> GetEvoMoves(PKM pkm, GameVersion Version, MoveSourceType types, IReadOnlyList<EvoCriteria> chain, int generation, int minLvLG1, int minLvLG2, bool RemoveTransferHM, int i, EvoCriteria evo)
        {
            int minlvlevo1 = GetEvoMoveMinLevel1(pkm, generation, minLvLG1, evo);
            int minlvlevo2 = GetEvoMoveMinLevel2(pkm, generation, minLvLG2, evo);
            var maxLevel = evo.Level;
            if (i != 0 && chain[i - 1].RequiresLvlUp) // evolution
                ++maxLevel; // allow lvlmoves from the level it evolved to the next species
            return GetMoves(pkm, evo.Species, minlvlevo1, minlvlevo2, maxLevel, evo.Form, Version, types, RemoveTransferHM, generation);
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

        private static IEnumerable<int> GetMoves(PKM pkm, int species, int minlvlG1, int minlvlG2, int lvl, int form, GameVersion Version, MoveSourceType types, bool RemoveTransferHM, int generation)
        {
            var r = new List<int>();
            if (types.HasFlagFast(MoveSourceType.LevelUp))
                r.AddRange(MoveLevelUp.GetMovesLevelUp(pkm, species, minlvlG1, minlvlG2, lvl, form, Version, types.HasFlagFast(MoveSourceType.Reminder), generation));
            if (types.HasFlagFast(MoveSourceType.Machine))
                r.AddRange(MoveTechnicalMachine.GetTMHM(pkm, species, form, generation, Version, RemoveTransferHM));
            if (types.HasFlagFast(MoveSourceType.TechnicalRecord))
                r.AddRange(MoveTechnicalMachine.GetRecords(pkm, species, form, generation));
            if (types.HasFlagFast(MoveSourceType.AllTutors))
                r.AddRange(MoveTutor.GetTutorMoves(pkm, species, form, types.HasFlagFast(MoveSourceType.SpecialTutor), generation));
            return r.Distinct();
        }
    }

    [Flags]
#pragma warning disable RCS1154 // Sort enum members.
    public enum MoveSourceType
#pragma warning restore RCS1154 // Sort enum members.
    {
        None,
        LevelUp         = 1 << 0,
        RelearnMoves    = 1 << 1,
        Machine         = 1 << 2,
        TypeTutor       = 1 << 3,
        SpecialTutor    = 1 << 4,
        EnhancedTutor   = 1 << 5,
        SharedEggMove   = 1 << 6,
        TechnicalRecord = 1 << 7,

        AllTutors = TypeTutor | SpecialTutor | EnhancedTutor,
        AllMachines = Machine | TechnicalRecord,

        Reminder = LevelUp | RelearnMoves | TechnicalRecord,
        Encounter = LevelUp | RelearnMoves,
        ExternalSources = Reminder | AllMachines | AllTutors,
        All = ExternalSources | SharedEggMove | RelearnMoves,
    }

    public static class MoveSourceTypeExtensions
    {
        public static bool HasFlagFast(this MoveSourceType value, MoveSourceType flag) => (value & flag) != 0;
        public static MoveSourceType ClearNonEggSources(this MoveSourceType value) => value & MoveSourceType.Encounter;
    }
}
