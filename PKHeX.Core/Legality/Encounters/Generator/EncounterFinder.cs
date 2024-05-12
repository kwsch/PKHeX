using System;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Finds matching <see cref="IEncounterable"/> data and relevant <see cref="LegalInfo"/> for a <see cref="PKM"/>.
/// </summary>
public static class EncounterFinder
{
    /// <summary>
    /// Iterates through all possible encounters until a sufficient match is found
    /// </summary>
    /// <remarks>
    /// The iterator lazily finds matching encounters, then verifies secondary checks to weed out any non-exact matches.
    /// </remarks>
    /// <param name="pk">Source data to find a match for</param>
    /// <param name="info">Object to store matched encounter info</param>
    /// <returns>
    /// Information containing the matched encounter and any parsed checks.
    /// If no clean match is found, the last checked match is returned.
    /// If no match is found, an invalid encounter object is returned.
    /// </returns>
    public static void FindVerifiedEncounter(PKM pk, LegalInfo info)
    {
        var encounters = EncounterGenerator.GetEncounters(pk, info);

        using var encounter = new PeekEnumerator<IEncounterable>(encounters);
        if (!encounter.PeekIsNext())
        {
            VerifyWithoutEncounter(pk, info);
            return;
        }

        var first = encounter.Current;
        var EncounterValidator = EncounterVerifier.GetEncounterVerifierMethod(first.Generation);
        while (encounter.MoveNext())
        {
            var enc = encounter.Current;

            // Check for basic compatibility.
            var e = EncounterValidator(pk, enc);
            if (!e.Valid && encounter.PeekIsNext())
                continue;

            // Looks like we might have a good enough match. Check if this is really a good match.
            info.EncounterMatch = enc;
            if (e.Comment.Length != 0)
                info.Parse.Add(e);
            if (!VerifySecondaryChecks(pk, info, encounter))
                continue;

            // Sanity Check -- Some secondary checks might not be as thorough as the partial-match leak-through checks done by the encounter.
            if (enc is not IEncounterMatch mx)
                break;

            var match = mx.GetMatchRating(pk);
            if (match < EncounterMatchRating.PartialMatch)
                break;

            // Reaching here implies the encounter wasn't valid. Try stepping to the next encounter.
            if (encounter.PeekIsNext())
                continue;

            // We ran out of possible encounters without finding a suitable match; add a message indicating that the encounter is not a complete match.
            info.Parse.Add(new CheckResult(Severity.Invalid, CheckIdentifier.Encounter, LEncInvalid));
            break;
        }

        var manual = info.ManualFlag;
        if (manual != EncounterYieldFlag.None)
        {
            if (!info.FrameMatches) // if false, all valid RNG frame matches have already been consumed
                info.Parse.Add(new CheckResult(ParseSettings.Settings.FramePattern.GetSeverity(info.Generation), CheckIdentifier.PID, LEncConditionBadRNGFrame));
            else if (!info.PIDIVMatches) // if false, all valid PIDIV matches have already been consumed
                info.Parse.Add(new CheckResult(Severity.Invalid, CheckIdentifier.PID, LPIDTypeMismatch));
        }
    }

