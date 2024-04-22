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
    public static bool IsEncounterMarkAllowed(IEncounterTemplate enc, PKM pk)
    {
        if (IsEncounterMarkLost(enc, pk))
            return false;
        return enc.Context is EntityContext.Gen8 or EntityContext.Gen9;
    }

    /// <summary>
    /// Checks if original marks and ribbons are lost via data manipulation.
    /// </summary>
    public static bool IsEncounterMarkLost(IEncounterTemplate enc, PKM pk)
    {
        // Nincada -> Shedinja loses all ribbons and marks, but does not purge any Affixed Ribbon value.
        return enc.Species is (int)Species.Nincada && pk.Species == (int)Species.Shedinja;
    }

    /// <summary>
    /// Checks if a characteristic encounter mark (only those that were introduced in SW/SH) is valid.
    /// </summary>
    public static bool IsEncounterMarkValid(RibbonIndex mark, PKM pk, IEncounterTemplate enc) => enc switch
    {
        EncounterSlot8 or EncounterStatic8 { Gift: false, ScriptedNoMarks: false } => IsMarkAllowedSpecific8(mark, pk, enc),
        EncounterSlot9 s => IsMarkAllowedSpecific9(mark, s),
        EncounterStatic9 s => IsMarkAllowedSpecific9(mark, s),
        EncounterOutbreak9 o when o.Ribbon == mark || IsMarkAllowedSpecific9(mark, pk) => true, // not guaranteed ribbon/mark
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
        MarkMisty when x.Generation == 8 && pk.MetLevel < EncounterArea8.BoostLevel && EncounterArea8.IsBoostedArea60Fog(pk.MetLocation) => false,
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

    /// <summary>
    /// Checks if a specific encounter mark is disallowed.
    /// </summary>
    /// <returns>False if mark is disallowed based on specific conditions.</returns>
    /// <remarks>ONLY USE FOR <see cref="EncounterOutbreak9"/></remarks>
    public static bool IsMarkAllowedSpecific9(RibbonIndex mark, PKM pk) => mark switch
    {
        MarkCurry => false,
        MarkFishing => false,
        MarkDestiny => true, // Capture on Birthday
        >= MarkLunchtime and <= MarkDawn => true, // no time restrictions
        >= MarkCloudy and <= MarkMisty => pk is PK8 || EncounterSlot9.CanSpawnInWeather(mark, (byte)pk.MetLocation),
        _ => true,
    };

    /// <summary>
    /// Checks if a specific encounter mark is disallowed.
    /// </summary>
    /// <returns>False if mark is disallowed based on specific conditions.</returns>
    public static bool IsMarkAllowedSpecific9(RibbonIndex mark, EncounterStatic9 s) => mark switch
    {
        MarkCrafty => s.RibbonMarkCrafty,
        _ => false,
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
        var location = s.Parent.Location;
        // If it's not in the main table, it can only have Normal weather.
        var weather = EncounterArea8.GetWeather(location);
        if (weather.HasFlag(permit))
            return true;

        // Valid tree/fishing weathers should have returned with main area weather.
        if ((s.Weather & (AreaWeather8.Shaking_Trees | AreaWeather8.Fishing)) != 0)
            return false;

        // Check bleed conditions otherwise.
        return EncounterArea8.IsWeatherBleedPossible(s.Type, permit, location);
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
        // 7-Star raids that can be captured force the mark when obtained.
        return enc is EncounterMight9 { Stars: 7 };
    }

    /// <summary>
    /// Checks if the input's <see cref="IRibbonSetMark9.RibbonMarkMightiest"/> mark state is valid.
    /// </summary>
    public static bool IsMarkValidMightiest(IEncounterTemplate enc, bool hasMark, EvolutionHistory evos)
    {
        if (IsMarkPresentMightiest(enc))
            return hasMark;
        if (enc.Species == (int)Species.Mew && evos.HasVisitedGen9)
            return true; // Can be awarded the mark for battling Mewtwo.
        return !hasMark;
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
    public static bool IsMarkValidItemFinder(EvolutionHistory evos) => evos.HasVisitedGen9; // Obtainable starting in DLC1.

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
            return RibbonIndexExtensions.MAX_G9;
        if (evos.HasVisitedSWSH)
            return RibbonIndexExtensions.MAX_G8; // Pioneer and Twinkling Star cannot be selected in SW/SH.
        return unchecked((RibbonIndex)(-1));
    }
}

/// <summary>
/// Indicates if the encounter is lacking a specific mark.
/// </summary>
/// <remarks>
/// Some encounters are made available with a specific mark, and the mark is required to be present.
/// </remarks>
public interface IEncounterMarkExtra
{
    /// <summary>
    /// Checks if the encounter is missing a specific mark.
    /// </summary>
    /// <param name="pk">The encounter to check.</param>
    /// <param name="missing">The missing mark.</param>
    /// <returns>True if the encounter is missing the mark.</returns>
    /// <remarks>
    /// If the encounter is missing the mark, the <paramref name="missing"/> value will be set to the missing mark.
    /// </remarks>
    bool IsMissingExtraMark(PKM pk, out RibbonIndex missing);
}
