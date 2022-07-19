using System;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Verifies the Friendship, Affection, and other miscellaneous stats that can be present for OT/HT data.
/// </summary>
public sealed class HistoryVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Memory;

    public override void Verify(LegalityAnalysis data)
    {
        bool neverOT = !GetCanOTHandle(data.Info.EncounterMatch, data.Entity, data.Info.Generation);
        VerifyHandlerState(data, neverOT);
        VerifyTradeState(data);
        VerifyOTMisc(data, neverOT);
        VerifyHTMisc(data);
    }

    private void VerifyTradeState(LegalityAnalysis data)
    {
        var pk = data.Entity;

        if (data.Entity is IGeoTrack t)
            VerifyGeoLocationData(data, t, data.Entity);

        if (pk.VC && pk is PK7 {Geo1_Country: 0}) // VC transfers set Geo1 Country
            data.AddLine(GetInvalid(LGeoMemoryMissing));

        if (!pk.IsUntraded)
        {
            // Can't have HT details even as a Link Trade egg, except in some games.
            if (pk.IsEgg && !EggStateLegality.IsValidHTEgg(pk))
                data.AddLine(GetInvalid(LMemoryArgBadHT));
            return;
        }

        if (pk.CurrentHandler != 0) // Badly edited; PKHeX doesn't trip this.
            data.AddLine(GetInvalid(LMemoryHTFlagInvalid));
        else if (pk.HT_Friendship != 0)
            data.AddLine(GetInvalid(LMemoryStatFriendshipHT0));
        else if (pk is IAffection {HT_Affection: not 0})
            data.AddLine(GetInvalid(LMemoryStatAffectionHT0));

        // Don't check trade evolutions if Untraded. The Evolution Chain already checks for trade evolutions.
    }

    /// <summary>
    /// Checks if the <see cref="PKM.CurrentHandler"/> state is set correctly.
    /// </summary>
    private void VerifyHandlerState(LegalityAnalysis data, bool neverOT)
    {
        var pk = data.Entity;
        var Info = data.Info;

        // HT Flag
        if (ParseSettings.CheckActiveHandler)
        {
            var tr = ParseSettings.ActiveTrainer;
            var withOT = tr.IsFromTrainer(pk);
            var flag = pk.CurrentHandler;
            var expect = withOT ? 0 : 1;
            if (flag != expect)
            {
                data.AddLine(GetInvalid(LTransferCurrentHandlerInvalid));
                return;
            }

            if (flag == 1)
            {
                if (pk.HT_Name != tr.OT)
                    data.AddLine(GetInvalid(LTransferHTMismatchName));
                if (pk is IHandlerLanguage h && h.HT_Language != tr.Language)
                    data.AddLine(GetInvalid(LTransferHTMismatchLanguage));
            }
        }

        if (!pk.IsUntraded && IsUntradeableEncounter(Info.EncounterMatch)) // Starter, untradeable
            data.AddLine(GetInvalid(LTransferCurrentHandlerInvalid));
        if ((Info.Generation != pk.Format || neverOT) && pk.CurrentHandler != 1)
            data.AddLine(GetInvalid(LTransferHTFlagRequired));
    }

    private static bool IsUntradeableEncounter(IEncounterTemplate enc) => enc switch
    {
        EncounterStatic7b { Location: 28 } => true, // LGP/E Starter
        _ => false,
    };

    /// <summary>
    /// Checks the non-Memory data for the <see cref="PKM.OT_Name"/> details.
    /// </summary>
    private void VerifyOTMisc(LegalityAnalysis data, bool neverOT)
    {
        var pk = data.Entity;
        var Info = data.Info;

        VerifyOTAffection(data, neverOT, Info.Generation, pk);
        VerifyOTFriendship(data, neverOT, Info.Generation, pk);
    }

    private void VerifyOTFriendship(LegalityAnalysis data, bool neverOT, int origin, PKM pk)
    {
        if (origin < 0)
            return;

        if (origin <= 2)
        {
            VerifyOTFriendshipVC12(data, pk);
            return;
        }

        if (neverOT)
        {
            // Verify the original friendship value since it cannot change from the value it was assigned in the original generation.
            // If none match, then it is not a valid OT friendship.
            var fs = pk.OT_Friendship;
            var enc = data.Info.EncounterMatch;
            if (GetBaseFriendship(enc, origin) != fs)
                data.AddLine(GetInvalid(LMemoryStatFriendshipOTBaseEvent));
        }
    }

    private void VerifyOTFriendshipVC12(LegalityAnalysis data, PKM pk)
    {
        // Verify the original friendship value since it cannot change from the value it was assigned in the original generation.
        // Since some evolutions have different base friendship values, check all possible evolutions for a match.
        // If none match, then it is not a valid OT friendship.
        // VC transfers use SM personal info
        var any = IsMatchFriendship(data.Info.EvoChainsAllGens.Gen7, pk.OT_Friendship);
        if (!any)
            data.AddLine(GetInvalid(LMemoryStatFriendshipOTBaseEvent));
    }

    private static bool IsMatchFriendship(EvoCriteria[] evos, int fs)
    {
        var pt = PersonalTable.USUM;
        foreach (var z in evos)
        {
            if (!pt.IsPresentInGame(z.Species, z.Form))
                continue;
            var entry = pt.GetFormEntry(z.Species, z.Form);
            if (entry.BaseFriendship == fs)
                return true;
        }
        return false;
    }

    private void VerifyOTAffection(LegalityAnalysis data, bool neverOT, int origin, PKM pk)
    {
        if (pk is not IAffection a)
            return;

        if (origin < 6)
        {
            // Can gain affection in Gen6 via the Contest glitch applying affection to OT rather than HT.
            // VC encounters cannot obtain OT affection since they can't visit Gen6.
            if ((origin <= 2 && a.OT_Affection != 0) || IsInvalidContestAffection(a))
                data.AddLine(GetInvalid(LMemoryStatAffectionOT0));
        }
        else if (neverOT)
        {
            if (origin == 6)
            {
                if (pk.IsUntraded && pk.XY)
                {
                    if (a.OT_Affection != 0)
                        data.AddLine(GetInvalid(LMemoryStatAffectionOT0));
                }
                else if (IsInvalidContestAffection(a))
                {
                    data.AddLine(GetInvalid(LMemoryStatAffectionOT0));
                }
            }
            else
            {
                if (a.OT_Affection != 0)
                    data.AddLine(GetInvalid(LMemoryStatAffectionOT0));
            }
        }
    }

    /// <summary>
    /// Checks the non-Memory data for the <see cref="PKM.HT_Name"/> details.
    /// </summary>
    private void VerifyHTMisc(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var htGender = pk.HT_Gender;
        if (htGender > 1 || (pk.IsUntraded && htGender != 0))
            data.AddLine(GetInvalid(string.Format(LMemoryHTGender, htGender)));

        if (pk is IHandlerLanguage h)
            VerifyHTLanguage(data, h, pk);
    }

    private void VerifyHTLanguage(LegalityAnalysis data, IHandlerLanguage h, PKM pk)
    {
        if (h.HT_Language == 0)
        {
            if (!string.IsNullOrWhiteSpace(pk.HT_Name))
                data.AddLine(GetInvalid(LMemoryHTLanguage));
            return;
        }

        if (string.IsNullOrWhiteSpace(pk.HT_Name))
            data.AddLine(GetInvalid(LMemoryHTLanguage));
        else if (h.HT_Language > (int)LanguageID.ChineseT)
            data.AddLine(GetInvalid(LMemoryHTLanguage));
    }

    private void VerifyGeoLocationData(LegalityAnalysis data, IGeoTrack t, PKM pk)
    {
        var valid = t.GetValidity();
        if (valid == GeoValid.CountryAfterPreviousEmpty)
            data.AddLine(GetInvalid(LGeoBadOrder));
        else if (valid == GeoValid.RegionWithoutCountry)
            data.AddLine(GetInvalid(LGeoNoRegion));
        if (t.Geo1_Country != 0 && pk.IsUntraded) // traded
            data.AddLine(GetInvalid(LGeoNoCountryHT));
    }

    // ORAS contests mistakenly apply 20 affection to the OT instead of the current handler's value
    private static bool IsInvalidContestAffection(IAffection pk) => pk.OT_Affection != 255 && pk.OT_Affection % 20 != 0;

    public static bool GetCanOTHandle(IEncounterTemplate enc, PKM pk, int generation)
    {
        // Handlers introduced in Generation 6. OT Handling was always the case for Generation 3-5 data.
        if (generation < 6)
            return generation >= 3;

        return enc switch
        {
            EncounterTrade => false,
            EncounterSlot8GO => false,
            WC6 wc6 when wc6.OT_Name.Length > 0 => false,
            WC7 wc7 when wc7.OT_Name.Length > 0 && wc7.TID != 18075 => false, // Ash Pikachu QR Gift doesn't set Current Handler
            WC8 wc8 when wc8.GetHasOT(pk.Language) => false,
            WB8 wb8 when wb8.GetHasOT(pk.Language) => false,
            WA8 wa8 when wa8.GetHasOT(pk.Language) => false,
            WC8 {IsHOMEGift: true} => false,
            _ => true,
        };
    }

    private static int GetBaseFriendship(IEncounterTemplate enc, int generation) => enc switch
    {
        IFixedOTFriendship f => f.OT_Friendship,

        { Version: GameVersion.BDSP or GameVersion.BD or GameVersion.SP }
            => PersonalTable.BDSP.GetFormEntry(enc.Species, enc.Form).BaseFriendship,
        { Version: GameVersion.PLA }
            => PersonalTable.LA  .GetFormEntry(enc.Species, enc.Form).BaseFriendship,

        _ => GetBaseFriendship(generation, enc.Species, enc.Form),
    };

    private static int GetBaseFriendship(int generation, int species, int form) => generation switch
    {
        6 => PersonalTable.AO[species].BaseFriendship,
        7 => PersonalTable.USUM[species].BaseFriendship,
        8 => PersonalTable.SWSH.GetFormEntry(species, form).BaseFriendship,
        _ => throw new ArgumentOutOfRangeException(nameof(generation)),
    };
}
