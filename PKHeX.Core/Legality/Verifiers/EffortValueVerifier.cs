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
            int sum = pkm.EVTotal;
            if (sum > 0 && pkm.IsEgg)
                data.AddLine(GetInvalid(LEffortEgg));

            // In Generations I and II, when a Pokémon is taken out of the Day Care, its experience will lower to the minimum value for its current level.
            int format = pkm.Format;
            if (format < 3) // can abuse daycare for EV training without EXP gain
                return;

            if (sum > 510) // format >= 3
                data.AddLine(GetInvalid(LEffortAbove510));
            var evs = pkm.EVs;
            if (format >= 6 && evs.Any(ev => ev > 252))
                data.AddLine(GetInvalid(LEffortAbove252));

            const int vitaMax = 100; // Vitamin Max
            if (format < 5) // 3/4
            {
                if (EncounterMatch.LevelMin == 100) // only true for Gen4 and Format=4
                {
                    // Cannot EV train at level 100 -- Certain events are distributed at level 100.
                    if (evs.Any(ev => ev > vitaMax)) // EVs can only be increased by vitamins to a max of 100.
                        data.AddLine(GetInvalid(LEffortCap100));
                }
                else // check for gained EVs without gaining EXP -- don't check gen5+ which have wings to boost above 100.
                {
                    var growth = PersonalTable.HGSS[EncounterMatch.Species].EXPGrowth;
                    var baseEXP = Experience.GetEXP(EncounterMatch.LevelMin, growth);
                    if (baseEXP == pkm.EXP && evs.Any(ev => ev > vitaMax))
                        data.AddLine(GetInvalid(string.Format(LEffortUntrainedCap, vitaMax)));
                }
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
                data.AddLine(GetInvalid(string.Format(LAwakenedShouldBeValue, 2))); // go park transfers have 2 AVs for all stats.
            else if (awakened.AwakeningSum() == 0 && !EncounterMatch.IsWithinRange(pkm))
                data.AddLine(Get(LAwakenedEXPIncreased, Severity.Fishy));
        }
    }
}
