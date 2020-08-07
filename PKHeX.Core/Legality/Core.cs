using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        // Gen 1
        internal static readonly Learnset[] LevelUpRB = LearnsetReader.GetArray(Util.GetBinaryResource("lvlmove_rb.pkl"), MaxSpeciesID_1);
        internal static readonly Learnset[] LevelUpY = LearnsetReader.GetArray(Util.GetBinaryResource("lvlmove_y.pkl"), MaxSpeciesID_1);

        // Gen 2
        internal static readonly EggMoves[] EggMovesGS = EggMoves2.GetArray(Util.GetBinaryResource("eggmove_gs.pkl"), MaxSpeciesID_2);
        internal static readonly Learnset[] LevelUpGS = LearnsetReader.GetArray(Util.GetBinaryResource("lvlmove_gs.pkl"), MaxSpeciesID_2);
        internal static readonly EggMoves[] EggMovesC = EggMoves2.GetArray(Util.GetBinaryResource("eggmove_c.pkl"), MaxSpeciesID_2);
        internal static readonly Learnset[] LevelUpC = LearnsetReader.GetArray(Util.GetBinaryResource("lvlmove_c.pkl"), MaxSpeciesID_2);

        // Gen 3
        internal static readonly Learnset[] LevelUpE = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_e.pkl"), "em"));
        internal static readonly Learnset[] LevelUpRS = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_rs.pkl"), "rs"));
        internal static readonly Learnset[] LevelUpFR = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_fr.pkl"), "fr"));
        internal static readonly Learnset[] LevelUpLG = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_lg.pkl"), "lg"));
        internal static readonly EggMoves6[] EggMovesRS = EggMoves6.GetArray(BinLinker.Unpack(Util.GetBinaryResource("eggmove_rs.pkl"), "rs"));

        // Gen 4
        internal static readonly Learnset[] LevelUpDP = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_dp.pkl"), "dp"));
        internal static readonly Learnset[] LevelUpPt = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_pt.pkl"), "pt"));
        internal static readonly Learnset[] LevelUpHGSS = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_hgss.pkl"), "hs"));
        internal static readonly EggMoves6[] EggMovesDPPt = EggMoves6.GetArray(BinLinker.Unpack(Util.GetBinaryResource("eggmove_dppt.pkl"), "dp"));
        internal static readonly EggMoves6[] EggMovesHGSS = EggMoves6.GetArray(BinLinker.Unpack(Util.GetBinaryResource("eggmove_hgss.pkl"), "hs"));

        // Gen 5
        internal static readonly Learnset[] LevelUpBW = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_bw.pkl"), "51"));
        internal static readonly Learnset[] LevelUpB2W2 = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_b2w2.pkl"), "52"));
        internal static readonly EggMoves6[] EggMovesBW = EggMoves6.GetArray(BinLinker.Unpack(Util.GetBinaryResource("eggmove_bw.pkl"), "bw"));

        // Gen 6
        internal static readonly EggMoves6[] EggMovesXY = EggMoves6.GetArray(BinLinker.Unpack(Util.GetBinaryResource("eggmove_xy.pkl"), "xy"));
        internal static readonly Learnset[] LevelUpXY = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_xy.pkl"), "xy"));
        internal static readonly EggMoves6[] EggMovesAO = EggMoves6.GetArray(BinLinker.Unpack(Util.GetBinaryResource("eggmove_ao.pkl"), "ao"));
        internal static readonly Learnset[] LevelUpAO = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_ao.pkl"), "ao"));

        // Gen 7
        internal static readonly EggMoves7[] EggMovesSM = EggMoves7.GetArray(BinLinker.Unpack(Util.GetBinaryResource("eggmove_sm.pkl"), "sm"));
        internal static readonly Learnset[] LevelUpSM = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_sm.pkl"), "sm"));
        internal static readonly EggMoves7[] EggMovesUSUM = EggMoves7.GetArray(BinLinker.Unpack(Util.GetBinaryResource("eggmove_uu.pkl"), "uu"));
        internal static readonly Learnset[] LevelUpUSUM = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_uu.pkl"), "uu"));
        internal static readonly Learnset[] LevelUpGG = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_gg.pkl"), "gg"));

        // Gen 8
        internal static readonly EggMoves7[] EggMovesSWSH = EggMoves7.GetArray(BinLinker.Unpack(Util.GetBinaryResource("eggmove_swsh.pkl"), "ss"));
        internal static readonly Learnset[] LevelUpSWSH = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_swsh.pkl"), "ss"));

        public static IReadOnlyList<byte> GetPPTable(PKM pkm, int format)
        {
            if (format != 7)
                return GetPPTable(format);
            return pkm.GG ? MovePP_GG : MovePP_SM;
        }

        public static IReadOnlyList<byte> GetPPTable(int format)
        {
            return format switch
            {
                1 => MovePP_RBY,
                2 => MovePP_GSC,
                3 => MovePP_RS,
                4 => MovePP_DP,
                5 => MovePP_BW,
                6 => MovePP_XY,
                7 => MovePP_SM,
                8 => MovePP_SWSH,
                _ => Array.Empty<byte>()
            };
        }

        internal static ICollection<int> GetWildBalls(int gen, GameVersion game)
        {
            return gen switch
            {
                1 => WildPokeBalls1,
                2 => WildPokeBalls2,
                3 => WildPokeBalls3,
                4 => GameVersion.HGSS.Contains(game) ? WildPokeBalls4_HGSS : WildPokeBalls4_DPPt,
                5 => WildPokeBalls5,
                6 => WildPokeballs6,
                7 => GameVersion.Gen7b.Contains(game) ? WildPokeballs7b : WildPokeballs7,
                8 => WildPokeballs8,
                _ => Array.Empty<int>()
            };
        }

        internal static ICollection<int> GetSplitBreedGeneration(int generation)
        {
            switch (generation)
            {
                case 3: return SplitBreed_3;

                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    return SplitBreed;

                default: return Array.Empty<int>();
            }
        }

        internal static int GetMaxSpeciesOrigin(PKM pkm)
        {
            if (pkm.Format == 1)
                return GetMaxSpeciesOrigin(1);
            if (pkm.Format == 2 || pkm.VC)
                return GetMaxSpeciesOrigin(2);
            return GetMaxSpeciesOrigin(pkm.GenNumber);
        }

        internal static int GetMaxSpeciesOrigin(int generation)
        {
            return generation switch
            {
                1 => MaxSpeciesID_1,
                2 => MaxSpeciesID_2,
                3 => MaxSpeciesID_3,
                4 => MaxSpeciesID_4,
                5 => MaxSpeciesID_5,
                6 => MaxSpeciesID_6,
                7 => MaxSpeciesID_7b,
                8 => MaxSpeciesID_8,
                _ => -1
            };
        }

        internal static ICollection<int> GetFutureGenEvolutions(int generation)
        {
            return generation switch
            {
                1 => FutureEvolutionsGen1,
                2 => FutureEvolutionsGen2,
                3 => FutureEvolutionsGen3,
                4 => FutureEvolutionsGen4,
                5 => FutureEvolutionsGen5,
                _ => Array.Empty<int>()
            };
        }

        internal static int GetDebutGeneration(int species)
        {
            if (species <= MaxSpeciesID_1)
                return 1;
            if (species <= MaxSpeciesID_2)
                return 2;
            if (species <= MaxSpeciesID_3)
                return 3;
            if (species <= MaxSpeciesID_4)
                return 4;
            if (species <= MaxSpeciesID_5)
                return 5;
            if (species <= MaxSpeciesID_6)
                return 6;
            if (species <= MaxSpeciesID_7b)
                return 7;
            if (species <= MaxSpeciesID_8)
                return 8;
            return -1;
        }

        internal static int GetMaxLanguageID(int generation)
        {
            return generation switch
            {
                1 => (int) LanguageID.Spanish, // 1-7 except 6
                3 => (int) LanguageID.Spanish, // 1-7 except 6
                2 => (int) LanguageID.Korean,
                4 => (int) LanguageID.Korean,
                5 => (int) LanguageID.Korean,
                6 => (int) LanguageID.Korean,
                7 => (int) LanguageID.ChineseT,
                8 => (int) LanguageID.ChineseT,
                _ => -1
            };
        }

        private static bool IsEvolvedFormChange(PKM pkm, int expected)
        {
            if (pkm.IsEgg)
                return false;

            if (pkm.Format >= 7 && (AlolanVariantEvolutions12.Contains(pkm.Species) || GalarVariantFormEvolutions.Contains(pkm.Species)))
                return pkm.AltForm == 1;
            if (pkm.Format >= 8)
            {
                if (GalarVariantFormEvolutions.Contains(pkm.Species))
                    return pkm.AltForm == 1;
                if (GalarForm0Evolutions.TryGetValue(pkm.Species, out var orig))
                    return pkm.AltForm != orig; // bad compare?
                if ((int) Species.Darmanitan == pkm.Species)
                    return pkm.AltForm == (expected == 1 ? 2 : 0);
                if ((int) Species.MrMime == pkm.Species)
                    return pkm.AltForm == (expected == 0 ? 1 : 0);
                if ((int) Species.Slowbro == pkm.Species)
                    return pkm.AltForm == (expected == 1 ? 2 : 0);
                if ((int) Species.Toxtricity == pkm.Species)
                    return pkm.AltForm == EvolutionMethod.GetAmpLowKeyResult(pkm.Nature);
                if ((int) Species.Alcremie == pkm.Species)
                    return true;
            }
            if ((pkm.Species == (int)Species.Meowstic || pkm.Species == (int)Species.Indeedee) && pkm.Gender == 1)
                return pkm.AltForm == 1;
            if (pkm.Species == (int)Species.Lycanroc)
                return pkm.AltForm < 3;
            return pkm.Species == (int)Species.Silvally;
        }

        internal static bool IsEvolutionValid(PKM pkm, int minSpecies = -1, int minLevel = -1)
        {
            var curr = EvolutionChain.GetValidPreEvolutions(pkm, minLevel: minLevel);
            var min = curr.FindLast(z => z.Species == minSpecies);
            if (min != null && min.Level < minLevel)
                return false;
            var poss = EvolutionChain.GetValidPreEvolutions(pkm, maxLevel: 100, minLevel: minLevel, skipChecks: true);

            if (minSpecies != -1)
            {
                int last = poss.FindLastIndex(z => z.Species == minSpecies);
                return curr.Count >= last;
            }
            int gen = pkm.GenNumber;
            if (gen >= 3 && GetSplitBreedGeneration(gen).Contains(EvoBase.GetBaseSpecies(poss, 1).Species))
                return curr.Count >= poss.Count - 1;
            return curr.Count >= poss.Count;
        }

        internal static bool IsEvolutionValidWithMove(PKM pkm, LegalInfo info)
        {
            // Exclude species that do not evolve leveling with a move
            // Exclude gen 1-3 formats
            // Exclude Mr. Mime and Snorlax for gen 1-3 games
            var gen = info.Generation;
            if (!SpeciesEvolutionWithMove.Contains(pkm.Species) || pkm.Format <= 3 || (BabyEvolutionWithMove.Contains(pkm.Species) && gen <= 3))
                return true;

            var index = Array.FindIndex(SpeciesEvolutionWithMove, p => p == pkm.Species);
            var levels = MinLevelEvolutionWithMove[index];
            var moves = MoveEvolutionWithMove[index];
            var allowegg = EggMoveEvolutionWithMove[index][gen];

            // Get the minimum level in any generation when the pokemon could learn the evolve move
            var LearnLevel = 101;
            for (int g = gen; g <= pkm.Format; g++)
            {
                if (pkm.InhabitedGeneration(g) && levels[g] > 0)
                    LearnLevel = Math.Min(LearnLevel, levels[g]);
            }

            // Check also if the current encounter include the evolve move as an special move
            // That means the pokemon have the move from the encounter level
            if (info.EncounterMatch is IMoveset s && s.Moves.Any(m => moves.Contains(m)))
                LearnLevel = Math.Min(LearnLevel, info.EncounterMatch.LevelMin);

            // If the encounter is a player hatched egg check if the move could be an egg move or inherited level up move
            if (info.EncounterMatch is EncounterEgg && allowegg)
            {
                if (IsMoveInherited(pkm, info, moves))
                    LearnLevel = Math.Min(LearnLevel, gen <= 3 ? 6 : 2);
            }

            // If has original met location the minimum evolution level is one level after met level
            // Gen 3 pokemon in gen 4 games: minimum level is one level after transfer to generation 4
            // VC pokemon: minimum level is one level after transfer to generation 7
            // Sylveon: always one level after met level, for gen 4 and 5 eevees in gen 6 games minimum for evolution is one level after transfer to generation 5
            if (pkm.HasOriginalMetLocation || (pkm.Format == 4 && gen == 3) || pkm.VC || pkm.Species == (int)Species.Sylveon)
                LearnLevel = Math.Max(pkm.Met_Level + 1, LearnLevel);

            // Current level must be at least one the minimum learn level
            // the level-up event that triggers the learning of the move also triggers evolution with no further level-up required
            return pkm.CurrentLevel >= LearnLevel;
        }

        private static bool IsMoveInherited(PKM pkm, LegalInfo info, int[] moves)
        {
            // In 3DS games, the inherited move must be in the relearn moves.
            if (info.Generation >= 6)
                return pkm.RelearnMoves.Any(moves.Contains);

            // In Pre-3DS games, the move is inherited if it has the move and it can be hatched with the move.
            if (pkm.Moves.Any(moves.Contains))
                return true;

            // If the pokemon does not have the move, it still could be an egg move that was forgotten.
            // This requires the pokemon to not have 4 other moves identified as egg moves or inherited level up moves.
            return 4 > info.Moves.Count(m => m.Source == MoveSource.EggMove || m.Source == MoveSource.InheritLevelUp);
        }

        /// <summary>Checks if the form may be different than the original encounter detail.</summary>
        /// <param name="pkm">Pokémon</param>
        /// <param name="species">Original species</param>
        /// <param name="form">Original form</param>
        internal static bool IsFormChangeable(PKM pkm, int species, int form)
        {
            if (FormChange.Contains(species))
                return true;
            if (species != pkm.Species && IsEvolvedFormChange(pkm, form))
                return true;
            if (species == (int)Species.Zygarde && pkm.InhabitedGeneration(7) && pkm.AltForm > 1)
                return true;
            return false;
        }

        internal static bool GetCanInheritMoves(int species)
        {
            if (FixedGenderFromBiGender.Contains(species)) // Nincada -> Shedinja loses gender causing 'false', edge case
                return true;
            var pi = PKX.Personal[species];
            if (!pi.Genderless && !pi.OnlyMale)
                return true;
            if (MixedGenderBreeding.Contains(species))
                return true;
            return false;
        }

        internal static bool GetCanLearnMachineMove(PKM pkm, int move, int generation, GameVersion version = GameVersion.Any)
        {
            return MoveList.GetValidMoves(pkm, version, EvolutionChain.GetValidPreEvolutions(pkm), generation, Machine: true).Contains(move);
        }

        internal static bool GetCanRelearnMove(PKM pkm, int move, int generation, IReadOnlyList<EvoCriteria> evos, GameVersion version = GameVersion.Any)
        {
            return MoveList.GetValidMoves(pkm, version, evos, generation, LVL: true, Relearn: true).Contains(move);
        }

        internal static bool GetCanKnowMove(PKM pkm, int move, int generation, IReadOnlyList<EvoCriteria> evos, GameVersion version = GameVersion.Any)
        {
            if (pkm.Species == (int)Species.Smeargle)
                return !InvalidSketch.Contains(move);
            return MoveList.GetValidMoves(pkm, version, evos, generation, LVL: true, Relearn: true, Tutor: true, Machine: true).Contains(move);
        }

        private static int GetMaxLevelGeneration(PKM pkm)
        {
            return GetMaxLevelGeneration(pkm, pkm.GenNumber);
        }

        private static int GetMaxLevelGeneration(PKM pkm, int generation)
        {
            if (!pkm.InhabitedGeneration(generation))
                return pkm.Met_Level;

            if (pkm.Format <= 2)
            {
                if (generation == 1 && FutureEvolutionsGen1_Gen2LevelUp.Contains(pkm.Species))
                    return pkm.CurrentLevel - 1;
                return pkm.CurrentLevel;
            }

            if (pkm.Species == (int)Species.Sylveon && generation == 5)
                return pkm.CurrentLevel - 1;

            if (pkm.Gen3 && pkm.Format > 4 && pkm.Met_Level == pkm.CurrentLevel && FutureEvolutionsGen3_LevelUpGen4.Contains(pkm.Species))
                return pkm.Met_Level - 1;

            if (!pkm.HasOriginalMetLocation)
                return pkm.Met_Level;

            return pkm.CurrentLevel;
        }

        internal static int GetMaxLevelEncounter(PKM pkm)
        {
            // Only for gen 3 pokemon in format 3, after transfer to gen 4 it should return transfer level
            if (pkm.Format == 3 && pkm.WasEgg)
                return 5;

            // Only for gen 4 pokemon in format 4, after transfer to gen 5 it should return transfer level
            if (pkm.Format == 4 && pkm.Gen4 && pkm.WasEgg)
                return 1;

            return pkm.HasOriginalMetLocation ? pkm.Met_Level : GetMaxLevelGeneration(pkm);
        }

        internal static bool IsCatchRateHeldItem(int rate) => ParseSettings.AllowGen1Tradeback && HeldItems_GSC.Contains((ushort) rate);

        internal const GameVersion NONE = GameVersion.Invalid;
        internal static readonly LearnVersion LearnNONE = new LearnVersion(-1);

        internal static bool HasVisitedB2W2(this PKM pkm, int species) => pkm.InhabitedGeneration(5, species);
        internal static bool HasVisitedORAS(this PKM pkm, int species) => pkm.InhabitedGeneration(6, species) && (pkm.AO || !pkm.IsUntraded);
        internal static bool HasVisitedUSUM(this PKM pkm, int species) => pkm.InhabitedGeneration(7, species) && (pkm.USUM || !pkm.IsUntraded);
        internal static bool IsMovesetRestricted(this PKM pkm, int gen) => (pkm.GG && gen == 7) || pkm.IsUntraded;

        public static bool HasMetLocationUpdatedTransfer(int originalGeneration, int currentGeneration)
        {
            if (originalGeneration < 3)
                return currentGeneration >= 3;
            if (originalGeneration <= 4)
                return currentGeneration != originalGeneration;
            return false;
        }

        public static bool IsValidMissingLanguage(PKM pkm)
        {
            return pkm.Format == 5 && pkm.BW;
        }

        public static string GetG1OT_GFMew(int lang) => lang == (int)LanguageID.Japanese ? "ゲーフリ" : "GF";
        public static string GetG5OT_NSparkle(int lang) => lang == (int)LanguageID.Japanese ? "Ｎ" : "N";

        public const string Stadium1JP = "スタジアム";

        public static int GetGBStadiumOTID_JPN(GameVersion s)
        {
            return s == GameVersion.Stadium2 ? 2000 : 1999;
        }

        public static int GetMaxLengthOT(int gen, LanguageID lang)
        {
            switch (lang)
            {
                case LanguageID.Korean:
                case LanguageID.Japanese: return gen >= 6 ? 6 : 5;
                default: return gen >= 6 ? 12 : 7;
            }
        }

        public static int GetMaxLengthNickname(int gen, LanguageID lang)
        {
            switch (lang)
            {
                case LanguageID.Korean:
                case LanguageID.Japanese: return gen >= 6 ? 6 : 5;
                default: return gen >= 6 ? 12 : 10;
            }
        }

        public static bool GetIsFixedIVSequenceValidSkipRand(IReadOnlyList<int> IVs, PKM pkm, int max = 31)
        {
            for (int i = 0; i < 6; i++)
            {
                if ((uint) IVs[i] > max) // random
                    continue;
                if (IVs[i] != pkm.GetIV(i))
                    return false;
            }
            return true;
        }

        public static bool GetIsFixedIVSequenceValidNoRand(IReadOnlyList<int> IVs, PKM pkm)
        {
            for (int i = 0; i < 6; i++)
            {
                if (IVs[i] != pkm.GetIV(i))
                    return false;
            }
            return true;
        }
    }
}
