using static PKHeX.Core.RibbonIndex;

namespace PKHeX.Core;

/// <summary>
/// Parsing logic for <see cref="IRibbonSetEvent3"/>.
/// </summary>
public static class RibbonVerifierEvent3
{
    public static void Parse(this IRibbonSetEvent3 r, in RibbonVerifierArguments args, ref RibbonResultList list)
    {
        var enc = args.Encounter;
        if (enc is IRibbonSetEvent3 e)
        {
            // The Earth Ribbon is a ribbon exclusive to Pokémon Colosseum and Pokémon XD: Gale of Darkness
            // Awarded to all Pokémon on the player's team when they complete the Mt. Battle challenge without switching the team at any point.
            if (e.RibbonEarth)
            {
                if (!r.RibbonEarth)
                    list.Add(Earth, true);
            }
            else if (r.RibbonEarth && enc.Generation != 3)
            {
                list.Add(Earth);
            }

            if (r.RibbonNational != e.RibbonNational)
                list.Add(National, e.RibbonNational);
            if (r.RibbonCountry != e.RibbonCountry)
                list.Add(Country, e.RibbonCountry);
            if (r.RibbonChampionBattle != e.RibbonChampionBattle)
                list.Add(ChampionBattle, e.RibbonChampionBattle);
            if (r.RibbonChampionRegional != e.RibbonChampionRegional)
                list.Add(ChampionRegional, e.RibbonChampionRegional);
            if (r.RibbonChampionNational != e.RibbonChampionNational)
                list.Add(ChampionNational, e.RibbonChampionNational);
        }
        else
        {
            // The Earth Ribbon is a ribbon exclusive to Pokémon Colosseum and Pokémon XD: Gale of Darkness
            // Awarded to all Pokémon on the player's team when they complete the Mt. Battle challenge without switching the team at any point.
            if (r.RibbonEarth && enc.Generation != 3)
                list.Add(Earth);

            var nationalRequired = RibbonRules.GetValidRibbonStateNational(args.Entity, enc);
            if (r.RibbonNational != nationalRequired)
                list.Add(National, nationalRequired);
            if (r.RibbonCountry)
                list.Add(Country);
            if (r.RibbonChampionBattle)
                list.Add(ChampionBattle);
            if (r.RibbonChampionRegional)
                list.Add(ChampionRegional);
            if (r.RibbonChampionNational)
                list.Add(ChampionNational);
        }
    }

    public static void ParseEgg(this IRibbonSetEvent3 r, ref RibbonResultList list)
    {
        if (r.RibbonEarth)
            list.Add(Earth);
        if (r.RibbonNational)
            list.Add(National);
        if (r.RibbonCountry)
            list.Add(Country);
        if (r.RibbonChampionBattle)
            list.Add(ChampionBattle);
        if (r.RibbonChampionRegional)
            list.Add(ChampionRegional);
        if (r.RibbonChampionNational)
            list.Add(ChampionNational);
    }
}
