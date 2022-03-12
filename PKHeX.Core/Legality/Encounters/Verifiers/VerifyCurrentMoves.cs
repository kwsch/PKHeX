using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.ParseSettings;

using static PKHeX.Core.MoveSource;
using static PKHeX.Core.Severity;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic to verify the current <see cref="PKM.Moves"/>.
    /// </summary>
    public static class VerifyCurrentMoves
    {
        /// <summary>
        /// Verifies the current moves of the <see cref="pkm"/> data based on the provided <see cref="info"/>.
        /// </summary>
        /// <param name="pkm">Data to check</param>
        /// <param name="info">Encounter conditions and legality info</param>
        /// <returns>Validity of the <see cref="PKM.Moves"/></returns>
        public static void VerifyMoves(PKM pkm, LegalInfo info)
        {
            var parse = info.Moves;
            Array.ForEach(parse, p => p.Reset());
            var currentMoves = pkm.Moves;
            ParseMovesForEncounters(pkm, parse, info, currentMoves);

            // Duplicate Moves Check
            VerifyNoEmptyDuplicates(currentMoves, parse);
            if (currentMoves[0] == 0) // Can't have an empty move slot for the first move.
                parse[0].FlagIllegal(LMoveSourceEmpty, CurrentMove);
        }

        private static void ParseMovesForEncounters(PKM pkm, CheckMoveResult[] parse, LegalInfo info, int[] currentMoves)
        {
            if (pkm.Species == (int)Species.Smeargle) // special handling for Smeargle
            {
                ParseMovesForSmeargle(pkm, parse, currentMoves, info); // Smeargle can have any moves except a few
                return;
            }

            // gather valid moves for encounter species
            info.EncounterMoves = new ValidEncounterMoves(pkm, info.EncounterMatch, info.EvoChainsAllGens);

            if (info.Generation >= 6)
                ParseMoves3DS(pkm, parse, currentMoves, info);
            else
                ParseMovesPre3DS(pkm, parse, currentMoves, info);
        }

        private static void ParseMovesForSmeargle(PKM pkm, CheckMoveResult[] parse, int[] currentMoves, LegalInfo info)
        {
            if (!pkm.IsEgg)
            {
                ParseMovesSketch(pkm, parse, currentMoves);
                return;
            }

            // can only know sketch as egg
            var levelup = new int[info.EvoChainsAllGens.Length][];
            levelup[pkm.Format] = new[] { (int)Move.Sketch };
            info.EncounterMoves = new ValidEncounterMoves(levelup);
            var source = new MoveParseSource { CurrentMoves = currentMoves };
            ParseMoves(pkm, source, info, parse);
        }

        private static void ParseMovesWasEggPreRelearn(PKM pkm, CheckMoveResult[] parse, IReadOnlyList<int> currentMoves, LegalInfo info, EncounterEgg e)
        {
            // Level up moves could not be inherited if Ditto is parent,
            // that means genderless species and male only species (except Nidoran-M and Volbeat; they breed with Nidoran-F and Illumise) could not have level up moves as an egg
            var pi = pkm.PersonalInfo;
            var AllowLevelUp = !pi.Genderless && !(pi.OnlyMale && Breeding.MixedGenderBreeding.Contains(e.Species));
            int BaseLevel = AllowLevelUp ? 100 : e.LevelMin;
            var LevelUp = MoveList.GetBaseEggMoves(pkm, e.Species, e.Form, e.Version, BaseLevel);

            var TradebackPreevo = pkm.Format == 2 && e.Species > 151;
            var NonTradebackLvlMoves = TradebackPreevo
                ? MoveList.GetExclusivePreEvolutionMoves(pkm, e.Species, info.EvoChainsAllGens[2], 2, e.Version).Where(m => m > Legal.MaxMoveID_1).ToArray()
                : Array.Empty<int>();

            var Egg = MoveEgg.GetEggMoves(pkm.PersonalInfo, e.Species, e.Form, e.Version, e.Generation);
            if (info.Generation < 3 && pkm.Format >= 7 && pkm.VC1)
                Egg = Array.FindAll(Egg, m => m <= Legal.MaxMoveID_1);

            var specialMoves = e.CanHaveVoltTackle ? new[] { (int)Move.VoltTackle } : Array.Empty<int>(); // Volt Tackle for bred Pichu line

            var source = new MoveParseSource
            {
                CurrentMoves = currentMoves,
                SpecialSource = specialMoves,
                NonTradeBackLevelUpMoves = NonTradebackLvlMoves,

                EggLevelUpSource = LevelUp,
                EggMoveSource = Egg,
            };
            ParseMoves(pkm, source, info, parse);
        }

        private static void ParseMovesSketch(PKM pkm, CheckMoveResult[] parse, ReadOnlySpan<int> currentMoves)
        {
            for (int i = parse.Length - 1; i >= 0; i--)
            {
                var r = parse[i];
                var move = currentMoves[i];
                if (move == 0)
                    r.Set(None, pkm.Format, Valid, LMoveSourceEmpty, CurrentMove);
                else if (Legal.IsValidSketch(move, pkm.Format))
                    r.Set(Sketch, pkm.Format, Valid, L_AValid, CurrentMove);
                else
                    r.Set(Unknown, pkm.Format, Invalid, LMoveSourceInvalidSketch, CurrentMove);
            }
        }

        private static void ParseMoves3DS(PKM pkm, CheckMoveResult[] parse, IReadOnlyList<int> currentMoves, LegalInfo info)
        {
            info.EncounterMoves.Relearn = info.Generation >= 6 ? pkm.RelearnMoves : Array.Empty<int>();
            if (info.EncounterMatch is IMoveset)
                ParseMovesSpecialMoveset(pkm, currentMoves, info, parse);
            else
                ParseMovesRelearn(pkm, currentMoves, info, parse);
        }

        private static void ParseMovesPre3DS(PKM pkm, CheckMoveResult[] parse, int[] currentMoves, LegalInfo info)
        {
            if (info.EncounterMatch is EncounterEgg e)
            {
                if (pkm.IsEgg)
                    VerifyRelearnMoves.VerifyEggMoveset(e, parse, currentMoves, CurrentMove);
                else
                    ParseMovesWasEggPreRelearn(pkm, parse, currentMoves, info, e);
                return;
            }

            // Not all games have a re-learner. Initial moves may not fill out all 4 slots.
            int gen = info.EncounterMatch.Generation;
            if (gen == 1 || (gen == 2 && !AllowGen2MoveReminder(pkm)))
                ParseMovesGenGB(pkm, currentMoves, info, parse);

            ParseMovesSpecialMoveset(pkm, currentMoves, info, parse);
        }

        private static void ParseMovesGenGB(PKM pkm, IReadOnlyList<int> currentMoves, LegalInfo info, CheckMoveResult[] parse)
        {
            var enc = info.EncounterMatch;
            var evos = info.EvoChainsAllGens[enc.Generation];
            var level = evos.Count > 0 ? evos[^1].MinLevel : enc.LevelMin;
            var InitialMoves = Array.Empty<int>();
            var SpecialMoves = GetSpecialMoves(enc);
            var games = enc.Generation == 1 ? GBRestrictions.GetGen1Versions(enc) : GBRestrictions.GetGen2Versions(enc, pkm.Korean);
            foreach (var ver in games)
            {
                var VerInitialMoves = enc is IMoveset {Moves.Count: not 0 } x ? (int[])x.Moves : MoveLevelUp.GetEncounterMoves(enc.Species, 0, level, ver);
                if (VerInitialMoves.Intersect(InitialMoves).Count() == VerInitialMoves.Length)
                    return;

                var source = new MoveParseSource
                {
                    CurrentMoves = currentMoves,
                    SpecialSource = SpecialMoves,
                    Base = VerInitialMoves,
                };
                ParseMoves(pkm, source, info, parse);

                // Must have a minimum count of moves, depending on the tradeback state.
                if (pkm is PK1 pk1)
                {
                    int count = GBRestrictions.GetRequiredMoveCount(pk1, source.CurrentMoves, info, source.Base);
                    if (count == 1)
                        return;

                    // Reverse for loop and break instead of 0..count continue -- early-breaks for the vast majority of cases.
                    // We already flag for empty interstitial moveslots.
                    for (int m = count - 1; m >= 0; m--)
                    {
                        var move = source.CurrentMoves[m];
                        if (move != 0)
                            break;

                        // There are ways to skip level up moves by leveling up more than once.
                        // https://bulbapedia.bulbagarden.net/wiki/List_of_glitches_(Generation_I)#Level-up_learnset_skipping
                        // Evolution canceling also leads to incorrect assumptions in the above used method, so just indicate them as fishy in that case.
                        // Not leveled up? Not possible to be missing the move slot.
                        var severity = enc.LevelMin == pkm.CurrentLevel ? Invalid : Fishy;
                        parse[m].Set(None, pkm.Format, severity, LMoveSourceEmpty, CurrentMove);
                    }
                }
                if (Array.TrueForAll(parse, z => z.Valid))
                    return;
                InitialMoves = VerInitialMoves;
            }
        }

        private static void ParseMovesSpecialMoveset(PKM pkm, IReadOnlyList<int> currentMoves, LegalInfo info, CheckMoveResult[] parse)
        {
            var source = new MoveParseSource
            {
                CurrentMoves = currentMoves,
                SpecialSource = GetSpecialMoves(info.EncounterMatch),
            };
            ParseMoves(pkm, source, info, parse);
        }

        private static IReadOnlyList<int> GetSpecialMoves(IEncounterTemplate enc)
        {
            if (enc is IMoveset mg)
                return mg.Moves;
            return Array.Empty<int>();
        }

        private static void ParseMovesRelearn(PKM pkm, IReadOnlyList<int> currentMoves, LegalInfo info, CheckMoveResult[] parse)
        {
            var source = new MoveParseSource
            {
                CurrentMoves = currentMoves,
                SpecialSource = GetSpecialMoves(info.EncounterMatch),
            };

            if (info.EncounterMatch is EncounterEgg e)
                source.EggMoveSource = MoveEgg.GetEggMoves(pkm.PersonalInfo, e.Species, e.Form, e.Version, e.Generation);

            ParseMoves(pkm, source, info, parse);
            var relearn = pkm.RelearnMoves;
            for (int i = parse.Length - 1; i >= 0; i--)
            {
                var r = parse[i];
                if (!r.IsRelearn && !pkm.IsEgg)
                    continue;
                if (relearn.Contains(currentMoves[i]))
                    continue;

                r.FlagIllegal(string.Format(LMoveRelearnFMiss_0, r.Comment));
            }
        }

        private static void ParseMoves(PKM pkm, MoveParseSource source, LegalInfo info, CheckMoveResult[] parse)
        {
            // Special considerations!
            const int NoMinGeneration = 0;
            int minGeneration = NoMinGeneration;
            if (pkm is IBattleVersion {BattleVersion: not 0} v)
            {
                minGeneration = ((GameVersion) v.BattleVersion).GetGeneration();
                source.ResetSources();
            }

            // Check empty moves and relearn moves before generation specific moves
            for (int m = parse.Length - 1; m >= 0; m--)
            {
                var move = source.CurrentMoves[m];
                var r = parse[m];
                if (move == 0)
                    r.Set(None, pkm.Format, Valid, LMoveSourceEmpty, CurrentMove);
                else if (minGeneration == NoMinGeneration && info.EncounterMoves.Relearn.Contains(move))
                    r.Set(Relearn, info.Generation, Valid, LMoveSourceRelearn, CurrentMove);
            }

            if (Array.TrueForAll(parse, z => z.IsParsed))
                return;

            // Encapsulate arguments to simplify method calls
            var moveInfo = new LearnInfo(pkm, source);
            // Check moves going backwards, marking the move valid in the most current generation when it can be learned
            int[] generations = GenerationTraversal.GetVisitedGenerationOrder(pkm, info.EncounterOriginal.Generation);
            if (pkm.Format <= 2)
                generations = Array.FindAll(generations, z => z < info.EncounterMoves.LevelUpMoves.Length);
            if (minGeneration != NoMinGeneration)
                generations = Array.FindAll(generations, z => z >= minGeneration);

            if (generations.Length != 0)
            {
                int lastgen = generations[^1];
                foreach (var gen in generations)
                {
                    ParseMovesByGeneration(pkm, parse, gen, info, moveInfo, lastgen);
                    if (Array.TrueForAll(parse, z => z.IsParsed))
                        return;
                }
            }

            if (pkm.Species == (int)Species.Shedinja && info.Generation <= 4)
                ParseShedinjaEvolveMoves(pkm, parse, source.CurrentMoves, info.EvoChainsAllGens);

            foreach (var r in parse)
            {
                if (!r.IsParsed)
                    r.Set(Unknown, info.Generation, Invalid, LMoveSourceInvalid, CurrentMove);
            }
        }

        private static void ParseMovesByGeneration(PKM pkm, CheckMoveResult[] parse, int gen, LegalInfo info, LearnInfo learnInfo, int last)
        {
            Span<bool> HMLearned = stackalloc bool[parse.Length];
            bool KnowDefogWhirlpool = GetHMCompatibility(pkm, parse, gen, learnInfo.Source.CurrentMoves, HMLearned);
            ParseMovesByGeneration(pkm, parse, gen, info, learnInfo);

            if (gen == last)
                ParseMovesByGenerationLast(parse, learnInfo, info.EncounterMatch);

            switch (gen)
            {
                case 1 or 2:
                    ParseMovesByGeneration12(pkm, parse, learnInfo.Source.CurrentMoves, gen, info, learnInfo);
                    break;

                case 3 or 4:
                    if (pkm.Format > gen)
                        FlagIncompatibleTransferHMs45(parse, learnInfo.Source.CurrentMoves, gen, HMLearned, KnowDefogWhirlpool);
                    break;
            }
        }

        private static void ParseMovesByGeneration(PKM pkm, CheckMoveResult[] parse, int gen, LegalInfo info, LearnInfo learnInfo)
        {
            var moves = learnInfo.Source.CurrentMoves;
            bool native = gen == pkm.Format;
            for (int m = parse.Length - 1; m >= 0; m--)
            {
                var r = parse[m];
                if (IsCheckValid(r)) // already validated with another generation
                    continue;
                int move = moves[m];
                if (move == 0)
                    continue;

                if (gen <= 2)
                {
                    if (gen == 2 && !native && move > Legal.MaxMoveID_1 && pkm.VC1)
                    {
                        r.Set(Unknown, gen, Invalid, LMoveSourceInvalid, CurrentMove);
                        continue;
                    }
                    if (gen == 2 && learnInfo.Source.EggMoveSource.Contains(move))
                        r.Set(EggMove, gen, Valid, LMoveSourceEgg, CurrentMove);
                    else if (learnInfo.Source.Base.Contains(move))
                        r.Set(Initial, gen, Valid, native ? LMoveSourceDefault : string.Format(LMoveFDefault_0, gen), CurrentMove);
                }
                if (info.EncounterMoves.LevelUpMoves[gen].Contains(move))
                    r.Set(LevelUp, gen, Valid, native ? LMoveSourceLevelUp : string.Format(LMoveFLevelUp_0, gen), CurrentMove);
                else if (info.EncounterMoves.TMHMMoves[gen].Contains(move))
                    r.Set(TMHM, gen, Valid, native ? LMoveSourceTMHM : string.Format(LMoveFTMHM_0, gen), CurrentMove);
                else if (info.EncounterMoves.TutorMoves[gen].Contains(move))
                    r.Set(Tutor, gen, Valid, native ? LMoveSourceTutor : string.Format(LMoveFTutor_0, gen), CurrentMove);
                else if (gen == info.Generation && learnInfo.Source.SpecialSource.Contains(move))
                    r.Set(Special, gen, Valid, LMoveSourceSpecial, CurrentMove);
                else if (gen >= 8 && MoveEgg.GetIsSharedEggMove(pkm, gen, move))
                    r.Set(Shared, gen, Valid, native ? LMoveSourceShared : string.Format(LMoveSourceSharedF, gen), CurrentMove);

                if (gen >= 3 || !IsCheckValid(r))
                    continue;

                // Gen1/Gen2 only below
                if (gen == 2 && learnInfo.Source.NonTradeBackLevelUpMoves.Contains(m))
                {
                    learnInfo.Gen2PreevoMoves.Add(m);
                }
                else if (gen == 1)
                {
                    learnInfo.Gen1Moves.Add(m);
                    if (learnInfo.Gen2PreevoMoves.Count != 0)
                        learnInfo.MixedGen12NonTradeback = true;
                }
            }
        }

        private static void ParseMovesByGeneration12(PKM pkm, CheckMoveResult[] parse, IReadOnlyList<int> currentMoves, int gen, LegalInfo info, LearnInfo learnInfo)
        {
            // Mark the gen 1 exclusive moves as illegal because the pokemon also have Non tradeback egg moves.
            if (learnInfo.MixedGen12NonTradeback)
            {
                foreach (int m in learnInfo.Gen1Moves)
                    parse[m].FlagIllegal(LG1MoveExclusive, CurrentMove);

                foreach (int m in learnInfo.Gen2PreevoMoves)
                    parse[m].FlagIllegal(LG1TradebackPreEvoMove, CurrentMove);
            }

            if (gen == 1 && pkm.Format == 1 && !AllowGen1Tradeback)
            {
                ParseRedYellowIncompatibleMoves(pkm, parse, currentMoves);
                ParseEvolutionsIncompatibleMoves(pkm, parse, currentMoves, info.EncounterMoves.TMHMMoves[1]);
            }
        }

        private static void ParseMovesByGenerationLast(CheckMoveResult[] parse, LearnInfo learnInfo, IEncounterTemplate enc)
        {
            int gen = enc.Generation;
            ParseEggMovesInherited(parse, gen, learnInfo);
            ParseEggMoves(parse, gen, learnInfo);
            ParseEggMovesRemaining(parse, learnInfo, enc);
        }

        private static void ParseEggMovesInherited(CheckMoveResult[] parse, int gen, LearnInfo learnInfo)
        {
            var moves = learnInfo.Source.CurrentMoves;
            // Check higher-level moves after all the moves but just before egg moves to differentiate it from normal level up moves
            // Also check if the base egg moves is a non tradeback move
            for (int m = parse.Length - 1; m >= 0; m--)
            {
                var r = parse[m];
                if (IsCheckValid(r)) // already validated
                {
                    if (gen == 2 && r.Generation != 1)
                        continue;
                }

                int move = moves[m];
                if (move == 0)
                    continue;
                if (!learnInfo.Source.EggLevelUpSource.Contains(move)) // Check if contains level-up egg moves from parents
                    continue;

                if (learnInfo.IsGen2Pkm && learnInfo.Gen1Moves.Count != 0 && move > Legal.MaxMoveID_1)
                {
                    r.Set(InheritLevelUp, gen, Invalid, LG1MoveTradeback, CurrentMove);
                    learnInfo.MixedGen12NonTradeback = true;
                }
                else
                {
                    r.Set(InheritLevelUp, gen, Valid, LMoveEggLevelUp, CurrentMove);
                }
                learnInfo.LevelUpEggMoves.Add(m);
                if (gen == 2 && learnInfo.Gen1Moves.Contains(m))
                    learnInfo.Gen1Moves.Remove(m);
            }
        }

        private static void ParseEggMoves(CheckMoveResult[] parse, int gen, LearnInfo learnInfo)
        {
            var moves = learnInfo.Source.CurrentMoves;
            // Check egg moves after all the generations and all the moves, every move that can't be learned in another source should have preference
            // the moves that can only be learned from egg moves should in the future check if the move combinations can be breed in gens 2 to 5
            for (int m = parse.Length - 1; m >= 0; m--)
            {
                var r = parse[m];
                if (IsCheckValid(r))
                    continue;
                int move = moves[m];
                if (move == 0)
                    continue;

                bool wasEggMove = learnInfo.Source.EggMoveSource.Contains(move);
                if (wasEggMove)
                {
                    // To learn exclusive generation 1 moves the pokemon was tradeback, but it can't be trade to generation 1
                    // without removing moves above MaxMoveID_1, egg moves above MaxMoveID_1 and gen 1 moves are incompatible
                    if (learnInfo.IsGen2Pkm && learnInfo.Gen1Moves.Count != 0 && move > Legal.MaxMoveID_1)
                    {
                        r.Set(EggMove, gen, Invalid, LG1MoveTradeback, CurrentMove);
                        learnInfo.MixedGen12NonTradeback = true;
                    }
                    else
                    {
                        r.Set(EggMove, gen, Valid, LMoveSourceEgg, CurrentMove);
                    }

                    learnInfo.EggMovesLearned.Add(m);
                }
                if (!learnInfo.Source.EggEventSource.Contains(move))
                    continue;

                if (!wasEggMove)
                {
                    if (learnInfo.IsGen2Pkm && learnInfo.Gen1Moves.Count != 0 && move > Legal.MaxMoveID_1)
                    {
                        r.Set(SpecialEgg, gen, Invalid, LG1MoveTradeback, CurrentMove);
                        learnInfo.MixedGen12NonTradeback = true;
                    }
                    else
                    {
                        r.Set(SpecialEgg, gen, Valid, LMoveSourceEggEvent, CurrentMove);
                    }
                }
                learnInfo.EventEggMoves.Add(m);
            }
        }

        private static void ParseEggMovesRemaining(CheckMoveResult[] parse, LearnInfo learnInfo, IEncounterTemplate enc)
        {
            // A pokemon could have normal egg moves and regular egg moves
            // Only if all regular egg moves are event egg moves or all event egg moves are regular egg moves
            var RegularEggMovesLearned = learnInfo.EggMovesLearned.FindAll(learnInfo.LevelUpEggMoves.Contains);
            if (RegularEggMovesLearned.Count != 0 && learnInfo.EventEggMoves.Count != 0)
            {
                // Moves that are egg moves or event egg moves but not both
                var IncompatibleEggMoves = RegularEggMovesLearned.Except(learnInfo.EventEggMoves).Union(learnInfo.EventEggMoves.Except(RegularEggMovesLearned));
                foreach (int m in IncompatibleEggMoves)
                {
                    bool isEvent = learnInfo.EventEggMoves.Contains(m);
                    if (isEvent)
                    {
                        if (!learnInfo.EggMovesLearned.Contains(m))
                            parse[m].FlagIllegal(LMoveEggIncompatibleEvent, CurrentMove);
                    }
                    else
                    {
                        if (learnInfo.EggMovesLearned.Contains(m))
                            parse[m].FlagIllegal(LMoveEggIncompatible, CurrentMove);
                        else if (learnInfo.LevelUpEggMoves.Contains(m))
                            parse[m].FlagIllegal(LMoveEventEggLevelUp, CurrentMove);
                    }
                }
            }
            else if (enc is not EncounterEgg)
            {
                // Event eggs cannot inherit moves from parents; they are not bred.
                var gift = enc is EncounterStatic {Gift: true}; // otherwise, EncounterInvalid
                foreach (int m in RegularEggMovesLearned)
                {
                    if (learnInfo.EggMovesLearned.Contains(m))
                        parse[m].FlagIllegal(gift ? LMoveEggMoveGift : LMoveEggInvalidEvent, CurrentMove);
                    else if (learnInfo.LevelUpEggMoves.Contains(m))
                        parse[m].FlagIllegal(gift ? LMoveEggInvalidEventLevelUpGift : LMoveEggInvalidEventLevelUp, CurrentMove);
                }
            }
        }

        private static void ParseRedYellowIncompatibleMoves(PKM pkm, CheckMoveResult[] parse, IReadOnlyList<int> currentMoves)
        {
            var incompatible = GetIncompatibleRBYMoves(pkm, currentMoves);
            if (incompatible.Count == 0)
                return;
            for (int m = parse.Length - 1; m >= 0; m--)
            {
                if (incompatible.Contains(currentMoves[m]))
                    parse[m].FlagIllegal(LG1MoveLearnSameLevel, CurrentMove);
            }
        }

        private static IList<int> GetIncompatibleRBYMoves(PKM pkm, IReadOnlyList<int> currentMoves)
        {
            // Check moves that are learned at the same level in Red/Blue and Yellow, these are illegal because there is no Move Reminder in Gen1.
            // There are only two incompatibilities for Gen1; there are no illegal combination in Gen2.

            switch (pkm.Species)
            {
                // Vaporeon in Yellow learns Mist and Haze at level 42, Mist can only be learned if it leveled up in the daycare
                // Vaporeon in Red/Blue learns Acid Armor at level 42 and level 47 in Yellow
                case (int)Species.Vaporeon when pkm.CurrentLevel < 47 && currentMoves.Contains((int)Move.AcidArmor):
                {
                    var incompatible = new List<int>(3);
                    if (currentMoves.Contains((int)Move.Mist)) incompatible.Add((int)Move.Mist);
                    if (currentMoves.Contains((int)Move.Haze)) incompatible.Add((int)Move.Haze);
                    if (incompatible.Count != 0)
                        incompatible.Add((int)Move.AcidArmor);
                    else
                        return Array.Empty<int>();
                    return incompatible;
                }

                // Flareon in Yellow learns Smog at level 42
                // Flareon in Red Blue learns Leer at level 42 and level 47 in Yellow
                case (int)Species.Flareon when pkm.CurrentLevel < 47 && currentMoves.Contains((int)Move.Leer) && currentMoves.Contains((int)Move.Smog):
                    return new[] { (int)Move.Leer, (int)Move.Smog };

                default: return Array.Empty<int>();
            }
        }

        private static void ParseEvolutionsIncompatibleMoves(PKM pkm, CheckMoveResult[] parse, IReadOnlyList<int> moves, IReadOnlyList<int> tmhm)
        {
            GBRestrictions.GetIncompatibleEvolutionMoves(pkm, moves, tmhm,
                out var prevSpeciesID,
                out var incompatPrev,
                out var incompatCurr);

            if (prevSpeciesID == 0)
                return;

            var prev = SpeciesStrings[prevSpeciesID];
            var curr = SpeciesStrings[pkm.Species];
            for (int m = parse.Length - 1; m >= 0; m--)
            {
                if (incompatCurr.Contains(moves[m]))
                    parse[m].FlagIllegal(string.Format(LMoveEvoFLower, curr, prev), CurrentMove);
                if (incompatPrev.Contains(moves[m]))
                    parse[m].FlagIllegal(string.Format(LMoveEvoFHigher, curr, prev), CurrentMove);
            }
        }

        private static void ParseShedinjaEvolveMoves(PKM pkm, CheckMoveResult[] parse, IReadOnlyList<int> currentMoves, IReadOnlyList<IReadOnlyList<EvoCriteria>> evos)
        {
            int shedinjaEvoMoveIndex = 0;
            var format = pkm.Format;
            for (int gen = Math.Min(format, 4); gen >= 3; gen--)
            {
                if (evos[gen].Count != 2)
                    continue; // Was not evolved in this generation
                if (gen == 4 && pkm.Ball != 4 && !(pkm.Ball == (int)Ball.Sport && pkm.HGSS))
                    continue; // Was definitively evolved in Gen3

                var maxLevel = pkm.CurrentLevel;
                var ninjaskMoves = MoveList.GetShedinjaEvolveMoves(pkm, gen, maxLevel);
                bool native = gen == format;
                for (int m = parse.Length - 1; m >= 0; m--)
                {
                    var r = parse[m];
                    if (IsCheckValid(r)) // already validated
                        continue;

                    if (!ninjaskMoves.Contains(currentMoves[m]))
                        continue;

                    // Can't have more than one Ninjask exclusive move on Shedinja
                    if (shedinjaEvoMoveIndex != 0)
                    {
                        r.FlagIllegal(LMoveNincada, CurrentMove);
                        continue;
                    }

                    var msg = native ? LMoveNincadaEvo : string.Format(LMoveNincadaEvoF_0, gen);
                    r.Set(ShedinjaEvo, gen, Valid, msg, CurrentMove);
                    shedinjaEvoMoveIndex = m;
                }
            }

            if (shedinjaEvoMoveIndex == 0)
                return;

            // Double check that the Ninjask move level isn't less than any Nincada move level
            {
                var r = parse[shedinjaEvoMoveIndex];
                if (r.Source != LevelUp)
                    return;

                var move = currentMoves[shedinjaEvoMoveIndex];
                int levelS = MoveList.GetShedinjaMoveLevel((int)Species.Shedinja, move, r.Generation);
                if (levelS > 0)
                    return;

                int levelN = MoveList.GetShedinjaMoveLevel((int)Species.Nincada, move, r.Generation);
                int levelJ = MoveList.GetShedinjaMoveLevel((int)Species.Ninjask, move, r.Generation);
                if (levelN > levelJ)
                    r.FlagIllegal(string.Format(LMoveEvoFHigher, SpeciesStrings[(int)Species.Nincada], SpeciesStrings[(int)Species.Ninjask]), CurrentMove);
            }
        }

        private static bool GetHMCompatibility(PKM pkm, IReadOnlyList<CheckMoveResult> parse, int gen, IReadOnlyList<int> moves, Span<bool> HMLearned)
        {
            // Check if pokemon knows HM moves from generation 3 and 4 but are not valid yet, that means it cant learn the HMs in future generations
            if (gen == 4 && pkm.Format > 4)
            {
                FlagIsHMSource(HMLearned, Legal.HM_4_RemovePokeTransfer);
                return moves.Where((m, i) => IsDefogWhirl(m) && IsCheckInvalid(parse[i])).Count() == 2;
            }
            if (gen == 3 && pkm.Format > 3)
                FlagIsHMSource(HMLearned, Legal.HM_3);
            return false;

            void FlagIsHMSource(Span<bool> flags, ICollection<int> source)
            {
                for (int i = parse.Count - 1; i >= 0; i--)
                    flags[i] = IsCheckInvalid(parse[i]) && source.Contains(moves[i]);
            }
        }

        private static bool IsDefogWhirl(int move) => move is (int)Move.Defog or (int)Move.Whirlpool;
        private static int GetDefogWhirlCount(IReadOnlyList<CheckMoveResult> parse, IReadOnlyList<int> moves)
        {
            int ctr = 0;
            for (int i = moves.Count - 1; i >= 0; i--)
            {
                if (!IsDefogWhirl(moves[i]))
                    continue;
                var r = parse[i];
                if (!r.Valid || r.Generation >= 5)
                    continue;
                ctr++;
            }
            return ctr;
        }

        private static bool IsCheckInvalid(CheckMoveResult chk) => chk.IsParsed && !chk.Valid;
        private static bool IsCheckValid(CheckMoveResult chk) => chk.IsParsed && chk.Valid;

        private static void FlagIncompatibleTransferHMs45(IReadOnlyList<CheckMoveResult> parse, IReadOnlyList<int> currentMoves, int gen, ReadOnlySpan<bool> HMLearned, bool KnowDefogWhirlpool)
        {
            // After all the moves from the generations 3 and 4,
            // including egg moves if is the origin generation because some hidden moves are also special egg moves in gen 3
            // Check if the marked hidden moves that were invalid at the start are now marked as valid, that means
            // the hidden move was learned in gen 3 or 4 but was not removed when transfer to 4 or 5
            if (KnowDefogWhirlpool)
            {
                int invalidCount = GetDefogWhirlCount(parse, currentMoves);
                if (invalidCount == 2) // can't know both at the same time
                {
                    for (int i = parse.Count - 1; i >= 0; i--) // flag both moves
                    {
                        if (IsDefogWhirl(currentMoves[i]))
                            parse[i].FlagIllegal(LTransferMoveG4HM, CurrentMove);
                    }
                }
            }

            // Flag moves that are only legal when learned from a past-gen HM source
            for (int i = HMLearned.Length - 1; i >= 0; i--)
            {
                if (HMLearned[i] && IsCheckValid(parse[i]))
                    parse[i].FlagIllegal(string.Format(LTransferMoveHM, gen, gen + 1), CurrentMove);
            }
        }

        private static void VerifyNoEmptyDuplicates(ReadOnlySpan<int> moves, ReadOnlySpan<CheckMoveResult> parse)
        {
            bool emptySlot = false;
            for (int i = 0; i < parse.Length; i++)
            {
                var move = moves[i];
                if (move == 0)
                {
                    emptySlot = true;
                    continue;
                }

                // If an empty slot was noted for a prior move, flag the empty slots.
                if (emptySlot)
                {
                    FlagEmptySlotsBeforeIndex(moves, parse, i);
                    emptySlot = false;
                    continue;
                }

                // Check for same move in next move slots
                FlagDuplicateMovesAfterIndex(moves, parse, i, move);
            }
        }

        private static void FlagDuplicateMovesAfterIndex(ReadOnlySpan<int> moves, ReadOnlySpan<CheckMoveResult> parse, int index, int move)
        {
            for (int i = parse.Length - 1; i > index; i--)
            {
                if (moves[i] != move)
                    continue;
                parse[index].FlagIllegal(LMoveSourceEmpty);
                return;
            }
        }

        private static void FlagEmptySlotsBeforeIndex(ReadOnlySpan<int> moves, ReadOnlySpan<CheckMoveResult> parse, int index)
        {
            for (int i = index - 1; i >= 0; i--)
            {
                if (moves[i] != 0)
                    return;
                parse[i].FlagIllegal(LMoveSourceEmpty);
            }
        }
    }
}
