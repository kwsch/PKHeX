using System;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public static class EncounterFinder
    {
        public static LegalInfo verifyEncounter(PKM pkm)
        {
            LegalInfo info = new LegalInfo(pkm);
            var encounters = EncounterGenerator.getEncounters(pkm, info);

            using (var encounter = new PeekEnumerator<IEncounterable>(encounters.GetEnumerator()))
            {
                if (!encounter.PeekIsNext())
                    return verifyWithoutEncounter(pkm, info);

                var EncounterValidator = getEncounterVerifier(pkm);
                while (encounter.MoveNext())
                {
                    var EncounterMatch = info.EncounterMatch = encounter.Current;

                    var e = EncounterValidator(pkm, EncounterMatch);
                    if (!e.Valid && encounter.PeekIsNext())
                        continue;
                    info.Parse.Add(e);

                    if (pkm.Format >= 6)
                    {
                        info.vRelearn = VerifyRelearnMoves.verifyRelearn(pkm, info);
                        if (info.vRelearn.Any(z => !z.Valid) && encounter.PeekIsNext())
                            continue;
                    }
                    else
                        for (int i = 0; i < 4; i++)
                            info.vRelearn[i] = new CheckResult(CheckIdentifier.RelearnMove);

                    info.vMoves = VerifyCurrentMoves.verifyMoves(pkm, info);
                    if (info.vMoves.Any(z => !z.Valid) && encounter.PeekIsNext())
                        continue;

                    var evo = VerifyEvolution.verifyEvolution(pkm, EncounterMatch);
                    if (!evo.Valid && encounter.PeekIsNext())
                        continue;

                    info.Parse.Add(evo);

                    // Encounter Passes
                    break;
                }
                return info;
            }
        }

        private static LegalInfo verifyWithoutEncounter(PKM pkm, LegalInfo info)
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
            info.vRelearn = VerifyRelearnMoves.verifyRelearn(pkm, info);
            info.vMoves = VerifyCurrentMoves.verifyMoves(pkm, info);
            return info;
        }

        private static Func<PKM, IEncounterable, CheckResult> getEncounterVerifier(PKM pkm)
        {
            switch (pkm.GenNumber)
            {
                case 1:
                case 2:
                    return VerifyEncounter.verifyEncounterG12;
                default:
                    return VerifyEncounter.verifyEncounter;
            }
        }
    }
}
