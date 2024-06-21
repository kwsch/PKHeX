using System;
using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.StringSource;

namespace PKHeX.Core;

/// <summary>
/// Verifies the trash bytes of various strings.
/// </summary>
public sealed class TrashByteVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.TrashBytes;

    private static string Format(StringSource s) => s switch
    {
        Nickname => L_XNickname,
        OriginalTrainer => L_XOT,
        HandlingTrainer => L_XHT,
        _ => throw new ArgumentOutOfRangeException(nameof(s)),
    };

    private static string Format(StringSource s, string msg) => string.Format(L_F0_1, Format(s), msg);

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.Format >= 8 || pk.Context == EntityContext.Gen7b)
        {
            VerifyTrashBytesHOME(data, pk);
        }
        else if (pk.Format == 4)
        {
            var enc = data.EncounterMatch;
            if (enc is PCD pcd)
                VerifyTrashBytesPCD(data, pk, pcd);
            else if (enc.Generation == 3)
                VerifyTrashBytesPalPark(data, pk);
        }
    }

    private void VerifyTrashBytesPalPark(LegalityAnalysis data, PKM pk)
    {
        if (pk.Japanese)
        {
            // Trash bytes should be zero.
            if (!TrashBytesUTF16.IsTrashEmpty(pk.OriginalTrainerTrash))
                data.AddLine(GetInvalid(Format(Nickname, LTrashBytesShouldBeEmpty)));
        }
        else
        {
            // Should have trash bytes from the transfer process.
            if (TrashBytesUTF16.IsTrashEmpty(pk.OriginalTrainerTrash))
                data.AddLine(GetInvalid(Format(Nickname, LTrashBytesExpected)));
        }
    }

    private void VerifyTrashBytesPCD(LegalityAnalysis data, PKM pk, PCD pcd)
    {
        var enc = pcd.Gift.PK;
        var ot = enc.OriginalTrainerTrash;
        if (!ot.SequenceEqual(pk.OriginalTrainerTrash))
            data.AddLine(GetInvalid(Format(OriginalTrainer, LTrashBytesMismatchInitial)));

        if (pcd.Species != pk.Species)
            return; // Evolved, trash bytes are rewritten.

        var nick = enc.NicknameTrash;
        if (!nick.SequenceEqual(pk.NicknameTrash))
            data.AddLine(GetInvalid(Format(Nickname, LTrashBytesMismatchInitial)));
    }

    private void VerifyTrashBytesHOME(LegalityAnalysis data, PKM pk)
    {
        if (!TrashBytesUTF16.IsFinalTerminatorPresent(pk.NicknameTrash))
            data.AddLine(GetInvalid(Format(Nickname, LTrashBytesMissingTerminator)));
        if (!TrashBytesUTF16.IsFinalTerminatorPresent(pk.OriginalTrainerTrash))
            data.AddLine(GetInvalid(Format(OriginalTrainer, LTrashBytesMissingTerminator)));
        if (!TrashBytesUTF16.IsFinalTerminatorPresent(pk.HandlingTrainerTrash))
            data.AddLine(GetInvalid(Format(HandlingTrainer, LTrashBytesMissingTerminator)));

        if (pk.IsEgg)
        {
            if (!pk.IsTradedEgg || pk.SWSH)
                VerifyTrashEmpty(data, pk.HandlingTrainerTrash, HandlingTrainer);
            else
                VerifyTrashNotEmpty(data, pk.HandlingTrainerTrash, HandlingTrainer);
            VerifyTrashNone(data, pk.OriginalTrainerTrash, OriginalTrainer);

            if (IsTrashCleanContext(pk.Context))
            {
                VerifyTrashNone(data, pk.NicknameTrash, Nickname);
                VerifyTrashNone(data, pk.HandlingTrainerTrash, HandlingTrainer);
            }
            else
            {
                // Species name is overwritten by "Egg"
                var origName = SpeciesName.GetSpeciesName(pk.Species, pk.Language);
                VerifyTrashSpecific(data, pk.NicknameTrash, origName, Nickname);
            }
            return;
        }

        VerifyTrashNickname(data, pk.NicknameTrash);
        var enc = data.Info.EncounterMatch;
        if (enc is EncounterEgg && pk.WasTradedEgg)
        {
            // Allow Traded eggs to have a single layer of OT trash bytes.
            VerifyTrashSingle(data, pk.OriginalTrainerTrash, OriginalTrainer);
            if (!pk.SWSH) // SW/SH does not update the HT data.
                VerifyTrashNotEmpty(data, pk.HandlingTrainerTrash, HandlingTrainer);
        }
        else if (enc is EncounterStatic8U { ShouldHaveScientistTrash: true })
        {
            var under = EncounterStatic8U.GetScientistName(pk.Language);
            VerifyTrashSpecific(data, pk.OriginalTrainerTrash, under, OriginalTrainer);
        }
        else
        {
            VerifyTrashNone(data, pk.OriginalTrainerTrash, OriginalTrainer);
        }
    }

    private static bool IsTrashCleanContext(EntityContext context)
    {
        return context is EntityContext.Gen8b;
    }

    private void VerifyTrashNickname(LegalityAnalysis data, ReadOnlySpan<byte> span)
    {
        var pk = data.Entity;
        if (pk.IsNicknamed)
        {
            var origName = SpeciesName.GetSpeciesName(pk.Species, pk.Language);
            VerifyTrashSpecific(data, span, origName, Nickname, Severity.Fishy);
        }
        else
        {
            VerifyTrashNone(data, span, Nickname, Severity.Fishy);
        }
    }

    private void VerifyTrashSingle(LegalityAnalysis data, ReadOnlySpan<byte> span, StringSource s)
    {
        var result = TrashBytesUTF16.IsTrashSingleOrNone(span);
        if (result.IsInvalid())
            data.AddLine(GetInvalid(Format(s, LTrashBytesShouldBeEmpty)));
    }

    private void VerifyTrashSpecific(LegalityAnalysis data, ReadOnlySpan<byte> span, ReadOnlySpan<char> under, StringSource s,
        Severity severity = Severity.Invalid)
    {
        var result = TrashBytesUTF16.IsTrashSpecific(span, under);
        if (result.IsInvalid())
            data.AddLine(Get(Format(s, string.Format(LTrashBytesExpected_0, under.ToString())), severity));
    }

    private void VerifyTrashNone(LegalityAnalysis data, ReadOnlySpan<byte> span, StringSource s,
        Severity severity = Severity.Invalid)
    {
        var result = TrashBytesUTF16.IsTrashNone(span);
        if (result.IsInvalid())
            data.AddLine(Get(Format(s, LTrashBytesShouldBeEmpty), severity));
    }

    private void VerifyTrashNotEmpty(LegalityAnalysis data, ReadOnlySpan<byte> span, StringSource s)
    {
        if (!TrashBytesUTF16.IsTrashNotEmpty(span))
            data.AddLine(GetInvalid(Format(s, LTrashBytesExpected)));
    }

    private void VerifyTrashEmpty(LegalityAnalysis data, ReadOnlySpan<byte> span, StringSource s)
    {
        if (!TrashBytesUTF16.IsTrashEmpty(span))
            data.AddLine(GetInvalid(Format(s, LTrashBytesShouldBeEmpty)));
    }
}

public enum StringSource : byte { Nickname, OriginalTrainer, HandlingTrainer }
