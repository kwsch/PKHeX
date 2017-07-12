using System;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public static class EncounterFinder
    {
        public static LegalInfo FindVerifiedEncounter(PKM pkm)
        {
            LegalInfo info = new LegalInfo(pkm);
            var encounters = EncounterGenerator.GetEncounters(pkm, info);

            using (var encounter = new PeekEnumerator<IEncounterable>(encounters.GetEnumerator()))
            {
                if (!encounter.PeekIsNext())
                    return VerifyWithoutEncounter(pkm, info);

                var EncounterValidator = GetEncounterVerifierMethod(pkm);
                while (encounter.MoveNext())
                {
                    bool PIDMatch = info.PIDIVMatches;
                    info.EncounterMatch = encounter.Current;
                    var e = EncounterValidator(pkm, info);
                    if (!e.Valid && encounter.PeekIsNext())
                    {
                        info.Reject(e);
                        continue;
                    }
                    info.Parse.Add(e);

                    if (VerifySecondaryChecks(pkm, info, PIDMatch, encounter))
                        break; // passes
                }
                return info;
            }
        }

        private static bool VerifySecondaryChecks(PKM pkm, LegalInfo info, bool PIDMatch, PeekEnumerator<IEncounterable> iterator)
        {
            if (pkm.Format >= 6)
            {
                info.Relearn = VerifyRelearnMoves.VerifyRelearn(pkm, info);
                if (info.Relearn.Any(z => !z.Valid) && iterator.PeekIsNext())
                    return false;
            }
            else
                for (int i = 0; i < 4; i++)
                    info.Relearn[i] = new CheckResult(CheckIdentifier.RelearnMove);

            info.Moves = VerifyCurrentMoves.VerifyMoves(pkm, info);
            if (info.Moves.Any(z => !z.Valid) && iterator.PeekIsNext())
                return false;

            var evo = EvolutionVerifier.VerifyEvolution(pkm, info);
            if (!evo.Valid && iterator.PeekIsNext())
                return false;
            info.Parse.Add(evo);

            if (!PIDMatch)
            {
                if (iterator.PeekIsNext())
                    return false; // continue to next
                info.Parse.Add(new CheckResult(Severity.Invalid, V411, CheckIdentifier.PID));
            }
            return true;
        }
        private static LegalInfo VerifyWithoutEncounter(PKM pkm, LegalInfo info)
        {
            info.EncounterMatch = new EncounterInvalid(pkm);

            string hint; // hint why an encounter was not found
            if (pkm.WasGiftEgg)
                hint = V359; 
            else if (pkm.WasEventEgg)
                hint = V360;
            else if (pkm.WasEvent)
                hint = V78;
            else
                hint = V80;

            info.Parse.Add(new CheckResult(Severity.Invalid, hint, CheckIdentifier.Encounter));
            info.Relearn = VerifyRelearnMoves.VerifyRelearn(pkm, info);
            info.Moves = VerifyCurrentMoves.VerifyMoves(pkm, info);
            return info;
        }
        private static Func<PKM, LegalInfo, CheckResult> GetEncounterVerifierMethod(PKM pkm)
        {
            switch (pkm.GenNumber)
            {
                case 1:
                case 2:
                    return EncounterVerifier.VerifyEncounterG12;
                default:
                    return EncounterVerifier.VerifyEncounter;
            }
        }
    }
}
