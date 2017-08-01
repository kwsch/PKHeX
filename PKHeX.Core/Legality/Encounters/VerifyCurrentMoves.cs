using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.LegalityAnalysis;

namespace PKHeX.Core
{
    public static class VerifyCurrentMoves
    {
        public static CheckMoveResult[] VerifyMoves(PKM pkm, LegalInfo info, GameVersion game = GameVersion.Any)
        {
            int[] Moves = pkm.Moves;
            var res = ParseMovesForEncounters(pkm, info, game, Moves);

            // Duplicate Moves Check
            VerifyNoEmptyDuplicates(Moves, res);
            if (Moves[0] == 0) // Can't have an empty moveslot for the first move.
                res[0] = new CheckMoveResult(res[0], Severity.Invalid, V167, CheckIdentifier.Move);

            return res;
        }

        private static CheckMoveResult[] ParseMovesForEncounters(PKM pkm, LegalInfo info, GameVersion game, int[] Moves)
        {
            if (pkm.Species == 235) // special handling for Smeargle
                return ParseMovesForSmeargle(pkm, Moves, info); // Smeargle can have any moves except a few

            // Iterate over encounters
            bool pre3DS = pkm.GenNumber < 6;

            // gather valid moves for encounter species

            info.EncounterMoves = GetEncounterValidMoves(pkm, info);

            if (pkm.GenNumber <= 3)
                pkm.WasEgg = info.EncounterMatch.EggEncounter;

            var EncounterMatchGen = info.EncounterMatch as IGeneration;
            var defaultG1LevelMoves = info.EncounterMoves.LevelUpMoves[1];
            var defaultG2LevelMoves = pkm.InhabitedGeneration(2) ? info.EncounterMoves.LevelUpMoves[2] : null;
            var defaultTradeback = pkm.TradebackStatus;
            if (EncounterMatchGen != null)
            {
                // Generation 1 can have different minimum level in different encounter of the same species; update valid level moves
                UptateGen1LevelUpMoves(pkm, info.EncounterMoves, info.EncounterMoves.MinimumLevelGen1, EncounterMatchGen.Generation, info);
                if(!Legal.AllowGen2MoveReminder && pkm.InhabitedGeneration(2))
                    // The same for Generation 2 if move reminder from Stadium 2 is not allowed
                    UptateGen2LevelUpMoves(pkm, info.EncounterMoves, info.EncounterMoves.MinimumLevelGen2, EncounterMatchGen.Generation, info);
            }

            var res = pre3DS
                ? ParseMovesPre3DS(pkm, Moves, info)
                : ParseMoves3DS(pkm, game, Moves, info);

            if (res.All(x => x.Valid))
                return res;

            if (EncounterMatchGen?.Generation == 1 || EncounterMatchGen?.Generation == 2) // not valid, restore generation 1 and 2 moves
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
            info.EncounterMoves = new ValidEncounterMoves
            {
                LevelUpMoves = Legal.GetValidMovesAllGens(pkm, info.EvoChainsAllGens, minLvLG1: 1, Tutor: false, Machine: false, RemoveTransferHM: false)
            };
            return ParseMoves(pkm, pkm.Moves, new int[0], new int[0], new int[0], new int[0], new int[0], new int[0], info);
        }

        private class MoveInfoSet
        {
            public List<int> SpecialMoves { get; set; }
            public List<int> EggMoves { get; set; }
            public List<int> BaseMoves { get; set; }