    /// <summary>
    /// Checks supplementary info to see if the encounter is still valid.
    /// </summary>
    /// <remarks>
    /// When an encounter is initially validated, only encounter-related checks are performed.
    /// By checking Moves, Evolution, and <see cref="PIDIV"/> data, the best match encounter can be found.
    /// If the encounter is not valid, the method will not reject it unless another encounter is available to check.
    /// </remarks>
    /// <param name="pk">Source data to check the match for</param>
    /// <param name="info">Information containing the matched encounter</param>
    /// <param name="iterator">Peekable iterator </param>
    /// <returns>Indication whether the encounter passes secondary checks</returns>
    private static bool VerifySecondaryChecks(PKM pk, LegalInfo info, PeekEnumerator<IEncounterable> iterator)
    {
        var relearn = info.Relearn.AsSpan();
        if (pk.Format >= 6)
        {
            LearnVerifierRelearn.Verify(relearn, info.EncounterOriginal, pk);
            if (!MoveResult.AllValid(relearn) && iterator.PeekIsNext())
                return false;
        }
        else
        {
            // Dummy to something valid.
            relearn.Fill(MoveResult.Relearn);
        }

        var moves = info.Moves.AsSpan();
        LearnVerifier.Verify(moves, pk, info.EncounterMatch, info.EvoChainsAllGens);
        if (!MoveResult.AllValid(moves) && iterator.PeekIsNext())
            return false;

        if (!info.Parse.TrueForAll(static z => z.Valid) && iterator.PeekIsNext())
            return false;

        var evo = EvolutionVerifier.VerifyEvolution(pk, info);
        if (!evo.Valid && iterator.PeekIsNext())
            return false;

        // Memories of Knowing a move which is later forgotten can be problematic with encounters that have special moves.
        if (pk is ITrainerMemories m)
        {
            if (m is IMemoryOT o && MemoryPermissions.IsMemoryOfKnownMove(o.OriginalTrainerMemory))
            {
                var mem = MemoryVariableSet.Read(m, 0);
                bool valid = MemoryPermissions.CanKnowMove(pk, mem, info.EncounterMatch.Context, info);
                if (!valid && iterator.PeekIsNext())
                    return false;
            }
            if (m is IMemoryHT h && MemoryPermissions.IsMemoryOfKnownMove(h.HandlingTrainerMemory) && !pk.HasMove(h.HandlingTrainerMemoryVariable))
            {
                var mem = MemoryVariableSet.Read(m, 1);

                var sources = MemoryRules.GetPossibleSources(info.EvoChainsAllGens);
                sources = MemoryRules.ReviseSourcesHandler(pk, sources, info.EncounterOriginal);

                bool valid = (sources.HasFlag(MemorySource.Gen6) && MemoryPermissions.CanKnowMove(pk, mem, EntityContext.Gen6, info))
                          || (sources.HasFlag(MemorySource.Gen8) && MemoryPermissions.CanKnowMove(pk, mem, EntityContext.Gen8, info));
                if (!valid && iterator.PeekIsNext())
                    return false;
            }
        }
        else if (pk is PK1 pk1)
        {
            var hasGen2 = MoveInfo.IsAnyFromGeneration(2, info.Moves);
            if (hasGen2)
            {
                if (!ParseSettings.AllowGen1Tradeback)
                    return false;
                if (!PK1.IsCatchRateHeldItem(pk1.CatchRate))
                    return false;
            }
        }

        info.Parse.Add(evo);
        return true;
    }

    /// <summary>
    /// Returns legality info for an unmatched encounter scenario, including a hint as to what the actual match could be.
    /// </summary>
    /// <param name="pk">Source data to check the match for</param>
    /// <param name="info">Information containing the unmatched encounter</param>
    /// <returns>Updated information pertaining to the unmatched encounter</returns>
    private static void VerifyWithoutEncounter(PKM pk, LegalInfo info)
    {
        info.EncounterMatch = new EncounterInvalid(pk);
        string hint = GetHintWhyNotFound(pk, info.EncounterMatch.Generation);

        info.Parse.Add(new CheckResult(Severity.Invalid, CheckIdentifier.Encounter, hint));
        LearnVerifierRelearn.Verify(info.Relearn, info.EncounterOriginal, pk);
        LearnVerifier.Verify(info.Moves, pk, info.EncounterMatch, info.EvoChainsAllGens);
    }

    private static string GetHintWhyNotFound(PKM pk, byte generation)
    {
        if (WasGiftEgg(pk, generation, pk.EggLocation))
            return LEncGift;
        if (WasEventEgg(pk, generation))
            return LEncGiftEggEvent;
        if (WasEvent(pk, generation))
            return LEncGiftNotFound;
        return LEncInvalid;
    }

    private static bool WasGiftEgg(PKM pk, byte generation, ushort eggLocation) => !pk.FatefulEncounter && generation switch
    {
        3 => pk.IsEgg && (byte)pk.MetLocation == 253, // Gift Egg, indistinguishable from normal eggs after hatch
        4 => eggLocation - 2009u <= (2014 - 2009) || (pk.Format != 4 && (eggLocation == Locations.Faraway4 && pk.HGSS)),
        5 => eggLocation is Locations.Breeder5,
        _ => eggLocation is Locations.Breeder6,
    };

    private static bool WasEventEgg(PKM pk, byte generation) => generation switch
    {
        // Event Egg, indistinguishable from normal eggs after hatch
        // can't tell after transfer
        3 => pk is { Context: EntityContext.Gen3, IsEgg: true } && Locations.IsEventLocation3(pk.MetLocation),

        // Manaphy was the only generation 4 released event egg
        _ => pk.FatefulEncounter && pk.EggDay != 0,
    };

    private static bool WasEvent(PKM pk, byte generation) => pk.FatefulEncounter || generation switch
    {
        3 => Locations.IsEventLocation3(pk.MetLocation) && pk.Format == 3,
        4 => Locations.IsEventLocation4(pk.MetLocation) && pk.Format == 4,
      >=5 => Locations.IsEventLocation5(pk.MetLocation),
        _ => false,
    };
}
