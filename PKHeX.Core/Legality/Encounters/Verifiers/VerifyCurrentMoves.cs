using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.ParseSettings;

using static PKHeX.Core.LearnMethod;

namespace PKHeX.Core;

/// <summary>
/// Logic to verify the current <see cref="PKM.Moves"/>.
/// </summary>
public static class VerifyCurrentMoves
{
    /// <summary>
    /// Verifies the current moves of the <see cref="pk"/> data based on the provided <see cref="info"/>.
    /// </summary>
    /// <param name="pk">Data to check</param>
    /// <param name="info">Encounter conditions and legality info</param>
    /// <returns>Validity of the <see cref="PKM.Moves"/></returns>
    public static void VerifyMoves(PKM pk, LegalInfo info)
    {
        var parse = info.Moves.AsSpan();
        parse.Clear();
        var currentMoves = pk.Moves;
        ParseMovesForEncounters(pk, parse, info, currentMoves);

        // Duplicate Moves Check
        VerifyNoEmptyDuplicates(currentMoves, parse);
        if (currentMoves[0] == 0) // Can't have an empty move slot for the first move.
            parse[0] = MoveResult.EmptyInvalid;
    }

    private static void ParseMovesForEncounters(PKM pk, Span<MoveResult> parse, LegalInfo info, int[] currentMoves)
    {
        if (pk.Species == (int)Species.Smeargle) // special handling for Smeargle
        {
            ParseMovesForSmeargle(pk, parse, currentMoves, info); // Smeargle can have any moves except a few
            return;
        }

        // gather valid moves for encounter species
        info.EncounterMoves = new ValidEncounterMoves(pk, info.EncounterMatch, info.EvoChainsAllGens);

        if (info.Generation >= 6)
            ParseMoves3DS(pk, parse, currentMoves, info);
        else
            ParseMovesPre3DS(pk, parse, currentMoves, info);
    }

    private static void ParseMovesForSmeargle(PKM pk, Span<MoveResult> parse, int[] currentMoves, LegalInfo info)
    {
        if (!pk.IsEgg)
        {
            ParseMovesSketch(pk, parse, currentMoves);
            return;
        }

        // can only know sketch as egg
        var levelup = new int[info.EvoChainsAllGens.Length][];
        levelup[pk.Format] = new[] { (int)Move.Sketch };
        info.EncounterMoves = new ValidEncounterMoves(levelup);
        var source = new MoveParseSource { CurrentMoves = currentMoves };
        ParseMoves(pk, source, info, parse);
    }

    private static void ParseMovesWasEggPreRelearn(PKM pk, Span<MoveResult> parse, int[] currentMoves, LegalInfo info, EncounterEgg e)
    {
        // Level up moves could not be inherited if Ditto is parent,
        // that means genderless species and male only species (except Nidoran-M and Volbeat; they breed with Nidoran-F and Illumise) could not have level up moves as an egg
        var AllowLevelUp = Breeding.GetCanInheritMoves(e.Species);
        int BaseLevel = AllowLevelUp ? 100 : e.LevelMin;
        var LevelUp = MoveList.GetBaseEggMoves(pk, e.Species, e.Form, e.Version, BaseLevel);

        var TradebackPreevo = pk.Format == 2 && e.Species > 151;
        var NonTradebackLvlMoves = TradebackPreevo
            ? MoveList.GetExclusivePreEvolutionMoves(pk, e.Species, info.EvoChainsAllGens.Gen2, 2, e.Version).Where(m => m > Legal.MaxMoveID_1).ToArray()
            : Array.Empty<int>();

        var Egg = MoveEgg.GetEggMoves(e.Species, e.Form, e.Version, e.Generation);
        if (info.Generation < 3 && pk.Format >= 7 && pk.VC1)
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
        ParseMoves(pk, source, info, parse);
    }

    private static void ParseMovesSketch(PKM pk, Span<MoveResult> parse, ReadOnlySpan<int> currentMoves)
    {
        for (int i = parse.Length - 1; i >= 0; i--)
        {
            var move = currentMoves[i];
            if (move == 0)
                parse[i] = MoveResult.Empty;
            else if (Legal.IsValidSketch(move, pk.Format))
                parse[i] = MoveResult.Sketch;
            else
                parse[i] = MoveResult.Unobtainable();
        }
    }