            public bool AllowInherited { get; set; }
            public List<int> TutorMoves { get; set; }
            public List<int> TMHMMoves { get; set; }
            public List<int> LvlMoves { get; set; }
        }
        private static CheckMoveResult[] ParseMovesIsEggPreRelearn(PKM pkm, int[] Moves, int[] SpecialMoves, bool allowinherited, EncounterEgg e)
        {
            CheckMoveResult[] res = new CheckMoveResult[4];

            var baseEggMoves = Legal.GetBaseEggMoves(pkm, e.Species, e.Game, pkm.GenNumber < 4 ? 5 : 1)?.ToList() ?? new List<int>();
            // Level up moves cannot be inherited if Ditto is parent, thus genderless/single gender species cannot have level up moves as an egg
            bool AllowLvlMoves = pkm.PersonalInfo.Gender > 0 && pkm.PersonalInfo.Gender < 255 || Legal.MixedGenderBreeding.Contains(e.Species);
            var InheritedLvlMoves = !AllowLvlMoves? new List<int>() : Legal.GetBaseEggMoves(pkm, e.Species, e.Game, 100)?.ToList() ?? new List<int>();
            InheritedLvlMoves.RemoveAll(x => baseEggMoves.Contains(x));

            var infoset = new MoveInfoSet
            {
                EggMoves = Legal.GetEggMoves(pkm, e.Species, pkm.AltForm)?.ToList() ?? new List<int>(),
                TutorMoves = e.Game == GameVersion.C ? Legal.GetTutorMoves(pkm, pkm.Species, pkm.AltForm, false, 2)?.ToList() : new List<int>(),
                TMHMMoves = Legal.GetTMHM(pkm, pkm.Species, pkm.AltForm, pkm.GenNumber, e.Game, false)?.ToList(),
                LvlMoves = InheritedLvlMoves,
                BaseMoves = baseEggMoves,
                SpecialMoves = SpecialMoves.Where(m => m != 0).ToList(),
                AllowInherited = allowinherited
            };
            // Only TM Hm moves from the source game of the egg, not any other games from the same generation

            if (pkm.Format > 2 || SpecialMoves.Any())
            {
                // For gen 2 is not possible to difference normal eggs from event eggs
                // If there is no special moves assume normal egg
                res = VerifyPreRelearnEggBase(pkm, Moves, infoset, e.Game);
            }
            else if (pkm.Format == 2)
            {
                infoset.SpecialMoves.Clear();
                infoset.AllowInherited = true;
                res = VerifyPreRelearnEggBase(pkm, Moves, infoset, e.Game);
            }

            return res;
        }
        private static CheckMoveResult[] ParseMovesWasEggPreRelearn(PKM pkm, int[] Moves, LegalInfo info, EncounterEgg e)
        {
            var EventEggMoves = GetSpecialMoves(info.EncounterMatch);
            // Level up moves could not be inherited if Ditto is parent, 
            // that means genderless species and male only species except Nidoran and Volbet (they breed with female nidoran and illumise) could not have level up moves as an egg
            var inheritLvlMoves = pkm.PersonalInfo.Gender > 0 && pkm.PersonalInfo.Gender < 255 || Legal.MixedGenderBreeding.Contains(e.Species);
            int BaseLvlMoves = inheritLvlMoves ? 100 : pkm.GenNumber <= 3 ? 5 : 1;
            var LvlupEggMoves = Legal.GetBaseEggMoves(pkm, e.Species, e.Game, BaseLvlMoves);
            var TradebackPreevo = pkm.Format == 2 && info.EncounterMatch.Species > 151;
            var NonTradebackLvlMoves = new int[0];
            if (TradebackPreevo)
                NonTradebackLvlMoves = Legal.GetExclusivePreEvolutionMoves(pkm, info.EncounterMatch.Species, info.EvoChainsAllGens[2], 2, e.Game).Where(m => m > Legal.MaxMoveID_1).ToArray();
            var EggMoves = Legal.GetEggMoves(pkm, e.Species, pkm.AltForm);

            bool volt = (pkm.GenNumber > 3 || e.Game == GameVersion.E) && Legal.LightBall.Contains(pkm.Species);
            var SpecialMoves = volt && EventEggMoves.Length == 0 ? new[] { 344 } : new int[0]; // Volt Tackle for bred Pichu line

            return ParseMoves(pkm, Moves, SpecialMoves, LvlupEggMoves, EggMoves, NonTradebackLvlMoves, EventEggMoves, new int[0], info);
        }
        private static CheckMoveResult[] ParseMovesSketch(PKM pkm, int[] Moves)
        {
            CheckMoveResult[] res = new CheckMoveResult[4];
            for (int i = 0; i < 4; i++)
                res[i] = Legal.InvalidSketch.Contains(Moves[i])
                    ? new CheckMoveResult(MoveSource.Unknown, pkm.Format, Severity.Invalid, V166, CheckIdentifier.Move)
                    : new CheckMoveResult(MoveSource.Sketch, pkm.Format, CheckIdentifier.Move);
            return res;
        }
        private static CheckMoveResult[] ParseMoves3DS(PKM pkm, GameVersion game, int[] Moves, LegalInfo info)
        {
            info.EncounterMoves.Relearn = pkm.GenNumber >= 6 ? pkm.RelearnMoves : new int[0];
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
                // Gift do not have special moves but also should not have normal egg moves
                var allowinherited = SpecialMoves == null && !pkm.WasGiftEgg && pkm.Species != 489 && pkm.Species != 490;
                return ParseMovesIsEggPreRelearn(pkm, Moves, SpecialMoves, allowinherited, egg);
            }
            var NoMoveReminder = (info.EncounterMatch as IGeneration)?.Generation == 1 || (info.EncounterMatch as IGeneration)?.Generation == 2 && !Legal.AllowGen2MoveReminder;
            if (pkm.GenNumber <= 2 && NoMoveReminder)
                return ParseMovesGenGB(pkm, Moves, info);
            if (info.EncounterMatch is EncounterEgg e)
                return ParseMovesWasEggPreRelearn(pkm, Moves, info, e);

