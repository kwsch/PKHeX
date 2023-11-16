using System;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="PKM.OT_Name"/>.
/// </summary>
public sealed class TrainerNameVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Trainer;

    private static readonly string[] SuspiciousOTNames =
    [
        "PKHeX",
        "ＰＫＨｅＸ",
    ];

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var enc = data.EncounterMatch;
        if (!IsPlayerOriginalTrainer(enc))
            return; // already verified

        var ot = pk.OT_Name;
        if (ot.Length == 0)
            data.AddLine(GetInvalid(LOTShort));

        if (IsOTNameSuspicious(ot))
        {
            data.AddLine(Get(LOTSuspicious, Severity.Fishy));
        }

        if (pk.VC)
        {
            VerifyOTGB(data);
        }
        else if (ot.Length > Legal.GetMaxLengthOT(data.Info.Generation, (LanguageID)pk.Language))
        {
            if (!IsEdgeCaseLength(pk, data.EncounterOriginal, ot))
                data.AddLine(Get(LOTLong, Severity.Invalid));
        }

        if (ParseSettings.CheckWordFilter)
        {
            if (WordFilter.IsFiltered(ot, out var badPattern))
                data.AddLine(GetInvalid($"Word Filter: {badPattern}"));
            if (ContainsTooManyNumbers(ot, data.Info.Generation))
                data.AddLine(GetInvalid("Word Filter: Too many numbers."));

            if (WordFilter.IsFiltered(pk.HT_Name, out badPattern))
                data.AddLine(GetInvalid($"Word Filter: {badPattern}"));
        }
    }

    /// <summary>
    /// Checks if any player (human) was the original OT.
    /// </summary>
    internal static bool IsPlayerOriginalTrainer(IEncounterable enc) => enc switch
    {
        IFixedTrainer { IsFixedTrainer: true } => false,
        MysteryGift { IsEgg: false } => false,
        EncounterStatic5N => false,
        _ => true,
    };

    public static bool IsEdgeCaseLength(PKM pk, IEncounterTemplate e, ReadOnlySpan<char> ot)
    {
        if (e.EggEncounter)
        {
            if (e is WC3 wc3 && pk.IsEgg && ot.SequenceEqual(wc3.OT_Name))
                return true; // Fixed OT Mystery Gift Egg
            bool eggEdge = pk.IsEgg ? pk.IsTradedEgg || pk.Format == 3 : pk.WasTradedEgg;
            if (!eggEdge)
                return false;
            var len = Legal.GetMaxLengthOT(e.Generation, LanguageID.English); // max case
            return ot.Length <= len;
        }

        if (e is IFixedTrainer { IsFixedTrainer: true })
            return true; // already verified

        if (e is MysteryGift mg && mg.OT_Name.Length == ot.Length)
            return true; // Mattle Ho-Oh
        return false;
    }

    public void VerifyOTGB(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var enc = data.EncounterOriginal;
        if (pk.OT_Gender == 1)
        {
            // Transferring from RBY->Gen7 won't have OT Gender in PK1, nor will PK1 originated encounters.
            // GSC Trades already checked for OT Gender matching.
            if (pk is { Format: > 2, VC1: true } || enc is { Generation: 1 } or EncounterGift2 { IsGift: true })
                data.AddLine(GetInvalid(LG1OTGender));
        }

        if (enc is IFixedTrainer { IsFixedTrainer: true })
            return; // already verified

        string tr = pk.OT_Name;
        if (tr.Length == 0)
        {
            if (pk is SK2 {TID16: 0, IsRental: true})
            {
                data.AddLine(Get(LOTShort, Severity.Fishy));
            }
            else
            {
                data.AddLine(GetInvalid(LOTShort));
                return;
            }
        }
        VerifyGBOTWithinBounds(data, tr);
    }

    private void VerifyGBOTWithinBounds(LegalityAnalysis data, ReadOnlySpan<char> str)
    {
        var pk = data.Entity;
        if (pk.Japanese)
        {
            if (str.Length > 5)
                data.AddLine(GetInvalid(LOTLong));
            if (!StringConverter12.GetIsG1Japanese(str))
                data.AddLine(GetInvalid(LG1CharOT));
        }
        else if (pk.Korean)
        {
            if (str.Length > 5)
                data.AddLine(GetInvalid(LOTLong));
            if (!StringConverter2KOR.GetIsG2Korean(str))
                data.AddLine(GetInvalid(LG1CharOT));
        }
        else
        {
            if (str.Length > 7)
                data.AddLine(GetInvalid(LOTLong));
            if (!StringConverter12.GetIsG1English(str))
                data.AddLine(GetInvalid(LG1CharOT));
        }
    }

    private static bool IsOTNameSuspicious(ReadOnlySpan<char> name)
    {
        foreach (var s in SuspiciousOTNames)
        {
            if (name.StartsWith(s, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    public static bool ContainsTooManyNumbers(ReadOnlySpan<char> str, int originalGeneration)
    {
        if (originalGeneration <= 3)
            return false; // no limit from these generations
        int max = originalGeneration < 6 ? 4 : 5;
        if (str.Length <= max)
            return false;
        int count = GetNumberCount(str);
        return count > max;
    }

    private static int GetNumberCount(ReadOnlySpan<char> str)
    {
        static bool IsNumber(char c)
        {
            if (c >= '０')
                return c <= '９';
            return (uint)(c - '0') <= 9;
        }

        int ctr = 0;
        foreach (var c in str)
        {
            if (IsNumber(c))
                ++ctr;
        }
        return ctr;
    }
}
