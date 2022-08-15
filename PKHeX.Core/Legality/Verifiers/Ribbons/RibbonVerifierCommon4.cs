using static PKHeX.Core.RibbonIndex;

namespace PKHeX.Core;

public static class RibbonVerifierCommon4
{
    public static void Parse(this IRibbonSetCommon4 r, RibbonVerifierArguments args, ref RibbonResultList list)
    {
        var evos = args.History;
        bool inhabited4 = evos.HasVisitedGen3 || evos.HasVisitedGen4;
        if (!inhabited4)
        {
            // Allow Sinnoh Champion if visited BD/SP. ILCA reused the Gen4 ribbon for the remake.
            if (r.RibbonChampionSinnoh && !evos.HasVisitedBDSP)
                list.Add(ChampionSinnoh);
            if (r.RibbonLegend)
                list.Add(Legend);
        }

        if (r.RibbonRecord)
            list.Add(Record); // Unobtainable
        var pk = args.Entity;
        if (r.RibbonFootprint && !RibbonRules.IsRibbonValidFootprint(pk, evos))
            list.Add(Footprint);

        bool visitBDSP = evos.HasVisitedBDSP;
        bool gen34 = evos.HasVisitedGen3 || evos.HasVisitedGen4;
        bool not6 = !evos.HasVisitedGen6;
        bool noDaily = !gen34 && not6 && !visitBDSP;
        bool noSinnoh = pk is G4PKM { Species: (int)Species.Pichu, Form: 1 }; // Spiky Pichu
        bool noCosmetic = (!gen34 && (not6 || (pk.XY && pk.IsUntraded)) && !visitBDSP) || noSinnoh;

        if (noSinnoh)
        {
            if (r.RibbonChampionSinnoh)
                list.Add(ChampionSinnoh);
        }

        if (noDaily)
        {
            if (r.RibbonAlert) list.Add(Alert);
            if (r.RibbonShock) list.Add(Shock);
            if (r.RibbonDowncast) list.Add(Downcast);
            if (r.RibbonCareless) list.Add(Careless);
            if (r.RibbonRelax) list.Add(Relax);
            if (r.RibbonSnooze) list.Add(Snooze);
            if (r.RibbonSmile) list.Add(Smile);
        }

        if (noCosmetic)
        {
            if (r.RibbonGorgeous) list.Add(Gorgeous);
            if (r.RibbonRoyal) list.Add(Royal);
            if (r.RibbonGorgeousRoyal) list.Add(GorgeousRoyal);
        }
    }

    public static void ParseEgg(this IRibbonSetCommon4 r, ref RibbonResultList list)
    {
        if (r.RibbonChampionSinnoh)
            list.Add(ChampionSinnoh);

        if (r.RibbonRecord)
            list.Add(Record);
        if (r.RibbonLegend)
            list.Add(Legend);

        if (r.RibbonFootprint)
            list.Add(Footprint);

        if (r.RibbonAlert) list.Add(Alert);
        if (r.RibbonShock) list.Add(Shock);
        if (r.RibbonDowncast) list.Add(Downcast);
        if (r.RibbonCareless) list.Add(Careless);
        if (r.RibbonRelax) list.Add(Relax);
        if (r.RibbonSnooze) list.Add(Snooze);
        if (r.RibbonSmile) list.Add(Smile);

        if (r.RibbonGorgeous) list.Add(Gorgeous);
        if (r.RibbonRoyal) list.Add(Royal);
        if (r.RibbonGorgeousRoyal) list.Add(GorgeousRoyal);
    }
}