            return ParseMovesSpecialMoveset(pkm, Moves, info);
        }
        private static CheckMoveResult[] ParseMovesGenGB(PKM pkm, int[] Moves, LegalInfo info)
        {
            GameVersion[] games = (info.EncounterMatch as IGeneration)?.Generation == 1 ? Legal.GetGen1Versions(info) : Legal.GetGen2Versions(info);
            CheckMoveResult[] res = new CheckMoveResult[4];
            var G1Encounter = info.EncounterMatch;
            if (G1Encounter == null)
                return ParseMovesSpecialMoveset(pkm, Moves, info);
            var InitialMoves = new int[0];
            int[] SpecialMoves = GetSpecialMoves(info.EncounterMatch);
            var emptyegg = new int[0];
            foreach (GameVersion ver in games)
            {
                var VerInitialMoves = Legal.GetInitialMovesGBEncounter(G1Encounter.Species, G1Encounter.LevelMin, ver).ToArray();
                if (VerInitialMoves.SequenceEqual(InitialMoves))
                    return res;
                res = ParseMoves(pkm, Moves, SpecialMoves, emptyegg, emptyegg, emptyegg, new int[0], VerInitialMoves, info);
                if (res.All(r => r.Valid))
                    return res;
                InitialMoves = VerInitialMoves;
            }
            return res;
        }
        private static CheckMoveResult[] ParseMovesSpecialMoveset(PKM pkm, int[] Moves, LegalInfo info)
        {
            int[] SpecialMoves = GetSpecialMoves(info.EncounterMatch);
            var emptyegg = new int[0];
            return ParseMoves(pkm, Moves, SpecialMoves, emptyegg, emptyegg, emptyegg, new int[0], new int[0], info);
        }
        private static int[] GetSpecialMoves(IEncounterable EncounterMatch)
        {
            switch (EncounterMatch)
            {
                case IMoveset mg:
                    return mg.Moves ?? new int[0];
                case EncounterSlot s when s.Type == SlotType.Swarm && (s.Species == 273 || s.Species == 274):
                    return new[] {73}; // Leech Seed for RSE Swarm (Seedot || Nuzleaf); only matches for RSE origin encounters.
            }
            return new int[0];
        }
        private static CheckMoveResult[] ParseMovesRelearn(PKM pkm, int[] Moves, LegalInfo info)
        {
            var emptyegg = new int[0];

            var e = info.EncounterMatch as EncounterEgg;
            var EggMoves = e != null ? Legal.GetEggMoves(pkm, e.Species, pkm.AltForm) : emptyegg;
            var TradebackPreevo = pkm.Format == 2 && info.EncounterMatch.Species > 151 && pkm.InhabitedGeneration(1);
            var NonTradebackLvlMoves = TradebackPreevo ? Legal.GetExclusivePreEvolutionMoves(pkm, info.EncounterMatch.Species, info.EvoChainsAllGens[2], 2, e.Game).Where(m => m > Legal.MaxMoveID_1).ToArray() : new int[0];
            
            int[] RelearnMoves = pkm.RelearnMoves;
            int[] SpecialMoves = GetSpecialMoves(info.EncounterMatch);

            CheckMoveResult[] res = ParseMoves(pkm, Moves, SpecialMoves, new int[0], EggMoves, NonTradebackLvlMoves, new int[0], new int[0], info);

            for (int i = 0; i < 4; i++)
                if ((pkm.IsEgg || res[i].Flag) && !RelearnMoves.Contains(Moves[i]))
                    res[i] = new CheckMoveResult(res[i], Severity.Invalid, string.Format(V170, res[i].Comment), res[i].Identifier);

            return res;
        }
        private static CheckMoveResult[] ParseMoves(PKM pkm, int[] moves, int[] special, int[] lvlupegg, int[] egg, int[] NonTradebackLvlMoves, int[] eventegg, int[] initialmoves, LegalInfo info)
        {
            CheckMoveResult[] res = new CheckMoveResult[4];
            var required = Legal.GetRequiredMoveCount(pkm, moves, info, initialmoves);

            // Check none moves and relearn moves before generation moves
            for (int m = 0; m < 4; m++)
            {
                if (moves[m] == 0)
                    res[m] = new CheckMoveResult(MoveSource.None, pkm.Format, m < required ? Severity.Fishy : Severity.Valid, V167, CheckIdentifier.Move);
                else if (info.EncounterMoves.Relearn.Contains(moves[m]))
                    res[m] = new CheckMoveResult(MoveSource.Relearn, pkm.GenNumber, Severity.Valid, V172, CheckIdentifier.Move) { Flag = true };
            }

            if (res.All(r => r != null))
                return res;

            bool MixedGen1NonTradebackGen2 = false;
            var Gen1MovesLearned = new List<int>();
            var Gen2PreevoMovesLearned = new List<int>();
            var EggMovesLearned = new List<int>();
            var LvlupEggMovesLearned = new List<int>();
            var EventEggMovesLearned = new List<int>();
            var IsGen2Pkm = pkm.Format == 2 || pkm.VC2;
            var IncenseMovesLearned = new List<int>();
            // Check moves going backwards, marking the move valid in the most current generation when it can be learned
            int[] generations = GetGenMovesCheckOrder(pkm);
            if (pkm.Format <= 2)
                generations = generations.Where(z => z < info.EncounterMoves.LevelUpMoves.Length).ToArray();
            foreach (var gen in generations)
            {
                var HMLearned = new int[0];
                // Check if pokemon knows HM moves from generation 3 and 4 but are not valid yet, that means it cant learn the HMs in future generations
                bool KnowDefogWhirlpool = false;
                if (gen == 4 && pkm.Format > 4)
                {
                    // Copy to array the hm found or else the list will be emptied when the legal status of moves changes in the current generation
                    HMLearned = moves.Where((m, i) => !(res[i]?.Valid ?? false) && Legal.HM_4_RemovePokeTransfer.Any(l => l == m)).Select((m, i) => i).ToArray();
                    // Defog and Whirlpool at the same time, also both can't be learned in future generations or else they will be valid
                    KnowDefogWhirlpool = moves.Where((m, i) => (m == 250 || m == 432) && !(res[i]?.Valid ?? false)).Count() == 2;
                }
                else if (gen == 3 && pkm.Format > 3)
                    HMLearned = moves.Select((m, i) => i).Where(i => !(res[i]?.Valid ?? false) && Legal.HM_3.Any(l => l == moves[i])).ToArray();

                bool native = gen == pkm.Format;
                for (int m = 0; m < 4; m++)
                {
                    if (res[m]?.Valid ?? false) // already validated with another generation
                        continue;
                    if (moves[m] == 0)
                        continue;

                    if (gen == 1 && initialmoves.Contains(moves[m]))
                        res[m] = new CheckMoveResult(MoveSource.Initial, gen, Severity.Valid, native ? V361 : string.Format(V362, gen), CheckIdentifier.Move);
                    else if (info.EncounterMoves.LevelUpMoves[gen].Contains(moves[m]))
                        res[m] = new CheckMoveResult(MoveSource.LevelUp, gen, Severity.Valid, native ? V177 : string.Format(V330, gen), CheckIdentifier.Move);
                    else if (info.EncounterMoves.TMHMMoves[gen].Contains(moves[m]))
                        res[m] = new CheckMoveResult(MoveSource.TMHM, gen, Severity.Valid, native ? V173 : string.Format(V331, gen), CheckIdentifier.Move);
                    else if (info.EncounterMoves.TutorMoves[gen].Contains(moves[m]))
                        res[m] = new CheckMoveResult(MoveSource.Tutor, gen, Severity.Valid, native ? V174 : string.Format(V332, gen), CheckIdentifier.Move);
                    else if (gen == pkm.GenNumber && special.Contains(moves[m]))
                        res[m] = new CheckMoveResult(MoveSource.Special, gen, Severity.Valid, V175, CheckIdentifier.Move);

                    if (res[m] == null || gen >= 3)
                        continue;

                    if (res[m].Valid && gen == 2 && NonTradebackLvlMoves.Contains(m))
                        Gen2PreevoMovesLearned.Add(m);
                    if (res[m].Valid && gen == 1)
                    {
                        Gen1MovesLearned.Add(m);
                        if (Gen2PreevoMovesLearned.Any())
                            MixedGen1NonTradebackGen2 = true;
                    }

                    if (res[m].Valid && gen <= 2 && pkm.TradebackStatus == TradebackType.Any && pkm.GenNumber != gen)
                        pkm.TradebackStatus = TradebackType.WasTradeback;
                }

                if (gen == generations.Last())
                {
                    // Check higher-level moves after all the moves but just before egg moves to differentiate it from normal level up moves
                    // Also check if the base egg moves is a non tradeback move
                    for (int m = 0; m < 4; m++)
                    {
                        if (res[m]?.Valid ?? false) // Skip valid move
                            continue;
                        if (moves[m] == 0)
                            continue;
                        if (!lvlupegg.Contains(moves[m])) // Check if contains level-up egg moves from parents
                            continue;

                        if (IsGen2Pkm && Gen1MovesLearned.Any() && moves[m] > Legal.MaxMoveID_1)
                        {
                            res[m] = new CheckMoveResult(MoveSource.InheritLevelUp, gen, Severity.Invalid, V334, CheckIdentifier.Move);
                            MixedGen1NonTradebackGen2 = true;
                        }
                        else
                            res[m] = new CheckMoveResult(MoveSource.InheritLevelUp, gen, Severity.Valid, V345, CheckIdentifier.Move);
                        LvlupEggMovesLearned.Add(m);
                        if (pkm.TradebackStatus == TradebackType.Any && pkm.GenNumber == 1)
                            pkm.TradebackStatus = TradebackType.WasTradeback;
                    }

                    // Check egg moves after all the generations and all the moves, every move that can't be learned in another source should have preference
                    // the moves that can only be learned from egg moves should in the future check if the move combinations can be breed in gens 2 to 5
                    for (int m = 0; m < 4; m++)
                    {
                        if (res[m]?.Valid ?? false)
                            continue;
                        if (moves[m] == 0)
                            continue;

                        if (egg.Contains(moves[m]))
                        {
                            if (IsGen2Pkm && Gen1MovesLearned.Any() && moves[m] > Legal.MaxMoveID_1)
                            {
                                // To learn exclusive generation 1 moves the pokemon was tradeback, but it can't be trade to generation 1
                                // without removing moves above MaxMoveID_1, egg moves above MaxMoveID_1 and gen 1 moves are incompatible
                                res[m] = new CheckMoveResult(MoveSource.EggMove, gen, Severity.Invalid, V334, CheckIdentifier.Move) { Flag = true };
                                MixedGen1NonTradebackGen2 = true;
                            }
                            else
                                res[m] = new CheckMoveResult(MoveSource.EggMove, gen, Severity.Valid, V171, CheckIdentifier.Move) { Flag = true };

                            EggMovesLearned.Add(m);
                            if (pkm.TradebackStatus == TradebackType.Any && pkm.GenNumber == 1)
                                pkm.TradebackStatus = TradebackType.WasTradeback;
                        }
                        if (!eventegg.Contains(moves[m]))
                            continue;

                        if (!egg.Contains(moves[m]))
                        {
                            if (IsGen2Pkm && Gen1MovesLearned.Any() && moves[m] > Legal.MaxMoveID_1)
                            {
                                res[m] = new CheckMoveResult(MoveSource.SpecialEgg, gen, Severity.Invalid, V334, CheckIdentifier.Move) { Flag = true };
                                MixedGen1NonTradebackGen2 = true;
                            }
                            else
                                res[m] = new CheckMoveResult(MoveSource.SpecialEgg, gen, Severity.Valid, V333, CheckIdentifier.Move) { Flag = true };
                        }
                        if (pkm.TradebackStatus == TradebackType.Any && pkm.GenNumber == 1)
                            pkm.TradebackStatus = TradebackType.WasTradeback;
                        EventEggMovesLearned.Add(m);
                    }

                    // A pokemon could have normal egg moves and regular egg moves
                    // Only if all regular egg moves are event egg moves or all event egg moves are regular egg moves
                    var RegularEggMovesLearned = EggMovesLearned.Union(LvlupEggMovesLearned).ToList();
                    if (RegularEggMovesLearned.Any() && EventEggMovesLearned.Any())
                    {
                        // Moves that are egg moves or event egg moves but not both
                        var IncompatibleEggMoves = RegularEggMovesLearned.Except(EventEggMovesLearned).Union(EventEggMovesLearned.Except(RegularEggMovesLearned)).ToList();
                        if (IncompatibleEggMoves.Any())
                        {
                            foreach (int m in IncompatibleEggMoves)
                            {
                                if (EventEggMovesLearned.Contains(m) && !EggMovesLearned.Contains(m))
                                    res[m] = new CheckMoveResult(res[m], Severity.Invalid, V337, CheckIdentifier.Move);
                                else if (!EventEggMovesLearned.Contains(m) && EggMovesLearned.Contains(m))
                                    res[m] = new CheckMoveResult(res[m], Severity.Invalid, V336, CheckIdentifier.Move);
                                else if (!EventEggMovesLearned.Contains(m) && LvlupEggMovesLearned.Contains(m))
                                    res[m] = new CheckMoveResult(res[m], Severity.Invalid, V358, CheckIdentifier.Move);
                            }
                        }
                    }
                    // If there is no incompatibility with event egg check that there is no inherited move in gift eggs and event eggs
                    else if (RegularEggMovesLearned.Any() && (pkm.WasGiftEgg || pkm.WasEventEgg))
                    {
                        foreach (int m in RegularEggMovesLearned)
                        {
                            if (EggMovesLearned.Contains(m))
                                res[m] = new CheckMoveResult(res[m], Severity.Invalid, pkm.WasGiftEgg ? V377 : V341, CheckIdentifier.Move);
                            else if (LvlupEggMovesLearned.Contains(m))
                                res[m] = new CheckMoveResult(res[m], Severity.Invalid, pkm.WasGiftEgg ? V378 : V347, CheckIdentifier.Move);
                        }
                    }
                }

                if (3 <= gen && gen <= 4 && pkm.Format > gen)
                {
                    // After all the moves from the generations 3 and 4, 
                    // including egg moves if is the origin generation because some hidden moves are also special egg moves in gen 3
                    // Check if the marked hidden moves that were invalid at the start are now marked as valid, that means 
                    // the hidden move was learned in gen 3 or 4 but was not removed when transfer to 4 or 5
                    if (KnowDefogWhirlpool)
                    {
                        int invalidCount = moves.Where((m, i) => (m == 250 || m == 432) && (res[i]?.Valid ?? false)).Count();
                        if (invalidCount == 2) // can't know both at the same time
                            for (int i = 0; i < 4; i++) // flag both moves
                                if (moves[i] == 250 || moves[i] == 432)
                                    res[i] = new CheckMoveResult(res[i], Severity.Invalid, V338, CheckIdentifier.Move);
                    }

                    for (int i = 0; i < HMLearned.Length; i++)
                        if (res[i]?.Valid ?? false)
                            res[i] = new CheckMoveResult(res[i], Severity.Invalid, string.Format(V339, gen, gen + 1), CheckIdentifier.Move);
                }

                // Mark the gen 1 exclusive moves as illegal because the pokemon also have Non tradeback egg moves.
                if (MixedGen1NonTradebackGen2)
                { 
                    foreach (int m in Gen1MovesLearned)
                        res[m] = new CheckMoveResult(res[m], Severity.Invalid, V335, CheckIdentifier.Move);

                    foreach (int m in Gen2PreevoMovesLearned)
                        res[m] = new CheckMoveResult(res[m], Severity.Invalid, V412, CheckIdentifier.Move);
                }

                if (gen == 1 && pkm.Format == 1 && pkm.Gen1_NotTradeback)
                {
                    // Check moves learned at the same level in red/blue and yellow, illegal because there is no move reminder
                    // Only two incompatibilites and only there are no illegal combination if generation 2 or 7 are included in the analysis
                    ParseRedYellowIncompatibleMoves(pkm, res, moves);

                    ParseEvolutionsIncompatibleMoves(pkm, res, moves, info.EncounterMoves.TMHMMoves[1]);
                }

                if (Legal.SpeciesEvolutionWithMove.Contains(pkm.Species))
                {
                    // Pokemon that evolved by leveling up while learning a specific move
                    // This pokemon could only have 3 moves from preevolutions that are not the move used to evolved
                    // including special and eggs moves before realearn generations
                    ParseEvolutionLevelupMove(pkm, res, moves, IncenseMovesLearned, info);
                }

                if (res.All(r => r != null))
                    return res;
            }

            if (pkm.Species == 292 && info.EncounterMatch.Species != 292)
            {
                // Ignore Shedinja if the Encounter was also a Shedinja, assume null Encounter as a Nincada egg
                // Check Shedinja evolved moves from Ninjask after egg moves
                // Those moves could also be inherited egg moves
                ParseShedinjaEvolveMoves(pkm, res, moves);
            }

            for (int m = 0; m < 4; m++)
            {
                if (res[m] == null)
                    res[m] = new CheckMoveResult(MoveSource.Unknown, pkm.GenNumber, Severity.Invalid, V176, CheckIdentifier.Move);
            }
            return res;
        }

