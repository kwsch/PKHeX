using static PKHeX.Core.RibbonIndex;

namespace PKHeX.Core;

/// <summary>
/// Parsing logic for <see cref="IRibbonSetEvent4"/>.
/// </summary>
public static class RibbonVerifierEvent4
{
    public static void Parse(this IRibbonSetEvent4 r, RibbonVerifierArguments args, ref RibbonResultList list)
    {
        var enc = args.Encounter;
        if (r.RibbonClassic && enc is not IRibbonSetEvent4 { RibbonClassic: true })
            list.Add(Classic);
        if ((r.RibbonWishing && enc is not IRibbonSetEvent4 { RibbonWishing: true }) || (enc is EncounterStatic7 { Species: (int)Species.Magearna } && !r.RibbonWishing))
            list.Add(Wishing);
        if (r.RibbonPremier && enc is not IRibbonSetEvent4 { RibbonPremier: true })
            list.Add(Premier);
        if (r.RibbonEvent && enc is not IRibbonSetEvent4 { RibbonEvent: true })
            list.Add(Event);
        if (r.RibbonBirthday && enc is not IRibbonSetEvent4 { RibbonBirthday: true })
            list.Add(Birthday);
        if (r.RibbonSpecial && enc is not IRibbonSetEvent4 { RibbonSpecial: true })
            list.Add(Special);
        if (r.RibbonWorld && enc is not IRibbonSetEvent4 { RibbonWorld: true })
            list.Add(World);
        if (r.RibbonChampionWorld && enc is not IRibbonSetEvent4 { RibbonChampionWorld: true })
            list.Add(ChampionWorld);
        if (r.RibbonSouvenir && enc is not IRibbonSetEvent4 { RibbonSouvenir: true })
            list.Add(Souvenir);
    }

    public static void ParseEgg(this IRibbonSetEvent4 r, RibbonVerifierArguments args, ref RibbonResultList list)
    {
        var enc = args.Encounter;
        if (r.RibbonClassic && enc is not IRibbonSetEvent4 { RibbonClassic: true })
            list.Add(Classic);
        if (r.RibbonWishing && enc is not IRibbonSetEvent4 { RibbonWishing: true })
            list.Add(Wishing);
        if (r.RibbonPremier && enc is not IRibbonSetEvent4 { RibbonPremier: true })
            list.Add(Premier);
        if (r.RibbonEvent && enc is not IRibbonSetEvent4 { RibbonEvent: true })
            list.Add(Event);
        if (r.RibbonBirthday && enc is not IRibbonSetEvent4 { RibbonBirthday: true })
            list.Add(Birthday);
        if (r.RibbonSpecial && enc is not IRibbonSetEvent4 { RibbonSpecial: true })
            list.Add(Special);
        if (r.RibbonWorld && enc is not IRibbonSetEvent4 { RibbonWorld: true })
            list.Add(World);
        if (r.RibbonChampionWorld && enc is not IRibbonSetEvent4 { RibbonChampionWorld: true })
            list.Add(ChampionWorld);
        if (r.RibbonSouvenir && enc is not IRibbonSetEvent4 { RibbonSouvenir: true })
            list.Add(Souvenir);
    }
}
