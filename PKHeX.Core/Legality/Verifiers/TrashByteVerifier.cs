using System;
using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.StringSource;

namespace PKHeX.Core;

/// <summary>
/// Verifies the trash bytes of various strings.
/// </summary>
public sealed class TrashByteVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.TrashBytes;

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

    private static void VerifyTrashBytesPalPark(LegalityAnalysis data, PKM pk)
    {
        if (pk.Japanese)
        {
            // Trash bytes should be zero.
            if (!TrashBytesUTF16.IsTrashEmpty(pk.OriginalTrainerTrash))
                data.AddLine(GetInvalid(CheckIdentifier.Trainer, TrashBytesShouldBeEmpty));
        }
        else
        {
            // Should have trash bytes from the transfer process.
            if (TrashBytesUTF16.IsTrashEmpty(pk.OriginalTrainerTrash))
                data.AddLine(GetInvalid(CheckIdentifier.Trainer, TrashBytesExpected));
        }
    }

    private void VerifyTrashBytesPCD(LegalityAnalysis data, PKM pk, PCD pcd)
    {
        var enc = pcd.Gift.PK;
        var ot = enc.OriginalTrainerTrash;
        if (!ot.SequenceEqual(pk.OriginalTrainerTrash))
            data.AddLine(GetInvalid(CheckIdentifier.Trainer, TrashBytesMismatchInitial));

        if (pcd.Species != pk.Species)
            return; // Evolved, trash bytes are rewritten.

        var nick = enc.NicknameTrash;
        if (!nick.SequenceEqual(pk.NicknameTrash))
            data.AddLine(GetInvalid(CheckIdentifier.Nickname, TrashBytesMismatchInitial));
    }

    private void VerifyTrashBytesHOME(LegalityAnalysis data, PKM pk)
    {
        if (!TrashBytesUTF16.IsFinalTerminatorPresent(pk.NicknameTrash))
            data.AddLine(GetInvalid(CheckIdentifier.Nickname, TrashBytesMissingTerminator));
        if (!TrashBytesUTF16.IsFinalTerminatorPresent(pk.OriginalTrainerTrash))
            data.AddLine(GetInvalid(CheckIdentifier.Trainer, TrashBytesMissingTerminator));
        if (!TrashBytesUTF16.IsFinalTerminatorPresent(pk.HandlingTrainerTrash))
            data.AddLine(GetInvalid(CheckIdentifier.Handler, TrashBytesMissingTerminator));

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
                var origName = SpeciesName.GetSpeciesNameGeneration(pk.Species, pk.Language, pk.Format);
                VerifyTrashSpecific(data, pk.NicknameTrash, origName, Nickname);
            }
            return;
        }

        VerifyTrashNickname(data, pk.NicknameTrash);
        var enc = data.Info.EncounterMatch;
        if (enc is IEncounterEgg && pk.WasTradedEgg)
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
            var origName = SpeciesName.GetSpeciesNameGeneration(pk.Species, pk.Language, pk.Format);
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
            data.AddLine(GetInvalid(GetIdentifier(s), TrashBytesShouldBeEmpty));
    }

    private void VerifyTrashSpecific(LegalityAnalysis data, ReadOnlySpan<byte> span, ReadOnlySpan<char> under, StringSource s, Severity severity = Severity.Invalid)
    {
        var result = TrashBytesUTF16.IsTrashSpecific(span, under);
        if (result.IsInvalid())
            data.AddLine(Get(GetIdentifier(s), severity, TrashBytesExpected));
    }

    private void VerifyTrashNone(LegalityAnalysis data, ReadOnlySpan<byte> span, StringSource s,
        Severity severity = Severity.Invalid)
    {
        var result = TrashBytesUTF16.IsTrashNone(span);
        if (result.IsInvalid())
            data.AddLine(Get(GetIdentifier(s), severity, TrashBytesShouldBeEmpty));
    }

    private void VerifyTrashNotEmpty(LegalityAnalysis data, ReadOnlySpan<byte> span, StringSource s)
    {
        if (!TrashBytesUTF16.IsTrashNotEmpty(span))
            data.AddLine(GetInvalid(GetIdentifier(s), TrashBytesExpected));
    }

    private void VerifyTrashEmpty(LegalityAnalysis data, ReadOnlySpan<byte> span, StringSource s)
    {
        if (!TrashBytesUTF16.IsTrashEmpty(span))
            data.AddLine(GetInvalid(GetIdentifier(s), TrashBytesShouldBeEmpty));
    }

    private static CheckIdentifier GetIdentifier(StringSource s) => s switch
    {
        Nickname => CheckIdentifier.Nickname,
        OriginalTrainer => CheckIdentifier.Trainer,
        HandlingTrainer => CheckIdentifier.Handler,
        _ => throw new ArgumentOutOfRangeException(nameof(s)),
    };
}

public enum StringSource : byte { Nickname, OriginalTrainer, HandlingTrainer }