        private static void ParseRedYellowIncompatibleMoves(PKM pkm, IList<CheckMoveResult> res, int[] moves)
        {
            var incompatible = new List<int>();
            if (pkm.Species == 134 && pkm.CurrentLevel < 47 && moves.Contains(151))
            {
                // Vaporeon in Yellow learn Mist and Haze at level 42, Mist only if level up in day-care
                // Vaporeon in Red Blue learn Acid Armor at level 42 and level 47 in Yellow
                if (moves.Contains(54))
                    incompatible.Add(54);
                if (moves.Contains(114))
                    incompatible.Add(114);
                if (incompatible.Any())
                    incompatible.Add(151);
            }
            if (pkm.Species == 136 && pkm.CurrentLevel < 47 && moves.Contains(43) && moves.Contains(123))
            {
                // Flareon in Yellow learn Smog at level 42
                // Flareon in Red Blue learn Leer at level 42 and level 47 in Yellow
                incompatible.Add(43);
                incompatible.Add(123);
            }
            for (int m = 0; m < 4; m++)
            {
                if (incompatible.Contains(moves[m]))
                    res[m] = new CheckMoveResult(res[m], Severity.Invalid, V363, CheckIdentifier.Move);
            }
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
                var ExclusiveMoves = Legal.GetExclusiveMoves(133, pkm.Species, 1, tmhm, moves);
                var EeveeLevels = Legal.GetMinLevelLearnMove(133, 1, ExclusiveMoves[0]);
                var EvoLevels = Legal.GetMaxLevelLearnMove(pkm.Species, 1, ExclusiveMoves[1]);

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
                    res[m] = new CheckMoveResult(res[m], Severity.Invalid, string.Format(V365, currentspecies, previousspecies), CheckIdentifier.Move);
                if (incompatible_previous.Contains(moves[m]))
                    res[m] = new CheckMoveResult(res[m], Severity.Invalid, string.Format(V366, currentspecies, previousspecies), CheckIdentifier.Move);
            }
        }
        private static void ParseShedinjaEvolveMoves(PKM pkm, IList<CheckMoveResult> res, int[] moves)
        {
            List<int>[] ShedinjaEvoMoves = Legal.GetShedinjaEvolveMoves(pkm);
            var ShedinjaEvoMovesLearned = new List<int>();
            for (int gen = Math.Min(pkm.Format, 4); gen >= 3; gen--)
            {
                bool native = gen == pkm.Format;
                for (int m = 0; m < 4; m++)
                {
                    if (res[m]?.Valid ?? false)
                        continue;

                    if (!ShedinjaEvoMoves[gen].Contains(moves[m]))
                        continue;

                    res[m] = new CheckMoveResult(MoveSource.ShedinjaEvo, gen, Severity.Valid, native ? V355 : string.Format(V356, gen), CheckIdentifier.Move);
                    ShedinjaEvoMovesLearned.Add(m);
                }
            }

            if (ShedinjaEvoMovesLearned.Count <= 1)
                return;

            foreach (int m in ShedinjaEvoMovesLearned)
                res[m] = new CheckMoveResult(res[m], Severity.Invalid, V357, CheckIdentifier.Move);
        }
        private static void ParseEvolutionLevelupMove(PKM pkm, IList<CheckMoveResult> res, int[] moves, List<int> IncenseMovesLearned, LegalInfo info)
        {
            // Ignore if there is an invalid move or an empty move, this validation is only for 4 non-empty moves that are all valid, but invalid as a 4 combination
            // Ignore Mr. Mime and Sudowodoo from generations 1 to 3, they cant be evolved from Bonsly or Munchlax
            // Ignore if encounter species is the evolution species, the pokemon was not evolved by the player
            if (!res.All(r => r?.Valid ?? false) || moves.Any(m => m == 0) ||
                (Legal.BabyEvolutionWithMove.Contains(pkm.Species) && pkm.GenNumber <= 3) ||
                info.EncounterMatch.Species == pkm.Species)
                return;

            var ValidMoves = Legal.GetValidPostEvolutionMoves(pkm, pkm.Species, info.EvoChainsAllGens, GameVersion.Any);
            // Add the evolution moves to valid moves in case some of this moves could not be learned after evolving
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
                res[m] = new CheckMoveResult(res[m], Severity.Invalid, string.Format(V385, SpeciesStrings[pkm.Species]), CheckIdentifier.Move);
        }

        /* Similar to verifyRelearnEgg but in pre relearn generation is the moves what should match the expected order but only if the pokemon is inside an egg */
        private static CheckMoveResult[] VerifyPreRelearnEggBase(PKM pkm, int[] Moves, MoveInfoSet infoset, GameVersion ver)
        {
            CheckMoveResult[] res = new CheckMoveResult[4];
            var gen = pkm.GenNumber;
            // Obtain level1 moves
            int baseCt = infoset.BaseMoves.Count;
            if (baseCt > 4) baseCt = 4;

            // Obtain Inherited moves
            var inherited = Moves.Where(m => m != 0 && (!infoset.BaseMoves.Contains(m) || infoset.SpecialMoves.Contains(m) || infoset.EggMoves.Contains(m) || infoset.LvlMoves.Contains(m) || infoset.TMHMMoves.Contains(m) || infoset.TutorMoves.Contains(m))).ToList();
            int inheritCt = inherited.Count;

            // Get required amount of base moves
            int unique = infoset.BaseMoves.Concat(inherited).Distinct().Count();
            int reqBase = inheritCt == 4 || baseCt + inheritCt > 4 ? 4 - inheritCt : baseCt;
            if (Moves.Where(m => m != 0).Count() < Math.Min(4, infoset.BaseMoves.Count))
                reqBase = Math.Min(4, unique);

            var em = string.Empty;
            // Check if the required amount of Base Egg Moves are present.
            for (int i = 0; i < reqBase; i++)
            {
                if (infoset.BaseMoves.Contains(Moves[i]))
                {
                    res[i] = new CheckMoveResult(MoveSource.Initial, gen, Severity.Valid, V179, CheckIdentifier.Move);
                    continue;
                }

                // mark remaining base egg moves missing
                for (int z = i; z < reqBase; z++)
                    res[z] = new CheckMoveResult(MoveSource.Initial, gen, Severity.Invalid, V180, CheckIdentifier.Move);

                // provide the list of suggested base moves for the last required slot
                em = string.Join(", ", infoset.BaseMoves.Select(m => m >= MoveStrings.Length ? V190 : MoveStrings[m]));
                break;
            }

            int moveoffset = reqBase;
            int endSpecial = moveoffset + infoset.SpecialMoves.Count;
            // Check also if the required amount of Special Egg Moves are present, ir are after base moves
            for (int i = moveoffset; i < endSpecial; i++)
            {
                if (infoset.SpecialMoves.Contains(Moves[i]))
                {
                    res[i] = new CheckMoveResult(MoveSource.SpecialEgg, gen, Severity.Valid, V333, CheckIdentifier.Move);
                    continue;
                }

                // Not in special moves, mark remaining special egg moves missing
                for (int z = i; z < endSpecial; z++)
                    res[z] = new CheckMoveResult(MoveSource.SpecialEgg, gen, Severity.Invalid, V342, CheckIdentifier.Move);

                // provide the list of suggested base moves and species moves for the last required slot
                if (!string.IsNullOrEmpty(em)) em += ", ";
                else
                    em = string.Join(", ", infoset.BaseMoves.Select(m => m >= MoveStrings.Length ? V190 : MoveStrings[m])) + ", ";
                em += string.Join(", ", infoset.SpecialMoves.Select(m => m >= MoveStrings.Length ? V190 : MoveStrings[m]));
                break;
            }

            if (!string.IsNullOrEmpty(em))
                res[reqBase > 0 ? reqBase - 1 : 0].Comment = string.Format(Environment.NewLine + V343, em);
            // Non-Base moves that can magically appear in the regular movepool
            if (pkm.GenNumber >= 3 && Legal.LightBall.Contains(pkm.Species))
                infoset.EggMoves.Add(344);

            // Inherited moves appear after the required base moves.
            var AllowInheritedSeverity = infoset.AllowInherited ? Severity.Valid : Severity.Invalid;
            for (int i = reqBase + infoset.SpecialMoves.Count; i < 4; i++)
            {
                if (Moves[i] == 0) // empty
                    res[i] = new CheckMoveResult(MoveSource.None, gen, Severity.Valid, V167, CheckIdentifier.Move);
                else if (infoset.EggMoves.Contains(Moves[i])) // inherited egg move
                    res[i] = new CheckMoveResult(MoveSource.EggMove, gen, AllowInheritedSeverity, infoset.AllowInherited ? V344 : V341, CheckIdentifier.Move);
                else if (infoset.LvlMoves.Contains(Moves[i])) // inherited lvl moves
                    res[i] = new CheckMoveResult(MoveSource.InheritLevelUp, gen, AllowInheritedSeverity, infoset.AllowInherited ? V345 : V347, CheckIdentifier.Move);
                else if (infoset.TMHMMoves.Contains(Moves[i])) // inherited TMHM moves
                    res[i] = new CheckMoveResult(MoveSource.TMHM, gen, AllowInheritedSeverity, infoset.AllowInherited ? V349 : V350, CheckIdentifier.Move);
                else if (infoset.TutorMoves.Contains(Moves[i])) // inherited tutor moves
                    res[i] = new CheckMoveResult(MoveSource.Tutor, gen, AllowInheritedSeverity, infoset.AllowInherited ? V346 : V348, CheckIdentifier.Move);
                else // not inheritable, flag
                    res[i] = new CheckMoveResult(MoveSource.Unknown, gen, Severity.Invalid, V340, CheckIdentifier.Move);
            }

            return res;
        }

        private static void VerifyNoEmptyDuplicates(int[] Moves, CheckMoveResult[] res)
        {
            bool emptySlot = false;
            for (int i = 0; i < 4; i++)
            {
                if (Moves[i] == 0)
                    emptySlot = true;
                else if (emptySlot)
                    res[i] = new CheckMoveResult(res[i], Severity.Invalid, V167, res[i].Identifier);
                else if (Moves.Count(m => m == Moves[i]) > 1)
                    res[i] = new CheckMoveResult(res[i], Severity.Invalid, V168, res[i].Identifier);
            }
        }
        private static void UptateGen1LevelUpMoves(PKM pkm, ValidEncounterMoves EncounterMoves, int defaultLvlG1, int generation, LegalInfo info)
        {
            switch (generation)
            {
                case 1:
                case 2:
                    var lvlG1 = info.EncounterMatch?.LevelMin + 1 ?? 6;
                    if (lvlG1 != defaultLvlG1)
                        EncounterMoves.LevelUpMoves[1] = Legal.GetValidMoves(pkm, info.EvoChainsAllGens[1], generation: 1, minLvLG1: lvlG1, LVL: true, Tutor: false, Machine: false, MoveReminder: false).ToList();
                    break;
            }
        }
        private static void UptateGen2LevelUpMoves(PKM pkm, ValidEncounterMoves EncounterMoves, int defaultLvlG2, int generation, LegalInfo info)
        {
            switch (generation)
            {
                case 1:
                case 2:
                    var lvlG2 = info.EncounterMatch?.LevelMin + 1 ?? 6;
                    if (lvlG2 != defaultLvlG2)
                        EncounterMoves.LevelUpMoves[2] = Legal.GetValidMoves(pkm, info.EvoChainsAllGens[2], generation: 2, minLvLG2: defaultLvlG2, LVL: true, Tutor: false, Machine: false, MoveReminder: false).ToList();
                    break;
            }
        }
        private static int[] GetGenMovesCheckOrder(PKM pkm)
        {
            if (pkm.Format == 1)
                return new[] { 1, 2 };
            if (pkm.Format == 2)
                return new[] { 2, 1 };
            if (pkm.Format == 7 && pkm.VC1)
                return new[] { 7, 1 };
            if (pkm.Format == 7 && pkm.VC2)
                return new[] { 7, 2, 1 };

            var order = new int[pkm.Format - pkm.GenNumber + 1];
            for (int i = 0; i < order.Length; i++)
                order[i] = pkm.Format - i;
            return order;
        }
        private static ValidEncounterMoves GetEncounterValidMoves(PKM pkm, LegalInfo info)
        {
            var minLvLG1 = pkm.GenNumber <= 2 ? info.EncounterMatch.LevelMin + 1 : 0;
            var minLvlG2 = Legal.AllowGen2MoveReminder ? 1 : info.EncounterMatch.LevelMin + 1;
            var encounterspecies = info.EncounterMatch.Species;
            var EvoChainsAllGens = info.EvoChainsAllGens;
            // If encounter species is the same species from the first match, the one in variable EncounterMatch, its evolution chains is already in EvoChainsAllGens
            var LevelMoves = Legal.GetValidMovesAllGens(pkm, EvoChainsAllGens, minLvLG1: minLvLG1, minLvLG2: minLvlG2, Tutor: false, Machine: false, RemoveTransferHM: false);
            var TMHMMoves = Legal.GetValidMovesAllGens(pkm, EvoChainsAllGens, LVL: false, Tutor: false, MoveReminder: false, RemoveTransferHM: false);
            var TutorMoves = Legal.GetValidMovesAllGens(pkm, EvoChainsAllGens, LVL: false, Machine: false, MoveReminder: false, RemoveTransferHM: false);
            return new ValidEncounterMoves
            {
                EncounterSpecies = encounterspecies,
                LevelUpMoves = LevelMoves,
                TMHMMoves = TMHMMoves,
                TutorMoves = TutorMoves,
                EvolutionChains = EvoChainsAllGens,
                MinimumLevelGen1 = minLvLG1,
                MinimumLevelGen2 = minLvlG2
            };
        }
    }
}
