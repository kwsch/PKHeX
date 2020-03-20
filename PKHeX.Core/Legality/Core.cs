using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        /// <summary> e-Reader Berry is Enigma or special berry </summary>
        public static bool EReaderBerryIsEnigma { get; set; } = true;

        /// <summary> e-Reader Berry Name </summary>
        public static string EReaderBerryName { get; set; } = string.Empty;

        /// <summary> e-Reader Berry Name formatted in Title Case </summary>
        public static string EReaderBerryDisplayName => string.Format(LegalityCheckStrings.L_XEnigmaBerry_0, Util.ToTitleCase(EReaderBerryName.ToLower()));

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

        internal static IReadOnlyList<int>[] GetValidMovesAllGens(PKM pkm, IReadOnlyList<EvoCriteria>[] evoChains, int minLvLG1 = 1, int minLvLG2 = 1, bool LVL = true, bool Tutor = true, bool Machine = true, bool MoveReminder = true, bool RemoveTransferHM = true)
        {
            var Moves = new IReadOnlyList<int>[evoChains.Length];
            for (int i = 1; i < evoChains.Length; i++)
            {
                if (evoChains[i].Count != 0)
                    Moves[i] = GetValidMoves(pkm, evoChains[i], i, minLvLG1, minLvLG2, LVL, Tutor, Machine, MoveReminder, RemoveTransferHM).ToList();
                else
                    Moves[i] = Array.Empty<int>();
            }
            return Moves;
        }

        internal static IEnumerable<int> GetValidMoves(PKM pkm, IReadOnlyList<EvoCriteria>[] evoChains, bool LVL = true, bool Tutor = true, bool Machine = true, bool MoveReminder = true, bool RemoveTransferHM = true)
        {
            GameVersion version = (GameVersion)pkm.Version;
            if (!pkm.IsUntraded)
                version = GameVersion.Any;
            return GetValidMoves(pkm, version, evoChains, minLvLG1: 1, minLvLG2: 1, LVL: LVL, Relearn: false, Tutor: Tutor, Machine: Machine, MoveReminder: MoveReminder, RemoveTransferHM: RemoveTransferHM);
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

        internal static IEnumerable<int> GetValidRelearn(PKM pkm, int species, int form, bool inheritlvlmoves, GameVersion version = GameVersion.Any)
        {
            if (pkm.GenNumber < 6)
                return Array.Empty<int>();

            var r = new List<int>();
            r.AddRange(MoveEgg.GetRelearnLVLMoves(pkm, species, 1, form, version));

            if (pkm.Format == 6 && pkm.Species != 678)
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
                    int[] getRBYCompatibleMoves(int[] moves) => pkm.Format == 1 ? moves.Where(m => m <= MaxMoveID_1).ToArray() : moves;
                    if (pkm.InhabitedGeneration(2))
                        return getRBYCompatibleMoves(LevelUpGS[species].GetMoves(lvl));
                    break;
                case GameVersion.C:
                    if (pkm.InhabitedGeneration(2))
                        return getRBYCompatibleMoves(LevelUpC[species].GetMoves(lvl));
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
                    // only difference in FR/LG is deoxys which doesn't breed.
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

        internal static bool GetCanBeEgg23(PKM pkm)
        {
            if (pkm.IsEgg)
                return true;
            if (pkm.Format > 2 && pkm.Ball != 4)
                return false;

            int lvl = pkm.CurrentLevel;
            if (lvl < 5)
                return false;

            if (pkm.Format > 3 && pkm.Met_Level < 5)
                return false;
            if (pkm.Format > 3 && pkm.FatefulEncounter)
                return false;

            return IsEvolutionValid(pkm);
        }

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

        internal static ICollection<int> GetWildBalls(PKM pkm)
        {
            return pkm.GenNumber switch
            {
                1 => WildPokeBalls1,
                2 => WildPokeBalls2,
                3 => WildPokeBalls3,
                4 => (pkm.HGSS ? WildPokeBalls4_HGSS : WildPokeBalls4_DPPt),
                5 => WildPokeBalls5,
                6 => WildPokeballs6,
                7 => (pkm.GG ? WildPokeballs7b : WildPokeballs7),
                8 => WildPokeballs8,
                _ => Array.Empty<int>()
            };
        }

        internal static int GetEggHatchLevel(PKM pkm) => GetEggHatchLevel(pkm.Format);
        internal static int GetEggHatchLevel(int gen) => gen <= 3 ? 5 : 1;

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

        private static bool IsEvolvedFormChange(PKM pkm)
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
                if ((int) Species.Toxtricity == pkm.Species)
                    return pkm.AltForm == EvolutionMethod.GetAmpLowKeyResult(pkm.Nature);
                if ((int) Species.Alcremie == pkm.Species)
                    return true;
            }
            if ((pkm.Species == (int)Species.Meowstic || pkm.Species == (int)Species.Indeedee) && pkm.Gender == 1)
                return pkm.AltForm == 1;
            return pkm.Species == 773;
        }

        internal static bool IsEvolutionValid(PKM pkm, int minSpecies = -1, int minLevel = -1)
        {
            var curr = EvolutionChain.GetValidPreEvolutions(pkm, minLevel: minLevel);
            var min = curr.FindLast(z => z.Species == minSpecies);
            if (min != null && min.Level < minLevel)
                return false;
            var poss = EvolutionChain.GetValidPreEvolutions(pkm, lvl: 100, minLevel: minLevel, skipChecks: true);

            if (minSpecies != -1)
            {
                int last = poss.FindLastIndex(z => z.Species == minSpecies);
                return curr.Count >= last;
            }
            int gen = pkm.GenNumber;
            if (gen >= 3 && GetSplitBreedGeneration(gen).Contains(GetBaseSpecies(poss, 1).Species))
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
            if (info.EncounterMatch.EggEncounter && !pkm.WasGiftEgg && !pkm.WasEventEgg && allowegg)
            {
                if (IsMoveInherited(pkm, info, moves))
                    LearnLevel = Math.Min(LearnLevel, gen <= 3 ? 6 : 2);
            }

            // If has original met location the minimum evolution level is one level after met level
            // Gen 3 pokemon in gen 4 games: minimum level is one level after transfer to generation 4
            // VC pokemon: minimum level is one level after transfer to generation 7
            // Sylveon: always one level after met level, for gen 4 and 5 eevees in gen 6 games minimum for evolution is one level after transfer to generation 5
            if (pkm.HasOriginalMetLocation || (pkm.Format == 4 && pkm.Gen3) || pkm.VC || pkm.Species == (int)Species.Sylveon)
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
        internal static bool IsFormChangeable(PKM pkm, int species)
        {
            if (FormChange.Contains(species))
                return true;
            if (species != pkm.Species && IsEvolvedFormChange(pkm))
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
            return GetValidMoves(pkm, version, EvolutionChain.GetValidPreEvolutions(pkm), generation, Machine: true).Contains(move);
        }

        internal static bool GetCanRelearnMove(PKM pkm, int move, int generation, GameVersion version = GameVersion.Any)
        {
            return GetValidMoves(pkm, version, EvolutionChain.GetValidPreEvolutions(pkm), generation, LVL: true, Relearn: true).Contains(move);
        }

        internal static bool GetCanKnowMove(PKM pkm, int move, int generation, GameVersion version = GameVersion.Any)
        {
            if (pkm.Species == (int)Species.Smeargle)
                return !InvalidSketch.Contains(move);
            return GetValidMoves(pkm, version, EvolutionChain.GetValidPreEvolutions(pkm), generation, LVL: true, Relearn: true, Tutor: true, Machine: true).Contains(move);
        }

        internal static EvoCriteria GetBaseEggSpecies(PKM pkm, int skipOption = 0)
        {
            if (pkm.Format == 1)
                return GetBaseSpecies(pkm, generation: 2);
            return GetBaseSpecies(pkm, skipOption);
        }

        internal static EvoCriteria GetBaseSpecies(PKM pkm, int skipOption = 0, int generation = -1)
        {
            int tree = generation != -1 ? generation : pkm.Format;
            var table = EvolutionTree.GetEvolutionTree(pkm, tree);
            int maxSpeciesOrigin = generation != -1 ? GetMaxSpeciesOrigin(generation) : -1;
            var evos = table.GetValidPreEvolutions(pkm, maxLevel: 100, maxSpeciesOrigin: maxSpeciesOrigin, skipChecks: true);
            return GetBaseSpecies(evos, skipOption);
        }

        private static readonly EvoCriteria Nincada = new EvoCriteria(290, 0)
        {
            Method = (int)EvolutionType.LevelUp,
            MinLevel = 20, Level = 20,
            RequiresLvlUp = true,
        };

        private static readonly EvoCriteria EvoEmpty = new EvoCriteria(0, 0)
        {
            Method = (int)EvolutionType.None,
        };

        internal static EvoCriteria GetBaseSpecies(IReadOnlyList<EvoCriteria> evos, int skipOption = 0)
        {
            int species = evos[0].Species;
            if (species == (int)Species.Shedinja) // Shedinja
                return Nincada; // Nincada

            // skip n from end, return species if invalid index
            int index = evos.Count - 1 - skipOption;
            return (uint)index >= evos.Count ? EvoEmpty : evos[index];
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

        internal static int GetMinLevelEncounter(PKM pkm)
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

        private static IEnumerable<int> GetValidMoves(PKM pkm, GameVersion Version, IReadOnlyList<IReadOnlyList<EvoCriteria>> vs, int minLvLG1 = 1, int minLvLG2 = 1, bool LVL = false, bool Relearn = false, bool Tutor = false, bool Machine = false, bool MoveReminder = true, bool RemoveTransferHM = true)
        {
            var r = new List<int> { 0 };
            if (Relearn && pkm.Format >= 6)
                r.AddRange(pkm.RelearnMoves);

            int start = pkm.GenNumber;
            if (start < 0)
                start = pkm.Format; // be generous instead of returning nothing

            for (int gen = start; gen <= pkm.Format; gen++)
            {
                if (vs[gen].Count != 0)
                    r.AddRange(GetValidMoves(pkm, Version, vs[gen], gen, minLvLG1: minLvLG1, minLvLG2: minLvLG2, LVL: LVL, Relearn: false, Tutor: Tutor, Machine: Machine, MoveReminder: MoveReminder, RemoveTransferHM: RemoveTransferHM));
            }

            return r.Distinct();
        }

        private static IEnumerable<int> GetValidMoves(PKM pkm, GameVersion Version, IReadOnlyList<EvoCriteria> vs, int generation, int minLvLG1 = 1, int minLvLG2 = 1, bool LVL = false, bool Relearn = false, bool Tutor = false, bool Machine = false, bool MoveReminder = true, bool RemoveTransferHM = true)
        {
            var r = new List<int> { 0 };
            int species = pkm.Species;

            if (FormChangeMoves.Contains(species)) // Deoxys & Shaymin & Giratina (others don't have extra but whatever)
            {
                // These don't evolve, so don't bother iterating for all entries in the evolution chain (should always be count==1).
                int formcount;

                // In gen 3 deoxys has different forms depending on the current game, in the PersonalInfo there is no alternate form info
                if (pkm.Format == 3 && species == (int)Species.Deoxys)
                    formcount = 4;
                else
                    formcount = pkm.PersonalInfo.FormeCount;

                for (int i = 0; i < formcount; i++)
                    r.AddRange(GetMoves(pkm, species, minLvLG1, minLvLG2, vs[0].Level, i, Tutor, Version, LVL, Tutor, Machine, MoveReminder, RemoveTransferHM, generation));
                if (Relearn)
                    r.AddRange(pkm.RelearnMoves);
                return r.Distinct();
            }

            // Special Type Tutors Availability
            bool moveTutor = Tutor || MoveReminder; // Usually true, except when called for move suggestions (no tutored moves)

            for (var i = 0; i < vs.Count; i++)
            {
                var evo = vs[i];
                var moves = GetEvoMoves(pkm, Version, vs, generation, minLvLG1, minLvLG2, LVL, Tutor, Machine, MoveReminder, RemoveTransferHM, moveTutor, i, evo);
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

        private static IEnumerable<int> GetEvoMoves(PKM pkm, GameVersion Version, IReadOnlyList<EvoCriteria> vs, int generation, int minLvLG1, int minLvLG2, bool LVL, bool Tutor, bool Machine, bool MoveReminder, bool RemoveTransferHM, bool moveTutor, int i, EvoCriteria evo)
        {
            int minlvlevo1 = GetEvoMoveMinLevel1(pkm, generation, minLvLG1, evo);
            int minlvlevo2 = GetEvoMoveMinLevel2(pkm, generation, minLvLG2, evo);
            var maxLevel = evo.Level;
            if (i != 0 && vs[i - 1].RequiresLvlUp) // evolution
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

        internal const GameVersion NONE = GameVersion.Invalid;
        internal static readonly LearnVersion LearnNONE = new LearnVersion(-1);

        internal static bool HasVisitedB2W2(this PKM pkm) => pkm.InhabitedGeneration(5);
        internal static bool HasVisitedORAS(this PKM pkm) => pkm.InhabitedGeneration(6) && (pkm.AO || !pkm.IsUntraded);
        internal static bool HasVisitedUSUM(this PKM pkm) => pkm.InhabitedGeneration(7) && (pkm.USUM || !pkm.IsUntraded);
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

        public static string GetGBStadiumOTName(bool jp, GameVersion s)
        {
            if (jp)
                return Stadium1JP;
            return s == GameVersion.Stadium2 ? "Stadium" : "STADIUM";
        }

        public static int GetGBStadiumOTID(bool jp, GameVersion s)
        {
            if (jp)
                return GetGBStadiumOTID_JPN(s);
            return 2000;
        }

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
