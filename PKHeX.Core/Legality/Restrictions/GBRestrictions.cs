using System;
using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.Legal;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    /// <summary>
    /// Miscellaneous GB Era restriction logic for legality checking
    /// </summary>
    internal static class GBRestrictions
    {
        private static readonly int[] G1CaterpieMoves = { 33, 81 };
        private static readonly int[] G1WeedleMoves = { 40, 81 };
        //private static readonly int[] G1MetapodMoves = { 33, 81, 106 };
        private static readonly int[] G1KakunaMoves = { 40, 81, 106 };
        private static readonly int[] G1Exeggcute_IncompatibleMoves = { 78, 77, 79 };

        internal static readonly int[] Stadium_CatchRate =
        {
            167, // Normal Box
            168, // Gorgeous Box
        };

        private static readonly HashSet<int> Stadium_GiftSpecies = new()
        {
            (int)Bulbasaur,
            (int)Charmander,
            (int)Squirtle,
            (int)Psyduck,
            (int)Hitmonlee,
            (int)Hitmonchan,
            (int)Eevee,
            (int)Omanyte,
            (int)Kabuto,
        };

        /// <summary>
        /// Species that have a specific minimum amount of moves based on their evolution state.
        /// </summary>
        private static readonly HashSet<int> SpecialMinMoveSlots = new()
        {
            (int)Pikachu,
            (int)Raichu,
            (int)NidoranF,
            (int)Nidorina,
            (int)Nidoqueen,
            (int)NidoranM,
            (int)Nidorino,
            (int)Nidoking,
            (int)Clefable,
            (int)Ninetales,
            (int)Wigglytuff,
            (int)Arcanine,
            (int)Cloyster,
            (int)Exeggutor,
            (int)Tangela,
            (int)Starmie,
        };

        internal static bool TypeIDExists(int type) => Types_Gen1.Contains(type);

        /// <summary>
        /// Valid type IDs extracted from the Personal Table used for R/G/B/Y games.
        /// </summary>
        private static readonly HashSet<int> Types_Gen1 = new()
        {
            0, 1, 2, 3, 4, 5, 7, 8, 20, 21, 22, 23, 24, 25, 26
        };

        /// <summary>
        /// Species that have a catch rate value that is different from their pre-evolutions, and cannot be obtained directly.
        /// </summary>
        internal static readonly HashSet<int> Species_NotAvailable_CatchRate = new()
        {
            (int)Butterfree,
            (int)Pidgeot,
            (int)Nidoqueen,
            (int)Nidoking,
            (int)Ninetales,
            (int)Vileplume,
            (int)Persian,
            (int)Arcanine,
            (int)Poliwrath,
            (int)Alakazam,
            (int)Machamp,
            (int)Victreebel,
            (int)Rapidash,
            (int)Cloyster,
            (int)Exeggutor,
            (int)Starmie,
            (int)Dragonite,
        };

        internal static readonly HashSet<int> Trade_Evolution1 = new()
        {
            (int)Kadabra,
            (int)Machoke,
            (int)Graveler,
            (int)Haunter,
        };

        public static bool RateMatchesEncounter(int species, GameVersion version, int rate)
        {
            if (version.Contains(YW))
            {
                if (rate == PersonalTable.Y[species].CatchRate)
                    return true;
                if (version == YW) // no RB
                    return false;
            }
            return rate == PersonalTable.RB[species].CatchRate;
        }

        private static int[] GetMinLevelLearnMoveG1(int species, List<int> moves)
        {
            var result = new int[moves.Count];
            for (int i = 0; i < result.Length; i++)
                result[i] = MoveLevelUp.GetIsLevelUp1(species, moves[i], 100, 0, 0).Level;
            return result;
        }

        private static int[] GetMaxLevelLearnMoveG1(int species, List<int> moves)
        {
            var result = new int[moves.Count];

            int index = PersonalTable.RB.GetFormIndex(species, 0);
            if (index == 0)
                return result;

            var pi_rb = ((PersonalInfoG1)PersonalTable.RB[index]).Moves;
            var pi_y = ((PersonalInfoG1)PersonalTable.Y[index]).Moves;

            for (int m = 0; m < moves.Count; m++)
            {
                bool start = pi_rb.Contains(moves[m]) && pi_y.Contains(moves[m]);
                result[m] = start ? 1 : Math.Max(GetHighest(LevelUpRB), GetHighest(LevelUpY));
                int GetHighest(IReadOnlyList<Learnset> learn) => learn[index].GetLevelLearnMove(moves[m]);
            }
            return result;
        }

        private static List<int>[] GetExclusiveMovesG1(int species1, int species2, IEnumerable<int> tmhm, IEnumerable<int> moves)
        {
            // Return from two species the exclusive moves that only one could learn and also the current pokemon have it in its current moveset
            var moves1 = MoveLevelUp.GetMovesLevelUp1(species1, 0, 1, 100);
            var moves2 = MoveLevelUp.GetMovesLevelUp1(species2, 0, 1, 100);

            // Remove common moves and remove tmhm, remove not learned moves
            var common = new HashSet<int>(moves1.Intersect(moves2).Concat(tmhm));
            var hashMoves = new HashSet<int>(moves);
            moves1.RemoveAll(x => !hashMoves.Contains(x) || common.Contains(x));
            moves2.RemoveAll(x => !hashMoves.Contains(x) || common.Contains(x));
            return new[] { moves1, moves2 };
        }

        internal static void GetIncompatibleEvolutionMoves(PKM pkm, IReadOnlyList<int> moves, IReadOnlyList<int> tmhm, out int previousspecies, out IList<int> incompatible_previous, out IList<int> incompatible_current)
        {
            switch (pkm.Species)
            {
                case (int)Nidoking when moves.Contains(31) && moves.Contains(37):
                    // Nidoking learns Thrash at level 23
                    // Nidorino learns Fury Attack at level 36, Nidoran♂ at level 30
                    // Other moves are either learned by Nidoran♂ up to level 23 or by TM
                    incompatible_current = new[] { 31 };
                    incompatible_previous = new[] { 37 };
                    previousspecies = 33;
                    return;

                case (int)Exeggutor when moves.Contains(23) && moves.Any(m => G1Exeggcute_IncompatibleMoves.Contains(m)):
                    // Exeggutor learns Stomp at level 28
                    // Exeggcute learns Stun Spore at 32, PoisonPowder at 37 and Sleep Powder at 48
                    incompatible_current = new[] { 23 };
                    incompatible_previous = G1Exeggcute_IncompatibleMoves;
                    previousspecies = 103;
                    return;

                case (int)Vaporeon or (int)Jolteon or (int)Flareon:
                    incompatible_previous = new List<int>();
                    incompatible_current = new List<int>();
                    previousspecies = 133;
                    var ExclusiveMoves = GetExclusiveMovesG1((int)Eevee, pkm.Species, tmhm, moves);
                    var EeveeLevels = GetMinLevelLearnMoveG1((int)Eevee, ExclusiveMoves[0]);
                    var EvoLevels = GetMaxLevelLearnMoveG1(pkm.Species, ExclusiveMoves[1]);

                    for (int i = 0; i < ExclusiveMoves[0].Count; i++)
                    {
                        // There is a evolution move with a lower level that current Eevee move
                        var el = EeveeLevels[i];
                        if (EvoLevels.Any(ev => ev < el))
                            incompatible_previous.Add(ExclusiveMoves[0][i]);
                    }
                    for (int i = 0; i < ExclusiveMoves[1].Count; i++)
                    {
                        // There is an Eevee move with a greater level that current evolution move
                        var el = EvoLevels[i];
                        if (EeveeLevels.Any(ev => ev > el))
                            incompatible_current.Add(ExclusiveMoves[1][i]);
                    }
                    return;
            }
            incompatible_previous = Array.Empty<int>();
            incompatible_current = Array.Empty<int>();
            previousspecies = 0;
        }

        internal static int GetRequiredMoveCount(PK1 pk, IReadOnlyList<int> moves, LegalInfo info, IReadOnlyList<int> initialmoves)
        {
            if (!pk.Gen1_NotTradeback) // No Move Deleter in Gen 1
                return 1; // Move Deleter exits, slots from 2 onwards can always be empty

            int required = GetRequiredMoveCount(pk, moves, info.EncounterMoves.LevelUpMoves, initialmoves);
            if (required >= 4)
                return 4;

            // tm, hm and tutor moves replace a free slots if the pokemon have less than 4 moves
            // Ignore tm, hm and tutor moves already in the learnset table
            var learn = info.EncounterMoves.LevelUpMoves;
            var tmhm = info.EncounterMoves.TMHMMoves;
            var tutor = info.EncounterMoves.TutorMoves;
            var union = initialmoves.Union(learn[1]);
            required += moves.Count(m => m != 0 && union.All(t => t != m) && (tmhm[1].Any(t => t == m) || tutor[1].Any(t => t == m)));

            return Math.Min(4, required);
        }

        private static int GetRequiredMoveCount(PKM pk, IReadOnlyList<int> moves, IReadOnlyList<int>[] learn, IReadOnlyList<int> initialmoves)
        {
            if (SpecialMinMoveSlots.Contains(pk.Species))
                return GetRequiredMoveCountSpecial(pk, moves, learn);

            // A pokemon is captured with initial moves and can't forget any until have all 4 slots used
            // If it has learn a move before having 4 it will be in one of the free slots
            int required = GetRequiredMoveSlotsRegular(pk, moves, learn, initialmoves);
            return required != 0 ? required : GetRequiredMoveCountDecrement(pk, moves, learn, initialmoves);
        }

        private static int GetRequiredMoveSlotsRegular(PKM pk, IReadOnlyList<int> moves, IReadOnlyList<int>[] learn, IReadOnlyList<int> initialmoves)
        {
            int species = pk.Species;
            int catch_rate = ((PK1)pk).Catch_Rate;
            // Caterpie and Metapod evolution lines have different count of possible slots available if captured in different evolutionary phases
            // Example: a level 7 caterpie evolved into metapod will have 3 learned moves, a captured metapod will have only 1 move
            if ((species is (int)Metapod or (int)Butterfree) && catch_rate is 120)
            {
                // Captured as Metapod without Caterpie moves
                return initialmoves.Union(learn[1]).Distinct().Count(lm => lm != 0 && !G1CaterpieMoves.Contains(lm));
                // There is no valid Butterfree encounter in generation 1 games
            }
            if ((species is (int)Kakuna or (int)Beedrill) && (catch_rate is 45 or 120))
            {
                if (species == (int)Beedrill && catch_rate == 45) // Captured as Beedril without Weedle and Kakuna moves
                    return initialmoves.Union(learn[1]).Distinct().Count(lm => lm != 0 && !G1KakunaMoves.Contains(lm));

                // Captured as Kakuna without Weedle moves
                return initialmoves.Union(learn[1]).Distinct().Count(lm => lm != 0 && !G1WeedleMoves.Contains(lm));
            }

            return IsMoveCountRequired3(species, pk.CurrentLevel, moves) ? 3 : 0; // no match
        }

        private static bool IsMoveCountRequired3(int species, int level, IReadOnlyList<int> moves) => species switch
        {
            // Species that evolve and learn the 4th move as evolved species at a greater level than base species
            // The 4th move is included in the level up table set as a pre-evolution move,
            // it should be removed from the used slots count if is not the learn move
            (int)Pidgeotto => level < 21 && !moves.Contains(018), // Whirlwind
            (int)Sandslash => level < 27 && !moves.Contains(040), // Poison Sting
            (int)Parasect  => level < 30 && !moves.Contains(147), // Spore
            (int)Golduck   => level < 39 && !moves.Contains(093), // Confusion
            (int)Dewgong   => level < 44 && !moves.Contains(156), // Rest
            (int)Weezing   => level < 39 && !moves.Contains(108), // Smoke Screen
            (int)Haunter or (int)Gengar => level < 29 && !moves.Contains(095), // Hypnosis
            _ => false,
        };

        private static int GetRequiredMoveCountDecrement(PKM pk, IReadOnlyList<int> moves, IReadOnlyList<int>[] learn, IReadOnlyList<int> initialmoves)
        {
            int usedslots = initialmoves.Union(learn[1]).Where(m => m != 0).Distinct().Count();
            switch (pk.Species)
            {
                case (int)Venonat: // Venonat; ignore Venomoth (by the time Venonat evolves it will always have 4 moves)
                    if (pk.CurrentLevel >= 11 && !moves.Contains(48)) // Supersonic
                        usedslots--;
                    if (pk.CurrentLevel >= 19 && !moves.Contains(93)) // Confusion
                        usedslots--;
                    break;
                case (int)Kadabra or (int)Alakazam: // Abra & Kadabra
                    int catch_rate = ((PK1)pk).Catch_Rate;
                    if (catch_rate != 100)// Initial Yellow Kadabra Kinesis (move 134)
                        usedslots--;
                    if (catch_rate == 200 && pk.CurrentLevel < 20) // Kadabra Disable, not learned until 20 if captured as Abra (move 50)
                        usedslots--;
                    break;
                case (int)Cubone or (int)Marowak: // Cubone & Marowak
                    if (!moves.Contains(39)) // Initial Yellow Tail Whip
                        usedslots--;
                    if (!moves.Contains(125)) // Initial Yellow Bone Club
                        usedslots--;
                    if (pk.Species == 105 && pk.CurrentLevel < 33 && !moves.Contains(116)) // Marowak evolved without Focus Energy
                        usedslots--;
                    break;
                case (int)Chansey:
                    if (!moves.Contains(39)) // Yellow Initial Tail Whip
                        usedslots--;
                    if (!moves.Contains(3)) // Yellow Lvl 12 and Initial Red/Blue Double Slap
                        usedslots--;
                    break;
                case (int)Mankey when pk.CurrentLevel >= 9 && !moves.Contains(67): // Mankey (Low Kick)
                case (int)Pinsir when pk.CurrentLevel >= 21 && !moves.Contains(20): // Pinsir (Bind)
                case (int)Gyarados when pk.CurrentLevel < 32: // Gyarados
                    usedslots--;
                    break;
                default: return usedslots;
            }
            return usedslots;
        }

        private static int GetRequiredMoveCountSpecial(PKM pk, IReadOnlyList<int> moves, IReadOnlyList<int>[] learn)
        {
            // Species with few mandatory slots, species with stone evolutions that could evolve at lower level and do not learn any more moves
            // and Pikachu and Nidoran family, those only have mandatory the initial moves and a few have one level up moves,
            // every other move could be avoided switching game or evolving
            var mandatory = GetRequiredMoveCountLevel(pk);
            switch (pk.Species)
            {
                case (int)Exeggutor when pk.CurrentLevel >= 28: // Exeggutor
                    // At level 28 learn different move if is a Exeggute or Exeggutor
                    if (moves.Contains(73))
                        mandatory.Add(73); // Leech Seed level 28 Exeggute
                    if (moves.Contains(23))
                        mandatory.Add(23); // Stomp level 28 Exeggutor
                    break;
                case (int)Pikachu when pk.CurrentLevel >= 33:
                    mandatory.Add(97); // Pikachu always learns Agility
                    break;
                case (int)Tangela:
                    mandatory.Add(132); // Tangela always has Constrict as Initial Move
                    break;
            }

            // Add to used slots the non-mandatory moves from the learnset table that the pokemon have learned
            return mandatory.Count + moves.Where(m => m != 0).Count(m => !mandatory.Contains(m) && learn[1].Contains(m));
        }

        private static List<int> GetRequiredMoveCountLevel(PKM pk)
        {
            int species = pk.Species;
            int basespecies = EvoBase.GetBaseSpecies(pk).Species;
            int maxlevel = 1;
            int minlevel = 1;

            if (species == (int)Tangela) // Tangela moves before level 32 are different in RB vs Y
            {
                minlevel = 32;
                maxlevel = pk.CurrentLevel;
            }
            else if (species is >= (int)NidoranF and <= (int)Nidoking && pk.CurrentLevel >= 8)
            {
                maxlevel = 8; // Always learns a third move at level 8
            }

            if (minlevel > pk.CurrentLevel)
                return new List<int>();

            return MoveLevelUp.GetMovesLevelUp1(basespecies, 0, maxlevel, minlevel);
        }

        internal static IEnumerable<GameVersion> GetGen2Versions(IEncounterable enc, bool korean)
        {
            if (ParseSettings.AllowGen2Crystal(korean) && enc.Version is C or GSC)
                yield return C;
            yield return GS;
        }

        internal static IEnumerable<GameVersion> GetGen1Versions(IEncounterable enc)
        {
            if (enc.Species == (int)Eevee && enc.Version == Stadium)
            {
                // Stadium Eevee; check for RB and yellow initial moves
                yield return RB;
                yield return YW;
                yield break;
            }
            if (enc.Version == YW)
            {
                yield return YW;
                yield break;
            }

            // Any encounter marked with version RBY is for pokemon with the same moves and catch rate in RB and Y,
            // it is sufficient to check just RB's case
            yield return RB;
        }

        private static bool GetCatchRateMatchesPreEvolution(PK1 pkm, int catch_rate)
        {
            // For species catch rate, discard any species that has no valid encounters and a different catch rate than their pre-evolutions
            var table = EvolutionTree.GetEvolutionTree(1);
            var chain = table.GetValidPreEvolutions(pkm, maxLevel: pkm.CurrentLevel);
            foreach (var entry in chain)
            {
                var s = entry.Species;
                if (Species_NotAvailable_CatchRate.Contains(s))
                    continue;
                if (catch_rate == PersonalTable.RB[s].CatchRate || catch_rate == PersonalTable.Y[s].CatchRate)
                    return true;
            }

            // Krabby encounter trade special catch rate
            int species = pkm.Species;
            if (catch_rate == 204 && (species is (int)Krabby or (int)Kingler))
                return true;

            if (Stadium_GiftSpecies.Contains(species) && Stadium_CatchRate.Contains(catch_rate))
                return true;

            return false;
        }

        /// <summary>
        /// Checks if the <see cref="pkm"/> can inhabit <see cref="GameVersion.Gen1"></see>
        /// </summary>
        /// <param name="pkm">Data to check</param>
        /// <returns>true if can inhabit, false if not.</returns>
        private static bool CanInhabitGen1(this PKM pkm)
        {
            // Korean Gen2 games can't trade-back because there are no Gen1 Korean games released
            if (pkm.Korean || pkm.IsEgg)
                return false;

            // Gen2 format with met data can't receive Gen1 moves, unless Stadium 2 is used (Oak's PC).
            // If you put a Pokemon in the N64 box, the met info is retained, even if you switch over to a Gen I game to teach it TMs
            // You can use rare candies from within the lab, so level-up moves from RBY context can be learned this way as well
            // Stadium 2 is GB Cart Era only (not 3DS Virtual Console).
            if (pkm is ICaughtData2 {CaughtData: not 0} && !ParseSettings.AllowGBCartEra)
                return false;

            // Sanity check species, if it could have existed as a pre-evolution.
            int species = pkm.Species;
            if (species <= MaxSpeciesID_1)
                return true;
            return EvolutionLegality.FutureEvolutionsGen1.Contains(species);
        }

        /// <summary>
        /// Gets the Tradeback status depending on various values.
        /// </summary>
        /// <param name="pkm">Pokémon to guess the tradeback status from.</param>
        internal static TradebackType GetTradebackStatusInitial(PKM pkm)
        {
            if (pkm is PK1 pk1)
                return GetTradebackStatusRBY(pk1);

            if (pkm.Format == 2 || pkm.VC2) // Check for impossible tradeback scenarios
                return !pkm.CanInhabitGen1() ? TradebackType.Gen2_NotTradeback : TradebackType.Any;

            // VC2 is released, we can assume it will be TradebackType.Any.
            // Is impossible to differentiate a VC1 pokemon traded to Gen7 after VC2 is available.
            // Met Date cannot be used definitively as the player can change their system clock.
            return TradebackType.Any;
        }

        /// <summary>
        /// Gets the Tradeback status depending on the <see cref="PK1.Catch_Rate"/>
        /// </summary>
        /// <param name="pkm">Pokémon to guess the tradeback status from.</param>
        private static TradebackType GetTradebackStatusRBY(PK1 pkm)
        {
            if (!ParseSettings.AllowGen1Tradeback)
                return TradebackType.Gen1_NotTradeback;

            // Detect tradeback status by comparing the catch rate(Gen1)/held item(Gen2) to the species in the pkm's evolution chain.
            var catch_rate = pkm.Catch_Rate;
            if (catch_rate == 0)
                return TradebackType.WasTradeback;

            bool matchAny = GetCatchRateMatchesPreEvolution(pkm, catch_rate);

            if (!matchAny)
                return TradebackType.WasTradeback;

            if (HeldItems_GSC.Contains((ushort)catch_rate))
                return TradebackType.Any;

            return TradebackType.Gen1_NotTradeback;
        }

        internal static bool IsTradedKadabraG1(PKM pkm)
        {
            if (pkm is not PK1 {Species: (int)Kadabra} pk1)
                return false;
            if (pk1.TradebackStatus == TradebackType.WasTradeback)
                return true;
            if (ParseSettings.ActiveTrainer.Game == (int)Any)
                return false;
            var IsYellow = ParseSettings.ActiveTrainer.Game == (int)YW;
            if (pk1.TradebackStatus == TradebackType.Gen1_NotTradeback)
            {
                // If catch rate is Abra catch rate it wont trigger as invalid trade without evolution, it could be traded as Abra
                // Yellow Kadabra catch rate in Red/Blue game, must be Alakazam
                var table = IsYellow ? PersonalTable.RB : PersonalTable.Y;
                if (pk1.Catch_Rate == table[(int)Kadabra].CatchRate)
                    return true;
            }
            if (IsYellow)
                return false;
            // Yellow only moves in Red/Blue game, must be Alakazam
            if (pk1.HasMove((int)Move.Kinesis)) // Kinesis, yellow only move
                return true;
            if (pk1.CurrentLevel < 20 && pk1.HasMove((int)Move.Disable)) // Obtaining Disable below level 20 implies a yellow only move
                return true;

            return false;
        }
    }
}
