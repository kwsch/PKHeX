using static PKHeX.Core.RibbonIndex;

namespace PKHeX.Core;

/// <summary>
/// Parsing logic for <see cref="IRibbonSetMark9"/>.
/// </summary>
public static class RibbonVerifierMark9
{
    public static void Parse(this IRibbonSetMark9 r, in RibbonVerifierArguments args, ref RibbonResultList list)
    {
        if (!MarkRules.IsMarkValidAlpha(args.Encounter, args.Entity))
            list.Add(MarkAlpha, !r.RibbonMarkAlpha);
        if (r.RibbonMarkGourmand && !MarkRules.IsMarkValidGourmand(args.History))
            list.Add(MarkGourmand);
        if (r.RibbonMarkItemfinder && !MarkRules.IsMarkValidItemFinder(args.History))
            list.Add(MarkItemfinder);
        if (r.RibbonMarkJumbo && !MarkRules.IsMarkAllowedJumbo(args.History, args.Entity))
            list.Add(MarkJumbo);
        if (!MarkRules.IsMarkValidMightiest(args.Encounter, r.RibbonMarkMightiest, args.History))
            list.Add(MarkMightiest, !r.RibbonMarkMightiest);
        if (r.RibbonMarkMini && !MarkRules.IsMarkAllowedMini(args.History, args.Entity))
            list.Add(MarkMini);
        if (r.RibbonMarkPartner && !MarkRules.IsMarkValidPartner(args.History))
            list.Add(MarkPartner);
        if (r.RibbonMarkTitan != MarkRules.IsMarkPresentTitan(args.Encounter))
            list.Add(MarkTitan, !r.RibbonMarkTitan);
    }

    public static void ParseEgg(this IRibbonSetMark9 r, ref RibbonResultList list)
    {
        if (r.RibbonMarkAlpha)
            list.Add(MarkAlpha);
        // Can apply to eggs if in party. Just can't affix.
        // if (r.RibbonMarkGourmand)
        //     list.Add(MarkGourmand);
        if (r.RibbonMarkItemfinder)
            list.Add(MarkItemfinder);
        if (r.RibbonMarkJumbo)
            list.Add(MarkJumbo);
        if (r.RibbonMarkMightiest)
            list.Add(MarkMightiest);
        if (r.RibbonMarkMini)
            list.Add(MarkMini);
        if (r.RibbonMarkPartner)
            list.Add(MarkPartner);
        if (r.RibbonMarkTitan)
            list.Add(MarkTitan);
    }
}
