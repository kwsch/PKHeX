using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Verifies the trash bytes of various strings.
/// </summary>
public sealed class TrashByteVerifier : Verifier
{
    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.Format >= 8 || pk.Context == EntityContext.Gen7b)
            VerifyTrashBytesHOME(data, pk);
    }

    private void VerifyTrashBytesHOME(LegalityAnalysis data, PKM pk)
    {
        if (pk.IsEgg)
        {
            if (!pk.IsTradedEgg)
                VerifyTrashEmpty(data, pk.HandlingTrainerTrash);
            VerifyTrashNone(data, pk.OriginalTrainerTrash);

            // Species name is overwritten by "Egg"
            var origName = SpeciesName.GetSpeciesName(pk.Species, pk.Language);
            VerifyTrashSpecific(data, pk.NicknameTrash, origName);
            return;
        }

        VerifyTrashNickname(data, pk.NicknameTrash);
        var enc = data.Info.EncounterMatch;
        if (enc is EncounterEgg && pk.WasTradedEgg)
        {
            // Allow Traded eggs to have a single layer of OT trash bytes.
            VerifyTrashSingle(data, pk.OriginalTrainerTrash);
            VerifyTrashNotEmpty(data, pk.HandlingTrainerTrash);
        }
        else if (enc is EncounterStatic8U { ShouldHaveScientistTrash: true })
        {
            VerifyTrashSpecific(data, pk.OriginalTrainerTrash, EncounterStatic8U.GetScientistName(pk.Language));
        }
        else
        {
            VerifyTrashNone(data, pk.OriginalTrainerTrash);
        }
    }

    private void VerifyTrashNickname(LegalityAnalysis data, Span<byte> span,
        [CallerArgumentExpression(nameof(span))] string? name = null)
    {
        var pk = data.Entity;
        if (pk.IsNicknamed)
        {
            var origName = SpeciesName.GetSpeciesName(pk.Species, pk.Language);
            VerifyTrashSpecific(data, span, origName, Severity.Fishy, name);
        }
        else
        {
            VerifyTrashNone(data, span, Severity.Fishy, name);
        }
    }

    private void VerifyTrashSingle(LegalityAnalysis data, Span<byte> span,
        [CallerArgumentExpression(nameof(span))] string? name = null)
    {
        var pk = data.Entity;
        int start = (pk.GetStringTerminatorIndex(span) + 1) * pk.GetBytesPerChar();
        if (start >= span.Length)
            return;

        var remain = span[start..];
        var end = pk.GetStringTerminatorIndex(remain);
        if (end >= remain.Length)
            return;

        VerifyTrashEmpty(data, span, name);
    }

    private void VerifyTrashSpecific(LegalityAnalysis data, Span<byte> span, ReadOnlySpan<char> under,
        Severity severity = Severity.Invalid,
        [CallerArgumentExpression(nameof(span))] string? name = null)
    {
        var pk = data.Entity;
        var bpc = pk.GetBytesPerChar();
        int start = (pk.GetStringTerminatorIndex(span) + 1) * bpc;
        if (start >= span.Length)
            return;

        var input = MemoryMarshal.Cast<byte, char>(span);
        if (input.Length < under.Length)
        {
            for (int i = start; i < input.Length; i++)
            {
                var c = input[i];
                if (BitConverter.IsLittleEndian)
                    c = (char)ReverseEndianness(c);
                if (c == under[i])
                    continue;
                data.AddLine(Get(name!, severity));
                return;
            }
        }

        start = Math.Max(start, under.Length * bpc);
        if (span[start..].ContainsAnyExcept<byte>(0))
            data.AddLine(Get(name!, severity));
        else
            data.AddLine(GetValid(name!));
    }

    private void VerifyTrashNone(LegalityAnalysis data, Span<byte> span,
        Severity severity = Severity.Invalid,
        [CallerArgumentExpression(nameof(span))] string? name = null)
    {
        var pk = data.Entity;
        int start = (pk.GetStringTerminatorIndex(span) + 1) * pk.GetBytesPerChar();
        if (start >= span.Length)
            return;

        if (span[start..].ContainsAnyExcept<byte>(0))
            data.AddLine(Get(name!, severity));
        else
            data.AddLine(GetValid(name!));
    }

    private void VerifyTrashNotEmpty(LegalityAnalysis data, Span<byte> span,
        [CallerArgumentExpression(nameof(span))] string? name = null)
    {
        if (!span.ContainsAnyExcept<byte>(0))
            data.AddLine(GetInvalid(name!));
    }

    private void VerifyTrashEmpty(LegalityAnalysis data, Span<byte> span,
        [CallerArgumentExpression(nameof(span))] string? name = null)
    {
        if (span.ContainsAnyExcept<byte>(0))
            data.AddLine(GetInvalid(name!));
    }

    protected override CheckIdentifier Identifier => CheckIdentifier.TrashBytes;
}
