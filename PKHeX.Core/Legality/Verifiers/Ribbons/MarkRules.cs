using static PKHeX.Core.RibbonIndex;

namespace PKHeX.Core;

/// <summary>
/// Rules for obtaining marks.
/// </summary>
/// <see cref="RibbonRules"/>
public static class MarkRules
{
    /// <summary>
    /// Checks if the ribbon index is one of the specific SW/SH encounter-only marks. These marks are granted when the encounter spawns in the wild.
    /// </summary>
    public static bool IsEncounterMark(this RibbonIndex m) => (byte)m is >= (int)MarkLunchtime and <= (int)MarkSlump;

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
    public static bool IsMarkValid8(RibbonIndex mark, PKM pk, IEncounterTemplate enc)
    {
        return IsEncounterMarkAllowedAny(enc) && IsMarkAllowedSpecific(mark, pk, enc);
    }

    /// <summary>
    /// Checks if a specific encounter mark is disallowed.
    /// </summary>
    /// <returns>False if mark is disallowed based on specific conditions.</returns>
    public static bool IsMarkAllowedSpecific(RibbonIndex mark, PKM pk, IEncounterTemplate x) => mark switch
    {
        MarkCurry when !IsMarkAllowedCurry(pk, x) => false,
        MarkFishing when !IsMarkAllowedFishing(x) => false,
        MarkMisty when x.Generation == 8 && pk.Met_Level < EncounterArea8.BoostLevel && EncounterArea8.IsBoostedArea60Fog(pk.Met_Location) => false,
        MarkDestiny => x is EncounterSlot9, // Capture on Birthday
        >= MarkCloudy and <= MarkMisty => IsWeatherPermitted(mark, x),
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

    /// <summary>
    /// Checks if any encounter-only mark is available for the <see cref="enc"/>.
    /// </summary>
    public static bool IsEncounterMarkAllowedAny(IEncounterTemplate enc) => enc.Generation >= 8 && enc switch
    {
        // Gen 8
        EncounterSlot8 or EncounterStatic8 { Gift: false, ScriptedNoMarks: false } => true,
        EncounterSlot9 => true,
        _ => false,
    };

    /// <summary>
    /// Checks if a <see cref="RibbonIndex.MarkCurry"/> mark is valid.
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
    /// Checks if a <see cref="RibbonIndex.MarkFishing"/> mark is valid.
    /// </summary>
    public static bool IsMarkAllowedFishing(IEncounterTemplate enc)
    {
        return enc is EncounterSlot8 { CanEncounterViaFishing: true };
    }

    /// <summary>
    /// Checks if the input can have the <see cref="IRibbonSetMark9.RibbonMarkAlpha"/> mark.
    /// </summary>
    public static bool IsMarkPresentAlpha(IEncounterTemplate enc) => enc is IAlphaReadOnly { IsAlpha: true};

    /// <summary>
    /// Checks if the input can have the <see cref="IRibbonSetMark9.RibbonMarkJumbo"/> mark.
    /// </summary>
    public static bool IsMarkAllowedJumbo(EvolutionHistory evos, PKM pk)
    {
        if (!evos.HasVisitedGen9)
            return false;
        if (pk is IScaledSize3 s)
            return s.Scale == byte.MaxValue;
        return false;
    }

    /// <summary>
    /// Checks if the input can have the <see cref="IRibbonSetMark9.RibbonMarkMini"/> mark.
    /// </summary>
    public static bool IsMarkAllowedMini(EvolutionHistory evos, PKM pk)
    {
        if (!evos.HasVisitedGen9)
            return false;
        if (pk is IScaledSize3 s)
            return s.Scale == 0;
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
        return enc is EncounterTera9 { Stars: 7 };
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
    public static RibbonIndex GetMaxAffixValue(int entityFormat) => entityFormat switch
    {
        <= 8 => MarkSlump, // Pioneer and Twinkling Star cannot be selected in SW/SH.
        _ => MarkTitan, // Max ribbon visible in SV.
    };
}
