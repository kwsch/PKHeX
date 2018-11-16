using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the <see cref="PKM.EVs"/>.
    /// </summary>
    public sealed class EffortValueVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.EVs;

        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm is IAwakened a)
            {
                VerifyAwakenedValues(data, a);
                return;
            }
            var EncounterMatch = data.EncounterMatch;
            var evs = pkm.EVs;
            int sum = pkm.EVTotal;
            if (sum > 0 && pkm.IsEgg)
                data.AddLine(GetInvalid(LEffortEgg));
            if (pkm.Format >= 3 && sum > 510)
                data.AddLine(GetInvalid(LEffortAbove510));
            if (pkm.Format >= 6 && evs.Any(ev => ev > 252))
                data.AddLine(GetInvalid(LEffortAbove252));
            if (pkm.Format == 4 && pkm.Gen4 && EncounterMatch.LevelMin == 100)
            {
                // Cannot EV train at level 100 -- Certain events are distributed at level 100.
                if (evs.Any(ev => ev > 100)) // EVs can only be increased by vitamins to a max of 100.
                    data.AddLine(GetInvalid(LEffortCap100));
            }
            else if (pkm.Format < 5)
            {
                // In Generations I and II, when a Pokémon is taken out of the Day Care, its experience will lower to the minimum value for its current level.
                if (pkm.Format < 3) // can abuse daycare for EV training without EXP gain
                    return;

                const int maxEV = 100; // Vitamin Max
                if (PKX.GetEXP(EncounterMatch.LevelMin, pkm.Species) == pkm.EXP && evs.Any(ev => ev > maxEV))
                    data.AddLine(GetInvalid(string.Format(LEffortUntrainedCap, maxEV)));
            }

            // Only one of the following can be true: 0, 508, and x%6!=0
            if (sum == 0 && !EncounterMatch.IsWithinRange(pkm))
                data.AddLine(Get(LEffortEXPIncreased, Severity.Fishy));
            else if (sum == 508)
                data.AddLine(Get(LEffort2Remaining, Severity.Fishy));
            else if (evs[0] != 0 && evs.All(ev => evs[0] == ev))
                data.AddLine(Get(LEffortAllEqual, Severity.Fishy));
        }

        private void VerifyAwakenedValues(LegalityAnalysis data, IAwakened awakened)
        {
            var pkm = data.pkm;
            int sum = pkm.EVTotal;
            if (sum != 0)
                data.AddLine(GetInvalid(LEffortShouldBeZero));

            var EncounterMatch = data.EncounterMatch;
            if (!awakened.AwakeningAllValid())
                data.AddLine(GetInvalid(LAwakenedCap));
            if (EncounterMatch is EncounterSlot s && s.Type == SlotType.GoPark && Enumerable.Range(0, 6).Select(awakened.GetAV).Any(z => z < 2))
                data.AddLine(GetInvalid(LAwakenedShouldBeValue)); // go park transfers have 2 AVs for all stats.
            else if (awakened.AwakeningSum() == 0 && !EncounterMatch.IsWithinRange(pkm))
                data.AddLine(Get(LAwakenedEXPIncreased, Severity.Fishy));
        }
    }
}
