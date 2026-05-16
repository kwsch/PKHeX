using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LegalityCheckResultCode;

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
                data.AddLine(GetInvalid(EffortEgg));
            return;
        }

        byte format = pk.Format;
        if (format < 3)
        {
            VerifyEVsGB(data, pk);
            return;
        }

        int sum = pk.EVTotal;
        if (sum > EffortValues.Max510) // format >= 3
            data.AddLine(GetInvalid(EffortAbove510));

        Span<int> evs = stackalloc int[6];
        pk.GetEVs(evs);

        var enc = data.EncounterMatch;
        if (format >= 6 && IsAnyAboveHardLimit6(evs))
            data.AddLine(GetInvalid(EffortAbove252));
        else if (format < 5) // 3/4
            VerifyGainedEVs34(data, enc, evs, pk);

        // Only one of the following can be true: 0, 508, and x%6!=0
        if (sum == 0 && !enc.IsWithinEncounterRange(pk))
            data.AddLine(Get(Severity.Fishy, EffortEXPIncreased));
        else if (sum == EffortValues.MaxEffective)
            data.AddLine(Get(Severity.Fishy, Effort2Remaining));
        else if (evs[0] != 0 && !evs.ContainsAnyExcept(evs[0]))
            data.AddLine(Get(Severity.Fishy, EffortAllEqual));
    }

    private void VerifyEVsGB(LegalityAnalysis data, PKM pk)
    {
        ReadOnlySpan<int> evs = [pk.EV_HP, pk.EV_ATK, pk.EV_DEF, pk.EV_SPA, pk.EV_SPE];
        var hasGainedEVsFromEnemy = IsAnyEVNotVitaminOr0(evs);
        if (!hasGainedEVsFromEnemy)
            return;

        // Generation 1/2 have special considerations to verify non-zero EVs, but it really isn't worth verifying.
        // If an EV stat is not maxed out, then there must be a valid encounter chain (considering teammates diluting the EV gain) that yields the exact EVs (and EXP minimum for Genera1).
        // In Generation 2, when a Pokémon is taken out of the Daycare, its experience will lower to the minimum value for its current level.
        // When transferred to Gen7+, EVs are reset to 0, so checks will be relevant then.
        // For Generation 1 entities that have never visited Generation 2 for the EXP reset:
        // * We COULD check the EVs but a heuristic for this would be extra annoying for minimal gain.
        // * Encounter matching deferrals would be required so that there is enough gap in MinLevel=>CurrentLevel to obtain the exact EVs with EXP gain.
        // * Vitamins in Gen1/2 add 2560 each, to a max of 25600 (10 vitamins used). Would need to subtract off vitamins to check for EVs needed, and species/EXP gain to get them.
        // * Gen1 EV gain is split same as EXP for participants against the defeated Pokémon; EV gains are the Base Stats of the defeated Pokémon.
        // * Unexplored: A {species, +EV[6], +EXP} permutation budget. Using Wild Encounters AND trainers, no-Switch AI must disallow the final mon from being used repeatedly.
        // * Are all EV gains for a specific stat consistent enough to prune to minimum EXP gains for +EV?
        // * Gen1 daycare can make up the remaining EXP difference.
        // Same for Generation 2, just with different available enemies and the ability to disregard EXP gain via daycare abuse.

        // Simple sanity check instead of nothing:
        // Since EV gains absorb base stats of the defeated, you can't have EVs in a stat if any other stat is 0.
        // Sometimes people may miss entering in a stat accidentally, so this is a simple check to catch that.

        if (evs.Contains(0))
            data.AddLine(GetInvalid(EffortShouldBeZero));
    }

    private static bool IsAnyEVNotVitaminOr0(ReadOnlySpan<int> evs)
    {
        foreach (var ev in evs)
        {
            if (!IsNot0VitaminGB(ev))
                continue;
            return true;
        }
        return false;

        static bool IsNot0VitaminGB(int ev) => ev > EffortValues.MaxVitamins12 || (ev % EffortValues.VitaminBoost12) != 0;
    }

    private void VerifyGainedEVs34(LegalityAnalysis data, IEncounterTemplate enc, ReadOnlySpan<int> evs, PKM pk)
    {
        var isVitaminResult = IsWithinVitaminRange34(evs, EffortValues.MaxVitamins34);
        if (isVitaminResult)
            return;

        if (enc.LevelMin == Experience.MaxLevel) // only true for Gen4 and Format=4
        {
            // Cannot EV train at level 100 -- Certain events are distributed at level 100.
            // EVs can only be increased by vitamins to a max of 100.
            data.AddLine(GetInvalid(EffortCap100));
        }
        else // Check for gained EVs without gaining EXP -- don't check Gen5+ which have wings to boost above 100.
        {
            // Be slightly forgiving for past gen transfers where Met Level is overwritten; we might not be able to infer the exact level met (RNG correlations?).
            // Current code won't be able to flag PokéSpot not-min-level-met. If correlation and pruning (evo chain) improved, we can use that level min instead.
            // (Wonder if this also impacts wild encounter slots with multiple levels? Hopefully the lowest level is yielded first).
            var metLevel = enc.Generation == pk.Format ? pk.MetLevel : enc.LevelMin;
            if (metLevel < enc.LevelMin)
                metLevel = enc.LevelMin;

            var growth = PersonalTable.HGSS[enc.Species].EXPGrowth;
            var baseEXP = Experience.GetEXP(metLevel, growth);
            if (baseEXP == pk.EXP)
                data.AddLine(GetInvalid(EffortUntrainedCap_0, EffortValues.MaxVitamins34));
        }
    }

    // Hard cap at 252 for Gen6+
    private static bool IsAnyAboveHardLimit6(ReadOnlySpan<int> evs)
        => evs.ContainsAnyExceptInRange(0, EffortValues.Max252);

    // Vitamins can only raise to 100 in Gen3/4, only in multiples of 10.
    private static bool IsWithinVitaminRange34(ReadOnlySpan<int> evs, [ConstantExpected] ushort limit)
    {
        foreach (var ev in evs)
        {
            // The majority of cases will pass here; 0 is default & valid.
            // Eager check to skip unnecessary less performant checks.
            if (ev is 0)
                continue;

            if (ev > limit)
                return false;
            if (ev % 10 != 0)
                return false;
        }
        return true;
    }
}
