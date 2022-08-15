using static PKHeX.Core.RibbonIndex;

namespace PKHeX.Core;

public static class RibbonVerifierCommon6
{
    public static void Parse(this IRibbonSetCommon6 r, RibbonVerifierArguments args, ref RibbonResultList list)
    {
        var evos = args.History;
        bool inhabited6 = evos.HasVisitedGen6;

        GetInvalidRibbons6Memory(r, args, ref list);
        if (inhabited6)
            GetInvalidRibbons6Any(r, args, ref list);
        else if (args.Entity.Format >= 8)
            GetInvalidRibbons6AnyG8(r, args, ref list);
        else
            GetInvalidRibbons6None(r, ref list);

        if (!inhabited6)
        {
            if (r.RibbonCountMemoryContest > 0)
                list.Add(CountMemoryContest);
            if (r.RibbonCountMemoryBattle > 0)
                list.Add(CountMemoryBattle);
        }

        if (r.RibbonBestFriends && !RibbonRules.IsRibbonValidBestFriend(args.Entity, evos, args.Encounter.Generation))
            list.Add(BestFriends);
    }

    public static void ParseEgg(this IRibbonSetCommon6 r, RibbonVerifierArguments args, ref RibbonResultList list)
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
        if (r.RibbonContestStar)
            list.Add(ContestStar);
    }

    private static void GetInvalidRibbons6Any(IRibbonSetCommon6 r, RibbonVerifierArguments args, ref RibbonResultList list)
    {
        var pk = args.Entity;
        var enc = args.Encounter;
        bool untraded = pk.IsUntraded || (enc is EncounterStatic6 { Species: (int)Species.Pikachu, Form: not 0 }); // Disallow cosplay pikachu from XY ribbons
        if (untraded)
            GetInvalidRibbons6Untraded(r, args, ref list);
        else
            GetInvalidRibbons6Traded(r, args, ref list);

        bool allContest = r.HasAllContestRibbons();
        if ((allContest != r.RibbonContestStar) && !(untraded && pk.XY)) // if not already checked
            list.Add(ContestStar, !r.RibbonContestStar);

        // Each contest victory requires a contest participation; each participation gives 20 OT affection (not current trainer).
        // Affection is discarded on PK7->PK8 in favor of friendship, which can be lowered.
        if (pk is IAffection a)
        {
            int expect = 0;
            var current = a.OT_Affection;
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

        // Gen6 can get the memory on those who did not participate by being in the party with other participants.
        // This includes those who cannot enter into the Maison; having memory and no ribbon.
        const int memChatelaine = 30;
        bool hasChampMemory = enc.Context == EntityContext.Gen7 && pk.Context == EntityContext.Gen7 && pk is ITrainerMemories m && (m.HT_Memory == memChatelaine || m.OT_Memory == memChatelaine);
        if (!RibbonRules.IsAllowedBattleFrontier(pk.Species))
        {
            if (hasChampMemory || r.RibbonBattlerSkillful) // having memory and not ribbon is too rare, just flag here.
                list.Add(BattlerSkillful);
            if (r.RibbonBattlerExpert)
                list.Add(BattlerExpert);
            return;
        }

        if (!hasChampMemory || r.RibbonBattlerSkillful || r.RibbonBattlerExpert)
            return;

        list.Add(BattlerSkillful, true);
        list.Add(BattlerExpert, true);
    }

    private static void GetInvalidRibbons6AnyG8(IRibbonSetCommon6 r, RibbonVerifierArguments args, ref RibbonResultList list)
    {
        if (!args.History.HasVisitedBDSP)
        {
            if (r.RibbonChampionKalos) list.Add(ChampionKalos);
            if (r.RibbonChampionG6Hoenn) list.Add(ChampionG6Hoenn);
            // RibbonBestFriends
            if (r.RibbonTraining) list.Add(Training);
            if (r.RibbonBattlerSkillful) list.Add(BattlerSkillful);
            if (r.RibbonBattlerExpert) list.Add(BattlerExpert);
            if (r.RibbonContestStar) list.Add(ContestStar);
            if (r.RibbonMasterCoolness) list.Add(MasterCoolness);
            if (r.RibbonMasterBeauty) list.Add(MasterBeauty);
            if (r.RibbonMasterCuteness) list.Add(MasterCuteness);
            if (r.RibbonMasterCleverness) list.Add(MasterCleverness);
            if (r.RibbonMasterToughness) list.Add(MasterToughness);
            return;
        }

        if (r.RibbonChampionKalos)
            list.Add(ChampionKalos);
        if (r.RibbonChampionG6Hoenn)
            list.Add(ChampionG6Hoenn);
        //if (s6.RibbonBestFriends)
        //    list.Add(BestFriends);
        if (r.RibbonTraining)
            list.Add(Training);
        if (r.RibbonBattlerSkillful)
            list.Add(BattlerSkillful);
        if (r.RibbonBattlerExpert)
            list.Add(BattlerExpert);

        // Can get contest ribbons via BD/SP contests.
        //if (s6.RibbonContestStar)
        //    list.Add(ContestStar);
        //if (s6.RibbonMasterCoolness)
        //    list.Add(MasterCoolness);
        //if (s6.RibbonMasterBeauty)
        //    list.Add(MasterBeauty);
        //if (s6.RibbonMasterCuteness)
        //    list.Add(MasterCuteness);
        //if (s6.RibbonMasterCleverness)
        //    list.Add(MasterCleverness);
        //if (s6.RibbonMasterToughness)
        //    list.Add(MasterToughness);

        bool hasAllContest = r.HasAllContestRibbons();
        if (hasAllContest != r.RibbonContestStar) // if not already checked
            list.Add(ContestStar, r.RibbonContestStar);
    }

    private static void GetInvalidRibbons6Memory(IRibbonSetCommon6 r, RibbonVerifierArguments args, ref RibbonResultList list)
    {
        (int contest, int battle) = RibbonRules.GetMaxMemoryCounts(args.History, args.Entity, args.Encounter);
        if (r.RibbonCountMemoryContest > contest)
            list.Add(CountMemoryContest);
        if (r.RibbonCountMemoryBattle > battle)
            list.Add(CountMemoryBattle);
    }

    private static void GetInvalidRibbons6Untraded(IRibbonSetCommon6 r, RibbonVerifierArguments args, ref RibbonResultList list)
    {
        var pk = args.Entity;
        if (pk.XY)
        {
            if (r.RibbonChampionG6Hoenn)
                list.Add(ChampionG6Hoenn);

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
        else if (pk.AO)
        {
            if (r.RibbonChampionKalos)
                list.Add(ChampionKalos);
        }
    }

    private static void GetInvalidRibbons6Traded(IRibbonSetCommon6 r, RibbonVerifierArguments args, ref RibbonResultList list)
    {
        var pk = args.Entity;
        // Medal count is wiped on transfer to pk8
        if (r.RibbonTraining && pk is ISuperTrain s && !RibbonRules.IsRibbonValidSuperTraining(s))
            list.Add(Training);

        if (pk is not ITrainerMemories m)
            return;

        const int memChampion = 27;
        bool hasChampMemory = (args.Encounter.Generation == 6 && m.OT_Memory == memChampion)
                              || (pk.Format < 8 && m.HT_Memory == memChampion);
        if (hasChampMemory && !r.RibbonChampionKalos && !r.RibbonChampionG6Hoenn)
        {
            list.Add(ChampionKalos, true);
            list.Add(ChampionG6Hoenn, true);
        }
    }

    private static void GetInvalidRibbons6None(IRibbonSetCommon6 r, ref RibbonResultList list)
    {
        if (r.RibbonChampionKalos) list.Add(ChampionKalos);
        if (r.RibbonChampionG6Hoenn) list.Add(ChampionG6Hoenn);
        //if (s6.RibbonBestFriends) list.Add(BestFriends);
        if (r.RibbonTraining) list.Add(Training);
        if (r.RibbonBattlerSkillful) list.Add(BattlerSkillful);
        if (r.RibbonBattlerExpert) list.Add(BattlerExpert);
        if (r.RibbonContestStar) list.Add(ContestStar);
        if (r.RibbonMasterCoolness) list.Add(MasterCoolness);
        if (r.RibbonMasterBeauty) list.Add(MasterBeauty);
        if (r.RibbonMasterCuteness) list.Add(MasterCuteness);
        if (r.RibbonMasterCleverness) list.Add(MasterCleverness);
        if (r.RibbonMasterToughness) list.Add(MasterToughness);
    }
}
