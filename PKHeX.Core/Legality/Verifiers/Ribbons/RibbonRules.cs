using System;

namespace PKHeX.Core;

/// <summary>
/// Rules for obtaining ribbons.
/// </summary>
public static class RibbonRules
{
    /// <summary>
    /// Checks if the input can receive the <see cref="IRibbonSetCommon7.RibbonChampionAlola"/> ribbon.
    /// </summary>
    public static bool IsRibbonValidAlolaChamp(IRibbonSetCommon7 s7, IEncounterTemplate enc, bool inhabited7)
    {
        // If the encounter comes with the ribbon, it must have the ribbon.
        if (enc is IRibbonSetCommon7 { RibbonChampionAlola: true })
            return s7.RibbonChampionAlola;
        // If it has visited, it can be either state.
        if (inhabited7)
            return true;
        // If it has not visited, it must not have it.
        return !s7.RibbonChampionAlola;
    }

    /// <summary>
    /// Checks if the input can receive the <see cref="IRibbonSetCommon3.RibbonEffort"/> ribbon.
    /// </summary>
    public static bool IsRibbonValidEffort(PKM pk, EvolutionHistory evos, int gen) => gen switch
    {
        5 when pk.Format == 5 => false, // Not available in BW/B2W2
        8 when !evos.HasVisitedSWSH && !evos.HasVisitedBDSP => false, // not available in PLA
        _ => true,
    };

    /// <summary>
    /// Checks if the input can receive the <see cref="IRibbonSetCommon6.RibbonBestFriends"/> ribbon.
    /// </summary>
    public static bool IsRibbonValidBestFriends(PKM pk, EvolutionHistory evos, int gen) => gen switch
    {
        < 7 when pk is { IsUntraded: true } and IAffection { OT_Affection: < 255 } => false, // Gen6/7 uses affection. Can't lower it on OT!
        8 when !evos.HasVisitedSWSH && !evos.HasVisitedBDSP => false, // Gen8+ replaced with Max Friendship.
        _ => true,
    };

    /// <summary>
    /// Checks if the input can receive the <see cref="IRibbonSetCommon4.RibbonFootprint"/> ribbon.
    /// </summary>
    public static bool IsRibbonValidFootprint(PKM pk, EvolutionHistory evos)
    {
        // Gen3/4: Friendship maxed. Can decrease after obtaining ribbon, no check needed.
        if (evos.HasVisitedGen3 || evos.HasVisitedGen4)
            return true;

        // Gen5: Can't obtain
        if (pk.Format < 6)
            return false;

        // Gen6/7: Increase level by 30 from original level
        static bool IsWellTraveled30(PKM pk) => pk.CurrentLevel - pk.Met_Level >= 30;
        if ((evos.HasVisitedGen6 || evos.HasVisitedGen7) && IsWellTraveled30(pk))
            return true;

        // Gen8-BDSP: Variable by species Footprint
        if (evos.HasVisitedBDSP)
        {
            if (IsAnyWithoutFootprint8b(evos.Gen8b))
                return true; // no footprint
            if (IsWellTraveled30(pk))
                return true; // traveled well
        }

        // Otherwise: Can't obtain
        return false;
    }

    /// <summary>
    /// Checks if the entity participated in SW/SH ranked battles for the <see cref="IRibbonSetCommon8.RibbonMasterRank"/> ribbon.
    /// </summary>
    public static bool IsRibbonValidMasterRankSWSH(PKM pk, IEncounterTemplate enc, EvolutionHistory evos)
    {
        if (!evos.HasVisitedSWSH)
            return false;

        if (enc.Generation < 8 && pk is IBattleVersion { BattleVersion: 0 })
            return false;

        // GO transfers: Capture date is global time, and not console changeable.
        bool hasRealDate = enc.Version == GameVersion.GO || enc is IEncounterServerDate { IsDateRestricted: true };
        if (hasRealDate)
        {
            var met = pk.MetDate;
            if (met > new DateTime(2022, 11, 1)) // Series 13 end date +1 day (wiggle room)
                return false; // Ranked is done!
        }

        // Series 13 rule-set was the first time Ranked Battles allowed the use of Mythical Pokémon.
        // All species that can exist in SW/SH can compete in ranked.
        return true;
    }

