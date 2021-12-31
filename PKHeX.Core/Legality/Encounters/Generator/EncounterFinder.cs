using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.Species;

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
        /// <param name="info">Object to store matched encounter info</param>
        /// <returns>
        /// Information containing the matched encounter and any parsed checks.
        /// If no clean match is found, the last checked match is returned.
        /// If no match is found, an invalid encounter object is returned.
        /// </returns>
        public static void FindVerifiedEncounter(PKM pkm, LegalInfo info)
        {
            var encounters = EncounterGenerator.GetEncounters(pkm, info);

            using var encounter = new PeekEnumerator<IEncounterable>(encounters);
            if (!encounter.PeekIsNext())
            {
                VerifyWithoutEncounter(pkm, info);
                return;
            }

            var first = encounter.Current;
            var EncounterValidator = EncounterVerifier.GetEncounterVerifierMethod(first.Generation);
            while (encounter.MoveNext())
            {
                var enc = encounter.Current;

                // Check for basic compatibility.
                var e = EncounterValidator(pkm, enc);
                if (!e.Valid && encounter.PeekIsNext())
                    continue;

                // Looks like we might have a good enough match. Check if this is really a good match.
                info.EncounterMatch = enc;
                info.Parse.Add(e);
                if (!VerifySecondaryChecks(pkm, info, encounter))
                    continue;

                // Sanity Check -- Some secondary checks might not be as thorough as the partial-match leak-through checks done by the encounter.
                if (enc is not IEncounterMatch mx)
                    break;

                var match = mx.GetMatchRating(pkm);
                if (match != EncounterMatchRating.PartialMatch)
                    break;

                // Reaching here implies the encounter wasn't valid. Try stepping to the next encounter.
                if (encounter.PeekIsNext())
                    continue;

                // We ran out of possible encounters without finding a suitable match; add a message indicating that the encounter is not a complete match.
                info.Parse.Add(new CheckResult(Severity.Invalid, LEncInvalid, CheckIdentifier.Encounter));
                break;
            }

            if (!info.FrameMatches && info.EncounterMatch is EncounterSlot {Version: not GameVersion.CXD}) // if false, all valid RNG frame matches have already been consumed
                info.Parse.Add(new CheckResult(ParseSettings.RNGFrameNotFound, LEncConditionBadRNGFrame, CheckIdentifier.PID)); // todo for further confirmation
            if (!info.PIDIVMatches) // if false, all valid PIDIV matches have already been consumed
                info.Parse.Add(new CheckResult(Severity.Invalid, LPIDTypeMismatch, CheckIdentifier.PID));
        }

        /// <summary>
        /// Iterates through all possible generations evolutions for the encounter until a sufficient match is found
        /// </summary>
        /// </summary>
        /// <param name="pkm">Source data to check the match for</param>
        /// <param name="info">Information containing the matched encounter and generation evolution</param>
        /// <returns>Indication whether or not the encounter passes secondary checks</returns>
        private static bool verifyGenerationEvolution(PKM pkm, ref LegalInfo info) => verifyGenerationEvolution(pkm, pkm.Species, info.EvoGenerationsInfo.EvoGenerations, info.EvoChainsAllGens, ref info);
        private static bool verifyGenerationEvolution(PKM pkm, int evolvedspecies, IEnumerable<int> generationsevolution, IReadOnlyList<EvoCriteria>[] chainallgens, ref LegalInfo info)
        {
            var chain = info.EvoChain;
            var gen = pkm.Generation;
            var format = pkm.Format;
            if (chain.Count <= 1 || gen == format || format <= 2 || !chain.Any(p => p.Species == evolvedspecies))
            {
                info.EvoGenerationsInfo.EvoGenerations = Enumerable.Repeat(gen, chain.Count - 1);
                // invalid pokemon, pokemon without evolutions or pokemon that has not been moved between generations
                var check = VerifySecondaryChecksEvolution(pkm, out LegalEvoGenerationsInfo EvoInfo, ref info);
                info.EvoGenerationsInfo = EvoInfo;
                return check;
            }

            var index = chain.Select((p, index) => (p.Species, index)).First(p => p.Species == evolvedspecies).index;
            var previousspecies = chain[index + 1].Species;
            var gensevolution = EvolutionChain.getGenerationsEvolution(pkm, chain, chainallgens, evolvedspecies);
            foreach (int genevolution in gensevolution)
            {
                // Iterate throught generations for second evolution or single evolution
                info.EvoGenerationsInfo.EvoChainsAllGens = EvolutionChain.GetChainsAllGensReduced(pkm, chainallgens, previousspecies, genevolution);
                info.EvoGenerationsInfo.EvoGenerations = generationsevolution.Union(new List<int>() { genevolution });
                if (chain.Last().Species == previousspecies)
                {
                    // no more evolutions to iterate
                    if (VerifySecondaryChecksEvolution(pkm, out LegalEvoGenerationsInfo EvoInfo, ref info))
                    {
                        info.EvoGenerationsInfo = EvoInfo;
                        return true;
                    }
                    if (!info.PreferredEvoGenerationsInfo.IsPreferredEvoGeneration(EvoInfo))
                        info.PreferredEvoGenerationsInfo = EvoInfo;
                    continue;
                }
                // Iterate throught the previous evolution
                if (verifyGenerationEvolution(pkm, previousspecies, info.EvoGenerationsInfo.EvoGenerations, info.EvoChainsAllGensReduced, ref info))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks supplementary info to see if the encounter and generation evolution is still valid.
        /// </summary>
        /// <param name="pkm">Source data to check the match for</param>
        /// <param name="info">Information containing the matched encounter and generation evolution</param>
        /// <returns>Indication whether or not the encounter passes secondary checks</returns>
        private static bool VerifySecondaryChecksEvolution(PKM pkm, out LegalEvoGenerationsInfo EvoInfo, ref LegalInfo info)
        {
            EvoInfo = new LegalEvoGenerationsInfo(info.EvoGenerationsInfo);
            var gen = pkm.Generation;
            var format = pkm.Format;
            EvoInfo.Moves = VerifyCurrentMoves.VerifyMoves(pkm, info);
            if (!EvoInfo.Moves.All(z => z.Valid))
                return false;

            if (gen == 3 && format is 4 or 5)
            {
                // ability check could change based on which generation the pokemon has evolved
                if (!AbilityVerifier.IsValidAbilityGen3Evolution(pkm, info))
                    return false;
            }

            if (gen < 8 && format >= 8)
            {
                if (!AbilityVerifier.IsValidAbilityPatch(pkm, info))
                    return false;
            }

            EvoInfo.ValidAbility = true;
            EvoInfo.Evolution = EvolutionVerifier.VerifyEvolution(pkm, info);

            if (!EvoInfo.Evolution.Valid)
                return false;

            // Memories of Knowing a move which is later forgotten can be problematic with encounters that have special moves.
            // The list of moves that a pokemon can move changes based on which generation the pokemon has evolved
            if (pkm is ITrainerMemories m)
            {
                if (m is IMemoryOT o && MemoryPermissions.IsMemoryOfKnownMove(o.OT_Memory))
                {
                    var mem = MemoryVariableSet.Read(m, 0);
                    if (!MemoryPermissions.CanKnowMove(pkm, mem, info.EncounterMatch.Generation, info))
                        return false;
                }
                if (m is IMemoryHT h && MemoryPermissions.IsMemoryOfKnownMove(h.HT_Memory) && !pkm.HasMove(h.HT_TextVar))
                {
                    var mem = MemoryVariableSet.Read(m, 1);
                    if (!MemoryPermissions.CanKnowMove(pkm, mem, pkm.Format, info))
                        return false;
                }
            }
            EvoInfo.ValidMemory = true;
            return true;
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
            var relearn = info.Relearn;
            if (pkm.Format >= 6)
            {
                VerifyRelearnMoves.VerifyRelearn(pkm, info.EncounterOriginal, relearn);
                if (relearn.Any(z => !z.Valid) && iterator.PeekIsNext())
                    return false;
            }
            else
            {
                for (int i = 0; i < 4; i++)
                    relearn[i] = VerifyRelearnMoves.DummyValid;
            }

            var verifygenevo = verifyGenerationEvolution(pkm, ref info);
            if (!verifygenevo && iterator.PeekIsNext())
               return false;

            if (!verifygenevo)
                info.EvoGenerationsInfo = info.PreferredEvoGenerationsInfo;

            if (info.Moves.Any(z => !z.Valid) && iterator.PeekIsNext())
                return false;

            if (info.Parse.Any(z => !z.Valid) && iterator.PeekIsNext())
                return false;

            if (!info.EvoGenerationsInfo.Evolution.Valid && iterator.PeekIsNext())
                return false;

            info.Parse.Add(info.EvoGenerationsInfo.Evolution);
            return true;
        }

        /// <summary>
        /// Returns legality info for an unmatched encounter scenario, including a hint as to what the actual match could be.
        /// </summary>
        /// <param name="pkm">Source data to check the match for</param>
        /// <param name="info">Information containing the unmatched encounter</param>
        /// <returns>Updated information pertaining to the unmatched encounter</returns>
        private static void VerifyWithoutEncounter(PKM pkm, LegalInfo info)
        {
            info.EncounterMatch = new EncounterInvalid(pkm);
            string hint = GetHintWhyNotFound(pkm, info.EncounterMatch.Generation);

            info.Parse.Add(new CheckResult(Severity.Invalid, hint, CheckIdentifier.Encounter));
            VerifyRelearnMoves.VerifyRelearn(pkm, info.EncounterOriginal, info.Relearn);
            info.EvoGenerationsInfo.Moves = VerifyCurrentMoves.VerifyMoves(pkm, info);
        }

        private static string GetHintWhyNotFound(PKM pkm, int gen)
        {
            if (WasGiftEgg(pkm, gen, pkm.Egg_Location))
                return LEncGift;
            if (WasEventEgg(pkm, gen))
                return LEncGiftEggEvent;
            if (WasEvent(pkm, gen))
                return LEncGiftNotFound;
            return LEncInvalid;
        }

        private static bool WasGiftEgg(PKM pkm, int gen, int loc) => !pkm.FatefulEncounter && gen switch
        {
            3 => pkm.IsEgg && pkm.Met_Location == 253, // Gift Egg, indistinguible from normal eggs after hatch
            4 => Legal.GiftEggLocation4.Contains(loc) || (pkm.Format != 4 && (loc == Locations.Faraway4 && pkm.HGSS)),
            5 => loc is Locations.Breeder5,
            _ => loc is Locations.Breeder6,
        };

        private static bool WasEventEgg(PKM pkm, int gen) => gen switch
        {
            // Event Egg, indistinguible from normal eggs after hatch
            // can't tell after transfer
            3 => pkm.Format == 3 && pkm.IsEgg && pkm.Met_Location == 255,

            // Manaphy was the only generation 4 released event egg
            _ => pkm.Egg_Location is not 0 && pkm.FatefulEncounter,
        };

        private static bool WasEvent(PKM pkm, int gen) => pkm.FatefulEncounter || gen switch
        {
            3 => (pkm.Met_Location == 255 && pkm.Format == 3),
            4 => (Locations.IsEventLocation4(pkm.Met_Location) && pkm.Format == 4),
          >=5 => Locations.IsEventLocation5(pkm.Met_Location),
            _ => false,
        };
    }
}
