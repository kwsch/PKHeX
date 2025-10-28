using static PKHeX.Core.RibbonIndex;

namespace PKHeX.Core;

/// <summary>
/// Parsing logic for <see cref="IRibbonSetEvent4"/>.
/// </summary>
public static class RibbonVerifierEvent4
{
    public static void Parse(this IRibbonSetEvent4 r, in RibbonVerifierArguments args, ref RibbonResultList list)
    {
        var enc = args.Encounter;
        if (enc is IRibbonSetEvent4 e)
            ParseEvent(r, e, ref list);
        else if (enc is EncounterStatic7 { Species: (int)Species.Magearna })
            ParseMagearna7(r, ref list);
        else
            ParseNone(r, ref list);
    }

    private static void ParseNone(IRibbonSetEvent4 r, ref RibbonResultList list)
    {
        if (r.RibbonClassic)
            list.Add(Classic);
        if (r.RibbonWishing)
            list.Add(Wishing);
        if (r.RibbonPremier)
            list.Add(Premier);
        if (r.RibbonEvent)
            list.Add(Event);
        if (r.RibbonBirthday)
            list.Add(Birthday);
        if (r.RibbonSpecial)
            list.Add(Special);
        if (r.RibbonWorld)
            list.Add(World);
        if (r.RibbonChampionWorld)
            list.Add(ChampionWorld);
        if (r.RibbonSouvenir)
            list.Add(Souvenir);
    }

    private static void ParseEvent(IRibbonSetEvent4 r, IRibbonSetEvent4 e, ref RibbonResultList list)
    {
        if (r.RibbonClassic != e.RibbonClassic)
            list.Add(Classic, e.RibbonClassic);
        if (r.RibbonWishing != e.RibbonWishing)
            list.Add(Wishing, e.RibbonWishing);
        if (r.RibbonPremier != e.RibbonPremier)
            list.Add(Premier, e.RibbonPremier);
        if (r.RibbonEvent != e.RibbonEvent)
            list.Add(Event, e.RibbonEvent);
        if (r.RibbonBirthday != e.RibbonBirthday)
            list.Add(Birthday, e.RibbonBirthday);
        if (r.RibbonSpecial != e.RibbonSpecial)
            list.Add(Special, e.RibbonSpecial);
        if (r.RibbonWorld != e.RibbonWorld)
            list.Add(World, e.RibbonWorld);
        if (r.RibbonChampionWorld != e.RibbonChampionWorld)
            list.Add(ChampionWorld, e.RibbonChampionWorld);
        if (r.RibbonSouvenir != e.RibbonSouvenir)
            list.Add(Souvenir, e.RibbonSouvenir);
    }

    private static void ParseMagearna7(IRibbonSetEvent4 r, ref RibbonResultList list)
    {
        if (r.RibbonClassic)
            list.Add(Classic);
        if (!r.RibbonWishing) // true for this oddball encounter.
            list.Add(Wishing, true);
        if (r.RibbonPremier)
            list.Add(Premier);
        if (r.RibbonEvent)
            list.Add(Event);
        if (r.RibbonBirthday)
            list.Add(Birthday);
        if (r.RibbonSpecial)
            list.Add(Special);
        if (r.RibbonWorld)
            list.Add(World);
        if (r.RibbonChampionWorld)
            list.Add(ChampionWorld);
        if (r.RibbonSouvenir)
            list.Add(Souvenir);
    }

    public static void ParseEgg(this IRibbonSetEvent4 r, ref RibbonResultList list, in RibbonVerifierArguments args)
    {
        var enc = args.Encounter;
        if (enc is IRibbonSetEvent4 e)
            ParseEvent(r, e, ref list);
        else
            ParseNone(r, ref list);
    }
}
