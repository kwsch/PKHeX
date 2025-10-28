using static PKHeX.Core.RibbonIndex;

namespace PKHeX.Core;

/// <summary>
/// Parsing logic for <see cref="IRibbonSetCommon8"/>.
/// </summary>
public static class RibbonVerifierCommon8
{
    public static void Parse(this IRibbonSetCommon8 r, in RibbonVerifierArguments args, ref RibbonResultList list)
    {
        var evos = args.History;
        if (r.RibbonTowerMaster && !RibbonRules.IsRibbonValidTowerMaster(evos))
            list.Add(TowerMaster);

        var pk = args.Entity;
        bool ranked = evos.HasVisitedSWSH || evos.HasVisitedGen9;

        if (!evos.HasVisitedSWSH)
        {
            if (r.RibbonChampionGalar)
                list.Add(ChampionGalar);
        }

        if (r.RibbonMasterRank && !ranked)
        {
            list.Add(MasterRank);
        }
        else
        {
            // If it can exist in SW/SH, it can have the ribbon.
            // If it doesn't have the ribbon and has the memory of having it, then flag it.
            var enc = args.Encounter;
            if (!r.RibbonChampionGalar)
            {
                const int memChampion = 27;
                bool hasChampMemory = (enc.Generation == 8 && pk is IMemoryOT { OriginalTrainerMemory: memChampion })
                                        || (pk.Format == 8 && pk is IMemoryHT { HandlingTrainerMemory: memChampion });
                if (hasChampMemory)
                    list.Add(ChampionGalar, true);
            }

            if (r.RibbonMasterRank && !RibbonRules.IsRibbonValidMasterRank(pk, enc, evos))
                list.Add(MasterRank);

            if (!r.RibbonTowerMaster)
            {
                // If the Tower Master ribbon is not present but a memory hint implies it should...
                // This memory can also be applied in Gen6/7 via defeating the Chatelaines, where legends are disallowed.
                const int strongest = 30;
                if (pk is IMemoryOT { OriginalTrainerMemory: strongest } or IMemoryHT { HandlingTrainerMemory: strongest })
                {
                    if (enc.Generation == 8 || !RibbonRules.IsAllowedBattleFrontier(pk.Species) || pk is IRibbonSetCommon6 { RibbonBattlerSkillful: false })
                        list.Add(TowerMaster, true);
                }
            }
        }

        if (r.RibbonTwinklingStar && !RibbonRules.IsRibbonValidTwinklingStar(evos, pk))
            list.Add(TwinklingStar);

        // received when capturing photos with Pokémon in the Photography Studio
        if (r.RibbonHisui && !evos.HasVisitedPLA)
            list.Add(Hisui);
    }

    public static void ParseEgg(this IRibbonSetCommon8 r, ref RibbonResultList list)
    {
        if (r.RibbonChampionGalar)
            list.Add(ChampionGalar);
        if (r.RibbonTowerMaster)
            list.Add(TowerMaster);
        if (r.RibbonMasterRank)
            list.Add(MasterRank);
        if (r.RibbonTwinklingStar)
            list.Add(TwinklingStar);
        if (r.RibbonHisui)
            list.Add(Hisui);
    }
}
