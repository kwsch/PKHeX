using System;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="EffortValues"/>.
/// </summary>
public sealed class EffortValueVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.EVs;

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.IsEgg)
        {
            if (pk.EVTotal is not 0)
                data.AddLine(GetInvalid(LEffortEgg));
            return;
        }

        // In Generation 1 & 2, when a Pokémon is taken out of the Day Care, its experience will lower to the minimum value for its current level.
        byte format = pk.Format;
        if (format < 3) // Can abuse daycare for EV training without EXP gain
            return;

        int sum = pk.EVTotal;
        if (sum > EffortValues.Max510) // format >= 3
            data.AddLine(GetInvalid(LEffortAbove510));

        var enc = data.EncounterMatch;
        Span<int> evs = stackalloc int[6];
        pk.GetEVs(evs);

        if (format >= 6 && IsAnyAboveHardLimit6(evs))
            data.AddLine(GetInvalid(LEffortAbove252));
        else if (format < 5) // 3/4
            VerifyGainedEVs34(data, enc, evs, pk);

        // Only one of the following can be true: 0, 508, and x%6!=0
        if (sum == 0 && !enc.IsWithinEncounterRange(pk))
            data.AddLine(Get(LEffortEXPIncreased, Severity.Fishy));
        else if (sum == EffortValues.MaxEffective)
            data.AddLine(Get(LEffort2Remaining, Severity.Fishy));
        else if (evs[0] != 0 && !evs.ContainsAnyExcept(evs[0]))
            data.AddLine(Get(LEffortAllEqual, Severity.Fishy));
    }

    private void VerifyGainedEVs34(LegalityAnalysis data, IEncounterTemplate enc, ReadOnlySpan<int> evs, PKM pk)
    {
        bool anyAbove100 = IsAnyAboveVitaminLimit(evs);
        if (!anyAbove100)
            return;

        if (enc.LevelMin == 100) // only true for Gen4 and Format=4
        {
            // Cannot EV train at level 100 -- Certain events are distributed at level 100.
            // EVs can only be increased by vitamins to a max of 100.
            data.AddLine(GetInvalid(LEffortCap100));
        }
        else // Check for gained EVs without gaining EXP -- don't check Gen5+ which have wings to boost above 100.
        {
            var growth = PersonalTable.HGSS[enc.Species].EXPGrowth;
            var baseEXP = Experience.GetEXP(enc.LevelMin, growth);
            if (baseEXP == pk.EXP)
                data.AddLine(GetInvalid(string.Format(LEffortUntrainedCap, EffortValues.MaxVitamins34)));
        }
    }

    // Hard cap at 252 for Gen6+
    private static bool IsAnyAboveHardLimit6(ReadOnlySpan<int> evs)
        => evs.ContainsAnyExceptInRange(0, EffortValues.Max252);

    // Vitamins can only raise to 100 in Gen3/4
    private static bool IsAnyAboveVitaminLimit(ReadOnlySpan<int> evs)
        => evs.ContainsAnyExceptInRange(0, EffortValues.MaxVitamins34);
}