    /// <summary>
    /// Checks if the input can receive the <see cref="IRibbonSetCommon6.RibbonTraining"/> ribbon.
    /// </summary>
    public static bool IsRibbonValidSuperTraining(ISuperTrain pk)
    {
        // It is assumed that the entity existed in the Gen6 game to receive the ribbon.
        // We only enter this method if the entity implements the interface.
        const int req = 12; // only first 12 are required to get the ribbon.
        int count = pk.SuperTrainingMedalCount(req);
        return count >= req;
    }

    /// <summary>
    /// Checks if the entity participated in battles for the <see cref="IRibbonSetCommon8.RibbonTowerMaster"/> ribbon.
    /// </summary>
    public static bool IsRibbonValidTowerMaster(EvolutionHistory evos)
    {
        if (evos.HasVisitedSWSH)
            return true; // Anything in SW/SH can be used in battle tower.

        if (!evos.HasVisitedBDSP)
            return false;
        // Mythicals cannot be used in BD/SP's Battle Tower
        return !Legal.Mythicals.Contains(evos.Gen8b[0].Species);
    }

    /// <summary>
    /// Checks if the input can receive the <see cref="IRibbonSetUnique3.RibbonWinning"/> ribbon.
    /// </summary>
    public static bool IsRibbonValidWinning(PKM pk, IEncounterTemplate enc, EvolutionHistory evos)
    {
        if (!evos.HasVisitedGen3)
            return false;
        if (!IsAllowedBattleFrontier(evos.Gen3[0].Species))
            return false;

        // Can only obtain if the current level on receiving the ribbon is <= level 50.
        if (pk.Format == 3) // Stored value is not yet overwritten (G3->G4), check directly.
            return pk.Met_Level <= 50;

        // Most encounter types can be below level 50; only Shadow Dragonite & Tyranitar, and select Gen3 Event Gifts.
        // These edge cases can't be obtained below level 50, unlike some wild Pokémon which can be encountered at different locations for lower levels.
        if (enc.LevelMin <= 50)
            return true;

        return enc is not (EncounterStaticShadow or WC3);
    }

    /// <summary>
    /// Checks if the input can receive the <see cref="IRibbonSetUnique3.RibbonVictory"/> ribbon.
    /// </summary>
    public static bool IsRibbonValidVictory(EvolutionHistory evos)
    {
        if (evos.HasVisitedGen3)
            return IsAllowedBattleFrontier(evos.Gen3[0].Species);
        return false;
    }

    /// <summary>
    /// Checks if the input can receive the <see cref="IRibbonSetCommon8.RibbonTwinklingStar"/> ribbon.
    /// </summary>
    public static bool IsRibbonValidTwinklingStar(EvolutionHistory evos, PKM pk)
    {
        // Can currently only obtain from BD/SP.
        if (!evos.HasVisitedBDSP)
            return false;

        // Can only obtain if it has already completed all the other contests and received the summation ribbon.
        if (pk is IRibbonSetCommon6 { RibbonContestStar: false })
            return false;

        return true;
    }

    /// <summary>
    /// Checks if any of the species it existed as in BD/SP lacked footprints.
    /// </summary>
    private static bool IsAnyWithoutFootprint8b(EvoCriteria[] evos)
    {
        var arr = HasFootprintBDSP;
        foreach (var evo in evos)
        {
            var species = evo.Species;
            if ((uint)species >= arr.Length)
                continue;
            if (!arr[species])
                return true;
        }
        return false;
    }

    // Derived from ROM data: true for all Footprint types besides 5 (5 = no feet).
    // If true, requires gaining 30 levels to obtain ribbon. If false, can obtain ribbon at any level.
    private static readonly bool[] HasFootprintBDSP =
    {
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true, false,  true,  true, false,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true, false, false,  true, false,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true, false, false,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        false, false,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true,  true, false,  true,  true,
        false,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true,  true,  true, false,  true,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true, false,  true,  true, false, false,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true, false,  true,  true,  true,  true,  true,  true,
        true,  true,  true, false,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true,  true, false,  true,  true,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true, false,  true, false,  true,
        true,  true,  true, false,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        false,  true,  true,  true,  true,  true,  true,  true,  true, false,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true,  true, false, false,  true,
        true,  true,  true, false, false, false, false, false,  true,  true,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true, false,  true, false, false,  true, false, false, false,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true, false, false,  true,  true,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true,  true,  true,  true,  true,  true,  true,
        true,  true, false,  true,  true,  true,  true,  true,  true,  true,
        true,  true,  true,  true, false,  true, false,  true,  true,  true,
        true,  true,  true,  true,  true,  true, false,  true,  true,  true,
        true,  true,  true,  true,
    };

