using static PKHeX.Core.RibbonIndex;

namespace PKHeX.Core;

/// <summary>
/// Parsing logic for <see cref="IRibbonSetCommon9"/>.
/// </summary>
public static class RibbonVerifierCommon9
{
    public static void Parse(this IRibbonSetCommon9 r, RibbonVerifierArguments args, ref RibbonResultList list)
    {
        if (r.RibbonChampionPaldea && !args.History.HasVisitedGen9)
            list.Add(ChampionPaldea);
        if (r.RibbonOnceInALifetime) // Flag it.
            list.Add(OnceInALifetime);
    }

    public static void ParseEgg(this IRibbonSetCommon9 r, ref RibbonResultList list)
    {
        if (r.RibbonChampionPaldea)
            list.Add(ChampionPaldea);
        if (r.RibbonOnceInALifetime)
            list.Add(OnceInALifetime);
    }
}
