using static PKHeX.Core.RibbonIndex;

namespace PKHeX.Core;

/// <summary>
/// Rules for obtaining marks.
/// </summary>
/// <see cref="RibbonRules"/>
public static class MarkRules
{
    /// <summary>
    /// Checks if an encounter-only mark is possible to obtain for the encounter, if not lost via data manipulation.
    /// </summary>
    public static bool IsEncounterMarkAllowed(LegalityAnalysis data)
    {
        if (IsEncounterMarkLost(data))
            return false;
        return data.Info.EncounterOriginal.Context is EntityContext.Gen8 or EntityContext.Gen9;
    }

    /// <summary>
    /// Checks if original marks and ribbons are lost via data manipulation.
    /// </summary>
    public static bool IsEncounterMarkLost(LegalityAnalysis data)
    {
        // Nincada -> Shedinja loses all ribbons and marks, but does not purge any Affixed Ribbon value.
        return data.EncounterOriginal.Species is (int)Species.Nincada && data.Entity.Species == (int)Species.Shedinja;
    }

    /// <summary>
    /// Checks if a SW/SH mark is valid.
    /// </summary>
    public static bool IsEncounterMarkValid(RibbonIndex mark, PKM pk, IEncounterTemplate enc) => enc switch
    {
        EncounterSlot8 or EncounterStatic8 { Gift: false, ScriptedNoMarks: false } => IsMarkAllowedSpecific8(mark, pk, enc),
        EncounterSlot9 s => IsMarkAllowedSpecific9(mark, s),
        WC9 wc9 => wc9.GetRibbonIndex(mark),
        _ => false,
    };

    /// <summary>
    /// Checks if a specific encounter mark is disallowed.
    /// </summary>
    /// <returns>False if mark is disallowed based on specific conditions.</returns>
    public static bool IsMarkAllowedSpecific8(RibbonIndex mark, PKM pk, IEncounterTemplate x) => mark switch
    {
        MarkCurry when !IsMarkAllowedCurry(pk, x) => false,
        MarkFishing when !IsMarkAllowedFishing(x) => false,
        MarkMisty when x.Generation == 8 && pk.Met_Level < EncounterArea8.BoostLevel && EncounterArea8.IsBoostedArea60Fog(pk.Met_Location) => false,
        MarkDestiny => x is EncounterSlot9, // Capture on Birthday
        >= MarkCloudy and <= MarkMisty => IsWeatherPermitted8(mark, x),
        _ => true,
    };

    /// <summary>
    /// Checks if a specific encounter mark is disallowed.
    /// </summary>
    /// <returns>False if mark is disallowed based on specific conditions.</returns>
    public static bool IsMarkAllowedSpecific9(RibbonIndex mark, EncounterSlot9 x) => mark switch
    {
        MarkCurry => false,
        MarkFishing => false,
        MarkDestiny => true, // Capture on Birthday
        >= MarkLunchtime and <= MarkDawn => x.CanSpawnAtTime(mark),
        >= MarkCloudy and <= MarkMisty => x.CanSpawnInWeather(mark),
        _ => true,
    };

    // Encounter slots check location weather, while static encounters check weather per encounter.
    private static bool IsWeatherPermitted8(RibbonIndex mark, IEncounterTemplate enc) => enc switch
    {
        EncounterSlot8 w => IsSlotWeatherPermittedSWSH(mark.GetWeather8(), w),
        EncounterStatic8 s => s.Weather.HasFlag(mark.GetWeather8()),
        _ => false,
    };

    private static bool IsSlotWeatherPermittedSWSH(AreaWeather8 permit, EncounterSlot8 s)
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

    /// <summary>
    /// Checks if a <see cref="MarkCurry"/> mark is valid.
    /// </summary>
    public static bool IsMarkAllowedCurry(PKM pk, IEncounterTemplate enc)
    {
        // Curry are only encounter slots, from the hidden table (not symbol). Slots taken from area's current weather(?).
        if (enc is not EncounterSlot8 { CanEncounterViaCurry: true })
            return false;

        var ball = pk.Ball;
        return (uint)(ball - 2) <= 2;
    }

