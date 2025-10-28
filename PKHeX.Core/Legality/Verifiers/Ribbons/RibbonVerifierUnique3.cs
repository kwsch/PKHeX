using static PKHeX.Core.RibbonIndex3;

namespace PKHeX.Core;

/// <summary>
/// Parsing logic for <see cref="IRibbonSetUnique3"/>.
/// </summary>
public static class RibbonVerifierUnique3
{
    public static void Parse(this IRibbonSetUnique3 r, in RibbonVerifierArguments args, ref RibbonResultList list)
    {
        var evos = args.History;
        if (evos.HasVisitedGen3)
        {
            PKM pk = args.Entity;
            if (r.RibbonWinning && !RibbonRules.IsRibbonValidWinning(pk, args.Encounter, evos))
                list.Add(Winning);
            if (r.RibbonVictory && !RibbonRules.IsRibbonValidVictory(evos))
                list.Add(Victory);
        }
        else // Gen4/5
        {
            if (r.RibbonWinning)
                list.Add(Winning);
            if (r.RibbonVictory)
                list.Add(Victory);
        }
    }

    public static void ParseEgg(this IRibbonSetUnique3 r, ref RibbonResultList list)
    {
        if (r.RibbonWinning)
            list.Add(Winning);
        if (r.RibbonVictory)
            list.Add(Victory);
    }
}
