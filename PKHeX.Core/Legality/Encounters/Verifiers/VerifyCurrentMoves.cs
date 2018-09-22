using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.LegalityAnalysis;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic to verify the current <see cref="PKM.Moves"/>.
    /// </summary>
    public static class VerifyCurrentMoves
    {
        public static CheckMoveResult[] VerifyMoves(PKM pkm, LegalInfo info)
        {
            int[] Moves = pkm.Moves;
            var res = ParseMovesForEncounters(pkm, info, Moves);

            // Duplicate Moves Check
            VerifyNoEmptyDuplicates(Moves, res);
            if (Moves[0] == 0) // Can't have an empty moveslot for the first move.
                res[0] = new CheckMoveResult(res[0], Severity.Invalid, LMoveSourceEmpty, CheckIdentifier.Move);

            return res;
        }

        private static CheckMoveResult[] ParseMovesForEncounters(PKM pkm, LegalInfo info, int[] Moves)
        {
            if (pkm.Species == 235) // special handling for Smeargle
                return ParseMovesForSmeargle(pkm, Moves, info); // Smeargle can have any moves except a few

            // gather valid moves for encounter species
            info.EncounterMoves = new ValidEncounterMoves(pkm, info);

            if (info.Generation <= 3)
                pkm.WasEgg = info.EncounterMatch.EggEncounter;

            var EncounterMatchGen = info.EncounterMatch as IGeneration;
            var defaultG1LevelMoves = info.EncounterMoves.LevelUpMoves[1];
            var defaultG2LevelMoves = pkm.InhabitedGeneration(2) ? info.EncounterMoves.LevelUpMoves[2] : null;
            var defaultTradeback = pkm.TradebackStatus;
            if (EncounterMatchGen?.Generation <= 2)
            {
                // Generation 1 can have different minimum level in different encounter of the same species; update valid level moves
                UpdateGen1LevelUpMoves(pkm, info.EncounterMoves, info.EncounterMoves.MinimumLevelGen1, EncounterMatchGen.Generation, info);

                // The same for Generation 2; if move reminder from Stadium 2 is not allowed
                if (!Legal.AllowGen2MoveReminder(pkm) && pkm.InhabitedGeneration(2))
                    UpdateGen2LevelUpMoves(pkm, info.EncounterMoves, info.EncounterMoves.MinimumLevelGen2, EncounterMatchGen.Generation, info);
            }

            var res = info.Generation < 6
                ? ParseMovesPre3DS(pkm, Moves, info)
                : ParseMoves3DS(pkm, Moves, info);

            if (res.All(x => x.Valid))
                return res;

            // not valid
            if (EncounterMatchGen?.Generation <= 2) // restore generation 1 and 2 moves
            {
                info.EncounterMoves.LevelUpMoves[1] = defaultG1LevelMoves;
                if (pkm.InhabitedGeneration(2))
                    info.EncounterMoves.LevelUpMoves[2] = defaultG2LevelMoves;
            }
            pkm.TradebackStatus = defaultTradeback;
            return res;
        }

        private static CheckMoveResult[] ParseMovesForSmeargle(PKM pkm, int[] Moves, LegalInfo info)
        {
            if (!pkm.IsEgg)
                return ParseMovesSketch(pkm, Moves);

            // can only know sketch as egg
            var levelup = new List<int>[info.EvoChainsAllGens.Length];
            levelup[pkm.Format] = new List<int>(1) {166};
            info.EncounterMoves = new ValidEncounterMoves(levelup);
            var source = new MoveParseSource { CurrentMoves = pkm.Moves, };
            return ParseMoves(pkm, source, info);
        }

        private static CheckMoveResult[] ParseMovesIsEggPreRelearn(PKM pkm, int[] Moves, int[] SpecialMoves, EncounterEgg e)
        {
            EggInfoSource infoset = new EggInfoSource(pkm, SpecialMoves, e);
            return VerifyPreRelearnEggBase(pkm, Moves, infoset);
        }

        private static CheckMoveResult[] ParseMovesWasEggPreRelearn(PKM pkm, int[] Moves, LegalInfo info, EncounterEgg e)
        {
            var EventEggMoves = GetSpecialMoves(info.EncounterMatch);
            // Level up moves could not be inherited if Ditto is parent,
            // that means genderless species and male only species except Nidoran and Volbeat (they breed with female nidoran and illumise) could not have level up moves as an egg
            var AllowLevelUp = (pkm.PersonalInfo.Gender > 0 && pkm.PersonalInfo.Gender < 255) || Legal.MixedGenderBreeding.Contains(e.Species);
            int BaseLevel = AllowLevelUp ? 100 : e.LevelMin;
            var LevelUp = Legal.GetBaseEggMoves(pkm, e.Species, e.Version, BaseLevel);

            var TradebackPreevo = pkm.Format == 2 && info.EncounterMatch.Species > 151;
            var NonTradebackLvlMoves = TradebackPreevo
                ? Legal.GetExclusivePreEvolutionMoves(pkm, info.EncounterMatch.Species, info.EvoChainsAllGens[2], 2, e.Version).Where(m => m > Legal.MaxMoveID_1).ToArray()
                : Array.Empty<int>();

            var Egg = MoveEgg.GetEggMoves(pkm, e.Species, pkm.AltForm, e.Version);
            if (info.Generation < 3 && pkm.Format >= 7 && pkm.VC1)
                Egg = Egg.Where(m => m <= Legal.MaxMoveID_1).ToArray();

            bool volt = (info.Generation > 3 || e.Version == GameVersion.E) && Legal.LightBall.Contains(pkm.Species);
            var Special = volt && EventEggMoves.Length == 0 ? new[] { 344 } : Array.Empty<int>(); // Volt Tackle for bred Pichu line

            var source = new MoveParseSource
            {
                CurrentMoves = Moves,
                SpecialSource = Special,
                NonTradeBackLevelUpMoves = NonTradebackLvlMoves,

                EggLevelUpSource = LevelUp,
                EggMoveSource = Egg,
                EggEventSource = EventEggMoves,
            };
            return ParseMoves(pkm, source, info);
        }

        private static CheckMoveResult[] ParseMovesSketch(PKM pkm, int[] Moves)
        {
            var res = new CheckMoveResult[4];
            for (int i = 0; i < 4; i++)
            {
                res[i] = Legal.InvalidSketch.Contains(Moves[i])
                    ? new CheckMoveResult(MoveSource.Unknown, pkm.Format, Severity.Invalid, LMoveSourceInvalidSketch, CheckIdentifier.Move)
                    : new CheckMoveResult(MoveSource.Sketch, pkm.Format, CheckIdentifier.Move);
            }

            return res;
        }

        private static CheckMoveResult[] ParseMoves3DS(PKM pkm, int[] Moves, LegalInfo info)
        {
            info.EncounterMoves.Relearn = info.Generation >= 6 ? pkm.RelearnMoves : Array.Empty<int>();
            if (info.EncounterMatch is IMoveset)
                return ParseMovesSpecialMoveset(pkm, Moves, info);

            // Everything else
            return ParseMovesRelearn(pkm, Moves, info);
        }

        private static CheckMoveResult[] ParseMovesPre3DS(PKM pkm, int[] Moves, LegalInfo info)
        {
            if (pkm.IsEgg && info.EncounterMatch is EncounterEgg egg)
            {
                int[] SpecialMoves = GetSpecialMoves(info.EncounterMatch);
                return ParseMovesIsEggPreRelearn(pkm, Moves, SpecialMoves, egg);
            }
            if (info.EncounterMatch is EncounterEgg e)
                return ParseMovesWasEggPreRelearn(pkm, Moves, info, e);
            if (info.Generation <= 2 && info.EncounterMatch is IGeneration g && (g.Generation == 1 || (g.Generation == 2 && !Legal.AllowGen2MoveReminder(pkm)))) // fixed encounter moves without relearning
                return ParseMovesGenGB(pkm, Moves, info);

            return ParseMovesSpecialMoveset(pkm, Moves, info);
        }

        private static CheckMoveResult[] ParseMovesGenGB(PKM pkm, int[] Moves, LegalInfo info)
        {
            CheckMoveResult[] res = new CheckMoveResult[4];
            var G1Encounter = info.EncounterMatch;
            if (G1Encounter == null)
                return ParseMovesSpecialMoveset(pkm, Moves, info);
            var InitialMoves = Array.Empty<int>();
            int[] SpecialMoves = GetSpecialMoves(info.EncounterMatch);
            var games = info.EncounterMatch is IGeneration g && g.Generation == 1 ? Legal.GetGen1Versions(info) : Legal.GetGen2Versions(info);
            foreach (var ver in games)
            {
                var VerInitialMoves = MoveLevelUp.GetEncounterMoves(G1Encounter.Species, 0, G1Encounter.LevelMin, ver);
                if (VerInitialMoves.Intersect(InitialMoves).Count() == VerInitialMoves.Length)
                    return res;

                var source = new MoveParseSource
                {
                    CurrentMoves = Moves,
                    SpecialSource = SpecialMoves,
                    Base = VerInitialMoves,
                };
                res = ParseMoves(pkm, source, info);
                if (res.All(r => r.Valid))
                    return res;
                InitialMoves = VerInitialMoves;
            }
            return res;
        }

        private static CheckMoveResult[] ParseMovesSpecialMoveset(PKM pkm, int[] Moves, LegalInfo info)
        {
            var source = new MoveParseSource
            {
                CurrentMoves = Moves,
                SpecialSource = GetSpecialMoves(info.EncounterMatch),
            };
            return ParseMoves(pkm, source, info);
        }

        private static int[] GetSpecialMoves(IEncounterable EncounterMatch)
        {
            if (EncounterMatch is IMoveset mg && mg.Moves != null)
                return mg.Moves;
            return Array.Empty<int>();
        }

        private static CheckMoveResult[] ParseMovesRelearn(PKM pkm, int[] Moves, LegalInfo info)
        {
            var source = new MoveParseSource
            {
                CurrentMoves = Moves,
                SpecialSource = GetSpecialMoves(info.EncounterMatch),
            };

            if (info.EncounterMatch is EncounterEgg e)
                source.EggMoveSource = MoveEgg.GetEggMoves(pkm, e.Species, pkm.AltForm, e.Version);

            CheckMoveResult[] res = ParseMoves(pkm, source, info);

            int[] RelearnMoves = pkm.RelearnMoves;
            for (int i = 0; i < 4; i++)
            {
                if ((pkm.IsEgg || res[i].Flag) && !RelearnMoves.Contains(Moves[i]))
                    res[i] = new CheckMoveResult(res[i], Severity.Invalid, string.Format(LMoveRelearnFMiss_0, res[i].Comment), res[i].Identifier);
            }

            return res;
        }

        private static CheckMoveResult[] ParseMoves(PKM pkm, MoveParseSource source, LegalInfo info)
        {
            CheckMoveResult[] res = new CheckMoveResult[4];
            bool AllParsed() => res.All(r => r != null);
            var required = Legal.GetRequiredMoveCount(pkm, source.CurrentMoves, info, source.Base);

            // Check empty moves and relearn moves before generation specific moves
            for (int m = 0; m < 4; m++)
            {
                if (source.CurrentMoves[m] == 0)
                    res[m] = new CheckMoveResult(MoveSource.None, pkm.Format, m < required ? Severity.Fishy : Severity.Valid, LMoveSourceEmpty, CheckIdentifier.Move);
                else if (info.EncounterMoves.Relearn.Contains(source.CurrentMoves[m]))
                    res[m] = new CheckMoveResult(MoveSource.Relearn, info.Generation, Severity.Valid, LMoveSourceRelearn, CheckIdentifier.Move) { Flag = true };
            }

            if (AllParsed())
                return res;

            // Encapsulate arguments to simplify method calls
            var moveInfo = new LearnInfo(pkm) { Source = source };
            // Check moves going backwards, marking the move valid in the most current generation when it can be learned
            int[] generations = GetGenMovesCheckOrder(pkm);
            if (pkm.Format <= 2)
                generations = generations.Where(z => z < info.EncounterMoves.LevelUpMoves.Length).ToArray();

            int lastgen = generations.LastOrDefault();
            foreach (var gen in generations)
            {
                ParseMovesByGeneration(pkm, res, gen, info, moveInfo, lastgen);
                if (AllParsed())
                    return res;
            }

            if (pkm.Species == 292 && info.Generation <= 4)
                ParseShedinjaEvolveMoves(pkm, res, source.CurrentMoves);

            for (int m = 0; m < 4; m++)
            {
                if (res[m] == null)
                    res[m] = new CheckMoveResult(MoveSource.Unknown, info.Generation, Severity.Invalid, LMoveSourceInvalid, CheckIdentifier.Move);
            }
            return res;
        }

        private static void ParseMovesByGeneration(PKM pkm, CheckMoveResult[] res, int gen, LegalInfo info, LearnInfo learnInfo, int last)
        {
            GetHMCompatibility(pkm, res, gen, learnInfo.Source.CurrentMoves, out bool[] HMLearned, out bool KnowDefogWhirlpool);
            ParseMovesByGeneration(pkm, res, gen, info, learnInfo);

            if (gen == last)
                ParseMovesByGenerationLast(pkm, res, gen, learnInfo);

            switch (gen)
            {
                case 3:
                case 4:
                    if (pkm.Format > gen)
                        FlagIncompatibleTransferHMs(res, learnInfo.Source.CurrentMoves, gen, HMLearned, KnowDefogWhirlpool);
                    break;

                case 1:
                case 2:
                    ParseMovesByGeneration12(pkm, res, learnInfo.Source.CurrentMoves, gen, info, learnInfo);
                    break;
            }

            // Pokemon that evolved by leveling up while learning a specific move
            // This pokemon could only have 3 moves from preevolutions that are not the move used to evolved
            // including special and eggs moves before realearn generations
            if (Legal.SpeciesEvolutionWithMove.Contains(pkm.Species))
                ParseEvolutionLevelupMove(pkm, res, learnInfo.Source.CurrentMoves, info);
        }

        private static void ParseMovesByGeneration(PKM pkm, IList<CheckMoveResult> res, int gen, LegalInfo info, LearnInfo learnInfo)
        {
            var moves = learnInfo.Source.CurrentMoves;
            bool native = gen == pkm.Format;
            for (int m = 0; m < 4; m++)
            {
                if (IsCheckValid(res[m])) // already validated with another generation
                    continue;
                int move = moves[m];
                if (move == 0)
                    continue;

                if (gen <= 2)
                {
                    if (gen == 2 && !native && move > Legal.MaxMoveID_1 && pkm.VC1)
                    {
                        res[m] = new CheckMoveResult(MoveSource.Unknown, gen, Severity.Invalid, LMoveSourceInvalid, CheckIdentifier.Move);
                        continue;
                    }
                    if (gen == 2 && learnInfo.Source.EggMoveSource.Contains(move))
                        res[m] = new CheckMoveResult(MoveSource.EggMove, gen, Severity.Valid, LMoveSourceEgg, CheckIdentifier.Move);
                    else if (learnInfo.Source.Base.Contains(move))
                        res[m] = new CheckMoveResult(MoveSource.Initial, gen, Severity.Valid, native ? LMoveSourceDefault : string.Format(LMoveFDefault_0, gen), CheckIdentifier.Move);
                }
                if (info.EncounterMoves.LevelUpMoves[gen].Contains(move))
                    res[m] = new CheckMoveResult(MoveSource.LevelUp, gen, Severity.Valid, native ? LMoveSourceLevelUp : string.Format(LMoveFLevelUp_0, gen), CheckIdentifier.Move);
                else if (info.EncounterMoves.TMHMMoves[gen].Contains(move))
                    res[m] = new CheckMoveResult(MoveSource.TMHM, gen, Severity.Valid, native ? LMoveSourceTMHM : string.Format(LMoveFTMHM_0, gen), CheckIdentifier.Move);
                else if (info.EncounterMoves.TutorMoves[gen].Contains(move))
                    res[m] = new CheckMoveResult(MoveSource.Tutor, gen, Severity.Valid, native ? LMoveSourceTutor : string.Format(LMoveFTutor_0, gen), CheckIdentifier.Move);
                else if (gen == info.Generation && learnInfo.Source.SpecialSource.Contains(move))
                    res[m] = new CheckMoveResult(MoveSource.Special, gen, Severity.Valid, LMoveSourceSpecial, CheckIdentifier.Move);

                if (res[m] == null || gen >= 3)
                    continue;

                if (res[m].Valid && gen == 2 && learnInfo.Source.NonTradeBackLevelUpMoves.Contains(m))
                    learnInfo.Gen2PreevoMoves.Add(m);
                if (res[m].Valid && gen == 1)
                {
                    learnInfo.Gen1Moves.Add(m);
                    if (learnInfo.Gen2PreevoMoves.Count != 0)
                        learnInfo.MixedGen12NonTradeback = true;
                }

                if (res[m].Valid && gen <= 2 && pkm.TradebackStatus == TradebackType.Any && info.Generation != gen)
                    pkm.TradebackStatus = TradebackType.WasTradeback;
            }
        }

        private static void ParseMovesByGeneration12(PKM pkm, CheckMoveResult[] res, int[] moves, int gen, LegalInfo info, LearnInfo learnInfo)
        {
            // Mark the gen 1 exclusive moves as illegal because the pokemon also have Non tradeback egg moves.
            if (learnInfo.MixedGen12NonTradeback)
            {
                foreach (int m in learnInfo.Gen1Moves)
                    res[m] = new CheckMoveResult(res[m], Severity.Invalid, LG1MoveExclusive, CheckIdentifier.Move);

                foreach (int m in learnInfo.Gen2PreevoMoves)
                    res[m] = new CheckMoveResult(res[m], Severity.Invalid, LG1TradebackPreEvoMove, CheckIdentifier.Move);
            }

            if (gen == 1 && pkm.Format == 1 && pkm.Gen1_NotTradeback)
            {
                ParseRedYellowIncompatibleMoves(pkm, res, moves);
                ParseEvolutionsIncompatibleMoves(pkm, res, moves, info.EncounterMoves.TMHMMoves[1]);
            }
        }

        private static void ParseMovesByGenerationLast(PKM pkm, CheckMoveResult[] res, int gen, LearnInfo learnInfo)
        {
            ParseEggMovesInherited(pkm, res, gen, learnInfo);
            ParseEggMoves(pkm, res, gen, learnInfo);
            ParseEggMovesRemaining(pkm, res, learnInfo);
        }

        private static void ParseEggMovesInherited(PKM pkm, CheckMoveResult[] res, int gen, LearnInfo learnInfo)
        {
            var moves = learnInfo.Source.CurrentMoves;
            // Check higher-level moves after all the moves but just before egg moves to differentiate it from normal level up moves
            // Also check if the base egg moves is a non tradeback move
            for (int m = 0; m < 4; m++)
            {
                if (IsCheckValid(res[m])) // already validated
                    continue;
                if (moves[m] == 0)
                    continue;
                if (!learnInfo.Source.EggLevelUpSource.Contains(moves[m])) // Check if contains level-up egg moves from parents
                    continue;

                if (learnInfo.IsGen2Pkm && learnInfo.Gen1Moves.Count != 0 && moves[m] > Legal.MaxMoveID_1)
                {
                    res[m] = new CheckMoveResult(MoveSource.InheritLevelUp, gen, Severity.Invalid, LG1MoveTradeback, CheckIdentifier.Move);
                    learnInfo.MixedGen12NonTradeback = true;
                }
                else
                {
                    res[m] = new CheckMoveResult(MoveSource.InheritLevelUp, gen, Severity.Valid, LMoveEggLevelUp, CheckIdentifier.Move);
                }
                learnInfo.LevelUpEggMoves.Add(m);
                if (pkm.TradebackStatus == TradebackType.Any && pkm.GenNumber == 1)
                    pkm.TradebackStatus = TradebackType.WasTradeback;
            }
        }

        private static void ParseEggMoves(PKM pkm, CheckMoveResult[] res, int gen, LearnInfo learnInfo)
        {
            var moves = learnInfo.Source.CurrentMoves;
            // Check egg moves after all the generations and all the moves, every move that can't be learned in another source should have preference
            // the moves that can only be learned from egg moves should in the future check if the move combinations can be breed in gens 2 to 5
            for (int m = 0; m < 4; m++)
            {
                if (IsCheckValid(res[m]))
                    continue;
                if (moves[m] == 0)
                    continue;

                bool wasEggMove = learnInfo.Source.EggMoveSource.Contains(moves[m]);
                if (wasEggMove)
                {
                    // To learn exclusive generation 1 moves the pokemon was tradeback, but it can't be trade to generation 1
                    // without removing moves above MaxMoveID_1, egg moves above MaxMoveID_1 and gen 1 moves are incompatible
                    if (learnInfo.IsGen2Pkm && learnInfo.Gen1Moves.Count != 0 && moves[m] > Legal.MaxMoveID_1)
                    {
                        res[m] = new CheckMoveResult(MoveSource.EggMove, gen, Severity.Invalid, LG1MoveTradeback, CheckIdentifier.Move);
                        learnInfo.MixedGen12NonTradeback = true;
                    }
                    else
                    {
                        res[m] = new CheckMoveResult(MoveSource.EggMove, gen, Severity.Valid, LMoveSourceEgg, CheckIdentifier.Move) { Flag = true };
                    }

                    learnInfo.EggMovesLearned.Add(m);
                    if (pkm.TradebackStatus == TradebackType.Any && pkm.GenNumber == 1)
                        pkm.TradebackStatus = TradebackType.WasTradeback;
                }
                if (!learnInfo.Source.EggEventSource.Contains(moves[m]))
                    continue;

                if (!wasEggMove)
                {
                    if (learnInfo.IsGen2Pkm && learnInfo.Gen1Moves.Count != 0 && moves[m] > Legal.MaxMoveID_1)
                    {
                        res[m] = new CheckMoveResult(MoveSource.SpecialEgg, gen, Severity.Invalid, LG1MoveTradeback, CheckIdentifier.Move);
                        learnInfo.MixedGen12NonTradeback = true;
                    }
                    else
                    {
                        res[m] = new CheckMoveResult(MoveSource.SpecialEgg, gen, Severity.Valid, LMoveSourceEggEvent, CheckIdentifier.Move);
                    }
                }
                if (pkm.TradebackStatus == TradebackType.Any && pkm.GenNumber == 1)
                    pkm.TradebackStatus = TradebackType.WasTradeback;
                learnInfo.EventEggMoves.Add(m);
            }
        }

        private static void ParseEggMovesRemaining(PKM pkm, CheckMoveResult[] res, LearnInfo learnInfo)
        {
            // A pokemon could have normal egg moves and regular egg moves
            // Only if all regular egg moves are event egg moves or all event egg moves are regular egg moves
            var RegularEggMovesLearned = learnInfo.EggMovesLearned.Union(learnInfo.LevelUpEggMoves).ToList();
            if (RegularEggMovesLearned.Count != 0 && learnInfo.EventEggMoves.Count != 0)
            {
                // Moves that are egg moves or event egg moves but not both
                var IncompatibleEggMoves = RegularEggMovesLearned.Except(learnInfo.EventEggMoves).Union(learnInfo.EventEggMoves.Except(RegularEggMovesLearned)).ToList();
                if (IncompatibleEggMoves.Count == 0)
                    return;
                foreach (int m in IncompatibleEggMoves)
                {
                    if (learnInfo.EventEggMoves.Contains(m) && !learnInfo.EggMovesLearned.Contains(m))
                        res[m] = new CheckMoveResult(res[m], Severity.Invalid, LMoveEggIncompatibleEvent, CheckIdentifier.Move);
                    else if (!learnInfo.EventEggMoves.Contains(m) && learnInfo.EggMovesLearned.Contains(m))
                        res[m] = new CheckMoveResult(res[m], Severity.Invalid, LMoveEggIncompatible, CheckIdentifier.Move);
                    else if (!learnInfo.EventEggMoves.Contains(m) && learnInfo.LevelUpEggMoves.Contains(m))
                        res[m] = new CheckMoveResult(res[m], Severity.Invalid, LMoveEventEggLevelUp, CheckIdentifier.Move);
                }
            }
            // If there is no incompatibility with event egg check that there is no inherited move in gift eggs and event eggs
            else if (RegularEggMovesLearned.Count != 0 && (pkm.WasGiftEgg || pkm.WasEventEgg))
            {
                foreach (int m in RegularEggMovesLearned)
                {
                    if (learnInfo.EggMovesLearned.Contains(m))
                        res[m] = new CheckMoveResult(res[m], Severity.Invalid, pkm.WasGiftEgg ? LMoveEggMoveGift : LMoveEggInvalidEvent, CheckIdentifier.Move);
                    else if (learnInfo.LevelUpEggMoves.Contains(m))
                        res[m] = new CheckMoveResult(res[m], Severity.Invalid, pkm.WasGiftEgg ? LMoveEggInvalidEventLevelUpGift : LMoveEggInvalidEventLevelUp, CheckIdentifier.Move);
                }
            }
        }

        private static void ParseRedYellowIncompatibleMoves(PKM pkm, IList<CheckMoveResult> res, int[] moves)
        {
            var incompatible = GetIncompatibleRBYMoves(pkm, moves);
            if (incompatible.Count == 0)
                return;
            for (int m = 0; m < 4; m++)
            {
                if (incompatible.Contains(moves[m]))
                    res[m] = new CheckMoveResult(res[m], Severity.Invalid, LG1MoveLearnSameLevel, CheckIdentifier.Move);
            }
        }

        private static List<int> GetIncompatibleRBYMoves(PKM pkm, int[] moves)
        {
            // Check moves that are learned at the same level in red/blue and yellow, these are illegal because there is no move reminder
            // There are only two incompatibilites; there is no illegal combination in generation 2+.
            var incompatible = new List<int>(3);

            switch (pkm.Species)
            {
                // Vaporeon in Yellow learns Mist and Haze at level 42, Mist can only be learned if it leveled up in the daycare
                // Vaporeon in Red/Blue learns Acid Armor at level 42 and level 47 in Yellow
                case 134 when pkm.CurrentLevel < 47 && moves.Contains(151):
                    if (moves.Contains(54))
                        incompatible.Add(54);
                    if (moves.Contains(114))
                        incompatible.Add(114);
                    if (incompatible.Count != 0)
                        incompatible.Add(151);
                    break;

                // Flareon in Yellow learns Smog at level 42
                // Flareon in Red Blue learns Leer at level 42 and level 47 in Yellow
                case 136 when pkm.CurrentLevel < 47 && moves.Contains(43) && moves.Contains(123):
                    incompatible.Add(43);
                    incompatible.Add(123);
                    break;
            }

            return incompatible;
        }

        private static void ParseEvolutionsIncompatibleMoves(PKM pkm, IList<CheckMoveResult> res, int[] moves, List<int> tmhm)
        {
            var species = SpeciesStrings;
            var currentspecies = species[pkm.Species];
            var previousspecies = string.Empty;
            var incompatible_previous = new List<int>();
            var incompatible_current = new List<int>();
            if (pkm.Species == 34 && moves.Contains(31) && moves.Contains(37))
            {
                // Nidoking learns Thrash at level 23
                // Nidorino learns Fury Attack at level 36, Nidoran♂ at level 30
                // Other moves are either learned by Nidoran♂ up to level 23 or by TM
                incompatible_current.Add(31);
                incompatible_previous.Add(37);
                previousspecies = species[33];
            }
            if (pkm.Species == 103 && moves.Contains(23) && moves.Any(m => Legal.G1Exeggcute_IncompatibleMoves.Contains(moves[m])))
            {
                // Exeggutor learns stomp at level 28
                // Exeggcute learns Stun Spore at 32, PoisonPowder at 37 and Sleep Powder at 48
                incompatible_current.Add(23);
                incompatible_previous.AddRange(Legal.G1Exeggcute_IncompatibleMoves);
                previousspecies = species[103];
            }
            if (134 <= pkm.Species && pkm.Species <= 136)
            {
                previousspecies = species[133];
                var ExclusiveMoves = Legal.GetExclusiveMovesG1(133, pkm.Species, tmhm, moves);
                var EeveeLevels = Legal.GetMinLevelLearnMoveG1(133, ExclusiveMoves[0]);
                var EvoLevels = Legal.GetMaxLevelLearnMoveG1(pkm.Species, ExclusiveMoves[1]);

                for (int i = 0; i < ExclusiveMoves[0].Count; i++)
                {
                    // There is a evolution move with a lower level that current eevee move
                    if (EvoLevels.Any(ev => ev < EeveeLevels[i]))
                        incompatible_previous.Add(ExclusiveMoves[0][i]);
                }
                for (int i = 0; i < ExclusiveMoves[1].Count; i++)
                {
                    // There is a eevee move with a greather level that current evolution move
                    if (EeveeLevels.Any(ev => ev > EvoLevels[i]))
                        incompatible_current.Add(ExclusiveMoves[1][i]);
                }
            }

            for (int m = 0; m < 4; m++)
            {
                if (incompatible_current.Contains(moves[m]))
                    res[m] = new CheckMoveResult(res[m], Severity.Invalid, string.Format(LMoveEvoFLower, currentspecies, previousspecies), CheckIdentifier.Move);
                if (incompatible_previous.Contains(moves[m]))
                    res[m] = new CheckMoveResult(res[m], Severity.Invalid, string.Format(LMoveEvoFHigher, currentspecies, previousspecies), CheckIdentifier.Move);
            }
        }

        private static void ParseShedinjaEvolveMoves(PKM pkm, IList<CheckMoveResult> res, int[] moves)
        {
            var ShedinjaEvoMovesLearned = new List<int>();
            for (int gen = Math.Min(pkm.Format, 4); gen >= 3; gen--)
            {
                var ninjaskMoves = Legal.GetShedinjaEvolveMoves(pkm, gen);
                bool native = gen == pkm.Format;
                for (int m = 0; m < 4; m++)
                {
                    if (IsCheckValid(res[m])) // already validated
                        continue;

                    if (!ninjaskMoves.Contains(moves[m]))
                        continue;

                    res[m] = new CheckMoveResult(MoveSource.ShedinjaEvo, gen, Severity.Valid, native ? LMoveNincadaEvo : string.Format(LMoveNincadaEvoF_0, gen), CheckIdentifier.Move);
                    ShedinjaEvoMovesLearned.Add(m);
                }
            }

            if (ShedinjaEvoMovesLearned.Count == 0)
                return;
            if (ShedinjaEvoMovesLearned.Count > 1)
            {
                // Can't have more than one Ninjask exclusive move on Shedinja
                foreach (int m in ShedinjaEvoMovesLearned)
                    res[m] = new CheckMoveResult(res[m], Severity.Invalid, LMoveNincada, CheckIdentifier.Move);
                return;
            }

            // Double check that the Ninjask move level isn't less than any Nincada move level
            int move = ShedinjaEvoMovesLearned[0];
            int g = res[move].Generation;
            int levelJ = Legal.GetShedinjaMoveLevel(291, moves[move], g);

            for (int m = 0; m < 4; m++)
            {
                if (m != move)
                    continue;
                if (res[m].Source != MoveSource.LevelUp)
                    continue;
                int levelS = Legal.GetShedinjaMoveLevel(292, moves[m], res[m].Generation);
                if (levelS > 0)
                    continue;

                int levelN = Legal.GetShedinjaMoveLevel(290, moves[m], res[m].Generation);
                if (levelN > levelJ)
                    res[m] = new CheckMoveResult(res[m], Severity.Invalid, string.Format(LMoveEvoFHigher, SpeciesStrings[290], SpeciesStrings[291]), CheckIdentifier.Move);
            }
        }

        private static void ParseEvolutionLevelupMove(PKM pkm, IList<CheckMoveResult> res, int[] moves, LegalInfo info)
        {
            // Ignore if there is an invalid move or an empty move, this validation is only for 4 non-empty moves that are all valid, but invalid as a 4 combination
            // Ignore Mr. Mime and Sudowodoo from generations 1 to 3, they cant be evolved from Bonsly or Munchlax
            // Ignore if encounter species is the evolution species, the pokemon was not evolved by the player
            if (info.EncounterMatch.Species == pkm.Species)
                return;
            if (!res.All(r => r?.Valid ?? false) || moves.Any(m => m == 0) || (Legal.BabyEvolutionWithMove.Contains(pkm.Species) && info.Generation <= 3))
                return;

            var ValidMoves = Legal.GetValidPostEvolutionMoves(pkm, pkm.Species, info.EvoChainsAllGens, GameVersion.Any);

            // Add the evolution moves to valid moves in case some of these moves could not be learned after evolving
            switch (pkm.Species)
            {
                case 122: // Mr. Mime (Mime Jr with Mimic)
                case 185: // Sudowoodo (Bonsly with Mimic)
                    ValidMoves.Add(102);
                    break;
                case 424: // Ambipom (Aipom with Double Hit)
                    ValidMoves.Add(458);
                    break;
                case 463: // Lickilicky (Lickitung with Rollout)
                    ValidMoves.Add(205);
                    break;
                case 465: // Tangrowth (Tangela with Ancient Power)
                case 469: // Yanmega (Yamma with Ancient Power)
                case 473: // Mamoswine (Piloswine with Ancient Power)
                    ValidMoves.Add(246);
                    break;
                case 700: // Sylveon (Eevee with Fairy Move)
                    // Add every fairy moves without cheking if eevee learn it or not, pokemon moves are determined legal before this function
                    ValidMoves.AddRange(Legal.FairyMoves);
                    break;
                case 763: // Tsareena (Steenee with Stomp)
                    ValidMoves.Add(023);
                    break;
            }

            if (moves.Any(m => ValidMoves.Contains(m)))
                return;

            for (int m = 0; m < 4; m++)
                res[m] = new CheckMoveResult(res[m], Severity.Invalid, string.Format(LMoveEvoFCombination_0, SpeciesStrings[pkm.Species]), CheckIdentifier.Move);
        }

        private static void GetHMCompatibility(PKM pkm, IReadOnlyList<CheckResult> res, int gen, IReadOnlyList<int> moves, out bool[] HMLearned, out bool KnowDefogWhirlpool)
        {
            HMLearned = new bool[4];
            // Check if pokemon knows HM moves from generation 3 and 4 but are not valid yet, that means it cant learn the HMs in future generations
            if (gen == 4 && pkm.Format > 4)
            {
                IsHMSource(HMLearned, Legal.HM_4_RemovePokeTransfer);
                KnowDefogWhirlpool = moves.Where((m, i) => IsDefogWhirl(m) && IsCheckInvalid(res[i])).Count() == 2;
                return;
            }
            KnowDefogWhirlpool = false;
            if (gen == 3 && pkm.Format > 3)
                IsHMSource(HMLearned, Legal.HM_3);

            void IsHMSource(IList<bool> flags, ICollection<int> source)
            {
                for (int i = 0; i < 4; i++)
                    flags[i] = IsCheckInvalid(res[i]) && source.Contains(moves[i]);
            }
        }

        private static bool IsDefogWhirl(int move) => move == 250 || move == 432;
        private static bool IsCheckInvalid(CheckResult chk) => !(chk?.Valid ?? false);
        private static bool IsCheckValid(CheckResult chk) => chk?.Valid ?? false;

        private static void FlagIncompatibleTransferHMs(CheckMoveResult[] res, int[] moves, int gen, bool[] HMLearned, bool KnowDefogWhirlpool)
        {
            // After all the moves from the generations 3 and 4,
            // including egg moves if is the origin generation because some hidden moves are also special egg moves in gen 3
            // Check if the marked hidden moves that were invalid at the start are now marked as valid, that means
            // the hidden move was learned in gen 3 or 4 but was not removed when transfer to 4 or 5
            if (KnowDefogWhirlpool)
            {
                int invalidCount = moves.Where((m, i) => IsDefogWhirl(m) && IsCheckValid(res[i])).Count();
                if (invalidCount == 2) // can't know both at the same time
                {
                    for (int i = 0; i < 4; i++) // flag both moves
                    {
                        if (IsDefogWhirl(moves[i]))
                            res[i] = new CheckMoveResult(res[i], Severity.Invalid, LTransferMoveG4HM, CheckIdentifier.Move);
                    }
                }
            }

            // Flag moves that are only legal when learned from a past-gen HM source
            for (int i = 0; i < HMLearned.Length; i++)
            {
                if (HMLearned[i] && IsCheckValid(res[i]))
                    res[i] = new CheckMoveResult(res[i], Severity.Invalid, string.Format(LTransferMoveHM, gen, gen + 1), CheckIdentifier.Move);
            }
        }

        /* Similar to verifyRelearnEgg but in pre relearn generation is the moves what should match the expected order but only if the pokemon is inside an egg */
        private static CheckMoveResult[] VerifyPreRelearnEggBase(PKM pkm, int[] Moves, EggInfoSource infoset)
        {
            CheckMoveResult[] res = new CheckMoveResult[4];
            var gen = pkm.GenNumber;
            // Obtain level1 moves
            var reqBase = GetRequiredBaseMoveCount(Moves, infoset);

            var sb = new System.Text.StringBuilder();
            // Check if the required amount of Base Egg Moves are present.
            for (int i = 0; i < reqBase; i++)
            {
                if (infoset.Base.Contains(Moves[i]))
                {
                    res[i] = new CheckMoveResult(MoveSource.Initial, gen, Severity.Valid, LMoveRelearnEgg, CheckIdentifier.Move);
                    continue;
                }

                // mark remaining base egg moves missing
                for (int z = i; z < reqBase; z++)
                    res[z] = new CheckMoveResult(MoveSource.Initial, gen, Severity.Invalid, LMoveRelearnEggMissing, CheckIdentifier.Move);

                // provide the list of suggested base moves for the last required slot
                sb.Append(string.Join(", ", GetMoveNames(infoset.Base)));
                break;
            }

            int moveoffset = reqBase;
            int endSpecial = moveoffset + infoset.Special.Count;
            // Check also if the required amount of Special Egg Moves are present, ir are after base moves
            for (int i = moveoffset; i < endSpecial; i++)
            {
                if (infoset.Special.Contains(Moves[i]))
                {
                    res[i] = new CheckMoveResult(MoveSource.SpecialEgg, gen, Severity.Valid, LMoveSourceEggEvent, CheckIdentifier.Move);
                    continue;
                }

                // Not in special moves, mark remaining special egg moves missing
                for (int z = i; z < endSpecial; z++)
                    res[z] = new CheckMoveResult(MoveSource.SpecialEgg, gen, Severity.Invalid, LMoveEggMissing, CheckIdentifier.Move);

                // provide the list of suggested base moves and species moves for the last required slot
                if (sb.Length == 0)
                    sb.Append(string.Join(", ", GetMoveNames(infoset.Base)));
                sb.Append(", ");
                sb.Append(string.Join(", ", GetMoveNames(infoset.Special)));
                break;
            }

            if (sb.Length != 0)
                res[reqBase > 0 ? reqBase - 1 : 0].Comment = string.Format(Environment.NewLine + LMoveFExpect_0, sb.ToString());

            // Inherited moves appear after the required base moves.
            var AllowInheritedSeverity = infoset.AllowInherited ? Severity.Valid : Severity.Invalid;
            for (int i = reqBase + infoset.Special.Count; i < 4; i++)
            {
                if (Moves[i] == 0) // empty
                    res[i] = new CheckMoveResult(MoveSource.None, gen, Severity.Valid, LMoveSourceEmpty, CheckIdentifier.Move);
                else if (infoset.Egg.Contains(Moves[i])) // inherited egg move
                    res[i] = new CheckMoveResult(MoveSource.EggMove, gen, AllowInheritedSeverity, infoset.AllowInherited ? LMoveEggInherited : LMoveEggInvalidEvent, CheckIdentifier.Move);
                else if (infoset.LevelUp.Contains(Moves[i])) // inherited lvl moves
                    res[i] = new CheckMoveResult(MoveSource.InheritLevelUp, gen, AllowInheritedSeverity, infoset.AllowInherited ? LMoveEggLevelUp : LMoveEggInvalidEventLevelUp, CheckIdentifier.Move);
                else if (infoset.TMHM.Contains(Moves[i])) // inherited TMHM moves
                    res[i] = new CheckMoveResult(MoveSource.TMHM, gen, AllowInheritedSeverity, infoset.AllowInherited ? LMoveEggTMHM : LMoveEggInvalidEventTMHM, CheckIdentifier.Move);
                else if (infoset.Tutor.Contains(Moves[i])) // inherited tutor moves
                    res[i] = new CheckMoveResult(MoveSource.Tutor, gen, AllowInheritedSeverity, infoset.AllowInherited ? LMoveEggInheritedTutor : LMoveEggInvalidEventTutor, CheckIdentifier.Move);
                else // not inheritable, flag
                    res[i] = new CheckMoveResult(MoveSource.Unknown, gen, Severity.Invalid, LMoveEggInvalid, CheckIdentifier.Move);
            }

            return res;
        }

        private static int GetRequiredBaseMoveCount(int[] Moves, EggInfoSource infoset)
        {
            int baseCt = infoset.Base.Count;
            if (baseCt > 4) baseCt = 4;

            // Obtain Inherited moves
            var inherited = Moves.Where(m => m != 0 && infoset.IsInherited(m)).ToList();
            int inheritCt = inherited.Count;

            // Get required amount of base moves
            int unique = infoset.Base.Union(inherited).Count();
            int reqBase = inheritCt == 4 || baseCt + inheritCt > 4 ? 4 - inheritCt : baseCt;
            if (Moves.Count(m => m != 0) < Math.Min(4, infoset.Base.Count))
                reqBase = Math.Min(4, unique);
            return reqBase;
        }

        private static void VerifyNoEmptyDuplicates(int[] Moves, CheckMoveResult[] res)
        {
            bool emptySlot = false;
            for (int i = 0; i < 4; i++)
            {
                if (Moves[i] == 0)
                    emptySlot = true;
                else if (emptySlot)
                    res[i] = new CheckMoveResult(res[i], Severity.Invalid, LMoveSourceEmpty, res[i].Identifier);
                else if (Moves.Count(m => m == Moves[i]) > 1)
                    res[i] = new CheckMoveResult(res[i], Severity.Invalid, LMoveSourceDuplicate, res[i].Identifier);
            }
        }

        private static void UpdateGen1LevelUpMoves(PKM pkm, ValidEncounterMoves EncounterMoves, int defaultLvlG1, int generation, LegalInfo info)
        {
            if (generation >= 3)
                return;
            var lvlG1 = info.EncounterMatch?.LevelMin + 1 ?? 6;
            if (lvlG1 == defaultLvlG1)
                return;
            EncounterMoves.LevelUpMoves[1] = Legal.GetValidMoves(pkm, info.EvoChainsAllGens[1], generation: 1, minLvLG1: lvlG1, LVL: true, Tutor: false, Machine: false, MoveReminder: false).ToList();
        }

        private static void UpdateGen2LevelUpMoves(PKM pkm, ValidEncounterMoves EncounterMoves, int defaultLvlG2, int generation, LegalInfo info)
        {
            if (generation >= 3)
                return;
            var lvlG2 = info.EncounterMatch?.LevelMin + 1 ?? 6;
            if (lvlG2 == defaultLvlG2)
                return;
            EncounterMoves.LevelUpMoves[2] = Legal.GetValidMoves(pkm, info.EvoChainsAllGens[2], generation: 2, minLvLG2: defaultLvlG2, LVL: true, Tutor: false, Machine: false, MoveReminder: false).ToList();
        }

        public static int[] GetGenMovesCheckOrder(PKM pkm)
        {
            if (pkm.Format < 3)
                return GetGenMovesCheckOrderGB(pkm, pkm.Format);
            if (pkm.VC)
                return GetGenMovesOrderVC(pkm);

            return GetGenMovesOrder(pkm.Format, pkm.GenNumber);
        }

        private static int[] GetGenMovesOrderVC(PKM pkm)
        {
            // VC case: check transfer games in reverse order (8, 7..) then past games.
            int[] xfer = GetGenMovesOrder(pkm.Format, 7);
            int[] past = GetGenMovesCheckOrderGB(pkm, pkm.GenNumber);
            int end = xfer.Length;
            Array.Resize(ref xfer, xfer.Length + past.Length);
            past.CopyTo(xfer, end);
            return xfer;
        }

        private static readonly int[] G2 = {2};
        private static readonly int[] G12 = {1, 2};

        private static int[] GetGenMovesCheckOrderGB(PKM pkm, int originalGeneration)
        {
            if (originalGeneration == 2)
                return pkm.Korean ? G2 : G12;
            return new[] {1, 2}; // RBY
        }

        private static int[] GetGenMovesOrder(int start, int end)
        {
            if (end < 0)
                return Array.Empty<int>();
            if (start <= end)
                return new[] {start};
            var order = new int[start - end + 1];
            for (int i = 0; i < order.Length; i++)
                order[i] = start - i;
            return order;
        }
    }
}