    /// <summary>
    /// Checks if a <see cref="MarkFishing"/> mark is valid.
    /// </summary>
    public static bool IsMarkAllowedFishing(IEncounterTemplate enc)
    {
        return enc is EncounterSlot8 { CanEncounterViaFishing: true };
    }

    /// <summary>
    /// Checks if a <see cref="MarkAlpha"/> mark is valid.
    /// </summary>
    public static bool IsMarkValidAlpha(PKM pk, bool wasAlpha)
    {
        if (pk is IAlpha a && a.IsAlpha != wasAlpha)
            return false;
        if (pk is not IRibbonSetMark9 m)
            return true;
        if (m.RibbonMarkAlpha == wasAlpha)
            return true;

        // Before HOME 3.0.0, this mark was never set.
        return wasAlpha && pk is PK8 or PB8 or PA8; // Not yet touched HOME 3.0.0
    }

    /// <summary>
    /// Checks if the input can have the <see cref="IRibbonSetMark9.RibbonMarkAlpha"/> mark.
    /// </summary>
    public static bool IsMarkValidAlpha(IEncounterTemplate enc, PKM pk)
    {
        var expect = enc is IAlphaReadOnly { IsAlpha: true };
        return IsMarkValidAlpha(pk, expect);
    }

    /// <summary>
    /// Checks if the input can have the <see cref="IRibbonSetMark9.RibbonMarkJumbo"/> mark.
    /// </summary>
    public static bool IsMarkAllowedJumbo(EvolutionHistory evos, PKM pk)
    {
        const byte expect = byte.MaxValue;
        if (!evos.HasVisitedGen9)
            return false;
        if (pk is IScaledSize3 s)
            return s.Scale == expect;
        if (pk is IScaledSize s2)
            return s2.HeightScalar == expect;
        return false;
    }

    /// <summary>
    /// Checks if the input can have the <see cref="IRibbonSetMark9.RibbonMarkMini"/> mark.
    /// </summary>
    public static bool IsMarkAllowedMini(EvolutionHistory evos, PKM pk)
    {
        const byte expect = byte.MinValue;
        if (!evos.HasVisitedGen9)
            return false;
        if (pk is IScaledSize3 s)
            return s.Scale == expect;
        if (pk is IScaledSize s2)
            return s2.HeightScalar == expect;
        return false;
    }

    /// <summary>
    /// Checks if the input can have the <see cref="IRibbonSetMark9.RibbonMarkGourmand"/> mark.
    /// </summary>
    public static bool IsMarkValidGourmand(EvolutionHistory evos)
    {
        // Requires eating food, either crafted sandwich (EXP) or store bought (no EXP).
        return evos.HasVisitedGen9;
    }

    /// <summary>
    /// Checks if the input should have the <see cref="IRibbonSetMark9.RibbonMarkMightiest"/> mark.
    /// </summary>
    public static bool IsMarkPresentMightiest(IEncounterTemplate enc)
    {
        // 7 star raids only.
        return enc is EncounterMight9 { Stars: 7 };
    }

    /// <summary>
    /// Checks if the input should have the <see cref="IRibbonSetMark9.RibbonMarkTitan"/> mark.
    /// </summary>
    public static bool IsMarkPresentTitan(IEncounterTemplate enc)
    {
        // Titan static encounters only.
        return enc is EncounterStatic9 { IsTitan: true };
    }

    /// <summary>
    /// Checks if the input should have the <see cref="IRibbonSetMark9.RibbonMarkItemfinder"/> mark.
    /// </summary>
    public static bool IsMarkValidItemFinder(EvolutionHistory evos) => false; // evos.HasVisitedGen9;

    /// <summary>
    /// Checks if the input should have the <see cref="IRibbonSetMark9.RibbonMarkPartner"/> mark.
    /// </summary>
    public static bool IsMarkValidPartner(EvolutionHistory evos) => evos.HasVisitedGen9;

    /// <summary>
    /// Gets the maximum obtainable <see cref="RibbonIndex"/> value for the format.
    /// </summary>
    public static RibbonIndex GetMaxAffixValue(EvolutionHistory evos)
    {
        if (evos.HasVisitedGen9)
            return MarkTitan;
        if (evos.HasVisitedSWSH)
            return MarkSlump; // Pioneer and Twinkling Star cannot be selected in SW/SH.
        return unchecked((RibbonIndex)(-1));
    }
}
