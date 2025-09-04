using System;
using static PKHeX.Core.LegalityCheckResultCode;

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

    public static byte GetSuggestedFriendshipCurrent(PKM pk, IEncounterTemplate enc)
    {
        if (pk.CurrentHandler == 0)
            return GetSuggestedFriendshipOT(pk, enc);
        return GetSuggestedFriendshipHT(pk);
    }

    public static byte GetSuggestedFriendshipOT(PKM pk, IEncounterTemplate enc)
    {
        if (pk.IsEgg)
            return enc is IHatchCycle h ? h.EggCycles : pk.PersonalInfo.HatchCycles;

        if (pk.Format <= 2)
            return GetSuggestedFriendshipByMove(pk);
        // VC transfers use S/M personal info
        if (enc is EncounterTransfer7 t7)
            return PersonalTable.SM[t7.Species].BaseFriendship;

        // 3+
        bool neverOT = !GetCanOTHandle(enc, pk, enc.Generation);
        if (neverOT)
            return GetBaseFriendship(enc);
        return GetSuggestedFriendshipByMove(pk);
    }

    public static byte GetSuggestedFriendshipHT(PKM pk)
    {
        if (pk.IsUntraded)
            return 0;
        return GetSuggestedFriendshipByMove(pk);
    }

    private static byte GetSuggestedFriendshipByMove(PKM pk)
    {
        if (pk.HasMove((ushort)Move.Return))
            return byte.MaxValue;
        if (pk.HasMove((ushort)Move.Frustration))
            return 0;
        return pk.PersonalInfo.BaseFriendship;
    }

    private void VerifyTradeState(LegalityAnalysis data)
    {
        var pk = data.Entity;

        if (data.Entity is IGeoTrack t)
            VerifyGeoLocationData(data, t, data.Entity);

        if (pk.VC && pk is PK7 {Geo1_Country: 0}) // VC transfers set Geo1 Country
            data.AddLine(GetInvalid(GeoMemoryMissing));

        if (!pk.IsUntraded)
        {
            // Can't have HT details even as a Link Trade egg, except in some games.
            if (pk.IsEgg && !EggStateLegality.IsValidHTEgg(pk))
                data.AddLine(GetInvalid(MemoryArgBadHT));
            return;
        }

        if (pk.CurrentHandler != 0) // Badly edited; PKHeX doesn't trip this.
            data.AddLine(GetInvalid(MemoryHTFlagInvalid));
        else if (pk.HandlingTrainerFriendship != 0)
            data.AddLine(GetInvalid(MemoryStatFriendshipHT0));
        else if (pk is IAffection {HandlingTrainerAffection: not 0})
            data.AddLine(GetInvalid(MemoryStatAffectionHT0));

        // Don't check trade evolutions if Untraded. The Evolution Chain already checks for trade evolutions.
    }

    /// <summary>
    /// Checks if the <see cref="PKM.CurrentHandler"/> state is set correctly.
    /// </summary>
    private void VerifyHandlerState(LegalityAnalysis data, bool neverOT)
    {
        var pk = data.Entity;
        var info = data.Info;
        var enc = info.EncounterOriginal;
        var current = pk.CurrentHandler;

        if (ParseSettings.Settings.Handler.CheckActiveHandler && ParseSettings.ActiveTrainer is { } tr)
        {
            var shouldBe0 = tr.IsFromTrainer(pk);
            byte expect = shouldBe0 ? (byte)0 : (byte)1;
            if (!IsHandlerStateCorrect(enc, pk, current, expect))
            {
                // generally disable this check if it's being edited inside a blank save file's environment.
                if (tr is not SaveFile { State.Exportable: false })
                    data.AddLine(GetInvalid(TransferCurrentHandlerInvalid));
                // if there's no HT data yet specified, don't bother checking further.
                // blank save exports will be injected and fixed later, and not-blanks will have been flagged by the above.
                if (pk.IsUntraded)
                    return;
            }

            if (current == 1)
                CheckHandlingTrainerEquals(data, pk, tr);
        }

        if (current != 1 && (enc.Context != pk.Context || neverOT))
            data.AddLine(GetInvalid(TransferHandlerFlagRequired));
        if (!pk.IsUntraded && IsUntradeableEncounter(enc)) // Starter, untradeable
            data.AddLine(GetInvalid(TransferCurrentHandlerInvalid));
    }

    public static bool IsHandlerStateCorrect(IEncounterTemplate enc, PKM pk, byte current, byte expect)
    {
        if (current == expect)
            return true;

        if (current == 0)
            return IsHandlerOriginalBug(enc, pk);
        return false; // HT [1] should be OT [0].
    }

    /// <summary> <see cref="Bulk.HandlerChecker.CheckHandlingTrainerEquals"/> </summary>
    private void CheckHandlingTrainerEquals(LegalityAnalysis data, PKM pk, ITrainerInfo tr)
    {
        Span<char> ht = stackalloc char[pk.TrashCharCountTrainer];
        var len = pk.LoadString(pk.HandlingTrainerTrash, ht);
        ht = ht[..len];

        if (!ht.SequenceEqual(tr.OT))
            data.AddLine(GetInvalid(TransferHandlerMismatchName));
        if (pk.HandlingTrainerGender != tr.Gender)
            data.AddLine(GetInvalid(TransferHandlerMismatchGender));

        // If the format exposes a language, check if it matches.
        // Can be mismatched as the game only checks OT/Gender equivalence -- if it matches, don't update everything else.
        // Statistically unlikely that players will play in different languages, but it's technically possible.
        if (pk is IHandlerLanguage h && h.HandlingTrainerLanguage != tr.Language)
            data.AddLine(Get(Severity.Fishy, TransferHandlerMismatchLanguage));
    }

    private static bool IsUntradeableEncounter(IEncounterTemplate enc) => enc switch
    {
        EncounterStatic7b { Location: 28 } => true, // LGP/E Starter
        EncounterStatic9  { StarterBoxLegend: true } => true, // S/V Ride legend
        _ => false,
    };

    /// <summary>
    /// Checks the non-Memory data for the <see cref="PKM.OriginalTrainerName"/> details.
    /// </summary>
    private void VerifyOTMisc(LegalityAnalysis data, bool neverOT)
    {
        var pk = data.Entity;
        var Info = data.Info;

        VerifyOTAffection(data, neverOT, Info.Generation, pk);
        VerifyOTFriendship(data, neverOT, Info.Generation, pk);
    }

    private void VerifyOTFriendship(LegalityAnalysis data, bool neverOT, byte generation, PKM pk)
    {
        if (generation == 0) // other things are invalid, don't bother checking
            return;

        if (generation <= 2)
        {
            VerifyOTFriendshipVC12(data, pk);
            return;
        }

        if (neverOT)
        {
            // Verify the original friendship value since it cannot change from the value it was assigned in the original generation.
            // If none match, then it is not a valid OT friendship.
            var fs = pk.OriginalTrainerFriendship;
            var enc = data.Info.EncounterMatch;
            var expect = GetBaseFriendship(enc);
            if (fs != expect)
                data.AddLine(GetInvalid(MemoryStatFriendshipOTBaseEvent_0, expect));
        }
    }

    private void VerifyOTFriendshipVC12(LegalityAnalysis data, PKM pk)
    {
        // Verify the original friendship value since it cannot change from the value it was assigned in the original generation.
        // Since some evolutions have different base friendship values, check all possible evolutions for a match.
        // If none match, then it is not a valid OT friendship.
        // VC transfers use S/M personal info
        var any = IsMatchFriendship(data.Info.EvoChainsAllGens.Gen7, pk.OriginalTrainerFriendship, out var hint);
        if (!any)
            data.AddLine(GetInvalid(MemoryStatFriendshipOTBaseEvent_0, hint));
    }

    private static bool IsMatchFriendship(ReadOnlySpan<EvoCriteria> evos, byte current, out byte expect)
    {
        expect = 0; // will be overridden on the first loop
        var pt = PersonalTable.USUM;
        foreach (var z in evos)
        {
            if (!pt.IsPresentInGame(z.Species, z.Form))
                continue;
            var entry = pt.GetFormEntry(z.Species, z.Form);
            expect = entry.BaseFriendship;
            if (expect == current)
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
            if ((origin <= 2 && a.OriginalTrainerAffection != 0) || IsInvalidContestAffection(a))
                data.AddLine(GetInvalid(MemoryStatAffectionOT0));
        }
        else if (neverOT)
        {
            if (origin == 6)
            {
                if (pk is { IsUntraded: true, XY: true })
                {
                    if (a.OriginalTrainerAffection != 0)
                        data.AddLine(GetInvalid(MemoryStatAffectionOT0));
                }
                else if (IsInvalidContestAffection(a))
                {
                    data.AddLine(GetInvalid(MemoryStatAffectionOT0));
                }
            }
            else
            {
                if (a.OriginalTrainerAffection != 0)
                    data.AddLine(GetInvalid(MemoryStatAffectionOT0));
            }
        }
    }

    /// <summary>
    /// Checks the non-Memory data for the <see cref="PKM.HandlingTrainerName"/> details.
    /// </summary>
    private void VerifyHTMisc(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var htGender = pk.HandlingTrainerGender;
        if (htGender > 1 || (pk.IsUntraded && htGender != 0))
            data.AddLine(GetInvalid(MemoryHTGender_0, htGender));
    }

    private void VerifyGeoLocationData(LegalityAnalysis data, IGeoTrack t, PKM pk)
    {
        var valid = t.GetValidity();
        if (valid == GeoValid.CountryAfterPreviousEmpty)
            data.AddLine(GetInvalid(GeoBadOrder));
        else if (valid == GeoValid.RegionWithoutCountry)
            data.AddLine(GetInvalid(GeoNoRegion));
        if (t.Geo1_Country != 0 && pk.IsUntraded) // traded
            data.AddLine(GetInvalid(GeoNoCountryHT));
    }

    // OR/AS contests mistakenly apply 20 affection to the OT instead of the current handler's value
    private static bool IsInvalidContestAffection(IAffection pk) => pk.OriginalTrainerAffection != 255 && pk.OriginalTrainerAffection % 20 != 0;

    public static bool GetCanOTHandle(IEncounterTemplate enc, PKM pk, byte generation)
    {
        // Handlers introduced in Generation 6. OT Handling was always the case for Generation 3-5 data.
        if (generation < 6)
            return generation >= 3;

        if (GetCanOTHandle(enc, pk))
            return true;

        if (ParseSettings.Settings.Handler.Restrictions.GetCanOTHandle(enc.Context))
            return true;

        return false;
    }

    private static bool GetCanOTHandle(IEncounterTemplate enc, PKM pk) => enc switch
    {
        IFixedTrainer { IsFixedTrainer: true } => false,
        EncounterSlot8GO => false,
        WC6 { IsOriginalTrainerNameSet: true } => false,
        WC7 { IsOriginalTrainerNameSet: true, IsAshPikachu: false } => false, // Ash Pikachu QR Gift doesn't set Current Handler
        WB7 wb7 when wb7.GetHasOT(pk.Language) => false,
        WC8 wc8 when wc8.GetHasOT(pk.Language) => false,
        WB8 wb8 when wb8.GetHasOT(pk.Language) => false,
        WA8 wa8 when wa8.GetHasOT(pk.Language) => false,
        WC9 wc9 when wc9.GetHasOT(pk.Language) => false,
        WC8 {IsHOMEGift: true} => false,
        WC9 {IsHOMEGift: true} => false,
        _ => true,
    };

    private static bool IsHandlerOriginalBug(IEncounterTemplate enc, PKM pk) => enc switch
    {
        WC7 { IsAshPikachu: true } => pk.Context == EntityContext.Gen7, // Ash Pikachu QR Gift doesn't set Current Handler
        _ => false,
    };

    private static byte GetBaseFriendship(IEncounterTemplate enc) => enc switch
    {
        IFixedOTFriendship f => f.OriginalTrainerFriendship,
        _ => GetBaseFriendship(enc.Context, enc.Species, enc.Form),
    };

    private static byte GetBaseFriendship(EntityContext context, ushort species, byte form) => context switch
    {
        EntityContext.Gen6  => PersonalTable.AO[species].BaseFriendship,
        EntityContext.Gen7  => PersonalTable.USUM[species].BaseFriendship,
        EntityContext.Gen7b => PersonalTable.GG[species].BaseFriendship,
        EntityContext.Gen8  => PersonalTable.SWSH.GetFormEntry(species, form).BaseFriendship,
        EntityContext.Gen8a => PersonalTable.LA.GetFormEntry(species, form).BaseFriendship,
        EntityContext.Gen8b => PersonalTable.BDSP.GetFormEntry(species, form).BaseFriendship,
        EntityContext.Gen9  => PersonalTable.SV.GetFormEntry(species, form).BaseFriendship,
        _ => throw new ArgumentOutOfRangeException(nameof(context)),
    };
}
