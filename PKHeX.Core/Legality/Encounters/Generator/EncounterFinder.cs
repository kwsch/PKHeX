using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Finds matching <see cref="IEncounterable"/> data and relevant <see cref="LegalInfo"/> for a <see cref="PKM"/>.
    /// </summary>
    public static class EncounterFinder
    {
        /// <summary>
        /// Iterates through all possible encounters until a sufficient match is found
        /// </summary>
        /// <remarks>
        /// The iterator lazily finds matching encounters, then verifies secondary checks to weed out any nonexact matches.
        /// </remarks>
        /// <param name="pkm">Source data to find a match for</param>
        /// <returns>
        /// Information containing the matched encounter and any parsed checks.
        /// If no clean match is found, the last checked match is returned.
        /// If no match is found, an invalid encounter object is returned.
        /// </returns>
        public static LegalInfo FindVerifiedEncounter(PKM pkm)
        {
            var info = new LegalInfo(pkm);
            var encounters = EncounterGenerator.GetEncounters(pkm, info);

            using var encounter = new PeekEnumerator<IEncounterable>(encounters);
            if (!encounter.PeekIsNext())
                return VerifyWithoutEncounter(pkm, info);

            var EncounterValidator = EncounterVerifier.GetEncounterVerifierMethod(pkm);
            while (encounter.MoveNext())
            {
                info.EncounterMatch = encounter.Current;
                var e = EncounterValidator(pkm, info);
                if (!e.Valid && encounter.PeekIsNext())
                {
                    info.Reject(e);
                    continue;
                }
                info.Parse.Add(e);

                if (VerifySecondaryChecks(pkm, info, encounter))
                    break; // passes
            }

            if (!info.FrameMatches && info.EncounterMatch is EncounterSlot && pkm.Version != (int)GameVersion.CXD) // if false, all valid RNG frame matches have already been consumed
                info.Parse.Add(new CheckResult(ParseSettings.RNGFrameNotFound, LEncConditionBadRNGFrame, CheckIdentifier.PID)); // todo for further confirmation
            if (!info.PIDIVMatches) // if false, all valid PIDIV matches have already been consumed
                info.Parse.Add(new CheckResult(Severity.Invalid, LPIDTypeMismatch, CheckIdentifier.PID));

            return info;
        }

        /// <summary>
        /// Checks supplementary info to see if the encounter is still valid.
        /// </summary>
        /// <remarks>
        /// When an encounter is initially validated, only encounter-related checks are performed.
        /// By checking Moves, Evolution, and <see cref="PIDIV"/> data, a best match encounter can be found.
        /// If the encounter is not valid, the method will not reject it unless another encounter is available to check.
        /// </remarks>
        /// <param name="pkm">Source data to check the match for</param>
        /// <param name="info">Information containing the matched encounter</param>
        /// <param name="iterator">Peekable iterator </param>
        /// <returns>Indication whether or not the encounter passes secondary checks</returns>
        private static bool VerifySecondaryChecks(PKM pkm, LegalInfo info, PeekEnumerator<IEncounterable> iterator)
        {
            if (pkm.Format >= 6)
            {
                info.Relearn = VerifyRelearnMoves.VerifyRelearn(pkm, info);
                if (info.Relearn.Any(z => !z.Valid) && iterator.PeekIsNext())
                    return false;
            }
            else
            {
                for (int i = 0; i < 4; i++)
                    info.Relearn[i] = new CheckResult(CheckIdentifier.RelearnMove);
            }

            info.Moves = VerifyCurrentMoves.VerifyMoves(pkm, info);
            if (info.Moves.Any(z => !z.Valid) && iterator.PeekIsNext())
                return false;

            var evo = EvolutionVerifier.VerifyEvolution(pkm, info);
            if (!evo.Valid && iterator.PeekIsNext())
                return false;
            info.Parse.Add(evo);

            return true;
        }

        /// <summary>
        /// Returns legality info for an unmatched encounter scenario, including a hint as to what the actual match could be.
        /// </summary>
        /// <param name="pkm">Source data to check the match for</param>
        /// <param name="info">Information containing the unmatched encounter</param>
        /// <returns>Updated information pertaining to the unmatched encounter</returns>
        private static LegalInfo VerifyWithoutEncounter(PKM pkm, LegalInfo info)
        {
            info.EncounterMatch = new EncounterInvalid(pkm);
            string hint = GetHintWhyNotFound(pkm);

            info.Parse.Add(new CheckResult(Severity.Invalid, hint, CheckIdentifier.Encounter));
            info.Relearn = VerifyRelearnMoves.VerifyRelearn(pkm, info);
            info.Moves = VerifyCurrentMoves.VerifyMoves(pkm, info);
            return info;
        }

        private static string GetHintWhyNotFound(PKM pkm)
        {
            if (pkm.WasGiftEgg)
                return LEncGift;
            if (pkm.WasEventEgg)
                return LEncGiftEggEvent;
            if (pkm.WasEvent)
                return LEncGiftNotFound;
            return LEncInvalid;
        }
    }
}
