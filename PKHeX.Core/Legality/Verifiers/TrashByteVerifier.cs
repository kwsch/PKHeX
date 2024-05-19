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
            VerifyTrashBytesHOME(data, pk);
    }

    private void VerifyTrashBytesHOME(LegalityAnalysis data, PKM pk)
    {
        if (!IsFinalTerminatorPresent(pk.NicknameTrash))
            data.AddLine(GetInvalid(Format(Nickname, LTrashBytesMissingTerminator)));
        if (!IsFinalTerminatorPresent(pk.OriginalTrainerTrash))
            data.AddLine(GetInvalid(Format(OriginalTrainer, LTrashBytesMissingTerminator)));
        if (!IsFinalTerminatorPresent(pk.HandlingTrainerTrash))
            data.AddLine(GetInvalid(Format(HandlingTrainer, LTrashBytesMissingTerminator)));

        if (pk.IsEgg)
        {
            if (!pk.IsTradedEgg || pk.SWSH)
                VerifyTrashEmpty(data, pk.HandlingTrainerTrash, HandlingTrainer);
            else
                VerifyTrashNotEmpty(data, pk.HandlingTrainerTrash, HandlingTrainer);
            VerifyTrashNone(data, pk.OriginalTrainerTrash, OriginalTrainer);

            // Species name is overwritten by "Egg"
            var origName = SpeciesName.GetSpeciesName(pk.Species, pk.Language);
            VerifyTrashSpecific(data, pk.NicknameTrash, origName, Nickname);
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
        var result = IsTrashSingleOrNone(span, data.Entity);
        if (result.IsInvalid())
            data.AddLine(GetInvalid(Format(s, LTrashBytesShouldBeEmpty)));
    }

    private void VerifyTrashSpecific(LegalityAnalysis data, ReadOnlySpan<byte> span, ReadOnlySpan<char> under, StringSource s,
        Severity severity = Severity.Invalid)
    {
        var result = IsTrashSpecific(span, data.Entity, under);
        if (result.IsInvalid())
            data.AddLine(Get(Format(s, string.Format(LTrashBytesExpected_0, under.ToString())), severity));
    }

    private void VerifyTrashNone(LegalityAnalysis data, ReadOnlySpan<byte> span, StringSource s,
        Severity severity = Severity.Invalid)
    {
        var result = IsTrashNone(span, data.Entity);
        if (result.IsInvalid())
            data.AddLine(Get(Format(s, LTrashBytesShouldBeEmpty), severity));
    }

    private void VerifyTrashNotEmpty(LegalityAnalysis data, ReadOnlySpan<byte> span, StringSource s)
    {
        if (!IsTrashNotEmpty(span))
            data.AddLine(GetInvalid(Format(s, LTrashBytesExpected)));
    }

    private void VerifyTrashEmpty(LegalityAnalysis data, ReadOnlySpan<byte> span, StringSource s)
    {
        if (!IsTrashEmpty(span))
            data.AddLine(GetInvalid(Format(s, LTrashBytesShouldBeEmpty)));
    }

    private static bool IsTrashNotEmpty(ReadOnlySpan<byte> span) => span.ContainsAnyExcept<byte>(0) || span.Length == 0;
    private static bool IsTrashEmpty(ReadOnlySpan<byte> span) => !span.ContainsAnyExcept<byte>(0) || span.Length == 0;
    private static bool IsFinalTerminatorPresent(ReadOnlySpan<byte> buffer, byte terminator = 0)
        => buffer[^1] == terminator && buffer[^2] == terminator;

    private static TrashMatch IsTrashNone<T>(ReadOnlySpan<byte> span, T tr)
        where T : ITrashIntrospection
    {
        var bpc = tr.GetBytesPerChar();
        var charsUsed = tr.GetStringTerminatorIndex(span) + 1;
        var start = charsUsed * bpc;
        if ((uint)start >= span.Length)
            return TrashMatch.TooLongToTell;

        var remain = span[start..];
        if (!IsTrashEmpty(remain))
            return TrashMatch.NotEmpty;
        return TrashMatch.PresentNone;
    }

    private static TrashMatch IsTrashSingleOrNone<T>(ReadOnlySpan<byte> span, T tr)
        where T : ITrashIntrospection
    {
        var bpc = tr.GetBytesPerChar();
        var charsUsed = tr.GetStringTerminatorIndex(span) + 1;
        var start = charsUsed * bpc;
        if ((uint)start >= span.Length)
            return TrashMatch.TooLongToTell;

        var remain = span[start..];
        var end = tr.GetStringTerminatorIndex(remain) + 1;
        start = end * bpc;
        if ((uint)start < remain.Length && !IsTrashEmpty(remain[start..]))
            return TrashMatch.NotEmpty;

        return end == 1 ? TrashMatch.PresentNone : TrashMatch.PresentSingle;
    }

    private static TrashMatch IsTrashSpecific<T>(ReadOnlySpan<byte> span, T tr, ReadOnlySpan<char> under)
        where T : ITrashIntrospection
    {
        var bpc = tr.GetBytesPerChar();
        var charsUsed = tr.GetStringTerminatorIndex(span) + 1;
        var start = charsUsed * bpc;
        if (start >= span.Length)
            return TrashMatch.TooLongToTell;

        var check = TrashBytes.IsUnderlayerPresent(under, span, charsUsed);
        if (check.IsInvalid())
            return TrashMatch.NotPresent;

        start = Math.Max(start, under.Length * bpc);
        if ((uint)start < span.Length && !IsTrashEmpty(span[start..]))
            return TrashMatch.NotEmpty;

        return TrashMatch.Present;
    }
}

public enum StringSource : byte { Nickname, OriginalTrainer, HandlingTrainer }
