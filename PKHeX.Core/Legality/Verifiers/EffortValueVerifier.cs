using System;
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
            var enc = data.EncounterMatch;
            if (pkm.IsEgg)
            {
                if (pkm.EVTotal is not 0)
                    data.AddLine(GetInvalid(LEffortEgg));
                return;
            }

            // In Generations I and II, when a Pokémon is taken out of the Day Care, its experience will lower to the minimum value for its current level.
            int format = pkm.Format;
            if (format < 3) // can abuse daycare for EV training without EXP gain
                return;

            int sum = pkm.EVTotal;
            if (sum > 510) // format >= 3
                data.AddLine(GetInvalid(LEffortAbove510));
            Span<int> evs = stackalloc int[6];
            pkm.GetEVs(evs);
            if (format >= 6 && evs.Find(ev => ev > 252) != default)
                data.AddLine(GetInvalid(LEffortAbove252));

            const int vitaMax = 100; // Vitamin Max
            if (format < 5) // 3/4
            {
                if (enc.LevelMin == 100) // only true for Gen4 and Format=4
                {
                    // Cannot EV train at level 100 -- Certain events are distributed at level 100.
                    if (evs.Find(ev => ev > vitaMax) != default) // EVs can only be increased by vitamins to a max of 100.
                        data.AddLine(GetInvalid(LEffortCap100));
                }
                else // check for gained EVs without gaining EXP -- don't check gen5+ which have wings to boost above 100.
                {
                    var growth = PersonalTable.HGSS[enc.Species].EXPGrowth;
                    var baseEXP = Experience.GetEXP(enc.LevelMin, growth);
                    if (baseEXP == pkm.EXP && evs.Find(ev => ev > vitaMax) != default)
                        data.AddLine(GetInvalid(string.Format(LEffortUntrainedCap, vitaMax)));
                }
            }

            // Only one of the following can be true: 0, 508, and x%6!=0
            if (sum == 0 && !enc.IsWithinEncounterRange(pkm))
                data.AddLine(Get(LEffortEXPIncreased, Severity.Fishy));
            else if (sum == 508)
                data.AddLine(Get(LEffort2Remaining, Severity.Fishy));
            else if (evs[0] != 0 && evs.Count(evs[0]) == evs.Length)
                data.AddLine(Get(LEffortAllEqual, Severity.Fishy));
        }

        private void VerifyAwakenedValues(LegalityAnalysis data, IAwakened awakened)
        {
            var pkm = data.pkm;
            int sum = pkm.EVTotal;
            if (sum != 0)
                data.AddLine(GetInvalid(LEffortShouldBeZero));

            if (!awakened.AwakeningAllValid())
                data.AddLine(GetInvalid(LAwakenedCap));

            var enc = data.EncounterMatch;

            // go park transfers have 2 AVs for all stats.
            if (enc is EncounterSlot7GO)
            {
                Span<byte> avs = stackalloc byte[6];
                awakened.GetAVs(avs);
                foreach (var av in avs)
                {
                    if (av >= 2)
                        continue;

                    data.AddLine(GetInvalid(string.Format(LAwakenedShouldBeValue, 2)));
                    break;
                }
                return;
            }

            if (awakened.AwakeningSum() == 0 && !enc.IsWithinEncounterRange(pkm))
                data.AddLine(Get(LAwakenedEXPIncreased, Severity.Fishy));
        }
    }
}
