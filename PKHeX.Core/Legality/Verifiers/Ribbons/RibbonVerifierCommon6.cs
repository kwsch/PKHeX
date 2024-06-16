using static PKHeX.Core.RibbonIndex;

namespace PKHeX.Core;

/// <summary>
/// Parsing logic for <see cref="IRibbonSetCommon6"/>.
/// </summary>
public static class RibbonVerifierCommon6
{
    public static void Parse(this IRibbonSetCommon6 r, in RibbonVerifierArguments args, ref RibbonResultList list)
    {
        if (r is IRibbonSetMemory6 m)
            GetInvalidRibbons6Memory(m, args, ref list);
        var pk = args.Entity;
        var evos = args.History;

        bool gen6 = evos.HasVisitedGen6;
        bool bdsp = evos.HasVisitedBDSP;
        bool kalos6 = gen6 && pk is not { IsUntraded: true, AO: true } && args.Encounter is not EncounterStatic6 { Species: (int)Species.Pikachu, Form: not 0 };
        bool oras6 = gen6 && pk is not { IsUntraded: true, XY: true };
        bool contest = oras6 || bdsp;

        bool k = r.RibbonChampionKalos;
        bool h = r.RibbonChampionG6Hoenn;
        if (k && !kalos6)
            list.Add(ChampionKalos);
        if (h && !oras6)
            list.Add(ChampionG6Hoenn);
        if (!k && !h) // no champ ribbon, check memory.
            CheckChampionMemory(args, ref list);

        if (!contest)
        {
            FlagContest(r, ref list);
        }
        else
        {
            // Winning a contest in Gen6 adds 20 to OT affection. Each ribbon, add 20 to our expected minimum.
            if (pk is IAffection a) // False in Gen8+
                FlagContestAffection(r, ref list, a.OriginalTrainerAffection);

            // Winning all contests grants the Contest Star ribbon.
            // If we have all ribbons and the star is not present, flag it as missing.
            // If we have the star and not all are present, flag it as invalid.
            bool allContest = r.HasAllContestRibbons();
            if (allContest != r.RibbonContestStar)
                list.Add(ContestStar, allContest);
        }

        if (r.RibbonBestFriends && !RibbonRules.IsRibbonValidBestFriends(args.Entity, evos))
            list.Add(BestFriends);

        if (!gen6)
        {
            if (r.RibbonTraining)
                list.Add(Training);

            // Maison
            if (r.RibbonBattlerSkillful)
                list.Add(BattlerSkillful);
            if (r.RibbonBattlerExpert)
                list.Add(BattlerExpert);
        }
        else
        {
            if (r.RibbonTraining && pk is ISuperTrain s && !RibbonRules.IsRibbonValidSuperTraining(s))
                list.Add(Training);

            // Maison
            CheckMaisonRibbons(r, args, ref list);
        }
    }

    public static void ParseEgg(this IRibbonSetCommon6 r, ref RibbonResultList list)
    {
        if (r.RibbonChampionKalos)
            list.Add(ChampionKalos);
        if (r.RibbonChampionG6Hoenn)
            list.Add(ChampionG6Hoenn);
        if (r.RibbonBestFriends)
            list.Add(BestFriends);
        if (r.RibbonTraining)
            list.Add(Training);
        if (r.RibbonBattlerSkillful)
            list.Add(BattlerSkillful);
        if (r.RibbonBattlerExpert)
            list.Add(BattlerExpert);

        FlagContest(r, ref list);
    }

    private static void GetInvalidRibbons6Memory(IRibbonSetMemory6 r, in RibbonVerifierArguments args, ref RibbonResultList list)
    {
        var format = args.Entity.Format;
        (byte contest, byte battle) = RibbonRules.GetMaxMemoryCounts(args.History, args.Entity, args.Encounter);
        if (!IsCountFlagValid(r.RibbonCountMemoryContest, r.HasContestMemoryRibbon, format, contest))
            list.Add(CountMemoryContest);
        if (!IsCountFlagValid(r.RibbonCountMemoryBattle, r.HasBattleMemoryRibbon, format, battle))
            list.Add(CountMemoryBattle);
    }

    private static bool IsCountFlagValid(byte count, bool state, byte format, byte max)
    {
        if (count > max)
            return false;
        bool expect = format >= 8 && count != 0; // Gen8+ use flag, Gen6/7 do not set it.
        return state == expect;
    }

    private static void FlagContestAffection(IRibbonSetCommon6 r, ref RibbonResultList list, int current)
    {
        int expect = 0;
        if (r.RibbonMasterCoolness && current < (expect += 20))
            list.Add(MasterCoolness);
        if (r.RibbonMasterBeauty && current < (expect += 20))
            list.Add(MasterBeauty);
        if (r.RibbonMasterCuteness && current < (expect += 20))
            list.Add(MasterCuteness);
        if (r.RibbonMasterCleverness && current < (expect += 20))
            list.Add(MasterCleverness);
        if (r.RibbonMasterToughness && current < (expect + 20))
            list.Add(MasterToughness);
    }

    private static void FlagContest(IRibbonSetCommon6 r, ref RibbonResultList list)
    {
        if (r.RibbonContestStar)
            list.Add(ContestStar);
        if (r.RibbonMasterCoolness)
            list.Add(MasterCoolness);
        if (r.RibbonMasterBeauty)
            list.Add(MasterBeauty);
        if (r.RibbonMasterCuteness)
            list.Add(MasterCuteness);
        if (r.RibbonMasterCleverness)
            list.Add(MasterCleverness);
        if (r.RibbonMasterToughness)
            list.Add(MasterToughness);
    }

    private static void CheckChampionMemory(in RibbonVerifierArguments args, ref RibbonResultList list)
    {
        var pk = args.Entity;
        var enc = args.Encounter;
        bool hasChampMemory = GetHasGen6ChampMemory(pk, enc);
        if (!hasChampMemory)
            return;

        var ribbon = pk.XY ? ChampionKalos : ChampionG6Hoenn;
        list.Add(ribbon, true);
    }

    private static void CheckMaisonRibbons(IRibbonSetCommon6 r, in RibbonVerifierArguments args, ref RibbonResultList list)
    {
        var pk = args.Entity;
        if (!RibbonRules.IsAllowedBattleFrontier(pk.Species))
        {
            if (r.RibbonBattlerSkillful)
                list.Add(BattlerSkillful);
            if (r.RibbonBattlerExpert)
                list.Add(BattlerExpert);
        }

        // const int memChatelaine = 30;
        // Gen6 can get the memory on those who did not participate by being in the party with other participants.
        // This includes those who cannot enter into the Maison; having memory and no ribbon.
    }

    private static bool GetHasGen6ChampMemory(PKM pk, IEncounterTemplate enc)
    {
        if (pk is not ITrainerMemories m)
            return false;

        // Gen6 can get the memory with any party member when defeating the champion.
        const int memChampion = 27;
        return (enc.Generation == 6 && m.OriginalTrainerMemory == memChampion)
                  || (pk.Format < 8 && m.HandlingTrainerMemory == memChampion);
    }
}
