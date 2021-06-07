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
        public static CheckMoveResult[] VerifyMoves(PKM pkm, LegalInfo info)
        {
            var currentMoves = pkm.Moves;
            var res = ParseMovesForEncounters(pkm, info, currentMoves);

            // Duplicate Moves Check
            VerifyNoEmptyDuplicates(currentMoves, res);
            if (currentMoves[0] == 0) // Can't have an empty move slot for the first move.
                res[0] = new CheckMoveResult(res[0], Invalid, LMoveSourceEmpty, CurrentMove);

            return res;
        }

        private static CheckMoveResult[] ParseMovesForEncounters(PKM pkm, LegalInfo info, int[] currentMoves)
        {
            if (pkm.Species == (int)Species.Smeargle) // special handling for Smeargle
                return ParseMovesForSmeargle(pkm, currentMoves, info); // Smeargle can have any moves except a few

            // gather valid moves for encounter species
            var restrict = new LevelUpRestriction(pkm, info);
            info.EncounterMoves = new ValidEncounterMoves(pkm, restrict, info.EncounterMatch);

            IReadOnlyList<int> defaultG1LevelMoves = Array.Empty<int>();
            IReadOnlyList<int> defaultG2LevelMoves = Array.Empty<int>();
            var defaultTradeback = pkm.TradebackStatus;
            bool gb = false;
            int gen = info.EncounterMatch.Generation;
            if (gen <= 2)
            {
                gb = true;
                defaultG1LevelMoves = info.EncounterMoves.LevelUpMoves[1];
                if (pkm.InhabitedGeneration(2))
                    defaultG2LevelMoves = info.EncounterMoves.LevelUpMoves[2];

                // Generation 1 can have different minimum level in different encounter of the same species; update valid level moves
                UpdateGen1LevelUpMoves(pkm, info.EncounterMoves, restrict.MinimumLevelGen1, gen, info);

                // The same for Generation 2; if move reminder from Stadium 2 is not allowed
                if (!AllowGen2MoveReminder(pkm) && pkm.InhabitedGeneration(2))
                    UpdateGen2LevelUpMoves(pkm, info.EncounterMoves, restrict.MinimumLevelGen2, gen, info);
            }

            var res = info.Generation < 6
                ? ParseMovesPre3DS(pkm, currentMoves, info)
                : ParseMoves3DS(pkm, currentMoves, info);

            if (res.All(x => x.Valid))
                return res;

            // not valid
            if (gb) // restore generation 1 and 2 moves
            {
                info.EncounterMoves.LevelUpMoves[1] = defaultG1LevelMoves;
                if (pkm.InhabitedGeneration(2))
                    info.EncounterMoves.LevelUpMoves[2] = defaultG2LevelMoves;
            }
            pkm.TradebackStatus = defaultTradeback;
            return res;
        }

        private static CheckMoveResult[] ParseMovesForSmeargle(PKM pkm, IReadOnlyList<int> currentMoves, LegalInfo info)
        {
            if (!pkm.IsEgg)
                return ParseMovesSketch(pkm, currentMoves);

            // can only know sketch as egg
            var levelup = new int[info.EvoChainsAllGens.Length][];
            levelup[pkm.Format] = new[] {166};
            info.EncounterMoves = new ValidEncounterMoves(levelup);
            var source = new MoveParseSource { CurrentMoves = currentMoves, };
            return ParseMoves(pkm, source, info);
        }

        private static CheckMoveResult[] ParseMovesWasEggPreRelearn(PKM pkm, IReadOnlyList<int> currentMoves, LegalInfo info, EncounterEgg e)
        {
            var EventEggMoves = GetSpecialMoves(info.EncounterMatch);
            bool notEvent = EventEggMoves.Count == 0;
            // Level up moves could not be inherited if Ditto is parent,
            // that means genderless species and male only species (except Nidoran-M and Volbeat; they breed with Nidoran-F and Illumise) could not have level up moves as an egg
            var pi = pkm.PersonalInfo;
            var AllowLevelUp = notEvent && !pi.Genderless && !(pi.OnlyMale && Breeding.MixedGenderBreeding.Contains(e.Species));
            int BaseLevel = AllowLevelUp ? 100 : e.LevelMin;
            var LevelUp = MoveList.GetBaseEggMoves(pkm, e.Species, e.Form, e.Version, BaseLevel);

            var TradebackPreevo = pkm.Format == 2 && info.EncounterMatch.Species > 151;
            var NonTradebackLvlMoves = TradebackPreevo
                ? MoveList.GetExclusivePreEvolutionMoves(pkm, info.EncounterMatch.Species, info.EvoChainsAllGens[2], 2, e.Version).Where(m => m > Legal.MaxMoveID_1).ToArray()
                : Array.Empty<int>();

            var Egg = MoveEgg.GetEggMoves(pkm.PersonalInfo, e.Species, e.Form, e.Version, e.Generation);
            if (info.Generation < 3 && pkm.Format >= 7 && pkm.VC1)
                Egg = Egg.Where(m => m <= Legal.MaxMoveID_1).ToArray();

            bool volt = (info.Generation > 3 || e.Version == GameVersion.E) && Legal.LightBall.Contains(pkm.Species);
            var specialMoves = volt && notEvent ? new[] { (int)Move.VoltTackle } : Array.Empty<int>(); // Volt Tackle for bred Pichu line

            var source = new MoveParseSource
            {
                CurrentMoves = currentMoves,
                SpecialSource = specialMoves,
                NonTradeBackLevelUpMoves = NonTradebackLvlMoves,

                EggLevelUpSource = LevelUp,
                EggMoveSource = Egg,
                EggEventSource = EventEggMoves,
            };
            return ParseMoves(pkm, source, info);
        }

        private static CheckMoveResult[] ParseMovesSketch(PKM pkm, IReadOnlyList<int> currentMoves)
        {
            var res = new CheckMoveResult[4];
            for (int i = 0; i < 4; i++)
            {
                var move = currentMoves[i];
                res[i] = Legal.IsValidSketch(move, pkm.Format)
                    ? new CheckMoveResult(Sketch, pkm.Format, CurrentMove)
                    : new CheckMoveResult(Unknown, pkm.Format, Invalid, LMoveSourceInvalidSketch, CurrentMove);
            }

            return res;
        }

        private static CheckMoveResult[] ParseMoves3DS(PKM pkm, IReadOnlyList<int> currentMoves, LegalInfo info)
        {
            info.EncounterMoves.Relearn = info.Generation >= 6 ? pkm.RelearnMoves : Array.Empty<int>();
            return info.EncounterMatch is IMoveset
                ? ParseMovesSpecialMoveset(pkm, currentMoves, info)
                : ParseMovesRelearn(pkm, currentMoves, info);
        }

        private static CheckMoveResult[] ParseMovesPre3DS(PKM pkm, int[] currentMoves, LegalInfo info)
        {
            if (info.EncounterMatch is EncounterEgg e)
            {
                return pkm.IsEgg
                    ? VerifyPreRelearnEggBase(currentMoves, e)
                    : ParseMovesWasEggPreRelearn(pkm, currentMoves, info, e);
            }

            // Not all games have a re-learner. Initial moves may not fill out all 4 slots.
            int gen = info.EncounterMatch.Generation;
            if (gen == 1 || (gen == 2 && !AllowGen2MoveReminder(pkm)))
                return ParseMovesGenGB(pkm, currentMoves, info);

            return ParseMovesSpecialMoveset(pkm, currentMoves, info);
        }

        private static CheckMoveResult[] ParseMovesGenGB(PKM pkm, IReadOnlyList<int> currentMoves, LegalInfo info)
        {
            var res = new CheckMoveResult[4];
            var enc = info.EncounterMatch;
            var InitialMoves = Array.Empty<int>();
            var SpecialMoves = GetSpecialMoves(enc);
            var games = enc.Generation == 1 ? GBRestrictions.GetGen1Versions(enc) : GBRestrictions.GetGen2Versions(enc, pkm.Korean);
            foreach (var ver in games)
            {
                var VerInitialMoves = MoveLevelUp.GetEncounterMoves(enc.Species, 0, enc.LevelMin, ver);
                if (VerInitialMoves.Intersect(InitialMoves).Count() == VerInitialMoves.Length)
                    return res;

                var source = new MoveParseSource
                {
                    CurrentMoves = currentMoves,
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

        private static CheckMoveResult[] ParseMovesSpecialMoveset(PKM pkm, IReadOnlyList<int> currentMoves, LegalInfo info)
        {
            var source = new MoveParseSource
            {
                CurrentMoves = currentMoves,
                SpecialSource = GetSpecialMoves(info.EncounterMatch),
            };
            return ParseMoves(pkm, source, info);
        }

        private static IReadOnlyList<int> GetSpecialMoves(IEncounterable enc)
        {
            if (enc is IMoveset mg)
                return mg.Moves;
            return Array.Empty<int>();
        }

        private static CheckMoveResult[] ParseMovesRelearn(PKM pkm, IReadOnlyList<int> currentMoves, LegalInfo info)
        {
            var source = new MoveParseSource
            {
                CurrentMoves = currentMoves,
                SpecialSource = GetSpecialMoves(info.EncounterMatch),
            };

            if (info.EncounterMatch is EncounterEgg e)
                source.EggMoveSource = MoveEgg.GetEggMoves(pkm.PersonalInfo, e.Species, e.Form, e.Version, e.Generation);

            var res = ParseMoves(pkm, source, info);
            var relearn = pkm.RelearnMoves;
            for (int i = 0; i < 4; i++)
            {
                if ((pkm.IsEgg || res[i].IsRelearn) && !relearn.Contains(currentMoves[i]))
                    res[i] = new CheckMoveResult(res[i], Invalid, string.Format(LMoveRelearnFMiss_0, res[i].Comment), res[i].Identifier);
            }

            return res;
        }

        private static CheckMoveResult[] ParseMoves(PKM pkm, MoveParseSource source, LegalInfo info)
        {
            var res = new CheckMoveResult[4];
            bool AllParsed() => res.All(z => z != null);
            var required = pkm is not PK1 pk1 ? 1 : GBRestrictions.GetRequiredMoveCount(pk1, source.CurrentMoves, info, source.Base);

            // Special considerations!
            int reset = 0;
            if (pkm is IBattleVersion {BattleVersion: not 0} v)
            {
                reset = ((GameVersion) v.BattleVersion).GetGeneration();
                source.ResetSources();
            }

            // Check empty moves and relearn moves before generation specific moves
            for (int m = 0; m < 4; m++)
            {
                if (source.CurrentMoves[m] == 0)
                    res[m] = new CheckMoveResult(None, pkm.Format, m < required ? Fishy : Valid, LMoveSourceEmpty, CurrentMove);
                else if (reset == 0 && info.EncounterMoves.Relearn.Contains(source.CurrentMoves[m]))
                    res[m] = new CheckMoveResult(Relearn, info.Generation, Valid, LMoveSourceRelearn, CurrentMove);
            }

            if (AllParsed())
                return res;

            // Encapsulate arguments to simplify method calls
            var moveInfo = new LearnInfo(pkm, source);
            // Check moves going backwards, marking the move valid in the most current generation when it can be learned
            int[] generations = GenerationTraversal.GetVisitedGenerationOrder(pkm, info.EncounterOriginal.Generation);
            if (pkm.Format <= 2)
                generations = generations.Where(z => z < info.EncounterMoves.LevelUpMoves.Length).ToArray();
            if (reset != 0)
                generations = generations.Where(z => z >= reset).ToArray();

            int lastgen = generations.Length == 0 ? 0 : generations[^1];
            foreach (var gen in generations)
            {
                ParseMovesByGeneration(pkm, res, gen, info, moveInfo, lastgen);
                if (AllParsed())
                    return res;
            }

            if (pkm.Species == (int)Species.Shedinja && info.Generation <= 4)
                ParseShedinjaEvolveMoves(pkm, res, source.CurrentMoves, info.EvoChainsAllGens);

            // ReSharper disable once ConstantNullCoalescingCondition
            for (int m = 0; m < 4; m++)
                res[m] ??= new CheckMoveResult(Unknown, info.Generation, Invalid, LMoveSourceInvalid, CurrentMove);
            return res;
        }

        private static void ParseMovesByGeneration(PKM pkm, CheckMoveResult[] res, int gen, LegalInfo info, LearnInfo learnInfo, int last)
        {
            GetHMCompatibility(pkm, res, gen, learnInfo.Source.CurrentMoves, out bool[] HMLearned, out bool KnowDefogWhirlpool);
            ParseMovesByGeneration(pkm, res, gen, info, learnInfo);

            if (gen == last)
                ParseMovesByGenerationLast(pkm, res, learnInfo, info.EncounterMatch);

            switch (gen)
            {
                case 1 or 2:
                    ParseMovesByGeneration12(pkm, res, learnInfo.Source.CurrentMoves, gen, info, learnInfo);
                    break;

                case 3 or 4:
                    if (pkm.Format > gen)
                        FlagIncompatibleTransferHMs45(res, learnInfo.Source.CurrentMoves, gen, HMLearned, KnowDefogWhirlpool);
                    break;
            }
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
                        res[m] = new CheckMoveResult(Unknown, gen, Invalid, LMoveSourceInvalid, CurrentMove);
                        continue;
                    }
                    if (gen == 2 && learnInfo.Source.EggMoveSource.Contains(move))
                        res[m] = new CheckMoveResult(EggMove, gen, Valid, LMoveSourceEgg, CurrentMove);
                    else if (learnInfo.Source.Base.Contains(move))
                        res[m] = new CheckMoveResult(Initial, gen, Valid, native ? LMoveSourceDefault : string.Format(LMoveFDefault_0, gen), CurrentMove);
                }
                if (info.EncounterMoves.LevelUpMoves[gen].Contains(move))
                    res[m] = new CheckMoveResult(LevelUp, gen, Valid, native ? LMoveSourceLevelUp : string.Format(LMoveFLevelUp_0, gen), CurrentMove);
                else if (info.EncounterMoves.TMHMMoves[gen].Contains(move))
                    res[m] = new CheckMoveResult(TMHM, gen, Valid, native ? LMoveSourceTMHM : string.Format(LMoveFTMHM_0, gen), CurrentMove);
                else if (info.EncounterMoves.TutorMoves[gen].Contains(move))
                    res[m] = new CheckMoveResult(Tutor, gen, Valid, native ? LMoveSourceTutor : string.Format(LMoveFTutor_0, gen), CurrentMove);
                else if (gen == info.Generation && learnInfo.Source.SpecialSource.Contains(move))
                    res[m] = new CheckMoveResult(Special, gen, Valid, LMoveSourceSpecial, CurrentMove);
                else if (gen >= 8 && MoveEgg.GetIsSharedEggMove(pkm, gen, move))
                    res[m] = new CheckMoveResult(Shared, gen, Valid, native ? LMoveSourceShared : string.Format(LMoveSourceSharedF, gen), CurrentMove);

                if (gen >= 3 || !IsCheckValid(res[m]))
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

                if (pkm.TradebackStatus == TradebackType.Any && info.Generation != gen)
                    pkm.TradebackStatus = TradebackType.WasTradeback;
            }
        }

        private static void ParseMovesByGeneration12(PKM pkm, CheckMoveResult[] res, IReadOnlyList<int> currentMoves, int gen, LegalInfo info, LearnInfo learnInfo)
        {
            // Mark the gen 1 exclusive moves as illegal because the pokemon also have Non tradeback egg moves.
            if (learnInfo.MixedGen12NonTradeback)
            {
                foreach (int m in learnInfo.Gen1Moves)
                    res[m] = new CheckMoveResult(res[m], Invalid, LG1MoveExclusive, CurrentMove);

                foreach (int m in learnInfo.Gen2PreevoMoves)
                    res[m] = new CheckMoveResult(res[m], Invalid, LG1TradebackPreEvoMove, CurrentMove);
            }

            if (gen == 1 && pkm.Format == 1 && pkm.Gen1_NotTradeback)
            {
                ParseRedYellowIncompatibleMoves(pkm, res, currentMoves);
                ParseEvolutionsIncompatibleMoves(pkm, res, currentMoves, info.EncounterMoves.TMHMMoves[1]);
            }
        }

        private static void ParseMovesByGenerationLast(PKM pkm, CheckMoveResult[] res, LearnInfo learnInfo, IEncounterable enc)
        {
            int gen = enc.Generation;
            ParseEggMovesInherited(pkm, res, gen, learnInfo);
            ParseEggMoves(pkm, res, gen, learnInfo);
            ParseEggMovesRemaining(pkm, res, learnInfo, enc);
        }

        private static void ParseEggMovesInherited(PKM pkm, CheckMoveResult[] res, int gen, LearnInfo learnInfo)
        {
            var moves = learnInfo.Source.CurrentMoves;
            // Check higher-level moves after all the moves but just before egg moves to differentiate it from normal level up moves
            // Also check if the base egg moves is a non tradeback move
            for (int m = 0; m < 4; m++)
            {
                var r = res[m];
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
                    res[m] = new CheckMoveResult(InheritLevelUp, gen, Invalid, LG1MoveTradeback, CurrentMove);
                    learnInfo.MixedGen12NonTradeback = true;
                }
                else
                {
                    res[m] = new CheckMoveResult(InheritLevelUp, gen, Valid, LMoveEggLevelUp, CurrentMove);
                }
                learnInfo.LevelUpEggMoves.Add(m);
                if (gen == 2 && learnInfo.Gen1Moves.Contains(m))
                    learnInfo.Gen1Moves.Remove(m);
            }

            if (gen <= 2 && learnInfo.Gen1Moves.Count == 0)
                pkm.TradebackStatus = TradebackType.Any;
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
                        res[m] = new CheckMoveResult(EggMove, gen, Invalid, LG1MoveTradeback, CurrentMove);
                        learnInfo.MixedGen12NonTradeback = true;
                    }
                    else
                    {
                        res[m] = new CheckMoveResult(EggMove, gen, Valid, LMoveSourceEgg, CurrentMove);
                    }

                    learnInfo.EggMovesLearned.Add(m);
                    if (pkm.TradebackStatus == TradebackType.Any && pkm.Generation == 1)
                        pkm.TradebackStatus = TradebackType.WasTradeback;
                }
                if (!learnInfo.Source.EggEventSource.Contains(move))
                    continue;

                if (!wasEggMove)
                {
                    if (learnInfo.IsGen2Pkm && learnInfo.Gen1Moves.Count != 0 && move > Legal.MaxMoveID_1)
                    {
                        res[m] = new CheckMoveResult(SpecialEgg, gen, Invalid, LG1MoveTradeback, CurrentMove);
                        learnInfo.MixedGen12NonTradeback = true;
                    }
                    else
                    {
                        res[m] = new CheckMoveResult(SpecialEgg, gen, Valid, LMoveSourceEggEvent, CurrentMove);
                    }
                }
                if (pkm.TradebackStatus == TradebackType.Any && pkm.Generation == 1)
                    pkm.TradebackStatus = TradebackType.WasTradeback;
                learnInfo.EventEggMoves.Add(m);
            }
        }

        private static void ParseEggMovesRemaining(PKM pkm, CheckMoveResult[] res, LearnInfo learnInfo, IEncounterable enc)
        {
            // A pokemon could have normal egg moves and regular egg moves
            // Only if all regular egg moves are event egg moves or all event egg moves are regular egg moves
            var RegularEggMovesLearned = learnInfo.EggMovesLearned.Union(learnInfo.LevelUpEggMoves).ToList();
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
                            res[m] = new CheckMoveResult(res[m], Invalid, LMoveEggIncompatibleEvent, CurrentMove);
                    }
                    else
                    {
                        if (learnInfo.EggMovesLearned.Contains(m))
                            res[m] = new CheckMoveResult(res[m], Invalid, LMoveEggIncompatible, CurrentMove);
                        else if (learnInfo.LevelUpEggMoves.Contains(m))
                            res[m] = new CheckMoveResult(res[m], Invalid, LMoveEventEggLevelUp, CurrentMove);
                    }
                }
            }
            else if (enc is not EncounterEgg)
            {
                // Event eggs cannot inherit moves from parents; they are not bred.
                foreach (int m in RegularEggMovesLearned)
                {
                    if (learnInfo.EggMovesLearned.Contains(m))
                        res[m] = new CheckMoveResult(res[m], Invalid, pkm.WasGiftEgg ? LMoveEggMoveGift : LMoveEggInvalidEvent, CurrentMove);
                    else if (learnInfo.LevelUpEggMoves.Contains(m))
                        res[m] = new CheckMoveResult(res[m], Invalid, pkm.WasGiftEgg ? LMoveEggInvalidEventLevelUpGift : LMoveEggInvalidEventLevelUp, CurrentMove);
                }
            }
        }

        private static void ParseRedYellowIncompatibleMoves(PKM pkm, IList<CheckMoveResult> res, IReadOnlyList<int> currentMoves)
        {
            var incompatible = GetIncompatibleRBYMoves(pkm, currentMoves);
            if (incompatible.Count == 0)
                return;
            for (int m = 0; m < 4; m++)
            {
                if (incompatible.Contains(currentMoves[m]))
                    res[m] = new CheckMoveResult(res[m], Invalid, LG1MoveLearnSameLevel, CurrentMove);
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
                    if (currentMoves.Contains((int)Move.Mist))
                        incompatible.Add((int)Move.Mist);
                    if (currentMoves.Contains((int)Move.Haze))
                        incompatible.Add((int)Move.Haze);
                    if (incompatible.Count != 0)
                        incompatible.Add((int)Move.AcidArmor);
                    return incompatible;
                }

                // Flareon in Yellow learns Smog at level 42
                // Flareon in Red Blue learns Leer at level 42 and level 47 in Yellow
                case (int)Species.Flareon when pkm.CurrentLevel < 47 && currentMoves.Contains((int)Move.Leer) && currentMoves.Contains((int)Move.Smog):
                    return new[] { (int)Move.Leer, (int)Move.Smog };

                default: return Array.Empty<int>();
            }
        }

        private static void ParseEvolutionsIncompatibleMoves(PKM pkm, IList<CheckMoveResult> res, IReadOnlyList<int> moves, IReadOnlyList<int> tmhm)
        {
            GBRestrictions.GetIncompatibleEvolutionMoves(pkm, moves, tmhm,
                out var prevSpeciesID,
                out var incompatPrev,
                out var incompatCurr);

            if (prevSpeciesID == 0)
                return;

            var prev = SpeciesStrings[prevSpeciesID];
            var curr = SpeciesStrings[pkm.Species];
            for (int m = 0; m < 4; m++)
            {
                if (incompatCurr.Contains(moves[m]))
                    res[m] = new CheckMoveResult(res[m], Invalid, string.Format(LMoveEvoFLower, curr, prev), CurrentMove);
                if (incompatPrev.Contains(moves[m]))
                    res[m] = new CheckMoveResult(res[m], Invalid, string.Format(LMoveEvoFHigher, curr, prev), CurrentMove);
            }
        }

        private static void ParseShedinjaEvolveMoves(PKM pkm, IList<CheckMoveResult> res, IReadOnlyList<int> currentMoves, IReadOnlyList<IReadOnlyList<EvoCriteria>> evos)
        {
            var ShedinjaEvoMovesLearned = new List<int>();
            var format = pkm.Format;
            for (int gen = Math.Min(format, 4); gen >= 3; gen--)
            {
                if (evos[gen].Count != 2)
                    continue; // Was not evolved in this generation
                if (gen == 4 && pkm.Ball != 4)
                    continue; // Was definitively evolved in Gen3

                var maxLevel = pkm.CurrentLevel;
                var ninjaskMoves = MoveList.GetShedinjaEvolveMoves(pkm, gen, maxLevel);
                bool native = gen == format;
                for (int m = 0; m < 4; m++)
                {
                    if (IsCheckValid(res[m])) // already validated
                        continue;

                    if (!ninjaskMoves.Contains(currentMoves[m]))
                        continue;

                    var msg = native ? LMoveNincadaEvo : string.Format(LMoveNincadaEvoF_0, gen);
                    res[m] = new CheckMoveResult(ShedinjaEvo, gen, Valid, msg, CurrentMove);
                    ShedinjaEvoMovesLearned.Add(m);
                }
            }

            if (ShedinjaEvoMovesLearned.Count == 0)
                return;
            if (ShedinjaEvoMovesLearned.Count > 1)
            {
                // Can't have more than one Ninjask exclusive move on Shedinja
                foreach (int m in ShedinjaEvoMovesLearned)
                    res[m] = new CheckMoveResult(res[m], Invalid, LMoveNincada, CurrentMove);
                return;
            }

            // Double check that the Ninjask move level isn't less than any Nincada move level
            int move = ShedinjaEvoMovesLearned[0];
            int g = res[move].Generation;
            int levelJ = MoveList.GetShedinjaMoveLevel((int)Species.Ninjask, currentMoves[move], g);

            for (int m = 0; m < 4; m++)
            {
                if (m != move)
                    continue;
                if (res[m].Source != LevelUp)
                    continue;
                int levelS = MoveList.GetShedinjaMoveLevel((int)Species.Shedinja, currentMoves[m], res[m].Generation);
                if (levelS > 0)
                    continue;

                int levelN = MoveList.GetShedinjaMoveLevel((int)Species.Nincada, currentMoves[m], res[m].Generation);
                if (levelN > levelJ)
                    res[m] = new CheckMoveResult(res[m], Invalid, string.Format(LMoveEvoFHigher, SpeciesStrings[(int)Species.Nincada], SpeciesStrings[(int)Species.Ninjask]), CurrentMove);
            }
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

        private static bool IsDefogWhirl(int move) => move is (int)Move.Defog or (int)Move.Whirlpool;
        private static bool IsCheckInvalid(CheckResult? chk) => !(chk?.Valid ?? false);
        private static bool IsCheckValid(CheckResult? chk) => chk?.Valid ?? false;

        private static void FlagIncompatibleTransferHMs45(CheckMoveResult[] res, IReadOnlyList<int> currentMoves, int gen, IReadOnlyList<bool> HMLearned, bool KnowDefogWhirlpool)
        {
            // After all the moves from the generations 3 and 4,
            // including egg moves if is the origin generation because some hidden moves are also special egg moves in gen 3
            // Check if the marked hidden moves that were invalid at the start are now marked as valid, that means
            // the hidden move was learned in gen 3 or 4 but was not removed when transfer to 4 or 5
            if (KnowDefogWhirlpool)
            {
                int invalidCount = currentMoves.Where((m, i) => IsDefogWhirl(m) && IsCheckValid(res[i])).Count();
                if (invalidCount == 2) // can't know both at the same time
                {
                    for (int i = 0; i < 4; i++) // flag both moves
                    {
                        if (IsDefogWhirl(currentMoves[i]))
                            res[i] = new CheckMoveResult(res[i], Invalid, LTransferMoveG4HM, CurrentMove);
                    }
                }
            }

            // Flag moves that are only legal when learned from a past-gen HM source
            for (int i = 0; i < HMLearned.Count; i++)
            {
                if (HMLearned[i] && IsCheckValid(res[i]))
                    res[i] = new CheckMoveResult(res[i], Invalid, string.Format(LTransferMoveHM, gen, gen + 1), CurrentMove);
            }
        }

        private static CheckMoveResult[] VerifyPreRelearnEggBase(int[] currentMoves, EncounterEgg e)
        {
            CheckMoveResult[] result = new CheckMoveResult[4];
            _ = VerifyRelearnMoves.VerifyEggMoveset(e, result, currentMoves, CurrentMove);
            return result;
        }

        private static void VerifyNoEmptyDuplicates(IReadOnlyList<int> moves, CheckMoveResult[] res)
        {
            bool emptySlot = false;
            for (int i = 0; i < 4; i++)
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
                    FlagEmptySlotsBeforeIndex(moves, res, i);
                    emptySlot = false;
                    continue;
                }

                // Check for same move in next move slots
                FlagDuplicateMovesAfterIndex(moves, res, i, move);
            }
        }

        private static void FlagDuplicateMovesAfterIndex(IReadOnlyList<int> moves, CheckMoveResult[] res, int i, int move)
        {
            for (int j = i + 1; j < 4; j++)
            {
                if (moves[j] != move)
                    continue;
                res[i] = new CheckMoveResult(res[i], Invalid, LMoveSourceDuplicate);
                return;
            }
        }

        private static void FlagEmptySlotsBeforeIndex(IReadOnlyList<int> moves, CheckMoveResult[] res, int i)
        {
            for (int k = i - 1; k >= 0; k--)
            {
                if (moves[k] != 0)
                    return;
                res[k] = new CheckMoveResult(res[k], Invalid, LMoveSourceEmpty);
            }
        }

        private static void UpdateGen1LevelUpMoves(PKM pkm, ValidEncounterMoves EncounterMoves, int defaultLvlG1, int generation, LegalInfo info)
        {
            if (generation >= 3)
                return;
            var lvlG1 = info.EncounterMatch.LevelMin + 1;
            if (lvlG1 == defaultLvlG1)
                return;
            EncounterMoves.LevelUpMoves[1] = MoveList.GetValidMoves(pkm, info.EvoChainsAllGens[1], generation: 1, minLvLG1: lvlG1, types: MoveSourceType.LevelUp).ToList();
        }

        private static void UpdateGen2LevelUpMoves(PKM pkm, ValidEncounterMoves EncounterMoves, int defaultLvlG2, int generation, LegalInfo info)
        {
            if (generation >= 3)
                return;
            var lvlG2 = info.EncounterMatch.LevelMin + 1;
            if (lvlG2 == defaultLvlG2)
                return;
            EncounterMoves.LevelUpMoves[2] = MoveList.GetValidMoves(pkm, info.EvoChainsAllGens[2], generation: 2, minLvLG2: defaultLvlG2, types: MoveSourceType.LevelUp).ToList();
        }
    }
}
