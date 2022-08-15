using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="RibbonIndex"/> values for markings.
/// </summary>
public sealed class MarkVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.RibbonMark;

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk is not IRibbonIndex m)
            return;

        if (data.Info.Generation != 8 || (pk.Species == (int)Species.Shedinja && data.EncounterOriginal.Species is not (int)Species.Shedinja)) // Shedinja doesn't copy Ribbons or Marks
            VerifyNoMarksPresent(data, m);
        else
            VerifyMarksPresent(data, m);

        VerifyAffixedRibbonMark(data, m);
    }

    private void VerifyNoMarksPresent(LegalityAnalysis data, IRibbonIndex m)
    {
        for (var x = RibbonIndex.MarkLunchtime; x <= RibbonIndex.MarkSlump; x++)
        {
            if (m.GetRibbon((int)x))
                data.AddLine(GetInvalid(string.Format(LRibbonMarkingFInvalid_0, x)));
        }
    }

    private void VerifyMarksPresent(LegalityAnalysis data, IRibbonIndex m)
    {
        bool hasOne = false;
        for (var mark = RibbonIndex.MarkLunchtime; mark <= RibbonIndex.MarkSlump; mark++)
        {
            bool has = m.GetRibbon((int)mark);
            if (!has)
                continue;

            if (hasOne)
            {
                data.AddLine(GetInvalid(string.Format(LRibbonMarkingFInvalid_0, GetRibbonNameSafe(mark))));
                return;
            }

            bool result = IsMarkValid(mark, data.Entity, data.EncounterMatch);
            if (!result)
            {
                data.AddLine(GetInvalid(string.Format(LRibbonMarkingFInvalid_0, GetRibbonNameSafe(mark))));
                return;
            }

            hasOne = true;
        }
    }

    private static string GetRibbonNameSafe(RibbonIndex index)
    {
        if (index >= RibbonIndex.MAX_COUNT)
            return index.ToString();
        var expect = $"Ribbon{index}";
        return RibbonStrings.GetName(expect);
    }

    public static bool IsMarkValid(RibbonIndex mark, PKM pk, IEncounterTemplate enc)
    {
        return IsMarkAllowedAny(enc) && IsMarkAllowedSpecific(mark, pk, enc);
    }

    public static bool IsMarkAllowedSpecific(RibbonIndex mark, PKM pk, IEncounterTemplate x) => mark switch
    {
        RibbonIndex.MarkCurry when !IsMarkAllowedCurry(pk, x) => false,
        RibbonIndex.MarkFishing when !IsMarkAllowedFishing(x) => false,
        RibbonIndex.MarkMisty when pk.Met_Level < EncounterArea8.BoostLevel && EncounterArea8.IsBoostedArea60Fog(pk.Met_Location) => false,
        RibbonIndex.MarkDestiny => false,
        >= RibbonIndex.MarkCloudy and <= RibbonIndex.MarkMisty => IsWeatherPermitted(mark, x),
        _ => true,
    };

    private static bool IsWeatherPermitted(RibbonIndex mark, IEncounterTemplate enc)
    {
        var permit = mark.GetWeather8();

        // Encounter slots check location weather, while static encounters check weather per encounter.
        return enc switch
        {
            EncounterSlot8 w => IsSlotWeatherPermitted(permit, w),
            EncounterStatic8 s => s.Weather.HasFlag(permit),
            _ => false,
        };
    }

    private static bool IsSlotWeatherPermitted(AreaWeather8 permit, EncounterSlot8 s)
    {
        var location = s.Location;
        // If it's not in the main table, it can only have Normal weather.
        if (!EncounterArea8.WeatherbyArea.TryGetValue(location, out var weather))
            weather = AreaWeather8.Normal;
        if (weather.HasFlag(permit))
            return true;

        // Valid tree/fishing weathers should have returned with main area weather.
        if ((s.Weather & (AreaWeather8.Shaking_Trees | AreaWeather8.Fishing)) != 0)
            return false;

        // Check bleed conditions otherwise.
        return EncounterArea8.IsWeatherBleedPossible(s.SlotType, permit, location);
    }

    public static bool IsMarkAllowedAny(IEncounterTemplate enc) => enc.Generation == 8 && enc switch
    {
        // Gen 8
        EncounterSlot8 or EncounterStatic8 {Gift: false, ScriptedNoMarks: false} => true,
        _ => false,
    };

    public static bool IsMarkAllowedCurry(PKM pk, IEncounterTemplate enc)
    {
        // Curry are only encounter slots, from the hidden table (not symbol). Slots taken from area's current weather(?).
        if (enc is not EncounterSlot8 {CanEncounterViaCurry: true})
            return false;

        var ball = pk.Ball;
        return (uint)(ball - 2) <= 2;
    }

    public static bool IsMarkAllowedFishing(IEncounterTemplate enc)
    {
        return enc is EncounterSlot8 {CanEncounterViaFishing: true};
    }

    private void VerifyAffixedRibbonMark(LegalityAnalysis data, IRibbonIndex m)
    {
        if (m is not IRibbonSetAffixed a)
            return;

        var affix = a.AffixedRibbon;
        if (affix == -1) // None
            return;

        if ((byte)affix > (int)RibbonIndex.MarkSlump) // SW/SH cannot affix anything higher.
        {
            data.AddLine(GetInvalid(string.Format(LRibbonMarkingAffixedF_0, GetRibbonNameSafe((RibbonIndex)affix))));
            return;
        }

        if (m is not PKM pk)
            return;

        if (pk.Species == (int)Species.Shedinja && data.EncounterOriginal.Species is not (int)Species.Shedinja)
        {
            VerifyShedinjaAffixed(data, affix, pk, m);
            return;
        }
        EnsureHasRibbon(data, m, affix);
    }

    private void VerifyShedinjaAffixed(LegalityAnalysis data, sbyte affix, PKM pk, IRibbonIndex r)
    {
        // Does not copy ribbons or marks, but retains the Affixed Ribbon value.
        // Try re-verifying to see if it could have had the Ribbon/Mark.

        var enc = data.EncounterOriginal;
        if ((byte) affix >= (int) RibbonIndex.MarkLunchtime)
        {
            if (!IsMarkValid((RibbonIndex)affix, pk, enc))
                data.AddLine(GetInvalid(string.Format(LRibbonMarkingAffixedF_0, GetRibbonNameSafe((RibbonIndex)affix))));
            return;
        }

        if (enc.Generation <= 4 && (pk.Ball != (int)Ball.Poke || IsMoveSetEvolvedShedinja(pk)))
        {
            // Evolved in a prior generation.
            EnsureHasRibbon(data, r, affix);
            return;
        }

        var clone = pk.Clone();
        clone.Species = (int) Species.Nincada;
        var args = new RibbonVerifierArguments(clone, enc, data.Info.EvoChainsAllGens);
        ((RibbonIndex)affix).Fix(args, true);
        var name = GetRibbonNameSafe((RibbonIndex)affix);
        bool invalid = RibbonVerifier.IsValidExtra((RibbonIndex)affix, args);
        var severity = invalid ? Severity.Invalid : Severity.Fishy;
        data.AddLine(Get(string.Format(LRibbonMarkingAffixedF_0, name), severity));
    }

    private static bool IsMoveSetEvolvedShedinja(PKM pk)
    {
        // Check for gen3/4 exclusive moves that are Ninjask glitch only.
        if (pk.HasMove((int) Move.Screech))
            return true;
        if (pk.HasMove((int) Move.SwordsDance))
            return true;
        if (pk.HasMove((int) Move.Slash))
            return true;
        if (pk.HasMove((int) Move.BatonPass))
            return true;
        return pk.HasMove((int)Move.Agility) && pk is PK8 pk8 && !pk8.GetMoveRecordFlag(12); // TR12 (Agility)
    }

    private void EnsureHasRibbon(LegalityAnalysis data, IRibbonIndex m, sbyte affix)
    {
        var hasRibbon = m.GetRibbonIndex((RibbonIndex) affix);
        if (!hasRibbon)
            data.AddLine(GetInvalid(string.Format(LRibbonMarkingAffixedF_0, GetRibbonNameSafe((RibbonIndex) affix))));
    }
}
