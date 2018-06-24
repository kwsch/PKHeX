using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public sealed class EffortValueVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.EVs;
        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            var EncounterMatch = data.EncounterMatch;
            var evs = pkm.EVs;
            int sum = pkm.EVTotal;
            if (sum > 0 && pkm.IsEgg)
                data.AddLine(GetInvalid(V22, CheckIdentifier.EVs));
            if (pkm.Format >= 3 && sum > 510)
                data.AddLine(GetInvalid(V25, CheckIdentifier.EVs));
            if (pkm.Format >= 6 && evs.Any(ev => ev > 252))
                data.AddLine(GetInvalid(V26, CheckIdentifier.EVs));
            if (pkm.Format == 4 && pkm.Gen4 && EncounterMatch.LevelMin == 100)
            {
                // Cannot EV train at level 100 -- Certain events are distributed at level 100.
                if (evs.Any(ev => ev > 100)) // EVs can only be increased by vitamins to a max of 100.
                    data.AddLine(GetInvalid(V367, CheckIdentifier.EVs));
            }
            else if (pkm.Format < 5)
            {
                // In Generations I and II, when a Pokémon is taken out of the Day Care, its experience will lower to the minimum value for its current level.
                if (pkm.Format < 3) // can abuse daycare for EV training without EXP gain
                    return;

                const int maxEV = 100; // Vitamin Max
                if (PKX.GetEXP(EncounterMatch.LevelMin, pkm.Species) == pkm.EXP && evs.Any(ev => ev > maxEV))
                    data.AddLine(GetInvalid(string.Format(V418, maxEV), CheckIdentifier.EVs));
            }

            // Only one of the following can be true: 0, 508, and x%6!=0
            if (sum == 0 && !EncounterMatch.IsWithinRange(pkm))
                data.AddLine(Get(V23, Severity.Fishy));
            else if (sum == 508)
                data.AddLine(Get(V24, Severity.Fishy));
            else if (evs[0] != 0 && evs.All(ev => evs[0] == ev))
                data.AddLine(Get(V27, Severity.Fishy));
        }
    }
}
