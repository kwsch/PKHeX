using static PKHeX.Core.RibbonIndex;

namespace PKHeX.Core;

/// <summary>
/// Parsing logic for <see cref="IRibbonSetCommon7"/>.
/// </summary>
public static class RibbonVerifierCommon7
{
    public static void Parse(this IRibbonSetCommon7 r, in RibbonVerifierArguments args, ref RibbonResultList list)
    {
        bool inhabited7 = args.History.HasVisitedGen7;
        bool alolaValid = RibbonRules.IsRibbonValidAlolaChamp(r, args.Encounter, inhabited7);
        if (!alolaValid)
            list.Add(ChampionAlola, !r.RibbonChampionAlola);

        var pk = args.Entity;
        if (inhabited7)
        {
            var species = args.History.Gen7[0].Species;
            bool allowed = RibbonRules.IsAllowedBattleFrontier(species);
            if (allowed)
                return; // Can have all 3 ribbons.

            if (r.RibbonBattleRoyale)
                list.Add(BattleRoyale);
            if (r.RibbonBattleTreeMaster)
                list.Add(BattleTreeMaster);

            // Great ribbon is only available in US/UM for banned species.
            if (r.RibbonBattleTreeGreat && !(pk.USUM || !pk.IsUntraded))
                list.Add(BattleTreeGreat);
            return;
        }

        if (r.RibbonBattleRoyale)
            list.Add(BattleRoyale);
        if (r.RibbonBattleTreeGreat)
            list.Add(BattleTreeGreat);
        if (r.RibbonBattleTreeMaster)
            list.Add(BattleTreeMaster);
    }

    public static void ParseEgg(this IRibbonSetCommon7 r, ref RibbonResultList list)
    {
        if (r.RibbonChampionAlola)
            list.Add(ChampionAlola);

        if (r.RibbonBattleRoyale)
            list.Add(BattleRoyale);
        if (r.RibbonBattleTreeGreat)
            list.Add(BattleTreeGreat);
        if (r.RibbonBattleTreeMaster)
            list.Add(BattleTreeMaster);
    }
}