    private static void ParseMoves3DS(PKM pk, Span<MoveResult> parse, int[] currentMoves, LegalInfo info)
    {
        info.EncounterMoves.Relearn = info.Generation >= 6 ? pk.RelearnMoves : Array.Empty<int>();
        if (info.EncounterMatch is IMoveset)
            ParseMovesSpecialMoveset(pk, currentMoves, info, parse);
        else
            ParseMovesRelearn(pk, currentMoves, info, parse);
    }

    private static void ParseMovesPre3DS(PKM pk, Span<MoveResult> parse, int[] currentMoves, LegalInfo info)
    {
        if (info.EncounterMatch is EncounterEgg e)
        {
            if (pk.IsEgg)
                VerifyRelearnMoves.VerifyEggMoveset(e, parse, currentMoves);
            else
                ParseMovesWasEggPreRelearn(pk, parse, currentMoves, info, e);
            return;
        }

        // Not all games have a re-learner. Initial moves may not fill out all 4 slots.
        int gen = info.EncounterMatch.Generation;
        if (gen == 1 || (gen == 2 && !AllowGen2MoveReminder(pk)))
            ParseMovesGenGB(pk, currentMoves, info, parse);

        ParseMovesSpecialMoveset(pk, currentMoves, info, parse);
    }

    private static void ParseMovesGenGB(PKM pk, int[] currentMoves, LegalInfo info, Span<MoveResult> parse)
    {
        var enc = info.EncounterMatch;
        var evos = info.EvoChainsAllGens[enc.Generation];
        var level = evos.Length > 0 ? evos[^1].LevelMin : enc.LevelMin;
        var InitialMoves = Array.Empty<int>();
        var games = enc.Generation == 1 ? GBRestrictions.GetGen1Versions(enc) : GBRestrictions.GetGen2Versions(enc, pk.Korean);
        foreach (var ver in games)
        {
            var VerInitialMoves = enc is IMoveset {Moves.Count: not 0 } x ? (int[])x.Moves : MoveLevelUp.GetEncounterMoves(enc.Species, 0, level, ver);
            if (VerInitialMoves.SequenceEqual(InitialMoves))
                return; // Already checked this combination, and it wasn't valid. Don't bother repeating.

            var source = new MoveParseSource
            {
                CurrentMoves = currentMoves,
                Base = VerInitialMoves,
            };
            ParseMoves(pk, source, info, parse);

            // Must have a minimum count of moves, depending on the tradeback state.
            if (pk is PK1 pk1)
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
                    var method = enc.LevelMin == pk.CurrentLevel ? Empty : EmptyFishy;
                    parse[m] = new(method);
                }
            }
            if (MoveResult.AllValid(parse))
                return;
            InitialMoves = VerInitialMoves;
        }
    }

    private static void ParseMovesSpecialMoveset(PKM pk, int[] currentMoves, LegalInfo info, Span<MoveResult> parse)
    {
        var source = new MoveParseSource
        {
            CurrentMoves = currentMoves,
            SpecialSource = GetSpecialMoves(info.EncounterMatch),
        };
        ParseMoves(pk, source, info, parse);
    }

    private static IReadOnlyList<int> GetSpecialMoves(IEncounterTemplate enc)
    {
        if (enc is IMoveset mg)
            return mg.Moves;
        return Array.Empty<int>();
    }

    private static void ParseMovesRelearn(PKM pk, int[] currentMoves, LegalInfo info, Span<MoveResult> parse)
    {
        var source = new MoveParseSource
        {
            CurrentMoves = currentMoves,
            SpecialSource = GetSpecialMoves(info.EncounterMatch),
        };

        if (info.EncounterMatch is EncounterEgg e)
            source.EggMoveSource = MoveEgg.GetEggMoves(e.Species, e.Form, e.Version, e.Generation);

        ParseMoves(pk, source, info, parse);
    }

    private static void ParseMoves(PKM pk, MoveParseSource source, LegalInfo info, Span<MoveResult> parse)
    {
        // Special considerations!
        const int NoMinGeneration = 0;
        int minGeneration = NoMinGeneration;
        if (pk.IsOriginalMovesetDeleted())
        {
            var (_, resetGame) = pk.IsMovesetRestricted();
            minGeneration = resetGame.GetGeneration();
            source.ResetSources();
        }

        // Check empty moves and relearn moves before generation specific moves
        for (int m = parse.Length - 1; m >= 0; m--)
        {
            var move = source.CurrentMoves[m];
            if (move == 0)
                parse[m] = MoveResult.Empty;
            else if (minGeneration == NoMinGeneration && info.EncounterMoves.Relearn.Contains(move))
                parse[m] = MoveResult.Relearn;
        }

        if (MoveResult.AllParsed(parse))
            return;

        // Encapsulate arguments to simplify method calls
        var moveInfo = new LearnInfo(pk, source);
        // Check moves going backwards, marking the move valid in the most current generation when it can be learned
        int[] generations = GenerationTraversal.GetVisitedGenerationOrder(pk, info.EncounterOriginal.Generation);
        if (pk.Format <= 2)
            generations = Array.FindAll(generations, z => z < info.EncounterMoves.LevelUpMoves.Length);
        if (minGeneration != NoMinGeneration)
            generations = Array.FindAll(generations, z => z >= minGeneration);

        if (generations.Length != 0)
        {
            int lastgen = generations[^1];
            foreach (var gen in generations)
            {
                ParseMovesByGeneration(pk, parse, gen, info, moveInfo, lastgen);
                if (MoveResult.AllParsed(parse))
                    return;
            }
        }

        if (pk.Species == (int)Species.Shedinja && info.Generation <= 4)
            ParseShedinjaEvolveMoves(pk, parse, source.CurrentMoves, info.EvoChainsAllGens);

        for (var i = 0; i < parse.Length; i++)
        {
            var r = parse[i];
            if (!r.IsParsed)
                parse[i] = MoveResult.Unobtainable();
        }
    }

    private static void ParseMovesByGeneration(PKM pk, Span<MoveResult> parse, int gen, LegalInfo info, LearnInfo learnInfo, int last)
    {
        Span<bool> HMLearned = stackalloc bool[parse.Length];
        bool KnowDefogWhirlpool = GetHMCompatibility(pk, parse, gen, learnInfo.Source.CurrentMoves, HMLearned);
        ParseMovesByGeneration(pk, parse, gen, info, learnInfo);

        if (gen == last)
            ParseMovesByGenerationLast(parse, learnInfo, info.EncounterMatch);

        switch (gen)
        {
            case 1 or 2:
                ParseMovesByGeneration12(pk, parse, learnInfo.Source.CurrentMoves, gen, info, learnInfo);
                break;

            case 3 or 4:
                if (pk.Format > gen)
                    FlagIncompatibleTransferHMs45(parse, learnInfo.Source.CurrentMoves, gen, HMLearned, KnowDefogWhirlpool);
                break;
        }
    }

    private static void ParseMovesByGeneration(PKM pk, Span<MoveResult> parse, int gen, LegalInfo info, LearnInfo learnInfo)
    {
        var moves = learnInfo.Source.CurrentMoves;
        bool native = gen == pk.Format;
        for (int m = parse.Length - 1; m >= 0; m--)
        {
            ref var r = ref parse[m];
            if (r.Valid) // already validated with another generation
                continue;
            int move = moves[m];
            if (move == 0)
                continue;

            if (gen <= 2)
            {
                if (gen == 2 && !native && move > Legal.MaxMoveID_1 && pk.VC1)
                {
                    r = new(Unobtainable) { Generation = (byte)gen };
                    continue;
                }
                if (gen == 2 && learnInfo.Source.EggMoveSource.Contains(move))
                    r = new(EggMove) { Generation = (byte)gen };
                else if (learnInfo.Source.Base.Contains(move))
                    r = new(Initial) { Generation = (byte)gen };
            }
            if (info.EncounterMoves.LevelUpMoves[gen].Contains(move))
                r = new(LevelUp) { Generation = (byte)gen };
            else if (info.EncounterMoves.TMHMMoves[gen].Contains(move))
                r = new(TMHM) { Generation = (byte)gen };
            else if (info.EncounterMoves.TutorMoves[gen].Contains(move))
                r = new(Tutor) { Generation = (byte)gen };
            else if (gen == info.Generation && learnInfo.Source.SpecialSource.Contains(move))
                r = new(Special) { Generation = (byte)gen };
            else if (gen >= 8 && MoveEgg.GetIsSharedEggMove(pk, gen, move))
                r = new(Shared) { Generation = (byte)gen };

            if (gen >= 3 || !r.Valid)
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

    private static void ParseMovesByGeneration12(PKM pk, Span<MoveResult> parse, ReadOnlySpan<int> currentMoves, int gen, LegalInfo info, LearnInfo learnInfo)
    {
        // Mark the gen 1 exclusive moves as illegal because the pokemon also have Non tradeback egg moves.
        if (learnInfo.MixedGen12NonTradeback)
        {
            foreach (int m in learnInfo.Gen1Moves)
                parse[m] = new(Unobtainable, LearnEnvironment.RB); // LG1MoveExclusive

            foreach (int m in learnInfo.Gen2PreevoMoves)
                parse[m] = new(Unobtainable, LearnEnvironment.RB); // LG1TradebackPreEvoMove
        }

        if (gen == 1 && pk.Format == 1 && !AllowGen1Tradeback)
        {
            ParseRedYellowIncompatibleMoves(pk, parse, currentMoves);
            ParseEvolutionsIncompatibleMoves(pk, parse, currentMoves, info.EncounterMoves.TMHMMoves[1].ToArray());
        }
    }

    private static void ParseMovesByGenerationLast(Span<MoveResult> parse, LearnInfo learnInfo, IEncounterTemplate enc)
    {
        int gen = enc.Generation;
        ParseEggMovesInherited(parse, gen, learnInfo);
        ParseEggMoves(parse, gen, learnInfo);
    }

    private static void ParseEggMovesInherited(Span<MoveResult> parse, int gen, LearnInfo learnInfo)
    {
        var moves = learnInfo.Source.CurrentMoves;
        // Check higher-level moves after all the moves but just before egg moves to differentiate it from normal level up moves
        // Also check if the base egg moves is a non tradeback move
        for (int m = parse.Length - 1; m >= 0; m--)
        {
            ref var r = ref parse[m];
            if (r.Valid) // already validated
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
                r = new(Unobtainable, LearnEnvironment.RB) { Generation = (byte)gen }; // LG1MoveTradeback
                learnInfo.MixedGen12NonTradeback = true;
            }
            else
            {
                r = new(InheritLevelUp, LearnEnvironment.RB) { Generation = (byte)gen };
            }
            if (gen == 2 && learnInfo.Gen1Moves.Contains(m))
                learnInfo.Gen1Moves.Remove(m);
        }
    }

    private static void ParseEggMoves(Span<MoveResult> parse, int gen, LearnInfo learnInfo)
    {
        var moves = learnInfo.Source.CurrentMoves;
        // Check egg moves after all the generations and all the moves, every move that can't be learned in another source should have preference
        // the moves that can only be learned from egg moves should in the future check if the move combinations can be breed in gens 2 to 5
        for (int m = parse.Length - 1; m >= 0; m--)
        {
            ref var r = ref parse[m];
            if (r.Valid)
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
                    r = new(Unobtainable, LearnEnvironment.RB) { Generation = (byte)gen }; // LG1MoveTradeback
                    learnInfo.MixedGen12NonTradeback = true;
                }
                else
                {
                    r = new(EggMove, LearnEnvironment.RB) { Generation = (byte)gen };
                }
            }
            if (!learnInfo.Source.EggEventSource.Contains(move))
                continue;

            if (!wasEggMove)
            {
                if (learnInfo.IsGen2Pkm && learnInfo.Gen1Moves.Count != 0 && move > Legal.MaxMoveID_1)
                {
                    r = new(Unobtainable, LearnEnvironment.RB) { Generation = (byte)gen }; // LG1MoveTradeback
                    learnInfo.MixedGen12NonTradeback = true;
                }
                else
                {
                    r = new(SpecialEgg, LearnEnvironment.RB) { Generation = (byte)gen };
                }
            }
        }
    }

    private static void ParseRedYellowIncompatibleMoves(PKM pk, Span<MoveResult> parse, ReadOnlySpan<int> currentMoves)
    {
        var incompatible = GetIncompatibleRBYMoves(pk, currentMoves);
        if (incompatible.Count == 0)
            return;
        for (int m = parse.Length - 1; m >= 0; m--)
        {
            if (incompatible.Contains(currentMoves[m]))
                parse[m] = new(Unobtainable, LearnEnvironment.RB); // LG1MoveLearnSameLevel
        }
    }

    private static IList<int> GetIncompatibleRBYMoves(PKM pk, ReadOnlySpan<int> currentMoves)
    {
        // Check moves that are learned at the same level in Red/Blue and Yellow, these are illegal because there is no Move Reminder in Gen1.
        // There are only two incompatibilities for Gen1; there are no illegal combination in Gen2.

        switch (pk.Species)
        {
            // Vaporeon in Yellow learns Mist and Haze at level 42, Mist can only be learned if it leveled up in the daycare
            // Vaporeon in Red/Blue learns Acid Armor at level 42 and level 47 in Yellow
            case (int)Species.Vaporeon when pk.CurrentLevel < 47 && currentMoves.Contains((int)Move.AcidArmor):
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
            case (int)Species.Flareon when pk.CurrentLevel < 47 && currentMoves.Contains((int)Move.Leer) && currentMoves.Contains((int)Move.Smog):
                return new[] { (int)Move.Leer, (int)Move.Smog };

            default: return Array.Empty<int>();
        }
    }

    private static void ParseEvolutionsIncompatibleMoves(PKM pk, Span<MoveResult> parse, ReadOnlySpan<int> moves, ReadOnlySpan<int> tmhm)
    {
        GBRestrictions.GetIncompatibleEvolutionMoves(pk, moves, tmhm,
            out var prevSpeciesID,
            out var incompatPrev,
            out var incompatCurr);

        if (prevSpeciesID == 0)
            return;

        for (int m = parse.Length - 1; m >= 0; m--)
        {
            if (incompatCurr.Contains(moves[m]))
                parse[m] = new(Unobtainable, LearnEnvironment.RB); // LMoveEvoFLower
            if (incompatPrev.Contains(moves[m]))
                parse[m] = new(Unobtainable, LearnEnvironment.RB); // LMoveEvoFHigher
        }
    }

    private static void ParseShedinjaEvolveMoves(PKM pk, Span<MoveResult> parse, IReadOnlyList<int> currentMoves, EvolutionHistory evos)
    {
        int shedinjaEvoMoveIndex = 0;
        var format = pk.Format;
        for (int gen = Math.Min(format, 4); gen >= 3; gen--)
        {
            if (evos[gen].Length != 2)
                continue; // Was not evolved in this generation
            if (gen == 4 && pk.Ball != 4 && !(pk.Ball == (int)Ball.Sport && pk.HGSS))
                continue; // Was definitively evolved in Gen3

            var maxLevel = pk.CurrentLevel;
            var ninjaskMoves = MoveList.GetShedinjaEvolveMoves(pk, gen, maxLevel);
            for (int m = parse.Length - 1; m >= 0; m--)
            {
                ref var r = ref parse[m];
                if (IsCheckValid(r)) // already validated
                    continue;

                if (!ninjaskMoves.Contains(currentMoves[m]))
                    continue;

                // Can't have more than one Ninjask exclusive move on Shedinja
                LearnMethod method = ShedinjaEvo;
                if (shedinjaEvoMoveIndex != 0)
                    method = Unobtainable; // LMoveNincada
                else
                    shedinjaEvoMoveIndex = m;
                r = new(method);
            }
        }

        if (shedinjaEvoMoveIndex == 0)
            return;

        // Double check that the Ninjask move level isn't less than any Nincada move level
        {
            ref var r = ref parse[shedinjaEvoMoveIndex];
            if (r.Info.Method != LevelUp)
                return;

            var move = currentMoves[shedinjaEvoMoveIndex];
            int levelS = MoveList.GetShedinjaMoveLevel((int)Species.Shedinja, move, r.Generation);
            if (levelS > 0)
                return;

            int levelN = MoveList.GetShedinjaMoveLevel((int)Species.Nincada, move, r.Generation);
            int levelJ = MoveList.GetShedinjaMoveLevel((int)Species.Ninjask, move, r.Generation);
            if (levelN > levelJ)
                r = new(Unobtainable); // LMoveEvoFHigher
        }
    }

    private static bool GetHMCompatibility(PKM pk, Span<MoveResult> parse, int gen, ReadOnlySpan<int> moves, Span<bool> HMLearned)
    {
        // Check if pokemon knows HM moves from generation 3 and 4 but are not valid yet, that means it cant learn the HMs in future generations
        if (gen == 3 && pk.Format > 3)
        {
            FlagIsHMSource(HMLearned, Legal.HM_3, parse, moves);
        }
        else if (gen == 4 && pk.Format > 4)
        {
            FlagIsHMSource(HMLearned, Legal.HM_4_RemovePokeTransfer, parse, moves);
            int count = 0;
            for (int i = 0; i < moves.Length; i++)
            {
                if (!IsCheckInvalid(parse[i]))
                    continue;
                var m = moves[i];
                bool dw = IsDefogWhirl(m);
                if (!dw)
                    continue;
                count++;
            }
            return count >= 2;
        }
        return false;

        static void FlagIsHMSource(Span<bool> flags, ICollection<int> source, Span<MoveResult> parse, ReadOnlySpan<int> moves)
        {
            for (int i = parse.Length - 1; i >= 0; i--)
                flags[i] = IsCheckInvalid(parse[i]) && source.Contains(moves[i]);
        }
    }

    private static bool IsDefogWhirl(int move) => move is (int)Move.Defog or (int)Move.Whirlpool;
    private static int GetDefogWhirlCount(Span<MoveResult> parse, IReadOnlyList<int> moves)
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

    private static bool IsCheckInvalid(in MoveResult chk) => chk.IsParsed && !chk.Valid;
    private static bool IsCheckValid(in MoveResult chk) => chk.Valid;

    private static void FlagIncompatibleTransferHMs45(Span<MoveResult> parse, IReadOnlyList<int> currentMoves, int gen, ReadOnlySpan<bool> HMLearned, bool KnowDefogWhirlpool)
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
                for (int i = parse.Length - 1; i >= 0; i--) // flag both moves
                {
                    if (IsDefogWhirl(currentMoves[i]))
                        parse[i] = new(Unobtainable); // LTransferMoveG4HM
                }
            }
        }

        // Flag moves that are only legal when learned from a past-gen HM source
        for (int i = HMLearned.Length - 1; i >= 0; i--)
        {
            if (HMLearned[i] && IsCheckValid(parse[i]))
                parse[i] = new(Unobtainable) { Generation = (byte)gen }; // LTransferMoveHM
        }
    }

    private static void VerifyNoEmptyDuplicates(ReadOnlySpan<int> moves, Span<MoveResult> parse)
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

    private static void FlagDuplicateMovesAfterIndex(ReadOnlySpan<int> moves, Span<MoveResult> parse, int index, int move)
    {
        for (int i = parse.Length - 1; i > index; i--)
        {
            if (moves[i] != move)
                continue;
            parse[index] = new(EmptyInvalid);
            return;
        }
    }

    private static void FlagEmptySlotsBeforeIndex(ReadOnlySpan<int> moves, Span<MoveResult> parse, int index)
    {
        for (int i = index - 1; i >= 0; i--)
        {
            if (moves[i] != 0)
                return;
            parse[i] = new(EmptyInvalid);
        }
    }
}