    /// <summary>
    /// Checks if the input can receive the <see cref="IRibbonSetEvent3.RibbonNational"/> ribbon.
    /// </summary>
    /// <remarks>
    /// If returns true, must have the ribbon. If returns false, must not have the ribbon.
    /// </remarks>
    public static bool GetValidRibbonStateNational(PKM pk, IEncounterTemplate enc)
    {
        // Can only obtain from Generation 3 Shadow Pokémon
        if (enc.Generation != 3)
            return false;

        if (enc is not EncounterStaticShadow)
            return false;

        // Ribbon is awarded when the Pokémon is purified in the game of origin.
        if (pk is IShadowPKM { IsShadow: true })
            return false;

        return true;
    }

    /// <summary>
    /// Gets the max count values the input can receive for the <see cref="IRibbonSetCommon6.RibbonCountMemoryContest"/> and <see cref="IRibbonSetCommon6.RibbonCountMemoryBattle"/> ribbon counts.
    /// </summary>
    public static (byte Contest, byte Battle) GetMaxMemoryCounts(EvolutionHistory evos, PKM pk, IEncounterTemplate enc)
    {
        // Contest: 20 in both Generations.
        const byte MaxContest4 = 20;
        const byte MaxContest3 = 20;
        const byte MaxContestBoth = MaxContest3 + MaxContest4; // 40

        // Battle: 2 in Gen3, 6 in Gen4; one (Winning) in Gen3 has extra restrictions.
        const byte MaxBattle3 = 2;
        const byte MaxBattle4 = 6;
        const byte MaxBattleBoth = MaxBattle3 + MaxBattle4; // 8
        const byte MaxBattleBothNoWinning = MaxBattleBoth - 1; // 7

        if (evos.HasVisitedGen3)
        {
            var head = evos.Gen3[0]; // Checking contest with Gen3 head is fine; all false cases cannot evolve (evolution chain is same Gen3/Gen4).
            var contest = IsAllowedContest4(head.Species, head.Form) ? MaxContestBoth : MaxContest3;
            var battle = IsAllowedBattleFrontier(head.Species) ? IsRibbonValidWinning(pk, enc, evos) ? MaxBattleBoth : MaxBattleBothNoWinning : (byte)0;
            return (contest, battle);
        }
        if (evos.HasVisitedGen4)
        {
            var head = evos.Gen4[0];
            var contest = IsAllowedContest4(head.Species, head.Form) ? MaxContest4 : (byte)0;
            var battle = IsAllowedBattleFrontier(head.Species) ? MaxBattle4 : (byte)0;
            return (contest, battle);
        }
        return default;
    }

    public static bool IsAllowedContest3(EvolutionHistory evos)
    {
        // Any species can enter contests in Gen3.
        return evos.HasVisitedGen3;
    }

    public static bool IsAllowedContest4(EvolutionHistory evos)
    {
        if (!evos.HasVisitedGen4)
            return false;
        var head = evos.Gen4[0];
        return IsAllowedContest4(head.Species, head.Form);
    }

    public static bool IsAllowedContest4(int species, int form) => species switch
    {
        // Disallow Unown and Ditto, and Spiky Pichu (cannot trade)
        (int)Species.Ditto => false,
        (int)Species.Unown => false,
        (int)Species.Pichu when form == 1 => false,
        _ => true,
    };

    public static bool IsAllowedBattleFrontier(int species) => !Legal.BattleFrontierBanlist.Contains(species);

    public static bool IsAllowedBattleFrontier4(EvolutionHistory evos)
    {
        if (!evos.HasVisitedGen4)
            return false;
        var head = evos.Gen4[0];
        return IsAllowedBattleFrontier(head.Species, head.Form, 4);
    }

    public static bool IsAllowedBattleFrontier(int species, int form, int gen)
    {
        if (gen == 4 && species == (int)Species.Pichu && form == 1) // spiky
            return false;
        return IsAllowedBattleFrontier(species);
    }
}
