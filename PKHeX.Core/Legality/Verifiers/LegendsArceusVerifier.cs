using System;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Verifies the stat details of data that has not yet left <see cref="GameVersion.PLA"/>.
/// </summary>
public sealed class LegendsArceusVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.RelearnMove;

    public override void Verify(LegalityAnalysis data)
    {
        if (data.Entity is not PA8 pa)
            return;

        if (pa.IsNoble)
            data.AddLine(GetInvalid(LStatNobleInvalid));
        if (pa.IsAlpha != data.EncounterMatch is IAlpha { IsAlpha: true })
            data.AddLine(GetInvalid(LStatAlphaInvalid));

        CheckScalars(data, pa);
        CheckGanbaru(data, pa);

        CheckLearnset(data, pa);
        CheckMastery(data, pa);
    }

    private static void CheckGanbaru(LegalityAnalysis data, PA8 pa)
    {
        for (int i = 0; i < 6; i++)
        {
            var gv = pa.GetGV(i);
            var max = pa.GetMaxGanbaru(i);
            if (gv <= max)
                continue;

            data.AddLine(GetInvalid(LGanbaruStatTooHigh, CheckIdentifier.GVs));
            return;
        }
    }

    private void CheckScalars(LegalityAnalysis data, PA8 pa)
    {
        // Static encounters hard-match the Height & Weight; only slots are unchecked for Alpha Height/Weight.
        if (pa.IsAlpha && data.EncounterMatch is EncounterSlot8a)
        {
            if (pa.HeightScalar != 255)
                data.AddLine(GetInvalid(LStatIncorrectHeightValue));
            if (pa.WeightScalar != 255)
                data.AddLine(GetInvalid(LStatIncorrectWeightValue));
        }

        // No way to mutate the display height scalar value. Must match!
        if (pa.HeightScalar != pa.HeightScalarCopy)
            data.AddLine(GetInvalid(LStatIncorrectHeightCopy, CheckIdentifier.Encounter));
    }

    private static void CheckLearnset(LegalityAnalysis data, PA8 pa)
    {
        var moveCount = GetMoveCount(pa);
        if (moveCount == 4)
            return;

        // Get the bare minimum moveset.
        Span<ushort> expect = stackalloc ushort[4];
        var minMoveCount = LoadBareMinimumMoveset(data.EncounterMatch, data.Info.EvoChainsAllGens, pa, expect);

        // Flag move slots that are empty.
        var moves = data.Info.Moves;
        for (int i = moveCount; i < minMoveCount; i++)
        {
            // Expected move should never be empty, but just future-proof against any revisions.
            moves[i] = MoveResult.Unobtainable(expect[i]);
        }
    }

    /// <summary>
    /// Gets the expected minimum count of moves, and modifies the input <see cref="moves"/> with the bare minimum move IDs.
    /// </summary>
    private static int LoadBareMinimumMoveset(ISpeciesForm enc, EvolutionHistory h, PA8 pa, Span<ushort> moves)
    {
        // Get any encounter moves
        var pt = PersonalTable.LA;
        var index = pt.GetFormIndex(enc.Species, enc.Form);
        var learn = Legal.LevelUpLA;
        var moveset = learn[index];
        if (enc is IMasteryInitialMoveShop8 ms)
            ms.LoadInitialMoveset(pa, moves, moveset, pa.Met_Level);
        else
            moveset.SetEncounterMoves(pa.Met_Level, moves);
        var count = moves.IndexOf((ushort)0);
        if ((uint)count >= 4)
            return 4;

        var purchasedCount = pa.GetPurchasedCount();
        Span<ushort> purchased = stackalloc ushort[purchasedCount];
        LoadPurchasedMoves(pa, purchased);

        // If it can be leveled up in other games, level it up in other games.
        if (h.HasVisitedSWSH || h.HasVisitedBDSP)
            return count;

        // Level up to current level
        var level = pa.CurrentLevel;
        moveset.SetLevelUpMoves(pa.Met_Level, level, moves, purchased, count);
        count = moves.IndexOf((ushort)0);
        if ((uint)count >= 4)
            return 4;

        // Evolve and try
        var evos = h.Gen8a;
        for (int i = 0; i < evos.Length - 1; i++)
        {
            var evo = evos[i];
            var x = pt.GetFormIndex(evo.Species, evo.Form);
            var m = learn[x];
            m.SetEvolutionMoves(moves, purchased, count);
            count = moves.IndexOf((ushort)0);
            if ((uint)count >= 4)
                return 4;
        }

        // Any tutored moves we don't know about??
        var currentIndex = pt.GetFormIndex(evos[0].Species, evos[0].Form);
        var currentLearn = learn[currentIndex];
        return AddMasteredMissing(pa, moves, count, moveset, currentLearn, level);
    }

    private static void LoadPurchasedMoves(IMoveShop8 pa, Span<ushort> result)
    {
        int ctr = 0;
        var purchased = pa.MoveShopPermitIndexes;
        for (int i = 0; i < purchased.Length; i++)
        {
            if (pa.GetPurchasedRecordFlag(i))
                result[ctr++] = purchased[i];
        }
    }

    private static int AddMasteredMissing(PA8 pa, Span<ushort> current, int ctr, Learnset baseLearn, Learnset currentLearn, int level)
    {
        for (int i = 0; i < pa.MoveShopPermitIndexes.Length; i++)
        {
            // Buying the move tutor grants access, but does not learn the move.
            // Mastering requires the move to be present in the movepool.
            if (!pa.GetMasteredRecordFlag(i))
                continue;

            // Purchased moves can be swapped with existing moves; we're only interested in special granted moves.
            if (pa.GetPurchasedRecordFlag(i))
                continue;

            // Check if we can swap it into the moveset after it evolves.
            var move = pa.MoveShopPermitIndexes[i];
            var baseLevel = baseLearn.GetMoveLevel(move);
            var mustKnow = baseLevel is not -1 && baseLevel <= pa.Met_Level;
            if (!mustKnow && currentLearn.GetMoveLevel(move) != level)
                continue;

            if (current.IndexOf(move) == -1)
                current[ctr++] = move;
            if (ctr == 4)
                return 4;
        }
        return ctr;
    }

    private static int GetMoveCount(PKM pa)
    {
        var count = 0;
        for (int i = 0; i < 4; i++)
        {
            if (pa.GetMove(i) is not 0)
                count++;
        }
        return count;
    }

    private void CheckMastery(LegalityAnalysis data, PA8 pa)
    {
        var bits = pa.MoveShopPermitFlags;
        var moves = pa.MoveShopPermitIndexes;
        var alphaMove = pa.AlphaMove;
        if (alphaMove is not 0)
            VerifyAlphaMove(data, pa, alphaMove, moves, bits);
        else
            VerifyAlphaMoveZero(data);

        for (int i = 0; i < bits.Length; i++)
            VerifyTutorMoveIndex(data, pa, i, bits, moves);
    }

    private void VerifyTutorMoveIndex(LegalityAnalysis data, PA8 pa, int i, ReadOnlySpan<bool> bits, ReadOnlySpan<ushort> moves)
    {
        bool isPurchased = pa.GetPurchasedRecordFlag(i);
        if (isPurchased)
        {
            // Check if the move can be purchased.
            if (bits[i])
                return; // If it has been legally purchased, then any mastery state is legal.

            data.AddLine(GetInvalid(string.Format(LMoveShopPurchaseInvalid_0, ParseSettings.MoveStrings[moves[i]])));
            return;
        }

        bool isMastered = pa.GetMasteredRecordFlag(i);
        if (!isMastered)
            return; // All good.

        // Check if the move can be purchased; using a Mastery Seed checks the permission.
        if (pa.AlphaMove == moves[i])
            return; // Previously checked.
        if (data.EncounterMatch is (IMoveset m and IMasteryInitialMoveShop8) && m.Moves.Contains(moves[i]))
            return; // Previously checked.
        if (!bits[i])
            data.AddLine(GetInvalid(string.Format(LMoveShopMasterInvalid_0, ParseSettings.MoveStrings[moves[i]])));
        else if (!CanLearnMoveByLevelUp(data, pa, i, moves))
            data.AddLine(GetInvalid(string.Format(LMoveShopMasterNotLearned_0, ParseSettings.MoveStrings[moves[i]])));
    }

    private static bool CanLearnMoveByLevelUp(LegalityAnalysis data, PA8 pa, int i, ReadOnlySpan<ushort> moves)
    {
        // Check if the move can be learned in the learnset...
        // Changing forms do not have separate tutor permissions, so we don't need to bother with form changes.
        // Level up movepools can grant moves for mastery at lower levels for earlier evolutions... find the minimum.
        int level = 101;
        foreach (var evo in data.Info.EvoChainsAllGens.Gen8a)
        {
            var pt = PersonalTable.LA;
            var index = pt.GetFormIndex(evo.Species, evo.Form);
            var moveset = Legal.LevelUpLA[index];
            var lvl = moveset.GetLevelLearnMove(moves[i]);
            if (lvl == -1)
                continue; // cannot learn via level up
            level = Math.Min(lvl, level);
        }
        return pa.CurrentLevel >= level;
    }

    private void VerifyAlphaMove(LegalityAnalysis data, PA8 pa, ushort alphaMove, ReadOnlySpan<ushort> moves, ReadOnlySpan<bool> bits)
    {
        if (!pa.IsAlpha || data.EncounterMatch is EncounterSlot8a { Type: SlotType.Landmark })
        {
            data.AddLine(GetInvalid(LMoveShopAlphaMoveShouldBeZero));
            return;
        }
        if (!CanMasterMoveFromMoveShop(alphaMove, moves, bits))
        {
            data.AddLine(GetInvalid(LMoveShopAlphaMoveShouldBeOther));
            return;
        }

        // An Alpha Move must be marked as mastered.
        var masteredIndex = moves.IndexOf(alphaMove);
        // Index is already >= 0, implicitly via the above call not returning false.
        if (!pa.GetMasteredRecordFlag(masteredIndex))
            data.AddLine(GetInvalid(LMoveShopAlphaMoveShouldBeMastered));
    }

    private void VerifyAlphaMoveZero(LegalityAnalysis data)
    {
        var enc = data.Info.EncounterMatch;
        if (enc is not IAlpha { IsAlpha: true })
            return; // okay

        if (enc is EncounterSlot8a { Type: SlotType.Landmark })
            return; // okay

        var pi = PersonalTable.LA.GetFormEntry(enc.Species, enc.Form);
        var tutors = pi.SpecialTutors[0];
        bool hasAnyTutor = Array.IndexOf(tutors, true) >= 0;
        if (hasAnyTutor) // must have had a tutor flag
            data.AddLine(GetInvalid(LMoveShopAlphaMoveShouldBeOther));
    }

    private static bool CanMasterMoveFromMoveShop(ushort move, ReadOnlySpan<ushort> moves, ReadOnlySpan<bool> bits)
    {
        var index = moves.IndexOf(move);
        if (index == -1)
            return false; // not in the list
        if (!bits[index])
            return false; // not a possible move
        return true;
    }
}
