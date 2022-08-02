using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="PKM"/> Ribbon values.
/// </summary>
public sealed class RibbonVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Ribbon;

    public override void Verify(LegalityAnalysis data)
    {
        // Flag VC (Gen1/2) ribbons using Gen7 origin rules.
        var enc = data.EncounterMatch;
        var pk = data.Entity;

        // Check Unobtainable Ribbons
        if (pk.IsEgg)
        {
            if (GetIncorrectRibbonsEgg(pk, enc))
                data.AddLine(GetInvalid(LRibbonEgg));
            return;
        }

        var result = GetIncorrectRibbons(pk, data.Info.EvoChainsAllGens, enc);
        if (result.Count != 0)
        {
            var msg = string.Join(Environment.NewLine, result);
            data.AddLine(GetInvalid(msg));
        }
        else
        {
            data.AddLine(GetValid(LRibbonAllValid));
        }
    }

    private static List<string> GetIncorrectRibbons(PKM pk, EvolutionHistory evos, IEncounterTemplate enc)
    {
        List<string> missingRibbons = new();
        List<string> invalidRibbons = new();
        var ribs = GetRibbonResults(pk, evos, enc);
        foreach (var bad in ribs)
            (bad.Invalid ? invalidRibbons : missingRibbons).Add(bad.Name);

        var result = new List<string>();
        if (missingRibbons.Count > 0)
            result.Add(string.Format(LRibbonFMissing_0, string.Join(", ", missingRibbons).Replace(RibbonInfo.PropertyPrefix, string.Empty)));
        if (invalidRibbons.Count > 0)
            result.Add(string.Format(LRibbonFInvalid_0, string.Join(", ", invalidRibbons).Replace(RibbonInfo.PropertyPrefix, string.Empty)));
        return result;
    }

    private static bool GetIncorrectRibbonsEgg(PKM pk, IEncounterTemplate enc)
    {
        var names = ReflectUtil.GetPropertiesStartWithPrefix(pk.GetType(), RibbonInfo.PropertyPrefix);
        if (enc is IRibbonSetEvent3 event3)
            names = names.Except(event3.RibbonNames());
        if (enc is IRibbonSetEvent4 event4)
            names = names.Except(event4.RibbonNames());

        foreach (var value in names.Select(name => ReflectUtil.GetValue(pk, name)))
        {
            if (value is null)
                continue;
            if (HasFlag(value) || HasCount(value))
                return true;

            static bool HasFlag(object o) => o is true;
            static bool HasCount(object o) => o is > 0;
        }
        return false;
    }

    internal static IEnumerable<RibbonResult> GetRibbonResults(PKM pk, EvolutionHistory evos, IEncounterTemplate enc)
    {
        return GetInvalidRibbons(pk, evos, enc)
            .Concat(GetInvalidRibbonsEvent1(pk, enc))
            .Concat(GetInvalidRibbonsEvent2(pk, enc));
    }

    private static IEnumerable<RibbonResult> GetInvalidRibbons(PKM pk, EvolutionHistory evos, IEncounterTemplate enc)
    {
        // is a part of Event4, but O3 doesn't have the others
        if (pk is IRibbonSetOnly3 {RibbonWorld: true})
            yield return new RibbonResult(nameof(IRibbonSetOnly3.RibbonWorld));

        if (pk is IRibbonSetUnique3 u3)
        {
            if (enc.Generation != 3)
            {
                if (u3.RibbonWinning)
                    yield return new RibbonResult(nameof(u3.RibbonWinning));
                if (u3.RibbonVictory)
                    yield return new RibbonResult(nameof(u3.RibbonVictory));
            }
            else
            {
                if (u3.RibbonWinning && !CanHaveRibbonWinning(pk, enc, 3))
                    yield return new RibbonResult(nameof(u3.RibbonWinning));
                if (u3.RibbonVictory && !CanHaveRibbonVictory(pk, 3))
                    yield return new RibbonResult(nameof(u3.RibbonVictory));
            }
        }

        int gen = enc.Generation;
        if (pk is IRibbonSetUnique4 u4)
        {
            if (!IsAllowedBattleFrontier(pk.Species, pk.Form, 4) || gen > 4)
            {
                foreach (var z in GetInvalidRibbonsNone(u4.RibbonBitsAbility(), u4.RibbonNamesAbility()))
                    yield return z;
            }

            var c3 = u4.RibbonBitsContest3();
            var c3n = u4.RibbonNamesContest3();
            var iter3 = gen == 3 ? GetMissingContestRibbons(c3, c3n) : GetInvalidRibbonsNone(c3, c3n);
            foreach (var z in iter3)
                yield return z;

            var c4 = u4.RibbonBitsContest4();
            var c4n = u4.RibbonNamesContest4();
            var iter4 = (gen is 3 or 4) && IsAllowedInContest4(pk.Species, pk.Form) ? GetMissingContestRibbons(c4, c4n) : GetInvalidRibbonsNone(c4, c4n);
            foreach (var z in iter4)
                yield return z;
        }
        if (pk is IRibbonSetCommon4 s4)
        {
            bool inhabited4 = gen is 3 or 4;
            var iterate = GetInvalidRibbons4Any(pk, evos, s4, gen);
            if (!inhabited4)
            {
                if (pk.HasVisitedBDSP(evos.Gen8b)) // Allow Sinnoh Champion. ILCA reused the Gen4 ribbon for the remake.
                    iterate = iterate.Concat(GetInvalidRibbonsNoneSkipIndex(s4.RibbonBitsOnly(), s4.RibbonNamesOnly(), 1));
                else
                    iterate = iterate.Concat(GetInvalidRibbonsNone(s4.RibbonBitsOnly(), s4.RibbonNamesOnly()));
            }
            foreach (var z in iterate)
                yield return z;
        }
        if (pk is IRibbonSetCommon6 s6)
        {
            bool inhabited6 = gen is >= 3 and <= 6;

            var iterate = inhabited6
                ? GetInvalidRibbons6Any(pk, s6, gen, enc)
                : pk.Format >= 8
                    ? GetInvalidRibbons6AnyG8(pk, s6, evos)
                    : GetInvalidRibbonsNone(s6.RibbonBits(), s6.RibbonNamesBool());
            foreach (var z in iterate)
                yield return z;

            if (!inhabited6)
            {
                if (s6.RibbonCountMemoryContest > 0)
                    yield return new RibbonResult(nameof(s6.RibbonCountMemoryContest));
                if (s6.RibbonCountMemoryBattle > 0)
                    yield return new RibbonResult(nameof(s6.RibbonCountMemoryBattle));
            }

            if (s6.RibbonBestFriends && !IsRibbonValidBestFriend(pk, evos, gen))
                yield return new RibbonResult(nameof(IRibbonSetCommon6.RibbonBestFriends));
        }
        if (pk is IRibbonSetCommon7 s7)
        {
            bool inhabited7 = gen <= 7 && !pk.GG;
            var iterate = inhabited7 ? GetInvalidRibbons7Any(pk, s7) : GetInvalidRibbonsNone(s7.RibbonBits(), s7.RibbonNames());
            foreach (var z in iterate)
                yield return z;
        }
        if (pk is IRibbonSetCommon3 s3)
        {
            if (s3.RibbonChampionG3 && gen != 3)
                yield return new RibbonResult(nameof(s3.RibbonChampionG3)); // RSE HoF
            if (s3.RibbonArtist && gen != 3)
                yield return new RibbonResult(nameof(s3.RibbonArtist)); // RSE Master Rank Portrait
            if (s3.RibbonEffort && !IsRibbonValidEffort(pk, evos, gen)) // unobtainable in Gen 5
                yield return new RibbonResult(nameof(s3.RibbonEffort));
        }
        if (pk is IRibbonSetCommon8 s8)
        {
            bool inhabited8 = gen <= 8;
            var iterate = inhabited8 ? GetInvalidRibbons8Any(pk, s8, enc, evos) : GetInvalidRibbonsNone(s8.RibbonBits(), s8.RibbonNames());
            foreach (var z in iterate)
                yield return z;
        }
    }

    private static bool IsRibbonValidEffort(PKM pk, EvolutionHistory evos, int gen) => gen switch
    {
        5 when pk.Format == 5 => false,
        8 when !pk.HasVisitedSWSH(evos.Gen8) && !pk.HasVisitedBDSP(evos.Gen8b) => false,
        _ => true,
    };

    private static bool IsRibbonValidBestFriend(PKM pk, EvolutionHistory evos, int gen) => gen switch
    {
        < 7 when pk is { IsUntraded: true } and IAffection { OT_Affection: < 255 } => false, // Gen6/7 uses affection. Can't lower it on OT!
        8 when !pk.HasVisitedSWSH(evos.Gen8) && !pk.HasVisitedBDSP(evos.Gen8b) => false, // Gen8+ replaced with Max Friendship.
        _ => true,
    };

    private static IEnumerable<RibbonResult> GetMissingContestRibbons(IReadOnlyList<bool> bits, IReadOnlyList<string> names)
    {
        for (int i = 0; i < bits.Count; i += 4)
        {
            bool required = false;
            for (int j = i + 3; j >= i; j--)
            {
                if (bits[j])
                    required = true;
                else if (required) yield return new RibbonResult(names[j], false);
            }
        }
    }

    private static IEnumerable<RibbonResult> GetInvalidRibbons4Any(PKM pk, EvolutionHistory evos, IRibbonSetCommon4 s4, int gen)
    {
        if (s4.RibbonRecord)
            yield return new RibbonResult(nameof(s4.RibbonRecord)); // Unobtainable
        if (s4.RibbonFootprint && !CanHaveFootprintRibbon(pk, evos, gen))
            yield return new RibbonResult(nameof(s4.RibbonFootprint));

        bool visitBDSP = pk.HasVisitedBDSP(evos.Gen8b);
        bool gen34 = gen is 3 or 4;
        bool not6 = pk.Format < 6 || gen is > 6 or < 3;
        bool noDaily = !gen34 && not6 && !visitBDSP;
        bool noSinnoh = pk is G4PKM { Species: (int)Species.Pichu, Form: 1 }; // Spiky Pichu
        bool noCosmetic = (!gen34 && (not6 || (pk.XY && pk.IsUntraded)) && !visitBDSP) || noSinnoh;

        if (noSinnoh)
        {
            if (s4.RibbonChampionSinnoh)
                yield return new RibbonResult(nameof(s4.RibbonChampionSinnoh));
        }

        if (noDaily)
        {
            foreach (var z in GetInvalidRibbonsNone(s4.RibbonBitsDaily(), s4.RibbonNamesDaily()))
                yield return z;
        }

        if (noCosmetic)
        {
            foreach (var z in GetInvalidRibbonsNone(s4.RibbonBitsCosmetic(), s4.RibbonNamesCosmetic()))
                yield return z;
        }
    }

    private static IEnumerable<RibbonResult> GetInvalidRibbons6Any(PKM pk, IRibbonSetCommon6 s6, int gen, IEncounterTemplate enc)
    {
        foreach (var p in GetInvalidRibbons6Memory(pk, s6, gen, enc))
            yield return p;

        bool untraded = pk.IsUntraded || (enc is EncounterStatic6 {Species:(int)Species.Pikachu, Form: not 0}); // Disallow cosplay pikachu from XY ribbons
        var iter = untraded ? GetInvalidRibbons6Untraded(pk, s6) : GetInvalidRibbons6Traded(pk, s6);
        foreach (var p in iter)
            yield return p;

        var contest = s6.RibbonBitsContest();
        bool allContest = contest.All(z => z);
        if ((allContest != s6.RibbonContestStar) && !(untraded && pk.XY)) // if not already checked
            yield return new RibbonResult(nameof(s6.RibbonContestStar), s6.RibbonContestStar);

        // Each contest victory requires a contest participation; each participation gives 20 OT affection (not current trainer).
        // Affection is discarded on PK7->PK8 in favor of friendship, which can be lowered.
        if (pk is IAffection a)
        {
            var affect = a.OT_Affection;
            var contMemory = s6.RibbonNamesContest();
            int contCount = 0;
            var present = contMemory.Where((_, i) => contest[i] && affect < 20 * ++contCount);
            foreach (var rib in present)
                yield return new RibbonResult(rib);
        }

        // Gen6 can get the memory on those who did not participate by being in the party with other participants.
        // This includes those who cannot enter into the Maison; having memory and no ribbon.
        const int memChatelaine = 30;
        bool hasChampMemory = enc.Generation == 7 && pk.Format == 7 && pk is ITrainerMemories m && (m.HT_Memory == memChatelaine || m.OT_Memory == memChatelaine);
        if (!IsAllowedBattleFrontier(pk.Species))
        {
            if (hasChampMemory || s6.RibbonBattlerSkillful) // having memory and not ribbon is too rare, just flag here.
                yield return new RibbonResult(nameof(s6.RibbonBattlerSkillful));
            if (s6.RibbonBattlerExpert)
                yield return new RibbonResult(nameof(s6.RibbonBattlerExpert));
            yield break;
        }
        if (!hasChampMemory || s6.RibbonBattlerSkillful || s6.RibbonBattlerExpert)
            yield break;

        var result = new RibbonResult(nameof(s6.RibbonBattlerSkillful), false);
        result.Combine(new RibbonResult(nameof(s6.RibbonBattlerExpert)));
        yield return result;
    }

    private static IEnumerable<RibbonResult> GetInvalidRibbons6AnyG8(PKM pk, IRibbonSetCommon6 s6, EvolutionHistory evos)
    {
        if (!pk.HasVisitedBDSP(evos.Gen8b))
        {
            var none = GetInvalidRibbonsNone(s6.RibbonBits(), s6.RibbonNamesBool());
            foreach (var x in none)
                yield return x;
            yield break;
        }

        if (s6.RibbonChampionKalos)
            yield return new RibbonResult(nameof(s6.RibbonChampionKalos));
        if (s6.RibbonChampionG6Hoenn)
            yield return new RibbonResult(nameof(s6.RibbonChampionG6Hoenn));
        //if (s6.RibbonBestFriends)
        //    yield return new RibbonResult(nameof(s6.RibbonBestFriends));
        if (s6.RibbonTraining)
            yield return new RibbonResult(nameof(s6.RibbonTraining));
        if (s6.RibbonBattlerSkillful)
            yield return new RibbonResult(nameof(s6.RibbonBattlerSkillful));
        if (s6.RibbonBattlerExpert)
            yield return new RibbonResult(nameof(s6.RibbonBattlerExpert));

        if (s6.RibbonCountMemoryContest != 0)
            yield return new RibbonResult(nameof(s6.RibbonCountMemoryContest));
        if (s6.RibbonCountMemoryBattle != 0)
            yield return new RibbonResult(nameof(s6.RibbonCountMemoryBattle));

        // Can get contest ribbons via BD/SP contests.
        //if (s6.RibbonContestStar)
        //    yield return new RibbonResult(nameof(s6.RibbonContestStar));
        //if (s6.RibbonMasterCoolness)
        //    yield return new RibbonResult(nameof(s6.RibbonMasterCoolness));
        //if (s6.RibbonMasterBeauty)
        //    yield return new RibbonResult(nameof(s6.RibbonMasterBeauty));
        //if (s6.RibbonMasterCuteness)
        //    yield return new RibbonResult(nameof(s6.RibbonMasterCuteness));
        //if (s6.RibbonMasterCleverness)
        //    yield return new RibbonResult(nameof(s6.RibbonMasterCleverness));
        //if (s6.RibbonMasterToughness)
        //    yield return new RibbonResult(nameof(s6.RibbonMasterToughness));

        var contest = s6.RibbonBitsContest();
        bool allContest = contest.All(z => z);
        if (allContest != s6.RibbonContestStar) // if not already checked
            yield return new RibbonResult(nameof(s6.RibbonContestStar), s6.RibbonContestStar);
    }

    private static IEnumerable<RibbonResult> GetInvalidRibbons6Memory(PKM pk, IRibbonSetCommon6 s6, int gen, IEncounterTemplate enc)
    {
        int contest = 0;
        int battle = 0;
        switch (gen)
        {
            case 3:
                contest = IsAllowedInContest4(pk.Species, pk.Form) ? 40 : 20;
                battle = IsAllowedBattleFrontier(pk.Species) ? CanHaveRibbonWinning(pk, enc, 3) ? 8 : 7 : 0;
                break;
            case 4:
                contest = IsAllowedInContest4(pk.Species, pk.Form) ? 20 : 0;
                battle = IsAllowedBattleFrontier(pk.Species) ? 6 : 0;
                break;
        }
        if (s6.RibbonCountMemoryContest > contest)
            yield return new RibbonResult(nameof(s6.RibbonCountMemoryContest));
        if (s6.RibbonCountMemoryBattle > battle)
            yield return new RibbonResult(nameof(s6.RibbonCountMemoryBattle));
    }

    private static IEnumerable<RibbonResult> GetInvalidRibbons6Untraded(PKM pk, IRibbonSetCommon6 s6)
    {
        if (pk.XY)
        {
            if (s6.RibbonChampionG6Hoenn)
                yield return new RibbonResult(nameof(s6.RibbonChampionG6Hoenn));

            if (s6.RibbonContestStar)
                yield return new RibbonResult(nameof(s6.RibbonContestStar));
            if (s6.RibbonMasterCoolness)
                yield return new RibbonResult(nameof(s6.RibbonMasterCoolness));
            if (s6.RibbonMasterBeauty)
                yield return new RibbonResult(nameof(s6.RibbonMasterBeauty));
            if (s6.RibbonMasterCuteness)
                yield return new RibbonResult(nameof(s6.RibbonMasterCuteness));
            if (s6.RibbonMasterCleverness)
                yield return new RibbonResult(nameof(s6.RibbonMasterCleverness));
            if (s6.RibbonMasterToughness)
                yield return new RibbonResult(nameof(s6.RibbonMasterToughness));
        }
        else if (pk.AO)
        {
            if (s6.RibbonChampionKalos)
                yield return new RibbonResult(nameof(s6.RibbonChampionKalos));
        }
    }

    private static IEnumerable<RibbonResult> GetInvalidRibbons6Traded(PKM pk, IRibbonSetCommon6 s6)
    {
        // Medal count is wiped on transfer to pk8
        if (s6.RibbonTraining && pk.Format <= 7)
        {
            const int req = 12; // only first 12
            int count = ((ISuperTrain)pk).SuperTrainingMedalCount(req);
            if (count < req)
                yield return new RibbonResult(nameof(s6.RibbonTraining));
        }

        const int memChampion = 27;
        bool hasChampMemory = pk is ITrainerMemories m && ((pk.Format < 8 && m.HT_Memory == memChampion) || (pk.Gen6 && m.OT_Memory == memChampion));
        if (!hasChampMemory || s6.RibbonChampionKalos || s6.RibbonChampionG6Hoenn)
            yield break;

        var result = new RibbonResult(nameof(s6.RibbonChampionKalos), false);
        result.Combine(new RibbonResult(nameof(s6.RibbonChampionG6Hoenn)));
        yield return result;
    }

    private static IEnumerable<RibbonResult> GetInvalidRibbons7Any(PKM pk, IRibbonSetCommon7 s7)
    {
        if (!IsAllowedBattleFrontier(pk.Species))
        {
            if (s7.RibbonBattleRoyale)
                yield return new RibbonResult(nameof(s7.RibbonBattleRoyale));
            if (s7.RibbonBattleTreeGreat && !pk.USUM && pk.IsUntraded)
                yield return new RibbonResult(nameof(s7.RibbonBattleTreeGreat));
            if (s7.RibbonBattleTreeMaster)
                yield return new RibbonResult(nameof(s7.RibbonBattleTreeMaster));
        }
    }

    private static IEnumerable<RibbonResult> GetInvalidRibbons8Any(PKM pk, IRibbonSetCommon8 s8, IEncounterTemplate enc, EvolutionHistory evos)
    {
        bool swsh = pk.HasVisitedSWSH(evos.Gen8);
        bool bdsp = pk.HasVisitedBDSP(evos.Gen8b);
        bool pla = pk.HasVisitedLA(evos.Gen8a);

        if (!swsh && !bdsp)
        {
            if (s8.RibbonTowerMaster)
                yield return new RibbonResult(nameof(s8.RibbonTowerMaster));
        }
        if (!swsh)
        {
            if (s8.RibbonChampionGalar)
                yield return new RibbonResult(nameof(s8.RibbonChampionGalar));
            if (s8.RibbonMasterRank)
                yield return new RibbonResult(nameof(s8.RibbonMasterRank));
        }
        else
        {
            const int memChampion = 27;
            {
                bool hasChampMemory = (pk.Format == 8 && pk is IMemoryHT {HT_Memory: memChampion}) ||
                                      (enc.Generation == 8 && pk is IMemoryOT {OT_Memory: memChampion});
                if (hasChampMemory && !s8.RibbonChampionGalar)
                    yield return new RibbonResult(nameof(s8.RibbonChampionGalar));
            }

            // Legends cannot compete in Ranked, thus cannot reach Master Rank and obtain the ribbon.
            // Past gen Pokemon can get the ribbon only if they've been reset.
            if (s8.RibbonMasterRank && !CanParticipateInRankedSWSH(pk, enc, evos))
                yield return new RibbonResult(nameof(s8.RibbonMasterRank));

            if (!s8.RibbonTowerMaster)
            {
                // If the Tower Master ribbon is not present but a memory hint implies it should...
                // This memory can also be applied in Gen6/7 via defeating the Chatelaines, where legends are disallowed.
                const int strongest = 30;
                if (pk is IMemoryOT {OT_Memory: strongest} or IMemoryHT {HT_Memory: strongest})
                {
                    if (enc.Generation == 8 || !IsAllowedBattleFrontier(pk.Species) || pk is IRibbonSetCommon6 {RibbonBattlerSkillful: false})
                        yield return new RibbonResult(nameof(s8.RibbonTowerMaster));
                }
            }
        }

        if (s8.RibbonTwinklingStar && (!bdsp || pk is IRibbonSetCommon6 {RibbonContestStar:false}))
        {
            yield return new RibbonResult(nameof(s8.RibbonTwinklingStar));
        }

        // received when capturing photos with Pokémon in the Photography Studio
        if (s8.RibbonPioneer && !pla)
        {
            yield return new RibbonResult(nameof(s8.RibbonPioneer));
        }
    }

    private static bool CanParticipateInRankedSWSH(PKM pk, IEncounterTemplate enc, EvolutionHistory evos)
    {
        bool exist = enc.Generation switch
        {
            < 8 => pk is IBattleVersion { BattleVersion: (int)GameVersion.SW or (int)GameVersion.SH },
            _ => pk.HasVisitedSWSH(evos.Gen8),
        };
        if (!exist)
            return false;

        // Clamp to permitted species
        var species = pk.Species;
        if (species > Legal.MaxSpeciesID_8_R2)
            return false;
        
        // Series 13 rule-set was the first time Ranked Battles allowed the use of Mythical Pokémon.
        if (Legal.Legends.Contains(species))
        {
            if (enc.Version == GameVersion.GO || enc is IEncounterServerDate { IsDateRestricted: true }) // Capture date is global time, and not console changeable.
            {
                if (pk.MetDate > new DateTime(2022, 11, 1)) // Series 13 end date
                    return false;
            }
        }

        return PersonalTable.SWSH.IsPresentInGame(species, pk.Form);
    }

    private static IEnumerable<RibbonResult> GetInvalidRibbonsEvent1(PKM pk, IEncounterTemplate enc)
    {
        if (pk is not IRibbonSetEvent3 set1)
            yield break;
        var names = set1.RibbonNames();
        var sb = set1.RibbonBits();
        var eb = enc is IRibbonSetEvent3 e3 ? e3.RibbonBits() : new bool[sb.Length];

        if (enc.Generation == 3)
        {
            eb[0] = sb[0]; // permit Earth Ribbon
            if (pk.Version == 15 && enc is EncounterStaticShadow s)
            {
                // only require national ribbon if no longer on origin game
                bool untraded = s.Version == GameVersion.XD
                    ? pk is XK3 {RibbonNational: false}
                    : pk is CK3 {RibbonNational: false};
                eb[1] = !untraded;
            }
        }

        for (int i = 0; i < sb.Length; i++)
        {
            if (sb[i] != eb[i])
                yield return new RibbonResult(names[i], !eb[i]); // only flag if invalid
        }
    }

    private static IEnumerable<RibbonResult> GetInvalidRibbonsEvent2(PKM pk, IEncounterTemplate enc)
    {
        if (pk is not IRibbonSetEvent4 set2)
            yield break;
        var names = set2.RibbonNames();
        var sb = set2.RibbonBits();
        var eb = enc is IRibbonSetEvent4 e4 ? e4.RibbonBits() : new bool[sb.Length];

        if (enc is EncounterStatic7 {Species: (int)Species.Magearna})
            eb[1] = true; // require Wishing Ribbon

        for (int i = 0; i < sb.Length; i++)
        {
            if (sb[i] != eb[i])
                yield return new RibbonResult(names[i], !eb[i]); // only flag if invalid
        }
    }

    private static IEnumerable<RibbonResult> GetInvalidRibbonsNone(IReadOnlyList<bool> bits, IReadOnlyList<string> names)
    {
        for (int i = 0; i < bits.Count; i++)
        {
            if (bits[i])
                yield return new RibbonResult(names[i]);
        }
    }

    private static IEnumerable<RibbonResult> GetInvalidRibbonsNoneSkipIndex(IReadOnlyList<bool> bits, IReadOnlyList<string> names, int skipIndex)
    {
        for (int i = 0; i < bits.Count; i++)
        {
            if (bits[i] && i != skipIndex)
                yield return new RibbonResult(names[i]);
        }
    }

    private static bool IsAllowedInContest4(int species, int form) => species switch
    {
        // Disallow Unown and Ditto, and Spiky Pichu (cannot trade)
        (int)Species.Ditto => false,
        (int)Species.Unown => false,
        (int)Species.Pichu when form == 1 => false,
        _ => true,
    };

    private static bool IsAllowedBattleFrontier(int species) => !Legal.BattleFrontierBanlist.Contains(species);

    private static bool IsAllowedBattleFrontier(int species, int form, int gen)
    {
        if (gen == 4 && species == (int)Species.Pichu && form == 1) // spiky
            return false;
        return IsAllowedBattleFrontier(species);
    }

    private static bool CanHaveFootprintRibbon(PKM pk, EvolutionHistory evos, int gen)
    {
        if (gen <= 4) // Friendship Check unnecessary - can decrease after obtaining ribbon.
            return true;
        // Gen5: Can't obtain
        if (pk.Format < 6)
            return false;

        // Gen6/7: Increase level by 30 from original level
        if (gen != 8 && !pk.GG && (pk.CurrentLevel - pk.Met_Level >= 30))
            return true;

        // Gen8-BDSP: Variable by species Footprint
        var bdspEvos = evos.Gen8b;
        if (pk.HasVisitedBDSP(bdspEvos))
        {
            if (bdspEvos.Any(z => PersonalTable.BDSP.IsPresentInGame(z.Species, z.Form) && !HasFootprintBDSP[z.Species]))
                return true; // no footprint
            if (pk.CurrentLevel - pk.Met_Level >= 30)
                return true; // traveled well
        }

        // Otherwise: Can't obtain
        return false;
    }

    private static bool CanHaveRibbonWinning(PKM pk, IEncounterTemplate enc, int gen)
    {
        if (gen != 3)
            return false;
        if (!IsAllowedBattleFrontier(pk.Species))
            return false;
        if (pk.Format == 3)
            return pk.Met_Level <= 50;

        // Most encounter types can be below level 50; only Shadow Dragonite & Tyranitar, and select Gen3 Event Gifts.
        // These edge cases can't be obtained below level 50, unlike some wild Pokémon which can be encountered at different locations for lower levels.
        if (enc.LevelMin <= 50)
            return true;

        return enc is not (EncounterStaticShadow or WC3);
    }

    private static bool CanHaveRibbonVictory(PKM pk, int gen)
    {
        return gen == 3 && IsAllowedBattleFrontier(pk.Species);
    }

    // Footprint type is not Type 5, requiring 30 levels.
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
}
